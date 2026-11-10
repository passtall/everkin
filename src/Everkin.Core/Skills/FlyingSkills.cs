using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Flying profession's skills (skills.md Flying): Mobility, Skywatch and Assault. Its actives are physical (not `magical`).
/// The rulings of 2026-11-10 (Take Off, Sky Lord, Scout Ahead, Watchtower, Airlift, Gust, Headwind, Boundless Sky) are in skills.md.
/// </summary>
public static class FlyingSkills
{
    public const int TakeOffLength = 20;
    public const int HighAltitudeLength = 40;
    public const int ScoutLength = 10;
    public const int WatchtowerEvery = 20;
    public const string WatchtowerTagPrefix = "watchtower:";

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new Airlift(D("airlift"));
        yield return new TakeOff(D("take_off"));
        yield return new HighAltitude(D("high_altitude"));
        yield return new SkyLord(D("sky_lord"));
        yield return new ScoutAhead(D("scout_ahead"));
        yield return new Spotter(D("spotter"));
        yield return new LongWatch(D("long_watch"));
        yield return new Watchtower(D("watchtower"));
        yield return new BoundlessSky(D("boundless_sky"));
        yield return new Gust(D("gust"));
        yield return new Headwind(D("headwind"));
        yield return new Turbulence(D("turbulence"));
        yield return new DustUp(D("dust_up"));
    }

    /// <summary>Scout Ahead on a side: every stealthed unit is revealed and none of the side can gain stealth for 10 time units.</summary>
    internal static void Scout(BattleState state, Side side)
    {
        foreach (var id in state.Formation.Units(side).ToArray())
            if (state.GetUnit(id).IsStealthed) Stealth.Break(state, id, StealthCause.Revealed);
        state.ScoutedUntil[(int)side] = state.Timeline.Now + ScoutLength;
    }

    /// <summary>Keeps Scout Ahead's "cannot gain stealth" in force. Installed once.</summary>
    internal static void InstallScoutBlocker(BattleState state)
    {
        if (!state.FirstInstall("scout_blocker")) return;
        state.AddStealthBlocker((st, unitId) => st.ScoutedUntil[(int)st.GetUnit(unitId).Side] is { } until && st.Timeline.Now < until);
    }

    /// <summary>The Watchtower's repeating auto event.</summary>
    internal static bool TryHandleAutoEvent(BattleState state, TimelineEntry entry)
    {
        if (entry.Tag is not { } tag || !tag.StartsWith(WatchtowerTagPrefix, StringComparison.Ordinal) || !int.TryParse(tag.AsSpan(WatchtowerTagPrefix.Length), out var id))
            return false;
        var unit = state.GetUnit(id);
        if (state.IsOver || unit.IsKnockedOut || unit.IsRemoved) return true;
        if (unit.IsAirborne) { Scout(state, unit.Side.Opposite()); Actions.Settle(state, id); }
        state.Timeline.Add(TimelineKind.AutoEvent, WatchtowerEvery, id, tag);
        return true;
    }
}

/// <summary>Base of the Flying profession's actives.</summary>
public abstract class FlyingSkill : ProfessionSkill
{
    protected FlyingSkill(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
}

// ------------------------------------------------------------------------------------------------ Mobility

/// <summary>Airlift: moves an ally to any free spot of your side.</summary>
public sealed class Airlift : FlyingSkill
{
    public Airlift(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.OwnSideOnly);

    public override bool TargetFilter(BattleState state, int userId, int targetId) => PlacementOptionsFor(state, userId, targetId).Count > 0;

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId)
    {
        var result = new List<MoveOption>();
        var unit = state.GetUnit(targetId);
        if (!Movement.IsMovable(unit) || Movement.IsImmune(state, targetId, state.GetUnit(userId).Side)) return result;
        var from = state.Formation.PositionOf(targetId);
        var formation = state.Formation.Clone();
        formation.Remove(targetId);
        for (var r = 0; r < Formation.RowCount; r++)
        {
            var count = formation.Count(from.Side, (Row)r);
            if (count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++)
                if (!((Row)r == from.Row && gap == from.Index)) result.Add(new MoveOption((Row)r, gap));
        }
        return result;
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        var place = context.Placement!.Value;
        state.Ground.Accrue();
        var from = state.Formation.Remove(target);
        state.Formation.Place(target, from.Side, place.Row, place.Index);
        var move = new UnitMoved(target, from, state.Formation.PositionOf(target), MoveCause.Forced);
        state.Emit(move);
        context.AfterForcedMove(new ForcedMoveResult(ForcedMoveStatus.Moved, new[] { move }, Array.Empty<int>()));
    }
}

/// <summary>Take Off: for 20 time units (40 with High Altitude) the unit is airborne.</summary>
public sealed class TakeOff : FlyingSkill
{
    public TakeOff(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId) =>
        Statuses.Has(state.GetUnit(userId), StatusIds.Weighted) ? "Curse of Weight: cannot become airborne" : null;

