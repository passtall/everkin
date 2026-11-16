# Run 20261116-210306-harness

AI level **harness**, seeds 1 to 1000, rules commit `6315222681ed8649c5533469435eebadb461a3f6`.

- Battles recorded: 1000 (1000 played to an end, 0 crashed)
- Side A wins 480, side B wins 487, draws 33 (3.3%), of which 33 hit the length cap
- Battle length in unit turns: median 167, mean 193.2, shortest 51, longest 503
- Average real time per battle: 14.3 s

## Momentum per side

The meter is sampled after every unit turn; lead is the side's own view (positive = ahead).

| Side | Samples | Ahead % | Level % | Behind % | Average lead | Payments per battle | Momentum paid per battle | Turns it could pay | Of those, did not pay % |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 155519 | 37.7 | 22.8 | 39.5 | -0.0 | 12.2 | 31.7 | 29332 | 58.4 |
| B | 155519 | 39.5 | 22.8 | 37.7 | 0.0 | 12.5 | 31.8 | 29875 | 58.2 |

Most paid-for skills:

| Skill | Payments | Momentum paid |
|---|---:|---:|
| clay_golem | 2931 | 11582 |
| hunters_shot | 2613 | 5226 |
| animate_blades | 2108 | 4209 |
| lift_a_skill | 1985 | 5955 |
| dive | 1935 | 3870 |
| rampage | 1574 | 3148 |
| lightning_bolt | 1267 | 2534 |
| net_trap | 1256 | 1256 |
| rotation | 1140 | 2223 |
| life_swap | 902 | 4510 |
| shield_dome | 880 | 4400 |
| bolt_turret | 763 | 1526 |
| rain_of_mending | 581 | 1162 |
| firebomb | 485 | 970 |
| ice_lance | 481 | 962 |

## Imbalance flags: top and bottom 10% by win rate (31 each, creatures with at least 20 appearances)

