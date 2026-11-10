using Everkin.Core.Skills;

namespace Everkin.Core.Battle;

/// <summary>
/// Runs the timeline up to the next unit that must choose an action (game_system.md sections 6.1, 6.2). Time advances only in
/// jumps between entries. For every entry it pops it does what the rules require: effect ends (statuses, ground effects, expiring
/// summons) end; whenever any unit's turn arrives, Poison and ground damage land and the KO check runs; the unit's own turn start
/// ends its 'until its next turn' statuses and skips the turn if it is Frozen or Stunned.
/// </summary>
public static class BattleDriver
{
    /// <summary>
    /// Advances to the next unit that has to act and returns its ID, or null when the battle is over or the timeline is empty.
    /// The events of what happened on the way are in the state's event buffer (<see cref="BattleState.DrainEvents"/>).
    /// The caller then plays an action with <see cref="Actions"/>, whose scheduling gives the unit its next turn.
    /// </summary>
    public static int? NextTurn(BattleState state, Everkin.Core.Skills.SkillCatalog? catalog = null)
    {
        while (!state.IsOver)
        {
            var entry = state.Timeline.Pop();
            if (entry is null) return null;

            switch (entry.Kind)
            {
                case TimelineKind.EffectEnd:
                    Statuses.HandleExpiry(state, entry);
                    Actions.Settle(state);
                    break;

                case TimelineKind.UnitTurn:
                    if (entry.OwnerUnitId is not { } unitId) break;
                    var unit = state.GetUnit(unitId);
                    if (unit.IsKnockedOut || unit.IsRemoved) break;

                    // Whenever any unit gets a turn, what has built up lands (section 10.5, 12.2).
                    Statuses.TickPoison(state);
                    state.Ground.Tick();
                    Actions.Settle(state);
                    if (state.IsOver) return null;
                    if (unit.IsKnockedOut) break;   // a tick that KOs the unit ends its turn

                    state.CountTurn();
                    state.Emit(new TurnStarted(unitId, state.Timeline.Now));
                    var start = Statuses.OnTurnStart(state, unitId);
                    if (start.TurnSkipped)
                    {
                        state.Emit(new TurnSkipped(unitId));
                        state.Timeline.ScheduleTurnWithWait(unitId, Timeline.SkippedTurnWait(unit.EffectiveSpeed));
                        break;
                    }
                    if (catalog is not null && unit.AutoSkillId is not null)
                    {
                        Everkin.Core.Skills.AutonomousTurns.Play(state, catalog, unitId);      // summons and undead play themselves
                        break;
                    }
                    unit.SneakReady = true;
                    return unitId;

                case TimelineKind.AutoEvent:
                    Everkin.Core.Skills.Totems.HandleAutoEvent(state, entry);
                    break;

                default:
                    // Delayed events and delayed events are resolved by the skills that create them (later M1.16 tasks).
                    break;
            }
        }
        return null;
    }
}
