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
- **Controllers.** Supported on Windows from the first Steam release. The Windows release aims for Steam Deck Verified (ui_spec.md, Controls).

### 1.2 Business model

- **Price.** One-time purchase on every platform. On mobile this is a free download with the demo content and one purchase that unlocks the full game. No ads, ever. No booster packs, paid random draws, trading or duplicate conversion (game_system.md §2.2).
- **Coins.** Coins are earned in play (game_system.md §2.2) and **can also be bought**. They unlock specific creatures, never random ones. **Coin packs** (adopted 2026-10-07): three packs of 500, 1,200 and 2,500 coins, kept generous compared with creature prices (game_system.md §2.2). Real-money prices are set at release.
- **Cosmetic frames.** Earned by playing, through campaign milestones and achievements. Never sold. Every card wears its home biome's frame by default (`art/frames/card/<biome_id>.png`, adopted 2026-10-08; this replaced the plain gold frame); an earned frame can replace it on creatures the player chooses (confirmed 2026-10-08). Frames never mark tier or class.
- **Demo.** A free demo with the tutorial and the first campaign stage. Progress carries over on purchase. On mobile the demo is the free download itself; on Steam it is a separate demo.

### 1.3 Languages, accessibility and data

- **Languages.** English only at release; more languages after release. All text already goes through translation keys (section 2.3).
- **Accessibility.** Statuses are always readable without color (icons and short labels). A reduced-motion setting and a UI text size setting.
- **Reporting.** Opt-in crash reports only. No gameplay tracking.

### 1.4 Saving

- **Profiles and timing.** One profile per device, stored locally. The game saves automatically after every battle and campaign step, and after every action in a battle, so a battle closed by the device resumes where it was. No manual save slots.
- **Format.** JSON with a version number. Older saves are upgraded step by step when loaded.
- **Damaged saves.** The game writes to a temporary file and then swaps it in, and keeps the previous save as a backup that loads automatically if the newest one is damaged.
- **Battles and updates.** A battle in progress resumes after an update only if the content it uses did not change. Otherwise it is dropped, and a short note says so (changed 2026-10-07; it used to restart from its setup).
- **Backups (adopted 2026-10-08).** The device keeps the last three automatic save backups. If the save is damaged, the game loads the newest good backup and shows a short note.
- **Cloud saves (adopted 2026-10-07).** At release each store's own cloud save backs up the profile: Steam Cloud on Windows, Google Play saved games on Android, iCloud on iOS. Saves do not move between platform families; that comes with online accounts. If the cloud save and the device save differ, the game asks the player which to keep and shows each save's progress and date.

### 1.5 Story presentation

Adopted 2026-10-06.

- **Format.** No story text for now. Campaign locations show their illustrations only; story scenes may come with the full campaign design.
- **Player role.** Decided together with the story later (game_system.md §15).

### 1.6 Art, audio and settings

Adopted 2026-10-06.

