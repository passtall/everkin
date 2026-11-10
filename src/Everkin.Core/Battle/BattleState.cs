using Everkin.Core.Content;

namespace Everkin.Core.Battle;

/// <summary>The two sides of a battle. Which one a human or the AI plays is up to the caller.</summary>
public enum Side
{
    A = 0,
    B = 1,
}

public static class SideExtensions
{
    public static Side Opposite(this Side side) => side == Side.A ? Side.B : Side.A;
}

/// <summary>What kind of unit it is (game_system.md section 11.1).</summary>
public enum UnitKind
{
    /// <summary>A creature of a party (an original member). Its body stays when it is KO.</summary>
    Creature,

    /// <summary>A summoned full unit: takes turns. KO'd, it is removed at once and frees its slot.</summary>
    Summon,

    /// <summary>A summoned unit that expires after a time and disappears without counting as a KO.</summary>
    Temporary,

    /// <summary>A stationary construct, trap or totem: has HP, can be targeted and intercept, takes no turns of its own.</summary>
    Construct,
}

/// <summary>
/// A unit in a battle. Holds the four stats of game_system.md section 5.1 as they are now (Cost is not used in combat).
/// Formation, timeline, statuses and Momentum are added by their own tasks.
/// </summary>
public sealed class Unit
{
    public Unit(int id, Side side, string name, string? creatureId, int maxHp, int speed, int power)
    {
        Id = id;
        Side = side;
        Name = name;
        CreatureId = creatureId;
        OriginalSide = side;
        MaxHp = maxHp;
        Hp = maxHp;
        Speed = speed;
        Power = power;
    }

    private Unit(Unit other)
    {
        Id = other.Id;
        Side = other.Side;
        Name = other.Name;
        CreatureId = other.CreatureId;
        MaxHp = other.MaxHp;
        Hp = other.Hp;
        Speed = other.Speed;
        Power = other.Power;
        IsStealthed = other.IsStealthed;
        Ward = other.Ward;
        Dampen = other.Dampen;
        InterceptionBlocked = other.InterceptionBlocked;
        IsImmobilized = other.IsImmobilized;
        KoProcessed = other.KoProcessed;
        IsAirborne = other.IsAirborne;
        OriginalSide = other.OriginalSide;
        Kind = other.Kind;
        SummonerId = other.SummonerId;
        DisappearsWithSummoner = other.DisappearsWithSummoner;
        IsRemoved = other.IsRemoved;
        IsUndead = other.IsUndead;
        OriginalRow = other.OriginalRow;
        OriginalMaxHp = other.OriginalMaxHp;
        LastActionIndex = other.LastActionIndex;
        LastHitBy = other.LastHitBy;
        _knownSkills.AddRange(other._knownSkills);
        OriginalSpeed = other.OriginalSpeed;
        OriginalPower = other.OriginalPower;
        LastSkillId = other.LastSkillId;
        LastActiveSkillId = other.LastActiveSkillId;
        MarkedAtKo = other.MarkedAtKo;
        SneakReady = other.SneakReady;
        AutoSkillId = other.AutoSkillId;
        NoReviveUntil = other.NoReviveUntil;
        foreach (var kv in other._statusInstances) _statusInstances[kv.Key] = kv.Value.Clone();
    }

    /// <summary>Unique inside the battle, assigned in the order units were added.</summary>
    public int Id { get; }

    /// <summary>The side the unit fights on now. An undead fights on its necromancer's side until it is restored.</summary>
    public Side Side { get; internal set; }

    /// <summary>The side the unit started on: its owner for the defeat check (an original raised as undead still counts as KO for it).</summary>
    public Side OriginalSide { get; }

    public UnitKind Kind { get; internal set; } = UnitKind.Creature;

    /// <summary>The unit that summoned or raised it. A summon's damage and healing scale with this unit's Power (section 5.2).</summary>
    public int? SummonerId { get; internal set; }

    /// <summary>The summoning skill said it disappears when its summoner is KO (it persists otherwise).</summary>
    public bool DisappearsWithSummoner { get; internal set; }

    /// <summary>Off the board for good: a KO'd or expired summon. Its slot is free and it cannot be revived.</summary>
    public bool IsRemoved { get; internal set; }

    /// <summary>Raised by a necromancer (section 11.5). Healing it hurts it; a resurrection restores the original creature.</summary>
    public bool IsUndead { get; internal set; }

    /// <summary>The row the body had before it was raised, for the free slot nearest it on restoration.</summary>
    public Row? OriginalRow { get; internal set; }

    public int? OriginalMaxHp { get; internal set; }
    public int? OriginalSpeed { get; internal set; }
    public int? OriginalPower { get; internal set; }

    /// <summary>A summon, construct or totem (not a creature of a party).</summary>
    public bool IsSummoned => Kind != UnitKind.Creature;

