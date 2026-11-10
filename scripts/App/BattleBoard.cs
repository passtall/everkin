using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Everkin.Core.Battle;
using Godot;

namespace Everkin.App;

/// <summary>A gap a unit can land in: the row and the index it would have.</summary>
public readonly record struct GapRef(Side Side, Row Row, int Index);

/// <summary>
/// The battle screen's picture of a battle (ui_spec "Battle screen"): the board with both sides, the timeline on the left, the Momentum meter on
/// the right. It reads a <see cref="BattleState"/>, never changes it, and plays the events of an action one after another (M3.2, M3.3).
/// </summary>
public partial class BattleBoard : Control
{
    private BattleState _state = null!;
    private readonly Dictionary<int, CardView> _cards = new();
    private readonly HashSet<int> _shown = new();
    private readonly Control _boardLayer = new();
    private readonly Control _gapLayer = new();
    private readonly Control _fxLayer = new();
    private readonly TextureRect _background = new();
    private readonly ColorRect _dim = new() { Color = new Color(0, 0, 0, 0.45f), MouseFilter = MouseFilterEnum.Ignore };
    private readonly VBoxContainer _timelineList = new();
    private readonly MomentumBar _momentum = new();
    private Rect2 _boardArea;
    private Vector2 _cardSize;
    private readonly List<Control> _gapMarkers = new();

    public Side Bottom { get; private set; } = Side.A;

    /// <summary>Both sides at once (desktop, tablet), or one side with bigger cards (phone).</summary>
    public bool OneSide { get; set; }

    public Side ShownSide { get; set; } = Side.A;

    /// <summary>The space kept free above the board and below it (the action bar), in pixels at the screen's size.</summary>
    public float TopMargin { get; set; } = 20f;
    public float BottomMargin { get; set; } = 230f;

    /// <summary>1 = normal, 2 = double speed, 0 = no waiting at all.</summary>
    public float Speed { get; set; } = 1f;

    public event Action<int>? CardClicked;
    public event Action<int>? CardRightClicked;
    public event Action<GapRef>? GapClicked;
    public event Action<int?>? HoverChanged;

    public BattleState State => _state;

    public CardView? CardOf(int unitId) => _cards.GetValueOrDefault(unitId);

    public Rect2 BoardArea => _boardArea;

    public override void _Ready()
    {
        _background.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        _background.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
        _background.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(_background);
        Ui.Fill(_background);
        AddChild(_dim);
        Ui.Fill(_dim);
        _boardLayer.MouseFilter = MouseFilterEnum.Ignore;
        _gapLayer.MouseFilter = MouseFilterEnum.Ignore;
        _fxLayer.MouseFilter = MouseFilterEnum.Ignore;
        AddChild(_boardLayer);
        AddChild(_gapLayer);
        AddChild(_momentum);
        AddChild(BuildTimeline());
        AddChild(_fxLayer);
        Resized += () => Layout(animate: false);
    }

    private Control BuildTimeline()
    {
        var scroll = new ScrollContainer { Name = "Timeline", HorizontalScrollMode = ScrollContainer.ScrollMode.Disabled, VerticalScrollMode = ScrollContainer.ScrollMode.ShowAlways };
        scroll.AddChild(_timelineList);
        _timelineList.AddThemeConstantOverride("separation", 6);
        _timelineList.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        var panel = Ui.Panel(scroll);
        panel.Name = "TimelinePanel";
        return panel;
    }

    // ---------------------------------------------------------------- setup and refresh

    public void Setup(BattleState state, Side bottom)
    {
        _state = state;
        Bottom = bottom;
        ShownSide = bottom;
        foreach (var c in _cards.Values) c.QueueFree();
        _cards.Clear();
        _shown.Clear();
        var biome = state.Units.Where(u => u.Side == bottom.Opposite() && u.CreatureId != null && Game.Content.CreaturesById.ContainsKey(u.CreatureId))
            .Select(u => Game.Content.CreaturesById[u.CreatureId!].Biome).FirstOrDefault() ?? "ruins";
        _background.Texture = Game.Tex($"res://assets/backgrounds/battle/{biome}.png");
        Refresh();
    }

