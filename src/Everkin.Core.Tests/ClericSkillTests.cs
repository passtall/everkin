using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class ClericSkillTests
{
    [Fact]
    public void AllClericSkillsAreImplemented() => AssertClassImplemented("cleric", magical: true);

    [Fact]
    public void AbsolveRemovesEveryHarmfulStatusAndKeepsNewOnesOffForTen()
    {
        var (s, c, e) = Board(1, "absolve");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Poison, Duration: 20, SourceUnitId: e));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Ward, Magnitude: 4));
        s.Timeline.ScheduleTurn(ally, 5, 5);
        var due = s.Timeline.TurnOf(ally)!.Due;
        Run(s, Use(c, "absolve", ally));
        Assert.False(Has(s, ally, StatusIds.Poison));
        Assert.False(Has(s, ally, StatusIds.Blind));
        Assert.Equal(4, s.GetUnit(ally).Ward);                                              // helpful statuses stay
        Assert.Equal(due, s.Timeline.TurnOf(ally)!.Due);                                    // no push back any more
        Assert.True(Has(s, ally, StatusIds.Absolved));
        Assert.False(Statuses.Apply(s, new StatusApplication(ally, StatusIds.Stunned, Duration: 5)));
        AdvanceTo(s, 10);
        Assert.False(Has(s, ally, StatusIds.Absolved));
        Assert.True(Statuses.Apply(s, new StatusApplication(ally, StatusIds.Stunned, Duration: 5)));
    }

    [Fact]
    public void SmiteTheAfflictedAddsTwoPerHarmfulStatusUpToSixExtra()
    {
        var (s0, c0, e0) = Board(1, "smite_the_afflicted");
        Run(s0, Use(c0, "smite_the_afflicted", e0));
        Assert.InRange(Lost(s0, e0), 3, 4);

        var (s2, c2, e2) = Board(1, "smite_the_afflicted");
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Bleeding, Duration: 20, Uses: 3));
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Ward, Magnitude: 0));          // a helpful status adds nothing
        Run(s2, Use(c2, "smite_the_afflicted", e2));
        Assert.InRange(Lost(s2, e2), 7, 8);

        var (s5, c5, e5) = Board(1, "smite_the_afflicted");
        foreach (var id in new[] { StatusIds.Blind, StatusIds.Silenced, StatusIds.Confused, StatusIds.Immobilized, StatusIds.Marked })
            Statuses.Apply(s5, new StatusApplication(e5, id, Duration: 20));
        Run(s5, Use(c5, "smite_the_afflicted", e5));
        Assert.InRange(Lost(s5, e5), 9, 10);                                                // 3-4 plus the cap of 6
    }

    [Fact]
    public void DivineInterventionCleansesEveryAllyAndWardsThemThree()
    {
        var (s, c, e) = Board(1, "divine_intervention");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20, SourceUnitId: e));
        Statuses.Apply(s, new StatusApplication(c, StatusIds.Poison, Duration: 20, SourceUnitId: e));
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Blind, Duration: 20));
        Run(s, Use(c, "divine_intervention"));
        Assert.False(Has(s, ally, StatusIds.Blind));
        Assert.False(Has(s, c, StatusIds.Poison));
        Assert.Equal(3, s.GetUnit(ally).Ward);
        Assert.Equal(3, s.GetUnit(c).Ward);
        Assert.True(Has(s, e, StatusIds.Blind));                                            // enemies are not touched
        Assert.Equal(0, s.GetUnit(e).Ward);
    }

    [Fact]
    public void JudgmentDealsFourToTheEnemyThatAppliedEachRemovedStatus()
    {
        var (s, c, e) = Board(1, "absolve", "judgment");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20, SourceUnitId: e));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Silenced, Duration: 20, SourceUnitId: e));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Confused, Duration: 20));       // no applier: nothing
        Run(s, Use(c, "absolve", ally));
        Assert.Equal(8, Lost(s, e));                                                            // two statuses x 4

        var (s2, c2, e2) = Board(1, "absolve");                                                 // without the keystone
        var ally2 = s2.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s2, new StatusApplication(ally2, StatusIds.Blind, Duration: 20, SourceUnitId: e2));
        Run(s2, Use(c2, "absolve", ally2));
        Assert.Equal(0, Lost(s2, e2));
    }

    [Fact]
    public void SanctityKeepsEveryHarmfulStatusOffTheClericButNotHelpfulOnes()
    {
        var (s, c, e) = Board(1, "sanctity");
        Assert.False(Statuses.Apply(s, new StatusApplication(c, StatusIds.Blind, Duration: 20)));
        Assert.False(Statuses.Apply(s, new StatusApplication(c, StatusIds.Stunned, Duration: 20)));
        Assert.True(Statuses.Apply(s, new StatusApplication(c, StatusIds.Ward, Magnitude: 3)));
        Assert.True(Statuses.Apply(s, new StatusApplication(e, StatusIds.Blind, Duration: 20)));
    }

    [Fact]
    public void PurifyingAuraMakesHarmfulStatusesOnAnAdjacentAllyRunDownTwiceAsFast()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "purifying_aura");
        var next = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Rear);
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(next, StatusIds.Blind, Duration: 20, SourceUnitId: e));
        Statuses.Apply(s, new StatusApplication(far, StatusIds.Blind, Duration: 20, SourceUnitId: e));
        s.RunAuras();
        Assert.Equal(10, Statuses.Get(s.GetUnit(next), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));
        Assert.Equal(20, Statuses.Get(s.GetUnit(far), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));
        AdvanceTo(s, 10);
        Assert.False(Has(s, next, StatusIds.Blind));
        Assert.True(Has(s, far, StatusIds.Blind));
    }

    [Fact]
    public void PurifyingAuraReachesTheWholeRowIncludingTheCleric()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "purifying_aura");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.A, Row.Front);
        var third = World.Unit(s, Side.A, Row.Front);
        var rear = World.Unit(s, Side.A, Row.Rear);
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        foreach (var id in new[] { c, third, rear })
            Statuses.Apply(s, new StatusApplication(id, StatusIds.Blind, Duration: 20, SourceUnitId: e));
        s.RunAuras();
        Assert.Equal(10, Statuses.Get(s.GetUnit(c), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));
        Assert.Equal(10, Statuses.Get(s.GetUnit(third), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));   // not next to the Cleric, but in its row
        Assert.Equal(20, Statuses.Get(s.GetUnit(rear), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void PurifyingAuraStopsWhenTheAllyLeavesAndAppliesToHelpfulStatusesNever()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "purifying_aura");
        var next = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(next, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(next, StatusIds.SongHaste, Duration: 20, Magnitude: 1));
        s.RunAuras();
        AdvanceTo(s, 4);                                                                    // 4 time units pass next to the Cleric: 8 of Blind used
        Assert.Equal(16, Statuses.Get(s.GetUnit(next), StatusIds.SongHaste)!.TimeLeft(s.Timeline.Now));
        Actions.Move(s, next, Row.Rear, 0);                                                 // leaves: the clock runs normally again
        s.RunAuras();
        Assert.Equal(12, Statuses.Get(s.GetUnit(next), StatusIds.Blind)!.TimeLeft(s.Timeline.Now) * 1 - 0);
    }
}
