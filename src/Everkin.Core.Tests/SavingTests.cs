using Everkin.Core.Ai;
using Everkin.Core.Battle;
using Everkin.Core.Setup;
using Everkin.Core.Skills;

namespace Everkin.Core.Tests;

public class SavingTests : IDisposable
{
    private readonly string _dir = Path.Combine(Path.GetTempPath(), "everkin-save-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(_dir)) Directory.Delete(_dir, recursive: true);
    }

    private SaveStore Store() => new(Path.Combine(_dir, "profile.json"));

    [Fact]
    public void SavingSwapsInTheNewFileAndKeepsTheLastThreeBackups()
    {
        var store = Store();
        for (var i = 1; i <= 6; i++) store.Save($"{{\"n\":{i}}}");
        Assert.Equal("{\"n\":6}", File.ReadAllText(store.PathOf));
        Assert.Equal("{\"n\":5}", File.ReadAllText(store.PathOf + ".bak1"));
        Assert.Equal("{\"n\":4}", File.ReadAllText(store.PathOf + ".bak2"));
        Assert.Equal("{\"n\":3}", File.ReadAllText(store.PathOf + ".bak3"));
        Assert.False(File.Exists(store.PathOf + ".bak4"));
        Assert.False(File.Exists(store.PathOf + ".tmp"));
    }

    [Fact]
    public void AFileThatDoesNotParseIsNeverSwappedIn()
    {
        var store = Store();
        store.Save("{\"ok\":1}");
        Assert.ThrowsAny<Exception>(() => store.Save("{broken"));
        Assert.Equal("{\"ok\":1}", File.ReadAllText(store.PathOf));
    }

    [Fact]
    public void ADamagedSaveLoadsTheNewestGoodBackupWithANote()
    {
        var store = Store();
        store.Save("{\"n\":1}");
        store.Save("{\"n\":2}");
        store.Save("{\"n\":3}");
        File.WriteAllText(store.PathOf, "{damaged");
        Assert.Equal("{\"n\":2}", store.Load());
        Assert.NotNull(store.LoadNote);
        File.WriteAllText(store.PathOf + ".bak1", "also damaged");
        Assert.Equal("{\"n\":1}", store.Load());
    }

    [Fact]
    public void ANormalLoadHasNoNoteAndAMissingSaveGivesNull()
    {
        var store = Store();
        Assert.Null(store.Load());
        store.Save("{\"n\":1}");
        Assert.Equal("{\"n\":1}", store.Load());
        Assert.Null(store.LoadNote);
        store.Delete();
        Assert.Null(store.Load());
    }

    [Fact]
    public void TheContentHashChangesWhenTheContentDoes()
    {
        Assert.Equal(ContentHash.Of(new[] { "a", "b" }), ContentHash.Of(new[] { "a", "b" }));
        Assert.NotEqual(ContentHash.Of(new[] { "a", "b" }), ContentHash.Of(new[] { "a", "c" }));
        Assert.Equal(ContentHash.Of(new[] { "a\r\nb" }), ContentHash.Of(new[] { "a\nb" }));
    }

    [Fact]
    public void ABattleReplaysFromItsSavedActionsToTheSamePosition()
    {
        var trees = new SkillTrees(World.Content);
        var spec = BattleSetup.RandomSpec(World.Content, trees, 21);
        var state = BattleSetup.Create(spec, World.Catalog);
        var save = BattleSave.Of(spec, "harness", null, "hash");
        var level = AiSettings.Load(Path.Combine(World.ContentDir, "ai_settings.json")).Level("harness");
        var aiA = new AiSearch(World.Catalog, level, new BattleRandom(1));
        var aiB = new AiSearch(World.Catalog, level, new BattleRandom(2));
        for (var i = 0; i < 25 && !state.IsOver; i++)
        {
            var unit = BattleDriver.NextTurn(state, World.Catalog);
            state.DrainEvents();
            if (state.IsOver || unit is not { } id) break;
            var action = (state.GetUnit(id).Side == Side.A ? aiA : aiB).Choose(state, id);
            action.Apply(state, World.Catalog, id);
            state.DrainEvents();
            save.Actions.Add(SavedAction.From(id, action));
        }

        var restored = BattleSave.FromJson(save.ToJson())!.Replay(World.Catalog)!;
        Assert.NotNull(restored);
        string Snap(BattleState s) => string.Join("|", s.Units.Select(u => $"{u.Hp}:{s.Formation.Contains(u.Id)}")) + "#" + s.Timeline.Now + "#" + s.Momentum.Value + "#" + s.Random.Clone().NextUInt64();
        Assert.Equal(Snap(state), Snap(restored));
    }

    [Fact]
    public void AnActionThatNoLongerFitsDropsTheSave()
    {
        var spec = BattleSetup.RandomSpec(World.Content, new SkillTrees(World.Content), 21);
        var save = BattleSave.Of(spec, "harness", null, "hash");
        save.Actions.Add(new SavedAction("skill", 0, "no_such_skill", 1));
        Assert.Null(save.Replay(World.Catalog));
    }
}