    /// <summary>Brings the whole picture in line with the state (used at the start and after an action has been played).</summary>
    public void Refresh() => Sync(updateInfos: true);

    /// <summary>Adds and removes cards and lays them out; with <paramref name="updateInfos"/> the existing cards also take the state's numbers.</summary>
    private void Sync(bool updateInfos)
    {
        foreach (var u in _state.Units)
        {
            var present = !u.IsRemoved && _state.Formation.Contains(u.Id);
            if (present && !_cards.ContainsKey(u.Id)) AddCard(u);
            if (!present && _cards.TryGetValue(u.Id, out var gone)) { gone.QueueFree(); _cards.Remove(u.Id); _shown.Remove(u.Id); }
            if (present) _shown.Add(u.Id);
        }
        if (updateInfos)
        {
            foreach (var (id, card) in _cards) UnitCards.Update(_state, _state.GetUnit(id), card.Info);
            _momentum.Value = _state.Momentum.Value * (Bottom == Side.A ? 1 : -1);
        }
        foreach (var card in _cards.Values) card.QueueRedraw();
        Layout(animate: !updateInfos);
        RefreshTimeline();
    }

    private void AddCard(Unit u)
    {
        var card = new CardView { Mode = CardMode.Compact, Tag = u.Id };
        card.Bind(UnitCards.Of(_state, u));
        card.Clicked += c => CardClicked?.Invoke((int)c.Tag!);
        card.RightClicked += c => CardRightClicked?.Invoke((int)c.Tag!);
        card.MouseEntered += () => HoverChanged?.Invoke(u.Id);
        card.MouseExited += () => HoverChanged?.Invoke(null);
        _boardLayer.AddChild(card);
        _cards[u.Id] = card;
    }

    // ---------------------------------------------------------------- layout

    private float RowY(Side side, Row row)
    {
        var rows = OneSide ? 3 : 6;
        var line = OneSide ? 0 : 14f;
        var index = OneSide
            ? (side == Bottom ? (int)row : 2 - (int)row)                                  // the shown side: its Front row at the top
            : side == Bottom ? 3 + (int)row : 2 - (int)row;                               // the far side above: Rear, Middle, Front; the near side: Front, Middle, Rear
        if (OneSide && side == ShownSide) index = (int)row;
        var extra = !OneSide && index >= 3 ? line : 0f;
        return _boardArea.Position.Y + index * (_cardSize.Y + 10) + extra;
    }

    public void Layout(bool animate)
    {
        if (_state == null || Size.X < 10) return;
        var w = Size.X;
        var h = Size.Y;
        var top = TopMargin;
        var bottom = Math.Min(BottomMargin, h * 0.23f);
        var left = Math.Min(300f, w * 0.17f);
        var right = 110f;
        _boardArea = new Rect2(left, top, w - left - right, h - top - bottom);
        var rows = OneSide ? 3 : 6;
        var ch = (_boardArea.Size.Y - (rows - 1) * 10 - (OneSide ? 0 : 14)) / rows;
        var cw = ch * 2f / 3f;
        var perRow = (_boardArea.Size.X) / 6f - 10;
        if (cw > perRow) { cw = perRow; ch = cw * 1.5f; }
        _cardSize = new Vector2(cw, ch);
        var centerX = _boardArea.Position.X + _boardArea.Size.X / 2;
        // If one row would be wider than the board, every card in the row shrinks evenly.
        foreach (var (id, card) in _cards)
        {
            var u = _state.GetUnit(id);
            if (!_state.Formation.Contains(id) || !_shown.Contains(id)) { card.Visible = false; continue; }
            var pos = _state.Formation.PositionOf(id);
            card.Visible = !OneSide || pos.Side == ShownSide;
            var coord = _state.Formation.CenterOf(pos);
            var target = new Vector2(centerX + coord * (cw + 10) / 2 - cw / 2, RowY(pos.Side, pos.Row));
            card.Size = _cardSize;
            if (animate && Speed > 0 && card.Position != Vector2.Zero)
            {
                var tween = CreateTween();
                tween.TweenProperty(card, "position", target, 0.25 / Speed);
            }
            else card.Position = target;
            card.ZIndex = 0;
        }
        _momentum.Position = new Vector2(w - right + 20, top + _boardArea.Size.Y * 0.15f);
        _momentum.Size = new Vector2(70, _boardArea.Size.Y * 0.7f);
        if (FindChild("TimelinePanel", false, false) is Control tl)
        {
            tl.Position = new Vector2(20, top + _boardArea.Size.Y * 0.15f);
            tl.Size = new Vector2(left - 40, _boardArea.Size.Y * 0.7f);
        }
    }

