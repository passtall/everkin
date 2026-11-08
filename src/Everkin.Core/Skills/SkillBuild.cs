using Everkin.Core.Battle;
using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// The skill trees of all professions (skills.md, game_system.md section 4.1). Each profession's skill list is its tree; each
/// branch is a real tree: its roots have no parent and every other node has exactly one parent in the same branch.
/// </summary>
public sealed class SkillTrees
{
    private readonly Dictionary<string, ProfessionSkillDef> _byId;
    private readonly Dictionary<string, List<string>> _children = new();
    private readonly Dictionary<string, int> _order = new();

    public SkillTrees(ContentSet content)
    {
        Content = content;
        _byId = content.ProfessionSkills.ToDictionary(s => s.Id);
        var i = 0;
        foreach (var s in content.ProfessionSkills)
        {
            _order[s.Id] = i++;
            if (s.Parent is { } parent)
            {
                if (!_children.TryGetValue(parent, out var list)) _children[parent] = list = new List<string>();
                list.Add(s.Id);
            }
        }
    }

    public ContentSet Content { get; }

    public ProfessionSkillDef Node(string id) =>
        _byId.TryGetValue(id, out var n) ? n : throw new ArgumentException($"Unknown skill '{id}'.", nameof(id));

    public bool Exists(string id) => _byId.ContainsKey(id);

    /// <summary>Every node of the given professions' trees, in the order of skills.md.</summary>
    public IReadOnlyList<ProfessionSkillDef> NodesOf(IEnumerable<string> professionIds)
    {
        var set = professionIds.ToHashSet();
        return Content.ProfessionSkills.Where(s => set.Contains(s.Profession)).ToArray();
    }

    /// <summary>The nodes that name this node as their parent.</summary>
    public IReadOnlyList<string> Children(string id) =>
        _children.TryGetValue(id, out var list) ? list : Array.Empty<string>();

    /// <summary>Every node below this one in its tree, nearest first.</summary>
    public IReadOnlyList<string> Descendants(string id)
    {
        var result = new List<string>();
        var queue = new Queue<string>(Children(id));
        while (queue.Count > 0)
        {
            var next = queue.Dequeue();
            result.Add(next);
            foreach (var c in Children(next)) queue.Enqueue(c);
        }
        return result;
    }

    /// <summary>A node's place in skills.md, to order lists the same way.</summary>
    public int OrderOf(string id) => _order[id];

    /// <summary>The one shared attack of an Attack Type: it costs no skill point (section 4.4).</summary>
    public string AttackSkillId(string attackTypeId) =>
        Content.AttackSkills.Single(a => a.AttackType == attackTypeId).Id;
}

/// <summary>
/// What a unit has learned in its trees (game_system.md section 4.1). A unit has 8 skill points to spend across the trees of
/// its one or two professions; every node costs 1; a node can be learned only once its parent is learned; a creature can
/// learn in every branch of its professions. Unlearning a node also unlearns every node below it. The Attack Type attack is
/// free and not part of the build.
/// </summary>
public sealed class SkillBuild
{
    public const int Points = 8;

    private readonly SkillTrees _trees;
    private readonly IReadOnlyList<string> _professions;
    private readonly List<string> _learned = new();

    public SkillBuild(SkillTrees trees, IReadOnlyList<string> professions, IEnumerable<string>? learned = null)
    {
        _trees = trees;
        _professions = professions;
        if (learned is null) return;
        foreach (var id in learned)
        {
            var why = WhyNot(id);
            if (why is not null) throw new ArgumentException($"{id}: {why}", nameof(learned));
            _learned.Add(id);
        }
    }

    /// <summary>The learned skills, in the order they were learned.</summary>
    public IReadOnlyList<string> Learned => _learned;

    public int PointsSpent => _learned.Count;

    public int PointsLeft => Points - _learned.Count;

    public bool Has(string skillId) => _learned.Contains(skillId);

    /// <summary>
    /// Why a node cannot be learned now, or null if it can: unknown skill, not in the unit's professions, already learned,
    /// no points left, or its parent is not learned yet.
    /// </summary>
    public string? WhyNot(string skillId)
    {
        if (!_trees.Exists(skillId)) return "unknown skill";
        var node = _trees.Node(skillId);
        if (!_professions.Contains(node.Profession)) return $"not a skill of this unit's professions ({node.Profession})";
        if (Has(skillId)) return "already learned";
        if (PointsLeft <= 0) return "no skill points left";
        if (node.Parent is { } parent && !Has(parent)) return $"needs {parent} first";
        return null;
    }

    public bool CanLearn(string skillId) => WhyNot(skillId) is null;

