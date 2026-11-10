using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Summoner's skills (skills.md Summoner): Necromancer, Golemancer and Conjurer, plus the hidden attack skills their summons use on
/// their own turns. The Summoner's actives are `magical`. The rulings of 2026-11-10 are in skills.md.
/// </summary>
public static class SummonerSkills
{
    public const string ClayGolem = "clay_golem";
    public const string RuneGolem = "rune_golem";
    public const string ClockworkGolem = "clockwork_golem";
    public const string BellGolem = "bell_golem";
    public const string Blades = "animate_blades_unit";
    public const string Mirror = "animate_mirror_unit";
    public const int UndeadHp = 12;
    public const int UndeadPower = 4;
    public const int ExplosionDamage = 6;
    public const int DesecrateLength = 40;
    public const int ConjuredLifetime = 30;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new RaiseUndead(D("raise_undead"));
        yield return new CorpseExplosion(D("corpse_explosion"));
        yield return new MassGrave(D("mass_grave"));
        yield return new PlagueCorpse(D("plague_corpse"));
        yield return new Desecrate(D("desecrate"));
        yield return new LichForm(D("lich_form"));
        yield return new ClayGolemSkill(D("clay_golem"));
        yield return new RuneGolemSkill(D("rune_golem"));
        yield return new ClockworkGolemSkill(D("clockwork_golem"));
        yield return new BellGolemSkill(D("bell_golem"));
        yield return new GolemHeart(D("golem_heart"));
        yield return new AnimateBlades(D("animate_blades"));
        yield return new Portal(D("portal"));
        yield return new BladeStorm(D("blade_storm"));
        yield return new AnimateMirror(D("animate_mirror"));

        // The hidden skills the summons use on their turns (not content: they have no node in a tree).
        yield return new SummonStrike("clay_golem_strike", new IntRange(5, 6), Delivery.Melee, Reach.Distance(1), SummonStrikeKind.Plain);
        yield return new SummonStrike("rune_golem_strike", new IntRange(3, 4), Delivery.Melee, Reach.Distance(1), SummonStrikeKind.Silence);
        yield return new SummonStrike("clockwork_golem_strike", new IntRange(3, 4), Delivery.Melee, Reach.Distance(1), SummonStrikeKind.Clockwork);
        yield return new SummonStrike("blade_shot", new IntRange(2, 3), Delivery.Projectile, Reach.Unlimited, SummonStrikeKind.Plain);
        yield return new BellPulse();
    }

    private static readonly Row[] AllRows = { Row.Front, Row.Middle, Row.Rear };

    internal static bool Portal(BattleState state, int userId) => state.GetUnit(userId).KnownSkills.Contains("portal");

    /// <summary>Where a summon may appear: a free gap next to the Summoner, or any free gap of the side with Portal.</summary>
    internal static IReadOnlyList<MoveOption> SummonPlaces(BattleState state, int userId)
    {
        var all = Summons.PlacementOptions(state, state.GetUnit(userId).Side, AllRows);
        return Portal(state, userId) ? all : Summons.GapsNextToUser(state, userId).Where(all.Contains).ToArray();
    }

    /// <summary>The unit that an autonomous summon with this definition is, and the golems of a Summoner.</summary>
    internal static bool IsGolem(Unit u) => u.CreatureId is ClayGolem or RuneGolem or ClockworkGolem or BellGolem;

    /// <summary>The hidden skill id of an undead or a copy of a creature's Attack Type attack, or null if the unit has no creature.</summary>
    internal static string? AttackSkillOf(SkillCatalog catalog, Unit unit) =>
        unit.CreatureId is { } id && catalog.Content.CreaturesById.TryGetValue(id, out var def) ? catalog.AttackSkillFor(def.AttackType) : null;
}

public enum SummonStrikeKind { Plain, Silence, Clockwork }

/// <summary>A hidden attack of a golem or the blades: a hit with the stated damage; the Rune Golem Silences, the Clockwork Golem hastens its Summoner.</summary>
public sealed class SummonStrike : Skill
{
    private readonly IntRange _range;
    private readonly Delivery _delivery;
    private readonly Reach _reach;
    private readonly SummonStrikeKind _kind;

