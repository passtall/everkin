using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class WarriorSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null) => new(user, skill, target);

    private static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static bool Has(BattleState s, int unit, string status) => Statuses.Has(s.GetUnit(unit), status);

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

    /// <summary>A Warrior in the Front with the given skills, one enemy in the Front, battle installed.</summary>
    private static (BattleState S, int Warrior, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var w = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, w, enemy);
    }

    [Fact]
    public void AllSixteenWarriorSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "warrior").ToList();
        Assert.Equal(16, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            Assert.False(skill.Magical);
        }
    }

    // ---- Berserker ----

    [Fact]
    public void RecklessSwingDoublesTheWarriorsHitsAndItTakesHalfOfWhatWasLost()
    {
        var (s, w, enemy) = Board(1, "brawl_strike", "endure", "reckless_swing");
        Run(s, Use(w, "brawl_strike", enemy));
        var dealt = 40 - s.GetUnit(enemy).Hp;
        Assert.InRange(dealt, 8, 10);
        Assert.Equal(dealt / 2, 40 - s.GetUnit(w).Hp);
    }

    [Fact]
    public void LastStandAddsFiftyPercentAndTheBonusesAddUp()
    {
        var (s, w, enemy) = Board(1, "brawl_strike", "endure", "unstoppable", "last_stand");
        Run(s, Use(w, "brawl_strike", enemy));
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 6, 7);                          // 4–5 × 150%

        var (s2, w2, e2) = Board(1, "brawl_strike", "endure", "reckless_swing", "unstoppable", "last_stand");
        Run(s2, Use(w2, "brawl_strike", e2));
        Assert.InRange(40 - s2.GetUnit(e2).Hp, 10, 12);                          // 4–5 × 250%
    }

    [Fact]
    public void LastStandStopsMovingAndBeingMoved()
    {
        var (s, w, enemy) = Board(1, "endure", "unstoppable", "last_stand");
        Assert.Empty(Movement.LegalMoves(s, w));
        Assert.Throws<InvalidOperationException>(() => Actions.Move(s, w, Row.Middle, 0));
        Assert.Equal(ForcedMoveStatus.NotMovable, Movement.ForceMove(s, w, Row.Middle, Side.B).Status);
        Assert.Equal(Row.Front, s.Formation.PositionOf(w).Row);
        Assert.Equal(enemy, enemy);
    }

    [Fact]
    public void EndureKeepsStatusDamageFromKillingButNotHits()
    {
        var (s, w, enemy) = Board(1, "endure");
        s.GetUnit(w).Hp = 2;
        s.DealDamage(null, w, 5, DamageSource.Periodic);
        Assert.Equal(1, s.GetUnit(w).Hp);
        s.DealDamage(null, w, 5);
        Assert.Equal(0, s.GetUnit(w).Hp);
        Assert.Equal(40, s.GetUnit(enemy).Hp);
    }

    [Fact]
    public void UnstoppableBelowAQuarterRefusesImmobilizedAndFrozenAndPushBacks()
    {
        var (s, w, _) = Board(1, "endure", "unstoppable");
        Assert.True(Statuses.Apply(s, new StatusApplication(w, StatusIds.Frozen)));       // healthy: works
        Statuses.Remove(s, w, StatusIds.Frozen, StatusEndCause.Consumed);
        s.GetUnit(w).Hp = 9;                                                              // 9 × 4 < 40
        Assert.False(Statuses.Apply(s, new StatusApplication(w, StatusIds.Immobilized, Duration: 10)));
        Assert.False(Statuses.Apply(s, new StatusApplication(w, StatusIds.Frozen)));
        Assert.True(Statuses.Apply(s, new StatusApplication(w, StatusIds.Blind, Duration: 10)));
        s.Timeline.ScheduleTurn(w, 5, 5);
        var due = s.Timeline.TurnOf(w)!.Due;
        Assert.False(s.PushTurnBack(w, 5));
        Assert.Equal(due, s.Timeline.TurnOf(w)!.Due);
    }

    // ---- Champion ----

    [Fact]
    public void ChallengeMarksOneUnitAtATime()
    {
        var (s, w, enemy) = Board(1, "challenge");
        var other = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(w, "challenge", enemy));
        Assert.True(Has(s, enemy, StatusIds.Marked));
        Ready(s, w);
        Run(s, Use(w, "challenge", other));
        Assert.True(Has(s, other, StatusIds.Marked));
        Assert.False(Has(s, enemy, StatusIds.Marked));
    }

    [Fact]
    public void DragOutPullsAMarkedUnitOneRowTowardTheFrontOrAnInterceptorTakesIt()
    {
        var pulled = 0;
        var intercepted = 0;
        for (ulong seed = 1; seed <= 40; seed++)
        {
            var (s, w, _) = Board(seed, "challenge", "drag_out");
            World.Unit(s, Side.B, Row.Front);                                              // two Front units: the pulled one lands between them
            var middle = World.Unit(s, Side.B, Row.Middle);
            if (seed == 1) Assert.NotNull(Why(s, Use(w, "drag_out", middle)));          // not Marked yet
            Run(s, Use(w, "challenge", middle));
            Ready(s, w);
            var events = Run(s, Use(w, "drag_out", middle));
            if (s.Formation.PositionOf(middle).Row == Row.Front) pulled++;
            if (events.OfType<Intercepted>().Any()) { intercepted++; Assert.Equal(Row.Middle, s.Formation.PositionOf(middle).Row); }
        }
        Assert.True(pulled > 0 && intercepted > 0);
    }

    [Fact]
    public void IsolatePushesTheLeftAndRightNeighboursBackAndLeavesTheMarkedUnit()
    {
        var (s, w, _) = Board(1, "challenge", "drag_out", "isolate");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        var c = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(w, "challenge", b));
        Ready(s, w);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(w, "isolate", b));
        Assert.Equal(Row.Front, s.Formation.PositionOf(b).Row);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(a).Row);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(c).Row);
    }

    [Fact]
    public void HuntDownStunsAMarkedEnemyThatIsMovedForFiveAndOnlyThen()
    {
        var stunned = 0;
        for (ulong seed = 1; seed <= 40; seed++)
        {
            var (s, w, _) = Board(seed, "challenge", "drag_out", "hunt_down");
            World.Unit(s, Side.B, Row.Front);
            var back = World.Unit(s, Side.B, Row.Middle);
            Run(s, Use(w, "challenge", back));
            Ready(s, w);
            Assert.False(Has(s, back, StatusIds.Stunned));
            Run(s, Use(w, "drag_out", back));
            if (s.Formation.PositionOf(back).Row == Row.Front)
            {
                stunned++;
                Assert.Equal(s.Timeline.Now + 5, Statuses.Get(s.GetUnit(back), StatusIds.Stunned)!.EndsAt);
            }
            else Assert.False(Has(s, back, StatusIds.Stunned));
        }
        Assert.True(stunned > 0);
    }

    [Fact]
    public void ExecutionKillsAMarkedUnitBelowAQuarterAndOnlyThen()
    {
        var (s, w, enemy) = Board(1, "challenge", "drag_out", "single_combat", "execution");
        Run(s, Use(w, "challenge", enemy));
        Ready(s, w);
        s.ShiftMomentum(Side.A, 4);
        Assert.NotNull(Why(s, Use(w, "execution", enemy)));                      // 40 HP
        s.GetUnit(enemy).Hp = 9;
        s.GetUnit(enemy).Ward = 20;
        World.Unit(s, Side.B, Row.Front);                                         // keep the battle going
        Assert.Null(Why(s, Use(w, "execution", enemy)));
        Run(s, Use(w, "execution", enemy));
        Assert.True(s.GetUnit(enemy).IsKnockedOut);
    }

    [Fact]
    public void SingleCombatLetsOnlyTheTwoDuellistsTargetEachOther()
    {
        var (s, w, enemy) = Board(1, "challenge", "drag_out", "single_combat");
        var ally = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var other = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        Run(s, Use(w, "challenge", enemy));
        Ready(s, w);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(w, "single_combat", enemy));
        Assert.NotNull(Why(s, Use(ally, "brawl_strike", enemy)));                 // outsiders cannot target the enemy duellist
        Assert.NotNull(Why(s, Use(other, "brawl_strike", w)));                    // nor the Warrior
        s.GetUnit(enemy).LearnSkill("brawl_strike");
        Assert.Null(Why(s, Use(enemy, "brawl_strike", w)));                       // the duellists can
        Assert.Null(Why(s, Use(ally, "brawl_strike", other)));                    // others stay targetable
        AdvanceTo(s, 11);
        Assert.Null(Why(s, Use(ally, "brawl_strike", enemy)));                    // over after 10
    }

    // ---- Whirlwind ----

    [Fact]
    public void BattleRoarPushesBackTheTurnsOfEnemiesInMeleeReachByFive()
    {
        var (s, w, enemy) = Board(1, "battle_roar");
        var back = World.Unit(s, Side.B, Row.Middle);
        s.Timeline.ScheduleTurn(enemy, 5, 5);
        s.Timeline.ScheduleTurn(back, 5, 5);
        var due = s.Timeline.TurnOf(enemy)!.Due;
        Run(s, Use(w, "battle_roar"));
        Assert.Equal(due + 5, s.Timeline.TurnOf(enemy)!.Due);
        Assert.Equal(due, s.Timeline.TurnOf(back)!.Due);
    }

    [Fact]
    public void SweepingKickPushesTheNearestEnemyRowBackAsAGroup()
    {
        var (s, w, a) = Board(1, "battle_roar", "sweeping_kick");
        var b = World.Unit(s, Side.B, Row.Front);
        var behind = World.Unit(s, Side.B, Row.Rear);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(w, "sweeping_kick"));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(a).Row);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(b).Row);
        Assert.Equal(Row.Rear, s.Formation.PositionOf(behind).Row);
    }

    [Fact]
    public void CrowdFighterMakesAttacksHitEveryEnemyInReachWhenThereAreThreeOrMore()
    {
        var (s, w, a) = Board(1, "brawl_strike", "battle_roar", "crowd_fighter");
        var b = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(w, "brawl_strike", a));
        Assert.Equal(40, s.GetUnit(b).Hp);                                         // only two enemies: a normal hit

        var c = World.Unit(s, Side.B, Row.Front);
        var far = World.Unit(s, Side.B, Row.Middle);
        Ready(s, w);
        Run(s, Use(w, "brawl_strike", a));
        Assert.True(s.GetUnit(b).Hp < 40 && s.GetUnit(c).Hp < 40);
        Assert.Equal(40, s.GetUnit(far).Hp);
    }

    // ---- Footwork ----

    [Fact]
    public void SidestepMakesTheNextAttackMissOnceAndCounterStepPullsTheTurnForward()
    {
        var (s, w, enemy) = Board(1, "sidestep", "counter_step");
        s.GetUnit(enemy).LearnSkill("brawl_strike");
        Run(s, Use(w, "sidestep"));
        var before = s.Timeline.TurnOf(w)!.Due;
        var events = Run(s, Use(enemy, "brawl_strike", w));
        Assert.Single(events.OfType<AttackMissed>());
        Assert.Equal(40, s.GetUnit(w).Hp);
        Assert.False(Has(s, w, StatusIds.Sidestep));
        Assert.Equal(before - 5, s.Timeline.TurnOf(w)!.Due);

        Ready(s, enemy);
        Run(s, Use(enemy, "brawl_strike", w));                                     // used up: this one lands
        Assert.True(s.GetUnit(w).Hp < 40);
    }

    [Fact]
    public void HitAndRunStrikesPushesTheTargetBackThreeAndMovesTheWarriorOneRowBack()
    {
        var (s, w, enemy) = Board(1, "sidestep", "hit_and_run");
        s.Timeline.ScheduleTurn(enemy, 5, 5);
        var due = s.Timeline.TurnOf(enemy)!.Due;
        Run(s, Use(w, "hit_and_run", enemy));
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 3, 4);
        Assert.Equal(due + 3, s.Timeline.TurnOf(enemy)!.Due);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(w).Row);
    }

    [Fact]
    public void HitAndRunWithLastStandStaysInPlaceAndTheRestResolves()
    {
        var (s, w, enemy) = Board(1, "sidestep", "hit_and_run", "endure", "unstoppable", "last_stand");
        Run(s, Use(w, "hit_and_run", enemy));
        Assert.Equal(Row.Front, s.Formation.PositionOf(w).Row);
        Assert.True(s.GetUnit(enemy).Hp < 40);
    }
}
