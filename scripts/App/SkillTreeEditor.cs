using System;
using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Content;
using Everkin.Core.Skills;
using Godot;

namespace Everkin.App;

/// <summary>One branch drawn as a tree: the root at the top, lines from each parent to its children; learned nodes and their lines are gold, nodes whose parent is missing are dim.</summary>
public partial class BranchView : Control
{
    private readonly List<(ProfessionSkillDef Def, Vector2 Pos)> _nodes = new();
    private readonly SkillBuild _build;
    private readonly Action<string> _clicked;
    private readonly Dictionary<string, Button> _buttons = new();
    private static readonly Vector2 NodeSize = new(150, 56);

    public BranchView(IReadOnlyList<ProfessionSkillDef> nodes, SkillBuild build, Action<string> clicked)
    {
        _build = build;
        _clicked = clicked;
        var byParent = nodes.Where(n => n.Parent != null && nodes.Any(p => p.Id == n.Parent)).GroupBy(n => n.Parent!).ToDictionary(g => g.Key, g => g.OrderBy(n => Game.Trees.OrderOf(n.Id)).ToList());
        var roots = nodes.Where(n => n.Parent == null || nodes.All(p => p.Id != n.Parent)).OrderBy(n => Game.Trees.OrderOf(n.Id)).ToList();
        var leaf = 0f;
        float Place(ProfessionSkillDef n, int depth)
        {
            var kids = byParent.GetValueOrDefault(n.Id) ?? new List<ProfessionSkillDef>();
            float x;
            if (kids.Count == 0) x = leaf++;
            else { var xs = kids.Select(k => Place(k, depth + 1)).ToList(); x = (xs.Min() + xs.Max()) / 2; }
            _nodes.Add((n, new Vector2(x * (NodeSize.X + 14) + 8, depth * 92 + 8)));
            return x;
        }
        foreach (var r in roots) Place(r, 0);
        var maxX = _nodes.Max(n => n.Pos.X) + NodeSize.X + 16;
        var maxY = _nodes.Max(n => n.Pos.Y) + NodeSize.Y + 16;
        CustomMinimumSize = new Vector2(maxX, maxY);
        foreach (var (def, pos) in _nodes)
        {
            var b = Ui.Button(def.Name + (def.Kind == "K" ? " ★" : def.Kind == "P" ? " ·P" : def.Kind == "Mod" ? " ·M" : ""), 17);
            b.Position = pos;
            b.Size = NodeSize;
            b.ClipText = true;
            var id = def.Id;
            b.Pressed += () => _clicked(id);
            AddChild(b);
            _buttons[def.Id] = b;
        }
        Refresh();
    }

    public void Refresh()
    {
        foreach (var (def, _) in _nodes)
        {
            var b = _buttons[def.Id];
            var learned = _build.Has(def.Id);
            var blocked = !learned && def.Parent != null && !_build.Has(def.Parent);
            b.AddThemeStyleboxOverride("normal", learned ? Ui.Box(new Color("4a3a1c"), Ui.Gold, 3) : blocked ? Ui.Box(new Color("15110d"), new Color("5a4a36"), 2) : Ui.Box(new Color("2a2118"), Ui.Bronze, 2));
            b.AddThemeColorOverride("font_color", blocked ? new Color("8a7c64") : Ui.Ink);
        }
        QueueRedraw();
    }

    public override void _Draw()
    {
        foreach (var (def, pos) in _nodes)
        {
            if (def.Parent == null) continue;
            var parent = _nodes.FirstOrDefault(n => n.Def.Id == def.Parent);
            if (parent.Def == null) continue;
            var from = parent.Pos + new Vector2(NodeSize.X / 2, NodeSize.Y);
            var to = pos + new Vector2(NodeSize.X / 2, 0);
            var mid = from.Y + (to.Y - from.Y) / 2;
            var color = _build.Has(def.Id) ? Ui.Gold : new Color("5a4a36");
            DrawPolyline(new[] { from, new Vector2(from.X, mid), new Vector2(to.X, mid), to }, color, _build.Has(def.Id) ? 3 : 2);
        }
    }
}

/// <summary>
/// The skill tree editor (M4.3, ui_spec "Skill tree editor"): the card back on the left, one panel per branch in the middle, the last clicked skill
/// on the right. Clicking a node learns it, clicking it again unlearns it with everything below it; there is no Learn button. Changes save at once.
/// </summary>
public partial class SkillTreeEditor : Control, IBackNavigable
{
    public bool GoBack() { _back(); return true; }

    private readonly Main _main;
    private readonly CreatureDef _def;
    private readonly Action _back;
    private SkillBuild _build = null!;
    private readonly List<BranchView> _branches = new();
    private readonly HBoxContainer _pips = new();
    private readonly Label _pointsLeft = Ui.Label("", 22, Ui.Gold, bold: true);
    private readonly CardView _card = new() { Mode = CardMode.Back };
    private readonly VBoxContainer _detail = new();
    private string _note = "";

    public SkillTreeEditor(Main main, string creatureId, Action back)
    {
        _main = main;
        _def = Game.Content.CreaturesById[creatureId];
        _back = back;
    }

    public override void _Ready()
    {
        _build = new SkillBuild(Game.Trees, _def.Professions, Profile.BuildOf(_def.Id));
        var bg = new ColorRect { Color = new Color("0d0a08") };
        AddChild(bg);
        Ui.Fill(bg);
        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 10);
        AddChild(root);
        Ui.Fill(root);
        root.OffsetLeft = 20; root.OffsetTop = 12; root.OffsetRight = -20; root.OffsetBottom = -12;

