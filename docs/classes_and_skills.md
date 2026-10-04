# Everkin — Classes and Skills

**Status:** Redesigned. **Healer** is decided by the owner. **Leader**: concept decided, skills Draft. **Hunter** and **Guardian**: restored from the owner-reviewed v1 and converted to the Delay scale. **Warrior, Feral, Elementalist**: Draft (proposed by Claude, not yet reviewed). Each class section states its own status.
**Global rules:** All game-wide rules live in [game_system.md](game_system.md): skill card fields and tags (§4.7), damage (§5.2), timeline, Speed and Delay (§6.1), Momentum (§6.7), interception (§8.2), columns and shapes (§8.4). This document holds only class and skill definitions, never global rules.
**Process:** One class at a time, with the owner, who decides everything. Order: Healer, Leader, Hunter, Guardian, Warrior, Feral, Elementalist.
**Labels:** **TBD** = not decided. Everything written here is decided by the owner.

## Card templates

```
### <Skill name>
Class: · Tree slot: <branch letter + number>
Delay: <1–10, 1 quickest> · Momentum cost: <0–10>
Tags: `tag` `tag`
Mechanic: <exact rules, written with tags>
Reach: <rows/distance>
Hidden stats: <coefficients or other designer-only numbers>
```

```
### <Node name>  [Passive | Modifier of <skill> | Keystone]
Class: · Tree slot: · Prerequisite:
Trigger: <when it fires>
Effect: <what changes in how the game works>
```

## 1. Healer

**Concept (decided):** Triage. Strong single-target heals, so positioning and who to save matter most. Heals are `direct`, so enemies cannot intercept them. Every skill has a target type and a range (distance rules: game_system.md §7.4). Reaching far is paid for with Delay and Momentum. Heavy heals have high Delay.

- **Lanes:** Mending (heals), Rites (revive, cleanse), Ward (shields, prevention). 8 skills plus 6 meaningful nodes over 3 branches.
- **No class attacks.** Only the creature's type-specific attack.
- **Momentum:** healing does not move the meter.
- **Main single heal (Mend):** a heavy heal, 18–22 for the reference human (Magic Power 5), Delay 7.
- **Revive:** one skill. Momentum cost 5, Delay 9, target returns at 25% HP.
- **Cleanse:** one skill that removes one chosen status from an ally.
- **No turn manipulation:** the Healer has no skill that pulls forward, pushes back or otherwise changes other units' turn timing.
- **Ranges** are the maximum distance (game_system.md §7.4). Numbers were set by Claude and are for later balancing. Any modifier that makes a skill stronger carries a cost.

### Branch A: Mending

### Mend
Class: Healer · Tree slot: A1
Delay: 7 · Momentum cost: 0
Tags: `heal` `direct` `single`
Mechanic: Heals one ally by 18–22.
Reach: Range 2.
Hidden stats: Healing coefficient 3.6–4.4.

### Renew
Class: Healer · Tree slot: A2
Delay: 4 · Momentum cost: 0
Tags: `heal` `status` `direct` `single`
Mechanic: The ally regains 3 HP at the start of each of its next 3 turns.
Reach: Range 2.
Hidden stats: Healing coefficient 0.6 per tick.

### Rain of Mending
Class: Healer · Tree slot: A3
Delay: 6 · Momentum cost: 2
Tags: `heal` `direct` `row`
Mechanic: Select an ally row. Every ally in it is healed by 4–6.
Reach: Range 2, measured to the nearest ally in the row.
Hidden stats: Healing coefficient 0.8–1.2.

### Renew Bloom  [Modifier of Renew]
Class: Healer · Tree slot: A4 · Prerequisite: Renew
Trigger: Renew's last tick lands.
Effect: Removes one status from the ally, chosen when Renew is cast. Cost: Renew's Delay is +1 (5).

### Branch B: Rites

### Purify
Class: Healer · Tree slot: B1
Delay: 4 · Momentum cost: 1
Tags: `status` `direct` `single`
Mechanic: Removes one chosen status from an ally. The ally cannot receive that status again until its next turn.
Reach: Range 2.
Hidden stats: none.