    /// <summary>Learns a node for 1 point. Throws if it is not allowed (see <see cref="WhyNot"/>).</summary>
    public void Learn(string skillId)
    {
        var why = WhyNot(skillId);
        if (why is not null) throw new InvalidOperationException($"Cannot learn {skillId}: {why}.");
        _learned.Add(skillId);
    }

    /// <summary>Unlearns a node and every node below it in its tree, giving the points back. Returns what was removed.</summary>
    public IReadOnlyList<string> Unlearn(string skillId)
    {
        if (!Has(skillId)) return Array.Empty<string>();
        var removed = new List<string> { skillId };
        removed.AddRange(_trees.Descendants(skillId).Where(Has));
        _learned.RemoveAll(removed.Contains);
        return removed;
    }

    /// <summary>Respec: forget everything. Free outside battle.</summary>
    public void Reset() => _learned.Clear();

    /// <summary>Every node that can be learned right now, in the order of skills.md.</summary>
    public IReadOnlyList<string> Learnable() =>
        _trees.NodesOf(_professions).Where(n => CanLearn(n.Id)).Select(n => n.Id).ToArray();

    public SkillBuild Clone() => new(_trees, _professions, _learned);

    /// <summary>Whether a list of skills is a legal build for a unit with these professions. Returns the problem, or null.</summary>
    public static string? Validate(SkillTrees trees, IReadOnlyList<string> professions, IReadOnlyList<string> skills)
    {
        if (skills.Count > Points) return $"{skills.Count} skills is more than the {Points} points";
        if (skills.Distinct().Count() != skills.Count) return "a skill is listed twice";
        var build = new SkillBuild(trees, professions);
        // Learn in an order that respects parents, whatever order the list is in.
        var left = skills.ToList();
        while (left.Count > 0)
        {
            var next = left.FirstOrDefault(id => build.CanLearn(id));
            if (next is null)
            {
                var stuck = left[0];
                return $"{stuck}: {build.WhyNot(stuck) ?? "cannot be learned"}";
            }
            build.Learn(next);
            left.Remove(next);
        }
        return null;
    }
}

/// <summary>Random legal builds (game_system.md section 4.1).</summary>
public static class SkillBuilds
{
    /// <summary>
    /// A random legal build: the 8 points are assigned one at a time, each to a node chosen uniformly among those that can be
    /// learned at that moment. The generator is the caller's (the batch runner derives it from the battle's seed, so the build
    /// is reproducible and can be recorded with the battle).
    /// </summary>
    public static SkillBuild Random(SkillTrees trees, IReadOnlyList<string> professions, BattleRandom random)
    {
        var build = new SkillBuild(trees, professions);
        while (build.PointsLeft > 0)
        {
            var options = build.Learnable();
            if (options.Count == 0) break; // the trees are smaller than the budget: nothing more to learn
            build.Learn(options[random.NextInt(options.Count)]);
        }
        return build;
    }
}

/// <summary>
/// Default builds: each creature has one, made once with the random-build generator and stored with its content
/// (content/default_builds.json). A newly unlocked creature arrives with it and the player can respec freely.
/// </summary>
public static class DefaultBuilds
{
    /// <summary>The master seed the stored default builds were made with.</summary>
    public const ulong MasterSeed = 20261108;

    /// <summary>
    /// A creature's default build from the master seed and its ID. Stable: the same creature always gets the same build,
    /// whatever else is in the roster.
    /// </summary>
    public static IReadOnlyList<string> For(SkillTrees trees, CreatureDef creature, ulong masterSeed = MasterSeed) =>
        SkillBuilds.Random(trees, creature.Professions, new BattleRandom(masterSeed ^ Fnv1a(creature.Id))).Learned;

    /// <summary>
    /// The default builds for a whole roster, keeping every build already stored and making only the missing ones, so a
    /// build is never re-rolled once it exists.
    /// </summary>
    public static IReadOnlyDictionary<string, IReadOnlyList<string>> Complete(
        SkillTrees trees, IReadOnlyDictionary<string, IReadOnlyList<string>>? existing, ulong masterSeed = MasterSeed)
    {
        var result = new SortedDictionary<string, IReadOnlyList<string>>(StringComparer.Ordinal);
        foreach (var c in trees.Content.Creatures)
            result[c.Id] = existing is not null && existing.TryGetValue(c.Id, out var kept) ? kept : For(trees, c, masterSeed);
        return result;
    }

    private static ulong Fnv1a(string text)
    {
        var hash = 14695981039346656037UL;
        foreach (var ch in text)
        {
            hash ^= ch;
            hash *= 1099511628211UL;
        }
        return hash;
    }
}
