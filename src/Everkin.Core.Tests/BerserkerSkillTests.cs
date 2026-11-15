using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class BerserkerSkillTests
{
    [Fact]
    public void AllBerserkerSkillsAreImplemented() => AssertClassImplemented("berserker", magical: false);

    [Fact]
    public void RampageHitsForTenToTwelveAndTheBerserkerTakesThree()
    {
        var dealt = new HashSet<int>();
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var (s, u, e) = Board(seed, "rampage");
            Run(s, Use(u, "rampage", e));
            dealt.Add(Lost(s, e));
            Assert.Equal(3, Lost(s, u));
        }
        Assert.Equal(new[] { 10, 11, 12 }, dealt.OrderBy(x => x));
    }

    [Fact]
    public void RampagesRecoilIsScaledAndAWardAbsorbsIt()
    {
        var (s, u, e) = Board(1, "rampage");
        s.GetUnit(u).Power = 10;
        s.GetUnit(u).Ward = 4;
        Run(s, Use(u, "rampage", e));
        Assert.Equal(2, Lost(s, u));                                        // 3 x 10 / 5 = 6, the Ward took 4
        Assert.InRange(Lost(s, e), 20, 24);
    }

    [Fact]
    public void RampageNeedsTheFrontRow()
    {
        var (s, u, e) = Board(1, "rampage");
        s.Formation.Remove(u);
        s.Formation.Place(u, Side.A, Row.Middle);
        Assert.Equal("needs the Front row", Why(s, Use(u, "rampage", e)));
    }

    [Fact]
    public void SweepingKickPushesEveryEnemyInMeleeReachOneRowBackAsAGroup()
    {
        var (s, u, e1) = Board(1, "sweeping_kick");
        var e2 = World.Unit(s, Side.B, Row.Front);
        var e3 = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(u, "sweeping_kick"));
        Assert.All(new[] { e1, e2, e3 }, id => Assert.Equal(Row.Middle, s.Formation.PositionOf(id).Row));
        Assert.Equal(new[] { e1, e2, e3 }, s.Formation.Units(Side.B, Row.Middle));      // side by side stays side by side
    }

    [Fact]
    public void SweepingKickCannotBeInterceptedAndMeleeCannotReachAnAirborneUnit()
    {
        var (s, u, e1) = Board(1, "sweeping_kick");
        var flyer = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(flyer).IsAirborne = true;
        var events = Run(s, Use(u, "sweeping_kick"));
        Assert.DoesNotContain(events, e => e is Intercepted);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(e1).Row);
        Assert.Equal(Row.Front, s.Formation.PositionOf(flyer).Row);
    }

    [Fact]
    public void SweepingKickWithAnEmptyEnemyFrontRowIsStillUsedAndSpendsItsDelayAndCost()
    {
        var (s, u, e) = Board(1, "sweeping_kick");
        s.Formation.Remove(e);
        s.Formation.Place(e, Side.B, Row.Middle);
        Assert.Null(Why(s, Use(u, "sweeping_kick")));
        Run(s, Use(u, "sweeping_kick"));
        Assert.Equal(4, s.Momentum.Value);                                   // cost 2 from +6
        Assert.Equal(Row.Middle, s.Formation.PositionOf(e).Row);
    }

    [Fact]
    public void BattleRoarPushesBackTheNextTurnOfEveryEnemyInMeleeReachByFive()
    {
        var (s, u, e1) = Board(1, "battle_roar");
        var e2 = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        foreach (var id in new[] { e1, e2, back }) s.Timeline.ScheduleTurn(id, 5, 5);
        var due = s.Timeline.TurnOf(e1)!.Due;
        Run(s, Use(u, "battle_roar"));
        Assert.Equal(due + 5, s.Timeline.TurnOf(e1)!.Due);
        Assert.Equal(due + 5, s.Timeline.TurnOf(e2)!.Due);
        Assert.Equal(due, s.Timeline.TurnOf(back)!.Due);
    }

    // ---- keystones ----

    [Fact]
    public void RecklessSwingDoublesHitsAndTheBerserkerTakesHalfOfWhatWasLost()
    {
        var (s, u, e) = Board(1, "tail_club", "reckless_swing");
        Run(s, Use(u, "tail_club", e));
        var dealt = Lost(s, e);
        Assert.InRange(dealt, 6, 8);                                         // 3-4 doubled
        Assert.Equal(dealt / 2, Lost(s, u));
    }

    [Fact]
    public void RecklessSwingsRecoilIsUnreducibleAndCanKoTheBerserker()
    {
        var (s, u, e) = Board(1, "tail_club", "reckless_swing");
        s.GetUnit(u).Hp = 1;
        s.GetUnit(u).Ward = 5;
        Run(s, Use(u, "tail_club", e));
        Assert.True(s.GetUnit(u).IsKnockedOut);
    }

    [Fact]
    public void LastStandAddsFiftyPercentAndTheBonusesAddUp()
    {
        var (s, u, e) = Board(1, "tail_club", "last_stand");
        Run(s, Use(u, "tail_club", e));
        Assert.InRange(Lost(s, e), 4, 6);                                    // 3-4 x 150%, rounded down

        var (s2, u2, e2) = Board(1, "tail_club", "reckless_swing", "last_stand");
        Run(s2, Use(u2, "tail_club", e2));
        Assert.InRange(Lost(s2, e2), 7, 10);                                 // 3-4 x 250%
    }

    [Fact]
    public void LastStandBlocksMoveAndEveryPushPullOrSwapOfTheBerserker()
    {
        var (s, u, e) = Board(1, "last_stand");
        Assert.Empty(Movement.LegalMoves(s, u));
        var result = Movement.ForceMove(s, u, Row.Middle, Side.B);
        Assert.Empty(result.Moves);
        Assert.Equal(Row.Front, s.Formation.PositionOf(u).Row);
        Assert.False(Movement.Swap(s, u, s.Formation.Units(Side.A, Row.Middle)[0], Side.B));
    }

    [Fact]
    public void UnstoppableOnlyWorksBelowAQuarterOfTheHp()
    {
        var (s, u, _) = Board(1, "unstoppable");
        s.Timeline.ScheduleTurn(u, 5, 5);
        Assert.True(Statuses.Apply(s, new StatusApplication(u, StatusIds.Frozen)));
        Statuses.Remove(s, u, StatusIds.Frozen, StatusEndCause.Cleansed);
        Assert.True(s.PushTurnBack(u, 3));

        s.GetUnit(u).Hp = 9;                                                 // below a quarter of 40
        Assert.False(Statuses.Apply(s, new StatusApplication(u, StatusIds.Frozen)));
        Assert.False(Statuses.Apply(s, new StatusApplication(u, StatusIds.Immobilized, Duration: 10)));
        Assert.False(s.PushTurnBack(u, 3));
        Assert.True(Statuses.Apply(s, new StatusApplication(u, StatusIds.Stunned, Duration: 5)));      // only Immobilized and Frozen
    }

    [Fact]
    public void UnstoppablesAtExactlyAQuarterDoesNotYetApply()
    {
        var (s, u, _) = Board(1, "unstoppable");
        s.GetUnit(u).Hp = 10;
        Assert.True(Statuses.Apply(s, new StatusApplication(u, StatusIds.Immobilized, Duration: 10)));
    }
}
