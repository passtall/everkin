namespace Everkin.Core.Battle;

/// <summary>What a summoning skill creates (game_system.md sections 11.1, 11.2).</summary>
/// <param name="Side">The side the summon fights on (the summoner's, normally).</param>
/// <param name="Kind">Summon, Temporary or Construct.</param>
/// <param name="MaxHp">Fixed HP stated in the skill (the Clay Golem's 25).</param>
/// <param name="Speed">Fixed Speed stated in the skill.</param>
/// <param name="SummonerId">The summoner: its Power scales the summon's damage and healing.</param>
/// <param name="TakesTurns">Golems and blades take turns of their own; traps, turrets, totems and barricades do not.</param>
/// <param name="Lifetime">Time units until it disappears, or null: it lasts until it is KO'd or the battle ends.</param>
/// <param name="DisappearsWithSummoner">Its skill says it vanishes when the summoner is KO'd.</param>
public sealed record SummonSpec(
    Side Side, string Name, UnitKind Kind, int MaxHp, int Speed, int SummonerId, bool TakesTurns,
    int? Lifetime = null, bool DisappearsWithSummoner = false, string? DefinitionId = null);

/// <summary>
/// Summoning (game_system.md section 11). A summon is a unit that appears mid-battle in a free slot, counts toward row
/// capacity (6 per row, 18 per side), is targetable and intercepts like any unit, and is removed at once when KO'd.
/// </summary>
public static class Summons
{
    /// <summary>The tag on the timeline marker that ends a temporary unit.</summary>
    public const string ExpiryTag = "summon_expiry";

    /// <summary>
    /// Every place a summon can appear: any gap of the allowed rows that still has room. Empty when the side is at its cap
    /// of 18 or has no free slot in those rows, in which case the summoning skill is unavailable (the UI says why).
    /// </summary>
    public static IReadOnlyList<MoveOption> PlacementOptions(BattleState state, Side side, IEnumerable<Row> allowedRows)
    {
        var result = new List<MoveOption>();
        if (state.Formation.Count(side) >= Formation.SideCapacity) return result;
        foreach (var row in allowedRows.Distinct().OrderBy(r => (int)r))
        {
            var count = state.Formation.Count(side, row);
            if (count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++) result.Add(new MoveOption(row, gap));
        }
        return result;
    }

    public static bool CanSummon(BattleState state, Side side, IEnumerable<Row> allowedRows) =>
        PlacementOptions(state, side, allowedRows).Count > 0;

    /// <summary>
    /// The free gaps next to the user, in its own row: the default placement of a summon or construct whose skill states no
    /// range (section 11.2). Empty when the row is full.
    /// </summary>
    public static IReadOnlyList<MoveOption> GapsNextToUser(BattleState state, int userId)
    {
        var p = state.Formation.PositionOf(userId);
        if (!state.Formation.CanPlace(p.Side, p.Row)) return Array.Empty<MoveOption>();
        return new[] { new MoveOption(p.Row, p.Index), new MoveOption(p.Row, p.Index + 1) };
    }

    /// <summary>
    /// Puts a summon on the board in the chosen gap. Its HP and Speed are the skill's fixed numbers. A turn-taking summon
    /// first acts 11 − Speed time units after it appears; a construct takes no turns. A lifetime puts an end marker on the
    /// timeline. Throws if the place is not legal (see <see cref="PlacementOptions"/>).
    /// </summary>
    public static int Summon(BattleState state, SummonSpec spec, Row row, int gap)
    {
        if (spec.Kind == UnitKind.Creature) throw new ArgumentException("A summon is not a party creature.", nameof(spec));
        if (!PlacementOptions(state, spec.Side, new[] { row }).Contains(new MoveOption(row, gap)))
            throw new InvalidOperationException($"No room for a summon in {spec.Side} {row} at gap {gap}.");

        state.Ground.Accrue();
        var power = state.GetUnit(spec.SummonerId).Power; // damage and healing use the summoner's Power through BattleState.PowerOf
        var unit = state.AddUnit(spec.Name, spec.Side, spec.MaxHp, spec.Speed, power, spec.DefinitionId);
        unit.Kind = spec.Kind;
        unit.SummonerId = spec.SummonerId;
        unit.DisappearsWithSummoner = spec.DisappearsWithSummoner;
        state.Formation.Place(unit.Id, spec.Side, row, gap);

        if (spec.TakesTurns) state.Timeline.Add(TimelineKind.UnitTurn, Timeline.InitialWait(spec.Speed), unit.Id);
        if (spec.Lifetime is { } lifetime) state.Timeline.Add(TimelineKind.EffectEnd, lifetime, unit.Id, ExpiryTag);

        state.Emit(new UnitSummoned(unit.Id, spec.SummonerId, spec.Kind, state.Formation.PositionOf(unit.Id)));
        return unit.Id;
    }