    public SummonStrike(string id, IntRange range, Delivery delivery, Reach reach, SummonStrikeKind kind) : base(id, 3, 0)
    {
        _range = range;
        _delivery = delivery;
        _reach = reach;
        _kind = kind;
    }

    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => _delivery;
    public override EffectTags Tags => (_delivery == Battle.Delivery.Melee ? EffectTags.Melee : EffectTags.Projectile) | EffectTags.Physical;
    public override bool IsAttack => true;
    public override Reach Reach => _reach;
    public override TargetOptions Options => new(TargetSide.EnemySideOnly);

    public override void Resolve(SkillContext context)
    {
        var report = context.Hit(context.TargetId!.Value, _range);
        if (_kind == SummonStrikeKind.Silence && report.Outcome != HitOutcome.Missed && !context.State.GetUnit(report.EffectReceiverId).IsKnockedOut)
            Statuses.Apply(context.State, new StatusApplication(report.EffectReceiverId, StatusIds.Silenced, 10, SourceUnitId: context.UserId));
        if (_kind == SummonStrikeKind.Clockwork && context.User.SummonerId is { } summoner && context.State.Timeline.TurnOf(summoner) is { } turn
            && !context.State.GetUnit(summoner).IsKnockedOut)
            context.State.Timeline.PullForward(turn, 2);
        context.Settle();
    }
}

/// <summary>The Bell Golem's turn: every adjacent ally loses one harmful status, chosen by the seeded roll.</summary>
public sealed class BellPulse : Skill
{
    public BellPulse() : base("bell_golem_pulse", 3, 0) { }
    public override TargetKind Target => TargetKind.None;

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        foreach (var id in state.Formation.Adjacent(context.UserId))
        {
            var ally = state.GetUnit(id);
            if (ally.IsKnockedOut || ally.Side != context.UserSide) continue;
            var harmful = ally.StatusInstances.Keys.Where(s => StatusRules.PolarityOf(s) == Polarity.Harmful).ToList();
            if (harmful.Count > 0) Statuses.Remove(state, id, HealerSkills.Pick(state, harmful), StatusEndCause.Cleansed, context.UserId);
        }
        context.Settle();
    }
}

/// <summary>Plays the turns of autonomous summons and undead (game_system section 11.2).</summary>
public static class AutonomousTurns
{
    public static void Play(BattleState state, SkillCatalog catalog, int unitId)
    {
        var unit = state.GetUnit(unitId);
        if (unit.AutoSkillId is not { } skillId || !catalog.TryGet(skillId, out var skill)) { Emit(state, Actions.SkipTurn(state, unitId)); return; }
        if (!unit.KnownSkills.Contains(skillId)) unit.LearnSkill(skillId);

        int? target = null;
        if (skill.Target == TargetKind.Unit)
        {
            target = Targeting.PickNearest(state, unitId, Actions.ReachOf(state, unitId, skill), skill.Options with { Delivery = skill.Delivery, Side = TargetSide.EnemySideOnly });
            if (target is null) { Emit(state, Actions.SkipTurn(state, unitId)); return; }
        }
        var use = new SkillUse(unitId, skillId, target);
        if (Actions.WhyNot(state, catalog, use) is not null) { Emit(state, Actions.SkipTurn(state, unitId)); return; }
        Emit(state, Actions.Use(state, catalog, use));
    }

    /// <summary>The actions return their events; the driver's caller reads them from the state's buffer.</summary>
    private static void Emit(BattleState state, IReadOnlyList<BattleEvent> events)
    {
        foreach (var e in events) state.Emit(e);
    }
}

/// <summary>Base of the Summoner's actives: `magical`.</summary>
public abstract class SummonerSkill : ProfessionSkill
{
    protected SummonerSkill(ProfessionSkillDef def) : base(def) { }
    public override bool Magical => true;
    public override EffectTags Tags => EffectTags.Magical;
    public override TargetKind Target => TargetKind.Unit;
}

/// <summary>A skill that places a summon next to the Summoner (or on a random free spot with Portal).</summary>
public abstract class SummonSkill : SummonerSkill
{
    protected SummonSkill(ProfessionSkillDef def) : base(def) { }
    public sealed override TargetKind Target => TargetKind.Placement;
    public override IReadOnlyList<MoveOption> PlacementOptions(BattleState state, int userId) => SummonerSkills.SummonPlaces(state, userId);
    public override bool PlacesRandomly(BattleState state, int userId) => SummonerSkills.Portal(state, userId);

