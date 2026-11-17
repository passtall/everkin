# Run 20261117-203056-harness

AI level **harness**, seeds 1 to 1000, rules commit `f9d0afd531d299aafa7b1ca8adfffc58b55763e2`.

- Battles recorded: 1000 (999 played to an end, 1 crashed)
- Side A wins 481, side B wins 495, draws 23 (2.3%), of which 23 hit the length cap
- Battle length in unit turns: median 164, mean 187.1, shortest 57, longest 508
- Average real time per battle: 15.4 s

## Momentum per side

The meter is sampled after every unit turn; lead is the side's own view (positive = ahead).

| Side | Samples | Ahead % | Level % | Behind % | Average lead | Payments per battle | Momentum paid per battle | Turns it could pay | Of those, did not pay % |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 142249 | 38.3 | 22.8 | 39.0 | -0.0 | 15.0 | 23.1 | 27732 | 46.0 |
| B | 142249 | 39.0 | 22.8 | 38.3 | 0.0 | 15.4 | 23.5 | 28135 | 45.3 |

Most paid-for skills:

| Skill | Payments | Momentum paid |
|---|---:|---:|
| shield_bash | 2953 | 2953 |
| lift_a_skill | 2923 | 2923 |
| rampage | 2322 | 2322 |
| hunters_shot | 2283 | 4566 |
| clay_golem | 2055 | 4078 |
| lullaby | 1838 | 3676 |
| lightning_bolt | 1344 | 2688 |
| dive | 1329 | 2658 |
| mend | 1324 | 1324 |
| bolt_turret | 1307 | 1307 |
| piercing_enchant | 1001 | 1001 |
| dispel | 989 | 989 |
| rotation | 850 | 1661 |
| life_swap | 801 | 2403 |
| shield_dome | 791 | 2373 |

## Imbalance flags: top and bottom 10% by win rate (21 each, creatures with at least 20 appearances)

