# Everkin — Profession Skills

**Status:** adopted 2026-11-03. Andreas chose every skill here in picker rounds, one profession at a time (2026-10-17 to 2026-11-03), followed by a review that removed plain attacks and flat bonuses. This document owns the skills of the 12 professions in [class_system.md](class_system.md); [classes_and_skills.md](archive/classes_and_skills.md) stays superseded. Global rules (statuses, Delay, Momentum, summons, ground effects) live in [game_system.md](game_system.md).

**Legend:** D = Delay (1 is quickest), M = Momentum cost. A = active, P = passive, Mod = modifier of the named skill, K = keystone. Durations are in time units (10 is about one former turn). Every damage, healing and shield number assumes Power 5 and is multiplied by the user's Power ÷ 5, rounded down, minimum 1 (game_system §5.2). Rule numbers such as bump, Bleeding, Poison and ground damage do not scale. Branches organize a profession's tree only; a creature can learn from every branch of its professions (class_system §4). Profession skills carry no tags; where a rule names one, it is derived from the Delivery column and the effect text (game_system §4.7).

**Trees:** each profession's list below is its skill tree. A unit has 8 skill points; every node costs 1. Each branch is a tree: a node needs its parent learned first, and a modifier's parent is its skill (game_system §4.1). The **Needs** column names each node's parent; — marks a starting skill (adopted 2026-11-08, Claude's suggestions accepted by Andreas).

**Delivery · Range:** `Melee`, `Projectile` (both can be intercepted) or `Direct` (cannot), then the range: On the enemy side D1 reaches the Front row, D2 the Middle row and D3 the Rear row. On your own side D1 reaches the user and its adjacent units, D2 or more any ally (game_system §7.4). Blank = no target (self, passives, summons). Summons, constructs and totems with a range are placed within it.

**Targets:** a targeted skill works on any unit in range, on either side; an area skill hits every unit in its area. "Own side only" marks the exceptions. No skill targets an empty spot: ground effects go on a target unit's spot or on a row (game_system §8.1).

Numbers are first drafts and are tuned in AI-versus-AI testing (§2.5).

## Attack Type attacks (adopted 2026-11-03)

Every creature starts with the one shared attack of its Attack Type ([class_system.md](class_system.md) §2, game_system §4.4). It costs no skill point.

| Attack Type | Attack | D / M | Effect |
|---|---|---|---|
| Brawl | Brawl Strike | 3 / 0 | `melee` `physical`. Hit for 4–5. |
| Bladed | Blade Strike | 3 / 0 | `melee` `physical`. Hit for 4–5; 30% chance to make the target Bleed. |
| Polearm | Polearm Thrust | 3 / 0 | `melee` `physical`. Hit for 4–5, with one row more reach than other melee. |
| Ranged | Shot | 3 / 0 | `projectile` `physical`. Hit any visible enemy for 3–4. |
| Magic | Bolt | 4 / 0 | `projectile` `magical`. Hit any visible enemy for 3–4. |
| Blunt | Heavy Blow | 4 / 0 | `melee` `physical`. Hit for 5–6; 30% chance to Stun the target for 5 time units. |
| Breath | Breath | 4 / 0 | `direct`, Range D1. Hit the target for 3–4 and every unit next to it for 1–2. |
| Whip | Lash | 3 / 0 | `melee` `physical`. Hit for 3–4, with one row more reach than other melee. |
| Aura | Pulse | 4 / 0 | `direct`. Every enemy in melee reach takes 2–3. |

**Reach of the attacks (adopted 2026-11-08, owner decision).** Plain melee attacks (Brawl Strike, Blade Strike, Heavy Blow) reach distance 1 (D1): the enemy Front row, empty rows skipped as in game_system §7.4. "One row more reach than other melee" means D2: Polearm Thrust and Lash reach distance 2. "Melee reach" (Pulse) is D1. Breath has Range D1. Shot and Bolt reach any visible enemy.

## Hunter (16)

Branches: Stalker, Tracker, Trapper, Predator.

