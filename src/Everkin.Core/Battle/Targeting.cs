namespace Everkin.Core.Battle;

/// <summary>
/// How far a skill reaches (skills.md Delivery · Range, game_system.md section 7.4): a maximum distance such as D3,
/// or unlimited ("any visible unit", Phase Weapon).
/// </summary>
public readonly record struct Reach(int? MaxDistance)
{
    public static Reach Distance(int maxDistance) => maxDistance >= 1
        ? new Reach(maxDistance)
        : throw new ArgumentOutOfRangeException(nameof(maxDistance), "The minimum distance is 1.");

    public static Reach Unlimited => new(null);

    public bool Allows(int distance) => MaxDistance is null || distance <= MaxDistance;
}

/// <summary>Which side a skill may target. Unless a skill says otherwise it may target any legal unit, ally or enemy (section 8.1).</summary>
public enum TargetSide
{
    Any,
    OwnSideOnly,
    EnemySideOnly,
}

/// <summary>What a skill may target besides the defaults.</summary>
/// <param name="Side">Which side is allowed. "Own side only" skills use <see cref="TargetSide.OwnSideOnly"/>.</param>
/// <param name="IncludeKnockedOut">Only resurrection skills can target KO bodies (section 8.4).</param>
/// <param name="Delivery">The skill's delivery, when known: melee attacks cannot reach an airborne unit (section 10.6).</param>
public readonly record struct TargetOptions(TargetSide Side = TargetSide.Any, bool IncludeKnockedOut = false, Delivery? Delivery = null);

/// <summary>
/// Range and target legality (game_system.md sections 7.4, 8.1, 8.5). Everything here only reads the state; the only
/// random draw is the tie roll in <see cref="PickNearest"/>.
/// </summary>
public static class Targeting
{
    /// <summary>
    /// The effective row of a unit for range: Front is 1, Middle 2, Rear 3, counted after skipping rows with no one in them
    /// (section 7.4). On the user's own side a row counts when it holds a living unit, stealthed or not. On the other side
    /// stealthed units and KO bodies do not make a row count. The unit's own row always counts. A unit's actual row never
    /// changes; only this calculation skips rows.
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
    /// The distance from a unit to a unit on the other side: attacker's effective row + target's effective row − 1
    /// (section 7.4 table). Horizontal position is ignored.
    /// </summary>
    public static int OpposingDistance(BattleState state, int attackerId, int targetId)
    {
        var attackerSide = state.Formation.PositionOf(attackerId).Side;
        var targetSide = state.Formation.PositionOf(targetId).Side;
        if (attackerSide == targetSide) throw new ArgumentException("Both units are on the same side.");
        return EffectiveRow(state, attackerId, attackerSide) + EffectiveRow(state, targetId, attackerSide) - 1;
    }

    /// <summary>
    /// Whether a target is within reach when the skill is cast (section 7.4). Other side: the opposing distance is at most
    /// the range. Own side: D1 reaches the user itself and its adjacent units, D2 or more any unit on the user's side.
    /// This checks reach only, not whether the target may be chosen at all; see <see cref="IsLegalTarget"/>.
    /// </summary>
    public static bool InReach(BattleState state, int userId, int targetId, Reach reach)
    {
        var userSide = state.Formation.PositionOf(userId).Side;
        var targetSide = state.Formation.PositionOf(targetId).Side;
        if (userSide != targetSide)
            return reach.Allows(OpposingDistance(state, userId, targetId));
        if (reach.MaxDistance is null or >= 2) return true;
        return userId == targetId || state.Formation.Adjacent(userId).Contains(targetId);
    }

    /// <summary>
    /// Whether the player or AI may choose this unit with a skill of this reach (sections 8.1, 10.1, 10.2): it is within
    /// reach, on an allowed side, not a KO body (unless the skill resurrects), and a unit of the other side must not be
    /// stealthed. Empty spots are never targets.
    /// </summary>
    public static bool IsLegalTarget(BattleState state, int userId, int targetId, Reach reach, TargetOptions options = default)
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
        if (options.Delivery == Everkin.Core.Battle.Delivery.Melee && target.IsAirborne && !state.GetUnit(userId).IsAirborne) return false;
        return InReach(state, userId, targetId, reach) && state.TargetRestrictions.All(r => r(state, userId, targetId));
    }

    /// <summary>
    /// Every legal target, the user's side first, each row Front to Rear and left to right. The user itself is a
    /// legal target of a skill that allows the own side (a heal on itself).
    /// </summary>
    public static IReadOnlyList<int> LegalTargets(BattleState state, int userId, Reach reach, TargetOptions options = default)
    {
        var userSide = state.Formation.PositionOf(userId).Side;
        var result = new List<int>();
        foreach (var side in new[] { userSide, userSide.Opposite() })
            foreach (var id in state.Formation.Units(side))
                if (IsLegalTarget(state, userId, id, reach, options))
                    result.Add(id);
        return result;
    }

    /// <summary>
    /// The legal targets on the other side that are equally nearest, by opposing distance (section 8.5). Stealthed units
    /// and KO bodies are not legal, so they never count. Empty when there is no legal enemy.
    /// </summary>
    public static IReadOnlyList<int> NearestEnemies(BattleState state, int userId, Reach reach, TargetOptions options = default)
    {
        var enemies = LegalTargets(state, userId, reach, options with { Side = TargetSide.EnemySideOnly });
        if (enemies.Count == 0) return enemies;
        var distances = enemies.Select(id => (id, d: OpposingDistance(state, userId, id))).ToList();
        var nearest = distances.Min(x => x.d);
        return distances.Where(x => x.d == nearest).Select(x => x.id).ToArray();
    }

    /// <summary>
    /// A "nearest" skill never asks for a choice: if several targets are equally close, the battle's seeded roll picks one,
    /// each with equal chance (section 8.5). A single candidate costs no roll. Returns null when there is no candidate.
    /// </summary>
    public static int? PickNearest(BattleState state, int userId, Reach reach, TargetOptions options = default)
    {
        var nearest = NearestEnemies(state, userId, reach, options);
        return nearest.Count switch
        {
            0 => null,
            1 => nearest[0],
            _ => nearest[state.Random.NextInt(nearest.Count)],
        };
    }
}
