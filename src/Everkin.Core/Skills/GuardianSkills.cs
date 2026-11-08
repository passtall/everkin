using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>A modifier node: it changes the skill it names and is never used as an action. The skill it modifies reads whether it is known.</summary>
public abstract class ModifierSkill : ProfessionSkill
{
    protected ModifierSkill(ProfessionSkillDef def) : base(def) { }
    public sealed override bool IsPassive => true;
    public sealed override TargetKind Target => TargetKind.None;
    public sealed override void Resolve(SkillContext context) => throw new InvalidOperationException($"{Id} is a modifier.");
}

/// <summary>The Guardian's skills (skills.md Guardian): Bulwark, Bodyguard and Keeper.</summary>
public static class GuardianSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new ShieldWall(D("shield_wall"));
        yield return new HoldPosition(D("hold_position"));
        yield return new FullCover(D("full_cover"));
        yield return new LivingWall(D("living_wall"));
        yield return new LeapToDefend(D("leap_to_defend"));
        yield return new Rescue(D("rescue"));
        yield return new TakeACharge(D("take_a_charge"));
        yield return new FollowTheCharge(D("follow_the_charge"));
        yield return new Distract(D("distract"));
        yield return new Pledge(D("pledge"));
        yield return new WardSkill(D("ward"));
        yield return new LastingWard(D("lasting_ward"));
        yield return new OverchargedWard(D("overcharged_ward"));
        yield return new ShieldDome(D("shield_dome"));
        yield return new KeeperOfTheGate(D("keeper_of_the_gate"));
    }

    /// <summary>The unit that is this Guardian's Charge, or null.</summary>
    internal static int? ChargeOf(BattleState state, int guardianId) =>
        state.Units.FirstOrDefault(u => Statuses.Get(u, StatusIds.Charge)?.SourceUnitId == guardianId && !u.IsKnockedOut && !u.IsRemoved)?.Id;

    /// <summary>Whether a unit in the row behind the Guardian is covered by it: same side, the row right behind, overlapping by at least 50%.</summary>
    internal static bool IsDirectlyBehind(BattleState state, int guardianId, int targetId)
    {
        if (!state.Formation.Contains(guardianId) || !state.Formation.Contains(targetId)) return false;
        var g = state.Formation.PositionOf(guardianId);
        var t = state.Formation.PositionOf(targetId);
        return g.Side == t.Side && (int)t.Row == (int)g.Row + 1 && state.Formation.UnitOverlapPercent(guardianId, targetId) >= 50;
    }

    /// <summary>The target options of the skills marked "Own side only".</summary>
    internal static readonly TargetOptions OwnSide = new(TargetSide.OwnSideOnly);
}

// ------------------------------------------------------------------------------------------------ Bulwark

/// <summary>
/// Shield Wall: needs 3 or more of your units side by side in the Guardian's row. For 10 time units, melee and projectile attacks aimed
/// at units behind the wall are always intercepted by the wall: the nearest wall unit that overlaps the target by at least 50% takes
/// it, with a 100% chance (decided 2026-11-08). Direct attacks pass.
/// </summary>
public sealed class ShieldWall : ProfessionSkill
{
    public ShieldWall(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId)
    {
        var p = state.Formation.PositionOf(userId);
        var wall = state.Formation.Units(p.Side, p.Row).Count(id => !state.GetUnit(id).IsKnockedOut);
        return wall >= 3 ? null : "needs 3 or more of your units side by side in the Guardian's row";
    }

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.UserId, StatusIds.ShieldWall, Duration: 10, SourceUnitId: context.UserId));
        context.Settle();
    }

    public override void Install(BattleState state, int ownerId) =>
        state.AddGuaranteedInterceptor((st, targetId) =>
        {
            var guardian = st.GetUnit(ownerId);
            if (guardian.IsKnockedOut || !Statuses.Has(guardian, StatusIds.ShieldWall) || !st.Formation.Contains(targetId)) return null;
            var g = st.Formation.PositionOf(ownerId);
            var t = st.Formation.PositionOf(targetId);
            if (t.Side != g.Side || (int)t.Row <= (int)g.Row) return null;            // only units behind the wall
            return st.Formation.Units(g.Side, g.Row)
                .Where(id => id != targetId && Interception.IsEligible(st, st.GetUnit(id)) && st.Formation.UnitOverlapPercent(id, targetId) >= 50)
                .OrderByDescending(id => st.Formation.UnitOverlapPercent(id, targetId))
                .Cast<int?>().FirstOrDefault();
        });
}

