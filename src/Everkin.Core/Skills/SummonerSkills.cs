using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Summoner (skills.md): golems and animated blades, plus the hidden attack skills its summons use on their own turns. Magical.</summary>
public static class SummonerSkills
{
    public const string ClayGolemId = "clay_golem";
    public const string RuneGolemId = "rune_golem";
    public const string BladesId = "animate_blades_unit";
    public const int BladesLifetime = 30;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new ClayGolem(D("clay_golem"));
        yield return new RuneGolem(D("rune_golem"));
        yield return new AnimateBlades(D("animate_blades"));
        yield return new GolemHeart(D("golem_heart"));
        yield return new Portal(D("portal"));
        yield return new Quickening(D("quickening"));

        // The hidden attacks the summons use on their turns (not content: they have no row in skills.md).
        yield return new SummonStrike("clay_golem_strike", new IntRange(5, 6), Delivery.Melee, SummonStrikeKind.Plain);
        yield return new SummonStrike("rune_golem_strike", new IntRange(3, 4), Delivery.Melee, SummonStrikeKind.Silence);
        yield return new SummonStrike("blade_shot", new IntRange(2, 3), Delivery.Projectile, SummonStrikeKind.Plain);
    }

    private static readonly Row[] AllRows = { Row.Front, Row.Middle, Row.Rear };

    internal static bool Portal(BattleState state, int userId) => state.GetUnit(userId).KnownSkills.Contains("portal");

    /// <summary>Any free spot of the user's side, between units or at either end (section 11).</summary>
    internal static IReadOnlyList<MoveOption> SummonPlaces(BattleState state, int userId) =>
        Summons.PlacementOptions(state, state.GetUnit(userId).Side, AllRows);

    /// <summary>The golems a Summoner has on the board.</summary>
    internal static IEnumerable<Unit> GolemsOf(BattleState state, int userId) =>
        state.Units.Where(u => u.SummonerId == userId && u.CreatureId is ClayGolemId or RuneGolemId && !u.IsKnockedOut && !u.IsRemoved);
}

public enum SummonStrikeKind { Plain, Silence }

/// <summary>A hidden attack of a golem or the blades: a hit with the stated damage; the Rune Golem's hit Silences the target for 10.</summary>
public sealed class SummonStrike : Skill
{
    private readonly IntRange _range;
    private readonly Delivery _delivery;
    private readonly SummonStrikeKind _kind;

