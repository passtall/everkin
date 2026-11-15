using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Hexer (skills.md): curses. Magical.</summary>
public static class HexerSkills
{
    public const int CurseLength = 20;
    public const int WitheringLength = 40;
    public const int WitheringDamage = 3;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new CurseOfBetrayal(D("curse_of_betrayal"));
        yield return new CurseOfWeight(D("curse_of_weight"));
        yield return new WitheringCurse(D("withering_curse"));
        yield return new WitchsPatience(D("witchs_patience"));
        yield return new LingeringMalice(D("lingering_malice"));
        yield return new SpreadingCurse(D("spreading_curse"));
    }

    /// <summary>
    /// Puts a curse on a unit for <paramref name="length"/> time units. With Witch's Patience it lasts until removed, but the Hexer can keep only one curse
    /// at a time: its earlier curse, on any unit, is removed.
    /// </summary>
    internal static void Curse(SkillContext context, string statusId, int length, int magnitude = 0)
    {
        var state = context.State;
        var patience = context.User.KnownSkills.Contains("witchs_patience");
        if (patience)
            foreach (var unit in state.Units.ToArray())
                foreach (var status in unit.StatusInstances.Values.Where(s => StatusIds.IsCurse(s.Id) && s.SourceUnitId == context.UserId).ToArray())
                    Statuses.Remove(state, unit.Id, status.Id, StatusEndCause.Expired);
        Statuses.Apply(state, new StatusApplication(context.TargetId!.Value, statusId, patience ? null : length, magnitude, context.UserId));
        context.Settle();
    }
}

/// <summary>Curse of Betrayal: for 20, healing the unit would receive goes to the Hexer's unit with the lowest HP instead.</summary>
public sealed class CurseOfBetrayal : ClassSkill
{
    public CurseOfBetrayal(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override void Resolve(SkillContext context) => HexerSkills.Curse(context, StatusIds.Betrayed, HexerSkills.CurseLength);
}

/// <summary>Curse of Weight: for 20, a unit cannot Move or become airborne.</summary>
public sealed class CurseOfWeight : ClassSkill
{
    public CurseOfWeight(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override void Resolve(SkillContext context) => HexerSkills.Curse(context, StatusIds.Weighted, HexerSkills.CurseLength);
}

/// <summary>Withering Curse: a unit takes 3 every 10 for 40 (scaled; not a hit, Wards do not absorb it).</summary>
public sealed class WitheringCurse : ClassSkill
{
    public WitheringCurse(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override void Resolve(SkillContext context) =>
        HexerSkills.Curse(context, StatusIds.Withering, HexerSkills.WitheringLength, context.Scaled(HexerSkills.WitheringDamage));
}

/// <summary>Witch's Patience: the Hexer's curses last until removed, but it can keep only one curse at a time. The cursing code reads this keystone.</summary>
public sealed class WitchsPatience : KeystoneSkill
{
    public WitchsPatience(SkillDef def) : base(def) { }
}

/// <summary>Lingering Malice: when an enemy removes one of the Hexer's curses, the remover is Silenced for 10.</summary>
public sealed class LingeringMalice : KeystoneSkill
{
    public const int SilenceLength = 10;
    public LingeringMalice(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStatusRemovedHook((st, unitId, status, byUnitId) =>
        {
            var owner = st.GetUnit(ownerId);
            var remover = st.GetUnit(byUnitId);
            if (owner.IsKnockedOut || remover.IsKnockedOut || remover.IsRemoved) return;
            if (!StatusIds.IsCurse(status.Id) || status.SourceUnitId != ownerId || remover.Side == owner.Side) return;
            Statuses.Apply(st, new StatusApplication(byUnitId, StatusIds.Silenced, SilenceLength, SourceUnitId: ownerId));
        });
}

/// <summary>Spreading Curse: when a cursed unit is KO, its curse jumps to the nearest enemy, keeping its remaining time.</summary>
public sealed class SpreadingCurse : KeystoneSkill
{
    public SpreadingCurse(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            var curses = st.GetUnit(ctx.Trigger.SubjectUnitId).StatusesAtKo
                .Where(s => StatusIds.IsCurse(s.Id) && s.SourceUnitId == ownerId && s.TimeLeft != 0).ToList();
            if (curses.Count == 0 || Targeting.PickNearest(st, ownerId, new TargetOptions(TargetSide.EnemySideOnly)) is not { } next) return;
            foreach (var curse in curses)
                Statuses.Apply(st, new StatusApplication(next, curse.Id, curse.TimeLeft, curse.Magnitude, ownerId));
        })
        {
            Condition = (st, t) => !st.GetUnit(ownerId).IsKnockedOut && t.SubjectUnitId != ownerId
                && st.GetUnit(t.SubjectUnitId).StatusesAtKo.Any(s => StatusIds.IsCurse(s.Id) && s.SourceUnitId == ownerId),
        });
}
