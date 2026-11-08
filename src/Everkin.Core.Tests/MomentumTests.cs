using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class MomentumTests
{
    [Fact]
    public void ABattleStartsAtNeutralZero()
    {
        var state = new BattleState(1);
        Assert.Equal(0, state.Momentum.Value);
        Assert.Equal(0, state.Momentum.AdvantageOf(Side.A));
        Assert.Equal(0, state.Momentum.AdvantageOf(Side.B));
    }

    [Fact]
    public void OneMeterTwoViews()
    {
        var m = new Momentum();
        m.Shift(Side.A, 3);
        Assert.Equal(3, m.AdvantageOf(Side.A));
        Assert.Equal(-3, m.AdvantageOf(Side.B));
        m.Shift(Side.B, 5);
        Assert.Equal(-2, m.AdvantageOf(Side.A));
        Assert.Equal(2, m.AdvantageOf(Side.B));
    }

    [Fact]
    public void TheMeterClampsAtTenAndExcessIsLost()
    {
        var m = new Momentum();
        m.Shift(Side.A, 25);
        Assert.Equal(10, m.Value);
        m.Shift(Side.A, 1);
        Assert.Equal(10, m.Value);
        m.Shift(Side.B, 40);
        Assert.Equal(-10, m.Value);
    }

    [Fact]
    public void ShiftsInOneBlockAreAppliedOneByOneAndClampedAfterEach()
    {
        var m = new Momentum();
        m.Shift(Side.A, 9);
        m.Shift(Side.A, 2);  // 11 -> clamped to 10
        m.Shift(Side.B, 1);  // 9, not 10
        Assert.Equal(9, m.Value);
    }

    [Fact]
    public void ACostNeedsALeadOfThatSizeAndMovesTheMeterTowardNeutral()
    {
        // 20.5: Momentum 2 with the side at +2: the cost is paid first (meter 0), then the hit moves it to +1.
        var m = new Momentum();
        m.Shift(Side.A, 2);
        Assert.True(m.CanPay(Side.A, 2));
        m.Pay(Side.A, 2);
        Assert.Equal(0, m.Value);
        m.Shift(Side.A, MomentumRules.ActionShift(new[] { HitOutcome.Landed }));
        Assert.Equal(1, m.Value);
    }

    [Fact]
    public void CostWorksFromSideBsPointOfView()
    {
        var m = new Momentum();
        m.Shift(Side.B, 4);
        m.Pay(Side.B, 3);
        Assert.Equal(1, m.AdvantageOf(Side.B));
        Assert.Equal(-1, m.Value);
    }

    [Fact]
    public void ACostIsNotAvailableWithoutTheLead()
    {
        var m = new Momentum();
        m.Shift(Side.A, 1);
        Assert.False(m.CanPay(Side.A, 2));
        Assert.False(m.CanPay(Side.B, 1));
        Assert.True(m.CanPay(Side.A, 0));
        Assert.Throws<InvalidOperationException>(() => m.Pay(Side.A, 2));
        Assert.Equal(1, m.Value); // nothing was paid
    }

    [Fact]
    public void CostsRunFromZeroToTen()
    {
        var m = new Momentum();
        m.Shift(Side.A, 10);
        Assert.True(m.CanPay(Side.A, 10));
        Assert.False(m.CanPay(Side.A, 11));
        Assert.False(m.CanPay(Side.A, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => m.Pay(Side.A, 11));
        m.Pay(Side.A, 10);
        Assert.Equal(0, m.Value);
    }

    [Fact]
    public void CostOfZeroChangesNothing()
    {
        var m = new Momentum();
        m.Pay(Side.A, 0);
        Assert.Equal(0, m.Value);
    }

    // ---- shifts per hit and per action (game_system 6.7, 20.5) ----

    [Theory]
    [InlineData(HitOutcome.Landed, 1)]
    [InlineData(HitOutcome.Missed, -1)]
    [InlineData(HitOutcome.Intercepted, 0)]
    [InlineData(HitOutcome.FullyAbsorbed, 0)]   // 7 damage into a shield of 8: Momentum does not change
    [InlineData(HitOutcome.OnActingSide, 0)]    // an attack on its own ally: Momentum does not change
    public void HitShift(HitOutcome outcome, int expected) => Assert.Equal(expected, MomentumRules.HitShift(outcome));

    [Fact]
    public void AMultiHitActionShiftsOnceTowardIfAnyHitLands()
    {
        Assert.Equal(1, MomentumRules.ActionShift(new[] { HitOutcome.Missed, HitOutcome.Landed, HitOutcome.Landed }));
        Assert.Equal(1, MomentumRules.ActionShift(new[] { HitOutcome.Intercepted, HitOutcome.Landed }));
    }

    [Fact]
    public void WithoutALandedHitAMissShiftsOnceAgainst()
    {
        Assert.Equal(-1, MomentumRules.ActionShift(new[] { HitOutcome.Missed, HitOutcome.Missed, HitOutcome.Intercepted }));
        Assert.Equal(-1, MomentumRules.ActionShift(new[] { HitOutcome.Intercepted, HitOutcome.Missed }));
    }

    [Fact]
    public void EverythingInterceptedAbsorbedOrFriendlyShiftsNothing()
    {
        Assert.Equal(0, MomentumRules.ActionShift(new[] { HitOutcome.Intercepted, HitOutcome.Intercepted, HitOutcome.Intercepted }));
        Assert.Equal(0, MomentumRules.ActionShift(new[] { HitOutcome.FullyAbsorbed, HitOutcome.OnActingSide }));
    }

    [Fact]
    public void AnActionWithoutDamagingHitsShiftsNothing()
    {
        // A Silence, a Mark, Move, Skip Turn, a heal: no damaging hit at all.
        Assert.Equal(0, MomentumRules.ActionShift(Array.Empty<HitOutcome>()));
    }

    [Fact]
    public void ACostIsNotRefundedWhenEveryHitIsIntercepted()
    {
        // 20.5: every hit of a Momentum 3 skill is intercepted: the 3 is not refunded.
        var m = new Momentum();
        m.Shift(Side.A, 3);
        m.Pay(Side.A, 3);
        m.Shift(Side.A, MomentumRules.ActionShift(new[] { HitOutcome.Intercepted, HitOutcome.Intercepted }));
        Assert.Equal(0, m.Value);
    }

    // ---- through the battle state ----

    [Fact]
    public void BattleStateReportsCostsAndShiftsInOrder()
    {
        var state = new BattleState(1);
        state.DrainEvents();
        state.ShiftMomentum(Side.A, 3);
        state.PayMomentum(Side.A, 2);
        state.ShiftMomentum(Side.A, 1);
        Assert.Equal(new BattleEvent[]
        {
            new MomentumChanged(MomentumCause.Shift, Side.A, 0, 3),
            new MomentumChanged(MomentumCause.Cost, Side.A, 3, 1),
            new MomentumChanged(MomentumCause.Shift, Side.A, 1, 2),
        }, state.DrainEvents());
    }

    [Fact]
    public void ANoChangeShiftReportsNothing()
    {
        var state = new BattleState(1);
        state.DrainEvents();
        state.ShiftMomentum(Side.A, 0);
        state.ShiftMomentum(Side.A, 10);
        state.DrainEvents();
        state.ShiftMomentum(Side.A, 1); // already at the limit: the excess is lost, nothing moves
        Assert.Empty(state.DrainEvents());
    }

    [Fact]
    public void CloneKeepsTheMeterIndependently()
    {
        var state = new BattleState(1);
        state.ShiftMomentum(Side.A, 4);
        var copy = state.Clone();
        copy.ShiftMomentum(Side.B, 9);
        Assert.Equal(4, state.Momentum.Value);
        Assert.Equal(-5, copy.Momentum.Value);
    }
}
