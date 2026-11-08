# Everkin implementation plan

This file is the prompt for every Claude Code session that builds Everkin. Read it fully at the start of every session, then follow **Starting or resuming a session** below. It changes rarely. What is done, what is in progress and what comes next lives in [PROGRESS.md](../PROGRESS.md) at the repository root, which you keep up to date as you work.

## Your job

Build Everkin, a turn-based tactical creature card battler for Windows, Android and iOS, with Godot 4.7 .NET and C#, exactly as the design documents in `docs/` specify. The design is finished. You implement it; you do not redesign it. Work through the milestones below in order, task by task.

How you work (details in [CLAUDE.md](../CLAUDE.md)):

- You run on the owner's PC through Remote Control. Build, test, run batches and use the Godot editor there.
- Commit and push straight to `main`, in small working steps. No branches, no pull requests, no CI.
- **Never invent a rule.** When the docs leave something open, contradict each other or do not cover a case, do not guess. Write the question into PROGRESS.md under "Questions for the owner", ask the owner if he is around, and continue with a task that does not depend on it.
- The docs win over code. A rule change goes into the docs first (everkin-design skill), then into code, and only when the owner agreed.
- `source-art/` is never changed. `docs/archive/` is never a source.

## Starting or resuming a session

A session can end at any moment (usage limit, closed window). Everything needed to continue must be in the repository and in PROGRESS.md, never only in your context. Every session starts the same way:

1. Read this file, CLAUDE.md and PROGRESS.md.
2. Run `git status` and `git log --oneline -10`. Compare them with PROGRESS.md's **Current task** and **In flight** sections:
   - Uncommitted changes that PROGRESS.md describes: they are the previous session's unfinished work. Read them, finish them, build and test, then commit.
   - Uncommitted changes PROGRESS.md does not mention: inspect them, and if their purpose is unclear, ask the owner before keeping or discarding them. Never discard them silently.
   - The last commit is newer than PROGRESS.md's last entry: read that commit and bring PROGRESS.md up to date first.
3. Build and run the tests (once the solution exists). If something is red, fixing it is the first job.
4. Continue at **Next step** in PROGRESS.md. If it is empty, take the first task of the plan that is not ticked.

## Keeping PROGRESS.md current

PROGRESS.md is the only memory between sessions. Treat updating it as part of every step, not as a wrap-up.

- **Before starting a task:** set **Current task** (its ID and name from this plan) and write the first **Next step**.
- **After every meaningful step** (a class compiles, a test passes, a screen works): update **Next step** and **In flight** (files touched but not committed, half-done parts, what is known to be broken), and commit together with the work when it builds.
- **Commit often.** Commit whenever the build is green and tests pass, at least every 30 to 60 minutes of work. A commit may finish only part of a task; say so in the message and in PROGRESS.md. Never commit a broken build; if you must stop with a broken build, leave the changes uncommitted and describe them precisely under **In flight**.
- **When a task is done:** tick it in the task list, add one line to the **Session log** (date, task, commit), clear **In flight**, set the next task.
- **Anything the next session needs** (a decision the owner made in chat, a gotcha, a workaround, a test that is slow) goes under **Notes**. Owner decisions that change rules go into the docs as well.
- Keep PROGRESS.md short enough to read in a minute: the session log keeps one line per task, and notes that no longer matter are removed.

## Definition of done for every task

- It does what the cited doc sections say, with no invented rules.
- `dotnet build` is clean (warnings are errors in core and runner) and `dotnet test` passes.
- New rules code has unit tests; any §20.5 worked check in its area is an automated test.
- UI work runs in the Godot editor on the PC; check it at desktop size and in the phone view.
- PROGRESS.md is updated, the work is committed and pushed.

## Repository layout to build towards

