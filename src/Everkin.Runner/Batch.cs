using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;
using Everkin.Core.Skills;

namespace Everkin.Runner;

/// <summary>One unit of a recorded battle with what it did (game_system section 2.5 "Saved per battle").</summary>
public sealed class UnitRecord
{
    public string Creature { get; set; } = "";
    public string Side { get; set; } = "";
    public string Row { get; set; } = "";
    public List<string> Build { get; set; } = new();
    public int Damage { get; set; }
    public int Taken { get; set; }
    public int Healing { get; set; }
    public int Kos { get; set; }
    public bool Fell { get; set; }
    public Dictionary<string, int> Skills { get; set; } = new();
}

/// <summary>One recorded battle: run ID, seed, teams, formations, result, length and per-unit statistics. No event log is stored.</summary>
public sealed class BattleRecord
{
    public string Run { get; set; } = "";
    public ulong Seed { get; set; }
    public string Result { get; set; } = "";
    public string Reason { get; set; } = "";
    public int Turns { get; set; }
    public bool HitCap { get; set; }
    public string? Error { get; set; }
    public double Seconds { get; set; }
    public List<UnitRecord> Units { get; set; } = new();
}

/// <summary>What a run is: stored beside its results so it can be reproduced (game_system section 2.5 "Run size").</summary>
public sealed class RunInfo
{
    public string Id { get; set; } = "";
    public string Level { get; set; } = "";
    public int Battles { get; set; }
    public ulong FirstSeed { get; set; }
    public string Commit { get; set; } = "";
    public string Started { get; set; } = "";
}

/// <summary>Plays AI-versus-AI battles in parallel and records them (milestone M2.2).</summary>
public static class BatchRunner
{
    public static string RunsFolder(string repoRoot) => Path.Combine(repoRoot, "runs");

    public static RunInfo Start(string repoRoot, string contentFolder, string level, int battles, ulong firstSeed, string? resumeId = null)
    {
        var id = resumeId ?? $"{DateTime.Now:yyyyMMdd-HHmmss}-{level}";
        var folder = Path.Combine(RunsFolder(repoRoot), id);
        var infoPath = Path.Combine(folder, "run.json");
        if (resumeId is not null && File.Exists(infoPath))
            return JsonSerializer.Deserialize<RunInfo>(File.ReadAllText(infoPath))!;

        Directory.CreateDirectory(Path.Combine(folder, "snapshot"));
        foreach (var file in Directory.GetFiles(contentFolder, "*.json"))
            File.Copy(file, Path.Combine(folder, "snapshot", Path.GetFileName(file)), overwrite: true);
        var info = new RunInfo { Id = id, Level = level, Battles = battles, FirstSeed = firstSeed, Commit = GitCommit(repoRoot), Started = DateTime.Now.ToString("s") };
        File.WriteAllText(infoPath, JsonSerializer.Serialize(info, new JsonSerializerOptions { WriteIndented = true }));
        return info;
    }

    /// <summary>Plays the battles of a run that are not recorded yet, with a given number of parallel workers.</summary>
    public static void Run(string repoRoot, string contentFolder, RunInfo info, int threads, Action<string> log)
    {
        var folder = Path.Combine(RunsFolder(repoRoot), info.Id);
        var snapshot = Path.Combine(folder, "snapshot");
        var content = ContentLoader.LoadFromDirectory(snapshot);
        var catalog = SkillCatalog.Create(content);
        var trees = new SkillTrees(content);
        var settings = AiSettings.Load(Path.Combine(snapshot, "ai_settings.json"));
        var level = settings.Level(info.Level);

        var resultsPath = Path.Combine(folder, "battles.jsonl");
        var done = new HashSet<ulong>();
        if (File.Exists(resultsPath))
            foreach (var line in File.ReadLines(resultsPath))
                if (line.Length > 2) done.Add(JsonSerializer.Deserialize<BattleRecord>(line)!.Seed);

        var todo = Enumerable.Range(0, info.Battles).Select(i => info.FirstSeed + (ulong)i).Where(s => !done.Contains(s)).ToList();
        log($"run {info.Id}: {todo.Count} battles to play ({done.Count} already recorded), {threads} workers");

        var queue = new ConcurrentQueue<ulong>(todo);
        var writer = new object();
        var finished = 0;
        var watch = Stopwatch.StartNew();
        using var output = new StreamWriter(resultsPath, append: true, new UTF8Encoding(false));
        var workers = Enumerable.Range(0, threads).Select(_ => Task.Run(() =>
        {
            while (queue.TryDequeue(out var seed))
            {
                var record = PlayOne(info.Id, seed, content, catalog, trees, level);
                var json = JsonSerializer.Serialize(record);
                lock (writer)
                {
                    output.WriteLine(json);
                    output.Flush();
                    finished++;
                    if (finished % 25 == 0)
                        log($"  {finished}/{todo.Count} battles, {watch.Elapsed.TotalMinutes:F1} min, about {(todo.Count - finished) * watch.Elapsed.TotalMinutes / finished:F0} min left");
                }
            }
        })).ToArray();
        Task.WaitAll(workers);
        log($"run {info.Id} finished in {watch.Elapsed.TotalMinutes:F1} min");
    }

