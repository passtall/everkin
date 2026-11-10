using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;

namespace Everkin.Core.Tests;

public class CampaignTests
{
    private static readonly CampaignDef Campaign = CampaignDef.FromJson(File.ReadAllText(Path.Combine(World.ContentDir, "campaign.json")), World.Content);

    [Fact]
    public void TheCampaignHasTheTutorialFourChaptersAndTheChallengeEncounters()
    {
        Assert.Equal(new[] { "T", "1", "2", "3", "4", "C" }, Campaign.Chapters.Select(c => c.Id));
        Assert.Equal(new[] { 3, 6, 6, 6, 4, 4 }, Campaign.Chapters.Select(c => c.Encounters.Count));
        Assert.Equal(new[] { "T1", "T2", "T3" }, Campaign.Tutorial.Encounters.Select(e => e.Id));
    }

    [Fact]
    public void EveryRewardIsAMemberOfItsEnemyTeamAndNoRewardIsGivenTwice()
    {
        var rewards = Campaign.AllEncounters.SelectMany(e => e.Rewards).ToList();
        Assert.Equal(rewards.Count, rewards.Distinct().Count());
        Assert.Equal(30, rewards.Count);                                                  // 4 tutorial + 22 chapters + 4 challenges
        foreach (var e in Campaign.AllEncounters)
            Assert.All(e.Rewards, r => Assert.Contains(e.Team, u => u.Creature == r));
    }

    [Fact]
    public void TeamCostsMatchTheTableAndEveryRowHoldsAtMostSixUnits()
    {
        foreach (var e in Campaign.AllEncounters)
        {
            Assert.Equal(e.Cost, e.Team.Sum(u => World.Content.CreaturesById[u.Creature].Cost));
            Assert.All(e.Team.GroupBy(u => u.Row), g => Assert.True(g.Count() <= Formation.RowCapacity));
        }
    }

    [Fact]
    public void ThereIsOneBossAtTheEndOfEachChapterAndHisStatsAreStrengthened()
    {
        foreach (var ch in Campaign.Story)
        {
            Assert.EndsWith("B", ch.Encounters[^1].Id);
            Assert.Single(ch.Encounters.Where(e => e.IsBoss));
        }
        Assert.Equal(75, BattleSetup.BossHp(50));                                         // Ranger Wolf
        Assert.Equal(110, BattleSetup.BossHp(75));                                        // the other three bosses
        var boss = Campaign.Encounter("1-B");
        var state = BattleSetup.Create(new BattleSpec(1, BattleSetup.RandomTeam(World.Content, new Skills.SkillTrees(World.Content), new BattleRandom(1)), boss.EnemyTeam(World.Content)), World.Catalog);
        var wolf = state.Units.First(u => u.Side == Side.B && u.MaxHp > 50);
        Assert.Equal((75, 7), (wolf.MaxHp, wolf.Power));
    }

    [Fact]
    public void TheStartingCreaturesExist()
    {
        var starting = CampaignDef.StartingCreatures(World.Content);
        Assert.Equal(2, starting.Count);
        Assert.All(starting, id => Assert.InRange(World.Content.CreaturesById[id].Cost, 2, 3));
    }
}
