# Creature art

Cleaned copies of the originals in `source-art/creatures/` (which are never changed), made by
`python3 tools/art/clean_creatures.py` (needs numpy, scipy and Pillow). Re-run it whenever an original changes.

- **One image per creature**, named `{species}-{nn}.png`, with a two-digit counter for every species (including single images). Filenames contain only the species and number; style labels, classes and attack types are omitted.
- 319 creature images are stored in this folder, one per row of `docs/creature-classes.md`.
- **1024 × 1536 (2:3), RGBA, transparent background.** It is the same size as the card frames.
- The creature is scaled as large as it fits inside a 48 px side margin and below y = 48, centered, with its lowest pixel on the **shared ground line at y = 1456**.
- Stray specks (detached islands under 400 source pixels) are removed. A leftover white background spot on `mantis-sickle` is erased by hand in the script.
- `tools/art/clean_report.json` records, by unit name, the art file, the source file, the specks removed and where the creature sits.
- Portrait (card front) and full-card (card back) crops are the **Front frame** and **Rear frame** columns in the Card details table of `docs/creature-classes.md`.

## Flagged
- `stag-glade`: its source is small, so the copy is upscaled 1.55× and may look soft at large sizes. It is a candidate for re-rendering.
- `mantis-sickle`: upscaled 1.29×, and it had a white leftover spot by its left foot, now erased.
- Creatures 18–27 (`mossback` to `gloom_scorpion`) are wide and low, so on a 2:3 card they fill the width but only about half the height. Their portraits crop to the head and front body.
