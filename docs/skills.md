# Everkin — Skills

Every active skill and keystone of the 28 classes in [class_system.md](class_system.md). Each creature's unique attack is in [creature-classes.md](creature-classes.md). Game-wide rules (delivery, targeting, interception, statuses, summons, ground effects) are in [game_system.md](game_system.md) and are not repeated here.

**Reading the tables**

- **Kind:** A = active skill, K = keystone (a passive; a creature has one keystone per class, fixed in the roster).
- **D / M:** Delay (1 is quickest) and Momentum cost.
- **Delivery:** `Melee` or `Projectile` (both can be intercepted) or `Direct` (cannot). **—** means the skill takes no target. A melee skill can be used only while its user stands in its own Front row (game_system §8.1).
- **Melee reach** in a text means every enemy in the enemy's Front row; the user must stand in its own Front row, whatever the delivery (game_system §8.1).
- **Numbers** are written for Power 5. Every damage, healing and Ward number is multiplied by the user's Power ÷ 5, rounded down, minimum 1 (game_system §5.2). Rule numbers (bump, Bleeding, Poisoned and ground damage) do not scale. Durations are in time units; 10 is about one turn.
- **Targets:** a targeted skill works on any legal unit on either side; "own side only" marks the exceptions. No skill targets an empty spot: ground effects go on a unit's spot or on a row (game_system §8.1).
- **Magical:** the actives of the magical classes and the Bard's songs (class_system §2) are magical; everything else is physical.

All numbers are first drafts, tuned in AI-versus-AI testing (game_system §2.5).

## Assassin

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Vanish | A | 4 / 3 | — | The Assassin gains stealth, wherever it stands. |
| Feint | A | 3 / 0 | Melee | Only from stealth: a unit is Confused for 10. The Assassin stays stealthed. |
| Backstab | A | 5 / 2 | Melee | Only from stealth: hit for 8–10 and the target Bleeds. Breaks stealth. |
| Unseen | K | | | The Assassin starts every battle stealthed. |
| Vanishing Kill | K | | | When the Assassin KOs a unit, it gains stealth again. |
| Venom Blades | K | | | Every hit of the Assassin Poisons the target for 20. |

## Tracker

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Target | A | 2 / 0 | Direct | Mark a unit. A Tracker has one Mark from Target at a time; marking another moves it. |
| Read Tracks | A | 2 / 0 | Direct | Pick a row. Every stealthed unit in it is revealed. |
| Hunter's Shot | A | 4 / 2 | Projectile | Hit a Marked unit for 9–11. |
| Sniff the Air | K | | | At battle start, the enemy with the highest Power becomes Marked. |
| Scent of Blood | K | | | When an enemy's HP falls below 20%, it becomes Marked. |
| Relentless | K | | | The Tracker's Marks last until their target is KO and cannot be removed. |

- A skill that reads a Mark (Hunter's Shot, the Champion's skills) works on any Marked unit, whoever placed the Mark (game_system §10.5).

## Bulwark

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Leap to Defend | A | 3 / 0 | Direct | Own side only. The Bulwark moves in front of an ally (any gap) and takes the next attack aimed at that ally. |
| Rescue | A | 5 / 2 | Direct | Own side only. Swap places with an ally below half HP. |
| Shield Bash | A | 4 / 0 | Melee | Hit for 6–7 and push the target one row back (bumps apply). |
| Shield Wall | K | | | While 2 or more of your units with Shield Wall stand side by side in one row, attacks on units behind them are always intercepted by them. |
| Hold Position | K | | | Enemy effects cannot push, pull or swap the Bulwark. |
| Unbreakable | K | | | The first time the Bulwark would be KO, it keeps 1 HP instead. |

- **Leap to Defend** lasts until the next attack of any delivery aimed at that ally; that attack is aimed at the Bulwark instead, once. It ends if the Bulwark is KO.
- **Shield Wall:** a melee or projectile attack on a unit behind the wall is intercepted by the nearest wall unit that overlaps that unit by at least 50%, with 100% chance instead of the roll; the hit is still full or partial as usual. Direct attacks pass.

