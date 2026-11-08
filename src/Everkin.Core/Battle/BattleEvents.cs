namespace Everkin.Core.Battle;

/// <summary>
/// Base of every event the core reports (production.md section 2.2): hit, intercept, move, status applied,
/// Momentum shift, KO and so on. Later tasks add the concrete events as their rules arrive; the UI plays
/// them in order. Events are plain immutable data.
/// </summary>
public abstract record BattleEvent;

/// <summary>A unit used a skill (the first event of the action).</summary>
public sealed record SkillUsed(int UserId, string SkillId, int? TargetUnitId) : BattleEvent;

/// <summary>A shared HP pool changed (Bound Resurrect). Each unit's HP shows the pool, capped at its own maximum.</summary>
public sealed record SharedHpChanged(int UnitA, int UnitB, int Pool, int Max) : BattleEvent;

/// <summary>A skill was used but lost: its cost and Delay were paid and it had no effect (Lie in Wait).</summary>
public sealed record SkillLost(int UserId, string SkillId) : BattleEvent;

/// <summary>A unit's turn began (after the statuses and ground effects of the turn start were counted).</summary>
public sealed record TurnStarted(int UnitId, int Time) : BattleEvent;

/// <summary>A unit's turn was skipped (Frozen or Stunned) and it waits the cost of a skipped turn.</summary>
public sealed record TurnSkipped(int UnitId) : BattleEvent;

/// <summary>An attack missed (a Blind unit's 50% chance).</summary>
public sealed record AttackMissed(int UserId, int TargetUnitId) : BattleEvent;

/// <summary>The battle exists with this seed.</summary>
public sealed record BattleStarted(ulong Seed) : BattleEvent;

/// <summary>A unit took damage. HP never goes below 0; reaching 0 means KO (handled by the KO rules).</summary>
/// <param name="SourceUnitId">The unit that caused it, or null for ground effects and the like.</param>
public sealed record DamageDealt(
    int? SourceUnitId, int TargetUnitId, DamageSource Source, DamageResult Result, int HpAfter) : BattleEvent;

/// <summary>A unit was healed by the amount that actually fitted under its maximum HP.</summary>
public sealed record Healed(int? SourceUnitId, int TargetUnitId, int Amount, int HpAfter) : BattleEvent;

/// <summary>A unit gained a Ward (or kept its larger one).</summary>
public sealed record WardChanged(int UnitId, int WardBefore, int WardAfter) : BattleEvent;

/// <summary>Why a unit changed its place.</summary>
public enum MoveCause
{
    /// <summary>The unit's own Move action.</summary>
    Voluntary,

    /// <summary>A push, pull, swap, a skill's own movement, or being bumped along a chain.</summary>
    Forced,
}

/// <summary>A unit changed its place on the board.</summary>
public sealed record UnitMoved(int UnitId, Position From, Position To, MoveCause Cause) : BattleEvent;

/// <summary>Two units swapped places.</summary>
public sealed record UnitsSwapped(int UnitA, int UnitB) : BattleEvent;

/// <summary>A forced move was blocked: nothing moved, and each unit in the chain took 1 bump damage.</summary>
public sealed record BumpBlocked(int MovedUnitId, IReadOnlyList<int> Chain) : BattleEvent;

/// <summary>A unit reached 0 HP and the KO rules ran: statuses cleared, pending events removed, the body keeps its slot.</summary>
public sealed record UnitKnockedOut(int UnitId) : BattleEvent;

/// <summary>A KO unit was revived at its slot with the given HP and has a turn on the timeline again.</summary>
public sealed record UnitRevived(int UnitId, int Hp) : BattleEvent;

/// <summary>Why a unit's stealth changed.</summary>
public enum StealthCause
{
    Gained,
    Attacked,
    Hit,
    Revealed,
    KnockedOut,
}

/// <summary>A unit gained or lost stealth.</summary>
public sealed record StealthChanged(int UnitId, bool Stealthed, StealthCause Cause) : BattleEvent;

/// <summary>A status was applied or refreshed.</summary>
/// <param name="Magnitude">Its size where it has one (Ward amount, Speed steps), else 0.</param>
/// <param name="Duration">Time units it lasts, or null when it has no clock (until broken, until a turn, a count of skills).</param>
public sealed record StatusApplied(int UnitId, string StatusId, int Magnitude, int? Duration, bool Refreshed) : BattleEvent;

/// <summary>Why a status ended.</summary>
public enum StatusEndCause
{
    Expired,
    Cleansed,
    Consumed,
    Broken,
    Hit,
    Fire,
    Landed,
}

/// <summary>A status ended.</summary>
public sealed record StatusEnded(int UnitId, string StatusId, StatusEndCause Cause) : BattleEvent;

/// <summary>A summon, construct or totem appeared.</summary>
public sealed record UnitSummoned(int UnitId, int SummonerId, UnitKind Kind, Position Position) : BattleEvent;

/// <summary>Why a unit left the board for good.</summary>
public enum RemovalCause
{
    /// <summary>KO'd: a KO'd summon or trap is removed at once.</summary>
    Ko,

    /// <summary>A temporary unit's time ran out. It is not a KO: no KO-triggered effects fire.</summary>
    Expired,

    /// <summary>Its summoner was KO'd and its skill says it disappears with it.</summary>
    SummonerGone,
}

/// <summary>A unit left the board and its slot is free.</summary>
public sealed record UnitRemoved(int UnitId, RemovalCause Cause, Position From) : BattleEvent;

/// <summary>A necromancer raised a KO body as an undead on its own side.</summary>
public sealed record UnitRaised(int UnitId, int NecromancerId, Position From, Position To, int Hp) : BattleEvent;

/// <summary>A resurrection restored an undead to the original creature on its owner's side.</summary>
public sealed record UndeadRestored(int UnitId, Position From, Position To, int Hp) : BattleEvent;
