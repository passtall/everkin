# Run 20261118-173101-harness

AI level **harness**, seeds 1 to 1000, rules commit `418b17a33458221bbe9357125609d9217264dc22`.

- Battles recorded: 1000 (1000 played to an end, 0 crashed)
- Side A wins 507, side B wins 470, draws 23 (2.3%), of which 23 hit the length cap
- Battle length in unit turns: median 151, mean 178.0, shortest 57, longest 505
- Average real time per battle: 14.7 s

## Momentum per side

The meter is sampled after every unit turn; lead is the side's own view (positive = ahead).

| Side | Samples | Ahead % | Level % | Behind % | Average lead | Payments per battle | Momentum paid per battle | Turns it could pay | Of those, did not pay % |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 134329 | 39.4 | 22.5 | 38.1 | 0.1 | 15.6 | 25.2 | 27639 | 43.6 |
| B | 134329 | 38.1 | 22.5 | 39.4 | -0.1 | 14.6 | 23.2 | 25772 | 43.2 |

Most paid-for skills:

| Skill | Payments | Momentum paid |
|---|---:|---:|
| lift_a_skill | 2964 | 2964 |
| shield_bash | 2933 | 2933 |
| rampage | 2219 | 2219 |
| divine_intervention | 2135 | 6405 |
| song_of_haste | 2029 | 2029 |
| hunters_shot | 1951 | 3902 |
| clay_golem | 1898 | 3766 |
| dive | 1214 | 2428 |
| bolt_turret | 1208 | 1208 |
| lullaby | 1199 | 2398 |
| lightning_bolt | 1146 | 2292 |
| mend | 1110 | 1110 |
| piercing_enchant | 859 | 859 |
| net_trap | 752 | 752 |
| life_swap | 738 | 2214 |

## Imbalance flags: top and bottom 10% by win rate (21 each, creatures with at least 20 appearances)

| | Creature | Appearances | Win rate % | Avg damage | Avg healing |
|---|---|---:|---:|---:|---:|
| top | badger-summoner-drum | 33 | 90.9 | 106.8 | 0.0 |
| top | pangolin-sorcerer-thunder | 27 | 88.9 | 174.5 | 0.0 |
| top | beacon-summoner | 42 | 83.3 | 107.0 | 0.0 |
| top | crane-summoner-feather | 31 | 80.6 | 113.5 | 0.0 |
| top | toad-bard-bell | 24 | 79.2 | 92.2 | 0.0 |
| top | brazier-summoner-coal | 38 | 78.9 | 144.5 | 0.0 |
| top | mandrake-primalist-root | 26 | 76.9 | 71.5 | 16.0 |
| top | bell-guardian-clapper | 51 | 76.5 | 74.4 | 0.0 |
| top | printingpress-summoner-type | 35 | 74.3 | 106.1 | 0.0 |
| top | yak-bard-gong | 23 | 73.9 | 102.0 | 0.0 |
| top | siphon-alchemist | 37 | 73.0 | 26.8 | 30.6 |
| top | kraken-colossus-basalt | 29 | 72.4 | 165.2 | 0.0 |
| top | tapestry-summoner | 29 | 72.4 | 131.1 | 0.0 |
| top | conchphone-bard-horn | 32 | 71.9 | 3.0 | 0.0 |
| top | beetle-2 | 31 | 71.0 | 52.8 | 0.0 |
| top | human-alchemist | 60 | 70.0 | 30.6 | 27.1 |
| top | termite-summoner-clockwork | 20 | 70.0 | 126.0 | 0.0 |
| top | maskrack-summoner-echo | 23 | 69.6 | 101.2 | 0.0 |
| top | reliquary-summoner-seal | 23 | 69.6 | 120.0 | 0.0 |
| top | human-brawler-hook | 65 | 69.2 | 64.8 | 0.0 |
| top | otter-hunter-sling | 32 | 68.8 | 21.0 | 1.8 |
| bottom | seal-bard-bottle | 35 | 17.1 | 5.4 | 9.2 |
| bottom | drum-summoner-war | 23 | 17.4 | 10.8 | 0.0 |
| bottom | beetle-knight-shield | 39 | 20.5 | 21.1 | 0.0 |
| bottom | gong-hexer-dissonance | 38 | 23.7 | 3.7 | 4.1 |
| bottom | peacock-bard-feather | 25 | 24.0 | 3.1 | 0.0 |
| bottom | human-necromancer-skull | 78 | 24.4 | 15.4 | 0.0 |
| bottom | human-spellblade | 71 | 25.4 | 8.8 | 0.0 |
| bottom | abacus-strategist | 35 | 25.7 | 3.2 | 0.0 |
| bottom | frigatebird-bard-throat | 31 | 25.8 | 4.2 | 0.0 |
| bottom | sistrum-summoner | 32 | 28.1 | 9.5 | 0.0 |
| bottom | roc-colossus-thunder | 39 | 28.2 | 27.0 | 0.0 |
| bottom | lobster-bard-castanet | 35 | 28.6 | 4.1 | 0.0 |
| bottom | human-guard-frost | 42 | 28.6 | 10.2 | 0.0 |
| bottom | turbine-skyguard | 90 | 28.9 | 17.3 | 0.6 |
| bottom | kiln-behemoth | 31 | 29.0 | 19.9 | 0.0 |
| bottom | human-spellblade-2 | 41 | 29.3 | 15.1 | 0.0 |
| bottom | chest-bodyguard-lock | 48 | 31.2 | 34.0 | 0.0 |
| bottom | buffalo-guard-root | 25 | 32.0 | 20.2 | 4.7 |
| bottom | vulture-healer-cinder | 53 | 32.1 | 39.8 | 0.0 |
| bottom | human-duelist-rapier | 49 | 32.7 | 25.5 | 1.2 |
| bottom | thorntreant-primalist-bramble | 30 | 33.3 | 9.7 | 0.0 |

