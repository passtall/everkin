using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>A profession skill: Delay and Momentum cost come from the content (passives have none: 0).</summary>
public abstract class ProfessionSkill : Skill
{
    protected ProfessionSkill(ProfessionSkillDef def) : base(def.Id, def.Delay ?? 0, def.Momentum ?? 0) { }
}

/// <summary>A passive node: always on. It does its work in <see cref="Skill.Install"/>.</summary>
public abstract class PassiveSkill : ProfessionSkill
{
    protected PassiveSkill(ProfessionSkillDef def) : base(def) { }
    public sealed override bool IsPassive => true;
    public sealed override TargetKind Target => TargetKind.None;
    public sealed override void Resolve(SkillContext context) => throw new InvalidOperationException($"{Id} is a passive.");
}

/// <summary>The Hunter's skills (skills.md Hunter): Stalker, Tracker, Trapper and Predator.</summary>
public static class HunterSkills
{
    public const string DecoyDefinition = "decoy";
    public const string NetTrapDefinition = "net_trap";

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new Stalk(D("stalk"));
        yield return new CreepCloser(D("creep_closer"));
        yield return new FreezeInPlace(D("freeze_in_place"));
        yield return new LieInWait(D("lie_in_wait"));
        yield return new Vanish(D("vanish"));
        yield return new ReadTracks(D("read_tracks"));
        yield return new Scent(D("scent"));
        yield return new ScentOfBlood(D("scent_of_blood"));
        yield return new TargetSkill(D("target"));
        yield return new SniffTheAir(D("sniff_the_air"));
        yield return new Decoy(D("decoy"));
        yield return new NetTrap(D("net_trap"));
        yield return new Cage(D("cage"));
        yield return new CutOffRetreat(D("cut_off_retreat"));
        yield return new Flush(D("flush"));
        yield return new PackStrike(D("pack_strike"));
    }

    /// <summary>Marks a unit for a Hunter. A Mark has no clock of its own here; marking skills that state a duration pass one.</summary>
    internal static void Mark(BattleState state, int targetId, int hunterId, int? duration = null) =>
        Statuses.Apply(state, new StatusApplication(targetId, StatusIds.Marked, Duration: duration, SourceUnitId: hunterId));

    internal static bool IsMarkedEnemy(BattleState state, int userId, int targetId)
    {
        var t = state.GetUnit(targetId);
        return t.Side != state.GetUnit(userId).Side && Statuses.Has(t, StatusIds.Marked);
    }
}

// ------------------------------------------------------------------------------------------------ Stalker

/// <summary>Stalk: the Hunter gains stealth. Needs the Hunter outside the Front row.</summary>
public sealed class Stalk : ProfessionSkill
{
    public Stalk(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId)
    {
        if (state.Formation.PositionOf(userId).Row == Row.Front) return "needs the Hunter outside the Front row";
        return Stealth.CanGain(state, userId) ? null : "cannot gain stealth now";
    }

