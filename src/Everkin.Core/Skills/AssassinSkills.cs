using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Assassin (skills.md): stealth and a hard strike from it. Physical.</summary>
public static class AssassinSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Vanish(D("vanish"));
        yield return new Feint(D("feint"));
        yield return new Backstab(D("backstab"));
        yield return new Unseen(D("unseen"));
        yield return new VanishingKill(D("vanishing_kill"));
        yield return new VenomBlades(D("venom_blades"));
    }
}

/// <summary>Vanish: the Assassin gains stealth and loses every harmful status.</summary>
public sealed class Vanish : ClassSkill
{
    public Vanish(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId) =>
        Stealth.CanGain(state, userId) ? null : "cannot gain stealth now";

    public override void Resolve(SkillContext context)
    {
        Statuses.RemoveAll(context.State, context.UserId, Polarity.Harmful, byUnitId: context.UserId);
        Stealth.Gain(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>Feint: only from stealth, a unit is Confused for 10. The Assassin stays stealthed. A melee effect: an interceptor takes it instead.</summary>
public sealed class Feint : ClassSkill
{
    public const int Length = 10;
    public Feint(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override string? Unavailable(BattleState state, int userId) => state.GetUnit(userId).IsStealthed ? null : "only from stealth";

    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(0, StatusIds.Confused, Length));
}

/// <summary>Backstab: only from stealth, hit for 8-10 and the target Bleeds. The attack breaks stealth.</summary>
public sealed class Backstab : ClassSkill
{
    public Backstab(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;
    public override string? Unavailable(BattleState state, int userId) => state.GetUnit(userId).IsStealthed ? null : "only from stealth";

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, new IntRange(8, 10));
        if (report.Outcome == HitOutcome.Missed || context.Over) return;
        var receiver = report.EffectReceiverId;
        if (context.State.GetUnit(receiver).IsKnockedOut) return;
        Statuses.Apply(context.State, new StatusApplication(receiver, StatusIds.Bleeding, Uses: 3, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Unseen: the Assassin starts every battle stealthed.</summary>
public sealed class Unseen : KeystoneSkill
{
    public Unseen(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (Stealth.CanGain(state, ownerId)) Stealth.Gain(state, ownerId);
    }
}

/// <summary>Vanishing Kill: when the Assassin KOs a unit, it gains stealth again.</summary>
public sealed class VanishingKill : KeystoneSkill
{
    public VanishingKill(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            if (Stealth.CanGain(st, ownerId)) Stealth.Gain(st, ownerId);
        })
        {
            Condition = (st, t) => t.SourceUnitId == ownerId && t.SubjectUnitId != ownerId,
        });
}

/// <summary>Venom Blades: every hit of the Assassin Poisons the target for 20.</summary>
public sealed class VenomBlades : KeystoneSkill
{
    public const int Length = 20;
    public VenomBlades(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddTargetHitHook((st, attackerId, targetId) =>
        {
            if (attackerId == ownerId && !st.GetUnit(targetId).IsKnockedOut)
                Statuses.Apply(st, new StatusApplication(targetId, StatusIds.Poison, Length, SourceUnitId: ownerId));
        });
}
