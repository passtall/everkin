using System;
using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Setup;
using Godot;

namespace Everkin.App;

/// <summary>The world map (M5.2, ui_spec "Campaign map"): the tutorial and the four chapters in their fixed order; chapters not yet reached stay dark.</summary>
public partial class WorldMapScreen : Control, IBackNavigable
{
    public bool GoBack() { _main.Show("menu"); return true; }

    private readonly Main _main;

    public WorldMapScreen(Main main) => _main = main;

    public override void _Ready()
    {
        var bg = new TextureRect { Texture = Game.Tex("res://assets/ui/menu-background.png"), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered };
        AddChild(bg);
        Ui.Fill(bg);
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.35f), MouseFilter = MouseFilterEnum.Ignore };
        AddChild(dim);
        Ui.Fill(dim);

        var top = new HBoxContainer();
        top.AddThemeConstantOverride("separation", 14);
        var back = Ui.Button("Back", 22);
        back.Pressed += () => _main.Show("menu");
        top.AddChild(back);
        top.AddChild(Ui.Label("World map", 34, Ui.Gold, bold: true));
        top.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        top.AddChild(Ui.Label("Coins: offline", 22, new Color("cdbd9a")));
        var panel = Ui.Panel(top);
        panel.Position = new Vector2(20, 12);
        panel.CustomMinimumSize = new Vector2(1000, 0);
        AddChild(panel);

        var chapters = new[] { Game.Campaign.Tutorial }.Concat(Game.Campaign.Story).ToList();
        var row = new HBoxContainer { Alignment = BoxContainer.AlignmentMode.Center };
        row.AddThemeConstantOverride("separation", 28);
        foreach (var ch in chapters)
        {
            var open = Profile.ChapterOpen(ch);
            var done = ch.Encounters.Count(e => Profile.IsDone(e.Id));
            var box = new VBoxContainer();
            box.AddThemeConstantOverride("separation", 6);
            var biome = ch.Biomes.FirstOrDefault() ?? "ruins";
            var pic = new TextureRect
            {
                Texture = Game.Tex($"res://assets/backgrounds/battle/{biome}.png"), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered, CustomMinimumSize = new Vector2(280, 200), Modulate = open ? Colors.White : new Color(0.25f, 0.25f, 0.3f),
            };
            box.AddChild(pic);
            box.AddChild(Ui.Label(ch.Id == "T" ? "Tutorial" : $"Chapter {ch.Id}", 20, new Color("cdbd9a")));
            box.AddChild(Ui.Label(ch.Name, 24, open ? Ui.Gold : new Color("7d725f"), bold: true));
            box.AddChild(Ui.Label($"{done} / {ch.Encounters.Count}", 20, Ui.Ink));
            var go = Ui.Button(open ? (done == ch.Encounters.Count ? "Visit" : "Go") : "Locked", 22);
            go.Disabled = !open;
            var chapter = ch;
            go.Pressed += () => _main.Open(new ChapterMapScreen(_main, chapter));
            box.AddChild(go);
            row.AddChild(Ui.Panel(box));
        }
        var center = new CenterContainer { MouseFilter = MouseFilterEnum.Ignore };
        center.AddChild(row);
        AddChild(center);
        Ui.Fill(center);
    }
}

/// <summary>
/// One stage's map: the encounters on a path in their fixed order; the next one highlighted, won ones marked, later ones dark, the boss larger. Selecting
/// one opens a panel with the reward, the enemy team, the team to fight with and Fight (ui_spec "Campaign map").
/// </summary>
public partial class ChapterMapScreen : Control, IBackNavigable
{
    public bool GoBack() { _main.Open(new WorldMapScreen(_main)); return true; }

    private readonly Main _main;
    private readonly ChapterDef _chapter;
    private EncounterDef _selected;
    private Control? _panel;

    public ChapterMapScreen(Main main, ChapterDef chapter)
    {
        _main = main;
        _chapter = chapter;
        _selected = Profile.NextOf(chapter) ?? chapter.Encounters[^1];
    }

    /// <summary>The challenge encounter that sits on this chapter's map after the story (C1 on chapter 1, and so on).</summary>
    private EncounterDef? Challenge => _chapter.Id is "1" or "2" or "3" or "4" && Profile.StoryComplete
        ? Game.Campaign.Challenges.Encounters[int.Parse(_chapter.Id) - 1] : null;

