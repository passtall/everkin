# Everkin – Class System

This document owns the classes, the keystone and unique attack rules, and how classes are assigned to creatures. Every skill and keystone with its numbers is in [skills.md](skills.md); each creature's classes, keystones and attack are in [creature-classes.md](creature-classes.md). Game-wide rules are in [game_system.md](game_system.md).

## 1. What a creature has

- **Two classes.** Every creature has exactly two different classes. Both may be close relatives (Mender and Cleric); there is no main class.
- **Six skills.** Each class has three active skills. A creature can always use all six actives of its two classes. There are no skill points, skill trees, builds or respecs.
- **Two keystones.** Each class has three keystones (powerful passives). A creature has one keystone from each of its classes, fixed in the roster: the player never picks them, and two creatures of the same class can have different keystones. Every keystone is used by at least one creature.
- **One unique attack.** Every creature has its own attack (section 4). There are no shared attacks.

A skill or keystone text that names its class ("the Bard", "the Engineer") means the creature that has it, and it applies to all of that creature's skills unless it names a skill.

## 2. The 28 classes

| Class | Role | Actives | Keystones |
|---|---|---|---|
| **Assassin** | Stealth and a hard strike from it. | Vanish, Feint, Backstab | Unseen, Vanishing Kill, Venom Blades |
| **Tracker** | Marks an enemy, finds stealthed units, shoots the marked. | Target, Read Tracks, Hunter's Shot | Sniff the Air, Scent of Blood, Relentless |
| **Bulwark** | Stands in front of allies and takes their hits. | Leap to Defend, Rescue, Shield Bash | Shield Wall, Hold Position, Unbreakable |
| **Keeper** | Wards that absorb damage. | Ward, Shield Dome, Ward Burst | Thorned Ward, Twin Wards, Steadfast Ward |
| **Mender** | Plain healing. | Mend, Rain of Mending, Searing Light | Overflowing Care, Regrowth, Triage |
| **Cleric** | Removes statuses, good and bad. | Absolve, Dispel, Turn Back | Sanctity, Purifying Aura, Judgment |
| **Lifebinder** | Revives, paid for with its own life. | Burnout Heal, Life for Life, Bound Resurrect | Martyr, Phoenix, Lifeblood |
| **Enchanter** | Enchants an ally's next attack or skill. | Echo Enchant, Phase Weapon, Piercing Enchant | Twin Enchant, Lingering Magic, Spell Ward |
| **Transmuter** | Changes what a unit is: stats, glass, stone. | Transmute, Glass Body, Petrify | Lasting Change, Brittle Glass, Mirror Form |
| **Hexer** | Curses and hexes. | Curse of Betrayal, Curse of Weight, Withering Hex | Witch's Patience, Lingering Malice, Spreading Curse |
| **Pyromancer** | Fire damage and burning ground. | Fireball, Kindle, Row of Fire | Wildfire, Inferno, Ashwalker |
| **Stormcaller** | Lightning, Stuns and wind. | Lightning Bolt, Chain Lightning, Tempest | Conductor, Static Field, Eye of the Storm |
| **Cryomancer** | Frost: Frozen and ice. | Ice Lance, Freeze, Frost Nova | Cold Heart, Brittle, Steam Burst |
| **Berserker** | Raw melee damage and pushing the enemy Front row. | Rampage, Sweeping Kick, Battle Roar | Reckless Swing, Last Stand, Unstoppable |
| **Champion** | Marks one enemy and duels it. | Challenge, Single Combat, Execution | Hunt Down, Duelist, Glory |
| **Tactician** | The timeline and Momentum. | Swap Turns, Press the Advantage, Coordinated Strike | All In, Inspiring Presence, Initiative |
| **Commander** | Moves your own units. | Advance, Switch Places, Rotation | Drill, Rally, Vanguard |
| **Bard** | Songs for allies, a lullaby for enemies. | Song of Haste, War Drums, Lullaby | Grand Chorus, Rhythm, Crescendo |
| **Thief** | Steals statuses and skills. | Melt Away, Lift a Skill, Snatch | Escape Artist, Master Thief, Shadow Purse |
| **Saboteur** | Blinds, delays and breaks plans. | Flash Powder, Cut the Strings, Jam | Wrecker, Chaos Agent, Disarm Trap |
| **Engineer** | Turrets, barricades and traps. | Bolt Turret, Barricade, Net Trap | Reinforced, Self-Repair, Rapid Fire |
| **Chemist** | Poison and fire on the ground, potions. | Poison Gas, Firebomb, Healing Draught | Showman, Potent Brew, Volatile |
| **Necromancer** | Raises and uses the dead. | Raise Undead, Corpse Explosion, Desecrate | Lich Form, Bone Armor, Death's Bounty |
| **Summoner** | Golems and animated blades. | Clay Golem, Rune Golem, Animate Blades | Golem Heart, Portal, Quickening |
| **Totemist** | Totems that protect a row. | Healing Totem, Spirit Link Totem, Totem of Return | Great Totem, Sturdy Totems, Spirit Shield |
| **Ritualist** | Water and ice on the ground. | Rain Dance, Hoarfrost, Rite of Mists | Deluge, Tidecaller, Frostbinder |
| **Spiritwalker** | Possession and soul swaps. | Puppet Soul, Life Swap, Forced Step | Soul Anchor, Haunting, Puppeteer |
| **Skyborne** | Flight, wind and dives. | Take Off, Gust, Dive | Sky Lord, Eagle Eye, Lift |