    public override void Resolve(SkillContext context)
    {
        var length = context.User.KnownSkills.Contains("high_altitude") ? FlyingSkills.HighAltitudeLength : FlyingSkills.TakeOffLength;
        Statuses.Apply(context.State, new StatusApplication(context.UserId, StatusIds.Airborne, length, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>High Altitude (modifier of Take Off): Take Off lasts 40 time units. Read by Take Off.</summary>
public sealed class HighAltitude : ModifierSkill
{
    public HighAltitude(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Sky Lord (keystone): the unit is always airborne, but has 30% less HP (rounded down).</summary>
public sealed class SkyLord : PassiveSkill
{
    public SkyLord(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        var unit = state.GetUnit(ownerId);
        unit.MaxHp = Math.Max(1, unit.MaxHp * 70 / 100);
        unit.Hp = Math.Min(unit.Hp, unit.MaxHp);
        Statuses.Apply(state, new StatusApplication(ownerId, StatusIds.Airborne, SourceUnitId: ownerId));
        state.AddAuraSync("sky_lord", st =>
        {
            foreach (var u in st.Units)
                if (u.KnownSkills.Contains("sky_lord") && !u.IsKnockedOut && !u.IsRemoved && !u.IsAirborne)
                    Statuses.Apply(st, new StatusApplication(u.Id, StatusIds.Airborne, SourceUnitId: u.Id));      // refused under Curse of Weight
        });
    }
}

// ------------------------------------------------------------------------------------------------ Skywatch

/// <summary>Scout Ahead: reveals the stealthed units of one side and keeps them from hiding again for 10 time units.</summary>
public sealed class ScoutAhead : FlyingSkill
{
    public ScoutAhead(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) =>
        new[] { Side.A, Side.B }.SelectMany(side => Enumerable.Range(0, Formation.RowCount).Select(r => new RowTarget(side, (Row)r))).ToArray();

    public override void Install(BattleState state, int ownerId) => FlyingSkills.InstallScoutBlocker(state);

    public override void Resolve(SkillContext context)
    {
        FlyingSkills.Scout(context.State, context.Row!.Value.Side);
        context.Settle();
    }
}

/// <summary>Spotter: a unit is Marked for 20 time units.</summary>
public sealed class Spotter : FlyingSkill
{
    public Spotter(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);       // Marked is an enemy-only status

    public override void Resolve(SkillContext context)
    {
        HunterSkills.Mark(context.State, context.TargetId!.Value, context.UserId, 20);
        context.Settle();
    }
}

/// <summary>Long Watch (passive): the Marks the unit places last 10 time units longer.</summary>
public sealed class LongWatch : PassiveSkill
{
    public LongWatch(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStatusApplicationModifier((st, a) =>
            a.SourceUnitId == ownerId && a.Id == StatusIds.Marked && a.Duration is { } d ? a with { Duration = d + 10 } : a);
}

/// <summary>Watchtower (passive): while airborne, the unit repeats Scout Ahead on the enemy side every 20 time units.</summary>
public sealed class Watchtower : PassiveSkill
{
    public Watchtower(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        FlyingSkills.InstallScoutBlocker(state);
        state.Timeline.Add(TimelineKind.AutoEvent, FlyingSkills.WatchtowerEvery, ownerId, FlyingSkills.WatchtowerTagPrefix + ownerId);
    }
}

// ------------------------------------------------------------------------------------------------ Assault

/// <summary>Boundless Sky (keystone): while airborne, all the unit's skills have unlimited range.</summary>
public sealed class BoundlessSky : PassiveSkill
{
    public BoundlessSky(ProfessionSkillDef def) : base(def) { }
    public override void Install(BattleState state, int ownerId) => state.AddReachOverride((st, userId) => userId == ownerId && st.GetUnit(userId).IsAirborne);
}

/// <summary>Gust: wind: pushes a unit one row back (bumps apply).</summary>
public sealed class Gust : FlyingSkill
{
    public Gust(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        var row = context.State.Formation.PositionOf(target).Row;
        if (row == Row.Rear) { context.Settle(); return; }
        context.AfterForcedMove(Movement.ForceMove(context.State, target, (Row)((int)row + 1), context.UserSide));
    }
}

/// <summary>Headwind: wind: a unit's next action comes 5 time units later.</summary>
public sealed class Headwind : FlyingSkill
{
    public Headwind(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        context.State.PushTurnBack(context.TargetId!.Value, 5);
        context.Settle();
    }
}

/// <summary>Turbulence: wind: every airborne unit, on both sides, lands.</summary>
public sealed class Turbulence : FlyingSkill
{
    public Turbulence(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        foreach (var unit in context.State.Units.Where(u => u.IsAirborne).ToArray())
            Statuses.Remove(context.State, unit.Id, StatusIds.Airborne, StatusEndCause.Expired);
        context.Settle();                                                          // the auras let a Sky Lord rise again
    }
}

/// <summary>Dust Up: wind: a unit is Blind for 10 time units.</summary>
public sealed class DustUp : FlyingSkill
{
    public DustUp(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.Blind, 10, SourceUnitId: context.UserId));
        context.Settle();
    }
}
