using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The Sorcerer's skills (skills.md Sorcerer): Enchantment, Transmutation and Malediction. Every active is `magical`, direct, reaches
/// distance 3 and only has effects: statuses on the target unit, which may be on either side. The Sorcerer rulings of 2026-11-10 are
/// in skills.md.
/// </summary>
public static class SorcererSkills
{
    /// <summary>How long an unused enchantment lasts without Enchanted Soul, and how long a curse lasts without Witch's Patience.</summary>
    public const int EnchantmentLength = 20;
    public const int CurseLength = 20;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        ProfessionSkillDef D(string id) => content.ProfessionSkills.Single(s => s.Id == id);
        yield return new EchoEnchant(D("echo_enchant"));
        yield return new PhaseWeapon(D("phase_weapon"));
        yield return new PhasingStrike(D("phasing_strike"));
        yield return new PiercingEnchant(D("piercing_enchant"));
        yield return new EnchantedSoul(D("enchanted_soul"));
        yield return new Petrify(D("petrify"));
        yield return new Transmute(D("transmute"));
        yield return new StoneShell(D("stone_shell"));
        yield return new LastingTransmute(D("lasting_transmute"));
        yield return new GlassBody(D("glass_body"));
        yield return new CurseOfStagnation(D("curse_of_stagnation"));
        yield return new CurseOfBetrayal(D("curse_of_betrayal"));
        yield return new CurseOfWeight(D("curse_of_weight"));
        yield return new LingeringMalice(D("lingering_malice"));
        yield return new WitchsPatience(D("witchs_patience"));
    }

    /// <summary>Puts an enchantment on a unit. With Enchanted Soul it never fades but the Sorcerer's earlier enchantment, on any unit, is removed.</summary>
    internal static void Enchant(SkillContext context, string statusId, int magnitude = 0)
    {
        var soul = context.User.KnownSkills.Contains("enchanted_soul");
        if (soul) RemoveEarlier(context, StatusIds.IsEnchantment);
        var target = context.TargetId!.Value;
        context.ApplyStatus(target, new StatusApplication(target, statusId, soul ? null : EnchantmentLength, magnitude));
    }

    /// <summary>Puts a curse on a unit. With Witch's Patience it lasts until removed but the Sorcerer's earlier curse, on any unit, is removed.</summary>
    internal static void Curse(SkillContext context, string statusId)
    {
        var patience = context.User.KnownSkills.Contains("witchs_patience");
        if (patience) RemoveEarlier(context, StatusIds.IsCurse);
        var target = context.TargetId!.Value;
        context.ApplyStatus(target, new StatusApplication(target, statusId, patience ? null : CurseLength));
    }

    private static void RemoveEarlier(SkillContext context, Func<string, bool> kind)
    {
        foreach (var unit in context.State.Units.ToArray())
            foreach (var status in unit.StatusInstances.Values.Where(s => kind(s.Id) && s.SourceUnitId == context.UserId).ToArray())
                Statuses.Remove(context.State, unit.Id, status.Id, StatusEndCause.Expired);
    }
}

/// <summary>Base of the Sorcerer's actives: `magical`, direct, distance 3, one unit of either side.</summary>
public abstract class SorcererSkill : ProfessionSkill
{
    protected SorcererSkill(ProfessionSkillDef def) : base(def) { }
    public override bool Magical => true;
    public override EffectTags Tags => EffectTags.Magical;
    public override TargetKind Target => TargetKind.Unit;
    public override Delivery? Delivery => Battle.Delivery.Direct;
    public override Reach Reach => Reach.Distance(3);
}

// ------------------------------------------------------------------------------------------------ Enchantment

/// <summary>Echo Enchant: a unit's next skill happens twice: the second use is free and hits the same target.</summary>
public sealed class EchoEnchant : SorcererSkill
{
    public EchoEnchant(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) => SorcererSkills.Enchant(context, StatusIds.Echo);
}

/// <summary>Phase Weapon: a unit's next attack has unlimited range. With Phasing Strike it also cannot be intercepted.</summary>
public sealed class PhaseWeapon : SorcererSkill
{
    public PhaseWeapon(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) =>
        SorcererSkills.Enchant(context, StatusIds.Phased, context.User.KnownSkills.Contains("phasing_strike") ? 1 : 0);
}

