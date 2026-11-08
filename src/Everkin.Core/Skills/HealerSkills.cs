using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Healer's skills (skills.md Healer): Mender, Cleric and Lifebinder. Every Healer active is `magical`.</summary>
public static class HealerSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new Mend(D("mend"));
        yield return new RainOfMending(D("rain_of_mending"));
        yield return new OverflowingCare(D("overflowing_care"));
        yield return new Downpour(D("downpour"));
        yield return new Absolve(D("absolve"));
        yield return new Dispel(D("dispel"));
        yield return new MassDispel(D("mass_dispel"));
        yield return new StealBlessing(D("steal_blessing"));
        yield return new TurnBack(D("turn_back"));
        yield return new Revive(D("revive"));
        yield return new LifeLink(D("life_link"));
        yield return new BoundResurrect(D("bound_resurrect"));
        yield return new BurnoutHeal(D("burnout_heal"));
        yield return new LifeForLife(D("life_for_life"));
        yield return new BoundStrength(D("bound_strength"));
    }

    /// <summary>The rows of either side that have a living unit within the reach: what a skill that picks a row may pick.</summary>
    internal static IReadOnlyList<RowTarget> RowsInReach(BattleState state, int userId, Reach reach)
    {
        var rows = new List<RowTarget>();
        foreach (var side in new[] { Side.A, Side.B })
            for (var r = 0; r < Formation.RowCount; r++)
                if (state.Formation.Units(side, (Row)r).Any(id => !state.GetUnit(id).IsKnockedOut && Targeting.InReach(state, userId, id, reach)))
                    rows.Add(new RowTarget(side, (Row)r));
        return rows;
    }

    /// <summary>Whether a KO body or an undead can be brought back by a resurrection (section 10.3, 11.5).</summary>
    internal static bool CanBeRevived(BattleState state, int unitId)
    {
        var u = state.GetUnit(unitId);
        if (u.IsRemoved || !state.Formation.Contains(unitId)) return false;
        return u.IsUndead ? Necromancy.CanRestore(state, unitId) : u.IsKnockedOut;
    }

    /// <summary>A quarter of the maximum HP, rounded down, minimum 1 (Revive, Bound Resurrect).</summary>
    internal static int QuarterHp(Unit u) => Math.Max(1, (u.IsUndead ? u.OriginalMaxHp ?? u.MaxHp : u.MaxHp) / 4);

    /// <summary>The helpful statuses a unit has: its Ward and every status of helpful polarity (stealth is a flag and is handled apart).</summary>
    internal static IReadOnlyList<string> HelpfulStatuses(Unit u)
    {
        var list = u.StatusInstances.Keys.Where(id => StatusRules.PolarityOf(id) == Polarity.Helpful).ToList();
        if (u.Ward > 0 && !list.Contains(StatusIds.Ward)) list.Add(StatusIds.Ward);
        return list;
    }

    /// <summary>Picks one of several by the battle's seeded roll, each with equal chance; a single choice costs no roll.</summary>
    internal static T Pick<T>(BattleState state, IReadOnlyList<T> options) =>
        options.Count == 1 ? options[0] : options[state.Random.NextInt(options.Count)];
}

/// <summary>Base of the Healer's actives: `magical`, so Silenced blocks them.</summary>
public abstract class HealerSkill : ProfessionSkill
{
    protected HealerSkill(ProfessionSkillDef def) : base(def) { }
    public override bool Magical => true;
    public override EffectTags Tags => EffectTags.Magical;
}

// ------------------------------------------------------------------------------------------------ Mender

/// <summary>Mend: heal one unit for 8–10.</summary>
public sealed class Mend : HealerSkill
{
    public Mend(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override void Resolve(SkillContext context) => context.Heal(context.TargetId!.Value, new IntRange(8, 10));
}

/// <summary>Rain of Mending: pick a row: every unit in it is healed for 3–4. With Downpour the row behind is healed for half.</summary>
public sealed class RainOfMending : HealerSkill
{
    public RainOfMending(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override bool IsArea => true;

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        var heals = context.State.Formation.Units(row.Side, row.Row)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(3, 4))).ToList();
        if (context.User.KnownSkills.Contains("downpour") && (int)row.Row + 1 < Formation.RowCount)
            heals.AddRange(context.State.Formation.Units(row.Side, (Row)((int)row.Row + 1))
                .Select(id => new SkillContext.HitSpec(id, new IntRange(1, 2))));            // half of 3–4, rounded down
        context.HealBlock(heals);
    }
}

