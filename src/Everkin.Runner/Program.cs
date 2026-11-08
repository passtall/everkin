using System.Text;
using System.Text.Json;
using Everkin.Core.Content;
using Everkin.Core.Skills;

const string Usage = """
    Everkin runner

    Usage:
      Everkin.Runner --help
      Everkin.Runner default-builds [--content <folder>]

    Commands:
      default-builds   Make content/default_builds.json: one random legal skill build per creature, from a fixed master seed.
                       Builds that are already stored are kept; only missing creatures get a new one.
                       The batch commands (AI-versus-AI runs) arrive with milestone M2.2.

    Options:
      --content <folder>   The content folder (default: the content folder of the repository).
      --help               Show this text.
    """;

if (args.Length == 0 || args[0] is "--help" or "-h" or "help")
{
    Console.WriteLine(Usage);
    return 0;
}

try
{
    switch (args[0])
    {
        case "default-builds":
            return DefaultBuildsCommand(args);
        default:
            Console.Error.WriteLine($"Unknown command '{args[0]}'.");
            Console.Error.WriteLine(Usage);
            return 2;
    }
}
catch (ContentException e)
{
    Console.Error.WriteLine($"CONTENT ERROR: {e.Message}");
    return 1;
}

static int DefaultBuildsCommand(string[] args)
{
    var folder = FindContentFolder(args);
    var content = ContentLoader.LoadFromDirectory(folder, requireDefaultBuilds: false);
    var trees = new SkillTrees(content);

    var path = Path.Combine(folder, ContentLoader.DefaultBuildsFile);
    Dictionary<string, IReadOnlyList<string>>? existing = null;
    if (File.Exists(path))
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(path));
        existing = doc.RootElement.GetProperty("builds").EnumerateObject()
            .ToDictionary(p => p.Name, p => (IReadOnlyList<string>)p.Value.EnumerateArray().Select(e => e.GetString()!).ToArray());
    }

    var builds = DefaultBuilds.Complete(trees, existing);
    var added = builds.Count - (existing?.Keys.Count(builds.ContainsKey) ?? 0);

    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(new { builds }, options).ReplaceLineEndings("\n") + "\n";
    File.WriteAllText(path, json, new UTF8Encoding(false));
    Console.WriteLine($"wrote {path}: {builds.Count} builds ({added} new)");
    return 0;
}

static string FindContentFolder(string[] args)
{
    var i = Array.IndexOf(args, "--content");
    if (i >= 0 && i + 1 < args.Length) return Path.GetFullPath(args[i + 1]);
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Everkin.sln"))) dir = dir.Parent;
    return Path.Combine(dir?.FullName ?? Directory.GetCurrentDirectory(), "content");
}
