# Everkin — Creatures

**Status:** Draft roster proposed by Claude on 2026-10-05 from the existing art, at the owner's request. Every creature, number, class and attack below is Draft until the owner reviews it; the rules it follows are adopted in [game_system.md](game_system.md) §3.7.
**Global rules:** Stats (§5.1), damage (§5.2), anchors (§5.5), timeline (§6.1), statuses (§10.5) and distance (§7.4) live in game_system.md. This document holds only the tiers, the attack library and the roster.

## Size tiers

Each creature belongs to one tier and tweaks the tier's base block slightly. Stats are HP, Speed, Power and Defense (game_system.md §5.1).

| Tier | Base block | HP band | Speed band | Power band | Defense |
|---|---|---|---|---|---|
| Critter | HP 20, Speed 8, Power 3, Defense 0% | 20–25 | 7–9 | 2–4 | 0% default |
| Medium | HP 40, Speed 5, Power 5, Defense 0% | 30–45 | 4–7 | 4–6 | 0% default |
| Large | HP 60, Speed 3, Power 6, Defense 0% | 50–70 | 1–4 | 5–7 | 0% default |

- HP moves in steps of 5. HP 75–80 is left free for bigger creatures later.
- Natural Defense is rare (0–40% in steps of 10) and only goes to creatures whose body is visibly armored: shells, plates, quills, crystal, a shield.
- A creature that breaks its tier's Speed band says why in the roster notes.
- Trade-offs inside a tier: more HP or Defense usually comes with less Speed or Power.

## Type-specific attack library

Each creature has exactly one of these attacks (game_system.md §4.4). The numbers are fixed; a creature cannot tweak them. Damage at Power 3 / 5 / 7 is shown for comparison, rounded down with a minimum of 1 (§5.2).

| Attack | Delay | Tags | Mechanic | Reach | Coefficient | Damage at Power 3 / 5 / 7 |
|---|---|---|---|---|---|---|
| **Claw** | 2 | `attack` `physical` `melee` `single` | A quick swipe. The fastest creature attack, for tempo and Momentum. | Range 2 | 0.5–0.7 | 1–2 / 2–3 / 3–4 |
| **Bite** | 4 | `attack` `physical` `melee` `status` `single` | Bites one unit. 25% chance to apply Bleeding (§10.5). | Range 2 | 0.9–1.2 | 2–3 / 4–6 / 6–8 |
| **Crush** | 5 | `attack` `physical` `melee` `single` | A heavy blow with horns, fists, tusks or a weapon. The strongest single hit. | Range 2 | 1.2–1.6 | 3–4 / 6–8 / 8–11 |
| **Stomp** | 6 | `attack` `physical` `melee` `circular` | Hits the target and the units adjacent to it (§8.4). One shared roll for all targets. | Range 2 | 0.6–0.8 | 1–2 / 3–4 / 4–5 |
| **Kick** | 3 | `attack` `physical` `melee` `move` `single` | Hits one unit, then pushes it one row back (forced repositioning, §9.5). The push happens after the damage and only if the target survives. | Range 2 | 0.5–0.7 | 1–2 / 2–3 / 3–4 |
| **Thrust** | 4 | `attack` `physical` `melee` `single` | A spear or beak thrust that reaches one row further than other melee attacks. | Range 3 | 0.7–0.9 | 2 / 3–4 / 4–6 |
| **Sting** | 3 | `attack` `physical` `melee` `status` `single` | A light sting. 50% chance to apply Poison (§10.5). | Range 2 | 0.3–0.5 | 1 / 1–2 / 2–3 |
| **Spit** | 4 | `attack` `physical` `projectile` `single` | Spits at one visible unit. Respects interception. | Any visible unit | 0.5–0.7 | 1–2 / 2–3 / 3–4 |
| **Glare** | 4 | `attack` `magical` `projectile` `single` | A magical bolt at one visible unit. Respects interception. Silenced stops it; Shield Wall does not. | Any visible unit | 0.6–0.8 | 1–2 / 3–4 / 4–5 |

