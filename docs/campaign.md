# Everkin — Campaign content

Every encounter, boss, reward, challenge encounter and achievement. Numbers (team Cost, boss stats, AI levels) are starting values for testing. The rules are in game_system.md §2.2 (tutorial, rewards, challenge encounters, coins), §7.6 and §12.3 (bosses, difficulty, enemy sides), §15 (chapters, setting) and production.md §1.2 (achievements); this file holds only content.

## How the campaign is built

- **One reward per encounter**, always a member of the defeated team, never repeated. The tutorial follows the adopted tutorial creatures (Tailwind Kite, Ripplequill, then Hornpip and Potionpaw), so those creatures appear in the tutorial enemy teams.
- **Chapters 1 and 2** walk through neighboring wild biomes; each battle uses creatures from its own biome. **Chapter 3** is the castle: the Ruins as its outer walls, the Cathedral as its halls and towers, with mostly living objects. **Chapter 4** has one dragon per battle in its home biome, and the strongest dragon, the Obsidian Elder, is the final boss.
- **Human recruits:** Tom Reed (chapter 1), Kofi Mensah (chapter 2), Edwin Marsh (chapter 3), and Ragnhild Eriksen as the reward of challenge C4, because every chapter 4 battle gives its dragon.
- **Difficulty** rises through the enemy rosters (team Cost from 14 to 51; enemy teams are not bound by the budget of 35) and the AI level: Easy in the tutorial and chapter 1, Normal in chapter 2 and the first half of chapter 3, Hard from then on. The tutorial's enemy sides have 2, 3 and 4 members, matching the player's growing team.
- **Formations** by role: Bulwark, Keeper and every creature with a melee or melee-reach attack in Front (melee needs the Front row, game_system §8.1); projectile and direct attackers and healers behind them. Tuned once the campaign is playable.
- **Bosses** are the strengthened version of a creature from their chapter: HP × 1.5 (rounded down to a multiple of 5) and Power + 2, everything else unchanged. Winning gives the normal version.
- **Names** are short storybook names. Chapter names are the only text the game shows besides encounter names (§15: no story text).

## Encounters

### Tutorial: The First Lanterns

Biomes: Autumn woods.

| # | Encounter | Biome | AI | Enemy team (Front · Middle · Rear) | Team Cost | Reward |
|---|---|---|---|---|---|---|
| T1 | Two by the Path | Autumn woods | Easy | Fox Bard · Tailwind Kite · — | 3 | Tailwind Kite |
| T2 | Ripples in the Leaves | Autumn woods | Easy | — · Caterpillar Bard · Ripplequill, Bellnap Dormouse | 6 | Ripplequill |
| T3 | The Hollow Oak | Autumn woods | Easy | — · Hornpip · Potionpaw, Dicetail, Bellnap Dormouse | 11 | Hornpip, Potionpaw |

### Chapter 1: The Lantern Woods

Biomes: Flower meadow, Mushroom grotto, Wet forest, Autumn woods, Pine forest.

| # | Encounter | Biome | AI | Enemy team (Front · Middle · Rear) | Team Cost | Reward |
|---|---|---|---|---|---|---|
| 1-1 | Humming Meadow | Flower meadow | Easy | Rapierbeak · — · Fiddlewhit, Firefly, Stingflask, Sunhum | 14 | Sunhum |
| 1-2 | The Glowing Hollow | Mushroom grotto | Easy | Gearclaw Mole, Hexhammer Mole, Ironroll · Brewshell · Sporedruid, Lanternmoth | 17 | Ironroll |
| 1-3 | Otter Falls | Wet forest | Easy | Captain Daggerbill, Glade Stag · Runebill, Runevelvet · Pumpwhisker, Featherveil Crane | 20 | Glade Stag |
| 1-4 | Bramble Hill | Autumn woods | Easy | Fox Bard, Quillflask, Drumbadger, Hexbristle · — · Dicetail | 19 | Drumbadger |
| 1-5 | The Archer's Clearing | Pine forest | Easy | Elsa Lindgren, Shieldhorn Stag · Plumesong · Totemshrew, Hootmage, Tom Reed | 26 | Tom Reed |
| 1-B | The Grey Ranger | Pine forest | Normal | **Ranger Wolf** (boss), Thornfang · Plumesong · Ivy Calloway, Hootmage, Totemshrew | 28 | Ranger Wolf |

Boss of 1-B: **Ranger Wolf** with HP 50 → 75 and Power 5 → 7; Speed 5, Cost, classes and skills unchanged. The reward is the normal version.

### Chapter 2: The Sunward Road

Biomes: Swamp, Jungle, Savannah, Desert.

