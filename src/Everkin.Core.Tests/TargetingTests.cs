using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class TargetingTests
{
    /// <summary>A battle where units are placed one call at a time; each call returns the new unit id.</summary>
    private sealed class Board
    {
        public BattleState State { get; } = new(1);

        public int Add(Side side, Row row, bool stealthed = false, bool ko = false)
        {
            var id = AddUnit(State, side, row);
            State.GetUnit(id).IsStealthed = stealthed;
            if (ko) State.GetUnit(id).Hp = 0;
            return id;
        }
    }

    private static int AddUnit(BattleState state, Side side, Row row)
    {
        var unit = state.AddUnit(World.Def(), side);
        state.Formation.Place(unit.Id, side, row);
        return unit.Id;
    }

    // ---- no range, melee needs the Front row (section 8.1) ----

    [Fact]
    public void AnyLegalUnitOnEitherSideCanBeTargetedWhateverTheRows()
    {
        var b = new Board();
        var a = b.Add(Side.A, Row.Rear);
        var ally = b.Add(Side.A, Row.Front);
        var eFront = b.Add(Side.B, Row.Front);
        var eRear = b.Add(Side.B, Row.Rear);
        Assert.Equal(new[] { a, ally, eFront, eRear }.Order(), Targeting.LegalTargets(b.State, a).Order());
    }

    [Theory]
    [InlineData(Row.Front, true)]
    [InlineData(Row.Middle, false)]
    [InlineData(Row.Rear, false)]
    public void AMeleeSkillNeedsTheUserInItsOwnFrontRow(Row userRow, bool legal)
    {
        var b = new Board();
        var u = b.Add(Side.A, userRow);
        var e = b.Add(Side.B, Row.Rear);
        Assert.Equal(legal, Targeting.IsLegalTarget(b.State, u, e, new TargetOptions(Delivery: Delivery.Melee)));
        // Projectile and direct skills work from any row.
        Assert.True(Targeting.IsLegalTarget(b.State, u, e, new TargetOptions(Delivery: Delivery.Projectile)));
        Assert.True(Targeting.IsLegalTarget(b.State, u, e, new TargetOptions(Delivery: Delivery.Direct)));
    }

    [Fact]
    public void TheFrontRowRuleCanBeSetAsideForTheSkillsThatSayIt()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Rear);
        var e = b.Add(Side.B, Row.Front);
        Assert.True(Targeting.IsLegalTarget(b.State, u, e, new TargetOptions(Delivery: Delivery.Melee, AnyRow: true)));
    }

    [Fact]
    public void FrontRowMeansTheRealFrontRowEvenWhenItsAlliesAreGone()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Middle);
        var e = b.Add(Side.B, Row.Front);
        Assert.False(Targeting.InFrontRow(b.State, u));
        Assert.False(Targeting.IsLegalTarget(b.State, u, e, new TargetOptions(Delivery: Delivery.Melee)));
    }

    [Fact]
    public void MeleeCannotTargetAnAirborneUnitUnlessTheUserIsAirborne()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var e = b.Add(Side.B, Row.Front);
        b.State.GetUnit(e).IsAirborne = true;
        var melee = new TargetOptions(Delivery: Delivery.Melee);
        Assert.False(Targeting.IsLegalTarget(b.State, u, e, melee));
        Assert.True(Targeting.IsLegalTarget(b.State, u, e, new TargetOptions(Delivery: Delivery.Projectile)));
        b.State.GetUnit(u).IsAirborne = true;
        Assert.True(Targeting.IsLegalTarget(b.State, u, e, melee));
    }

    // ---- melee reach (section 8.1) ----

    [Fact]
    public void MeleeReachIsTheWholeEnemyFrontRowKoBodiesOutStealthedUnitsIn()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var e1 = b.Add(Side.B, Row.Front);
        var e2 = b.Add(Side.B, Row.Front, stealthed: true);
        b.Add(Side.B, Row.Front, ko: true);
        b.Add(Side.B, Row.Middle);
        Assert.Equal(new[] { e1, e2 }, Targeting.EnemiesInMeleeReach(b.State, u));
    }

    [Fact]
    public void MeleeReachIsEmptyWhenTheUserIsNotInItsFrontRowOrNoLivingEnemyIsLeft()
    {
        var b = new Board();
        var back = b.Add(Side.A, Row.Middle);
        var front = b.Add(Side.A, Row.Front);
        var mid = b.Add(Side.B, Row.Middle);
        Assert.Equal(new[] { mid }, Targeting.EnemiesInMeleeReach(b.State, front));          // the Front row is empty: the Middle row is hit
        var f = b.Add(Side.B, Row.Front);
        Assert.Empty(Targeting.EnemiesInMeleeReach(b.State, back));
        Assert.Equal(new[] { f }, Targeting.EnemiesInMeleeReach(b.State, front));
        b.State.GetUnit(f).Hp = 0;
        b.State.GetUnit(mid).Hp = 0;
        Assert.Empty(Targeting.EnemiesInMeleeReach(b.State, front));
    }

    [Fact]
    public void MeleeReachPassesOverRowsWithOnlyKoBodiesOrOnlyStealthedUnitsAndHitsNobodyWhenEveryoneIsStealthed()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, ko: true);
        b.Add(Side.B, Row.Middle, stealthed: true);
        var rear = b.Add(Side.B, Row.Rear);
        Assert.Equal(new[] { rear }, Targeting.EnemiesInMeleeReach(b.State, u));             // KO-only and stealth-only rows are passed over
        b.State.GetUnit(rear).Hp = 0;
        Assert.Empty(Targeting.EnemiesInMeleeReach(b.State, u));                             // every living enemy is stealthed: nobody is hit
    }

    // ---- legality ----

    [Fact]
    public void AStealthedEnemyCannotBeTargetedButAStealthedAllyCan()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var ally = b.Add(Side.A, Row.Middle, stealthed: true);
        var enemy = b.Add(Side.B, Row.Front, stealthed: true);
        Assert.False(Targeting.IsLegalTarget(b.State, u, enemy));
        Assert.True(Targeting.IsLegalTarget(b.State, u, ally));
    }

    [Fact]
    public void KoBodiesAreOnlyLegalForSkillsThatSayIt()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var body = b.Add(Side.A, Row.Middle, ko: true);
        Assert.False(Targeting.IsLegalTarget(b.State, u, body));
        Assert.True(Targeting.IsLegalTarget(b.State, u, body, new TargetOptions(IncludeKnockedOut: true)));
    }

    [Fact]
    public void OwnSideOnlyAndEnemySideOnlyFilterTheSide()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var ally = b.Add(Side.A, Row.Middle);
        var enemy = b.Add(Side.B, Row.Front);
        Assert.Equal(new[] { u, ally }, Targeting.LegalTargets(b.State, u, new TargetOptions(TargetSide.OwnSideOnly)));
        Assert.Equal(new[] { enemy }, Targeting.LegalTargets(b.State, u, new TargetOptions(TargetSide.EnemySideOnly)));
    }

    [Fact]
    public void ATargetRestrictionRemovesAUnitFromTheLegalTargets()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var e1 = b.Add(Side.B, Row.Front);
        var e2 = b.Add(Side.B, Row.Front);
        b.State.AddTargetRestriction((_, _, target) => target != e1);
        Assert.Equal(new[] { u, e2 }, Targeting.LegalTargets(b.State, u));
    }

    // ---- nearest (section 8.5) ----

    [Fact]
    public void NearestIsTheEnemyInTheFewestRowsFromItsFrontEmptyRowsIgnored()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Rear);
        var middle = b.Add(Side.B, Row.Middle);
        b.Add(Side.B, Row.Rear);
        Assert.Equal(new[] { middle }, Targeting.NearestEnemies(b.State, u));
        Assert.Equal(middle, Targeting.PickNearest(b.State, u));
    }

    [Fact]
    public void AStealthedOrKoFrontDoesNotMakeTheFrontRowCountSoTheMiddleIsNearest()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, stealthed: true);
        b.Add(Side.B, Row.Front, ko: true);
        var middle = b.Add(Side.B, Row.Middle);
        var rear = b.Add(Side.B, Row.Rear);
        Assert.Equal(new[] { middle }, Targeting.NearestEnemies(b.State, u));
        Assert.Contains(rear, Targeting.LegalTargets(b.State, u));
    }

    [Fact]
    public void NoLegalEnemyMeansNoNearestTarget()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        b.Add(Side.B, Row.Front, stealthed: true);
        Assert.Empty(Targeting.NearestEnemies(b.State, u));
        Assert.Null(Targeting.PickNearest(b.State, u));
    }

    [Fact]
    public void ASingleNearestTargetCostsNoRoll()
    {
        var b = new Board();
        var u = b.Add(Side.A, Row.Front);
        var e = b.Add(Side.B, Row.Front);
        b.Add(Side.B, Row.Rear);
        var before = b.State.Random.Clone();
        Assert.Equal(e, Targeting.PickNearest(b.State, u));
        Assert.Equal(before.NextUInt64(), b.State.Random.NextUInt64());
    }

    [Fact]
    public void EquallyNearTargetsAreChosenByTheSeededRollWithEqualChance()
    {
        // 20.5: a skill hits the nearest enemy and two are equally near: a seeded roll picks one, each with equal chance.
        var counts = new int[2];
        for (ulong seed = 1; seed <= 4000; seed++)
        {
            var state = new BattleState(seed);
            var u = AddUnit(state, Side.A, Row.Front);
            var e1 = AddUnit(state, Side.B, Row.Front);
            var e2 = AddUnit(state, Side.B, Row.Front);
            var picked = Targeting.PickNearest(state, u)!.Value;
            counts[picked == e1 ? 0 : 1]++;
            Assert.Contains(picked, new[] { e1, e2 });
        }
        Assert.All(counts, c => Assert.InRange(c, 1800, 2200));
    }

    [Fact]
    public void TheSameSeedPicksTheSameTarget()
    {
        int Pick()
        {
            var state = new BattleState(77);
            var u = AddUnit(state, Side.A, Row.Front);
            AddUnit(state, Side.B, Row.Front);
            AddUnit(state, Side.B, Row.Front);
            AddUnit(state, Side.B, Row.Front);
            return Targeting.PickNearest(state, u)!.Value;
        }
        Assert.Equal(Pick(), Pick());
    }
}