    public override string? Unavailable(BattleState state, int userId) =>
        PlacementOptions(state, userId).Count == 0 ? "no free place for it (or the side is at its cap)" : null;

    /// <summary>The place the player chose, or a random one (seeded) when Portal places the summon.</summary>
    protected static MoveOption ChoosePlace(SkillContext context)
    {
        var options = SummonerSkills.SummonPlaces(context.State, context.UserId);
        return SummonerSkills.Portal(context.State, context.UserId) || context.Placement is null ? HealerSkills.Pick(context.State, options) : context.Placement.Value;
    }

    protected static int Summon(SkillContext context, MoveOption place, string name, int hp, int speed, string definition, bool takesTurns, int? lifetime)
    {
        var id = Summons.Summon(context.State, new SummonSpec(context.UserSide, name, UnitKind.Summon, hp, speed, context.UserId, takesTurns, lifetime,
            DefinitionId: definition), place.Row, place.Index);
        return id;
    }

    protected static void Autonomous(SkillContext context, int unitId, string skillId)
    {
        var unit = context.State.GetUnit(unitId);
        unit.LearnSkill(skillId);
        unit.AutoSkillId = skillId;
    }
}

// ------------------------------------------------------------------------------------------------ Necromancer

/// <summary>Raise Undead: turns one KO body into an undead of yours (HP 12, Power 4), placed in a free slot. Mass Grave raises one more.</summary>
public sealed class RaiseUndead : SummonerSkill
{
    public RaiseUndead(ProfessionSkillDef def) : base(def) { }
    public override TargetKind Target => TargetKind.UnitAndPlacement;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(IncludeKnockedOut: true);
    public override bool PlacesRandomly(BattleState state, int userId) => SummonerSkills.Portal(state, userId);

    public override bool TargetFilter(BattleState state, int userId, int targetId) => Raisable(state, userId, targetId);

    private static bool Raisable(BattleState state, int userId, int bodyId)
    {
        var body = state.GetUnit(bodyId);
        return body.IsKnockedOut && !body.IsRemoved && state.Formation.Contains(bodyId) && Necromancy.RaisePlacements(state, userId, bodyId).Count > 0;
    }

    public override IReadOnlyList<MoveOption> PlacementOptionsFor(BattleState state, int userId, int targetId) =>
        Necromancy.RaisePlacements(state, userId, targetId);

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var body = context.TargetId!.Value;
        var place = SummonerSkills.Portal(state, context.UserId) || context.Placement is null
            ? HealerSkills.Pick(state, Necromancy.RaisePlacements(state, context.UserId, body)) : context.Placement.Value;
        Raise(context, body, place);

        if (context.User.KnownSkills.Contains("mass_grave"))
        {
            var bodies = state.Units.Where(u => u.Id != body && Raisable(state, context.UserId, u.Id)).Select(u => u.Id).ToList();
            if (bodies.Count > 0)
            {
                var another = HealerSkills.Pick(state, bodies);
                Raise(context, another, HealerSkills.Pick(state, Necromancy.RaisePlacements(state, context.UserId, another)));
            }
        }
        context.Settle();
    }

    private static void Raise(SkillContext context, int body, MoveOption place)
    {
        var power = SummonerSkills.UndeadPower + (context.User.KnownSkills.Contains("lich_form") ? 2 : 0);
        Necromancy.Raise(context.State, context.UserId, body, place.Row, place.Index, SummonerSkills.UndeadHp, power);
        var unit = context.State.GetUnit(body);
        if (SummonerSkills.AttackSkillOf(context.Catalog!, unit) is { } attack)
        {
            unit.LearnSkill(attack);
            unit.AutoSkillId = attack;
        }
    }
}

