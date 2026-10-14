# Everkin – Class System

**Status:** Finalized  
**Version:** 1.4

> Posted by Andreas on 2026-10-09 as the single source of truth for creature classes. It supersedes every earlier class and skill definition: the seven-class roster (Hunter, Elementalist, Healer, Warrior, Guardian, Feral, Leader), the skills in [classes_and_skills.md](classes_and_skills.md), the type-specific attack library and class assignments in [creatures.md](creatures.md), and the class rules in [game_system.md](game_system.md) §3.7 and §4. How skills, skill trees and basic attacks are built on top of this system is not decided yet (game_system.md §19).
>
> **v1.2 (2026-10-14):** Black Mage removed. Necromancy moved to Summoner, curses, hexes and life drain moved to Sorcerer, and elemental magic left Sorcerer for the new Elementalist profession. Still 16 Professions. Decided in the class rework quiz.
>
> **v1.3 (2026-10-14):** Merges decided in the class merge and names quizzes. Professions: Bodyguard merged into Guardian; Assassin and Trickster merged into Rogue; Alchemist and Engineer merged into Tinker; Bard, Leader and Primalist sharpened; Paladin moved to Healer. Attack Types: Thrown merged into Ranged; Unarmed and Feral merged into Brawl; Lashing renamed Whip. Now 9 Attack Types and 13 Professions.
>
> **v1.4 (2026-10-14):** Skill-fit rules added (section 4).

## 1. Core Concept

Every Everkin creature has two independent class components:

- **1 Attack Type (required):** Defines the creature's primary attack method, including its basic attack behavior and animation style.
- **1–2 Professions (required):** Define the creature's special abilities, tactical role, strengths, and identity.

Attack Types and Professions can be combined freely.

The system is designed for creatures of any shape, including humanoids, animals, floating entities, amorphous creatures, and completely abstract beings.

Classes describe broad archetypes rather than individual weapons or specialized fantasy RPG roles.

---

## 2. Attack Types (9)

| Attack Type | Description |
|---|---|
| **Brawl** | The body itself: fists, kicks, martial arts, claws, bites, horns, beaks and fangs |
| **Bladed** | Swords, daggers, axes, scythes, and bladed weapons |
| **Polearm** | Spears, lances, halberds, and other long-reach weapons |
| **Ranged** | Bows, crossbows, slings, and other projectile weapons, and directly thrown objects such as knives, rocks, spears and explosives |
| **Magic** | Spells, magical bolts, and supernatural projectiles |
| **Blunt** | Hammers, clubs, maces, and other blunt weapons |
| **Breath** | Fire breath, frost breath, poison spit, acid sprays, and similar emissions |
| **Whip** | Whips, tentacles, tails, vines, and flexible appendages |
| **Aura** | Continuous or pulsing damage fields surrounding the creature |

### Attack Type Rules

- Every creature has exactly one primary Attack Type.
- Attack Types describe how the creature normally attacks, not every ability it can use.
- Individual weapons, body parts, and animations are variants within their Attack Type.
- Aura describes area-based damage originating around the creature.
- Magic describes the primary attack method and does not require the Sorcerer profession.
- Special abilities may employ other attack methods without changing the primary Attack Type.

---

## 3. Professions (13)

| Profession | Description and included archetypes |
|---|---|
| **Hunter** | Tracking, hunting, trapping, survival. Includes Ranger, Tracker, Trapper |
| **Guardian** | Defensive abilities, endurance, area protection, battlefield control, and protecting allies: intercepting attacks and taking damage for others. Includes Defender, Tank, Sentinel, Bodyguard |
| **Healer** | Restoring health, cleansing conditions, regeneration, holy protection. Includes Medic, Cleric, Paladin |
| **Sorcerer** | All non-elemental spells: arcane magic, enchantment, magical enhancement, and dark magic such as curses, hexes and life drain. Includes Enchanter, Warlock, Hexer |
| **Elementalist** | Fire, frost, water, storm and earth magic; earth includes stone and crystal. Includes elemental weapons and armor |
| **Warrior** | Offensive martial abilities, endurance, combat mastery. Includes Berserker, Duelist, Knight, Monk |
| **Bard** | Buffing allies and weakening enemies through music, sound, dance and performance. Includes Minstrel, Dancer |
| **Leader** | Commanding the flow of battle: the shared timeline and Momentum, who acts when, tactical coordination. Includes Commander, Tactician |
| **Rogue** | Stealth, precision, critical attacks, executing vulnerable targets, deception, illusions, misdirection, sabotage and theft. Includes Assassin, Ninja, Trickster, Illusionist, Thief |
| **Tinker** | Making things: potions, chemicals, poisons, mixtures, machinery, gadgets, constructs, mechanical traps. Includes Alchemist, Herbalist, Poisoner, Engineer, Artificer |
| **Summoner** | Summoning and commanding additional creatures, spirits, entities, or constructs, including necromancy: raising the dead and bone constructs. Includes Beastmaster, Necromancer |
| **Primalist** | Primal nature and the soul: plants, roots, beasts, shapeshifting, totems, nature spirits, rituals and life force. Spirits of the dead belong to Summoner. Includes Druid, Shaman |
| **Flying** | Flight, aerial mobility, evasive maneuvers, dive attacks, and airborne combat advantages |

### Profession Rules

- Every creature receives one or two Professions.
- Professions describe special abilities and tactical identity, not the creature's basic weapon or attack animation.
- Professions are intentionally broad. Narrower fantasy archetypes are included rather than becoming independent classes.
- Two Professions combine their capabilities and can create unusual playstyles.
- Professions are not restricted by Attack Type or physical appearance.
- Flying describes aerial capabilities and movement rather than a primary attack method.

---

## 4. Skill Fit (adopted 2026-10-14)

Andreas's goal: no creature gets access to skills that clearly do not fit its design. Decided in the skill-fit quiz:

- **No subclasses and no tags.** There are no element tags, no summon kinds, no assigned branches and no per-creature exclusion lists. Every Elementalist can learn skills of every element.
- **Branches are organizational only.** A profession's skill tree may be grouped into branches (for example by the archetypes listed in section 3), but a creature can learn from all branches of its professions. Only skill points limit what it learns.
- **No body traits.** Skills are not gated by hands, wings, voice or similar traits.
- **Fit comes from care, not rules.** Avoid only what clearly clashes with a creature's art when assigning professions and designing skills. Everkin is a card game with light animation, so generous fits are fine: a living drawbridge can have a charge attack.
- **A profession must be visible in the art (Andreas, 2026-10-14).** Its cue has to be shown, not implied by color or material. Sorcerers and Elementalists need a casting tool or clear spellcasting: a wand, staff, orb, tome, runes or a spell in hand. A fire-colored body alone is not an Elementalist; it suggests a fire basic attack (Breath, Aura or Magic with that element) instead.
