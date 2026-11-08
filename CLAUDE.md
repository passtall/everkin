# Everkin: instructions for implementing sessions

Everkin is a turn-based tactical creature card battler (Godot 4.7 .NET, C#). The design is finished; the documents in `docs/` are the specification. Your job is to build it, not to redesign it.

## How you work

- **Where.** Every implementation session runs on the owner's PC through Remote Control (production.md §2.1). Build, run tests and batches and use the Godot editor there. Nothing is built or run in cloud sessions, and there is no GitHub CI.
- **Git.** Commit and push straight to `main`. No branches for features, no pull requests. Push small, working steps.
- **Plan and progress.** [docs/implementation-plan.md](docs/implementation-plan.md) is your prompt: the milestones, the tasks and how to start or resume a session. [PROGRESS.md](PROGRESS.md) is the live state: current task, next step, uncommitted work, questions, notes. Update PROGRESS.md after every meaningful step and commit it with the work, so a session that stops at any moment can be continued by the next one.
- **Never invent a rule.** If the docs leave a rule open, contradict each other or do not cover a case, stop and ask the owner. Do not pick a "sensible default" in code. Write the question down in PROGRESS.md's "Questions for the owner" list if the owner is not around, and work on something else.
- **The docs win.** When code and docs disagree, the docs are right unless the owner says otherwise; a rule change is written into the docs first (see the everkin-design skill), then coded.
- **Original art is never changed.** `source-art/` is read-only. Asset scripts write to `assets/`.

## Reading order

0. [docs/implementation-plan.md](docs/implementation-plan.md) and [PROGRESS.md](PROGRESS.md), every session, first.
1. [README.md](README.md): repository layout.
2. [docs/production.md](docs/production.md) §2 and §3: technology, code layout, tests, online backend, roadmap.
3. [docs/game_system.md](docs/game_system.md): every rule. It is about 1500 lines; read the section you need. §2.5 is the AI-versus-AI harness, §19 lists what is still open, §20.5 lists the worked checks that become tests.
4. [docs/class_system.md](docs/class_system.md), [docs/skills.md](docs/skills.md), [docs/creature-classes.md](docs/creature-classes.md): content.
5. [docs/ui_spec.md](docs/ui_spec.md) and [docs/mockups/](docs/mockups/README.md) once UI work starts.
6. [assets/README.md](assets/README.md) for every asset file and its format.

Never use `docs/archive/` as a source; it is history only.

## Source of truth

| Topic | Owner document |
|---|---|
| Game rules: timeline, Momentum, damage, ranges, interception, movement, statuses, summons, ground effects, battle end, AI harness | docs/game_system.md |
| Attack Types and Professions | docs/class_system.md |
| Every skill: Delay, Momentum, delivery, range, effect (numbers at Power 5) | docs/skills.md |
| Creature roster: unit name (= ID), display name, classes, biome, stats, Cost, art file, card crops | docs/creature-classes.md |
| Screens, layouts, interactions | docs/ui_spec.md (mockups are reference only) |
| Platforms, technology, saving, online backend, art and audio formats, roadmap | docs/production.md |
| Asset files and formats | assets/README.md; prompts in docs/assets/ |

## Code layout and conventions (production.md §2)

- `src/` holds the rules core (plain C# library, no Godot dependency), the batch runner (.NET console app) and later the ASP.NET Core battle service. `server/` holds the Spring Boot server (online milestone only). The Godot project stays at the repository root.
- Content is JSON, one file per creature list, class and status set, generated from or checked against the document tables by a test.
- IDs: creatures use their unit name (`weasel-rogue-dagger`); skills use the skill name in snake case (`heavy_blow`); biomes use the IDs in assets/README.md.
- Rules code uses integers only (percentages as whole numbers, coefficients and fractional damage in hundredths, explicit rounding down). One seeded random generator per battle, never the platform's.
- Only the core changes battle state; every action returns an ordered event list that the UI plays.
- Nullable reference types on and warnings as errors in the core and runner. Tests use xUnit; every worked check in game_system.md §20.5 becomes a test.
- All UI text goes through translation keys (English only at first).

## Build and test

Once the solution exists (milestone 1 of the plan), the commands go here:

- Build: `dotnet build`
- Tests: `dotnet test`
- Batch run: see the runner's `--help`
- Godot: open the project at the root in the Godot 4.7 .NET editor; the renderer is Compatibility on every platform.
