# Everkin

A turn-based tactical creature card battler for Windows, Android and iOS, built with Godot 4.7 and C#. The design is finished before implementation starts; the documents in `docs/` are the specification.

## Layout

| Path | What |
|---|---|
| `docs/` | The specification. Start with [game_system.md](docs/game_system.md) (rules, scope, open decisions in §19), then [class_system.md](docs/class_system.md), [skills.md](docs/skills.md), [creature-classes.md](docs/creature-classes.md) (the roster), [ui_spec.md](docs/ui_spec.md) and [production.md](docs/production.md) (platforms, technology, saving, online). |
| `docs/campaign.md` | Draft campaign content: encounters, bosses, rewards, achievements (not adopted yet). |
| `docs/implementation-plan.md` | Milestones and progress of the implementation. |
| `docs/mockups/` | The reviewed HTML mockups of the battle screen, team editor and campaign map (ui_spec.md wins where they differ). |
| `docs/assets/` | Prompts and provenance for every generated asset. |
| `docs/archive/` | Superseded documents, history only. Never a source for rules. |
| `assets/` | Everything the game loads: creature and summon art, card frames, backgrounds, icons, brand, fonts, audio. See [assets/README.md](assets/README.md). |
| `source-art/` | Original creature images. Never changed; `tools/art/clean_creatures.py` makes the cleaned copies in `assets/creatures/`. |
| `tools/` | Python scripts that build assets: creature art cleaning and generation (`art/`), card backgrounds (`backgrounds/`), sounds (`audio/`), icon contact sheet (`icons/`). |
| `project.godot` | The Godot project, at the root. Code will go to `src/` (rules core, batch runner, battle service) and `server/` (Spring Boot), as set in production.md §2. |
| `CLAUDE.md` | How an implementing Claude Code session works. |
| `.claude/skills/` | The design-reference skill for Claude sessions. |

`docs/`, `tools/` and `source-art/` carry a `.gdignore`, so Godot does not import them.

## Working rules

- Changes go straight to `main`; there are no pull requests and no CI.
- Never invent a rule the docs leave open: ask the owner.
