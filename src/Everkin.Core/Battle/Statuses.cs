namespace Everkin.Core.Battle;

/// <summary>Status IDs: the named statuses of game_system.md section 10.5 and the effects the rules treat as statuses.</summary>
public static class StatusIds
{
    public const string Blind = "blind";
    public const string Poison = "poison";
    public const string Bleeding = "bleeding";
    public const string Silenced = "silenced";
    public const string Chilled = "chilled";
    public const string Frozen = "frozen";
    public const string Stunned = "stunned";
    public const string Confused = "confused";
    public const string Immobilized = "immobilized";
    public const string Marked = "marked";
    public const string Ward = "ward";
    public const string Dampen = "dampen";
    public const string Airborne = "airborne";

    /// <summary>Haste: Speed raised by the status's magnitude in steps (section 6.1).</summary>
    public const string Haste = "haste";

    /// <summary>Slow: Speed lowered by the status's magnitude in steps (section 6.1).</summary>
    public const string Slow = "slow";
}

/// <summary>What happens when a status is applied to a unit that already has it (section 10.5).</summary>
public enum ReapplyRule
{
    /// <summary>The new duration replaces the old one, even if it is shorter. There is no stacking.</summary>
    Replace,

    /// <summary>Stunned: the longer remaining time stays.</summary>
    KeepLongerDuration,

    /// <summary>Ward: a unit has one Ward; a new one replaces it only if it absorbs at least as much as the old one has left.</summary>
    KeepLargerMagnitude,
}

/// <summary>Whether a status is harmful or helpful, for cleanse and dispel skills.</summary>
public enum Polarity
{
    Harmful,
    Helpful,

    /// <summary>The docs do not say yet (Airborne).</summary>
    Unclassified,
}

/// <summary>One status on one unit. A unit has at most one instance per status type.</summary>
public sealed class StatusInstance
{
    public StatusInstance(string id) => Id = id;

    public string Id { get; }

    /// <summary>Its size where it has one: Speed steps for Haste and Slow, the Ward amount when applied. 0 otherwise.</summary>
    public int Magnitude { get; internal set; }

    /// <summary>The unit that applied it. A status keeps going if its source is KO (section 10.5).</summary>
    public int? SourceUnitId { get; internal set; }

    /// <summary>The timeline marker for its end, when it runs on the battle clock.</summary>
    public int? MarkerEntryId { get; internal set; }

    /// <summary>The battle time at which it ends, when it runs on the battle clock.</summary>
    public int? EndsAt { get; internal set; }

    /// <summary>It ends when this unit's next turn arrives (Ward, Orders, Braced and others last until the caster's next turn).</summary>
    public int? EndsAtNextTurnOf { get; internal set; }

    /// <summary>Skills left for a status that counts them (Bleeding: the unit's next 3 melee or move skills).</summary>
    public int? UsesLeft { get; internal set; }

    /// <summary>Poison and similar damage over time: what has built up, in hundredths of HP, not yet landed (section 10.5).</summary>
    public int BuildupHundredths { get; internal set; }

    /// <summary>The battle time up to which the buildup has been counted.</summary>
    public int LastTickTime { get; internal set; }

    internal StatusInstance Clone() => (StatusInstance)MemberwiseClone();
}

/// <summary>Static facts about status types.</summary>
public static class StatusRules
{
    /// <summary>Poison: 1 damage per 10 time units, which is 10 hundredths of HP per time unit (section 10.5).</summary>
    public const int PoisonHundredthsPerTimeUnit = 10;

    /// <summary>Bleeding: 2 damage after each melee or move skill. A rule number: it does not scale with Power.</summary>
    public const int BleedingDamage = 2;

    /// <summary>Blind: all of the unit's attacks have a 50% chance to miss.</summary>
    public const int BlindMissChance = 50;

    /// <summary>Confused: each skill has a 50% chance to target a random unit instead.</summary>
    public const int ConfusedRetargetChance = 50;

    /// <summary>Chilled lowers Speed by 2.</summary>
    public const int ChilledSpeedLoss = 2;

    public static ReapplyRule Reapply(string statusId) => statusId switch
    {
        StatusIds.Stunned => ReapplyRule.KeepLongerDuration,
        StatusIds.Ward => ReapplyRule.KeepLargerMagnitude,
        _ => ReapplyRule.Replace,
    };

    public static Polarity PolarityOf(string statusId) => statusId switch
    {
        StatusIds.Blind or StatusIds.Poison or StatusIds.Bleeding or StatusIds.Silenced or StatusIds.Chilled
            or StatusIds.Frozen or StatusIds.Stunned or StatusIds.Confused or StatusIds.Immobilized
            or StatusIds.Marked or StatusIds.Slow => Polarity.Harmful,
        StatusIds.Ward or StatusIds.Dampen or StatusIds.Haste => Polarity.Helpful,
        _ => Polarity.Unclassified,
    };

