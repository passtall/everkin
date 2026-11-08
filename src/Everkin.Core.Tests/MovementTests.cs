using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class MoveActionTests
{
    [Fact]
    public void MoveOffersEveryGapOfEveryOtherNonFullRow()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Front);              // a neighbor: reordering inside the row is not offered
        Place.Unit(s, Side.A, Row.Middle);
        Place.Unit(s, Side.A, Row.Middle);
        for (var i = 0; i < 6; i++) Place.Unit(s, Side.A, Row.Rear); // full: not offered
        var options = Movement.LegalMoves(s, u);
        Assert.Equal(new[] { new MoveOption(Row.Middle, 0), new MoveOption(Row.Middle, 1), new MoveOption(Row.Middle, 2) }, options);
    }

    [Fact]
    public void MoveLandsInTheChosenGapAndTheRowRecenters()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        var m1 = Place.Unit(s, Side.A, Row.Middle);
        var m2 = Place.Unit(s, Side.A, Row.Middle);
        s.DrainEvents();
        Movement.Move(s, u, Row.Middle, 1);
        Assert.Equal(new[] { m1, u, m2 }, s.Formation.Units(Side.A, Row.Middle));
        Assert.Empty(s.Formation.Units(Side.A, Row.Front));
        var e = Assert.IsType<UnitMoved>(Assert.Single(s.DrainEvents()));
        Assert.Equal(MoveCause.Voluntary, e.Cause);
        Assert.Equal(new Position(Side.A, Row.Front, 0), e.From);
        Assert.Equal(new Position(Side.A, Row.Middle, 1), e.To);
    }

    [Fact]
    public void MoveNeverBumpsBecauseAFullRowIsNotOffered()
    {
        // 20.5: a unit tries to Move into a full row: that row is not offered.
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        for (var i = 0; i < 6; i++) Place.Unit(s, Side.A, Row.Middle);
        Assert.DoesNotContain(Movement.LegalMoves(s, u), o => o.Row == Row.Middle);
        Assert.Throws<InvalidOperationException>(() => Movement.Move(s, u, Row.Middle, 0));
    }

    [Fact]
    public void ACannotMoveWithinItsOwnRowOrToANonexistentGap()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        Assert.Throws<InvalidOperationException>(() => Movement.Move(s, u, Row.Front, 0));
        Assert.Throws<InvalidOperationException>(() => Movement.Move(s, u, Row.Middle, 3));
    }

    [Fact]
    public void KoAndImmobilizedUnitsCannotMove()
    {
        var s = new BattleState(1);
        var ko = Place.Unit(s, Side.A, Row.Front);
        var stuck = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(ko).Hp = 0;
        s.GetUnit(stuck).IsImmobilized = true;
        Assert.Empty(Movement.LegalMoves(s, ko));
        Assert.Empty(Movement.LegalMoves(s, stuck));
    }

    [Fact]
    public void MoveCostsADelayFiveSkillLikeASkippedTurn() =>
        Assert.Equal(10, Timeline.Wait(Movement.MoveDelay, speed: 6)); // 20.5: Speed 6 uses Move: 5 + (11 - 6)
}

public class ForcedMovementTests
{
    private static int HpOf(BattleState s, int id) => s.GetUnit(id).Hp;

    [Fact]
    public void AGustOnAFrontUnitWithAnEmptyMiddleLandsInMiddleAndBumpsNothing()
    {
        // 20.5: even if the Rear row has a unit behind it.
        var s = new BattleState(1);
        var front = Place.Unit(s, Side.A, Row.Front);
        var rear = Place.Unit(s, Side.A, Row.Rear);
        var r = Movement.ForceMove(s, front, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Moved, r.Status);
        Assert.Equal(new Position(Side.A, Row.Middle, 0), s.Formation.PositionOf(front));
        Assert.Equal(new Position(Side.A, Row.Rear, 0), s.Formation.PositionOf(rear));
        Assert.Single(r.Moves);
        Assert.Equal(40, HpOf(s, rear));
    }

    [Fact]
    public void AUnitDirectlyBehindAt100PercentOverlapIsShovedBackToo()
    {
        // 20.5.
        var s = new BattleState(1);
        var front = Place.Unit(s, Side.A, Row.Front);
        var middle = Place.Unit(s, Side.A, Row.Middle);
        var r = Movement.ForceMove(s, front, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Moved, r.Status);
        Assert.Equal(new Position(Side.A, Row.Middle, 0), s.Formation.PositionOf(front));
        Assert.Equal(new Position(Side.A, Row.Rear, 0), s.Formation.PositionOf(middle));
        Assert.Equal(new[] { front, middle }, r.Moves.Select(m => m.UnitId));
        Assert.All(r.Moves, m => Assert.Equal(MoveCause.Forced, m.Cause));
        Assert.Equal(40, HpOf(s, front)); // everyone had room: no damage
        Assert.Equal(40, HpOf(s, middle));
    }

