using Everkin.Core.Battle;

namespace Everkin.Core.Skills;

/// <summary>What a skill would do to one unit, in the lowest and highest case.</summary>
/// <param name="DamageMin">Damage the unit is dealt before shields (Ward, Dampen), summed over the action's hits on it.</param>
/// <param name="HpLostMin">What actually reaches HP after shields.</param>
/// <param name="HealMin">HP restored.</param>
public sealed record UnitPreview(int UnitId, int DamageMin, int DamageMax, int HpLostMin, int HpLostMax, int HealMin, int HealMax);

/// <summary>A unit that may step in for the target and the chance that it does (50 or 25).</summary>
public sealed record InterceptorPreview(int UnitId, int Percent);

/// <summary>An entry of the timeline whose time would change (a pull, a push, Haste, Swap Turns).</summary>
public sealed record TimelineShift(int EntryId, int? UnitId, int From, int To);

/// <summary>
/// What a skill would do, as the UI and the AI need it (production.md section 2.2, ui_spec "Target preview", "Forced movement preview").
/// It is computed by running the real rules on a clone of the state with every roll at its lowest and then at its highest value and
/// with every chance failing, so it can never disagree with execution and never changes the state it is given. Choices that depend on
/// a roll (which of several equally near units is picked, which random status goes) are shown as the lowest-roll choice.
/// </summary>
public sealed record SkillPreview(
    string SkillId,
    string? IllegalReason,
    IReadOnlyList<UnitPreview> Units,
    int SuccessChance,
    IReadOnlyList<InterceptorPreview> Interceptors,
    int MomentumCost,
    int MomentumBefore,
    int MomentumAfterCost,
    int NextTurnDue,
    int NextTurnPosition,
    IReadOnlyList<TimelineShift> TimelineShifts,
    IReadOnlyList<UnitMoved> Moves,
    IReadOnlyList<BumpBlocked> BlockedChains,
    IReadOnlyList<int> BumpDamaged,
    IReadOnlyList<BattleEvent> LowEvents)
{
    public bool IsLegal => IllegalReason is null;
}

/// <summary>Preview queries (M1.18): read-only answers computed with the same code that resolves actions.</summary>
public static class Preview
{
    /// <summary>Previews a skill use. When the use is not legal, only <see cref="SkillPreview.IllegalReason"/> is filled.</summary>
    public static SkillPreview Skill(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        var why = Actions.WhyNot(state, catalog, use);
        if (why is not null) return Illegal(use.SkillId, why);
        var skill = catalog.Get(use.SkillId);
        var user = state.GetUnit(use.UserId);

        var low = Simulate(state, Mode.Lowest, s => Actions.Use(s, catalog, use), use.UserId, out var lowState);
        var high = Simulate(state, Mode.Highest, s => Actions.Use(s, catalog, use), use.UserId, out _);

        var ids = low.Concat(high).SelectMany(Affected).Distinct().ToList();
        var units = ids.Select(id => new UnitPreview(id,
            Damage(low, id, e => e.Result.Incoming), Damage(high, id, e => e.Result.Incoming),
            Damage(low, id, e => e.Result.HpLost), Damage(high, id, e => e.Result.HpLost),
            Healed(low, id), Healed(high, id))).ToList();

        var interceptors = new List<InterceptorPreview>();
        if (skill.Delivery is { } delivery && Interception.CanBeIntercepted(delivery) && use.TargetUnitId is { } target
            && !(skill.IsAttack && Statuses.Get(user, StatusIds.Phased) is { Magnitude: > 0 }))
            foreach (var defender in Interception.Defenders(state, target))
                interceptors.Add(new InterceptorPreview(defender, Interception.ChanceFor(state.Formation.UnitOverlapPercent(defender, target))));

        var success = 100;
        if (skill.IsAttack)
        {
            success -= Statuses.AttackMissChance(user) * success / 100;
            if (use.TargetUnitId is { } t && Statuses.Has(state.GetUnit(t), StatusIds.Sidestep)) success = 0;
        }

        var cost = Actions.CostOf(state, use.UserId, skill);
        var after = low.OfType<MomentumChanged>().FirstOrDefault(m => m.Cause == MomentumCause.Cost)?.After ?? state.Momentum.Value;
        var turn = lowState.Timeline.TurnOf(use.UserId);
        var upcoming = lowState.Timeline.Upcoming().ToList();

        return new SkillPreview(
            skill.Id, null, units, success, interceptors, cost, state.Momentum.Value, after,
            turn?.Due ?? -1, turn is null ? -1 : upcoming.FindIndex(e => e.Id == turn.Id),
            Shifts(state, lowState, use.UserId),
            low.OfType<UnitMoved>().ToList(), low.OfType<BumpBlocked>().ToList(),
            low.OfType<DamageDealt>().Where(d => d.Source == DamageSource.Unreducible).Select(d => d.TargetUnitId).Distinct().ToList(),
            low);
    }

