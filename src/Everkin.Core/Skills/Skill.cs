using Everkin.Core.Battle;

namespace Everkin.Core.Skills;

/// <summary>What a skill asks its user to select (game_system.md section 4.7).</summary>
public enum TargetKind
{
    /// <summary>Nothing: self, passives, areas that pick their own units.</summary>
    None,

    /// <summary>One unit.</summary>
    Unit,

    /// <summary>One row of either side (Read Tracks, Creep Closer's destination row).</summary>
    Row,

    /// <summary>A free place on the board: a row and a gap (placing a trap, a summon, a construct).</summary>
    Placement,

    /// <summary>A unit and a place next to it (Leap to Defend).</summary>
    UnitAndPlacement,

    /// <summary>Two different units (Life Link; Steal Blessing: the unit it is taken from, then the unit it goes to).</summary>
    TwoUnits,
}

/// <summary>A unit chose to use a skill, with its target if the skill needs one.</summary>
public sealed record SkillUse(
    int UserId, string SkillId, int? TargetUnitId = null, RowTarget? Row = null, MoveOption? Placement = null,
    int? SecondTargetUnitId = null);

/// <summary>
/// One skill: a small class found by its skill ID (production.md section 2.2). The numbers that belong to the skill's definition
/// (Delay, Momentum cost, delivery, reach) are given by the skill; the mechanic is <see cref="Resolve"/>, built from the shared
/// building blocks of <see cref="SkillContext"/> so no rule is written twice.
/// </summary>
public abstract class Skill
{
    protected Skill(string id, int delay, int momentumCost)
    {
        Id = id;
        Delay = delay;
        MomentumCost = momentumCost;
    }

    public string Id { get; }

    /// <summary>1 is the quickest, 10 the heaviest. Effects that add or remove Delay are applied by the executor and clamped to 1 to 10.</summary>
    public int Delay { get; }

    /// <summary>The Momentum the user's side must be ahead by, and spends (0 to 10).</summary>
    public int MomentumCost { get; }

    public abstract TargetKind Target { get; }

    /// <summary>A passive or keystone: always on, never used as an action. Its <see cref="Install"/> sets it up at battle start.</summary>
    public virtual bool IsPassive => false;

    /// <summary>An area skill: its hits are area effects (Freeze in Place reacts to them).</summary>
    public virtual bool IsArea => false;

    /// <summary>An attack: any skill that deals damage to a unit (game_system section 4.7). Living Wall bans it, Distract aims it.</summary>
    public virtual bool IsAttack => false;

    /// <summary>`melee`, `projectile` or `direct`; null for skills that are none of these (self effects, placements).</summary>
    public virtual Delivery? Delivery => null;

    /// <summary>The tags the skill's hits carry for the rules that react to them (section 4.7).</summary>
    public virtual EffectTags Tags => EffectTags.None;

    /// <summary>`magical` skills are blocked by Silenced (Bolt and every active skill of Sorcerer, Elementalist, Healer, Summoner and Primalist).</summary>
    public virtual bool Magical => false;

    /// <summary>The reach of a skill with a target (section 7.4).</summary>
    public virtual Reach Reach => Reach.Unlimited;

    /// <summary>Which side the target may be on, and the other targeting options.</summary>
    public virtual TargetOptions Options => default;

    /// <summary>Position or state requirements beyond the target: why the skill cannot be used now, or null. The UI shows the reason.</summary>
    public virtual string? Unavailable(BattleState state, int userId) => null;

    /// <summary>Extra condition on the target beyond reach and legality (a skill that needs a Marked unit).</summary>
    public virtual bool TargetFilter(BattleState state, int userId, int targetId) => true;

    /// <summary>Extra checks on the whole use, after the target checks (Lift a Skill: the copied skill and its target). Null if fine.</summary>
    public virtual string? CheckUse(BattleState state, SkillCatalog catalog, SkillUse use) => null;

    /// <summary>For a skill with two targets: whether this pair is allowed.</summary>
    public virtual bool PairFilter(BattleState state, int userId, int firstId, int secondId) => true;

