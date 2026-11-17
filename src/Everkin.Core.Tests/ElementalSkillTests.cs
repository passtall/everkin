using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The Pyromancer, Stormcaller and Cryomancer.</summary>
public class ElementalSkillTests
{
    [Fact]
    public void AllElementalClassesAreImplemented()
    {
        AssertClassImplemented("pyromancer", magical: true);
        AssertClassImplemented("stormcaller", magical: true);
        AssertClassImplemented("cryomancer", magical: true);
    }

    private static Spot SpotOf(BattleState s, int unit) => s.Ground.SpotOf(unit);

    // ---- Pyromancer ----

    [Fact]
    public void FireballHitsForEightToNineAndSetsFireOnTheTargetsSpot()
    {
        var (s, p, e) = Board(1, "fireball");
        var spot = SpotOf(s, e);
        Run(s, Use(p, "fireball", e));
        Assert.InRange(Lost(s, e), 8, 9);
        var fire = s.Ground.Find(spot, GroundKind.Burning)!;
        Assert.Equal(20, fire.EndsAt - s.Timeline.Now);
    }

    [Fact]
    public void AnInterceptedFireballSetsFireOnTheInterceptorsSpot()
    {
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = new BattleState(seed);
            var p = World.Unit(s, Side.A, Row.Rear, "fireball");
            s.ShiftMomentum(Side.A, 6);
            var guard = World.Unit(s, Side.B, Row.Front);
            var target = World.Unit(s, Side.B, Row.Middle);
            var events = Run(s, Use(p, "fireball", target));
            if (events.OfType<Intercepted>().FirstOrDefault() is not { } ic) continue;
            Assert.Equal(guard, ic.InterceptorUnitId);
            Assert.True(s.Ground.Has(SpotOf(s, guard), GroundKind.Burning));
            return;
        }
        Assert.Fail("never intercepted");
    }

    [Fact]
    public void KindleSetsFireOnAUnitsSpotAndTheSpotsNextToIt()
    {
        var s = new BattleState(1);
        var p = World.Unit(s, Side.A, Row.Front, "kindle");
        var l = World.Unit(s, Side.B, Row.Front);
        var m = World.Unit(s, Side.B, Row.Front);
        var r = World.Unit(s, Side.B, Row.Front);                                           // centres -2, 0, 2
        Run(s, Use(p, "kindle", m));
        Assert.All(new[] { -2, 0, 2 }, x => Assert.True(s.Ground.Has(new Spot(Side.B, Row.Front, x), GroundKind.Burning)));
        Assert.False(s.Ground.Has(new Spot(Side.B, Row.Front, 4), GroundKind.Burning));
        Assert.Equal(0, Lost(s, m));
    }

    [Fact]
    public void KindleAtTheEdgeOfTheRowStaysInsideTheRowsWidth()
    {
        var s = new BattleState(1);
        var p = World.Unit(s, Side.A, Row.Front, "kindle");
        var units = Enumerable.Range(0, 6).Select(_ => World.Unit(s, Side.B, Row.Front)).ToArray();     // centres -5 .. 5
        Run(s, Use(p, "kindle", units[5]));
        Assert.True(s.Ground.Has(new Spot(Side.B, Row.Front, 5), GroundKind.Burning));
        Assert.True(s.Ground.Has(new Spot(Side.B, Row.Front, 3), GroundKind.Burning));
        Assert.DoesNotContain(s.Ground.Effects, f => Math.Abs(f.Spot.Coordinate) > 5);
    }

    [Fact]
    public void RowOfFireBurnsTheSpotOfEveryUnitInARowAndNotTheEmptyGaps()
    {
        var s = new BattleState(1);
        var p = World.Unit(s, Side.A, Row.Front, "row_of_fire");
        s.ShiftMomentum(Side.A, 6);
        var a = World.Unit(s, Side.B, Row.Middle);
        var b = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(p, "row_of_fire", row: new RowTarget(Side.B, Row.Middle)));
        Assert.Equal(2, s.Ground.Effects.Count(f => f.Kind == GroundKind.Burning));
        Assert.True(s.Ground.Has(SpotOf(s, a), GroundKind.Burning));
        Assert.True(s.Ground.Has(SpotOf(s, b), GroundKind.Burning));
        Assert.Equal(3, s.Momentum.Value);
    }

    [Fact]
    public void WildfireDoublesTheLengthOfTheBurningGroundAndInfernoItsDamage()
    {
        var (s, p, e) = Board(1, "kindle", "wildfire", "inferno");
        var spot = SpotOf(s, e);
        Run(s, Use(p, "kindle", e));
        Assert.Equal(40, s.Ground.Find(spot, GroundKind.Burning)!.EndsAt - s.Timeline.Now);
        AdvanceTo(s, 10);
        s.Ground.Accrue();
        Assert.Equal(1000, s.Ground.BuildupOf(e));                                          // 10 time units x 1 per time unit (double of 0.5)

        var (s2, p2, e2) = Board(1, "kindle");
        Run(s2, Use(p2, "kindle", e2));
        AdvanceTo(s2, 10);
        s2.Ground.Accrue();
        Assert.Equal(500, s2.Ground.BuildupOf(e2));
    }

    [Fact]
    public void AshwalkerTakesNoDamageFromBurningGroundAndGainsOneSpeedWhileOnIt()
    {
        var s = new BattleState(1);
        var p = World.Unit(s, Side.A, Row.Front, "ashwalker");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Assert.Equal(5, s.GetUnit(p).EffectiveSpeed);
        s.Ground.Place(GroundKind.Burning, SpotOf(s, p), 20);
        s.RunAuras();
        Assert.Equal(6, s.GetUnit(p).EffectiveSpeed);
        AdvanceTo(s, 10);
        s.Ground.Accrue();
        Assert.Equal(0, s.Ground.BuildupOf(p));
        Movement.Move(s, p, Row.Middle, 0);
        s.RunAuras();
        Assert.Equal(5, s.GetUnit(p).EffectiveSpeed);
    }

    // ---- Stormcaller ----

    [Fact]
    public void LightningBoltHitsForNineToTenAndIsNeverIntercepted()
    {
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 60; seed++)
        {
            var s = new BattleState(seed);
            var c = World.Unit(s, Side.A, Row.Front, "lightning_bolt");
            s.ShiftMomentum(Side.A, 6);
            World.Unit(s, Side.B, Row.Front);
            var back = World.Unit(s, Side.B, Row.Middle);
            Assert.DoesNotContain(Run(s, Use(c, "lightning_bolt", back)), e => e is Intercepted);
            damages.Add(Lost(s, back));
        }
        Assert.Equal(new[] { 9, 10 }, damages.OrderBy(x => x));
    }

    [Fact]
    public void LightningBoltExplodesThePoisonGasTheTargetStandsInAndTheGasTouchingIt()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "lightning_bolt");
        s.ShiftMomentum(Side.A, 6);
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        s.Ground.Place(GroundKind.Poison, SpotOf(s, a), 20);
        s.Ground.Place(GroundKind.Poison, SpotOf(s, b), 20);
        Run(s, Use(c, "lightning_bolt", a));
        Assert.False(s.Ground.Has(SpotOf(s, a), GroundKind.Poison));
        Assert.False(s.Ground.Has(SpotOf(s, b), GroundKind.Poison));
        Assert.Equal(6, Lost(s, b));                                                         // the explosion hit the neighbour
        Assert.InRange(Lost(s, a), 15, 16);                                                  // bolt plus explosion
    }

    [Fact]
    public void ChainLightningHitsFourUnitsOnceEachAndStopsWhenNothingIsLeft()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "chain_lightning");
        s.ShiftMomentum(Side.A, 6);
        var row = Enumerable.Range(0, 6).Select(_ => World.Unit(s, Side.B, Row.Front)).ToArray();
        Run(s, Use(c, "chain_lightning", row[0]));
        var hit = row.Where(id => Lost(s, id) > 0).ToList();
        Assert.Equal(4, hit.Count);
        Assert.All(hit, id => Assert.InRange(Lost(s, id), 1, 2));
        Assert.Equal(7, s.Momentum.Value + 0);  // M5.10 costs
    }

    [Fact]
    public void ChainLightningEndsEarlyWhenNoUnshitAdjacentUnitIsLeft()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "chain_lightning");
        s.ShiftMomentum(Side.A, 6);
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(c, "chain_lightning", a));
        Assert.InRange(Lost(s, a), 1, 2);
        Assert.InRange(Lost(s, b), 1, 2);
    }

    [Fact]
    public void ChainLightningWithOvercharge_JumpsTwoMoreTimes()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "chain_lightning", "overcharge");
        s.ShiftMomentum(Side.A, 6);
        var row = Enumerable.Range(0, 6).Select(_ => World.Unit(s, Side.B, Row.Front)).ToArray();
        Run(s, Use(c, "chain_lightning", row[0]));
        Assert.Equal(6, row.Count(id => Lost(s, id) > 0));
    }

    [Fact]
    public void EachChainHitExplodesTheGasUnderItsUnit()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "chain_lightning");
        s.ShiftMomentum(Side.A, 6);
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        s.Ground.Place(GroundKind.Poison, SpotOf(s, b), 20);
        Run(s, Use(c, "chain_lightning", a));
        Assert.False(s.Ground.Has(SpotOf(s, b), GroundKind.Poison));
        Assert.True(Lost(s, b) >= 6);
    }

    [Fact]
    public void TempestPushesEveryUnitOfARowOneRowBackKeepingItsShapeAndTheRearStays()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "tempest");
        s.ShiftMomentum(Side.A, 6);
        var f = Enumerable.Range(0, 3).Select(_ => World.Unit(s, Side.B, Row.Front)).ToArray();
        var rear = World.Unit(s, Side.B, Row.Rear);
        Run(s, Use(c, "tempest", row: new RowTarget(Side.B, Row.Front)));
        Assert.Equal(f, s.Formation.Units(Side.B, Row.Middle));
        Assert.Equal(4, s.Momentum.Value);
        K.Ready(s, c);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(c, "tempest", row: new RowTarget(Side.B, Row.Rear)));                      // the Rear row stays
        Assert.Equal(Row.Rear, s.Formation.PositionOf(rear).Row);
    }

    [Fact]
    public void StaticFieldStunsAUnitThatHitsTheStormcallerInMeleeForFive()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "static_field");
        var e = World.Unit(s, Side.B, Row.Front, "tail_club", "rivet_shot");
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(e, "rivet_shot", c));                                                      // not melee: nothing
        Assert.False(Has(s, e, StatusIds.Stunned));
        K.Ready(s, e);
        Run(s, Use(e, "tail_club", c));
        Assert.Equal(5, Statuses.Get(s.GetUnit(e), StatusIds.Stunned)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void StaticFieldDoesNotTriggerOnBumpDamage()
    {
        // 20.5: bump damage is not melee.
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "static_field");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.DealDamage(null, c, 1, DamageSource.Unreducible);
        Reactions.Resolve(s, new[] { new Trigger(TriggerKind.WhenHit, c, null, EffectTags.None) });
        Assert.False(Has(s, e, StatusIds.Stunned));
    }

    [Fact]
    public void EyeOfTheStormStopsPushesPullsAndStuns()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "eye_of_the_storm");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Assert.False(Statuses.Apply(s, new StatusApplication(c, StatusIds.Stunned, Duration: 5)));
        Assert.Empty(Movement.ForceMove(s, c, Row.Middle, Side.B).Moves);
        Assert.True(Statuses.Apply(s, new StatusApplication(c, StatusIds.Blind, Duration: 5)));
    }

    // ---- Cryomancer ----

    [Fact]
    public void IceLanceHitsForSevenToEightAndAFrozenTargetTakesDouble()
    {
        var (s, c, e) = Board(1, "ice_lance");
        Run(s, Use(c, "ice_lance", e));
        Assert.InRange(Lost(s, e), 7, 8);

        var (s2, c2, e2) = Board(1, "ice_lance");
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Frozen));
        Run(s2, Use(c2, "ice_lance", e2));
        Assert.InRange(Lost(s2, e2), 14, 16);
    }

    [Fact]
    public void FreezeFreezesAUnitAndItsNextTurnIsSkipped()
    {
        var (s, c, e) = Board(1, "freeze");
        s.Timeline.ScheduleTurn(e, 5, 5);
        Run(s, Use(c, "freeze", e));
        Assert.True(Has(s, e, StatusIds.Frozen));
        Assert.Equal(5, s.Momentum.Value);
    }

    [Fact]
    public void FrostNovaHitsEveryEnemyInMeleeReachForOneToTwoAndFreezesThem()
    {
        var (s, c, e1) = Board(1, "frost_nova");
        var e2 = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(c, "frost_nova"));
        Assert.All(new[] { e1, e2 }, id => { Assert.InRange(Lost(s, id), 1, 2); Assert.True(Has(s, id, StatusIds.Frozen)); });
        Assert.Equal(0, Lost(s, back));
        Assert.False(Has(s, back, StatusIds.Frozen));
    }

    [Fact]
    public void ColdHeartFreezesForTwoTurnsAndKeepsOnlyOneUnitFrozen()
    {
        var (s, c, e1) = Board(1, "freeze", "cold_heart");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(c, "freeze", e1));
        Assert.Equal(2, Statuses.Get(s.GetUnit(e1), StatusIds.Frozen)!.UsesLeft);
        K.Ready(s, c);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(c, "freeze", e2));
        Assert.False(Has(s, e1, StatusIds.Frozen));                                           // the earlier one thaws
        Assert.True(Has(s, e2, StatusIds.Frozen));
    }

    [Fact]
    public void ColdHeartWithFrostNovaKeepsOneUnitBySeededRoll()
    {
        var counts = new int[2];
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var (s, c, e1) = Board(seed, "frost_nova", "cold_heart");
            var e2 = World.Unit(s, Side.B, Row.Front);
            Run(s, Use(c, "frost_nova"));
            Assert.Equal(1, new[] { e1, e2 }.Count(id => Has(s, id, StatusIds.Frozen)));
            counts[Has(s, e1, StatusIds.Frozen) ? 0 : 1]++;
        }
        Assert.All(counts, n => Assert.InRange(n, 70, 130));
    }

    [Fact]
    public void BrittleMakesFrozenEnemiesTakeFiftyPercentMoreBeforeWard()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "brittle");
        var e = World.Unit(s, Side.B, Row.Front);
        var ally = World.Unit(s, Side.A, Row.Middle);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Frozen));
        Assert.Equal(9, s.DealDamage(ally, e, 6).HpLost);                                     // 6 x 3 / 2
        Assert.Equal(7, s.DealDamage(ally, e, 5).HpLost);                                     // 7.5 rounded down
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Frozen));
        Assert.Equal(6, s.DealDamage(e, ally, 6).HpLost);                                     // only Frozen enemies of the Cryomancer
    }

    [Fact]
    public void DeepChillPutsIceOnTheSpotOfAUnitTheCryomancerFreezes()
    {
        var (s, c, e) = Board(1, "freeze", "deep_chill");
        var spot = SpotOf(s, e);
        Run(s, Use(c, "freeze", e));
        Assert.Equal(20, s.Ground.Find(spot, GroundKind.Ice)!.EndsAt - s.Timeline.Now);
    }
}
