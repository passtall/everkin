using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class FlyingSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, MoveOption? place = null, RowTarget? row = null) =>
        new(user, skill, target, row, place);

    private static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static bool Has(BattleState s, int unit, string status) => Statuses.Has(s.GetUnit(unit), status);

    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    private static void AdvanceTo(BattleState s, int time)
    {
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            if (e.Kind == TimelineKind.AutoEvent) Totems.HandleAutoEvent(s, e);
            else Statuses.HandleExpiry(s, e);
        }
    }

    private static (BattleState S, int Flyer, int Ally, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var flyer = World.Unit(s, Side.A, Row.Front, skills);
        var ally = World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, flyer, ally, enemy);
    }

    [Fact]
    public void AllThirteenFlyingSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "flying").ToList();
        Assert.Equal(13, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
        }
    }

    // ---- Mobility ----

    [Fact]
    public void TakeOffMakesTheUnitAirborneForTwentyOrFortyTimeUnits()
    {
        var (s, f, _, _) = Board(1, "take_off", "high_altitude");
        Run(s, Use(f, "take_off"));
        Assert.True(s.GetUnit(f).IsAirborne);
        Assert.Equal(40, Statuses.Get(s.GetUnit(f), StatusIds.Airborne)!.EndsAt);

        var (s2, f2, _, _) = Board(1, "take_off");
        Run(s2, Use(f2, "take_off"));
        Assert.Equal(20, Statuses.Get(s2.GetUnit(f2), StatusIds.Airborne)!.EndsAt);
        AdvanceTo(s2, 21);
        Assert.False(s2.GetUnit(f2).IsAirborne);
    }

    [Fact]
    public void AirborneUnitsCannotBeReachedByMeleeAndTakeOffIsRefusedUnderCurseOfWeight()
    {
        var (s, f, _, enemy) = Board(1, "take_off");
        s.GetUnit(enemy).LearnSkill("brawl_strike");
        Run(s, Use(f, "take_off"));
        Assert.NotNull(Why(s, Use(enemy, "brawl_strike", f)));

        var (s2, f2, _, _) = Board(1, "take_off");
        Statuses.Apply(s2, new StatusApplication(f2, StatusIds.Weighted, Duration: 20));
        Assert.NotNull(Why(s2, Use(f2, "take_off")));
    }

    [Fact]
    public void SkyLordIsAlwaysAirborneWithThirtyPercentLessHpAndRisesAgainAfterTurbulence()
    {
        var (s, f, _, _) = Board(1, "take_off", "high_altitude", "sky_lord", "gust", "headwind", "turbulence");
        Assert.True(s.GetUnit(f).IsAirborne);
        Assert.Equal(28, s.GetUnit(f).MaxHp);
        Assert.Equal(28, s.GetUnit(f).Hp);
        Assert.Null(Statuses.Get(s.GetUnit(f), StatusIds.Airborne)!.EndsAt);
        s.ShiftMomentum(Side.A, 0);
        Run(s, Use(f, "turbulence"));
        Assert.True(s.GetUnit(f).IsAirborne);                                // up again at once
    }

    [Fact]
    public void AirliftMovesAnAllyToAnyFreeGapOfYourSideIncludingItsOwnRow()
    {
        var (s, f, ally, _) = Board(1, "take_off", "airlift");
        World.Unit(s, Side.A, Row.Middle);
        var options = World.Catalog.Get("airlift").PlacementOptionsFor(s, f, ally);
        Assert.Contains(new MoveOption(Row.Rear, 0), options);
        Assert.Contains(new MoveOption(Row.Middle, 1), options);
        Assert.DoesNotContain(new MoveOption(Row.Middle, 0), options);          // where it already is
        Run(s, Use(f, "airlift", ally, new MoveOption(Row.Rear, 0)));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(ally).Row);
        Assert.NotNull(Why(s, Use(f, "airlift", s.Units.First(u => u.Side == Side.B).Id, new MoveOption(Row.Rear, 0))));   // own side only
    }

    // ---- Skywatch ----

    [Fact]
    public void ScoutAheadRevealsASideAndKeepsItFromHidingForTenTimeUnits()
    {
        var (s, f, _, enemy) = Board(1, "scout_ahead");
        Stealth.Gain(s, enemy);
        Run(s, Use(f, "scout_ahead", row: new RowTarget(Side.B, Row.Rear)));
        Assert.False(s.GetUnit(enemy).IsStealthed);
        Assert.False(Stealth.CanGain(s, enemy));
        AdvanceTo(s, 11);
        Assert.True(Stealth.CanGain(s, enemy));
    }

    [Fact]
    public void SpotterMarksForTwentyAndLongWatchAddsTen()
    {
        var (s, f, _, enemy) = Board(1, "scout_ahead", "spotter");
        Run(s, Use(f, "spotter", enemy));
        Assert.Equal(20, Statuses.Get(s.GetUnit(enemy), StatusIds.Marked)!.EndsAt);

        var (s2, f2, _, e2) = Board(1, "scout_ahead", "spotter", "long_watch");
        Run(s2, Use(f2, "spotter", e2));
        Assert.Equal(30, Statuses.Get(s2.GetUnit(e2), StatusIds.Marked)!.EndsAt);
    }

    [Fact]
    public void WatchtowerRepeatsScoutAheadOnTheEnemySideEveryTwentyTimeUnitsWhileAirborne()
    {
        var (s, f, _, enemy) = Board(1, "take_off", "scout_ahead", "spotter", "long_watch", "watchtower");
        Stealth.Gain(s, enemy);
        AdvanceTo(s, 20);
        Assert.True(s.GetUnit(enemy).IsStealthed);                             // not airborne: nothing happens
        Run(s, Use(f, "take_off"));
        AdvanceTo(s, 40);
        Assert.False(s.GetUnit(enemy).IsStealthed);                            // airborne at the next tick
    }

    // ---- Assault ----

    [Fact]
    public void GustPushesAUnitOneRowBackHeadwindDelaysItsTurnAndDustUpBlindsIt()
    {
        var (s, f, _, enemy) = Board(1, "gust", "headwind", "dust_up", "turbulence");
        s.Timeline.ScheduleTurn(enemy, 4, 5);
        var due = s.Timeline.TurnOf(enemy)!.Due;
        Run(s, Use(f, "headwind", enemy));
        Assert.Equal(due + 5, s.Timeline.TurnOf(enemy)!.Due);
        Ready(s, f);
        Run(s, Use(f, "dust_up", enemy));
        Assert.Equal(s.Timeline.Now + 10, Statuses.Get(s.GetUnit(enemy), StatusIds.Blind)!.EndsAt);
        Ready(s, f);
        Run(s, Use(f, "gust", enemy));
        Assert.NotEqual(Row.Front, s.Formation.PositionOf(enemy).Row);
    }

    [Fact]
    public void TurbulenceLandsEveryAirborneUnitOnBothSides()
    {
        var (s, f, ally, enemy) = Board(1, "take_off", "gust", "headwind", "turbulence");
        foreach (var id in new[] { f, ally, enemy }) Statuses.Apply(s, new StatusApplication(id, StatusIds.Airborne, Duration: 30));
        s.ShiftMomentum(Side.A, 0);
        Run(s, Use(f, "turbulence"));
        Assert.False(s.GetUnit(f).IsAirborne || s.GetUnit(ally).IsAirborne || s.GetUnit(enemy).IsAirborne);
    }

    [Fact]
    public void BoundlessSkyGivesAnAirborneUnitsSkillsUnlimitedRange()
    {
        var (s, f, _, _) = Board(1, "take_off", "high_altitude", "sky_lord", "gust", "headwind", "dust_up", "boundless_sky");
        var far = World.Unit(s, Side.B, Row.Rear);
        Assert.Null(Why(s, Use(f, "dust_up", far)));                           // airborne (Sky Lord): any distance
        Statuses.Remove(s, f, StatusIds.Airborne, StatusEndCause.Expired);
        Assert.NotNull(Why(s, Use(f, "dust_up", far)));                        // grounded: D1 only
    }
}
