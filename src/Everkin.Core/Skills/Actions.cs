using Everkin.Core.Battle;

namespace Everkin.Core.Skills;

/// <summary>
/// The actions a unit can take on its turn: a skill, Move or Skip Turn (game_system.md sections 6.3, 6.4). Each returns the
/// ordered list of events it caused, which the UI plays; only the core changes the state.
/// </summary>
public static class Actions
{
    /// <summary>
    /// Why a skill cannot be used now, or null if it can (section 4.6: the UI shows the concrete reason): the battle is over,
    /// the unit cannot act or does not know the skill, Silenced blocks a `magical` skill, the side is not far enough ahead
    /// for the Momentum cost, the skill's own requirements, or the target is missing or not legal.
    /// </summary>
    public static string? WhyNot(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        if (state.IsOver) return "the battle is over";
        if (use.UserId < 0 || use.UserId >= state.Units.Count) return "unknown unit";
        var user = state.GetUnit(use.UserId);
        if (user.IsKnockedOut || !state.Formation.Contains(use.UserId)) return "the unit cannot act";
        if (!catalog.TryGet(use.SkillId, out var skill)) return $"unknown skill {use.SkillId}";
        if (!user.KnownSkills.Contains(use.SkillId)) return "the unit does not know this skill";
        if (skill.IsPassive) return "a passive is always on and is not used";
        if (skill.Magical && !Statuses.CanUseMagical(user)) return "Silenced: magical skills cannot be used";
        if (!state.Momentum.CanPay(user.Side, skill.MomentumCost)) return $"needs the side to be {skill.MomentumCost} ahead in Momentum";
        var own = skill.Unavailable(state, use.UserId);
        if (own is not null) return own;

        switch (skill.Target)
        {
            case TargetKind.None:
                return use.TargetUnitId is null && use.Row is null && use.Placement is null ? null : "this skill takes no target";
            case TargetKind.Row:
                if (use.Row is not { } row) return "needs a row";
                return skill.LegalRows(state, use.UserId).Contains(row) ? null : "not a legal row";
            case TargetKind.Placement:
                if (use.Placement is not { } place) return "needs a place";
                return skill.PlacementOptions(state, use.UserId).Contains(place) ? null : "not a legal place";
            default:
                if (use.TargetUnitId is not { } target) return "needs a target";
                if (target < 0 || target >= state.Units.Count) return "unknown target";
                var options = skill.Options with { Delivery = skill.Delivery };
                if (!Targeting.IsLegalTarget(state, use.UserId, target, skill.Reach, options)) return "not a legal target";
                return skill.TargetFilter(state, use.UserId, target) ? null : "not a valid target for this skill";
        }
    }

    /// <summary>Every unit that is a legal target of the skill right now (empty for skills that take none).</summary>
    public static IReadOnlyList<int> LegalTargets(BattleState state, SkillCatalog catalog, int userId, string skillId)
    {
        var skill = catalog.Get(skillId);
        if (skill.Target != TargetKind.Unit) return Array.Empty<int>();
        return Targeting.LegalTargets(state, userId, skill.Reach, skill.Options with { Delivery = skill.Delivery })
            .Where(id => skill.TargetFilter(state, userId, id)).ToArray();
    }

