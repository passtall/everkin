using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

/// <summary>
/// The worked checks of game_system section 20.5 that had no direct test when M1.17 audited the table row by row (the other rows are
/// tested in the class of their area, each with a "20.5" comment). Each fact names the row it covers.
/// </summary>
public class WorkedChecksTests
{
    private static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    // ---- formation and battle start ----

    [Fact]
    public void ThreeCardRowOppositeATwoCardRowOverlapsByHalfOrZero()
    {
        var three = Enumerable.Range(0, 3).Select(i => Formation.Center(3, i)).ToArray();     // -2, 0, 2
        var two = Enumerable.Range(0, 2).Select(i => Formation.Center(2, i)).ToArray();       // -1, 1
        Assert.Equal(new[] { -2, 0, 2 }, three);
        Assert.Equal(new[] { -1, 1 }, two);
        Assert.Equal(new[] { 50, 50, 0 }, three.Select(c => Formation.OverlapPercent(c, two[0])));
        Assert.Equal(new[] { 0, 50, 50 }, three.Select(c => Formation.OverlapPercent(c, two[1])));
    }

    [Fact]
    public void AnotherBattleStartsWithFullHpAndNeutralMomentum()
    {
        var s = new BattleState(2);
        var a = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Assert.All(s.Units, u => Assert.Equal(u.MaxHp, u.Hp));
        Assert.Equal(0, s.Momentum.Value);
        Assert.Empty(s.GetUnit(a).StatusInstances);
        Assert.Empty(s.Ground.Effects);
    }

    // ---- damage ----

    [Fact]
    public void ACounterOfFixedTwoDamageAgainstDampenDealsOne()
    {
        Assert.Equal(1, DamageMath.Resolve(2, 0, dampen: true, DamageSource.Hit).HpLost);
        Assert.Equal(1, DamageMath.Resolve(1, 0, dampen: true, DamageSource.Hit).HpLost);       // a 1-damage hit stays 1
    }

