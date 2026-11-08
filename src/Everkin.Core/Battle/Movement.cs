namespace Everkin.Core.Battle;

/// <summary>A place a unit may move to with its Move action: a row and the gap in it (0 is the far left).</summary>
public readonly record struct MoveOption(Row Row, int Index);

public enum ForcedMoveStatus
{
    /// <summary>The unit and everything it bumped moved.</summary>
    Moved,

    /// <summary>The chain was blocked: nothing moved and each unit in it took bump damage.</summary>
    Blocked,

    /// <summary>The unit cannot be moved at all (a KO body, Immobilized): nothing happened.</summary>
    NotMovable,
}

/// <summary>What a forced move did.</summary>
/// <param name="Moves">Every unit that changed place, the moved unit first, then the bumped units in chain order.</param>
/// <param name="BumpDamaged">The units that took bump damage, when the chain was blocked (the caller fires "when hit" reactions and KO checks).</param>
public sealed record ForcedMoveResult(
    ForcedMoveStatus Status, IReadOnlyList<UnitMoved> Moves, IReadOnlyList<int> BumpDamaged)
{
    public static ForcedMoveResult NotMovable { get; } =
        new(ForcedMoveStatus.NotMovable, Array.Empty<UnitMoved>(), Array.Empty<int>());
}

/// <summary>
/// Movement (game_system.md section 9): the Move action, forced repositioning with realistic landing, bumps and bump
/// damage, and swaps. Positions are the half-card coordinates of section 7.3.
/// </summary>
public static class Movement
{
    /// <summary>Bump damage is fixed at 1 and cannot be lowered by anything (section 9.5).</summary>
    public const int BumpDamage = 1;

    /// <summary>The Move action costs as much as a Delay 5 skill, the same as a skipped turn (section 9.1).</summary>
    public const int MoveDelay = Timeline.SkippedTurnDelay;

    /// <summary>An effect caused by this side cannot push, pull or swap the unit (a passive such as Hold Position).</summary>
    public static bool IsImmune(BattleState state, int unitId, Side? causedBy) =>
        causedBy is { } side && state.ForcedMoveImmunities.Any(i => i(state, unitId, side));

    /// <summary>Whether a unit can move or be moved at all: KO bodies and Immobilized units cannot.</summary>
    public static bool IsMovable(Unit unit) => !unit.IsKnockedOut && !unit.IsImmobilized;

    // ------------------------------------------------------------------ Move

    /// <summary>
    /// Every place the Move action offers: any gap in any other row of the unit's own side, except full rows
    /// (section 9.1, 9.2). It cannot reorder a unit within its own row. Empty when the unit cannot move.
    /// </summary>
    public static IReadOnlyList<MoveOption> LegalMoves(BattleState state, int unitId)
    {
        var result = new List<MoveOption>();
        if (!IsMovable(state.GetUnit(unitId))) return result;
        var from = state.Formation.PositionOf(unitId);
        for (var r = 0; r < Formation.RowCount; r++)
        {
            var row = (Row)r;
            if (row == from.Row) continue;
            var count = state.Formation.Count(from.Side, row);
            if (count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++) result.Add(new MoveOption(row, gap));
        }
        return result;
    }

    /// <summary>
    /// The Move action: the unit lands in the gap its player chose. The row recenters. Move never bumps and never swaps.
    /// The caller schedules the next turn with Delay 5 and counts it as a `move` skill for Bleeding.
    /// </summary>
    public static void Move(BattleState state, int unitId, Row row, int gap)
    {
        if (!LegalMoves(state, unitId).Contains(new MoveOption(row, gap)))
            throw new InvalidOperationException($"Unit {unitId} cannot move to {row} gap {gap}.");
        state.Ground.Accrue();
        var from = state.Formation.Remove(unitId);
        state.Formation.Place(unitId, from.Side, row, gap);
        state.Emit(new UnitMoved(unitId, from, state.Formation.PositionOf(unitId), MoveCause.Voluntary));
        foreach (var hook in state.AfterMoveHooks.ToArray()) hook(state, unitId);
    }

    // ------------------------------------------------------------------ forced movement