    // ---------------------------------------------------------------- timeline

    public void RefreshTimeline()
    {
        foreach (var child in _timelineList.GetChildren()) child.QueueFree();
        var now = _state.Timeline.Now;
        foreach (var e in _state.Timeline.Upcoming().Where(e => e.Kind is TimelineKind.UnitTurn or TimelineKind.AutoEvent).Take(40))
        {
            if (e.OwnerUnitId is not { } id) continue;
            var u = _state.GetUnit(id);
            var row = new HBoxContainer();
            row.AddThemeConstantOverride("separation", 6);
            var stripe = new ColorRect { Color = u.Side == Side.A ? Ui.SideA : Ui.SideB, CustomMinimumSize = new Vector2(5, 0) };
            row.AddChild(stripe);
            var portrait = new CardView { Mode = CardMode.Compact, CustomMinimumSize = new Vector2(44, 66), MouseFilter = MouseFilterEnum.Ignore };
            portrait.Bind(_cards.TryGetValue(id, out var existing) ? existing.Info : UnitCards.Of(_state, u));
            row.AddChild(portrait);
            var text = new VBoxContainer();
            text.AddChild(Ui.Label(u.Name, 18, e.Kind == TimelineKind.AutoEvent ? new Color("a89d86") : Ui.Ink, bold: true));
            text.AddChild(Ui.Label((e.Kind == TimelineKind.AutoEvent ? "shot · " : "") + $"in {Math.Max(0, e.Due - now)}", 16, new Color("cdbd9a")));
            row.AddChild(text);
            var wrap = Ui.Panel(row);
            wrap.AddThemeStyleboxOverride("panel", Ui.Box(e.Kind == TimelineKind.AutoEvent ? new Color("0c0a08") : new Color("1e1812"), Ui.Bronze, 2, 4));
            _timelineList.AddChild(wrap);
        }
    }

    // ---------------------------------------------------------------- highlights, gaps, ghost

    public void ClearHighlights()
    {
        foreach (var c in _cards.Values) c.Highlight = CardHighlight.None;
        foreach (var m in _gapMarkers) m.QueueFree();
        _gapMarkers.Clear();
    }

    public void SetHighlight(int unitId, CardHighlight h)
    {
        if (_cards.TryGetValue(unitId, out var c)) c.Highlight = h;
    }

    /// <summary>Shows a clickable marker at each gap a unit could land in.</summary>
    public void ShowGaps(IEnumerable<GapRef> gaps)
    {
        FirstGap = null;
        foreach (var g in gaps)
        {
            FirstGap ??= g;
            var units = _state.Formation.Units(g.Side, g.Row);
            var cw = _cardSize.X;
            var centerX = _boardArea.Position.X + _boardArea.Size.X / 2;
            float x;
            if (units.Count == 0) x = centerX;
            else
            {
                float Cx(int id) => centerX + _state.Formation.CenterOf(_state.Formation.PositionOf(id)) * (cw + 10) / 2;
                x = g.Index <= 0 ? Cx(units[0]) - (cw + 10) / 2
                    : g.Index >= units.Count ? Cx(units[^1]) + (cw + 10) / 2
                    : (Cx(units[g.Index - 1]) + Cx(units[g.Index])) / 2;
            }
            var marker = new Button { Text = "", CustomMinimumSize = new Vector2(18, _cardSize.Y), Size = new Vector2(18, _cardSize.Y), Position = new Vector2(x - 9, RowY(g.Side, g.Row)) };
            marker.AddThemeStyleboxOverride("normal", Ui.Box(new Color(Ui.Gold, 0.55f), Ui.Gold, 2, 4));
            marker.AddThemeStyleboxOverride("hover", Ui.Box(new Color(1, 1, 1, 0.8f), Colors.White, 2, 4));
            marker.Pressed += () => GapClicked?.Invoke(g);
            _gapLayer.AddChild(marker);
            _gapMarkers.Add(marker);
        }
    }