    public override void Resolve(SkillContext context)
    {
        Stealth.Gain(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>Vanish: gain stealth, even in the Front row.</summary>
public sealed class Vanish : ProfessionSkill
{
    public Vanish(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId) =>
        Stealth.CanGain(state, userId) ? null : "cannot gain stealth now";

    public override void Resolve(SkillContext context)
    {
        Stealth.Gain(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>Creep Closer: only while stealthed, move to an adjacent row on your side without breaking stealth.</summary>
public sealed class CreepCloser : ProfessionSkill
{
    public CreepCloser(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override EffectTags Tags => EffectTags.Move;   // a skill that moves its user (Bleeding)

    public override string? Unavailable(BattleState state, int userId) =>
        state.GetUnit(userId).IsStealthed ? null : "only while stealthed";

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId)
    {
        var p = state.Formation.PositionOf(userId);
        return new[] { (int)p.Row - 1, (int)p.Row + 1 }.Where(r => r is >= 0 and < Formation.RowCount)
            .Select(r => new RowTarget(p.Side, (Row)r)).ToArray();
    }

    public override void Resolve(SkillContext context)
    {
        // Movement inside a skill follows the forced-repositioning rules (section 9.1). Moving does not touch stealth.
        var result = Movement.ForceMove(context.State, context.UserId, context.Row!.Value.Row);
        context.AfterForcedMove(result);
    }
}

/// <summary>Freeze in Place: when an area effect hits the stealthed Hunter, 50% chance to stay stealthed (it still takes the damage).</summary>
public sealed class FreezeInPlace : PassiveSkill
{
    public FreezeInPlace(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStealthBreakVeto((st, unitId, cause, area) => unitId == ownerId && area && st.Random.Chance(50));
}

/// <summary>Lie in Wait: only while stealthed, pick a unit in melee reach. Its next skill is lost and it is Stunned for 10 time units.</summary>
public sealed class LieInWait : ProfessionSkill
{
    public LieInWait(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);

    public override string? Unavailable(BattleState state, int userId) =>
        state.GetUnit(userId).IsStealthed ? null : "only while stealthed";

    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(0, StatusIds.LieInWait));
}

// ------------------------------------------------------------------------------------------------ Tracker

/// <summary>Target: mark any visible unit. One Mark from this Hunter at a time; marking another moves it.</summary>
public sealed class TargetSkill : ProfessionSkill
{
    public TargetSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override EffectTags Tags => EffectTags.Direct | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);   // Marked is an enemy-only status

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

/// <summary>Read Tracks: pick a row. Every stealthed unit in it is revealed.</summary>
public sealed class ReadTracks : ProfessionSkill
{
    public ReadTracks(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId)
    {
        var rows = new List<RowTarget>();
        foreach (var side in new[] { Side.A, Side.B })
            for (var r = 0; r < Formation.RowCount; r++)
                if (state.Formation.Units(side, (Row)r).Any(id => !state.GetUnit(id).IsKnockedOut && Targeting.InReach(state, userId, id, Reach)))
                    rows.Add(new RowTarget(side, (Row)r));
        return rows;
    }

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).ToArray())
            Stealth.Break(context.State, id, StealthCause.Revealed);
        context.Settle();
    }
}

/// <summary>Scent: Marked enemies cannot gain stealth.</summary>
public sealed class Scent : PassiveSkill
{
    public Scent(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStealthBlocker((st, unitId) =>
            !st.GetUnit(ownerId).IsKnockedOut
            && st.GetUnit(unitId).Side != st.GetUnit(ownerId).Side
            && Statuses.Has(st.GetUnit(unitId), StatusIds.Marked));
}

/// <summary>Scent of Blood: when an enemy's HP falls below 20%, it becomes Marked.</summary>
public sealed class ScentOfBlood : PassiveSkill
{
    public ScentOfBlood(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenHit, (st, ctx) =>
            HunterSkills.Mark(st, ctx.Trigger.SubjectUnitId, ownerId))
        {
            Condition = (st, t) =>
            {
                var u = st.GetUnit(t.SubjectUnitId);
                return u.Side != st.GetUnit(ownerId).Side && !u.IsKnockedOut && u.Hp * 100 < 20 * u.MaxHp
                    && !Statuses.Has(u, StatusIds.Marked);
            },
        });
}

/// <summary>Sniff the Air: at battle start, the enemy with the highest Power becomes Marked (ties: the seeded roll).</summary>
public sealed class SniffTheAir : PassiveSkill
{
    public SniffTheAir(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        var side = state.GetUnit(ownerId).Side;
        var enemies = state.Units.Where(u => u.Side != side && !u.IsKnockedOut && !u.IsRemoved && state.Formation.Contains(u.Id)).ToList();
        if (enemies.Count == 0) return;
        var top = enemies.Max(u => u.Power);
        var best = enemies.Where(u => u.Power == top).ToList();
        var chosen = best.Count == 1 ? best[0] : best[state.Random.NextInt(best.Count)];
        HunterSkills.Mark(state, chosen.Id, ownerId);
    }
}

// ------------------------------------------------------------------------------------------------ Trapper

/// <summary>
/// Decoy: when a Marked enemy attacks the Hunter, the Hunter leaves a decoy (HP 10) in its place and appears one row behind.
/// The decoy takes the hit (decided 2026-11-08). No decoy if the Hunter is in the Rear row or cannot move back.
/// </summary>
public sealed class Decoy : PassiveSkill
{
    public Decoy(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddPreHitHook((st, attackerId, targetId) =>
        {
            if (targetId != ownerId) return null;
            var hunter = st.GetUnit(ownerId);
            if (hunter.IsKnockedOut || !HunterSkills.IsMarkedEnemy(st, ownerId, attackerId)) return null;
            var pos = st.Formation.PositionOf(ownerId);
            if (pos.Row == Row.Rear) return null;
            var back = (Row)((int)pos.Row + 1);

            // Try the move on a copy first: a blocked move would hurt units, and then there is no decoy.
            var trial = st.Clone();
            if (Movement.ForceMove(trial, ownerId, back).Status != ForcedMoveStatus.Moved) return null;
            if (!trial.Formation.CanPlace(pos.Side, pos.Row)) return null;

            Movement.ForceMove(st, ownerId, back);
            return Summons.Summon(st, new SummonSpec(pos.Side, "Decoy", UnitKind.Construct, 10, 1, ownerId, TakesTurns: false,
                DefinitionId: HunterSkills.DecoyDefinition), pos.Row, pos.Index);
        });
}

/// <summary>Net Trap: a trap card with HP 4. A unit that hits it has +3 Delay on its next skill.</summary>
public sealed class NetTrap : ProfessionSkill
{
    public NetTrap(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Placement;

    // No range is stated: the trap is placed in a free gap next to its user (section 11.2).
    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId)
    {
        var side = state.GetUnit(userId).Side;
        var allowed = Summons.PlacementOptions(state, side, new[] { state.Formation.PositionOf(userId).Row });
        return Summons.GapsNextToUser(state, userId).Where(allowed.Contains).ToArray();
    }

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place next to the Hunter (or the side is at its cap)" : null;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var place = context.Placement!.Value;
        var side = context.UserSide;
        var trap = Summons.Summon(state, new SummonSpec(side, "Net Trap", UnitKind.Construct, 4, 1, context.UserId, TakesTurns: false,
            DefinitionId: HunterSkills.NetTrapDefinition), place.Row, place.Index);

        void Slow(BattleState st, ReactionContext ctx)
        {
            if (ctx.Trigger.SourceUnitId is not { } attacker || st.GetUnit(attacker).IsKnockedOut) return;
            if (st.GetUnit(attacker).Side != st.GetUnit(trap).Side && st.GetUnit(attacker).KnownSkills.Contains("disarm_trap"))
            {
                // Disarm Trap: the trap goes off without its effect on the Rogue and is used up.
                if (!st.GetUnit(trap).IsRemoved) Summons.Remove(st, trap, RemovalCause.Expired);
                return;
            }
            Statuses.Apply(st, new StatusApplication(attacker, StatusIds.NextSkillDelay, Magnitude: 3, SourceUnitId: trap));
        }
        // A hit that destroys the trap still counts: a KO'd owner reacts only through "when KO'd", so both are registered.
        state.AddReaction(new ReactionDef("net_trap_hit", trap, TriggerKind.WhenHit, Slow));
        state.AddReaction(new ReactionDef("net_trap_ko", trap, TriggerKind.WhenKo, Slow));
        context.Settle();
    }
}

/// <summary>Cage: a unit in melee reach is Immobilized for 20 time units.</summary>
public sealed class Cage : ProfessionSkill
{
    public Cage(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(0, StatusIds.Immobilized, Duration: 20));
}

// ------------------------------------------------------------------------------------------------ Predator

/// <summary>Cut Off Retreat: a Marked unit is Immobilized for 20 time units.</summary>
public sealed class CutOffRetreat : ProfessionSkill
{
    public CutOffRetreat(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override EffectTags Tags => EffectTags.Direct | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(3);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        Statuses.Has(state.GetUnit(targetId), StatusIds.Marked);

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.Immobilized, Duration: 20, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Flush: pull a Marked unit into its own Front row (bumps apply). It loses stealth.</summary>
public sealed class Flush : ProfessionSkill
{
    public Flush(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override EffectTags Tags => EffectTags.Direct | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(3);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        Statuses.Has(state.GetUnit(targetId), StatusIds.Marked)
        && Movement.IsMovable(state.GetUnit(targetId))
        && state.Formation.PositionOf(targetId).Row != Row.Front;

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        var result = Movement.ForceMove(context.State, target, Row.Front, context.UserSide);   // an effect of this side: Hold Position stops it
        Stealth.Break(context.State, target, StealthCause.Revealed);
        context.AfterForcedMove(result);
    }
}

/// <summary>Pack Strike: every ally that hit a Marked unit since the Hunter's last turn has its next turn pulled forward 3.</summary>
public sealed class PackStrike : ProfessionSkill
{
    public PackStrike(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var allies = state.MarkedHits
            .Where(h => h.ActionIndex > state.GetUnit(context.UserId).LastActionIndex)
            .Select(h => h.AttackerId)
            .Where(id => id != context.UserId && state.GetUnit(id).Side == context.UserSide && !state.GetUnit(id).IsKnockedOut)
            .Distinct();
        foreach (var id in allies)
            if (state.Timeline.TurnOf(id) is { } turn) state.Timeline.PullForward(turn, 3);
        context.Settle();
    }
}
