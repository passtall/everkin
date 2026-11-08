using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class SummonTests
{
    private static SummonSpec Golem(int summoner, Side side = Side.A, int? lifetime = null, bool vanishes = false) =>
        new(side, "Clay Golem", UnitKind.Summon, MaxHp: 25, Speed: 3, summoner, TakesTurns: true,
            Lifetime: lifetime, DisappearsWithSummoner: vanishes);

    private static SummonSpec Trap(int summoner, Side side = Side.A) =>
        new(side, "Net Trap", UnitKind.Construct, MaxHp: 4, Speed: 1, summoner, TakesTurns: false);

    private static (BattleState S, int Summoner) Board()
    {
        var s = new BattleState(1);
        var summoner = Place.Unit(s, Side.A, Row.Middle);
        Place.Unit(s, Side.B, Row.Front);
        s.DrainEvents();
        return (s, summoner);
    }

    // ---- placement and cap ----

    [Fact]
    public void PlacementOffersEveryGapOfTheAllowedRowsWithRoom()
    {
        var (s, _) = Board();
        Place.Unit(s, Side.A, Row.Front);
        var options = Summons.PlacementOptions(s, Side.A, new[] { Row.Front, Row.Rear });
        Assert.Equal(new[] { new MoveOption(Row.Front, 0), new MoveOption(Row.Front, 1), new MoveOption(Row.Rear, 0) }, options);
    }

    [Fact]
    public void ASideAtItsCapOfEighteenCannotSummon()
    {
        // 20.5: a side has 18 units and its Hunter uses Net Trap: the skill is unavailable.
        var s = new BattleState(1);
        var hunter = Place.Unit(s, Side.A, Row.Front);
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.CanPlace(Side.A, row)) Place.Unit(s, Side.A, row);
        Assert.Equal(18, s.Formation.Count(Side.A));
        Assert.False(Summons.CanSummon(s, Side.A, new[] { Row.Front, Row.Middle, Row.Rear }));
        Assert.Throws<InvalidOperationException>(() => Summons.Summon(s, Trap(hunter), Row.Front, 0));
    }

    [Fact]
    public void AFullRowIsNotOfferedEvenIfTheSideHasRoomElsewhere()
    {
        var (s, _) = Board();
        for (var i = 0; i < 6; i++) Place.Unit(s, Side.A, Row.Rear);
        Assert.False(Summons.CanSummon(s, Side.A, new[] { Row.Rear }));
        Assert.True(Summons.CanSummon(s, Side.A, new[] { Row.Rear, Row.Front }));
    }

    [Fact]
    public void ASummonCanGoBetweenExistingUnitsAndTheRowRecenters()
    {
        var (s, summoner) = Board();
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.A, Row.Front);
        var id = Summons.Summon(s, Trap(summoner), Row.Front, 1);
        Assert.Equal(new[] { a, id, b }, s.Formation.Units(Side.A, Row.Front));
    }

    [Fact]
    public void WithoutARangeASummonGoesInAFreeGapNextToItsUser()
    {
        var (s, summoner) = Board();
        Place.Unit(s, Side.A, Row.Middle);
        var gaps = Summons.GapsNextToUser(s, summoner);
        Assert.Equal(new[] { new MoveOption(Row.Middle, 0), new MoveOption(Row.Middle, 1) }, gaps);
        for (var i = 0; i < 4; i++) Place.Unit(s, Side.A, Row.Middle);
        Assert.Empty(Summons.GapsNextToUser(s, summoner)); // the row is full
    }

    // ---- stats, turns, lifetime ----

    [Fact]
    public void ASummonHasTheSkillsFixedHpAndSpeedAndItsSummonersPower()
    {
        var (s, summoner) = Board();
        s.GetUnit(summoner).Power = 7;
        var id = Summons.Summon(s, Golem(summoner), Row.Front, 0);
        var golem = s.GetUnit(id);
        Assert.Equal(25, golem.MaxHp);
        Assert.Equal(25, golem.Hp);
        Assert.Equal(3, golem.Speed);
        Assert.Equal(UnitKind.Summon, golem.Kind);
        Assert.Equal(7, s.PowerOf(id));
        s.GetUnit(summoner).Power = 4;           // damage and healing scale with the summoner's Power as it is now
        Assert.Equal(4, s.PowerOf(id));
        Assert.Equal(s.GetUnit(summoner).Power, s.PowerOf(summoner));
    }

    [Fact]
    public void ATurnTakingSummonFirstActsElevenMinusSpeedTimeUnitsAfterItAppears()
    {
        // 20.5: a Speed 6 turn-taking summon appears at time 10: it first acts at time 15.
        var (s, summoner) = Board();
        s.Timeline.Add(TimelineKind.DelayedEvent, 10);
        s.Timeline.Pop();
        Assert.Equal(10, s.Timeline.Now);
        var id = Summons.Summon(s, Golem(summoner) with { Speed = 6 }, Row.Front, 0);
        Assert.Equal(15, s.Timeline.TurnOf(id)!.Due);
    }

    [Fact]
    public void AConstructTakesNoTurns()
    {
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Trap(summoner), Row.Front, 0);
        Assert.Null(s.Timeline.TurnOf(id));
        Assert.Empty(s.Timeline.Upcoming());
    }

    [Fact]
    public void ASummonIsReportedWhereItAppeared()
    {
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Golem(summoner), Row.Front, 0);
        Assert.Contains(new UnitSummoned(id, summoner, UnitKind.Summon, new Position(Side.A, Row.Front, 0)), s.DrainEvents());
    }

    [Fact]
    public void ATemporaryUnitDisappearsWhenItsTimeRunsOutWithoutBeingAKo()
    {
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Golem(summoner, lifetime: 30) with { Kind = UnitKind.Temporary }, Row.Front, 0);
        s.DrainEvents();
        TimelineEntry? entry;
        while ((entry = s.Timeline.Pop()) is not null)
        {
            Statuses.HandleExpiry(s, entry);
            if (entry.Tag == Summons.ExpiryTag) break;
        }
        Assert.Equal(30, s.Timeline.Now);
        Assert.False(s.Formation.Contains(id));
        Assert.True(s.GetUnit(id).IsRemoved);
        var events = s.DrainEvents();
        Assert.Contains(new UnitRemoved(id, RemovalCause.Expired, new Position(Side.A, Row.Front, 0)), events);
        Assert.DoesNotContain(events, e => e is UnitKnockedOut); // KO-triggered effects do not fire
        Assert.Empty(s.Timeline.Upcoming());                     // its turn is gone too
    }

    // ---- KO of summons ----

    [Fact]
    public void AKoSummonIsRemovedAtOnceAndFreesItsSlot()
    {
        // 20.5: a trap is KO'd: it is removed at once and its slot is free. It cannot be revived.
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Trap(summoner), Row.Front, 0);
        s.DealDamage(null, id, 99);
        Assert.True(Knockout.Resolve(s, id));
        Assert.False(s.Formation.Contains(id));
        Assert.Equal(0, s.Formation.Count(Side.A, Row.Front));
        Assert.True(s.GetUnit(id).IsRemoved);
        Assert.Throws<InvalidOperationException>(() => Knockout.Revive(s, id, 4, 5));
    }

    [Fact]
    public void AKoSummonStillCountsAsAKoForKoTriggeredEffects()
    {
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Golem(summoner), Row.Front, 0);
        s.GetUnit(id).Hp = 0;
        s.DrainEvents();
        Knockout.Resolve(s, id);
        var events = s.DrainEvents();
        Assert.IsType<UnitKnockedOut>(events[0]);
        Assert.IsType<UnitRemoved>(events[1]);
        Assert.Equal(RemovalCause.Ko, ((UnitRemoved)events[1]).Cause);
    }

    [Fact]
    public void AHunterKoedWhileItsTrapsAreOnTheBoardLeavesTheTrapsInPlace()
    {
        // 20.5: the traps persist.
        var (s, hunter) = Board();
        var trap = Summons.Summon(s, Trap(hunter), Row.Front, 0);
        s.GetUnit(hunter).Hp = 0;
        Knockout.Resolve(s, hunter);
        Assert.True(s.Formation.Contains(trap));
        Assert.False(s.GetUnit(trap).IsRemoved);
        Assert.True(s.GetUnit(hunter).IsKnockedOut);
    }

    [Fact]
    public void SummonsPersistWhenTheirSummonerIsKoUnlessTheSkillSaysTheyDisappear()
    {
        var (s, summoner) = Board();
        var stays = Summons.Summon(s, Golem(summoner), Row.Front, 0);
        var vanishes = Summons.Summon(s, Golem(summoner, vanishes: true), Row.Front, 1);
        s.GetUnit(summoner).Hp = 0;
        s.DrainEvents();
        Knockout.Resolve(s, summoner);
        Assert.True(s.Formation.Contains(stays));
        Assert.False(s.Formation.Contains(vanishes));
        Assert.Contains(s.DrainEvents(), e => e is UnitRemoved { UnitId: var u, Cause: RemovalCause.SummonerGone } && u == vanishes);
    }

    [Fact]
    public void AnOriginalCreatureIsNotRemovedWhenKoAndASummonDoesNotCountAsOne()
    {
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Golem(summoner), Row.Front, 0);
        Assert.True(s.GetUnit(id).IsSummoned);
        Assert.False(s.GetUnit(summoner).IsSummoned);
        s.GetUnit(summoner).Hp = 0;
        Knockout.Resolve(s, summoner);
        Assert.True(s.Formation.Contains(summoner)); // the body stays
    }

    // ---- summons are units ----

    [Fact]
    public void ASummonInterceptsIsTargetableAndTakesStatusesLikeAnyUnit()
    {
        var (s, summoner) = Board();
        var target = Place.Unit(s, Side.A, Row.Rear);
        var golem = Summons.Summon(s, Golem(summoner), Row.Middle, 1);
        // A lone Rear unit at center 0 is covered by the Middle summon (Middle: 2 cards, centers -1 and 1 -> 50%).
        Assert.Contains(golem, Interception.Defenders(s, target));
        var enemy = s.Formation.Units(Side.B, Row.Front)[0];
        Assert.True(Targeting.IsLegalTarget(s, enemy, golem, Reach.Unlimited));
        Statuses.Apply(s, new StatusApplication(golem, StatusIds.Poison, Duration: 50));
        Assert.True(Statuses.Has(s.GetUnit(golem), StatusIds.Poison));
    }

    [Fact]
    public void SummonedUnitsSurviveClone()
    {
        var (s, summoner) = Board();
        var id = Summons.Summon(s, Golem(summoner, vanishes: true), Row.Front, 0);
        var copy = s.Clone();
        var u = copy.GetUnit(id);
        Assert.Equal(UnitKind.Summon, u.Kind);
        Assert.Equal(summoner, u.SummonerId);
        Assert.True(u.DisappearsWithSummoner);
        Summons.Remove(copy, id, RemovalCause.Ko);
        Assert.True(s.Formation.Contains(id));
    }

    [Fact]
    public void ASummonSpecMustNotBeAPartyCreature()
    {
        var (s, summoner) = Board();
        Assert.Throws<ArgumentException>(() =>
            Summons.Summon(s, Golem(summoner) with { Kind = UnitKind.Creature }, Row.Front, 0));
    }
}

