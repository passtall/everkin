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

/// <summary>Units that share one HP pool (Bound Resurrect, game_system section 10.4): the Lifebinder and everyone it revived with it while it was sharing.</summary>
public sealed class SharedHp
{
    public SharedHp(int unitA, int unitB, int pool, int max, bool boundStrength)
    {
        Members = new List<int> { unitA, unitB };
        Pool = pool;
        Max = max;
        BoundStrength = boundStrength;
    }

    public List<int> Members { get; private set; }
    public int UnitA => Members[0];
    public int UnitB => Members[1];

    /// <summary>The pool's current HP; the maximum is the sum of both units' maximum HP.</summary>
    public int Pool { get; internal set; }

    public int Max { get; internal set; }

    /// <summary>The Healer knew Bound Strength: while the two share their pool, both have 1 less Delay on every skill.</summary>
    public bool BoundStrength { get; }

    internal SharedHp Clone() { var c = (SharedHp)MemberwiseClone(); c.Members = new List<int>(Members); return c; }
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

/// <summary>A status was cleansed off a unit by another unit's skill (Lingering Malice).</summary>
public delegate void StatusRemovedHook(BattleState state, int unitId, StatusInstance status, int byUnitId);

/// <summary>Changes the damage a hit would do to a target before shields (Brittle). Returns the new amount.</summary>
public delegate int DamageModifier(BattleState state, int? sourceUnitId, int targetId, int amount);

/// <summary>A percentage added to the damage of the attacker's hits (Reckless Swing +100, Last Stand +50). All bonuses of a hit are added together.</summary>
public delegate int DamageBonus(BattleState state, int attackerId, int targetId);

/// <summary>Called after a hit with the HP the target and its interceptors actually lost (Reckless Swing's recoil).</summary>
public delegate void PostHitHook(BattleState state, int attackerId, int targetId, int hpLost);

/// <summary>Called when an attack misses its target (Sidestep, Blind): Counter Step.</summary>
public delegate void AttackMissHook(BattleState state, int attackerId, int targetId);

/// <summary>The HP that damage from statuses cannot take a unit below (Endure: 1). 0 for no floor.</summary>
public delegate int DamageFloor(BattleState state, int unitId);

/// <summary>True when the unit cannot gain the status (Unstoppable: Immobilized, Frozen).</summary>
public delegate bool StatusImmunity(BattleState state, int unitId, string statusId);

/// <summary>True when the unit's turn cannot be pushed back (Unstoppable).</summary>
public delegate bool TurnPushImmunity(BattleState state, int unitId);

/// <summary>True when the unit cannot use the Move action (Last Stand).</summary>
public delegate bool MoveBan(BattleState state, int unitId);

/// <summary>False when the user may not target that unit (Single Combat).</summary>
public delegate bool TargetRestriction(BattleState state, int userId, int targetId);

/// <summary>Extra units that an attack by this unit also hits (Crowd Fighter).</summary>
public delegate IReadOnlyList<int> AttackExpander(BattleState state, int attackerId);

/// <summary>Changes the Momentum shift of an action by the user's hits (All In). Returns the new shift (from the user's view).</summary>
public delegate int MomentumShiftModifier(BattleState state, int userId, int shift);

/// <summary>Changes the Momentum cost of a skill for its user (All In). Returns the new cost.</summary>
public delegate int MomentumCostModifier(BattleState state, int userId, int cost);

/// <summary>Less Delay for a skill the unit uses again right after using it (Rhythm). Returns how much less.</summary>
public delegate int RepeatDelayReduction(BattleState state, int userId, string skillId);

/// <summary>Called when a hit reaches its target itself (landed or fully absorbed, not taken by an interceptor): Light Fingers.</summary>
public delegate void TargetHitHook(BattleState state, int attackerId, int targetId);

/// <summary>Changes a status before it is applied (Showman: Blind and Stunned from the Tinker last twice as long).</summary>
public delegate StatusApplication StatusApplicationModifier(BattleState state, StatusApplication application);

/// <summary>Extra Delay for a skill a unit uses (Big Finish: +2 on Firebomb and Poison Gas). Returns the extra Delay.</summary>
public delegate int SkillDelayModifier(BattleState state, int userId, string skillId);

/// <summary>The other units that share the damage a unit takes (Spirit Link Totem). Empty for none.</summary>
public delegate IReadOnlyList<int> DamageSharing(BattleState state, int targetId);

/// <summary>A unit that would drop to 0 HP is saved by a totem (Totem of Return): returns the totem that breaks, or null.</summary>
public delegate int? LethalGuard(BattleState state, int targetId);

/// <summary>Called after a unit took an action (a skill, Move or Skip Turn): the Engineer's turrets fire.</summary>
public delegate void AfterActionHook(BattleState state, int userId);

/// <summary>An auto event reached its turn; returns true when it was handled.</summary>
public delegate bool AutoEventHandler(BattleState state, TimelineEntry entry);

/// <summary>A unit that would drop to 0 HP survives with 1 HP when this returns true; the hook keeps its own count (Unbreakable: the first time only).</summary>
public delegate bool LethalSave(BattleState state, int targetId);

/// <summary>A hit (damage from a skill, not from a status) was partly or fully absorbed by the target's Ward (Thorned Ward).</summary>
public delegate void WardHitHook(BattleState state, int? sourceUnitId, int targetId, int absorbed);

/// <summary>A unit lost HP from any damage (a hit, Poisoned, Bleeding, ground damage): its HP before and after (Scent of Blood).</summary>
public delegate void AfterDamageHook(BattleState state, int targetId, int hpBefore, int hpAfter);

/// <summary>True when a cleansing skill cannot remove this status instance (Relentless: the Tracker's Marks).</summary>
public delegate bool StatusRemovalVeto(BattleState state, int unitId, StatusInstance status, int? byUnitId);

/// <summary>A unit stole statuses (Snatch, an attack that steals): who stole, from whom and how many (Shadow Purse).</summary>
public delegate void StealHook(BattleState state, int thiefId, int fromId, int count);

/// <summary>A unit's turn has just started and it will act (Bone Armor renews its Ward).</summary>
public delegate void TurnStartHook(BattleState state, int unitId);

/// <summary>A summon or construct left the board for good, with the position it had (Spirit Shield: a totem broke).</summary>
public delegate void RemovalHook(BattleState state, int unitId, RemovalCause cause, Position from);
