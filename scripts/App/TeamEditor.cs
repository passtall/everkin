using System;
using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;
using Godot;

namespace Everkin.App;

/// <summary>A card that can be dragged: from the collection into a row, between rows, or out of the team (ui_spec "Party setup").</summary>
public partial class DragCard : CardView
{
    public string CreatureId { get; init; } = "";
    public bool InTeam { get; init; }
    public bool Draggable { get; init; } = true;

    public DragCard() => ClickOnRelease = true;

    public override Variant _GetDragData(Vector2 atPosition)
    {
        if (!Draggable) return default;
        var preview = new CardView { Mode = CardMode.Compact, Size = new Vector2(100, 150), Modulate = new Color(1, 1, 1, 0.85f) };
        preview.Bind(Info);
        var holder = new Control();
        holder.AddChild(preview);
        preview.Position = -preview.Size / 2;
        SetDragPreview(holder);
        var data = new Godot.Collections.Dictionary { ["creature"] = CreatureId, ["fromTeam"] = InTeam };
        return data;
    }
}

/// <summary>One row of the formation on the left: cards in order, dropping a creature inserts it where the pointer is.</summary>
public partial class RowDrop : Control
{
    private readonly TeamEditor _editor;
    public Row Row { get; }
    private readonly HBoxContainer _cards = new();

    public RowDrop(TeamEditor editor, Row row)
    {
        _editor = editor;
        Row = row;
        CustomMinimumSize = new Vector2(0, 190);
        _cards.Alignment = BoxContainer.AlignmentMode.Center;
        _cards.AddThemeConstantOverride("separation", 10);
        _cards.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(_cards);
        Ui.Fill(_cards);
    }

    public HBoxContainer Cards => _cards;

    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, Size), new Color(0, 0, 0, 0.25f));
        DrawRect(new Rect2(Vector2.Zero, Size), new Color(Ui.Bronze, 0.8f), false, 2);
        DrawString(Game.BodyBold(), new Vector2(10, 22), Row.ToString(), HorizontalAlignment.Left, -1, 18, new Color(Ui.Gold, 0.7f));
    }

    private int IndexAt(Vector2 at)
    {
        var index = 0;
        foreach (var c in _cards.GetChildren().OfType<Control>())
            if (c.Position.X + c.Size.X / 2 < at.X) index++;
        return index;
    }

    public override bool _CanDropData(Vector2 atPosition, Variant data) => data.VariantType == Variant.Type.Dictionary && _editor.CanPlace(data.AsGodotDictionary()["creature"].AsString(), Row, out _);

    public override void _DropData(Vector2 atPosition, Variant data) =>
        _editor.Place(data.AsGodotDictionary()["creature"].AsString(), Row, IndexAt(atPosition));
}

/// <summary>The collection panel; dropping a team member on it takes that creature out of the team.</summary>
public partial class CollectionDrop : PanelContainer
{
    private readonly TeamEditor _editor;

    public CollectionDrop(TeamEditor editor) => _editor = editor;

    public override bool _CanDropData(Vector2 atPosition, Variant data) => data.VariantType == Variant.Type.Dictionary && data.AsGodotDictionary()["fromTeam"].AsBool();

    public override void _DropData(Vector2 atPosition, Variant data) => _editor.Remove(data.AsGodotDictionary()["creature"].AsString());
}

/// <summary>
/// The team editor in the manner of a deck builder (M4.2, ui_spec "Party setup"): saved teams as chips, the formation on the left, the collection on
/// the right with filters and sorting. Creatures are added and moved by dragging; every change is saved at once.
/// </summary>
public partial class TeamEditor : Control, IBackNavigable
{
    public bool GoBack() { if (Back != null) Back(); else _main.Show("menu"); return true; }

    private const int PageSize = 15;
    private readonly Main _main;
    private readonly HBoxContainer _chips = new();
    private readonly Label _cost = Ui.Label("", 24, Ui.Gold, bold: true);
    private readonly Label _message = Ui.Label("", 20, Ui.Bad);
    private readonly Dictionary<Row, RowDrop> _rows = new();
    private readonly GridContainer _grid = new() { Columns = 5 };
    private readonly Label _page = Ui.Label("", 20);
    private OptionButton _attack = null!, _profession = null!, _sort = null!, _owned = null!;

    /// <summary>Where Back leads (the campaign map when opened from it); the main menu by default.</summary>
    public Action? Back { get; init; }

    /// <summary>The tutorial's automatic team is being edited (the one team the tutorial uses).</summary>
    public bool TutorialTeam { get; init; }
    private int _pageIndex;
    private InspectionLite? _inspection;

    public TeamEditor(Main main) => _main = main;

    private TeamData Team => TutorialTeam ? Profile.TutorialTeam() : Profile.CurrentTeam;

