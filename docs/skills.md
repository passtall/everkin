# Everkin — Profession Skills

**Status:** adopted 2026-11-03. Andreas chose every skill here in picker rounds, one profession at a time (2026-10-17 to 2026-11-03), followed by a review that removed plain attacks and flat bonuses. This document owns the skills of the 12 professions in [class_system.md](class_system.md); [classes_and_skills.md](classes_and_skills.md) stays superseded. Global rules (statuses, Delay, Momentum, summons, ground effects) live in [game_system.md](game_system.md).

**Legend:** D = Delay (1 is quickest), M = Momentum cost. A = active, P = passive, Mod = modifier of the named skill, K = keystone. Durations are in time units (10 is about one former turn). Damage and healing numbers assume Power 5. Branches organize a profession's tree only; a creature can learn from every branch of its professions (class_system §4). There are no tags.

**Trees:** each profession's list below is its skill tree. A unit has 10 skill points; every node costs 1; a modifier needs its parent skill and a keystone needs 3 other nodes from its branch (game_system §4.1).

**Targets:** a targeted skill works on any unit in range, on either side; an area skill hits every unit in its area. "Own side only" marks the exceptions (game_system §8.1).

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
| Breath | Breath | 4 / 0 | `direct`. Hit the target for 3–4 and every unit next to it for 1–2. |
| Whip | Lash | 3 / 0 | `melee` `physical`. Hit for 3–4, with one row more reach than other melee. |
| Aura | Pulse | 4 / 0 | `direct`. Every enemy in melee reach takes 2–3. |

## Hunter (16)

Branches: Stalker, Tracker, Trapper, Predator.

