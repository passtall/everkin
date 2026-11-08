using Everkin.Core.Battle;

namespace Everkin.Core.Tests;

public class ReactionTests
{
    /// <summary>A test counter: when its owner is hit by someone, it hits that unit back for a fixed amount.</summary>
    private static ReactionDef Counter(
        int owner, string id, int damage, EffectTags ownTags = EffectTags.Melee, EffectTags required = EffectTags.None,
        int? uses = null, ReactionPriority priority = ReactionPriority.Normal, bool canChain = true) =>
        new(id, owner, TriggerKind.WhenHit, (s, ctx) =>
        {
            if (ctx.Trigger.SourceUnitId is not { } attacker || s.GetUnit(attacker).IsKnockedOut) return;
            s.DealDamage(owner, attacker, damage); // a reaction's hit goes straight to its target: no interception
            ctx.Raise(new Trigger(TriggerKind.WhenHit, attacker, owner, ownTags, damage));
        })
        { RequiredTags = required, MaxUsesPerBattle = uses, Priority = priority, CanChain = canChain };

    private static Trigger Hit(int subject, int source, EffectTags tags = EffectTags.Melee) =>
        new(TriggerKind.WhenHit, subject, source, tags);

    private static (BattleState S, int A, int B, int C) Three()
    {
        var s = new BattleState(1);
        var a = Place.Unit(s, Side.A, Row.Front);
        var b = Place.Unit(s, Side.B, Row.Front);
        var c = Place.Unit(s, Side.B, Row.Middle);
        s.DrainEvents();
        return (s, a, b, c);
    }

    // ---- chains (game_system 20.5) ----

