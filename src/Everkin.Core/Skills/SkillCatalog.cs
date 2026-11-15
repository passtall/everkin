using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// Every implemented skill, found by its skill ID (production.md section 2.2). Delay and Momentum come from the content files;
/// the mechanic is the skill's class. The unique attacks are one parametric skill each, the class skills one class per skill.
/// </summary>
public sealed class SkillCatalog
{
    private readonly Dictionary<string, Skill> _skills = new();

    public SkillCatalog(ContentSet content, IEnumerable<Skill> skills)
    {
        Content = content;
        foreach (var s in skills)
            if (!_skills.TryAdd(s.Id, s))
                throw new ContentException("skills", s.Id, "two skill classes share this ID");
    }

    public ContentSet Content { get; }

    /// <summary>The unique attack of every creature, then the classes implemented so far.</summary>
    public static SkillCatalog Create(ContentSet content) =>
        new(content, content.Attacks.Select(a => (Skill)new UniqueAttackSkill(a)));

    public bool Has(string skillId) => _skills.ContainsKey(skillId);

    public bool TryGet(string skillId, out Skill skill) => _skills.TryGetValue(skillId, out skill!);

    public Skill Get(string skillId) =>
        _skills.TryGetValue(skillId, out var s) ? s : throw new ArgumentException($"Skill '{skillId}' is not implemented.", nameof(skillId));

    public IEnumerable<string> Ids => _skills.Keys;

    /// <summary>Whether a skill ID is a creature's unique attack.</summary>
    public bool IsUniqueAttack(string skillId) => Content.AttacksById.ContainsKey(skillId);
}
