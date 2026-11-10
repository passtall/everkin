using System;
using System.Linq;
using Godot;

namespace Everkin.App;

/// <summary>Creates screens by name.</summary>
public static class Screens
{
    public static Control Create(Main main, string name, string[] args)
    {
        string? Arg(string key) => args.FirstOrDefault(a => a.StartsWith(key + "="))?.Substring(key.Length + 1);
        switch (name)
        {
            case "cards": return new CardsDemo();
            case "viewer":
                return new ViewerScreen(Arg("level") ?? "harness", ulong.TryParse(Arg("seed"), out var seed) ? seed : null);
            default: return new MenuScreen(main);
        }
    }
}

/// <summary>The main menu: where the game's parts are reached from.</summary>
public partial class MenuScreen : Control
{
    private readonly Main _main;

    public MenuScreen(Main main) => _main = main;

    public override void _Ready()
    {
        var bg = new TextureRect { Texture = Game.Tex("res://assets/ui/menu-background.png"), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered };
        AddChild(bg);
        Ui.Fill(bg);
        var box = new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        box.AddThemeConstantOverride("separation", 14);
        if (Game.Tex("res://assets/brand/wordmark.png") is { } word)
            box.AddChild(new TextureRect { Texture = word, ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional, CustomMinimumSize = new Vector2(520, 160), StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered });
        void Item(string text, string screen)
        {
            var b = Ui.Button(text, 28);
            b.CustomMinimumSize = new Vector2(420, 64);
            b.Pressed += () => _main.Show(screen);
            box.AddChild(b);
        }
        Item("Free battle", "free");
        Item("Team editor", "teams");
        Item("Battle viewer (AI vs AI)", "viewer");
        Item("Cards", "cards");
        var center = new CenterContainer();
        center.AddChild(Ui.Panel(box));
        AddChild(center);
        Ui.Fill(center);
    }
}

/// <summary>A sheet of cards in every mode, for checking the card look (M3.1).</summary>
public partial class CardsDemo : Control
{
    public override void _Ready()
    {
        var bg = new ColorRect { Color = new Color("0d0a08") };
        AddChild(bg);
        Ui.Fill(bg);
        var defs = Game.Content.Creatures.Skip(30).Take(3).ToList();
        var x = 20f;
        foreach (var def in defs)
        {
            var info = CardInfo.FromCreature(def, Game.CardSkills(new[] { "brawl_strike", "heavy_blow" }));
            foreach (var (mode, w) in new[] { (CardMode.Front, 300f), (CardMode.Back, 300f), (CardMode.Compact, 96f) })
            {
                var card = new CardView { Mode = mode, Position = new Vector2(x, 20), Size = new Vector2(w, w * 1.5f) };
                card.Bind(info);
                AddChild(card);
                x += w + 10;
            }
        }
    }
}