    /// <summary>The number of the unit's last action (skill, Move or Skip Turn), for rules that look at what happened since then.</summary>
    public int LastActionIndex { get; internal set; }

    /// <summary>The unit that hit this unit most recently (Distract looks at who last hit the Charge).</summary>
    public int? LastHitBy { get; internal set; }

    private readonly List<string> _knownSkills = new();

    /// <summary>
    /// The skills the unit can use besides Move and Skip Turn: its Attack Type attack and what it learned (section 4.4).
    /// All unlocked skills are available in combat. Set up before the battle (a build, or by hand in tests).
    /// </summary>
    public IReadOnlyList<string> KnownSkills => _knownSkills;

    public void LearnSkill(string skillId)
    {
        if (!_knownSkills.Contains(skillId)) _knownSkills.Add(skillId);
    }
    public string Name { get; }

    /// <summary>The roster creature's unit name, or null for units that do not come from the roster.</summary>
    public string? CreatureId { get; }

    public int MaxHp { get; set; }
    public int Hp { get; set; }
    public int Speed { get; set; }
    public int Power { get; set; }

    /// <summary>The skill this unit used last, null after a Move or Skip Turn (Rhythm: the same skill twice in a row).</summary>
    public string? LastSkillId { get; set; }

    /// <summary>The last skill this unit used with a turn action (not cleared by Move or Skip Turn): Lift a Skill copies it.</summary>
    public string? LastActiveSkillId { get; set; }

    /// <summary>It was Marked when it was KO'd (the KO clears statuses): Deathmark Bloom.</summary>
    public bool MarkedAtKo { get; set; }

    /// <summary>The turn has just started and the unit has not acted yet: Sneak's extra move is allowed.</summary>
    public bool SneakReady { get; set; }

    /// <summary>The Animate Mirror is answering a hit right now (guards against two mirrors answering each other forever).</summary>
    public bool Answering { get; set; }

    /// <summary>The hidden skill an autonomous summon or undead uses on each of its turns (the core plays it), or null for units a player or AI controls.</summary>
    public string? AutoSkillId { get; set; }

    /// <summary>Desecrate: no resurrection works on this body before this battle time.</summary>
    public int? NoReviveUntil { get; set; }

    /// <summary>Healing hurts this unit: an undead, or a Summoner with Lich Form.</summary>
    public bool HealingHurts => IsUndead || _knownSkills.Contains("lich_form");

    /// <summary>Knocked out: HP is 0 (game_system.md section 10.2). A KO body keeps its place in the row.</summary>
    public bool IsKnockedOut => Hp <= 0;

    /// <summary>
    /// Stealthed (section 10.1). Set by the stealth rules (M1.10); the targeting code only reads it: a stealthed unit
    /// cannot be targeted by the other side and does not make a row count for the other side's range.
    /// </summary>
    public bool IsStealthed { get; set; }

    /// <summary>
    /// What the unit's Ward still absorbs, 0 for none (section 5.2). The status layer (M1.11) will add its duration and
    /// removal; the damage rules only read and spend this number.
    /// </summary>
    public int Ward { get; set; }

    /// <summary>Dampen halves damage that gets through the shield (section 5.2). M1.11 adds its duration.</summary>
    public bool Dampen { get; set; }

    /// <summary>
    /// A status forbids intercepting: Stunned, Frozen or Immobilized (section 8.2). The status rules (M1.11) set and clear
    /// it; the interception code only reads it.
    /// </summary>
    public bool InterceptionBlocked { get; set; }

    /// <summary>
    /// Immobilized (section 10.5): the unit cannot Move and cannot be moved by any skill. The status rules (M1.11) set and
    /// clear it; the movement code only reads it.
    /// </summary>
    public bool IsImmobilized { get; set; }

    /// <summary>True once the KO rules have run for this unit (statuses cleared, pending events removed). Reset by revival.</summary>
    public bool KoProcessed { get; set; }

    /// <summary>Airborne (section 10.6): melee cannot reach the unit and it ignores ground effects. Kept by the status rules.</summary>
    public bool IsAirborne { get; set; }

    private readonly Dictionary<string, StatusInstance> _statusInstances = new();

    /// <summary>The statuses that run on the battle clock or count something, one instance per status type (section 10.5).</summary>
    public IReadOnlyDictionary<string, StatusInstance> StatusInstances => _statusInstances;

    internal Dictionary<string, StatusInstance> StatusStore => _statusInstances;

    /// <summary>
    /// Speed as the turn formula sees it before clamping to 1–10: the creature's Speed plus Haste, minus Slow and Chilled
    /// (section 6.1). The timeline clamps it when it computes a Wait.
    /// </summary>
    public int EffectiveSpeed => (_statusInstances.ContainsKey(StatusIds.Transmuted) ? Power : Speed) + _statusInstances.Values.Sum(StatusRules.SpeedModifier);