### Revive
Class: Healer · Tree slot: B2
Delay: 9 · Momentum cost: 5
Tags: `heal` `direct` `single`
Mechanic: Revives a KO ally at 25% of its maximum HP (rounded down, minimum 1). The scheduling rule in game_system.md §10.3 applies to the revived unit (it waits as though it had cast Revive, Delay 9).
Reach: Range 1.
Hidden stats: Revival HP 25% of maximum.

### Resolute Rites  [Modifier of Revive]
Class: Healer · Tree slot: B3 · Prerequisite: Revive
Trigger: Revive resolves.
Effect: The revived unit returns with a Ward absorbing up to 4 damage from its next hit. Cost: Revive's Momentum cost is +1 (6).

### Steady Hands  [Passive]
Class: Healer · Tree slot: B4 · Prerequisite: none
Trigger: The Healer is the target of a push, pull or swap.
Effect: The movement fails. The Healer stays in place.

### Branch C: Ward

### Ward
Class: Healer · Tree slot: C1
Delay: 5 · Momentum cost: 0
Tags: `status` `cover` `direct` `single`
Mechanic: Until the Healer's next turn, the next hit on the ally is reduced by up to 8 damage. Any remainder goes through.
Reach: Range 2.
Hidden stats: Absorb amount 8.

### Dampen
Class: Healer · Tree slot: C2
Delay: 3 · Momentum cost: 0
Tags: `status` `cover` `direct` `single`
Mechanic: Until the Healer's next turn, the next hit on the ally deals half damage (rounded down, minimum 1).
Reach: Range 1.
Hidden stats: Reduction 50%.

### Last Stand
Class: Healer · Tree slot: C3
Delay: 6 · Momentum cost: 4
Tags: `status` `direct` `single`
Mechanic: The next time the ally would be KO, it stays at 1 HP instead. Lasts until used.
Reach: Range 2.
Hidden stats: none.

### Spent Ward  [Modifier of Ward]
Class: Healer · Tree slot: C4 · Prerequisite: Ward
Trigger: A Warded hit is reduced by the Ward.
Effect: That hit moves Momentum by 0. Cost: Ward's Delay is +1 (6).

### Expand Ward  [Modifier of Ward]
Class: Healer · Tree slot: C5 · Prerequisite: Ward
Trigger: Casting Ward.
Effect: Ward gains the `row` tag. Select an ally row; each unit in it is Warded for up to 4. Cost: Ward's Delay is +2 (7) and its Momentum cost is 2.

### Shared Stand  [Modifier of Last Stand]
Class: Healer · Tree slot: C6 · Prerequisite: Last Stand
Trigger: Casting Last Stand.
Effect: The Healer also gains the effect. Cost: Last Stand's Delay is +2 (8).

### TBD

- Whether Ward and Dampen can both be on one unit (and which applies first).
- Exact node tree layout and costs (owner sorts manually at the end).

## 2. Leader

