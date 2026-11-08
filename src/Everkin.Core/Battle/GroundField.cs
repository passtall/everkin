namespace Everkin.Core.Battle;

public enum GroundKind
{
    Burning,
    Ice,
    Water,
    Poison,
}

/// <summary>
/// A spot on the board: a side, a row and a horizontal coordinate in half-card units (game_system.md section 7.3). A spot
/// stays on its coordinate when the row recenters.
/// </summary>
public readonly record struct Spot(Side Side, Row Row, int Coordinate);

/// <summary>One ground effect on one spot.</summary>
public sealed class GroundEffect
{
    internal GroundEffect(int id, GroundKind kind, Spot spot, int placedAt, int endsAt, int markerEntryId, int? sourceUnitId)
    {
        Id = id;
        Kind = kind;
        Spot = spot;
        PlacedAt = placedAt;
        EndsAt = endsAt;
        MarkerEntryId = markerEntryId;
        SourceUnitId = sourceUnitId;
    }

    public int Id { get; }
    public GroundKind Kind { get; }
    public Spot Spot { get; }
    public int PlacedAt { get; internal set; }
    public int EndsAt { get; internal set; }
    public int MarkerEntryId { get; internal set; }

    /// <summary>The unit that placed it. Its effects stay if it is KO (section 12.2).</summary>
    public int? SourceUnitId { get; internal set; }

    /// <summary>Poison ground: how long each unit has stood on it, in time units.</summary>
    internal Dictionary<int, int> StandTimes { get; } = new();

    internal GroundEffect Copy()
    {
        var c = (GroundEffect)MemberwiseClone();
        foreach (var kv in StandTimes) c.StandTimes[kv.Key] = kv.Value;
        return c;
    }
}

public enum GroundEndCause
{
    Expired,
    Extinguished,
    Frozen,
    Steam,
    Exploded,
}

public sealed record GroundEffectPlaced(int EffectId, GroundKind Kind, Spot Spot, int EndsAt, bool Refreshed) : BattleEvent;

public sealed record GroundEffectEnded(int EffectId, GroundKind Kind, Spot Spot, GroundEndCause Cause) : BattleEvent;

/// <summary>Poison ground exploded: each unit on it took damage and the spot became burning ground.</summary>
public sealed record PoisonGroundExploded(Spot Spot, IReadOnlyList<int> Units) : BattleEvent;

/// <summary>
/// The ground effects of a battle (game_system.md section 12.2): burning ground, ice, water and poison ground. An effect sits
/// on a spot and stays on its coordinate when rows recenter; it affects the unit standing there (the unit with the same
/// coordinate, or both units that overlap it by 50% each). It lasts a stated number of time units, its end a marker on the
/// timeline. Airborne units and KO bodies are not affected. Which skills place which effects and where is skill code.
/// </summary>
public sealed class GroundField
{
    public const string TagPrefix = "ground:";

    /// <summary>The reference length (section 12.2); combos start the new effect with it.</summary>
    public const int ReferenceLength = 20;

    /// <summary>Burning ground deals 1 per 2 time units: 50 hundredths of HP per time unit.</summary>
    public const int BurningHundredthsPerTimeUnit = 50;

    /// <summary>Poison ground poisons a unit that has stood there for 5 time units, and again for every further 5.</summary>
    public const int PoisonGroundStandTime = 5;

    /// <summary>The length of the Poison status it applies (the named status table: 50 time units).</summary>
    public const int PoisonStatusLength = 50;

    /// <summary>Exploding poison ground deals 6 to each unit on it. A rule number: it does not scale.</summary>
    public const int ExplosionDamage = 6;

    /// <summary>Steam confuses the unit there for 10 time units.</summary>
    public const int SteamConfusionLength = 10;

    private readonly BattleState _state;
    private readonly List<GroundEffect> _effects = new();
    private readonly Dictionary<int, int> _buildup = new();
    private int _nextId;
    private int _lastAccrual;

    internal GroundField(BattleState state) => _state = state;

