using Godot;

namespace Everkin.App;

/// <summary>The look of the HUD (ui_spec "HUD style"): a dark panel with a gold frame, bronze-rimmed slots.</summary>
public static class Ui
{
    public static readonly Color Gold = new("d8b45a");
    public static readonly Color Bronze = new("8a5a2b");
    public static readonly Color PanelDark = new("14100d");
    public static readonly Color Ink = new("f1e6cf");
    public static readonly Color SideA = new("3f7fd1");
    public static readonly Color SideB = new("c4483f");
    public static readonly Color Good = new("6fcf6f");
    public static readonly Color Bad = new("e0584c");

    public static StyleBoxFlat Box(Color bg, Color border, int borderWidth = 2, int radius = 6)
    {
        var s = new StyleBoxFlat { BgColor = bg, BorderColor = border, CornerRadiusTopLeft = radius, CornerRadiusTopRight = radius, CornerRadiusBottomLeft = radius, CornerRadiusBottomRight = radius };
        s.SetBorderWidthAll(borderWidth);
        s.SetContentMarginAll(8);
        return s;
    }

    public static PanelContainer Panel(Control? child = null)
    {
        var p = new PanelContainer();
        p.AddThemeStyleboxOverride("panel", Box(new Color(PanelDark, 0.92f), Gold));
        if (child != null) p.AddChild(child);
        return p;
    }

    /// <summary>The skill tooltip panel (ui_spec "Skill tooltip"): name, class, Delay, Momentum, delivery and the effect text; an optional note (why it cannot be used now).</summary>
    public static Control SkillTip(Everkin.Core.Content.SkillTooltip tip, string? note = null)
    {
        var box = new VBoxContainer { CustomMinimumSize = new Vector2(420, 0) };
        box.AddThemeConstantOverride("separation", 4);
        box.AddChild(Label(tip.Name, 26, Gold, true));
        var facts = new System.Collections.Generic.List<string> { tip.Kind };
        if (tip.Delay is { } d) facts.Add($"Delay {d}");
        if (tip.Momentum is { } m) facts.Add($"Momentum {m}");
        if (tip.Delivery is { } dv) facts.Add(char.ToUpperInvariant(dv[0]) + dv[1..]);
        box.AddChild(Label(string.Join(" · ", facts), 18, new Color("cdbd9a")));
        var effect = Label(tip.Effect, 20);
        effect.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        effect.CustomMinimumSize = new Vector2(420, 0);
        box.AddChild(effect);
        if (note != null)
        {
            var n = Label(note, 18, new Color("e07060"));
            n.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            n.CustomMinimumSize = new Vector2(420, 0);
            box.AddChild(n);
        }
        var margin = new MarginContainer();
        foreach (var side in new[] { "left", "right", "top", "bottom" }) margin.AddThemeConstantOverride("margin_" + side, 10);
        margin.AddChild(box);
        return Panel(margin);
    }

    public static Label Label(string text, int size = 22, Color? color = null, bool bold = false, HorizontalAlignment align = HorizontalAlignment.Left)
    {
        var l = new Label { Text = text, HorizontalAlignment = align };
        l.AddThemeFontOverride("font", bold ? Game.BodyBold() : Game.Body());
        l.AddThemeFontSizeOverride("font_size", size);
        l.AddThemeColorOverride("font_color", color ?? Ink);
        return l;
    }

    public static Button Button(string text, int size = 22)
    {
        var b = new Button { Text = text };
        b.AddThemeFontOverride("font", Game.BodyBold());
        b.AddThemeFontSizeOverride("font_size", size);
        b.AddThemeColorOverride("font_color", Ink);
        b.AddThemeStyleboxOverride("normal", Box(new Color("2a2118"), Bronze));
        b.AddThemeStyleboxOverride("hover", Box(new Color("3a2e20"), Gold));
        b.AddThemeStyleboxOverride("pressed", Box(new Color("4a3a24"), Gold));
        b.AddThemeStyleboxOverride("disabled", Box(new Color("1a1612"), new Color("3a3228")));
        b.AddThemeColorOverride("font_disabled_color", new Color("7d725f"));
        return b;
    }

    public static void Fill(Control c) => c.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
}
