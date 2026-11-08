# Progress

The implementation state, kept current by every Claude Code session ([docs/implementation-plan.md](docs/implementation-plan.md) says how). If a session stops at any moment, the next one continues from here.

**Last updated:** 2026-11-08, M1.4 done.

## Current task

None between tasks. Next is **M1.5**.

## Next step

M1.5 Momentum: read game_system §6.7 (meter -10 to +10, shifts, costs, refunds, shifts per action) and the §20.5 Momentum checks; add it to BattleState.

## In flight

Nothing.

## Questions for the owner

1. **Stale text, no task blocked (game_system §2.5, line "Each unit gets a random legal build: its 10 skill points").** §4.1 says 8 skill points (changed 2026-11-08), and skills.md and the plan say 8. I use 8; please fix §2.5 (or tell me 10 is meant).
2. **Timeline gaps, no task blocked (game_system §6.1).** I filled these with the most literal reading; tell me if any is wrong: (a) Haste or Slow that would take the remaining Wait below 0 makes the turn arrive now, like the Pull Forward overshoot rule; (b) Speed changes move the remaining Wait by the change in Speed as the formula sees it (clamped to 1-10), so Haste +2 at Speed 9 shortens it by 1; (c) Reset uses the Wait the entry was scheduled with and ignores later Haste or Slow; (d) an entry that is pulled or pushed but stays in the same time unit keeps its place in the tie order; (e) a new Wait (turn, auto event, delayed event, effect end) is at least 1.

## Notes