    /// <summary>Power as the damage and healing rules see it: the creature's Power, or its Speed while Transmuted (Power and Speed swap).</summary>
    public int EffectivePower => _statusInstances.ContainsKey(StatusIds.Transmuted) ? Speed : Power;

    /// <summary>
    /// Removes every status: KO removes all of them, helpful and harmful alike (section 10.3). Today these are Ward, Dampen,
    /// stealth, Immobilized and the interception block; the status layer (M1.11) adds the others.
    /// </summary>
    public void ClearStatuses()
    {
        Ward = 0;
        Dampen = false;
        IsStealthed = false;
        IsImmobilized = false;
        InterceptionBlocked = false;
        IsAirborne = false;
        _statusInstances.Clear();
    }

    public Unit Clone() => new(this);
}

/// <summary>
/// The whole mutable state of one battle. Only the core changes it; every action reports what happened as an
/// ordered event list (production.md section 2.2). <see cref="Clone"/> is cheap and fully independent, so the AI
/// search can copy the state before trying an action.
/// </summary>
public sealed class BattleState
{
    private readonly List<Unit> _units = new();
    private readonly List<BattleEvent> _pendingEvents = new();
    private readonly Formation _formation;
    private readonly Timeline _timeline;
    private readonly Momentum _momentum;
    private readonly GroundField _ground;
    private readonly List<MarkedHit> _markedHits = new();
    private readonly List<StealthBlocker> _stealthBlockers = new();
    private readonly List<StealthBreakVeto> _stealthVetoes = new();
    private readonly List<PreHitHook> _preHitHooks = new();
    private readonly List<InterceptionBlocker> _interceptionBlockers = new();
    private readonly List<FullHitRule> _fullHitRules = new();
    private readonly List<GuaranteedInterceptor> _guaranteedInterceptors = new();
    private readonly List<ForcedMoveImmunity> _forcedMoveImmunities = new();
    private readonly List<AreaRedirect> _areaRedirects = new();
    private readonly List<AfterMove> _afterMoveHooks = new();
    private readonly List<AttackBan> _attackBans = new();
    private readonly List<StatusRemovedHook> _statusRemovedHooks = new();
    private readonly List<DamageModifier> _damageModifiers = new();
    private readonly List<DamageSharing> _damageSharings = new();
    private readonly List<LethalGuard> _lethalGuards = new();
    private readonly List<StatusApplicationModifier> _statusApplicationModifiers = new();
    private readonly List<SkillDelayModifier> _skillDelayModifiers = new();
    private readonly List<TargetHitHook> _targetHitHooks = new();
    private readonly List<MomentumShiftModifier> _momentumShiftModifiers = new();
    private readonly List<MomentumCostModifier> _momentumCostModifiers = new();
    private readonly List<RepeatDelayReduction> _repeatDelayReductions = new();
    private readonly List<(string Key, Action<BattleState> Sync)> _auraSyncs = new();
    private readonly HashSet<string> _installedKeys = new();
    private readonly List<DamageBonus> _damageBonuses = new();
    private readonly List<PostHitHook> _postHitHooks = new();
    private readonly List<AttackMissHook> _attackMissHooks = new();
    private readonly List<DamageFloor> _damageFloors = new();
    private readonly List<StatusImmunity> _statusImmunities = new();
    private readonly List<TurnPushImmunity> _turnPushImmunities = new();
    private readonly List<MoveBan> _moveBans = new();
    private readonly List<TargetRestriction> _targetRestrictions = new();
    private readonly List<AttackExpander> _attackExpanders = new();
    private readonly List<SharedHp> _sharedPools = new();
    private readonly List<ReactionDef> _reactions = new();
    private readonly Dictionary<string, int> _reactionUses = new();

    public BattleState(ulong seed)
    {
        Random = new BattleRandom(seed);
        _formation = new Formation();
        _timeline = new Timeline();
        _momentum = new Momentum();
        _ground = new GroundField(this);
        Emit(new BattleStarted(seed));
    }