    /// <summary>Where the unit's next turn lands if it takes Skip Turn (position 0 is the next turn on the timeline).</summary>
    public static (int Due, int Position) SkipTurn(BattleState state, int userId)
    {
        Simulate(state, Mode.Lowest, s => Actions.SkipTurn(s, userId), userId, out var after);
        return Landing(after, userId);
    }

    /// <summary>Where the unit's next turn lands if it Moves to the given gap.</summary>
    public static (int Due, int Position) Move(BattleState state, int userId, Row row, int gap)
    {
        Simulate(state, Mode.Lowest, s => Actions.Move(s, userId, row, gap), userId, out var after);
        return Landing(after, userId);
    }

    // ---------------------------------------------------------------- helpers

    private enum Mode { Lowest, Highest }

    private static SkillPreview Illegal(string skillId, string why) =>
        new(skillId, why, Array.Empty<UnitPreview>(), 0, Array.Empty<InterceptorPreview>(), 0, 0, 0, -1, -1,
            Array.Empty<TimelineShift>(), Array.Empty<UnitMoved>(), Array.Empty<BumpBlocked>(), Array.Empty<int>(), Array.Empty<BattleEvent>());

    private static IReadOnlyList<BattleEvent> Simulate(BattleState state, Mode mode, Func<BattleState, IReadOnlyList<BattleEvent>> action, int userId, out BattleState clone)
    {
        clone = state.Clone();
        clone.Random.Mode = mode == Mode.Lowest ? BattleRandom.RollMode.Lowest : BattleRandom.RollMode.Highest;
        return action(clone);
    }

    private static (int Due, int Position) Landing(BattleState after, int userId)
    {
        var turn = after.Timeline.TurnOf(userId);
        return turn is null ? (-1, -1) : (turn.Due, after.Timeline.Upcoming().ToList().FindIndex(e => e.Id == turn.Id));
    }

    private static IEnumerable<int> Affected(BattleEvent e) => e switch
    {
        DamageDealt d => new[] { d.TargetUnitId },
        Healed h => new[] { h.TargetUnitId },
        StatusApplied s => new[] { s.UnitId },
        UnitMoved m => new[] { m.UnitId },
        UnitsSwapped w => new[] { w.UnitA, w.UnitB },
        _ => Array.Empty<int>(),
    };

    private static int Damage(IEnumerable<BattleEvent> events, int unitId, Func<DamageDealt, int> pick) =>
        events.OfType<DamageDealt>().Where(d => d.TargetUnitId == unitId).Sum(pick);

    private static int Healed(IEnumerable<BattleEvent> events, int unitId) =>
        events.OfType<Healed>().Where(h => h.TargetUnitId == unitId).Sum(h => h.Amount);

    private static IReadOnlyList<TimelineShift> Shifts(BattleState before, BattleState after, int userId)
    {
        var old = before.Timeline.Upcoming().ToDictionary(e => e.Id);
        var shifts = new List<TimelineShift>();
        foreach (var e in after.Timeline.Upcoming())
            if (old.TryGetValue(e.Id, out var was) && was.Due != e.Due && !(e.Kind == TimelineKind.UnitTurn && e.OwnerUnitId == userId))
                shifts.Add(new TimelineShift(e.Id, e.OwnerUnitId, was.Due, e.Due));
        return shifts;
    }
}
