using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;
using Godot;

namespace Everkin.App;

/// <summary>A saved unit of a team: the creature and the row it starts in. Within a row units stand in list order.</summary>
public sealed class TeamUnit
{
    public string Creature { get; set; } = "";
    public string Row { get; set; } = "Front";
}

/// <summary>A named team (ui_spec "Party setup").</summary>
public sealed class TeamData
{
    public string Name { get; set; } = "Team";
    public List<TeamUnit> Units { get; set; } = new();
}

/// <summary>
/// What the player has saved (production section 1.4): the creatures owned, campaign progress, teams and skill builds. JSON with a version number;
/// older saves are upgraded step by step when loaded.
/// </summary>
public sealed class ProfileData
{
    public const int CurrentVersion = 2;

    public int Version { get; set; } = CurrentVersion;
    public List<string> Owned { get; set; } = new();
    public List<string> Completed { get; set; } = new();
    public List<TeamData> Teams { get; set; } = new();
    public int Selected { get; set; }
    public Dictionary<string, List<string>> Builds { get; set; } = new();
}

/// <summary>The player's profile, kept in user://profile.json with a temporary-file swap and three backups. Changes save at once (ui_spec: "no Save button").</summary>
public static class Profile
{
    private static ProfileData? _data;
    private static SaveStore? _store;

    private static SaveStore Store => _store ??= new SaveStore(ProjectSettings.GlobalizePath("user://profile.json"));

    /// <summary>A short note to show when the newest save was damaged and a backup was loaded.</summary>
    public static string? LoadNote { get; private set; }

    public static ProfileData Data => _data ??= Load();

    private static ProfileData Load()
    {
        ProfileData? data = null;
        try
        {
            if (Store.Load() is { } text) data = Upgrade(JsonSerializer.Deserialize<ProfileData>(text));
            LoadNote = Store.LoadNote;
        }
        catch (System.Exception e) { GD.PushWarning($"profile could not be read: {e.Message}"); }
        return data ?? New();
    }

    /// <summary>Brings an older save up to the current version, one step at a time.</summary>
    private static ProfileData? Upgrade(ProfileData? data)
    {
        if (data == null) return null;
        if (data.Version < 2)
        {
            // Version 1 had teams and builds only: the player now owns the starting creatures and has finished nothing.
            if (data.Owned.Count == 0) data.Owned = CampaignDef.StartingCreatures(Game.Content).ToList();
            data.Version = 2;
        }
        if (data.Owned.Count == 0) data.Owned = CampaignDef.StartingCreatures(Game.Content).ToList();
        return data;
    }

    private static ProfileData New() => new() { Owned = CampaignDef.StartingCreatures(Game.Content).ToList(), Teams = { new TeamData { Name = "Team 1" } } };

