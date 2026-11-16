using System.Text.RegularExpressions;
using Everkin.Core.Battle;

namespace Everkin.Core.Content;

/// <summary>What a skill tooltip shows (ui_spec "Skill tooltip"). Delay and Momentum are null for a keystone.</summary>
public sealed record SkillTooltip(string Name, string Kind, int? Delay, int? Momentum, string? Delivery, string Effect);

/// <summary>
/// Builds the tooltip of any skill, keystone, unique attack, Move or Skip Turn with the numbers scaled to a creature's
/// Power (game_system section 5.2; skills.md marks every scaling number with braces, unique attacks carry their damage as data).
/// </summary>
public static class SkillTooltips
{
    private static readonly Regex Marked = new(@"\{(\d+)(?:–(\d+))?\}", RegexOptions.Compiled);

    /// <summary>Replaces each <c>{n}</c> or <c>{a–b}</c> of a skills.md effect with the value scaled to <paramref name="power"/>.</summary>
    public static string ScaleTemplate(string template, int power) =>
        Marked.Replace(template, m =>
        {
            var low = DamageMath.Scale(int.Parse(m.Groups[1].Value), power);
            if (!m.Groups[2].Success) return low.ToString();
            var high = DamageMath.Scale(int.Parse(m.Groups[2].Value), power);
            return $"{low}–{high}";
        });

    /// <summary>The effect text of a unique attack with its damage (and splash damage) scaled to the Power.</summary>
    public static string ScaleAttack(AttackDef attack, int power)
    {
        var text = attack.Effect;
        // The splash range comes second in the text of a breath ("... and every unit next to it for c–d"), so replace it first.
        if (attack.Shape == "splash" && attack.SplashMax > 0)
            text = ReplaceLast(text, $"{attack.SplashMin}–{attack.SplashMax}",
                $"{DamageMath.Scale(attack.SplashMin, power)}–{DamageMath.Scale(attack.SplashMax, power)}");
        var first = $"{attack.Min}–{attack.Max}";
        var at = text.IndexOf(first, StringComparison.Ordinal);
        if (at >= 0)
            text = string.Concat(text.AsSpan(0, at), $"{DamageMath.Scale(attack.Min, power)}–{DamageMath.Scale(attack.Max, power)}", text.AsSpan(at + first.Length));
        return text;
    }

    private static string ReplaceLast(string text, string find, string with)
    {
        var at = text.LastIndexOf(find, StringComparison.Ordinal);
        return at < 0 ? text : string.Concat(text.AsSpan(0, at), with, text.AsSpan(at + find.Length));
    }

    /// <summary>The tooltip of a skill ID (unique attack, class active or keystone) for a creature of the given Power, or null for an unknown ID.</summary>
    public static SkillTooltip? For(ContentSet content, string id, int power)
    {
        if (content.AttacksById.TryGetValue(id, out var a))
            return new SkillTooltip(a.Name, a.Delivery == "direct" ? (a.Shape == "pulse" ? "Attack · Pulse" : "Attack · Breath") : "Attack",
                a.Delay, a.Momentum, a.Delivery, ScaleAttack(a, power));
        if (!content.SkillsById.TryGetValue(id, out var s)) return null;
        var cls = content.ClassesById[s.Class].Name;
        var isKeystone = s.Kind == "K";
        return new SkillTooltip(s.Name, isKeystone ? $"{cls} · Keystone" : cls,
            isKeystone ? null : s.Delay, isKeystone ? null : s.Momentum,
            isKeystone || s.Delivery is null or "none" ? null : s.Delivery, ScaleTemplate(s.Template ?? s.Effect, power));
    }

    public static SkillTooltip Move { get; } = new("Move", "Action", Movement.MoveDelay, 0, null,
        "Move to a free place in another row of your side. Moves no Momentum.");

    public static SkillTooltip SkipTurn { get; } = new("Skip Turn", "Action", Timeline.SkippedTurnDelay, 0, null,
        "Do nothing this turn. Always allowed. Moves no Momentum.");
}