## Classes and keystones

| Kind | Name | Appearances | Win rate % |
|---|---|---:|---:|
| class | summoner | 858 | 71.7 |
| class | pyromancer | 167 | 61.7 |
| class | cryomancer | 404 | 55.9 |
| class | berserker | 1563 | 53.6 |
| class | spiritwalker | 359 | 52.1 |
| class | chemist | 806 | 51.9 |
| class | tracker | 1697 | 51.7 |
| class | ritualist | 362 | 51.1 |
| class | thief | 1282 | 50.5 |
| class | bulwark | 2014 | 50.2 |
| class | assassin | 518 | 50.2 |
| class | engineer | 1612 | 49.6 |
| class | saboteur | 156 | 47.4 |
| class | transmuter | 410 | 47.3 |
| class | commander | 1356 | 46.4 |
| class | bard | 877 | 46.3 |
| class | champion | 1848 | 45.6 |
| class | lifebinder | 348 | 45.4 |
| class | stormcaller | 615 | 45.2 |
| class | skyborne | 1660 | 45.1 |
| class | keeper | 595 | 45.0 |
| class | enchanter | 1159 | 44.4 |
| class | cleric | 1196 | 44.4 |
| class | tactician | 154 | 44.2 |
| class | totemist | 418 | 43.3 |
| class | mender | 547 | 43.1 |
| class | hexer | 646 | 42.1 |
| class | necromancer | 373 | 41.8 |
| keystone | quickening | 326 | 80.7 |
| keystone | golem_heart | 315 | 70.8 |
| keystone | wildfire | 55 | 65.5 |
| keystone | inferno | 93 | 62.4 |
| keystone | chaos_agent | 67 | 59.7 |
| keystone | last_stand | 456 | 59.6 |
| keystone | portal | 217 | 59.4 |
| keystone | initiative | 51 | 58.8 |
| keystone | master_thief | 337 | 58.8 |
| keystone | soul_anchor | 96 | 58.3 |
| keystone | frostbinder | 89 | 57.3 |
| keystone | brittle | 153 | 56.9 |
| keystone | deep_chill | 129 | 56.6 |
| keystone | sniff_the_air | 563 | 55.8 |
| keystone | relentless | 472 | 55.7 |
| keystone | cold_heart | 122 | 54.1 |
| keystone | volatile | 237 | 53.2 |
| keystone | big_finish | 356 | 53.1 |
| keystone | unbreakable | 677 | 53.0 |
| keystone | reinforced | 544 | 52.9 |
| keystone | puppeteer | 150 | 52.7 |
| keystone | lasting_change | 160 | 52.5 |
| keystone | rapid_fire | 414 | 52.4 |
| keystone | deluge | 161 | 52.2 |
| keystone | lifeblood | 94 | 52.1 |
| keystone | reckless_swing | 535 | 52.0 |
| keystone | rally | 557 | 51.7 |
| keystone | venom_blades | 118 | 51.7 |
| keystone | hold_position | 660 | 51.5 |
| keystone | sky_lord | 496 | 51.4 |
| keystone | grand_chorus | 422 | 50.9 |
| keystone | martyr | 106 | 50.9 |
| keystone | overflowing_care | 118 | 50.8 |
| keystone | unstoppable | 572 | 50.2 |
| keystone | unseen | 218 | 50.0 |
| keystone | shadow_purse | 468 | 50.0 |
| keystone | vanishing_kill | 182 | 49.5 |
| keystone | spell_ward | 369 | 48.8 |
| keystone | potent_brew | 213 | 48.4 |
| keystone | purifying_aura | 262 | 48.1 |
| keystone | duelist | 588 | 47.8 |
| keystone | hunt_down | 617 | 47.6 |
| keystone | rhythm | 225 | 47.6 |
| keystone | ashwalker | 19 | 47.4 |
| keystone | eye_of_the_storm | 241 | 47.3 |
| keystone | thorned_ward | 199 | 46.7 |
| keystone | steadfast_ward | 148 | 46.6 |
| keystone | disarm_trap | 39 | 46.2 |
| keystone | shield_wall | 677 | 46.1 |
| keystone | haunting | 113 | 46.0 |
| keystone | scent_of_blood | 662 | 45.5 |
| keystone | brittle_glass | 110 | 45.5 |
| keystone | twin_enchant | 466 | 45.3 |
| keystone | escape_artist | 477 | 45.1 |
| keystone | self_repair | 654 | 45.0 |
| keystone | tidecaller | 112 | 44.6 |
| keystone | eagle_eye | 601 | 44.6 |
| keystone | lich_form | 240 | 44.6 |
| keystone | spirit_shield | 27 | 44.4 |
| keystone | overcharge | 194 | 44.3 |
| keystone | sanctity | 253 | 43.9 |
| keystone | vanguard | 400 | 43.5 |
| keystone | witchs_patience | 513 | 43.5 |
| keystone | great_totem | 203 | 43.3 |
| keystone | static_field | 180 | 43.3 |
| keystone | judgment | 681 | 43.2 |
| keystone | inspiring_presence | 58 | 43.1 |
| keystone | sturdy_totems | 188 | 43.1 |
| keystone | mirror_form | 140 | 42.9 |
| keystone | twin_wards | 248 | 42.7 |
| keystone | drill | 399 | 41.9 |
| keystone | glory | 643 | 41.7 |
| keystone | triage | 281 | 41.3 |
| keystone | regrowth | 148 | 40.5 |
| keystone | lift | 563 | 40.1 |
| keystone | bone_armor | 46 | 39.1 |
| keystone | lingering_magic | 324 | 38.3 |
| keystone | spreading_curse | 117 | 37.6 |
| keystone | phoenix | 148 | 37.2 |
| keystone | echo | 230 | 36.5 |
| keystone | deaths_bounty | 87 | 35.6 |
| keystone | wrecker | 50 | 32.0 |
| keystone | lingering_malice | 16 | 31.2 |
| keystone | all_in | 45 | 28.9 |

