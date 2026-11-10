using System.Globalization;
using System.Text;
using System.Text.Json;
using Everkin.Core.Content;

namespace Everkin.Runner;

/// <summary>Turns the recorded battles of a run into CSV files and a Markdown report with imbalance flags (game_system section 2.5 "Report").</summary>
public static class Report
{
    private sealed class Tally
    {
        public int Appearances, Wins, Draws;
        public long Damage, Taken, Healing, Kos, Uses;
        public double Rate => Appearances == 0 ? 0 : 100.0 * Wins / Appearances;
    }

    public static void Write(string repoRoot, string runId, Action<string> log)
    {
        var folder = Path.Combine(BatchRunner.RunsFolder(repoRoot), runId);
        var content = ContentLoader.LoadFromDirectory(Path.Combine(folder, "snapshot"));
        var info = JsonSerializer.Deserialize<RunInfo>(File.ReadAllText(Path.Combine(folder, "run.json")))!;
        var records = File.ReadLines(Path.Combine(folder, "battles.jsonl")).Where(l => l.Length > 2)
            .Select(l => JsonSerializer.Deserialize<BattleRecord>(l)!).ToList();
        var ok = records.Where(r => r.Error is null).ToList();
        var errors = records.Where(r => r.Error is not null).ToList();

        var creatures = new Dictionary<string, Tally>();
        var byRow = new Dictionary<(string, string), Tally>();
        var skills = new Dictionary<string, Tally>();
        var attackTypes = new Dictionary<string, Tally>();
        var professions = new Dictionary<string, Tally>();
        var withMates = new Dictionary<(string, string), Tally>();
        var against = new Dictionary<(string, string), Tally>();

        static Tally Get<K>(Dictionary<K, Tally> d, K key) where K : notnull
        {
            if (!d.TryGetValue(key, out var t)) d[key] = t = new Tally();
            return t;
        }

        foreach (var r in ok)
        {
            foreach (var u in r.Units)
            {
                var won = r.Result == "SideAWins" ? u.Side == "A" : r.Result == "SideBWins" && u.Side == "B";
                var draw = r.Result == "Draw";
                void Add(Tally t)
                {
                    t.Appearances++;
                    if (won) t.Wins++;
                    if (draw) t.Draws++;
                    t.Damage += u.Damage; t.Taken += u.Taken; t.Healing += u.Healing; t.Kos += u.Kos; t.Uses += u.Skills.Values.Sum();
                }
                Add(Get(creatures, u.Creature));
                Add(Get(byRow, (u.Creature, u.Row)));
                var def = content.CreaturesById[u.Creature];
                Add(Get(attackTypes, def.AttackType));
                foreach (var p in def.Professions) Add(Get(professions, p));
                foreach (var skill in u.Build.Concat(new[] { u.Skills.Keys.FirstOrDefault(k => content.AttackSkills.Any(a => a.Id == k)) ?? "" }).Where(s => s != "").Distinct())
                {
                    var t = Get(skills, skill);
                    t.Appearances++;
                    if (won) t.Wins++;
                    if (draw) t.Draws++;
                    t.Uses += u.Skills.GetValueOrDefault(skill);
                }
            }
            for (var i = 0; i < r.Units.Count; i++)
                for (var j = 0; j < r.Units.Count; j++)
                {
                    if (i == j) continue;
                    var a = r.Units[i];
                    var b = r.Units[j];
                    var aWon = r.Result == "SideAWins" ? a.Side == "A" : r.Result == "SideBWins" && a.Side == "B";
                    var t = a.Side == b.Side ? Get(withMates, (a.Creature, b.Creature)) : Get(against, (a.Creature, b.Creature));
                    t.Appearances++;
                    if (aWon) t.Wins++;
                }
        }

        string F(double v) => v.ToString("F1", CultureInfo.InvariantCulture);
        var csv = new StringBuilder();
        csv.AppendLine("creature,appearances,wins,draws,win_rate,avg_damage,avg_taken,avg_healing,avg_kos");
        foreach (var (name, t) in creatures.OrderBy(x => x.Key))
            csv.AppendLine($"{name},{t.Appearances},{t.Wins},{t.Draws},{F(t.Rate)},{F((double)t.Damage / t.Appearances)},{F((double)t.Taken / t.Appearances)},{F((double)t.Healing / t.Appearances)},{F((double)t.Kos / t.Appearances)}");
        File.WriteAllText(Path.Combine(folder, "creatures.csv"), csv.ToString());

        csv.Clear();
        csv.AppendLine("kind,name,appearances,wins,win_rate");
        foreach (var (n, t) in attackTypes.OrderBy(x => x.Key)) csv.AppendLine($"attack_type,{n},{t.Appearances},{t.Wins},{F(t.Rate)}");
        foreach (var (n, t) in professions.OrderBy(x => x.Key)) csv.AppendLine($"profession,{n},{t.Appearances},{t.Wins},{F(t.Rate)}");
        File.WriteAllText(Path.Combine(folder, "classes.csv"), csv.ToString());

        csv.Clear();
        csv.AppendLine("skill,units_with_skill,wins,win_rate,uses,uses_per_unit");
        foreach (var (n, t) in skills.OrderBy(x => x.Key)) csv.AppendLine($"{n},{t.Appearances},{t.Wins},{F(t.Rate)},{t.Uses},{F((double)t.Uses / t.Appearances)}");
        File.WriteAllText(Path.Combine(folder, "skills.csv"), csv.ToString());

        csv.Clear();
        csv.AppendLine("creature,row,appearances,wins,win_rate");
        foreach (var ((c, row), t) in byRow.OrderBy(x => x.Key)) csv.AppendLine($"{c},{row},{t.Appearances},{t.Wins},{F(t.Rate)}");
        File.WriteAllText(Path.Combine(folder, "rows.csv"), csv.ToString());

        csv.Clear();
        csv.AppendLine("kind,creature,other,appearances,wins,win_rate");
        foreach (var ((a, b), t) in withMates.Where(x => x.Value.Appearances >= 10).OrderBy(x => x.Key)) csv.AppendLine($"teammate,{a},{b},{t.Appearances},{t.Wins},{F(t.Rate)}");
        foreach (var ((a, b), t) in against.Where(x => x.Value.Appearances >= 10).OrderBy(x => x.Key)) csv.AppendLine($"opponent,{a},{b},{t.Appearances},{t.Wins},{F(t.Rate)}");
        File.WriteAllText(Path.Combine(folder, "pairs.csv"), csv.ToString());

        // The Markdown report.
        var md = new StringBuilder();
        var turns = ok.Select(r => r.Turns).OrderBy(x => x).ToList();
        md.AppendLine($"# Run {runId}");
        md.AppendLine();
        md.AppendLine($"AI level **{info.Level}**, seeds {info.FirstSeed} to {info.FirstSeed + (ulong)info.Battles - 1}, rules commit `{info.Commit}`.");
        md.AppendLine();
        md.AppendLine($"- Battles recorded: {records.Count} ({ok.Count} played to an end, {errors.Count} crashed)");
        md.AppendLine($"- Side A wins {ok.Count(r => r.Result == "SideAWins")}, side B wins {ok.Count(r => r.Result == "SideBWins")}, draws {ok.Count(r => r.Result == "Draw")} ({F(100.0 * ok.Count(r => r.Result == "Draw") / Math.Max(1, ok.Count))}%), of which {ok.Count(r => r.HitCap)} hit the length cap");
        if (turns.Count > 0) md.AppendLine($"- Battle length in unit turns: median {turns[turns.Count / 2]}, mean {F(turns.Average())}, shortest {turns[0]}, longest {turns[^1]}");
        md.AppendLine($"- Average real time per battle: {F(ok.Count == 0 ? 0 : ok.Average(r => r.Seconds))} s");
        md.AppendLine();

        var ranked = creatures.Where(x => x.Value.Appearances >= 20).OrderByDescending(x => x.Value.Rate).ToList();
        var tenth = Math.Max(1, ranked.Count / 10);
        md.AppendLine($"## Imbalance flags: top and bottom 10% by win rate ({tenth} each, creatures with at least 20 appearances)");
        md.AppendLine();
        md.AppendLine("| | Creature | Appearances | Win rate % | Avg damage | Avg healing |");
        md.AppendLine("|---|---|---:|---:|---:|---:|");
        foreach (var (n, t) in ranked.Take(tenth))
            md.AppendLine($"| top | {n} | {t.Appearances} | {F(t.Rate)} | {F((double)t.Damage / t.Appearances)} | {F((double)t.Healing / t.Appearances)} |");
        foreach (var (n, t) in ranked.TakeLast(tenth).Reverse())
            md.AppendLine($"| bottom | {n} | {t.Appearances} | {F(t.Rate)} | {F((double)t.Damage / t.Appearances)} | {F((double)t.Healing / t.Appearances)} |");
        md.AppendLine();

        md.AppendLine("## Attack types and professions");
        md.AppendLine();
        md.AppendLine("| Kind | Name | Appearances | Win rate % |");
        md.AppendLine("|---|---|---:|---:|");
        foreach (var (n, t) in attackTypes.OrderByDescending(x => x.Value.Rate)) md.AppendLine($"| attack type | {n} | {t.Appearances} | {F(t.Rate)} |");
        foreach (var (n, t) in professions.OrderByDescending(x => x.Value.Rate)) md.AppendLine($"| profession | {n} | {t.Appearances} | {F(t.Rate)} |");
        md.AppendLine();

        var skillRanked = skills.Where(x => x.Value.Appearances >= 50).OrderByDescending(x => x.Value.Rate).ToList();
        md.AppendLine("## Skills: best and worst win rate of the units that have them (at least 50 units), and usage");
        md.AppendLine();
        md.AppendLine("| | Skill | Units | Win rate % | Uses per unit |");
        md.AppendLine("|---|---|---:|---:|---:|");
        foreach (var (n, t) in skillRanked.Take(15)) md.AppendLine($"| best | {n} | {t.Appearances} | {F(t.Rate)} | {F((double)t.Uses / t.Appearances)} |");
        foreach (var (n, t) in skillRanked.TakeLast(15).Reverse()) md.AppendLine($"| worst | {n} | {t.Appearances} | {F(t.Rate)} | {F((double)t.Uses / t.Appearances)} |");
        var unused = content.ProfessionSkills.Where(s => s.Kind == "A" || s.Kind.StartsWith("A (")).Where(s => !skills.TryGetValue(s.Id, out var t) || t.Uses == 0).Select(s => s.Id).ToList();
        md.AppendLine();
        md.AppendLine($"Active skills never used in any battle ({unused.Count}): {string.Join(", ", unused)}");
        md.AppendLine();

        md.AppendLine("## Win rate by starting row (all creatures)");
        md.AppendLine();
        md.AppendLine("| Row | Appearances | Win rate % |");
        md.AppendLine("|---|---:|---:|");
        foreach (var row in new[] { "Front", "Middle", "Rear" })
        {
            var a = byRow.Where(x => x.Key.Item2 == row).Sum(x => x.Value.Appearances);
            var w = byRow.Where(x => x.Key.Item2 == row).Sum(x => x.Value.Wins);
            md.AppendLine($"| {row} | {a} | {F(a == 0 ? 0 : 100.0 * w / a)} |");
        }
        md.AppendLine();

        var matesRanked = withMates.Where(x => x.Value.Appearances >= 25 && string.CompareOrdinal(x.Key.Item1, x.Key.Item2) < 0).OrderByDescending(x => x.Value.Rate).ToList();
        md.AppendLine("## Teammate pairs (at least 25 shared battles)");
        md.AppendLine();
        md.AppendLine("| | Pair | Battles | Win rate % |");
        md.AppendLine("|---|---|---:|---:|");
        foreach (var ((a, b), t) in matesRanked.Take(10)) md.AppendLine($"| best | {a} + {b} | {t.Appearances} | {F(t.Rate)} |");
        foreach (var ((a, b), t) in matesRanked.TakeLast(10).Reverse()) md.AppendLine($"| worst | {a} + {b} | {t.Appearances} | {F(t.Rate)} |");
        md.AppendLine();

        if (errors.Count > 0)
        {
            md.AppendLine("## Crashed battles");
            md.AppendLine();
            foreach (var g in errors.GroupBy(e => e.Error!.Split('\n')[0]).OrderByDescending(g => g.Count()).Take(10))
                md.AppendLine($"- {g.Count()}x `{g.Key}` (first seed {g.Min(e => e.Seed)})");
            md.AppendLine();
        }
        File.WriteAllText(Path.Combine(folder, "report.md"), md.ToString());
        log($"report written to {Path.Combine(folder, "report.md")}");
    }
}