    public override void _Ready()
    {
        var bg = new ColorRect { Color = new Color("0d0a08") };
        AddChild(bg);
        Ui.Fill(bg);

        var root = new VBoxContainer();
        root.AddThemeConstantOverride("separation", 10);
        AddChild(root);
        Ui.Fill(root);
        root.OffsetLeft = 20; root.OffsetTop = 12; root.OffsetRight = -20; root.OffsetBottom = -12;

        var top = new HBoxContainer();
        top.AddThemeConstantOverride("separation", 10);
        var back = Ui.Button("Back", 22);
        back.Pressed += () => { if (Back != null) Back(); else _main.Show("menu"); };
        top.AddChild(back);
        if (!TutorialTeam) top.AddChild(_chips);
        top.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        top.AddChild(_message);
        top.AddChild(_cost);
        root.AddChild(Ui.Panel(top));

        var body = new HBoxContainer { SizeFlagsVertical = SizeFlags.ExpandFill };
        body.AddThemeConstantOverride("separation", 14);
        root.AddChild(body);

        var left = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsStretchRatio = 1f };
        left.AddThemeConstantOverride("separation", 10);
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
        {
            var drop = new RowDrop(this, row) { SizeFlagsVertical = SizeFlags.ExpandFill };
            _rows[row] = drop;
            left.AddChild(drop);
        }
        body.AddChild(left);

        var right = new CollectionDrop(this) { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsStretchRatio = 1.5f };
        right.AddThemeStyleboxOverride("panel", Ui.Box(new Color(Ui.PanelDark, 0.95f), Ui.Gold));
        var rightBox = new VBoxContainer();
        rightBox.AddThemeConstantOverride("separation", 8);
        var filters = new HBoxContainer();
        filters.AddThemeConstantOverride("separation", 8);
        _attack = Options(filters, "All attack types", Game.Content.AttackTypes.Select(a => (a.Id, a.Name)));
        _profession = Options(filters, "All professions", Game.Content.Professions.Select(p => (p.Id, p.Name)));
        _sort = Options(filters, null, new[] { ("name", "Name"), ("hp", "HP"), ("speed", "Speed"), ("power", "Power"), ("cost", "Cost") });
        _owned = Options(filters, null, new[] { ("owned", "Owned"), ("not", "Not owned") });
        rightBox.AddChild(filters);
        _grid.AddThemeConstantOverride("h_separation", 8);
        _grid.AddThemeConstantOverride("v_separation", 8);
        rightBox.AddChild(new ScrollContainer { SizeFlagsVertical = SizeFlags.ExpandFill, HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled }.WithChild(_grid));
        var pager = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        var prev = Ui.Button("<", 20);
        var next = Ui.Button(">", 20);
        prev.Pressed += () => { _pageIndex = Math.Max(0, _pageIndex - 1); RebuildCollection(); };
        next.Pressed += () => { _pageIndex++; RebuildCollection(); };
        pager.AddChild(prev);
        pager.AddChild(_page);
        pager.AddChild(next);
        rightBox.AddChild(pager);
        right.AddChild(rightBox);
        body.AddChild(right);

