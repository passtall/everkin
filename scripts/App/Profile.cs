using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Everkin.Core.Battle;
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

/// <summary>What the player has saved so far: teams and skill builds. (M5.1 adds versioned saving with backups and the campaign progress.)</summary>
public sealed class ProfileData
{
    public int Version { get; set; } = 1;
    public List<TeamData> Teams { get; set; } = new();
    public int Selected { get; set; }
    public Dictionary<string, List<string>> Builds { get; set; } = new();
}

/// <summary>The player's profile, kept in user://profile.json. Changes save at once (ui_spec: "no Save button").</summary>
public static class Profile
{
    private const string Path = "user://profile.json";
    private static ProfileData? _data;

    public static ProfileData Data => _data ??= Load();

    private static ProfileData Load()
    {
        try
        {
            if (FileAccess.FileExists(Path))
                return JsonSerializer.Deserialize<ProfileData>(FileAccess.GetFileAsString(Path)) ?? New();
        }
        catch (System.Exception e) { GD.PushWarning($"profile could not be read: {e.Message}"); }
        return New();
    }

    private static ProfileData New() => new() { Teams = { new TeamData { Name = "Team 1" } } };

    public static void Save()
    {
        using var file = FileAccess.Open(Path, FileAccess.ModeFlags.Write);
        file?.StoreString(JsonSerializer.Serialize(Data, new JsonSerializerOptions { WriteIndented = true }));
    }

    /// <summary>Forgets what was loaded (tests and a changed user folder).</summary>
    public static void Reset(ProfileData? data = null) => _data = data;

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
}
