using System.Text.Json;
using System.Text.Json.Serialization;

namespace Everkin.Core.Content;

/// <summary>Loads and validates classes.json, skills.json, creatures.json and statuses.json (production.md sections 2.1, 2.2).</summary>
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

    public static ContentSet LoadFromDirectory(string directory) =>
        LoadFromJson(
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

        Unique(CreaturesFile, creatures.Creatures.Select(c => c.Id));
        Unique(ClassesFile, classes.Classes.Select(c => c.Id));
        Unique(SkillsFile, skills.Skills.Select(s => s.Id));
        Unique(SkillsFile, skills.Attacks.Select(a => a.Id));
        var set = new ContentSet
        {
            Classes = classes.Classes,
            Skills = skills.Skills,
            Attacks = skills.Attacks,
            Creatures = creatures.Creatures,
            Statuses = statuses.Statuses,
            CreaturesById = creatures.Creatures.ToDictionary(c => c.Id),
            ClassesById = classes.Classes.ToDictionary(c => c.Id),
            SkillsById = skills.Skills.ToDictionary(s => s.Id),
            AttacksById = skills.Attacks.ToDictionary(a => a.Id),
        };
        Validate(set);
        return set;
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
        Unique(ClassesFile, set.Classes.Select(c => c.Id));
        var skillIds = Unique(SkillsFile, set.Skills.Select(s => s.Id));
        Unique(SkillsFile, set.Attacks.Select(a => a.Id));
        foreach (var a in set.Attacks)
            if (skillIds.Contains(a.Id)) throw new ContentException(SkillsFile, a.Id, "an attack shares its ID with a class skill");

        foreach (var s in set.Skills)
        {
            if (!set.ClassesById.TryGetValue(s.Class, out var cls))
                throw new ContentException(SkillsFile, s.Id, $"unknown class '{s.Class}'");
            if (s.Kind is not ("A" or "K"))
                throw new ContentException(SkillsFile, s.Id, $"unknown kind '{s.Kind}'");
            if (s.Kind == "A" && (s.Delay is null || s.Momentum is null || s.Delivery is null))
                throw new ContentException(SkillsFile, s.Id, "active skill without Delay, Momentum and delivery");
            var listed = s.Kind == "A" ? cls.Actives : cls.Keystones;
            if (!listed.Contains(s.Id))
                throw new ContentException(SkillsFile, s.Id, $"not listed in the {(s.Kind == "A" ? "actives" : "keystones")} of {s.Class}");
        }
        foreach (var c in set.Classes)
        {
            if (c.Actives.Count != 3 || c.Keystones.Count != 3)
                throw new ContentException(ClassesFile, c.Id, "a class has three actives and three keystones");
            foreach (var id in c.Actives.Concat(c.Keystones))
                if (!set.SkillsById.ContainsKey(id))
                    throw new ContentException(ClassesFile, c.Id, $"skill '{id}' is missing from {SkillsFile}");
        }
        foreach (var a in set.Attacks)
        {
            if (!set.CreaturesById.ContainsKey(a.Creature))
                throw new ContentException(SkillsFile, a.Id, $"unknown creature '{a.Creature}'");
            if (a.Delivery is not ("melee" or "projectile" or "direct") || a.Shape is not ("single" or "splash" or "pulse"))
                throw new ContentException(SkillsFile, a.Id, "unknown delivery or shape");
            if (a.Min < 1 || a.Max < a.Min) throw new ContentException(SkillsFile, a.Id, "bad damage range");
        }

        Unique(StatusesFile, set.Statuses.Select(st => st.Id));
        Unique(CreaturesFile, set.Creatures.Select(c => c.Id));
        foreach (var c in set.Creatures)
        {
            if (c.Classes.Count != 2 || c.Classes.Distinct().Count() != 2)
                throw new ContentException(CreaturesFile, c.Id, "a creature has two different classes");
            if (c.Keystones.Count != 2) throw new ContentException(CreaturesFile, c.Id, "a creature has two keystones");
            for (var i = 0; i < 2; i++)
            {
                if (!set.ClassesById.TryGetValue(c.Classes[i], out var cls))
                    throw new ContentException(CreaturesFile, c.Id, $"unknown class '{c.Classes[i]}'");
                if (!cls.Keystones.Contains(c.Keystones[i]))
                    throw new ContentException(CreaturesFile, c.Id, $"'{c.Keystones[i]}' is not a keystone of {cls.Id}");
            }
            if (!set.AttacksById.TryGetValue(c.Attack, out var attack) || attack.Creature != c.Id)
                throw new ContentException(CreaturesFile, c.Id, $"unknown attack '{c.Attack}'");
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

    private sealed record ClassesDoc(IReadOnlyList<ClassDef> Classes);

    private sealed record SkillsDoc(IReadOnlyList<SkillDef> Skills, IReadOnlyList<AttackDef> Attacks);

    private sealed record CreaturesDoc(IReadOnlyList<CreatureDef> Creatures);

    private sealed record StatusesDoc(IReadOnlyList<StatusDef> Statuses);
}
