using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>The Enchanter (skills.md): enchants an ally's next attack or skill. Magical.</summary>
public static class EnchanterSkills
{
    /// <summary>An unused enchantment fades after 20 time units; any number can be active together.</summary>
    public const int EnchantmentLength = 20;

    public static IEnumerable<Skill> All(ContentSet content)
    {
        SkillDef D(string id) => content.SkillsById[id];
        yield return new EchoEnchant(D("echo_enchant"));
        yield return new PhaseWeapon(D("phase_weapon"));
        yield return new PiercingEnchant(D("piercing_enchant"));
        yield return new TwinEnchant(D("twin_enchant"));
        yield return new LingeringMagic(D("lingering_magic"));
        yield return new SpellWard(D("spell_ward"));
    }

    /// <summary>
    /// Puts an enchantment (a helpful status) on a unit for 20 time units, with two uses under Lingering Magic, and with Twin Enchant also on one ally
    /// next to the target, picked by the seeded roll.
    /// </summary>
    internal static void Enchant(SkillContext context, string statusId)
    {
        var state = context.State;
        var target = context.TargetId!.Value;
        int? uses = context.User.KnownSkills.Contains("lingering_magic") ? 2 : null;
        void Put(int id) => Statuses.Apply(state, new StatusApplication(id, statusId, EnchantmentLength, SourceUnitId: context.UserId, Uses: uses));
        Put(target);
        if (context.User.KnownSkills.Contains("twin_enchant"))
        {
            var allies = state.Formation.Adjacent(target)
                .Where(id => state.GetUnit(id).Side == context.UserSide && !state.GetUnit(id).IsKnockedOut).ToList();
            if (allies.Count > 0) Put(ClassHelpers.Pick(state, allies));
        }
        context.Settle();
    }

    /// <summary>The unit carries an enchantment from this Enchanter.</summary>
    internal static bool Carries(BattleState state, int unitId, int enchanterId) =>
        state.GetUnit(unitId).StatusInstances.Values.Any(s => StatusIds.IsEnchantment(s.Id) && s.SourceUnitId == enchanterId);
}

/// <summary>Echo Enchant: a unit's next skill happens twice: the second use is free (no Delay, no Momentum) and hits the same target.</summary>
public sealed class EchoEnchant : ClassSkill
{
    public EchoEnchant(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override void Resolve(SkillContext context) => EnchanterSkills.Enchant(context, StatusIds.Echo);
}

/// <summary>Phase Weapon: a unit's next attack cannot be intercepted.</summary>
public sealed class PhaseWeapon : ClassSkill
{
    public PhaseWeapon(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override void Resolve(SkillContext context) => EnchanterSkills.Enchant(context, StatusIds.Phased);
}

/// <summary>Piercing Enchant: a unit's next attack also hits the unit behind its target.</summary>
public sealed class PiercingEnchant : ClassSkill
{
    public PiercingEnchant(SkillDef def) : base(def, magical: true) { }
    public override TargetKind Target => TargetKind.Unit;
    public override void Resolve(SkillContext context) => EnchanterSkills.Enchant(context, StatusIds.Piercing);
}

/// <summary>Twin Enchant: each enchantment also goes on one ally next to the target. The enchanting code reads this keystone.</summary>
public sealed class TwinEnchant : KeystoneSkill
{
    public TwinEnchant(SkillDef def) : base(def) { }
}

/// <summary>Lingering Magic: each enchantment from the Enchanter lasts for two uses instead of one. The enchanting code reads this keystone.</summary>
public sealed class LingeringMagic : KeystoneSkill
{
    public LingeringMagic(SkillDef def) : base(def) { }
}

/// <summary>Spell Ward: a unit carrying an enchantment from the Enchanter cannot be Silenced or Stunned.</summary>
public sealed class SpellWard : KeystoneSkill
{
    public SpellWard(SkillDef def) : base(def) { }

    public override void Install(BattleState state, int ownerId) =>
        state.AddStatusImmunity((st, unitId, statusId) =>
            statusId is StatusIds.Silenced or StatusIds.Stunned && !st.GetUnit(ownerId).IsKnockedOut && EnchanterSkills.Carries(st, unitId, ownerId));
}
