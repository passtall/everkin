using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class GuardianSkillTests
{
    private static SkillUse Use(int user, string skill, int? target = null, MoveOption? place = null) =>
        new(user, skill, target, null, place);

    private static void Run(BattleState s, SkillUse use) => Actions.Use(s, World.Catalog, use);

    private static string? Why(BattleState s, SkillUse use) => Actions.WhyNot(s, World.Catalog, use);

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

    // ---- the catalog ----

    [Fact]
    public void AllFifteenGuardianSkillsAreImplementedWithTheirContentNumbers()
    {
        var guardian = World.Content.ProfessionSkills.Where(s => s.Profession == "guardian").ToList();
        Assert.Equal(15, guardian.Count);
        foreach (var def in guardian)
        {
            var skill = World.Catalog.Get(def.Id);
            Assert.Equal(def.Kind != "A", skill.IsPassive);          // passives, modifiers and keystones are always on
            Assert.Equal(def.Delay ?? 0, skill.Delay);
            Assert.Equal(def.Momentum ?? 0, skill.MomentumCost);
        }
    }

    // ---- Keeper: Ward ----

    [Fact]
    public void WardGivesAWardOfFourUntilTheGuardiansNextTurn()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "ward");
        var ally = World.Unit(s, Side.A, Row.Rear);          // own side: D2 reaches any ally
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(g, "ward", ally));
        Assert.Equal(4, s.GetUnit(ally).Ward);
        Assert.Equal(Timeline.Wait(3, 5), s.Timeline.TurnOf(g)!.Due);
        Statuses.OnTurnStart(s, g);
        Assert.Equal(0, s.GetUnit(ally).Ward);
    }

    [Fact]
    public void AWardIsScaledByThePowerOfTheGuardian()
    {
        // 20.5: a Power 7 Guardian uses Ward (4 at Power 5): the Ward absorbs 5.
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "ward");
        s.GetUnit(g).Power = 7;
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(g, "ward", g));
        Assert.Equal(5, s.GetUnit(g).Ward);
    }

    [Fact]
    public void AGuardianInFrontReachesAnAllyAtTheFarEndOfItsRearRow()
    {
        // 20.5: on its own side, D2 reaches any unit.
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "ward");
        for (var i = 0; i < 5; i++) World.Unit(s, Side.A, Row.Rear);
        var far = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        Assert.Null(Why(s, Use(g, "ward", far)));
    }

    [Fact]
    public void WardCanAlsoBeUsedOnAnEnemy()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "ward");
        var enemy = World.Unit(s, Side.B, Row.Front);
        Assert.Null(Why(s, Use(g, "ward", enemy)));        // either side is legal unless a skill says own side only
    }

    [Fact]
    public void LastingWardKeepsTheWardUntilItIsBroken()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "ward", "lasting_ward");
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(g, "ward", g));
        Statuses.OnTurnStart(s, g);
        Assert.Equal(4, s.GetUnit(g).Ward);
        Assert.Throws<InvalidOperationException>(() => Actions.Use(s, World.Catalog, new SkillUse(g, "lasting_ward")));
    }

    [Fact]
    public void OverchargedWardIsEightAndLastsOnlyUntilThatUnitsNextTurn()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "overcharged_ward");
        var ally = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 3);
        Run(s, Use(g, "overcharged_ward", ally));
        Assert.Equal(8, s.GetUnit(ally).Ward);
        Statuses.OnTurnStart(s, g);                          // the Guardian's own turn does not end it
        Assert.Equal(8, s.GetUnit(ally).Ward);
        Statuses.OnTurnStart(s, ally);
        Assert.Equal(0, s.GetUnit(ally).Ward);
    }

    [Fact]
    public void WardKeepsTheLargerAmount()
    {
        // 20.5: a unit with 3 Ward left gets Shield Dome's Ward 2: it keeps 3. Ward 4 then Overcharged Ward 8: the new one replaces it.
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "shield_dome", "overcharged_ward");
        var ally = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 10);
        s.GetUnit(ally).Ward = 3;
        Run(s, Use(g, "shield_dome"));
        Assert.Equal(3, s.GetUnit(ally).Ward);
        Assert.Equal(2, s.GetUnit(g).Ward);

        s.GetUnit(ally).Ward = 4;
        Ready(s, g);
        Run(s, Use(g, "overcharged_ward", ally));
        Assert.Equal(8, s.GetUnit(ally).Ward);
    }

    [Fact]
    public void ShieldDomeGivesEveryAllyAWardOfTwoUntilTheGuardiansNextTurnAtAMomentumCostOfFive()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "shield_dome");
        var a1 = World.Unit(s, Side.A, Row.Middle);
        var a2 = World.Unit(s, Side.A, Row.Rear);
        var enemy = World.Unit(s, Side.B, Row.Front);
        Assert.Equal("needs the side to be 5 ahead in Momentum", Why(s, Use(g, "shield_dome")));
        s.ShiftMomentum(Side.A, 5);
        Run(s, Use(g, "shield_dome"));
        Assert.All(new[] { g, a1, a2 }, id => Assert.Equal(2, s.GetUnit(id).Ward));
        Assert.Equal(0, s.GetUnit(enemy).Ward);
        Assert.Equal(0, s.Momentum.Value);
        Assert.Equal(Timeline.Wait(7, 5), s.Timeline.TurnOf(g)!.Due);
    }

    // ---- Bulwark passives ----

    [Fact]
    public void HoldPositionStopsEnemyEffectsFromPushingPullingOrSwappingTheGuardianButNotItsOwnSide()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "hold_position");
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Assert.Equal(ForcedMoveStatus.NotMovable, Movement.ForceMove(s, g, Row.Middle, Side.B).Status);
        Assert.False(Movement.Swap(s, g, World.Unit(s, Side.A, Row.Rear), Side.B));
        Assert.Equal(Row.Front, s.Formation.PositionOf(g).Row);
        Assert.Equal(ForcedMoveStatus.Moved, Movement.ForceMove(s, g, Row.Middle, Side.A).Status);   // an ally's effect is fine
    }

    [Fact]
    public void AnEnemyPushThatWouldBumpTheGuardianIsBlockedAndEveryUnitInTheChainTakesBumpDamage()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Middle, "hold_position");
        var front = World.Unit(s, Side.A, Row.Front);
        BattleStart.Install(s, World.Catalog);
        var r = Movement.ForceMove(s, front, Row.Middle, Side.B);          // would bump the Guardian onward
        Assert.Equal(ForcedMoveStatus.Blocked, r.Status);
        Assert.Contains(g, r.BumpDamaged);
        Assert.Equal(Row.Front, s.Formation.PositionOf(front).Row);
    }

    [Fact]
    public void FullCoverMakesTheGuardianAlwaysTakeTheWholeHit()
    {
        int partial = 0, withoutCoverPartial = 0;
        for (ulong seed = 1; seed <= 600; seed++)
        {
            foreach (var cover in new[] { true, false })
            {
                var s = new BattleState(seed);
                var g = World.Unit(s, Side.A, Row.Front, cover ? new[] { "full_cover" } : Array.Empty<string>());
                var target = World.Unit(s, Side.A, Row.Middle);
                var attacker = World.Unit(s, Side.B, Row.Front, "shot");
                BattleStart.Install(s, World.Catalog);
                var r = Interception.Resolve(s, target, 8);
                if (r.Intercepted && r.Shares[0].InterceptorId == g && !r.Shares[0].Full)
                { if (cover) partial++; else withoutCoverPartial++; }
                Assert.NotEqual(-1, attacker);
            }
        }
        Assert.Equal(0, partial);
        Assert.True(withoutCoverPartial > 50);
    }

    [Fact]
    public void LivingWallAlwaysInterceptsForTheUnitDirectlyBehindItAndCanNeverAttack()
    {
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = new BattleState(seed);
            var g = World.Unit(s, Side.A, Row.Front, "living_wall", "brawl_strike");
            var behind = World.Unit(s, Side.A, Row.Middle);
            World.Unit(s, Side.B, Row.Front, "shot");
            BattleStart.Install(s, World.Catalog);
            var r = Interception.Resolve(s, behind, 8);
            Assert.True(r.Intercepted);                              // 100%, no roll
            Assert.Equal(g, r.Shares[0].InterceptorId);
        }
        var s2 = new BattleState(1);
        var g2 = World.Unit(s2, Side.A, Row.Front, "living_wall", "brawl_strike");
        var e = World.Unit(s2, Side.B, Row.Front);
        BattleStart.Install(s2, World.Catalog);
        Assert.Equal("this unit can never attack", Why(s2, Use(g2, "brawl_strike", e)));
    }

    [Fact]
    public void LivingWallDoesNotCoverAUnitThatDoesNotOverlapIt()
    {
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = new BattleState(seed);
            var g = World.Unit(s, Side.A, Row.Front, "living_wall");   // Front centers -2, 0, 2: the Guardian is at -2
            World.Unit(s, Side.A, Row.Front);
            World.Unit(s, Side.A, Row.Front);
            var behind = World.Unit(s, Side.A, Row.Middle);             // one Middle card at 0: no overlap with -2
            BattleStart.Install(s, World.Catalog);
            Assert.DoesNotContain(Interception.Resolve(s, behind, 8).Shares, share => share.InterceptorId == g);
        }
    }

    [Fact]
    public void KeeperOfTheGateStopsTheGuardianFromIntercepting()
    {
        for (ulong seed = 1; seed <= 50; seed++)
        {
            var s = new BattleState(seed);
            var g = World.Unit(s, Side.A, Row.Front, "keeper_of_the_gate");
            var behind = World.Unit(s, Side.A, Row.Middle);
            BattleStart.Install(s, World.Catalog);
            Assert.Empty(Interception.Defenders(s, behind));
            Assert.DoesNotContain(Interception.Resolve(s, behind, 8).Shares, share => share.InterceptorId == g);
        }
    }

    // ---- Shield Wall ----

    [Fact]
    public void ShieldWallNeedsThreeUnitsInTheGuardiansRowAndLastsTenTimeUnits()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "shield_wall");
        World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 4);
        Assert.Equal("needs 3 or more of your units side by side in the Guardian's row", Why(s, Use(g, "shield_wall")));
        World.Unit(s, Side.A, Row.Front);
        Run(s, Use(g, "shield_wall"));
        Assert.Equal(10, Statuses.Get(s.GetUnit(g), StatusIds.ShieldWall)!.EndsAt);
        Assert.Equal(0, s.Momentum.Value);                          // the cost of 4
        AdvanceTo(s, 12);
        Assert.False(Statuses.Has(s.GetUnit(g), StatusIds.ShieldWall));
    }

    [Fact]
    public void TheWallAlwaysInterceptsForTheUnitsBehindItAndOnlyForMeleeAndProjectileAttacks()
    {
        for (ulong seed = 1; seed <= 100; seed++)
        {
            var s = new BattleState(seed);
            var left = World.Unit(s, Side.A, Row.Front, "shield_wall");
            var mid = World.Unit(s, Side.A, Row.Front);
            World.Unit(s, Side.A, Row.Front);
            var behind = World.Unit(s, Side.A, Row.Middle);           // one card at 0: 100% overlap with the middle wall unit
            World.Unit(s, Side.B, Row.Front);
            BattleStart.Install(s, World.Catalog);
            Statuses.Apply(s, new StatusApplication(left, StatusIds.ShieldWall, Duration: 10, SourceUnitId: left));
            var r = Interception.Resolve(s, behind, 8);
            Assert.True(r.Intercepted);
            Assert.Equal(mid, r.Shares[0].InterceptorId);             // the nearest wall unit that overlaps it
        }
    }

    [Fact]
    public void TheWallDoesNotCoverAUnitInFrontOfIt()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Middle, "shield_wall");
        var front = World.Unit(s, Side.A, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(g, StatusIds.ShieldWall, Duration: 10, SourceUnitId: g));
        Assert.Empty(Interception.Resolve(s, front, 8).Shares);
    }

    // ---- Leap to Defend ----

    [Fact]
    public void LeapToDefendMovesTheGuardianNextToAnAllyInAnyRowAndGuardsItForTheNextAttack()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Rear, "leap_to_defend");
        var ally = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        BattleStart.Install(s, World.Catalog);
        var options = World.Catalog.Get("leap_to_defend").PlacementOptionsFor(s, g, ally);
        Assert.Equal(new[] { new MoveOption(Row.Front, 0), new MoveOption(Row.Front, 1) }, options);
        Assert.Equal("needs a place", Why(s, Use(g, "leap_to_defend", ally)));
        Run(s, Use(g, "leap_to_defend", ally, new MoveOption(Row.Front, 0)));
        Assert.Equal(new[] { g, ally }, s.Formation.Units(Side.A, Row.Front));
        Assert.True(Statuses.Has(s.GetUnit(ally), StatusIds.LeapCover));

        Run(s, Use(enemy, "brawl_strike", ally));                    // the next attack on the ally hits the Guardian instead
        Assert.Equal(40, s.GetUnit(ally).Hp);
        Assert.True(s.GetUnit(g).Hp < 40);
        Assert.False(Statuses.Has(s.GetUnit(ally), StatusIds.LeapCover));     // once

        Ready(s, enemy);
        Run(s, Use(enemy, "brawl_strike", ally));
        Assert.True(s.GetUnit(ally).Hp < 40);                        // the second attack is not covered
    }

    [Fact]
    public void LeapToDefendTakesAnyKindOfAttackEvenADirectOne()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Rear, "leap_to_defend");
        var ally = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front, "breath");
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(g, "leap_to_defend", ally, new MoveOption(Row.Front, 1)));
        Run(s, Use(enemy, "breath", ally));
        Assert.True(s.GetUnit(g).Hp < 40);
    }

    [Fact]
    public void LeapToDefendEndsWhenTheGuardianIsKoAndCannotTargetItselfOrAnEnemy()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Rear, "leap_to_defend");
        var ally = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        BattleStart.Install(s, World.Catalog);
        Assert.Equal("not a valid target for this skill", Why(s, Use(g, "leap_to_defend", g, new MoveOption(Row.Rear, 0))));
        Assert.Equal("not a legal target", Why(s, Use(g, "leap_to_defend", enemy, new MoveOption(Row.Front, 0))));
        Run(s, Use(g, "leap_to_defend", ally, new MoveOption(Row.Front, 0)));
        s.GetUnit(g).Hp = 0;
        Knockout.Resolve(s, g);
        Run(s, Use(enemy, "brawl_strike", ally));
        Assert.True(s.GetUnit(ally).Hp < 40);                        // no Guardian to take it
    }

    [Fact]
    public void LeapToDefendInTheAlliesRowOnlyWorksIfAlreadyNextToIt()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "leap_to_defend");
        var adjacent = World.Unit(s, Side.A, Row.Front);
        var far = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        var skill = World.Catalog.Get("leap_to_defend");
        Assert.Equal(new[] { new MoveOption(Row.Front, 0) }, skill.PlacementOptionsFor(s, g, adjacent));
        Assert.Empty(skill.PlacementOptionsFor(s, g, far));
    }

    // ---- Bodyguard ----

    [Fact]
    public void RescueSwapsPlacesWithAnAllyBelowHalfHp()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "rescue");
        var hurt = World.Unit(s, Side.A, Row.Rear);
        var healthy = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(hurt).Hp = 19;                                     // below half of 40
        s.ShiftMomentum(Side.A, 2);
        Assert.Equal("not a valid target for this skill", Why(s, Use(g, "rescue", healthy)));
        Run(s, Use(g, "rescue", hurt));
        Assert.Equal(Row.Rear, s.Formation.PositionOf(g).Row);
        Assert.Equal(Row.Front, s.Formation.PositionOf(hurt).Row);
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void RescueNeedsAnAllyBelowHalfOnlyAtExactlyHalfItDoesNotWork()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "rescue");
        var half = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(half).Hp = 20;
        s.ShiftMomentum(Side.A, 2);
        Assert.Equal("not a valid target for this skill", Why(s, Use(g, "rescue", half)));
    }

    [Fact]
    public void TakeAChargeMakesOneAllyTheChargeAtATime()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "take_a_charge");
        var a1 = World.Unit(s, Side.A, Row.Middle);
        var a2 = World.Unit(s, Side.A, Row.Rear);
        World.Unit(s, Side.B, Row.Front);
        Run(s, Use(g, "take_a_charge", a1));
        Assert.Equal(a1, GuardianSkillsHelper.ChargeOf(s, g));
        Ready(s, g);
        Run(s, Use(g, "take_a_charge", a2));
        Assert.Equal(a2, GuardianSkillsHelper.ChargeOf(s, g));
        Assert.False(Statuses.Has(s.GetUnit(a1), StatusIds.Charge));
        Assert.Equal("not a valid target for this skill", Why(s, Use(g, "take_a_charge", g)));
    }

    [Fact]
    public void FollowTheChargeMovesTheGuardianIntoTheRowInFrontOfTheChargeWhenItMoves()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "follow_the_charge");
        var charge = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(charge, StatusIds.Charge, SourceUnitId: g));
        Actions.Move(s, charge, Row.Rear, 0);                         // the Charge moves back
        Assert.Equal(Row.Rear, s.Formation.PositionOf(charge).Row);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(g).Row);      // the Guardian stays lined up in front of it
        Assert.Null(s.Timeline.TurnOf(g));                            // free: no Delay was paid
    }

    [Fact]
    public void FollowTheChargeAlsoFollowsAForcedMoveButNotWhenTheChargeIsInFrontOrTheSpotIsTaken()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "follow_the_charge");
        var charge = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(charge, StatusIds.Charge, SourceUnitId: g));
        Movement.ForceMove(s, charge, Row.Rear, Side.B);
        Assert.Equal(Row.Middle, s.Formation.PositionOf(g).Row);

        var s2 = new BattleState(1);
        var g2 = World.Unit(s2, Side.A, Row.Rear, "follow_the_charge");
        var c2 = World.Unit(s2, Side.A, Row.Rear);
        World.Unit(s2, Side.A, Row.Middle);                           // a unit already stands in front of the Charge
        World.Unit(s2, Side.B, Row.Front);
        BattleStart.Install(s2, World.Catalog);
        Statuses.Apply(s2, new StatusApplication(c2, StatusIds.Charge, SourceUnitId: g2));
        Actions.Move(s2, c2, Row.Front, 0);                           // moves to the Front: nothing is in front of it
        Assert.Equal(Row.Rear, s2.Formation.PositionOf(g2).Row);
    }

    [Fact]
    public void DistractMakesTheEnemyThatLastHitTheChargeAttackTheGuardianIfItCan()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "distract", "take_a_charge");
        var charge = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        World.Unit(s, Side.B, Row.Rear);
        Statuses.Apply(s, new StatusApplication(charge, StatusIds.Charge, SourceUnitId: g));
        Assert.Equal("no enemy has hit the Charge", Why(s, Use(g, "distract")));
        Run(s, Use(enemy, "brawl_strike", charge));                   // the enemy hits the Charge
        Ready(s, enemy);
        Run(s, Use(g, "distract"));
        Assert.True(Statuses.Has(s.GetUnit(enemy), StatusIds.Distracted));

        Assert.Equal("Distracted: must target the Guardian", Why(s, Use(enemy, "brawl_strike", charge)));
        Assert.Null(Why(s, Use(enemy, "brawl_strike", g)));
        Assert.Equal(new[] { g }, Actions.LegalTargets(s, World.Catalog, enemy, "brawl_strike"));
        Run(s, Use(enemy, "brawl_strike", g));
        Assert.False(Statuses.Has(s.GetUnit(enemy), StatusIds.Distracted));     // it ended after that attack
        Ready(s, enemy);
        Assert.Null(Why(s, Use(enemy, "brawl_strike", charge)));
    }

    [Fact]
    public void DistractChangesNothingWhenTheGuardianIsNotALegalTarget()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Rear, "distract");
        var charge = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front, "brawl_strike");
        Statuses.Apply(s, new StatusApplication(charge, StatusIds.Charge, SourceUnitId: g));
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Distracted, SourceUnitId: g));
        Assert.Null(Why(s, Use(enemy, "brawl_strike", charge)));       // melee D1 cannot reach the Rear Guardian: no restriction
    }

    [Fact]
    public void PledgeMakesAreaEffectsHitTheGuardianInsteadOfTheChargeOnce()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "pledge");
        var charge = World.Unit(s, Side.A, Row.Front);
        var other = World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front, "breath");
        BattleStart.Install(s, World.Catalog);
        Statuses.Apply(s, new StatusApplication(charge, StatusIds.Charge, SourceUnitId: g));
        s.ShiftMomentum(Side.A, 5);
        Run(s, Use(g, "pledge"));
        Assert.True(Statuses.Has(s.GetUnit(g), StatusIds.Pledge));
        Ready(s, enemy);
        s.Timeline.Remove(s.Timeline.TurnOf(enemy)?.Id ?? -1);

        // Breath on the Charge's neighbor: the splash would reach the Charge, so the Guardian takes it instead (once, in the same block).
        s.Momentum.Shift(Side.A, 0);
        Run(s, Use(enemy, "breath", other));
        Assert.Equal(40, s.GetUnit(charge).Hp);
        Assert.True(s.GetUnit(g).Hp < 40);
    }

    [Fact]
    public void PledgeNeedsAChargeAndEndsAfterTenTimeUnits()
    {
        var s = new BattleState(1);
        var g = World.Unit(s, Side.A, Row.Front, "pledge");
        var charge = World.Unit(s, Side.A, Row.Middle);
        World.Unit(s, Side.B, Row.Front);
        s.ShiftMomentum(Side.A, 2);
        Assert.Equal("needs a Charge", Why(s, Use(g, "pledge")));
        Statuses.Apply(s, new StatusApplication(charge, StatusIds.Charge, SourceUnitId: g));
        Run(s, Use(g, "pledge"));
        Assert.Equal(10, Statuses.Get(s.GetUnit(g), StatusIds.Pledge)!.EndsAt);
        AdvanceTo(s, 12);
        Assert.False(Statuses.Has(s.GetUnit(g), StatusIds.Pledge));
    }
}

/// <summary>Test access to the Guardian's Charge lookup.</summary>
internal static class GuardianSkillsHelper
{
    public static int? ChargeOf(BattleState state, int guardianId) =>
        state.Units.FirstOrDefault(u => Statuses.Get(u, StatusIds.Charge)?.SourceUnitId == guardianId)?.Id;
}
