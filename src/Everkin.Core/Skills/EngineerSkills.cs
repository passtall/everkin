using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Engineer (skills.md): turrets, barricades and traps. Physical.</summary>
public static class EngineerSkills
{
    public const string BoltTurretId = "bolt_turret";
    public const string BarricadeId = "barricade";
    public const string NetTrapId = "net_trap";
    public const int TurretHp = 8;
    public const int BarricadeHp = 10;
    public const int TrapHp = 4;
    public const int TrapDelay = 3;
    public const int RepairAmount = 2;

    private static readonly Row[] AllRows = { Row.Front, Row.Middle, Row.Rear };

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new BoltTurret(D("bolt_turret"));
        yield return new Barricade(D("barricade"));
        yield return new NetTrap(D("net_trap"));
        yield return new Reinforced(D("reinforced"));
        yield return new SelfRepair(D("self_repair"));
        yield return new RapidFire(D("rapid_fire"));
    }

    /// <summary>Any free spot on the user's side, between units or at either end (section 11).</summary>
    internal static IReadOnlyList<MoveOption> Places(BattleState state, int userId) =>
        Summons.PlacementOptions(state, state.GetUnit(userId).Side, AllRows);

    internal static int Hp(SkillContext context, int hp) => context.User.KnownSkills.Contains("reinforced") ? hp * 2 : hp;

    internal static bool IsTurret(Unit u) => u.CreatureId == BoltTurretId;
}

/// <summary>Base of the construct-placing skills: the Engineer puts a construct on any free spot of its side.</summary>
public abstract class PlaceConstructSkill : ClassSkill
{
    protected PlaceConstructSkill(SkillDef def) : base(def) { }
    public sealed override TargetKind Target => TargetKind.Placement;
    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) => EngineerSkills.Places(state, userId);
    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place for it (or the side is at its cap)" : null;
}

/// <summary>Bolt Turret: place a turret (HP 8). Each time the Engineer takes a turn, the turret hits the nearest enemy for 2-3.</summary>
public sealed class BoltTurret : PlaceConstructSkill
{
    public BoltTurret(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("turrets")) return;
        state.AddAfterActionHook(Turrets.AfterAction);
        state.AddAutoEventHandler(Turrets.HandleAutoEvent);
    }

    public override void Resolve(SkillContext context)
    {
        var place = context.Placement!.Value;
        Summons.Summon(context.State, new SummonSpec(context.UserSide, "Bolt Turret", UnitKind.Construct, EngineerSkills.Hp(context, EngineerSkills.TurretHp), 1, context.UserId,
            TakesTurns: false, DefinitionId: EngineerSkills.BoltTurretId), place.Row, place.Index);
        context.Settle();
    }
}

/// <summary>Barricade: place a barricade (HP 10).</summary>
public sealed class Barricade : PlaceConstructSkill
{
    public Barricade(SkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        var place = context.Placement!.Value;
        Summons.Summon(context.State, new SummonSpec(context.UserSide, "Barricade", UnitKind.Construct, EngineerSkills.Hp(context, EngineerSkills.BarricadeHp), 1, context.UserId,
            TakesTurns: false, DefinitionId: EngineerSkills.BarricadeId), place.Row, place.Index);
        context.Settle();
    }
}

/// <summary>Net Trap: place a trap (HP 4). A unit that hits it has +3 Delay on its next skill (a Saboteur with Disarm Trap sets it off without the effect).</summary>
public sealed class NetTrap : PlaceConstructSkill
{
    public NetTrap(SkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        var place = context.Placement!.Value;
        var id = Summons.Summon(context.State, new SummonSpec(context.UserSide, "Net Trap", UnitKind.Construct, EngineerSkills.Hp(context, EngineerSkills.TrapHp), 1, context.UserId,
            TakesTurns: false, DefinitionId: EngineerSkills.NetTrapId), place.Row, place.Index);
        context.State.AddReaction(new ReactionDef("net_trap", id, TriggerKind.WhenHit, (st, ctx) =>
        {
            if (ctx.Trigger.SourceUnitId is not { } hitter) return;
            if (st.GetUnit(hitter).KnownSkills.Contains("disarm_trap"))
            {
                Summons.Remove(st, id, RemovalCause.Expired);                    // set off and removed, the effect does not reach the Saboteur
                return;
            }
            Statuses.Apply(st, new StatusApplication(hitter, StatusIds.NextSkillDelay, Magnitude: EngineerSkills.TrapDelay, SourceUnitId: id));
        })
        {
            Condition = (st, t) => t.SubjectUnitId == id && t.SourceUnitId is { } src && st.GetUnit(src).Side != st.GetUnit(id).Side && !st.GetUnit(src).IsKnockedOut,
        });
        context.Settle();
    }
}