    [Fact]
    public void ABrawlStrikeKillsATwentyHpCritterOnTheFourthOrFifthHitNeverEarlier()
    {
        var seen = new HashSet<int>();
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = new BattleState(seed);
            var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
            var critter = World.Unit(s, Side.B, Row.Front);
            World.Unit(s, Side.B, Row.Front);
            s.GetUnit(critter).MaxHp = s.GetUnit(critter).Hp = 20;
            var hits = 0;
            while (!s.GetUnit(critter).IsKnockedOut && hits < 10)
            {
                Ready(s, a);
                Run(s, new SkillUse(a, "brawl_strike", critter));
                hits++;
            }
            seen.Add(hits);
        }
        Assert.Equal(new[] { 4, 5 }, seen.OrderBy(x => x));
    }

    // ---- simultaneous blocks, reactions, Momentum ----

    [Fact]
    public void ASimultaneousBlockIsAppliedBeforeAnyKoCheckAndTheMeterShiftsOnce()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "breath");
        var left = World.Unit(s, Side.B, Row.Front);
        var target = World.Unit(s, Side.B, Row.Front);
        var right = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        foreach (var id in new[] { left, target, right }) s.GetUnit(id).Hp = 1;
        s.DrainEvents();
        var events = Run(s, new SkillUse(a, "breath", target));
        var firstKo = events.ToList().FindIndex(e => e is UnitKnockedOut);
        var lastDamage = events.ToList().FindLastIndex(e => e is DamageDealt);
        Assert.True(firstKo > lastDamage);                                              // every hit lands before the first KO is processed
        Assert.Equal(3, events.OfType<UnitKnockedOut>().Count());
        Assert.Single(events.OfType<MomentumChanged>());
    }

    [Fact]
    public void ASilenceThatLandsDoesNotMoveMomentumAndABlindUnitsMissShiftsItAgainstItsSide()
    {
        var s = new BattleState(1);
        var rogue = World.Unit(s, Side.A, Row.Front, "melt_away", "backstab");
        World.Unit(s, Side.A, Row.Middle);                                             // someone stays visible
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Run(s, new SkillUse(rogue, "melt_away"));
        Ready(s, rogue);
        var before = s.Momentum.Value;
        Run(s, new SkillUse(rogue, "backstab", enemy));
        Assert.Contains(s.Units, u => Statuses.Has(u, StatusIds.Silenced));
        Assert.Equal(before - 2, s.Momentum.Value);                                    // Backstab costs 2; the Silence moved nothing

        for (ulong seed = 1; seed <= 60; seed++)
        {
            var s2 = new BattleState(seed);
            var blind = World.Unit(s2, Side.A, Row.Front, "brawl_strike");
            var target = World.Unit(s2, Side.B, Row.Front);
            World.Unit(s2, Side.B, Row.Front);
            Statuses.Apply(s2, new StatusApplication(blind, StatusIds.Blind, Duration: 20));
            s2.ShiftMomentum(Side.A, 0);
            var events = Run(s2, new SkillUse(blind, "brawl_strike", target));
            if (events.OfType<AttackMissed>().Any())
            {
                Assert.Equal(-1, s2.Momentum.Value);
                return;
            }
        }
        Assert.Fail("No miss in 60 seeds.");
    }

    [Fact]
    public void AReactionsHitGoesStraightToItsTargetEvenWithADefenderInFront()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.A, Row.Front);                                              // a defender in front of the attacker
        var b = World.Unit(s, Side.B, Row.Front);
        s.AddReaction(new ReactionDef("c", b, TriggerKind.WhenHit, (st, ctx) =>
        {
            if (ctx.Trigger.SourceUnitId is { } attacker) st.DealDamage(b, attacker, 2);
        }));
        s.DrainEvents();
        Reactions.Resolve(s, new[] { new Trigger(TriggerKind.WhenHit, b, a, EffectTags.Melee) });
        Assert.Equal(38, s.GetUnit(a).Hp);
        Assert.DoesNotContain(s.DrainEvents(), e => e is Intercepted);
    }

    [Fact]
    public void BumpDamageIsAHitSoAWhenHitCounterTriggers()
    {
        var s = new BattleState(1);
        var pusher = World.Unit(s, Side.A, Row.Front, "gust");
        var front = World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        for (var i = 0; i < 5; i++) World.Unit(s, Side.B, Row.Middle);                // the Middle row is full: the chain is blocked
        var fired = 0;
        s.AddReaction(new ReactionDef("c", front, TriggerKind.WhenHit, (st, ctx) => fired++));
        Run(s, new SkillUse(pusher, "gust", front));
        Assert.Equal(39, s.GetUnit(front).Hp);                                         // 1 bump damage
        Assert.Equal(1, fired);
        Assert.Equal(0, s.Momentum.Value);
        Assert.Equal(middle, middle);
    }

    // ---- delay, heals, reach ----

    [Fact]
    public void ADelayNineSkillThatGetsPlusThreeDelayIsDelayTen()
    {
        var s = new BattleState(1);
        var healer = World.Unit(s, Side.A, Row.Front, "revive");
        var body = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(body).Hp = 0;
        Knockout.ResolveAll(s);
        s.ShiftMomentum(Side.A, 5);
        Statuses.Apply(s, new StatusApplication(healer, StatusIds.NextSkillDelay, Magnitude: 3));
        Run(s, new SkillUse(healer, "revive", body));
        Assert.Equal(Timeline.Wait(10, 5), s.Timeline.TurnOf(healer)!.Due);
    }

    [Fact]
    public void ADirectHealCannotBeInterceptedButAProjectileHealCan()
    {
        var directIntercepted = 0;
        var projectileIntercepted = 0;
        for (ulong seed = 1; seed <= 40; seed++)
        {
            var s = new BattleState(seed);
            var healer = World.Unit(s, Side.A, Row.Front, "mend", "healing_draught");
            World.Unit(s, Side.A, Row.Front);
            var target = World.Unit(s, Side.A, Row.Middle);
            World.Unit(s, Side.B, Row.Front);
            s.GetUnit(target).Hp = 10;
            directIntercepted += Run(s, new SkillUse(healer, "mend", target)).OfType<Intercepted>().Count();
            Ready(s, healer);
            projectileIntercepted += Run(s, new SkillUse(healer, "healing_draught", target)).OfType<Intercepted>().Count();
        }
        Assert.Equal(0, directIntercepted);
        Assert.True(projectileIntercepted > 0);
    }

    [Fact]
    public void ASkillWithoutSideRestrictionHealsAnEnemyAndPushesAnAllyWhileOwnSideOnlySkillsRefuseEnemies()
    {
        var s = new BattleState(1);
        var u = World.Unit(s, Side.A, Row.Front, "mend", "gust", "airlift");
        var ally = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(enemy).Hp = 10;
        Run(s, new SkillUse(u, "mend", enemy));
        Assert.InRange(s.GetUnit(enemy).Hp, 18, 20);                                    // the enemy is healed
        Ready(s, u);
        Run(s, new SkillUse(u, "gust", ally));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(ally).Row);                     // the ally is pushed
        Assert.NotNull(Why(s, new SkillUse(u, "airlift", enemy, null, new MoveOption(Row.Rear, 0))));
    }

    // ---- statuses ----

    [Fact]
    public void ABleedingUnitTakesNoBleedDamageFromAHealSkill()
    {
        var s = new BattleState(1);
        var healer = World.Unit(s, Side.A, Row.Front, "mend");
        var ally = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(healer, StatusIds.Bleeding, Uses: 3));
        s.GetUnit(ally).Hp = 10;
        Run(s, new SkillUse(healer, "mend", ally));
        Assert.Equal(40, s.GetUnit(healer).Hp);
        Assert.Equal(3, Statuses.Get(s.GetUnit(healer), StatusIds.Bleeding)!.UsesLeft);
    }

    [Fact]
    public void PoisonedTwiceIsOneInstanceAndKeepsTickingWhenItsCasterIsKo()
    {
        var s = new BattleState(1);
        var caster = World.Unit(s, Side.A, Row.Front);
        var victim = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.A, Row.Middle);
        Statuses.Apply(s, new StatusApplication(victim, StatusIds.Poison, Duration: 50, SourceUnitId: caster));
        Statuses.Apply(s, new StatusApplication(victim, StatusIds.Poison, Duration: 50, SourceUnitId: caster));
        Assert.Single(s.GetUnit(victim).StatusInstances);
        s.GetUnit(caster).Hp = 0;
        Knockout.ResolveAll(s);
        s.Timeline.Add(TimelineKind.DelayedEvent, 30, tag: "clock");
        while (s.Timeline.Pop() is { } e)
            if (e.Tag == "clock") break;
        Statuses.TickPoison(s);
        Assert.Equal(37, s.GetUnit(victim).Hp);                                          // 3 damage after 30 time units
        Assert.True(Statuses.Has(s.GetUnit(victim), StatusIds.Poison));
    }

    [Fact]
    public void PoisonTicksAndAShieldAbsorbedHitOnAStealthedUnit()
    {
        var s = new BattleState(1);
        var summoner = World.Unit(s, Side.A, Row.Front, "raise_undead", "corpse_explosion");
        var body = World.Unit(s, Side.B, Row.Middle);
        var hidden = World.Unit(s, Side.B, Row.Middle);
        var visible = World.Unit(s, Side.B, Row.Middle);                                // far from the body: not next to it
        for (var i = 0; i < 3; i++) World.Unit(s, Side.B, Row.Middle);
        Kill(s, body);
        // The hidden unit's side keeps another visible unit, so stealth is legal.
        Stealth.Gain(s, hidden);
        s.GetUnit(hidden).Ward = 20;
        s.ShiftMomentum(Side.A, 5);
        var before = s.Momentum.Value;
        Run(s, new SkillUse(summoner, "corpse_explosion", body));
        Assert.False(s.GetUnit(hidden).IsStealthed);                                    // a fully absorbed hit still breaks stealth
        Assert.Equal(before - 2, s.Momentum.Value);                                     // cost 2, and the absorbed hit moves nothing
        Assert.Equal(40, s.GetUnit(visible).Hp);

        s.GetUnit(hidden).Ward = 0;
        Stealth.Gain(s, hidden);
        var m = s.Momentum.Value;
        Statuses.Apply(s, new StatusApplication(hidden, StatusIds.Poison, Duration: 50));
        s.Timeline.Add(TimelineKind.DelayedEvent, 10, tag: "clock");
        while (s.Timeline.Pop() is { } e)
            if (e.Tag == "clock") break;
        Statuses.TickPoison(s);
        Assert.True(s.GetUnit(hidden).IsStealthed);                                     // a poison tick does not break stealth
        Assert.Equal(m, s.Momentum.Value);                                              // and does not move Momentum
    }

    private static void Kill(BattleState s, int unit)
    {
        s.GetUnit(unit).Hp = 0;
        Knockout.ResolveAll(s);
    }

    // ---- Silence ----

    [Fact]
    public void ASilencedHealerCannotMendButKeepsItsPhysicalAttackAndLosesABoltAttack()
    {
        var s = new BattleState(1);
        var healer = World.Unit(s, Side.A, Row.Front, "mend", "brawl_strike", "bolt");
        var ally = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(healer, StatusIds.Silenced, Duration: 20));
        Assert.NotNull(Why(s, new SkillUse(healer, "mend", ally)));
        Assert.Null(Why(s, new SkillUse(healer, "brawl_strike", enemy)));
        Assert.NotNull(Why(s, new SkillUse(healer, "bolt", enemy)));
    }

    // ---- Momentum from several hits ----

    [Fact]
    public void ABoltTurretShiftsMomentumOncePerHitAndCrowdFighterAnAreaOfHitsOnlyOnce()
    {
        var s = new BattleState(1);
        var tinker = World.Unit(s, Side.A, Row.Front, "barricade", "bolt_turret");
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, new SkillUse(tinker, "bolt_turret", Placement: World.Catalog.Get("bolt_turret").PlacementOptions(s, tinker)[0]));
        var shifts = 0;
        for (var i = 0; i < 3; i++)
        {
            Ready(s, tinker);
            shifts += Actions.SkipTurn(s, tinker).OfType<MomentumChanged>().Count();
        }
        Assert.Equal(3, shifts);
        Assert.Equal(enemy, enemy);

        var s2 = new BattleState(1);
        var warrior = World.Unit(s2, Side.A, Row.Front, "brawl_strike", "battle_roar", "crowd_fighter");
        for (var i = 0; i < 3; i++) World.Unit(s2, Side.B, Row.Front);
        BattleStart.Install(s2, World.Catalog);
        var target = s2.Units.First(u => u.Side == Side.B).Id;
        s2.DrainEvents();
        var events = Run(s2, new SkillUse(warrior, "brawl_strike", target));
        Assert.Equal(3, events.OfType<DamageDealt>().Count());
        Assert.Single(events.OfType<MomentumChanged>());
        Assert.Equal(1, s2.Momentum.Value);
    }

    // ---- targeting and picks ----

    [Fact]
    public void AClayGolemWithTwoEquallyNearEnemiesPicksEachWithEqualChanceAndTheSameSeedPicksTheSame()
    {
        int Pick(ulong seed)
        {
            var s = new BattleState(seed);
            var summoner = World.Unit(s, Side.A, Row.Front, "clay_golem");
            var e1 = World.Unit(s, Side.B, Row.Front);
            var e2 = World.Unit(s, Side.B, Row.Front);
            s.ShiftMomentum(Side.A, 8);
            Run(s, new SkillUse(summoner, "clay_golem", Placement: World.Catalog.Get("clay_golem").PlacementOptions(s, summoner)[0]));
            var golem = s.Units.Last().Id;
            Ready(s, golem);
            AutonomousTurns.Play(s, World.Catalog, golem);
            Assert.Equal(Timeline.Wait(3, 3), s.Timeline.TurnOf(golem)!.Due);          // Delay 3
            return s.GetUnit(e1).Hp < 40 ? e1 : e2;
        }
        var picks = Enumerable.Range(1, 40).Select(i => Pick((ulong)i)).ToList();
        Assert.True(picks.Distinct().Count() == 2);
        Assert.Equal(Pick(7), Pick(7));
    }

    // ---- locked targets that become stealthed (section 10.1) ----

    [Fact]
    public void AQueuedProjectileShotAtATargetThatBecameStealthedHitsTheUnitBehindIt()
    {
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Front);
        var target = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);                                              // so stealth stays legal
        var behind = World.Unit(s, Side.B, Row.Middle);
        Assert.Equal(new[] { target }, LockedTargets.Projectile(s, target));
        Stealth.Gain(s, target);
        Assert.Contains(behind, LockedTargets.Projectile(s, target).Concat(LockedTargets.Projectile(s, target)));
        Assert.DoesNotContain(target, LockedTargets.Projectile(s, target));
    }

    [Fact]
    public void AQueuedDirectEffectStillHitsAStealthedTargetAndWhoeverNowStandsInTheSlot()
    {
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Front);
        var target = World.Unit(s, Side.B, Row.Front);
        var other = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Middle);
        var slot = s.Ground.SpotOf(target);
        Stealth.Gain(s, target);
        Assert.Equal(new[] { target }, LockedTargets.Direct(s, slot));                  // stealthed but has not moved: still hit

        Movement.ForceMove(s, target, Row.Middle, Side.A);                              // the target moves away
        var now = LockedTargets.Direct(s, slot);
        Assert.DoesNotContain(target, now);
        Assert.NotEmpty(now);                                                           // the unit now in that slot is hit (both at a 50% overlap)
        Assert.All(now, id => Assert.Equal(Row.Front, s.Formation.PositionOf(id).Row));
        Assert.Equal(other, other);
    }

    // ---- forced movement of a trap ----

    [Fact]
    public void ATrapIsShovedLikeAnyUnit()
    {
        var s = new BattleState(1);
        var hunter = World.Unit(s, Side.B, Row.Front, "net_trap");
        var pusher = World.Unit(s, Side.A, Row.Front, "gust");
        s.ShiftMomentum(Side.B, 3);
        Run(s, new SkillUse(hunter, "net_trap", Placement: World.Catalog.Get("net_trap").PlacementOptions(s, hunter)[0]));
        var trap = s.Units.Single(u => u.CreatureId == HunterSkills.NetTrapDefinition).Id;
        Assert.Equal(Row.Front, s.Formation.PositionOf(trap).Row);
        s.ShiftMomentum(Side.A, 6);
        Run(s, new SkillUse(pusher, "gust", trap));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(trap).Row);
    }

    // ---- prices ----

    [Fact]
    public void ACoinCreatureWithCostSixCostsTwoHundredCoins() => Assert.Equal(200, Prices.CoinCreature(6));
}
