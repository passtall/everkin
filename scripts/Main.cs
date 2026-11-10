using System;
using System.Linq;
using Everkin.App;
using Godot;

namespace Everkin;

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

    public override void _Ready()
    {
        Game.Load();
        Ui.Fill(this);
        var args = OS.GetCmdlineUserArgs().Select(a => a.TrimStart('-')).ToArray();
        string? Arg(string name) => args.FirstOrDefault(a => a.StartsWith(name + "="))?.Substring(name.Length + 1);
        _shot = Arg("shot");
        if (double.TryParse(Arg("wait"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var wait)) _shotAt = wait;
        var screen = Arg("screen") ?? "menu";
        Show(screen, args);
    }

    public override void _Process(double delta)
    {
        if (_shot == null) return;
        _time += delta;
        if (_time < _shotAt) return;
        var image = GetViewport().GetTexture().GetImage();
        image.SavePng(_shot);
        GD.Print($"screenshot saved: {_shot}");
        GetTree().Quit();
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
