namespace Everkin.Core.Battle;

/// <summary>
/// Delayed and auto events that locked a target (game_system section 10.1, "Locked targets that become stealthed"): who is hit when the
/// event resolves. A `direct` effect hits a slot and so whoever stands there now (both units of a 50% overlap), stealthed or not, and
/// nobody if the slot is empty. A `projectile` passes a target that has become stealthed and hits the units behind it under the
/// behind rule of section 8.4 (100% overlap automatically, 50% overlap with a 50% roll); if nothing is hit the shot is lost. Row and area
/// skills hit a position and are not affected.
/// </summary>
public static class LockedTargets
{
    /// <summary>The units a delayed `direct` effect on this slot hits now.</summary>
    public static IReadOnlyList<int> Direct(BattleState state, Spot slot) =>
        state.Ground.UnitsCovering(slot).Where(id => !state.GetUnit(id).IsKnockedOut).ToArray();

    /// <summary>The units a queued `projectile` shot at this unit hits now.</summary>
    public static IReadOnlyList<int> Projectile(BattleState state, int lockedTargetId)
    {
        var target = state.GetUnit(lockedTargetId);
        if (!target.IsKnockedOut && !target.IsStealthed) return new[] { lockedTargetId };
        return AreaPatterns.HitBehind(state, AreaPatterns.UnitsBehind(state, lockedTargetId))
            .Where(id => !state.GetUnit(id).IsKnockedOut).ToArray();
    }
}

/// <summary>Prices (game_system section 2.2, production section 1.4).</summary>
public static class Prices
{
    /// <summary>A coin creature costs 50 + 25 × its Cost in coins.</summary>
    public static int CoinCreature(int cost) => 50 + 25 * cost;
}