    [Fact]
    public void AChainBlockedAtTheFormationBoundaryMovesNothingAndHurtsEveryUnitInIt()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Middle);
        var c = Place.Unit(s, Side.A, Row.Rear);
        s.DrainEvents();
        var r = Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Empty(r.Moves);
        Assert.Equal(new[] { a, b, c }, r.BumpDamaged);
        Assert.Equal(new Position(Side.A, Row.Front, 0), s.Formation.PositionOf(a));
        Assert.Equal(new Position(Side.A, Row.Middle, 0), s.Formation.PositionOf(b));
        Assert.Equal(new Position(Side.A, Row.Rear, 0), s.Formation.PositionOf(c));
        Assert.All(new[] { a, b, c }, id => Assert.Equal(39, HpOf(s, id)));
        Assert.Contains(s.DrainEvents(), e => e is BumpBlocked);
    }

    [Fact]
    public void FiftyPercentOverlapAtTheEdgeInsertsAtThatEnd()
    {
        var s = new BattleState(1);
        var left = Place.Unit(s, Side.A, Row.Front);   // centers -1 and 1
        var right = Place.Unit(s, Side.A, Row.Front);
        var mid = Place.Unit(s, Side.A, Row.Middle);   // one card at 0
        var r = Movement.ForceMove(s, left, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Moved, r.Status);
        Assert.Equal(new[] { left, mid }, s.Formation.Units(Side.A, Row.Middle));   // coordinate -1 falls left of the unit at 0
        Assert.Equal(new[] { right }, s.Formation.Units(Side.A, Row.Front));
        Assert.Single(r.Moves);
    }

    [Fact]
    public void FiftyPercentOverlapBetweenTwoUnitsInsertsBetweenThem()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);      // center 0
        var m1 = Place.Unit(s, Side.A, Row.Middle);    // centers -1, 1
        var m2 = Place.Unit(s, Side.A, Row.Middle);
        Movement.ForceMove(s, u, Row.Middle);
        Assert.Equal(new[] { m1, u, m2 }, s.Formation.Units(Side.A, Row.Middle));
    }

    [Fact]
    public void AnInsertionIntoAFullRowIsBlockedAndHurtsTheMovedUnitAndTheTwoItWouldHaveSplit()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);      // center 0: between the 3rd and 4th of six (centers odd)
        var row = Enumerable.Range(0, 6).Select(_ => Place.Unit(s, Side.A, Row.Middle)).ToArray();
        var r = Movement.ForceMove(s, u, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Equal(new[] { u, row[2], row[3] }, r.BumpDamaged);
        Assert.Equal(39, HpOf(s, u));
        Assert.Equal(40, HpOf(s, row[0]));
        Assert.Equal(6, s.Formation.Count(Side.A, Row.Middle));
        Assert.Equal(Row.Front, s.Formation.PositionOf(u).Row);
    }

    [Fact]
    public void HitAndRunIntoAFullRowBehindStrikesButTheMoveBackIsBlocked()
    {
        // 20.5: the move back is blocked, every unit in the chain takes 1 bump damage, the Warrior stays where it was.
        var s = new BattleState(1);
        var warrior = Place.Unit(s, Side.A, Row.Middle);
        Place.Unit(s, Side.A, Row.Middle);             // Middle has 2 cards: centers -1, 1
        var rear = Enumerable.Range(0, 6).Select(_ => Place.Unit(s, Side.A, Row.Rear)).ToArray(); // full, centers -5..5
        var r = Movement.ForceMove(s, warrior, Row.Rear);
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Equal(new[] { warrior, rear[2] }, r.BumpDamaged);   // the unit at -1 is bumped and has nowhere to go
        Assert.Equal(Row.Middle, s.Formation.PositionOf(warrior).Row);
        Assert.Equal(39, HpOf(s, warrior));
        Assert.Equal(39, HpOf(s, rear[2]));
    }

    [Fact]
    public void APullBumpsAUnitInFrontOfItForwardAndIsBlockedAtTheFront()
    {
        // Flush: a unit pulled into the Front row lands on a Front unit at 100% overlap, which has nowhere to go.
        var s = new BattleState(1);
        var front = Place.Unit(s, Side.B, Row.Front);
        var pulled = Place.Unit(s, Side.B, Row.Rear);
        var r = Movement.ForceMove(s, pulled, Row.Front);
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Equal(new[] { pulled, front }, r.BumpDamaged);
    }

    [Fact]
    public void APullIntoAnEmptyFrontRowSimplyMoves()
    {
        var s = new BattleState(1);
        var pulled = Place.Unit(s, Side.B, Row.Rear);
        var r = Movement.ForceMove(s, pulled, Row.Front);
        Assert.Equal(ForcedMoveStatus.Moved, r.Status);
        Assert.Equal(new Position(Side.B, Row.Front, 0), s.Formation.PositionOf(pulled));
    }

    [Fact]
    public void ABumpKeepsRowCountsAndOnlyShiftsTheChain()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Middle);
        Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(0, s.Formation.Count(Side.A, Row.Front));
        Assert.Equal(1, s.Formation.Count(Side.A, Row.Middle));
        Assert.Equal(1, s.Formation.Count(Side.A, Row.Rear));
        Assert.Equal(new[] { a }, s.Formation.Units(Side.A, Row.Middle));
        Assert.Equal(new[] { b }, s.Formation.Units(Side.A, Row.Rear));
    }

    [Fact]
    public void ABumpOnlyHappensOnTheMovedUnitsOwnSide()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var enemy = Place.Unit(s, Side.B, Row.Middle);
        Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(new Position(Side.B, Row.Middle, 0), s.Formation.PositionOf(enemy));
    }

    // ---- bump damage ----

    [Fact]
    public void BumpDamageIgnoresShieldsAndReductionsDoesNotMoveMomentumAndEndsStealth()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Middle);
        var c = Place.Unit(s, Side.A, Row.Rear);
        s.GetUnit(b).Ward = 8;
        s.GetUnit(b).Dampen = true;
        s.GetUnit(c).IsStealthed = true;
        Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(39, HpOf(s, b));           // still 1
        Assert.Equal(8, s.GetUnit(b).Ward);     // the Ward was not touched
        Assert.False(s.GetUnit(c).IsStealthed); // a hit: stealth ends
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void BumpDamageCanKo()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Middle);
        Place.Unit(s, Side.A, Row.Rear);
        s.GetUnit(a).Hp = 1;
        Movement.ForceMove(s, a, Row.Middle);
        Assert.True(s.GetUnit(a).IsKnockedOut);
    }

    // ---- bodies and immobilized units ----

    [Fact]
    public void AKoBodyCannotBeMovedAndAMovedUnitThatLandsOnOneIsBlockedWithoutHurtingTheBody()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var body = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(body).Hp = 0;
        Assert.Equal(ForcedMoveStatus.NotMovable, Movement.ForceMove(s, body, Row.Rear).Status);
        var r = Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Equal(new[] { a }, r.BumpDamaged);
        Assert.Equal(0, HpOf(s, body));
        Assert.Equal(39, HpOf(s, a));
    }

    [Fact]
    public void AnImmobilizedUnitCannotBeMovedAndBlocksAChainButTakesTheDamage()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var stuck = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(stuck).IsImmobilized = true;
        Assert.Equal(ForcedMoveStatus.NotMovable, Movement.ForceMove(s, stuck, Row.Rear).Status);
        var r = Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Equal(new[] { a, stuck }, r.BumpDamaged);
    }

    [Fact]
    public void AMovedUnitCannotBeSentToItsOwnRow()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        Assert.Throws<ArgumentException>(() => Movement.ForceMove(s, a, Row.Front));
    }

    // ---- swaps ----

    [Fact]
    public void SwapExchangesPlaces()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Rear);
        s.DrainEvents();
        Assert.True(Movement.Swap(s, a, b));
        Assert.Equal(new Position(Side.A, Row.Rear, 0), s.Formation.PositionOf(a));
        Assert.Equal(new Position(Side.A, Row.Front, 0), s.Formation.PositionOf(b));
        Assert.Equal(new BattleEvent[] { new UnitsSwapped(a, b) }, s.DrainEvents());
    }

    [Fact]
    public void ASwapWithABodyAnImmobilizedUnitOrTheOtherSideFails()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var body = Place.Unit(s, Side.A, Row.Middle);
        var stuck = Place.Unit(s, Side.A, Row.Middle);
        var enemy = Place.Unit(s, Side.B, Row.Front);
        s.GetUnit(body).Hp = 0;
        s.GetUnit(stuck).IsImmobilized = true;
        Assert.False(Movement.Swap(s, a, body));
        Assert.False(Movement.Swap(s, a, stuck));
        Assert.False(Movement.Swap(s, a, enemy));
        Assert.False(Movement.Swap(s, a, a));
        Assert.Equal(new Position(Side.A, Row.Front, 0), s.Formation.PositionOf(a));
    }

    [Fact]
    public void FormationSwapKeepsRowsAndCounts()
    {
        var f = new Formation();
        f.Place(0, Side.A, Row.Front);
        f.Place(1, Side.A, Row.Front);
        f.Place(2, Side.A, Row.Middle);
        f.Swap(0, 2);
        Assert.Equal(new[] { 2, 1 }, f.Units(Side.A, Row.Front));
        Assert.Equal(new[] { 0 }, f.Units(Side.A, Row.Middle));
        f.Place(3, Side.B, Row.Front);
        Assert.Throws<InvalidOperationException>(() => f.Swap(0, 3));
    }
}
