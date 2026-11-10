using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class ElementalistSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null) => new(user, skill, target, row);

    private static void Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    private static bool Has(BattleState s, int unit, string status) => Statuses.Has(s.GetUnit(unit), status);

    private static void AdvanceTo(BattleState s, int time)
    {
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            Statuses.HandleExpiry(s, e);
        }
    }

    private static (BattleState S, int Mage) Board(ulong seed, params string[] skills)
    {
        var s = new BattleState(seed);
        var mage = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.A, Row.Middle);
        s.ShiftMomentum(Side.A, 6);
        return (s, mage);
    }

    private static bool Burns(BattleState s, int unit) => s.Ground.Has(s.Ground.SpotOf(unit), GroundKind.Burning);

    [Fact]
    public void AllSeventeenElementalistSkillsAreImplementedAndEveryActiveIsMagical()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "elementalist").ToList();
        Assert.Equal(17, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            if (def.Kind == "A") Assert.True(skill.Magical);
        }
    }

    // ---- Pyromancy ----

    [Fact]
    public void KindleSetsFireOnTheUnitsSpotForTwentyTimeUnits()
    {
        var (s, mage) = Board(1, "kindle");
        var enemy = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "kindle", enemy));
        Assert.True(Burns(s, enemy));
        Assert.Equal(20, s.Ground.Find(s.Ground.SpotOf(enemy), GroundKind.Burning)!.EndsAt);
        AdvanceTo(s, 21);
        Assert.False(Burns(s, enemy));
    }

    [Fact]
    public void ScorchSpreadsKindleToTheSpotsNextToIt()
    {
        var (s, mage) = Board(1, "kindle", "scorch");
        var left = World.Unit(s, Side.B, Row.Front);
        var mid = World.Unit(s, Side.B, Row.Front);
        var right = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "kindle", mid));
        Assert.True(Burns(s, left) && Burns(s, mid) && Burns(s, right));
    }

    [Fact]
    public void KindleWithoutScorchBurnsOnlyOneSpot()
    {
        var (s, mage) = Board(1, "kindle");
        var left = World.Unit(s, Side.B, Row.Front);
        var mid = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "kindle", mid));
        Assert.True(Burns(s, mid));
        Assert.False(Burns(s, left));
    }

    [Fact]
    public void RowOfFireBurnsTheSpotOfEveryUnitInTheRow()
    {
        var (s, mage) = Board(1, "row_of_fire");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        var other = World.Unit(s, Side.B, Row.Middle);
        var row = new RowTarget(Side.B, Row.Front);
        Assert.Contains(row, World.Catalog.Get("row_of_fire").LegalRows(s, mage));
        Run(s, Use(mage, "row_of_fire", row: row));
        Assert.True(Burns(s, a) && Burns(s, b));
        Assert.False(Burns(s, other));
    }

    [Fact]
    public void FireLaneBurnsTheSameColumnInEveryRowOfTheSide()
    {
        var (s, mage) = Board(1, "fire_lane");
        var front = World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        var rear = World.Unit(s, Side.B, Row.Rear);
        Run(s, Use(mage, "fire_lane", front));
        Assert.True(Burns(s, front) && Burns(s, middle) && Burns(s, rear));
    }

    [Fact]
    public void BackdraftNeedsBurningGroundAndSpreadsWithTheRemainingTime()
    {
        var (s, mage) = Board(1, "kindle", "backdraft", "ignition_point");
        var left = World.Unit(s, Side.B, Row.Front);
        var mid = World.Unit(s, Side.B, Row.Front);
        Assert.NotNull(Actions.WhyNot(s, World.Catalog, Use(mage, "backdraft", mid)));    // not burning yet
        Run(s, Use(mage, "kindle", mid));
        AdvanceTo(s, 8);
        Assert.Null(Actions.WhyNot(s, World.Catalog, Use(mage, "backdraft", mid)));
        Ready(s, mage);
        Run(s, Use(mage, "backdraft", mid));
        Assert.True(Burns(s, left));
        Assert.Equal(28, s.Ground.Find(s.Ground.SpotOf(mid), GroundKind.Burning)!.EndsAt);   // Ignition Point refreshed to 20
        Assert.Equal(28, s.Ground.Find(s.Ground.SpotOf(left), GroundKind.Burning)!.EndsAt);
    }

    [Fact]
    public void BackdraftWithoutIgnitionPointDoesNotRefreshAndLeavesBurningNeighbours()
    {
        var (s, mage) = Board(1, "kindle", "backdraft");
        var left = World.Unit(s, Side.B, Row.Front);
        var mid = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "kindle", left));
        AdvanceTo(s, 5);
        Ready(s, mage);
        Run(s, Use(mage, "kindle", mid));
        AdvanceTo(s, 10);
        Ready(s, mage);
        Run(s, Use(mage, "backdraft", mid));
        Assert.Equal(25, s.Ground.Find(s.Ground.SpotOf(mid), GroundKind.Burning)!.EndsAt);
        Assert.Equal(20, s.Ground.Find(s.Ground.SpotOf(left), GroundKind.Burning)!.EndsAt);   // not restarted
    }

    // ---- Stormcalling ----

    [Fact]
    public void ShockStunsForTenTimeUnits()
    {
        var (s, mage) = Board(1, "shock");
        var enemy = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "shock", enemy));
        Assert.Equal(10, Statuses.Get(s.GetUnit(enemy), StatusIds.Stunned)!.EndsAt);
    }

    [Fact]
    public void LightningOnAUnitInWaterAlsoStunsTheUnitsOnTouchingWaterForHalf()
    {
        var (s, mage) = Board(1, "shock");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        var c = World.Unit(s, Side.B, Row.Front);
        foreach (var id in new[] { a, b }) s.Ground.Place(GroundKind.Water, s.Ground.SpotOf(id), 20);
        Run(s, Use(mage, "shock", a));
        Assert.Equal(10, Statuses.Get(s.GetUnit(a), StatusIds.Stunned)!.EndsAt);
        Assert.Equal(5, Statuses.Get(s.GetUnit(b), StatusIds.Stunned)!.EndsAt);
        Assert.False(Has(s, c, StatusIds.Stunned));
    }

    [Fact]
    public void ChainLightningStunsFiveAndJumpsTwiceToUnitsNotYetHit()
    {
        for (ulong seed = 1; seed <= 20; seed++)
        {
            var (s, mage) = Board(seed, "chain_lightning");
            var ids = Enumerable.Range(0, 5).Select(_ => World.Unit(s, Side.B, Row.Front)).ToList();
            Run(s, Use(mage, "chain_lightning", ids[2]));
            var stunned = ids.Where(id => Has(s, id, StatusIds.Stunned)).ToList();
            Assert.Equal(3, stunned.Count);
            Assert.Contains(ids[2], stunned);
            Assert.All(stunned, id => Assert.Equal(5, Statuses.Get(s.GetUnit(id), StatusIds.Stunned)!.EndsAt));
        }
    }

    [Fact]
    public void ChainLightningEndsEarlyWhenNobodyIsLeftAndArcAddsTwoJumps()
    {
        var (s, mage) = Board(1, "chain_lightning");
        var only = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "chain_lightning", only));
        Assert.True(Has(s, only, StatusIds.Stunned));

        var (s2, mage2) = Board(1, "chain_lightning", "arc");
        var row = Enumerable.Range(0, 6).Select(_ => World.Unit(s2, Side.B, Row.Front)).ToList();
        Run(s2, Use(mage2, "chain_lightning", row[0]));
        Assert.Equal(5, row.Count(id => Has(s2, id, StatusIds.Stunned)));
    }

    [Fact]
    public void TempestPushesEveryUnitInARowOneRowBackWithoutDamage()
    {
        var (s, mage) = Board(1, "tempest");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        var rowTarget = new RowTarget(Side.B, Row.Front);
        Assert.Contains(rowTarget, World.Catalog.Get("tempest").LegalRows(s, mage));
        Run(s, Use(mage, "tempest", row: rowTarget));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(a).Row);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(b).Row);
        Assert.Equal(40, s.GetUnit(a).Hp);
        Assert.Equal(40, s.GetUnit(b).Hp);
    }

    [Fact]
    public void MegavoltStunsARowAndSilencesTheElementalist()
    {
        var (s, mage) = Board(1, "megavolt", "shock");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(mage, "megavolt", row: new RowTarget(Side.B, Row.Front)));
        Assert.Equal(5, Statuses.Get(s.GetUnit(a), StatusIds.Stunned)!.EndsAt);
        Assert.Equal(5, Statuses.Get(s.GetUnit(b), StatusIds.Stunned)!.EndsAt);
        Assert.Equal(10, Statuses.Get(s.GetUnit(mage), StatusIds.Silenced)!.EndsAt);
        Assert.NotNull(Actions.WhyNot(s, World.Catalog, Use(mage, "shock", a)));
    }

    // ---- Cryomancy ----

    [Fact]
    public void FreezeFreezesAUnitAndItsTurnIsSkipped()
    {
        var (s, mage) = Board(1, "freeze");
        var enemy = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "freeze", enemy));
        Assert.True(Has(s, enemy, StatusIds.Frozen));
        Assert.True(Statuses.OnTurnStart(s, enemy).TurnSkipped);
        Assert.False(Has(s, enemy, StatusIds.Frozen));
    }

    [Fact]
    public void GlacierFreezesAWholeRowAndFrostNovaTheEnemiesInMeleeReach()
    {
        var (s, mage) = Board(1, "glacier", "frost_nova");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        var back = World.Unit(s, Side.B, Row.Middle);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(mage, "frost_nova"));
        Assert.True(Has(s, a, StatusIds.Frozen) && Has(s, b, StatusIds.Frozen));
        Assert.False(Has(s, back, StatusIds.Frozen));
        Ready(s, mage);
        Run(s, Use(mage, "glacier", row: new RowTarget(Side.B, Row.Middle)));
        Assert.True(Has(s, back, StatusIds.Frozen));
    }

    [Fact]
    public void FireEndsFrozenAndBrittleAddsHalfDamageToFrozenEnemies()
    {
        var (s, mage) = Board(1, "freeze", "kindle", "brittle");
        BattleStart.Install(s, World.Catalog);
        var ally = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var enemy = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(ally, "brawl_strike", enemy));
        var normal = 40 - s.GetUnit(enemy).Hp;
        Assert.InRange(normal, 4, 5);
        s.GetUnit(enemy).Hp = 40;
        Run(s, Use(mage, "freeze", enemy));
        Ready(s, ally);
        Run(s, Use(ally, "brawl_strike", enemy));
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 6, 7);                        // 4–5 × 3 ÷ 2 rounded down

        Run(s, Use(mage, "freeze", enemy));
        Ready(s, mage);
        Run(s, Use(mage, "kindle", enemy));
        Assert.False(Has(s, enemy, StatusIds.Frozen));
    }

    [Fact]
    public void ColdHeartFreezesForTwoTurnsAndKeepsOnlyOneUnitFrozen()
    {
        var (s, mage) = Board(1, "freeze", "frost_nova", "cold_heart");
        var a = World.Unit(s, Side.B, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(mage, "freeze", a));
        Assert.True(Statuses.OnTurnStart(s, a).TurnSkipped);
        Assert.True(Has(s, a, StatusIds.Frozen));                              // one more turn left
        Assert.True(Statuses.OnTurnStart(s, a).TurnSkipped);
        Assert.False(Has(s, a, StatusIds.Frozen));

        s.ShiftMomentum(Side.A, 10);
        Run(s, Use(mage, "freeze", a));
        Ready(s, mage);
        s.ShiftMomentum(Side.A, 10);
        Run(s, Use(mage, "freeze", b));
        Assert.False(Has(s, a, StatusIds.Frozen));
        Assert.True(Has(s, b, StatusIds.Frozen));

        Ready(s, mage);
        s.ShiftMomentum(Side.A, 10);
        Run(s, Use(mage, "frost_nova"));                                       // both in reach: the roll keeps one
        Assert.Equal(1, new[] { a, b }.Count(id => Has(s, id, StatusIds.Frozen)));
    }
}
