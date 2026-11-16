# Run 20261116-191930-harness

AI level **harness**, seeds 1 to 10000, rules commit `b6b59e330bf74e74eed5d36d488030ccc3a0e8e9`.

- Battles recorded: 5737 (5737 played to an end, 0 crashed)
- Side A wins 2789, side B wins 2783, draws 165 (2.9%), of which 165 hit the length cap
- Battle length in unit turns: median 161, mean 187.2, shortest 38, longest 508
- Average real time per battle: 9.1 s

## Momentum per side

The meter is sampled after every unit turn; lead is the side's own view (positive = ahead).

| Side | Samples | Ahead % | Level % | Behind % | Average lead | Payments per battle | Momentum paid per battle | Turns it could pay | Of those, did not pay % |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A | 844546 | 39.7 | 19.9 | 40.4 | -0.1 | 10.3 | 28.3 | 163046 | 63.8 |
| B | 844546 | 40.4 | 19.9 | 39.7 | 0.1 | 10.3 | 28.2 | 166476 | 64.5 |

Most paid-for skills:

| Skill | Payments | Momentum paid |
|---|---:|---:|
| clay_golem | 21227 | 83010 |
| dive | 14886 | 29772 |
| hunters_shot | 12308 | 24616 |
| animate_blades | 9511 | 18983 |
| rampage | 9465 | 18930 |
| lightning_bolt | 8742 | 17484 |
| lift_a_skill | 6616 | 19848 |
| shield_dome | 5222 | 26110 |
| life_swap | 5159 | 25795 |
| bolt_turret | 4498 | 8996 |
| ice_lance | 3427 | 6854 |
| coordinated_strike | 2871 | 7800 |
| rain_dance | 2516 | 7548 |
| firebomb | 2437 | 4874 |
| rain_of_mending | 2277 | 4554 |

## Imbalance flags: top and bottom 10% by win rate (31 each, creatures with at least 20 appearances)

