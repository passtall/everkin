# Creature Roster

Every creature that has art: its unit name, display name, two classes, two keystones and unique attack ([class_system.md](class_system.md)), and below, its biome, stats, Cost and card crops. Skill and keystone texts are in [skills.md](skills.md).

**Roster table.** Keystones are listed in the order of the classes (one from each). The attack columns give its delivery, Delay / Momentum, animation style and effect at Power 5. "Direct, melee reach" attacks are breaths and pulses: they need the creature in its Front row and hit the enemy Front row (game_system §8.1).

**Art pipeline.** `tools/art/clean_creatures.py` reads each row's **Source** in `source-art/creatures/` and writes the cleaned copy to `assets/creatures/` under the **Art file** name. Art added straight to `assets/creatures/` has no original there; its Source is —. Art file names are `{species}-{nn}.png`, with a two-digit counter per species.

**Unit names** follow the schema below. The unit name is the creature's stable ID in code, content files and saves. Creatures named before the schema (`stag-glade`, `turtle-hornshell`, `boar-magma`) never take a counter.

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
4. Look-alikes get the same classes and a similar attack unless the art clearly shows a different fighting style.

| Unit | Name | Classes | Keystones | Attack | Delivery | D / M | Style | Attack effect | Art file | Source |
|---|---|---|---|---|---|---|---|---|---|---|
| `abacus-strategist` | Tallymarch | Tactician, Commander | All In, Drill | Abacus Headbutt | Melee | 3 / 0 | lunge | Hit for 4–5. | `abacus-01.png` | — |
| `accordion-warlord` | Wheezewarden | Bard, Berserker | Grand Chorus, Reckless Swing | Accordion Slam | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `accordion-01.png` | — |
| `anchor-sentinel` | Barnacle Bulwark | Bulwark, Keeper | Shield Wall, Thorned Ward | Anchor Bash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `anchor-01.png` | — |
| `ankylosaur-guardian-club` | Clubtail Grumbleback | Bulwark, Berserker | Hold Position, Last Stand | Club Clobber | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `ankylosaur-01.png` | — |
| `ant-engineer-gear` | Cogmarch Ant | Engineer, Chemist | Reinforced, Showman | Gear Fling | Projectile | 3 / 0 | shot | Hit for 3–4. | `ant-01.png` | — |
| `armadillo-knight-2` | Armadillo Recruit | Bulwark, Keeper | Unbreakable, Steadfast Ward | Armadillo Thump | Melee | 4 / 0 | swing | Hit for 5–6. | `armadillo-01.png` | `armadillo-knight-2.png` |
| `armadillo-knight` | Armadillo Knight | Bulwark, Keeper | Shield Wall, Twin Wards | Armadillo Clobber | Melee | 4 / 0 | swing | Hit for 5–6. | `armadillo-02.png` | `armadillo-knight.png` |
| `astrolabe-judge` | Starwright Judge | Enchanter, Commander | Twin Enchant, Rally | Astrolabe Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `astrolabe-01.png` | — |
| `aurochs-bodyguard-plate` | Ironhide Aurochs | Berserker, Champion | Unstoppable, Hunt Down | Plate Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `aurochs-01.png` | — |
| `axolotl-mage-water` | Ripplequill | Cryomancer, Enchanter | Cold Heart, Lingering Magic | Water Spark | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: push the target one row back. | `axolotl-01.png` | — |
| `ayeaye-hexer-candle` | Candlefinger | Hexer, Thief | Witch's Patience, Escape Artist | Candle Burst | Projectile | 4 / 0 | bolt | Hit for 3–4; heals the attacker for 1. | `ayeaye-01.png` | — |
| `badger-summoner-drum` | Drumbadger | Summoner, Bard | Golem Heart, Rhythm | Drum Thump | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `badger-01.png` | — |
| `ballast-bodyguard` | Sandbag Sentinel | Bulwark, Skyborne | Hold Position, Sky Lord | Ballast Crush | Melee | 4 / 0 | swing | Hit for 5–6. | `ballast-01.png` | — |
| `basilisk-sorcerer-gaze` | Stonegaze Basilisk | Berserker, Thief | Reckless Swing, Master Thief | Gaze Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Stunned for 5 time units. | `basilisk-01.png` | — |
| `bassoon-hexer-requiem` | Dirgewind | Bard, Commander | Crescendo, Vanguard | Requiem Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2; heals the attacker for 1. | `bassoon-01.png` | — |
| `bat-summoner-moon` | Moonwing | Skyborne, Summoner | Lift, Portal | Moon Burst | Projectile | 4 / 0 | bolt | Hit for 3–4; heals the attacker for 1. | `bat-01.png` | — |
| `bat-rogue-shadow` | Bat Rogue | Assassin, Skyborne | Unseen, Eagle Eye | Shadow Carve | Melee | 3 / 0 | swing | Hit for 3–4; cannot be intercepted. | `bat-02.png` | `bat-rogue-shadow.png` |
| `baton-marshal` | Marshal Tempo | Commander, Tactician | Drill, Inspiring Presence | Baton Poke | Melee | 3 / 0 | lunge | Hit for 4–5. | `baton-01.png` | — |
| `beacon-summoner` | Lighthouse Wakelamp | Skyborne, Summoner | Sky Lord, Quickening | Beacon Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `beacon-01.png` | — |
| `beaver-healer-pump` | Pumpwhisker | Engineer, Mender | Self-Repair, Overflowing Care | Pump Flick | Projectile | 3 / 0 | shot | Hit for 3–4. | `beaver-01.png` | — |
| `bee-shaman-sun` | Sunhum | Totemist, Skyborne | Great Totem, Eagle Eye | Sun Missile | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: burning ground on the target's spot. | `bee-01.png` | — |
| `beetle-2` | Beetle Recruit | Bulwark, Berserker | Unbreakable, Last Stand | Beetle Butt | Melee | 3 / 0 | lunge | Hit for 4–5. | `beetle-01.png` | `beetle-2.png` |
| `beetle-knight-shield` | Beetle Guardian | Bulwark, Champion | Shield Wall, Duelist | Shield Bash | Melee | 4 / 0 | swing | Hit for 5–6. | `beetle-02.png` | `beetle-knight-shield.png` |
| `beetle-engineer-seed` | Seedshot Scarab | Chemist, Ritualist | Potent Brew, Deluge | Seed Volley | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: the target is Immobilized for 5 time units. | `beetle-04.png` | — |
| `behemoth-titan-iron` | Ironbound Behemoth | Bulwark, Stormcaller | Hold Position, Conductor | Iron Kick | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `behemoth-01.png` | — |
| `bell-guardian-clapper` | Clapperward | Bard, Summoner | Grand Chorus, Golem Heart | Clapper Clobber | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `bell-01.png` | — |
| `bellows-drake` | Puffing Forgedrake | Engineer, Champion | Rapid Fire, Glory | Bellows Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `bellows-01.png` | — |
| `binturong-bard-vine` | Vinelilt | Bard, Ritualist | Rhythm, Tidecaller | Vine Lash | Melee | 3 / 0 | swing | Hit for 2–3; 20%: pull the target one row toward the battle line. | `binturong-01.png` | — |
| `bloomspear` | Bloomspear | Spiritwalker, Champion | Soul Anchor, Hunt Down | Bloomspear Lunge | Melee | 3 / 0 | lunge | Hit for 4–5. | `bloomspear-01.png` | `7_f.png` |
| `boar-magma` | Magma Hog | Berserker, Champion | Unstoppable, Duelist | Boar Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: burning ground on the target's spot. | `boar-01.png` | `10_f.png` |
| `boar-guard-hex` | Hexbristle | Bulwark, Hexer | Unbreakable, Lingering Malice | Hex Butt | Melee | 3 / 0 | lunge | Hit for 4–5; heals the attacker for 1. | `boar-02.png` | — |
| `bombardierbeetle-alchemist-acid` | Acidpop | Chemist, Saboteur | Volatile, Wrecker | Acid Throw | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `bombardierbeetle-01.png` | — |
| `book-summoner-fold` | Foldtome | Summoner, Spiritwalker | Portal, Haunting | Fold Bolt | Projectile | 4 / 0 | bolt | Hit for 3–4. | `book-01.png` | — |
| `brazier-summoner-coal` | Coalwraith Brazier | Summoner, Pyromancer | Quickening, Wildfire | Coal Pulse | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: burning ground on the target's spot. | `brazier-01.png` | — |
| `buffalo-guard-root` | Rootyoke Buffalo | Bulwark, Ritualist | Shield Wall, Frostbinder | Root Snap | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Immobilized for 5 time units. | `buffalo-01.png` | — |
| `bugle-skyrider` | Skybrass | Commander, Skyborne | Rally, Lift | Bugle Plume | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `bugle-01.png` | — |
| `candelabrum-warrior-wax` | Waxbrawler | Berserker, Transmuter | Reckless Swing, Lasting Change | Wax Jab | Melee | 3 / 0 | lunge | Hit for 4–5. | `candelabrum-01.png` | — |
| `cannon-marauder` | Powderbelch | Berserker, Tracker | Last Stand, Sniff the Air | Cannon Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `cannon-01.png` | — |
| `capybara-guard-calm` | Calmwater Capybara | Lifebinder, Summoner | Martyr, Golem Heart | Calm Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: push the target one row back. | `capybara-01.png` | — |
| `caracal-hunter-spear` | Tuftspear | Enchanter, Thief | Spell Ward, Shadow Purse | Spear Bite | Melee | 3 / 0 | lunge | Hit for 4–5. | `caracal-01.png` | — |
| `carillon-summoner-chime` | Chimespire | Bard, Keeper | Crescendo, Thorned Ward | Chime Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `carillon-01.png` | — |
| `cassowary-captain-horn` | Crestcaller | Bard, Berserker | Grand Chorus, Unstoppable | Horn Breath | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `cassowary-01.png` | — |
| `caterpillar-bard` | Caterpillar Bard | Bard, Commander | Rhythm, Vanguard | Caterpillar Breath | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `caterpillar-01.png` | `caterpillar-bard.png` |
| `catfolk-spellblade-moon` | Moonclaw Spellblade | Champion, Thief | Glory, Escape Artist | Moon Slash | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `catfolk-01.png` | — |
| `centipede-assassin-sickle` | Sicklecoil | Assassin, Champion | Vanishing Kill, Hunt Down | Sickle Cleave | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `centipede-01.png` | — |
| `chalice-of-storms` | Stormgoblet | Cryomancer, Skyborne | Brittle, Sky Lord | Storms Bolt | Projectile | 4 / 0 | bolt | Hit for 2–3; the target's next turn comes 2 time units later. | `chalice-01.png` | — |
| `chameleon-trickster-tongue` | Shiftscale | Saboteur, Tracker | Chaos Agent, Scent of Blood | Tongue Flick | Melee | 3 / 0 | swing | Hit for 2–3; 20%: pull the target one row toward the battle line. | `chameleon-01.png` | — |
| `chandelier-summoner-candle` | Candlecrown | Necromancer, Skyborne | Lich Form, Eagle Eye | Candle Spark | Projectile | 4 / 0 | bolt | Hit for 3–4; heals the attacker for 1. | `chandelier-01.png` | — |
| `chest-bodyguard-lock` | Lockjaw Chest | Bulwark, Engineer | Hold Position, Reinforced | Lock Bite | Melee | 3 / 0 | lunge | Hit for 4–5. | `chest-01.png` | — |
| `chinchilla-summoner-ocarina` | Ocarina Puff | Bard, Summoner | Crescendo, Portal | Ocarina Bolt | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `chinchilla-01.png` | — |
| `cicada-bard-chime` | Chimewing Cicada | Enchanter, Mender | Twin Enchant, Regrowth | Chime Missile | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `cicada-01.png` | — |
| `cinderhorn` | Cinderhorn | Champion, Berserker | Duelist, Reckless Swing | Cinderhorn Slash | Melee | 3 / 0 | swing | Hit for 3–4; 20%: the target is Confused for 5 time units. | `cinderhorn-01.png` | `8_f.png` |
| `coati-trickster-dice` | Diceroll Coati | Saboteur, Thief | Disarm Trap, Master Thief | Dice Fling | Projectile | 3 / 0 | shot | Hit for 2–3; the target's next turn comes 2 time units later. | `coati-01.png` | — |
| `cobra` | Hoodspitter | Thief, Saboteur | Shadow Purse, Wrecker | Cobra Spray | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `cobra-01.png` | `cobra.png` |
| `compass-hunter-needle` | Northneedle | Assassin, Engineer | Venom Blades, Self-Repair | Needle Cut | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `compass-02.png` | — |
| `conchphone-bard-horn` | Conchhorn | Bard, Commander | Grand Chorus, Drill | Horn Spray | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `conchphone-01.png` | — |
| `corkscrew-stalker` | Twistcork | Assassin, Engineer | Unseen, Rapid Fire | Corkscrew Cut | Melee | 3 / 0 | swing | Hit for 4–5. | `corkscrew-01.png` | — |
| `crab-alchemist` | Crab Alchemist | Chemist, Engineer | Showman, Reinforced | Crab Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `crab-01.png` | `crab-alchemist.png` |
| `crab-smith` | Crab Smith | Engineer, Berserker | Self-Repair, Last Stand | Crab Crush | Melee | 4 / 0 | swing | Hit for 5–6. | `crab-02.png` | `crab-smith.png` |
| `crab-guard-decoy` | Decoy Crab | Bulwark, Keeper | Unbreakable, Twin Wards | Decoy Headbutt | Melee | 3 / 0 | lunge | Hit for 4–5. | `crab-03.png` | — |
| `crane-hoist-titan` | Hoist Titan | Mender, Engineer | Triage, Rapid Fire | Titan Rake | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `crane-01.png` | — |
| `crane-summoner-feather` | Featherveil Crane | Summoner, Stormcaller | Quickening, Static Field | Feather Burst | Projectile | 4 / 0 | bolt | Hit for 3–4. | `crane-02.png` | — |
| `cricket-bard-fiddle` | Fiddlewhit | Bard, Commander | Rhythm, Rally | Fiddle Missile | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `cricket-01.png` | — |
| `crocodile-shaman` | Crocodile Shaman | Totemist, Berserker | Sturdy Totems, Unstoppable | Crocodile Jab | Melee | 3 / 0 | lunge | Hit for 4–5. | `crocodile-01.png` | `crocodile-shaman.png` |
| `crow-necromancer-skull` | Skullcaw | Necromancer, Skyborne | Bone Armor, Lift | Skull Bolt | Projectile | 4 / 0 | bolt | Hit for 3–4; heals the attacker for 1. | `crow-01.png` | — |
| `crucible-summoner` | Moltencup | Bulwark, Keeper | Shield Wall, Steadfast Ward | Crucible Pulse | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `crucible-01.png` | — |
| `cuttlefish-assassin-needle` | Needleglow | Assassin, Thief | Vanishing Kill, Escape Artist | Needle Carve | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `cuttlefish-01.png` | — |
| `stag-glade` | Glade Stag | Ritualist, Totemist | Frostbinder, Great Totem | Stag Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Immobilized for 5 time units. | `deer-01.png` | `26_f.png` |
| `dormouse-summoner-bell` | Bellnap Dormouse | Summoner, Enchanter | Golem Heart, Lingering Magic | Bell Spark | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `dormouse-01.png` | — |
| `dragon-colossus-ember` | Emberwyrm Colossus | Champion, Skyborne | Glory, Sky Lord | Ember Breath | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: burning ground on the target's spot. | `dragon-01.png` | — |
| `dragon-elder-obsidian` | Obsidian Elder | Commander, Skyborne | Vanguard, Eagle Eye | Obsidian Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `dragon-02.png` | — |
| `dragon-sovereign-tempest` | Tempest Sovereign | Commander, Skyborne | Drill, Lift | Tempest Plume | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; also hits one unit next to the target for 1–2. | `dragon-03.png` | — |
| `dragon-titan-glacier` | Glacier Titan | Cryomancer, Bulwark | Steam Burst, Hold Position | Glacier Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; the target's next turn comes 2 time units later. | `dragon-04.png` | — |
| `dragonfly-scout-bow` | Reedarrow | Skyborne, Tracker | Sky Lord, Relentless | Bow Toss | Projectile | 3 / 0 | shot | Hit for 3–4. | `dragonfly-01.png` | — |
| `drake-titan-crystal` | Crystalback Drake | Stormcaller, Transmuter | Eye of the Storm, Brittle Glass | Crystal Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `drake-01.png` | — |
| `drawbridge-bodyguard-chain` | Chaingate | Bulwark, Keeper | Unbreakable, Thorned Ward | Chain Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: pull the target one row toward the battle line. | `drawbridge-01.png` | — |
| `drum-summoner-war` | Wardrum | Bard, Berserker | Crescendo, Reckless Swing | War Throb | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Confused for 5 time units. | `drum-01.png` | — |
| `dugong-guard-crystal` | Crystalfin Dugong | Transmuter, Stormcaller | Mirror Form, Conductor | Crystal Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `dugong-02.png` | — |
| `elephant-summoner-idol` | Idolbearer | Summoner, Totemist | Portal, Spirit Shield | Idol Bite | Melee | 3 / 0 | lunge | Hit for 4–5. | `elephant-01.png` | — |
| `ferret-summoner-mask` | Maskwhisk | Thief, Summoner | Master Thief, Quickening | Mask Bite | Melee | 3 / 0 | lunge | Hit for 3–4; cannot be intercepted. | `ferret-01.png` | — |
| `fiddlercrab-trickster-coin` | Coinclaw | Saboteur, Engineer | Chaos Agent, Reinforced | Coin Slam | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `fiddlercrab-01.png` | — |
| `firefly-summoner-lantern` | Lanternwing | Skyborne, Summoner | Lift, Golem Heart | Lantern Pulse | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: burning ground on the target's spot. | `firefly-01.png` | — |
| `firefly` | Firefly | Skyborne, Mender | Eagle Eye, Overflowing Care | Firefly Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: burning ground on the target's spot. | `firefly-02.png` | `firefly.png` |
| `flyingfish-scout-dart` | Dartfin | Skyborne, Assassin | Sky Lord, Venom Blades | Dart Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `flyingfish-01.png` | — |
| `fossa-hunter-harpoon` | Harpoon Fossa | Tracker, Thief | Sniff the Air, Shadow Purse | Harpoon Jab | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: pull the target one row toward the battle line. | `fossa-01.png` | — |
| `fox-bard` | Fox Bard | Bard, Thief | Grand Chorus, Escape Artist | Fox Butt | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `fox-01.png` | `fox-bard.png` |
| `frigatebird-bard-throat` | Throatdrum | Bard, Skyborne | Rhythm, Eagle Eye | Throat Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `frigatebird-01.png` | — |
| `frog-bard-dagger` | Frog Bard Dagger | Bard, Assassin | Crescendo, Unseen | Dagger Hack | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `frog-01.png` | `frog-bard-dagger.png` |
| `furnace-titan` | Furnace Titan | Champion, Bulwark | Hunt Down, Shield Wall | Furnace Spray | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: burning ground on the target's spot. | `furnace-01.png` | — |
| `gargoyle-sentinel-stone` | Stonewatch Gargoyle | Skyborne, Tracker | Lift, Scent of Blood | Stone Poke | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `gargoyle-01.png` | — |
| `gaur-bodyguard-armor` | Bolted Gaur | Bulwark, Berserker | Hold Position, Last Stand | Armor Stomp | Melee | 3 / 0 | lunge | Hit for 4–5. | `gaur-01.png` | — |
| `gecko-alchemist-resin` | Resinpaw | Chemist, Thief | Potent Brew, Master Thief | Resin Throw | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `gecko-01.png` | — |
| `gecko-summoner-prism` | Prismtoe | Summoner, Transmuter | Portal, Lasting Change | Prism Bolt | Projectile | 4 / 0 | bolt | Hit for 3–4. | `gecko-02.png` | — |
| `goat-shaman-totem` | Totemhorn | Totemist, Lifebinder | Great Totem, Phoenix | Totem Jab | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `goat-01.png` | — |
| `goblet-paladin` | Grailward | Cleric, Champion | Sanctity, Duelist | Goblet Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `goblet-01.png` | — |
| `golem-colossus-furnace` | Furnaceheart Colossus | Berserker, Bulwark | Unstoppable, Unbreakable | Furnace Claw | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: burning ground on the target's spot. | `golem-01.png` | — |
| `golem-engineer-gear` | Gearwright Golem | Engineer, Bulwark | Self-Repair, Shield Wall | Gear Jab | Melee | 3 / 0 | lunge | Hit for 4–5. | `golem-02.png` | — |
| `gong-hexer-dissonance` | Discord Gong | Bard, Necromancer | Grand Chorus, Death's Bounty | Dissonance Throb | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3; heals the attacker for 1. | `gong-01.png` | — |
| `gorilla-bodyguard-fists` | Ironfist Silverback | Bulwark, Berserker | Hold Position, Reckless Swing | Fists Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `gorilla-01.png` | — |
| `griffin-knight-lance` | Lancewing Griffin | Skyborne, Champion | Sky Lord, Glory | Lance Lunge | Melee | 3 / 0 | lunge | Hit for 4–5. | `griffin-01.png` | — |
| `harpsichord-bard-keys` | Keyclatter | Bard, Commander | Rhythm, Rally | Keys Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Confused for 5 time units. | `harpsichord-01.png` | — |
| `harpy-eagle-scout-claws` | Talonstrike | Skyborne, Tracker | Eagle Eye, Relentless | Claws Headbutt | Melee | 3 / 0 | lunge | Hit for 3–4; 30%: the target Bleeds. | `harpy-eagle-01.png` | — |
| `hawk-scout-bow` | Skybow Hawk | Tracker, Skyborne | Sniff the Air, Lift | Bow Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `hawk-01.png` | — |
| `hermitcrab-sentinel-basalt` | Basaltshell | Bulwark, Keeper | Unbreakable, Twin Wards | Basalt Headbutt | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `hermitcrab-01.png` | — |
| `heron-engineer-wing` | Wingwright Heron | Engineer, Skyborne | Rapid Fire, Sky Lord | Wing Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `heron-01.png` | — |
| `heron-hunter-harpoon` | Stillspear | Skyborne, Champion | Eagle Eye, Hunt Down | Harpoon Skewer | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: pull the target one row toward the battle line. | `heron-02.png` | — |
| `hippo-healer-potion` | Tonic Hippo | Mender, Chemist | Regrowth, Volatile | Potion Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 30%: the target is Poisoned for 20 time units. | `hippo-01.png` | — |
| `hornbill-bard-drum` | Drumbill | Bard, Skyborne | Crescendo, Lift | Drum Crush | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `hornbill-01.png` | — |
| `horseshoe-champion` | Lucky Ironshoe | Champion, Bulwark | Duelist, Shield Wall | Horseshoe Butt | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `horseshoe-01.png` | — |
| `horseshoecrab-sentinel-bronze` | Bronzeback Sentinel | Bulwark, Stormcaller | Hold Position, Static Field | Bronze Gore | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `horseshoecrab-01.png` | — |
| `hourglass-hexer-sand` | Sandhex Glass | Hexer, Commander | Spreading Curse, Vanguard | Sand Pulse | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3; heals the attacker for 1. | `hourglass-01.png` | — |
| `howlermonkey-bard-roar` | Roarleaf | Bard, Commander | Grand Chorus, Drill | Roar Spray | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Immobilized for 5 time units. | `howlermonkey-01.png` | — |
| `human-alchemist-shield` | Gerald Hollins | Chemist, Bulwark | Potent Brew, Unbreakable | Shield Fling | Projectile | 3 / 0 | shot | Hit for 3–4. | `human-01.png` | `human-alchemist-shield.png` |
| `human-alchemist` | Maggie Wren | Chemist, Thief | Showman, Shadow Purse | Alchemist Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `human-02.png` | `human-alchemist.png` |
| `human-archer-crystal` | Mira Castell | Tracker, Transmuter | Relentless, Brittle Glass | Crystal Flick | Projectile | 3 / 0 | shot | Hit for 3–4. | `human-03.png` | `human-archer-crystal.png` |
| `human-archer-frost` | Sigrid Halvorsen | Tracker, Cryomancer | Sniff the Air, Cold Heart | Frost Toss | Projectile | 3 / 0 | shot | Hit for 2–3; the target's next turn comes 2 time units later. | `human-04.png` | `human-archer-frost.png` |
| `human-archer-gear` | Otto Brenner | Tracker, Engineer | Scent of Blood, Reinforced | Gear Throw | Projectile | 3 / 0 | shot | Hit for 3–4. | `human-05.png` | `human-archer-gear.png` |
| `human-archer-sun` | Lucia Marin | Tracker, Cleric | Relentless, Purifying Aura | Sun Toss | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: burning ground on the target's spot. | `human-06.png` | `human-archer-sun.png` |
| `human-archer` | Tom Reed | Tracker, Spiritwalker | Scent of Blood, Puppeteer | Archer Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `human-07.png` | `human-archer.png` |
| `human-barbarian-frost` | Ragnhild Eriksen | Berserker, Enchanter | Unstoppable, Spell Ward | Frost Carve | Melee | 3 / 0 | swing | Hit for 3–4; the target's next turn comes 2 time units later. | `human-08.png` | `human-barbarian-frost.png` |
| `human-barbarian` | Gunnar Holt | Berserker, Champion | Last Stand, Glory | Barbarian Slash | Melee | 3 / 0 | swing | Hit for 4–5. | `human-09.png` | `human-barbarian.png` |
| `human-bard-shield` | Felix Lowell | Bard, Berserker | Crescendo, Reckless Swing | Shield Smash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `human-10.png` | `human-bard-shield.png` |
| `human-bard` | Lorenzo Varga | Bard, Commander | Rhythm, Rally | Bard Burst | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `human-11.png` | `human-bard.png` |
| `human-brawler-hook` | Jack Morrow | Berserker, Tracker | Last Stand, Sniff the Air | Hook Smash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: pull the target one row toward the battle line. | `human-12.png` | `human-brawler-hook.png` |
| `human-captain-banner` | Helena Strand | Commander, Champion | Vanguard, Hunt Down | Banner Lunge | Melee | 3 / 0 | lunge | Hit for 4–5. | `human-13.png` | `human-captain-banner.png` |
| `human-captain-pistol` | Carlos Fonseca | Commander, Thief | Drill, Escape Artist | Pistol Fling | Projectile | 3 / 0 | shot | Hit for 3–4. | `human-14.png` | `human-captain-pistol.png` |
| `human-captain-rapier` | Beatrix Alvarez | Enchanter, Thief | Twin Enchant, Master Thief | Rapier Hack | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `human-15.png` | `human-captain-rapier.png` |
| `human-cleric-lantern` | Agnes Brook | Cleric, Engineer | Judgment, Self-Repair | Lantern Bolt | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-16.png` | `human-cleric-lantern.png` |
| `human-dancer-spear` | Amara Okafor | Champion, Stormcaller | Duelist, Eye of the Storm | Spear Poke | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `human-17.png` | `human-dancer-spear.png` |
| `human-dancer` | Nadia Petrova | Thief, Tracker | Shadow Purse, Scent of Blood | Dancer Cleave | Melee | 3 / 0 | swing | Hit for 3–4; 20%: the target is Confused for 5 time units. | `human-18.png` | `human-dancer.png` |
| `human-druid-leaf` | Fiona Ashby | Ritualist, Lifebinder | Tidecaller, Martyr | Leaf Ray | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: burning ground on the target's spot. | `human-19.png` | `human-druid-leaf.png` |
| `human-druid` | Ivy Calloway | Ritualist, Lifebinder | Deluge, Lifeblood | Druid Spark | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-20.png` | `human-druid.png` |
| `human-duelist-rapier` | Camille Moreau | Champion, Thief | Glory, Escape Artist | Rapier Carve | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `human-21.png` | `human-duelist-rapier.png` |
| `human-engineer-crystal` | Lukas Weiss | Engineer, Stormcaller | Rapid Fire, Conductor | Crystal Spark | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-22.png` | `human-engineer-crystal.png` |
| `human-engineer-hook` | Max Hooper | Saboteur, Tracker | Disarm Trap, Relentless | Hook Shot | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: pull the target one row toward the battle line. | `human-23.png` | `human-engineer-hook.png` |
| `human-guard-frost` | Ingrid Lund | Champion, Enchanter | Duelist, Lingering Magic | Frost Skewer | Melee | 3 / 0 | lunge | Hit for 3–4; the target's next turn comes 2 time units later. | `human-24.png` | `human-guard-frost.png` |
| `human-guard-stone` | Leo Stone | Stormcaller, Champion | Static Field, Glory | Stone Slash | Melee | 3 / 0 | swing | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `human-25.png` | `human-guard-stone.png` |
| `human-guard` | Peter Holm | Bulwark, Champion | Shield Wall, Hunt Down | Guard Thrust | Melee | 3 / 0 | lunge | Hit for 4–5. | `human-26.png` | `human-guard.png` |
| `human-knight-2` | Johanna Weber | Champion, Bulwark | Duelist, Hold Position | Knight Hack | Melee | 3 / 0 | swing | Hit for 4–5. | `human-27.png` | `human-knight-2.png` |
| `human-knight-banner` | Albrecht Lang | Commander, Bulwark | Vanguard, Unbreakable | Banner Thump | Melee | 4 / 0 | swing | Hit for 5–6. | `human-28.png` | `human-knight-banner.png` |
| `human-knight-hammer` | Johanka Weber | Bulwark, Champion | Shield Wall, Glory | Hammer Smash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `human-29.png` | `human-knight-hammer.png` |
| `human-knight-shield` | Edwin Marsh | Bulwark, Champion | Hold Position, Hunt Down | Shield Clobber | Melee | 4 / 0 | swing | Hit for 5–6. | `human-30.png` | `human-knight-shield.png` |
| `human-knight` | William Hart | Champion, Commander | Hunt Down, Rally | Knight Slice | Melee | 3 / 0 | swing | Hit for 4–5. | `human-31.png` | `human-knight.png` |
| `human-mage-crystal` | Kai Lindqvist | Stormcaller, Transmuter | Eye of the Storm, Mirror Form | Crystal Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-32.png` | `human-mage-crystal.png` |
| `human-mage-fire` | Carmen Costa | Pyromancer, Thief | Inferno, Master Thief | Fire Spark | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: burning ground on the target's spot. | `human-33.png` | `human-mage-fire.png` |
| `human-mage-frost` | Astor Nilsen | Cryomancer, Enchanter | Brittle, Spell Ward | Frost Spark | Projectile | 4 / 0 | bolt | Hit for 2–3; the target's next turn comes 2 time units later. | `human-34.png` | `human-mage-frost.png` |
| `human-mage-stone` | Henrik Berg | Stormcaller, Commander | Conductor, Drill | Stone Bolt | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Stunned for 5 time units. | `human-35.png` | `human-mage-stone.png` |
| `human-mage-water` | Mateo Santos | Cryomancer, Tracker | Steam Burst, Sniff the Air | Water Missile | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: push the target one row back. | `human-36.png` | `human-mage-water.png` |
| `human-monk-hammer` | Kenji Mori | Berserker, Champion | Unstoppable, Duelist | Hammer Thump | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `human-37.png` | `human-monk-hammer.png` |
| `human-necromancer-skull` | Viktor Graves | Necromancer, Champion | Lich Form, Glory | Skull Slice | Melee | 3 / 0 | swing | Hit for 4–5; heals the attacker for 1. | `human-38.png` | `human-necromancer-skull.png` |
| `human-priest-bell` | Martin Becker | Cleric, Bard | Purifying Aura, Grand Chorus | Bell Crush | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `human-39.png` | `human-priest-bell.png` |
| `human-priest-moon` | Luna Ortiz | Cleric, Hexer | Judgment, Witch's Patience | Moon Bolt | Projectile | 4 / 0 | bolt | Hit for 3–4; heals the attacker for 1. | `human-40.png` | `human-priest-moon.png` |
| `human-priest-sun` | Aurelia Day | Cleric, Tracker | Sanctity, Scent of Blood | Sun Fling | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: burning ground on the target's spot. | `human-41.png` | `human-priest-sun.png` |
| `human-priest` | Thomas Abbott | Cleric, Mender | Sanctity, Triage | Priest Burst | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-42.png` | `human-priest.png` |
| `human-rogue-potion` | Paul Fischer | Chemist, Thief | Volatile, Escape Artist | Potion Cut | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target is Poisoned for 20 time units. | `human-43.png` | `human-rogue-potion.png` |
| `human-rogue-shadow` | Nika Volkova | Assassin, Summoner | Vanishing Kill, Quickening | Shadow Cut | Melee | 3 / 0 | swing | Hit for 3–4; cannot be intercepted. | `human-44.png` | `human-rogue-shadow.png` |
| `human-rogue` | Daniel Hart | Thief, Saboteur | Shadow Purse, Wrecker | Rogue Cut | Melee | 3 / 0 | swing | Hit for 4–5. | `human-45.png` | `human-rogue.png` |
| `human-scout-lantern` | Elsa Lindgren | Thief, Summoner | Master Thief, Golem Heart | Lantern Hack | Melee | 3 / 0 | swing | Hit for 4–5. | `human-46.png` | `human-scout-lantern.png` |
| `human-shaman` | Kofi Mensah | Totemist, Commander | Sturdy Totems, Rally | Shaman Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-47.png` | `human-shaman.png` |
| `human-smith` | Hugo Schmidt | Engineer, Berserker | Reinforced, Reckless Swing | Smith Crush | Melee | 4 / 0 | swing | Hit for 5–6. | `human-48.png` | `human-smith.png` |
| `human-spellblade-2` | Laura Cole | Champion, Enchanter | Duelist, Lingering Magic | Spellblade Cleave | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `human-49.png` | `human-spellblade-2.png` |
| `human-spellblade` | Richard Novak | Champion, Enchanter | Hunt Down, Twin Enchant | Spellblade Slash | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `human-51.png` | `human-spellblade.png` |
| `human-wizard` | Arthur Pembroke | Enchanter, Engineer | Spell Ward, Self-Repair | Wizard Bolt | Projectile | 4 / 0 | bolt | Hit for 3–4. | `human-52.png` | `human-wizard.png` |
| `hummingbird-duelist-rapier` | Rapierbeak | Skyborne, Thief | Sky Lord, Shadow Purse | Rapier Slash | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `hummingbird-01.png` | — |
| `hydra-colossus-trident` | Tridentmaw Hydra | Summoner, Enchanter | Portal, Twin Enchant | Trident Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Stunned for 5 time units. | `hydra-01.png` | — |
| `hyena-engineer-saw` | Sawgrin | Engineer, Tracker | Rapid Fire, Relentless | Saw Flick | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target Bleeds. | `hyena-01.png` | — |
| `ibex-captain-standard` | Standardhorn | Bard, Commander | Rhythm, Vanguard | Standard Swipe | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `ibex-01.png` | — |
| `incense-censor` | Smokeswing | Cleric, Summoner | Purifying Aura, Quickening | Incense Throb | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `incense-censor-01.png` | — |
| `inkwell-trickster-quill` | Quillspatter | Saboteur, Tactician | Chaos Agent, Initiative | Quill Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `inkwell-01.png` | — |
| `isopod-sentinel-iron` | Ironroll | Bulwark, Keeper | Unbreakable, Steadfast Ward | Iron Stomp | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `isopod-01.png` | — |
| `jackalope-hexer-curse` | Cursehorn | Necromancer, Thief | Bone Armor, Escape Artist | Curse Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3; heals the attacker for 1. | `jackalope-01.png` | — |
| `jelly-mage-storm` | Stormbell Jelly | Skyborne, Stormcaller | Lift, Static Field | Storm Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Confused for 5 time units. | `jelly-01.png` | — |
| `jelly-healer` | Mendbloom Jelly | Mender, Skyborne | Overflowing Care, Eagle Eye | Jelly Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `jellyfish-02.png` | `jelly-healer.png` |
| `jerboa-trickster-acorn` | Acornpop | Saboteur, Engineer | Disarm Trap, Reinforced | Acorn Shot | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: the target is Immobilized for 5 time units. | `jerboa-01.png` | — |
| `kestrel-hunter-lure` | Lurewing | Tracker, Summoner | Sniff the Air, Golem Heart | Lure Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `kestrel-01.png` | — |
| `kettle-alchemist-siege` | Siegekettle | Champion, Bulwark | Glory, Shield Wall | Siege Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `kettle-01.png` | — |
| `kiln-behemoth` | Kiln Behemoth | Bulwark, Keeper | Hold Position, Thorned Ward | Kiln Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `kiln-01.png` | — |
| `kingfisher-captain-dagger` | Captain Daggerbill | Assassin, Skyborne | Venom Blades, Sky Lord | Dagger Slice | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `kingfisher-01.png` | — |
| `kinkajou-summoner-idol` | Idolpaw | Summoner, Thief | Portal, Master Thief | Idol Butt | Melee | 3 / 0 | lunge | Hit for 4–5. | `kinkajou-01.png` | — |
| `kite-primalist-tail` | Tailwind Kite | Skyborne, Mender | Eagle Eye, Regrowth | Tail Whip | Melee | 3 / 0 | swing | Hit for 3–4. | `kite-01.png` | — |
| `komodo-warrior-cleaver` | Cleaver Komodo | Berserker, Champion | Last Stand, Hunt Down | Cleaver Crush | Melee | 4 / 0 | swing | Hit for 5–6. | `komodo-01.png` | — |
| `kraken-colossus-basalt` | Basalt Kraken | Bulwark, Summoner | Unbreakable, Quickening | Basalt Flick | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Stunned for 5 time units. | `kraken-01.png` | — |
| `lantern-healer-prism` | Prismlamp | Mender, Cleric | Triage, Judgment | Prism Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `lantern-01.png` | — |
| `reef-warden` | Reef Warden | Keeper, Berserker | Thorned Ward, Reckless Swing | Reef Thrust | Melee | 3 / 0 | lunge | Hit for 4–5. | `lizardfolk-02.png` | `9_f.png` |
| `lizardfolk-barbarian-axe` | Axejaw Lizardfolk | Berserker, Champion | Unstoppable, Duelist | Axe Hack | Melee | 3 / 0 | swing | Hit for 4–5. | `lizardfolk-03.png` | — |
| `lizardfolk-ninja-kunai` | Kunai Lizardfolk | Assassin, Thief | Unseen, Shadow Purse | Kunai Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `lizardfolk-04.png` | — |
| `lobster-bard-castanet` | Castanet Lobster | Bard, Commander | Crescendo, Drill | Castanet Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `lobster-01.png` | — |
| `lobster-engineer-rivet` | Rivetclaw | Engineer, Chemist | Self-Repair, Showman | Rivet Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `lobster-02.png` | — |
| `lockbox-dreadnought` | Lockbox Dreadnought | Bulwark, Keeper | Shield Wall, Twin Wards | Lockbox Jab | Melee | 3 / 0 | lunge | Hit for 4–5. | `lockbox-01.png` | — |
| `loom-binder` | Threadbinder Loom | Engineer, Commander | Rapid Fire, Rally | Loom Lash | Melee | 3 / 0 | swing | Hit for 3–4. | `loom-01.png` | — |
| `lyrebird-bard-plume` | Plumesong | Bard, Skyborne | Grand Chorus, Lift | Plume Spray | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `lyrebird-01.png` | — |
| `mammoth-titan-tundra` | Tundra Titan | Bulwark, Cryomancer | Hold Position, Cold Heart | Tundra Slam | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `mammoth-01.png` | — |
| `manatee-healer-shell` | Shellmend Manatee | Mender, Cleric | Overflowing Care, Sanctity | Shell Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `manatee-01.png` | — |
| `mandrake-primalist-root` | Screamroot | Ritualist, Summoner | Frostbinder, Golem Heart | Root Flick | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Immobilized for 5 time units. | `mandrake-01.png` | — |
| `mandrill-brawler-gloves` | Glovegrin | Berserker, Champion | Reckless Swing, Glory | Gloves Butt | Melee | 3 / 0 | lunge | Hit for 4–5. | `mandrill-01.png` | — |
| `mannequin` | Stitchfist Mannequin | Berserker, Bulwark | Last Stand, Unbreakable | Mannequin Gore | Melee | 3 / 0 | lunge | Hit for 4–5. | `mannequin-01.png` | `34_f.png` |
| `manticore-assassin-barb` | Barbtail Manticore | Assassin, Skyborne | Vanishing Kill, Sky Lord | Barb Lash | Melee | 3 / 0 | swing | Hit for 3–4. | `manticore-01.png` | — |
| `mantis-engineer-blade` | Bladewright Mantis | Saboteur, Engineer | Wrecker, Reinforced | Blade Cut | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `mantis-01.png` | — |
| `marabou-healer-balm` | Balmbeak | Engineer, Thief | Self-Repair, Escape Artist | Balm Kick | Melee | 3 / 0 | lunge | Hit for 4–5. | `marabou-01.png` | — |
| `maskrack-summoner-echo` | Echomask Rack | Summoner, Skyborne | Portal, Eagle Eye | Echo Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; cannot be intercepted. | `maskrack-01.png` | — |
| `mastiff-guard-shield` | Shieldjaw Mastiff | Bulwark, Tracker | Shield Wall, Scent of Blood | Shield Bite | Melee | 3 / 0 | lunge | Hit for 4–5. | `mastiff-01.png` | — |
| `meerkat-captain-spyglass` | Spyglass Meerkat | Commander, Tracker | Vanguard, Relentless | Spyglass Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `meerkat-01.png` | — |
| `millstone-juggernaut` | Millstone Juggernaut | Berserker, Bulwark | Unstoppable, Hold Position | Millstone Thump | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `millstone-01.png` | — |
| `mirror-revenant` | Mirror Revenant | Enchanter, Skyborne | Lingering Magic, Lift | Mirror Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `mirror-01.png` | — |
| `mole-engineer-gear` | Gearclaw Mole | Engineer, Chemist | Rapid Fire, Potent Brew | Gear Smash | Melee | 4 / 0 | swing | Hit for 5–6. | `mole-01.png` | — |
| `mole-engineer-hexhammer` | Hexhammer Mole | Engineer, Hexer | Reinforced, Lingering Malice | Hexhammer Thump | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `mole-02.png` | — |
| `mongoose-assassin-sickle` | Sicklewhisk | Assassin, Thief | Venom Blades, Master Thief | Sickle Slash | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `mongoose-01.png` | — |
| `mongoose-hunter-hex` | Hexdart Mongoose | Assassin, Thief | Unseen, Shadow Purse | Hex Volley | Projectile | 3 / 0 | shot | Hit for 3–4; heals the attacker for 1. | `mongoose-02.png` | — |
| `monkey-trickster-sling` | Slingtail | Saboteur, Tracker | Chaos Agent, Sniff the Air | Sling Flick | Projectile | 3 / 0 | shot | Hit for 3–4. | `monkey-01.png` | — |
| `moray-trickster-hook` | Hookgrin Moray | Saboteur, Tracker | Disarm Trap, Scent of Blood | Hook Snap | Melee | 3 / 0 | swing | Hit for 2–3; 20%: pull the target one row toward the battle line. | `moray-01.png` | — |
| `mortar-alchemist-bastion` | Bastion Mortar | Berserker, Bulwark | Reckless Swing, Unbreakable | Bastion Clobber | Melee | 4 / 0 | swing | Hit for 5–6. | `mortar-01.png` | — |
| `moth-cleric-lantern` | Lanternmoth | Cleric, Skyborne | Purifying Aura, Sky Lord | Lantern Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `moth-02.png` | — |
| `mushroom-druid` | Sporedruid | Ritualist, Lifebinder | Deluge, Phoenix | Mushroom Ray | Projectile | 4 / 0 | bolt | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `mushroom-01.png` | `mushroom-druid.png` |
| `mushroom-sentinel-shield` | Capshield | Bulwark, Spiritwalker | Shield Wall, Soul Anchor | Shield Pulse | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 30%: the target is Poisoned for 20 time units. | `mushroom-03.png` | — |
| `musicbox-summoner-drum` | Windup Drummer | Summoner, Bulwark | Quickening, Hold Position | Drum Headbutt | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `musicbox-01.png` | — |
| `muskox-bodyguard-buckler` | Bucklerhorn | Bulwark, Berserker | Unbreakable, Last Stand | Buckler Bash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `muskox-01.png` | — |
| `newt-alchemist-coil` | Coilnewt | Chemist, Engineer | Volatile, Self-Repair | Coil Crack | Melee | 3 / 0 | swing | Hit for 3–4. | `newt-01.png` | — |
| `olm-summoner-shade` | Shadeolm | Summoner, Enchanter | Golem Heart, Spell Ward | Shade Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `olm-01.png` | — |
| `opossum-summoner-dice` | Dicetail | Summoner, Tactician | Portal, All In | Dice Flick | Projectile | 3 / 0 | shot | Hit for 2–3; the target's next turn comes 2 time units later. | `opossum-01.png` | — |
| `orc-summoner-skull` | Grukk Skullcaller | Necromancer, Spiritwalker | Death's Bounty, Haunting | Skull Thump | Melee | 4 / 0 | swing | Hit for 5–6; heals the attacker for 1. | `orc-01.png` | — |
| `orchidmantis-trickster-leaf` | Petalshade | Ritualist, Saboteur | Tidecaller, Wrecker | Leaf Crack | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Immobilized for 5 time units. | `orchidmantis-01.png` | — |
| `organ-summoner` | Pipe Organ Choir | Commander, Bulwark | Drill, Shield Wall | Organ Plume | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `organ-01.png` | — |
| `orrery-sorcerer-orbit` | Orbitwright | Enchanter, Commander | Twin Enchant, Rally | Orbit Missile | Projectile | 4 / 0 | bolt | Hit for 3–4. | `orrery-01.png` | — |
| `ossuary-summoner-cube` | Bonecube | Necromancer, Bulwark | Lich Form, Hold Position | Cube Gore | Melee | 3 / 0 | lunge | Hit for 4–5. | `ossuary-01.png` | — |
| `otter-hunter-sling` | Slingwhisker | Tracker, Thief | Relentless, Escape Artist | Sling Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `otter-01.png` | — |
| `owl-wizard` | Hootmage | Enchanter, Skyborne | Lingering Magic, Eagle Eye | Owl Spark | Projectile | 4 / 0 | bolt | Hit for 3–4. | `owl-01.png` | `owl-wizard.png` |
| `panda-monk-fists` | Bamboo Monk | Berserker, Champion | Unstoppable, Hunt Down | Fists Gore | Melee | 3 / 0 | lunge | Hit for 4–5. | `panda-01.png` | — |
| `pangolin-guard-decoy` | Decoy Pangolin | Bulwark, Thief | Unbreakable, Master Thief | Decoy Bash | Melee | 4 / 0 | swing | Hit for 5–6. | `pangolin-01.png` | — |
| `pangolin-sorcerer-thunder` | Thunderscale | Bulwark, Summoner | Shield Wall, Quickening | Thunder Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; also hits one unit next to the target for 1–2. | `pangolin-02.png` | — |
| `panpipe-summoner-reed` | Reedcaller | Commander, Summoner | Vanguard, Golem Heart | Reed Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `panpipe-01.png` | — |
| `peacock-bard-feather` | Plumecrown | Bard, Skyborne | Rhythm, Lift | Feather Bolt | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `peacock-01.png` | — |
| `phoenix-titan-solar` | Solar Phoenix | Mender, Pyromancer | Regrowth, Ashwalker | Solar Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Stunned for 5 time units. | `phoenix-01.png` | — |
| `pika-summoner-horn` | Hornpip | Bard, Summoner | Crescendo, Portal | Horn Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `pika-01.png` | — |
| `platypus-alchemist-rune` | Runebill | Chemist, Engineer | Showman, Rapid Fire | Rune Plume | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `platypus-01.png` | — |
| `plough-ravager` | Furrow Ravager | Champion, Summoner | Duelist, Quickening | Plough Slash | Melee | 3 / 0 | swing | Hit for 4–5. | `plough-01.png` | — |
| `pocketwatch-warden` | Tickwarden | Keeper, Commander | Steadfast Ward, Drill | Pocketwatch Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `pocketwatch-01.png` | — |
| `porcupine-alchemist-shield` | Quillflask | Chemist, Bulwark | Potent Brew, Hold Position | Shield Flick | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `porcupine-01.png` | — |
| `portcullis-colossus` | Portcullis Colossus | Bulwark, Keeper | Unbreakable, Thorned Ward | Portcullis Smash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `portcullis-01.png` | — |
| `printingpress-summoner-type` | Typesetter | Summoner, Engineer | Golem Heart, Reinforced | Type Fling | Projectile | 3 / 0 | shot | Hit for 3–4. | `printingpress-01.png` | — |
| `prism-hexer` | Shardhex | Skyborne, Keeper | Sky Lord, Twin Wards | Prism Spark | Projectile | 4 / 0 | bolt | Hit for 3–4; heals the attacker for 1. | `prism-01.png` | — |
| `puffin-bodyguard-shield` | Shieldpuff | Skyborne, Bulwark | Eagle Eye, Shield Wall | Shield Thump | Melee | 4 / 0 | swing | Hit for 5–6. | `puffin-01.png` | — |
| `quern-oracle` | Grindstone Oracle | Spiritwalker, Skyborne | Puppeteer, Lift | Quern Pound | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `quern-01.png` | — |
| `quokka-summoner-pebble` | Pebblegrin | Summoner, Enchanter | Portal, Spell Ward | Pebble Burst | Projectile | 4 / 0 | bolt | Hit for 3–4. | `quokka-01.png` | — |
| `raccoon-alchemist-potion` | Potionpaw | Chemist, Thief | Volatile, Shadow Purse | Potion Volley | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `raccoon-01.png` | — |
| `ram-paladin-sun` | Sunhorn Paladin | Bard, Keeper | Rhythm, Steadfast Ward | Sun Slam | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: burning ground on the target's spot. | `ram-01.png` | `ram-paladin-sun.png` |
| `ram-bard-bellows` | Bellows Ram | Bard, Engineer | Grand Chorus, Self-Repair | Bellows Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `ram-02.png` | — |
| `reliquary-summoner-seal` | Sealed Reliquary | Summoner, Enchanter | Quickening, Twin Enchant | Seal Spark | Projectile | 4 / 0 | bolt | Hit for 3–4. | `reliquary-01.png` | — |
| `retort-alchemist` | Bubblehex Retort | Chemist, Bulwark | Showman, Hold Position | Retort Spray | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2; heals the attacker for 1. | `retort-01.png` | — |
| `rhinoceros-guard-shadow` | Shadowhorn | Assassin, Champion | Vanishing Kill, Glory | Shadow Slash | Melee | 3 / 0 | swing | Hit for 3–4; cannot be intercepted. | `rhinoceros-01.png` | — |
| `roadrunner-hunter-whistle` | Whistledash | Tracker, Skyborne | Sniff the Air, Sky Lord | Whistle Toss | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: burning ground on the target's spot. | `roadrunner-01.png` | — |
| `roc-colossus-thunder` | Thunder Roc | Tracker, Skyborne | Scent of Blood, Eagle Eye | Thunder Pounce | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Stunned for 5 time units. | `roc-01.png` | — |
| `rooster-captain-banner` | Banner Cock | Commander, Champion | Rally, Hunt Down | Banner Thrust | Melee | 3 / 0 | lunge | Hit for 4–5. | `rooster-01.png` | — |
| `sableantelope-captain-pennant` | Pennant Sable | Commander, Champion | Vanguard, Duelist | Pennant Poke | Melee | 3 / 0 | lunge | Hit for 4–5. | `sableantelope-01.png` | — |
| `safe-hexer-vault` | Vaulthex | Bulwark, Keeper | Unbreakable, Twin Wards | Vault Bite | Melee | 3 / 0 | lunge | Hit for 4–5; heals the attacker for 1. | `safe-01.png` | — |
| `sail-hexer-night` | Nightsail | Skyborne, Thief | Lift, Escape Artist | Night Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3; heals the attacker for 1. | `sail-01.png` | — |
| `salamander-rogue` | Slinkember | Thief, Tracker | Master Thief, Relentless | Salamander Gore | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: burning ground on the target's spot. | `salamander-01.png` | `salamander-rogue.png` |
| `salamander-summoner-flask` | Flaskember | Chemist, Summoner | Potent Brew, Golem Heart | Flask Volley | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: burning ground on the target's spot. | `salamander-02.png` | — |
| `sarcophagus-warden` | Tombwarden | Summoner, Keeper | Portal, Steadfast Ward | Sarcophagus Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `sarcophagus-01.png` | — |
| `scarab-engineer-rune` | Runeshell Scarab | Engineer, Tracker | Rapid Fire, Sniff the Air | Rune Flick | Projectile | 3 / 0 | shot | Hit for 3–4. | `scarab-01.png` | — |
| `scorpion` | Stingtail | Thief, Tracker | Shadow Purse, Scent of Blood | Scorpion Whip | Melee | 3 / 0 | swing | Hit for 3–4. | `scorpion-01.png` | `scorpion.png` |
| `scorpion-healer-ribbon` | Ribbonsting | Engineer, Tracker | Reinforced, Relentless | Ribbon Crack | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Confused for 5 time units. | `scorpion-02.png` | — |
| `seal-bard-bottle` | Bottlenose Bard | Engineer, Bard | Self-Repair, Crescendo | Bottle Plume | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `seal-01.png` | — |
| `sealpress-summoner` | Waxseal Press | Summoner, Keeper | Quickening, Thorned Ward | Sealpress Missile | Projectile | 4 / 0 | bolt | Hit for 3–4. | `sealpress-01.png` | — |
| `secretarybird-hunter-javelin` | Javelin Stalker | Skyborne, Assassin | Sky Lord, Venom Blades | Javelin Thrust | Melee | 3 / 0 | lunge | Hit for 4–5. | `secretarybird-01.png` | — |
| `serpent-titan-crown` | Crowned Serpent | Thief, Summoner | Escape Artist, Golem Heart | Crown Shot | Projectile | 3 / 0 | shot | Hit for 2–3; 20%: the target is Stunned for 5 time units. | `serpent-01.png` | — |
| `serval-hunter-crossbow` | Boltear | Tracker, Thief | Sniff the Air, Master Thief | Crossbow Flick | Projectile | 3 / 0 | shot | Hit for 3–4. | `serval-01.png` | — |
| `sewingbox-engineer-ribbon` | Stitchkit | Engineer, Mender | Rapid Fire, Triage | Ribbon Whip | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Confused for 5 time units. | `sewingbox-01.png` | — |
| `sextant-admiral` | Admiral Starsight | Tracker, Commander | Scent of Blood, Drill | Sextant Throw | Projectile | 3 / 0 | shot | Hit for 3–4. | `sextant-01.png` | — |
| `shield-vanguard` | Vanguard Aegis | Bulwark, Champion | Shield Wall, Glory | Shield Crush | Melee | 4 / 0 | swing | Hit for 5–6. | `shield-01.png` | — |
| `shoebill-captain-pike` | Pikebill | Commander, Tracker | Rally, Relentless | Pike Jab | Melee | 3 / 0 | lunge | Hit for 4–5. | `shoebill-01.png` | — |
| `shrew-summoner-totem` | Totemshrew | Summoner, Totemist | Portal, Spirit Shield | Totem Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `shrew-01.png` | — |
| `siphon-alchemist` | Siphonworm | Chemist, Bulwark | Volatile, Hold Position | Siphon Crack | Melee | 3 / 0 | swing | Hit for 3–4. | `siphon-01.png` | — |
| `sistrum-summoner` | Rattlechant | Commander, Enchanter | Vanguard, Lingering Magic | Sistrum Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `sistrum-01.png` | — |
| `sledgehammer-colossus` | Sledge Colossus | Berserker, Bulwark | Last Stand, Unbreakable | Sledgehammer Thump | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Stunned for 5 time units. | `sledgehammer-01.png` | — |
| `sloth-guard-bark` | Barkhold Sloth | Spiritwalker, Mender | Soul Anchor, Overflowing Care | Bark Gore | Melee | 3 / 0 | lunge | Hit for 4–5. | `sloth-01.png` | — |
| `snail-cleric-crystal` | Prism Snail | Stormcaller, Cleric | Eye of the Storm, Judgment | Crystal Missile | Projectile | 4 / 0 | bolt | Hit for 3–4. | `snail-01.png` | `snail-cleric-crystal.png` |
| `snail-bard-brewer` | Brewshell | Engineer, Bard | Reinforced, Grand Chorus | Brewer Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Confused for 5 time units. | `snail-05.png` | — |
| `spikeback` | Spikeback | Berserker, Bulwark | Unstoppable, Shield Wall | Spikeback Bite | Melee | 3 / 0 | lunge | Hit for 4–5. | `spikeback-01.png` | `1_f.png` |
| `spindle-summoner` | Spindlewhirl | Commander, Tactician | Drill, Inspiring Presence | Spindle Lash | Melee | 3 / 0 | swing | Hit for 3–4. | `spindle-01.png` | — |
| `stag-hunter-shield` | Shieldhorn Stag | Tracker, Bulwark | Sniff the Air, Hold Position | Shield Lunge | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: the target is Confused for 5 time units. | `stag-01.png` | — |
| `stoat-healer-needle` | Needlewhisk | Assassin, Thief | Unseen, Shadow Purse | Needle Slash | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `stoat-01.png` | — |
| `sugarglider-scout-sail` | Sailglide | Skyborne, Enchanter | Eagle Eye, Spell Ward | Sail Flick | Projectile | 3 / 0 | shot | Hit for 3–4. | `sugarglider-01.png` | — |
| `sundial-vanguard` | Noonward | Keeper, Bulwark | Twin Wards, Unbreakable | Sundial Poke | Melee | 3 / 0 | lunge | Hit for 3–4; 20%: burning ground on the target's spot. | `sundial-01.png` | — |
| `sundrum-hexer` | Sunhex Drum | Engineer, Keeper | Self-Repair, Steadfast Ward | Sundrum Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: burning ground on the target's spot. | `sundrum-01.png` | — |
| `tapestry-summoner` | Woven Host | Summoner, Bulwark | Quickening, Shield Wall | Tapestry Glow | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `tapestry-01.png` | — |
| `tarsier-hexer-mask` | Maskeye | Thief, Hexer | Escape Artist, Spreading Curse | Mask Swipe | Melee | 3 / 0 | lunge | Hit for 3–4; cannot be intercepted. | `tarsier-01.png` | — |
| `teapot-alchemist-volcano` | Volcano Teapot | Chemist, Pyromancer | Showman, Wildfire | Volcano Gust | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `teapot-01.png` | — |
| `termite-engineer-drill` | Drillmite | Engineer, Bulwark | Rapid Fire, Hold Position | Drill Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `termite-01.png` | — |
| `termite-summoner-clockwork` | Clockmite | Engineer, Summoner | Reinforced, Golem Heart | Clockwork Volley | Projectile | 3 / 0 | shot | Hit for 3–4. | `termite-02.png` | — |
| `thorntreant-primalist-bramble` | Bramblestalker | Spiritwalker, Assassin | Haunting, Vanishing Kill | Bramble Hum | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 30%: the target Bleeds. | `thorntreant-01.png` | — |
| `toad-boulder` | Boulder Toad | Bulwark, Berserker | Unbreakable, Last Stand | Toad Bash | Melee | 4 / 0 | swing | Hit for 5–6. | `toad-01.png` | `5_f.png` |
| `toad-druid` | Druid Toad | Ritualist, Lifebinder | Deluge, Lifeblood | Toad Spit | Direct, melee reach | 4 / 0 | breath | Hit the target for 3–4 and every unit next to it for 1–2. | `toad-02.png` | `toad-druid.png` |
| `toad-bard-bell` | Belltoad | Bard, Summoner | Rhythm, Portal | Bell Bolt | Projectile | 4 / 0 | bolt | Hit for 2–3; 20%: the target is Confused for 5 time units. | `toad-03.png` | — |
| `tortoise-colossus-mountain` | Mountainshell | Bulwark, Spiritwalker | Shield Wall, Puppeteer | Mountain Wave | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 1–2; 20%: the target is Stunned for 5 time units. | `tortoise-01.png` | — |
| `tortoise-sentinel-obsidian` | Obsidian Tortoise | Bulwark, Summoner | Hold Position, Quickening | Obsidian Throb | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `tortoise-02.png` | — |
| `treant-druid-leaf` | Leafwarden | Ritualist, Lifebinder | Tidecaller, Martyr | Leaf Snap | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Immobilized for 5 time units. | `treant-01.png` | — |
| `treant-titan-ancient` | Elderbough | Spiritwalker, Mender | Soul Anchor, Regrowth | Ancient Flick | Melee | 3 / 0 | swing | Hit for 2–3; 20%: the target is Stunned for 5 time units. | `treant-02.png` | — |
| `trowel-crusader` | Trowel Crusader | Bulwark, Champion | Unbreakable, Hunt Down | Trowel Hack | Melee | 3 / 0 | swing | Hit for 4–5. | `trowel-01.png` | — |
| `tuningfork-champion` | Resonant Champion | Commander, Champion | Rally, Duelist | Tuningfork Swipe | Melee | 3 / 0 | lunge | Hit for 4–5. | `tuningfork-01.png` | — |
| `turbine-skyguard` | Skyturbine | Skyborne, Champion | Lift, Glory | Turbine Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `turbine-01.png` | — |
| `turtle-monk` | Shellmonk | Cleric, Bulwark | Sanctity, Hold Position | Turtle Thump | Melee | 4 / 0 | swing | Hit for 5–6. | `turtle-02.png` | `turtle-monk.png` |
| `turtle-bard-frost` | Frostshell Bard | Bulwark, Enchanter | Shield Wall, Twin Enchant | Frost Breath | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; the target's next turn comes 2 time units later. | `turtle-03.png` | — |
| `umbrella-guardian-fan` | Fanbrella | Skyborne, Stormcaller | Sky Lord, Conductor | Fan Smash | Melee | 4 / 0 | swing | Hit for 5–6. | `umbrella-01.png` | — |
| `velvetworm-hexer-rune` | Runevelvet | Thief, Spiritwalker | Master Thief, Haunting | Rune Lash | Melee | 3 / 0 | swing | Hit for 3–4; heals the attacker for 1. | `velvetworm-01.png` | — |
| `vinegaroon-alchemist-flask` | Vinegar Flask | Chemist, Tracker | Potent Brew, Relentless | Flask Throw | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `vinegaroon-01.png` | — |
| `vise-enforcer` | Vise Enforcer | Bulwark, Engineer | Unbreakable, Self-Repair | Vise Slam | Melee | 3 / 0 | lunge | Hit for 4–5. | `vise-01.png` | — |
| `vulture-healer-cinder` | Cinderbeak | Skyborne, Stormcaller | Eagle Eye, Static Field | Cinder Throb | Direct, melee reach | 4 / 0 | pulse | Every enemy in melee reach takes 2–3. | `vulture-01.png` | — |
| `walrus-bard-horn` | Tuskhorn Walrus | Bard, Commander | Crescendo, Vanguard | Horn Plume | Direct, melee reach | 4 / 0 | breath | Hit the target for 2–3 and every unit next to it for 1–2; 20%: the target is Confused for 5 time units. | `walrus-01.png` | — |
| `warthog-engineer-cannon` | Cannon Warthog | Engineer, Chemist | Rapid Fire, Volatile | Cannon Fling | Projectile | 3 / 0 | shot | Hit for 3–4. | `warthog-01.png` | — |
| `wasp-alchemist-potion` | Stingflask | Chemist, Skyborne | Showman, Lift | Potion Toss | Projectile | 3 / 0 | shot | Hit for 2–3; 30%: the target is Poisoned for 20 time units. | `wasp-01.png` | — |
| `weasel-rogue-dagger` | Daggerwhisk | Assassin, Thief | Venom Blades, Shadow Purse | Dagger Carve | Melee | 3 / 0 | swing | Hit for 3–4; 30%: the target Bleeds. | `weasel-01.png` | — |
| `weathervane-summoner-arrow` | Gustarrow | Summoner, Skyborne | Golem Heart, Sky Lord | Arrow Shot | Projectile | 3 / 0 | shot | Hit for 3–4. | `weathervane-01.png` | — |
| `weaverbird-summoner-banner` | Bannerweave | Commander, Summoner | Drill, Portal | Banner Ray | Projectile | 4 / 0 | bolt | Hit for 3–4. | `weaverbird-01.png` | — |
| `thornfang` | Thornfang | Tracker, Berserker | Scent of Blood, Reckless Swing | Thornfang Bite | Melee | 3 / 0 | lunge | Hit for 3–4; 30%: the target Bleeds. | `werewolf-01.png` | `6_f.png` |
| `wisp-hexer-shadow` | Shadowwisp | Hexer, Assassin | Witch's Patience, Unseen | Shadow Burst | Projectile | 4 / 0 | bolt | Hit for 2–3; cannot be intercepted. | `wisp-01.png` | — |
| `wolf-ranger` | Ranger Wolf | Tracker, Commander | Sniff the Air, Rally | Wolf Slam | Melee | 3 / 0 | lunge | Hit for 4–5. | `wolf-01.png` | `wolf-ranger.png` |
| `wolverine-warrior-mace` | Macefang Wolverine | Berserker, Champion | Unstoppable, Hunt Down | Mace Thump | Melee | 4 / 0 | swing | Hit for 4–5; 30%: the target Bleeds. | `wolverine-01.png` | — |
| `wyvern-scout-obsidian` | Obsidian Wyvern | Skyborne, Transmuter | Eagle Eye, Lasting Change | Obsidian Slice | Melee | 3 / 0 | swing | Hit for 4–5. | `wyvern-01.png` | — |
| `yak-bard-gong` | Gongyak | Bard, Summoner | Grand Chorus, Quickening | Gong Bash | Melee | 4 / 0 | swing | Hit for 4–5; 20%: the target is Confused for 5 time units. | `yak-01.png` | — |

