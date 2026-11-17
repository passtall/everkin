using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class KeeperSkillTests
{
    [Fact]
    public void AllKeeperSkillsAreImplemented() => AssertClassImplemented("keeper", magical: false);

    [Fact]
    public void WardGivesAWardOfFourScaledToPowerUntilTheKeepersNextTurn()
    {
        var (s, k, e) = Board(1, "ward");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Run(s, Use(k, "ward", ally));
        Assert.Equal(4, s.GetUnit(ally).Ward);
        Assert.Equal(k, Statuses.Get(s.GetUnit(ally), StatusIds.Ward)!.EndsAtNextTurnOf);
        s.GetUnit(k).Power = 7;                                                           // 4 x 7 / 5 = 5 (section 20.5)
        K.Ready(s, k);
        Run(s, Use(k, "ward", k));
        Assert.Equal(5, s.GetUnit(k).Ward);
    }

    [Fact]
    public void WardWorksFromAnyRowOnAnyUnitAndEndsWhenTheKeepersTurnComes()
    {
        var (s, k, e) = Board(1, "ward");
        s.Formation.Remove(k);
        s.Formation.Place(k, Side.A, Row.Middle);
        var rear = World.Unit(s, Side.A, Row.Rear);
        Run(s, Use(k, "ward", rear));                                                     // a Middle Keeper warding a Rear ally works
        Assert.Equal(4, s.GetUnit(rear).Ward);
        Assert.Equal(k, BattleDriver.NextTurn(s));
        Assert.Equal(0, s.GetUnit(rear).Ward);
    }

    [Fact]
    public void ANewWardReplacesTheOldOneOnlyIfItAbsorbsAtLeastAsMuch()
    {
        var (s, k, e) = Board(1, "ward", "shield_dome");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Ward, Magnitude: 3, SourceUnitId: 99));
        Run(s, Use(k, "shield_dome"));                                                    // Ward 2 against 3 left
        Assert.Equal(3, s.GetUnit(ally).Ward);
        K.Ready(s, k);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(k, "ward", ally));                                                     // Ward 4 replaces Ward 3
        Assert.Equal(4, s.GetUnit(ally).Ward);
    }

    [Fact]
    public void ShieldDomeGivesEveryLivingAllyAWardOfTwo()
    {
        var (s, k, e) = Board(1, "shield_dome");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        var body = World.Unit(s, Side.A, Row.Rear);
        s.GetUnit(body).Hp = 0;
        Knockout.ResolveAll(s);
        Run(s, Use(k, "shield_dome"));
        Assert.Equal(2, s.GetUnit(k).Ward);
        Assert.Equal(2, s.GetUnit(ally).Ward);
        Assert.Equal(0, s.GetUnit(e).Ward);
        Assert.Equal(0, s.GetUnit(body).Ward);
        Assert.Equal(3, s.Momentum.Value);  // M5.10 costs
    }

    [Fact]
    public void WardBurstBreaksAWardAndHitsEveryEnemyInMeleeReachForItsRemainingValue()
    {
        var (s, k, e1) = Board(1, "ward_burst");
        var e2 = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        var ally = World.Unit(s, Side.A, Row.Rear);                                       // the warded unit may stand anywhere
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Ward, Magnitude: 3, SourceUnitId: k));
        Run(s, Use(k, "ward_burst", ally));
        Assert.Equal(0, s.GetUnit(ally).Ward);
        Assert.False(Has(s, ally, StatusIds.Ward));
        Assert.Equal(3, Lost(s, e1));
        Assert.Equal(3, Lost(s, e2));
        Assert.Equal(0, Lost(s, back));
        Assert.Equal(6, s.Momentum.Value);  // M5.10 costs
    }

    [Fact]
    public void WardBurstIsNotScaledAgainAndNeedsAWardAndTheFrontRow()
    {
        var (s, k, e) = Board(1, "ward_burst");
        s.GetUnit(k).Power = 10;
        Assert.NotNull(Why(s, Use(k, "ward_burst", k)));                                  // no Ward
        s.GetUnit(k).Ward = 8;
        Run(s, Use(k, "ward_burst", k));
        Assert.Equal(8, Lost(s, e));                                                      // not scaled to Power 10 again

        var (s2, k2, e2) = Board(1, "ward_burst");
        s2.GetUnit(k2).Ward = 4;
        s2.Formation.Remove(k2);
        s2.Formation.Place(k2, Side.A, Row.Middle);
        Assert.Equal("needs the Front row", Why(s2, Use(k2, "ward_burst", k2)));
    }

    [Fact]
    public void WardBurstWithNobodyInReachStillBreaksTheWardAndSpendsTheTurn()
    {
        var (s, k, e) = Board(1, "ward_burst");
        s.Formation.Remove(e);
        s.Formation.Place(e, Side.B, Row.Middle);
        s.GetUnit(k).Ward = 5;
        Run(s, Use(k, "ward_burst", k));
        Assert.Equal(0, s.GetUnit(k).Ward);
        Assert.Equal(0, Lost(s, e));
        Assert.Equal(5, s.Momentum.Value);
    }

    [Fact]
    public void ThornedWardHurtsTheAttackerOfAWardFromTheKeeperForThree()
    {
        var s = new BattleState(1);
        var k = World.Unit(s, Side.A, Row.Rear, "ward", "thorned_ward");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front, "tail_club", "rivet_shot");
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(k, "ward", ally));
        Run(s, Use(enemy, "rivet_shot", ally));
        Assert.Equal(3, Lost(s, enemy));
        Assert.Equal(0, Lost(s, ally));                                                   // the Ward of 4 took the whole hit
    }

    [Fact]
    public void ThornedWardOnlyReactsToAWardFromThatKeeper()
    {
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Rear, "ward", "thorned_ward");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front, "tail_club", "rivet_shot");
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Ward, Magnitude: 10, SourceUnitId: 99));
        Run(s, Use(enemy, "rivet_shot", ally));
        Assert.Equal(0, Lost(s, enemy));                                                  // someone else's Ward
    }

    [Fact]
    public void TwinWardsGivesTheSameWardToOneAllyNextToTheTarget()
    {
        var s = new BattleState(1);
        var k = World.Unit(s, Side.A, Row.Rear, "ward", "twin_wards");
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(k, "ward", a));
        Assert.Equal(4, s.GetUnit(a).Ward);
        Assert.Equal(4, s.GetUnit(b).Ward);                                               // the only ally next to a
        Assert.Equal(0, s.GetUnit(k).Ward);
    }

    [Fact]
    public void TwinWardsPicksOneAllyBySeededRollAndFindsNoAllyNextToAnEnemy()
    {
        var picks = new int[2];
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = new BattleState(seed);
            var k = World.Unit(s, Side.A, Row.Rear, "ward", "twin_wards");
            var left = World.Unit(s, Side.A, Row.Front);
            var mid = World.Unit(s, Side.A, Row.Front);
            var right = World.Unit(s, Side.A, Row.Front);
            World.Unit(s, Side.B, Row.Front);
            BattleStart.Install(s, World.Catalog);
            Run(s, Use(k, "ward", mid));
            Assert.Equal(1, new[] { left, right }.Count(id => s.GetUnit(id).Ward > 0));
            picks[s.GetUnit(left).Ward > 0 ? 0 : 1]++;
        }
        Assert.All(picks, p => Assert.InRange(p, 60, 140));

        var s2 = new BattleState(1);
        var k2 = World.Unit(s2, Side.A, Row.Rear, "ward", "twin_wards");
        var e1 = World.Unit(s2, Side.B, Row.Front);
        var e2 = World.Unit(s2, Side.B, Row.Front);
        BattleStart.Install(s2, World.Catalog);
        Run(s2, Use(k2, "ward", e1));                                                      // next to an enemy: no ally there
        Assert.Equal(4, s2.GetUnit(e1).Ward);
        Assert.Equal(0, s2.GetUnit(e2).Ward);
    }

    [Fact]
    public void SteadfastWardStopsStunAndEnemyMovesWhileTheWardLastsOnly()
    {
        var s = new BattleState(1);
        var k = World.Unit(s, Side.A, Row.Front, "ward", "steadfast_ward");
        var ally = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Assert.True(Statuses.Apply(s, new StatusApplication(ally, StatusIds.Stunned, Duration: 5)));
        Statuses.Remove(s, ally, StatusIds.Stunned, StatusEndCause.Cleansed);
        Run(s, Use(k, "ward", ally));
        Assert.False(Statuses.Apply(s, new StatusApplication(ally, StatusIds.Stunned, Duration: 5)));
        Assert.Empty(Movement.ForceMove(s, ally, Row.Rear, Side.B).Moves);
        Assert.NotEmpty(Movement.ForceMove(s, ally, Row.Rear, Side.A).Moves);              // its own side can still move it
        s.GetUnit(ally).Ward = 0;
        Statuses.Remove(s, ally, StatusIds.Ward, StatusEndCause.Broken);
        Assert.True(Statuses.Apply(s, new StatusApplication(ally, StatusIds.Stunned, Duration: 5)));
    }

    // ---- 20.5 rows owned by this class ----

    [Fact]
    public void AKeeperInTheMiddleRowUsesWardOnAnAllyInTheRearAndCannotUseWardBurst()
    {
        var s = new BattleState(1);
        var k = World.Unit(s, Side.A, Row.Middle, "ward", "ward_burst");
        var rear = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        Assert.Null(Why(s, Use(k, "ward", rear)));
        s.GetUnit(rear).Ward = 3;
        Assert.Equal("needs the Front row", Why(s, Use(k, "ward_burst", rear)));
    }

    [Fact]
    public void WardsAbsorbDamageFirstAndOnlyTheRemainderReachesHp()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front);
        Assert.Equal(9, s.DealDamage(null, a, 9).HpLost);
        s.GetUnit(a).Ward = 8;
        var r = s.DealDamage(null, a, 9);
        Assert.Equal((8, 1), (r.AbsorbedByWard, r.HpLost));
        s.GetUnit(a).Ward = 8;
        var r2 = s.DealDamage(null, a, 7);
        Assert.Equal((7, 0, 1), (r2.AbsorbedByWard, r2.HpLost, s.GetUnit(a).Ward));
    }
}