## Keeper

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Ward | A | 3 / 0 | Direct | A unit gains a Ward of 4 until the Keeper's next turn. |
| Shield Dome | A | 7 / 5 | — | Every ally gains a Ward of 2 until the Keeper's next turn. |
| Ward Burst | A | 4 / 2 | Direct | A unit's Ward breaks and deals its remaining value to every enemy in melee reach. Needs the Keeper in its Front row; the warded unit may stand anywhere. |
| Thorned Ward | K | | | When an attack hits a Ward from the Keeper, the attacker takes 3. |
| Twin Wards | K | | | Ward also gives the same Ward to one ally next to the target. |
| Steadfast Ward | K | | | While a unit has a Ward from the Keeper, it cannot be Stunned or moved by enemies. |

## Mender

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Mend | A | 4 / 0 | Direct | Heal a unit for 8–10. |
| Rain of Mending | A | 6 / 2 | Direct | Pick a row: every unit in it heals 3–4, and every unit in the row behind it heals half that. |
| Searing Light | A | 4 / 0 | Direct | Hit for 7–8. Undead take double. |
| Overflowing Care | K | | | Healing beyond the target's max HP goes to its adjacent ally with the lowest HP. |
| Regrowth | K | | | A unit the Mender heals also regains 2 HP every 10 for 20. |
| Triage | K | | | Mend heals double on units below 25% HP. |

## Cleric

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Absolve | A | 4 / 0 | Direct | Remove every harmful status from a unit; its next turn is pushed back 2. |
| Dispel | A | 3 / 0 | Direct | Remove every helpful status (Ward, Haste, stealth, enchantments and others) from a unit. |
| Turn Back | A | 3 / 0 | Direct | Move one harmful status from a unit onto the unit that applied it. |
| Sanctity | K | | | The Cleric cannot get harmful statuses. |
| Purifying Aura | K | | | Harmful statuses on allies next to the Cleric last half as long. |
| Judgment | K | | | Each status the Cleric's Dispel removes deals 4 to that unit. |

- **Turn Back:** if the unit has several harmful statuses whose applier is still standing, the seeded roll picks one, each with equal chance.

## Lifebinder

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Burnout Heal | A | 7 / 6 | — | The Lifebinder and its adjacent allies heal 6–8; then the Lifebinder is KO (game_system §10.4). |
| Life for Life | A | 6 / 3 | Direct | The Lifebinder gives up half its current HP to revive a KO unit with that much HP. |
| Bound Resurrect | A | 8 / 4 | Direct | Revive a KO unit at 25% of its max HP (rounded down, minimum 1); it and the Lifebinder share one HP pool (game_system §10.4). |
| Martyr | K | | | When the Lifebinder is KO, every ally heals 6–8. |
| Phoenix | K | | | Life for Life costs only a quarter of the Lifebinder's current HP. |
| Lifeblood | K | | | When an ally is KO, the Lifebinder heals 5. |

## Enchanter

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Echo Enchant | A | 6 / 3 | Direct | A unit's next skill happens twice: the second use is free (no Delay, no Momentum) and hits the same target. |
| Phase Weapon | A | 3 / 0 | Direct | A unit's next attack cannot be intercepted. |
| Piercing Enchant | A | 4 / 0 | Direct | A unit's next attack also hits the unit behind its target. |
| Twin Enchant | K | | | Each enchantment also goes on one ally next to the target. |
| Lingering Magic | K | | | Each enchantment from the Enchanter lasts for two uses instead of one. |
| Spell Ward | K | | | A unit carrying an enchantment from the Enchanter cannot be Silenced or Stunned. |

- **Enchantments** are helpful statuses on the enchanted unit. An unused enchantment fades after 20; any number can be active together. Phase Weapon and Piercing Enchant are used up by the unit's next attack, Echo Enchant by its next skill.

