# Creature Classes

Attack type and professions for every creature that has art, following [class_system.md](class_system.md) v1.2 (assessed 2026-10-09 from each creature's art and, for the first 54, its earlier classes in [creatures.md](creatures.md)). Each creature has one attack type and one or two professions. Professions have no order of importance. On 2026-10-14 Black Mage was removed: necromantic units became Summoners, the rest Sorcerers, and Sorcerers with fire, frost, water, storm, stone or crystal magic became Elementalists.

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
| `abacus-strategist` | — | Tallymarch | Unarmed | Engineer, Leader | `abacus-01.png` | — |
| `accordion-warlord` | — | Wheezewarden | Unarmed | Bard, Warrior | `accordion-01.png` | — |
| `anchor-sentinel` | — | Barnacle Bulwark | Blunt | Bodyguard, Leader | `anchor-01.png` | — |
| `ankylosaur-guardian-club` | — | Clubtail Grumbleback | Blunt | Guardian, Warrior | `ankylosaur-01.png` | — |
| `ant-engineer-gear` | — | Cogmarch Ant | Ranged | Engineer | `ant-01.png` | — |
| `armadillo-knight-2` | — | Armadillo Recruit | Blunt | Bodyguard | `armadillo-01.png` | `armadillo-knight-2.png` |
| `armadillo-knight` | — | Armadillo Knight | Blunt | Guardian | `armadillo-02.png` | `armadillo-knight.png` |
| `astrolabe-judge` | — | Starwright Judge | Aura | Sorcerer, Leader | `astrolabe-01.png` | — |
| `aurochs-bodyguard-plate` | — | Ironhide Aurochs | Feral | Bodyguard | `aurochs-01.png` | — |
| `axolotl-mage-water` | — | Ripplequill | Magic | Elementalist, Trickster | `axolotl-01.png` | — |
| `ayeaye-hexer-candle` | — | Candlefinger | Magic | Sorcerer, Assassin | `ayeaye-01.png` | — |
| `badger-summoner-drum` | — | Drumbadger | Blunt | Summoner, Warrior | `badger-01.png` | — |
| `ballast-bodyguard` | — | Sandbag Sentinel | Blunt | Bodyguard, Flying | `ballast-01.png` | — |
| `basilisk-sorcerer-gaze` | — | Stonegaze Basilisk | Aura | Sorcerer | `basilisk-01.png` | — |
| `bassoon-hexer-requiem` | — | Dirgewind | Breath | Sorcerer, Bard | `bassoon-01.png` | — |
| `bat-summoner-moon` | — | Moonwing | Magic | Flying, Summoner | `bat-01.png` | — |
| `bat-rogue-shadow` | — | Bat Rogue | Bladed | Assassin, Flying | `bat-02.png` | `bat-rogue-shadow.png` |
| `baton-marshal` | — | Marshal Tempo | Polearm | Leader, Summoner | `baton-01.png` | — |
| `beacon-summoner` | — | Lighthouse Wakelamp | Aura | Summoner, Flying | `beacon-01.png` | — |
| `beaver-healer-pump` | — | Pumpwhisker | Ranged | Engineer, Healer | `beaver-01.png` | — |
| `bee-shaman-sun` | — | Sunhum | Magic | Primalist, Flying | `bee-01.png` | — |
| `beetle-2` | — | Beetle Recruit | Feral | Guardian, Warrior | `beetle-01.png` | `beetle-2.png` |
| `beetle-knight-shield` | — | Beetle Guardian | Blunt | Guardian, Warrior | `beetle-02.png` | `beetle-knight-shield.png` |
| `beetle-engineer-seed` | — | Seedshot Scarab | Ranged | Engineer, Primalist | `beetle-04.png` | — |
| `behemoth-titan-iron` | — | Ironbound Behemoth | Blunt | Bodyguard, Guardian | `behemoth-01.png` | — |
| `bell-guardian-clapper` | — | Clapperward | Blunt | Bard, Summoner | `bell-01.png` | — |
| `bellows-drake` | — | Puffing Forgedrake | Breath | Alchemist, Warrior | `bellows-01.png` | — |
| `binturong-bard-vine` | — | Vinelilt | Lashing | Bard, Primalist | `binturong-01.png` | — |
| `bloomspear` | `bloomspear` | Bloomspear | Polearm | Primalist | `bloomspear-01.png` | `7_f.png` |
| `boar-magma` | `magma_hog` | Magma Hog | Feral | Warrior, Elementalist | `boar-01.png` | `10_f.png` |
| `boar-guard-hex` | — | Hexbristle | Aura | Sorcerer, Bodyguard | `boar-02.png` | — |
| `bombardierbeetle-alchemist-acid` | — | Acidpop | Thrown | Alchemist, Engineer | `bombardierbeetle-01.png` | — |
| `book-summoner-fold` | — | Foldtome | Magic | Summoner, Primalist | `book-01.png` | — |
| `brazier-summoner-coal` | — | Coalwraith Brazier | Magic | Summoner, Sorcerer | `brazier-01.png` | — |
| `buffalo-guard-root` | — | Rootyoke Buffalo | Lashing | Bodyguard, Primalist | `buffalo-01.png` | — |
| `bugle-skyrider` | — | Skybrass | Breath | Bard, Flying | `bugle-01.png` | — |
| `candelabrum-warrior-wax` | — | Waxbrawler | Unarmed | Warrior, Sorcerer | `candelabrum-01.png` | — |
| `cannon-marauder` | — | Powderbelch | Ranged | Hunter, Warrior | `cannon-01.png` | — |
| `capybara-guard-calm` | — | Calmwater Capybara | Aura | Healer, Summoner | `capybara-01.png` | — |
| `caracal-hunter-spear` | — | Tuftspear | Polearm | Hunter, Assassin | `caracal-01.png` | — |
| `carillon-summoner-chime` | — | Chimespire | Magic | Summoner, Bard | `carillon-01.png` | — |
| `cassowary-captain-horn` | — | Crestcaller | Breath | Leader, Warrior | `cassowary-01.png` | — |
| `caterpillar-bard` | — | Caterpillar Bard | Breath | Bard | `caterpillar-01.png` | `caterpillar-bard.png` |
| `catfolk-spellblade-moon` | — | Moonclaw Spellblade | Bladed | Sorcerer, Warrior | `catfolk-01.png` | — |
| `centipede-assassin-sickle` | — | Sicklecoil | Bladed | Assassin, Guardian | `centipede-01.png` | — |
| `chalice-of-storms` | — | Stormgoblet | Magic | Elementalist, Flying | `chalice-01.png` | — |
| `chameleon-trickster-tongue` | — | Shiftscale | Lashing | Trickster, Assassin | `chameleon-01.png` | — |
| `chandelier-summoner-candle` | — | Candlecrown | Magic | Summoner, Bard | `chandelier-01.png` | — |
| `chest-bodyguard-lock` | — | Lockjaw Chest | Feral | Bodyguard, Alchemist | `chest-01.png` | — |
| `chinchilla-summoner-ocarina` | — | Ocarina Puff | Magic | Bard, Summoner | `chinchilla-01.png` | — |
| `cicada-bard-chime` | — | Chimewing Cicada | Magic | Bard, Sorcerer | `cicada-01.png` | — |
| `cinderhorn` | `cinderhorn` | Cinderhorn | Bladed | Warrior, Leader | `cinderhorn-01.png` | `8_f.png` |
| `coati-trickster-dice` | — | Diceroll Coati | Thrown | Trickster, Bard | `coati-01.png` | — |
| `cobra` | — | Hoodspitter | Breath | Alchemist, Assassin | `cobra-01.png` | `cobra.png` |
| `compass-hunter-needle` | — | Northneedle | Bladed | Hunter, Assassin | `compass-02.png` | — |
| `conchphone-bard-horn` | — | Conchhorn | Breath | Bard, Trickster | `conchphone-01.png` | — |
| `corkscrew-stalker` | — | Twistcork | Bladed | Assassin, Trickster | `corkscrew-01.png` | — |
| `crab-alchemist` | — | Crab Alchemist | Thrown | Alchemist | `crab-01.png` | `crab-alchemist.png` |
| `crab-smith` | — | Crab Smith | Blunt | Engineer, Warrior | `crab-02.png` | `crab-smith.png` |
| `crab-guard-decoy` | — | Decoy Crab | Blunt | Bodyguard, Trickster | `crab-03.png` | — |
| `crane-hoist-titan` | — | Hoist Titan | Unarmed | Engineer, Guardian | `crane-01.png` | — |
| `crane-summoner-feather` | — | Featherveil Crane | Magic | Summoner, Healer | `crane-02.png` | — |
| `cricket-bard-fiddle` | — | Fiddlewhit | Magic | Bard, Trickster | `cricket-01.png` | — |
| `crocodile-shaman` | — | Crocodile Shaman | Feral | Primalist, Warrior | `crocodile-01.png` | `crocodile-shaman.png` |
| `crow-necromancer-skull` | — | Skullcaw | Magic | Summoner, Flying | `crow-01.png` | — |
| `crucible-summoner` | — | Moltencup | Breath | Summoner, Alchemist | `crucible-01.png` | — |
| `cuttlefish-assassin-needle` | — | Needleglow | Bladed | Assassin, Hunter | `cuttlefish-01.png` | — |
| `stag-glade` | `glade_stag` | Glade Stag | Feral | Primalist, Healer | `deer-01.png` | `26_f.png` |
| `dormouse-summoner-bell` | — | Bellnap Dormouse | Magic | Summoner, Healer | `dormouse-01.png` | — |
| `dragon-colossus-ember` | — | Emberwyrm Colossus | Breath | Warrior, Elementalist | `dragon-01.png` | — |
| `dragon-elder-obsidian` | — | Obsidian Elder | Aura | Sorcerer, Leader | `dragon-02.png` | — |
| `dragon-sovereign-tempest` | — | Tempest Sovereign | Ranged | Leader, Flying | `dragon-03.png` | — |
| `dragon-titan-glacier` | — | Glacier Titan | Magic | Elementalist, Guardian | `dragon-04.png` | — |
| `dragonfly-scout-bow` | — | Reedarrow | Ranged | Flying, Hunter | `dragonfly-01.png` | — |
| `drake-titan-crystal` | — | Crystalback Drake | Bladed | Guardian, Elementalist | `drake-01.png` | — |
| `drawbridge-bodyguard-chain` | — | Chaingate | Blunt | Bodyguard, Engineer | `drawbridge-01.png` | — |
| `drum-summoner-war` | — | Wardrum | Aura | Summoner, Bard | `drum-01.png` | — |
| `dugong-guard-crystal` | — | Crystalfin Dugong | Aura | Bodyguard, Elementalist | `dugong-02.png` | — |
| `elephant-summoner-idol` | — | Idolbearer | Magic | Bodyguard, Summoner | `elephant-01.png` | — |
| `ferret-summoner-mask` | — | Maskwhisk | Magic | Assassin, Summoner | `ferret-01.png` | — |
| `fiddlercrab-trickster-coin` | — | Coinclaw | Blunt | Trickster, Engineer | `fiddlercrab-01.png` | — |
| `firefly-summoner-lantern` | — | Lanternwing | Aura | Flying, Summoner | `firefly-01.png` | — |
| `firefly` | — | Firefly | Magic | Flying, Sorcerer | `firefly-02.png` | `firefly.png` |
| `flyingfish-scout-dart` | — | Dartfin | Ranged | Flying, Trickster | `flyingfish-01.png` | — |
| `fossa-hunter-harpoon` | — | Harpoon Fossa | Polearm | Hunter, Trickster | `fossa-01.png` | — |
| `fox-bard` | — | Fox Bard | Feral | Bard, Trickster | `fox-01.png` | `fox-bard.png` |
| `frigatebird-bard-throat` | — | Throatdrum | Breath | Bard, Flying | `frigatebird-01.png` | — |
| `frog-bard-dagger` | — | Frog Bard Dagger | Bladed | Bard, Trickster | `frog-01.png` | `frog-bard-dagger.png` |
| `furnace-titan` | — | Furnace Titan | Breath | Warrior, Engineer | `furnace-01.png` | — |
| `gargoyle-sentinel-stone` | — | Stonewatch Gargoyle | Polearm | Guardian, Flying | `gargoyle-01.png` | — |
| `gaur-bodyguard-armor` | — | Bolted Gaur | Feral | Bodyguard, Engineer | `gaur-01.png` | — |
| `gecko-alchemist-resin` | — | Resinpaw | Thrown | Alchemist, Hunter | `gecko-01.png` | — |
| `gecko-summoner-prism` | — | Prismtoe | Magic | Summoner, Sorcerer | `gecko-02.png` | — |
| `gnome-alchemist-flask` | — | Fizzwick Flaskbottom | Thrown | Alchemist, Engineer | `gnome-01.png` | — |
| `goat-shaman-totem` | — | Totemhorn | Feral | Primalist, Warrior | `goat-01.png` | — |
| `goblet-paladin` | — | Grailward | Aura | Healer, Warrior | `goblet-01.png` | — |
| `golem-colossus-furnace` | — | Furnaceheart Colossus | Blunt | Engineer, Warrior | `golem-01.png` | — |
| `golem-engineer-gear` | — | Gearwright Golem | Aura | Engineer, Guardian | `golem-02.png` | — |
| `gong-hexer-dissonance` | — | Discord Gong | Aura | Sorcerer, Bard | `gong-01.png` | — |
| `gorilla-bodyguard-fists` | — | Ironfist Silverback | Unarmed | Bodyguard, Warrior | `gorilla-01.png` | — |
| `griffin-knight-lance` | — | Lancewing Griffin | Polearm | Flying, Warrior | `griffin-01.png` | — |
| `halfling-alchemist` | — | Halfling Alchemist | Thrown | Alchemist | `halfling-01.png` | `halfling-alchemist.png` |
| `harpsichord-bard-keys` | — | Keyclatter | Ranged | Bard, Engineer | `harpsichord-01.png` | — |
| `harpy-eagle-scout-claws` | — | Talonstrike | Unarmed | Flying, Assassin | `harpy-eagle-01.png` | — |
| `hawk-scout-bow` | — | Skybow Hawk | Ranged | Hunter, Flying | `hawk-01.png` | — |
| `hermitcrab-sentinel-basalt` | — | Basaltshell | Aura | Guardian, Primalist | `hermitcrab-01.png` | — |
| `heron-engineer-wing` | — | Wingwright Heron | Ranged | Engineer, Flying | `heron-01.png` | — |
| `heron-hunter-harpoon` | — | Stillspear | Polearm | Hunter, Assassin | `heron-02.png` | — |
| `hippo-healer-potion` | — | Tonic Hippo | Breath | Healer, Guardian | `hippo-01.png` | — |
| `hornbill-bard-drum` | — | Drumbill | Blunt | Bard, Leader | `hornbill-01.png` | — |
| `horseshoe-champion` | — | Lucky Ironshoe | Unarmed | Warrior, Leader | `horseshoe-01.png` | — |
| `horseshoecrab-sentinel-bronze` | — | Bronzeback Sentinel | Blunt | Guardian, Engineer | `horseshoecrab-01.png` | — |
| `hourglass-hexer-sand` | — | Sandhex Glass | Aura | Sorcerer, Trickster | `hourglass-01.png` | — |
| `howlermonkey-bard-roar` | — | Roarleaf | Breath | Bard, Leader | `howlermonkey-01.png` | — |
| `human-alchemist-shield` | — | Gerald Hollins | Thrown | Alchemist, Guardian | `human-01.png` | `human-alchemist-shield.png` |
| `human-alchemist` | — | Maggie Wren | Thrown | Alchemist, Hunter | `human-02.png` | `human-alchemist.png` |
| `human-archer-crystal` | — | Mira Castell | Ranged | Elementalist, Hunter | `human-03.png` | `human-archer-crystal.png` |
| `human-archer-frost` | — | Sigrid Halvorsen | Ranged | Hunter, Elementalist | `human-04.png` | `human-archer-frost.png` |
| `human-archer-gear` | — | Otto Brenner | Ranged | Hunter, Engineer | `human-05.png` | `human-archer-gear.png` |
| `human-archer-sun` | — | Lucia Marin | Ranged | Hunter, Healer | `human-06.png` | `human-archer-sun.png` |
| `human-archer` | — | Tom Reed | Ranged | Hunter | `human-07.png` | `human-archer.png` |
| `human-barbarian-frost` | — | Ragnhild Eriksen | Bladed | Warrior, Elementalist | `human-08.png` | `human-barbarian-frost.png` |
| `human-barbarian` | — | Gunnar Holt | Bladed | Warrior | `human-09.png` | `human-barbarian.png` |
| `human-bard-shield` | — | Felix Lowell | Blunt | Bard, Guardian | `human-10.png` | `human-bard-shield.png` |
| `human-bard` | — | Lorenzo Varga | Magic | Bard | `human-11.png` | `human-bard.png` |
| `human-brawler-hook` | — | Jack Morrow | Blunt | Warrior, Bodyguard | `human-12.png` | `human-brawler-hook.png` |
| `human-captain-banner` | — | Helena Strand | Polearm | Leader, Warrior | `human-13.png` | `human-captain-banner.png` |
| `human-captain-pistol` | — | Carlos Fonseca | Ranged | Leader, Trickster | `human-14.png` | `human-captain-pistol.png` |
| `human-captain-rapier` | — | Beatrix Alvarez | Bladed | Leader, Trickster | `human-15.png` | `human-captain-rapier.png` |
| `human-cleric-lantern` | — | Agnes Brook | Magic | Healer, Alchemist | `human-16.png` | `human-cleric-lantern.png` |
| `human-dancer-spear` | — | Amara Okafor | Polearm | Bard, Warrior | `human-17.png` | `human-dancer-spear.png` |
| `human-dancer` | — | Nadia Petrova | Bladed | Bard, Assassin | `human-18.png` | `human-dancer.png` |
| `human-druid-leaf` | — | Fiona Ashby | Magic | Primalist, Healer | `human-19.png` | `human-druid-leaf.png` |
| `human-druid` | — | Ivy Calloway | Magic | Primalist, Healer | `human-20.png` | `human-druid.png` |
| `human-duelist-rapier` | — | Camille Moreau | Bladed | Warrior, Trickster | `human-21.png` | `human-duelist-rapier.png` |
| `human-engineer-crystal` | — | Lukas Weiss | Magic | Engineer, Elementalist | `human-22.png` | `human-engineer-crystal.png` |
| `human-engineer-hook` | — | Max Hooper | Thrown | Engineer, Trickster | `human-23.png` | `human-engineer-hook.png` |
| `human-guard-frost` | — | Ingrid Lund | Polearm | Elementalist, Warrior | `human-24.png` | `human-guard-frost.png` |
| `human-guard-stone` | — | Leo Stone | Bladed | Guardian, Elementalist | `human-25.png` | `human-guard-stone.png` |
| `human-guard` | — | Peter Holm | Polearm | Guardian, Bodyguard | `human-26.png` | `human-guard.png` |
| `human-knight-2` | — | Johanna Weber | Bladed | Warrior, Guardian | `human-27.png` | `human-knight-2.png` |
| `human-knight-banner` | — | Albrecht Lang | Blunt | Leader, Guardian | `human-28.png` | `human-knight-banner.png` |
| `human-knight-hammer` | — | Gregor Kohl | Blunt | Guardian, Warrior | `human-29.png` | `human-knight-hammer.png` |
| `human-knight-shield` | — | Edwin Marsh | Blunt | Guardian, Bodyguard | `human-30.png` | `human-knight-shield.png` |
| `human-knight` | — | William Hart | Bladed | Warrior, Guardian | `human-31.png` | `human-knight.png` |
| `human-mage-crystal` | — | Kai Lindqvist | Magic | Elementalist | `human-32.png` | `human-mage-crystal.png` |
| `human-mage-fire` | — | Carmen Costa | Magic | Elementalist | `human-33.png` | `human-mage-fire.png` |
| `human-mage-frost` | — | Astrid Nilsen | Magic | Elementalist | `human-34.png` | `human-mage-frost.png` |
| `human-mage-stone` | — | Henrik Berg | Magic | Elementalist | `human-35.png` | `human-mage-stone.png` |
| `human-mage-water` | — | Mateo Santos | Magic | Elementalist, Leader | `human-36.png` | `human-mage-water.png` |
| `human-monk-hammer` | — | Kenji Mori | Blunt | Warrior, Guardian | `human-37.png` | `human-monk-hammer.png` |
| `human-necromancer-skull` | — | Viktor Graves | Bladed | Summoner, Warrior | `human-38.png` | `human-necromancer-skull.png` |
| `human-priest-bell` | — | Martin Becker | Blunt | Healer, Bard | `human-39.png` | `human-priest-bell.png` |
| `human-priest-moon` | — | Luna Ortiz | Magic | Healer, Sorcerer | `human-40.png` | `human-priest-moon.png` |
| `human-priest-sun` | — | Aurelia Day | Ranged | Healer, Primalist | `human-41.png` | `human-priest-sun.png` |
| `human-priest` | — | Thomas Abbott | Magic | Healer, Leader | `human-42.png` | `human-priest.png` |
| `human-rogue-potion` | — | Paul Fischer | Bladed | Alchemist, Assassin | `human-43.png` | `human-rogue-potion.png` |
| `human-rogue-shadow` | — | Nika Volkova | Bladed | Assassin, Sorcerer | `human-44.png` | `human-rogue-shadow.png` |
| `human-rogue` | — | Daniel Hart | Bladed | Assassin | `human-45.png` | `human-rogue.png` |
| `human-scout-lantern` | — | Elsa Lindgren | Bladed | Hunter, Healer | `human-46.png` | `human-scout-lantern.png` |
| `human-shaman` | — | Kofi Mensah | Magic | Primalist, Summoner | `human-47.png` | `human-shaman.png` |
| `human-smith` | — | Hugo Schmidt | Blunt | Engineer, Warrior | `human-48.png` | `human-smith.png` |
| `human-spellblade-2` | — | Laura Cole | Bladed | Warrior, Sorcerer | `human-49.png` | `human-spellblade-2.png` |
| `human-spellblade` | — | Richard Novak | Bladed | Warrior, Sorcerer | `human-51.png` | `human-spellblade.png` |
| `human-wizard` | — | Arthur Pembroke | Magic | Sorcerer | `human-52.png` | `human-wizard.png` |
| `hummingbird-duelist-rapier` | — | Rapierbeak | Bladed | Flying, Trickster | `hummingbird-01.png` | — |
| `hydra-colossus-trident` | — | Tridentmaw Hydra | Breath | Warrior, Alchemist | `hydra-01.png` | — |
| `hyena-engineer-saw` | — | Sawgrin | Ranged | Engineer, Warrior | `hyena-01.png` | — |
| `ibex-captain-standard` | — | Standardhorn | Polearm | Leader, Guardian | `ibex-01.png` | — |
| `incense-censor` | — | Smokeswing | Aura | Sorcerer, Healer | `incense-censor-01.png` | — |
| `inkwell-trickster-quill` | — | Quillspatter | Thrown | Trickster, Assassin | `inkwell-01.png` | — |
| `isopod-sentinel-iron` | — | Ironroll | Unarmed | Guardian, Engineer | `isopod-01.png` | — |
| `jackalope-hexer-curse` | — | Cursehorn | Magic | Sorcerer, Primalist | `jackalope-01.png` | — |
| `jelly-mage-storm` | — | Stormbell Jelly | Aura | Elementalist, Flying | `jelly-01.png` | — |
| `jelly-healer` | — | Mendbloom Jelly | Aura | Healer, Primalist | `jellyfish-02.png` | `jelly-healer.png` |
| `jerboa-trickster-acorn` | — | Acornpop | Thrown | Trickster, Alchemist | `jerboa-01.png` | — |
| `kestrel-hunter-lure` | — | Lurewing | Ranged | Hunter, Summoner | `kestrel-01.png` | — |
| `kettle-alchemist-siege` | — | Siegekettle | Breath | Alchemist, Bard | `kettle-01.png` | — |
| `kiln-behemoth` | — | Kiln Behemoth | Breath | Alchemist, Bodyguard | `kiln-01.png` | — |
| `kingfisher-captain-dagger` | — | Captain Daggerbill | Bladed | Assassin, Leader | `kingfisher-01.png` | — |
| `kinkajou-summoner-idol` | — | Idolpaw | Magic | Summoner, Sorcerer | `kinkajou-01.png` | — |
| `kite-primalist-tail` | — | Tailwind Kite | Lashing | Primalist, Flying | `kite-01.png` | — |
| `komodo-warrior-cleaver` | — | Cleaver Komodo | Blunt | Warrior, Bodyguard | `komodo-01.png` | — |
| `kraken-colossus-basalt` | — | Basalt Kraken | Unarmed | Guardian, Sorcerer | `kraken-01.png` | — |
| `lantern-healer-prism` | — | Prismlamp | Aura | Healer, Sorcerer | `lantern-01.png` | — |
| `reef-warden` | `reef_warden` | Reef Warden | Polearm | Guardian, Leader | `lizardfolk-02.png` | `9_f.png` |
| `lizardfolk-barbarian-axe` | — | Axejaw Lizardfolk | Bladed | Warrior, Leader | `lizardfolk-03.png` | — |
| `lizardfolk-ninja-kunai` | — | Kunai Lizardfolk | Thrown | Assassin, Hunter | `lizardfolk-04.png` | — |
| `lobster-bard-castanet` | — | Castanet Lobster | Unarmed | Bard, Engineer | `lobster-01.png` | — |
| `lobster-engineer-rivet` | — | Rivetclaw | Ranged | Engineer, Guardian | `lobster-02.png` | — |
| `lockbox-dreadnought` | — | Lockbox Dreadnought | Feral | Bodyguard, Sorcerer | `lockbox-01.png` | — |
| `loom-binder` | — | Threadbinder Loom | Lashing | Summoner, Engineer | `loom-01.png` | — |
| `lyrebird-bard-plume` | — | Plumesong | Breath | Bard, Flying | `lyrebird-01.png` | — |
| `mammoth-titan-tundra` | — | Tundra Titan | Polearm | Leader, Bodyguard | `mammoth-01.png` | — |
| `manatee-healer-shell` | — | Shellmend Manatee | Breath | Healer, Guardian | `manatee-01.png` | — |
| `mandrake-primalist-root` | — | Screamroot | Lashing | Primalist, Guardian | `mandrake-01.png` | — |
| `mandrill-brawler-gloves` | — | Glovegrin | Unarmed | Warrior, Trickster | `mandrill-01.png` | — |
| `mannequin` | `mannequin` | Stitchfist Mannequin | Unarmed | Warrior, Guardian | `mannequin-01.png` | `34_f.png` |
| `manticore-assassin-barb` | — | Barbtail Manticore | Lashing | Assassin, Flying | `manticore-01.png` | — |
| `mantis-engineer-blade` | — | Bladewright Mantis | Bladed | Assassin, Engineer | `mantis-01.png` | — |
| `marabou-healer-balm` | — | Balmbeak | Magic | Healer, Alchemist | `marabou-01.png` | — |
| `maskrack-summoner-echo` | — | Echomask Rack | Aura | Summoner, Sorcerer | `maskrack-01.png` | — |
| `mastiff-guard-shield` | — | Shieldjaw Mastiff | Feral | Bodyguard, Hunter | `mastiff-01.png` | — |
| `meerkat-captain-spyglass` | — | Spyglass Meerkat | Ranged | Leader, Hunter | `meerkat-01.png` | — |
| `millstone-juggernaut` | — | Millstone Juggernaut | Blunt | Warrior, Guardian | `millstone-01.png` | — |
| `mirror-revenant` | — | Mirror Revenant | Magic | Sorcerer, Trickster | `mirror-01.png` | — |
| `mole-engineer-gear` | — | Gearclaw Mole | Blunt | Engineer, Hunter | `mole-01.png` | — |
| `mole-engineer-hexhammer` | — | Hexhammer Mole | Blunt | Sorcerer, Engineer | `mole-02.png` | — |
| `mongoose-assassin-sickle` | — | Sicklewhisk | Bladed | Assassin, Hunter | `mongoose-01.png` | — |
| `mongoose-hunter-hex` | — | Hexdart Mongoose | Ranged | Sorcerer, Hunter | `mongoose-02.png` | — |
| `monkey-trickster-sling` | — | Slingtail | Ranged | Trickster, Hunter | `monkey-01.png` | — |
| `moray-trickster-hook` | — | Hookgrin Moray | Lashing | Trickster, Assassin | `moray-01.png` | — |
| `mortar-alchemist-bastion` | — | Bastion Mortar | Thrown | Alchemist, Bodyguard | `mortar-01.png` | — |
| `moth-cleric-lantern` | — | Lanternmoth | Magic | Healer, Flying | `moth-02.png` | — |
| `mushroom-druid` | — | Sporedruid | Magic | Primalist, Healer | `mushroom-01.png` | `mushroom-druid.png` |
| `mushroom-sentinel-shield` | — | Capshield | Aura | Guardian, Primalist | `mushroom-03.png` | — |
| `musicbox-summoner-drum` | — | Windup Drummer | Magic | Summoner, Bard | `musicbox-01.png` | — |
| `muskox-bodyguard-buckler` | — | Bucklerhorn | Blunt | Bodyguard, Warrior | `muskox-01.png` | — |
| `nautilus` | — | Spiralmend | Lashing | Sorcerer, Healer | `nautilus-01.png` | `nautilus.png` |
| `nautilus-sorcerer-tide` | — | Tidecoil | Magic | Elementalist, Primalist | `nautilus-02.png` | — |
| `newt-alchemist-coil` | — | Coilnewt | Lashing | Alchemist, Sorcerer | `newt-01.png` | — |
| `nudibranch-alchemist-vial` | — | Vialfrill | Thrown | Alchemist, Primalist | `nudibranch-01.png` | — |
| `octopus-alchemist` | — | Eightflask | Thrown | Alchemist, Trickster | `octopus-01.png` | `octopus-alchemist.png` |
| `octopus-assassin-ink` | — | Inkveil | Lashing | Assassin, Trickster | `octopus-02.png` | — |
| `okapi-primalist-vine` | — | Vinestripe | Aura | Primalist, Hunter | `okapi-01.png` | — |
| `olm-summoner-shade` | — | Shadeolm | Magic | Summoner, Sorcerer | `olm-01.png` | — |
| `opossum-summoner-dice` | — | Dicetail | Thrown | Summoner, Trickster | `opossum-01.png` | — |
| `orc-summoner-skull` | — | Grukk Skullcaller | Blunt | Summoner, Sorcerer | `orc-01.png` | — |
| `orchidmantis-trickster-leaf` | — | Petalshade | Lashing | Primalist, Trickster | `orchidmantis-01.png` | — |
| `organ-summoner` | — | Pipe Organ Choir | Breath | Summoner, Bard | `organ-01.png` | — |
| `orrery-sorcerer-orbit` | — | Orbitwright | Magic | Sorcerer, Leader | `orrery-01.png` | — |
| `ossuary-summoner-cube` | — | Bonecube | Thrown | Summoner, Sorcerer | `ossuary-01.png` | — |
| `otter-hunter-sling` | — | Slingwhisker | Ranged | Hunter, Trickster | `otter-01.png` | — |
| `owl-wizard` | — | Hootmage | Magic | Sorcerer, Flying | `owl-01.png` | `owl-wizard.png` |
| `panda-monk-fists` | — | Bamboo Monk | Unarmed | Warrior, Healer | `panda-01.png` | — |
| `pangolin-guard-decoy` | — | Decoy Pangolin | Blunt | Guardian, Trickster | `pangolin-01.png` | — |
| `pangolin-sorcerer-thunder` | — | Thunderscale | Aura | Elementalist, Guardian | `pangolin-02.png` | — |
| `panpipe-summoner-reed` | — | Reedcaller | Breath | Summoner, Bard | `panpipe-01.png` | — |
| `peacock-bard-feather` | — | Plumecrown | Magic | Bard, Leader | `peacock-01.png` | — |
| `peccary-warrior-wraps` | — | Wrapfist Peccary | Unarmed | Warrior, Bodyguard | `peccary-01.png` | — |
| `pelican-guard-pouch` | — | Pouchguard | Thrown | Flying, Bodyguard | `pelican-01.png` | — |
| `phoenix-titan-solar` | — | Solar Phoenix | Aura | Healer, Flying | `phoenix-01.png` | — |
| `pika-summoner-horn` | — | Hornpip | Breath | Leader, Summoner | `pika-01.png` | — |
| `platypus-alchemist-rune` | — | Runebill | Breath | Alchemist, Sorcerer | `platypus-01.png` | — |
| `plough-ravager` | — | Furrow Ravager | Bladed | Warrior, Primalist | `plough-01.png` | — |
| `pocketwatch-warden` | — | Tickwarden | Aura | Guardian, Sorcerer | `pocketwatch-01.png` | — |
| `porcupine-alchemist-shield` | — | Quillflask | Thrown | Alchemist, Bodyguard | `porcupine-01.png` | — |
| `portcullis-colossus` | — | Portcullis Colossus | Blunt | Guardian, Leader | `portcullis-01.png` | — |
| `printingpress-summoner-type` | — | Typesetter | Thrown | Summoner, Engineer | `printingpress-01.png` | — |
| `prism-hexer` | — | Shardhex | Magic | Sorcerer, Flying | `prism-01.png` | — |
| `puffin-bodyguard-shield` | — | Shieldpuff | Blunt | Flying, Bodyguard | `puffin-01.png` | — |
| `quern-oracle` | — | Grindstone Oracle | Magic | Primalist, Sorcerer | `quern-01.png` | — |
| `quokka-summoner-pebble` | — | Pebblegrin | Magic | Summoner, Guardian | `quokka-01.png` | — |
| `raccoon-alchemist-potion` | — | Potionpaw | Thrown | Alchemist, Trickster | `raccoon-01.png` | — |
| `ram-paladin-sun` | — | Sunhorn Paladin | Feral | Primalist, Leader | `ram-01.png` | `ram-paladin-sun.png` |
| `ram-bard-bellows` | — | Bellows Ram | Breath | Bard, Engineer | `ram-02.png` | — |
| `ray-mage-crystal` | — | Crystalray | Magic | Flying, Elementalist | `ray-01.png` | `ray-mage-crystal.png` |
| `reliquary-summoner-seal` | — | Sealed Reliquary | Magic | Summoner, Sorcerer | `reliquary-01.png` | — |
| `retort-alchemist` | — | Bubblehex Retort | Breath | Alchemist, Sorcerer | `retort-01.png` | — |
| `rhinoceros-guard-shadow` | — | Shadowhorn | Bladed | Assassin, Bodyguard | `rhinoceros-01.png` | — |
| `roadrunner-hunter-whistle` | — | Whistledash | Ranged | Bard, Hunter | `roadrunner-01.png` | — |
| `roc-colossus-thunder` | — | Thunder Roc | Feral | Flying, Hunter | `roc-01.png` | — |
| `rooster-captain-banner` | — | Banner Cock | Polearm | Leader, Warrior | `rooster-01.png` | — |
| `sableantelope-captain-pennant` | — | Pennant Sable | Polearm | Leader, Warrior | `sableantelope-01.png` | — |
| `safe-hexer-vault` | — | Vaulthex | Unarmed | Sorcerer, Bodyguard | `safe-01.png` | — |
| `sail-hexer-night` | — | Nightsail | Magic | Sorcerer, Flying | `sail-01.png` | — |
| `salamander-rogue` | — | Slinkember | Feral | Trickster, Assassin | `salamander-01.png` | `salamander-rogue.png` |
| `salamander-summoner-flask` | — | Flaskember | Thrown | Alchemist, Summoner | `salamander-02.png` | — |
| `sarcophagus-warden` | — | Tombwarden | Aura | Summoner, Guardian | `sarcophagus-01.png` | — |
| `scarab-engineer-rune` | — | Runeshell Scarab | Ranged | Sorcerer, Engineer | `scarab-01.png` | — |
| `scorpion` | — | Stingtail | Lashing | Assassin, Alchemist | `scorpion-01.png` | `scorpion.png` |
| `scorpion-healer-ribbon` | — | Ribbonsting | Lashing | Assassin, Healer | `scorpion-02.png` | — |
| `slug-sea` | — | Sea Slug | Aura | Alchemist, Healer | `sea-slug-01.png` | `slug-sea.png` |
| `seahorse-sorcerer-frost` | — | Frostfin Seahorse | Magic | Elementalist, Flying | `seahorse-01.png` | — |
| `seal-bard-bottle` | — | Bottlenose Bard | Breath | Alchemist, Bard | `seal-01.png` | — |
| `sealpress-summoner` | — | Waxseal Press | Magic | Summoner, Sorcerer | `sealpress-01.png` | — |
| `secretarybird-hunter-javelin` | — | Javelin Stalker | Polearm | Hunter, Flying | `secretarybird-01.png` | — |
| `serpent-titan-crown` | — | Crowned Serpent | Thrown | Alchemist, Leader | `serpent-01.png` | — |
| `serval-hunter-crossbow` | — | Boltear | Ranged | Hunter, Trickster | `serval-01.png` | — |
| `sewingbox-engineer-ribbon` | — | Stitchkit | Lashing | Engineer, Healer | `sewingbox-01.png` | — |
| `sextant-admiral` | — | Admiral Starsight | Magic | Leader, Hunter | `sextant-01.png` | — |
| `shark-land` | `land_shark` | Land Shark | Feral | Warrior, Hunter | `shark-02.png` | `12_f.png` |
| `shield-vanguard` | — | Vanguard Aegis | Blunt | Guardian, Warrior | `shield-01.png` | — |
| `shoebill-captain-pike` | — | Pikebill | Polearm | Leader, Hunter | `shoebill-01.png` | — |
| `shrew-summoner-totem` | — | Totemshrew | Magic | Summoner, Primalist | `shrew-01.png` | — |
| `siphon-alchemist` | — | Siphonworm | Lashing | Alchemist, Engineer | `siphon-01.png` | — |
| `sistrum-summoner` | — | Rattlechant | Magic | Summoner, Bard | `sistrum-01.png` | — |
| `sledgehammer-colossus` | — | Sledge Colossus | Blunt | Bodyguard, Warrior | `sledgehammer-01.png` | — |
| `sloth-guard-bark` | — | Barkhold Sloth | Feral | Bodyguard, Primalist | `sloth-01.png` | — |
| `snail-cleric-crystal` | — | Prism Snail | Magic | Elementalist, Healer | `snail-01.png` | `snail-cleric-crystal.png` |
| `snail-bard-brewer` | — | Brewshell | Aura | Alchemist, Bard | `snail-05.png` | — |
| `spider-mage-crystal` | — | Crystal Weaver | Magic | Elementalist, Trickster | `spider-01.png` | `spider-mage-crystal.png` |
| `spider-rogue` | — | Skitterknife | Feral | Trickster, Assassin | `spider-02.png` | `spider-rogue.png` |
| `spikeback` | `spikeback` | Spikeback | Feral | Warrior, Guardian | `spikeback-01.png` | `1_f.png` |
| `spindle-summoner` | — | Spindlewhirl | Lashing | Summoner, Bard | `spindle-01.png` | — |
| `squid-alchemist-crystal` | — | Crystalink Squid | Magic | Alchemist, Elementalist | `squid-01.png` | — |
| `stag-hunter-shield` | — | Shieldhorn Stag | Polearm | Hunter, Guardian | `stag-01.png` | — |
| `starfish-mage-crystal` | — | Starmend | Magic | Elementalist, Healer | `starfish-01.png` | `starfish-mage-crystal.png` |
| `stingray-healer-coral` | — | Coralmend Ray | Breath | Healer, Primalist | `stingray-01.png` | — |
| `stoat-healer-needle` | — | Needlewhisk | Bladed | Assassin, Healer | `stoat-01.png` | — |
| `sugarglider-scout-sail` | — | Sailglide | Ranged | Flying, Hunter | `sugarglider-01.png` | — |
| `sundial-vanguard` | — | Noonward | Polearm | Leader, Sorcerer | `sundial-01.png` | — |
| `sundrum-hexer` | — | Sunhex Drum | Aura | Sorcerer, Alchemist | `sundrum-01.png` | — |
| `tanuki-trickster-leaf` | — | Leafshift Tanuki | Magic | Trickster, Alchemist | `tanuki-01.png` | — |
| `tapestry-summoner` | — | Woven Host | Aura | Summoner, Sorcerer | `tapestry-01.png` | — |
| `tapir-primalist-orchid` | — | Orchid Tapir | Aura | Primalist, Healer | `tapir-01.png` | — |
| `tarsier-hexer-mask` | — | Maskeye | Magic | Sorcerer, Trickster | `tarsier-01.png` | — |
| `teapot-alchemist-volcano` | — | Volcano Teapot | Breath | Alchemist, Elementalist | `teapot-01.png` | — |
| `termite-engineer-drill` | — | Drillmite | Ranged | Engineer, Alchemist | `termite-01.png` | — |
| `termite-summoner-clockwork` | — | Clockmite | Magic | Engineer, Summoner | `termite-02.png` | — |
| `thorntreant-primalist-bramble` | — | Bramblestalker | Aura | Primalist, Assassin | `thorntreant-01.png` | — |
| `toad-boulder` | `boulder_toad` | Boulder Toad | Blunt | Guardian, Warrior | `toad-01.png` | `5_f.png` |
| `toad-druid` | — | Druid Toad | Breath | Alchemist, Primalist | `toad-02.png` | `toad-druid.png` |
| `toad-bard-bell` | — | Belltoad | Magic | Bard, Sorcerer | `toad-03.png` | — |
| `tortoise-colossus-mountain` | — | Mountainshell | Aura | Guardian, Primalist | `tortoise-01.png` | — |
| `tortoise-sentinel-obsidian` | — | Obsidian Tortoise | Aura | Guardian, Sorcerer | `tortoise-02.png` | — |
| `treant-druid-leaf` | — | Leafwarden | Lashing | Primalist, Guardian | `treant-01.png` | — |
| `treant-titan-ancient` | — | Elderbough | Lashing | Primalist, Summoner | `treant-02.png` | — |
| `trowel-crusader` | — | Trowel Crusader | Bladed | Guardian, Primalist | `trowel-01.png` | — |
| `tuningfork-champion` | — | Resonant Champion | Unarmed | Bard, Warrior | `tuningfork-01.png` | — |
| `turbine-skyguard` | — | Skyturbine | Ranged | Flying, Engineer | `turbine-01.png` | — |
| `turtle-hornshell` | `hornshell` | Hornshell | Feral | Guardian | `turtle-01.png` | `11_f.png` |
| `turtle-monk` | — | Shellmonk | Blunt | Healer, Guardian | `turtle-02.png` | `turtle-monk.png` |
| `turtle-bard-frost` | — | Frostshell Bard | Breath | Bard, Elementalist | `turtle-03.png` | — |
| `umbrella-guardian-fan` | — | Fanbrella | Blunt | Guardian, Flying | `umbrella-01.png` | — |
| `velvetworm-hexer-rune` | — | Runevelvet | Lashing | Sorcerer, Trickster | `velvetworm-01.png` | — |
| `vinegaroon-alchemist-flask` | — | Vinegar Flask | Thrown | Alchemist, Assassin | `vinegaroon-01.png` | — |
| `vise-enforcer` | — | Vise Enforcer | Unarmed | Bodyguard, Engineer | `vise-01.png` | — |
| `vulture-healer-cinder` | — | Cinderbeak | Magic | Healer, Sorcerer | `vulture-01.png` | — |
| `walrus-bard-horn` | — | Tuskhorn Walrus | Breath | Bard, Guardian | `walrus-01.png` | — |
| `warthog-engineer-cannon` | — | Cannon Warthog | Ranged | Engineer, Bodyguard | `warthog-01.png` | — |
| `wasp-alchemist-potion` | — | Stingflask | Thrown | Alchemist, Flying | `wasp-01.png` | — |
| `weasel-rogue-dagger` | — | Daggerwhisk | Bladed | Assassin, Trickster | `weasel-01.png` | — |
| `weathervane-summoner-arrow` | — | Gustarrow | Ranged | Summoner, Flying | `weathervane-01.png` | — |
| `weaverbird-summoner-banner` | — | Bannerweave | Magic | Leader, Summoner | `weaverbird-01.png` | — |
| `thornfang` | `thornfang` | Thornfang | Feral | Hunter, Warrior | `werewolf-01.png` | `6_f.png` |
| `windsock-summoner-gale` | — | Galesock | Lashing | Summoner, Flying | `windsock-01.png` | — |
| `wisp-hexer-shadow` | — | Shadowwisp | Magic | Sorcerer, Assassin | `wisp-01.png` | — |
| `wolf-ranger` | — | Ranger Wolf | Feral | Hunter, Bodyguard | `wolf-01.png` | `wolf-ranger.png` |
| `wolverine-warrior-mace` | — | Macefang Wolverine | Blunt | Warrior, Guardian | `wolverine-01.png` | — |
| `wombat-warrior-fists` | — | Fistburrow | Unarmed | Warrior, Engineer | `wombat-01.png` | — |
| `wyvern-scout-obsidian` | — | Obsidian Wyvern | Bladed | Flying, Hunter | `wyvern-01.png` | — |
| `yak-bard-gong` | — | Gongyak | Blunt | Bard, Bodyguard | `yak-01.png` | — |

## Card details

Home biome, stats and card framing per creature. Frames are `x,y,w,h` in the art's 1024 × 1536 pixels (2:3; they may reach past the art edges, so x and y can be negative). The front frame is the card front's portrait, the rear frame the card back.

| Art file | Name | Biome | HP | Speed | Power | Defense | Front frame | Rear frame | Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|
| `abacus-01.png` | Tallymarch | Cathedral | 40 | 5 | 5 | 0% | 189,57,745,1117 | 0,0,1024,1536 | reviewed |  |
| `accordion-01.png` | Wheezewarden | Cathedral | 40 | 5 | 5 | 0% | 255,117,840,1260 | -6,-62,1095,1643 | reviewed |  |
| `anchor-01.png` | Barnacle Bulwark | Coast | 60 | 3 | 6 | 0% | 150,-37,877,1316 | 0,0,1024,1536 | reviewed |  |
| `ankylosaur-01.png` | Clubtail Grumbleback | Mountain | 60 | 3 | 6 | 20% | 75,324,640,960 | 0,0,1061,1592 | reviewed |  |
| `ant-01.png` | Cogmarch Ant | Jungle | 20 | 8 | 3 | 0% | 160,54,794,1191 | 0,0,1024,1536 | reviewed |  |
| `armadillo-01.png` | Armadillo Recruit | Desert | 40 | 6 | 5 | 10% | 38,507,640,960 | 28,70,977,1466 | reviewed | remove tier stat completely. we dont need it. you can keep it in the dialog so the dialog doesnt need rework, but we ignore it from now on |
| `armadillo-02.png` | Armadillo Knight | Desert | 45 | 4 | 6 | 20% | 266,255,640,960 | 31,46,993,1490 | reviewed |  |
| `astrolabe-01.png` | Starwright Judge | Cathedral | 40 | 5 | 5 | 10% | 93,-25,930,1395 | 0,0,1024,1536 | reviewed |  |
| `aurochs-01.png` | Ironhide Aurochs | Savannah | 60 | 3 | 6 | 10% | 296,192,733,1100 | 9,-6,1089,1634 | reviewed |  |
| `axolotl-01.png` | Ripplequill | Lagoon | 20 | 8 | 3 | 0% | 193,98,717,1076 | 0,0,1024,1536 | reviewed |  |
| `ayeaye-01.png` | Candlefinger | Jungle | 20 | 8 | 3 | 0% | 272,144,640,960 | 0,0,1024,1536 | reviewed |  |
| `badger-01.png` | Drumbadger | Autumn woods | 40 | 5 | 5 | 10% | 262,65,683,1025 | 0,0,1024,1536 | reviewed |  |
| `ballast-01.png` | Sandbag Sentinel | Storm cliffs | 40 | 5 | 5 | 20% | 188,-9,760,1140 | 0,0,1024,1536 | reviewed |  |
| `basilisk-01.png` | Stonegaze Basilisk | Desert | 60 | 3 | 6 | 10% | 345,111,640,960 | 0,0,1024,1536 | reviewed |  |
| `bassoon-01.png` | Dirgewind | Ruins | 40 | 5 | 5 | 0% | 310,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `bat-01.png` | Moonwing | Crystal caverns | 20 | 8 | 3 | 0% | 326,131,640,960 | 0,0,1024,1536 | reviewed |  |
| `bat-02.png` | Bat Rogue | Crystal caverns | 20 | 7 | 4 | 0% | 239,28,640,960 | 0,0,1024,1536 | reviewed |  |
| `baton-01.png` | Marshal Tempo | Cathedral | 40 | 5 | 5 | 0% | 140,0,770,1154 | 0,0,1024,1536 | reviewed |  |
| `beacon-01.png` | Lighthouse Wakelamp | Coast | 60 | 3 | 6 | 20% | 195,-22,779,1169 | 0,0,1024,1536 | reviewed |  |
| `beaver-01.png` | Pumpwhisker | Wet forest | 40 | 5 | 5 | 0% | 310,124,640,960 | 0,0,1024,1536 | reviewed |  |
| `bee-01.png` | Sunhum | Flower meadow | 20 | 8 | 3 | 0% | 274,187,634,951 | 0,0,1024,1536 | reviewed |  |
| `beetle-01.png` | Beetle Recruit | Jungle | 30 | 5 | 5 | 30% | 214,321,810,1215 | 45,67,979,1469 | reviewed |  |
| `beetle-02.png` | Beetle Guardian | Jungle | 45 | 4 | 7 | 20% | 225,255,731,1097 | 28,42,996,1494 | reviewed |  |
| `beetle-04.png` | Seedshot Scarab | Jungle | 20 | 8 | 3 | 20% | 348,197,640,960 | 0,0,1024,1536 | reviewed |  |
| `behemoth-01.png` | Ironbound Behemoth | Mountain | 60 | 3 | 6 | 30% | 389,92,640,960 | 0,0,1024,1536 | reviewed |  |
| `bell-01.png` | Clapperward | Cathedral | 60 | 3 | 6 | 10% | 253,65,640,960 | 0,0,1024,1536 | reviewed |  |
| `bellows-01.png` | Puffing Forgedrake | Ashlands | 40 | 5 | 5 | 10% | 41,419,640,960 | 0,0,1024,1536 | reviewed |  |
| `binturong-01.png` | Vinelilt | Jungle | 40 | 5 | 5 | 0% | 118,77,640,960 | 0,0,1024,1536 | reviewed |  |
| `bloomspear-01.png` | Bloomspear | Flower meadow | 40 | 5 | 5 | 0% | 259,110,640,960 | 42,51,973,1460 | reviewed |  |
| `boar-01.png` | Magma Hog | Ashlands | 60 | 4 | 6 | 0% | 35,550,640,960 | 0,0,1024,1536 | reviewed |  |
| `boar-02.png` | Hexbristle | Autumn woods | 40 | 5 | 5 | 10% | 433,181,640,960 | 0,0,1024,1536 | reviewed |  |
| `bombardierbeetle-01.png` | Acidpop | Desert | 20 | 8 | 3 | 20% | 65,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `book-01.png` | Foldtome | Ruins | 40 | 5 | 5 | 0% | 220,475,640,960 | 0,0,1024,1536 | reviewed |  |
| `brazier-01.png` | Coalwraith Brazier | Ashlands | 40 | 5 | 5 | 20% | 203,1,740,1110 | 0,0,1024,1536 | reviewed |  |
| `buffalo-01.png` | Rootyoke Buffalo | Savannah | 60 | 3 | 6 | 10% | 327,141,640,960 | 0,0,1024,1536 | reviewed |  |
| `bugle-01.png` | Skybrass | Storm cliffs | 20 | 8 | 3 | 0% | 377,201,640,960 | 0,0,1024,1536 | reviewed |  |
| `candelabrum-01.png` | Waxbrawler | Cathedral | 40 | 5 | 5 | 10% | 212,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `cannon-01.png` | Powderbelch | Cathedral | 60 | 3 | 6 | 30% | 200,79,813,1220 | 0,0,1024,1536 | reviewed |  |
| `capybara-01.png` | Calmwater Capybara | Jungle | 40 | 5 | 5 | 0% | 422,195,640,960 | 0,0,1024,1536 | reviewed |  |
| `caracal-01.png` | Tuftspear | Savannah | 40 | 5 | 5 | 0% | 250,135,640,960 | 0,0,1024,1536 | draft |  |
| `carillon-01.png` | Chimespire | Cathedral | 60 | 3 | 6 | 20% | 161,1,640,960 | 0,0,1024,1536 | draft |  |
| `cassowary-01.png` | Crestcaller | Jungle | 40 | 5 | 5 | 0% | 384,0,640,960 | 0,0,1024,1536 | draft |  |
| `caterpillar-01.png` | Caterpillar Bard | Mushroom grotto | 20 | 7 | 3 | 0% | 57,362,640,960 | 0,0,1024,1536 | reviewed |  |
| `catfolk-01.png` | Moonclaw Spellblade | Desert | 40 | 5 | 5 | 0% | 150,25,640,960 | 0,0,1024,1536 | draft |  |
| `centipede-01.png` | Sicklecoil | Jungle | 40 | 5 | 5 | 10% | 170,44,640,960 | 0,0,1024,1536 | draft |  |
| `chalice-01.png` | Stormgoblet | Storm cliffs | 20 | 8 | 3 | 10% | 202,0,640,960 | 0,0,1024,1536 | draft |  |
| `chameleon-01.png` | Shiftscale | Jungle | 20 | 8 | 3 | 0% | 101,232,640,960 | 0,0,1024,1536 | draft |  |
| `chandelier-01.png` | Candlecrown | Cathedral | 40 | 5 | 5 | 10% | 192,34,640,960 | 0,0,1024,1536 | draft |  |
| `chest-01.png` | Lockjaw Chest | Ruins | 40 | 5 | 5 | 30% | 170,242,640,960 | 0,0,1024,1536 | draft |  |
| `chinchilla-01.png` | Ocarina Puff | Mountain | 20 | 8 | 3 | 0% | 192,135,640,960 | 0,0,1024,1536 | draft |  |
| `cicada-01.png` | Chimewing Cicada | Savannah | 20 | 8 | 3 | 10% | 275,0,640,960 | 0,0,1024,1536 | draft |  |
| `cinderhorn-01.png` | Cinderhorn | Ashlands | 45 | 5 | 6 | 0% | 272,32,640,960 | 0,0,1024,1536 | reviewed |  |
| `coati-01.png` | Diceroll Coati | Jungle | 20 | 8 | 3 | 0% | 210,76,640,960 | 0,0,1024,1536 | draft |  |
| `cobra-01.png` | Hoodspitter | Desert | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `compass-02.png` | Northneedle | Storm cliffs | 20 | 8 | 3 | 10% | 253,34,640,960 | 0,0,1024,1536 | draft |  |
| `conchphone-01.png` | Conchhorn | Coast | 40 | 5 | 5 | 10% | 198,9,640,960 | 0,0,1024,1536 | draft |  |
| `corkscrew-01.png` | Twistcork | Ruins | 20 | 8 | 3 | 10% | 207,44,640,960 | 0,0,1024,1536 | draft |  |
| `crab-01.png` | Crab Alchemist | Coast | 45 | 6 | 4 | 20% | 266,302,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-02.png` | Crab Smith | Coast | 40 | 5 | 5 | 20% | 222,287,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-03.png` | Decoy Crab | Coast | 40 | 5 | 5 | 20% | 297,318,640,960 | 0,0,1024,1536 | draft |  |
| `crane-01.png` | Hoist Titan | Ruins | 60 | 3 | 6 | 30% | 226,14,640,960 | 0,0,1024,1536 | draft |  |
| `crane-02.png` | Featherveil Crane | Wet forest | 40 | 5 | 5 | 0% | 135,81,640,960 | 0,0,1024,1536 | draft |  |
| `cricket-01.png` | Fiddlewhit | Flower meadow | 20 | 8 | 3 | 0% | 178,50,640,960 | 0,0,1024,1536 | draft |  |
| `crocodile-01.png` | Crocodile Shaman | Swamp | 40 | 3 | 7 | 0% | 266,290,747,1121 | 0,0,1024,1536 | reviewed |  |
| `crow-01.png` | Skullcaw | Ruins | 20 | 8 | 3 | 0% | 186,107,640,960 | 0,0,1024,1536 | draft |  |
| `crucible-01.png` | Moltencup | Ashlands | 40 | 5 | 5 | 20% | 205,60,640,960 | 0,0,1024,1536 | draft |  |
| `cuttlefish-01.png` | Needleglow | Lagoon | 20 | 8 | 3 | 0% | 190,214,640,960 | 0,0,1024,1536 | draft |  |
| `deer-01.png` | Glade Stag | Wet forest | 40 | 6 | 5 | 0% | 256,288,640,960 | 0,0,1024,1536 | draft |  |
| `dormouse-01.png` | Bellnap Dormouse | Autumn woods | 20 | 8 | 3 | 0% | 203,191,640,960 | 0,0,1024,1536 | draft |  |
| `dragon-01.png` | Emberwyrm Colossus | Ashlands | 60 | 3 | 6 | 20% | 191,68,640,960 | 0,0,1024,1536 | draft |  |
| `dragon-02.png` | Obsidian Elder | Ashlands | 60 | 3 | 6 | 20% | 183,0,640,960 | 0,0,1024,1536 | draft |  |
| `dragon-03.png` | Tempest Sovereign | Storm cliffs | 60 | 3 | 6 | 10% | 221,71,640,960 | 0,0,1024,1536 | draft |  |
| `dragon-04.png` | Glacier Titan | Arctic | 60 | 3 | 6 | 20% | 186,9,640,960 | 0,0,1024,1536 | draft |  |
| `dragonfly-01.png` | Reedarrow | Swamp | 20 | 8 | 3 | 0% | 194,111,640,960 | 0,0,1024,1536 | draft |  |
| `drake-01.png` | Crystalback Drake | Crystal caverns | 60 | 3 | 6 | 30% | 299,67,640,960 | 0,0,1024,1536 | draft |  |
| `drawbridge-01.png` | Chaingate | Cathedral | 60 | 3 | 6 | 30% | 173,52,640,960 | 0,0,1024,1536 | draft |  |
| `drum-01.png` | Wardrum | Savannah | 40 | 5 | 5 | 10% | 158,87,640,960 | 0,0,1024,1536 | draft |  |
| `dugong-02.png` | Crystalfin Dugong | Lagoon | 60 | 3 | 6 | 10% | 202,173,640,960 | 0,0,1024,1536 | draft |  |
| `elephant-01.png` | Idolbearer | Savannah | 60 | 3 | 6 | 10% | 210,101,640,960 | 0,0,1024,1536 | draft |  |
| `ferret-01.png` | Maskwhisk | Ruins | 20 | 8 | 3 | 0% | 197,156,640,960 | 0,0,1024,1536 | draft |  |
| `fiddlercrab-01.png` | Coinclaw | Coast | 20 | 8 | 3 | 20% | 163,261,640,960 | 0,0,1024,1536 | draft |  |
| `firefly-01.png` | Lanternwing | Flower meadow | 20 | 8 | 3 | 0% | 195,171,640,960 | 0,0,1024,1536 | draft |  |
| `firefly-02.png` | Firefly | Flower meadow | 20 | 8 | 3 | 0% | 339,425,640,960 | 0,0,1024,1536 | reviewed |  |
| `flyingfish-01.png` | Dartfin | Lagoon | 20 | 8 | 3 | 0% | 194,79,640,960 | 0,0,1024,1536 | draft |  |
| `fossa-01.png` | Harpoon Fossa | Jungle | 40 | 5 | 5 | 0% | 172,175,640,960 | 0,0,1024,1536 | draft |  |
| `fox-01.png` | Fox Bard | Autumn woods | 20 | 8 | 3 | 0% | 55,415,640,960 | 0,0,1024,1536 | reviewed |  |
| `frigatebird-01.png` | Throatdrum | Coast | 40 | 5 | 5 | 0% | 177,94,640,960 | 0,0,1024,1536 | draft |  |
| `frog-01.png` | Frog Bard Dagger | Swamp | 20 | 8 | 3 | 0% | 287,138,640,960 | 0,0,1024,1536 | reviewed |  |
| `furnace-01.png` | Furnace Titan | Ashlands | 60 | 3 | 6 | 30% | 181,30,640,960 | 0,0,1024,1536 | draft |  |
| `gargoyle-01.png` | Stonewatch Gargoyle | Cathedral | 40 | 5 | 5 | 30% | 254,53,640,960 | 0,0,1024,1536 | draft |  |
| `gaur-01.png` | Bolted Gaur | Jungle | 60 | 3 | 6 | 30% | 260,170,640,960 | 0,0,1024,1536 | draft |  |
| `gecko-01.png` | Resinpaw | Jungle | 20 | 8 | 3 | 0% | 186,150,640,960 | 0,0,1024,1536 | draft |  |
| `gecko-02.png` | Prismtoe | Crystal caverns | 20 | 8 | 3 | 0% | 247,18,640,960 | 0,0,1024,1536 | draft |  |
| `gnome-01.png` | Fizzwick Flaskbottom | Mushroom grotto | 20 | 8 | 3 | 0% | 188,50,640,960 | 0,0,1024,1536 | draft |  |
| `goat-01.png` | Totemhorn | Mountain | 40 | 5 | 5 | 0% | 190,46,640,960 | 0,0,1024,1536 | draft |  |
| `goblet-01.png` | Grailward | Cathedral | 40 | 5 | 5 | 10% | 150,115,640,960 | 0,0,1024,1536 | draft |  |
| `golem-01.png` | Furnaceheart Colossus | Ashlands | 60 | 3 | 6 | 30% | 192,63,640,960 | 0,0,1024,1536 | draft |  |
| `golem-02.png` | Gearwright Golem | Ruins | 60 | 3 | 6 | 30% | 218,75,640,960 | 0,0,1024,1536 | draft |  |
| `gong-01.png` | Discord Gong | Ruins | 40 | 5 | 5 | 20% | 153,68,640,960 | 0,0,1024,1536 | draft |  |
| `gorilla-01.png` | Ironfist Silverback | Jungle | 60 | 3 | 6 | 0% | 174,82,640,960 | 0,0,1024,1536 | draft |  |
| `griffin-01.png` | Lancewing Griffin | Mountain | 60 | 3 | 6 | 10% | 193,76,640,960 | 0,0,1024,1536 | draft |  |
| `halfling-01.png` | Halfling Alchemist | Pine forest | 20 | 8 | 3 | 0% | 192,98,640,960 | 0,0,1024,1536 | reviewed |  |
| `harpsichord-01.png` | Keyclatter | Cathedral | 60 | 3 | 6 | 10% | 292,105,640,960 | 0,0,1024,1536 | draft |  |
| `harpy-eagle-01.png` | Talonstrike | Jungle | 40 | 5 | 5 | 0% | 203,78,640,960 | 0,0,1024,1536 | draft |  |
| `hawk-01.png` | Skybow Hawk | Mountain | 20 | 8 | 3 | 0% | 184,99,640,960 | 0,0,1024,1536 | draft |  |
| `hermitcrab-01.png` | Basaltshell | Coast | 40 | 5 | 5 | 30% | 126,143,640,960 | 0,0,1024,1536 | draft |  |
| `heron-01.png` | Wingwright Heron | Swamp | 40 | 5 | 5 | 0% | 189,1,640,960 | 0,0,1024,1536 | draft |  |
| `heron-02.png` | Stillspear | Swamp | 40 | 5 | 5 | 0% | 215,7,640,960 | 0,0,1024,1536 | draft |  |
| `hippo-01.png` | Tonic Hippo | Swamp | 60 | 3 | 6 | 10% | 141,96,640,960 | 0,0,1024,1536 | draft |  |
| `hornbill-01.png` | Drumbill | Jungle | 40 | 5 | 5 | 0% | 274,44,640,960 | 0,0,1024,1536 | draft |  |
| `horseshoe-01.png` | Lucky Ironshoe | Savannah | 40 | 5 | 5 | 20% | 184,131,640,960 | 0,0,1024,1536 | draft |  |
| `horseshoecrab-01.png` | Bronzeback Sentinel | Coast | 40 | 5 | 5 | 30% | 294,202,640,960 | 0,0,1024,1536 | draft |  |
| `hourglass-01.png` | Sandhex Glass | Desert | 40 | 5 | 5 | 10% | 217,12,640,960 | 0,0,1024,1536 | draft |  |
| `howlermonkey-01.png` | Roarleaf | Jungle | 40 | 5 | 5 | 0% | 140,123,640,960 | 0,0,1024,1536 | draft |  |
| `human-01.png` | Gerald Hollins | Ruins | 40 | 5 | 5 | 20% | 192,101,640,960 | 0,0,1024,1536 | draft |  |
| `human-02.png` | Maggie Wren | Ruins | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-03.png` | Mira Castell | Crystal caverns | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-04.png` | Sigrid Halvorsen | Arctic | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-05.png` | Otto Brenner | Ruins | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-06.png` | Lucia Marin | Savannah | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-07.png` | Tom Reed | Pine forest | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-08.png` | Ragnhild Eriksen | Arctic | 40 | 5 | 5 | 10% | 192,59,640,960 | 0,0,1024,1536 | draft |  |
| `human-09.png` | Gunnar Holt | Mountain | 40 | 5 | 5 | 0% | 192,84,640,960 | 0,0,1024,1536 | draft |  |
| `human-10.png` | Felix Lowell | Cathedral | 40 | 5 | 5 | 20% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-11.png` | Lorenzo Varga | Flower meadow | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-12.png` | Jack Morrow | Coast | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-13.png` | Helena Strand | Cathedral | 40 | 5 | 5 | 10% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-14.png` | Carlos Fonseca | Coast | 40 | 5 | 5 | 0% | 192,166,640,960 | 0,0,1024,1536 | draft |  |
| `human-15.png` | Beatrix Alvarez | Coast | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-16.png` | Agnes Brook | Cathedral | 40 | 5 | 5 | 0% | 192,34,640,960 | 0,0,1024,1536 | draft |  |
| `human-17.png` | Amara Okafor | Savannah | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-18.png` | Nadia Petrova | Desert | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-19.png` | Fiona Ashby | Wet forest | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-20.png` | Ivy Calloway | Pine forest | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-21.png` | Camille Moreau | Cathedral | 40 | 5 | 5 | 0% | 192,81,640,960 | 0,0,1024,1536 | draft |  |
| `human-22.png` | Lukas Weiss | Crystal caverns | 40 | 5 | 5 | 10% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-23.png` | Max Hooper | Ruins | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-24.png` | Ingrid Lund | Arctic | 40 | 5 | 5 | 20% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-25.png` | Leo Stone | Mountain | 40 | 5 | 5 | 20% | 192,151,640,960 | 0,0,1024,1536 | draft |  |
| `human-26.png` | Peter Holm | Cathedral | 40 | 5 | 5 | 20% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-27.png` | Johanna Weber | Cathedral | 40 | 5 | 5 | 30% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-28.png` | Albrecht Lang | Cathedral | 40 | 5 | 5 | 30% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-29.png` | Gregor Kohl | Mountain | 40 | 5 | 5 | 30% | 192,99,640,960 | 0,0,1024,1536 | draft |  |
| `human-30.png` | Edwin Marsh | Cathedral | 40 | 5 | 5 | 30% | 192,165,640,960 | 0,0,1024,1536 | draft |  |
| `human-31.png` | William Hart | Cathedral | 40 | 5 | 5 | 30% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-32.png` | Kai Lindqvist | Crystal caverns | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-33.png` | Carmen Costa | Ashlands | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-34.png` | Astrid Nilsen | Arctic | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-35.png` | Henrik Berg | Mountain | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-36.png` | Mateo Santos | Lagoon | 40 | 5 | 5 | 0% | 192,35,640,960 | 0,0,1024,1536 | draft |  |
| `human-37.png` | Kenji Mori | Mountain | 40 | 5 | 5 | 0% | 192,89,640,960 | 0,0,1024,1536 | draft |  |
| `human-38.png` | Viktor Graves | Ruins | 40 | 5 | 5 | 10% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-39.png` | Martin Becker | Cathedral | 40 | 5 | 5 | 0% | 192,38,640,960 | 0,0,1024,1536 | draft |  |
| `human-40.png` | Luna Ortiz | Flower meadow | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-41.png` | Aurelia Day | Desert | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-42.png` | Thomas Abbott | Cathedral | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-43.png` | Paul Fischer | Swamp | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-44.png` | Nika Volkova | Ruins | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-45.png` | Daniel Hart | Ruins | 40 | 5 | 5 | 0% | 192,132,640,960 | 0,0,1024,1536 | draft |  |
| `human-46.png` | Elsa Lindgren | Pine forest | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-47.png` | Kofi Mensah | Jungle | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-48.png` | Hugo Schmidt | Mountain | 40 | 5 | 5 | 10% | 192,111,640,960 | 0,0,1024,1536 | draft |  |
| `human-49.png` | Laura Cole | Storm cliffs | 40 | 5 | 5 | 10% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-51.png` | Richard Novak | Storm cliffs | 40 | 5 | 5 | 10% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `human-52.png` | Arthur Pembroke | Cathedral | 40 | 5 | 5 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `hummingbird-01.png` | Rapierbeak | Flower meadow | 20 | 8 | 3 | 0% | 206,170,640,960 | 0,0,1024,1536 | draft |  |
| `hydra-01.png` | Tridentmaw Hydra | Swamp | 60 | 3 | 6 | 10% | 195,0,640,960 | 0,0,1024,1536 | draft |  |
| `hyena-01.png` | Sawgrin | Savannah | 40 | 5 | 5 | 10% | 190,56,640,960 | 0,0,1024,1536 | draft |  |
| `ibex-01.png` | Standardhorn | Mountain | 40 | 5 | 5 | 10% | 219,38,640,960 | 0,0,1024,1536 | draft |  |
| `incense-censor-01.png` | Smokeswing | Cathedral | 40 | 5 | 5 | 10% | 183,17,640,960 | 0,0,1024,1536 | draft |  |
| `inkwell-01.png` | Quillspatter | Ruins | 20 | 8 | 3 | 10% | 384,68,640,960 | 0,0,1024,1536 | draft |  |
| `isopod-01.png` | Ironroll | Mushroom grotto | 20 | 8 | 3 | 30% | 147,251,640,960 | 0,0,1024,1536 | draft |  |
| `jackalope-01.png` | Cursehorn | Desert | 20 | 8 | 3 | 0% | 267,0,640,960 | 0,0,1024,1536 | draft |  |
| `jelly-01.png` | Stormbell Jelly | Lagoon | 20 | 8 | 3 | 0% | 205,132,640,960 | 0,0,1024,1536 | draft |  |
| `jellyfish-02.png` | Mendbloom Jelly | Lagoon | 20 | 8 | 3 | 0% | 192,16,640,960 | 0,0,1024,1536 | draft |  |
| `jerboa-01.png` | Acornpop | Desert | 20 | 8 | 3 | 0% | 272,20,640,960 | 0,0,1024,1536 | draft |  |
| `kestrel-01.png` | Lurewing | Savannah | 20 | 8 | 3 | 0% | 171,63,640,960 | 0,0,1024,1536 | draft |  |
| `kettle-01.png` | Siegekettle | Ruins | 40 | 5 | 5 | 20% | 215,0,640,960 | 0,0,1024,1536 | draft |  |
| `kiln-01.png` | Kiln Behemoth | Ashlands | 60 | 3 | 6 | 30% | 271,139,640,960 | 0,0,1024,1536 | draft |  |
| `kingfisher-01.png` | Captain Daggerbill | Wet forest | 20 | 8 | 3 | 0% | 206,54,640,960 | 0,0,1024,1536 | draft |  |
| `kinkajou-01.png` | Idolpaw | Jungle | 20 | 8 | 3 | 0% | 161,114,640,960 | 0,0,1024,1536 | draft |  |
| `kite-01.png` | Tailwind Kite | Storm cliffs | 20 | 8 | 3 | 0% | 262,0,640,960 | 0,0,1024,1536 | draft |  |
| `komodo-01.png` | Cleaver Komodo | Jungle | 40 | 5 | 5 | 10% | 136,42,640,960 | 0,0,1024,1536 | draft |  |
| `kraken-01.png` | Basalt Kraken | Lagoon | 60 | 3 | 6 | 20% | 164,73,640,960 | 0,0,1024,1536 | draft |  |
| `lantern-01.png` | Prismlamp | Crystal caverns | 20 | 8 | 3 | 10% | 168,29,640,960 | 0,0,1024,1536 | draft |  |
| `lizardfolk-02.png` | Reef Warden | Lagoon | 40 | 6 | 5 | 10% | 352,64,640,960 | 0,0,1024,1536 | draft |  |
| `lizardfolk-03.png` | Axejaw Lizardfolk | Swamp | 40 | 5 | 5 | 10% | 140,22,640,960 | 0,0,1024,1536 | draft |  |
| `lizardfolk-04.png` | Kunai Lizardfolk | Swamp | 40 | 5 | 5 | 0% | 223,74,640,960 | 0,0,1024,1536 | draft |  |
| `lobster-01.png` | Castanet Lobster | Coast | 40 | 5 | 5 | 20% | 202,138,640,960 | 0,0,1024,1536 | draft |  |
| `lobster-02.png` | Rivetclaw | Coast | 40 | 5 | 5 | 20% | 230,93,640,960 | 0,0,1024,1536 | draft |  |
| `lockbox-01.png` | Lockbox Dreadnought | Ruins | 60 | 3 | 6 | 30% | 260,120,640,960 | 0,0,1024,1536 | draft |  |
| `loom-01.png` | Threadbinder Loom | Ruins | 60 | 3 | 6 | 10% | 281,49,640,960 | 0,0,1024,1536 | draft |  |
| `lyrebird-01.png` | Plumesong | Pine forest | 20 | 8 | 3 | 0% | 197,66,640,960 | 0,0,1024,1536 | draft |  |
| `mammoth-01.png` | Tundra Titan | Arctic | 60 | 3 | 6 | 10% | 225,131,640,960 | 0,0,1024,1536 | draft |  |
| `manatee-01.png` | Shellmend Manatee | Lagoon | 60 | 3 | 6 | 10% | 278,143,640,960 | 0,0,1024,1536 | draft |  |
| `mandrake-01.png` | Screamroot | Swamp | 20 | 8 | 3 | 0% | 269,27,640,960 | 0,0,1024,1536 | draft |  |
| `mandrill-01.png` | Glovegrin | Jungle | 40 | 5 | 5 | 0% | 221,59,640,960 | 0,0,1024,1536 | draft |  |
| `mannequin-01.png` | Stitchfist Mannequin | Ruins | 40 | 5 | 5 | 0% | 96,0,640,960 | 0,0,1024,1536 | draft |  |
| `manticore-01.png` | Barbtail Manticore | Desert | 60 | 3 | 6 | 10% | 235,46,640,960 | 0,0,1024,1536 | draft |  |
| `mantis-01.png` | Bladewright Mantis | Jungle | 40 | 5 | 5 | 10% | 262,11,640,960 | 0,0,1024,1536 | draft |  |
| `marabou-01.png` | Balmbeak | Savannah | 40 | 5 | 5 | 0% | 216,28,640,960 | 0,0,1024,1536 | draft |  |
| `maskrack-01.png` | Echomask Rack | Ruins | 40 | 5 | 5 | 10% | 305,0,640,960 | 0,0,1024,1536 | draft |  |
| `mastiff-01.png` | Shieldjaw Mastiff | Cathedral | 40 | 5 | 5 | 20% | 264,97,640,960 | 0,0,1024,1536 | draft |  |
| `meerkat-01.png` | Spyglass Meerkat | Desert | 20 | 8 | 3 | 0% | 170,149,640,960 | 0,0,1024,1536 | draft |  |
| `millstone-01.png` | Millstone Juggernaut | Savannah | 60 | 3 | 6 | 30% | 201,102,640,960 | 0,0,1024,1536 | draft |  |
| `mirror-01.png` | Mirror Revenant | Cathedral | 40 | 5 | 5 | 10% | 178,0,640,960 | 0,0,1024,1536 | draft |  |
| `mole-01.png` | Gearclaw Mole | Mushroom grotto | 20 | 8 | 3 | 10% | 330,126,640,960 | 0,0,1024,1536 | draft |  |
| `mole-02.png` | Hexhammer Mole | Mushroom grotto | 20 | 8 | 3 | 10% | 310,268,640,960 | 0,0,1024,1536 | draft |  |
| `mongoose-01.png` | Sicklewhisk | Savannah | 20 | 8 | 3 | 0% | 192,122,640,960 | 0,0,1024,1536 | draft |  |
| `mongoose-02.png` | Hexdart Mongoose | Savannah | 20 | 8 | 3 | 0% | 120,112,640,960 | 0,0,1024,1536 | draft |  |
| `monkey-01.png` | Slingtail | Jungle | 20 | 8 | 3 | 0% | 150,113,640,960 | 0,0,1024,1536 | draft |  |
| `moray-01.png` | Hookgrin Moray | Lagoon | 40 | 5 | 5 | 0% | 196,36,640,960 | 0,0,1024,1536 | draft |  |
| `mortar-01.png` | Bastion Mortar | Ruins | 60 | 3 | 6 | 30% | 148,0,640,960 | 0,0,1024,1536 | draft |  |
| `moth-02.png` | Lanternmoth | Mushroom grotto | 20 | 8 | 3 | 0% | 181,130,640,960 | 0,0,1024,1536 | draft |  |
| `mushroom-01.png` | Sporedruid | Mushroom grotto | 20 | 8 | 3 | 0% | 192,223,640,960 | 0,0,1024,1536 | draft |  |
| `mushroom-03.png` | Capshield | Mushroom grotto | 40 | 5 | 5 | 20% | 182,189,640,960 | 0,0,1024,1536 | draft |  |
| `musicbox-01.png` | Windup Drummer | Ruins | 20 | 8 | 3 | 10% | 188,88,640,960 | 0,0,1024,1536 | draft |  |
| `muskox-01.png` | Bucklerhorn | Arctic | 60 | 3 | 6 | 20% | 292,187,640,960 | 0,0,1024,1536 | draft |  |
| `nautilus-01.png` | Spiralmend | Lagoon | 20 | 8 | 3 | 20% | 192,300,640,960 | 0,0,1024,1536 | draft |  |
| `nautilus-02.png` | Tidecoil | Lagoon | 40 | 5 | 5 | 20% | 198,162,640,960 | 0,0,1024,1536 | draft |  |
| `newt-01.png` | Coilnewt | Swamp | 20 | 8 | 3 | 0% | 173,120,640,960 | 0,0,1024,1536 | draft |  |
| `nudibranch-01.png` | Vialfrill | Lagoon | 20 | 8 | 3 | 0% | 315,183,640,960 | 0,0,1024,1536 | draft |  |
| `octopus-01.png` | Eightflask | Lagoon | 40 | 5 | 5 | 0% | 192,250,640,960 | 0,0,1024,1536 | draft |  |
| `octopus-02.png` | Inkveil | Lagoon | 40 | 5 | 5 | 0% | 159,163,640,960 | 0,0,1024,1536 | draft |  |
| `okapi-01.png` | Vinestripe | Jungle | 40 | 5 | 5 | 0% | 187,11,640,960 | 0,0,1024,1536 | draft |  |
| `olm-01.png` | Shadeolm | Crystal caverns | 20 | 8 | 3 | 0% | 194,45,640,960 | 0,0,1024,1536 | draft |  |
| `opossum-01.png` | Dicetail | Autumn woods | 20 | 8 | 3 | 0% | 190,108,640,960 | 0,0,1024,1536 | draft |  |
| `orc-01.png` | Grukk Skullcaller | Ruins | 40 | 5 | 5 | 10% | 224,20,640,960 | 0,0,1024,1536 | draft |  |
| `orchidmantis-01.png` | Petalshade | Flower meadow | 20 | 8 | 3 | 0% | 240,0,640,960 | 0,0,1024,1536 | draft |  |
| `organ-01.png` | Pipe Organ Choir | Cathedral | 60 | 3 | 6 | 20% | 181,0,640,960 | 0,0,1024,1536 | draft |  |
| `orrery-01.png` | Orbitwright | Cathedral | 40 | 5 | 5 | 10% | 211,76,640,960 | 0,0,1024,1536 | draft |  |
| `ossuary-01.png` | Bonecube | Ruins | 40 | 5 | 5 | 20% | 189,64,640,960 | 0,0,1024,1536 | draft |  |
| `otter-01.png` | Slingwhisker | Wet forest | 20 | 8 | 3 | 0% | 268,142,640,960 | 0,0,1024,1536 | draft |  |
| `owl-01.png` | Hootmage | Pine forest | 20 | 8 | 3 | 0% | 192,97,640,960 | 0,0,1024,1536 | draft |  |
| `panda-01.png` | Bamboo Monk | Mountain | 60 | 3 | 6 | 0% | 298,104,640,960 | 0,0,1024,1536 | draft |  |
| `pangolin-01.png` | Decoy Pangolin | Savannah | 40 | 5 | 5 | 30% | 141,181,640,960 | 0,0,1024,1536 | draft |  |
| `pangolin-02.png` | Thunderscale | Savannah | 40 | 5 | 5 | 30% | 267,135,640,960 | 0,0,1024,1536 | draft |  |
| `panpipe-01.png` | Reedcaller | Flower meadow | 20 | 8 | 3 | 0% | 291,55,640,960 | 0,0,1024,1536 | draft |  |
| `peacock-01.png` | Plumecrown | Flower meadow | 40 | 5 | 5 | 0% | 158,84,640,960 | 0,0,1024,1536 | draft |  |
| `peccary-01.png` | Wrapfist Peccary | Desert | 40 | 5 | 5 | 0% | 253,98,640,960 | 0,0,1024,1536 | draft |  |
| `pelican-01.png` | Pouchguard | Coast | 40 | 5 | 5 | 0% | 192,53,640,960 | 0,0,1024,1536 | draft |  |
| `phoenix-01.png` | Solar Phoenix | Ashlands | 60 | 3 | 6 | 0% | 188,12,640,960 | 0,0,1024,1536 | draft |  |
| `pika-01.png` | Hornpip | Mountain | 20 | 8 | 3 | 0% | 207,153,640,960 | 0,0,1024,1536 | draft |  |
| `platypus-01.png` | Runebill | Wet forest | 20 | 8 | 3 | 0% | 193,219,640,960 | 0,0,1024,1536 | draft |  |
| `plough-01.png` | Furrow Ravager | Savannah | 60 | 3 | 6 | 20% | 72,91,640,960 | 0,0,1024,1536 | draft |  |
| `pocketwatch-01.png` | Tickwarden | Ruins | 20 | 8 | 3 | 10% | 305,0,640,960 | 0,0,1024,1536 | draft |  |
| `porcupine-01.png` | Quillflask | Autumn woods | 20 | 8 | 3 | 20% | 152,151,640,960 | 0,0,1024,1536 | draft |  |
| `portcullis-01.png` | Portcullis Colossus | Cathedral | 60 | 3 | 6 | 40% | 180,53,640,960 | 0,0,1024,1536 | draft |  |
| `printingpress-01.png` | Typesetter | Ruins | 60 | 3 | 6 | 20% | 229,61,640,960 | 0,0,1024,1536 | draft |  |
| `prism-01.png` | Shardhex | Crystal caverns | 20 | 8 | 3 | 10% | 189,58,640,960 | 0,0,1024,1536 | draft |  |
| `puffin-01.png` | Shieldpuff | Arctic | 20 | 8 | 3 | 10% | 180,42,640,960 | 0,0,1024,1536 | draft |  |
| `quern-01.png` | Grindstone Oracle | Ruins | 40 | 5 | 5 | 20% | 218,0,640,960 | 0,0,1024,1536 | draft |  |
| `quokka-01.png` | Pebblegrin | Savannah | 20 | 8 | 3 | 0% | 119,132,640,960 | 0,0,1024,1536 | draft |  |
| `raccoon-01.png` | Potionpaw | Autumn woods | 20 | 8 | 3 | 0% | 180,124,640,960 | 0,0,1024,1536 | draft |  |
| `ram-01.png` | Sunhorn Paladin | Mountain | 40 | 5 | 5 | 10% | 192,77,640,960 | 0,0,1024,1536 | draft |  |
| `ram-02.png` | Bellows Ram | Mountain | 40 | 5 | 5 | 10% | 204,42,640,960 | 0,0,1024,1536 | draft |  |
| `ray-01.png` | Crystalray | Lagoon | 40 | 5 | 5 | 0% | 192,262,640,960 | 0,0,1024,1536 | draft |  |
| `reliquary-01.png` | Sealed Reliquary | Cathedral | 40 | 5 | 5 | 20% | 142,0,640,960 | 0,0,1024,1536 | draft |  |
| `retort-01.png` | Bubblehex Retort | Ruins | 20 | 8 | 3 | 10% | 304,43,640,960 | 0,0,1024,1536 | draft |  |
| `rhinoceros-01.png` | Shadowhorn | Savannah | 60 | 3 | 6 | 20% | 247,203,640,960 | 0,0,1024,1536 | draft |  |
| `roadrunner-01.png` | Whistledash | Desert | 20 | 8 | 3 | 0% | 249,0,640,960 | 0,0,1024,1536 | draft |  |
| `roc-01.png` | Thunder Roc | Storm cliffs | 60 | 3 | 6 | 0% | 189,73,640,960 | 0,0,1024,1536 | draft |  |
| `rooster-01.png` | Banner Cock | Savannah | 20 | 8 | 3 | 0% | 37,27,640,960 | 0,0,1024,1536 | draft |  |
| `sableantelope-01.png` | Pennant Sable | Savannah | 40 | 5 | 5 | 0% | 249,29,640,960 | 0,0,1024,1536 | draft |  |
| `safe-01.png` | Vaulthex | Ruins | 60 | 3 | 6 | 40% | 165,58,640,960 | 0,0,1024,1536 | draft |  |
| `sail-01.png` | Nightsail | Storm cliffs | 40 | 5 | 5 | 0% | 315,0,640,960 | 0,0,1024,1536 | draft |  |
| `salamander-01.png` | Slinkember | Ashlands | 20 | 8 | 3 | 0% | 192,475,640,960 | 0,0,1024,1536 | draft |  |
| `salamander-02.png` | Flaskember | Ashlands | 20 | 8 | 3 | 0% | 192,255,640,960 | 0,0,1024,1536 | draft |  |
| `sarcophagus-01.png` | Tombwarden | Desert | 60 | 3 | 6 | 30% | 250,9,640,960 | 0,0,1024,1536 | draft |  |
| `scarab-01.png` | Runeshell Scarab | Desert | 20 | 8 | 3 | 20% | 236,264,640,960 | 0,0,1024,1536 | draft |  |
| `scorpion-01.png` | Stingtail | Desert | 40 | 5 | 5 | 20% | 192,234,640,960 | 0,0,1024,1536 | draft |  |
| `scorpion-02.png` | Ribbonsting | Desert | 40 | 5 | 5 | 20% | 194,79,640,960 | 0,0,1024,1536 | draft |  |
| `sea-slug-01.png` | Sea Slug | Lagoon | 20 | 8 | 3 | 0% | 192,557,640,960 | 0,0,1024,1536 | draft |  |
| `seahorse-01.png` | Frostfin Seahorse | Arctic | 20 | 8 | 3 | 0% | 168,19,640,960 | 0,0,1024,1536 | draft |  |
| `seal-01.png` | Bottlenose Bard | Arctic | 40 | 5 | 5 | 0% | 292,148,640,960 | 0,0,1024,1536 | draft |  |
| `sealpress-01.png` | Waxseal Press | Cathedral | 40 | 5 | 5 | 20% | 238,0,640,960 | 0,0,1024,1536 | draft |  |
| `secretarybird-01.png` | Javelin Stalker | Savannah | 40 | 5 | 5 | 0% | 313,7,640,960 | 0,0,1024,1536 | draft |  |
| `serpent-01.png` | Crowned Serpent | Jungle | 60 | 3 | 6 | 10% | 264,0,640,960 | 0,0,1024,1536 | draft |  |
| `serval-01.png` | Boltear | Savannah | 20 | 8 | 3 | 0% | 318,0,640,960 | 0,0,1024,1536 | draft |  |
| `sewingbox-01.png` | Stitchkit | Ruins | 20 | 8 | 3 | 10% | 160,81,640,960 | 0,0,1024,1536 | draft |  |
| `sextant-01.png` | Admiral Starsight | Coast | 40 | 5 | 5 | 10% | 271,19,640,960 | 0,0,1024,1536 | draft |  |
| `shark-02.png` | Land Shark | Desert | 45 | 6 | 6 | 0% | 384,576,640,960 | 0,0,1024,1536 | draft |  |
| `shield-01.png` | Vanguard Aegis | Cathedral | 40 | 5 | 5 | 40% | 175,52,640,960 | 0,0,1024,1536 | draft |  |
| `shoebill-01.png` | Pikebill | Swamp | 40 | 5 | 5 | 0% | 96,0,640,960 | 0,0,1024,1536 | draft |  |
| `shrew-01.png` | Totemshrew | Pine forest | 20 | 8 | 3 | 0% | 218,116,640,960 | 0,0,1024,1536 | draft |  |
| `siphon-01.png` | Siphonworm | Ruins | 40 | 5 | 5 | 10% | 223,0,640,960 | 0,0,1024,1536 | draft |  |
| `sistrum-01.png` | Rattlechant | Desert | 20 | 8 | 3 | 10% | 172,0,640,960 | 0,0,1024,1536 | draft |  |
| `sledgehammer-01.png` | Sledge Colossus | Mountain | 60 | 3 | 6 | 20% | 189,30,640,960 | 0,0,1024,1536 | draft |  |
| `sloth-01.png` | Barkhold Sloth | Jungle | 40 | 5 | 5 | 10% | 202,183,640,960 | 0,0,1024,1536 | draft |  |
| `snail-01.png` | Prism Snail | Crystal caverns | 20 | 8 | 3 | 20% | 192,259,640,960 | 0,0,1024,1536 | draft |  |
| `snail-05.png` | Brewshell | Mushroom grotto | 20 | 8 | 3 | 20% | 119,205,640,960 | 0,0,1024,1536 | draft |  |
| `spider-01.png` | Crystal Weaver | Crystal caverns | 40 | 5 | 5 | 0% | 192,268,640,960 | 0,0,1024,1536 | draft |  |
| `spider-02.png` | Skitterknife | Jungle | 20 | 8 | 3 | 0% | 192,521,640,960 | 0,0,1024,1536 | draft |  |
| `spikeback-01.png` | Spikeback | Desert | 55 | 4 | 6 | 0% | 256,355,768,1152 | 0,0,1024,1536 | draft |  |
| `spindle-01.png` | Spindlewhirl | Ruins | 20 | 8 | 3 | 10% | 129,0,640,960 | 0,0,1024,1536 | draft |  |
| `squid-01.png` | Crystalink Squid | Lagoon | 40 | 5 | 5 | 0% | 170,175,640,960 | 0,0,1024,1536 | draft |  |
| `stag-01.png` | Shieldhorn Stag | Pine forest | 40 | 5 | 5 | 10% | 333,0,640,960 | 0,0,1024,1536 | draft |  |
| `starfish-01.png` | Starmend | Coast | 20 | 8 | 3 | 10% | 192,310,640,960 | 0,0,1024,1536 | draft |  |
| `stingray-01.png` | Coralmend Ray | Lagoon | 40 | 5 | 5 | 0% | 192,217,640,960 | 0,0,1024,1536 | draft |  |
| `stoat-01.png` | Needlewhisk | Arctic | 20 | 8 | 3 | 0% | 189,208,640,960 | 0,0,1024,1536 | draft |  |
| `sugarglider-01.png` | Sailglide | Jungle | 20 | 8 | 3 | 0% | 208,148,640,960 | 0,0,1024,1536 | draft |  |
| `sundial-01.png` | Noonward | Ruins | 40 | 5 | 5 | 20% | 69,12,640,960 | 0,0,1024,1536 | draft |  |
| `sundrum-01.png` | Sunhex Drum | Desert | 40 | 5 | 5 | 10% | 191,0,640,960 | 0,0,1024,1536 | draft |  |
| `tanuki-01.png` | Leafshift Tanuki | Autumn woods | 20 | 8 | 3 | 0% | 161,151,640,960 | 0,0,1024,1536 | draft |  |
| `tapestry-01.png` | Woven Host | Cathedral | 40 | 5 | 5 | 0% | 165,0,640,960 | 0,0,1024,1536 | draft |  |
| `tapir-01.png` | Orchid Tapir | Jungle | 40 | 5 | 5 | 0% | 144,195,640,960 | 0,0,1024,1536 | draft |  |
| `tarsier-01.png` | Maskeye | Jungle | 20 | 8 | 3 | 0% | 166,120,640,960 | 0,0,1024,1536 | draft |  |
| `teapot-01.png` | Volcano Teapot | Ashlands | 20 | 8 | 3 | 10% | 198,123,640,960 | 0,0,1024,1536 | draft |  |
| `termite-01.png` | Drillmite | Savannah | 20 | 8 | 3 | 20% | 210,116,640,960 | 0,0,1024,1536 | draft |  |
| `termite-02.png` | Clockmite | Savannah | 20 | 8 | 3 | 20% | 109,5,640,960 | 0,0,1024,1536 | draft |  |
| `thorntreant-01.png` | Bramblestalker | Wet forest | 60 | 3 | 6 | 10% | 237,0,640,960 | 0,0,1024,1536 | draft |  |
| `toad-01.png` | Boulder Toad | Mountain | 60 | 2 | 7 | 0% | 256,304,640,960 | 0,0,1024,1536 | draft |  |
| `toad-02.png` | Druid Toad | Swamp | 40 | 5 | 5 | 0% | 192,173,640,960 | 0,0,1024,1536 | draft |  |
| `toad-03.png` | Belltoad | Swamp | 40 | 5 | 5 | 0% | 280,272,640,960 | 0,0,1024,1536 | draft |  |
| `tortoise-01.png` | Mountainshell | Mountain | 60 | 3 | 6 | 40% | 229,82,640,960 | 0,0,1024,1536 | draft |  |
| `tortoise-02.png` | Obsidian Tortoise | Ashlands | 60 | 3 | 6 | 40% | 233,250,640,960 | 0,0,1024,1536 | draft |  |
| `treant-01.png` | Leafwarden | Wet forest | 60 | 3 | 6 | 10% | 230,35,640,960 | 0,0,1024,1536 | draft |  |
| `treant-02.png` | Elderbough | Wet forest | 60 | 3 | 6 | 10% | 190,0,640,960 | 0,0,1024,1536 | draft |  |
| `trowel-01.png` | Trowel Crusader | Ruins | 40 | 5 | 5 | 10% | 108,0,640,960 | 0,0,1024,1536 | draft |  |
| `tuningfork-01.png` | Resonant Champion | Cathedral | 40 | 5 | 5 | 10% | 256,0,640,960 | 0,0,1024,1536 | draft |  |
| `turbine-01.png` | Skyturbine | Storm cliffs | 40 | 5 | 5 | 20% | 161,0,640,960 | 0,0,1024,1536 | draft |  |
| `turtle-01.png` | Hornshell | Coast | 65 | 1 | 5 | 30% | 0,576,640,960 | 0,0,1024,1536 | draft |  |
| `turtle-02.png` | Shellmonk | Coast | 40 | 5 | 5 | 30% | 192,75,640,960 | 0,0,1024,1536 | draft |  |
| `turtle-03.png` | Frostshell Bard | Arctic | 40 | 5 | 5 | 30% | 251,259,640,960 | 0,0,1024,1536 | draft |  |
| `umbrella-01.png` | Fanbrella | Storm cliffs | 20 | 8 | 3 | 10% | 217,5,640,960 | 0,0,1024,1536 | draft |  |
| `velvetworm-01.png` | Runevelvet | Wet forest | 20 | 8 | 3 | 0% | 384,255,640,960 | 0,0,1024,1536 | draft |  |
| `vinegaroon-01.png` | Vinegar Flask | Desert | 20 | 8 | 3 | 10% | 384,150,640,960 | 0,0,1024,1536 | draft |  |
| `vise-01.png` | Vise Enforcer | Ruins | 40 | 5 | 5 | 30% | 250,225,640,960 | 0,0,1024,1536 | draft |  |
| `vulture-01.png` | Cinderbeak | Desert | 40 | 5 | 5 | 0% | 195,58,640,960 | 0,0,1024,1536 | draft |  |
| `walrus-01.png` | Tuskhorn Walrus | Arctic | 60 | 3 | 6 | 10% | 309,90,640,960 | 0,0,1024,1536 | draft |  |
| `warthog-01.png` | Cannon Warthog | Savannah | 40 | 5 | 5 | 20% | 221,9,640,960 | 0,0,1024,1536 | draft |  |
| `wasp-01.png` | Stingflask | Flower meadow | 20 | 8 | 3 | 0% | 190,87,640,960 | 0,0,1024,1536 | draft |  |
| `weasel-01.png` | Daggerwhisk | Autumn woods | 20 | 8 | 3 | 0% | 200,165,640,960 | 0,0,1024,1536 | draft |  |
| `weathervane-01.png` | Gustarrow | Storm cliffs | 40 | 5 | 5 | 10% | 161,6,640,960 | 0,0,1024,1536 | draft |  |
| `weaverbird-01.png` | Bannerweave | Savannah | 20 | 8 | 3 | 0% | 248,45,640,960 | 0,0,1024,1536 | draft |  |
| `werewolf-01.png` | Thornfang | Pine forest | 40 | 7 | 5 | 0% | 384,96,640,960 | 0,0,1024,1536 | draft |  |
| `windsock-01.png` | Galesock | Storm cliffs | 40 | 5 | 5 | 0% | 219,5,640,960 | 0,0,1024,1536 | draft |  |
| `wisp-01.png` | Shadowwisp | Ruins | 20 | 8 | 3 | 0% | 289,75,640,960 | 0,0,1024,1536 | draft |  |
| `wolf-01.png` | Ranger Wolf | Pine forest | 40 | 5 | 5 | 0% | 192,143,640,960 | 0,0,1024,1536 | draft |  |
| `wolverine-01.png` | Macefang Wolverine | Arctic | 40 | 5 | 5 | 10% | 154,208,640,960 | 0,0,1024,1536 | draft |  |
| `wombat-01.png` | Fistburrow | Savannah | 40 | 5 | 5 | 10% | 260,72,640,960 | 0,0,1024,1536 | draft |  |
| `wyvern-01.png` | Obsidian Wyvern | Ashlands | 60 | 3 | 6 | 10% | 187,104,640,960 | 0,0,1024,1536 | draft |  |
| `yak-01.png` | Gongyak | Mountain | 60 | 3 | 6 | 10% | 327,101,640,960 | 0,0,1024,1536 | draft |  |
