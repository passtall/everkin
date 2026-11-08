using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class StatusTests
{
    private static (BattleState State, int A, int B) Duel(int hp = 40)
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.B, Row.Front);
        s.GetUnit(a).Hp = s.GetUnit(a).MaxHp = hp;
        s.GetUnit(b).Hp = s.GetUnit(b).MaxHp = hp;
        s.DrainEvents();
        return (s, a, b);
    }

    private static void AdvanceTo(BattleState s, int time)
    {
        // Moves the clock by popping a delayed event placed at that time (other entries due earlier are popped first).
        s.Timeline.Add(TimelineKind.DelayedEvent, time - s.Timeline.Now, tag: "clock");
        while (s.Timeline.Pop() is { } e)
        {
            if (e.Tag == "clock") break;
            Statuses.HandleExpiry(s, e);
        }
    }

    // ---- application, refresh, expiry ----

    [Fact]
    public void AStatusHasOneInstanceAndAMarkerOnTheTimeline()
    {
        var (s, a, _) = Duel();
        Assert.True(Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50)));
        var instance = Statuses.Get(s.GetUnit(a), StatusIds.Poison)!;
        Assert.Equal(50, instance.EndsAt);
        var marker = s.Timeline.Find(instance.MarkerEntryId!.Value)!;
        Assert.Equal(TimelineKind.EffectEnd, marker.Kind);
        Assert.Equal(a, marker.OwnerUnitId);
        Assert.Equal(StatusIds.Poison, marker.Tag);
        Assert.Equal(new BattleEvent[] { new StatusApplied(a, StatusIds.Poison, 0, 50, false) }, s.DrainEvents());
    }

    [Fact]
    public void PoisonAppliedTwiceIsStillOneInstanceWithTheDurationRefreshed()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        Assert.Single(s.GetUnit(a).StatusInstances);
        Assert.Single(s.Timeline.Upcoming().Where(e => e.Kind == TimelineKind.EffectEnd));
    }

    [Fact]
    public void ANewDurationAlwaysReplacesTheOldEvenIfShorter()
    {
        // 20.5: Poison with 30 time units left is reapplied by a skill giving 20: it now has 20 left.
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        AdvanceTo(s, 20); // 30 left
        var left = Statuses.Get(s.GetUnit(a), StatusIds.Poison)!.EndsAt - s.Timeline.Now;
        Assert.Equal(30, left);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 20));
        Assert.Equal(20, Statuses.Get(s.GetUnit(a), StatusIds.Poison)!.EndsAt - s.Timeline.Now);
    }

    [Fact]
    public void AStatusEndsWhenItsMarkerArrivesAndIsReported()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        s.DrainEvents();
        AdvanceTo(s, 25);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Blind));
        Assert.Contains(new StatusEnded(a, StatusIds.Blind, StatusEndCause.Expired), s.DrainEvents());
    }

    [Fact]
    public void AStaleMarkerOfAReplacedStatusDoesNothing()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        var oldMarker = s.Timeline.Upcoming()[0];
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 40));
        Statuses.HandleExpiry(s, oldMarker); // not on the timeline any more
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Blind));
    }

    [Fact]
    public void AStatusEndThatJoinedFirstEndsBeforeTheTurnThatMovedIntoItsTimeUnit()
    {
        // 20.5: the status ends first, so the unit acts without it.
        var (s, a, _) = Duel();
        s.Timeline.Add(TimelineKind.UnitTurn, 8, a);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Chilled, Duration: 12));
        s.Timeline.PushBack(s.Timeline.TurnOf(a)!, 4); // joins time unit 12 behind the marker
        var first = s.Timeline.Pop()!;
        Assert.Equal(TimelineKind.EffectEnd, first.Kind);
    }

    [Fact]
    public void ADurationRunsOnRegardlessOfWhatTheUnitDoes()
    {
        // 20.5: a Blind unit chooses Skip Turn: Blind's 20 time units keep running.
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        Statuses.OnTurnStart(s, a);
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Blind));
        Assert.Equal(20, Statuses.Get(s.GetUnit(a), StatusIds.Blind)!.EndsAt);
    }

    [Fact]
    public void AKoUnitCannotGainStatuses()
    {
        var (s, a, _) = Duel();
        s.GetUnit(a).Hp = 0;
        Assert.Throws<InvalidOperationException>(() => Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 5)));
    }

    // ---- Stunned ----

    [Fact]
    public void ANewStunKeepsWhicheverTimeIsLonger()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 10));
        Assert.False(Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 5)));
        Assert.Equal(10, Statuses.Get(s.GetUnit(a), StatusIds.Stunned)!.EndsAt);
        Assert.True(Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 15)));
        Assert.Equal(15, Statuses.Get(s.GetUnit(a), StatusIds.Stunned)!.EndsAt);
    }

    [Fact]
    public void AnyHitEndsStunnedEarlyAndStunnedCannotIntercept()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 10));
        Assert.True(s.GetUnit(a).InterceptionBlocked);
        Assert.False(Interception.IsEligible(s.GetUnit(a)));
        Statuses.OnHit(s, a);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Stunned));
        Assert.False(s.GetUnit(a).InterceptionBlocked);
        Assert.Empty(s.Timeline.Upcoming()); // its marker is gone too
    }

    [Fact]
    public void AStunnedUnitReportsThatItTakesNoActions()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 10));
        var r = Statuses.OnTurnStart(s, a);
        Assert.True(r.Stunned);
        Assert.True(r.TurnSkipped);                      // skipped at the cost of a skipped turn (decided 2026-11-08)
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Stunned)); // the Stun keeps running its own time
        Assert.Equal(11, Timeline.SkippedTurnWait(5));
    }

    [Fact]
    public void AFrozenTurnIsSkippedToo()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Frozen));
        Assert.True(Statuses.OnTurnStart(s, a).TurnSkipped);
        Assert.False(Statuses.OnTurnStart(s, a).TurnSkipped);
    }

    // ---- Frozen, Immobilized ----

    [Fact]
    public void AFrozenUnitsTurnIsSkippedAndFrozenEnds()
    {
        // 20.5.
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Frozen));
        Assert.True(s.GetUnit(a).InterceptionBlocked);
        var r = Statuses.OnTurnStart(s, a);
        Assert.True(r.FrozenTurnSkipped);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Frozen));
        Assert.False(s.GetUnit(a).InterceptionBlocked);
        Assert.False(Statuses.OnTurnStart(s, a).FrozenTurnSkipped);
        // The skipped turn costs 5 + (11 - Speed).
        Assert.Equal(11, Timeline.SkippedTurnWait(5));
    }

    [Fact]
    public void FireOnAFrozenUnitsSpotEndsFrozenEarlyAndTheUnitKeepsItsTurn()
    {
        var (s, a, _) = Duel();
        s.Timeline.Add(TimelineKind.UnitTurn, 8, a);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Frozen));
        Assert.True(Statuses.Remove(s, a, StatusIds.Frozen, StatusEndCause.Fire));
        Assert.NotNull(s.Timeline.TurnOf(a));
        Assert.False(Statuses.OnTurnStart(s, a).FrozenTurnSkipped);
    }

    [Fact]
    public void ImmobilizedSetsTheMovementAndInterceptionFlagsAndStunnedOrFrozenKeepTheBlock()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Immobilized, Duration: 20));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 10));
        var unit = s.GetUnit(a);
        Assert.True(unit.IsImmobilized);
        Assert.True(unit.InterceptionBlocked);
        Assert.Empty(Movement.LegalMoves(s, a));
        Statuses.Remove(s, a, StatusIds.Immobilized, StatusEndCause.Cleansed);
        Assert.False(unit.IsImmobilized);
        Assert.True(unit.InterceptionBlocked); // still Stunned
        Statuses.Remove(s, a, StatusIds.Stunned, StatusEndCause.Cleansed);
        Assert.False(unit.InterceptionBlocked);
    }

    // ---- Ward and Dampen ----

    [Fact]
    public void ANewWardReplacesTheOldOnlyIfItAbsorbsAtLeastAsMuchAsIsLeft()
    {
        // 20.5: Ward 3 left gets Ward 2: keeps 3. Ward 4 gets Ward 8: replaced.
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 3));
        Assert.False(Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 2)));
        Assert.Equal(3, s.GetUnit(a).Ward);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 4));
        Assert.True(Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 8)));
        Assert.Equal(8, s.GetUnit(a).Ward);
    }

    [Fact]
    public void ABrokenWardLeavesNoInstanceOrMarkerBehind()
    {
        var (s, a, b) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Duration: 30, Magnitude: 4));
        s.DrainEvents();
        s.DealDamage(b, a, 6);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Ward));
        Assert.Equal(38, s.GetUnit(a).Hp);
        Assert.Empty(s.Timeline.Upcoming());
        Assert.Contains(new StatusEnded(a, StatusIds.Ward, StatusEndCause.Broken), s.DrainEvents());
    }

    [Fact]
    public void AWardLastsUntilTheCastersNextTurn()
    {
        var (s, a, b) = Duel();
        Statuses.Apply(s, new StatusApplication(b, StatusIds.Ward, Magnitude: 4, SourceUnitId: a, EndsAtNextTurnOf: a));
        Statuses.OnTurnStart(s, b);
        Assert.True(Statuses.Has(s.GetUnit(b), StatusIds.Ward)); // not its owner's turn
        Statuses.OnTurnStart(s, a);
        Assert.False(Statuses.Has(s.GetUnit(b), StatusIds.Ward));
        Assert.Equal(0, s.GetUnit(b).Ward);
    }

    [Fact]
    public void DampenAsAStatusHalvesDamageAndExpires()
    {
        var (s, a, b) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Dampen, Duration: 10));
        Assert.True(s.GetUnit(a).Dampen);
        Assert.Equal(4, s.DealDamage(b, a, 9).HpLost);
        AdvanceTo(s, 12);
        Assert.False(s.GetUnit(a).Dampen);
    }

    // ---- Poison ----

    [Fact]
    public void PoisonBuildsUpPerTimeUnitAndLandsWheneverAnyUnitGetsATurnFiveInTotal()
    {
        // 20.5: 1 damage per 10 time units for 50 units, 5 in total, landing whenever any unit gets a turn.
        var (s, a, b) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50, SourceUnitId: b));
        AdvanceTo(s, 15);
        Assert.Equal(new[] { a }, Statuses.TickPoison(s));       // 150 hundredths: 1 lands, 50 carried
        Assert.Equal(39, s.GetUnit(a).Hp);
        Assert.Equal(50, Statuses.Get(s.GetUnit(a), StatusIds.Poison)!.BuildupHundredths);
        Assert.Empty(Statuses.TickPoison(s));                    // nothing new yet
        AdvanceTo(s, 40);
        Statuses.TickPoison(s);                                  // 50 + 250 = 300: 3 land
        Assert.Equal(36, s.GetUnit(a).Hp);
        AdvanceTo(s, 55);                                        // the marker at 50 lands the rest and ends it
        Assert.Equal(35, s.GetUnit(a).Hp);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Poison));
    }

    [Fact]
    public void ALongGapBetweenTurnsStillLandsEverythingBuiltUp()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        AdvanceTo(s, 55);
        Assert.Equal(35, s.GetUnit(a).Hp); // 5 in total even though no turn came in between
    }

    [Fact]
    public void APoisonTickIsNotAbsorbedByAWardDoesNotEndStealthAndDoesNotMoveMomentum()
    {
        // 20.5.
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 8));
        s.GetUnit(a).IsStealthed = true;
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(39, s.GetUnit(a).Hp);
        Assert.Equal(8, s.GetUnit(a).Ward);
        Assert.True(s.GetUnit(a).IsStealthed);
        Assert.Equal(0, s.Momentum.Value);
    }

    [Fact]
    public void PoisonKeepsTickingWhenItsCasterIsKo()
    {
        // 20.5.
        var (s, a, b) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50, SourceUnitId: b));
        s.GetUnit(b).Hp = 0;
        Knockout.Resolve(s, b);
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(39, s.GetUnit(a).Hp);
    }

    [Fact]
    public void PoisonIsReducedByDampenWithAMinimumOfOne()
    {
        var (s, a, _) = Duel();
        s.GetUnit(a).Dampen = true;
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        AdvanceTo(s, 10);
        Statuses.TickPoison(s);
        Assert.Equal(39, s.GetUnit(a).Hp);
    }

    // ---- Bleeding ----

    [Fact]
    public void BleedingHurtsAfterEachMeleeOrMoveSkillAndCountsDown()
    {
        // 20.5: Bleeding (3 skills left) uses a melee skill: 2 damage, 2 skills remain.
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Bleeding, Uses: 3));
        Assert.True(Statuses.OnSkillUsed(s, a, melee: true, move: false));
        Assert.Equal(38, s.GetUnit(a).Hp);
        Assert.Equal(2, Statuses.Get(s.GetUnit(a), StatusIds.Bleeding)!.UsesLeft);
    }

    [Fact]
    public void BleedingDoesNotTriggerOnAHealOrAProjectileButDoesOnMove()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Bleeding, Uses: 3));
        Assert.False(Statuses.OnSkillUsed(s, a, melee: false, move: false)); // a heal, a Bolt
        Assert.Equal(40, s.GetUnit(a).Hp);
        Assert.Equal(3, Statuses.Get(s.GetUnit(a), StatusIds.Bleeding)!.UsesLeft);
        Assert.True(Statuses.OnSkillUsed(s, a, melee: false, move: true));    // Move, Creep Closer
        Assert.Equal(38, s.GetUnit(a).Hp);
    }

    [Fact]
    public void BleedingEndsAfterItsLastSkillAndIsNotAbsorbedByAWardButIsReducedByDampen()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Bleeding, Uses: 1));
        s.GetUnit(a).Ward = 8;
        s.GetUnit(a).Dampen = true;
        Statuses.OnSkillUsed(s, a, melee: true, move: false);
        Assert.Equal(39, s.GetUnit(a).Hp);   // 2 halved to 1
        Assert.Equal(8, s.GetUnit(a).Ward);
        Assert.False(Statuses.Has(s.GetUnit(a), StatusIds.Bleeding));
    }

    // ---- Blind, Silenced, Confused ----

    [Fact]
    public void BlindGivesAFiftyPercentMissChanceAndSilencedBlocksOnlyMagicalSkills()
    {
        var (s, a, _) = Duel();
        var unit = s.GetUnit(a);
        Assert.Equal(0, Statuses.AttackMissChance(unit));
        Assert.True(Statuses.CanUseMagical(unit));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Silenced, Duration: 20));
        Assert.Equal(50, Statuses.AttackMissChance(unit));
        Assert.False(Statuses.CanUseMagical(unit)); // Mend is magical; a physical creature attack still works
    }

    [Fact]
    public void ConfusionRollsOnlyWhenConfusedAndHalfTheTime()
    {
        var (s, a, _) = Duel();
        var before = s.Random.Clone();
        Assert.False(Statuses.RollConfusion(s, a));
        Assert.Equal(before.NextUInt64(), s.Random.NextUInt64()); // no draw when not confused

        var hits = 0;
        for (ulong seed = 1; seed <= 4000; seed++)
        {
            var state = new BattleState(seed);
            var id = Place.Unit(state, Side.A, Row.Front);
            Statuses.Apply(state, new StatusApplication(id, StatusIds.Confused, Duration: 20));
            if (Statuses.RollConfusion(state, id)) hits++;
        }
        Assert.InRange(hits, 1800, 2200);
    }

    // ---- Speed ----

    [Fact]
    public void ChilledLowersSpeedByTwoAndLengthensTheRemainingWait()
    {
        var (s, a, _) = Duel();
        s.GetUnit(a).Speed = 6;
        var turn = s.Timeline.Add(TimelineKind.UnitTurn, 10, a);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Chilled, Duration: 20));
        Assert.Equal(4, s.GetUnit(a).EffectiveSpeed);
        Assert.Equal(12, s.Timeline.Remaining(turn));
        Statuses.Remove(s, a, StatusIds.Chilled, StatusEndCause.Expired);
        Assert.Equal(10, s.Timeline.Remaining(turn)); // expiry reverses it
    }

    [Fact]
    public void HasteAfterFourOfElevenTimeUnitsTurnsSevenRemainingIntoFive()
    {
        // 20.5.
        var (s, a, _) = Duel();
        s.GetUnit(a).Speed = 5;
        var turn = s.Timeline.Add(TimelineKind.UnitTurn, 11, a);
        AdvanceTo(s, 4);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Haste, Duration: 20, Magnitude: 2));
        Assert.Equal(5, s.Timeline.Remaining(turn));
        Assert.Equal(7, s.GetUnit(a).EffectiveSpeed);
    }

    [Fact]
    public void SlowAndHasteCancelAndEachExpiresOnItsOwn()
    {
        var (s, a, _) = Duel();
        s.GetUnit(a).Speed = 5;
        var turn = s.Timeline.Add(TimelineKind.UnitTurn, 11, a);
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Haste, Duration: 20, Magnitude: 2));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Slow, Duration: 30, Magnitude: 2));
        Assert.Equal(5, s.GetUnit(a).EffectiveSpeed);
        Assert.Equal(11, s.Timeline.Remaining(turn));
        Statuses.Remove(s, a, StatusIds.Haste, StatusEndCause.Expired);
        Assert.Equal(3, s.GetUnit(a).EffectiveSpeed);
        Assert.Equal(13, s.Timeline.Remaining(turn));
    }

    // ---- Airborne ----

    [Fact]
    public void MeleeCannotReachAnAirborneUnitButProjectilesAndOtherAirborneUnitsCan()
    {
        var (s, a, b) = Duel();
        var reach = Reach.Distance(1);
        Statuses.Apply(s, new StatusApplication(b, StatusIds.Airborne, Duration: 20));
        Assert.True(s.GetUnit(b).IsAirborne);
        Assert.False(Targeting.IsLegalTarget(s, a, b, reach, new TargetOptions(Delivery: Delivery.Melee)));
        Assert.True(Targeting.IsLegalTarget(s, a, b, reach, new TargetOptions(Delivery: Delivery.Projectile)));
        Assert.True(Targeting.IsLegalTarget(s, a, b, reach, new TargetOptions(Delivery: Delivery.Direct)));
        Assert.True(Targeting.IsLegalTarget(s, a, b, reach)); // delivery not given: reach only
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Airborne, Duration: 20));
        Assert.True(Targeting.IsLegalTarget(s, a, b, reach, new TargetOptions(Delivery: Delivery.Melee)));
    }

    [Fact]
    public void AnAirborneUnitStillInterceptsIsStillHitByAreasAndLandsWhenItEnds()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Airborne, Duration: 20));
        Assert.True(Interception.IsEligible(s.GetUnit(a)));
        Assert.Contains(a, AreaPatterns.Row(s, Side.A, Row.Front));
        AdvanceTo(s, 25);
        Assert.False(s.GetUnit(a).IsAirborne);
    }

    [Fact]
    public void ASkillCanMakeAnAirborneUnitLand()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Airborne, Duration: 40));
        Assert.True(Statuses.Remove(s, a, StatusIds.Airborne, StatusEndCause.Landed));
        Assert.False(s.GetUnit(a).IsAirborne);
        Assert.Empty(s.Timeline.Upcoming());
    }

    // ---- KO, cleanse, clone ----

    [Fact]
    public void KoRemovesEveryStatusAndItsMarkers()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Stunned, Duration: 10));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Airborne, Duration: 20));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Haste, Duration: 20, Magnitude: 2));
        s.GetUnit(a).Hp = 0;
        Knockout.Resolve(s, a);
        var unit = s.GetUnit(a);
        Assert.Empty(unit.StatusInstances);
        Assert.False(unit.IsAirborne);
        Assert.False(unit.InterceptionBlocked);
        Assert.Empty(s.Timeline.Upcoming());
        Assert.Equal(unit.Speed, unit.EffectiveSpeed);
    }

    [Fact]
    public void AbsolveRemovesHarmfulStatusesAndDispelRemovesHelpfulOnesWardIncluded()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 4));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Haste, Duration: 20, Magnitude: 2));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Airborne, Duration: 20));
        var harmful = Statuses.RemoveAll(s, a, Polarity.Harmful);
        Assert.Equal(new[] { StatusIds.Poison, StatusIds.Blind }.OrderBy(x => x), harmful.OrderBy(x => x));
        var helpful = Statuses.RemoveAll(s, a, Polarity.Helpful);
        Assert.Equal(new[] { StatusIds.Haste, StatusIds.Ward }.OrderBy(x => x), helpful.OrderBy(x => x));
        Assert.True(s.GetUnit(a).IsAirborne); // Airborne is not classified: neither cleanse touches it
        Assert.Equal(0, s.GetUnit(a).Ward);
    }

    [Fact]
    public void CloneKeepsStatusesIndependently()
    {
        var (s, a, _) = Duel();
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 50));
        var copy = s.Clone();
        Statuses.Remove(copy, a, StatusIds.Poison, StatusEndCause.Cleansed);
        Assert.True(Statuses.Has(s.GetUnit(a), StatusIds.Poison));
        Assert.False(Statuses.Has(copy.GetUnit(a), StatusIds.Poison));
        Assert.Single(s.Timeline.Upcoming());
        Assert.Empty(copy.Timeline.Upcoming());
    }

    [Fact]
    public void MarkedIsAnOrdinaryTimedStatus()
    {
        var (s, a, b) = Duel();
        Statuses.Apply(s, new StatusApplication(b, StatusIds.Marked, Duration: 40, SourceUnitId: a));
        Assert.True(Statuses.Has(s.GetUnit(b), StatusIds.Marked));
        Assert.Equal(a, Statuses.Get(s.GetUnit(b), StatusIds.Marked)!.SourceUnitId);
    }
}
