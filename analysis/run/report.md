# Run 20261117-175706-harness

AI level **harness**, seeds 1 to 10000, rules commit `8a33ca860a60ad27b2a30f1f87028ac0676d8d43`.

- Battles recorded: 4085 (4085 played to an end, 0 crashed)
- Side A wins 2012, side B wins 1923, draws 150 (3.7%), of which 150 hit the length cap
- Battle length in unit turns: median 174, mean 200.5, shortest 46, longest 510
- Average real time per battle: 16.6 s

## Momentum per side

The meter is sampled after every unit turn; lead is the side's own view (positive = ahead).

| Side | Samples | Ahead % | Level % | Behind % | Average lead | Payments per battle | Momentum paid per battle | Turns it could pay | Of those, did not pay % |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 531837 | 39.8 | 20.2 | 40.0 | -0.0 | 16.2 | 25.7 | 110021 | 39.9 |
| B | 531837 | 40.0 | 20.2 | 39.8 | 0.0 | 16.1 | 25.3 | 111390 | 40.8 |

Most paid-for skills:

| Skill | Payments | Momentum paid |
|---|---:|---:|
| lift_a_skill | 14396 | 14396 |
| clay_golem | 13705 | 26905 |
| shield_bash | 13256 | 13256 |
| lullaby | 12757 | 25514 |
| rampage | 7348 | 7348 |
| hunters_shot | 6484 | 12968 |
| dive | 6139 | 12278 |
| song_of_haste | 5235 | 5235 |
| shield_dome | 5116 | 15348 |
| bolt_turret | 4848 | 4848 |
| rotation | 4586 | 8978 |
| lightning_bolt | 4416 | 8832 |
| piercing_enchant | 3933 | 3933 |
| mend | 3642 | 3642 |
| life_swap | 3021 | 9063 |

## Imbalance flags: top and bottom 10% by win rate (31 each, creatures with at least 20 appearances)

