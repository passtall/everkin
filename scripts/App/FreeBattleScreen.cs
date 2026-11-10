using System;
using System.Linq;
using Everkin.Core.Battle;
using Everkin.Core.Setup;
using Godot;

namespace Everkin.App;

/// <summary>Free battle setup (M4.4): pick a team, a difficulty and an enemy team (random, within the budget), then fight.</summary>
public partial class FreeBattleScreen : Control
{
    private readonly Main _main;
    private readonly Random _random = new();
    private string _level = "normal";
    private TeamData? _team;

    public FreeBattleScreen(Main main) => _main = main;

    public override void _Ready()
    {
        var bg = new ColorRect { Color = new Color("0d0a08") };
        AddChild(bg);
        Ui.Fill(bg);
        var box = new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        box.AddThemeConstantOverride("separation", 16);
        box.AddChild(Ui.Label("Free battle", 44, Ui.Gold, bold: true, align: HorizontalAlignment.Center));

        box.AddChild(Ui.Label("Your team", 24, Ui.Ink, bold: true));
        var teams = new HBoxContainer();
        teams.AddThemeConstantOverride("separation", 8);
        _team = Profile.CurrentTeam;
        foreach (var t in Profile.Data.Teams)
        {
            var b = Ui.Button((t.Units.Count < BattleSetup.TeamSize ? "⚠ " : "") + t.Name + $"  ({t.Units.Count})", 22);
            var team = t;
            b.Pressed += () => { _team = team; Profile.Data.Selected = Profile.Data.Teams.IndexOf(team); Profile.Save(); Rebuild(); };
            if (t == _team) b.AddThemeStyleboxOverride("normal", Ui.Box(new Color("4a3a24"), Ui.Gold, 3));
            teams.AddChild(b);
        }
        box.AddChild(teams);

        box.AddChild(Ui.Label("Difficulty", 24, Ui.Ink, bold: true));
        var levels = new HBoxContainer();
        levels.AddThemeConstantOverride("separation", 8);
        foreach (var (id, name) in new[] { ("easy", "Easy"), ("normal", "Normal"), ("hard", "Hard") })
        {
            var b = Ui.Button(name, 22);
            var level = id;
            b.Pressed += () => { _level = level; Rebuild(); };
            if (id == _level) b.AddThemeStyleboxOverride("normal", Ui.Box(new Color("4a3a24"), Ui.Gold, 3));
            levels.AddChild(b);
        }
        box.AddChild(levels);

        var start = Ui.Button("Fight a random team", 30);
        start.CustomMinimumSize = new Vector2(420, 70);
        start.Disabled = _team == null || _team.Units.Count == 0;
        start.Pressed += Fight;
        box.AddChild(start);
        if (start.Disabled) box.AddChild(Ui.Label("Put at least one creature in the team first (Team editor).", 20, Ui.Bad, align: HorizontalAlignment.Center));
        var back = Ui.Button("Back", 22);
        back.Pressed += () => _main.Show("menu");
        box.AddChild(back);

        var center = new CenterContainer();
        center.AddChild(Ui.Panel(box));
        AddChild(center);
        Ui.Fill(center);
    }

    private void Rebuild() => _main.Open(new FreeBattleScreen(_main) { _levelPreset = _level });

    private string _levelPreset { set => _level = value; }

    private void Fight()
    {
        if (_team == null) return;
        var seed = (ulong)_random.NextInt64(1, 1_000_000_000);
        var rng = new BattleRandom(seed ^ 0x5EED5E7UL);
        var enemy = BattleSetup.RandomTeam(Game.Content, Game.Trees, rng);
        _main.StartBattle(new BattleSpec(seed, Profile.ToSetup(_team), enemy), _level);
    }
}
