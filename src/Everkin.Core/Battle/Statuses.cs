namespace Everkin.Core.Battle;

/// <summary>Status IDs: the named statuses of game_system.md section 10.5 and the effects the rules treat as statuses.</summary>
public static class StatusIds
{
    public const string Blind = "blind";
    public const string Poison = "poisoned";
    public const string Bleeding = "bleeding";
    public const string Silenced = "silenced";
    public const string Frozen = "frozen";
    public const string Stunned = "stunned";
    public const string Confused = "confused";
    public const string Immobilized = "immobilized";
    public const string Marked = "marked";
    public const string Ward = "ward";
    public const string Dampen = "dampen";
    public const string Airborne = "airborne";

    /// <summary>Lie in Wait: the next skill the unit uses is lost and it is Stunned (skills.md Hunter).</summary>
    public const string LieInWait = "lie_in_wait";

    /// <summary>Life Link: healing on this unit also heals the linked unit for half. The magnitude is the linked unit's ID.</summary>
    public const string LifeLink = "life_link";

    /// <summary>The Guardian's Charge: the ally it protects (Take a Charge). The status's source is the Guardian.</summary>
    public const string Charge = "charge";

    /// <summary>Leap to Defend: the Guardian takes the next attack aimed at this unit. The source is the Guardian.</summary>
    public const string LeapCover = "leap_cover";

    /// <summary>Distract: the unit's next attack must target the Guardian if it can. The source is the Guardian.</summary>
    public const string Distracted = "distracted";

    /// <summary>Pledge: area effects that would hit the Guardian's Charge hit the Guardian instead.</summary>
    public const string Pledge = "pledge";

    /// <summary>Shield Wall: the Guardian's row intercepts every melee or projectile attack aimed at units behind it.</summary>
    public const string ShieldWall = "shield_wall";

    /// <summary>Its next skill has more Delay by the status's magnitude (Net Trap: +3).</summary>
    public const string NextSkillDelay = "next_skill_delay";



    /// <summary>Phase Weapon: the unit's next attack has unlimited range (magnitude 1 with Phasing Strike: it also cannot be intercepted). The source is the Sorcerer.</summary>
    public const string Phased = "phased";

    /// <summary>Piercing Enchant: the unit's next attack also hits the unit behind its target.</summary>
    public const string Piercing = "piercing";

    /// <summary>Echo Enchant: the unit's next skill happens a second time, free.</summary>
    public const string Echo = "echo";

    /// <summary>Glass Body: the next hit on the unit deals double damage.</summary>
    public const string GlassBody = "glass_body";

    /// <summary>Transmute: the unit's Power and Speed swap values.</summary>
    public const string Transmuted = "transmuted";

    /// <summary>Stone Shell: the unit cannot act and cannot be damaged. A status, so it can be cleansed.</summary>
    public const string StoneShell = "stone_shell";

    /// <summary>Curse of Weight: the unit cannot Move or become airborne.</summary>
    public const string Weighted = "weighted";

    /// <summary>Curse of Stagnation: the unit's timed statuses do not run down.</summary>
    public const string Stagnant = "stagnant";

    /// <summary>Curse of Betrayal: healing the unit would receive goes to the Sorcerer's side instead.</summary>
    public const string Betrayed = "betrayed";

    /// <summary>Sidestep: the next attack on the unit misses.</summary>
    public const string Sidestep = "sidestep";

    /// <summary>Single Combat: the unit and its duel partner (the magnitude is the partner's ID) can be targeted only by each other.</summary>
    public const string Duel = "duel";

    /// <summary>Linked Fates / Shared Fate: when this unit loses HP, the unit whose ID is the magnitude loses half as much.</summary>
    public const string FateLink = "fate_link";

    /// <summary>Press the Advantage: the unit's next skill costs no Momentum.</summary>
    public const string FreeMomentum = "free_momentum";

    /// <summary>Song of Haste: Speed raised by the magnitude while a Leader's song reaches the unit. Kept by the aura, not applied by a skill.</summary>
    public const string SongHaste = "song_haste";

    /// <summary>War Drums (a song): cannot be Stunned or Frozen. Helpful.</summary>
    public const string WarDrums = "war_drums";

    /// <summary>Mirror Form: the unit has another unit's Power and Speed (Magnitude = Power x 100 + Speed). Neither helpful nor harmful.</summary>
    public const string Traded = "traded";

    /// <summary>Withering Curse: damage over time, Magnitude HP per 10 time units. A curse.</summary>
    public const string Withering = "withering";

