using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Ai;

/// <summary>
/// The position score (game_system section 2.5 "Scoring"): each side's remaining HP as a share of its maximum, every living unit counting
/// as if it had 25% more HP; each point of Momentum worth 2% of a team's HP; harmful statuses on enemies and helpful ones on allies count a
/// little, more the longer they last; summons and constructs count at half value. Integers: 10000 is a whole team. Positive favours Side A.
/// </summary>
public static class Evaluator
{
    public const int Win = 1_000_000;

    public static int Score(BattleState state, AiLevel level)
    {
        if (state.IsOver)
            return state.Outcome switch { BattleOutcome.SideAWins => Win, BattleOutcome.SideBWins => -Win, _ => 0 };
        return SideScore(state, Side.A, level) - SideScore(state, Side.B, level) + state.Momentum.Value * level.MomentumPoint;
    }

    private static int SideScore(BattleState state, Side side, AiLevel level)
    {
        long num = 0, den = 0;
        var statuses = 0L;
        foreach (var u in state.Units)
        {
            if (u.IsRemoved) continue;
            var original = u.Kind == UnitKind.Creature && !u.IsUndead;
            if (original && u.OriginalSide == side) den += 5L * u.MaxHp * 100;
            if (u.Side != side || u.IsKnockedOut) continue;
            var weight = original ? 100 : level.SummonPercent;
            num += weight * (4L * u.Hp + u.MaxHp);
            foreach (var s in u.StatusInstances.Values)
            {
                var polarity = StatusRules.PolarityOf(s.Id);
                if (polarity == Polarity.Helpful) statuses += Remaining(s, state) ;
            }
        }
        // Harmful statuses on the other side's units count for this side.
        foreach (var u in state.Units)
            if (!u.IsRemoved && !u.IsKnockedOut && u.Side != side)
                foreach (var s in u.StatusInstances.Values)
                    if (StatusRules.PolarityOf(s.Id) == Polarity.Harmful) statuses += Remaining(s, state);
        var share = den == 0 ? 0 : (int)(num * 10000 / den);
        return share + (int)(statuses * level.StatusValue / 20);
    }

    /// <summary>Time units a status has left, up to 20 (a status without a clock counts as 20).</summary>
    private static int Remaining(StatusInstance s, BattleState state) => Math.Min(20, s.TimeLeft(state.Timeline.Now) ?? 20);
}

/// <summary>
/// The AI (game_system section 2.5, production section 2.3): alpha-beta minimax with iterative deepening over a fixed number of upcoming
/// timeline turns counted across both sides. It works on clones with the generator in <see cref="BattleRandom.RollMode.Average"/> mode, so
/// damage rolls and chances are averages, and it never draws from the battle's generator: its weaker picks use its own.
/// </summary>
public sealed class AiSearch
{
    private readonly SkillCatalog _catalog;
    private readonly AiLevel _level;
    private readonly BattleRandom _random;

    public AiSearch(SkillCatalog catalog, AiLevel level, BattleRandom random)
    {
        _catalog = catalog;
        _level = level;
        _random = random;
    }

    /// <summary>The nodes the last <see cref="Choose"/> looked at (for speed measurements).</summary>
    public long Nodes { get; private set; }

    /// <summary>Nodes over all choices made with this instance.</summary>
    public long TotalNodes { get; private set; }

    /// <summary>How many choices were made with this instance.</summary>
    public int Choices { get; private set; }

    /// <summary>Chooses the action of the unit whose turn it is. The state is not changed.</summary>
    public AiAction Choose(BattleState real, int unitId)
    {
        TotalNodes += Nodes;
        Choices++;
        Nodes = 0;
        var moves = MoveGenerator.Legal(real, _catalog, unitId, true, _level.MaxPlacements, _level.MaxPairs);
        if (moves.Count == 1) return moves[0];
        var maximizing = real.GetUnit(unitId).Side == Side.A;

        List<(AiAction Move, int Score)> ranked = moves.Select(m => (m, 0)).ToList();
        for (var depth = 1; depth <= _level.Depth; depth++)
        {
            var scored = new List<(AiAction Move, int Score)>();
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            foreach (var (move, _) in ranked)
            {
                var child = Child(real, unitId, move);
                // A weaker level needs the true score of every top move, so it searches each with a full window.
                var score = Node(child, depth - 1, _level.PickTop > 1 ? int.MinValue : alpha, _level.PickTop > 1 ? int.MaxValue : beta);
                scored.Add((move, score));
                if (_level.PickTop <= 1)
                {
                    if (maximizing) alpha = Math.Max(alpha, score); else beta = Math.Min(beta, score);
                }
            }
            ranked = scored.OrderBy(x => maximizing ? -x.Score : x.Score).ToList();      // the best move first for the next, deeper round
            if (_level.RootCandidates > 0 && ranked.Count > Math.Max(_level.RootCandidates, _level.PickTop)) ranked = ranked.Take(Math.Max(_level.RootCandidates, _level.PickTop)).ToList();
        }

        if (_level.PickTop > 1 && _level.PickPercent > 0 && _random.Chance(_level.PickPercent))
            return ranked[_random.NextInt(Math.Min(_level.PickTop, ranked.Count))].Move;
        return ranked[0].Move;
    }

    private BattleState Child(BattleState parent, int unitId, AiAction move)
    {
        var child = parent.Clone();
        child.Random.Mode = BattleRandom.RollMode.Average;
        move.Apply(child, _catalog, unitId, validated: true);
        child.DrainEvents();
        return child;
    }

    private int Node(BattleState s, int depth, int alpha, int beta)
    {
        Nodes++;
        if (s.IsOver || depth == 0) return Evaluator.Score(s, _level);
        var next = BattleDriver.NextTurn(s, _catalog);
        s.DrainEvents();
        if (s.IsOver || next is not { } unitId) return Evaluator.Score(s, _level);

        var maximizing = s.GetUnit(unitId).Side == Side.A;
        var moves = MoveGenerator.Legal(s, _catalog, unitId, true, _level.MaxPlacements, _level.MaxPairs);
        if (!_level.DeeperMoveAndSkip)
        {
            var actions = moves.Where(m => m.Kind == AiActionKind.Skill).ToList();
            if (actions.Count > 0) moves = actions;
        }

        // Quick score: apply each candidate and evaluate at once; search only the best few.
        var children = moves.Select(m => { var c = Child(s, unitId, m); return (State: c, Quick: Evaluator.Score(c, _level)); })
            .OrderBy(x => maximizing ? -x.Quick : x.Quick).Take(_level.Candidates).ToList();

        var best = maximizing ? int.MinValue : int.MaxValue;
        foreach (var (child, _) in children)
        {
            var score = Node(child, depth - 1, alpha, beta);
            if (maximizing) { best = Math.Max(best, score); alpha = Math.Max(alpha, best); }
            else { best = Math.Min(best, score); beta = Math.Min(beta, best); }
            if (alpha >= beta) break;
        }
        return best;
    }
}