/// <summary>Hold Position: enemy effects cannot push, pull or swap the Guardian.</summary>
public sealed class HoldPosition : PassiveSkill
{
    public HoldPosition(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddForcedMoveImmunity((st, unitId, causedBy) => unitId == ownerId && causedBy != st.GetUnit(ownerId).Side);
}

/// <summary>Full Cover: when the Guardian intercepts, it always takes the whole hit, never a partial one.</summary>
public sealed class FullCover : PassiveSkill
{
    public FullCover(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddFullHitRule((st, interceptorId) => interceptorId == ownerId);
}

/// <summary>Living Wall (keystone): the Guardian always intercepts for the units directly behind it, but it can never attack.</summary>
public sealed class LivingWall : PassiveSkill
{
    public LivingWall(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddGuaranteedInterceptor((st, targetId) =>
            !st.GetUnit(ownerId).IsKnockedOut && GuardianSkills.IsDirectlyBehind(st, ownerId, targetId) ? ownerId : null);
        state.AddAttackBan((st, unitId) => unitId == ownerId);
    }
}

/// <summary>
/// Leap to Defend: own side only. The Guardian moves next to an ally anywhere on your side (any gap) and takes the next attack of any
/// kind aimed at that ally instead of it, once; it ends if the Guardian is KO'd (decided 2026-11-08).
/// </summary>
public sealed class LeapToDefend : ProfessionSkill
{
    public LeapToDefend(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override TargetOptions Options => GuardianSkills.OwnSide;
    public override EffectTags Tags => EffectTags.Move;   // a skill that moves its user (Bleeding)

    public override bool TargetFilter(BattleState state, int userId, int targetId) => targetId != userId;

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId)
    {
        var me = state.Formation.PositionOf(userId);
        var ally = state.Formation.PositionOf(targetId);
        if (me.Row == ally.Row)
            // Already in the ally's row: only if it already stands next to it (rows cannot be reordered by a move).
            return Math.Abs(me.Index - ally.Index) == 1 ? new[] { new MoveOption(me.Row, me.Index) } : Array.Empty<MoveOption>();
        if (!state.Formation.CanPlace(ally.Side, ally.Row)) return Array.Empty<MoveOption>();
        return new[] { new MoveOption(ally.Row, ally.Index), new MoveOption(ally.Row, ally.Index + 1) };
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var me = state.Formation.PositionOf(context.UserId);
        var place = context.Placement!.Value;
        if (!(place.Row == me.Row && place.Index == me.Index))
        {
            state.Ground.Accrue();
            var from = state.Formation.Remove(context.UserId);
            state.Formation.Place(context.UserId, from.Side, place.Row, place.Index);
            state.Emit(new UnitMoved(context.UserId, from, state.Formation.PositionOf(context.UserId), MoveCause.Forced));
            foreach (var hook in state.AfterMoveHooks.ToArray()) hook(state, context.UserId);
        }
        Statuses.Apply(state, new StatusApplication(context.TargetId!.Value, StatusIds.LeapCover, SourceUnitId: context.UserId));
        context.Settle();
    }

