using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Setup;
using Everkin.Core.Skills;
using Godot;

namespace Everkin.App;

/// <summary>
/// A battle in which the player controls side A against the AI (M4.1): the action bar, skill then target selection with previews, Move, Skip
/// Turn, surrender, and the result overlay. Side B and every summon play themselves.
/// </summary>
public partial class PlayScreen : Control
{
    private readonly Main _main;
    private readonly BattleSpec _spec;
    private readonly string _level;
    private readonly BattleBoard _board = new();
    private readonly ActionBar _bar = new();
    private readonly PanelContainer _previewPanel = new();
    private readonly VBoxContainer _previewBox = new();
    private readonly Label _hint = Ui.Label("", 22, Ui.Gold);
    private BattleState _state = null!;
    private AiSearch _ai = null!;
    private TaskCompletionSource<AiAction>? _choice;
    private int _active = -1;
    private string? _selected;
    private SkillUse? _partial;                       // the first part of a use that needs several clicks
    private bool _resolving;
    private bool _surrendered;
    private InspectionPanel? _inspection;
    private Control? _result;

    public PlayScreen(Main main, BattleSpec spec, string level)
    {
        _main = main;
        _spec = spec;
        _level = level;
    }

    public override void _Ready()
    {
        if (Instant) _board.Speed = 0;
        _board.TopMargin = 20;
        _board.BottomMargin = 290;
        AddChild(_board);
        Ui.Fill(_board);
        _board.CardClicked += OnCardClicked;
        _board.CardRightClicked += id => Inspect(id);
        _board.GapClicked += OnGapClicked;
        _board.HoverChanged += OnHover;

        _bar.SetAnchorsPreset(LayoutPreset.CenterBottom);
        _bar.GrowHorizontal = GrowDirection.Both;
        _bar.GrowVertical = GrowDirection.Begin;
        _bar.Position = new Vector2(0, -8);
        _bar.SlotPressed += OnSlot;
        AddChild(_bar);

        _previewPanel.AddThemeStyleboxOverride("panel", Ui.Box(new Color(Ui.PanelDark, 0.95f), Ui.Gold));
        _previewPanel.AddChild(_previewBox);
        _previewPanel.Visible = false;
        _previewPanel.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(_previewPanel);

        _hint.SetAnchorsPreset(LayoutPreset.CenterTop);
        _hint.Position = new Vector2(-300, 12);
        AddChild(_hint);

        var top = new HBoxContainer();
        top.AddThemeConstantOverride("separation", 8);
        var view = Ui.Button("View", 20);
        view.Pressed += () => { _board.OneSide = !_board.OneSide; _board.Layout(false); };
        var surrender = Ui.Button("Surrender", 20);
        surrender.Pressed += ConfirmSurrender;
        top.AddChild(view);
        top.AddChild(surrender);
        var holder = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.End, MouseFilter = MouseFilterEnum.Ignore };
        holder.SetAnchorsAndOffsetsPreset(LayoutPreset.TopWide);
        holder.OffsetTop = 8;
        holder.OffsetRight = -20;
        holder.AddChild(Ui.Panel(top));
        AddChild(holder);

