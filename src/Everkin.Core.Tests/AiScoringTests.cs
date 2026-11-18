using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Setup;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

/// <summary>M5.9: the scoring parts and the candidate rule of game_system section 2.5.</summary>
public class AiScoringTests
{
    private static AiSettings Settings => AiSettings.Load(Path.Combine(World.ContentDir, "ai_settings.json"));

    [Fact]
    public void TheHarnessPlacesUnitsByRole()
    {
        var content = World.Content;
        for (ulong seed = 1; seed <= 30; seed++)
        {
            var spec = BattleSetup.RandomSpec(content, seed);
            foreach (var u in spec.SideA.Units.Concat(spec.SideB.Units))
            {
                var def = content.CreaturesById[u.CreatureId];
                var delivery = content.AttacksById[def.Attack].Delivery;
                if (def.Classes.Contains("bulwark") || def.Hp >= 60) Assert.Equal(Row.Front, u.Row);
                else if (delivery == "melee") Assert.Equal(Row.Front, u.Row);
                else if (delivery == "projectile") Assert.Equal(Row.Rear, u.Row);
                else Assert.Contains(u.Row, new[] { Row.Middle, Row.Rear });
            }
        }
    }

    [Fact]
    public void MomentumIsWorthLessTheFurtherAhead()
    {
        var level = Settings.Level("harness");
        var p = level.MomentumPoint;
        Assert.Equal(3 * p, Evaluator.MomentumValue(3, level));
        Assert.Equal(3 * p + 6 * p / 2, Evaluator.MomentumValue(9, level));
        Assert.Equal(Evaluator.MomentumValue(9, level), Evaluator.MomentumValue(10, level));       // the cap adds nothing
        Assert.Equal(-Evaluator.MomentumValue(4, level), Evaluator.MomentumValue(-4, level));
    }

