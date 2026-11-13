# Work order: classes rework (milestone M5.6)

The owner turned the old profession branches into 28 classes (2026-11-11) and dropped skill distance (2026-11-12). The rules now live in the docs; this file is only the order of work for the code. Where it and a doc differ, the doc wins.

| Topic | Where the rules are |
|---|---|
| 28 classes, what a creature has, unique attacks, magical classes | docs/class_system.md |
| Each class's 3 actives and 3 keystones, with Delay, Momentum, delivery and rulings | docs/skills.md |
| Each creature's 2 classes, 2 keystones and unique attack | docs/creature-classes.md |
| No range, Front-row melee, melee reach, interception, nearest, tags, summons, airborne | docs/game_system.md §4.7, §7.4, §8.1 to §8.5, §10.6, §11 |
| Card, action bar, target preview, team editor | docs/ui_spec.md |
| Formations by role, achievements | docs/campaign.md |

## Order

1. **Content.** `classes.json` (28 classes), `skills.json` (84 actives, 84 keystones, 319 unique attacks), `creatures.json` (classes, keystones and attack per creature). Delete `default_builds.json`; remove enemy builds from `campaign.json`. Drop Clockwork Golem, Bell Golem, Animate Mirror, Flame Turret, Cage and the Hunter decoy (keep their art). A content test checks the JSON against the doc tables.
2. **Core.** Regroup `Skills/*Skills.cs` by class and add the new skills. Keystones are passives fixed per creature. One parametric unique attack (delivery, damage, one extra). Remove `SkillBuild.cs` and the nine shared attacks. Magical tag from class_system.md; no element tags. Range removed: Front-row melee, melee reach, melee interception 75/50 and own-side hits never intercepted (`Targeting.cs`, `Interception.cs`); "nearest" per game_system §8.5; summon placement on any free spot. Coordinated Strike, songs and Hunt Down as in skills.md.
3. **Tests.** Rewrite the game_system §20.5 worked checks.
4. **AI.** Remove random builds; make sure the AI scores every new skill and keystone.
5. **UI.** Remove `SkillTreeEditor.cs` and its entry points; team editor class filter; action bar order; card front plate and card back keystones; inspection panel keystones; target preview with the direct-hit chance.
6. **Saving.** Bump the save version and reset older profiles.
7. **Icons.** One icon per class active and unique attack, named by skill ID (prompts in docs/assets/skill-icons-classes.md and skill-icons-attacks.md); placeholders until the art exists, then delete the 47 old profession and Attack Type icons. Five new status icons (docs/assets/status-icons-new.md). The five unused summon cards are not loaded.
8. **Then.** Rerun the AI-versus-AI harness, decide the Cost formula (game_system §5.1) and recalculate Cost.
