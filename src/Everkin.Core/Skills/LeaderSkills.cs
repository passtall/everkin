using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Leader's skills (skills.md Leader): Tactician, Commander and Bard. The Leader's actives are not `magical`. The rulings of
/// 2026-11-10 (Rotation, All In, the Bard's reach, Rhythm) are in skills.md.
/// </summary>
public static class LeaderSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new SwapTurns(D("swap_turns"));
        yield return new PressTheAdvantage(D("press_the_advantage"));
        yield return new InspiringPresence(D("inspiring_presence"));
        yield return new AllIn(D("all_in"));
        yield return new Reposition(D("reposition"));
        yield return new FallBack(D("fall_back"));
        yield return new Advance(D("advance"));
        yield return new SwitchPlaces(D("switch_places"));
        yield return new Rotation(D("rotation"));
        yield return new Drill(D("drill"));
        yield return new SongOfHaste(D("song_of_haste"));
        yield return new WarDrums(D("war_drums"));
        yield return new Rhythm(D("rhythm"));
        yield return new SoloPerformer(D("solo_performer"));
        yield return new GrandChorus(D("grand_chorus"));
    }

    /// <summary>Drill: every unit one of the Leader's movement skills moved has its next turn pulled forward 3 time units.</summary>
    internal static void Drill(SkillContext context, IEnumerable<int> moved)
    {
        if (!context.User.KnownSkills.Contains("drill")) return;
        foreach (var id in moved.Distinct())
            if (!context.State.GetUnit(id).IsKnockedOut && context.State.Timeline.TurnOf(id) is { } turn)
                context.State.Timeline.PullForward(turn, Drill_Pull);
    }

    public const int Drill_Pull = 3;
}

/// <summary>The Bard's reach rules (Song of Haste, War Drums, Rhythm and the two keystones).</summary>
public static class Bard
{
    public enum Reach { OwnRow, Neighbours, EveryAlly }

    /// <summary>Whether the Leader's Bard passive with this default reach covers a unit: Grand Chorus reaches every ally, Solo Performer only the Leader's row.</summary>
    public static bool Covers(BattleState state, int leaderId, int unitId, Reach defaultReach)
    {
        var leader = state.GetUnit(leaderId);
        var unit = state.GetUnit(unitId);
        if (leader.IsKnockedOut || unit.IsKnockedOut || unit.IsRemoved || unit.Side != leader.Side) return false;
        if (!state.Formation.Contains(leaderId) || !state.Formation.Contains(unitId)) return false;
        if (leader.KnownSkills.Contains("grand_chorus")) return true;
        var sameRow = state.Formation.PositionOf(leaderId).Row == state.Formation.PositionOf(unitId).Row;
        if (leader.KnownSkills.Contains("solo_performer")) return sameRow;
        return defaultReach switch
        {
            Reach.OwnRow => sameRow,
            Reach.Neighbours => unitId == leaderId || state.Formation.Adjacent(leaderId).Contains(unitId),
            _ => true,
        };
    }

    /// <summary>Solo Performer doubles the Bard's numbers.</summary>
    public static int Strength(BattleState state, int leaderId) => state.GetUnit(leaderId).KnownSkills.Contains("solo_performer") ? 2 : 1;

    public static IEnumerable<Unit> Leaders(BattleState state, string skillId) =>
        state.Units.Where(u => !u.IsKnockedOut && !u.IsRemoved && u.KnownSkills.Contains(skillId));
}

// ------------------------------------------------------------------------------------------------ Tactician

/// <summary>Swap Turns: two allies exchange the due times of their next turns.</summary>
public sealed class SwapTurns : ProfessionSkill
{
    public SwapTurns(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.OwnSideOnly);

    public override void Resolve(SkillContext context)
    {
        var timeline = context.State.Timeline;
        if (timeline.TurnOf(context.TargetId!.Value) is { } a && timeline.TurnOf(context.SecondTargetId!.Value) is { } b)
            timeline.SwapDue(a, b);
        context.Settle();
    }
}

