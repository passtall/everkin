# Implementation plan

Progress tracking for building Everkin (adopted 2026-11-08, production.md §3). Milestones follow the roadmap in production.md §3. Tick a box in the same commit as the work. How to work is in [CLAUDE.md](../CLAUDE.md).

## 0. Setup

- [x] Renderer set to Compatibility in project.godot (done in the spec phase, 2026-11-08)
- [ ] .NET solution in `src/`: rules core library, batch runner, xUnit test project; nullable on, warnings as errors
- [ ] Godot C# project builds and opens; the solution references the core
- [ ] Early export test: an Android build of the C# project runs on a phone (production.md §1.1); Windows build runs
- [ ] Fill in the build and test commands in CLAUDE.md

## 1. Rules core

- [ ] Content JSON for creatures, classes, skills and statuses, with a test that checks it against the document tables (creature-classes.md, class_system.md, skills.md)
- [ ] Battle state with fast `Clone()`, seeded random generator, event list per action
- [ ] Timeline, Delay, Speed, ties (game_system §6)
- [ ] Momentum (§6.7)
- [ ] Formation, rows, distance and own-side range (§7)
- [ ] Targeting, interception, area patterns (§8)
- [ ] Damage, healing, shields, Power scaling (§5.2)
- [ ] Movement and forced repositioning (§9)
- [ ] Stealth, KO, revival, statuses (§10)
- [ ] Summons and undead (§11)
- [ ] Ground effects and battle end (§12)
- [ ] Skill trees and builds: 10 points, prerequisites, random legal builds, default builds (§4.1)
- [ ] Every skill in skills.md implemented
- [ ] Every worked check in game_system §20.5 is a passing test
- [ ] Preview queries (damage range, interception chance, timeline position, forced movement)

## 2. AI and AI-versus-AI harness

- [ ] AI search: alpha-beta minimax with iterative deepening, scoring from §2.5, difficulty levels
- [ ] Batch runner: random teams and formations, parallel battles, run folder with snapshot, CSV and Markdown report (§2.5)
- [ ] First 10,000-battle run on the owner's PC; report reviewed with the owner

## 3. Battle viewer

- [ ] Card scene (full, compact, back) with real art, frames and backgrounds
- [ ] Battle screen layout from ui_spec.md, playing core events from a queue
- [ ] Viewer controls: pause, step, speed, inspect, start from a recorded setup

## 4. Free battle against the AI

- [ ] Player input: skills, targeting, previews, Move, Skip Turn
- [ ] Team editor and collection, skill tree editor
- [ ] Free battle setup and results

## 5. Tutorial and chapter 1

- [ ] Profile and saving (production.md §1.4)
- [ ] Campaign map, tutorial, chapter 1 encounters (needs docs/campaign.md)
- [ ] Coins, prices, rewards

## 6. Local two-player

- [ ] Hot-seat battles with the board rotated 180° for player 2

## 7. Rest of the campaign

- [ ] Chapters 2 to 4, bosses, achievements, cosmetic frames

## 8. Post-story content

- [ ] Random battles, challenge encounters, coin creatures

## 9. Online play

- [ ] Spring Boot server in `server/`, battle service in `src/`, PostgreSQL, staging on Hetzner (production.md §2.4)
- [ ] Sign-in, profile sync, matchmaking, online battles, reconnect

## 10. Release

- [ ] Settings, accessibility, audio, vibration, Sentry crash reports
- [ ] Mobile art compression and download size check (under 200 MB)
- [ ] Store builds: Windows (Steam), Android; iOS once a Mac is available

## Questions for the owner

(None yet.)

## Notes

(Things the next session needs to know.)