| | Creature | Appearances | Win rate % | Avg damage | Avg healing |
|---|---|---:|---:|---:|---:|
| top | incense-censor | 147 | 85.0 | 132.3 | 0.0 |
| top | human-rogue-shadow | 148 | 83.8 | 116.5 | 0.0 |
| top | brazier-summoner-coal | 152 | 81.6 | 118.8 | 0.0 |
| top | crane-summoner-feather | 139 | 81.3 | 126.9 | 0.0 |
| top | book-summoner-fold | 139 | 77.0 | 57.0 | 0.0 |
| top | badger-summoner-drum | 137 | 75.2 | 88.8 | 0.0 |
| top | reliquary-summoner-seal | 140 | 75.0 | 120.5 | 0.0 |
| top | beacon-summoner | 123 | 74.8 | 99.4 | 0.0 |
| top | maskrack-summoner-echo | 140 | 73.6 | 93.2 | 0.0 |
| top | firefly-summoner-lantern | 172 | 72.1 | 76.0 | 0.0 |
| top | tapestry-summoner | 150 | 72.0 | 99.1 | 0.0 |
| top | pangolin-sorcerer-thunder | 156 | 71.8 | 92.4 | 0.0 |
| top | sealpress-summoner | 140 | 70.0 | 89.8 | 0.0 |
| top | kraken-colossus-basalt | 116 | 69.0 | 100.1 | 0.0 |
| top | weathervane-summoner-arrow | 128 | 68.8 | 115.5 | 0.0 |
| top | capybara-guard-calm | 164 | 68.3 | 91.6 | 12.9 |
| top | plough-ravager | 110 | 68.2 | 81.9 | 0.0 |
| top | tortoise-sentinel-obsidian | 122 | 68.0 | 86.5 | 0.0 |
| top | crab-smith | 170 | 67.6 | 53.8 | 0.2 |
| top | teapot-alchemist-volcano | 145 | 66.9 | 52.2 | 17.4 |
| top | panpipe-summoner-reed | 150 | 64.7 | 59.4 | 0.0 |
| top | salamander-summoner-flask | 174 | 64.4 | 72.4 | 11.6 |
| top | toad-bard-bell | 134 | 64.2 | 90.2 | 0.0 |
| top | musicbox-summoner-drum | 181 | 64.1 | 68.1 | 0.0 |
| top | mannequin | 143 | 63.6 | 53.2 | 0.0 |
| top | sarcophagus-warden | 107 | 63.6 | 59.5 | 0.0 |
| top | thornfang | 150 | 63.3 | 45.6 | 0.0 |
| top | yak-bard-gong | 152 | 63.2 | 68.7 | 0.0 |
| top | mandrake-primalist-root | 169 | 62.7 | 64.4 | 20.7 |
| top | chinchilla-summoner-ocarina | 144 | 62.5 | 47.9 | 0.0 |
| top | beetle-2 | 165 | 61.8 | 46.5 | 0.0 |
| bottom | roadrunner-hunter-whistle | 163 | 30.7 | 23.3 | 0.0 |
| bottom | pocketwatch-warden | 136 | 32.4 | 5.4 | 0.0 |
| bottom | cassowary-captain-horn | 143 | 32.9 | 5.0 | 0.0 |
| bottom | sableantelope-captain-pennant | 164 | 32.9 | 15.4 | 0.0 |
| bottom | goblet-paladin | 158 | 33.5 | 4.1 | 0.0 |
| bottom | seal-bard-bottle | 155 | 33.5 | 5.2 | 11.5 |
| bottom | sistrum-summoner | 142 | 33.8 | 11.1 | 0.0 |
| bottom | human-priest-moon | 141 | 34.0 | 9.0 | 0.0 |
| bottom | human-knight-hammer | 146 | 34.2 | 24.0 | 1.0 |
| bottom | dragon-sovereign-tempest | 122 | 34.4 | 7.0 | 0.0 |
| bottom | sugarglider-scout-sail | 159 | 34.6 | 6.9 | 0.0 |
| bottom | cobra | 156 | 34.6 | 14.1 | 1.6 |
| bottom | human-spellblade | 149 | 34.9 | 10.1 | 0.0 |
| bottom | organ-summoner | 110 | 35.5 | 12.5 | 0.0 |
| bottom | puffin-bodyguard-shield | 172 | 35.5 | 13.4 | 0.0 |
| bottom | hawk-scout-bow | 156 | 35.9 | 24.9 | 0.0 |
| bottom | human-priest-bell | 139 | 36.0 | 2.8 | 0.0 |
| bottom | fossa-hunter-harpoon | 150 | 36.0 | 33.5 | 1.3 |
| bottom | vise-enforcer | 170 | 36.5 | 32.4 | 1.6 |
| bottom | meerkat-captain-spyglass | 178 | 36.5 | 21.5 | 0.0 |
| bottom | bugle-skyrider | 197 | 36.5 | 5.7 | 0.0 |
| bottom | hourglass-hexer-sand | 152 | 36.8 | 16.0 | 0.0 |
| bottom | harpy-eagle-scout-claws | 160 | 36.9 | 27.2 | 0.0 |
| bottom | cicada-bard-chime | 165 | 37.0 | 11.0 | 30.0 |
| bottom | fox-bard | 226 | 37.2 | 5.3 | 0.0 |
| bottom | compass-hunter-needle | 164 | 37.2 | 38.0 | 7.6 |
| bottom | boar-guard-hex | 145 | 37.2 | 34.2 | 0.0 |
| bottom | moth-cleric-lantern | 161 | 37.3 | 24.6 | 0.0 |
| bottom | stag-hunter-shield | 161 | 37.3 | 33.2 | 0.0 |
| bottom | isopod-sentinel-iron | 181 | 37.6 | 12.7 | 0.0 |
| bottom | peacock-bard-feather | 154 | 37.7 | 3.8 | 0.0 |

## Classes and keystones