## Transmuter

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Transmute | A | 4 / 0 | Direct | A unit's Power and Speed swap values for 20. |
| Glass Body | A | 4 / 0 | Direct | The next hit on a unit deals 50% more damage. |
| Petrify | A | 7 / 4 | Direct | A unit turns to stone: it is Stunned for 10. |
| Lasting Change | K | | | Transmute lasts until the end of battle, but only one unit can be transmuted by the Transmuter at a time. |
| Brittle Glass | K | | | Glass Body makes the next hit deal double instead of 50% more. |
| Mirror Form | K | | | Transmute can instead swap Power and Speed between two units. |

## Hexer

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Curse of Betrayal | A | 6 / 3 | Direct | For 20, healing the unit would receive goes to the Hexer's unit with the lowest HP instead. |
| Curse of Weight | A | 4 / 0 | Direct | For 20, a unit cannot Move or become airborne. |
| Withering Hex | A | 4 / 0 | Direct | A unit takes 3 every 10 for 40. |
| Witch's Patience | K | | | The Hexer's curses last until removed, but it can keep only one curse at a time. |
| Lingering Malice | K | | | When an enemy removes one of the Hexer's curses or hexes, the remover is Silenced for 10. |
| Spreading Curse | K | | | When a cursed unit is KO, its curse jumps to the nearest enemy. |

- **Curses** are Curse of Betrayal and Curse of Weight; Withering Hex is a hex. With Witch's Patience a new curse removes the Hexer's earlier one (on any unit).
- **Curse of Betrayal:** ties for lowest HP go to the seeded roll. If the Hexer's side has no unit that can be healed, the cursed unit is healed as usual.

## Pyromancer

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Fireball | A | 5 / 2 | Projectile | Hit for 8–9 and set fire to the target's spot. |
| Kindle | A | 2 / 0 | Direct | Set fire to a unit's spot and the spots next to it. |
| Row of Fire | A | 6 / 3 | Direct | Set fire to a whole row. |
| Wildfire | K | | | The Pyromancer's burning ground lasts twice as long. |
| Inferno | K | | | Burning ground from the Pyromancer deals double damage. |
| Ashwalker | K | | | The Pyromancer takes no damage from burning ground and has +1 Speed while standing on it. |

- Fire from these skills burns 20. "The spots next to" a spot and "a whole row" are defined in game_system §12.2. An intercepted Fireball hits the interceptor and sets fire to its spot.

## Stormcaller

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Lightning Bolt | A | 5 / 2 | Direct | Hit for 9–10. |
| Chain Lightning | A | 5 / 3 | Direct | Hit a unit for 1–2 and Stun it for 5, then jump to an adjacent unit and do the same, three more times. |
| Tempest | A | 8 / 5 | Direct | Push every unit in one row one row back (bumps apply). |
| Conductor | K | | | Stuns from the Stormcaller last 5 longer. |
| Static Field | K | | | A unit that hits the Stormcaller in melee is Stunned for 5. |
| Eye of the Storm | K | | | The Stormcaller cannot be pushed, pulled or Stunned. |

- **Chain Lightning** jumps to a random adjacent living unit (game_system §8.4) that this chain has not hit yet, and ends early when none is left. Lightning on a unit standing in water spreads as game_system §12.2 says.
- **Tempest** picks any row of either side that has a living unit. Units in the Rear row stay. The row moves together: each unit keeps its coordinate, so units side by side stay side by side.

## Cryomancer

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Ice Lance | A | 4 / 2 | Projectile | Hit for 7–8; a Frozen target takes double. |
| Freeze | A | 7 / 3 | Direct | A unit is Frozen. |
| Frost Nova | A | 6 / 3 | Direct | Every enemy in melee reach takes 1–2 and is Frozen. |
| Cold Heart | K | | | Frozen from the Cryomancer lasts 1 more turn, but it can keep only one unit Frozen at a time. |
| Brittle | K | | | Frozen enemies take 50% more damage. |
| Steam Burst | K | | | When fire is set on an iced spot, both end and the unit there is Confused for 10. |