Every attack respects interception (§8.2) and shifts Momentum normally (§6.7). Check against the anchor: a Large creature's Crush at Power 7 deals 8–11, so a 20 HP critter survives one hit and falls to two or three.

## Roster

53 creatures, one per existing art pair in `images/kin/` (`<art>_f.png` front, `<art>_r.png` rear). Art numbers 13 and above 54 do not exist; 13 was deleted. IDs are stable text IDs (§3.7); the art files may later be renamed to match.

| Art | ID | Name | Tier | HP | Speed | Power | Defense | Classes | Attack | Notes |
|---:|---|---|---|---:|---:|---:|---:|---|---|---|
| 1 | `spikeback` | Spikeback | Large | 55 | 4 | 6 | 0% | Warrior, Feral | Crush |  |
| 2 | `gazer_jelly` | Gazer Jelly | Medium | 30 | 6 | 6 | 0% | Elementalist | Glare |  |
| 3 | `prism_snail` | Prism Snail | Medium | 35 | 4 | 5 | 0% | Healer, Elementalist | Glare |  |
| 4 | `bone_idol` | Bone Idol | Medium | 30 | 5 | 5 | 0% | Leader, Healer | Glare |  |
| 5 | `boulder_toad` | Boulder Toad | Large | 60 | 2 | 7 | 0% | Guardian, Warrior | Crush |  |
| 6 | `thornfang` | Thornfang | Medium | 40 | 7 | 5 | 0% | Feral, Hunter | Claw |  |
| 7 | `bloomspear` | Bloomspear | Medium | 40 | 5 | 5 | 0% | Healer, Hunter, Guardian | Thrust |  |
| 8 | `cinderhorn` | Cinderhorn | Large | 65 | 3 | 7 | 0% | Warrior, Leader | Crush |  |
| 9 | `reef_warden` | Reef Warden | Medium | 40 | 6 | 5 | 10% | Guardian, Hunter, Leader | Thrust | Defense from its shield. |
| 10 | `magma_hog` | Magma Hog | Large | 60 | 4 | 6 | 0% | Warrior, Elementalist | Crush |  |
| 11 | `hornshell` | Hornshell Tortoise | Large | 65 | 1 | 5 | 30% | Guardian | Crush | Defense from its shell. |
| 12 | `land_shark` | Land Shark | Medium | 45 | 6 | 6 | 0% | Feral, Warrior | Bite |  |
| 14 | `ember_snail` | Ember Snail | Medium | 40 | 3 | 5 | 20% | Elementalist, Guardian | Spit | Speed 3 is below the medium band: it is a snail. |
| 15 | `rhino_beetle` | Rhino Beetle | Medium | 35 | 5 | 5 | 20% | Guardian, Warrior | Crush | Defense from its carapace. |
| 16 | `fin_stalker` | Fin Stalker | Medium | 35 | 7 | 5 | 0% | Hunter, Feral | Bite | Kept distinct from the Land Shark: faster, lighter, a Hunter. |
| 17 | `shade_ray` | Shade Ray | Medium | 30 | 8 | 5 | 0% | Feral, Elementalist | Sting | Speed 8 is above the medium band: it glides. |
| 18 | `mossback` | Mossback | Medium | 45 | 4 | 4 | 20% | Guardian, Healer | Stomp | Defense from its plates. |
| 19 | `leafwisp` | Leafwisp | Critter | 20 | 9 | 3 | 0% | Healer | Kick |  |
| 20 | `emberbulb` | Emberbulb | Critter | 25 | 7 | 4 | 0% | Elementalist | Glare |  |
| 21 | `stone_turtle` | Stone Turtle | Large | 60 | 1 | 5 | 40% | Guardian | Stomp | HP anchor (section 5.5). Defense from its stone shell. |
| 22 | `plume_gazelle` | Plume Gazelle | Medium | 30 | 7 | 4 | 0% | Leader, Hunter | Kick |  |
| 23 | `sailback` | Sailback | Medium | 40 | 5 | 5 | 0% | Feral, Elementalist | Bite |  |
| 24 | `sickle_mantis` | Sickle Mantis | Medium | 35 | 6 | 6 | 0% | Feral, Warrior | Claw |  |
| 25 | `magma_crawler` | Magma Crawler | Large | 55 | 3 | 6 | 0% | Elementalist, Warrior | Stomp |  |
| 26 | `glade_stag` | Glade Stag | Medium | 40 | 6 | 5 | 0% | Healer, Leader | Kick |  |
| 27 | `gloom_scorpion` | Gloom Scorpion | Medium | 35 | 5 | 5 | 0% | Hunter, Feral | Sting |  |
| 28 | `ember_shard` | Ember Shard | Medium | 30 | 4 | 6 | 20% | Elementalist | Glare | Defense from its crystal body. |
| 29 | `bloom_pod` | Bloom Pod | Medium | 35 | 4 | 4 | 0% | Healer, Hunter | Spit |  |
| 30 | `woodpecker` | Giant Woodpecker | Medium | 30 | 7 | 5 | 0% | Hunter, Leader | Thrust |  |
| 31 | `bog_toad` | Bog Toad | Medium | 45 | 4 | 5 | 0% | Healer, Elementalist | Spit |  |
| 32 | `ash_moth` | Ash Moth | Critter | 20 | 9 | 3 | 0% | Elementalist, Feral | Sting |  |
| 33 | `giant_mole` | Giant Mole | Medium | 45 | 4 | 5 | 0% | Warrior, Guardian | Claw |  |
| 34 | `mannequin` | Mannequin | Medium | 40 | 5 | 5 | 0% | Warrior, Guardian, Leader | Kick | A construct with three classes, as a stand-in for a human-like generalist. |
| 35 | `fox` | Fox | Critter | 20 | 8 | 3 | 0% | Feral, Hunter | Bite | HP anchor: the critter at 20 (section 5.5). |
| 36 | `hare` | Hare | Critter | 20 | 9 | 2 | 0% | Feral, Leader | Kick |  |
| 37 | `otter` | Otter | Critter | 25 | 8 | 3 | 0% | Healer | Bite |  |
| 38 | `hedgehog` | Hedgehog | Critter | 25 | 7 | 3 | 20% | Guardian | Bite | Defense from its quills. |
| 39 | `dormouse` | Dormouse | Critter | 20 | 9 | 2 | 0% | Healer, Leader | Claw |  |
| 40 | `brown_bear` | Brown Bear | Large | 65 | 3 | 7 | 0% | Warrior, Guardian | Crush |  |
| 41 | `moose` | Moose | Large | 60 | 3 | 6 | 0% | Guardian, Leader | Kick |  |
| 42 | `bison` | Bison | Large | 70 | 2 | 6 | 0% | Warrior, Guardian | Stomp |  |
| 43 | `rhino` | Rhino | Large | 65 | 2 | 7 | 20% | Warrior | Crush | Defense from its hide. |
| 44 | `elephant` | Elephant | Large | 70 | 1 | 7 | 0% | Guardian, Leader | Stomp |  |
| 45 | `wolf` | Wolf | Medium | 35 | 7 | 5 | 0% | Feral, Leader | Bite |  |
| 46 | `lynx` | Lynx | Medium | 30 | 7 | 5 | 0% | Feral, Hunter | Claw |  |
| 47 | `boar` | Boar | Medium | 40 | 5 | 5 | 0% | Warrior | Crush |  |
| 48 | `badger` | Badger | Medium | 40 | 5 | 4 | 0% | Guardian, Feral | Claw |  |
| 49 | `wolverine` | Wolverine | Medium | 35 | 6 | 6 | 0% | Feral, Warrior | Bite |  |
| 50 | `tiger` | Tiger | Medium | 45 | 6 | 6 | 0% | Feral, Warrior | Claw |  |
| 51 | `zebra` | Zebra | Medium | 40 | 6 | 4 | 0% | Leader, Hunter | Kick |  |
| 52 | `kangaroo` | Kangaroo | Medium | 35 | 6 | 5 | 0% | Warrior, Leader | Kick |  |
| 53 | `anteater` | Giant Anteater | Medium | 40 | 4 | 4 | 0% | Feral, Hunter | Claw |  |
| 54 | `raccoon` | Raccoon | Critter | 25 | 7 | 3 | 0% | Hunter, Leader | Claw |  |

