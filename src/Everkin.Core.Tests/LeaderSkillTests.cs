using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class LeaderSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null, MoveOption? place = null, int? second = null) =>
        new(user, skill, target, row, place, second);

    private static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    private static Row RowOf(BattleState s, int unit) => s.Formation.PositionOf(unit).Row;

    private static (BattleState S, int Leader) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var leader = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 5);
        return (s, leader);
    }

    [Fact]
    public void AllFifteenLeaderSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "leader").ToList();
        Assert.Equal(15, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
        }
    }

    // ---- Tactician ----

    [Fact]
    public void SwapTurnsExchangesTheDueTimesOfTwoAllies()
    {
        var (s, leader) = Board(1, "press_the_advantage", "swap_turns");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        s.Timeline.ScheduleTurn(a, 2, 5);
        s.Timeline.ScheduleTurn(b, 8, 5);
        var (dueA, dueB) = (s.Timeline.TurnOf(a)!.Due, s.Timeline.TurnOf(b)!.Due);
        Run(s, Use(leader, "swap_turns", a, second: b));
        Assert.Equal(dueB, s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(dueA, s.Timeline.TurnOf(b)!.Due);
        Assert.NotNull(Why(s, Use(leader, "swap_turns", a, second: World.Unit(s, Side.B, Row.Middle))));   // own side only
    }

    [Fact]
    public void PressTheAdvantageMakesTheNextSkillFreeOnce()
    {
        var (s, leader) = Board(1, "press_the_advantage", "swap_turns");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var other = World.Unit(s, Side.A, Row.Rear);
        Run(s, Use(leader, "press_the_advantage", leader));
        Assert.Equal(3, s.Momentum.AdvantageOf(Side.A));                  // press itself cost 2
        Ready(s, leader);
        Run(s, Use(leader, "swap_turns", ally, second: other));
        Assert.Equal(3, s.Momentum.AdvantageOf(Side.A));                  // free
        Assert.False(Statuses.Has(s.GetUnit(leader), StatusIds.FreeMomentum));
        Ready(s, leader);
        Run(s, Use(leader, "swap_turns", ally, second: other));
        Assert.Equal(1, s.Momentum.AdvantageOf(Side.A));                  // paid again

        s.ShiftMomentum(Side.A, -10);
        Statuses.Apply(s, new StatusApplication(leader, StatusIds.FreeMomentum));
        Assert.Null(Why(s, Use(leader, "swap_turns", ally, second: other)));   // no lead needed
    }

    [Fact]
    public void AllInHalvesCostsRoundedUpAndZeroesGainedMomentum()
    {
        var (s, leader) = Board(1, "press_the_advantage", "inspiring_presence", "all_in", "brawl_strike");
        var enemy = s.Units.First(u => u.Side == Side.B).Id;
        var ally = World.Unit(s, Side.A, Row.Middle);
        Run(s, Use(leader, "press_the_advantage", ally));
        Assert.Equal(4, s.Momentum.AdvantageOf(Side.A));                  // cost 2 became 1
        Ready(s, leader);
        Run(s, Use(leader, "brawl_strike", enemy));
        Assert.True(s.GetUnit(enemy).Hp < 40);
        Assert.Equal(4, s.Momentum.AdvantageOf(Side.A));                  // the landed hit gained nothing
    }

    [Fact]
    public void InspiringPresenceAddsOneMomentumWhenAnAdjacentAllyKosAnEnemy()
    {
        var (s, leader) = Board(1, "press_the_advantage", "inspiring_presence");
        var ally = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var far = World.Unit(s, Side.A, Row.Rear, "shot");
        var enemy = s.Units.First(u => u.Side == Side.B).Id;
        var e2 = World.Unit(s, Side.B, Row.Front);
        var e3 = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(enemy).Hp = 1;
        s.GetUnit(e2).Hp = 1;
        s.GetUnit(e3).Hp = 1;
        var before = s.Momentum.AdvantageOf(Side.A);
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.True(s.GetUnit(enemy).IsKnockedOut);
        Assert.Equal(before + 2, s.Momentum.AdvantageOf(Side.A));         // the hit's 1 and the song's 1

        before = s.Momentum.AdvantageOf(Side.A);
        Run(s, Use(far, "shot", e2));                                     // the Rear row is not next to the Leader
        Assert.True(s.Momentum.AdvantageOf(Side.A) <= before + 1);
    }

    // ---- Commander ----

    [Fact]
    public void RepositionMovesAnAllyOneRowToAnyGapWithoutTouchingItsTimeline()
    {
        var (s, leader) = Board(1, "reposition");
        var ally = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.A, Row.Rear);
        s.Timeline.ScheduleTurn(ally, 4, 5);
        var due = s.Timeline.TurnOf(ally)!.Due;
        var options = World.Catalog.Get("reposition").PlacementOptionsFor(s, leader, ally);
        Assert.Contains(new MoveOption(Row.Rear, 1), options);
        Assert.DoesNotContain(new MoveOption(Row.Rear, 5), options);
        Run(s, Use(leader, "reposition", ally, place: new MoveOption(Row.Rear, 1)));
        Assert.Equal(Row.Rear, RowOf(s, ally));
        Assert.Equal(1, s.Formation.PositionOf(ally).Index);
        Assert.Equal(due, s.Timeline.TurnOf(ally)!.Due);
        Assert.NotNull(Why(s, Use(leader, "reposition", s.Units.First(u => u.Side == Side.B).Id, place: new MoveOption(Row.Middle, 0))));
    }

    [Fact]
    public void FallBackMovesTheFrontRowOneRowBackAndDrillPullsTheirTurnsForward()
    {
        var (s, leader) = Board(1, "reposition", "fall_back", "switch_places", "drill");
        var a = World.Unit(s, Side.A, Row.Front);
        var back = World.Unit(s, Side.A, Row.Middle);
        foreach (var id in new[] { leader, a, back }) s.Timeline.ScheduleTurn(id, 6, 5);
        var due = s.Timeline.TurnOf(a)!.Due;
        Run(s, Use(leader, "fall_back"));
        Assert.Equal(Row.Middle, RowOf(s, leader));
        Assert.Equal(Row.Middle, RowOf(s, a));
        Assert.Equal(due - 3, s.Timeline.TurnOf(a)!.Due);                  // Drill
        Assert.Equal(Row.Rear, RowOf(s, back));                            // bumped on
    }

    [Fact]
    public void AdvanceMovesARowForwardAndFrontCannotAdvance()
    {
        var (s, leader) = Board(1, "reposition", "advance");
        World.Unit(s, Side.A, Row.Front);                                      // two Front units: a pulled unit lands between them
        var a = World.Unit(s, Side.A, Row.Middle);
        var legal = World.Catalog.Get("advance").LegalRows(s, leader);
        Assert.Contains(new RowTarget(Side.A, Row.Middle), legal);
        Assert.DoesNotContain(new RowTarget(Side.A, Row.Front), legal);
        Assert.DoesNotContain(new RowTarget(Side.B, Row.Front), legal);
        Run(s, Use(leader, "advance", row: new RowTarget(Side.A, Row.Middle)));
        Assert.Equal(Row.Front, RowOf(s, a));
    }

    [Fact]
    public void SwitchPlacesSwapsTwoAlliesAndDrillPullsBothForward()
    {
        var (s, leader) = Board(1, "reposition", "switch_places", "drill");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        s.Timeline.ScheduleTurn(a, 6, 5);
        s.Timeline.ScheduleTurn(b, 6, 5);
        var due = s.Timeline.TurnOf(a)!.Due;
        Run(s, Use(leader, "switch_places", a, second: b));
        Assert.Equal(Row.Rear, RowOf(s, a));
        Assert.Equal(Row.Middle, RowOf(s, b));
        Assert.Equal(due - 3, s.Timeline.TurnOf(a)!.Due);
        Assert.Equal(due - 3, s.Timeline.TurnOf(b)!.Due);
    }

    [Fact]
    public void RotationTurnsTheRowsAndLeavesBodiesAndImmobilizedUnitsAlone()
    {
        var (s, leader) = Board(1, "reposition", "fall_back", "rotation");
        var mid = World.Unit(s, Side.A, Row.Middle);
        var rear = World.Unit(s, Side.A, Row.Rear);
        var stuck = World.Unit(s, Side.A, Row.Rear);
        Statuses.Apply(s, new StatusApplication(stuck, StatusIds.Immobilized, Duration: 50));
        s.ShiftMomentum(Side.A, 5);
        Run(s, Use(leader, "rotation"));
        Assert.Equal(Row.Middle, RowOf(s, leader));
        Assert.Equal(Row.Rear, RowOf(s, mid));
        Assert.Equal(Row.Front, RowOf(s, rear));
        Assert.Equal(Row.Rear, RowOf(s, stuck));                           // immobilized: stays
    }

    [Fact]
    public void RotationFailsWithoutDamageWhenARowWouldOverflow()
    {
        var (s, leader) = Board(1, "reposition", "fall_back", "rotation");
        for (var i = 0; i < 5; i++) World.Unit(s, Side.A, Row.Middle);
        var stayers = Enumerable.Range(0, 6).Select(_ => World.Unit(s, Side.A, Row.Rear)).ToList();   // a full Rear row
        foreach (var id in stayers) Statuses.Apply(s, new StatusApplication(id, StatusIds.Immobilized, Duration: 50));
        var hp = s.GetUnit(leader).Hp;
        Run(s, Use(leader, "rotation"));
        Assert.Equal(Row.Front, RowOf(s, leader));                         // Middle would get the Front unit + 5 + nobody moving in: 6 fits; Rear row of stayers is full and Middle units move in
        Assert.Equal(hp, s.GetUnit(leader).Hp);
    }

    // ---- Bard ----

    [Fact]
    public void SongOfHasteGivesAlliesInTheLeadersRowOneSpeedAndFollowsTheFormation()
    {
        var (s, leader) = Board(1, "song_of_haste");
        var same = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Middle);
        s.RunAuras();
        Assert.Equal(6, s.GetUnit(same).EffectiveSpeed);
        Assert.Equal(6, s.GetUnit(leader).EffectiveSpeed);
        Assert.Equal(5, s.GetUnit(other).EffectiveSpeed);
        Movement.ForceMove(s, same, Row.Middle, Side.B);
        s.RunAuras();
        Assert.Equal(5, s.GetUnit(same).EffectiveSpeed);
        s.GetUnit(leader).Hp = 0;
        Knockout.ResolveAll(s);
        s.RunAuras();
        Assert.Equal(5, s.GetUnit(other).EffectiveSpeed);
    }

    [Fact]
    public void SoloPerformerDoublesTheBardAndKeepsItInTheLeadersRow()
    {
        var (s, leader) = Board(1, "song_of_haste", "war_drums", "solo_performer");
        var same = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Middle);
        s.RunAuras();
        Assert.Equal(7, s.GetUnit(same).EffectiveSpeed);
        Assert.Equal(5, s.GetUnit(other).EffectiveSpeed);
        Assert.False(Statuses.Apply(s, new StatusApplication(same, StatusIds.Stunned, Duration: 10)));
        Assert.True(Statuses.Apply(s, new StatusApplication(other, StatusIds.Stunned, Duration: 10)));
        Assert.NotEqual(leader, other);
    }

    [Fact]
    public void GrandChorusReachesEveryAllyButLimitsTheLeaderToItsAttack()
    {
        var (s, leader) = Board(1, "song_of_haste", "rhythm", "grand_chorus", "press_the_advantage", "brawl_strike");
        var far = World.Unit(s, Side.A, Row.Rear);
        s.RunAuras();
        Assert.Equal(6, s.GetUnit(far).EffectiveSpeed);
        Assert.NotNull(Why(s, Use(leader, "press_the_advantage", far)));
        var enemy = s.Units.First(u => u.Side == Side.B).Id;
        Assert.Null(Why(s, Use(leader, "brawl_strike", enemy)));
    }

    [Fact]
    public void WarDrumsProtectTheLeaderAndItsNeighboursFromStunAndFrost()
    {
        var (s, leader) = Board(1, "song_of_haste", "war_drums");
        var next = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Rear);
        Assert.False(Statuses.Apply(s, new StatusApplication(leader, StatusIds.Stunned, Duration: 10)));
        Assert.False(Statuses.Apply(s, new StatusApplication(next, StatusIds.Frozen)));
        Assert.True(Statuses.Apply(s, new StatusApplication(far, StatusIds.Stunned, Duration: 10)));
        Assert.True(Statuses.Apply(s, new StatusApplication(next, StatusIds.Blind, Duration: 10)));
    }

    [Fact]
    public void RhythmGivesTheSecondUseOfTheSameSkillOneLessDelayUntilSomethingElseHappens()
    {
        var (s, leader) = Board(1, "rhythm");
        var ally = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var enemy = s.Units.First(u => u.Side == Side.B).Id;
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(ally)!.Due);
        Ready(s, ally);
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.Equal(Timeline.Wait(2, 5), s.Timeline.TurnOf(ally)!.Due);   // 1 less Delay
        Ready(s, ally);
        Actions.SkipTurn(s, ally);                                         // breaks the streak
        Ready(s, ally);
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(ally)!.Due);
        Assert.NotEqual(leader, ally);
    }
}
