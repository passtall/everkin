using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Transmuter (skills.md): changes what a unit is: stats, glass, stone. Magical.</summary>
public static class TransmuterSkills
{
    public const int Length = 20;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Transmute(D("transmute"));
        yield return new GlassBody(D("glass_body"));
        yield return new Petrify(D("petrify"));
        yield return new LastingChange(D("lasting_change"));
        yield return new BrittleGlass(D("brittle_glass"));
        yield return new MirrorForm(D("mirror_form"));
    }
}

/// <summary>
/// Transmute: a unit's Power and Speed swap values for 20. With Mirror Form it can instead make two units trade their Power and Speed (the second unit is
/// the skill's optional second target). With Lasting Change it lasts until the end of the battle, but only one unit can be transmuted by the Transmuter at a time.
/// </summary>
public sealed class Transmute : ClassSkill
{
    public Transmute(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override SecondTargetChoice? SecondTargetFor(BattleState state, SkillCatalog catalog, int userId, int firstId) =>
        state.GetUnit(userId).KnownSkills.Contains("mirror_form")
            ? new SecondTargetChoice(Targeting.LegalTargets(state, userId, Options with { Delivery = Delivery }).Where(t => t != firstId).ToArray(), Required: false)
            : null;

    public override string? CheckUse(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        if (use.SecondTargetUnitId is not { } second) return null;
        if (!state.GetUnit(use.UserId).KnownSkills.Contains("mirror_form")) return "needs Mirror Form to trade with a second unit";
        if (second < 0 || second >= state.Units.Count || second == use.TargetUnitId) return "needs two different units";
        return Targeting.IsLegalTarget(state, use.UserId, second, Options with { Delivery = Delivery }) ? null : "not a legal target";
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var lasting = context.User.KnownSkills.Contains("lasting_change");
        int? length = lasting ? null : TransmuterSkills.Length;
        if (lasting)
            foreach (var unit in state.Units.ToArray())
                foreach (var id in new[] { StatusIds.Transmuted, StatusIds.Traded })
                    if (Statuses.Get(unit, id)?.SourceUnitId == context.UserId) Statuses.Remove(state, unit.Id, id, StatusEndCause.Consumed);

        var first = state.GetUnit(context.TargetId!.Value);
        if (context.SecondTargetId is { } secondId)
        {
            var second = state.GetUnit(secondId);
            Statuses.Apply(state, new StatusApplication(first.Id, StatusIds.Traded, length, second.Power * 100 + second.Speed, context.UserId));
            Statuses.Apply(state, new StatusApplication(second.Id, StatusIds.Traded, length, first.Power * 100 + first.Speed, context.UserId));
        }
        else Statuses.Apply(state, new StatusApplication(first.Id, StatusIds.Transmuted, length, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Glass Body: the next hit on a unit deals 50% more damage (double with Brittle Glass). It has no time limit: it waits for that hit.</summary>
public sealed class GlassBody : ClassSkill
{
    public GlassBody(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var brittle = context.User.KnownSkills.Contains("brittle_glass");
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.GlassBody, null, brittle ? 100 : 50, context.UserId));
        context.Settle();
    }
}

/// <summary>Petrify: a unit turns to stone: it is Stunned for 10.</summary>
public sealed class Petrify : ClassSkill
{
    public const int StunLength = 10;
    public Petrify(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.Stunned, StunLength, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Lasting Change: Transmute lasts until the end of the battle, but only one unit can be transmuted at a time. Transmute reads this keystone.</summary>
public sealed class LastingChange : KeystoneSkill
{
    public LastingChange(SkillDef def) : base(def) { }
}

/// <summary>Brittle Glass: Glass Body makes the next hit deal double instead of 50% more. Glass Body reads this keystone.</summary>
public sealed class BrittleGlass : KeystoneSkill
{
    public BrittleGlass(SkillDef def) : base(def) { }
}

/// <summary>Mirror Form: Transmute can instead make two units trade their Power and Speed. Transmute reads this keystone.</summary>
public sealed class MirrorForm : KeystoneSkill
{
    public MirrorForm(SkillDef def) : base(def) { }
}
