using System.Text.Json;
using Everkin.Core.Battle;
using Everkin.Core.Setup;

namespace Everkin.Core.Content;

/// <summary>One creature of an encounter's enemy team: where it stands and whether it is the boss (campaign.md).</summary>
public sealed record EncounterUnit(string Creature, string Row, bool Boss);

/// <summary>A fixed battle of the campaign with its enemy team and the reward creatures (campaign.md, game_system section 2.2).</summary>
public sealed record EncounterDef(
    string Id, string Name, string Biome, string Ai, IReadOnlyList<EncounterUnit> Team, int Cost, IReadOnlyList<string> Rewards)
{
    public bool IsBoss => Team.Any(u => u.Boss);

    /// <summary>The enemy team: default builds (game_system section 4.1) and the stated formation; the boss is strengthened when the battle is set up.</summary>
    public TeamSetup EnemyTeam(ContentSet content) =>
        new(Team.Select(u => new UnitSetup(u.Creature, content.DefaultBuilds.TryGetValue(u.Creature, out var b) ? b : Array.Empty<string>(), Enum.Parse<Row>(u.Row), u.Boss)).ToList());
}

/// <summary>The tutorial, a chapter, or the challenge encounters.</summary>
public sealed record ChapterDef(string Id, string Name, IReadOnlyList<string> Biomes, IReadOnlyList<EncounterDef> Encounters);

/// <summary>The campaign (docs/campaign.md as content/campaign.json): the tutorial, chapters 1 to 4 and the challenge encounters.</summary>
public sealed class CampaignDef
{
    public IReadOnlyList<ChapterDef> Chapters { get; init; } = Array.Empty<ChapterDef>();

    /// <summary>The creatures the player owns at the start (campaign tutorial, adopted 2026-11-04): display names, looked up in the roster.</summary>
    public static readonly string[] StartingNames = { "Daggerwhisk", "Slingwhisker" };

    public ChapterDef Tutorial => Chapters.First(c => c.Id == "T");

    public ChapterDef Challenges => Chapters.First(c => c.Id == "C");

    /// <summary>The story chapters 1 to 4 in order.</summary>
    public IEnumerable<ChapterDef> Story => Chapters.Where(c => c.Id is "1" or "2" or "3" or "4");

    public IEnumerable<EncounterDef> AllEncounters => Chapters.SelectMany(c => c.Encounters);

    public EncounterDef Encounter(string id) => AllEncounters.First(e => e.Id == id);

    public ChapterDef ChapterOf(string encounterId) => Chapters.First(c => c.Encounters.Any(e => e.Id == encounterId));

    public static IReadOnlyList<string> StartingCreatures(ContentSet content) =>
        StartingNames.Select(n => content.Creatures.First(c => c.Name == n).Id).ToArray();

    public static CampaignDef FromJson(string json, ContentSet content)
    {
        var doc = JsonDocument.Parse(json).RootElement;
        var chapters = new List<ChapterDef>();
        foreach (var ch in doc.GetProperty("chapters").EnumerateArray())
        {
            var encounters = new List<EncounterDef>();
            foreach (var e in ch.GetProperty("encounters").EnumerateArray())
            {
                var id = e.GetProperty("id").GetString()!;
                var team = e.GetProperty("team").EnumerateArray()
                    .Select(t => new EncounterUnit(t.GetProperty("creature").GetString()!, t.GetProperty("row").GetString()!, t.GetProperty("boss").GetBoolean())).ToList();
                foreach (var u in team)
                    if (!content.CreaturesById.ContainsKey(u.Creature)) throw new ContentException("campaign.json", id, $"unknown creature '{u.Creature}'");
                var rewards = e.GetProperty("rewards").EnumerateArray().Select(r => r.GetString()!).ToList();
                encounters.Add(new EncounterDef(id, e.GetProperty("name").GetString()!, e.GetProperty("biome").GetString()!, e.GetProperty("ai").GetString()!, team, e.GetProperty("cost").GetInt32(), rewards));
            }
            chapters.Add(new ChapterDef(ch.GetProperty("id").GetString()!, ch.GetProperty("name").GetString()!, ch.GetProperty("biomes").EnumerateArray().Select(b => b.GetString()!).ToList(), encounters));
        }
        return new CampaignDef { Chapters = chapters };
    }
}
