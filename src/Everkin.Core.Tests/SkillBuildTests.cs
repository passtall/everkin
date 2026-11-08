using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class SkillBuildTests
{
    private static readonly string ContentDir = Path.Combine(FindRoot(), "content");

    private static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Everkin.sln"))) dir = dir.Parent;
        return dir!.FullName;
    }

    private static readonly ContentSet Content = ContentLoader.LoadFromDirectory(ContentDir);
    private static readonly SkillTrees Trees = new(Content);

    private static SkillBuild Hunter() => new(Trees, new[] { "hunter" });

    // ---- the tree model (4.1) ----

    [Fact]
    public void EveryBranchIsARealTreeWithRootsAndOneParentPerNode()
    {
        foreach (var branch in Content.ProfessionSkills.GroupBy(s => (s.Profession, s.Branch)))
        {
            Assert.Contains(branch, s => s.Parent is null);                        // at least one root per branch
            Assert.All(branch, s => Assert.True(s.Parent is null || Trees.Node(s.Parent).Branch == s.Branch));
        }
    }

    [Fact]
    public void ABuildHasEightPointsAndEveryNodeCostsOne()
    {
        Assert.Equal(8, SkillBuild.Points);
        var b = Hunter();
        Assert.Equal(8, b.PointsLeft);
        b.Learn("stalk");
        Assert.Equal(1, b.PointsSpent);
        Assert.Equal(7, b.PointsLeft);
    }

    [Fact]
    public void OnlyRootsCanBeLearnedFirstAndOtherNodesNeedTheirParent()
    {
        var b = Hunter();
        Assert.True(b.CanLearn("stalk"));          // root of Stalker
        Assert.True(b.CanLearn("target"));         // root of Tracker
        Assert.False(b.CanLearn("creep_closer"));  // needs Stalk
        Assert.Equal("needs stalk first", b.WhyNot("creep_closer"));
        Assert.Throws<InvalidOperationException>(() => b.Learn("creep_closer"));
        b.Learn("stalk");
        Assert.True(b.CanLearn("creep_closer"));
        Assert.True(b.CanLearn("freeze_in_place"));   // the tree splits: Stalk has two children
        Assert.False(b.CanLearn("lie_in_wait"));      // needs Creep Closer
    }

    [Fact]
    public void ANodeHasExactlyOneParentAndBranchesDoNotLockEachOther()
    {
        var b = Hunter();
        b.Learn("stalk");
        b.Learn("target");                              // another branch: no lock
        b.Learn("net_trap");
        Assert.Equal(new[] { "stalk", "target", "net_trap" }, b.Learned);
    }

    [Fact]
    public void AModifierNeedsItsSkill()
    {
        var b = new SkillBuild(Trees, new[] { "guardian" });
        Assert.False(b.CanLearn("lasting_ward"));
        b.Learn("ward");
        Assert.True(b.CanLearn("lasting_ward"));
    }

    [Fact]
    public void AKeystoneNeedsOnlyItsParentAndTheBudget()
    {
        // 4.1 replaced the old rule of 3 other nodes: a keystone needs its parent only.
        var b = new SkillBuild(Trees, new[] { "guardian" });
        foreach (var id in new[] { "leap_to_defend", "full_cover" }) b.Learn(id);
        Assert.Equal("K", Trees.Node("living_wall").Kind);
        Assert.True(b.CanLearn("living_wall"));
    }

    [Fact]
    public void ThePointBudgetStopsAtEight()
    {
        var b = Hunter();
        foreach (var id in new[] { "stalk", "target", "net_trap", "cut_off_retreat", "read_tracks", "scent_of_blood", "creep_closer", "freeze_in_place" })
            b.Learn(id);
        Assert.Equal(0, b.PointsLeft);
        Assert.Equal("no skill points left", b.WhyNot("decoy"));
        Assert.Empty(b.Learnable());
    }

    [Fact]
    public void OnlyTheUnitsOwnProfessionsCanBeLearned()
    {
        var b = Hunter();
        Assert.StartsWith("not a skill of this unit's professions", b.WhyNot("ward")!);
        var two = new SkillBuild(Trees, new[] { "hunter", "guardian" });
        Assert.True(two.CanLearn("ward"));
        Assert.True(two.CanLearn("stalk"));
    }

    [Fact]
    public void ALearnedNodeCannotBeLearnedAgainAndUnknownSkillsAreRefused()
    {
        var b = Hunter();
        b.Learn("stalk");
        Assert.Equal("already learned", b.WhyNot("stalk"));
        Assert.Equal("unknown skill", b.WhyNot("no_such_skill"));
    }

    [Fact]
    public void UnlearningANodeAlsoUnlearnsEveryNodeBelowItAndGivesThePointsBack()
    {
        var b = Hunter();
        foreach (var id in new[] { "stalk", "creep_closer", "lie_in_wait", "freeze_in_place", "vanish", "target" }) b.Learn(id);
        Assert.Equal(2, b.PointsLeft);
        var removed = b.Unlearn("creep_closer");
        Assert.Equal(new[] { "creep_closer", "lie_in_wait" }, removed);   // Lie in Wait hangs below Creep Closer
        Assert.Equal(new[] { "stalk", "freeze_in_place", "vanish", "target" }, b.Learned);
        Assert.Equal(4, b.PointsLeft);
        Assert.Empty(b.Unlearn("creep_closer"));                           // nothing to do the second time
    }

    [Fact]
    public void UnlearningARootRemovesItsWholeSubtree()
    {
        var b = Hunter();
        foreach (var id in new[] { "stalk", "creep_closer", "freeze_in_place", "vanish", "target" }) b.Learn(id);
        b.Unlearn("stalk");
        Assert.Equal(new[] { "target" }, b.Learned);
    }

    [Fact]
    public void RespecIsFreeAndLearnableListsWhatIsOpenNow()
    {
        var b = Hunter();
        Assert.Equal(new[] { "stalk", "target", "net_trap", "cut_off_retreat" }, b.Learnable());   // the four roots
        b.Learn("stalk");
        Assert.Contains("creep_closer", b.Learnable());
        b.Reset();
        Assert.Empty(b.Learned);
        Assert.Equal(8, b.PointsLeft);
    }

    [Fact]
    public void ABuildCanBeBuiltFromAListInAnyOrderAndRefusesIllegalOnes()
    {
        Assert.Null(SkillBuild.Validate(Trees, new[] { "hunter" }, new[] { "creep_closer", "stalk" }));   // child listed first
        Assert.NotNull(SkillBuild.Validate(Trees, new[] { "hunter" }, new[] { "creep_closer" }));          // parent missing
        Assert.NotNull(SkillBuild.Validate(Trees, new[] { "hunter" }, new[] { "stalk", "stalk" }));
        Assert.NotNull(SkillBuild.Validate(Trees, new[] { "hunter" }, new[] { "ward" }));
        Assert.NotNull(SkillBuild.Validate(Trees, new[] { "hunter" }, Enumerable.Repeat("stalk", 9).ToArray()));
        Assert.Throws<ArgumentException>(() => new SkillBuild(Trees, new[] { "hunter" }, new[] { "creep_closer" }));
    }

    [Fact]
    public void CloneIsIndependent()
    {
        var b = Hunter();
        b.Learn("stalk");
        var copy = b.Clone();
        copy.Learn("target");
        Assert.Single(b.Learned);
        Assert.Equal(2, copy.Learned.Count);
    }

    [Fact]
    public void TheAttackTypeAttackIsFreeAndNotPartOfTheBuild()
    {
        Assert.Equal("brawl_strike", Trees.AttackSkillId("brawl"));
        Assert.Equal(8, Hunter().PointsLeft);
    }

    [Fact]
    public void DescendantsAreNearestFirst()
    {
        Assert.Equal(new[] { "creep_closer", "freeze_in_place", "lie_in_wait", "vanish" }, Trees.Descendants("stalk"));
    }

    // ---- random legal builds ----

    [Fact]
    public void ARandomBuildIsLegalAndUsesAllEightPoints()
    {
        var b = SkillBuilds.Random(Trees, new[] { "hunter", "rogue" }, new BattleRandom(5));
        Assert.Equal(8, b.Learned.Count);
        Assert.Null(SkillBuild.Validate(Trees, new[] { "hunter", "rogue" }, b.Learned));
    }

    [Fact]
    public void RandomBuildsAreReproducibleFromTheSeedAndDifferBetweenSeeds()
    {
        var p = new[] { "elementalist" };
        Assert.Equal(SkillBuilds.Random(Trees, p, new BattleRandom(3)).Learned, SkillBuilds.Random(Trees, p, new BattleRandom(3)).Learned);
        var distinct = Enumerable.Range(1, 30).Select(i => string.Join(",", SkillBuilds.Random(Trees, p, new BattleRandom((ulong)i)).Learned)).Distinct().Count();
        Assert.True(distinct > 20);
    }

    [Fact]
    public void RandomBuildsReachEveryNodeOfATreeOverManySeeds()
    {
        var seen = new HashSet<string>();
        for (ulong seed = 1; seed <= 400; seed++)
            foreach (var id in SkillBuilds.Random(Trees, new[] { "flying" }, new BattleRandom(seed)).Learned) seen.Add(id);
        Assert.Equal(Content.ProfessionSkills.Count(s => s.Profession == "flying"), seen.Count);
    }

    [Fact]
    public void EveryCreatureCanGetAFullRandomBuild()
    {
        foreach (var c in Content.Creatures)
        {
            var b = SkillBuilds.Random(Trees, c.Professions, new BattleRandom(1));
            Assert.Equal(8, b.Learned.Count);
        }
    }

    // ---- default builds ----

    [Fact]
    public void EveryCreatureHasALegalDefaultBuildInTheContent()
    {
        Assert.Equal(Content.Creatures.Count, Content.DefaultBuilds.Count);
        foreach (var c in Content.Creatures)
        {
            var build = Content.DefaultBuilds[c.Id];
            Assert.Equal(8, build.Count);
            Assert.Null(SkillBuild.Validate(Trees, c.Professions, build));
        }
    }

    [Fact]
    public void TheStoredDefaultBuildsAreTheOnesTheMasterSeedMakes()
    {
        foreach (var c in Content.Creatures.Take(40))
            Assert.Equal(DefaultBuilds.For(Trees, c), Content.DefaultBuilds[c.Id]);
    }

    [Fact]
    public void ACreaturesDefaultBuildDoesNotDependOnTheRestOfTheRoster()
    {
        var one = Content.Creatures[10];
        Assert.Equal(DefaultBuilds.For(Trees, one), DefaultBuilds.For(Trees, one));
        Assert.NotEqual(DefaultBuilds.For(Trees, one, 1), DefaultBuilds.For(Trees, one, 2));
    }

    [Fact]
    public void CompletingDefaultBuildsKeepsWhatIsStoredAndOnlyAddsTheMissing()
    {
        var stored = new Dictionary<string, IReadOnlyList<string>> { [Content.Creatures[0].Id] = new[] { "stalk" } };
        var all = DefaultBuilds.Complete(Trees, stored);
        Assert.Equal(Content.Creatures.Count, all.Count);
        Assert.Equal(new[] { "stalk" }, all[Content.Creatures[0].Id]);                                // kept as it was
        Assert.Equal(Content.DefaultBuilds[Content.Creatures[1].Id], all[Content.Creatures[1].Id]);   // the rest as the generator makes them
    }

    // ---- the loader checks the stored builds ----

    private static string Read(string file) => File.ReadAllText(Path.Combine(ContentDir, file));

    private static ContentException Load(string builds) => Assert.Throws<ContentException>(() => ContentLoader.LoadFromJson(
        Read(ContentLoader.ClassesFile), Read(ContentLoader.SkillsFile), Read(ContentLoader.CreaturesFile),
        Read(ContentLoader.StatusesFile), builds));

    [Fact]
    public void AnIllegalDefaultBuildStopsTheLoadWithFileAndCreature()
    {
        var builds = Read(ContentLoader.DefaultBuildsFile).Replace("\"reposition\"", "\"no_such_skill\"");
        var e = Load(builds);
        Assert.Contains("default_builds.json", e.Message);
        Assert.Contains("abacus-strategist", e.Message);
        Assert.Contains("no_such_skill", e.Message);
    }

    [Fact]
    public void ADefaultBuildThatBreaksTheParentRuleIsRefused()
    {
        var e = Load("{\"builds\": {\"abacus-strategist\": [\"inspiring_presence\"]}}");
        Assert.Contains("abacus-strategist", e.Message);
    }

    [Fact]
    public void AMissingDefaultBuildStopsTheLoad()
    {
        var e = Load("{\"builds\": {}}");
        Assert.Contains("default_builds.json", e.Message);
        Assert.Contains("no default build", e.Message);
    }

    [Fact]
    public void ABuildForAnUnknownCreatureStopsTheLoad()
    {
        var e = Load("{\"builds\": {\"no-such-creature\": []}}");
        Assert.Contains("no-such-creature", e.Message);
    }

    [Fact]
    public void LoadingAFolderWithoutDefaultBuildsStopsUnlessTheToolAsksToSkipThem()
    {
        var dir = Path.Combine(Path.GetTempPath(), "everkin-content-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        try
        {
            foreach (var f in new[] { ContentLoader.ClassesFile, ContentLoader.SkillsFile, ContentLoader.CreaturesFile, ContentLoader.StatusesFile })
                File.Copy(Path.Combine(ContentDir, f), Path.Combine(dir, f));
            var e = Assert.Throws<ContentException>(() => ContentLoader.LoadFromDirectory(dir));
            Assert.Contains("default_builds.json", e.Message);
            Assert.Empty(ContentLoader.LoadFromDirectory(dir, requireDefaultBuilds: false).DefaultBuilds);
        }
        finally { Directory.Delete(dir, recursive: true); }
    }
}