        Start();
        if (AutoSurrender)
            GetTree().CreateTimer(2.5).Timeout += () => { _surrendered = true; BattleEnd.Surrender(_state, Side.A); _choice?.TrySetResult(AiAction.Skip); };
    }

    private void Start()
    {
        _state = BattleSetup.Create(_spec, Game.Catalog);
        _ai = new AiSearch(Game.Catalog, Game.Settings.Level(_level), new BattleRandom(_spec.Seed * 2 + 2));
        _board.Setup(_state, Side.A);
        _ = RunAsync();
    }

    // ---------------------------------------------------------------- the battle loop

    private async Task RunAsync()
    {
        while (!_state.IsOver && !_surrendered)
        {
            var unitId = BattleDriver.NextTurn(_state, Game.Catalog);
            await _board.PlayAsync(_state.DrainEvents());
            if (_state.IsOver || unitId is not { } id) break;

            AiAction action;
            if (_state.GetUnit(id).Side == Side.A) action = await AskPlayer(id);
            else
            {
                var snapshot = _state;
                action = await Task.Run(() => _ai.Choose(snapshot, id));
            }
            if (_surrendered) break;
            _resolving = true;
            var events = action.Apply(_state, Game.Catalog, id);
            if (BattleEnd.CheckTurnCap(_state)) events = events.Concat(_state.DrainEvents()).ToList();
            await _board.PlayAsync(events);
            _resolving = false;
        }
        ShowResult();
    }

    private Task<AiAction> AskPlayer(int unitId)
    {
        _active = unitId;
        _selected = null;
        _partial = null;
        _choice = new TaskCompletionSource<AiAction>();
        _board.ClearHighlights();
        _board.SetHighlight(unitId, CardHighlight.Selected);
        _bar.Fill(_state, unitId);
        _hint.Text = $"{_state.GetUnit(unitId).Name}'s turn";
        if (SelfTest) _ = SelfTestStep(unitId);
        return _choice.Task;
    }

    private void Choose(AiAction action)
    {
        _bar.Clear();
        _board.ClearHighlights();
        _previewPanel.Visible = false;
        _board.SetGhost(null);
        _hint.Text = "";
        _active = -1;
        _choice?.TrySetResult(action);
    }

    /// <summary>Plays the player's turns by clicking through the UI code (for checking the screens without a person): set with --selftest.</summary>
    public bool SelfTest { get; set; }

    /// <summary>Plays animations without waiting (with --instant).</summary>
    public bool Instant { get; set; }

    /// <summary>Surrenders after a moment (with --surrender), to see the result overlay.</summary>
    public bool AutoSurrender { get; set; }

    private int _selfTestTurn;

    private async Task SelfTestStep(int unitId)
    {
        await ToSignal(GetTree().CreateTimer(Instant ? 0.02 : 0.4), SceneTreeTimer.SignalName.Timeout);
        _selfTestTurn++;
        var slots = _bar.Slots.Where(s => s.Enabled && !s.Id.StartsWith('@')).ToList();
        if (slots.Count == 0) { OnSlot(_bar.Slots.First(s => s.Id == "@skip")); await Release(); return; }
        var slot = slots[_selfTestTurn % slots.Count];
        OnSlot(slot);
        await ToSignal(GetTree().CreateTimer(0.3), SceneTreeTimer.SignalName.Timeout);
        var skill = Game.Catalog.Get(slot.Id);
        if (skill.Target == TargetKind.None) { OnSlot(slot); return; }
        var legal = _state.Units.Where(u => _board.CardOf(u.Id)?.Highlight == CardHighlight.Legal).Select(u => u.Id).ToList();
        if (legal.Count > 0)
        {
            OnHover(legal[0]);
            await ToSignal(GetTree().CreateTimer(0.3), SceneTreeTimer.SignalName.Timeout);
            OnCardClicked(legal[0]);
            if (_partial != null)
            {
                await ToSignal(GetTree().CreateTimer(0.2), SceneTreeTimer.SignalName.Timeout);
                var gap = _board.GapCount > 0 ? _board.FirstGap : null;
                var second = _state.Units.Where(u => u.Id != legal[0] && _board.CardOf(u.Id)?.Highlight == CardHighlight.Legal).Select(u => (int?)u.Id).FirstOrDefault();
                if (gap is { } g) OnGapClicked(g);
                else if (second is { } sec) OnCardClicked(sec);
            }
        }
        else if (_board.GapCount > 0 && _board.FirstGap is { } fg) OnGapClicked(fg);
        await Release();
    }

    private async Task Release()
    {
        await ToSignal(GetTree().CreateTimer(0.3), SceneTreeTimer.SignalName.Timeout);
        if (_choice is { Task.IsCompleted: false }) Choose(AiAction.Skip);
    }

    // ---------------------------------------------------------------- selection

    private bool Locked => _choice == null || _choice.Task.IsCompleted || _resolving || _active < 0;

    public override void _UnhandledKeyInput(InputEvent e)
    {
        if (e is not InputEventKey { Pressed: true } k) return;
        if (k.Keycode == Key.Escape) { if (_inspection != null && IsInstanceValid(_inspection)) return; Cancel(); return; }
        if (Locked) return;
        if (k.Keycode == Key.M) OnSlot(_bar.Slots.First(s => s.Id == "@move"));
        else if (k.Keycode == Key.S) OnSlot(_bar.Slots.First(s => s.Id == "@skip"));
        else if (k.Keycode >= Key.Key1 && k.Keycode <= Key.Key9)
        {
            var slot = _bar.Slots.FirstOrDefault(s => s.Key == (int)(k.Keycode - Key.Key0));
            if (slot != null) OnSlot(slot);
        }
        else if (k.Keycode == Key.Enter && _selected != null) OnSlot(_bar.Slots.First(s => s.Id == _selected));
    }

    private void Cancel()
    {
        if (Locked) return;
        _selected = null;
        _partial = null;
        _bar.Select(null);
        _board.ClearHighlights();
        _board.SetHighlight(_active, CardHighlight.Selected);
        _previewPanel.Visible = false;
        _board.SetGhost(null);
        _hint.Text = $"{_state.GetUnit(_active).Name}'s turn";
    }

    private void OnSlot(SkillSlot slot)
    {
        if (Locked) return;
        if (!slot.Enabled) { _hint.Text = slot.DisabledReason ?? ""; return; }
        if (_selected == slot.Id)
        {
            // Selecting the same skill again: a skill that needs no target is used now, otherwise it cancels.
            if (slot.Id == "@skip") { Choose(AiAction.Skip); return; }
            if (slot.Id != "@move" && Game.Catalog.Get(slot.Id) is { } sk && NeedsNothing(sk)) { Choose(new AiAction(AiActionKind.Skill, new SkillUse(_active, slot.Id))); return; }
            Cancel();
            return;
        }
        _selected = slot.Id;
        _partial = null;
        _bar.Select(slot.Id);
        _board.ClearHighlights();
        _board.SetHighlight(_active, CardHighlight.Selected);
        _previewPanel.Visible = false;

        if (slot.Id == "@skip")
        {
            var (due, pos) = Preview.SkipTurn(_state, _active);
            ShowPreview(new[] { $"Skip Turn: next turn at time {due}", $"position {pos + 1} on the timeline", "Press again to skip." });
            return;
        }
        if (slot.Id == "@move")
        {
            _hint.Text = "Choose where to move";
            _board.ShowGaps(Movement.LegalMoves(_state, _active).Select(m => new GapRef(_state.GetUnit(_active).Side, m.Row, m.Index)));
            return;
        }
        var skill = Game.Catalog.Get(slot.Id);
        _hint.Text = Game.SkillName(slot.Id);
        ShowTargets(skill);
        if (NeedsNothing(skill)) ShowUse(new SkillUse(_active, skill.Id), "Press again to use.");
        _board.SetGhost(GhostFor(skill));
    }

    private static bool NeedsNothing(Skill skill) =>
        skill.Target == TargetKind.None || (skill.Target is TargetKind.Placement && false);

    private int? GhostFor(Skill skill)
    {
        var cost = Actions.CostOf(_state, _active, skill);
        if (cost <= 0) return null;
        var value = _state.Momentum.Value;
        var side = _state.GetUnit(_active).Side;
        var advantage = _state.Momentum.AdvantageOf(side) - cost;
        var meter = side == Side.A ? advantage : -advantage;
        return meter * (_board.Bottom == Side.A ? 1 : -1);
    }

    /// <summary>Highlights what the selected skill can be aimed at now.</summary>
    private void ShowTargets(Skill skill)
    {
        var user = _active;
        switch (skill.Target)
        {
            case TargetKind.Unit:
                foreach (var t in Actions.LegalTargets(_state, Game.Catalog, user, skill.Id)) _board.SetHighlight(t, CardHighlight.Legal);
                break;
            case TargetKind.UnitAndPlacement:
            case TargetKind.TwoUnits:
                foreach (var t in LegalUnits(skill)) _board.SetHighlight(t, CardHighlight.Legal);
                break;
            case TargetKind.Row:
                foreach (var row in skill.LegalRows(_state, user))
                    foreach (var id in _state.Formation.Units(row.Side, row.Row)) _board.SetHighlight(id, CardHighlight.Legal);
                break;
            case TargetKind.Placement:
                if (skill.PlacesRandomly(_state, user)) break;
                _board.ShowGaps(skill.PlacementOptions(_state, user).Select(p => new GapRef(_state.GetUnit(user).Side, p.Row, p.Index)));
                break;
        }
    }

    private IEnumerable<int> LegalUnits(Skill skill) =>
        Targeting.LegalTargets(_state, _active, Actions.ReachOf(_state, _active, skill), skill.Options with { Delivery = skill.Delivery })
            .Where(t => skill.TargetFilter(_state, _active, t)
                && (skill.Target != TargetKind.UnitAndPlacement || skill.PlacesRandomly(_state, _active) || skill.PlacementOptionsFor(_state, _active, t).Count > 0));

    private void OnCardClicked(int id)
    {
        if (Locked || _selected == null || _selected.StartsWith('@')) return;
        var skill = Game.Catalog.Get(_selected);
        switch (skill.Target)
        {
            case TargetKind.Unit:
                TryUse(new SkillUse(_active, skill.Id, id));
                break;
            case TargetKind.Row:
                var pos = _state.Formation.PositionOf(id);
                TryUse(new SkillUse(_active, skill.Id, Row: new RowTarget(pos.Side, pos.Row)));
                break;
            case TargetKind.UnitAndPlacement:
                if (_partial == null)
                {
                    if (!LegalUnits(skill).Contains(id)) return;
                    if (skill.PlacesRandomly(_state, _active)) { TryUse(new SkillUse(_active, skill.Id, id)); return; }
                    _partial = new SkillUse(_active, skill.Id, id);
                    _board.ClearHighlights();
                    _board.SetHighlight(_active, CardHighlight.Selected);
                    _board.SetHighlight(id, CardHighlight.Selected);
                    var side = _state.Formation.PositionOf(id).Side;
                    _board.ShowGaps(skill.PlacementOptionsFor(_state, _active, id).Select(p => new GapRef(side, p.Row, p.Index)));
                    _hint.Text = "Choose where it goes";
                }
                break;
            case TargetKind.TwoUnits:
                if (_partial == null)
                {
                    if (!LegalUnits(skill).Contains(id)) return;
                    _partial = new SkillUse(_active, skill.Id, id);
                    _board.ClearHighlights();
                    _board.SetHighlight(_active, CardHighlight.Selected);
                    _board.SetHighlight(id, CardHighlight.Selected);
                    foreach (var other in LegalUnits(skill).Where(o => o != id && skill.PairFilter(_state, _active, id, o))) _board.SetHighlight(other, CardHighlight.Legal);
                    _hint.Text = "Choose the second unit";
                }
                else TryUse(_partial with { SecondTargetUnitId = id });
                break;
        }
    }

    private void OnGapClicked(GapRef gap)
    {
        if (Locked || _selected == null) return;
        if (_selected == "@move")
        {
            Choose(new AiAction(AiActionKind.Move, null, gap.Row, gap.Index));
            return;
        }
        var skill = Game.Catalog.Get(_selected);
        if (skill.Target == TargetKind.Placement) TryUse(new SkillUse(_active, skill.Id, Placement: new MoveOption(gap.Row, gap.Index)));
        else if (skill.Target == TargetKind.UnitAndPlacement && _partial != null) TryUse(_partial with { Placement = new MoveOption(gap.Row, gap.Index) });
    }

    private void TryUse(SkillUse use)
    {
        var why = Actions.WhyNot(_state, Game.Catalog, use);
        if (why != null) { _hint.Text = why; return; }
        Choose(new AiAction(AiActionKind.Skill, use));
    }

    // ---------------------------------------------------------------- previews

    private void OnHover(int? id)
    {
        if (Locked || _selected == null || _selected.StartsWith('@') || id == null) { if (id == null && _partial == null && _selected != null && !(Game.Catalog.TryGet(_selected, out var s0) && NeedsNothing(s0))) _previewPanel.Visible = false; return; }
        var skill = Game.Catalog.Get(_selected);
        SkillUse? use = skill.Target switch
        {
            TargetKind.Unit => new SkillUse(_active, skill.Id, id),
            TargetKind.Row => _state.Formation.Contains(id.Value) ? new SkillUse(_active, skill.Id, Row: new RowTarget(_state.Formation.PositionOf(id.Value).Side, _state.Formation.PositionOf(id.Value).Row)) : null,
            _ => null,
        };
        if (use != null) ShowUse(use, null);
    }

    /// <summary>The floating preview panel (ui_spec "Target preview"): damage ranges, success chance, possible interceptors, timing.</summary>
    private void ShowUse(SkillUse use, string? footer)
    {
        var p = Preview.Skill(_state, Game.Catalog, use);
        var lines = new List<string>();
        if (!p.IsLegal) { ShowPreview(new[] { p.IllegalReason ?? "" }); return; }
        foreach (var u in p.Units)
        {
            var name = _state.GetUnit(u.UnitId).Name;
            if (u.DamageMax > 0) lines.Add($"{name}: {(u.DamageMin == u.DamageMax ? u.DamageMin.ToString() : $"{u.DamageMin}–{u.DamageMax}")} damage" + (u.HpLostMax < u.DamageMax ? $" ({u.HpLostMin}–{u.HpLostMax} after shields)" : ""));
            if (u.HealMax > 0) lines.Add($"{name}: +{(u.HealMin == u.HealMax ? u.HealMin.ToString() : $"{u.HealMin}–{u.HealMax}")} healing");
        }
        if (p.Units.Count > 0 && lines.Count == 0) lines.Add("Affects " + string.Join(", ", p.Units.Select(u => _state.GetUnit(u.UnitId).Name)));
        if (_state.GetUnit(use.UserId) is { } user && Game.Catalog.Get(use.SkillId).IsAttack) lines.Add($"Success chance {p.SuccessChance}%");
        if (p.Interceptors.Count > 0) lines.Add("May be intercepted by " + string.Join(", ", p.Interceptors.Select(i => $"{_state.GetUnit(i.UnitId).Name} {i.Percent}%")));
        foreach (var m in p.Moves.Where(m => m.Cause == MoveCause.Forced)) lines.Add($"{_state.GetUnit(m.UnitId).Name} moves to {m.To.Row}");
        foreach (var b in p.BlockedChains) lines.Add($"Blocked: {string.Join(", ", b.Chain.Select(c => _state.GetUnit(c).Name))} take 1 bump damage");
        if (p.MomentumCost > 0) lines.Add($"Momentum {p.MomentumBefore} → {p.MomentumAfterCost}");
        lines.Add($"Next turn at time {p.NextTurnDue} (position {p.NextTurnPosition + 1})");
        foreach (var s in p.TimelineShifts) lines.Add($"{(s.UnitId is { } id ? _state.GetUnit(id).Name : "Event")}: time {s.From} → {s.To}");
        if (footer != null) lines.Add(footer);
        ShowPreview(lines);
    }

    private void ShowPreview(IEnumerable<string> lines)
    {
        foreach (var c in _previewBox.GetChildren()) c.QueueFree();
        foreach (var l in lines) _previewBox.AddChild(Ui.Label(l, 20));
        _previewPanel.Visible = true;
        _previewPanel.Position = new Vector2(Size.X - 560, Size.Y - 520);
        _previewPanel.Size = new Vector2(360, 10);
    }

    // ---------------------------------------------------------------- inspection, surrender, result

    private void Inspect(int unitId)
    {
        _inspection?.QueueFree();
        _inspection = InspectionPanel.Open(this, _state, unitId);
    }

    private void ConfirmSurrender()
    {
        if (_state.IsOver || _surrendered) return;
        var dialog = new ConfirmationDialog { DialogText = "Surrender this battle? It counts as a defeat.", OkButtonText = "Surrender" };
        dialog.Confirmed += () =>
        {
            _surrendered = true;
            BattleEnd.Surrender(_state, Side.A);
            _choice?.TrySetResult(AiAction.Skip);
        };
        AddChild(dialog);
        dialog.PopupCentered();
    }

    private void ShowResult()
    {
        _bar.Clear();
        _hint.Text = "";
        _previewPanel.Visible = false;
        _board.ClearHighlights();
        var outcome = _state.Outcome;
        var result = BattleEnd.ResultFor(outcome, Side.A, drawCountsAsDefeat: true);
        var text = _surrendered ? "Surrendered" : result.ToString();
        var box = new VBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        box.AddThemeConstantOverride("separation", 16);
        box.AddChild(Ui.Label(text, 64, result == PlayerResult.Victory ? Ui.Gold : Ui.Ink, bold: true, align: HorizontalAlignment.Center));
        if (outcome == BattleOutcome.Draw) box.AddChild(Ui.Label("A draw counts as a defeat.", 22, new Color("cdbd9a"), align: HorizontalAlignment.Center));
        var retry = Ui.Button("Retry", 28);
        retry.Pressed += () => _main.Show("play-again");
        var exit = Ui.Button("Exit", 28);
        exit.Pressed += () => _main.Show("menu");
        var row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        row.AddThemeConstantOverride("separation", 16);
        row.AddChild(retry);
        row.AddChild(exit);
        box.AddChild(row);
        var center = new CenterContainer();
        center.AddChild(Ui.Panel(box));
        AddChild(center);
        Ui.Fill(center);
        _result = center;
    }

    /// <summary>The team setup of this battle, for Retry.</summary>
    public BattleSpec Spec => _spec;

    public string Level => _level;
}