| | Creature | Appearances | Win rate % | Avg damage | Avg healing |
|---|---|---:|---:|---:|---:|
| top | muskox-bodyguard-buckler | 174 | 70.7 | 83.7 | 0.0 |
| top | teapot-alchemist-volcano | 222 | 70.3 | 51.6 | 18.9 |
| top | inkwell-trickster-quill | 235 | 69.8 | 14.2 | 0.0 |
| top | toad-boulder | 197 | 69.5 | 91.1 | 0.0 |
| top | chameleon-trickster-tongue | 241 | 69.3 | 22.1 | 0.0 |
| top | ossuary-summoner-cube | 187 | 68.4 | 37.8 | 0.0 |
| top | ankylosaur-guardian-club | 196 | 68.4 | 81.0 | 0.0 |
| top | beetle-2 | 238 | 68.1 | 76.8 | 0.0 |
| top | gaur-bodyguard-armor | 193 | 67.9 | 87.0 | 0.0 |
| top | human-mage-fire | 211 | 67.8 | 48.8 | 0.3 |
| top | orchidmantis-trickster-leaf | 228 | 66.7 | 11.1 | 7.4 |
| top | chest-bodyguard-lock | 220 | 66.4 | 53.9 | 0.0 |
| top | mannequin | 216 | 66.2 | 81.9 | 0.0 |
| top | sledgehammer-colossus | 165 | 66.1 | 85.2 | 0.0 |
| top | millstone-juggernaut | 180 | 65.6 | 59.1 | 0.0 |
| top | horseshoecrab-sentinel-bronze | 213 | 64.8 | 62.1 | 0.0 |
| top | behemoth-titan-iron | 176 | 64.8 | 59.8 | 0.0 |
| top | fiddlercrab-trickster-coin | 215 | 64.7 | 11.8 | 0.0 |
| top | loom-binder | 172 | 64.5 | 72.5 | 0.0 |
| top | brazier-summoner-coal | 216 | 64.4 | 88.2 | 0.0 |
| top | vise-enforcer | 209 | 64.1 | 57.7 | 2.6 |
| top | pangolin-sorcerer-thunder | 215 | 63.3 | 79.6 | 0.0 |
| top | human-knight-2 | 174 | 63.2 | 51.5 | 0.0 |
| top | tortoise-sentinel-obsidian | 163 | 63.2 | 98.5 | 0.0 |
| top | orrery-sorcerer-orbit | 197 | 62.4 | 47.8 | 0.0 |
| top | human-rogue-shadow | 199 | 62.3 | 100.1 | 0.0 |
| top | astrolabe-judge | 205 | 62.0 | 44.9 | 0.0 |
| top | human-knight-shield | 195 | 61.5 | 51.4 | 0.0 |
| top | mammoth-titan-tundra | 194 | 61.3 | 51.0 | 0.0 |
| top | bellows-drake | 207 | 60.9 | 78.0 | 0.2 |
| top | mole-engineer-gear | 291 | 60.8 | 54.1 | 37.2 |
| bottom | goblet-paladin | 207 | 28.5 | 5.2 | 0.0 |
| bottom | hydra-colossus-trident | 160 | 28.8 | 30.9 | 0.0 |
| bottom | quokka-summoner-pebble | 274 | 29.6 | 34.8 | 0.0 |
| bottom | bat-summoner-moon | 202 | 29.7 | 26.5 | 0.0 |
| bottom | pocketwatch-warden | 206 | 31.1 | 6.7 | 0.0 |
| bottom | kettle-alchemist-siege | 176 | 31.8 | 18.7 | 0.3 |
| bottom | cobra | 210 | 32.4 | 18.0 | 5.8 |
| bottom | furnace-titan | 157 | 33.1 | 13.7 | 0.0 |
| bottom | shrew-summoner-totem | 247 | 33.2 | 21.4 | 0.0 |
| bottom | maskrack-summoner-echo | 212 | 34.0 | 37.0 | 0.0 |
| bottom | weaverbird-summoner-banner | 244 | 34.0 | 25.6 | 0.0 |
| bottom | sugarglider-scout-sail | 219 | 34.2 | 8.4 | 0.0 |
| bottom | dragon-sovereign-tempest | 185 | 35.1 | 8.7 | 0.0 |
| bottom | human-guard-frost | 239 | 35.1 | 11.8 | 0.0 |
| bottom | drum-summoner-war | 179 | 35.2 | 8.7 | 0.0 |
| bottom | pika-summoner-horn | 258 | 35.3 | 26.8 | 0.0 |
| bottom | crow-necromancer-skull | 215 | 35.3 | 11.7 | 0.0 |
| bottom | carillon-summoner-chime | 195 | 35.4 | 7.1 | 0.0 |
| bottom | crucible-summoner | 212 | 35.8 | 15.0 | 0.0 |
| bottom | mole-engineer-hexhammer | 326 | 35.9 | 13.8 | 0.0 |
| bottom | kinkajou-summoner-idol | 320 | 35.9 | 41.8 | 0.1 |
| bottom | roadrunner-hunter-whistle | 241 | 36.1 | 28.4 | 0.0 |
| bottom | human-priest-moon | 190 | 36.3 | 29.6 | 0.0 |
| bottom | chinchilla-summoner-ocarina | 231 | 36.4 | 17.5 | 0.0 |
| bottom | accordion-warlord | 202 | 36.6 | 6.1 | 0.0 |
| bottom | roc-colossus-thunder | 199 | 36.7 | 33.5 | 0.0 |
| bottom | serval-hunter-crossbow | 234 | 36.8 | 34.6 | 0.1 |
| bottom | fossa-hunter-harpoon | 211 | 37.4 | 43.2 | 1.9 |
| bottom | heron-hunter-harpoon | 192 | 37.5 | 15.1 | 0.0 |
| bottom | cuttlefish-assassin-needle | 317 | 37.5 | 11.1 | 0.3 |
| bottom | salamander-rogue | 285 | 37.5 | 19.7 | 0.3 |

## Classes and keystones

