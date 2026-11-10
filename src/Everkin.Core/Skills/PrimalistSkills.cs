using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Primalist's skills (skills.md Primalist): Totemist, Ritualist and Spiritwalker. The Primalist's actives are `magical`. The
/// rulings of 2026-11-10 (totems, rituals, possession, links) are in skills.md.
/// </summary>
public static class PrimalistSkills
{
    public const string HealingTotemDefinition = "healing_totem";
    public const string SpiritLinkDefinition = "spirit_link_totem";
    public const string ReturnDefinition = "totem_of_return";
    public const int RainLength = 20;
    public const int LinkLength = 20;
    public const int MistsLength = 20;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new HealingTotem(D("healing_totem"));
        yield return new SpiritLinkTotem(D("spirit_link_totem"));
        yield return new TotemOfReturn(D("totem_of_return"));
        yield return new GreatTotem(D("great_totem"));
        yield return new RainDance(D("rain_dance"));
        yield return new Hoarfrost(D("hoarfrost"));
        yield return new TotemRitual(D("totem_ritual"));
        yield return new LongRain(D("long_rain"));
        yield return new HealingRain(D("healing_rain"));
        yield return new RiteOfMists(D("rite_of_mists"));
        yield return new PuppetSoul(D("puppet_soul"));
        yield return new LifeSwap(D("life_swap"));
        yield return new ForcedStep(D("forced_step"));
        yield return new LinkedFates(D("linked_fates"));
        yield return new SharedFate(D("shared_fate"));
    }

    internal static bool IsTotem(Unit u) => u.CreatureId is HealingTotemDefinition or SpiritLinkDefinition or ReturnDefinition;

    /// <summary>The totems of a Primalist.</summary>
    internal static IEnumerable<Unit> TotemsOf(BattleState state, int primalistId) =>
        state.Units.Where(u => u.SummonerId == primalistId && IsTotem(u) && !u.IsKnockedOut && !u.IsRemoved);

    /// <summary>Whether a totem affects a unit: an ally in its row, or (Great Totem) in the rows directly in front of and behind it.</summary>
    internal static bool Covers(BattleState state, Unit totem, int unitId)
    {
        var unit = state.GetUnit(unitId);
        if (totem.IsKnockedOut || totem.IsRemoved || unit.IsKnockedOut || unit.IsRemoved || unit.Side != totem.Side) return false;
        if (!state.Formation.Contains(totem.Id) || !state.Formation.Contains(unitId)) return false;
        var rows = Math.Abs((int)state.Formation.PositionOf(totem.Id).Row - (int)state.Formation.PositionOf(unitId).Row);
        var great = totem.SummonerId is { } p && state.GetUnit(p).KnownSkills.Contains("great_totem");
        return rows == 0 || (great && rows == 1);
    }

    internal static IEnumerable<Unit> Totems(BattleState state, string definition) =>
        state.Units.Where(u => u.CreatureId == definition && !u.IsKnockedOut && !u.IsRemoved);
}

/// <summary>The auto events of totems (and of turrets): the Healing Totem's pulse every 10 time units.</summary>
public static class Totems
{
    public const string HealTagPrefix = "totem_heal:";
    public const int PulseEvery = 10;
    public const int PulseHeal = 2;

    public static void HandleAutoEvent(BattleState state, TimelineEntry entry)
    {
        if (entry.Tag is { } tag && tag.StartsWith(HealTagPrefix, StringComparison.Ordinal) && int.TryParse(tag.AsSpan(HealTagPrefix.Length), out var totemId))
        {
            Pulse(state, totemId);
            return;
        }
        if (FlyingSkills.TryHandleAutoEvent(state, entry)) return;
        Turrets.HandleAutoEvent(state, entry);
    }

