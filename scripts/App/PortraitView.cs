using System;
using System.Collections.Generic;
using Godot;

namespace Everkin.App;

/// <summary>A control that hands its drawing to a callback (the layers of a portrait and of the ground).</summary>
public partial class DrawControl : Control
{
    public Action<DrawControl>? OnDraw;

    public DrawControl() => MouseFilter = MouseFilterEnum.Ignore;

    public override void _Draw() => OnDraw?.Invoke(this);
}

/// <summary>
/// The round battle portrait of a unit (ui_spec "Board"): the whole creature on a plain disc inside a ring, an HP badge, a KO crack, stealth
/// shimmer, airborne bob, the active glow and the target highlights. It is a <see cref="CardView"/> so the board and the screens handle it like
/// any card, but it draws nothing of the card.
/// </summary>
public partial class PortraitView : CardView
{
    private const float ArtRadius = 430f / 1024f;
    private static readonly Color DarkPlum = new("2a1a2e");
    private static readonly Color LighterPlum = new("5d4263");
    private static Dictionary<string, string>? _discs;
    private static Shader? _maskShader;

    private readonly DrawControl _body = new();
    private readonly TextureRect _art = new() { ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.Scale, MouseFilter = MouseFilterEnum.Ignore };
    private readonly TextureRect _ring = new() { ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.Scale, MouseFilter = MouseFilterEnum.Ignore };
    private readonly DrawControl _overlay = new();
    private string _signature = "";
    private double _time;

    public PortraitView()
    {
        ClipContents = false;
        _art.Material = new ShaderMaterial { Shader = MaskShader() };
        _body.AddChild(_art);
        _body.AddChild(_ring);
        _body.OnDraw = DrawDisc;
        _overlay.OnDraw = DrawOverlay;
        AddChild(_body);
        AddChild(_overlay);
        SetProcess(true);
    }

    private static Shader MaskShader() => _maskShader ??= new Shader
    {
        Code = "shader_type canvas_item;\nuniform float radius = 0.4199;\nvoid fragment() { COLOR = texture(TEXTURE, UV); if (length(UV - vec2(0.5)) > radius) COLOR.a = 0.0; }",
    };

    /// <summary>The portrait file of a creature or a summon; falls back to the card art when there is none.</summary>
    public static string? PortraitPath(CardInfo info)
    {
        if (info.ArtPath != null)
        {
            var p = info.ArtPath.Replace("assets/summons/", "assets/portraits/summons/");
            return ResourceLoader.Exists(p) ? p : info.ArtPath;
        }
        var path = $"res://assets/portraits/{info.ArtFile}";
        return ResourceLoader.Exists(path) ? path : $"res://assets/creatures/{info.ArtFile}";
    }

    private static bool LighterDisc(string path)
    {
        if (_discs == null)
        {
            _discs = new Dictionary<string, string>();
            foreach (var dir in new[] { "res://assets/portraits/", "res://assets/portraits/summons/" })
            {
                if (!FileAccess.FileExists(dir + "discs.json")) continue;
                if (Json.ParseString(FileAccess.GetFileAsString(dir + "discs.json")).Obj is not Godot.Collections.Dictionary dict) continue;
                foreach (var key in dict.Keys)
                {
                    var name = key.AsString();
                    if (dict[key].Obj is Godot.Collections.Dictionary entry && entry.ContainsKey("disc"))
                        _discs[dir + (name.EndsWith(".png") ? name : name + ".png")] = entry["disc"].AsString();
                }
            }
        }
        return _discs.TryGetValue(path, out var d) && d.StartsWith("lighter");
    }

    public override void _Process(double delta)
    {
        _time += delta;
        var info = Info;
        var sig = $"{info.Hp}/{info.MaxHp}{info.Ko}{info.Stealthed}{info.Undead}{info.Summon}{info.Airborne}{info.Mirror}{info.Active}{Highlight}{Size}{info.ArtFile}{info.ArtPath}";
        if (sig != _signature) { _signature = sig; Rebuild(); }
        if (info.Airborne || info.Stealthed)
        {
            Animate();
            if (info.Airborne) _body.QueueRedraw();
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized) Rebuild();
    }

    private void Rebuild()
    {
        var info = Info;
        var s = Math.Min(Size.X, Size.Y);
        PivotOffset = Size / 2;
        Scale = info.Active ? new Vector2(1.1f, 1.1f) : Vector2.One;
        _body.Size = new Vector2(s, s);
        _overlay.Size = new Vector2(s, s);
        _art.Size = _ring.Size = new Vector2(s, s);
        var path = PortraitPath(info);
        _art.Texture = path != null ? Game.Tex(path) : null;
        _art.FlipH = info.Mirror;
        _ring.Texture = Game.Tex(info.Undead ? "res://assets/frames/portrait-ring-undead.png" : info.Summon ? "res://assets/frames/portrait-ring-summon.png" : "res://assets/frames/portrait-ring.png");
        _art.Modulate = info.Ko ? new Color(0.45f, 0.45f, 0.5f) : Colors.White;
        _ring.Modulate = info.Ko ? new Color(0.5f, 0.5f, 0.5f) : Colors.White;
        _body.Modulate = new Color(1, 1, 1, info.Stealthed ? 0.55f : 1f);
        Animate();
        _body.QueueRedraw();
        _overlay.QueueRedraw();
        QueueRedraw();
    }