- **Cold Heart:** Frozen from the Cryomancer skips the unit's next 2 turns. Freezing another unit thaws the earlier one; when one skill freezes several units, the seeded roll keeps one.
- **Brittle:** a Frozen enemy takes the hit's damage × 3 ÷ 2, rounded down, before Ward.

## Berserker

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Rampage | A | 5 / 2 | Melee | Hit for 10–12. The Berserker takes 3. |
| Sweeping Kick | A | 5 / 2 | Melee | Push every enemy in melee reach one row back (bumps apply). |
| Battle Roar | A | 4 / 0 | Direct | Every enemy in melee reach has its next turn pushed back 5. |
| Reckless Swing | K | | | The Berserker's hits deal double damage, but it takes half of the damage it deals (rounded down). |
| Last Stand | K | | | The Berserker cannot Move or be moved, but deals 50% more damage. |
| Unstoppable | K | | | Below a quarter of its HP, the Berserker cannot be Immobilized or Frozen, and its turns cannot be pushed back. |

- **Sweeping Kick** pushes the row as a group, like Tempest, and cannot be intercepted.
- **Damage bonuses add up:** Reckless Swing +100% and Last Stand +50% make +150% (× 2.5), applied to the rolled amount (rounded down) before interception and Wards.
- **Reckless Swing's recoil** is half (rounded down) of the HP the target and its interceptors actually lost. It cannot be reduced and can KO the Berserker.
- **Last Stand** blocks the Move action, every push, pull or swap of the Berserker by anyone, and the Berserker's own moves.
- **Unstoppable:** Immobilized and Frozen are refused and its turn cannot be pushed back (pulling it forward still works).

## Champion

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Challenge | A | 2 / 0 | Direct | Mark a unit. A Champion has one Mark from Challenge at a time; marking another moves it. |
| Single Combat | A | 6 / 3 | Direct | For 10, the Champion and one Marked unit can be targeted only by each other. |
| Execution | A | 7 / 4 | Melee | A Marked unit below 25% HP is KO. |
| Hunt Down | K | | | When a Marked enemy moves or is moved, the Champion Stuns it for 5. |
| Duelist | K | | | The Champion deals 50% more damage to the unit it has Marked. |
| Glory | K | | | When the Champion KOs its Marked unit, it heals a quarter of its max HP. |

- **Single Combat:** no other unit can target either duellist (helpful skills included); each may still target itself and others. It ends if either is KO.
- **Execution** cannot be intercepted and ignores Wards: the unit's HP goes to 0.
- **Hunt Down** reacts to any movement of a Marked enemy, its own Move included, whoever marked it.

## Tactician

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Swap Turns | A | 5 / 2 | Direct | Own side only. Two allies swap their places on the timeline. |
| Press the Advantage | A | 4 / 2 | Direct | A unit's next skill costs no Momentum. |
| Coordinated Strike | A | 5 / 3 | Direct | Pick an enemy. Two of your allies make their unique attack on it right now. |
| All In | K | | | The Tactician's Momentum costs are halved (rounded up), but Momentum gained from its hits is 0. |
| Inspiring Presence | K | | | When an ally next to the Tactician KOs an enemy with its own skill, Momentum moves 1 more toward your side. |
| Initiative | K | | | At battle start, every ally's first turn is pulled forward 5. |

- **Swap Turns:** the two allies exchange the due times (and tie order) of their next turns.
- **Press the Advantage** has no time limit; the unit's next skill costs 0 Momentum and needs no lead.
- **Coordinated Strike:** the two allies are picked at random among those whose attack can hit the enemy now (a melee attack needs the ally in its Front row and a target that is not airborne). If only one can, only it attacks; if none can, nothing happens. The attacks are free.
- **All In** halves the cost of every skill of the Tactician, including the lead it needs.

