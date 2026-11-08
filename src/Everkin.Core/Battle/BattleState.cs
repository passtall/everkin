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
        _knownSkills.AddRange(other._knownSkills);
        OriginalSpeed = other.OriginalSpeed;
        OriginalPower = other.OriginalPower;
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
    public int EffectiveSpeed => Speed + _statusInstances.Values.Sum(StatusRules.SpeedModifier);

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
        return unit.IsSummoned && unit.SummonerId is { } summoner ? GetUnit(summoner).Power : unit.Power;
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
    public DamageResult DealDamage(int? sourceUnitId, int targetId, int amount, DamageSource source = DamageSource.Hit)
    {
        var target = GetUnit(targetId);
        if (target.IsKnockedOut) throw new InvalidOperationException($"Unit {targetId} is KO and cannot take damage.");
        var result = DamageMath.Resolve(amount, target.Ward, target.Dampen, source);
        target.Ward = result.WardLeft;
        target.Hp = Math.Max(0, target.Hp - result.HpLost);
        Emit(new DamageDealt(sourceUnitId, targetId, source, result, target.Hp));
        if (result.AbsorbedByWard > 0 && target.Ward == 0) Statuses.Remove(this, targetId, StatusIds.Ward, StatusEndCause.Broken);
        return result;
    }

    /// <summary>
    /// Heals a unit, capped at maximum HP (no overheal, section 10.3). Returns the HP actually restored. The unit must not
    /// be KO: only revival brings a KO unit back.
    /// </summary>
    public int Heal(int? sourceUnitId, int targetId, int amount)
    {
        var target = GetUnit(targetId);
        if (target.IsKnockedOut) throw new InvalidOperationException($"Unit {targetId} is KO and cannot be healed, only revived.");
        if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
        if (target.IsUndead)
        {
            // Healing an undead deals that amount as damage instead (section 11.5).
            if (amount > 0) DealDamage(sourceUnitId, targetId, amount);
            return 0;
        }
        var after = DamageMath.HealedHp(target.Hp, target.MaxHp, amount);
        var healed = after - target.Hp;
        target.Hp = after;
        Emit(new Healed(sourceUnitId, targetId, healed, after));
        return healed;
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