### Appearance

What each fantasy creature's art shows, so the names can be checked against the images. The animals (35–54) are named after what they are.

- **Spikeback** (1): Green horned lizard brute with stone spikes and claws.
- **Gazer Jelly** (2): Floating jellyfish with one violet eye and crystal tentacles.
- **Prism Snail** (3): Snail with a crystal shell and crystal-tipped antennae.
- **Bone Idol** (4): Floating hooked bone totem holding an amber gem.
- **Boulder Toad** (5): Upright toad brute with rock shoulders and boulder fists.
- **Thornfang** (6): Lean upright wolf-beast covered in bone thorns.
- **Bloomspear** (7): Walking sapling with a glowing flower head and a spear.
- **Cinderhorn** (8): Ram-horned dark brute with an ember heart and an axe.
- **Reef Warden** (9): Blue finned lizard warrior with spear and round shield.
- **Magma Hog** (10): Tusked lava-cracked boar beast with a spiked tail.
- **Hornshell Tortoise** (11): Horned tortoise with a heavy domed shell.
- **Land Shark** (12): Stocky blue shark walking on four legs.
- **Ember Snail** (14): Rock-shelled slug with a glowing ember belly.
- **Rhino Beetle** (15): Armored olive beetle with a long horn.
- **Fin Stalker** (16): Sleek blue shark on long legs, lighter than the Land Shark.
- **Shade Ray** (17): Gliding violet manta wraith with a barbed tail.
- **Mossback** (18): Small mossy plated dinosaur.
- **Leafwisp** (19): Tiny leaf-tailed forest sprite on thin legs.
- **Emberbulb** (20): Round bulb creature with a flame crown and a lantern tail.
- **Stone Turtle** (21): Giant tortoise with a craggy stone shell.
- **Plume Gazelle** (22): Slender gazelle with a long feathered tail.
- **Sailback** (23): Teal sail-finned lizard.
- **Sickle Mantis** (24): Hooded upright insect with sickle arms.
- **Magma Crawler** (25): Squat lava-rock beast with a glowing maw.
- **Glade Stag** (26): Pale spirit stag with branching antlers.
- **Gloom Scorpion** (27): Violet scorpion-like crawler with glowing eyes.
- **Ember Shard** (28): Cracked lava crystal walking on stone legs.
- **Bloom Pod** (29): Walking seed pod with flower buds.
- **Giant Woodpecker** (30): Tall blue woodpecker with a red crest.
- **Bog Toad** (31): Heavy purple toad.
- **Ash Moth** (32): Black moth with large pale wings.
- **Giant Mole** (33): Upright giant mole with digging claws.
- **Mannequin** (34): Jointed wooden mannequin.

### Totals

- **Tiers:** 9 critters, 32 medium, 12 large.
- **Classes:** Guardian 16, Warrior 17, Feral 17, Leader 15, Hunter 13, Elementalist 11, Healer 10. Every creature has one or two classes, except Bloomspear, Reef Warden and Mannequin, which have three.
- **Attacks:** Crush 9, Claw 9, Bite 8, Kick 8, Glare 5, Stomp 5, Thrust 3, Spit 3, Sting 3.
- **Natural Defense:** 9 creatures; everyone else has 0%.

## TBD

- Owner review of every name, number, class and attack above.
- Balance after the first AI-vs-AI runs (game_system.md §2.4).
- Human recruits: none exist in the art yet. They follow §3.4 and §3.7 when added.
- Art framing and cleanup before card layout ([unit-art-compatibility.md](unit-art-compatibility.md)).
