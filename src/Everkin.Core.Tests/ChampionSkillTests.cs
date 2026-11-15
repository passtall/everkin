using Everkin.Core.Battle;
using Everkin.Core.Skills;
using static Everkin.Core.Tests.K;

namespace Everkin.Core.Tests;

public class ChampionSkillTests
{
    [Fact]
    public void AllChampionSkillsAreImplemented() => AssertClassImplemented("champion", magical: false);

    [Fact]
    public void ChallengeMarksAnEnemyForThirtyAndMovesTheEarlierMark()
    {
        var (s, u, e1) = Board(1, "challenge");
        var e2 = World.Unit(s, Side.B, Row.Middle);
        Run(s, Use(u, "challenge", e1));
        Assert.Equal(30, Statuses.Get(s.GetUnit(e1), StatusIds.Marked)!.TimeLeft(s.Timeline.Now));
        Run(s, Use(u, "challenge", e2));
        Assert.False(Has(s, e1, StatusIds.Marked));
        Assert.True(Has(s, e2, StatusIds.Marked));
        Assert.Equal(0, s.Momentum.Value - 6);                               // no Momentum shift for a skill that deals no damage
    }

    [Fact]
    public void OnlyEnemiesCanBeMarked()
    {
        var (s, u, _) = Board(1, "challenge");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Assert.NotNull(Why(s, Use(u, "challenge", ally)));
    }

