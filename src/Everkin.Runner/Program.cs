using System.Text;
using System.Text.Json;
using Everkin.Core.Content;
using Everkin.Core.Skills;

const string Usage = """
    Everkin runner

    Usage:
      Everkin.Runner --help
      Everkin.Runner default-builds [--content <folder>]
      Everkin.Runner play --seed <n> --level <easy|normal|hard|name>
      Everkin.Runner run [--level <name>] [--battles 10000] [--first-seed 1] [--threads n] [--resume <run id>]
      Everkin.Runner report --run <run id>

    Commands:
      default-builds   Make content/default_builds.json: one random legal skill build per creature, from a fixed master seed.
                       Builds that are already stored are kept; only missing creatures get a new one.
      play             Plays one random AI-versus-AI battle from a seed and prints the result and the time it took.
      run              Plays a run of AI-versus-AI battles in parallel into runs/<run id>/ (battles.jsonl, a snapshot of the content and AI
                       settings, then the CSV files and report.md). --resume continues an interrupted run.
      report           Writes the CSV files and report.md again from the recorded battles of a run.

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
        case "run":
            return RunCommand(args);
        case "report":
            Everkin.Runner.Report.Write(RepoRoot(), Option(args, "--run") ?? throw new ArgumentException("--run <id> is needed"), Console.WriteLine);
            return 0;
        case "play":
            return PlayCommand(args);
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

static int RunCommand(string[] args)
{
    var folder = FindContentFolder(args);
    var level = Option(args, "--level") ?? "hard";
    var battles = int.Parse(Option(args, "--battles") ?? "10000");
    var seed = ulong.Parse(Option(args, "--first-seed") ?? "1");
    var threads = int.Parse(Option(args, "--threads") ?? Math.Max(1, Environment.ProcessorCount - 1).ToString());
    var info = Everkin.Runner.BatchRunner.Start(RepoRoot(), folder, level, battles, seed, Option(args, "--resume"));
    Everkin.Runner.BatchRunner.Run(RepoRoot(), folder, info, threads, Console.WriteLine);
    Everkin.Runner.Report.Write(RepoRoot(), info.Id, Console.WriteLine);
    return 0;
}

static string RepoRoot() => Path.GetDirectoryName(FindContentFolder(Array.Empty<string>()))!;

static int PlayCommand(string[] args)
{
    var folder = FindContentFolder(args);
    var content = ContentLoader.LoadFromDirectory(folder);
    var catalog = SkillCatalog.Create(content);
    var trees = new SkillTrees(content);
    var settings = Everkin.Core.Ai.AiSettings.Load(Path.Combine(folder, "ai_settings.json"));
    var seed = ulong.Parse(Option(args, "--seed") ?? "1");
    var level = settings.Level(Option(args, "--level") ?? "easy");
    var spec = Everkin.Core.Setup.BattleSetup.RandomSpec(content, trees, seed);
    var state = Everkin.Core.Setup.BattleSetup.Create(spec, catalog);
    var aiA = new Everkin.Core.Ai.AiSearch(catalog, level, new Everkin.Core.Battle.BattleRandom(seed + 1));
    var aiB = new Everkin.Core.Ai.AiSearch(catalog, level, new Everkin.Core.Battle.BattleRandom(seed + 2));
    var watch = System.Diagnostics.Stopwatch.StartNew();
    var result = Everkin.Core.Ai.BattlePlayer.Play(state, catalog, aiA, aiB);
    watch.Stop();
    Console.WriteLine($"seed {seed}: {result.Outcome} ({result.Reason}) after {result.Turns} turns, {watch.Elapsed.TotalSeconds:F1} s, nodes {aiA.TotalNodes + aiB.TotalNodes}, choices {aiA.Choices + aiB.Choices}");
    return 0;
}

static string? Option(string[] args, string name)
{
    var i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

static string FindContentFolder(string[] args)
{
    var i = Array.IndexOf(args, "--content");
    if (i >= 0 && i + 1 < args.Length) return Path.GetFullPath(args[i + 1]);
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Everkin.sln"))) dir = dir.Parent;
    return Path.Combine(dir?.FullName ?? Directory.GetCurrentDirectory(), "content");
}
