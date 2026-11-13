# Asset prompts and provenance

How every generated asset in `assets/` was made, so it can be extended or remade in the same style. The asset files themselves and their formats are listed in [assets/README.md](../../assets/README.md).

| Asset | Prompt (to make more) | Record of the actual run |
|---|---|---|
| Creature art | [creature-art-prompt.md](creature-art-prompt.md); script `tools/art/generate_creatures.py` | originals in `source-art/creatures/`, cleaned by `tools/art/clean_creatures.py` |
| Card frames (18 biomes) | [frames.md](frames.md) | — |
| Undead card frame | [undead-frame.md](undead-frame.md) | — |
| Battle portraits (319 creatures, 9 summons) and their three rings | overview [battle-portraits.md](battle-portraits.md); prompts [portrait-creatures.md](portrait-creatures.md), [portrait-summons.md](portrait-summons.md), [portrait-ring.md](portrait-ring.md), [portrait-ring-summon.md](portrait-ring-summon.md), [portrait-ring-undead.md](portrait-ring-undead.md) | not made yet (2026-11-10) |
| Summon and construct cards (9 in use, 5 unused) | [summons.md](summons.md) | [summons-prompts-used.md](summons-prompts-used.md), [summons-notes.md](summons-notes.md) |
| Skill icons: overview, Move and Skip Turn (made) | [skill-icons.md](skill-icons.md) | [skill-icons-prompts-used.json](skill-icons-prompts-used.json), [skill-icons-notes.md](skill-icons-notes.md); contact sheet: `python3 tools/icons/make_preview.py` |
| Class skill icons (84, **not made**) | [skill-icons-classes.md](skill-icons-classes.md) | — |
| Unique attack icons (319, **not made**) | [skill-icons-attacks.md](skill-icons-attacks.md) | — |
| Status, marker and HUD icons (24 in use; 8 unused kept) | [ui-icons.md](ui-icons.md) | — |
| New status icons (5, **not made**) | [status-icons-new.md](status-icons-new.md) | — |
| Emblem, wordmark, app icon | [brand.md](brand.md) | — |
| Sounds (37 effects, 2 stings, 18 ambient loops; 109 files) | [sounds.md](sounds.md); gas explosion (made 2026-11-13): [sound-gas-explode.md](sound-gas-explode.md) | synthesized by `tools/audio/build_audio.py`, not by an audio model: [sounds-notes.md](sounds-notes.md), [sounds-provenance.md](sounds-provenance.md), measurements in `tools/audio/report/` |
| Biome backgrounds | `assets/backgrounds/README.md`; card backgrounds by `tools/backgrounds/gen_backgrounds.py` | — |

Missing: the battle portraits and rings (2026-11-10), the 403 new skill and attack icons, 5 new status icons (2026-11-13). Fonts are Alegreya and Alegreya Sans, downloaded from Google Fonts into `assets/fonts/` (2026-11-08). The brand PNGs are final; there are no SVG redraws (2026-11-08).
