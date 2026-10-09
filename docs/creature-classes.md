# Creature Classes

Attack type and professions for every creature that has art, following [class_system.md](class_system.md) v1.1 (assessed 2026-10-09 from each creature's art and, for the first 54, its earlier classes in [creatures.md](creatures.md)). Each creature has one attack type and one or two professions; the first profession is the main one.

The table also drives the art pipeline: `tools/art/clean_creatures.py` reads each row's **Source** in `images/kin/` and writes the cleaned copy to `art/creatures/` under the **Art file** name.

**Art file names:** `{unitName}_{attackType}_{profession1}_{profession2}.png`, or `{unitName}_{attackType}_{profession}.png` for one profession. The unit name comes first and starts with the creature's kind, so the files sort by creature type (all `human-…` together, all `snail-…` together). Everything is lowercase; underscores separate the parts, and hyphens join words inside a part (`black-mage`, `stag-glade`).

Unit names follow the schema below. Creatures with a creature ID in creatures.md keep their ID in the ID column; their unit name is the ID with hyphens and the kind in front where it is not already first (`glade_stag` → `stag-glade`, `hornshell` → `turtle-hornshell`, `magma_hog` → `boar-magma`). They never take a counter. Creatures without an ID have no stats yet.

## Unit name schema

`{kind}-{archetype}[-{signature}][-{n}]`, so that similar designs get similar names and sort next to each other.

| Part | What it says | Vocabulary |
|---|---|---|
| **kind** | What the creature is; always the first word | `human`, `elf`, `dwarf`, `gnome`, `halfling`, `orc`, `centaur`, `lizardfolk`, `catfolk`, or the animal or plant (`fox`, `snail`, `frog`, `turtle`, `jelly`, `beetle`, `mushroom` …) |
| **archetype** | The role the costume shows; left out for an animal without gear (`cobra`, `pillbug`) | `alchemist`, `archer`, `bard`, `barbarian`, `brawler`, `captain`, `cleric`, `dancer`, `druid`, `duelist`, `engineer`, `guard`, `healer`, `knight`, `mage`, `monk`, `necromancer`, `paladin`, `priest`, `ranger`, `rogue`, `scout`, `shaman`, `smith`, `spellblade`, `witch`, `wizard` |
| **signature** | Optional: the one prop or element that sets the design apart | `banner`, `bell`, `censer`, `crystal`, `dagger`, `feather`, `fire`, `frost`, `gear`, `hammer`, `harpoon`, `hook`, `lantern`, `leaf`, `moon`, `pistol`, `potion`, `rapier`, `shadow`, `shield`, `skull`, `sling`, `spear`, `stone`, `sun`, `totem`, `water` |
| **n** | Counter for near-identical designs | `2`, `3` … in the order the art was added; the first has none |

1. Name what the art shows, never a made-up title.
2. Pick each part from its list. A new word joins a list only when no existing word fits.
3. Look-alikes share kind, archetype and signature and differ only in the counter.
4. Look-alikes get the same attack type and professions unless the art clearly shows a different fighting style.

| Unit | ID | Name | Attack type | Professions | Art file | Source |
|---|---|---|---|---|---|---|
| `anteater` | `anteater` | Giant Anteater | Feral | Hunter, Assassin | `anteater_feral_hunter_assassin.png` | `53_f.png` |
| `armadillo-knight-2` | — | Armadillo Knight 2 | Feral | Guardian, Bodyguard | `armadillo-knight-2_feral_guardian_bodyguard.png` | `armadillo-knight-2.png` |
| `armadillo-knight` | — | Armadillo Knight | Blunt | Guardian, Bodyguard | `armadillo-knight_blunt_guardian_bodyguard.png` | `armadillo-knight.png` |
| `badger` | `badger` | Badger | Feral | Guardian, Warrior | `badger_feral_guardian_warrior.png` | `48_f.png` |
| `bat-rogue-shadow` | — | Bat Rogue Shadow | Bladed | Assassin, Flying | `bat-rogue-shadow_bladed_assassin_flying.png` | `bat-rogue-shadow.png` |
| `bear-brown` | `brown_bear` | Brown Bear | Feral | Warrior, Guardian | `bear-brown_feral_warrior_guardian.png` | `40_f.png` |
| `beetle-2` | — | Beetle 2 | Feral | Guardian, Warrior | `beetle-2_feral_guardian_warrior.png` | `beetle-2.png` |
| `beetle-knight-shield` | — | Beetle Knight Shield | Blunt | Guardian, Bodyguard | `beetle-knight-shield_blunt_guardian_bodyguard.png` | `beetle-knight-shield.png` |
| `beetle-rhino` | `rhino_beetle` | Rhino Beetle | Feral | Guardian, Warrior | `beetle-rhino_feral_guardian_warrior.png` | `15_f.png` |
| `beetle` | — | Beetle | Feral | Guardian, Warrior | `beetle_feral_guardian_warrior.png` | `beetle.png` |
| `bison` | `bison` | Bison | Feral | Guardian, Warrior | `bison_feral_guardian_warrior.png` | `42_f.png` |
| `bloom-pod` | `bloom_pod` | Bloom Pod | Breath | Healer, Alchemist | `bloom-pod_breath_healer_alchemist.png` | `29_f.png` |
| `bloomspear` | `bloomspear` | Bloomspear | Polearm | Primalist, Hunter | `bloomspear_polearm_primalist_hunter.png` | `7_f.png` |
| `boar-magma` | `magma_hog` | Magma Hog | Feral | Warrior, Sorcerer | `boar-magma_feral_warrior_sorcerer.png` | `10_f.png` |
| `boar` | `boar` | Boar | Feral | Warrior | `boar_feral_warrior.png` | `47_f.png` |
| `bone-idol` | `bone_idol` | Bone Idol | Magic | Black Mage, Leader | `bone-idol_magic_black-mage_leader.png` | `4_f.png` |
| `caterpillar-bard` | — | Caterpillar Bard | Magic | Bard, Healer | `caterpillar-bard_magic_bard_healer.png` | `caterpillar-bard.png` |
| `centaur-ranger` | — | Centaur Ranger | Polearm | Hunter, Warrior | `centaur-ranger_polearm_hunter_warrior.png` | `centaur-ranger.png` |
| `centipede` | — | Centipede | Feral | Assassin, Alchemist | `centipede_feral_assassin_alchemist.png` | `centipede.png` |
| `cinderhorn` | `cinderhorn` | Cinderhorn | Bladed | Warrior, Leader | `cinderhorn_bladed_warrior_leader.png` | `8_f.png` |
| `cobra` | — | Cobra | Breath | Alchemist, Assassin | `cobra_breath_alchemist_assassin.png` | `cobra.png` |
| `crab-alchemist` | — | Crab Alchemist | Thrown | Alchemist | `crab-alchemist_thrown_alchemist.png` | `crab-alchemist.png` |
| `crab-horseshoe` | — | Crab Horseshoe | Feral | Guardian, Warrior | `crab-horseshoe_feral_guardian_warrior.png` | `crab-horseshoe.png` |
| `crab-smith` | — | Crab Smith | Blunt | Engineer, Guardian | `crab-smith_blunt_engineer_guardian.png` | `crab-smith.png` |
| `crocodile-shaman` | — | Crocodile Shaman | Feral | Primalist, Warrior | `crocodile-shaman_feral_primalist_warrior.png` | `crocodile-shaman.png` |
| `dormouse` | `dormouse` | Dormouse | Feral | Healer, Trickster | `dormouse_feral_healer_trickster.png` | `39_f.png` |
| `elephant` | `elephant` | Elephant | Feral | Guardian, Leader | `elephant_feral_guardian_leader.png` | `44_f.png` |
| `ember-shard` | `ember_shard` | Ember Shard | Magic | Sorcerer | `ember-shard_magic_sorcerer.png` | `28_f.png` |
| `emberbulb` | `emberbulb` | Emberbulb | Magic | Sorcerer | `emberbulb_magic_sorcerer.png` | `20_f.png` |
| `firefly` | — | Firefly | Magic | Flying, Sorcerer | `firefly_magic_flying_sorcerer.png` | `firefly.png` |
| `fox-bard` | — | Fox Bard | Feral | Bard, Trickster | `fox-bard_feral_bard_trickster.png` | `fox-bard.png` |
| `fox` | `fox` | Fox | Feral | Hunter, Trickster | `fox_feral_hunter_trickster.png` | `35_f.png` |
| `frog-bard-dagger` | — | Frog Bard Dagger | Bladed | Bard, Trickster | `frog-bard-dagger_bladed_bard_trickster.png` | `frog-bard-dagger.png` |
| `gazelle-plume` | `plume_gazelle` | Plume Gazelle | Feral | Leader, Hunter | `gazelle-plume_feral_leader_hunter.png` | `22_f.png` |
| `halfling-alchemist` | — | Halfling Alchemist | Thrown | Alchemist | `halfling-alchemist_thrown_alchemist.png` | `halfling-alchemist.png` |
| `hare` | `hare` | Hare | Feral | Trickster | `hare_feral_trickster.png` | `36_f.png` |
| `hedgehog` | `hedgehog` | Hedgehog | Feral | Guardian | `hedgehog_feral_guardian.png` | `38_f.png` |
| `human-alchemist-shield` | — | Human Alchemist Shield | Thrown | Alchemist, Guardian | `human-alchemist-shield_thrown_alchemist_guardian.png` | `human-alchemist-shield.png` |
| `human-alchemist` | — | Human Alchemist | Thrown | Alchemist, Hunter | `human-alchemist_thrown_alchemist_hunter.png` | `human-alchemist.png` |
| `human-archer-crystal` | — | Human Archer Crystal | Ranged | Sorcerer, Hunter | `human-archer-crystal_ranged_sorcerer_hunter.png` | `human-archer-crystal.png` |
| `human-archer-frost` | — | Human Archer Frost | Ranged | Hunter, Sorcerer | `human-archer-frost_ranged_hunter_sorcerer.png` | `human-archer-frost.png` |
| `human-archer-gear` | — | Human Archer Gear | Ranged | Hunter, Engineer | `human-archer-gear_ranged_hunter_engineer.png` | `human-archer-gear.png` |
| `human-archer-sun` | — | Human Archer Sun | Ranged | Hunter, Healer | `human-archer-sun_ranged_hunter_healer.png` | `human-archer-sun.png` |
| `human-archer` | — | Human Archer | Ranged | Hunter | `human-archer_ranged_hunter.png` | `human-archer.png` |
| `human-barbarian-frost` | — | Human Barbarian Frost | Bladed | Warrior, Sorcerer | `human-barbarian-frost_bladed_warrior_sorcerer.png` | `human-barbarian-frost.png` |
| `human-barbarian` | — | Human Barbarian | Bladed | Warrior | `human-barbarian_bladed_warrior.png` | `human-barbarian.png` |
| `human-bard-shield` | — | Human Bard Shield | Blunt | Bard, Guardian | `human-bard-shield_blunt_bard_guardian.png` | `human-bard-shield.png` |
| `human-bard` | — | Human Bard | Magic | Bard | `human-bard_magic_bard.png` | `human-bard.png` |
| `human-brawler-hook` | — | Human Brawler Hook | Blunt | Warrior, Bodyguard | `human-brawler-hook_blunt_warrior_bodyguard.png` | `human-brawler-hook.png` |
| `human-captain-banner` | — | Human Captain Banner | Polearm | Leader, Warrior | `human-captain-banner_polearm_leader_warrior.png` | `human-captain-banner.png` |
| `human-captain-pistol` | — | Human Captain Pistol | Ranged | Leader, Trickster | `human-captain-pistol_ranged_leader_trickster.png` | `human-captain-pistol.png` |
| `human-captain-rapier` | — | Human Captain Rapier | Bladed | Leader, Trickster | `human-captain-rapier_bladed_leader_trickster.png` | `human-captain-rapier.png` |
| `human-cleric-lantern` | — | Human Cleric Lantern | Magic | Healer, Alchemist | `human-cleric-lantern_magic_healer_alchemist.png` | `human-cleric-lantern.png` |
| `human-dancer-spear` | — | Human Dancer Spear | Polearm | Bard, Warrior | `human-dancer-spear_polearm_bard_warrior.png` | `human-dancer-spear.png` |
| `human-dancer` | — | Human Dancer | Bladed | Bard, Assassin | `human-dancer_bladed_bard_assassin.png` | `human-dancer.png` |
| `human-druid-leaf` | — | Human Druid Leaf | Magic | Primalist, Healer | `human-druid-leaf_magic_primalist_healer.png` | `human-druid-leaf.png` |
| `human-druid` | — | Human Druid | Magic | Primalist, Healer | `human-druid_magic_primalist_healer.png` | `human-druid.png` |
| `human-duelist-rapier` | — | Human Duelist Rapier | Bladed | Warrior, Trickster | `human-duelist-rapier_bladed_warrior_trickster.png` | `human-duelist-rapier.png` |
| `human-engineer-crystal` | — | Human Engineer Crystal | Magic | Engineer, Sorcerer | `human-engineer-crystal_magic_engineer_sorcerer.png` | `human-engineer-crystal.png` |
| `human-engineer-hook` | — | Human Engineer Hook | Thrown | Engineer, Trickster | `human-engineer-hook_thrown_engineer_trickster.png` | `human-engineer-hook.png` |
| `human-guard-frost` | — | Human Guard Frost | Polearm | Sorcerer, Warrior | `human-guard-frost_polearm_sorcerer_warrior.png` | `human-guard-frost.png` |
| `human-guard-stone` | — | Human Guard Stone | Bladed | Guardian, Sorcerer | `human-guard-stone_bladed_guardian_sorcerer.png` | `human-guard-stone.png` |
| `human-guard` | — | Human Guard | Polearm | Guardian, Bodyguard | `human-guard_polearm_guardian_bodyguard.png` | `human-guard.png` |
| `human-knight-2` | — | Human Knight 2 | Bladed | Warrior, Guardian | `human-knight-2_bladed_warrior_guardian.png` | `human-knight-2.png` |
| `human-knight-banner` | — | Human Knight Banner | Blunt | Leader, Guardian | `human-knight-banner_blunt_leader_guardian.png` | `human-knight-banner.png` |
| `human-knight-hammer` | — | Human Knight Hammer | Blunt | Guardian, Warrior | `human-knight-hammer_blunt_guardian_warrior.png` | `human-knight-hammer.png` |
| `human-knight-shield` | — | Human Knight Shield | Blunt | Guardian, Bodyguard | `human-knight-shield_blunt_guardian_bodyguard.png` | `human-knight-shield.png` |
| `human-knight` | — | Human Knight | Bladed | Warrior, Guardian | `human-knight_bladed_warrior_guardian.png` | `human-knight.png` |
| `human-mage-crystal` | — | Human Mage Crystal | Magic | Sorcerer | `human-mage-crystal_magic_sorcerer.png` | `human-mage-crystal.png` |
| `human-mage-fire` | — | Human Mage Fire | Magic | Sorcerer | `human-mage-fire_magic_sorcerer.png` | `human-mage-fire.png` |
| `human-mage-frost` | — | Human Mage Frost | Magic | Sorcerer | `human-mage-frost_magic_sorcerer.png` | `human-mage-frost.png` |
| `human-mage-stone` | — | Human Mage Stone | Magic | Sorcerer | `human-mage-stone_magic_sorcerer.png` | `human-mage-stone.png` |
| `human-mage-water` | — | Human Mage Water | Magic | Sorcerer, Leader | `human-mage-water_magic_sorcerer_leader.png` | `human-mage-water.png` |
| `human-monk-hammer` | — | Human Monk Hammer | Blunt | Warrior, Guardian | `human-monk-hammer_blunt_warrior_guardian.png` | `human-monk-hammer.png` |
| `human-necromancer-skull` | — | Human Necromancer Skull | Bladed | Black Mage, Warrior | `human-necromancer-skull_bladed_black-mage_warrior.png` | `human-necromancer-skull.png` |
| `human-priest-bell` | — | Human Priest Bell | Blunt | Healer, Bard | `human-priest-bell_blunt_healer_bard.png` | `human-priest-bell.png` |
| `human-priest-moon` | — | Human Priest Moon | Magic | Healer, Sorcerer | `human-priest-moon_magic_healer_sorcerer.png` | `human-priest-moon.png` |
| `human-priest-sun` | — | Human Priest Sun | Ranged | Healer, Primalist | `human-priest-sun_ranged_healer_primalist.png` | `human-priest-sun.png` |
| `human-priest` | — | Human Priest | Magic | Healer, Leader | `human-priest_magic_healer_leader.png` | `human-priest.png` |
| `human-rogue-potion` | — | Human Rogue Potion | Bladed | Alchemist, Assassin | `human-rogue-potion_bladed_alchemist_assassin.png` | `human-rogue-potion.png` |
| `human-rogue-shadow` | — | Human Rogue Shadow | Bladed | Assassin, Black Mage | `human-rogue-shadow_bladed_assassin_black-mage.png` | `human-rogue-shadow.png` |
| `human-rogue` | — | Human Rogue | Bladed | Assassin | `human-rogue_bladed_assassin.png` | `human-rogue.png` |
| `human-scout-lantern` | — | Human Scout Lantern | Bladed | Hunter, Healer | `human-scout-lantern_bladed_hunter_healer.png` | `human-scout-lantern.png` |
| `human-shaman` | — | Human Shaman | Magic | Primalist, Summoner | `human-shaman_magic_primalist_summoner.png` | `human-shaman.png` |
| `human-smith` | — | Human Smith | Blunt | Engineer, Warrior | `human-smith_blunt_engineer_warrior.png` | `human-smith.png` |
| `human-spellblade-2` | — | Human Spellblade 2 | Bladed | Warrior, Sorcerer | `human-spellblade-2_bladed_warrior_sorcerer.png` | `human-spellblade-2.png` |
| `human-spellblade-3` | — | Human Spellblade 3 | Bladed | Warrior, Sorcerer | `human-spellblade-3_bladed_warrior_sorcerer.png` | `human-spellblade-3.png` |
| `human-spellblade` | — | Human Spellblade | Bladed | Warrior, Sorcerer | `human-spellblade_bladed_warrior_sorcerer.png` | `human-spellblade.png` |
| `human-wizard` | — | Human Wizard | Magic | Sorcerer | `human-wizard_magic_sorcerer.png` | `human-wizard.png` |
| `jelly-gazer` | `gazer_jelly` | Gazer Jelly | Magic | Sorcerer, Black Mage | `jelly-gazer_magic_sorcerer_black-mage.png` | `2_f.png` |
| `jelly-healer` | — | Jelly Healer | Aura | Healer, Primalist | `jelly-healer_aura_healer_primalist.png` | `jelly-healer.png` |
| `kangaroo` | `kangaroo` | Kangaroo | Unarmed | Warrior | `kangaroo_unarmed_warrior.png` | `52_f.png` |
| `leafwisp` | `leafwisp` | Leafwisp | Magic | Healer, Primalist | `leafwisp_magic_healer_primalist.png` | `19_f.png` |
| `lizardfolk-rogue-fire` | — | Lizardfolk Rogue Fire | Bladed | Assassin, Hunter | `lizardfolk-rogue-fire_bladed_assassin_hunter.png` | `lizardfolk-rogue-fire.png` |
| `lynx` | `lynx` | Lynx | Feral | Hunter, Assassin | `lynx_feral_hunter_assassin.png` | `46_f.png` |
| `magma-crawler` | `magma_crawler` | Magma Crawler | Breath | Sorcerer, Warrior | `magma-crawler_breath_sorcerer_warrior.png` | `25_f.png` |
| `mannequin` | `mannequin` | Mannequin | Unarmed | Warrior, Guardian | `mannequin_unarmed_warrior_guardian.png` | `34_f.png` |
| `mantis-sickle` | `sickle_mantis` | Sickle Mantis | Feral | Assassin, Warrior | `mantis-sickle_feral_assassin_warrior.png` | `24_f.png` |
| `mole-giant` | `giant_mole` | Giant Mole | Feral | Warrior, Guardian | `mole-giant_feral_warrior_guardian.png` | `33_f.png` |
| `moose` | `moose` | Moose | Feral | Guardian, Leader | `moose_feral_guardian_leader.png` | `41_f.png` |
| `mossback` | `mossback` | Mossback | Feral | Guardian, Healer | `mossback_feral_guardian_healer.png` | `18_f.png` |
| `moth-ash` | `ash_moth` | Ash Moth | Aura | Sorcerer, Flying | `moth-ash_aura_sorcerer_flying.png` | `32_f.png` |
| `mushroom-druid` | — | Mushroom Druid | Magic | Primalist, Healer | `mushroom-druid_magic_primalist_healer.png` | `mushroom-druid.png` |
| `nautilus` | — | Nautilus | Lashing | Sorcerer, Healer | `nautilus_lashing_sorcerer_healer.png` | `nautilus.png` |
| `octopus-alchemist` | — | Octopus Alchemist | Thrown | Alchemist, Trickster | `octopus-alchemist_thrown_alchemist_trickster.png` | `octopus-alchemist.png` |
| `otter` | `otter` | Otter | Feral | Healer, Trickster | `otter_feral_healer_trickster.png` | `37_f.png` |
| `owl-wizard` | — | Owl Wizard | Magic | Sorcerer, Flying | `owl-wizard_magic_sorcerer_flying.png` | `owl-wizard.png` |
| `pillbug` | — | Pillbug | Feral | Guardian, Bodyguard | `pillbug_feral_guardian_bodyguard.png` | `pillbug.png` |
| `raccoon` | `raccoon` | Raccoon | Feral | Trickster, Hunter | `raccoon_feral_trickster_hunter.png` | `54_f.png` |
| `ram-paladin-sun` | — | Ram Paladin Sun | Feral | Primalist, Leader | `ram-paladin-sun_feral_primalist_leader.png` | `ram-paladin-sun.png` |
| `ray-mage-crystal` | — | Ray Mage Crystal | Magic | Flying, Sorcerer | `ray-mage-crystal_magic_flying_sorcerer.png` | `ray-mage-crystal.png` |
| `ray-shade` | `shade_ray` | Shade Ray | Lashing | Assassin, Flying | `ray-shade_lashing_assassin_flying.png` | `17_f.png` |
| `ray` | — | Ray | Feral | Flying, Hunter | `ray_feral_flying_hunter.png` | `ray.png` |
| `reef-warden` | `reef_warden` | Reef Warden | Polearm | Guardian, Leader | `reef-warden_polearm_guardian_leader.png` | `9_f.png` |
| `rhino` | `rhino` | Rhino | Feral | Warrior | `rhino_feral_warrior.png` | `43_f.png` |
| `sailback` | `sailback` | Sailback | Feral | Sorcerer, Warrior | `sailback_feral_sorcerer_warrior.png` | `23_f.png` |
| `salamander-rogue` | — | Salamander Rogue | Feral | Trickster, Assassin | `salamander-rogue_feral_trickster_assassin.png` | `salamander-rogue.png` |
| `scorpion-gloom` | `gloom_scorpion` | Gloom Scorpion | Lashing | Assassin, Alchemist | `scorpion-gloom_lashing_assassin_alchemist.png` | `27_f.png` |
| `scorpion` | — | Scorpion | Lashing | Assassin, Alchemist | `scorpion_lashing_assassin_alchemist.png` | `scorpion.png` |
| `shark-fin-stalker` | `fin_stalker` | Fin Stalker | Feral | Hunter, Assassin | `shark-fin-stalker_feral_hunter_assassin.png` | `16_f.png` |
| `shark-land` | `land_shark` | Land Shark | Feral | Warrior, Hunter | `shark-land_feral_warrior_hunter.png` | `12_f.png` |
| `slug-sea` | — | Slug Sea | Aura | Alchemist, Healer | `slug-sea_aura_alchemist_healer.png` | `slug-sea.png` |
| `snail-cleric-crystal` | — | Snail Cleric Crystal | Magic | Sorcerer, Healer | `snail-cleric-crystal_magic_sorcerer_healer.png` | `snail-cleric-crystal.png` |
| `snail-cleric-lantern` | — | Snail Cleric Lantern | Magic | Healer, Primalist | `snail-cleric-lantern_magic_healer_primalist.png` | `snail-cleric-lantern.png` |
| `snail-ember` | `ember_snail` | Ember Snail | Breath | Sorcerer, Guardian | `snail-ember_breath_sorcerer_guardian.png` | `14_f.png` |
| `snail-prism` | `prism_snail` | Prism Snail | Magic | Healer, Sorcerer | `snail-prism_magic_healer_sorcerer.png` | `3_f.png` |
| `spider-mage-crystal` | — | Spider Mage Crystal | Magic | Sorcerer, Trickster | `spider-mage-crystal_magic_sorcerer_trickster.png` | `spider-mage-crystal.png` |
| `spider-rogue` | — | Spider Rogue | Feral | Trickster, Assassin | `spider-rogue_feral_trickster_assassin.png` | `spider-rogue.png` |
| `spikeback` | `spikeback` | Spikeback | Feral | Warrior, Guardian | `spikeback_feral_warrior_guardian.png` | `1_f.png` |
| `stag-glade` | `glade_stag` | Glade Stag | Feral | Primalist, Healer | `stag-glade_feral_primalist_healer.png` | `26_f.png` |
| `starfish-mage-crystal` | — | Starfish Mage Crystal | Magic | Sorcerer, Healer | `starfish-mage-crystal_magic_sorcerer_healer.png` | `starfish-mage-crystal.png` |
| `thornfang` | `thornfang` | Thornfang | Feral | Hunter, Warrior | `thornfang_feral_hunter_warrior.png` | `6_f.png` |
| `tiger` | `tiger` | Tiger | Feral | Warrior, Hunter | `tiger_feral_warrior_hunter.png` | `50_f.png` |
| `toad-bog` | `bog_toad` | Bog Toad | Breath | Alchemist, Healer | `toad-bog_breath_alchemist_healer.png` | `31_f.png` |
| `toad-boulder` | `boulder_toad` | Boulder Toad | Blunt | Guardian, Warrior | `toad-boulder_blunt_guardian_warrior.png` | `5_f.png` |
| `toad-druid` | — | Toad Druid | Breath | Alchemist, Primalist | `toad-druid_breath_alchemist_primalist.png` | `toad-druid.png` |
| `turtle-hornshell` | `hornshell` | Hornshell Tortoise | Feral | Guardian | `turtle-hornshell_feral_guardian.png` | `11_f.png` |
| `turtle-monk` | — | Turtle Monk | Blunt | Healer, Guardian | `turtle-monk_blunt_healer_guardian.png` | `turtle-monk.png` |
| `turtle-stone` | `stone_turtle` | Stone Turtle | Feral | Guardian, Bodyguard | `turtle-stone_feral_guardian_bodyguard.png` | `21_f.png` |
| `wolf-ranger` | — | Wolf Ranger | Feral | Hunter, Bodyguard | `wolf-ranger_feral_hunter_bodyguard.png` | `wolf-ranger.png` |
| `wolf` | `wolf` | Wolf | Feral | Hunter, Leader | `wolf_feral_hunter_leader.png` | `45_f.png` |
| `wolverine` | `wolverine` | Wolverine | Feral | Warrior | `wolverine_feral_warrior.png` | `49_f.png` |
| `worm-engineer` | — | Worm Engineer | Feral | Engineer, Hunter | `worm-engineer_feral_engineer_hunter.png` | `worm-engineer.png` |
