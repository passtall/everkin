# Run 20261118-213438-harness

AI level **harness**, seeds 1 to 10000, rules commit `83c422d2ea2a24a5dd3dc9590a52e6641d6668ad`.

- Battles recorded: 3397 (3396 played to an end, 1 crashed)
- Side A wins 1609, side B wins 1657, draws 130 (3.8%), of which 130 hit the length cap
- Battle length in unit turns: median 164, mean 193.6, shortest 50, longest 512
- Average real time per battle: 18.1 s

## Momentum per side

The meter is sampled after every unit turn; lead is the side's own view (positive = ahead).

| Side | Samples | Ahead % | Level % | Behind % | Average lead | Payments per battle | Momentum paid per battle | Turns it could pay | Of those, did not pay % |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 444262 | 39.2 | 19.8 | 41.0 | -0.1 | 15.6 | 26.1 | 90488 | 41.4 |
| B | 444262 | 41.0 | 19.8 | 39.2 | 0.1 | 16.3 | 26.9 | 95132 | 41.8 |

Most paid-for skills:

| Skill | Payments | Momentum paid |
|---|---:|---:|
| lift_a_skill | 11314 | 11314 |
| clay_golem | 10423 | 20674 |
| shield_bash | 8436 | 8436 |
| song_of_haste | 7775 | 7775 |
| divine_intervention | 6178 | 18534 |
| rampage | 6028 | 6028 |
| hunters_shot | 5976 | 11952 |
| lullaby | 5799 | 11598 |
| dive | 4325 | 8650 |
| lightning_bolt | 4000 | 8000 |
| piercing_enchant | 3891 | 3891 |
| bolt_turret | 3739 | 3739 |
| life_swap | 2996 | 8988 |
| shield_dome | 2994 | 8982 |
| rotation | 2968 | 5850 |

## Imbalance flags: top and bottom 10% by win rate (30 each, creatures with at least 20 appearances)