    private void Animate()
    {
        var info = Info;
        var s = Math.Min(Size.X, Size.Y);
        var lift = info.Airborne ? -s * 0.07f + (float)Math.Sin(_time * 2.2) * s * 0.012f : 0f;
        _body.Position = new Vector2(0, lift + (info.Ko ? s * 0.04f : 0));
        if (info.Stealthed) _body.Modulate = new Color(1, 1, 1, 0.5f + 0.1f * (float)Math.Sin(_time * 3));
    }

    /// <summary>The steady glow of the active unit sits behind the disc.</summary>
    public override void _Draw()
    {
        if (!Info.Active) return;
        var s = Math.Min(Size.X, Size.Y);
        var c = new Vector2(s / 2, s / 2);
        for (var i = 0; i < 5; i++) DrawCircle(c, s * (0.54f - i * 0.02f), new Color(Ui.Gold, 0.12f + i * 0.04f));
    }

    private void DrawDisc(DrawControl c)
    {
        var s = c.Size.X;
        var path = PortraitPath(Info);
        var color = path != null && LighterDisc(path) ? LighterPlum : DarkPlum;
        c.DrawCircle(new Vector2(s / 2, s / 2), s * ArtRadius, Info.Ko ? color.Darkened(0.3f) : color);
        if (!Info.Airborne) return;
        for (var i = 0; i < 3; i++)
        {
            var phase = (float)((_time * 0.6 + i / 3.0) % 1.0);
            var x = s * (0.35f + i * 0.15f);
            var y = s * (1.0f - phase * 0.25f);
            c.DrawLine(new Vector2(x, y), new Vector2(x, y - s * 0.05f), new Color(1, 1, 1, 0.35f * (1 - phase)), Math.Max(1.5f, s * 0.012f));
        }
    }

    private void DrawOverlay(DrawControl c)
    {
        var s = c.Size.X;
        var centre = new Vector2(s / 2, s / 2);
        if (Info.Ko) DrawPortraitCrack(c, s);
        var ringR = s * 0.46f;
        switch (Highlight)
        {
            case CardHighlight.Legal: c.DrawArc(centre, ringR, 0, Mathf.Tau, 48, Ui.Gold, Math.Max(3f, s * 0.035f)); break;
            case CardHighlight.Selected: c.DrawArc(centre, ringR, 0, Mathf.Tau, 48, Colors.White, Math.Max(4f, s * 0.045f)); break;
            case CardHighlight.Hover: c.DrawArc(centre, ringR, 0, Mathf.Tau, 48, new Color(1, 1, 1, 0.7f), Math.Max(3f, s * 0.03f)); break;
            case CardHighlight.Illegal: c.DrawCircle(centre, s * ArtRadius, new Color(0, 0, 0, 0.5f)); break;
        }
        // HP badge at the bottom of the ring.
        var r = Math.Max(11f, s * 0.12f);
        var pos = new Vector2(s / 2, s * 0.9f);
        c.DrawCircle(pos, r, new Color(0.06f, 0.05f, 0.05f, 0.92f));
        var share = Info.MaxHp == 0 ? 0 : Mathf.Clamp((float)Info.Hp / Info.MaxHp, 0, 1);
        var ring = Info.Hp <= 0 ? new Color("777777") : share > 0.5f ? new Color("c0392b") : share > 0.25f ? new Color("d9822b") : new Color("ff5a4a");
        c.DrawArc(pos, r, 0, Mathf.Tau, 24, ring, Math.Max(2f, r * 0.16f));
        var fs = Math.Max(11, (int)(r * 1.25f));
        var text = Info.Hp.ToString();
        var size = Game.BodyBold().GetStringSize(text, HorizontalAlignment.Left, -1, fs);
        c.DrawString(Game.BodyBold(), pos + new Vector2(-size.X / 2, fs * 0.33f), text, HorizontalAlignment.Left, -1, fs, Info.Hp < Info.MaxHp ? new Color("ffb4a8") : Colors.White);
    }

    /// <summary>The KO crack: a thin light line with a few side branches, the same for a given unit.</summary>
    private void DrawPortraitCrack(DrawControl c, float s)
    {
        var rng = new RandomNumberGenerator { Seed = (ulong)Math.Abs(Info.Seed) + 7 };
        var p = new Vector2(s * (0.35f + rng.Randf() * 0.3f), s * 0.1f);
        var light = new Color(0.95f, 0.95f, 0.9f, 0.9f);
        var width = Math.Max(1.5f, s * 0.012f);
        for (var i = 0; i < 8; i++)
        {
            var next = p + new Vector2((rng.Randf() - 0.5f) * s * 0.2f, s * 0.1f);
            c.DrawLine(p, next, light, width);
            if (rng.Randf() < 0.6f) c.DrawLine(next, next + new Vector2((rng.Randf() - 0.5f) * s * 0.35f, s * 0.05f), light, width * 0.6f);
            p = next;
        }
    }
}