        Rebuild();
    }

    private OptionButton Options(Control parent, string? all, IEnumerable<(string Id, string Name)> items)
    {
        var o = new OptionButton();
        o.AddThemeFontSizeOverride("font_size", 18);
        if (all != null) o.AddItem(all);
        var ids = new List<string>();
        foreach (var (id, name) in items) { o.AddItem(name); ids.Add(id); }
        o.SetMeta("ids", string.Join(",", ids));
        o.ItemSelected += _ => { _pageIndex = 0; RebuildCollection(); };
        parent.AddChild(o);
        return o;
    }

    private static string? Selected(OptionButton o, bool hasAll)
    {
        var ids = o.GetMeta("ids").AsString().Split(',');
        var index = o.Selected - (hasAll ? 1 : 0);
        return index >= 0 && index < ids.Length ? ids[index] : null;
    }

    // ---------------------------------------------------------------- team operations

    /// <summary>Whether the creature may be put in a row now: at most six, each once, the total Cost within the budget, the row not full.</summary>
    public bool CanPlace(string creature, Row row, out string? why)
    {
        why = null;
        var existing = Team.Units.FirstOrDefault(u => u.Creature == creature);
        var others = Team.Units.Count(u => u.Creature != creature);
        if (!Profile.Owns(creature)) why = "You do not own this creature yet";
        else if (others >= BattleSetup.TeamSize) why = "A team holds up to six creatures";
        else if (Team.Units.Count(u => u.Row == row.ToString() && u.Creature != creature) >= Formation.RowCapacity) why = "That row is full";
        else if (Profile.Cost(Team) - (existing != null ? Game.Content.CreaturesById[creature].Cost : 0) + Game.Content.CreaturesById[creature].Cost > BattleSetup.TeamBudget)
            why = $"Over the team budget of {BattleSetup.TeamBudget}";
        return why == null;
    }

    public void Place(string creature, Row row, int index)
    {
        if (!CanPlace(creature, row, out var why)) { Say(why); return; }
        Team.Units.RemoveAll(u => u.Creature == creature);
        var inRow = Team.Units.Where(u => u.Row == row.ToString()).ToList();
        index = Math.Clamp(index, 0, inRow.Count);
        var unit = new TeamUnit { Creature = creature, Row = row.ToString() };
        if (index >= inRow.Count) Team.Units.Insert(inRow.Count == 0 ? Team.Units.Count : Team.Units.IndexOf(inRow[^1]) + 1, unit);
        else Team.Units.Insert(Team.Units.IndexOf(inRow[index]), unit);
        Profile.Save();
        Say("");
        Rebuild();
    }

    public void Remove(string creature)
    {
        Team.Units.RemoveAll(u => u.Creature == creature);
        Profile.Save();
        Rebuild();
    }

    private void Say(string? text) => _message.Text = text ?? "";

    // ---------------------------------------------------------------- drawing the editor

    private void Rebuild()
    {
        RebuildChips();
        foreach (var (row, drop) in _rows)
        {
            foreach (var c in drop.Cards.GetChildren()) c.QueueFree();
            foreach (var u in Team.Units.Where(u => u.Row == row.ToString()))
            {
                if (!Game.Content.CreaturesById.TryGetValue(u.Creature, out var def)) continue;
                drop.Cards.AddChild(MakeCard(def, CardMode.Front, new Vector2(120, 180), inTeam: true));
            }
            drop.QueueRedraw();
        }
        var cost = Profile.Cost(Team);
        _cost.Text = $"Cost {cost} / {BattleSetup.TeamBudget}" + (Team.Units.Count < BattleSetup.TeamSize ? $"   ⚠ {Team.Units.Count}/6" : "");
        RebuildCollection();
    }

    private DragCard MakeCard(CreatureDef def, CardMode mode, Vector2 size, bool inTeam, bool draggable = true)
    {
        var card = new DragCard { CreatureId = def.Id, InTeam = inTeam, Draggable = draggable, Mode = mode, CustomMinimumSize = size, Size = size, SizeFlagsHorizontal = SizeFlags.ShrinkCenter, SizeFlagsVertical = SizeFlags.ShrinkCenter, TooltipText = $"{def.Name}\n{CardInfo.FromCreature(def).ClassLine}\nHP {def.Hp}  Power {def.Power}  Speed {def.Speed}  Cost {def.Cost}" };
        card.Bind(CardInfo.FromCreature(def));
        card.Clicked += c => _main.Open(new SkillTreeEditor(_main, ((DragCard)c).CreatureId, () => _main.Open(new TeamEditor(_main) { Back = Back, TutorialTeam = TutorialTeam })));
        card.RightClicked += c => Inspect(((DragCard)c).CreatureId);
        return card;
    }

    private void RebuildChips()
    {
        foreach (var c in _chips.GetChildren()) c.QueueFree();
        for (var i = 0; i < Profile.Data.Teams.Count; i++)
        {
            var index = i;
            var t = Profile.Data.Teams[i];
            var chip = Ui.Button((t.Units.Count < BattleSetup.TeamSize ? "⚠ " : "") + t.Name, 20);
            if (i == Profile.Data.Selected) chip.AddThemeStyleboxOverride("normal", Ui.Box(new Color("4a3a24"), Ui.Gold, 3));
            chip.Pressed += () =>
            {
                if (Profile.Data.Selected == index) ChipMenu(chip);
                else { Profile.Data.Selected = index; Profile.Save(); Rebuild(); }
            };
            _chips.AddChild(chip);
        }
        var add = Ui.Button("+", 20);
        add.Pressed += () =>
        {
            Profile.Data.Teams.Add(new TeamData { Name = $"Team {Profile.Data.Teams.Count + 1}" });
            Profile.Data.Selected = Profile.Data.Teams.Count - 1;
            Profile.Save();
            Rebuild();
        };
        _chips.AddChild(add);
    }

    private void ChipMenu(Control at)
    {
        var menu = new PopupMenu();
        menu.AddItem("Rename", 0);
        menu.AddItem("Copy", 1);
        menu.AddItem("Delete", 2);
        menu.IdPressed += id =>
        {
            switch (id)
            {
                case 0: Rename(); break;
                case 1:
                    Profile.Data.Teams.Add(new TeamData { Name = Team.Name + " copy", Units = Team.Units.Select(u => new TeamUnit { Creature = u.Creature, Row = u.Row }).ToList() });
                    Profile.Data.Selected = Profile.Data.Teams.Count - 1;
                    Profile.Save();
                    Rebuild();
                    break;
                case 2: ConfirmDelete(); break;
            }
        };
        AddChild(menu);
        menu.Position = (Vector2I)at.GetGlobalRect().End;
        menu.Popup();
    }

    private void Rename()
    {
        var dialog = new ConfirmationDialog { Title = "Rename team" };
        var edit = new LineEdit { Text = Team.Name, CustomMinimumSize = new Vector2(300, 0) };
        dialog.AddChild(edit);
        dialog.Confirmed += () =>
        {
            if (!string.IsNullOrWhiteSpace(edit.Text)) Team.Name = edit.Text.Trim();
            Profile.Save();
            Rebuild();
        };
        AddChild(dialog);
        dialog.PopupCentered(new Vector2I(360, 120));
    }

    private void ConfirmDelete()
    {
        var dialog = new ConfirmationDialog { DialogText = $"Delete {Team.Name}?", OkButtonText = "Delete" };
        dialog.Confirmed += () =>
        {
            Profile.Data.Teams.RemoveAt(Profile.Data.Selected);
            Profile.Data.Selected = 0;
            Profile.Save();
            Rebuild();
        };
        AddChild(dialog);
        dialog.PopupCentered();
    }

    private void RebuildCollection()
    {
        foreach (var c in _grid.GetChildren()) c.QueueFree();
        var notOwned = Selected(_owned, false) == "not";
        IEnumerable<CreatureDef> list = Game.Content.Creatures.Where(c => Profile.Owns(c.Id) != notOwned);
        if (Selected(_attack, true) is { } at) list = list.Where(c => c.AttackType == at);
        if (Selected(_profession, true) is { } pr) list = list.Where(c => c.Professions.Contains(pr));
        list = Selected(_sort, false) switch
        {
            "hp" => list.OrderByDescending(c => c.Hp).ThenBy(c => c.Name),
            "speed" => list.OrderByDescending(c => c.Speed).ThenBy(c => c.Name),
            "power" => list.OrderByDescending(c => c.Power).ThenBy(c => c.Name),
            "cost" => list.OrderBy(c => c.Cost).ThenBy(c => c.Name),
            _ => list.OrderBy(c => c.Name),
        };
        var all = list.ToList();
        var pages = Math.Max(1, (all.Count + PageSize - 1) / PageSize);
        _pageIndex = Math.Clamp(_pageIndex, 0, pages - 1);
        _page.Text = $"{_pageIndex + 1} / {pages}   ({all.Count} creatures)";
        foreach (var def in all.Skip(_pageIndex * PageSize).Take(PageSize))
        {
            var inTeam = Team.Units.Any(u => u.Creature == def.Id);
            var card = MakeCard(def, CardMode.Front, new Vector2(140, 210), inTeam: false, draggable: !notOwned);
            if (inTeam || notOwned) card.Highlight = CardHighlight.Illegal;
            if (notOwned) card.TooltipText = def.Name + ", " + Profile.SourceOf(def.Id);
            _grid.AddChild(card);
        }
    }

    private void Inspect(string creatureId)
    {
        _inspection?.QueueFree();
        _inspection = new InspectionLite(CardInfo.FromCreature(Game.Content.CreaturesById[creatureId], Game.CardSkills(Profile.BuildOf(creatureId).Prepend(Game.Catalog.AttackSkillFor(Game.Content.CreaturesById[creatureId].AttackType)))));
        AddChild(_inspection);
        Ui.Fill(_inspection);
    }
}