    /// <summary>How much a status changes Speed, in steps.</summary>
    public static int SpeedModifier(StatusInstance status) => status.Id switch
    {
        StatusIds.Chilled => -ChilledSpeedLoss,
        StatusIds.Haste => status.Magnitude,
        StatusIds.Slow => -status.Magnitude,
        _ => 0,
    };
}

/// <summary>A request to put a status on a unit.</summary>
/// <param name="Duration">Time units it lasts on the battle clock, or null for a status without a clock.</param>
/// <param name="Magnitude">Ward amount or Speed steps.</param>
/// <param name="EndsAtNextTurnOf">The unit whose next turn ends it.</param>
/// <param name="Uses">Skills it counts (Bleeding).</param>
public readonly record struct StatusApplication(
    int UnitId, string Id, int? Duration = null, int Magnitude = 0, int? SourceUnitId = null,
    int? EndsAtNextTurnOf = null, int? Uses = null);

/// <summary>What the start of a unit's turn did to it.</summary>
/// <param name="FrozenTurnSkipped">Frozen was consumed: the turn is skipped, at the cost of a skipped turn.</param>
/// <param name="Stunned">The unit is Stunned: it takes no actions (see the open question on what such a turn costs).</param>
public readonly record struct TurnStartResult(bool FrozenTurnSkipped, bool Stunned);

/// <summary>
/// The status framework (game_system.md section 10.5, 10.6): one instance per status type, durations in time units
/// kept by an end marker on the timeline, reapplication rules, expiry, and the hooks of the named statuses for the battle
/// driver to call. KO clears everything. Ward, Dampen, Immobilized, the interception block and Airborne also live as
/// flags on the unit, which this class keeps in step.
/// </summary>
public static class Statuses
{
    // ---------------------------------------------------------------- reading

    public static bool Has(Unit unit, string statusId) =>
        statusId == StatusIds.Ward ? unit.Ward > 0 : unit.StatusInstances.ContainsKey(statusId);

    public static StatusInstance? Get(Unit unit, string statusId) =>
        unit.StatusInstances.TryGetValue(statusId, out var s) ? s : null;

    /// <summary>The chance, in percent, that an attack by this unit misses (Blind).</summary>
    public static int AttackMissChance(Unit unit) => Has(unit, StatusIds.Blind) ? StatusRules.BlindMissChance : 0;

    /// <summary>Silenced units cannot use `magical` skills; physical skills, including a physical Attack Type attack, still work.</summary>
    public static bool CanUseMagical(Unit unit) => !Has(unit, StatusIds.Silenced);

    /// <summary>
    /// A Confused unit's skill has a 50% chance to target a random unit instead (any unit the skill could legally reach,
    /// on either side; helpful skills included). Draws from the battle generator only when the unit is Confused.
    /// </summary>
    public static bool RollConfusion(BattleState state, int unitId) =>
        Has(state.GetUnit(unitId), StatusIds.Confused) && state.Random.Chance(StatusRules.ConfusedRetargetChance);

    // ---------------------------------------------------------------- applying and removing

    /// <summary>
    /// Puts a status on a unit and reports it. A unit has one instance per type: reapplying refreshes the duration, and
    /// the new duration always replaces the old one even if shorter, except Stunned (the longer time stays) and Ward (the
    /// larger amount stays). Returns false when the old status was kept. The end of a clock status is a marker on the
    /// timeline; Speed statuses adjust the unit's remaining Wait as Haste and Slow do (section 6.1).
    /// </summary>
    public static bool Apply(BattleState state, StatusApplication a)
    {
        var unit = state.GetUnit(a.UnitId);
        if (unit.IsKnockedOut) throw new InvalidOperationException($"Unit {a.UnitId} is KO and cannot gain statuses.");
        var existing = Get(unit, a.Id);
        var timeline = state.Timeline;

        switch (StatusRules.Reapply(a.Id))
        {
            case ReapplyRule.KeepLargerMagnitude:
                if (unit.Ward > 0 && a.Magnitude < unit.Ward) return false;
                break;
            case ReapplyRule.KeepLongerDuration:
                if (existing?.EndsAt is { } ends && a.Duration is { } d && ends - timeline.Now >= d) return false;
                break;
        }

        if (a.Id == StatusIds.Airborne) state.Ground.Accrue();
        var oldSpeed = unit.EffectiveSpeed;
        var refreshed = existing is not null || (a.Id == StatusIds.Ward && unit.Ward > 0);
        var buildup = existing?.BuildupHundredths ?? 0;
        var lastTick = existing?.LastTickTime ?? timeline.Now;
        if (existing?.MarkerEntryId is { } oldMarker) timeline.Remove(oldMarker);

        var instance = new StatusInstance(a.Id)
        {
            Magnitude = a.Magnitude,
            SourceUnitId = a.SourceUnitId,
            EndsAtNextTurnOf = a.EndsAtNextTurnOf,
            UsesLeft = a.Uses,
            BuildupHundredths = buildup,
            LastTickTime = lastTick,
        };
        if (a.Duration is { } duration)
        {
            instance.MarkerEntryId = timeline.Add(TimelineKind.EffectEnd, duration, a.UnitId, a.Id).Id;
            instance.EndsAt = timeline.Now + duration;
        }
        unit.StatusStore[a.Id] = instance;
        if (a.Id == StatusIds.Ward) unit.Ward = a.Magnitude;
        Sync(unit, a.Id);
        AdjustTimelineForSpeed(state, unit, oldSpeed);

        state.Emit(new StatusApplied(a.UnitId, a.Id, a.Magnitude, a.Duration, refreshed));
        return true;
    }