## Skills: best and worst win rate of the units that have them (at least 50 units), and usage

| | Skill | Units | Win rate % | Uses per unit |
|---|---|---:|---:|---:|
| best | toll | 51 | 76.5 | 0.2 |
| best | clay_golem | 858 | 71.7 | 2.2 |
| best | rune_golem | 858 | 71.7 | 0.0 |
| best | animate_blades | 858 | 71.7 | 5.5 |
| best | quick_jab | 60 | 70.0 | 1.2 |
| best | rope_hook | 65 | 69.2 | 1.8 |
| best | jaw_snap | 60 | 66.7 | 1.8 |
| best | hammer_claw | 73 | 64.4 | 2.4 |
| best | frost_arrow | 78 | 64.1 | 0.2 |
| best | fire_spark | 93 | 62.4 | 7.8 |
| best | fireball | 167 | 61.7 | 2.3 |
| best | kindle | 167 | 61.7 | 0.7 |
| best | row_of_fire | 167 | 61.7 | 0.0 |
| best | rune_cannon | 57 | 61.4 | 7.3 |
| best | axe_hook | 53 | 60.4 | 2.5 |
| worst | soul_rend | 78 | 24.4 | 3.9 |
| worst | glyph_strike | 71 | 25.4 | 2.2 |
| worst | turbine_blade | 90 | 28.9 | 4.5 |
| worst | cinder_dive | 53 | 32.1 | 3.0 |
| worst | horn_butt | 139 | 35.3 | 3.4 |
| worst | lantern_ray | 51 | 35.3 | 6.2 |
| worst | wing_buzz | 85 | 35.3 | 1.3 |
| worst | starsight_ray | 62 | 35.5 | 3.1 |
| worst | death_roll | 61 | 36.1 | 0.9 |
| worst | moonwind_theft | 69 | 36.2 | 9.8 |
| worst | petrifying_glare | 85 | 36.5 | 6.8 |
| worst | prism_wave | 63 | 36.5 | 1.4 |
| worst | sand_pulse | 112 | 36.6 | 2.7 |
| worst | owl_spark | 68 | 36.8 | 11.5 |
| worst | stone_fist | 53 | 37.7 | 1.8 |

Active skills never used in any battle (3): row_of_fire, cut_the_strings, totem_of_return

## What the AI does with each skill (turns where it was legal, searched further, chosen)

Skills with at least 200 legal turns. Searched = the search looked at one of its uses further; chosen = it was played.