| | Creature | Appearances | Win rate % | Avg damage | Avg healing |
|---|---|---:|---:|---:|---:|
| top | snail-bard-brewer | 38 | 81.6 | 2.8 | 0.0 |
| top | caterpillar-bard | 46 | 80.4 | 0.6 | 0.0 |
| top | peacock-bard-feather | 31 | 77.4 | 0.6 | 0.0 |
| top | frog-bard-dagger | 40 | 75.0 | 1.3 | 0.0 |
| top | cricket-bard-fiddle | 36 | 75.0 | 2.6 | 0.0 |
| top | muskox-bodyguard-buckler | 35 | 74.3 | 84.5 | 0.0 |
| top | badger-summoner-drum | 35 | 74.3 | 44.1 | 0.0 |
| top | conchphone-bard-horn | 37 | 73.0 | 0.6 | 0.0 |
| top | seal-bard-bottle | 36 | 72.2 | 1.6 | 0.0 |
| top | pangolin-sorcerer-thunder | 46 | 71.7 | 91.4 | 0.0 |
| top | frigatebird-bard-throat | 45 | 71.1 | 0.1 | 0.0 |
| top | walrus-bard-horn | 27 | 70.4 | 0.6 | 0.0 |
| top | ibex-captain-standard | 30 | 70.0 | 2.7 | 0.0 |
| top | binturong-bard-vine | 33 | 69.7 | 2.2 | 9.3 |
| top | beetle-2 | 39 | 69.2 | 64.1 | 0.0 |
| top | teapot-alchemist-volcano | 42 | 69.0 | 52.2 | 29.6 |
| top | golem-colossus-furnace | 35 | 68.6 | 32.9 | 0.0 |
| top | hornbill-bard-drum | 47 | 68.1 | 2.3 | 0.0 |
| top | mannequin | 25 | 68.0 | 84.8 | 0.0 |
| top | gong-hexer-dissonance | 34 | 67.6 | 0.5 | 2.7 |
| top | human-bard | 43 | 67.4 | 3.7 | 0.0 |
| top | book-summoner-fold | 30 | 66.7 | 42.0 | 0.0 |
| top | human-bard-shield | 30 | 66.7 | 29.0 | 0.0 |
| top | sledgehammer-colossus | 30 | 66.7 | 74.4 | 0.0 |
| top | lyrebird-bard-plume | 50 | 66.0 | 0.4 | 0.0 |
| top | cassowary-captain-horn | 41 | 65.9 | 5.3 | 0.0 |
| top | pika-summoner-horn | 41 | 65.9 | 19.9 | 0.0 |
| top | ram-bard-bellows | 32 | 65.6 | 0.7 | 0.0 |
| top | ram-paladin-sun | 26 | 65.4 | 1.0 | 0.0 |
| top | bassoon-hexer-requiem | 34 | 64.7 | 2.9 | 0.0 |
| top | dragon-titan-glacier | 34 | 64.7 | 54.0 | 0.0 |
| bottom | sundrum-hexer | 30 | 6.7 | 3.0 | 1.5 |
| bottom | shrew-summoner-totem | 35 | 22.9 | 16.8 | 7.3 |
| bottom | sistrum-summoner | 34 | 23.5 | 13.0 | 0.0 |
| bottom | phoenix-titan-solar | 28 | 25.0 | 13.0 | 98.4 |
| bottom | quokka-summoner-pebble | 45 | 26.7 | 35.8 | 0.0 |
| bottom | owl-wizard | 33 | 27.3 | 22.2 | 0.0 |
| bottom | caracal-hunter-spear | 28 | 28.6 | 40.3 | 4.2 |
| bottom | bat-rogue-shadow | 42 | 28.6 | 15.7 | 0.0 |
| bottom | human-duelist-rapier | 45 | 28.9 | 32.0 | 2.2 |
| bottom | bat-summoner-moon | 38 | 28.9 | 19.7 | 0.0 |
| bottom | stag-glade | 27 | 29.6 | 21.3 | 52.7 |
| bottom | cicada-bard-chime | 47 | 29.8 | 5.0 | 97.8 |
| bottom | gargoyle-sentinel-stone | 30 | 30.0 | 30.1 | 0.0 |
| bottom | boar-magma | 30 | 30.0 | 15.5 | 0.0 |
| bottom | behemoth-titan-iron | 30 | 30.0 | 43.4 | 0.0 |
| bottom | beetle-knight-shield | 33 | 30.3 | 28.8 | 0.0 |
| bottom | sugarglider-scout-sail | 26 | 30.8 | 6.1 | 0.0 |
| bottom | dugong-guard-crystal | 42 | 31.0 | 25.0 | 0.0 |
| bottom | dragon-sovereign-tempest | 29 | 31.0 | 7.8 | 0.0 |
| bottom | harpy-eagle-scout-claws | 45 | 31.1 | 36.6 | 0.0 |
| bottom | human-mage-stone | 38 | 31.6 | 29.3 | 0.0 |
| bottom | human-wizard | 41 | 31.7 | 26.5 | 0.2 |
| bottom | goat-shaman-totem | 44 | 31.8 | 14.4 | 44.0 |
| bottom | mole-engineer-hexhammer | 56 | 32.1 | 14.6 | 0.0 |
| bottom | panpipe-summoner-reed | 28 | 32.1 | 32.8 | 0.0 |
| bottom | human-captain-rapier | 43 | 32.6 | 23.7 | 0.9 |
| bottom | tarsier-hexer-mask | 60 | 33.3 | 25.6 | 1.9 |
| bottom | ballast-bodyguard | 21 | 33.3 | 56.1 | 0.0 |
| bottom | human-spellblade | 33 | 33.3 | 9.1 | 0.0 |
| bottom | human-archer-frost | 51 | 33.3 | 39.5 | 0.0 |
| bottom | human-guard-frost | 36 | 33.3 | 6.7 | 0.0 |

## Classes and keystones

