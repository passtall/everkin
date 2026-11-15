using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Commander (skills.md): moves your own units. Physical.</summary>
public static class CommanderSkills
{
    public const int DrillPull = 3;
    public const int VanguardWard = 3;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Advance(D("advance"));
        yield return new SwitchPlaces(D("switch_places"));
        yield return new Rotation(D("rotation"));
        yield return new Drill(D("drill"));
        yield return new Rally(D("rally"));
        yield return new Vanguard(D("vanguard"));
    }

    /// <summary>
    /// What follows a Commander's movement skill: Drill pulls the next turn of every unit it moved forward 3 (the Commander itself included when its own skill
    /// moved it); Vanguard gives a unit that was moved into the Front row a Ward of 3.
    /// </summary>
    internal static void AfterMoving(SkillContext context, IEnumerable<int> moved, IReadOnlyDictionary<int, Row> before)
    {
        var state = context.State;
        foreach (var id in moved.Distinct())
        {
            if (state.GetUnit(id).IsKnockedOut || !state.Formation.Contains(id)) continue;
            if (context.User.KnownSkills.Contains("drill") && state.Timeline.TurnOf(id) is { } turn) state.Timeline.PullForward(turn, DrillPull);
            if (context.User.KnownSkills.Contains("vanguard") && state.Formation.PositionOf(id).Row == Row.Front && before.TryGetValue(id, out var was) && was != Row.Front)
                Statuses.Apply(state, new StatusApplication(id, StatusIds.Ward, Magnitude: context.Scaled(VanguardWard), SourceUnitId: context.UserId));
        }
    }

    internal static Dictionary<int, Row> Rows(BattleState state, IEnumerable<int> ids) =>
        ids.Where(state.Formation.Contains).ToDictionary(id => id, id => state.Formation.PositionOf(id).Row);
}

/// <summary>Advance: own side only. Move every ally in one row one row forward (bumps apply). It cannot pick the Front row.</summary>
public sealed class Advance : ClassSkill
{
    public Advance(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) =>
        ClassHelpers.AllRows().Where(r => r.Side == state.GetUnit(userId).Side && r.Row != Row.Front).ToArray();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        var units = context.State.Formation.Units(row.Side, row.Row).ToArray();
        var before = CommanderSkills.Rows(context.State, context.State.Formation.Units(context.UserSide));
        var moved = context.PullGroupForward(units);
        CommanderSkills.AfterMoving(context, moved, before);
        context.Settle();
    }
}

/// <summary>Switch Places: own side only. Two allies swap places.</summary>
public sealed class SwitchPlaces : ClassSkill
{
    public SwitchPlaces(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override TargetOptions Options => new(TargetSide.OwnSideOnly);
    public override EffectTags Tags => base.Tags;

    public override bool PairFilter(BattleState state, int userId, int firstId, int secondId) =>
        Movement.IsMovable(state.GetUnit(firstId)) && Movement.IsMovable(state.GetUnit(secondId))
        && !Movement.IsImmune(state, firstId, state.GetUnit(userId).Side) && !Movement.IsImmune(state, secondId, state.GetUnit(userId).Side);

    public override void Resolve(SkillContext context)
    {
        var a = context.TargetId!.Value;
        var b = context.SecondTargetId!.Value;
        var before = (A: context.State.Formation.PositionOf(a), B: context.State.Formation.PositionOf(b));
        var rows = new Dictionary<int, Row> { [a] = before.A.Row, [b] = before.B.Row };
        if (Movement.Swap(context.State, a, b, context.UserSide))
        {
            var moves = new List<UnitMoved>
            {
                new(a, before.A, context.State.Formation.PositionOf(a), MoveCause.Forced),
                new(b, before.B, context.State.Formation.PositionOf(b), MoveCause.Forced),
            };
            context.AfterForcedMove(new ForcedMoveResult(ForcedMoveStatus.Moved, moves, Array.Empty<int>()));
            CommanderSkills.AfterMoving(context, new[] { a, b }, rows);
        }
        context.Settle();
    }
}

/// <summary>
/// Rotation: every ally moves one row back and the Rear row moves to the Front. KO bodies, Immobilized units and units immune to being moved stay.
/// Movers keep their left-to-right order and coordinates and are placed among the units that stayed. If a row would hold more than 6 units, nothing moves.
/// </summary>
public sealed class Rotation : ClassSkill
{
    public Rotation(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var f = state.Formation;
        var side = context.UserSide;
        var everyone = f.Units(side).ToArray();
        var coordinate = everyone.ToDictionary(id => id, id => f.CenterOf(f.PositionOf(id)));
        var before = everyone.ToDictionary(id => id, id => f.PositionOf(id));
        var movers = everyone.Where(id => !state.GetUnit(id).IsKnockedOut && Movement.IsMovable(state.GetUnit(id)) && !Movement.IsImmune(state, id, side)).ToList();
        Row Destination(int id) => (Row)(((int)before[id].Row + 1) % Formation.RowCount);

        for (var r = 0; r < Formation.RowCount; r++)
        {
            var stay = everyone.Count(id => (int)before[id].Row == r && !movers.Contains(id));
            if (stay + movers.Count(id => (int)Destination(id) == r) > Formation.RowCapacity) { context.Settle(); return; }
        }
        if (movers.Count == 0) { context.Settle(); return; }

        state.Ground.Accrue();
        foreach (var id in movers) f.Remove(id);
        for (var r = 0; r < Formation.RowCount; r++)
        {
            var row = (Row)r;
            var merged = everyone.Where(id => movers.Contains(id) ? Destination(id) == row : before[id].Row == row)
                .OrderBy(id => coordinate[id]).ThenBy(id => movers.Contains(id) ? 1 : 0).ToList();
            for (var i = 0; i < merged.Count; i++)
                if (movers.Contains(merged[i])) f.Place(merged[i], side, row, i);
        }
        var moves = movers.Select(id => new UnitMoved(id, before[id], f.PositionOf(id), MoveCause.Forced)).ToList();
        foreach (var move in moves) state.Emit(move);
        context.AfterForcedMove(new ForcedMoveResult(ForcedMoveStatus.Moved, moves, Array.Empty<int>()));
        CommanderSkills.AfterMoving(context, movers, before.ToDictionary(kv => kv.Key, kv => kv.Value.Row));
        context.Settle();
    }
}

/// <summary>Drill: an ally the Commander moves has its next turn pulled forward 3 (also the Commander itself when its own skill moves it). The movement skills read this keystone.</summary>
public sealed class Drill : KeystoneSkill
{
    public Drill(SkillDef def) : base(def) { }
}

/// <summary>Rally: when an ally is KO, every other ally's next turn is pulled forward 3.</summary>
public sealed class Rally : KeystoneSkill
{
    public Rally(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            foreach (var id in st.Formation.Units(st.GetUnit(ownerId).Side).ToArray())
                if (id != ctx.Trigger.SubjectUnitId && !st.GetUnit(id).IsKnockedOut && st.Timeline.TurnOf(id) is { } turn)
                    st.Timeline.PullForward(turn, CommanderSkills.DrillPull);
        })
        {
            Condition = (st, t) => !st.GetUnit(ownerId).IsKnockedOut && t.SubjectUnitId != ownerId && st.GetUnit(t.SubjectUnitId).Side == st.GetUnit(ownerId).Side,
        });
}

/// <summary>Vanguard: an ally the Commander moves into the Front row gains a Ward of 3. The movement skills read this keystone.</summary>
public sealed class Vanguard : KeystoneSkill
{
    public Vanguard(SkillDef def) : base(def) { }
}