| Kind | Name | Appearances | Win rate % |
|---|---|---:|---:|
| class | pyromancer | 840 | 63.8 |
| class | bulwark | 13067 | 55.1 |
| class | chemist | 5152 | 54.6 |
| class | saboteur | 3031 | 54.4 |
| class | berserker | 7182 | 52.3 |
| class | mender | 3040 | 51.3 |
| class | engineer | 8597 | 51.1 |
| class | tactician | 1148 | 50.8 |
| class | ritualist | 2416 | 49.2 |
| class | stormcaller | 2959 | 48.4 |
| class | commander | 8100 | 47.7 |
| class | champion | 8825 | 47.7 |
| class | spiritwalker | 2303 | 47.3 |
| class | cleric | 2751 | 47.0 |
| class | keeper | 4201 | 47.0 |
| class | tracker | 8188 | 46.7 |
| class | summoner | 9763 | 46.5 |
| class | necromancer | 1495 | 46.5 |
| class | assassin | 4512 | 46.4 |
| class | cryomancer | 1478 | 46.3 |
| class | hexer | 1747 | 46.1 |
| class | bard | 7300 | 45.9 |
| class | transmuter | 1465 | 45.7 |
| class | thief | 9339 | 45.4 |
| class | enchanter | 4882 | 45.4 |
| class | lifebinder | 1493 | 45.1 |
| class | skyborne | 10912 | 45.0 |
| class | totemist | 1502 | 40.7 |
| keystone | initiative | 235 | 69.8 |
| keystone | inferno | 211 | 67.8 |
| keystone | wildfire | 438 | 67.4 |
| keystone | chaos_agent | 924 | 63.2 |
| keystone | last_stand | 2495 | 61.3 |
| keystone | unbreakable | 4442 | 58.1 |
| keystone | hold_position | 4346 | 57.9 |
| keystone | potent_brew | 1656 | 56.0 |
| keystone | lich_form | 591 | 55.8 |
| keystone | triage | 864 | 55.7 |
| keystone | big_finish | 1926 | 55.4 |
| keystone | quickening | 3057 | 54.6 |
| keystone | rapid_fire | 2931 | 54.0 |
| keystone | tidecaller | 819 | 53.2 |
| keystone | wrecker | 1101 | 52.9 |
| keystone | rally | 2790 | 52.5 |
| keystone | volatile | 1570 | 52.2 |
| keystone | twin_wards | 1494 | 51.7 |
| keystone | ashwalker | 191 | 51.3 |
| keystone | soul_anchor | 847 | 51.1 |
| keystone | rhythm | 2316 | 50.9 |
| keystone | overcharge | 1009 | 50.6 |
| keystone | sanctity | 1066 | 50.5 |
| keystone | scent_of_blood | 2584 | 50.3 |
| keystone | static_field | 1095 | 50.3 |
| keystone | self_repair | 2768 | 50.2 |
| keystone | golem_heart | 3270 | 49.6 |
| keystone | regrowth | 1125 | 49.6 |
| keystone | overflowing_care | 1051 | 49.6 |
| keystone | hunt_down | 3111 | 49.2 |
| keystone | shield_wall | 4279 | 49.2 |
| keystone | cold_heart | 663 | 49.0 |
| keystone | reinforced | 2898 | 48.9 |
| keystone | unstoppable | 2324 | 48.7 |
| keystone | brittle_glass | 391 | 48.3 |
| keystone | twin_enchant | 1664 | 48.0 |
| keystone | disarm_trap | 1006 | 48.0 |
| keystone | spreading_curse | 517 | 48.0 |
| keystone | mirror_form | 424 | 47.6 |
| keystone | venom_blades | 1474 | 47.6 |
| keystone | vanguard | 2551 | 47.5 |
| keystone | martyr | 594 | 47.3 |
| keystone | deluge | 933 | 47.3 |
| keystone | glory | 2866 | 47.1 |
| keystone | frostbinder | 664 | 47.0 |
| keystone | puppeteer | 596 | 47.0 |
| keystone | inspiring_presence | 450 | 46.9 |
| keystone | unseen | 1759 | 46.7 |
| keystone | relentless | 2762 | 46.5 |
| keystone | sky_lord | 3744 | 46.5 |
| keystone | duelist | 2848 | 46.5 |
| keystone | reckless_swing | 2363 | 46.4 |
| keystone | judgment | 849 | 46.4 |
| keystone | thorned_ward | 1326 | 46.2 |
| keystone | deep_chill | 390 | 46.2 |
| keystone | witchs_patience | 679 | 46.1 |
| keystone | master_thief | 3190 | 45.8 |
| keystone | shadow_purse | 3019 | 45.7 |
| keystone | grand_chorus | 2607 | 45.4 |
| keystone | lift | 3507 | 45.0 |
| keystone | all_in | 463 | 44.9 |
| keystone | vanishing_kill | 1279 | 44.7 |
| keystone | escape_artist | 3130 | 44.7 |
| keystone | lifeblood | 442 | 44.6 |
| keystone | lingering_malice | 551 | 44.5 |
| keystone | lingering_magic | 1645 | 44.1 |
| keystone | spell_ward | 1573 | 43.9 |
| keystone | haunting | 860 | 43.8 |
| keystone | deaths_bounty | 419 | 43.7 |
| keystone | eagle_eye | 3661 | 43.6 |
| keystone | sniff_the_air | 2842 | 43.6 |
| keystone | eye_of_the_storm | 855 | 43.4 |
| keystone | purifying_aura | 836 | 43.3 |
| keystone | sturdy_totems | 393 | 43.3 |
| keystone | drill | 2759 | 43.0 |
| keystone | lasting_change | 650 | 42.9 |
| keystone | phoenix | 457 | 42.9 |
| keystone | steadfast_ward | 1381 | 42.5 |
| keystone | brittle | 425 | 42.1 |
| keystone | great_totem | 647 | 42.0 |
| keystone | crescendo | 2377 | 41.6 |
| keystone | bone_armor | 485 | 37.5 |
| keystone | spirit_shield | 462 | 36.6 |
| keystone | portal | 3436 | 36.4 |

