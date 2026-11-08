using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class KnockoutTests
{
    private static (BattleState State, int Unit) KoSetup(int hp = 5)
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(u).Hp = hp;
        return (s, u);
    }

    [Fact]
    public void ADamagedUnitAtZeroIsKoAfterTheCheckAndItsStatusesClear()
    {
        var (s, u) = KoSetup();
        var unit = s.GetUnit(u);
        unit.Ward = 4;
        unit.Dampen = true;
        unit.IsImmobilized = true;
        unit.InterceptionBlocked = true;
        unit.IsStealthed = true;
        s.DealDamage(null, u, 99);
        s.DrainEvents();
        Assert.True(Knockout.Resolve(s, u));
        Assert.Equal(0, unit.Ward);
        Assert.False(unit.Dampen);
        Assert.False(unit.IsImmobilized);
        Assert.False(unit.InterceptionBlocked);
        Assert.False(unit.IsStealthed);
        Assert.Equal(new BattleEvent[]
        {
            new StealthChanged(u, false, StealthCause.KnockedOut),
            new UnitKnockedOut(u),
        }, s.DrainEvents());
    }

    [Fact]
    public void ABodyKeepsItsSlotAndRowCapacityAndNoLongerIntercepts()
    {
        // 20.5: a unit becomes KO: statuses clear; ordered occupancy and row capacity remain; it no longer intercepts.
        var s = new BattleState(1);
        var front = Place.Unit(s, Side.A, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Middle);
        Assert.Equal(new[] { front }, Interception.Defenders(s, target));
        s.GetUnit(front).Hp = 0;
        Knockout.Resolve(s, front);
        Assert.Equal(new Position(Side.A, Row.Front, 0), s.Formation.PositionOf(front));
        Assert.Equal(1, s.Formation.Count(Side.A, Row.Front));
        Assert.Empty(Interception.Defenders(s, target));
        for (var i = 0; i < 5; i++) Place.Unit(s, Side.A, Row.Front);
        Assert.False(s.Formation.CanPlace(Side.A, Row.Front)); // the body still takes a place
    }

    [Fact]
    public void ResolveDoesNothingForALivingOrAlreadyProcessedUnit()
    {
        var (s, u) = KoSetup(hp: 10);
        Assert.False(Knockout.Resolve(s, u));
        s.GetUnit(u).Hp = 0;
        Assert.True(Knockout.Resolve(s, u));
        s.DrainEvents();
        Assert.False(Knockout.Resolve(s, u));
        Assert.Empty(s.DrainEvents());
    }

    [Fact]
    public void KoRemovesTheTurnAndPendingAutoAndDelayedEventsButNotOthers()
    {
        var s = new BattleState(1);
        var owner = Place.Unit(s, Side.A, Row.Front);
        var other = Place.Unit(s, Side.A, Row.Front);
        s.Timeline.Add(TimelineKind.UnitTurn, 5, owner);
        s.Timeline.Add(TimelineKind.AutoEvent, 3, owner, "shot");
        s.Timeline.Add(TimelineKind.DelayedEvent, 4, owner, "comet");
        s.Timeline.Add(TimelineKind.UnitTurn, 6, other);
        s.GetUnit(owner).Hp = 0;
        Knockout.Resolve(s, owner);
        Assert.Equal(new[] { other }, s.Timeline.Upcoming().Select(e => e.OwnerUnitId!.Value));
    }

    [Fact]
    public void ASkillCanKeepTheEventsItAlreadyLaunched()
    {
        var s = new BattleState(1);
        var owner = Place.Unit(s, Side.A, Row.Front);
        s.Timeline.Add(TimelineKind.UnitTurn, 5, owner);
        s.Timeline.Add(TimelineKind.DelayedEvent, 4, owner, "comet");
        s.GetUnit(owner).Hp = 0;
        Knockout.Resolve(s, owner, keepPendingEvents: true);
        Assert.Equal(new[] { TimelineKind.DelayedEvent }, s.Timeline.Upcoming().Select(e => e.Kind));
    }

    [Fact]
    public void ATurretOfAKoUnitsSideStaysWhenTheOwnersAutoEventGoes()
    {
        // 20.5: the turret (its own unit) stays; the auto event the KO'd unit owns is removed.
        var s = new BattleState(1);
        var tinker = Place.Unit(s, Side.A, Row.Middle);
        var turret = Place.Unit(s, Side.A, Row.Front);
        s.Timeline.Add(TimelineKind.AutoEvent, 3, tinker, "tinker_event");
        s.Timeline.Add(TimelineKind.UnitTurn, 6, turret);
        s.GetUnit(tinker).Hp = 0;
        Knockout.Resolve(s, tinker);
        Assert.Equal(new[] { turret }, s.Timeline.Upcoming().Select(e => e.OwnerUnitId!.Value));
        Assert.False(s.GetUnit(turret).IsKnockedOut);
    }

    [Fact]
    public void SeveralUnitsKoInOneBlockAreResolvedInARandomOrderWithEqualChance()
    {
        var firsts = new int[2];
        for (ulong seed = 1; seed <= 4000; seed++)
        {
            var s = new BattleState(seed);
            var a = Place.Unit(s, Side.A, Row.Front);
            var b = Place.Unit(s, Side.B, Row.Front);
            s.GetUnit(a).Hp = 0;
            s.GetUnit(b).Hp = 0;
            var order = Knockout.ResolveAll(s);
            Assert.Equal(2, order.Count);
            firsts[order[0] == a ? 0 : 1]++;
        }
        Assert.All(firsts, c => Assert.InRange(c, 1800, 2200));
    }

    [Fact]
    public void ResolveAllIsReproducibleAndSkipsUnitsAlreadyProcessed()
    {
        BattleState Build()
        {
            var s = new BattleState(5);
            for (var i = 0; i < 4; i++) { var id = Place.Unit(s, Side.A, Row.Front); s.GetUnit(id).Hp = 0; }
            return s;
        }
        var first = Knockout.ResolveAll(Build());
        var second = Knockout.ResolveAll(Build());
        Assert.Equal(first, second);

        var s2 = Build();
        Knockout.Resolve(s2, 0);
        Assert.DoesNotContain(0, Knockout.ResolveAll(s2));
        Assert.Empty(Knockout.ResolveAll(s2));
    }

    // ---- revival ----

    [Fact]
    public void ARevivedUnitReturnsToItsSlotWithTheStatedHpAndNoFreeTurn()
    {
        var s = new BattleState(1);
        var pos = Place.Unit(s, Side.A, Row.Middle);
        Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(pos).Hp = 0;
        Knockout.Resolve(s, pos);
        s.Timeline.Add(TimelineKind.DelayedEvent, 3);
        s.Timeline.Pop(); // now 3
        s.DrainEvents();

        Knockout.Revive(s, pos, hp: 10, delay: 9);

        var unit = s.GetUnit(pos);
        Assert.False(unit.IsKnockedOut);
        Assert.Equal(10, unit.Hp);
        Assert.Equal(new Position(Side.A, Row.Middle, 0), s.Formation.PositionOf(pos));
        var turn = s.Timeline.TurnOf(pos)!;
        Assert.Equal(3 + Timeline.Wait(9, unit.Speed), turn.Due); // as though revival had been its own action
        Assert.NotEqual(s.Timeline.Now, turn.Due);                // no immediate turn
        Assert.Equal(new BattleEvent[] { new UnitRevived(pos, 10) }, s.DrainEvents());
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(25, 25)]
    [InlineData(500, 40)]
    public void ReviveHpIsAtLeastOneAndAtMostMaximum(int asked, int expected)
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(u).Hp = 0;
        Knockout.Revive(s, u, asked, 5);
        Assert.Equal(expected, s.GetUnit(u).Hp);
    }

    [Fact]
    public void OnlyAKoUnitCanBeRevived()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        Assert.Throws<InvalidOperationException>(() => Knockout.Revive(s, u, 5, 5));
    }

    [Fact]
    public void ARevivedUnitIsAnActiveDefenderAgainAndCanBeKoAgain()
    {
        var s = new BattleState(1);
        var front = Place.Unit(s, Side.A, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(front).Hp = 0;
        Knockout.Resolve(s, front);
        Assert.Empty(Interception.Defenders(s, target));
        Knockout.Revive(s, front, 10, 9);
        Assert.Equal(new[] { front }, Interception.Defenders(s, target));
        // No limit on repeated revival.
        s.DealDamage(null, front, 99);
        Assert.True(Knockout.Resolve(s, front));
        Knockout.Revive(s, front, 10, 9);
        Assert.False(s.GetUnit(front).IsKnockedOut);
    }

    [Fact]
    public void ARevivedUnitComesBackWithoutAnyStatuses()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(u).Ward = 5;
        s.GetUnit(u).Hp = 0;
        Knockout.Resolve(s, u);
        Knockout.Revive(s, u, 10, 5);
        Assert.Equal(0, s.GetUnit(u).Ward);
    }

    [Fact]
    public void ABodyThatWasNeverProcessedIsProcessedBeforeItIsRevived()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        s.Timeline.Add(TimelineKind.UnitTurn, 5, u);
        s.GetUnit(u).Hp = 0;
        Knockout.Revive(s, u, 10, 5);
        Assert.Single(s.Timeline.Upcoming()); // the old turn is gone, only the new one is there
    }

    [Fact]
    public void KoStateSurvivesClone()
    {
        var s = new BattleState(1);
        var u = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(u).Hp = 0;
        Knockout.Resolve(s, u);
        var copy = s.Clone();
        Assert.True(copy.GetUnit(u).KoProcessed);
        Assert.False(Knockout.Resolve(copy, u));
    }
}

