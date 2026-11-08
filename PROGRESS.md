# Progress

The implementation state, kept current by every Claude Code session ([docs/implementation-plan.md](docs/implementation-plan.md) says how). If a session stops at any moment, the next one continues from here.

**Last updated:** 2026-11-08, M0.2 done.

## Current task

None between tasks. Next is **M0.3**.

## Next step

M0.3: export test. Android needs the Android SDK/JDK and export templates; Windows needs export templates. Ask the owner for a phone or emulator.

## In flight

Nothing. (List uncommitted files, half-done parts and anything known to be broken here.)

## Questions for the owner

None yet. (Add each open question with the task it blocks and the doc section it concerns.)

## Notes

- M0.1 uses net10.0 (newest LTS installed, SDK 10.0.301). Verified in M0.2: Godot.NET.Sdk 4.7.2 builds the root project on net10.0 (the editor itself runs on net8 with rollForward).
- Godot 4.7.2 mono is at `E:\Downloads\Neuer Ordner\Godot_v4.7.2-stable_mono_win64\` (use the `_console.exe` for headless: `--headless --path . --build-solutions --quit`). Running it once adds the NuGet fallback so plain `dotnet build` works at the root. `.import`/`.uid` files are committed (Godot convention).
- Core and runner have TreatWarningsAsErrors; tests do not. `dotnet build` / `dotnet test` at the root work.
- The renderer is already set to Compatibility in project.godot (done in the spec phase, 2026-11-08).

## Task list

- [x] M0.1 Solution and projects
- [x] M0.2 Godot C# project referencing the core
- [ ] M0.3 Android and Windows export test
- [ ] M0.4 Build and test commands in CLAUDE.md
- [ ] M1.1 Content pipeline and validation
- [ ] M1.2 Battle state, random generator, events
- [ ] M1.3 Formation
- [ ] M1.4 Timeline
- [ ] M1.5 Momentum
- [ ] M1.6 Ranges and targeting
- [ ] M1.7 Damage, healing, shields
- [ ] M1.8 Interception and area patterns
- [ ] M1.9 Movement
- [ ] M1.10 Stealth, KO, revival
- [ ] M1.11 Statuses and airborne
- [ ] M1.12 Reactions
- [ ] M1.13 Summons and undead
- [ ] M1.14 Ground effects and battle end
- [ ] M1.15 Skill trees and builds
- [ ] M1.16 Attack Type attacks
- [ ] M1.16a Hunter skills
- [ ] M1.16b Guardian skills
- [ ] M1.16c Healer skills
- [ ] M1.16d Sorcerer skills
- [ ] M1.16e Elementalist skills
- [ ] M1.16f Warrior skills
- [ ] M1.16g Leader skills
- [ ] M1.16h Rogue skills
- [ ] M1.16i Tinker skills
- [ ] M1.16j Summoner skills
- [ ] M1.16k Primalist skills
- [ ] M1.16l Flying skills
- [ ] M1.17 All §20.5 worked checks pass
- [ ] M1.18 Preview queries
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
