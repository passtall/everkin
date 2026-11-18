using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Bard (skills.md): songs for allies, a lullaby for enemies. The three songs are magical.</summary>
public static class BardSkills
{
    public const int SongLength = 20;
    public const int HastePull = 3;
    public const int LullabyPush = 5;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new SongOfHaste(D("song_of_haste"));
        yield return new WarDrums(D("war_drums"));
        yield return new Lullaby(D("lullaby"));
        yield return new GrandChorus(D("grand_chorus"));
        yield return new Rhythm(D("rhythm"));
        yield return new BardEcho(D("echo"));
    }

    /// <summary>
    /// A song with a status that lasts 20: the units it reaches are fixed when it is cast (a unit that leaves keeps it, a unit that arrives
    /// does not get it). With Grand Chorus it reaches every ally, but only one of the Bard's songs can be active at a time: casting one ends the other.
    /// </summary>
    internal static void Sing(SkillContext context, string statusId, Func<BattleState, int, IEnumerable<int>> defaultReach, int magnitude = 0)
    {
        var state = context.State;
        var chorus = context.User.KnownSkills.Contains("grand_chorus");
        if (chorus)
            foreach (var unit in state.Units.ToArray())
                foreach (var song in new[] { StatusIds.SongHaste, StatusIds.WarDrums })
                    if (song != statusId && Statuses.Get(unit, song)?.SourceUnitId == context.UserId) Statuses.Remove(state, unit.Id, song, StatusEndCause.Expired);
        var length = SongLength;
        var reached = chorus
            ? state.Formation.Units(context.UserSide).Where(id => !state.GetUnit(id).IsKnockedOut).ToList()
            : defaultReach(state, context.UserId).Where(id => !state.GetUnit(id).IsKnockedOut).ToList();
        foreach (var id in reached)
            Statuses.Apply(state, new StatusApplication(id, statusId, length, magnitude, context.UserId));
        if (context.User.KnownSkills.Contains("echo")) Echo(context, statusId);
        if (statusId == StatusIds.SongHaste)
            foreach (var id in reached)
                if (state.Timeline.TurnOf(id) is { } turn) state.Timeline.PullForward(turn, HastePull);
        context.Settle();
    }

    /// <summary>Echo: the Bard's other lasting song, if it is still running, is renewed on the units that still hold it (time back to 20, no new pull).</summary>
    private static void Echo(SkillContext context, string castId)
    {
        var state = context.State;
        foreach (var song in new[] { StatusIds.SongHaste, StatusIds.WarDrums })
        {
            if (song == castId) continue;
            foreach (var unit in state.Units.ToArray())
                if (!unit.IsKnockedOut && Statuses.Get(unit, song) is { } held && held.SourceUnitId == context.UserId)
                    Statuses.Apply(state, new StatusApplication(unit.Id, song, SongLength, held.Magnitude, context.UserId));
        }
    }
}

/// <summary>Song of Haste: song. Allies in the Bard's row (the Bard included) have +1 Speed for 20.</summary>
public sealed class SongOfHaste : ClassSkill
{
    public SongOfHaste(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context) =>
        BardSkills.Sing(context, StatusIds.SongHaste, (st, bard) => st.Formation.Units(st.Formation.PositionOf(bard).Side, st.Formation.PositionOf(bard).Row), magnitude: 1);
}

/// <summary>War Drums: song. For 20, the Bard and its adjacent allies cannot be Stunned, Frozen, Confused or Immobilized.</summary>
public sealed class WarDrums : ClassSkill
{
    public WarDrums(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("war_drums")) return;
        state.AddStatusImmunity((st, unitId, statusId) => statusId is StatusIds.Stunned or StatusIds.Frozen or StatusIds.Confused or StatusIds.Immobilized && Statuses.Has(st.GetUnit(unitId), StatusIds.WarDrums));
    }

    public override void Resolve(SkillContext context) =>
        BardSkills.Sing(context, StatusIds.WarDrums, (st, bard) => st.Formation.Adjacent(bard).Where(id => st.GetUnit(id).Side == st.GetUnit(bard).Side).Prepend(bard));
}

/// <summary>Lullaby: song. Every enemy in one row has its next turn pushed back 5. It is instant, so the one-song limit of Grand Chorus do not touch it.</summary>
public sealed class Lullaby : ClassSkill
{
    public Lullaby(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) =>
        ClassHelpers.AllRows().Where(r => r.Side != state.GetUnit(userId).Side).ToArray();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).ToArray())
            if (!context.State.GetUnit(id).IsKnockedOut) context.State.PushTurnBack(id, BardSkills.LullabyPush);
        context.Settle();
    }
}

/// <summary>Grand Chorus: the Bard's songs reach every ally, but only one of its songs can be active at a time. The songs read this keystone.</summary>
public sealed class GrandChorus : KeystoneSkill
{
    public GrandChorus(SkillDef def) : base(def) { }
}

/// <summary>Rhythm: when an ally uses the same skill twice in a row, the second use has 1 less Delay. A Move or Skip Turn between the two uses breaks the streak.</summary>
public sealed class Rhythm : KeystoneSkill
{
    public Rhythm(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        if (!state.FirstInstall("rhythm")) return;
        state.AddRepeatDelayReduction((st, userId, skillId) =>
        {
            var side = st.GetUnit(userId).Side;
            return st.Units.Any(u => !u.IsKnockedOut && !u.IsRemoved && u.Side == side && u.KnownSkills.Contains("rhythm")) ? 1 : 0;
        });
    }
}

/// <summary>Echo: when the Bard casts a song, its other lasting song that is still running is renewed. <see cref="BardSkills.Sing"/> reads this keystone.</summary>
public sealed class BardEcho : KeystoneSkill
{
    public BardEcho(SkillDef def) : base(def) { }
}
