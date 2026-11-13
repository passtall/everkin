# Sound effects, stings and ambient loops

From production.md §1.6 (Sound list, adopted 2026-10-08): quiet, non-melodic, AI-generated with licensing checked per tool. Format **OGG Vorbis, 48 kHz**; effects **mono**, ambient **stereo**; all levelled to the same loudness (about −16 LUFS integrated for effects, −23 for ambient). Battle sounds come in **3 variants** (`_1`, `_2`, `_3`); the game adds a small random pitch change.

## Shared style line (paste after each prompt)

```
Style: soft storybook fantasy game sound, gentle and clean, no music, no melody, no voices, no reverb tail longer than the stated length, no clipping. Suitable for ages 7 and up: impacts are cushioned, never gory.
```

## Interface → `assets/audio/ui/`

| File | Length | Prompt |
|---|---|---|
| `confirm.ogg` | 0.2 s | A soft wooden click with a tiny bright chime on top. |
| `cancel.ogg` | 0.2 s | A soft low wooden tap, slightly falling. |
| `invalid.ogg` | 0.25 s | A muted double thud, like knocking on a padded book cover. |
| `panel_open.ogg` | 0.3 s | A soft paper and cloth swish opening upward. |
| `panel_close.ogg` | 0.3 s | The same swish closing, falling. |
| `hover.ogg` | 0.08 s | A very faint airy tick. |
| `tab_switch.ogg` | 0.15 s | A light page flick. |
| `drag_pickup.ogg` | 0.15 s | A card lifted off felt: soft papery slide up. |
| `drag_drop.ogg` | 0.2 s | A card set down on felt: soft papery pat. |
| `card_flip.ogg` | 0.3 s | A thick playing card flipping over with a soft snap. |
| `coins.ogg` | 0.5 s | A few small gold coins clinking softly into a pouch. |

## Battle → `assets/audio/battle/` (3 variants each)

| File | Length | Prompt |
|---|---|---|
| `melee_hit` | 0.3 s | A cushioned blunt impact with a short faceted crystal crunch, like hitting a low-poly figure. |
| `projectile_launch` | 0.25 s | A soft whoosh of something thrown or shot. |
| `projectile_hit` | 0.3 s | A small thunk with a short crystal shard sprinkle. |
| `magic_cast` | 0.5 s | A soft rising shimmer of glowing light gathering. |
| `magic_hit` | 0.4 s | A gentle magic burst, airy sparkle. |
| `heal` | 0.7 s | Warm rising soft chimes and a gentle breath of light. |
| `status_helpful` | 0.5 s | A short warm upward sparkle. |
| `status_harmful` | 0.5 s | A short low downward warble with a dull hum. |
| `status_end` | 0.4 s | A soft bubble pop fading out. |
| `intercept` | 0.35 s | A quick card slide followed by a muted shield clank. |
| `shield_absorb` | 0.35 s | A glassy hum, a hit muffled by a bubble. |
| `bump` | 0.25 s | Two wooden blocks knocking together softly. |
| `stealth_on` | 0.5 s | A soft fading whoosh into silence. |
| `stealth_off` | 0.4 s | A quick reveal swish. |
| `summon_appear` | 0.8 s | Glass shards gathering and clicking into place. |
| `ground_fire` | 0.6 s | A soft flame catching with a low crackle. |
| `ground_ice` | 0.5 s | Ice forming with small crisp crackles. |
| `ground_water` | 0.5 s | A gentle splash spreading into a puddle. |
| `ground_steam` | 0.6 s | A hiss of steam rising. |
| `ground_poison` | 0.6 s | Poison gas spreading: a soft hiss with a few slow bubbles popping. |
| `gas_explode` | 0.6 s | A soft muffled whump of gas going up, a short crackle. |
| `turn_chime` | 0.4 s | A single soft high bell, warm and short (played when the player's unit gets its turn). |
| `momentum_threshold` | 0.5 s | A rising soft spark whoosh. |
| `momentum_max` | 0.8 s | A bright swelling shimmer reaching a soft peak. |
| `ko_crack` | 0.4 s | A thin glassy crack spreading across a pane. |
| `ko_thud` | 0.4 s | A soft card flip ending in a muffled thud on felt. |

## Results → `assets/audio/result/`

| File | Length | Prompt |
|---|---|---|
| `victory.ogg` | 2 s | A warm non-melodic swell of soft chimes and glowing air rising to a gentle bright peak. |
| `defeat.ogg` | 2 s | A soft low falling swell, a single muted chime fading into night air. Not sad horns, not dramatic. |

## Biome ambient loops → `assets/audio/ambient/` (stereo, 60–90 s, seamless loop)

Prompt template: `A quiet night ambience loop of {scene}. Calm, sparse, no music, no voices, no sudden loud events. Seamless loop.`

| File | {scene} |
|---|---|
| `pine_forest.ogg` | a dry pine forest at night: wind in needles, an owl far away, crickets |
| `jungle.ogg` | a jungle at night: insects, tree frogs, distant drips |
| `wet_forest.ogg` | a damp mossy forest at night: dripping leaves, soft rain, frogs |
| `desert.ogg` | a desert at night: soft wind over sand, a far coyote-like call, rare insect chirps |
| `mountain.ogg` | a high mountain pass at night: thin wind, distant rockfall pebbles |
| `cathedral.ogg` | inside a vast empty castle hall at night: soft drafts, faint echoes, a distant creak |
| `savannah.ogg` | a savannah at night: warm wind in grass, crickets, a far animal call |
| `arctic.ogg` | an icy coast at night: cold wind, creaking ice, soft snow hiss |
| `swamp.ogg` | a swamp at night: frogs, bubbling mud, buzzing insects |
| `coast.ogg` | a rocky coast at night: gentle waves on pebbles, soft wind |
| `ashlands.ogg` | volcanic ashlands at night: low rumble, faint crackle of cooling lava, hot wind |
| `crystal_caverns.ogg` | a crystal cave: slow drips, soft glassy resonance, deep hush |
| `mushroom_grotto.ogg` | a glowing mushroom grotto: soft drips, gentle spore puffs, a low hum |
| `lagoon.ogg` | a calm lagoon at night: lapping water, soft reef bubbles, a far seabird |
| `ruins.ogg` | overgrown ruins at night: wind through stone, rustling ivy, crickets |
| `autumn_woods.ogg` | autumn woods at night: rustling dry leaves, a soft breeze, an owl |
| `storm_cliffs.ogg` | storm cliffs at night: strong wind, distant thunder, surf far below |
| `flower_meadow.ogg` | a moonlit flower meadow: soft breeze, crickets, gentle insect hum |
