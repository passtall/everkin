using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Ai;

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

    /// <summary>The skills that had a legal use at the last choice, and those the search looked at further (game_system section 2.5 "Saved per battle").</summary>
    public IReadOnlySet<string> LastLegalSkills { get; private set; } = new HashSet<string>();
    public IReadOnlySet<string> LastSearchedSkills { get; private set; } = new HashSet<string>();

    /// <summary>A candidate first move with what the search needs to know about it.</summary>
    private sealed record Candidate(AiAction Move, int Score, bool Costed, bool Control, int Effect = 0);

    /// <summary>For tools: called with the searched first moves (best first) with their scores and tie-break ranks, when a choice is made.</summary>
    public Action<BattleState, int, IReadOnlyList<(AiAction Move, int Score, int Effect)>>? OnChosen { get; set; }

    /// <summary>Chooses the action of the unit whose turn it is. The state is not changed.</summary>
    public AiAction Choose(BattleState real, int unitId)
    {
        TotalNodes += Nodes;
        Choices++;
        Nodes = 0;
        _context = new EvalContext(_catalog, real);
        var moves = MoveGenerator.Legal(real, _catalog, unitId, true, _level.MaxPlacements, _level.MaxPairs);
        LastLegalSkills = moves.Where(m => m.Kind == AiActionKind.Skill).Select(m => m.Use!.SkillId).ToHashSet();
        LastSearchedSkills = new HashSet<string>();
        if (moves.Count == 1) return moves[0];
        var maximizing = real.GetUnit(unitId).Side == Side.A;

        var ranked = moves.Select(m => new Candidate(m, 0, false, false)).ToList();
        for (var depth = 1; depth <= _level.Depth; depth++)
        {
            var scored = new List<Candidate>();
            var alpha = int.MinValue;
            var beta = int.MaxValue;
            foreach (var cand in ranked)
            {
                var child = Child(real, unitId, cand.Move);
                var costed = cand.Costed;
                var control = cand.Control;
                var effect = cand.Effect;
                if (depth == 1 && cand.Move.Kind == AiActionKind.Skill)
                {
                    costed = IsCosted(real, unitId, cand.Move);
                    control = IsControl(real, child, unitId);
                }
                if (depth == 1) effect = EffectRank(real, child, unitId, cand.Move);
                // The harness looks one timeline turn further after a costed or control skill, so its payoff can show.
                var further = _level.DeeperAfterCostedOrControl && depth >= 2 && (costed || control) ? 0 : 1;
                // A weaker level needs the true score of every top move, so it searches each with a full window.
                var score = Node(child, depth - further, _level.PickTop > 1 ? int.MinValue : alpha, _level.PickTop > 1 ? int.MaxValue : beta);
                scored.Add(new Candidate(cand.Move, score, costed, control, effect));
                if (_level.PickTop <= 1)
                {
                    if (maximizing) alpha = Math.Max(alpha, score); else beta = Math.Min(beta, score);
                }
            }
            ranked = scored.OrderBy(x => maximizing ? -x.Score : x.Score).ThenByDescending(x => x.Effect).ToList();      // the best move first for the next, deeper round
            if (_level.RootCandidates > 0) ranked = Trim(ranked, Math.Max(_level.RootCandidates, _level.PickTop));
            if (depth == 1 && _level.Depth >= 2)
                LastSearchedSkills = ranked.Where(c => c.Move.Kind == AiActionKind.Skill).Select(c => c.Move.Use!.SkillId).ToHashSet();
        }

        OnChosen?.Invoke(real, unitId, ranked.Select(c => (c.Move, c.Score, c.Effect)).ToList());
        if (_level.PickTop > 1 && _level.PickPercent > 0 && _random.Chance(_level.PickPercent))
            return ranked[_random.NextInt(Math.Min(_level.PickTop, ranked.Count))].Move;
        return ranked[0].Move;
    }

    /// <summary>The best <paramref name="keep"/> candidates, plus (when the level asks) the best costed skill and the best control skill if they are not among them.</summary>
    private List<Candidate> Trim(List<Candidate> ranked, int keep)
    {
        if (ranked.Count <= keep) return ranked;
        var kept = ranked.Take(keep).ToList();
        if (_level.AlwaysSearchCostedAndControl)
        {
            var costed = ranked.FirstOrDefault(c => c.Costed);
            if (costed is not null && !kept.Contains(costed)) kept.Add(costed);
            var control = ranked.FirstOrDefault(c => c.Control);
            if (control is not null && !kept.Contains(control)) kept.Add(control);
        }
        return kept;
    }

    /// <summary>
    /// How much a first move does, for breaking ties (game_system section 2.5 "Ties"): 2 when the enemy lost HP, 1 when a skill changed anything else
    /// (a status, an ally's HP, a summon, the Momentum meter, a turn), 0 for a move, a Skip Turn or a skill that only changed positions.
    /// </summary>
    private static int EffectRank(BattleState before, BattleState after, int unitId, AiAction move)
    {
        if (move.Kind != AiActionKind.Skill) return 0;
        var side = before.GetUnit(unitId).Side;
        int EnemyHp(BattleState s) => s.Units.Where(u => u.Side != side && u.Kind != UnitKind.Construct).Sum(u => u.Hp);
        if (EnemyHp(after) < EnemyHp(before)) return 2;
        if (after.Units.Count != before.Units.Count || after.Momentum.Value != before.Momentum.Value) return 1;
        foreach (var u in before.Units)
        {
            var now = after.GetUnit(u.Id);
            if (now.Hp != u.Hp || now.Ward != u.Ward || now.IsKnockedOut != u.IsKnockedOut || now.IsRemoved != u.IsRemoved || now.StatusInstances.Count != u.StatusInstances.Count) return 1;
            foreach (var (id, status) in now.StatusInstances)
                if (!u.StatusInstances.TryGetValue(id, out var old) || old.EndsAt != status.EndsAt) return 1;
            if (before.Timeline.TurnOf(u.Id)?.Due != after.Timeline.TurnOf(u.Id)?.Due && u.Id != unitId) return 1;
        }
        return 0;
    }

    private bool IsCosted(BattleState before, int unitId, AiAction move) =>
        move.Kind == AiActionKind.Skill && _catalog.TryGet(move.Use!.SkillId, out var skill) && Actions.CostOf(before, unitId, skill) > 0;

    private static readonly string[] ControlStatuses =
        { StatusIds.Stunned, StatusIds.Frozen, StatusIds.Confused, StatusIds.Immobilized, StatusIds.Silenced, StatusIds.Blind };

    /// <summary>
    /// Whether the move was a control skill (game_system section 2.5 "Candidates"), read from what it did: an enemy got Stunned, Frozen, Confused,
    /// Immobilized, Silenced or Blind, an enemy's next turn moved later, or a totem or trap of the user appeared.
    /// </summary>
    private static bool IsControl(BattleState before, BattleState after, int unitId)
    {
        var side = before.GetUnit(unitId).Side;
        foreach (var u in before.Units)
        {
            if (u.Side == side || u.IsKnockedOut || u.IsRemoved) continue;
            var now = after.GetUnit(u.Id);
            if (now.IsKnockedOut || now.IsRemoved) continue;
            foreach (var id in ControlStatuses)
            {
                if (!now.StatusInstances.TryGetValue(id, out var n)) continue;
                if (!u.StatusInstances.TryGetValue(id, out var o) || n.EndsAt > o.EndsAt) return true;
            }
            var was = before.Timeline.TurnOf(u.Id);
            var later = after.Timeline.TurnOf(u.Id);
            if (was is not null && later is not null && later.Due > was.Due) return true;
        }
        for (var i = before.Units.Count; i < after.Units.Count; i++)
        {
            var added = after.Units[i];
            if (added.SummonerId == unitId && added.Kind == UnitKind.Construct
                && (TotemistSkills.IsTotem(added) || added.CreatureId == EngineerSkills.NetTrapId)) return true;
        }
        return false;
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

        // Quick score: apply each candidate and evaluate at once; search only the best few, and always the best costed and best control skill.
        var scored = moves.Select(m =>
        {
            var c = Child(s, unitId, m);
            return (State: c, Quick: Evaluator.Score(c, _level, _context),
                Costed: _level.AlwaysSearchCostedAndControl && IsCosted(s, unitId, m),
                Control: _level.AlwaysSearchCostedAndControl && m.Kind == AiActionKind.Skill && IsControl(s, c, unitId));
        }).OrderBy(x => maximizing ? -x.Quick : x.Quick).ToList();
        var children = scored.Take(_level.Candidates).ToList();
        if (_level.AlwaysSearchCostedAndControl)
        {
            var costed = scored.FirstOrDefault(x => x.Costed);
            if (costed.State is not null && !children.Contains(costed)) children.Add(costed);
            var control = scored.FirstOrDefault(x => x.Control);
            if (control.State is not null && !children.Contains(control)) children.Add(control);
        }

        var best = maximizing ? int.MinValue : int.MaxValue;
        foreach (var (child, _, _, _) in children)
        {
            var score = Node(child, depth - 1, alpha, beta);
            if (maximizing) { best = Math.Max(best, score); alpha = Math.Max(alpha, best); }
            else { best = Math.Min(best, score); beta = Math.Min(beta, best); }
            if (alpha >= beta) break;
        }
        return best;
    }
}