    private BattleState(BattleState other)
    {
        Random = other.Random.Clone();
        _formation = other._formation.Clone();
        _timeline = other._timeline.Clone();
        _momentum = other._momentum.Clone();
        _ground = new GroundField(this, other._ground);
        ActionCounter = other.ActionCounter;
        _markedHits.AddRange(other._markedHits);
        _stealthBlockers.AddRange(other._stealthBlockers);
        _stealthVetoes.AddRange(other._stealthVetoes);
        _preHitHooks.AddRange(other._preHitHooks);
        _interceptionBlockers.AddRange(other._interceptionBlockers);
        _fullHitRules.AddRange(other._fullHitRules);
        _guaranteedInterceptors.AddRange(other._guaranteedInterceptors);
        _forcedMoveImmunities.AddRange(other._forcedMoveImmunities);
        _areaRedirects.AddRange(other._areaRedirects);
        _afterMoveHooks.AddRange(other._afterMoveHooks);
        _attackBans.AddRange(other._attackBans);
        _statusRemovedHooks.AddRange(other._statusRemovedHooks);
        _damageModifiers.AddRange(other._damageModifiers);
        _damageSharings.AddRange(other._damageSharings);
        _lethalGuards.AddRange(other._lethalGuards);
        MistsUntil = other.MistsUntil;
        _statusApplicationModifiers.AddRange(other._statusApplicationModifiers);
        _skillDelayModifiers.AddRange(other._skillDelayModifiers);
        _targetHitHooks.AddRange(other._targetHitHooks);
        _momentumShiftModifiers.AddRange(other._momentumShiftModifiers);
        _momentumCostModifiers.AddRange(other._momentumCostModifiers);
        _repeatDelayReductions.AddRange(other._repeatDelayReductions);
        _auraSyncs.AddRange(other._auraSyncs);
        foreach (var key in other._installedKeys) _installedKeys.Add(key);
        _damageBonuses.AddRange(other._damageBonuses);
        _postHitHooks.AddRange(other._postHitHooks);
        _attackMissHooks.AddRange(other._attackMissHooks);
        _damageFloors.AddRange(other._damageFloors);
        _statusImmunities.AddRange(other._statusImmunities);
        _turnPushImmunities.AddRange(other._turnPushImmunities);
        _moveBans.AddRange(other._moveBans);
        _targetRestrictions.AddRange(other._targetRestrictions);
        _attackExpanders.AddRange(other._attackExpanders);
        _sharedPools.AddRange(other._sharedPools.Select(p => p.Clone()));
        Outcome = other.Outcome;
        EndReason = other.EndReason;
        TurnsTaken = other.TurnsTaken;
        _reactions.AddRange(other._reactions);
        foreach (var kv in other._reactionUses) _reactionUses[kv.Key] = kv.Value;
        foreach (var u in other._units) _units.Add(u.Clone());
        // The copy starts with no pending events: they belong to the action that produced them.
    }

    /// <summary>The battle's own random generator. Nothing else in a battle may draw random numbers.</summary>
    public BattleRandom Random { get; }

    public ulong Seed => Random.Seed;

    public IReadOnlyList<Unit> Units => _units;

    /// <summary>Where every unit stands (rows, order, alignment, adjacency).</summary>
    public Formation Formation => _formation;

    /// <summary>The shared timeline: turns, auto events, delayed events and effect ends.</summary>
    public Timeline Timeline => _timeline;

    /// <summary>The contested Momentum meter. Change it through <see cref="PayMomentum"/> and <see cref="ShiftMomentum"/> so the change is reported.</summary>
    public Momentum Momentum => _momentum;

    /// <summary>The number of actions taken so far in the battle. Every skill, Move and Skip Turn counts one.</summary>
    public int ActionCounter { get; private set; }

    /// <summary>Starts the next action and returns its number.</summary>
    public int BeginAction() => ++ActionCounter;

    /// <summary>Hits that landed on Marked units, for skills that look at what allies did since a unit's last action (Pack Strike).</summary>
    public IReadOnlyList<MarkedHit> MarkedHits => _markedHits;

    public void LogMarkedHit(int attackerId, int targetId) => _markedHits.Add(new MarkedHit(ActionCounter, attackerId, targetId));

    /// <summary>Passives that stop a unit from gaining stealth (Scent). Installed at battle start.</summary>
    public IReadOnlyList<StealthBlocker> StealthBlockers => _stealthBlockers;

    public void AddStealthBlocker(StealthBlocker blocker) => _stealthBlockers.Add(blocker);

    /// <summary>Passives that can stop a hit from breaking stealth (Freeze in Place).</summary>
    public IReadOnlyList<StealthBreakVeto> StealthBreakVetoes => _stealthVetoes;

    public void AddStealthBreakVeto(StealthBreakVeto veto) => _stealthVetoes.Add(veto);

    /// <summary>Passives that act before an attack resolves and may aim it at another unit (Decoy).</summary>
    public IReadOnlyList<PreHitHook> PreHitHooks => _preHitHooks;

    public void AddPreHitHook(PreHitHook hook) => _preHitHooks.Add(hook);

    public IReadOnlyList<InterceptionBlocker> InterceptionBlockers => _interceptionBlockers;
    public void AddInterceptionBlocker(InterceptionBlocker hook) => _interceptionBlockers.Add(hook);

    public IReadOnlyList<FullHitRule> FullHitRules => _fullHitRules;
    public void AddFullHitRule(FullHitRule hook) => _fullHitRules.Add(hook);

    public IReadOnlyList<GuaranteedInterceptor> GuaranteedInterceptors => _guaranteedInterceptors;
    public void AddGuaranteedInterceptor(GuaranteedInterceptor hook) => _guaranteedInterceptors.Add(hook);

