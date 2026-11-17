using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The Skyborne, Spiritwalker and Ritualist.</summary>
public class SkyWaterSoulTests
{
    [Fact]
    public void AllThreeClassesAreImplemented()
    {
        AssertClassImplemented("skyborne", magical: false);
        AssertClassImplemented("spiritwalker", magical: true);
        AssertClassImplemented("ritualist", magical: true);
    }

    // ---- Skyborne ----

    [Fact]
    public void TakeOffMakesTheSkyborneAirborneForFortyAndAUnitUnderCurseOfWeightCannotUseIt()
    {
        var (s, sky, e) = Board(1, "take_off");
        Run(s, Use(sky, "take_off"));
        Assert.True(s.GetUnit(sky).IsAirborne);
        Assert.Equal(40, Statuses.Get(s.GetUnit(sky), StatusIds.Airborne)!.TimeLeft(s.Timeline.Now));
        AdvanceTo(s, 40);
        Assert.False(s.GetUnit(sky).IsAirborne);
        Statuses.Apply(s, new StatusApplication(sky, StatusIds.Weighted, Duration: 20));
        Assert.NotNull(Why(s, Use(sky, "take_off")));
    }

    [Fact]
    public void TakeOffAgainWhileAirborneRestartsTheTimer()
    {
        var (s, sky, _) = Board(1, "take_off");
        Run(s, Use(sky, "take_off"));
        AdvanceTo(s, 15);
        K.Ready(s, sky);
        Run(s, Use(sky, "take_off"));
        Assert.Equal(40, Statuses.Get(s.GetUnit(sky), StatusIds.Airborne)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void LiftMakesOneAllyNextToTheSkyborneAirborneForTheSameTime()
    {
        var s = new BattleState(1);
        var sky = World.Unit(s, Side.A, Row.Front, "take_off", "lift");
        var next = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(sky, "take_off"));
        Assert.True(s.GetUnit(next).IsAirborne);
        Assert.False(s.GetUnit(far).IsAirborne);
        Assert.Equal(40, Statuses.Get(s.GetUnit(next), StatusIds.Airborne)!.TimeLeft(s.Timeline.Now));
    }

    [Fact]
    public void GustPushesAUnitOneRowBackAndARearUnitStays()
    {
        var (s, sky, e) = Board(1, "gust");
        Run(s, Use(sky, "gust", e));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(e).Row);
        K.Ready(s, sky);
        s.Formation.Remove(e);
        s.Formation.Place(e, Side.B, Row.Rear);
        Run(s, Use(sky, "gust", e));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(e).Row);
    }

    [Fact]
    public void GustPushesAFrontUnitIntoAnEmptyMiddleRowWithoutBumpingTheRearUnitBehindIt()
    {
        // 20.5: Gust pushes a Front unit back while the Middle row is empty: it lands in Middle and nothing is bumped.
        var (s, sky, e) = Board(1, "gust");
        var rear = World.Unit(s, Side.B, Row.Rear);
        Run(s, Use(sky, "gust", e));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(e).Row);
        Assert.Equal(Row.Rear, s.Formation.PositionOf(rear).Row);
        Assert.Equal(0, Lost(s, e) + Lost(s, rear));
    }

    [Fact]
    public void DiveNeedsFlightHitsForNineToElevenFromAnyRowAndCannotBeInterceptedThenLands()
    {
        var s = new BattleState(1);
        var sky = World.Unit(s, Side.A, Row.Rear, "dive", "take_off");
        s.ShiftMomentum(Side.A, 6);
        var guard = World.Unit(s, Side.B, Row.Front);
        var target = World.Unit(s, Side.B, Row.Middle);
        Assert.Equal("only while airborne", Why(s, Use(sky, "dive", target)));
        Run(s, Use(sky, "take_off"));
        K.Ready(s, sky);
        Assert.Null(Why(s, Use(sky, "dive", target)));                                     // from the Rear row
        var events = Run(s, Use(sky, "dive", target));
        Assert.DoesNotContain(events, e => e is Intercepted);
        Assert.InRange(Lost(s, target), 9, 11);
        Assert.False(s.GetUnit(sky).IsAirborne);                                           // landed
    }

    [Fact]
    public void DiveCannotHitAnAirborneUnitAndStillTargetsTheRearRowWhileAMeleeSkillOfTheNonAirborneWouldNot()
    {
        var s = new BattleState(1);
        var sky = World.Unit(s, Side.A, Row.Middle, "dive");
        s.GetUnit(sky).IsAirborne = true;
        var flyer = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(flyer).IsAirborne = true;
        s.ShiftMomentum(Side.A, 6);
        Assert.Empty(Actions.LegalTargets(s, World.Catalog, sky, "dive"));
    }

    [Fact]
    public void SkyLordIsAlwaysAirborneWithThirtyPercentLessHpAndRisesAgainAfterLanding()
    {
        var s = new BattleState(1);
        var sky = World.Unit(s, Side.A, Row.Front, "sky_lord", "dive");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Assert.True(s.GetUnit(sky).IsAirborne);
        Assert.Equal((28, 28), (s.GetUnit(sky).MaxHp, s.GetUnit(sky).Hp));
        Statuses.Remove(s, sky, StatusIds.Airborne, StatusEndCause.Expired);
        s.RunAuras();
        Assert.True(s.GetUnit(sky).IsAirborne);                                            // rose at once
        Statuses.Apply(s, new StatusApplication(sky, StatusIds.Weighted, Duration: 20));
        Statuses.Remove(s, sky, StatusIds.Airborne, StatusEndCause.Expired);
        s.RunAuras();
        Assert.False(s.GetUnit(sky).IsAirborne);                                           // Curse of Weight keeps it down
    }

    [Fact]
    public void EagleEyeRevealsEveryStealthedEnemyWhileAirborneAndAnEnemyThatGainsStealthLosesItAtOnce()
    {
        var s = new BattleState(1);
        var sky = World.Unit(s, Side.A, Row.Front, "eagle_eye");
        var hidden = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(hidden).IsStealthed = true;
        s.RunAuras();
        Assert.True(s.GetUnit(hidden).IsStealthed);                                        // not airborne: no effect
        s.GetUnit(sky).IsAirborne = true;
        s.RunAuras();
        Assert.False(s.GetUnit(hidden).IsStealthed);
        Stealth.Gain(s, hidden);
        s.RunAuras();
        Assert.False(s.GetUnit(hidden).IsStealthed);
    }

    // ---- Spiritwalker ----

    private static (BattleState S, int Walker, int A, int B) Souls(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var w = World.Unit(s, Side.A, Row.Rear, skills);
        var a = s.AddUnit(World.Def("ankylosaur-guardian-club", power: 5), Side.B);
        s.Formation.Place(a.Id, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, w, a.Id, b);
    }

    [Fact]
    public void PuppetSoulMakesAUnitHitAUnitNextToItWithItsUniqueAttackUsingItsOwnPowerForFree()
    {
        var (s, w, a, b) = Souls(1, "puppet_soul");
        s.GetUnit(a).Power = 10;                                                             // 3-4 at Power 10 is 6-8
        Run(s, Use(w, "puppet_soul", a, second: b));
        Assert.InRange(Lost(s, b), 6, 8);
        Assert.Equal(Timeline.Wait(6, 5), s.Timeline.TurnOf(w)!.Due);                       // only the Spiritwalker's turn is spent
    }

    [Fact]
    public void PuppetSoulNeedsTwoNeighboursAndIgnoresTheFrontRowRuleButNotAirborne()
    {
        var (s, w, a, b) = Souls(1, "puppet_soul");
        var far = World.Unit(s, Side.B, Row.Rear);
        Assert.NotNull(Why(s, Use(w, "puppet_soul", a, second: far)));                       // not next to it
        s.Formation.Remove(a);
        s.Formation.Place(a, Side.B, Row.Middle);                                            // a melee attacker outside the Front row still hits
        s.Formation.Remove(b);
        s.Formation.Place(b, Side.B, Row.Middle);
        Assert.Null(Why(s, Use(w, "puppet_soul", a, second: b)));
        s.GetUnit(b).IsAirborne = true;
        Assert.NotNull(Why(s, Use(w, "puppet_soul", a, second: b)));
    }

    [Fact]
    public void PuppeteerDoublesPuppetSoulHitsOnly()
    {
        var (s, w, a, b) = Souls(1, "puppet_soul", "puppeteer");
        Run(s, Use(w, "puppet_soul", a, second: b));
        Assert.InRange(Lost(s, b), 6, 8);                                                    // 3-4 doubled
        Assert.Null(s.PuppeteerId);
    }

    [Fact]
    public void LifeSwapSwapsTheCurrentHpOfTwoUnitsEachCappedAtItsMax()
    {
        var (s, w, a, b) = Souls(1, "life_swap");
        s.GetUnit(a).Hp = 5;
        s.GetUnit(b).MaxHp = 20;
        s.GetUnit(b).Hp = 20;
        Run(s, Use(w, "life_swap", a, second: b));
        Assert.Equal(20, s.GetUnit(a).Hp);
        Assert.Equal(5, s.GetUnit(b).Hp);
        Assert.Equal(3, s.Momentum.Value);  // M5.10 costs
        K.Ready(s, w);
        s.ShiftMomentum(Side.A, 5);
        s.GetUnit(a).Hp = 40;
        s.GetUnit(b).MaxHp = 10;
        s.GetUnit(b).Hp = 3;
        Run(s, Use(w, "life_swap", a, second: b));
        Assert.Equal(10, s.GetUnit(b).Hp);                                                   // capped at its max
        Assert.Equal(3, s.GetUnit(a).Hp);
    }

    [Fact]
    public void LifeSwapCanKoAUnitThatReceivesZeroHp()
    {
        var (s, w, a, b) = Souls(1, "life_swap");
        s.GetUnit(b).Hp = 1;
        s.GetUnit(a).Hp = 1;
        s.GetUnit(w).Hp = 40;
        Run(s, Use(w, "life_swap", w, second: a));
        Assert.Equal(1, s.GetUnit(w).Hp);
        Assert.Equal(40, s.GetUnit(a).Hp);
    }

    [Fact]
    public void ForcedStepMovesAUnitToAFreeSpotOfYourChoiceOnItsOwnSideAndImmobilizedOrHoldPositionStopIt()
    {
        var (s, w, a, b) = Souls(1, "forced_step");
        var places = World.Catalog.Get("forced_step").PlacementOptionsFor(s, w, a);
        Assert.DoesNotContain(places, p => p.Row == Row.Front);
        Run(s, Use(w, "forced_step", a, place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(a).Row);
        Assert.Equal(Side.B, s.GetUnit(a).Side);

        Statuses.Apply(s, new StatusApplication(b, StatusIds.Immobilized, Duration: 10));
        K.Ready(s, w);
        s.ShiftMomentum(Side.A, 4);
        Assert.NotNull(Why(s, Use(w, "forced_step", b, place: new MoveOption(Row.Rear, 0))));
        var s2 = new BattleState(1);
        var w2 = World.Unit(s2, Side.A, Row.Rear, "forced_step");
        var holder = World.Unit(s2, Side.B, Row.Front, "hold_position");
        BattleStart.Install(s2, World.Catalog);
        Assert.NotNull(Why(s2, Use(w2, "forced_step", holder, place: new MoveOption(Row.Rear, 0))));
    }

    [Fact]
    public void SoulAnchorStopsEnemiesFromPossessingMovingOrSoulSwappingTheSpiritwalker()
    {
        var s = new BattleState(1);
        var anchor = World.Unit(s, Side.A, Row.Front, "soul_anchor");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front, "puppet_soul", "life_swap", "forced_step");
        var other = World.Unit(s, Side.B, Row.Middle);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.B, 6);
        Assert.NotNull(Why(s, Use(enemy, "life_swap", anchor, second: other)));
        Assert.NotNull(Why(s, Use(enemy, "forced_step", anchor, place: new MoveOption(Row.Rear, 0))));
        Assert.NotNull(Why(s, Use(enemy, "puppet_soul", anchor, second: ally)));
        Assert.Empty(Movement.ForceMove(s, anchor, Row.Middle, Side.B).Moves);
        Assert.Null(Why(s, Use(enemy, "life_swap", ally, second: other)));                   // others are fine
    }

    [Fact]
    public void HauntingConfusesTheUnitThatKosTheSpiritwalkerForTwenty()
    {
        var s = new BattleState(1);
        var w = World.Unit(s, Side.A, Row.Front, "haunting");
        var killer = World.Unit(s, Side.B, Row.Front, "tail_club");
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(w).Hp = 1;
        Run(s, Use(killer, "tail_club", w));
        Assert.True(s.GetUnit(w).IsKnockedOut);
        Assert.Equal(20, Statuses.Get(s.GetUnit(killer), StatusIds.Confused)!.TimeLeft(s.Timeline.Now));
    }

    // ---- Ritualist ----

    [Fact]
    public void RainDanceWatersEveryUnitSpotOfARowFor40AndAlliesStandingInItHealFour()
    {
        var s = new BattleState(1);
        var r = World.Unit(s, Side.A, Row.Front, "rain_dance");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Middle);
        var e = World.Unit(s, Side.B, Row.Middle);
        s.ShiftMomentum(Side.A, 6);
        foreach (var id in new[] { a, b, e }) s.GetUnit(id).Hp = 10;
        Run(s, Use(r, "rain_dance", row: new RowTarget(Side.A, Row.Middle)));
        Assert.Equal(2, s.Ground.Effects.Count(f => f.Kind == GroundKind.Water));
        Assert.Equal(40, s.Ground.Find(s.Ground.SpotOf(a), GroundKind.Water)!.EndsAt - s.Timeline.Now);
        Assert.Equal(14, s.GetUnit(a).Hp);
        Assert.Equal(14, s.GetUnit(b).Hp);
        K.Ready(s, r);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(r, "rain_dance", row: new RowTarget(Side.B, Row.Middle)));
        Assert.Equal(10, s.GetUnit(e).Hp);                                                   // enemies are not healed
    }

    [Fact]
    public void HoarfrostPutsIceOnAUnitsSpotForTwenty()
    {
        var (s, r, e) = Board(1, "hoarfrost");
        Run(s, Use(r, "hoarfrost", e));
        Assert.Equal(20, s.Ground.Find(s.Ground.SpotOf(e), GroundKind.Ice)!.EndsAt - s.Timeline.Now);
    }

    [Fact]
    public void RiteOfMistsHalvesProjectileHitsFromUnitsOnWaterForTwentyOnly()
    {
        var s = new BattleState(1);
        var r = World.Unit(s, Side.A, Row.Rear, "rite_of_mists");
        var shooter = World.Unit(s, Side.B, Row.Front, "rivet_shot");
        var target = World.Unit(s, Side.A, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        s.Ground.Place(GroundKind.Water, s.Ground.SpotOf(shooter), 60);
        Run(s, Use(r, "rite_of_mists"));
        Run(s, Use(shooter, "rivet_shot", target));
        Assert.InRange(Lost(s, target), 1, 2);                                              // 3-4 halved, rounded down
        AdvanceTo(s, 21);
        K.Ready(s, shooter);
        var before = Lost(s, target);
        Run(s, Use(shooter, "rivet_shot", target));
        Assert.InRange(Lost(s, target) - before, 3, 4);
    }

    [Fact]
    public void DelugeMakesWaterLastUntilTheEndOfBattle()
    {
        var (s, r, e) = Board(1, "rain_dance", "deluge");
        Run(s, Use(r, "rain_dance", row: new RowTarget(Side.B, Row.Front)));
        AdvanceTo(s, 5000);
        Assert.True(s.Ground.Has(s.Ground.SpotOf(e), GroundKind.Water));
    }

    [Fact]
    public void TidecallerGivesAlliesOnWaterOneLessDelay()
    {
        var s = new BattleState(1);
        var r = World.Unit(s, Side.A, Row.Rear, "rain_dance", "tidecaller");
        var ally = World.Unit(s, Side.A, Row.Front, "tail_club");
        var enemy = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        s.Ground.Place(GroundKind.Water, s.Ground.SpotOf(ally), 40);
        Run(s, Use(ally, "tail_club", enemy));
        Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(ally)!.Due);                     // Delay 4 - 1
    }

    [Fact]
    public void FrostbinderImmobilizesAUnitStandingOnTheRitualistsIceUntilItLeavesOrTheIceEnds()
    {
        var s = new BattleState(1);
        var r = World.Unit(s, Side.A, Row.Rear, "hoarfrost", "frostbinder");
        var e = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Middle);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(r, "hoarfrost", e));
        Assert.True(s.GetUnit(e).IsImmobilized);
        Assert.Empty(Movement.LegalMoves(s, e));
        AdvanceTo(s, 20);
        s.RunAuras();
        Assert.False(s.GetUnit(e).IsImmobilized);
        Assert.NotEmpty(Movement.LegalMoves(s, e));
    }
}
