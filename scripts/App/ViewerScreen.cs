using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Setup;
using Godot;

namespace Everkin.App;

/// <summary>
/// The battle viewer (M3.4, production section 2.3): AI against AI on the card UI as a debug mode. Pause, step one action, speed 1x / 2x /
/// instant, inspect any unit (right-click), start a new battle with the same setup as a recorded one. It shows only what a player would see.
/// </summary>
public partial class ViewerScreen : Control, IBackNavigable
{
    private Main? _main;

    public ViewerScreen WithMain(Main main) { _main = main; return this; }

    public bool GoBack() { if (_main == null) return false; _generation++; _main.Show("menu"); return true; }

    private readonly BattleBoard _board = new();
    private readonly Label _status = Ui.Label("", 20);
    private readonly Button _pauseButton = Ui.Button("Pause", 20);
    private readonly Random _seeds = new();
    private BattleState _state = null!;
    private AiSearch _aiA = null!, _aiB = null!;
    private bool _paused;
    private bool _stepOnce;
    private int _generation;
    private InspectionPanel? _inspection;
    private readonly string _levelName;
    private readonly ulong? _firstSeed;

    public ViewerScreen(string level = "harness", ulong? seed = null)
    {
        _levelName = level;
        _firstSeed = seed;
    }

    public override void _Ready()
    {
        _board.TopMargin = 96;
        _board.BottomMargin = 40;
        AddChild(_board);
        Ui.Fill(_board);
        _board.CardRightClicked += id => Inspect(id);

        var bar = new HBoxContainer();
        bar.AddThemeConstantOverride("separation", 8);
        var back = Ui.Button("Back", 20);
        back.Pressed += () => GoBack();
        bar.AddChild(back);
        _pauseButton.Pressed += () => { _paused = !_paused; _pauseButton.Text = _paused ? "Play" : "Pause"; };
        bar.AddChild(_pauseButton);
        var step = Ui.Button("Step", 20);
        step.Pressed += () => { _paused = true; _pauseButton.Text = "Play"; _stepOnce = true; };
        bar.AddChild(step);
        foreach (var (text, speed) in new[] { ("1x", 1f), ("2x", 2f), ("Instant", 0f) })
        {
            var b = Ui.Button(text, 20);
            b.Pressed += () => _board.Speed = speed;
            bar.AddChild(b);
        }
        var fresh = Ui.Button("New battle", 20);
        fresh.Pressed += () => Start(BattleSetup.RandomSpec(Game.Content, Game.Trees, (ulong)_seeds.NextInt64(1, 1_000_000)));
        bar.AddChild(fresh);
        var runs = RecordedRuns();
        if (runs.Count > 0)
        {
            var pick = new OptionButton();
            pick.AddThemeFontSizeOverride("font_size", 18);
            pick.AddItem("Same setup as a recorded battle…");
            foreach (var r in runs) pick.AddItem(r);
            pick.ItemSelected += i => { if (i > 0) StartFromRecord(runs[(int)i - 1]); };
            bar.AddChild(pick);
        }
        bar.AddChild(_status);
        var panel = Ui.Panel(bar);
        panel.Position = new Vector2(20, 8);
        AddChild(panel);

        var view = Ui.Button("View", 20);
        view.Pressed += () => { _board.OneSide = !_board.OneSide; _board.Layout(false); };
        var top = Ui.Panel(view);
        top.SetAnchorsPreset(LayoutPreset.TopRight);
        top.Position = new Vector2(-140, 8);
        AddChild(top);

        var spec = BattleSetup.RandomSpec(Game.Content, Game.Trees, _firstSeed ?? (ulong)_seeds.NextInt64(1, 1_000_000));
        Start(spec);
    }

    private static List<string> RecordedRuns()
    {
        var root = ProjectSettings.GlobalizePath("res://runs");
        return Directory.Exists(root) ? Directory.GetDirectories(root).Select(Path.GetFileName).Where(n => n != null).Select(n => n!).OrderByDescending(n => n).Take(10).ToList() : new();
    }

    private sealed class RecUnit { public string Creature { get; set; } = ""; public string Side { get; set; } = ""; public string Row { get; set; } = ""; public List<string> Build { get; set; } = new(); }
    private sealed class Rec { public ulong Seed { get; set; } public List<RecUnit> Units { get; set; } = new(); }

    /// <summary>Starts a new battle with the teams and formations of a recorded battle from a run (it need not play out the same way).</summary>
    private void StartFromRecord(string runId)
    {
        var path = ProjectSettings.GlobalizePath($"res://runs/{runId}/battles.jsonl");
        if (!File.Exists(path)) return;
        var lines = File.ReadLines(path).Where(l => l.Length > 2).Take(200).ToList();
        var rec = JsonSerializer.Deserialize<Rec>(lines[_seeds.Next(lines.Count)])!;
        if (rec.Units.Count == 0) return;
        TeamSetup Team(string side) => new(rec.Units.Where(u => u.Side == side).Select(u => new UnitSetup(u.Creature, u.Build, Enum.Parse<Row>(u.Row))).ToList());
        Start(new BattleSpec((ulong)_seeds.NextInt64(1, 1_000_000), Team("A"), Team("B")));
    }

    private void Start(BattleSpec spec)
    {
        _generation++;
        _state = BattleSetup.Create(spec, Game.Catalog);
        var level = Game.Settings.Level(_levelName);
        _aiA = new AiSearch(Game.Catalog, level, new BattleRandom(spec.Seed * 2 + 1));
        _aiB = new AiSearch(Game.Catalog, level, new BattleRandom(spec.Seed * 2 + 2));
        _board.Setup(_state, Side.A);
        _ = RunAsync(_generation);
    }

    private async Task RunAsync(int generation)
    {
        while (generation == _generation && !_state.IsOver)
        {
            while (_paused && !_stepOnce && generation == _generation) await ToSignal(GetTree().CreateTimer(0.05), SceneTreeTimer.SignalName.Timeout);
            if (generation != _generation) return;
            _stepOnce = false;

            var unitId = BattleDriver.NextTurn(_state, Game.Catalog);
            await _board.PlayAsync(_state.DrainEvents());
            if (_state.IsOver || unitId is not { } id) break;

            var ai = _state.GetUnit(id).Side == Side.A ? _aiA : _aiB;
            var snapshot = _state;
            var action = await Task.Run(() => ai.Choose(snapshot, id));
            if (generation != _generation) return;
            var events = action.Apply(_state, Game.Catalog, id);
            if (BattleEnd.CheckTurnCap(_state)) events = events.Concat(_state.DrainEvents()).ToList();
            await _board.PlayAsync(events);
            _status.Text = $"turn {_state.TurnsTaken}   time {_state.Timeline.Now}";
        }
        if (generation == _generation)
            _status.Text = $"{_state.Outcome} ({_state.EndReason}) after {_state.TurnsTaken} turns";
    }

    private void Inspect(int unitId)
    {
        _inspection?.QueueFree();
        _inspection = InspectionPanel.Open(this, _state, unitId);
    }
}
