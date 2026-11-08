namespace Everkin.Core.Battle;

/// <summary>
/// Base of every event the core reports (production.md section 2.2): hit, intercept, move, status applied,
/// Momentum shift, KO and so on. Later tasks add the concrete events as their rules arrive; the UI plays
/// them in order. Events are plain immutable data.
/// </summary>
public abstract record BattleEvent;

/// <summary>The battle exists with this seed.</summary>
public sealed record BattleStarted(ulong Seed) : BattleEvent;