    /// <summary>Ashwalker: +1 Speed while standing on burning ground (kept by an aura sync, no clock).</summary>
    public const string AshSpeed = "ash_speed";

    /// <summary>Regrowth (Mender keystone): regains HP over time. Magnitude is the HP per 10 time units. Helpful.</summary>
    public const string Regrowth = "regrowth";

    /// <summary>The Sorcerer's curses (Lingering Malice and Witch's Patience look at these).</summary>
    public static bool IsCurse(string statusId) => statusId is Weighted or Withering or Betrayed;

    /// <summary>The Sorcerer's enchantments (Enchanted Soul looks at these).</summary>
    public static bool IsEnchantment(string statusId) => statusId is Phased or Piercing or Echo;
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

    /// <summary>Curse of Stagnation holds this timed status: its end marker is off the timeline and the time left is kept in <see cref="HeldTimeLeft"/>.</summary>
    public bool Held { get; internal set; }

    /// <summary>The time units the status still has to run once it is released (only while <see cref="Held"/>).</summary>
    public int HeldTimeLeft { get; internal set; }

    /// <summary>Time units left on the battle clock at the given time, or null for a status without a clock.</summary>
    public int? TimeLeft(int now) => Held ? HeldTimeLeft : EndsAt is { } end ? Math.Max(0, end - now) : null;

    /// <summary>How fast its clock runs down: 1 normally, 2 next to a Cleric with Purifying Aura (a harmful status on an ally). See <see cref="Statuses.SetRate"/>.</summary>
    public int AuraRate { get; internal set; } = 1;

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

    public static ReapplyRule Reapply(string statusId) => statusId switch
    {
        StatusIds.Stunned => ReapplyRule.KeepLongerDuration,
        StatusIds.Ward => ReapplyRule.KeepLargerMagnitude,
        _ => ReapplyRule.Replace,
    };

    public static Polarity PolarityOf(string statusId) => statusId switch
    {
        StatusIds.Blind or StatusIds.Poison or StatusIds.Bleeding or StatusIds.Silenced
            or StatusIds.Frozen or StatusIds.Stunned or StatusIds.Confused or StatusIds.Immobilized
            or StatusIds.Marked or StatusIds.LieInWait or StatusIds.NextSkillDelay or StatusIds.Distracted
            or StatusIds.GlassBody or StatusIds.StoneShell or StatusIds.Weighted or StatusIds.Stagnant or StatusIds.Betrayed or StatusIds.Withering => Polarity.Harmful,
        StatusIds.Ward or StatusIds.Dampen or StatusIds.SongHaste or StatusIds.WarDrums or StatusIds.Regrowth or StatusIds.Charge or StatusIds.Pledge
            or StatusIds.ShieldWall or StatusIds.LifeLink or StatusIds.Phased or StatusIds.Piercing or StatusIds.Echo or StatusIds.Sidestep or StatusIds.FreeMomentum => Polarity.Helpful,
        _ => Polarity.Unclassified,
    };

