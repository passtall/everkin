using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Totemist (skills.md): totems that protect a row. Magical.</summary>
public static class TotemistSkills
{
    public const string HealingTotemId = "healing_totem";
    public const string SpiritLinkId = "spirit_link_totem";
    public const string ReturnTotemId = "totem_of_return";
    public const string HealTagPrefix = "totem_heal:";
    public const int PulseEvery = 10;
    public const int PulseHeal = 2;
    public const int SpiritShieldWard = 4;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new HealingTotem(D("healing_totem"));
        yield return new SpiritLinkTotem(D("spirit_link_totem"));
        yield return new TotemOfReturn(D("totem_of_return"));
        yield return new GreatTotem(D("great_totem"));
        yield return new SturdyTotems(D("sturdy_totems"));
        yield return new SpiritShield(D("spirit_shield"));
    }

    internal static bool IsTotem(Unit u) => u.CreatureId is HealingTotemId or SpiritLinkId or ReturnTotemId;

    /// <summary>The totems a Totemist has on the board.</summary>
    public static IEnumerable<Unit> TotemsOf(BattleState state, int userId) =>
        state.Units.Where(u => u.SummonerId == userId && IsTotem(u) && !u.IsKnockedOut && !u.IsRemoved);

    /// <summary>Whether a unit is covered by a totem: an ally in its row, the totem itself included; with Great Totem also the rows next to it.</summary>
    public static bool Covers(BattleState state, Unit totem, int unitId)
    {
        if (!state.Formation.Contains(totem.Id) || !state.Formation.Contains(unitId)) return false;
        var t = state.Formation.PositionOf(totem.Id);
        var u = state.Formation.PositionOf(unitId);
        if (t.Side != u.Side) return false;
        var reach = totem.SummonerId is { } owner && state.GetUnit(owner).KnownSkills.Contains("great_totem") ? 1 : 0;
        return Math.Abs((int)t.Row - (int)u.Row) <= reach;
    }

    /// <summary>Registers the totem rules shared by every totem on the board: pulses, damage sharing and the Totem of Return save.</summary>
    internal static void InstallShared(BattleState state)
    {
        if (!state.FirstInstall("totems")) return;
        state.AddAutoEventHandler(HandleAutoEvent);
        state.AddDamageSharing((st, targetId) =>
        {
            foreach (var totem in st.Units.Where(u => u.CreatureId == SpiritLinkId && !u.IsKnockedOut && !u.IsRemoved))
                if (targetId != totem.Id && st.GetUnit(targetId).Kind != UnitKind.Construct && Covers(st, totem, targetId))
                    return st.Units.Where(u => u.Id != totem.Id && u.Kind != UnitKind.Construct && !u.IsKnockedOut && Covers(st, totem, u.Id)).Select(u => u.Id).ToArray();
            return Array.Empty<int>();
        });
        state.AddLethalGuard((st, targetId) =>
        {
            foreach (var totem in st.Units.Where(u => u.CreatureId == ReturnTotemId && !u.IsKnockedOut && !u.IsRemoved))
                if (targetId != totem.Id && st.GetUnit(targetId).Kind != UnitKind.Construct && Covers(st, totem, targetId)) return totem.Id;
            return null;
        });
    }

    public static bool HandleAutoEvent(BattleState state, TimelineEntry entry)
    {
        if (entry.Tag is not { } tag || !tag.StartsWith(HealTagPrefix, StringComparison.Ordinal) || !int.TryParse(tag.AsSpan(HealTagPrefix.Length), out var totemId)) return false;
        var totem = state.GetUnit(totemId);
        if (state.IsOver || totem.IsKnockedOut || totem.IsRemoved) return true;
        var amount = DamageMath.Scale(PulseHeal, state.PowerOf(totemId));
        foreach (var id in state.Units.Where(u => !u.IsKnockedOut && Covers(state, totem, u.Id)).Select(u => u.Id).ToArray())
            state.Heal(totemId, id, amount);
        state.Timeline.Add(TimelineKind.AutoEvent, PulseEvery, totemId, HealTagPrefix + totemId);
        Actions.Settle(state, totemId);
        return true;
    }
}

/// <summary>Base of the skills that place a totem on any free spot of your side.</summary>
public abstract class TotemSkill : ClassSkill
{
    private readonly string _name;
    private readonly int _hp;
    private readonly string _definition;

    protected TotemSkill(SkillDef def, string name, int hp, string definition) : base(def, magical: true)
    {
        _name = name;
        _hp = hp;
        _definition = definition;
    }