/// <summary>Press the Advantage: a unit's next skill costs no Momentum.</summary>
public sealed class PressTheAdvantage : ProfessionSkill
{
    public PressTheAdvantage(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override void Resolve(SkillContext context)
    {
        Statuses.Apply(context.State, new StatusApplication(context.TargetId!.Value, StatusIds.FreeMomentum, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Inspiring Presence: when an ally next to the Leader KOs an enemy, Momentum moves 1 more toward your side.</summary>
public sealed class InspiringPresence : PassiveSkill
{
    public InspiringPresence(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) => st.ShiftMomentum(st.GetUnit(ownerId).Side, 1))
        {
            Condition = (st, t) =>
            {
                var owner = st.GetUnit(ownerId);
                return t.SourceUnitId is { } killer && killer != ownerId && st.GetUnit(killer).Side == owner.Side
                    && st.GetUnit(t.SubjectUnitId).Side != owner.Side
                    && st.Formation.Contains(ownerId) && st.Formation.Contains(killer) && st.Formation.Adjacent(ownerId).Contains(killer);
            },
        });
}

/// <summary>All In (keystone): the Leader's Momentum costs are halved (rounded up), but Momentum gained from its hits is 0.</summary>
public sealed class AllIn : PassiveSkill
{
    public AllIn(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddMomentumCostModifier((st, userId, cost) => userId == ownerId ? (cost + 1) / 2 : cost);
        state.AddMomentumShiftModifier((st, userId, shift) => userId == ownerId && shift > 0 ? 0 : shift);
    }
}

// ------------------------------------------------------------------------------------------------ Commander

/// <summary>Reposition: move an ally one row, to any gap, at no cost to its timeline.</summary>
public sealed class Reposition : ProfessionSkill
{
    public Reposition(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.OwnSideOnly);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        PlacementOptionsFor(state, userId, targetId).Count > 0;

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId)
    {
        var unit = state.GetUnit(targetId);
        var result = new List<MoveOption>();
        if (!Movement.IsMovable(unit) || Movement.IsImmune(state, targetId, state.GetUnit(userId).Side)) return result;
        var from = state.Formation.PositionOf(targetId);
        foreach (var delta in new[] { -1, 1 })
        {
            var r = (int)from.Row + delta;
            if (r < 0 || r >= Formation.RowCount) continue;
            var count = state.Formation.Count(from.Side, (Row)r);
            if (count >= Formation.RowCapacity) continue;
            for (var gap = 0; gap <= count; gap++) result.Add(new MoveOption((Row)r, gap));
        }
        return result;
    }

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        Movement.Move(context.State, target, context.Placement!.Value.Row, context.Placement!.Value.Index);
        LeaderSkills.Drill(context, new[] { target });
        context.Settle();
    }
}

/// <summary>Fall Back: move every ally in the Front row one row back (bumps apply).</summary>
public sealed class FallBack : ProfessionSkill
{
    public FallBack(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        var moved = context.PushGroupBack(context.State.Formation.Units(context.UserSide, Row.Front).ToArray());
        LeaderSkills.Drill(context, moved);
    }
}

/// <summary>Advance: move every ally in one row one row forward (bumps apply).</summary>
public sealed class Advance : ProfessionSkill
{
    public Advance(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) =>
        HealerSkills.RowsInReach(state, userId, Reach).Where(r => r.Side == state.GetUnit(userId).Side && r.Row != Row.Front).ToArray();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        var moved = context.PullGroupForward(context.State.Formation.Units(row.Side, row.Row).ToArray());
        LeaderSkills.Drill(context, moved);
    }
}

