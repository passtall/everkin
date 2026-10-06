# Production decisions

**Status:** Decisions that do not concern game mechanics: platforms, release, business model, technology, saving, languages and accessibility.
**Game rules:** [game_system.md](game_system.md). **Screens and interactions:** [ui_spec.md](ui_spec.md).

Game mechanics stay in game_system.md and classes_and_skills.md. Every new decision that is not a game mechanic goes here.

## 1. Platforms, release and business model

Adopted 2026-10-06.

### 1.1 Platforms

- **Release order.** Android and iOS release first; Windows follows.
- **Stores.** The Windows version is sold on Steam.
- **Minimum devices.** Godot's own minimum OS versions. The game is tested on a mid-range phone from about 2020 and runs on phones and tablets.
- **Screen shapes.** Everything from 4:3 tablets to 21:9 phones. The board stays 16:9 and extra space goes to margins. Phone notches and other unsafe areas are kept clear.
- **Renderer.** Godot's Compatibility renderer (OpenGL) on every platform.
- **Controllers.** Supported on Windows from the first Steam release.

### 1.2 Business model

- **Price.** One-time purchase on every platform. No in-app purchases and no ads, ever. No booster packs, paid random draws, trading or duplicate conversion (game_system.md §2.2).
- **Cosmetic frames.** Earned by playing, through campaign milestones and achievements. Never sold.
- **Demo.** A free demo with the tutorial and the first campaign stage. Progress carries over on purchase.

### 1.3 Languages, accessibility and data

- **Languages.** English only at release; more languages after release. All text already goes through translation keys (section 2.3).
- **Accessibility.** Statuses are always readable without color (icons and short labels). A reduced-motion setting and a UI text size setting.
- **Reporting.** Opt-in crash reports only. No gameplay tracking.

### 1.4 Saving

- **Profiles and timing.** One profile per device, stored locally; cloud saves come later. The game saves automatically after every battle and campaign step, and after every action in a battle, so a battle closed by the device resumes where it was. No manual save slots.
- **Format.** JSON with a version number. Older saves are upgraded step by step when loaded.
- **Damaged saves.** The game writes to a temporary file and then swaps it in, and keeps the previous save as a backup that loads automatically if the newest one is damaged.
- **Battles and updates.** A battle in progress resumes after an update only if the content it uses did not change. Otherwise it restarts from its setup.

## 2. Technical foundation

Godot 4.7 with .NET is installed and the repository contains a Godot project. No code exists yet. Background and the earlier proposal are in game_system.md §16.

### 2.1 Foundation (adopted 2026-10-05)

- **Language.** C# everywhere, including the Godot UI.
- **Rules core.** A plain C# library with no Godot dependency. The game and the test runner both use it.
- **Batch runner.** A small .NET command-line program built on the rules core runs AI-versus-AI batches (game_system.md §2.5).
- **Code location.** The same repository: the core and the runner in a `src` folder, the Godot project stays at the root.
- **Content format.** JSON files in the repository, one per creature list, class and status set, using the stable text IDs (game_system.md §3.7).
- **Skill mechanics.** Numbers, tags and reach live in JSON. Each skill's mechanic is C# code built from shared building blocks (hit, heal, move, apply status).
- **Source of truth.** The design documents stay the specification. A test checks that the JSON content matches the document tables.
- **Randomness.** One seeded random generator per battle, implemented in the core (never the platform's own random). The AI's search never draws from it.
- **Tests.** Every worked check in game_system.md §20.5 becomes an automated test, plus unit tests per rule.
- **Where tests and batches run.** Locally on the owner's PC. There is no GitHub CI, and batches are not run in Claude's sessions.
- **Online backend.** Online play stays deferred (game_system.md §2.4). When it comes, its backend server will be a **Spring Boot (Java)** application.

### 2.2 Rules core internals (adopted 2026-10-05)

- **Battle state and search.** The battle state is mutable with a fast `Clone()`. The AI search clones before trying an action.
- **Events.** Every action returns an ordered list of events (hit, intercept, move, status applied, Momentum shift, KO). The UI animates them.
- **Authority.** Only the core changes battle state. Godot sends chosen actions in and only reads state and events out.
- **Previews.** The core answers preview queries (damage ranges, interception chances, timeline position, forced movement) with the same code that resolves actions, without changing state.
- **Numbers.** The core uses integers only: percentages as whole numbers, coefficients in hundredths, explicit rounding down. No floating point in rules code.
- **Online checking.** How the Spring Boot server checks battles is decided when online play starts; nothing in the core depends on it. The online transport is expected to use WebSockets.
- **.NET version.** The newest long-term-support .NET that Godot 4.7 supports, shared by the core, the runner and the Godot project.
- **Skill code.** One small C# class per skill, found by its skill ID. Shared behavior lives in reusable, specialized classes per tag or mechanic (for example interception, area shapes, forced movement), and skill classes delegate to them, so no rule is written twice.
- **Broken content.** Content is validated at load. Any error stops the program with a clear message naming the file and ID.
- **Tests.** xUnit.
- **Compiler.** Nullable reference types on and warnings as errors in the core and the runner; relaxed in the Godot project.
- **Parallel runs.** The runner plays battles in parallel on all CPU cores. Each battle is single-threaded and deterministic.
- **Run output.** One folder per run: a snapshot (content JSON, git commit, AI settings), one CSV row per battle, and a Markdown report.

### 2.3 AI search and Godot app (adopted 2026-10-05)

- **Search algorithm.** Alpha-beta minimax with iterative deepening (depth 1, then 2, and so on up to the difficulty's limit).
- **Candidate actions.** All legal actions are considered at the top of the search. Deeper down, only the best few candidates (for example 8) by a quick score are searched. Leaving Move and Skip Turn out of the deeper search is an option to try in testing.
- **Weaker picks.** A lower difficulty sometimes picks among its top few actions instead of the best, using the AI's own seeded generator, separate from the battle's.
- **Thinking time.** Fixed depth only, no time cap. Difficulty levels are tuned so the hardest stays quick on a mid-range phone.
- **Cards.** One reusable Card scene with display modes, used in every screen.
- **Input.** Godot input actions: mouse, touch and keyboard map to the same commands (select, confirm, inspect, cancel).
- **Animation.** Battle events play one after another from a queue, each with its own duration, with a speed setting (1x, 2x, instant).
- **Resolution.** The UI is designed for 1920 × 1080 and scaled down for phones.
- **Creature art.** Art files are renamed to the creature IDs, with separate front and rear files (for example `fox_front` and `fox_rear`), and loaded by name.
- **Text.** All UI and content text goes through translation keys from day one. English only at first.
- **Battle viewer controls.** Pause, step one action, speed (1x, 2x, instant), inspect any unit, and pick a recorded setup to start from.

## 3. Still open

- Final commercial name (game_system.md §18).
- Cloud saves, and when they come.
- Prices, store pages, and when a Steam page goes up.
- Performance targets, sound and music, and settings beyond the ones above.
