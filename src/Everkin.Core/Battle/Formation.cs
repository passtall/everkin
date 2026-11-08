namespace Everkin.Core.Battle;

/// <summary>The three rows of a side, Front nearest the battle line (game_system.md section 7.1).</summary>
public enum Row
{
    Front = 0,
    Middle = 1,
    Rear = 2,
}

/// <summary>A place on the board: a side, a row and the zero-based order in that row, left to right.</summary>
public readonly record struct Position(Side Side, Row Row, int Index);

/// <summary>
/// Where every unit stands (game_system.md section 7, section 8.4 adjacency). Each side has three rows of at most six
/// units, kept in order from left to right, gapless and centered. The formation knows only positions: whether a unit is
/// KO, stealthed or otherwise ignored is for the rules that use it, since a KO body still blocks its place (section 7.5).
/// </summary>
public sealed class Formation
{
    public const int RowCapacity = 6;
    public const int RowCount = 3;
    public const int SideCapacity = RowCapacity * RowCount;

    private readonly List<int>[,] _rows = new List<int>[2, RowCount];

    public Formation()
    {
        for (var s = 0; s < 2; s++)
            for (var r = 0; r < RowCount; r++)
                _rows[s, r] = new List<int>();
    }

    private Formation(Formation other) : this()
    {
        for (var s = 0; s < 2; s++)
            for (var r = 0; r < RowCount; r++)
                _rows[s, r].AddRange(other._rows[s, r]);
    }

    public Formation Clone() => new(this);

    /// <summary>The unit ids of a row, left to right.</summary>
    public IReadOnlyList<int> Units(Side side, Row row) => _rows[(int)side, (int)row];

    public int Count(Side side, Row row) => _rows[(int)side, (int)row].Count;

    /// <summary>Every unit on a side, Front row first, each row left to right.</summary>
    public IEnumerable<int> Units(Side side)
    {
        for (var r = 0; r < RowCount; r++)
            foreach (var id in _rows[(int)side, r])
                yield return id;
    }

    public int Count(Side side) => Units(side).Count();

    public bool Contains(int unitId) => TryFind(unitId, out _);

    /// <summary>Where a unit stands. Throws if the unit is not on the board.</summary>
    public Position PositionOf(int unitId) =>
        TryFind(unitId, out var p) ? p : throw new ArgumentException($"Unit {unitId} is not on the board.", nameof(unitId));

    public bool TryFind(int unitId, out Position position)
    {
        for (var s = 0; s < 2; s++)
            for (var r = 0; r < RowCount; r++)
            {
                var i = _rows[s, r].IndexOf(unitId);
                if (i >= 0)
                {
                    position = new Position((Side)s, (Row)r, i);
                    return true;
                }
            }
        position = default;
        return false;
    }

    /// <summary>True when the row has room (six per row, eighteen per side, section 7.1).</summary>
    public bool CanPlace(Side side, Row row) =>
        Count(side, row) < RowCapacity && Count(side) < SideCapacity;

    /// <summary>
    /// Puts a unit into a row at the given order (0 is the far left). Without an index it joins at the right end.
    /// Units to its right shift one place. Throws if the unit is already placed, the index is out of range or the row is full.
    /// </summary>
    public void Place(int unitId, Side side, Row row, int? index = null)
    {
        if (Contains(unitId)) throw new InvalidOperationException($"Unit {unitId} is already on the board.");
        if (!CanPlace(side, row)) throw new InvalidOperationException($"No room in {side} {row}.");
        var list = _rows[(int)side, (int)row];
        var at = index ?? list.Count;
        if (at < 0 || at > list.Count) throw new ArgumentOutOfRangeException(nameof(index));
        list.Insert(at, unitId);
    }

    /// <summary>
    /// Two units on the same side exchange their places (row and order). Throws if either is not on the board or they are
    /// on different sides.
    /// </summary>
    public void Swap(int unitA, int unitB)
    {
        var a = PositionOf(unitA);
        var b = PositionOf(unitB);
        if (a.Side != b.Side) throw new InvalidOperationException("Only units of one side can swap places.");
        _rows[(int)a.Side, (int)a.Row][a.Index] = unitB;
        _rows[(int)b.Side, (int)b.Row][b.Index] = unitA;
    }

    /// <summary>Takes a unit off the board. The row closes up and recenters. Returns where it stood.</summary>
    public Position Remove(int unitId)
    {
        var position = PositionOf(unitId);
        _rows[(int)position.Side, (int)position.Row].RemoveAt(position.Index);
        return position;
    }

    /// <summary>
    /// Horizontal center of a card in half-card-width units: 2 × index − (count − 1) (section 7.3).
    /// A three-card row has centers −2, 0, 2; a two-card row −1, 1.
    /// </summary>
    public static int Center(int count, int index) => 2 * index - (count - 1);

    /// <summary>
    /// How much two cards overlap horizontally, in percent: 100, 50 or 0 (section 7.3, card width 2 units):
    /// max(0, 1 − |centerA − centerB| / 2).
    /// </summary>
    public static int OverlapPercent(int centerA, int centerB) => Math.Max(0, 100 - 50 * Math.Abs(centerA - centerB));

    public int CenterOf(Position p) => Center(Count(p.Side, p.Row), p.Index);

    /// <summary>The horizontal overlap of two units on the board, whichever rows and sides they are in.</summary>
    public int UnitOverlapPercent(int unitA, int unitB) =>
        OverlapPercent(CenterOf(PositionOf(unitA)), CenterOf(PositionOf(unitB)));

    /// <summary>
    /// The units adjacent to a unit (section 8.4): its left and right neighbors in the same row, plus the units in the
    /// row directly in front of it and directly behind it, on the same side, with at least 50% horizontal overlap.
    /// Adjacency is physical and uses the nominal rows, so an empty row is not skipped. Order: left, right, in front
    /// (each left to right), behind (each left to right).
    /// </summary>
    public IReadOnlyList<int> Adjacent(int unitId)
    {
        var p = PositionOf(unitId);
        var result = new List<int>();
        var row = _rows[(int)p.Side, (int)p.Row];
        if (p.Index > 0) result.Add(row[p.Index - 1]);
        if (p.Index < row.Count - 1) result.Add(row[p.Index + 1]);

        var center = CenterOf(p);
        foreach (var other in new[] { (int)p.Row - 1, (int)p.Row + 1 })
        {
            if (other < 0 || other >= RowCount) continue;
            var list = _rows[(int)p.Side, other];
            for (var i = 0; i < list.Count; i++)
                if (OverlapPercent(center, Center(list.Count, i)) >= 50)
                    result.Add(list[i]);
        }
        return result;
    }
}
