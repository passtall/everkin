using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class AreaPatternTests
{
    [Fact]
    public void CircularHitsTheTargetAndItsAdjacentUnitsTargetFirst()
    {
        var s = new BattleState(1);
        var left = Place.Unit(s, Side.A, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Front);
        var right = Place.Unit(s, Side.A, Row.Front);
        var behind = Place.Unit(s, Side.A, Row.Middle); // lone Middle card at 0: 100% overlap with the middle Front card
        var far = Place.Unit(s, Side.A, Row.Rear);      // two rows behind: not adjacent
        var hit = AreaPatterns.Circular(s, target);
        Assert.Equal(target, hit[0]);
        Assert.Equal(new[] { left, right, behind }.OrderBy(x => x), hit.Skip(1).OrderBy(x => x));
        Assert.DoesNotContain(far, hit);
    }

    [Fact]
    public void AKoBodyNextToTheTargetIsNotHit()
    {
        // 20.5: Breath hits a target with a KO body next to it: the body is not hit.
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        var body = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(body).Hp = 0;
        Assert.Equal(new[] { target }, AreaPatterns.Circular(s, target));
    }

    [Fact]
    public void AStealthedUnitInsideTheAreaIsStillHit()
    {
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        var hidden = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(hidden).IsStealthed = true;
        Assert.Contains(hidden, AreaPatterns.Circular(s, target));
    }

    [Fact]
    public void RowAndAllSkipKoBodies()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var body = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Middle);
        var enemy = Place.Unit(s, Side.B, Row.Front);
        s.GetUnit(body).Hp = 0;
        Assert.Equal(new[] { a }, AreaPatterns.Row(s, Side.A, Row.Front));
        Assert.Equal(new[] { a, b }, AreaPatterns.AllOf(s, Side.A));
        Assert.Equal(new[] { enemy }, AreaPatterns.AllOf(s, Side.B));
        Assert.Empty(AreaPatterns.Row(s, Side.A, Row.Rear));
    }

    [Fact]
    public void UnitsBehindAreOnlyThoseOfTheNextRowThatHasAUnit()
    {
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        var m1 = Place.Unit(s, Side.A, Row.Middle);  // lone card: 100%
        Place.Unit(s, Side.A, Row.Rear);             // lone card: 100%, but the rule never reaches past the Middle row
        Assert.Equal(new[] { new BehindUnit(m1, 100, 1) }, AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void AnEmptyRowBehindIsPassedOver()
    {
        // 20.5: the row directly behind is empty and the Rear row has a unit at 100% overlap: that unit is the one behind.
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        var rear = Place.Unit(s, Side.A, Row.Rear);
        Assert.Equal(new[] { new BehindUnit(rear, 100, 2) }, AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void ARowWithOnlyKoBodiesCountsAsEmpty()
    {
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        var body = Place.Unit(s, Side.A, Row.Middle);
        var rear = Place.Unit(s, Side.A, Row.Rear);
        s.GetUnit(body).Hp = 0;
        Assert.Equal(new[] { new BehindUnit(rear, 100, 2) }, AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void IfNoUnitOfTheNextRowOverlapsTheTargetNothingIsHitBehind()
    {
        // 20.5: the next row has units but none overlaps the target: nothing is hit behind, and the Rear row is not tried.
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);   // Front centers -2, 0, 2: the target is the left one (-2)
        Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Middle);               // one Middle card at 0: no overlap with -2
        Place.Unit(s, Side.A, Row.Rear);                 // one Rear card at 0: would not overlap either
        Assert.Empty(AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void TheNextRowCanHoldSeveralUnitsBehindAtHalfOverlap()
    {
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);   // center 0
        var m1 = Place.Unit(s, Side.A, Row.Middle);      // centers -1 and 1: both overlap by 50%
        var m2 = Place.Unit(s, Side.A, Row.Middle);
        Assert.Equal(new[] { new BehindUnit(m1, 50, 1), new BehindUnit(m2, 50, 1) }, AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void StealthedUnitsCountAsBehind()
    {
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        var hidden = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(hidden).IsStealthed = true;
        Assert.Equal(new[] { new BehindUnit(hidden, 100, 1) }, AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void UnitsBehindDropsNoOverlapAndKoBodiesButKeepsStealthed()
    {
        var s = new BattleState(1);
        // Front: 3 cards (-2, 0, 2). Target is the left one; Middle: 2 cards (-1, 1).
        var target = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Front);
        var m1 = Place.Unit(s, Side.A, Row.Middle);  // center -1: 50% with -2
        Place.Unit(s, Side.A, Row.Middle);           // center 1: 0% with -2
        s.GetUnit(m1).IsStealthed = true;
        var behind = AreaPatterns.UnitsBehind(s, target);
        Assert.Equal(new[] { new BehindUnit(m1, 50, 1) }, behind);
        s.GetUnit(m1).Hp = 0;
        Assert.Empty(AreaPatterns.UnitsBehind(s, target));
    }

    [Fact]
    public void BehindRuleHitsFullOverlapAutomaticallyAndHalfOverlapHalfTheTime()
    {
        var auto = 0;
        var half = 0;
        for (ulong seed = 1; seed <= 4000; seed++)
        {
            var s = new BattleState(seed);
            var candidates = new[] { new BehindUnit(10, 100, 1), new BehindUnit(11, 50, 1) };
            var hit = AreaPatterns.HitBehind(s, candidates);
            if (hit.Contains(10)) auto++;
            if (hit.Contains(11)) half++;
        }
        Assert.Equal(4000, auto);
        Assert.InRange(half, 1800, 2200);
    }

    [Fact]
    public void AFullOverlapOnlyBehindRuleCostsNoRoll()
    {
        var s = new BattleState(1);
        var before = s.Random.Clone();
        AreaPatterns.HitBehind(s, new[] { new BehindUnit(1, 100, 1) });
        Assert.Equal(before.NextUInt64(), s.Random.NextUInt64());
    }
}