| Kind | Name | Appearances | Win rate % |
|---|---|---:|---:|
| class | bard | 1228 | 66.5 |
| class | pyromancer | 143 | 55.2 |
| class | berserker | 1285 | 54.8 |
| class | bulwark | 2345 | 51.8 |
| class | chemist | 935 | 50.6 |
| class | engineer | 1523 | 50.5 |
| class | saboteur | 513 | 50.1 |
| class | summoner | 1664 | 49.4 |
| class | necromancer | 241 | 49.0 |
| class | commander | 1380 | 48.7 |
| class | cryomancer | 301 | 47.2 |
| class | spiritwalker | 439 | 46.5 |
| class | tracker | 1452 | 46.0 |
| class | assassin | 776 | 46.0 |
| class | keeper | 718 | 45.8 |
| class | transmuter | 256 | 45.7 |
| class | cleric | 497 | 45.5 |
| class | tactician | 192 | 45.3 |
| class | skyborne | 1840 | 45.2 |
| class | champion | 1607 | 45.0 |
| class | ritualist | 374 | 44.9 |
| class | thief | 1618 | 44.7 |
| class | lifebinder | 247 | 42.9 |
| class | stormcaller | 525 | 42.7 |
| class | mender | 506 | 42.5 |
| class | totemist | 275 | 41.1 |
| class | enchanter | 801 | 40.1 |
| class | hexer | 319 | 38.9 |
| keystone | rhythm | 396 | 70.7 |
| keystone | wildfire | 83 | 65.1 |
| keystone | grand_chorus | 442 | 64.9 |
| keystone | crescendo | 390 | 64.1 |
| keystone | initiative | 42 | 61.9 |
| keystone | last_stand | 439 | 60.8 |
| keystone | chaos_agent | 171 | 57.3 |
| keystone | inferno | 32 | 56.2 |
| keystone | deaths_bounty | 66 | 56.1 |
| keystone | quickening | 560 | 54.6 |
| keystone | unbreakable | 761 | 54.3 |
| keystone | rapid_fire | 492 | 54.3 |
| keystone | hold_position | 777 | 53.0 |
| keystone | reckless_swing | 431 | 52.4 |
| keystone | big_finish | 358 | 52.2 |
| keystone | triage | 152 | 52.0 |
| keystone | potent_brew | 271 | 51.3 |
| keystone | vanguard | 417 | 51.1 |
| keystone | unstoppable | 415 | 50.8 |
| keystone | thorned_ward | 234 | 50.4 |
| keystone | lasting_change | 127 | 50.4 |
| keystone | tidecaller | 120 | 50.0 |
| keystone | hunt_down | 576 | 49.7 |
| keystone | martyr | 101 | 49.5 |
| keystone | reinforced | 524 | 49.2 |
| keystone | rally | 497 | 49.1 |
| keystone | deep_chill | 86 | 48.8 |
| keystone | disarm_trap | 156 | 48.7 |
| keystone | haunting | 151 | 48.3 |
| keystone | shield_wall | 807 | 48.3 |
| keystone | self_repair | 507 | 48.1 |
| keystone | scent_of_blood | 460 | 48.0 |
| keystone | volatile | 306 | 48.0 |
| keystone | lift | 627 | 47.8 |
| keystone | unseen | 283 | 47.7 |
| keystone | golem_heart | 566 | 47.7 |
| keystone | sniff_the_air | 526 | 47.0 |
| keystone | lich_form | 98 | 46.9 |
| keystone | sturdy_totems | 92 | 46.7 |
| keystone | judgment | 161 | 46.6 |
| keystone | cold_heart | 131 | 46.6 |
| keystone | eye_of_the_storm | 142 | 46.5 |
| keystone | puppeteer | 112 | 46.4 |
| keystone | brittle | 84 | 46.4 |
| keystone | drill | 466 | 46.1 |
| keystone | sky_lord | 584 | 46.1 |
| keystone | deluge | 148 | 45.9 |
| keystone | portal | 538 | 45.7 |
| keystone | vanishing_kill | 244 | 45.5 |
| keystone | bone_armor | 77 | 45.5 |
| keystone | sanctity | 194 | 45.4 |
| keystone | master_thief | 550 | 45.3 |
| keystone | soul_anchor | 176 | 44.9 |
| keystone | wrecker | 186 | 44.6 |
| keystone | venom_blades | 249 | 44.6 |
| keystone | shadow_purse | 521 | 44.5 |
| keystone | purifying_aura | 142 | 44.4 |
| keystone | twin_wards | 255 | 44.3 |
| keystone | escape_artist | 547 | 44.2 |
| keystone | glory | 544 | 43.4 |
| keystone | lifeblood | 60 | 43.3 |
| keystone | static_field | 208 | 43.3 |
| keystone | relentless | 466 | 42.9 |
| keystone | lingering_magic | 268 | 42.9 |
| keystone | steadfast_ward | 229 | 42.8 |
| keystone | brittle_glass | 52 | 42.3 |
| keystone | all_in | 71 | 42.3 |
| keystone | eagle_eye | 629 | 41.7 |
| keystone | duelist | 487 | 41.3 |
| keystone | twin_enchant | 272 | 41.2 |
| keystone | witchs_patience | 121 | 40.5 |
| keystone | mirror_form | 77 | 40.3 |
| keystone | overflowing_care | 172 | 40.1 |
| keystone | spirit_shield | 70 | 40.0 |
| keystone | inspiring_presence | 79 | 39.2 |
| keystone | overcharge | 175 | 38.9 |
| keystone | lingering_malice | 100 | 38.0 |
| keystone | spreading_curse | 98 | 37.8 |
| keystone | frostbinder | 106 | 37.7 |
| keystone | great_totem | 113 | 37.2 |
| keystone | regrowth | 182 | 36.8 |
| keystone | spell_ward | 261 | 36.0 |
| keystone | phoenix | 86 | 34.9 |
| keystone | ashwalker | 28 | 25.0 |

