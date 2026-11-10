using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Warrior's skills (skills.md Warrior): Berserker, Champion, Whirlwind and Footwork. The Warrior's actives are physical (not
/// `magical`). The rulings of 2026-11-10 are in skills.md.
/// </summary>
public static class WarriorSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new RecklessSwing(D("reckless_swing"));
        yield return new Unstoppable(D("unstoppable"));
        yield return new Endure(D("endure"));
        yield return new LastStand(D("last_stand"));
        yield return new Challenge(D("challenge"));
        yield return new DragOut(D("drag_out"));
        yield return new Isolate(D("isolate"));
        yield return new HuntDown(D("hunt_down"));
        yield return new Execution(D("execution"));
        yield return new SingleCombat(D("single_combat"));
        yield return new SweepingKick(D("sweeping_kick"));
        yield return new CrowdFighter(D("crowd_fighter"));
        yield return new BattleRoar(D("battle_roar"));
        yield return new Sidestep(D("sidestep"));
        yield return new CounterStep(D("counter_step"));
        yield return new HitAndRun(D("hit_and_run"));
    }

    /// <summary>Whether a unit is below a quarter of its maximum HP.</summary>
    internal static bool BelowQuarter(Unit u) => u.Hp * 4 < u.MaxHp;

    /// <summary>The enemies within distance 1 of the unit, by the targeting rules (stealthed units left out).</summary>
    internal static IReadOnlyList<int> EnemiesInMeleeReach(BattleState state, int unitId, Delivery? delivery = null) =>
        Targeting.LegalTargets(state, unitId, Reach.Distance(1), new TargetOptions(TargetSide.EnemySideOnly, Delivery: delivery));
}

/// <summary>Base of the Warrior's targeted actives: a Marked enemy within the skill's reach.</summary>
public abstract class WarriorSkill : ProfessionSkill
{
    protected WarriorSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
}

/// <summary>A Warrior active that needs a Marked enemy as its target.</summary>
public abstract class MarkedTargetSkill : WarriorSkill
{
    protected MarkedTargetSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);
    public override bool TargetFilter(BattleState state, int userId, int targetId) => HunterSkills.IsMarkedEnemy(state, userId, targetId);
}

// ------------------------------------------------------------------------------------------------ Berserker

/// <summary>Reckless Swing: the Warrior's hits deal double damage, but it takes half of the damage it deals (rounded down).</summary>
public sealed class RecklessSwing : PassiveSkill
{
    public RecklessSwing(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddDamageBonus((st, attacker, target) => attacker == ownerId ? 100 : 0);
        state.AddPostHitHook((st, attacker, target, hpLost) =>
        {
            var owner = st.GetUnit(ownerId);
            var recoil = hpLost / 2;
            if (attacker == ownerId && recoil > 0 && !owner.IsKnockedOut)
                st.DealDamage(ownerId, ownerId, recoil, DamageSource.Unreducible);
        });
    }
}

/// <summary>Unstoppable: below a quarter of its HP the Warrior cannot be Immobilized or Frozen and its turns cannot be pushed back.</summary>
public sealed class Unstoppable : PassiveSkill
{
    public Unstoppable(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddStatusImmunity((st, unitId, statusId) =>
            unitId == ownerId && WarriorSkills.BelowQuarter(st.GetUnit(ownerId)) && statusId is StatusIds.Immobilized or StatusIds.Frozen);
        state.AddTurnPushImmunity((st, unitId) => unitId == ownerId && WarriorSkills.BelowQuarter(st.GetUnit(ownerId)));
    }
}

/// <summary>Endure: damage from statuses cannot drop the Warrior below 1 HP.</summary>
public sealed class Endure : PassiveSkill
{
    public Endure(ProfessionSkillDef def) : base(def) { }
    public override void Install(BattleState state, int ownerId) => state.AddDamageFloor((st, unitId) => unitId == ownerId ? 1 : 0);
}