    /// <summary>Removes a status and its end marker and reports it. Returns false if the unit does not have it.</summary>
    public static bool Remove(BattleState state, int unitId, string statusId, StatusEndCause cause)
    {
        var unit = state.GetUnit(unitId);
        if (!unit.StatusStore.TryGetValue(statusId, out var instance)) return false;
        if (statusId == StatusIds.Airborne) state.Ground.Accrue();
        var oldSpeed = unit.EffectiveSpeed;
        if (instance.MarkerEntryId is { } marker) state.Timeline.Remove(marker);
        unit.StatusStore.Remove(statusId);
        if (statusId == StatusIds.Ward) unit.Ward = 0;
        Sync(unit, statusId);
        AdjustTimelineForSpeed(state, unit, oldSpeed);
        state.Emit(new StatusEnded(unitId, statusId, cause));
        return true;
    }

    /// <summary>Removes every status of the given polarity (Absolve removes the harmful ones, Dispel the helpful ones). Returns what ended.</summary>
    public static IReadOnlyList<string> RemoveAll(BattleState state, int unitId, Polarity polarity, StatusEndCause cause = StatusEndCause.Cleansed)
    {
        var unit = state.GetUnit(unitId);
        var ids = unit.StatusInstances.Keys.Where(id => StatusRules.PolarityOf(id) == polarity).ToList();
        if (polarity == Polarity.Helpful && unit.Ward > 0 && !ids.Contains(StatusIds.Ward)) ids.Add(StatusIds.Ward);
        var removed = new List<string>();
        foreach (var id in ids)
            if (Remove(state, unitId, id, cause)) removed.Add(id);
        return removed;
    }

    /// <summary>
    /// KO removes every status (section 10.5): the instances and their markers on the timeline. Quiet: the KO event covers it.
    /// </summary>
    public static void ClearAll(BattleState state, int unitId)
    {
        var unit = state.GetUnit(unitId);
        foreach (var instance in unit.StatusInstances.Values)
            if (instance.MarkerEntryId is { } marker) state.Timeline.Remove(marker);
        // Unit.ClearStatuses empties the instances and the flags.
    }

    private static void Sync(Unit unit, string statusId)
    {
        switch (statusId)
        {
            case StatusIds.Immobilized:
                unit.IsImmobilized = unit.StatusInstances.ContainsKey(StatusIds.Immobilized);
                goto case StatusIds.Stunned;
            case StatusIds.Stunned:
            case StatusIds.Frozen:
                unit.InterceptionBlocked = unit.StatusInstances.ContainsKey(StatusIds.Stunned)
                    || unit.StatusInstances.ContainsKey(StatusIds.Frozen)
                    || unit.StatusInstances.ContainsKey(StatusIds.Immobilized);
                break;
            case StatusIds.Dampen:
                unit.Dampen = unit.StatusInstances.ContainsKey(StatusIds.Dampen);
                break;
            case StatusIds.Airborne:
                unit.IsAirborne = unit.StatusInstances.ContainsKey(StatusIds.Airborne);
                break;
        }
    }

    private static void AdjustTimelineForSpeed(BattleState state, Unit unit, int oldSpeed)
    {
        var now = unit.EffectiveSpeed;
        if (now != oldSpeed) state.Timeline.ChangeSpeed(unit.Id, oldSpeed, now);
    }

    // ---------------------------------------------------------------- the clock