    internal GroundField(BattleState state, GroundField other) : this(state)
    {
        _nextId = other._nextId;
        _lastAccrual = other._lastAccrual;
        foreach (var e in other._effects) _effects.Add(e.Copy());
        foreach (var kv in other._buildup) _buildup[kv.Key] = kv.Value;
    }

    public IReadOnlyList<GroundEffect> Effects => _effects;

    public GroundEffect? Find(Spot spot, GroundKind kind) =>
        _effects.FirstOrDefault(e => e.Spot == spot && e.Kind == kind);

    public bool Has(Spot spot, GroundKind kind) => Find(spot, kind) is not null;

    /// <summary>Hundredths of HP of ground damage that have built up on a unit and not landed yet.</summary>
    public int BuildupOf(int unitId) => _buildup.GetValueOrDefault(unitId);

    // ------------------------------------------------------------------ spots

    /// <summary>The spot a unit stands on: its row and horizontal coordinate now.</summary>
    public Spot SpotOf(int unitId)
    {
        var p = _state.Formation.PositionOf(unitId);
        return new Spot(p.Side, p.Row, _state.Formation.CenterOf(p));
    }

    /// <summary>
    /// The units an effect on a spot reaches: the unit whose center is the coordinate (100% overlap), or, when none is,
    /// the units that overlap it by 50% (one on each side at most), which are both affected (section 7.3). Any state of
    /// unit is listed; <see cref="Affected"/> leaves out the ones ground effects do not touch.
    /// </summary>
    public IReadOnlyList<int> UnitsCovering(Spot spot)
    {
        var list = _state.Formation.Units(spot.Side, spot.Row);
        var n = list.Count;
        for (var i = 0; i < n; i++)
            if (Formation.Center(n, i) == spot.Coordinate) return new[] { list[i] };
        var half = new List<int>();
        for (var i = 0; i < n; i++)
            if (Math.Abs(Formation.Center(n, i) - spot.Coordinate) == 1) half.Add(list[i]);
        return half;
    }

    /// <summary>The units a ground effect on this spot affects: alive, and not airborne (an airborne unit ignores every ground effect, section 10.6).</summary>
    public IReadOnlyList<int> Affected(Spot spot) =>
        UnitsCovering(spot).Where(id => !_state.GetUnit(id).IsKnockedOut && !_state.GetUnit(id).IsAirborne).ToArray();

    /// <summary>
    /// Water spots touch when they are side by side in a row (coordinates 2 apart) or overlap across neighboring rows
    /// (rows next to each other, coordinates at most 1 apart) of one side. Returns the water spots touching this one.
    /// </summary>
    public IReadOnlyList<Spot> WaterSpotsTouching(Spot spot) =>
        _effects.Where(e => e.Kind == GroundKind.Water && e.Spot.Side == spot.Side && e.Spot != spot
            && ((e.Spot.Row == spot.Row && Math.Abs(e.Spot.Coordinate - spot.Coordinate) == 2)
                || (Math.Abs((int)e.Spot.Row - (int)spot.Row) == 1 && Math.Abs(e.Spot.Coordinate - spot.Coordinate) <= 1)))
            .Select(e => e.Spot).ToArray();

    // ------------------------------------------------------------------ placing and combos