    public IReadOnlyList<ForcedMoveImmunity> ForcedMoveImmunities => _forcedMoveImmunities;
    public void AddForcedMoveImmunity(ForcedMoveImmunity hook) => _forcedMoveImmunities.Add(hook);

    public IReadOnlyList<AreaRedirect> AreaRedirects => _areaRedirects;
    public void AddAreaRedirect(AreaRedirect hook) => _areaRedirects.Add(hook);

    public IReadOnlyList<AfterMove> AfterMoveHooks => _afterMoveHooks;
    public void AddAfterMoveHook(AfterMove hook) => _afterMoveHooks.Add(hook);

    public IReadOnlyList<AttackBan> AttackBans => _attackBans;

    /// <summary>The shared HP pools of Bound Resurrect (game_system section 10.4).</summary>
    public IReadOnlyList<SharedHp> SharedPools => _sharedPools;

    public SharedHp? SharedPoolOf(int unitId) => _sharedPools.FirstOrDefault(p => p.UnitA == unitId || p.UnitB == unitId);

    /// <summary>
    /// Two units share one HP pool: its maximum is the sum of both maximum HP and it starts at the given value. Every hit on either
    /// reduces the pool, healing either heals it, and at zero both are KO. Each unit's HP shows the pool, capped at its own maximum.
    /// </summary>
    public SharedHp AddSharedPool(int unitA, int unitB, int pool, bool boundStrength)
    {
        var max = GetUnit(unitA).MaxHp + GetUnit(unitB).MaxHp;
        var p = new SharedHp(unitA, unitB, Math.Min(pool, max), max, boundStrength);
        _sharedPools.Add(p);
        SyncPool(p);
        return p;
    }

    /// <summary>The link ends when either unit is KO: the pool goes away and each keeps the HP it shows.</summary>
    public void EndSharedPool(int unitId) => _sharedPools.RemoveAll(p => p.UnitA == unitId || p.UnitB == unitId);

    private void SyncPool(SharedHp pool)
    {
        foreach (var id in new[] { pool.UnitA, pool.UnitB })
            GetUnit(id).Hp = pool.Pool <= 0 ? 0 : Math.Min(pool.Pool, GetUnit(id).MaxHp);
        Emit(new SharedHpChanged(pool.UnitA, pool.UnitB, pool.Pool, pool.Max));
    }
    public void AddAttackBan(AttackBan hook) => _attackBans.Add(hook);

    /// <summary>Called when another unit's skill cleanses a status off a unit (Lingering Malice).</summary>
    public IReadOnlyList<StatusRemovedHook> StatusRemovedHooks => _statusRemovedHooks;
    public void AddStatusRemovedHook(StatusRemovedHook hook) => _statusRemovedHooks.Add(hook);

    public IReadOnlyList<TargetHitHook> TargetHitHooks => _targetHitHooks;
    public void AddTargetHitHook(TargetHitHook hook) => _targetHitHooks.Add(hook);

    public IReadOnlyList<StatusApplicationModifier> StatusApplicationModifiers => _statusApplicationModifiers;
    public void AddStatusApplicationModifier(StatusApplicationModifier hook) => _statusApplicationModifiers.Add(hook);

    public IReadOnlyList<SkillDelayModifier> SkillDelayModifiers => _skillDelayModifiers;
    public void AddSkillDelayModifier(SkillDelayModifier hook) => _skillDelayModifiers.Add(hook);

    public IReadOnlyList<DamageSharing> DamageSharings => _damageSharings;
    public void AddDamageSharing(DamageSharing hook) => _damageSharings.Add(hook);

    public IReadOnlyList<LethalGuard> LethalGuards => _lethalGuards;
    public void AddLethalGuard(LethalGuard hook) => _lethalGuards.Add(hook);

    /// <summary>Rite of Mists: until this battle time, projectile hits from units standing on water deal half damage.</summary>
    public int? MistsUntil { get; set; }

    // Leader hooks (docs/skills.md Leader rulings)
    public IReadOnlyList<MomentumShiftModifier> MomentumShiftModifiers => _momentumShiftModifiers;
    public void AddMomentumShiftModifier(MomentumShiftModifier hook) => _momentumShiftModifiers.Add(hook);

    public IReadOnlyList<MomentumCostModifier> MomentumCostModifiers => _momentumCostModifiers;
    public void AddMomentumCostModifier(MomentumCostModifier hook) => _momentumCostModifiers.Add(hook);

    public IReadOnlyList<RepeatDelayReduction> RepeatDelayReductions => _repeatDelayReductions;
    public void AddRepeatDelayReduction(RepeatDelayReduction hook) => _repeatDelayReductions.Add(hook);

    /// <summary>True the first time a key is seen: passives of several units that install one shared rule use it to install it once.</summary>
    public bool FirstInstall(string key) => _installedKeys.Add(key);

