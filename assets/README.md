# Game assets

Everything the game loads. Godot sees this folder as `res://assets/`. Originals that are never changed live in `source-art/`; prompts and provenance are in `docs/assets/`; the scripts that build assets are in `tools/`.

| Folder | Contents | Format |
|---|---|---|
| `creatures/` | One image per creature, named by the Art file column of `docs/creature-classes.md`. Crops are in that file's Card details table. See its README. | 1024 × 1536 RGBA PNG, ground line y 1456 |
| `summons/` | 14 summon, construct, totem and trap cards (skills.md): `clay-golem`, `rune-golem`, `clockwork-golem`, `bell-golem`, `animated-blades`, `animated-mirror`, `bolt-turret`, `flame-turret`, `barricade`, `healing-totem`, `spirit-link-totem`, `totem-of-return`, `net-trap`, `hunter-decoy`. Undead reuse the creature's art. | 1024 × 1536 RGBA PNG, ground line y 1456 |
| `frames/` | Card frames: one per biome (`<biome_id>.png`) plus `undead.png`. Geometry in `backgrounds/README.md`. | 1024 × 1536 RGBA PNG |
| `backgrounds/card/` | One card background per biome. | 1024 × 1536 WebP |
| `backgrounds/battle/` | One photo-real night battle background per biome. | 1672 × 941 PNG, cropped to 16:9 at build |
| `ui/` | Menu background (night fairytale), PNG and SVG. | |
| `icons/skills/` | Action bar icons: `attack-<type>`, `action-move`, `action-skip`, `<profession>-<branch>`. A skill uses its branch icon unless `<skill_id>.png` exists. | 512 × 512 RGB PNG, full bleed |
| `icons/ui/` | `status-*`, `ground-*`, `marker-*`, `hud-*`. The game overlays the status letter. | 256 × 256 RGBA PNG |
| `brand/` | `emblem.png`, `wordmark.png`, `app-icon.png` (also the Godot project icon). | PNG |
| `audio/ui/`, `audio/battle/`, `audio/result/`, `audio/ambient/` | Interface sounds; battle sounds in variants `_1`–`_3`; victory and defeat stings; one 60 s seamless loop per biome. | OGG Vorbis 48 kHz; effects mono, ambient stereo |

Biome IDs (file names): `arctic`, `ashlands`, `autumn_woods`, `cathedral`, `coast`, `crystal_caverns`, `desert`, `flower_meadow`, `jungle`, `lagoon`, `mountain`, `mushroom_grotto`, `pine_forest`, `ruins`, `savannah`, `storm_cliffs`, `swamp`, `wet_forest`. The Biome column of `docs/creature-classes.md` uses the readable names (Storm cliffs → `storm_cliffs`).
