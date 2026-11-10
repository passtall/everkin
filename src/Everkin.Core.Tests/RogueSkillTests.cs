using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class RogueSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, int? second = null) => new(user, skill, target, null, null, second);

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

    /// <summary>A Rogue in the Front row with a companion behind it, and an enemy in the Front row; the battle is installed.</summary>
    private static (BattleState S, int Rogue, int Enemy) Board(ulong seed = 1, Row row = Row.Front, params string[] skills)
    {
        var s = new BattleState(seed);
        var rogue = World.Unit(s, Side.A, row, skills);
        World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, rogue, enemy);
    }

    [Fact]
    public void AllFourteenRogueSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "rogue").ToList();
        Assert.Equal(14, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            Assert.False(skill.Magical);
        }
    }

    // ---- Thief ----

    [Fact]
    public void HideNeedsTheRogueOutsideTheFrontRowAndMeltAwayWorksAnywhere()
    {
        var (s, rogue, _) = Board(1, Row.Front, "hide", "melt_away");
        Assert.NotNull(Why(s, Use(rogue, "hide")));
        Run(s, Use(rogue, "melt_away"));
        Assert.True(s.GetUnit(rogue).IsStealthed);

        var (s2, rogue2, _) = Board(1, Row.Middle, "hide");
        Run(s2, Use(rogue2, "hide"));
        Assert.True(s2.GetUnit(rogue2).IsStealthed);
    }

    [Fact]
    public void SneakLetsAStealthedRogueStepOneRowAtTheStartOfItsTurnOnceAndKeepsStealth()
    {
        var (s, rogue, _) = Board(1, Row.Middle, "hide", "sneak");
        Assert.Empty(Actions.SneakOptions(s, rogue));                          // not stealthed, not its turn
        Run(s, Use(rogue, "hide"));
        s.GetUnit(rogue).SneakReady = true;                                    // its next turn has started
        var options = Actions.SneakOptions(s, rogue);
        Assert.NotEmpty(options);
        Assert.All(options, o => Assert.NotEqual(Row.Middle, o.Row));
        var due = s.Timeline.TurnOf(rogue)!.Due;
        Actions.SneakMove(s, rogue, Row.Rear, 0);
        Assert.Equal(Row.Rear, s.Formation.PositionOf(rogue).Row);
        Assert.True(s.GetUnit(rogue).IsStealthed);
        Assert.Equal(due, s.Timeline.TurnOf(rogue)!.Due);
        Assert.Empty(Actions.SneakOptions(s, rogue));                          // once per turn
    }

    [Fact]
    public void LightFingersStealsOneHelpfulStatusWhenTheHitReachesTheEnemy()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "brawl_strike", "hide", "sneak", "light_fingers");
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Haste, Duration: 30, Magnitude: 2));
        Run(s, Use(rogue, "brawl_strike", enemy));
        Assert.False(Has(s, enemy, StatusIds.Haste));
        Assert.True(Has(s, rogue, StatusIds.Haste));
        Assert.Equal(30, Statuses.Get(s.GetUnit(rogue), StatusIds.Haste)!.EndsAt);

        Ready(s, rogue);                                                       // nothing left to steal: nothing happens
        Run(s, Use(rogue, "brawl_strike", enemy));
        Assert.True(Has(s, rogue, StatusIds.Haste));
    }

    [Fact]
    public void LiftASkillCopiesTheTargetsLastActiveSkillForFreeWithTheRoguesPower()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "hide", "sneak", "light_fingers", "lift_a_skill", "brawl_strike");
        var caster = World.Unit(s, Side.B, Row.Front, "mend");
        s.GetUnit(caster).Hp = 10;
        s.GetUnit(rogue).Hp = 10;
        s.ShiftMomentum(Side.B, 12);
        Assert.NotNull(Why(s, Use(rogue, "lift_a_skill", caster)));            // it has used nothing yet
        Run(s, Use(caster, "mend", caster));
        Ready(s, caster);
        s.ShiftMomentum(Side.A, 12);
        Assert.NotNull(Why(s, Use(rogue, "lift_a_skill", caster)));            // the copy needs a target
        var ally = World.Unit(s, Side.A, Row.Front);
        s.GetUnit(ally).Hp = 10;
        var momentum = s.Momentum.AdvantageOf(Side.A);
        Assert.Null(Why(s, Use(rogue, "lift_a_skill", caster, ally)));
        Run(s, Use(rogue, "lift_a_skill", caster, ally));
        Assert.InRange(s.GetUnit(ally).Hp - 10, 8, 10);                        // Mend's 8–10 at Power 5
        Assert.Equal(momentum - 3, s.Momentum.AdvantageOf(Side.A));            // only Lift's own cost
        Assert.Equal(Timeline.Wait(6, 5), s.Timeline.TurnOf(rogue)!.Due);      // only Lift's own Delay
        Assert.Equal(enemy, enemy);
    }

    // ---- Assassin ----

    [Fact]
    public void BackstabNeedsStealthSilencesAndMakesTheTargetBleedWithoutBreakingStealth()
    {
        var (s, rogue, enemy) = Board(1, Row.Middle, "hide", "mark_for_death", "backstab");
        var front = World.Unit(s, Side.A, Row.Front);
        Assert.Equal(front, front);
        Assert.NotNull(Why(s, Use(rogue, "backstab", enemy)));                 // not stealthed
        Run(s, Use(rogue, "hide"));
        Ready(s, rogue);
        // From the Middle row the enemy Front row is at distance 2: move to the Front to reach with D1.
        Movement.Move(s, rogue, Row.Front, 0);
        s.ShiftMomentum(Side.A, 4);
        var intercepted = 0;
        Run(s, Use(rogue, "backstab", enemy));
        var receiver = Has(s, enemy, StatusIds.Silenced) ? enemy : s.Units.First(u => Has(s, u.Id, StatusIds.Silenced)).Id;
        if (receiver != enemy) intercepted++;
        Assert.Equal(20, Statuses.Get(s.GetUnit(receiver), StatusIds.Silenced)!.EndsAt);
        Assert.Equal(3, Statuses.Get(s.GetUnit(receiver), StatusIds.Bleeding)!.UsesLeft);
        Assert.Equal(40, s.GetUnit(receiver).Hp);                              // no damage
        Assert.True(s.GetUnit(rogue).IsStealthed);
        Assert.Equal(0, intercepted);
    }

    [Fact]
    public void ContractDoublesDamageToMarkedEnemiesAndHalvesItForOthers()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "mark_for_death", "contract", "brawl_strike");
        var other = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(rogue, "brawl_strike", other));
        Assert.InRange(40 - s.GetUnit(other).Hp, 2, 2);                        // 4–5 halved, rounded down
        Ready(s, rogue);
        Run(s, Use(rogue, "mark_for_death", enemy));
        Ready(s, rogue);
        Run(s, Use(rogue, "brawl_strike", enemy));
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 8, 10);
    }

    [Fact]
    public void PoisonVialPutsPoisonGroundOnTheSpotForTwentyTimeUnits()
    {
        for (ulong seed = 1; seed <= 20; seed++)
        {
            var (s, rogue, enemy) = Board(seed, Row.Front, "mark_for_death", "poison_vial");
            var events = Run(s, Use(rogue, "poison_vial", enemy));
            var effect = s.Ground.Effects.Single(e => e.Kind == GroundKind.Poison);
            Assert.Equal(20, effect.EndsAt);
            if (!events.OfType<Intercepted>().Any()) Assert.Equal(s.Ground.SpotOf(enemy), effect.Spot);
        }
    }

    [Fact]
    public void MarkForDeathMarksForTwentyTimeUnits()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "mark_for_death");
        Run(s, Use(rogue, "mark_for_death", enemy));
        Assert.Equal(20, Statuses.Get(s.GetUnit(enemy), StatusIds.Marked)!.EndsAt);
        AdvanceTo(s, 21);
        Assert.False(Has(s, enemy, StatusIds.Marked));
    }

    [Fact]
    public void DeathmarkBloomPassesTheMarkToTheNearestEnemyWhenAMarkedEnemyFalls()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "mark_for_death", "poison_vial", "backstab", "deathmark_bloom", "brawl_strike");
        var neighbour = World.Unit(s, Side.B, Row.Front);
        var far = World.Unit(s, Side.B, Row.Rear);
        s.GetUnit(enemy).Hp = 1;
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Marked, SourceUnitId: rogue));
        Run(s, Use(rogue, "brawl_strike", enemy));
        Assert.True(s.GetUnit(enemy).IsKnockedOut);
        Assert.True(Has(s, neighbour, StatusIds.Marked));
        Assert.False(Has(s, far, StatusIds.Marked));
        Assert.Equal(s.Timeline.Now + 20, Statuses.Get(s.GetUnit(neighbour), StatusIds.Marked)!.EndsAt);
    }

    // ---- Saboteur ----

    [Fact]
    public void FlashPowderBlindsEveryEnemyInMeleeReachForTwentyTimeUnits()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "jam", "flash_powder");
        var b = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(rogue, "flash_powder"));
        Assert.Equal(20, Statuses.Get(s.GetUnit(enemy), StatusIds.Blind)!.EndsAt);
        Assert.True(Has(s, b, StatusIds.Blind));
        Assert.False(Has(s, back, StatusIds.Blind));
    }

    [Fact]
    public void JamAddsThreeDelayToTheTargetsNextSkillOrTheInterceptors()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "jam");
        s.GetUnit(enemy).LearnSkill("brawl_strike");
        var events = Run(s, Use(rogue, "jam", enemy));
        var receiver = events.OfType<Intercepted>().FirstOrDefault()?.InterceptorUnitId ?? enemy;
        Assert.Equal(3, Statuses.Get(s.GetUnit(receiver), StatusIds.NextSkillDelay)!.Magnitude);
        Assert.Equal(enemy, receiver);                                         // nobody stands in front of a Front unit
        Run(s, Use(enemy, "brawl_strike", rogue));
        Assert.Equal(Timeline.Wait(6, 5), s.Timeline.TurnOf(enemy)!.Due);     // Delay 3 + 3
    }

    [Fact]
    public void CutTheStringsRemovesTheNextAutoEventOfTheChosenEnemyUnit()
    {
        var (s, rogue, enemy) = Board(1, Row.Front, "jam", "cut_the_strings");
        Assert.NotNull(Why(s, Use(rogue, "cut_the_strings", enemy)));          // owns no auto event
        var first = s.Timeline.Add(TimelineKind.AutoEvent, 4, enemy, "shot");
        var second = s.Timeline.Add(TimelineKind.AutoEvent, 9, enemy, "shot");
        s.ShiftMomentum(Side.A, 4);
        Assert.Null(Why(s, Use(rogue, "cut_the_strings", enemy)));
        Run(s, Use(rogue, "cut_the_strings", enemy));
        Assert.DoesNotContain(s.Timeline.Upcoming(), e => e.Id == first.Id);
        Assert.Contains(s.Timeline.Upcoming(), e => e.Id == second.Id);
    }

    [Fact]
    public void DisarmTrapLetsTheRogueHitAnEnemyTrapWithoutItsEffectAndUsesItUp()
    {
        var s = new BattleState(1);
        var hunter = World.Unit(s, Side.B, Row.Front, "net_trap");
        var rogue = World.Unit(s, Side.A, Row.Front, "brawl_strike", "jam", "cut_the_strings", "disarm_trap");
        var plain = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.B, 3);
        var place = World.Catalog.Get("net_trap").PlacementOptions(s, hunter)[0];
        Run(s, new SkillUse(hunter, "net_trap", Placement: place));
        var trap = s.Units.Single(u => u.CreatureId == HunterSkills.NetTrapDefinition).Id;

        Run(s, Use(plain, "brawl_strike", trap));
        Assert.True(Has(s, plain, StatusIds.NextSkillDelay) || s.GetUnit(trap).IsRemoved);   // the plain unit pays for it

        var s2 = new BattleState(1);
        var hunter2 = World.Unit(s2, Side.B, Row.Front, "net_trap");
        var rogue2 = World.Unit(s2, Side.A, Row.Front, "brawl_strike", "disarm_trap");
        World.Unit(s2, Side.B, Row.Front);
        BattleStart.Install(s2, World.Catalog);
        s2.ShiftMomentum(Side.B, 3);
        Run(s2, new SkillUse(hunter2, "net_trap", Placement: World.Catalog.Get("net_trap").PlacementOptions(s2, hunter2)[0]));
        var trap2 = s2.Units.Single(u => u.CreatureId == HunterSkills.NetTrapDefinition).Id;
        Run(s2, Use(rogue2, "brawl_strike", trap2));
        Assert.False(Has(s2, rogue2, StatusIds.NextSkillDelay));
        Assert.True(s2.GetUnit(trap2).IsRemoved);
        Assert.Equal(rogue, rogue);
    }
}