| Kind | Name | Appearances | Win rate % |
|---|---|---:|---:|
| class | pyromancer | 571 | 65.0 |
| class | summoner | 6842 | 64.3 |
| class | cryomancer | 1088 | 55.0 |
| class | berserker | 5194 | 53.2 |
| class | chemist | 3686 | 49.6 |
| class | spiritwalker | 1679 | 49.0 |
| class | enchanter | 3457 | 48.9 |
| class | engineer | 6147 | 48.4 |
| class | stormcaller | 2113 | 48.4 |
| class | tactician | 791 | 48.2 |
| class | lifebinder | 1073 | 48.0 |
| class | transmuter | 1045 | 47.8 |
| class | bulwark | 9393 | 47.5 |
| class | bard | 5174 | 46.9 |
| class | totemist | 1118 | 46.8 |
| class | ritualist | 1719 | 46.0 |
| class | cleric | 1981 | 46.0 |
| class | saboteur | 2167 | 46.0 |
| class | mender | 2140 | 45.7 |
| class | necromancer | 1057 | 45.6 |
| class | assassin | 3190 | 45.4 |
| class | thief | 6603 | 45.4 |
| class | champion | 6396 | 45.2 |
| class | skyborne | 7741 | 45.0 |
| class | keeper | 2964 | 44.9 |
| class | tracker | 5811 | 44.8 |
| class | commander | 5705 | 44.0 |
| class | hexer | 1195 | 42.8 |
| keystone | wildfire | 297 | 74.4 |
| keystone | quickening | 2173 | 72.2 |
| keystone | golem_heart | 2329 | 62.1 |
| keystone | initiative | 163 | 60.1 |
| keystone | portal | 2340 | 59.1 |
| keystone | last_stand | 1761 | 58.1 |
| keystone | inferno | 152 | 57.9 |
| keystone | cold_heart | 500 | 55.4 |
| keystone | deep_chill | 298 | 54.7 |
| keystone | brittle | 290 | 54.5 |
| keystone | martyr | 425 | 53.9 |
| keystone | static_field | 798 | 52.8 |
| keystone | spirit_shield | 320 | 52.5 |
| keystone | reckless_swing | 1725 | 52.1 |
| keystone | haunting | 617 | 52.0 |
| keystone | big_finish | 1398 | 51.7 |
| keystone | ashwalker | 122 | 50.8 |
| keystone | twin_enchant | 1177 | 50.3 |
| keystone | rapid_fire | 2052 | 50.2 |
| keystone | purifying_aura | 590 | 50.0 |
| keystone | chaos_agent | 647 | 49.9 |
| keystone | lasting_change | 477 | 49.9 |
| keystone | reinforced | 2094 | 49.6 |
| keystone | spell_ward | 1130 | 49.4 |
| keystone | rhythm | 1604 | 49.3 |
| keystone | frostbinder | 485 | 49.3 |
| keystone | master_thief | 2276 | 49.2 |
| keystone | hold_position | 3197 | 49.1 |
| keystone | unstoppable | 1708 | 49.1 |
| keystone | all_in | 312 | 49.0 |
| keystone | thorned_ward | 968 | 49.0 |
| keystone | puppeteer | 442 | 48.9 |
| keystone | unbreakable | 3070 | 48.6 |
| keystone | deaths_bounty | 303 | 48.5 |
| keystone | rally | 2050 | 48.4 |
| keystone | volatile | 1117 | 48.3 |
| keystone | potent_brew | 1171 | 48.3 |
| keystone | sturdy_totems | 305 | 47.9 |
| keystone | triage | 604 | 47.7 |
| keystone | vanishing_kill | 927 | 47.5 |
| keystone | lich_form | 424 | 47.4 |
| keystone | lingering_magic | 1150 | 47.0 |
| keystone | overcharge | 711 | 46.7 |
| keystone | crescendo | 1672 | 46.6 |
| keystone | sky_lord | 2553 | 46.5 |
| keystone | soul_anchor | 620 | 46.1 |
| keystone | sniff_the_air | 2057 | 46.1 |
| keystone | scent_of_blood | 1819 | 46.0 |
| keystone | mirror_form | 309 | 46.0 |
| keystone | brittle_glass | 259 | 45.9 |
| keystone | overflowing_care | 783 | 45.8 |
| keystone | glory | 2030 | 45.8 |
| keystone | hunt_down | 2291 | 45.7 |
| keystone | grand_chorus | 1898 | 45.2 |
| keystone | self_repair | 2001 | 45.2 |
| keystone | deluge | 676 | 45.1 |
| keystone | venom_blades | 1055 | 45.0 |
| keystone | sanctity | 796 | 45.0 |
| keystone | shield_wall | 3126 | 44.8 |
| keystone | phoenix | 342 | 44.7 |
| keystone | escape_artist | 2181 | 44.6 |
| keystone | wrecker | 807 | 44.6 |
| keystone | eagle_eye | 2646 | 44.6 |
| keystone | eye_of_the_storm | 604 | 44.5 |
| keystone | witchs_patience | 445 | 44.5 |
| keystone | tidecaller | 558 | 44.3 |
| keystone | unseen | 1208 | 44.2 |
| keystone | duelist | 2075 | 44.1 |
| keystone | regrowth | 753 | 44.0 |
| keystone | lift | 2542 | 43.9 |
| keystone | disarm_trap | 713 | 43.9 |
| keystone | lifeblood | 306 | 43.5 |
| keystone | steadfast_ward | 952 | 43.4 |
| keystone | judgment | 595 | 43.4 |
| keystone | spreading_curse | 370 | 43.2 |
| keystone | twin_wards | 1044 | 42.5 |
| keystone | relentless | 1935 | 42.5 |
| keystone | great_totem | 493 | 42.4 |
| keystone | shadow_purse | 2146 | 42.3 |
| keystone | drill | 1891 | 41.9 |
| keystone | vanguard | 1764 | 41.3 |
| keystone | inspiring_presence | 316 | 41.1 |
| keystone | bone_armor | 330 | 40.6 |
| keystone | lingering_malice | 380 | 40.5 |