- Every skill that reads a Marked enemy works on any Marked enemy, whoever placed the Mark (§10.5).

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Stalker | Stalk | A | 3 / 0 | The Hunter gains stealth. Needs the Hunter outside the Front row. |
| Stalker | Creep Closer | A | 2 / 0 | Only while stealthed: move to an adjacent row on your side without breaking stealth. |
| Stalker | Freeze in Place | P |  | When an area effect hits the stealthed Hunter, 50% chance to stay stealthed (it still takes the damage). |
| Stalker | Lie in Wait | A | 2 / 0 | Only while stealthed: pick a unit in melee reach. The next time it uses a skill, it is Stunned for 10 time units before that skill resolves (this breaks the Hunter's stealth). |
| Stalker | Vanish | A | 4 / 3 | Gain stealth, even in the Front row. |
| Tracker | Read Tracks | A | 2 / 0 | Pick a row. Every stealthed unit in it is revealed. |
| Tracker | Scent | P |  | Marked enemies cannot gain stealth. |
| Tracker | Scent of Blood | P |  | When an enemy's HP falls below 20%, it becomes Marked. |
| Tracker | Target | A | 2 / 0 | Mark any visible unit (one Mark from this Hunter at a time; marking another moves it). |
| Tracker | Sniff the Air | P |  | At battle start, the enemy with the highest Power becomes Marked. |
| Trapper | Decoy | P |  | When a Marked enemy attacks the Hunter, the Hunter leaves a decoy (HP 10) in its place and appears one row behind. |
| Trapper | Net Trap | A (trap card) | 5 / 1 | HP 4. A unit that hits it has +3 Delay on its next skill. |
| Trapper | Cage | A | 6 / 3 | A unit in melee reach is Immobilized for 20 time units. |
| Predator | Cut Off Retreat | A | 3 / 0 | A Marked unit is Immobilized for 20 time units. |
| Predator | Flush | A | 4 / 0 | Pull a Marked unit into its own Front row (bumps apply). It loses stealth. |
| Predator | Pack Strike | A | 5 / 3 | Every ally that hit a Marked unit since the Hunter's last turn has its next turn pulled forward 3. |

## Guardian (15)

Branches: Bulwark, Bodyguard, Keeper.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Bulwark | Shield Wall | A | 6 / 4 | Needs 3 or more of your units side by side in the Guardian's row. For 10 time units, attacks aimed at units behind the wall are always intercepted by the wall. |
| Bulwark | Hold Position | P |  | Enemy effects cannot push, pull or swap the Guardian. |
| Bulwark | Full Cover | P |  | When the Guardian intercepts, it always takes the whole hit, never a partial one. |
| Bulwark | Living Wall | K |  | The Guardian always intercepts for the units directly behind it, but it can never attack. |
| Bulwark | Leap to Defend | A | 3 / 0 | Own side only. The Guardian moves next to an ally anywhere on your side (any gap) and takes the next attack on that ally instead of it. |
| Bodyguard | Rescue | A | 5 / 2 | Own side only. Swap places with an ally within distance 2 that is below half HP. |
| Bodyguard | Take a Charge | A | 2 / 0 | Own side only. An ally becomes the Guardian's Charge (one at a time; choosing another moves it). |
| Bodyguard | Follow the Charge | P |  | When the Charge moves or is moved, the Guardian may move with it to stay lined up in front of it. |
| Bodyguard | Distract | A | 2 / 0 | The enemy that last hit the Charge must target the Guardian with its next attack, if it can. |
| Bodyguard | Pledge | A | 5 / 2 | For 10 time units, area effects that would hit the Charge hit the Guardian instead. |
| Keeper | Ward | A | 3 / 0 | A unit within distance 2 gains a Ward of 4 until the Guardian's next turn. |
| Keeper | Lasting Ward | Mod of Ward |  | Ward lasts until it is broken, not until the Guardian's next turn. |
| Keeper | Overcharged Ward | A | 5 / 3 | A unit gains a Ward of 8 that lasts only until that unit's next turn. |
| Keeper | Shield Dome | A | 7 / 5 | Every ally gains a Ward of 2 until the Guardian's next turn. |
| Keeper | Keeper of the Gate | K |  | Your barricades have double HP, but the Guardian itself cannot intercept. |

## Healer (15)

Branches: Mender, Cleric, Lifebinder.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Mender | Mend | A | 4 / 0 | Heal one unit for 8–10. |
| Mender | Rain of Mending | A | 6 / 2 | Pick a row: every unit in it is healed for 3–4. |
| Mender | Overflowing Care | P |  | Healing beyond the target's max HP goes to its adjacent ally with the lowest HP. |
| Mender | Downpour | Mod of Rain of Mending |  | Rain of Mending also heals the row behind for half. |
| Cleric | Absolve | A | 4 / 0 | Remove every harmful status from one unit; its next turn is pushed back 2. |
| Cleric | Dispel | A | 3 / 0 | Remove every helpful status (Ward, Haste, stealth and others) from a unit within distance 3. |
| Cleric | Mass Dispel | A | 7 / 4 | Remove every helpful status from every unit in one row. |
| Cleric | Steal Blessing | A | 4 / 2 | Move one helpful status from one unit onto another. |
| Cleric | Turn Back | A | 3 / 0 | Move one harmful status from a unit onto the unit that applied it. |
| Lifebinder | Revive | A | 9 / 5 | Revive a KO unit at 25% of its max HP. |
| Lifebinder | Life Link | A | 4 / 0 | Link two units for 10 time units: healing on one also heals the other for half. |
| Lifebinder | Bound Resurrect | A | 8 / 4 | Revive a KO unit; it and the Healer share one HP pool (game_system §10.4). |
| Lifebinder | Burnout Heal | A | 7 / 6 | Heal every unit around the Healer for a large amount; the Healer is KO (§10.4). |
| Lifebinder | Life for Life | A | 6 / 3 | The Healer gives up half its current HP to revive a KO unit with that much HP. |
| Lifebinder | Bound Strength | Mod of Bound Resurrect |  | While the two units share their HP pool, both have 1 less Delay on every skill. |

## Sorcerer (15)

Branches: Enchantment, Transmutation, Malediction.

- Sorcerer skills are effects only: no damage skills and no flat bonuses.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Enchantment | Echo Enchant | A | 6 / 3 | A unit's next skill happens twice. |
| Enchantment | Phase Weapon | A | 3 / 0 | A unit's next attack has unlimited range. |
| Enchantment | Phasing Strike | Mod of Phase Weapon |  | The attack also cannot be intercepted. |
| Enchantment | Piercing Enchant | A | 4 / 0 | A unit's next attack also hits the unit behind its target. |
| Enchantment | Enchanted Soul | K |  | The Sorcerer's enchantments last until used, but only one can be active at a time. |
| Transmutation | Petrify | A | 7 / 4 | A unit turns to stone: it is Stunned for 10 time units. |
| Transmutation | Transmute | A | 4 / 0 | A unit's Power and Speed swap values for 20 time units. |
| Transmutation | Stone Shell | A | 4 / 0 | A unit turns to stone for 10 time units: it cannot act and cannot be damaged. |
| Transmutation | Lasting Transmute | Mod of Transmute |  | Transmute lasts 40 time units. |
| Transmutation | Glass Body | A | 4 / 0 | The next hit on a unit deals double damage. |
| Malediction | Curse of Stagnation | A | 5 / 2 | For 20 time units, statuses on a unit do not run down. |
| Malediction | Curse of Betrayal | A | 6 / 3 | For 20 time units, a unit's healing goes to your units instead. |
| Malediction | Curse of Weight | A | 4 / 0 | A unit cannot Move or become airborne for 20 time units. |
| Malediction | Lingering Malice | P |  | When an enemy removes one of the Sorcerer's curses or hexes, the remover is Silenced for 10 time units. |
| Malediction | Witch's Patience | K |  | The Sorcerer's curses last until removed, but it can keep only one curse at a time. |

## Elementalist (17)

Branches: Pyromancy, Stormcalling, Cryomancy.

- Mainly a damage dealer with some control: fire blocks ground and damages rows, frost locks units with Frozen, storm is direct effects and chains.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Pyromancy | Row of Fire | A | 6 / 3 | Set fire to a whole row for 20 time units. |
| Pyromancy | Kindle | A | 2 / 0 | Set fire to one spot. |
| Pyromancy | Backdraft | A | 6 / 3 | A unit standing on burning ground: the fire on its spot spreads to every spot next to it. |
| Pyromancy | Fire Lane | A | 5 / 2 | Set fire to one spot in every row of one side, all in the same column. |
| Pyromancy | Scorch | Mod of Kindle |  | The fire from Kindle also spreads to the spots next to it. |
| Pyromancy | Ignition Point | Mod of Backdraft |  | Backdraft leaves the fire burning. |
| Stormcalling | Chain Lightning | A | 5 / 3 | Stuns a unit for 5 time units, then jumps to an adjacent unit and Stuns it, then once more. |
| Stormcalling | Shock | A | 5 / 2 | Stuns any unit for 10 time units; it cannot be intercepted. |
| Stormcalling | Arc | Mod of Chain Lightning |  | Chain Lightning jumps 2 more times. |
| Stormcalling | Tempest | A | 8 / 5 | Push every unit on one side one row back (bumps apply). No extra hit. |
| Stormcalling | Megavolt | A | 9 / 5 | Every unit in one row is Stunned for 5 time units; the Elementalist is Silenced for 10 time units. |
| Cryomancy | Steam Burst | P |  | When fire is set on an iced spot, both end and the unit there is Confused for 10 time units. |
| Cryomancy | Freeze | A | 7 / 3 | A unit is Frozen. |
| Cryomancy | Glacier | A | 8 / 5 | Every unit in one row is Frozen. |
| Cryomancy | Brittle | P |  | Frozen enemies take 50% more damage. |
| Cryomancy | Frost Nova | A | 6 / 3 | Every unit in melee reach of the Elementalist is Frozen. |
| Cryomancy | Cold Heart | K |  | Frozen from the Elementalist lasts 1 more turn, but it can keep only one unit Frozen at a time. |

## Warrior (16)

Branches: Berserker, Champion, Whirlwind, Footwork.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Berserker | Reckless Swing | P |  | The Warrior's hits deal double damage, but the Warrior takes half of the damage it deals. |
| Berserker | Unstoppable | P |  | Below a quarter of its HP, the Warrior cannot be moved or Frozen, and its turns cannot be pushed back. |
| Berserker | Endure | P |  | Damage from statuses cannot drop the Warrior below 1 HP. |
| Berserker | Last Stand | K |  | The Warrior cannot Move or be moved, but deals 50% more damage. |
| Champion | Challenge | A | 2 / 0 | Mark a unit (one Mark from this Warrior at a time). |
| Champion | Drag Out | A | 4 / 0 | Pull a Marked unit one row toward the battle line. |
| Champion | Isolate | A | 5 / 2 | Push every unit adjacent to a Marked unit one row back, leaving it alone. |
| Champion | Hunt Down | P |  | When a Marked enemy is moved, the Warrior Stuns it for 5 time units. |
| Champion | Execution | A | 7 / 4 | A Marked unit below 25% HP is KO. |
| Champion | Single Combat | A | 6 / 3 | For 10 time units, the Warrior and one Marked unit can be targeted only by each other. |
| Whirlwind | Sweeping Kick | A | 5 / 2 | Push every unit in melee reach one row back (bumps apply). |
| Whirlwind | Crowd Fighter | P |  | While three or more enemies are in its melee reach, the Warrior's attacks hit all of them. |
| Whirlwind | Battle Roar | A | 4 / 0 | Every unit in melee reach has its next turn pushed back 5 time units. |
| Footwork | Sidestep | A | 2 / 0 | The next attack on the Warrior misses. |
| Footwork | Counter Step | P |  | When an attack on the Warrior misses, its next turn is pulled forward 5 time units. |
| Footwork | Hit and Run | A | 4 / 0 | Strike a unit in melee: its next action comes 3 time units later, then the Warrior moves one row back. |

## Leader (15)

Branches: Tactician, Commander, Bard.

- Tactician handles the timeline and Momentum, Commander moves your own units, Bard has only passives and keystones (each a different song or sound).

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Tactician | Swap Turns | A | 5 / 2 | Two units swap their places on the timeline. |
| Tactician | Press the Advantage | A | 4 / 2 | A unit's next skill costs no Momentum. |
| Tactician | Inspiring Presence | P |  | When an ally next to the Leader KOs an enemy, Momentum moves 1 more toward your side. |
| Tactician | All In | K |  | The Leader's Momentum costs are halved (rounded up), but Momentum gained from its hits is 0. |
| Commander | Reposition | A | 2 / 0 | Move a unit one row, to any gap, at no cost to its timeline. |
| Commander | Fall Back | A | 3 / 0 | Move every unit in one side's Front row one row back (bumps apply). |
| Commander | Advance | A | 3 / 0 | Move every unit in one row one row forward (bumps apply). |
| Commander | Switch Places | A | 3 / 0 | Own side only. Two allies swap places. |
| Commander | Rotation | A | 5 / 2 | Every unit on one side moves one row back, and the Rear row moves to the Front. |
| Commander | Drill | P |  | An ally the Leader moves has its next turn pulled forward 3 time units. |
| Bard | Song of Haste | P |  | Allies in the Leader's row have +1 Speed. |
| Bard | War Drums | P |  | Allies next to the Leader cannot be Stunned or Frozen. |
| Bard | Rhythm | P |  | When an ally uses the same skill twice in a row, the second use has 1 less Delay. |
| Bard | Solo Performer | K |  | The Leader's Bard passives are twice as strong, but they reach only the Leader's own row. |
| Bard | Grand Chorus | K |  | The Leader's Bard passives reach every ally, but the Leader can use no active skills except its type-specific attack. |

## Rogue (14)

Branches: Thief, Assassin, Saboteur.

- Thief is also about stealth.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Thief | Hide | A | 3 / 0 | The Rogue gains stealth. Needs the Rogue outside the Front row. |
| Thief | Melt Away | A | 4 / 2 | The Rogue gains stealth anywhere, even in the Front row. |
| Thief | Sneak | P |  | While stealthed, the Rogue may move one row at the start of its turn, at no cost to its timeline and without losing stealth. |
| Thief | Lift a Skill | A | 6 / 3 | Use a copy of the last skill the target used, with the Rogue's own Power. |
| Thief | Light Fingers | P |  | Whenever the Rogue hits an enemy that has a helpful status, it steals that status. |
| Assassin | Backstab | A | 5 / 2 | Only from stealth: the target is Silenced and Bleeds, and the Rogue stays stealthed. |
| Assassin | Contract | K |  | The Rogue deals double to Marked enemies and half to everyone else. |
| Assassin | Poison Vial | A | 3 / 0 | Throw a vial: poison ground on one spot for 20 time units. |
| Assassin | Mark for Death | A | 2 / 0 | Mark a unit for 20 time units. |
| Assassin | Deathmark Bloom | P |  | When a Marked enemy is KO, its Mark jumps to the nearest enemy. |
| Saboteur | Flash Powder | A | 3 / 0 | Every unit in melee reach is Blind for 20 time units. |
| Saboteur | Cut the Strings | A | 5 / 2 | Remove one auto event the other side has on the timeline. |
| Saboteur | Jam | A | 3 / 0 | A unit's next skill has +3 Delay. |
| Saboteur | Disarm Trap | P |  | The Rogue sets off enemy traps without taking their effect. |

## Tinker (13)

Branches: Engineer, Chemist, Pyrotechnician.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Engineer | Bolt Turret | A (construct) | 5 / 2 | Places a turret (HP 8). Each time the Tinker takes a turn, it hits the nearest enemy for 2–3. |
| Engineer | Barricade | A (construct) | 4 / 0 | Places a barricade (HP 10) on any free spot of your side. |
| Engineer | Overclock | A | 3 / 0 | Every construct of the Tinker acts once more right now. |
| Engineer | Repair | A | 3 / 0 | A construct heals 8. |
| Chemist | Poison Gas | A | 4 / 0 | Poison ground on a spot and the spots beside it, for 20 time units. |
| Chemist | Gas Cloud | A | 6 / 3 | Poison ground on a whole row for 20 time units. |
| Chemist | Healing Draught | A | 3 / 0 | Throw a potion: a unit heals 6–7. |
| Chemist | Antidote | A | 2 / 0 | Remove Poison, Bleeding and Blind from a unit. |
| Chemist | Plague Doctor | K |  | The Tinker's poison ground lasts twice as long, but the Tinker cannot heal. |
| Pyrotechnician | Firebomb | A | 5 / 2 | Fire on a spot and the spots beside it, for 20 time units. |
| Pyrotechnician | Flame Turret | A (construct) | 6 / 3 | Places a turret (HP 8). Each time the Tinker takes a turn, it sets fire to one enemy spot in front of it. |
| Pyrotechnician | Flashbang | A | 4 / 0 | Every unit in one row is Blind for 10 time units. |
| Pyrotechnician | Showman | K |  | The Tinker's Blind and Stunned last twice as long, but the Tinker's skills that deal damage have +2 Delay. |

## Summoner (15)

Branches: Necromancer, Golemancer, Conjurer.

- Summons appear next to the Summoner unless a skill says otherwise; Portal places them on a random free spot.
- Undead follow game_system §11.5.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Necromancer | Raise Undead | A (summon) | 8 / 4 | Turns one KO'd body from either side into an undead of yours, placed in a free slot: same art in an undead frame, its own basic attack, HP 12, Power 4. Healing damages it. When it dies it leaves its body on your side. |
| Necromancer | Corpse Explosion | A | 5 / 2 | A body or an undead of the Summoner explodes: 6 to every unit next to it. An exploding undead takes the 6 as well; a body stays where it is. |
| Necromancer | Mass Grave | Mod of Raise Undead |  | Raise Undead raises up to two bodies at once. |
| Necromancer | Plague Corpse | Mod of Corpse Explosion |  | Every unit hit by the explosion is Poisoned for 20 time units. |
| Necromancer | Desecrate | A | 5 / 2 | A KO'd body cannot be revived for 40 time units. |
| Necromancer | Lich Form | K |  | The Summoner counts as undead (healing hurts it), but every undead of the Summoner has +2 Power. |
| Golemancer | Clay Golem | A (summon) | 8 / 4 | Summons a golem (HP 25, Speed 3) that makes melee hits for 5–6. The plain, sturdy golem. |
| Golemancer | Rune Golem | A (summon) | 8 / 4 | Summons a rune golem (HP 18, Speed 4) that makes melee hits for 3–4; each hit Silences the target for 10 time units. |
| Golemancer | Clockwork Golem | A (summon) | 7 / 3 | Summons a clockwork golem (HP 16, Speed 5) that makes melee hits for 3–4; each time it acts, the Summoner's next action moves 2 time units sooner. |
| Golemancer | Bell Golem | A (summon) | 7 / 3 | Summons a bell golem (HP 18, Speed 4) that does not attack; each time it acts, every ally next to it loses one status. |
| Golemancer | Golem Heart | K |  | The Summoner can keep only one golem, but it has double HP. |
| Conjurer | Animate Blades | A (summon) | 5 / 2 | Summons flying blades (HP 6, Speed 7) that make ranged hits for 2–3. Lasts 30 time units. |
| Conjurer | Portal | P |  | The Summoner's summons appear on a random free spot of your side instead of next to the Summoner. |
| Conjurer | Blade Storm | Mod of Animate Blades |  | Animate Blades summons two sets of blades. |
| Conjurer | Animate Mirror | A (summon) | 6 / 3 | Summons a mirror (HP 8, Speed 5) that answers each hit on it with the attacker's own basic attack. Lasts 30 time units. |

## Primalist (15)

Branches: Totemist, Ritualist, Spiritwalker.

- A Primalist may keep any number of totems; Great Totem limits it to one.
- Spiritwalker focuses on possession and soul swap.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Totemist | Healing Totem | A (construct) | 5 / 2 | Places a totem (HP 8): allies in its row regain 2 HP every 10 time units. |
| Totemist | Spirit Link Totem | A (construct) | 6 / 3 | Places a totem (HP 10): damage to allies in its row is split evenly between them. |
| Totemist | Totem of Return | A (construct) | 7 / 3 | Places a totem (HP 6): the first ally in its row that would be KO keeps 1 HP instead, and the totem breaks. |
| Totemist | Great Totem | K |  | The Primalist keeps only 1 totem, but it has triple HP and also affects the rows next to it. |
| Ritualist | Rain Dance | A | 6 / 3 | Water on every spot of one row (§12.2). |
| Ritualist | Hoarfrost | A | 5 / 2 | Ice on one spot (§12.2). |
| Ritualist | Totem Ritual | A | 5 / 2 | Every totem of the Primalist regains all its HP. |
| Ritualist | Long Rain | Mod of Rain Dance |  | Rain Dance's water lasts 20 time units longer. |
| Ritualist | Healing Rain | Mod of Rain Dance |  | Allies in the rain regain 4 HP. |
| Ritualist | Rite of Mists | A | 6 / 3 | Ranged hits from units standing on water deal half damage for 20 time units. |
| Spiritwalker | Puppet Soul | A | 6 / 3 | Possession. A unit hits a unit next to it at once with its basic attack. |
| Spiritwalker | Life Swap | A | 8 / 5 | Soul swap. Two units swap their HP as a share of their maximum. |
| Spiritwalker | Forced Step | A | 4 / 0 | Possession. A unit moves to a free spot of your choice on its own side. |
| Spiritwalker | Linked Fates | A | 5 / 2 | Soul swap. For 20 time units, one unit also takes half of all damage another unit takes. |
| Spiritwalker | Shared Fate | A | 6 / 3 | Soul swap. For 20 time units, two units each also take half of the damage the other takes. |

## Flying (13)

Branches: Mobility, Skywatch, Assault.

- Airborne follows game_system §10.6.

| Branch | Skill | Kind | D / M | Effect |
|---|---|---|---|---|
| Mobility | Airlift | A | 3 / 0 | Own side only. Moves an ally to any free spot of your side. |
| Mobility | Take Off | A | 3 / 0 | For 20 time units the unit is airborne (game_system §10.6): melee cannot reach it and it ignores ground effects; it uses its skills as normal. |
| Mobility | High Altitude | Mod of Take Off |  | Take Off lasts 40 time units. |
| Mobility | Sky Lord | K |  | The unit is always airborne, but has 30% less HP. |
| Skywatch | Scout Ahead | A | 2 / 0 | Reveals stealthed units in every row of one side for 10 time units. |
| Skywatch | Spotter | A | 3 / 0 | A unit is Marked for 20 time units. |
| Skywatch | Long Watch | P |  | Marks the unit places last 10 time units longer. |
| Skywatch | Watchtower | P |  | While airborne, the unit repeats Scout Ahead on its own every 20 time units. |
| Assault | Boundless Sky | K |  | While airborne, all the unit's skills have unlimited range. |
| Assault | Gust | A | 3 / 0 | Wind. Pushes a unit one row back. |
| Assault | Headwind | A | 4 / 0 | Wind. A unit's next action comes 5 time units later. |
| Assault | Turbulence | A | 4 / 0 | Wind. Every airborne unit, on both sides, lands. |
| Assault | Dust Up | A | 4 / 0 | Wind. A unit is Blind for 10 time units. |