    public SummonStrike(string id, IntRange range, Delivery delivery, SummonStrikeKind kind) : base(id, 3, 0)
    {
        _range = range;
        _delivery = delivery;
        _kind = kind;
    }

    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => _delivery;
    public override EffectTags Tags => (_delivery == Battle.Delivery.Melee ? EffectTags.Melee : EffectTags.Projectile) | EffectTags.Physical;
    public override bool IsAttack => true;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, _range);
        if (_kind == SummonStrikeKind.Silence && report.Outcome != HitOutcome.Missed && !context.State.GetUnit(report.EffectReceiverId).IsKnockedOut)
            Statuses.Apply(context.State, new StatusApplication(report.EffectReceiverId, StatusIds.Silenced, 10, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>A skill that places a summon on any free spot of your side (or on a random free spot with Portal).</summary>
public abstract class SummonSkill : ClassSkill
{
    protected SummonSkill(SkillDef def) : base(def, magical: true) { }
    public sealed override TargetKind Target => TargetKind.Placement;
    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) => SummonerSkills.SummonPlaces(state, userId);
    public override bool PlacesRandomly(BattleState state, int userId) => SummonerSkills.Portal(state, userId);

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place for it (or the side is at its cap)" : null;

    /// <summary>The place the player chose, or a random one (seeded) when Portal places the summon.</summary>
    protected static MoveOption ChoosePlace(SkillContext context)
    {
        var options = SummonerSkills.SummonPlaces(context.State, context.UserId);
        return SummonerSkills.Portal(context.State, context.UserId) || context.Placement is null ? ClassHelpers.Pick(context.State, options) : context.Placement.Value;
    }

    protected static int Summon(SkillContext context, MoveOption place, string name, int hp, int speed, string definition, string autoSkill, int? lifetime)
    {
        // An earlier golem may have left (Golem Heart): the chosen gap is kept as far as it still exists, otherwise any free spot.
        var options = SummonerSkills.SummonPlaces(context.State, context.UserId);
        if (!options.Contains(place))
        {
            var near = new MoveOption(place.Row, Math.Min(place.Index, context.State.Formation.Count(context.UserSide, place.Row)));
            place = options.Contains(near) ? near : ClassHelpers.Pick(context.State, options);
        }
        var id = Summons.Summon(context.State, new SummonSpec(context.UserSide, name, UnitKind.Summon, hp, speed, context.UserId, TakesTurns: true, lifetime, DefinitionId: definition),
            place.Row, place.Index);
        var unit = context.State.GetUnit(id);
        unit.LearnSkill(autoSkill);
        unit.AutoSkillId = autoSkill;
        if (context.User.KnownSkills.Contains("quickening") && context.State.Timeline.TurnOf(id) is { } first)
        {
            context.State.Timeline.Remove(first.Id);
            context.State.Timeline.AddTurnNow(id);                         // Quickening: the summon takes its first turn at once
        }
        return id;
    }

    /// <summary>With Golem Heart a new golem replaces the old one.</summary>
    protected static void ReplaceGolems(SkillContext context)
    {
        if (!context.User.KnownSkills.Contains("golem_heart")) return;
        foreach (var old in SummonerSkills.GolemsOf(context.State, context.UserId).ToArray())
            Summons.Remove(context.State, old.Id, RemovalCause.Replaced);
    }
}

/// <summary>Clay Golem: summon a golem (HP 25, Speed 3) that makes melee hits for 5-6.</summary>
public sealed class ClayGolem : SummonSkill
{
    public ClayGolem(SkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        ReplaceGolems(context);
        var hp = context.User.KnownSkills.Contains("golem_heart") ? 50 : 25;
        Summon(context, ChoosePlace(context), "Clay Golem", hp, 3, SummonerSkills.ClayGolemId, "clay_golem_strike", null);
        context.Settle();
    }
}

/// <summary>Rune Golem: summon a rune golem (HP 18, Speed 4) that makes melee hits for 3-4; each hit Silences the target for 10.</summary>
public sealed class RuneGolem : SummonSkill
{
    public RuneGolem(SkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        ReplaceGolems(context);
        var hp = context.User.KnownSkills.Contains("golem_heart") ? 36 : 18;
        Summon(context, ChoosePlace(context), "Rune Golem", hp, 4, SummonerSkills.RuneGolemId, "rune_golem_strike", null);
        context.Settle();
    }
}

/// <summary>Animate Blades: summon two sets of flying blades (HP 6, Speed 7) that make projectile hits for 2-3. They last 30. The second set goes on a random free spot.</summary>
public sealed class AnimateBlades : SummonSkill
{
    public AnimateBlades(SkillDef def) : base(def) { }

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place for it (or the side is at its cap)" : null;

    public override void Resolve(SkillContext context)
    {
        Summon(context, ChoosePlace(context), "Animated Blades", 6, 7, SummonerSkills.BladesId, "blade_shot", SummonerSkills.BladesLifetime);
        var more = SummonerSkills.SummonPlaces(context.State, context.UserId);
        if (more.Count > 0)
            Summon(context, ClassHelpers.Pick(context.State, more), "Animated Blades", 6, 7, SummonerSkills.BladesId, "blade_shot", SummonerSkills.BladesLifetime);
        context.Settle();
    }
}

/// <summary>Golem Heart: the Summoner keeps only one golem (a new one replaces the old), but its golems have double HP (Clay 50, Rune 36). The golem skills read this keystone.</summary>
public sealed class GolemHeart : KeystoneSkill
{
    public GolemHeart(SkillDef def) : base(def) { }
}

/// <summary>Portal: the Summoner's summons appear on a random free spot of your side instead of being placed. The summoning skills read this keystone.</summary>
public sealed class Portal : KeystoneSkill
{
    public Portal(SkillDef def) : base(def) { }
}

/// <summary>Quickening: the Summoner's summons take their first turn at once. The summoning skills read this keystone.</summary>
public sealed class Quickening : KeystoneSkill
{
    public Quickening(SkillDef def) : base(def) { }
}
