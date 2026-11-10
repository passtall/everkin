using System.Collections.Generic;
using System.Linq;
using Everkin.Core.Battle;

namespace Everkin.App;

/// <summary>Builds the card a unit shows in a battle.</summary>
public static class UnitCards
{
    private static readonly Dictionary<string, string> SummonArt = new()
    {
        ["clay_golem"] = "clay-golem", ["rune_golem"] = "rune-golem", ["clockwork_golem"] = "clockwork-golem", ["bell_golem"] = "bell-golem",
        ["animate_blades_unit"] = "animated-blades", ["animate_mirror_unit"] = "animated-mirror", ["bolt_turret"] = "bolt-turret",
        ["flame_turret"] = "flame-turret", ["barricade"] = "barricade", ["healing_totem"] = "healing-totem", ["spirit_link_totem"] = "spirit-link-totem",
        ["totem_of_return"] = "totem-of-return", ["net_trap"] = "net-trap", ["decoy"] = "hunter-decoy",
    };

    public static CardInfo Of(BattleState state, Unit u)
    {
        var def = u.CreatureId is { } id && Game.Content.CreaturesById.TryGetValue(id, out var d) ? d : null;
        CardInfo info;
        if (def != null && (u.Kind == UnitKind.Creature))
        {
            info = CardInfo.FromCreature(def);
            info.Undead = u.IsUndead;
        }
        else
        {
            var summoner = u.SummonerId is { } sid ? state.GetUnit(sid) : null;
            var home = summoner?.CreatureId is { } hid && Game.Content.CreaturesById.TryGetValue(hid, out var hd) ? hd.Biome : "ruins";
            info = new CardInfo
            {
                Name = u.Name,
                ClassLine = u.Kind switch { UnitKind.Construct => "Construct", UnitKind.Temporary => "Temporary", _ => "Summon" },
                Biome = home,
                ArtPath = u.CreatureId != null && SummonArt.TryGetValue(u.CreatureId, out var art) ? $"res://assets/summons/{art}.png" : null,
                ShowCost = false,
                Summon = true,
            };
        }
        info.Seed = u.Id * 7919 + 13;
        Update(state, u, info);
        return info;
    }

    /// <summary>Copies what changes during a battle (stats, HP, statuses, KO, stealth, skills) from the unit.</summary>
    public static void Update(BattleState state, Unit u, CardInfo info)
    {
        info.Hp = u.Hp;
        info.MaxHp = u.MaxHp;
        info.Speed = u.EffectiveSpeed;
        info.Power = state.PowerOf(u.Id);
        info.BaseSpeed = u.Speed;
        info.BasePower = u.IsSummoned ? info.Power : u.Power;
        info.Ko = u.IsKnockedOut;
        info.Stealthed = u.IsStealthed;
        info.Undead = u.IsUndead;
        var statuses = u.StatusInstances.Keys.Where(k => k is not (StatusIds.SongHaste or StatusIds.FreeMomentum)).ToList();
        if (u.Ward > 0 && !statuses.Contains(StatusIds.Ward)) statuses.Add(StatusIds.Ward);
        info.Statuses = statuses;
        info.Skills = Game.CardSkills(u.KnownSkills.Where(id => Game.Content.AttackSkills.Any(a => a.Id == id) || Game.Content.ProfessionSkills.Any(p => p.Id == id)));
    }
}