    /// <summary>The rows a skill with a row target may pick now.</summary>
    public virtual IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => Array.Empty<RowTarget>();

    /// <summary>The places a skill that places something may put it.</summary>
    public virtual IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) => Array.Empty<MoveOption>();

    /// <summary>The places allowed next to a chosen unit, for a skill that takes a unit and a place.</summary>
    public virtual IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId) => Array.Empty<MoveOption>();

    /// <summary>
    /// Called once at battle start for every unit that knows the skill (section 6): a passive registers its reactions and
    /// hooks here, or does what it does "at battle start".
    /// </summary>
    public virtual void Install(BattleState state, int ownerId) { }

    /// <summary>The skill's effects, in the order its definition states. Everything goes through the context.</summary>
    public abstract void Resolve(SkillContext context);
}

/// <summary>
/// What a skill needs while it resolves: the state, the user, the target, and the building blocks that carry the rules of
/// section 6.5 (apply, KO check, Momentum shift, reactions, victory check) and of damage, interception, Momentum, stealth and
/// statuses, so each skill only says what it does.
/// </summary>
public sealed class SkillContext
{
    private readonly Skill _skill;
    private bool _attacked;
    private bool _pierced;
    private bool _expanded;

    internal SkillContext(BattleState state, Skill skill, int userId, SkillUse use, int? targetId)
    {
        State = state;
        _skill = skill;
        UserId = userId;
        TargetId = targetId;
        Row = use.Row;
        Placement = use.Placement;
        SecondTargetId = use.SecondTargetUnitId;
    }

    /// <summary>Phasing Strike: this attack cannot be intercepted (set by the executor from the user's Phase Weapon enchantment).</summary>
    internal bool IgnoresInterception { get; set; }

    /// <summary>Piercing Enchant: this attack also hits the unit behind its target (set by the executor).</summary>
    internal bool PierceBehind { get; set; }

    /// <summary>The catalog the skill was found in (Lift a Skill copies another skill).</summary>
    public SkillCatalog? Catalog { get; internal init; }

    /// <summary>The second unit of a skill with two targets.</summary>
    public int? SecondTargetId { get; }

    /// <summary>The chosen row, for a skill with a row target.</summary>
    public RowTarget? Row { get; }

    /// <summary>The chosen place, for a skill that places something.</summary>
    public MoveOption? Placement { get; }

    public BattleState State { get; }
    public int UserId { get; }

    /// <summary>The chosen target, or null.</summary>
    public int? TargetId { get; }

    public Unit User => State.GetUnit(UserId);

    public Side UserSide => User.Side;

    /// <summary>The battle is decided: pending events and remaining hits are cancelled (section 12.3).</summary>
    public bool Over => State.IsOver;

    /// <summary>A skill number written for Power 5, scaled to the user's Power (summons use their summoner's), rounded down, minimum 1.</summary>
    public IntRange Scaled(IntRange atPower5) => DamageMath.ScaleRange(atPower5, State.PowerOf(UserId));

    public int Scaled(int atPower5) => DamageMath.Scale(atPower5, State.PowerOf(UserId));

    /// <summary>One piece of a hit: who is hit and for how much (a rolled, scaled amount).</summary>
    public readonly record struct HitSpec(int TargetId, IntRange Range, bool NoInterception = false);

    /// <summary>What happened to one hit.</summary>
    public sealed record HitReport(int TargetId, HitOutcome Outcome, IReadOnlyList<InterceptedShare> Interceptions, int Rolled)
    {
        /// <summary>Whoever took the status or other non-damage effect riding on the hit: the first interceptor if the hit was intercepted, else the target.</summary>
        public int EffectReceiverId => Interceptions.Count > 0 ? Interceptions[0].InterceptorId : TargetId;
    }

    /// <summary>
    /// A single damaging event: roll the damage (scaled to the user's Power), let interception and shields act, check KO, shift
    /// Momentum, resolve reactions, check victory (section 6.5 step 4). Returns what happened so the skill can add its riders
    /// (a Bleeding chance on the target).
    /// </summary>
    public HitReport Hit(int targetId, IntRange rangeAtPower5)
    {
        var reports = HitBlock(new[] { new HitSpec(targetId, rangeAtPower5) });
        return reports[0];
    }

