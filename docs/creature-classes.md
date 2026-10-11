# Creature Classes

Attack type and professions for every creature that has art, following [class_system.md](class_system.md) v1.1 (assessed 2026-10-09 from each creature's art and, for the first 54, its earlier classes in [creatures.md](creatures.md)). Each creature has one attack type and one or two professions; the first profession is the main one.

The table also drives the art pipeline: `tools/art/clean_creatures.py` reads each row's **Source** in `images/kin/` and writes the cleaned copy to `art/creatures/` under the **Art file** name.

**Art file names:** `fits_{species}[-{n}].png` or `does-not-fit_{species}[-{n}].png`. These prefixes record a visual style review against the chunky, friendly low-poly art generated in the chat. Only the species and a two-digit counter for repeated species remain in the image filename; unit names, attack types and professions stay in this table. The cleaning tool uses the Art file column, so it preserves these reviewed filenames.

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
| `anteater` | `anteater` | Giant Anteater | Feral | Hunter, Assassin | `does-not-fit_anteater.png` | `53_f.png` |
| `armadillo-knight-2` | — | Armadillo Knight 2 | Feral | Guardian, Bodyguard | `fits_armadillo-01.png` | `armadillo-knight-2.png` |
| `armadillo-knight` | — | Armadillo Knight | Blunt | Guardian, Bodyguard | `fits_armadillo-02.png` | `armadillo-knight.png` |
| `badger` | `badger` | Badger | Feral | Guardian, Warrior | `does-not-fit_badger.png` | `48_f.png` |
| `bat-rogue-shadow` | — | Bat Rogue Shadow | Bladed | Assassin, Flying | `fits_bat.png` | `bat-rogue-shadow.png` |
| `bear-brown` | `brown_bear` | Brown Bear | Feral | Warrior, Guardian | `does-not-fit_brown-bear.png` | `40_f.png` |
| `beetle-2` | — | Beetle 2 | Feral | Guardian, Warrior | `fits_beetle-01.png` | `beetle-2.png` |
| `beetle-knight-shield` | — | Beetle Knight Shield | Blunt | Guardian, Bodyguard | `fits_beetle-02.png` | `beetle-knight-shield.png` |
| `beetle-rhino` | `rhino_beetle` | Rhino Beetle | Feral | Guardian, Warrior | `does-not-fit_beetle-03.png` | `15_f.png` |
| `beetle` | — | Beetle | Feral | Guardian, Warrior | `fits_beetle-04.png` | `beetle.png` |
| `bison` | `bison` | Bison | Feral | Guardian, Warrior | `does-not-fit_bison.png` | `42_f.png` |
| `bloom-pod` | `bloom_pod` | Bloom Pod | Breath | Healer, Alchemist | `does-not-fit_bloom-pod.png` | `29_f.png` |
| `bloomspear` | `bloomspear` | Bloomspear | Polearm | Primalist, Hunter | `does-not-fit_bloomspear.png` | `7_f.png` |
| `boar-magma` | `magma_hog` | Magma Hog | Feral | Warrior, Sorcerer | `does-not-fit_boar-01.png` | `10_f.png` |
| `boar` | `boar` | Boar | Feral | Warrior | `does-not-fit_boar-02.png` | `47_f.png` |
| `bone-idol` | `bone_idol` | Bone Idol | Magic | Black Mage, Leader | `does-not-fit_bone-idol.png` | `4_f.png` |
| `caterpillar-bard` | — | Caterpillar Bard | Magic | Bard, Healer | `fits_caterpillar.png` | `caterpillar-bard.png` |
| `centaur-ranger` | — | Centaur Ranger | Polearm | Hunter, Warrior | `does-not-fit_centaur.png` | `centaur-ranger.png` |
| `centipede` | — | Centipede | Feral | Assassin, Alchemist | `does-not-fit_centipede.png` | `centipede.png` |
| `cinderhorn` | `cinderhorn` | Cinderhorn | Bladed | Warrior, Leader | `does-not-fit_cinderhorn.png` | `8_f.png` |
| `cobra` | — | Cobra | Breath | Alchemist, Assassin | `fits_cobra.png` | `cobra.png` |
| `crab-alchemist` | — | Crab Alchemist | Thrown | Alchemist | `fits_crab-01.png` | `crab-alchemist.png` |
| `crab-horseshoe` | — | Crab Horseshoe | Feral | Guardian, Warrior | `fits_horseshoe-crab.png` | `crab-horseshoe.png` |
| `crab-smith` | — | Crab Smith | Blunt | Engineer, Guardian | `fits_crab-02.png` | `crab-smith.png` |
| `crocodile-shaman` | — | Crocodile Shaman | Feral | Primalist, Warrior | `fits_crocodile.png` | `crocodile-shaman.png` |
| `dormouse` | `dormouse` | Dormouse | Feral | Healer, Trickster | `fits_dormouse.png` | `39_f.png` |
| `elephant` | `elephant` | Elephant | Feral | Guardian, Leader | `does-not-fit_elephant.png` | `44_f.png` |
| `ember-shard` | `ember_shard` | Ember Shard | Magic | Sorcerer | `does-not-fit_ember-shard.png` | `28_f.png` |
| `emberbulb` | `emberbulb` | Emberbulb | Magic | Sorcerer | `fits_emberbulb.png` | `20_f.png` |
| `firefly` | — | Firefly | Magic | Flying, Sorcerer | `fits_firefly.png` | `firefly.png` |
| `fox-bard` | — | Fox Bard | Feral | Bard, Trickster | `fits_fox-01.png` | `fox-bard.png` |
| `fox` | `fox` | Fox | Feral | Hunter, Trickster | `fits_fox-02.png` | `35_f.png` |
| `frog-bard-dagger` | — | Frog Bard Dagger | Bladed | Bard, Trickster | `fits_frog.png` | `frog-bard-dagger.png` |
| `gazelle-plume` | `plume_gazelle` | Plume Gazelle | Feral | Leader, Hunter | `does-not-fit_gazelle.png` | `22_f.png` |
| `halfling-alchemist` | — | Halfling Alchemist | Thrown | Alchemist | `fits_halfling.png` | `halfling-alchemist.png` |
| `hare` | `hare` | Hare | Feral | Trickster | `does-not-fit_hare.png` | `36_f.png` |
| `hedgehog` | `hedgehog` | Hedgehog | Feral | Guardian | `fits_hedgehog.png` | `38_f.png` |
| `human-alchemist-shield` | — | Human Alchemist Shield | Thrown | Alchemist, Guardian | `fits_human-01.png` | `human-alchemist-shield.png` |
| `human-alchemist` | — | Human Alchemist | Thrown | Alchemist, Hunter | `does-not-fit_human-02.png` | `human-alchemist.png` |
| `human-archer-crystal` | — | Human Archer Crystal | Ranged | Sorcerer, Hunter | `does-not-fit_human-03.png` | `human-archer-crystal.png` |
| `human-archer-frost` | — | Human Archer Frost | Ranged | Hunter, Sorcerer | `does-not-fit_human-04.png` | `human-archer-frost.png` |
| `human-archer-gear` | — | Human Archer Gear | Ranged | Hunter, Engineer | `does-not-fit_human-05.png` | `human-archer-gear.png` |
| `human-archer-sun` | — | Human Archer Sun | Ranged | Hunter, Healer | `does-not-fit_human-06.png` | `human-archer-sun.png` |
| `human-archer` | — | Human Archer | Ranged | Hunter | `does-not-fit_human-07.png` | `human-archer.png` |
| `human-barbarian-frost` | — | Human Barbarian Frost | Bladed | Warrior, Sorcerer | `does-not-fit_human-08.png` | `human-barbarian-frost.png` |
| `human-barbarian` | — | Human Barbarian | Bladed | Warrior | `does-not-fit_human-09.png` | `human-barbarian.png` |
| `human-bard-shield` | — | Human Bard Shield | Blunt | Bard, Guardian | `does-not-fit_human-10.png` | `human-bard-shield.png` |
| `human-bard` | — | Human Bard | Magic | Bard | `does-not-fit_human-11.png` | `human-bard.png` |
| `human-brawler-hook` | — | Human Brawler Hook | Blunt | Warrior, Bodyguard | `fits_human-12.png` | `human-brawler-hook.png` |
| `human-captain-banner` | — | Human Captain Banner | Polearm | Leader, Warrior | `does-not-fit_human-13.png` | `human-captain-banner.png` |
| `human-captain-pistol` | — | Human Captain Pistol | Ranged | Leader, Trickster | `does-not-fit_human-14.png` | `human-captain-pistol.png` |
| `human-captain-rapier` | — | Human Captain Rapier | Bladed | Leader, Trickster | `does-not-fit_human-15.png` | `human-captain-rapier.png` |
| `human-cleric-lantern` | — | Human Cleric Lantern | Magic | Healer, Alchemist | `fits_human-16.png` | `human-cleric-lantern.png` |
| `human-dancer-spear` | — | Human Dancer Spear | Polearm | Bard, Warrior | `does-not-fit_human-17.png` | `human-dancer-spear.png` |
| `human-dancer` | — | Human Dancer | Bladed | Bard, Assassin | `does-not-fit_human-18.png` | `human-dancer.png` |
| `human-druid-leaf` | — | Human Druid Leaf | Magic | Primalist, Healer | `does-not-fit_human-19.png` | `human-druid-leaf.png` |
| `human-druid` | — | Human Druid | Magic | Primalist, Healer | `does-not-fit_human-20.png` | `human-druid.png` |
| `human-duelist-rapier` | — | Human Duelist Rapier | Bladed | Warrior, Trickster | `does-not-fit_human-21.png` | `human-duelist-rapier.png` |
| `human-engineer-crystal` | — | Human Engineer Crystal | Magic | Engineer, Sorcerer | `does-not-fit_human-22.png` | `human-engineer-crystal.png` |
| `human-engineer-hook` | — | Human Engineer Hook | Thrown | Engineer, Trickster | `does-not-fit_human-23.png` | `human-engineer-hook.png` |
| `human-guard-frost` | — | Human Guard Frost | Polearm | Sorcerer, Warrior | `does-not-fit_human-24.png` | `human-guard-frost.png` |
| `human-guard-stone` | — | Human Guard Stone | Bladed | Guardian, Sorcerer | `fits_human-25.png` | `human-guard-stone.png` |
| `human-guard` | — | Human Guard | Polearm | Guardian, Bodyguard | `does-not-fit_human-26.png` | `human-guard.png` |
| `human-knight-2` | — | Human Knight 2 | Bladed | Warrior, Guardian | `does-not-fit_human-27.png` | `human-knight-2.png` |
| `human-knight-banner` | — | Human Knight Banner | Blunt | Leader, Guardian | `does-not-fit_human-28.png` | `human-knight-banner.png` |
| `human-knight-hammer` | — | Human Knight Hammer | Blunt | Guardian, Warrior | `fits_human-29.png` | `human-knight-hammer.png` |
| `human-knight-shield` | — | Human Knight Shield | Blunt | Guardian, Bodyguard | `does-not-fit_human-30.png` | `human-knight-shield.png` |
| `human-knight` | — | Human Knight | Bladed | Warrior, Guardian | `does-not-fit_human-31.png` | `human-knight.png` |
| `human-mage-crystal` | — | Human Mage Crystal | Magic | Sorcerer | `does-not-fit_human-32.png` | `human-mage-crystal.png` |
| `human-mage-fire` | — | Human Mage Fire | Magic | Sorcerer | `does-not-fit_human-33.png` | `human-mage-fire.png` |
| `human-mage-frost` | — | Human Mage Frost | Magic | Sorcerer | `does-not-fit_human-34.png` | `human-mage-frost.png` |
| `human-mage-stone` | — | Human Mage Stone | Magic | Sorcerer | `does-not-fit_human-35.png` | `human-mage-stone.png` |
| `human-mage-water` | — | Human Mage Water | Magic | Sorcerer, Leader | `fits_human-36.png` | `human-mage-water.png` |
| `human-monk-hammer` | — | Human Monk Hammer | Blunt | Warrior, Guardian | `does-not-fit_human-37.png` | `human-monk-hammer.png` |
| `human-necromancer-skull` | — | Human Necromancer Skull | Bladed | Black Mage, Warrior | `does-not-fit_human-38.png` | `human-necromancer-skull.png` |
| `human-priest-bell` | — | Human Priest Bell | Blunt | Healer, Bard | `fits_human-39.png` | `human-priest-bell.png` |
| `human-priest-moon` | — | Human Priest Moon | Magic | Healer, Sorcerer | `does-not-fit_human-40.png` | `human-priest-moon.png` |
| `human-priest-sun` | — | Human Priest Sun | Ranged | Healer, Primalist | `does-not-fit_human-41.png` | `human-priest-sun.png` |
| `human-priest` | — | Human Priest | Magic | Healer, Leader | `fits_human-42.png` | `human-priest.png` |
| `human-rogue-potion` | — | Human Rogue Potion | Bladed | Alchemist, Assassin | `does-not-fit_human-43.png` | `human-rogue-potion.png` |
| `human-rogue-shadow` | — | Human Rogue Shadow | Bladed | Assassin, Black Mage | `does-not-fit_human-44.png` | `human-rogue-shadow.png` |
| `human-rogue` | — | Human Rogue | Bladed | Assassin | `does-not-fit_human-45.png` | `human-rogue.png` |
| `human-scout-lantern` | — | Human Scout Lantern | Bladed | Hunter, Healer | `does-not-fit_human-46.png` | `human-scout-lantern.png` |
| `human-shaman` | — | Human Shaman | Magic | Primalist, Summoner | `does-not-fit_human-47.png` | `human-shaman.png` |
| `human-smith` | — | Human Smith | Blunt | Engineer, Warrior | `does-not-fit_human-48.png` | `human-smith.png` |
| `human-spellblade-2` | — | Human Spellblade 2 | Bladed | Warrior, Sorcerer | `does-not-fit_human-49.png` | `human-spellblade-2.png` |
| `human-spellblade-3` | — | Human Spellblade 3 | Bladed | Warrior, Sorcerer | `does-not-fit_human-50.png` | `human-spellblade-3.png` |
| `human-spellblade` | — | Human Spellblade | Bladed | Warrior, Sorcerer | `fits_human-51.png` | `human-spellblade.png` |
| `human-wizard` | — | Human Wizard | Magic | Sorcerer | `fits_human-52.png` | `human-wizard.png` |
| `jelly-gazer` | `gazer_jelly` | Gazer Jelly | Magic | Sorcerer, Black Mage | `does-not-fit_jellyfish-01.png` | `2_f.png` |
| `jelly-healer` | — | Jelly Healer | Aura | Healer, Primalist | `fits_jellyfish-02.png` | `jelly-healer.png` |
| `kangaroo` | `kangaroo` | Kangaroo | Unarmed | Warrior | `does-not-fit_kangaroo.png` | `52_f.png` |
| `leafwisp` | `leafwisp` | Leafwisp | Magic | Healer, Primalist | `does-not-fit_leafwisp.png` | `19_f.png` |
| `lizardfolk-rogue-fire` | — | Lizardfolk Rogue Fire | Bladed | Assassin, Hunter | `does-not-fit_lizardfolk-01.png` | `lizardfolk-rogue-fire.png` |
| `lynx` | `lynx` | Lynx | Feral | Hunter, Assassin | `does-not-fit_lynx.png` | `46_f.png` |
| `magma-crawler` | `magma_crawler` | Magma Crawler | Breath | Sorcerer, Warrior | `does-not-fit_magma-crawler.png` | `25_f.png` |
| `mannequin` | `mannequin` | Mannequin | Unarmed | Warrior, Guardian | `does-not-fit_mannequin.png` | `34_f.png` |
| `mantis-sickle` | `sickle_mantis` | Sickle Mantis | Feral | Assassin, Warrior | `does-not-fit_mantis.png` | `24_f.png` |
| `mole-giant` | `giant_mole` | Giant Mole | Feral | Warrior, Guardian | `does-not-fit_mole.png` | `33_f.png` |
| `moose` | `moose` | Moose | Feral | Guardian, Leader | `does-not-fit_moose.png` | `41_f.png` |
| `mossback` | `mossback` | Mossback | Feral | Guardian, Healer | `fits_mossback.png` | `18_f.png` |
| `moth-ash` | `ash_moth` | Ash Moth | Aura | Sorcerer, Flying | `does-not-fit_moth.png` | `32_f.png` |
| `mushroom-druid` | — | Mushroom Druid | Magic | Primalist, Healer | `fits_mushroom.png` | `mushroom-druid.png` |
| `nautilus` | — | Nautilus | Lashing | Sorcerer, Healer | `fits_nautilus.png` | `nautilus.png` |
| `octopus-alchemist` | — | Octopus Alchemist | Thrown | Alchemist, Trickster | `fits_octopus.png` | `octopus-alchemist.png` |
| `otter` | `otter` | Otter | Feral | Healer, Trickster | `does-not-fit_otter.png` | `37_f.png` |
| `owl-wizard` | — | Owl Wizard | Magic | Sorcerer, Flying | `fits_owl.png` | `owl-wizard.png` |
| `pillbug` | — | Pillbug | Feral | Guardian, Bodyguard | `fits_pillbug.png` | `pillbug.png` |
| `raccoon` | `raccoon` | Raccoon | Feral | Trickster, Hunter | `does-not-fit_raccoon.png` | `54_f.png` |
| `ram-paladin-sun` | — | Ram Paladin Sun | Feral | Primalist, Leader | `fits_ram.png` | `ram-paladin-sun.png` |
| `ray-mage-crystal` | — | Ray Mage Crystal | Magic | Flying, Sorcerer | `fits_ray-01.png` | `ray-mage-crystal.png` |
| `ray-shade` | `shade_ray` | Shade Ray | Lashing | Assassin, Flying | `does-not-fit_ray-02.png` | `17_f.png` |
| `ray` | — | Ray | Feral | Flying, Hunter | `fits_ray-03.png` | `ray.png` |
| `reef-warden` | `reef_warden` | Reef Warden | Polearm | Guardian, Leader | `does-not-fit_lizardfolk-02.png` | `9_f.png` |
| `rhino` | `rhino` | Rhino | Feral | Warrior | `does-not-fit_rhino.png` | `43_f.png` |
| `sailback` | `sailback` | Sailback | Feral | Sorcerer, Warrior | `does-not-fit_sailback.png` | `23_f.png` |
| `salamander-rogue` | — | Salamander Rogue | Feral | Trickster, Assassin | `fits_salamander.png` | `salamander-rogue.png` |
| `scorpion-gloom` | `gloom_scorpion` | Gloom Scorpion | Lashing | Assassin, Alchemist | `does-not-fit_scorpion-01.png` | `27_f.png` |
| `scorpion` | — | Scorpion | Lashing | Assassin, Alchemist | `fits_scorpion-02.png` | `scorpion.png` |
| `shark-fin-stalker` | `fin_stalker` | Fin Stalker | Feral | Hunter, Assassin | `does-not-fit_shark-01.png` | `16_f.png` |
| `shark-land` | `land_shark` | Land Shark | Feral | Warrior, Hunter | `does-not-fit_shark-02.png` | `12_f.png` |
| `slug-sea` | — | Slug Sea | Aura | Alchemist, Healer | `fits_sea-slug.png` | `slug-sea.png` |
| `snail-cleric-crystal` | — | Snail Cleric Crystal | Magic | Sorcerer, Healer | `fits_snail-01.png` | `snail-cleric-crystal.png` |
| `snail-cleric-lantern` | — | Snail Cleric Lantern | Magic | Healer, Primalist | `fits_snail-02.png` | `snail-cleric-lantern.png` |
| `snail-ember` | `ember_snail` | Ember Snail | Breath | Sorcerer, Guardian | `does-not-fit_snail-03.png` | `14_f.png` |
| `snail-prism` | `prism_snail` | Prism Snail | Magic | Healer, Sorcerer | `fits_snail-04.png` | `3_f.png` |
| `spider-mage-crystal` | — | Spider Mage Crystal | Magic | Sorcerer, Trickster | `fits_spider-01.png` | `spider-mage-crystal.png` |
| `spider-rogue` | — | Spider Rogue | Feral | Trickster, Assassin | `fits_spider-02.png` | `spider-rogue.png` |
| `spikeback` | `spikeback` | Spikeback | Feral | Warrior, Guardian | `does-not-fit_spikeback.png` | `1_f.png` |
| `stag-glade` | `glade_stag` | Glade Stag | Feral | Primalist, Healer | `does-not-fit_deer.png` | `26_f.png` |
| `starfish-mage-crystal` | — | Starfish Mage Crystal | Magic | Sorcerer, Healer | `fits_starfish.png` | `starfish-mage-crystal.png` |
| `thornfang` | `thornfang` | Thornfang | Feral | Hunter, Warrior | `does-not-fit_werewolf.png` | `6_f.png` |
| `tiger` | `tiger` | Tiger | Feral | Warrior, Hunter | `does-not-fit_tiger.png` | `50_f.png` |
| `toad-bog` | `bog_toad` | Bog Toad | Breath | Alchemist, Healer | `does-not-fit_toad-01.png` | `31_f.png` |
| `toad-boulder` | `boulder_toad` | Boulder Toad | Blunt | Guardian, Warrior | `does-not-fit_toad-02.png` | `5_f.png` |
| `toad-druid` | — | Toad Druid | Breath | Alchemist, Primalist | `fits_toad-03.png` | `toad-druid.png` |
| `turtle-hornshell` | `hornshell` | Hornshell Tortoise | Feral | Guardian | `does-not-fit_turtle-01.png` | `11_f.png` |
| `turtle-monk` | — | Turtle Monk | Blunt | Healer, Guardian | `fits_turtle-02.png` | `turtle-monk.png` |
| `turtle-stone` | `stone_turtle` | Stone Turtle | Feral | Guardian, Bodyguard | `fits_turtle-03.png` | `21_f.png` |
| `wolf-ranger` | — | Wolf Ranger | Feral | Hunter, Bodyguard | `fits_wolf-01.png` | `wolf-ranger.png` |
| `wolf` | `wolf` | Wolf | Feral | Hunter, Leader | `does-not-fit_wolf-02.png` | `45_f.png` |
| `wolverine` | `wolverine` | Wolverine | Feral | Warrior | `does-not-fit_wolverine.png` | `49_f.png` |
| `worm-engineer` | — | Worm Engineer | Feral | Engineer, Hunter | `fits_worm.png` | `worm-engineer.png` |
