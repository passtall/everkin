# Creature Classes

Attack type and professions for every creature that has art, following [class_system.md](class_system.md) v1.1 (assessed 2026-10-09 from each creature's art and, for the first 54, its earlier classes in [creatures.md](creatures.md)). Each creature has one attack type and one or two professions; the first profession is the main one.

The table also drives the art pipeline: `tools/art/clean_creatures.py` reads each row's **Source** in `images/kin/` and writes the cleaned copy to `art/creatures/` under the **Art file** name.

**Art file names:** `{attackType}_{profession1}_{profession2}_{unitName}.png`, or `{attackType}_{profession}_{unitName}.png` for one profession. Everything is lowercase; underscores separate the parts, and hyphens join words inside a part (`black-mage`, `glade-stag`). For creatures with a creature ID in creatures.md, the unit name is that ID with hyphens. The other unit names were chosen from what the art shows; these creatures have no stats yet.

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
| `astromancer` | — | Astromancer | Magic | Sorcerer, Leader | `magic_sorcerer_leader_astromancer.png` | `astromancer.png` |
| `brawler` | — | Brawler | Unarmed | Warrior | `unarmed_warrior_brawler.png` | `brawler.png` |
| `lantern-healer` | — | Lantern Healer | Magic | Healer, Alchemist | `magic_healer_alchemist_lantern-healer.png` | `lantern-healer.png` |
| `shield-knight` | — | Shield Knight | Blunt | Guardian, Bodyguard | `blunt_guardian_bodyguard_shield-knight.png` | `shield-knight.png` |
| `blade-dancer` | — | Blade Dancer | Bladed | Bard, Assassin | `bladed_bard_assassin_blade-dancer.png` | `blade-dancer.png` |
| `earth-sage` | — | Earth Sage | Magic | Sorcerer | `magic_sorcerer_earth-sage.png` | `earth-sage.png` |
| `dockhand` | — | Dockhand | Blunt | Warrior, Bodyguard | `blunt_warrior_bodyguard_dockhand.png` | `dockhand.png` |
| `banner-maiden` | — | Banner Maiden | Polearm | Leader, Warrior | `polearm_leader_warrior_banner-maiden.png` | `banner-maiden.png` |
| `potion-brewer` | — | Potion Brewer | Thrown | Alchemist | `thrown_alchemist_potion-brewer.png` | `potion-brewer.png` |
| `crystal-sorceress` | — | Crystal Sorceress | Magic | Sorcerer | `magic_sorcerer_crystal-sorceress.png` | `crystal-sorceress.png` |
| `spear-guard` | — | Spear Guard | Polearm | Guardian, Bodyguard | `polearm_guardian_bodyguard_spear-guard.png` | `spear-guard.png` |
| `duelist` | — | Duelist | Bladed | Warrior, Trickster | `bladed_warrior_trickster_duelist.png` | `duelist.png` |
| `fire-mage` | — | Fire Mage | Magic | Sorcerer | `magic_sorcerer_fire-mage.png` | `fire-mage.png` |
| `tribal-shaman` | — | Tribal Shaman | Magic | Primalist, Summoner | `magic_primalist_summoner_tribal-shaman.png` | `tribal-shaman.png` |
| `tinker` | — | Tinker | Blunt | Engineer | `blunt_engineer_tinker.png` | `tinker.png` |
| `hammer-knight` | — | Hammer Knight | Blunt | Guardian, Warrior | `blunt_guardian_warrior_hammer-knight.png` | `hammer-knight.png` |
| `moon-priestess` | — | Moon Priestess | Magic | Healer, Primalist | `magic_healer_primalist_moon-priestess.png` | `moon-priestess.png` |
| `pirate-captain` | — | Pirate Captain | Ranged | Leader, Trickster | `ranged_leader_trickster_pirate-captain.png` | `pirate-captain.png` |
| `shadow-rogue` | — | Shadow Rogue | Bladed | Assassin, Black Mage | `bladed_assassin_black-mage_shadow-rogue.png` | `shadow-rogue.png` |
| `frost-mage` | — | Frost Mage | Magic | Sorcerer | `magic_sorcerer_frost-mage.png` | `frost-mage.png` |
| `sun-dancer` | — | Sun Dancer | Polearm | Bard, Primalist | `polearm_bard_primalist_sun-dancer.png` | `sun-dancer.png` |
| `harpooner` | — | Harpooner | Polearm | Hunter | `polearm_hunter_harpooner.png` | `harpooner.png` |
| `moon-oracle` | — | Moon Oracle | Magic | Sorcerer, Healer | `magic_sorcerer_healer_moon-oracle.png` | `moon-oracle.png` |
| `blacksmith` | — | Blacksmith | Blunt | Engineer, Warrior | `blunt_engineer_warrior_blacksmith.png` | `blacksmith.png` |
| `frost-archer` | — | Frost Archer | Ranged | Hunter, Sorcerer | `ranged_hunter_sorcerer_frost-archer.png` | `frost-archer.png` |
| `artificer` | — | Artificer | Magic | Engineer, Sorcerer | `magic_engineer_sorcerer_artificer.png` | `artificer.png` |
| `fencer` | — | Fencer | Bladed | Warrior, Assassin | `bladed_warrior_assassin_fencer.png` | `fencer.png` |
| `grove-shaman` | — | Grove Shaman | Magic | Primalist, Healer | `magic_primalist_healer_grove-shaman.png` | `grove-shaman.png` |
| `buccaneer` | — | Buccaneer | Bladed | Trickster, Warrior | `bladed_trickster_warrior_buccaneer.png` | `buccaneer.png` |
| `nightblade` | — | Nightblade | Bladed | Assassin, Trickster | `bladed_assassin_trickster_nightblade.png` | `nightblade.png` |
| `sword-knight` | — | Sword Knight | Bladed | Warrior, Guardian | `bladed_warrior_guardian_sword-knight.png` | `sword-knight.png` |
| `lantern-cleric` | — | Lantern Cleric | Blunt | Healer, Primalist | `blunt_healer_primalist_lantern-cleric.png` | `lantern-cleric.png` |
| `wizard` | — | Wizard | Magic | Sorcerer | `magic_sorcerer_wizard.png` | `wizard.png` |
| `archer` | — | Archer | Ranged | Hunter | `ranged_hunter_archer.png` | `archer.png` |
| `rogue` | — | Rogue | Bladed | Assassin | `bladed_assassin_rogue.png` | `rogue.png` |
| `monk` | — | Monk | Unarmed | Warrior | `unarmed_warrior_monk.png` | `monk.png` |
| `minstrel` | — | Minstrel | Magic | Bard | `magic_bard_minstrel.png` | `minstrel.png` |
| `druid` | — | Druid | Magic | Primalist, Healer | `magic_primalist_healer_druid.png` | `druid.png` |
| `hex-witch` | — | Hex Witch | Magic | Black Mage, Sorcerer | `magic_black-mage_sorcerer_hex-witch.png` | `hex-witch.png` |
| `barbarian` | — | Barbarian | Bladed | Warrior | `bladed_warrior_barbarian.png` | `barbarian.png` |
| `paladin` | — | Paladin | Bladed | Primalist, Guardian | `bladed_primalist_guardian_paladin.png` | `paladin.png` |
| `leaf-archer` | — | Leaf Archer | Ranged | Hunter, Primalist | `ranged_hunter_primalist_leaf-archer.png` | `leaf-archer.png` |
| `poisoner` | — | Poisoner | Thrown | Alchemist, Assassin | `thrown_alchemist_assassin_poisoner.png` | `poisoner.png` |
| `battle-cleric` | — | Battle Cleric | Blunt | Healer, Guardian | `blunt_healer_guardian_battle-cleric.png` | `battle-cleric.png` |
| `shadow-monk` | — | Shadow Monk | Unarmed | Assassin, Warrior | `unarmed_assassin_warrior_shadow-monk.png` | `shadow-monk.png` |
| `rapier-bard` | — | Rapier Bard | Bladed | Bard, Warrior | `bladed_bard_warrior_rapier-bard.png` | `rapier-bard.png` |
| `totem-barbarian` | — | Totem Barbarian | Bladed | Warrior, Primalist | `bladed_warrior_primalist_totem-barbarian.png` | `totem-barbarian.png` |
| `star-sage` | — | Star Sage | Magic | Sorcerer, Healer | `magic_sorcerer_healer_star-sage.png` | `star-sage.png` |
| `spellblade` | — | Spellblade | Bladed | Sorcerer, Warrior | `bladed_sorcerer_warrior_spellblade.png` | `spellblade.png` |
| `crossbow-ranger` | — | Crossbow Ranger | Ranged | Hunter, Engineer | `ranged_hunter_engineer_crossbow-ranger.png` | `crossbow-ranger.png` |
| `battle-mage` | — | Battle Mage | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_battle-mage.png` | `battle-mage.png` |
| `potion-brawler` | — | Potion Brawler | Unarmed | Warrior, Alchemist | `unarmed_warrior_alchemist_potion-brawler.png` | `potion-brawler.png` |
| `crystal-barbarian` | — | Crystal Barbarian | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_crystal-barbarian.png` | `crystal-barbarian.png` |
| `arcane-duelist` | — | Arcane Duelist | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_arcane-duelist.png` | `arcane-duelist.png` |
| `gear-archer` | — | Gear Archer | Ranged | Hunter, Engineer | `ranged_hunter_engineer_gear-archer.png` | `gear-archer.png` |
| `moon-abbess` | — | Moon Abbess | Magic | Healer, Sorcerer | `magic_healer_sorcerer_moon-abbess.png` | `moon-abbess.png` |
| `dagger-bard` | — | Dagger Bard | Bladed | Bard, Trickster | `bladed_bard_trickster_dagger-bard.png` | `dagger-bard.png` |
| `lantern-templar` | — | Lantern Templar | Blunt | Guardian, Healer | `blunt_guardian_healer_lantern-templar.png` | `lantern-templar.png` |
| `rogue-alchemist` | — | Rogue Alchemist | Bladed | Alchemist, Assassin | `bladed_alchemist_assassin_rogue-alchemist.png` | `rogue-alchemist.png` |
| `grove-ranger` | — | Grove Ranger | Ranged | Hunter, Primalist | `ranged_hunter_primalist_grove-ranger.png` | `grove-ranger.png` |
| `spell-knight` | — | Spell Knight | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_spell-knight.png` | `spell-knight.png` |
| `censer-priestess` | — | Censer Priestess | Bladed | Healer, Assassin | `bladed_healer_assassin_censer-priestess.png` | `censer-priestess.png` |
| `abbot` | — | Abbot | Blunt | Healer, Primalist | `blunt_healer_primalist_abbot.png` | `abbot.png` |
| `war-bard` | — | War Bard | Blunt | Bard, Guardian | `blunt_bard_guardian_war-bard.png` | `war-bard.png` |
| `necromancer` | — | Necromancer | Magic | Black Mage, Summoner | `magic_black-mage_summoner_necromancer.png` | `necromancer.png` |
| `frost-barbarian` | — | Frost Barbarian | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_frost-barbarian.png` | `frost-barbarian.png` |
| `shield-alchemist` | — | Shield Alchemist | Thrown | Alchemist, Guardian | `thrown_alchemist_guardian_shield-alchemist.png` | `shield-alchemist.png` |
| `hexblade` | — | Hexblade | Bladed | Assassin, Black Mage | `bladed_assassin_black-mage_hexblade.png` | `hexblade.png` |
| `feather-archer` | — | Feather Archer | Ranged | Hunter, Primalist | `ranged_hunter_primalist_feather-archer.png` | `feather-archer.png` |
| `autumn-ranger` | — | Autumn Ranger | Ranged | Primalist, Hunter | `ranged_primalist_hunter_autumn-ranger.png` | `autumn-ranger.png` |
| `spellsword` | — | Spellsword | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_spellsword.png` | `spellsword.png` |
| `lantern-scout` | — | Lantern Scout | Bladed | Hunter, Healer | `bladed_hunter_healer_lantern-scout.png` | `lantern-scout.png` |
| `forest-archer` | — | Forest Archer | Ranged | Hunter | `ranged_hunter_forest-archer.png` | `forest-archer.png` |
| `storm-barbarian` | — | Storm Barbarian | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_storm-barbarian.png` | `storm-barbarian.png` |
| `elder-priest` | — | Elder Priest | Magic | Healer, Leader | `magic_healer_leader_elder-priest.png` | `elder-priest.png` |
| `shield-bard` | — | Shield Bard | Blunt | Bard, Guardian | `blunt_bard_guardian_shield-bard.png` | `shield-bard.png` |
| `potion-thief` | — | Potion Thief | Bladed | Alchemist, Trickster | `bladed_alchemist_trickster_potion-thief.png` | `potion-thief.png` |
| `deathblade` | — | Deathblade | Bladed | Black Mage, Warrior | `bladed_black-mage_warrior_deathblade.png` | `deathblade.png` |
| `sun-archer` | — | Sun Archer | Ranged | Hunter, Healer | `ranged_hunter_healer_sun-archer.png` | `sun-archer.png` |
| `sapling-druid` | — | Sapling Druid | Magic | Primalist, Healer | `magic_primalist_healer_sapling-druid.png` | `sapling-druid.png` |
| `arcane-knight` | — | Arcane Knight | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_arcane-knight.png` | `arcane-knight.png` |
| `lantern-crone` | — | Lantern Crone | Bladed | Healer, Trickster | `bladed_healer_trickster_lantern-crone.png` | `lantern-crone.png` |
| `forest-scout` | — | Forest Scout | Ranged | Hunter, Primalist | `ranged_hunter_primalist_forest-scout.png` | `forest-scout.png` |
| `red-barbarian` | — | Red Barbarian | Bladed | Warrior, Sorcerer | `bladed_warrior_sorcerer_red-barbarian.png` | `red-barbarian.png` |
| `elder-pilgrim` | — | Elder Pilgrim | Magic | Healer, Primalist | `magic_healer_primalist_elder-pilgrim.png` | `elder-pilgrim.png` |
| `lute-guard` | — | Lute Guard | Blunt | Bard, Bodyguard | `blunt_bard_bodyguard_lute-guard.png` | `lute-guard.png` |
| `grave-knight` | — | Grave Knight | Bladed | Black Mage, Summoner | `bladed_black-mage_summoner_grave-knight.png` | `grave-knight.png` |
| `vial-rogue` | — | Vial Rogue | Bladed | Alchemist, Assassin | `bladed_alchemist_assassin_vial-rogue.png` | `vial-rogue.png` |
| `crystal-druid` | — | Crystal Druid | Magic | Primalist, Sorcerer | `magic_primalist_sorcerer_crystal-druid.png` | `crystal-druid.png` |
| `star-archer` | — | Star Archer | Ranged | Sorcerer, Hunter | `ranged_sorcerer_hunter_star-archer.png` | `star-archer.png` |
| `dwarf-smith` | — | Dwarf Smith | Blunt | Engineer, Guardian | `blunt_engineer_guardian_dwarf-smith.png` | `dwarf-smith.png` |
| `elf-druid` | — | Elf Druid | Ranged | Primalist, Hunter | `ranged_primalist_hunter_elf-druid.png` | `elf-druid.png` |
| `apprentice-mage` | — | Apprentice Mage | Magic | Sorcerer, Alchemist | `magic_sorcerer_alchemist_apprentice-mage.png` | `apprentice-mage.png` |
| `orc-shaman` | — | Orc Shaman | Bladed | Primalist, Warrior | `bladed_primalist_warrior_orc-shaman.png` | `orc-shaman.png` |
| `halfling-bard` | — | Halfling Bard | Bladed | Bard, Trickster | `bladed_bard_trickster_halfling-bard.png` | `halfling-bard.png` |
| `lizard-rogue` | — | Lizard Rogue | Bladed | Assassin, Hunter | `bladed_assassin_hunter_lizard-rogue.png` | `lizard-rogue.png` |
| `gnome-artificer` | — | Gnome Artificer | Magic | Engineer, Sorcerer | `magic_engineer_sorcerer_gnome-artificer.png` | `gnome-artificer.png` |
| `cat-paladin` | — | Cat Paladin | Blunt | Guardian, Primalist | `blunt_guardian_primalist_cat-paladin.png` | `cat-paladin.png` |
| `centaur` | — | Centaur | Polearm | Hunter, Warrior | `polearm_hunter_warrior_centaur.png` | `centaur.png` |
| `tortoise-sage` | — | Tortoise Sage | Blunt | Healer, Primalist | `blunt_healer_primalist_tortoise-sage.png` | `tortoise-sage.png` |
| `turtle-monk` | — | Turtle Monk | Blunt | Guardian, Healer | `blunt_guardian_healer_turtle-monk.png` | `turtle-monk.png` |
| `beetle-knight` | — | Beetle Knight | Blunt | Guardian, Bodyguard | `blunt_guardian_bodyguard_beetle-knight.png` | `beetle-knight.png` |
| `frog-bard` | — | Frog Bard | Bladed | Bard, Trickster | `bladed_bard_trickster_frog-bard.png` | `frog-bard.png` |
| `owl-wizard` | — | Owl Wizard | Magic | Sorcerer, Flying | `magic_sorcerer_flying_owl-wizard.png` | `owl-wizard.png` |
| `salamander-duelist` | — | Salamander Duelist | Bladed | Assassin, Warrior | `bladed_assassin_warrior_salamander-duelist.png` | `salamander-duelist.png` |
| `crab-alchemist` | — | Crab Alchemist | Thrown | Alchemist | `thrown_alchemist_crab-alchemist.png` | `crab-alchemist.png` |
| `armadillo-knight` | — | Armadillo Knight | Blunt | Guardian, Bodyguard | `blunt_guardian_bodyguard_armadillo-knight.png` | `armadillo-knight.png` |
| `mushroom-druid` | — | Mushroom Druid | Magic | Primalist, Healer | `magic_primalist_healer_mushroom-druid.png` | `mushroom-druid.png` |
| `bat-assassin` | — | Bat Assassin | Bladed | Assassin, Flying | `bladed_assassin_flying_bat-assassin.png` | `bat-assassin.png` |
| `snail-cleric` | — | Snail Cleric | Magic | Healer, Guardian | `magic_healer_guardian_snail-cleric.png` | `snail-cleric.png` |
| `fox-bard` | — | Fox Bard | Feral | Bard, Trickster | `feral_bard_trickster_fox-bard.png` | `fox-bard.png` |
| `lance-beetle` | — | Lance Beetle | Feral | Warrior, Guardian | `feral_warrior_guardian_lance-beetle.png` | `lance-beetle.png` |
| `toadstool-toad` | — | Toadstool Toad | Breath | Alchemist, Primalist | `breath_alchemist_primalist_toadstool-toad.png` | `toadstool-toad.png` |
| `wolf-ranger` | — | Wolf Ranger | Feral | Hunter, Bodyguard | `feral_hunter_bodyguard_wolf-ranger.png` | `wolf-ranger.png` |
| `croc-shaman` | — | Croc Shaman | Feral | Primalist, Warrior | `feral_primalist_warrior_croc-shaman.png` | `croc-shaman.png` |
| `lantern-snail` | — | Lantern Snail | Magic | Healer, Primalist | `magic_healer_primalist_lantern-snail.png` | `lantern-snail.png` |
| `armadillo-squire` | — | Armadillo Squire | Feral | Guardian, Bodyguard | `feral_guardian_bodyguard_armadillo-squire.png` | `armadillo-squire.png` |
| `crystal-spider` | — | Crystal Spider | Magic | Sorcerer, Trickster | `magic_sorcerer_trickster_crystal-spider.png` | `crystal-spider.png` |
| `ember-newt` | — | Ember Newt | Feral | Trickster, Assassin | `feral_trickster_assassin_ember-newt.png` | `ember-newt.png` |
| `sun-ram` | — | Sun Ram | Feral | Primalist, Leader | `feral_primalist_leader_sun-ram.png` | `sun-ram.png` |
| `crystal-snail` | — | Crystal Snail | Magic | Sorcerer, Healer | `magic_sorcerer_healer_crystal-snail.png` | `crystal-snail.png` |
| `jade-beetle` | — | Jade Beetle | Feral | Guardian, Warrior | `feral_guardian_warrior_jade-beetle.png` | `jade-beetle.png` |
| `potion-octopus` | — | Potion Octopus | Thrown | Alchemist, Trickster | `thrown_alchemist_trickster_potion-octopus.png` | `potion-octopus.png` |
| `jeweled-cobra` | — | Jeweled Cobra | Breath | Alchemist, Assassin | `breath_alchemist_assassin_jeweled-cobra.png` | `jeweled-cobra.png` |
| `crab-smith` | — | Crab Smith | Blunt | Engineer, Guardian | `blunt_engineer_guardian_crab-smith.png` | `crab-smith.png` |
| `halo-jelly` | — | Halo Jelly | Aura | Healer, Primalist | `aura_healer_primalist_halo-jelly.png` | `halo-jelly.png` |
| `lute-caterpillar` | — | Lute Caterpillar | Magic | Bard, Healer | `magic_bard_healer_lute-caterpillar.png` | `lute-caterpillar.png` |
| `sky-manta` | — | Sky Manta | Magic | Flying, Sorcerer | `magic_flying_sorcerer_sky-manta.png` | `sky-manta.png` |
| `crimson-scorpion` | — | Crimson Scorpion | Lashing | Assassin, Alchemist | `lashing_assassin_alchemist_crimson-scorpion.png` | `crimson-scorpion.png` |
| `starfish-mage` | — | Starfish Mage | Magic | Sorcerer, Healer | `magic_sorcerer_healer_starfish-mage.png` | `starfish-mage.png` |
| `rock-squire` | — | Rock Squire | Bladed | Guardian, Sorcerer | `bladed_guardian_sorcerer_rock-squire.png` | `rock-squire.png` |
| `duchess` | — | Duchess | Bladed | Leader, Trickster | `bladed_leader_trickster_duchess.png` | `duchess.png` |
| `bell-friar` | — | Bell Friar | Blunt | Healer, Bard | `blunt_healer_bard_bell-friar.png` | `bell-friar.png` |
| `frost-lancer` | — | Frost Lancer | Polearm | Sorcerer, Warrior | `polearm_sorcerer_warrior_frost-lancer.png` | `frost-lancer.png` |
| `young-scout` | — | Young Scout | Ranged | Hunter, Trickster | `ranged_hunter_trickster_young-scout.png` | `young-scout.png` |
| `shield-matron` | — | Shield Matron | Blunt | Bodyguard, Guardian | `blunt_bodyguard_guardian_shield-matron.png` | `shield-matron.png` |
| `wind-dancer` | — | Wind Dancer | Bladed | Bard, Assassin | `bladed_bard_assassin_wind-dancer.png` | `wind-dancer.png` |
| `tide-mage` | — | Tide Mage | Magic | Sorcerer, Leader | `magic_sorcerer_leader_tide-mage.png` | `tide-mage.png` |
| `mirror-queen` | — | Mirror Queen | Magic | Sorcerer, Trickster | `magic_sorcerer_trickster_mirror-queen.png` | `mirror-queen.png` |
| `potion-monk` | — | Potion Monk | Blunt | Healer, Alchemist | `blunt_healer_alchemist_potion-monk.png` | `potion-monk.png` |
| `pillbug` | — | Pillbug | Feral | Guardian, Bodyguard | `feral_guardian_bodyguard_pillbug.png` | `pillbug.png` |
| `nautilus` | — | Nautilus | Lashing | Sorcerer, Healer | `lashing_sorcerer_healer_nautilus.png` | `nautilus.png` |
| `blood-centipede` | — | Blood Centipede | Feral | Assassin, Alchemist | `feral_assassin_alchemist_blood-centipede.png` | `blood-centipede.png` |
| `dream-jelly` | — | Dream Jelly | Aura | Healer, Sorcerer | `aura_healer_sorcerer_dream-jelly.png` | `dream-jelly.png` |
| `horn-crab` | — | Horn Crab | Feral | Guardian, Warrior | `feral_guardian_warrior_horn-crab.png` | `horn-crab.png` |
| `sea-slug` | — | Sea Slug | Aura | Alchemist, Healer | `aura_alchemist_healer_sea-slug.png` | `sea-slug.png` |
| `glide-ray` | — | Glide Ray | Feral | Flying, Hunter | `feral_flying_hunter_glide-ray.png` | `glide-ray.png` |
| `masked-spider` | — | Masked Spider | Feral | Trickster, Assassin | `feral_trickster_assassin_masked-spider.png` | `masked-spider.png` |
| `firefly` | — | Firefly | Magic | Flying, Sorcerer | `magic_flying_sorcerer_firefly.png` | `firefly.png` |
| `drill-worm` | — | Drill Worm | Feral | Engineer, Hunter | `feral_engineer_hunter_drill-worm.png` | `drill-worm.png` |
| `hermit` | — | Hermit | Polearm | Primalist, Healer | `polearm_primalist_healer_hermit.png` | `hermit.png` |
| `brewmistress` | — | Brewmistress | Blunt | Alchemist, Warrior | `blunt_alchemist_warrior_brewmistress.png` | `brewmistress.png` |
| `sling-courier` | — | Sling Courier | Ranged | Trickster, Hunter | `ranged_trickster_hunter_sling-courier.png` | `sling-courier.png` |
| `banner-knight` | — | Banner Knight | Blunt | Leader, Guardian | `blunt_leader_guardian_banner-knight.png` | `banner-knight.png` |
| `ice-wizard` | — | Ice Wizard | Magic | Sorcerer | `magic_sorcerer_ice-wizard.png` | `ice-wizard.png` |
| `spear-dancer` | — | Spear Dancer | Polearm | Bard, Warrior | `polearm_bard_warrior_spear-dancer.png` | `spear-dancer.png` |
| `scholar` | — | Scholar | Magic | Sorcerer | `magic_sorcerer_scholar.png` | `scholar.png` |
| `hedge-witch` | — | Hedge Witch | Magic | Healer, Alchemist | `magic_healer_alchemist_hedge-witch.png` | `hedge-witch.png` |
| `sun-priestess` | — | Sun Priestess | Ranged | Healer, Primalist | `ranged_healer_primalist_sun-priestess.png` | `sun-priestess.png` |
| `hammer-monk` | — | Hammer Monk | Blunt | Warrior, Guardian | `blunt_warrior_guardian_hammer-monk.png` | `hammer-monk.png` |
| `mechanic` | — | Mechanic | Blunt | Engineer | `blunt_engineer_mechanic.png` | `mechanic.png` |
| `crystal-apprentice` | — | Crystal Apprentice | Magic | Sorcerer | `magic_sorcerer_crystal-apprentice.png` | `crystal-apprentice.png` |
| `wanderer` | — | Wanderer | Thrown | Alchemist, Hunter | `thrown_alchemist_hunter_wanderer.png` | `wanderer.png` |
| `huntress` | — | Huntress | Ranged | Hunter, Assassin | `ranged_hunter_assassin_huntress.png` | `huntress.png` |
| `sprout-druid` | — | Sprout Druid | Magic | Primalist, Healer | `magic_primalist_healer_sprout-druid.png` | `sprout-druid.png` |
| `flower-druid` | — | Flower Druid | Magic | Primalist, Healer | `magic_primalist_healer_flower-druid.png` | `flower-druid.png` |
| `bowmaiden` | — | Bowmaiden | Ranged | Hunter | `ranged_hunter_bowmaiden.png` | `bowmaiden.png` |
| `shield-maiden` | — | Shield Maiden | Bladed | Guardian, Primalist | `bladed_guardian_primalist_shield-maiden.png` | `shield-maiden.png` |
| `hook-rogue` | — | Hook Rogue | Thrown | Engineer, Trickster | `thrown_engineer_trickster_hook-rogue.png` | `hook-rogue.png` |
| `grey-wizard` | — | Grey Wizard | Magic | Sorcerer, Leader | `magic_sorcerer_leader_grey-wizard.png` | `grey-wizard.png` |