/// <summary>Last Stand (keystone): the Warrior cannot Move or be moved, but deals 50% more damage.</summary>
public sealed class LastStand : PassiveSkill
{
    public LastStand(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddForcedMoveImmunity((st, unitId, causedBy) => unitId == ownerId);       // from any side, its own skills included
        state.AddMoveBan((st, unitId) => unitId == ownerId);
        state.AddDamageBonus((st, attacker, target) => attacker == ownerId ? 50 : 0);
    }
}

// ------------------------------------------------------------------------------------------------ Champion

/// <summary>Challenge: Mark a unit. One Mark from this Warrior at a time; marking another moves it.</summary>
public sealed class Challenge : WarriorSkill
{
    public Challenge(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);       // Marked is an enemy-only status

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        foreach (var unit in state.Units.ToArray())
            if (unit.Id != context.TargetId && Statuses.Get(unit, StatusIds.Marked) is { } m && m.SourceUnitId == context.UserId)
                Statuses.Remove(state, unit.Id, StatusIds.Marked, StatusEndCause.Consumed);
        HunterSkills.Mark(state, context.TargetId!.Value, context.UserId);
        context.Settle();
    }
}

/// <summary>Drag Out: pull a Marked unit one row toward the battle line. A melee effect: an interceptor takes it instead.</summary>
public sealed class DragOut : MarkedTargetSkill
{
    public DragOut(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var receiver = context.InterceptEffect(context.TargetId!.Value);
        var position = state.Formation.PositionOf(receiver);
        if (position.Row == Row.Front || !Movement.IsMovable(state.GetUnit(receiver))) { context.Settle(); return; }
        context.AfterForcedMove(Movement.ForceMove(state, receiver, (Row)((int)position.Row - 1), context.UserSide));
    }
}

/// <summary>Isolate: push the units to the left and right of a Marked unit one row back as a group, leaving it alone.</summary>
public sealed class Isolate : MarkedTargetSkill
{
    public Isolate(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        var position = state.Formation.PositionOf(target);
        var row = state.Formation.Units(position.Side, position.Row);
        var neighbours = new List<int>();
        if (position.Index > 0) neighbours.Add(row[position.Index - 1]);
        if (position.Index < row.Count - 1) neighbours.Add(row[position.Index + 1]);
        context.PushGroupBack(neighbours);
    }
}

/// <summary>Hunt Down: when a Marked enemy is moved, the Warrior Stuns it for 5 time units.</summary>
public sealed class HuntDown : PassiveSkill
{
    public const int StunLength = 5;
    public HuntDown(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenMoved, (st, ctx) =>
        {
            var subject = st.GetUnit(ctx.Trigger.SubjectUnitId);
            if (!subject.IsKnockedOut)
                Statuses.Apply(st, new StatusApplication(subject.Id, StatusIds.Stunned, StunLength, SourceUnitId: ownerId));
        })
        {
            Condition = (st, t) => HunterSkills.IsMarkedEnemy(st, ownerId, t.SubjectUnitId) && !st.GetUnit(t.SubjectUnitId).IsKnockedOut,
        });
}

