using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

/// <summary>Shared test setup: the real content, the skill catalog and small helpers to put units on a board.</summary>
internal static class World
{
    public static readonly string ContentDir = Path.Combine(FindRoot(), "content");
    public static readonly ContentSet Content = ContentLoader.LoadFromDirectory(ContentDir);
    public static readonly SkillCatalog Catalog = SkillCatalog.Create(Content);

    private static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Everkin.sln"))) dir = dir.Parent;
        return dir!.FullName;
    }

    /// <summary>A bare creature definition for tests that need a roster creature but none of its content.</summary>
    public static CreatureDef Def(string id = "t", int hp = 40, int speed = 5, int power = 5, int cost = 1) =>
        new(id, id, new[] { "berserker", "champion" }, new[] { "reckless_swing", "hunt_down" }, "bead_count", "t.png", "ruins",
            hp, speed, power, cost, new[] { 0, 0, 1, 1 }, new[] { 0, 0, 1, 1 });

    /// <summary>A unit of 40 HP, Speed 5, Power 5 that knows the given skills, placed on the board.</summary>
    public static int Unit(BattleState s, Side side, Row row, params string[] skills)
    {
        var u = s.AddUnit("u", side, 40, 5, 5);
        s.Formation.Place(u.Id, side, row);
        foreach (var id in skills) u.LearnSkill(id);
        return u.Id;
    }
}
