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
