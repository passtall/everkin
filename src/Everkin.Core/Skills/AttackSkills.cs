using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The nine Attack Type attacks (skills.md "Attack Type attacks"). Every creature starts with the one shared attack of its
/// Attack Type; it costs no skill point. Reaches (owner decision 2026-11-08): plain melee D1, Polearm Thrust and Lash D2,
/// Breath D1, Pulse D1, Shot and Bolt any visible enemy.
/// </summary>
public static class AttackSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        AttackSkillDef Def(string id) => content.AttackSkills.Single(a => a.Id == id);
        Skill Make(Func<AttackSkillDef, Skill> factory, string id) => factory(Def(id));

        yield return Make(d => new BrawlStrike(d), "brawl_strike");
        yield return Make(d => new BladeStrike(d), "blade_strike");
        yield return Make(d => new PolearmThrust(d), "polearm_thrust");
        yield return Make(d => new Shot(d), "shot");
        yield return Make(d => new Bolt(d), "bolt");
        yield return Make(d => new HeavyBlow(d), "heavy_blow");
        yield return Make(d => new Breath(d), "breath");
        yield return Make(d => new Lash(d), "lash");
        yield return Make(d => new Pulse(d), "pulse");
    }
}

/// <summary>A single-target damaging attack: one hit on the chosen unit, with an optional rider on whoever took the hit.</summary>
public abstract class StrikeSkill : Skill
{
    protected StrikeSkill(AttackSkillDef def) : base(def.Id, def.Delay, def.Momentum) { }

    public sealed override TargetKind Target => TargetKind.Unit;

    /// <summary>The damage written for Power 5.</summary>
    protected abstract IntRange Damage { get; }

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, Damage);
        if (report.Outcome != HitOutcome.Missed) Rider(context, report);
    }

    /// <summary>What rides on a hit that did not miss (a status chance). The status goes to whoever took the hit: an interceptor if one stepped in.</summary>
    protected virtual void Rider(SkillContext context, SkillContext.HitReport report) { }

    /// <summary>Applies a status with a percentage chance to whoever received the hit, if it is still standing.</summary>
    protected static void MaybeApply(SkillContext context, SkillContext.HitReport report, int chancePercent, StatusApplication template)
    {
        if (!context.State.Random.Chance(chancePercent)) return;
        var receiver = report.EffectReceiverId;
        if (context.State.GetUnit(receiver).IsKnockedOut) return;
        Statuses.Apply(context.State, template with { UnitId = receiver, SourceUnitId = context.UserId });
    }
}

public sealed class BrawlStrike : StrikeSkill
{
    public BrawlStrike(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);
    protected override IntRange Damage => new(4, 5);
}

public sealed class BladeStrike : StrikeSkill
{
    public BladeStrike(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);
    protected override IntRange Damage => new(4, 5);

    protected override void Rider(SkillContext context, SkillContext.HitReport report) =>
        MaybeApply(context, report, 30, new StatusApplication(0, StatusIds.Bleeding, Uses: 3));   // 30% chance to make the target Bleed
}

public sealed class PolearmThrust : StrikeSkill
{
    public PolearmThrust(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(2);   // one row more reach than other melee
    protected override IntRange Damage => new(4, 5);
}

public sealed class Lash : StrikeSkill
{
    public Lash(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(2);   // one row more reach than other melee
    protected override IntRange Damage => new(3, 4);
}

public sealed class HeavyBlow : StrikeSkill
{
    public HeavyBlow(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Melee;
    public override EffectTags Tags => EffectTags.Melee | EffectTags.Physical;
    public override Reach Reach => Reach.Distance(1);
    protected override IntRange Damage => new(5, 6);

    protected override void Rider(SkillContext context, SkillContext.HitReport report) =>
        MaybeApply(context, report, 30, new StatusApplication(0, StatusIds.Stunned, Duration: 5));   // 30% chance to Stun for 5 time units
}

public sealed class Shot : StrikeSkill
{
    public Shot(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Projectile;
    public override EffectTags Tags => EffectTags.Projectile | EffectTags.Physical;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);   // any visible enemy
    protected override IntRange Damage => new(3, 4);
}

public sealed class Bolt : StrikeSkill
{
    public Bolt(AttackSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Projectile;
    public override EffectTags Tags => EffectTags.Projectile | EffectTags.Magical;
    public override bool Magical => true;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);   // any visible enemy
    protected override IntRange Damage => new(3, 4);
}

/// <summary>Breath: hit the target for 3–4 and every unit next to it for 1–2. `direct`: it cannot be intercepted.</summary>
public sealed class Breath : Skill
{
    public Breath(AttackSkillDef def) : base(def.Id, def.Delay, def.Momentum) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override EffectTags Tags => EffectTags.Direct;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        var hits = new List<SkillContext.HitSpec> { new(target, new IntRange(3, 4)) };
        foreach (var id in AreaPatterns.Circular(context.State, target).Skip(1))
            hits.Add(new SkillContext.HitSpec(id, new IntRange(1, 2)));
        context.HitBlock(hits);
    }
}

/// <summary>Pulse: every enemy in melee reach takes 2–3. `direct`, no target to choose.</summary>
public sealed class Pulse : Skill
{
    public Pulse(AttackSkillDef def) : base(def.Id, def.Delay, def.Momentum) { }
    public override TargetKind Target => TargetKind.None;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override EffectTags Tags => EffectTags.Direct;

    private static IReadOnlyList<int> EnemiesInReach(BattleState state, int userId)
    {
        var enemy = state.GetUnit(userId).Side.Opposite();
        return state.Formation.Units(enemy)
            .Where(id => !state.GetUnit(id).IsKnockedOut && Targeting.InReach(state, userId, id, Reach.Distance(1)))
            .ToArray();
    }

    public override string? Unavailable(BattleState state, int userId) =>
        EnemiesInReach(state, userId).Count == 0 ? "no enemy in melee reach" : null;

    public override void Resolve(SkillContext context) =>
        context.HitBlock(EnemiesInReach(context.State, context.UserId)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(2, 3))).ToArray());
}
