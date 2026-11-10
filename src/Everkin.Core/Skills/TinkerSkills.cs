using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Tinker's skills (skills.md Tinker): Engineer, Chemist and Pyrotechnician. The Tinker's actives are physical (not `magical`).
/// The rulings of 2026-11-10 (turret shots, targets, placement, Plague Doctor, Showman) are in skills.md.
/// </summary>
public static class TinkerSkills
{
    public const string BoltTurretDefinition = "bolt_turret";
    public const string FlameTurretDefinition = "flame_turret";
    public const string BarricadeDefinition = "barricade";
    public const int GroundLength = 20;
    public const int FlashbangBlind = 10;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new BoltTurret(D("bolt_turret"));
        yield return new Barricade(D("barricade"));
        yield return new Overclock(D("overclock"));
        yield return new Repair(D("repair"));
        yield return new PoisonGas(D("poison_gas"));
        yield return new GasCloud(D("gas_cloud"));
        yield return new HealingDraught(D("healing_draught"));
        yield return new Antidote(D("antidote"));
        yield return new PlagueDoctor(D("plague_doctor"));
        yield return new Firebomb(D("firebomb"));
        yield return new FlameTurret(D("flame_turret"));
        yield return new Flashbang(D("flashbang"));
        yield return new Showman(D("showman"));
    }

    private static readonly Row[] AllRows = { Row.Front, Row.Middle, Row.Rear };

    internal static IReadOnlyList<MoveOption> TurretPlaces(BattleState state, int userId)
    {
        var allowed = Summons.PlacementOptions(state, state.GetUnit(userId).Side, AllRows);
        return Summons.GapsNextToUser(state, userId).Where(allowed.Contains).ToArray();
    }

    /// <summary>The length of the Tinker's poison ground: twice as long with Plague Doctor.</summary>
    internal static int PoisonLength(SkillContext context) => context.User.KnownSkills.Contains("plague_doctor") ? GroundLength * 2 : GroundLength;
}

/// <summary>The auto events of the Tinker's turrets (game_system sections 6.2, 6.7): a shot after each action of the Tinker.</summary>
public static class Turrets
{
    public const string EventTagPrefix = "turret:";

    private sealed class TurretShotSkill : Skill
    {
        public TurretShotSkill() : base("turret_shot", 0, 0) { }
        public override TargetKind Target => TargetKind.Unit;
        public override Delivery? Delivery => Battle.Delivery.Projectile;
        public override EffectTags Tags => EffectTags.Projectile | EffectTags.Physical;
        public override bool IsAttack => true;
        public override void Resolve(SkillContext context) => throw new InvalidOperationException("A turret shot is resolved by Turrets.Fire.");
    }

    private static readonly TurretShotSkill Shot = new();

    public static bool IsTurret(Unit unit) =>
        unit.CreatureId is TinkerSkills.BoltTurretDefinition or TinkerSkills.FlameTurretDefinition;

    /// <summary>The Tinker took an action: every turret of it queues a shot due now, and the shots resolve in the order the turrets were placed.</summary>
    public static void AfterAction(BattleState state, int tinkerId)
    {
        if (state.IsOver || state.GetUnit(tinkerId).IsKnockedOut) return;
        FireAll(state, tinkerId);
    }

    /// <summary>Queues one shot for each live turret of the Tinker and resolves them now. Returns how many were queued.</summary>
    public static int FireAll(BattleState state, int tinkerId)
    {
        var turrets = state.Units.Where(u => u.SummonerId == tinkerId && IsTurret(u) && !u.IsKnockedOut && !u.IsRemoved).Select(u => u.Id).ToList();
        var queued = turrets.Select(id => state.Timeline.AddNow(tinkerId, EventTagPrefix + id)).ToList();
        foreach (var entry in queued)
        {
            if (state.IsOver) break;
            if (state.Timeline.Remove(entry.Id)) HandleAutoEvent(state, entry);   // false: the event was removed on the way
        }
        return queued.Count;
    }

    /// <summary>An auto event reached its turn.</summary>
    public static void HandleAutoEvent(BattleState state, TimelineEntry entry)
    {
        if (entry.Tag is { } tag && tag.StartsWith(EventTagPrefix, StringComparison.Ordinal) && int.TryParse(tag.AsSpan(EventTagPrefix.Length), out var turretId))
            Fire(state, turretId);
    }

