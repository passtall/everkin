namespace Everkin.Core.Battle;

/// <summary>
/// KO and revival (game_system.md sections 10.2 to 10.4). 0 HP means KO. A KO unit stays in its row and keeps consuming
/// row capacity; it no longer intercepts, cannot be targeted except by resurrection, and cannot be moved. KO removes all
/// statuses and, by default, the unit's pending turn, auto events and delayed events.
/// </summary>
public static class Knockout
{
    /// <summary>
    /// Runs the KO rules for a unit that is at 0 HP and has not been processed yet: clears its statuses, removes its turn
    /// and, unless <paramref name="keepPendingEvents"/> (a skill stated that its events persist), its pending auto events
    /// and delayed events, and reports it. Returns false if the unit is not KO or was already processed. The body keeps its
    /// slot. Removing KO'd summons and traps (section 11.3) and the victory check are separate steps.
    /// </summary>
    public static bool Resolve(BattleState state, int unitId, bool keepPendingEvents = false)
    {
        var unit = state.GetUnit(unitId);
        if (!unit.IsKnockedOut || unit.KoProcessed) return false;
        unit.KoProcessed = true;
        state.EndSharedPool(unitId);   // the link ends when either unit is KO
        state.Ground.Accrue();

        if (unit.IsStealthed) Stealth.Break(state, unitId, StealthCause.KnockedOut);
        Statuses.ClearAll(state, unitId);
        unit.ClearStatuses();

        state.Timeline.RemoveOwnedBy(unitId, includeTurn: true, keepEvents: keepPendingEvents);

        state.Emit(new UnitKnockedOut(unitId));

        // A KO'd summon, construct or trap is removed at once and frees its slot; it cannot be revived (sections 10.3, 11.2).
        if (unit.IsSummoned) Summons.Remove(state, unitId, RemovalCause.Ko);

        // Summons persist when their summoner is KO'd, unless the skill says they disappear.
        foreach (var other in state.Units.ToArray())
            if (other.SummonerId == unitId && other.DisappearsWithSummoner && !other.IsRemoved)
                Summons.Remove(state, other.Id, RemovalCause.SummonerGone);
        return true;
    }

    /// <summary>
    /// The KO check after an event or a simultaneous block (section 6.5): every unit that is now at 0 HP and not yet
    /// processed is resolved. When several fall at once, the order is random, each order equally likely, from the battle's
    /// generator. Returns the units in the order they were resolved.
    /// </summary>
    public static IReadOnlyList<int> ResolveAll(BattleState state)
    {
        var fallen = state.Units.Where(u => u.IsKnockedOut && !u.KoProcessed).Select(u => u.Id).ToList();
        for (var i = fallen.Count - 1; i > 0; i--)
        {
            var j = state.Random.NextInt(i + 1);
            (fallen[i], fallen[j]) = (fallen[j], fallen[i]);
        }
        foreach (var id in fallen) Resolve(state, id);
        return fallen;
    }

    /// <summary>
    /// Revives a KO unit at its own slot with the HP the skill states (capped at its maximum, at least 1). There is no free
    /// turn: it is scheduled as though the revival had been its own action, so its next turn comes Wait(delay, Speed) time
    /// units from now. A skill may override this; pass its Delay. There is no limit on repeated revival; the skill's
    /// costs are the limit. Throws if the unit is not KO.
    /// </summary>
    public static void Revive(BattleState state, int unitId, int hp, int delay)
    {
        var unit = state.GetUnit(unitId);
        if (unit.IsRemoved) throw new InvalidOperationException($"Unit {unitId} was removed and cannot be revived.");
        if (!unit.IsKnockedOut) throw new InvalidOperationException($"Unit {unitId} is not KO.");
        if (!unit.KoProcessed) Resolve(state, unitId); // a body that was never processed is processed first
        state.Ground.Accrue();
        unit.Hp = Math.Clamp(hp, 1, unit.MaxHp);
        unit.KoProcessed = false;
        state.Timeline.ScheduleTurn(unitId, delay, unit.EffectiveSpeed);
        state.Emit(new UnitRevived(unitId, unit.Hp));
    }
}
