using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Saboteur (skills.md): blinds, delays and breaks plans. Physical.</summary>
public static class SaboteurSkills
{
    public const int BlindLength = 20;
    public const int JamDelay = 5;
    public const int ChaosPush = 2;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new FlashPowder(D("flash_powder"));
        yield return new CutTheStrings(D("cut_the_strings"));
        yield return new Jam(D("jam"));
        yield return new Wrecker(D("wrecker"));
        yield return new ChaosAgent(D("chaos_agent"));
        yield return new DisarmTrap(D("disarm_trap"));
    }

    /// <summary>The auto events a unit owns on the timeline, soonest first.</summary>
    internal static IReadOnlyList<TimelineEntry> AutoEventsOf(BattleState state, int unitId) =>
        state.Timeline.Upcoming().Where(e => e.Kind == TimelineKind.AutoEvent && e.OwnerUnitId == unitId).ToArray();
}

/// <summary>Flash Powder: every enemy in melee reach is Blind for 20.</summary>
public sealed class FlashPowder : ClassSkill
{
    public FlashPowder(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override bool IsArea => true;
    public override bool NeedsFrontRow => true;

    public override void Resolve(SkillContext context)
    {
        foreach (var id in Targeting.EnemiesInMeleeReach(context.State, context.UserId))
            Statuses.Apply(context.State, new StatusApplication(id, StatusIds.Blind, SaboteurSkills.BlindLength, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Cut the Strings: pick an enemy that owns auto events: its next auto event is removed from the timeline.</summary>
public sealed class CutTheStrings : ClassSkill
{
    public CutTheStrings(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);
    public override bool TargetFilter(BattleState state, int userId, int targetId) => SaboteurSkills.AutoEventsOf(state, targetId).Count > 0;

    public override void Resolve(SkillContext context)
    {
        if (SaboteurSkills.AutoEventsOf(context.State, context.TargetId!.Value).FirstOrDefault() is { } next)
            context.State.Timeline.Remove(next.Id);
        context.Settle();
    }
}

/// <summary>Jam: a unit's next skill has +5 Delay. An interceptor takes the Delay instead.</summary>
public sealed class Jam : ClassSkill
{
    public Jam(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var receiver = context.InterceptEffect(context.TargetId!.Value);
        if (!context.State.GetUnit(receiver).IsKnockedOut)
            Statuses.Apply(context.State, new StatusApplication(receiver, StatusIds.NextSkillDelay, Magnitude: SaboteurSkills.JamDelay, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Wrecker: the Saboteur deals double damage to constructs, totems, traps and barricades.</summary>
public sealed class Wrecker : KeystoneSkill
{
    public Wrecker(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddDamageBonus((st, attacker, target) => attacker == ownerId && st.GetUnit(target).Kind == UnitKind.Construct ? 100 : 0);
}

/// <summary>Chaos Agent: every attack of the Saboteur that hits an enemy also pushes its next turn back 2 (attacks only: Flash Powder and Jam do not).</summary>
public sealed class ChaosAgent : KeystoneSkill
{
    public ChaosAgent(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddTargetHitHook((st, attackerId, targetId) =>
        {
            if (attackerId == ownerId && st.GetUnit(targetId).Side != st.GetUnit(ownerId).Side && !st.GetUnit(targetId).IsKnockedOut)
                st.PushTurnBack(targetId, SaboteurSkills.ChaosPush);
        });
}

/// <summary>Disarm Trap: when the Saboteur hits an enemy trap, the trap goes off and is removed, but its effect does not reach the Saboteur. The traps read this keystone.</summary>
public sealed class DisarmTrap : KeystoneSkill
{
    public DisarmTrap(SkillDef def) : base(def) { }
}
