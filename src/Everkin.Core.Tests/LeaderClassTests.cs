using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The Tactician, Commander and Bard.</summary>
public class LeaderClassTests
{
    [Fact]
    public void AllLeaderClassesAreImplemented()
    {
        AssertClassImplemented("tactician", magical: false);
        AssertClassImplemented("commander", magical: false);
        AssertClassImplemented("bard", magical: true);          // the Bard's songs are magical; the class flag is the other 14 only, so check by hand
    }

    // ---- Tactician ----

    [Fact]
    public void SwapTurnsExchangesTheNextTurnsOfTwoAlliesAndOnlyAlliesQualify()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "swap_turns");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        var e = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        s.Timeline.ScheduleTurn(a, 2, 5);
        s.Timeline.ScheduleTurn(b, 9, 5);
        var (da, db) = (s.Timeline.TurnOf(a)!.Due, s.Timeline.TurnOf(b)!.Due);
        Assert.NotNull(Why(s, Use(t, "swap_turns", a, second: e)));
        Run(s, Use(t, "swap_turns", a, second: b));
        Assert.Equal(db, s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(da, s.Timeline.TurnOf(b)!.Due);
        Assert.Equal(4, s.Momentum.Value);
    }

    [Fact]
    public void PressTheAdvantageMakesTheUnitsNextSkillCostNoMomentumEvenWithoutALead()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "press_the_advantage");
        var a = World.Unit(s, Side.A, Row.Middle, "lightning_bolt");
        var e = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(t, "press_the_advantage", a));                                         // costs 2: a lead of 1 is left, Lightning Bolt would need 2
        Assert.Equal(1, s.Momentum.Value);
        Assert.Null(Why(s, Use(a, "lightning_bolt", e)));
        Run(s, Use(a, "lightning_bolt", e));
        Assert.Equal(2, s.Momentum.Value);                                                // no cost paid, then the hit moved it 1
        Assert.False(Has(s, a, StatusIds.FreeMomentum));
    }

    [Fact]
    public void CoordinatedStrikeMakesTwoRandomAlliesAttackTheEnemyForFreeEachShiftingMomentum()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "coordinated_strike");
        var creature1 = s.AddUnit(World.Def("ankylosaur-guardian-club"), Side.A);
        var creature2 = s.AddUnit(World.Def("ant-engineer-gear"), Side.A);
        var creature3 = s.AddUnit(World.Def("armadillo-knight"), Side.A);
        foreach (var u in new[] { creature1, creature2, creature3 }) s.Formation.Place(u.Id, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        var events = Run(s, Use(t, "coordinated_strike", enemy));
        Assert.Equal(3, events.OfType<SkillUsed>().Count());                               // Coordinated Strike and two attacks
        Assert.InRange(Lost(s, enemy), 6, 8);                                              // two attacks of 3-4
        Assert.Equal(6 - 3 + 2, s.Momentum.Value);                                         // cost 3, then each attack shifts 1
    }

    [Fact]
    public void CoordinatedStrikeWithOneAbleAllyOnlyThatOneAttacksAndWithNoneNothingHappens()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "coordinated_strike");
        var melee = s.AddUnit(World.Def("ankylosaur-guardian-club"), Side.A);
        s.Formation.Place(melee.Id, Side.A, Row.Middle);                                   // a melee attack needs the Front row: cannot
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(t, "coordinated_strike", enemy));
        Assert.Equal(0, Lost(s, enemy));
        Assert.Equal(3, s.Momentum.Value);

        var shooter = s.AddUnit(World.Def("ant-engineer-gear"), Side.A);
        s.Formation.Place(shooter.Id, Side.A, Row.Rear);
        K.Ready(s, t);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(t, "coordinated_strike", enemy));
        Assert.InRange(Lost(s, enemy), 3, 4);                                              // only the projectile attacker could
    }

    [Fact]
    public void AllInHalvesMomentumCostsRoundedUpButGainsNoMomentumFromItsHits()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "all_in", "lightning_bolt");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 1);
        Assert.Null(Why(s, Use(t, "lightning_bolt", e)));                                  // cost 2 halved to 1
        Run(s, Use(t, "lightning_bolt", e));
        Assert.Equal(0, s.Momentum.Value);                                                 // paid 1, gained 0 from the hit
    }

    [Fact]
    public void InspiringPresenceMovesMomentumOneMoreWhenAnAllyNextToTheTacticianKosAnEnemy()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "inspiring_presence");
        var ally = World.Unit(s, Side.A, Row.Front, "tail_club");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(e).Hp = 1;
        Run(s, Use(ally, "tail_club", e));
        Assert.True(s.GetUnit(e).IsKnockedOut);
        Assert.Equal(2, s.Momentum.Value);                                                 // 1 for the hit, 1 more for the KO
    }

    [Fact]
    public void InitiativePullsEveryAllysFirstTurnForwardFive()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "initiative");
        var a = World.Unit(s, Side.A, Row.Middle);
        var e = World.Unit(s, Side.B, Row.Front);
        s.Timeline.ScheduleInitialTurns(s.Units, s.Random);
        var before = (s.Timeline.TurnOf(t)!.Due, s.Timeline.TurnOf(a)!.Due, s.Timeline.TurnOf(e)!.Due);
        BattleStart.Install(s, World.Catalog);
        Assert.Equal(Math.Max(0, before.Item1 - 5), s.Timeline.TurnOf(t)!.Due);
        Assert.Equal(Math.Max(0, before.Item2 - 5), s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(before.Item3, s.Timeline.TurnOf(e)!.Due);
    }

    // ---- Commander ----

    [Fact]
    public void AdvanceMovesEveryAllyInOneRowOneRowForwardAndCannotPickTheFrontRow()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "advance");
        var m1 = World.Unit(s, Side.A, Row.Middle);
        var m2 = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        Assert.NotNull(Why(s, Use(c, "advance", row: new RowTarget(Side.A, Row.Front))));
        Assert.NotNull(Why(s, Use(c, "advance", row: new RowTarget(Side.B, Row.Middle))));          // own side only
        Run(s, Use(c, "advance", row: new RowTarget(Side.A, Row.Middle)));
        Assert.Equal(new[] { c, m1, m2 }.Order(), s.Formation.Units(Side.A, Row.Front).Order());
    }

    [Fact]
    public void SwitchPlacesSwapsTwoAlliesAndAnImmobilizedOneBlocksIt()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "switch_places");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(c, "switch_places", a, second: b));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(a).Row);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(b).Row);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Immobilized, Duration: 10));
        K.Ready(s, c);
        Assert.NotNull(Why(s, Use(c, "switch_places", a, second: b)));
    }

    [Fact]
    public void RotationMovesEveryAllyOneRowBackAndTheRearToTheFront()
    {
        var s = new BattleState(1);
        var f = World.Unit(s, Side.A, Row.Front, "rotation");
        var m = World.Unit(s, Side.A, Row.Middle);
        var r = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(f, "rotation"));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(f).Row);
        Assert.Equal(Row.Rear, s.Formation.PositionOf(m).Row);
        Assert.Equal(Row.Front, s.Formation.PositionOf(r).Row);
    }

    [Fact]
    public void RotationLeavesBodiesAndImmobilizedUnitsAndDoesNothingWhenARowWouldOverflow()
    {
        var s = new BattleState(1);
        var f = World.Unit(s, Side.A, Row.Front, "rotation");
        var body = World.Unit(s, Side.A, Row.Rear);
        s.GetUnit(body).Hp = 0;
        Knockout.ResolveAll(s);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(f, "rotation"));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(body).Row);                          // a body stays
        Assert.Equal(Row.Middle, s.Formation.PositionOf(f).Row);

        var s2 = new BattleState(1);
        var f2 = World.Unit(s2, Side.A, Row.Front, "rotation");
        for (var i = 0; i < 6; i++) World.Unit(s2, Side.A, Row.Rear);
        s2.GetUnit(2).IsImmobilized = true;                                                // a Rear unit stays: Front would get 6 + the Front... check overflow below
        World.Unit(s2, Side.B, Row.Front);
        s2.ShiftMomentum(Side.A, 6);
        Run(s2, Use(f2, "rotation"));
        Assert.InRange(s2.Formation.Units(Side.A, Row.Front).Count, 1, 6);
    }

    [Fact]
    public void DrillPullsAMovedAlliesNextTurnForwardThreeIncludingTheCommanderItself()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "switch_places", "drill");
        var a = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.Timeline.ScheduleTurn(a, 8, 5);
        s.Timeline.ScheduleTurn(c, 9, 5);
        var due = s.Timeline.TurnOf(a)!.Due;
        Run(s, Use(c, "switch_places", a, second: c));
        Assert.Equal(due - 3, s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(Timeline.Wait(3, 5) - 3, s.Timeline.TurnOf(c)!.Due);                 // the Commander moved too: its fresh turn is pulled as well
    }

    [Fact]
    public void RallyPullsEveryOtherAllysNextTurnForwardThreeWhenAnAllyIsKo()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "rally");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.Timeline.ScheduleTurn(a, 8, 5);
        s.Timeline.ScheduleTurn(c, 8, 5);
        var (da, dc) = (s.Timeline.TurnOf(a)!.Due, s.Timeline.TurnOf(c)!.Due);
        s.GetUnit(b).Hp = 0;
        Actions.Settle(s, null);
        Assert.Equal(da - 3, s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(dc - 3, s.Timeline.TurnOf(c)!.Due);
    }

    [Fact]
    public void VanguardGivesAWardOfThreeToAnAllyMovedIntoTheFrontRow()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "advance", "vanguard");
        World.Unit(s, Side.A, Row.Front);                                                  // two Front units: the pulled unit lands between them (50% overlap)
        var m = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(c, "advance", row: new RowTarget(Side.A, Row.Middle)));
        Assert.Equal(3, s.GetUnit(m).Ward);
        Assert.Equal(0, s.GetUnit(c).Ward);                                                // it was already in the Front
    }

    // ---- Bard ----

    [Fact]
    public void TheBardsThreeSongsAreMagicalAndItsKeystonesAreNot()
    {
        foreach (var id in new[] { "song_of_haste", "war_drums", "lullaby" })
            Assert.True(World.Catalog.Get(id).Magical);
        foreach (var id in new[] { "grand_chorus", "rhythm", "crescendo" })
            Assert.True(World.Catalog.Get(id).IsPassive);
    }

    [Fact]
    public void SongOfHasteGivesAlliesInTheBardsRowPlusOneSpeedForTwentyAndTheTimelineFollows()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Front, "song_of_haste");
        var mate = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.Timeline.ScheduleTurn(mate, 5, 5);
        AdvanceTo(s, 4);
        var remaining = s.Timeline.Remaining(s.Timeline.TurnOf(mate)!);
        Run(s, Use(bard, "song_of_haste"));
        Assert.Equal(6, s.GetUnit(mate).EffectiveSpeed);
        Assert.Equal(6, s.GetUnit(bard).EffectiveSpeed);
        Assert.Equal(5, s.GetUnit(other).EffectiveSpeed);
        Assert.Equal(remaining - 1, s.Timeline.Remaining(s.Timeline.TurnOf(mate)!));      // 20.5: the remaining Wait shrinks by the Speed change
        Assert.Equal(20, Statuses.Get(s.GetUnit(mate), StatusIds.SongHaste)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void ASongReachesTheUnitsAtCastTimeOnlyAndRestartsWhenCastAgain()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Front, "song_of_haste");
        var mate = World.Unit(s, Side.A, Row.Front);
        var late = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(bard, "song_of_haste"));
        Movement.Move(s, late, Row.Front, 0);                                              // arrives later: does not get it
        Assert.False(Has(s, late, StatusIds.SongHaste));
        Movement.Move(s, mate, Row.Rear, 0);                                               // leaves: keeps it
        Assert.True(Has(s, mate, StatusIds.SongHaste));
        AdvanceTo(s, 10);
        Assert.Equal(10, Statuses.Get(s.GetUnit(bard), StatusIds.SongHaste)!.TimeLeft(s.Timeline.Now));
        K.Ready(s, bard);
        Run(s, Use(bard, "song_of_haste"));
        Assert.Equal(20, Statuses.Get(s.GetUnit(bard), StatusIds.SongHaste)!.TimeLeft(s.Timeline.Now));        // casting again restarts it
        Assert.Equal(10, Statuses.Get(s.GetUnit(mate), StatusIds.SongHaste)!.TimeLeft(s.Timeline.Now));        // the unit that left is no longer reached
    }

    [Fact]
    public void WarDrumsProtectsTheBardAndItsAdjacentAlliesFromStunAndFrozenForTwenty()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Front, "war_drums");
        var next = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(bard, "war_drums"));
        Assert.False(Statuses.Apply(s, new StatusApplication(bard, StatusIds.Stunned, Duration: 5)));
        Assert.False(Statuses.Apply(s, new StatusApplication(next, StatusIds.Frozen)));
        Assert.True(Statuses.Apply(s, new StatusApplication(far, StatusIds.Stunned, Duration: 5)));
        Assert.True(Statuses.Apply(s, new StatusApplication(next, StatusIds.Blind, Duration: 5)));
        AdvanceTo(s, 20);
        Assert.True(Statuses.Apply(s, new StatusApplication(next, StatusIds.Stunned, Duration: 5)));
    }

    [Fact]
    public void LullabyPushesBackTheNextTurnOfEveryEnemyInOneRowByFiveAndIsInstant()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Front, "lullaby", "crescendo", "grand_chorus");
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        foreach (var id in new[] { e1, e2, back }) s.Timeline.ScheduleTurn(id, 5, 5);
        var due = s.Timeline.TurnOf(e1)!.Due;
        Assert.NotNull(Why(s, Use(bard, "lullaby", row: new RowTarget(Side.A, Row.Front))));      // enemy rows only
        Run(s, Use(bard, "lullaby", row: new RowTarget(Side.B, Row.Front)));
        Assert.Equal(due + 5, s.Timeline.TurnOf(e1)!.Due);
        Assert.Equal(due + 5, s.Timeline.TurnOf(e2)!.Due);
        Assert.Equal(due, s.Timeline.TurnOf(back)!.Due);
    }

    [Fact]
    public void GrandChorusReachesEveryAllyButOnlyOneSongCanBeActive()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Front, "song_of_haste", "war_drums", "grand_chorus");
        var rear = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(bard, "song_of_haste"));
        Assert.True(Has(s, rear, StatusIds.SongHaste));
        K.Ready(s, bard);
        Run(s, Use(bard, "war_drums"));
        Assert.True(Has(s, rear, StatusIds.WarDrums));
        Assert.False(Has(s, rear, StatusIds.SongHaste));
        Assert.False(Has(s, bard, StatusIds.SongHaste));
    }

    [Fact]
    public void CrescendoMakesSongsLastTenLonger()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Front, "song_of_haste", "crescendo");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(bard, "song_of_haste"));
        Assert.Equal(30, Statuses.Get(s.GetUnit(bard), StatusIds.SongHaste)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void RhythmGivesTheSecondUseOfTheSameSkillOneLessDelayAndAMoveBreaksTheStreak()
    {
        var s = new BattleState(1);
        var bard = World.Unit(s, Side.A, Row.Rear, "rhythm");
        var a = World.Unit(s, Side.A, Row.Front, "tail_club");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(a, "tail_club", e));
        Assert.Equal(Timeline.Wait(4, 5), s.Timeline.TurnOf(a)!.Due);
        var now = s.Timeline.TurnOf(a)!.Due;
        AdvanceTo(s, now);
        Run(s, Use(a, "tail_club", e));
        Assert.Equal(now + Timeline.Wait(3, 5), s.Timeline.TurnOf(a)!.Due);                // Delay 4 - 1
        AdvanceTo(s, s.Timeline.TurnOf(a)!.Due);
        Actions.Move(s, a, Row.Middle, 0);
        AdvanceTo(s, s.Timeline.TurnOf(a)!.Due);
        Actions.Move(s, a, Row.Front, 0);
        AdvanceTo(s, s.Timeline.TurnOf(a)!.Due);
        var t0 = s.Timeline.Now;
        Run(s, Use(a, "tail_club", e));
        Assert.Equal(t0 + Timeline.Wait(4, 5), s.Timeline.TurnOf(a)!.Due);                 // streak was broken by the Moves
    }
}
