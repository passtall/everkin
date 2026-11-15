using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Berserker (skills.md): raw melee damage and pushing the enemy Front row. Physical.</summary>
public static class BerserkerSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Rampage(D("rampage"));
        yield return new SweepingKick(D("sweeping_kick"));
        yield return new BattleRoar(D("battle_roar"));
        yield return new RecklessSwing(D("reckless_swing"));
        yield return new LastStand(D("last_stand"));
        yield return new Unstoppable(D("unstoppable"));
    }

    /// <summary>Whether a unit is below a quarter of its maximum HP.</summary>
    internal static bool BelowQuarter(Unit u) => u.Hp * 4 < u.MaxHp;
}

/// <summary>Rampage: hit for 10-12; the Berserker takes 3 (scaled, and it goes through its Ward like any fixed damage).</summary>
public sealed class Rampage : ClassSkill
{
    public const int Recoil = 3;
    public Rampage(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        context.Hit(context.TargetId!.Value, new IntRange(10, 12));
        if (context.Over || context.User.IsKnockedOut) return;
        context.State.DealDamage(context.UserId, context.UserId, context.Scaled(Recoil));
        context.Settle();
    }
}

/// <summary>Sweeping Kick: push every enemy in melee reach one row back as a group (bumps apply). It cannot be intercepted; melee cannot reach an airborne unit.</summary>
public sealed class SweepingKick : ClassSkill
{
    public SweepingKick(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override bool NeedsFrontRow => true;

    public override void Resolve(SkillContext context) =>
        context.PushGroupBack(Targeting.EnemiesInMeleeReach(context.State, context.UserId).Where(id => !context.State.GetUnit(id).IsAirborne));
}

/// <summary>Battle Roar: every enemy in melee reach has its next turn pushed back 5.</summary>
public sealed class BattleRoar : ClassSkill
{
    public const int PushLength = 5;
    public BattleRoar(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override bool NeedsFrontRow => true;

    public override void Resolve(SkillContext context)
    {
        foreach (var id in Targeting.EnemiesInMeleeReach(context.State, context.UserId))
            context.State.PushTurnBack(id, PushLength);
        context.Settle();
    }
}

/// <summary>Reckless Swing: the Berserker's hits deal double damage, but it takes half of the damage it deals (rounded down, unreducible).</summary>
public sealed class RecklessSwing : KeystoneSkill
{
    public RecklessSwing(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddDamageBonus((st, attacker, target) => attacker == ownerId ? 100 : 0);
        state.AddPostHitHook((st, attacker, target, hpLost) =>
        {
            var recoil = hpLost / 2;
            if (attacker == ownerId && recoil > 0 && !st.GetUnit(ownerId).IsKnockedOut)
                st.DealDamage(ownerId, ownerId, recoil, DamageSource.Unreducible);
        });
    }
}

/// <summary>Last Stand: the Berserker cannot Move or be moved, but deals 50% more damage.</summary>
public sealed class LastStand : KeystoneSkill
{
    public LastStand(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddForcedMoveImmunity((st, unitId, causedBy) => unitId == ownerId);       // from any side, its own skills included
        state.AddMoveBan((st, unitId) => unitId == ownerId);
        state.AddDamageBonus((st, attacker, target) => attacker == ownerId ? 50 : 0);
    }
}

/// <summary>Unstoppable: below a quarter of its HP the Berserker cannot be Immobilized or Frozen and its turns cannot be pushed back.</summary>
public sealed class Unstoppable : KeystoneSkill
{
    public Unstoppable(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddStatusImmunity((st, unitId, statusId) =>
            unitId == ownerId && BerserkerSkills.BelowQuarter(st.GetUnit(ownerId)) && statusId is StatusIds.Immobilized or StatusIds.Frozen);
        state.AddTurnPushImmunity((st, unitId) => unitId == ownerId && BerserkerSkills.BelowQuarter(st.GetUnit(ownerId)));
    }
}