## Skills: best and worst win rate of the units that have them (at least 50 units), and usage

| | Skill | Units | Win rate % | Uses per unit |
|---|---|---:|---:|---:|
| best | incense_cloud | 147 | 85.0 | 0.1 |
| best | void_shard | 148 | 83.8 | 1.4 |
| best | ember_toss | 152 | 81.6 | 0.5 |
| best | storm_plume | 139 | 81.3 | 0.2 |
| best | paper_cut | 139 | 77.0 | 0.9 |
| best | drumroll | 137 | 75.2 | 0.0 |
| best | relic_glow | 140 | 75.0 | 0.0 |
| best | lamp_beam | 123 | 74.8 | 0.4 |
| best | echo_scream | 140 | 73.6 | 0.1 |
| best | lantern_flash | 172 | 72.1 | 0.4 |
| best | woven_snare | 150 | 72.0 | 0.6 |
| best | thunder_hum | 156 | 71.8 | 0.2 |
| best | wax_press | 140 | 70.0 | 0.3 |
| best | tentacle_grab | 116 | 69.0 | 0.2 |
| best | turning_gust | 128 | 68.8 | 1.3 |
| worst | shrill_whistle | 163 | 30.7 | 0.5 |
| worst | tick_tock | 136 | 32.4 | 1.0 |
| worst | horn_call | 143 | 32.9 | 0.3 |
| worst | rallying_charge | 164 | 32.9 | 4.0 |
| worst | holy_pour | 158 | 33.5 | 0.6 |
| worst | splash_song | 155 | 33.5 | 0.5 |
| worst | rattling_chant | 142 | 33.8 | 3.0 |
| worst | eclipse | 141 | 34.0 | 0.6 |
| worst | hammer_smash | 146 | 34.2 | 3.4 |
| worst | tempest_breath | 122 | 34.4 | 1.1 |
| worst | gliding_swoop | 159 | 34.6 | 4.4 |
| worst | venom_spray | 156 | 34.6 | 0.4 |
| worst | glyph_strike | 149 | 34.9 | 2.8 |
| worst | pipe_chord | 110 | 35.5 | 0.6 |
| worst | shield_thump | 172 | 35.5 | 3.7 |

