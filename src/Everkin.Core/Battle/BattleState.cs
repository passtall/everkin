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
    }

    /// <summary>Unique inside the battle, assigned in the order units were added.</summary>
    public int Id { get; }

    public Side Side { get; }
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

    public BattleState(ulong seed)
    {
        Random = new BattleRandom(seed);
        _formation = new Formation();
        _timeline = new Timeline();
        _momentum = new Momentum();
        Emit(new BattleStarted(seed));
    }

    private BattleState(BattleState other)
    {
        Random = other.Random.Clone();
        _formation = other._formation.Clone();
        _timeline = other._timeline.Clone();
        _momentum = other._momentum.Clone();
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
