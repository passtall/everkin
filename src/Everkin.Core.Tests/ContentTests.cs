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
        Assert.Equal(9, set.AttackTypes.Count);
        Assert.Equal(12, set.Professions.Count);
        Assert.Equal(9, set.AttackSkills.Count);
        Assert.Equal(179, set.ProfessionSkills.Count);
        Assert.Equal(319, set.Creatures.Count);
        Assert.Equal("brawl", set.CreaturesById["abacus-strategist"].AttackType);
        Assert.Null(set.SkillsById["stalk"].Parent);
        Assert.Equal("stalk", set.SkillsById["creep_closer"].Parent);
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
            ContentLoader.LoadFromJson(Read(ContentLoader.ClassesFile), Read(ContentLoader.SkillsFile), creatures));
        Assert.Contains("creatures.json", e.Message);
        Assert.Contains("abacus-strategist", e.Message);
    }

    [Fact]
    public void UnknownProfessionStopsWithFileAndId()
    {
        var creatures = Read(ContentLoader.CreaturesFile)
            .Replace("\"leader\"", "\"jester\"");
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson(Read(ContentLoader.ClassesFile), Read(ContentLoader.SkillsFile), creatures));
        Assert.Contains("creatures.json", e.Message);
        Assert.Contains("jester", e.Message);
    }

    [Fact]
    public void MissingParentStopsWithFileAndId()
    {
        var skills = Read(ContentLoader.SkillsFile).Replace("\"parent\": \"stalk\"", "\"parent\": \"no_such_skill\"");
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson(Read(ContentLoader.ClassesFile), skills, Read(ContentLoader.CreaturesFile)));
        Assert.Contains("skills.json", e.Message);
        Assert.Contains("no_such_skill", e.Message);
    }

    [Fact]
    public void MalformedJsonNamesTheFile()
    {
        var e = Assert.Throws<ContentException>(() =>
            ContentLoader.LoadFromJson("{", Read(ContentLoader.SkillsFile), Read(ContentLoader.CreaturesFile)));
        Assert.Contains("classes.json", e.Message);
    }
}
