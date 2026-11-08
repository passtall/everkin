namespace Everkin.Core.Battle;

/// <summary>
/// The tags a hit or effect carries for the rules that react to them (game_system.md section 4.7). A reaction has its
/// own tags and does not inherit those of the skill or event that triggered it or of the skill that created it (section 6.6).
/// </summary>
[Flags]
public enum EffectTags
{
    None = 0,
    Melee = 1,
    Projectile = 2,
    Direct = 4,
    Physical = 8,
    Magical = 16,
    Move = 32,
}

public enum TriggerKind
{
    /// <summary>The subject was hit (bump damage is a hit too).</summary>
    WhenHit,

    /// <summary>The subject was knocked out. KO'd owners react only to this (section 6.6).</summary>
    WhenKo,

    /// <summary>The subject was moved by someone else. Forced movement itself triggers no reactions unless a passive opts in.</summary>
    WhenMoved,
}

/// <summary>
/// Simultaneous reactions use three levels (section 6.6): effects that change or remove other effects first, damage,
/// healing and statuses normally, timeline changes last.
/// </summary>
public enum ReactionPriority
{
    First = 0,
    Normal = 1,
    Last = 2,
}

/// <summary>Something happened that reactions may answer.</summary>
/// <param name="SubjectUnitId">The unit it happened to (the hit unit, the KO'd unit, the moved unit).</param>
/// <param name="SourceUnitId">The unit that caused it, if any.</param>
/// <param name="Tags">The tags of the triggering hit or effect.</param>
public sealed record Trigger(
    TriggerKind Kind, int SubjectUnitId, int? SourceUnitId = null, EffectTags Tags = EffectTags.None, int Amount = 0);

/// <summary>What a reaction can see and do while it resolves.</summary>
public sealed class ReactionContext
{
    private readonly List<Trigger> _raised;
    private readonly bool _canChain;

    internal ReactionContext(BattleState state, ReactionDef reaction, Trigger trigger, List<Trigger> raised)
    {
        State = state;
        Reaction = reaction;
        Trigger = trigger;
        _raised = raised;
        _canChain = reaction.CanChain;
    }

    public BattleState State { get; }
    public ReactionDef Reaction { get; }
    public Trigger Trigger { get; }

    /// <summary>
    /// Reports something the reaction caused, so reactions to it can follow (a counter's hit raises a `WhenHit` trigger with
    /// the counter's own tags). Ignored when the reaction is declared not to trigger further reactions.
    /// </summary>
    public void Raise(Trigger trigger)
    {
        if (_canChain) _raised.Add(trigger);
    }
}

/// <summary>A reaction a unit has from a skill or passive: a counter, a thorns effect, a "when KO'd" effect.</summary>
public sealed class ReactionDef
{
    public ReactionDef(string id, int ownerUnitId, TriggerKind kind, Action<BattleState, ReactionContext> effect)
    {
        Id = id;
        OwnerUnitId = ownerUnitId;
        Kind = kind;
        Effect = effect;
    }

    public string Id { get; }
    public int OwnerUnitId { get; }
    public TriggerKind Kind { get; }
    public Action<BattleState, ReactionContext> Effect { get; }

    public ReactionPriority Priority { get; init; } = ReactionPriority.Normal;

    /// <summary>The trigger must carry all of these tags (a counter that only answers `melee` hits).</summary>
    public EffectTags RequiredTags { get; init; } = EffectTags.None;

    /// <summary>How many times it can fire in a battle ("once per battle"), or null for no limit. Once per hit needs no limit: each hit is its own trigger.</summary>
    public int? MaxUsesPerBattle { get; init; }

    /// <summary>False when the reaction states that it cannot trigger further reactions.</summary>
    public bool CanChain { get; init; } = true;

    /// <summary>Extra condition. By default the trigger's subject must be the owner ("when I am hit").</summary>
    public Func<BattleState, Trigger, bool>? Condition { get; init; }
}

/// <summary>A reaction is about to resolve.</summary>
public sealed record ReactionFired(int OwnerUnitId, string ReactionId, TriggerKind Kind) : BattleEvent;

/// <summary>A reaction chain went past the limit and was stopped: a design bug for the test harness to report (section 6.6).</summary>
public sealed record ReactionChainAborted(int Fired) : BattleEvent;

/// <summary>What resolving a batch of triggers did.</summary>
/// <param name="Fired">How many reactions resolved, chains included.</param>
/// <param name="Aborted">The chain did not end and was stopped at the limit.</param>
public sealed record ReactionResult(int Fired, bool Aborted);

