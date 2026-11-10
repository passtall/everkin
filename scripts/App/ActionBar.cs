using System;
using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Battle;
using Everkin.Core.Skills;
using Godot;

namespace Everkin.App;

/// <summary>One slot of the MMO-style action bar (ui_spec "Battle screen"): the skill's icon in a bronze rim, its name below, a key number, Delay and Momentum gems.</summary>
public partial class SkillSlot : Control
{
    public string Id { get; init; } = "";
    public string Title { get; init; } = "";
    public string IconPath { get; init; } = "";
    public int Key { get; init; }
    public int Delay { get; init; }
    public int Momentum { get; init; }
    public bool Round { get; init; }
    public bool Selected { get; set; }
    public string? DisabledReason { get; set; }
    public event Action<SkillSlot>? Pressed;

    public bool Enabled => DisabledReason == null;

    public SkillSlot() => MouseFilter = MouseFilterEnum.Stop;

    public override void _GuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) Pressed?.Invoke(this);
    }

    public override string _GetTooltip(Vector2 atPosition) => DisabledReason == null ? Title : $"{Title}\n{DisabledReason}";

    public override void _Draw()
    {
        var box = Math.Min(Size.X, Size.Y - 30);
        var rect = new Rect2((Size.X - box) / 2, 0, box, box);
        var tint = Enabled ? Colors.White : new Color(0.4f, 0.4f, 0.4f, 0.8f);
        if (Round) DrawCircle(rect.GetCenter(), box / 2, new Color("2a2118"));
        if (Game.Tex(IconPath) is { } icon) DrawTextureRect(icon, rect.Grow(-4), false, tint);
        else DrawRect(rect.Grow(-4), new Color("3a2e20"));
        if (Round) DrawArc(rect.GetCenter(), box / 2 - 2, 0, Mathf.Tau, 36, Selected ? Colors.White : Ui.Bronze, 4);
        else DrawRect(rect, Selected ? Colors.White : Enabled ? Ui.Bronze : new Color("4a4036"), false, Selected ? 5 : 4);
        var font = Game.BodyBold();
        if (Key > 0)
        {
            DrawRect(new Rect2(rect.Position + new Vector2(4, 4), new Vector2(26, 26)), new Color(0, 0, 0, 0.75f));
            DrawString(font, rect.Position + new Vector2(11, 25), Key.ToString(), HorizontalAlignment.Left, -1, 20, Ui.Ink);
        }
        if (!Round)
        {
            Gem(rect.Position + new Vector2(20, box - 20), new Color("2e86c1"), Delay.ToString());
            if (Momentum > 0) Gem(rect.Position + new Vector2(box - 20, box - 20), new Color("b8962e"), Momentum.ToString());
        }
        DrawString(Game.Body(), new Vector2(0, box + 22), Title, HorizontalAlignment.Center, Size.X, 18, Enabled ? Ui.Ink : new Color("7d725f"));
    }

    private void Gem(Vector2 pos, Color ring, string text)
    {
        DrawCircle(pos, 16, new Color(0.06f, 0.05f, 0.05f, 0.92f));
        DrawArc(pos, 16, 0, Mathf.Tau, 20, ring, 3);
        var size = Game.BodyBold().GetStringSize(text, HorizontalAlignment.Left, -1, 20);
        DrawString(Game.BodyBold(), pos + new Vector2(-size.X / 2, 7), text, HorizontalAlignment.Left, -1, 20, Colors.White);
    }
}

/// <summary>The action bar: the active unit's skills, then Move and Skip Turn as round slots set apart at the end.</summary>
public partial class ActionBar : PanelContainer
{
    private readonly HBoxContainer _row = new();
    public List<SkillSlot> Slots { get; } = new();
    public event Action<SkillSlot>? SlotPressed;

