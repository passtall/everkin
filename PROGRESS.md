# Progress

The implementation state, kept current by every Claude Code session ([docs/implementation-plan.md](docs/implementation-plan.md) says how). If a session stops at any moment, the next one continues from here.

**Last updated:** 2026-11-08, M1.12 done.

## Current task

None between tasks. Next is **M1.13**.

## Next step

M1.13 summons, constructs, totems, traps, undead: read game_system §11 (11.1 kinds, 11.2 operating rules, 11.3 capacity 18, 11.5 undead) and the §20.5 summon / trap / construct / undead rows. Summons are units (own stats, Speed, HP, their own skill text, damage scaling with the summoner's Power, lifetimes, first turn 11 - Speed later, KO'd summons removed at once, traps removed and slot freed, the summoner KO rules) added to BattleState mid-battle; placement is not targeting.

## In flight

Nothing.

## Questions for the owner

1. **Stale text, no task blocked (game_system §2.5, line "Each unit gets a random legal build: its 10 skill points").** §4.1 says 8 skill points (changed 2026-11-08), and skills.md and the plan say 8. I use 8; please fix §2.5 (or tell me 10 is meant).
2. **Timeline gaps, no task blocked (game_system §6.1).** I filled these with the most literal reading; tell me if any is wrong: (a) Haste or Slow that would take the remaining Wait below 0 makes the turn arrive now, like the Pull Forward overshoot rule; (b) Speed changes move the remaining Wait by the change in Speed as the formula sees it (clamped to 1-10), so Haste +2 at Speed 9 shortens it by 1; (c) Reset uses the Wait the entry was scheduled with and ignores later Haste or Slow; (d) an entry that is pulled or pushed but stays in the same time unit keeps its place in the tie order; (e) a new Wait (turn, auto event, delayed event, effect end) is at least 1.
3. **Attack Type attack ranges, blocks M1.16 only (skills.md, nine attacks).** The table gives no range for Brawl Strike, Blade Strike, Heavy Blow and Breath; Shot and Bolt are "any visible enemy", Pulse is "every enemy in melee reach", and Polearm Thrust and Lash have "one row more reach than other melee". I need the numbers: is base melee reach D1 (Polearm and Whip D2), and what is Breath's range (it is `direct`)? Not needed before M1.16.
4. **"Behind" in the behind rule, blocks M1.16 only (game_system §8.4, skills.md Piercing Enchant, Phasing / projectile at a stealthed target).** §8.4 says a skill that hits units behind its target hits those at 100% overlap automatically and those at 50% with a 50% chance, but not how many rows count: only the row directly behind, every row behind, or the next row that has anyone. skills.md says "the unit behind its target" (singular). `AreaPatterns.UnitsBehind` lists every row behind with its distance, so the skill can choose; tell me which rule is meant.
5. **Movement edges, no task blocked (game_system §9.5).** Read literally and implemented; tell me if any is wrong: (a) a moved unit that overlaps one end unit of the destination row by 50% (no unit on the other side) is inserted at that end, like any 50% case; (b) a pull bumps along the direction of the pull (toward the Front), so a pulled unit landing on a Front unit at 100% overlap is blocked at the formation boundary and everyone in the chain takes 1; (c) a KO body in a chain blocks it but takes no bump damage (it has 0 HP); an Immobilized unit blocks it and does take the damage; (d) a forced move of several rows only runs the bump chain in the destination row and beyond, not in the rows it passes; (e) a unit that cannot be moved at all (KO body, Immobilized) is not a legal target: nothing happens and no damage is dealt.
6. **Stunned and the turn, blocks the battle driver only (game_system §10.5).** "Stunned: the unit takes no actions and cannot intercept; the timeline keeps running; any hit ends it early." It does not say whether a Stunned unit's turn arrives and is skipped at the skipped-turn cost like Frozen's, or what else happens. `Statuses.OnTurnStart` reports `Stunned` and leaves the cost to the driver; tell me the rule (my guess: the turn is skipped at the Delay 5 cost).
7. **Small status gaps, none block a task.** (a) Airborne has no polarity in the docs, so Absolve / Dispel do not touch it (`Polarity.Unclassified`); (b) Poison that has built up but not landed at the moment it expires lands its whole points at that moment and drops the fraction (so 50 time units give 5 in total); (c) Haste and Slow are statuses `haste` / `slow` with a Speed-step magnitude (the table has none; §6.1 needs them for expiry); (d) Stealth stays a plain flag with no duration unless a skill adds a timer.

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
- M1.5: `Momentum` (on `BattleState.Momentum`; change it through `state.PayMomentum(side, cost)` / `state.ShiftMomentum(side, amount)`, which emit `MomentumChanged` only when the meter moves). The meter is stored from Side A's view (positive favors A); `AdvantageOf(side)` gives either view. `CanPay`/`Pay` (needs a lead of N, moves N toward neutral, never refunded), `Shift` clamps after each shift. `MomentumRules.HitShift(HitOutcome)` and `ActionShift(hits)` give the per-hit and per-action shift from the actor's view (Landed +1, Missed -1, rest 0; one shift per action: any Landed = +1, else any Missed = -1). The skill code (M1.7 onwards) must pay the cost first, then shift per event, using the hit outcomes it sees; skills or statuses that modify a shift (e.g. double for Marked) change the amount before calling `ShiftMomentum`.
- M1.6: `Targeting` (static, read-only): `Reach` (max distance or `Reach.Unlimited`), `TargetOptions(Side, IncludeKnockedOut)`; `EffectiveRow`, `OpposingDistance` (attacker row + target row - 1 after skipping rows with no living unit; on the other side stealthed units and KO bodies do not make a row count), `InReach` (own side: D1 = user + adjacent, D2+ = any), `IsLegalTarget`, `LegalTargets` (own side first), `NearestEnemies`, `PickNearest` (ties by the battle's seeded roll; no roll for a single candidate). `Unit.IsKnockedOut` (HP 0) and `Unit.IsStealthed` (plain flag, M1.10 maintains it) were added for it. Not here: interception (M1.8), placement of summons (M1.13), confusion (M1.11).
- M1.7: `DamageMath` (static, integers): `Scale(value, power)` = value x power / 5 rounded down, minimum 1; `ScaleRange`; `Roll(random, range)`; `Resolve(incoming, ward, dampen, source)` (Ward first, then Dampen halves rounded down, minimum 1 if anything passed the shield); `CombineWard`; `HealedHp`. `DamageSource`: Hit (shield + reductions, also burning ground and counters), Periodic (Poison, Bleeding: reductions only), Unreducible (bump). `BattleState.DealDamage(source, target, amount, source)`, `Heal`, `ApplyWard` mutate and emit `DamageDealt` / `Healed` / `WardChanged`; a KO unit cannot be damaged or healed (callers filter; revival is M1.10). `Unit.Ward` (int) and `Unit.Dampen` (bool) are plain values for now: M1.11 adds durations, expiry (timeline EffectEnd) and removal on KO. Not done here: the skill's Power scaling is applied by the caller (`ScaleRange` then `Roll`), Momentum shift from `DamageResult.FullyAbsorbed` / landed, KO check after damage (M1.10), the 'shared roll or independent rolls' choice (each skill).
- M1.8: `Interception` (static): `CanBeIntercepted(delivery, ignores)` (only melee and projectile), `IsEligible(unit)` (not KO, not stealthed, not `Unit.InterceptionBlocked`, which the status rules M1.11 set for Stunned / Frozen / Immobilized), `ChanceFor(overlap)` (50 / 25), `Defenders`, `Resolve(state, target, amount, splittable)` -> `InterceptionResult(Shares, Remaining)`: rows nearest the line first, random order inside a row (Fisher-Yates from the battle generator), per defender one roll for the interception chance and, if it intercepts and the effect can be split (amount >= 2), one 50% roll for full or partial (half rounded down); emits `Intercepted`. The CALLER applies each share (damage to the interceptor through its own Ward / Dampen, a heal, a status) and then `Remaining` to the target. Not modelled yet: skill overrides (Shield Wall, Living Wall, Full Cover, Leap to Defend, Phasing Strike's ignore is just the flag), area skills that are melee or projectile (§8.2 says they can be intercepted; how it works per affected unit is not specified: ask when the first area skill in M1.16 needs it), Momentum from interception (0). `AreaPatterns`: `Circular` (target first), `Row`, `AllOf` (KO bodies never, stealthed included), `UnitsBehind`, `HitBehind` (100% auto, 50% with a 50% roll). `chain` and `column` are left to the skills.
- M1.9: `Movement` (static): `LegalMoves(state, unit)` (every gap of every other non-full row of the own side, empty for KO / Immobilized), `Move(state, unit, row, gap)` (Voluntary; Delay 5 = `Movement.MoveDelay`, the caller schedules the turn and counts the `move` tag for Bleeding), `ForceMove(state, unit, destinationRow)` (push = toward the Rear, pull = toward the Front; realistic landing by half-card coordinate; 100% overlap bumps the occupant onward in the same direction; 50% / none inserts or appends; a blocked chain moves nothing and deals `Movement.BumpDamage` = 1 as `DamageSource.Unreducible` to the moved unit, the pressed units and the blocker, ends their stealth, emits `BumpBlocked`; returns `ForcedMoveResult` with the moves or the damaged units so the caller can run 'when hit' reactions and KO checks), `Swap(state, a, b)` (same side, fails with bodies and Immobilized). `Unit.IsImmobilized` is a plain flag for M1.11 to maintain. Events: `UnitMoved` (Voluntary / Forced), `UnitsSwapped`, `BumpBlocked`. Forced-movement immunity (Hold Position) and 'when moved' reactions are skill / reaction work (M1.12, M1.16). Bump damage does not shift Momentum (the caller simply does not shift).
- M1.10: `Knockout.Resolve(state, unit, keepPendingEvents)` (0 HP, not yet processed: breaks stealth, `Unit.ClearStatuses()`, removes the unit's turn and its auto / delayed events unless kept, emits `UnitKnockedOut`; idempotent through `Unit.KoProcessed`), `ResolveAll(state)` (the KO check after an event or simultaneous block; random order from the battle generator when several fall), `Revive(state, unit, hp, delay)` (HP clamped 1..max, no free turn: next turn at `Wait(delay, Speed)` from now, `UnitRevived`; repeatable). `Stealth.CanGain` (alive, not stealthed, another living non-stealthed unit on the side), `Gain`, `Break(state, unit, cause)` with causes Gained / Attacked / Hit / Revealed / KnockedOut and `StealthChanged`. `DealDamage` does not touch stealth: the action code calls `Stealth.Break(..., Hit)` for hits (bump damage already does) and `Attacked` for attackers; Poison and Bleeding ticks must not. Not done here: removal of KO'd summons and traps (M1.13), victory (M1.14), EffectEnd markers of cleared statuses (M1.11: they will need an owner that is the unit the status sits on), 'when KO'd' triggers (M1.12).
- M1.11: `Statuses` (static) + `StatusInstance` on `Unit.StatusInstances` (one per status type) + `StatusRules` (reapply rule, polarity, Speed modifier) + `StatusIds`. `Apply(state, StatusApplication(unit, id, Duration, Magnitude, Source, EndsAtNextTurnOf, Uses))`: replace on reapply (even if shorter), except Stunned keeps the longer time and Ward the larger amount (returns false when the old one stays); a clock status has an `EffectEnd` marker on the timeline (`HandleExpiry(state, poppedEntry)` ends it; the driver must call it for every popped EffectEnd). `Remove(state, unit, id, cause)`, `RemoveAll(state, unit, polarity)`, `ClearAll` (KO; Knockout calls it). Flags on `Unit` kept in step by the status layer: `Ward`, `Dampen`, `IsImmobilized`, `InterceptionBlocked` (Stunned / Frozen / Immobilized), `IsAirborne`; `Unit.EffectiveSpeed` = Speed + Haste - Slow - Chilled 2, and Apply / Remove call `Timeline.ChangeSpeed` (so ScheduleTurn must be given `EffectiveSpeed`). Driver hooks: `OnTurnStart(state, unit)` (ends 'until its next turn' statuses; Frozen consumed = turn skipped), `TickPoison(state)` (call when any turn begins; lands whole points, fraction carries; Periodic damage: no Ward, Dampen applies, no stealth break, no Momentum), `OnSkillUsed(state, unit, melee, move)` (Bleeding 2 damage per melee / move skill, counts down), `OnHit(state, unit)` (ends Stunned), `AttackMissChance`, `CanUseMagical`, `RollConfusion`. `BattleState.DealDamage` removes a Ward status when the Ward is spent. `TargetOptions.Delivery`: melee cannot target an airborne unit unless the user is airborne. content/statuses.json (10 named statuses) is generated by tools/content from game_system §10.5 and loaded as `ContentSet.Statuses` (`ContentLoader.LoadFromJson` now takes 4 JSON strings). Not done here: ground effects and 'Fire ends Frozen' (M1.14 calls `Remove(..., Fire)`), skills that apply statuses (M1.16), 'cannot become airborne' (Curse of Weight, skill), always-airborne keystone (Sky Lord).
- M1.12: `Reactions.Resolve(state, triggers, maxReactions)` with `ReactionDef` (owner, `TriggerKind` WhenHit / WhenKo / WhenMoved, `ReactionPriority` First / Normal / Last, `RequiredTags` (`EffectTags` Melee, Projectile, Direct, Physical, Magical, Move), `MaxUsesPerBattle`, `CanChain`, `Condition`, `Effect(state, ctx)`) registered with `BattleState.AddReaction`; uses are counted in the state (`ReactionUses`, cloned). The CALLER raises triggers: after each event or simultaneous block the skill executor builds the `Trigger`s (hit units with the hit's tags, KO'd units from `Knockout.ResolveAll`, moved units from `ForceMove`; a bump hit is a WhenHit) and calls `Resolve`. Order: priority level, then the owner's turn position on the timeline (Due, join Order; owners without a turn last), then trigger order, then registration order. A KO'd owner (HP 0) is skipped at the moment it would react, except for WhenKo reactions; this is rechecked when the reaction resolves, so a unit KO'd by an earlier reaction of the same batch does not react. A reaction's own hits go straight through `DealDamage` (no interception, Ward / Dampen apply) and the effect reports them with `ctx.Raise(new Trigger(WhenHit, target, owner, ownTags))`, which resolves right after that reaction, before the batch's next reaction (depth first; this reading of 'resolve after each event' is mine). `CanChain = false` swallows what the reaction raises. The limit (`DefaultMaxReactions` = 1000) only stops an endless chain: `ReactionResult.Aborted` and a `ReactionChainAborted` event; the harness (M2.2) must count and report these. Momentum for a reaction's hit is the effect's job (`MomentumRules`, section 6.7).
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
- [x] M1.5 Momentum
- [x] M1.6 Ranges and targeting
- [x] M1.7 Damage, healing, shields
- [x] M1.8 Interception and area patterns
- [x] M1.9 Movement
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
2026-11-08 M1.5 Momentum (see git log)
2026-11-08 M1.6 Ranges and targeting (see git log)
2026-11-08 M1.7 Damage, healing and shields (see git log)
2026-11-08 M1.8 Interception and area patterns (see git log)
2026-11-08 M1.9 Movement (see git log)
2026-11-08 M1.10 Stealth, KO, bodies, revival (see git log)
2026-11-08 M1.11 Statuses and airborne (see git log)
2026-11-08 M1.12 Reactions (see git log)
