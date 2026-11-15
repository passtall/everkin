using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Chemist (skills.md): poison and fire on the ground, potions. Physical.</summary>
public static class ChemistSkills
{
    public const int BigFinishDelay = 2;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new PoisonGas(D("poison_gas"));
        yield return new Firebomb(D("firebomb"));
        yield return new HealingDraught(D("healing_draught"));
        yield return new BigFinish(D("big_finish"));
        yield return new PotentBrew(D("potent_brew"));
        yield return new Volatile(D("volatile"));
    }

    /// <summary>A spot, the spots next to it, and with Big Finish the spots next to those (five spots in all).</summary>
    internal static IEnumerable<Spot> Area(SkillContext context, Spot spot)
    {
        var spots = new List<Spot> { spot };
        spots.AddRange(PyromancerSkills.Beside(spot));
        if (context.User.KnownSkills.Contains("big_finish"))
            foreach (var next in PyromancerSkills.Beside(spot).ToList())
                foreach (var far in PyromancerSkills.Beside(next))
                    if (!spots.Contains(far)) spots.Add(far);
        return spots;
    }
}

/// <summary>Poison Gas: poison gas on a unit's spot and the spots next to it.</summary>
public sealed class PoisonGas : ClassSkill
{
    public PoisonGas(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Install(BattleState state, int ownerId) =>
        state.AddSkillDelayModifier((st, userId, skillId) => userId == ownerId && skillId is "poison_gas" or "firebomb" && st.GetUnit(ownerId).KnownSkills.Contains("big_finish") ? ChemistSkills.BigFinishDelay : 0);

    public override void Resolve(SkillContext context)
    {
        var ground = context.State.Ground;
        foreach (var s in ChemistSkills.Area(context, ground.SpotOf(context.TargetId!.Value)))
            ground.Place(GroundKind.Poison, s, UniqueAttackSkill.GasLength, context.UserId);
        context.Settle();
    }
}

/// <summary>Firebomb: fire on a unit's spot and the spots next to it, for 20. An intercepted Firebomb lands on the interceptor's spot.</summary>
public sealed class Firebomb : ClassSkill
{
    public Firebomb(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var receiver = context.InterceptEffect(context.TargetId!.Value);
        var ground = context.State.Ground;
        foreach (var s in ChemistSkills.Area(context, ground.SpotOf(receiver)))
            ground.Place(GroundKind.Burning, s, PyromancerSkills.FireLength, context.UserId);
        context.Settle();
    }
}

/// <summary>Healing Draught: a unit heals 6-7. With Potent Brew it also removes one harmful status (picked by the seeded roll).</summary>
public sealed class HealingDraught : ClassSkill
{
    public HealingDraught(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var report = context.Heal(context.TargetId!.Value, new IntRange(6, 7));
        if (!context.User.KnownSkills.Contains("potent_brew")) return;
        var receiver = report.Interceptions.Count > 0 ? report.Interceptions[0].InterceptorId : report.TargetId;
        var harmful = context.State.GetUnit(receiver).StatusInstances.Values.Where(s => StatusRules.PolarityOf(s.Id) == Polarity.Harmful).Select(s => s.Id).ToList();
        if (harmful.Count == 0 || context.State.GetUnit(receiver).IsKnockedOut) return;
        Statuses.Remove(context.State, receiver, ClassHelpers.Pick(context.State, harmful), StatusEndCause.Cleansed, context.UserId);
        context.Settle();
    }
}

/// <summary>Big Finish: Firebomb and Poison Gas also cover the spots next to those spots (five spots in all), but both have +2 Delay. The skills read this keystone.</summary>
public sealed class BigFinish : KeystoneSkill
{
    public BigFinish(SkillDef def) : base(def) { }
}

/// <summary>Potent Brew: Healing Draught also removes one harmful status. The skill reads this keystone.</summary>
public sealed class PotentBrew : KeystoneSkill
{
    public PotentBrew(SkillDef def) : base(def) { }
}

/// <summary>Volatile: the Chemist's fire and poison gas last twice as long. The ground code reads this keystone.</summary>
public sealed class Volatile : KeystoneSkill
{
    public Volatile(SkillDef def) : base(def) { }
}
