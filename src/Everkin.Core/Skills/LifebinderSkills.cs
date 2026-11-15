using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Lifebinder (skills.md): revives, paid for with its own life. Magical.</summary>
public static class LifebinderSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new BurnoutHeal(D("burnout_heal"));
        yield return new LifeForLife(D("life_for_life"));
        yield return new BoundResurrect(D("bound_resurrect"));
        yield return new Martyr(D("martyr"));
        yield return new Phoenix(D("phoenix"));
        yield return new Lifeblood(D("lifeblood"));
    }

    /// <summary>Whether a KO body or an undead can be brought back by a resurrection (sections 10.3, 11.5, Desecrate).</summary>
    internal static bool CanBeRevived(BattleState state, int unitId)
    {
        var u = state.GetUnit(unitId);
        if (u.IsRemoved || !state.Formation.Contains(unitId)) return false;
        if (u.NoReviveUntil is { } until && state.Timeline.Now < until) return false;       // Desecrate
        return u.IsUndead ? Necromancy.CanRestore(state, unitId) : u.IsKnockedOut;
    }

    /// <summary>A quarter of the maximum HP, rounded down, minimum 1 (Bound Resurrect).</summary>
    internal static int QuarterHp(Unit u) => Math.Max(1, (u.IsUndead ? u.OriginalMaxHp ?? u.MaxHp : u.MaxHp) / 4);
}

/// <summary>
/// Burnout Heal: the Lifebinder and its adjacent allies heal 6-8; then the Lifebinder is KO. The heals are one simultaneous block; the KO resolves after it
/// and before reactions are collected, so the Lifebinder's own reactions do not fire (game_system.md section 10.4).
/// </summary>
public sealed class BurnoutHeal : ClassSkill
{
    public BurnoutHeal(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var heals = state.Formation.Adjacent(context.UserId).Where(id => !state.GetUnit(id).IsKnockedOut && state.GetUnit(id).Side == context.UserSide)
            .Prepend(context.UserId).Select(id => new SkillContext.HitSpec(id, new IntRange(6, 8))).ToArray();
        context.HealBlock(heals);
        if (!context.User.IsKnockedOut)
        {
            state.DealDamage(context.UserId, context.UserId, context.User.Hp, DamageSource.Unreducible);   // the sacrifice
            context.Settle();
        }
    }
}

/// <summary>Life for Life: the Lifebinder gives up half its current HP (a quarter with Phoenix) to revive a KO unit with that much HP. It needs at least 1 HP to give.</summary>
public sealed class LifeForLife : ClassSkill
{
    public LifeForLife(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(IncludeKnockedOut: true);   // a resurrection can target allied or enemy bodies

    private static int Gift(BattleState state, int userId) =>
        state.GetUnit(userId).Hp / (state.GetUnit(userId).KnownSkills.Contains("phoenix") ? 4 : 2);

    public override string? Unavailable(BattleState state, int userId) =>
        Gift(state, userId) >= 1 ? null : "needs at least 1 HP to give up";

    public override bool TargetFilter(BattleState state, int userId, int targetId) => LifebinderSkills.CanBeRevived(state, targetId);

    public override void Resolve(SkillContext context)
    {
        var give = Gift(context.State, context.UserId);
        context.State.DealDamage(context.UserId, context.UserId, give, DamageSource.Unreducible);   // given up: no Ward or reduction applies
        context.ReviveOrRestore(context.TargetId!.Value, give);
    }
}

/// <summary>
/// Bound Resurrect: revive a KO unit at 25% of its max HP (rounded down, minimum 1); it and the Lifebinder share one HP pool (game_system.md section 10.4): the
/// pool's maximum is the sum of both maximum HP and it starts at the Lifebinder's current HP plus the revival HP.
/// </summary>
public sealed class BoundResurrect : ClassSkill
{
    public BoundResurrect(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(IncludeKnockedOut: true);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        LifebinderSkills.CanBeRevived(state, targetId);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var id = context.TargetId!.Value;
        context.ReviveOrRestore(id, LifebinderSkills.QuarterHp(state.GetUnit(id)));
        if (state.GetUnit(id).IsKnockedOut || context.User.IsKnockedOut) return;
        if (state.SharedPoolOf(context.UserId) is { } pool) state.JoinSharedPool(pool, id);
        else state.AddSharedPool(context.UserId, id, context.User.Hp + state.GetUnit(id).Hp, boundStrength: false);
    }
}

/// <summary>Martyr: when the Lifebinder is KO, every ally heals 6-8.</summary>
public sealed class Martyr : KeystoneSkill
{
    public Martyr(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            var range = DamageMath.ScaleRange(new IntRange(6, 8), st.PowerOf(ownerId));
            foreach (var id in st.Formation.Units(st.GetUnit(ownerId).Side).ToArray())
                if (id != ownerId && !st.GetUnit(id).IsKnockedOut) st.Heal(ownerId, id, DamageMath.Roll(st.Random, range));
        })
        {
            Condition = (st, t) => t.SubjectUnitId == ownerId,
        });
}

/// <summary>Phoenix: Life for Life costs only a quarter of the Lifebinder's current HP. Life for Life reads this keystone.</summary>
public sealed class Phoenix : KeystoneSkill
{
    public Phoenix(SkillDef def) : base(def) { }
}

/// <summary>Lifeblood: when an ally is KO, the Lifebinder heals 5.</summary>
public sealed class Lifeblood : KeystoneSkill
{
    public const int Healing = 5;
    public Lifeblood(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            if (!st.GetUnit(ownerId).IsKnockedOut) st.Heal(ownerId, ownerId, DamageMath.Scale(Healing, st.PowerOf(ownerId)));
        })
        {
            Condition = (st, t) => t.SubjectUnitId != ownerId && st.GetUnit(t.SubjectUnitId).Side == st.GetUnit(ownerId).Side,
        });
}
