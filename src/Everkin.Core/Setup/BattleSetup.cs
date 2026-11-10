using Everkin.Core.Battle;
using Everkin.Core.Content;
using Everkin.Core.Skills;

namespace Everkin.Core.Setup;

/// <summary>One creature of a team: which creature, its skill build (node IDs) and where it stands.</summary>
public sealed record UnitSetup(string CreatureId, IReadOnlyList<string> Build, Row Row, bool Boss = false);

/// <summary>A team: up to six units; within a row the units stand in the order they are listed.</summary>
public sealed record TeamSetup(IReadOnlyList<UnitSetup> Units)
{
    public int TotalCost(ContentSet content) => Units.Sum(u => content.CreaturesById[u.CreatureId].Cost);
}

/// <summary>Everything that makes a battle reproducible: the seed and both teams (production section 2.2, game_system section 2.5).</summary>
public sealed record BattleSpec(ulong Seed, TeamSetup SideA, TeamSetup SideB);

/// <summary>Builds a battle from a spec and draws random legal teams (game_system sections 2.5, 4.1, 5.1).</summary>
public static class BattleSetup
{
    public const int TeamSize = 6;
    public const int TeamBudget = 35;

    /// <summary>A boss is the strengthened version of its creature: HP × 1.5 rounded down to a multiple of 5, Power + 2 (campaign.md, starting values).</summary>
    public static int BossHp(int hp) => hp * 3 / 2 / 5 * 5;

    public const int BossPowerBonus = 2;

    /// <summary>
    /// A ready battle: the units of side A then side B with their Attack Type attack and build skills, the formation, every unit's first
    /// turn on the timeline (ties by the battle's seeded generator) and the passives installed. Call nothing else before the first turn.
    /// </summary>
    public static BattleState Create(BattleSpec spec, SkillCatalog catalog)
    {
        var content = catalog.Content;
        var state = new BattleState(spec.Seed);
        foreach (var (team, side) in new[] { (spec.SideA, Side.A), (spec.SideB, Side.B) })
        {
            var perRow = new Dictionary<Row, List<int>> { [Row.Front] = new(), [Row.Middle] = new(), [Row.Rear] = new() };
            foreach (var u in team.Units)
            {
                var def = content.CreaturesById[u.CreatureId];
                var unit = state.AddUnit(def, side);
                if (u.Boss)
                {
                    unit.MaxHp = unit.Hp = BossHp(def.Hp);
                    unit.Power = def.Power + BossPowerBonus;
                }
                unit.LearnSkill(catalog.AttackSkillFor(def.AttackType));
                foreach (var node in u.Build) unit.LearnSkill(node);
                perRow[u.Row].Add(unit.Id);
            }
            foreach (var row in new[] { Row.Front, Row.Middle, Row.Rear })
                foreach (var id in perRow[row]) state.Formation.Place(id, side, row);
        }
        state.Timeline.ScheduleInitialTurns(state.Units, state.Random);
        BattleStart.Install(state, catalog);
        state.DrainEvents();
        return state;
    }

    /// <summary>
    /// A random team of up to six different creatures whose total Cost fits the budget, each with a random legal build and the whole team
    /// in a random legal formation. All draws come from <paramref name="random"/>, so a seed gives the same team.
    /// </summary>
    public static TeamSetup RandomTeam(ContentSet content, SkillTrees trees, BattleRandom random, int budget = TeamBudget, int size = TeamSize)
    {
        var roster = content.Creatures;
        var picked = new List<CreatureDef>();
        var spent = 0;
        // Draw without replacement; a creature that does not fit is skipped. Retry from scratch if the team ended up short.
        for (var attempt = 0; attempt < 50; attempt++)
        {
            picked.Clear();
            spent = 0;
            var pool = roster.ToList();
            while (picked.Count < size && pool.Count > 0)
            {
                var i = random.NextInt(pool.Count);
                var c = pool[i];
                pool.RemoveAt(i);
                if (spent + c.Cost > budget) continue;
                picked.Add(c);
                spent += c.Cost;
            }
            if (picked.Count == size) break;
        }

        var rows = RandomRows(picked.Count, random);
        var units = new List<UnitSetup>();
        for (var i = 0; i < picked.Count; i++)
        {
            var build = SkillBuilds.Random(trees, picked[i].Professions, random);
            units.Add(new UnitSetup(picked[i].Id, build.Learned.ToArray(), rows[i]));
        }
        return new TeamSetup(units);
    }

    /// <summary>A random legal formation: each unit gets a row, at most six per row, and the order inside a row follows the unit order.</summary>
    private static IReadOnlyList<Row> RandomRows(int count, BattleRandom random)
    {
        var rows = new Row[count];
        var used = new int[Formation.RowCount];
        for (var i = 0; i < count; i++)
        {
            Row row;
            do { row = (Row)random.NextInt(Formation.RowCount); } while (used[(int)row] >= Formation.RowCapacity);
            used[(int)row]++;
            rows[i] = row;
        }
        return rows;
    }

    /// <summary>A random battle spec for a seed: both teams drawn from a generator derived from the seed (separate from the battle's own).</summary>
    public static BattleSpec RandomSpec(ContentSet content, SkillTrees trees, ulong seed)
    {
        var random = new BattleRandom(seed ^ 0x5EED5E7UL);
        return new BattleSpec(seed, RandomTeam(content, trees, random), RandomTeam(content, trees, random));
    }
}