    public int GapCount => _gapMarkers.Count;

    public GapRef? FirstGap { get; private set; }

    public void SetGhost(int? meterFromBottom) => _momentum.Ghost = meterFromBottom;

    // ---------------------------------------------------------------- playing events

    private double D(double seconds) => Speed <= 0 ? 0 : seconds / Speed;

    private async Task Wait(double seconds)
    {
        if (Speed <= 0 || seconds <= 0) return;
        await ToSignal(GetTree().CreateTimer(D(seconds)), SceneTreeTimer.SignalName.Timeout);
    }

    /// <summary>Plays the events of an action one after another, each with its own duration, and ends with the state's picture.</summary>
    public async Task PlayAsync(IEnumerable<BattleEvent> events)
    {
        foreach (var e in events) await PlayEvent(e);
        Refresh();
    }

    private async Task PlayEvent(BattleEvent e)
    {
        switch (e)
        {
            case SkillUsed used:
                if (_cards.TryGetValue(used.UserId, out var user)) { await Pulse(user); }
                break;
            case DamageDealt d:
                if (_cards.TryGetValue(d.TargetUnitId, out var hit))
                {
                    hit.Info.Hp = d.HpAfter;
                    hit.QueueRedraw();
                    if (d.Result.HpLost > 0) Float(hit, $"-{d.Result.HpLost}", Ui.Bad);
                    else if (d.Result.AbsorbedByWard > 0) Float(hit, $"{d.Result.AbsorbedByWard}", new Color("8fb8e8"));
                    await Shake(hit);
                }
                break;
            case Healed h when h.Amount > 0 && _cards.TryGetValue(h.TargetUnitId, out var healed):
                healed.Info.Hp = h.HpAfter;
                healed.QueueRedraw();
                Float(healed, $"+{h.Amount}", Ui.Good);
                await Wait(0.25);
                break;
            case AttackMissed miss when _cards.TryGetValue(miss.TargetUnitId, out var missed):
                Float(missed, "Miss", Ui.Ink);
                await Wait(0.25);
                break;
            case Intercepted ic when _cards.TryGetValue(ic.InterceptorUnitId, out var guard):
                Float(guard, "Guard", Ui.Gold);
                await Wait(0.2);
                break;
            case StatusApplied sa when _cards.TryGetValue(sa.UnitId, out var st):
                if (!st.Info.Statuses.Contains(sa.StatusId)) st.Info.Statuses.Add(sa.StatusId);
                st.QueueRedraw();
                await Wait(0.12);
                break;
            case StatusEnded se when _cards.TryGetValue(se.UnitId, out var ended):
                ended.Info.Statuses.Remove(se.StatusId);
                ended.QueueRedraw();
                break;
            case UnitKnockedOut ko when _cards.TryGetValue(ko.UnitId, out var fallen):
                fallen.Info.Hp = 0;
                fallen.Info.Ko = true;
                fallen.QueueRedraw();
                await Shake(fallen);
                break;
            case UnitRevived rv when _cards.TryGetValue(rv.UnitId, out var back):
                back.Info.Ko = false;
                back.Info.Hp = rv.Hp;
                back.QueueRedraw();
                await Wait(0.2);
                break;
            case UnitMoved or UnitsSwapped or UnitSummoned or UnitRemoved or UnitRaised or UndeadRestored:
                Sync(updateInfos: false);
                await Wait(0.3);
                break;
            case MomentumChanged m:
                _momentum.Value = m.After * (Bottom == Side.A ? 1 : -1);
                await Wait(0.1);
                break;
            case GroundEffectPlaced:
                await Wait(0.15);
                break;
        }
    }

