using Godot;
using Everkin.Core;

namespace Everkin;

/// <summary>M0.3 export test: proves C# runs and the core assembly is reachable on every platform.</summary>
public partial class Boot : Label
{
    public override void _Ready()
    {
        Text = $"Everkin boot OK\ncore: {CoreInfo.Name}\n.NET {System.Environment.Version}";
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        SetAnchorsPreset(Control.LayoutPreset.FullRect);
        AddThemeFontSizeOverride("font_size", 48);
        GD.Print(Text);
    }
}
