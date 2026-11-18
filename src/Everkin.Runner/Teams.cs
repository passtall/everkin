using System.Text.Json;
using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;

namespace Everkin.Runner;

/// <summary>One team of a batch file.</summary>
public sealed class BatchTeam
{
    public string Id { get; set; } = "";
    public string Anchor { get; set; } = "";
    public string Template { get; set; } = "";
    public int Cost { get; set; }
    public List<BatchUnit> Units { get; set; } = new();
}

public sealed class BatchUnit
{
    public string Creature { get; set; } = "";
    public string Row { get; set; } = "";
}

/// <summary>A named, committed set of sample teams (teams/batches/&lt;name&gt;.json). A batch can be played again after any rule change.</summary>
public sealed class TeamBatch
{
    public string Name { get; set; } = "";
    public string Note { get; set; } = "";
    public int BuilderVersion { get; set; }
    public ulong Seed { get; set; }
    public int TeamsPerCreature { get; set; }
    public string Commit { get; set; } = "";
    public List<BatchTeam> Teams { get; set; } = new();

    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static string Folder(string repoRoot) => Path.Combine(repoRoot, "teams", "batches");
    public static string PathOf(string repoRoot, string name) => Path.Combine(Folder(repoRoot), name + ".json");

    public static TeamBatch Load(string path) => JsonSerializer.Deserialize<TeamBatch>(File.ReadAllText(path))!;

    public void Save(string repoRoot)
    {
        Directory.CreateDirectory(Folder(repoRoot));
        File.WriteAllText(PathOf(repoRoot, Name), JsonSerializer.Serialize(this, Options));
    }

    /// <summary>
    /// A batch with <paramref name="perCreature"/> teams anchored on every creature (equal appearance as anchor), each from its own seeded generator,
    /// so adding creatures or changing one team never changes the others.
    /// </summary>
    public static TeamBatch Build(ContentSet content, string name, ulong seed, int perCreature, string note, string commit)
    {
        var batch = new TeamBatch { Name = name, Note = note, BuilderVersion = TeamBuilder.Version, Seed = seed, TeamsPerCreature = perCreature, Commit = commit };
        foreach (var creature in content.Creatures.OrderBy(c => c.Id, StringComparer.Ordinal))
            for (var k = 0; k < perCreature; k++)
            {
                var random = new BattleRandom(seed ^ Hash(creature.Id) ^ ((ulong)k * 0x9E3779B97F4A7C15UL));
                var built = TeamBuilder.Build(content, creature, random);
                batch.Teams.Add(new BatchTeam
                {
                    Id = $"{creature.Id}#{k + 1}", Anchor = creature.Id, Template = built.Template, Cost = built.Cost,
                    Units = built.Team.Units.Select(u => new BatchUnit { Creature = u.CreatureId, Row = u.Row.ToString() }).ToList(),
                });
            }
        return batch;
    }

    /// <summary>
    /// A batch with every constellation of <see cref="TeamBuilder.Constellations"/> built <paramref name="perConstellation"/> times (the anchors go round the
    /// creatures whose role fits, so every creature anchors about equally often), plus <paramref name="perCreature"/> template teams for every creature.
    /// </summary>
    public static TeamBatch BuildConstellations(ContentSet content, string name, ulong seed, int perConstellation, int perCreature, string note, string commit)
    {
        var batch = Build(content, name, seed, perCreature, note, commit);
        var creatures = content.Creatures.OrderBy(c => c.Id, StringComparer.Ordinal).ToList();
        var cursor = 0;
        foreach (var (cname, slots) in TeamBuilder.Constellations())
        {
            var fitting = creatures.Where(c => TeamBuilder.Fits(content, c, slots)).ToList();
            for (var k = 0; k < perConstellation; k++)
            {
                var anchor = fitting[(cursor++) % fitting.Count];
                var random = new BattleRandom(seed ^ Hash(cname) ^ Hash(anchor.Id) ^ ((ulong)(k + 1) * 0xD1B54A32D192ED03UL));
                var built = TeamBuilder.Build(content, anchor, random, constellation: cname, constellationSlots: slots);
                batch.Teams.Add(new BatchTeam
                {
                    Id = $"{cname}:{anchor.Id}#{k + 1}", Anchor = anchor.Id, Template = cname, Cost = built.Cost,
                    Units = built.Team.Units.Select(u => new BatchUnit { Creature = u.CreatureId, Row = u.Row.ToString() }).ToList(),
                });
            }
        }
        return batch;
    }

    public TeamSetup SetupOf(BatchTeam t) => new(t.Units.Select(u => new UnitSetup(u.Creature, Enum.Parse<Row>(u.Row))).ToList());

    /// <summary>
    /// The spec of battle number <paramref name="index"/>: the "focus" team is team index mod count (so every team gets the same number of battles), its opponent is drawn
    /// from the other teams, and the side each plays is a seeded coin flip.
    /// </summary>
    public (BattleSpec Spec, string TeamA, string TeamB) SpecFor(ulong seed, long index)
    {
        var random = new BattleRandom(seed ^ 0xB47C4A7UL);
        var focus = (int)(index % Teams.Count);
        var other = random.NextInt(Teams.Count - 1);
        if (other >= focus) other++;
        var focusFirst = random.NextInt(2) == 0;
        var (a, b) = focusFirst ? (Teams[focus], Teams[other]) : (Teams[other], Teams[focus]);
        return (new BattleSpec(seed, SetupOf(a), SetupOf(b)), a.Id, b.Id);
    }

    private static ulong Hash(string s)
    {
        ulong h = 1469598103934665603UL;
        foreach (var ch in s) h = (h ^ ch) * 1099511628211UL;
        return h;
    }
}