/// <summary>
/// Reaction resolution (game_system.md section 6.6). After each event or simultaneous block the caller hands the resulting
/// triggers to <see cref="Resolve"/>. Reactions chain by default: a reaction that causes a hit raises that hit's own
/// reactions, which resolve right after it, before the next reaction of the batch (depth first). There is no chain limit
/// in the rules; the limit here only stops an endless chain and reports it.
/// </summary>
public static class Reactions
{
    /// <summary>A generous default: real chains are short.</summary>
    public const int DefaultMaxReactions = 1000;

    public static ReactionResult Resolve(BattleState state, IEnumerable<Trigger> triggers, int maxReactions = DefaultMaxReactions)
    {
        var budget = new Budget(maxReactions);
        Process(state, triggers.ToList(), budget);
        return new ReactionResult(budget.Fired, budget.Aborted);
    }

    private sealed class Budget
    {
        public Budget(int max) => Max = max;
        public int Max { get; }
        public int Fired { get; set; }
        public bool Aborted { get; set; }
    }

    private static void Process(BattleState state, List<Trigger> batch, Budget budget)
    {
        var pairs = Collect(state, batch);
        foreach (var (reaction, trigger) in pairs)
        {
            if (budget.Aborted) return;
            if (!StillEligible(state, reaction, trigger)) continue;
            if (budget.Fired >= budget.Max)
            {
                budget.Aborted = true;
                state.Emit(new ReactionChainAborted(budget.Fired));
                return;
            }

            budget.Fired++;
            state.CountReactionUse(reaction);
            state.Emit(new ReactionFired(reaction.OwnerUnitId, reaction.Id, reaction.Kind));
            var raised = new List<Trigger>();
            reaction.Effect(state, new ReactionContext(state, reaction, trigger, raised));
            if (raised.Count > 0) Process(state, raised, budget);
        }
    }

    /// <summary>
    /// Every (reaction, trigger) pair that applies, in resolution order: priority level first, then the owners' order on
    /// the timeline (the one that joined the timeline first goes first), then the order of the triggers, then the order the
    /// reactions were given.
    /// </summary>
    private static List<(ReactionDef Reaction, Trigger Trigger)> Collect(BattleState state, List<Trigger> batch)
    {
        var found = new List<(ReactionDef Reaction, Trigger Trigger, int TriggerIndex, int ReactionIndex)>();
        for (var t = 0; t < batch.Count; t++)
        {
            var reactions = state.Reactions;
            for (var r = 0; r < reactions.Count; r++)
                if (Matches(state, reactions[r], batch[t]))
                    found.Add((reactions[r], batch[t], t, r));
        }
        return found
            .OrderBy(x => (int)x.Reaction.Priority)
            .ThenBy(x => OwnerPosition(state, x.Reaction.OwnerUnitId).Due)
            .ThenBy(x => OwnerPosition(state, x.Reaction.OwnerUnitId).Order)
            .ThenBy(x => x.TriggerIndex)
            .ThenBy(x => x.ReactionIndex)
            .Select(x => (x.Reaction, x.Trigger))
            .ToList();
    }

    private static (int Due, long Order) OwnerPosition(BattleState state, int ownerId)
    {
        var turn = state.Timeline.TurnOf(ownerId);
        return turn is null ? (int.MaxValue, long.MaxValue) : (turn.Due, turn.Order);
    }

    private static bool Matches(BattleState state, ReactionDef reaction, Trigger trigger)
    {
        if (reaction.Kind != trigger.Kind) return false;
        if ((trigger.Tags & reaction.RequiredTags) != reaction.RequiredTags) return false;
        var applies = reaction.Condition is null
            ? trigger.SubjectUnitId == reaction.OwnerUnitId
            : reaction.Condition(state, trigger);
        return applies && StillEligible(state, reaction, trigger);
    }

    /// <summary>A KO'd owner does not react, except through an explicit "when KO'd" trigger; a spent reaction does not fire.</summary>
    private static bool StillEligible(BattleState state, ReactionDef reaction, Trigger trigger)
    {
        var owner = state.GetUnit(reaction.OwnerUnitId);
        if (owner.IsKnockedOut && reaction.Kind != TriggerKind.WhenKo) return false;
        return reaction.MaxUsesPerBattle is not { } max || state.ReactionUses(reaction) < max;
    }
}
