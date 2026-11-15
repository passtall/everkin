using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class LifebinderSkillTests
{
    [Fact]
    public void AllLifebinderSkillsAreImplemented() => AssertClassImplemented("lifebinder", magical: true);

    private static (BattleState S, int Lifebinder, int Ally) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var h = World.Unit(s, Side.A, Row.Front, skills);
        var ally = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, h, ally);
    }

    private static void Ko(BattleState s, int unit)
    {
        s.GetUnit(unit).Hp = 0;
        Knockout.Resolve(s, unit);
        s.DrainEvents();
    }

    [Fact]
    public void BurnoutHealHealsTheLifebinderAndItsAdjacentAlliesForSixToEightThenTheLifebinderIsKo()
    {
        var s = new BattleState(1);
        var left = World.Unit(s, Side.A, Row.Front);
        var h = World.Unit(s, Side.A, Row.Front, "burnout_heal");
        var right = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        foreach (var id in new[] { left, right, far }) s.GetUnit(id).Hp = 10;
        s.ShiftMomentum(Side.A, 6);
        s.DrainEvents();
        var events = Run(s, Use(h, "burnout_heal"));
        Assert.InRange(s.GetUnit(left).Hp - 10, 6, 8);
        Assert.InRange(s.GetUnit(right).Hp - 10, 6, 8);
        Assert.Equal(10, s.GetUnit(far).Hp);
        Assert.True(s.GetUnit(h).IsKnockedOut);
        Assert.Contains(events, e => e is UnitKnockedOut { UnitId: var u } && u == h);
        Assert.Equal(0, s.Momentum.Value);                                              // cost 6
    }

    [Fact]
    public void TheLifebindersKoInBurnoutHealResolvesBeforeItsOwnReactionsCanFire()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "burnout_heal");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        var fired = false;
        s.AddReaction(new ReactionDef("when_hit", h, TriggerKind.WhenHit, (_, _) => fired = true));
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(h, "burnout_heal"));
        Assert.False(fired);
        Assert.True(s.GetUnit(h).IsKnockedOut);
    }

    [Fact]
    public void LifeForLifeGivesUpHalfTheCurrentHpToReviveAKoUnitWithThatMuchAndNoFreeTurn()
    {
        var (s, h, ally) = Board(1, "life_for_life");
        s.GetUnit(h).Hp = 30;
        Ko(s, ally);
        Run(s, Use(h, "life_for_life", ally));
        Assert.Equal(15, s.GetUnit(h).Hp);
        Assert.Equal(15, s.GetUnit(ally).Hp);
        Assert.False(s.GetUnit(ally).IsKnockedOut);
        Assert.Equal(Timeline.Wait(6, 5), s.Timeline.TurnOf(ally)!.Due);               // as its own action: Life for Life's Delay 6
    }

    [Fact]
    public void LifeForLifeNeedsAtLeastOneHpToGiveAndAKoBody()
    {
        var (s, h, ally) = Board(1, "life_for_life");
        Ko(s, ally);
        s.GetUnit(h).Hp = 1;
        Assert.NotNull(Why(s, Use(h, "life_for_life", ally)));                           // half of 1 is 0: nothing to give
        s.GetUnit(h).Hp = 2;
        Assert.Null(Why(s, Use(h, "life_for_life", ally)));
        Assert.NotNull(Why(s, Use(h, "life_for_life", 2)));                               // a living enemy is not a legal target
    }

    [Fact]
    public void LifeForLifeCanReviveAnEnemyBodyAndIgnoresTheLifebindersWard()
    {
        var (s, h, ally) = Board(1, "life_for_life");
        var enemyBody = World.Unit(s, Side.B, Row.Middle);
        Ko(s, enemyBody);
        s.GetUnit(h).Ward = 20;
        Run(s, Use(h, "life_for_life", enemyBody));
        Assert.Equal(20, s.GetUnit(h).Ward);                                              // given up unreducibly: the Ward is untouched
        Assert.Equal(20, s.GetUnit(enemyBody).Hp);
    }

    [Fact]
    public void PhoenixMakesLifeForLifeCostOnlyAQuarter()
    {
        var (s, h, ally) = Board(1, "life_for_life", "phoenix");
        s.GetUnit(h).Hp = 30;
        Ko(s, ally);
        Run(s, Use(h, "life_for_life", ally));
        Assert.Equal(23, s.GetUnit(h).Hp);
        Assert.Equal(7, s.GetUnit(ally).Hp);
    }

    [Fact]
    public void BoundResurrectSharesOnePoolOfSumMaxHpStartingAtLifebinderHpPlusRevivalHp()
    {
        // 20.5: reviver 30 HP of 40, revived unit max 20 with 25% revival HP: the pool maximum is 60 and starts at 30 + 5 = 35.
        var (s, h, ally) = Board(1, "bound_resurrect");
        s.GetUnit(h).Hp = 30;
        s.GetUnit(ally).MaxHp = 20;
        Ko(s, ally);
        Run(s, Use(h, "bound_resurrect", ally));
        var pool = s.SharedPoolOf(h)!;
        Assert.Equal(60, pool.Max);
        Assert.Equal(35, pool.Pool);
        Assert.Equal(35, s.GetUnit(h).Hp);
        Assert.Equal(20, s.GetUnit(ally).Hp);
        Assert.Equal(2, s.Momentum.Value);                                               // cost 4
    }

    [Fact]
    public void EveryHitOnEitherSharerReducesThePoolAndAtZeroBothAreKo()
    {
        var (s, h, ally) = Board(1, "bound_resurrect");
        s.GetUnit(h).Hp = 30;
        s.GetUnit(ally).MaxHp = 20;
        Ko(s, ally);
        Run(s, Use(h, "bound_resurrect", ally));
        s.DealDamage(null, ally, 10);
        Assert.Equal(25, s.SharedPoolOf(h)!.Pool);
        s.Heal(null, ally, 8);                                                            // healing either heals the pool
        Assert.Equal(33, s.SharedPoolOf(h)!.Pool);
        s.DealDamage(null, h, 99);
        Assert.True(s.GetUnit(h).IsKnockedOut);
        Assert.True(s.GetUnit(ally).IsKnockedOut);
    }

    [Fact]
    public void BoundResurrectCannotBeUsedWhileAlreadySharing()
    {
        var (s, h, ally) = Board(1, "bound_resurrect");
        var body = World.Unit(s, Side.A, Row.Rear);
        Ko(s, ally);
        Run(s, Use(h, "bound_resurrect", ally));
        Ko(s, body);
        K.Ready(s, h);
        s.ShiftMomentum(Side.A, 4);
        Assert.NotNull(Why(s, Use(h, "bound_resurrect", body)));
    }

    [Fact]
    public void MartyrHealsEveryAllySixToEightWhenTheLifebinderIsKo()
    {
        var (s, h, ally) = Board(1, "martyr");
        var other = World.Unit(s, Side.A, Row.Rear);
        var enemy = s.Formation.Units(Side.B, Row.Front)[0];
        s.GetUnit(ally).Hp = 10;
        s.GetUnit(other).Hp = 10;
        s.GetUnit(enemy).Hp = 10;
        s.GetUnit(h).Hp = 0;
        Actions.Settle(s, enemy);
        Assert.InRange(s.GetUnit(ally).Hp - 10, 6, 8);
        Assert.InRange(s.GetUnit(other).Hp - 10, 6, 8);
        Assert.Equal(10, s.GetUnit(enemy).Hp);
    }

    [Fact]
    public void LifebloodHealsTheLifebinderFiveWhenAnAllyIsKo()
    {
        var (s, h, ally) = Board(1, "lifeblood");
        s.GetUnit(h).Hp = 20;
        s.GetUnit(ally).Hp = 0;
        Actions.Settle(s, 2);
        Assert.Equal(25, s.GetUnit(h).Hp);
        var enemy = s.Formation.Units(Side.B, Row.Front)[0];
        s.GetUnit(enemy).Hp = 0;
        Actions.Settle(s, h);
        Assert.Equal(25, s.GetUnit(h).Hp);                                                // an enemy KO does not count
    }
}
