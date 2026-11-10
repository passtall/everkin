using System;
using System.Linq;
using Everkin.Core.Battle;
using Godot;

namespace Everkin.App;

/// <summary>
/// The inspection panel (ui_spec "Inspection panel"): the card front and back side by side, the statuses with their time left and source.
/// The board stays visible behind it; tapping the panel or the key closes it.
/// </summary>
public partial class InspectionPanel : Control
{
    public static InspectionPanel Open(Control parent, BattleState state, int unitId)
    {
        var panel = new InspectionPanel();
        parent.AddChild(panel);
        Ui.Fill(panel);
        panel.Build(state, unitId);
        return panel;
    }

    public event Action? Closed;

    private void Build(BattleState state, int unitId)
    {
        MouseFilter = MouseFilterEnum.Stop;
        var u = state.GetUnit(unitId);
        var info = UnitCards.Of(state, u);
        var h = Math.Min(Size.Y * 0.8f, 760f);
        var w = h / 1.5f;

        var row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        row.AddThemeConstantOverride("separation", 20);
        foreach (var mode in new[] { CardMode.Front, CardMode.Back })
        {
            var card = new CardView { Mode = mode, CustomMinimumSize = new Vector2(w, h), MouseFilter = MouseFilterEnum.Ignore };
            card.Bind(info);
            row.AddChild(card);
        }

        var side = new VBoxContainer();
        side.CustomMinimumSize = new Vector2(380, 0);
        side.AddThemeConstantOverride("separation", 6);
        side.AddChild(Ui.Label(info.Name, 34, Ui.Gold, bold: true));
        side.AddChild(Ui.Label(info.ClassLine, 22, new Color("cdbd9a")));
        side.AddChild(Ui.Label($"HP {u.Hp}/{u.MaxHp}   Power {state.PowerOf(u.Id)}   Speed {u.EffectiveSpeed}" + (u.EffectiveSpeed != u.Speed ? $" (base {u.Speed})" : ""), 22));
        if (u.IsStealthed) side.AddChild(Ui.Label("Stealthed", 22, new Color("9fc7e8")));
        if (u.Ward > 0) side.AddChild(Ui.Label($"Ward {u.Ward}", 22, new Color("8fb8e8")));
        foreach (var s in u.StatusInstances.Values.Where(s => s.Id is not (StatusIds.SongHaste or StatusIds.FreeMomentum)))
        {
            var left = s.TimeLeft(state.Timeline.Now);
            var from = s.SourceUnitId is { } src ? $" · from {state.GetUnit(src).Name}" : "";
            side.AddChild(Ui.Label($"{Game.Content.Statuses.FirstOrDefault(x => x.Id == s.Id)?.Name ?? s.Id.Replace('_', ' ')}{(left is { } t ? $" · {t} left" : "")}{from}", 20, StatusRules.PolarityOf(s.Id) == Polarity.Harmful ? Ui.Bad : Ui.Ink));
        }
        side.AddChild(Ui.Label("Right-click or Esc to close", 16, new Color("7d725f")));
        row.AddChild(Ui.Panel(side));

        var center = new CenterContainer();
        center.AddChild(row);
        AddChild(center);
        Ui.Fill(center);
        center.MouseFilter = MouseFilterEnum.Ignore;
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.35f), MouseFilter = MouseFilterEnum.Ignore };
        AddChild(dim);
        MoveChild(dim, 0);
        Ui.Fill(dim);
    }

    public override void _GuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton { Pressed: true }) Close();
    }

    public override void _UnhandledKeyInput(InputEvent e)
    {
        if (e is InputEventKey { Pressed: true, Keycode: Key.Escape or Key.I }) { GetViewport().SetInputAsHandled(); Close(); }
    }

    private void Close()
    {
        Closed?.Invoke();
        QueueFree();
    }
}
