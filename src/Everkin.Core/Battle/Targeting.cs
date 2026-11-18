namespace Everkin.Core.Battle;

/// <summary>Which side a skill may target. Unless a skill says otherwise it may target any legal unit, ally or enemy (game_system.md section 8.1).</summary>
public enum TargetSide
{
    Any,
    OwnSideOnly,
    EnemySideOnly,
}

/// <summary>What a skill may target besides the defaults.</summary>
/// <param name="Side">Which side is allowed. "Own side only" skills use <see cref="TargetSide.OwnSideOnly"/>.</param>
/// <param name="IncludeKnockedOut">Only resurrection skills can target KO bodies (section 8.4).</param>
/// <param name="Delivery">The skill's delivery, when known: a melee skill needs the user in its Front row and cannot reach an airborne unit (sections 8.1, 10.6).</param>
/// <param name="AnyRow">The melee Front-row rule does not apply (Dive, Puppet Soul's forced hit).</param>
public readonly record struct TargetOptions(TargetSide Side = TargetSide.Any, bool IncludeKnockedOut = false, Delivery? Delivery = null, bool AnyRow = false);

/// <summary>
/// Target legality (game_system.md sections 8.1, 8.5). There is no range: any legal unit on either side can be targeted. Everything here only
/// reads the state; the only random draw is the tie roll in <see cref="PickNearest"/>.
/// </summary>
public static class Targeting
{
    /// <summary>A unit can use a `melee` skill or attack only while it stands in its own Front row (section 8.1).</summary>
    public static bool InFrontRow(BattleState state, int unitId) =>
        state.Formation.Contains(unitId) && state.Formation.PositionOf(unitId).Row == Row.Front;

    /// <summary>
    /// Melee reach (section 8.1): every enemy in the enemy's first row that holds a visible living unit (Front, else Middle, else Rear; rows with no living unit,
    /// only KO bodies or only stealthed units are passed over), KO bodies excluded, stealthed units of that row included. When no row holds a visible unit
    /// (every living enemy is stealthed) it is empty, as it is when the user is not in its own Front row.
    /// </summary>
    public static IReadOnlyList<int> EnemiesInMeleeReach(BattleState state, int userId)
    {
        if (!InFrontRow(state, userId)) return Array.Empty<int>();
        var enemy = state.GetUnit(userId).Side.Opposite();
        IReadOnlyList<int> Living(Row row) => state.Formation.Units(enemy, row).Where(id => !state.GetUnit(id).IsKnockedOut).ToArray();
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            if (Living(row).Any(id => !state.GetUnit(id).IsStealthed)) return Living(row);
        return Array.Empty<int>();
    }

    /// <summary>
    /// The effective row of a unit for "nearest": Front is 1, Middle 2, Rear 3, counted after skipping rows with no one in them. On the user's own side a row
    /// counts when it holds a living unit; on the other side stealthed units and KO bodies do not make a row count (sections 8.5, 10.1).
    /// </summary>
    public static int EffectiveRow(BattleState state, int unitId, Side viewer)
    {
        var position = state.Formation.PositionOf(unitId);
        var own = position.Side == viewer;
        var effective = 1;
        for (var r = 0; r < (int)position.Row; r++)
        {
            var counts = state.Formation.Units(position.Side, (Row)r).Any(id =>
            {
                var u = state.GetUnit(id);
                return !u.IsKnockedOut && (own || !u.IsStealthed);
            });
            if (counts) effective++;
        }
        return effective;
    }

    /// <summary>
    /// Whether the player or AI may choose this unit (sections 8.1, 10.1, 10.2): it is on an allowed side, not a KO body (unless the skill resurrects),
    /// not a stealthed unit of the other side, reachable by melee if the skill is melee, and not excluded by a restriction. Empty spots are never targets.
    /// </summary>
    public static bool IsLegalTarget(BattleState state, int userId, int targetId, TargetOptions options = default)
    {
        if (!state.Formation.Contains(userId) || !state.Formation.Contains(targetId)) return false;
        var userSide = state.Formation.PositionOf(userId).Side;
        var target = state.GetUnit(targetId);
        var targetSide = state.Formation.PositionOf(targetId).Side;
        var own = userSide == targetSide;

        if (options.Side == TargetSide.OwnSideOnly && !own) return false;
        if (options.Side == TargetSide.EnemySideOnly && own) return false;
        if (target.IsKnockedOut && !options.IncludeKnockedOut) return false;
        if (!own && target.IsStealthed) return false;
        if (options.Delivery == Everkin.Core.Battle.Delivery.Melee)
        {
            if (!options.AnyRow && !InFrontRow(state, userId)) return false;
            if (target.IsAirborne && !state.GetUnit(userId).IsAirborne) return false;
        }
        return state.TargetRestrictions.All(r => r(state, userId, targetId));
    }

    /// <summary>
    /// Every legal target, the user's side first, each row Front to Rear and left to right. The user itself is a
    /// legal target of a skill that allows the own side (a heal on itself).
    /// </summary>
    public static IReadOnlyList<int> LegalTargets(BattleState state, int userId, TargetOptions options = default)
    {
        var userSide = state.Formation.PositionOf(userId).Side;
        var result = new List<int>();
        foreach (var side in new[] { userSide, userSide.Opposite() })
            foreach (var id in state.Formation.Units(side))
                if (IsLegalTarget(state, userId, id, options))
                    result.Add(id);
        return result;
    }

    /// <summary>
    /// The legal enemies that are equally nearest (section 8.5): in the fewest rows from their side's Front, empty rows ignored. Stealthed units and KO
    /// bodies are not legal, so they never count. Empty when there is no legal enemy.
    /// </summary>
    public static IReadOnlyList<int> NearestEnemies(BattleState state, int userId, TargetOptions options = default)
    {
        var enemies = LegalTargets(state, userId, options with { Side = TargetSide.EnemySideOnly });
        if (enemies.Count == 0) return enemies;
        var userSide = state.Formation.PositionOf(userId).Side;
        var rows = enemies.Select(id => (id, d: EffectiveRow(state, id, userSide))).ToList();
        var nearest = rows.Min(x => x.d);
        return rows.Where(x => x.d == nearest).Select(x => x.id).ToArray();
    }

    /// <summary>
    /// A "nearest" skill never asks for a choice: if several targets are equally close, the battle's seeded roll picks one,
    /// each with equal chance (section 8.5). A single candidate costs no roll. Returns null when there is no candidate.
    /// </summary>
    public static int? PickNearest(BattleState state, int userId, TargetOptions options = default)
    {
        var nearest = NearestEnemies(state, userId, options);
        return nearest.Count switch
        {
            0 => null,
            1 => nearest[0],
            _ => nearest[state.Random.NextInt(nearest.Count)],
        };
    }
}