- **Art style.** The current faceted creature art is the final style. It is cleaned up and made consistent, but **the original files are never changed**: cleanup works on copies.
- **Art files.** Cleaned copies use one canvas of 1024 × 1536 (2:3) with a transparent background, and every creature stands on a shared ground line.
- **Creature size.** Cards do not show creature size: every creature fills its frame, with no size scaling or size icon. Size contrast was a leftover from the 3D direction (game_system.md §14.3).
- **Card backgrounds.** A soft background behind the creature from its home biome: one card background design per biome (adopted 2026-10-08, biome list in game_system.md §14.5).
- **Background art.** `art/backgrounds/` holds one card and one battle background per biome. Card backgrounds are generated by `tools/backgrounds/gen_backgrounds.py` and faceted like the creature art (accepted 2026-10-08). **Battle backgrounds are photo-real** (adopted 2026-10-08): night or dusk nature photos, 16:9 at 1920×1080 or larger, dark and calm in the centre, top and bottom 8% kept free. All 18 are in `art/backgrounds/battle/<biome_id>.png`.
- **Battle backgrounds.** An illustrated, dimmed background per biome. Campaign battles use their stage's biome; free and random battles use a random biome the player has reached in the campaign (adopted 2026-10-08).
- **UI look.** Night fairytale (a daytime version was rejected): menus and screens use an illustrated fairytale background, and the UI colors, panels and buttons are styled to fit it (adopted 2026-10-06 from the mockup review).
- **Idle motion.** None. Cards only react to hits and effects.
- **Music.** Quiet ambient sound only; no melodic soundtrack.
- **Sound effects.** One set per tag and effect (melee, projectile, magic, heal, status, KO). No creature cries.
- **Sound list** (adopted 2026-10-08):
  - *Interface:* confirm, cancel, invalid action, panel open and close, hover, tab switch, drag pickup and drop, card flip.
  - *Coins:* a soft coin sound when coins are gained or spent.
  - *Battle:* three variants of each battle sound, picked at random with a small random pitch change. A soft chime when one of the player's units gets its turn (no cue for enemy turns). Momentum sounds only when it crosses a big threshold or reaches the end of the bar. Statuses: one sound for a helpful status, one for a harmful status, one for a status ending. KO: a glassy crack, then a soft card-flip thud, matching the crack animation.
  - *Result:* a short non-melodic sting (a swell or chime, about 2 seconds) for victory and one for defeat.
  - *Ambient:* one loop per biome. Made only once the biome list is final. The campaign map plays the current stage's biome loop; the other menus play the loop of a random biome.
  - *Format:* OGG Vorbis at 48 kHz, effects mono, ambient stereo, all levelled to the same loudness.
- **Audio source.** AI-generated, with licensing checked for each tool used.
- **Vibration.** Light vibration on hits and KOs on phones, with a setting to turn it off.
- **Settings menu.** Music and sound volume, reduced motion, text size, vibration, language, crash reports, and key and controller bindings on Windows. There is no battle speed setting anywhere in the game (adopted 2026-10-08); only the development battle viewer has speed controls.
- **Performance.** No hard numbers: the game must feel smooth. As orientation, about 30 frames per second on the test phone and 60 on Windows, and the AI deciding within about 2 seconds.
- **Download size.** The mobile download stays under 200 MB, with art compressed for phones.
- **Art pipeline.** A script in the repository reads `images/kin` and writes the cleaned, ID-named 1024 × 1536 copies to a separate folder. It can be re-run at any time.

### 1.7 Playtesting

- A closed test with friends through TestFlight and Google Play internal testing before release.

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
- **Animation.** Battle events play one after another from a queue, each with its own duration, always at one speed (the battle viewer can speed it up).
- **Resolution.** The UI is designed for 1920 × 1080 and scaled down for phones.
- **Creature art.** The game loads art by creature ID, with separate front and rear files (for example `fox_front` and `fox_rear`). The original files in `images/kin` are never changed; cleaned and renamed copies are produced from them (section 1.6).
- **Text.** All UI and content text goes through translation keys from day one. English only at first.
- **Battle viewer controls.** Pause, step one action, speed (1x, 2x, instant), inspect any unit, and pick a recorded setup to start from.

## 3. Roadmap

Adopted 2026-10-06.

- **Specification first.** The specification is finished completely before implementation starts. How progress is tracked is decided just before implementation.
- **Order after the AI-versus-AI milestone.** Battle viewer on the real cards, then free battle against the AI, then the tutorial and stage 1, then local two-player, then the rest of the campaign, then post-story content, then release.
- **Code changes.** Code goes straight to main, like the docs. There are no pull requests.

## 4. Still open

- Availability check for the name Everkin (trademarks, stores, domains) before a store page goes up (game_system.md §18).
- Game price, real-money prices for the coin packs, store pages, and when a Steam page goes up.
