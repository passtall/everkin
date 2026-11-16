using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Setup;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class AiTests
{
    private static AiSettings Settings => AiSettings.Load(Path.Combine(World.ContentDir, "ai_settings.json"));

    [Fact]
    public void RandomTeamsFitTheBudgetHaveNoDuplicatesAndLegalFormations()
    {
        for (ulong seed = 1; seed <= 30; seed++)
        {
            var spec = BattleSetup.RandomSpec(World.Content, seed);
            foreach (var team in new[] { spec.SideA, spec.SideB })
            {
                Assert.Equal(BattleSetup.TeamSize, team.Units.Count);
                Assert.InRange(team.TotalCost(World.Content), 6, BattleSetup.TeamBudget);
                Assert.Equal(team.Units.Count, team.Units.Select(u => u.CreatureId).Distinct().Count());
                Assert.All(team.Units.GroupBy(u => u.Row), g => Assert.True(g.Count() <= Formation.RowCapacity));
            }
        }
    }

    [Fact]
    public void ASeedGivesTheSameTeamsAndTheSameBattleSetup()
    {
        var a = BattleSetup.RandomSpec(World.Content, 42);
        var b = BattleSetup.RandomSpec(World.Content, 42);
        Assert.Equal(a.SideA.Units.Select(u => (u.CreatureId, u.Row)), b.SideA.Units.Select(u => (u.CreatureId, u.Row)));
        Assert.NotEqual(a.SideA.Units.Select(u => u.CreatureId), BattleSetup.RandomSpec(World.Content, 43).SideA.Units.Select(u => u.CreatureId));
        var s1 = BattleSetup.Create(a, World.Catalog);
        var s2 = BattleSetup.Create(b, World.Catalog);
        Assert.Equal(12, s1.Units.Count);
        Assert.Equal(s1.Timeline.Upcoming().Select(e => e.OwnerUnitId), s2.Timeline.Upcoming().Select(e => e.OwnerUnitId));
        Assert.All(s1.Units, u => Assert.Contains(World.Content.CreaturesById[u.CreatureId!].Attack, u.KnownSkills));
    }

    [Fact]
    public void ABattleBetweenTwoAisRunsToAnEndAndIsReproducible()
    {
        BattleResult Play(ulong seed)
        {
            var state = BattleSetup.Create(BattleSetup.RandomSpec(World.Content, seed), World.Catalog);
            var level = Settings.Level("harness");
            return BattlePlayer.Play(state, World.Catalog, new AiSearch(World.Catalog, level, new BattleRandom(seed + 1)), new AiSearch(World.Catalog, level, new BattleRandom(seed + 2)));
        }
        var first = Play(3);
        Assert.NotEqual(BattleOutcome.Ongoing, first.Outcome);
        Assert.True(first.Turns > 0);
        Assert.Equal(first, Play(3));
    }

    [Fact]
    public void TheSearchDoesNotChangeTheStateItIsGiven()
    {
        var state = BattleSetup.Create(BattleSetup.RandomSpec(World.Content, 9), World.Catalog);
        var unit = BattleDriver.NextTurn(state, World.Catalog)!.Value;
        state.DrainEvents();
        string Snap() => string.Join("|", state.Units.Select(u => $"{u.Hp}:{u.Ward}:{state.Formation.PositionOf(u.Id)}")) + "#" + state.Timeline.Now + "#" + state.Momentum.Value + "#" + state.Random.Clone().NextUInt64();
        var before = Snap();
        new AiSearch(World.Catalog, Settings.Level("normal"), new BattleRandom(1)).Choose(state, unit);
        Assert.Equal(before, Snap());
    }

    [Fact]
    public void TheEvaluationFavoursTheSideWithMoreHpAndMomentum()
    {
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        var level = Settings.Level("hard");
        Assert.Equal(0, Evaluator.Score(s, level));
        s.ShiftMomentum(Side.A, 2);
        Assert.Equal(2 * level.MomentumPoint, Evaluator.Score(s, level));
        s.GetUnit(0).Hp = 20;
        Assert.True(Evaluator.Score(s, level) < 2 * level.MomentumPoint);
    }

    [Fact]
    public void EveryLevelIsDefinedWithTheAdoptedDepths()
    {
        Assert.Equal((2, 3, 35), (Settings.Level("easy").Depth, Settings.Level("easy").PickTop, Settings.Level("easy").PickPercent));
        Assert.Equal((4, 2, 10), (Settings.Level("normal").Depth, Settings.Level("normal").PickTop, Settings.Level("normal").PickPercent));
        Assert.Equal((6, 1, 0), (Settings.Level("hard").Depth, Settings.Level("hard").PickTop, Settings.Level("hard").PickPercent));
    }

    [Fact]
    public void AMomentumPointIsHalfAPercentOfATeamsHp()
    {
        foreach (var name in new[] { "easy", "normal", "harness", "hard" }) Assert.Equal(50, Settings.Level(name).MomentumPoint);
    }

    [Fact]
    public void ARenewedStatusScoresNothingAndAMarkNeedsASkillThatUsesIt()
    {
        var s = new BattleState(1);
        var owner = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        var level = Settings.Level("hard");
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Silenced, Duration: 10, SourceUnitId: owner));
        var context = new EvalContext(World.Catalog, s);
        var before = Evaluator.Score(s, level, context);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Silenced, Duration: 20, SourceUnitId: owner));
        Assert.Equal(before, Evaluator.Score(s, level, context));                          // renewing adds nothing
        Assert.True(Evaluator.Score(s, level) > before);                                   // without the baseline it would

        var s2 = new BattleState(2);
        var tracker = World.Unit(s2, Side.A, Row.Front);
        var target = World.Unit(s2, Side.B, Row.Front);
        var unmarked = Evaluator.Score(s2, level, new EvalContext(World.Catalog, s2));
        Statuses.Apply(s2, new StatusApplication(target, StatusIds.Marked, Duration: 20, SourceUnitId: tracker));
        Assert.Equal(unmarked, Evaluator.Score(s2, level, new EvalContext(World.Catalog, s2)));     // nobody on the side reads a Mark
        s2.GetUnit(tracker).LearnSkill("hunters_shot");
        Assert.True(Evaluator.Score(s2, level, new EvalContext(World.Catalog, s2)) > unmarked);
    }
}