    private async Task Pulse(CardView card)
    {
        if (Speed <= 0) return;
        card.ZIndex = 5;
        var tween = CreateTween();
        tween.TweenProperty(card, "scale", new Vector2(1.08f, 1.08f), D(0.1));
        tween.TweenProperty(card, "scale", Vector2.One, D(0.1));
        card.PivotOffset = card.Size / 2;
        await ToSignal(tween, Tween.SignalName.Finished);
        card.ZIndex = 0;
    }

    private async Task Shake(CardView card)
    {
        if (Speed <= 0 || card.Info.Ko && false) return;
        var origin = card.Position;
        var tween = CreateTween();
        for (var i = 0; i < 3; i++)
        {
            tween.TweenProperty(card, "position", origin + new Vector2(6, 0), D(0.04));
            tween.TweenProperty(card, "position", origin - new Vector2(6, 0), D(0.04));
        }
        tween.TweenProperty(card, "position", origin, D(0.04));
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    private void Float(CardView card, string text, Color color)
    {
        if (Speed <= 0) return;
        var label = Ui.Label(text, 26, color, bold: true, align: HorizontalAlignment.Center);
        label.AddThemeColorOverride("font_outline_color", Colors.Black);
        label.AddThemeConstantOverride("outline_size", 6);
        label.Size = new Vector2(card.Size.X, 34);
        label.Position = card.Position + new Vector2(0, card.Size.Y * 0.3f);
        label.ZIndex = 20;
        _fxLayer.AddChild(label);
        var tween = CreateTween();
        tween.SetParallel(true);
        tween.TweenProperty(label, "position:y", label.Position.Y - 50, D(0.9));
        tween.TweenProperty(label, "modulate:a", 0f, D(0.9)).SetDelay(D(0.4));
        tween.Chain().TweenCallback(Callable.From(label.QueueFree));
    }
}

/// <summary>The vertical Momentum meter at the right edge: the player's end at the bottom, a marker at the current value and a ghost marker for a preview.</summary>
public partial class MomentumBar : Control
{
    private int _value;
    private int? _ghost;

    public int Value
    {
        get => _value;
        set { _value = Math.Clamp(value, -10, 10); QueueRedraw(); }
    }

    /// <summary>Where the meter would land after paying a skill's cost (a ghost marker), in the meter's own scale, or null.</summary>
    public int? Ghost
    {
        get => _ghost;
        set { _ghost = value; QueueRedraw(); }
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, Size), new Color(Ui.PanelDark, 0.92f));
        DrawRect(new Rect2(Vector2.Zero, Size), Ui.Gold, false, 3);
        var inner = new Rect2(16, 14, Size.X - 32, Size.Y - 28);
        DrawRect(inner, new Color("0a0806"));
        // From top (-10) to bottom (+10): the player's end is at the bottom, so a positive value fills upward from the middle.
        var mid = inner.Position.Y + inner.Size.Y / 2;
        var unit = inner.Size.Y / 20f;
        var y = mid - _value * unit;
        var color = _value >= 0 ? Ui.SideA : Ui.SideB;
        if (_value >= 0) DrawRect(new Rect2(inner.Position.X, y, inner.Size.X, mid - y), new Color(color, 0.8f));
        else DrawRect(new Rect2(inner.Position.X, mid, inner.Size.X, y - mid), new Color(color, 0.8f));
        for (var i = -10; i <= 10; i += 5)
            DrawLine(new Vector2(inner.Position.X, mid - i * unit), new Vector2(inner.End.X, mid - i * unit), new Color(1, 1, 1, i == 0 ? 0.8f : 0.3f), i == 0 ? 3 : 1);
        DrawRect(new Rect2(inner.Position.X - 6, y - 4, inner.Size.X + 12, 8), Ui.Gold);
        if (_ghost is { } g)
        {
            var gy = mid - g * unit;
            DrawRect(new Rect2(inner.Position.X - 6, gy - 3, inner.Size.X + 12, 6), new Color(1, 1, 1, 0.55f));
        }
    }
}