    /// <summary>
    /// A simultaneous block of hits (an area): every hit is applied, then the KO check, then one Momentum shift for the whole
    /// action (1 toward if any hit lands, else 1 against if one missed, else 0), then the reactions, then the victory check.
    /// Each target rolls its own damage.
    /// </summary>
    public IReadOnlyList<HitReport> HitBlock(IReadOnlyList<HitSpec> hits)
    {
        var reports = new List<HitReport>();
        var triggers = new List<Trigger>();
        if (!_attacked)
        {
            _attacked = true;
            Stealth.Break(State, UserId, StealthCause.Attacked);   // attacking ends the user's stealth
        }

        if (_skill.IsArea)
        {
            // An area effect that would hit a protected unit hits its protector instead, once (Pledge).
            var redirected = new List<HitSpec>();
            foreach (var spec in hits)
            {
                var to = spec.TargetId;
                foreach (var hook in State.AreaRedirects)
                    if (hook(State, to) is { } other) { to = other; break; }
                if (redirected.All(r => r.TargetId != to)) redirected.Add(spec with { TargetId = to });
            }
            hits = redirected;
        }

        if (_skill.IsAttack && !_expanded && hits.Count > 0)
        {
            // Crowd Fighter: the attack also hits the other units a passive names, with the same range.
            _expanded = true;
            var first = hits[0].Range;
            var current = hits;
            var extra = State.AttackExpanders.SelectMany(e => e(State, UserId)).Distinct()
                .Where(id => current.All(h => h.TargetId != id)).Select(id => new HitSpec(id, first)).ToList();
            if (extra.Count > 0) hits = hits.Concat(extra).ToList();
        }

        if (PierceBehind && !_pierced && TargetId is { } aimedAt && hits.FirstOrDefault(h => h.TargetId == aimedAt) is { Range: var pierceRange } && hits.Any(h => h.TargetId == aimedAt))
        {
            // Piercing Enchant: the unit behind the target is also hit, a separate roll of the same range, and it cannot be intercepted.
            _pierced = true;
            var extra = AreaPatterns.HitBehind(State, AreaPatterns.UnitsBehind(State, aimedAt))
                .Select(id => new HitSpec(id, pierceRange, NoInterception: true));
            hits = hits.Concat(extra).ToList();
        }

        foreach (var spec in hits)
        {
            if (State.GetUnit(spec.TargetId).IsKnockedOut) continue;   // an earlier part of the block took it out
            reports.Add(ApplyHit(spec, triggers));
        }

        EndOfEvent(reports.Select(r => r.Outcome), triggers, shiftMomentum: true);
        return reports;
    }

