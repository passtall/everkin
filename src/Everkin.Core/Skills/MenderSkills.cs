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
        yield return new SearingLight(D("searing_light"));
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

/// <summary>Rain of Mending: pick a row: every unit in it heals 3-4, and every unit in the row behind it heals half that.</summary>
public sealed class RainOfMending : ClassSkill
{
    public RainOfMending(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => ClassHelpers.AllRows();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        var heals = context.State.Formation.Units(row.Side, row.Row)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(3, 4))).ToList();
        if ((int)row.Row + 1 < Formation.RowCount)
            heals.AddRange(context.State.Formation.Units(row.Side, (Row)((int)row.Row + 1))
                .Select(id => new SkillContext.HitSpec(id, new IntRange(1, 2))));            // half of 3-4, rounded down
        context.HealBlock(heals);
    }
}

/// <summary>Searing Light: hit for 4-5; undead take double.</summary>
public sealed class SearingLight : ClassSkill
{
    public SearingLight(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var undead = context.State.GetUnit(context.TargetId!.Value).IsUndead;
        context.Hit(context.TargetId!.Value, undead ? new IntRange(8, 10) : new IntRange(4, 5));
    }
}

/// <summary>Overflowing Care: healing beyond the target's max HP goes to its adjacent ally with the lowest HP. The work is done by the healing code, which reads this keystone.</summary>
public sealed class OverflowingCare : KeystoneSkill
{
    public OverflowingCare(SkillDef def) : base(def) { }
}

/// <summary>Regrowth: a unit the Mender heals also regains 2 HP every 10 for 20. The healing code applies the status.</summary>
public sealed class Regrowth : KeystoneSkill
{
    public Regrowth(SkillDef def) : base(def) { }
}

/// <summary>Triage: Mend heals double on units below 25% HP. Mend reads this keystone.</summary>
public sealed class Triage : KeystoneSkill
{
    public Triage(SkillDef def) : base(def) { }
}