    private static void Fire(BattleState state, int turretId)
    {
        var turret = state.GetUnit(turretId);
        if (state.IsOver || turret.IsKnockedOut || turret.IsRemoved || turret.SummonerId is not { } tinker) return;
        if (Targeting.PickNearest(state, turretId, Reach.Unlimited) is not { } target) return;       // no legal enemy: the shot is lost

        if (turret.CreatureId == TinkerSkills.FlameTurretDefinition)
        {
            state.Ground.Place(GroundKind.Burning, state.Ground.SpotOf(target), TinkerSkills.GroundLength, tinker);
            Actions.Settle(state, tinker);
            return;
        }
        new SkillContext(state, Shot, turretId, new SkillUse(turretId, Shot.Id, target), target).Hit(target, new IntRange(2, 3));
    }
}

/// <summary>Base of the Tinker's actives.</summary>
public abstract class TinkerSkill : ProfessionSkill
{
    protected TinkerSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
}

/// <summary>Base of the construct-placing skills.</summary>
public abstract class PlaceConstructSkill : TinkerSkill
{
    protected PlaceConstructSkill(ProfessionSkillDef def) : base(def) { }
    public sealed override TargetKind Target => TargetKind.Placement;

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place for it (or the side is at its cap)" : null;
}

// ------------------------------------------------------------------------------------------------ Engineer

/// <summary>Bolt Turret: places a turret (HP 8). Each time the Tinker takes a turn, it hits the nearest enemy for 2–3.</summary>
public sealed class BoltTurret : PlaceConstructSkill
{
    public BoltTurret(ProfessionSkillDef def) : base(def) { }
    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) => TinkerSkills.TurretPlaces(state, userId);

    public override void Resolve(SkillContext context)
    {
        var place = context.Placement!.Value;
        Summons.Summon(context.State, new SummonSpec(context.UserSide, "Bolt Turret", UnitKind.Construct, 8, 1, context.UserId, TakesTurns: false,
            DefinitionId: TinkerSkills.BoltTurretDefinition), place.Row, place.Index);
        context.Settle();
    }
}

/// <summary>Barricade: places a barricade (HP 10) on any free spot of your side. Double HP with the Guardian's Keeper of the Gate.</summary>
public sealed class Barricade : PlaceConstructSkill
{
    public Barricade(ProfessionSkillDef def) : base(def) { }

    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) =>
        Summons.PlacementOptions(state, state.GetUnit(userId).Side, new[] { Row.Front, Row.Middle, Row.Rear });

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var place = context.Placement!.Value;
        var keeper = state.Units.Any(u => u.Side == context.UserSide && !u.IsKnockedOut && !u.IsRemoved && u.KnownSkills.Contains("keeper_of_the_gate"));
        Summons.Summon(state, new SummonSpec(context.UserSide, "Barricade", UnitKind.Construct, keeper ? 20 : 10, 1, context.UserId, TakesTurns: false,
            DefinitionId: TinkerSkills.BarricadeDefinition), place.Row, place.Index);
        context.Settle();
    }
}

