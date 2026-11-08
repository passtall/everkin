using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class TargetingTests
{
    /// <summary>A battle where units are placed one call at a time; each call returns the new unit's id.</summary>
    private sealed class Board
    {
        public BattleState State { get; } = new(1);

        public int Add(Side side, Row row, bool stealthed = false, bool ko = false)
        {
            var id = AddUnit(State, side, row);
            State.GetUnit(id).IsStealthed = stealthed;
            if (ko) State.GetUnit(id).Hp = 0;
            return id;
        }
    }

    // ---- opposing distance, the full table of section 7.4 ----

    [Theory]
    [InlineData(Row.Front, Row.Front, 1)]
    [InlineData(Row.Front, Row.Middle, 2)]
    [InlineData(Row.Front, Row.Rear, 3)]
    [InlineData(Row.Middle, Row.Front, 2)]
    [InlineData(Row.Middle, Row.Middle, 3)]
    [InlineData(Row.Middle, Row.Rear, 4)]
    [InlineData(Row.Rear, Row.Front, 3)]
    [InlineData(Row.Rear, Row.Middle, 4)]
    [InlineData(Row.Rear, Row.Rear, 5)]
    public void DistanceTableWithEveryRowOccupied(Row attackerRow, Row targetRow, int expected)
    {
        var b = new Board();
        // Fill every row on both sides so nothing is skipped.
        var ids = new Dictionary<(Side, Row), int>();
        foreach (var side in new[] { Side.A, Side.B })
            foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
                ids[(side, row)] = b.Add(side, row);
        Assert.Equal(expected, Targeting.OpposingDistance(b.State, ids[(Side.A, attackerRow)], ids[(Side.B, targetRow)]));
    }

    [Fact]
    public void AttackerInFrontEnemyOnlyInRearIsDistanceOne()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        var t = b.Add(Side.B, Row.Rear);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, t));
    }

    [Fact]
    public void AttackerInRearWithEmptyFrontAndMiddleCountsAsRowOne()
    {
        // 20.5: distance to the enemy Front is 1 and to the enemy Rear is 3; it still stands in Rear.
        var b = new Board();
        var a = b.Add(Side.A, Row.Rear);
        var eF = b.Add(Side.B, Row.Front);
        b.Add(Side.B, Row.Middle);
        var eR = b.Add(Side.B, Row.Rear);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, eF));
        Assert.Equal(3, Targeting.OpposingDistance(b.State, a, eR));
        Assert.Equal(Row.Rear, b.State.Formation.PositionOf(a).Row);
    }

    [Fact]
    public void YourOnlyUnitInRearAndEnemiesInMiddleAndRearIsDistanceOne()
    {
        // The example under the table.
        var b = new Board();
        var a = b.Add(Side.A, Row.Rear);
        var eM = b.Add(Side.B, Row.Middle);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, eM));
    }

    [Fact]
    public void AStealthedOnlyEnemyFrontDoesNotCountSoTheMiddleCountsAsRowOne()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, stealthed: true);
        var eM = b.Add(Side.B, Row.Middle);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, eM));
    }

    [Fact]
    public void AKoBodyOnlyEnemyFrontDoesNotCount()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, ko: true);
        var eM = b.Add(Side.B, Row.Middle);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, eM));
    }

    [Fact]
    public void StealthedOwnUnitsStillMakeARowCountOnYourOwnSide()
    {
        var b = new Board();
        b.Add(Side.A, Row.Front, stealthed: true);
        var a = b.Add(Side.A, Row.Middle);
        var e = b.Add(Side.B, Row.Front);
        Assert.Equal(2, Targeting.OpposingDistance(b.State, a, e)); // own Front counts: row 2 + 1 - 1
    }

    [Fact]
    public void AKoBodyOnYourOwnSideDoesNotMakeARowCount()
    {
        var b = new Board();
        b.Add(Side.A, Row.Front, ko: true);
        var a = b.Add(Side.A, Row.Middle);
        var e = b.Add(Side.B, Row.Front);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, e));
    }

    [Fact]
    public void ReviveingAUnitInFrontRestoresTheProtectionWithoutMovingAnyone()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        var body = b.Add(Side.B, Row.Front, ko: true);
        var rear = b.Add(Side.B, Row.Rear);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, rear));
        b.State.GetUnit(body).Hp = 10; // revived
        Assert.Equal(2, Targeting.OpposingDistance(b.State, a, rear));
        Assert.Equal(Row.Rear, b.State.Formation.PositionOf(rear).Row);
    }

    [Fact]
    public void HorizontalPositionIsIgnoredForDistance()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front);
        b.Add(Side.B, Row.Front);
        var far = b.Add(Side.B, Row.Front);
        Assert.Equal(1, Targeting.OpposingDistance(b.State, a, far));
    }

    [Fact]
    public void DistanceBetweenUnitsOfOneSideIsNotDefined()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        var c = b.Add(Side.A, Row.Middle);
        Assert.Throws<ArgumentException>(() => Targeting.OpposingDistance(b.State, a, c));
    }

    // ---- reach ----

    [Fact]
    public void ReachHasAMinimumOfOneAndCanBeUnlimited()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Reach.Distance(0));
        Assert.True(Reach.Unlimited.Allows(99));
        Assert.True(Reach.Distance(2).Allows(2));
        Assert.False(Reach.Distance(2).Allows(3));
    }

    [Fact]
    public void EnemySideReachFollowsTheDistance()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Front);
        var eF = b.Add(Side.B, Row.Front);
        var eM = b.Add(Side.B, Row.Middle);
        var eR = b.Add(Side.B, Row.Rear);
        Assert.True(Targeting.InReach(b.State, a, eF, Reach.Distance(1)));
        Assert.False(Targeting.InReach(b.State, a, eM, Reach.Distance(1)));
        Assert.True(Targeting.InReach(b.State, a, eM, Reach.Distance(2)));
        Assert.False(Targeting.InReach(b.State, a, eR, Reach.Distance(2)));
        Assert.True(Targeting.InReach(b.State, a, eR, Reach.Distance(3)));
    }

    [Fact]
    public void ShotReachesTheFarthestVisibleEnemyAtAnyDistance()
    {
        // 20.5: "any visible unit" is legal at any distance.
        var b = new Board();
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
        {
            b.Add(Side.A, row);
            b.Add(Side.B, row);
        }
        var a = b.State.Formation.Units(Side.A, Row.Rear)[0];
        var far = b.State.Formation.Units(Side.B, Row.Rear)[0];
        Assert.Equal(5, Targeting.OpposingDistance(b.State, a, far));
        Assert.False(Targeting.IsLegalTarget(b.State, a, far, Reach.Distance(4)));
        Assert.True(Targeting.IsLegalTarget(b.State, a, far, Reach.Unlimited));
    }

    [Fact]
    public void OwnSideD1ReachesTheUserAndItsAdjacentUnits()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var right = b.Add(Side.A, Row.Front);
        var far = b.Add(Side.A, Row.Front);
        var behind = b.Add(Side.A, Row.Middle);
        Assert.True(Targeting.InReach(b.State, u, u, Reach.Distance(1)));
        Assert.True(Targeting.InReach(b.State, u, right, Reach.Distance(1)));
        Assert.False(Targeting.InReach(b.State, u, far, Reach.Distance(1)));
        // A lone Middle unit behind a 3-card Front row overlaps only the middle Front card (100%): u is the left card, no overlap.
        Assert.False(Targeting.InReach(b.State, u, behind, Reach.Distance(1)));
    }

    [Fact]
    public void GuardianInFrontReachesAnAllyAtTheFarEndOfItsRearRowWithD2()
    {
        // 20.5: on its own side, D2 reaches any unit.
        var b = new Board();
        var g = b.Add(Side.A, Row.Front);
        for (var i = 0; i < 5; i++) b.Add(Side.A, Row.Rear);
        var last = b.Add(Side.A, Row.Rear);
        Assert.True(Targeting.InReach(b.State, g, last, Reach.Distance(2)));
        Assert.False(Targeting.InReach(b.State, g, last, Reach.Distance(1)));
    }

    // ---- legality ----

    [Fact]
    public void EitherSideTargetsAreLegalByDefault()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var ally = b.Add(Side.A, Row.Front);
        var enemy = b.Add(Side.B, Row.Front);
        Assert.Equal(new[] { u, ally, enemy }, Targeting.LegalTargets(b.State, u, Reach.Distance(1)));
    }

    [Fact]
    public void OwnSideOnlyAndEnemySideOnlyRestrictTheSide()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var ally = b.Add(Side.A, Row.Front);
        var enemy = b.Add(Side.B, Row.Front);
        Assert.Equal(new[] { u, ally }, Targeting.LegalTargets(b.State, u, Reach.Distance(1), new TargetOptions(TargetSide.OwnSideOnly)));
        Assert.Equal(new[] { enemy }, Targeting.LegalTargets(b.State, u, Reach.Distance(1), new TargetOptions(TargetSide.EnemySideOnly)));
    }

    [Fact]
    public void AStealthedEnemyCannotBeTargetedButAStealthedAllyCan()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var ally = b.Add(Side.A, Row.Front, stealthed: true);
        var enemy = b.Add(Side.B, Row.Front, stealthed: true);
        Assert.True(Targeting.IsLegalTarget(b.State, u, ally, Reach.Distance(1)));
        Assert.False(Targeting.IsLegalTarget(b.State, u, enemy, Reach.Unlimited));
    }

    [Fact]
    public void KoBodiesAreOnlyTargetsOfResurrection()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var body = b.Add(Side.A, Row.Front, ko: true);
        var enemyBody = b.Add(Side.B, Row.Front, ko: true);
        Assert.False(Targeting.IsLegalTarget(b.State, u, body, Reach.Unlimited));
        Assert.True(Targeting.IsLegalTarget(b.State, u, body, Reach.Unlimited, new TargetOptions(IncludeKnockedOut: true)));
        // Resurrection skills can target allied or enemy bodies; the enemy Front here is a body, so reach counts it as its own row.
        Assert.True(Targeting.IsLegalTarget(b.State, u, enemyBody, Reach.Unlimited, new TargetOptions(IncludeKnockedOut: true)));
    }

    [Fact]
    public void UnitsNotOnTheBoardAreNeverLegal()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var enemy = b.Add(Side.B, Row.Front);
        b.State.Formation.Remove(enemy);
        Assert.False(Targeting.IsLegalTarget(b.State, u, enemy, Reach.Unlimited));
    }

    // ---- nearest ----

    [Fact]
    public void NearestSkipsStealthedAndKoUnitsAndEmptyRows()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, stealthed: true);
        b.Add(Side.B, Row.Front, ko: true);
        var visible = b.Add(Side.B, Row.Middle);
        b.Add(Side.B, Row.Rear);
        Assert.Equal(new[] { visible }, Targeting.NearestEnemies(b.State, u, Reach.Unlimited));
    }

    [Fact]
    public void NoLegalEnemyMeansNoNearestTarget()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, stealthed: true);
        Assert.Empty(Targeting.NearestEnemies(b.State, u, Reach.Unlimited));
        Assert.Null(Targeting.PickNearest(b.State, u, Reach.Unlimited));
    }

    [Fact]
    public void ASingleNearestTargetCostsNoRoll()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var e = b.Add(Side.B, Row.Front);
        b.Add(Side.B, Row.Rear);
        var before = b.State.Random.Clone();
        Assert.Equal(e, Targeting.PickNearest(b.State, u, Reach.Unlimited));
        Assert.Equal(before.NextUInt64(), b.State.Random.NextUInt64());
    }

    [Fact]
    public void EquallyNearTargetsAreChosenByTheSeededRollWithEqualChance()
    {
        // 20.5: a skill hits the nearest enemy and two are equally near: a seeded roll picks one, each with equal chance.
        var counts = new int[2];
        for (ulong seed = 1; seed <= 4000; seed++)
        {
            var state = new BattleState(seed);
            var u = AddUnit(state, Side.A, Row.Front);
            var e1 = AddUnit(state, Side.B, Row.Front);
            var e2 = AddUnit(state, Side.B, Row.Front);
            var picked = Targeting.PickNearest(state, u, Reach.Unlimited)!.Value;
            counts[picked == e1 ? 0 : 1]++;
            Assert.Contains(picked, new[] { e1, e2 });
        }
        Assert.All(counts, c => Assert.InRange(c, 1800, 2200));
    }

    [Fact]
    public void TheSameSeedPicksTheSameTarget()
    {
        int Pick()
        {
            var state = new BattleState(77);
            var u = AddUnit(state, Side.A, Row.Front);
            AddUnit(state, Side.B, Row.Front);
            AddUnit(state, Side.B, Row.Front);
            AddUnit(state, Side.B, Row.Front);
            return Targeting.PickNearest(state, u, Reach.Unlimited)!.Value;
        }
        Assert.Equal(Pick(), Pick());
    }

    private static int AddUnit(BattleState state, Side side, Row row)
    {
        var def = new Everkin.Core.Content.CreatureDef("t", "t", "brawl", new[] { "warrior" }, "t.png", null, "ruins",
            40, 5, 5, 1, new[] { 0, 0, 1, 1 }, new[] { 0, 0, 1, 1 }, "reviewed", "");
        var unit = state.AddUnit(def, side);
        state.Formation.Place(unit.Id, side, row);
        return unit.Id;
    }
}
