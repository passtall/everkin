# Creature art

Cleaned copies of the originals in `images/kin/` (which are never changed), made by
`python3 tools/art/clean_creatures.py` (needs numpy, scipy and Pillow). Re-run it whenever an original changes.

- **One image per creature**, named `{species}-{nn}.png`, with a two-digit counter for every species (including single images). Filenames contain only the species and number; style labels, classes and attack types are omitted.
- 373 creature images are currently stored in this folder.
- **1024 × 1536 (2:3), RGBA, transparent background.** It is the same size as the card frames.
- The creature is scaled as large as it fits inside a 48 px side margin and below y = 48, centered, with its lowest pixel on the **shared ground line at y = 1456**.
- Stray specks (detached islands under 400 source pixels) are removed. A leftover white background spot on `mantis-sickle` is erased by hand in the script.
- `portraits.json` holds, by creature ID, each creature's **portrait crop** for the card front: a 2:3 rectangle (`x`, `y`, `w`, `h` in these 1024 × 1536 pixels), mostly 640 × 960, placed around the head and upper body. The card back shows the whole image.
- `framing.json` holds, by unit name, the **front** (card front portrait) and **rear** (card back) crops Andreas chose in the creature quiz (2026-10-09): 2:3 rectangles in these 1024 × 1536 pixels. For creatures with a creature ID the front crop is copied into `portraits.json` too.
- `clean_report.json` records, by unit name, the art file, the source file, the specks removed and where the creature sits.

## Flagged
- `stag-glade`: its source is small, so the copy is upscaled 1.55× and may look soft at large sizes. It is a candidate for re-rendering.
- `mantis-sickle`: upscaled 1.29×, and it had a white leftover spot by its left foot, now erased.
- Creatures 18–27 (`mossback` to `gloom_scorpion`) are wide and low, so on a 2:3 card they fill the width but only about half the height. Their portraits crop to the head and front body.
