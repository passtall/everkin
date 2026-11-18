using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Mender (skills.md): plain healing. Magical.</summary>
public static class MenderSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Mend(D("mend"));
        yield return new RainOfMending(D("rain_of_mending"));
        yield return new SunBurst(D("sun_burst"));
        yield return new OverflowingCare(D("overflowing_care"));
        yield return new Regrowth(D("regrowth"));
        yield return new Triage(D("triage"));
    }
}

/// <summary>Mend: heal a unit for 8-10 (double on a unit below 25% HP with Triage).</summary>
public sealed class Mend : ClassSkill
{
    public Mend(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var target = context.State.GetUnit(context.TargetId!.Value);
        var triage = context.User.KnownSkills.Contains("triage") && target.Hp * 4 < target.MaxHp;
        context.Heal(target.Id, triage ? new IntRange(16, 20) : new IntRange(8, 10));
    }
}

/// <summary>Rain of Mending: every ally heals 4-5.</summary>
public sealed class RainOfMending : ClassSkill
{
    public RainOfMending(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;
    public override bool IsArea => true;

    public override void Resolve(SkillContext context) =>
        context.HealBlock(AreaPatterns.AllOf(context.State, context.UserSide).Select(id => new SkillContext.HitSpec(id, new IntRange(4, 5))).ToList());
}

/// <summary>Sun Burst: every enemy takes 2-3 and every ally heals 2-3. No target, any row; the hits are one block, the heals follow.</summary>
public sealed class SunBurst : ClassSkill
{
    public SunBurst(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;
    public override bool IsAttack => true;
    public override bool IsArea => true;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        context.HitBlock(AreaPatterns.AllOf(state, context.UserSide.Opposite()).Select(id => new SkillContext.HitSpec(id, new IntRange(2, 3))).ToList());
        if (context.Over) return;
        context.HealBlock(AreaPatterns.AllOf(state, context.UserSide).Where(id => !state.GetUnit(id).IsKnockedOut)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(2, 3))).ToList());
    }
}

/// <summary>Overflowing Care: healing beyond the target's max HP goes to its adjacent ally with the lowest HP. The work is done by the healing code, which reads this keystone.</summary>
public sealed class OverflowingCare : KeystoneSkill
{
    public OverflowingCare(SkillDef def) : base(def) { }
}

/// <summary>Regrowth: a unit the Mender heals also gains a Ward of 2 until its next turn. The healing code applies the Ward.</summary>
public sealed class Regrowth : KeystoneSkill
{
    public Regrowth(SkillDef def) : base(def) { }
}

/// <summary>Triage: Mend heals double on units below 25% HP. Mend reads this keystone.</summary>
public sealed class Triage : KeystoneSkill
{
    public Triage(SkillDef def) : base(def) { }
}
