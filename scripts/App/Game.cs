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
    public static AiSettings Settings = null!;
    public static CampaignDef Campaign = null!;

    /// <summary>The content files a battle in progress depends on (a saved battle resumes only if their fingerprint is unchanged).</summary>
    public static string ContentFingerprint = "";

    private static readonly Dictionary<string, Texture2D?> Textures = new();
    private static readonly Dictionary<string, Font?> Fonts = new();

    /// <summary>Loads the content JSON from res://content (packed into exports) once.</summary>
    public static void Load()
    {
        if (Content != null) return;
        string Read(string name) => FileAccess.GetFileAsString("res://content/" + name);
        Content = ContentLoader.LoadFromJson(Read("classes.json"), Read("skills.json"), Read("creatures.json"), Read("statuses.json"));
        Catalog = SkillCatalog.Create(Content);
        Settings = AiSettings.FromJson(Read("ai_settings.json"));
        Campaign = CampaignDef.FromJson(Read("campaign.json"), Content);
        ContentFingerprint = Everkin.Core.Setup.ContentHash.Of(new[] { "classes.json", "skills.json", "creatures.json", "statuses.json" }.Select(Read));
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

    /// <summary>The display name of a skill ID (a unique attack, a class active or a keystone).</summary>
    public static string SkillName(string id) =>
        Content.AttacksById.TryGetValue(id, out var a) ? a.Name : Content.SkillsById.TryGetValue(id, out var s) ? s.Name : id.Replace('_', ' ');

    /// <summary>The short type line of a skill on a card back, for example "Melee attack" or "Keystone . Berserker".</summary>
    public static string SkillLine(string id)
    {
        static string Cap(string text) => text.Length == 0 ? text : char.ToUpperInvariant(text[0]) + text[1..];
        if (Content.AttacksById.TryGetValue(id, out var a))
            return a.Delivery == "direct" ? (a.Shape == "pulse" ? "Pulse \u00b7 melee reach" : "Breath \u00b7 melee reach") : $"{Cap(a.Delivery)} attack";
        if (!Content.SkillsById.TryGetValue(id, out var d)) return "";
        var cls = Content.ClassesById[d.Class].Name;
        return d.Kind == "K" ? $"Keystone \u00b7 {cls}" : d.Delivery is null or "none" ? cls : $"{Cap(d.Delivery)} \u00b7 {cls}";
    }

    /// <summary>The card-back rows for a list of skill IDs: Delay and Momentum come from the content (a keystone has neither).</summary>
    public static List<CardSkill> CardSkills(IEnumerable<string> ids, int power)
    {
        var rows = new List<CardSkill>();
        foreach (var id in ids)
        {
            if (Content.AttacksById.TryGetValue(id, out var a)) rows.Add(new CardSkill(a.Name, SkillLine(id), a.Delay, a.Momentum, SkillTooltips.For(Content, id, power)));
            else if (Content.SkillsById.TryGetValue(id, out var s)) rows.Add(new CardSkill(s.Name, SkillLine(id), s.Delay ?? 0, s.Momentum ?? 0, SkillTooltips.For(Content, id, power)));
        }
        return rows;
    }
}