- M0.1 uses net10.0 (newest LTS installed, SDK 10.0.301). Verified in M0.2: Godot.NET.Sdk 4.7.2 builds the root project on net10.0 (the editor itself runs on net8 with rollForward).
- Godot 4.7.2 mono is at `E:\Downloads\Neuer Ordner\Godot_v4.7.2-stable_mono_win64\` (use the `_console.exe` for headless: `--headless --path . --build-solutions --quit`). Running it once adds the NuGet fallback so plain `dotnet build` works at the root. `.import`/`.uid` files are committed (Godot convention).
- Core and runner have TreatWarningsAsErrors; tests do not. `dotnet build` / `dotnet test` at the root work.
- M0.3: export presets are in `export_presets.cfg` (Windows Desktop, Android). Export templates 4.7.2.stable.mono are in %APPDATA%\Godot\export_templates. Android SDK is at %LOCALAPPDATA%\Android\Sdk (platform 35, build-tools 35.0.1, NDK 28.1.13356709, licences accepted), JDK is Zulu 17; Godot editor settings already point to both.
- Headless export: `Godot...console.exe --headless --path . --export-debug "Windows Desktop" build/windows/Everkin.exe --quit` (without `--quit` the process hangs after exporting). Output in `build/` (git-ignored). Windows build verified: boot scene prints core name and .NET 10.0.9.
- M0.3 verified on a Samsung SM-S928B (Android, Adreno 750): boot scene shows core name and .NET 10.0.9. The phone screen must be unlocked for adb launches; `adb shell monkey -p com.everkin.game -c android.intent.category.LAUNCHER 1` starts the app, `adb logcat -d -s godot` shows its output. Use bash (not PowerShell) for `adb exec-out screencap -p > file.png`.
- M1.1: `python tools/content/build_content.py` regenerates `content/classes.json`, `skills.json`, `creatures.json` from the docs (`--check` compares without writing; the xunit test `JsonMatchesTheDocs` runs it, so python must be on PATH). After any change to creature-classes.md, class_system.md or skills.md, rerun it and commit the JSON. `Everkin.Core.Content.ContentLoader` loads and validates (file + ID in every error). Skill effect text stays prose in JSON: numbers inside effects are NOT parsed yet; each skill's numbers go into code or JSON when its skill class is written (M1.16). Delivery `inherit` = modifier ("As parent"), `none` = placement range only (summons, constructs, traps); `placement` is summon / construct / trap_card. Statuses, encounters and achievements are not content files yet (M1.11, M5.3, M7.2).
- M1.2: `Everkin.Core.Battle`: `BattleRandom` (xoshiro256** seeded by SplitMix64, own code, sequence pinned by a test with values from an independent Python implementation: never change it, or old seeds stop replaying), `BattleState` (units, `Clone()`, event buffer `Emit`/`DrainEvents`; a clone starts with no pending events), `Unit` (id, side, name, creature id, HP, Speed, Power), `Side` A/B, `BattleEvent` base record (+ `BattleStarted`). Later tasks add formation, timeline, Momentum, statuses and the concrete events. Damage rolls will use `Random.NextRange(min, max)`.
- M1.3: `Formation` (on `BattleState.Formation`): rows Front/Middle/Rear per `Side`, 6 per row, 18 per side, ordered left to right; `Place`/`Remove`/`PositionOf`/`Units`; `Center(count, index) = 2i-(n-1)`, `OverlapPercent` (100/50/0), `UnitOverlapPercent(a, b)`, `Adjacent(unit)` (left, right, then the same-side rows in front and behind at >= 50% overlap, nominal rows). It knows positions only: KO, stealth and visibility filtering belong to the rules that use it (a KO body still blocks its place, §7.5). Interpretation to confirm, no task blocked: adjacency stays within one side (a Front unit's "row in front" is not the enemy Front); §8.4 does not say so, but otherwise Breath on an enemy Front unit would splash your own Front row. Moves, swaps and bumps are M1.9; opposing distance is M1.6.
- M1.4: `Timeline` (on `BattleState.Timeline`): `Wait(delay, speed)`, `InitialWait`, `SkippedTurnWait`; entries have `Due` and join `Order` (kinds UnitTurn, AutoEvent, DelayedEvent, EffectEnd; `Tag` says what they stand for); `Add`, `AddBatch`/`ScheduleInitialTurns` (ties among simultaneous new entries by the battle's seeded generator), `ScheduleTurn`, `Pop` (advances `Now`), `Remove`, `RemoveOwnedBy` (KO), `PullForward`, `PushBack`, `Reset`, `ChangeSpeed(unit, oldSpeed, newSpeed)`. Blocking auto events hold the owner's turn back until the last one is done (turn cost then starts). Not done yet: the battle driver that pops entries and runs them (comes with the skills, M1.12 onwards); events for timeline changes; the status layer that decides a unit's effective Speed. Call `ScheduleTurn` when a skill is used (before its effects), `ChangeSpeed` when effective Speed changes.
- Boot scene `scenes/boot.tscn` + `scripts/Boot.cs` is a placeholder main scene for the export test; replace in M3.
- **Android needs Gradle builds** to keep net10.0: the non-Gradle template only supports net9.0 (owner chose "try Gradle, keep net10.0" on 2026-11-08; it works). The Gradle template lives in git-ignored `android/build/` (extract `android_source.zip` from the export templates there and write `android/.build_version` containing `4.7.2.stable.mono`; do this again after a Godot update). `project.godot` has `textures/vram_compression/import_etc2_astc=true`, required for Android export. Debug APK is 570 MB until M10.3 compresses the art.
- Android export: same headless command with preset `"Android"` and `build/android/Everkin.apk`.
- Do not rewrite `export_presets.cfg` with PowerShell `Set-Content -Encoding utf8` (adds a BOM and Godot then sees no presets).
- The renderer is already set to Compatibility in project.godot (done in the spec phase, 2026-11-08).

## Task list

- [x] M0.1 Solution and projects
- [x] M0.2 Godot C# project referencing the core
- [x] M0.3 Android and Windows export test
- [x] M0.4 Build and test commands in CLAUDE.md
- [x] M1.1 Content pipeline and validation
- [x] M1.2 Battle state, random generator, events
- [x] M1.3 Formation
- [x] M1.4 Timeline
- [ ] M1.5 Momentum
- [ ] M1.6 Ranges and targeting
- [ ] M1.7 Damage, healing, shields
- [ ] M1.8 Interception and area patterns
- [ ] M1.9 Movement
- [x] M1.10 Stealth, KO, revival
- [x] M1.11 Statuses and airborne
- [x] M1.12 Reactions
- [x] M1.13 Summons and undead
- [x] M1.14 Ground effects and battle end
- [x] M1.15 Skill trees and builds
- [x] M1.16 Attack Type attacks
- [x] M1.16a Hunter skills
- [x] M1.16b Guardian skills
- [x] M1.16c Healer skills
- [x] M1.16d Sorcerer skills
- [x] M1.16e Elementalist skills
- [x] M1.16f Warrior skills
- [x] M1.16g Leader skills
- [x] M1.16h Rogue skills
- [x] M1.16i Tinker skills
- [x] M1.16j Summoner skills
- [x] M1.16k Primalist skills
- [x] M1.16l Flying skills
- [x] M1.17 All §20.5 worked checks pass
- [x] M1.18 Preview queries
- [ ] M2.1 AI search
- [ ] M2.2 Batch runner
- [ ] M2.3 First 10,000-battle run reviewed
- [ ] M3.1 Card scene
- [ ] M3.2 Battle screen layout
- [ ] M3.3 Event animation
- [ ] M3.4 Viewer controls
- [ ] M4.1 Player turn
- [ ] M4.2 Team editor and collection
- [ ] M4.3 Skill tree editor
- [ ] M4.4 Free battle
- [ ] M5.1 Profile and saving
- [ ] M5.2 World and chapter maps
- [ ] M5.3 Encounters, rewards, tutorial
- [ ] M5.4 Chapter 1
- [ ] M6.1 Local two-player
- [ ] M7.1 Chapters 2 to 4
- [ ] M7.2 Achievements
- [ ] M8.1 Random battles
- [ ] M8.2 Challenge encounters
- [ ] M8.3 Coin creatures
- [ ] M9.1 Battle service
- [ ] M9.2 Spring Boot server
- [ ] M9.3 Online client
- [ ] M9.4 Deploy
- [ ] M10.1 Settings and accessibility
- [ ] M10.2 Crash reports
- [ ] M10.3 Mobile art and download size
- [ ] M10.4 Store builds and demo

## Session log

One line per finished task: date, task, commit.
2026-11-08 M0.1 Solution and projects (see git log)
2026-11-08 M0.2 Godot C# project (see git log)
2026-11-08 M0.3 Export test, M0.4 Commands in CLAUDE.md (see git log)
2026-11-08 M1.1 Content pipeline and validation (see git log)
2026-11-08 M1.2 Battle state, random generator, events (see git log)
2026-11-08 M1.3 Formation (see git log)
2026-11-08 M1.4 Timeline (see git log)