    /// <summary>Registers an aura that keeps statuses in step with positions (Song of Haste). Run by <see cref="RunAuras"/> after every event.</summary>
    public void AddAuraSync(string key, Action<BattleState> sync)
    {
        if (_auraSyncs.All(a => a.Key != key)) _auraSyncs.Add((key, sync));
    }

    /// <summary>Brings every aura up to date with the formation and the living units.</summary>
    public void RunAuras()
    {
        foreach (var (_, sync) in _auraSyncs.ToArray()) sync(this);
    }

    // Warrior hooks (docs/skills.md Warrior rulings)
    public IReadOnlyList<DamageBonus> DamageBonuses => _damageBonuses;
    public void AddDamageBonus(DamageBonus hook) => _damageBonuses.Add(hook);

    public IReadOnlyList<PostHitHook> PostHitHooks => _postHitHooks;
    public void AddPostHitHook(PostHitHook hook) => _postHitHooks.Add(hook);

    public IReadOnlyList<AttackMissHook> AttackMissHooks => _attackMissHooks;
    public void AddAttackMissHook(AttackMissHook hook) => _attackMissHooks.Add(hook);

    public IReadOnlyList<DamageFloor> DamageFloors => _damageFloors;
    public void AddDamageFloor(DamageFloor hook) => _damageFloors.Add(hook);

    public IReadOnlyList<StatusImmunity> StatusImmunities => _statusImmunities;
    public void AddStatusImmunity(StatusImmunity hook) => _statusImmunities.Add(hook);

    public IReadOnlyList<TurnPushImmunity> TurnPushImmunities => _turnPushImmunities;
    public void AddTurnPushImmunity(TurnPushImmunity hook) => _turnPushImmunities.Add(hook);

    public IReadOnlyList<MoveBan> MoveBans => _moveBans;
    public void AddMoveBan(MoveBan hook) => _moveBans.Add(hook);

    public IReadOnlyList<TargetRestriction> TargetRestrictions => _targetRestrictions;
    public void AddTargetRestriction(TargetRestriction hook) => _targetRestrictions.Add(hook);

    public IReadOnlyList<AttackExpander> AttackExpanders => _attackExpanders;
    public void AddAttackExpander(AttackExpander hook) => _attackExpanders.Add(hook);

    /// <summary>Pushes a unit's turn back, unless a passive forbids it (Unstoppable). Returns whether it moved.</summary>
    public bool PushTurnBack(int unitId, int timeUnits)
    {
        if (_turnPushImmunities.Any(h => h(this, unitId))) return false;
        if (Timeline.TurnOf(unitId) is not { } turn) return false;
        Timeline.PushBack(turn, timeUnits);
        return true;
    }

    /// <summary>Changes the damage of hits before shields (Brittle).</summary>
    public void AddDamageModifier(DamageModifier hook) => _damageModifiers.Add(hook);

    /// <summary>Whether the battle is over and who won (section 12.3). Set by <see cref="BattleEnd"/>.</summary>
    public BattleOutcome Outcome { get; private set; }

    public EndReason? EndReason { get; private set; }

    public bool IsOver => Outcome != BattleOutcome.Ongoing;

    /// <summary>How many unit turns have been taken (for the harness's cap). The battle driver counts them.</summary>
    public int TurnsTaken { get; private set; }

    public void CountTurn() => TurnsTaken++;

    internal void SetOutcome(BattleOutcome outcome, EndReason reason)
    {
        Outcome = outcome;
        EndReason = reason;
    }

    /// <summary>The ground effects: burning ground, ice, water and poison ground (section 12.2).</summary>
    public GroundField Ground => _ground;

    /// <summary>The reactions the units have from their skills and passives (section 6.6), in the order they were given.</summary>
    public IReadOnlyList<ReactionDef> Reactions => _reactions;

    public void AddReaction(ReactionDef reaction) => _reactions.Add(reaction);

    /// <summary>How many times a reaction has fired in this battle.</summary>
    public int ReactionUses(ReactionDef reaction) => _reactionUses.GetValueOrDefault(UseKey(reaction));

    internal void CountReactionUse(ReactionDef reaction) => _reactionUses[UseKey(reaction)] = ReactionUses(reaction) + 1;

    private static string UseKey(ReactionDef reaction) => $"{reaction.OwnerUnitId}:{reaction.Id}";

    /// <summary>Pays a skill's Momentum cost for a side and reports it. Throws if the side is not far enough ahead.</summary>
    public void PayMomentum(Side side, int cost)
    {
        var before = _momentum.Value;
        _momentum.Pay(side, cost);
        if (_momentum.Value != before) Emit(new MomentumChanged(MomentumCause.Cost, side, before, _momentum.Value));
    }

    /// <summary>Shifts the meter toward a side (negative: against it), clamped, and reports it if it moved.</summary>
    public void ShiftMomentum(Side side, int amount)
    {
        var before = _momentum.Value;
        _momentum.Shift(side, amount);
        if (_momentum.Value != before) Emit(new MomentumChanged(MomentumCause.Shift, side, before, _momentum.Value));
    }