    /// <summary>Plays one battle from its seed: random teams, builds and formations, both sides with the same AI settings.</summary>
    public static BattleRecord PlayOne(string runId, ulong seed, ContentSet content, SkillCatalog catalog, SkillTrees trees, AiLevel level)
    {
        var record = new BattleRecord { Run = runId, Seed = seed };
        var watch = Stopwatch.StartNew();
        try
        {
            var spec = BattleSetup.RandomSpec(content, trees, seed);
            var state = BattleSetup.Create(spec, catalog);
            var units = state.Units.Select(u => new UnitRecord
            {
                Creature = u.CreatureId!, Side = u.Side.ToString(), Row = state.Formation.PositionOf(u.Id).Row.ToString(),
                Build = (u.Side == Side.A ? spec.SideA : spec.SideB).Units.First(x => x.CreatureId == u.CreatureId).Build.ToList(),
            }).ToList();

            void Observe(BattleEvent e)
            {
                switch (e)
                {
                    case DamageDealt d when d.Result.HpLost > 0:
                        if (Original(state, d.TargetUnitId) is { } t) units[t].Taken += d.Result.HpLost;
                        if (d.SourceUnitId is { } s && Original(state, s) is { } a && state.GetUnit(a).Side != state.GetUnit(d.TargetUnitId).Side) units[a].Damage += d.Result.HpLost;
                        break;
                    case Healed h when h.Amount > 0 && h.SourceUnitId is { } hs && Original(state, hs) is { } healer:
                        units[healer].Healing += h.Amount;
                        break;
                    case UnitKnockedOut k:
                        if (Original(state, k.UnitId) is { } fallen) units[fallen].Fell = true;
                        if (state.GetUnit(k.UnitId).LastHitBy is { } by && Original(state, by) is { } killer && state.GetUnit(killer).Side != state.GetUnit(k.UnitId).Side) units[killer].Kos++;
                        break;
                    case SkillUsed used when Original(state, used.UserId) is { } user:
                        units[user].Skills[used.SkillId] = units[user].Skills.GetValueOrDefault(used.SkillId) + 1;
                        break;
                }
            }

            var aiA = new AiSearch(catalog, level, new BattleRandom(seed * 2 + 1));
            var aiB = new AiSearch(catalog, level, new BattleRandom(seed * 2 + 2));
            var result = BattlePlayer.Play(state, catalog, aiA, aiB, Observe);
            record.Result = result.Outcome.ToString();
            record.Reason = result.Reason?.ToString() ?? "";
            record.Turns = result.Turns;
            record.HitCap = result.HitCap;
            record.Units = units;
        }
        catch (Exception e)
        {
            record.Result = "Error";
            record.Error = $"{e.GetType().Name}: {e.Message}\n{e.StackTrace}";
        }
        record.Seconds = Math.Round(watch.Elapsed.TotalSeconds, 2);
        return record;
    }

    /// <summary>The index of the original party member a unit's actions count for: itself, or its summoner for a summon (indices equal unit IDs for the 12 originals).</summary>
    private static int? Original(BattleState state, int unitId)
    {
        var u = state.GetUnit(unitId);
        if (u.Kind == UnitKind.Creature && !u.IsRemoved && unitId < 12) return unitId;
        return u.SummonerId is { } s && s < 12 ? s : null;
    }

    private static string GitCommit(string root)
    {
        try
        {
            var p = Process.Start(new ProcessStartInfo("git", "rev-parse HEAD") { WorkingDirectory = root, RedirectStandardOutput = true, UseShellExecute = false });
            var text = p!.StandardOutput.ReadToEnd().Trim();
            p.WaitForExit();
            return text;
        }
        catch { return ""; }
    }
}
