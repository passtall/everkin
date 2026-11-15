using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Spiritwalker (skills.md): possession and soul swaps. Magical.</summary>
public static class SpiritwalkerSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new PuppetSoul(D("puppet_soul"));
        yield return new LifeSwap(D("life_swap"));
        yield return new ForcedStep(D("forced_step"));
        yield return new SoulAnchor(D("soul_anchor"));
        yield return new Haunting(D("haunting"));
        yield return new Puppeteer(D("puppeteer"));
    }

    /// <summary>Soul Anchor: enemies cannot possess, move or soul-swap the Spiritwalker.</summary>
    internal static bool Anchored(BattleState state, int userId, int targetId) =>
        state.GetUnit(targetId).KnownSkills.Contains("soul_anchor") && !state.GetUnit(targetId).IsKnockedOut && state.GetUnit(targetId).Side != state.GetUnit(userId).Side;
}

/// <summary>
/// Puppet Soul: possession. A unit hits a unit next to it at once with its unique attack. The player picks the unit and one unit next to it. The hit is free and uses
/// the first unit's Power. It ignores the Front-row rule, but a melee attack still cannot hit an airborne unit.
/// </summary>
public sealed class PuppetSoul : ClassSkill
{
    public PuppetSoul(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.TwoUnits;

    public override bool TargetFilter(BattleState state, int userId, int targetId) => !SpiritwalkerSkills.Anchored(state, userId, targetId);

    public override bool PairFilter(BattleState state, int userId, int firstId, int secondId) =>
        state.Formation.Adjacent(firstId).Contains(secondId) && !state.GetUnit(secondId).IsKnockedOut;

    public override string? CheckUse(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        if (use.TargetUnitId is not { } first || use.SecondTargetUnitId is not { } second) return null;
        if (TacticianSkills.AttackOf(state, catalog, first) is not { } attack) return "the unit has no unique attack";
        if (state.GetUnit(second).IsAirborne && catalog.Get(attack).Delivery == Battle.Delivery.Melee && !state.GetUnit(first).IsAirborne) return "a melee attack cannot hit an airborne unit";
        return null;
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var first = context.TargetId!.Value;
        var second = context.SecondTargetId!.Value;
        if (TacticianSkills.AttackOf(state, context.Catalog!, first) is not { } attackId || context.Catalog!.Get(attackId) is not UniqueAttackSkill attack) { context.Settle(); return; }
        state.Emit(new SkillUsed(first, attackId, second));
        state.PuppeteerId = context.UserId;
        try
        {
            if (!state.GetUnit(first).IsKnockedOut) attack.ResolveOn(new SkillContext(state, attack, first, new SkillUse(first, attackId, second), second) { Catalog = context.Catalog }, second);
        }
        finally { state.PuppeteerId = null; }
        context.Settle();
    }
}

/// <summary>Life Swap: soul swap. Two units swap their current HP (each capped at its max).</summary>
public sealed class LifeSwap : ClassSkill
{
    public LifeSwap(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override bool TargetFilter(BattleState state, int userId, int targetId) => !SpiritwalkerSkills.Anchored(state, userId, targetId);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var a = state.GetUnit(context.TargetId!.Value);
        var b = state.GetUnit(context.SecondTargetId!.Value);
        var (hpA, hpB) = (a.Hp, b.Hp);
        a.Hp = Math.Min(a.MaxHp, hpB);
        b.Hp = Math.Min(b.MaxHp, hpA);
        state.Emit(new HpSwapped(a.Id, b.Id, a.Hp, b.Hp));
        context.Settle();
    }
}

/// <summary>Forced Step: possession. A unit moves to a free spot of your choice on its own side. A forced move: Immobilized units and Hold Position stop it.</summary>
public sealed class ForcedStep : ClassSkill
{
    public ForcedStep(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override EffectTags Tags => base.Tags | EffectTags.Move;

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        !SpiritwalkerSkills.Anchored(state, userId, targetId) && Movement.IsMovable(state.GetUnit(targetId))
        && !Movement.IsImmune(state, targetId, state.GetUnit(userId).Side) && PlacementOptionsFor(state, userId, targetId).Count > 0;

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId)
    {
        var from = state.Formation.PositionOf(targetId);
        var result = new List<MoveOption>();
        for (var r = 0; r < Formation.RowCount; r++)
        {
            if ((Row)r == from.Row) continue;
            var count = state.Formation.Count(from.Side, (Row)r);
            if (count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++) result.Add(new MoveOption((Row)r, gap));
        }
        return result;
    }

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        var from = context.State.Formation.PositionOf(target);
        var place = context.Placement!.Value;
        Movement.Move(context.State, target, place.Row, place.Index);
        context.AfterForcedMove(new ForcedMoveResult(ForcedMoveStatus.Moved, new[] { new UnitMoved(target, from, context.State.Formation.PositionOf(target), MoveCause.Forced) }, Array.Empty<int>()));
    }
}

/// <summary>Soul Anchor: enemies cannot possess, move or soul-swap the Spiritwalker. The skills read this keystone; pushes and swaps are stopped here.</summary>
public sealed class SoulAnchor : KeystoneSkill
{
    public SoulAnchor(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddForcedMoveImmunity((st, unitId, causedBy) => unitId == ownerId && causedBy != st.GetUnit(ownerId).Side);
}

/// <summary>Haunting: when the Spiritwalker is KO, the unit that KO'd it is Confused for 20.</summary>
public sealed class Haunting : KeystoneSkill
{
    public const int Length = 20;
    public Haunting(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            if (ctx.Trigger.SourceUnitId is { } killer && !st.GetUnit(killer).IsKnockedOut)
                Statuses.Apply(st, new StatusApplication(killer, StatusIds.Confused, Length, SourceUnitId: ownerId));
        })
        {
            Condition = (st, t) => t.SubjectUnitId == ownerId && t.SourceUnitId is { } src && src != ownerId,
        });
}

/// <summary>Puppeteer: Puppet Soul hits deal double damage.</summary>
public sealed class Puppeteer : KeystoneSkill
{
    public Puppeteer(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddDamageBonus((st, attackerId, targetId) => st.PuppeteerId == ownerId ? 100 : 0);
}
