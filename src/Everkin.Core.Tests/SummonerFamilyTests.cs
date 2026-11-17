using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

/// <summary>The Necromancer, Summoner and Totemist.</summary>
public class SummonerFamilyTests
{
    [Fact]
    public void AllThreeClassesAreImplemented()
    {
        AssertClassImplemented("necromancer", magical: true);
        AssertClassImplemented("summoner", magical: true);
        AssertClassImplemented("totemist", magical: true);
    }

    private static void Ko(BattleState s, int unit)
    {
        s.GetUnit(unit).Hp = 0;
        Knockout.Resolve(s, unit);
        s.DrainEvents();
    }

    // ---- Necromancer ----

    private static (BattleState S, int Necro, int Body) Graveyard(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var n = World.Unit(s, Side.A, Row.Front, skills);
        var body = s.AddUnit(World.Def("ankylosaur-guardian-club", hp: 40, speed: 4, power: 8), Side.B);
        s.Formation.Place(body.Id, Side.B, Row.Front);
        Ko(s, body.Id);
        World.Unit(s, Side.B, Row.Middle);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, n, body.Id);
    }

    [Fact]
    public void RaiseUndeadTurnsAKoBodyOfEitherSideIntoAnUndeadOfYoursWithTwelveHpAndTheNecromancersPower()
    {
        var (s, n, body) = Graveyard(1, "raise_undead");
        s.GetUnit(n).Power = 7;
        Run(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Middle, 0)));
        var undead = s.GetUnit(body);
        Assert.True(undead.IsUndead);
        Assert.Equal((Side.A, 12, 12, Row.Middle), (undead.Side, undead.Hp, undead.MaxHp, s.Formation.PositionOf(body).Row));
        Assert.Equal(7, s.PowerOf(body));                                                    // the Necromancer's current Power, not the creature's own
        s.GetUnit(n).Power = 3;
        Assert.Equal(3, s.PowerOf(body));
        Assert.Equal(4, s.GetUnit(body).Speed);                                              // it keeps its own Speed
        Assert.Equal("tail_club", s.GetUnit(body).AutoSkillId);                              // and its own unique attack
        Assert.Equal(4, s.Momentum.Value);  // M5.10 costs
    }

    [Fact]
    public void RaiseUndeadNeedsAKoBodyAndAFreeSpotOnTheNecromancersSide()
    {
        var s = new BattleState(1);
        var n = World.Unit(s, Side.A, Row.Front, "raise_undead");
        var alive = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Assert.NotNull(Why(s, Use(n, "raise_undead", alive, place: new MoveOption(Row.Middle, 0))));     // a living unit is not a body
        var body = World.Unit(s, Side.B, Row.Middle);
        Ko(s, body);
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.Count(Side.A, row) < 6) World.Unit(s, Side.A, row);
        Assert.Equal(18, s.Formation.Count(Side.A));
        Assert.NotNull(Why(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Middle, 0))));      // no free spot
    }

    [Fact]
    public void RaiseUndeadAlsoRaisesOneMoreRandomBodyOnARandomFreeSpot()
    {
        var (s, n, body) = Graveyard(1, "raise_undead");
        var other = s.AddUnit(World.Def("ant-engineer-gear"), Side.B);
        s.Formation.Place(other.Id, Side.B, Row.Rear);
        Ko(s, other.Id);
        Run(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Middle, 0)));
        Assert.True(s.GetUnit(body).IsUndead);
        Assert.True(s.GetUnit(other.Id).IsUndead);
        Assert.Equal(Side.A, s.GetUnit(other.Id).Side);
    }

    [Fact]
    public void AKoUndeadsBodyCanBeRaisedAgainAndHealingAnUndeadHurtsIt()
    {
        var (s, n, body) = Graveyard(1, "raise_undead", "mend");
        Run(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Middle, 0)));
        s.Heal(null, body, 6);
        Assert.Equal(6, s.GetUnit(body).Hp);                                                 // 12 - 6: healing damaged it
        Ko(s, body);
        Assert.True(s.GetUnit(body).IsKnockedOut);
        K.Ready(s, n);
        s.ShiftMomentum(Side.A, 4);
        Assert.Null(Why(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Rear, 0))));
    }

    [Fact]
    public void LichFormMakesHealingHurtTheNecromancerAndGivesItsUndeadPlusTwoPower()
    {
        var (s, n, body) = Graveyard(1, "raise_undead", "lich_form");
        s.GetUnit(n).Hp = 20;
        s.Heal(null, n, 5);
        Assert.Equal(15, s.GetUnit(n).Hp);
        Run(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Middle, 0)));
        Assert.Equal(7, s.PowerOf(body));                                                    // 5 + 2
    }

    [Fact]
    public void CorpseExplosionBurstsABodyIntoPoisonGasOnItsSpotAndTheSpotsNextToItWithoutDamagingIt()
    {
        var (s, n, body) = Graveyard(1, "corpse_explosion");
        var neighbour = World.Unit(s, Side.B, Row.Front);
        var spot = s.Ground.SpotOf(body);
        Run(s, Use(n, "corpse_explosion", body));
        Assert.True(s.Ground.Has(spot, GroundKind.Poison));
        Assert.True(Has(s, neighbour, StatusIds.Poison));
        Assert.True(s.GetUnit(body).IsKnockedOut);
        Assert.Equal(0, s.GetUnit(body).Hp);
        Assert.Equal(Row.Front, s.Formation.PositionOf(body).Row);                          // the body stays where it is
    }

    [Fact]
    public void DesecrateBlocksEveryReviveOfABodyFor40ButNotRaiseUndead()
    {
        var (s, n, body) = Graveyard(1, "desecrate", "raise_undead");
        var life = World.Unit(s, Side.A, Row.Middle, "life_for_life");
        s.GetUnit(life).Hp = 20;
        Run(s, Use(n, "desecrate", body));
        Assert.NotNull(Why(s, Use(life, "life_for_life", body)));
        AdvanceTo(s, 41);
        Assert.Null(Why(s, Use(life, "life_for_life", body)));
        K.Ready(s, n);
        Assert.Null(Why(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Rear, 0))));
    }

    [Fact]
    public void BoneArmorGivesAWardOfTwoPerUndeadAtEveryTurnOfTheNecromancer()
    {
        var (s, n, body) = Graveyard(1, "raise_undead", "bone_armor");
        Run(s, Use(n, "raise_undead", body, place: new MoveOption(Row.Middle, 0)));
        K.Ready(s, n);
        s.Timeline.ScheduleTurn(n, 1, 10);
        Assert.Equal(n, BattleDriver.NextTurn(s));
        Assert.Equal(2, s.GetUnit(n).Ward);                                                  // one undead: Ward 2
        s.GetUnit(n).Ward = 0;
        Statuses.Remove(s, n, StatusIds.Ward, StatusEndCause.Broken);
        Ko(s, body);
        K.Ready(s, n);
        s.Timeline.ScheduleTurn(n, 1, 10);
        BattleDriver.NextTurn(s);
        Assert.Equal(0, s.GetUnit(n).Ward);                                                  // no undead left standing
    }

    [Fact]
    public void DeathsBountyHealsTheNecromancerFiveWhenAUnitNextToItIsKo()
    {
        var s = new BattleState(1);
        var n = World.Unit(s, Side.A, Row.Front, "deaths_bounty");
        var next = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(n).Hp = 20;
        s.GetUnit(far).Hp = 0;
        Actions.Settle(s, null);
        Assert.Equal(20, s.GetUnit(n).Hp);
        s.GetUnit(next).Hp = 0;
        Actions.Settle(s, null);
        Assert.Equal(25, s.GetUnit(n).Hp);
    }

    // ---- Summoner ----

    [Fact]
    public void ClayGolemSummonsAGolemOfHp25AndSpeed3OnAnyFreeSpotWhichTakesItsFirstTurnElevenMinusSpeedLater()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "clay_golem");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        AdvanceTo(s, 10);
        Run(s, Use(c, "clay_golem", place: new MoveOption(Row.Rear, 0)));
        var golem = s.Units.Single(u => u.CreatureId == "clay_golem");
        Assert.Equal((25, 3, Row.Rear), (golem.Hp, golem.Speed, s.Formation.PositionOf(golem.Id).Row));
        Assert.Equal(18, s.Timeline.TurnOf(golem.Id)!.Due);                                  // 20.5: appears at 10, first acts at 10 + 8
        Assert.Equal(4, s.Momentum.Value);
    }

    [Fact]
    public void AGolemAttacksTheNearestEnemyWithAMeleeHitOnItsOwnTurnAndTakesSkipTurnOutsideTheFrontRow()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "clay_golem");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(c, "clay_golem", place: new MoveOption(Row.Front, 0)));
        var golem = s.Units.Single(u => u.CreatureId == "clay_golem");
        BattleDriver.NextTurn(s, World.Catalog);
        Assert.InRange(Lost(s, e), 5, 6);

        var s2 = new BattleState(1);
        var c2 = World.Unit(s2, Side.A, Row.Front, "clay_golem");
        World.Unit(s2, Side.B, Row.Front);
        BattleStart.Install(s2, World.Catalog);
        s2.ShiftMomentum(Side.A, 6);
        Run(s2, Use(c2, "clay_golem", place: new MoveOption(Row.Middle, 0)));                // 20.5: a golem in the Middle row has no legal target
        var g2 = s2.Units.Single(u => u.CreatureId == "clay_golem");
        Assert.Equal(c2, BattleDriver.NextTurn(s2, World.Catalog));
        Assert.InRange(Lost(s2, s2.Formation.Units(Side.B, Row.Front)[0]), 0, 0);
    }

    [Fact]
    public void RuneGolemHasHp18Speed4AndItsHitSilencesTheTargetForTen()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "rune_golem");
        var e = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(c, "rune_golem", place: new MoveOption(Row.Front, 0)));
        var golem = s.Units.Single(u => u.CreatureId == "rune_golem");
        Assert.Equal((18, 4), (golem.Hp, golem.Speed));
        Assert.Equal(c, BattleDriver.NextTurn(s, World.Catalog));                            // the golem acted once on its own, then the Summoner is up
        Assert.InRange(Lost(s, e), 3, 4);
        Assert.Equal(17, Statuses.Get(s.GetUnit(e), StatusIds.Silenced)!.EndsAt);                // the golem hit at time 7: Silenced for 10
    }

    [Fact]
    public void AnimateBladesSummonsTwoSetsOfBladesThatLastThirty()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "animate_blades");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(c, "animate_blades", place: new MoveOption(Row.Middle, 0)));
        var blades = s.Units.Where(u => u.CreatureId == "animate_blades_unit").ToList();
        Assert.Equal(2, blades.Count);
        Assert.All(blades, b => Assert.Equal((6, 7), (b.Hp, b.Speed)));
        AdvanceTo(s, 31);
        Assert.All(blades, b => Assert.True(b.IsRemoved));                                  // gone without being KO
    }

    [Fact]
    public void GolemHeartKeepsOneGolemAndDoublesItsHp()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "clay_golem", "rune_golem", "golem_heart");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 8);
        Run(s, Use(c, "clay_golem", place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(50, s.Units.Single(u => u.CreatureId == "clay_golem").Hp);
        K.Ready(s, c);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(c, "rune_golem", place: new MoveOption(Row.Rear, 0)));
        Assert.True(s.Units.Single(u => u.CreatureId == "clay_golem").IsRemoved);            // replaced
        Assert.Equal(36, s.Units.Single(u => u.CreatureId == "rune_golem" && !u.IsRemoved).Hp);
    }

    [Fact]
    public void PortalPutsTheSummonOnARandomFreeSpotAndQuickeningGivesItsFirstTurnAtOnce()
    {
        var spots = new HashSet<Row>();
        for (ulong seed = 1; seed <= 30; seed++)
        {
            var s = new BattleState(seed);
            var c = World.Unit(s, Side.A, Row.Front, "clay_golem", "portal", "quickening");
            World.Unit(s, Side.B, Row.Front);
            BattleStart.Install(s, World.Catalog);
            s.ShiftMomentum(Side.A, 6);
            Assert.Null(Why(s, Use(c, "clay_golem")));                                       // no place asked
            Run(s, Use(c, "clay_golem"));
            var golem = s.Units.Single(u => u.CreatureId == "clay_golem");
            spots.Add(s.Formation.PositionOf(golem.Id).Row);
            Assert.Equal(s.Timeline.Now, s.Timeline.TurnOf(golem.Id)!.Due);                  // Quickening
        }
        Assert.True(spots.Count >= 2);
    }

    [Fact]
    public void ASummonFailsWhenTheSideHasEighteenUnits()
    {
        var s = new BattleState(1);
        var c = World.Unit(s, Side.A, Row.Front, "clay_golem");
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.Count(Side.A, row) < 6) World.Unit(s, Side.A, row);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 6);
        Assert.NotNull(Why(s, Use(c, "clay_golem", place: new MoveOption(Row.Front, 0))));
    }

    // ---- Totemist ----

    private static (BattleState S, int Totemist) TotemBoard(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var t = World.Unit(s, Side.A, Row.Rear, skills);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, t);
    }

    [Fact]
    public void HealingTotemHealsAlliesInItsRowEveryTenTimeUnitsAndKeepsGoingAfterTheTotemistIsKo()
    {
        var (s, t) = TotemBoard(1, "healing_totem");
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Middle);
        foreach (var id in new[] { a, b, other }) s.GetUnit(id).Hp = 10;
        Run(s, Use(t, "healing_totem", place: new MoveOption(Row.Front, 1)));
        var totem = s.Units.Single(u => u.CreatureId == "healing_totem");
        Assert.Equal(8, totem.Hp);
        s.GetUnit(t).Hp = 0;
        Actions.Settle(s, null);
        for (var i = 0; i < 4; i++) { var e = s.Timeline.Pop()!; if (e.Kind == TimelineKind.AutoEvent) TotemistSkills.HandleAutoEvent(s, e); if (s.Timeline.Now >= 10) break; }
        Assert.Equal(12, s.GetUnit(a).Hp);
        Assert.Equal(12, s.GetUnit(b).Hp);
        Assert.Equal(10, s.GetUnit(other).Hp);
        Assert.NotNull(s.Timeline.Upcoming().FirstOrDefault(x => x.Kind == TimelineKind.AutoEvent && x.OwnerUnitId == totem.Id));      // the pulse repeats
    }

    [Fact]
    public void SpiritLinkTotemSplitsDamageToAlliesInItsRowEvenlyTheHurtUnitKeepsTheRemainder()
    {
        var (s, t) = TotemBoard(1, "spirit_link_totem");
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.A, Row.Front);
        var c = World.Unit(s, Side.A, Row.Front);
        Run(s, Use(t, "spirit_link_totem", place: new MoveOption(Row.Front, 3)));
        s.DealDamage(null, a, 10);
        Assert.Equal(3, 40 - s.GetUnit(b).Hp);
        Assert.Equal(3, 40 - s.GetUnit(c).Hp);
        Assert.Equal(4, 40 - s.GetUnit(a).Hp);                                                 // 10 split three ways: 3 each, the hurt unit keeps the rest
        var totem = s.Units.Single(u => u.CreatureId == "spirit_link_totem");
        Assert.Equal(10, totem.Hp);                                                            // the totem does not share
    }

    [Fact]
    public void TotemOfReturnKeepsTheFirstAllyThatWouldBeKoAtOneHpAndBreaks()
    {
        var (s, t) = TotemBoard(1, "totem_of_return");
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.A, Row.Front);
        Run(s, Use(t, "totem_of_return", place: new MoveOption(Row.Front, 2)));
        var totem = s.Units.Single(u => u.CreatureId == "totem_of_return");
        Assert.Equal(6, totem.Hp);
        s.DealDamage(null, a, 99);
        Assert.Equal(1, s.GetUnit(a).Hp);
        Assert.True(totem.IsRemoved);                                                          // removed, not KO'd
        s.DealDamage(null, b, 99);
        Assert.True(s.GetUnit(b).IsKnockedOut);                                                // only the first one
    }

    [Fact]
    public void GreatTotemKeepsOneTotemWithTripleHpAndCoversTheRowsNextToIt()
    {
        var (s, t) = TotemBoard(1, "healing_totem", "spirit_link_totem", "great_totem");
        var front = World.Unit(s, Side.A, Row.Front);
        var mid = World.Unit(s, Side.A, Row.Middle);
        s.GetUnit(front).Hp = 10;
        s.GetUnit(mid).Hp = 10;
        Run(s, Use(t, "healing_totem", place: new MoveOption(Row.Front, 1)));
        var first = s.Units.Single(u => u.CreatureId == "healing_totem");
        Assert.Equal(24, first.Hp);
        Assert.True(TotemistSkills.Covers(s, first, mid));                                      // the Middle row is next to the Front row
        Assert.False(TotemistSkills.Covers(s, first, t));                                       // the Rear row is two away
        K.Ready(s, t);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(t, "spirit_link_totem", place: new MoveOption(Row.Front, 1)));
        Assert.True(first.IsRemoved);                                                           // replaced
        Assert.Single(TotemistSkills.TotemsOf(s, t));
    }

    [Fact]
    public void SturdyTotemsDoubleTheHpOfTheTotemistsTotems()
    {
        var (s, t) = TotemBoard(1, "healing_totem", "sturdy_totems");
        World.Unit(s, Side.A, Row.Front);
        Run(s, Use(t, "healing_totem", place: new MoveOption(Row.Front, 0)));
        Assert.Equal(16, s.Units.Single(u => u.CreatureId == "healing_totem").Hp);
    }

    [Fact]
    public void SpiritShieldGivesEveryAllyInTheRowAWardOfFourWhenATotemBreaks()
    {
        var (s, t) = TotemBoard(1, "healing_totem", "spirit_shield");
        var a = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Middle);
        Run(s, Use(t, "healing_totem", place: new MoveOption(Row.Front, 0)));
        var totem = s.Units.Single(u => u.CreatureId == "healing_totem");
        s.DealDamage(null, totem.Id, 99);
        Actions.Settle(s, null);
        Assert.True(totem.IsRemoved);
        Assert.Equal(4, s.GetUnit(a).Ward);
        Assert.Equal(0, s.GetUnit(other).Ward);
    }
}