        var info = CardInfo.FromCreature(_def);
        var top = new HBoxContainer();
        top.AddThemeConstantOverride("separation", 14);
        var back = Ui.Button("Back", 22);
        back.Pressed += _back;
        top.AddChild(back);
        top.AddChild(Ui.Label(info.Name, 32, Ui.Gold, bold: true));
        top.AddChild(Ui.Label(info.ClassLine, 22, new Color("cdbd9a")));
        top.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        top.AddChild(_pips);
        top.AddChild(_pointsLeft);
        var defaults = Ui.Button("Default build", 20);
        defaults.Pressed += () => { _build = new SkillBuild(Game.Trees, _def.Professions, Game.Content.DefaultBuilds.GetValueOrDefault(_def.Id) ?? new List<string>()); Changed("Default build restored."); };
        var reset = Ui.Button("Reset", 20);
        reset.Pressed += () => { _build.Reset(); Changed("All skills unlearned."); };
        top.AddChild(defaults);
        top.AddChild(reset);
        root.AddChild(Ui.Panel(top));

        var body = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        body.AddThemeConstantOverride("separation", 14);
        root.AddChild(body);
        _card.CustomMinimumSize = new Vector2(330, 495);
        body.AddChild(new VBoxContainer().WithChild(_card));

        var middle = new VBoxContainer();
        middle.AddThemeConstantOverride("separation", 10);
        foreach (var prof in _def.Professions.Select(p => Game.Content.Professions.First(x => x.Id == p)))
        {
            middle.AddChild(Ui.Label(prof.Name, 28, Ui.Gold, bold: true));
            var branchRow = new HBoxContainer();
            branchRow.AddThemeConstantOverride("separation", 14);
            foreach (var branch in prof.Branches)
            {
                var nodes = Game.Trees.NodesOf(new[] { prof.Id }).Where(n => n.Branch == branch).ToList();
                if (nodes.Count == 0) continue;
                var view = new BranchView(nodes, _build, OnNode);
                _branches.Add(view);
                var column = new VBoxContainer();
                column.AddChild(Ui.Label(branch, 22, Ui.Ink, bold: true));
                column.AddChild(view);
                branchRow.AddChild(Ui.Panel(column));
            }
            middle.AddChild(branchRow);
        }
        var scroll = new ScrollContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        scroll.AddChild(middle);
        body.AddChild(scroll);

        _detail.CustomMinimumSize = new Vector2(380, 0);
        _detail.AddThemeConstantOverride("separation", 6);
        var detailPanel = Ui.Panel(_detail);
        detailPanel.CustomMinimumSize = new Vector2(400, 0);
        detailPanel.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
        body.AddChild(detailPanel);
        Refresh();
    }

    private void OnNode(string id)
    {
        var def = Game.Trees.Node(id);
        if (_build.Has(id))
        {
            var removed = _build.Unlearn(id);
            _note = removed.Count > 1 ? $"Unlearned {def.Name} and {removed.Count - 1} skill(s) below it." : $"Unlearned {def.Name}.";
        }
        else if (_build.WhyNot(id) is { } why) _note = why;
        else
        {
            _build.Learn(id);
            _note = $"Learned {def.Name}.";
        }
        Show(def);
        Profile.SetBuild(_def.Id, _build.Learned);
        Refresh();
    }

    private void Changed(string note)
    {
        _note = note;
        Profile.SetBuild(_def.Id, _build.Learned);
        // The branch views read the build they were made with: rebuild the screen.
        _main.Open(new SkillTreeEditor(_main, _def.Id, _back));
    }

    private void Refresh()
    {
        foreach (var b in _branches) b.Refresh();
        foreach (var c in _pips.GetChildren()) c.QueueFree();
        for (var i = 0; i < SkillBuild.Points; i++)
            _pips.AddChild(Ui.Label(i < _build.PointsSpent ? "●" : "○", 26, Ui.Gold));
        _pointsLeft.Text = $"{_build.PointsLeft} left";
        var info = CardInfo.FromCreature(_def, Game.CardSkills(_build.Learned.Prepend(Game.Catalog.AttackSkillFor(_def.AttackType))));
        _card.Bind(info);
        if (_detail.GetChildCount() == 0 || !string.IsNullOrEmpty(_note)) ShowNote();
    }

    private void Show(ProfessionSkillDef def)
    {
        foreach (var c in _detail.GetChildren()) c.QueueFree();
        _detail.AddChild(Ui.Label(def.Name, 30, Ui.Gold, bold: true));
        var kind = def.Kind switch { "A" => "Active", "P" => "Passive", "K" => "Keystone", "Mod" => $"Modifier of {Game.SkillName(def.ModifierOf ?? "")}", _ => def.Kind };
        _detail.AddChild(Ui.Label(kind + (def.Placement != null ? $" · {def.Placement}" : ""), 20, new Color("cdbd9a")));
        var line = new[] { def.Delay is { } d ? $"Delay {d}" : null, def.Momentum is { } m ? $"Momentum {m}" : null, def.Delivery, def.Range }.Where(x => !string.IsNullOrWhiteSpace(x));
        _detail.AddChild(Ui.Label(string.Join(" · ", line), 20));
        var effect = Ui.Label(def.Effect, 20);
        effect.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        effect.CustomMinimumSize = new Vector2(340, 0);
        _detail.AddChild(effect);
        _detail.AddChild(Ui.Label($"(numbers are for Power 5; this creature has Power {_def.Power})", 16, new Color("7d725f")));
        ShowNote();
    }

    private void ShowNote()
    {
        if (string.IsNullOrEmpty(_note)) return;
        var note = Ui.Label(_note, 20, Ui.Gold);
        note.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        note.CustomMinimumSize = new Vector2(340, 0);
        _detail.AddChild(note);
    }
}
