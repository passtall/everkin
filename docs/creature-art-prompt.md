# Creature art prompt (ChatGPT)

The template for new creature art (2026-10-09). Write the creature description first, in your own words, then paste this text after it. Each creature gets one image. After adding the files to `images/kin/` as `<n>_f.png`, run `tools/art/clean_creatures.py` to make the cleaned copies and set a portrait crop in `art/creatures/portraits.json`.

```
Render the creature(s) described above as card art for "Everkin", a fantasy creature card game. Make ONE separate image per creature, and do not combine creatures into one image.

FORMAT
- PNG, 1024 × 1536 px portrait (2:3), with a fully transparent background (real alpha, not a checkerboard or white).
- Show exactly one creature per image, with no ground, shadow, scenery, frame, text, logo, watermark or signature.
- Show the whole creature with every part inside the image (ears, horns, antennae, tail tips, wings, feet), leaving at least 48 px of empty space on all sides.
- Scale the creature as large as it fits within those margins.
- The lowest point (feet or base) sits near the bottom, about 80 px above the lower edge.

STYLE (identical for every creature)
- A simple low-poly 3D game model with broad, clearly visible flat polygon facets and matte color blocks.
- No realistic fur strands, scales, feathers or fine texture; detail comes from the facets and color only.
- Soft directional light from the upper left, with moderate contrast and gentle shading on the facets. No rim glow, bloom, lens effects or outlines.
- A clean silhouette with crisp edges, and no halo, fringe or stray pixels around the creature.
- Colors are natural and slightly muted, so creatures read well on a dark plum background. Glowing parts (embers, crystals, magic) may be brighter.

POSE AND VIEW
- A three-quarter front view, turned slightly to one side, from about chest height.
- An alert, natural stance with believable anatomy, all limbs correctly attached and the joints bending the right way.
- The head and face are clearly visible and facing mostly toward the viewer.
- Nothing covers the face.
- The creature carries no clothing, armor, saddle or equipment unless the description says so.

TONE
- Friendly but capable, suitable for ages 7 and up: no blood, wounds, gore or horror.

```
