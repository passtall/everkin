using System.Text.RegularExpressions;
using Everkin.Core.Content;

namespace Everkin.Core.Tests;

public class SkillTooltipTests
{
    private static readonly ContentSet Content = World.Content;

    [Fact]
    public void EverySkillKeystoneAndAttackHasATooltipWithoutBraces()
    {
        var ids = Content.AttacksById.Keys.Concat(Content.SkillsById.Keys).ToList();
        Assert.Equal(319 + 168, ids.Count);
        foreach (var id in ids)
        {
            var tip = SkillTooltips.For(Content, id, 5);
            Assert.NotNull(tip);
            Assert.False(string.IsNullOrWhiteSpace(tip!.Name), id);
            Assert.False(string.IsNullOrWhiteSpace(tip.Effect), id);
            Assert.DoesNotContain("{", tip.Effect);
            Assert.False(tip.Kind.Contains("Keystone") && tip.Delay != null, id);
        }
        Assert.NotNull(SkillTooltips.Move.Effect);
        Assert.NotNull(SkillTooltips.SkipTurn.Effect);
    }

    [Fact]
    public void AtPowerFiveTheTooltipIsTheDocumentText()
    {
        foreach (var s in Content.SkillsById.Values) Assert.Equal(s.Effect, SkillTooltips.For(Content, s.Id, 5)!.Effect);
        foreach (var a in Content.AttacksById.Values) Assert.Equal(a.Effect, SkillTooltips.For(Content, a.Id, 5)!.Effect);
    }

    [Fact]
    public void EveryUniqueAttackShowsItsDamageAndSplashDamage()
    {
        foreach (var a in Content.AttacksById.Values)
        {
            Assert.Contains($"{a.Min}–{a.Max}", a.Effect);
            if (a.Shape == "splash") Assert.Contains($"{a.SplashMin}–{a.SplashMax}", a.Effect);
        }
    }

    [Fact]
    public void NumbersScaleWithPowerRoundedDownMinimumOne()
    {
        Assert.Equal("Hit for 16–20.", SkillTooltips.ScaleTemplate("Hit for {8–10}.", 10));
        Assert.Equal("A Ward of 1, for 10.", SkillTooltips.ScaleTemplate("A Ward of {4}, for 10.", 2));   // 4 x 2 / 5 = 1
        Assert.Equal("Hit for 1–1.", SkillTooltips.ScaleTemplate("Hit for {1–2}.", 3));
        Assert.Equal("Hit for 7–8.", SkillTooltips.ScaleTemplate("Hit for {5–6}.", 7));
        var mend = SkillTooltips.For(Content, "mend", 10)!;
        Assert.Equal("Heal a unit for 16–20.", mend.Effect);
    }

    [Fact]
    public void ARangeMarkedInTheDocsIsAlwaysWellFormed()
    {
        foreach (var s in Content.SkillsById.Values.Where(s => s.Template != null))
            Assert.Equal(0, Regex.Matches(Regex.Replace(s.Template!, @"\{\d+(–\d+)?\}", ""), @"[{}]").Count);
    }
}
