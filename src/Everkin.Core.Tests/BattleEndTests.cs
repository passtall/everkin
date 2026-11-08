using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class BattleEndTests
{
    private static BattleState TwoVsTwo(out int[] a, out int[] b)
    {
        var s = new BattleState(1);
        a = new[] { Place.Unit(s, Side.A, Row.Front), Place.Unit(s, Side.A, Row.Rear) };
        b = new[] { Place.Unit(s, Side.B, Row.Front), Place.Unit(s, Side.B, Row.Rear) };
        s.DrainEvents();
        return s;
    }

    private static void Ko(BattleState s, int id)
    {
        s.GetUnit(id).Hp = 0;
        Knockout.Resolve(s, id);
    }

    [Fact]
    public void ABattleStartsOngoing()
    {
        var s = TwoVsTwo(out _, out _);
        Assert.Equal(BattleOutcome.Ongoing, BattleEnd.Check(s));
        Assert.False(BattleEnd.Resolve(s));
        Assert.False(s.IsOver);
    }

    [Fact]
    public void ASideLosesWhenAllItsOriginalMembersAreKo()
    {
        var s = TwoVsTwo(out var a, out var b);
        Ko(s, a[0]);
        Assert.Equal(BattleOutcome.Ongoing, BattleEnd.Check(s));      // one is still standing
        Ko(s, a[1]);
        Assert.True(BattleEnd.Resolve(s));
        Assert.Equal(BattleOutcome.SideBWins, s.Outcome);
        Assert.Equal(EndReason.AllOriginalsKo, s.EndReason);
        Assert.Contains(new BattleEnded(BattleOutcome.SideBWins, EndReason.AllOriginalsKo), s.DrainEvents());
        Assert.False(s.GetUnit(b[0]).IsKnockedOut);
    }

    [Fact]
    public void ActiveSummonsDoNotPreventTheLoss()
    {
        // 20.5: all original members are KO while a summon survives: the party loses.
        var s = TwoVsTwo(out var a, out _);
        var golem = Summons.Summon(s,
            new SummonSpec(Side.A, "Golem", UnitKind.Summon, 25, 3, a[0], TakesTurns: true), Row.Middle, 0);
        Ko(s, a[0]);
        Ko(s, a[1]);
        Assert.False(s.GetUnit(golem).IsKnockedOut);
        Assert.Equal(BattleOutcome.SideBWins, BattleEnd.Check(s));
    }

    [Fact]
    public void ARevivedOriginalCountsAsActiveAgain()
    {
        var s = TwoVsTwo(out var a, out _);
        Ko(s, a[0]);
        Ko(s, a[1]);
        Assert.True(BattleEnd.IsDefeated(s, Side.A));
        Knockout.Revive(s, a[0], 10, 9);
        Assert.False(BattleEnd.IsDefeated(s, Side.A));
        Assert.Equal(BattleOutcome.Ongoing, BattleEnd.Check(s));
    }

    [Fact]
    public void AnOriginalRaisedAsUndeadStillCountsAsKoForItsOwner()
    {
        var s = TwoVsTwo(out var a, out var b);
        Ko(s, b[0]);
        Necromancy.Raise(s, a[0], b[0], Row.Middle, 0, hp: 10);        // B's first original fights for A now
        Assert.True(s.GetUnit(b[0]).IsUndead);
        Assert.False(BattleEnd.IsDefeated(s, Side.B));                  // b[1] still stands
        Ko(s, b[1]);
        Assert.True(BattleEnd.IsDefeated(s, Side.B));                   // the undead original counts as KO for B
        Assert.Equal(BattleOutcome.SideAWins, BattleEnd.Check(s));
    }

    [Fact]
    public void BothLastOriginalsKoInOneBlockIsADraw()
    {
        // 20.5.
        var s = TwoVsTwo(out var a, out var b);
        Ko(s, a[0]);
        Ko(s, b[0]);
        s.GetUnit(a[1]).Hp = 0;
        s.GetUnit(b[1]).Hp = 0;
        Knockout.ResolveAll(s);
        Assert.True(BattleEnd.Resolve(s));
        Assert.Equal(BattleOutcome.Draw, s.Outcome);
        Assert.Equal(EndReason.Simultaneous, s.EndReason);
    }

    [Fact]
    public void VictoryIsCheckedAfterReactionsSoACounterCanTurnAWinIntoADraw()
    {
        // 20.5: a player's last original member is KO'd by a counter after the player's attack KO'd the enemy's last member.
        var s = new BattleState(1);
        var attacker = Place.Unit(s, Side.A, Row.Front);               // A's only original
        var target = Place.Unit(s, Side.B, Row.Front);                 // B's only original
        var summon = Summons.Summon(s,
            new SummonSpec(Side.B, "Thorn Spirit", UnitKind.Summon, 10, 5, target, TakesTurns: false), Row.Middle, 0);
        s.GetUnit(attacker).Hp = 1;
        s.AddReaction(new ReactionDef("avenger", summon, TriggerKind.WhenHit, (st, _) => st.DealDamage(summon, attacker, 5))
        {
            Condition = (_, t) => t.SubjectUnitId == target,
        });

        s.DealDamage(attacker, target, 99);
        Knockout.ResolveAll(s);
        Assert.Equal(BattleOutcome.SideAWins, BattleEnd.Check(s));      // before the reactions it looks like a win

        Reactions.Resolve(s, new[] { new Trigger(TriggerKind.WhenHit, target, attacker, EffectTags.Melee) });
        Knockout.ResolveAll(s);
        Assert.True(BattleEnd.Resolve(s));                              // after them it is a draw
        Assert.Equal(BattleOutcome.Draw, s.Outcome);
    }

    [Fact]
    public void ADecidedBattleIsNotDecidedAgain()
    {
        var s = TwoVsTwo(out var a, out _);
        Ko(s, a[0]);
        Ko(s, a[1]);
        Assert.True(BattleEnd.Resolve(s));
        s.DrainEvents();
        Assert.False(BattleEnd.Resolve(s));
        Assert.False(BattleEnd.Surrender(s, Side.B));
        Assert.Empty(s.DrainEvents());
        Assert.Equal(BattleOutcome.SideBWins, s.Outcome);
    }

    [Fact]
    public void ASurrenderIsADefeatForTheSurrenderingSide()
    {
        var s = TwoVsTwo(out _, out _);
        Assert.True(BattleEnd.Surrender(s, Side.A));
        Assert.Equal(BattleOutcome.SideBWins, s.Outcome);
        Assert.Equal(EndReason.Surrender, s.EndReason);
        Assert.Equal(PlayerResult.Defeat, BattleEnd.ResultFor(s.Outcome, Side.A, drawCountsAsDefeat: true));
        Assert.Equal(PlayerResult.Victory, BattleEnd.ResultFor(s.Outcome, Side.B, drawCountsAsDefeat: false));
    }

    [Fact]
    public void TheHarnessCapEndsABattleAsADraw()
    {
        // 20.5: a battle reaches the harness's technical cap: it is reported and recorded as a draw.
        var s = TwoVsTwo(out _, out _);
        for (var i = 0; i < 499; i++) s.CountTurn();
        Assert.False(BattleEnd.CheckTurnCap(s));
        s.CountTurn();
        Assert.True(BattleEnd.CheckTurnCap(s));
        Assert.Equal(BattleOutcome.Draw, s.Outcome);
        Assert.Equal(EndReason.TurnCap, s.EndReason);
    }

    [Fact]
    public void ADrawIsADefeatInSinglePlayerAndASharedDrawElsewhere()
    {
        // 20.5.
        Assert.Equal(PlayerResult.Defeat, BattleEnd.ResultFor(BattleOutcome.Draw, Side.A, drawCountsAsDefeat: true));
        Assert.Equal(PlayerResult.Draw, BattleEnd.ResultFor(BattleOutcome.Draw, Side.A, drawCountsAsDefeat: false));
        Assert.Equal(PlayerResult.Victory, BattleEnd.ResultFor(BattleOutcome.SideAWins, Side.A, true));
        Assert.Equal(PlayerResult.Defeat, BattleEnd.ResultFor(BattleOutcome.SideAWins, Side.B, true));
        Assert.Throws<InvalidOperationException>(() => BattleEnd.ResultFor(BattleOutcome.Ongoing, Side.A, true));
    }

    [Fact]
    public void ASideWithADifferentNumberOfOriginalsUsesItsOwn()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b1 = Place.Unit(s, Side.B, Row.Front);
        var b2 = Place.Unit(s, Side.B, Row.Front);
        var b3 = Place.Unit(s, Side.B, Row.Front);
        Ko(s, b1);
        Ko(s, b2);
        Assert.Equal(BattleOutcome.Ongoing, BattleEnd.Check(s));
        Ko(s, b3);
        Assert.Equal(BattleOutcome.SideAWins, BattleEnd.Check(s));
        Assert.False(s.GetUnit(a).IsKnockedOut);
    }

    [Fact]
    public void TheOutcomeSurvivesCloneAndRemovedSummonsNeverMatter()
    {
        var s = TwoVsTwo(out var a, out _);
        Ko(s, a[0]);
        Ko(s, a[1]);
        BattleEnd.Resolve(s);
        var copy = s.Clone();
        Assert.Equal(BattleOutcome.SideBWins, copy.Outcome);
        Assert.Equal(EndReason.AllOriginalsKo, copy.EndReason);
        Assert.True(copy.IsOver);
    }
}
