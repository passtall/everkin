using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// A creature's unique attack (class_system.md section 4): one parametric skill built from the attack's data. A name, a delivery, damage, one small
/// extra, Delay and Momentum. Every attack hits an enemy; Direct attacks (breaths and pulses) work in melee reach (game_system.md section 8.1).
/// </summary>
public sealed class UniqueAttackSkill : Skill
{
    /// <summary>Burning ground, ice and water from an attack last the reference length; poison gas lasts 10 (game_system.md section 12.2).</summary>
    public const int GroundLength = 20;
    public const int GasLength = 10;

    private readonly AttackDef _def;

    public UniqueAttackSkill(AttackDef def) : base(def.Id, def.Delay, def.Momentum) => _def = def;

    public AttackDef Def => _def;

    public override TargetKind Target => _def.Shape == "pulse" ? TargetKind.None : TargetKind.Unit;
    public override bool IsAttack => true;
    public override bool IsArea => _def.Shape != "single";
    public override bool Magical => _def.Magical;

    public override Delivery? Delivery => _def.Delivery switch
    {
        "melee" => Battle.Delivery.Melee,
        "projectile" => Battle.Delivery.Projectile,
        _ => Battle.Delivery.Direct,
    };

    public override EffectTags Tags =>
        (Delivery switch { Battle.Delivery.Melee => EffectTags.Melee, Battle.Delivery.Projectile => EffectTags.Projectile, _ => EffectTags.Direct })
        | (_def.Magical ? EffectTags.Magical : EffectTags.Physical);

    /// <summary>Melee and melee-reach (Direct) attacks need the Front row.</summary>
    public override bool NeedsFrontRow => _def.Delivery is "melee" or "direct";

    /// <summary>Every attack hits an enemy.</summary>
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    /// <summary>A breath picks one enemy of the enemy Front row as its target (melee reach).</summary>
    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        _def.Delivery != "direct" || Targeting.EnemiesInMeleeReach(state, userId).Contains(targetId);

    public override void Resolve(SkillContext context)
    {
        IReadOnlyList<SkillContext.HitReport> reports;
        switch (_def.Shape)
        {
            case "pulse":
                reports = context.HitBlock(Targeting.EnemiesInMeleeReach(context.State, context.UserId)
                    .Select(id => new SkillContext.HitSpec(id, new IntRange(_def.Min, _def.Max))).ToArray());
                break;
            case "splash":
            {
                var target = context.TargetId!.Value;
                var hits = new List<SkillContext.HitSpec> { new(target, new IntRange(_def.Min, _def.Max)) };
                foreach (var id in AreaPatterns.Circular(context.State, target).Skip(1))
                    hits.Add(new SkillContext.HitSpec(id, new IntRange(_def.SplashMin, _def.SplashMax)));
                reports = context.HitBlock(hits);
                break;
            }
            default:
                reports = new[] { context.Hit(context.TargetId!.Value, new IntRange(_def.Min, _def.Max)) };
                break;
        }

        // The extra is rolled once for every enemy the attack hit (class_system.md section 4), after the hit.
        var applied = false;
        foreach (var report in reports)
        {
            if (context.Over) return;
            if (report.Outcome == HitOutcome.Missed) continue;
            applied |= ApplyExtra(context, report);
        }
        if (applied && !context.Over) context.Settle();
    }

    private bool ApplyExtra(SkillContext context, SkillContext.HitReport report)
    {
        var extra = _def.Extra;
        var state = context.State;
        if (extra.Kind == "stealth")
        {
            if (!Stealth.CanGain(state, context.UserId)) return false;
            Stealth.Gain(state, context.UserId);
            return true;
        }
        if (extra.Chance < 100 && !state.Random.Chance(extra.Chance)) return false;
        var receiver = report.EffectReceiverId;
        if (state.GetUnit(receiver).IsKnockedOut || !state.Formation.Contains(receiver)) return false;

        switch (extra.Kind)
        {
            case "status":
                return Statuses.Apply(state, extra.Status switch
                {
                    "stunned" => new StatusApplication(receiver, StatusIds.Stunned, extra.Duration, SourceUnitId: context.UserId),
                    "confused" => new StatusApplication(receiver, StatusIds.Confused, extra.Duration, SourceUnitId: context.UserId),
                    "immobilized" => new StatusApplication(receiver, StatusIds.Immobilized, extra.Duration, SourceUnitId: context.UserId),
                    "blind" => new StatusApplication(receiver, StatusIds.Blind, extra.Duration, SourceUnitId: context.UserId),
                    "silenced" => new StatusApplication(receiver, StatusIds.Silenced, extra.Duration, SourceUnitId: context.UserId),
                    "poisoned" => new StatusApplication(receiver, StatusIds.Poison, extra.Duration, SourceUnitId: context.UserId),
                    "bleeding" => new StatusApplication(receiver, StatusIds.Bleeding, Uses: 3, SourceUnitId: context.UserId),
                    "marked" => new StatusApplication(receiver, StatusIds.Marked, Statuses.MarkLength, SourceUnitId: context.UserId),
                    _ => throw new InvalidOperationException($"Unknown attack status {extra.Status}."),
                });
            case "push":
            case "pull":
            {
                var row = state.Formation.PositionOf(receiver).Row;
                var destination = extra.Kind == "push" ? (int)row + 1 : (int)row - 1;
                if (destination is < 0 or > 2) return false;                       // Rear units stay; Front units stay
                var result = Movement.ForceMove(state, receiver, (Row)destination, context.UserSide);
                context.AfterForcedMove(result);
                return false;
            }
            case "ground":
            {
                var spot = state.Ground.SpotOf(receiver);
                var (kind, length) = extra.Ground switch
                {
                    "burning" => (GroundKind.Burning, GroundLength),
                    "ice" => (GroundKind.Ice, GroundLength),
                    "water" => (GroundKind.Water, GroundLength),
                    _ => (GroundKind.Poison, GasLength),
                };
                return state.Ground.Place(kind, spot, length, context.UserId);
            }
            case "steal":
                // Stealing happens only if the hit reaches the target itself (landed or absorbed by a Ward, not taken by an interceptor).
                return report.Interceptions.Count == 0 && Statuses.Steal(state, context.UserId, report.TargetId, all: false) > 0;
            case "delay_turn":
                state.PushTurnBack(receiver, extra.Amount);
                return false;
            default:
                throw new InvalidOperationException($"Unknown attack extra {extra.Kind}.");
        }
    }
}