| | Creature | Appearances | Win rate % | Avg damage | Avg healing |
|---|---|---:|---:|---:|---:|
| top | toad-bard-bell | 24 | 87.5 | 96.3 | 0.0 |
| top | beetle-2 | 31 | 87.1 | 60.2 | 0.0 |
| top | badger-summoner-drum | 33 | 84.8 | 125.9 | 0.0 |
| top | brazier-summoner-coal | 38 | 84.2 | 152.6 | 0.0 |
| top | crane-summoner-feather | 31 | 83.9 | 133.9 | 0.0 |
| top | yak-bard-gong | 23 | 82.6 | 115.1 | 0.0 |
| top | pangolin-sorcerer-thunder | 27 | 81.5 | 140.6 | 0.0 |
| top | beacon-summoner | 42 | 81.0 | 120.7 | 0.0 |
| top | reliquary-summoner-seal | 23 | 78.3 | 129.0 | 0.0 |
| top | corkscrew-stalker | 36 | 77.8 | 40.0 | 0.0 |
| top | tapestry-summoner | 29 | 75.9 | 125.7 | 0.0 |
| top | termite-summoner-clockwork | 20 | 75.0 | 121.8 | 0.0 |
| top | printingpress-summoner-type | 35 | 74.3 | 114.8 | 0.0 |
| top | maskrack-summoner-echo | 23 | 73.9 | 111.7 | 0.0 |
| top | mandrake-primalist-root | 26 | 73.1 | 65.0 | 17.6 |
| top | bee-shaman-sun | 26 | 73.1 | 10.3 | 40.5 |
| top | hydra-colossus-trident | 32 | 71.9 | 84.9 | 0.0 |
| top | panpipe-summoner-reed | 21 | 71.4 | 43.7 | 0.0 |
| top | stag-glade | 38 | 71.1 | 16.1 | 79.2 |
| top | golem-colossus-furnace | 41 | 70.7 | 35.5 | 0.0 |
| top | harpy-eagle-scout-claws | 44 | 70.5 | 36.3 | 0.0 |
| bottom | drum-summoner-war | 23 | 13.0 | 6.8 | 0.0 |
| bottom | gong-hexer-dissonance | 38 | 13.2 | 3.7 | 4.9 |
| bottom | goblet-paladin | 22 | 18.2 | 2.4 | 0.0 |
| bottom | sistrum-summoner | 32 | 18.8 | 13.6 | 0.0 |
| bottom | treant-druid-leaf | 20 | 20.0 | 18.1 | 48.0 |
| bottom | abacus-strategist | 35 | 22.9 | 2.6 | 0.0 |
| bottom | buffalo-guard-root | 25 | 24.0 | 22.3 | 14.0 |
| bottom | howlermonkey-bard-roar | 37 | 24.3 | 2.6 | 0.0 |
| bottom | turbine-skyguard | 90 | 25.6 | 17.6 | 0.7 |
| bottom | human-necromancer-skull | 78 | 25.6 | 14.0 | 0.0 |
| bottom | beetle-knight-shield | 39 | 25.6 | 25.8 | 0.0 |
| bottom | thorntreant-primalist-bramble | 30 | 26.7 | 8.4 | 0.0 |
| bottom | kiln-behemoth | 31 | 29.0 | 23.6 | 0.0 |
| bottom | human-priest-bell | 120 | 29.2 | 3.6 | 0.0 |
| bottom | human-spellblade | 71 | 29.6 | 9.2 | 0.0 |
| bottom | baton-marshal | 30 | 30.0 | 3.0 | 0.0 |
| bottom | goat-shaman-totem | 138 | 31.2 | 11.8 | 85.5 |
| bottom | lobster-bard-castanet | 35 | 31.4 | 1.5 | 0.0 |
| bottom | seal-bard-bottle | 35 | 31.4 | 6.1 | 13.6 |
| bottom | human-spellblade-2 | 41 | 31.7 | 16.2 | 0.0 |
| bottom | lantern-healer-prism | 63 | 31.7 | 24.7 | 14.9 |

## Classes and keystones

