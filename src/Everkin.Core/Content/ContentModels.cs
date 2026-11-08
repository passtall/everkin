namespace Everkin.Core.Content;

/// <summary>One of the 9 Attack Types (class_system.md section 2).</summary>
public sealed record AttackTypeDef(string Id, string Name, string Description);

/// <summary>One of the 12 Professions with its skill-tree branches (class_system.md section 3, skills.md).</summary>
public sealed record ProfessionDef(string Id, string Name, string Description, IReadOnlyList<string> Branches);

/// <summary>The shared attack of an Attack Type (skills.md "Attack Type attacks").</summary>
public sealed record AttackSkillDef(
    string Id, string Name, string AttackType, int Delay, int Momentum, string Effect);

/// <summary>A node of a profession skill tree (skills.md). Kind is A, P, Mod or K.</summary>
public sealed record ProfessionSkillDef(
    string Id,
    string Name,
    string Profession,
    string Branch,
    string Kind,
    string? Placement,
    string? ModifierOf,
    string? Parent,
    int? Delay,
    int? Momentum,
    string? Delivery,
    string? Range,
    string Effect);

/// <summary>A roster creature (creature-classes.md). The Id is the unit name.</summary>
public sealed record CreatureDef(
    string Id,
    string Name,
    string AttackType,
    IReadOnlyList<string> Professions,
    string ArtFile,
    string? Source,
    string Biome,
    int Hp,
    int Speed,
    int Power,
    int Cost,
    IReadOnlyList<int> FrontFrame,
    IReadOnlyList<int> RearFrame,
    string Status,
    string Notes);

/// <summary>One of the named statuses of game_system.md section 10.5. Its rules are code (<c>Statuses</c>); this is the text and tag.</summary>
public sealed record StatusDef(string Id, string Name, string? Tag, string Effect, string Duration);

/// <summary>Everything the rules core loads from content/*.json.</summary>
public sealed class ContentSet
{
    public required IReadOnlyList<AttackTypeDef> AttackTypes { get; init; }
    public required IReadOnlyList<ProfessionDef> Professions { get; init; }
    public required IReadOnlyList<AttackSkillDef> AttackSkills { get; init; }
    public required IReadOnlyList<ProfessionSkillDef> ProfessionSkills { get; init; }
    public required IReadOnlyList<CreatureDef> Creatures { get; init; }
    public required IReadOnlyList<StatusDef> Statuses { get; init; }

    public IReadOnlyDictionary<string, CreatureDef> CreaturesById { get; init; } = new Dictionary<string, CreatureDef>();
    public IReadOnlyDictionary<string, ProfessionSkillDef> SkillsById { get; init; } = new Dictionary<string, ProfessionSkillDef>();
}

/// <summary>Raised when content is broken. The message names the file and the ID (production.md section 2.2).</summary>
public sealed class ContentException : Exception
{
    public ContentException(string file, string id, string message)
        : base($"{file}: {id}: {message}") { }
}
