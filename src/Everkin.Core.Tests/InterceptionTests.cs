using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Tests;

internal static class Place
{
    private static readonly CreatureDef Def = new("t", "t", "brawl", new[] { "warrior" }, "t.png", null, "ruins", 40, 5, 5, 1,
        new[] { 0, 0, 1, 1 }, new[] { 0, 0, 1, 1 }, "reviewed", "");

    public static int Unit(BattleState state, Side side, Row row)
    {
        var unit = state.AddUnit(Def, side);
        state.Formation.Place(unit.Id, side, row);
        return unit.Id;
    }
}

public class InterceptionTests
{
    // ---- who can be intercepted, who can intercept ----

    [Theory]
    [InlineData(Delivery.Melee, false, true)]
    [InlineData(Delivery.Projectile, false, true)]
    [InlineData(Delivery.Direct, false, false)]
    [InlineData(Delivery.Melee, true, false)]       // Phasing Strike
    [InlineData(Delivery.Projectile, true, false)]
    public void OnlyMeleeAndProjectileCanBeIntercepted(Delivery delivery, bool ignores, bool expected) =>
        Assert.Equal(expected, Interception.CanBeIntercepted(delivery, ignores));

    [Theory]
    [InlineData(100, 50)]
    [InlineData(50, 25)]
    [InlineData(0, 0)]
    public void ChanceFollowsTheOverlap(int overlap, int chance) => Assert.Equal(chance, Interception.ChanceFor(overlap));

    [Fact]
    public void NobodyCoversAFrontUnit()
    {
        var s = new BattleState(1);
        var target = Place.Unit(s, Side.A, Row.Front);
        Place.Unit(s, Side.A, Row.Front);
        Assert.Empty(Interception.Defenders(s, target));
        var r = Interception.Resolve(s, target, 8);
        Assert.Empty(r.Shares);
        Assert.Equal(8, r.Remaining);
    }

    [Fact]
    public void ForwardUnitsWithOverlapCoverAndOthersDoNot()
    {
        var s = new BattleState(1);
        var f1 = Place.Unit(s, Side.A, Row.Front);
        var f2 = Place.Unit(s, Side.A, Row.Front);
        var f3 = Place.Unit(s, Side.A, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Middle);
        // Front centers -2, 0, 2; the lone Middle unit is at 0: only f2 overlaps (100%).
        Assert.Equal(new[] { f2 }, Interception.Defenders(s, target));
        Assert.DoesNotContain(f1, Interception.Defenders(s, target));
        Assert.DoesNotContain(f3, Interception.Defenders(s, target));
    }

    [Fact]
    public void DefendersAreListedFromTheNearestRowToTheTarget()
    {
        var s = new BattleState(1);
        var middle = Place.Unit(s, Side.A, Row.Middle);
        var front = Place.Unit(s, Side.A, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Rear);
        Assert.Equal(new[] { front, middle }, Interception.Defenders(s, target));
    }

    [Fact]
    public void KoStealthedAndBlockedUnitsDoNotIntercept()
    {
        var s = new BattleState(1);
        var ko = Place.Unit(s, Side.A, Row.Front);
        var stealthed = Place.Unit(s, Side.A, Row.Front);
        var stunned = Place.Unit(s, Side.A, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Middle);
        s.GetUnit(ko).Hp = 0;
        s.GetUnit(stealthed).IsStealthed = true;
        s.GetUnit(stunned).InterceptionBlocked = true;
        Assert.Empty(Interception.Defenders(s, target));
    }

    [Fact]
    public void AnEnemyNeverCoversATarget()
    {
        var s = new BattleState(1);
        Place.Unit(s, Side.B, Row.Front);
        var target = Place.Unit(s, Side.A, Row.Middle);
        Assert.Empty(Interception.Defenders(s, target));
    }

    // ---- the roll ----

    private static BattleState OneDefender(ulong seed, out int defender, out int target)
    {
        var s = new BattleState(seed);
        defender = Place.Unit(s, Side.A, Row.Front);
        target = Place.Unit(s, Side.A, Row.Middle); // single cards: 100% overlap
        return s;
    }

    [Fact]
    public void AFullOverlapDefenderInterceptsHalfTheTimeAndTakesAllOrHalfWithEqualChance()
    {
        int none = 0, full = 0, partial = 0;
        for (ulong seed = 1; seed <= 8000; seed++)
        {
            var s = OneDefender(seed, out _, out var target);
            var r = Interception.Resolve(s, target, 8);
            if (!r.Intercepted) none++;
            else if (r.FullyIntercepted) full++;
            else partial++;
        }
        Assert.InRange(none, 3700, 4300);     // 50%
        Assert.InRange(full, 1700, 2300);     // 25%
        Assert.InRange(partial, 1700, 2300);  // 25%
    }

    [Fact]
    public void AHalfOverlapDefenderInterceptsAQuarterOfTheTime()
    {
        // Front two cards (centers -1 and 1), the Middle target alone at center 0: each Front card overlaps it by 50%.
        var intercepted = 0;
        for (ulong seed = 1; seed <= 8000; seed++)
        {
            var s = new BattleState(seed);
            Place.Unit(s, Side.A, Row.Front);
            var only = Place.Unit(s, Side.A, Row.Front);
            var t = Place.Unit(s, Side.A, Row.Middle);
            // Both Front cards overlap the lone Middle card by 50%: each has a 25% chance; skip the other one.
            s.GetUnit(only).IsStealthed = true;
            if (Interception.Resolve(s, t, 8).Intercepted) intercepted++;
        }
        Assert.InRange(intercepted, 1800, 2200); // 25%
    }

