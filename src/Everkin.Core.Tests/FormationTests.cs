using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Tests;

public class FormationTests
{
    private static Formation Build(params (int id, Side side, Row row)[] units)
    {
        var f = new Formation();
        foreach (var (id, side, row) in units) f.Place(id, side, row);
        return f;
    }

    [Theory]
    [InlineData(3, 0, -2)]
    [InlineData(3, 1, 0)]
    [InlineData(3, 2, 2)]
    [InlineData(2, 0, -1)]
    [InlineData(2, 1, 1)]
    [InlineData(1, 0, 0)]
    [InlineData(6, 0, -5)]
    [InlineData(6, 5, 5)]
    public void CenterFollowsTheFormula(int count, int index, int expected) =>
        Assert.Equal(expected, Formation.Center(count, index));

    [Theory]
    [InlineData(0, 0, 100)]
    [InlineData(0, 1, 50)]
    [InlineData(-1, 1, 0)]
    [InlineData(-2, 0, 0)]
    [InlineData(-1, 0, 50)]
    public void OverlapIsOneHalfOrNone(int a, int b, int expected) =>
        Assert.Equal(expected, Formation.OverlapPercent(a, b));

    [Fact]
    public void ThreeCardRowAgainstOneCardRowOverlapsOnlyInTheMiddle()
    {
        // Section 7.3: against a one-card row only the middle card of the three-card row fully overlaps.
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Front), (2, Side.A, Row.Front), (3, Side.B, Row.Front));
        Assert.Equal(0, f.UnitOverlapPercent(0, 3));
        Assert.Equal(100, f.UnitOverlapPercent(1, 3));
        Assert.Equal(0, f.UnitOverlapPercent(2, 3));
    }

    [Fact]
    public void ThreeCardRowAgainstTwoCardRowGivesHalfOverlaps()
    {
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Front), (2, Side.A, Row.Front),
                      (3, Side.B, Row.Front), (4, Side.B, Row.Front));
        Assert.Equal(50, f.UnitOverlapPercent(0, 3));
        Assert.Equal(50, f.UnitOverlapPercent(1, 3));
        Assert.Equal(0, f.UnitOverlapPercent(2, 3));
        Assert.Equal(0, f.UnitOverlapPercent(0, 4));
        Assert.Equal(50, f.UnitOverlapPercent(1, 4));
        Assert.Equal(50, f.UnitOverlapPercent(2, 4));
    }

    [Fact]
    public void RowsHoldSixAndASideEighteen()
    {
        var f = new Formation();
        for (var i = 0; i < 6; i++) f.Place(i, Side.A, Row.Front);
        Assert.False(f.CanPlace(Side.A, Row.Front));
        Assert.True(f.CanPlace(Side.B, Row.Front));
        Assert.Throws<InvalidOperationException>(() => f.Place(99, Side.A, Row.Front));
        for (var i = 6; i < 12; i++) f.Place(i, Side.A, Row.Middle);
        for (var i = 12; i < 18; i++) f.Place(i, Side.A, Row.Rear);
        Assert.Equal(18, f.Count(Side.A));
        Assert.False(f.CanPlace(Side.A, Row.Rear));
    }

    [Fact]
    public void PlaceInsertsAtTheIndexAndShiftsTheRest()
    {
        var f = Build((0, Side.A, Row.Middle), (1, Side.A, Row.Middle));
        f.Place(2, Side.A, Row.Middle, 0);
        f.Place(3, Side.A, Row.Middle, 2);
        Assert.Equal(new[] { 2, 0, 3, 1 }, f.Units(Side.A, Row.Middle));
        Assert.Equal(new Position(Side.A, Row.Middle, 2), f.PositionOf(3));
        Assert.Throws<ArgumentOutOfRangeException>(() => f.Place(4, Side.A, Row.Middle, 9));
    }

    [Fact]
    public void PlacingTwiceOrReadingAnUnknownUnitThrows()
    {
        var f = Build((0, Side.A, Row.Front));
        Assert.Throws<InvalidOperationException>(() => f.Place(0, Side.B, Row.Rear));
        Assert.Throws<ArgumentException>(() => f.PositionOf(7));
        Assert.False(f.Contains(7));
    }

    [Fact]
    public void RemoveClosesTheRowAndRecenters()
    {
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Front), (2, Side.A, Row.Front));
        var was = f.Remove(1);
        Assert.Equal(new Position(Side.A, Row.Front, 1), was);
        Assert.Equal(new[] { 0, 2 }, f.Units(Side.A, Row.Front));
        Assert.Equal(-1, f.CenterOf(f.PositionOf(0)));
        Assert.Equal(1, f.CenterOf(f.PositionOf(2)));
        Assert.False(f.Contains(1));
    }

    [Fact]
    public void AdjacentIncludesLeftAndRightNeighborsInTheSameRow()
    {
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Front), (2, Side.A, Row.Front));
        Assert.Equal(new[] { 1 }, f.Adjacent(0));
        Assert.Equal(new[] { 0, 2 }, f.Adjacent(1));
        Assert.Equal(new[] { 1 }, f.Adjacent(2));
    }

    [Fact]
    public void AdjacentInTheRowsInFrontAndBehindNeedsHalfOverlap()
    {
        // Front: 3 units (centers -2, 0, 2). Middle: 2 units (centers -1, 1): each overlaps two Front units by 50%.
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Front), (2, Side.A, Row.Front),
                      (3, Side.A, Row.Middle), (4, Side.A, Row.Middle));
        Assert.Equal(new[] { 1, 3 }, f.Adjacent(0).OrderBy(x => x));       // neighbor 1; Middle 3 at 50%, Middle 4 at 0%
        Assert.Equal(new[] { 0, 2, 3, 4 }, f.Adjacent(1).OrderBy(x => x)); // both neighbors; both Middle units at 50%
        Assert.Equal(new[] { 0, 1, 4 }, f.Adjacent(3).OrderBy(x => x));    // neighbor 4; Front 0 and 1 at 50%
        Assert.DoesNotContain(2, f.Adjacent(3));                         // 0% overlap
    }

    [Fact]
    public void BreathCheck_FrontUnitWithEmptyMiddleDoesNotTouchTheRearUnit()
    {
        // game_system 20.5: Front unit, Middle empty, Rear unit at 100% overlap: not adjacent (nominal rows).
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Rear));
        Assert.Equal(100, f.UnitOverlapPercent(0, 1));
        Assert.Empty(f.Adjacent(0));
        Assert.Empty(f.Adjacent(1));
    }

    [Fact]
    public void BreathCheck_NeighborOnTheLeftAndAUnitBehindAtHalfOverlapAreBothAdjacent()
    {
        // game_system 20.5: a neighbor on the left and a unit in the row behind at 50% overlap are both adjacent.
        var f = Build((0, Side.A, Row.Front), (1, Side.A, Row.Front), (2, Side.A, Row.Middle));
        // Front centers -1, 1; Middle one card at 0: 50% overlap with each Front unit.
        var adjacent = f.Adjacent(1);
        Assert.Contains(0, adjacent);
        Assert.Contains(2, adjacent);
        Assert.Equal(50, f.UnitOverlapPercent(1, 2));
    }

    [Fact]
    public void AdjacencyNeverCrossesTheBattleLine()
    {
        var f = Build((0, Side.A, Row.Front), (1, Side.B, Row.Front));
        Assert.Equal(100, f.UnitOverlapPercent(0, 1));
        Assert.Empty(f.Adjacent(0));
        Assert.Empty(f.Adjacent(1));
    }

    [Fact]
    public void CloneIsIndependent()
    {
        var f = Build((0, Side.A, Row.Front), (1, Side.B, Row.Rear));
        var copy = f.Clone();
        copy.Remove(0);
        copy.Place(5, Side.A, Row.Middle);
        Assert.True(f.Contains(0));
        Assert.False(f.Contains(5));
        Assert.False(copy.Contains(0));
        Assert.Equal(new[] { 0 }, f.Units(Side.A));
    }

    [Fact]
    public void UnitsListFrontRowFirst()
    {
        var f = Build((0, Side.A, Row.Rear), (1, Side.A, Row.Front), (2, Side.A, Row.Middle), (3, Side.A, Row.Front));
        Assert.Equal(new[] { 1, 3, 2, 0 }, f.Units(Side.A));
    }

    [Fact]
    public void BattleStateKeepsItsFormationThroughClone()
    {
        var creature = new CreatureDef("wolf-ranger", "Ranger Wolf", "ranged", new[] { "hunter" }, "wolf-01.png", null,
            "pine_forest", 50, 5, 5, 6, new[] { 0, 0, 640, 960 }, new[] { 0, 0, 1024, 1536 }, "reviewed", "");
        var state = new BattleState(1);
        var u = state.AddUnit(creature, Side.A);
        state.Formation.Place(u.Id, Side.A, Row.Front);
        var copy = state.Clone();
        copy.Formation.Remove(u.Id);
        Assert.True(state.Formation.Contains(u.Id));
        Assert.False(copy.Formation.Contains(u.Id));
    }
}