/// <summary>Switch Places: two allies swap places.</summary>
public sealed class SwitchPlaces : ProfessionSkill
{
    public SwitchPlaces(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.TwoUnits;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(TargetSide.OwnSideOnly);

    public override bool PairFilter(BattleState state, int userId, int firstId, int secondId) =>
        Movement.IsMovable(state.GetUnit(firstId)) && Movement.IsMovable(state.GetUnit(secondId))
        && !Movement.IsImmune(state, firstId, state.GetUnit(userId).Side) && !Movement.IsImmune(state, secondId, state.GetUnit(userId).Side);

    public override void Resolve(SkillContext context)
    {
        var a = context.TargetId!.Value;
        var b = context.SecondTargetId!.Value;
        var before = (A: context.State.Formation.PositionOf(a), B: context.State.Formation.PositionOf(b));
        if (Movement.Swap(context.State, a, b, context.UserSide))
        {
            var moves = new List<UnitMoved>
            {
                new(a, before.A, context.State.Formation.PositionOf(a), MoveCause.Forced),
                new(b, before.B, context.State.Formation.PositionOf(b), MoveCause.Forced),
            };
            context.AfterForcedMove(new ForcedMoveResult(ForcedMoveStatus.Moved, moves, Array.Empty<int>()));
            LeaderSkills.Drill(context, new[] { a, b });
        }
        context.Settle();
    }
}

/// <summary>
/// Rotation: every ally moves one row back and the Rear row moves to the Front. Only living allies that are not Immobilized and not
/// immune to being moved take part; they keep their order and coordinates. If a row would hold more than 6 units, nothing moves.
/// </summary>
public sealed class Rotation : ProfessionSkill
{
    public Rotation(ProfessionSkillDef def) : base(def) { }
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
        LeaderSkills.Drill(context, movers);
        context.Settle();
    }
}

/// <summary>Drill (passive): an ally the Leader moves has its next turn pulled forward 3 time units. Read by the movement skills.</summary>
public sealed class Drill : PassiveSkill
{
    public Drill(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Bard

/// <summary>Song of Haste: allies in the Leader's row have +1 Speed. Kept by an aura that follows the formation.</summary>
public sealed class SongOfHaste : PassiveSkill
{
    public SongOfHaste(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        state.AddAuraSync("song_of_haste", st =>
        {
            var leaders = Bard.Leaders(st, Id).ToList();
            foreach (var unit in st.Units)
            {
                if (unit.IsKnockedOut) continue;
                var bonus = leaders.Where(l => Bard.Covers(st, l.Id, unit.Id, Bard.Reach.OwnRow)).Select(l => Bard.Strength(st, l.Id)).DefaultIfEmpty(0).Max();
                var current = Statuses.Get(unit, StatusIds.SongHaste)?.Magnitude ?? 0;
                if (bonus == current) continue;
                if (bonus == 0) Statuses.Remove(st, unit.Id, StatusIds.SongHaste, StatusEndCause.Expired);
                else Statuses.Apply(st, new StatusApplication(unit.Id, StatusIds.SongHaste, Magnitude: bonus));
            }
        });
        state.RunAuras();
    }
}

/// <summary>War Drums: allies next to the Leader cannot be Stunned or Frozen.</summary>
public sealed class WarDrums : PassiveSkill
{
    public WarDrums(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("war_drums")) return;
        state.AddStatusImmunity((st, unitId, statusId) =>
            statusId is StatusIds.Stunned or StatusIds.Frozen
            && Bard.Leaders(st, Id).Any(l => Bard.Covers(st, l.Id, unitId, Bard.Reach.Neighbours)));
    }
}

/// <summary>Rhythm: when an ally uses the same skill twice in a row, the second use has 1 less Delay.</summary>
public sealed class Rhythm : PassiveSkill
{
    public Rhythm(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("rhythm")) return;
        state.AddRepeatDelayReduction((st, userId, skillId) =>
            Bard.Leaders(st, Id).Where(l => Bard.Covers(st, l.Id, userId, Bard.Reach.EveryAlly)).Select(l => Bard.Strength(st, l.Id)).DefaultIfEmpty(0).Max());
    }
}

/// <summary>Solo Performer (keystone): the Leader's Bard passives are twice as strong, but reach only the Leader's own row. Read by <see cref="Bard"/>.</summary>
public sealed class SoloPerformer : PassiveSkill
{
    public SoloPerformer(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Grand Chorus (keystone): the Bard passives reach every ally, but the Leader can use no active except its type-specific attack. Read by <see cref="Bard"/> and <see cref="Actions"/>.</summary>
public sealed class GrandChorus : PassiveSkill
{
    public GrandChorus(ProfessionSkillDef def) : base(def) { }
}