    public sealed override TargetKind Target => TargetKind.Placement;
    public override void Install(BattleState state, int ownerId) => TotemistSkills.InstallShared(state);

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place for it (or the side is at its cap)" : null;

    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId)
    {
        var all = new[] { Row.Front, Row.Middle, Row.Rear };
        // With Great Totem the earlier totem leaves first, so the places are those of the board without it.
        if (!state.GetUnit(userId).KnownSkills.Contains("great_totem") || !TotemistSkills.TotemsOf(state, userId).Any())
            return Summons.PlacementOptions(state, state.GetUnit(userId).Side, all);
        var without = state.Clone();
        foreach (var old in TotemistSkills.TotemsOf(without, userId).ToArray()) Summons.Remove(without, old.Id, RemovalCause.Replaced);
        return Summons.PlacementOptions(without, state.GetUnit(userId).Side, all);
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var known = context.User.KnownSkills;
        if (known.Contains("great_totem"))
            foreach (var old in TotemistSkills.TotemsOf(state, context.UserId).ToArray()) Summons.Remove(state, old.Id, RemovalCause.Replaced);
        var hp = known.Contains("great_totem") ? _hp * 3 : known.Contains("sturdy_totems") ? _hp * 2 : _hp;
        var place = context.Placement!.Value;
        var id = Summons.Summon(state, new SummonSpec(context.UserSide, _name, UnitKind.Construct, hp, 1, context.UserId, TakesTurns: false, DefinitionId: _definition), place.Row, place.Index);
        Placed(context, id);
        context.Settle();
    }

    protected virtual void Placed(SkillContext context, int totemId) { }
}

/// <summary>Healing Totem: place a totem (HP 8): every 10, allies in its row heal 2 (a repeating auto event owned by the totem, so it goes on if the Totemist is KO).</summary>
public sealed class HealingTotem : TotemSkill
{
    public HealingTotem(SkillDef def) : base(def, "Healing Totem", 8, TotemistSkills.HealingTotemId) { }

    protected override void Placed(SkillContext context, int totemId) =>
        context.State.Timeline.Add(TimelineKind.AutoEvent, TotemistSkills.PulseEvery, totemId, TotemistSkills.HealTagPrefix + totemId);
}

/// <summary>Spirit Link Totem: place a totem (HP 10): damage to allies in its row is split evenly between them (the totem excluded; the hurt unit keeps the remainder).</summary>
public sealed class SpiritLinkTotem : TotemSkill
{
    public SpiritLinkTotem(SkillDef def) : base(def, "Spirit Link Totem", 10, TotemistSkills.SpiritLinkId) { }
}

/// <summary>Totem of Return: place a totem (HP 6): the first ally in its row that would be KO keeps 1 HP instead, and the totem breaks (it is removed, not KO'd).</summary>
public sealed class TotemOfReturn : TotemSkill
{
    public TotemOfReturn(SkillDef def) : base(def, "Totem of Return", 6, TotemistSkills.ReturnTotemId) { }
}

/// <summary>Great Totem: the Totemist keeps only one totem (a new one replaces the old), but it has triple HP and also affects the rows next to it. The totem skills read this keystone.</summary>
public sealed class GreatTotem : KeystoneSkill
{
    public GreatTotem(SkillDef def) : base(def) { }
}

/// <summary>Sturdy Totems: totems of the Totemist have double HP. The totem skills read this keystone.</summary>
public sealed class SturdyTotems : KeystoneSkill
{
    public SturdyTotems(SkillDef def) : base(def) { }
}

/// <summary>Spirit Shield: when a totem of the Totemist breaks, every ally in its row gains a Ward of 4.</summary>
public sealed class SpiritShield : KeystoneSkill
{
    public SpiritShield(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddRemovalHook((st, unitId, cause, from) =>
        {
            var totem = st.GetUnit(unitId);
            if (cause is not (RemovalCause.Ko or RemovalCause.Expired) || !TotemistSkills.IsTotem(totem) || totem.SummonerId != ownerId || st.GetUnit(ownerId).IsKnockedOut) return;
            var reach = st.GetUnit(ownerId).KnownSkills.Contains("great_totem") ? 1 : 0;
            foreach (var id in st.Units.Where(u => u.Side == from.Side && !u.IsKnockedOut && !u.IsRemoved && st.Formation.Contains(u.Id)
                && Math.Abs((int)st.Formation.PositionOf(u.Id).Row - (int)from.Row) <= reach).Select(u => u.Id).ToArray())
                Statuses.Apply(st, new StatusApplication(id, StatusIds.Ward, Magnitude: DamageMath.Scale(TotemistSkills.SpiritShieldWard, st.PowerOf(ownerId)), SourceUnitId: ownerId));
        });
}
