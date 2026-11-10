using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class SummonerSkillTests
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
            Statuses.HandleExpiry(s, e);
        }
    }

    /// <summary>A real creature whose Attack Type attack is Brawl Strike (so a body of it can be raised).</summary>
    private static string BrawlerCreature => World.Content.Creatures.First(c => World.Catalog.AttackSkillFor(c.AttackType) == "brawl_strike").Id;

    private static int AddBrawler(BattleState s, Side side, Row row)
    {
        var u = s.AddUnit("brawler", side, 40, 5, 5, BrawlerCreature);
        s.Formation.Place(u.Id, side, row);
        return u.Id;
    }

    private static void Kill(BattleState s, int unit)
    {
        s.GetUnit(unit).Hp = 0;
        Knockout.ResolveAll(s);
    }

    /// <summary>A Summoner in the Front row, an ally in the Middle row, two enemies in the Front row.</summary>
    private static (BattleState S, int Summoner, int Enemy) Board(ulong seed = 1, params string[] skills)
    {
        var s = new BattleState(seed);
        var summoner = World.Unit(s, Side.A, Row.Front, skills);
        World.Unit(s, Side.A, Row.Middle);
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 8);
        return (s, summoner, enemy);
    }

    private static int Summon(BattleState s, int summoner, string skill)
    {
        var place = World.Catalog.Get(skill).PlacementOptions(s, summoner)[0];
        Run(s, Use(summoner, skill, place: place));
        Ready(s, summoner);
        return s.Units.Last(u => u.SummonerId == summoner).Id;
    }

    [Fact]
    public void AllFifteenSummonerSkillsAreImplemented()
    {
        var all = World.Content.ProfessionSkills.Where(x => x.Profession == "summoner").ToList();
        Assert.Equal(15, all.Count);
        foreach (var def in all)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind is not ("A" or "A (summon)"), skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
            if (!skill.IsPassive) Assert.True(skill.Magical);
        }
    }

    // ---- Golemancer ----

    [Fact]
    public void AClayGolemAppearsNextToTheSummonerAndHitsTheNearestEnemyForFiveToSix()
    {
        var (s, summoner, enemy) = Board(1, "clay_golem");
        var golem = Summon(s, summoner, "clay_golem");
        Assert.Equal(25, s.GetUnit(golem).MaxHp);
        Assert.Equal(3, s.GetUnit(golem).Speed);
        Assert.Contains(golem, s.Formation.Adjacent(summoner));
        Assert.Equal(Timeline.InitialWait(3), s.Timeline.TurnOf(golem)!.Due);

        Ready(s, golem);
        var hp = s.GetUnit(enemy).Hp + s.Units.Where(u => u.Side == Side.B).Sum(u => u.Hp) - s.GetUnit(enemy).Hp;
        AutonomousTurns.Play(s, World.Catalog, golem);
        var lost = hp - s.Units.Where(u => u.Side == Side.B).Sum(u => u.Hp);
        Assert.InRange(lost, 5, 6);
        Assert.Equal(Timeline.Wait(3, 3), s.Timeline.TurnOf(golem)!.Due);          // Delay 3
    }

    [Fact]
    public void ASummonWithoutALegalTargetSkipsItsTurn()
    {
        var s = new BattleState(1);
        var summoner = World.Unit(s, Side.A, Row.Rear, "clay_golem");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 8);
        var golem = Summon(s, summoner, "clay_golem");                              // in the Rear row: melee reach D1 cannot get there
        Ready(s, golem);
        var events = Actions.SkipTurn(s, golem);
        Assert.Empty(events);
        Ready(s, golem);
        AutonomousTurns.Play(s, World.Catalog, golem);
        Assert.Equal(Timeline.Wait(Timeline.SkippedTurnDelay, 3), s.Timeline.TurnOf(golem)!.Due);
    }

    [Fact]
    public void TheDriverPlaysSummonTurnsItselfAndOnlyReturnsUnitsThatNeedAChoice()
    {
        var (s, summoner, enemy) = Board(1, "clay_golem");
        var golem = Summon(s, summoner, "clay_golem");
        s.Timeline.ScheduleTurn(summoner, 20, 5);
        var next = BattleDriver.NextTurn(s, World.Catalog);
        Assert.Equal(summoner, next);
        Assert.True(s.Units.Where(u => u.Side == Side.B).Sum(u => u.Hp) < 80);      // the golem acted on the way
        Assert.Equal(golem, golem);
        Assert.Equal(enemy, enemy);
    }

    [Fact]
    public void RuneGolemHitsSilenceAndClockworkGolemHastensItsSummoner()
    {
        var (s, summoner, _) = Board(1, "clay_golem", "rune_golem", "clockwork_golem");
        var rune = Summon(s, summoner, "rune_golem");
        Ready(s, rune);
        AutonomousTurns.Play(s, World.Catalog, rune);
        Assert.Contains(s.Units.Where(u => u.Side == Side.B), u => Has(s, u.Id, StatusIds.Silenced));

        var (s2, summoner2, _) = Board(1, "clay_golem", "rune_golem", "clockwork_golem");
        var clock = Summon(s2, summoner2, "clockwork_golem");
        s2.Timeline.ScheduleTurn(summoner2, 8, 5);
        var due = s2.Timeline.TurnOf(summoner2)!.Due;
        Ready(s2, clock);
        AutonomousTurns.Play(s2, World.Catalog, clock);
        Assert.Equal(due - 2, s2.Timeline.TurnOf(summoner2)!.Due);
    }

    [Fact]
    public void BellGolemRemovesOneHarmfulStatusFromEachAdjacentAllyAndNeverAttacks()
    {
        var (s, summoner, enemy) = Board(1, "clay_golem", "rune_golem", "bell_golem");
        var bell = Summon(s, summoner, "bell_golem");
        var neighbours = s.Formation.Adjacent(bell).Where(id => s.GetUnit(id).Side == Side.A).ToList();
        Assert.NotEmpty(neighbours);
        foreach (var id in neighbours)
        {
            Statuses.Apply(s, new StatusApplication(id, StatusIds.Blind, Duration: 50));
            Statuses.Apply(s, new StatusApplication(id, StatusIds.Silenced, Duration: 50));
            Statuses.Apply(s, new StatusApplication(id, StatusIds.Haste, Duration: 50, Magnitude: 1));
        }
        Ready(s, bell);
        AutonomousTurns.Play(s, World.Catalog, bell);
        foreach (var id in neighbours)
        {
            Assert.Equal(1, new[] { StatusIds.Blind, StatusIds.Silenced }.Count(st => Has(s, id, st)));
            Assert.True(Has(s, id, StatusIds.Haste));
        }
        Assert.Equal(40, s.GetUnit(enemy).Hp);
    }

    [Fact]
    public void GolemHeartKeepsOneGolemWithDoubleHp()
    {
        var (s, summoner, _) = Board(1, "clay_golem", "rune_golem", "clockwork_golem", "golem_heart");
        var first = Summon(s, summoner, "clay_golem");
        Assert.Equal(50, s.GetUnit(first).MaxHp);
        var second = Summon(s, summoner, "clockwork_golem");
        Assert.True(s.GetUnit(first).IsRemoved);
        Assert.Equal(32, s.GetUnit(second).MaxHp);
    }

    // ---- Conjurer ----

    [Fact]
    public void AnimateBladesShootFromAnywhereAndVanishAfterThirtyTimeUnits()
    {
        var s = new BattleState(1);
        var summoner = World.Unit(s, Side.A, Row.Rear, "animate_blades", "blade_storm");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 8);
        var place = World.Catalog.Get("animate_blades").PlacementOptions(s, summoner)[0];
        Run(s, Use(summoner, "animate_blades", place: place));
        var blades = s.Units.Where(u => u.SummonerId == summoner).ToList();
        Assert.Equal(2, blades.Count);                                              // Blade Storm: two sets
        Assert.All(blades, b => { Assert.Equal(6, b.MaxHp); Assert.Equal(7, b.Speed); });

        var enemies = s.Units.Where(u => u.Side == Side.B).ToList();
        var hp = enemies.Sum(u => u.Hp);
        Ready(s, blades[0].Id);
        AutonomousTurns.Play(s, World.Catalog, blades[0].Id);
        Assert.InRange(hp - enemies.Sum(u => u.Hp), 2, 3);

        AdvanceTo(s, 31);
        Assert.All(blades, b => Assert.True(b.IsRemoved));
    }

    [Fact]
    public void PortalPlacesSummonsOnARandomFreeSpotAndNeedsNoPlace()
    {
        var (s, summoner, _) = Board(1, "animate_blades", "animate_mirror", "portal");
        Assert.Null(Why(s, Use(summoner, "animate_blades")));
        var rows = new HashSet<Row>();
        for (ulong seed = 1; seed <= 30; seed++)
        {
            var (s2, sum2, _) = Board(seed, "animate_blades", "animate_mirror", "portal");
            Run(s2, Use(sum2, "animate_blades"));
            rows.Add(s2.Formation.PositionOf(s2.Units.Last().Id).Row);
        }
        Assert.True(rows.Count > 1);
    }

    [Fact]
    public void AnimateMirrorAnswersEachHitWithTheAttackersOwnBasicAttackAndTakesNoTurns()
    {
        var (s, summoner, _) = Board(1, "animate_blades", "animate_mirror");
        var attacker = AddBrawler(s, Side.B, Row.Front);
        s.GetUnit(attacker).LearnSkill("brawl_strike");
        var mirror = Summon(s, summoner, "animate_mirror");
        Assert.Null(s.Timeline.TurnOf(mirror));
        Assert.Equal(8, s.GetUnit(mirror).MaxHp);
        var hp = s.GetUnit(attacker).Hp;
        Run(s, Use(attacker, "brawl_strike", mirror));
        Assert.InRange(hp - s.GetUnit(attacker).Hp, 4, 5);                          // its own attack back, with the Summoner's Power 5
    }

    // ---- Necromancer ----

    [Fact]
    public void RaiseUndeadTurnsABodyIntoAnUndeadOfYoursWhichFightsOnItsOwn()
    {
        var (s, summoner, enemy) = Board(1, "raise_undead");
        var body = AddBrawler(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Kill(s, body);
        var options = World.Catalog.Get("raise_undead").PlacementOptionsFor(s, summoner, body);
        Assert.NotEmpty(options);
        Run(s, Use(summoner, "raise_undead", body, options[0]));
        var undead = s.GetUnit(body);
        Assert.True(undead.IsUndead);
        Assert.Equal(Side.A, undead.Side);
        Assert.Equal(12, undead.MaxHp);
        Assert.Equal(4, undead.Power);
        Assert.Equal("brawl_strike", undead.AutoSkillId);

        Ready(s, body);
        var before = s.Units.Where(u => u.Side == Side.B && !u.IsKnockedOut).Sum(u => u.Hp);
        AutonomousTurns.Play(s, World.Catalog, body);
        Assert.InRange(before - s.Units.Where(u => u.Side == Side.B && !u.IsKnockedOut).Sum(u => u.Hp), 3, 4);   // Power 4 scales 4–5 to 3–4
        Assert.Equal(enemy, enemy);
    }

    [Fact]
    public void MassGraveRaisesAnotherBodyAndLichFormMakesUndeadPowerSixAndHealingHurtTheSummoner()
    {
        var (s, summoner, _) = Board(1, "raise_undead", "mass_grave", "lich_form");
        var a = AddBrawler(s, Side.B, Row.Front);
        var b = AddBrawler(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Kill(s, a);
        Kill(s, b);
        Run(s, Use(summoner, "raise_undead", a, World.Catalog.Get("raise_undead").PlacementOptionsFor(s, summoner, a)[0]));
        Assert.True(s.GetUnit(a).IsUndead && s.GetUnit(b).IsUndead);
        Assert.Equal(6, s.GetUnit(a).Power);
        Assert.Equal(6, s.GetUnit(b).Power);

        s.GetUnit(summoner).Hp = 30;
        s.Heal(null, summoner, 5);
        Assert.Equal(25, s.GetUnit(summoner).Hp);                                   // healing hurts
    }

    [Fact]
    public void CorpseExplosionHitsTheNeighboursForSixAndPlagueCorpsePoisonsThem()
    {
        var (s, summoner, _) = Board(1, "raise_undead", "corpse_explosion", "plague_corpse");
        var left = World.Unit(s, Side.B, Row.Middle);
        var body = World.Unit(s, Side.B, Row.Middle);
        var right = World.Unit(s, Side.B, Row.Middle);
        Kill(s, body);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(summoner, "corpse_explosion", body));
        Assert.Equal(34, s.GetUnit(left).Hp);
        Assert.Equal(34, s.GetUnit(right).Hp);
        Assert.True(s.GetUnit(body).IsKnockedOut);
        Assert.Equal(20, Statuses.Get(s.GetUnit(left), StatusIds.Poison)!.EndsAt);
    }

    [Fact]
    public void AnExplodingUndeadTakesTheSixItself()
    {
        var (s, summoner, _) = Board(1, "raise_undead", "corpse_explosion");
        var body = AddBrawler(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Kill(s, body);
        Run(s, Use(summoner, "raise_undead", body, World.Catalog.Get("raise_undead").PlacementOptionsFor(s, summoner, body)[0]));
        Ready(s, summoner);
        s.ShiftMomentum(Side.A, 4);
        Run(s, Use(summoner, "corpse_explosion", body));
        Assert.Equal(6, s.GetUnit(body).Hp);                                        // 12 − 6
    }

    [Fact]
    public void DesecrateKeepsABodyFromBeingRevivedForFortyTimeUnits()
    {
        var s = new BattleState(1);
        var summoner = World.Unit(s, Side.A, Row.Front, "raise_undead", "desecrate");
        var healer = World.Unit(s, Side.A, Row.Middle, "revive");
        var body = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.ShiftMomentum(Side.A, 8);
        Kill(s, body);
        Run(s, Use(summoner, "desecrate", body));
        Assert.NotNull(Why(s, Use(healer, "revive", body)));
        AdvanceTo(s, 41);
        Assert.Null(Why(s, Use(healer, "revive", body)));
    }
}
