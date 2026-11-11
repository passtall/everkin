# Spec: branches become classes

**Status:** DRAFT 2026-11-11, being decided in quiz rounds (rounds 1 to 8 and the creature assignment applied; doc rewrite waits for the owner's go). Separate from [spec.md](spec.md) (battle UI work order). Until this spec is final, the current rules in docs/ stay valid; when it is final, its decisions are written into class_system.md, skills.md, game_system.md, creature-classes.md and ui_spec.md, and this file becomes the work order for the code.

## 0. Owner's request (2026-11-11)

1. The branches of the current professions become classes (roughly as they are; they will be adjusted later).
2. Every creature gets **two** of the new classes.
3. Skill points and skill trees are gone.
4. A creature has **all** skills of its classes unlocked at all times.
5. Keystones stay as plain passives. Creatures with the same class may still have different keystones (a keystone is assigned per creature, not per class).
6. Classes may have damage skills again; most classes should have at least one.

First step: go through every new class and decide whether it stays, merges into another or is removed.

## 1. Classes (round 1 decided 2026-11-11)

The 12 professions had 38 branches in [skills.md](docs/skills.md). Round 1 (https://claude.ai/artifact/TuKYsxEXMpwNqNjvSsc4hd, db doc answers/round1) kept or merged each one; none was removed outright. **30 classes** (29 after round 2 merged Bodyguard into Bulwark):

| Old profession | Classes (merged branches in brackets) |
|---|---|
| Hunter | Stalker, Tracker (+ Predator); Trapper merged into Engineer |
| Guardian | Bulwark, Bodyguard, Keeper |
| Healer | Mender, Cleric, Lifebinder |
| Sorcerer | Enchantment, Transmutation, Malediction |
| Elementalist | Pyromancy, Stormcalling, Cryomancy |
| Warrior | Berserker (+ Whirlwind), Champion (+ Footwork) |
| Leader | Tactician, Commander, Bard |
| Rogue | Thief, Assassin, Saboteur |
| Tinker | Engineer (+ Trapper), Chemist (+ Pyrotechnician) |
| Summoner | Necromancer, Golemancer + Conjurer (one class, name open) |
| Primalist | Totemist, Ritualist, Spiritwalker |
| Flying | Flying (Mobility, Skywatch and Assault in one class) |

New skill requested for **Stalker**: a real backstab, only from stealth, range 1, high damage (round 2 suggests "Pounce": D 5 / M 2, Melee · D1, hit for 12–14, breaks stealth).

Round 2 (which skills each class keeps, db doc answers/round2), answered 2026-11-11 for Stalker to Cleric; every skill below Cleric kept:

- **Stalker:** Stalk, Creep Closer, Freeze in Place and Lie in Wait cut; Vanish kept (no row requirement); Pounce kept; add a skill that Confuses.
- **Tracker:** Scent, Cut Off Retreat and Pack Strike cut; Scent of Blood and Sniff the Air become keystones.
- **Bulwark:** Bodyguard merged into Bulwark (29 classes). Full Cover, Living Wall, Take a Charge, Follow the Charge, Distract and Pledge cut. Shield Wall becomes a keystone that works with every unit that has it; Hold Position a keystone. Leap to Defend: the Bulwark moves in front of the ally and takes the attack right away.
- **Keeper:** Lasting Ward and Keeper of the Gate cut.
- **Mender, Cleric:** all kept.

## 1a. Class shape (owner, 2026-11-11)

Each class has exactly **3 active skills** and **3 keystones** (powerful passives). A creature gets one keystone of each of its two classes, so creatures of the same class can differ. Other passives are gone; modifiers are folded into their skill.

Round 3 (pick the 3 actives and 3 keystones per class, with Claude's suggestions and new skills including damage skills): same page, db doc answers/round3 {pick:{"Class/Skill": true|false, only changes from the suggestion}, n:{"Class/Skill" or "class:Class": note}, note}. List: project file everkin/class-branches-round3.md.

## 1b. The 28 classes (rounds 3 and 4, answered 2026-11-11)

Skill texts for new and reworded skills: project file everkin/class-branches-round3.md. Round 4 (db doc answers/round4, list everkin/class-branches-round4.md) filled the classes that were short. Round 4b (db doc answers/round4b) let the owner pick the missing keystones himself; Flying's third keystone is Lift (round 4c, db doc answers/round4c: Take Off also makes one ally next to the unit airborne for the same time). Keystone texts: project files everkin/class-branches-round4.md and class-branches/round4c.json.

| Class | Actives | Keystones |
|---|---|---|
| Assassin | Vanish, Feint, Backstab | Unseen, Vanishing Kill, Venom Blades |
| Tracker | Target, Read Tracks, Hunter's Shot | Sniff the Air, Scent of Blood, Relentless |
| Bulwark | Leap to Defend, Rescue, Shield Bash | Shield Wall, Hold Position, Unbreakable |
| Keeper | Ward, Shield Dome, Ward Burst | Thorned Ward, Twin Wards, Steadfast Ward |
| Mender | Mend, Rain of Mending, Searing Light | Overflowing Care, Regrowth, Triage |
| Cleric | Absolve, Dispel, Turn Back | Sanctity, Purifying Aura, Judgment |
| Lifebinder | Burnout Heal, Life for Life, Bound Resurrect | Martyr, Phoenix, Lifeblood |
| Enchantment | Echo Enchant, Phase Weapon, Piercing Enchant | Twin Enchant, Lingering Magic, Spell Ward |
| Transmutation | Transmute, Glass Body, Petrify | Lasting Change, Brittle Glass, Mirror Form |
| Malediction | Curse of Betrayal, Curse of Weight, Withering Hex | Witch's Patience, Lingering Malice, Spreading Curse |
| Pyromancy | Fireball, Kindle, Row of Fire | Wildfire, Inferno, Ashwalker |
| Stormcalling | Lightning Bolt, Chain Lightning, Tempest | Conductor, Static Field, Eye of the Storm |
| Cryomancy | Ice Lance, Freeze, Frost Nova | Cold Heart, Brittle, Steam Burst |
| Berserker | Rampage, Sweeping Kick, Battle Roar | Reckless Swing, Last Stand, Unstoppable |
| Champion | Challenge, Single Combat, Execution | Hunt Down, Duelist, Glory |
| Tactician | Swap Turns, Press the Advantage, Coordinated Strike | All In, Inspiring Presence, Initiative |
| Commander | Advance, Switch Places, Rotation | Drill, Rally, Vanguard |
| Bard | Song of Haste, War Drums, Lullaby | Grand Chorus, Rhythm, Crescendo |
| Thief | Melt Away, Lift a Skill, Snatch | Escape Artist, Master Thief, Shadow Purse |
| Saboteur | Flash Powder, Cut the Strings, Jam | Wrecker, Chaos Agent, Disarm Trap |
| Engineer | Bolt Turret, Barricade, Net Trap | Reinforced, Self-Repair, Rapid Fire |
| Chemist | Poison Gas, Firebomb, Healing Draught | Showman, Potent Brew, Volatile |
| Necromancer | Raise Undead, Corpse Explosion, Desecrate | Lich Form, Bone Armor, Death's Bounty |
| Golemancer | Clay Golem, Rune Golem, Animate Blades | Golem Heart, Portal, Quickening |
| Totemist | Healing Totem, Spirit Link Totem, Totem of Return | Great Totem, Twin Totems, Spirit Shield |
| Ritualist | Rain Dance, Hoarfrost, Rite of Mists | Deluge, Tidecaller, Frostbinder |
| Spiritwalker | Puppet Soul, Life Swap, Forced Step | Soul Anchor, Haunting, Puppeteer |
| Flying | Take Off, Gust, Dive | Sky Lord, Eagle Eye, Lift |

Changes from the owner's notes:
- **Assassin** merged with **Stalker** (28 classes); round 4 renamed the merged class **Assassin**. Pounce is cut; **Backstab**: only from stealth, hit for 8–10 and the target Bleeds, no double damage, breaks stealth.
- **Shield Wall** already works with 2 units side by side.
- **Thorned Ward:** the attacker takes 3 when it attacks a Ward (not only when the Ward breaks).
- **Frost Nova** and **Chain Lightning** also deal low damage.
- **Conductor** needs no water: Stuns from the Stormcaller last 5 time units longer.
- **Jam** adds more Delay.
- **Life Swap** is a real HP swap (current HP values, not shares).
- **Lasting Change** can affect only one unit at a time.
- **Grand Chorus:** songs reach every ally, but the Bard can have only one song at a time.
- **Glass Body:** the next hit deals 50% more; **Brittle Glass** makes it double.
- **Mirror Form:** Transmute swaps Power and Speed between two units.
- **Boundless Sky** dropped: flyers already have unlimited range (owner confirmed 2026-11-11).

## 1c. Names and rules (round 5, answered 2026-11-11)

- **Names.** The six school classes are named after their user: Enchanter, Transmuter, Hexer (was Malediction), Pyromancer, Stormcaller, Cryomancer. The merged Golemancer and Conjurer class is called **Summoner**. Flying is renamed **Skyborne**.
- **Two classes per creature.** Claude proposes two classes for each creature, fitting its art, and the owner reviews them in a creature quiz. Both classes may come from the same old profession. Old professions guide the proposal but are not required. Every keystone of every class must be used by at least one creature.
- **Keystones.** A creature has two keystones, one from each of its classes, fixed per creature in the roster (no player choice, no builds).
- **Attack Type is dropped.** Each creature has its own unique attack instead of one of the nine shared Attack Type attacks.
- **Cost** stays as it is until AI-versus-AI testing; it is recalculated then.
- **Skill points, skill trees, the tree editor and default builds are removed** everywhere (docs, tree editor screen, content/default_builds.json).

## 1d. Unique attacks (round 6, answered 2026-11-11)

- An attack has a **name, delivery (Melee, Projectile or Direct), range, damage, and at most one small extra**.
- **Strength:** the same band as the old shared attacks: 3 to 6 damage at Power 5, Delay 3 or 4, no Momentum cost. An extra is paid for with less damage.
- **Extras:** a short list guides them (Bleed, Poison, Stun, burning ground, push, splash, longer reach, cannot be intercepted, life steal, each at a low chance or small size), but anything that fits the art may be proposed; the owner rejects what is too strong.
- Attacks always hit an enemy; healing and support stay in class skills.
- **Animation:** a few shared styles (lunge, swing, shot, throw, bolt, breath, pulse); each attack picks one and a colour.
- **Claude writes all 319 attacks** in the creature quiz, together with each creature's two classes and two keystones.

## 1e. Class sheet (texts as of 2026-11-11)

Every class with its 3 actives and 3 keystones. Old class names inside the texts (Warrior, Tinker, Leader, Guardian and so on) mean the class the skill now belongs to. Delivery and range of new skills are still open.

**Assassin**  
Actives: *Vanish*: Gain stealth, wherever the Assassin stands.; *Feint*: Only from stealth: a unit is Confused for 10 time units. The Stalker stays stealthed.; *Backstab*: Only from stealth: hit for 8–10 and the target Bleeds. Breaks stealth.  
Keystones: *Unseen*: The Stalker starts every battle stealthed.; *Vanishing Kill*: When the Stalker KOs a unit, it gains stealth again (Shadow Kill folded in).; *Venom Blades*: Every hit of the Stalker Poisons the target for 20 time units.

**Bard**  
Actives: *Song of Haste*: Allies in the Bard's row have +1 Speed for 20 time units.; *War Drums*: For 20 time units, allies next to the Bard cannot be Stunned or Frozen.; *Lullaby*: Song. Every enemy in one row has its next turn pushed back 5 time units.  
Keystones: *Grand Chorus*: The Bard's songs reach every ally, but the Bard can have only one song active at a time.; *Rhythm*: When an ally uses the same skill twice in a row, the second use has 1 less Delay.; *Crescendo*: The Bard's songs last 10 time units longer.

**Berserker**  
Actives: *Rampage*: Hit for 10–12. The Berserker takes 3.; *Sweeping Kick*: Push every unit in melee reach one row back (bumps apply).; *Battle Roar*: Every unit in melee reach has its next turn pushed back 5 time units.  
Keystones: *Reckless Swing*: The Warrior's hits deal double damage, but the Warrior takes half of the damage it deals (rounded down).; *Last Stand*: The Warrior cannot Move or be moved, but deals 50% more damage.; *Unstoppable*: Below a quarter of its HP, the Warrior cannot be Immobilized or Frozen, and its turns cannot be pushed back.

**Bulwark**  
Actives: *Leap to Defend*: Own side only. The Bulwark moves in front of an ally anywhere on your side (any gap) and takes the next attack aimed at that ally (melee, projectile or direct), once.; *Rescue*: Own side only. Swap places with an ally within distance 2 that is below half HP.; *Shield Bash*: Hit for 6–7 and push the target one row back (bumps apply).  
Keystones: *Shield Wall*: While 2 or more of your units that have Shield Wall stand side by side in one row, attacks on units behind them are always intercepted by them.; *Hold Position*: Enemy effects cannot push, pull or swap the Guardian.; *Unbreakable*: The first time the Bulwark would be KO, it keeps 1 HP instead.

**Champion**  
Actives: *Challenge*: Mark a unit (one Mark from this Warrior at a time).; *Single Combat*: For 10 time units, the Warrior and one Marked unit can be targeted only by each other.; *Execution*: A Marked unit below 25% HP is KO.  
Keystones: *Hunt Down*: When a Marked enemy is moved, the Warrior Stuns it for 5 time units.; *Duelist*: The Champion deals 50% more damage to the unit it has Marked.; *Glory*: When the Champion KOs its Marked unit, it heals a quarter of its max HP.

**Chemist**  
Actives: *Poison Gas*: Poison ground on a unit's spot and the spots beside it, for 20 time units.; *Firebomb*: Fire on a unit's spot and the spots beside it, for 20 time units.; *Healing Draught*: Throw a potion: a unit heals 6–7.  
Keystones: *Showman*: The Tinker's Blind and Stunned last twice as long, but the Tinker's skills that deal damage have +2 Delay.; *Potent Brew*: Healing Draught also removes one harmful status.; *Volatile*: The Chemist's fire and poison ground last twice as long.

**Cleric**  
Actives: *Absolve*: Remove every harmful status from one unit; its next turn is pushed back 2.; *Dispel*: Remove every helpful status (Ward, Haste, stealth and others) from a unit within distance 3.; *Turn Back*: Move one harmful status from a unit onto the unit that applied it. If the unit has several harmful statuses whose applier is still standing, the battle's seeded roll picks one, each with equal chance (decided 2026-11-08).  
Keystones: *Sanctity*: The Cleric cannot get harmful statuses.; *Purifying Aura*: Harmful statuses on allies next to the Cleric last half as long.; *Judgment*: Each status the Cleric's Dispel removes deals 4 damage to that unit.

**Commander**  
Actives: *Advance*: Own side only. Move every ally in one row one row forward (bumps apply).; *Switch Places*: Own side only. Two allies swap places.; *Rotation*: Own side only. Every ally moves one row back, and the Rear row moves to the Front.  
Keystones: *Drill*: An ally the Leader moves has its next turn pulled forward 3 time units.; *Rally*: When an ally is KO, every other ally's next turn is pulled forward 3 time units.; *Vanguard*: An ally the Commander moves into the Front row gains a Ward of 3.

**Cryomancer**  
Actives: *Ice Lance*: Hit for 7–8; Frozen units take double.; *Freeze*: A unit is Frozen.; *Frost Nova*: Every enemy in melee reach of the Cryomancer takes 1–2 and is Frozen.  
Keystones: *Cold Heart*: Frozen from the Elementalist lasts 1 more turn, but it can keep only one unit Frozen at a time.; *Brittle*: Frozen enemies take 50% more damage.; *Steam Burst*: When fire is set on an iced spot, both end and the unit there is Confused for 10 time units.

**Enchanter**  
Actives: *Echo Enchant*: A unit's next skill happens twice: the second use is free (no Delay, no Momentum) and hits the same target.; *Phase Weapon*: A unit's next attack has unlimited range and cannot be intercepted (Phasing Strike folded in).; *Piercing Enchant*: A unit's next attack also hits the unit behind its target.  
Keystones: *Twin Enchant*: Each enchantment also goes on one ally next to the target.; *Lingering Magic*: Each enchantment from the Enchanter lasts for two uses instead of one.; *Spell Ward*: A unit carrying an enchantment from the Enchanter cannot be Silenced or Stunned.

**Engineer**  
Actives: *Bolt Turret*: Places a turret (HP 8). Each time the Tinker takes a turn, it hits the nearest enemy for 2–3.; *Barricade*: Places a barricade (HP 10) on any free spot of your side.; *Net Trap*: HP 4. A unit that hits it has +3 Delay on its next skill.  
Keystones: *Reinforced*: The Engineer's constructs and traps have double HP.; *Self-Repair*: The Engineer's constructs regain 2 HP each time the Engineer takes a turn.; *Rapid Fire*: Bolt Turret fires twice each time.

**Hexer**  
Actives: *Curse of Betrayal*: For 20 time units, a unit's healing goes to your units instead.; *Curse of Weight*: A unit cannot Move or become airborne for 20 time units.; *Withering Hex*: A unit takes 3 every 10 time units for 40 time units.  
Keystones: *Witch's Patience*: The Sorcerer's curses last until removed, but it can keep only one curse at a time.; *Lingering Malice*: When an enemy removes one of the Sorcerer's curses or hexes, the remover is Silenced for 10 time units.; *Spreading Curse*: When a cursed unit is KO, its curse jumps to the nearest enemy.

**Keeper**  
Actives: *Ward*: A unit within distance 2 gains a Ward of 4 until the Guardian's next turn.; *Shield Dome*: Every ally gains a Ward of 2 until the Guardian's next turn.; *Ward Burst*: A unit's Ward breaks and deals its remaining value as damage to every enemy in melee reach of that unit.  
Keystones: *Thorned Ward*: When an attack hits a Ward from the Keeper, the attacker takes 3.; *Twin Wards*: Ward and Overcharged Ward also give the same Ward to one adjacent ally.; *Steadfast Ward*: While a unit has a Ward from the Keeper, it cannot be Stunned or moved by enemies.

**Lifebinder**  
Actives: *Burnout Heal*: Heal every unit around the Healer for 6–8; the Healer is KO (§10.4).; *Life for Life*: The Healer gives up half its current HP to revive a KO unit with that much HP.; *Bound Resurrect*: Revive a KO unit at 25% of its max HP (rounded down, minimum 1); it and the Healer share one HP pool (game_system §10.4).  
Keystones: *Martyr*: When the Lifebinder is KO, every ally heals 6–8.; *Phoenix*: Life for Life costs only a quarter of the Lifebinder's current HP.; *Lifeblood*: When an ally is KO, the Lifebinder heals 5.

**Mender**  
Actives: *Mend*: Heal one unit for 8–10.; *Rain of Mending*: Pick a row: every unit in it is healed for 3–4, and the row behind for half (Downpour folded in).; *Searing Light*: Hit for 7–8. Undead take double.  
Keystones: *Overflowing Care*: Healing beyond the target's max HP goes to its adjacent ally with the lowest HP.; *Regrowth*: A unit the Mender heals also regains 2 HP every 10 time units for 20 time units.; *Triage*: Mend heals double on units below 25% HP.

**Necromancer**  
Actives: *Raise Undead*: Turns one KO'd body from either side into an undead of yours (HP 12, Power 4); also raises one more random body (Mass Grave folded in).; *Corpse Explosion*: A body or an undead of the Necromancer explodes: 6 to every unit next to it, and they are Poisoned for 20 time units (Plague Corpse folded in).; *Desecrate*: A KO'd body cannot be revived for 40 time units.  
Keystones: *Lich Form*: The Summoner counts as undead (healing hurts it), but every undead of the Summoner has +2 Power.; *Bone Armor*: For each undead it has, the Necromancer has a Ward of 2 that renews every turn.; *Death's Bounty*: When a unit is KO next to the Necromancer, the Necromancer heals 5.

**Pyromancer**  
Actives: *Fireball*: Hit for 8–9 and set fire to the target's spot.; *Kindle*: Set fire to a unit's spot and the spots next to it (Scorch folded in).; *Row of Fire*: Set fire to a whole row for 20 time units.  
Keystones: *Wildfire*: The Pyromancer's burning ground lasts twice as long.; *Inferno*: Burning ground from the Pyromancer deals double damage.; *Ashwalker*: The Pyromancer takes no damage from burning ground and has +1 Speed while standing on it.

**Ritualist**  
Actives: *Rain Dance*: Water on every spot of one row; lasts 20 time units longer than usual and allies in it regain 4 HP (Long Rain and Healing Rain folded in).; *Hoarfrost*: Ice on a unit's spot (§12.2).; *Rite of Mists*: Ranged hits from units standing on water deal half damage for 20 time units.  
Keystones: *Deluge*: Water from the Ritualist lasts until the end of battle.; *Tidecaller*: Allies standing on water have 1 less Delay on every skill.; *Frostbinder*: A unit standing on the Ritualist's ice is Immobilized.

**Saboteur**  
Actives: *Flash Powder*: Every unit in melee reach is Blind for 20 time units.; *Cut the Strings*: Remove one auto event the other side has on the timeline.; *Jam*: A unit's next skill has +5 Delay.  
Keystones: *Wrecker*: The Saboteur deals double damage to constructs, totems, traps and barricades.; *Chaos Agent*: Every skill of the Saboteur that hits an enemy also pushes its next turn back 2 time units.; *Disarm Trap*: The Rogue sets off enemy traps without taking their effect.

**Skyborne**  
Actives: *Take Off*: For 40 time units the unit is airborne (High Altitude folded in).; *Gust*: Wind. Pushes a unit one row back.; *Dive*: Only while airborne: hit any visible unit for 9–11, then land.  
Keystones: *Sky Lord*: The unit is always airborne, but has 30% less HP (rounded down).; *Eagle Eye*: While airborne, the unit sees stealthed units and reveals them.; *Lift*: Take Off also makes one ally next to the unit airborne for the same time.

**Spiritwalker**  
Actives: *Puppet Soul*: Possession. A unit hits a unit next to it at once with its basic attack.; *Life Swap*: Soul swap. Two units swap their current HP (each capped at its max).; *Forced Step*: Possession. A unit moves to a free spot of your choice on its own side.  
Keystones: *Soul Anchor*: Enemies cannot possess, move or soul-swap the Spiritwalker.; *Haunting*: When the Spiritwalker is KO, the unit that KO'd it is Confused for 20 time units.; *Puppeteer*: Puppet Soul hits deal double damage.

**Stormcaller**  
Actives: *Lightning Bolt*: Hit for 9–10. Cannot be intercepted.; *Chain Lightning*: Hits a unit for 1–2 and Stuns it for 5 time units, then jumps to an adjacent unit, three more times.; *Tempest*: Push every unit in one row one row back (bumps apply). No extra hit.  
Keystones: *Conductor*: Stuns from the Stormcaller last 5 time units longer.; *Static Field*: A unit that hits the Stormcaller in melee is Stunned for 5 time units.; *Eye of the Storm*: The Stormcaller cannot be pushed, pulled or Stunned.

**Summoner**  
Actives: *Clay Golem*: Summons a golem (HP 25, Speed 3) that makes melee hits for 5–6. The plain, sturdy golem.; *Rune Golem*: Summons a rune golem (HP 18, Speed 4) that makes melee hits for 3–4; each hit Silences the target for 10 time units.; *Animate Blades*: Summons two sets of flying blades (HP 6, Speed 7) that make ranged hits for 2–3. Lasts 30 time units (Blade Storm folded in).  
Keystones: *Golem Heart*: The Summoner can keep only one golem, but it has double HP.; *Portal*: The Summoner's summons appear on a random free spot of your side instead of being placed.; *Quickening*: The Golemancer's summons take their first turn at once.

**Tactician**  
Actives: *Swap Turns*: Own side only. Two allies swap their places on the timeline.; *Press the Advantage*: A unit's next skill costs no Momentum.; *Coordinated Strike*: Own side only. Two allies each make their basic attack on one enemy right now (each must be able to reach it).  
Keystones: *All In*: The Leader's Momentum costs are halved (rounded up), but Momentum gained from its hits is 0.; *Inspiring Presence*: When an ally next to the Leader KOs an enemy, Momentum moves 1 more toward your side.; *Initiative*: At battle start, every ally's first turn is pulled forward 5 time units.

**Thief**  
Actives: *Melt Away*: The Rogue gains stealth anywhere, even in the Front row.; *Lift a Skill*: Use a copy of the last active skill the target used, with the Rogue's own Power.; *Snatch*: Hit for 5–6 and steal one helpful status from the target.  
Keystones: *Escape Artist*: The first time the Thief drops below half HP, it gains stealth.; *Master Thief*: Snatch steals every helpful status from the target, not just one.; *Shadow Purse*: Each time the Thief steals a status, Momentum moves 1 toward your side.

**Totemist**  
Actives: *Healing Totem*: Places a totem (HP 8): allies in its row regain 2 HP every 10 time units.; *Spirit Link Totem*: Places a totem (HP 10): damage to allies in its row is split evenly between them.; *Totem of Return*: Places a totem (HP 6): the first ally in its row that would be KO keeps 1 HP instead, and the totem breaks.  
Keystones: *Great Totem*: The Primalist keeps only 1 totem, but it has triple HP and also affects the rows next to it.; *Sturdy Totems*: Totems of the Totemist have double HP.; *Spirit Shield*: When a totem of the Totemist breaks, every ally in its row gains a Ward of 4.

**Tracker**  
Actives: *Target*: Mark any visible unit (one Mark from this Hunter at a time; marking another moves it).; *Read Tracks*: Pick a row. Every stealthed unit in it is revealed.; *Hunter's Shot*: Hit a Marked unit for 9–11.  
Keystones: *Sniff the Air*: At battle start, the enemy with the highest Power becomes Marked.; *Scent of Blood*: When an enemy's HP falls below 20%, it becomes Marked.; *Relentless*: The Tracker's Mark lasts until its target is KO and cannot be removed.

**Transmuter**  
Actives: *Transmute*: A unit's Power and Speed swap values for 20 time units.; *Glass Body*: The next hit on a unit deals 50% more damage.; *Petrify*: A unit turns to stone: it is Stunned for 10 time units.  
Keystones: *Lasting Change*: Transmute lasts until the end of battle, but only one unit can be transmuted at a time.; *Brittle Glass*: Glass Body makes the next hit deal double instead of 50% more.; *Mirror Form*: Transmute can instead swap Power and Speed between two units.

## 1f. Creatures (accepted 2026-11-11)

All 319 creatures have two classes, two keystones and a unique attack in [classes-creatures.md](classes-creatures.md). Every one of the 84 keystones is used at least once. Round 3 note changes filled in by Claude: Jam +5 Delay; Life Swap swaps current HP (capped at max); Frost Nova and Chain Lightning deal 1–2.

## 1g. Delivery, range and action bar (round 7, answered 2026-11-11)

| Skill | Class | Delivery · Range |
|---|---|---|
| Feint | Assassin | Melee · D1 |
| Backstab | Assassin | Melee · D1 |
| Hunter's Shot | Tracker | Projectile · D3 |
| Leap to Defend | Bulwark | Direct · D3, own side |
| Shield Bash | Bulwark | Melee · D1 |
| Ward Burst | Keeper | Direct · D1 |
| Searing Light | Mender | Direct · D3 |
| Withering Hex | Hexer | Direct · D3 |
| Fireball | Pyromancer | Projectile · D3 |
| Lightning Bolt | Stormcaller | Direct · D3 |
| Ice Lance | Cryomancer | Projectile · D3 |
| Rampage | Berserker | Melee · D1 |
| Coordinated Strike | Tactician | Direct · D3 (any visible enemy) |
| Song of Haste | Bard | No target (the Bard's row) |
| War Drums | Bard | No target (allies next to the Bard) |
| Lullaby | Bard | Direct · D3 |
| Snatch | Thief | Melee · D1 |
| Dive | Skyborne | Melee · any visible enemy |

- **Coordinated Strike:** two random allies among all allies whose attack can reach the enemy make their attack (equal chances); if only one can, only it attacks; if none can, nothing happens.
- **Action bar:** the attack first, then the first class's 3 skills, then the second class's 3 skills, with a small gap between the groups; the class name shows on hover.
- **Keystones** show only in the inspection panel, not on the action bar.

## 1h. Loose ends (round 8, answered 2026-11-11)

- **Twin Totems** is replaced by **Sturdy Totems**: totems of the Totemist have double HP. There is still no totem limit.
- **Songs:** Song of Haste, War Drums and Lullaby count as songs for Grand Chorus. Lullaby stays instant, so Encore and Crescendo affect only Song of Haste and War Drums.
- **Undead, Puppet Soul and Coordinated Strike** use the creature's own unique attack.
- **Dropped content:** Clockwork Golem, Bell Golem, Animate Mirror, Flame Turret, Cage and the Hunter decoy leave the game; their art files are kept for later.
- **Achievements #29 Keystones and #30 Tinkerer are removed.**
- **Campaign:** enemies have no builds. Formations by role: Bulwark, Keeper and creatures with a melee D1 attack in Front, D2 attacks in the Middle, projectile and direct attacks and healers in the Rear.
- **Team editor** filters by class only.
- **Skill icons:** one icon per skill (84 actives) plus one per unique attack.
- **Hunt Down** also triggers when the Marked enemy moves by itself.

## 2. Open questions

- **Doc rewrite:** class_system.md, skills.md, game_system.md (§4.1 trees, §4.4 attacks, §19 Builds), creature-classes.md, ui_spec.md (tree editor, action bar), implementation plan and code (default builds, tree editor). Not started: the owner confirms first (2026-11-11).
