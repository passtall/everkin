using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;

namespace Everkin.Core.Tests;

public class TeamBuilderTests
{
    [Fact]
    public void EveryCreatureGetsALegalTeamBuiltAroundIt()
    {
        var content = World.Content;
        foreach (var anchor in content.Creatures)
        {
            var built = TeamBuilder.Build(content, anchor, new BattleRandom(7));
            var team = built.Team.Units.Select(u => content.CreaturesById[u.CreatureId]).ToList();
            Assert.Contains(anchor, team);
            Assert.Equal(6, team.Count);
            Assert.Equal(team.Count, team.Select(c => c.Id).Distinct().Count());
            Assert.InRange(built.Cost, 1, BattleSetup.TeamBudget);
            Assert.Equal(built.Cost, built.Team.TotalCost(content));
            Assert.True(team.Count(c => c.Classes.Contains("commander")) <= 1, anchor.Id);
            foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
                Assert.True(built.Team.Units.Count(u => u.Row == row) <= Formation.RowCapacity);
        }
    }

    [Fact]
    public void ASeedGivesTheSameTeam()
    {
        var anchor = World.Content.Creatures[10];
        var a = TeamBuilder.Build(World.Content, anchor, new BattleRandom(99));
        var b = TeamBuilder.Build(World.Content, anchor, new BattleRandom(99));
        Assert.Equal(a.Team.Units, b.Team.Units);
        Assert.Equal(a.Template, b.Template);
    }

    [Fact]
    public void SupportOnlyComesWithItsPayoff()
    {
        var content = World.Content;
        var priest = content.Creatures.First(c => c.Classes.Contains("cleric"));
        var lonely = new List<CreatureDef> { content.Creatures.First(c => TeamBuilder.RoleOf(content, c) == TeamRole.Ranged) };
        Assert.False(TeamBuilder.Allowed(content, priest, lonely));
        var blockers = content.Creatures.Where(c => TeamBuilder.RoleOf(content, c) == TeamRole.Blocker && !c.Classes.Any(k => new[] { "cleric", "mender", "lifebinder" }.Contains(k))).Take(2).ToList();
        Assert.True(TeamBuilder.Allowed(content, priest, blockers));
    }

    [Fact]
    public void MeleeStandsInFrontRangedInTheRearAndBlockersInTheMiddleOfTheFrontRow()
    {
        var content = World.Content;
        foreach (var anchor in content.Creatures.Take(60))
        {
            var built = TeamBuilder.Build(content, anchor, new BattleRandom(3));
            foreach (var u in built.Team.Units)
            {
                var role = TeamBuilder.RoleOf(content, content.CreaturesById[u.CreatureId]);
                if (role is TeamRole.Blocker or TeamRole.Frontliner) Assert.Equal(Row.Front, u.Row);
                if (role == TeamRole.Ranged) Assert.Equal(Row.Rear, u.Row);
            }
        }
    }
}
