namespace Everkin.Core.Battle;

/// <summary>
/// Stealth (game_system.md section 10.1). A stealthed unit is ignored by the other side's targeting, range, nearest-target
/// and interception (the readers are in <see cref="Targeting"/> and <see cref="Interception"/>); area effects still hit it.
/// This class gains and loses it and reports the change. Stealth is binary and lasts until broken.
/// </summary>
public static class Stealth
{
    /// <summary>Stealth lasts 40 time units unless it is broken first or a skill states another length (section 10.1).</summary>
    public const int DefaultLength = 40;

    /// <summary>
    /// Whether the unit may gain stealth: it is alive, on the board, not already stealthed and not stopped by a passive (Scent). There is no restriction by
    /// row or by the other units of the side: a side's last visible unit can hide (section 10.1, 20.5).
    /// </summary>
    public static bool CanGain(BattleState state, int unitId)
    {
        var unit = state.GetUnit(unitId);
        if (unit.IsKnockedOut || unit.IsStealthed || !state.Formation.Contains(unitId)) return false;
        return !state.StealthBlockers.Any(b => b(state, unitId));
    }

    /// <summary>Gives the unit stealth for <paramref name="length"/> time units (40 by default). Throws if <see cref="CanGain"/> is false.</summary>
    public static void Gain(BattleState state, int unitId, int length = DefaultLength)
    {
        if (!CanGain(state, unitId)) throw new InvalidOperationException($"Unit {unitId} cannot gain stealth.");
        var unit = state.GetUnit(unitId);
        unit.IsStealthed = true;
        unit.StealthMarkerId = state.Timeline.Add(TimelineKind.EffectEnd, length, unitId, ExpiryTag).Id;
        state.Emit(new StealthChanged(unitId, true, StealthCause.Gained));
    }

    /// <summary>The tag of the timeline marker that ends a unit's stealth.</summary>
    public const string ExpiryTag = "stealth";

    /// <summary>The marker of a unit's stealth reached its time.</summary>
    public static void HandleExpiry(BattleState state, TimelineEntry entry)
    {
        if (entry.OwnerUnitId is not { } unitId) return;
        var unit = state.GetUnit(unitId);
        if (unit.StealthMarkerId == entry.Id) Break(state, unitId, StealthCause.Expired);
    }

    /// <summary>
    /// Ends the unit's stealth, if it has any (reports it). Stealth ends when the unit attacks (<see cref="StealthCause.Attacked"/>)
    /// or is hit (<see cref="StealthCause.Hit"/>): area effects and hits reaching it without targeting it count, and a hit a
    /// shield fully absorbs still breaks it; a hit that is intercepted hits the interceptor instead, and Poison and Bleeding
    /// ticks are not hits. A reveal effect or a cleanse ends it too (<see cref="StealthCause.Revealed"/>). The caller decides
    /// which of these happened; <see cref="BattleState.DealDamage"/> does not touch stealth by itself.
    /// </summary>
    public static bool Break(BattleState state, int unitId, StealthCause cause, bool area = false)
    {
        var unit = state.GetUnit(unitId);
        if (!unit.IsStealthed) return false;
        if (cause == StealthCause.Hit && state.StealthBreakVetoes.Any(v => v(state, unitId, cause, area))) return false;   // Freeze in Place
        unit.IsStealthed = false;
        if (unit.StealthMarkerId is { } marker) { state.Timeline.Remove(marker); unit.StealthMarkerId = null; }
        state.Emit(new StealthChanged(unitId, false, cause));
        return true;
    }
}
