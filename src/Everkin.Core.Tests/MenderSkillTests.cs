using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class MenderSkillTests
{
    [Fact]
    public void AllMenderSkillsAreImplemented() => AssertClassImplemented("mender", magical: true);

    private static (BattleState S, int Mender, int Ally) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var m = World.Unit(s, Side.A, Row.Front, skills);
        var ally = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(ally).Hp = 10;
        return (s, m, ally);
    }

    [Fact]
    public void MendHealsEightToTenAtPowerFiveAndFourToSixAtPowerThree()
    {
        var healed = new HashSet<int>();
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var (s, m, ally) = Board(seed, "mend");
            Run(s, Use(m, "mend", ally));
            healed.Add(s.GetUnit(ally).Hp - 10);
        }
        Assert.Equal(new[] { 8, 9, 10 }, healed.OrderBy(x => x));

        var low = new HashSet<int>();
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var (s, m, ally) = Board(seed, "mend");
            s.GetUnit(m).Power = 3;                                                       // 20.5: 8 x 3 / 5 = 4, 10 x 3 / 5 = 6
            Run(s, Use(m, "mend", ally));
            low.Add(s.GetUnit(ally).Hp - 10);
        }
        Assert.Equal(new[] { 4, 5, 6 }, low.OrderBy(x => x));
    }

    [Fact]
    public void HealingIsCappedAtMaximumHp()
    {
        var (s, m, ally) = Board(1, "mend");
        s.GetUnit(ally).Hp = 38;
        Run(s, Use(m, "mend", ally));
        Assert.Equal(40, s.GetUnit(ally).Hp);
    }

    [Fact]
    public void MendIsMagicalSoSilencedBlocksIt()
    {
        var (s, m, ally) = Board(1, "mend", "sun_burst");
        Statuses.Apply(s, new StatusApplication(m, StatusIds.Silenced, Duration: 20));
        Assert.Contains("Silenced", Why(s, Use(m, "mend", ally)));
        Assert.Contains("Silenced", Why(s, Use(m, "sun_burst")));
    }

    [Fact]
    public void MendCostsNoMomentum()
    {
        var (s, m, ally) = Board(1, "mend");
        var before = s.Momentum.Value;
        Run(s, Use(m, "mend", ally));
        Assert.Equal(before, s.Momentum.Value);
    }

    [Fact]
    public void RainOfMendingHealsEveryAllyFourToFiveForTwoMomentum()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Front, "rain_of_mending");
        var mid = World.Unit(s, Side.A, Row.Middle);
        var rear = World.Unit(s, Side.A, Row.Rear);
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        foreach (var id in new[] { m, mid, rear, enemy }) s.GetUnit(id).Hp = 10;
        Run(s, Use(m, "rain_of_mending"));
        foreach (var id in new[] { m, mid, rear }) Assert.InRange(s.GetUnit(id).Hp - 10, 4, 5);
        Assert.Equal(10, s.GetUnit(enemy).Hp);                                            // enemies are not healed
        Assert.Equal(4, s.Momentum.Value);
    }

    [Fact]
    public void SunBurstHitsEveryEnemyTwoToThreeAndHealsEveryAllyTwoToThree()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Rear, "sun_burst");
        var ally = World.Unit(s, Side.A, Row.Front);
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Middle);
        var e3 = World.Unit(s, Side.B, Row.Rear);
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(m).Hp = 10;
        s.GetUnit(ally).Hp = 10;
        Assert.Null(Why(s, Use(m, "sun_burst")));                                          // works from the Rear row
        Run(s, Use(m, "sun_burst"));
        foreach (var e in new[] { e1, e2, e3 }) Assert.InRange(Lost(s, e), 2, 3);
        Assert.InRange(s.GetUnit(m).Hp - 10, 2, 3);
        Assert.InRange(s.GetUnit(ally).Hp - 10, 2, 3);
        Assert.Equal(5, s.Momentum.Value);                                                 // paid 2, then the landed hits shift 1 back
    }

    [Fact]
    public void SunBurstIsDirectSoItIsNeverIntercepted()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Front, "sun_burst");
        World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Rear);
        s.ShiftMomentum(Side.A, 6);
        Assert.DoesNotContain(Run(s, Use(m, "sun_burst")), e => e is Intercepted);
    }

    [Fact]
    public void OverflowingCareSendsTheExcessToTheAdjacentAllyWithTheLowestHp()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Rear, "mend", "overflowing_care");
        var target = World.Unit(s, Side.A, Row.Front);
        var hurt = World.Unit(s, Side.A, Row.Front);
        var hurtMore = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(target).Hp = 39;
        s.GetUnit(hurt).Hp = 30;
        s.GetUnit(hurtMore).Hp = 20;                                                       // not adjacent to the target (two places away)
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(m, "mend", target));
        Assert.Equal(40, s.GetUnit(target).Hp);
        Assert.InRange(s.GetUnit(hurt).Hp, 37, 39);                                        // 8-10 minus 1 healed on the target
        Assert.Equal(20, s.GetUnit(hurtMore).Hp);
    }

    [Fact]
    public void RegrowthGivesAHealedUnitAWardOfTwoUntilItsNextTurn()
    {
        var (s, m, ally) = Board(1, "mend", "regrowth");
        s.Timeline.ScheduleTurn(ally, 5, 5);
        Run(s, Use(m, "mend", ally));
        Assert.Equal(2, s.GetUnit(ally).Ward);
        Assert.False(Has(s, ally, StatusIds.Regrowth));                                    // the old heal-over-time status is gone
        var (s2, m2, ally2) = Board(1, "mend");                                            // without the keystone: no Ward
        Run(s2, Use(m2, "mend", ally2));
        Assert.Equal(0, s2.GetUnit(ally2).Ward);
    }

    [Fact]
    public void TriageMakesMendHealDoubleOnAUnitBelowAQuarterOfItsHp()
    {
        var (s, m, ally) = Board(1, "mend", "triage");
        s.GetUnit(ally).Hp = 9;                                                            // below 25% of 40
        Run(s, Use(m, "mend", ally));
        Assert.InRange(s.GetUnit(ally).Hp - 9, 16, 20);

        var (s2, m2, ally2) = Board(1, "mend", "triage");
        s2.GetUnit(ally2).Hp = 10;                                                         // exactly a quarter is not below
        Run(s2, Use(m2, "mend", ally2));
        Assert.InRange(s2.GetUnit(ally2).Hp - 10, 8, 10);
    }

    [Fact]
    public void AMendOnAnUndeadHurtsIt()
    {
        var (s, m, ally) = Board(1, "mend");
        s.GetUnit(ally).IsUndead = true;
        Run(s, Use(m, "mend", ally));
        Assert.InRange(10 - s.GetUnit(ally).Hp, 8, 10);
    }
}