    /// <summary>A temporary unit's time ran out: it disappears like a KO'd summon but is not a KO.</summary>
    public static void HandleExpiry(BattleState state, TimelineEntry entry)
    {
        if (entry.Kind != TimelineKind.EffectEnd || entry.Tag != ExpiryTag || entry.OwnerUnitId is not { } id) return;
        var unit = state.GetUnit(id);
        if (unit.IsRemoved || !state.Formation.Contains(id)) return;
        Remove(state, id, RemovalCause.Expired);
    }

    /// <summary>
    /// Takes a summon off the board for good: its slot is free, its turn, events and markers are gone and it cannot be
    /// revived. A KO'd summon goes at once; an expired or vanishing one is no KO.
    /// </summary>
    public static void Remove(BattleState state, int unitId, RemovalCause cause)
    {
        var unit = state.GetUnit(unitId);
        if (unit.IsRemoved || !state.Formation.Contains(unitId)) return;
        state.Ground.Accrue();
        Statuses.ClearAll(state, unitId);
        unit.ClearStatuses();
        state.Timeline.RemoveOwnedBy(unitId, includeTurn: true);
        foreach (var marker in state.Timeline.Upcoming().Where(e => e.OwnerUnitId == unitId).ToArray())
            state.Timeline.Remove(marker.Id);
        var from = state.Formation.Remove(unitId);
        unit.IsRemoved = true;
        unit.KoProcessed = true;
        unit.Hp = 0;
        state.Emit(new UnitRemoved(unitId, cause, from));
    }
}

/// <summary>
/// Undead (game_system.md section 11.5): a necromancer raises a KO body onto its own side; healing an undead hurts it; a
/// resurrection restores the original creature to its owner's side.
/// </summary>
public static class Necromancy
{
    /// <summary>
    /// Where a raised body can go: any gap in a row of the necromancer's side with room, counted after the body has left
    /// its own slot (it may be on the same side). Empty when there is no free slot: the raise cannot be used.
    /// </summary>
    public static IReadOnlyList<MoveOption> RaisePlacements(BattleState state, int necromancerId, int bodyId)
    {
        var body = state.GetUnit(bodyId);
        var necroSide = state.Formation.PositionOf(necromancerId).Side;
        if (!body.IsKnockedOut || body.IsRemoved || !state.Formation.Contains(bodyId)) return Array.Empty<MoveOption>();
        var formation = state.Formation.Clone();
        formation.Remove(bodyId);
        var result = new List<MoveOption>();
        if (formation.Count(necroSide) >= Formation.SideCapacity) return result;
        for (var r = 0; r < Formation.RowCount; r++)
        {
            var count = formation.Count(necroSide, (Row)r);
            if (count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++) result.Add(new MoveOption((Row)r, gap));
        }
        return result;
    }

