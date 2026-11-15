using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Skyborne (skills.md): flight, wind and dives. Physical.</summary>
public static class SkyborneSkills
{
    public const int FlightLength = 40;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new TakeOff(D("take_off"));
        yield return new Gust(D("gust"));
        yield return new Dive(D("dive"));
        yield return new SkyLord(D("sky_lord"));
        yield return new EagleEye(D("eagle_eye"));
        yield return new Lift(D("lift"));
    }
}

/// <summary>Take Off: the Skyborne is airborne for 40. With Lift it also makes one ally next to it airborne for the same time. A unit under Curse of Weight cannot use it.</summary>
public sealed class TakeOff : ClassSkill
{
    public TakeOff(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;

    public override string? Unavailable(BattleState state, int userId) =>
        Statuses.Has(state.GetUnit(userId), StatusIds.Weighted) ? "Curse of Weight: cannot become airborne" : null;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        Statuses.Apply(state, new StatusApplication(context.UserId, StatusIds.Airborne, SkyborneSkills.FlightLength, SourceUnitId: context.UserId));
        if (context.User.KnownSkills.Contains("lift"))
        {
            var allies = state.Formation.Adjacent(context.UserId)
                .Where(id => state.GetUnit(id).Side == context.UserSide && !state.GetUnit(id).IsKnockedOut).ToList();
            if (allies.Count > 0)
                Statuses.Apply(state, new StatusApplication(ClassHelpers.Pick(state, allies), StatusIds.Airborne, SkyborneSkills.FlightLength, SourceUnitId: context.UserId));
        }
        context.Settle();
    }
}

/// <summary>Gust: wind. Push a unit one row back (bumps apply). A Rear unit stays.</summary>
public sealed class Gust : ClassSkill
{
    public Gust(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var target = context.TargetId!.Value;
        var row = context.State.Formation.PositionOf(target).Row;
        if (row == Row.Rear) { context.Settle(); return; }
        context.AfterForcedMove(Movement.ForceMove(context.State, target, (Row)((int)row + 1), context.UserSide));
    }
}

/// <summary>Dive: only while airborne, hit for 9-11, then land. It works from any row and cannot be intercepted; it cannot hit an airborne unit.</summary>
public sealed class Dive : ClassSkill
{
    public Dive(SkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;
    public override bool NeedsFrontRow => false;
    public override TargetOptions Options => new(AnyRow: true);

    public override string? Unavailable(BattleState state, int userId) => state.GetUnit(userId).IsAirborne ? null : "only while airborne";
    public override bool TargetFilter(BattleState state, int userId, int targetId) => !state.GetUnit(targetId).IsAirborne;

    public override void Resolve(SkillContext context)
    {
        context.HitBlock(new[] { new SkillContext.HitSpec(context.TargetId!.Value, new IntRange(9, 11), NoInterception: true) });
        if (context.Over || context.User.IsKnockedOut) return;
        Statuses.Remove(context.State, context.UserId, StatusIds.Airborne, StatusEndCause.Consumed);      // then land (a Sky Lord rises again at once)
        context.Settle();
    }
}

/// <summary>Sky Lord: the Skyborne is always airborne, but has 30% less HP (rounded down, minimum 1). If something makes it land, it takes to the air again at once unless Curse of Weight forbids it.</summary>
public sealed class SkyLord : KeystoneSkill
{
    public SkyLord(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId)
    {
        var unit = state.GetUnit(ownerId);
        unit.MaxHp = Math.Max(1, unit.MaxHp * 70 / 100);
        unit.Hp = Math.Min(unit.Hp, unit.MaxHp);
        Statuses.Apply(state, new StatusApplication(ownerId, StatusIds.Airborne, SourceUnitId: ownerId));
        state.AddAuraSync("sky_lord", st =>
        {
            foreach (var u in st.Units)
                if (u.KnownSkills.Contains("sky_lord") && !u.IsKnockedOut && !u.IsRemoved && !u.IsAirborne && st.Formation.Contains(u.Id))
                    Statuses.Apply(st, new StatusApplication(u.Id, StatusIds.Airborne, SourceUnitId: u.Id));      // refused under Curse of Weight
        });
    }
}

/// <summary>Eagle Eye: while the Skyborne is airborne, every enemy loses its stealth, and an enemy that gains stealth loses it at once.</summary>
public sealed class EagleEye : KeystoneSkill
{
    public EagleEye(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAuraSync("eagle_eye", st =>
        {
            foreach (var eye in st.Units.Where(u => u.KnownSkills.Contains("eagle_eye") && u.IsAirborne && !u.IsKnockedOut && !u.IsRemoved).ToList())
                foreach (var enemy in st.Units.Where(u => u.Side != eye.Side && u.IsStealthed).ToList())
                    Stealth.Break(st, enemy.Id, StealthCause.Revealed);
        });
}

/// <summary>Lift: Take Off also makes one ally next to the Skyborne airborne for the same time. Take Off reads this keystone.</summary>
public sealed class Lift : KeystoneSkill
{
    public Lift(SkillDef def) : base(def) { }
}
