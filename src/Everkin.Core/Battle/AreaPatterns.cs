namespace Everkin.Core.Battle;

/// <summary>A unit behind a target and how much it overlaps it.</summary>
/// <param name="UnitId">The unit behind.</param>
/// <param name="OverlapPercent">100 or 50 (units with no overlap are not behind the target).</param>
/// <param name="RowsBehind">1 for the row directly behind the target's row, 2 for the one after that.</param>
public readonly record struct BehindUnit(int UnitId, int OverlapPercent, int RowsBehind);

/// <summary>
/// Area patterns (game_system.md section 8.4): fixed shapes based on rows and adjacency. Area effects reach stealthed units
/// physically inside the area and never hit KO bodies (unless the skill says so, which these functions do not model).
/// Whether an effect also hits allies or the user is the skill's business.
/// </summary>
public static class AreaPatterns
{
    /// <summary>`circular`: the target and the units adjacent to it (left and right neighbors, and the rows directly in front and behind at 50% overlap or more). The target comes first.</summary>
    public static IReadOnlyList<int> Circular(BattleState state, int targetId)
    {
        var result = new List<int> { targetId };
        foreach (var id in state.Formation.Adjacent(targetId))
            if (!state.GetUnit(id).IsKnockedOut)
                result.Add(id);
        return result;
    }

    /// <summary>`row`: every unit in the chosen row, left to right.</summary>
    public static IReadOnlyList<int> Row(BattleState state, Side side, Row row) =>
        state.Formation.Units(side, row).Where(id => !state.GetUnit(id).IsKnockedOut).ToArray();

    /// <summary>`all`: every unit of one side, Front row first.</summary>
    public static IReadOnlyList<int> AllOf(BattleState state, Side side) =>
        state.Formation.Units(side).Where(id => !state.GetUnit(id).IsKnockedOut).ToArray();

    /// <summary>
    /// The units behind a target on its own side, row by row, nearest row first, with their overlap (100% or 50%). It uses
    /// the nominal rows. Stealthed units are included; KO bodies are not. A skill that hits "the unit behind" decides how
    /// many rows count; see <see cref="HitBehind"/> for the 100% / 50% rule.
    /// </summary>
    public static IReadOnlyList<BehindUnit> UnitsBehind(BattleState state, int targetId)
    {
        var position = state.Formation.PositionOf(targetId);
        var result = new List<BehindUnit>();
        for (var r = (int)position.Row + 1; r < Formation.RowCount; r++)
            foreach (var id in state.Formation.Units(position.Side, (Row)r))
            {
                if (state.GetUnit(id).IsKnockedOut) continue;
                var overlap = state.Formation.UnitOverlapPercent(id, targetId);
                if (overlap >= 50) result.Add(new BehindUnit(id, overlap, r - (int)position.Row));
            }
        return result;
    }

    /// <summary>
    /// The behind rule: units with 100% overlap are hit automatically, units with 50% overlap with a 50% chance each,
    /// rolled in board order from the battle's generator. Returns the units that are hit.
    /// </summary>
    public static IReadOnlyList<int> HitBehind(BattleState state, IReadOnlyList<BehindUnit> candidates)
    {
        var hit = new List<int>();
        foreach (var c in candidates)
            if (c.OverlapPercent >= 100 || state.Random.Chance(50))
                hit.Add(c.UnitId);
        return hit;
    }
}
