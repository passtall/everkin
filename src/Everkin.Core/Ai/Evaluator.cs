using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Ai;

/// <summary>What the score needs to know besides the position: the skills (which statuses can still do something, what a summon does) and the statuses already there when the search started.</summary>
public sealed class EvalContext
{
    public EvalContext(SkillCatalog catalog, BattleState root)
    {
        Catalog = catalog;
        foreach (var u in root.Units)
            foreach (var s in u.StatusInstances.Values)
                Baseline[(u.Id, s.Id, s.SourceUnitId)] = s.EndsAt;
        ReferenceDamagePerTurn = ReferenceFor(catalog);
    }

    public SkillCatalog Catalog { get; }

    /// <summary>Statuses already on a unit at the root of the search (unit, status, source) with the time they end. Renewing one scores nothing: it never counts beyond that end.</summary>
    public Dictionary<(int Unit, string Status, int? Source), int?> Baseline { get; } = new();

    /// <summary>What a Power 5 creature's unique attack does per time unit on average (twice the damage per 1000 time units), the yardstick for a summon.</summary>
    public long ReferenceDamagePerTurn { get; }

    private static readonly Dictionary<SkillCatalog, long> References = new();

    private static long ReferenceFor(SkillCatalog catalog)
    {
        lock (References)
        {
            if (References.TryGetValue(catalog, out var cached)) return cached;
            var attacks = catalog.Content.Attacks;
            var value = attacks.Count == 0 ? 1 : Math.Max(1, (long)attacks.Average(a => (a.Min + a.Max) * 1000.0 / Timeline.Wait(a.Delay, 5)));
            References[catalog] = value;
            return value;
        }
    }
}

