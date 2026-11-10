namespace Everkin.Core.Battle;

/// <summary>
/// The one seeded random generator of a battle (production.md section 2.1, 2.2).
/// xoshiro256** seeded through SplitMix64, written out here so that a seed gives the same
/// numbers on every platform and .NET version. Integers only. The AI search never draws from it.
/// </summary>
public sealed class BattleRandom
{
    /// <summary>How the generator answers. Only preview queries on a clone change it from <see cref="RollMode.Normal"/>.</summary>
    public enum RollMode
    {
        Normal,

        /// <summary>Every roll is its lowest possible value and every chance fails (no miss, no interception): the low end of a preview.</summary>
        Lowest,

        /// <summary>Every roll is its highest possible value and every chance fails: the high end of a preview.</summary>
        Highest,

        /// <summary>Every roll is the middle of its range and a chance succeeds when it is 50% or more: the "average values" the AI search uses (game_system section 2.5).</summary>
        Average,
    }

    /// <summary>Preview queries set this on a clone; battles never do. It does not change the sequence of <see cref="RollMode.Normal"/>.</summary>
    public RollMode Mode { get; set; }

    private ulong _s0, _s1, _s2, _s3;

    public BattleRandom(ulong seed)
    {
        Seed = seed;
        var x = seed;
        _s0 = SplitMix64(ref x);
        _s1 = SplitMix64(ref x);
        _s2 = SplitMix64(ref x);
        _s3 = SplitMix64(ref x);
    }

    private BattleRandom(BattleRandom other)
    {
        Seed = other.Seed;
        _s0 = other._s0;
        _s1 = other._s1;
        _s2 = other._s2;
        _s3 = other._s3;
        Mode = other.Mode;
    }

    /// <summary>The seed this generator started from (battles are replayed from it).</summary>
    public ulong Seed { get; }

    /// <summary>An independent copy that continues with exactly the same numbers.</summary>
    public BattleRandom Clone() => new(this);

    public ulong NextUInt64()
    {
        var result = RotateLeft(_s1 * 5, 7) * 9;
        var t = _s1 << 17;
        _s2 ^= _s0;
        _s3 ^= _s1;
        _s1 ^= _s2;
        _s0 ^= _s3;
        _s2 ^= t;
        _s3 = RotateLeft(_s3, 45);
        return result;
    }

    /// <summary>A uniform whole number from 0 to <paramref name="maxExclusive"/> - 1, without modulo bias.</summary>
    public int NextInt(int maxExclusive)
    {
        if (maxExclusive <= 0) throw new ArgumentOutOfRangeException(nameof(maxExclusive));
        if (Mode == RollMode.Lowest) return 0;
        if (Mode == RollMode.Highest) return maxExclusive - 1;
        if (Mode == RollMode.Average) return (maxExclusive - 1) / 2;
        var n = (ulong)maxExclusive;
        var threshold = (0UL - n) % n;
        ulong r;
        do { r = NextUInt64(); } while (r < threshold);
        return (int)(r % n);
    }

    /// <summary>A uniform whole number from <paramref name="min"/> to <paramref name="max"/>, both included (damage and healing rolls, section 5.2).</summary>
    public int NextRange(int min, int max)
    {
        if (max < min) throw new ArgumentOutOfRangeException(nameof(max));
        return min + NextInt(max - min + 1);
    }

    /// <summary>True with the given whole-number percentage chance (0 never, 100 always).</summary>
    public bool Chance(int percent)
    {
        if (percent <= 0) return false;
        if (percent >= 100) return true;
        if (Mode == RollMode.Average) return percent >= 50;
        if (Mode != RollMode.Normal) return false;
        return NextInt(100) < percent;
    }

    private static ulong RotateLeft(ulong x, int k) => (x << k) | (x >> (64 - k));

    private static ulong SplitMix64(ref ulong x)
    {
        x += 0x9E3779B97F4A7C15UL;
        var z = x;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }
}
