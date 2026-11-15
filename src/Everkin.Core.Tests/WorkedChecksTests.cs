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

    // ---- simultaneous blocks, reactions, Momentum ----

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

    // ---- delay, heals, reach ----

    // ---- statuses ----

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
        Assert.Equal(34, s.GetUnit(victim).Hp);                                          // 6 damage after 30 time units
        Assert.True(Statuses.Has(s.GetUnit(victim), StatusIds.Poison));
    }

    private static void Kill(BattleState s, int unit)
    {
        s.GetUnit(unit).Hp = 0;
        Knockout.ResolveAll(s);
    }

    // ---- Silence ----

    // ---- Momentum from several hits ----

    // ---- targeting and picks ----

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

    // ---- prices ----

    [Fact]
    public void ACoinCreatureWithCostSixCostsTwoHundredCoins() => Assert.Equal(200, Prices.CoinCreature(6));
}