- Every skill that reads a Marked enemy works on any Marked enemy, whoever placed the Mark (§10.5).

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Stalker | Stalk | A | — | 3 / 0 |  | The Hunter gains stealth. Needs the Hunter outside the Front row. |
| Stalker | Creep Closer | A | Stalk | 2 / 0 |  | Only while stealthed: move to an adjacent row on your side without breaking stealth. |
| Stalker | Freeze in Place | P | Stalk |  |  | When an area effect hits the stealthed Hunter, 50% chance to stay stealthed (it still takes the damage). |
| Stalker | Lie in Wait | A | Creep Closer | 2 / 0 | Melee · D1 | Only while stealthed: pick a unit in melee reach. The next time it uses a skill, it is Stunned for 10 time units before that skill resolves (this breaks the Hunter's stealth). |
| Stalker | Vanish | A | Freeze in Place | 4 / 3 |  | Gain stealth, even in the Front row. |
| Tracker | Read Tracks | A | Target | 2 / 0 | Direct · D3 | Pick a row. Every stealthed unit in it is revealed. |
| Tracker | Scent | P | Read Tracks |  |  | Marked enemies cannot gain stealth. |
| Tracker | Scent of Blood | P | Target |  |  | When an enemy's HP falls below 20%, it becomes Marked. |
| Tracker | Target | A | — | 2 / 0 | Direct · D3 | Mark any visible unit (one Mark from this Hunter at a time; marking another moves it). |
| Tracker | Sniff the Air | P | Scent of Blood |  |  | At battle start, the enemy with the highest Power becomes Marked. |
| Trapper | Decoy | P | Net Trap |  |  | When a Marked enemy attacks the Hunter, the Hunter leaves a decoy (HP 10) in its place and appears one row behind. |
| Trapper | Net Trap | A (trap card) | — | 5 / 1 |  | HP 4. A unit that hits it has +3 Delay on its next skill. |
| Trapper | Cage | A | Net Trap | 6 / 3 | Melee · D1 | A unit in melee reach is Immobilized for 20 time units. |
| Predator | Cut Off Retreat | A | — | 3 / 0 | Direct · D3 | A Marked unit is Immobilized for 20 time units. |
| Predator | Flush | A | Cut Off Retreat | 4 / 0 | Direct · D3 | Pull a Marked unit into its own Front row (bumps apply). It loses stealth. |
| Predator | Pack Strike | A | Cut Off Retreat | 5 / 3 |  | Every ally that hit a Marked unit since the Hunter's last turn has its next turn pulled forward 3. |

## Guardian (15)

Branches: Bulwark, Bodyguard, Keeper.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Bulwark | Shield Wall | A | Hold Position | 6 / 4 |  | Needs 3 or more of your units side by side in the Guardian's row. For 10 time units, attacks aimed at units behind the wall are always intercepted by the wall. |
| Bulwark | Hold Position | P | Leap to Defend |  |  | Enemy effects cannot push, pull or swap the Guardian. |
| Bulwark | Full Cover | P | Leap to Defend |  |  | When the Guardian intercepts, it always takes the whole hit, never a partial one. |
| Bulwark | Living Wall | K | Full Cover |  |  | The Guardian always intercepts for the units directly behind it, but it can never attack. |
| Bulwark | Leap to Defend | A | — | 3 / 0 |  | Own side only. The Guardian moves next to an ally anywhere on your side (any gap) and takes the next attack on that ally instead of it. |
| Bodyguard | Rescue | A | Follow the Charge | 5 / 2 | Direct · D2 | Own side only. Swap places with an ally within distance 2 that is below half HP. |
| Bodyguard | Take a Charge | A | — | 2 / 0 | Direct · D3 | Own side only. An ally becomes the Guardian's Charge (one at a time; choosing another moves it). |
| Bodyguard | Follow the Charge | P | Take a Charge |  |  | When the Charge moves or is moved, the Guardian may move with it to stay lined up in front of it. |
| Bodyguard | Distract | A | Take a Charge | 2 / 0 |  | The enemy that last hit the Charge must target the Guardian with its next attack, if it can. |
| Bodyguard | Pledge | A | Distract | 5 / 2 |  | For 10 time units, area effects that would hit the Charge hit the Guardian instead. |
| Keeper | Ward | A | — | 3 / 0 | Direct · D2 | A unit within distance 2 gains a Ward of 4 until the Guardian's next turn. |
| Keeper | Lasting Ward | Mod of Ward | Ward |  | As parent | Ward lasts until it is broken, not until the Guardian's next turn. |
| Keeper | Overcharged Ward | A | Ward | 5 / 3 | Direct · D3 | A unit gains a Ward of 8 that lasts only until that unit's next turn. |
| Keeper | Shield Dome | A | Overcharged Ward | 7 / 5 |  | Every ally gains a Ward of 2 until the Guardian's next turn. |
| Keeper | Keeper of the Gate | K | Lasting Ward |  |  | Your barricades have double HP, but the Guardian itself cannot intercept. |

## Healer (15)

Branches: Mender, Cleric, Lifebinder.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Mender | Mend | A | — | 4 / 0 | Direct · D3 | Heal one unit for 8–10. |
| Mender | Rain of Mending | A | Mend | 6 / 2 | Direct · D3 | Pick a row: every unit in it is healed for 3–4. |
| Mender | Overflowing Care | P | Mend |  |  | Healing beyond the target's max HP goes to its adjacent ally with the lowest HP. |
| Mender | Downpour | Mod of Rain of Mending | Rain of Mending |  | As parent | Rain of Mending also heals the row behind for half. |
| Cleric | Absolve | A | — | 4 / 0 | Direct · D3 | Remove every harmful status from one unit; its next turn is pushed back 2. |
| Cleric | Dispel | A | Absolve | 3 / 0 | Direct · D3 | Remove every helpful status (Ward, Haste, stealth and others) from a unit within distance 3. |
| Cleric | Mass Dispel | A | Dispel | 7 / 4 | Direct · D3 | Remove every helpful status from every unit in one row. |
| Cleric | Steal Blessing | A | Dispel | 4 / 2 | Direct · D3 | Move one helpful status from one unit onto another. |
| Cleric | Turn Back | A | Absolve | 3 / 0 | Direct · D3 | Move one harmful status from a unit onto the unit that applied it. |
| Lifebinder | Revive | A | Life Link | 9 / 5 | Direct · D3 | Revive a KO unit at 25% of its max HP (rounded down, minimum 1). |
| Lifebinder | Life Link | A | — | 4 / 0 | Direct · D3 | Link two units for 10 time units: healing on one also heals the other for half. |
| Lifebinder | Bound Resurrect | A | Life Link | 8 / 4 | Direct · D3 | Revive a KO unit at 25% of its max HP (rounded down, minimum 1); it and the Healer share one HP pool (game_system §10.4). |
| Lifebinder | Burnout Heal | A | Life for Life | 7 / 6 | Direct · D1 | Heal every unit around the Healer for 6–8; the Healer is KO (§10.4). |
| Lifebinder | Life for Life | A | Revive | 6 / 3 | Direct · D3 | The Healer gives up half its current HP to revive a KO unit with that much HP. |
| Lifebinder | Bound Strength | Mod of Bound Resurrect | Bound Resurrect |  | As parent | While the two units share their HP pool, both have 1 less Delay on every skill. |

## Sorcerer (15)

Branches: Enchantment, Transmutation, Malediction.

- Sorcerer skills are effects only: no damage skills and no flat bonuses.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Enchantment | Echo Enchant | A | Piercing Enchant | 6 / 3 | Direct · D3 | A unit's next skill happens twice: the second use is free (no Delay, no Momentum) and hits the same target. |
| Enchantment | Phase Weapon | A | — | 3 / 0 | Direct · D3 | A unit's next attack has unlimited range. |
| Enchantment | Phasing Strike | Mod of Phase Weapon | Phase Weapon |  | As parent | The attack also cannot be intercepted. |
| Enchantment | Piercing Enchant | A | Phase Weapon | 4 / 0 | Direct · D3 | A unit's next attack also hits the unit behind its target. |
| Enchantment | Enchanted Soul | K | Echo Enchant |  |  | The Sorcerer's enchantments last until used, but only one can be active at a time. |
| Transmutation | Petrify | A | Stone Shell | 7 / 4 | Direct · D3 | A unit turns to stone: it is Stunned for 10 time units. |
| Transmutation | Transmute | A | Glass Body | 4 / 0 | Direct · D3 | A unit's Power and Speed swap values for 20 time units. |
| Transmutation | Stone Shell | A | Glass Body | 4 / 0 | Direct · D3 | A unit turns to stone for 10 time units: it cannot act and cannot be damaged. This is a status and can be cleansed. |
| Transmutation | Lasting Transmute | Mod of Transmute | Transmute |  | As parent | Transmute lasts 40 time units. |
| Transmutation | Glass Body | A | — | 4 / 0 | Direct · D3 | The next hit on a unit deals double damage. |
| Malediction | Curse of Stagnation | A | Curse of Weight | 5 / 2 | Direct · D3 | For 20 time units, statuses on a unit do not run down. |
| Malediction | Curse of Betrayal | A | Curse of Weight | 6 / 3 | Direct · D3 | For 20 time units, a unit's healing goes to your units instead. |
| Malediction | Curse of Weight | A | — | 4 / 0 | Direct · D3 | A unit cannot Move or become airborne for 20 time units. |
| Malediction | Lingering Malice | P | Curse of Stagnation |  |  | When an enemy removes one of the Sorcerer's curses or hexes, the remover is Silenced for 10 time units. |
| Malediction | Witch's Patience | K | Curse of Betrayal |  |  | The Sorcerer's curses last until removed, but it can keep only one curse at a time. |

## Elementalist (17)

Branches: Pyromancy, Stormcalling, Cryomancy.

- Mainly a damage dealer with some control: fire blocks ground and damages rows, frost locks units with Frozen, storm is direct effects and chains.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Pyromancy | Row of Fire | A | Fire Lane | 6 / 3 | Direct · D3 | Set fire to a whole row for 20 time units. |
| Pyromancy | Kindle | A | — | 2 / 0 | Direct · D3 | Set fire to a unit's spot. |
| Pyromancy | Backdraft | A | Kindle | 4 / 2 | Direct · D3 | A unit standing on burning ground: the fire on its spot spreads to every spot next to it. |
| Pyromancy | Fire Lane | A | Kindle | 5 / 2 | Direct · D1 | Set fire to a unit's spot and to the spots in the same column in every other row of its side. |
| Pyromancy | Scorch | Mod of Kindle | Kindle |  | As parent | The fire from Kindle also spreads to the spots next to it. |
| Pyromancy | Ignition Point | Mod of Backdraft | Backdraft |  | As parent | Backdraft refreshes the fire's duration to 20 time units. |
| Stormcalling | Chain Lightning | A | Shock | 5 / 3 | Direct · D3 | Stuns a unit for 5 time units, then jumps to an adjacent unit and Stuns it, then once more. |
| Stormcalling | Shock | A | — | 5 / 2 | Direct · D3 | Stuns any unit for 10 time units; it cannot be intercepted. |
| Stormcalling | Arc | Mod of Chain Lightning | Chain Lightning |  | As parent | Chain Lightning jumps 2 more times. |
| Stormcalling | Tempest | A | Shock | 8 / 5 | None · D1 | Push every unit in one row one row back (bumps apply). No extra hit. |
| Stormcalling | Megavolt | A | Chain Lightning | 9 / 5 | Direct · D3 | Every unit in one row is Stunned for 5 time units; the Elementalist is Silenced for 10 time units. |
| Cryomancy | Steam Burst | P | Freeze |  |  | When fire is set on an iced spot, both end and the unit there is Confused for 10 time units. |
| Cryomancy | Freeze | A | — | 7 / 3 | Direct · D3 | A unit is Frozen. |
| Cryomancy | Glacier | A | Frost Nova | 8 / 5 | Direct · D3 | Every unit in one row is Frozen. |
| Cryomancy | Brittle | P | Freeze |  |  | Frozen enemies take 50% more damage. |
| Cryomancy | Frost Nova | A | Freeze | 6 / 3 | Direct · D1 | Every enemy in melee reach of the Elementalist is Frozen. |
| Cryomancy | Cold Heart | K | Brittle |  |  | Frozen from the Elementalist lasts 1 more turn, but it can keep only one unit Frozen at a time. |

## Warrior (16)

Branches: Berserker, Champion, Whirlwind, Footwork.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Berserker | Reckless Swing | P | Endure |  |  | The Warrior's hits deal double damage, but the Warrior takes half of the damage it deals (rounded down). |
| Berserker | Unstoppable | P | Endure |  |  | Below a quarter of its HP, the Warrior cannot be Immobilized or Frozen, and its turns cannot be pushed back. |
| Berserker | Endure | P | — |  |  | Damage from statuses cannot drop the Warrior below 1 HP. |
| Berserker | Last Stand | K | Unstoppable |  |  | The Warrior cannot Move or be moved, but deals 50% more damage. |
| Champion | Challenge | A | — | 2 / 0 | Direct · D3 | Mark a unit (one Mark from this Warrior at a time). |
| Champion | Drag Out | A | Challenge | 4 / 0 | Melee · D3 | Pull a Marked unit one row toward the battle line. |
| Champion | Isolate | A | Drag Out | 5 / 2 | Direct · D3 | Push every unit adjacent to a Marked unit one row back, leaving it alone. |
| Champion | Hunt Down | P | Drag Out |  |  | When a Marked enemy is moved, the Warrior Stuns it for 5 time units. |
| Champion | Execution | A | Single Combat | 7 / 4 | Melee · D1 | A Marked unit below 25% HP is KO. |
| Champion | Single Combat | A | Challenge | 6 / 3 | Direct · D3 | For 10 time units, the Warrior and one Marked unit can be targeted only by each other. |
| Whirlwind | Sweeping Kick | A | Battle Roar | 5 / 2 | Melee · D1 | Push every unit in melee reach one row back (bumps apply). |
| Whirlwind | Crowd Fighter | P | Battle Roar |  |  | While three or more enemies are in its melee reach, the Warrior's attacks hit all of them. |
| Whirlwind | Battle Roar | A | — | 4 / 0 | Direct · D1 | Every unit in melee reach has its next turn pushed back 5 time units. |
| Footwork | Sidestep | A | — | 2 / 0 |  | The next attack on the Warrior misses. |
| Footwork | Counter Step | P | Sidestep |  |  | When an attack on the Warrior misses, its next turn is pulled forward 5 time units. |
| Footwork | Hit and Run | A | Sidestep | 4 / 0 | Melee · D1 | Strike a unit in melee for 3–4: its next action comes 3 time units later, then the Warrior moves one row back. |

## Leader (15)

Branches: Tactician, Commander, Bard.

- Tactician handles the timeline and Momentum, Commander moves your own units, Bard has only passives and keystones (each a different song or sound).

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Tactician | Swap Turns | A | Press the Advantage | 5 / 2 | Direct · D3 | Own side only. Two allies swap their places on the timeline. |
| Tactician | Press the Advantage | A | — | 4 / 2 | Direct · D3 | A unit's next skill costs no Momentum. |
| Tactician | Inspiring Presence | P | Press the Advantage |  |  | When an ally next to the Leader KOs an enemy, Momentum moves 1 more toward your side. |
| Tactician | All In | K | Inspiring Presence |  |  | The Leader's Momentum costs are halved (rounded up), but Momentum gained from its hits is 0. |
| Commander | Reposition | A | — | 2 / 0 | Direct · D3 | Own side only. Move an ally one row, to any gap, at no cost to its timeline. |
| Commander | Fall Back | A | Reposition | 3 / 0 |  | Own side only. Move every ally in the Front row one row back (bumps apply). |
| Commander | Advance | A | Reposition | 3 / 0 | Direct · D3 | Own side only. Move every ally in one row one row forward (bumps apply). |
| Commander | Switch Places | A | Reposition | 3 / 0 | Direct · D3 | Own side only. Two allies swap places. |
| Commander | Rotation | A | Fall Back | 5 / 2 |  | Own side only. Every ally moves one row back, and the Rear row moves to the Front. |
| Commander | Drill | P | Switch Places |  |  | An ally the Leader moves has its next turn pulled forward 3 time units. |
| Bard | Song of Haste | P | — |  |  | Allies in the Leader's row have +1 Speed. |
| Bard | War Drums | P | Song of Haste |  |  | Allies next to the Leader cannot be Stunned or Frozen. |
| Bard | Rhythm | P | Song of Haste |  |  | When an ally uses the same skill twice in a row, the second use has 1 less Delay. |
| Bard | Solo Performer | K | War Drums |  |  | The Leader's Bard passives are twice as strong, but they reach only the Leader's own row. |
| Bard | Grand Chorus | K | Rhythm |  |  | The Leader's Bard passives reach every ally, but the Leader can use no active skills except its type-specific attack. |

## Rogue (14)

Branches: Thief, Assassin, Saboteur.

- Thief is also about stealth.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Thief | Hide | A | — | 3 / 0 |  | The Rogue gains stealth. Needs the Rogue outside the Front row. |
| Thief | Melt Away | A | Hide | 4 / 2 |  | The Rogue gains stealth anywhere, even in the Front row. |
| Thief | Sneak | P | Hide |  |  | While stealthed, the Rogue may move one row at the start of its turn, at no cost to its timeline and without losing stealth. |
| Thief | Lift a Skill | A | Light Fingers | 6 / 3 | Direct · D3 | Use a copy of the last active skill the target used, with the Rogue's own Power. |
| Thief | Light Fingers | P | Sneak |  |  | Whenever the Rogue hits an enemy that has a helpful status, it steals that status. |
| Assassin | Backstab | A | Mark for Death | 5 / 2 | Melee · D1 | Only from stealth: the target is Silenced and Bleeds, and the Rogue stays stealthed. |
| Assassin | Contract | K | Deathmark Bloom |  |  | The Rogue deals double to Marked enemies and half to everyone else. |
| Assassin | Poison Vial | A | Mark for Death | 3 / 0 | Projectile · D3 | Throw a vial: poison ground on a unit's spot for 20 time units. |
| Assassin | Mark for Death | A | — | 2 / 0 | Direct · D3 | Mark a unit for 20 time units. |
| Assassin | Deathmark Bloom | P | Backstab |  |  | When a Marked enemy is KO, its Mark jumps to the nearest enemy. |
| Saboteur | Flash Powder | A | Jam | 3 / 0 | Direct · D1 | Every unit in melee reach is Blind for 20 time units. |
| Saboteur | Cut the Strings | A | Jam | 5 / 2 | None · D3 | Remove one auto event the other side has on the timeline. |
| Saboteur | Jam | A | — | 3 / 0 | Projectile · D3 | A unit's next skill has +3 Delay. |
| Saboteur | Disarm Trap | P | Cut the Strings |  |  | The Rogue sets off enemy traps without taking their effect. |

## Tinker (13)

Branches: Engineer, Chemist, Pyrotechnician.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Engineer | Bolt Turret | A (construct) | Barricade | 5 / 2 | None · D1 | Places a turret (HP 8). Each time the Tinker takes a turn, it hits the nearest enemy for 2–3. |
| Engineer | Barricade | A (construct) | — | 4 / 0 | None · D2 | Places a barricade (HP 10) on any free spot of your side. |
| Engineer | Overclock | A | Bolt Turret | 3 / 0 |  | Every construct of the Tinker acts once more right now. |
| Engineer | Repair | A | Barricade | 3 / 0 | Direct · D3 | A construct heals 8. |
| Chemist | Poison Gas | A | Healing Draught | 4 / 0 | Direct · D3 | Poison ground on a unit's spot and the spots beside it, for 20 time units. |
| Chemist | Gas Cloud | A | Poison Gas | 6 / 3 | Direct · D3 | Poison ground on a whole row for 20 time units. |
| Chemist | Healing Draught | A | — | 3 / 0 | Projectile · D2 | Throw a potion: a unit heals 6–7. |
| Chemist | Antidote | A | Healing Draught | 2 / 0 | Direct · D2 | Remove Poison, Bleeding and Blind from a unit. |
| Chemist | Plague Doctor | K | Gas Cloud |  |  | The Tinker's poison ground lasts twice as long, but the Tinker cannot heal. |
| Pyrotechnician | Firebomb | A | Flashbang | 5 / 2 | Projectile · D2 | Fire on a unit's spot and the spots beside it, for 20 time units. |
| Pyrotechnician | Flame Turret | A (construct) | Firebomb | 6 / 3 | None · D1 | Places a turret (HP 8). Each time the Tinker takes a turn, it sets fire to the spot of one enemy in front of it. |
| Pyrotechnician | Flashbang | A | — | 4 / 0 | Direct · D2 | Every unit in one row is Blind for 10 time units. |
| Pyrotechnician | Showman | K | Flashbang |  |  | The Tinker's Blind and Stunned last twice as long, but the Tinker's skills that deal damage have +2 Delay. |

## Summoner (15)

Branches: Necromancer, Golemancer, Conjurer.

- Summons appear next to the Summoner unless a skill says otherwise; Portal places them on a random free spot.
- Undead follow game_system §11.5.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Necromancer | Raise Undead | A (summon) | — | 8 / 4 | Direct · D3 | Turns one KO'd body from either side into an undead of yours, placed in a free slot: same art in an undead frame, its own basic attack, HP 12, Power 4. Healing damages it. When it dies it leaves its body on your side. |
| Necromancer | Corpse Explosion | A | Raise Undead | 5 / 2 | Direct · D3 | A body or an undead of the Summoner explodes: 6 to every unit next to it. An exploding undead takes the 6 as well; a body stays where it is. |
| Necromancer | Mass Grave | Mod of Raise Undead | Raise Undead |  | As parent | Raise Undead also raises one more random body. |
| Necromancer | Plague Corpse | Mod of Corpse Explosion | Corpse Explosion |  | As parent | Every unit hit by the explosion is Poisoned for 20 time units. |
| Necromancer | Desecrate | A | Raise Undead | 5 / 2 | Direct · D3 | A KO'd body cannot be revived for 40 time units. |
| Necromancer | Lich Form | K | Mass Grave |  |  | The Summoner counts as undead (healing hurts it), but every undead of the Summoner has +2 Power. |
| Golemancer | Clay Golem | A (summon) | — | 8 / 4 | None · D1 | Summons a golem (HP 25, Speed 3) that makes melee hits for 5–6. The plain, sturdy golem. |
| Golemancer | Rune Golem | A (summon) | Clay Golem | 8 / 4 | None · D1 | Summons a rune golem (HP 18, Speed 4) that makes melee hits for 3–4; each hit Silences the target for 10 time units. |
| Golemancer | Clockwork Golem | A (summon) | Clay Golem | 7 / 3 | None · D1 | Summons a clockwork golem (HP 16, Speed 5) that makes melee hits for 3–4; each time it acts, the Summoner's next action moves 2 time units sooner. |
| Golemancer | Bell Golem | A (summon) | Rune Golem | 7 / 3 | None · D1 | Summons a bell golem (HP 18, Speed 4) that does not attack; each time it acts, every ally next to it loses one status. |
| Golemancer | Golem Heart | K | Clockwork Golem |  |  | The Summoner can keep only one golem, but it has double HP. |
| Conjurer | Animate Blades | A (summon) | — | 5 / 2 | None · D1 | Summons flying blades (HP 6, Speed 7) that make ranged hits for 2–3. Lasts 30 time units. |
| Conjurer | Portal | P | Animate Mirror |  |  | The Summoner's summons appear on a random free spot of your side instead of being placed. |
| Conjurer | Blade Storm | Mod of Animate Blades | Animate Blades |  | As parent | Animate Blades summons two sets of blades. |
| Conjurer | Animate Mirror | A (summon) | Animate Blades | 6 / 3 | None · D1 | Summons a mirror (HP 8, Speed 5) that answers each hit on it with the attacker's own basic attack. Lasts 30 time units. |

## Primalist (15)

Branches: Totemist, Ritualist, Spiritwalker.

- A Primalist may keep any number of totems; Great Totem limits it to one.
- Spiritwalker focuses on possession and soul swap.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Totemist | Healing Totem | A (construct) | — | 5 / 2 | None · D1 | Places a totem (HP 8): allies in its row regain 2 HP every 10 time units. |
| Totemist | Spirit Link Totem | A (construct) | Healing Totem | 6 / 3 | None · D1 | Places a totem (HP 10): damage to allies in its row is split evenly between them. |
| Totemist | Totem of Return | A (construct) | Healing Totem | 7 / 3 | None · D1 | Places a totem (HP 6): the first ally in its row that would be KO keeps 1 HP instead, and the totem breaks. |
| Totemist | Great Totem | K | Spirit Link Totem |  |  | The Primalist keeps only 1 totem, but it has triple HP and also affects the rows next to it. |
| Ritualist | Rain Dance | A | Hoarfrost | 6 / 3 | Direct · D3 | Water on every spot of one row (§12.2). |
| Ritualist | Hoarfrost | A | — | 5 / 2 | Direct · D3 | Ice on a unit's spot (§12.2). |
| Ritualist | Totem Ritual | A | Hoarfrost | 5 / 2 |  | Every totem of the Primalist regains all its HP. |
| Ritualist | Long Rain | Mod of Rain Dance | Rain Dance |  | As parent | Rain Dance's water lasts 20 time units longer. |
| Ritualist | Healing Rain | Mod of Rain Dance | Rain Dance |  | As parent | Allies in the rain regain 4 HP. |
| Ritualist | Rite of Mists | A | Rain Dance | 6 / 3 |  | Ranged hits from units standing on water deal half damage for 20 time units. |
| Spiritwalker | Puppet Soul | A | Forced Step | 6 / 3 | Direct · D3 | Possession. A unit hits a unit next to it at once with its basic attack. |
| Spiritwalker | Life Swap | A | Shared Fate | 8 / 5 | Direct · D3 | Soul swap. Two units swap their HP as a share of their maximum. |
| Spiritwalker | Forced Step | A | — | 4 / 0 | Direct · D3 | Possession. A unit moves to a free spot of your choice on its own side. |
| Spiritwalker | Linked Fates | A | Forced Step | 5 / 2 | Direct · D3 | Soul swap. For 20 time units, one unit also takes half of all damage another unit takes. |
| Spiritwalker | Shared Fate | A | Linked Fates | 6 / 3 | Direct · D3 | Soul swap. For 20 time units, two units each also take half of the damage the other takes. |

## Flying (13)

Branches: Mobility, Skywatch, Assault.

- Airborne follows game_system §10.6.

| Branch | Skill | Kind | Needs | D / M | Delivery · Range | Effect |
|---|---|---|---|---|---|---|
| Mobility | Airlift | A | Take Off | 3 / 0 | Direct · D3 | Own side only. Moves an ally to any free spot of your side. |
| Mobility | Take Off | A | — | 3 / 0 |  | For 20 time units the unit is airborne (game_system §10.6): melee cannot reach it and it ignores ground effects; it uses its skills as normal. |
| Mobility | High Altitude | Mod of Take Off | Take Off |  | As parent | Take Off lasts 40 time units. |
| Mobility | Sky Lord | K | High Altitude |  |  | The unit is always airborne, but has 30% less HP (rounded down). |
| Skywatch | Scout Ahead | A | — | 2 / 0 |  | Reveals stealthed units in every row of one side for 10 time units. |
| Skywatch | Spotter | A | Scout Ahead | 3 / 0 | Direct · D3 | A unit is Marked for 20 time units. |
| Skywatch | Long Watch | P | Spotter |  |  | Marks the unit places last 10 time units longer. |
| Skywatch | Watchtower | P | Scout Ahead |  |  | While airborne, the unit repeats Scout Ahead on its own every 20 time units. |
| Assault | Boundless Sky | K | Dust Up |  |  | While airborne, all the unit's skills have unlimited range. |
| Assault | Gust | A | — | 3 / 0 | Direct · D1 | Wind. Pushes a unit one row back. |
| Assault | Headwind | A | Gust | 4 / 0 | Direct · D1 | Wind. A unit's next action comes 5 time units later. |
| Assault | Turbulence | A | Headwind | 4 / 0 |  | Wind. Every airborne unit, on both sides, lands. |
| Assault | Dust Up | A | Gust | 4 / 0 | Direct · D1 | Wind. A unit is Blind for 10 time units. |