    public Unit GetUnit(int id) => _units[id];

    /// <summary>Adds a unit with explicit stats at full HP (summons, constructs, tests). Placing it on the board is separate.</summary>
    public Unit AddUnit(string name, Side side, int maxHp, int speed, int power, string? creatureId = null)
    {
        var unit = new Unit(_units.Count, side, name, creatureId, maxHp, speed, power);
        _units.Add(unit);
        return unit;
    }

    /// <summary>
    /// The Power a unit's damage and healing scale with (section 5.2): a summon, construct or totem uses its summoner's
    /// Power, everything else its own. HP and Speed of a summon stay fixed.
    /// </summary>
    public int PowerOf(int unitId)
    {
        var unit = GetUnit(unitId);
        return unit.IsSummoned && unit.SummonerId is { } summoner ? PowerOf(summoner) : unit.EffectivePower;
    }

    /// <summary>Adds a unit with the stats of a roster creature at full HP.</summary>
    public Unit AddUnit(CreatureDef creature, Side side)
    {
        var unit = new Unit(_units.Count, side, creature.Name, creature.Id, creature.Hp, creature.Speed, creature.Power);
        _units.Add(unit);
        return unit;
    }

    /// <summary>
    /// Deals already-rolled, already-scaled damage to a unit: Ward first, then Dampen, then HP (section 5.2). The unit must
    /// not be KO. Returns what happened, and reports it. Momentum, KO and reactions are the caller's next steps.
    /// </summary>
    public DamageResult DealDamage(int? sourceUnitId, int targetId, int amount, DamageSource source = DamageSource.Hit) =>
        DealDamageCore(sourceUnitId, targetId, amount, source, secondary: false);

    /// <summary>
    /// The damage rules. A <paramref name="secondary"/> blow is a share or an echo of damage already being dealt (Spirit Link, Linked
    /// Fates): it meets the target's shields and floors but is not changed again, shared again or echoed again.
    /// </summary>
    private DamageResult DealDamageCore(int? sourceUnitId, int targetId, int amount, DamageSource source, bool secondary)
    {
        var target = GetUnit(targetId);
        if (target.IsKnockedOut) throw new InvalidOperationException($"Unit {targetId} is KO and cannot take damage.");
        if (Statuses.Has(target, StatusIds.StoneShell))
        {
            // Stone Shell: the unit cannot be damaged. Nothing reaches the shield or HP.
            var none = new DamageResult(amount, 0, 0, target.Ward);
            Emit(new DamageDealt(sourceUnitId, targetId, source, none, target.Hp));
            return none;
        }
        if (!secondary && source == DamageSource.Hit && Statuses.Has(target, StatusIds.GlassBody))
        {
            // Glass Body: the next hit on the unit deals double damage, then the status is used up.
            amount *= 2;
            Statuses.Remove(this, targetId, StatusIds.GlassBody, StatusEndCause.Consumed);
        }
        if (!secondary && source == DamageSource.Hit)
            foreach (var modifier in _damageModifiers) amount = modifier(this, sourceUnitId, targetId, amount);
        if (!secondary && _damageSharings.Count > 0)
        {
            // Spirit Link Totem: the damage is split evenly among the allies in the row; the hurt unit keeps the remainder.
            foreach (var sharing in _damageSharings)
            {
                var others = sharing(this, targetId).Where(id => id != targetId && !GetUnit(id).IsKnockedOut).ToList();
                if (others.Count == 0) continue;
                var share = amount / (others.Count + 1);
                if (share > 0)
                {
                    foreach (var other in others) DealDamageCore(sourceUnitId, other, share, source, secondary: true);
                    amount -= share * others.Count;
                }
                break;
            }
        }
        var result = DamageMath.Resolve(amount, target.Ward, target.Dampen, source);
        if (source == DamageSource.Periodic && _damageFloors.Count > 0)
        {
            // Endure: damage from statuses cannot take the unit below its floor.
            var floor = _damageFloors.Max(h => h(this, targetId));
            if (floor > 0 && target.Hp - result.HpLost < floor)
                result = new DamageResult(result.Incoming, result.AbsorbedByWard, Math.Max(0, target.Hp - floor), result.WardLeft);
        }
        if (result.HpLost >= target.Hp && _lethalGuards.Count > 0)
        {
            // Totem of Return: the first ally that would be KO keeps 1 HP and the totem breaks.
            foreach (var guard in _lethalGuards)
                if (guard(this, targetId) is { } totem)
                {
                    result = new DamageResult(result.Incoming, result.AbsorbedByWard, Math.Max(0, target.Hp - 1), result.WardLeft);
                    Summons.Remove(this, totem, RemovalCause.Expired);
                    break;
                }
        }
        target.Ward = result.WardLeft;
        target.Hp = Math.Max(0, target.Hp - result.HpLost);
        if (result.HpLost > 0 && SharedPoolOf(targetId) is { } pool)
        {
            pool.Pool = Math.Max(0, pool.Pool - result.HpLost);   // every hit on either reduces the pool
            SyncPool(pool);
        }
        Emit(new DamageDealt(sourceUnitId, targetId, source, result, target.Hp));
        if (result.AbsorbedByWard > 0 && target.Ward == 0) Statuses.Remove(this, targetId, StatusIds.Ward, StatusEndCause.Broken);

        // Linked Fates / Shared Fate: the linked unit loses half of the HP this unit lost, unreducibly, and that never echoes again.
        if (!secondary && result.HpLost >= 2 && Statuses.Get(target, StatusIds.FateLink) is { } link)
        {
            var other = GetUnit(link.Magnitude);
            if (!other.IsKnockedOut && !other.IsRemoved) DealDamageCore(sourceUnitId, other.Id, result.HpLost / 2, DamageSource.Unreducible, secondary: true);
        }
        return result;
    }