| # | Encounter | Biome | AI | Enemy team (Front · Middle · Rear) | Team Cost | Reward |
|---|---|---|---|---|---|---|
| 2-1 | Reedmist Fen | Swamp | Normal | Axejaw Lizardfolk, Paul Fischer · Screamroot, Pikebill · Reedarrow, Druid Toad | 26 | Screamroot |
| 2-2 | Canopy Drums | Jungle | Normal | Glovegrin, Cleaver Komodo, Sicklecoil, Drumbill · Roarleaf, Vinelilt · — | 29 | Sicklecoil |
| 2-3 | The Shaman's Clearing | Jungle | Normal | Barkhold Sloth, Talonstrike · Harpoon Fossa · Kofi Mensah, Calmwater Capybara, Seedshot Scarab | 30 | Kofi Mensah |
| 2-4 | Long Grass | Savannah | Normal | Tuftspear, Pennant Sable, Ironhide Aurochs · Javelin Stalker · Sawgrin, Lucia Marin | 32 | Javelin Stalker |
| 2-5 | The Glass Dunes | Desert | Normal | Nadia Petrova, Armadillo Knight · Hoodspitter, Sandhex Glass, Stingtail · Aurelia Day | 33 | Sandhex Glass |
| 2-B | The Sunken Tomb | Desert | Normal | **Tombwarden** (boss) · Stonegaze Basilisk, Barbtail Manticore, Cinderbeak, Ribbonsting · — | 38 | Tombwarden |

Boss of 2-B: **Tombwarden** with HP 75 → 110 and Power 6 → 8; Speed 3, Cost, classes and skills unchanged. The reward is the normal version.

### Chapter 3: The Waking Castle

Biomes: Ruins (outer walls), Cathedral (halls and towers).

| # | Encounter | Biome | AI | Enemy team (Front · Middle · Rear) | Team Cost | Reward |
|---|---|---|---|---|---|---|
| 3-1 | The Fallen Gate | Ruins | Normal | Lockjaw Chest, Stitchfist Mannequin, Windup Drummer, Tickwarden · — · Quillspatter, Foldtome | 32 | Foldtome |
| 3-2 | The Toppled Workshop | Ruins | Normal | Siphonworm, Vise Enforcer, Bonecube, Twistcork · Discord Gong, Stitchkit · — | 36 | Discord Gong |
| 3-3 | The Great Hall | Cathedral | Normal | Wheezewarden, Waxbrawler · Keyclatter · Orbitwright, Grailward, Smokeswing | 37 | Grailward |
| 3-4 | The Knights' Gallery | Cathedral | Hard | Edwin Marsh, Vanguard Aegis, Resonant Champion, Johanna Weber · — · Thomas Abbott, Agnes Brook | 39 | Edwin Marsh |
| 3-5 | The Bell Tower | Cathedral | Hard | Clapperward, Chimespire · Stonewatch Gargoyle, Skybrass · Mirror Revenant, Candlecrown | 40 | Mirror Revenant |
| 3-B | The Castle's Heart | Cathedral | Hard | **Pipe Organ Choir** (boss), Portcullis Colossus, Woven Host, Waxseal Press · — · Sealed Reliquary, Orbitwright | 48 | Pipe Organ Choir |

Boss of 3-B: **Pipe Organ Choir** with HP 75 → 110 and Power 6 → 8; Speed 3, Cost, classes and skills unchanged. The reward is the normal version.

### Chapter 4: The Dragons' Wake

Biomes: Arctic, Storm cliffs, Ashlands.

| # | Encounter | Biome | AI | Enemy team (Front · Middle · Rear) | Team Cost | Reward |
|---|---|---|---|---|---|---|
| 4-1 | The Frozen Shelf | Arctic | Hard | Glacier Titan, Tundra Titan, Bucklerhorn, Ingrid Lund, Frostshell Bard · — · Astor Nilsen | 44 | Glacier Titan |
| 4-2 | The Thunder Ledge | Storm cliffs | Hard | Thunder Roc, Richard Novak · Tempest Sovereign · Skyturbine, Gustarrow, Stormgoblet | 42 | Tempest Sovereign |
| 4-3 | The Ember Fields | Ashlands | Hard | Magma Hog, Cinderhorn, Obsidian Wyvern · Emberwyrm Colossus · Carmen Costa, Solar Phoenix | 44 | Emberwyrm Colossus |
| 4-B | The Obsidian Throne | Ashlands | Hard | Furnace Titan, Kiln Behemoth, Furnaceheart Colossus · **Obsidian Elder** (boss), Puffing Forgedrake, Coalwraith Brazier · — | 51 | Obsidian Elder |

