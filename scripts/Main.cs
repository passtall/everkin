using System;
using System.Linq;
using Everkin.App;
using Godot;

namespace Everkin;

/// <summary>A screen that knows where "back" leads: Esc (and the Android back button) calls it. Every screen except the title has one.</summary>
public interface IBackNavigable
{
    /// <summary>Goes back one step. Returns false when there is nothing to go back to.</summary>
    bool GoBack();
}

/// <summary>
/// The root of the game. It owns the current screen. Command line (after a lone "--"): --screen=&lt;name&gt; opens a screen directly, and
/// --shot=&lt;file.png&gt; saves a screenshot after a moment and quits (used to check the UI without a person watching).
/// </summary>
public partial class Main : Control
{
    private Control? _screen;
    private string? _shot;
    private double _shotAt = 1.5;
    private double _time;
    private Vector2? _click;

    public override void _Ready()
    {
        Game.Load();
        Ui.Fill(this);
        var args = OS.GetCmdlineUserArgs().Select(a => a.TrimStart('-')).ToArray();
        string? Arg(string name) => args.FirstOrDefault(a => a.StartsWith(name + "="))?.Substring(name.Length + 1);
        _shot = Arg("shot");
        if (Arg("click") is { } click && click.Split(',') is { Length: 2 } xy && float.TryParse(xy[0], out var cx) && float.TryParse(xy[1], out var cy)) _click = new Vector2(cx, cy);
        if (double.TryParse(Arg("wait"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var wait)) _shotAt = wait;
        var screen = Arg("screen") ?? "menu";
        Show(screen, args);
    }

    public override void _UnhandledKeyInput(InputEvent e)
    {
        if (e is InputEventKey { Pressed: true, Keycode: Key.Escape } && _screen is IBackNavigable nav && nav.GoBack())
            GetViewport().SetInputAsHandled();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMGoBackRequest && _screen is IBackNavigable nav) nav.GoBack();
    }

    public override void _Process(double delta)
    {
        _time += delta;
        if (_click is { } at && _time > 1.0)
        {
            // --click=x,y presses the left mouse button there once (to check what a click reaches without a person).
            _click = null;
            foreach (var pressed in new[] { true, false })
                Input.ParseInputEvent(new InputEventMouseButton { Position = at, GlobalPosition = at, ButtonIndex = MouseButton.Left, Pressed = pressed });
        }
        if (_shot == null) return;
        if (_time < _shotAt) return;
        var image = GetViewport().GetTexture().GetImage();
        image.SavePng(_shot);
        GD.Print($"screenshot saved: {_shot}");
        GetTree().Quit();
    }

    /// <summary>The setup of the last battle the player started (Retry plays it again).</summary>
    public Everkin.Core.Setup.BattleSpec? LastSpec { get; set; }

    public string LastLevel { get; set; } = "normal";

    public string? LastEncounter { get; set; }

    /// <summary>Starts a battle against the AI for the player (side A) and remembers its setup.</summary>
    public void StartBattle(Everkin.Core.Setup.BattleSpec spec, string level, string? encounter = null)
    {
        LastSpec = spec;
        LastLevel = level;
        LastEncounter = encounter;
        Open(new PlayScreen(this, spec, level, encounter));
    }

    /// <summary>Replaces the current screen with one that has been made already.</summary>
    public void Open(Control screen)
    {
        _screen?.QueueFree();
        _screen = screen;
        AddChild(_screen);
        Ui.Fill(_screen);
    }

    /// <summary>Replaces the current screen.</summary>
    public void Show(string name, string[]? args = null)
    {
        _screen?.QueueFree();
        _screen = Screens.Create(this, name, args ?? Array.Empty<string>());
        AddChild(_screen);
        Ui.Fill(_screen);
    }
}