    public override void Install(BattleState state, int ownerId) =>
        state.AddPreHitHook((st, attackerId, targetId) =>
        {
            var cover = Statuses.Get(st.GetUnit(targetId), StatusIds.LeapCover);
            if (cover?.SourceUnitId != ownerId || targetId == ownerId) return null;
            var guardian = st.GetUnit(ownerId);
            if (guardian.IsKnockedOut || !st.Formation.Contains(ownerId)) return null;
            Statuses.Remove(st, targetId, StatusIds.LeapCover, StatusEndCause.Consumed);   // once
            return ownerId;
        });
}

// ------------------------------------------------------------------------------------------------ Bodyguard

/// <summary>Rescue: own side only. Swap places with an ally within distance 2 that is below half HP.</summary>
public sealed class Rescue : ProfessionSkill
{
    public Rescue(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(2);
    public override TargetOptions Options => GuardianSkills.OwnSide;

    public override bool TargetFilter(BattleState state, int userId, int targetId)
    {
        var t = state.GetUnit(targetId);
        return targetId != userId && t.Hp * 2 < t.MaxHp && Movement.IsMovable(t) && Movement.IsMovable(state.GetUnit(userId));
    }

    public override void Resolve(SkillContext context)
    {
        Movement.Swap(context.State, context.UserId, context.TargetId!.Value, context.UserSide);
        context.Settle();
    }
}

/// <summary>Take a Charge: own side only. An ally becomes the Guardian's Charge (one at a time; choosing another moves it).</summary>
public sealed class TakeACharge : ProfessionSkill
{
    public TakeACharge(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => GuardianSkills.OwnSide;

    public override bool TargetFilter(BattleState state, int userId, int targetId) => targetId != userId;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        foreach (var unit in state.Units.ToArray())
            if (unit.Id != context.TargetId && Statuses.Get(unit, StatusIds.Charge)?.SourceUnitId == context.UserId)
                Statuses.Remove(state, unit.Id, StatusIds.Charge, StatusEndCause.Consumed);
        Statuses.Apply(state, new StatusApplication(context.TargetId!.Value, StatusIds.Charge, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>
/// Follow the Charge: when the Charge moves or is moved, the Guardian moves with it to stay lined up in front of it. The move is automatic
/// and free: right after the Charge moves, the Guardian moves into the row in front of it, at the Charge's coordinate, if the row has
/// room and that spot is free (decided 2026-11-08).
/// </summary>
public sealed class FollowTheCharge : PassiveSkill
{
    public FollowTheCharge(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAfterMoveHook((st, movedId) =>
        {
            if (Statuses.Get(st.GetUnit(movedId), StatusIds.Charge)?.SourceUnitId != ownerId) return;
            var guardian = st.GetUnit(ownerId);
            if (guardian.IsKnockedOut || !Movement.IsMovable(guardian) || !st.Formation.Contains(ownerId)) return;
            var charge = st.Formation.PositionOf(movedId);
            var me = st.Formation.PositionOf(ownerId);
            if (charge.Side != me.Side || charge.Row == Row.Front) return;
            var front = (Row)((int)charge.Row - 1);
            if (me.Row == front) return;                                             // already in front of it

            // Lined up at the Charge's coordinate: the gap where that coordinate falls; refused if a unit already stands exactly there or the row is full.
            var coordinate = st.Formation.CenterOf(charge);
            var row = st.Formation.Units(me.Side, front);
            var n = row.Count;
            if (n >= Formation.RowCapacity || Enumerable.Range(0, n).Any(i => Formation.Center(n, i) == coordinate)) return;
            var index = Enumerable.Range(0, n).Count(i => Formation.Center(n, i) < coordinate);

            st.Ground.Accrue();
            var from = st.Formation.Remove(ownerId);
            st.Formation.Place(ownerId, me.Side, front, index);
            st.Emit(new UnitMoved(ownerId, from, st.Formation.PositionOf(ownerId), MoveCause.Forced));
        });
}

/// <summary>
/// Distract: the enemy that last hit the Charge must target the Guardian with its next attack, if it can. For that enemy's next damaging
/// skill that takes a target, the Guardian is the only legal target if it is legal; it ends after that attack or if the Guardian is KO'd
/// (decided 2026-11-08).
/// </summary>
public sealed class Distract : ProfessionSkill
{
    public Distract(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    private static int? Culprit(BattleState state, int userId)
    {
        if (GuardianSkills.ChargeOf(state, userId) is not { } charge) return null;
        var hitter = state.GetUnit(charge).LastHitBy;
        if (hitter is not { } h) return null;
        var enemy = state.GetUnit(h);
        return enemy.Side != state.GetUnit(userId).Side && !enemy.IsKnockedOut && !enemy.IsRemoved ? h : null;
    }

    public override string? Unavailable(BattleState state, int userId) =>
        GuardianSkills.ChargeOf(state, userId) is null ? "needs a Charge"
        : Culprit(state, userId) is null ? "no enemy has hit the Charge" : null;

    public override void Resolve(SkillContext context)
    {
        if (Culprit(context.State, context.UserId) is { } enemy)
            Statuses.Apply(context.State, new StatusApplication(enemy, StatusIds.Distracted, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>
/// Pledge: for 10 time units, area effects that would hit the Charge hit the Guardian instead: the Charge is left out of the area and the
/// Guardian is hit in its place, once even if it is already in the area (decided 2026-11-08).
/// </summary>
public sealed class Pledge : ProfessionSkill
{
    public Pledge(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId) =>
        GuardianSkills.ChargeOf(state, userId) is null ? "needs a Charge" : null;

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.UserId, StatusIds.Pledge, Duration: 10, SourceUnitId: context.UserId));
        context.Settle();
    }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAreaRedirect((st, targetId) =>
        {
            var guardian = st.GetUnit(ownerId);
            if (guardian.IsKnockedOut || !Statuses.Has(guardian, StatusIds.Pledge)) return null;
            return Statuses.Get(st.GetUnit(targetId), StatusIds.Charge)?.SourceUnitId == ownerId ? ownerId : null;
        });
}

// ------------------------------------------------------------------------------------------------ Keeper

/// <summary>Ward: a unit within distance 2 gains a Ward of 4 until the Guardian's next turn (until broken with Lasting Ward).</summary>
public sealed class WardSkill : ProfessionSkill
{
    public WardSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(2);

    public override void Resolve(SkillContext context)
    {
        var lasting = context.User.KnownSkills.Contains("lasting_ward");
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.Ward, Magnitude: context.Scaled(4),
            SourceUnitId: context.UserId, EndsAtNextTurnOf: lasting ? null : context.UserId));
        context.Settle();
    }
}

/// <summary>Lasting Ward (modifier of Ward): the Ward lasts until it is broken, not until the Guardian's next turn.</summary>
public sealed class LastingWard : ModifierSkill
{
    public LastingWard(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Overcharged Ward: a unit gains a Ward of 8 that lasts only until that unit's next turn.</summary>
public sealed class OverchargedWard : ProfessionSkill
{
    public OverchargedWard(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        Statuses.Apply(context.State, new StatusApplication(target, StatusIds.Ward, Magnitude: context.Scaled(8),
            SourceUnitId: context.UserId, EndsAtNextTurnOf: target));
        context.Settle();
    }
}

/// <summary>Shield Dome: every ally gains a Ward of 2 until the Guardian's next turn.</summary>
public sealed class ShieldDome : ProfessionSkill
{
    public ShieldDome(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        foreach (var id in context.State.Formation.Units(context.UserSide).ToArray())
            if (!context.State.GetUnit(id).IsKnockedOut)
                Statuses.Apply(context.State, new StatusApplication(id, StatusIds.Ward, Magnitude: context.Scaled(2),
                    SourceUnitId: context.UserId, EndsAtNextTurnOf: context.UserId));
        context.Settle();
    }
}

/// <summary>
/// Keeper of the Gate (keystone): your barricades have double HP, but the Guardian itself cannot intercept. The barricade HP is applied
/// where barricades are made (the Tinker's Barricade).
/// </summary>
public sealed class KeeperOfTheGate : PassiveSkill
{
    public KeeperOfTheGate(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddInterceptionBlocker((st, unitId) => unitId == ownerId);
}
