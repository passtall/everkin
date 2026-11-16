using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Ai;

/// <summary>What the score needs to know besides the position: the skills (which statuses can still do something) and the statuses already there when the search started.</summary>
public sealed class EvalContext
{
    public EvalContext(SkillCatalog catalog, BattleState root)
    {
        Catalog = catalog;
        foreach (var u in root.Units)
            foreach (var s in u.StatusInstances.Values)
                Baseline[(u.Id, s.Id, s.SourceUnitId)] = s.EndsAt;
    }

    public SkillCatalog Catalog { get; }

    /// <summary>Statuses already on a unit at the root of the search (unit, status, source) with the time they end. Renewing one scores nothing: it never counts beyond that end.</summary>
    public Dictionary<(int Unit, string Status, int? Source), int?> Baseline { get; } = new();
}

/// <summary>
/// The position score (game_system section 2.5 "Scoring"): each side's remaining HP as a share of its maximum, every living unit counting
/// as if it had 25% more HP; each point of Momentum worth 0.5% of a team's HP; harmful statuses on enemies and helpful ones on allies count a
/// little, more the longer they last, but only a status that can still do something counts and renewing a status adds nothing; summons and
/// undead count at half value, constructs at nothing. Integers: 10000 is a whole team. Positive favours Side A.
/// </summary>
public static class Evaluator
{
    public const int Win = 1_000_000;

    /// <summary>The skills and keystones that read or use a Mark (Tracker and Champion).</summary>
    private static readonly HashSet<string> MarkUsers = new()
    {
        "target", "hunters_shot", "sniff_the_air", "scent_of_blood", "relentless",
        "challenge", "single_combat", "execution", "hunt_down", "duelist", "glory",
    };

    public static int Score(BattleState state, AiLevel level, EvalContext? context = null)
    {
        if (state.IsOver)
            return state.Outcome switch { BattleOutcome.SideAWins => Win, BattleOutcome.SideBWins => -Win, _ => 0 };
        return SideScore(state, Side.A, level, context) - SideScore(state, Side.B, level, context) + state.Momentum.Value * level.MomentumPoint;
    }

    private static int SideScore(BattleState state, Side side, AiLevel level, EvalContext? context)
    {
        long num = 0, den = 0;
        var statuses = 0L;
        foreach (var u in state.Units)
        {
            if (u.IsRemoved) continue;
            var original = u.Kind == UnitKind.Creature && !u.IsUndead;
            if (original && u.OriginalSide == side) den += 5L * u.MaxHp * 100;
            if (u.Side != side || u.IsKnockedOut) continue;
            var weight = original ? 100 : u.Kind == UnitKind.Construct ? level.ConstructPercent : level.SummonPercent;
            num += weight * (4L * u.Hp + u.MaxHp);
            if (u.Kind == UnitKind.Construct) continue;                       // a construct's statuses count nothing either
            foreach (var s in u.StatusInstances.Values)
                if (StatusRules.PolarityOf(s.Id) == Polarity.Helpful) statuses += Remaining(state, u, s, context);
        }
        // Harmful statuses on the other side's units count for this side.
        foreach (var u in state.Units)
            if (!u.IsRemoved && !u.IsKnockedOut && u.Side != side && u.Kind != UnitKind.Construct)
                foreach (var s in u.StatusInstances.Values)
                    if (StatusRules.PolarityOf(s.Id) == Polarity.Harmful) statuses += Remaining(state, u, s, context);
        var share = den == 0 ? 0 : (int)(num * 10000 / den);
        return share + (int)(statuses * level.StatusValue / 20);
    }

    /// <summary>
    /// Time units a status counts for, up to 20 (a status without a clock counts as 20): none when nothing can use it any more, and never
    /// beyond the end it had when the search started, so a renewal scores nothing.
    /// </summary>
    private static int Remaining(BattleState state, Unit carrier, StatusInstance s, EvalContext? context)
    {
        if (context is not null && !CanDoSomething(state, carrier, s, context.Catalog)) return 0;
        var left = Math.Min(20, s.TimeLeft(state.Timeline.Now) ?? 20);
        if (context is not null && context.Baseline.TryGetValue((carrier.Id, s.Id, s.SourceUnitId), out var endsAt) && endsAt is { } end)
            left = Math.Min(left, Math.Max(0, end - state.Timeline.Now));
        return left;
    }

    /// <summary>Whether a status still has something to act on (game_system section 2.5 "Scoring").</summary>
    private static bool CanDoSomething(BattleState state, Unit carrier, StatusInstance s, SkillCatalog catalog)
    {
        switch (s.Id)
        {
            case StatusIds.Marked:                                              // only while the Mark's owner side has a skill that uses it
                return state.Units.Any(u => u.Side != carrier.Side && !u.IsRemoved && !u.IsKnockedOut && u.KnownSkills.Any(MarkUsers.Contains));
            case StatusIds.Phased:
            case StatusIds.Piercing:                                            // the next attack: the unit needs an attack it can use
                return carrier.KnownSkills.Any(id => catalog.TryGet(id, out var k) && !k.IsPassive && k.IsAttack && Usable(carrier, k));
            case StatusIds.Echo:                                                // the next skill
                return carrier.KnownSkills.Any(id => catalog.TryGet(id, out var k) && !k.IsPassive && Usable(carrier, k));
            case StatusIds.FreeMomentum:                                        // the next skill with a Momentum cost
                return carrier.KnownSkills.Any(id => catalog.TryGet(id, out var k) && !k.IsPassive && k.MomentumCost > 0 && Usable(carrier, k));
            default:                                                            // Ward, songs and the rest sit on a unit that is still standing
                return true;
        }
    }

    private static bool Usable(Unit unit, Skill skill) => !skill.Magical || Statuses.CanUseMagical(unit);
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
    private EvalContext? _context;

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
        _context = new EvalContext(_catalog, real);
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
        if (s.IsOver || depth == 0) return Evaluator.Score(s, _level, _context);
        var next = BattleDriver.NextTurn(s, _catalog);
        s.DrainEvents();
        if (s.IsOver || next is not { } unitId) return Evaluator.Score(s, _level, _context);

        var maximizing = s.GetUnit(unitId).Side == Side.A;
        var moves = MoveGenerator.Legal(s, _catalog, unitId, true, _level.MaxPlacements, _level.MaxPairs);
        if (!_level.DeeperMoveAndSkip)
        {
            var actions = moves.Where(m => m.Kind == AiActionKind.Skill).ToList();
            if (actions.Count > 0) moves = actions;
        }

        // Quick score: apply each candidate and evaluate at once; search only the best few.
        var children = moves.Select(m => { var c = Child(s, unitId, m); return (State: c, Quick: Evaluator.Score(c, _level, _context)); })
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
