using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Bulwark (skills.md): stands in front of allies and takes their hits. Physical.</summary>
public static class BulwarkSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new LeapToDefend(D("leap_to_defend"));
        yield return new Rescue(D("rescue"));
        yield return new ShieldBash(D("shield_bash"));
        yield return new ShieldWall(D("shield_wall"));
        yield return new HoldPosition(D("hold_position"));
        yield return new Unbreakable(D("unbreakable"));
    }

    /// <summary>The target options of the skills marked "Own side only".</summary>
    internal static readonly TargetOptions OwnSide = new(TargetSide.OwnSideOnly);
}

/// <summary>
/// Leap to Defend: own side only. The Bulwark moves in front of an ally (any gap of the row in front of it) and takes the next attack of any delivery aimed
/// at that ally instead, once. Only when nothing is free in front (or the ally stands in the Front row) it goes next to the ally, on a side picked by the seeded
/// roll; when neither is possible the skill cannot be used on that ally. It ends if the Bulwark is KO.
/// </summary>
public sealed class LeapToDefend : ClassSkill
{
    public LeapToDefend(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override TargetOptions Options => BulwarkSkills.OwnSide;
    public override EffectTags Tags => base.Tags | EffectTags.Move;           // a skill that moves its own user (Bleeding)
    public override bool PlacementOptional => true;

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        targetId != userId && (InFront(state, userId, targetId).Count > 0 || Beside(state, userId, targetId).Count > 0);

    /// <summary>The gaps in the row in front of the ally (the player picks one); the first choice.</summary>
    private static IReadOnlyList<MoveOption> InFront(BattleState state, int userId, int targetId)
    {
        var me = state.Formation.PositionOf(userId);
        var ally = state.Formation.PositionOf(targetId);
        if (ally.Row == Row.Front) return Array.Empty<MoveOption>();
        var front = (Row)((int)ally.Row - 1);
        if (me.Row == front) return new[] { new MoveOption(me.Row, me.Index) };      // already in front of the ally
        if (!state.Formation.CanPlace(ally.Side, front)) return Array.Empty<MoveOption>();
        return Enumerable.Range(0, state.Formation.Count(ally.Side, front) + 1).Select(i => new MoveOption(front, i)).ToArray();
    }

    /// <summary>The places next to the ally in its own row, the fallback when nothing is free in front of it (the seeded roll picks the side).</summary>
    private static IReadOnlyList<MoveOption> Beside(BattleState state, int userId, int targetId)
    {
        var me = state.Formation.PositionOf(userId);
        var ally = state.Formation.PositionOf(targetId);
        if (me.Row == ally.Row) return Math.Abs(me.Index - ally.Index) == 1 ? new[] { new MoveOption(me.Row, me.Index) } : Array.Empty<MoveOption>();
        if (!state.Formation.CanPlace(ally.Side, ally.Row)) return Array.Empty<MoveOption>();
        return new[] { new MoveOption(ally.Row, ally.Index), new MoveOption(ally.Row, ally.Index + 1) };
    }

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId) => InFront(state, userId, targetId);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var me = state.Formation.PositionOf(context.UserId);
        var place = context.Placement;
        if (place is null)
        {
            var beside = Beside(state, context.UserId, context.TargetId!.Value);
            if (beside.Count > 0) place = beside[state.Random.NextInt(beside.Count)];
        }
        if (place is { } to && !(to.Row == me.Row && to.Index == me.Index))
        {
            state.Ground.Accrue();
            var from = state.Formation.Remove(context.UserId);
            state.Formation.Place(context.UserId, from.Side, to.Row, to.Index);
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

/// <summary>Rescue: own side only. Swap places with an ally below half HP.</summary>
public sealed class Rescue : ClassSkill
{
    public Rescue(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => BulwarkSkills.OwnSide;
    public override EffectTags Tags => base.Tags | EffectTags.Move;

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

/// <summary>Shield Bash: hit for 6-7 and push the target one row back (bumps apply). A unit that took the hit instead gets the push.</summary>
public sealed class ShieldBash : ClassSkill
{
    public ShieldBash(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var report = context.Hit(context.TargetId!.Value, new IntRange(6, 7));
        if (report.Outcome == HitOutcome.Missed || context.Over) return;
        var receiver = report.EffectReceiverId;
        if (state.GetUnit(receiver).IsKnockedOut || !state.Formation.Contains(receiver)) return;
        var row = state.Formation.PositionOf(receiver).Row;
        if (row == Row.Rear) return;
        context.AfterForcedMove(Movement.ForceMove(state, receiver, (Row)((int)row + 1), context.UserSide));
    }
}

/// <summary>
/// Shield Wall: while 2 or more of your units with Shield Wall stand side by side in one row, a melee or projectile attack on a unit behind them is intercepted by
/// the nearest wall unit that overlaps that unit by at least 50%, with 100% chance instead of the roll. The hit is still full or partial as usual. Direct attacks pass.
/// </summary>
public sealed class ShieldWall : KeystoneSkill
{
    public ShieldWall(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddGuaranteedInterceptor((st, targetId) => Designated(st, targetId) is { } who && who == ownerId ? who : null);

    private static bool IsWallMember(BattleState st, int id) =>
        !st.GetUnit(id).IsKnockedOut && st.GetUnit(id).KnownSkills.Contains("shield_wall");

    /// <summary>The wall unit that intercepts for the target, or null: rows nearest the battle line first, the wall unit with the most overlap.</summary>
    internal static int? Designated(BattleState st, int targetId)
    {
        if (!st.Formation.Contains(targetId)) return null;
        var t = st.Formation.PositionOf(targetId);
        for (var r = 0; r < (int)t.Row; r++)
        {
            var row = st.Formation.Units(t.Side, (Row)r);
            var inWall = new bool[row.Count];
            for (var i = 0; i < row.Count; i++)
                if (IsWallMember(st, row[i]) && ((i > 0 && IsWallMember(st, row[i - 1])) || (i + 1 < row.Count && IsWallMember(st, row[i + 1])))) inWall[i] = true;
            int? best = null;
            var bestOverlap = 0;
            for (var i = 0; i < row.Count; i++)
            {
                if (!inWall[i] || !Interception.IsEligible(st, st.GetUnit(row[i]))) continue;
                var overlap = st.Formation.UnitOverlapPercent(row[i], targetId);
                if (overlap >= 50 && overlap > bestOverlap) { best = row[i]; bestOverlap = overlap; }
            }
            if (best is not null) return best;
        }
        return null;
    }
}

/// <summary>Hold Position: enemy effects cannot push, pull or swap the Bulwark.</summary>
public sealed class HoldPosition : KeystoneSkill
{
    public HoldPosition(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddForcedMoveImmunity((st, unitId, causedBy) => unitId == ownerId && causedBy != st.GetUnit(ownerId).Side);
}

/// <summary>Unbreakable: the first time the Bulwark would be KO, it keeps 1 HP instead.</summary>
public sealed class Unbreakable : KeystoneSkill
{
    public Unbreakable(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddLethalSave((st, targetId) => targetId == ownerId && st.ConsumeOnce("unbreakable:" + ownerId));
}
