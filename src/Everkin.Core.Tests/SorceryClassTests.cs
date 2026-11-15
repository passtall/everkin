using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The Enchanter, Transmuter and Hexer.</summary>
public class SorceryClassTests
{
    [Fact]
    public void AllSorceryClassesAreImplemented()
    {
        AssertClassImplemented("enchanter", magical: true);
        AssertClassImplemented("transmuter", magical: true);
        AssertClassImplemented("hexer", magical: true);
    }

    // ---- Enchanter ----

    [Fact]
    public void EnchantmentsAreHelpfulStatusesThatFadeAfterTwentyAndAnyNumberCanBeActive()
    {
        var (s, en, e) = Board(1, "echo_enchant", "phase_weapon", "piercing_enchant");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        s.ShiftMomentum(Side.A, 4);
        foreach (var skill in new[] { "echo_enchant", "phase_weapon", "piercing_enchant" })
        {
            K.Ready(s, en);
            Run(s, Use(en, skill, ally));
        }
        foreach (var id in new[] { StatusIds.Echo, StatusIds.Phased, StatusIds.Piercing })
        {
            Assert.Equal(20, Statuses.Get(s.GetUnit(ally), id)!.TimeLeft(s.Timeline.Now));
            Assert.Equal(Polarity.Helpful, StatusRules.PolarityOf(id));
        }
        AdvanceTo(s, 20);
        Assert.False(Has(s, ally, StatusIds.Echo));
    }

