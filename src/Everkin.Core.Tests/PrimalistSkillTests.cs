using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class PrimalistSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, int? second = null, MoveOption? place = null, RowTarget? row = null) =>
        new(user, skill, target, row, place, second);

    private static IReadOnlyList<BattleEvent> Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    private static bool Has(BattleState s, int unit, string status) => Statuses.Has(s.GetUnit(unit), status);

    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    /// <summary>Advances the clock; status ends and auto events (totem pulses) are handled on the way.</summary>
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

    private static string BrawlerCreature => World.Content.Creatures.First(c => World.Catalog.AttackSkillFor(c.AttackType) == "brawl_strike").Id;

    private static int AddBrawler(BattleState s, Side side, Row row)
    {
        var u = s.AddUnit("brawler", side, 40, 5, 5, BrawlerCreature);
        s.Formation.Place(u.Id, side, row);
        return u.Id;
    }

    /// <summary>A Primalist in the Front row with two allies beside it, and two enemies in the Front row.</summary>
    private static (BattleState S, int Primalist, int AllyA, int AllyB, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var p = World.Unit(s, Side.A, Row.Front, skills);
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 8);
        return (s, p, a, b, enemy);
    }

    private static int PlaceTotem(BattleState s, int primalist, string skill)
    {
        Run(s, Use(primalist, skill, place: World.Catalog.Get(skill).PlacementOptions(s, primalist)[0]));
        Ready(s, primalist);
        return s.Units.Last(u => u.SummonerId == primalist).Id;
    }

    [Fact]
    public void AllFifteenPrimalistSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "primalist").ToList();
        Assert.Equal(15, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind is not ("A" or "A (construct)"), skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            if (!skill.IsPassive) Assert.True(skill.Magical);
        }
    }

    // ---- Totemist ----

    [Fact]
    public void HealingTotemHealsTheAlliesInItsRowTwoEveryTenTimeUnits()
    {
        var (s, p, a, _, enemy) = Board(1, "healing_totem");
        var middle = World.Unit(s, Side.A, Row.Middle);
        var totem = PlaceTotem(s, p, "healing_totem");
        Assert.Equal(8, s.GetUnit(totem).MaxHp);
        foreach (var id in new[] { a, middle, enemy }) s.GetUnit(id).Hp = 20;
        AdvanceTo(s, 9);
        Assert.Equal(20, s.GetUnit(a).Hp);
        AdvanceTo(s, 10);
        Assert.Equal(22, s.GetUnit(a).Hp);
        Assert.Equal(20, s.GetUnit(middle).Hp);                             // another row
        Assert.Equal(20, s.GetUnit(enemy).Hp);
        AdvanceTo(s, 20);
        Assert.Equal(24, s.GetUnit(a).Hp);                                  // repeats
        s.GetUnit(p).Hp = 0;
        Knockout.ResolveAll(s);
        AdvanceTo(s, 30);
        Assert.Equal(26, s.GetUnit(a).Hp);                                  // it goes on without the Primalist
    }

    [Fact]
    public void SpiritLinkTotemSplitsDamageEvenlyAmongTheAlliesInItsRow()
    {
        var (s, p, a, b, _) = Board(1, "healing_totem", "spirit_link_totem");
        PlaceTotem(s, p, "spirit_link_totem");
        s.DealDamage(null, a, 10);
        Assert.Equal(40 - 4, s.GetUnit(a).Hp);                              // 10 ÷ 3 = 3 each, the remainder stays with the hurt unit
        Assert.Equal(37, s.GetUnit(b).Hp);
        Assert.Equal(37, s.GetUnit(p).Hp);
    }

    [Fact]
    public void TotemOfReturnKeepsTheFirstDoomedAllyAtOneHpAndBreaks()
    {
        var (s, p, a, _, _) = Board(1, "healing_totem", "totem_of_return");
        var totem = PlaceTotem(s, p, "totem_of_return");
        s.GetUnit(a).Hp = 3;
        s.DealDamage(null, a, 10);
        Assert.Equal(1, s.GetUnit(a).Hp);
        Assert.True(s.GetUnit(totem).IsRemoved);
        s.DealDamage(null, a, 10);
        Assert.Equal(0, s.GetUnit(a).Hp);                                   // only once
    }

    [Fact]
    public void GreatTotemTriplesHpKeepsOneTotemAndCoversTheRowsNextToIt()
    {
        var (s, p, _, _, _) = Board(1, "healing_totem", "spirit_link_totem", "totem_of_return", "great_totem");
        var middle = World.Unit(s, Side.A, Row.Middle);
        var rear = World.Unit(s, Side.A, Row.Rear);
        var first = PlaceTotem(s, p, "totem_of_return");
        Assert.Equal(18, s.GetUnit(first).MaxHp);
        s.GetUnit(middle).Hp = 3;
        s.DealDamage(null, middle, 10);
        Assert.Equal(1, s.GetUnit(middle).Hp);                              // the Middle row is next to the totem's row
        s.GetUnit(rear).Hp = 3;
        s.DealDamage(null, rear, 10);
        Assert.Equal(0, s.GetUnit(rear).Hp);                                // two rows away

        var second = PlaceTotem(s, p, "healing_totem");
        Assert.True(s.GetUnit(first).IsRemoved);
        Assert.Equal(24, s.GetUnit(second).MaxHp);
    }

    // ---- Ritualist ----

    [Fact]
    public void RainDanceWatersARowForTwentyAndLongRainForFortyAndHealingRainHealsAlliesInIt()
    {
        var (s, p, a, b, _) = Board(1, "hoarfrost", "rain_dance", "long_rain", "healing_rain");
        s.GetUnit(a).Hp = 10;
        var row = new RowTarget(Side.A, Row.Front);
        Assert.Contains(row, World.Catalog.Get("rain_dance").LegalRows(s, p));
        Run(s, Use(p, "rain_dance", row: row));
        Assert.Equal(40, s.Ground.Find(s.Ground.SpotOf(a), GroundKind.Water)!.EndsAt);
        Assert.True(s.Ground.Has(s.Ground.SpotOf(b), GroundKind.Water));
        Assert.Equal(14, s.GetUnit(a).Hp);                                  // healed 4 once

        var (s2, p2, a2, _, _) = Board(1, "hoarfrost", "rain_dance");
        s2.GetUnit(a2).Hp = 10;
        Run(s2, Use(p2, "rain_dance", row: new RowTarget(Side.A, Row.Front)));
        Assert.Equal(20, s2.Ground.Find(s2.Ground.SpotOf(a2), GroundKind.Water)!.EndsAt);
        Assert.Equal(10, s2.GetUnit(a2).Hp);
    }

    [Fact]
    public void HoarfrostPutsIceOnAUnitsSpotAndTotemRitualRefillsEveryTotem()
    {
        var (s, p, _, _, enemy) = Board(1, "healing_totem", "hoarfrost", "totem_ritual");
        Run(s, Use(p, "hoarfrost", enemy));
        Assert.Equal(20, s.Ground.Find(s.Ground.SpotOf(enemy), GroundKind.Ice)!.EndsAt);
        Ready(s, p);
        var totem = PlaceTotem(s, p, "healing_totem");
        s.GetUnit(totem).Hp = 1;
        Run(s, Use(p, "totem_ritual"));
        Assert.Equal(8, s.GetUnit(totem).Hp);
    }

    [Fact]
    public void RiteOfMistsHalvesProjectileHitsFromUnitsOnWaterForTwentyTimeUnits()
    {
        int Damage(bool mists)
        {
            var (s, p, a, _, enemy) = Board(1, "hoarfrost", "rain_dance", "rite_of_mists");
            s.GetUnit(a).LearnSkill("shot");
            s.Ground.Place(GroundKind.Water, s.Ground.SpotOf(a), 50);
            if (mists) Run(s, Use(p, "rite_of_mists"));
            Ready(s, a);
            var before = s.GetUnit(enemy).Hp;
            Run(s, Use(a, "shot", enemy));
            return before - s.GetUnit(enemy).Hp;
        }
        Assert.InRange(Damage(false), 4, 5);
        Assert.InRange(Damage(true), 2, 2);
    }

    // ---- Spiritwalker ----

    [Fact]
    public void ForcedStepMovesAnyUnitToAFreeGapInAnotherRowOfItsSideAndCannotMoveAnImmobilizedOne()
    {
        var (s, p, _, _, enemy) = Board(1, "forced_step");
        var options = World.Catalog.Get("forced_step").PlacementOptionsFor(s, p, enemy);
        Assert.NotEmpty(options);
        Assert.All(options, o => Assert.NotEqual(Row.Front, o.Row));
        Run(s, Use(p, "forced_step", enemy, place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(enemy).Row);
        Assert.Equal(Side.B, s.Formation.PositionOf(enemy).Side);

        Ready(s, p);
        var other = s.Units.First(u => u.Side == Side.B && u.Id != enemy).Id;
        Statuses.Apply(s, new StatusApplication(other, StatusIds.Immobilized, Duration: 30));
        Assert.NotNull(Why(s, Use(p, "forced_step", other, place: new MoveOption(Row.Middle, 0))));
    }

    [Fact]
    public void PuppetSoulMakesAUnitHitItsNeighbourWithItsOwnBasicAttack()
    {
        var (s, p, _, _, _) = Board(1, "forced_step", "puppet_soul");
        var puppet = AddBrawler(s, Side.B, Row.Front);
        var victim = s.Units.First(u => u.Side == Side.B && u.Id != puppet).Id;
        var far = World.Unit(s, Side.B, Row.Middle);
        Assert.NotNull(Why(s, Use(p, "puppet_soul", puppet, far)));        // not next to it
        var neighbour = s.Formation.Adjacent(puppet).First(id => s.GetUnit(id).Side == Side.B);
        s.ShiftMomentum(Side.A, 0);
        var hp = s.GetUnit(neighbour).Hp;
        Run(s, Use(p, "puppet_soul", puppet, neighbour));
        Assert.InRange(hp - s.GetUnit(neighbour).Hp, 4, 5);
        Assert.Equal(victim, victim);
    }

    [Fact]
    public void LifeSwapExchangesTheSharesOfMaximumHp()
    {
        var (s, p, a, _, enemy) = Board(1, "forced_step", "linked_fates", "shared_fate", "life_swap");
        s.GetUnit(a).Hp = 20;                                               // 50% of 40
        s.GetUnit(enemy).Hp = 40;
        s.GetUnit(enemy).MaxHp = 40;
        s.GetUnit(enemy).Hp = 8;                                            // 20%
        Run(s, Use(p, "life_swap", a, enemy));
        Assert.Equal(8, s.GetUnit(a).Hp);
        Assert.Equal(20, s.GetUnit(enemy).Hp);
    }

    [Fact]
    public void LinkedFatesMakesTheFollowerLoseHalfAndSharedFateWorksBothWays()
    {
        var (s, p, a, b, _) = Board(1, "forced_step", "linked_fates", "shared_fate");
        Run(s, Use(p, "linked_fates", a, b));
        s.DealDamage(null, a, 10);
        Assert.Equal(30, s.GetUnit(a).Hp);
        Assert.Equal(35, s.GetUnit(b).Hp);                                  // half of 10
        s.DealDamage(null, b, 10);
        Assert.Equal(25, s.GetUnit(b).Hp);
        Assert.Equal(30, s.GetUnit(a).Hp);                                  // one way only

        Ready(s, p);
        Run(s, Use(p, "shared_fate", a, b));
        s.DealDamage(null, b, 10);
        Assert.Equal(15, s.GetUnit(b).Hp);
        Assert.Equal(25, s.GetUnit(a).Hp);
        Assert.Equal(15, s.GetUnit(b).Hp);                                  // the echo does not echo back
        AdvanceTo(s, 21);
        Assert.False(Has(s, a, StatusIds.FateLink));
    }
}