/// <summary>Phasing Strike (modifier of Phase Weapon): the enchanted attack also cannot be intercepted.</summary>
public sealed class PhasingStrike : ModifierSkill
{
    public PhasingStrike(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Piercing Enchant: a unit's next attack also hits the unit behind its target.</summary>
public sealed class PiercingEnchant : SorcererSkill
{
    public PiercingEnchant(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) => SorcererSkills.Enchant(context, StatusIds.Piercing);
}

/// <summary>Enchanted Soul (keystone): the Sorcerer's enchantments last until used, but only one can be active at a time. Read by the enchanting skills.</summary>
public sealed class EnchantedSoul : PassiveSkill
{
    public EnchantedSoul(ProfessionSkillDef def) : base(def) { }
}

// ------------------------------------------------------------------------------------------------ Transmutation

/// <summary>Petrify: a unit turns to stone: it is Stunned for 10 time units.</summary>
public sealed class Petrify : SorcererSkill
{
    public const int StunLength = 10;
    public Petrify(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(context.TargetId!.Value, StatusIds.Stunned, StunLength));
}

/// <summary>Transmute: a unit's Power and Speed swap values for 20 time units (40 with Lasting Transmute).</summary>
public sealed class Transmute : SorcererSkill
{
    public const int Length = 20;
    public const int LastingLength = 40;
    public Transmute(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(context.TargetId!.Value, StatusIds.Transmuted,
            context.User.KnownSkills.Contains("lasting_transmute") ? LastingLength : Length));
}

/// <summary>Stone Shell: a unit turns to stone for 10 time units: it cannot act and cannot be damaged. A status that can be cleansed.</summary>
public sealed class StoneShell : SorcererSkill
{
    public const int Length = 10;
    public StoneShell(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(context.TargetId!.Value, StatusIds.StoneShell, Length));
}

/// <summary>Lasting Transmute (modifier of Transmute): Transmute lasts 40 time units. Read by Transmute.</summary>
public sealed class LastingTransmute : ModifierSkill
{
    public LastingTransmute(ProfessionSkillDef def) : base(def) { }
}

/// <summary>Glass Body: the next hit on a unit deals double damage.</summary>
public sealed class GlassBody : SorcererSkill
{
    public GlassBody(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) =>
        context.ApplyStatus(context.TargetId!.Value, new StatusApplication(context.TargetId!.Value, StatusIds.GlassBody));
}

// ------------------------------------------------------------------------------------------------ Malediction

/// <summary>Curse of Stagnation: for 20 time units the timed statuses on a unit do not run down.</summary>
public sealed class CurseOfStagnation : SorcererSkill
{
    public CurseOfStagnation(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) => SorcererSkills.Curse(context, StatusIds.Stagnant);
}

/// <summary>Curse of Betrayal: for 20 time units the healing a unit would receive goes to the Sorcerer's unit with the lowest HP.</summary>
public sealed class CurseOfBetrayal : SorcererSkill
{
    public CurseOfBetrayal(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) => SorcererSkills.Curse(context, StatusIds.Betrayed);
}

/// <summary>Curse of Weight: a unit cannot Move or become airborne for 20 time units.</summary>
public sealed class CurseOfWeight : SorcererSkill
{
    public CurseOfWeight(ProfessionSkillDef def) : base(def) { }
    public override void Resolve(SkillContext context) => SorcererSkills.Curse(context, StatusIds.Weighted);
}

/// <summary>Lingering Malice: when an enemy removes one of the Sorcerer's curses, the remover is Silenced for 10 time units.</summary>
public sealed class LingeringMalice : PassiveSkill
{
    public const int SilenceLength = 10;
    public LingeringMalice(ProfessionSkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStatusRemovedHook((st, unitId, status, byUnitId) =>
        {
            var owner = st.GetUnit(ownerId);
            var remover = st.GetUnit(byUnitId);
            if (owner.IsKnockedOut || remover.IsKnockedOut || remover.IsRemoved) return;
            if (!StatusIds.IsCurse(status.Id) || status.SourceUnitId != ownerId || remover.Side == owner.Side) return;
            Statuses.Apply(st, new StatusApplication(byUnitId, StatusIds.Silenced, SilenceLength, SourceUnitId: ownerId));
        });
}

/// <summary>Witch's Patience (keystone): the Sorcerer's curses last until removed, but it can keep only one curse at a time. Read by the curses.</summary>
public sealed class WitchsPatience : PassiveSkill
{
    public WitchsPatience(ProfessionSkillDef def) : base(def) { }
}
