using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Setup;

/// <summary>What a creature is for in a team (derived from its unique attack and classes, never hand-tagged).</summary>
public enum TeamRole { Blocker, Frontliner, Ranged, Caster, Support, Summoner }

/// <summary>A built sample team with the facts the batch file records.</summary>
public sealed record BuiltTeam(string Anchor, string Template, TeamSetup Team, int Cost);

/// <summary>
/// Builds sensible sample teams for the AI-versus-AI harness (owner decisions 2026-11-17): every team is built around one anchor creature from a
/// template of role slots, filled to a Cost of 33-35, with melee in the Front row, ranged in the Rear, blockers centred in the Front row and
/// support only where it has a payoff. All draws come from the given generator, so a seed gives the same team.
/// </summary>
public static class TeamBuilder
{
    /// <summary>Bumped whenever the rules below change, so a batch file says which builder made it.</summary>
    public const int Version = 1;

    public const int MinCost = 33;

    /// <summary>Classes that do no damage of their own; a creature with two of them is pure support.</summary>
    public static readonly IReadOnlySet<string> SupportClasses = new HashSet<string>
    {
        "mender", "cleric", "lifebinder", "bard", "hexer", "saboteur", "commander", "tactician", "totemist", "enchanter"
    };

    private static readonly string[] Healers = { "mender", "cleric", "lifebinder" };

    private enum Slot { Blocker, Frontliner, Ranged, Caster, Support, Flex }

    /// <summary>The templates (slot lists). Support slots come last when a team is filled, so their payoff is known.</summary>
    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Templates = new Dictionary<string, IReadOnlyList<string>>
    {
        ["balanced"] = new[] { "Blocker", "Frontliner", "Ranged", "Ranged", "Support", "Flex" },
        ["wall"] = new[] { "Blocker", "Blocker", "Blocker", "Ranged", "Ranged", "Caster" },
        ["aggro"] = new[] { "Frontliner", "Frontliner", "Ranged", "Ranged", "Caster", "Flex" },
        ["support_heavy"] = new[] { "Blocker", "Frontliner", "Ranged", "Support", "Support", "Flex" },
    };

    public static TeamRole RoleOf(ContentSet content, CreatureDef c)
    {
        if (c.Classes.Contains("summoner")) return TeamRole.Summoner;
        if (c.Classes.Contains("bulwark") || c.Classes.Contains("keeper") || c.Hp >= 60) return TeamRole.Blocker;
        if (c.Classes.All(SupportClasses.Contains)) return TeamRole.Support;
        return content.AttacksById[c.Attack].Delivery switch
        {
            "melee" => TeamRole.Frontliner,
            "projectile" => TeamRole.Ranged,
            _ => TeamRole.Caster,
        };
    }

    public static Row RowOf(ContentSet content, CreatureDef c) => RoleOf(content, c) switch
    {
        TeamRole.Blocker or TeamRole.Frontliner => Row.Front,
        TeamRole.Ranged or TeamRole.Summoner => Row.Rear,
        _ => Row.Middle,
    };

    private static bool IsAttacker(TeamRole r) => r is TeamRole.Frontliner or TeamRole.Ranged or TeamRole.Caster or TeamRole.Summoner;

    private static bool Accepts(Slot slot, TeamRole role) => slot switch
    {
        Slot.Blocker => role == TeamRole.Blocker,
        Slot.Frontliner => role == TeamRole.Frontliner,
        Slot.Ranged => role == TeamRole.Ranged,
        Slot.Caster => role is TeamRole.Caster or TeamRole.Summoner,
        Slot.Support => role == TeamRole.Support,
        _ => true,
    };

    /// <summary>
    /// The payoff rules: at most one Commander; a healer class needs two durable frontliners (blockers, or melee units with 40 HP or more); a Bard
    /// needs two attackers; a Hexer or Saboteur needs three. Pure checks against the creatures already in the team.
    /// </summary>
    public static bool Allowed(ContentSet content, CreatureDef candidate, IReadOnlyList<CreatureDef> team)
    {
        if (team.Any(t => t.Id == candidate.Id)) return false;
        if (candidate.Classes.Contains("commander") && team.Any(t => t.Classes.Contains("commander"))) return false;
        var attackers = team.Count(t => IsAttacker(RoleOf(content, t)));
        var durable = team.Count(t => RoleOf(content, t) == TeamRole.Blocker || (RoleOf(content, t) == TeamRole.Frontliner && t.Hp >= 40));
        if (candidate.Classes.Any(c => Healers.Contains(c)) && durable < 2) return false;
        if (candidate.Classes.Contains("bard") && attackers < 2) return false;
        if ((candidate.Classes.Contains("hexer") || candidate.Classes.Contains("saboteur")) && attackers < 3) return false;
        return true;
    }