| Kind | Name | Appearances | Win rate % |
|---|---|---:|---:|
| class | summoner | 858 | 73.8 |
| class | pyromancer | 167 | 65.9 |
| class | cryomancer | 403 | 60.0 |
| class | chemist | 806 | 52.6 |
| class | engineer | 1610 | 52.2 |
| class | assassin | 518 | 51.9 |
| class | tracker | 1695 | 51.9 |
| class | berserker | 1562 | 51.6 |
| class | ritualist | 360 | 51.4 |
| class | bulwark | 2013 | 51.4 |
| class | spiritwalker | 359 | 51.0 |
| class | thief | 1282 | 50.5 |
| class | stormcaller | 614 | 49.0 |
| class | skyborne | 1659 | 48.9 |
| class | saboteur | 156 | 48.7 |
| class | transmuter | 410 | 47.8 |
| class | enchanter | 1159 | 47.5 |
| class | totemist | 416 | 45.7 |
| class | champion | 1848 | 44.9 |
| class | lifebinder | 346 | 44.8 |
| class | commander | 1354 | 44.7 |
| class | keeper | 594 | 44.4 |
| class | necromancer | 372 | 43.5 |
| class | mender | 546 | 42.7 |
| class | bard | 875 | 41.6 |
| class | tactician | 154 | 40.9 |
| class | cleric | 1194 | 36.3 |
| class | hexer | 646 | 35.0 |
| keystone | quickening | 326 | 80.4 |
| keystone | golem_heart | 315 | 74.3 |
| keystone | wildfire | 55 | 70.9 |
| keystone | inferno | 93 | 64.5 |
| keystone | brittle | 152 | 64.5 |
| keystone | portal | 217 | 63.1 |
| keystone | deep_chill | 129 | 62.0 |
| keystone | master_thief | 337 | 59.3 |
| keystone | last_stand | 455 | 58.7 |
| keystone | frostbinder | 89 | 58.4 |
| keystone | ashwalker | 19 | 57.9 |
| keystone | rapid_fire | 413 | 56.7 |
| keystone | sniff_the_air | 562 | 56.6 |
| keystone | volatile | 237 | 56.5 |
| keystone | unseen | 218 | 55.5 |
| keystone | puppeteer | 150 | 55.3 |
| keystone | sky_lord | 495 | 54.9 |
| keystone | unbreakable | 677 | 54.7 |
| keystone | venom_blades | 118 | 54.2 |
| keystone | reinforced | 544 | 54.2 |
| keystone | chaos_agent | 67 | 53.7 |
| keystone | lifeblood | 94 | 53.2 |
| keystone | deluge | 161 | 52.8 |
| keystone | relentless | 472 | 52.8 |
| keystone | cold_heart | 122 | 52.5 |
| keystone | hold_position | 659 | 52.0 |
| keystone | big_finish | 356 | 52.0 |
| keystone | spell_ward | 369 | 51.5 |
| keystone | martyr | 105 | 51.4 |
| keystone | disarm_trap | 39 | 51.3 |
| keystone | rally | 556 | 51.1 |
| keystone | eagle_eye | 601 | 50.7 |
| keystone | unstoppable | 572 | 50.7 |
| keystone | static_field | 180 | 50.0 |
| keystone | rhythm | 224 | 49.6 |
| keystone | eye_of_the_storm | 241 | 49.4 |
| keystone | shadow_purse | 468 | 49.4 |
| keystone | potent_brew | 213 | 49.3 |
| keystone | brittle_glass | 110 | 49.1 |
| keystone | initiative | 51 | 49.0 |
| keystone | haunting | 113 | 48.7 |
| keystone | sturdy_totems | 187 | 48.7 |
| keystone | lasting_change | 160 | 48.1 |
| keystone | twin_enchant | 466 | 47.9 |
| keystone | overcharge | 193 | 47.7 |
| keystone | self_repair | 653 | 47.6 |
| keystone | shield_wall | 677 | 47.4 |
| keystone | lich_form | 239 | 47.3 |
| keystone | scent_of_blood | 661 | 47.2 |
| keystone | hunt_down | 617 | 47.0 |
| keystone | soul_anchor | 96 | 46.9 |
| keystone | overflowing_care | 118 | 46.6 |
| keystone | reckless_swing | 535 | 46.5 |
| keystone | mirror_form | 140 | 46.4 |
| keystone | duelist | 588 | 46.3 |
| keystone | vanishing_kill | 182 | 46.2 |
| keystone | escape_artist | 477 | 45.3 |
| keystone | thorned_ward | 199 | 45.2 |
| keystone | vanguard | 399 | 44.9 |
| keystone | twin_wards | 248 | 44.4 |
| keystone | great_totem | 202 | 44.1 |
| keystone | tidecaller | 110 | 43.6 |
| keystone | steadfast_ward | 147 | 43.5 |
| keystone | spreading_curse | 117 | 42.7 |
| keystone | regrowth | 148 | 42.6 |
| keystone | lingering_magic | 324 | 42.3 |
| keystone | glory | 643 | 41.7 |
| keystone | lift | 563 | 41.6 |
| keystone | sanctity | 251 | 41.4 |
| keystone | inspiring_presence | 58 | 41.4 |
| keystone | triage | 280 | 41.1 |
| keystone | grand_chorus | 422 | 40.3 |
| keystone | wrecker | 50 | 40.0 |
| keystone | bone_armor | 46 | 39.1 |
| keystone | purifying_aura | 262 | 38.5 |
| keystone | lingering_malice | 16 | 37.5 |
| keystone | spirit_shield | 27 | 37.0 |
| keystone | crescendo | 229 | 36.2 |
| keystone | deaths_bounty | 87 | 35.6 |
| keystone | drill | 399 | 35.6 |
| keystone | phoenix | 147 | 34.7 |
| keystone | judgment | 681 | 33.6 |
| keystone | witchs_patience | 513 | 33.1 |
| keystone | all_in | 45 | 31.1 |