/// <summary>Corpse Explosion: a body or an undead of the Summoner explodes: 6 to every unit next to it; an undead takes the 6 too.</summary>
public sealed class CorpseExplosion : SummonerSkill
{
    public CorpseExplosion(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override bool IsArea => true;
    public override TargetOptions Options => new(IncludeKnockedOut: true);

    public override bool TargetFilter(BattleState state, int userId, int targetId)
    {
        var u = state.GetUnit(targetId);
        if (u.IsRemoved || !state.Formation.Contains(targetId)) return false;
        return u.IsKnockedOut || (u.IsUndead && u.SummonerId == userId);
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        var hits = state.Formation.Adjacent(target).Where(id => !state.GetUnit(id).IsKnockedOut)
            .Select(id => new SkillContext.HitSpec(id, new IntRange(SummonerSkills.ExplosionDamage, SummonerSkills.ExplosionDamage))).ToList();
        if (!state.GetUnit(target).IsKnockedOut) hits.Add(new SkillContext.HitSpec(target, new IntRange(SummonerSkills.ExplosionDamage, SummonerSkills.ExplosionDamage)));
        var reports = context.HitBlock(hits);
        if (context.User.KnownSkills.Contains("plague_corpse"))
            foreach (var r in reports.Where(r => r.Outcome != HitOutcome.Missed))
                if (!state.GetUnit(r.TargetId).IsKnockedOut)
                    Statuses.Apply(state, new StatusApplication(r.TargetId, StatusIds.Poison, 20, SourceUnitId: context.UserId));
        context.Settle();
    }
}

/// <summary>Mass Grave (modifier of Raise Undead): it raises one more random body. Read by Raise Undead.</summary>
public sealed class MassGrave : ModifierSkill
{
    public MassGrave(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Plague Corpse (modifier of Corpse Explosion): every unit hit is Poisoned for 20 time units. Read by Corpse Explosion.</summary>
public sealed class PlagueCorpse : ModifierSkill
{
    public PlagueCorpse(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Desecrate: a KO body cannot be revived for 40 time units.</summary>
public sealed class Desecrate : SummonerSkill
{
    public Desecrate(ProfessionSkillDef def) : base(def) { }
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
    public override TargetOptions Options => new(IncludeKnockedOut: true);

    public override bool TargetFilter(BattleState state, int userId, int targetId) =>
        state.GetUnit(targetId).IsKnockedOut && !state.GetUnit(targetId).IsRemoved;

    public override void Resolve(SkillContext context)
    {
        context.State.GetUnit(context.TargetId!.Value).NoReviveUntil = context.State.Timeline.Now + SummonerSkills.DesecrateLength;
        context.Settle();
    }
}

/// <summary>Lich Form (keystone): the Summoner counts as undead for healing, but its undead have +2 Power. Read by <see cref="Unit.HealingHurts"/> and Raise Undead.</summary>
public sealed class LichForm : PassiveSkill
{
    public LichForm(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Golemancer

/// <summary>Base of the golem summoning skills.</summary>
public abstract class GolemSkill : SummonSkill
{
    private readonly string _name;
    private readonly int _hp;
    private readonly int _speed;
    private readonly string _definition;
    private readonly string _auto;

    protected GolemSkill(ProfessionSkillDef def, string name, int hp, int speed, string definition, string auto) : base(def)
    {
        _name = name;
        _hp = hp;
        _speed = speed;
        _definition = definition;
        _auto = auto;
    }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var heart = context.User.KnownSkills.Contains("golem_heart");
        if (heart)
            foreach (var old in state.Units.Where(u => u.SummonerId == context.UserId && SummonerSkills.IsGolem(u) && !u.IsRemoved).ToArray())
                Summons.Remove(state, old.Id, RemovalCause.Expired);
        var place = ChoosePlace(context);
        var id = Summon(context, place, _name, heart ? _hp * 2 : _hp, _speed, _definition, takesTurns: true, lifetime: null);
        Autonomous(context, id, _auto);
        context.Settle();
    }
}

/// <summary>Clay Golem: HP 25, Speed 3, melee hits for 5–6.</summary>
public sealed class ClayGolemSkill : GolemSkill
{
    public ClayGolemSkill(ProfessionSkillDef def) : base(def, "Clay Golem", 25, 3, SummonerSkills.ClayGolem, "clay_golem_strike") { }
}

/// <summary>Rune Golem: HP 18, Speed 4, melee hits for 3–4 that Silence for 10 time units.</summary>
public sealed class RuneGolemSkill : GolemSkill
{
    public RuneGolemSkill(ProfessionSkillDef def) : base(def, "Rune Golem", 18, 4, SummonerSkills.RuneGolem, "rune_golem_strike") { }
}

/// <summary>Clockwork Golem: HP 16, Speed 5, melee hits for 3–4; each time it acts the Summoner's next action comes 2 sooner.</summary>
public sealed class ClockworkGolemSkill : GolemSkill
{
    public ClockworkGolemSkill(ProfessionSkillDef def) : base(def, "Clockwork Golem", 16, 5, SummonerSkills.ClockworkGolem, "clockwork_golem_strike") { }
}

/// <summary>Bell Golem: HP 18, Speed 4; does not attack; each time it acts every adjacent ally loses a harmful status.</summary>
public sealed class BellGolemSkill : GolemSkill
{
    public BellGolemSkill(ProfessionSkillDef def) : base(def, "Bell Golem", 18, 4, SummonerSkills.BellGolem, "bell_golem_pulse") { }
}

/// <summary>Golem Heart (keystone): the Summoner can keep only one golem, but it has double HP. Read by the golem skills.</summary>
public sealed class GolemHeart : PassiveSkill
{
    public GolemHeart(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Conjurer

/// <summary>Animate Blades: flying blades (HP 6, Speed 7) make ranged hits for 2–3. Lasts 30 time units. Blade Storm summons two sets.</summary>
public sealed class AnimateBlades : SummonSkill
{
    public AnimateBlades(ProfessionSkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        Set(context, ChoosePlace(context));
        if (context.User.KnownSkills.Contains("blade_storm"))
        {
            var more = SummonerSkills.SummonPlaces(context.State, context.UserId);
            if (more.Count > 0) Set(context, HealerSkills.Pick(context.State, more));
        }
        context.Settle();
    }

    private static void Set(SkillContext context, MoveOption place)
    {
        var id = Summon(context, place, "Animate Blades", 6, 7, SummonerSkills.Blades, takesTurns: true, lifetime: SummonerSkills.ConjuredLifetime);
        Autonomous(context, id, "blade_shot");
    }
}

/// <summary>Portal (passive): the Summoner's summons appear on a random free spot of your side. Read by the summoning skills.</summary>
public sealed class Portal : PassiveSkill
{
    public Portal(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Blade Storm (modifier of Animate Blades): it summons two sets of blades. Read by Animate Blades.</summary>
public sealed class BladeStorm : ModifierSkill
{
    public BladeStorm(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Animate Mirror: a mirror (HP 8, Speed 5, 30 time units) answers each hit on it with the attacker's own basic attack.</summary>
public sealed class AnimateMirror : SummonSkill
{
    public AnimateMirror(ProfessionSkillDef def) : base(def) { }

    public override void Resolve(SkillContext context)
    {
        var state = context.State;
        var catalog = context.Catalog!;
        var mirror = Summon(context, ChoosePlace(context), "Animate Mirror", 8, 5, SummonerSkills.Mirror, takesTurns: false, lifetime: SummonerSkills.ConjuredLifetime);

        void Answer(BattleState st, ReactionContext ctx)
        {
            if (ctx.Trigger.SourceUnitId is not { } attackerId || st.GetUnit(attackerId).IsKnockedOut) return;
            if (SummonerSkills.AttackSkillOf(catalog, st.GetUnit(attackerId)) is not { } skillId) return;
            var mirrorUnit = st.GetUnit(mirror);
            if (mirrorUnit.Answering) return;                                            // two mirrors cannot answer each other forever
            var skill = catalog.Get(skillId);
            mirrorUnit.Answering = true;
            try
            {
                var target = skill.Target == TargetKind.Unit ? attackerId : (int?)null;
                skill.Resolve(new SkillContext(st, skill, mirror, new SkillUse(mirror, skillId, target), target) { Catalog = catalog });
            }
            finally { mirrorUnit.Answering = false; }
        }
        state.AddReaction(new ReactionDef("mirror_hit", mirror, TriggerKind.WhenHit, Answer) { CanChain = false });
        state.AddReaction(new ReactionDef("mirror_ko", mirror, TriggerKind.WhenKo, Answer) { CanChain = false });
        context.Settle();
    }
}
