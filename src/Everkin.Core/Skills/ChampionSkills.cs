using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Champion (skills.md): marks one enemy and duels it. Physical.</summary>
public static class ChampionSkills
{
    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Challenge(D("challenge"));
        yield return new SingleCombat(D("single_combat"));
        yield return new Execution(D("execution"));
        yield return new HuntDown(D("hunt_down"));
        yield return new Duelist(D("duelist"));
        yield return new Glory(D("glory"));
    }
}

/// <summary>Challenge: Mark a unit. A Champion has one Mark from Challenge at a time; marking another moves it.</summary>
public sealed class Challenge : ClassSkill
{
    public Challenge(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);       // Marked is an enemy-only status

    public override void Resolve(SkillContext context)
    {
        Marks.Place(context.State, context.UserId, context.TargetId!.Value);
        context.Settle();
    }
}

/// <summary>Single Combat: for 10 time units the Champion and one Marked unit can be targeted only by each other.</summary>
public sealed class SingleCombat : ClassSkill
{
    public const int Length = 10;
    public SingleCombat(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);
    public override bool TargetFilter(BattleState state, int userId, int targetId) => Marks.IsMarkedEnemy(state, userId, targetId);

    public override void Install(BattleState state, int ownerId) =>
        state.AddTargetRestriction((st, userId, targetId) =>
        {
            if (userId == targetId || Statuses.Get(st.GetUnit(targetId), StatusIds.Duel) is not { } duel) return true;
            var partner = st.GetUnit(duel.Magnitude);
            return partner.IsKnockedOut || userId == partner.Id;                      // the duel is over when a duellist is KO
        });

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var enemy = context.TargetId!.Value;
        foreach (var id in new[] { context.UserId, enemy })                           // an old duel ends for its other duellist
            if (Statuses.Get(state.GetUnit(id), StatusIds.Duel) is { } old && old.Magnitude != context.UserId && old.Magnitude != enemy
                && !state.GetUnit(old.Magnitude).IsKnockedOut)
                Statuses.Remove(state, old.Magnitude, StatusIds.Duel, StatusEndCause.Consumed);
        Statuses.Apply(state, new StatusApplication(context.UserId, StatusIds.Duel, Length, enemy, context.UserId));
        Statuses.Apply(state, new StatusApplication(enemy, StatusIds.Duel, Length, context.UserId, context.UserId));
        context.Settle();
    }
}

/// <summary>Execution: a Marked unit below 25% HP is KO. It cannot be intercepted and ignores Wards: the unit's HP goes to 0.</summary>
public sealed class Execution : ClassSkill
{
    public Execution(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        Marks.IsMarkedEnemy(state, userId, targetId) && BerserkerSkills.BelowQuarter(state.GetUnit(targetId));

    public override void Resolve(SkillContext context) => context.Execute(context.TargetId!.Value);
}

/// <summary>Hunt Down: when a Marked enemy moves or is moved, the Champion Stuns it for 5. Its own Move counts, whoever placed the Mark.</summary>
public sealed class HuntDown : KeystoneSkill
{
    public const int StunLength = 5;
    public HuntDown(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAfterMoveHook((st, movedId) =>
        {
            if (st.GetUnit(ownerId).IsKnockedOut || !Marks.IsMarkedEnemy(st, ownerId, movedId) || st.GetUnit(movedId).IsKnockedOut) return;
            Statuses.Apply(st, new StatusApplication(movedId, StatusIds.Stunned, StunLength, SourceUnitId: ownerId));
        });
}

/// <summary>Duelist: the Champion deals 50% more damage to the unit it has Marked.</summary>
public sealed class Duelist : KeystoneSkill
{
    public Duelist(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddDamageBonus((st, attacker, target) => attacker == ownerId && Marks.IsMarkedBy(st, ownerId, target) ? 50 : 0);
}

/// <summary>Glory: when the Champion KOs its Marked unit, it heals a quarter of its max HP.</summary>
public sealed class Glory : KeystoneSkill
{
    public Glory(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddReaction(new ReactionDef(Id, ownerId, TriggerKind.WhenKo, (st, ctx) =>
        {
            var owner = st.GetUnit(ownerId);
            if (!owner.IsKnockedOut) st.Heal(ownerId, ownerId, owner.MaxHp / 4);
        })
        {
            Condition = (st, t) => t.SourceUnitId == ownerId && st.GetUnit(t.SubjectUnitId).MarkedByAtKo == ownerId,
        });
}