/// <summary>Reinforced: the Engineer's turrets, barricades and traps have double HP. The placing skills read this keystone.</summary>
public sealed class Reinforced : KeystoneSkill
{
    public Reinforced(SkillDef def) : base(def) { }
}

/// <summary>Self-Repair: the Engineer's turrets and barricades regain 2 HP each time the Engineer takes a turn.</summary>
public sealed class SelfRepair : KeystoneSkill
{
    public SelfRepair(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAfterActionHook((st, userId) =>
        {
            if (userId != ownerId || st.GetUnit(ownerId).IsKnockedOut) return;
            foreach (var u in st.Units.Where(u => u.SummonerId == ownerId && !u.IsKnockedOut && !u.IsRemoved && st.Formation.Contains(u.Id)
                && u.CreatureId is EngineerSkills.BoltTurretId or EngineerSkills.BarricadeId).ToArray())
                st.Heal(ownerId, u.Id, DamageMath.Scale(EngineerSkills.RepairAmount, st.PowerOf(ownerId)));
        });
}

/// <summary>Rapid Fire: Bolt Turret fires twice each time. The turret code reads this keystone.</summary>
public sealed class RapidFire : KeystoneSkill
{
    public RapidFire(SkillDef def) : base(def) { }
}

/// <summary>
/// The auto events of the Engineer's turrets (game_system.md sections 6.2, 6.7): each time the Engineer takes an action, every turret queues one auto event owned
/// by the Engineer, due now (two with Rapid Fire); the shots resolve right after the action in the order the turrets were placed. Each shot shifts Momentum like a
/// separate action. A KO'd Engineer loses its queued shots; the turrets stay.
/// </summary>
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

    /// <summary>The Engineer took an action: every turret of it queues its shots and they resolve now.</summary>
    public static void AfterAction(BattleState state, int engineerId)
    {
        if (state.IsOver || state.GetUnit(engineerId).IsKnockedOut) return;
        FireAll(state, engineerId);
    }

    /// <summary>Queues the shots of every live turret of the Engineer and resolves them now. Returns how many were queued.</summary>
    public static int FireAll(BattleState state, int engineerId)
    {
        var shots = state.GetUnit(engineerId).KnownSkills.Contains("rapid_fire") ? 2 : 1;
        var turrets = state.Units.Where(u => u.SummonerId == engineerId && EngineerSkills.IsTurret(u) && !u.IsKnockedOut && !u.IsRemoved).Select(u => u.Id).ToList();
        var queued = turrets.SelectMany(id => Enumerable.Range(0, shots).Select(_ => state.Timeline.AddNow(engineerId, EventTagPrefix + id))).ToList();
        foreach (var entry in queued)
        {
            if (state.IsOver) break;
            if (state.Timeline.Remove(entry.Id)) HandleAutoEvent(state, entry);   // false: the event was removed on the way
        }
        return queued.Count;
    }

    /// <summary>An auto event reached its turn.</summary>
    public static bool HandleAutoEvent(BattleState state, TimelineEntry entry)
    {
        if (entry.Tag is { } tag && tag.StartsWith(EventTagPrefix, StringComparison.Ordinal) && int.TryParse(tag.AsSpan(EventTagPrefix.Length), out var turretId))
        {
            Fire(state, turretId);
            return true;
        }
        return false;
    }

    private static void Fire(BattleState state, int turretId)
    {
        var turret = state.GetUnit(turretId);
        if (state.IsOver || turret.IsKnockedOut || turret.IsRemoved || turret.SummonerId is not { } engineer) return;
        var options = new TargetOptions(TargetSide.EnemySideOnly, Delivery: Delivery.Projectile);
        if (Targeting.PickNearest(state, turretId, options) is not { } target) return;       // no legal enemy: the shot is lost
        new SkillContext(state, Shot, turretId, new SkillUse(turretId, Shot.Id, target), target).Hit(target, new IntRange(2, 3));
    }
}