    /// <summary>
    /// Pushes, pulls and a skill's own movement of its user (section 9.5). The unit keeps its horizontal position and lands
    /// in the destination row where that position falls (a push goes toward the Rear, a pull toward the Front; the
    /// destination is a row of the unit's own side other than its current one):
    /// 100% overlap with a unit bumps it onward in the same direction, and so on down the chain; 50% overlap between two
    /// units inserts it between them; no unit within reach puts it at the end of the row on that side. A chain either
    /// completes or does not: if some unit has nowhere to go, nothing moves and each unit in the chain, the moved unit,
    /// every unit pressed along and the blocker, takes bump damage of 1 (a hit: stealth ends). Bump damage does not move
    /// Momentum. A KO body or an Immobilized unit cannot be moved, so meeting one blocks the chain.
    /// </summary>
    public static ForcedMoveResult ForceMove(BattleState state, int unitId, Row destination, Side? causedBy = null)
    {
        var f = state.Formation;
        var mover = state.GetUnit(unitId);
        if (!IsMovable(mover) || IsImmune(state, unitId, causedBy)) return ForcedMoveResult.NotMovable;
        var start = f.PositionOf(unitId);
        if (start.Row == destination) throw new ArgumentException("The unit is already in that row.", nameof(destination));

        state.Ground.Accrue();
        var direction = destination > start.Row ? 1 : -1;
        var coordinate = f.CenterOf(start);
        var side = start.Side;
        destination = state.Ground.SlideOnIce(side, destination, coordinate, direction); // ice: one row further

        // Plan the chain first; nothing changes until it is known to complete.
        var chain = new List<int> { unitId };                    // moved unit, then every bumped unit
        var landings = new List<(int Unit, Row Row, int Index)>();
        var current = unitId;
        var row = (int)destination;
        List<int>? blockers = null;

        while (true)
        {
            if (row < 0 || row >= Formation.RowCount) { blockers = new List<int>(); break; } // formation boundary
            var list = f.Units(side, (Row)row);
            var n = list.Count;

            // Is there a unit at 100% overlap (the same center)?
            var at = -1;
            for (var i = 0; i < n; i++)
                if (Formation.Center(n, i) == coordinate) { at = i; break; }

            if (at >= 0)
            {
                var occupant = list[at];
                landings.Add((current, (Row)row, at));
                if (!IsMovable(state.GetUnit(occupant)) || IsImmune(state, occupant, causedBy)) { blockers = new List<int> { occupant }; break; }
                chain.Add(occupant);
                current = occupant;
                row += direction;
                continue;
            }

            // 50% overlap between two units, at the edge, or no unit within reach: it goes in at the index where its
            // coordinate falls (insertion into the row, which needs room).
            var index = 0;
            for (var i = 0; i < n; i++)
                if (Formation.Center(n, i) < coordinate) index = i + 1;
            if (n >= Formation.RowCapacity)
            {
                blockers = new List<int>();
                if (index > 0) blockers.Add(list[index - 1]);
                if (index < n) blockers.Add(list[index]);
                break;
            }
            landings.Add((current, (Row)row, index));
            break;
        }

        if (blockers is not null)
        {
            var damaged = chain.Concat(blockers).Distinct().ToList();
            state.Emit(new BumpBlocked(unitId, damaged));
            var hit = new List<int>();
            foreach (var id in damaged)
            {
                var u = state.GetUnit(id);
                if (u.IsKnockedOut) continue; // a body cannot be hurt
                state.DealDamage(null, id, BumpDamage, DamageSource.Unreducible);
                Stealth.Break(state, id, StealthCause.Hit); // bump damage is a hit
                hit.Add(id);
            }
            return new ForcedMoveResult(ForcedMoveStatus.Blocked, Array.Empty<UnitMoved>(), hit);
        }

        // The chain completes: apply it from the far end back, so every hole is free when it is filled.
        var before = chain.ToDictionary(id => id, id => f.PositionOf(id));
        for (var i = landings.Count - 1; i >= 0; i--)
        {
            var (id, landRow, index) = landings[i];
            f.Remove(id);
            f.Place(id, side, landRow, index);
        }

        var moves = chain.Select(id => new UnitMoved(id, before[id], f.PositionOf(id), MoveCause.Forced)).ToList();
        foreach (var m in moves) state.Emit(m);
        foreach (var m in moves)
            foreach (var hook in state.AfterMoveHooks.ToArray()) hook(state, m.UnitId);
        return new ForcedMoveResult(ForcedMoveStatus.Moved, moves, Array.Empty<int>());
    }

    // ------------------------------------------------------------------ swaps

    /// <summary>
    /// Two units of one side exchange places (a skill such as Switch Places). A swap with a KO body or an Immobilized unit
    /// fails (section 9.5): nothing happens and false is returned.
    /// </summary>
    public static bool Swap(BattleState state, int unitA, int unitB, Side? causedBy = null)
    {
        if (unitA == unitB) return false;
        if (!IsMovable(state.GetUnit(unitA)) || !IsMovable(state.GetUnit(unitB))) return false;
        if (IsImmune(state, unitA, causedBy) || IsImmune(state, unitB, causedBy)) return false;
        var a = state.Formation.PositionOf(unitA);
        var b = state.Formation.PositionOf(unitB);
        if (a.Side != b.Side) return false;
        state.Ground.Accrue();
        state.Formation.Swap(unitA, unitB);
        state.Emit(new UnitsSwapped(unitA, unitB));
        return true;
    }
}
