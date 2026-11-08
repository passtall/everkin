namespace Everkin.Core.Battle;

/// <summary>How a skill reaches its target (skills.md Delivery column, game_system.md section 4.7).</summary>
public enum Delivery
{
    Melee,
    Projectile,
    Direct,
}

/// <summary>One interceptor's part of an incoming effect.</summary>
/// <param name="InterceptorId">The unit that stepped in.</param>
/// <param name="Amount">The damage or healing it receives; for an effect that cannot be split, the effect's amount.</param>
/// <param name="Full">True when it took the whole remaining effect.</param>
public readonly record struct InterceptedShare(int InterceptorId, int Amount, bool Full);

/// <summary>What interception did to an incoming effect.</summary>
/// <param name="Shares">The interceptors, in the order they stepped in. The caller applies each share to its unit.</param>
/// <param name="Remaining">What still reaches the target. Zero when the effect was fully intercepted.</param>
public sealed record InterceptionResult(IReadOnlyList<InterceptedShare> Shares, int Remaining)
{
    public bool Intercepted => Shares.Count > 0;

    /// <summary>Nothing reaches the target. Momentum does not change for an intercepted hit (section 6.7).</summary>
    public bool FullyIntercepted => Shares.Count > 0 && Remaining == 0;
}

/// <summary>A unit stepped in for the target and took the whole effect or part of it.</summary>
public sealed record Intercepted(int TargetUnitId, int InterceptorUnitId, int Amount, bool Full) : BattleEvent;

/// <summary>
/// Interception (game_system.md section 8.2). Defenders stand in rows in front of the target and overlap it horizontally:
/// 100% overlap gives a 50% chance to intercept, 50% overlap a 25% chance. An interceptor takes the full effect (50%)
/// or half of it, rounded down, with the rest continuing to the next defender and finally the target (50%).
/// </summary>
public static class Interception
{
    public const int ChanceAtFullOverlap = 50;
    public const int ChanceAtHalfOverlap = 25;
    public const int FullHitChance = 50;

    /// <summary>
    /// Only `melee` and `projectile` skills can be intercepted, whichever side uses them and whatever they do; a `direct`
    /// skill cannot. A skill may say it ignores interception (Phasing Strike).
    /// </summary>
    public static bool CanBeIntercepted(Delivery delivery, bool ignoresInterception = false) =>
        !ignoresInterception && delivery != Delivery.Direct;

    /// <summary>
    /// Whether a unit can intercept at all: not KO and not stealthed (section 10.1), and not blocked by a status that
    /// forbids it (Stunned, Frozen, Immobilized: <see cref="Unit.InterceptionBlocked"/>, kept by the status rules).
    /// </summary>
    public static bool IsEligible(Unit unit) => !unit.IsKnockedOut && !unit.IsStealthed && !unit.InterceptionBlocked;

    /// <summary>As the plain check, and not stopped by a passive that forbids intercepting (Keeper of the Gate).</summary>
    public static bool IsEligible(BattleState state, Unit unit) =>
        IsEligible(unit) && !state.InterceptionBlockers.Any(b => b(state, unit.Id));

    /// <summary>The chance to intercept for a given horizontal overlap: 50% at 100%, 25% at 50%, none otherwise.</summary>
    public static int ChanceFor(int overlapPercent) => overlapPercent >= 100 ? ChanceAtFullOverlap
        : overlapPercent >= 50 ? ChanceAtHalfOverlap
        : 0;

    /// <summary>
    /// The eligible defenders of a target in the order they are tried: rows nearest the battle line first, up to the row in
    /// front of the target's own; only units that overlap the target at least 50%. The order inside a row is the board
    /// order here; <see cref="Resolve"/> shuffles each row with the battle's random generator.
    /// </summary>
    public static IReadOnlyList<int> Defenders(BattleState state, int targetId)
    {
        var position = state.Formation.PositionOf(targetId);
        var result = new List<int>();
        for (var r = 0; r < (int)position.Row; r++)
            foreach (var id in RowDefenders(state, targetId, position.Side, (Row)r))
                result.Add(id);
        return result;
    }

    private static List<int> RowDefenders(BattleState state, int targetId, Side side, Row row)
    {
        var list = new List<int>();
        foreach (var id in state.Formation.Units(side, row))
            if (IsEligible(state, state.GetUnit(id)) && ChanceFor(state.Formation.UnitOverlapPercent(id, targetId)) > 0)
                list.Add(id);
        return list;
    }

    /// <summary>
    /// Runs the defenders against an incoming effect of <paramref name="amount"/> (damage or healing) aimed at the target.
    /// Each defender, nearest row first and in random order inside a row, tries to intercept with its chance; if it does,
    /// it takes the whole remainder (50%) or half of it rounded down, the rest continuing (50%). A remainder of 1 cannot
    /// be split, and an effect that is not <paramref name="splittable"/> (a status, a push) goes entirely to the first
    /// interceptor. The caller applies each share and then whatever <see cref="InterceptionResult.Remaining"/> reaches
    /// the target, through its shields and reductions as usual. Every interception is reported.
    /// </summary>
    public static InterceptionResult Resolve(BattleState state, int targetId, int amount, bool splittable = true)
    {
        var position = state.Formation.PositionOf(targetId);
        var shares = new List<InterceptedShare>();
        var remaining = amount;
        var used = new HashSet<int>();

        // A unit that always intercepts (Shield Wall, Living Wall) goes first, with a 100% chance; the hit is full or partial as usual.
        foreach (var hook in state.GuaranteedInterceptors)
        {
            if (remaining <= 0) break;
            if (hook(state, targetId) is not { } guard || guard == targetId || used.Contains(guard)) continue;
            if (!IsEligible(state, state.GetUnit(guard))) continue;
            used.Add(guard);
            remaining = TakeShare(state, targetId, guard, remaining, splittable, shares);
        }

        for (var r = 0; r < (int)position.Row && remaining > 0; r++)
        {
            var row = RowDefenders(state, targetId, position.Side, (Row)r);
            Shuffle(row, state.Random);
            foreach (var id in row)
            {
                if (used.Contains(id)) continue;
                var chance = ChanceFor(state.Formation.UnitOverlapPercent(id, targetId));
                if (!state.Random.Chance(chance)) continue;

                remaining = TakeShare(state, targetId, id, remaining, splittable, shares);
                if (remaining <= 0) break;
            }
        }
        return new InterceptionResult(shares, Math.Max(0, remaining));
    }

    /// <summary>
    /// One interceptor steps in: it takes the whole remainder or, if it can be split, half of it (50% each, unless a passive such as
    /// Full Cover says it always takes the whole hit). Returns what is left.
    /// </summary>
    private static int TakeShare(BattleState state, int targetId, int interceptorId, int remaining, bool splittable, List<InterceptedShare> shares)
    {
        var canSplit = splittable && remaining >= 2;
        var whole = state.FullHitRules.Any(r => r(state, interceptorId));
        var full = !canSplit || whole || state.Random.Chance(FullHitChance);
        var share = full ? remaining : remaining / 2;
        shares.Add(new InterceptedShare(interceptorId, share, full));
        state.Emit(new Intercepted(targetId, interceptorId, share, full));
        return remaining - share;
    }

    /// <summary>Random order with every order equally likely (section 6.5), from the battle's generator.</summary>
    private static void Shuffle(List<int> list, BattleRandom random)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = random.NextInt(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
