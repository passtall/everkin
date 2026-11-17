using System.Text;
using System.Text.Json;
using Everkin.Core.Content;
using Everkin.Core.Skills;

const string Usage = """
    Everkin runner

    Usage:
      Everkin.Runner --help
      Everkin.Runner play --seed <n> --level <easy|normal|hard|name>
      Everkin.Runner run [--level <name>] [--battles 10000] [--first-seed 1] [--threads n] [--resume <run id>]
      Everkin.Runner report --run <run id>
      Everkin.Runner teams build --name <batch> [--seed 1] [--per-creature 1] [--note "text"]
      Everkin.Runner run --batch <batch> [--battles 1000] ...

    Commands:
      play             Plays one random AI-versus-AI battle from a seed and prints the result and the time it took.
      run              Plays a run of AI-versus-AI battles in parallel into runs/<run id>/ (battles.jsonl, a snapshot of the content and AI
                       settings, then the CSV files and report.md). --resume continues an interrupted run.
      teams build      Builds a named batch of sample teams (teams/batches/<name>.json): every creature anchors the same number of teams,
                       built by the role rules of TeamBuilder (melee front, ranged rear, one Commander, support with a payoff, Cost 33-35).
      run --batch      Plays the battles between the teams of a batch (each team gets the same number of battles as the focus team).
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
        case "run":
            return RunCommand(args);
        case "report":
            Everkin.Runner.Report.Write(RepoRoot(), Option(args, "--run") ?? throw new ArgumentException("--run <id> is needed"), Console.WriteLine);
            return 0;
        case "play":
            return PlayCommand(args);
        case "teams":
            return TeamsCommand(args);
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

static int RunCommand(string[] args)
{
    var folder = FindContentFolder(args);
    var level = Option(args, "--level") ?? "hard";
    var battles = int.Parse(Option(args, "--battles") ?? "10000");
    var seed = ulong.Parse(Option(args, "--first-seed") ?? "1");
    var threads = int.Parse(Option(args, "--threads") ?? Math.Max(1, Environment.ProcessorCount - 1).ToString());
    var info = Everkin.Runner.BatchRunner.Start(RepoRoot(), folder, level, battles, seed, Option(args, "--resume"), Option(args, "--batch"));
    Everkin.Runner.BatchRunner.Run(RepoRoot(), folder, info, threads, Console.WriteLine);
    Everkin.Runner.Report.Write(RepoRoot(), info.Id, Console.WriteLine);
    return 0;
}

static int TeamsCommand(string[] args)
{
    if (args.Length < 2 || args[1] != "build") throw new ArgumentException("usage: teams build --name <batch>");
    var folder = FindContentFolder(args);
    var content = ContentLoader.LoadFromDirectory(folder);
    var name = Option(args, "--name") ?? throw new ArgumentException("--name <batch> is needed");
    var batch = Everkin.Runner.TeamBatch.Build(content, name, ulong.Parse(Option(args, "--seed") ?? "1"), int.Parse(Option(args, "--per-creature") ?? "1"),
        Option(args, "--note") ?? "", GitCommit(RepoRoot()));
    batch.Save(RepoRoot());
    var costs = batch.Teams.Select(t => t.Cost).ToList();
    Console.WriteLine($"batch {name}: {batch.Teams.Count} teams, Cost {costs.Min()}-{costs.Max()} (mean {costs.Average():F1}), {costs.Count(c => c >= 33)} at 33 or more");
    foreach (var g in batch.Teams.GroupBy(t => t.Template)) Console.WriteLine($"  {g.Key}: {g.Count()}");
    return 0;
}

static string GitCommit(string root)
{
    try
    {
        var p = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("git", "rev-parse HEAD") { WorkingDirectory = root, RedirectStandardOutput = true, UseShellExecute = false });
        var text = p!.StandardOutput.ReadToEnd().Trim();
        p.WaitForExit();
        return text;
    }
    catch { return ""; }
}

static string RepoRoot() => Path.GetDirectoryName(FindContentFolder(Array.Empty<string>()))!;

static int PlayCommand(string[] args)
{
    var folder = FindContentFolder(args);
    var content = ContentLoader.LoadFromDirectory(folder);
    var catalog = SkillCatalog.Create(content);
    var settings = Everkin.Core.Ai.AiSettings.Load(Path.Combine(folder, "ai_settings.json"));
    var seed = ulong.Parse(Option(args, "--seed") ?? "1");
    var level = settings.Level(Option(args, "--level") ?? "easy");
    var spec = Everkin.Core.Setup.BattleSetup.RandomSpec(content, seed);
    var batchName = Option(args, "--batch");
    if (batchName is not null)
    {
        var batch = Everkin.Runner.TeamBatch.Load(Everkin.Runner.TeamBatch.PathOf(RepoRoot(), batchName));
        spec = batch.SpecFor(seed, long.Parse(Option(args, "--index") ?? "0")).Spec;
    }
    var state = Everkin.Core.Setup.BattleSetup.Create(spec, catalog);
    var aiA = new Everkin.Core.Ai.AiSearch(catalog, level, new Everkin.Core.Battle.BattleRandom(seed + 1));
    var aiB = new Everkin.Core.Ai.AiSearch(catalog, level, new Everkin.Core.Battle.BattleRandom(seed + 2));
    var watchCreature = Option(args, "--watch");
    if (watchCreature is not null)
    {
        void Show(Everkin.Core.Battle.BattleState st, int unitId, IReadOnlyList<(Everkin.Core.Ai.AiAction Move, int Score, int Effect)> ranked)
        {
            var u = st.GetUnit(unitId);
            if (u.CreatureId != watchCreature) return;
            Console.WriteLine($"-- t={st.Timeline.Now} {u.CreatureId} (side {u.Side}, HP {u.Hp}) chooses among {ranked.Count} searched moves:");
            foreach (var r in ranked.Take(10)) Console.WriteLine($"     score {r.Score,7}  effect {r.Effect}  {r.Move}");
        }
        aiA.OnChosen = Show;
        aiB.OnChosen = Show;
    }
    var watch = System.Diagnostics.Stopwatch.StartNew();
    Action<Everkin.Core.Battle.BattleEvent>? observe = null;
    if (args.Contains("--trace"))
    {
        string Name(int id) => $"{id}:{state.GetUnit(id).CreatureId ?? state.GetUnit(id).Kind.ToString()}";
        observe = e =>
        {
            switch (e)
            {
                case Everkin.Core.Battle.TurnStarted t: Console.WriteLine($"t={t.Time} turn {Name(t.UnitId)} hp {state.GetUnit(t.UnitId).Hp}"); break;
                case Everkin.Core.Battle.SkillUsed s: Console.WriteLine($"    skill {s.SkillId} -> {(s.TargetUnitId is { } x ? Name(x) : "-")}"); break;
                case Everkin.Core.Battle.UnitMoved m: Console.WriteLine($"    move {m.From} -> {m.To} ({m.Cause})"); break;
                case Everkin.Core.Battle.TurnSkipped k: Console.WriteLine($"    skipped {Name(k.UnitId)}"); break;
                case Everkin.Core.Battle.DamageDealt d when d.Result.HpLost > 0: Console.WriteLine($"    dmg {d.Result.HpLost} on {Name(d.TargetUnitId)}"); break;
                case Everkin.Core.Battle.UnitKnockedOut k: Console.WriteLine($"    KO {Name(k.UnitId)}"); break;
            }
        };
    }
    var result = Everkin.Core.Ai.BattlePlayer.Play(state, catalog, aiA, aiB, observe);
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
