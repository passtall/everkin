using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Tests;

public class BattleRandomTests
{
    [Fact]
    public void SameSeedGivesTheSameNumbers()
    {
        var a = new BattleRandom(42);
        var b = new BattleRandom(42);
        for (var i = 0; i < 1000; i++) Assert.Equal(a.NextUInt64(), b.NextUInt64());
    }

    [Fact]
    public void DifferentSeedsGiveDifferentNumbers()
    {
        Assert.NotEqual(new BattleRandom(1).NextUInt64(), new BattleRandom(2).NextUInt64());
    }

    [Fact]
    public void SequenceIsPinnedForReplays()
    {
        // Values from an independent Python implementation of xoshiro256** seeded by SplitMix64.
        // If these change, old seeds no longer replay: do not "fix" the test.
        var r = new BattleRandom(1);
        Assert.Equal(12966619160104079557UL, r.NextUInt64());
        Assert.Equal(9600361134598540522UL, r.NextUInt64());
        Assert.Equal(10590380919521690900UL, r.NextUInt64());

        var r10000 = new BattleRandom(10000);
        Assert.Equal(17335105848851343585UL, r10000.NextUInt64());
        Assert.Equal(17403182622469942189UL, r10000.NextUInt64());

        var ints = new BattleRandom(1);
        var rolled = Enumerable.Range(0, 8).Select(_ => ints.NextInt(10)).ToArray();
        Assert.Equal(new[] { 7, 2, 0, 3, 1, 2, 6, 9 }, rolled);
    }

    [Fact]
    public void CloneContinuesIndependentlyWithTheSameNumbers()
    {
        var a = new BattleRandom(7);
        a.NextUInt64();
        var b = a.Clone();
        Assert.Equal(a.Seed, b.Seed);
        var fromA = Enumerable.Range(0, 5).Select(_ => a.NextUInt64()).ToArray();
        var fromB = Enumerable.Range(0, 5).Select(_ => b.NextUInt64()).ToArray();
        Assert.Equal(fromA, fromB);
    }

    [Fact]
    public void NextRangeIncludesBothEndsAndStaysInside()
    {
        var r = new BattleRandom(3);
        var counts = new int[6];
        for (var i = 0; i < 60000; i++)
        {
            var v = r.NextRange(1, 6);
            Assert.InRange(v, 1, 6);
            counts[v - 1]++;
        }
        // Uniform: each face about 10000 times.
        Assert.All(counts, c => Assert.InRange(c, 9500, 10500));
    }

    [Fact]
    public void SingleValueRangeAlwaysReturnsIt() => Assert.Equal(5, new BattleRandom(9).NextRange(5, 5));

    [Fact]
    public void ChanceHandlesTheEdgesAndAveragesOut()
    {
        var r = new BattleRandom(11);
        Assert.False(r.Chance(0));
        Assert.True(r.Chance(100));
        var hits = Enumerable.Range(0, 100000).Count(_ => r.Chance(30));
        Assert.InRange(hits, 29000, 31000);
    }

    [Fact]
    public void InvalidArgumentsThrow()
    {
        var r = new BattleRandom(1);
        Assert.Throws<ArgumentOutOfRangeException>(() => r.NextInt(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => r.NextRange(3, 2));
    }
}

public class BattleStateTests
{
    private static CreatureDef Creature(string id = "wolf-ranger", int hp = 50, int speed = 5, int power = 5) =>
        new(id, "Ranger Wolf", "ranged", new[] { "hunter" }, "wolf-01.png", null, "pine_forest",
            hp, speed, power, 6, new[] { 0, 0, 640, 960 }, new[] { 0, 0, 1024, 1536 }, "reviewed", "");

    [Fact]
    public void NewBattleReportsItsStartAsAnEvent()
    {
        var state = new BattleState(5);
        var events = state.DrainEvents();
        Assert.Equal(new BattleEvent[] { new BattleStarted(5) }, events);
        Assert.Empty(state.DrainEvents());
    }

    [Fact]
    public void AddUnitCopiesTheCreatureStatsAtFullHp()
    {
        var state = new BattleState(1);
        var u = state.AddUnit(Creature(hp: 65, speed: 3, power: 6), Side.B);
        Assert.Equal(0, u.Id);
        Assert.Equal(Side.B, u.Side);
        Assert.Equal(65, u.Hp);
        Assert.Equal(65, u.MaxHp);
        Assert.Equal(3, u.Speed);
        Assert.Equal(6, u.Power);
        Assert.Equal("wolf-ranger", u.CreatureId);
        Assert.Same(u, state.GetUnit(0));
    }

    [Fact]
    public void UnitIdsFollowTheOrderUnitsWereAdded()
    {
        var state = new BattleState(1);
        var ids = new[] { Side.A, Side.B, Side.A }.Select(s => state.AddUnit(Creature(), s).Id).ToArray();
        Assert.Equal(new[] { 0, 1, 2 }, ids);
    }

    [Fact]
    public void CloneIsIndependentOfTheOriginal()
    {
        var state = new BattleState(1);
        var u = state.AddUnit(Creature(), Side.A);
        var copy = state.Clone();

        copy.GetUnit(0).Hp -= 10;
        copy.Random.NextUInt64();
        copy.AddUnit(Creature("wolf-two"), Side.B);

        Assert.Equal(50, u.Hp);
        Assert.Single(state.Units);
        Assert.Equal(40, copy.GetUnit(0).Hp);
        Assert.Equal(2, copy.Units.Count);
        Assert.NotSame(state.GetUnit(0), copy.GetUnit(0));
    }

    [Fact]
    public void CloneKeepsTheRandomPositionAndStartsWithoutPendingEvents()
    {
        var state = new BattleState(99);
        state.Random.NextUInt64();
        var copy = state.Clone();
        Assert.Empty(copy.DrainEvents());
        Assert.Equal(state.Random.NextUInt64(), copy.Random.NextUInt64());
        Assert.Equal(99UL, copy.Seed);
    }

    [Fact]
    public void SideOppositeSwaps()
    {
        Assert.Equal(Side.B, Side.A.Opposite());
        Assert.Equal(Side.A, Side.B.Opposite());
    }
}