    /// <summary>
    /// Uses a skill in the order of section 6.5: check that it is usable and the target legal, pay the Momentum cost, set the
    /// user's next Wait, then the skill's events (each: apply, KO check, Momentum shift, reactions, victory check), and finally
    /// Bleeding for a `melee` or `move` skill. A Confused unit's skill has a 50% chance to take a random legal target instead.
    /// Throws if the use is not allowed (see <see cref="WhyNot"/>).
    /// </summary>
    public static IReadOnlyList<BattleEvent> Use(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        var why = WhyNot(state, catalog, use);
        if (why is not null) throw new InvalidOperationException($"Cannot use {use.SkillId}: {why}.");
        var skill = catalog.Get(use.SkillId);
        var user = state.GetUnit(use.UserId);

        var target = use.TargetUnitId;
        if (skill.Target == TargetKind.Unit && Statuses.RollConfusion(state, use.UserId))
        {
            var options = LegalTargets(state, catalog, use.UserId, use.SkillId);
            if (options.Count > 0) target = options[state.Random.NextInt(options.Count)];
        }

        state.BeginAction();
        state.Emit(new SkillUsed(use.UserId, use.SkillId, target));
        if (skill.MomentumCost > 0) state.PayMomentum(user.Side, skill.MomentumCost);
        // Effects that add Delay to the next skill (Net Trap's +3) count once; Delay stays within 1 to 10 after all modifiers.
        ScheduleNextTurn(state, use.UserId, skill.Delay + Statuses.ConsumeNextSkillDelay(state, use.UserId));

        if (Statuses.Get(user, StatusIds.LieInWait) is { } lie)
        {
            // Lie in Wait: the Stun lands first and the skill is lost; its cost and Delay were paid (decided 2026-11-08).
            Statuses.Remove(state, use.UserId, StatusIds.LieInWait, StatusEndCause.Consumed);
            Statuses.Apply(state, new StatusApplication(use.UserId, StatusIds.Stunned, Duration: 10, SourceUnitId: lie.SourceUnitId));
            if (lie.SourceUnitId is { } hunter) Stealth.Break(state, hunter, StealthCause.Attacked);
            state.Emit(new SkillLost(use.UserId, use.SkillId));
            Settle(state, lie.SourceUnitId);
        }
        else
        {
            skill.Resolve(new SkillContext(state, skill, use.UserId, use, target));
            AfterSkill(state, use.UserId, skill.Tags.HasFlag(EffectTags.Melee), skill.Tags.HasFlag(EffectTags.Move));
        }
        user.LastActionIndex = state.ActionCounter;
        return state.DrainEvents();
    }

    /// <summary>
    /// The basic Move action (section 9.1): the unit lands in the gap its player chose. Delay 5, the `move` tag (it triggers
    /// Bleeding). A Speed 6 unit waits 10 time units.
    /// </summary>
    public static IReadOnlyList<BattleEvent> Move(BattleState state, int userId, Row row, int gap)
    {
        if (state.IsOver) throw new InvalidOperationException("The battle is over.");
        state.BeginAction();
        Movement.Move(state, userId, row, gap);
        ScheduleNextTurn(state, userId, Movement.MoveDelay);
        AfterSkill(state, userId, melee: false, move: true);
        state.GetUnit(userId).LastActionIndex = state.ActionCounter;
        return state.DrainEvents();
    }

    /// <summary>Skip Turn (section 6.4): always allowed; it costs the same as a Delay 5 skill, 5 + (11 − Speed).</summary>
    public static IReadOnlyList<BattleEvent> SkipTurn(BattleState state, int userId)
    {
        if (state.IsOver) throw new InvalidOperationException("The battle is over.");
        state.BeginAction();
        ScheduleNextTurn(state, userId, Timeline.SkippedTurnDelay);
        state.GetUnit(userId).LastActionIndex = state.ActionCounter;
        return state.DrainEvents();
    }

    /// <summary>The next Wait is set when the skill is used, before its effects (section 6.1). A turn still on the timeline is replaced.</summary>
    private static void ScheduleNextTurn(BattleState state, int userId, int delay)
    {
        if (state.Timeline.TurnOf(userId) is { } old) state.Timeline.Remove(old.Id);
        state.Timeline.ScheduleTurn(userId, delay, state.GetUnit(userId).EffectiveSpeed);
    }

    private static void AfterSkill(BattleState state, int userId, bool melee, bool move)
    {
        if (state.IsOver || state.GetUnit(userId).IsKnockedOut) return;
        if (Statuses.OnSkillUsed(state, userId, melee, move)) Settle(state, userId);
    }

    /// <summary>
    /// KO check, the reactions to any KO, and the victory check, for damage that is not a skill's own hit (statuses, ground
    /// effects, Bleeding). Not a hit: no "when hit" reactions, no stealth break, no Momentum.
    /// </summary>
    public static void Settle(BattleState state, int? sourceUnitId = null)
    {
        for (var guard = 0; guard < 50; guard++)
        {
            var fallen = Knockout.ResolveAll(state);
            if (fallen.Count == 0) break;
            Reactions.Resolve(state, fallen.Select(id => new Trigger(TriggerKind.WhenKo, id, sourceUnitId)).ToArray());
        }
        BattleEnd.Resolve(state);
    }
}
