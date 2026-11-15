using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class BulwarkSkillTests
{
    [Fact]
    public void AllBulwarkSkillsAreImplemented() => AssertClassImplemented("bulwark", magical: false);

    // ---- Leap to Defend ----

    private static (BattleState S, int Bulwark, int Ally, int Enemy) Leap(ulong seed = 1)
    {
        var s = new BattleState(seed);
        var b = World.Unit(s, Side.A, Row.Rear, "leap_to_defend");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front, "tail_club", "rivet_shot");
        BattleStart.Install(s, World.Catalog);
        return (s, b, ally, enemy);
    }

    [Fact]
    public void LeapToDefendMovesTheBulwarkIntoAnyGapOfTheRowInFrontOfTheAllyAndOnlyOwnSideAllies()
    {
        var (s, b, ally, enemy) = Leap();
        var front = World.Unit(s, Side.A, Row.Front);
        var places = World.Catalog.Get("leap_to_defend").PlacementOptionsFor(s, b, ally);
        Assert.Equal(new[] { new MoveOption(Row.Front, 0), new MoveOption(Row.Front, 1) }, places);
        Assert.NotNull(Why(s, Use(b, "leap_to_defend", enemy, place: places[0])));       // own side only
        Run(s, Use(b, "leap_to_defend", ally, place: places[1]));
        Assert.Equal(new MoveOption(Row.Front, 1).Row, s.Formation.PositionOf(b).Row);
        Assert.Equal(new[] { front, b }, s.Formation.Units(Side.A, Row.Front));
        Assert.True(Has(s, ally, StatusIds.LeapCover));
    }

    [Fact]
    public void TheNextAttackAimedAtTheAllyHitsTheBulwarkInsteadOnce()
    {
        var (s, b, ally, enemy) = Leap();
        World.Unit(s, Side.A, Row.Front);
        Run(s, Use(b, "leap_to_defend", ally, place: World.Catalog.Get("leap_to_defend").PlacementOptionsFor(s, b, ally)[0]));
        Run(s, Use(enemy, "rivet_shot", ally));
        Assert.Equal(40, s.GetUnit(ally).Hp);
        Assert.InRange(Lost(s, b), 3, 4);
        Assert.False(Has(s, ally, StatusIds.LeapCover));
        K.Ready(s, enemy);
        Run(s, Use(enemy, "rivet_shot", ally));                                          // used up
        Assert.InRange(Lost(s, ally), 3, 4);
    }

    [Fact]
    public void ForAnAllyInTheFrontRowTheBulwarkGoesIntoAGapNextToIt()
    {
        var s = new BattleState(1);
        var b = World.Unit(s, Side.A, Row.Middle, "leap_to_defend");
        var ally = World.Unit(s, Side.A, Row.Front);
        var places = World.Catalog.Get("leap_to_defend").PlacementOptionsFor(s, b, ally);
        Assert.Equal(new[] { new MoveOption(Row.Front, 0), new MoveOption(Row.Front, 1) }, places);
    }

    [Fact]
    public void WhenTheRowIsFullTheBulwarkCannotMoveButStillTakesTheAttack()
    {
        var (s, b, ally, enemy) = Leap();
        for (var i = 0; i < 6; i++) World.Unit(s, Side.A, Row.Front);
        Assert.Empty(World.Catalog.Get("leap_to_defend").PlacementOptionsFor(s, b, ally));
        Assert.Null(Why(s, Use(b, "leap_to_defend", ally)));                              // no place needed
        Run(s, Use(b, "leap_to_defend", ally));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(b).Row);
        Run(s, Use(enemy, "rivet_shot", ally));
        Assert.Equal(0, Lost(s, ally));                                                  // the attack was aimed at the Bulwark (its row may still intercept)
    }

    [Fact]
    public void LeapToDefendCountsAsAMoveSoItTriggersBleeding()
    {
        Assert.True(World.Catalog.Get("leap_to_defend").Tags.HasFlag(EffectTags.Move));
        Assert.False(World.Catalog.Get("leap_to_defend").Magical);
    }

    // ---- Rescue ----

    [Fact]
    public void RescueSwapsWithAnAllyBelowHalfHpAnywhereOnTheSide()
    {
        var s = new BattleState(1);
        var b = World.Unit(s, Side.A, Row.Front, "rescue");
        var weak = World.Unit(s, Side.A, Row.Rear);
        var healthy = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(weak).Hp = 19;
        Assert.NotNull(Why(s, Use(b, "rescue", healthy)));                               // not below half
        Assert.NotNull(Why(s, Use(b, "rescue", 3)));                                     // an enemy: own side only
        Run(s, Use(b, "rescue", weak));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(b).Row);
        Assert.Equal(Row.Front, s.Formation.PositionOf(weak).Row);
        Assert.Equal(4, s.Momentum.Value);                                               // cost 2
    }

    [Fact]
    public void RescueNeedsBothUnitsToBeMovable()
    {
        var s = new BattleState(1);
        var b = World.Unit(s, Side.A, Row.Front, "rescue");
        var weak = World.Unit(s, Side.A, Row.Rear);
        s.ShiftMomentum(Side.A, 6);
        s.GetUnit(weak).Hp = 10;
        Statuses.Apply(s, new StatusApplication(weak, StatusIds.Immobilized, Duration: 10));
        Assert.NotNull(Why(s, Use(b, "rescue", weak)));
    }

    // ---- Shield Bash ----

    [Fact]
    public void ShieldBashHitsForSixToSevenAndPushesTheTargetOneRowBack()
    {
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 60; seed++)
        {
            var (s, b, e) = Board(seed, "shield_bash");
            Run(s, Use(b, "shield_bash", e));
            damages.Add(Lost(s, e));
            Assert.Equal(Row.Middle, s.Formation.PositionOf(e).Row);
        }
        Assert.Equal(new[] { 6, 7 }, damages.OrderBy(x => x));
    }

    [Fact]
    public void ShieldBashOnARearUnitJustHitsAndNeedsTheFrontRow()
    {
        var (s, b, e) = Board(1, "shield_bash");
        s.Formation.Remove(e);
        s.Formation.Place(e, Side.B, Row.Rear);
        Run(s, Use(b, "shield_bash", e));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(e).Row);
        Assert.InRange(Lost(s, e), 6, 7);

        var (s2, b2, e2) = Board(1, "shield_bash");
        s2.Formation.Remove(b2);
        s2.Formation.Place(b2, Side.A, Row.Middle);
        Assert.Equal("needs the Front row", Why(s2, Use(b2, "shield_bash", e2)));
    }

    // ---- keystones ----

    private static (BattleState S, int[] Wall, int Target, int Enemy) Wall(int wallSize, ulong seed = 1, params string[] enemySkills)
    {
        var s = new BattleState(seed);
        var wall = Enumerable.Range(0, wallSize).Select(_ => World.Unit(s, Side.A, Row.Front, "shield_wall")).ToArray();
        var target = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front, enemySkills);
        BattleStart.Install(s, World.Catalog);
        return (s, wall, target, enemy);
    }

    [Fact]
    public void ShieldWallNeedsTwoWallUnitsSideBySideAndThenAlwaysInterceptsMeleeAndProjectiles()
    {
        for (ulong seed = 1; seed <= 40; seed++)
        {
            var (s, wall, target, enemy) = Wall(2, seed, "tail_club", "rivet_shot");
            // The Middle target stands at centre 0, between the two wall units at -1 and +1: 50% overlap each.
            var events = Run(s, Use(enemy, "tail_club", target));
            Assert.Contains(events, e => e is Intercepted ic && wall.Contains(ic.InterceptorUnitId));
        }
        var (s2, wall2, t2, e2) = Wall(1, 1, "tail_club");                                // one wall unit is not a wall
        var anyIntercept = 0;
        for (ulong seed = 1; seed <= 60; seed++)
        {
            var (s3, w3, t3, e3) = Wall(1, seed, "tail_club");
            if (Run(s3, Use(e3, "tail_club", t3)).Any(e => e is Intercepted)) anyIntercept++;
        }
        Assert.InRange(anyIntercept, 1, 59);                                              // only the usual roll
    }

    [Fact]
    public void ShieldWallDoesNotStopDirectAttacksAndNeedsLivingNeighbours()
    {
        var (s, wall, target, enemy) = Wall(2, 1, "sand_spill");
        var events = Run(s, Use(enemy, "sand_spill", wall[0]));                           // a direct attack is never intercepted
        Assert.DoesNotContain(events, e => e is Intercepted);

        var (s2, wall2, t2, e2) = Wall(2, 1, "tail_club");
        s2.GetUnit(wall2[1]).Hp = 0;
        Knockout.ResolveAll(s2);
        var intercepted = 0;
        for (ulong seed = 1; seed <= 60; seed++)
        {
            var (s3, w3, t3, e3) = Wall(2, seed, "tail_club");
            s3.GetUnit(w3[1]).Hp = 0;
            Knockout.ResolveAll(s3);
            if (Run(s3, Use(e3, "tail_club", t3)).Any(e => e is Intercepted)) intercepted++;
        }
        Assert.InRange(intercepted, 1, 59);                                               // a KO body is not part of the wall
    }

    [Fact]
    public void ShieldWallStillGivesAFullOrPartialHit()
    {
        var full = 0;
        var partial = 0;
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var (s, wall, target, enemy) = Wall(2, seed, "tail_club");
            foreach (var e in Run(s, Use(enemy, "tail_club", target)).OfType<Intercepted>().Take(1))
                if (e.Full) full++; else partial++;
        }
        Assert.InRange(full, 25, 75);
        Assert.InRange(partial, 25, 75);
    }

    [Fact]
    public void HoldPositionStopsEnemyEffectsButNotAnAlliesMove()
    {
        var (s, b, e) = Board(1, "hold_position");
        s.Formation.Remove(e);
        s.Formation.Place(e, Side.B, Row.Front);
        Assert.Empty(Movement.ForceMove(s, b, Row.Middle, Side.B).Moves);
        Assert.Equal(Row.Front, s.Formation.PositionOf(b).Row);
        Assert.NotEmpty(Movement.ForceMove(s, b, Row.Middle, Side.A).Moves);               // its own side can still move it
        Assert.False(Movement.Swap(s, b, s.Formation.Units(Side.A, Row.Front).FirstOrDefault(x => x != b), Side.B));
    }

    [Fact]
    public void UnbreakableKeepsOneHpTheFirstTimeOnlyAnyDamageIncluded()
    {
        var (s, b, e) = Board(1, "unbreakable", "tail_club");
        s.GetUnit(b).Hp = 2;
        s.DealDamage(e, b, 30);
        Assert.Equal(1, s.GetUnit(b).Hp);
        Assert.False(s.GetUnit(b).IsKnockedOut);
        s.DealDamage(e, b, 1);
        Assert.True(s.GetUnit(b).IsKnockedOut);
    }

    [Fact]
    public void UnbreakableAlsoSavesFromUnreducibleDamageAndIsPerBulwark()
    {
        var s = new BattleState(1);
        var b = World.Unit(s, Side.A, Row.Front, "unbreakable");
        var b2 = World.Unit(s, Side.A, Row.Front, "unbreakable");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(b).Hp = 1;
        s.GetUnit(b2).Hp = 1;
        s.DealDamage(null, b, 1, DamageSource.Unreducible);
        Assert.Equal(1, s.GetUnit(b).Hp);
        s.DealDamage(null, b2, 1, DamageSource.Unreducible);
        Assert.Equal(1, s.GetUnit(b2).Hp);
    }
}