    private HitReport ApplyHit(HitSpec spec, List<Trigger> triggers)
    {
        // Passives that act before an attack resolves may aim it at another unit (Decoy).
        var aimed = spec.TargetId;
        foreach (var hook in State.PreHitHooks)
            if (hook(State, UserId, aimed) is { } other) { aimed = other; break; }
        spec = spec with { TargetId = aimed };

        var target = State.GetUnit(spec.TargetId);
        var friendly = target.Side == UserSide;
        var rolled = DamageMath.Roll(State.Random, Scaled(spec.Range));
        var tags = _skill.Tags;

        var bonus = State.DamageBonuses.Sum(h => h(State, UserId, spec.TargetId));
        if (bonus != 0) rolled = Math.Max(1, rolled * (100 + bonus) / 100);                  // Reckless Swing, Last Stand: the bonuses add up

        // Sidestep: the next attack on the unit misses and the status is spent.
        if (_skill.IsAttack && Statuses.Has(target, StatusIds.Sidestep))
        {
            Statuses.Remove(State, spec.TargetId, StatusIds.Sidestep, StatusEndCause.Consumed);
            return Missed(spec, rolled);
        }

        // Blind: all of the unit's attacks have a 50% chance to miss.
        var miss = Statuses.AttackMissChance(User);
        if (miss > 0 && State.Random.Chance(miss)) return Missed(spec, rolled);

        var interception = _skill.Delivery is { } delivery && Interception.CanBeIntercepted(delivery, IgnoresInterception || spec.NoInterception)
            ? Interception.Resolve(State, spec.TargetId, rolled)
            : new InterceptionResult(Array.Empty<InterceptedShare>(), rolled);

        var hpLost = 0;
        foreach (var share in interception.Shares)
        {
            var interceptor = State.GetUnit(share.InterceptorId);
            if (interceptor.IsKnockedOut) continue;
            hpLost += State.DealDamage(UserId, share.InterceptorId, share.Amount).HpLost;
            WasHit(share.InterceptorId, triggers, tags);
        }

        var outcome = HitOutcome.Intercepted;
        if (interception.Remaining > 0)
        {
            var result = State.DealDamage(UserId, spec.TargetId, interception.Remaining);
            hpLost += result.HpLost;
            if (!friendly) foreach (var hook in State.TargetHitHooks.ToArray()) hook(State, UserId, spec.TargetId);
            WasHit(spec.TargetId, triggers, tags);
            outcome = interception.Intercepted ? HitOutcome.Intercepted
                : friendly ? HitOutcome.OnActingSide
                : result.FullyAbsorbed || Statuses.Has(target, StatusIds.StoneShell) ? HitOutcome.FullyAbsorbed   // Stone Shell takes nothing, like a full shield
                : HitOutcome.Landed;
        }
        else if (friendly && !interception.Intercepted) outcome = HitOutcome.OnActingSide;

        if (friendly && outcome == HitOutcome.Landed) outcome = HitOutcome.OnActingSide;
        foreach (var hook in State.PostHitHooks.ToArray()) hook(State, UserId, spec.TargetId, hpLost);
        return new HitReport(spec.TargetId, outcome, interception.Shares, rolled);
    }

    private HitReport Missed(HitSpec spec, int rolled)
    {
        State.Emit(new AttackMissed(UserId, spec.TargetId));
        foreach (var hook in State.AttackMissHooks.ToArray()) hook(State, UserId, spec.TargetId);
        return new HitReport(spec.TargetId, HitOutcome.Missed, Array.Empty<InterceptedShare>(), rolled);
    }

    /// <summary>Execution: the target's HP goes to 0. Not a roll: it ignores interception and shields. Breaks the user's stealth, counts as a landed hit.</summary>
    public void Execute(int targetId)
    {
        if (!_attacked)
        {
            _attacked = true;
            Stealth.Break(State, UserId, StealthCause.Attacked);
        }
        var triggers = new List<Trigger>();
        State.DealDamage(UserId, targetId, State.GetUnit(targetId).Hp, DamageSource.Unreducible);
        WasHit(targetId, triggers, _skill.Tags);
        EndOfEvent(new[] { HitOutcome.Landed }, triggers, shiftMomentum: true);
    }

    /// <summary>Pushes units one row back as a group, each landing where its coordinate was before the push (Tempest, Isolate, Sweeping Kick). Rear-row units stay.</summary>
    public IReadOnlyList<int> PushGroupBack(IEnumerable<int> unitIds) => MoveGroup(unitIds, 1);

    /// <summary>As <see cref="PushGroupBack"/> but one row toward the Front (Advance). Front-row units stay.</summary>
    public IReadOnlyList<int> PullGroupForward(IEnumerable<int> unitIds) => MoveGroup(unitIds, -1);

    private IReadOnlyList<int> MoveGroup(IEnumerable<int> unitIds, int direction)
    {
        var moved = new List<int>();
        var group = unitIds.Where(id => State.Formation.Contains(id) && !State.GetUnit(id).IsKnockedOut)
            .Select(id => (Id: id, Position: State.Formation.PositionOf(id), Coordinate: State.Formation.CenterOf(State.Formation.PositionOf(id)))).ToArray();
        foreach (var (id, start, coordinate) in group)
        {
            if (direction > 0 ? start.Row == Battle.Row.Rear : start.Row == Battle.Row.Front) continue;
            if (!State.Formation.Contains(id) || State.GetUnit(id).IsKnockedOut) continue;
            var position = State.Formation.PositionOf(id);
            if (position.Side != start.Side || position.Row != start.Row) continue;      // an earlier push bumped it elsewhere
            var result = Movement.ForceMove(State, id, (Battle.Row)((int)start.Row + direction), UserSide, coordinate);
            moved.AddRange(result.Moves.Select(m => m.UnitId));
            AfterForcedMove(result);
            if (Over) return moved;
        }
        Settle();
        return moved;
    }