## Skills: best and worst win rate of the units that have them (at least 50 units), and usage

| | Skill | Units | Win rate % | Uses per unit |
|---|---|---:|---:|---:|
| best | buckler_bash | 174 | 70.7 | 1.0 |
| best | volcano_spout | 222 | 70.3 | 0.4 |
| best | ink_splash | 235 | 69.8 | 14.5 |
| best | boulder_belly | 197 | 69.5 | 1.1 |
| best | snag_and_vanish | 241 | 69.3 | 9.1 |
| best | bone_rattle | 187 | 68.4 | 0.6 |
| best | tail_club | 196 | 68.4 | 0.6 |
| best | beetle_butt | 238 | 68.1 | 0.7 |
| best | armor_stomp | 193 | 67.9 | 0.6 |
| best | fire_spark | 211 | 67.8 | 5.9 |
| best | petal_slash | 228 | 66.7 | 6.3 |
| best | lid_snap | 220 | 66.4 | 1.1 |
| best | mannequin_slam | 216 | 66.2 | 0.7 |
| best | sledge_slam | 165 | 66.1 | 1.0 |
| best | millstone_crush | 180 | 65.6 | 0.6 |
| worst | holy_pour | 207 | 28.5 | 1.0 |
| worst | trident_gust | 160 | 28.8 | 0.5 |
| worst | happy_glow | 274 | 29.6 | 10.7 |
| worst | screech | 202 | 29.7 | 0.7 |
| worst | tick_tock | 206 | 31.1 | 1.9 |
| worst | boiling_spout | 176 | 31.8 | 0.2 |
| worst | venom_spray | 210 | 32.4 | 0.3 |
| worst | furnace_blast | 157 | 33.1 | 0.1 |
| worst | spirit_nip | 247 | 33.2 | 3.1 |
| worst | echo_scream | 212 | 34.0 | 0.8 |
| worst | banner_snap | 244 | 34.0 | 2.8 |
| worst | gliding_swoop | 219 | 34.2 | 5.2 |
| worst | tempest_breath | 185 | 35.1 | 1.7 |
| worst | ice_lance_thrust | 239 | 35.1 | 4.3 |
| worst | war_throb | 179 | 35.2 | 0.2 |

Active skills never used in any battle (9): petrify, curse_of_betrayal, row_of_fire, melt_away, cut_the_strings, desecrate, spirit_link_totem, totem_of_return, rite_of_mists

## Win rate by starting row (all creatures)

| Row | Appearances | Win rate % |
|---|---:|---:|
| Front | 30143 | 50.4 |
| Middle | 16229 | 44.7 |
| Rear | 22472 | 48.9 |

## Teammate pairs (at least 25 shared battles)

| | Pair | Battles | Win rate % |
|---|---|---:|---:|

