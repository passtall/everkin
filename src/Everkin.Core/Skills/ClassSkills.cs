using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// A class active or keystone (skills.md): Delay and Momentum cost come from the content (a keystone has none), the delivery too. Whether the skill
/// is magical comes from its class (class_system.md section 2), so each class file passes it on.
/// </summary>
public abstract class ClassSkill : Skill
{
    private readonly Delivery? _delivery;
    private readonly bool _magical;

    protected ClassSkill(SkillDef def, bool magical = false) : base(def.Id, def.Delay ?? 0, def.Momentum ?? 0)
    {
        Def = def;
        _magical = magical;
        _delivery = def.Delivery switch
        {
            "melee" => Battle.Delivery.Melee,
            "projectile" => Battle.Delivery.Projectile,
            "direct" => Battle.Delivery.Direct,
            _ => null,
        };
    }

    public SkillDef Def { get; }

    public override Delivery? Delivery => _delivery;
    public override bool Magical => _magical;

    public override EffectTags Tags =>
        (_delivery switch { Battle.Delivery.Melee => EffectTags.Melee, Battle.Delivery.Projectile => EffectTags.Projectile, Battle.Delivery.Direct => EffectTags.Direct, _ => EffectTags.None })
        | (_magical ? EffectTags.Magical : EffectTags.Physical);
}

/// <summary>A keystone: a passive, always on; it does its work in <see cref="Skill.Install"/> (class_system.md section 1).</summary>
public abstract class KeystoneSkill : ClassSkill
{
    protected KeystoneSkill(SkillDef def, bool magical = false) : base(def, magical) { }
    public sealed override bool IsPassive => true;
    public sealed override TargetKind Target => TargetKind.None;
    public sealed override void Resolve(SkillContext context) => throw new InvalidOperationException($"{Id} is a keystone.");
}

/// <summary>Every implemented class skill; the catalog adds them to the unique attacks.</summary>
public static class ClassSkills
{
    public static IEnumerable<Skill> All(ContentSet content) =>
        BerserkerSkills.All(content)
            .Concat(ChampionSkills.All(content))
            .Concat(BulwarkSkills.All(content))
            .Concat(KeeperSkills.All(content))
            .Concat(MenderSkills.All(content))
            .Concat(ClericSkills.All(content))
            .Concat(AssassinSkills.All(content))
            .Concat(TrackerSkills.All(content));
}

/// <summary>Small helpers the class skills share.</summary>
internal static class ClassHelpers
{
    /// <summary>Every row of either side: a skill that picks a row may pick any of them, even one that looks empty (game_system.md section 8.1).</summary>
    public static IReadOnlyList<RowTarget> AllRows() =>
        new[] { Side.A, Side.B }.SelectMany(side => new[] { Row.Front, Row.Middle, Row.Rear }.Select(row => new RowTarget(side, row))).ToArray();

    /// <summary>Picks one of several by the battle's seeded roll, each with equal chance; a single choice costs no roll.</summary>
    public static T Pick<T>(BattleState state, IReadOnlyList<T> options) =>
        options.Count == 1 ? options[0] : options[state.Random.NextInt(options.Count)];

    /// <summary>The helpful statuses a unit has: its Ward and every status of helpful polarity (stealth is a flag and is handled apart).</summary>
    public static IReadOnlyList<string> HelpfulStatuses(Unit u)
    {
        var list = u.StatusInstances.Keys.Where(id => StatusRules.PolarityOf(id) == Polarity.Helpful).ToList();
        if (u.Ward > 0 && !list.Contains(StatusIds.Ward)) list.Add(StatusIds.Ward);
        return list;
    }
}

/// <summary>Marks (game_system.md section 10.5, skills.md Tracker and Champion): only enemies can be Marked; a skill that reads a Mark works on any Marked unit.</summary>
public static class Marks
{
    /// <summary>Marks a unit for the marker. The marker has one Mark from its skills at a time: an earlier one is removed. A null duration is no time limit (Relentless).</summary>
    public static void Place(BattleState state, int markerId, int targetId, int? duration = Statuses.MarkLength)
    {
        if (state.GetUnit(markerId).KnownSkills.Contains("relentless")) duration = null;     // Relentless: the Tracker's Marks have no time limit
        foreach (var unit in state.Units.ToArray())
            if (unit.Id != targetId && Statuses.Get(unit, StatusIds.Marked) is { } m && m.SourceUnitId == markerId)
                Statuses.Remove(state, unit.Id, StatusIds.Marked, StatusEndCause.Consumed);
        Statuses.Apply(state, new StatusApplication(targetId, StatusIds.Marked, duration, SourceUnitId: markerId));
    }

    /// <summary>The unit is Marked, whoever placed the Mark, and is an enemy of the user.</summary>
    public static bool IsMarkedEnemy(BattleState state, int userId, int targetId)
    {
        var t = state.GetUnit(targetId);
        return t.Side != state.GetUnit(userId).Side && Statuses.Has(t, StatusIds.Marked);
    }

    /// <summary>The unit is Marked by this unit.</summary>
    public static bool IsMarkedBy(BattleState state, int markerId, int targetId) =>
        Statuses.Get(state.GetUnit(targetId), StatusIds.Marked)?.SourceUnitId == markerId;
}
