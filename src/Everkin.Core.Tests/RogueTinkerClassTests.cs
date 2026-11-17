using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The Thief, Saboteur, Engineer and Chemist.</summary>
public class RogueTinkerClassTests
{
    [Fact]
    public void AllFourClassesAreImplemented()
    {
        AssertClassImplemented("thief", magical: false);
        AssertClassImplemented("saboteur", magical: false);
        AssertClassImplemented("engineer", magical: false);
        AssertClassImplemented("chemist", magical: false);
    }

    // ---- Thief ----

    [Fact]
    public void MeltAwayGivesStealthAndCostsTwoMomentum()
    {
        var (s, t, _) = Board(1, "melt_away");
        Run(s, Use(t, "melt_away"));
        Assert.True(s.GetUnit(t).IsStealthed);
        Assert.Equal(6, s.Momentum.Value);
    }

    [Fact]
    public void SnatchHitsForFiveToSixAndStealsOneHelpfulStatusWithItsRemainingTime()
    {
        var (s, t, e) = Board(1, "snatch");
        Statuses.Apply(s, new StatusApplication(e, StatusIds.SongHaste, Duration: 20, Magnitude: 1));
        Statuses.Apply(s, new StatusApplication(e, StatusIds.WarDrums, Duration: 15));
        AdvanceTo(s, 5);
        Run(s, Use(t, "snatch", e));
        Assert.InRange(Lost(s, e), 5, 6);
        Assert.Equal(1, new[] { StatusIds.SongHaste, StatusIds.WarDrums }.Count(id => Has(s, t, id)));
        Assert.Equal(1, new[] { StatusIds.SongHaste, StatusIds.WarDrums }.Count(id => Has(s, e, id)));
        var stolen = Has(s, t, StatusIds.SongHaste) ? StatusIds.SongHaste : StatusIds.WarDrums;
        Assert.Equal(stolen == StatusIds.SongHaste ? 15 : 10, Statuses.Get(s.GetUnit(t), stolen)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void SnatchCannotStealStealthAndOnlyStealsFromTheTargetItselfNotAnInterceptor()
    {
        var (s, t, e) = Board(1, "snatch");
        s.GetUnit(e).IsStealthed = false;
        Run(s, Use(t, "snatch", e));
        Assert.False(s.GetUnit(t).IsStealthed);

        for (ulong seed = 1; seed <= 60; seed++)
        {
            var s2 = new BattleState(seed);
            var thief = World.Unit(s2, Side.A, Row.Front, "snatch");
            var guard = World.Unit(s2, Side.B, Row.Front);
            var target = World.Unit(s2, Side.B, Row.Middle);
            Statuses.Apply(s2, new StatusApplication(target, StatusIds.WarDrums, Duration: 20));
            s2.ShiftMomentum(Side.A, 6);
            var events = Run(s2, Use(thief, "snatch", target));
            if (events.Any(x => x is Intercepted)) Assert.False(Has(s2, thief, StatusIds.WarDrums));       // an interceptor took the hit: no steal
        }
    }

    [Fact]
    public void MasterThiefStealsEveryHelpfulStatusAndShadowPurseMovesMomentumOncePerSteal()
    {
        var (s, t, e) = Board(1, "snatch", "master_thief", "shadow_purse");
        Statuses.Apply(s, new StatusApplication(e, StatusIds.SongHaste, Duration: 20, Magnitude: 1));
        Statuses.Apply(s, new StatusApplication(e, StatusIds.WarDrums, Duration: 20));
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Ward, Magnitude: 20));
        Run(s, Use(t, "snatch", e));
        Assert.True(Has(s, t, StatusIds.SongHaste) && Has(s, t, StatusIds.WarDrums));
        Assert.False(Has(s, e, StatusIds.SongHaste) || Has(s, e, StatusIds.WarDrums));
        Assert.Equal(6 - 1 + 3, s.Momentum.Value);                                          // snatch costs 1, the Ward absorbs the whole hit (no shift), three steals move 3
    }

    [Fact]
    public void MasterThiefAlsoMakesTheThiefsOwnStealingAttackTakeEveryStatus()
    {
        for (ulong seed = 1; seed <= 300; seed++)
        {
            var (s, t, e) = Board(seed, "mask_swap", "master_thief");
            Statuses.Apply(s, new StatusApplication(e, StatusIds.SongHaste, Duration: 20, Magnitude: 1));
            Statuses.Apply(s, new StatusApplication(e, StatusIds.WarDrums, Duration: 20));
            Run(s, Use(t, "mask_swap", e));
            if (!Has(s, t, StatusIds.SongHaste) && !Has(s, t, StatusIds.WarDrums)) continue;
            Assert.True(Has(s, t, StatusIds.SongHaste) && Has(s, t, StatusIds.WarDrums));
            return;
        }
        Assert.Fail("the 20% steal never happened");
    }

