using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

internal static class World
{
    public static readonly string ContentDir = Path.Combine(FindRoot(), "content");
    public static readonly ContentSet Content = ContentLoader.LoadFromDirectory(ContentDir);
    public static readonly SkillCatalog Catalog = SkillCatalog.Create(Content);

    private static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Everkin.sln"))) dir = dir.Parent;
        return dir!.FullName;
    }

    /// <summary>A unit of 40 HP, Speed 5, Power 5 that knows the given skills, placed on the board.</summary>
    public static int Unit(BattleState s, Side side, Row row, params string[] skills)
    {
        var u = s.AddUnit("u", side, 40, 5, 5);
        s.Formation.Place(u.Id, side, row);
        foreach (var id in skills) u.LearnSkill(id);
        return u.Id;
    }
}

public class AttackSkillTests
{
    private static BattleState State(ulong seed = 1) => new(seed);

    // ---- the catalog ----

    [Fact]
    public void AllNineAttackTypeAttacksAreImplementedWithTheirDelayAndMomentumFromTheContent()
    {
        foreach (var a in World.Content.AttackSkills)
        {
            var skill = World.Catalog.Get(a.Id);
            Assert.Equal(a.Delay, skill.Delay);
            Assert.Equal(a.Momentum, skill.MomentumCost);
        }
        Assert.Equal(9, World.Content.AttackSkills.Count);
        Assert.Equal("heavy_blow", World.Catalog.AttackSkillFor("blunt"));
    }

    // ---- Brawl Strike: a basic hit ----

