# Creature art

Cleaned copies of the originals in `images/kin/` (which are never changed), made by
`python3 tools/art/clean_creatures.py` (needs numpy, scipy and Pillow). Re-run it whenever an original changes.

- **One image per creature**, named `{attackType}_{profession1}_{profession2}_{unitName}.png` (one profession: `{attackType}_{profession}_{unitName}.png`). Lowercase, underscores between parts, hyphens inside a part (`black-mage`, `glade-stag`). The names, classes and source files are listed in `docs/creature-classes.md`, which the script reads.
- 223 creatures: the 53 with a creature ID in docs/creatures.md (sources `images/kin/<n>_f.png`) and 170 added on 2026-10-09 (sources `images/kin/<unitName>.png`), which have no stats or portrait crop yet.
- **1024 × 1536 (2:3), RGBA, transparent background.** It is the same size as the card frames.
- The creature is scaled as large as it fits inside a 48 px side margin and below y = 48, centered, with its lowest pixel on the **shared ground line at y = 1456**.
- Stray specks (detached islands under 400 source pixels) are removed. A leftover white background spot on `sickle-mantis` is erased by hand in the script.
- `portraits.json` holds, by creature ID, each creature's **portrait crop** for the card front: a 2:3 rectangle (`x`, `y`, `w`, `h` in these 1024 × 1536 pixels), mostly 640 × 960, placed around the head and upper body. The card back shows the whole image.
- `clean_report.json` records, by unit name, the art file, the source file, the specks removed and where the creature sits.

## Flagged
- `glade-stag`: its source is small, so the copy is upscaled 1.55× and may look soft at large sizes. It is a candidate for re-rendering.
- `sickle-mantis`: upscaled 1.29×, and it had a white leftover spot by its left foot, now erased.
- Creatures 18–27 (`mossback` to `gloom_scorpion`) are wide and low, so on a 2:3 card they fill the width but only about half the height. Their portraits crop to the head and front body.
