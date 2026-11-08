using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Tests;

public class DamageMathTests
{
    // ---- Power scaling (game_system 5.2, 20.5) ----

    [Theory]
    [InlineData(4, 7, 5)]   // a Power 7 Guardian's Ward of 4 absorbs 5 (28 / 5 = 5.6 -> 5)
    [InlineData(4, 5, 4)]   // reference Power
    [InlineData(4, 3, 2)]   // 12 / 5 = 2.4 -> 2
    [InlineData(5, 3, 3)]
    [InlineData(1, 1, 1)]   // minimum 1
    [InlineData(2, 1, 1)]   // 2 / 5 -> 0, minimum 1
    [InlineData(10, 10, 20)]
    public void ScaleMultipliesByPowerOverFiveRoundingDownWithMinimumOne(int value, int power, int expected) =>
        Assert.Equal(expected, DamageMath.Scale(value, power));

    [Fact]
    public void ARangeScalesAtBothEnds()
    {
        Assert.Equal(new IntRange(2, 3), DamageMath.ScaleRange(new IntRange(4, 5), 3)); // 4-5 at Power 3 is 2-3
        Assert.Equal(new IntRange(4, 5), DamageMath.ScaleRange(new IntRange(4, 5), 5));
        Assert.Equal(new IntRange(5, 7), DamageMath.ScaleRange(new IntRange(4, 5), 7)); // 28/5=5, 35/5=7
    }

    [Fact]
    public void RollsAreUniformWholeNumbersInsideTheRange()
    {
        var r = new BattleRandom(5);
        var counts = new int[2];
        for (var i = 0; i < 20000; i++)
        {
            var v = DamageMath.Roll(r, new IntRange(4, 5));
            Assert.InRange(v, 4, 5);
            counts[v - 4]++;
        }
        Assert.All(counts, c => Assert.InRange(c, 9500, 10500));
        Assert.Equal(3, DamageMath.Roll(r, IntRange.Of(3)));
        Assert.Throws<ArgumentOutOfRangeException>(() => DamageMath.Roll(r, new IntRange(5, 4)));
    }

    // ---- resolution order (5.2, 20.5) ----

    [Fact]
    public void ANineDamageHitWithoutAShieldDealsNine()
    {
        var r = DamageMath.Resolve(9, ward: 0, dampen: false);
        Assert.Equal(9, r.HpLost);
        Assert.Equal(0, r.AbsorbedByWard);
    }

    [Fact]
    public void AShieldTakesDamageFirstAndTheRestReachesHp()
    {
        var r = DamageMath.Resolve(9, ward: 8, dampen: false);
        Assert.Equal(8, r.AbsorbedByWard);
        Assert.Equal(1, r.HpLost);
        Assert.Equal(0, r.WardLeft);
        Assert.False(r.FullyAbsorbed);
    }

    [Fact]
    public void ASevenDamageHitIntoAShieldOfEightDealsNothingAndLeavesOneWard()
    {
        var r = DamageMath.Resolve(7, ward: 8, dampen: false);
        Assert.Equal(7, r.AbsorbedByWard);
        Assert.Equal(0, r.HpLost);
        Assert.Equal(1, r.WardLeft);
        Assert.True(r.FullyAbsorbed); // Momentum does not change
    }

    [Fact]
    public void DampenHalvesRoundingDown()
    {
        Assert.Equal(4, DamageMath.Resolve(9, 0, dampen: true).HpLost); // 4.5 -> 4
        Assert.Equal(1, DamageMath.Resolve(1, 0, dampen: true).HpLost); // minimum 1
        Assert.Equal(1, DamageMath.Resolve(2, 0, dampen: true).HpLost); // a fixed 2 counter on a Dampened unit deals 1
        Assert.Equal(2, DamageMath.Resolve(5, 0, dampen: true).HpLost);
    }

    [Fact]
    public void WardAbsorbsFirstThenDampenLeavesAtLeastOne()
    {
        var r = DamageMath.Resolve(9, ward: 8, dampen: true);
        Assert.Equal(8, r.AbsorbedByWard);
        Assert.Equal(1, r.HpLost); // 1 / 2 = 0, minimum 1
    }

    [Fact]
    public void DampenDoesNotApplyToWhatTheShieldTookOrToNothing()
    {
        var r = DamageMath.Resolve(5, ward: 8, dampen: true);
        Assert.Equal(0, r.HpLost);
        Assert.True(r.FullyAbsorbed);
        Assert.Equal(0, DamageMath.Resolve(0, 3, true).HpLost);
    }

    [Fact]
    public void PoisonIsNotAbsorbedByAWardButIsReducedByDampen()
    {
        var r = DamageMath.Resolve(1, ward: 8, dampen: false, DamageSource.Periodic);
        Assert.Equal(1, r.HpLost);
        Assert.Equal(8, r.WardLeft);
        Assert.Equal(4, DamageMath.Resolve(9, 8, true, DamageSource.Periodic).HpLost);
    }

    [Fact]
    public void BumpDamageIgnoresShieldsAndReductions()
    {
        var r = DamageMath.Resolve(1, ward: 8, dampen: true, DamageSource.Unreducible);
        Assert.Equal(1, r.HpLost);
        Assert.Equal(8, r.WardLeft);
        Assert.Equal(3, DamageMath.Resolve(3, 0, true, DamageSource.Unreducible).HpLost);
    }