    private void WasHit(int unitId, List<Trigger> triggers, EffectTags tags)
    {
        Stealth.Break(State, unitId, StealthCause.Hit, _skill.IsArea);
        Statuses.OnHit(State, unitId);
        State.GetUnit(unitId).LastHitBy = UserId;
        if (Statuses.Has(State.GetUnit(unitId), StatusIds.Marked)) State.LogMarkedHit(UserId, unitId);
        triggers.Add(new Trigger(TriggerKind.WhenHit, unitId, UserId, tags));
    }

    /// <summary>
    /// The end of an event (section 6.5): KO check, the Momentum shift, the reactions to what happened (and to any KO), then
    /// the victory check. Skills that do not hit call it with no outcomes to settle what they changed.
    /// </summary>
    public void EndOfEvent(IEnumerable<HitOutcome> outcomes, IReadOnlyList<Trigger> triggers, bool shiftMomentum)
    {
        var batch = new List<Trigger>(triggers);
        foreach (var id in Knockout.ResolveAll(State))
            batch.Add(new Trigger(TriggerKind.WhenKo, id, UserId));

        if (shiftMomentum)
        {
            var shift = MomentumRules.ActionShift(outcomes);
            foreach (var modifier in State.MomentumShiftModifiers) shift = modifier(State, UserId, shift);   // All In
            if (shift != 0) State.ShiftMomentum(UserSide, shift);
        }

        // Reactions can KO more units, whose own "when KO'd" reactions follow, until things settle.
        for (var guard = 0; batch.Count > 0 && guard < 50; guard++)
        {
            Reactions.Resolve(State, batch);
            batch = new List<Trigger>();
            foreach (var id in Knockout.ResolveAll(State))
                batch.Add(new Trigger(TriggerKind.WhenKo, id, UserId));
        }

        State.RunAuras();
        BattleEnd.Resolve(State);
    }

    /// <summary>The user's side and the other.</summary>
    public Side EnemySide => UserSide.Opposite();

    /// <summary>What a heal did.</summary>
    public sealed record HealReport(int TargetId, int Healed, IReadOnlyList<InterceptedShare> Interceptions, int Rolled);

    /// <summary>One heal: roll it (scaled to the user's Power), let interception act if the skill is `melee` or `projectile`, heal. Then settle.</summary>
    public HealReport Heal(int targetId, IntRange rangeAtPower5)
    {
        var report = HealOne(new HitSpec(targetId, rangeAtPower5));
        Settle();
        return report;
    }

    /// <summary>A simultaneous block of heals: each unit rolls its own; then one KO check (healing an undead hurts it).</summary>
    public IReadOnlyList<HealReport> HealBlock(IReadOnlyList<HitSpec> heals)
    {
        var reports = new List<HealReport>();
        foreach (var spec in heals)
            if (!State.GetUnit(spec.TargetId).IsKnockedOut) reports.Add(HealOne(spec));
        Settle();
        return reports;
    }

    private HealReport HealOne(HitSpec spec)
    {
        var rolled = DamageMath.Roll(State.Random, Scaled(spec.Range));
        var interception = _skill.Delivery is { } d && Interception.CanBeIntercepted(d)
            ? Interception.Resolve(State, spec.TargetId, rolled)
            : new InterceptionResult(Array.Empty<InterceptedShare>(), rolled);
        foreach (var share in interception.Shares)
            if (!State.GetUnit(share.InterceptorId).IsKnockedOut) HealUnit(share.InterceptorId, share.Amount);
        var healed = interception.Remaining > 0 ? HealUnit(spec.TargetId, interception.Remaining) : 0;
        return new HealReport(spec.TargetId, healed, interception.Shares, rolled);
    }

