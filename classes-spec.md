# Spec: branches become classes

**Status:** DRAFT 2026-11-11, being decided in quiz rounds (rounds 1 to 6 applied; creature quiz next). Separate from [spec.md](spec.md) (battle UI work order). Until this spec is final, the current rules in docs/ stay valid; when it is final, its decisions are written into class_system.md, skills.md, game_system.md, creature-classes.md and ui_spec.md, and this file becomes the work order for the code.

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

## 2. Open questions (later rounds)

- **Creature assignment** (round 7): two classes, two keystones and the unique attack for each of the 319 creatures; Claude proposes, the owner reviews.
- **Skill details for new skills:** Delivery and range for the new and reworded skills, checked against game_system §7.4 and §8.1.
- **Action bar:** the attack plus 6 class skills = 7 buttons (ui_spec action bar); confirm the layout once the attacks are known.
- **Doc rewrite** once final: class_system.md, skills.md, game_system.md (§4.1 trees, §4.4 attacks, §19 Builds), creature-classes.md, ui_spec.md (tree editor), implementation plan and code (default builds, tree editor).