    /// <summary>
    /// Heals a unit, capped at maximum HP (no overheal, section 10.3). Returns the HP actually restored. The unit must not
    /// be KO: only revival brings a KO unit back.
    /// </summary>
    public int Heal(int? sourceUnitId, int targetId, int amount) => HealCore(sourceUnitId, targetId, amount, allowLink: true);

    private int HealCore(int? sourceUnitId, int targetId, int amount, bool allowLink)
    {
        var target = GetUnit(targetId);
        if (target.IsKnockedOut) throw new InvalidOperationException($"Unit {targetId} is KO and cannot be healed, only revived.");
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (allowLink && Statuses.Get(target, StatusIds.Betrayed) is { SourceUnitId: { } sorcerer } && BetrayalRecipient(sorcerer) is { } recipient)
        {
            // Curse of Betrayal: the healing goes to the Sorcerer's unit with the lowest HP instead; it is not redirected again.
            HealCore(sourceUnitId, recipient, amount, allowLink: false);
            return 0;
        }
        if (target.HealingHurts)
        {
            // Healing an undead deals that amount as damage instead (section 11.5).
            if (amount > 0) DealDamage(sourceUnitId, targetId, amount);
            return 0;
        }
        int healed;
        if (SharedPoolOf(targetId) is { } pool)
        {
            var before = pool.Pool;
            pool.Pool = Math.Min(pool.Max, pool.Pool + amount);   // healing either unit heals the pool
            healed = pool.Pool - before;
            SyncPool(pool);
        }
        else
        {
            var after = DamageMath.HealedHp(target.Hp, target.MaxHp, amount);
            healed = after - target.Hp;
            target.Hp = after;
        }
        Emit(new Healed(sourceUnitId, targetId, healed, target.Hp));

        // Life Link: healing on one also heals the other for half (no further link from that second heal).
        if (allowLink && healed > 0 && Statuses.Get(target, StatusIds.LifeLink) is { } link)
        {
            var partner = GetUnit(link.Magnitude);
            if (!partner.IsKnockedOut && !partner.IsRemoved) HealCore(sourceUnitId, partner.Id, Math.Max(1, healed / 2), allowLink: false);
        }
        return healed;
    }

    /// <summary>The Sorcerer's unit that gets healing taken by Curse of Betrayal: the living, healable unit of its side with the lowest HP (ties by the seeded roll); null if none.</summary>
    private int? BetrayalRecipient(int sorcererId)
    {
        var side = GetUnit(sorcererId).Side;
        var candidates = _units.Where(u => u.Side == side && !u.IsKnockedOut && !u.IsRemoved && !u.IsUndead && _formation.Contains(u.Id)).ToList();
        if (candidates.Count == 0) return null;
        var lowest = candidates.Min(u => u.Hp);
        var tied = candidates.Where(u => u.Hp == lowest).ToList();
        return tied.Count == 1 ? tied[0].Id : tied[Random.NextInt(tied.Count)].Id;
    }

    /// <summary>Gives a unit a Ward; a smaller new Ward does not replace a larger one that is left (section 5.2).</summary>
    public void ApplyWard(int targetId, int amount)
    {
        var target = GetUnit(targetId);
        var before = target.Ward;
        target.Ward = DamageMath.CombineWard(before, amount);
        if (target.Ward != before) Emit(new WardChanged(targetId, before, target.Ward));
    }

    /// <summary>Records an event. It is returned, in order, by the next <see cref="DrainEvents"/>.</summary>
    public void Emit(BattleEvent e) => _pendingEvents.Add(e);

    /// <summary>Returns the events recorded since the last call, in order, and clears them.</summary>
    public IReadOnlyList<BattleEvent> DrainEvents()
    {
        var events = _pendingEvents.ToArray();
        _pendingEvents.Clear();
        return events;
    }

    public BattleState Clone() => new(this);
}
