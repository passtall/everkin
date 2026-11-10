using System;
using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Content;
using Godot;

namespace Everkin.App;

public enum CardMode { Front, Back, Compact }

public enum CardHighlight { None, Legal, Illegal, Selected, Hover }

/// <summary>A skill row on a card back.</summary>
public sealed record CardSkill(string Name, string Line, int Delay, int Momentum);

/// <summary>Everything a card shows (ui_spec "Required information hierarchy"). Built from a roster creature or from a unit in a battle.</summary>
public sealed class CardInfo
{
    public string Name = "";
    public string ClassLine = "";
    public string Biome = "ruins";
    public string ArtFile = "";
    public int[] FrontFrame = { 0, 0, 1024, 1536 };
    public int[] RearFrame = { 0, 0, 1024, 1536 };
    public int Hp, MaxHp, Speed, Power, Cost;
    public int BaseSpeed, BasePower;
    public List<string> Statuses = new();
    public bool Ko, Stealthed, Summon, Undead;
    public bool ShowCost = true;
    public int Seed;
    public List<CardSkill> Skills = new();
    public string? ArtPath;

    public static CardInfo FromCreature(CreatureDef def, IEnumerable<CardSkill>? skills = null)
    {
        var attackType = Game.Content.AttackTypes.FirstOrDefault(a => a.Id == def.AttackType)?.Name ?? def.AttackType;
        var professions = def.Professions.Select(p => Game.Content.Professions.FirstOrDefault(x => x.Id == p)?.Name ?? p);
        return new CardInfo
        {
            Name = def.Name,
            ClassLine = string.Join(" · ", professions.Prepend(attackType)),
            Biome = def.Biome,
            ArtFile = def.ArtFile,
            FrontFrame = def.FrontFrame.ToArray(),
            RearFrame = def.RearFrame.ToArray(),
            Hp = def.Hp, MaxHp = def.Hp, Speed = def.Speed, Power = def.Power, Cost = def.Cost,
            BaseSpeed = def.Speed, BasePower = def.Power,
            Seed = def.Id.GetHashCode(),
            Skills = skills?.ToList() ?? new List<CardSkill>(),
        };
    }
}

/// <summary>
/// One reusable card (production section 2.3 "Cards"): full front, back, or the compact board card. Everything is drawn at the scale of the
/// card's width against the 1024 x 1536 art canvas, so any size works.
/// </summary>
public partial class CardView : Control
{
    private CardInfo _info = new();
    private CardMode _mode = CardMode.Front;
    private CardHighlight _highlight;
    private Color? _sideColor;

    public event Action<CardView>? Clicked;
    public event Action<CardView>? RightClicked;

    /// <summary>The unit ID or creature ID this card stands for (set by the owner screen).</summary>
    public object? Tag { get; set; }

    public CardInfo Info => _info;

    public CardMode Mode
    {
        get => _mode;
        set { _mode = value; QueueRedraw(); }
    }

    public CardHighlight Highlight
    {
        get => _highlight;
        set { _highlight = value; QueueRedraw(); }
    }

    /// <summary>A thin stripe in the side colour on the left edge (timeline entries, summoner hover).</summary>
    public Color? SideColor
    {
        get => _sideColor;
        set { _sideColor = value; QueueRedraw(); }
    }

    public CardView()
    {
        ClipContents = true;
        MouseFilter = MouseFilterEnum.Stop;
    }

    public void Bind(CardInfo info)
    {
        _info = info;
        QueueRedraw();
    }

