using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Tracker (skills.md): marks an enemy, finds stealthed units, shoots the marked. Physical.</summary>
public static class TrackerSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new TargetSkill(D("target"));
        yield return new ReadTracks(D("read_tracks"));
        yield return new HuntersShot(D("hunters_shot"));
        yield return new SniffTheAir(D("sniff_the_air"));
        yield return new ScentOfBlood(D("scent_of_blood"));
        yield return new Relentless(D("relentless"));
    }
}

/// <summary>Target: Mark a unit. A Tracker has one Mark at a time; marking another moves it.</summary>
public sealed class TargetSkill : ClassSkill
{
    public TargetSkill(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);       // only enemies can be Marked

    public override void Resolve(SkillContext context)
    {
        Marks.Place(context.State, context.UserId, context.TargetId!.Value);
        context.Settle();
    }
}

/// <summary>Read Tracks: pick a row; every stealthed unit in it is revealed.</summary>
public sealed class ReadTracks : ClassSkill
{
    public ReadTracks(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => ClassHelpers.AllRows();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).ToArray())
            Stealth.Break(context.State, id, StealthCause.Revealed);
        context.Settle();
    }
}

/// <summary>Hunter's Shot: hit a Marked unit for 9-11.</summary>
public sealed class HuntersShot : ClassSkill
{
    public HuntersShot(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);
    public override bool TargetFilter(BattleState state, int userId, int targetId) => Marks.IsMarkedEnemy(state, userId, targetId);

    public override void Resolve(SkillContext context) => context.Hit(context.TargetId!.Value, new IntRange(9, 11));
}

/// <summary>Sniff the Air: at battle start, the enemy with the highest Power becomes Marked (ties by the seeded roll).</summary>
public sealed class SniffTheAir : KeystoneSkill
{
    public SniffTheAir(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        var side = state.GetUnit(ownerId).Side;
        var enemies = state.Units.Where(u => u.Side != side && !u.IsKnockedOut && !u.IsRemoved && state.Formation.Contains(u.Id)).ToList();
        if (enemies.Count == 0) return;
        var best = enemies.Max(u => u.EffectivePower);
        var tied = enemies.Where(u => u.EffectivePower == best).Select(u => u.Id).ToList();
        Marks.Place(state, ownerId, ClassHelpers.Pick(state, tied));
    }
}

/// <summary>Scent of Blood: when an enemy's HP falls below 20%, it becomes Marked.</summary>
public sealed class ScentOfBlood : KeystoneSkill
{
    public ScentOfBlood(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAfterDamageHook((st, targetId, before, after) =>
        {
            var owner = st.GetUnit(ownerId);
            var target = st.GetUnit(targetId);
            if (owner.IsKnockedOut || target.Side == owner.Side || target.IsKnockedOut || !st.Formation.Contains(targetId)) return;
            if (before * 5 >= target.MaxHp && after * 5 < target.MaxHp) Marks.Place(st, ownerId, targetId);
        });
}

/// <summary>Relentless: the Tracker's Marks have no time limit: they last until the target is KO and cannot be removed (only moved by the Tracker's own next Mark).</summary>
public sealed class Relentless : KeystoneSkill
{
    public Relentless(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStatusRemovalVeto((st, unitId, status, by) => status.Id == StatusIds.Marked && status.SourceUnitId == ownerId);
}
