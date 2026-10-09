# Creature Classes

Attack type and professions for every creature that has art, following [class_system.md](class_system.md) v1.1 (assessed 2026-10-09 from each creature's art and, for the first 54, its earlier classes in [creatures.md](creatures.md)). Each creature has one attack type and one or two professions; the first profession is the main one.

The table also drives the art pipeline: `tools/art/clean_creatures.py` reads each row's **Source** in `images/kin/` and writes the cleaned copy to `art/creatures/` under the **Art file** name.

**Art file names:** `{attackType}_{profession1}_{profession2}_{unitName}.png`, or `{attackType}_{profession}_{unitName}.png` for one profession. Everything is lowercase; underscores separate the parts, and hyphens join words inside a part (`black-mage`, `glade-stag`). For creatures with a creature ID in creatures.md, the unit name is that ID with hyphens. All other creatures have no stats yet and are named by the schema below (adopted 2026-10-09).

## Unit name schema

`{kind}-{archetype}[-{signature}][-{n}]`, so that similar designs get similar names and sort next to each other.

| Part | What it says | Vocabulary |
|---|---|---|
| **kind** | What the creature is | `human`, `elf`, `dwarf`, `gnome`, `halfling`, `orc`, `centaur`, `lizardfolk`, `catfolk`, or the animal or plant (`fox`, `snail`, `frog`, `turtle`, `jelly`, `beetle`, `mushroom` …) |
| **archetype** | The role the costume shows; left out for an animal without gear (`cobra`, `pillbug`) | `alchemist`, `archer`, `bard`, `barbarian`, `brawler`, `captain`, `cleric`, `dancer`, `druid`, `duelist`, `engineer`, `guard`, `healer`, `knight`, `mage`, `monk`, `necromancer`, `paladin`, `priest`, `ranger`, `rogue`, `scout`, `shaman`, `smith`, `spellblade`, `witch`, `wizard` |
| **signature** | Optional: the one prop or element that sets the design apart | `banner`, `bell`, `censer`, `crystal`, `dagger`, `feather`, `fire`, `frost`, `gear`, `hammer`, `harpoon`, `hook`, `lantern`, `leaf`, `moon`, `pistol`, `potion`, `rapier`, `shadow`, `shield`, `skull`, `sling`, `spear`, `stone`, `sun`, `totem`, `water` |
| **n** | Counter for near-identical designs | `2`, `3` … in the order the art was added; the first has none |

1. Name what the art shows, never a made-up title.
2. Pick each part from its list. A new word joins a list only when no existing word fits.
3. Look-alikes share kind, archetype and signature and differ only in the counter.
4. Look-alikes get the same attack type and professions unless the art clearly shows a different fighting style.

| Unit | ID | Name | Attack type | Professions | Art file | Source |
|---|---|---|---|---|---|---|
| `anteater` | `anteater` | Giant Anteater | Feral | Hunter, Assassin | `feral_hunter_assassin_anteater.png` | `53_f.png` |
| `ash-moth` | `ash_moth` | Ash Moth | Aura | Sorcerer, Flying | `aura_sorcerer_flying_ash-moth.png` | `32_f.png` |
| `badger` | `badger` | Badger | Feral | Guardian, Warrior | `feral_guardian_warrior_badger.png` | `48_f.png` |
| `bison` | `bison` | Bison | Feral | Guardian, Warrior | `feral_guardian_warrior_bison.png` | `42_f.png` |
| `bloom-pod` | `bloom_pod` | Bloom Pod | Breath | Healer, Alchemist | `breath_healer_alchemist_bloom-pod.png` | `29_f.png` |
| `bloomspear` | `bloomspear` | Bloomspear | Polearm | Primalist, Hunter | `polearm_primalist_hunter_bloomspear.png` | `7_f.png` |
| `boar` | `boar` | Boar | Feral | Warrior | `feral_warrior_boar.png` | `47_f.png` |
| `bog-toad` | `bog_toad` | Bog Toad | Breath | Alchemist, Healer | `breath_alchemist_healer_bog-toad.png` | `31_f.png` |
| `bone-idol` | `bone_idol` | Bone Idol | Magic | Black Mage, Leader | `magic_black-mage_leader_bone-idol.png` | `4_f.png` |
| `boulder-toad` | `boulder_toad` | Boulder Toad | Blunt | Guardian, Warrior | `blunt_guardian_warrior_boulder-toad.png` | `5_f.png` |
| `brown-bear` | `brown_bear` | Brown Bear | Feral | Warrior, Guardian | `feral_warrior_guardian_brown-bear.png` | `40_f.png` |
| `cinderhorn` | `cinderhorn` | Cinderhorn | Bladed | Warrior, Leader | `bladed_warrior_leader_cinderhorn.png` | `8_f.png` |
| `dormouse` | `dormouse` | Dormouse | Feral | Healer, Trickster | `feral_healer_trickster_dormouse.png` | `39_f.png` |
| `elephant` | `elephant` | Elephant | Feral | Guardian, Leader | `feral_guardian_leader_elephant.png` | `44_f.png` |
| `ember-shard` | `ember_shard` | Ember Shard | Magic | Sorcerer | `magic_sorcerer_ember-shard.png` | `28_f.png` |
| `ember-snail` | `ember_snail` | Ember Snail | Breath | Sorcerer, Guardian | `breath_sorcerer_guardian_ember-snail.png` | `14_f.png` |
| `emberbulb` | `emberbulb` | Emberbulb | Magic | Sorcerer | `magic_sorcerer_emberbulb.png` | `20_f.png` |
| `fin-stalker` | `fin_stalker` | Fin Stalker | Feral | Hunter, Assassin | `feral_hunter_assassin_fin-stalker.png` | `16_f.png` |
| `fox` | `fox` | Fox | Feral | Hunter, Trickster | `feral_hunter_trickster_fox.png` | `35_f.png` |
| `gazer-jelly` | `gazer_jelly` | Gazer Jelly | Magic | Sorcerer, Black Mage | `magic_sorcerer_black-mage_gazer-jelly.png` | `2_f.png` |
| `giant-mole` | `giant_mole` | Giant Mole | Feral | Warrior, Guardian | `feral_warrior_guardian_giant-mole.png` | `33_f.png` |
| `glade-stag` | `glade_stag` | Glade Stag | Feral | Primalist, Healer | `feral_primalist_healer_glade-stag.png` | `26_f.png` |
| `gloom-scorpion` | `gloom_scorpion` | Gloom Scorpion | Lashing | Assassin, Alchemist | `lashing_assassin_alchemist_gloom-scorpion.png` | `27_f.png` |
| `hare` | `hare` | Hare | Feral | Trickster | `feral_trickster_hare.png` | `36_f.png` |
| `hedgehog` | `hedgehog` | Hedgehog | Feral | Guardian | `feral_guardian_hedgehog.png` | `38_f.png` |
| `hornshell` | `hornshell` | Hornshell Tortoise | Feral | Guardian | `feral_guardian_hornshell.png` | `11_f.png` |
| `kangaroo` | `kangaroo` | Kangaroo | Unarmed | Warrior | `unarmed_warrior_kangaroo.png` | `52_f.png` |
| `land-shark` | `land_shark` | Land Shark | Feral | Warrior, Hunter | `feral_warrior_hunter_land-shark.png` | `12_f.png` |
| `leafwisp` | `leafwisp` | Leafwisp | Magic | Healer, Primalist | `magic_healer_primalist_leafwisp.png` | `19_f.png` |
| `lynx` | `lynx` | Lynx | Feral | Hunter, Assassin | `feral_hunter_assassin_lynx.png` | `46_f.png` |
| `magma-crawler` | `magma_crawler` | Magma Crawler | Breath | Sorcerer, Warrior | `breath_sorcerer_warrior_magma-crawler.png` | `25_f.png` |
| `magma-hog` | `magma_hog` | Magma Hog | Feral | Warrior, Sorcerer | `feral_warrior_sorcerer_magma-hog.png` | `10_f.png` |
| `mannequin` | `mannequin` | Mannequin | Unarmed | Warrior, Guardian | `unarmed_warrior_guardian_mannequin.png` | `34_f.png` |
| `moose` | `moose` | Moose | Feral | Guardian, Leader | `feral_guardian_leader_moose.png` | `41_f.png` |
| `mossback` | `mossback` | Mossback | Feral | Guardian, Healer | `feral_guardian_healer_mossback.png` | `18_f.png` |
| `otter` | `otter` | Otter | Feral | Healer, Trickster | `feral_healer_trickster_otter.png` | `37_f.png` |
| `plume-gazelle` | `plume_gazelle` | Plume Gazelle | Feral | Leader, Hunter | `feral_leader_hunter_plume-gazelle.png` | `22_f.png` |
| `prism-snail` | `prism_snail` | Prism Snail | Magic | Healer, Sorcerer | `magic_healer_sorcerer_prism-snail.png` | `3_f.png` |
| `raccoon` | `raccoon` | Raccoon | Feral | Trickster, Hunter | `feral_trickster_hunter_raccoon.png` | `54_f.png` |
| `reef-warden` | `reef_warden` | Reef Warden | Polearm | Guardian, Leader | `polearm_guardian_leader_reef-warden.png` | `9_f.png` |
| `rhino` | `rhino` | Rhino | Feral | Warrior | `feral_warrior_rhino.png` | `43_f.png` |
| `rhino-beetle` | `rhino_beetle` | Rhino Beetle | Feral | Guardian, Warrior | `feral_guardian_warrior_rhino-beetle.png` | `15_f.png` |
| `sailback` | `sailback` | Sailback | Feral | Sorcerer, Warrior | `feral_sorcerer_warrior_sailback.png` | `23_f.png` |
| `shade-ray` | `shade_ray` | Shade Ray | Lashing | Assassin, Flying | `lashing_assassin_flying_shade-ray.png` | `17_f.png` |
| `sickle-mantis` | `sickle_mantis` | Sickle Mantis | Feral | Assassin, Warrior | `feral_assassin_warrior_sickle-mantis.png` | `24_f.png` |
| `spikeback` | `spikeback` | Spikeback | Feral | Warrior, Guardian | `feral_warrior_guardian_spikeback.png` | `1_f.png` |
| `stone-turtle` | `stone_turtle` | Stone Turtle | Feral | Guardian, Bodyguard | `feral_guardian_bodyguard_stone-turtle.png` | `21_f.png` |
| `thornfang` | `thornfang` | Thornfang | Feral | Hunter, Warrior | `feral_hunter_warrior_thornfang.png` | `6_f.png` |
| `tiger` | `tiger` | Tiger | Feral | Warrior, Hunter | `feral_warrior_hunter_tiger.png` | `50_f.png` |
| `wolf` | `wolf` | Wolf | Feral | Hunter, Leader | `feral_hunter_leader_wolf.png` | `45_f.png` |
| `wolverine` | `wolverine` | Wolverine | Feral | Warrior | `feral_warrior_wolverine.png` | `49_f.png` |
| `woodpecker` | `woodpecker` | Giant Woodpecker | Feral | Hunter, Flying | `feral_hunter_flying_woodpecker.png` | `30_f.png` |
| `zebra` | `zebra` | Zebra | Feral | Leader | `feral_leader_zebra.png` | `51_f.png` |
| `armadillo-knight-2` | — | Armadillo Knight 2 | Feral | Guardian, Bodyguard | `feral_guardian_bodyguard_armadillo-knight-2.png` | `armadillo-knight-2.png` |
| `armadillo-knight` | — | Armadillo Knight | Blunt | Guardian, Bodyguard | `blunt_guardian_bodyguard_armadillo-knight.png` | `armadillo-knight.png` |
| `bat-rogue-shadow` | — | Bat Rogue Shadow | Bladed | Assassin, Flying | `bladed_assassin_flying_bat-rogue-shadow.png` | `bat-rogue-shadow.png` |
| `beetle-2` | — | Beetle 2 | Feral | Guardian, Warrior | `feral_guardian_warrior_beetle-2.png` | `beetle-2.png` |
| `beetle-knight-shield` | — | Beetle Knight Shield | Blunt | Guardian, Bodyguard | `blunt_guardian_bodyguard_beetle-knight-shield.png` | `beetle-knight-shield.png` |
| `beetle` | — | Beetle | Feral | Guardian, Warrior | `feral_guardian_warrior_beetle.png` | `beetle.png` |
| `caterpillar-bard` | — | Caterpillar Bard | Magic | Bard, Healer | `magic_bard_healer_caterpillar-bard.png` | `caterpillar-bard.png` |
| `catfolk-paladin` | — | Catfolk Paladin | Blunt | Guardian, Primalist | `blunt_guardian_primalist_catfolk-paladin.png` | `catfolk-paladin.png` |
| `centaur-ranger` | — | Centaur Ranger | Polearm | Hunter, Warrior | `polearm_hunter_warrior_centaur-ranger.png` | `centaur-ranger.png` |
| `centipede` | — | Centipede | Feral | Assassin, Alchemist | `feral_assassin_alchemist_centipede.png` | `centipede.png` |
| `cobra` | — | Cobra | Breath | Alchemist, Assassin | `breath_alchemist_assassin_cobra.png` | `cobra.png` |
| `crab-alchemist` | — | Crab Alchemist | Thrown | Alchemist | `thrown_alchemist_crab-alchemist.png` | `crab-alchemist.png` |
| `crab-smith` | — | Crab Smith | Blunt | Engineer, Guardian | `blunt_engineer_guardian_crab-smith.png` | `crab-smith.png` |
| `crocodile-shaman` | — | Crocodile Shaman | Feral | Primalist, Warrior | `feral_primalist_warrior_crocodile-shaman.png` | `crocodile-shaman.png` |
| `dwarf-smith-shield` | — | Dwarf Smith Shield | Blunt | Engineer, Guardian | `blunt_engineer_guardian_dwarf-smith-shield.png` | `dwarf-smith-shield.png` |
| `elf-archer-leaf` | — | Elf Archer Leaf | Ranged | Hunter, Primalist | `ranged_hunter_primalist_elf-archer-leaf.png` | `elf-archer-leaf.png` |
| `firefly` | — | Firefly | Magic | Flying, Sorcerer | `magic_flying_sorcerer_firefly.png` | `firefly.png` |
| `fox-bard` | — | Fox Bard | Feral | Bard, Trickster | `feral_bard_trickster_fox-bard.png` | `fox-bard.png` |
| `frog-bard-dagger` | — | Frog Bard Dagger | Bladed | Bard, Trickster | `bladed_bard_trickster_frog-bard-dagger.png` | `frog-bard-dagger.png` |
| `gnome-engineer-crystal` | — | Gnome Engineer Crystal | Magic | Engineer, Sorcerer | `magic_engineer_sorcerer_gnome-engineer-crystal.png` | `gnome-engineer-crystal.png` |
| `halfling-alchemist` | — | Halfling Alchemist | Thrown | Alchemist | `thrown_alchemist_halfling-alchemist.png` | `halfling-alchemist.png` |
| `halfling-bard-dagger` | — | Halfling Bard Dagger | Bladed | Bard, Trickster | `bladed_bard_trickster_halfling-bard-dagger.png` | `halfling-bard-dagger.png` |
| `horseshoe-crab` | — | Horseshoe Crab | Feral | Guardian, Warrior | `feral_guardian_warrior_horseshoe-crab.png` | `horseshoe-crab.png` |
| `human-alchemist-shield` | — | Human Alchemist Shield | Thrown | Alchemist, Guardian | `thrown_alchemist_guardian_human-alchemist-shield.png` | `human-alchemist-shield.png` |
| `human-alchemist` | — | Human Alchemist | Thrown | Alchemist, Hunter | `thrown_alchemist_hunter_human-alchemist.png` | `human-alchemist.png` |
| `human-archer-2` | — | Human Archer 2 | Ranged | Hunter | `ranged_hunter_human-archer-2.png` | `human-archer-2.png` |
| `human-archer-3` | — | Human Archer 3 | Ranged | Hunter | `ranged_hunter_human-archer-3.png` | `human-archer-3.png` |
| `human-archer-4` | — | Human Archer 4 | Ranged | Hunter | `ranged_hunter_human-archer-4.png` | `human-archer-4.png` |
| `human-archer-5` | — | Human Archer 5 | Ranged | Hunter | `ranged_hunter_human-archer-5.png` | `human-archer-5.png` |
| `human-archer-crystal` | — | Human Archer Crystal | Ranged | Sorcerer, Hunter | `ranged_sorcerer_hunter_human-archer-crystal.png` | `human-archer-crystal.png` |
| `human-archer-feather` | — | Human Archer Feather | Ranged | Hunter, Primalist | `ranged_hunter_primalist_human-archer-feather.png` | `human-archer-feather.png` |
| `human-archer-frost` | — | Human Archer Frost | Ranged | Hunter, Sorcerer | `ranged_hunter_sorcerer_human-archer-frost.png` | `human-archer-frost.png` |
| `human-archer-gear-2` | — | Human Archer Gear 2 | Ranged | Hunter, Engineer | `ranged_hunter_engineer_human-archer-gear-2.png` | `human-archer-gear-2.png` |
| `human-archer-gear` | — | Human Archer Gear | Ranged | Hunter, Engineer | `ranged_hunter_engineer_human-archer-gear.png` | `human-archer-gear.png` |
| `human-archer-leaf-2` | — | Human Archer Leaf 2 | Ranged | Hunter, Primalist | `ranged_hunter_primalist_human-archer-leaf-2.png` | `human-archer-leaf-2.png` |
| `human-archer-leaf-3` | — | Human Archer Leaf 3 | Ranged | Hunter, Primalist | `ranged_hunter_primalist_human-archer-leaf-3.png` | `human-archer-leaf-3.png` |
| `human-archer-leaf` | — | Human Archer Leaf | Ranged | Hunter, Primalist | `ranged_hunter_primalist_human-archer-leaf.png` | `human-archer-leaf.png` |
| `human-archer-sun` | — | Human Archer Sun | Ranged | Hunter, Healer | `ranged_hunter_healer_human-archer-sun.png` | `human-archer-sun.png` |
| `human-archer` | — | Human Archer | Ranged | Hunter | `ranged_hunter_human-archer.png` | `human-archer.png` |
| `human-barbarian-frost-2` | — | Human Barbarian Frost 2 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-barbarian-frost-2.png` | `human-barbarian-frost-2.png` |
| `human-barbarian-frost-3` | — | Human Barbarian Frost 3 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-barbarian-frost-3.png` | `human-barbarian-frost-3.png` |
| `human-barbarian-frost` | — | Human Barbarian Frost | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-barbarian-frost.png` | `human-barbarian-frost.png` |
| `human-barbarian-totem-2` | — | Human Barbarian Totem 2 | Bladed | Warrior, Primalist | `bladed_warrior_primalist_human-barbarian-totem-2.png` | `human-barbarian-totem-2.png` |
| `human-barbarian-totem` | — | Human Barbarian Totem | Bladed | Warrior, Primalist | `bladed_warrior_primalist_human-barbarian-totem.png` | `human-barbarian-totem.png` |
| `human-barbarian` | — | Human Barbarian | Bladed | Warrior | `bladed_warrior_human-barbarian.png` | `human-barbarian.png` |
| `human-bard-dagger` | — | Human Bard Dagger | Bladed | Bard, Trickster | `bladed_bard_trickster_human-bard-dagger.png` | `human-bard-dagger.png` |
| `human-bard-rapier` | — | Human Bard Rapier | Bladed | Bard, Warrior | `bladed_bard_warrior_human-bard-rapier.png` | `human-bard-rapier.png` |
| `human-bard-shield-2` | — | Human Bard Shield 2 | Blunt | Bard, Guardian | `blunt_bard_guardian_human-bard-shield-2.png` | `human-bard-shield-2.png` |
| `human-bard-shield-3` | — | Human Bard Shield 3 | Blunt | Bard, Guardian | `blunt_bard_guardian_human-bard-shield-3.png` | `human-bard-shield-3.png` |
| `human-bard-shield` | — | Human Bard Shield | Blunt | Bard, Guardian | `blunt_bard_guardian_human-bard-shield.png` | `human-bard-shield.png` |
| `human-bard` | — | Human Bard | Magic | Bard | `magic_bard_human-bard.png` | `human-bard.png` |
| `human-brawler-hook` | — | Human Brawler Hook | Blunt | Warrior, Bodyguard | `blunt_warrior_bodyguard_human-brawler-hook.png` | `human-brawler-hook.png` |
| `human-brawler-potion` | — | Human Brawler Potion | Unarmed | Warrior, Alchemist | `unarmed_warrior_alchemist_human-brawler-potion.png` | `human-brawler-potion.png` |
| `human-brawler` | — | Human Brawler | Unarmed | Warrior | `unarmed_warrior_human-brawler.png` | `human-brawler.png` |
| `human-captain-banner` | — | Human Captain Banner | Polearm | Leader, Warrior | `polearm_leader_warrior_human-captain-banner.png` | `human-captain-banner.png` |
| `human-captain-pistol-2` | — | Human Captain Pistol 2 | Ranged | Leader, Trickster | `ranged_leader_trickster_human-captain-pistol-2.png` | `human-captain-pistol-2.png` |
| `human-captain-pistol` | — | Human Captain Pistol | Ranged | Leader, Trickster | `ranged_leader_trickster_human-captain-pistol.png` | `human-captain-pistol.png` |
| `human-captain-rapier` | — | Human Captain Rapier | Bladed | Leader, Trickster | `bladed_leader_trickster_human-captain-rapier.png` | `human-captain-rapier.png` |
| `human-cleric-lantern-2` | — | Human Cleric Lantern 2 | Blunt | Healer, Guardian | `blunt_healer_guardian_human-cleric-lantern-2.png` | `human-cleric-lantern-2.png` |
| `human-cleric-lantern-3` | — | Human Cleric Lantern 3 | Blunt | Healer, Guardian | `blunt_healer_guardian_human-cleric-lantern-3.png` | `human-cleric-lantern-3.png` |
| `human-cleric-lantern-4` | — | Human Cleric Lantern 4 | Blunt | Healer, Guardian | `blunt_healer_guardian_human-cleric-lantern-4.png` | `human-cleric-lantern-4.png` |
| `human-cleric-lantern` | — | Human Cleric Lantern | Magic | Healer, Alchemist | `magic_healer_alchemist_human-cleric-lantern.png` | `human-cleric-lantern.png` |
| `human-dancer-2` | — | Human Dancer 2 | Bladed | Bard, Assassin | `bladed_bard_assassin_human-dancer-2.png` | `human-dancer-2.png` |
| `human-dancer-spear-2` | — | Human Dancer Spear 2 | Polearm | Bard, Warrior | `polearm_bard_warrior_human-dancer-spear-2.png` | `human-dancer-spear-2.png` |
| `human-dancer-spear` | — | Human Dancer Spear | Polearm | Bard, Warrior | `polearm_bard_warrior_human-dancer-spear.png` | `human-dancer-spear.png` |
| `human-dancer` | — | Human Dancer | Bladed | Bard, Assassin | `bladed_bard_assassin_human-dancer.png` | `human-dancer.png` |
| `human-druid-2` | — | Human Druid 2 | Magic | Primalist, Healer | `magic_primalist_healer_human-druid-2.png` | `human-druid-2.png` |
| `human-druid-crystal-2` | — | Human Druid Crystal 2 | Magic | Primalist, Healer | `magic_primalist_healer_human-druid-crystal-2.png` | `human-druid-crystal-2.png` |
| `human-druid-crystal` | — | Human Druid Crystal | Magic | Primalist, Healer | `magic_primalist_healer_human-druid-crystal.png` | `human-druid-crystal.png` |
| `human-druid-leaf` | — | Human Druid Leaf | Magic | Primalist, Healer | `magic_primalist_healer_human-druid-leaf.png` | `human-druid-leaf.png` |
| `human-druid` | — | Human Druid | Magic | Primalist, Healer | `magic_primalist_healer_human-druid.png` | `human-druid.png` |
| `human-duelist-rapier-2` | — | Human Duelist Rapier 2 | Bladed | Warrior, Trickster | `bladed_warrior_trickster_human-duelist-rapier-2.png` | `human-duelist-rapier-2.png` |
| `human-duelist-rapier` | — | Human Duelist Rapier | Bladed | Warrior, Trickster | `bladed_warrior_trickster_human-duelist-rapier.png` | `human-duelist-rapier.png` |
| `human-engineer-crystal` | — | Human Engineer Crystal | Magic | Engineer, Sorcerer | `magic_engineer_sorcerer_human-engineer-crystal.png` | `human-engineer-crystal.png` |
| `human-engineer-hammer` | — | Human Engineer Hammer | Blunt | Engineer | `blunt_engineer_human-engineer-hammer.png` | `human-engineer-hammer.png` |
| `human-engineer-hook` | — | Human Engineer Hook | Thrown | Engineer, Trickster | `thrown_engineer_trickster_human-engineer-hook.png` | `human-engineer-hook.png` |
| `human-engineer` | — | Human Engineer | Blunt | Engineer | `blunt_engineer_human-engineer.png` | `human-engineer.png` |
| `human-guard-frost` | — | Human Guard Frost | Polearm | Sorcerer, Warrior | `polearm_sorcerer_warrior_human-guard-frost.png` | `human-guard-frost.png` |
| `human-guard-stone` | — | Human Guard Stone | Bladed | Guardian, Sorcerer | `bladed_guardian_sorcerer_human-guard-stone.png` | `human-guard-stone.png` |
| `human-guard` | — | Human Guard | Polearm | Guardian, Bodyguard | `polearm_guardian_bodyguard_human-guard.png` | `human-guard.png` |
| `human-knight-2` | — | Human Knight 2 | Bladed | Warrior, Guardian | `bladed_warrior_guardian_human-knight-2.png` | `human-knight-2.png` |
| `human-knight-banner` | — | Human Knight Banner | Blunt | Leader, Guardian | `blunt_leader_guardian_human-knight-banner.png` | `human-knight-banner.png` |
| `human-knight-hammer-2` | — | Human Knight Hammer 2 | Blunt | Guardian, Warrior | `blunt_guardian_warrior_human-knight-hammer-2.png` | `human-knight-hammer-2.png` |
| `human-knight-hammer` | — | Human Knight Hammer | Blunt | Guardian, Warrior | `blunt_guardian_warrior_human-knight-hammer.png` | `human-knight-hammer.png` |
| `human-knight-shield` | — | Human Knight Shield | Blunt | Guardian, Bodyguard | `blunt_guardian_bodyguard_human-knight-shield.png` | `human-knight-shield.png` |
| `human-knight` | — | Human Knight | Bladed | Warrior, Guardian | `bladed_warrior_guardian_human-knight.png` | `human-knight.png` |
| `human-mage-crystal-2` | — | Human Mage Crystal 2 | Magic | Sorcerer | `magic_sorcerer_human-mage-crystal-2.png` | `human-mage-crystal-2.png` |
| `human-mage-crystal-3` | — | Human Mage Crystal 3 | Magic | Sorcerer | `magic_sorcerer_human-mage-crystal-3.png` | `human-mage-crystal-3.png` |
| `human-mage-crystal` | — | Human Mage Crystal | Magic | Sorcerer | `magic_sorcerer_human-mage-crystal.png` | `human-mage-crystal.png` |
| `human-mage-fire` | — | Human Mage Fire | Magic | Sorcerer | `magic_sorcerer_human-mage-fire.png` | `human-mage-fire.png` |
| `human-mage-frost-2` | — | Human Mage Frost 2 | Magic | Sorcerer | `magic_sorcerer_human-mage-frost-2.png` | `human-mage-frost-2.png` |
| `human-mage-frost-3` | — | Human Mage Frost 3 | Magic | Sorcerer | `magic_sorcerer_human-mage-frost-3.png` | `human-mage-frost-3.png` |
| `human-mage-frost` | — | Human Mage Frost | Magic | Sorcerer | `magic_sorcerer_human-mage-frost.png` | `human-mage-frost.png` |
| `human-mage-stone` | — | Human Mage Stone | Magic | Sorcerer | `magic_sorcerer_human-mage-stone.png` | `human-mage-stone.png` |
| `human-mage-water` | — | Human Mage Water | Magic | Sorcerer, Leader | `magic_sorcerer_leader_human-mage-water.png` | `human-mage-water.png` |
| `human-mage` | — | Human Mage | Magic | Sorcerer, Leader | `magic_sorcerer_leader_human-mage.png` | `human-mage.png` |
| `human-monk-hammer` | — | Human Monk Hammer | Blunt | Warrior, Guardian | `blunt_warrior_guardian_human-monk-hammer.png` | `human-monk-hammer.png` |
| `human-monk-potion` | — | Human Monk Potion | Blunt | Healer, Alchemist | `blunt_healer_alchemist_human-monk-potion.png` | `human-monk-potion.png` |
| `human-monk-shadow` | — | Human Monk Shadow | Unarmed | Assassin, Warrior | `unarmed_assassin_warrior_human-monk-shadow.png` | `human-monk-shadow.png` |
| `human-monk` | — | Human Monk | Unarmed | Warrior | `unarmed_warrior_human-monk.png` | `human-monk.png` |
| `human-necromancer-skull-2` | — | Human Necromancer Skull 2 | Bladed | Black Mage, Warrior | `bladed_black-mage_warrior_human-necromancer-skull-2.png` | `human-necromancer-skull-2.png` |
| `human-necromancer-skull-3` | — | Human Necromancer Skull 3 | Bladed | Black Mage, Warrior | `bladed_black-mage_warrior_human-necromancer-skull-3.png` | `human-necromancer-skull-3.png` |
| `human-necromancer-skull` | — | Human Necromancer Skull | Magic | Black Mage, Summoner | `magic_black-mage_summoner_human-necromancer-skull.png` | `human-necromancer-skull.png` |
| `human-paladin-crystal` | — | Human Paladin Crystal | Bladed | Primalist, Guardian | `bladed_primalist_guardian_human-paladin-crystal.png` | `human-paladin-crystal.png` |
| `human-priest-2` | — | Human Priest 2 | Magic | Healer, Leader | `magic_healer_leader_human-priest-2.png` | `human-priest-2.png` |
| `human-priest-bell` | — | Human Priest Bell | Blunt | Healer, Bard | `blunt_healer_bard_human-priest-bell.png` | `human-priest-bell.png` |
| `human-priest-censer-2` | — | Human Priest Censer 2 | Blunt | Healer, Primalist | `blunt_healer_primalist_human-priest-censer-2.png` | `human-priest-censer-2.png` |
| `human-priest-censer` | — | Human Priest Censer | Bladed | Healer, Assassin | `bladed_healer_assassin_human-priest-censer.png` | `human-priest-censer.png` |
| `human-priest-moon-2` | — | Human Priest Moon 2 | Magic | Healer, Sorcerer | `magic_healer_sorcerer_human-priest-moon-2.png` | `human-priest-moon-2.png` |
| `human-priest-moon-3` | — | Human Priest Moon 3 | Magic | Healer, Sorcerer | `magic_healer_sorcerer_human-priest-moon-3.png` | `human-priest-moon-3.png` |
| `human-priest-moon-4` | — | Human Priest Moon 4 | Magic | Healer, Sorcerer | `magic_healer_sorcerer_human-priest-moon-4.png` | `human-priest-moon-4.png` |
| `human-priest-moon` | — | Human Priest Moon | Magic | Healer, Sorcerer | `magic_healer_sorcerer_human-priest-moon.png` | `human-priest-moon.png` |
| `human-priest-sun` | — | Human Priest Sun | Ranged | Healer, Primalist | `ranged_healer_primalist_human-priest-sun.png` | `human-priest-sun.png` |
| `human-priest` | — | Human Priest | Magic | Healer, Leader | `magic_healer_leader_human-priest.png` | `human-priest.png` |
| `human-ranger-harpoon` | — | Human Ranger Harpoon | Polearm | Hunter | `polearm_hunter_human-ranger-harpoon.png` | `human-ranger-harpoon.png` |
| `human-rogue-potion-2` | — | Human Rogue Potion 2 | Bladed | Alchemist, Assassin | `bladed_alchemist_assassin_human-rogue-potion-2.png` | `human-rogue-potion-2.png` |
| `human-rogue-potion-3` | — | Human Rogue Potion 3 | Bladed | Alchemist, Assassin | `bladed_alchemist_assassin_human-rogue-potion-3.png` | `human-rogue-potion-3.png` |
| `human-rogue-potion-4` | — | Human Rogue Potion 4 | Bladed | Alchemist, Assassin | `bladed_alchemist_assassin_human-rogue-potion-4.png` | `human-rogue-potion-4.png` |
| `human-rogue-potion` | — | Human Rogue Potion | Bladed | Alchemist, Assassin | `bladed_alchemist_assassin_human-rogue-potion.png` | `human-rogue-potion.png` |
| `human-rogue-shadow-2` | — | Human Rogue Shadow 2 | Bladed | Assassin, Black Mage | `bladed_assassin_black-mage_human-rogue-shadow-2.png` | `human-rogue-shadow-2.png` |
| `human-rogue-shadow-3` | — | Human Rogue Shadow 3 | Bladed | Assassin, Black Mage | `bladed_assassin_black-mage_human-rogue-shadow-3.png` | `human-rogue-shadow-3.png` |
| `human-rogue-shadow` | — | Human Rogue Shadow | Bladed | Assassin, Black Mage | `bladed_assassin_black-mage_human-rogue-shadow.png` | `human-rogue-shadow.png` |
| `human-rogue` | — | Human Rogue | Bladed | Assassin | `bladed_assassin_human-rogue.png` | `human-rogue.png` |
| `human-scout-lantern-2` | — | Human Scout Lantern 2 | Bladed | Hunter, Healer | `bladed_hunter_healer_human-scout-lantern-2.png` | `human-scout-lantern-2.png` |
| `human-scout-lantern` | — | Human Scout Lantern | Bladed | Hunter, Healer | `bladed_hunter_healer_human-scout-lantern.png` | `human-scout-lantern.png` |
| `human-scout-sling` | — | Human Scout Sling | Ranged | Trickster, Hunter | `ranged_trickster_hunter_human-scout-sling.png` | `human-scout-sling.png` |
| `human-scout` | — | Human Scout | Ranged | Hunter, Trickster | `ranged_hunter_trickster_human-scout.png` | `human-scout.png` |
| `human-shaman-2` | — | Human Shaman 2 | Magic | Primalist, Summoner | `magic_primalist_summoner_human-shaman-2.png` | `human-shaman-2.png` |
| `human-shaman-spear` | — | Human Shaman Spear | Polearm | Primalist, Healer | `polearm_primalist_healer_human-shaman-spear.png` | `human-shaman-spear.png` |
| `human-shaman` | — | Human Shaman | Magic | Primalist, Summoner | `magic_primalist_summoner_human-shaman.png` | `human-shaman.png` |
| `human-smith-potion` | — | Human Smith Potion | Blunt | Alchemist, Warrior | `blunt_alchemist_warrior_human-smith-potion.png` | `human-smith-potion.png` |
| `human-smith` | — | Human Smith | Blunt | Engineer, Warrior | `blunt_engineer_warrior_human-smith.png` | `human-smith.png` |
| `human-spellblade-2` | — | Human Spellblade 2 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-spellblade-2.png` | `human-spellblade-2.png` |
| `human-spellblade-3` | — | Human Spellblade 3 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-spellblade-3.png` | `human-spellblade-3.png` |
| `human-spellblade-4` | — | Human Spellblade 4 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-spellblade-4.png` | `human-spellblade-4.png` |
| `human-spellblade-5` | — | Human Spellblade 5 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-spellblade-5.png` | `human-spellblade-5.png` |
| `human-spellblade-6` | — | Human Spellblade 6 | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-spellblade-6.png` | `human-spellblade-6.png` |
| `human-spellblade` | — | Human Spellblade | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_human-spellblade.png` | `human-spellblade.png` |
| `human-witch-lantern` | — | Human Witch Lantern | Magic | Healer, Alchemist | `magic_healer_alchemist_human-witch-lantern.png` | `human-witch-lantern.png` |
| `human-witch-shadow` | — | Human Witch Shadow | Magic | Black Mage, Sorcerer | `magic_black-mage_sorcerer_human-witch-shadow.png` | `human-witch-shadow.png` |
| `human-wizard-2` | — | Human Wizard 2 | Magic | Sorcerer | `magic_sorcerer_human-wizard-2.png` | `human-wizard-2.png` |
| `human-wizard-potion` | — | Human Wizard Potion | Magic | Sorcerer, Alchemist | `magic_sorcerer_alchemist_human-wizard-potion.png` | `human-wizard-potion.png` |
| `human-wizard` | — | Human Wizard | Magic | Sorcerer | `magic_sorcerer_human-wizard.png` | `human-wizard.png` |
| `jelly-healer-2` | — | Jelly Healer 2 | Aura | Healer, Primalist | `aura_healer_primalist_jelly-healer-2.png` | `jelly-healer-2.png` |
| `jelly-healer` | — | Jelly Healer | Aura | Healer, Primalist | `aura_healer_primalist_jelly-healer.png` | `jelly-healer.png` |
| `lizardfolk-rogue-fire` | — | Lizardfolk Rogue Fire | Bladed | Assassin, Hunter | `bladed_assassin_hunter_lizardfolk-rogue-fire.png` | `lizardfolk-rogue-fire.png` |
| `lizardfolk-rogue` | — | Lizardfolk Rogue | Bladed | Assassin, Hunter | `bladed_assassin_hunter_lizardfolk-rogue.png` | `lizardfolk-rogue.png` |
| `mushroom-druid` | — | Mushroom Druid | Magic | Primalist, Healer | `magic_primalist_healer_mushroom-druid.png` | `mushroom-druid.png` |
| `nautilus` | — | Nautilus | Lashing | Sorcerer, Healer | `lashing_sorcerer_healer_nautilus.png` | `nautilus.png` |
| `octopus-alchemist` | — | Octopus Alchemist | Thrown | Alchemist, Trickster | `thrown_alchemist_trickster_octopus-alchemist.png` | `octopus-alchemist.png` |
| `orc-barbarian-totem` | — | Orc Barbarian Totem | Bladed | Warrior, Primalist | `bladed_warrior_primalist_orc-barbarian-totem.png` | `orc-barbarian-totem.png` |
| `owl-wizard` | — | Owl Wizard | Magic | Sorcerer, Flying | `magic_sorcerer_flying_owl-wizard.png` | `owl-wizard.png` |
| `pillbug` | — | Pillbug | Feral | Guardian, Bodyguard | `feral_guardian_bodyguard_pillbug.png` | `pillbug.png` |
| `ram-paladin-sun` | — | Ram Paladin Sun | Feral | Primalist, Leader | `feral_primalist_leader_ram-paladin-sun.png` | `ram-paladin-sun.png` |
| `ray-mage-crystal` | — | Ray Mage Crystal | Magic | Flying, Sorcerer | `magic_flying_sorcerer_ray-mage-crystal.png` | `ray-mage-crystal.png` |
| `ray` | — | Ray | Feral | Flying, Hunter | `feral_flying_hunter_ray.png` | `ray.png` |
| `salamander-rogue` | — | Salamander Rogue | Feral | Trickster, Assassin | `feral_trickster_assassin_salamander-rogue.png` | `salamander-rogue.png` |
| `scorpion` | — | Scorpion | Lashing | Assassin, Alchemist | `lashing_assassin_alchemist_scorpion.png` | `scorpion.png` |
| `sea-slug` | — | Sea Slug | Aura | Alchemist, Healer | `aura_alchemist_healer_sea-slug.png` | `sea-slug.png` |
| `snail-cleric-crystal` | — | Snail Cleric Crystal | Magic | Sorcerer, Healer | `magic_sorcerer_healer_snail-cleric-crystal.png` | `snail-cleric-crystal.png` |
| `snail-cleric-lantern-2` | — | Snail Cleric Lantern 2 | Magic | Healer, Primalist | `magic_healer_primalist_snail-cleric-lantern-2.png` | `snail-cleric-lantern-2.png` |
| `snail-cleric-lantern` | — | Snail Cleric Lantern | Magic | Healer, Primalist | `magic_healer_primalist_snail-cleric-lantern.png` | `snail-cleric-lantern.png` |
| `spider-mage-crystal` | — | Spider Mage Crystal | Magic | Sorcerer, Trickster | `magic_sorcerer_trickster_spider-mage-crystal.png` | `spider-mage-crystal.png` |
| `spider-rogue` | — | Spider Rogue | Feral | Trickster, Assassin | `feral_trickster_assassin_spider-rogue.png` | `spider-rogue.png` |
| `starfish-mage-crystal` | — | Starfish Mage Crystal | Magic | Sorcerer, Healer | `magic_sorcerer_healer_starfish-mage-crystal.png` | `starfish-mage-crystal.png` |
| `toad-druid` | — | Toad Druid | Breath | Alchemist, Primalist | `breath_alchemist_primalist_toad-druid.png` | `toad-druid.png` |
| `turtle-monk-2` | — | Turtle Monk 2 | Blunt | Healer, Guardian | `blunt_healer_guardian_turtle-monk-2.png` | `turtle-monk-2.png` |
| `turtle-monk` | — | Turtle Monk | Blunt | Healer, Guardian | `blunt_healer_guardian_turtle-monk.png` | `turtle-monk.png` |
| `wolf-ranger` | — | Wolf Ranger | Feral | Hunter, Bodyguard | `feral_hunter_bodyguard_wolf-ranger.png` | `wolf-ranger.png` |
| `worm-engineer` | — | Worm Engineer | Feral | Engineer, Hunter | `feral_engineer_hunter_worm-engineer.png` | `worm-engineer.png` |