    /// <summary>How much a status changes Speed, in steps.</summary>
    public static int SpeedModifier(StatusInstance status) => status.Id switch
    {
        StatusIds.SongHaste => status.Magnitude,
        StatusIds.AshSpeed => status.Magnitude,
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
/// <param name="Stunned">The unit is Stunned: it takes no actions, and its turn is skipped at the cost of a skipped turn (section 10.5).</param>
public readonly record struct TurnStartResult(bool FrozenTurnSkipped, bool Stunned)
{
    /// <summary>The turn does not happen: the driver schedules the next one with <see cref="Timeline.SkippedTurnWait"/>.</summary>
    public bool TurnSkipped => FrozenTurnSkipped || Stunned;
}

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
        foreach (var modifier in state.StatusApplicationModifiers) a = modifier(state, a);
        var existing = Get(unit, a.Id);
        var timeline = state.Timeline;
        if (state.StatusImmunities.Any(h => h(state, a.UnitId, a.Id))) return false;   // Unstoppable
        if (a.Id == StatusIds.Airborne && Has(unit, StatusIds.Weighted)) return false;   // Curse of Weight: cannot become airborne

        switch (StatusRules.Reapply(a.Id))
        {
            case ReapplyRule.KeepLargerMagnitude:
                if (unit.Ward > 0 && a.Magnitude < unit.Ward) return false;
                break;
            case ReapplyRule.KeepLongerDuration:
                if (existing?.TimeLeft(timeline.Now) is { } left && a.Duration is { } d && left >= d) return false;
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
        // Curse of Stagnation: a timed status applied under the curse is held at once; the curse itself holds the ones already there.
        if (a.Id != StatusIds.Stagnant && Has(unit, StatusIds.Stagnant)) Hold(state, unit, instance);
        if (a.Id == StatusIds.Stagnant)
            foreach (var other in unit.StatusInstances.Values.Where(o => o.Id != StatusIds.Stagnant).ToArray()) Hold(state, unit, other);
        AdjustTimelineForSpeed(state, unit, oldSpeed);

        state.Emit(new StatusApplied(a.UnitId, a.Id, a.Magnitude, a.Duration, refreshed));
        if (a.Id == StatusIds.Stunned && !_spreadingStun && a.Duration is { } stunLength) SpreadStunOverWater(state, a.UnitId, stunLength, a.SourceUnitId);
        return true;
    }

    [ThreadStatic] private static bool _spreadingStun;

    /// <summary>Water: any Stun on a unit standing on water also Stuns the units on the water spots touching it, for half the time (rounded down; game_system.md section 12.2).</summary>
    private static void SpreadStunOverWater(BattleState state, int unitId, int length, int? source)
    {
        if (length / 2 < 1 || !state.Formation.Contains(unitId)) return;
        var ground = state.Ground;
        var spot = ground.SpotOf(unitId);
        if (!ground.Has(spot, GroundKind.Water) || !ground.Affected(spot).Contains(unitId)) return;
        _spreadingStun = true;
        try
        {
            foreach (var touching in ground.WaterSpotsTouching(spot))
                foreach (var id in ground.Affected(touching))
                    if (id != unitId) Apply(state, new StatusApplication(id, StatusIds.Stunned, length / 2, SourceUnitId: source));
        }
        finally { _spreadingStun = false; }
    }

    /// <summary>Removes a status and its end marker and reports it. Returns false if the unit does not have it.</summary>
    public static bool Remove(BattleState state, int unitId, string statusId, StatusEndCause cause, int? byUnitId = null)
    {
        var unit = state.GetUnit(unitId);
        if (!unit.StatusStore.TryGetValue(statusId, out var instance)) return false;
        if (cause == StatusEndCause.Cleansed && state.StatusRemovalVetoes.Any(v => v(state, unitId, instance, byUnitId))) return false;   // Relentless
        if (statusId == StatusIds.Airborne) state.Ground.Accrue();
        var oldSpeed = unit.EffectiveSpeed;
        if (instance.MarkerEntryId is { } marker) state.Timeline.Remove(marker);
        unit.StatusStore.Remove(statusId);
        if (statusId == StatusIds.Ward) unit.Ward = 0;
        Sync(unit, statusId);
        AdjustTimelineForSpeed(state, unit, oldSpeed);
        state.Emit(new StatusEnded(unitId, statusId, cause));
        if (statusId == StatusIds.Stagnant)
            foreach (var held in unit.StatusInstances.Values.Where(o => o.Held).ToArray()) Release(state, unit, held);
        if (cause == StatusEndCause.Cleansed && byUnitId is { } by)
            foreach (var hook in state.StatusRemovedHooks.ToArray()) hook(state, unitId, instance, by);
        return true;
    }

    /// <summary>Curse of Stagnation: stops a timed status running down. Its end marker leaves the timeline and the time left is kept.</summary>
    private static void Hold(BattleState state, Unit unit, StatusInstance instance)
    {
        if (instance.Held || instance.EndsAt is not { } end) return;
        if (instance.Id == StatusIds.Poison) LandPoison(state, unit, instance);   // what has built up lands before it stops
        instance.HeldTimeLeft = Math.Max(0, end - state.Timeline.Now);
        if (instance.MarkerEntryId is { } marker) state.Timeline.Remove(marker);
        instance.MarkerEntryId = null;
        instance.EndsAt = null;
        instance.Held = true;
    }

    /// <summary>The curse ended: a held status runs again for the time it had left.</summary>
    private static void Release(BattleState state, Unit unit, StatusInstance instance)
    {
        instance.Held = false;
        var left = Math.Max(1, instance.HeldTimeLeft);
        instance.MarkerEntryId = state.Timeline.Add(TimelineKind.EffectEnd, left, unit.Id, instance.Id).Id;
        instance.EndsAt = state.Timeline.Now + left;
        instance.LastTickTime = state.Timeline.Now;
    }

    /// <summary>Removes every status of the given polarity (Absolve removes the harmful ones, Dispel the helpful ones). Returns what ended.</summary>
    public static IReadOnlyList<string> RemoveAll(BattleState state, int unitId, Polarity polarity, StatusEndCause cause = StatusEndCause.Cleansed, int? byUnitId = null)
    {
        var unit = state.GetUnit(unitId);
        var ids = unit.StatusInstances.Keys.Where(id => StatusRules.PolarityOf(id) == polarity).ToList();
        if (polarity == Polarity.Helpful && unit.Ward > 0 && !ids.Contains(StatusIds.Ward)) ids.Add(StatusIds.Ward);
        var removed = new List<string>();
        foreach (var id in ids)
            if (Remove(state, unitId, id, cause, byUnitId)) removed.Add(id);
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
            case StatusIds.StoneShell:
                unit.InterceptionBlocked = unit.StatusInstances.ContainsKey(StatusIds.Stunned)
                    || unit.StatusInstances.ContainsKey(StatusIds.StoneShell)
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
        if (statusId == Stealth.ExpiryTag) { Stealth.HandleExpiry(state, entry); return; }
        var unit = state.GetUnit(unitId);
        var instance = Get(unit, statusId);
        if (instance is null || instance.MarkerEntryId != entry.Id) return;
        if (statusId == StatusIds.Poison) LandPoison(state, unit, instance);
        if (statusId == StatusIds.Regrowth) LandRegrowth(state, unit, instance);
        if (statusId == StatusIds.Withering) LandWithering(state, unit, instance);
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
            if (unit.IsKnockedOut) continue;
            if (unit.StatusStore.TryGetValue(StatusIds.Poison, out var poison) && LandPoison(state, unit, poison)) damaged.Add(unit.Id);
            if (!unit.IsKnockedOut && unit.StatusStore.TryGetValue(StatusIds.Withering, out var withering) && LandWithering(state, unit, withering)) damaged.Add(unit.Id);
            if (!unit.IsKnockedOut && unit.StatusStore.TryGetValue(StatusIds.Regrowth, out var regrowth) && LandRegrowth(state, unit, regrowth)) damaged.Add(unit.Id);   // an undead is hurt by healing
        }
        return damaged;
    }