    /// <summary>
    /// Heals one unit by an already rolled amount. Healing beyond the maximum goes to the unit's adjacent ally with the lowest HP when the user
    /// knows Overflowing Care (ties: the seeded roll); that second heal does not overflow again. Healing an undead hurts it instead.
    /// </summary>
    public int HealUnit(int unitId, int amount)
    {
        var healed = State.Heal(UserId, unitId, amount);
        var overflow = amount - healed;
        if (overflow <= 0 || State.GetUnit(unitId).IsUndead || Statuses.Has(State.GetUnit(unitId), StatusIds.Betrayed) || !User.KnownSkills.Contains("overflowing_care")) return healed;
        var side = State.GetUnit(unitId).Side;
        var candidates = State.Formation.Adjacent(unitId)
            .Where(id => State.GetUnit(id).Side == side && !State.GetUnit(id).IsKnockedOut && !State.GetUnit(id).IsUndead).ToList();
        if (candidates.Count == 0) return healed;
        var lowest = candidates.Min(id => State.GetUnit(id).Hp);
        var tied = candidates.Where(id => State.GetUnit(id).Hp == lowest).ToList();
        var chosen = tied.Count == 1 ? tied[0] : tied[State.Random.NextInt(tied.Count)];
        State.Heal(UserId, chosen, overflow);
        return healed;
    }

    /// <summary>
    /// Brings a KO body back (Revive, Life for Life) with the given HP, or, for an undead, restores the original creature. The body keeps
    /// its slot; the revived unit is scheduled as though the revival had been its own action (the skill's Delay). Then settle.
    /// </summary>
    public void ReviveOrRestore(int targetId, int hp)
    {
        if (State.GetUnit(targetId).IsUndead) Necromancy.Restore(State, targetId, hp, _skill.Delay);
        else Knockout.Revive(State, targetId, hp, _skill.Delay);
        Settle();
    }

    /// <summary>
    /// A non-damaging effect aimed at a unit (a status, a push). If the skill is `melee` or `projectile` it can be intercepted:
    /// an effect that cannot be split goes entirely to the first interceptor (section 8.2). Returns the unit that received it.
    /// The event then settles (KO check, reactions, victory); Momentum does not move for an action without a damaging hit.
    /// </summary>
    public int ApplyStatus(int targetId, StatusApplication template)
    {
        var receiver = InterceptEffect(targetId);
        if (!State.GetUnit(receiver).IsKnockedOut)
            Statuses.Apply(State, template with { UnitId = receiver, SourceUnitId = UserId });
        EndOfEvent(Array.Empty<HitOutcome>(), Array.Empty<Trigger>(), shiftMomentum: false);
        return receiver;
    }

    /// <summary>A non-damaging effect aimed at a unit goes entirely to the first interceptor if the skill is `melee` or `projectile` and someone intercepts. Returns who gets it.</summary>
    public int InterceptEffect(int targetId)
    {
        if (_skill.Delivery is { } d && Interception.CanBeIntercepted(d, IgnoresInterception))
        {
            var r = Interception.Resolve(State, targetId, 1, splittable: false);
            if (r.Intercepted) return r.Shares[0].InterceptorId;
        }
        return targetId;
    }

    /// <summary>
    /// Settles what a skill changed without hitting anyone (a move, a placement, a reveal): KO check, reactions to any KO and the victory check.
    /// </summary>
    public void Settle() => EndOfEvent(Array.Empty<HitOutcome>(), Array.Empty<Trigger>(), shiftMomentum: false);

    /// <summary>Raises the "when hit" reactions for units that took bump damage in a blocked forced move, then settles.</summary>
    public void AfterForcedMove(ForcedMoveResult result)
    {
        var triggers = result.BumpDamaged.Select(id => new Trigger(TriggerKind.WhenHit, id, null, EffectTags.None))
            .Concat(result.Moves.Select(m => new Trigger(TriggerKind.WhenMoved, m.UnitId, UserId))).ToArray();
        EndOfEvent(Array.Empty<HitOutcome>(), triggers, shiftMomentum: false);
    }
}
