# Creature Classes

Attack type and professions for every creature that has art, following [class_system.md](class_system.md) v1.4 (assessed 2026-10-09 from each creature's art and, for the first 54, its earlier classes in [creatures.md](creatures.md)). Each creature has one attack type and one or two professions. Professions have no order of importance. On 2026-10-14 Black Mage was removed: necromantic units became Summoners, the rest Sorcerers, and Sorcerers with fire, frost, water, storm, stone or crystal magic became Elementalists. Later that day professions and attack types were merged (class_system.md v1.3): Bodyguard into Guardian, Assassin and Trickster into Rogue, Alchemist and Engineer into Tinker, Thrown into Ranged, Unarmed and Feral into Brawl, and Lashing renamed Whip. 15 units that had both halves of a merged pair have one profession for now; Andreas will revisit them. On 2026-10-17 Andreas reviewed every creature again under class_system.md v1.4, where a profession must be visible in the art, and set each creature's Cost.

Every creature with art in `art/creatures/` has a row. Rows marked **reviewed** in Card details were confirmed by Andreas in the [creature quiz](https://claude.ai/artifact/EbNvfcZZyU3CaBUq6QVvUW); **draft** rows carry the quiz's suggested defaults. Stats have no size tier any more (Andreas, 2026-10-09).

The table also drives the art pipeline: `tools/art/clean_creatures.py` reads each row's **Source** in `images/kin/` and writes the cleaned copy to `art/creatures/` under the **Art file** name. Art added straight to `art/creatures/` has no original there; its Source is —.

**Art file names:** `{species}-{nn}.png`, with a two-digit counter for every species. Unit names, attack types and professions stay in this table. The cleaning tool uses the Art file column.

Unit names follow the schema below. Creatures with a creature ID in creatures.md keep their ID in the ID column; their unit name is the ID with hyphens and the kind in front where it is not already first (`glade_stag` → `stag-glade`, `hornshell` → `turtle-hornshell`, `magma_hog` → `boar-magma`). They never take a counter.

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
| `abacus-strategist` | — | Tallymarch | Brawl | Leader | `abacus-01.png` | — |
| `accordion-warlord` | — | Wheezewarden | Brawl | Bard, Warrior | `accordion-01.png` | — |
| `anchor-sentinel` | — | Barnacle Bulwark | Blunt | Guardian | `anchor-01.png` | — |
| `ankylosaur-guardian-club` | — | Clubtail Grumbleback | Blunt | Guardian, Warrior | `ankylosaur-01.png` | — |
| `ant-engineer-gear` | — | Cogmarch Ant | Ranged | Tinker | `ant-01.png` | — |
| `armadillo-knight-2` | — | Armadillo Recruit | Blunt | Guardian | `armadillo-01.png` | `armadillo-knight-2.png` |
| `armadillo-knight` | — | Armadillo Knight | Blunt | Guardian | `armadillo-02.png` | `armadillo-knight.png` |
| `astrolabe-judge` | — | Starwright Judge | Aura | Sorcerer, Leader | `astrolabe-01.png` | — |
| `aurochs-bodyguard-plate` | — | Ironhide Aurochs | Brawl | Warrior | `aurochs-01.png` | — |
| `axolotl-mage-water` | — | Ripplequill | Magic | Elementalist, Sorcerer | `axolotl-01.png` | — |
| `ayeaye-hexer-candle` | — | Candlefinger | Magic | Sorcerer, Rogue | `ayeaye-01.png` | — |
| `badger-summoner-drum` | — | Drumbadger | Blunt | Summoner, Bard | `badger-01.png` | — |
| `ballast-bodyguard` | — | Sandbag Sentinel | Blunt | Guardian, Flying | `ballast-01.png` | — |
| `basilisk-sorcerer-gaze` | — | Stonegaze Basilisk | Aura | Warrior, Rogue | `basilisk-01.png` | — |
| `bassoon-hexer-requiem` | — | Dirgewind | Breath | Bard | `bassoon-01.png` | — |
| `bat-summoner-moon` | — | Moonwing | Magic | Flying, Summoner | `bat-01.png` | — |
| `bat-rogue-shadow` | — | Bat Rogue | Bladed | Rogue, Flying | `bat-02.png` | `bat-rogue-shadow.png` |
| `baton-marshal` | — | Marshal Tempo | Polearm | Leader, Bard | `baton-01.png` | — |
| `beacon-summoner` | — | Lighthouse Wakelamp | Aura | Flying, Summoner | `beacon-01.png` | — |
| `beaver-healer-pump` | — | Pumpwhisker | Ranged | Tinker, Healer | `beaver-01.png` | — |
| `bee-shaman-sun` | — | Sunhum | Magic | Primalist, Flying | `bee-01.png` | — |
| `beetle-2` | — | Beetle Recruit | Brawl | Guardian, Warrior | `beetle-01.png` | `beetle-2.png` |
| `beetle-knight-shield` | — | Beetle Guardian | Blunt | Guardian, Warrior | `beetle-02.png` | `beetle-knight-shield.png` |
| `beetle-engineer-seed` | — | Seedshot Scarab | Ranged | Tinker, Primalist | `beetle-04.png` | — |
| `behemoth-titan-iron` | — | Ironbound Behemoth | Brawl | Guardian, Elementalist | `behemoth-01.png` | — |
| `bell-guardian-clapper` | — | Clapperward | Blunt | Bard, Summoner | `bell-01.png` | — |
| `bellows-drake` | — | Puffing Forgedrake | Breath | Tinker, Warrior | `bellows-01.png` | — |
| `binturong-bard-vine` | — | Vinelilt | Whip | Bard, Primalist | `binturong-01.png` | — |
| `bloomspear` | `bloomspear` | Bloomspear | Polearm | Primalist, Warrior | `bloomspear-01.png` | `7_f.png` |
| `boar-magma` | `magma_hog` | Magma Hog | Brawl | Warrior | `boar-01.png` | `10_f.png` |
| `boar-guard-hex` | — | Hexbristle | Brawl | Guardian, Sorcerer | `boar-02.png` | — |
| `bombardierbeetle-alchemist-acid` | — | Acidpop | Ranged | Tinker, Rogue | `bombardierbeetle-01.png` | — |
| `book-summoner-fold` | — | Foldtome | Magic | Summoner, Primalist | `book-01.png` | — |
| `brazier-summoner-coal` | — | Coalwraith Brazier | Aura | Summoner, Elementalist | `brazier-01.png` | — |
| `buffalo-guard-root` | — | Rootyoke Buffalo | Whip | Guardian, Primalist | `buffalo-01.png` | — |
| `bugle-skyrider` | — | Skybrass | Breath | Bard, Flying | `bugle-01.png` | — |
| `candelabrum-warrior-wax` | — | Waxbrawler | Brawl | Warrior, Sorcerer | `candelabrum-01.png` | — |
| `cannon-marauder` | — | Powderbelch | Ranged | Warrior, Hunter | `cannon-01.png` | — |
| `capybara-guard-calm` | — | Calmwater Capybara | Aura | Healer, Summoner | `capybara-01.png` | — |
| `caracal-hunter-spear` | — | Tuftspear | Brawl | Sorcerer, Rogue | `caracal-01.png` | — |
| `carillon-summoner-chime` | — | Chimespire | Brawl | Bard, Guardian | `carillon-01.png` | — |
| `cassowary-captain-horn` | — | Crestcaller | Breath | Leader, Warrior | `cassowary-01.png` | — |
| `caterpillar-bard` | — | Caterpillar Bard | Breath | Bard | `caterpillar-01.png` | `caterpillar-bard.png` |
| `catfolk-spellblade-moon` | — | Moonclaw Spellblade | Bladed | Warrior, Rogue | `catfolk-01.png` | — |
| `centipede-assassin-sickle` | — | Sicklecoil | Bladed | Rogue, Warrior | `centipede-01.png` | — |
| `chalice-of-storms` | — | Stormgoblet | Magic | Elementalist, Flying | `chalice-01.png` | — |
| `chameleon-trickster-tongue` | — | Shiftscale | Whip | Rogue | `chameleon-01.png` | — |
| `chandelier-summoner-candle` | — | Candlecrown | Magic | Summoner, Flying | `chandelier-01.png` | — |
| `chest-bodyguard-lock` | — | Lockjaw Chest | Brawl | Guardian, Tinker | `chest-01.png` | — |
| `chinchilla-summoner-ocarina` | — | Ocarina Puff | Magic | Bard, Summoner | `chinchilla-01.png` | — |
| `cicada-bard-chime` | — | Chimewing Cicada | Magic | Sorcerer, Healer | `cicada-01.png` | — |
| `cinderhorn` | `cinderhorn` | Cinderhorn | Bladed | Warrior | `cinderhorn-01.png` | `8_f.png` |
| `coati-trickster-dice` | — | Diceroll Coati | Ranged | Rogue | `coati-01.png` | — |
| `cobra` | — | Hoodspitter | Breath | Rogue | `cobra-01.png` | `cobra.png` |
| `compass-hunter-needle` | — | Northneedle | Bladed | Hunter, Tinker | `compass-02.png` | — |
| `conchphone-bard-horn` | — | Conchhorn | Breath | Bard | `conchphone-01.png` | — |
| `corkscrew-stalker` | — | Twistcork | Bladed | Rogue | `corkscrew-01.png` | — |
| `crab-alchemist` | — | Crab Alchemist | Ranged | Tinker | `crab-01.png` | `crab-alchemist.png` |
| `crab-smith` | — | Crab Smith | Blunt | Tinker, Warrior | `crab-02.png` | `crab-smith.png` |
| `crab-guard-decoy` | — | Decoy Crab | Brawl | Guardian | `crab-03.png` | — |
| `crane-hoist-titan` | — | Hoist Titan | Brawl | Healer, Tinker | `crane-01.png` | — |
| `crane-summoner-feather` | — | Featherveil Crane | Magic | Summoner, Elementalist | `crane-02.png` | — |
| `cricket-bard-fiddle` | — | Fiddlewhit | Magic | Bard, Leader | `cricket-01.png` | — |
| `crocodile-shaman` | — | Crocodile Shaman | Brawl | Primalist, Warrior | `crocodile-01.png` | `crocodile-shaman.png` |
| `crow-necromancer-skull` | — | Skullcaw | Magic | Summoner, Flying | `crow-01.png` | — |
| `crucible-summoner` | — | Moltencup | Aura | Guardian | `crucible-01.png` | — |
| `cuttlefish-assassin-needle` | — | Needleglow | Bladed | Rogue | `cuttlefish-01.png` | — |
| `stag-glade` | `glade_stag` | Glade Stag | Brawl | Primalist | `deer-01.png` | `26_f.png` |
| `dormouse-summoner-bell` | — | Bellnap Dormouse | Magic | Summoner | `dormouse-01.png` | — |
| `dragon-colossus-ember` | — | Emberwyrm Colossus | Breath | Warrior, Flying | `dragon-01.png` | — |
| `dragon-elder-obsidian` | — | Obsidian Elder | Aura | Leader, Flying | `dragon-02.png` | — |
| `dragon-sovereign-tempest` | — | Tempest Sovereign | Breath | Leader, Flying | `dragon-03.png` | — |
| `dragon-titan-glacier` | — | Glacier Titan | Breath | Elementalist, Guardian | `dragon-04.png` | — |
| `dragonfly-scout-bow` | — | Reedarrow | Ranged | Flying, Hunter | `dragonfly-01.png` | — |
| `drake-titan-crystal` | — | Crystalback Drake | Brawl | Elementalist, Sorcerer | `drake-01.png` | — |
| `drawbridge-bodyguard-chain` | — | Chaingate | Brawl | Guardian | `drawbridge-01.png` | — |
| `drum-summoner-war` | — | Wardrum | Aura | Bard, Warrior | `drum-01.png` | — |
| `dugong-guard-crystal` | — | Crystalfin Dugong | Aura | Sorcerer, Elementalist | `dugong-02.png` | — |
| `elephant-summoner-idol` | — | Idolbearer | Brawl | Summoner, Primalist | `elephant-01.png` | — |
| `ferret-summoner-mask` | — | Maskwhisk | Brawl | Rogue, Summoner | `ferret-01.png` | — |
| `fiddlercrab-trickster-coin` | — | Coinclaw | Brawl | Rogue, Tinker | `fiddlercrab-01.png` | — |
| `firefly-summoner-lantern` | — | Lanternwing | Aura | Flying, Summoner | `firefly-01.png` | — |
| `firefly` | — | Firefly | Aura | Flying, Healer | `firefly-02.png` | `firefly.png` |
| `flyingfish-scout-dart` | — | Dartfin | Ranged | Flying, Rogue | `flyingfish-01.png` | — |
| `fossa-hunter-harpoon` | — | Harpoon Fossa | Polearm | Hunter, Rogue | `fossa-01.png` | — |
| `fox-bard` | — | Fox Bard | Brawl | Bard, Rogue | `fox-01.png` | `fox-bard.png` |
| `frigatebird-bard-throat` | — | Throatdrum | Breath | Bard, Flying | `frigatebird-01.png` | — |
| `frog-bard-dagger` | — | Frog Bard Dagger | Bladed | Bard, Rogue | `frog-01.png` | `frog-bard-dagger.png` |
| `furnace-titan` | — | Furnace Titan | Breath | Warrior, Guardian | `furnace-01.png` | — |
| `gargoyle-sentinel-stone` | — | Stonewatch Gargoyle | Polearm | Flying, Hunter | `gargoyle-01.png` | — |
| `gaur-bodyguard-armor` | — | Bolted Gaur | Brawl | Guardian, Warrior | `gaur-01.png` | — |
| `gecko-alchemist-resin` | — | Resinpaw | Ranged | Tinker, Rogue | `gecko-01.png` | — |
| `gecko-summoner-prism` | — | Prismtoe | Magic | Summoner, Sorcerer | `gecko-02.png` | — |
| `goat-shaman-totem` | — | Totemhorn | Brawl | Primalist, Healer | `goat-01.png` | — |
| `goblet-paladin` | — | Grailward | Aura | Healer, Warrior | `goblet-01.png` | — |
| `golem-colossus-furnace` | — | Furnaceheart Colossus | Brawl | Warrior, Guardian | `golem-01.png` | — |
| `golem-engineer-gear` | — | Gearwright Golem | Brawl | Tinker, Guardian | `golem-02.png` | — |
| `gong-hexer-dissonance` | — | Discord Gong | Aura | Bard, Summoner | `gong-01.png` | — |
| `gorilla-bodyguard-fists` | — | Ironfist Silverback | Brawl | Guardian, Warrior | `gorilla-01.png` | — |
| `griffin-knight-lance` | — | Lancewing Griffin | Polearm | Flying, Warrior | `griffin-01.png` | — |
| `harpsichord-bard-keys` | — | Keyclatter | Aura | Bard | `harpsichord-01.png` | — |
| `harpy-eagle-scout-claws` | — | Talonstrike | Brawl | Flying, Hunter | `harpy-eagle-01.png` | — |
| `hawk-scout-bow` | — | Skybow Hawk | Ranged | Hunter, Flying | `hawk-01.png` | — |
| `hermitcrab-sentinel-basalt` | — | Basaltshell | Brawl | Guardian | `hermitcrab-01.png` | — |
| `heron-engineer-wing` | — | Wingwright Heron | Ranged | Tinker, Flying | `heron-01.png` | — |
| `heron-hunter-harpoon` | — | Stillspear | Polearm | Flying, Warrior | `heron-02.png` | — |
| `hippo-healer-potion` | — | Tonic Hippo | Breath | Healer, Tinker | `hippo-01.png` | — |
| `hornbill-bard-drum` | — | Drumbill | Blunt | Bard, Flying | `hornbill-01.png` | — |
| `horseshoe-champion` | — | Lucky Ironshoe | Brawl | Warrior, Guardian | `horseshoe-01.png` | — |
| `horseshoecrab-sentinel-bronze` | — | Bronzeback Sentinel | Brawl | Guardian, Elementalist | `horseshoecrab-01.png` | — |
| `hourglass-hexer-sand` | — | Sandhex Glass | Aura | Sorcerer, Leader | `hourglass-01.png` | — |
| `howlermonkey-bard-roar` | — | Roarleaf | Breath | Bard, Leader | `howlermonkey-01.png` | — |
| `human-alchemist-shield` | — | Gerald Hollins | Ranged | Tinker, Guardian | `human-01.png` | `human-alchemist-shield.png` |
| `human-alchemist` | — | Maggie Wren | Ranged | Tinker, Rogue | `human-02.png` | `human-alchemist.png` |
| `human-archer-crystal` | — | Mira Castell | Ranged | Hunter, Sorcerer | `human-03.png` | `human-archer-crystal.png` |
| `human-archer-frost` | — | Sigrid Halvorsen | Ranged | Hunter, Elementalist | `human-04.png` | `human-archer-frost.png` |
| `human-archer-gear` | — | Otto Brenner | Ranged | Hunter, Tinker | `human-05.png` | `human-archer-gear.png` |
| `human-archer-sun` | — | Lucia Marin | Ranged | Hunter, Healer | `human-06.png` | `human-archer-sun.png` |
| `human-archer` | — | Tom Reed | Ranged | Hunter, Primalist | `human-07.png` | `human-archer.png` |
| `human-barbarian-frost` | — | Ragnhild Eriksen | Bladed | Warrior, Sorcerer | `human-08.png` | `human-barbarian-frost.png` |
| `human-barbarian` | — | Gunnar Holt | Bladed | Warrior | `human-09.png` | `human-barbarian.png` |
| `human-bard-shield` | — | Felix Lowell | Blunt | Bard, Warrior | `human-10.png` | `human-bard-shield.png` |
| `human-bard` | — | Lorenzo Varga | Magic | Bard, Leader | `human-11.png` | `human-bard.png` |
| `human-brawler-hook` | — | Jack Morrow | Blunt | Warrior, Hunter | `human-12.png` | `human-brawler-hook.png` |
| `human-captain-banner` | — | Helena Strand | Polearm | Leader, Warrior | `human-13.png` | `human-captain-banner.png` |
| `human-captain-pistol` | — | Carlos Fonseca | Ranged | Leader, Rogue | `human-14.png` | `human-captain-pistol.png` |
| `human-captain-rapier` | — | Beatrix Alvarez | Bladed | Sorcerer, Rogue | `human-15.png` | `human-captain-rapier.png` |
| `human-cleric-lantern` | — | Agnes Brook | Magic | Healer, Tinker | `human-16.png` | `human-cleric-lantern.png` |
| `human-dancer-spear` | — | Amara Okafor | Polearm | Warrior, Elementalist | `human-17.png` | `human-dancer-spear.png` |
| `human-dancer` | — | Nadia Petrova | Bladed | Rogue, Hunter | `human-18.png` | `human-dancer.png` |
| `human-druid-leaf` | — | Fiona Ashby | Magic | Primalist, Healer | `human-19.png` | `human-druid-leaf.png` |
| `human-druid` | — | Ivy Calloway | Magic | Primalist, Healer | `human-20.png` | `human-druid.png` |
| `human-duelist-rapier` | — | Camille Moreau | Bladed | Warrior, Rogue | `human-21.png` | `human-duelist-rapier.png` |
| `human-engineer-crystal` | — | Lukas Weiss | Magic | Tinker, Elementalist | `human-22.png` | `human-engineer-crystal.png` |
| `human-engineer-hook` | — | Max Hooper | Ranged | Rogue, Hunter | `human-23.png` | `human-engineer-hook.png` |
| `human-guard-frost` | — | Ingrid Lund | Polearm | Warrior, Sorcerer | `human-24.png` | `human-guard-frost.png` |
| `human-guard-stone` | — | Leo Stone | Bladed | Elementalist, Warrior | `human-25.png` | `human-guard-stone.png` |
| `human-guard` | — | Peter Holm | Polearm | Guardian, Warrior | `human-26.png` | `human-guard.png` |
| `human-knight-2` | — | Johanna Weber | Bladed | Warrior, Guardian | `human-27.png` | `human-knight-2.png` |
| `human-knight-banner` | — | Albrecht Lang | Blunt | Leader, Guardian | `human-28.png` | `human-knight-banner.png` |
| `human-knight-hammer` | — | Johanka Weber | Blunt | Guardian, Warrior | `human-29.png` | `human-knight-hammer.png` |
| `human-knight-shield` | — | Edwin Marsh | Blunt | Guardian | `human-30.png` | `human-knight-shield.png` |
| `human-knight` | — | William Hart | Bladed | Warrior, Leader | `human-31.png` | `human-knight.png` |
| `human-mage-crystal` | — | Kai Lindqvist | Magic | Elementalist, Sorcerer | `human-32.png` | `human-mage-crystal.png` |
| `human-mage-fire` | — | Carmen Costa | Magic | Elementalist, Rogue | `human-33.png` | `human-mage-fire.png` |
| `human-mage-frost` | — | Astor Nilsen | Magic | Elementalist, Sorcerer | `human-34.png` | `human-mage-frost.png` |
| `human-mage-stone` | — | Henrik Berg | Magic | Elementalist, Leader | `human-35.png` | `human-mage-stone.png` |
| `human-mage-water` | — | Mateo Santos | Magic | Elementalist, Hunter | `human-36.png` | `human-mage-water.png` |
| `human-monk-hammer` | — | Kenji Mori | Blunt | Warrior | `human-37.png` | `human-monk-hammer.png` |
| `human-necromancer-skull` | — | Viktor Graves | Bladed | Summoner, Warrior | `human-38.png` | `human-necromancer-skull.png` |
| `human-priest-bell` | — | Martin Becker | Blunt | Healer, Bard | `human-39.png` | `human-priest-bell.png` |
| `human-priest-moon` | — | Luna Ortiz | Magic | Healer, Sorcerer | `human-40.png` | `human-priest-moon.png` |
| `human-priest-sun` | — | Aurelia Day | Ranged | Healer, Hunter | `human-41.png` | `human-priest-sun.png` |
| `human-priest` | — | Thomas Abbott | Magic | Healer | `human-42.png` | `human-priest.png` |
| `human-rogue-potion` | — | Paul Fischer | Bladed | Tinker, Rogue | `human-43.png` | `human-rogue-potion.png` |
| `human-rogue-shadow` | — | Nika Volkova | Bladed | Rogue, Summoner | `human-44.png` | `human-rogue-shadow.png` |
| `human-rogue` | — | Daniel Hart | Bladed | Rogue | `human-45.png` | `human-rogue.png` |
| `human-scout-lantern` | — | Elsa Lindgren | Bladed | Rogue, Summoner | `human-46.png` | `human-scout-lantern.png` |
| `human-shaman` | — | Kofi Mensah | Magic | Primalist, Leader | `human-47.png` | `human-shaman.png` |
| `human-smith` | — | Hugo Schmidt | Blunt | Tinker, Warrior | `human-48.png` | `human-smith.png` |
| `human-spellblade-2` | — | Laura Cole | Bladed | Warrior, Sorcerer | `human-49.png` | `human-spellblade-2.png` |
| `human-spellblade` | — | Richard Novak | Bladed | Warrior, Sorcerer | `human-51.png` | `human-spellblade.png` |
| `human-wizard` | — | Arthur Pembroke | Magic | Sorcerer, Tinker | `human-52.png` | `human-wizard.png` |
| `hummingbird-duelist-rapier` | — | Rapierbeak | Bladed | Flying, Rogue | `hummingbird-01.png` | — |
| `hydra-colossus-trident` | — | Tridentmaw Hydra | Breath | Summoner | `hydra-01.png` | — |
| `hyena-engineer-saw` | — | Sawgrin | Ranged | Tinker, Hunter | `hyena-01.png` | — |
| `ibex-captain-standard` | — | Standardhorn | Brawl | Leader | `ibex-01.png` | — |
| `incense-censor` | — | Smokeswing | Aura | Healer, Summoner | `incense-censor-01.png` | — |
| `inkwell-trickster-quill` | — | Quillspatter | Ranged | Rogue, Leader | `inkwell-01.png` | — |
| `isopod-sentinel-iron` | — | Ironroll | Brawl | Guardian | `isopod-01.png` | — |
| `jackalope-hexer-curse` | — | Cursehorn | Aura | Summoner, Rogue | `jackalope-01.png` | — |
| `jelly-mage-storm` | — | Stormbell Jelly | Aura | Flying, Elementalist | `jelly-01.png` | — |
| `jelly-healer` | — | Mendbloom Jelly | Aura | Healer, Flying | `jellyfish-02.png` | `jelly-healer.png` |
| `jerboa-trickster-acorn` | — | Acornpop | Ranged | Rogue, Tinker | `jerboa-01.png` | — |
| `kestrel-hunter-lure` | — | Lurewing | Ranged | Hunter, Summoner | `kestrel-01.png` | — |
| `kettle-alchemist-siege` | — | Siegekettle | Breath | Warrior, Guardian | `kettle-01.png` | — |
| `kiln-behemoth` | — | Kiln Behemoth | Breath | Guardian | `kiln-01.png` | — |
| `kingfisher-captain-dagger` | — | Captain Daggerbill | Bladed | Rogue, Flying | `kingfisher-01.png` | — |
| `kinkajou-summoner-idol` | — | Idolpaw | Brawl | Summoner, Rogue | `kinkajou-01.png` | — |
| `kite-primalist-tail` | — | Tailwind Kite | Whip | Flying, Healer | `kite-01.png` | — |
| `komodo-warrior-cleaver` | — | Cleaver Komodo | Blunt | Warrior | `komodo-01.png` | — |
| `kraken-colossus-basalt` | — | Basalt Kraken | Whip | Guardian, Summoner | `kraken-01.png` | — |
| `lantern-healer-prism` | — | Prismlamp | Aura | Healer | `lantern-01.png` | — |
| `reef-warden` | `reef_warden` | Reef Warden | Polearm | Guardian, Warrior | `lizardfolk-02.png` | `9_f.png` |
| `lizardfolk-barbarian-axe` | — | Axejaw Lizardfolk | Bladed | Warrior | `lizardfolk-03.png` | — |
| `lizardfolk-ninja-kunai` | — | Kunai Lizardfolk | Ranged | Rogue | `lizardfolk-04.png` | — |
| `lobster-bard-castanet` | — | Castanet Lobster | Brawl | Bard | `lobster-01.png` | — |
| `lobster-engineer-rivet` | — | Rivetclaw | Ranged | Tinker | `lobster-02.png` | — |
| `lockbox-dreadnought` | — | Lockbox Dreadnought | Brawl | Guardian | `lockbox-01.png` | — |
| `loom-binder` | — | Threadbinder Loom | Whip | Tinker, Leader | `loom-01.png` | — |
| `lyrebird-bard-plume` | — | Plumesong | Breath | Bard, Flying | `lyrebird-01.png` | — |
| `mammoth-titan-tundra` | — | Tundra Titan | Brawl | Guardian, Elementalist | `mammoth-01.png` | — |
| `manatee-healer-shell` | — | Shellmend Manatee | Breath | Healer | `manatee-01.png` | — |
| `mandrake-primalist-root` | — | Screamroot | Whip | Primalist, Summoner | `mandrake-01.png` | — |
| `mandrill-brawler-gloves` | — | Glovegrin | Brawl | Warrior | `mandrill-01.png` | — |
| `mannequin` | `mannequin` | Stitchfist Mannequin | Brawl | Warrior, Guardian | `mannequin-01.png` | `34_f.png` |
| `manticore-assassin-barb` | — | Barbtail Manticore | Whip | Rogue, Flying | `manticore-01.png` | — |
| `mantis-engineer-blade` | — | Bladewright Mantis | Bladed | Rogue, Tinker | `mantis-01.png` | — |
| `marabou-healer-balm` | — | Balmbeak | Brawl | Tinker, Rogue | `marabou-01.png` | — |
| `maskrack-summoner-echo` | — | Echomask Rack | Aura | Summoner, Flying | `maskrack-01.png` | — |
| `mastiff-guard-shield` | — | Shieldjaw Mastiff | Brawl | Guardian, Hunter | `mastiff-01.png` | — |
| `meerkat-captain-spyglass` | — | Spyglass Meerkat | Ranged | Leader, Hunter | `meerkat-01.png` | — |
| `millstone-juggernaut` | — | Millstone Juggernaut | Blunt | Warrior, Guardian | `millstone-01.png` | — |
| `mirror-revenant` | — | Mirror Revenant | Magic | Sorcerer, Flying | `mirror-01.png` | — |
| `mole-engineer-gear` | — | Gearclaw Mole | Blunt | Tinker | `mole-01.png` | — |
| `mole-engineer-hexhammer` | — | Hexhammer Mole | Blunt | Tinker, Sorcerer | `mole-02.png` | — |
| `mongoose-assassin-sickle` | — | Sicklewhisk | Bladed | Rogue, Hunter | `mongoose-01.png` | — |
| `mongoose-hunter-hex` | — | Hexdart Mongoose | Ranged | Hunter | `mongoose-02.png` | — |
| `monkey-trickster-sling` | — | Slingtail | Ranged | Rogue, Hunter | `monkey-01.png` | — |
| `moray-trickster-hook` | — | Hookgrin Moray | Whip | Rogue | `moray-01.png` | — |
| `mortar-alchemist-bastion` | — | Bastion Mortar | Blunt | Warrior, Guardian | `mortar-01.png` | — |
| `moth-cleric-lantern` | — | Lanternmoth | Magic | Healer, Flying | `moth-02.png` | — |
| `mushroom-druid` | — | Sporedruid | Magic | Primalist, Healer | `mushroom-01.png` | `mushroom-druid.png` |
| `mushroom-sentinel-shield` | — | Capshield | Aura | Guardian, Primalist | `mushroom-03.png` | — |
| `musicbox-summoner-drum` | — | Windup Drummer | Brawl | Summoner, Guardian | `musicbox-01.png` | — |
| `muskox-bodyguard-buckler` | — | Bucklerhorn | Blunt | Guardian, Warrior | `muskox-01.png` | — |
| `newt-alchemist-coil` | — | Coilnewt | Whip | Tinker | `newt-01.png` | — |
| `olm-summoner-shade` | — | Shadeolm | Magic | Summoner, Sorcerer | `olm-01.png` | — |
| `opossum-summoner-dice` | — | Dicetail | Ranged | Summoner, Leader | `opossum-01.png` | — |
| `orc-summoner-skull` | — | Grukk Skullcaller | Blunt | Summoner, Primalist | `orc-01.png` | — |
| `orchidmantis-trickster-leaf` | — | Petalshade | Whip | Primalist, Rogue | `orchidmantis-01.png` | — |
| `organ-summoner` | — | Pipe Organ Choir | Breath | Bard, Guardian | `organ-01.png` | — |
| `orrery-sorcerer-orbit` | — | Orbitwright | Magic | Sorcerer, Leader | `orrery-01.png` | — |
| `ossuary-summoner-cube` | — | Bonecube | Brawl | Summoner, Guardian | `ossuary-01.png` | — |
| `otter-hunter-sling` | — | Slingwhisker | Ranged | Hunter, Rogue | `otter-01.png` | — |
| `owl-wizard` | — | Hootmage | Magic | Sorcerer, Flying | `owl-01.png` | `owl-wizard.png` |
| `panda-monk-fists` | — | Bamboo Monk | Brawl | Warrior | `panda-01.png` | — |
| `pangolin-guard-decoy` | — | Decoy Pangolin | Blunt | Guardian, Rogue | `pangolin-01.png` | — |
| `pangolin-sorcerer-thunder` | — | Thunderscale | Aura | Guardian, Summoner | `pangolin-02.png` | — |
| `panpipe-summoner-reed` | — | Reedcaller | Breath | Bard, Summoner | `panpipe-01.png` | — |
| `peacock-bard-feather` | — | Plumecrown | Magic | Bard, Flying | `peacock-01.png` | — |
| `phoenix-titan-solar` | — | Solar Phoenix | Aura | Healer, Elementalist | `phoenix-01.png` | — |
| `pika-summoner-horn` | — | Hornpip | Breath | Leader, Summoner | `pika-01.png` | — |
| `platypus-alchemist-rune` | — | Runebill | Breath | Tinker | `platypus-01.png` | — |
| `plough-ravager` | — | Furrow Ravager | Bladed | Warrior, Summoner | `plough-01.png` | — |
| `pocketwatch-warden` | — | Tickwarden | Aura | Guardian, Leader | `pocketwatch-01.png` | — |
| `porcupine-alchemist-shield` | — | Quillflask | Ranged | Tinker, Guardian | `porcupine-01.png` | — |
| `portcullis-colossus` | — | Portcullis Colossus | Blunt | Guardian | `portcullis-01.png` | — |
| `printingpress-summoner-type` | — | Typesetter | Ranged | Summoner, Tinker | `printingpress-01.png` | — |
| `prism-hexer` | — | Shardhex | Magic | Flying, Guardian | `prism-01.png` | — |
| `puffin-bodyguard-shield` | — | Shieldpuff | Blunt | Flying, Guardian | `puffin-01.png` | — |
| `quern-oracle` | — | Grindstone Oracle | Blunt | Primalist, Flying | `quern-01.png` | — |
| `quokka-summoner-pebble` | — | Pebblegrin | Magic | Summoner | `quokka-01.png` | — |
| `raccoon-alchemist-potion` | — | Potionpaw | Ranged | Tinker, Rogue | `raccoon-01.png` | — |
| `ram-paladin-sun` | — | Sunhorn Paladin | Brawl | Leader, Guardian | `ram-01.png` | `ram-paladin-sun.png` |
| `ram-bard-bellows` | — | Bellows Ram | Breath | Bard, Tinker | `ram-02.png` | — |
| `reliquary-summoner-seal` | — | Sealed Reliquary | Magic | Summoner, Sorcerer | `reliquary-01.png` | — |
| `retort-alchemist` | — | Bubblehex Retort | Breath | Tinker, Guardian | `retort-01.png` | — |
| `rhinoceros-guard-shadow` | — | Shadowhorn | Bladed | Rogue, Warrior | `rhinoceros-01.png` | — |
| `roadrunner-hunter-whistle` | — | Whistledash | Ranged | Hunter, Flying | `roadrunner-01.png` | — |
| `roc-colossus-thunder` | — | Thunder Roc | Brawl | Hunter, Flying | `roc-01.png` | — |
| `rooster-captain-banner` | — | Banner Cock | Polearm | Leader, Warrior | `rooster-01.png` | — |
| `sableantelope-captain-pennant` | — | Pennant Sable | Polearm | Leader, Warrior | `sableantelope-01.png` | — |
| `safe-hexer-vault` | — | Vaulthex | Brawl | Guardian | `safe-01.png` | — |
| `sail-hexer-night` | — | Nightsail | Aura | Flying, Rogue | `sail-01.png` | — |
| `salamander-rogue` | — | Slinkember | Brawl | Rogue | `salamander-01.png` | `salamander-rogue.png` |
| `salamander-summoner-flask` | — | Flaskember | Ranged | Tinker, Summoner | `salamander-02.png` | — |
| `sarcophagus-warden` | — | Tombwarden | Aura | Summoner, Guardian | `sarcophagus-01.png` | — |
| `scarab-engineer-rune` | — | Runeshell Scarab | Ranged | Tinker, Hunter | `scarab-01.png` | — |
| `scorpion` | — | Stingtail | Whip | Rogue, Hunter | `scorpion-01.png` | `scorpion.png` |
| `scorpion-healer-ribbon` | — | Ribbonsting | Whip | Tinker, Hunter | `scorpion-02.png` | — |
| `seal-bard-bottle` | — | Bottlenose Bard | Breath | Tinker, Bard | `seal-01.png` | — |
| `sealpress-summoner` | — | Waxseal Press | Magic | Summoner, Guardian | `sealpress-01.png` | — |
| `secretarybird-hunter-javelin` | — | Javelin Stalker | Polearm | Flying, Hunter | `secretarybird-01.png` | — |
| `serpent-titan-crown` | — | Crowned Serpent | Ranged | Rogue, Summoner | `serpent-01.png` | — |
| `serval-hunter-crossbow` | — | Boltear | Ranged | Hunter, Rogue | `serval-01.png` | — |
| `sewingbox-engineer-ribbon` | — | Stitchkit | Whip | Tinker, Healer | `sewingbox-01.png` | — |
| `sextant-admiral` | — | Admiral Starsight | Ranged | Hunter, Leader | `sextant-01.png` | — |
| `shield-vanguard` | — | Vanguard Aegis | Blunt | Guardian, Warrior | `shield-01.png` | — |
| `shoebill-captain-pike` | — | Pikebill | Polearm | Leader, Hunter | `shoebill-01.png` | — |
| `shrew-summoner-totem` | — | Totemshrew | Magic | Summoner, Primalist | `shrew-01.png` | — |
| `siphon-alchemist` | — | Siphonworm | Whip | Tinker, Guardian | `siphon-01.png` | — |
| `sistrum-summoner` | — | Rattlechant | Aura | Bard, Sorcerer | `sistrum-01.png` | — |
| `sledgehammer-colossus` | — | Sledge Colossus | Blunt | Warrior, Guardian | `sledgehammer-01.png` | — |
| `sloth-guard-bark` | — | Barkhold Sloth | Brawl | Primalist, Healer | `sloth-01.png` | — |
| `snail-cleric-crystal` | — | Prism Snail | Magic | Elementalist, Healer | `snail-01.png` | `snail-cleric-crystal.png` |
| `snail-bard-brewer` | — | Brewshell | Aura | Tinker, Bard | `snail-05.png` | — |
| `spikeback` | `spikeback` | Spikeback | Brawl | Warrior, Guardian | `spikeback-01.png` | `1_f.png` |
| `spindle-summoner` | — | Spindlewhirl | Whip | Bard, Leader | `spindle-01.png` | — |
| `stag-hunter-shield` | — | Shieldhorn Stag | Polearm | Hunter, Guardian | `stag-01.png` | — |
| `stoat-healer-needle` | — | Needlewhisk | Bladed | Rogue, Hunter | `stoat-01.png` | — |
| `sugarglider-scout-sail` | — | Sailglide | Ranged | Flying, Sorcerer | `sugarglider-01.png` | — |
| `sundial-vanguard` | — | Noonward | Polearm | Guardian | `sundial-01.png` | — |
| `sundrum-hexer` | — | Sunhex Drum | Aura | Tinker, Guardian | `sundrum-01.png` | — |
| `tapestry-summoner` | — | Woven Host | Aura | Summoner, Guardian | `tapestry-01.png` | — |
| `tarsier-hexer-mask` | — | Maskeye | Brawl | Rogue, Sorcerer | `tarsier-01.png` | — |
| `teapot-alchemist-volcano` | — | Volcano Teapot | Breath | Tinker, Elementalist | `teapot-01.png` | — |
| `termite-engineer-drill` | — | Drillmite | Ranged | Tinker | `termite-01.png` | — |
| `termite-summoner-clockwork` | — | Clockmite | Ranged | Tinker, Summoner | `termite-02.png` | — |
| `thorntreant-primalist-bramble` | — | Bramblestalker | Aura | Primalist, Rogue | `thorntreant-01.png` | — |
| `toad-boulder` | `boulder_toad` | Boulder Toad | Blunt | Guardian, Warrior | `toad-01.png` | `5_f.png` |
| `toad-druid` | — | Druid Toad | Breath | Primalist, Healer | `toad-02.png` | `toad-druid.png` |
| `toad-bard-bell` | — | Belltoad | Magic | Bard, Summoner | `toad-03.png` | — |
| `tortoise-colossus-mountain` | — | Mountainshell | Aura | Guardian, Primalist | `tortoise-01.png` | — |
| `tortoise-sentinel-obsidian` | — | Obsidian Tortoise | Aura | Guardian, Summoner | `tortoise-02.png` | — |
| `treant-druid-leaf` | — | Leafwarden | Whip | Primalist, Healer | `treant-01.png` | — |
| `treant-titan-ancient` | — | Elderbough | Whip | Primalist, Healer | `treant-02.png` | — |
| `trowel-crusader` | — | Trowel Crusader | Bladed | Guardian, Warrior | `trowel-01.png` | — |
| `tuningfork-champion` | — | Resonant Champion | Brawl | Bard, Warrior | `tuningfork-01.png` | — |
| `turbine-skyguard` | — | Skyturbine | Ranged | Flying, Warrior | `turbine-01.png` | — |
| `turtle-monk` | — | Shellmonk | Blunt | Healer, Guardian | `turtle-02.png` | `turtle-monk.png` |
| `turtle-bard-frost` | — | Frostshell Bard | Breath | Guardian, Sorcerer | `turtle-03.png` | — |
| `umbrella-guardian-fan` | — | Fanbrella | Blunt | Flying, Elementalist | `umbrella-01.png` | — |
| `velvetworm-hexer-rune` | — | Runevelvet | Whip | Rogue, Primalist | `velvetworm-01.png` | — |
| `vinegaroon-alchemist-flask` | — | Vinegar Flask | Ranged | Tinker, Hunter | `vinegaroon-01.png` | — |
| `vise-enforcer` | — | Vise Enforcer | Brawl | Guardian, Tinker | `vise-01.png` | — |
| `vulture-healer-cinder` | — | Cinderbeak | Aura | Flying, Elementalist | `vulture-01.png` | — |
| `walrus-bard-horn` | — | Tuskhorn Walrus | Breath | Bard | `walrus-01.png` | — |
| `warthog-engineer-cannon` | — | Cannon Warthog | Ranged | Tinker | `warthog-01.png` | — |
| `wasp-alchemist-potion` | — | Stingflask | Ranged | Tinker, Flying | `wasp-01.png` | — |
| `weasel-rogue-dagger` | — | Daggerwhisk | Bladed | Rogue, Hunter | `weasel-01.png` | — |
| `weathervane-summoner-arrow` | — | Gustarrow | Ranged | Summoner, Flying | `weathervane-01.png` | — |
| `weaverbird-summoner-banner` | — | Bannerweave | Magic | Leader, Summoner | `weaverbird-01.png` | — |
| `thornfang` | `thornfang` | Thornfang | Brawl | Hunter, Warrior | `werewolf-01.png` | `6_f.png` |
| `wisp-hexer-shadow` | — | Shadowwisp | Magic | Sorcerer, Rogue | `wisp-01.png` | — |
| `wolf-ranger` | — | Ranger Wolf | Brawl | Hunter, Leader | `wolf-01.png` | `wolf-ranger.png` |
| `wolverine-warrior-mace` | — | Macefang Wolverine | Blunt | Warrior | `wolverine-01.png` | — |
| `wyvern-scout-obsidian` | — | Obsidian Wyvern | Bladed | Flying, Sorcerer | `wyvern-01.png` | — |
| `yak-bard-gong` | — | Gongyak | Blunt | Bard, Summoner | `yak-01.png` | — |

## Card details

Home biome, stats and card framing per creature. Frames are `x,y,w,h` in the art's 1024 × 1536 pixels (2:3; they may reach past the art edges, so x and y can be negative). The front frame is the card front's portrait, the rear frame the card back. There is no Defense stat (removed 2026-10-15): creatures that had Defense got +5 HP per 10%, within HP 20–80. **Cost** (1–10) is team-building points; a team of up to six creatures must fit a budget of 35 (game_system.md §5.1).

| Art file | Name | Biome | HP | Speed | Power | Cost | Front frame | Rear frame | Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|
| `abacus-01.png` | Tallymarch | Cathedral | 40 | 5 | 5 | 5 | 279,-63,603,905 | 0,0,1024,1536 | reviewed |  |
| `accordion-01.png` | Wheezewarden | Cathedral | 40 | 5 | 5 | 5 | 376,0,750,1125 | -6,-62,1095,1643 | reviewed |  |
| `anchor-01.png` | Barnacle Bulwark | Coast | 65 | 3 | 6 | 8 | 252,-105,698,1047 | 0,0,1024,1536 | reviewed |  |
| `ankylosaur-01.png` | Clubtail Grumbleback | Mountain | 70 | 3 | 6 | 9 | 75,324,640,960 | 0,0,1061,1592 | reviewed |  |
| `ant-01.png` | Cogmarch Ant | Jungle | 20 | 8 | 3 | 2 | 206,57,634,951 | 0,0,1024,1536 | reviewed |  |
| `armadillo-01.png` | Armadillo Recruit | Desert | 45 | 6 | 5 | 7 | 38,507,640,960 | 28,70,977,1466 | reviewed |  |
| `armadillo-02.png` | Armadillo Knight | Desert | 50 | 4 | 6 | 7 | 266,255,640,960 | 31,46,993,1490 | reviewed |  |
| `astrolabe-01.png` | Starwright Judge | Cathedral | 40 | 5 | 5 | 5 | 278,8,632,948 | 0,0,1024,1536 | reviewed |  |
| `aurochs-01.png` | Ironhide Aurochs | Savannah | 65 | 3 | 6 | 8 | 296,192,733,1100 | 9,-6,1089,1634 | reviewed |  |
| `axolotl-01.png` | Ripplequill | Lagoon | 20 | 8 | 3 | 2 | 193,98,717,1076 | 0,0,1024,1536 | reviewed |  |
| `ayeaye-01.png` | Candlefinger | Jungle | 20 | 8 | 3 | 2 | 272,144,640,960 | 0,0,1024,1536 | reviewed |  |
| `badger-01.png` | Drumbadger | Autumn woods | 45 | 5 | 5 | 6 | 262,65,683,1025 | 0,0,1024,1536 | reviewed |  |
| `ballast-01.png` | Sandbag Sentinel | Storm cliffs | 50 | 5 | 5 | 7 | 188,-9,760,1140 | 0,0,1024,1536 | reviewed |  |
| `basilisk-01.png` | Stonegaze Basilisk | Desert | 65 | 3 | 6 | 8 | 345,111,640,960 | 0,0,1024,1536 | reviewed |  |
| `bassoon-01.png` | Dirgewind | Ruins | 40 | 5 | 5 | 5 | 310,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `bat-01.png` | Moonwing | Crystal caverns | 20 | 8 | 3 | 2 | 326,131,640,960 | 0,0,1024,1536 | reviewed |  |
| `bat-02.png` | Bat Rogue | Crystal caverns | 20 | 7 | 4 | 2 | 239,28,640,960 | 0,0,1024,1536 | reviewed |  |
| `baton-01.png` | Marshal Tempo | Cathedral | 40 | 5 | 5 | 5 | 140,0,770,1154 | 0,0,1024,1536 | reviewed |  |
| `beacon-01.png` | Lighthouse Wakelamp | Coast | 70 | 3 | 6 | 9 | 195,-22,779,1169 | 0,0,1024,1536 | reviewed |  |
| `beaver-01.png` | Pumpwhisker | Wet forest | 40 | 5 | 5 | 5 | 310,124,640,960 | 0,0,1024,1536 | reviewed |  |
| `bee-01.png` | Sunhum | Flower meadow | 20 | 8 | 3 | 2 | 274,187,634,951 | 0,0,1024,1536 | reviewed |  |
| `beetle-01.png` | Beetle Recruit | Jungle | 45 | 5 | 5 | 6 | 214,321,810,1215 | 45,67,979,1469 | reviewed |  |
| `beetle-02.png` | Beetle Guardian | Jungle | 55 | 4 | 7 | 9 | 225,255,731,1097 | 28,42,996,1494 | reviewed |  |
| `beetle-04.png` | Seedshot Scarab | Jungle | 30 | 8 | 3 | 4 | 348,197,640,960 | 0,0,1024,1536 | reviewed |  |
| `behemoth-01.png` | Ironbound Behemoth | Mountain | 75 | 3 | 6 | 10 | 389,92,640,960 | 0,0,1024,1536 | reviewed |  |
| `bell-01.png` | Clapperward | Cathedral | 65 | 3 | 6 | 8 | 253,65,640,960 | 0,0,1024,1536 | reviewed |  |
| `bellows-01.png` | Puffing Forgedrake | Ashlands | 45 | 5 | 5 | 6 | 41,419,640,960 | 0,0,1024,1536 | reviewed |  |
| `binturong-01.png` | Vinelilt | Jungle | 40 | 5 | 5 | 5 | 118,77,640,960 | 0,0,1024,1536 | reviewed |  |
| `bloomspear-01.png` | Bloomspear | Flower meadow | 40 | 5 | 5 | 5 | 259,110,640,960 | 42,51,973,1460 | reviewed |  |
| `boar-01.png` | Magma Hog | Ashlands | 60 | 4 | 6 | 8 | 35,550,640,960 | -2,272,1024,1536 | reviewed |  |
| `boar-02.png` | Hexbristle | Autumn woods | 45 | 5 | 5 | 6 | 433,181,640,960 | 0,0,1024,1536 | reviewed |  |
| `bombardierbeetle-01.png` | Acidpop | Desert | 30 | 8 | 3 | 4 | 65,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `book-01.png` | Foldtome | Ruins | 40 | 5 | 5 | 5 | 220,475,640,960 | 0,0,1024,1536 | reviewed |  |
| `brazier-01.png` | Coalwraith Brazier | Ashlands | 50 | 5 | 5 | 7 | 203,1,740,1110 | 0,0,1024,1536 | reviewed |  |
| `buffalo-01.png` | Rootyoke Buffalo | Savannah | 65 | 3 | 6 | 8 | 327,141,640,960 | 0,0,1024,1536 | reviewed |  |
| `bugle-01.png` | Skybrass | Cathedral | 20 | 8 | 3 | 2 | 377,201,640,960 | 0,0,1024,1536 | reviewed |  |
| `candelabrum-01.png` | Waxbrawler | Cathedral | 45 | 5 | 5 | 6 | 212,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `cannon-01.png` | Powderbelch | Cathedral | 75 | 3 | 6 | 10 | 466,72,592,888 | 0,0,1024,1536 | reviewed |  |
| `capybara-01.png` | Calmwater Capybara | Jungle | 40 | 5 | 5 | 5 | 422,195,640,960 | 0,0,1024,1536 | reviewed |  |
| `caracal-01.png` | Tuftspear | Savannah | 40 | 5 | 5 | 5 | 364,221,640,960 | 0,0,1024,1536 | reviewed |  |
| `carillon-01.png` | Chimespire | Cathedral | 75 | 3 | 6 | 10 | 161,1,640,960 | 0,0,1024,1536 | reviewed |  |
| `cassowary-01.png` | Crestcaller | Jungle | 40 | 5 | 5 | 5 | 384,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `caterpillar-01.png` | Caterpillar Bard | Mushroom grotto | 20 | 7 | 3 | 1 | 57,362,640,960 | 0,0,1024,1536 | reviewed |  |
| `catfolk-01.png` | Moonclaw Spellblade | Desert | 40 | 5 | 5 | 5 | 281,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `centipede-01.png` | Sicklecoil | Jungle | 50 | 5 | 5 | 7 | 170,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `chalice-01.png` | Stormgoblet | Storm cliffs | 30 | 8 | 3 | 4 | 202,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `chameleon-01.png` | Shiftscale | Jungle | 20 | 8 | 3 | 2 | 277,224,640,960 | 0,0,1024,1536 | reviewed |  |
| `chandelier-01.png` | Candlecrown | Cathedral | 45 | 5 | 5 | 6 | 230,364,640,960 | 0,0,1024,1536 | reviewed |  |
| `chest-01.png` | Lockjaw Chest | Ruins | 55 | 5 | 5 | 8 | 205,292,739,1109 | 0,0,1024,1536 | reviewed |  |
| `chinchilla-01.png` | Ocarina Puff | Mountain | 20 | 8 | 3 | 2 | 105,245,640,960 | 0,0,1024,1536 | reviewed |  |
| `cicada-01.png` | Chimewing Cicada | Savannah | 20 | 8 | 3 | 2 | 334,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `cinderhorn-01.png` | Cinderhorn | Ashlands | 45 | 5 | 6 | 7 | 272,32,640,960 | 0,0,1024,1536 | reviewed |  |
| `coati-01.png` | Diceroll Coati | Jungle | 20 | 8 | 3 | 2 | 135,129,640,960 | 0,0,1024,1536 | reviewed |  |
| `cobra-01.png` | Hoodspitter | Desert | 40 | 5 | 5 | 5 | 330,-36,640,960 | 0,0,1024,1536 | reviewed |  |
| `compass-02.png` | Northneedle | Storm cliffs | 30 | 8 | 3 | 4 | 189,317,640,960 | 0,0,1024,1536 | reviewed |  |
| `conchphone-01.png` | Conchhorn | Coast | 45 | 5 | 5 | 6 | 159,538,640,960 | 0,0,1024,1536 | reviewed |  |
| `corkscrew-01.png` | Twistcork | Ruins | 30 | 8 | 3 | 4 | 207,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-01.png` | Crab Alchemist | Coast | 55 | 6 | 4 | 8 | 266,302,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-02.png` | Crab Smith | Coast | 50 | 5 | 5 | 7 | 222,287,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-03.png` | Decoy Crab | Coast | 50 | 5 | 5 | 7 | 297,318,640,960 | 0,0,1024,1536 | reviewed |  |
| `crane-01.png` | Hoist Titan | Cathedral | 75 | 3 | 6 | 10 | 226,14,640,960 | 0,0,1024,1536 | reviewed |  |
| `crane-02.png` | Featherveil Crane | Wet forest | 40 | 5 | 5 | 5 | 135,81,640,960 | 0,0,1024,1536 | reviewed |  |
| `cricket-01.png` | Fiddlewhit | Flower meadow | 20 | 8 | 3 | 2 | 178,50,640,960 | 0,0,1024,1536 | reviewed |  |
| `crocodile-01.png` | Crocodile Shaman | Swamp | 40 | 3 | 7 | 5 | 266,290,747,1121 | 0,0,1024,1536 | reviewed |  |
| `crow-01.png` | Skullcaw | Ruins | 20 | 8 | 3 | 2 | 186,107,640,960 | 0,0,1024,1536 | reviewed |  |
| `crucible-01.png` | Moltencup | Ashlands | 55 | 5 | 5 | 8 | 205,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `cuttlefish-01.png` | Needleglow | Lagoon | 20 | 8 | 3 | 2 | 190,214,640,960 | 0,0,1024,1536 | reviewed |  |
| `deer-01.png` | Glade Stag | Wet forest | 40 | 6 | 5 | 6 | 256,288,640,960 | 0,0,1024,1536 | reviewed |  |
| `dormouse-01.png` | Bellnap Dormouse | Autumn woods | 20 | 8 | 3 | 2 | 203,191,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-01.png` | Emberwyrm Colossus | Ashlands | 70 | 3 | 6 | 9 | 191,68,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-02.png` | Obsidian Elder | Ashlands | 75 | 3 | 6 | 10 | 183,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-03.png` | Tempest Sovereign | Storm cliffs | 70 | 3 | 6 | 9 | 221,71,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-04.png` | Glacier Titan | Arctic | 75 | 3 | 6 | 10 | 300,15,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragonfly-01.png` | Reedarrow | Swamp | 20 | 8 | 3 | 2 | 342,151,640,960 | 0,0,1024,1536 | reviewed |  |
| `drake-01.png` | Crystalback Drake | Crystal caverns | 75 | 3 | 6 | 10 | 382,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `drawbridge-01.png` | Chaingate | Cathedral | 75 | 3 | 6 | 10 | 173,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `drum-01.png` | Wardrum | Savannah | 50 | 5 | 5 | 7 | 158,87,640,960 | 0,0,1024,1536 | reviewed |  |
| `dugong-02.png` | Crystalfin Dugong | Lagoon | 70 | 3 | 6 | 9 | 307,136,640,960 | 0,0,1024,1536 | reviewed |  |
| `elephant-01.png` | Idolbearer | Savannah | 70 | 3 | 6 | 9 | 370,101,640,960 | 0,0,1024,1536 | reviewed |  |
| `ferret-01.png` | Maskwhisk | Ruins | 20 | 8 | 3 | 2 | 37,67,640,960 | 0,0,1024,1536 | reviewed |  |
| `fiddlercrab-01.png` | Coinclaw | Coast | 30 | 8 | 3 | 4 | 296,236,640,960 | 0,0,1024,1536 | reviewed |  |
| `firefly-01.png` | Lanternwing | Flower meadow | 20 | 8 | 3 | 2 | 84,128,640,960 | 0,0,1024,1536 | reviewed |  |
| `firefly-02.png` | Firefly | Flower meadow | 20 | 8 | 3 | 2 | 339,425,640,960 | 0,0,1024,1536 | reviewed |  |
| `flyingfish-01.png` | Dartfin | Lagoon | 20 | 8 | 3 | 2 | 388,153,640,960 | 0,0,1024,1536 | reviewed |  |
| `fossa-01.png` | Harpoon Fossa | Jungle | 40 | 5 | 5 | 5 | 348,194,640,960 | 0,0,1024,1536 | reviewed |  |
| `fox-01.png` | Fox Bard | Autumn woods | 20 | 8 | 3 | 2 | 55,415,640,960 | 0,0,1024,1536 | reviewed |  |
| `frigatebird-01.png` | Throatdrum | Coast | 40 | 5 | 5 | 5 | 356,75,640,960 | 0,0,1024,1536 | reviewed |  |
| `frog-01.png` | Frog Bard Dagger | Swamp | 20 | 8 | 3 | 2 | 268,45,640,960 | 0,0,1024,1536 | reviewed |  |
| `furnace-01.png` | Furnace Titan | Ashlands | 75 | 3 | 6 | 10 | 280,11,640,960 | 0,0,1024,1536 | reviewed |  |
| `gargoyle-01.png` | Stonewatch Gargoyle | Cathedral | 55 | 5 | 5 | 8 | 220,155,640,960 | 0,0,1024,1536 | reviewed |  |
| `gaur-01.png` | Bolted Gaur | Jungle | 70 | 3 | 6 | 9 | 346,226,640,960 | 0,0,1024,1536 | reviewed |  |
| `gecko-01.png` | Resinpaw | Jungle | 20 | 8 | 3 | 2 | 291,119,640,960 | 0,0,1024,1536 | reviewed |  |
| `gecko-02.png` | Prismtoe | Crystal caverns | 20 | 8 | 3 | 2 | 235,12,640,960 | 0,0,1024,1536 | reviewed |  |
| `goat-01.png` | Totemhorn | Mountain | 40 | 5 | 5 | 5 | 190,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `goblet-01.png` | Grailward | Cathedral | 50 | 5 | 5 | 7 | 264,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `golem-01.png` | Furnaceheart Colossus | Ashlands | 75 | 3 | 6 | 10 | 266,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `golem-02.png` | Gearwright Golem | Ruins | 75 | 3 | 6 | 10 | 237,152,640,960 | 0,0,1024,1536 | reviewed |  |
| `gong-01.png` | Discord Gong | Ruins | 50 | 5 | 5 | 7 | 233,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `gorilla-01.png` | Ironfist Silverback | Jungle | 60 | 3 | 6 | 7 | 242,57,640,960 | 0,0,1024,1536 | reviewed |  |
| `griffin-01.png` | Lancewing Griffin | Mountain | 65 | 3 | 6 | 8 | 193,76,640,960 | 0,0,1024,1536 | reviewed |  |
| `harpsichord-01.png` | Keyclatter | Cathedral | 65 | 3 | 6 | 8 | 181,256,640,960 | 0,0,1024,1536 | reviewed |  |
| `harpy-eagle-01.png` | Talonstrike | Jungle | 40 | 5 | 5 | 5 | 299,87,640,960 | 0,0,1024,1536 | reviewed |  |
| `hawk-01.png` | Skybow Hawk | Mountain | 20 | 8 | 3 | 2 | 264,105,640,960 | 0,0,1024,1536 | reviewed |  |
| `hermitcrab-01.png` | Basaltshell | Coast | 55 | 5 | 5 | 8 | 277,291,640,960 | 0,0,1024,1536 | reviewed |  |
| `heron-01.png` | Wingwright Heron | Swamp | 40 | 5 | 5 | 5 | 288,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `heron-02.png` | Stillspear | Swamp | 40 | 5 | 5 | 5 | 215,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `hippo-01.png` | Tonic Hippo | Swamp | 60 | 3 | 6 | 7 | 298,152,640,960 | 0,0,1024,1536 | reviewed |  |
| `hornbill-01.png` | Drumbill | Jungle | 40 | 5 | 5 | 5 | 274,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `horseshoe-01.png` | Lucky Ironshoe | Savannah | 55 | 5 | 5 | 8 | 230,97,640,960 | 0,0,1024,1536 | reviewed |  |
| `horseshoecrab-01.png` | Bronzeback Sentinel | Coast | 55 | 5 | 5 | 8 | 180,239,640,960 | 0,0,1024,1536 | reviewed |  |
| `hourglass-01.png` | Sandhex Glass | Desert | 45 | 5 | 5 | 6 | 217,12,640,960 | 0,0,1024,1536 | reviewed |  |
| `howlermonkey-01.png` | Roarleaf | Jungle | 40 | 5 | 5 | 5 | 325,83,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-01.png` | Gerald Hollins | Ruins | 50 | 5 | 5 | 7 | 192,101,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-02.png` | Maggie Wren | Ruins | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-03.png` | Mira Castell | Crystal caverns | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-04.png` | Sigrid Halvorsen | Arctic | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-05.png` | Otto Brenner | Ruins | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-06.png` | Lucia Marin | Savannah | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-07.png` | Tom Reed | Pine forest | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-08.png` | Ragnhild Eriksen | Arctic | 40 | 5 | 5 | 5 | 192,59,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-09.png` | Gunnar Holt | Mountain | 40 | 5 | 5 | 5 | 192,84,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-10.png` | Felix Lowell | Cathedral | 45 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-11.png` | Lorenzo Varga | Flower meadow | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-12.png` | Jack Morrow | Coast | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-13.png` | Helena Strand | Cathedral | 45 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-14.png` | Carlos Fonseca | Coast | 40 | 5 | 5 | 5 | 192,166,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-15.png` | Beatrix Alvarez | Coast | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-16.png` | Agnes Brook | Cathedral | 40 | 5 | 5 | 5 | 192,34,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-17.png` | Amara Okafor | Savannah | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-18.png` | Nadia Petrova | Desert | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-19.png` | Fiona Ashby | Wet forest | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-20.png` | Ivy Calloway | Pine forest | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-21.png` | Camille Moreau | Cathedral | 40 | 5 | 5 | 5 | 192,81,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-22.png` | Lukas Weiss | Crystal caverns | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-23.png` | Max Hooper | Ruins | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-24.png` | Ingrid Lund | Arctic | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-25.png` | Leo Stone | Mountain | 40 | 5 | 5 | 5 | 192,151,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-26.png` | Peter Holm | Cathedral | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-27.png` | Johanna Weber | Cathedral | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-28.png` | Albrecht Lang | Cathedral | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-29.png` | Johanka Weber | Mountain | 55 | 5 | 5 | 8 | 192,99,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-30.png` | Edwin Marsh | Cathedral | 55 | 5 | 5 | 8 | 192,165,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-31.png` | William Hart | Cathedral | 55 | 5 | 5 | 8 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-32.png` | Kai Lindqvist | Crystal caverns | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-33.png` | Carmen Costa | Ashlands | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-34.png` | Astor Nilsen | Arctic | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-35.png` | Henrik Berg | Mountain | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-36.png` | Mateo Santos | Lagoon | 40 | 5 | 5 | 5 | 192,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-37.png` | Kenji Mori | Mountain | 40 | 5 | 7 | 5 | 192,89,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-38.png` | Viktor Graves | Ruins | 40 | 5 | 5 | 5 | 214,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-39.png` | Martin Becker | Cathedral | 40 | 5 | 5 | 5 | 192,38,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-40.png` | Luna Ortiz | Flower meadow | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-41.png` | Aurelia Day | Desert | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-42.png` | Thomas Abbott | Cathedral | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-43.png` | Paul Fischer | Swamp | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-44.png` | Nika Volkova | Ruins | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-45.png` | Daniel Hart | Ruins | 40 | 5 | 5 | 5 | 192,132,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-46.png` | Elsa Lindgren | Pine forest | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-47.png` | Kofi Mensah | Jungle | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-48.png` | Hugo Schmidt | Mountain | 40 | 5 | 5 | 5 | 192,111,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-49.png` | Laura Cole | Storm cliffs | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-51.png` | Richard Novak | Storm cliffs | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-52.png` | Arthur Pembroke | Cathedral | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `hummingbird-01.png` | Rapierbeak | Flower meadow | 20 | 8 | 3 | 2 | 206,170,640,960 | 0,0,1024,1536 | reviewed |  |
| `hydra-01.png` | Tridentmaw Hydra | Swamp | 70 | 3 | 6 | 9 | 175,-67,729,1094 | 0,0,1024,1536 | reviewed |  |
| `hyena-01.png` | Sawgrin | Savannah | 40 | 5 | 5 | 5 | 332,102,640,960 | 0,0,1024,1536 | reviewed |  |
| `ibex-01.png` | Standardhorn | Mountain | 40 | 5 | 5 | 5 | 219,38,640,960 | 0,0,1024,1536 | reviewed |  |
| `incense-censor-01.png` | Smokeswing | Cathedral | 50 | 5 | 5 | 7 | 183,17,640,960 | 0,0,1024,1536 | reviewed |  |
| `inkwell-01.png` | Quillspatter | Ruins | 25 | 8 | 3 | 3 | 190,410,640,960 | 0,0,1024,1536 | reviewed |  |
| `isopod-01.png` | Ironroll | Mushroom grotto | 35 | 8 | 3 | 5 | 354,359,640,960 | 0,0,1024,1536 | reviewed |  |
| `jackalope-01.png` | Cursehorn | Desert | 20 | 8 | 3 | 2 | 313,225,640,960 | 0,0,1024,1536 | reviewed |  |
| `jelly-01.png` | Stormbell Jelly | Lagoon | 20 | 8 | 3 | 2 | 205,132,640,960 | 0,0,1024,1536 | reviewed |  |
| `jellyfish-02.png` | Mendbloom Jelly | Lagoon | 20 | 8 | 3 | 2 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `jerboa-01.png` | Acornpop | Desert | 20 | 8 | 3 | 2 | 361,51,640,960 | 0,0,1024,1536 | reviewed |  |
| `kestrel-01.png` | Lurewing | Savannah | 20 | 8 | 3 | 2 | 171,63,640,960 | 0,0,1024,1536 | reviewed |  |
| `kettle-01.png` | Siegekettle | Ruins | 55 | 5 | 5 | 8 | 215,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `kiln-01.png` | Kiln Behemoth | Ashlands | 75 | 3 | 6 | 10 | 212,102,640,960 | 0,0,1024,1536 | reviewed |  |
| `kingfisher-01.png` | Captain Daggerbill | Wet forest | 20 | 8 | 3 | 2 | 206,54,640,960 | 0,0,1024,1536 | reviewed |  |
| `kinkajou-01.png` | Idolpaw | Jungle | 20 | 8 | 3 | 2 | 257,160,640,960 | 0,0,1024,1536 | reviewed |  |
| `kite-01.png` | Tailwind Kite | Storm cliffs | 20 | 8 | 3 | 2 | 262,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `komodo-01.png` | Cleaver Komodo | Jungle | 45 | 5 | 5 | 6 | 269,54,640,960 | 0,0,1024,1536 | reviewed |  |
| `kraken-01.png` | Basalt Kraken | Lagoon | 70 | 3 | 6 | 9 | 164,73,640,960 | 0,0,1024,1536 | reviewed |  |
| `lantern-01.png` | Prismlamp | Crystal caverns | 30 | 8 | 3 | 4 | 168,29,640,960 | 0,0,1024,1536 | reviewed |  |
| `lizardfolk-02.png` | Reef Warden | Lagoon | 45 | 6 | 5 | 7 | 352,64,640,960 | 0,0,1024,1536 | reviewed |  |
| `lizardfolk-03.png` | Axejaw Lizardfolk | Swamp | 40 | 5 | 5 | 5 | 140,22,640,960 | 0,0,1024,1536 | reviewed |  |
| `lizardfolk-04.png` | Kunai Lizardfolk | Swamp | 40 | 5 | 5 | 5 | 223,74,640,960 | 0,0,1024,1536 | reviewed |  |
| `lobster-01.png` | Castanet Lobster | Coast | 50 | 5 | 5 | 7 | 202,138,640,960 | 0,0,1024,1536 | reviewed |  |
| `lobster-02.png` | Rivetclaw | Coast | 50 | 5 | 5 | 7 | 144,105,640,960 | 0,0,1024,1536 | reviewed |  |
| `lockbox-01.png` | Lockbox Dreadnought | Ruins | 75 | 3 | 6 | 10 | 260,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `loom-01.png` | Threadbinder Loom | Ruins | 65 | 3 | 6 | 8 | 232,61,640,960 | 0,0,1024,1536 | reviewed |  |
| `lyrebird-01.png` | Plumesong | Pine forest | 20 | 8 | 3 | 2 | 311,109,640,960 | 0,0,1024,1536 | reviewed |  |
| `mammoth-01.png` | Tundra Titan | Arctic | 65 | 3 | 6 | 8 | 244,97,640,960 | 0,0,1024,1536 | reviewed |  |
| `manatee-01.png` | Shellmend Manatee | Lagoon | 60 | 3 | 6 | 7 | 278,103,640,960 | 0,0,1024,1536 | reviewed |  |
| `mandrake-01.png` | Screamroot | Swamp | 25 | 8 | 3 | 3 | 269,27,640,960 | 0,0,1024,1536 | reviewed |  |
| `mandrill-01.png` | Glovegrin | Jungle | 40 | 5 | 5 | 5 | 221,59,640,960 | 0,0,1024,1536 | reviewed |  |
| `mannequin-01.png` | Stitchfist Mannequin | Ruins | 50 | 5 | 5 | 7 | 96,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `manticore-01.png` | Barbtail Manticore | Desert | 60 | 3 | 6 | 7 | 334,246,640,960 | 0,0,1024,1536 | reviewed |  |
| `mantis-01.png` | Bladewright Mantis | Jungle | 45 | 5 | 5 | 6 | 290,54,640,960 | 0,0,1024,1536 | reviewed |  |
| `marabou-01.png` | Balmbeak | Savannah | 40 | 5 | 5 | 5 | 259,19,640,960 | 0,0,1024,1536 | reviewed |  |
| `maskrack-01.png` | Echomask Rack | Ruins | 45 | 5 | 5 | 6 | 253,-31,640,960 | 0,0,1024,1536 | reviewed |  |
| `mastiff-01.png` | Shieldjaw Mastiff | Cathedral | 50 | 5 | 5 | 7 | 326,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `meerkat-01.png` | Spyglass Meerkat | Desert | 20 | 8 | 3 | 2 | 170,149,640,960 | 0,0,1024,1536 | reviewed |  |
| `millstone-01.png` | Millstone Juggernaut | Savannah | 75 | 3 | 6 | 10 | 250,87,640,960 | 0,0,1024,1536 | reviewed |  |
| `mirror-01.png` | Mirror Revenant | Cathedral | 45 | 5 | 5 | 6 | 178,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `mole-01.png` | Gearclaw Mole | Mushroom grotto | 20 | 8 | 3 | 2 | 330,126,640,960 | 0,0,1024,1536 | reviewed |  |
| `mole-02.png` | Hexhammer Mole | Mushroom grotto | 20 | 8 | 3 | 2 | 258,222,640,960 | 0,0,1024,1536 | reviewed |  |
| `mongoose-01.png` | Sicklewhisk | Savannah | 20 | 8 | 3 | 2 | 343,113,640,960 | 0,0,1024,1536 | reviewed |  |
| `mongoose-02.png` | Hexdart Mongoose | Savannah | 20 | 8 | 3 | 2 | 250,109,640,960 | 0,0,1024,1536 | reviewed |  |
| `monkey-01.png` | Slingtail | Jungle | 20 | 8 | 3 | 2 | 150,113,640,960 | 0,0,1024,1536 | reviewed |  |
| `moray-01.png` | Hookgrin Moray | Lagoon | 40 | 5 | 5 | 5 | 375,51,640,960 | 0,0,1024,1536 | reviewed |  |
| `mortar-01.png` | Bastion Mortar | Ruins | 75 | 3 | 6 | 10 | 176,43,640,960 | 0,0,1024,1536 | reviewed |  |
| `moth-02.png` | Lanternmoth | Mushroom grotto | 20 | 8 | 3 | 2 | 181,130,640,960 | 0,0,1024,1536 | reviewed |  |
| `mushroom-01.png` | Sporedruid | Mushroom grotto | 20 | 8 | 3 | 2 | 192,223,640,960 | 0,0,1024,1536 | reviewed |  |
| `mushroom-03.png` | Capshield | Mushroom grotto | 45 | 5 | 5 | 6 | 182,189,640,960 | 0,0,1024,1536 | reviewed |  |
| `musicbox-01.png` | Windup Drummer | Ruins | 35 | 8 | 3 | 5 | 188,88,640,960 | 0,0,1024,1536 | reviewed |  |
| `muskox-01.png` | Bucklerhorn | Arctic | 70 | 3 | 6 | 9 | 292,187,640,960 | 0,0,1024,1536 | reviewed |  |
| `newt-01.png` | Coilnewt | Swamp | 20 | 8 | 3 | 2 | 392,265,640,960 | 0,0,1024,1536 | reviewed |  |
| `olm-01.png` | Shadeolm | Crystal caverns | 20 | 8 | 3 | 2 | 194,45,640,960 | 0,0,1024,1536 | reviewed |  |
| `opossum-01.png` | Dicetail | Autumn woods | 20 | 8 | 3 | 2 | 335,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `orc-01.png` | Grukk Skullcaller | Ruins | 45 | 5 | 5 | 6 | 224,20,640,960 | 0,0,1024,1536 | reviewed |  |
| `orchidmantis-01.png` | Petalshade | Flower meadow | 20 | 8 | 3 | 2 | 240,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `organ-01.png` | Pipe Organ Choir | Cathedral | 75 | 3 | 6 | 10 | 181,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `orrery-01.png` | Orbitwright | Cathedral | 45 | 5 | 5 | 6 | 211,76,640,960 | 0,0,1024,1536 | reviewed |  |
| `ossuary-01.png` | Bonecube | Ruins | 55 | 5 | 5 | 8 | 235,55,640,960 | 0,0,1024,1536 | reviewed |  |
| `otter-01.png` | Slingwhisker | Wet forest | 20 | 8 | 3 | 2 | 268,142,640,960 | 0,0,1024,1536 | reviewed |  |
| `owl-01.png` | Hootmage | Pine forest | 20 | 8 | 3 | 2 | 192,97,640,960 | 0,0,1024,1536 | reviewed |  |
| `panda-01.png` | Bamboo Monk | Mountain | 60 | 3 | 6 | 7 | 181,48,640,960 | 0,0,1024,1536 | reviewed |  |
| `pangolin-01.png` | Decoy Pangolin | Savannah | 50 | 5 | 5 | 7 | 354,150,640,960 | 0,0,1024,1536 | reviewed |  |
| `pangolin-02.png` | Thunderscale | Savannah | 50 | 5 | 5 | 7 | 372,184,640,960 | 0,0,1024,1536 | reviewed |  |
| `panpipe-01.png` | Reedcaller | Flower meadow | 25 | 8 | 3 | 3 | 189,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `peacock-01.png` | Plumecrown | Flower meadow | 40 | 5 | 5 | 5 | 399,53,640,960 | 0,0,1024,1536 | reviewed |  |
| `phoenix-01.png` | Solar Phoenix | Ashlands | 60 | 3 | 6 | 7 | 188,12,640,960 | 0,0,1024,1536 | reviewed |  |
| `pika-01.png` | Hornpip | Mountain | 20 | 8 | 3 | 2 | 207,153,640,960 | 0,0,1024,1536 | reviewed |  |
| `platypus-01.png` | Runebill | Wet forest | 20 | 8 | 3 | 2 | 335,207,640,960 | 0,0,1024,1536 | reviewed |  |
| `plough-01.png` | Furrow Ravager | Savannah | 75 | 3 | 6 | 10 | 306,233,640,960 | 0,0,1024,1536 | reviewed |  |
| `pocketwatch-01.png` | Tickwarden | Ruins | 35 | 8 | 3 | 5 | 246,37,711,1067 | 0,0,1024,1536 | reviewed |  |
| `porcupine-01.png` | Quillflask | Autumn woods | 25 | 8 | 3 | 3 | 359,185,640,960 | 0,0,1024,1536 | reviewed |  |
| `portcullis-01.png` | Portcullis Colossus | Cathedral | 75 | 3 | 6 | 10 | 263,-21,640,960 | 0,0,1024,1536 | reviewed |  |
| `printingpress-01.png` | Typesetter | Ruins | 75 | 3 | 6 | 10 | 229,61,640,960 | 0,0,1024,1536 | reviewed |  |
| `prism-01.png` | Shardhex | Crystal caverns | 30 | 8 | 3 | 4 | 300,243,640,960 | 0,0,1024,1536 | reviewed |  |
| `puffin-01.png` | Shieldpuff | Arctic | 20 | 8 | 3 | 2 | 337,221,640,960 | 0,0,1024,1536 | reviewed |  |
| `quern-01.png` | Grindstone Oracle | Ruins | 55 | 5 | 5 | 8 | 218,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `quokka-01.png` | Pebblegrin | Savannah | 20 | 8 | 3 | 2 | 119,132,640,960 | 0,0,1024,1536 | reviewed |  |
| `raccoon-01.png` | Potionpaw | Autumn woods | 20 | 8 | 3 | 2 | 211,124,640,960 | 0,0,1024,1536 | reviewed |  |
| `ram-01.png` | Sunhorn Paladin | Mountain | 50 | 5 | 5 | 7 | 275,62,640,960 | 0,0,1024,1536 | reviewed |  |
| `ram-02.png` | Bellows Ram | Mountain | 40 | 5 | 5 | 5 | 204,42,640,960 | 0,0,1024,1536 | reviewed |  |
| `reliquary-01.png` | Sealed Reliquary | Cathedral | 55 | 5 | 5 | 8 | 191,-3,640,960 | 0,0,1024,1536 | reviewed |  |
| `retort-01.png` | Bubblehex Retort | Ruins | 35 | 8 | 3 | 5 | 187,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `rhinoceros-01.png` | Shadowhorn | Savannah | 70 | 3 | 6 | 9 | 247,203,640,960 | 0,0,1024,1536 | reviewed |  |
| `roadrunner-01.png` | Whistledash | Desert | 20 | 8 | 3 | 2 | 141,15,640,960 | 0,0,1024,1536 | reviewed |  |
| `roc-01.png` | Thunder Roc | Storm cliffs | 60 | 3 | 6 | 7 | 278,95,640,960 | 0,0,1024,1536 | reviewed |  |
| `rooster-01.png` | Banner Cock | Savannah | 20 | 8 | 3 | 2 | 133,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `sableantelope-01.png` | Pennant Sable | Savannah | 40 | 5 | 5 | 5 | 301,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `safe-01.png` | Vaulthex | Ruins | 75 | 3 | 6 | 10 | 258,58,640,960 | 0,0,1024,1536 | reviewed |  |
| `sail-01.png` | Nightsail | Storm cliffs | 40 | 5 | 5 | 5 | 284,167,640,960 | 0,0,1024,1536 | reviewed |  |
| `salamander-01.png` | Slinkember | Ashlands | 20 | 8 | 3 | 2 | 7,367,640,960 | 0,0,1024,1536 | reviewed |  |
| `salamander-02.png` | Flaskember | Ashlands | 20 | 8 | 3 | 2 | 306,240,640,960 | 0,0,1024,1536 | reviewed |  |
| `sarcophagus-01.png` | Tombwarden | Desert | 75 | 3 | 6 | 10 | 250,9,640,960 | 0,0,1024,1536 | reviewed |  |
| `scarab-01.png` | Runeshell Scarab | Desert | 30 | 8 | 3 | 4 | 375,286,640,960 | 0,0,1024,1536 | reviewed |  |
| `scorpion-01.png` | Stingtail | Desert | 45 | 5 | 5 | 6 | 124,428,640,960 | 0,0,1024,1536 | reviewed |  |
| `scorpion-02.png` | Ribbonsting | Desert | 45 | 5 | 5 | 6 | 336,378,640,960 | 0,0,1024,1536 | reviewed |  |
| `seal-01.png` | Bottlenose Bard | Arctic | 40 | 5 | 5 | 5 | 292,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `sealpress-01.png` | Waxseal Press | Cathedral | 55 | 5 | 5 | 8 | 253,-6,640,960 | 0,0,1024,1536 | reviewed |  |
| `secretarybird-01.png` | Javelin Stalker | Savannah | 40 | 5 | 5 | 5 | 313,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `serpent-01.png` | Crowned Serpent | Jungle | 65 | 3 | 6 | 8 | 286,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `serval-01.png` | Boltear | Savannah | 20 | 8 | 3 | 2 | 321,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `sewingbox-01.png` | Stitchkit | Ruins | 25 | 8 | 3 | 3 | 138,266,640,960 | 0,0,1024,1536 | reviewed |  |
| `sextant-01.png` | Admiral Starsight | Coast | 55 | 5 | 5 | 8 | 228,-3,640,960 | 0,0,1024,1536 | reviewed |  |
| `shield-01.png` | Vanguard Aegis | Cathedral | 55 | 5 | 5 | 8 | 175,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `shoebill-01.png` | Pikebill | Swamp | 40 | 5 | 5 | 5 | 216,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `shrew-01.png` | Totemshrew | Pine forest | 20 | 8 | 3 | 2 | 218,116,640,960 | 0,0,1024,1536 | reviewed |  |
| `siphon-01.png` | Siphonworm | Ruins | 55 | 5 | 5 | 8 | 223,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `sistrum-01.png` | Rattlechant | Desert | 35 | 8 | 3 | 5 | 172,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `sledgehammer-01.png` | Sledge Colossus | Mountain | 75 | 3 | 6 | 10 | 217,33,640,960 | 0,0,1024,1536 | reviewed |  |
| `sloth-01.png` | Barkhold Sloth | Jungle | 45 | 5 | 5 | 6 | 248,168,640,960 | 0,0,1024,1536 | reviewed |  |
| `snail-01.png` | Prism Snail | Crystal caverns | 25 | 8 | 3 | 3 | 331,244,640,960 | 0,0,1024,1536 | reviewed |  |
| `snail-05.png` | Brewshell | Mushroom grotto | 25 | 8 | 3 | 3 | 292,319,640,960 | 0,0,1024,1536 | reviewed |  |
| `spikeback-01.png` | Spikeback | Desert | 65 | 4 | 6 | 9 | 256,355,768,1152 | 0,0,1024,1536 | reviewed |  |
| `spindle-01.png` | Spindlewhirl | Ruins | 25 | 8 | 3 | 3 | 246,-31,640,960 | 0,0,1024,1536 | reviewed |  |
| `stag-01.png` | Shieldhorn Stag | Pine forest | 40 | 5 | 5 | 5 | 333,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `stoat-01.png` | Needlewhisk | Arctic | 20 | 8 | 3 | 2 | 380,230,640,960 | 0,0,1024,1536 | reviewed |  |
| `sugarglider-01.png` | Sailglide | Jungle | 20 | 8 | 3 | 2 | 208,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `sundial-01.png` | Noonward | Ruins | 55 | 5 | 5 | 8 | 356,222,640,960 | 0,0,1024,1536 | reviewed |  |
| `sundrum-01.png` | Sunhex Drum | Desert | 55 | 5 | 5 | 8 | 182,-65,640,960 | 0,0,1024,1536 | reviewed |  |
| `tapestry-01.png` | Woven Host | Cathedral | 55 | 5 | 5 | 8 | 279,-22,640,960 | 0,0,1024,1536 | reviewed |  |
| `tarsier-01.png` | Maskeye | Jungle | 20 | 8 | 3 | 2 | 166,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `teapot-01.png` | Volcano Teapot | Ashlands | 30 | 8 | 3 | 4 | 198,123,640,960 | 0,0,1024,1536 | reviewed |  |
| `termite-01.png` | Drillmite | Savannah | 25 | 8 | 3 | 3 | 333,212,640,960 | 0,0,1024,1536 | reviewed |  |
| `termite-02.png` | Clockmite | Savannah | 25 | 8 | 3 | 3 | 288,107,640,960 | 0,0,1024,1536 | reviewed |  |
| `thorntreant-01.png` | Bramblestalker | Wet forest | 70 | 3 | 6 | 9 | 237,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `toad-01.png` | Boulder Toad | Mountain | 70 | 2 | 7 | 9 | 256,304,640,960 | 0,0,1024,1536 | reviewed |  |
| `toad-02.png` | Druid Toad | Swamp | 40 | 5 | 5 | 5 | 352,284,640,960 | 0,0,1024,1536 | reviewed |  |
| `toad-03.png` | Belltoad | Swamp | 40 | 5 | 5 | 5 | 243,201,640,960 | 0,0,1024,1536 | reviewed |  |
| `tortoise-01.png` | Mountainshell | Mountain | 75 | 3 | 6 | 10 | 337,85,640,960 | 0,0,1024,1536 | reviewed |  |
| `tortoise-02.png` | Obsidian Tortoise | Ashlands | 75 | 3 | 6 | 10 | 353,293,640,960 | 0,0,1024,1536 | reviewed |  |
| `treant-01.png` | Leafwarden | Wet forest | 65 | 3 | 6 | 8 | 230,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `treant-02.png` | Elderbough | Wet forest | 70 | 3 | 6 | 9 | 190,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `trowel-01.png` | Trowel Crusader | Ruins | 55 | 5 | 5 | 8 | 228,56,640,960 | 0,0,1024,1536 | reviewed |  |
| `tuningfork-01.png` | Resonant Champion | Cathedral | 55 | 5 | 5 | 8 | 256,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `turbine-01.png` | Skyturbine | Storm cliffs | 55 | 5 | 5 | 8 | 161,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `turtle-02.png` | Shellmonk | Coast | 50 | 5 | 5 | 7 | 192,75,640,960 | 0,0,1024,1536 | reviewed |  |
| `turtle-03.png` | Frostshell Bard | Arctic | 50 | 5 | 5 | 7 | 384,244,640,960 | 0,0,1024,1536 | reviewed |  |
| `umbrella-01.png` | Fanbrella | Storm cliffs | 25 | 8 | 3 | 3 | 217,5,640,960 | 0,0,1024,1536 | reviewed |  |
| `velvetworm-01.png` | Runevelvet | Wet forest | 20 | 8 | 3 | 2 | 384,255,640,960 | 0,0,1024,1536 | reviewed |  |
| `vinegaroon-01.png` | Vinegar Flask | Desert | 25 | 8 | 3 | 3 | 205,289,640,960 | 0,0,1024,1536 | reviewed |  |
| `vise-01.png` | Vise Enforcer | Ruins | 55 | 5 | 5 | 8 | 250,185,640,960 | 0,0,1024,1536 | reviewed |  |
| `vulture-01.png` | Cinderbeak | Desert | 40 | 5 | 5 | 5 | 173,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `walrus-01.png` | Tuskhorn Walrus | Arctic | 60 | 3 | 6 | 7 | 176,136,640,960 | 0,0,1024,1536 | reviewed |  |
| `warthog-01.png` | Cannon Warthog | Savannah | 45 | 5 | 5 | 6 | 314,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `wasp-01.png` | Stingflask | Flower meadow | 20 | 8 | 3 | 2 | 310,143,640,960 | 0,0,1024,1536 | reviewed |  |
| `weasel-01.png` | Daggerwhisk | Autumn woods | 20 | 8 | 3 | 2 | 351,162,640,960 | 0,0,1024,1536 | reviewed |  |
| `weathervane-01.png` | Gustarrow | Storm cliffs | 55 | 5 | 5 | 8 | 288,37,640,960 | 0,0,1024,1536 | reviewed |  |
| `weaverbird-01.png` | Bannerweave | Savannah | 20 | 8 | 3 | 2 | 155,76,640,960 | 0,0,1024,1536 | reviewed |  |
| `werewolf-01.png` | Thornfang | Pine forest | 40 | 7 | 5 | 7 | 344,102,640,960 | 0,0,1024,1536 | reviewed |  |
| `wisp-01.png` | Shadowwisp | Ruins | 20 | 8 | 3 | 2 | 289,78,640,960 | 0,0,1024,1536 | reviewed |  |
| `wolf-01.png` | Ranger Wolf | Pine forest | 50 | 5 | 5 | 7 | 331,152,640,960 | 0,0,1024,1536 | reviewed |  |
| `wolverine-01.png` | Macefang Wolverine | Arctic | 40 | 5 | 5 | 5 | 277,180,640,960 | 0,0,1024,1536 | reviewed |  |
| `wyvern-01.png` | Obsidian Wyvern | Ashlands | 65 | 3 | 6 | 8 | 357,150,640,960 | 0,0,1024,1536 | reviewed |  |
| `yak-01.png` | Gongyak | Mountain | 65 | 3 | 6 | 8 | 244,79,800,1200 | 0,0,1024,1536 | reviewed |  |