    public ActionBar()
    {
        AddThemeStyleboxOverride("panel", Ui.Box(new Color(Ui.PanelDark, 0.94f), Ui.Gold, 3, 8));
        _row.AddThemeConstantOverride("separation", 10);
        AddChild(_row);
    }

    public static string IconFor(string skillId)
    {
        if (Game.Tex($"res://assets/icons/skills/{skillId}.png") != null) return $"res://assets/icons/skills/{skillId}.png";
        if (Game.Content.AttackSkills.FirstOrDefault(a => a.Id == skillId) is { } a) return $"res://assets/icons/skills/attack-{a.AttackType}.png";
        if (Game.Content.ProfessionSkills.FirstOrDefault(p => p.Id == skillId) is { } d)
            return $"res://assets/icons/skills/{d.Profession}-{d.Branch.ToLowerInvariant().Replace(' ', '-')}.png";
        return "";
    }

    /// <summary>Fills the bar for a unit: every active skill it knows (with the reason when it cannot be used now), then Move and Skip Turn.</summary>
    public void Fill(BattleState state, int unitId)
    {
        foreach (var c in _row.GetChildren()) c.QueueFree();
        Slots.Clear();
        Visible = true;
        var unit = state.GetUnit(unitId);
        var key = 1;
        foreach (var id in unit.KnownSkills.Distinct())
        {
            if (!Game.Catalog.TryGet(id, out var skill) || skill.IsPassive) continue;
            if (!Game.Content.AttackSkills.Any(a => a.Id == id) && !Game.Content.ProfessionSkills.Any(p => p.Id == id)) continue;
            var slot = new SkillSlot
            {
                Id = id, Title = Game.SkillName(id), IconPath = IconFor(id), Key = key <= 9 ? key : 0,
                Delay = skill.Delay, Momentum = Actions.CostOf(state, unitId, skill), CustomMinimumSize = new Vector2(112, 150),
            };
            key++;
            slot.DisabledReason = Reason(state, unitId, skill);
            Add(slot);
        }
        var sep = new VSeparator();
        _row.AddChild(sep);
        var canMove = Movement.LegalMoves(state, unitId).Count > 0;
        Add(new SkillSlot { Id = "@move", Title = "Move", IconPath = "res://assets/icons/skills/action-move.png", Round = true, Delay = 5, CustomMinimumSize = new Vector2(96, 130), DisabledReason = canMove ? null : "No free place to move to" });
        Add(new SkillSlot { Id = "@skip", Title = "Skip Turn", IconPath = "res://assets/icons/skills/action-skip.png", Round = true, Delay = 5, CustomMinimumSize = new Vector2(96, 130) });
    }

    private void Add(SkillSlot slot)
    {
        slot.Pressed += s => SlotPressed?.Invoke(s);
        Slots.Add(slot);
        _row.AddChild(slot);
    }

    public void Clear()
    {
        foreach (var c in _row.GetChildren()) c.QueueFree();
        Slots.Clear();
        Visible = false;
    }

    public void Select(string? id)
    {
        foreach (var s in Slots) { s.Selected = s.Id == id; s.QueueRedraw(); }
    }

    /// <summary>Why a skill cannot be used now: Silenced, the Momentum lead, its own requirements, or no legal target.</summary>
    private static string? Reason(BattleState state, int unitId, Skill skill)
    {
        var user = state.GetUnit(unitId);
        if (skill.Magical && !Statuses.CanUseMagical(user)) return "Silenced: magical skills cannot be used";
        var cost = Actions.CostOf(state, unitId, skill);
        if (!state.Momentum.CanPay(user.Side, cost)) return $"Needs the side to be {cost} ahead in Momentum";
        if (skill.Unavailable(state, unitId) is { } own) return own;
        var any = Everkin.Core.Ai.MoveGenerator.Legal(state, Game.Catalog, unitId, includeMoveAndSkip: false)
            .Any(m => m.Use?.SkillId == skill.Id);
        return any ? null : "No legal target";
    }
}
