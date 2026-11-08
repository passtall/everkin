using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class BattleDriverTests
{
    private static BattleState Duel(ulong seed, int speedA = 5, int speedB = 5)
    {
        var s = new BattleState(seed);
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var b = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        s.GetUnit(a).Speed = speedA;
        s.GetUnit(b).Speed = speedB;
        s.Timeline.ScheduleInitialTurns(s.Units, s.Random);
        s.DrainEvents();
        return s;
    }

    [Fact]
    public void TheFasterUnitActsFirstAndTheClockJumpsToItsTurn()
    {
        var s = Duel(1, speedA: 8, speedB: 5);
        Assert.Equal(0, BattleDriver.NextTurn(s));
        Assert.Equal(3, s.Timeline.Now);                     // 11 - 8
        Assert.Contains(s.DrainEvents(), e => e is TurnStarted { UnitId: 0, Time: 3 });
    }

    [Fact]
    public void AfterAnActionTheNextTurnComesWhenTheWaitIsUp()
    {
        var s = Duel(1, speedA: 8, speedB: 5);
        var first = BattleDriver.NextTurn(s)!.Value;
        Actions.Use(s, World.Catalog, new SkillUse(first, "brawl_strike", 1));
        // Unit 0 acts again at 3 + (3 + 3) = 9, unit 1 at 6: unit 1 is next.
        Assert.Equal(1, BattleDriver.NextTurn(s));
        Assert.Equal(6, s.Timeline.Now);
    }

    [Fact]
    public void AFrozenUnitsTurnIsSkippedAndItWaitsTheSkippedTurnCost()
    {
        var s = Duel(1, speedA: 8, speedB: 5);
        Statuses.Apply(s, new StatusApplication(0, StatusIds.Frozen));
        s.DrainEvents();
        Assert.Equal(1, BattleDriver.NextTurn(s));            // unit 0 was skipped: its turn at 3 did not happen
        Assert.Contains(s.DrainEvents(), e => e is TurnSkipped { UnitId: 0 });
        Assert.Equal(3 + Timeline.SkippedTurnWait(8), s.Timeline.TurnOf(0)!.Due);
        Assert.False(Statuses.Has(s.GetUnit(0), StatusIds.Frozen));
    }

    [Fact]
    public void AStunnedUnitsTurnsAreSkippedUntilTheStunRunsOut()
    {
        var s = Duel(1, speedA: 8, speedB: 5);
        Statuses.Apply(s, new StatusApplication(0, StatusIds.Stunned, Duration: 8));
        Assert.Equal(1, BattleDriver.NextTurn(s));            // turn at 3: skipped, Stunned still on
        Assert.True(Statuses.Has(s.GetUnit(0), StatusIds.Stunned));
        Assert.Equal(3 + Timeline.SkippedTurnWait(8), s.Timeline.TurnOf(0)!.Due);   // skipped at the Delay 5 cost
    }

    [Fact]
    public void PoisonAndGroundDamageLandWheneverAnyUnitGetsATurn()
    {
        var s = Duel(1, speedA: 5, speedB: 5);
        Statuses.Apply(s, new StatusApplication(1, StatusIds.Poison, Duration: 50));
        var first = BattleDriver.NextTurn(s)!.Value;          // time 6: 60 hundredths built up
        Assert.Equal(6, s.Timeline.Now);
        Assert.Equal(40, s.GetUnit(1).Hp);
        Actions.SkipTurn(s, first);
        BattleDriver.NextTurn(s);                              // the other unit's turn at 6 too, then time 17
        Assert.True(s.GetUnit(1).Hp <= 40);
    }

    [Fact]
    public void ATickThatKosTheUnitEndsItsTurn()
    {
        var s = Duel(1, speedA: 5, speedB: 5);
        s.GetUnit(0).Hp = 1;
        Statuses.Apply(s, new StatusApplication(0, StatusIds.Poison, Duration: 50));
        s.Timeline.Add(TimelineKind.DelayedEvent, 1);
        // Both turns are at 6; the poison has built up 60 hundredths: nothing lands yet. Push time on.
        s.Timeline.PushBack(s.Timeline.TurnOf(0)!, 10);        // unit 0's turn at 16: 160 hundredths -> 1 lands, KO
        Assert.Equal(1, BattleDriver.NextTurn(s));
        Actions.SkipTurn(s, 1);
        Assert.Null(BattleDriver.NextTurn(s));                 // unit 0 was KO'd by the tick before its turn: the battle is over
        Assert.True(s.IsOver);
    }

    [Fact]
    public void AnEmptyTimelineOrAFinishedBattleReturnsNull()
    {
        var s = new BattleState(1);
        Assert.Null(BattleDriver.NextTurn(s));
        var d = Duel(1);
        BattleEnd.Surrender(d, Side.A);
        Assert.Null(BattleDriver.NextTurn(d));
    }

    // ---- a whole battle ----

    /// <summary>A trivial policy: every unit strikes the nearest legal enemy (ties by the battle's own roll).</summary>
    private static (BattleOutcome Outcome, int Turns, int Hash) Play(ulong seed)
    {
        var s = new BattleState(seed);
        var rows = new[] { Row.Front, Row.Middle };
        var units = new List<int>();
        foreach (var side in new[] { Side.A, Side.B })
            for (var i = 0; i < 3; i++)
            {
                var id = World.Unit(s, side, rows[i % 2], "brawl_strike", "polearm_thrust", "shot", "breath", "pulse");
                units.Add(id);
                s.GetUnit(id).Speed = 3 + ((i * 3 + (side == Side.A ? 0 : 1)) % 6);
            }
        s.Timeline.ScheduleInitialTurns(s.Units, s.Random);
        var hash = 17;
        var turns = 0;
        while (BattleDriver.NextTurn(s) is { } unit && turns++ < 500)
        {
            var best = Targeting.PickNearest(s, unit, Reach.Unlimited);
            if (best is { } target && Actions.WhyNot(s, World.Catalog, new SkillUse(unit, "shot", target)) is null)
                hash = hash * 31 + Actions.Use(s, World.Catalog, new SkillUse(unit, "shot", target)).Count;
            else
                hash = hash * 31 + Actions.SkipTurn(s, unit).Count;
        }
        return (s.Outcome, turns, hash);
    }

    [Fact]
    public void AWholeBattleRunsToAnEndAndIsTheSameEveryTimeFromTheSameSeed()
    {
        var a = Play(7);
        var b = Play(7);
        Assert.Equal(a, b);
        Assert.NotEqual(BattleOutcome.Ongoing, a.Outcome);
        Assert.InRange(a.Turns, 6, 500);
    }

    [Fact]
    public void DifferentSeedsPlayOutDifferentBattles()
    {
        var results = Enumerable.Range(1, 20).Select(i => Play((ulong)i)).ToList();
        Assert.True(results.Select(r => r.Hash).Distinct().Count() > 10);
        Assert.All(results, r => Assert.NotEqual(BattleOutcome.Ongoing, r.Outcome));
    }
}
