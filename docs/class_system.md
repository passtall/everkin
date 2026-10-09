# Everkin – Class System

**Status:** Finalized  
**Version:** 1.1

> Posted by Andreas on 2026-10-09 as the single source of truth for creature classes. It supersedes every earlier class and skill definition: the seven-class roster (Hunter, Elementalist, Healer, Warrior, Guardian, Feral, Leader), the skills in [classes_and_skills.md](classes_and_skills.md), the type-specific attack library and class assignments in [creatures.md](creatures.md), and the class rules in [game_system.md](game_system.md) §3.7 and §4. How skills, skill trees and basic attacks are built on top of this system is not decided yet (game_system.md §19).

## 1. Core Concept

Every Everkin creature has two independent class components:

- **1 Attack Type (required):** Defines the creature's primary attack method, including its basic attack behavior and animation style.
- **1–2 Professions (required):** Define the creature's special abilities, tactical role, strengths, and identity.

Attack Types and Professions can be combined freely.

The system is designed for creatures of any shape, including humanoids, animals, floating entities, amorphous creatures, and completely abstract beings.

Classes describe broad archetypes rather than individual weapons or specialized fantasy RPG roles.

---

## 2. Attack Types (11)

| Attack Type | Description |
|---|---|
| **Unarmed** | Fists, kicks, martial arts, and other unarmed physical strikes |
| **Bladed** | Swords, daggers, axes, scythes, and bladed weapons |
| **Polearm** | Spears, lances, halberds, and other long-reach weapons |
| **Ranged** | Bows, crossbows, slings, and projectile-launching weapons |
| **Feral** | Natural weapons such as claws, bites, horns, beaks, and fangs |
| **Magic** | Spells, magical bolts, and supernatural projectiles |
| **Blunt** | Hammers, clubs, maces, and other blunt weapons |
| **Breath** | Fire breath, frost breath, poison spit, acid sprays, and similar emissions |
| **Thrown** | Directly thrown objects such as knives, rocks, spears, and explosives |
| **Lashing** | Whips, tentacles, tails, vines, and flexible appendages |
| **Aura** | Continuous or pulsing damage fields surrounding the creature |

### Attack Type Rules

- Every creature has exactly one primary Attack Type.
- Attack Types describe how the creature normally attacks, not every ability it can use.
- Individual weapons, body parts, and animations are variants within their Attack Type.
- Aura describes area-based damage originating around the creature.
- Magic describes the primary attack method and does not require the Sorcerer profession.
- Special abilities may employ other attack methods without changing the primary Attack Type.

---

## 3. Professions (16)

| Profession | Description and included archetypes |
|---|---|
| **Hunter** | Tracking, hunting, trapping, survival. Includes Ranger, Tracker, Trapper |
| **Guardian** | Defensive abilities, endurance, area protection, battlefield control. Includes Defender, Tank, Sentinel |
| **Alchemist** | Potions, chemicals, poisons, transformations, mixtures. Includes Herbalist, Poisoner |
| **Healer** | Restoring health, cleansing conditions, regeneration. Includes Medic, Cleric |
| **Sorcerer** | Arcane and elemental magic, magical enhancement. Includes Elementalist, Enchanter |
| **Black Mage** | Dark magic, curses, necromancy, life drain. Includes Necromancer, Warlock, Hexer |
| **Warrior** | Offensive martial abilities, endurance, combat mastery. Includes Berserker, Duelist, Knight, Monk |
| **Bard** | Inspiration, music, performance, morale and team enhancement. Includes Minstrel, Dancer |
| **Assassin** | Stealth, precision, critical attacks, executing vulnerable targets. Includes Rogue, Ninja |
| **Bodyguard** | Protecting specific allies, intercepting attacks, taking damage for others |
| **Leader** | Commanding allies, tactical coordination, strengthening group performance. Includes Commander, Tactician |
| **Trickster** | Deception, illusions, misdirection, sabotage and theft. Includes Illusionist, Thief |
| **Engineer** | Machinery, gadgets, constructs, mechanical traps, technological equipment. Includes Artificer |
| **Summoner** | Summoning and commanding additional creatures, spirits, entities, or constructs. Includes Beastmaster |
| **Primalist** | Nature magic, spirits, sacred powers, rituals, protection and life-force manipulation. Includes Druid, Shaman, Paladin |
| **Flying** | Flight, aerial mobility, evasive maneuvers, dive attacks, and airborne combat advantages |

### Profession Rules

- Every creature receives one or two Professions.
- Professions describe special abilities and tactical identity, not the creature's basic weapon or attack animation.
- Professions are intentionally broad. Narrower fantasy archetypes are included rather than becoming independent classes.
- Two Professions combine their capabilities and can create unusual playstyles.
- Professions are not restricted by Attack Type or physical appearance.
- Flying describes aerial capabilities and movement rather than a primary attack method.
