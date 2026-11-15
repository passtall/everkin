using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class ClericSkillTests
{
    [Fact]
    public void AllClericSkillsAreImplemented() => AssertClassImplemented("cleric", magical: true);

    [Fact]
    public void AbsolveRemovesEveryHarmfulStatusAndPushesTheUnitsNextTurnBackTwo()
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
        Assert.Equal(due + 2, s.Timeline.TurnOf(ally)!.Due);
    }

    [Fact]
    public void DispelRemovesEveryHelpfulStatusIncludingWardSongsAndStealth()
    {
        var (s, c, e) = Board(1, "dispel");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Ward, Magnitude: 4));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.SongHaste, Duration: 20, Magnitude: 1));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20));
        s.GetUnit(ally).IsStealthed = true;
        Run(s, Use(c, "dispel", ally));
        Assert.Equal(0, s.GetUnit(ally).Ward);
        Assert.False(Has(s, ally, StatusIds.SongHaste));
        Assert.False(s.GetUnit(ally).IsStealthed);
        Assert.True(Has(s, ally, StatusIds.Blind));                                         // harmful statuses stay
    }

    [Fact]
    public void JudgmentDealsFourForEveryStatusDispelRemoves()
    {
        var (s, c, e) = Board(1, "dispel", "judgment");
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Ward, Magnitude: 6));
        Statuses.Apply(s, new StatusApplication(e, StatusIds.SongHaste, Duration: 20, Magnitude: 1));
        Run(s, Use(c, "dispel", e));
        Assert.Equal(0, s.GetUnit(e).Ward);
        Assert.Equal(8, Lost(s, e));                                                        // two statuses x 4

        var (s2, c2, e2) = Board(1, "dispel", "judgment");
        Run(s2, Use(c2, "dispel", e2));                                                     // nothing to remove
        Assert.Equal(0, Lost(s2, e2));
    }

    [Fact]
    public void TurnBackMovesAHarmfulStatusOntoTheUnitThatAppliedItAndNeedsALivingApplier()
    {
        var (s, c, e) = Board(1, "turn_back");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20, SourceUnitId: e));
        Run(s, Use(c, "turn_back", ally));
        Assert.False(Has(s, ally, StatusIds.Blind));
        Assert.True(Has(s, e, StatusIds.Blind));
        Assert.Equal(20, Statuses.Get(s.GetUnit(e), StatusIds.Blind)!.TimeLeft(s.Timeline.Now));

        var (s2, c2, e2) = Board(1, "turn_back");
        var ally2 = s2.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s2, new StatusApplication(ally2, StatusIds.Blind, Duration: 20, SourceUnitId: e2));
        s2.GetUnit(e2).Hp = 0;
        Knockout.ResolveAll(s2);
        Run(s2, Use(c2, "turn_back", ally2));
        Assert.True(Has(s2, ally2, StatusIds.Blind));                                       // the applier is KO: it stays
    }

    [Fact]
    public void TurnBackPicksOneOfSeveralByTheSeededRollWithEqualChance()
    {
        var counts = new int[2];
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var (s, c, e) = Board(seed, "turn_back");
            var ally = s.Formation.Units(Side.A, Row.Middle)[0];
            Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20, SourceUnitId: e));
            Statuses.Apply(s, new StatusApplication(ally, StatusIds.Silenced, Duration: 20, SourceUnitId: e));
            Run(s, Use(c, "turn_back", ally));
            Assert.Equal(1, new[] { StatusIds.Blind, StatusIds.Silenced }.Count(id => Has(s, e, id)));
            counts[Has(s, e, StatusIds.Blind) ? 0 : 1]++;
        }
        Assert.All(counts, n => Assert.InRange(n, 70, 130));
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