    public override void _Ready()
    {
        var biome = _chapter.Biomes.FirstOrDefault() ?? "ruins";
        var bg = new TextureRect { Texture = Game.Tex($"res://assets/backgrounds/battle/{biome}.png"), ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize, StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered };
        AddChild(bg);
        Ui.Fill(bg);
        var dim = new ColorRect { Color = new Color(0, 0, 0, 0.4f), MouseFilter = MouseFilterEnum.Ignore };
        AddChild(dim);
        Ui.Fill(dim);

        var top = new HBoxContainer();
        top.AddThemeConstantOverride("separation", 14);
        var back = Ui.Button("World map", 22);
        back.Pressed += () => _main.Open(new WorldMapScreen(_main));
        top.AddChild(back);
        var title = Ui.Button("Title", 22);
        title.Pressed += () => _main.Show("menu");
        top.AddChild(title);
        top.AddChild(Ui.Label(_chapter.Name, 34, Ui.Gold, bold: true));
        top.AddChild(new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill });
        var all = new[] { Game.Campaign.Tutorial }.Concat(Game.Campaign.Story).ToList();
        var index = all.IndexOf(_chapter);
        if (index > 0 && Profile.ChapterOpen(all[index - 1]))
        {
            var prev = Ui.Button("◀ " + all[index - 1].Name, 20);
            prev.Pressed += () => _main.Open(new ChapterMapScreen(_main, all[index - 1]));
            top.AddChild(prev);
        }
        if (index >= 0 && index + 1 < all.Count && Profile.ChapterOpen(all[index + 1]))
        {
            var next = Ui.Button(all[index + 1].Name + " ▶", 20);
            next.Pressed += () => _main.Open(new ChapterMapScreen(_main, all[index + 1]));
            top.AddChild(next);
        }
        top.AddChild(Ui.Label("Coins: offline", 20, new Color("cdbd9a")));
        var topPanel = Ui.Panel(top);
        topPanel.Position = new Vector2(20, 12);
        topPanel.CustomMinimumSize = new Vector2(1500, 0);
        AddChild(topPanel);