**Concept (decided):** Commander. It coordinates the party and does not heal or attack. The Leader has no class attacks (only the creature's type-specific attack). It owns **timeline manipulation** (pulling allies' turns forward, pushing enemies' back). It both spends and generates Momentum. Its skills are mostly `single` ally targets, plus one `row`, one `all` and `circular` through nodes.

**Class rule (decided):** **Order statuses** (Focus Target, Far Reach, Rally Cry) end as soon as the Leader takes damage, so the Leader must be protected.

**Skills below are Draft** (proposed by Claude from the decided concept).

### Branch A: Orders

### Focus Target
Class: Leader · Tree slot: A1
Delay: 4 · Momentum cost: 0
Tags: `status` `direct` `single`
Mechanic: Choose an enemy. Until the Leader's next turn, every allied hit on it deals +2 damage. This is an Order.
Reach: Range 3 (enemy distance, game_system.md §7.4).
Hidden stats: Bonus damage 2.

### Forward!
Class: Leader · Tree slot: A2
Delay: 5 · Momentum cost: 2
Tags: `timeline` `direct` `single`
Mechanic: Pulls one ally's next turn forward by 3 time units.
Reach: Range 3.
Hidden stats: Pull 3.

### Hold!
Class: Leader · Tree slot: A3
Delay: 5 · Momentum cost: 2
Tags: `timeline` `direct` `single`
Mechanic: Pushes one enemy's next turn back by 3 time units.
Reach: Range 3 (enemy distance).
Hidden stats: Push 3.

### Chain of Command  [Modifier of Forward!]
Class: Leader · Tree slot: A4 · Prerequisite: Forward!
Trigger: Casting Forward!
Effect: Forward! gains `circular`: the target and its adjacent allies are each pulled forward by 2 instead. Cost: Forward!'s Delay is +2 (7) and its Momentum cost is +1 (3).

### Unbroken Orders  [Modifier of Focus Target]
Class: Leader · Tree slot: A5 · Prerequisite: Focus Target
Trigger: The Leader takes damage while Focus Target is active.
Effect: Focus Target ends only if the Leader is KO. Cost: Focus Target's Delay is +2 (6) and its Momentum cost is 1.

### Branch B: Formation

### Redeploy
Class: Leader · Tree slot: B1
Delay: 3 · Momentum cost: 0
Tags: `move` `direct` `single`
Mechanic: Moves one ally one row forward or back (chosen when cast). Fails if the destination row is full or the ally is already in the outermost row in that direction.
Reach: Range 2.
Hidden stats: none.

### Far Reach
Class: Leader · Tree slot: B2
Delay: 4 · Momentum cost: 1
Tags: `status` `direct` `single`
Mechanic: The ally's next skill has +2 range. This is an Order.
Reach: Range 3.
Hidden stats: Range bonus 2.

### Ever Forward  [Modifier of Redeploy]
Class: Leader · Tree slot: B3 · Prerequisite: Redeploy
Trigger: Redeploy moves an ally forward.
Effect: That ally's next turn is pulled forward by 1 time unit. Cost: Redeploy's Delay is +1 (4).

### Wide Reach  [Modifier of Far Reach]
Class: Leader · Tree slot: B4 · Prerequisite: Far Reach
Trigger: Casting Far Reach.
Effect: Far Reach gains `circular`: the target and its adjacent allies each get +2 range on their next skill. Cost: Far Reach's Delay is +2 (6).

### Branch C: Rally

### Rally Cry
Class: Leader · Tree slot: C1
Delay: 6 · Momentum cost: 0
Tags: `status` `direct` `row`
Mechanic: Select an ally row. The next hit that each ally in it lands moves Momentum by 2 instead of 1 (until the Leader's next turn). This is an Order.
Reach: Range 2, measured to the nearest ally in the row.
Hidden stats: Momentum shift 2.

### Battle Cry
Class: Leader · Tree slot: C2
Delay: 7 · Momentum cost: 0
Tags: `momentum`
Mechanic: Moves the Momentum meter 2 toward the Leader's side.
Reach: Self.
Hidden stats: Shift 2.

### Surge
Class: Leader · Tree slot: C3
Delay: 6 · Momentum cost: 6
Tags: `timeline` `all`
Mechanic: Pulls every ally's next turn forward by 2 time units.
Reach: All allies.
Hidden stats: Pull 2.

### Rousing Cry  [Modifier of Rally Cry]
Class: Leader · Tree slot: C4 · Prerequisite: Rally Cry
Trigger: Casting Rally Cry.
Effect: Rally Cry gains `all`: it affects every ally. Cost: Rally Cry's Delay is +2 (8) and its Momentum cost is 2.

## 3. Hunter

**Status:** Restored from the owner-reviewed v1 and converted to the Delay scale (old Very Fast/Fast/Normal/Slow = 2/3/4/6). Damage numbers are unchanged. The nodes marked Modifier (Double Tap, Snare, Quarry) are Draft.

A sniper, trapper and tracker. It uses Physical skills only. It has no special consequence for being in the Front row.

### Branch A: Marksman

### Sniper Shot
Class: Hunter · Tree slot: A1
Delay: 6 · Momentum cost: 0
Tags: `attack` `physical` `direct` `single`
Mechanic: Hits one visible enemy for 7–9. Ignores interception.
Reach: Any visible enemy.
Hidden stats: Coefficient 1.4–1.8.

### Rapid Shots
Class: Hunter · Tree slot: A2
Delay: 3 · Momentum cost: 0
Tags: `attack` `physical` `projectile` `single`
Mechanic: Shoots once now, then puts 2 more shots on the timeline as blocking auto events 2 and 4 time units later. The Hunter takes no turn until the last shot has resolved; its Delay then begins. Each shot hits for 2–3 and respects interception. If the target is KO, a shot hits the unit behind it. If the Hunter is KO, the remaining shots are removed.
Reach: Any visible enemy.
Hidden stats: Coefficient 0.4–0.6 per shot.

### Blind Shot
Class: Hunter · Tree slot: A3
Delay: 2 · Momentum cost: 0
Tags: `attack` `physical` `projectile` `random`
Mechanic: Hits one random living enemy for 1–5, including summons and stealthed units. KO bodies are excluded. Respects interception.
Reach: All enemies.
Hidden stats: Coefficient 0.2–1.0.

### Pierce
Class: Hunter · Tree slot: A4
Delay: 4 · Momentum cost: 3
Tags: `attack` `physical` `projectile` `column`
Mechanic: Hits the selected enemy and the units behind it in the same column (game_system.md §8.4) for 4–6 each. Stealthed enemies in the column can be hit.
Reach: Any visible enemy.
Hidden stats: Coefficient 0.8–1.2.

### Charged Shot
Class: Hunter · Tree slot: A5
Delay: 3 · Momentum cost: 4
Tags: `attack` `physical` `projectile` `delayed` `single`
Mechanic: Locks onto one enemy (as a unit). An auto event fires 10 time units later and hits it for 12–15, respecting interception. Enemy effects can delay or advance the event. It is removed if the Hunter is KO. If the target is KO when it fires, the shot hits the unit behind it.
Reach: Any visible enemy.
Hidden stats: Coefficient 2.4–3.0.

### Double Tap  [Modifier of Sniper Shot]
Class: Hunter · Tree slot: A6 · Prerequisite: Sniper Shot
Trigger: Casting Sniper Shot.
Effect: A second shot on the same target resolves as an auto event 3 time units later for half damage (rounded down, minimum 1), also ignoring interception. Cost: Sniper Shot's Delay is +2 (8).

### Branch B: Trapper

Traps are **summons that act as regular units**. Placed **between units** in one of your rows (recentering it, counting toward its 6-unit cap). A trap can only be placed in a row where one of your units is **directly behind** it, and never in front of the Front row. It takes one slot, has HP (0% Physical Defense), takes no turns, can be targeted, and **intercepts** like any unit for the 1–2 units directly behind it. There is no limit on the number of traps.

### Spike
Class: Hunter · Tree slot: B1
Delay: 6 · Momentum cost: 2
Tags: `summon`
Mechanic: Places a trap with HP 6. When hit by a `melee` attack, it deals 2 damage to the attacker.
Reach: Any of your own rows that allows placement.
Hidden stats: Counter damage 2 (fixed).

### Decoy
Class: Hunter · Tree slot: B2
Delay: 6 · Momentum cost: 3
Tags: `summon`
Mechanic: Places a trap with HP 12. It does nothing but intercept.
Reach: As Spike.
Hidden stats: none.

### Snare  [Modifier of Spike]
Class: Hunter · Tree slot: B3 · Prerequisite: Spike
Trigger: A `melee` attacker takes Spike's damage.
Effect: The attacker's next turn is pushed back by 2 time units. Cost: Spike's Momentum cost is +1 (3).

### Branch C: Tracker

Marks last until replaced. A Hunter can have **one of each Mark** active (one Track, one Bounty, one Pack Signal). Casting one again moves it.

### Track
Class: Hunter · Tree slot: C1
Delay: 6 · Momentum cost: 0
Tags: `status` `direct` `single`
Mechanic: The target cannot gain stealth.
Reach: Any visible enemy.
Hidden stats: none.

### Bounty
Class: Hunter · Tree slot: C2
Delay: 6 · Momentum cost: 0
Tags: `status` `direct` `single`
Mechanic: Hits on the target by any ally (including the Hunter) move Momentum by double.
Reach: Any visible enemy.
Hidden stats: Multiplier 2.

### Pack Signal
Class: Hunter · Tree slot: C3
Delay: 6 · Momentum cost: 0
Tags: `status` `direct` `single`
Mechanic: Whenever an ally other than the Hunter hits the marked enemy, one auto shot of 2–3 is queued as an auto event 2 time units later (up to 3 queued). Hits from your Spike traps count. Queued shots are removed if the Hunter is KO.
Reach: Any visible enemy.
Hidden stats: Coefficient 0.4–0.6.

### Quarry  [Modifier of Track]
Class: Hunter · Tree slot: C4 · Prerequisite: Track
Trigger: Casting Track.
Effect: A stealthed target is revealed (its stealth is removed). Cost: Track's Delay is +1 (7).

## 4. Guardian

**Status:** Restored from the owner-reviewed v1 and converted to the Delay scale. Ranges were converted to the §7.4 distance model: "same row or one row ahead/behind" is now range 2. These converted ranges are Draft.

The Guardian decides who gets hit. It changes interception, forces enemies to attack it, and repositions units. Its offense is tempo, not damage. Because an intercepted hit moves Momentum by 0, successful cover also denies the enemy Momentum. Durations generally last until the Guardian's next turn.

### Branch A: Bulwark

### Brace
Class: Guardian · Tree slot: A1
Delay: 3 · Momentum cost: 0
Tags: `status` `cover`
Mechanic: Until the Guardian's next turn, its chance to intercept doubles (100% overlap: 100%, 50% overlap: 50%). While Braced, the Guardian cannot be pushed, pulled or swapped.
Reach: Self.
Hidden stats: none.

### Shield Wall
Class: Guardian · Tree slot: A2
Delay: 6 · Momentum cost: 4
Tags: `status` `cover`
Mechanic: Needs 3 or more of your units next to each other in the Guardian's row, including the Guardian. Until the Guardian's next turn, `physical` attacks aimed at units in the rows behind the wall are blocked completely if the target overlaps a wall unit horizontally (no damage, Momentum 0). `magical` attacks are not affected. It ends early if a wall unit is KO or leaves the row. Skills that ignore interception ignore it.
Reach: Protects units in the rows behind the wall.
Hidden stats: none.

### Bodyguard
Class: Guardian · Tree slot: A3
Delay: 4 · Momentum cost: 2
Tags: `status` `cover` `direct` `single`
Mechanic: Choose an ally. Until the Guardian's next turn, single-target attacks aimed at that ally are redirected to the Guardian, which takes the full hit. The redirect only applies to attacks that could legally target the Guardian. It counts as interception. A new use replaces the previous target.
Reach: Range 2.
Hidden stats: none.

### Shield Bash
Class: Guardian · Tree slot: A4
Delay: 4 · Momentum cost: 0
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 3–5 and pushes its next turn back by 3 time units. Respects interception.
Reach: Range 2 (enemy distance).
Hidden stats: Coefficient 0.6–1.0.

### Branch B: Challenger

### Provoke
Class: Guardian · Tree slot: B1
Delay: 3 · Momentum cost: 1
Tags: `status` `direct` `single`
Mechanic: The target enemy is Provoked until it has taken its next turn: its `attack` skills must target the Guardian whenever the Guardian is a legal target. Interception still applies. It ends if the Guardian is KO.
Reach: Visible enemies, range 3.
Hidden stats: none.

### Haul
Class: Guardian · Tree slot: B2
Delay: 6 · Momentum cost: 2
Tags: `move` `direct` `single`
Mechanic: Moves an enemy one row forward. Fails if the destination row is full or the enemy is already in Front. Forced repositioning has its own rules (game_system.md §9.5).
Reach: Range 3.
Hidden stats: none.

### Shove
Class: Guardian · Tree slot: B3
Delay: 3 · Momentum cost: 0
Tags: `move` `direct` `single`
Mechanic: Moves an enemy one row back. Fails if the destination row is full or the enemy is already in Rear (§9.5).
Reach: Range 2.
Hidden stats: none.

### Hold the Line  [Passive]
Class: Guardian · Tree slot: B4 · Prerequisite: Provoke
Trigger: A Provoked enemy's attack damages the Guardian.
Effect: The Guardian's next turn is pulled forward by 2 time units (once per attack).

### Branch C: Warden

### Swap Places
Class: Guardian · Tree slot: C1
Delay: 3 · Momentum cost: 0
Tags: `move` `direct` `single`
Mechanic: Swaps positions with an ally. Both keep their ordered positions in the other's place, so no row recenters.
Reach: Range 2.
Hidden stats: none.

### Shared Burden
Class: Guardian · Tree slot: C2
Delay: 4 · Momentum cost: 2
Tags: `status` `cover` `direct` `single`
Mechanic: Choose an ally. Until the Guardian's next turn, the Guardian takes half of the damage the ally would take (half rounded down). A hit of 1 goes to the ally. It ends if either unit is KO or the ally leaves range.
Reach: Range 2.
Hidden stats: Share 50%.

### Shielded Step  [Passive]
Class: Guardian · Tree slot: C3 · Prerequisite: none
Trigger: The Guardian intercepts an attack aimed at an ally, fully or partially.
Effect: That ally's next turn is pulled forward by 2 time units (once per attack).

### TBD

- Forced repositioning details (landing position and more): game_system.md §9.5.
- Whether summons and traps count toward Shield Wall's three units.

## 5. Warrior

**Status:** All Draft (proposed by Claude). Identity: a Front-line bruiser that pays HP for power. It uses Physical skills only and has no special consequence for being in the Front row.

### Branch A: Strikes

### Cleave
Class: Warrior · Tree slot: A1
Delay: 4 · Momentum cost: 0
Tags: `attack` `physical` `melee` `circular`
Mechanic: Hits the selected enemy and the enemies adjacent to it for 3–5 each. Respects interception.
Reach: Range 2 (enemy distance).
Hidden stats: Coefficient 0.6–1.0.

### Heavy Slam
Class: Warrior · Tree slot: A2
Delay: 7 · Momentum cost: 0
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 10–13. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 2.0–2.6.

### Rend
Class: Warrior · Tree slot: A3
Delay: 4 · Momentum cost: 0
Tags: `attack` `physical` `melee` `status` `single`
Mechanic: Hits one enemy for 3–4 and applies Bleeding: it takes 1 damage at the start of each of its next 4 turns. Reapplying refreshes the duration.
Reach: Range 2.
Hidden stats: Coefficient 0.6–0.8.

### Whirl  [Modifier of Cleave]
Class: Warrior · Tree slot: A4 · Prerequisite: Cleave
Trigger: Casting Cleave.
Effect: Cleave gains `row`: select an enemy row and hit every unit in it. Cost: Cleave's Delay is +2 (6) and its Momentum cost is 1.

### Branch B: Fury

### Charge
Class: Warrior · Tree slot: B1
Delay: 4 · Momentum cost: 0
Tags: `attack` `physical` `melee` `move` `single`
Mechanic: If the Warrior is not in Front, it first moves to the Front row (if the row is full, the skill fails). It then hits one enemy for 4–6. Respects interception.
Reach: Range 3.
Hidden stats: Coefficient 0.8–1.2.

### Blood Rage
Class: Warrior · Tree slot: B2
Delay: 3 · Momentum cost: 0
Tags: `status`
Mechanic: The Warrior takes 3 damage (unusable at 3 HP or less). Its next `melee` attack deals +4 damage.
Reach: Self.
Hidden stats: Bonus damage 4.

### Parry
Class: Warrior · Tree slot: B3
Delay: 3 · Momentum cost: 0
Tags: `status` `melee`
Mechanic: Until the Warrior's next turn, the next `melee` hit on the Warrior is countered: the attacker takes 4 damage.
Reach: Self.
Hidden stats: Counter damage 4.

### Reckless Rage  [Modifier of Blood Rage]
Class: Warrior · Tree slot: B4 · Prerequisite: Blood Rage
Trigger: Casting Blood Rage.
Effect: The bonus applies to the next two `melee` attacks. Cost: Blood Rage costs 6 HP instead of 3 (unusable at 6 HP or less).

### Branch C: Breaking

### Execute
Class: Warrior · Tree slot: C1
Delay: 5 · Momentum cost: 3
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 6–8. Deals double damage if the target has half its maximum HP or less. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 1.2–1.6.

### Smash
Class: Warrior · Tree slot: C2
Delay: 6 · Momentum cost: 2
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 6–8. Deals double damage to summons and traps. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 1.2–1.6.

### Brutal Finish  [Modifier of Execute]
Class: Warrior · Tree slot: C3 · Prerequisite: Execute
Trigger: Execute KOs its target.
Effect: The Warrior's next turn is pulled forward by 3 time units. Cost: Execute's Momentum cost is +1 (4).

### Blood Scent  [Passive]
Class: Warrior · Tree slot: C4 · Prerequisite: none
Trigger: The Warrior's `melee` hit lands on a Bleeding enemy.
Effect: The Warrior's next turn is pulled forward by 1 time unit (once per attack).

## 6. Feral

**Status:** All Draft (proposed by Claude). Identity: a fast predator that uses stealth and tempo, with pack tactics. It uses Physical skills only. The quickest skills in the game (Delay 2) belong to the Feral.

### Branch A: Fangs

### Rake
Class: Feral · Tree slot: A1
Delay: 2 · Momentum cost: 0
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 2–4. Breaks the Feral's stealth. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 0.4–0.8.

### Maul
Class: Feral · Tree slot: A2
Delay: 5 · Momentum cost: 0
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 7–9, or 10–12 if the Feral is stealthed (Ambush). Breaks stealth. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 1.4–1.8 (Ambush 2.0–2.4).

### Feral Frenzy
Class: Feral · Tree slot: A3
Delay: 6 · Momentum cost: 4
Tags: `attack` `physical` `melee` `random`
Mechanic: Makes 4 hits, each on a random living enemy within range, for 2–4 each. Each hit respects interception and is a separate event (game_system.md §6.5).
Reach: Enemies within range 3.
Hidden stats: Coefficient 0.4–0.8 per hit.

### Silent Strike  [Modifier of Rake]
Class: Feral · Tree slot: A4 · Prerequisite: Rake
Trigger: Rake hits.
Effect: Rake no longer breaks the Feral's stealth. Cost: Rake's Delay is +1 (3).

### Branch B: Instinct

### Prowl
Class: Feral · Tree slot: B1
Delay: 3 · Momentum cost: 0
Tags: `status`
Mechanic: The Feral becomes stealthed. Needs the Feral to be outside the Front row. Stealth ends when the Feral attacks (unless a skill says otherwise) or takes damage.
Reach: Self.
Hidden stats: none.

### Leap Away
Class: Feral · Tree slot: B2
Delay: 2 · Momentum cost: 0
Tags: `move`
Mechanic: The Feral moves one row back. Fails if the row behind is full or it is already in the Rear.
Reach: Self.
Hidden stats: none.

### Cornered Beast  [Passive]
Class: Feral · Tree slot: B3 · Prerequisite: none
Trigger: The Feral falls to 25% HP or less (once per battle).
Effect: The Feral's next turn is pulled forward by 3 time units.

### Branch C: Pack

### Pack Hunt
Class: Feral · Tree slot: C1
Delay: 4 · Momentum cost: 0
Tags: `attack` `physical` `melee` `single`
Mechanic: Hits one enemy for 4–6, or 7–9 if an ally has already hit the target since the Feral's last turn. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 0.8–1.2 (bonus 1.4–1.8).

### Hamstring
Class: Feral · Tree slot: C2
Delay: 4 · Momentum cost: 1
Tags: `attack` `physical` `melee` `status` `single`
Mechanic: Hits one enemy for 2–4. The target's Speed is lowered by 2 for its next 2 turns. Respects interception.
Reach: Range 2.
Hidden stats: Coefficient 0.4–0.8.

### Crippling Bite  [Modifier of Hamstring]
Class: Feral · Tree slot: C3 · Prerequisite: Hamstring
Trigger: Casting Hamstring.
Effect: The Speed penalty lasts for the target's next 4 turns instead of 2. Cost: Hamstring's Momentum cost is +1 (2).

## 7. Elementalist

**Status:** All Draft (proposed by Claude). Identity: a caster whose skills leave **element** statuses that other skills react to. It uses `magical` skills only. Statuses are defined by the skill that applies them:

- **Burning** (`fire`): 2 damage at the start of each of the target's next 3 turns. Reapplying refreshes the duration.
- **Chilled** (`frost`): the target's Speed is lowered by 2 for its next 2 turns.
- **Frozen** (`frost`): the target's progress is stopped for 4 time units. It ends early if the target is hit by a `fire` skill.

### Branch A: Fire

### Fire Bolt
Class: Elementalist · Tree slot: A1
Delay: 4 · Momentum cost: 0
Tags: `attack` `magical` `projectile` `fire` `single`
Mechanic: Hits one enemy for 6–8 and applies Burning. Respects interception.
Reach: Any visible enemy.
Hidden stats: Coefficient 1.2–1.6.

### Flame Wave
Class: Elementalist · Tree slot: A2
Delay: 6 · Momentum cost: 3
Tags: `attack` `magical` `projectile` `fire` `row`
Mechanic: Select an enemy row. Every enemy in it takes 4–6. Each hit respects interception.
Reach: Any enemy row with a visible unit.
Hidden stats: Coefficient 0.8–1.2.

### Meteor
Class: Elementalist · Tree slot: A3
Delay: 6 · Momentum cost: 5
Tags: `attack` `magical` `direct` `fire` `delayed` `circular`
Mechanic: Locks onto one enemy (as a unit). An auto event 12 time units later hits it for 10–12 and the enemies adjacent to it for 5–6. Ignores interception. It is removed if the Elementalist is KO. If the target is KO when it lands, it hits the unit behind it.
Reach: Any visible enemy.
Hidden stats: Coefficient 2.0–2.4 (adjacent 1.0–1.2).

### Thermal Shock  [Modifier of Fire Bolt]
Class: Elementalist · Tree slot: A4 · Prerequisite: Fire Bolt
Trigger: Fire Bolt hits a Chilled target.
Effect: Chilled is removed and Fire Bolt deals +4 damage. Cost: Fire Bolt's Delay is +1 (5).

### Branch B: Frost

### Frost Lance
Class: Elementalist · Tree slot: B1
Delay: 4 · Momentum cost: 0
Tags: `attack` `magical` `projectile` `frost` `single`
Mechanic: Hits one enemy for 5–7 and applies Chilled. Respects interception.
Reach: Any visible enemy.
Hidden stats: Coefficient 1.0–1.4.

### Flash Freeze
Class: Elementalist · Tree slot: B2
Delay: 6 · Momentum cost: 3
Tags: `status` `direct` `frost` `single`
Mechanic: The target becomes Frozen (progress stopped for 4 time units; a `fire` hit ends it early).
Reach: Any visible enemy.
Hidden stats: Stop 4.

### Meltwater  [Modifier of Frost Lance]
Class: Elementalist · Tree slot: B3 · Prerequisite: Frost Lance
Trigger: Frost Lance hits a Burning target.
Effect: Burning is removed and Frost Lance deals +3 damage. Cost: Frost Lance's Delay is +1 (5).

### Deep Freeze  [Modifier of Flash Freeze]
Class: Elementalist · Tree slot: B4 · Prerequisite: Flash Freeze
Trigger: Casting Flash Freeze.
Effect: Frozen lasts 6 time units and is no longer ended by `fire` hits. Cost: Flash Freeze's Momentum cost is +1 (4).

### Branch C: Storm

### Spark
Class: Elementalist · Tree slot: C1
Delay: 2 · Momentum cost: 0
Tags: `attack` `magical` `projectile` `storm` `single`
Mechanic: Hits one enemy for 2–4. Respects interception.
Reach: Any visible enemy.
Hidden stats: Coefficient 0.4–0.8.

### Chain Lightning
Class: Elementalist · Tree slot: C2
Delay: 5 · Momentum cost: 2
Tags: `attack` `magical` `projectile` `storm` `chain`
Mechanic: Hits the selected enemy for 4–6, then jumps up to 2 times to a random enemy adjacent to the last unit hit (not already hit) for 3–5. Only the first hit can be intercepted.
Reach: Any visible enemy.
Hidden stats: Coefficient 0.8–1.2 (jumps 0.6–1.0).

### Overcharge
Class: Elementalist · Tree slot: C3
Delay: 3 · Momentum cost: 0
Tags: `status`
Mechanic: The Elementalist's next `attack` skill deals +4 damage, and that skill's Delay is +2.
Reach: Self.
Hidden stats: Bonus damage 4.

### Storm Conduction  [Passive]
Class: Elementalist · Tree slot: C4 · Prerequisite: Chain Lightning
Trigger: Chain Lightning chooses a jump target.
Effect: Burning or Chilled enemies are chosen before others.