    [Fact]
    public void ANextTurnThatComesLaterCountsAgainstItsSide()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front);
        var b = World.Unit(s, Side.B, Row.Front);
        var level = Settings.Level("harness");
        s.Timeline.ScheduleTurn(a, 5, 5);
        s.Timeline.ScheduleTurn(b, 5, 5);
        Assert.Equal(0, Evaluator.Score(s, level));
        s.Timeline.PushBack(s.Timeline.TurnOf(b)!, 3);
        Assert.Equal(3 * level.TempoPerTime, Evaluator.Score(s, level));                      // Power 5: each time unit is worth TempoPerTime
        s.Timeline.PushBack(s.Timeline.TurnOf(b)!, 100);
        Assert.Equal(11 * level.TempoPerTime, Evaluator.Score(s, level));                     // at most one normal Wait (5 + 11 - 5)
    }

    [Fact]
    public void TempoScalesWithPower()
    {
        var level = Settings.Level("harness");
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(a).Power = 10;
        s.Timeline.ScheduleTurn(a, 2, 5);                                                      // 3 time units sooner than a normal Wait of 11
        Assert.Equal(3 * level.TempoPerTime * 2, Evaluator.Score(s, level) - 0);
    }

    [Fact]
    public void EveryStatusHasItsOwnWeight()
    {
        var level = Settings.Level("harness");
        foreach (var id in new[] { "blind", "poisoned", "bleeding", "silenced", "frozen", "stunned", "marked", "ward", "song_haste", "echo" })
            Assert.True(level.StatusWeights.ContainsKey(id), id);
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Front);
        var enemy = World.Unit(s, Side.B, Row.Front);
        Statuses.Apply(s, new StatusApplication(enemy, StatusIds.Blind, Duration: 20));
        var plain = Evaluator.Score(s, level);
        var heavy = new AiLevel { StatusWeights = new() { ["blind"] = 120 }, MomentumPoint = level.MomentumPoint };
        Assert.True(Evaluator.Score(s, heavy) > plain);
    }

    [Fact]
    public void SilencedCountsOnlyOnAUnitWithAMagicalSkillAndBleedingOnlyWithAMeleeSkill()
    {
        var level = Settings.Level("harness");
        var s = new BattleState(1);
        World.Unit(s, Side.A, Row.Front);
        var healer = World.Unit(s, Side.B, Row.Front, "mend");
        var none = Evaluator.Score(s, level, new EvalContext(World.Catalog, s));
        Statuses.Apply(s, new StatusApplication(healer, StatusIds.Silenced, Duration: 20));
        Assert.True(Evaluator.Score(s, level, new EvalContext(World.Catalog, s)) > none);     // Mend is magical

        var s2 = new BattleState(1);
        World.Unit(s2, Side.A, Row.Front);
        var plain = World.Unit(s2, Side.B, Row.Front);
        var none2 = Evaluator.Score(s2, level, new EvalContext(World.Catalog, s2));
        Statuses.Apply(s2, new StatusApplication(plain, StatusIds.Silenced, Duration: 20));
        Statuses.Apply(s2, new StatusApplication(plain, StatusIds.Bleeding, Duration: 20, Uses: 3));
        Assert.Equal(none2, Evaluator.Score(s2, level, new EvalContext(World.Catalog, s2)));  // no magical and no melee skill: nothing to stop
    }

    [Fact]
    public void ASummonCountsByItsDamageAndEachFurtherSummonOfACasterCountsLess()
    {
        var level = Settings.Level("harness");
        var s = new BattleState(1);
        var caster = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        var ctx = new EvalContext(World.Catalog, s);
        var before = Evaluator.Score(s, level, ctx);
        Summons.Summon(s, new SummonSpec(Side.A, "Clay Golem", UnitKind.Summon, 20, 5, caster, TakesTurns: true), Row.Front, 1);
        s.GetUnit(s.Units.Count - 1).AutoSkillId = "clay_golem_strike";
        var one = Evaluator.Score(s, level, ctx);
        Summons.Summon(s, new SummonSpec(Side.A, "Clay Golem", UnitKind.Summon, 20, 5, caster, TakesTurns: true), Row.Front, 2);
        s.GetUnit(s.Units.Count - 1).AutoSkillId = "clay_golem_strike";
        var two = Evaluator.Score(s, level, ctx);
        Assert.True(one > before);
        Assert.True(two > one);
        Assert.True(two - one < one - before);                                                 // the second counts less
    }

    [Fact]
    public void AHealingTotemCountsWhatItWillHealAndABarricadeCountsNothing()
    {
        var level = Settings.Level("harness");
        var s = new BattleState(1);
        var caster = World.Unit(s, Side.A, Row.Front);
        World.Unit(s, Side.B, Row.Front);
        s.GetUnit(caster).Hp = 20;
        var before = Evaluator.Score(s, level, new EvalContext(World.Catalog, s));
        Summons.Summon(s, new SummonSpec(Side.A, "Wall", UnitKind.Construct, 10, 1, caster, TakesTurns: false, DefinitionId: "barricade"), Row.Front, 1);
        Assert.Equal(before, Evaluator.Score(s, level, new EvalContext(World.Catalog, s)));
        Summons.Summon(s, new SummonSpec(Side.A, "Healing Totem", UnitKind.Construct, 8, 1, caster, TakesTurns: false, DefinitionId: TotemistSkills.HealingTotemId), Row.Front, 2);
        Assert.True(Evaluator.Score(s, level, new EvalContext(World.Catalog, s)) > before);
    }

    [Fact]
    public void TheSearchAlwaysLooksFurtherAtACostedSkill()
    {
        var s = new BattleState(1);
        var a = World.Unit(s, Side.A, Row.Front, "rain_of_mending", "mend");
        for (var i = 0; i < 3; i++) World.Unit(s, Side.B, Row.Front);
        var hurt = World.Unit(s, Side.A, Row.Middle);
        s.GetUnit(hurt).Hp = 10;
        s.ShiftMomentum(Side.A, 5);
        s.Timeline.ScheduleTurn(a, 1, 5);
        var narrow = new AiLevel { Depth = 2, Candidates = 1, RootCandidates = 1, PickTop = 1, AlwaysSearchCostedAndControl = true };
        var ai = new AiSearch(World.Catalog, narrow, new BattleRandom(1));
        ai.Choose(s, a);
        Assert.Contains("rain_of_mending", ai.LastLegalSkills);
        Assert.Contains("rain_of_mending", ai.LastSearchedSkills);                             // costed: kept although not among the best one
        var off = new AiSearch(World.Catalog, new AiLevel { Depth = 2, Candidates = 1, RootCandidates = 1, PickTop = 1, AlwaysSearchCostedAndControl = false }, new BattleRandom(1));
        off.Choose(s, a);
        Assert.True(off.LastSearchedSkills.Count <= 1);
    }
}
