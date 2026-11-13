# Game assets

Everything the game loads. Godot sees this folder as `res://assets/`. Originals that are never changed live in `source-art/`; prompts and provenance are in `docs/assets/`; the scripts that build assets are in `tools/`.

| Folder | Contents | Format |
|---|---|---|
| `creatures/` | One image per creature, named by the Art file column of `docs/creature-classes.md`. Crops are in that file's Card details table. See its README. | 1024 × 1536 RGBA PNG, ground line y 1456 |
| `summons/` | 9 summon, construct, totem and trap cards (skills.md): `clay-golem`, `rune-golem`, `animated-blades`, `bolt-turret`, `barricade`, `net-trap`, `healing-totem`, `spirit-link-totem`, `totem-of-return`. Undead reuse the creature's art. Unused, kept from removed skills: `clockwork-golem`, `bell-golem`, `animated-mirror`, `flame-turret`, `hunter-decoy`. | 1024 × 1536 RGBA PNG, ground line y 1456 |
| `frames/` | Card frames: one per biome (`<biome_id>.png`) plus `undead.png`. Geometry in `backgrounds/README.md`. Battle portrait rings `portrait-ring.png`, `portrait-ring-undead.png` and `portrait-ring-summon.png` (not made yet; geometry in docs/assets/battle-portraits.md). | 1024 × 1536 RGBA PNG; rings 1024 × 1024 RGBA PNG |
| `portraits/` | Round battle portraits facing right (not made yet, 2026-11-10): `<art file>.png` per creature, `summons/<id>.png` for the 9 summons in use. Prompt and geometry: docs/assets/battle-portraits.md. | 1024 × 1024 RGBA PNG, creature inside a centred 860 px circle |
| `backgrounds/card/` | One card background per biome. | 1024 × 1536 WebP |
| `backgrounds/battle/` | One photo-real night battle background per biome. | 1672 × 941 PNG, scaled up to 1920 × 1080 on import |
| `ui/` | Menu background (night fairytale), PNG and SVG. | |
| `icons/skills/` | Action bar icons: `action-move`, `action-skip`, and one `<skill_id>.png` per class active (84) and unique attack (319); prompts in docs/assets/skill-icons-classes.md and skill-icons-attacks.md. Not made yet (2026-11-13); until then the old `attack-<type>` and `<profession>-<branch>` icons stand in. | 512 × 512 RGB PNG, full bleed |
| `icons/ui/` | `status-*`, `marker-*`, `hud-*`. The game overlays the status letter. Not made yet: `status-song`, `status-glass`, `status-transmuted`, `status-duel`, `status-delayed` (docs/assets/status-icons-new.md). Unused, kept: `ground-*` (ground effects are drawn by shaders, ui_spec) and `status-chilled`, `status-stone-shell`, `status-charge` (statuses that no longer exist). | 256 × 256 RGBA PNG |
| `brand/` | `emblem.png`, `wordmark.png`, `app-icon.png` (also the Godot project icon). No vector versions. | PNG |
| `fonts/` | Alegreya (headings and names, variable weight, plus italic) and Alegreya Sans (body text and numbers: Regular, Medium, Bold, Italic), from Google Fonts. Their licences `OFL-Alegreya.txt` and `OFL-AlegreyaSans.txt` ship with the game. | TTF |
| `audio/ui/`, `audio/battle/`, `audio/result/`, `audio/ambient/` | Interface sounds; battle sounds in variants `_1`–`_3`; victory and defeat stings; one 60 s seamless loop per biome. | OGG Vorbis 48 kHz; effects mono, ambient stereo |

Biome IDs (file names): `arctic`, `ashlands`, `autumn_woods`, `cathedral`, `coast`, `crystal_caverns`, `desert`, `flower_meadow`, `jungle`, `lagoon`, `mountain`, `mushroom_grotto`, `pine_forest`, `ruins`, `savannah`, `storm_cliffs`, `swamp`, `wet_forest`. The Biome column of `docs/creature-classes.md` uses the readable names (Storm cliffs → `storm_cliffs`).