**Magical classes.** Every active of Mender, Cleric, Lifebinder, Enchanter, Transmuter, Hexer, Pyromancer, Stormcaller, Cryomancer, Necromancer, Summoner, Totemist, Ritualist and Spiritwalker is magical, and so are the Bard's three songs. Every other skill is physical. Silenced blocks magical skills (game_system §10.5).

## 3. Assigning classes

- **Visible in the art.** A class must be visible in the creature's art, shown and not implied by colour or material. Casters (Mender, Cleric, Lifebinder, Enchanter, Transmuter, Hexer, the three elemental classes, Necromancer, Summoner, Totemist, Ritualist, Spiritwalker) need a casting tool or clear spellcasting: a wand, staff, orb, tome, runes or a spell in hand. A fire-coloured body alone is not a Pyromancer; it suggests a fire attack instead. Skyborne needs wings or another clear way to fly.
- **Fit comes from care, not rules.** Avoid only what clearly clashes with the art. Everkin is a card game with light animation, so generous fits are fine: a living drawbridge can have a charge.
- **No gating.** There are no subclasses, element tags, body traits or per-creature exclusion lists.

## 4. Unique attacks

Every creature has one attack of its own, listed in [creature-classes.md](creature-classes.md). It is always the first button of the action bar.

- **Parts:** a name, a delivery (Melee, Projectile or Direct), damage, at most one small extra, Delay and Momentum, and an animation style.
- **Strength:** 3 to 6 damage at Power 5, Delay 3 or 4, no Momentum cost. An extra is paid for with less damage. Extras are small: a low chance of Bleeding, Poisoned, Stunned or Confused, a push or pull, splash on the units next to the target, life steal of 1, or anything similar that fits the art and is not too strong.
- **Always hostile:** an attack always hits an enemy; healing and support stay in class skills.
- **Breath and pulse attacks** (Direct) work in melee reach (game_system §8.1): a breath hits one enemy there and splashes the units next to it, a pulse hits every enemy there. A pulse has no single target: its extra ("the target…", "heals the attacker") applies to, and rolls for, each enemy it hits.
- **Magical attacks:** an attack with the bolt or pulse animation is magical; every other attack is physical.
- **Animation:** one of the shared styles lunge, swing, shot, throw, bolt, breath or pulse, with a colour.
- **Used by others:** raised undead, Puppet Soul and Coordinated Strike use the creature's own unique attack.