| | Creature | Appearances | Win rate % | Avg damage | Avg healing |
|---|---|---:|---:|---:|---:|
| top | human-rogue-shadow | 132 | 84.8 | 127.2 | 0.0 |
| top | brazier-summoner-coal | 163 | 82.8 | 119.6 | 0.0 |
| top | hummingbird-duelist-rapier | 28 | 82.1 | 23.0 | 0.7 |
| top | pangolin-sorcerer-thunder | 166 | 81.3 | 134.7 | 0.0 |
| top | reliquary-summoner-seal | 122 | 81.1 | 137.3 | 0.0 |
| top | yak-bard-gong | 200 | 81.0 | 92.3 | 0.0 |
| top | quokka-summoner-pebble | 30 | 80.0 | 46.1 | 0.0 |
| top | tapestry-summoner | 157 | 79.6 | 127.9 | 0.0 |
| top | tortoise-sentinel-obsidian | 52 | 78.8 | 132.8 | 0.0 |
| top | badger-summoner-drum | 87 | 78.2 | 95.5 | 0.0 |
| top | sealpress-summoner | 127 | 78.0 | 126.3 | 0.0 |
| top | kestrel-hunter-lure | 40 | 77.5 | 85.2 | 0.0 |
| top | incense-censor | 99 | 76.8 | 102.8 | 0.0 |
| top | human-scout-lantern | 98 | 76.5 | 110.0 | 6.7 |
| top | capybara-guard-calm | 123 | 76.4 | 97.8 | 14.0 |
| top | maskrack-summoner-echo | 143 | 75.5 | 102.0 | 0.0 |
| top | firefly-summoner-lantern | 47 | 74.5 | 72.8 | 0.0 |
| top | mole-engineer-gear | 31 | 74.2 | 103.9 | 21.4 |
| top | book-summoner-fold | 173 | 73.4 | 58.3 | 0.0 |
| top | crane-summoner-feather | 157 | 73.2 | 110.9 | 0.0 |
| top | termite-summoner-clockwork | 160 | 73.1 | 82.9 | 0.0 |
| top | musicbox-summoner-drum | 63 | 73.0 | 96.5 | 0.0 |
| top | weathervane-summoner-arrow | 96 | 72.9 | 107.5 | 0.0 |
| top | caterpillar-bard | 44 | 72.7 | 3.5 | 0.0 |
| top | printingpress-summoner-type | 71 | 71.8 | 99.0 | 0.0 |
| top | bellows-drake | 194 | 70.6 | 70.9 | 0.1 |
| top | toad-bard-bell | 127 | 70.1 | 99.4 | 0.0 |
| top | kraken-colossus-basalt | 143 | 69.2 | 142.1 | 0.0 |
| top | mandrake-primalist-root | 61 | 68.9 | 80.5 | 25.0 |
| top | ferret-summoner-mask | 32 | 68.8 | 81.7 | 2.2 |
| bottom | orchidmantis-trickster-leaf | 26 | 7.7 | 4.9 | 0.3 |
| bottom | turtle-monk | 44 | 9.1 | 33.1 | 0.0 |
| bottom | fox-bard | 30 | 23.3 | 7.2 | 0.4 |
| bottom | jelly-healer | 34 | 23.5 | 11.9 | 48.1 |
| bottom | golem-engineer-gear | 59 | 25.4 | 29.9 | 0.9 |
| bottom | wasp-alchemist-potion | 31 | 25.8 | 35.9 | 7.2 |
| bottom | ayeaye-hexer-candle | 27 | 25.9 | 19.1 | 1.9 |
| bottom | manatee-healer-shell | 46 | 26.1 | 31.1 | 59.2 |
| bottom | wisp-hexer-shadow | 42 | 26.2 | 19.7 | 0.0 |
| bottom | raccoon-alchemist-potion | 26 | 26.9 | 47.3 | 8.0 |
| bottom | sextant-admiral | 159 | 27.7 | 17.2 | 0.0 |
| bottom | crane-hoist-titan | 39 | 28.2 | 24.6 | 96.1 |
| bottom | tuningfork-champion | 35 | 28.6 | 4.4 | 0.0 |
| bottom | mantis-engineer-blade | 41 | 29.3 | 21.2 | 0.0 |
| bottom | treant-druid-leaf | 30 | 30.0 | 22.4 | 25.5 |
| bottom | cuttlefish-assassin-needle | 20 | 30.0 | 16.8 | 0.3 |
| bottom | moray-trickster-hook | 33 | 30.3 | 21.6 | 0.0 |
| bottom | firefly | 78 | 30.8 | 18.4 | 71.9 |
| bottom | axolotl-mage-water | 45 | 31.1 | 52.7 | 0.0 |
| bottom | beetle-knight-shield | 92 | 31.5 | 29.2 | 0.0 |
| bottom | hippo-healer-potion | 60 | 31.7 | 33.2 | 82.0 |
| bottom | manticore-assassin-barb | 170 | 31.8 | 38.9 | 0.0 |
| bottom | fossa-hunter-harpoon | 208 | 32.2 | 36.2 | 4.4 |
| bottom | hawk-scout-bow | 61 | 32.8 | 18.5 | 0.0 |
| bottom | human-necromancer-skull | 203 | 33.0 | 13.2 | 0.0 |
| bottom | chest-bodyguard-lock | 154 | 33.1 | 30.2 | 0.0 |
| bottom | sugarglider-scout-sail | 27 | 33.3 | 4.8 | 0.0 |
| bottom | turbine-skyguard | 456 | 34.0 | 17.9 | 1.0 |
| bottom | toad-druid | 85 | 34.1 | 11.2 | 34.1 |
| bottom | wyvern-scout-obsidian | 128 | 34.4 | 14.6 | 0.0 |

## Classes and keystones