    /// <summary>
    /// Places an effect on a spot for a number of time units, resolving what the effects do to each other (section 12.2):
    /// placing the same effect again restarts its length; water stops burning ground and fire does not start on water;
    /// frost turns water to ice and the unit there is Frozen; fire meeting ice makes steam (both end, the unit there is
    /// Confused for 10 time units, water takes the spot); fire ends Frozen on its spot; fire placed on a row with poison
    /// ground explodes every poison spot of that row. Returns false when the effect did not start.
    /// </summary>
    public bool Place(GroundKind kind, Spot spot, int duration, int? sourceUnitId = null)
    {
        if (duration < 1) throw new ArgumentOutOfRangeException(nameof(duration));
        Accrue();

        switch (kind)
        {
            case GroundKind.Burning:
                if (Has(spot, GroundKind.Water)) return false;          // fire does not start on water; the water stays
                if (Has(spot, GroundKind.Ice)) { Steam(spot); return false; }
                break;
            case GroundKind.Ice:
                if (Find(spot, GroundKind.Water) is { } water)
                {
                    Remove(water, GroundEndCause.Frozen);
                    Add(GroundKind.Ice, spot, duration, sourceUnitId);
                    foreach (var id in Affected(spot))
                        Statuses.Apply(_state, new StatusApplication(id, StatusIds.Frozen, SourceUnitId: sourceUnitId));
                    return true;
                }
                if (Has(spot, GroundKind.Burning)) { Steam(spot); return false; }
                break;
            case GroundKind.Water:
                if (Find(spot, GroundKind.Burning) is { } fire) Remove(fire, GroundEndCause.Extinguished);
                break;
        }

        Add(kind, spot, duration, sourceUnitId);

        if (kind == GroundKind.Burning)
        {
            foreach (var id in Affected(spot))
                Statuses.Remove(_state, id, StatusIds.Frozen, StatusEndCause.Fire); // the unit keeps its turn
            ExplodePoisonInRow(spot.Side, spot.Row);
        }
        return true;
    }

    private GroundEffect Add(GroundKind kind, Spot spot, int duration, int? sourceUnitId)
    {
        var timeline = _state.Timeline;
        var existing = Find(spot, kind);
        if (existing is not null)
        {
            timeline.Remove(existing.MarkerEntryId);
            existing.PlacedAt = timeline.Now;
            existing.EndsAt = timeline.Now + duration;
            existing.SourceUnitId = sourceUnitId;
            existing.MarkerEntryId = timeline.Add(TimelineKind.EffectEnd, duration, null, TagPrefix + existing.Id).Id;
            _state.Emit(new GroundEffectPlaced(existing.Id, kind, spot, existing.EndsAt, true));
            return existing;
        }

        var id = _nextId++;
        var marker = timeline.Add(TimelineKind.EffectEnd, duration, null, TagPrefix + id);
        var effect = new GroundEffect(id, kind, spot, timeline.Now, timeline.Now + duration, marker.Id, sourceUnitId);
        _effects.Add(effect);
        _state.Emit(new GroundEffectPlaced(id, kind, spot, effect.EndsAt, false));
        return effect;
    }

    /// <summary>Takes an effect off the board with its marker.</summary>
    public void Remove(GroundEffect effect, GroundEndCause cause)
    {
        if (!_effects.Remove(effect)) return;
        _state.Timeline.Remove(effect.MarkerEntryId);
        _state.Emit(new GroundEffectEnded(effect.Id, effect.Kind, effect.Spot, cause));
    }

    private void Steam(Spot spot)
    {
        if (Find(spot, GroundKind.Burning) is { } fire) Remove(fire, GroundEndCause.Steam);
        if (Find(spot, GroundKind.Ice) is { } ice) Remove(ice, GroundEndCause.Steam);
        foreach (var id in Affected(spot))
            Statuses.Apply(_state, new StatusApplication(id, StatusIds.Confused, Duration: SteamConfusionLength));
        Add(GroundKind.Water, spot, ReferenceLength, null); // water stays on the spot
    }

    private void ExplodePoisonInRow(Side side, Row row)
    {
        var spots = _effects.Where(e => e.Kind == GroundKind.Poison && e.Spot.Side == side && e.Spot.Row == row).ToList();
        if (spots.Count == 0) return;
        var hit = new List<(Spot Spot, int[] Units)>();
        foreach (var e in spots)
        {
            var units = Affected(e.Spot).ToArray();
            Remove(e, GroundEndCause.Exploded);
            hit.Add((e.Spot, units));
        }
        foreach (var (spot, units) in hit)
        {
            foreach (var id in units)
                if (!_state.GetUnit(id).IsKnockedOut)
                    _state.DealDamage(null, id, ExplosionDamage);
            _state.Emit(new PoisonGroundExploded(spot, units));
        }
        foreach (var (spot, _) in hit) Place(GroundKind.Burning, spot, ReferenceLength);
    }

