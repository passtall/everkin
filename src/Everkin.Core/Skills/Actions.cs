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
    public static string? WhyNot(BattleState state, SkillCatalog catalog, SkillUse use) =>
        WhyNotCore(state, catalog, use) ?? (catalog.TryGet(use.SkillId, out var skill) ? skill.CheckUse(state, catalog, use) : null);

    private static string? WhyNotCore(BattleState state, SkillCatalog catalog, SkillUse use)
    {
        if (state.IsOver) return "the battle is over";
        if (use.UserId < 0 || use.UserId >= state.Units.Count) return "unknown unit";
        var user = state.GetUnit(use.UserId);
        if (user.IsKnockedOut || !state.Formation.Contains(use.UserId)) return "the unit cannot act";
        if (!catalog.TryGet(use.SkillId, out var skill)) return $"unknown skill {use.SkillId}";
        if (!user.KnownSkills.Contains(use.SkillId)) return "the unit does not know this skill";
        if (skill.IsPassive) return "a passive is always on and is not used";
        if (skill.IsAttack && state.AttackBans.Any(b => b(state, use.UserId))) return "this unit can never attack";
        if (skill.Magical && !Statuses.CanUseMagical(user)) return "Silenced: magical skills cannot be used";
        if (user.KnownSkills.Contains("grand_chorus") && !catalog.Content.AttackSkills.Any(a => a.Id == use.SkillId))
            return "Grand Chorus: only the type-specific attack can be used";
        var cost = CostOf(state, use.UserId, skill);
        if (!state.Momentum.CanPay(user.Side, cost)) return $"needs the side to be {cost} ahead in Momentum";
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
                if (skill.PlacesRandomly(state, use.UserId)) return skill.PlacementOptions(state, use.UserId).Count > 0 ? null : "no free place";
                if (use.Placement is not { } place) return "needs a place";
                return skill.PlacementOptions(state, use.UserId).Contains(place) ? null : "not a legal place";
            case TargetKind.TwoUnits:
            {
                if (use.TargetUnitId is not { } first || use.SecondTargetUnitId is not { } second) return "needs two targets";
                if (first < 0 || first >= state.Units.Count || second < 0 || second >= state.Units.Count) return "unknown target";
                if (first == second) return "needs two different units";
                var opts = skill.Options with { Delivery = skill.Delivery };
                foreach (var who in new[] { first, second })
                {
                    if (!Targeting.IsLegalTarget(state, use.UserId, who, ReachOf(state, use.UserId, skill), opts)) return "not a legal target";
                    if (!skill.TargetFilter(state, use.UserId, who)) return "not a valid target for this skill";
                }
                return skill.PairFilter(state, use.UserId, first, second) ? null : "not a valid pair for this skill";
            }
            case TargetKind.UnitAndPlacement:
            {
                if (use.TargetUnitId is not { } who) return "needs a target";
                if (who < 0 || who >= state.Units.Count) return "unknown target";
                if (!Targeting.IsLegalTarget(state, use.UserId, who, ReachOf(state, use.UserId, skill), skill.Options with { Delivery = skill.Delivery })) return "not a legal target";
                if (!skill.TargetFilter(state, use.UserId, who)) return "not a valid target for this skill";
                if (skill.PlacesRandomly(state, use.UserId)) return skill.PlacementOptionsFor(state, use.UserId, who).Count > 0 ? null : "no free place";
                if (use.Placement is not { } where) return "needs a place";
                return skill.PlacementOptionsFor(state, use.UserId, who).Contains(where) ? null : "not a legal place";
            }
            default:
                if (use.TargetUnitId is not { } target) return "needs a target";
                if (target < 0 || target >= state.Units.Count) return "unknown target";
                var options = skill.Options with { Delivery = skill.Delivery };
                if (!Targeting.IsLegalTarget(state, use.UserId, target, ReachOf(state, use.UserId, skill), options)) return "not a legal target";
                if (!skill.TargetFilter(state, use.UserId, target)) return "not a valid target for this skill";
                return DistractedFrom(state, use.UserId, skill, target);
        }
    }

    /// <summary>The Momentum a skill costs its user now: 0 after Press the Advantage, otherwise the skill's cost changed by passives (All In).</summary>
    public static int CostOf(BattleState state, int userId, Skill skill)
    {
        if (Statuses.Has(state.GetUnit(userId), StatusIds.FreeMomentum)) return 0;
        var cost = skill.MomentumCost;
        foreach (var modifier in state.MomentumCostModifiers) cost = modifier(state, userId, cost);
        return cost;
    }

    /// <summary>The reach a skill has for this user now: unlimited for an attack by a unit with Phase Weapon on it (its next attack), else the skill's own.</summary>
    public static Reach ReachOf(BattleState state, int userId, Skill skill) =>
        (skill.IsAttack && Statuses.Has(state.GetUnit(userId), StatusIds.Phased)) || state.ReachOverrides.Any(o => o(state, userId)) ? Reach.Unlimited : skill.Reach;

    /// <summary>Every unit that is a legal target of the skill right now (empty for skills that take none).</summary>
    public static IReadOnlyList<int> LegalTargets(BattleState state, SkillCatalog catalog, int userId, string skillId)
    {
        var skill = catalog.Get(skillId);
        if (skill.Target != TargetKind.Unit) return Array.Empty<int>();
        return Targeting.LegalTargets(state, userId, ReachOf(state, userId, skill), skill.Options with { Delivery = skill.Delivery })
            .Where(id => skill.TargetFilter(state, userId, id) && DistractedFrom(state, userId, skill, id) is null).ToArray();
    }

    /// <summary>
    /// Distract: for the Distracted unit's next attack that takes a target, the Guardian is the only legal target if it is a legal
    /// target of that skill; otherwise nothing changes. Returns the reason a target is refused, or null.
    /// </summary>
    private static string? DistractedFrom(BattleState state, int userId, Skill skill, int targetId)
    {
        if (!skill.IsAttack || Statuses.Get(state.GetUnit(userId), StatusIds.Distracted)?.SourceUnitId is not { } guardian) return null;
        if (guardian == targetId || state.GetUnit(guardian).IsKnockedOut) return null;
        var options = skill.Options with { Delivery = skill.Delivery };
        return Targeting.IsLegalTarget(state, userId, guardian, ReachOf(state, userId, skill), options) && skill.TargetFilter(state, userId, guardian)
            ? "Distracted: must target the Guardian" : null;
    }

    /// <summary>
    /// Uses a skill in the order of section 6.5: check that it is usable and the target legal, pay the Momentum cost, set the
    /// user's next Wait, then the skill's events (each: apply, KO check, Momentum shift, reactions, victory check), and finally
    /// Bleeding for a `melee` or `move` skill. A Confused unit's skill has a 50% chance to take a random legal target instead.
    /// Throws if the use is not allowed (see <see cref="WhyNot"/>).
    /// </summary>
    public static IReadOnlyList<BattleEvent> Use(BattleState state, SkillCatalog catalog, SkillUse use, bool validated = false)
    {
        var why = validated ? null : WhyNot(state, catalog, use);   // the AI search has just checked the move it plays
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
        var cost = CostOf(state, use.UserId, skill);
        if (Statuses.Has(user, StatusIds.FreeMomentum)) Statuses.Remove(state, use.UserId, StatusIds.FreeMomentum, StatusEndCause.Consumed);
        if (cost > 0) state.PayMomentum(user.Side, cost);
        var rhythm = user.LastSkillId == skill.Id ? state.RepeatDelayReductions.Select(r => r(state, use.UserId, skill.Id)).DefaultIfEmpty(0).Max() : 0;
        // Effects that add Delay to the next skill (Net Trap's +3) count once; Delay stays within 1 to 10 after all modifiers.
        var boundStrength = state.SharedPoolOf(use.UserId) is { BoundStrength: true } ? 1 : 0;   // Bound Strength: 1 less Delay while sharing HP
        ScheduleNextTurn(state, use.UserId, Math.Clamp(skill.Delay + Statuses.ConsumeNextSkillDelay(state, use.UserId) - boundStrength - rhythm
            + state.SkillDelayModifiers.Sum(m => m(state, use.UserId, skill.IsAttack)), 1, 10));

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
            if (skill.IsAttack && Statuses.Has(user, StatusIds.Distracted))
                Statuses.Remove(state, use.UserId, StatusIds.Distracted, StatusEndCause.Consumed);   // it counted: this was its next attack
            // The Sorcerer's enchantments on the user are used up by this skill: Phase Weapon and Piercing Enchant by an attack, Echo Enchant by any skill.
            var phased = skill.IsAttack ? Statuses.Get(user, StatusIds.Phased) : null;
            var piercing = skill.IsAttack ? Statuses.Get(user, StatusIds.Piercing) : null;
            var echo = Statuses.Get(user, StatusIds.Echo);
            foreach (var used in new[] { phased?.Id, piercing?.Id, echo?.Id })
                if (used is not null) Statuses.Remove(state, use.UserId, used, StatusEndCause.Consumed);

            var context = new SkillContext(state, skill, use.UserId, use, target)
            {
                Catalog = catalog,
                IgnoresInterception = phased is { Magnitude: > 0 },   // Phasing Strike
                PierceBehind = piercing is not null,
            };
            skill.Resolve(context);
            AfterSkill(state, use.UserId, skill.Tags.HasFlag(EffectTags.Melee), skill.Tags.HasFlag(EffectTags.Move));

            // Echo Enchant: the skill happens a second time at the same target, free (no Delay, no Momentum). Skills that place something do not echo.
            if (echo is not null && CanEcho(state, skill, use, target))
            {
                state.Emit(new SkillUsed(use.UserId, use.SkillId, target));
                skill.Resolve(new SkillContext(state, skill, use.UserId, use, target) { IgnoresInterception = context.IgnoresInterception, Catalog = catalog });
            }
        }
        user.LastActionIndex = state.ActionCounter;
        user.LastSkillId = skill.Id;
        user.LastActiveSkillId = skill.Id;
        user.SneakReady = false;
        Turrets.AfterAction(state, use.UserId);
        state.RunAuras();
        return state.DrainEvents();
    }

    /// <summary>Whether Echo Enchant's second use can happen: the battle goes on, the user stands and the same target is still there.</summary>
    private static bool CanEcho(BattleState state, Skill skill, SkillUse use, int? target)
    {
        if (state.IsOver || state.GetUnit(use.UserId).IsKnockedOut) return false;
        if (skill.Unavailable(state, use.UserId) is not null) return false;      // a second Hide has nothing to do
        return skill.Target switch
        {
            TargetKind.None => true,
            TargetKind.Unit => target is { } t && !state.GetUnit(t).IsRemoved && state.Formation.Contains(t)
                && (state.GetUnit(t).IsKnockedOut ? skill.Options.IncludeKnockedOut : true) && skill.TargetFilter(state, use.UserId, t),
            TargetKind.Row => use.Row is { } row && skill.LegalRows(state, use.UserId).Contains(row),
            _ => false,
        };
    }

    /// <summary>Sneak: the places the stealthed Rogue may step to at the start of its turn: one row forward or back, any gap. Empty if it cannot.</summary>
    public static IReadOnlyList<MoveOption> SneakOptions(BattleState state, int userId)
    {
        var unit = state.GetUnit(userId);
        if (state.IsOver || !unit.SneakReady || !unit.IsStealthed || unit.IsKnockedOut || !unit.KnownSkills.Contains("sneak"))
            return Array.Empty<MoveOption>();
        var row = (int)state.Formation.PositionOf(userId).Row;
        return Movement.LegalMoves(state, userId).Where(m => Math.Abs((int)m.Row - row) == 1).ToArray();
    }

    /// <summary>Sneak: the extra move. It costs nothing on the timeline and stealth stays. Allowed once, before the unit's action.</summary>
    public static IReadOnlyList<BattleEvent> SneakMove(BattleState state, int userId, Row row, int gap)
    {
        if (!SneakOptions(state, userId).Contains(new MoveOption(row, gap)))
            throw new InvalidOperationException("Sneak cannot move there now.");
        state.GetUnit(userId).SneakReady = false;
        Movement.Move(state, userId, row, gap);
        Settle(state, userId);
        state.RunAuras();
        return state.DrainEvents();
    }

    /// <summary>
    /// The basic Move action (section 9.1): the unit lands in the gap its player chose. Delay 5, the `move` tag (it triggers
    /// Bleeding). A Speed 6 unit waits 10 time units.
    /// </summary>
    public static IReadOnlyList<BattleEvent> Move(BattleState state, int userId, Row row, int gap)
    {
        if (state.IsOver) throw new InvalidOperationException("The battle is over.");
        if (!Movement.LegalMoves(state, userId).Contains(new MoveOption(row, gap)))
            throw new InvalidOperationException($"Unit {userId} cannot move to {row} gap {gap}.");
        state.BeginAction();
        Movement.Move(state, userId, row, gap);
        ScheduleNextTurn(state, userId, Movement.MoveDelay);
        AfterSkill(state, userId, melee: false, move: true);
        state.GetUnit(userId).LastActionIndex = state.ActionCounter;
        state.GetUnit(userId).LastSkillId = null;
        state.GetUnit(userId).SneakReady = false;
        Turrets.AfterAction(state, userId);
        state.RunAuras();
        return state.DrainEvents();
    }

    /// <summary>Skip Turn (section 6.4): always allowed; it costs the same as a Delay 5 skill, 5 + (11 − Speed).</summary>
    public static IReadOnlyList<BattleEvent> SkipTurn(BattleState state, int userId)
    {
        if (state.IsOver) throw new InvalidOperationException("The battle is over.");
        state.BeginAction();
        ScheduleNextTurn(state, userId, Timeline.SkippedTurnDelay);
        state.GetUnit(userId).LastActionIndex = state.ActionCounter;
        state.GetUnit(userId).LastSkillId = null;
        state.GetUnit(userId).SneakReady = false;
        Turrets.AfterAction(state, userId);
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
        state.RunAuras();
        BattleEnd.Resolve(state);
    }
}