    /// <summary>Withering Curse deals Magnitude damage per 10 time units, built up per time unit and landing whenever any unit gets a turn; reductions apply, Wards do not absorb it (section 10.5).</summary>
    private static bool LandWithering(BattleState state, Unit unit, StatusInstance curse)
    {
        if (curse.Held) return false;
        var until = Math.Min(state.Timeline.Now, curse.EndsAt ?? state.Timeline.Now);
        curse.BuildupHundredths += Math.Max(0, until - curse.LastTickTime) * curse.Magnitude * 10;
        curse.LastTickTime = Math.Max(curse.LastTickTime, until);
        var whole = curse.BuildupHundredths / 100;
        curse.BuildupHundredths %= 100;
        if (whole <= 0) return false;
        state.DealDamage(curse.SourceUnitId, unit.Id, whole, DamageSource.Periodic);
        return true;
    }

    /// <summary>
    /// A skill or attack used up one use of a status that counts uses (an enchantment): with more than one use left it only goes down, otherwise the
    /// status ends (Lingering Magic gives the enchantments two uses).
    /// </summary>
    public static void ConsumeUse(BattleState state, int unitId, string statusId)
    {
        var instance = Get(state.GetUnit(unitId), statusId);
        if (instance is null) return;
        if (instance.UsesLeft is > 1) instance.UsesLeft--;
        else Remove(state, unitId, statusId, StatusEndCause.Consumed);
    }

    /// <summary>Regrowth heals Magnitude HP per 10 time units, built up per time unit and landing whenever any unit gets a turn (section 10.5); the fraction carries over.</summary>
    private static bool LandRegrowth(BattleState state, Unit unit, StatusInstance regrowth)
    {
        if (regrowth.Held) return false;
        var until = Math.Min(state.Timeline.Now, regrowth.EndsAt ?? state.Timeline.Now);
        regrowth.BuildupHundredths += Math.Max(0, until - regrowth.LastTickTime) * regrowth.Magnitude * 10;
        regrowth.LastTickTime = Math.Max(regrowth.LastTickTime, until);
        var whole = regrowth.BuildupHundredths / 100;
        regrowth.BuildupHundredths %= 100;
        if (whole <= 0) return false;
        state.Heal(regrowth.SourceUnitId, unit.Id, whole);
        return unit.IsKnockedOut;
    }

