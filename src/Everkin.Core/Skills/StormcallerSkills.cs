using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Stormcaller (skills.md): lightning, Stuns and wind. Magical.</summary>
public static class StormcallerSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new LightningBolt(D("lightning_bolt"));
        yield return new ChainLightning(D("chain_lightning"));
        yield return new Tempest(D("tempest"));
        yield return new Overcharge(D("overcharge"));
        yield return new StaticField(D("static_field"));
        yield return new EyeOfTheStorm(D("eye_of_the_storm"));
    }

    /// <summary>Lightning explodes the poison gas the unit stands in (that spot and the gas spots touching it).</summary>
    internal static void ExplodeGasAt(BattleState state, int unitId)
    {
        if (state.GetUnit(unitId).IsKnockedOut || !state.Formation.Contains(unitId)) return;
        var spot = state.Ground.SpotOf(unitId);
        if (state.Ground.Has(spot, GroundKind.Poison) && state.Ground.Affected(spot).Contains(unitId)) state.Ground.ExplodeGas(spot);
    }
}

/// <summary>Lightning Bolt: hit for 9-10. Explodes the poison gas the target stands in.</summary>
public sealed class LightningBolt : ClassSkill
{
    public LightningBolt(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, new IntRange(9, 10));
        if (context.Over || report.Outcome == HitOutcome.Missed) return;
        StormcallerSkills.ExplodeGasAt(context.State, report.TargetId);
        context.Settle();
    }
}

/// <summary>
/// Chain Lightning: hit a unit for 1-2, then jump to a random adjacent living unit that this chain has not hit yet and do the same, three more times
/// (five more with Overcharge). Each hit explodes the poison gas the unit stands in. The chain ends early when no unit is left to jump to.
/// </summary>
public sealed class ChainLightning : ClassSkill
{
    public const int Jumps = 3;
    public const int OverchargeJumps = 5;
    public ChainLightning(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var jumps = context.User.KnownSkills.Contains("overcharge") ? OverchargeJumps : Jumps;
        var hit = new List<int>();
        var current = context.TargetId!.Value;
        context.BeginBatch();
        for (var i = 0; i <= jumps && !context.Over; i++)
        {
            if (state.GetUnit(current).IsKnockedOut || !state.Formation.Contains(current)) break;
            hit.Add(current);
            var report = context.Hit(current, new IntRange(1, 2));
            if (!context.Over && report.Outcome != HitOutcome.Missed) StormcallerSkills.ExplodeGasAt(state, current);
            if (context.Over) break;
            // Each jump is made when it happens, from the units adjacent to the unit just hit (it may have moved or fallen).
            var next = state.Formation.Contains(current)
                ? state.Formation.Adjacent(current).Where(id => !hit.Contains(id) && !state.GetUnit(id).IsKnockedOut).ToList()
                : new List<int>();
            if (next.Count == 0) break;
            current = ClassHelpers.Pick(state, next);
        }
        context.EndBatch();
    }
}

/// <summary>Tempest: push every unit in one row one row back (bumps apply). Units in the Rear row stay. The row moves together: each unit keeps its coordinate.</summary>
public sealed class Tempest : ClassSkill
{
    public Tempest(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Row;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => ClassHelpers.AllRows();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        context.PushGroupBack(context.State.Formation.Units(row.Side, row.Row).ToArray());
    }
}

/// <summary>Overcharge: the Stormcaller's Chain Lightning jumps two more times (six units in all). Chain Lightning reads this keystone.</summary>
public sealed class Overcharge : KeystoneSkill
{
    public Overcharge(SkillDef def) : base(def) { }
}

/// <summary>Static Field: a unit that hits the Stormcaller in melee is Stunned for 5.</summary>
public sealed class StaticField : KeystoneSkill
{
    public const int StunLength = 5;
    public StaticField(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenHit, (st, ctx) =>
        {
            if (ctx.Trigger.SourceUnitId is { } attacker && !st.GetUnit(attacker).IsKnockedOut)
                Statuses.Apply(st, new StatusApplication(attacker, StatusIds.Stunned, StunLength, SourceUnitId: ownerId));
        })
        {
            RequiredTags = EffectTags.Melee,
            Condition = (st, t) => t.SubjectUnitId == ownerId && t.SourceUnitId is { } src && st.GetUnit(src).Side != st.GetUnit(ownerId).Side,
        });
}

/// <summary>Eye of the Storm: the Stormcaller cannot be pushed, pulled or Stunned.</summary>
public sealed class EyeOfTheStorm : KeystoneSkill
{
    public EyeOfTheStorm(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddForcedMoveImmunity((st, unitId, causedBy) => unitId == ownerId);
        state.AddStatusImmunity((st, unitId, statusId) => unitId == ownerId && statusId == StatusIds.Stunned);
    }
}