| Kind | Name | Appearances | Win rate % |
|---|---|---:|---:|
| class | summoner | 4716 | 70.9 |
| class | pyromancer | 578 | 62.3 |
| class | tactician | 859 | 58.0 |
| class | saboteur | 999 | 51.4 |
| class | spiritwalker | 1453 | 51.3 |
| class | bard | 3191 | 51.2 |
| class | berserker | 4768 | 49.8 |
| class | cryomancer | 1074 | 49.6 |
| class | chemist | 2372 | 48.9 |
| class | keeper | 1946 | 48.7 |
| class | bulwark | 6166 | 48.6 |
| class | enchanter | 4124 | 48.3 |
| class | engineer | 5088 | 47.6 |
| class | thief | 4616 | 47.4 |
| class | assassin | 1918 | 47.2 |
| class | commander | 4676 | 46.1 |
| class | champion | 6706 | 45.2 |
| class | lifebinder | 982 | 44.6 |
| class | skyborne | 6478 | 44.6 |
| class | ritualist | 1209 | 44.6 |
| class | stormcaller | 2109 | 44.5 |
| class | tracker | 5448 | 44.2 |
| class | transmuter | 1156 | 44.1 |
| class | necromancer | 1207 | 43.1 |
| class | totemist | 1345 | 42.9 |
| class | mender | 1434 | 42.6 |
| class | cleric | 3165 | 41.3 |
| class | hexer | 1721 | 37.8 |
| keystone | quickening | 1955 | 76.5 |
| keystone | wildfire | 244 | 73.8 |
| keystone | golem_heart | 1382 | 68.8 |
| keystone | portal | 1379 | 65.0 |
| keystone | initiative | 514 | 61.1 |
| keystone | chaos_agent | 612 | 59.6 |
| keystone | inspiring_presence | 220 | 57.7 |
| keystone | bone_armor | 116 | 56.9 |
| keystone | rhythm | 814 | 55.2 |
| keystone | spirit_shield | 189 | 55.0 |
| keystone | inferno | 288 | 54.5 |
| keystone | master_thief | 1260 | 53.7 |
| keystone | martyr | 344 | 53.5 |
| keystone | thorned_ward | 655 | 53.1 |
| keystone | steadfast_ward | 566 | 53.0 |
| keystone | last_stand | 1453 | 53.0 |
| keystone | haunting | 545 | 52.7 |
| keystone | twin_enchant | 1708 | 52.2 |
| keystone | soul_anchor | 434 | 52.1 |
| keystone | brittle | 413 | 51.8 |
| keystone | rapid_fire | 1307 | 51.2 |
| keystone | deep_chill | 274 | 51.1 |
| keystone | grand_chorus | 1495 | 50.6 |
| keystone | potent_brew | 650 | 50.3 |
| keystone | ashwalker | 46 | 50.0 |
| keystone | big_finish | 1090 | 49.6 |
| keystone | unstoppable | 1525 | 49.5 |
| keystone | rally | 1909 | 49.5 |
| keystone | spell_ward | 1051 | 49.3 |
| keystone | puppeteer | 474 | 49.2 |
| keystone | hold_position | 1951 | 48.9 |
| keystone | sniff_the_air | 1607 | 48.8 |
| keystone | unbreakable | 2111 | 48.7 |
| keystone | frostbinder | 318 | 48.7 |
| keystone | static_field | 690 | 48.6 |
| keystone | echo | 882 | 48.5 |
| keystone | shield_wall | 2104 | 48.2 |
| keystone | lingering_malice | 52 | 48.1 |
| keystone | vanishing_kill | 815 | 47.9 |
| keystone | sky_lord | 1820 | 47.7 |
| keystone | unseen | 678 | 47.6 |
| keystone | reinforced | 1790 | 47.6 |
| keystone | reckless_swing | 1790 | 47.5 |
| keystone | purifying_aura | 648 | 46.9 |
| keystone | brittle_glass | 328 | 46.6 |
| keystone | volatile | 632 | 46.4 |
| keystone | cold_heart | 387 | 46.3 |
| keystone | hunt_down | 2149 | 45.7 |
| keystone | escape_artist | 1739 | 45.6 |
| keystone | all_in | 125 | 45.6 |
| keystone | duelist | 2342 | 45.4 |
| keystone | self_repair | 1991 | 45.4 |
| keystone | venom_blades | 425 | 45.2 |
| keystone | drill | 1246 | 44.9 |
| keystone | eagle_eye | 2137 | 44.9 |
| keystone | relentless | 1521 | 44.8 |
| keystone | glory | 2215 | 44.5 |
| keystone | overcharge | 627 | 44.5 |
| keystone | deluge | 560 | 44.5 |
| keystone | shadow_purse | 1617 | 44.3 |
| keystone | triage | 690 | 44.2 |
| keystone | lasting_change | 408 | 43.9 |
| keystone | vanguard | 1521 | 42.9 |
| keystone | lifeblood | 302 | 42.7 |
| keystone | lingering_magic | 1365 | 42.7 |
| keystone | regrowth | 353 | 42.5 |
| keystone | mirror_form | 420 | 42.4 |
| keystone | lift | 2521 | 42.0 |
| keystone | lich_form | 760 | 41.8 |
| keystone | sanctity | 696 | 41.5 |
| keystone | twin_wards | 725 | 41.4 |
| keystone | deaths_bounty | 331 | 41.1 |
| keystone | disarm_trap | 156 | 41.0 |
| keystone | sturdy_totems | 583 | 41.0 |
| keystone | eye_of_the_storm | 792 | 40.9 |
| keystone | great_totem | 573 | 40.8 |
| keystone | tidecaller | 331 | 40.8 |
| keystone | scent_of_blood | 2320 | 40.6 |
| keystone | overflowing_care | 391 | 39.9 |
| keystone | judgment | 1821 | 39.2 |
| keystone | witchs_patience | 1330 | 37.5 |
| keystone | spreading_curse | 339 | 37.5 |
| keystone | phoenix | 336 | 37.2 |
| keystone | wrecker | 231 | 36.4 |