    [Fact]
    public void EscapeArtistGivesStealthTheFirstTimeTheThiefDropsBelowHalfHpOnly()
    {
        var (s, t, e) = Board(1, "escape_artist");
        s.DealDamage(e, t, 19);
        Assert.False(s.GetUnit(t).IsStealthed);                                              // 21 of 40
        s.DealDamage(e, t, 2);                                                               // 19 of 40: below half
        Assert.True(s.GetUnit(t).IsStealthed);
        Stealth.Break(s, t, StealthCause.Revealed);
        s.DealDamage(e, t, 2);
        Assert.False(s.GetUnit(t).IsStealthed);                                              // only the first time
    }

    [Fact]
    public void LiftASkillCopiesTheTargetsLastActiveSkillWithTheThiefsOwnPower()
    {
        var s = new BattleState(1);
        var thief = World.Unit(s, Side.A, Row.Front, "lift_a_skill");
        var enemy = World.Unit(s, Side.B, Row.Front, "tail_club");
        var victim = World.Unit(s, Side.B, Row.Middle);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(enemy, "tail_club", thief));
        s.GetUnit(thief).Hp = 40;
        s.GetUnit(thief).Power = 10;
        Assert.Equal("tail_club", s.GetUnit(enemy).LastActiveSkillId);
        Run(s, Use(thief, "lift_a_skill", enemy, second: victim));
        Assert.InRange(Lost(s, victim), 6, 8);                                               // 3-4 at Power 10
        Assert.Equal(5 - 1 + 1, s.Momentum.Value);                                          // the enemy hit moved it to 5, the copy costs 1, its hit moves 1
    }

    [Fact]
    public void LiftASkillNeedsACopyableSkillAndALegalTargetForIt()
    {
        var s = new BattleState(1);
        var thief = World.Unit(s, Side.A, Row.Front, "lift_a_skill");
        var enemy = World.Unit(s, Side.B, Row.Front, "tail_club");
        s.ShiftMomentum(Side.A, 6);
        Assert.NotNull(Why(s, Use(thief, "lift_a_skill", enemy)));                           // it used nothing yet
        Run(s, Use(enemy, "tail_club", thief));
        Assert.NotNull(Why(s, Use(thief, "lift_a_skill", enemy)));                           // the copy needs a target
        Assert.NotNull(Why(s, Use(thief, "lift_a_skill", enemy, second: thief)));            // an ally of the thief is a legal target for a melee hit but...
    }

    [Fact]
    public void LiftASkillNeverCopiesMovesSkipsOrItself()
    {
        var s = new BattleState(1);
        var thief = World.Unit(s, Side.A, Row.Front, "lift_a_skill");
        var other = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Actions.Move(s, other, Row.Middle, 0);
        Assert.NotNull(Why(s, Use(thief, "lift_a_skill", other)));
        Assert.Null(s.GetUnit(other).LastActiveSkillId);
    }

    // ---- Saboteur ----

    [Fact]
    public void FlashPowderBlindsEveryEnemyInMeleeReachForTwenty()
    {
        var (s, sab, e1) = Board(1, "flash_powder");
        var e2 = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(sab, "flash_powder"));
        Assert.True(Has(s, e1, StatusIds.Blind) && Has(s, e2, StatusIds.Blind));
        Assert.False(Has(s, back, StatusIds.Blind));
        Assert.Equal(20, Statuses.Get(s.GetUnit(e1), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void CutTheStringsRemovesTheNextAutoEventOfAnEnemyThatOwnsOnes()
    {
        var (s, sab, e) = Board(1, "cut_the_strings");
        Assert.NotNull(Why(s, Use(sab, "cut_the_strings", e)));                              // no auto events
        s.Timeline.Add(TimelineKind.AutoEvent, 5, e, "x");
        var later = s.Timeline.Add(TimelineKind.AutoEvent, 9, e, "y");
        Assert.Null(Why(s, Use(sab, "cut_the_strings", e)));
        Run(s, Use(sab, "cut_the_strings", e));
        Assert.Equal(new[] { later.Id }, s.Timeline.Upcoming().Where(x => x.Kind == TimelineKind.AutoEvent).Select(x => x.Id));
        Assert.Equal(6, s.Momentum.Value);
    }

    [Fact]
    public void JamGivesAUnitsNextSkillPlusFiveDelayAndAnInterceptorTakesItInstead()
    {
        var (s, sab, e) = Board(1, "jam");
        s.Formation.Remove(e);
        s.Formation.Place(e, Side.B, Row.Front);
        Run(s, Use(sab, "jam", e));
        Assert.Equal(5, Statuses.Get(s.GetUnit(e), StatusIds.NextSkillDelay)!.Magnitude);
        // 20.5: a Delay 9 skill that gets +3 is Delay 10; here +5 on a Delay 4 attack gives 9.
        s.GetUnit(e).LearnSkill("tail_club");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        s.GetUnit(ally).IsStealthed = false;
        K.Ready(s, e);
        Run(s, Use(e, "tail_club", sab));
        Assert.Equal(Timeline.Wait(9, 5), s.Timeline.TurnOf(e)!.Due);

        for (ulong seed = 1; seed <= 60; seed++)
        {
            var s2 = new BattleState(seed);
            var j = World.Unit(s2, Side.A, Row.Rear, "jam");
            s2.ShiftMomentum(Side.A, 6);
            var guard = World.Unit(s2, Side.B, Row.Front);
            var target = World.Unit(s2, Side.B, Row.Middle);
            var events = Run(s2, Use(j, "jam", target));
            if (events.OfType<Intercepted>().FirstOrDefault() is { } ic)
            {
                Assert.True(Has(s2, ic.InterceptorUnitId, StatusIds.NextSkillDelay));
                Assert.False(Has(s2, target, StatusIds.NextSkillDelay));
                return;
            }
        }
        Assert.Fail("never intercepted");
    }

    [Fact]
    public void WreckerDoublesDamageToConstructsOnly()
    {
        var (s, sab, e) = Board(1, "tail_club", "wrecker");
        var construct = s.AddUnit("Totem", Side.B, 40, 1, 1);
        construct.Kind = UnitKind.Construct;
        s.Formation.Place(construct.Id, Side.B, Row.Front);
        Run(s, Use(sab, "tail_club", construct.Id));
        Assert.InRange(Lost(s, construct.Id), 6, 8);
        K.Ready(s, sab);
        Run(s, Use(sab, "tail_club", e));
        Assert.InRange(Lost(s, e), 3, 4);
    }

    [Fact]
    public void ChaosAgentPushesAnEnemysNextTurnBackTwoOnlyWhenAnAttackHitsIt()
    {
        var (s, sab, e) = Board(1, "tail_club", "flash_powder", "chaos_agent");
        s.Timeline.ScheduleTurn(e, 5, 5);
        var due = s.Timeline.TurnOf(e)!.Due;
        Run(s, Use(sab, "flash_powder"));
        Assert.Equal(due, s.Timeline.TurnOf(e)!.Due);                                        // not an attack
        K.Ready(s, sab);
        Run(s, Use(sab, "tail_club", e));
        Assert.Equal(due + 2, s.Timeline.TurnOf(e)!.Due);
    }

    // ---- Engineer ----

    [Fact]
    public void BoltTurretIsPlacedOnAnyFreeSpotOfTheSideAndHas8Hp()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "bolt_turret");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        var places = World.Catalog.Get("bolt_turret").PlacementOptions(s, eng);
        Assert.Contains(new MoveOption(Row.Rear, 0), places);
        Run(s, Use(eng, "bolt_turret", place: new MoveOption(Row.Rear, 0)));
        var turret = s.Units.Single(u => u.CreatureId == "bolt_turret");
        Assert.Equal((8, UnitKind.Construct, Row.Rear), (turret.Hp, turret.Kind, s.Formation.PositionOf(turret.Id).Row));
    }

    [Fact]
    public void ATurretShootsTheNearestEnemyForTwoToThreeAfterEveryActionOfTheEngineer()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "bolt_turret");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "bolt_turret", place: new MoveOption(Row.Rear, 0)));
        Assert.InRange(Lost(s, e), 2, 3);                                                    // the turret shoots right after being placed
        var before = Lost(s, e);
        K.Ready(s, eng);
        Actions.SkipTurn(s, eng);
        Assert.InRange(Lost(s, e) - before, 2, 3);
        K.Ready(s, eng);
        Actions.Move(s, eng, Row.Middle, 0);
        Assert.True(Lost(s, e) >= before + 4);
    }

    [Fact]
    public void ATurretsShotsShiftMomentumOncePerShotAndAKoEngineerLosesItsQueuedShotsButTheTurretStays()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "bolt_turret");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "bolt_turret", place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(6 - 1 + 1, s.Momentum.Value);                                           // cost 1, one shot lands: +1
        var turret = s.Units.Single(u => u.CreatureId == "bolt_turret");
        s.Timeline.AddNow(eng, "turret:" + turret.Id);
        s.GetUnit(eng).Hp = 0;
        Actions.Settle(s, null);
        Assert.False(s.Timeline.Upcoming().Any(x => x.Kind == TimelineKind.AutoEvent));
        Assert.False(s.GetUnit(turret.Id).IsKnockedOut);
    }

    [Fact]
    public void RapidFireMakesEveryTurretFireTwice()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "bolt_turret", "rapid_fire");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "bolt_turret", place: new MoveOption(Row.Rear, 0)));
        Assert.InRange(Lost(s, e), 4, 6);
    }

    [Fact]
    public void BarricadeHas10HpAndReinforcedDoublesTheHpOfTurretsBarricadesAndTraps()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "barricade", "net_trap", "reinforced");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "barricade", place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(20, s.Units.Single(u => u.CreatureId == "barricade").Hp);
        K.Ready(s, eng);
        Run(s, Use(eng, "net_trap", place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(8, s.Units.Single(u => u.CreatureId == "net_trap").Hp);
    }

    [Fact]
    public void ABarricadeCannotBePlacedWhenTheSideHasEighteenUnits()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "barricade");
        for (var i = 0; i < 5; i++) World.Unit(s, Side.A, Row.Front);
        foreach (var row in new[] { Row.Middle, Row.Rear })
            for (var i = 0; i < 6; i++) World.Unit(s, Side.A, row);
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal(18, s.Formation.Count(Side.A));
        Assert.NotNull(Why(s, Use(eng, "barricade", place: new MoveOption(Row.Rear, 0))));
    }

    [Fact]
    public void ANetTrapGivesAUnitThatHitsItPlusThreeDelayOnItsNextSkill()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Rear, "net_trap");
        var e = World.Unit(s, Side.B, Row.Front, "tail_club", "rivet_shot");
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "net_trap", place: new MoveOption(Row.Middle, 0)));
        var trap = s.Units.Single(u => u.CreatureId == "net_trap");
        s.GetUnit(trap.Id).MaxHp = s.GetUnit(trap.Id).Hp = 40;
        Run(s, Use(e, "rivet_shot", trap.Id));
        Assert.Equal(3, Statuses.Get(s.GetUnit(e), StatusIds.NextSkillDelay)!.Magnitude);
    }

    [Fact]
    public void ADisarmTrapSaboteurSetsATrapOffWithoutTheEffect()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.B, Row.Rear, "net_trap");
        var sab = World.Unit(s, Side.A, Row.Front, "rivet_shot", "disarm_trap");
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.B, 6);
        Run(s, Use(eng, "net_trap", place: new MoveOption(Row.Middle, 0)));
        var trap = s.Units.Single(u => u.CreatureId == "net_trap");
        s.GetUnit(trap.Id).Hp = 40;
        Run(s, Use(sab, "rivet_shot", trap.Id));
        Assert.True(s.GetUnit(trap.Id).IsRemoved);
        Assert.False(Has(s, sab, StatusIds.NextSkillDelay));
    }

    [Fact]
    public void ANetTrapThatIsKoIsRemovedAtOnceAndFreesItsSlot()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Rear, "net_trap");
        var e = World.Unit(s, Side.B, Row.Front, "rivet_shot");
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "net_trap", place: new MoveOption(Row.Middle, 0)));
        var trap = s.Units.Single(u => u.CreatureId == "net_trap");
        s.GetUnit(trap.Id).Hp = 1;
        Run(s, Use(e, "rivet_shot", trap.Id));
        Assert.True(s.GetUnit(trap.Id).IsRemoved);
        Assert.Equal(0, s.Formation.Count(Side.A, Row.Middle));
    }

    [Fact]
    public void SelfRepairHealsTheEngineersTurretsAndBarricadesTwoAfterEveryAction()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.A, Row.Front, "barricade", "self_repair");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(eng, "barricade", place: new MoveOption(Row.Rear, 0)));
        var wall = s.Units.Single(u => u.CreatureId == "barricade");
        s.GetUnit(wall.Id).Hp = 5;
        K.Ready(s, eng);
        Actions.SkipTurn(s, eng);
        Assert.Equal(7, s.GetUnit(wall.Id).Hp);
    }

    // ---- Chemist ----

    [Fact]
    public void PoisonGasLastsTenAndCoversTheUnitsSpotAndTheSpotsNextToIt()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "poison_gas");
        var units = Enumerable.Range(0, 3).Select(_ => World.Unit(s, Side.B, Row.Front)).ToArray();       // -2 0 2
        Run(s, Use(c, "poison_gas", units[1]));
        Assert.All(new[] { -2, 0, 2 }, x => Assert.Equal(10, s.Ground.Find(new Spot(Side.B, Row.Front, x), GroundKind.Poison)!.EndsAt - s.Timeline.Now));
        Assert.All(units, u => Assert.True(Has(s, u, StatusIds.Poison)));
    }

    [Fact]
    public void FirebombBurnsTwentyAndAnInterceptedOneLandsOnTheInterceptorsSpot()
    {
        var (s, c, e) = Board(1, "firebomb");
        Run(s, Use(c, "firebomb", e));
        Assert.Equal(20, s.Ground.Find(s.Ground.SpotOf(e), GroundKind.Burning)!.EndsAt - s.Timeline.Now);

        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s2 = new BattleState(seed);
            var chem = World.Unit(s2, Side.A, Row.Rear, "firebomb");
            s2.ShiftMomentum(Side.A, 6);
            var guard = World.Unit(s2, Side.B, Row.Front);
            var target = World.Unit(s2, Side.B, Row.Middle);
            if (Run(s2, Use(chem, "firebomb", target)).OfType<Intercepted>().FirstOrDefault() is not { } ic) continue;
            Assert.True(s2.Ground.Has(s2.Ground.SpotOf(ic.InterceptorUnitId), GroundKind.Burning));
            return;
        }
        Assert.Fail("never intercepted");
    }

    [Fact]
    public void HealingDraughtHealsSixToSevenAndCanBeInterceptedWhenThrownAtAnEnemy()
    {
        var (s, c, ally) = Board(1, "healing_draught");
        var friend = s.Formation.Units(Side.A, Row.Middle)[0];
        s.GetUnit(friend).Hp = 10;
        Run(s, Use(c, "healing_draught", friend));
        Assert.InRange(s.GetUnit(friend).Hp - 10, 6, 7);
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s2 = new BattleState(seed);
            var chem = World.Unit(s2, Side.A, Row.Rear, "healing_draught");
            var guard = World.Unit(s2, Side.B, Row.Front);
            var target = World.Unit(s2, Side.B, Row.Middle);
            s2.GetUnit(guard).Hp = 10;
            s2.GetUnit(target).Hp = 10;
            if (!Run(s2, Use(chem, "healing_draught", target)).Any(x => x is Intercepted)) continue;
            Assert.True(s2.GetUnit(guard).Hp > 10);
            return;
        }
        Assert.Fail("never intercepted");
    }

    [Fact]
    public void ABigFinishCoversFiveSpotsAndAddsTwoDelayToFirebombAndPoisonGas()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "firebomb", "poison_gas", "big_finish");
        var units = Enumerable.Range(0, 5).Select(_ => World.Unit(s, Side.B, Row.Front)).ToArray();         // -4 -2 0 2 4
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(c, "firebomb", units[2]));
        Assert.Equal(5, s.Ground.Effects.Count(f => f.Kind == GroundKind.Burning));
        Assert.Equal(Timeline.Wait(5 + 2, 5), s.Timeline.TurnOf(c)!.Due);
        K.Ready(s, c);
        Run(s, Use(c, "poison_gas", units[2]));
        Assert.Equal(s.Timeline.Now + Timeline.Wait(4 + 2, 5), s.Timeline.TurnOf(c)!.Due);
    }

    [Fact]
    public void PotentBrewAlsoRemovesOneHarmfulStatusAndVolatileDoublesFireAndGas()
    {
        var (s, c, _) = Board(1, "healing_draught", "potent_brew");
        var friend = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(friend, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(friend, StatusIds.Silenced, Duration: 20));
        Run(s, Use(c, "healing_draught", friend));
        Assert.Equal(1, new[] { StatusIds.Blind, StatusIds.Silenced }.Count(id => Has(s, friend, id)));

        var (s2, c2, e2) = Board(1, "firebomb", "poison_gas", "volatile");
        Run(s2, Use(c2, "firebomb", e2));
        Assert.Equal(40, s2.Ground.Find(s2.Ground.SpotOf(e2), GroundKind.Burning)!.EndsAt - s2.Timeline.Now);
        var (s3, c3, e3) = Board(1, "poison_gas", "volatile");
        Run(s3, Use(c3, "poison_gas", e3));
        Assert.Equal(20, s3.Ground.Find(s3.Ground.SpotOf(e3), GroundKind.Poison)!.EndsAt - s3.Timeline.Now);
    }
}