- `project.godot`, Godot scenes and scripts at the root (`scenes/`, `scripts/`), assets in `assets/`.
- `content/`: the JSON content files the core loads (creatures, classes, skills, statuses, encounters, achievements), generated from the docs tables by a script in `tools/content/` and checked against them by a test.
- `src/Everkin.Core/` (rules core, plain C#, no Godot), `src/Everkin.Runner/` (batch runner), `src/Everkin.Core.Tests/` (xUnit), later `src/Everkin.BattleService/` (ASP.NET Core). `server/` for Spring Boot in milestone 9.
- One `Everkin.sln` at the root with the Godot project and the `src/` projects. **Gotcha:** the Godot `.csproj` at the root compiles every `.cs` file below it, so exclude `src/**` (and `tools/**`) from it with `<Compile Remove="..."/>`.

## Milestones and tasks

Each task names the doc sections it implements. Read those sections before coding the task; read neighbouring sections when a rule refers to them.

### M0 Setup

- **M0.1** Create `Everkin.sln`, `src/Everkin.Core`, `src/Everkin.Runner`, `src/Everkin.Core.Tests` on the newest LTS .NET that Godot 4.7 supports; nullable on, warnings as errors in core and runner (production §2.1, §2.2).
- **M0.2** Create the Godot C# project file, reference the core, exclude `src/**` and `tools/**` from it, and confirm the empty project opens and runs with the Compatibility renderer (production §1.1).
- **M0.3** Export test: an Android build of the C# project runs on a phone, and a Windows build runs (production §1.1). Write down in Notes what was needed.
- **M0.4** Fill in the build, test and run commands in CLAUDE.md.

### M1 Rules core

- **M1.1** Content pipeline: `tools/content/` script that reads creature-classes.md, class_system.md and skills.md (including the Needs column) and writes `content/*.json`; a test that the JSON matches the docs; load-time validation that stops with file and ID on any error (production §2.1, §2.2). IDs: unit names, snake_case skill names, biome IDs.
- **M1.2** Battle state with fast `Clone()`, the per-battle seeded random generator, the event list returned by every action, integer-only numbers (production §2.2).
- **M1.3** Formation: rows, capacity, ordered positions, alignment, adjacency (game_system §7, §8.4 adjacency).
- **M1.4** Timeline: Delay, Speed, ties, auto events, delayed events, skipped turns (§6.1 to §6.5).
- **M1.5** Momentum: scale, gains, costs, refunds, shifts (§6.7).
- **M1.6** Ranges and targeting: opposing distance, own-side range, either-side rule, Own side only, empty rows (§7.4, §8.1).
- **M1.7** Damage, healing and shields: rolls, Power scaling ×Power÷5, Ward and Dampen, minimum 1 (§5.2).
- **M1.8** Interception and area patterns (§8.2 to §8.5).
- **M1.9** Movement: Move, movement inside skills, forced repositioning and bumps (§9).
- **M1.10** Stealth, KO, bodies, revival (§10.1 to §10.4).
- **M1.11** Statuses and airborne (§10.5, §10.6).
- **M1.12** Reactions (§6.6).
- **M1.13** Summons, constructs, totems, traps, undead (§11).
- **M1.14** Ground effects (§12.2) and battle end (§12.3).
- **M1.15** Skill trees and builds: 8 points, one parent per node (Needs column), random legal builds, default builds (§4.1).
- **M1.16** The nine Attack Type attacks, then every profession skill in skills.md, one profession at a time, one class per skill built from shared building blocks (production §2.2). One task per profession: M1.16a Hunter, b Guardian, c Healer, d Sorcerer, e Elementalist, f Warrior, g Leader, h Rogue, i Tinker, j Summoner, k Primalist, l Flying.
- **M1.17** Every worked check in §20.5 is a passing test.
- **M1.18** Preview queries: damage range, interception chance, timeline position, forced movement (production §2.2).

### M2 AI and AI-versus-AI harness

- **M2.1** AI search: alpha-beta minimax with iterative deepening, candidate pruning, scoring and difficulty levels (game_system §2.5, production §2.3).
- **M2.2** Batch runner: random teams within the budget, random formations, parallel battles, length cap, run folder with snapshot, CSV and Markdown report with imbalance flags (§2.5, production §2.2).
- **M2.3** First full run (10,000 battles) on the PC. Summarize the report in PROGRESS.md and ask the owner to review it. Balance changes only with his approval (§2.5).

### M3 Battle viewer

- **M3.1** Card scene with its display modes: full front, back, compact board card; frames, card backgrounds, stat gems, crops from creature-classes.md (ui_spec card styling, production §1.6, assets/README.md).
- **M3.2** Battle screen layout: board, timeline left, Momentum meter right, action bar bottom, phone one-side view and view button (ui_spec battle screen, docs/mockups).
- **M3.3** Event queue animation of core events, effects and sounds (production §1.6 visual effects and sounds).
- **M3.4** Viewer controls: pause, step, speed, inspect, start from a recorded setup (production §2.3, game_system §2.5).

### M4 Free battle against the AI

- **M4.1** Player turn: action bar, targeting, previews, Move, Skip Turn, surrender (ui_spec).
- **M4.2** Team editor and collection (ui_spec party setup and collection).
- **M4.3** Skill tree editor (ui_spec skill tree editor).
- **M4.4** Free battle setup, difficulty choice and result screen.

### M5 Tutorial and chapter 1

- **M5.1** Profile and saving: local JSON with version, temp-file swap, three backups, battle resume (production §1.4).
- **M5.2** World map and chapter maps (ui_spec campaign map).
- **M5.3** Encounters and rewards from campaign.md as content; tutorial with the automatic tutorial team (game_system §2.2, campaign.md).
- **M5.4** Chapter 1 playable start to boss.

### M6 Local two-player

- **M6.1** Hot-seat battles with the board rotated 180° for player 2 (game_system §2.4 and the 2P rules).

### M7 Rest of the campaign

- **M7.1** Chapters 2 to 4 with their bosses (campaign.md).
- **M7.2** Achievements: the 30 in campaign.md, page and toasts (ui_spec), platform mirroring later in M10.

### M8 Post-story content

- **M8.1** Random battles with difficulty choice (game_system §2.2).
- **M8.2** Challenge encounters C1 to C4 (campaign.md).
- **M8.3** Coin creatures, prices 50 + 25 × Cost, buying from the collection; coins only while connected, so the offline game shows them as unavailable until M9 (game_system §2.2, production §1.4).

### M9 Online play

- **M9.1** Battle service in `src/Everkin.BattleService` (ASP.NET Core, SignalR) running the core (production §2.4).
- **M9.2** Spring Boot server in `server/`: sign-in, profiles and sync, coins, matchmaking, ratings, PostgreSQL with Flyway (production §2.4).
- **M9.3** Game client: sign-in, sync, online screens, reconnect window (ui_spec online screens).
- **M9.4** Staging and production deploy on Hetzner with Docker Compose and Caddy, backups, monitoring (production §2.4). Ask the owner before anything touches a real server.

### M10 Release

- **M10.1** Settings, accessibility, vibration, translation keys complete (production §1.3, §1.6).
- **M10.2** Sentry crash reports, opt-in (production §1.3).
- **M10.3** Mobile art compression (lossy WebP, 768 × 1152 on mobile) and the 200 MB download check (production §1.6).
- **M10.4** Platform achievements, store builds for Windows (Steam) and Android, demo build; iOS once a Mac is available (production §1.1, §1.2). Store accounts and anything public need the owner.