public class StealthTests
{
    [Fact]
    public void AUnitGainsStealthAndItIsReported()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Middle);
        Place.Unit(s, Side.A, Row.Front);
        s.DrainEvents();
        Assert.True(Stealth.CanGain(s, a));
        Stealth.Gain(s, a);
        Assert.True(s.GetUnit(a).IsStealthed);
        Assert.Equal(new BattleEvent[] { new StealthChanged(a, true, StealthCause.Gained) }, s.DrainEvents());
    }

    [Fact]
    public void ASidesLastVisibleUnitCannotHideItself()
    {
        // 20.5: not legal, so a side never has zero visible units through its own stealth.
        var s = new BattleState(1);
        var alone = Place.Unit(s, Side.A, Row.Front);
        Assert.False(Stealth.CanGain(s, alone));
        Assert.Throws<InvalidOperationException>(() => Stealth.Gain(s, alone));

        var other = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(other).IsStealthed = true;
        Assert.False(Stealth.CanGain(s, alone)); // the only other unit is already hidden
    }

    [Fact]
    public void KoBodiesAndTheOtherSideDoNotCountAsVisibleUnits()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var body = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.B, Row.Front);
        s.GetUnit(body).Hp = 0;
        Assert.False(Stealth.CanGain(s, a));
    }

    [Fact]
    public void AKoOrAlreadyStealthedUnitCannotGainStealth()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(a).Hp = 0;
        s.GetUnit(b).IsStealthed = true;
        Assert.False(Stealth.CanGain(s, a));
        Assert.False(Stealth.CanGain(s, b));
    }

    [Theory]
    [InlineData(StealthCause.Attacked)]
    [InlineData(StealthCause.Hit)]
    [InlineData(StealthCause.Revealed)]
    public void BreakEndsStealthAndNamesTheCause(StealthCause cause)
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(a).IsStealthed = true;
        s.DrainEvents();
        Assert.True(Stealth.Break(s, a, cause));
        Assert.False(s.GetUnit(a).IsStealthed);
        Assert.Equal(new BattleEvent[] { new StealthChanged(a, false, cause) }, s.DrainEvents());
        Assert.False(Stealth.Break(s, a, cause)); // nothing left to break
        Assert.Empty(s.DrainEvents());
    }

    [Fact]
    public void ABumpHitEndsStealthWithAReportedCause()
    {
        // 20.5: a stealthed unit takes bump damage: it counts as a hit and stealth ends, Momentum does not change.
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Middle);
        var hidden = Place.Unit(s, Side.A, Row.Rear);
        s.GetUnit(hidden).IsStealthed = true;
        s.DrainEvents();
        Movement.ForceMove(s, a, Row.Middle);
        Assert.Contains(new StealthChanged(hidden, false, StealthCause.Hit), s.DrainEvents());
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void APoisonTickDoesNotEndStealthBecauseDealDamageLeavesItAlone()
    {
        // 20.5: a stealthed unit takes a poison tick: stealth does not end.
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        s.GetUnit(a).IsStealthed = true;
        s.DealDamage(null, a, 1, DamageSource.Periodic);
        Assert.True(s.GetUnit(a).IsStealthed);
    }

    [Fact]
    public void AStealthedAllyCannotInterceptAndStealthDoesNotBlockAreaPatterns()
    {
        // 20.5: a stealthed unit in front of an ally cannot intercept; a stealthed Rogue hit by Breath's splash is hit.
        var s = new BattleState(1);
        var hidden = Place.Unit(s, Side.A, Row.Front);
        var ally = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(hidden).IsStealthed = true;
        Assert.Empty(Interception.Defenders(s, ally));
        Assert.Contains(hidden, AreaPatterns.Circular(s, ally));
    }

    [Fact]
    public void AStealthedEnemyIsIgnoredByTargetingButNotByTheAreaOfEffect()
    {
        var s = new BattleState(1);
        var me = Place.Unit(s, Side.A, Row.Front);
        var enemy = Place.Unit(s, Side.B, Row.Front);
        Stealth.Break(s, enemy, StealthCause.Hit); // no-op: not stealthed yet
        s.GetUnit(enemy).IsStealthed = true;
        Assert.False(Targeting.IsLegalTarget(s, me, enemy, Reach.Unlimited));
        Assert.Contains(enemy, AreaPatterns.Row(s, Side.B, Row.Front));
    }
}
