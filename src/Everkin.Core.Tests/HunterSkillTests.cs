using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class HunterSkillTests
{
    private static readonly string[] AllHunter =
    {
        "stalk", "creep_closer", "freeze_in_place", "lie_in_wait", "vanish", "read_tracks", "scent", "scent_of_blood",
        "target", "sniff_the_air", "decoy", "net_trap", "cage", "cut_off_retreat", "flush", "pack_strike",
    };

    private static SkillUse Use(int user, string skill, int? target = null, RowTarget? row = null, MoveOption? place = null) =>
        new(user, skill, target, row, place);

    private static void Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

    /// <summary>Clears a unit's scheduled turn so the unit can act again at once in a test.</summary>
    private static void Ready(BattleState s, int unit)
    {
        if (s.Timeline.TurnOf(unit) is { } t) s.Timeline.Remove(t.Id);
    }

    // ---- the catalog ----

    [Fact]
    public void AllSixteenHunterSkillsAreImplementedWithTheirContentNumbers()
    {
        var hunter = World.Content.ProfessionSkills.Where(s => s.Profession == "hunter").ToList();
        Assert.Equal(16, hunter.Count);
        foreach (var def in hunter)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind == "P", skill.IsPassive);
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
        }
        Assert.Equal(AllHunter.OrderBy(x => x), hunter.Select(h => h.Id).OrderBy(x => x));
    }

    [Fact]
    public void APassiveIsNeverUsedAsAnAction()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "scent");
        Assert.Equal("a passive is always on and is not used", Why(s, Use(h, "scent")));
    }

    // ---- Stalker ----

    [Fact]
    public void StalkGivesStealthButNeedsTheHunterOutsideTheFrontRow()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "stalk");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal("needs the Hunter outside the Front row", Why(s, Use(h, "stalk")));
        Actions.Move(s, h, Row.Middle, 0);
        Ready(s, h);
        s.DrainEvents();
        Run(s, Use(h, "stalk"));
        Assert.True(s.GetUnit(h).IsStealthed);
        Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(h)!.Due - s.Timeline.Now);
    }

    [Fact]
    public void StalkIsNotLegalForTheSidesLastVisibleUnit()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "stalk");
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal("cannot gain stealth now", Why(s, Use(h, "stalk")));
    }

    [Fact]
    public void VanishGivesStealthEvenInTheFrontRowForThreeMomentum()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "vanish");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal("needs the side to be 3 ahead in Momentum", Why(s, Use(h, "vanish")));
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(h, "vanish"));
        Assert.True(s.GetUnit(h).IsStealthed);
        Assert.Equal(0, s.Momentum.Value);     // the cost of 3 was spent
    }

    [Fact]
    public void CreepCloserNeedsStealthMovesToAnAdjacentRowAndKeepsStealth()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "creep_closer");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal("only while stealthed", Why(s, Use(h, "creep_closer", row: new RowTarget(Side.A, Row.Front))));
        s.GetUnit(h).IsStealthed = true;
        Assert.Equal("not a legal row", Why(s, Use(h, "creep_closer", row: new RowTarget(Side.B, Row.Front))));
        Assert.Equal("not a legal row", Why(s, Use(h, "creep_closer", row: new RowTarget(Side.A, Row.Middle))));
        Run(s, Use(h, "creep_closer", row: new RowTarget(Side.A, Row.Rear)));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(h).Row);
        Assert.True(s.GetUnit(h).IsStealthed);
        Assert.Equal(Timeline.Wait(2, 5), s.Timeline.TurnOf(h)!.Due - s.Timeline.Now);
    }

    [Fact]
    public void ABleedingHunterUsingCreepCloserTakesBleedingDamage()
    {
        // 20.5: a Bleeding Hunter uses Creep Closer: it takes 2 Bleeding damage, since the skill moves its user.
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "creep_closer");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(h).IsStealthed = true;
        Statuses.Apply(s, new StatusApplication(h, StatusIds.Bleeding, Uses: 3));
        Run(s, Use(h, "creep_closer", row: new RowTarget(Side.A, Row.Rear)));
        Assert.Equal(38, s.GetUnit(h).Hp);
    }

    [Fact]
    public void FreezeInPlaceLetsAStealthedHunterKeepItsStealthHalfTheTimeWhenAnAreaHitsIt()
    {
        int kept = 0;
        for (ulong seed = 1; seed <= 600; seed++)
        {
            var s = new BattleState(seed);
            var hunter = World.Unit(s, Side.B, Row.Front, "freeze_in_place");
            var other = World.Unit(s, Side.B, Row.Front);
            var a = World.Unit(s, Side.A, Row.Front, "breath");
            BattleStart.Install(s, World.Catalog);
            s.GetUnit(hunter).IsStealthed = true;
            Run(s, Use(a, "breath", other));          // the splash reaches the stealthed Hunter next to it
            Assert.True(s.GetUnit(hunter).Hp < 40);   // it still takes the damage
            if (s.GetUnit(hunter).IsStealthed) kept++;
        }
        Assert.InRange(kept, 230, 370);
    }

    [Fact]
    public void WithoutFreezeInPlaceAnAreaHitAlwaysBreaksStealth()
    {
        // 20.5: a stealthed Rogue is hit by the splash of Breath: it is hit and its stealth ends.
        var s = new BattleState(1);
        var rogue = World.Unit(s, Side.B, Row.Front);
        var other = World.Unit(s, Side.B, Row.Front);
        var a = World.Unit(s, Side.A, Row.Front, "breath");
        s.GetUnit(rogue).IsStealthed = true;
        Run(s, Use(a, "breath", other));
        Assert.False(s.GetUnit(rogue).IsStealthed);
    }

    [Fact]
    public void LieInWaitNeedsStealthAndMeleeReachAndLaysTheTrap()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "lie_in_wait");
        World.Unit(s, Side.A, Row.Middle);
        var front = World.Unit(s, Side.B, Row.Front);
        var middle = World.Unit(s, Side.B, Row.Middle);
        Assert.Equal("only while stealthed", Why(s, Use(h, "lie_in_wait", front)));
        s.GetUnit(h).IsStealthed = true;
        Assert.Equal("not a legal target", Why(s, Use(h, "lie_in_wait", middle)));     // melee reach only
        Run(s, Use(h, "lie_in_wait", front));
        Assert.True(Statuses.Has(s.GetUnit(front), StatusIds.LieInWait));
        Assert.True(s.GetUnit(h).IsStealthed);                                         // laying the trap does not break stealth
    }

    [Fact]
    public void TheNextSkillOfALieInWaitVictimIsLostItIsStunnedAndTheHuntersStealthBreaks()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "lie_in_wait");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var victim = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        s.GetUnit(h).IsStealthed = true;
        Run(s, Use(h, "lie_in_wait", victim));
        s.Timeline.Add(TimelineKind.DelayedEvent, 3);
        s.Timeline.Pop();
        s.DrainEvents();

        var events = Actions.Use(s, World.Catalog, new SkillUse(victim, "brawl_strike", ally));
        Assert.Contains(new SkillLost(victim, "brawl_strike"), events);
        Assert.Equal(40, s.GetUnit(ally).Hp);                                   // the skill had no effect
        Assert.DoesNotContain(events, e => e is DamageDealt);
        Assert.Equal(13, Statuses.Get(s.GetUnit(victim), StatusIds.Stunned)!.EndsAt);   // 10 time units from now (3)
        Assert.False(Statuses.Has(s.GetUnit(victim), StatusIds.LieInWait));
        Assert.False(s.GetUnit(h).IsStealthed);
        Assert.Equal(3 + Timeline.Wait(3, 5), s.Timeline.TurnOf(victim)!.Due);   // its Delay was paid
    }

    [Fact]
    public void LieInWaitCanBeInterceptedAndTheInterceptorGetsTheTrap()
    {
        var found = false;
        for (ulong seed = 1; seed <= 200 && !found; seed++)
        {
            var s = new BattleState(seed);
            var h = World.Unit(s, Side.A, Row.Front, "lie_in_wait");
            World.Unit(s, Side.A, Row.Middle);
            var front = World.Unit(s, Side.B, Row.Front);
            s.GetUnit(h).IsStealthed = true;
            Run(s, Use(h, "lie_in_wait", front));
            found = Statuses.Has(s.GetUnit(front), StatusIds.LieInWait);
            Assert.True(found);
        }
        Assert.True(found);
    }

    // ---- Tracker ----

    [Fact]
    public void TargetMarksAnyVisibleEnemyAndMarkingAnotherMovesTheMark()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "target");
        var ally = World.Unit(s, Side.A, Row.Middle);
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Rear);
        Assert.Equal("not a legal target", Why(s, Use(h, "target", ally)));          // Marked is enemy-only
        Run(s, Use(h, "target", e1));
        Assert.True(Statuses.Has(s.GetUnit(e1), StatusIds.Marked));
        Ready(s, h);
        Run(s, Use(h, "target", e2));
        Assert.False(Statuses.Has(s.GetUnit(e1), StatusIds.Marked));                  // the Mark moved
        Assert.True(Statuses.Has(s.GetUnit(e2), StatusIds.Marked));
        Assert.Equal(0, s.Momentum.Value);                                            // no damage: no Momentum
    }

    [Fact]
    public void TargetCannotBeInterceptedAndCannotChooseAStealthedEnemy()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "target");
        var front = World.Unit(s, Side.B, Row.Front);
        var behind = World.Unit(s, Side.B, Row.Middle);
        var hidden = World.Unit(s, Side.B, Row.Rear);
        s.GetUnit(hidden).IsStealthed = true;
        Assert.Equal("not a legal target", Why(s, Use(h, "target", hidden)));
        Run(s, Use(h, "target", behind));                                              // direct: no interception
        Assert.True(Statuses.Has(s.GetUnit(behind), StatusIds.Marked));
        Assert.False(Statuses.Has(s.GetUnit(front), StatusIds.Marked));
    }

    [Fact]
    public void ReadTracksRevealsEveryStealthedUnitInTheChosenRow()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "read_tracks");
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Front);
        var other = World.Unit(s, Side.B, Row.Middle);
        foreach (var id in new[] { e1, e2, other }) s.GetUnit(id).IsStealthed = true;
        Assert.Contains(new RowTarget(Side.B, Row.Front), World.Catalog.Get("read_tracks").LegalRows(s, h));
        s.DrainEvents();
        Run(s, Use(h, "read_tracks", row: new RowTarget(Side.B, Row.Front)));
        Assert.False(s.GetUnit(e1).IsStealthed);
        Assert.False(s.GetUnit(e2).IsStealthed);
        Assert.True(s.GetUnit(other).IsStealthed);                                    // another row
    }

    [Fact]
    public void ReadTracksNeedsARowWithSomeoneInReach()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "read_tracks");
        World.Unit(s, Side.B, Row.Front);
        Assert.Equal("not a legal row", Why(s, Use(h, "read_tracks", row: new RowTarget(Side.B, Row.Rear))));
    }

    [Fact]
    public void ScentStopsMarkedEnemiesFromGainingStealth()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "scent");
        World.Unit(s, Side.A, Row.Middle);
        var marked = World.Unit(s, Side.B, Row.Front);
        var other = World.Unit(s, Side.B, Row.Middle);
        World.Unit(s, Side.B, Row.Rear);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(marked, StatusIds.Marked, SourceUnitId: h));
        Assert.False(Stealth.CanGain(s, marked));
        Assert.True(Stealth.CanGain(s, other));
        Assert.True(Stealth.CanGain(s, h));                                            // its own side is not affected
        s.GetUnit(h).Hp = 0;
        Assert.True(Stealth.CanGain(s, marked));                                       // a KO Hunter smells nothing
    }

    [Fact]
    public void ScentOfBloodMarksAnEnemyWhoseHpFallsBelowTwentyPercent()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "scent_of_blood");
        var ally = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var enemy = World.Unit(s, Side.B, Row.Front);
        var enemy2 = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(ally, "brawl_strike", enemy));                                      // 40 -> 35..36: not below 20%
        Assert.False(Statuses.Has(s.GetUnit(enemy), StatusIds.Marked));
        s.GetUnit(enemy2).Hp = 9;
        Ready(s, ally);
        Run(s, Use(ally, "brawl_strike", enemy2));                                     // 9 -> 4..5: below 8 (20% of 40)
        Assert.True(Statuses.Has(s.GetUnit(enemy2), StatusIds.Marked));
        Assert.Equal(h, Statuses.Get(s.GetUnit(enemy2), StatusIds.Marked)!.SourceUnitId);
    }

    [Fact]
    public void ScentOfBloodDoesNotMarkAlliesOrFireWhenTheHunterIsKo()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "scent_of_blood");
        var ally = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var friend = World.Unit(s, Side.A, Row.Front);
        BattleStart.Install(s, World.Catalog);
        s.GetUnit(friend).Hp = 9;
        Run(s, Use(ally, "brawl_strike", friend));
        Assert.False(Statuses.Has(s.GetUnit(friend), StatusIds.Marked));
        s.GetUnit(h).Hp = 0;
        var enemy = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(enemy).Hp = 9;
        s.Timeline.Remove(s.Timeline.TurnOf(ally)!.Id);
        Actions.Use(s, World.Catalog, new SkillUse(ally, "brawl_strike", enemy));
        Assert.False(Statuses.Has(s.GetUnit(enemy), StatusIds.Marked));          // a KO Hunter reacts to nothing
    }

    [Fact]
    public void SniffTheAirMarksTheEnemyWithTheHighestPowerAtBattleStart()
    {
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Middle, "sniff_the_air");
        var weak = World.Unit(s, Side.B, Row.Front);
        var strong = World.Unit(s, Side.B, Row.Front);
        s.GetUnit(strong).Power = 8;
        BattleStart.Install(s, World.Catalog);
        Assert.True(Statuses.Has(s.GetUnit(strong), StatusIds.Marked));
        Assert.False(Statuses.Has(s.GetUnit(weak), StatusIds.Marked));
    }

    [Fact]
    public void SniffTheAirBreaksTiesWithTheSeededRollEquallyOften()
    {
        var firsts = new int[2];
        for (ulong seed = 1; seed <= 1000; seed++)
        {
            var s = new BattleState(seed);
            World.Unit(s, Side.A, Row.Middle, "sniff_the_air");
            var e1 = World.Unit(s, Side.B, Row.Front);
            World.Unit(s, Side.B, Row.Front);
            BattleStart.Install(s, World.Catalog);
            firsts[Statuses.Has(s.GetUnit(e1), StatusIds.Marked) ? 0 : 1]++;
        }
        Assert.All(firsts, c => Assert.InRange(c, 420, 580));
    }

    // ---- Trapper ----

    [Fact]
    public void ADecoyTakesTheHitWhenAMarkedEnemyAttacksTheHunter()
    {
        var s = new BattleState(1);
        var hunter = World.Unit(s, Side.A, Row.Middle, "decoy");
        var enemy = World.Unit(s, Side.B, Row.Front, "shot");
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Marked, SourceUnitId: hunter));
        Run(s, Use(enemy, "shot", hunter));
        Assert.Equal(40, s.GetUnit(hunter).Hp);                                         // the Hunter was not hit
        Assert.Equal(Row.Rear, s.Formation.PositionOf(hunter).Row);                     // it appears one row behind
        var decoy = s.Formation.Units(Side.A, Row.Middle).Single(id => s.GetUnit(id).Kind == UnitKind.Construct);
        Assert.Equal(10, s.GetUnit(decoy).MaxHp);
        Assert.True(s.GetUnit(decoy).Hp < 10);                                          // the decoy took the hit
        Assert.Equal(hunter, s.GetUnit(decoy).SummonerId);
    }

    [Fact]
    public void NoDecoyForAnUnmarkedAttackerOrAHunterInTheRearRow()
    {
        var s = new BattleState(1);
        var hunter = World.Unit(s, Side.A, Row.Middle, "decoy");
        var enemy = World.Unit(s, Side.B, Row.Front, "shot");
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(enemy, "shot", hunter));                                              // not Marked
        Assert.True(s.GetUnit(hunter).Hp < 40);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(hunter).Row);

        var s2 = new BattleState(1);
        var h2 = World.Unit(s2, Side.A, Row.Rear, "decoy");
        var e2 = World.Unit(s2, Side.B, Row.Front, "shot");
        BattleStart.Install(s2, World.Catalog);
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Marked, SourceUnitId: h2));
        Run(s2, Use(e2, "shot", h2));
        Assert.True(s2.GetUnit(h2).Hp < 40);                                             // no row behind: no decoy
        Assert.Equal(1, s2.Formation.Count(Side.A));
    }

    [Fact]
    public void NoDecoyWhenTheHunterCannotMoveBack()
    {
        var s = new BattleState(1);
        var hunter = World.Unit(s, Side.A, Row.Middle, "decoy");
        for (var i = 0; i < 6; i++) World.Unit(s, Side.A, Row.Rear);          // a full row behind
        var enemy = World.Unit(s, Side.B, Row.Front, "shot");
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Marked, SourceUnitId: hunter));
        var hpBefore = s.Units.Where(u => u.Side == Side.A).Sum(u => u.Hp);
        Run(s, Use(enemy, "shot", hunter));
        Assert.Equal(Row.Middle, s.Formation.PositionOf(hunter).Row);
        Assert.True(s.GetUnit(hunter).Hp < 40);                               // the Hunter takes it; nobody took bump damage
        Assert.True(s.Units.Where(u => u.Side == Side.A).Sum(u => u.Hp) <= hpBefore);
        Assert.DoesNotContain(s.Units, u => u.Kind == UnitKind.Construct);
    }

    [Fact]
    public void NetTrapIsPlacedNextToTheHunterAsAConstructWithFourHp()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "net_trap");
        World.Unit(s, Side.B, Row.Front);
        var options = World.Catalog.Get("net_trap").PlacementOptions(s, h);
        Assert.Equal(new[] { new MoveOption(Row.Front, 0), new MoveOption(Row.Front, 1) }, options);
        s.ShiftMomentum(Side.A, 1);
        Assert.Equal("not a legal place", Why(s, Use(h, "net_trap", place: new MoveOption(Row.Rear, 0))));
        Run(s, Use(h, "net_trap", place: new MoveOption(Row.Front, 1)));
        var trap = s.Formation.Units(Side.A, Row.Front)[1];
        Assert.Equal(UnitKind.Construct, s.GetUnit(trap).Kind);
        Assert.Equal(4, s.GetUnit(trap).MaxHp);
        Assert.Null(s.Timeline.TurnOf(trap));                                            // takes no turns
        Assert.Equal(0, s.Momentum.Value);                                               // cost 1 was spent
    }

    [Fact]
    public void NetTrapIsUnavailableWhenTheSideIsFull()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "net_trap");
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
            while (s.Formation.CanPlace(Side.A, row)) World.Unit(s, Side.A, row);
        s.ShiftMomentum(Side.A, 1);
        Assert.StartsWith("no free place", Why(s, Use(h, "net_trap", place: new MoveOption(Row.Front, 0)))!);
    }

    [Fact]
    public void AUnitThatHitsANetTrapHasThreeMoreDelayOnItsNextSkillOnce()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "net_trap");
        var attacker = World.Unit(s, Side.B, Row.Front, "shot", "brawl_strike");
        World.Unit(s, Side.B, Row.Rear);
        s.ShiftMomentum(Side.A, 1);
        Run(s, Use(h, "net_trap", place: new MoveOption(Row.Front, 1)));
        var trap = s.Formation.Units(Side.A, Row.Front)[1];

        Run(s, Use(attacker, "shot", trap));                                  // Delay 3 skill hits the trap (HP 4): it may be destroyed
        Assert.True(Statuses.Has(s.GetUnit(attacker), StatusIds.NextSkillDelay));
        Assert.Equal(3, Statuses.Get(s.GetUnit(attacker), StatusIds.NextSkillDelay)!.Magnitude);

        Ready(s, attacker);
        s.Timeline.Add(TimelineKind.DelayedEvent, 1);
        s.Timeline.Pop();
        var now = s.Timeline.Now;
        Run(s, Use(attacker, "brawl_strike", h));                              // its next skill: Delay 3 + 3
        Assert.Equal(now + Timeline.Wait(6, 5), s.Timeline.TurnOf(attacker)!.Due);
        Assert.False(Statuses.Has(s.GetUnit(attacker), StatusIds.NextSkillDelay));   // it counted once
    }

    [Fact]
    public void ADestroyedNetTrapStillSlowsTheUnitThatDestroyedItExactlyOnce()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "net_trap");
        var attacker = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        s.ShiftMomentum(Side.A, 1);
        Run(s, Use(h, "net_trap", place: new MoveOption(Row.Front, 1)));
        var trap = s.Formation.Units(Side.A, Row.Front)[1];
        s.GetUnit(trap).Hp = 1;
        Run(s, Use(attacker, "brawl_strike", trap));
        Assert.True(s.GetUnit(trap).IsRemoved);                                // removed at once, slot free
        Assert.Equal(1, s.Formation.Count(Side.A, Row.Front));
        Assert.Equal(3, Statuses.Get(s.GetUnit(attacker), StatusIds.NextSkillDelay)!.Magnitude);
    }

    [Fact]
    public void CageImmobilizesAUnitInMeleeReachForTwentyTimeUnits()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "cage");
        var target = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Middle);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(h, "cage", target));
        Assert.True(Statuses.Has(s.GetUnit(target), StatusIds.Immobilized));
        Assert.Equal(Timeline.Wait(6, 5), s.Timeline.TurnOf(h)!.Due);
    }

    [Fact]
    public void ACagedUnitCannotMove()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "cage");
        var target = World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(h, "cage", target));
        var caged = s.Units.Single(u => Statuses.Has(u, StatusIds.Immobilized));
        Assert.Equal(20, Statuses.Get(caged, StatusIds.Immobilized)!.EndsAt);
        Assert.Empty(Movement.LegalMoves(s, caged.Id));
    }

    // ---- Predator ----

    [Fact]
    public void CutOffRetreatNeedsAMarkedUnitAndImmobilizesItForTwentyTimeUnits()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "cut_off_retreat");
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Rear);
        Assert.Equal("not a valid target for this skill", Why(s, Use(h, "cut_off_retreat", e1)));
        Statuses.Apply(s, new StatusApplication(e2, StatusIds.Marked, SourceUnitId: h));
        Assert.Null(Why(s, Use(h, "cut_off_retreat", e2)));
        Run(s, Use(h, "cut_off_retreat", e2));
        Assert.Equal(20, Statuses.Get(s.GetUnit(e2), StatusIds.Immobilized)!.EndsAt);
    }

    [Fact]
    public void FlushPullsAMarkedUnitIntoItsOwnFrontRow()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "flush");
        World.Unit(s, Side.B, Row.Front);
        var rear = World.Unit(s, Side.B, Row.Rear);
        Statuses.Apply(s, new StatusApplication(rear, StatusIds.Marked, SourceUnitId: h));
        // The pulled unit lands on the Front unit at 100% overlap, which has nowhere to go: the chain is blocked and everyone in it is bumped.
        Run(s, Use(h, "flush", rear));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(rear).Row);
        Assert.Equal(39, s.GetUnit(rear).Hp);
    }

    [Fact]
    public void FlushMovesAMarkedUnitWhenTheFrontRowHasRoom()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "flush");
        var middle = World.Unit(s, Side.B, Row.Middle);
        World.Unit(s, Side.B, Row.Middle);                                                // Middle centers -1, 1; the Front row is empty
        Statuses.Apply(s, new StatusApplication(middle, StatusIds.Marked, SourceUnitId: h));
        Run(s, Use(h, "flush", middle));
        Assert.Equal(Row.Front, s.Formation.PositionOf(middle).Row);
        Assert.Equal(Timeline.Wait(4, 5), s.Timeline.TurnOf(h)!.Due);
    }

    [Fact]
    public void FlushOnlyAcceptsAMarkedMovableUnitNotAlreadyInFront()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "flush");
        var front = World.Unit(s, Side.B, Row.Front);
        var stuck = World.Unit(s, Side.B, Row.Middle);
        Statuses.Apply(s, new StatusApplication(front, StatusIds.Marked, SourceUnitId: h));
        Statuses.Apply(s, new StatusApplication(stuck, StatusIds.Marked, SourceUnitId: h));
        Statuses.Apply(s, new StatusApplication(stuck, StatusIds.Immobilized, Duration: 20));
        Assert.Equal("not a valid target for this skill", Why(s, Use(h, "flush", front)));
        Assert.Equal("not a valid target for this skill", Why(s, Use(h, "flush", stuck)));
    }

    [Fact]
    public void PackStrikePullsForwardThreeTheNextTurnOfEveryAllyThatHitAMarkedUnitSinceTheHuntersLastAction()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "pack_strike", "target");
        var hitter = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var idle = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Rear);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Marked, SourceUnitId: h));
        s.Timeline.Add(TimelineKind.UnitTurn, 20, hitter);
        s.Timeline.Add(TimelineKind.UnitTurn, 20, idle);
        s.ShiftMomentum(Side.A, 3);
        Ready(s, hitter);
        s.Timeline.Add(TimelineKind.UnitTurn, 20, hitter);

        Actions.Use(s, World.Catalog, new SkillUse(hitter, "brawl_strike", enemy));      // hits the Marked enemy
        var hitterDue = s.Timeline.TurnOf(hitter)!.Due;
        var idleDue = s.Timeline.TurnOf(idle)!.Due;
        Run(s, Use(h, "pack_strike"));
        Assert.Equal(hitterDue - 3, s.Timeline.TurnOf(hitter)!.Due);
        Assert.Equal(idleDue, s.Timeline.TurnOf(idle)!.Due);
    }

    [Fact]
    public void PackStrikeIgnoresHitsFromBeforeTheHuntersLastAction()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Middle, "pack_strike", "target");
        var hitter = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var enemy = World.Unit(s, Side.B, Row.Front);
        World.Unit(s, Side.B, Row.Rear);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Marked, SourceUnitId: h));
        Actions.Use(s, World.Catalog, new SkillUse(hitter, "brawl_strike", enemy));      // before the Hunter's next action
        Actions.SkipTurn(s, h);                                                          // the Hunter acts: its "last turn" is now
        s.Timeline.Remove(s.Timeline.TurnOf(h)!.Id);
        s.ShiftMomentum(Side.A, 3);
        var due = s.Timeline.TurnOf(hitter)!.Due;
        Run(s, Use(h, "pack_strike"));
        Assert.Equal(due, s.Timeline.TurnOf(hitter)!.Due);
    }

    [Fact]
    public void PackStrikeNeverPullsTheHuntersOwnTurnOrAnEnemys()
    {
        var s = new BattleState(1);
        var h = World.Unit(s, Side.A, Row.Front, "pack_strike", "brawl_strike");
        var enemy = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        Statuses.Apply(s, new StatusApplication(h, StatusIds.Marked, SourceUnitId: enemy));   // a Marked ally of the enemy
        s.Timeline.Add(TimelineKind.UnitTurn, 30, enemy);
        Actions.Use(s, World.Catalog, new SkillUse(enemy, "brawl_strike", h));
        s.ShiftMomentum(Side.A, 4);                                                      // enough for the cost of 3
        var enemyDue = s.Timeline.TurnOf(enemy)!.Due;
        Run(s, Use(h, "pack_strike"));
        Assert.Equal(enemyDue, s.Timeline.TurnOf(enemy)!.Due);
    }

    // ---- the hit log ----

    [Fact]
    public void HitsOnMarkedUnitsAreLoggedByAction()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "brawl_strike");
        var e = World.Unit(s, Side.B, Row.Front);
        Run(s, Use(a, "brawl_strike", e));
        Assert.Empty(s.MarkedHits);
        Ready(s, a);
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked));
        Run(s, Use(a, "brawl_strike", e));
        Assert.Equal(new[] { new MarkedHit(2, a, e) }, s.MarkedHits);
    }
}
