using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Rogue's skills (skills.md Rogue): Thief, Assassin and Saboteur. The Rogue's actives are physical (not `magical`). The rulings of
/// 2026-11-10 (Sneak, Light Fingers, Lift a Skill, Backstab, Contract, Deathmark Bloom, Cut the Strings, Disarm Trap) are in skills.md.
/// </summary>
public static class RogueSkills
{
    public const int MarkLength = 20;
    public const int PoisonGroundLength = 20;
    public const int BackstabSilence = 20;
    public const int BackstabBleedingSkills = 3;
    public const int FlashPowderBlind = 20;
    public const int JamDelay = 3;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new Hide(D("hide"));
        yield return new MeltAway(D("melt_away"));
        yield return new Sneak(D("sneak"));
        yield return new LiftASkill(D("lift_a_skill"));
        yield return new LightFingers(D("light_fingers"));
        yield return new Backstab(D("backstab"));
        yield return new Contract(D("contract"));
        yield return new PoisonVial(D("poison_vial"));
        yield return new MarkForDeath(D("mark_for_death"));
        yield return new DeathmarkBloom(D("deathmark_bloom"));
        yield return new FlashPowder(D("flash_powder"));
        yield return new CutTheStrings(D("cut_the_strings"));
        yield return new Jam(D("jam"));
        yield return new DisarmTrap(D("disarm_trap"));
    }

    /// <summary>The auto events a unit owns on the timeline, soonest first.</summary>
    internal static IReadOnlyList<TimelineEntry> AutoEventsOf(BattleState state, int unitId) =>
        state.Timeline.Upcoming().Where(e => e.Kind == TimelineKind.AutoEvent && e.OwnerUnitId == unitId).ToArray();
}

/// <summary>Base of the Rogue's targeted actives.</summary>
public abstract class RogueSkill : ProfessionSkill
{
    protected RogueSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
}

// ------------------------------------------------------------------------------------------------ Thief

/// <summary>Hide: the Rogue gains stealth. Needs the Rogue outside the Front row.</summary>
public sealed class Hide : RogueSkill
{
    public Hide(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId)
    {
        if (state.Formation.PositionOf(userId).Row == Row.Front) return "needs the Rogue outside the Front row";
        return Stealth.CanGain(state, userId) ? null : "cannot gain stealth now";
    }