    [Fact]
    public void ASkillThatReadsAMarkWorksOnAnyMarkedUnitWhoeverPlacedIt()
    {
        var (s, u, e) = Board(1, "single_combat");
        s.ShiftMomentum(Side.A, 0);
        Assert.NotNull(Why(s, Use(u, "single_combat", e)));                  // not Marked yet
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: 99));
        Assert.Null(Why(s, Use(u, "single_combat", e)));
    }

    [Fact]
    public void SingleCombatLetsOnlyTheTwoDuellistsTargetEachOther()
    {
        var (s, u, e) = Board(1, "single_combat");
        var other = World.Unit(s, Side.B, Row.Front, "tail_club");
        var ally = s.Formation.Units(Side.A, Row.Middle)[0];
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: u));
        Run(s, Use(u, "single_combat", e));
        Assert.True(Has(s, u, StatusIds.Duel));
        Assert.True(Has(s, e, StatusIds.Duel));

        Assert.False(Targeting.IsLegalTarget(s, other, u));                  // no one else can target the Champion or its opponent
        Assert.False(Targeting.IsLegalTarget(s, ally, e));
        Assert.True(Targeting.IsLegalTarget(s, e, u));                       // the duellists can target each other
        Assert.True(Targeting.IsLegalTarget(s, u, e));
        Assert.True(Targeting.IsLegalTarget(s, u, other));                   // and still others
        Assert.True(Targeting.IsLegalTarget(s, u, u));
    }

    [Fact]
    public void SingleCombatEndsAfterTenOrWhenADuellistIsKo()
    {
        var (s, u, e) = Board(1, "single_combat");
        var other = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 60, SourceUnitId: u));
        Run(s, Use(u, "single_combat", e));
        s.GetUnit(e).Hp = 0;
        Knockout.ResolveAll(s);
        Assert.True(Targeting.IsLegalTarget(s, u, other) && Targeting.IsLegalTarget(s, other, u));

        var (s2, u2, e2) = Board(1, "single_combat");
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Marked, Duration: 60, SourceUnitId: u2));
        Run(s2, Use(u2, "single_combat", e2));
        AdvanceTo(s2, 11);
        Assert.False(Has(s2, u2, StatusIds.Duel));
    }

    [Fact]
    public void ExecutionOnlyTargetsAMarkedUnitBelowAQuarterAndKosItThroughAWard()
    {
        var (s, u, e) = Board(1, "execution");
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: u));
        Assert.NotNull(Why(s, Use(u, "execution", e)));                      // full HP
        s.GetUnit(e).Hp = 11;
        Assert.NotNull(Why(s, Use(u, "execution", e)));                      // 11 of 40 is above 25%
        s.GetUnit(e).Hp = 9;
        s.GetUnit(e).Ward = 20;
        Assert.Null(Why(s, Use(u, "execution", e)));
        var guard = World.Unit(s, Side.B, Row.Front);                        // a defender in front cannot stop it
        Run(s, Use(u, "execution", e));
        Assert.True(s.GetUnit(e).IsKnockedOut);
        Assert.DoesNotContain(s.DrainEvents(), x => x is Intercepted);
        Assert.False(s.GetUnit(guard).IsKnockedOut);
    }

    [Fact]
    public void ExecutionNeedsAnUnmarkedUnitToBeMarkedAndTheFrontRow()
    {
        var (s, u, e) = Board(1, "execution");
        s.GetUnit(e).Hp = 5;
        Assert.NotNull(Why(s, Use(u, "execution", e)));                      // not Marked
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: u));
        s.Formation.Remove(u);
        s.Formation.Place(u, Side.A, Row.Middle);
        Assert.Equal("needs the Front row", Why(s, Use(u, "execution", e)));
    }

    [Fact]
    public void HuntDownStunsAMarkedEnemyThatMovesWhetherByItsOwnMoveOrBeingPushed()
    {
        var (s, u, e) = Board(1, "hunt_down");
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: 99));
        Actions.Move(s, e, Row.Middle, 0);
        Assert.Equal(5, Statuses.Get(s.GetUnit(e), StatusIds.Stunned)!.TimeLeft(s.Timeline.Now));

        var (s2, u2, e2) = Board(1, "hunt_down");
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Marked, Duration: 30, SourceUnitId: u2));
        Movement.ForceMove(s2, e2, Row.Middle, Side.A);
        Assert.True(Has(s2, e2, StatusIds.Stunned));

        var (s3, _, e3) = Board(1, "hunt_down");
        Actions.Move(s3, e3, Row.Middle, 0);                                 // not Marked: nothing
        Assert.False(Has(s3, e3, StatusIds.Stunned));
    }

    [Fact]
    public void DuelistAddsFiftyPercentOnlyAgainstTheUnitTheChampionMarked()
    {
        var (s, u, e) = Board(1, "tail_club", "duelist");
        var other = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: u));
        Run(s, Use(u, "tail_club", e));
        Assert.InRange(Lost(s, e), 4, 6);                                    // 3-4 x 150%
        K.Ready(s, u);
        Run(s, Use(u, "tail_club", other));
        Assert.InRange(Lost(s, other), 3, 4);

        var (s2, u2, e2) = Board(1, "tail_club", "duelist");
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Marked, Duration: 30, SourceUnitId: 99));
        Run(s2, Use(u2, "tail_club", e2));                                   // someone else's Mark: no bonus
        Assert.InRange(Lost(s2, e2), 3, 4);
    }

    [Fact]
    public void GloryHealsAQuarterOfMaxHpWhenTheChampionKosItsMarkedUnit()
    {
        var (s, u, e) = Board(1, "tail_club", "glory");
        s.GetUnit(u).Hp = 20;
        s.GetUnit(e).Hp = 1;
        Statuses.Apply(s, new StatusApplication(e, StatusIds.Marked, Duration: 30, SourceUnitId: u));
        Run(s, Use(u, "tail_club", e));
        Assert.True(s.GetUnit(e).IsKnockedOut);
        Assert.Equal(30, s.GetUnit(u).Hp);                                   // a quarter of 40
    }

    [Fact]
    public void GloryDoesNothingForAnUnmarkedKoOrSomeoneElsesMark()
    {
        var (s, u, e) = Board(1, "tail_club", "glory");
        s.GetUnit(u).Hp = 20;
        s.GetUnit(e).Hp = 1;
        Run(s, Use(u, "tail_club", e));
        Assert.Equal(20, s.GetUnit(u).Hp);

        var (s2, u2, e2) = Board(1, "tail_club", "glory");
        s2.GetUnit(u2).Hp = 20;
        s2.GetUnit(e2).Hp = 1;
        Statuses.Apply(s2, new StatusApplication(e2, StatusIds.Marked, Duration: 30, SourceUnitId: 99));
        Run(s2, Use(u2, "tail_club", e2));
        Assert.Equal(20, s2.GetUnit(u2).Hp);
    }
}