/// <summary>Overflowing Care: healing beyond the target's maximum HP goes to its adjacent ally with the lowest HP.</summary>
public sealed class OverflowingCare : PassiveSkill
{
    public OverflowingCare(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Downpour (modifier of Rain of Mending): it also heals the row behind for half.</summary>
public sealed class Downpour : ModifierSkill
{
    public Downpour(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Cleric

/// <summary>Absolve: remove every harmful status from one unit; its next turn is pushed back 2. A cleanse also ends stealth.</summary>
public sealed class Absolve : HealerSkill
{
    public Absolve(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        Statuses.RemoveAll(context.State, id, Polarity.Harmful);
        Stealth.Break(context.State, id, StealthCause.Revealed);
        if (context.State.Timeline.TurnOf(id) is { } turn) context.State.Timeline.PushBack(turn, 2);
        context.Settle();
    }
}

/// <summary>Dispel: remove every helpful status (Ward, Haste, stealth and others) from a unit within distance 3.</summary>
public sealed class Dispel : HealerSkill
{
    public Dispel(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        Statuses.RemoveAll(context.State, id, Polarity.Helpful);
        Stealth.Break(context.State, id, StealthCause.Revealed);
        context.Settle();
    }
}

/// <summary>Mass Dispel: remove every helpful status from every unit in one row.</summary>
public sealed class MassDispel : HealerSkill
{
    public MassDispel(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override bool IsArea => true;

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).Where(i => !context.State.GetUnit(i).IsKnockedOut).ToArray())
        {
            Statuses.RemoveAll(context.State, id, Polarity.Helpful);
            Stealth.Break(context.State, id, StealthCause.Revealed);
        }
        context.Settle();
    }
}

/// <summary>
/// Steal Blessing: move one helpful status from one unit onto another. The skill names both units; if the first has several helpful
/// statuses the seeded roll picks one (decided 2026-11-08).
/// </summary>
public sealed class StealBlessing : HealerSkill
{
    public StealBlessing(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override bool PairFilter(BattleState state, int userId, int firstId, int secondId) =>
        HealerSkills.HelpfulStatuses(state.GetUnit(firstId)).Count > 0;

    public override void Resolve(SkillContext context)
    {
        var from = context.TargetId!.Value;
        var to = context.SecondTargetId!.Value;
        var status = HealerSkills.Pick(context.State, HealerSkills.HelpfulStatuses(context.State.GetUnit(from)));
        Statuses.Transfer(context.State, from, to, status);
        context.Settle();
    }
}

/// <summary>
/// Turn Back: move one harmful status from a unit onto the unit that applied it. If several qualify (their applier still standing),
/// the seeded roll picks one (decided 2026-11-08).
/// </summary>
public sealed class TurnBack : HealerSkill
{
    public TurnBack(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    private static IReadOnlyList<StatusInstance> Returnable(BattleState state, int unitId) =>
        state.GetUnit(unitId).StatusInstances.Values
            .Where(s => StatusRules.PolarityOf(s.Id) == Polarity.Harmful
                && s.SourceUnitId is { } src && src != unitId
                && !state.GetUnit(src).IsKnockedOut && !state.GetUnit(src).IsRemoved)
            .ToArray();

    public override bool TargetFilter(BattleState state, int userId, int targetId) => Returnable(state, targetId).Count > 0;

    public override void Resolve(SkillContext context)
    {
        var from = context.TargetId!.Value;
        var chosen = HealerSkills.Pick(context.State, Returnable(context.State, from));
        Statuses.Transfer(context.State, from, chosen.SourceUnitId!.Value, chosen.Id);
        context.Settle();
    }
}

// ------------------------------------------------------------------------------------------------ Lifebinder

/// <summary>Revive: revive a KO unit at 25% of its maximum HP (rounded down, minimum 1). On an undead it restores the original creature.</summary>
public sealed class Revive : HealerSkill
{
    public Revive(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(IncludeKnockedOut: true);   // a resurrection can target allied or enemy bodies

    public override bool TargetFilter(BattleState state, int userId, int targetId) => HealerSkills.CanBeRevived(state, targetId);

    public override void Resolve(SkillContext context)
    {
        var id = context.TargetId!.Value;
        context.ReviveOrRestore(id, HealerSkills.QuarterHp(context.State.GetUnit(id)));
    }
}

/// <summary>Life Link: link two units for 10 time units: healing on one also heals the other for half.</summary>
public sealed class LifeLink : HealerSkill
{
    public LifeLink(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var a = context.TargetId!.Value;
        var b = context.SecondTargetId!.Value;
        foreach (var id in new[] { a, b })
            // A unit has one link: the unit it was linked to loses its side of it too.
            if (Statuses.Get(state.GetUnit(id), StatusIds.LifeLink) is { } old && old.Magnitude is var other and not -1
                && Statuses.Get(state.GetUnit(other), StatusIds.LifeLink)?.Magnitude == id)
                Statuses.Remove(state, other, StatusIds.LifeLink, StatusEndCause.Cleansed);
        Statuses.Apply(state, new StatusApplication(a, StatusIds.LifeLink, Duration: 10, Magnitude: b, SourceUnitId: context.UserId));
        Statuses.Apply(state, new StatusApplication(b, StatusIds.LifeLink, Duration: 10, Magnitude: a, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>
/// Bound Resurrect: revive a KO unit at 25% of its maximum HP (rounded down, minimum 1); it and the Healer share one HP pool (game_system
/// section 10.4): the pool's maximum is the sum of both maximum HP and it starts at the Healer's current HP plus the revival HP.
/// </summary>
public sealed class BoundResurrect : HealerSkill
{
    public BoundResurrect(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(IncludeKnockedOut: true);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        HealerSkills.CanBeRevived(state, targetId) && state.SharedPoolOf(userId) is null;

    public override string? Unavailable(BattleState state, int userId) =>
        state.SharedPoolOf(userId) is null ? null : "the Healer already shares its HP";

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var id = context.TargetId!.Value;
        context.ReviveOrRestore(id, HealerSkills.QuarterHp(state.GetUnit(id)));
        if (state.GetUnit(id).IsKnockedOut || context.User.IsKnockedOut) return;
        var pool = context.User.Hp + state.GetUnit(id).Hp;
        state.AddSharedPool(context.UserId, id, pool, boundStrength: context.User.KnownSkills.Contains("bound_strength"));
    }
}

/// <summary>Bound Strength (modifier of Bound Resurrect): while the two units share their HP pool, both have 1 less Delay on every skill.</summary>
public sealed class BoundStrength : ModifierSkill
{
    public BoundStrength(ProfessionSkillDef def) : base(def) { }
}

/// <summary>
/// Burnout Heal: heal every unit around the Healer for 6–8; the Healer is KO. The heals are one simultaneous block; the Healer's KO resolves
/// after it and before reactions are collected, so its own reactions do not fire (section 10.4).
/// </summary>
public sealed class BurnoutHeal : HealerSkill
{
    public BurnoutHeal(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override Delivery? Delivery => Battle.Delivery.Direct;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var around = state.Formation.Adjacent(context.UserId).Where(id => !state.GetUnit(id).IsKnockedOut)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(6, 8))).ToArray();
        context.HealBlock(around);
        if (!context.User.IsKnockedOut)
        {
            state.DealDamage(context.UserId, context.UserId, context.User.Hp, DamageSource.Unreducible);   // the sacrifice
            context.Settle();
        }
    }
}

/// <summary>Life for Life: the Healer gives up half its current HP to revive a KO unit with that much HP.</summary>
public sealed class LifeForLife : HealerSkill
{
    public LifeForLife(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(IncludeKnockedOut: true);

    public override string? Unavailable(BattleState state, int userId) =>
        state.GetUnit(userId).Hp >= 2 ? null : "needs at least 2 HP to give up half";

    public override bool TargetFilter(BattleState state, int userId, int targetId) => HealerSkills.CanBeRevived(state, targetId);

    public override void Resolve(SkillContext context)
    {
        var give = context.User.Hp / 2;
        context.State.DealDamage(context.UserId, context.UserId, give, DamageSource.Unreducible);   // given up: no shield or reduction applies
        context.ReviveOrRestore(context.TargetId!.Value, give);
    }
}