    public override void _GuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton { Pressed: true } mb)
        {
            if (mb.ButtonIndex == MouseButton.Left) Clicked?.Invoke(this);
            else if (mb.ButtonIndex == MouseButton.Right) RightClicked?.Invoke(this);
        }
    }

    public override void _Notification(int what)
    {
        if (what == NotificationResized) QueueRedraw();
    }

    // ---------------------------------------------------------------- drawing

    public override void _Draw()
    {
        var s = Size.X / 1024f;
        var info = _info;
        var faceDown = info.Ko || _mode == CardMode.Back;
        var sink = info.Ko ? 18f * s : 0f;
        var biome = info.Undead ? "undead" : info.Biome;

        if (Game.Tex($"res://assets/backgrounds/card/{info.Biome}.webp") is { } bg)
            DrawTextureRect(bg, new Rect2(0, 0, Size.X, Size.Y), false);
        else DrawRect(new Rect2(Vector2.Zero, Size), new Color("2a2118"));

        var art = info.ArtPath != null ? Game.Tex(info.ArtPath) : Game.Tex($"res://assets/creatures/{info.ArtFile}");
        if (art != null)
        {
            var f = faceDown ? info.RearFrame : info.FrontFrame;
            var k = 1024f / Math.Max(1, f[2]);
            var tint = faceDown ? new Color(0.42f, 0.42f, 0.46f) : info.Stealthed ? new Color(1, 1, 1, 0.55f) : Colors.White;
            DrawTextureRect(art, new Rect2(-f[0] * k * s, -f[1] * k * s + sink, 1024 * k * s, 1536 * k * s), false, tint);
        }

        if (Game.Tex($"res://assets/frames/{biome}.png") is { } frame)
            DrawTextureRect(frame, new Rect2(0, 0, Size.X, Size.Y), false);

        if (info.Ko) DrawCrack(s);
        else if (_mode == CardMode.Back) DrawBackList(s);

        if (!faceDown)
        {
            DrawGems(s, _mode == CardMode.Compact);
            if (_mode == CardMode.Compact) DrawCompactBottom(s);
            else DrawPlate(s);
        }
        else if (info.Ko || _mode == CardMode.Back) DrawGems(s, _mode == CardMode.Compact, dim: true);

        if (info.Summon && Game.Tex("res://assets/icons/ui/marker-summon.png") is { } sm) DrawTextureRect(sm, new Rect2(Size.X * 0.36f, 4, Size.X * 0.28f, Size.X * 0.28f), false);
        if (info.Undead && Game.Tex("res://assets/icons/ui/marker-undead.png") is { } um) DrawTextureRect(um, new Rect2(Size.X * 0.36f, 4, Size.X * 0.28f, Size.X * 0.28f), false);
        if (info.Stealthed && Game.Tex("res://assets/icons/ui/status-stealth.png") is { } st) DrawTextureRect(st, new Rect2(Size.X * 0.36f, Size.Y * 0.12f, Size.X * 0.28f, Size.X * 0.28f), false);

        if (_sideColor is { } sc) DrawRect(new Rect2(0, 0, Math.Max(4, Size.X * 0.04f), Size.Y), sc);
        DrawHighlight();
    }

    private void DrawHighlight()
    {
        switch (_highlight)
        {
            case CardHighlight.Legal: DrawRect(new Rect2(1, 1, Size.X - 2, Size.Y - 2), new Color(Ui.Gold, 1), false, 4); break;
            case CardHighlight.Selected: DrawRect(new Rect2(1, 1, Size.X - 2, Size.Y - 2), Colors.White, false, 5); break;
            case CardHighlight.Hover: DrawRect(new Rect2(1, 1, Size.X - 2, Size.Y - 2), new Color(1, 1, 1, 0.75f), false, 3); break;
            case CardHighlight.Illegal: DrawRect(new Rect2(Vector2.Zero, Size), new Color(0, 0, 0, 0.5f)); break;
        }
    }

    private void DrawGems(float s, bool compact, bool dim = false)
    {
        var r = compact ? Math.Max(13f, 66f * s) : 66f * s;
        var font = Game.BodyBold();
        var fs = compact ? Math.Max(20, (int)(r * 1.3f)) : (int)(r * 1.25f);
        var info = _info;
        (Vector2 Pos, Color Ring, string Text, Color Text2)[] gems =
        {
            (new Vector2(133, 152) * s, new Color("c0392b"), info.Hp.ToString(), info.Hp < info.MaxHp ? new Color("ffb4a8") : Colors.White),
            (new Vector2(891, 152) * s, new Color("d9822b"), info.Power.ToString(), info.Power != info.BasePower ? new Color("fff0a0") : Colors.White),
            (new Vector2(120, 1336) * s, new Color("2e86c1"), info.Speed.ToString(), info.Speed != info.BaseSpeed ? new Color("fff0a0") : Colors.White),
            (new Vector2(910, 1336) * s, new Color("b8962e"), info.ShowCost ? info.Cost.ToString() : "–", Colors.White),
        };
        if (compact)
        {
            // Keep the four gems inside the card at small sizes.
            gems[0].Pos = new Vector2(Math.Max(gems[0].Pos.X, r + 2), Math.Max(gems[0].Pos.Y, r + 2));
            gems[1].Pos = new Vector2(Math.Min(gems[1].Pos.X, Size.X - r - 2), Math.Max(gems[1].Pos.Y, r + 2));
            gems[2].Pos = new Vector2(Math.Max(gems[2].Pos.X, r + 2), Math.Min(gems[2].Pos.Y, Size.Y - r - 2));
            gems[3].Pos = new Vector2(Math.Min(gems[3].Pos.X, Size.X - r - 2), Math.Min(gems[3].Pos.Y, Size.Y - r - 2));
        }
        foreach (var (pos, ring, text, color) in gems)
        {
            DrawCircle(pos, r, new Color(0.06f, 0.05f, 0.05f, dim ? 0.55f : 0.9f));
            DrawArc(pos, r, 0, Mathf.Tau, 24, dim ? new Color(ring, 0.5f) : ring, Math.Max(2f, r * 0.14f));
            var size = font.GetStringSize(text, HorizontalAlignment.Left, -1, fs);
            DrawString(font, pos + new Vector2(-size.X / 2, fs * 0.33f), text, HorizontalAlignment.Left, -1, fs, dim ? new Color(color, 0.6f) : color);
        }
    }

    private void DrawPlate(float s)
    {
        var plate = new Rect2(250 * s, 1226 * s, 524 * s, 184 * s);
        DrawRect(plate, new Color(0.05f, 0.04f, 0.04f, 0.82f));
        DrawRect(plate, new Color(Ui.Gold, 0.8f), false, Math.Max(1.5f, 4 * s));
        var heading = Game.Heading();
        var fs = Math.Max(10, (int)(58 * s));
        DrawString(heading, new Vector2(plate.Position.X, plate.Position.Y + 70 * s), _info.Name, HorizontalAlignment.Center, plate.Size.X - 16 * s, fs, Ui.Ink);
        var small = Math.Max(8, (int)(34 * s));
        DrawString(Game.Body(), new Vector2(plate.Position.X, plate.Position.Y + 130 * s), _info.ClassLine, HorizontalAlignment.Center, plate.Size.X - 16 * s, small, new Color("cdbd9a"));
    }

    private void DrawCompactBottom(float s)
    {
        var barH = Math.Max(8f, 70f * s);
        var bar = new Rect2(Size.X * 0.2f, Size.Y - barH - Size.Y * 0.045f, Size.X * 0.6f, barH);
        DrawRect(bar, new Color(0.05f, 0.04f, 0.04f, 0.85f));
        var share = _info.MaxHp == 0 ? 0 : Mathf.Clamp((float)_info.Hp / _info.MaxHp, 0, 1);
        var fill = share > 0.5f ? new Color("5fbf5f") : share > 0.25f ? new Color("d9b43a") : new Color("d9503a");
        DrawRect(new Rect2(bar.Position, new Vector2(bar.Size.X * share, bar.Size.Y)), fill);
        DrawRect(bar, new Color(Ui.Gold, 0.9f), false, 1.5f);

        var icon = Math.Max(16f, Size.X * 0.2f);
        var shown = _info.Statuses.Take(4).ToList();
        for (var i = 0; i < shown.Count; i++) DrawStatus(shown[i], new Rect2(Size.X * 0.14f + i * (icon + 1), Size.Y * 0.2f, icon, icon));
        if (_info.Statuses.Count > 4)
            DrawString(Game.BodyBold(), new Vector2(Size.X * 0.14f + 4 * (icon + 1), Size.Y * 0.2f + icon), $"+{_info.Statuses.Count - 4}", HorizontalAlignment.Left, -1, 18, Ui.Ink);
    }

    private void DrawStatus(string id, Rect2 rect)
    {
        var icon = Game.Tex($"res://assets/icons/ui/status-{id.Replace('_', '-')}.png");
        if (icon != null) DrawTextureRect(icon, rect, false);
        else
        {
            DrawRect(rect, new Color(0.1f, 0.08f, 0.07f, 0.9f));
            DrawRect(rect, Ui.Gold, false, 1);
        }
        var letter = id.Length > 0 ? char.ToUpper(id[0]).ToString() : "?";
        DrawString(Game.BodyBold(), rect.Position + new Vector2(rect.Size.X * 0.2f, rect.Size.Y * 0.85f), letter, HorizontalAlignment.Left, -1, (int)(rect.Size.Y * 0.8f), Colors.White);
    }

    private void DrawBackList(float s)
    {
        var top = 260 * s;
        var row = 118 * s;
        var fs = Math.Max(9, (int)(44 * s));
        var small = Math.Max(8, (int)(28 * s));
        var shown = _info.Skills.Take(9).ToList();
        for (var i = 0; i < shown.Count; i++)
        {
            var y = top + i * row;
            var rect = new Rect2(190 * s, y, 644 * s, row - 10 * s);
            DrawRect(rect, new Color(0.04f, 0.035f, 0.03f, 0.78f));
            DrawRect(rect, new Color(Ui.Bronze, 0.9f), false, Math.Max(1, 3 * s));
            DrawString(Game.BodyBold(), new Vector2(rect.Position.X + 14 * s, rect.Position.Y + 48 * s), shown[i].Name, HorizontalAlignment.Left, rect.Size.X - 190 * s, fs, Ui.Ink);
            DrawString(Game.Body(), new Vector2(rect.Position.X + 14 * s, rect.Position.Y + 88 * s), shown[i].Line, HorizontalAlignment.Left, rect.Size.X - 190 * s, small, new Color("cdbd9a"));
            DrawMiniGem(new Vector2(rect.End.X - 120 * s, rect.Position.Y + rect.Size.Y / 2), 30 * s, new Color("2e86c1"), shown[i].Delay.ToString());
            DrawMiniGem(new Vector2(rect.End.X - 46 * s, rect.Position.Y + rect.Size.Y / 2), 30 * s, new Color("b8962e"), shown[i].Momentum.ToString());
        }
    }

    private void DrawMiniGem(Vector2 pos, float r, Color ring, string text)
    {
        r = Math.Max(r, 9);
        DrawCircle(pos, r, new Color(0.06f, 0.05f, 0.05f, 0.9f));
        DrawArc(pos, r, 0, Mathf.Tau, 20, ring, Math.Max(1.5f, r * 0.15f));
        var fs = (int)(r * 1.2f);
        var size = Game.BodyBold().GetStringSize(text, HorizontalAlignment.Left, -1, fs);
        DrawString(Game.BodyBold(), pos + new Vector2(-size.X / 2, fs * 0.33f), text, HorizontalAlignment.Left, -1, fs, Colors.White);
    }

    /// <summary>The KO crack: a thin light line with a few side branches (ui_spec "KO and hit feedback"), the same for a given card.</summary>
    private void DrawCrack(float s)
    {
        var rng = new RandomNumberGenerator { Seed = (ulong)Math.Abs(_info.Seed) + 7 };
        var p = new Vector2(Size.X * (0.35f + rng.Randf() * 0.3f), Size.Y * 0.12f);
        var light = new Color(0.95f, 0.95f, 0.9f, 0.9f);
        var width = Math.Max(1.5f, 6 * s);
        for (var i = 0; i < 9; i++)
        {
            var next = p + new Vector2((rng.Randf() - 0.5f) * Size.X * 0.22f, Size.Y * 0.095f);
            DrawLine(p, next, light, width);
            if (rng.Randf() < 0.6f)
            {
                var side = next + new Vector2((rng.Randf() - 0.5f) * Size.X * 0.4f, Size.Y * 0.05f);
                DrawLine(next, side, light, width * 0.6f);
            }
            p = next;
        }
    }
}
