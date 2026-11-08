namespace Everkin.Core.Battle;

/// <summary>
/// Base of every event the core reports (production.md section 2.2): hit, intercept, move, status applied,
/// Momentum shift, KO and so on. Later tasks add the concrete events as their rules arrive; the UI plays
/// them in order. Events are plain immutable data.
/// </summary>
public abstract record BattleEvent;

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
