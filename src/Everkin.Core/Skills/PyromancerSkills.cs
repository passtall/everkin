using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Pyromancer (skills.md): fire damage and burning ground. Magical.</summary>
public static class PyromancerSkills
{
    /// <summary>The reference length of a ground effect (game_system.md section 12.2): fire from these skills burns 20.</summary>
    public const int FireLength = 20;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new Fireball(D("fireball"));
        yield return new Kindle(D("kindle"));
        yield return new RowOfFire(D("row_of_fire"));
        yield return new Wildfire(D("wildfire"));
        yield return new Inferno(D("inferno"));
        yield return new Ashwalker(D("ashwalker"));
    }

    /// <summary>The spots one card width to the left and right in the same row, inside the row width (coordinates -5 to 5).</summary>
    internal static IEnumerable<Spot> Beside(Spot spot)
    {
        foreach (var c in new[] { spot.Coordinate - 2, spot.Coordinate + 2 })
            if (Math.Abs(c) <= 5) yield return spot with { Coordinate = c };
    }

    /// <summary>Sets fire on a spot (Wildfire and Volatile change the length where the ground is placed).</summary>
    internal static void Ignite(SkillContext context, Spot spot) =>
        context.State.Ground.Place(GroundKind.Burning, spot, FireLength, context.UserId);
}

/// <summary>Fireball: hit for 8-9 and set fire to the target's spot. An intercepted Fireball hits the interceptor and sets fire to its spot.</summary>
public sealed class Fireball : ClassSkill
{
    public Fireball(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override bool IsAttack => true;

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, new IntRange(8, 9));
        if (report.Outcome == HitOutcome.Missed || context.Over) return;
        var receiver = report.EffectReceiverId;
        if (context.State.GetUnit(receiver).IsKnockedOut || !context.State.Formation.Contains(receiver)) { context.Settle(); return; }
        PyromancerSkills.Ignite(context, context.State.Ground.SpotOf(receiver));
        context.Settle();
    }
}

/// <summary>Kindle: set fire to a unit's spot and the spots next to it.</summary>
public sealed class Kindle : ClassSkill
{
    public Kindle(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;

    public override void Resolve(SkillContext context)
    {
        var spot = context.State.Ground.SpotOf(context.TargetId!.Value);
        PyromancerSkills.Ignite(context, spot);
        foreach (var next in PyromancerSkills.Beside(spot)) PyromancerSkills.Ignite(context, next);
        context.Settle();
    }
}

/// <summary>Row of Fire: set fire to a whole row: the spot of every unit standing in it (empty gaps get nothing).</summary>
public sealed class RowOfFire : ClassSkill
{
    public RowOfFire(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => ClassHelpers.AllRows();

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).ToArray())
            PyromancerSkills.Ignite(context, context.State.Ground.SpotOf(id));
        context.Settle();
    }
}

/// <summary>Wildfire: the Pyromancer's burning ground lasts twice as long. The ground code reads this keystone when the fire is placed.</summary>
public sealed class Wildfire : KeystoneSkill
{
    public Wildfire(SkillDef def) : base(def) { }
}

/// <summary>Inferno: burning ground from the Pyromancer deals double damage. The ground code reads this keystone.</summary>
public sealed class Inferno : KeystoneSkill
{
    public Inferno(SkillDef def) : base(def) { }
}

/// <summary>Ashwalker: the Pyromancer takes no damage from burning ground (the ground code reads this) and has +1 Speed while standing on it.</summary>
public sealed class Ashwalker : KeystoneSkill
{
    public Ashwalker(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddAuraSync("ashwalker", st =>
        {
            foreach (var unit in st.Units)
            {
                if (unit.IsRemoved || !unit.KnownSkills.Contains("ashwalker")) continue;
                var onFire = !unit.IsKnockedOut && st.Formation.Contains(unit.Id)
                    && st.Ground.Has(st.Ground.SpotOf(unit.Id), GroundKind.Burning) && st.Ground.Affected(st.Ground.SpotOf(unit.Id)).Contains(unit.Id);
                var has = Statuses.Has(unit, StatusIds.AshSpeed);
                if (onFire && !has) Statuses.Apply(st, new StatusApplication(unit.Id, StatusIds.AshSpeed, Magnitude: 1, SourceUnitId: unit.Id));
                else if (!onFire && has) Statuses.Remove(st, unit.Id, StatusIds.AshSpeed, StatusEndCause.Expired);
            }
        });
}