## Skills: best and worst win rate of the units that have them (at least 50 units), and usage

| | Skill | Units | Win rate % | Uses per unit |
|---|---|---:|---:|---:|
| best | void_shard | 132 | 84.8 | 1.6 |
| best | ember_toss | 163 | 82.8 | 0.7 |
| best | thunder_hum | 166 | 81.3 | 1.0 |
| best | relic_glow | 122 | 81.1 | 0.6 |
| best | gong_crash | 200 | 81.0 | 0.1 |
| best | woven_snare | 157 | 79.6 | 1.5 |
| best | obsidian_shard | 52 | 78.8 | 1.1 |
| best | drumroll | 87 | 78.2 | 0.3 |
| best | wax_press | 127 | 78.0 | 0.1 |
| best | incense_cloud | 99 | 76.8 | 0.2 |
| best | lantern_glare | 98 | 76.5 | 0.1 |
| best | water_splash | 123 | 76.4 | 1.4 |
| best | echo_scream | 143 | 75.5 | 0.6 |
| best | paper_cut | 173 | 73.4 | 1.1 |
| best | storm_plume | 157 | 73.2 | 0.2 |
| worst | steam_vent | 59 | 25.4 | 3.1 |
| worst | starsight_ray | 159 | 27.7 | 2.8 |
| worst | glow_dart | 78 | 30.8 | 3.2 |
| worst | carapace_bash | 92 | 31.5 | 3.1 |
| worst | potion_spit | 60 | 31.7 | 1.3 |
| worst | barb_lash | 170 | 31.8 | 3.7 |
| worst | harpoon_throw | 208 | 32.2 | 5.7 |
| worst | pinning_arrow | 61 | 32.8 | 5.7 |
| worst | soul_rend | 203 | 33.0 | 3.2 |
| worst | lid_snap | 154 | 33.1 | 4.7 |
| worst | turbine_blade | 456 | 34.0 | 4.5 |
| worst | spore_croak | 85 | 34.1 | 1.1 |
| worst | petrifying_gaze | 128 | 34.4 | 4.4 |
| worst | horn_butt | 292 | 34.9 | 3.5 |
| worst | river_stone | 74 | 35.1 | 12.2 |

Active skills never used in any battle (3): row_of_fire, cut_the_strings, totem_of_return

## What the AI does with each skill (turns where it was legal, searched further, chosen)

Skills with at least 200 legal turns. Searched = the search looked at one of its uses further; chosen = it was played.