    /// <summary>
    /// A team built around <paramref name="anchor"/>: a template whose slots fit the anchor's role is drawn, the other slots are filled (attackers and
    /// blockers first, support last) within the payoff rules, and the Cost is brought to 33-35. If no attempt reaches 33 the nearest one is kept.
    /// </summary>
    public static BuiltTeam Build(ContentSet content, CreatureDef anchor, BattleRandom random, int budget = BattleSetup.TeamBudget)
    {
        var role = RoleOf(content, anchor);
        var names = new List<string> { "balanced", "wall", "aggro", "support_heavy" };
        for (var i = names.Count - 1; i > 0; i--) { var j = random.NextInt(i + 1); (names[i], names[j]) = (names[j], names[i]); }
        var fitting = names.Where(n => Templates[n].Select(Parse).Any(s => s != Slot.Flex && Accepts(s, role)) || Templates[n].Select(Parse).Contains(Slot.Flex)).ToList();
        var template = fitting.Count > 0 ? fitting[0] : names[0];
        var slots = Templates[template].Select(Parse).ToList();

        // The anchor takes its own slot (a matching specific one, else a Flex slot); the rest are filled in the order below.
        var anchorSlot = slots.FindIndex(s => s != Slot.Flex && Accepts(s, role));
        if (anchorSlot < 0) anchorSlot = slots.IndexOf(Slot.Flex);
        if (anchorSlot < 0) anchorSlot = 0;
        var order = new[] { Slot.Blocker, Slot.Frontliner, Slot.Ranged, Slot.Caster, Slot.Flex, Slot.Support };
        var open = Enumerable.Range(0, slots.Count).Where(i => i != anchorSlot).OrderBy(i => Array.IndexOf(order, slots[i])).ToList();

        var roster = content.Creatures;
        List<CreatureDef>? best = null;
        var bestCost = -1;
        for (var attempt = 0; attempt < 300; attempt++)
        {
            var team = new List<CreatureDef> { anchor };
            var spent = anchor.Cost;
            var failed = false;
            for (var n = 0; n < open.Count && !failed; n++)
            {
                var slot = slots[open[n]];
                var left = open.Count - n - 1;
                var remaining = budget - spent;
                var pool = roster.Where(c => Accepts(slot, RoleOf(content, c)) && c.Cost <= remaining - left && Allowed(content, c, team)).ToList();
                if (pool.Count == 0) { failed = true; break; }
                CreatureDef pick;
                if (left == 0)
                {
                    // The last slot closes the budget: the Cost nearest to what is left.
                    var nearest = pool.Min(c => remaining - c.Cost);
                    var close = pool.Where(c => remaining - c.Cost == nearest).ToList();
                    pick = close[random.NextInt(close.Count)];
                }
                else
                {
                    var average = (remaining + left) / (left + 1);
                    var near = pool.Where(c => Math.Abs(c.Cost - average) <= 2).ToList();
                    var from = near.Count > 0 ? near : pool;
                    pick = from[random.NextInt(from.Count)];
                }
                team.Add(pick);
                spent += pick.Cost;
            }
            if (failed || team.Count != slots.Count) continue;
            if (spent > bestCost) { best = team; bestCost = spent; }
            if (spent >= MinCost) break;
        }
        // The anchor alone costs too much for the template: fall back to what the anchor can carry.
        best ??= new List<CreatureDef> { anchor };
        return new BuiltTeam(anchor.Id, template, Place(content, best, random), best.Sum(c => c.Cost));
    }

    private static Slot Parse(string s) => Enum.Parse<Slot>(s);

    /// <summary>Rows by role; in the Front row the blockers stand in the middle of the row so they overlap the units behind them.</summary>
    private static TeamSetup Place(ContentSet content, List<CreatureDef> team, BattleRandom random)
    {
        var shuffled = team.ToList();
        for (var i = shuffled.Count - 1; i > 0; i--) { var j = random.NextInt(i + 1); (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]); }
        var units = new List<UnitSetup>();
        foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
        {
            var inRow = shuffled.Where(c => RowOf(content, c) == row).ToList();
            var blockers = inRow.Where(c => RoleOf(content, c) == TeamRole.Blocker).ToList();
            var others = inRow.Where(c => RoleOf(content, c) != TeamRole.Blocker).ToList();
            var half = others.Count / 2;
            foreach (var c in others.Take(half).Concat(blockers).Concat(others.Skip(half))) units.Add(new UnitSetup(c.Id, row));
        }
        return new TeamSetup(units);
    }
}
