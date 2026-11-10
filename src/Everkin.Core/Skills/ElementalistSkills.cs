using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Elementalist's skills (skills.md Elementalist): Pyromancy, Stormcalling and Cryomancy. Every active is `magical` and direct.
/// The rulings of 2026-11-10 (spots, chains, rows, Cold Heart, Brittle) are in skills.md.
/// </summary>
public static class ElementalistSkills
{
    /// <summary>The reference length of a ground effect (game_system section 12.2).</summary>
    public const int FireLength = 20;
    public const int ChainStun = 5;
    public const int ShockStun = 10;
    public const int MegavoltSilence = 10;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new RowOfFire(D("row_of_fire"));
        yield return new Kindle(D("kindle"));
        yield return new Backdraft(D("backdraft"));
        yield return new FireLane(D("fire_lane"));
        yield return new Scorch(D("scorch"));
        yield return new IgnitionPoint(D("ignition_point"));
        yield return new ChainLightning(D("chain_lightning"));
        yield return new Shock(D("shock"));
        yield return new Arc(D("arc"));
        yield return new Tempest(D("tempest"));
        yield return new Megavolt(D("megavolt"));
        yield return new SteamBurst(D("steam_burst"));
        yield return new Freeze(D("freeze"));
        yield return new Glacier(D("glacier"));
        yield return new Brittle(D("brittle"));
        yield return new FrostNova(D("frost_nova"));
        yield return new ColdHeart(D("cold_heart"));
    }

    /// <summary>The spots one card width to the left and right in the same row, inside the row's width.</summary>
    internal static IEnumerable<Spot> Beside(Spot spot)
    {
        foreach (var c in new[] { spot.Coordinate - 2, spot.Coordinate + 2 })
            if (Math.Abs(c) <= 2 * (Formation.RowCapacity - 1) / 2) yield return spot with { Coordinate = c };
    }

    /// <summary>Sets fire on a spot for the reference length.</summary>
    internal static void Ignite(SkillContext context, Spot spot) =>
        context.State.Ground.Place(GroundKind.Burning, spot, FireLength, context.UserId);

    /// <summary>
    /// A lightning Stun on a unit. A unit standing in water also Stuns the units on the water spots touching its spot, for half the
    /// time rounded down (game_system section 12.2).
    /// </summary>
    internal static void Lightning(SkillContext context, int unitId, int duration)
    {
        var state = context.State;
        if (state.GetUnit(unitId).IsKnockedOut) return;
        Statuses.Apply(state, new StatusApplication(unitId, StatusIds.Stunned, duration, SourceUnitId: context.UserId));
        var spot = state.Ground.SpotOf(unitId);
        var half = duration / 2;
        if (half < 1 || !state.Ground.Has(spot, GroundKind.Water) || !state.Ground.Affected(spot).Contains(unitId)) return;
        foreach (var touching in state.Ground.WaterSpotsTouching(spot))
            foreach (var id in state.Ground.Affected(touching))
                if (id != unitId && !state.GetUnit(id).IsKnockedOut)
                    Statuses.Apply(state, new StatusApplication(id, StatusIds.Stunned, half, SourceUnitId: context.UserId));
    }

    /// <summary>
    /// Freezes units. With Cold Heart the Frozen skips the next 2 turns, only one unit stays Frozen (when several are frozen at once the
    /// seeded roll keeps one) and the Elementalist's earlier Frozen unit thaws.
    /// </summary>
    internal static void Freeze(SkillContext context, IEnumerable<int> ids)
    {
        var state = context.State;
        var targets = ids.Where(id => !state.GetUnit(id).IsKnockedOut).ToList();
        var cold = context.User.KnownSkills.Contains("cold_heart");
        if (cold)
        {
            if (targets.Count > 1) targets = new List<int> { HealerSkills.Pick(state, targets) };
            foreach (var unit in state.Units.ToArray())
                if (!targets.Contains(unit.Id) && Statuses.Get(unit, StatusIds.Frozen)?.SourceUnitId == context.UserId)
                    Statuses.Remove(state, unit.Id, StatusIds.Frozen, StatusEndCause.Expired);
        }
        foreach (var id in targets)
            Statuses.Apply(state, new StatusApplication(id, StatusIds.Frozen, SourceUnitId: context.UserId, Uses: cold ? 2 : null));
    }
}

