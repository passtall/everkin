using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class UniqueAttackTests
{
    private static BattleState State(ulong seed = 1) => new(seed);

    private static IReadOnlyList<BattleEvent> Use(BattleState s, int user, string skill, int? target = null) =>
        Actions.Use(s, World.Catalog, new SkillUse(user, skill, target));

    // ---- the catalog ----

    [Fact]
    public void EveryCreatureHasItsUniqueAttackInTheCatalogWithDelayAndMomentumFromTheContent()
    {
        Assert.Equal(319, World.Content.Attacks.Count);
        foreach (var a in World.Content.Attacks)
        {
            var skill = World.Catalog.Get(a.Id);
            Assert.Equal((a.Delay, a.Momentum), (skill.Delay, skill.MomentumCost));
            Assert.True(skill.IsAttack);
            Assert.Equal(a.Magical, skill.Magical);
            Assert.Equal(a.Shape == "pulse" ? TargetKind.None : TargetKind.Unit, skill.Target);
        }
    }

    [Fact]
    public void BoltAndPulseAttacksAreMagicalAndEveryOtherStyleIsPhysical()
    {
        Assert.All(World.Content.Attacks, a => Assert.Equal(a.Style is "bolt" or "pulse", a.Magical));
    }

    [Fact]
    public void ABattleUnitKnowsItsAttackFirstAndNothingElseUntilItsClassesArePorted()
    {
        var spec = new Everkin.Core.Setup.BattleSpec(1,
            new Everkin.Core.Setup.TeamSetup(new[] { new Everkin.Core.Setup.UnitSetup("abacus-strategist", Row.Front) }),
            new Everkin.Core.Setup.TeamSetup(new[] { new Everkin.Core.Setup.UnitSetup("ankylosaur-guardian-club", Row.Front) }));
        var s = Everkin.Core.Setup.BattleSetup.Create(spec, World.Catalog);
        Assert.Equal("bead_count", s.Units[0].KnownSkills[0]);
        Assert.Equal("tail_club", s.Units[1].KnownSkills[0]);
    }

    // ---- a melee attack: damage, Front row, timing ----

    [Fact]
    public void ATailClubHitsForThreeToFourAtPowerFiveAndSetsTheNextWait()
    {
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "tail_club");
            var b = World.Unit(s, Side.B, Row.Front);
            s.DrainEvents();
            var events = Use(s, a, "tail_club", b);
            damages.Add(40 - s.GetUnit(b).Hp);
            Assert.Equal(new SkillUsed(a, "tail_club", b), events[0]);
            Assert.Equal(Timeline.Wait(4, 5), s.Timeline.TurnOf(a)!.Due);
        }
        Assert.Equal(new[] { 3, 4 }, damages.OrderBy(x => x));
    }

    [Fact]
    public void DamageScalesWithPowerRoundedDownWithAMinimumOfOne()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "tail_club");
        var b = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(a).Power = 1;                                  // 3 x 1 / 5 = 0 -> 1, 4 x 1 / 5 = 0 -> 1
        Use(s, a, "tail_club", b);
        Assert.Equal(39, s.GetUnit(b).Hp);
    }

    [Fact]
    public void AMeleeAttackNeedsTheUserInItsFrontRow()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Middle, "tail_club");
        var b = World.Unit(s, Side.B, Row.Front);
        Assert.Equal("needs the Front row", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "tail_club", b)));
        Assert.Empty(Actions.LegalTargets(s, World.Catalog, a, "tail_club"));
        s.Formation.Remove(a);
        s.Formation.Place(a, Side.A, Row.Front);
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "tail_club", b)));
    }

    [Fact]
    public void AProjectileAttackWorksFromAnyRowAndCanOnlyTargetEnemies()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Rear, "rivet_shot");
        var ally = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.B, Row.Rear);
        Assert.Equal(new[] { b }, Actions.LegalTargets(s, World.Catalog, a, "rivet_shot"));
        Assert.Equal("not a legal target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "rivet_shot", ally)));
    }

    [Fact]
    public void AMeleeAttackCannotTargetAnAirborneUnit()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "tail_club");
        var b = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(b).IsAirborne = true;
        Assert.Empty(Actions.LegalTargets(s, World.Catalog, a, "tail_club"));
    }

    // ---- Direct attacks: breath and pulse work in melee reach ----

    [Fact]
    public void ABreathCanOnlyPickAnEnemyOfTheEnemyFrontRowAndHitsItsNeighboursForLess()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "sand_spill");
        var left = World.Unit(s, Side.B, Row.Front);
        var target = World.Unit(s, Side.B, Row.Front);
        Assert.Equal(new[] { left, target }, Actions.LegalTargets(s, World.Catalog, a, "sand_spill"));
        Use(s, a, "sand_spill", target);
        Assert.InRange(40 - s.GetUnit(target).Hp, 2, 3);
        Assert.InRange(40 - s.GetUnit(left).Hp, 1, 2);
    }

    [Fact]
    public void ABreathDoesNotReachARearUnitWhenTheMiddleRowIsEmpty()
    {
        // 20.5: the Rear unit is not adjacent to a Front unit when the Middle row is empty.
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "sand_spill");
        var target = World.Unit(s, Side.B, Row.Front);
        var rear = World.Unit(s, Side.B, Row.Rear);
        Use(s, a, "sand_spill", target);
        Assert.Equal(40, s.GetUnit(rear).Hp);
    }

    [Fact]
    public void ABreathHitsAMiddleUnitBehindAtFiftyPercentOverlap()
    {
        // 20.5: a Front unit with a neighbour and a Middle unit behind it at 50% overlap: both are adjacent.
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "sand_spill");
        var target = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);                      // centre 0 against Front centres -1 and +1
        Use(s, a, "sand_spill", target);
        Assert.InRange(40 - s.GetUnit(middle).Hp, 1, 2);
    }

    [Fact]
    public void APulseHitsEveryEnemyInTheFrontRowStealthedOnesToo()
    {
        // 20.5: a pulse against three Front units, one of them stealthed, hits all three.
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "sour_chord");
        var enemies = new[] { World.Unit(s, Side.B, Row.Front), World.Unit(s, Side.B, Row.Front), World.Unit(s, Side.B, Row.Front) };
        s.GetUnit(enemies[1]).IsStealthed = true;
        var middle = World.Unit(s, Side.B, Row.Middle);
        Use(s, a, "sour_chord");
        Assert.All(enemies, e => Assert.InRange(40 - s.GetUnit(e).Hp, 1, 2));
        Assert.False(s.GetUnit(enemies[1]).IsStealthed);
        Assert.Equal(40, s.GetUnit(middle).Hp);
    }

    [Fact]
    public void APulseAgainstAnEmptyFrontRowCanBeUsedHitsNobodyAndSpendsItsDelay()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "sour_chord");
        World.Unit(s, Side.B, Row.Middle);
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "sour_chord")));
        Use(s, a, "sour_chord");
        Assert.Equal(Timeline.Wait(4, 5), s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void AMiddleRowUnitCannotUseAPulse()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Middle, "sour_chord");
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal("needs the Front row", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "sour_chord")));
    }

    [Fact]
    public void APulseThatHitsThreeEnemiesMovesMomentumOnceInOurFavour()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "sour_chord");
        for (var i = 0; i < 3; i++) World.Unit(s, Side.B, Row.Front);
        Use(s, a, "sour_chord");
        Assert.Equal(1, s.Momentum.Value);
    }

    // ---- extras ----

    private static int Count(Func<ulong, bool> test, int seeds = 1000)
    {
        var n = 0;
        for (ulong seed = 1; seed <= (ulong)seeds; seed++) if (test(seed)) n++;
        return n;
    }

    [Fact]
    public void ATwentyPercentStunLandsAboutOneTimeInFive()
    {
        var stunned = Count(seed =>
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "tail_club");
            var b = World.Unit(s, Side.B, Row.Front);
            Use(s, a, "tail_club", b);
            return Statuses.Has(s.GetUnit(b), StatusIds.Stunned);
        });
        Assert.InRange(stunned, 150, 250);
    }

    [Fact]
    public void TheStunLastsItsFiveTimeUnits()
    {
        for (ulong seed = 1; seed < 200; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "tail_club");
            var b = World.Unit(s, Side.B, Row.Front);
            Use(s, a, "tail_club", b);
            if (!Statuses.Has(s.GetUnit(b), StatusIds.Stunned)) continue;
            Assert.Equal(5, Statuses.Get(s.GetUnit(b), StatusIds.Stunned)!.TimeLeft(s.Timeline.Now));
            return;
        }
        Assert.Fail("no stun in 200 seeds");
    }

    [Fact]
    public void ABleedingExtraGivesThreeBleedingSkills()
    {
        for (ulong seed = 1; seed < 200; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "scale_swipe");
            var b = World.Unit(s, Side.B, Row.Front);
            Use(s, a, "scale_swipe", b);
            if (Statuses.Get(s.GetUnit(b), StatusIds.Bleeding) is not { } bleeding) continue;
            Assert.Equal(3, bleeding.UsesLeft);
            return;
        }
        Assert.Fail("no bleeding in 200 seeds");
    }

    [Fact]
    public void ACertainLaterNextTurnPushesTheTargetsTurnBackTwo()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "bead_count");
        var b = World.Unit(s, Side.B, Row.Front);
        s.Timeline.ScheduleTurn(b, 5, 5);
        var due = s.Timeline.TurnOf(b)!.Due;
        Use(s, a, "bead_count", b);
        Assert.Equal(due + 2, s.Timeline.TurnOf(b)!.Due);
    }

    [Fact]
    public void AWaterGroundExtraAppearsAboutHalfTheTimeOnTheTargetsSpot()
    {
        var water = Count(seed =>
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Rear, "bubble_orb");
            var b = World.Unit(s, Side.B, Row.Front);
            var spot = s.Ground.SpotOf(b);
            Use(s, a, "bubble_orb", b);
            return s.Ground.Has(spot, GroundKind.Water);
        });
        Assert.InRange(water, 420, 580);
    }

    [Fact]
    public void APushExtraMovesTheTargetOneRowBackAndARearUnitStays()
    {
        var pushed = 0;
        for (ulong seed = 1; seed <= 300; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "trample");
            var front = World.Unit(s, Side.B, Row.Front);
            Use(s, a, "trample", front);
            if (s.Formation.PositionOf(front).Row == Row.Middle) pushed++;
        }
        Assert.InRange(pushed, 30, 100);

        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "trample");
            var rear = World.Unit(s, Side.B, Row.Rear);
            Use(s, a, "trample", rear);
            Assert.Equal(Row.Rear, s.Formation.PositionOf(rear).Row);
        }
    }

    [Fact]
    public void ThePullExtraBringsTheTargetOneRowTowardsTheBattleLine()
    {
        var pulled = 0;
        for (ulong seed = 1; seed <= 300; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "ribbon_lash");
            var middle = World.Unit(s, Side.B, Row.Middle);
            Use(s, a, "ribbon_lash", middle);
            if (s.Formation.PositionOf(middle).Row == Row.Front) pulled++;
        }
        Assert.InRange(pulled, 30, 100);
    }

    [Fact]
    public void TheStealthExtraGivesTheAttackerStealthAfterItsHit()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "snag_and_vanish");
        World.Unit(s, Side.A, Row.Middle);                                   // stealth needs another visible ally (section 10.1)
        var b = World.Unit(s, Side.B, Row.Front);
        Use(s, a, "snag_and_vanish", b);
        Assert.True(s.GetUnit(a).IsStealthed);
        Assert.InRange(40 - s.GetUnit(b).Hp, 2, 3);
    }

    [Fact]
    public void AStealExtraMovesAHelpfulStatusFromTheTargetToTheAttacker()
    {
        for (ulong seed = 1; seed < 300; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "mask_swap");
            var b = World.Unit(s, Side.B, Row.Front);
            Statuses.Apply(s, new StatusApplication(b, StatusIds.Ward, Duration: 30, Magnitude: 10));
            Use(s, a, "mask_swap", b);
            if (s.GetUnit(a).Ward == 0) continue;
            Assert.Equal(0, s.GetUnit(b).Ward);                              // the Ward took part of the hit, the rest moved
            return;
        }
        Assert.Fail("no steal in 300 seeds");
    }

    // ---- interception: melee 75 / 50, projectile 50 / 25, own side never ----

    [Fact]
    public void AMeleeHitIsInterceptedBySomeoneInFrontAtSeventyFivePercentAndAProjectileAtFifty()
    {
        int Intercepted(string skill, bool melee)
        {
            return Count(seed =>
            {
                var s = State(seed);
                var a = World.Unit(s, Side.A, melee ? Row.Front : Row.Rear, skill);
                var guard = World.Unit(s, Side.B, Row.Front);
                var target = World.Unit(s, Side.B, Row.Middle);              // the same centre: 100% overlap
                s.DrainEvents();
                var events = Use(s, a, skill, target);
                return events.Any(e => e is Intercepted);
            }, 2000);
        }
        // Full hits and partial hits both count as an interception; a hit of 1 cannot be split.
        Assert.InRange(Intercepted("tail_club", melee: true), 1400, 1600);
        Assert.InRange(Intercepted("rivet_shot", melee: false), 900, 1100);
    }

    [Fact]
    public void AnAttackOnTheUsersOwnSideIsNeverIntercepted()
    {
        var s = State();
        World.Unit(s, Side.A, Row.Front);
        var target = World.Unit(s, Side.A, Row.Middle);
        for (var i = 0; i < 50; i++) Assert.Empty(Interception.Resolve(s, target, 5, true, Delivery.Melee, Side.A).Shares);
    }

    [Fact]
    public void AFiftyPercentOverlapGivesSeventyFiveBecomesFiftyForMeleeAndTwentyFiveForProjectiles()
    {
        Assert.Equal((75, 50), (Interception.ChanceFor(100, Delivery.Melee), Interception.ChanceFor(50, Delivery.Melee)));
        Assert.Equal((50, 25), (Interception.ChanceFor(100, Delivery.Projectile), Interception.ChanceFor(50, Delivery.Projectile)));
        Assert.Equal(0, Interception.ChanceFor(0, Delivery.Melee));
    }

    // ---- Silenced and Blind ----

    [Fact]
    public void ASilencedUnitCannotUseABoltAttackButCanUseAPhysicalOne()
    {
        var s = State();
        var bolt = World.Unit(s, Side.A, Row.Front, "ring_spin", "tail_club");       // Ring Spin has the bolt style
        World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(bolt, StatusIds.Silenced, Duration: 20));
        Assert.Contains("Silenced", Actions.WhyNot(s, World.Catalog, new SkillUse(bolt, "ring_spin", 1)));
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(bolt, "tail_club", 1)));
    }
}