    [Fact]
    public void BurningGroundIsAbsorbedLikeAHit()
    {
        // 20.5: burning ground damages a unit with a Ward of 8: the Ward absorbs it (a Hit-source damage).
        var r = DamageMath.Resolve(2, ward: 8, dampen: false, DamageSource.Hit);
        Assert.Equal(2, r.AbsorbedByWard);
        Assert.Equal(0, r.HpLost);
    }

    [Theory]
    [InlineData(3, 2, 3)]   // a unit with 3 Ward left gets Shield Dome's Ward 2: keeps 3
    [InlineData(4, 8, 8)]   // Ward 4 gets Overcharged Ward 8: replaced
    [InlineData(4, 4, 4)]   // equal: the new one replaces (same value)
    [InlineData(0, 2, 2)]
    public void OneWardAndTheLargerStays(int current, int incoming, int expected) =>
        Assert.Equal(expected, DamageMath.CombineWard(current, incoming));

    [Theory]
    [InlineData(30, 50, 8, 38)]
    [InlineData(48, 50, 8, 50)]   // healing exceeds missing HP: stops at maximum
    [InlineData(50, 50, 8, 50)]
    public void HealingStopsAtMaximumHp(int hp, int max, int amount, int expected) =>
        Assert.Equal(expected, DamageMath.HealedHp(hp, max, amount));
}

public class DamageOnStateTests
{
    private static BattleState StateWithUnits(out int a, out int b, int hp = 50, int power = 5)
    {
        var state = new BattleState(1);
        var def = new CreatureDef("t", "t", "brawl", new[] { "warrior" }, "t.png", null, "ruins", hp, 5, power, 1,
            new[] { 0, 0, 1, 1 }, new[] { 0, 0, 1, 1 }, "reviewed", "");
        a = state.AddUnit(def, Side.A).Id;
        b = state.AddUnit(def, Side.B).Id;
        state.DrainEvents();
        return state;
    }

    [Fact]
    public void DamageReducesHpAndIsReported()
    {
        var state = StateWithUnits(out var a, out var b);
        var result = state.DealDamage(a, b, 9);
        Assert.Equal(41, state.GetUnit(b).Hp);
        Assert.Equal(9, result.HpLost);
        Assert.Equal(new BattleEvent[] { new DamageDealt(a, b, DamageSource.Hit, result, 41) }, state.DrainEvents());
    }

    [Fact]
    public void WardAndDampenWorkThroughTheState()
    {
        var state = StateWithUnits(out var a, out var b);
        state.GetUnit(b).Ward = 8;
        state.GetUnit(b).Dampen = true;
        var result = state.DealDamage(a, b, 9);
        Assert.Equal(8, result.AbsorbedByWard);
        Assert.Equal(49, state.GetUnit(b).Hp);
        Assert.Equal(0, state.GetUnit(b).Ward);
    }

    [Fact]
    public void HpStopsAtZero()
    {
        var state = StateWithUnits(out var a, out var b, hp: 5);
        state.DealDamage(a, b, 40);
        Assert.Equal(0, state.GetUnit(b).Hp);
        Assert.True(state.GetUnit(b).IsKnockedOut);
    }

    [Fact]
    public void AKoUnitCannotBeDamagedOrHealed()
    {
        var state = StateWithUnits(out var a, out var b, hp: 5);
        state.DealDamage(a, b, 5);
        Assert.Throws<InvalidOperationException>(() => state.DealDamage(a, b, 1));
        Assert.Throws<InvalidOperationException>(() => state.Heal(a, b, 1));
    }

    [Fact]
    public void HealingIsCappedAndReportsWhatActuallyHealed()
    {
        var state = StateWithUnits(out var a, out var b);
        state.DealDamage(a, b, 5);
        state.DrainEvents();
        var healed = state.Heal(a, b, 8);
        Assert.Equal(5, healed);
        Assert.Equal(50, state.GetUnit(b).Hp);
        Assert.Equal(new BattleEvent[] { new Healed(a, b, 5, 50) }, state.DrainEvents());
    }

    [Fact]
    public void WardKeepsTheLargerAndOnlyReportsAChange()
    {
        var state = StateWithUnits(out _, out var b);
        state.ApplyWard(b, 3);
        state.DrainEvents();
        state.ApplyWard(b, 2); // smaller: the old one stays, nothing to report
        Assert.Equal(3, state.GetUnit(b).Ward);
        Assert.Empty(state.DrainEvents());
        state.ApplyWard(b, 8);
        Assert.Equal(8, state.GetUnit(b).Ward);
        Assert.Equal(new BattleEvent[] { new WardChanged(b, 3, 8) }, state.DrainEvents());
    }

    [Fact]
    public void ASeededRollScaledByPowerGivesAReproducibleHit()
    {
        // A Power 3 unit's "hit for 4-5" is 2-3.
        int Hit(ulong seed)
        {
            var state = new BattleState(seed);
            var range = DamageMath.ScaleRange(new IntRange(4, 5), 3);
            return DamageMath.Roll(state.Random, range);
        }
        Assert.Equal(Hit(9), Hit(9));
        Assert.InRange(Hit(9), 2, 3);
    }

    [Fact]
    public void CloneCopiesWardAndDampen()
    {
        var state = StateWithUnits(out _, out var b);
        state.GetUnit(b).Ward = 4;
        state.GetUnit(b).Dampen = true;
        var copy = state.Clone();
        copy.GetUnit(b).Ward = 0;
        Assert.Equal(4, state.GetUnit(b).Ward);
        Assert.True(copy.GetUnit(b).Dampen);
    }
}