Boss of 4-B: **Obsidian Elder** with HP 75 → 110 and Power 6 → 8; Speed 3, Cost, classes and skills unchanged. The reward is the normal version.

### Challenge encounters: After the story

Biomes: Lagoon, Coast, Crystal caverns, Mountain.

| # | Encounter | Biome | AI | Enemy team (Front · Middle · Rear) | Team Cost | Reward |
|---|---|---|---|---|---|---|
| C1 | The Moonlit Lagoon | Lagoon | Hard | Basalt Kraken, Reef Warden · Crystalfin Dugong, Hookgrin Moray · Shellmend Manatee, Mateo Santos | 42 | Basalt Kraken |
| C2 | Wreckers' Cove | Coast | Hard | Barnacle Bulwark, Shellmonk, Crab Smith · Lighthouse Wakelamp · Admiral Starsight, Carlos Fonseca | 41 | Lighthouse Wakelamp |
| C3 | The Under-Vault | Crystal caverns | Hard | Crystalback Drake, Shardhex · — · Kai Lindqvist, Lukas Weiss, Mira Castell, Prism Snail | 33 | Crystalback Drake |
| C4 | Ragnhild's Pass | Mountain | Hard | Ragnhild Eriksen, Ironbound Behemoth, Mountainshell, Lancewing Griffin, Sunhorn Paladin · — · Henrik Berg | 45 | Ragnhild Eriksen |

Rewards: 30 creatures from encounters (4 in the tutorial, 22 in chapters 1–4, 4 from challenge encounters), plus the 2 starting creatures. The other 287 creatures are coin creatures.

**Challenge encounters** appear after the story, one on each chapter's map: C1 on chapter 1's (the lagoon beyond the wet forest), C2 on chapter 2's (the coast past the desert), C3 on chapter 3's (the crystal caverns under the castle), C4 on chapter 4's (the mountain pass toward the arctic). They give only their creature, no coins (§2.2).

## Achievements

About 30, all earnable offline, none hidden, no goal above 100 wins (production.md §1.2). Collection goals stay within the 32 creatures an offline player can own (2 starters, 26 campaign rewards, 4 challenge rewards), because coins need a connection.

| # | Name | Goal |
|---|---|---|
| 1 | First Lanterns | Finish the tutorial. |
| 2 | Through the Woods | Finish chapter 1. |
| 3 | The Sunward Road | Finish chapter 2. |
| 4 | The Castle Sleeps | Finish chapter 3. |
| 5 | Dragonwake | Finish chapter 4, the end of the story. |
| 6 | Steady Hands | Beat a chapter boss without any of your creatures being KO'd. |
| 7 | Challenger | Win all four challenge encounters. |
| 8 | A Small Band | Own 10 creatures. |
| 9 | A Growing Band | Own 20 creatures. |
| 10 | A Full Lantern | Own 30 creatures. |
| 11 | Dragon Keeper | Own all four dragons. |
| 12 | Every Calling | Own at least one creature of each of the 28 classes. |
| 13 | Sparring | Win 10 free or random battles. |
| 14 | Seasoned | Win 50 free or random battles. |
| 15 | Veteran | Win 100 free or random battles. |
| 16 | Hard Lessons | Win a random battle on Hard. |
| 17 | Hardened | Win 10 random battles on Hard. |
| 18 | Friends at the Table | Finish a local two-player battle. |
| 19 | Untouched | Win a battle without any of your creatures being KO'd. |
| 20 | Last One Standing | Win a battle with only one of your original creatures still standing. |
| 21 | Small but Mighty | Win a random battle on Hard with a team of total Cost 25 or less. |
| 22 | Three's Company | Win a campaign battle in chapter 2 or later with three or fewer creatures. |
| 23 | Back from the Dark | Revive a KO'd creature. |
| 24 | Swarm | Have six summons on your side at the same time. |
| 25 | Unseen | KO an enemy with an attack made from stealth. |
| 26 | Domino | Push or pull an enemy into another unit so that it bumps. |
| 27 | Chain Reaction | Make poison gas explode. |
| 28 | Nine Ways | Win a battle with 12 different classes on your team. |

When earned frames return (production.md §1.2, postponed), the harder ones (6, 7, 15, 17, 21) are the candidates to give one.

## Notes

- **Chapter 4** has four battles instead of "about five", because there are four dragons.
- **Lagoon, Coast, Crystal caverns and Mountain** appear only in the challenge encounters. Free battles use biomes the player has reached, so their backgrounds show up only after those challenges.
- **Coin creatures.** 287 of 319 creatures are coin creatures, following the roster split, so an offline player sees 32 creatures at most.
