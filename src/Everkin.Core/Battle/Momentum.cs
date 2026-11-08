namespace Everkin.Core.Battle;

/// <summary>What happened to one hit of a damaging attack, as far as Momentum cares (game_system.md section 6.7).</summary>
public enum HitOutcome
{
    /// <summary>The hit landed on a unit of the other side and something got through (a partly absorbed hit counts).</summary>
    Landed,

    /// <summary>The attack missed (a chance to hit failed, for example a Blind unit's attack).</summary>
    Missed,

    /// <summary>The hit was intercepted.</summary>
    Intercepted,

    /// <summary>A shield absorbed the whole hit: nothing reached HP.</summary>
    FullyAbsorbed,

    /// <summary>The target belongs to the acting side (friendly fire, or a skill used on an ally).</summary>
    OnActingSide,
}

public enum MomentumCause
{
    /// <summary>A skill's Momentum cost was paid.</summary>
    Cost,

    /// <summary>The meter shifted because of what happened in the battle.</summary>
    Shift,
}

/// <summary>The meter changed. Values are on the meter itself: positive favors Side A, negative Side B.</summary>
public sealed record MomentumChanged(MomentumCause Cause, Side Actor, int Before, int After) : BattleEvent;

/// <summary>
/// The single battle-wide contested Momentum meter (game_system.md section 6.7). It runs from −10 to +10 and starts at
/// neutral zero. A positive value favors Side A, a negative one Side B; <see cref="AdvantageOf"/> gives either side's view.
/// Integers only.
/// </summary>
public sealed class Momentum
{
    public const int Limit = 10;
    public const int MaxCost = 10;

    public Momentum() { }

    private Momentum(Momentum other) => Value = other.Value;

    public Momentum Clone() => new(this);

    /// <summary>The meter: −10 to +10, positive favors Side A.</summary>
    public int Value { get; private set; }

    /// <summary>How far ahead a side is: positive when the flow of battle is in its favor.</summary>
    public int AdvantageOf(Side side) => side == Side.A ? Value : -Value;

    /// <summary>
    /// A skill with Momentum cost N needs its side to be at least +N ahead (section 6.7). Costs run from 0 to 10.
    /// </summary>
    public bool CanPay(Side side, int cost) =>
        cost is >= 0 and <= MaxCost && AdvantageOf(side) >= cost;

    /// <summary>
    /// Pays a skill's cost: the meter moves N toward neutral from the side's lead. Paid before any effect and never
    /// refunded. Throws if the side is not far enough ahead.
    /// </summary>
    public void Pay(Side side, int cost)
    {
        if (cost is < 0 or > MaxCost) throw new ArgumentOutOfRangeException(nameof(cost), "A Momentum cost is 0 to 10.");
        if (!CanPay(side, cost)) throw new InvalidOperationException($"Side {side} is not {cost} ahead.");
        Value += side == Side.A ? -cost : cost;
    }

    /// <summary>
    /// Moves the meter by a whole number of points toward <paramref name="side"/> (negative: against it). The meter clamps
    /// at −10 and +10 and any excess is lost.
    /// </summary>
    public void Shift(Side side, int amount)
    {
        var toward = side == Side.A ? amount : -amount;
        Value = Math.Clamp(Value + toward, -Limit, Limit);
    }
}

/// <summary>How much one hit or one action moves the meter, from the acting side's point of view (section 6.7).</summary>
public static class MomentumRules
{
    /// <summary>
    /// One hit: a damaging hit that lands moves it 1 toward the actor, a miss 1 against, and everything else 0:
    /// intercepted, fully absorbed by a shield, or on the acting side itself.
    /// </summary>
    public static int HitShift(HitOutcome outcome) => outcome switch
    {
        HitOutcome.Landed => 1,
        HitOutcome.Missed => -1,
        _ => 0,
    };

    /// <summary>
    /// A whole action causes one shift by default: 1 toward its side if any hit lands, otherwise 1 against if it missed,
    /// and 0 if everything was intercepted, absorbed or on the acting side. An action with no damaging hit (a Mark, a
    /// Silence, Move, Skip Turn, healing) moves it 0. A skill may state otherwise.
    /// </summary>
    public static int ActionShift(IEnumerable<HitOutcome> hits)
    {
        var any = false;
        var missed = false;
        foreach (var h in hits)
        {
            any = true;
            if (h == HitOutcome.Landed) return 1;
            if (h == HitOutcome.Missed) missed = true;
        }
        return any && missed ? -1 : 0;
    }
}