| | Skill | Legal turns | Searched % of legal | Chosen % of legal | Chosen % of searched |
|---|---|---:|---:|---:|---:|
| rarely chosen | cut_the_strings | 586 | 0.0 | 0.0 |  |
| rarely chosen | row_of_fire | 780 | 0.6 | 0.0 | 0.0 |
| rarely chosen | totem_of_return | 1679 | 0.4 | 0.0 | 0.0 |
| rarely chosen | rune_golem | 5398 | 22.6 | 0.0 | 0.2 |
| rarely chosen | echo_enchant | 5770 | 0.5 | 0.1 | 26.9 |
| rarely chosen | read_tracks | 20349 | 17.1 | 0.2 | 1.0 |
| rarely chosen | melt_away | 14260 | 10.0 | 0.2 | 2.1 |
| rarely chosen | rescue | 7832 | 6.0 | 0.4 | 6.2 |
| rarely chosen | spirit_link_totem | 1679 | 70.5 | 0.6 | 0.8 |
| rarely chosen | tempest | 2459 | 4.7 | 0.7 | 13.8 |
| rarely chosen | desecrate | 1999 | 16.9 | 0.7 | 4.1 |
| rarely chosen | sweeping_kick | 4140 | 6.7 | 0.9 | 13.0 |
| rarely chosen | burnout_heal | 1396 | 4.6 | 0.9 | 20.3 |
| rarely chosen | withering_curse | 7199 | 21.0 | 1.3 | 6.2 |
| rarely chosen | press_the_advantage | 975 | 12.7 | 1.4 | 11.3 |
| often chosen | soul_rend | 342 | 95.6 | 89.2 | 93.3 |
| often chosen | grave_peck | 295 | 95.3 | 86.1 | 90.4 |
| often chosen | shadow_carve | 298 | 97.7 | 84.9 | 86.9 |
| often chosen | mirror_ray | 1560 | 98.8 | 84.0 | 85.0 |
| often chosen | spinning_hammer | 561 | 97.7 | 83.4 | 85.4 |
| often chosen | raking_claws | 279 | 93.9 | 82.8 | 88.2 |
| often chosen | venom_fang | 559 | 97.7 | 81.0 | 83.0 |
| often chosen | ring_spin | 487 | 99.6 | 80.7 | 81.0 |
| often chosen | orbit_missile | 1603 | 99.8 | 79.4 | 79.5 |
| often chosen | shadow_touch | 225 | 96.9 | 79.1 | 81.7 |
| often chosen | valiant_strike | 424 | 93.6 | 78.8 | 84.1 |
| often chosen | balm_flask | 976 | 98.0 | 77.7 | 79.3 |
| often chosen | ice_lance | 4578 | 98.8 | 77.3 | 78.2 |
| often chosen | ranger_arrow | 1186 | 96.6 | 77.1 | 79.8 |
| often chosen | pike_thrust | 313 | 95.2 | 73.8 | 77.5 |

The full list is in `skills.csv`.

## Win rate by starting row (all creatures)

| Row | Appearances | Win rate % |
|---|---:|---:|
| Front | 5217 | 47.7 |
| Middle | 2307 | 41.3 |
| Rear | 4476 | 54.1 |

## Teammate pairs (at least 25 shared battles)

| | Pair | Battles | Win rate % |
|---|---|---:|---:|
| best | human-priest-moon + thornfang | 34 | 55.9 |
| best | human-priest-moon + scorpion-healer-ribbon | 37 | 54.1 |
| best | candelabrum-warrior-wax + human-priest-moon | 36 | 52.8 |
| best | human-priest-moon + retort-alchemist | 25 | 52.0 |
| best | human-priest-moon + lizardfolk-barbarian-axe | 29 | 51.7 |
| best | crab-smith + human-priest-moon | 28 | 50.0 |
| best | human-knight + human-priest-moon | 25 | 48.0 |
| best | human-priest-moon + orrery-sorcerer-orbit | 30 | 46.7 |
| best | hourglass-hexer-sand + human-priest-moon | 29 | 41.4 |
| best | human-guard-stone + human-priest-moon | 27 | 40.7 |
| worst | human-priest-moon + owl-wizard | 25 | 24.0 |
| worst | human-priest-moon + sextant-admiral | 27 | 29.6 |
| worst | human-barbarian-frost + human-priest-moon | 26 | 30.8 |
| worst | human-cleric-lantern + human-priest-moon | 28 | 35.7 |
| worst | human-priest-moon + sewingbox-engineer-ribbon | 25 | 36.0 |
| worst | human-priest-moon + mirror-revenant | 27 | 37.0 |
| worst | human-archer-sun + human-priest-moon | 41 | 39.0 |
| worst | human-archer + human-priest-moon | 30 | 40.0 |
| worst | human-priest-moon + wolf-ranger | 27 | 40.7 |
| worst | human-guard-stone + human-priest-moon | 27 | 40.7 |

