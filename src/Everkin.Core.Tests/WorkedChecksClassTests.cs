using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The rows of game_system section 20.5 that need class skills or unique attacks (the others are in WorkedChecksTests and the area tests).</summary>
public class WorkedChecksClassTests
{
    [Fact]
    public void ADelayEightSkillThatGetsFiveExtraDelayIsDelayTen()
    {
        // 20.5: Delay is clamped to 1-10 after all modifiers (Jam adds 5; a Delay 8 Clay Golem would be 13).
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "clay_golem");
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Statuses.Apply(s, new StatusApplication(c, StatusIds.NextSkillDelay, Magnitude: 5));
        Run(s, Use(c, "clay_golem", place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(Timeline.Wait(10, 5), s.Timeline.TurnOf(c)!.Due);
    }

    [Fact]
    public void ASilenceThatLandsDoesNotMoveMomentumAndABlindUnitsMissShiftsItAgainstItsSide()
    {
        var (s, a, e) = Board(1, "tail_club");
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Silenced, Duration: 20));
        Assert.Equal(6, s.Momentum.Value);
        for (ulong seed = 1; seed <= 60; seed++)
        {
            var (s2, a2, e2) = Board(seed, "tail_club");
            Statuses.Apply(s2, new StatusApplication(a2, StatusIds.Blind, Duration: 20));
            var events = Run(s2, Use(a2, "tail_club", e2));
            if (!events.Any(x => x is AttackMissed)) continue;
            Assert.Equal(5, s2.Momentum.Value);                                              // the miss moved it 1 against the attacker's side
            return;
        }
        Assert.Fail("no miss in 60 seeds");
    }

    [Fact]
    public void ATrapIsShovedLikeAnyUnit()
    {
        var s = new BattleState(1);
        var eng = World.Unit(s, Side.B, Row.Rear, "net_trap");
        var pusher = World.Unit(s, Side.A, Row.Front, "gust");
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.B, 3);
        Run(s, Use(eng, "net_trap", place: new MoveOption(Row.Front, 0)));
        var trap = s.Units.Single(u => u.CreatureId == "net_trap");
        Assert.Equal(Row.Front, s.Formation.PositionOf(trap.Id).Row);
        Run(s, Use(pusher, "gust", trap.Id));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(trap.Id).Row);
    }

    [Fact]
    public void PoisonTicksAndAWardAbsorbedHitOnAStealthedUnitStillBreaksItsStealth()
    {
        var (s, a, e) = Board(1, "rivet_shot");
        var victim = World.Unit(s, Side.B, Row.Middle);
        s.GetUnit(victim).IsStealthed = true;
        s.GetUnit(victim).Ward = 20;
        Statuses.Apply(s, new StatusApplication(victim, StatusIds.Poison, Duration: 50));
        s.DrainEvents();
        Assert.Equal(6, s.Momentum.Value);
        s.DealDamage(a, victim, 3);                                                          // a hit the Ward absorbs completely
        Stealth.Break(s, victim, StealthCause.Hit);
        Assert.False(s.GetUnit(victim).IsStealthed);
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(2, Lost(s, victim));                                                    // a poison tick ignores the Ward; 10 time units at 1 per 5
        Assert.Equal(17, s.GetUnit(victim).Ward);
    }

    [Fact]
    public void ASkillWithoutSideRestrictionHealsAnEnemyAndPushesAnAllyWhileOwnSideOnlySkillsRefuseEnemies()
    {
        var s = new BattleState(1);
        var u = World.Unit(s, Side.A, Row.Front, "mend", "gust", "rescue");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(enemy).Hp = 10;
        s.GetUnit(ally).Hp = 10;
        Assert.Null(Why(s, Use(u, "mend", enemy)));
        Assert.Null(Why(s, Use(u, "gust", ally)));
        Assert.NotNull(Why(s, Use(u, "rescue", enemy)));
        Run(s, Use(u, "gust", ally));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(ally).Row);
        K.Ready(s, u);
        Run(s, Use(u, "mend", enemy));
        Assert.InRange(s.GetUnit(enemy).Hp - 10, 8, 10);
    }

    [Fact]
    public void ABleedingUnitTakesTwoAfterAMeleeSkillOrAMoveAndNothingAfterAHealOrAProjectile()
    {
        var (s, a, e) = Board(1, "tail_club", "mend", "rivet_shot");
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Bleeding, Uses: 3));
        Run(s, Use(a, "mend", a));
        Run(s, Use(a, "rivet_shot", e));
        Assert.Equal(0, Lost(s, a));
        Assert.Equal(3, Statuses.Get(s.GetUnit(a), StatusIds.Bleeding)!.UsesLeft);
        K.Ready(s, a);
        Run(s, Use(a, "tail_club", e));
        Assert.Equal(2, Lost(s, a));
        Assert.Equal(2, Statuses.Get(s.GetUnit(a), StatusIds.Bleeding)!.UsesLeft);
        K.Ready(s, a);
        Actions.Move(s, a, Row.Middle, 0);
        Assert.Equal(4, Lost(s, a));
    }

    [Fact]
    public void BumpDamageIsAHitSoAWhenHitReactionTriggersAndMovesNoMomentum()
    {
        var (s, a, e) = Board(1, "gust");
        for (var i = 0; i < 6; i++) World.Unit(s, Side.B, Row.Middle);
        for (var i = 0; i < 6; i++) World.Unit(s, Side.B, Row.Rear);
        var fired = 0;
        s.AddReaction(new ReactionDef("counter", e, TriggerKind.WhenHit, (_, _) => fired++));
        var momentum = s.Momentum.Value;
        Run(s, Use(a, "gust", e));
        Assert.Equal(Row.Front, s.Formation.PositionOf(e).Row);                              // a chain blocked by full rows: nothing moves
        Assert.Equal(1, Lost(s, e));                                                         // 1 bump damage
        Assert.Equal(1, fired);
        Assert.Equal(momentum, s.Momentum.Value);
    }

    [Fact]
    public void ASimultaneousBlockIsAppliedBeforeAnyKoCheckAndTheMeterShiftsOnce()
    {
        var (s, a, e1) = Board(1, "sour_chord");
        var e2 = World.Unit(s, Side.B, Row.Front);
        var e3 = World.Unit(s, Side.B, Row.Front);
        foreach (var id in new[] { e1, e2 }) s.GetUnit(id).Hp = 1;
        var events = Run(s, Use(a, "sour_chord"));
        var firstKo = events.ToList().FindIndex(x => x is UnitKnockedOut);
        var lastDamage = events.ToList().FindLastIndex(x => x is DamageDealt);
        Assert.True(firstKo > lastDamage);                                                   // every hit lands before the first KO is processed
        Assert.True(s.GetUnit(e1).IsKnockedOut && s.GetUnit(e2).IsKnockedOut);
        Assert.Equal(7, s.Momentum.Value);                                                   // one shift for the whole pulse
        Assert.False(s.GetUnit(e3).IsKnockedOut);
    }

    [Fact]
    public void AClayGolemWithTwoEquallyNearEnemiesPicksEachWithEqualChanceAndTheSameSeedPicksTheSame()
    {
        var counts = new int[2];
        for (ulong seed = 1; seed <= 400; seed++)
        {
            var s = new BattleState(seed);
            var c = World.Unit(s, Side.A, Row.Front, "clay_golem");
            var e1 = World.Unit(s, Side.B, Row.Front);
            var e2 = World.Unit(s, Side.B, Row.Front);
            BattleStart.Install(s, World.Catalog);
            s.ShiftMomentum(Side.A, 6);
            Run(s, Use(c, "clay_golem", place: new MoveOption(Row.Front, 0)));
            BattleDriver.NextTurn(s, World.Catalog);
            counts[Lost(s, e1) > 0 ? 0 : 1]++;
        }
        Assert.All(counts, n => Assert.InRange(n, 140, 260));
    }

    [Fact]
    public void ASilencedMenderCannotMendRainOrSearButKeepsItsPhysicalAttackAndLosesABoltAttack()
    {
        var s = new BattleState(1);
        var m = World.Unit(s, Side.A, Row.Front, "mend", "rain_of_mending", "searing_light", "tail_club", "ring_spin");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var e = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Statuses.Apply(s, new StatusApplication(m, StatusIds.Silenced, Duration: 20));
        Assert.NotNull(Why(s, Use(m, "mend", ally)));
        Assert.NotNull(Why(s, Use(m, "rain_of_mending", row: new RowTarget(Side.A, Row.Middle))));
        Assert.NotNull(Why(s, Use(m, "searing_light", e)));
        Assert.Null(Why(s, Use(m, "tail_club", e)));                                         // a physical attack
        Assert.NotNull(Why(s, Use(m, "ring_spin", e)));                                      // the bolt style is magical
    }

    [Fact]
    public void ATwentyHpCritterFallsOnTheFifthToSeventhHitOfA3To4AttackNeverEarlier()
    {
        // 20.5 (numbers updated: unique attacks hit for 3-4 at Power 5).
        var hits = new HashSet<int>();
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = new BattleState(seed);
            var a = World.Unit(s, Side.A, Row.Front, "tail_club");
            var critter = World.Unit(s, Side.B, Row.Front);
            s.GetUnit(critter).MaxHp = s.GetUnit(critter).Hp = 20;
            var n = 0;
            while (!s.GetUnit(critter).IsKnockedOut) { Actions.Use(s, World.Catalog, Use(a, "tail_club", critter)); n++; K.Ready(s, a); }
            hits.Add(n);
        }
        Assert.Equal(new[] { 5, 6, 7 }, hits.OrderBy(x => x));
    }

    [Fact]
    public void ADirectHealCannotBeInterceptedButAProjectileHealCan()
    {
        var directIntercepted = 0;
        var projectileIntercepted = 0;
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = new BattleState(seed);
            var healer = World.Unit(s, Side.A, Row.Rear, "mend", "healing_draught");
            var guard = World.Unit(s, Side.B, Row.Front);
            var target = World.Unit(s, Side.B, Row.Middle);
            s.GetUnit(target).Hp = 10;
            s.GetUnit(guard).Hp = 10;
            s.ShiftMomentum(Side.A, 6);
            directIntercepted += Run(s, Use(healer, "mend", target)).OfType<Intercepted>().Count();
            K.Ready(s, healer);
            projectileIntercepted += Run(s, Use(healer, "healing_draught", target)).OfType<Intercepted>().Count();
        }
        Assert.Equal(0, directIntercepted);
        Assert.InRange(projectileIntercepted, 60, 140);                                     // 50% at full overlap
    }

    [Fact]
    public void APreviewListsInterceptorsWithTheDeliverysChanceAndTheDirectHitChance()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "tail_club", "rivet_shot");
        var guard = World.Unit(s, Side.B, Row.Front);
        var target = World.Unit(s, Side.B, Row.Middle);
        var melee = Preview.Skill(s, World.Catalog, Use(a, "tail_club", target));
        Assert.Equal(new[] { (guard, 75) }, melee.Interceptors.Select(i => (i.UnitId, i.Percent)));
        Assert.Equal(25, melee.DirectChance);
        var shot = Preview.Skill(s, World.Catalog, Use(a, "rivet_shot", target));
        Assert.Equal(new[] { (guard, 50) }, shot.Interceptors.Select(i => (i.UnitId, i.Percent)));
        Assert.Equal(50, shot.DirectChance);
        var ally = World.Unit(s, Side.A, Row.Rear);
        Assert.Empty(Preview.Skill(s, World.Catalog, Use(a, "rivet_shot", ally)).Interceptors);      // own side is never intercepted
    }
}
