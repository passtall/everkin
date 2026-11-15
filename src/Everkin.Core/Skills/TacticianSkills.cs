using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Tactician (skills.md): the timeline and Momentum. Physical.</summary>
public static class TacticianSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new SwapTurns(D("swap_turns"));
        yield return new PressTheAdvantage(D("press_the_advantage"));
        yield return new CoordinatedStrike(D("coordinated_strike"));
        yield return new AllIn(D("all_in"));
        yield return new InspiringPresence(D("inspiring_presence"));
        yield return new Initiative(D("initiative"));
    }

    /// <summary>The skill ID of the unit's own attack: the creature's unique attack, or the hit of a golem or blades.</summary>
    internal static string? AttackOf(BattleState state, SkillCatalog catalog, int unitId)
    {
        var unit = state.GetUnit(unitId);
        if (unit.AutoSkillId is { } auto) return auto;
        return unit.CreatureId is { } cid && catalog.Content.CreaturesById.TryGetValue(cid, out var def) ? def.Attack : null;
    }
}

/// <summary>Swap Turns: own side only. Two allies swap their places on the timeline (they exchange the due times and tie order of their next turns).</summary>
public sealed class SwapTurns : ClassSkill
{
    public SwapTurns(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override TargetOptions Options => new(TargetSide.OwnSideOnly);

    public override void Resolve(SkillContext context)
    {
        var timeline = context.State.Timeline;
        if (timeline.TurnOf(context.TargetId!.Value) is { } a && timeline.TurnOf(context.SecondTargetId!.Value) is { } b)
            timeline.SwapDue(a, b);
        context.Settle();
    }
}

/// <summary>Press the Advantage: a unit's next skill costs no Momentum. It has no time limit and needs no lead.</summary>
public sealed class PressTheAdvantage : ClassSkill
{
    public PressTheAdvantage(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.FreeMomentum, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>
/// Coordinated Strike: pick an enemy. Two of your allies (not the Tactician) make their unique attack on it right now, picked at random among those whose
/// attack can hit it. If only one can, only it attacks; if none can, nothing happens. The attacks are free and each moves Momentum like a separate action.
/// </summary>
public sealed class CoordinatedStrike : ClassSkill
{
    public CoordinatedStrike(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        var candidates = state.Formation.Units(context.UserSide)
            .Where(id => id != context.UserId && TacticianSkills.AttackOf(state, context.Catalog!, id) is { } skill
                && Actions.CanUseFree(state, context.Catalog!, new SkillUse(id, skill, target))).ToList();
        for (var picks = 0; picks < 2 && candidates.Count > 0 && !context.Over; picks++)
        {
            var ally = ClassHelpers.Pick(state, candidates);
            candidates.Remove(ally);
            if (state.GetUnit(ally).IsKnockedOut || state.GetUnit(target).IsKnockedOut && state.Formation.Contains(target) is false) continue;
            var skill = TacticianSkills.AttackOf(state, context.Catalog!, ally)!;
            var use = new SkillUse(ally, skill, target);
            if (Actions.CanUseFree(state, context.Catalog!, use)) Actions.UseFree(state, context.Catalog!, use);
        }
        context.Settle();
    }
}

/// <summary>All In: the Tactician's Momentum costs are halved (rounded up), but Momentum gained from its hits is 0.</summary>
public sealed class AllIn : KeystoneSkill
{
    public AllIn(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddMomentumCostModifier((st, userId, cost) => userId == ownerId ? (cost + 1) / 2 : cost);
        state.AddMomentumShiftModifier((st, userId, shift) => userId == ownerId && shift > 0 ? 0 : shift);
    }
}

/// <summary>Inspiring Presence: when an ally next to the Tactician KOs an enemy with its own skill, Momentum moves 1 more toward your side.</summary>
public sealed class InspiringPresence : KeystoneSkill
{
    public InspiringPresence(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) => st.ShiftMomentum(st.GetUnit(ownerId).Side, 1))
        {
            Condition = (st, t) =>
            {
                var owner = st.GetUnit(ownerId);
                return t.SourceUnitId is { } killer && killer != ownerId && st.GetUnit(killer).Side == owner.Side
                    && st.GetUnit(t.SubjectUnitId).Side != owner.Side
                    && st.Formation.Contains(ownerId) && st.Formation.Contains(killer) && st.Formation.Adjacent(ownerId).Contains(killer);
            },
        });
}

/// <summary>Initiative: at battle start, every ally's first turn is pulled forward 5.</summary>
public sealed class Initiative : KeystoneSkill
{
    public const int Pull = 5;
    public Initiative(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        foreach (var id in state.Formation.Units(state.GetUnit(ownerId).Side).ToArray())
            if (!state.GetUnit(id).IsKnockedOut && state.Timeline.TurnOf(id) is { } turn)
                state.Timeline.PullForward(turn, Pull);
    }
}
