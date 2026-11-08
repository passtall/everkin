using System.Text.Json;
using System.Text.Json.Serialization;

namespace Everkin.Core.Content;

/// <summary>Loads and validates classes.json, skills.json and creatures.json (production.md sections 2.1, 2.2).</summary>
public static class ContentLoader
{
    public const string ClassesFile = "classes.json";
    public const string SkillsFile = "skills.json";
    public const string CreaturesFile = "creatures.json";
    public const string StatusesFile = "statuses.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
    };

    public static ContentSet LoadFromDirectory(string directory) => LoadFromJson(
        File.ReadAllText(Path.Combine(directory, ClassesFile)),
        File.ReadAllText(Path.Combine(directory, SkillsFile)),
        File.ReadAllText(Path.Combine(directory, CreaturesFile)),
        File.ReadAllText(Path.Combine(directory, StatusesFile)));

    public static ContentSet LoadFromJson(string classesJson, string skillsJson, string creaturesJson, string statusesJson)
    {
        var classes = Parse<ClassesDoc>(ClassesFile, classesJson);
        var skills = Parse<SkillsDoc>(SkillsFile, skillsJson);
        var creatures = Parse<CreaturesDoc>(CreaturesFile, creaturesJson);
        var statuses = Parse<StatusesDoc>(StatusesFile, statusesJson);

        var set = new ContentSet
        {
            AttackTypes = classes.AttackTypes,
            Professions = classes.Professions,
            AttackSkills = skills.AttackSkills,
            ProfessionSkills = skills.ProfessionSkills,
            Creatures = creatures.Creatures,
            Statuses = statuses.Statuses,
        };
        Validate(set);
        return new ContentSet
        {
            AttackTypes = set.AttackTypes,
            Professions = set.Professions,
            AttackSkills = set.AttackSkills,
            ProfessionSkills = set.ProfessionSkills,
            Creatures = set.Creatures,
            Statuses = set.Statuses,
            CreaturesById = set.Creatures.ToDictionary(c => c.Id),
            SkillsById = set.ProfessionSkills.ToDictionary(s => s.Id),
        };
    }

    private static T Parse<T>(string file, string json) where T : class
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json, Options)
                ?? throw new ContentException(file, "(file)", "empty document");
        }
        catch (JsonException e)
        {
            throw new ContentException(file, "(file)", e.Message);
        }
    }

    private static void Validate(ContentSet set)
    {
        var attackTypes = Unique(ClassesFile, set.AttackTypes.Select(a => a.Id));
        var professions = Unique(ClassesFile, set.Professions.Select(p => p.Id));

        var skillIds = new HashSet<string>();
        foreach (var a in set.AttackSkills)
        {
            if (!skillIds.Add(a.Id)) throw new ContentException(SkillsFile, a.Id, "duplicate skill ID");
            if (!attackTypes.Contains(a.AttackType))
                throw new ContentException(SkillsFile, a.Id, $"unknown attack type '{a.AttackType}'");
        }

        var skills = new Dictionary<string, ProfessionSkillDef>();
        foreach (var s in set.ProfessionSkills)
        {
            if (!skillIds.Add(s.Id)) throw new ContentException(SkillsFile, s.Id, "duplicate skill ID");
            skills[s.Id] = s;
            var prof = set.Professions.FirstOrDefault(p => p.Id == s.Profession)
                ?? throw new ContentException(SkillsFile, s.Id, $"unknown profession '{s.Profession}'");
            if (!prof.Branches.Contains(s.Branch))
                throw new ContentException(SkillsFile, s.Id, $"branch '{s.Branch}' is not a branch of {s.Profession}");
            if (s.Kind is not ("A" or "P" or "K" or "Mod"))
                throw new ContentException(SkillsFile, s.Id, $"unknown kind '{s.Kind}'");
            if (s.Kind == "A" && (s.Delay is null || s.Momentum is null))
                throw new ContentException(SkillsFile, s.Id, "active skill without Delay and Momentum");
            if (s.Kind == "Mod" && s.ModifierOf is null)
                throw new ContentException(SkillsFile, s.Id, "modifier without ModifierOf");
        }

        foreach (var s in skills.Values)
        {
            foreach (var reference in new[] { s.Parent, s.ModifierOf })
            {
                if (reference is null) continue;
                if (!skills.TryGetValue(reference, out var target) || target.Profession != s.Profession)
                    throw new ContentException(SkillsFile, s.Id, $"'{reference}' is not a skill of the same profession");
            }

            // One parent per node; the chain stays in the branch and ends at a starting skill (game_system section 4.1).
            var seen = new HashSet<string>();
            var cur = s;
            while (cur.Parent is not null)
            {
                if (!seen.Add(cur.Id)) throw new ContentException(SkillsFile, s.Id, "parent cycle");
                cur = skills[cur.Parent];
                if (cur.Branch != s.Branch)
                    throw new ContentException(SkillsFile, s.Id, $"parent '{cur.Id}' is in another branch");
            }
        }

        Unique(StatusesFile, set.Statuses.Select(st => st.Id));
        Unique(CreaturesFile, set.Creatures.Select(c => c.Id));
        foreach (var c in set.Creatures)
        {
            if (!attackTypes.Contains(c.AttackType))
                throw new ContentException(CreaturesFile, c.Id, $"unknown attack type '{c.AttackType}'");
            if (c.Professions.Count is < 1 or > 2 || c.Professions.Distinct().Count() != c.Professions.Count)
                throw new ContentException(CreaturesFile, c.Id, "a creature has one or two different professions");
            foreach (var p in c.Professions)
                if (!professions.Contains(p))
                    throw new ContentException(CreaturesFile, c.Id, $"unknown profession '{p}'");
            if (c.Hp <= 0 || c.Speed <= 0 || c.Power <= 0)
                throw new ContentException(CreaturesFile, c.Id, "HP, Speed and Power must be positive");
            if (c.Cost is < 1 or > 10)
                throw new ContentException(CreaturesFile, c.Id, $"Cost {c.Cost} outside 1 to 10");
            if (c.FrontFrame.Count != 4 || c.RearFrame.Count != 4)
                throw new ContentException(CreaturesFile, c.Id, "frames need four numbers");
        }
    }

    private static HashSet<string> Unique(string file, IEnumerable<string> ids)
    {
        var set = new HashSet<string>();
        foreach (var id in ids)
            if (!set.Add(id)) throw new ContentException(file, id, "duplicate ID");
        return set;
    }

    private sealed record ClassesDoc(IReadOnlyList<AttackTypeDef> AttackTypes, IReadOnlyList<ProfessionDef> Professions);

    private sealed record SkillsDoc(IReadOnlyList<AttackSkillDef> AttackSkills, IReadOnlyList<ProfessionSkillDef> ProfessionSkills);

    private sealed record CreaturesDoc(IReadOnlyList<CreatureDef> Creatures);

    private sealed record StatusesDoc(IReadOnlyList<StatusDef> Statuses);
}
