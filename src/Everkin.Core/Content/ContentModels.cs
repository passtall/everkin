namespace Everkin.Core.Content;

/// <summary>One of the 28 classes (class_system.md section 2): three actives and three keystones.</summary>
public sealed record ClassDef(string Id, string Name, string Role, bool Magical, IReadOnlyList<string> Actives, IReadOnlyList<string> Keystones);

/// <summary>A class active ("A") or keystone ("K") of skills.md. Delivery is melee, projectile, direct or none; Delay and Momentum are only set for actives.</summary>
public sealed record SkillDef(string Id, string Name, string Class, string Kind, int? Delay, int? Momentum, string? Delivery, string Effect, string? Template = null);

/// <summary>The one small extra of a unique attack, parsed from its effect text (class_system.md section 4).</summary>
/// <param name="Kind">status, push, pull, ground, steal, delay_turn or stealth.</param>
/// <param name="Chance">Percent; 100 for a certain extra.</param>
/// <param name="Status">For kind status: stunned, confused, immobilized, blind, silenced, poisoned, bleeding or marked.</param>
/// <param name="Duration">For kind status: time units (0 = the status's own length).</param>
/// <param name="Ground">For kind ground: burning, ice, water or poison.</param>
/// <param name="Amount">For kind delay_turn: time units the next turn comes later.</param>
public sealed record AttackExtraDef(string Kind, int Chance, string? Status = null, int Duration = 0, string? Ground = null, int Amount = 0);

/// <summary>
/// A creature's unique attack. Shape: single (one target), splash (the target and the units next to it) or pulse (every enemy in melee reach, no target).
/// Direct attacks work in melee reach (game_system.md section 8.1).
/// </summary>
public sealed record AttackDef(
    string Id, string Name, string Creature, string Delivery, string Shape, int Delay, int Momentum, string Style, bool Magical,
    int Min, int Max, int SplashMin, int SplashMax, AttackExtraDef Extra, string Effect);

/// <summary>A roster creature (creature-classes.md). The Id is the unit name.</summary>
public sealed record CreatureDef(
    string Id,
    string Name,
    IReadOnlyList<string> Classes,
    IReadOnlyList<string> Keystones,
    string Attack,
    string ArtFile,
    string Biome,
    int Hp,
    int Speed,
    int Power,
    int Cost,
    IReadOnlyList<int> FrontFrame,
    IReadOnlyList<int> RearFrame);

/// <summary>One of the named statuses of game_system.md section 10.5. Its rules are code (<c>Statuses</c>); this is the text and tag.</summary>
public sealed record StatusDef(string Id, string Name, string? Tag, string Effect, string Duration);

/// <summary>Everything the rules core loads from content/*.json.</summary>
public sealed class ContentSet
{
    public required IReadOnlyList<ClassDef> Classes { get; init; }
    public required IReadOnlyList<SkillDef> Skills { get; init; }
    public required IReadOnlyList<AttackDef> Attacks { get; init; }
    public required IReadOnlyList<CreatureDef> Creatures { get; init; }
    public required IReadOnlyList<StatusDef> Statuses { get; init; }

    public IReadOnlyDictionary<string, CreatureDef> CreaturesById { get; init; } = new Dictionary<string, CreatureDef>();
    public IReadOnlyDictionary<string, ClassDef> ClassesById { get; init; } = new Dictionary<string, ClassDef>();
    public IReadOnlyDictionary<string, SkillDef> SkillsById { get; init; } = new Dictionary<string, SkillDef>();
    public IReadOnlyDictionary<string, AttackDef> AttacksById { get; init; } = new Dictionary<string, AttackDef>();

    /// <summary>The six actives and the two keystones of a creature (every class skill it has), in class order.</summary>
    public IEnumerable<string> ClassSkillsOf(CreatureDef creature) =>
        creature.Classes.SelectMany(c => ClassesById[c].Actives).Concat(creature.Keystones);
}

/// <summary>Raised when content is broken. The message names the file and the ID (production.md section 2.2).</summary>
public sealed class ContentException : Exception
{
    public ContentException(string file, string id, string message)
        : base($"{file}: {id}: {message}") { }
}