/// <summary>Base of the Elementalist's actives: `magical`, direct, distance 3, one unit by default.</summary>
public abstract class ElementalistSkill : ProfessionSkill
{
    protected ElementalistSkill(ProfessionSkillDef def) : base(def) { }
    public override bool Magical => true;
    public override EffectTags Tags => EffectTags.Magical;
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
}

// ------------------------------------------------------------------------------------------------ Pyromancy

/// <summary>Row of Fire: set fire to a whole row for 20 time units: the spot of every unit standing in it.</summary>
public sealed class RowOfFire : ElementalistSkill
{
    public RowOfFire(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        var units = context.State.Formation.Units(row.Side, row.Row).ToArray();
        foreach (var id in units)
            ElementalistSkills.Ignite(context, context.State.Ground.SpotOf(id));
        context.Settle();
    }
}

/// <summary>Kindle: set fire to a unit's spot. With Scorch it also spreads to the spots next to it.</summary>
public sealed class Kindle : ElementalistSkill
{
    public Kindle(ProfessionSkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        var spot = context.State.Ground.SpotOf(context.TargetId!.Value);
        ElementalistSkills.Ignite(context, spot);
        if (context.User.KnownSkills.Contains("scorch"))
            foreach (var next in ElementalistSkills.Beside(spot)) ElementalistSkills.Ignite(context, next);
        context.Settle();
    }
}

/// <summary>Backdraft: the fire on a burning unit's spot spreads to the spots next to it, burning as long as the source fire has left.</summary>
public sealed class Backdraft : ElementalistSkill
{
    public Backdraft(ProfessionSkillDef def) : base(def) { }

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        state.Ground.Has(state.Ground.SpotOf(targetId), GroundKind.Burning);

    public override void Resolve(SkillContext context)
    {
        var ground = context.State.Ground;
        var spot = ground.SpotOf(context.TargetId!.Value);
        if (context.User.KnownSkills.Contains("ignition_point"))
            ground.Place(GroundKind.Burning, spot, ElementalistSkills.FireLength, context.UserId);   // refreshed to 20
        var remaining = ground.Find(spot, GroundKind.Burning) is { } fire ? fire.EndsAt - context.State.Timeline.Now : 0;
        if (remaining >= 1)
            foreach (var next in ElementalistSkills.Beside(spot))
                if (!ground.Has(next, GroundKind.Burning)) ground.Place(GroundKind.Burning, next, remaining, context.UserId);
        context.Settle();
    }
}

/// <summary>Fire Lane: set fire to a unit's spot and to the same column in every other row of its side.</summary>
public sealed class FireLane : ElementalistSkill
{
    public FireLane(ProfessionSkillDef def) : base(def) { }
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        var spot = context.State.Ground.SpotOf(context.TargetId!.Value);
        for (var r = 0; r < Formation.RowCount; r++)
            ElementalistSkills.Ignite(context, spot with { Row = (Row)r });
        context.Settle();
    }
}

