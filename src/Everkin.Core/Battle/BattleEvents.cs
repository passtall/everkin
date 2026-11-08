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
