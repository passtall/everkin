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
public partial class PlayScreen : Control, IBackNavigable
{
    private bool _left;

    /// <summary>Back during a battle: first cancels a half-made choice, then asks whether to leave (the battle stays saved and can be resumed).</summary>
    public bool GoBack()
    {
        if (_result != null) { LeaveBattle(); return true; }
        if (_selected != null || _partial != null) { Cancel(); return true; }
        ConfirmLeave();
        return true;
    }

    private void ConfirmLeave()
    {
        var dialog = new ConfirmationDialog { DialogText = "Leave the battle? It is saved and can be resumed from the menu.", OkButtonText = "Leave" };
        dialog.Confirmed += LeaveBattle;
        AddChild(dialog);
        dialog.PopupCentered();
    }

    private void LeaveBattle()
    {
        _left = true;
        _choice?.TrySetResult(AiAction.Skip);
        if (_encounter != null) _main.Open(new ChapterMapScreen(_main, Game.Campaign.ChapterOf(_encounter)));
        else _main.Show("menu");
    }

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

    private readonly string? _encounter;
    private BattleSave _save;
    private readonly BattleSave? _resume;
    private static SaveStore BattleStore => new(ProjectSettings.GlobalizePath("user://battle.json"));

    /// <summary>The battle in progress on disk (written after every action), or null when there is none.</summary>
    public static BattleSave? SavedBattle()
    {
        try { return BattleStore.Load() is { } text ? BattleSave.FromJson(text) : null; }
        catch (Exception) { return null; }
    }

    public static void DropSavedBattle() => BattleStore.Delete();

    public PlayScreen(Main main, BattleSpec spec, string level, string? encounter = null, BattleSave? resume = null)
    {
        _main = main;
        _spec = resume?.ToSpec() ?? spec;
        _level = resume?.Level ?? level;
        _encounter = resume?.Encounter ?? encounter;
        _resume = resume;
        _save = resume ?? BattleSave.Of(_spec, _level, _encounter, Game.ContentFingerprint);
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
        var leave = Ui.Button("Leave", 20);
        leave.Pressed += ConfirmLeave;
        var surrender = Ui.Button("Surrender", 20);
        surrender.Pressed += ConfirmSurrender;
        top.AddChild(leave);
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
        _state = _resume != null ? _resume.Replay(Game.Catalog) ?? BattleSetup.Create(_spec, Game.Catalog) : BattleSetup.Create(_spec, Game.Catalog);
        if (_resume != null && _state.TurnsTaken == 0) _save.Actions.Clear();
        _ai = new AiSearch(Game.Catalog, Game.Settings.Level(_level), new BattleRandom(_spec.Seed * 2 + 2));
        _aiPlayer = new AiSearch(Game.Catalog, Game.Settings.Level("harness"), new BattleRandom(_spec.Seed * 2 + 1));
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
            if (_left || _state.IsOver || unitId is not { } id) break;

            AiAction action;
            if (_state.GetUnit(id).Side == Side.A && Autoplay)
            {
                var snap = _state;
                action = await Task.Run(() => _aiPlayer.Choose(snap, id));
            }
            else if (_state.GetUnit(id).Side == Side.A) action = await AskPlayer(id);
            else
            {
                var snapshot = _state;
                action = await Task.Run(() => _ai.Choose(snapshot, id));
            }
            if (_surrendered || _left) break;
            _resolving = true;
            var events = action.Apply(_state, Game.Catalog, id);
            if (BattleEnd.CheckTurnCap(_state)) events = events.Concat(_state.DrainEvents()).ToList();
            _save.Actions.Add(SavedAction.From(id, action));
            if (!_state.IsOver) BattleStore.Save(_save.ToJson());                       // the battle resumes where it stopped (production section 1.4)
            await _board.PlayAsync(events);
            _resolving = false;
        }
        if (!_left) ShowResult();
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

    /// <summary>The AI plays the player's side too (with --autoplay), to check a whole campaign battle end to end.</summary>
    public bool Autoplay { get; set; }

    private AiSearch _aiPlayer = null!;

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
        if (k.Keycode == Key.Escape) return;                                   // Esc is "back": see GoBack (the main screen calls it)
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
        DropSavedBattle();
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
        var row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        row.AddThemeConstantOverride("separation", 16);
        var enc = _encounter != null ? Game.Campaign.Encounter(_encounter) : null;
        var won = result == PlayerResult.Victory;
        if (enc != null && won)
        {
            var fresh = Profile.Complete(enc);
            if (fresh.Count > 0)
                box.AddChild(Ui.Label("New: " + string.Join(", ", fresh.Select(f => Game.Content.CreaturesById[f].Name)), 28, Ui.Gold, bold: true, align: HorizontalAlignment.Center));
            else box.AddChild(Ui.Label("The reward was already received.", 22, new Color("cdbd9a"), align: HorizontalAlignment.Center));
            var cont = Ui.Button("Continue", 28);
            cont.Pressed += () => _main.Open(new ChapterMapScreen(_main, Game.Campaign.ChapterOf(enc.Id)));
            row.AddChild(cont);
        }
        else
        {
            var retry = Ui.Button("Retry", 28);
            retry.Pressed += () => _main.Show("play-again");
            row.AddChild(retry);
            if (enc != null)
            {
                var edit = Ui.Button("Edit team", 28);
                edit.Pressed += () => _main.Open(new TeamEditor(_main) { Back = () => _main.Open(new ChapterMapScreen(_main, Game.Campaign.ChapterOf(enc.Id))) });
                row.AddChild(edit);
            }
        }
        var exit = Ui.Button(enc != null ? "Map" : "Exit", 28);
        exit.Pressed += () => { if (enc != null) _main.Open(new ChapterMapScreen(_main, Game.Campaign.ChapterOf(enc.Id))); else _main.Show("menu"); };
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
