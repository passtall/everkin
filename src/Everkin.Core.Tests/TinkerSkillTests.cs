using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class TinkerSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null, MoveOption? place = null) =>
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
            Statuses.HandleExpiry(s, e);
        }
    }

    /// <summary>A Tinker in the Front row, an ally in the Middle row and an enemy in the Front row.</summary>
    private static (BattleState S, int Tinker, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var tinker = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 6);
        return (s, tinker, enemy);
    }

    private static int PlaceTurret(BattleState s, int tinker, string skill)
    {
        var place = World.Catalog.Get(skill).PlacementOptions(s, tinker)[0];
        Run(s, Use(tinker, skill, place: place));
        Ready(s, tinker);
        return s.Units.Last(u => u.SummonerId == tinker).Id;
    }

    [Fact]
    public void AllThirteenTinkerSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "tinker").ToList();
        Assert.Equal(13, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind is not ("A" or "A (construct)"), skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
        }
    }

    // ---- Engineer ----

    [Fact]
    public void BarricadeHasTenHpOrTwentyWithKeeperOfTheGateAndTakesAnyFreeGap()
    {
        var (s, tinker, _) = Board(1, "barricade");
        var places = World.Catalog.Get("barricade").PlacementOptions(s, tinker);
        Assert.Contains(new MoveOption(Row.Rear, 0), places);
        Run(s, Use(tinker, "barricade", place: new MoveOption(Row.Rear, 0)));
        var wall = s.Units.Last().Id;
        Assert.Equal(10, s.GetUnit(wall).MaxHp);
        Assert.Equal(UnitKind.Construct, s.GetUnit(wall).Kind);
        Assert.Equal(Row.Rear, s.Formation.PositionOf(wall).Row);

        var (s2, t2, _) = Board(1, "barricade");
        World.Unit(s2, Side.A, Row.Middle, "keeper_of_the_gate");
        BattleStart.Install(s2, World.Catalog);
        Run(s2, Use(t2, "barricade", place: new MoveOption(Row.Rear, 0)));
        Assert.Equal(20, s2.Units.Last().MaxHp);
    }

    [Fact]
    public void ABoltTurretShootsTheNearestEnemyForTwoToThreeAfterEachTinkerActionIncludingTheOneThatPlacedIt()
    {
        var (s, tinker, enemy) = Board(1, "barricade", "bolt_turret");
        var place = World.Catalog.Get("bolt_turret").PlacementOptions(s, tinker)[0];
        Assert.NotEqual(Row.Rear, place.Row);
        var momentum = s.Momentum.AdvantageOf(Side.A);
        var events = Run(s, Use(tinker, "bolt_turret", place: place));
        var turret = s.Units.Last().Id;
        Assert.Equal(8, s.GetUnit(turret).MaxHp);
        Assert.InRange(40 - s.GetUnit(enemy).Hp, 2, 3);                     // it exists when the action ends, so it shoots at once
        Assert.Contains(events, e => e is DamageDealt);
        Assert.Equal(momentum - 2 + 1, s.Momentum.AdvantageOf(Side.A));     // the skill's cost 2, then the shot's own shift of 1

        var hp = s.GetUnit(enemy).Hp;
        Ready(s, tinker);
        Run(s, Use(tinker, "barricade", place: new MoveOption(Row.Rear, 0)));
        Assert.InRange(hp - s.GetUnit(enemy).Hp, 2, 3);

        hp = s.GetUnit(enemy).Hp;
        Ready(s, tinker);
        Actions.SkipTurn(s, tinker);                                         // a skipped action also counts
        Assert.InRange(hp - s.GetUnit(enemy).Hp, 2, 3);
    }

    [Fact]
    public void ATinkerThatIsKoLosesQueuedShotsButTheTurretStays()
    {
        var (s, tinker, enemy) = Board(1, "barricade", "bolt_turret");
        var turret = PlaceTurret(s, tinker, "bolt_turret");
        var hp = s.GetUnit(enemy).Hp;
        s.Timeline.AddNow(tinker, Turrets.EventTagPrefix + turret);
        s.GetUnit(tinker).Hp = 0;
        Knockout.ResolveAll(s);
        Assert.DoesNotContain(s.Timeline.Upcoming(), e => e.Kind == TimelineKind.AutoEvent);
        Assert.False(s.GetUnit(turret).IsKnockedOut);
        Assert.Equal(hp, s.GetUnit(enemy).Hp);
    }

    [Fact]
    public void OverclockFiresEveryTurretOnceMoreNow()
    {
        var (s, tinker, enemy) = Board(1, "barricade", "bolt_turret", "overclock");
        PlaceTurret(s, tinker, "bolt_turret");
        PlaceTurret(s, tinker, "bolt_turret");
        var before = s.GetUnit(enemy).Hp;
        Run(s, Use(tinker, "overclock"));
        Assert.InRange(before - s.GetUnit(enemy).Hp, 8, 12);               // 2 turrets × 2 volleys × 2–3
    }

    [Fact]
    public void CutTheStringsCanCutAQueuedTurretShot()
    {
        var (s, tinker, enemy) = Board(1, "barricade", "bolt_turret");
        var turret = PlaceTurret(s, tinker, "bolt_turret");
        var hp = s.GetUnit(enemy).Hp;
        var queued = s.Timeline.Add(TimelineKind.AutoEvent, 5, tinker, Turrets.EventTagPrefix + turret);
        s.GetUnit(enemy).LearnSkill("brawl_strike");
        Assert.Contains(s.Timeline.Upcoming(), e => e.Id == queued.Id);
        s.Timeline.Remove(queued.Id);                                         // what Cut the Strings does
        Assert.DoesNotContain(s.Timeline.Upcoming(), e => e.Id == queued.Id);
        Assert.Equal(hp, s.GetUnit(enemy).Hp);
    }

    [Fact]
    public void TheDriverResolvesAnAutoEventThatReachesItsTime()
    {
        var (s, tinker, enemy) = Board(1, "barricade", "bolt_turret");
        var turret = PlaceTurret(s, tinker, "bolt_turret");
        var hp = s.GetUnit(enemy).Hp;
        s.Timeline.Add(TimelineKind.AutoEvent, 3, tinker, Turrets.EventTagPrefix + turret);
        for (var i = 0; i < 5 && s.GetUnit(enemy).Hp == hp; i++) BattleDriver.NextTurn(s);
        Assert.True(s.GetUnit(enemy).Hp < hp);
    }

    [Fact]
    public void ARepairHealsAConstructForEightAndOnlyConstructs()
    {
        var (s, tinker, _) = Board(1, "barricade", "repair");
        var wall = PlaceTurret(s, tinker, "barricade");
        s.GetUnit(wall).Hp = 1;
        Assert.Null(Why(s, Use(tinker, "repair", wall)));
        Assert.NotNull(Why(s, Use(tinker, "repair", tinker)));
        Run(s, Use(tinker, "repair", wall));
        Assert.Equal(9, s.GetUnit(wall).Hp);
    }

    // ---- Chemist ----

    [Fact]
    public void PoisonGasPoisonsTheSpotAndTheSpotsBesideForTwentyAndGasCloudAWholeRow()
    {
        var (s, tinker, a) = Board(1, "healing_draught", "poison_gas", "gas_cloud");
        var b = World.Unit(s, Side.B, Row.Front);
        var c = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(tinker, "poison_gas", b));
        Assert.All(new[] { a, b, c }, id => Assert.Equal(20, s.Ground.Find(s.Ground.SpotOf(id), GroundKind.Poison)!.EndsAt));

        var (s2, t2, _) = Board(1, "healing_draught", "poison_gas", "gas_cloud");
        var middle = World.Unit(s2, Side.B, Row.Middle);
        s2.ShiftMomentum(Side.A, 4);
        Run(s2, Use(t2, "gas_cloud", row: new RowTarget(Side.B, Row.Middle)));
        Assert.True(s2.Ground.Has(s2.Ground.SpotOf(middle), GroundKind.Poison));
    }

    [Fact]
    public void PlagueDoctorDoublesPoisonGroundButTheTinkerCannotHeal()
    {
        var (s, tinker, enemy) = Board(1, "healing_draught", "poison_gas", "gas_cloud", "plague_doctor", "barricade", "repair");
        Run(s, Use(tinker, "poison_gas", enemy));
        Assert.Equal(40, s.Ground.Find(s.Ground.SpotOf(enemy), GroundKind.Poison)!.EndsAt);
        Assert.NotNull(Why(s, Use(tinker, "healing_draught", tinker)));
        Assert.NotNull(Why(s, Use(tinker, "repair", enemy)));
    }

    [Fact]
    public void HealingDraughtHealsSixToSevenAndAntidoteRemovesPoisonBleedingAndBlind()
    {
        var (s, tinker, _) = Board(1, "healing_draught", "antidote");
        var ally = World.Unit(s, Side.A, Row.Front);
        s.GetUnit(ally).Hp = 10;
        Run(s, Use(tinker, "healing_draught", ally));
        Assert.InRange(s.GetUnit(ally).Hp - 10, 6, 7);

        Ready(s, tinker);
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Poison, Duration: 50));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Bleeding, Uses: 3));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(ally, StatusIds.Silenced, Duration: 20));
        Run(s, Use(tinker, "antidote", ally));
        Assert.False(Has(s, ally, StatusIds.Poison) || Has(s, ally, StatusIds.Bleeding) || Has(s, ally, StatusIds.Blind));
        Assert.True(Has(s, ally, StatusIds.Silenced));
    }

    // ---- Pyrotechnician ----

    [Fact]
    public void FirebombBurnsTheSpotAndTheSpotsBesideItAndFlameTurretLightsTheNearestEnemyAfterEachAction()
    {
        var (s, tinker, a) = Board(1, "flashbang", "firebomb", "flame_turret");
        var b = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(tinker, "firebomb", b));
        Assert.True(s.Ground.Has(s.Ground.SpotOf(a), GroundKind.Burning) && s.Ground.Has(s.Ground.SpotOf(b), GroundKind.Burning));

        var (s2, t2, e2) = Board(2, "flashbang", "firebomb", "flame_turret");
        PlaceTurret(s2, t2, "flame_turret");                                   // it fires right after the placing action
        Assert.Equal(s2.Timeline.Now + 20, s2.Ground.Find(s2.Ground.SpotOf(e2), GroundKind.Burning)!.EndsAt);
    }

    [Fact]
    public void FlashbangBlindsAWholeRowForTenAndShowmanDoublesItButAddsDelayToDamagingSkills()
    {
        var (s, tinker, a) = Board(1, "flashbang");
        var b = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(tinker, "flashbang", row: new RowTarget(Side.B, Row.Front)));
        Assert.Equal(10, Statuses.Get(s.GetUnit(a), StatusIds.Blind)!.EndsAt);
        Assert.Equal(10, Statuses.Get(s.GetUnit(b), StatusIds.Blind)!.EndsAt);

        var (s2, t2, e2) = Board(1, "flashbang", "showman", "brawl_strike");
        Run(s2, Use(t2, "flashbang", row: new RowTarget(Side.B, Row.Front)));
        Assert.Equal(20, Statuses.Get(s2.GetUnit(e2), StatusIds.Blind)!.EndsAt);
        Ready(s2, t2);
        Run(s2, Use(t2, "brawl_strike", e2));
        Assert.Equal(Timeline.Wait(3 + 2, 5) , s2.Timeline.TurnOf(t2)!.Due);   // +2 Delay on the damaging skill
    }
}
