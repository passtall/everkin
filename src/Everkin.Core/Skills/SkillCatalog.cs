using Everkin.Core.Content;

namespace Everkin.Core.Skills;

/// <summary>
/// Every implemented skill, found by its skill ID (production.md section 2.2). Delay and Momentum come from the content files;
/// the mechanic is the skill's class.
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

    /// <summary>The skills implemented so far: the nine Attack Type attacks, then the professions one by one.</summary>
    public static SkillCatalog Create(ContentSet content) =>
        new(content, AttackSkills.All(content).Concat(HunterSkills.All(content)).Concat(GuardianSkills.All(content)).Concat(HealerSkills.All(content)).Concat(SorcererSkills.All(content)).Concat(ElementalistSkills.All(content)).Concat(WarriorSkills.All(content)).Concat(LeaderSkills.All(content)));

    public bool Has(string skillId) => _skills.ContainsKey(skillId);

    public bool TryGet(string skillId, out Skill skill) => _skills.TryGetValue(skillId, out skill!);

    public Skill Get(string skillId) =>
        _skills.TryGetValue(skillId, out var s) ? s : throw new ArgumentException($"Skill '{skillId}' is not implemented.", nameof(skillId));

    public IEnumerable<string> Ids => _skills.Keys;

    /// <summary>The ID of the shared attack of an Attack Type: every creature starts with it (section 4.4).</summary>
    public string AttackSkillFor(string attackTypeId) =>
        Content.AttackSkills.Single(a => a.AttackType == attackTypeId).Id;
}