public class UndeadTests
{
    private static (BattleState S, int Necro, int Body) Setup(bool bodyOnEnemySide = true)
    {
        var s = new BattleState(1);
        var necro = Place.Unit(s, Side.A, Row.Middle);
        var body = Place.Unit(s, bodyOnEnemySide ? Side.B : Side.A, Row.Front);
        s.GetUnit(body).Speed = 6;
        s.GetUnit(body).Hp = 0;
        Knockout.Resolve(s, body);
        s.DrainEvents();
        return (s, necro, body);
    }

    [Fact]
    public void RaisingMovesTheBodyFromEitherSideToTheNecromancersSideAndFreesItsOldSlot()
    {
        var (s, necro, body) = Setup();
        var options = Necromancy.RaisePlacements(s, necro, body);
        Assert.Contains(new MoveOption(Row.Front, 0), options);
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 12);
        var u = s.GetUnit(body);
        Assert.Equal(Side.A, u.Side);
        Assert.Equal(Side.B, u.OriginalSide);
        Assert.True(u.IsUndead);
        Assert.Equal(12, u.Hp);
        Assert.Equal(6, u.Speed);                        // it keeps its creature's Speed
        Assert.Equal(new Position(Side.A, Row.Front, 0), s.Formation.PositionOf(body));
        Assert.Equal(0, s.Formation.Count(Side.B));       // the old slot is free
        Assert.Equal(new BattleEvent[]
        {
            new UnitRaised(body, necro, new Position(Side.B, Row.Front, 0), new Position(Side.A, Row.Front, 0), 12),
        }, s.DrainEvents());
    }

    [Fact]
    public void AnUndeadFirstActsElevenMinusSpeedAfterItAppears()
    {
        var (s, necro, body) = Setup();
        Necromancy.Raise(s, necro, body, Row.Rear, 0, hp: 12);
        Assert.Equal(5, s.Timeline.TurnOf(body)!.Due); // 11 - 6
    }

    [Fact]
    public void ARaiseIsImpossibleWithoutAFreeSlotOnTheNecromancersSide()
    {
        var (s, necro, body) = Setup();
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.CanPlace(Side.A, row)) Place.Unit(s, Side.A, row);
        Assert.Empty(Necromancy.RaisePlacements(s, necro, body));
        Assert.Throws<InvalidOperationException>(() => Necromancy.Raise(s, necro, body, Row.Front, 0, 5));
    }

    [Fact]
    public void ABodyOnTheNecromancersOwnSideFreesItsSlotForTheRaise()
    {
        var (s, necro, body) = Setup(bodyOnEnemySide: false);
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.CanPlace(Side.A, row)) Place.Unit(s, Side.A, row);
        Assert.NotEmpty(Necromancy.RaisePlacements(s, necro, body)); // the body's own slot is free once it leaves
    }

    [Fact]
    public void OnlyAKoBodyCanBeRaised()
    {
        var s = new BattleState(1);
        var necro = Place.Unit(s, Side.A, Row.Middle);
        var alive = Place.Unit(s, Side.B, Row.Front);
        Assert.Empty(Necromancy.RaisePlacements(s, necro, alive));
    }

    [Fact]
    public void HealingAnUndeadDealsThatAmountAsDamageInstead()
    {
        var (s, necro, body) = Setup();
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 12);
        Assert.Equal(0, s.Heal(necro, body, 5));
        Assert.Equal(7, s.GetUnit(body).Hp);
        s.Heal(necro, body, 99);
        Assert.True(s.GetUnit(body).IsKnockedOut);
    }

    [Fact]
    public void AKoUndeadLeavesItsBodyOnTheNecromancersSide()
    {
        var (s, necro, body) = Setup();
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 3);
        s.DealDamage(null, body, 9);
        Knockout.Resolve(s, body);
        Assert.True(s.Formation.Contains(body));
        Assert.False(s.GetUnit(body).IsRemoved);
        Assert.Equal(Side.A, s.Formation.PositionOf(body).Side);
        Assert.Equal(Side.B, s.GetUnit(body).OriginalSide); // while raised, the original still counts as KO for its owner
    }

    // ---- resurrection restores ----

    [Fact]
    public void AResurrectionRestoresTheOriginalCreatureToItsOwnersSideNearestItsOldRow()
    {
        var (s, necro, body) = Setup();
        s.GetUnit(body).MaxHp = 40;
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 12, power: 9);
        s.Timeline.Add(TimelineKind.DelayedEvent, 4);
        s.Timeline.Pop();
        s.DrainEvents();

        Necromancy.Restore(s, body, hp: 10, delay: 9);

        var u = s.GetUnit(body);
        Assert.False(u.IsUndead);
        Assert.Equal(Side.B, u.Side);
        Assert.Equal(new Position(Side.B, Row.Front, 0), s.Formation.PositionOf(body)); // its old row was Front
        Assert.Equal(40, u.MaxHp);
        Assert.Equal(5, u.Power);                       // the original Power, not the undead's 9
        Assert.Equal(10, u.Hp);
        Assert.Null(u.SummonerId);
        Assert.Equal(4 + Timeline.Wait(9, 6), s.Timeline.TurnOf(body)!.Due); // scheduled as a new action
        Assert.Equal(0, s.Formation.Count(Side.A, Row.Front));
    }

    [Fact]
    public void ARestoreWorksOnAKoUndeadToo()
    {
        var (s, necro, body) = Setup();
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 3);
        s.DealDamage(null, body, 9);
        Knockout.Resolve(s, body);
        Necromancy.Restore(s, body, hp: 8, delay: 9);
        Assert.False(s.GetUnit(body).IsKnockedOut);
        Assert.Equal(Side.B, s.Formation.PositionOf(body).Side);
    }

    [Fact]
    public void WithoutAFreeSlotOnTheOwnersSideTheUndeadCannotBeRestored()
    {
        var (s, necro, body) = Setup();
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 12);
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.CanPlace(Side.B, row)) Place.Unit(s, Side.B, row);
        Assert.False(Necromancy.CanRestore(s, body));
        Assert.Throws<InvalidOperationException>(() => Necromancy.Restore(s, body, 5, 9));
    }

    [Fact]
    public void RestoringUsesTheNearestRowWithRoomAndAFreeTieIsDecidedByTheSeededRoll()
    {
        // The old row was Middle; Front and Rear are equally near and free: the roll picks one, each with equal chance.
        var counts = new Dictionary<Row, int>();
        for (ulong seed = 1; seed <= 2000; seed++)
        {
            var s = new BattleState(seed);
            var necro = Place.Unit(s, Side.A, Row.Middle);
            var body = Place.Unit(s, Side.B, Row.Middle);
            s.GetUnit(body).Hp = 0;
            Knockout.Resolve(s, body);
            Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 5);
            for (var i = 0; i < 6; i++) Place.Unit(s, Side.B, Row.Middle); // Middle is full
            Necromancy.Restore(s, body, 5, 9);
            var row = s.Formation.PositionOf(body).Row;
            counts[row] = counts.GetValueOrDefault(row) + 1;
        }
        Assert.Equal(2, counts.Count);
        Assert.All(counts.Values, c => Assert.InRange(c, 900, 1100));
    }
}