    /// <summary>
    /// Changes how fast a timed status runs down (Purifying Aura: twice as fast next to the Cleric). The time left, counted at the old rate, becomes
    /// the time left at the new rate (rounded down, at least 1); its end marker moves on the timeline.
    /// </summary>
    public static void SetRate(BattleState state, StatusInstance status, int rate)
    {
        if (status.AuraRate == rate) return;
        if (status.MarkerEntryId is { } marker && !status.Held && state.Timeline.Find(marker) is { } entry)
        {
            var remaining = Math.Max(0, entry.Due - state.Timeline.Now);
            var scaled = Math.Max(1, remaining * status.AuraRate / rate);
            state.Timeline.AdjustRemaining(entry, scaled - remaining);
            status.EndsAt = state.Timeline.Now + scaled;
        }
        status.AuraRate = rate;
    }

    private static bool LandPoison(BattleState state, Unit unit, StatusInstance poison)
    {
        if (poison.Held) return false;
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
        var frozenStatus = Get(unit, StatusIds.Frozen);
        var frozen = frozenStatus is not null;
        if (frozenStatus is { UsesLeft: > 1 }) frozenStatus.UsesLeft--;        // Cold Heart: Frozen skips more than one turn
        else Remove(state, unitId, StatusIds.Frozen, StatusEndCause.Consumed);
        return new TurnStartResult(frozen, Has(unit, StatusIds.Stunned) || Has(unit, StatusIds.StoneShell));
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

    /// <summary>
    /// A unit uses a skill: the extra Delay its next skill carries (Net Trap's +3) is added once and the status ends.
    /// Returns the extra Delay, 0 if none.
    /// </summary>
    public static int ConsumeNextSkillDelay(BattleState state, int unitId)
    {
        var status = Get(state.GetUnit(unitId), StatusIds.NextSkillDelay);
        if (status is null) return 0;
        var extra = status.Magnitude;
        Remove(state, unitId, StatusIds.NextSkillDelay, StatusEndCause.Consumed);
        return extra;
    }

    /// <summary>A Mark lasts 30 time units unless the marking skill says otherwise (game_system.md section 10.5).</summary>
    public const int MarkLength = 30;

    /// <summary>
    /// Steals helpful statuses from a unit (game_system.md section 10.5): one, picked by the seeded roll with equal chance, or every one with
    /// <paramref name="all"/>. Each moves to the thief with its remaining time. Stealth cannot be stolen. Returns how many moved.
    /// </summary>
    public static int Steal(BattleState state, int thiefId, int fromId, bool all)
    {
        var from = state.GetUnit(fromId);
        var ids = from.StatusInstances.Keys.Where(id => StatusRules.PolarityOf(id) == Polarity.Helpful).ToList();
        if (from.Ward > 0 && !ids.Contains(StatusIds.Ward)) ids.Add(StatusIds.Ward);
        var stolen = 0;
        while (ids.Count > 0)
        {
            var pick = ids.Count == 1 ? 0 : state.Random.NextInt(ids.Count);
            var id = ids[pick];
            ids.RemoveAt(pick);
            if (Transfer(state, fromId, thiefId, id, thiefId)) stolen++;
            if (!all) break;
        }
        return stolen;
    }

    /// <summary>
    /// Moves a status from one unit to another (Steal Blessing, Turn Back): it leaves the first unit and starts on the second with what
    /// time, amount, source and count it had left. The second unit's reapplication rule applies (a smaller Ward does not replace a larger
    /// one). Returns false if the second unit did not take it.
    /// </summary>
    public static bool Transfer(BattleState state, int fromId, int toId, string statusId, int? byUnitId = null)
    {
        var from = state.GetUnit(fromId);
        var instance = Get(from, statusId);
        var magnitude = statusId == StatusIds.Ward ? from.Ward : instance?.Magnitude ?? 0;
        if (instance is null && !(statusId == StatusIds.Ward && from.Ward > 0)) return false;
        int? remaining = instance?.TimeLeft(state.Timeline.Now) is { } left ? Math.Max(1, left) : null;
        var application = new StatusApplication(toId, statusId, remaining, magnitude, instance?.SourceUnitId, instance?.EndsAtNextTurnOf, instance?.UsesLeft);
        if (instance is not null) Remove(state, fromId, statusId, StatusEndCause.Cleansed, byUnitId);
        else from.Ward = 0;
        return Apply(state, application);
    }

    /// <summary>A unit was hit: any hit ends Stunned early (section 10.5).</summary>
    public static void OnHit(BattleState state, int unitId) =>
        Remove(state, unitId, StatusIds.Stunned, StatusEndCause.Hit);
}


/// <summary>A status a unit had when it was KO'd, with its remaining time (null: no time limit), magnitude and source.</summary>
public sealed record StatusSnapshot(string Id, int? TimeLeft, int Magnitude, int? SourceUnitId);
