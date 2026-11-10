using Everkin.Core.Battle;
using Everkin.Core.Skills;

namespace Everkin.Core.Ai;

/// <summary>What a played battle came to.</summary>
public sealed record BattleResult(BattleOutcome Outcome, EndReason? Reason, int Turns, bool HitCap);

/// <summary>
/// Plays a whole battle between two AIs (game_system section 2.5): the driver hands out turns (summon and undead turns are played by the
/// core), the AI of the acting unit's side chooses, the action is applied to the real state with the battle's generator. A battle stops
/// after the length cap and is recorded as a draw.
/// </summary>
public static class BattlePlayer
{
    public static BattleResult Play(BattleState state, SkillCatalog catalog, AiSearch aiA, AiSearch aiB, Action<BattleEvent>? observer = null, int turnCap = BattleEnd.HarnessTurnCap)
    {
        var hitCap = false;
        void Observe()
        {
            var events = state.DrainEvents();
            if (observer is not null) foreach (var e in events) observer(e);
        }

        while (!state.IsOver)
        {
            var unitId = BattleDriver.NextTurn(state, catalog);
            Observe();
            if (state.IsOver || unitId is not { } id) break;
            var ai = state.GetUnit(id).Side == Side.A ? aiA : aiB;
            var action = ai.Choose(state, id);
            action.Apply(state, catalog, id);
            Observe();
            if (BattleEnd.CheckTurnCap(state, turnCap)) { hitCap = true; Observe(); }
        }
        return new BattleResult(state.Outcome, state.EndReason, state.TurnsTaken, hitCap);
    }
}
