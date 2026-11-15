using System.Diagnostics;
using Everkin.Core.Content;

namespace Everkin.Core.Tests;

public class ContentTests
{
    private static readonly string Root = FindRoot();
    private static readonly string ContentDir = Path.Combine(Root, "content");

    private static string FindRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Everkin.sln")))
            dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("Everkin.sln not found above the test binaries");
    }

    private static string Read(string file) => File.ReadAllText(Path.Combine(ContentDir, file));

    [Fact]
    public void RealContentLoadsAndHasTheDocumentedSizes()
    {
        var set = ContentLoader.LoadFromDirectory(ContentDir);
        Assert.Equal(28, set.Classes.Count);
        Assert.Equal(168, set.Skills.Count);
        Assert.Equal(84, set.Skills.Count(x => x.Kind == "A"));
        Assert.Equal(84, set.Skills.Count(x => x.Kind == "K"));
        Assert.Equal(319, set.Attacks.Count);
        Assert.Equal(319, set.Creatures.Count);
        Assert.Equal(new[] { "tactician", "commander" }, set.CreaturesById["abacus-strategist"].Classes);
        Assert.Equal("bead_count", set.CreaturesById["abacus-strategist"].Attack);
        Assert.Equal(new[] { "all_in", "drill" }, set.CreaturesById["abacus-strategist"].Keystones);
        Assert.Equal(14, set.Classes.Count(c => c.Magical));
        var bead = set.AttacksById["bead_count"];
        Assert.Equal(("melee", "single", 3, 4), (bead.Delivery, bead.Shape, bead.Min, bead.Max));
        Assert.Equal(new AttackExtraDef("delay_turn", 100, Amount: 2), bead.Extra);
        Assert.Equal("pulse", set.AttacksById["sour_chord"].Shape);
        Assert.True(set.AttacksById["sour_chord"].Magical);
    }

    [Fact]
    public void EveryTeamBudgetFieldIsInRange()
    {
        var set = ContentLoader.LoadFromDirectory(ContentDir);
        Assert.All(set.Creatures, c => Assert.InRange(c.Cost, 1, 10));
    }

    [Fact]
    public void JsonMatchesTheDocs()
    {
        // tools/content/build_content.py --check regenerates the content from docs/ and compares it with content/.
        var psi = new ProcessStartInfo("python", "tools/content/build_content.py --check")
        {
            WorkingDirectory = Root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
        };
        using var p = Process.Start(psi) ?? throw new InvalidOperationException("cannot start python");
        var output = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
        p.WaitForExit();
        Assert.True(p.ExitCode == 0, output);
    }

    [Fact]
    public void DuplicateCreatureIdStopsWithFileAndId()
    {
        var creatures = Read(ContentLoader.CreaturesFile).Replace("\"id\": \"accordion-warlord\"", "\"id\": \"abacus-strategist\"");
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson(Read(ContentLoader.ClassesFile), Read(ContentLoader.SkillsFile), creatures, Read(ContentLoader.StatusesFile)));
        Assert.Contains("creatures.json", e.Message);
        Assert.Contains("abacus-strategist", e.Message);
    }

    [Fact]
    public void UnknownClassStopsWithFileAndId()
    {
        var creatures = Read(ContentLoader.CreaturesFile)
            .Replace("\"tactician\"", "\"jester\"");
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson(Read(ContentLoader.ClassesFile), Read(ContentLoader.SkillsFile), creatures, Read(ContentLoader.StatusesFile)));
        Assert.Contains("creatures.json", e.Message);
        Assert.Contains("jester", e.Message);
    }

    [Fact]
    public void AKeystoneOfAnotherClassStopsWithFileAndId()
    {
        var creatures = Read(ContentLoader.CreaturesFile).Replace("\"all_in\"", "\"drill\"");
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson(Read(ContentLoader.ClassesFile), Read(ContentLoader.SkillsFile), creatures, Read(ContentLoader.StatusesFile)));
        Assert.Contains("creatures.json", e.Message);
        Assert.Contains("drill", e.Message);
    }

    [Fact]
    public void MalformedJsonNamesTheFile()
    {
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson("{", Read(ContentLoader.SkillsFile), Read(ContentLoader.CreaturesFile), Read(ContentLoader.StatusesFile)));
        Assert.Contains("classes.json", e.Message);
    }

    [Fact]
    public void EveryNamedStatusInTheDocsHasAnIdTheRulesCodeKnows()
    {
        var known = typeof(Everkin.Core.Battle.StatusIds)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToHashSet();
        var set = ContentLoader.LoadFromDirectory(ContentDir);
        Assert.All(set.Statuses, st => Assert.Contains(st.Id, known));
        Assert.Equal("As stated (Flash Powder: 20)", set.Statuses.Single(st => st.Id == "blind").Duration);
    }
}