/// <summary>A card's front and back side by side over the screen, closed by a click (the inspection outside a battle).</summary>
public partial class InspectionLite : Control
{
    private readonly CardInfo _info;

    public InspectionLite(CardInfo info) => _info = info;

    public override void _Ready()
    {
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.6f), MouseFilter = MouseFilterEnum.Stop };
        AddChild(dim);
        Ui.Fill(dim);
        dim.GuiInput += e => { if (e is InputEventMouseButton { Pressed: true }) QueueFree(); };
        var row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center, MouseFilter = MouseFilterEnum.Ignore };
        row.AddThemeConstantOverride("separation", 24);
        var h = Math.Min(Size.Y * 0.85f, 780f);
        foreach (var mode in new[] { CardMode.Front, CardMode.Back })
        {
            var card = new CardView { Mode = mode, CustomMinimumSize = new Vector2(h / 1.5f, h), MouseFilter = MouseFilterEnum.Ignore };
            card.Bind(_info);
            row.AddChild(card);
        }
        var center = new CenterContainer { MouseFilter = MouseFilterEnum.Ignore };
        center.AddChild(row);
        AddChild(center);
        Ui.Fill(center);
    }
}

internal static class ControlExtensions
{
    public static T WithChild<T>(this T parent, Control child) where T : Control
    {
        parent.AddChild(child);
        return parent;
    }
}
