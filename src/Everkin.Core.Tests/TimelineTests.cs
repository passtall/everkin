using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class TimelineTests
{
    private static Unit U(int id, int speed) => new(id, Side.A, "u" + id, null, 40, speed, 5);

    // ---- the formula (game_system 6.1 and 20.5) ----

    [Theory]
    [InlineData(5, 5, 11)]   // Speed 5, Delay 5
    [InlineData(10, 1, 2)]   // fastest cycle
    [InlineData(10, 5, 6)]
    [InlineData(1, 1, 11)]
    [InlineData(1, 10, 20)]  // slowest cycle
    public void WaitIsDelayPlusElevenMinusSpeed(int speed, int delay, int wait) =>
        Assert.Equal(wait, Timeline.Wait(delay, speed));

    [Fact]
    public void FastestAndSlowestCyclesAreTenTimesApart() =>
        Assert.Equal(10, Timeline.Wait(10, 1) / Timeline.Wait(1, 10)); // Delay 10 at Speed 1 (20) versus Delay 1 at Speed 10 (2)

    [Fact]
    public void DelayAndSpeedAreClampedAndTheWaitIsAtLeastOne()
    {
        Assert.Equal(Timeline.Wait(10, 5), Timeline.Wait(14, 5)); // Delay clamped to 10
        Assert.Equal(Timeline.Wait(1, 5), Timeline.Wait(-3, 5));  // Delay clamped to 1
        Assert.Equal(Timeline.Wait(5, 10), Timeline.Wait(5, 14)); // Speed clamped to 10
        Assert.Equal(1, Timeline.InitialWait(10));
    }

    [Fact]
    public void SkippedTurnCostsADelayFiveSkill() =>
        Assert.Equal(7, Timeline.SkippedTurnWait(9)); // 5 + (11 - 9)

    [Fact]
    public void FirstWaitIsElevenMinusSpeed()
    {
        // A Speed 8 and a Speed 5 unit: first Waits are 3 and 6, so the Speed 8 unit acts first.
        var t = new Timeline();
        var entries = t.ScheduleInitialTurns(new[] { U(0, 5), U(1, 8) }, new BattleRandom(1));
        Assert.Equal(6, t.Remaining(entries[0]));
        Assert.Equal(3, t.Remaining(entries[1]));
        Assert.Equal(1, t.Pop()!.OwnerUnitId);
        Assert.Equal(3, t.Now);
        Assert.Equal(0, t.Pop()!.OwnerUnitId);
        Assert.Equal(6, t.Now);
    }

    // ---- ties ----

    [Fact]
    public void AMovedEntryActsAfterTheOneAlreadyInTheTimeUnit()
    {
        var t = new Timeline();
        var a = t.Add(TimelineKind.UnitTurn, 10, 0);
        var b = t.Add(TimelineKind.UnitTurn, 6, 1);
        t.PushBack(b, 4); // joins time unit 10 behind a
        Assert.Equal(new[] { 0, 1 }, t.Upcoming().Select(e => e.OwnerUnitId!.Value));
        Assert.Equal(10, b.Due);
        Assert.Equal(10, a.Due);
    }

    [Fact]
    public void FirstToJoinATimeUnitStaysFirst()
    {
        var t = new Timeline();
        var first = t.Add(TimelineKind.UnitTurn, 5, 0);
        var second = t.Add(TimelineKind.UnitTurn, 5, 1);
        Assert.Same(first, t.Pop());
        Assert.Same(second, t.Pop());
    }

    [Fact]
    public void AStatusEndThatJoinedBeforeATurnMovedInEndsFirst()
    {
        // 20.5: the status end joined time unit 12 before the unit's turn moved into it, so the unit acts without the status.
        var t = new Timeline();
        var turn = t.Add(TimelineKind.UnitTurn, 8, 0);
        var end = t.Add(TimelineKind.EffectEnd, 12, 0, "chilled");
        t.PushBack(turn, 4);
        Assert.Equal(TimelineKind.EffectEnd, t.Pop()!.Kind);
        Assert.Equal(TimelineKind.UnitTurn, t.Pop()!.Kind);
        Assert.Equal(12, t.Now);
        Assert.NotNull(end);
    }

    [Fact]
    public void SimultaneousFirstTurnsAreOrderedBySeedAndReproducible()
    {
        List<int> Order(ulong seed)
        {
            var t = new Timeline();
            t.ScheduleInitialTurns(new[] { U(0, 5), U(1, 5), U(2, 5), U(3, 5) }, new BattleRandom(seed));
            var order = new List<int>();
            while (t.Pop() is { } e) order.Add(e.OwnerUnitId!.Value);
            return order;
        }

        Assert.Equal(Order(7), Order(7));
    }

    [Fact]
    public void SimultaneousFirstTurnsAreAllOrdersEquallyLikely()
    {
        var counts = new Dictionary<string, int>();
        for (ulong seed = 1; seed <= 6000; seed++)
        {
            var t = new Timeline();
            t.ScheduleInitialTurns(new[] { U(0, 5), U(1, 5), U(2, 5) }, new BattleRandom(seed));
            var key = string.Join(",", t.Upcoming().Select(e => e.OwnerUnitId));
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }
        Assert.Equal(6, counts.Count);
        Assert.All(counts.Values, c => Assert.InRange(c, 850, 1150)); // about 1000 each
    }

    // ---- manipulation ----

    [Fact]
    public void HastePreservesProgressAndShortensTheRemainingWait()
    {
        // 20.5: after 4 of 11 units, Haste +2 turns the remaining 7 into 5.
        var t = new Timeline();
        var other = t.Add(TimelineKind.DelayedEvent, 4);
        var turn = t.Add(TimelineKind.UnitTurn, 11, 0);
        t.Pop(); // advance to time 4
        Assert.Equal(4, t.Now);
        Assert.Equal(7, t.Remaining(turn));
        t.ChangeSpeed(0, 5, 7);
        Assert.Equal(5, t.Remaining(turn));
        Assert.Equal(9, turn.Due);
        Assert.NotNull(other);
    }

    [Fact]
    public void SlowLengthensTheRemainingWaitAndExpiryReversesIt()
    {
        var t = new Timeline();
        var turn = t.Add(TimelineKind.UnitTurn, 10, 0);
        t.ChangeSpeed(0, 6, 4);
        Assert.Equal(12, t.Remaining(turn));
        t.ChangeSpeed(0, 4, 6);
        Assert.Equal(10, t.Remaining(turn));
    }

    [Fact]
    public void SpeedChangeIsMeasuredWithinOneToTen()
    {
        var t = new Timeline();
        var turn = t.Add(TimelineKind.UnitTurn, 10, 0);
        t.ChangeSpeed(0, 9, 11); // 9 -> 10 as far as the formula is concerned
        Assert.Equal(9, t.Remaining(turn));
    }

    [Fact]
    public void PullForwardRemovesTimeAndOvershootIsLost()
    {
        var t = new Timeline();
        var turn = t.Add(TimelineKind.UnitTurn, 10, 0);
        t.PullForward(turn, 3);
        Assert.Equal(7, t.Remaining(turn));
        t.PullForward(turn, 50);
        Assert.Equal(0, t.Remaining(turn)); // arrives now
        Assert.Equal(0, turn.Due);
        // The lost extra does not shorten a later Wait.
        Assert.Equal(11, Timeline.Wait(5, 5));
    }

    [Fact]
    public void PushBackAddsTimeAndResetRestoresTheLastWaitFromNow()
    {
        var t = new Timeline();
        var tick = t.Add(TimelineKind.DelayedEvent, 4);
        var turn = t.Add(TimelineKind.UnitTurn, 11, 0);
        t.Pop();
        Assert.Equal(4, t.Now);
        Assert.Equal(4, tick.Due);
        t.PushBack(turn, 5);
        Assert.Equal(12, t.Remaining(turn));
        t.Reset(turn);
        Assert.Equal(11, t.Remaining(turn));
        Assert.Equal(15, turn.Due);
    }

    [Fact]
    public void ANewTurnIsScheduledFromNow()
    {
        var t = new Timeline();
        t.Add(TimelineKind.DelayedEvent, 6);
        t.Pop(); // now 6
        var turn = t.ScheduleTurn(0, delay: 5, speed: 5)!;
        Assert.Equal(17, turn.Due);
    }

    [Fact]
    public void AUnitCannotHaveTwoTurns()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 5, 0);
        Assert.Throws<InvalidOperationException>(() => t.Add(TimelineKind.UnitTurn, 6, 0));
    }

    [Fact]
    public void WaitsMustBeAtLeastOne()
    {
        var t = new Timeline();
        Assert.Throws<ArgumentOutOfRangeException>(() => t.Add(TimelineKind.DelayedEvent, 0));
    }

    // ---- removal and auto events ----

    [Fact]
    public void RemovingAnOwnersEventsKeepsOrDropsItsTurn()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 5, 0);
        t.Add(TimelineKind.AutoEvent, 3, 0, "shot");
        t.Add(TimelineKind.DelayedEvent, 4, 0, "comet");
        t.Add(TimelineKind.EffectEnd, 6, 0, "ward");
        t.RemoveOwnedBy(0, includeTurn: false);
        Assert.Equal(new[] { TimelineKind.UnitTurn, TimelineKind.EffectEnd }, t.Upcoming().Select(e => e.Kind));
        t.RemoveOwnedBy(0, includeTurn: true);
        Assert.Equal(new[] { TimelineKind.EffectEnd }, t.Upcoming().Select(e => e.Kind));
    }

    [Fact]
    public void BlockingAutoEventsHoldBackTheOwnersTurnUntilTheLastOneIsDone()
    {
        var t = new Timeline();
        // The skill set the owner's next Wait first (11), then created two blocking auto events.
        t.Add(TimelineKind.UnitTurn, 11, 0);
        t.Add(TimelineKind.AutoEvent, 3, 0, "a", blocking: true);
        t.Add(TimelineKind.AutoEvent, 6, 0, "b", blocking: true);
        Assert.Null(t.TurnOf(0));
        Assert.Equal("a", t.Pop()!.Tag);
        Assert.Null(t.TurnOf(0));
        Assert.Equal("b", t.Pop()!.Tag);
        Assert.Equal(6, t.Now);
        // The turn cost starts after the last blocking event: now 6 + 11.
        Assert.Equal(17, t.TurnOf(0)!.Due);
    }

    [Fact]
    public void RemovingTheLastBlockingEventAlsoReleasesTheTurn()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 11, 0);
        var a = t.Add(TimelineKind.AutoEvent, 3, 0, "a", blocking: true);
        Assert.True(t.Remove(a.Id));
        Assert.Equal(11, t.TurnOf(0)!.Due);
    }

    [Fact]
    public void ATurnScheduledWhileBlockingEventsArePendingWaitsForThem()
    {
        var t = new Timeline();
        t.Add(TimelineKind.AutoEvent, 4, 0, "a", blocking: true);
        Assert.Null(t.ScheduleTurnWithWait(0, 9));
        t.Pop();
        Assert.Equal(13, t.TurnOf(0)!.Due);
    }

    [Fact]
    public void HasteOnTheOwnerOnlyChangesPendingBlockingEvents()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 11, 0);
        var a = t.Add(TimelineKind.AutoEvent, 8, 0, "a", blocking: true);
        t.ChangeSpeed(0, 5, 7);
        Assert.Equal(6, t.Remaining(a));
        t.Pop();
        Assert.Equal(17, t.TurnOf(0)!.Due); // 6 + the full 11: the turn's cost was not touched
    }

    [Fact]
    public void KoOfTheOwnerDropsItsBlockingEventsAndTheHeldTurn()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 11, 0);
        t.Add(TimelineKind.AutoEvent, 3, 0, "a", blocking: true);
        t.RemoveOwnedBy(0, includeTurn: true);
        Assert.Equal(0, t.Count);
    }

    [Fact]
    public void OnlyAutoEventsCanBlock()
    {
        var t = new Timeline();
        Assert.Throws<ArgumentException>(() => t.Add(TimelineKind.DelayedEvent, 3, 0, blocking: true));
    }

    // ---- state ----

    [Fact]
    public void CloneIsIndependent()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 5, 0);
        t.Add(TimelineKind.AutoEvent, 3, 0, "a", blocking: true);
        var copy = t.Clone();
        copy.Pop(); // the blocking event: releases the held turn in the copy only
        Assert.Equal(1, t.Count); // only the blocking event: the turn is held back
        Assert.Equal(0, t.Now);
        Assert.NotEqual(0, copy.Now);
        Assert.Null(t.TurnOf(0)); // still held back in the original
        Assert.NotNull(copy.TurnOf(0));
    }

    [Fact]
    public void PoppingAnEmptyTimelineGivesNull() => Assert.Null(new Timeline().Pop());

    [Fact]
    public void BattleStateOwnsAClonedTimeline()
    {
        var state = new BattleState(1);
        state.Timeline.Add(TimelineKind.UnitTurn, 5, 0);
        var copy = state.Clone();
        copy.Timeline.Pop();
        Assert.Equal(1, state.Timeline.Count);
        Assert.Equal(0, copy.Timeline.Count);
    }

    [Fact]
    public void AutoEventsAndTurnsShareOnePositionOrder()
    {
        var t = new Timeline();
        t.Add(TimelineKind.UnitTurn, 5, 0);
        t.Add(TimelineKind.AutoEvent, 5, 1, "shot");
        t.Add(TimelineKind.DelayedEvent, 5, null, "comet");
        Assert.Equal(new[] { TimelineKind.UnitTurn, TimelineKind.AutoEvent, TimelineKind.DelayedEvent },
            t.Upcoming().Select(e => e.Kind));
    }
}