    // ------------------------------------------------------------------ time

    /// <summary>
    /// An effect's end marker reached its time: the effect ends. Everything built up to that moment is counted first.
    /// </summary>
    public void HandleExpiry(TimelineEntry entry)
    {
        if (entry.Tag is not { } tag || !tag.StartsWith(TagPrefix, StringComparison.Ordinal)) return;
        if (!int.TryParse(tag.AsSpan(TagPrefix.Length), out var id)) return;
        var effect = _effects.FirstOrDefault(e => e.Id == id && e.MarkerEntryId == entry.Id);
        if (effect is null) return;
        Accrue();
        Remove(effect, GroundEndCause.Expired);
    }

    /// <summary>
    /// Counts what has built up since the last count: a unit on burning ground collects damage for every time unit it stands
    /// there, and a unit on poison ground counts its standing time (Poisoned after 5, restarted by every further 5). Time only
    /// passes between turns, so this is called when a turn begins and, to catch the moment a unit steps on or off a spot,
    /// before anything that changes who stands where (movement, summoning, KO, landing). Calling it again at the same time
    /// does nothing.
    /// </summary>
    public void Accrue()
    {
        var now = _state.Timeline.Now;
        if (now <= _lastAccrual) { _lastAccrual = Math.Max(_lastAccrual, now); return; }
        var last = _lastAccrual;
        _lastAccrual = now;

        foreach (var effect in _effects.ToArray())
        {
            var from = Math.Max(last, effect.PlacedAt);
            var to = Math.Min(now, effect.EndsAt);
            var dt = to - from;
            if (dt <= 0) continue;
            var covered = Affected(effect.Spot);

            if (effect.Kind == GroundKind.Burning)
            {
                foreach (var id in covered)
                    _buildup[id] = _buildup.GetValueOrDefault(id) + dt * BurningHundredthsPerTimeUnit;
            }
            else if (effect.Kind == GroundKind.Poison)
            {
                foreach (var gone in effect.StandTimes.Keys.Where(k => !covered.Contains(k)).ToArray())
                    effect.StandTimes.Remove(gone);
                foreach (var id in covered)
                {
                    var before = effect.StandTimes.GetValueOrDefault(id);
                    var after = before + dt;
                    effect.StandTimes[id] = after;
                    if (after / PoisonGroundStandTime > before / PoisonGroundStandTime)
                        Statuses.Apply(_state, new StatusApplication(id, StatusIds.Poison, Duration: PoisonStatusLength, SourceUnitId: effect.SourceUnitId));
                }
            }
        }
    }

    /// <summary>
    /// Lands what has built up, whenever any unit gets a turn (section 12.2): whole points go to the unit as damage that
    /// shields absorb and Dampen reduces, the fraction carries over. Ground damage is not a hit: it does not end stealth or
    /// move Momentum. Returns the units that took damage, for the KO check (a tick that KOs a unit ends its turn).
    /// </summary>
    public IReadOnlyList<int> Tick()
    {
        Accrue();
        var damaged = new List<int>();
        foreach (var id in _buildup.Keys.ToArray())
        {
            var unit = _state.GetUnit(id);
            if (unit.IsKnockedOut) { _buildup.Remove(id); continue; }
            var whole = _buildup[id] / 100;
            _buildup[id] %= 100;
            if (whole <= 0) continue;
            _state.DealDamage(null, id, whole);
            damaged.Add(id);
        }
        return damaged;
    }

    // ------------------------------------------------------------------ ice

    /// <summary>
    /// A unit pushed or pulled onto an iced spot slides one row further in the same direction (section 12.2); bumps apply to
    /// where it ends. Returns the row it ends in: the destination, or the next row if the spot there is iced and that row exists.
    /// </summary>
    public Row SlideOnIce(Side side, Row destination, int coordinate, int direction)
    {
        if (!Has(new Spot(side, destination, coordinate), GroundKind.Ice)) return destination;
        var next = (int)destination + direction;
        return next is >= 0 and < Formation.RowCount ? (Row)next : destination;
    }
}