    [Fact]
    public void ACountersHitCanTriggerTheAttackersOwnCounterAndTheChainEndsWhenBothAreSpent()
    {
        // 20.5: two units each have a once-per-battle `melee` test counter, and one hits the other in melee.
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2, uses: 1));
        s.AddReaction(Counter(a, "a_counter", 2, uses: 1));
        var r = Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(2, r.Fired);
        Assert.False(r.Aborted);
        Assert.Equal(38, s.GetUnit(a).Hp);   // the target's counter hit the attacker
        Assert.Equal(38, s.GetUnit(b).Hp);   // the attacker's counter answered it
        Assert.Equal(1, s.ReactionUses(s.Reactions[0]));
        Assert.Equal(1, s.ReactionUses(s.Reactions[1]));
    }

    [Fact]
    public void ACounterHitsTheAttackerWhoseOwnCounterThenTriggersOnThatHit()
    {
        // 20.5: the attacker's counter triggers on that hit, since reactions can trigger reactions.
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2, uses: 1));
        s.AddReaction(Counter(a, "a_counter", 2, uses: 1));
        s.DrainEvents();
        Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(new[] { "b_counter", "a_counter" },
            s.DrainEvents().OfType<ReactionFired>().Select(e => e.ReactionId));
    }

    [Fact]
    public void AReactionHasItsOwnTagsSoItsHitIsNotMeleeUnlessItSaysSo()
    {
        // 20.5: a reaction created by a `melee` skill whose own text does not say `melee`: its hits are not `melee`.
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2, ownTags: EffectTags.Physical, uses: 1));
        s.AddReaction(Counter(a, "a_melee_counter", 2, required: EffectTags.Melee, uses: 1));
        var r = Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(1, r.Fired); // the melee-only counter did not answer the non-melee counter hit
        Assert.Equal(40, s.GetUnit(b).Hp);
    }

    [Fact]
    public void ACounterThatSaysMeleeCanSetOffMeleeOnlyCounters()
    {
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2, ownTags: EffectTags.Melee, uses: 1));
        s.AddReaction(Counter(a, "a_melee_counter", 2, required: EffectTags.Melee, uses: 1));
        Assert.Equal(2, Reactions.Resolve(s, new[] { Hit(b, a) }).Fired);
    }

    [Fact]
    public void ARequiredTagStopsAReactionFromAnsweringOtherHits()
    {
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_melee_counter", 2, required: EffectTags.Melee));
        Assert.Equal(0, Reactions.Resolve(s, new[] { Hit(b, a, EffectTags.Projectile) }).Fired);
        Assert.Equal(1, Reactions.Resolve(s, new[] { Hit(b, a, EffectTags.Melee | EffectTags.Physical) }).Fired);
    }

    [Fact]
    public void AMultiHitSkillTriggersAOncePerHitReactionOncePerHit()
    {
        // 20.5: a multi-hit skill hits a unit with a once-per-hit reaction three times: it triggers three times.
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 1, ownTags: EffectTags.None));
        var r = Reactions.Resolve(s, new[] { Hit(b, a), Hit(b, a), Hit(b, a) });
        Assert.Equal(3, r.Fired);
        Assert.Equal(37, s.GetUnit(a).Hp);
    }

    [Fact]
    public void AnOncePerBattleReactionFiresOnlyOnce()
    {
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 1, ownTags: EffectTags.None, uses: 1));
        Assert.Equal(1, Reactions.Resolve(s, new[] { Hit(b, a), Hit(b, a) }).Fired);
        Assert.Equal(0, Reactions.Resolve(s, new[] { Hit(b, a) }).Fired);
    }

    // ---- KO ----

    [Fact]
    public void AUnitKoedByTheHitDoesNotReact()
    {
        // 20.5: a unit is KO'd by a hit and had a "when hit" counter: the counter does not fire.
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2));
        s.GetUnit(b).Hp = 0;
        Assert.Equal(0, Reactions.Resolve(s, new[] { Hit(b, a) }).Fired);
        Assert.Equal(40, s.GetUnit(a).Hp);
    }

    [Fact]
    public void AKoOwnerStillReactsToAnExplicitWhenKoTrigger()
    {
        var (s, _, b, _) = Three();
        var fired = false;
        s.AddReaction(new ReactionDef("last_words", b, TriggerKind.WhenKo, (_, _) => fired = true));
        s.GetUnit(b).Hp = 0;
        Reactions.Resolve(s, new[] { new Trigger(TriggerKind.WhenKo, b) });
        Assert.True(fired);
    }

    [Fact]
    public void AnOwnerKoedByAnEarlierReactionOfTheSameBatchDoesNotReactLater()
    {
        var (s, a, b, c) = Three();
        s.AddReaction(new ReactionDef("finisher", c, TriggerKind.WhenHit, (st, _) => st.DealDamage(c, b, 99))
        { Priority = ReactionPriority.First });
        s.AddReaction(Counter(b, "b_counter", 2));
        var r = Reactions.Resolve(s, new[] { Hit(c, a), Hit(b, a) });
        Assert.Equal(1, r.Fired);
        Assert.Equal(40, s.GetUnit(a).Hp);
    }

    // ---- order ----

    [Fact]
    public void FirstGoesBeforeNormalAndNormalBeforeLastWhateverTheRegistrationOrder()
    {
        // 20.5: a counter and a timeline pull trigger from the same hit: the counter (Normal) resolves before the pull (Last).
        var (s, a, b, _) = Three();
        var order = new List<string>();
        ReactionDef R(string id, ReactionPriority p) => new(id, b, TriggerKind.WhenHit, (_, _) => order.Add(id)) { Priority = p };
        s.AddReaction(R("pull", ReactionPriority.Last));
        s.AddReaction(R("counter", ReactionPriority.Normal));
        s.AddReaction(R("cleanse", ReactionPriority.First));
        Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(new[] { "cleanse", "counter", "pull" }, order);
    }

    [Fact]
    public void ReactionsOfOneLevelResolveByTheirOwnersTimelineOrder()
    {
        var s = new BattleState(1);
        var u1 = Place.Unit(s, Side.A, Row.Front);
        var u2 = Place.Unit(s, Side.A, Row.Front);
        var u3 = Place.Unit(s, Side.B, Row.Front);
        s.Timeline.Add(TimelineKind.UnitTurn, 9, u1);
        s.Timeline.Add(TimelineKind.UnitTurn, 3, u2);
        var order = new List<int>();
        s.AddReaction(new ReactionDef("r1", u1, TriggerKind.WhenHit, (_, _) => order.Add(u1)));
        s.AddReaction(new ReactionDef("r2", u2, TriggerKind.WhenHit, (_, _) => order.Add(u2)));
        // One simultaneous block that hits both owners.
        Reactions.Resolve(s, new[] { Hit(u1, u3), Hit(u2, u3) });
        Assert.Equal(new[] { u2, u1 }, order);
    }

    [Fact]
    public void OwnersInTheSameTimeUnitGoInTheOrderTheyJoinedTheTimeline()
    {
        var s = new BattleState(1);
        var u1 = Place.Unit(s, Side.A, Row.Front);
        var u2 = Place.Unit(s, Side.A, Row.Front);
        var other = Place.Unit(s, Side.B, Row.Front);
        s.Timeline.Add(TimelineKind.UnitTurn, 5, u2);   // joined first
        s.Timeline.Add(TimelineKind.UnitTurn, 5, u1);
        var order = new List<int>();
        s.AddReaction(new ReactionDef("r1", u1, TriggerKind.WhenHit, (_, _) => order.Add(u1)));
        s.AddReaction(new ReactionDef("r2", u2, TriggerKind.WhenHit, (_, _) => order.Add(u2)));
        Reactions.Resolve(s, new[] { Hit(u1, other), Hit(u2, other) });
        Assert.Equal(new[] { u2, u1 }, order);
    }

    [Fact]
    public void AReactionsOwnReactionsResolveRightAfterItBeforeTheNextOneOfTheBatch()
    {
        var s = new BattleState(1);
        var b = Place.Unit(s, Side.A, Row.Front);
        var c = Place.Unit(s, Side.A, Row.Front);
        var d = Place.Unit(s, Side.A, Row.Front);
        var enemy = Place.Unit(s, Side.B, Row.Front);
        var order = new List<string>();
        s.AddReaction(new ReactionDef("b_reaction", b, TriggerKind.WhenHit, (_, ctx) =>
        {
            order.Add("b_reaction");
            ctx.Raise(new Trigger(TriggerKind.WhenHit, c, b));
        }));
        s.AddReaction(new ReactionDef("c_reaction", c, TriggerKind.WhenHit, (_, _) => order.Add("c_reaction")));
        s.AddReaction(new ReactionDef("d_last", d, TriggerKind.WhenHit, (_, _) => order.Add("d_last")) { Priority = ReactionPriority.Last });
        Reactions.Resolve(s, new[] { Hit(b, enemy), Hit(d, enemy) });
        Assert.Equal(new[] { "b_reaction", "c_reaction", "d_last" }, order);
    }

    [Fact]
    public void ASimultaneousBlockHittingSeveralUnitsGivesEachItsReaction()
    {
        var (s, a, b, c) = Three();
        s.AddReaction(Counter(b, "b_counter", 1, ownTags: EffectTags.None));
        s.AddReaction(Counter(c, "c_counter", 1, ownTags: EffectTags.None));
        var r = Reactions.Resolve(s, new[] { Hit(b, a), Hit(c, a) });
        Assert.Equal(2, r.Fired);
        Assert.Equal(38, s.GetUnit(a).Hp);
    }

    // ---- chains: limits and safeguards ----

    [Fact]
    public void AnEndlessChainIsStoppedAtTheLimitAndReported()
    {
        // 6.6: no chain limit in the rules; the harness must detect and report chains that do not terminate.
        var (s, a, b, _) = Three();
        s.GetUnit(a).Hp = s.GetUnit(a).MaxHp = 100000;
        s.GetUnit(b).Hp = s.GetUnit(b).MaxHp = 100000;
        s.AddReaction(Counter(b, "b_counter", 1));
        s.AddReaction(Counter(a, "a_counter", 1));
        s.DrainEvents();
        var r = Reactions.Resolve(s, new[] { Hit(b, a) }, maxReactions: 20);
        Assert.True(r.Aborted);
        Assert.Equal(20, r.Fired);
        Assert.Contains(new ReactionChainAborted(20), s.DrainEvents());
    }

    [Fact]
    public void AReactionThatCannotChainRaisesNoFurtherReactions()
    {
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2, canChain: false));
        s.AddReaction(Counter(a, "a_counter", 2));
        var r = Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(1, r.Fired);
        Assert.Equal(38, s.GetUnit(a).Hp);
        Assert.Equal(40, s.GetUnit(b).Hp);
    }

    [Fact]
    public void AReactionsHitGoesThroughShieldsAndReductionsLikeAnyHit()
    {
        var (s, a, b, _) = Three();
        s.GetUnit(a).Ward = 8;
        s.AddReaction(Counter(b, "b_counter", 2, ownTags: EffectTags.None));
        Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(6, s.GetUnit(a).Ward);
        Assert.Equal(40, s.GetUnit(a).Hp);
    }

    // ---- conditions, events, state ----

    [Fact]
    public void AConditionCanAnswerSomethingThatHappenedToAnAlly()
    {
        var (s, a, b, c) = Three();
        var answered = new List<int>();
        s.AddReaction(new ReactionDef("guard", c, TriggerKind.WhenHit, (_, ctx) => answered.Add(ctx.Trigger.SubjectUnitId))
        {
            Condition = (st, t) => t.SubjectUnitId != c && st.Formation.PositionOf(t.SubjectUnitId).Side == st.Formation.PositionOf(c).Side,
        });
        Reactions.Resolve(s, new[] { Hit(a, b), Hit(b, a), Hit(c, a) });
        Assert.Equal(new[] { b }, answered); // only the ally of the owner, not the enemy and not the owner itself
    }

    [Fact]
    public void WhenMovedReactionsAnswerOnlyWhenMovedTriggersAreRaised()
    {
        var (s, a, b, _) = Three();
        var moved = 0;
        s.AddReaction(new ReactionDef("on_move", b, TriggerKind.WhenMoved, (_, _) => moved++));
        Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(0, moved);
        Reactions.Resolve(s, new[] { new Trigger(TriggerKind.WhenMoved, b, a) });
        Assert.Equal(1, moved);
    }

    [Fact]
    public void EachReactionIsReportedBeforeItsEffects()
    {
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 2, ownTags: EffectTags.None));
        s.DrainEvents();
        Reactions.Resolve(s, new[] { Hit(b, a) });
        var events = s.DrainEvents();
        Assert.IsType<ReactionFired>(events[0]);
        Assert.IsType<DamageDealt>(events[1]);
    }

    [Fact]
    public void UseCountsAreClonedIndependently()
    {
        var (s, a, b, _) = Three();
        s.AddReaction(Counter(b, "b_counter", 1, ownTags: EffectTags.None, uses: 1));
        var copy = s.Clone();
        Reactions.Resolve(s, new[] { Hit(b, a) });
        Assert.Equal(1, s.ReactionUses(s.Reactions[0]));
        Assert.Equal(0, copy.ReactionUses(copy.Reactions[0]));
        Assert.Equal(1, Reactions.Resolve(copy, new[] { Hit(b, a) }).Fired);
    }

    [Fact]
    public void NothingTriggeredMeansNothingFired()
    {
        var (s, _, _, _) = Three();
        Assert.Equal(new ReactionResult(0, false), Reactions.Resolve(s, Array.Empty<Trigger>()));
    }
}
