using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Cryomancer (skills.md): frost, Frozen and ice. Magical.</summary>
public static class CryomancerSkills
{
    public const int IceLength = 20;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new IceLance(D("ice_lance"));
        yield return new Freeze(D("freeze"));
        yield return new FrostNova(D("frost_nova"));
        yield return new ColdHeart(D("cold_heart"));
        yield return new Brittle(D("brittle"));
        yield return new DeepChill(D("deep_chill"));
    }

    /// <summary>
    /// Freezes units. With Cold Heart the Frozen unit skips its next 2 turns, only one unit stays Frozen (when one skill freezes several the seeded roll
    /// keeps one) and the Cryomancer's earlier Frozen unit thaws. With Deep Chill each unit frozen also gets ice on its spot for 20.
    /// </summary>
    internal static void Freeze(SkillContext context, IEnumerable<int> ids)
    {
        var state = context.State;
        var targets = ids.Where(id => !state.GetUnit(id).IsKnockedOut && state.Formation.Contains(id)).ToList();
        var cold = context.User.KnownSkills.Contains("cold_heart");
        if (cold)
        {
            if (targets.Count > 1) targets = new List<int> { ClassHelpers.Pick(state, targets) };
            foreach (var unit in state.Units.ToArray())
                if (!targets.Contains(unit.Id) && Statuses.Get(unit, StatusIds.Frozen)?.SourceUnitId == context.UserId)
                    Statuses.Remove(state, unit.Id, StatusIds.Frozen, StatusEndCause.Expired);
        }
        var chill = context.User.KnownSkills.Contains("deep_chill");
        foreach (var id in targets)
        {
            Statuses.Apply(state, new StatusApplication(id, StatusIds.Frozen, SourceUnitId: context.UserId, Uses: cold ? 2 : null));
            if (chill && !state.GetUnit(id).IsKnockedOut) state.Ground.Place(GroundKind.Ice, state.Ground.SpotOf(id), IceLength, context.UserId);
        }
    }
}

/// <summary>Ice Lance: hit for 7-8; a Frozen target takes double.</summary>
public sealed class IceLance : ClassSkill
{
    public IceLance(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var frozen = Statuses.Has(context.State.GetUnit(context.TargetId!.Value), StatusIds.Frozen);
        context.Hit(context.TargetId!.Value, frozen ? new IntRange(14, 16) : new IntRange(7, 8));
    }
}

/// <summary>Freeze: a unit is Frozen (its next turn is skipped).</summary>
public sealed class Freeze : ClassSkill
{
    public Freeze(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        CryomancerSkills.Freeze(context, new[] { context.TargetId!.Value });
        context.Settle();
    }
}

/// <summary>Frost Nova: every enemy in melee reach takes 1-2 and is Frozen.</summary>
public sealed class FrostNova : ClassSkill
{
    public FrostNova(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;
    public override bool IsAttack => true;
    public override bool IsArea => true;
    public override bool NeedsFrontRow => true;

    public override void Resolve(SkillContext context)
    {
        var reports = context.HitBlock(Targeting.EnemiesInMeleeReach(context.State, context.UserId)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(1, 2))).ToArray());
        if (context.Over) return;
        CryomancerSkills.Freeze(context, reports.Where(r => r.Outcome != HitOutcome.Missed).Select(r => r.TargetId));
        context.Settle();
    }
}

/// <summary>Cold Heart: Frozen from the Cryomancer lasts 1 more turn, but it can keep only one unit Frozen at a time. Freezing reads this keystone.</summary>
public sealed class ColdHeart : KeystoneSkill
{
    public ColdHeart(SkillDef def) : base(def) { }
}

/// <summary>Brittle: Frozen enemies take 50% more damage (the hit's damage x 3 / 2, rounded down, before Ward).</summary>
public sealed class Brittle : KeystoneSkill
{
    public Brittle(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddDamageModifier((st, source, targetId, amount) =>
        {
            var owner = st.GetUnit(ownerId);
            var target = st.GetUnit(targetId);
            return !owner.IsKnockedOut && target.Side != owner.Side && Statuses.Has(target, StatusIds.Frozen) ? amount * 3 / 2 : amount;
        });
}

/// <summary>Deep Chill: a unit the Cryomancer Freezes also gets ice on its spot for 20. Freezing reads this keystone.</summary>
public sealed class DeepChill : KeystoneSkill
{
    public DeepChill(SkillDef def) : base(def) { }
}