    private static void Pulse(BattleState state, int totemId)
    {
        var totem = state.GetUnit(totemId);
        if (state.IsOver || totem.IsKnockedOut || totem.IsRemoved) return;
        var amount = DamageMath.Scale(PulseHeal, state.PowerOf(totemId));
        foreach (var id in state.Units.Where(u => PrimalistSkills.Covers(state, totem, u.Id)).Select(u => u.Id).ToArray())
            if (!state.GetUnit(id).IsKnockedOut) state.Heal(totemId, id, amount);
        state.Timeline.Add(TimelineKind.AutoEvent, PulseEvery, totemId, HealTagPrefix + totemId);
        Actions.Settle(state, totemId);
    }

    public static void StartHealing(BattleState state, int totemId) =>
        state.Timeline.Add(TimelineKind.AutoEvent, PulseEvery, totemId, HealTagPrefix + totemId);
}

/// <summary>Base of the Primalist's actives.</summary>
public abstract class PrimalistSkill : ProfessionSkill
{
    protected PrimalistSkill(ProfessionSkillDef def) : base(def) { }
    public override bool Magical => true;
    public override EffectTags Tags => EffectTags.Magical;
    public override TargetKind Target => TargetKind.Unit;
}

/// <summary>Base of the skills that place a totem next to the Primalist.</summary>
public abstract class TotemSkill : PrimalistSkill
{
    private readonly string _name;
    private readonly int _hp;
    private readonly string _definition;

    protected TotemSkill(ProfessionSkillDef def, string name, int hp, string definition) : base(def)
    {
        _name = name;
        _hp = hp;
        _definition = definition;
    }

    public sealed override TargetKind Target => TargetKind.Placement;

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place next to the Primalist (or the side is at its cap)" : null;

    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId)
    {
        // With Great Totem the earlier totem leaves first, so the places are those of the board without it.
        if (!state.GetUnit(userId).KnownSkills.Contains("great_totem") || !PrimalistSkills.TotemsOf(state, userId).Any())
            return TinkerSkills.TurretPlaces(state, userId);
        var without = state.Clone();
        foreach (var old in PrimalistSkills.TotemsOf(without, userId).ToArray()) Summons.Remove(without, old.Id, RemovalCause.Expired);
        return TinkerSkills.TurretPlaces(without, userId);
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var great = context.User.KnownSkills.Contains("great_totem");
        if (great)
            foreach (var old in PrimalistSkills.TotemsOf(state, context.UserId).ToArray())
                Summons.Remove(state, old.Id, RemovalCause.Expired);       // the Primalist keeps one totem
        var place = context.Placement!.Value;
        var id = Summons.Summon(state, new SummonSpec(context.UserSide, _name, UnitKind.Construct, great ? _hp * 3 : _hp, 1, context.UserId, TakesTurns: false,
            DefinitionId: _definition), place.Row, place.Index);
        if (_definition == PrimalistSkills.HealingTotemDefinition) Totems.StartHealing(state, id);
        context.Settle();
    }
}

// ------------------------------------------------------------------------------------------------ Totemist

/// <summary>Healing Totem: allies in its row regain 2 HP every 10 time units. HP 8.</summary>
public sealed class HealingTotem : TotemSkill
{
    public HealingTotem(ProfessionSkillDef def) : base(def, "Healing Totem", 8, PrimalistSkills.HealingTotemDefinition) { }
}

/// <summary>Spirit Link Totem: damage to allies in its row is split evenly between them. HP 10.</summary>
public sealed class SpiritLinkTotem : TotemSkill
{
    public SpiritLinkTotem(ProfessionSkillDef def) : base(def, "Spirit Link Totem", 10, PrimalistSkills.SpiritLinkDefinition) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("spirit_link_totem")) return;
        state.AddDamageSharing((st, targetId) =>
        {
            var totem = PrimalistSkills.Totems(st, PrimalistSkills.SpiritLinkDefinition).FirstOrDefault(t => t.Id != targetId && PrimalistSkills.Covers(st, t, targetId));
            if (totem is null) return Array.Empty<int>();
            return st.Units.Where(u => u.Id != totem.Id && u.Id != targetId && !PrimalistSkills.IsTotem(u) && PrimalistSkills.Covers(st, totem, u.Id)).Select(u => u.Id).ToArray();
        });
    }
}

