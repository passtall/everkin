using System.Text.Json;
using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Ai;

public enum AiActionKind { Skill, Move, SkipTurn }

/// <summary>One thing a unit can do on its turn: a skill use, a Move to a gap, or Skip Turn.</summary>
public sealed record AiAction(AiActionKind Kind, SkillUse? Use = null, Row Row = Row.Front, int Gap = 0)
{
    public static AiAction Skip { get; } = new(AiActionKind.SkipTurn);

    /// <summary>Plays the action on a state (the real battle, or a clone in the search). Returns the events it caused.</summary>
    public IReadOnlyList<BattleEvent> Apply(BattleState state, SkillCatalog catalog, int unitId, bool validated = false) => Kind switch
    {
        AiActionKind.Skill => Actions.Use(state, catalog, Use!, validated),
        AiActionKind.Move => Actions.Move(state, unitId, Row, Gap),
        _ => Actions.SkipTurn(state, unitId),
    };

    public override string ToString() => Kind switch
    {
        AiActionKind.Skill => $"{Use!.SkillId}({Use.TargetUnitId}{(Use.SecondTargetUnitId is { } s ? "," + s : "")})",
        AiActionKind.Move => $"move({Row},{Gap})",
        _ => "skip",
    };
}

/// <summary>Every legal action of a unit (game_system section 6.3): each known active skill with each legal target, Move and Skip Turn.</summary>
public static class MoveGenerator
{
    public static IReadOnlyList<AiAction> Legal(BattleState state, SkillCatalog catalog, int unitId, bool includeMoveAndSkip = true, int maxPlaces = 0, int maxPairs = 0)
    {
        var unit = state.GetUnit(unitId);
        var result = new List<AiAction>();
        foreach (var skillId in unit.KnownSkills.Distinct())
        {
            if (!catalog.TryGet(skillId, out var skill) || skill.IsPassive) continue;
            foreach (var use in Uses(state, unitId, skill, maxPlaces, maxPairs))
                if (Actions.WhyNot(state, catalog, use) is null) result.Add(new AiAction(AiActionKind.Skill, use));
        }
        if (includeMoveAndSkip)
        {
            foreach (var m in Movement.LegalMoves(state, unitId)) result.Add(new AiAction(AiActionKind.Move, null, m.Row, m.Index));
            result.Add(AiAction.Skip);
        }
        return result;
    }

    /// <summary>At most <paramref name="max"/> items spread evenly over the list, ends included; all of them when max is 0.</summary>
    private static IReadOnlyList<T> Spread<T>(IReadOnlyList<T> items, int max)
    {
        if (max <= 0 || items.Count <= max) return items;
        return Enumerable.Range(0, max).Select(i => items[max == 1 ? 0 : i * (items.Count - 1) / (max - 1)]).ToArray();
    }

    private static IEnumerable<SkillUse> Uses(BattleState state, int userId, Skill skill, int maxPlaces, int maxPairs)
    {
        var options = skill.Options with { Delivery = skill.Delivery };
        IReadOnlyList<int> Targets() => Targeting.LegalTargets(state, userId, Actions.ReachOf(state, userId, skill), options)
            .Where(t => skill.TargetFilter(state, userId, t)).ToArray();

        switch (skill.Target)
        {
            case TargetKind.None:
                yield return new SkillUse(userId, skill.Id);
                break;
            case TargetKind.Unit:
                foreach (var t in Targets())
                {
                    yield return new SkillUse(userId, skill.Id, t);
                    if (skill.Id == "lift_a_skill")                                   // the copied skill may need a target of its own
                        foreach (var second in Spread(state.Units.Where(u => u.Id != t && !u.IsKnockedOut).ToArray(), maxPairs))
                            yield return new SkillUse(userId, skill.Id, t, SecondTargetUnitId: second.Id);
                }
                break;
            case TargetKind.Row:
                foreach (var row in skill.LegalRows(state, userId)) yield return new SkillUse(userId, skill.Id, Row: row);
                break;
            case TargetKind.Placement:
                if (skill.PlacesRandomly(state, userId)) yield return new SkillUse(userId, skill.Id);
                else foreach (var p in Spread(skill.PlacementOptions(state, userId), maxPlaces)) yield return new SkillUse(userId, skill.Id, Placement: p);
                break;
            case TargetKind.UnitAndPlacement:
                foreach (var t in Targets())
                {
                    if (skill.PlacesRandomly(state, userId)) { yield return new SkillUse(userId, skill.Id, t); continue; }
                    foreach (var p in Spread(skill.PlacementOptionsFor(state, userId, t), maxPlaces)) yield return new SkillUse(userId, skill.Id, t, Placement: p);
                }
                break;
            case TargetKind.TwoUnits:
                var all = Targets();
                var pairs = all.SelectMany(a => all.Where(b => b != a).Select(b => (a, b))).ToArray();
                foreach (var (a, b) in Spread(pairs, maxPairs)) yield return new SkillUse(userId, skill.Id, a, SecondTargetUnitId: b);
                break;
        }
    }
}

/// <summary>One difficulty level of the AI (game_system section 2.5, production section 2.3). The numbers live in content/ai_settings.json.</summary>
public sealed class AiLevel
{
    /// <summary>Search depth in timeline turns counted across both sides.</summary>
    public int Depth { get; init; } = 4;

    /// <summary>Below the first move, only this many best candidates (by a quick score) are searched.</summary>
    public int Candidates { get; init; } = 8;

    /// <summary>
    /// 0 searches every legal first move at every depth. Otherwise the deeper rounds of the iterative deepening only refine the best
    /// this-many first moves of the previous round (round one still scores all of them). A speed setting for the test harness.
    /// </summary>
    public int RootCandidates { get; init; }

    /// <summary>Speed settings for the test harness: at most this many places per placement skill and pairs per two-target skill (0 = all).</summary>
    public int MaxPlacements { get; init; }
    public int MaxPairs { get; init; }

    /// <summary>With <see cref="PickPercent"/> percent chance the AI picks at random among its best <see cref="PickTop"/> actions instead of the best.</summary>
    public int PickTop { get; init; } = 1;
    public int PickPercent { get; init; }

    /// <summary>Whether Move and Skip Turn are searched below the first move (when other actions exist).</summary>
    public bool DeeperMoveAndSkip { get; init; }

    /// <summary>Evaluation weights, in hundredths of a percent of a team's HP share (10000 = the whole team).</summary>
    public int MomentumPoint { get; init; } = 200;

    /// <summary>Per status that is helpful on an ally or harmful on an enemy at full 20 time units left; fewer time units count proportionally.</summary>
    public int StatusValue { get; init; } = 60;

    /// <summary>Weight of a summon, construct or undead compared with an original, in percent.</summary>
    public int SummonPercent { get; init; } = 50;
}

/// <summary>The AI settings of all levels.</summary>
public sealed class AiSettings
{
    public IReadOnlyDictionary<string, AiLevel> Levels { get; init; } = new Dictionary<string, AiLevel>();

    public AiLevel Level(string name) => Levels.TryGetValue(name, out var l) ? l : throw new ArgumentException($"No AI level '{name}'.");

    public static AiSettings FromJson(string json)
    {
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<AiSettings>(json, options) ?? throw new InvalidOperationException("Empty AI settings.");
    }

    public static AiSettings Load(string path) => FromJson(File.ReadAllText(path));
}
