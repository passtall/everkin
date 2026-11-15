# Game assets

Everything the game loads. Godot sees this folder as `res://assets/`.

| Folder | Contents | Format |
|---|---|---|
| `creatures/` | One image per creature, named by the Art file column of `docs/creature-classes.md` (`{species}-{nn}.png`); its Card details table gives the Front frame (card front) and Rear frame (card back) crops. The creature fills the canvas inside a 48 px side margin, centred, its lowest pixel on the shared ground line. | 1024 × 1536 RGBA PNG, ground line y 1456 |
| `summons/` | Summon, construct, totem and trap cards (skills.md): `clay-golem`, `rune-golem`, `animated-blades`, `bolt-turret`, `barricade`, `net-trap`, `healing-totem`, `spirit-link-totem`, `totem-of-return`. Undead reuse their creature's art. | 1024 × 1536 RGBA PNG, ground line y 1456 |
| `portraits/` | Round battle portraits facing right: `<art file>.png` per creature, `summons/<id>.png` per summon card. The creature sits inside a centred 860 px circle; the game draws the disc, masks the portrait to that circle and lays the ring over it. `discs.json` in each folder marks the portraits that need the lighter plum disc. | 1024 × 1024 RGBA PNG |
| `frames/` | Card frames, one per biome (`<biome_id>.png`) plus `undead.png`. Geometry in frame pixels: stat-gem centres (133, 152), (891, 152), (120, 1336), (910, 1336); name plate about x 330–700, y 1300–1375; open window about x 165–860, y 223–1264. Portrait rings `portrait-ring.png`, `portrait-ring-undead.png`, `portrait-ring-summon.png`, each with an open circle of about 860 px centred at (512, 512). | Frames 1024 × 1536, rings 1024 × 1024, RGBA PNG |
| `backgrounds/card/` | One faceted card background per biome. | 1024 × 1536 WebP |
| `backgrounds/battle/` | One photo-real night battle background per biome; crop to 16:9 and scale to 1920 × 1080 on import. | 1672 × 941 PNG |
| `ui/` | Menu background (night fairytale). | PNG, SVG |
| `icons/skills/` | Action bar icons: `action-move`, `action-skip`, and `<skill_id>.png` for every class active and every unique attack. The `attack-<type>` and `<profession>-<branch>` icons serve the current code and are deleted in M5.6. | 512 × 512 RGB PNG, full bleed |
| `icons/ui/` | `status-*` (the game overlays the status letter), `marker-*`, `hud-*`. | 256 × 256 RGBA PNG |
| `brand/` | `emblem.png`, `wordmark.png`, `app-icon.png` (also the Godot project icon). | PNG |
| `fonts/` | Alegreya (headings and names, variable weight, plus italic) and Alegreya Sans (body text and numbers: Regular, Medium, Bold, Italic). The licences `OFL-Alegreya.txt` and `OFL-AlegreyaSans.txt` ship with the game. | TTF |
| `audio/ui/`, `audio/battle/`, `audio/result/`, `audio/ambient/` | Interface sounds; battle sounds in variants `_1` to `_3`; victory and defeat stings; one 60 s seamless loop per biome. | OGG Vorbis 48 kHz; effects mono, ambient stereo |

Biome IDs (file names): `arctic`, `ashlands`, `autumn_woods`, `cathedral`, `coast`, `crystal_caverns`, `desert`, `flower_meadow`, `jungle`, `lagoon`, `mountain`, `mushroom_grotto`, `pine_forest`, `ruins`, `savannah`, `storm_cliffs`, `swamp`, `wet_forest`. The Biome column of `docs/creature-classes.md` uses the readable names (Storm cliffs → `storm_cliffs`).
