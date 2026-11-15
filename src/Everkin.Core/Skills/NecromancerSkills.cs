using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Necromancer (skills.md): raises and uses the dead. Magical.</summary>
public static class NecromancerSkills
{
    public const int UndeadHp = 12;
    public const int DesecrateLength = 40;
    public const int BoneArmorWard = 2;
    public const int BountyHealing = 5;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new RaiseUndead(D("raise_undead"));
        yield return new CorpseExplosion(D("corpse_explosion"));
        yield return new Desecrate(D("desecrate"));
        yield return new LichForm(D("lich_form"));
        yield return new BoneArmor(D("bone_armor"));
        yield return new DeathsBounty(D("deaths_bounty"));
    }

    /// <summary>A KO body (of either side) that can be raised and has a free spot on the Necromancer's side.</summary>
    internal static bool Raisable(BattleState state, int necromancerId, int bodyId)
    {
        var body = state.GetUnit(bodyId);
        return body.IsKnockedOut && !body.IsRemoved && state.Formation.Contains(bodyId) && Necromancy.RaisePlacements(state, necromancerId, bodyId).Count > 0;
    }

    /// <summary>The undead a Necromancer has: raised units of its side that are still undead and standing.</summary>
    internal static IEnumerable<Unit> UndeadOf(BattleState state, int necromancerId) =>
        state.Units.Where(u => u.IsUndead && u.SummonerId == necromancerId && !u.IsKnockedOut && !u.IsRemoved && state.Formation.Contains(u.Id));
}

/// <summary>
/// Raise Undead: turn a KO'd body of either side into an undead of yours (HP 12, the Necromancer's current Power, its own unique attack and Speed), and raise one more
/// random body. The player picks the body and a free spot on your side; the second body is picked by the seeded roll and goes on a random free spot (skipped when there is
/// none or no room). A KO undead's body can be raised again.
/// </summary>
public sealed class RaiseUndead : ClassSkill
{
    public RaiseUndead(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override TargetOptions Options => new(IncludeKnockedOut: true);
    public override bool TargetFilter(BattleState state, int userId, int targetId) => NecromancerSkills.Raisable(state, userId, targetId);
    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId) => Necromancy.RaisePlacements(state, userId, targetId);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var first = context.TargetId!.Value;
        Raise(context, first, context.Placement!.Value);
        var others = state.Units.Where(u => u.Id != first && NecromancerSkills.Raisable(state, context.UserId, u.Id)).Select(u => u.Id).ToList();
        if (others.Count > 0)
        {
            var second = ClassHelpers.Pick(state, others);
            var places = Necromancy.RaisePlacements(state, context.UserId, second);
            if (places.Count > 0) Raise(context, second, ClassHelpers.Pick(state, places));
        }
        context.Settle();
    }

    private static void Raise(SkillContext context, int body, MoveOption place)
    {
        Necromancy.Raise(context.State, context.UserId, body, place.Row, place.Index, NecromancerSkills.UndeadHp);
        var unit = context.State.GetUnit(body);
        if (unit.CreatureId is { } cid && context.Catalog!.Content.CreaturesById.TryGetValue(cid, out var def))
        {
            unit.LearnSkill(def.Attack);
            unit.AutoSkillId = def.Attack;                                  // an undead uses its creature's own unique attack on its turns
        }
    }
}

/// <summary>Corpse Explosion: a body or an undead of the Necromancer bursts into poison gas on its spot and the spots next to it. The body stays where it is and takes no damage.</summary>
public sealed class CorpseExplosion : ClassSkill
{
    public CorpseExplosion(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(IncludeKnockedOut: true);

    public override bool TargetFilter(BattleState state, int userId, int targetId)
    {
        var u = state.GetUnit(targetId);
        if (u.IsRemoved || !state.Formation.Contains(targetId)) return false;
        return u.IsKnockedOut || (u.IsUndead && u.SummonerId == userId);
    }

    public override void Resolve(SkillContext context)
    {
        var ground = context.State.Ground;
        var spot = ground.SpotOf(context.TargetId!.Value);
        foreach (var s in new[] { spot }.Concat(PyromancerSkills.Beside(spot)))
            ground.Place(GroundKind.Poison, s, UniqueAttackSkill.GasLength, context.UserId);
        context.Settle();
    }
}

/// <summary>Desecrate: a KO'd body cannot be revived for 40. It blocks every revive (Life for Life, Bound Resurrect, restoring an undead); Raise Undead still works.</summary>
public sealed class Desecrate : ClassSkill
{
    public Desecrate(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(IncludeKnockedOut: true);
    public override bool TargetFilter(BattleState state, int userId, int targetId) => state.GetUnit(targetId).IsKnockedOut && !state.GetUnit(targetId).IsRemoved;

    public override void Resolve(SkillContext context)
    {
        context.State.GetUnit(context.TargetId!.Value).NoReviveUntil = context.State.Timeline.Now + NecromancerSkills.DesecrateLength;
        context.Settle();
    }
}

/// <summary>Lich Form: healing hurts the Necromancer, but its undead have +2 Power. <see cref="Unit.HealingHurts"/> and <see cref="BattleState.PowerOf"/> read this keystone.</summary>
public sealed class LichForm : KeystoneSkill
{
    public LichForm(SkillDef def) : base(def) { }
}

/// <summary>Bone Armor: for each undead it has, the Necromancer has a Ward of 2 that renews every turn (one Ward of 2 per undead, reset at each of its turns).</summary>
public sealed class BoneArmor : KeystoneSkill
{
    public BoneArmor(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddTurnStartHook((st, unitId) =>
        {
            if (unitId != ownerId) return;
            var count = NecromancerSkills.UndeadOf(st, ownerId).Count();
            if (count == 0) return;
            Statuses.Apply(st, new StatusApplication(ownerId, StatusIds.Ward, Magnitude: count * DamageMath.Scale(NecromancerSkills.BoneArmorWard, st.PowerOf(ownerId)),
                SourceUnitId: ownerId, EndsAtNextTurnOf: ownerId));
        });
}

/// <summary>Death's Bounty: when a unit is KO next to the Necromancer, the Necromancer heals 5.</summary>
public sealed class DeathsBounty : KeystoneSkill
{
    public DeathsBounty(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            if (!st.GetUnit(ownerId).IsKnockedOut) st.Heal(ownerId, ownerId, DamageMath.Scale(NecromancerSkills.BountyHealing, st.PowerOf(ownerId)));
        })
        {
            Condition = (st, t) => t.SubjectUnitId != ownerId && st.Formation.Contains(t.SubjectUnitId) && st.Formation.Contains(ownerId) && st.Formation.Adjacent(ownerId).Contains(t.SubjectUnitId),
        });
}
