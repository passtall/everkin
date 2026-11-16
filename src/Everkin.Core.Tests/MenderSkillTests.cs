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
        var (s, m, ally) = Board(1, "mend", "searing_light");
        Statuses.Apply(s, new StatusApplication(m, StatusIds.Silenced, Duration: 20));
        Assert.Contains("Silenced", Why(s, Use(m, "mend", ally)));
        Assert.Contains("Silenced", Why(s, Use(m, "searing_light", 2)));
    }

    [Fact]
    public void RainOfMendingHealsTheRowAndHalfThatInTheRowBehind()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Front, "rain_of_mending");
        var mid1 = World.Unit(s, Side.A, Row.Middle);
        var mid2 = World.Unit(s, Side.A, Row.Middle);
        var rear = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        foreach (var id in new[] { m, mid1, mid2, rear }) s.GetUnit(id).Hp = 10;
        Run(s, Use(m, "rain_of_mending", row: new RowTarget(Side.A, Row.Middle)));
        Assert.InRange(s.GetUnit(mid1).Hp - 10, 3, 4);
        Assert.InRange(s.GetUnit(mid2).Hp - 10, 3, 4);
        Assert.InRange(s.GetUnit(rear).Hp - 10, 1, 2);
        Assert.Equal(10, s.GetUnit(m).Hp);
        Assert.Equal(4, s.Momentum.Value);
    }

    [Fact]
    public void RainOfMendingCanPickAnyRowOfEitherSideEvenAnEmptyOne()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Front, "rain_of_mending");
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Assert.Null(Why(s, Use(m, "rain_of_mending", row: new RowTarget(Side.B, Row.Rear))));
        Run(s, Use(m, "rain_of_mending", row: new RowTarget(Side.B, Row.Rear)));
        Assert.Equal(4, s.Momentum.Value);                                                // cost paid, nothing healed
    }

    [Fact]
    public void SearingLightHitsForFourToFiveAndUndeadTakeDouble()
    {
        var (s, m, ally) = Board(1, "searing_light");
        var enemy = s.Formation.Units(Side.B, Row.Front)[0];
        Run(s, Use(m, "searing_light", enemy));
        Assert.InRange(Lost(s, enemy), 4, 5);

        var (s2, m2, _) = Board(1, "searing_light");
        var undead = s2.Formation.Units(Side.B, Row.Front)[0];
        s2.GetUnit(undead).IsUndead = true;
        Run(s2, Use(m2, "searing_light", undead));
        Assert.InRange(Lost(s2, undead), 8, 10);
    }

    [Fact]
    public void SearingLightIsADirectAttackSoItIsNeverIntercepted()
    {
        var (s, m, _) = Board(1, "searing_light");
        var front = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        for (ulong seed = 1; seed <= 20; seed++)
        {
            var s2 = s.Clone();
            Assert.DoesNotContain(Actions.Use(s2, World.Catalog, Use(m, "searing_light", back)), e => e is Intercepted);
        }
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
        Run(s, Use(m, "mend", target));
        Assert.Equal(40, s.GetUnit(target).Hp);
        Assert.InRange(s.GetUnit(hurt).Hp, 37, 39);                                        // 8-10 minus 1 healed on the target
        Assert.Equal(20, s.GetUnit(hurtMore).Hp);
    }

    [Fact]
    public void RegrowthGivesTwoHpEveryTenTimeUnitsForTwentyToAHealedUnit()
    {
        var (s, m, ally) = Board(1, "mend", "regrowth");
        s.GetUnit(ally).Hp = 1;
        Run(s, Use(m, "mend", ally));
        var hp = s.GetUnit(ally).Hp;
        var status = Statuses.Get(s.GetUnit(ally), StatusIds.Regrowth)!;
        Assert.Equal(20, status.TimeLeft(s.Timeline.Now));
        Assert.Equal(2, status.Magnitude);
        Assert.Equal(Polarity.Helpful, StatusRules.PolarityOf(StatusIds.Regrowth));
        s.Timeline.Add(TimelineKind.DelayedEvent, 10, tag: "t1");
        s.Timeline.Add(TimelineKind.UnitTurn, 10, ally);
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(hp + 2, s.GetUnit(ally).Hp);
        AdvanceTo(s, 20);
        Assert.False(Has(s, ally, StatusIds.Regrowth));
        Assert.Equal(hp + 4, s.GetUnit(ally).Hp);                                          // whole points land at the end
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
