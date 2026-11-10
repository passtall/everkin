using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class PreviewTests
{
    private static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null) => new(user, skill, target, row);

    private static BattleState Board(ulong seed, out int attacker, out int target, params string[] skills)
    {
        var s = new BattleState(seed);
        attacker = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.A, Row.Middle);
        target = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        return s;
    }

    /// <summary>Everything a preview must leave alone.</summary>
    private static string Snapshot(BattleState s) =>
        string.Join("|", s.Units.Select(u => $"{u.Id}:{u.Hp}:{u.Ward}:{string.Join(",", u.StatusInstances.Keys)}:{s.Formation.PositionOf(u.Id)}"))
        + "#" + string.Join(",", s.Timeline.Upcoming().Select(e => $"{e.Id}@{e.Due}")) + "#" + s.Momentum.Value + "#" + s.Timeline.Now
        + "#" + s.Random.Clone().NextUInt64();

    [Fact]
    public void ADamageRangeIsTheScaledRangeAndTheRealDamageStaysInsideIt()
    {
        var s = Board(1, out var a, out var t, "brawl_strike");
        var p = Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t));
        var unit = Assert.Single(p.Units, u => u.UnitId == t);
        Assert.Equal((4, 5), (unit.DamageMin, unit.DamageMax));
        Assert.Equal((4, 5), (unit.HpLostMin, unit.HpLostMax));
        for (ulong seed = 1; seed <= 30; seed++)
        {
            var real = Board(seed, out var a2, out var t2, "brawl_strike");
            Actions.Use(real, World.Catalog, Use(a2, "brawl_strike", t2));
            Assert.InRange(40 - real.GetUnit(t2).Hp, unit.HpLostMin, unit.HpLostMax);
        }

        s.GetUnit(a).Power = 3;                                                      // 4–5 at Power 3 is 2–3
        var weak = Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t)).Units.Single(u => u.UnitId == t);
        Assert.Equal((2, 3), (weak.DamageMin, weak.DamageMax));
    }

    [Fact]
    public void AShieldShowsInTheGapBetweenDamageAndHpLost()
    {
        var s = Board(1, out var a, out var t, "brawl_strike");
        s.GetUnit(t).Ward = 3;
        var unit = Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t)).Units.Single(u => u.UnitId == t);
        Assert.Equal((4, 5), (unit.DamageMin, unit.DamageMax));
        Assert.Equal((1, 2), (unit.HpLostMin, unit.HpLostMax));
    }

    [Fact]
    public void AHealRangeAndAnAreaOfSeveralUnitsArePreviewedPerUnit()
    {
        var s = new BattleState(1);
        var healer = World.Unit(s, Side.A, Row.Front, "mend", "rain_of_mending");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var other = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(ally).Hp = 10;
        s.GetUnit(other).Hp = 10;
        var mend = Preview.Skill(s, World.Catalog, Use(healer, "mend", ally)).Units.Single();
        Assert.Equal((8, 10), (mend.HealMin, mend.HealMax));
        s.ShiftMomentum(Side.A, 3);
        var rain = Preview.Skill(s, World.Catalog, Use(healer, "rain_of_mending", row: new RowTarget(Side.A, Row.Middle)));
        Assert.Equal(2, rain.Units.Count);
        Assert.All(rain.Units, u => Assert.Equal((3, 4), (u.HealMin, u.HealMax)));
    }

    [Fact]
    public void APreviewNeverChangesTheStateOrTheRandomSequence()
    {
        var s = Board(5, out var a, out var t, "brawl_strike", "gust", "tempest", "healing_draught");
        s.DrainEvents();
        var before = Snapshot(s);
        Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t));
        Preview.Skill(s, World.Catalog, Use(a, "gust", t));
        Preview.Skill(s, World.Catalog, Use(a, "healing_draught", a));
        Preview.SkipTurn(s, a);
        Preview.Move(s, a, Row.Middle, 0);
        Assert.Equal(before, Snapshot(s));
        Assert.Empty(s.DrainEvents());                                                 // no event leaks out of a preview
    }

    [Fact]
    public void InterceptorsAreListedWithTheirChanceAndDirectSkillsHaveNone()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "shot", "mend");
        var defender = World.Unit(s, Side.B, Row.Front);
        var half = World.Unit(s, Side.B, Row.Front);
        var target = World.Unit(s, Side.B, Row.Middle);                                // centers: Front -1, +1; the target 0 → 50% overlap
        var p = Preview.Skill(s, World.Catalog, Use(a, "shot", target));
        Assert.Equal(new[] { (defender, 25), (half, 25) }, p.Interceptors.Select(i => (i.UnitId, i.Percent)));
        Assert.Empty(Preview.Skill(s, World.Catalog, Use(a, "mend", target)).Interceptors);      // direct

        var s2 = new BattleState(1);
        var a2 = World.Unit(s2, Side.A, Row.Front, "shot");
        var front = World.Unit(s2, Side.B, Row.Front);
        var behind = World.Unit(s2, Side.B, Row.Middle);                               // the same centre: 100%
        Assert.Equal((front, 50), Preview.Skill(s2, World.Catalog, Use(a2, "shot", behind)).Interceptors.Select(i => (i.UnitId, i.Percent)).Single());
    }

    [Fact]
    public void TheSuccessChanceFallsWithBlindAndIsZeroAgainstSidestep()
    {
        var s = Board(1, out var a, out var t, "brawl_strike", "mend");
        Assert.Equal(100, Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t)).SuccessChance);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        Assert.Equal(50, Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t)).SuccessChance);
        Assert.Equal(100, Preview.Skill(s, World.Catalog, Use(a, "mend", a)).SuccessChance);     // not an attack
        Statuses.Apply(s, new StatusApplication(t, StatusIds.Sidestep));
        Assert.Equal(0, Preview.Skill(s, World.Catalog, Use(a, "brawl_strike", t)).SuccessChance);
    }

    [Fact]
    public void TheMomentumGhostShowsTheMeterAfterTheCost()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "freeze");
        var t = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 5);
        var p = Preview.Skill(s, World.Catalog, Use(a, "freeze", t));
        Assert.Equal((3, 5, 2), (p.MomentumCost, p.MomentumBefore, p.MomentumAfterCost));
    }

    [Fact]
    public void TheNextTurnLandsWhereExecutionPutsIt()
    {
        var s = Board(1, out var a, out var t, "brawl_strike", "gust");
        s.Timeline.ScheduleTurn(t, 6, 5);
        s.Timeline.ScheduleTurn(a, 2, 5);
        foreach (var (skill, target) in new[] { ("brawl_strike", t), ("gust", t) })
        {
            var p = Preview.Skill(s, World.Catalog, Use(a, skill, target));
            var real = s.Clone();
            Actions.Use(real, World.Catalog, Use(a, skill, target));
            var turn = real.Timeline.TurnOf(a)!;
            Assert.Equal(turn.Due, p.NextTurnDue);
            Assert.Equal(real.Timeline.Upcoming().ToList().FindIndex(e => e.Id == turn.Id), p.NextTurnPosition);
        }
        var skip = Preview.SkipTurn(s, a);
        Assert.Equal(s.Timeline.Now + Timeline.SkippedTurnWait(5), skip.Due);
    }

    [Fact]
    public void OtherTimelineChangesAreListed()
    {
        var s = new BattleState(1);
        var leader = World.Unit(s, Side.A, Row.Front, "press_the_advantage", "swap_turns");
        var a = World.Unit(s, Side.A, Row.Middle);
        var b = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 5);
        s.Timeline.ScheduleTurn(a, 2, 5);
        s.Timeline.ScheduleTurn(b, 9, 5);
        var p = Preview.Skill(s, World.Catalog, new SkillUse(leader, "swap_turns", a, null, null, b));
        Assert.Equal(2, p.TimelineShifts.Count);
        Assert.Contains(p.TimelineShifts, x => x.UnitId == a && x.To > x.From);
        Assert.Contains(p.TimelineShifts, x => x.UnitId == b && x.To < x.From);
    }

    [Fact]
    public void AForcedMovePreviewShowsTheLandingAndABlockedChainShowsNothingMovingAndTheBumpDamage()
    {
        var s = Board(1, out var a, out var t, "gust");
        var open = Preview.Skill(s, World.Catalog, Use(a, "gust", t));
        var move = Assert.Single(open.Moves);
        Assert.Equal((t, Row.Middle), (move.UnitId, move.To.Row));
        Assert.Empty(open.BlockedChains);
        var real = s.Clone();
        Actions.Use(real, World.Catalog, Use(a, "gust", t));
        Assert.Equal(real.Formation.PositionOf(t), move.To);                          // the executed landing is the previewed one

        var blocked = Board(1, out var a2, out var t2, "gust");
        for (var i = 0; i < 6; i++) World.Unit(blocked, Side.B, Row.Middle);           // full Middle and Rear rows: the chain cannot end
        for (var i = 0; i < 6; i++) World.Unit(blocked, Side.B, Row.Rear);
        var p = Preview.Skill(blocked, World.Catalog, Use(a2, "gust", t2));
        Assert.Empty(p.Moves);
        Assert.Single(p.BlockedChains);
        Assert.Contains(t2, p.BumpDamaged);
        Assert.Equal(1, p.Units.Single(u => u.UnitId == t2).DamageMin);
    }

    [Fact]
    public void AnIllegalUseOnlyReportsTheReason()
    {
        var s = Board(1, out var a, out var t, "brawl_strike");
        var p = Preview.Skill(s, World.Catalog, Use(a, "mend", t));
        Assert.False(p.IsLegal);
        Assert.NotNull(p.IllegalReason);
        Assert.Empty(p.Units);
    }
}
