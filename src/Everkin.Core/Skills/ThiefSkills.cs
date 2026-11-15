using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Thief (skills.md): steals statuses and skills. Physical.</summary>
public static class ThiefSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new MeltAway(D("melt_away"));
        yield return new LiftASkill(D("lift_a_skill"));
        yield return new Snatch(D("snatch"));
        yield return new EscapeArtist(D("escape_artist"));
        yield return new MasterThief(D("master_thief"));
        yield return new ShadowPurse(D("shadow_purse"));
    }
}

/// <summary>Melt Away: the Thief gains stealth.</summary>
public sealed class MeltAway : ClassSkill
{
    public MeltAway(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override string? Unavailable(BattleState state, int userId) => Stealth.CanGain(state, userId) ? null : "cannot gain stealth now";

    public override void Resolve(SkillContext context)
    {
        Stealth.Gain(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>
/// Lift a Skill: use a copy of the last active skill the target used, with the Thief's own Power. The player picks any unit, ally or enemy, and, if the copy
/// needs a target, a legal target (the second target). The copy is free. Any skill used with a turn action counts, unique attacks included; Move, Skip Turn,
/// keystones and Lift a Skill itself never do. Only copies that need no target or one unit target are possible. A magical copy cannot be made while the
/// Thief is Silenced; a melee copy follows the Front-row rule.
/// </summary>
public sealed class LiftASkill : ClassSkill
{
    public LiftASkill(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    private static Skill? Copied(BattleState state, SkillCatalog catalog, int targetId) =>
        state.GetUnit(targetId).LastActiveSkillId is { } id && id != "lift_a_skill" && catalog.TryGet(id, out var skill)
            && !skill.IsPassive && skill.Target is TargetKind.None or TargetKind.Unit ? skill : null;

    public override bool TargetFilter(BattleState state, int userId, int targetId) => state.Units.Count > targetId;

    public override SecondTargetChoice? SecondTargetFor(BattleState state, SkillCatalog catalog, int userId, int firstId)
    {
        if (Copied(state, catalog, firstId) is not { Target: TargetKind.Unit } copy) return null;
        var units = state.Units.Where(u => !u.IsKnockedOut && Actions.CanUseFree(state, catalog, new SkillUse(userId, copy.Id, u.Id))).Select(u => u.Id).ToArray();
        return new SecondTargetChoice(units, Required: true);
    }

    public override string? CheckUse(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        if (use.TargetUnitId is not { } who || who < 0 || who >= state.Units.Count) return null;                 // the target checks report it
        if (Copied(state, catalog, who) is not { } copy) return "the target has no skill that can be copied";
        if (copy.Magical && !Statuses.CanUseMagical(state.GetUnit(use.UserId))) return "Silenced: the copied skill is magical";
        if (copy.NeedsFrontRow && !Targeting.InFrontRow(state, use.UserId)) return "the copied skill needs the Front row";
        if (copy.Target != TargetKind.Unit) return null;
        if (use.SecondTargetUnitId is not { } second || second < 0 || second >= state.Units.Count) return "the copied skill needs a target";
        return Actions.CanUseFree(state, catalog, new SkillUse(use.UserId, copy.Id, second)) ? null : "not a legal target for the copied skill";
    }

    public override void Resolve(SkillContext context)
    {
        // Confusion may have aimed the skill at another unit than the one that was checked: then there may be nothing to copy (the skill is spent).
        if (Copied(context.State, context.Catalog!, context.TargetId!.Value) is not { } copy) { context.Settle(); return; }
        var second = copy.Target == TargetKind.Unit ? context.SecondTargetId : null;
        var use = new SkillUse(context.UserId, copy.Id, second);
        if (copy.Target == TargetKind.Unit && second is null || !Actions.CanUseFree(context.State, context.Catalog!, use)) { context.Settle(); return; }
        Actions.UseFree(context.State, context.Catalog!, use);
    }
}

/// <summary>Snatch: hit for 5-6 and steal one helpful status from the target (every one with Master Thief). The target must be hit itself, not an interceptor.</summary>
public sealed class Snatch : ClassSkill
{
    public Snatch(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, new IntRange(5, 6));
        if (context.Over || report.Outcome is HitOutcome.Missed or HitOutcome.Intercepted || report.Interceptions.Count > 0) return;
        var target = context.State.GetUnit(report.TargetId);
        if (target.IsKnockedOut || target.Side == context.UserSide) return;
        Statuses.Steal(context.State, context.UserId, report.TargetId, all: context.User.KnownSkills.Contains("master_thief"));
        context.Settle();
    }
}

/// <summary>Escape Artist: the first time the Thief drops below half HP, it gains stealth.</summary>
public sealed class EscapeArtist : KeystoneSkill
{
    public EscapeArtist(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAfterDamageHook((st, targetId, before, after) =>
        {
            var owner = st.GetUnit(ownerId);
            if (targetId != ownerId || owner.IsKnockedOut || before * 2 < owner.MaxHp || after * 2 >= owner.MaxHp) return;
            if (!st.ConsumeOnce("escape_artist:" + ownerId)) return;
            if (Stealth.CanGain(st, ownerId)) Stealth.Gain(st, ownerId);
        });
}

/// <summary>Master Thief: Snatch and the Thief's own attack steal every helpful status from the target, not just one. Read by Snatch and the unique attacks.</summary>
public sealed class MasterThief : KeystoneSkill
{
    public MasterThief(SkillDef def) : base(def) { }
}

/// <summary>Shadow Purse: each time the Thief steals a status, Momentum moves 1 toward your side (every steal by the Thief counts, its own attack included).</summary>
public sealed class ShadowPurse : KeystoneSkill
{
    public ShadowPurse(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStealHook((st, thiefId, fromId, count) =>
        {
            if (thiefId == ownerId && !st.GetUnit(ownerId).IsKnockedOut) st.ShiftMomentum(st.GetUnit(ownerId).Side, count);
        });
}