/// <summary>
/// The position score (game_system section 2.5 "Scoring"). Integers: 10000 is a whole team's HP. Positive favours Side A. Parts: HP share (every living
/// unit as if it had 25% more); Momentum with diminishing value; tempo (how soon each unit's next turn comes against its normal Wait, scaled by Power);
/// statuses with a weight each, counted only while they can still do something and never for a renewal; summons by their damage per turn with each
/// further summon of a caster counting 20% less; undead at half; totems and traps by what they do in the next 20 time units, other constructs nothing.
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

    /// <summary>The next 20 time units: the time a totem or trap is scored over.</summary>
    private const int Horizon = 20;

    public static int Score(BattleState state, AiLevel level, EvalContext? context = null)
    {
        if (state.IsOver)
            return state.Outcome switch { BattleOutcome.SideAWins => Win, BattleOutcome.SideBWins => -Win, _ => 0 };
        return SideScore(state, Side.A, level, context) - SideScore(state, Side.B, level, context) + MomentumValue(state.Momentum.Value, level);
    }

    /// <summary>The meter's worth for Side A (negative when behind): the first points of a lead full, the next half, the cap nothing.</summary>
    public static int MomentumValue(int meter, AiLevel level)
    {
        var lead = Math.Abs(meter);
        var full = Math.Min(lead, level.MomentumFullPoints);
        var half = Math.Clamp(lead - level.MomentumFullPoints, 0, Math.Max(0, level.MomentumHalfPoints - level.MomentumFullPoints));
        var value = full * level.MomentumPoint + half * level.MomentumPoint / 2;
        return meter < 0 ? -value : value;
    }

    private static int SideScore(BattleState state, Side side, AiLevel level, EvalContext? context)
    {
        long num = 0, den = 0, share = 0, statuses = 0;
        var summonRank = new Dictionary<int, int>();
        foreach (var u in state.Units.OrderBy(u => u.Id))
        {
            if (u.IsRemoved) continue;
            var original = u.Kind == UnitKind.Creature && !u.IsUndead;
            if (original && u.OriginalSide == side) den += 5L * u.MaxHp * 100;
            if (u.Side != side || u.IsKnockedOut) continue;

            if (u.Kind == UnitKind.Construct)                                  // a construct's statuses count nothing either
            {
                num += ConstructValue(state, u, level, ref share);
                continue;
            }
            var weight = original ? 100 : UnitWeight(state, u, level, context, summonRank);
            num += weight * (4L * u.Hp + u.MaxHp);
            share += Tempo(state, u, weight, level);
            foreach (var s in u.StatusInstances.Values)
                if (StatusRules.PolarityOf(s.Id) == Polarity.Helpful) statuses += Remaining(state, u, s, context) * level.WeightOf(s.Id);
        }
        // Harmful statuses on the other side's units count for this side.
        foreach (var u in state.Units)
            if (!u.IsRemoved && !u.IsKnockedOut && u.Side != side && u.Kind != UnitKind.Construct)
                foreach (var s in u.StatusInstances.Values)
                    if (StatusRules.PolarityOf(s.Id) == Polarity.Harmful) statuses += Remaining(state, u, s, context) * level.WeightOf(s.Id);
        var hp = den == 0 ? 0 : (int)(num * 10000 / den);
        return hp + (int)share + (int)(statuses / 20);
    }

    /// <summary>A summon's or undead's weight in percent: undead half (the level's setting), a summon by its damage per turn against a creature's unique attack, 20% less for each further summon of the same caster.</summary>
    private static long UnitWeight(BattleState state, Unit u, AiLevel level, EvalContext? context, Dictionary<int, int> rank)
    {
        if (u.IsUndead || context is null || u.AutoSkillId is null || u.SummonerId is null) return level.SummonPercent;
        var power = state.PowerOf(u.Id);
        long avg2 = 0;
        var delay = 3;
        if (context.Catalog.TryGet(u.AutoSkillId, out var skill))
        {
            delay = skill.Delay;
            avg2 = skill switch
            {
                SummonStrike strike => strike.Range.Min + strike.Range.Max,
                UniqueAttackSkill attack => attack.Def.Min + attack.Def.Max,
                _ => 0,
            };
        }
        var perTurn = avg2 * power / 5 * 1000 / Timeline.Wait(delay, u.EffectiveSpeed);
        var byDamage = Math.Min(100, perTurn * 100 / context.ReferenceDamagePerTurn);
        var caster = u.SummonerId.Value;
        var k = rank.GetValueOrDefault(caster);
        rank[caster] = k + 1;
        return byDamage * Math.Max(20, 100 - 20 * k) / 100;
    }

    /// <summary>How soon the unit's next turn comes compared with its normal Wait, in score units: sooner counts for its side, later against, at most one normal Wait either way.</summary>
    private static long Tempo(BattleState state, Unit u, long weight, AiLevel level)
    {
        var entry = state.Timeline.TurnOf(u.Id);
        if (entry is null) return 0;
        var speed = u.EffectiveSpeed;
        var normal = Timeline.Wait(Timeline.SkippedTurnDelay, speed);             // a normal Wait: the Delay of a Move
        var remaining = state.Timeline.Remaining(entry);
        if (Statuses.Has(u, StatusIds.Stunned) || Statuses.Has(u, StatusIds.Frozen)) remaining += normal;     // that turn is skipped: the next one comes a Wait later
        var diff = Math.Clamp(normal - remaining, -normal, normal);
        return diff * level.TempoPerTime * state.PowerOf(u.Id) / 5 * weight / 100;
    }

    /// <summary>
    /// A totem or trap counts what it does over the next 20 time units: the Healing Totem's heals on the allies in its row (as HP), the Net Trap's
    /// Delay (as tempo). Spirit Link, Totem of Return, turrets and barricades give nothing numbered, so they count nothing. Returns HP-score units
    /// (like weight × (4 HP + max)); tempo goes to <paramref name="share"/>.
    /// </summary>
    private static long ConstructValue(BattleState state, Unit u, AiLevel level, ref long share)
    {
        switch (u.CreatureId)
        {
            case TotemistSkills.HealingTotemId:
                var amount = DamageMath.Scale(TotemistSkills.PulseHeal, state.PowerOf(u.Id));
                var pulses = Horizon / TotemistSkills.PulseEvery;
                long healed = 0;
                foreach (var ally in state.Units)
                    if (!ally.IsKnockedOut && !ally.IsRemoved && ally.Kind != UnitKind.Construct && !ally.HealingHurts && TotemistSkills.Covers(state, u, ally.Id))
                        healed += Math.Min(ally.MaxHp - ally.Hp, amount * pulses);
                return 100 * 4 * healed;
            case EngineerSkills.NetTrapId:
                share += (long)EngineerSkills.TrapDelay * level.TempoPerTime;                 // one hitter delayed, at a Power 5 unit's worth
                return 0;
            default:
                return 0;
        }
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
            case StatusIds.Silenced:                                            // only a unit with a magical skill or attack
                return carrier.KnownSkills.Any(id => catalog.TryGet(id, out var k) && !k.IsPassive && k.Magical);
            case StatusIds.Bleeding:                                            // only a unit with a melee attack or skill
                return carrier.KnownSkills.Any(id => catalog.TryGet(id, out var k) && !k.IsPassive && k.Tags.HasFlag(EffectTags.Melee));
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