/// <summary>Scorch (modifier of Kindle): the fire also spreads to the spots next to it. Read by Kindle.</summary>
public sealed class Scorch : ModifierSkill
{
    public Scorch(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Ignition Point (modifier of Backdraft): Backdraft refreshes the fire's duration to 20. Read by Backdraft.</summary>
public sealed class IgnitionPoint : ModifierSkill
{
    public IgnitionPoint(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Stormcalling

/// <summary>Chain Lightning: Stuns a unit for 5 time units, then jumps to an adjacent unit not yet hit and Stuns it, then once more (Arc: 2 more).</summary>
public sealed class ChainLightning : ElementalistSkill
{
    public ChainLightning(ProfessionSkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var current = context.TargetId!.Value;
        var hit = new HashSet<int> { current };
        ElementalistSkills.Lightning(context, current, ElementalistSkills.ChainStun);
        var jumps = context.User.KnownSkills.Contains("arc") ? 4 : 2;
        for (var i = 0; i < jumps; i++)
        {
            var next = state.Formation.Adjacent(current).Where(id => !hit.Contains(id) && !state.GetUnit(id).IsKnockedOut).ToList();
            if (next.Count == 0) break;
            current = HealerSkills.Pick(state, next);
            hit.Add(current);
            ElementalistSkills.Lightning(context, current, ElementalistSkills.ChainStun);
        }
        context.Settle();
    }
}

/// <summary>Shock: Stuns any unit for 10 time units; it cannot be intercepted (it is direct).</summary>
public sealed class Shock : ElementalistSkill
{
    public Shock(ProfessionSkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        ElementalistSkills.Lightning(context, context.TargetId!.Value, ElementalistSkills.ShockStun);
        context.Settle();
    }
}

/// <summary>Arc (modifier of Chain Lightning): it jumps 2 more times. Read by Chain Lightning.</summary>
public sealed class Arc : ModifierSkill
{
    public Arc(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Tempest: push every unit in one row one row back (bumps apply). No damage. Not a delivery of its own, reach distance 1.</summary>
public sealed class Tempest : ElementalistSkill
{
    public Tempest(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override Delivery? Delivery => null;
    public override Reach Reach => Reach.Distance(1);

    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) =>
        HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var row = context.Row!.Value;
        if (row.Row == Row.Rear) { context.Settle(); return; }
        context.PushGroupBack(state.Formation.Units(row.Side, row.Row).ToArray());     // the row moves together
    }
}

/// <summary>Megavolt: every unit in one row is Stunned for 5 time units; the Elementalist is Silenced for 10 time units.</summary>
public sealed class Megavolt : ElementalistSkill
{
    public Megavolt(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        foreach (var id in context.State.Formation.Units(row.Side, row.Row).ToArray())
            ElementalistSkills.Lightning(context, id, ElementalistSkills.ChainStun);
        if (!context.User.IsKnockedOut)
            Statuses.Apply(context.State, new StatusApplication(context.UserId, StatusIds.Silenced, ElementalistSkills.MegavoltSilence, SourceUnitId: context.UserId));
        context.Settle();
    }
}

// ------------------------------------------------------------------------------------------------ Cryomancy

/// <summary>Steam Burst (passive): fire set on an iced spot ends both and Confuses the unit there. The ground rules (GroundField) already do it for every source.</summary>
public sealed class SteamBurst : PassiveSkill
{
    public SteamBurst(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Freeze: a unit is Frozen.</summary>
public sealed class Freeze : ElementalistSkill
{
    public Freeze(ProfessionSkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        ElementalistSkills.Freeze(context, new[] { context.TargetId!.Value });
        context.Settle();
    }
}

/// <summary>Glacier: every unit in one row is Frozen.</summary>
public sealed class Glacier : ElementalistSkill
{
    public Glacier(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.Row;
    public override bool IsArea => true;
    public override IReadOnlyList<RowTarget> LegalRows(BattleState state, int userId) => HealerSkills.RowsInReach(state, userId, Reach);

    public override void Resolve(SkillContext context)
    {
        var row = context.Row!.Value;
        ElementalistSkills.Freeze(context, context.State.Formation.Units(row.Side, row.Row).ToArray());
        context.Settle();
    }
}

/// <summary>Brittle (passive): Frozen enemies of the Elementalist take 50% more damage from hits (× 3 ÷ 2, rounded down, before shields).</summary>
public sealed class Brittle : PassiveSkill
{
    public Brittle(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddDamageModifier((st, source, targetId, amount) =>
        {
            var owner = st.GetUnit(ownerId);
            var target = st.GetUnit(targetId);
            return !owner.IsKnockedOut && target.Side != owner.Side && Statuses.Has(target, StatusIds.Frozen) ? amount * 3 / 2 : amount;
        });
}

/// <summary>Frost Nova: every enemy within distance 1 of the Elementalist is Frozen.</summary>
public sealed class FrostNova : ElementalistSkill
{
    public FrostNova(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.None;
    public override bool IsArea => true;
    public override Reach Reach => Reach.Distance(1);

    public override void Resolve(SkillContext context)
    {
        ElementalistSkills.Freeze(context, Targeting.LegalTargets(context.State, context.UserId, Reach, new TargetOptions(TargetSide.EnemySideOnly)));
        context.Settle();
    }
}

/// <summary>Cold Heart (keystone): Frozen from the Elementalist lasts 1 more turn, but it keeps only one unit Frozen at a time. Read by the freezing skills.</summary>
public sealed class ColdHeart : PassiveSkill
{
    public ColdHeart(ProfessionSkillDef def) : base(def) { }
}
