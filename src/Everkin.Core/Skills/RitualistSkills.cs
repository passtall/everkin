using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Ritualist (skills.md): water and ice on the ground. Magical.</summary>
public static class RitualistSkills
{
    public const int RainLength = 40;
    public const int IceLength = 20;
    public const int MistLength = 20;
    public const int RainHealing = 4;
    public const int FrostMarker = 77;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new RainDance(D("rain_dance"));
        yield return new Hoarfrost(D("hoarfrost"));
        yield return new RiteOfMists(D("rite_of_mists"));
        yield return new Deluge(D("deluge"));
        yield return new Tidecaller(D("tidecaller"));
        yield return new Frostbinder(D("frostbinder"));
    }

    /// <summary>Whether a unit stands on water (an airborne unit ignores ground effects).</summary>
    internal static bool OnWater(BattleState state, int unitId) =>
        state.Formation.Contains(unitId) && state.Ground.Has(state.Ground.SpotOf(unitId), GroundKind.Water) && state.Ground.Affected(state.Ground.SpotOf(unitId)).Contains(unitId);
}

/// <summary>Rain Dance: water on every spot of one row for 40; allies standing in it heal 4 (once, when it is cast).</summary>
public sealed class RainDance : ClassSkill
{
    public RainDance(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => ClassHelpers.AllRows();

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var row = context.Row!.Value;
        var units = state.Formation.Units(row.Side, row.Row).ToArray();
        foreach (var id in units) state.Ground.Place(GroundKind.Water, state.Ground.SpotOf(id), RitualistSkills.RainLength, context.UserId);
        var heals = units.Where(id => state.GetUnit(id).Side == context.UserSide && !state.GetUnit(id).IsKnockedOut && RitualistSkills.OnWater(state, id))
            .Select(id => new SkillContext.HitSpec(id, new IntRange(RitualistSkills.RainHealing, RitualistSkills.RainHealing))).ToArray();
        context.HealBlock(heals);
    }
}

/// <summary>Hoarfrost: ice on a unit's spot for 20.</summary>
public sealed class Hoarfrost : ClassSkill
{
    public Hoarfrost(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        context.State.Ground.Place(GroundKind.Ice, context.State.Ground.SpotOf(context.TargetId!.Value), RitualistSkills.IceLength, context.UserId);
        context.Settle();
    }
}

/// <summary>Rite of Mists: for 20, projectile hits from units standing on water deal half damage (rounded down, minimum 1). Turret and blade shots count too; casting again restarts it.</summary>
public sealed class RiteOfMists : ClassSkill
{
    public RiteOfMists(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        context.State.MistsUntil = context.State.Timeline.Now + RitualistSkills.MistLength;
        context.Settle();
    }
}

/// <summary>Deluge: water from the Ritualist lasts until the end of battle. The ground code reads this keystone.</summary>
public sealed class Deluge : KeystoneSkill
{
    public Deluge(SkillDef def) : base(def) { }
}

/// <summary>Tidecaller: allies standing on water have 1 less Delay on every skill.</summary>
public sealed class Tidecaller : KeystoneSkill
{
    public Tidecaller(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddSkillDelayModifier((st, userId, skillId) =>
            !st.GetUnit(ownerId).IsKnockedOut && st.GetUnit(userId).Side == st.GetUnit(ownerId).Side && RitualistSkills.OnWater(st, userId) ? -1 : 0);
}

/// <summary>Frostbinder: a unit standing on the Ritualist's ice is Immobilized (an aura that follows the board).</summary>
public sealed class Frostbinder : KeystoneSkill
{
    public Frostbinder(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAuraSync("frostbinder:" + ownerId, st =>
        {
            foreach (var unit in st.Units)
            {
                if (unit.IsRemoved || unit.IsKnockedOut || !st.Formation.Contains(unit.Id)) continue;
                var spot = st.Ground.SpotOf(unit.Id);
                var iced = !st.GetUnit(ownerId).IsKnockedOut && st.Ground.Find(spot, GroundKind.Ice) is { } ice && ice.SourceUnitId == ownerId && st.Ground.Affected(spot).Contains(unit.Id);
                var mine = Statuses.Get(unit, StatusIds.Immobilized) is { } s && s.Magnitude == RitualistSkills.FrostMarker && s.SourceUnitId == ownerId;
                if (iced && !Statuses.Has(unit, StatusIds.Immobilized))
                    Statuses.Apply(st, new StatusApplication(unit.Id, StatusIds.Immobilized, Magnitude: RitualistSkills.FrostMarker, SourceUnitId: ownerId));
                else if (!iced && mine) Statuses.Remove(st, unit.Id, StatusIds.Immobilized, StatusEndCause.Expired);
            }
        });
}