## Skills: best and worst win rate of the units that have them (at least 50 units), and usage

| | Skill | Units | Win rate % | Uses per unit |
|---|---|---:|---:|---:|
| best | song_of_haste | 1228 | 66.5 | 1.7 |
| best | war_drums | 1228 | 66.5 | 1.4 |
| best | lullaby | 1228 | 66.5 | 10.5 |
| best | plume_fan | 50 | 66.0 | 0.1 |
| best | venom_bite | 52 | 63.5 | 3.1 |
| best | rune_spray | 67 | 62.7 | 0.4 |
| best | void_shard | 50 | 60.0 | 7.2 |
| best | balm_flask | 56 | 57.1 | 9.4 |
| best | axe_hook | 51 | 56.9 | 2.5 |
| best | windup_punch | 54 | 55.6 | 0.4 |
| best | fireball | 143 | 55.2 | 2.1 |
| best | kindle | 143 | 55.2 | 1.0 |
| best | row_of_fire | 143 | 55.2 | 0.0 |
| best | rampage | 1285 | 54.8 | 1.2 |
| best | sweeping_kick | 1285 | 54.8 | 0.0 |
| worst | hexhammer | 56 | 32.1 | 4.0 |
| worst | skull_hex | 60 | 33.3 | 12.7 |
| worst | frost_arrow | 51 | 33.3 | 7.0 |
| worst | stone_fist | 52 | 34.6 | 3.2 |
| worst | idol_glow | 54 | 35.2 | 9.5 |
| worst | sickle_slash | 50 | 36.0 | 0.7 |
| worst | ancestor_call | 53 | 37.7 | 1.3 |
| worst | frost_missile | 52 | 38.5 | 6.4 |
| worst | withering_curse | 319 | 38.9 | 1.0 |
| worst | curse_of_weight | 319 | 38.9 | 1.3 |
| worst | curse_of_betrayal | 319 | 38.9 | 0.2 |
| worst | eclipse | 51 | 39.2 | 0.2 |
| worst | piercing_enchant | 801 | 40.1 | 0.4 |
| worst | phase_weapon | 801 | 40.1 | 3.1 |
| worst | echo_enchant | 801 | 40.1 | 0.3 |

