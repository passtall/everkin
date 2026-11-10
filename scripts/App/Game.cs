using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Ai;
using Everkin.Core.Content;
using Everkin.Core.Skills;
using Godot;

namespace Everkin.App;

/// <summary>The loaded rules content and shared resources of the running game.</summary>
public static class Game
{
    public static ContentSet Content = null!;
    public static SkillCatalog Catalog = null!;
    public static SkillTrees Trees = null!;
    public static AiSettings Settings = null!;

    private static readonly Dictionary<string, Texture2D?> Textures = new();
    private static readonly Dictionary<string, Font?> Fonts = new();

    /// <summary>Loads the content JSON from res://content (packed into exports) once.</summary>
    public static void Load()
    {
        if (Content != null) return;
        string Read(string name) => FileAccess.GetFileAsString("res://content/" + name);
        Content = ContentLoader.LoadFromJson(Read("classes.json"), Read("skills.json"), Read("creatures.json"), Read("statuses.json"), Read("default_builds.json"));
        Catalog = SkillCatalog.Create(Content);
        Trees = new SkillTrees(Content);
        Settings = AiSettings.FromJson(Read("ai_settings.json"));
    }

    /// <summary>A texture from res:// (null if the file is missing). Cached.</summary>
    public static Texture2D? Tex(string path)
    {
        if (Textures.TryGetValue(path, out var cached)) return cached;
        Texture2D? tex = ResourceLoader.Exists(path) ? ResourceLoader.Load<Texture2D>(path) : null;
        Textures[path] = tex;
        return tex;
    }

    public static Font Heading() => LoadFont("res://assets/fonts/Alegreya-Variable.ttf");

    public static Font Body() => LoadFont("res://assets/fonts/AlegreyaSans-Medium.ttf");

    public static Font BodyBold() => LoadFont("res://assets/fonts/AlegreyaSans-Bold.ttf");

    private static Font LoadFont(string path)
    {
        if (!Fonts.TryGetValue(path, out var f))
            Fonts[path] = f = ResourceLoader.Exists(path) ? ResourceLoader.Load<Font>(path) : null;
        return f ?? ThemeDB.FallbackFont;
    }

    /// <summary>The display name of a skill ID (Attack Type attacks and profession nodes).</summary>
    public static string SkillName(string id) =>
        Content.AttackSkills.FirstOrDefault(a => a.Id == id)?.Name ?? Content.ProfessionSkills.FirstOrDefault(p => p.Id == id)?.Name ?? id.Replace('_', ' ');

    /// <summary>The short type line of a skill on a card back, for example "Melee · D1" or "Passive".</summary>
    public static string SkillLine(string id)
    {
        if (Content.AttackSkills.FirstOrDefault(a => a.Id == id) is { } a)
            return Content.AttackTypes.FirstOrDefault(t => t.Id == a.AttackType)?.Name ?? a.AttackType;
        if (Content.ProfessionSkills.FirstOrDefault(p => p.Id == id) is not { } d) return "";
        return d.Kind switch
        {
            "P" => "Passive",
            "K" => "Keystone",
            "Mod" => "Modifier",
            _ => string.Join(" · ", new[] { d.Delivery, d.Range }.Where(x => !string.IsNullOrWhiteSpace(x))),
        };
    }

    /// <summary>The card-back rows for a list of skill IDs: Delay and Momentum come from the implemented skills.</summary>
    public static List<CardSkill> CardSkills(IEnumerable<string> ids)
    {
        var rows = new List<CardSkill>();
        foreach (var id in ids)
            if (Catalog.TryGet(id, out var skill))
                rows.Add(new CardSkill(SkillName(id), SkillLine(id), skill.Delay, skill.MomentumCost));
        return rows;
    }
}