## Commander

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Advance | A | 3 / 0 | Direct | Own side only. Move every ally in one row one row forward (bumps apply). |
| Switch Places | A | 3 / 0 | Direct | Own side only. Two allies swap places. |
| Rotation | A | 5 / 2 | — | Every ally moves one row back, and the Rear row moves to the Front. |
| Drill | K | | | An ally the Commander moves has its next turn pulled forward 3. |
| Rally | K | | | When an ally is KO, every other ally's next turn is pulled forward 3. |
| Vanguard | K | | | An ally the Commander moves into the Front row gains a Ward of 3. |

- **Rotation** turns the three rows: Front to Middle, Middle to Rear, Rear to Front. KO bodies, Immobilized units and units immune to being moved stay. Movers keep their left-to-right order and coordinates and are placed among the units that stayed. If a row would hold more than 6 units, nothing moves.
- **Drill** also applies to the Commander when its own skill moves it.

## Bard

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Song of Haste | A | 4 / 0 | — | Song. Allies in the Bard's row have +1 Speed for 20. |
| War Drums | A | 4 / 0 | — | Song. For 20, the Bard and its adjacent allies cannot be Stunned or Frozen. |
| Lullaby | A | 4 / 0 | Direct | Song. Every enemy in one row has its next turn pushed back 5. |
| Grand Chorus | K | | | The Bard's songs reach every ally, but only one of its songs can be active at a time. |
| Rhythm | K | | | When an ally uses the same skill twice in a row, the second use has 1 less Delay. |
| Crescendo | K | | | The Bard's songs last 10 longer. |

- **Songs** are magical. Song of Haste and War Drums count the Bard as an ally and last 20; casting one again restarts it. The same song from several Bards does not add up: the strongest applies. Lullaby is instant, so Crescendo and the one-song limit of Grand Chorus do not touch it.
- **Rhythm:** a Move or Skip Turn between the two uses breaks the streak.

## Thief

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Melt Away | A | 4 / 2 | — | The Thief gains stealth, even in the Front row. |
| Lift a Skill | A | 6 / 3 | Direct | Use a copy of the last active skill the target used, with the Thief's own Power. |
| Snatch | A | 4 / 0 | Melee | Hit for 5–6 and steal one helpful status from the target. |
| Escape Artist | K | | | The first time the Thief drops below half HP, it gains stealth. |
| Master Thief | K | | | Snatch steals every helpful status from the target, not just one. |
| Shadow Purse | K | | | Each time the Thief steals a status, Momentum moves 1 toward your side. |

- **Lift a Skill:** the player picks the enemy and, if the copy needs a target, a legal target. The copy is free and uses the Thief's Power. Any skill used with a turn action counts, unique attacks included; Move, Skip Turn, keystones and Lift a Skill itself never do. Only copies that need no target or one unit target are possible. A magical copy cannot be made while the Thief is Silenced. A melee copy follows the Front-row rule.
- **Stealing:** a stolen status moves to the Thief with its remaining time; the seeded roll picks which. Stealth cannot be stolen. Snatch steals only if its hit reaches the target itself (landed or absorbed by a Ward, not taken by an interceptor).

## Saboteur

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Flash Powder | A | 3 / 0 | Direct | Every enemy in melee reach is Blind for 20. |
| Cut the Strings | A | 5 / 2 | Direct | Pick an enemy that owns auto events: its next auto event is removed from the timeline. |
| Jam | A | 3 / 0 | Projectile | A unit's next skill has +5 Delay. |
| Wrecker | K | | | The Saboteur deals double damage to constructs, totems, traps and barricades. |
| Chaos Agent | K | | | Every skill of the Saboteur that hits an enemy also pushes its next turn back 2. |
| Disarm Trap | K | | | The Saboteur sets off enemy traps without taking their effect. |

- **Jam:** an interceptor takes the Delay instead.
- **Disarm Trap:** when the Saboteur hits an enemy trap, the trap goes off and is removed, but its effect does not reach the Saboteur.

