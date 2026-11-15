using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

/// <summary>Shared helpers for the class skill tests: use a skill, read a status, advance the clock, set up a small board.</summary>
internal static class K
{
    public static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null, MoveOption? place = null, int? second = null) =>
        new(user, skill, target, row, place, second);

    public static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    public static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    public static bool Has(BattleState s, int unit, string status) => Statuses.Has(s.GetUnit(unit), status);

    public static int Lost(BattleState s, int unit) => s.GetUnit(unit).MaxHp - s.GetUnit(unit).Hp;

    /// <summary>The unit has no turn on the timeline (tests that read a due time start from a clean slate).</summary>
    public static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    /// <summary>Moves the clock to a time, ending every status and effect due on the way.</summary>
    public static void AdvanceTo(BattleState s, int time)
    {
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            Statuses.HandleExpiry(s, e);
        }
    }

    /// <summary>
    /// A 40 HP unit in the Front with the given skills, an ally in the Middle, an enemy in the Front, the passives installed and the side ahead by 6
    /// in Momentum (so Momentum costs are payable).
    /// </summary>
    public static (BattleState S, int User, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var u = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, u, enemy);
    }

    /// <summary>Every skill of the class is implemented with the Delay, Momentum and magical flag of the content.</summary>
    public static void AssertClassImplemented(string classId, bool magical)
    {
        var cls = World.Content.ClassesById[classId];
        foreach (var id in cls.Actives.Concat(cls.Keystones))
        {
            var def = World.Content.SkillsById[id];
            var skill = World.Catalog.Get(id);
            Assert.Equal(def.Kind == "K", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            Assert.Equal(magical && def.Kind == "A", skill.Magical);
        }
    }
}
