# Everkin

A turn-based tactical creature card battler for Windows, Android and iOS, built with Godot 4.7 and C#. The documents in `docs/` are the specification.

## Layout

| Path | What |
|---|---|
| `docs/` | The specification: [game_system.md](docs/game_system.md) (rules, scope, open decisions in §19), [class_system.md](docs/class_system.md), [skills.md](docs/skills.md), [creature-classes.md](docs/creature-classes.md) (the roster), [campaign.md](docs/campaign.md), [ui_spec.md](docs/ui_spec.md) and [production.md](docs/production.md) (platforms, technology, saving, online). |
| `docs/implementation-plan.md` | The prompt for implementing sessions: milestones, tasks, how to resume. |
| `PROGRESS.md` | Live implementation state: current task, next step, uncommitted work, questions. |
| `assets/` | Everything the game loads. See [assets/README.md](assets/README.md). |
| `content/` | The JSON content the core loads, generated from the doc tables by `tools/content/build_content.py`. |
| `project.godot`, `scenes/`, `scripts/` | The Godot project at the root. |
| `src/` | Rules core, batch runner and tests (later the battle service); `server/` will hold the Spring Boot server (production.md §2). |
| `teams/` | Named batches of sample teams for the AI-versus-AI harness ([teams/README.md](teams/README.md), [docs/team-improvement-prompt.md](docs/team-improvement-prompt.md)). |
| `analysis/`, `analysis-baseline-1/`, `analysis-round1/` | Summaries of harness runs kept in git (the runs themselves are in the git-ignored `runs/`). |
| `CLAUDE.md`, `.claude/skills/` | How an implementing Claude Code session works, and the design-reference skill. |

`docs/` and `tools/` carry a `.gdignore`, so Godot does not import them.

## Working rules

- Changes go straight to `main`; there are no pull requests and no CI.
- Never invent a rule the docs leave open: ask the owner.