/// <summary>Totem of Return: the first ally in its row that would be KO keeps 1 HP instead, and the totem breaks. HP 6.</summary>
public sealed class TotemOfReturn : TotemSkill
{
    public TotemOfReturn(ProfessionSkillDef def) : base(def, "Totem of Return", 6, PrimalistSkills.ReturnDefinition) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("totem_of_return")) return;
        state.AddLethalGuard((st, targetId) =>
            PrimalistSkills.Totems(st, PrimalistSkills.ReturnDefinition).FirstOrDefault(t => t.Id != targetId && PrimalistSkills.Covers(st, t, targetId))?.Id);
    }
}

/// <summary>Great Totem (keystone): the Primalist keeps only 1 totem, but it has triple HP and also affects the rows next to it. Read by the totem skills.</summary>
public sealed class GreatTotem : PassiveSkill
{
    public GreatTotem(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Ritualist

/// <summary>Rain Dance: water on every spot of one row (20 time units, 40 with Long Rain). Healing Rain heals allies in the rain once.</summary>
public sealed class RainDance : PrimalistSkill
{
    public RainDance(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var row = context.Row!.Value;
        var length = PrimalistSkills.RainLength * (context.User.KnownSkills.Contains("long_rain") ? 2 : 1);
        foreach (var id in state.Formation.Units(row.Side, row.Row).ToArray())
            state.Ground.Place(GroundKind.Water, state.Ground.SpotOf(id), length, context.UserId);
        if (context.User.KnownSkills.Contains("healing_rain"))
            foreach (var unit in state.Units.Where(u => u.Side == context.UserSide && !u.IsKnockedOut && !u.IsRemoved && state.Formation.Contains(u.Id)).ToArray())
            {
                var spot = state.Ground.SpotOf(unit.Id);
                if (state.Ground.Has(spot, GroundKind.Water) && state.Ground.Affected(spot).Contains(unit.Id)) context.HealUnit(unit.Id, context.Scaled(4));
            }
        context.Settle();
    }
}

/// <summary>Hoarfrost: ice on a unit's spot (20 time units).</summary>
public sealed class Hoarfrost : PrimalistSkill
{
    public Hoarfrost(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        context.State.Ground.Place(GroundKind.Ice, context.State.Ground.SpotOf(context.TargetId!.Value), PrimalistSkills.RainLength, context.UserId);
        context.Settle();
    }
}

/// <summary>Totem Ritual: every totem of the Primalist regains all its HP.</summary>
public sealed class TotemRitual : PrimalistSkill
{
    public TotemRitual(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        foreach (var totem in PrimalistSkills.TotemsOf(context.State, context.UserId))
            context.State.Heal(context.UserId, totem.Id, totem.MaxHp);
        context.Settle();
    }
}

/// <summary>Long Rain (modifier of Rain Dance): the water lasts 20 time units longer. Read by Rain Dance.</summary>
public sealed class LongRain : ModifierSkill
{
    public LongRain(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Healing Rain (modifier of Rain Dance): allies in the rain regain 4 HP. Read by Rain Dance.</summary>
public sealed class HealingRain : ModifierSkill
{
    public HealingRain(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Rite of Mists: projectile hits from units standing on water deal half damage for 20 time units.</summary>
public sealed class RiteOfMists : PrimalistSkill
{
    public RiteOfMists(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        context.State.MistsUntil = context.State.Timeline.Now + PrimalistSkills.MistsLength;
        context.Settle();
    }
}

// ------------------------------------------------------------------------------------------------ Spiritwalker

/// <summary>Forced Step: possession: a unit moves to a free gap of your choice in another row of its own side.</summary>
public sealed class ForcedStep : PrimalistSkill
{
    public ForcedStep(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override bool TargetFilter(BattleState state, int userId, int targetId) => PlacementOptionsFor(state, userId, targetId).Count > 0;

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId)
    {
        var result = new List<MoveOption>();
        var unit = state.GetUnit(targetId);
        if (!Movement.IsMovable(unit) || Movement.IsImmune(state, targetId, state.GetUnit(userId).Side)) return result;
        var from = state.Formation.PositionOf(targetId);
        for (var r = 0; r < Formation.RowCount; r++)
        {
            var count = state.Formation.Count(from.Side, (Row)r);
            if ((Row)r == from.Row || count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++) result.Add(new MoveOption((Row)r, gap));
        }
        return result;
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        var place = context.Placement!.Value;
        state.Ground.Accrue();
        var from = state.Formation.Remove(target);
        state.Formation.Place(target, from.Side, place.Row, place.Index);
        var move = new UnitMoved(target, from, state.Formation.PositionOf(target), MoveCause.Forced);
        state.Emit(move);
        context.AfterForcedMove(new ForcedMoveResult(ForcedMoveStatus.Moved, new[] { move }, Array.Empty<int>()));
    }
}

/// <summary>Puppet Soul: possession: a unit hits a unit next to it at once with its basic attack.</summary>
public sealed class PuppetSoul : PrimalistSkill
{
    public PuppetSoul(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override bool PairFilter(BattleState state, int userId, int firstId, int secondId) =>
        !state.GetUnit(firstId).IsKnockedOut && state.Formation.Adjacent(firstId).Contains(secondId);

    public override string? CheckUse(BattleState state, SkillCatalog catalog, SkillUse use) =>
        use.TargetUnitId is { } puppet && puppet >= 0 && puppet < state.Units.Count && SummonerSkills.AttackSkillOf(catalog, state.GetUnit(puppet)) is null
            ? "the puppet has no basic attack" : null;

    public override void Resolve(SkillContext context)
    {
        var puppet = context.TargetId!.Value;
        var victim = context.SecondTargetId!.Value;
        var skill = context.Catalog!.Get(SummonerSkills.AttackSkillOf(context.Catalog, context.State.GetUnit(puppet))!);
        var target = skill.Target == TargetKind.Unit ? victim : (int?)null;
        skill.Resolve(new SkillContext(context.State, skill, puppet, new SkillUse(puppet, skill.Id, target), target) { Catalog = context.Catalog });
    }
}

/// <summary>Life Swap: soul swap: two units swap their HP as a share of their maximum.</summary>
public sealed class LifeSwap : PrimalistSkill
{
    public LifeSwap(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var a = context.State.GetUnit(context.TargetId!.Value);
        var b = context.State.GetUnit(context.SecondTargetId!.Value);
        var hpA = Math.Max(1, b.Hp * a.MaxHp / b.MaxHp);
        var hpB = Math.Max(1, a.Hp * b.MaxHp / a.MaxHp);
        a.Hp = hpA;
        b.Hp = hpB;
        context.State.Emit(new HpSwapped(a.Id, b.Id, hpA, hpB));
        context.Settle();
    }
}

/// <summary>Linked Fates: soul swap: for 20 time units, the second unit also takes half of all damage the first takes.</summary>
public sealed class LinkedFates : PrimalistSkill
{
    public LinkedFates(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.FateLink, PrimalistSkills.LinkLength, context.SecondTargetId!.Value, context.UserId));
        context.Settle();
    }
}

/// <summary>Shared Fate: soul swap: for 20 time units, two units each also take half of the damage the other takes.</summary>
public sealed class SharedFate : PrimalistSkill
{
    public SharedFate(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var a = context.TargetId!.Value;
        var b = context.SecondTargetId!.Value;
        Statuses.Apply(context.State, new StatusApplication(a, StatusIds.FateLink, PrimalistSkills.LinkLength, b, context.UserId));
        Statuses.Apply(context.State, new StatusApplication(b, StatusIds.FateLink, PrimalistSkills.LinkLength, a, context.UserId));
        context.Settle();
    }
}