/// <summary>Overclock: every construct of the Tinker acts once more right now (each turret fires a shot).</summary>
public sealed class Overclock : TinkerSkill
{
    public Overclock(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        Turrets.FireAll(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>Repair: a construct heals 8.</summary>
public sealed class Repair : TinkerSkill
{
    public Repair(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override string? Unavailable(BattleState state, int userId) =>
        state.GetUnit(userId).KnownSkills.Contains("plague_doctor") ? "Plague Doctor: the Tinker cannot heal" : null;

    public override bool TargetFilter(BattleState state, int userId, int targetId) => state.GetUnit(targetId).Kind == UnitKind.Construct;
    public override void Resolve(SkillContext context) => context.Heal(context.TargetId!.Value, new IntRange(8, 8));
}

// ------------------------------------------------------------------------------------------------ Chemist

/// <summary>Poison Gas: poison ground on a unit's spot and the spots beside it, for 20 time units (40 with Plague Doctor).</summary>
public sealed class PoisonGas : TinkerSkill
{
    public PoisonGas(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var ground = context.State.Ground;
        var spot = ground.SpotOf(context.TargetId!.Value);
        var length = TinkerSkills.PoisonLength(context);
        foreach (var s in new[] { spot }.Concat(ElementalistSkills.Beside(spot)))
            ground.Place(GroundKind.Poison, s, length, context.UserId);
        context.Settle();
    }
}

/// <summary>Gas Cloud: poison ground on a whole row for 20 time units (the spot of every unit in it).</summary>
public sealed class GasCloud : TinkerSkill
{
    public GasCloud(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        var length = TinkerSkills.PoisonLength(context);
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).ToArray())
            context.State.Ground.Place(GroundKind.Poison, context.State.Ground.SpotOf(id), length, context.UserId);
        context.Settle();
    }
}

/// <summary>Healing Draught: throw a potion: a unit heals 6–7. A projectile: an interceptor can take it.</summary>
public sealed class HealingDraught : TinkerSkill
{
    public HealingDraught(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Projectile;
    public override EffectTags Tags => EffectTags.Projectile | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(2);

    public override string? Unavailable(BattleState state, int userId) =>
        state.GetUnit(userId).KnownSkills.Contains("plague_doctor") ? "Plague Doctor: the Tinker cannot heal" : null;

    public override void Resolve(SkillContext context) => context.Heal(context.TargetId!.Value, new IntRange(6, 7));
}

/// <summary>Antidote: remove Poison, Bleeding and Blind from a unit.</summary>
public sealed class Antidote : TinkerSkill
{
    public Antidote(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(2);

    public override void Resolve(SkillContext context)
    {
        foreach (var id in new[] { StatusIds.Poison, StatusIds.Bleeding, StatusIds.Blind })
            Statuses.Remove(context.State, context.TargetId!.Value, id, StatusEndCause.Cleansed, context.UserId);
        context.Settle();
    }
}

/// <summary>Plague Doctor (keystone): the Tinker's poison ground lasts twice as long, but the Tinker cannot heal. Read by the skills concerned.</summary>
public sealed class PlagueDoctor : PassiveSkill
{
    public PlagueDoctor(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Pyrotechnician

/// <summary>Firebomb: fire on a unit's spot and the spots beside it, for 20 time units. A projectile: an interceptor's spot burns instead.</summary>
public sealed class Firebomb : TinkerSkill
{
    public Firebomb(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Projectile;
    public override EffectTags Tags => EffectTags.Projectile | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(2);

    public override void Resolve(SkillContext context)
    {
        var spot = context.State.Ground.SpotOf(context.InterceptEffect(context.TargetId!.Value));
        foreach (var s in new[] { spot }.Concat(ElementalistSkills.Beside(spot))) ElementalistSkills.Ignite(context, s);
        context.Settle();
    }
}

/// <summary>Flame Turret: places a turret (HP 8). Each time the Tinker takes a turn, it sets fire to the spot of the nearest enemy.</summary>
public sealed class FlameTurret : PlaceConstructSkill
{
    public FlameTurret(ProfessionSkillDef def) : base(def) { }
    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) => TinkerSkills.TurretPlaces(state, userId);

    public override void Resolve(SkillContext context)
    {
        var place = context.Placement!.Value;
        Summons.Summon(context.State, new SummonSpec(context.UserSide, "Flame Turret", UnitKind.Construct, 8, 1, context.UserId, TakesTurns: false,
            DefinitionId: TinkerSkills.FlameTurretDefinition), place.Row, place.Index);
        context.Settle();
    }
}

/// <summary>Flashbang: every unit in one row is Blind for 10 time units.</summary>
public sealed class Flashbang : TinkerSkill
{
    public Flashbang(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override bool IsArea => true;
    public override Reach Reach => Reach.Distance(2);
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in AreaPatterns.Row(context.State, row.Side, row.Row))
            Statuses.Apply(context.State, new StatusApplication(id, StatusIds.Blind, TinkerSkills.FlashbangBlind, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Showman (keystone): the Tinker's Blind and Stunned last twice as long, but its skills that deal damage have +2 Delay.</summary>
public sealed class Showman : PassiveSkill
{
    public Showman(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddStatusApplicationModifier((st, a) =>
            a.SourceUnitId == ownerId && a.Id is StatusIds.Blind or StatusIds.Stunned && a.Duration is { } d ? a with { Duration = d * 2 } : a);
        state.AddSkillDelayModifier((st, userId, isAttack) => userId == ownerId && isAttack ? 2 : 0);
    }
}
