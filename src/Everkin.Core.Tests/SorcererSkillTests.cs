using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class SorcererSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null) => new(user, skill, target);

    private static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static void AdvanceTo(BattleState s, int time)
    {
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            Statuses.HandleExpiry(s, e);
        }
    }

    private static bool Has(BattleState s, int unit, string status) => Statuses.Has(s.GetUnit(unit), status);

    /// <summary>A Sorcerer with the given skills in the Rear, an ally in the Front, and an enemy in the Front.</summary>
    private static (BattleState S, int Sorcerer, int Ally, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var sorcerer = World.Unit(s, Side.A, Row.Rear, skills);
        var ally = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 5);
        return (s, sorcerer, ally, enemy);
    }

    // ---- the catalog ----

    [Fact]
    public void AllFifteenSorcererSkillsAreImplementedAndEveryActiveIsMagical()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "sorcerer").ToList();
        Assert.Equal(15, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            if (def.Kind == "A") Assert.True(skill.Magical);
        }
    }

    // ---- Enchantment ----

    [Fact]
    public void PhaseWeaponGivesTheNextAttackUnlimitedRangeAndIsUsedUp()
    {
        var (s, sorc, ally, enemy) = Board(1, "phase_weapon");
        s.GetUnit(ally).LearnSkill("brawl_strike");
        var rear = World.Unit(s, Side.B, Row.Rear);
        Assert.NotNull(Why(s, Use(ally, "brawl_strike", rear)));          // melee D1 cannot reach the Rear row

        Run(s, Use(sorc, "phase_weapon", ally));
        Assert.True(Has(s, ally, StatusIds.Phased));
        Assert.Null(Why(s, Use(ally, "brawl_strike", rear)));
        Assert.Contains(rear, Actions.LegalTargets(s, World.Catalog, ally, "brawl_strike"));

        Run(s, Use(ally, "brawl_strike", rear));
        Assert.True(s.GetUnit(rear).Hp + s.GetUnit(enemy).Hp < 80);          // it landed on the Rear unit or its interceptor
        Assert.False(Has(s, ally, StatusIds.Phased));
        Assert.NotNull(Why(s, Use(ally, "brawl_strike", rear)));          // used up
    }

    [Fact]
    public void PhasingStrikeMakesTheEnchantedAttackImpossibleToIntercept()
    {
        int Intercepts(bool phasing)
        {
            var count = 0;
            for (ulong seed = 1; seed <= 60; seed++)
            {
                var s = new BattleState(seed);
                var sorc = World.Unit(s, Side.A, Row.Rear, phasing ? new[] { "phase_weapon", "phasing_strike" } : new[] { "phase_weapon" });
                var attacker = World.Unit(s, Side.A, Row.Front, "brawl_strike");
                World.Unit(s, Side.B, Row.Front);                           // the defender
                var target = World.Unit(s, Side.B, Row.Middle);
                Run(s, Use(sorc, "phase_weapon", attacker));
                var events = Run(s, Use(attacker, "brawl_strike", target));
                count += events.OfType<Intercepted>().Count();
                if (phasing) Assert.True(s.GetUnit(target).Hp < 40);
            }
            return count;
        }
        Assert.True(Intercepts(false) > 0);
        Assert.Equal(0, Intercepts(true));
    }

    [Fact]
    public void PiercingEnchantAlsoHitsTheUnitBehindTheTargetOnce()
    {
        var (s, sorc, ally, enemy) = Board(1, "piercing_enchant");
        s.GetUnit(ally).LearnSkill("brawl_strike");
        var behind = World.Unit(s, Side.B, Row.Middle);
        var untouched = World.Unit(s, Side.B, Row.Rear);
        Run(s, Use(sorc, "piercing_enchant", ally));
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 4, 5);
        Assert.InRange(40 - s.GetUnit(behind).Hp, 4, 5);
        Assert.Equal(40, s.GetUnit(untouched).Hp);
        Assert.False(Has(s, ally, StatusIds.Piercing));

        Run(s, Use(ally, "brawl_strike", enemy));                           // used up: no second pierce
        Assert.InRange(40 - s.GetUnit(behind).Hp, 4, 5);
    }

    [Fact]
    public void EchoEnchantRepeatsTheNextSkillForFreeOnTheSameTarget()
    {
        var (s, sorc, ally, enemy) = Board(1, "echo_enchant");
        s.GetUnit(ally).LearnSkill("brawl_strike");
        var before = s.Momentum.AdvantageOf(Side.A);
        Run(s, Use(sorc, "echo_enchant", ally));
        Assert.Equal(before - 3, s.Momentum.AdvantageOf(Side.A));

        s.Timeline.Remove(s.Timeline.TurnOf(ally)?.Id ?? -1);
        var events = Run(s, Use(ally, "brawl_strike", enemy));
        Assert.Equal(2, events.OfType<SkillUsed>().Count());
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 8, 10);                    // two hits
        Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(ally)!.Due - 0);   // only one Delay was paid
        Assert.False(Has(s, ally, StatusIds.Echo));

        var again = Run(s, Use(ally, "brawl_strike", enemy));
        Assert.Single(again.OfType<SkillUsed>());
    }

    [Fact]
    public void EchoEnchantDoesNotRepeatAfterTheTargetFell()
    {
        var (s, sorc, ally, enemy) = Board(1, "echo_enchant");
        s.GetUnit(ally).LearnSkill("brawl_strike");
        World.Unit(s, Side.B, Row.Front);                                   // keep the battle going
        s.GetUnit(enemy).Hp = 1;
        Run(s, Use(sorc, "echo_enchant", ally));
        var events = Run(s, Use(ally, "brawl_strike", enemy));
        Assert.Single(events.OfType<SkillUsed>());
        Assert.True(s.GetUnit(enemy).IsKnockedOut);
    }

    [Fact]
    public void EnchantmentsFadeAfterTwentyTimeUnitsWithoutEnchantedSoulAndSeveralCanBeActive()
    {
        var (s, sorc, ally, _) = Board(1, "phase_weapon", "piercing_enchant");
        Run(s, Use(sorc, "phase_weapon", ally));
        Run(s, Use(sorc, "piercing_enchant", ally));
        Assert.True(Has(s, ally, StatusIds.Phased) && Has(s, ally, StatusIds.Piercing));
        AdvanceTo(s, 19);
        Assert.True(Has(s, ally, StatusIds.Phased));
        AdvanceTo(s, 21);
        Assert.False(Has(s, ally, StatusIds.Phased) || Has(s, ally, StatusIds.Piercing));
    }

    [Fact]
    public void EnchantedSoulMakesEnchantmentsLastUntilUsedButOnlyOneAtATime()
    {
        var (s, sorc, ally, _) = Board(1, "phase_weapon", "piercing_enchant", "enchanted_soul");
        var other = World.Unit(s, Side.A, Row.Middle);
        Run(s, Use(sorc, "phase_weapon", ally));
        AdvanceTo(s, 100);
        Assert.True(Has(s, ally, StatusIds.Phased));                         // never fades

        Run(s, Use(sorc, "piercing_enchant", other));
        Assert.True(Has(s, other, StatusIds.Piercing));
        Assert.False(Has(s, ally, StatusIds.Phased));                        // the earlier one was removed
    }

    // ---- Transmutation ----

    [Fact]
    public void PetrifyStunsForTenTimeUnits()
    {
        var (s, sorc, _, enemy) = Board(1, "petrify");
        Run(s, Use(sorc, "petrify", enemy));
        Assert.Equal(10, Statuses.Get(s.GetUnit(enemy), StatusIds.Stunned)!.EndsAt);
        AdvanceTo(s, 11);
        Assert.False(Has(s, enemy, StatusIds.Stunned));
    }

    [Fact]
    public void TransmuteSwapsPowerAndSpeedForTwentyTimeUnitsAndFortyWithLastingTransmute()
    {
        foreach (var lasting in new[] { false, true })
        {
            var s = new BattleState(1);
            var sorc = World.Unit(s, Side.A, Row.Rear, lasting ? new[] { "transmute", "lasting_transmute" } : new[] { "transmute" });
            var target = s.AddUnit("t", Side.B, 40, 3, 8);
            s.Formation.Place(target.Id, Side.B, Row.Front);
            World.Unit(s, Side.A, Row.Front);
            Run(s, Use(sorc, "transmute", target.Id));
            Assert.Equal(8, target.EffectiveSpeed);
            Assert.Equal(3, s.PowerOf(target.Id));
            var length = lasting ? 40 : 20;
            AdvanceTo(s, length - 1);
            Assert.Equal(8, target.EffectiveSpeed);
            AdvanceTo(s, length + 1);
            Assert.Equal(3, target.EffectiveSpeed);
            Assert.Equal(8, s.PowerOf(target.Id));
        }
    }

    [Fact]
    public void TransmuteChangesTheRemainingWaitLikeAnySpeedChange()
    {
        var s = new BattleState(1);
        var sorc = World.Unit(s, Side.A, Row.Rear, "transmute");
        var target = s.AddUnit("t", Side.B, 40, 2, 9);
        s.Formation.Place(target.Id, Side.B, Row.Front);
        World.Unit(s, Side.A, Row.Front);
        s.Timeline.ScheduleTurn(target.Id, 5, target.EffectiveSpeed);
        var before = s.Timeline.TurnOf(target.Id)!.Due;
        Run(s, Use(sorc, "transmute", target.Id));
        Assert.True(s.Timeline.TurnOf(target.Id)!.Due < before);             // Speed 2 became 9: the turn comes sooner
    }

    [Fact]
    public void StoneShellCannotActCannotBeDamagedCannotInterceptAndEndsAfterTenTimeUnits()
    {
        var (s, sorc, ally, enemy) = Board(1, "stone_shell");
        s.GetUnit(enemy).LearnSkill("brawl_strike");
        Run(s, Use(sorc, "stone_shell", ally));
        var unit = s.GetUnit(ally);
        Assert.True(Statuses.OnTurnStart(s, ally).TurnSkipped);              // cannot act
        Assert.True(unit.InterceptionBlocked);                               // cannot intercept
        var momentum = s.Momentum.AdvantageOf(Side.B);
        Run(s, Use(enemy, "brawl_strike", ally));
        Assert.Equal(40, unit.Hp);                                           // cannot be damaged
        Assert.Equal(momentum, s.Momentum.AdvantageOf(Side.B));              // a fully absorbed hit moves nothing
        Assert.Equal(Polarity.Harmful, StatusRules.PolarityOf(StatusIds.StoneShell));   // can be cleansed
        AdvanceTo(s, 11);
        Assert.False(Has(s, ally, StatusIds.StoneShell));
        Assert.False(unit.InterceptionBlocked);
    }

    [Fact]
    public void GlassBodyDoublesTheNextHitOnly()
    {
        var (s, sorc, ally, enemy) = Board(1, "glass_body");
        s.GetUnit(ally).LearnSkill("brawl_strike");
        Run(s, Use(sorc, "glass_body", enemy));
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 8, 10);
        Assert.False(Has(s, enemy, StatusIds.GlassBody));
        var hp = s.GetUnit(enemy).Hp;
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.InRange(hp - s.GetUnit(enemy).Hp, 4, 5);
    }

    // ---- Malediction ----

    [Fact]
    public void CurseOfWeightStopsMovingAndBecomingAirborneForTwentyTimeUnits()
    {
        var (s, sorc, _, enemy) = Board(1, "curse_of_weight");
        Run(s, Use(sorc, "curse_of_weight", enemy));
        Assert.Empty(Movement.LegalMoves(s, enemy));
        Assert.Throws<InvalidOperationException>(() => Actions.Move(s, enemy, Row.Middle, 0));
        Assert.False(Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Airborne, Duration: 10)));
        Assert.False(s.GetUnit(enemy).IsAirborne);
        AdvanceTo(s, 21);
        Assert.NotEmpty(Movement.LegalMoves(s, enemy));
        Assert.True(Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Airborne, Duration: 10)));
    }

    [Fact]
    public void CurseOfStagnationHoldsTimedStatusesAlreadyThereAndAppliedLater()
    {
        var (s, sorc, _, enemy) = Board(1, "curse_of_stagnation");
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Blind, Duration: 10));
        Run(s, Use(sorc, "curse_of_stagnation", enemy));                     // lands at 0, lasts to 20
        AdvanceTo(s, 5);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Silenced, Duration: 10));   // held at once
        AdvanceTo(s, 19);
        Assert.True(Has(s, enemy, StatusIds.Blind) && Has(s, enemy, StatusIds.Silenced));
        AdvanceTo(s, 21);                                                   // the curse ended at 20: Blind has 10 left, Silenced 10
        Assert.False(Has(s, enemy, StatusIds.Stagnant));
        Assert.True(Has(s, enemy, StatusIds.Blind) && Has(s, enemy, StatusIds.Silenced));
        AdvanceTo(s, 29);
        Assert.True(Has(s, enemy, StatusIds.Blind));
        AdvanceTo(s, 31);
        Assert.False(Has(s, enemy, StatusIds.Blind) || Has(s, enemy, StatusIds.Silenced));
    }

    [Fact]
    public void CurseOfStagnationCleansedEarlyReleasesTheHeldStatuses()
    {
        var s = new BattleState(1);
        var sorc = World.Unit(s, Side.A, Row.Rear, "curse_of_stagnation");
        World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 5);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Blind, Duration: 10));
        Run(s, Use(sorc, "curse_of_stagnation", enemy));
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Silenced, Duration: 10, Magnitude: 0));
        Statuses.Remove(s, enemy, StatusIds.Stagnant, StatusEndCause.Cleansed);
        Assert.Equal(10, Statuses.Get(s.GetUnit(enemy), StatusIds.Blind)!.EndsAt);
        Assert.Equal(10, Statuses.Get(s.GetUnit(enemy), StatusIds.Silenced)!.EndsAt);
    }

    [Fact]
    public void CurseOfBetrayalSendsHealingToTheSorcerersLowestHpUnit()
    {
        var s = new BattleState(1);
        var sorc = World.Unit(s, Side.A, Row.Rear, "curse_of_betrayal");
        var low = World.Unit(s, Side.A, Row.Front);
        var cursed = World.Unit(s, Side.B, Row.Front);
        var healer = World.Unit(s, Side.B, Row.Rear, "mend");
        s.GetUnit(sorc).Hp = 30;
        s.GetUnit(low).Hp = 10;
        s.GetUnit(cursed).Hp = 20;
        s.ShiftMomentum(Side.A, 5);
        Run(s, Use(sorc, "curse_of_betrayal", cursed));
        s.ShiftMomentum(Side.B, 10);
        Run(s, Use(healer, "mend", cursed));
        Assert.Equal(20, s.GetUnit(cursed).Hp);
        Assert.InRange(s.GetUnit(low).Hp - 10, 8, 10);
        Assert.Equal(30, s.GetUnit(sorc).Hp);

        AdvanceTo(s, 21);                                                   // the curse is over: healing works again
        s.Timeline.Remove(s.Timeline.TurnOf(healer)?.Id ?? -1);
        Run(s, Use(healer, "mend", cursed));
        Assert.True(s.GetUnit(cursed).Hp > 20);
    }

    [Fact]
    public void CurseOfBetrayalHealsTheTargetNormallyWhenNoSorcererUnitCanBeHealed()
    {
        var s = new BattleState(1);
        var sorc = World.Unit(s, Side.A, Row.Rear, "curse_of_betrayal");
        var other = World.Unit(s, Side.A, Row.Front);
        var cursed = World.Unit(s, Side.B, Row.Front);
        var healer = World.Unit(s, Side.B, Row.Rear, "mend");
        s.GetUnit(cursed).Hp = 20;
        s.ShiftMomentum(Side.A, 5);
        Run(s, Use(sorc, "curse_of_betrayal", cursed));
        s.GetUnit(sorc).Hp = 0;
        s.GetUnit(other).Hp = 0;
        s.ShiftMomentum(Side.B, 10);
        s.GetUnit(healer).Hp = 40;
        s.Heal(healer, cursed, 5);
        Assert.True(s.GetUnit(cursed).Hp > 20);
    }

    [Fact]
    public void LingeringMaliceSilencesAnEnemyWhoRemovesTheSorcerersCurse()
    {
        var s = new BattleState(1);
        var sorc = World.Unit(s, Side.A, Row.Rear, "curse_of_weight", "lingering_malice");
        World.Unit(s, Side.A, Row.Front);
        var cursed = World.Unit(s, Side.B, Row.Front);
        var healer = World.Unit(s, Side.B, Row.Rear, "absolve");
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.B, 0);
        Run(s, Use(sorc, "curse_of_weight", cursed));
        Assert.True(Has(s, cursed, StatusIds.Weighted));

        Run(s, Use(healer, "absolve", cursed));
        Assert.False(Has(s, cursed, StatusIds.Weighted));
        Assert.Equal(10, Statuses.Get(s.GetUnit(healer), StatusIds.Silenced)!.EndsAt);
    }

    [Fact]
    public void LingeringMaliceDoesNotTriggerWhenTheCurseSimplyRunsOut()
    {
        var s = new BattleState(1);
        var sorc = World.Unit(s, Side.A, Row.Rear, "curse_of_weight", "lingering_malice");
        World.Unit(s, Side.A, Row.Front);
        var cursed = World.Unit(s, Side.B, Row.Front);
        var healer = World.Unit(s, Side.B, Row.Rear, "absolve");
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(sorc, "curse_of_weight", cursed));
        AdvanceTo(s, 21);
        Assert.False(Has(s, cursed, StatusIds.Weighted));
        Assert.False(Has(s, healer, StatusIds.Silenced));
    }

    [Fact]
    public void WitchsPatienceMakesCursesLastUntilRemovedButKeepsOnlyOne()
    {
        var (s, sorc, ally, enemy) = Board(1, "curse_of_weight", "curse_of_betrayal", "witchs_patience");
        Run(s, Use(sorc, "curse_of_weight", enemy));
        AdvanceTo(s, 200);
        Assert.True(Has(s, enemy, StatusIds.Weighted));                      // no end

        Run(s, Use(sorc, "curse_of_betrayal", ally));
        Assert.True(Has(s, ally, StatusIds.Betrayed));
        Assert.False(Has(s, enemy, StatusIds.Weighted));                     // the earlier curse is gone
    }

    [Fact]
    public void WithoutWitchsPatienceCursesLastTwentyTimeUnitsAndSeveralCanBeActive()
    {
        var (s, sorc, ally, enemy) = Board(1, "curse_of_weight", "curse_of_betrayal");
        Run(s, Use(sorc, "curse_of_weight", enemy));
        Run(s, Use(sorc, "curse_of_betrayal", ally));
        Assert.True(Has(s, enemy, StatusIds.Weighted) && Has(s, ally, StatusIds.Betrayed));
        AdvanceTo(s, 19);
        Assert.True(Has(s, enemy, StatusIds.Weighted));
        AdvanceTo(s, 21);
        Assert.False(Has(s, enemy, StatusIds.Weighted) || Has(s, ally, StatusIds.Betrayed));
    }
}