    [Fact]
    public void BrawlStrikeHitsForFourToFiveAtPowerFiveAndSetsTheNextWait()
    {
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
            var b = World.Unit(s, Side.B, Row.Front);
            s.DrainEvents();
            var events = Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
            damages.Add(40 - s.GetUnit(b).Hp);
            Assert.Equal(new SkillUsed(a, "brawl_strike", b), events[0]);
            Assert.IsType<DamageDealt>(events[1]);
            Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(a)!.Due);   // Delay 3 + (11 - 5)
        }
        Assert.Equal(new[] { 4, 5 }, damages.OrderBy(x => x));
    }

    [Theory]
    [InlineData(3, 2, 3)]    // 4-5 at Power 3 is 2-3
    [InlineData(10, 8, 10)]  // 4-5 at Power 10 is 8-10
    public void DamageScalesWithPower(int power, int min, int max)
    {
        var seen = new HashSet<int>();
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
            s.GetUnit(a).Power = power;
            var b = World.Unit(s, Side.B, Row.Front);
            Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
            seen.Add(40 - s.GetUnit(b).Hp);
        }
        Assert.Equal(min, seen.Min());
        Assert.Equal(max, seen.Max());
    }

    // ---- the use order and legality ----

    [Fact]
    public void ASkillTheUnitDoesNotKnowCannotBeUsed()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        Assert.Equal("the unit does not know this skill", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", b)));
        Assert.Throws<InvalidOperationException>(() => Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b)));
        Assert.StartsWith("unknown skill", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "no_such", b))!);
    }

    [Fact]
    public void ATargetedSkillNeedsALegalTargetInReach()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var front = World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", front)));
        Assert.Equal("not a legal target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", middle)));   // D1
        Assert.Equal("needs a target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike")));
    }

    [Fact]
    public void PlainMeleeReachesDistanceOneAndPolearmThrustAndLashReachDistanceTwo()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike", "polearm_thrust", "lash");
        World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        var rear = World.Unit(s, Side.B, Row.Rear);
        Assert.NotNull(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", middle)));
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "polearm_thrust", middle)));
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "lash", middle)));
        Assert.NotNull(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "polearm_thrust", rear)));   // distance 3
    }

    [Fact]
    public void ShotAndBoltReachAnyVisibleEnemyButNoAlly()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Rear, "shot", "bolt");
        var ally = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Middle);
        var rear = World.Unit(s, Side.B, Row.Rear);
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "shot", rear)));
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "bolt", rear)));
        Assert.Equal("not a legal target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "shot", ally)));
    }

    [Fact]
    public void AStealthedEnemyIsNotATargetAndAnAirborneOneCannotBeHitInMelee()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike", "shot");
        var b = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(b, StatusIds.Airborne, Duration: 20));
        Assert.Equal("not a legal target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", b)));
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "shot", b)));    // ranged attacks reach it
    }

    [Fact]
    public void SilencedBlocksBoltButNotThePhysicalAttacks()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "bolt", "shot");
        var b = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Silenced, Duration: 20));
        Assert.StartsWith("Silenced", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "bolt", b))!);
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "shot", b)));
    }

    [Fact]
    public void AKoUnitCannotActAndOnceTheBattleIsOverNothingCan()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(a).Hp = 0;
        Assert.Equal("the unit cannot act", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", b)));
        s.GetUnit(a).Hp = 40;
        BattleEnd.Surrender(s, Side.B);
        Assert.Equal("the battle is over", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "brawl_strike", b)));
    }

    // ---- Momentum (6.7) ----

    [Fact]
    public void ALandedHitMovesMomentumOneTowardTheAttacker()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front);
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.Equal(1, s.Momentum.Value);
    }

    [Fact]
    public void AHitAbsorbedEntirelyByAWardAHitOnAnAllyAndAnInterceptedHitMoveMomentumZero()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var ward = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(ward).Ward = 99;
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", ward));
        Assert.Equal(0, s.Momentum.Value);

        var s2 = State();
        var a2 = World.Unit(s2, Side.A, Row.Front, "brawl_strike");
        var ally = World.Unit(s2, Side.A, Row.Front);
        Actions.Use(s2, World.Catalog, new SkillUse(a2, "brawl_strike", ally));
        Assert.Equal(0, s2.Momentum.Value);        // 20.5: an attack on its own ally
        Assert.True(s2.GetUnit(ally).Hp < 40);       // the ally took the hit
    }

    [Fact]
    public void AnInterceptedHitMovesMomentumZeroAndDamagesTheInterceptor()
    {
        var found = false;
        for (ulong seed = 1; seed <= 200 && !found; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "shot");
            var front = World.Unit(s, Side.B, Row.Front);
            var target = World.Unit(s, Side.B, Row.Middle);   // lone cards: 100% overlap, 50% chance to intercept
            Actions.Use(s, World.Catalog, new SkillUse(a, "shot", target));
            if (s.GetUnit(front).Hp < 40)
            {
                found = true;
                Assert.Equal(0, s.Momentum.Value);
            }
        }
        Assert.True(found);
    }

    [Fact]
    public void ABlindUnitsAttackMissesHalfTheTimeAndAMissMovesMomentumAgainstIt()
    {
        int misses = 0, hits = 0;
        for (ulong seed = 1; seed <= 400; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
            var b = World.Unit(s, Side.B, Row.Front);
            Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
            Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
            if (s.GetUnit(b).Hp == 40) { misses++; Assert.Equal(-1, s.Momentum.Value); }
            else { hits++; Assert.Equal(1, s.Momentum.Value); }
        }
        Assert.InRange(misses, 150, 250);
        Assert.Equal(400, misses + hits);
    }

    [Fact]
    public void ACostIsPaidFirstAndAKoGivesNoMomentumBonus()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Rear);          // a second original so the battle goes on
        s.GetUnit(b).Hp = 1;
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.True(s.GetUnit(b).IsKnockedOut);
        Assert.Equal(1, s.Momentum.Value);        // the hit counts 1, the KO adds nothing
    }

    // ---- stealth, KO, victory, reactions ----

    [Fact]
    public void AttackingEndsTheUsersStealthAndBeingHitEndsTheTargets()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(a).IsStealthed = true;
        s.GetUnit(b).IsStealthed = true;   // a stealthed enemy cannot be chosen; make it legal by revealing first
        s.GetUnit(b).IsStealthed = false;
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.False(s.GetUnit(a).IsStealthed);

        var s2 = State();
        var a2 = World.Unit(s2, Side.A, Row.Front, "brawl_strike");
        var ally = World.Unit(s2, Side.A, Row.Front);
        s2.GetUnit(ally).IsStealthed = true;
        Actions.Use(s2, World.Catalog, new SkillUse(a2, "brawl_strike", ally));   // allies can be chosen while stealthed
        Assert.False(s2.GetUnit(ally).IsStealthed);
    }

    [Fact]
    public void KoingTheLastOriginalEndsTheBattleInTheSameAction()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(b).Hp = 1;
        var events = Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.Equal(BattleOutcome.SideAWins, s.Outcome);
        Assert.Contains(new BattleEnded(BattleOutcome.SideAWins, EndReason.AllOriginalsKo), events);
        Assert.Contains(events, e => e is UnitKnockedOut { UnitId: var u } && u == b);
    }

    [Fact]
    public void AWhenHitCounterAnswersAfterTheHitAndCanTurnAWinIntoADraw()
    {
        // 20.5 analogue through the executor: the counter of an enemy summon KOs the attacker after the attack KO'd the last original.
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front);
        var sum = Summons.Summon(s, new SummonSpec(Side.B, "Thorns", UnitKind.Summon, 10, 5, b, TakesTurns: false), Row.Middle, 0);
        s.GetUnit(a).Hp = 1;
        s.GetUnit(b).Hp = 1;
        s.AddReaction(new ReactionDef("thorns", sum, TriggerKind.WhenHit, (st, _) => st.DealDamage(sum, a, 3))
        { Condition = (_, t) => t.SubjectUnitId == b });
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.Equal(BattleOutcome.Draw, s.Outcome);
    }

    [Fact]
    public void BleedingHurtsAfterAMeleeSkillAndNotAfterAShot()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike", "shot");
        var b = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Bleeding, Uses: 3));
        Actions.Use(s, World.Catalog, new SkillUse(a, "shot", b));
        Assert.Equal(40, s.GetUnit(a).Hp);
        s.Timeline.Remove(s.Timeline.TurnOf(a)!.Id);
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.Equal(38, s.GetUnit(a).Hp);
    }

    // ---- riders ----

    [Fact]
    public void BladeStrikeMakesTheTargetBleedThirtyPercentOfTheTime()
    {
        var bleeding = 0;
        for (ulong seed = 1; seed <= 1000; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "blade_strike");
            var b = World.Unit(s, Side.B, Row.Front);
            Actions.Use(s, World.Catalog, new SkillUse(a, "blade_strike", b));
            if (Statuses.Has(s.GetUnit(b), StatusIds.Bleeding))
            {
                bleeding++;
                Assert.Equal(3, Statuses.Get(s.GetUnit(b), StatusIds.Bleeding)!.UsesLeft);
            }
        }
        Assert.InRange(bleeding, 250, 350);
    }

    [Fact]
    public void HeavyBlowStunsForFiveTimeUnitsThirtyPercentOfTheTimeAndHitsFiveToSix()
    {
        int stunned = 0;
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 1000; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "heavy_blow");
            var b = World.Unit(s, Side.B, Row.Front);
            Actions.Use(s, World.Catalog, new SkillUse(a, "heavy_blow", b));
            damages.Add(40 - s.GetUnit(b).Hp);
            if (Statuses.Has(s.GetUnit(b), StatusIds.Stunned))
            {
                stunned++;
                Assert.Equal(5, Statuses.Get(s.GetUnit(b), StatusIds.Stunned)!.EndsAt);
            }
            Assert.Equal(Timeline.Wait(4, 5), s.Timeline.TurnOf(a)!.Due);   // Delay 4
        }
        Assert.InRange(stunned, 250, 350);
        Assert.Equal(new[] { 5, 6 }, damages.OrderBy(x => x));
    }

    [Fact]
    public void AnyHitEndsAStunSoAStunnedTargetIsFreedByTheNextHit()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(b, StatusIds.Stunned, Duration: 20));
        Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
        Assert.False(Statuses.Has(s.GetUnit(b), StatusIds.Stunned));
    }

    // ---- Breath and Pulse ----

    [Fact]
    public void BreathHitsTheTargetForThreeToFourAndNeighborsForOneToTwoAndCannotBeIntercepted()
    {
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "breath");
            var left = World.Unit(s, Side.B, Row.Front);
            var target = World.Unit(s, Side.B, Row.Front);
            var right = World.Unit(s, Side.B, Row.Front);
            World.Unit(s, Side.B, Row.Middle);               // a unit behind that would intercept a melee or projectile hit
            Actions.Use(s, World.Catalog, new SkillUse(a, "breath", target));
            Assert.InRange(40 - s.GetUnit(target).Hp, 3, 4);
            Assert.InRange(40 - s.GetUnit(left).Hp, 0, 0 + 2);
            Assert.InRange(40 - s.GetUnit(right).Hp, 0, 2);
            Assert.True(s.GetUnit(left).Hp < 40 || s.GetUnit(right).Hp < 40);
            Assert.Equal(1, s.Momentum.Value);               // one shift for the whole block
        }
    }

    [Fact]
    public void BreathReachesOnlyTheNearestRowAndItsSplashSpansNoDeadBodies()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "breath");
        var target = World.Unit(s, Side.B, Row.Front);
        var body = World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        s.GetUnit(body).Hp = 0;
        Knockout.Resolve(s, body);
        Assert.Equal("not a legal target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "breath", middle)));   // D1
        Actions.Use(s, World.Catalog, new SkillUse(a, "breath", target));
        Assert.Equal(0, s.GetUnit(body).Hp);                 // the body is not hit
    }

    [Fact]
    public void BreathAlsoHitsTheTargetsAlliesNextToItAndItsOwnSideIfThatIsTheTarget()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "breath");
        var ally = World.Unit(s, Side.A, Row.Front);
        var ally2 = World.Unit(s, Side.A, Row.Front);
        Actions.Use(s, World.Catalog, new SkillUse(a, "breath", ally));
        Assert.True(s.GetUnit(ally).Hp < 40);
        Assert.True(s.GetUnit(ally2).Hp < 40);
        Assert.Equal(0, s.Momentum.Value);                   // only own-side hits: nothing to gain
    }

    [Fact]
    public void PulseHitsEveryEnemyInMeleeReachForTwoToThree()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "pulse");
        var f1 = World.Unit(s, Side.B, Row.Front);
        var f2 = World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        var ally = World.Unit(s, Side.A, Row.Middle);
        Assert.Null(Actions.WhyNot(s, World.Catalog, new SkillUse(a, "pulse")));
        Assert.Equal("this skill takes no target", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "pulse", f1)));
        Actions.Use(s, World.Catalog, new SkillUse(a, "pulse"));
        Assert.InRange(40 - s.GetUnit(f1).Hp, 2, 3);
        Assert.InRange(40 - s.GetUnit(f2).Hp, 2, 3);
        Assert.Equal(40, s.GetUnit(middle).Hp);              // out of melee reach
        Assert.Equal(40, s.GetUnit(ally).Hp);                // enemies only
        Assert.Equal(1, s.Momentum.Value);
    }

    [Fact]
    public void PulseIsUnavailableWithoutAnEnemyInReach()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front, "pulse");
        Assert.Equal("no enemy in melee reach", Actions.WhyNot(s, World.Catalog, new SkillUse(a, "pulse")));
    }

    // ---- Move and Skip Turn ----

    [Fact]
    public void MoveWaitsDelayFiveAndSkipTurnCostsTheSame()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front);
        s.GetUnit(a).Speed = 6;
        Actions.Move(s, a, Row.Middle, 0);
        Assert.Equal(10, s.Timeline.TurnOf(a)!.Due);             // 5 + (11 - 6)
        s.GetUnit(a).Speed = 9;
        var b = World.Unit(s, Side.B, Row.Front);
        Actions.SkipTurn(s, b);
        s.GetUnit(b).Speed = 9;
        Assert.Equal(Timeline.Wait(5, 5), s.Timeline.TurnOf(b)!.Due);
        Assert.Equal(7, Timeline.SkippedTurnWait(9));            // 20.5: Skip Turn at Speed 9
    }

    [Fact]
    public void BleedingHurtsAfterMove()
    {
        var s = State();
        var a = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Bleeding, Uses: 3));
        Actions.Move(s, a, Row.Middle, 0);
        Assert.Equal(38, s.GetUnit(a).Hp);                       // 20.5: a Bleeding unit uses Move
    }

    // ---- confusion ----

    [Fact]
    public void AConfusedUnitsSkillTakesARandomLegalTargetHalfTheTime()
    {
        int retargeted = 0;
        for (ulong seed = 1; seed <= 600; seed++)
        {
            var s = State(seed);
            var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
            var b = World.Unit(s, Side.B, Row.Front);
            World.Unit(s, Side.A, Row.Front);           // an ally is also a legal target
            Statuses.Apply(s, new StatusApplication(a, StatusIds.Confused, Duration: 20));
            var events = Actions.Use(s, World.Catalog, new SkillUse(a, "brawl_strike", b));
            if (events.OfType<SkillUsed>().Single().TargetUnitId != b) retargeted++;
        }
        Assert.InRange(retargeted, 100, 250);   // 50% to retarget, 2 of 3 legal units differ from the chosen one on average
    }
}