## Engineer

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Bolt Turret | A | 5 / 2 | — | Place a turret (HP 8). Each time the Engineer takes a turn, the turret hits the nearest enemy for 2–3. |
| Barricade | A | 4 / 0 | — | Place a barricade (HP 10). |
| Net Trap | A | 5 / 1 | — | Place a trap (HP 4). A unit that hits it has +3 Delay on its next skill. |
| Reinforced | K | | | The Engineer's turrets, barricades and traps have double HP. |
| Self-Repair | K | | | The Engineer's turrets and barricades regain 2 HP each time the Engineer takes a turn. |
| Rapid Fire | K | | | Bolt Turret fires twice each time. |

- **Turret shots:** each time the Engineer takes an action (a skill, Move or Skip Turn; a skipped turn does not count), every turret queues one auto event owned by the Engineer, due now; the shots resolve right after the action in the order the turrets were placed. Each shot shifts Momentum like a separate action. A KO'd Engineer loses its queued shots; the turrets stay.
- **Turret targets:** when the shot resolves it picks the nearest legal enemy (game_system §8.5; stealthed units are not legal) and uses the Engineer's Power. A shot is a projectile hit and can be intercepted. With no legal enemy the shot is lost.

## Chemist

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Poison Gas | A | 4 / 0 | Direct | Poison ground on a unit's spot and the spots next to it, for 20. |
| Firebomb | A | 5 / 2 | Projectile | Fire on a unit's spot and the spots next to it, for 20. |
| Healing Draught | A | 3 / 0 | Projectile | A unit heals 6–7. |
| Showman | K | | | Blind and Stunned from the Chemist last twice as long, but its skills that deal damage have +2 Delay. |
| Potent Brew | K | | | Healing Draught also removes one harmful status. |
| Volatile | K | | | The Chemist's fire and poison ground last twice as long. |

- **Showman** covers every skill and the attack of the creature; Delay is clamped to 10.
- An intercepted Firebomb lands on the interceptor's spot.

## Necromancer

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Raise Undead | A | 8 / 4 | Direct | Turn a KO'd body of either side into an undead of yours (HP 12, Power 4), and raise one more random body. |
| Corpse Explosion | A | 5 / 2 | Direct | A body or an undead of the Necromancer explodes: 6 to every unit next to it, and they are Poisoned for 20. |
| Desecrate | A | 5 / 2 | Direct | A KO'd body cannot be revived for 40. |
| Lich Form | K | | | Healing hurts the Necromancer, but its undead have +2 Power. |
| Bone Armor | K | | | For each undead it has, the Necromancer has a Ward of 2 that renews every turn. |
| Death's Bounty | K | | | When a unit is KO next to the Necromancer, the Necromancer heals 5. |

- **Raise Undead:** the player picks the body and a free spot on your side for it. The second body is picked by the seeded roll among the remaining KO bodies of either side and goes on a random free spot; it is skipped when there is no body or no room. Undead follow game_system §11.5.
- **Corpse Explosion:** "next to" uses game_system §8.4 adjacency on the side of the exploding unit, so allies and enemies alike if they stand there. An exploding undead takes the 6 too; a body stays where it is.
- **Desecrate** blocks every revive (Life for Life, Bound Resurrect, restoring an undead); Raise Undead still works.

## Summoner

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Clay Golem | A | 8 / 4 | — | Summon a golem (HP 25, Speed 3) that makes melee hits for 5–6. |
| Rune Golem | A | 8 / 4 | — | Summon a rune golem (HP 18, Speed 4) that makes melee hits for 3–4; each hit Silences the target for 10. |
| Animate Blades | A | 5 / 2 | — | Summon two sets of flying blades (HP 6, Speed 7) that make projectile hits for 2–3. They last 30. |
| Golem Heart | K | | | The Summoner keeps only one golem (a new one replaces the old), but its golems have double HP (Clay 50, Rune 36). |
| Portal | K | | | The Summoner's summons appear on a random free spot of your side instead of being placed. |
| Quickening | K | | | The Summoner's summons take their first turn at once. |

- Golems and blades take their own turns (game_system §11). A golem placed outside your Front row cannot attack.