    public static void Save() => Store.Save(JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true }));

    /// <summary>Forgets what was loaded and points the profile at another file (tests and a changed user folder).</summary>
    public static void Reset(ProfileData? data = null, string? path = null)
    {
        _data = data;
        _store = path == null ? null : new SaveStore(path);
    }

    // ---------------------------------------------------------------- teams and builds

    public static TeamData CurrentTeam
    {
        get
        {
            if (Data.Teams.Count == 0) Data.Teams.Add(new TeamData { Name = "Team 1" });
            Data.Selected = System.Math.Clamp(Data.Selected, 0, Data.Teams.Count - 1);
            return Data.Teams[Data.Selected];
        }
    }

    /// <summary>The build of a creature: the player's own or the default one it arrived with.</summary>
    public static IReadOnlyList<string> BuildOf(string creatureId) =>
        Data.Builds.TryGetValue(creatureId, out var b) ? b : Game.Content.DefaultBuilds.TryGetValue(creatureId, out var d) ? d : new List<string>();

    public static void SetBuild(string creatureId, IEnumerable<string> build)
    {
        Data.Builds[creatureId] = build.ToList();
        Save();
    }

    public static int Cost(TeamData team) => team.Units.Where(u => Game.Content.CreaturesById.ContainsKey(u.Creature)).Sum(u => Game.Content.CreaturesById[u.Creature].Cost);

    /// <summary>The team as a battle setup (builds from the profile, rows as saved).</summary>
    public static TeamSetup ToSetup(TeamData team) =>
        new(team.Units.Select(u => new UnitSetup(u.Creature, BuildOf(u.Creature).ToArray(), System.Enum.Parse<Row>(u.Row))).ToList());

    // ---------------------------------------------------------------- collection and campaign progress

    public static bool Owns(string creatureId) => Data.Owned.Contains(creatureId);

    public static bool IsDone(string encounterId) => Data.Completed.Contains(encounterId);

    /// <summary>The tutorial (three battles) is finished: every other mode is open (game_system section 2.2).</summary>
    public static bool TutorialComplete => Game.Campaign.Tutorial.Encounters.All(e => IsDone(e.Id));

    /// <summary>The four story chapters are won: challenge encounters appear.</summary>
    public static bool StoryComplete => Game.Campaign.Story.All(c => IsDone(c.Encounters[^1].Id));

    /// <summary>The next encounter of a chapter the player has not won, or null.</summary>
    public static EncounterDef? NextOf(ChapterDef chapter) => chapter.Encounters.FirstOrDefault(e => !IsDone(e.Id));

    /// <summary>Whether a chapter can be entered: the tutorial first, then each chapter after the boss of the one before (a linear sequence).</summary>
    public static bool ChapterOpen(ChapterDef chapter)
    {
        if (chapter.Id == "T") return true;
        if (chapter.Id == "C") return StoryComplete;
        var all = new[] { Game.Campaign.Tutorial }.Concat(Game.Campaign.Story).ToList();
        var index = all.IndexOf(chapter);
        return index <= 0 || IsDone(all[index - 1].Encounters[^1].Id);
    }

    /// <summary>An encounter is open when it is the next of its chapter or already won (they are fought in a fixed order), and challenges once the story is won.</summary>
    public static bool EncounterOpen(EncounterDef e)
    {
        var chapter = Game.Campaign.ChapterOf(e.Id);
        if (!ChapterOpen(chapter)) return false;
        if (chapter.Id == "C") return true;
        return IsDone(e.Id) || NextOf(chapter) == e;
    }

    /// <summary>Records a win: the encounter is done and its reward creatures are owned (once). Returns the creatures that were new.</summary>
    public static List<string> Complete(EncounterDef e)
    {
        var fresh = new List<string>();
        if (!Data.Completed.Contains(e.Id)) Data.Completed.Add(e.Id);
        foreach (var r in e.Rewards)
            if (!Data.Owned.Contains(r)) { Data.Owned.Add(r); fresh.Add(r); }
        if (TutorialComplete && Data.Teams.FirstOrDefault(t => t.Name != "Tutorial team" && t.Units.Count == 0) is { } empty)
        {
            // The tutorial is over: the first empty saved team starts as the tutorial team, which the player can now change freely.
            var tutorial = TutorialTeam();
            empty.Units = tutorial.Units.Select(u => new TeamUnit { Creature = u.Creature, Row = u.Row }).ToList();
            Data.Selected = Data.Teams.IndexOf(empty);
        }
        Save();
        return fresh;
    }

    /// <summary>
    /// The tutorial's one automatic team (game_system section 2.2): every owned creature in a default formation (Guardians, Warriors and short
    /// melee in front, Polearm and Whip in the middle, Ranged, Magic, Breath and Aura behind). Edit team arranges it, so it is kept in the saved
    /// team list under its own name.
    /// </summary>
    public static TeamData TutorialTeam()
    {
        var team = Data.Teams.FirstOrDefault(t => t.Name == "Tutorial team");
        if (team == null) { team = new TeamData { Name = "Tutorial team" }; Data.Teams.Insert(0, team); }
        foreach (var id in Data.Owned.Where(o => team.Units.All(u => u.Creature != o)).ToList())
        {
            var def = Game.Content.CreaturesById[id];
            var row = def.AttackType switch { "polearm" or "whip" => "Middle", "ranged" or "magic" or "breath" or "aura" => "Rear", _ => "Front" };
            team.Units.Add(new TeamUnit { Creature = id, Row = row });
        }
        team.Units.RemoveAll(u => !Data.Owned.Contains(u.Creature));
        Data.Selected = Data.Teams.IndexOf(team);
        Save();
        return team;
    }

    /// <summary>Where a creature that is not owned can be had: the encounter that rewards it, or its coin price.</summary>
    public static string SourceOf(string creatureId)
    {
        var e = Game.Campaign.AllEncounters.FirstOrDefault(x => x.Rewards.Contains(creatureId));
        return e != null ? $"Reward of {e.Id} {e.Name}" : $"{Prices.CoinCreature(Game.Content.CreaturesById[creatureId].Cost)} coins";
    }
}