        // The path: encounters from left to right on a gentle wave.
        var nodes = _chapter.Encounters.ToList();
        if (Challenge is { } c) nodes.Add(c);
        for (var i = 0; i < nodes.Count; i++)
        {
            var e = nodes[i];
            var open = Profile.EncounterOpen(e);
            var done = Profile.IsDone(e.Id);
            var isNext = Profile.NextOf(_chapter) == e || (e.Id.StartsWith('C') && !done);
            var size = e.IsBoss ? 96f : e.Id.StartsWith('C') ? 84f : 72f;
            var pos = new Vector2(140 + i * 150, 330 + (float)Math.Sin(i * 1.2) * 70);
            var node = new Button { Text = done ? "✓" : e.Id.Replace("T", ""), Position = pos - new Vector2(size / 2, size / 2), CustomMinimumSize = new Vector2(size, size), Size = new Vector2(size, size) };
            node.AddThemeFontOverride("font", Game.BodyBold());
            node.AddThemeFontSizeOverride("font_size", e.IsBoss ? 28 : 24);
            var bgColor = done ? new Color("4a3a1c") : isNext ? new Color("5a3a1c") : open ? new Color("2a2118") : new Color("0e0b09");
            var border = isNext ? Colors.White : done ? Ui.Gold : open ? Ui.Bronze : new Color("3a3228");
            node.AddThemeStyleboxOverride("normal", Ui.Box(bgColor, border, isNext ? 5 : 3, (int)(size / 2)));
            node.AddThemeStyleboxOverride("hover", Ui.Box(bgColor.Lightened(0.15f), border, 4, (int)(size / 2)));
            node.AddThemeStyleboxOverride("pressed", Ui.Box(bgColor, Colors.White, 4, (int)(size / 2)));
            node.AddThemeColorOverride("font_color", open ? Ui.Ink : new Color("5a4f40"));
            node.TooltipText = open ? e.Name : "Locked";
            node.Disabled = !open;
            var enc = e;
            node.Pressed += () => { _selected = enc; ShowPanel(); };
            if (i > 0)
            {
                var prevPos = new Vector2(140 + (i - 1) * 150, 330 + (float)Math.Sin((i - 1) * 1.2) * 70);
                var line = new Line2D { Width = 6, DefaultColor = done || Profile.IsDone(nodes[i - 1].Id) ? Ui.Gold : new Color("5a4a36"), Points = new[] { prevPos, pos } };
                AddChild(line);
            }
            AddChild(node);
            AddChild(Label(e.Name, pos + new Vector2(-70, size / 2 + 4), open));
        }
        ShowPanel();
    }

    private static Label Label(string text, Vector2 at, bool open)
    {
        var l = Ui.Label(text, 18, open ? Ui.Ink : new Color("7d725f"), align: HorizontalAlignment.Center);
        l.Position = at;
        l.CustomMinimumSize = new Vector2(140, 0);
        l.Size = new Vector2(140, 50);
        l.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        l.AddThemeColorOverride("font_outline_color", Colors.Black);
        l.AddThemeConstantOverride("outline_size", 5);
        return l;
    }

    private void ShowPanel()
    {
        _panel?.QueueFree();
        var e = _selected;
        var done = Profile.IsDone(e.Id);
        var revealed = done || Profile.NextOf(_chapter) == e || e.Id.StartsWith('C');          // locked ones show only the reward
        var box = new VBoxContainer();
        box.AddThemeConstantOverride("separation", 8);
        box.AddChild(Ui.Label($"{e.Id}  {e.Name}", 30, Ui.Gold, bold: true));
        box.AddChild(Ui.Label($"AI: {e.Ai}", 18, new Color("cdbd9a")));

        box.AddChild(Ui.Label("Reward", 22, Ui.Ink, bold: true));
        var rewardRow = new HBoxContainer();
        rewardRow.AddThemeConstantOverride("separation", 8);
        foreach (var r in e.Rewards)
        {
            var info = CardInfo.FromCreature(Game.Content.CreaturesById[r]);
            var card = new CardView { Mode = CardMode.Front, CustomMinimumSize = new Vector2(110, 165), Size = new Vector2(110, 165), TooltipText = info.Name };
            card.Bind(info);
            card.RightClicked += _ => { };
            rewardRow.AddChild(card);
        }
        if (done) rewardRow.AddChild(Ui.Label("received", 18, Ui.Good));
        box.AddChild(rewardRow);

        if (revealed)
        {
            box.AddChild(Ui.Label("Enemy team", 22, Ui.Ink, bold: true));
            foreach (var row in new[] { "Front", "Middle", "Rear" })
            {
                var units = e.Team.Where(u => u.Row == row).ToList();
                if (units.Count == 0) continue;
                var h = new HBoxContainer();
                h.AddThemeConstantOverride("separation", 4);
                foreach (var u in units)
                {
                    var def = Game.Content.CreaturesById[u.Creature];
                    var info = CardInfo.FromCreature(def);
                    if (u.Boss) { info.MaxHp = info.Hp = BattleSetup.BossHp(def.Hp); info.Power = info.BasePower = def.Power + BattleSetup.BossPowerBonus; }
                    var card = new CardView { Mode = CardMode.Compact, CustomMinimumSize = new Vector2(70, 105), Size = new Vector2(70, 105), TooltipText = def.Name + (u.Boss ? " (boss)" : "") };
                    card.Bind(info);
                    h.AddChild(card);
                }
                box.AddChild(h);
            }
        }
        else box.AddChild(Ui.Label("Win the earlier encounters to see this team.", 18, new Color("7d725f")));

        if (Profile.EncounterOpen(e))
        {
            var tutorial = _chapter.Id == "T";
            box.AddChild(Ui.Label("Your team", 22, Ui.Ink, bold: true));
            var team = tutorial ? Profile.TutorialTeam() : Profile.CurrentTeam;
            var teamRow = new HBoxContainer();
            teamRow.AddThemeConstantOverride("separation", 8);
            if (tutorial) teamRow.AddChild(Ui.Label($"Tutorial team ({team.Units.Count} creatures)", 20));
            else
            {
                var pick = new OptionButton();
                pick.AddThemeFontSizeOverride("font_size", 20);
                foreach (var t in Profile.Data.Teams) pick.AddItem((t.Units.Count < BattleSetup.TeamSize ? "⚠ " : "") + t.Name);
                pick.Selected = Profile.Data.Selected;
                pick.ItemSelected += i => { Profile.Data.Selected = (int)i; Profile.Save(); };
                teamRow.AddChild(pick);
            }
            var edit = Ui.Button("Edit team", 20);
            edit.Pressed += () => _main.Open(new TeamEditor(_main) { Back = () => _main.Open(new ChapterMapScreen(_main, _chapter)), TutorialTeam = tutorial });
            teamRow.AddChild(edit);
            box.AddChild(teamRow);
            var fight = Ui.Button(done ? "Replay" : "Fight", 28);
            fight.Disabled = team.Units.Count == 0;
            fight.Pressed += () => Fight(e, team);
            box.AddChild(fight);
            if (done) box.AddChild(Ui.Label("The reward was already received.", 16, new Color("7d725f")));
        }

        var panel = Ui.Panel(box);
        panel.Position = new Vector2(Size.X > 0 ? Size.X - 560 : 1340, 100);
        panel.CustomMinimumSize = new Vector2(520, 0);
        _panel = panel;
        AddChild(panel);
    }

    private void Fight(EncounterDef e, TeamData team)
    {
        var seed = (ulong)new Random().NextInt64(1, 1_000_000_000);
        _main.StartBattle(new BattleSpec(seed, Profile.ToSetup(team), e.EnemyTeam(Game.Content)), e.Ai, e.Id);
    }
}