## Totemist

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Healing Totem | A | 5 / 2 | — | Place a totem (HP 8): every 10, allies in its row heal 2. |
| Spirit Link Totem | A | 6 / 3 | — | Place a totem (HP 10): damage to allies in its row is split evenly between them. |
| Totem of Return | A | 7 / 3 | — | Place a totem (HP 6): the first ally in its row that would be KO keeps 1 HP instead, and the totem breaks. |
| Great Totem | K | | | The Totemist keeps only one totem (a new one replaces the old), but it has triple HP and also affects the rows next to it. |
| Sturdy Totems | K | | | Totems of the Totemist have double HP. |
| Spirit Shield | K | | | When a totem of the Totemist breaks, every ally in its row gains a Ward of 4. |

- There is no totem limit without Great Totem.
- **Healing Totem:** a repeating auto event owned by the totem, so it goes on if the Totemist is KO. Each living ally in the totem's row, the totem included, heals 2 (scaled to the Totemist's Power).
- **Spirit Link Totem:** when an ally in its row takes damage, the total is split evenly among the living allies in the row other than the totem, rounded down for the others, the rest staying with the hurt unit. Each share meets that unit's Ward but is not a new hit.
- **Totem of Return** is removed when it breaks, not KO'd.

## Ritualist

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Rain Dance | A | 6 / 3 | Direct | Water on every spot of one row for 40; allies standing in it heal 4. |
| Hoarfrost | A | 5 / 2 | Direct | Ice on a unit's spot for 20. |
| Rite of Mists | A | 6 / 3 | — | For 20, projectile hits from units standing on water deal half damage (rounded down, minimum 1). |
| Deluge | K | | | Water from the Ritualist lasts until the end of battle. |
| Tidecaller | K | | | Allies standing on water have 1 less Delay on every skill. |
| Frostbinder | K | | | A unit standing on the Ritualist's ice is Immobilized. |

- **Rain Dance** picks a row of either side and heals once, when it is cast. Rite of Mists covers turret and blade shots too; casting it again restarts the 20.

## Spiritwalker

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Puppet Soul | A | 6 / 3 | Direct | Possession. A unit hits a unit next to it at once with its unique attack. |
| Life Swap | A | 8 / 5 | Direct | Soul swap. Two units swap their current HP (each capped at its max). |
| Forced Step | A | 4 / 0 | Direct | Possession. A unit moves to a free spot of your choice on its own side. |
| Soul Anchor | K | | | Enemies cannot possess, move or soul-swap the Spiritwalker. |
| Haunting | K | | | When the Spiritwalker is KO, the unit that KO'd it is Confused for 20. |
| Puppeteer | K | | | Puppet Soul hits deal double damage. |

- **Puppet Soul:** the player picks a unit and one unit next to it (game_system §8.4). The hit is free and uses the first unit's Power. It ignores the Front-row rule, but a melee attack still cannot hit an airborne unit.
- **Forced Step** is a forced move: Immobilized units and Hold Position stop it.

## Skyborne

| Skill | Kind | D / M | Delivery | Effect |
|---|---|---|---|---|
| Take Off | A | 3 / 0 | — | The Skyborne is airborne for 40. |
| Gust | A | 3 / 0 | Direct | Wind. Push a unit one row back (bumps apply). |
| Dive | A | 5 / 2 | Melee | Only while airborne: hit for 9–11, then land. |
| Sky Lord | K | | | The Skyborne is always airborne, but has 30% less HP (rounded down). |
| Eagle Eye | K | | | While airborne, the Skyborne reveals stealthed enemies. |
| Lift | K | | | Take Off also makes one ally next to the Skyborne airborne for the same time. |

- **Take Off** again while airborne restarts the timer. A unit under Curse of Weight cannot use it.
- **Dive** works from any row and cannot be intercepted; it cannot hit an airborne unit.
- **Sky Lord:** at battle start max HP and HP become 70% (rounded down, minimum 1) and the unit is airborne with no end. If something makes it land, it takes to the air again at once unless Curse of Weight forbids it.
