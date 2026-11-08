using Everkin.Core.Battle;

namespace Everkin.Core.Skills;

/// <summary>
/// Battle start (game_system.md section 6.7: every battle starts clean). Gives every unit that knows a passive the chance to set
/// it up: reactions and hooks are registered, "at battle start" effects happen. Call it once, after the units, their skills and
/// the formation are in place and before the first turn.
/// </summary>
public static class BattleStart
{
    public static void Install(BattleState state, SkillCatalog catalog)
    {
        foreach (var unit in state.Units.ToArray())
        {
            if (unit.IsKnockedOut || unit.IsRemoved) continue;
            foreach (var id in unit.KnownSkills)
                if (catalog.TryGet(id, out var skill)) skill.Install(state, unit.Id);
        }
    }
}