    [Fact]
    public void ASevenDamageHitSplitsIntoThreeAndFourWithFourContinuing()
    {
        // 20.5: an interceptor takes a partial hit from 7: it takes 3 and 4 continues down the chain.
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = OneDefender(seed, out var d, out var t);
            var r = Interception.Resolve(s, t, 7);
            if (r.Intercepted && !r.FullyIntercepted)
            {
                Assert.Equal(new InterceptedShare(d, 3, false), r.Shares[0]);
                Assert.Equal(4, r.Remaining);
                return;
            }
        }
        Assert.Fail("no partial interception in 200 seeds");
    }

    [Fact]
    public void AHitOfOneCannotBeSplitAndGoesEntirelyToTheInterceptor()
    {
        var seen = 0;
        for (ulong seed = 1; seed <= 400; seed++)
        {
            var s = OneDefender(seed, out var d, out var t);
            var r = Interception.Resolve(s, t, 1);
            if (!r.Intercepted) continue;
            seen++;
            Assert.Equal(new InterceptedShare(d, 1, true), r.Shares[0]);
            Assert.Equal(0, r.Remaining);
        }
        Assert.True(seen > 100);
    }

    [Fact]
    public void AnEffectThatCannotBeSplitGoesEntirelyToTheInterceptor()
    {
        // A projectile status: the interceptor gets the status, the target does not. No full/partial roll.
        var seen = 0;
        for (ulong seed = 1; seed <= 400; seed++)
        {
            var s = OneDefender(seed, out var d, out var t);
            var r = Interception.Resolve(s, t, 5, splittable: false);
            if (!r.Intercepted) continue;
            seen++;
            Assert.True(r.FullyIntercepted);
            Assert.Equal(d, r.Shares[0].InterceptorId);
        }
        Assert.True(seen > 100);
    }

    [Fact]
    public void ASplittableHealIsSplitLikeDamage()
    {
        // 20.5: a projectile heal of 9 partly intercepted: the interceptor is healed by 4 and the target by 5.
        for (ulong seed = 1; seed <= 200; seed++)
        {
            var s = OneDefender(seed, out _, out var t);
            var r = Interception.Resolve(s, t, 9);
            if (r.Intercepted && !r.FullyIntercepted)
            {
                Assert.Equal(4, r.Shares[0].Amount);
                Assert.Equal(5, r.Remaining);
                return;
            }
        }
        Assert.Fail("no partial interception in 200 seeds");
    }

    [Fact]
    public void ARemainderCanBeInterceptedAgainByAFurtherDefender()
    {
        // 20.5: partial interception with another eligible defender behind: the remainder may undergo further defense.
        for (ulong seed = 1; seed <= 2000; seed++)
        {
            var s = new BattleState(seed);
            var front = Place.Unit(s, Side.A, Row.Front);
            var middle = Place.Unit(s, Side.A, Row.Middle);
            var target = Place.Unit(s, Side.A, Row.Rear);
            var r = Interception.Resolve(s, target, 8);
            if (r.Shares.Count == 2)
            {
                Assert.Equal(front, r.Shares[0].InterceptorId);   // the nearer row goes first
                Assert.Equal(middle, r.Shares[1].InterceptorId);
                Assert.False(r.Shares[0].Full);
                Assert.Equal(4, r.Shares[0].Amount);
                Assert.Equal(r.Shares[1].Full ? 4 : 2, r.Shares[1].Amount);
                Assert.Equal(8 - r.Shares.Sum(x => x.Amount), r.Remaining);
                return;
            }
        }
        Assert.Fail("no double interception in 2000 seeds");
    }

    [Fact]
    public void WhenTwoUnitsHalfCoverTheTargetEachGetsToRollFirstWithEqualChance()
    {
        var first = new int[2];
        for (ulong seed = 1; seed <= 6000; seed++)
        {
            var s = new BattleState(seed);
            var a = Place.Unit(s, Side.A, Row.Front);
            Place.Unit(s, Side.A, Row.Front);
            var t = Place.Unit(s, Side.A, Row.Middle);
            var r = Interception.Resolve(s, t, 8);
            if (r.Intercepted) first[r.Shares[0].InterceptorId == a ? 0 : 1]++;
        }
        Assert.InRange(first[0], 0.45 * first.Sum(), 0.55 * first.Sum());
    }

    [Fact]
    public void ResolveReportsEveryInterceptionAndIsReproducible()
    {
        InterceptionResult Run(ulong seed, out List<BattleEvent> events)
        {
            var s = OneDefender(seed, out _, out var t);
            s.DrainEvents();
            var r = Interception.Resolve(s, t, 8);
            events = s.DrainEvents().ToList();
            return r;
        }

        for (ulong seed = 1; seed <= 100; seed++)
        {
            var a = Run(seed, out var ea);
            var b = Run(seed, out var eb);
            Assert.Equal(a.Remaining, b.Remaining);
            Assert.Equal(a.Shares, b.Shares);
            Assert.Equal(ea, eb);
            Assert.Equal(a.Shares.Count, ea.OfType<Intercepted>().Count());
        }
    }

    [Fact]
    public void ANothingToInterceptEffectSkipsTheRolls()
    {
        var s = OneDefender(1, out _, out var t);
        var before = s.Random.Clone();
        var r = Interception.Resolve(s, t, 0);
        Assert.False(r.Intercepted);
        Assert.Equal(before.NextUInt64(), s.Random.NextUInt64());
    }
}