    /// <summary>
    /// Raises a KO body, from either side: the card leaves its slot (freeing it), moves to the chosen free slot on the
    /// necromancer's side and becomes an undead with the HP (and, if given, Power) the skill sets. It keeps its creature's
    /// Speed and its own Attack Type attack, and takes turns: it first acts 11 − Speed time units after it appears.
    /// </summary>
    public static void Raise(BattleState state, int necromancerId, int bodyId, Row row, int gap, int hp, int? power = null)
    {
        if (!RaisePlacements(state, necromancerId, bodyId).Contains(new MoveOption(row, gap)))
            throw new InvalidOperationException($"Body {bodyId} cannot be raised in {row} at gap {gap}.");
        var body = state.GetUnit(bodyId);
        var necroSide = state.Formation.PositionOf(necromancerId).Side;
        state.Ground.Accrue();

        body.OriginalMaxHp ??= body.MaxHp;
        body.OriginalSpeed ??= body.Speed;
        body.OriginalPower ??= body.Power;
        body.OriginalRow ??= state.Formation.PositionOf(bodyId).Row;

        var from = state.Formation.Remove(bodyId);
        body.Side = necroSide;
        state.Formation.Place(bodyId, necroSide, row, gap);
        body.IsUndead = true;
        body.SummonerId = necromancerId;
        body.MaxHp = Math.Max(1, hp);
        body.Hp = body.MaxHp;
        if (power is { } p) body.Power = p;
        body.KoProcessed = false;
        state.Timeline.Add(TimelineKind.UnitTurn, Timeline.InitialWait(body.EffectiveSpeed), bodyId);
        state.Emit(new UnitRaised(bodyId, necromancerId, from, state.Formation.PositionOf(bodyId), body.Hp));
    }

    /// <summary>
    /// The place a resurrection would restore an undead to: a free slot on its owner's side, in the row nearest its old
    /// row (rows equally near: the battle's seeded roll picks one), at the end of that row. Null when the owner's side has
    /// no free slot: the undead cannot be targeted by the resurrection.
    /// </summary>
    public static MoveOption? RestorePlacement(BattleState state, int undeadId, bool drawTie = false)
    {
        var unit = state.GetUnit(undeadId);
        if (!unit.IsUndead || unit.OriginalRow is not { } old) return null;
        var owner = unit.OriginalSide;
        if (state.Formation.Count(owner) >= Formation.SideCapacity) return null;
        var candidates = Enumerable.Range(0, Formation.RowCount).Where(r => state.Formation.Count(owner, (Row)r) < Formation.RowCapacity).ToList();
        if (candidates.Count == 0) return null;
        var nearest = candidates.Min(r => Math.Abs(r - (int)old));
        var tied = candidates.Where(r => Math.Abs(r - (int)old) == nearest).ToList();
        var chosen = tied.Count == 1 || !drawTie ? tied[0] : tied[state.Random.NextInt(tied.Count)];
        return new MoveOption((Row)chosen, state.Formation.Count(owner, (Row)chosen));
    }

    public static bool CanRestore(BattleState state, int undeadId) => RestorePlacement(state, undeadId) is not null;

    /// <summary>
    /// A resurrection on an undead, alive or KO'd, restores the original creature: it returns to its owner's side, into
    /// the free slot nearest its old row, with its own stats and the revival HP the skill states, and is scheduled as a
    /// new action (Wait(delay, Speed)). Throws if there is no free slot.
    /// </summary>
    public static void Restore(BattleState state, int undeadId, int hp, int delay)
    {
        var unit = state.GetUnit(undeadId);
        var place = RestorePlacement(state, undeadId, drawTie: true)
            ?? throw new InvalidOperationException($"Undead {undeadId} cannot be restored: no free slot on its owner's side.");

        state.Ground.Accrue();
        Statuses.ClearAll(state, undeadId);
        unit.ClearStatuses();
        state.Timeline.RemoveOwnedBy(undeadId, includeTurn: true);
        var from = state.Formation.Remove(undeadId);
        unit.Side = unit.OriginalSide;
        state.Formation.Place(undeadId, unit.OriginalSide, place.Row, place.Index);

        unit.IsUndead = false;
        unit.SummonerId = null;
        unit.MaxHp = unit.OriginalMaxHp ?? unit.MaxHp;
        unit.Speed = unit.OriginalSpeed ?? unit.Speed;
        unit.Power = unit.OriginalPower ?? unit.Power;
        unit.Hp = Math.Clamp(hp, 1, unit.MaxHp);
        unit.KoProcessed = false;
        state.Timeline.ScheduleTurn(undeadId, delay, unit.EffectiveSpeed);
        state.Emit(new UndeadRestored(undeadId, from, state.Formation.PositionOf(undeadId), unit.Hp));
    }
}
