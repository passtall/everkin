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

/// <summary>Two units that share one HP pool (Bound Resurrect, game_system section 10.4).</summary>
public sealed class SharedHp
{
    public SharedHp(int unitA, int unitB, int pool, int max, bool boundStrength)
    {
        UnitA = unitA;
        UnitB = unitB;
        Pool = pool;
        Max = max;
        BoundStrength = boundStrength;
    }

    public int UnitA { get; }
    public int UnitB { get; }

    /// <summary>The pool's current HP; the maximum is the sum of both units' maximum HP.</summary>
    public int Pool { get; internal set; }

    public int Max { get; }

    /// <summary>The Healer knew Bound Strength: while the two share their pool, both have 1 less Delay on every skill.</summary>
    public bool BoundStrength { get; }

    internal SharedHp Clone() => (SharedHp)MemberwiseClone();
}

/// <summary>True when the unit cannot intercept for others (Keeper of the Gate).</summary>
public delegate bool InterceptionBlocker(BattleState state, int unitId);

/// <summary>True when the interceptor always takes the whole hit, never a partial one (Full Cover).</summary>
public delegate bool FullHitRule(BattleState state, int interceptorId);

/// <summary>A unit that always intercepts for the target, with a 100% chance, before the usual roll (Shield Wall, Living Wall); or null.</summary>
public delegate int? GuaranteedInterceptor(BattleState state, int targetId);

/// <summary>True when an effect caused by the given side cannot push, pull or swap the unit (Hold Position).</summary>
public delegate bool ForcedMoveImmunity(BattleState state, int unitId, Side causedBy);

/// <summary>For an area effect that would hit the target: another unit that is hit in its place, or null (Pledge).</summary>
public delegate int? AreaRedirect(BattleState state, int targetId);

/// <summary>Called after a unit has moved, by its own Move or by forced movement (Follow the Charge).</summary>
public delegate void AfterMove(BattleState state, int movedUnitId);

/// <summary>True when the unit can never attack (Living Wall).</summary>
public delegate bool AttackBan(BattleState state, int unitId);