Active skills never used in any battle (5): cut_the_strings, desecrate, spirit_link_totem, totem_of_return, rite_of_mists

## What the AI does with each skill (turns where it was legal, searched further, chosen)

Skills with at least 200 legal turns. Searched = the search looked at one of its uses further; chosen = it was played.

| | Skill | Legal turns | Searched % of legal | Chosen % of legal | Chosen % of searched |
|---|---|---:|---:|---:|---:|
| rarely chosen | rite_of_mists | 1935 | 0.0 | 0.0 |  |
| rarely chosen | spirit_link_totem | 1317 | 1.6 | 0.0 | 0.0 |
| rarely chosen | totem_of_return | 1317 | 0.1 | 0.0 | 0.0 |
| rarely chosen | desecrate | 680 | 0.9 | 0.0 | 0.0 |
| rarely chosen | rune_golem | 7676 | 32.9 | 0.0 | 0.1 |
| rarely chosen | read_tracks | 19896 | 23.6 | 0.1 | 0.4 |
| rarely chosen | tempest | 2289 | 2.8 | 0.1 | 4.6 |
| rarely chosen | row_of_fire | 680 | 3.2 | 0.1 | 4.5 |
| rarely chosen | burnout_heal | 1251 | 2.1 | 0.2 | 7.7 |
| rarely chosen | press_the_advantage | 1201 | 8.7 | 0.2 | 1.9 |
| rarely chosen | chain_lightning | 2289 | 11.1 | 0.6 | 5.1 |
| rarely chosen | swap_turns | 1201 | 6.5 | 0.7 | 10.3 |
| rarely chosen | melt_away | 7835 | 3.8 | 0.7 | 18.3 |
| rarely chosen | life_for_life | 781 | 11.3 | 0.8 | 6.8 |
| rarely chosen | banner_snap | 275 | 7.3 | 1.1 | 15.0 |
| often chosen | shadow_touch | 741 | 98.9 | 91.2 | 92.2 |
| often chosen | mirror_ray | 531 | 99.2 | 88.1 | 88.8 |
| often chosen | antler_toss | 201 | 98.5 | 86.6 | 87.9 |
| often chosen | hat_trick | 497 | 98.0 | 86.3 | 88.1 |
| often chosen | orbit_missile | 461 | 97.0 | 86.1 | 88.8 |
| often chosen | spore_ray | 734 | 99.7 | 84.6 | 84.8 |
| often chosen | spear_throw | 310 | 98.1 | 83.9 | 85.5 |
| often chosen | root_grasp | 327 | 99.4 | 83.8 | 84.3 |
| often chosen | skull_hex | 918 | 95.8 | 82.8 | 86.5 |
| often chosen | thorn_volley | 331 | 100.0 | 81.6 | 81.6 |
| often chosen | turbine_blade | 200 | 95.0 | 81.5 | 85.8 |
| often chosen | ranger_arrow | 409 | 99.5 | 80.4 | 80.8 |
| often chosen | owl_spark | 592 | 92.6 | 79.9 | 86.3 |
| often chosen | wax_drip | 425 | 95.3 | 79.1 | 83.0 |
| often chosen | ring_spin | 352 | 97.2 | 78.4 | 80.7 |

The full list is in `skills.csv`.

## Win rate by starting row (all creatures)

| Row | Appearances | Win rate % |
|---|---:|---:|
| Front | 6775 | 47.9 |
| Middle | 965 | 49.7 |
| Rear | 4260 | 48.8 |

## Teammate pairs (at least 25 shared battles)

| | Pair | Battles | Win rate % |
|---|---|---:|---:|