Active skills never used in any battle (1): totem_of_return

## What the AI does with each skill (turns where it was legal, searched further, chosen)

Skills with at least 200 legal turns. Searched = the search looked at one of its uses further; chosen = it was played.

| | Skill | Legal turns | Searched % of legal | Chosen % of legal | Chosen % of searched |
|---|---|---:|---:|---:|---:|
| rarely chosen | totem_of_return | 4331 | 0.3 | 0.0 | 0.0 |
| rarely chosen | echo_enchant | 19311 | 0.2 | 0.0 | 6.2 |
| rarely chosen | cut_the_strings | 3494 | 0.1 | 0.0 | 25.0 |
| rarely chosen | rune_golem | 37826 | 29.2 | 0.0 | 0.1 |
| rarely chosen | row_of_fire | 2323 | 1.8 | 0.1 | 4.9 |
| rarely chosen | spirit_link_totem | 4331 | 47.6 | 0.3 | 0.7 |
| rarely chosen | rescue | 40220 | 6.7 | 0.3 | 4.9 |
| rarely chosen | read_tracks | 67991 | 15.4 | 0.3 | 2.2 |
| rarely chosen | press_the_advantage | 4717 | 7.5 | 0.4 | 5.1 |
| rarely chosen | tempest | 8240 | 4.3 | 0.7 | 16.8 |
| rarely chosen | rite_of_mists | 8315 | 5.6 | 0.8 | 14.0 |
| rarely chosen | melt_away | 68603 | 13.1 | 0.9 | 6.6 |
| rarely chosen | burnout_heal | 4395 | 6.1 | 1.1 | 17.4 |
| rarely chosen | sweeping_kick | 14489 | 8.6 | 1.3 | 14.9 |
| rarely chosen | frost_spill | 2213 | 50.7 | 1.5 | 2.9 |
| often chosen | orbit_missile | 2074 | 99.7 | 87.7 | 88.0 |
| often chosen | shadow_slash | 689 | 97.5 | 84.0 | 86.2 |
| often chosen | soul_rend | 686 | 93.6 | 84.0 | 89.7 |
| often chosen | ring_spin | 2033 | 99.4 | 83.8 | 84.4 |
| often chosen | shadow_carve | 956 | 97.1 | 83.6 | 86.1 |
| often chosen | shadow_touch | 2614 | 97.1 | 83.4 | 85.9 |
| often chosen | venom_fang | 1040 | 96.6 | 83.4 | 86.3 |
| often chosen | needle_point | 1446 | 98.0 | 82.0 | 83.7 |
| often chosen | mirror_ray | 1957 | 98.6 | 81.3 | 82.5 |
| often chosen | raking_claws | 917 | 95.9 | 81.1 | 84.6 |
| often chosen | hat_trick | 1943 | 98.1 | 80.8 | 82.3 |
| often chosen | grapple_yank | 1978 | 95.8 | 80.3 | 83.8 |
| often chosen | ranger_arrow | 1550 | 98.1 | 78.6 | 80.1 |
| often chosen | arcane_edge | 872 | 92.3 | 77.6 | 84.1 |
| often chosen | smoke_kunai | 2052 | 96.7 | 76.8 | 79.4 |

The full list is in `skills.csv`.

## Win rate by starting row (all creatures)

| Row | Appearances | Win rate % |
|---|---:|---:|
| Front | 27571 | 47.0 |
| Middle | 3888 | 49.0 |
| Rear | 17561 | 49.7 |

## Teammate pairs (at least 25 shared battles)

| | Pair | Battles | Win rate % |
|---|---|---:|---:|