## Skills: best and worst win rate of the units that have them (at least 50 units), and usage

| | Skill | Units | Win rate % | Uses per unit |
|---|---|---:|---:|---:|
| best | clay_golem | 858 | 73.8 | 2.4 |
| best | rune_golem | 858 | 73.8 | 0.0 |
| best | animate_blades | 858 | 73.8 | 5.7 |
| best | toll | 51 | 66.7 | 0.1 |
| best | fireball | 167 | 65.9 | 2.6 |
| best | kindle | 167 | 65.9 | 0.7 |
| best | row_of_fire | 167 | 65.9 | 0.0 |
| best | rope_hook | 64 | 65.6 | 1.8 |
| best | quick_jab | 60 | 65.0 | 1.1 |
| best | fire_spark | 93 | 64.5 | 8.4 |
| best | rune_cannon | 57 | 63.2 | 6.9 |
| best | frost_spark | 105 | 62.9 | 0.2 |
| best | frost_missile | 114 | 62.3 | 0.6 |
| best | frost_arrow | 78 | 61.5 | 0.2 |
| best | ice_lance | 403 | 60.0 | 9.3 |
| worst | turbine_blade | 90 | 25.6 | 4.5 |
| worst | soul_rend | 78 | 25.6 | 3.5 |
| worst | bell_toll | 120 | 29.2 | 0.8 |
| worst | glyph_strike | 71 | 29.6 | 2.4 |
| worst | horn_butt | 138 | 31.2 | 3.6 |
| worst | prism_wave | 63 | 31.7 | 0.7 |
| worst | eclipse | 493 | 31.8 | 0.6 |
| worst | warning_shot | 55 | 32.7 | 5.4 |
| worst | withering_curse | 646 | 35.0 | 0.3 |
| worst | curse_of_weight | 646 | 35.0 | 1.5 |
| worst | curse_of_betrayal | 646 | 35.0 | 0.3 |
| worst | sun_arrow | 73 | 35.6 | 9.2 |
| worst | moonwind_theft | 69 | 36.2 | 10.3 |
| worst | turn_back | 1194 | 36.3 | 2.3 |
| worst | dispel | 1194 | 36.3 | 0.8 |

Active skills never used in any battle (3): row_of_fire, cut_the_strings, totem_of_return

## What the AI does with each skill (turns where it was legal, searched further, chosen)

Skills with at least 200 legal turns. Searched = the search looked at one of its uses further; chosen = it was played.

