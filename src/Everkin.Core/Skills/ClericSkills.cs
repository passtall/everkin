using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Cleric (skills.md): removes harmful statuses, punishes the afflicted. Magical.</summary>
public static class ClericSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Absolve(D("absolve"));
        yield return new SmiteTheAfflicted(D("smite_the_afflicted"));
        yield return new DivineIntervention(D("divine_intervention"));
        yield return new Sanctity(D("sanctity"));
        yield return new PurifyingAura(D("purifying_aura"));
        yield return new Judgment(D("judgment"));
    }

    public const int JudgmentDamage = 4;

    /// <summary>
    /// Removes every harmful status from a unit. With Judgment, each status removed deals 4 (scaled) to the unit that applied it, if that is another unit
    /// that is still on the board and not KO (skills.md Cleric).
    /// </summary>
    public static void Cleanse(SkillContext context, int unitId)
    {
        var state = context.State;
        var sources = state.GetUnit(unitId).StatusInstances.Values
            .Where(s => StatusRules.PolarityOf(s.Id) == Polarity.Harmful)
            .ToDictionary(s => s.Id, s => s.SourceUnitId);
        var removed = Statuses.RemoveAll(state, unitId, Polarity.Harmful, byUnitId: context.UserId);
        if (!context.User.KnownSkills.Contains("judgment")) return;
        foreach (var id in removed)
            if (sources.TryGetValue(id, out var source) && source is { } applier && applier != unitId
                && !state.GetUnit(applier).IsKnockedOut && !state.GetUnit(applier).IsRemoved && state.Formation.Contains(applier))
                state.DealDamage(context.UserId, applier, context.Scaled(JudgmentDamage));
    }
}

/// <summary>Absolve: remove every harmful status from a unit; for 10 it cannot get new harmful statuses.</summary>
public sealed class Absolve : ClassSkill
{
    public const int ImmunityTime = 10;
    public Absolve(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("absolved_immunity")) return;
        state.AddStatusImmunity((st, unitId, statusId) =>
            StatusRules.PolarityOf(statusId) == Polarity.Harmful && Statuses.Has(st.GetUnit(unitId), StatusIds.Absolved));
    }

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        ClericSkills.Cleanse(context, id);
        if (!context.State.GetUnit(id).IsKnockedOut)
            Statuses.Apply(context.State, new StatusApplication(id, StatusIds.Absolved, Duration: ImmunityTime, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Smite the Afflicted: hit an enemy for 3-4, plus 2 for each harmful status on it (up to 6 extra).</summary>
public sealed class SmiteTheAfflicted : ClassSkill
{
    public const int PerStatus = 2;
    public const int MaxExtra = 6;
    public SmiteTheAfflicted(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        var harmful = context.State.GetUnit(id).StatusInstances.Keys.Count(s => StatusRules.PolarityOf(s) == Polarity.Harmful);
        var extra = Math.Min(harmful * PerStatus, MaxExtra);
        context.Hit(id, new IntRange(3 + extra, 4 + extra));
        if (context.Over) return;
        context.Settle();
    }
}

/// <summary>Divine Intervention: every ally loses every harmful status and gains a Ward of 3 until the Cleric's next turn.</summary>
public sealed class DivineIntervention : ClassSkill
{
    public DivineIntervention(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        foreach (var id in state.Formation.Units(context.UserSide).ToArray())
        {
            if (state.GetUnit(id).IsKnockedOut) continue;
            ClericSkills.Cleanse(context, id);
            if (state.GetUnit(id).IsKnockedOut) continue;
            Statuses.Apply(state, new StatusApplication(id, StatusIds.Ward, Magnitude: context.Scaled(3),
                SourceUnitId: context.UserId, EndsAtNextTurnOf: context.UserId));
        }
        context.Settle();
    }
}

/// <summary>Sanctity: the Cleric cannot get harmful statuses.</summary>
public sealed class Sanctity : KeystoneSkill
{
    public Sanctity(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStatusImmunity((st, unitId, statusId) => unitId == ownerId && StatusRules.PolarityOf(statusId) == Polarity.Harmful);
}

/// <summary>
/// Purifying Aura: harmful statuses on allies in the Cleric's row (the Cleric included) last half as long: their clocks run down twice as fast while the
/// ally stands in that row. An aura sync keeps each timed harmful status at the right rate after every event.
/// </summary>
public sealed class PurifyingAura : KeystoneSkill
{
    public PurifyingAura(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAuraSync("purifying_aura", st =>
        {
            var clerics = st.Units.Where(u => !u.IsKnockedOut && !u.IsRemoved && st.Formation.Contains(u.Id) && u.KnownSkills.Contains("purifying_aura")).ToList();
            foreach (var unit in st.Units)
            {
                if (unit.IsKnockedOut || unit.IsRemoved || !st.Formation.Contains(unit.Id)) continue;
                var row = st.Formation.PositionOf(unit.Id).Row;
                var covered = clerics.Any(c => c.Side == unit.Side && st.Formation.PositionOf(c.Id).Row == row);
                foreach (var status in unit.StatusInstances.Values.Where(s => s.MarkerEntryId is not null && StatusRules.PolarityOf(s.Id) == Polarity.Harmful).ToArray())
                    Statuses.SetRate(st, status, covered ? 2 : 1);
            }
        });
}

/// <summary>Judgment: each harmful status that Absolve or Divine Intervention removes deals 4 to the enemy that applied it. <see cref="ClericSkills.Cleanse"/> reads this keystone.</summary>
public sealed class Judgment : KeystoneSkill
{
    public Judgment(SkillDef def) : base(def) { }
}
