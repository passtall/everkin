using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Keeper (skills.md): Wards that absorb damage. Physical.</summary>
public static class KeeperSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new WardSkill(D("ward"));
        yield return new ShieldDome(D("shield_dome"));
        yield return new WardBurst(D("ward_burst"));
        yield return new ThornedWard(D("thorned_ward"));
        yield return new TwinWards(D("twin_wards"));
        yield return new SteadfastWard(D("steadfast_ward"));
    }

    /// <summary>The unit has a Ward from this Keeper.</summary>
    internal static bool HasWardFrom(BattleState state, int unitId, int keeperId) =>
        state.GetUnit(unitId).Ward > 0 && Statuses.Get(state.GetUnit(unitId), StatusIds.Ward)?.SourceUnitId == keeperId;
}

/// <summary>Ward: a unit gains a Ward of 4 until the Keeper's next turn.</summary>
public sealed class WardSkill : ClassSkill
{
    public WardSkill(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        void Give(int id) => Statuses.Apply(state, new StatusApplication(id, StatusIds.Ward, Magnitude: context.Scaled(4), SourceUnitId: context.UserId, EndsAtNextTurnOf: context.UserId));
        Give(target);
        if (context.User.KnownSkills.Contains("twin_wards"))
        {
            // Twin Wards: the same Ward goes to one ally next to the target, picked by the seeded roll.
            var allies = state.Formation.Adjacent(target)
                .Where(id => state.GetUnit(id).Side == context.UserSide && !state.GetUnit(id).IsKnockedOut).ToList();
            if (allies.Count > 0) Give(allies.Count == 1 ? allies[0] : allies[state.Random.NextInt(allies.Count)]);
        }
        context.Settle();
    }
}

/// <summary>Shield Dome: every ally gains a Ward of 2 until the Keeper's next turn.</summary>
public sealed class ShieldDome : ClassSkill
{
    public ShieldDome(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        foreach (var id in context.State.Formation.Units(context.UserSide).ToArray())
            if (!context.State.GetUnit(id).IsKnockedOut)
                Statuses.Apply(context.State, new StatusApplication(id, StatusIds.Ward, Magnitude: context.Scaled(2),
                    SourceUnitId: context.UserId, EndsAtNextTurnOf: context.UserId));
        context.Settle();
    }
}

/// <summary>
/// Ward Burst: a unit's Ward breaks and deals its remaining value to every enemy in melee reach. It needs the Keeper in its Front row; the warded unit may stand
/// anywhere. The value is the Ward as it is (already scaled when cast, not scaled again). Each is a hit.
/// </summary>
public sealed class WardBurst : ClassSkill
{
    public WardBurst(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;
    public override bool IsArea => true;
    public override bool NeedsFrontRow => true;

    public override bool TargetFilter(BattleState state, int userId, int targetId) => state.GetUnit(targetId).Ward > 0;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var warded = state.GetUnit(context.TargetId!.Value);
        var amount = warded.Ward;
        warded.Ward = 0;
        if (Statuses.Has(warded, StatusIds.Ward)) Statuses.Remove(state, warded.Id, StatusIds.Ward, StatusEndCause.Broken);
        context.HitBlock(Targeting.EnemiesInMeleeReach(state, context.UserId)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(amount, amount), Unscaled: true)).ToArray());
    }
}

/// <summary>Thorned Ward: when an attack hits a Ward from the Keeper, the attacker takes 3.</summary>
public sealed class ThornedWard : KeystoneSkill
{
    public const int Thorns = 3;
    public ThornedWard(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddWardHitHook((st, attackerId, targetId, absorbed) =>
        {
            if (attackerId is not { } attacker || attacker == ownerId && targetId == ownerId) return;
            if (st.GetUnit(ownerId).IsKnockedOut || st.GetUnit(attacker).IsKnockedOut) return;
            if (Statuses.Get(st.GetUnit(targetId), StatusIds.Ward)?.SourceUnitId != ownerId) return;
            st.DealDamage(ownerId, attacker, DamageMath.Scale(Thorns, st.PowerOf(ownerId)));
        });
}

/// <summary>Twin Wards: Ward also gives the same Ward to one ally next to the target. The work is done by Ward, which reads this keystone.</summary>
public sealed class TwinWards : KeystoneSkill
{
    public TwinWards(SkillDef def) : base(def) { }
}

/// <summary>Steadfast Ward: while a unit has a Ward from the Keeper, it cannot be Stunned or moved by enemies.</summary>
public sealed class SteadfastWard : KeystoneSkill
{
    public SteadfastWard(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddStatusImmunity((st, unitId, statusId) => statusId == StatusIds.Stunned && KeeperSkills.HasWardFrom(st, unitId, ownerId) && !st.GetUnit(ownerId).IsKnockedOut);
        state.AddForcedMoveImmunity((st, unitId, causedBy) =>
            causedBy != st.GetUnit(unitId).Side && KeeperSkills.HasWardFrom(st, unitId, ownerId) && !st.GetUnit(ownerId).IsKnockedOut);
    }
}