/// <summary>Execution: a Marked unit below 25% HP is KO. Not a roll: it cannot be intercepted and ignores shields.</summary>
public sealed class Execution : MarkedTargetSkill
{
    public Execution(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override bool IsAttack => true;
    public override Reach Reach => Reach.Distance(1);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        base.TargetFilter(state, userId, targetId) && WarriorSkills.BelowQuarter(state.GetUnit(targetId));

    public override void Resolve(SkillContext context) => context.Execute(context.TargetId!.Value);
}

/// <summary>Single Combat: for 10 time units the Warrior and one Marked unit can be targeted only by each other.</summary>
public sealed class SingleCombat : MarkedTargetSkill
{
    public const int Length = 10;
    public SingleCombat(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Install(BattleState state, int ownerId) =>
        state.AddTargetRestriction((st, userId, targetId) =>
        {
            if (userId == targetId || Statuses.Get(st.GetUnit(targetId), StatusIds.Duel) is not { } duel) return true;
            var partner = st.GetUnit(duel.Magnitude);
            return partner.IsKnockedOut || userId == partner.Id;                      // the duel is over when a duellist is KO
        });

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var enemy = context.TargetId!.Value;
        foreach (var id in new[] { context.UserId, enemy })                           // an old duel ends for its other duellist
            if (Statuses.Get(state.GetUnit(id), StatusIds.Duel) is { } old && old.Magnitude != context.UserId && old.Magnitude != enemy
                && !state.GetUnit(old.Magnitude).IsKnockedOut)
                Statuses.Remove(state, old.Magnitude, StatusIds.Duel, StatusEndCause.Consumed);
        Statuses.Apply(state, new StatusApplication(context.UserId, StatusIds.Duel, Length, enemy, context.UserId));
        Statuses.Apply(state, new StatusApplication(enemy, StatusIds.Duel, Length, context.UserId, context.UserId));
        context.Settle();
    }
}

// ------------------------------------------------------------------------------------------------ Whirlwind

/// <summary>Sweeping Kick: push every enemy in melee reach one row back as a group (bumps apply).</summary>
public sealed class SweepingKick : WarriorSkill
{
    public SweepingKick(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context) =>
        context.PushGroupBack(WarriorSkills.EnemiesInMeleeReach(context.State, context.UserId, Battle.Delivery.Melee));
}

/// <summary>Crowd Fighter: while three or more enemies are in melee reach, the Warrior's attacks hit all of them.</summary>
public sealed class CrowdFighter : PassiveSkill
{
    public const int Needed = 3;
    public CrowdFighter(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAttackExpander((st, attacker) =>
        {
            if (attacker != ownerId) return Array.Empty<int>();
            var enemies = WarriorSkills.EnemiesInMeleeReach(st, ownerId);
            return enemies.Count >= Needed ? enemies : Array.Empty<int>();
        });
}

/// <summary>Battle Roar: every enemy in melee reach has its next turn pushed back 5 time units.</summary>
public sealed class BattleRoar : WarriorSkill
{
    public const int PushLength = 5;
    public BattleRoar(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        foreach (var id in WarriorSkills.EnemiesInMeleeReach(context.State, context.UserId))
            context.State.PushTurnBack(id, PushLength);
        context.Settle();
    }
}

// ------------------------------------------------------------------------------------------------ Footwork

/// <summary>Sidestep: the next attack on the Warrior misses.</summary>
public sealed class Sidestep : WarriorSkill
{
    public Sidestep(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.UserId, StatusIds.Sidestep, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Counter Step: when an attack on the Warrior misses, its next turn is pulled forward 5 time units.</summary>
public sealed class CounterStep : PassiveSkill
{
    public const int PullLength = 5;
    public CounterStep(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAttackMissHook((st, attacker, target) =>
        {
            if (target == ownerId && !st.GetUnit(ownerId).IsKnockedOut && st.Timeline.TurnOf(ownerId) is { } turn)
                st.Timeline.PullForward(turn, PullLength);
        });
}

/// <summary>Hit and Run: strike a unit in melee for 3–4; its next action comes 3 time units later; then the Warrior moves one row back.</summary>
public sealed class HitAndRun : WarriorSkill
{
    public HitAndRun(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override bool IsAttack => true;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var report = context.Hit(context.TargetId!.Value, new IntRange(3, 4));
        if (report.Outcome != HitOutcome.Missed && !state.GetUnit(report.EffectReceiverId).IsKnockedOut)
            state.PushTurnBack(report.EffectReceiverId, 3);
        if (context.Over || context.User.IsKnockedOut) return;
        var position = state.Formation.PositionOf(context.UserId);
        if (position.Row != Row.Rear)
            context.AfterForcedMove(Movement.ForceMove(state, context.UserId, (Row)((int)position.Row + 1), context.UserSide));
    }
}