    [Fact]
    public void EchoEnchantMakesTheUnitsNextSkillHappenTwiceTheSecondFree()
    {
        var s = new BattleState(1);
        var en = World.Unit(s, Side.A, Row.Rear, "echo_enchant");
        var a = World.Unit(s, Side.A, Row.Front, "tail_club");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(en, "echo_enchant", a));
        K.Ready(s, a);
        var events = Run(s, Use(a, "tail_club", e));
        Assert.Equal(2, events.OfType<SkillUsed>().Count());
        Assert.False(Has(s, a, StatusIds.Echo));
        Assert.Equal(Timeline.Wait(4, 5), s.Timeline.TurnOf(a)!.Due);                     // the second use costs no Delay
    }

    [Fact]
    public void PhaseWeaponMakesTheNextAttackIgnoreInterceptionButOnlyOnce()
    {
        var intercepted = 0;
        for (ulong seed = 1; seed <= 40; seed++)
        {
            var s = new BattleState(seed);
            var en = World.Unit(s, Side.A, Row.Rear, "phase_weapon");
            var a = World.Unit(s, Side.A, Row.Front, "rivet_shot");
            World.Unit(s, Side.B, Row.Front);
            var target = World.Unit(s, Side.B, Row.Middle);
            BattleStart.Install(s, World.Catalog);
            Run(s, Use(en, "phase_weapon", a));
            K.Ready(s, a);
            Assert.DoesNotContain(Run(s, Use(a, "rivet_shot", target)), x => x is Intercepted);
            K.Ready(s, a);
            if (Run(s, Use(a, "rivet_shot", target)).Any(x => x is Intercepted)) intercepted++;
        }
        Assert.InRange(intercepted, 4, 36);                                               // back to the usual roll
    }

    [Fact]
    public void PiercingEnchantMakesTheNextAttackAlsoHitTheUnitBehindItsTarget()
    {
        var s = new BattleState(1);
        var en = World.Unit(s, Side.A, Row.Rear, "piercing_enchant");
        var a = World.Unit(s, Side.A, Row.Front, "rivet_shot");
        var front = World.Unit(s, Side.B, Row.Front);
        var behind = World.Unit(s, Side.B, Row.Middle);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(en, "piercing_enchant", a));
        K.Ready(s, a);
        Run(s, Use(a, "rivet_shot", front));
        Assert.InRange(Lost(s, behind), 3, 4);
    }

    [Fact]
    public void TwinEnchantAlsoPutsTheEnchantmentOnOneAllyNextToTheTarget()
    {
        var s = new BattleState(1);
        var en = World.Unit(s, Side.A, Row.Rear, "phase_weapon", "twin_enchant");
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(en, "phase_weapon", a));
        Assert.True(Has(s, a, StatusIds.Phased));
        Assert.True(Has(s, b, StatusIds.Phased));
    }

    [Fact]
    public void LingeringMagicGivesEachEnchantmentTwoUses()
    {
        var s = new BattleState(1);
        var en = World.Unit(s, Side.A, Row.Rear, "phase_weapon", "lingering_magic");
        var a = World.Unit(s, Side.A, Row.Front, "rivet_shot");
        var target = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(en, "phase_weapon", a));
        K.Ready(s, a);
        Run(s, Use(a, "rivet_shot", target));
        Assert.True(Has(s, a, StatusIds.Phased));
        K.Ready(s, a);
        Run(s, Use(a, "rivet_shot", target));
        Assert.False(Has(s, a, StatusIds.Phased));
    }

    [Fact]
    public void SpellWardStopsSilenceAndStunOnAUnitCarryingAnEnchantmentFromTheEnchanter()
    {
        var s = new BattleState(1);
        var en = World.Unit(s, Side.A, Row.Rear, "phase_weapon", "spell_ward");
        var a = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Assert.True(Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 5)));
        Statuses.Remove(s, a, StatusIds.Stunned, StatusEndCause.Cleansed);
        Run(s, Use(en, "phase_weapon", a));
        Assert.False(Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 5)));
        Assert.False(Statuses.Apply(s, new StatusApplication(a, StatusIds.Silenced, Duration: 5)));
        Assert.True(Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 5)));
    }

    // ---- Transmuter ----

    [Fact]
    public void TransmuteSwapsPowerAndSpeedForTwentyAndTheDamageUsesTheNewPower()
    {
        var (s, t, e) = Board(1, "transmute", "tail_club");
        s.GetUnit(e).Power = 7;
        s.GetUnit(e).Speed = 2;
        Run(s, Use(t, "transmute", e));
        Assert.Equal(7, s.GetUnit(e).EffectiveSpeed);
        Assert.Equal(2, s.GetUnit(e).EffectivePower);
        Assert.Equal(20, Statuses.Get(s.GetUnit(e), StatusIds.Transmuted)!.TimeLeft(s.Timeline.Now));
        AdvanceTo(s, 20);
        Assert.Equal(7, s.GetUnit(e).EffectivePower);
    }

    [Fact]
    public void TransmuteIsNeitherHelpfulNorHarmfulSoNothingCleansesIt()
    {
        var (s, t, e) = Board(1, "transmute");
        Run(s, Use(t, "transmute", e));
        Assert.Equal(Polarity.Unclassified, StatusRules.PolarityOf(StatusIds.Transmuted));
        Statuses.RemoveAll(s, e, Polarity.Harmful);
        Statuses.RemoveAll(s, e, Polarity.Helpful);
        Assert.True(Has(s, e, StatusIds.Transmuted));
    }

    [Fact]
    public void LastingChangeLastsAllBattleButOnlyOneUnitAtATime()
    {
        var (s, t, e1) = Board(1, "transmute", "lasting_change");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(t, "transmute", e1));
        Assert.Null(Statuses.Get(s.GetUnit(e1), StatusIds.Transmuted)!.TimeLeft(s.Timeline.Now));
        AdvanceTo(s, 200);
        Assert.True(Has(s, e1, StatusIds.Transmuted));
        K.Ready(s, t);
        Run(s, Use(t, "transmute", e2));
        Assert.False(Has(s, e1, StatusIds.Transmuted));
        Assert.True(Has(s, e2, StatusIds.Transmuted));
    }

    [Fact]
    public void MirrorFormLetsTwoUnitsTradeTheirPowerAndSpeed()
    {
        var (s, t, e1) = Board(1, "transmute", "mirror_form");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        s.GetUnit(e1).Power = 8;
        s.GetUnit(e1).Speed = 3;
        s.GetUnit(e2).Power = 2;
        s.GetUnit(e2).Speed = 9;
        Assert.Null(Why(s, Use(t, "transmute", e1, second: e2)));
        Run(s, Use(t, "transmute", e1, second: e2));
        Assert.Equal((2, 9), (s.GetUnit(e1).EffectivePower, s.GetUnit(e1).EffectiveSpeed));
        Assert.Equal((8, 3), (s.GetUnit(e2).EffectivePower, s.GetUnit(e2).EffectiveSpeed));
        AdvanceTo(s, 20);
        Assert.Equal((8, 3), (s.GetUnit(e1).EffectivePower, s.GetUnit(e1).EffectiveSpeed));
    }

    [Fact]
    public void TransmuteOffersAnOptionalSecondTargetOnlyWithMirrorFormAndTheAiGeneratesThePairs()
    {
        var (s, t, e1) = Board(1, "transmute", "mirror_form");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        var choice = World.Catalog.Get("transmute").SecondTargetFor(s, World.Catalog, t, e1);
        Assert.NotNull(choice);
        Assert.False(choice!.Required);
        Assert.DoesNotContain(e1, choice.Units);
        Assert.Contains(e2, choice.Units);
        Assert.Contains(Everkin.Core.Ai.MoveGenerator.Legal(s, World.Catalog, t), a => a.Use is { SkillId: "transmute", SecondTargetUnitId: not null });
        var (s2, t2, f) = Board(1, "transmute");
        Assert.Null(World.Catalog.Get("transmute").SecondTargetFor(s2, World.Catalog, t2, f));
    }

    [Fact]
    public void TradingNeedsMirrorFormAndTwoDifferentUnits()
    {
        var (s, t, e1) = Board(1, "transmute");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        Assert.NotNull(Why(s, Use(t, "transmute", e1, second: e2)));
        var (s2, t2, e) = Board(1, "transmute", "mirror_form");
        Assert.NotNull(Why(s2, Use(t2, "transmute", e, second: e)));
    }

    [Fact]
    public void GlassBodyMakesTheNextHitDealFiftyPercentMoreWithNoTimeLimit()
    {
        var (s, t, e) = Board(1, "glass_body");
        Run(s, Use(t, "glass_body", e));
        AdvanceTo(s, 500);
        Assert.True(Has(s, e, StatusIds.GlassBody));
        Assert.Equal(Polarity.Harmful, StatusRules.PolarityOf(StatusIds.GlassBody));
        Assert.Equal(9, s.DealDamage(t, e, 6).HpLost);
        Assert.False(Has(s, e, StatusIds.GlassBody));
        Assert.Equal(6, s.DealDamage(t, e, 6).HpLost);
    }

    [Fact]
    public void BrittleGlassMakesTheNextHitDealDouble()
    {
        var (s, t, e) = Board(1, "glass_body", "brittle_glass");
        Run(s, Use(t, "glass_body", e));
        Assert.Equal(12, s.DealDamage(t, e, 6).HpLost);
    }

    [Fact]
    public void PetrifyStunsAUnitForTen()
    {
        var (s, t, e) = Board(1, "petrify");
        Run(s, Use(t, "petrify", e));
        Assert.Equal(10, Statuses.Get(s.GetUnit(e), StatusIds.Stunned)!.TimeLeft(s.Timeline.Now));
        Assert.Equal(2, s.Momentum.Value);                                                // cost 4 from +6
    }

    // ---- Hexer ----

    [Fact]
    public void CurseOfWeightStopsMoveAndAirborneForTwenty()
    {
        var (s, h, e) = Board(1, "curse_of_weight");
        Run(s, Use(h, "curse_of_weight", e));
        Assert.Empty(Movement.LegalMoves(s, e));
        Assert.False(Statuses.Apply(s, new StatusApplication(e, StatusIds.Airborne, Duration: 20)));
        AdvanceTo(s, 20);
        Assert.NotEmpty(Movement.LegalMoves(s, e));
    }

    [Fact]
    public void CurseOfBetrayalSendsTheHealingToTheHexersLowestHpUnit()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "curse_of_betrayal");
        var low = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        var healer = World.Unit(s, Side.B, Row.Middle);
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(low).Hp = 5;
        s.GetUnit(enemy).Hp = 10;
        Run(s, Use(h, "curse_of_betrayal", enemy));
        s.Heal(healer, enemy, 8);
        Assert.Equal(10, s.GetUnit(enemy).Hp);
        Assert.Equal(13, s.GetUnit(low).Hp);
    }

    [Fact]
    public void WitheringCurseTakesThreeEveryTenForFortyAndALandsWholePointsAtTheEnd()
    {
        var (s, h, e) = Board(1, "withering_curse");
        Run(s, Use(h, "withering_curse", e));
        var status = Statuses.Get(s.GetUnit(e), StatusIds.Withering)!;
        Assert.Equal(40, status.TimeLeft(s.Timeline.Now));
        Assert.Equal(3, status.Magnitude);
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(3, Lost(s, e));
        AdvanceTo(s, 40);
        Assert.False(Has(s, e, StatusIds.Withering));
        Assert.Equal(12, Lost(s, e));
    }

    [Fact]
    public void WitheringCurseIsNotAHitSoItMovesNoMomentumAndAWardDoesNotAbsorbIt()
    {
        var (s, h, e) = Board(1, "withering_curse");
        s.GetUnit(e).Ward = 20;
        Run(s, Use(h, "withering_curse", e));
        var momentum = s.Momentum.Value;
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(3, Lost(s, e));
        Assert.Equal(20, s.GetUnit(e).Ward);
        Assert.Equal(momentum, s.Momentum.Value);
    }

    [Fact]
    public void WitchsPatienceMakesCursesLastUntilRemovedAndKeepsOnlyOne()
    {
        var (s, h, e1) = Board(1, "curse_of_weight", "withering_curse", "witchs_patience");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(h, "curse_of_weight", e1));
        Assert.Null(Statuses.Get(s.GetUnit(e1), StatusIds.Weighted)!.TimeLeft(s.Timeline.Now));
        AdvanceTo(s, 100);
        Assert.True(Has(s, e1, StatusIds.Weighted));
        K.Ready(s, h);
        Run(s, Use(h, "withering_curse", e2));
        Assert.False(Has(s, e1, StatusIds.Weighted));                                     // the earlier curse is removed, on any unit
        Assert.True(Has(s, e2, StatusIds.Withering));
    }

    [Fact]
    public void LingeringMaliceSilencesAnEnemyThatRemovesTheHexersCurse()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "curse_of_weight", "lingering_malice");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var cleric = World.Unit(s, Side.B, Row.Front, "absolve");
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(h, "curse_of_weight", cleric));                                        // a curse on the enemy cleric, then it cleanses itself
        Statuses.RemoveAll(s, cleric, Polarity.Harmful, byUnitId: cleric);
        Assert.True(Has(s, cleric, StatusIds.Silenced));
        Assert.Equal(10, Statuses.Get(s.GetUnit(cleric), StatusIds.Silenced)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void SpreadingCurseMovesTheCurseOfAKoUnitToTheNearestEnemyWithItsRemainingTime()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "withering_curse", "spreading_curse");
        var victim = World.Unit(s, Side.B, Row.Front);
        var next = World.Unit(s, Side.B, Row.Middle);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(h, "withering_curse", victim));
        AdvanceTo(s, 15);
        s.GetUnit(victim).Hp = 0;
        Actions.Settle(s, h);
        Assert.True(s.GetUnit(victim).IsKnockedOut);
        var jumped = Statuses.Get(s.GetUnit(next), StatusIds.Withering)!;
        Assert.Equal(25, jumped.TimeLeft(s.Timeline.Now));
        Assert.Equal(h, jumped.SourceUnitId);
    }
}
