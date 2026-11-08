using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class HealerSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null, int? second = null) =>
        new(user, skill, target, row, null, second);

    private static void Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    private static void AdvanceTo(BattleState s, int time)
    {
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            Statuses.HandleExpiry(s, e);
        }
    }

    private static (BattleState S, int Healer, int Ally, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var h = World.Unit(s, Side.A, Row.Front, skills);
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        return (s, h, ally, enemy);
    }

    // ---- the catalog ----

    [Fact]
    public void AllFifteenHealerSkillsAreImplementedAndEveryActiveIsMagical()
    {
        var healer = World.Content.ProfessionSkills.Where(x => x.Profession == "healer").ToList();
        Assert.Equal(15, healer.Count);
        foreach (var def in healer)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            if (def.Kind == "A") Assert.True(skill.Magical);
        }
    }

    // ---- Mender ----

    [Fact]
    public void MendHealsEightToTenAndStopsAtMaximumHp()
    {
        var seen = new HashSet<int>();
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var (s, h, ally, _) = Board(seed, "mend");
            s.GetUnit(ally).Hp = 10;
            Run(s, Use(h, "mend", ally));
            seen.Add(s.GetUnit(ally).Hp - 10);
        }
        Assert.Equal(new[] { 8, 9, 10 }, seen.OrderBy(x => x));

        var (s2, h2, a2, _) = Board(1, "mend");
        s2.GetUnit(a2).Hp = 38;                                  // 20.5: healing exceeds missing HP: HP stops at maximum
        Run(s2, Use(h2, "mend", a2));
        Assert.Equal(40, s2.GetUnit(a2).Hp);
        Assert.Equal(Timeline.Wait(4, 5), s2.Timeline.TurnOf(h2)!.Due);
    }

    [Fact]
    public void HealingScalesWithPowerAndCannotTargetAKoUnitOrBeIntercepted()
    {
        var (s, h, ally, _) = Board(1, "mend");
        s.GetUnit(h).Power = 10;
        s.GetUnit(ally).Hp = 1;
        Run(s, Use(h, "mend", ally));
        Assert.InRange(s.GetUnit(ally).Hp - 1, 16, 20);          // 8-10 at Power 10
        var (s2, h2, a2, _) = Board(1, "mend");
        s2.GetUnit(a2).Hp = 0;
        Assert.Equal("not a legal target", Why(s2, Use(h2, "mend", a2)));
    }

    [Fact]
    public void ADirectHealIsNotInterceptedEvenWithAnAllyInFront()
    {
        for (ulong seed = 1; seed <= 40; seed++)
        {
            var s = new BattleState(seed);
            var h = World.Unit(s, Side.A, Row.Rear, "mend");
            var front = World.Unit(s, Side.A, Row.Front);
            var target = World.Unit(s, Side.A, Row.Middle);
            World.Unit(s, Side.B, Row.Front);
            s.GetUnit(target).Hp = 10;
            Run(s, Use(h, "mend", target));
            Assert.True(s.GetUnit(target).Hp >= 18);
            Assert.Equal(40, s.GetUnit(front).Hp);
        }
    }

    [Fact]
    public void AMendCanBeUsedOnAnEnemy()
    {
        var (s, h, _, enemy) = Board(1, "mend");
        s.GetUnit(enemy).Hp = 10;
        Run(s, Use(h, "mend", enemy));
        Assert.True(s.GetUnit(enemy).Hp > 10);
        Assert.Equal(0, s.Momentum.Value);                       // healing moves Momentum by 0
    }

    [Fact]
    public void ASilencedHealerCannotMendButCanUseItsPhysicalAttackUnlessItIsBolt()
    {
        // 20.5.
        var (s, h, ally, enemy) = Board(1, "mend", "shot", "bolt");
        Statuses.Apply(s, new StatusApplication(h, StatusIds.Silenced, Duration: 20));
        Assert.StartsWith("Silenced", Why(s, Use(h, "mend", ally))!);
        Assert.StartsWith("Silenced", Why(s, Use(h, "bolt", enemy))!);
        Assert.Null(Why(s, Use(h, "shot", enemy)));
    }

    [Fact]
    public void OverflowingCareSendsTheExcessToTheAdjacentAllyWithTheLowestHp()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Rear, "mend", "overflowing_care");
        var left = World.Unit(s, Side.A, Row.Front);
        var target = World.Unit(s, Side.A, Row.Front);
        var right = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(target).Hp = 39;                                // room for 1
        s.GetUnit(left).Hp = 20;
        s.GetUnit(right).Hp = 12;                                 // the lowest
        Run(s, Use(h, "mend", target));
        Assert.Equal(40, s.GetUnit(target).Hp);
        Assert.True(s.GetUnit(right).Hp >= 12 + 7);               // 8-10 healed, 1 used: 7-9 over
        Assert.Equal(20, s.GetUnit(left).Hp);
    }

    [Fact]
    public void WithoutOverflowingCareTheExcessIsLostAndNoChainContinues()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Rear, "mend");
        var target = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(other).Hp = 5;
        Run(s, Use(h, "mend", target));
        Assert.Equal(5, s.GetUnit(other).Hp);

        var s2 = new BattleState(1);
        var h2 = World.Unit(s2, Side.A, Row.Rear, "mend", "overflowing_care");
        var t2 = World.Unit(s2, Side.A, Row.Front);
        var o2 = World.Unit(s2, Side.A, Row.Front);
        World.Unit(s2, Side.B, Row.Front);
        s2.GetUnit(o2).Hp = 39;                                   // the neighbor can only take 1: its own excess is not passed on
        s2.GetUnit(t2).Hp = 40;
        Run(s2, Use(h2, "mend", t2));
        Assert.Equal(40, s2.GetUnit(o2).Hp);
    }

    [Fact]
    public void RainOfMendingHealsEveryUnitInARowForThreeToFour()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "rain_of_mending");
        var a1 = World.Unit(s, Side.A, Row.Middle);
        var a2 = World.Unit(s, Side.A, Row.Middle);
        var outside = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        foreach (var id in new[] { a1, a2, outside }) s.GetUnit(id).Hp = 10;
        s.ShiftMomentum(Side.A, 2);
        Assert.Contains(new RowTarget(Side.A, Row.Middle), World.Catalog.Get("rain_of_mending").LegalRows(s, h));
        Run(s, Use(h, "rain_of_mending", row: new RowTarget(Side.A, Row.Middle)));
        Assert.InRange(s.GetUnit(a1).Hp - 10, 3, 4);
        Assert.InRange(s.GetUnit(a2).Hp - 10, 3, 4);
        Assert.Equal(10, s.GetUnit(outside).Hp);
        Assert.Equal(0, s.Momentum.Value);                        // cost 2 paid
    }

    [Fact]
    public void DownpourAlsoHealsTheRowBehindForHalf()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "rain_of_mending", "downpour");
        var a1 = World.Unit(s, Side.A, Row.Middle);
        var behind = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(a1).Hp = 10;
        s.GetUnit(behind).Hp = 10;
        s.ShiftMomentum(Side.A, 2);
        Run(s, Use(h, "rain_of_mending", row: new RowTarget(Side.A, Row.Middle)));
        Assert.InRange(s.GetUnit(a1).Hp - 10, 3, 4);
        Assert.InRange(s.GetUnit(behind).Hp - 10, 1, 2);          // half of 3-4
    }

    // ---- Cleric ----

    [Fact]
    public void AbsolveRemovesEveryHarmfulStatusKeepsTheHelpfulAndPushesTheNextTurnBackTwo()
    {
        var (s, h, ally, _) = Board(1, "absolve");
        s.Timeline.Add(TimelineKind.UnitTurn, 9, ally);
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Poison, Duration: 50));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Ward, Magnitude: 4));
        s.GetUnit(ally).IsStealthed = true;
        Run(s, Use(h, "absolve", ally));
        Assert.False(Statuses.Has(s.GetUnit(ally), StatusIds.Poison));
        Assert.False(Statuses.Has(s.GetUnit(ally), StatusIds.Blind));
        Assert.Equal(4, s.GetUnit(ally).Ward);
        Assert.False(s.GetUnit(ally).IsStealthed);                 // a cleansing skill that hits a stealthed unit removes its stealth
        Assert.Equal(11, s.Timeline.TurnOf(ally)!.Due);
    }

    [Fact]
    public void DispelRemovesEveryHelpfulStatusAndLeavesTheHarmfulOnes()
    {
        var (s, h, ally, enemy) = Board(1, "dispel");
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Ward, Magnitude: 4));
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Haste, Duration: 20, Magnitude: 2));
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Poison, Duration: 50));
        Run(s, Use(h, "dispel", enemy));
        Assert.Equal(0, s.GetUnit(enemy).Ward);
        Assert.False(Statuses.Has(s.GetUnit(enemy), StatusIds.Haste));
        Assert.True(Statuses.Has(s.GetUnit(enemy), StatusIds.Poison));
        Assert.NotEqual(-1, ally);
    }

    [Fact]
    public void MassDispelClearsHelpfulStatusesFromEveryUnitInARow()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "mass_dispel");
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Front);
        var e3 = World.Unit(s, Side.B, Row.Middle);
        foreach (var id in new[] { e1, e2, e3 }) Statuses.Apply(s, new StatusApplication(id, StatusIds.Ward, Magnitude: 4));
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(h, "mass_dispel", row: new RowTarget(Side.B, Row.Front)));
        Assert.Equal(0, s.GetUnit(e1).Ward);
        Assert.Equal(0, s.GetUnit(e2).Ward);
        Assert.Equal(4, s.GetUnit(e3).Ward);
        Assert.Equal(Timeline.Wait(7, 5), s.Timeline.TurnOf(h)!.Due);
    }

    [Fact]
    public void StealBlessingMovesOneHelpfulStatusFromOneUnitOntoAnother()
    {
        var (s, h, ally, enemy) = Board(1, "steal_blessing");
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Ward, Magnitude: 6));
        s.ShiftMomentum(Side.A, 2);
        Run(s, Use(h, "steal_blessing", enemy, second: ally));
        Assert.Equal(0, s.GetUnit(enemy).Ward);
        Assert.Equal(6, s.GetUnit(ally).Ward);
    }

    [Fact]
    public void StealBlessingNeedsAHelpfulStatusAndTwoDifferentUnits()
    {
        var (s, h, ally, enemy) = Board(1, "steal_blessing");
        s.ShiftMomentum(Side.A, 2);
        Assert.Equal("not a valid pair for this skill", Why(s, Use(h, "steal_blessing", enemy, second: ally)));
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Ward, Magnitude: 6));
        Assert.Equal("needs two different units", Why(s, Use(h, "steal_blessing", enemy, second: enemy)));
        Assert.Equal("needs two targets", Why(s, Use(h, "steal_blessing", enemy)));
    }

    [Fact]
    public void StealBlessingPicksAmongSeveralStatusesBySeededRollWithEqualChance()
    {
        var firstTaken = new int[2];
        for (ulong seed = 1; seed <= 600; seed++)
        {
            var (s, h, ally, enemy) = Board(seed, "steal_blessing");
            Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Ward, Magnitude: 6));
            Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Haste, Duration: 20, Magnitude: 2));
            s.ShiftMomentum(Side.A, 2);
            Run(s, Use(h, "steal_blessing", enemy, second: ally));
            firstTaken[Statuses.Has(s.GetUnit(ally), StatusIds.Haste) ? 0 : 1]++;
        }
        Assert.All(firstTaken, c => Assert.InRange(c, 230, 370));
    }

    [Fact]
    public void TurnBackSendsAHarmfulStatusBackToTheUnitThatAppliedIt()
    {
        var (s, h, ally, enemy) = Board(1, "turn_back");
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Poison, Duration: 50, SourceUnitId: enemy));
        Run(s, Use(h, "turn_back", ally));
        Assert.False(Statuses.Has(s.GetUnit(ally), StatusIds.Poison));
        Assert.True(Statuses.Has(s.GetUnit(enemy), StatusIds.Poison));
        Assert.Equal(enemy, Statuses.Get(s.GetUnit(enemy), StatusIds.Poison)!.SourceUnitId);
    }

    [Fact]
    public void TurnBackNeedsAHarmfulStatusWhoseApplierIsStillStandingAndKeepsItsRemainingTime()
    {
        var (s, h, ally, enemy) = Board(1, "turn_back");
        Assert.Equal("not a valid target for this skill", Why(s, Use(h, "turn_back", ally)));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20, SourceUnitId: enemy));
        AdvanceTo(s, 8);
        Run(s, Use(h, "turn_back", ally));
        Assert.Equal(20, Statuses.Get(s.GetUnit(enemy), StatusIds.Blind)!.EndsAt);   // 12 left from time 8
        s.GetUnit(enemy).Hp = 0;
        Knockout.Resolve(s, enemy);
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Silenced, Duration: 20, SourceUnitId: enemy));
        Assert.Equal("not a valid target for this skill", Why(s, Use(h, "turn_back", ally)));
    }

    // ---- Lifebinder ----

    [Fact]
    public void ReviveBringsAKoUnitBackAtAQuarterOfItsMaximumHpWithoutAFreeTurn()
    {
        var (s, h, ally, enemy) = Board(1, "revive");
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 5);
        Assert.Null(Why(s, Use(h, "revive", ally)));
        Run(s, Use(h, "revive", ally));
        Assert.Equal(10, s.GetUnit(ally).Hp);                         // 25% of 40
        Assert.Equal(0, s.Momentum.Value);                            // Revive's Momentum 5
        Assert.Equal(Timeline.Wait(9, 5), s.Timeline.TurnOf(ally)!.Due);   // as its own action: Delay 9
        Assert.NotEqual(-1, enemy);
    }

    [Fact]
    public void ReviveCanTargetAnEnemyBodyAndNeverALivingUnitOrARemovedSummon()
    {
        var (s, h, ally, enemy) = Board(1, "revive");
        s.ShiftMomentum(Side.A, 5);
        Assert.Equal("not a valid target for this skill", Why(s, Use(h, "revive", ally)));     // alive
        s.GetUnit(enemy).Hp = 0;
        Knockout.Resolve(s, enemy);
        Assert.Null(Why(s, Use(h, "revive", enemy)));
        var golem = Summons.Summon(s, new SummonSpec(Side.A, "Golem", UnitKind.Summon, 25, 3, h, TakesTurns: true), Row.Rear, 0);
        s.GetUnit(golem).Hp = 0;
        Knockout.Resolve(s, golem);
        Assert.NotNull(Why(s, Use(h, "revive", golem)));                                       // removed at once: cannot be revived
    }

    [Fact]
    public void ReviveHpIsAtLeastOne()
    {
        var (s, h, ally, _) = Board(1, "revive");
        s.GetUnit(ally).MaxHp = 3;
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 5);
        Run(s, Use(h, "revive", ally));
        Assert.Equal(1, s.GetUnit(ally).Hp);
    }

    [Fact]
    public void ReviveOnAnUndeadRestoresTheOriginalCreature()
    {
        var s = new BattleState(1);
        var necro = World.Unit(s, Side.A, Row.Middle);
        var h = World.Unit(s, Side.B, Row.Front, "revive");
        var body = World.Unit(s, Side.B, Row.Middle);
        s.GetUnit(body).Hp = 0;
        Knockout.Resolve(s, body);
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 12);
        s.ShiftMomentum(Side.B, 5);
        Assert.Null(Why(s, Use(h, "revive", body)));
        Run(s, Use(h, "revive", body));
        Assert.False(s.GetUnit(body).IsUndead);
        Assert.Equal(Side.B, s.GetUnit(body).Side);
        Assert.Equal(10, s.GetUnit(body).Hp);
    }

    [Fact]
    public void HealingAnUndeadHurtsIt()
    {
        var s = new BattleState(1);
        var necro = World.Unit(s, Side.A, Row.Middle, "mend");
        var body = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Rear);
        s.GetUnit(body).Hp = 0;
        Knockout.Resolve(s, body);
        Necromancy.Raise(s, necro, body, Row.Front, 0, hp: 20);
        Run(s, Use(necro, "mend", body));
        Assert.InRange(s.GetUnit(body).Hp, 10, 12);                  // 20 - (8..10)
    }

    [Fact]
    public void LifeLinkMakesHealingOnOneHealTheOtherForHalf()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "life_link", "mend");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(a).Hp = 10;
        s.GetUnit(b).Hp = 10;
        Run(s, Use(h, "life_link", a, second: b));
        Assert.Equal(b, Statuses.Get(s.GetUnit(a), StatusIds.LifeLink)!.Magnitude);
        Ready(s, h);
        Run(s, Use(h, "mend", a));
        var healed = s.GetUnit(a).Hp - 10;
        Assert.InRange(healed, 8, 10);
        Assert.Equal(10 + healed / 2, s.GetUnit(b).Hp);              // half, and no echo back to a
    }

    [Fact]
    public void LifeLinkEndsAfterTenTimeUnitsAndAReLinkBreaksTheOldPartner()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "life_link");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Middle);
        var c = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(h, "life_link", a, second: b));
        Ready(s, h);
        Run(s, Use(h, "life_link", a, second: c));
        Assert.False(Statuses.Has(s.GetUnit(b), StatusIds.LifeLink));   // b lost its side of the link
        Assert.Equal(c, Statuses.Get(s.GetUnit(a), StatusIds.LifeLink)!.Magnitude);
        AdvanceTo(s, 12);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.LifeLink));
        Assert.False(Statuses.Has(s.GetUnit(c), StatusIds.LifeLink));
    }

    [Fact]
    public void BoundResurrectSharesOnePoolOfSumMaxHpStartingAtHealerHpPlusRevivalHp()
    {
        // 20.5: reviver 30 HP of 40, revived unit max 20 with 25% revival HP: the pool maximum is 60 and starts at 30 + 5 = 35.
        var (s, h, ally, _) = Board(1, "bound_resurrect");
        s.GetUnit(h).Hp = 30;
        s.GetUnit(ally).MaxHp = 20;
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(h, "bound_resurrect", ally));
        var pool = s.SharedPoolOf(h)!;
        Assert.Equal(60, pool.Max);
        Assert.Equal(35, pool.Pool);
        Assert.Equal(35, s.GetUnit(h).Hp);                             // each unit shows the pool, capped at its own maximum
        Assert.Equal(20, s.GetUnit(ally).Hp);
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void EveryHitOnEitherSharerReducesThePoolAndAtZeroBothAreKo()
    {
        var (s, h, ally, _) = Board(1, "bound_resurrect");
        s.GetUnit(h).Hp = 30;
        s.GetUnit(ally).MaxHp = 20;
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(h, "bound_resurrect", ally));
        s.DealDamage(null, ally, 10);
        Assert.Equal(25, s.SharedPoolOf(h)!.Pool);
        Assert.Equal(25, s.GetUnit(h).Hp);
        s.Heal(null, ally, 8);                                          // healing either heals the pool
        Assert.Equal(33, s.SharedPoolOf(h)!.Pool);
        s.DealDamage(null, h, 99);
        Assert.Equal(0, s.SharedPoolOf(h)!.Pool);
        Assert.True(s.GetUnit(h).IsKnockedOut);
        Assert.True(s.GetUnit(ally).IsKnockedOut);
        Knockout.ResolveAll(s);
        Assert.Null(s.SharedPoolOf(h));                                 // the link ends when either unit is KO
    }

    [Fact]
    public void ThePoolIsCappedAtTheSumOfMaximumHp()
    {
        var (s, h, ally, _) = Board(1, "bound_resurrect");
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(h, "bound_resurrect", ally));
        s.Heal(null, h, 999);
        Assert.Equal(s.SharedPoolOf(h)!.Max, s.SharedPoolOf(h)!.Pool);
    }

    [Fact]
    public void BoundStrengthGivesBothSharersOneLessDelayOnEverySkill()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "bound_resurrect", "bound_strength", "brawl_strike");
        var ally = World.Unit(s, Side.A, Row.Middle, "brawl_strike");
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Rear);
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(h, "bound_resurrect", ally));
        Ready(s, h);
        Ready(s, ally);
        var now = s.Timeline.Now;
        Actions.Use(s, World.Catalog, new SkillUse(h, "brawl_strike", enemy));
        Assert.Equal(now + Timeline.Wait(2, 5), s.Timeline.TurnOf(h)!.Due);   // Delay 3 - 1
        Assert.True(s.SharedPoolOf(ally)!.BoundStrength);
    }

    [Fact]
    public void BurnoutHealHealsEveryUnitAroundTheHealerForSixToEightAndTheHealerIsKo()
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
        var events = Actions.Use(s, World.Catalog, new SkillUse(h, "burnout_heal"));
        Assert.InRange(s.GetUnit(left).Hp - 10, 6, 8);
        Assert.InRange(s.GetUnit(right).Hp - 10, 6, 8);
        Assert.Equal(10, s.GetUnit(far).Hp);
        Assert.True(s.GetUnit(h).IsKnockedOut);
        Assert.Contains(events, e => e is UnitKnockedOut { UnitId: var u } && u == h);
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void TheHealersKoInBurnoutHealResolvesBeforeItsOwnReactionsCanFire()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "burnout_heal");
        var ally = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        var fired = false;
        s.AddReaction(new ReactionDef("when_hit", h, TriggerKind.WhenHit, (_, _) => fired = true));
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(ally).Hp = 5;
        Run(s, Use(h, "burnout_heal"));
        Assert.False(fired);
        Assert.True(s.GetUnit(h).IsKnockedOut);
    }

    [Fact]
    public void LifeForLifeGivesUpHalfTheHealersHpToReviveAKoUnitWithThatMuch()
    {
        var (s, h, ally, _) = Board(1, "life_for_life");
        s.GetUnit(h).Hp = 30;
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(h, "life_for_life", ally));
        Assert.Equal(15, s.GetUnit(h).Hp);
        Assert.Equal(15, s.GetUnit(ally).Hp);
        Assert.Equal(Timeline.Wait(6, 5), s.Timeline.TurnOf(ally)!.Due);       // scheduled as its own action: Life for Life's Delay 6
    }

    [Fact]
    public void LifeForLifeNeedsAtLeastTwoHp()
    {
        var (s, h, ally, _) = Board(1, "life_for_life");
        s.GetUnit(h).Hp = 1;
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 3);
        Assert.Equal("needs at least 2 HP to give up half", Why(s, Use(h, "life_for_life", ally)));
    }

    [Fact]
    public void CloneKeepsSharedPoolsIndependent()
    {
        var (s, h, ally, _) = Board(1, "bound_resurrect");
        s.GetUnit(ally).Hp = 0;
        Knockout.Resolve(s, ally);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(h, "bound_resurrect", ally));
        var copy = s.Clone();
        copy.DealDamage(null, h, 10);
        Assert.NotEqual(s.SharedPoolOf(h)!.Pool, copy.SharedPoolOf(h)!.Pool);
    }
}
