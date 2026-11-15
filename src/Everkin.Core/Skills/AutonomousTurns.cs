using Everkin.Core.Battle;

namespace Everkin.Core.Skills;

/// <summary>Plays the turns of autonomous summons and undead (game_system.md section 11): the nearest legal enemy, or Skip Turn.</summary>
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
            target = Targeting.PickNearest(state, unitId, skill.Options with { Delivery = skill.Delivery, Side = TargetSide.EnemySideOnly });
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
