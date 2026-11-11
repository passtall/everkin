# Spec: branches become classes

**Status:** DRAFT 2026-11-11, being decided in quiz rounds (rounds 1 to 4b applied; Flying's third keystone open; round 5 drafted). Separate from [spec.md](spec.md) (battle UI work order). Until this spec is final, the current rules in docs/ stay valid; when it is final, its decisions are written into class_system.md, skills.md, game_system.md, creature-classes.md and ui_spec.md, and this file becomes the work order for the code.

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

Skill texts for new and reworded skills: project file everkin/class-branches-round3.md. Round 4 (db doc answers/round4, list everkin/class-branches-round4.md) filled the classes that were short. Round 4b (db doc answers/round4b) let the owner pick the missing keystones himself; Flying's third keystone is open (round 4c, db doc answers/round4c). Keystone texts: project files everkin/class-branches-round4.md and class-branches/round4c.json.

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
| Flying | Take Off, Gust, Dive | Sky Lord, Eagle Eye, (third pending) |

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

## 2. Open questions (later rounds)

- **Names.** Some branch names read as skill groups, not classes (Mobility, Assault, Enchantment, Pyromancy). Rename where needed; a class must still be visible in the art (class_system §4).
- **Creature assignment.** 319 creatures need exactly two new classes each; 56 have only one profession today. Does a creature's two classes have to come from its old professions?
- **Keystones per creature.** Which creature gets which keystone, how many at most (0 or 1?), and whether classes without a keystone get one.
- **Damage skills.** Which classes get one, and how strong (today a damage skill must "count": big, with a condition or a cost).
- **Thin classes.** Classes with 3 or 4 skills (Trapper, Footwork, Tactician, Conjurer and others) may need more skills; with everything unlocked, class size sets how many buttons a creature has.
- **Attack Type.** Stays as it is (one per creature, its shared attack)? Confirm.
- **Action bar size.** Two classes with all skills plus the attack can mean 12 or more buttons (ui_spec action bar).
- **Balance.** Cost (game_system §5.1, cost.py) and the AI harness builds depend on classes and learned skills; `content/default_builds.json` and the tree editor go away.
- **Marked.** Marks come from Tracker, Champion, Assassin and Skywatch; check that each Mark payoff class can still get Marks.
