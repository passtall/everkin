namespace Everkin.Core.Battle;

public enum BattleOutcome
{
    Ongoing,
    SideAWins,
    SideBWins,
    Draw,
}

public enum EndReason
{
    /// <summary>One side's original members are all KO.</summary>
    AllOriginalsKo,

    /// <summary>Both sides' last original members fell in the same block (or the reactions that followed it).</summary>
    Simultaneous,

    Surrender,

    /// <summary>The AI-versus-AI harness's technical cap on battle length (section 2.5).</summary>
    TurnCap,
}

/// <summary>The battle is decided. Pending events and remaining hits are cancelled (section 12.3).</summary>
public sealed record BattleEnded(BattleOutcome Outcome, EndReason Reason) : BattleEvent;

/// <summary>How a result reads for one player.</summary>
public enum PlayerResult
{
    Victory,
    Defeat,
    Draw,
}

/// <summary>
/// Battle end (game_system.md section 12.3). A side loses when all its original members are KO at once: summons never count,
/// a revived original counts as active again, and an original raised as undead still counts as KO for its owner. If both
/// sides' last originals are down together, the battle is a draw.
/// </summary>
public static class BattleEnd
{
    /// <summary>The harness stops a battle after 500 unit turns and records a draw (section 2.5).</summary>
    public const int HarnessTurnCap = 500;

    /// <summary>
    /// An original member that still counts for its owner: a party creature that is not KO and not raised as undead.
    /// Summons, constructs and totems never count (section 11).
    /// </summary>
    public static bool IsActiveOriginal(Unit unit) =>
        unit.Kind == UnitKind.Creature && !unit.IsRemoved && !unit.IsKnockedOut && !unit.IsUndead;

    /// <summary>A side is defeated when none of its original members is active.</summary>
    public static bool IsDefeated(BattleState state, Side side) =>
        !state.Units.Any(u => u.OriginalSide == side && IsActiveOriginal(u));

    /// <summary>What the board says right now, without deciding anything.</summary>
    public static BattleOutcome Check(BattleState state)
    {
        var a = IsDefeated(state, Side.A);
        var b = IsDefeated(state, Side.B);
        if (a && b) return BattleOutcome.Draw;
        if (a) return BattleOutcome.SideBWins;
        if (b) return BattleOutcome.SideAWins;
        return BattleOutcome.Ongoing;
    }

    /// <summary>
    /// The victory check: call it after each event's KO check and after that event's reactions have fully resolved (a
    /// counter can still KO the attacker and turn a win into a draw). If the result is decided, it is stored and reported,
    /// and the caller drops pending events and remaining hits. Returns true when the battle ended now.
    /// </summary>
    public static bool Resolve(BattleState state)
    {
        if (state.Outcome != BattleOutcome.Ongoing) return false;
        var outcome = Check(state);
        if (outcome == BattleOutcome.Ongoing) return false;
        Decide(state, outcome, outcome == BattleOutcome.Draw ? EndReason.Simultaneous : EndReason.AllOriginalsKo);
        return true;
    }

    /// <summary>A player can surrender at any time: a defeat with no other penalty.</summary>
    public static bool Surrender(BattleState state, Side loser)
    {
        if (state.Outcome != BattleOutcome.Ongoing) return false;
        Decide(state, loser == Side.A ? BattleOutcome.SideBWins : BattleOutcome.SideAWins, EndReason.Surrender);
        return true;
    }

    /// <summary>The harness's cap on battle length: a draw. Returns true when the cap ended the battle now.</summary>
    public static bool CheckTurnCap(BattleState state, int cap = HarnessTurnCap)
    {
        if (state.Outcome != BattleOutcome.Ongoing || state.TurnsTaken < cap) return false;
        Decide(state, BattleOutcome.Draw, EndReason.TurnCap);
        return true;
    }

    /// <summary>
    /// A result as one player sees it. In single-player modes a draw counts as a defeat for the player; in local two-player
    /// and AI-versus-AI it is a shared draw (section 12.3).
    /// </summary>
    public static PlayerResult ResultFor(BattleOutcome outcome, Side player, bool drawCountsAsDefeat)
    {
        if (outcome == BattleOutcome.Ongoing) throw new InvalidOperationException("The battle is not over.");
        if (outcome == BattleOutcome.Draw) return drawCountsAsDefeat ? PlayerResult.Defeat : PlayerResult.Draw;
        var won = (outcome == BattleOutcome.SideAWins) == (player == Side.A);
        return won ? PlayerResult.Victory : PlayerResult.Defeat;
    }

    private static void Decide(BattleState state, BattleOutcome outcome, EndReason reason)
    {
        state.SetOutcome(outcome, reason);
        state.Emit(new BattleEnded(outcome, reason));
    }
}
