namespace Everkin.Core.Battle;

/// <summary>A hit that landed on a Marked unit: which action, who hit, who was hit.</summary>
public sealed record MarkedHit(int ActionIndex, int AttackerId, int TargetId);

/// <summary>A row of one side: what a skill that picks a row selects.</summary>
public readonly record struct RowTarget(Side Side, Row Row);

/// <summary>True when the unit cannot gain stealth.</summary>
public delegate bool StealthBlocker(BattleState state, int unitId);

/// <summary>True when the stealth of a unit that is being hit survives (the hit still lands).</summary>
public delegate bool StealthBreakVeto(BattleState state, int unitId, StealthCause cause, bool area);

/// <summary>Called before an attack resolves against a target; returns another unit to aim it at, or null to leave it.</summary>
public delegate int? PreHitHook(BattleState state, int attackerId, int targetId);