| | Skill | Legal turns | Searched % of legal | Chosen % of legal | Chosen % of searched |
|---|---|---:|---:|---:|---:|
| rarely chosen | row_of_fire | 2440 | 1.1 | 0.0 | 0.0 |
| rarely chosen | totem_of_return | 5676 | 0.2 | 0.0 | 0.0 |
| rarely chosen | cut_the_strings | 2590 | 0.2 | 0.0 | 0.0 |
| rarely chosen | echo_enchant | 22473 | 0.2 | 0.0 | 19.6 |
| rarely chosen | rune_golem | 28115 | 28.2 | 0.1 | 0.2 |
| rarely chosen | read_tracks | 61770 | 14.9 | 0.2 | 1.1 |
| rarely chosen | melt_away | 49960 | 10.8 | 0.3 | 2.7 |
| rarely chosen | spirit_link_totem | 5676 | 58.0 | 0.4 | 0.6 |
| rarely chosen | press_the_advantage | 6196 | 10.2 | 0.5 | 4.4 |
| rarely chosen | rescue | 24479 | 6.1 | 0.5 | 8.4 |
| rarely chosen | tempest | 8125 | 4.3 | 0.5 | 12.2 |
| rarely chosen | sweeping_kick | 12695 | 8.2 | 0.8 | 9.8 |
| rarely chosen | desecrate | 6157 | 26.6 | 0.8 | 3.1 |
| rarely chosen | withering_curse | 18315 | 16.3 | 1.1 | 7.0 |
| rarely chosen | rite_of_mists | 6288 | 9.2 | 1.3 | 13.7 |
| often chosen | shadow_carve | 1162 | 96.8 | 90.6 | 93.6 |
| often chosen | shadow_touch | 587 | 95.6 | 87.6 | 91.6 |
| often chosen | trident_gust | 301 | 93.7 | 87.4 | 93.3 |
| often chosen | orbit_missile | 5913 | 99.3 | 83.7 | 84.2 |
| often chosen | needle_point | 852 | 97.7 | 83.0 | 85.0 |
| often chosen | soul_rend | 800 | 94.4 | 82.2 | 87.2 |
| often chosen | skull_hex | 238 | 92.0 | 81.5 | 88.6 |
| often chosen | clockwork_nip | 243 | 100.0 | 81.5 | 81.5 |
| often chosen | shadow_slash | 517 | 93.8 | 80.1 | 85.4 |
| often chosen | ring_spin | 1583 | 98.9 | 79.8 | 80.8 |
| often chosen | spinning_hammer | 1572 | 96.9 | 79.2 | 81.7 |
| often chosen | roc_shriek | 590 | 97.6 | 79.2 | 81.1 |
| often chosen | mirror_ray | 5736 | 97.6 | 79.1 | 81.0 |
| often chosen | wax_drip | 4290 | 94.8 | 77.9 | 82.2 |
| often chosen | venom_fang | 1648 | 95.9 | 77.8 | 81.1 |

The full list is in `skills.csv`.

## Win rate by starting row (all creatures)

| Row | Appearances | Win rate % |
|---|---:|---:|
| Front | 17632 | 45.6 |
| Middle | 7711 | 45.5 |
| Rear | 15409 | 52.2 |

## Teammate pairs (at least 25 shared battles)

| | Pair | Battles | Win rate % |
|---|---|---:|---:|
| best | human-rogue-shadow + orrery-sorcerer-orbit | 34 | 100.0 |
| best | brazier-summoner-coal + yak-bard-gong | 27 | 96.3 |
| best | inkwell-trickster-quill + yak-bard-gong | 40 | 92.5 |
| best | brazier-summoner-coal + inkwell-trickster-quill | 51 | 92.2 |
| best | human-priest-moon + human-rogue-shadow | 33 | 87.9 |
| best | crocodile-shaman + maskrack-summoner-echo | 25 | 84.0 |
| best | human-druid + retort-alchemist | 25 | 84.0 |
| best | inkwell-trickster-quill + serpent-titan-crown | 34 | 82.4 |
| best | baton-marshal + yak-bard-gong | 27 | 81.5 |
| best | human-priest-moon + reliquary-summoner-seal | 25 | 80.0 |
| worst | human-priest-moon + lizardfolk-ninja-kunai | 30 | 6.7 |
| worst | fossa-hunter-harpoon + human-spellblade | 28 | 7.1 |
| worst | human-guard-frost + hyena-engineer-saw | 26 | 7.7 |
| worst | human-captain-banner + mirror-revenant | 29 | 10.3 |
| worst | cicada-bard-chime + human-mage-crystal | 27 | 11.1 |
| worst | fossa-hunter-harpoon + human-shaman | 25 | 12.0 |
| worst | human-mage-crystal + human-priest-moon | 25 | 12.0 |
| worst | human-priest-moon + mirror-revenant | 73 | 12.3 |
| worst | gargoyle-sentinel-stone + human-spellblade-2 | 26 | 15.4 |
| worst | gargoyle-sentinel-stone + scorpion-healer-ribbon | 28 | 17.9 |

## Crashed battles

- 1x `InvalidOperationException: Unit 3 is KO and cannot take damage.` (first seed 794)

