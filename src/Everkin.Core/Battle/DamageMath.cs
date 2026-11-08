namespace Everkin.Core.Battle;

/// <summary>An inclusive whole-number range such as "hit for 4–5" or "heal 8–10".</summary>
public readonly record struct IntRange(int Min, int Max)
{
    public static IntRange Of(int value) => new(value, value);

    public bool IsValid => Min <= Max;
}

/// <summary>Where damage comes from, which decides what protects against it (game_system.md sections 5.2, 9.5, 10.5).</summary>
public enum DamageSource
{
    /// <summary>A hit, a counter, a turret shot, burning ground: shields absorb it first, then reductions apply.</summary>
    Hit,

    /// <summary>Poison and Bleeding: shields do not absorb them, but reductions such as Dampen apply.</summary>
    Periodic,

    /// <summary>Bump damage: fixed and unreducible, neither shields nor reductions apply.</summary>
    Unreducible,
}

/// <summary>What one piece of damage did, in the order of section 5.2: shield, then reductions, then HP.</summary>
/// <param name="Incoming">The damage that arrived (the roll).</param>
/// <param name="AbsorbedByWard">How much the Ward took.</param>
/// <param name="HpLost">What reached HP.</param>
/// <param name="WardLeft">The Ward after the hit.</param>
public readonly record struct DamageResult(int Incoming, int AbsorbedByWard, int HpLost, int WardLeft)
{
    /// <summary>A shield took the whole hit: nothing reached HP. Momentum does not move (section 6.7).</summary>
    public bool FullyAbsorbed => AbsorbedByWard > 0 && HpLost == 0;
}

/// <summary>Integer rules math for damage, healing and shields (game_system.md section 5.2). No floating point.</summary>
public static class DamageMath
{
    /// <summary>
    /// skills.md numbers are written for Power 5. A number is multiplied by Power ÷ 5, rounded down, minimum 1.
    /// A Power 7 Ward of 4 absorbs 5 (28 ÷ 5 = 5.6 → 5).
    /// </summary>
    public static int Scale(int valueAtPower5, int power) => Math.Max(1, valueAtPower5 * power / 5);

    /// <summary>A range scales at both ends (4–5 at Power 3 is 2–3).</summary>
    public static IntRange ScaleRange(IntRange rangeAtPower5, int power) =>
        new(Scale(rangeAtPower5.Min, power), Scale(rangeAtPower5.Max, power));

    /// <summary>A uniform whole-number roll inside the range, both ends included.</summary>
    public static int Roll(BattleRandom random, IntRange range)
    {
        if (!range.IsValid) throw new ArgumentOutOfRangeException(nameof(range));
        return random.NextRange(range.Min, range.Max);
    }

    /// <summary>
    /// Resolves one piece of damage against a Ward and Dampen (section 5.2 order): the Ward takes damage first, then
    /// Dampen halves the remainder (rounded down); if anything passed the shield the result is at least 1.
    /// </summary>
    public static DamageResult Resolve(int incoming, int ward, bool dampen, DamageSource source = DamageSource.Hit)
    {
        if (incoming < 0) throw new ArgumentOutOfRangeException(nameof(incoming));
        if (ward < 0) throw new ArgumentOutOfRangeException(nameof(ward));
        if (incoming == 0) return new DamageResult(0, 0, 0, ward);

        var absorbed = 0;
        var remainder = incoming;
        if (source == DamageSource.Hit)
        {
            absorbed = Math.Min(incoming, ward);
            remainder -= absorbed;
        }
        if (remainder > 0 && dampen && source != DamageSource.Unreducible)
            remainder = Math.Max(1, remainder / 2);
        return new DamageResult(incoming, absorbed, remainder, ward - absorbed);
    }

    /// <summary>
    /// A unit has at most one Ward. A new Ward replaces the old one only if it absorbs at least as much as the old one
    /// has left; otherwise the old one stays (section 5.2).
    /// </summary>
    public static int CombineWard(int currentWardLeft, int newWard) => newWard >= currentWardLeft ? newWard : currentWardLeft;

    /// <summary>Healing is capped at maximum HP: there is no overheal (section 10.3). Returns the HP after healing.</summary>
    public static int HealedHp(int hp, int maxHp, int amount) => Math.Min(maxHp, hp + amount);
}
