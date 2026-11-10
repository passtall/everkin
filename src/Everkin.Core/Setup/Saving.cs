using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Setup;

/// <summary>
/// Saving a JSON file safely (production section 1.4): the new text goes to a temporary file and is checked, then swapped in; the previous file
/// becomes backup 1, the older ones move up, and only the last three backups are kept. Loading takes the newest file that parses.
/// </summary>
public sealed class SaveStore
{
    public const int Backups = 3;
    private readonly string _path;

    public SaveStore(string path) => _path = path;

    /// <summary>What the last <see cref="Load"/> had to do: null when the newest file was fine, else a short note for the player.</summary>
    public string? LoadNote { get; private set; }

    public string PathOf => _path;

    public void Save(string json)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(_path))!);
        var temp = _path + ".tmp";
        File.WriteAllText(temp, json, new UTF8Encoding(false));
        using (JsonDocument.Parse(File.ReadAllText(temp))) { }                          // never swap in text that does not parse
        if (File.Exists(_path))
        {
            for (var i = Backups; i >= 2; i--)
                if (File.Exists(Backup(i - 1))) File.Move(Backup(i - 1), Backup(i), overwrite: true);
            File.Move(_path, Backup(1), overwrite: true);
        }
        File.Move(temp, _path, overwrite: true);
    }

    /// <summary>The text of the newest file that is valid JSON, or null when there is none.</summary>
    public string? Load()
    {
        LoadNote = null;
        var candidates = new[] { _path }.Concat(Enumerable.Range(1, Backups).Select(Backup));
        var first = true;
        foreach (var file in candidates)
        {
            if (!File.Exists(file)) { first = false; continue; }
            try
            {
                var text = File.ReadAllText(file);
                using (JsonDocument.Parse(text)) { }
                if (!first) LoadNote = "The save was damaged; the newest good backup was loaded.";
                return text;
            }
            catch (JsonException) { first = false; }
            catch (IOException) { first = false; }
        }
        return null;
    }

    public void Delete()
    {
        foreach (var f in new[] { _path, _path + ".tmp" }.Concat(Enumerable.Range(1, Backups).Select(Backup)))
            if (File.Exists(f)) File.Delete(f);
    }

    private string Backup(int n) => $"{_path}.bak{n}";
}

/// <summary>A fingerprint of the content a battle used: a battle in progress resumes only if it is unchanged (production section 1.4).</summary>
public static class ContentHash
{
    public static string Of(IEnumerable<string> texts)
    {
        using var sha = SHA256.Create();
        foreach (var t in texts) sha.TransformBlock(Encoding.UTF8.GetBytes(t.ReplaceLineEndings("\n")), 0, Encoding.UTF8.GetByteCount(t.ReplaceLineEndings("\n")), null, 0);
        sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        return Convert.ToHexString(sha.Hash!);
    }
}

/// <summary>One action taken in a battle, in a form that can be saved and played again.</summary>
public sealed record SavedAction(
    string Kind, int Unit, string? Skill = null, int? Target = null, int? Second = null,
    string? RowSide = null, string? RowRow = null, string? PlaceRow = null, int? PlaceIndex = null, string? MoveRow = null, int? MoveGap = null)
{
    public static SavedAction From(int unitId, AiAction a) => a.Kind switch
    {
        AiActionKind.Move => new SavedAction("move", unitId, MoveRow: a.Row.ToString(), MoveGap: a.Gap),
        AiActionKind.SkipTurn => new SavedAction("skip", unitId),
        _ => new SavedAction("skill", unitId, a.Use!.SkillId, a.Use.TargetUnitId, a.Use.SecondTargetUnitId,
            a.Use.Row?.Side.ToString(), a.Use.Row?.Row.ToString(), a.Use.Placement?.Row.ToString(), a.Use.Placement?.Index),
    };

    public AiAction ToAction() => Kind switch
    {
        "move" => new AiAction(AiActionKind.Move, null, Enum.Parse<Row>(MoveRow!), MoveGap ?? 0),
        "skip" => AiAction.Skip,
        _ => new AiAction(AiActionKind.Skill, new SkillUse(Unit, Skill!, Target,
            RowSide != null ? new RowTarget(Enum.Parse<Side>(RowSide), Enum.Parse<Row>(RowRow!)) : null,
            PlaceRow != null ? new MoveOption(Enum.Parse<Row>(PlaceRow), PlaceIndex ?? 0) : null, Second)),
    };
}

/// <summary>A battle in progress: its setup and every action so far. Playing the actions again from the setup gives the same position, so the battle resumes where it stopped.</summary>
public sealed class BattleSave
{
    public int Version { get; set; } = 1;
    public string ContentHash { get; set; } = "";
    public ulong Seed { get; set; }
    public List<SavedUnit> SideA { get; set; } = new();
    public List<SavedUnit> SideB { get; set; } = new();
    public string Level { get; set; } = "normal";
    public string? Encounter { get; set; }
    public List<SavedAction> Actions { get; set; } = new();

    public sealed class SavedUnit
    {
        public string Creature { get; set; } = "";
        public List<string> Build { get; set; } = new();
        public string Row { get; set; } = "Front";
        public bool Boss { get; set; }
    }

    private static readonly JsonSerializerOptions Options = new() { WriteIndented = false };

    public static BattleSave Of(BattleSpec spec, string level, string? encounter, string contentHash) => new()
    {
        ContentHash = contentHash, Seed = spec.Seed, Level = level, Encounter = encounter,
        SideA = spec.SideA.Units.Select(u => new SavedUnit { Creature = u.CreatureId, Build = u.Build.ToList(), Row = u.Row.ToString(), Boss = u.Boss }).ToList(),
        SideB = spec.SideB.Units.Select(u => new SavedUnit { Creature = u.CreatureId, Build = u.Build.ToList(), Row = u.Row.ToString(), Boss = u.Boss }).ToList(),
    };

    public BattleSpec ToSpec() => new(Seed,
        new TeamSetup(SideA.Select(u => new UnitSetup(u.Creature, u.Build, Enum.Parse<Row>(u.Row), u.Boss)).ToList()),
        new TeamSetup(SideB.Select(u => new UnitSetup(u.Creature, u.Build, Enum.Parse<Row>(u.Row), u.Boss)).ToList()));

    public string ToJson() => JsonSerializer.Serialize(this, Options);

    public static BattleSave? FromJson(string json) => JsonSerializer.Deserialize<BattleSave>(json, Options);

    /// <summary>
    /// Plays the saved actions again from the setup. Returns the state, or null when the saved battle no longer fits the rules (a unit that
    /// should act did not, or an action is not legal): the caller then drops the save.
    /// </summary>
    public BattleState? Replay(SkillCatalog catalog)
    {
        var state = BattleSetup.Create(ToSpec(), catalog);
        foreach (var saved in Actions)
        {
            if (state.IsOver) break;
            var unit = BattleDriver.NextTurn(state, catalog);
            state.DrainEvents();
            if (unit != saved.Unit) return null;
            var action = saved.ToAction();
            if (action.Kind == AiActionKind.Skill && Skills.Actions.WhyNot(state, catalog, action.Use!) is not null) return null;
            action.Apply(state, catalog, saved.Unit, validated: true);
            state.DrainEvents();
            BattleEnd.CheckTurnCap(state);
        }
        return state;
    }
}