    /// <summary>
    /// A status end marker reached its time on the timeline: the status ends (Poison first lands what it has built up to
    /// that moment). A marker that no longer belongs to a status (it was replaced) does nothing.
    /// </summary>
    public static void HandleExpiry(BattleState state, TimelineEntry entry)
    {
        if (entry.Kind == TimelineKind.EffectEnd && entry.Tag is { } groundTag && groundTag.StartsWith(GroundField.TagPrefix, StringComparison.Ordinal))
        {
            state.Ground.HandleExpiry(entry);
            return;
        }
        if (entry.Kind != TimelineKind.EffectEnd || entry.OwnerUnitId is not { } unitId || entry.Tag is not { } statusId) return;
        if (statusId == Summons.ExpiryTag) { Summons.HandleExpiry(state, entry); return; }
        var unit = state.GetUnit(unitId);
        var instance = Get(unit, statusId);
        if (instance is null || instance.MarkerEntryId != entry.Id) return;
        if (statusId == StatusIds.Poison) LandPoison(state, unit, instance);
        Remove(state, unitId, statusId, StatusEndCause.Expired);
    }

    /// <summary>
    /// Poison builds up per time unit and lands as whole points whenever any unit gets a turn; the fraction carries over
    /// (section 10.5). Call it when a turn begins. Returns the units that took damage, for the KO check (a tick that KOs
    /// the unit ends its turn). Poison is not a hit: shields do not absorb it, stealth is not affected, Momentum does not move.
    /// </summary>
    public static IReadOnlyList<int> TickPoison(BattleState state)
    {
        var damaged = new List<int>();
        foreach (var unit in state.Units)
        {
            if (unit.IsKnockedOut || !unit.StatusStore.TryGetValue(StatusIds.Poison, out var poison)) continue;
            if (LandPoison(state, unit, poison)) damaged.Add(unit.Id);
        }
        return damaged;
    }

    private static bool LandPoison(BattleState state, Unit unit, StatusInstance poison)
    {
        var until = Math.Min(state.Timeline.Now, poison.EndsAt ?? state.Timeline.Now);
        poison.BuildupHundredths += Math.Max(0, until - poison.LastTickTime) * StatusRules.PoisonHundredthsPerTimeUnit;
        poison.LastTickTime = Math.Max(poison.LastTickTime, until);
        var whole = poison.BuildupHundredths / 100;
        poison.BuildupHundredths %= 100;
        if (whole <= 0) return false;
        state.DealDamage(poison.SourceUnitId, unit.Id, whole, DamageSource.Periodic);
        return true;
    }

    // ---------------------------------------------------------------- hooks for the battle driver

    /// <summary>
    /// A unit's turn arrives. Statuses that last until its next turn end (Ward, Orders, Braced); a Frozen unit's turn is
    /// skipped and Frozen ends (section 10.5); a Stunned unit takes no actions.
    /// </summary>
    public static TurnStartResult OnTurnStart(BattleState state, int unitId)
    {
        foreach (var other in state.Units.ToArray())
            foreach (var instance in other.StatusInstances.Values.Where(s => s.EndsAtNextTurnOf == unitId).ToArray())
                Remove(state, other.Id, instance.Id, StatusEndCause.Expired);

        var unit = state.GetUnit(unitId);
        var frozen = Remove(state, unitId, StatusIds.Frozen, StatusEndCause.Consumed);
        return new TurnStartResult(frozen, Has(unit, StatusIds.Stunned));
    }

    /// <summary>
    /// A unit used a skill. A Bleeding unit takes 2 damage after each `melee` or `move` skill it actually uses, and the
    /// count of skills left goes down; other skills (a heal, a Bolt) cost nothing. Bleeding is not a hit: shields do not
    /// absorb it, Dampen applies, stealth stays. Returns true if damage was dealt (for the KO check).
    /// </summary>
    public static bool OnSkillUsed(BattleState state, int unitId, bool melee, bool move)
    {
        var unit = state.GetUnit(unitId);
        if (!(melee || move) || !unit.StatusStore.TryGetValue(StatusIds.Bleeding, out var bleeding)) return false;
        state.DealDamage(bleeding.SourceUnitId, unitId, StatusRules.BleedingDamage, DamageSource.Periodic);
        bleeding.UsesLeft = (bleeding.UsesLeft ?? 1) - 1;
        if (bleeding.UsesLeft <= 0) Remove(state, unitId, StatusIds.Bleeding, StatusEndCause.Consumed);
        return true;
    }

    /// <summary>A unit was hit: any hit ends Stunned early (section 10.5).</summary>
    public static void OnHit(BattleState state, int unitId) =>
        Remove(state, unitId, StatusIds.Stunned, StatusEndCause.Hit);
}