    public override void Resolve(SkillContext context)
    {
        Stealth.Gain(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>Melt Away: the Rogue gains stealth anywhere, even in the Front row.</summary>
public sealed class MeltAway : RogueSkill
{
    public MeltAway(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override string? Unavailable(BattleState state, int userId) => Stealth.CanGain(state, userId) ? null : "cannot gain stealth now";

    public override void Resolve(SkillContext context)
    {
        Stealth.Gain(context.State, context.UserId);
        context.Settle();
    }
}

/// <summary>Sneak (passive): while stealthed, the Rogue may move one row at the start of its turn, free and without losing stealth. See <see cref="Actions.SneakMove"/>.</summary>
public sealed class Sneak : PassiveSkill
{
    public Sneak(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Lift a Skill: use a copy of the last active skill the target used, with the Rogue's own Power. The copy is free.</summary>
public sealed class LiftASkill : RogueSkill
{
    public LiftASkill(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override bool TargetFilter(BattleState state, int userId, int targetId) => state.GetUnit(targetId).LastActiveSkillId is not null;

    private static Skill? Copied(BattleState state, SkillCatalog catalog, int targetId) =>
        state.GetUnit(targetId).LastActiveSkillId is { } id && id != "lift_a_skill" && catalog.TryGet(id, out var skill)
            && !skill.IsPassive && skill.Target is TargetKind.None or TargetKind.Unit ? skill : null;

    public override string? CheckUse(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        if (use.TargetUnitId is not { } who || who < 0 || who >= state.Units.Count) return null;      // the target checks report it
        if (Copied(state, catalog, who) is not { } copy) return "the target has no skill that can be copied";
        if (copy.Magical && !Statuses.CanUseMagical(state.GetUnit(use.UserId))) return "Silenced: the copied skill is magical";
        if (copy.Target != TargetKind.Unit) return null;
        if (use.SecondTargetUnitId is not { } second || second < 0 || second >= state.Units.Count) return "the copied skill needs a target";
        var options = copy.Options with { Delivery = copy.Delivery };
        return Targeting.IsLegalTarget(state, use.UserId, second, Actions.ReachOf(state, use.UserId, copy), options) && copy.TargetFilter(state, use.UserId, second)
            ? null : "not a legal target for the copied skill";
    }

    public override void Resolve(SkillContext context)
    {
        var copy = Copied(context.State, context.Catalog!, context.TargetId!.Value)!;
        var second = copy.Target == TargetKind.Unit ? context.SecondTargetId : null;
        copy.Resolve(new SkillContext(context.State, copy, context.UserId, new SkillUse(context.UserId, copy.Id, second), second) { Catalog = context.Catalog });
    }
}

/// <summary>Light Fingers (passive): whenever the Rogue's hit reaches an enemy that has a helpful status, it steals one.</summary>
public sealed class LightFingers : PassiveSkill
{
    public LightFingers(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddTargetHitHook((st, attacker, target) =>
        {
            if (attacker != ownerId || st.GetUnit(ownerId).IsKnockedOut) return;
            var victim = st.GetUnit(target);
            if (victim.IsKnockedOut || victim.Side == st.GetUnit(ownerId).Side) return;
            var helpful = HealerSkills.HelpfulStatuses(victim);
            if (helpful.Count > 0) Statuses.Transfer(st, target, ownerId, HealerSkills.Pick(st, helpful));
        });
}

// ------------------------------------------------------------------------------------------------ Assassin

/// <summary>Backstab: only from stealth: the target is Silenced and Bleeds; the Rogue stays stealthed. No damage.</summary>
public sealed class Backstab : RogueSkill
{
    public Backstab(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override string? Unavailable(BattleState state, int userId) => state.GetUnit(userId).IsStealthed ? null : "only from stealth";

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var receiver = context.InterceptEffect(context.TargetId!.Value);
        if (!state.GetUnit(receiver).IsKnockedOut)
        {
            Statuses.Apply(state, new StatusApplication(receiver, StatusIds.Silenced, RogueSkills.BackstabSilence, SourceUnitId: context.UserId));
            Statuses.Apply(state, new StatusApplication(receiver, StatusIds.Bleeding, SourceUnitId: context.UserId, Uses: RogueSkills.BackstabBleedingSkills));
        }
        context.Settle();
    }
}

/// <summary>Contract (keystone): the Rogue deals double to Marked enemies and half to everyone else.</summary>
public sealed class Contract : PassiveSkill
{
    public Contract(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddDamageBonus((st, attacker, target) =>
            attacker != ownerId ? 0 : HunterSkills.IsMarkedEnemy(st, ownerId, target) ? 100 : -50);
}

/// <summary>Poison Vial: throw a vial: poison ground on a unit's spot for 20 time units. A projectile: an interceptor takes it.</summary>
public sealed class PoisonVial : RogueSkill
{
    public PoisonVial(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Projectile;
    public override EffectTags Tags => EffectTags.Projectile | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var receiver = context.InterceptEffect(context.TargetId!.Value);
        context.State.Ground.Place(GroundKind.Poison, context.State.Ground.SpotOf(receiver), RogueSkills.PoisonGroundLength, context.UserId);
        context.Settle();
    }
}

/// <summary>Mark for Death: mark a unit for 20 time units.</summary>
public sealed class MarkForDeath : RogueSkill
{
    public MarkForDeath(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);       // Marked is an enemy-only status

    public override void Resolve(SkillContext context)
    {
        HunterSkills.Mark(context.State, context.TargetId!.Value, context.UserId, RogueSkills.MarkLength);
        context.Settle();
    }
}

/// <summary>Deathmark Bloom (passive): when a Marked enemy is KO, its Mark jumps to the nearest enemy (20 time units).</summary>
public sealed class DeathmarkBloom : PassiveSkill
{
    public DeathmarkBloom(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            var fallen = ctx.Trigger.SubjectUnitId;
            if (!st.Formation.Contains(fallen)) return;                               // a summon that has left the board
            var position = st.Formation.PositionOf(fallen);
            var coordinate = st.Formation.CenterOf(position);
            var candidates = st.Units.Where(u => u.Side == st.GetUnit(fallen).Side && u.Id != fallen && !u.IsKnockedOut && !u.IsRemoved && st.Formation.Contains(u.Id))
                .Select(u => (u.Id, Rows: Math.Abs((int)st.Formation.PositionOf(u.Id).Row - (int)position.Row),
                    Across: Math.Abs(st.Formation.CenterOf(st.Formation.PositionOf(u.Id)) - coordinate))).ToList();
            if (candidates.Count == 0) return;
            var best = candidates.Min(c => (c.Rows, c.Across));
            var nearest = candidates.Where(c => (c.Rows, c.Across) == best).Select(c => c.Id).ToList();
            HunterSkills.Mark(st, HealerSkills.Pick(st, nearest), ownerId, RogueSkills.MarkLength);
        })
        {
            Condition = (st, t) => st.GetUnit(t.SubjectUnitId).MarkedAtKo && st.GetUnit(t.SubjectUnitId).Side != st.GetUnit(ownerId).Side,
        });
}

// ------------------------------------------------------------------------------------------------ Saboteur

/// <summary>Flash Powder: every enemy in melee reach is Blind for 20 time units.</summary>
public sealed class FlashPowder : RogueSkill
{
    public FlashPowder(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override bool IsArea => true;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        foreach (var id in WarriorSkills.EnemiesInMeleeReach(context.State, context.UserId))
            Statuses.Apply(context.State, new StatusApplication(id, StatusIds.Blind, RogueSkills.FlashPowderBlind, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Cut the Strings: remove one auto event the other side has on the timeline: the next one of the chosen enemy unit.</summary>
public sealed class CutTheStrings : RogueSkill
{
    public CutTheStrings(ProfessionSkillDef def) : base(def) { }
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override bool TargetFilter(BattleState state, int userId, int targetId) => RogueSkills.AutoEventsOf(state, targetId).Count > 0;

    public override void Resolve(SkillContext context)
    {
        if (RogueSkills.AutoEventsOf(context.State, context.TargetId!.Value).FirstOrDefault() is { } next)
            context.State.Timeline.Remove(next.Id);
        context.Settle();
    }
}

/// <summary>Jam: a unit's next skill has +3 Delay. A projectile: an interceptor takes it.</summary>
public sealed class Jam : RogueSkill
{
    public Jam(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Projectile;
    public override EffectTags Tags => EffectTags.Projectile | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var receiver = context.InterceptEffect(context.TargetId!.Value);
        if (!context.State.GetUnit(receiver).IsKnockedOut)
            Statuses.Apply(context.State, new StatusApplication(receiver, StatusIds.NextSkillDelay, Magnitude: RogueSkills.JamDelay, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Disarm Trap (passive): the Rogue sets off enemy traps without taking their effect. Read by the trap skills (Net Trap).</summary>
public sealed class DisarmTrap : PassiveSkill
{
    public DisarmTrap(ProfessionSkillDef def) : base(def) { }
}
