using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class TrackerSkillTests
{
    [Fact]
    public void AllTrackerSkillsAreImplemented() => AssertClassImplemented("tracker", magical: false);

    [Fact]
    public void TargetMarksAnEnemyAndMarkingAnotherMovesTheMark()
    {
        var (s, t, e1) = Board(1, "target");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(t, "target", e1));
        Assert.Equal(30, Statuses.Get(s.GetUnit(e1), StatusIds.Marked)!.TimeLeft(s.Timeline.Now));
        K.Ready(s, t);
        Run(s, Use(t, "target", e2));
        Assert.False(Has(s, e1, StatusIds.Marked));
        Assert.True(Has(s, e2, StatusIds.Marked));
        Assert.NotNull(Why(s, Use(t, "target", s.Formation.Units(Side.A, Row.Middle)[0])));   // only enemies
    }

    [Fact]
    public void ReadTracksRevealsEveryStealthedUnitOfAChosenRow()
    {
        var (s, t, e) = Board(1, "read_tracks");
        var hidden = World.Unit(s, Side.B, Row.Front);
        var hiddenBack = World.Unit(s, Side.B, Row.Middle);
        s.GetUnit(hidden).IsStealthed = true;
        s.GetUnit(hiddenBack).IsStealthed = true;
        Run(s, Use(t, "read_tracks", row: new RowTarget(Side.B, Row.Front)));
        Assert.False(s.GetUnit(hidden).IsStealthed);
        Assert.True(s.GetUnit(hiddenBack).IsStealthed);
    }

    [Fact]
    public void ReadTracksCanPickAnEmptyRowAndSpendsItsTurn()
    {
        var (s, t, _) = Board(1, "read_tracks");
        Assert.Null(Why(s, Use(t, "read_tracks", row: new RowTarget(Side.B, Row.Rear))));
        Run(s, Use(t, "read_tracks", row: new RowTarget(Side.B, Row.Rear)));
        Assert.Equal(Timeline.Wait(2, 5), s.Timeline.TurnOf(t)!.Due);
    }

    [Fact]
    public void HuntersShotHitsAMarkedUnitForNineToElevenAndNeedsAMark()
    {
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 80; seed++)
        {
            var (s, t, e) = Board(seed, "hunters_shot");
            Assert.NotNull(Why(s, Use(t, "hunters_shot", e)));
            Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: 99));     // whoever placed the Mark
            Assert.Null(Why(s, Use(t, "hunters_shot", e)));
            Run(s, Use(t, "hunters_shot", e));
            damages.Add(Lost(s, e));
        }
        Assert.Equal(new[] { 9, 10, 11 }, damages.OrderBy(x => x));
    }

    [Fact]
    public void HuntersShotPartlyInterceptedForNineSplitsFourAndFive()
    {
        // 20.5: an interceptor takes 4 of a partial 9 and the rest, 5, continues.
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s2 = new BattleState(seed);
            World.Unit(s2, Side.A, Row.Rear);
            World.Unit(s2, Side.B, Row.Front);
            var t2 = World.Unit(s2, Side.B, Row.Middle);
            var res = Interception.Resolve(s2, t2, 9, true, Delivery.Projectile, Side.A);
            if (res.Shares.Count == 1 && !res.Shares[0].Full)
            {
                Assert.Equal(4, res.Shares[0].Amount);
                Assert.Equal(5, res.Remaining);
                return;
            }
        }
        Assert.Fail("no partial interception in 200 seeds");
    }

    [Fact]
    public void SniffTheAirMarksTheEnemyWithTheHighestPowerAtBattleStart()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "sniff_the_air");
        var weak = World.Unit(s, Side.B, Row.Front);
        var strong = World.Unit(s, Side.B, Row.Middle);
        s.GetUnit(strong).Power = 9;
        BattleStart.Install(s, World.Catalog);
        Assert.False(Has(s, weak, StatusIds.Marked));
        Assert.True(Has(s, strong, StatusIds.Marked));
        Assert.Equal(t, Statuses.Get(s.GetUnit(strong), StatusIds.Marked)!.SourceUnitId);
    }

    [Fact]
    public void SniffTheAirBreaksTiesWithTheSeededRoll()
    {
        var counts = new int[2];
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = new BattleState(seed);
            World.Unit(s, Side.A, Row.Front, "sniff_the_air");
            var e1 = World.Unit(s, Side.B, Row.Front);
            var e2 = World.Unit(s, Side.B, Row.Front);
            BattleStart.Install(s, World.Catalog);
            counts[Has(s, e1, StatusIds.Marked) ? 0 : 1]++;
        }
        Assert.All(counts, c => Assert.InRange(c, 70, 130));
    }

    [Fact]
    public void ScentOfBloodMarksAnEnemyThatFallsBelowTwentyPercentAndMovesTheMark()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "scent_of_blood");
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.DealDamage(t, e1, 31);                                                            // 9 of 40 left: 22%, not yet
        Assert.False(Has(s, e1, StatusIds.Marked));
        s.DealDamage(t, e1, 2);                                                             // 7 of 40 is below 20%
        Assert.True(Has(s, e1, StatusIds.Marked));
        s.DealDamage(t, e2, 34);
        Assert.True(Has(s, e2, StatusIds.Marked));
        Assert.False(Has(s, e1, StatusIds.Marked));                                         // one Mark per Tracker
    }

    [Fact]
    public void ScentOfBloodAlsoReactsToStatusDamageButNotToAllies()
    {
        var s = new BattleState(1);
        var t = World.Unit(s, Side.A, Row.Front, "scent_of_blood");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.DealDamage(null, ally, 35);
        Assert.False(Has(s, ally, StatusIds.Marked));
        s.DealDamage(null, e, 33, DamageSource.Periodic);
        Assert.True(Has(s, e, StatusIds.Marked));
    }

    [Fact]
    public void RelentlessMarksHaveNoTimeLimitAndSurviveCleansing()
    {
        var (s, t, e) = Board(1, "target", "relentless");
        s.Formation.Units(Side.A, Row.Middle);
        Run(s, Use(t, "target", e));
        Assert.Null(Statuses.Get(s.GetUnit(e), StatusIds.Marked)!.TimeLeft(s.Timeline.Now));
        Assert.True(Statuses.RemoveAll(s, e, Polarity.Harmful, byUnitId: 99).Count == 0);
        Assert.True(Has(s, e, StatusIds.Marked));
        AdvanceTo(s, 100);
        Assert.True(Has(s, e, StatusIds.Marked));
        // ... but the Tracker's own next Mark moves it, and KO clears it.
        var e2 = World.Unit(s, Side.B, Row.Middle);
        K.Ready(s, t);
        Run(s, Use(t, "target", e2));
        Assert.False(Has(s, e, StatusIds.Marked));
        s.GetUnit(e2).Hp = 0;
        Knockout.ResolveAll(s);
        Assert.False(Has(s, e2, StatusIds.Marked));
    }
}
