namespace Everkin.Core.Battle;

/// <summary>
/// Stealth (game_system.md section 10.1). A stealthed unit is ignored by the other side's targeting, range, nearest-target
/// and interception (the readers are in <see cref="Targeting"/> and <see cref="Interception"/>); area effects still hit it.
/// This class gains and loses it and reports the change. Stealth is binary and lasts until broken.
/// </summary>
public static class Stealth
{
    /// <summary>
    /// Whether a stealth skill may be used on this unit: it is alive and not already stealthed, and the side keeps at least
    /// one other visible unit, so a side never has zero visible units through its own stealth (section 10.1). Skills add
    /// their own conditions (Stalk needs the Hunter outside the Front row).
    /// </summary>
    public static bool CanGain(BattleState state, int unitId)
    {
        var unit = state.GetUnit(unitId);
        if (unit.IsKnockedOut || unit.IsStealthed || !state.Formation.Contains(unitId)) return false;
        var side = state.Formation.PositionOf(unitId).Side;
        return state.Formation.Units(side).Any(id =>
        {
            if (id == unitId) return false;
            var other = state.GetUnit(id);
            return !other.IsKnockedOut && !other.IsStealthed;
        });
    }

    /// <summary>Gives the unit stealth. Throws if <see cref="CanGain"/> is false.</summary>
    public static void Gain(BattleState state, int unitId)
    {
        if (!CanGain(state, unitId)) throw new InvalidOperationException($"Unit {unitId} cannot gain stealth.");
        state.GetUnit(unitId).IsStealthed = true;
        state.Emit(new StealthChanged(unitId, true, StealthCause.Gained));
    }

    /// <summary>
    /// Ends the unit's stealth, if it has any (reports it). Stealth ends when the unit attacks (<see cref="StealthCause.Attacked"/>)
    /// or is hit (<see cref="StealthCause.Hit"/>): area effects and hits reaching it without targeting it count, and a hit a
    /// shield fully absorbs still breaks it; a hit that is intercepted hits the interceptor instead, and Poison and Bleeding
    /// ticks are not hits. A reveal effect or a cleanse ends it too (<see cref="StealthCause.Revealed"/>). The caller decides
    /// which of these happened; <see cref="BattleState.DealDamage"/> does not touch stealth by itself.
    /// </summary>
    public static bool Break(BattleState state, int unitId, StealthCause cause)
    {
        var unit = state.GetUnit(unitId);
        if (!unit.IsStealthed) return false;
        unit.IsStealthed = false;
        state.Emit(new StealthChanged(unitId, false, cause));
        return true;
    }
}
