using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class GroundTests
{
    private static BattleState Duel(out int a, out int b)
    {
        var s = new BattleState(1);
        a = Place.Unit(s, Side.A, Row.Front);   // center 0
        b = Place.Unit(s, Side.B, Row.Front);
        s.DrainEvents();
        return s;
    }

    private static void AdvanceTo(BattleState s, int time)
    {
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            Statuses.HandleExpiry(s, e);
        }
    }

    private static Spot Front0(Side side = Side.A) => new(side, Row.Front, 0);

    // ---- spots and coverage ----

    [Fact]
    public void AnEffectReachesTheUnitWhoseCenterIsTheCoordinate()
    {
        var s = new BattleState(1);
        var l = Place.Unit(s, Side.A, Row.Front);
        var m = Place.Unit(s, Side.A, Row.Front);
        var r = Place.Unit(s, Side.A, Row.Front); // centers -2, 0, 2
        Assert.Equal(new[] { m }, s.Ground.UnitsCovering(new Spot(Side.A, Row.Front, 0)));
        Assert.Equal(new[] { r }, s.Ground.UnitsCovering(new Spot(Side.A, Row.Front, 2)));
        Assert.Equal(new Spot(Side.A, Row.Front, -2), s.Ground.SpotOf(l));
    }

    [Fact]
    public void AnEffectOnAHalfOverlapReachesBothUnits()
    {
        // 7.3: if two units overlap the coordinate at 50% each, the effect affects both.
        var s = new BattleState(1);
        var l = Place.Unit(s, Side.A, Row.Front);
        var r = Place.Unit(s, Side.A, Row.Front); // centers -1 and 1
        Assert.Equal(new[] { l, r }, s.Ground.UnitsCovering(new Spot(Side.A, Row.Front, 0)));
        Assert.Empty(s.Ground.UnitsCovering(new Spot(Side.A, Row.Front, 5)));
    }

    [Fact]
    public void AnEffectStaysOnItsCoordinateWhenTheRowRecenters()
    {
        var s = new BattleState(1);
        var l = Place.Unit(s, Side.A, Row.Front);
        var r = Place.Unit(s, Side.A, Row.Front);   // centers -1, 1
        s.Ground.Place(GroundKind.Burning, new Spot(Side.A, Row.Front, -1), 20);
        Assert.Equal(new[] { l }, s.Ground.Affected(new Spot(Side.A, Row.Front, -1)));
        s.Formation.Remove(l);                       // the row recenters: r is now at 0
        Assert.Equal(0, s.Ground.SpotOf(r).Coordinate);
        Assert.Equal(new Spot(Side.A, Row.Front, -1), s.Ground.Effects[0].Spot);   // the effect did not move
        Assert.Equal(new[] { r }, s.Ground.UnitsCovering(new Spot(Side.A, Row.Front, -1)));  // r now overlaps it by 50%
    }

    // ---- length ----

    [Fact]
    public void AnEffectHasAnEndMarkerAndEndsWhenItArrives()
    {
        var s = Duel(out _, out _);
        s.Ground.Place(GroundKind.Water, Front0(), 20);
        var effect = s.Ground.Effects.Single();
        Assert.Equal(20, effect.EndsAt);
        Assert.Equal(TimelineKind.EffectEnd, s.Timeline.Find(effect.MarkerEntryId)!.Kind);
        s.DrainEvents();
        AdvanceTo(s, 25);
        Assert.Empty(s.Ground.Effects);
        Assert.Contains(s.DrainEvents(), e => e is GroundEffectEnded { Cause: GroundEndCause.Expired });
    }

    [Fact]
    public void PlacingTheSameEffectAgainRestartsItsLengthFromNow()
    {
        var s = Duel(out _, out _);
        s.Ground.Place(GroundKind.Water, Front0(), 20);
        AdvanceTo(s, 12);
        s.Ground.Place(GroundKind.Water, Front0(), 20);
        Assert.Single(s.Ground.Effects);
        Assert.Equal(32, s.Ground.Effects[0].EndsAt);
        Assert.Single(s.Timeline.Upcoming().Where(e => e.Kind == TimelineKind.EffectEnd));
    }

    [Fact]
    public void AnEffectStaysWhenItsCasterIsKo()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Poison, new Spot(Side.B, Row.Front, 0), 20, sourceUnitId: a);
        s.GetUnit(a).Hp = 0;
        Knockout.Resolve(s, a);
        Assert.Single(s.Ground.Effects);
    }

    // ---- burning ground ----

    [Fact]
    public void BurningGroundHurtsOnePerTwoTimeUnitsLandingWheneverAUnitGetsATurn()
    {
        // 12.2: a unit standing in fire for 20 time units is hurt for each of those units: 10 over 20.
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        AdvanceTo(s, 7);
        Assert.Equal(new[] { a }, s.Ground.Tick());   // 350 hundredths: 3 land, 50 carried
        Assert.Equal(37, s.GetUnit(a).Hp);
        Assert.Equal(50, s.Ground.BuildupOf(a));
        AdvanceTo(s, 25);                              // the effect ended at 20
        s.Ground.Tick();
        Assert.Equal(30, s.GetUnit(a).Hp);             // 10 in total
        Assert.Empty(s.Ground.Tick());
    }

    [Fact]
    public void AUnitPushedOffMidTurnStopsAccumulatingThenAndOnStartsThen()
    {
        var s = Duel(out var a, out _);
        var other = Place.Unit(s, Side.A, Row.Middle);      // center 0 in Middle
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        AdvanceTo(s, 10);
        Movement.ForceMove(s, a, Row.Middle);               // a bumps `other` onward: a is in Middle now, off the fire
        Assert.NotEqual(Row.Front, s.Formation.PositionOf(a).Row);
        AdvanceTo(s, 25);
        s.Ground.Tick();
        Assert.Equal(35, s.GetUnit(a).Hp);                  // fire for 10 time units only: 5 damage
        Assert.Equal(40, s.GetUnit(other).Hp);
    }

    [Fact]
    public void AUnitThatStepsOntoFireStartsAccumulatingAtThatMoment()
    {
        var s = Duel(out _, out _);
        var u = Place.Unit(s, Side.A, Row.Middle);          // alone in Middle at center 0
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        AdvanceTo(s, 10);
        Movement.ForceMove(s, u, Row.Front);                // pulled onto the burning spot; the Front unit there is bumped forward
        AdvanceTo(s, 25);
        s.Ground.Tick();
        Assert.True(s.GetUnit(u).Hp <= 40);
    }

    [Fact]
    public void AWardAbsorbsBurningGroundDamageAndItIsNotAHit()
    {
        // 20.5: burning ground damages a unit with a Ward of 8: the Ward absorbs the damage.
        var s = Duel(out var a, out _);
        s.GetUnit(a).Ward = 8;
        s.GetUnit(a).IsStealthed = true;
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        AdvanceTo(s, 8);
        s.Ground.Tick();                                    // 4 points
        Assert.Equal(4, s.GetUnit(a).Ward);
        Assert.Equal(40, s.GetUnit(a).Hp);
        Assert.True(s.GetUnit(a).IsStealthed);              // not a hit
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void AnAirborneUnitIgnoresGroundEffectsIncludingOnesAlreadyOnItsSpot()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Airborne, Duration: 10));
        AdvanceTo(s, 10);                                   // airborne ends at 10; the 10 airborne time units do not count
        s.Ground.Tick();
        Assert.Equal(40, s.GetUnit(a).Hp);
        AdvanceTo(s, 20);
        s.Ground.Tick();
        Assert.Equal(35, s.GetUnit(a).Hp);                  // only the time on the ground counts
    }

    [Fact]
    public void KoBodiesTakeNoGroundDamage()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        AdvanceTo(s, 4);
        s.GetUnit(a).Hp = 0;
        Knockout.Resolve(s, a);
        AdvanceTo(s, 20);
        Assert.Empty(s.Ground.Tick());
        Assert.Equal(0, s.Ground.BuildupOf(a));
        Assert.Equal(0, s.GetUnit(a).Hp);
    }

    // ---- combos ----

    [Fact]
    public void WaterStopsBurningGroundAndFireDoesNotStartOnWater()
    {
        var s = Duel(out _, out _);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        s.Ground.Place(GroundKind.Water, Front0(), 20);
        Assert.False(s.Ground.Has(Front0(), GroundKind.Burning));
        Assert.True(s.Ground.Has(Front0(), GroundKind.Water));

        Assert.False(s.Ground.Place(GroundKind.Burning, Front0(), 20));   // fire set on water does not start
        Assert.True(s.Ground.Has(Front0(), GroundKind.Water));            // and the water stays
        Assert.False(s.Ground.Has(Front0(), GroundKind.Burning));
    }

    [Fact]
    public void FrostOnWaterMakesIceAndFreezesTheUnitThere()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Water, Front0(), 20);
        Assert.True(s.Ground.Place(GroundKind.Ice, Front0(), 20));
        Assert.False(s.Ground.Has(Front0(), GroundKind.Water));
        Assert.True(s.Ground.Has(Front0(), GroundKind.Ice));
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Frozen));
    }

    [Fact]
    public void IceOnADrySpotDoesNotFreezeAnyone()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Ice, Front0(), 20);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Frozen));
    }

    [Fact]
    public void FireMeetingIceMakesSteamBothEndTheUnitIsConfusedAndWaterStays()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Ice, Front0(), 20);
        s.DrainEvents();
        Assert.False(s.Ground.Place(GroundKind.Burning, Front0(), 20));
        Assert.False(s.Ground.Has(Front0(), GroundKind.Ice));
        Assert.False(s.Ground.Has(Front0(), GroundKind.Burning));
        Assert.True(s.Ground.Has(Front0(), GroundKind.Water));
        var confused = Statuses.Get(s.GetUnit(a), StatusIds.Confused)!;
        Assert.Equal(10, confused.EndsAt);
        Assert.Equal(20, s.Ground.Find(Front0(), GroundKind.Water)!.EndsAt); // the water starts its full length
    }

    [Fact]
    public void IceSetOnBurningGroundMakesSteamToo()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        s.Ground.Place(GroundKind.Ice, Front0(), 20);
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Confused));
        Assert.True(s.Ground.Has(Front0(), GroundKind.Water));
        Assert.False(s.Ground.Has(Front0(), GroundKind.Ice));
    }

    [Fact]
    public void FireOnAFrozenUnitsSpotEndsFrozenEarlyAndTheUnitKeepsItsTurn()
    {
        // 20.5.
        var s = Duel(out var a, out _);
        s.Timeline.Add(TimelineKind.UnitTurn, 8, a);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Frozen));
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Frozen));
        Assert.NotNull(s.Timeline.TurnOf(a));
        Assert.False(Statuses.OnTurnStart(s, a).FrozenTurnSkipped);
    }

    // ---- poison ground ----

    [Fact]
    public void ABurningSpotBesideIsNotAffectedByAPoisonSpotOnAnotherRow()
    {
        var s = Duel(out _, out _);
        Place.Unit(s, Side.A, Row.Middle);
        s.Ground.Place(GroundKind.Poison, new Spot(Side.A, Row.Middle, 0), 20);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        Assert.True(s.Ground.Has(new Spot(Side.A, Row.Middle, 0), GroundKind.Poison)); // another row: nothing explodes
    }

    [Fact]
    public void AUnitThatHasStoodOnPoisonGroundForFiveTimeUnitsIsPoisonedAndEveryFurtherFiveRestartsIt()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Poison, Front0(), 30);
        AdvanceTo(s, 4);
        s.Ground.Accrue();
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Poison));
        AdvanceTo(s, 5);
        s.Ground.Accrue();
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Poison));
        Assert.Equal(55, Statuses.Get(s.GetUnit(a), StatusIds.Poison)!.EndsAt);
        AdvanceTo(s, 11);
        s.Ground.Accrue();                                   // crossed 10: the Poison length restarts
        Assert.Equal(61, Statuses.Get(s.GetUnit(a), StatusIds.Poison)!.EndsAt);
    }

    [Fact]
    public void LeavingPoisonGroundResetsTheStandingTime()
    {
        var s = Duel(out var a, out _);
        Place.Unit(s, Side.A, Row.Middle);
        s.Ground.Place(GroundKind.Poison, Front0(), 30);
        AdvanceTo(s, 3);
        s.Ground.Accrue();
        Movement.Move(s, a, Row.Rear, 0);                    // steps off
        AdvanceTo(s, 4);
        s.Ground.Accrue();                                   // counted: not covered any more
        Movement.Move(s, a, Row.Front, 0);                   // steps back on
        AdvanceTo(s, 8);
        s.Ground.Accrue();                                   // 4 time units since: still under 5
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Poison));
    }

    [Fact]
    public void FireOnARowWithPoisonGroundExplodesEveryPoisonSpotOfThatRow()
    {
        var s = new BattleState(1);
        var u1 = Place.Unit(s, Side.A, Row.Front);
        var u2 = Place.Unit(s, Side.A, Row.Front);           // centers -1, 1
        var mid = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(u2).Ward = 8;
        s.Ground.Place(GroundKind.Poison, new Spot(Side.A, Row.Front, -1), 20);
        s.Ground.Place(GroundKind.Poison, new Spot(Side.A, Row.Front, 1), 20);
        s.Ground.Place(GroundKind.Poison, new Spot(Side.A, Row.Middle, 0), 20);
        s.DrainEvents();

        s.Ground.Place(GroundKind.Burning, new Spot(Side.A, Row.Front, -1), 20);

        Assert.Equal(34, s.GetUnit(u1).Hp);                  // 6 damage
        Assert.Equal(2, s.GetUnit(u2).Ward);                 // a Ward absorbs the explosion like any ground damage
        Assert.Equal(40, s.GetUnit(u2).Hp);
        Assert.Equal(40, s.GetUnit(mid).Hp);                 // another row: not exploded
        Assert.True(s.Ground.Has(new Spot(Side.A, Row.Middle, 0), GroundKind.Poison));
        Assert.False(s.Ground.Has(new Spot(Side.A, Row.Front, -1), GroundKind.Poison));
        Assert.True(s.Ground.Has(new Spot(Side.A, Row.Front, -1), GroundKind.Burning));   // the spot becomes burning ground
        Assert.True(s.Ground.Has(new Spot(Side.A, Row.Front, 1), GroundKind.Burning));
        Assert.Equal(2, s.DrainEvents().OfType<PoisonGroundExploded>().Count());
    }

    // ---- ice and water ----

    [Fact]
    public void AUnitPushedOntoIceSlidesOneRowFurtherAndBumpsApply()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Ice, new Spot(Side.A, Row.Middle, 0), 20);
        var r = Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(ForcedMoveStatus.Moved, r.Status);
        Assert.Equal(Row.Rear, s.Formation.PositionOf(a).Row);
    }

    [Fact]
    public void WithoutIceThePushStopsInTheDestinationRow()
    {
        var s = Duel(out var a, out _);
        Movement.ForceMove(s, a, Row.Middle);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(a).Row);
    }

    [Fact]
    public void SlidingBumpsTheUnitInTheRowItSlidesInto()
    {
        var s = Duel(out var a, out _);
        var rear = Place.Unit(s, Side.A, Row.Rear);
        s.Ground.Place(GroundKind.Ice, new Spot(Side.A, Row.Middle, 0), 20);
        var r = Movement.ForceMove(s, a, Row.Middle);          // slides into Rear at 100% overlap: the chain hits the boundary
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Contains(rear, r.BumpDamaged);
    }

    [Fact]
    public void IceOnTheLastRowSlidesNoFurther()
    {
        var s = Duel(out _, out _);
        Place.Unit(s, Side.A, Row.Middle);
        s.Ground.Place(GroundKind.Ice, new Spot(Side.A, Row.Rear, 0), 20);
        Assert.Equal(Row.Rear, s.Ground.SlideOnIce(Side.A, Row.Rear, 0, 1));
        Assert.Equal(Row.Middle, s.Ground.SlideOnIce(Side.A, Row.Middle, 0, -1)); // no ice there: stays
    }

    [Fact]
    public void WaterSpotsTouchSideBySideInARowAndAcrossNeighboringRows()
    {
        var s = Duel(out _, out _);
        var here = new Spot(Side.A, Row.Front, 0);
        s.Ground.Place(GroundKind.Water, here, 20);
        s.Ground.Place(GroundKind.Water, new Spot(Side.A, Row.Front, 2), 20);     // side by side
        s.Ground.Place(GroundKind.Water, new Spot(Side.A, Row.Front, 4), 20);     // one card gap: not touching
        s.Ground.Place(GroundKind.Water, new Spot(Side.A, Row.Middle, 1), 20);    // overlaps across neighboring rows
        s.Ground.Place(GroundKind.Water, new Spot(Side.A, Row.Rear, 0), 20);      // two rows away
        s.Ground.Place(GroundKind.Water, new Spot(Side.B, Row.Front, 0), 20);     // the other side
        var touching = s.Ground.WaterSpotsTouching(here);
        Assert.Equal(2, touching.Count);
        Assert.Contains(new Spot(Side.A, Row.Front, 2), touching);
        Assert.Contains(new Spot(Side.A, Row.Middle, 1), touching);
    }

    // ---- state ----

    [Fact]
    public void AccruingTwiceAtTheSameTimeChangesNothingAndCloneIsIndependent()
    {
        var s = Duel(out var a, out _);
        s.Ground.Place(GroundKind.Burning, Front0(), 20);
        AdvanceTo(s, 10);
        s.Ground.Accrue();
        s.Ground.Accrue();
        Assert.Equal(500, s.Ground.BuildupOf(a));
        var copy = s.Clone();
        copy.Ground.Remove(copy.Ground.Effects[0], GroundEndCause.Expired);
        Assert.Single(s.Ground.Effects);
        Assert.Empty(copy.Ground.Effects);
        Assert.Equal(500, copy.Ground.BuildupOf(a));
    }

    [Fact]
    public void ThereIsNoEffectOnAnEmptyRowToHurtAnyoneAndPlacingNeedsAPositiveLength()
    {
        var s = Duel(out _, out _);
        Assert.Throws<ArgumentOutOfRangeException>(() => s.Ground.Place(GroundKind.Water, Front0(), 0));
        s.Ground.Place(GroundKind.Burning, new Spot(Side.A, Row.Rear, 0), 20);
        AdvanceTo(s, 10);
        Assert.Empty(s.Ground.Tick());
    }
}