| | Skill | Legal turns | Searched % of legal | Chosen % of legal | Chosen % of searched |
|---|---|---:|---:|---:|---:|
| rarely chosen | cut_the_strings | 604 | 0.3 | 0.0 | 0.0 |
| rarely chosen | row_of_fire | 829 | 0.7 | 0.0 | 0.0 |
| rarely chosen | totem_of_return | 1853 | 0.1 | 0.0 | 0.0 |
| rarely chosen | rune_golem | 5767 | 22.0 | 0.1 | 0.3 |
| rarely chosen | read_tracks | 21578 | 16.3 | 0.1 | 0.7 |
| rarely chosen | echo_enchant | 6242 | 0.5 | 0.2 | 33.3 |
| rarely chosen | melt_away | 15200 | 10.8 | 0.2 | 1.8 |
| rarely chosen | desecrate | 2060 | 16.3 | 0.2 | 1.5 |
| rarely chosen | rescue | 7910 | 6.8 | 0.3 | 5.0 |
| rarely chosen | storm_plume | 285 | 22.5 | 0.4 | 1.6 |
| rarely chosen | spirit_link_totem | 1853 | 73.0 | 0.4 | 0.6 |
| rarely chosen | burnout_heal | 1540 | 3.0 | 0.5 | 15.2 |
| rarely chosen | sweeping_kick | 4141 | 6.3 | 0.8 | 13.4 |
| rarely chosen | tempest | 2636 | 5.0 | 0.9 | 18.8 |
| rarely chosen | press_the_advantage | 964 | 15.1 | 1.1 | 7.5 |
| often chosen | venom_fang | 451 | 97.3 | 88.9 | 91.3 |
| often chosen | grave_peck | 249 | 94.4 | 87.1 | 92.3 |
| often chosen | soul_rend | 319 | 95.6 | 86.5 | 90.5 |
| often chosen | shadow_carve | 314 | 98.1 | 83.8 | 85.4 |
| often chosen | shadow_touch | 227 | 98.7 | 83.7 | 84.8 |
| often chosen | orbit_missile | 1617 | 99.7 | 81.8 | 82.0 |
| often chosen | valiant_strike | 388 | 95.4 | 81.2 | 85.1 |
| often chosen | pike_thrust | 321 | 97.5 | 79.8 | 81.8 |
| often chosen | balm_flask | 1101 | 97.7 | 79.6 | 81.4 |
| often chosen | ring_spin | 554 | 96.6 | 78.2 | 80.9 |
| often chosen | ranger_arrow | 1390 | 98.3 | 77.8 | 79.2 |
| often chosen | mirror_ray | 1573 | 96.1 | 77.4 | 80.5 |
| often chosen | arcane_edge | 225 | 92.9 | 76.0 | 81.8 |
| often chosen | ice_lance | 4949 | 98.4 | 75.9 | 77.2 |
| often chosen | smoke_kunai | 1506 | 98.3 | 75.8 | 77.1 |

The full list is in `skills.csv`.

## Win rate by starting row (all creatures)

| Row | Appearances | Win rate % |
|---|---:|---:|
| Front | 5212 | 48.3 |
| Middle | 2303 | 37.4 |
| Rear | 4473 | 55.4 |

## Teammate pairs (at least 25 shared battles)

| | Pair | Battles | Win rate % |
|---|---|---:|---:|
| best | human-knight + human-priest-moon | 25 | 44.0 |
| best | human-archer + human-priest-moon | 30 | 43.3 |
| best | human-guard-stone + human-priest-moon | 27 | 40.7 |
| best | candelabrum-warrior-wax + human-priest-moon | 36 | 38.9 |
| best | human-priest-moon + thornfang | 34 | 38.2 |
| best | human-priest-moon + orrery-sorcerer-orbit | 30 | 36.7 |
| best | human-priest-moon + scorpion-healer-ribbon | 37 | 35.1 |
| best | human-priest-moon + wolf-ranger | 27 | 33.3 |
| best | human-cleric-lantern + human-priest-moon | 28 | 32.1 |
| best | human-priest-moon + retort-alchemist | 25 | 32.0 |
| worst | human-archer-sun + human-priest-moon | 41 | 17.1 |
| worst | human-priest-moon + lizardfolk-barbarian-axe | 29 | 17.2 |
| worst | crab-smith + human-priest-moon | 28 | 21.4 |
| worst | human-priest-moon + mirror-revenant | 27 | 22.2 |
| worst | human-priest-moon + sextant-admiral | 27 | 22.2 |
| worst | human-barbarian-frost + human-priest-moon | 26 | 30.8 |
| worst | hourglass-hexer-sand + human-priest-moon | 29 | 31.0 |
| worst | human-priest-moon + sewingbox-engineer-ribbon | 25 | 32.0 |
| worst | human-priest-moon + owl-wizard | 25 | 32.0 |
| worst | human-priest-moon + retort-alchemist | 25 | 32.0 |

## Crashed battles

- 1x `InvalidOperationException: Unit 4 is KO and cannot be healed, only revived.` (first seed 56)

