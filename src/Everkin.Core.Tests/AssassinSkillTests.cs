using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class AssassinSkillTests
{
    [Fact]
    public void AllAssassinSkillsAreImplemented() => AssertClassImplemented("assassin", magical: false);

    [Fact]
    public void VanishGivesStealthAndRemovesEveryHarmfulStatus()
    {
        var (s, a, e) = Board(1, "vanish");
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Blind, Duration: 20));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Poison, Duration: 20));
        Statuses.Apply(s, new StatusApplication(a, StatusIds.Ward, Magnitude: 3));
        Run(s, Use(a, "vanish"));
        Assert.True(s.GetUnit(a).IsStealthed);
        Assert.False(Has(s, a, StatusIds.Blind));
        Assert.False(Has(s, a, StatusIds.Poison));
        Assert.Equal(3, s.GetUnit(a).Ward);
        Assert.Equal(5, s.Momentum.Value);                                                  // cost 1 from +6
    }

    [Fact]
    public void VanishWorksForASidesLastVisibleUnitAndTheEnemyCanStillHitItWithAreas()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "vanish");
        var e = World.Unit(s, Side.B, Row.Front, "sour_chord");
        s.ShiftMomentum(Side.A, 6);
        Run(s, Use(a, "vanish"));
        Assert.True(s.GetUnit(a).IsStealthed);
        Assert.Empty(Actions.LegalTargets(s, World.Catalog, e, "sour_chord").Where(t => t == a));
        Run(s, Use(e, "sour_chord"));                                                       // the pulse hits the stealthed unit and breaks its stealth
        Assert.InRange(Lost(s, a), 1, 2);
        Assert.False(s.GetUnit(a).IsStealthed);
    }

    [Fact]
    public void VanishCannotBeUsedWhileAlreadyStealthed()
    {
        var (s, a, _) = Board(1, "vanish");
        s.GetUnit(a).IsStealthed = true;
        Assert.NotNull(Why(s, Use(a, "vanish")));
    }

    [Fact]
    public void FeintAndBackstabOnlyWorkFromStealth()
    {
        var (s, a, e) = Board(1, "feint", "backstab");
        Assert.Equal("only from stealth", Why(s, Use(a, "feint", e)));
        Assert.Equal("only from stealth", Why(s, Use(a, "backstab", e)));
        s.GetUnit(a).IsStealthed = true;
        Assert.Null(Why(s, Use(a, "feint", e)));
        Assert.Null(Why(s, Use(a, "backstab", e)));
    }

    [Fact]
    public void FeintConfusesAUnitForTenAndTheAssassinStaysStealthed()
    {
        var (s, a, e) = Board(1, "feint");
        s.GetUnit(a).IsStealthed = true;
        Run(s, Use(a, "feint", e));
        Assert.Equal(10, Statuses.Get(s.GetUnit(e), StatusIds.Confused)!.TimeLeft(s.Timeline.Now));
        Assert.True(s.GetUnit(a).IsStealthed);
        Assert.Equal(0, Lost(s, e));
    }

    [Fact]
    public void BackstabHitsForEightToTenMakesTheTargetBleedAndBreaksStealth()
    {
        var damages = new HashSet<int>();
        for (ulong seed = 1; seed <= 80; seed++)
        {
            var (s, a, e) = Board(seed, "backstab");
            s.GetUnit(a).IsStealthed = true;
            Run(s, Use(a, "backstab", e));
            damages.Add(Lost(s, e));
            Assert.Equal(3, Statuses.Get(s.GetUnit(e), StatusIds.Bleeding)!.UsesLeft);
            Assert.False(s.GetUnit(a).IsStealthed);
        }
        Assert.Equal(new[] { 8, 9, 10 }, damages.OrderBy(x => x));
    }

    [Fact]
    public void UnseenStartsTheBattleStealthed()
    {
        var (s, a, _) = Board(1, "unseen");
        Assert.True(s.GetUnit(a).IsStealthed);
    }

    [Fact]
    public void VanishingKillGivesStealthBackWhenTheAssassinKosAUnit()
    {
        var (s, a, e) = Board(1, "tail_club", "vanishing_kill");
        s.GetUnit(e).Hp = 1;
        Run(s, Use(a, "tail_club", e));
        Assert.True(s.GetUnit(e).IsKnockedOut);
        Assert.True(s.GetUnit(a).IsStealthed);
    }

    [Fact]
    public void VanishingKillDoesNotTriggerWhenSomeoneElseMakesTheKill()
    {
        var (s, a, e) = Board(1, "vanishing_kill");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        s.GetUnit(e).Hp = 1;
        s.DealDamage(ally, e, 5);
        Knockout.ResolveAll(s);
        Assert.False(s.GetUnit(a).IsStealthed);
    }

    [Fact]
    public void VenomBladesPoisonsTheTargetOfEveryHitForTwenty()
    {
        var (s, a, e) = Board(1, "tail_club", "venom_blades");
        Run(s, Use(a, "tail_club", e));
        Assert.Equal(20, Statuses.Get(s.GetUnit(e), StatusIds.Poison)!.TimeLeft(s.Timeline.Now));
        Assert.Equal(a, Statuses.Get(s.GetUnit(e), StatusIds.Poison)!.SourceUnitId);
    }

    [Fact]
    public void VenomBladesPoisonsAnAreasTargetsButNotAnInterceptor()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "sour_chord", "venom_blades");
        var e1 = World.Unit(s, Side.B, Row.Front);
        var e2 = World.Unit(s, Side.B, Row.Front);
        BattleStart.Install(s, World.Catalog);
        Run(s, Use(a, "sour_chord"));
        Assert.True(Has(s, e1, StatusIds.Poison));
        Assert.True(Has(s, e2, StatusIds.Poison));
    }
}
