using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Cleric (skills.md): removes statuses, good and bad. Magical.</summary>
public static class ClericSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Absolve(D("absolve"));
        yield return new Dispel(D("dispel"));
        yield return new TurnBack(D("turn_back"));
        yield return new Sanctity(D("sanctity"));
        yield return new PurifyingAura(D("purifying_aura"));
        yield return new Judgment(D("judgment"));
    }
}

/// <summary>Absolve: remove every harmful status from a unit; its next turn is pushed back 2.</summary>
public sealed class Absolve : ClassSkill
{
    public Absolve(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        Statuses.RemoveAll(context.State, id, Polarity.Harmful, byUnitId: context.UserId);
        context.State.PushTurnBack(id, 2);
        context.Settle();
    }
}

/// <summary>Dispel: remove every helpful status (Ward, songs, stealth, enchantments and others) from a unit. With Judgment each status removed deals 4 to that unit.</summary>
public sealed class Dispel : ClassSkill
{
    public const int JudgmentDamage = 4;
    public Dispel(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        var removed = Statuses.RemoveAll(context.State, id, Polarity.Helpful, byUnitId: context.UserId).Count;
        if (context.State.GetUnit(id).IsStealthed)
        {
            Stealth.Break(context.State, id, StealthCause.Revealed);                  // a cleansing skill that hits a stealthed unit removes its stealth
            removed++;
        }
        if (context.User.KnownSkills.Contains("judgment") && !context.State.GetUnit(id).IsKnockedOut)
            for (var i = 0; i < removed && !context.State.GetUnit(id).IsKnockedOut; i++)
                context.State.DealDamage(context.UserId, id, context.Scaled(JudgmentDamage));
        context.Settle();
    }
}

/// <summary>Turn Back: move one harmful status from a unit onto the unit that applied it (picked by the seeded roll when several qualify).</summary>
public sealed class TurnBack : ClassSkill
{
    public TurnBack(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    private static IReadOnlyList<StatusInstance> Candidates(BattleState state, int unitId) =>
        state.GetUnit(unitId).StatusInstances.Values
            .Where(s => StatusRules.PolarityOf(s.Id) == Polarity.Harmful && s.SourceUnitId is { } src && src != unitId
                && !state.GetUnit(src).IsKnockedOut && !state.GetUnit(src).IsRemoved && state.Formation.Contains(src))
            .ToArray();

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var id = context.TargetId!.Value;
        var options = Candidates(state, id);
        if (options.Count > 0)
        {
            var pick = ClassHelpers.Pick(state, options);
            Statuses.Transfer(state, id, pick.SourceUnitId!.Value, pick.Id, context.UserId);
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
/// Purifying Aura: harmful statuses on allies next to the Cleric last half as long: their clocks run down twice as fast while the ally stands next to it.
/// An aura sync keeps each timed harmful status at the right rate after every event.
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
                var covered = clerics.Any(c => c.Id != unit.Id && c.Side == unit.Side && st.Formation.Adjacent(c.Id).Contains(unit.Id));
                foreach (var status in unit.StatusInstances.Values.Where(s => s.MarkerEntryId is not null && StatusRules.PolarityOf(s.Id) == Polarity.Harmful).ToArray())
                    Statuses.SetRate(st, status, covered ? 2 : 1);
            }
        });
}

/// <summary>Judgment: each status the Cleric's Dispel removes deals 4 to that unit. Dispel reads this keystone.</summary>
public sealed class Judgment : KeystoneSkill
{
    public Judgment(SkillDef def) : base(def) { }
}
