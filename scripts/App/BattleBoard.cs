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
    private const float LineGap = 26f;
    private const float Gap = 10f;
    private readonly DrawControl _ground = new();
    private readonly List<Pool> _pools = new();
    private readonly HashSet<int> _placed = new();
    private readonly Dictionary<int, Tween> _moves = new();
    private double _time;
    private int? _activeId;
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
        _ground.OnDraw = DrawGround;
        AddChild(_ground);
        Ui.Fill(_ground);
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
        _placed.Clear();
        _moves.Clear();
        _pools.Clear();
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
            if (present && _cards.TryGetValue(u.Id, out var live)) { live.Info.Mirror = u.Side != Bottom; live.Info.Active = u.Id == _activeId; }
        }
        if (updateInfos)
        {
            foreach (var (id, card) in _cards) UnitCards.Update(_state, _state.GetUnit(id), card.Info);
            _momentum.Value = _state.Momentum.Value * (Bottom == Side.A ? 1 : -1);
            _pools.Clear();
            foreach (var g in _state.Ground.Effects) _pools.Add(new Pool(g.Id, g.Kind, g.Spot) { Appear = 1 });
        }
        foreach (var card in _cards.Values) card.QueueRedraw();
        Layout(animate: !updateInfos);
        RefreshTimeline();
    }

    private void AddCard(Unit u)
    {
        var card = new PortraitView { Mode = CardMode.Compact, Tag = u.Id };
        var info = UnitCards.Of(_state, u);
        info.Mirror = u.Side != Bottom;
        info.Active = u.Id == _activeId;
        card.Bind(info);
        card.Clicked += c => CardClicked?.Invoke((int)c.Tag!);
        card.RightClicked += c => CardRightClicked?.Invoke((int)c.Tag!);
        card.MouseEntered += () => HoverChanged?.Invoke(u.Id);
        card.MouseExited += () => HoverChanged?.Invoke(null);
        _boardLayer.AddChild(card);
        _cards[u.Id] = card;
    }

    // ---------------------------------------------------------------- layout

    /// <summary>The unit the active glow shows on (null for none).</summary>
    public int? ActiveUnitId
    {
        get => _activeId;
        set
        {
            _activeId = value;
            foreach (var (id, card) in _cards) card.Info.Active = id == value;
        }
    }

    /// <summary>Switches the one-side view to the other side, with a smooth slide.</summary>
    public void ShowSide(Side side)
    {
        if (ShownSide == side) return;
        ShownSide = side;
        Layout(animate: true);
    }

    /// <summary>The column of a row from the left edge: the player's Rear, Middle, Front, the battle line, then the enemy's Front, Middle, Rear.</summary>
    private int Column(Side side, Row row)
    {
        var own = side == Bottom;
        if (OneSide) return side == ShownSide ? (own ? 2 - (int)row : (int)row) : (own ? -5 : 8);
        return own ? 2 - (int)row : 3 + (int)row;
    }

    private float ColumnX(int column)
    {
        if (OneSide) return _boardArea.Position.X + (_boardArea.Size.X - (3 * _cardSize.X + 2 * Gap)) / 2 + column * (_cardSize.X + Gap);
        var total = 6 * _cardSize.X + 5 * Gap + LineGap;
        return _boardArea.Position.X + (_boardArea.Size.X - total) / 2 + column * (_cardSize.X + Gap) + (column >= 3 ? LineGap : 0);
    }

    private float CoordY(int coordinate) => _boardArea.Position.Y + _boardArea.Size.Y / 2 + coordinate * (_cardSize.Y + Gap) / 2 - _cardSize.Y / 2;

    private Vector2 SpotCentre(Spot spot) => new(ColumnX(Column(spot.Side, spot.Row)) + _cardSize.X / 2, CoordY(spot.Coordinate) + _cardSize.Y / 2);

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
        var fullest = 1;
        foreach (var side in new[] { Side.A, Side.B })
        {
            if (OneSide && side != ShownSide) continue;
            foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear }) fullest = Math.Max(fullest, _state.Formation.Units(side, row).Count);
        }
        var byHeight = (_boardArea.Size.Y - (fullest - 1) * Gap) / fullest;
        var byWidth = OneSide ? (_boardArea.Size.X - 2 * Gap) / 3f : (_boardArea.Size.X - 5 * Gap - LineGap) / 6f;
        var p = Math.Max(24f, Math.Min(Math.Min(byHeight, byWidth), OneSide ? 300f : 200f));
        _cardSize = new Vector2(p, p);
        var order = 0;
        foreach (var (id, card) in _cards)
        {
            if (!_state.Formation.Contains(id) || !_shown.Contains(id)) { card.Visible = false; continue; }
            var pos = _state.Formation.PositionOf(id);
            card.Visible = !OneSide || pos.Side == ShownSide;
            var target = new Vector2(ColumnX(Column(pos.Side, pos.Row)), CoordY(_state.Formation.CenterOf(pos)));
            if (_moves.TryGetValue(id, out var running)) { running.Kill(); _moves.Remove(id); }
            if (_placed.Add(id) && Speed > 0)
            {
                // The first appearance: the team slides in from its own screen edge.
                card.Size = _cardSize;
                card.Position = new Vector2(pos.Side == Bottom ? -p : w + p, target.Y);
                var slide = _moves[id] = CreateTween();
                slide.TweenInterval(0.05 * order++ / Speed);
                slide.TweenProperty(card, "position", target, 0.5 / Speed).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
                continue;
            }
            if (animate && Speed > 0 && card.Size.X > 1)
            {
                var tween = _moves[id] = CreateTween();
                tween.SetParallel(true);
                tween.TweenProperty(card, "position", target, 0.25 / Speed);
                tween.TweenProperty(card, "size", _cardSize, 0.25 / Speed);
            }
            else { card.Position = target; card.Size = _cardSize; }
        }
        _momentum.Position = new Vector2(w - right + 20, top + _boardArea.Size.Y * 0.15f);
        _momentum.Size = new Vector2(70, _boardArea.Size.Y * 0.7f);
        if (FindChild("TimelinePanel", false, false) is Control tl)
        {
            tl.Position = new Vector2(20, top + _boardArea.Size.Y * 0.15f);
            tl.Size = new Vector2(left - 40, _boardArea.Size.Y * 0.7f);
        }
        _ground.QueueRedraw();
    }

    public override void _Process(double delta)
    {
        _time += delta;
        if (_pools.Count == 0) return;
        for (var i = _pools.Count - 1; i >= 0; i--)
        {
            var pool = _pools[i];
            var step = Speed <= 0 ? 1f : (float)(delta * Speed / 0.4);
            if (pool.Ending) { pool.Fade -= step; if (pool.Fade <= 0) { _pools.RemoveAt(i); continue; } }
            else if (pool.Appear < 1) pool.Appear = Math.Min(1, pool.Appear + step);
        }
        _ground.QueueRedraw();
    }

    // ---------------------------------------------------------------- ground effects

    private sealed class Pool
    {
        public Pool(int id, GroundKind kind, Spot spot) { Id = id; Kind = kind; Spot = spot; }
        public int Id { get; }
        public GroundKind Kind { get; }
        public Spot Spot { get; }
        public float Appear;
        public float Fade = 1;
        public bool Ending;
    }

    private static Color PoolColor(GroundKind kind) => kind switch
    {
        GroundKind.Burning => new Color(0.95f, 0.45f, 0.1f),
        GroundKind.Ice => new Color(0.75f, 0.9f, 1f),
        GroundKind.Water => new Color(0.12f, 0.3f, 0.65f),
        _ => new Color(0.4f, 0.75f, 0.25f),
    };

    /// <summary>The ground layer: a round pool about 1.3 portraits wide on each spot, touching pools of one effect merged into one patch.</summary>
    private void DrawGround(DrawControl c)
    {
        if (_state == null || _pools.Count == 0) return;
        var r = _cardSize.X * 0.65f;
        foreach (var kind in new[] { GroundKind.Water, GroundKind.Ice, GroundKind.Poison, GroundKind.Burning })
        {
            var mine = _pools.Where(p => p.Kind == kind && (!OneSide || p.Spot.Side == ShownSide)).ToList();
            foreach (var pool in mine)
            {
                var alpha = 0.5f * Math.Clamp(pool.Appear, 0, 1) * Math.Clamp(pool.Fade, 0, 1);
                var rad = r * (0.4f + 0.6f * Math.Clamp(pool.Appear, 0, 1));
                var centre = SpotCentre(pool.Spot);
                var colour = new Color(PoolColor(kind), alpha);
                c.DrawCircle(centre, rad, colour);
                foreach (var other in mine)
                {
                    if (other.Id <= pool.Id || other.Spot.Side != pool.Spot.Side || other.Spot.Row != pool.Spot.Row || Math.Abs(other.Spot.Coordinate - pool.Spot.Coordinate) > 2) continue;
                    c.DrawLine(centre, SpotCentre(other.Spot), colour, rad * 2);
                }
                DrawPoolDetail(c, kind, centre, rad, alpha);
            }
        }
    }

    private void DrawPoolDetail(DrawControl c, GroundKind kind, Vector2 centre, float rad, float alpha)
    {
        var t = (float)_time;
        switch (kind)
        {
            case GroundKind.Burning:
                c.DrawCircle(centre, rad * (0.55f + 0.08f * Mathf.Sin(t * 9)), new Color(1f, 0.8f, 0.2f, alpha * 0.7f));
                for (var i = 0; i < 4; i++)
                {
                    var ph = (t * 0.7f + i * 0.25f) % 1f;
                    c.DrawCircle(centre + new Vector2(Mathf.Sin(i * 2.1f) * rad * 0.5f, -ph * rad), Math.Max(1.5f, rad * 0.05f), new Color(1f, 0.7f, 0.2f, Math.Min(1f, alpha * 1.4f * (1 - ph))));
                }
                break;
            case GroundKind.Ice:
                c.DrawLine(centre + new Vector2(-rad * 0.6f, 0), centre + new Vector2(rad * 0.5f, -rad * 0.2f), new Color(1, 1, 1, Math.Min(1f, alpha * 1.2f)), 2);
                c.DrawLine(centre + new Vector2(-rad * 0.1f, -rad * 0.5f), centre + new Vector2(rad * 0.2f, rad * 0.55f), new Color(1, 1, 1, Math.Min(1f, alpha * 1.2f)), 2);
                var glint = (t * 0.5f) % 1f;
                c.DrawCircle(centre + new Vector2(-rad * 0.5f + glint * rad, -rad * 0.3f + glint * rad * 0.3f), Math.Max(2f, rad * 0.06f), new Color(1, 1, 1, Math.Min(1f, alpha * 1.6f)));
                break;
            case GroundKind.Water:
                for (var i = 0; i < 2; i++)
                {
                    var ph = (t * 0.4f + i * 0.5f) % 1f;
                    c.DrawArc(centre, rad * (0.3f + 0.6f * ph), 0, Mathf.Tau, 28, new Color(0.6f, 0.8f, 1f, alpha * (1 - ph)), 2);
                }
                break;
            default:
                for (var i = 0; i < 4; i++)
                {
                    var ph = (t * 0.5f + i * 0.25f) % 1f;
                    c.DrawArc(centre + new Vector2(Mathf.Cos(i * 1.7f) * rad * 0.45f, rad * 0.4f - ph * rad * 0.9f), Math.Max(2f, rad * 0.07f), 0, Mathf.Tau, 12, new Color(0.7f, 1f, 0.5f, Math.Min(1f, alpha * 1.5f * (1 - ph))), 1.5f);
                }
                break;
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
            var portrait = new PortraitView { Mode = CardMode.Compact, CustomMinimumSize = new Vector2(56, 56), MouseFilter = MouseFilterEnum.Ignore };
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

    /// <summary>Shows a clickable marker at each gap a unit could land in: a thin bar across the column between two portraits.</summary>
    public void ShowGaps(IEnumerable<GapRef> gaps)
    {
        FirstGap = null;
        foreach (var g in gaps)
        {
            FirstGap ??= g;
            var units = _state.Formation.Units(g.Side, g.Row);
            var step = (_cardSize.Y + Gap) / 2;
            var centreY = _boardArea.Position.Y + _boardArea.Size.Y / 2;
            float y;
            if (units.Count == 0) y = centreY;
            else
            {
                float Cy(int id) => centreY + _state.Formation.CenterOf(_state.Formation.PositionOf(id)) * step;
                y = g.Index <= 0 ? Cy(units[0]) - step
                    : g.Index >= units.Count ? Cy(units[^1]) + step
                    : (Cy(units[g.Index - 1]) + Cy(units[g.Index])) / 2;
            }
            var x = ColumnX(Column(g.Side, g.Row));
            var marker = new Button { Text = "", CustomMinimumSize = new Vector2(_cardSize.X, 18), Size = new Vector2(_cardSize.X, 18), Position = new Vector2(x, y - 9) };
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
            case GroundEffectPlaced gp:
                if (!gp.Refreshed && _pools.All(p => p.Id != gp.EffectId)) _pools.Add(new Pool(gp.EffectId, gp.Kind, gp.Spot) { Appear = Speed <= 0 ? 1 : 0 });
                await Wait(0.4);
                break;
            case GroundEffectEnded ge:
                foreach (var p in _pools.Where(p => p.Id == ge.EffectId)) p.Ending = true;
                await Wait(0.2);
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