## Card details

Home biome, stats and card framing per creature. Frames are `x,y,w,h` in the art's 1024 × 1536 pixels (2:3; they may reach past the art edges, so x and y can be negative). The front frame is the card front's portrait, the rear frame the card back. HP is 20–80; Power and Speed are 1–10. **Cost** (1–10) is team-building points; a team of up to six creatures must fit a budget of 35. Cost comes from each creature's power score (game_system §5.1) and is recalculated after AI-versus-AI testing. **Status** says whether the owner reviewed the row.

| Art file | Name | Biome | HP | Speed | Power | Cost | Front frame | Rear frame | Status | Notes |
|---|---|---|---|---|---|---|---|---|---|---|
| `abacus-01.png` | Tallymarch | Cathedral | 40 | 5 | 5 | 4 | 279,-63,603,905 | 0,0,1024,1536 | reviewed |  |
| `accordion-01.png` | Wheezewarden | Cathedral | 40 | 5 | 5 | 5 | 376,0,750,1125 | -6,-62,1095,1643 | reviewed |  |
| `anchor-01.png` | Barnacle Bulwark | Coast | 65 | 3 | 6 | 7 | 252,-105,698,1047 | 0,0,1024,1536 | reviewed |  |
| `ankylosaur-01.png` | Clubtail Grumbleback | Mountain | 70 | 3 | 6 | 8 | 75,324,640,960 | 0,0,1061,1592 | reviewed |  |
| `ant-01.png` | Cogmarch Ant | Jungle | 20 | 8 | 3 | 1 | 206,57,634,951 | 0,0,1024,1536 | reviewed |  |
| `armadillo-01.png` | Armadillo Recruit | Desert | 45 | 6 | 5 | 6 | 38,507,640,960 | 28,70,977,1466 | reviewed |  |
| `armadillo-02.png` | Armadillo Knight | Desert | 50 | 4 | 6 | 6 | 266,255,640,960 | 31,46,993,1490 | reviewed |  |
| `astrolabe-01.png` | Starwright Judge | Cathedral | 40 | 5 | 5 | 5 | 278,8,632,948 | 0,0,1024,1536 | reviewed |  |
| `aurochs-01.png` | Ironhide Aurochs | Savannah | 65 | 3 | 6 | 7 | 296,192,733,1100 | 9,-6,1089,1634 | reviewed |  |
| `axolotl-01.png` | Ripplequill | Lagoon | 20 | 8 | 3 | 3 | 193,98,717,1076 | 0,0,1024,1536 | reviewed |  |
| `ayeaye-01.png` | Candlefinger | Jungle | 20 | 8 | 3 | 3 | 272,144,640,960 | 0,0,1024,1536 | reviewed |  |
| `badger-01.png` | Drumbadger | Autumn woods | 45 | 5 | 5 | 5 | 262,65,683,1025 | 0,0,1024,1536 | reviewed |  |
| `ballast-01.png` | Sandbag Sentinel | Storm cliffs | 50 | 5 | 5 | 7 | 188,-9,760,1140 | 0,0,1024,1536 | reviewed |  |
| `basilisk-01.png` | Stonegaze Basilisk | Desert | 65 | 3 | 6 | 8 | 345,111,640,960 | 0,0,1024,1536 | reviewed |  |
| `bassoon-01.png` | Dirgewind | Ruins | 40 | 5 | 5 | 5 | 310,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `bat-01.png` | Moonwing | Crystal caverns | 20 | 8 | 3 | 4 | 326,131,640,960 | 0,0,1024,1536 | reviewed |  |
| `bat-02.png` | Bat Rogue | Crystal caverns | 20 | 7 | 4 | 4 | 239,28,640,960 | 0,0,1024,1536 | reviewed |  |
| `baton-01.png` | Marshal Tempo | Cathedral | 40 | 5 | 5 | 4 | 140,0,770,1154 | 0,0,1024,1536 | reviewed |  |
| `beacon-01.png` | Lighthouse Wakelamp | Coast | 70 | 3 | 6 | 9 | 195,-22,779,1169 | 0,0,1024,1536 | reviewed |  |
| `beaver-01.png` | Pumpwhisker | Wet forest | 40 | 5 | 5 | 5 | 310,124,640,960 | 0,0,1024,1536 | reviewed |  |
| `bee-01.png` | Sunhum | Flower meadow | 20 | 8 | 3 | 4 | 274,187,634,951 | 0,0,1024,1536 | reviewed |  |
| `beetle-01.png` | Beetle Recruit | Jungle | 45 | 5 | 5 | 6 | 214,321,810,1215 | 45,67,979,1469 | reviewed |  |
| `beetle-02.png` | Beetle Guardian | Jungle | 55 | 4 | 7 | 9 | 225,255,731,1097 | 28,42,996,1494 | reviewed |  |
| `beetle-04.png` | Seedshot Scarab | Jungle | 30 | 8 | 3 | 4 | 348,197,640,960 | 0,0,1024,1536 | reviewed |  |
| `behemoth-01.png` | Ironbound Behemoth | Mountain | 75 | 3 | 6 | 9 | 389,92,640,960 | 0,0,1024,1536 | reviewed |  |
| `bell-01.png` | Clapperward | Cathedral | 65 | 3 | 6 | 7 | 253,65,640,960 | 0,0,1024,1536 | reviewed |  |
| `bellows-01.png` | Puffing Forgedrake | Ashlands | 45 | 5 | 5 | 6 | 41,419,640,960 | 0,0,1024,1536 | reviewed |  |
| `binturong-01.png` | Vinelilt | Jungle | 40 | 5 | 5 | 5 | 118,77,640,960 | 0,0,1024,1536 | reviewed |  |
| `bloomspear-01.png` | Bloomspear | Flower meadow | 40 | 5 | 5 | 5 | 259,110,640,960 | 42,51,973,1460 | reviewed |  |
| `boar-01.png` | Magma Hog | Ashlands | 60 | 4 | 6 | 7 | 35,550,640,960 | -2,272,1024,1536 | reviewed |  |
| `boar-02.png` | Hexbristle | Autumn woods | 45 | 5 | 5 | 6 | 433,181,640,960 | 0,0,1024,1536 | reviewed |  |
| `bombardierbeetle-01.png` | Acidpop | Desert | 30 | 8 | 3 | 4 | 65,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `book-01.png` | Foldtome | Ruins | 40 | 5 | 5 | 6 | 220,475,640,960 | 0,0,1024,1536 | reviewed |  |
| `brazier-01.png` | Coalwraith Brazier | Ashlands | 50 | 5 | 5 | 7 | 203,1,740,1110 | 0,0,1024,1536 | reviewed |  |
| `buffalo-01.png` | Rootyoke Buffalo | Savannah | 65 | 3 | 6 | 8 | 327,141,640,960 | 0,0,1024,1536 | reviewed |  |
| `bugle-01.png` | Skybrass | Cathedral | 20 | 8 | 3 | 3 | 377,201,640,960 | 0,0,1024,1536 | reviewed |  |
| `candelabrum-01.png` | Waxbrawler | Cathedral | 45 | 5 | 5 | 5 | 212,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `cannon-01.png` | Powderbelch | Cathedral | 75 | 3 | 6 | 10 | 466,72,592,888 | 0,0,1024,1536 | reviewed |  |
| `capybara-01.png` | Calmwater Capybara | Jungle | 40 | 5 | 5 | 5 | 422,195,640,960 | 0,0,1024,1536 | reviewed |  |
| `caracal-01.png` | Tuftspear | Savannah | 40 | 5 | 5 | 5 | 364,221,640,960 | 0,0,1024,1536 | reviewed |  |
| `carillon-01.png` | Chimespire | Cathedral | 75 | 3 | 6 | 9 | 161,1,640,960 | 0,0,1024,1536 | reviewed |  |
| `cassowary-01.png` | Crestcaller | Jungle | 40 | 5 | 5 | 5 | 384,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `caterpillar-01.png` | Caterpillar Bard | Mushroom grotto | 20 | 7 | 3 | 1 | 57,362,640,960 | 0,0,1024,1536 | reviewed |  |
| `catfolk-01.png` | Moonclaw Spellblade | Desert | 40 | 5 | 5 | 5 | 281,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `centipede-01.png` | Sicklecoil | Jungle | 50 | 5 | 5 | 6 | 170,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `chalice-01.png` | Stormgoblet | Storm cliffs | 30 | 8 | 3 | 4 | 202,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `chameleon-01.png` | Shiftscale | Jungle | 20 | 8 | 3 | 2 | 277,224,640,960 | 0,0,1024,1536 | reviewed |  |
| `chandelier-01.png` | Candlecrown | Cathedral | 45 | 5 | 5 | 7 | 230,364,640,960 | 0,0,1024,1536 | reviewed |  |
| `chest-01.png` | Lockjaw Chest | Ruins | 55 | 5 | 5 | 7 | 205,292,739,1109 | 0,0,1024,1536 | reviewed |  |
| `chinchilla-01.png` | Ocarina Puff | Mountain | 20 | 8 | 3 | 3 | 105,245,640,960 | 0,0,1024,1536 | reviewed |  |
| `cicada-01.png` | Chimewing Cicada | Savannah | 20 | 8 | 3 | 3 | 334,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `cinderhorn-01.png` | Cinderhorn | Ashlands | 45 | 5 | 6 | 6 | 272,32,640,960 | 0,0,1024,1536 | reviewed |  |
| `coati-01.png` | Diceroll Coati | Jungle | 20 | 8 | 3 | 1 | 135,129,640,960 | 0,0,1024,1536 | reviewed |  |
| `cobra-01.png` | Hoodspitter | Desert | 40 | 5 | 5 | 5 | 330,-36,640,960 | 0,0,1024,1536 | reviewed |  |
| `compass-02.png` | Northneedle | Storm cliffs | 30 | 8 | 3 | 4 | 189,317,640,960 | 0,0,1024,1536 | reviewed |  |
| `conchphone-01.png` | Conchhorn | Coast | 45 | 5 | 5 | 5 | 159,538,640,960 | 0,0,1024,1536 | reviewed |  |
| `corkscrew-01.png` | Twistcork | Ruins | 30 | 8 | 3 | 4 | 207,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-01.png` | Crab Alchemist | Coast | 55 | 6 | 4 | 6 | 266,302,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-02.png` | Crab Smith | Coast | 50 | 5 | 5 | 6 | 222,287,640,960 | 0,0,1024,1536 | reviewed |  |
| `crab-03.png` | Decoy Crab | Coast | 50 | 5 | 5 | 6 | 297,318,640,960 | 0,0,1024,1536 | reviewed |  |
| `crane-01.png` | Hoist Titan | Cathedral | 75 | 3 | 6 | 8 | 226,14,640,960 | 0,0,1024,1536 | reviewed |  |
| `crane-02.png` | Featherveil Crane | Wet forest | 40 | 5 | 5 | 6 | 135,81,640,960 | 0,0,1024,1536 | reviewed |  |
| `cricket-01.png` | Fiddlewhit | Flower meadow | 20 | 8 | 3 | 2 | 178,50,640,960 | 0,0,1024,1536 | reviewed |  |
| `crocodile-01.png` | Crocodile Shaman | Swamp | 40 | 3 | 7 | 6 | 266,290,747,1121 | 0,0,1024,1536 | reviewed |  |
| `crow-01.png` | Skullcaw | Ruins | 20 | 8 | 3 | 4 | 186,107,640,960 | 0,0,1024,1536 | reviewed |  |
| `crucible-01.png` | Moltencup | Ashlands | 55 | 5 | 5 | 7 | 205,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `cuttlefish-01.png` | Needleglow | Lagoon | 20 | 8 | 3 | 1 | 190,214,640,960 | 0,0,1024,1536 | reviewed |  |
| `deer-01.png` | Glade Stag | Wet forest | 40 | 6 | 5 | 4 | 256,288,640,960 | 0,0,1024,1536 | reviewed |  |
| `dormouse-01.png` | Bellnap Dormouse | Autumn woods | 20 | 8 | 3 | 2 | 203,191,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-01.png` | Emberwyrm Colossus | Ashlands | 70 | 3 | 6 | 9 | 191,68,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-02.png` | Obsidian Elder | Ashlands | 75 | 3 | 6 | 10 | 183,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-03.png` | Tempest Sovereign | Storm cliffs | 70 | 3 | 6 | 9 | 221,71,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragon-04.png` | Glacier Titan | Arctic | 75 | 3 | 6 | 10 | 300,15,640,960 | 0,0,1024,1536 | reviewed |  |
| `dragonfly-01.png` | Reedarrow | Swamp | 20 | 8 | 3 | 3 | 342,151,640,960 | 0,0,1024,1536 | reviewed |  |
| `drake-01.png` | Crystalback Drake | Crystal caverns | 75 | 3 | 6 | 8 | 382,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `drawbridge-01.png` | Chaingate | Cathedral | 75 | 3 | 6 | 8 | 173,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `drum-01.png` | Wardrum | Savannah | 50 | 5 | 5 | 7 | 158,87,640,960 | 0,0,1024,1536 | reviewed |  |
| `dugong-02.png` | Crystalfin Dugong | Lagoon | 70 | 3 | 6 | 8 | 307,136,640,960 | 0,0,1024,1536 | reviewed |  |
| `elephant-01.png` | Idolbearer | Savannah | 70 | 3 | 6 | 8 | 370,101,640,960 | 0,0,1024,1536 | reviewed |  |
| `ferret-01.png` | Maskwhisk | Ruins | 20 | 8 | 3 | 1 | 37,67,640,960 | 0,0,1024,1536 | reviewed |  |
| `fiddlercrab-01.png` | Coinclaw | Coast | 30 | 8 | 3 | 4 | 296,236,640,960 | 0,0,1024,1536 | reviewed |  |
| `firefly-01.png` | Lanternwing | Flower meadow | 20 | 8 | 3 | 3 | 84,128,640,960 | 0,0,1024,1536 | reviewed |  |
| `firefly-02.png` | Firefly | Flower meadow | 20 | 8 | 3 | 3 | 339,425,640,960 | 0,0,1024,1536 | reviewed |  |
| `flyingfish-01.png` | Dartfin | Lagoon | 20 | 8 | 3 | 3 | 388,153,640,960 | 0,0,1024,1536 | reviewed |  |
| `fossa-01.png` | Harpoon Fossa | Jungle | 40 | 5 | 5 | 5 | 348,194,640,960 | 0,0,1024,1536 | reviewed |  |
| `fox-01.png` | Fox Bard | Autumn woods | 20 | 8 | 3 | 1 | 55,415,640,960 | 0,0,1024,1536 | reviewed |  |
| `frigatebird-01.png` | Throatdrum | Coast | 40 | 5 | 5 | 6 | 356,75,640,960 | 0,0,1024,1536 | reviewed |  |
| `frog-01.png` | Frog Bard Dagger | Swamp | 20 | 8 | 3 | 2 | 268,45,640,960 | 0,0,1024,1536 | reviewed |  |
| `furnace-01.png` | Furnace Titan | Ashlands | 75 | 3 | 6 | 10 | 280,11,640,960 | 0,0,1024,1536 | reviewed |  |
| `gargoyle-01.png` | Stonewatch Gargoyle | Cathedral | 55 | 5 | 5 | 7 | 220,155,640,960 | 0,0,1024,1536 | reviewed |  |
| `gaur-01.png` | Bolted Gaur | Jungle | 70 | 3 | 6 | 8 | 346,226,640,960 | 0,0,1024,1536 | reviewed |  |
| `gecko-01.png` | Resinpaw | Jungle | 20 | 8 | 3 | 3 | 291,119,640,960 | 0,0,1024,1536 | reviewed |  |
| `gecko-02.png` | Prismtoe | Crystal caverns | 20 | 8 | 3 | 3 | 235,12,640,960 | 0,0,1024,1536 | reviewed |  |
| `goat-01.png` | Totemhorn | Mountain | 40 | 5 | 5 | 5 | 190,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `goblet-01.png` | Grailward | Cathedral | 50 | 5 | 5 | 7 | 264,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `golem-01.png` | Furnaceheart Colossus | Ashlands | 75 | 3 | 6 | 9 | 266,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `golem-02.png` | Gearwright Golem | Ruins | 75 | 3 | 6 | 9 | 237,152,640,960 | 0,0,1024,1536 | reviewed |  |
| `gong-01.png` | Discord Gong | Ruins | 50 | 5 | 5 | 7 | 233,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `gorilla-01.png` | Ironfist Silverback | Jungle | 60 | 3 | 6 | 8 | 242,57,640,960 | 0,0,1024,1536 | reviewed |  |
| `griffin-01.png` | Lancewing Griffin | Mountain | 65 | 3 | 6 | 8 | 193,76,640,960 | 0,0,1024,1536 | reviewed |  |
| `harpsichord-01.png` | Keyclatter | Cathedral | 65 | 3 | 6 | 7 | 181,256,640,960 | 0,0,1024,1536 | reviewed |  |
| `harpy-eagle-01.png` | Talonstrike | Jungle | 40 | 5 | 5 | 5 | 299,87,640,960 | 0,0,1024,1536 | reviewed |  |
| `hawk-01.png` | Skybow Hawk | Mountain | 20 | 8 | 3 | 3 | 264,105,640,960 | 0,0,1024,1536 | reviewed |  |
| `hermitcrab-01.png` | Basaltshell | Coast | 55 | 5 | 5 | 6 | 277,291,640,960 | 0,0,1024,1536 | reviewed |  |
| `heron-01.png` | Wingwright Heron | Swamp | 40 | 5 | 5 | 6 | 288,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `heron-02.png` | Stillspear | Swamp | 40 | 5 | 5 | 5 | 215,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `hippo-01.png` | Tonic Hippo | Swamp | 60 | 3 | 6 | 8 | 298,152,640,960 | 0,0,1024,1536 | reviewed |  |
| `hornbill-01.png` | Drumbill | Jungle | 40 | 5 | 5 | 5 | 274,44,640,960 | 0,0,1024,1536 | reviewed |  |
| `horseshoe-01.png` | Lucky Ironshoe | Savannah | 55 | 5 | 5 | 7 | 230,97,640,960 | 0,0,1024,1536 | reviewed |  |
| `horseshoecrab-01.png` | Bronzeback Sentinel | Coast | 55 | 5 | 5 | 7 | 180,239,640,960 | 0,0,1024,1536 | reviewed |  |
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
| `human-09.png` | Gunnar Holt | Mountain | 40 | 5 | 5 | 4 | 192,84,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-10.png` | Felix Lowell | Cathedral | 45 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-11.png` | Lorenzo Varga | Flower meadow | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-12.png` | Jack Morrow | Coast | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-13.png` | Helena Strand | Cathedral | 45 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-14.png` | Carlos Fonseca | Coast | 40 | 5 | 5 | 5 | 192,166,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-15.png` | Beatrix Alvarez | Coast | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-16.png` | Agnes Brook | Cathedral | 40 | 5 | 5 | 6 | 192,34,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-17.png` | Amara Okafor | Savannah | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-18.png` | Nadia Petrova | Desert | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-19.png` | Fiona Ashby | Wet forest | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-20.png` | Ivy Calloway | Pine forest | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-21.png` | Camille Moreau | Cathedral | 40 | 5 | 5 | 5 | 192,81,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-22.png` | Lukas Weiss | Crystal caverns | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-23.png` | Max Hooper | Ruins | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-24.png` | Ingrid Lund | Arctic | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-25.png` | Leo Stone | Mountain | 40 | 5 | 5 | 5 | 192,151,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-26.png` | Peter Holm | Cathedral | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-27.png` | Johanna Weber | Cathedral | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-28.png` | Albrecht Lang | Cathedral | 50 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-29.png` | Johanka Weber | Mountain | 55 | 5 | 5 | 7 | 192,99,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-30.png` | Edwin Marsh | Cathedral | 55 | 5 | 5 | 7 | 192,165,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-31.png` | William Hart | Cathedral | 55 | 5 | 5 | 7 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-32.png` | Kai Lindqvist | Crystal caverns | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-33.png` | Carmen Costa | Ashlands | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-34.png` | Astor Nilsen | Arctic | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-35.png` | Henrik Berg | Mountain | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-36.png` | Mateo Santos | Lagoon | 40 | 5 | 5 | 6 | 192,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-37.png` | Kenji Mori | Mountain | 40 | 5 | 7 | 6 | 192,89,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-38.png` | Viktor Graves | Ruins | 40 | 5 | 5 | 5 | 214,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-39.png` | Martin Becker | Cathedral | 40 | 5 | 5 | 5 | 192,38,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-40.png` | Luna Ortiz | Flower meadow | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-41.png` | Aurelia Day | Desert | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-42.png` | Thomas Abbott | Cathedral | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-43.png` | Paul Fischer | Swamp | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-44.png` | Nika Volkova | Ruins | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-45.png` | Daniel Hart | Ruins | 40 | 5 | 5 | 4 | 192,132,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-46.png` | Elsa Lindgren | Pine forest | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-47.png` | Kofi Mensah | Jungle | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-48.png` | Hugo Schmidt | Mountain | 40 | 5 | 5 | 5 | 192,111,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-49.png` | Laura Cole | Storm cliffs | 40 | 5 | 5 | 5 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-51.png` | Richard Novak | Storm cliffs | 50 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `human-52.png` | Arthur Pembroke | Cathedral | 40 | 5 | 5 | 6 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `hummingbird-01.png` | Rapierbeak | Flower meadow | 20 | 8 | 3 | 2 | 206,170,640,960 | 0,0,1024,1536 | reviewed |  |
| `hydra-01.png` | Tridentmaw Hydra | Swamp | 70 | 3 | 6 | 8 | 175,-67,729,1094 | 0,0,1024,1536 | reviewed |  |
| `hyena-01.png` | Sawgrin | Savannah | 40 | 5 | 5 | 5 | 332,102,640,960 | 0,0,1024,1536 | reviewed |  |
| `ibex-01.png` | Standardhorn | Mountain | 40 | 5 | 5 | 4 | 219,38,640,960 | 0,0,1024,1536 | reviewed |  |
| `incense-censor-01.png` | Smokeswing | Cathedral | 50 | 5 | 5 | 7 | 183,17,640,960 | 0,0,1024,1536 | reviewed |  |
| `inkwell-01.png` | Quillspatter | Ruins | 25 | 8 | 3 | 4 | 190,410,640,960 | 0,0,1024,1536 | reviewed |  |
| `isopod-01.png` | Ironroll | Mushroom grotto | 35 | 8 | 3 | 4 | 354,359,640,960 | 0,0,1024,1536 | reviewed |  |
| `jackalope-01.png` | Cursehorn | Desert | 20 | 8 | 3 | 2 | 313,225,640,960 | 0,0,1024,1536 | reviewed |  |
| `jelly-01.png` | Stormbell Jelly | Lagoon | 20 | 8 | 3 | 3 | 205,132,640,960 | 0,0,1024,1536 | reviewed |  |
| `jellyfish-02.png` | Mendbloom Jelly | Lagoon | 20 | 8 | 3 | 3 | 192,16,640,960 | 0,0,1024,1536 | reviewed |  |
| `jerboa-01.png` | Acornpop | Desert | 20 | 8 | 3 | 3 | 361,51,640,960 | 0,0,1024,1536 | reviewed |  |
| `kestrel-01.png` | Lurewing | Savannah | 20 | 8 | 3 | 3 | 171,63,640,960 | 0,0,1024,1536 | reviewed |  |
| `kettle-01.png` | Siegekettle | Ruins | 55 | 5 | 5 | 8 | 215,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `kiln-01.png` | Kiln Behemoth | Ashlands | 75 | 3 | 6 | 9 | 212,102,640,960 | 0,0,1024,1536 | reviewed |  |
| `kingfisher-01.png` | Captain Daggerbill | Wet forest | 20 | 8 | 3 | 2 | 206,54,640,960 | 0,0,1024,1536 | reviewed |  |
| `kinkajou-01.png` | Idolpaw | Jungle | 20 | 8 | 3 | 1 | 257,160,640,960 | 0,0,1024,1536 | reviewed |  |
| `kite-01.png` | Tailwind Kite | Storm cliffs | 20 | 8 | 3 | 2 | 262,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `komodo-01.png` | Cleaver Komodo | Jungle | 45 | 5 | 5 | 4 | 269,54,640,960 | 0,0,1024,1536 | reviewed |  |
| `kraken-01.png` | Basalt Kraken | Lagoon | 70 | 3 | 6 | 9 | 164,73,640,960 | 0,0,1024,1536 | reviewed |  |
| `lantern-01.png` | Prismlamp | Crystal caverns | 30 | 8 | 3 | 4 | 168,29,640,960 | 0,0,1024,1536 | reviewed |  |
| `lizardfolk-02.png` | Reef Warden | Lagoon | 45 | 6 | 5 | 7 | 352,64,640,960 | 0,0,1024,1536 | reviewed |  |
| `lizardfolk-03.png` | Axejaw Lizardfolk | Swamp | 40 | 5 | 5 | 4 | 140,22,640,960 | 0,0,1024,1536 | reviewed |  |
| `lizardfolk-04.png` | Kunai Lizardfolk | Swamp | 40 | 5 | 5 | 5 | 223,74,640,960 | 0,0,1024,1536 | reviewed |  |
| `lobster-01.png` | Castanet Lobster | Coast | 50 | 5 | 5 | 5 | 202,138,640,960 | 0,0,1024,1536 | reviewed |  |
| `lobster-02.png` | Rivetclaw | Coast | 50 | 5 | 5 | 6 | 144,105,640,960 | 0,0,1024,1536 | reviewed |  |
| `lockbox-01.png` | Lockbox Dreadnought | Ruins | 75 | 3 | 6 | 8 | 260,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `loom-01.png` | Threadbinder Loom | Ruins | 65 | 3 | 6 | 8 | 232,61,640,960 | 0,0,1024,1536 | reviewed |  |
| `lyrebird-01.png` | Plumesong | Pine forest | 20 | 8 | 3 | 3 | 311,109,640,960 | 0,0,1024,1536 | reviewed |  |
| `mammoth-01.png` | Tundra Titan | Arctic | 65 | 3 | 6 | 8 | 244,97,640,960 | 0,0,1024,1536 | reviewed |  |
| `manatee-01.png` | Shellmend Manatee | Lagoon | 60 | 3 | 6 | 7 | 278,103,640,960 | 0,0,1024,1536 | reviewed |  |
| `mandrake-01.png` | Screamroot | Swamp | 25 | 8 | 3 | 4 | 269,27,640,960 | 0,0,1024,1536 | reviewed |  |
| `mandrill-01.png` | Glovegrin | Jungle | 40 | 5 | 5 | 4 | 221,59,640,960 | 0,0,1024,1536 | reviewed |  |
| `mannequin-01.png` | Stitchfist Mannequin | Ruins | 50 | 5 | 5 | 7 | 96,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `manticore-01.png` | Barbtail Manticore | Desert | 60 | 3 | 6 | 8 | 334,246,640,960 | 0,0,1024,1536 | reviewed |  |
| `mantis-01.png` | Bladewright Mantis | Jungle | 45 | 5 | 5 | 6 | 290,54,640,960 | 0,0,1024,1536 | reviewed |  |
| `marabou-01.png` | Balmbeak | Savannah | 40 | 5 | 5 | 5 | 259,19,640,960 | 0,0,1024,1536 | reviewed |  |
| `maskrack-01.png` | Echomask Rack | Ruins | 45 | 5 | 5 | 6 | 253,-31,640,960 | 0,0,1024,1536 | reviewed |  |
| `mastiff-01.png` | Shieldjaw Mastiff | Cathedral | 50 | 5 | 5 | 7 | 326,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `meerkat-01.png` | Spyglass Meerkat | Desert | 20 | 8 | 3 | 3 | 170,149,640,960 | 0,0,1024,1536 | reviewed |  |
| `millstone-01.png` | Millstone Juggernaut | Savannah | 75 | 3 | 6 | 9 | 250,87,640,960 | 0,0,1024,1536 | reviewed |  |
| `mirror-01.png` | Mirror Revenant | Cathedral | 45 | 5 | 5 | 7 | 178,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `mole-01.png` | Gearclaw Mole | Mushroom grotto | 20 | 8 | 3 | 1 | 330,126,640,960 | 0,0,1024,1536 | reviewed |  |
| `mole-02.png` | Hexhammer Mole | Mushroom grotto | 20 | 8 | 3 | 1 | 258,222,640,960 | 0,0,1024,1536 | reviewed |  |
| `mongoose-01.png` | Sicklewhisk | Savannah | 20 | 8 | 3 | 2 | 343,113,640,960 | 0,0,1024,1536 | reviewed |  |
| `mongoose-02.png` | Hexdart Mongoose | Savannah | 20 | 8 | 3 | 1 | 250,109,640,960 | 0,0,1024,1536 | reviewed |  |
| `monkey-01.png` | Slingtail | Jungle | 20 | 8 | 3 | 3 | 150,113,640,960 | 0,0,1024,1536 | reviewed |  |
| `moray-01.png` | Hookgrin Moray | Lagoon | 40 | 5 | 5 | 5 | 375,51,640,960 | 0,0,1024,1536 | reviewed |  |
| `mortar-01.png` | Bastion Mortar | Ruins | 75 | 3 | 6 | 9 | 176,43,640,960 | 0,0,1024,1536 | reviewed |  |
| `moth-02.png` | Lanternmoth | Mushroom grotto | 20 | 8 | 3 | 4 | 181,130,640,960 | 0,0,1024,1536 | reviewed |  |
| `mushroom-01.png` | Sporedruid | Mushroom grotto | 20 | 8 | 3 | 3 | 192,223,640,960 | 0,0,1024,1536 | reviewed |  |
| `mushroom-03.png` | Capshield | Mushroom grotto | 45 | 5 | 5 | 6 | 182,189,640,960 | 0,0,1024,1536 | reviewed |  |
| `musicbox-01.png` | Windup Drummer | Ruins | 35 | 8 | 3 | 4 | 188,88,640,960 | 0,0,1024,1536 | reviewed |  |
| `muskox-01.png` | Bucklerhorn | Arctic | 70 | 3 | 6 | 8 | 292,187,640,960 | 0,0,1024,1536 | reviewed |  |
| `newt-01.png` | Coilnewt | Swamp | 20 | 8 | 3 | 1 | 392,265,640,960 | 0,0,1024,1536 | reviewed |  |
| `olm-01.png` | Shadeolm | Crystal caverns | 20 | 8 | 3 | 3 | 194,45,640,960 | 0,0,1024,1536 | reviewed |  |
| `opossum-01.png` | Dicetail | Autumn woods | 20 | 8 | 3 | 3 | 335,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `orc-01.png` | Grukk Skullcaller | Ruins | 45 | 5 | 5 | 5 | 224,20,640,960 | 0,0,1024,1536 | reviewed |  |
| `orchidmantis-01.png` | Petalshade | Flower meadow | 20 | 8 | 3 | 2 | 240,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `organ-01.png` | Pipe Organ Choir | Cathedral | 75 | 3 | 6 | 10 | 181,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `orrery-01.png` | Orbitwright | Cathedral | 45 | 5 | 5 | 6 | 211,76,640,960 | 0,0,1024,1536 | reviewed |  |
| `ossuary-01.png` | Bonecube | Ruins | 55 | 5 | 5 | 7 | 235,55,640,960 | 0,0,1024,1536 | reviewed |  |
| `otter-01.png` | Slingwhisker | Wet forest | 20 | 8 | 3 | 3 | 268,142,640,960 | 0,0,1024,1536 | reviewed |  |
| `owl-01.png` | Hootmage | Pine forest | 20 | 8 | 3 | 4 | 192,97,640,960 | 0,0,1024,1536 | reviewed |  |
| `panda-01.png` | Bamboo Monk | Mountain | 60 | 3 | 6 | 6 | 181,48,640,960 | 0,0,1024,1536 | reviewed |  |
| `pangolin-01.png` | Decoy Pangolin | Savannah | 50 | 5 | 5 | 7 | 354,150,640,960 | 0,0,1024,1536 | reviewed |  |
| `pangolin-02.png` | Thunderscale | Savannah | 50 | 5 | 5 | 7 | 372,184,640,960 | 0,0,1024,1536 | reviewed |  |
| `panpipe-01.png` | Reedcaller | Flower meadow | 25 | 8 | 3 | 4 | 189,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `peacock-01.png` | Plumecrown | Flower meadow | 40 | 5 | 5 | 6 | 399,53,640,960 | 0,0,1024,1536 | reviewed |  |
| `phoenix-01.png` | Solar Phoenix | Ashlands | 60 | 3 | 6 | 8 | 188,12,640,960 | 0,0,1024,1536 | reviewed |  |
| `pika-01.png` | Hornpip | Mountain | 20 | 8 | 3 | 3 | 207,153,640,960 | 0,0,1024,1536 | reviewed |  |
| `platypus-01.png` | Runebill | Wet forest | 20 | 8 | 3 | 1 | 335,207,640,960 | 0,0,1024,1536 | reviewed |  |
| `plough-01.png` | Furrow Ravager | Savannah | 75 | 3 | 6 | 9 | 306,233,640,960 | 0,0,1024,1536 | reviewed |  |
| `pocketwatch-01.png` | Tickwarden | Ruins | 35 | 8 | 3 | 4 | 246,37,711,1067 | 0,0,1024,1536 | reviewed |  |
| `porcupine-01.png` | Quillflask | Autumn woods | 25 | 8 | 3 | 4 | 359,185,640,960 | 0,0,1024,1536 | reviewed |  |
| `portcullis-01.png` | Portcullis Colossus | Cathedral | 75 | 3 | 6 | 8 | 263,-21,640,960 | 0,0,1024,1536 | reviewed |  |
| `printingpress-01.png` | Typesetter | Ruins | 75 | 3 | 6 | 10 | 229,61,640,960 | 0,0,1024,1536 | reviewed |  |
| `prism-01.png` | Shardhex | Crystal caverns | 30 | 8 | 3 | 4 | 300,243,640,960 | 0,0,1024,1536 | reviewed |  |
| `puffin-01.png` | Shieldpuff | Arctic | 20 | 8 | 3 | 3 | 337,221,640,960 | 0,0,1024,1536 | reviewed |  |
| `quern-01.png` | Grindstone Oracle | Ruins | 55 | 5 | 5 | 7 | 218,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `quokka-01.png` | Pebblegrin | Savannah | 20 | 8 | 3 | 2 | 119,132,640,960 | 0,0,1024,1536 | reviewed |  |
| `raccoon-01.png` | Potionpaw | Autumn woods | 20 | 8 | 3 | 3 | 211,124,640,960 | 0,0,1024,1536 | reviewed |  |
| `ram-01.png` | Sunhorn Paladin | Mountain | 50 | 5 | 5 | 7 | 275,62,640,960 | 0,0,1024,1536 | reviewed |  |
| `ram-02.png` | Bellows Ram | Mountain | 40 | 5 | 5 | 5 | 204,42,640,960 | 0,0,1024,1536 | reviewed |  |
| `reliquary-01.png` | Sealed Reliquary | Cathedral | 55 | 5 | 5 | 8 | 191,-3,640,960 | 0,0,1024,1536 | reviewed |  |
| `retort-01.png` | Bubblehex Retort | Ruins | 35 | 8 | 3 | 4 | 187,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `rhinoceros-01.png` | Shadowhorn | Savannah | 70 | 3 | 6 | 8 | 247,203,640,960 | 0,0,1024,1536 | reviewed |  |
| `roadrunner-01.png` | Whistledash | Desert | 20 | 8 | 3 | 3 | 141,15,640,960 | 0,0,1024,1536 | reviewed |  |
| `roc-01.png` | Thunder Roc | Storm cliffs | 60 | 3 | 6 | 7 | 278,95,640,960 | 0,0,1024,1536 | reviewed |  |
| `rooster-01.png` | Banner Cock | Savannah | 20 | 8 | 3 | 2 | 133,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `sableantelope-01.png` | Pennant Sable | Savannah | 40 | 5 | 5 | 5 | 301,60,640,960 | 0,0,1024,1536 | reviewed |  |
| `safe-01.png` | Vaulthex | Ruins | 75 | 3 | 6 | 8 | 258,58,640,960 | 0,0,1024,1536 | reviewed |  |
| `sail-01.png` | Nightsail | Storm cliffs | 40 | 5 | 5 | 6 | 284,167,640,960 | 0,0,1024,1536 | reviewed |  |
| `salamander-01.png` | Slinkember | Ashlands | 20 | 8 | 3 | 1 | 7,367,640,960 | 0,0,1024,1536 | reviewed |  |
| `salamander-02.png` | Flaskember | Ashlands | 20 | 8 | 3 | 3 | 306,240,640,960 | 0,0,1024,1536 | reviewed |  |
| `sarcophagus-01.png` | Tombwarden | Desert | 75 | 3 | 6 | 10 | 250,9,640,960 | 0,0,1024,1536 | reviewed |  |
| `scarab-01.png` | Runeshell Scarab | Desert | 30 | 8 | 3 | 4 | 375,286,640,960 | 0,0,1024,1536 | reviewed |  |
| `scorpion-01.png` | Stingtail | Desert | 45 | 5 | 5 | 6 | 124,428,640,960 | 0,0,1024,1536 | reviewed |  |
| `scorpion-02.png` | Ribbonsting | Desert | 45 | 5 | 5 | 6 | 336,378,640,960 | 0,0,1024,1536 | reviewed |  |
| `seal-01.png` | Bottlenose Bard | Arctic | 40 | 5 | 5 | 5 | 292,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `sealpress-01.png` | Waxseal Press | Cathedral | 55 | 5 | 5 | 8 | 253,-6,640,960 | 0,0,1024,1536 | reviewed |  |
| `secretarybird-01.png` | Javelin Stalker | Savannah | 40 | 5 | 5 | 5 | 313,7,640,960 | 0,0,1024,1536 | reviewed |  |
| `serpent-01.png` | Crowned Serpent | Jungle | 65 | 3 | 6 | 8 | 286,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `serval-01.png` | Boltear | Savannah | 20 | 8 | 3 | 3 | 321,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `sewingbox-01.png` | Stitchkit | Ruins | 25 | 8 | 3 | 4 | 138,266,640,960 | 0,0,1024,1536 | reviewed |  |
| `sextant-01.png` | Admiral Starsight | Coast | 55 | 5 | 5 | 7 | 228,-3,640,960 | 0,0,1024,1536 | reviewed |  |
| `shield-01.png` | Vanguard Aegis | Cathedral | 55 | 5 | 5 | 7 | 175,52,640,960 | 0,0,1024,1536 | reviewed |  |
| `shoebill-01.png` | Pikebill | Swamp | 40 | 5 | 5 | 5 | 216,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `shrew-01.png` | Totemshrew | Pine forest | 20 | 8 | 3 | 3 | 218,116,640,960 | 0,0,1024,1536 | reviewed |  |
| `siphon-01.png` | Siphonworm | Ruins | 55 | 5 | 5 | 7 | 223,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `sistrum-01.png` | Rattlechant | Desert | 35 | 8 | 3 | 4 | 172,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `sledgehammer-01.png` | Sledge Colossus | Mountain | 75 | 3 | 6 | 9 | 217,33,640,960 | 0,0,1024,1536 | reviewed |  |
| `sloth-01.png` | Barkhold Sloth | Jungle | 45 | 5 | 5 | 5 | 248,168,640,960 | 0,0,1024,1536 | reviewed |  |
| `snail-01.png` | Prism Snail | Crystal caverns | 25 | 8 | 3 | 4 | 331,244,640,960 | 0,0,1024,1536 | reviewed |  |
| `snail-05.png` | Brewshell | Mushroom grotto | 25 | 8 | 3 | 4 | 292,319,640,960 | 0,0,1024,1536 | reviewed |  |
| `spikeback-01.png` | Spikeback | Desert | 65 | 4 | 6 | 9 | 256,355,768,1152 | 0,0,1024,1536 | reviewed |  |
| `spindle-01.png` | Spindlewhirl | Ruins | 25 | 8 | 3 | 3 | 246,-31,640,960 | 0,0,1024,1536 | reviewed |  |
| `stag-01.png` | Shieldhorn Stag | Pine forest | 40 | 5 | 5 | 6 | 333,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `stoat-01.png` | Needlewhisk | Arctic | 20 | 8 | 3 | 2 | 380,230,640,960 | 0,0,1024,1536 | reviewed |  |
| `sugarglider-01.png` | Sailglide | Jungle | 20 | 8 | 3 | 3 | 208,148,640,960 | 0,0,1024,1536 | reviewed |  |
| `sundial-01.png` | Noonward | Ruins | 55 | 5 | 5 | 7 | 356,222,640,960 | 0,0,1024,1536 | reviewed |  |
| `sundrum-01.png` | Sunhex Drum | Desert | 55 | 5 | 5 | 8 | 182,-65,640,960 | 0,0,1024,1536 | reviewed |  |
| `tapestry-01.png` | Woven Host | Cathedral | 55 | 5 | 5 | 8 | 279,-22,640,960 | 0,0,1024,1536 | reviewed |  |
| `tarsier-01.png` | Maskeye | Jungle | 20 | 8 | 3 | 1 | 166,120,640,960 | 0,0,1024,1536 | reviewed |  |
| `teapot-01.png` | Volcano Teapot | Ashlands | 30 | 8 | 3 | 4 | 198,123,640,960 | 0,0,1024,1536 | reviewed |  |
| `termite-01.png` | Drillmite | Savannah | 25 | 8 | 3 | 4 | 333,212,640,960 | 0,0,1024,1536 | reviewed |  |
| `termite-02.png` | Clockmite | Savannah | 25 | 8 | 3 | 4 | 288,107,640,960 | 0,0,1024,1536 | reviewed |  |
| `thorntreant-01.png` | Bramblestalker | Wet forest | 70 | 3 | 6 | 8 | 237,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `toad-01.png` | Boulder Toad | Mountain | 70 | 2 | 7 | 9 | 256,304,640,960 | 0,0,1024,1536 | reviewed |  |
| `toad-02.png` | Druid Toad | Swamp | 40 | 5 | 5 | 5 | 352,284,640,960 | 0,0,1024,1536 | reviewed |  |
| `toad-03.png` | Belltoad | Swamp | 40 | 5 | 5 | 6 | 243,201,640,960 | 0,0,1024,1536 | reviewed |  |
| `tortoise-01.png` | Mountainshell | Mountain | 75 | 3 | 6 | 10 | 337,85,640,960 | 0,0,1024,1536 | reviewed |  |
| `tortoise-02.png` | Obsidian Tortoise | Ashlands | 75 | 3 | 6 | 10 | 353,293,640,960 | 0,0,1024,1536 | reviewed |  |
| `treant-01.png` | Leafwarden | Wet forest | 65 | 3 | 6 | 8 | 230,35,640,960 | 0,0,1024,1536 | reviewed |  |
| `treant-02.png` | Elderbough | Wet forest | 70 | 3 | 6 | 8 | 190,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `trowel-01.png` | Trowel Crusader | Ruins | 55 | 5 | 5 | 7 | 228,56,640,960 | 0,0,1024,1536 | reviewed |  |
| `tuningfork-01.png` | Resonant Champion | Cathedral | 55 | 5 | 5 | 7 | 256,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `turbine-01.png` | Skyturbine | Storm cliffs | 55 | 5 | 5 | 8 | 161,0,640,960 | 0,0,1024,1536 | reviewed |  |
| `turtle-02.png` | Shellmonk | Coast | 50 | 5 | 5 | 7 | 192,75,640,960 | 0,0,1024,1536 | reviewed |  |
| `turtle-03.png` | Frostshell Bard | Arctic | 50 | 5 | 5 | 7 | 384,244,640,960 | 0,0,1024,1536 | reviewed |  |
| `umbrella-01.png` | Fanbrella | Storm cliffs | 25 | 8 | 3 | 4 | 217,5,640,960 | 0,0,1024,1536 | reviewed |  |
| `velvetworm-01.png` | Runevelvet | Wet forest | 20 | 8 | 3 | 2 | 384,255,640,960 | 0,0,1024,1536 | reviewed |  |
| `vinegaroon-01.png` | Vinegar Flask | Desert | 25 | 8 | 3 | 4 | 205,289,640,960 | 0,0,1024,1536 | reviewed |  |
| `vise-01.png` | Vise Enforcer | Ruins | 55 | 5 | 5 | 7 | 250,185,640,960 | 0,0,1024,1536 | reviewed |  |
| `vulture-01.png` | Cinderbeak | Desert | 40 | 5 | 5 | 6 | 173,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `walrus-01.png` | Tuskhorn Walrus | Arctic | 60 | 3 | 6 | 7 | 176,136,640,960 | 0,0,1024,1536 | reviewed |  |
| `warthog-01.png` | Cannon Warthog | Savannah | 45 | 5 | 5 | 5 | 314,46,640,960 | 0,0,1024,1536 | reviewed |  |
| `wasp-01.png` | Stingflask | Flower meadow | 20 | 8 | 3 | 3 | 310,143,640,960 | 0,0,1024,1536 | reviewed |  |
| `weasel-01.png` | Daggerwhisk | Autumn woods | 20 | 8 | 3 | 2 | 351,162,640,960 | 0,0,1024,1536 | reviewed |  |
| `weathervane-01.png` | Gustarrow | Storm cliffs | 55 | 5 | 5 | 8 | 288,37,640,960 | 0,0,1024,1536 | reviewed |  |
| `weaverbird-01.png` | Bannerweave | Savannah | 20 | 8 | 3 | 3 | 155,76,640,960 | 0,0,1024,1536 | reviewed |  |
| `werewolf-01.png` | Thornfang | Pine forest | 40 | 7 | 5 | 6 | 344,102,640,960 | 0,0,1024,1536 | reviewed |  |
| `wisp-01.png` | Shadowwisp | Ruins | 20 | 8 | 3 | 3 | 289,78,640,960 | 0,0,1024,1536 | reviewed |  |
| `wolf-01.png` | Ranger Wolf | Pine forest | 50 | 5 | 5 | 6 | 331,152,640,960 | 0,0,1024,1536 | reviewed |  |
| `wolverine-01.png` | Macefang Wolverine | Arctic | 40 | 5 | 5 | 4 | 277,180,640,960 | 0,0,1024,1536 | reviewed |  |
| `wyvern-01.png` | Obsidian Wyvern | Ashlands | 65 | 3 | 6 | 8 | 357,150,640,960 | 0,0,1024,1536 | reviewed |  |
| `yak-01.png` | Gongyak | Mountain | 65 | 3 | 6 | 7 | 244,79,800,1200 | 0,0,1024,1536 | reviewed |  |
