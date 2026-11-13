# Creature battle portraits (319)

Part of the battle portrait set; overview, geometry, workflow and file names in [battle-portraits.md](battle-portraits.md). Written 2026-11-10 from the owner's battle UI quiz answers.

One image per creature, facing right; the enemy side shows it mirrored. Result: `source-art/portraits/<art file>.png` (original, never changed), cleaned copy in `assets/portraits/<art file>.png`.

## Prompt

Attach the creature's card art (`assets/creatures/<art file>.png`) and paste:

```
The attached image is a creature from the Everkin card game. Make a new picture of exactly this creature for a small round battle token: the same species, body shape, colours, markings, gear, props and glowing accent, in the same art style. Change only the direction it faces.

FORMAT
- PNG, 1024 x 1024 px square, fully transparent background (real alpha, not a checkerboard or white).
- Exactly one creature. No ground, shadow, scenery, frame, ring, text, logo, watermark or signature.
- The whole creature (ears, horns, antennae, tails, wings, feet, props) fits inside a centred circle about 860 px across. Leave the corners empty.
- Scale the creature as large as it fits in that circle, centred. Tall, wide and tiny creatures alike fill the circle the same way: big creatures are scaled down, tiny ones scaled up. Never crop.

POSE AND VIEW
- Keep the pose and expression of the attached image, but turn the creature to a three-quarter view facing RIGHT: its body and face point toward the right edge of the image, as if looking at an opponent on the right. Both eyes and the face stay readable.
- Seen from about chest height.

STYLE
- Low-poly 3D game model exactly like the attached image: broad flat polygon facets, matte colours, gentle facet shading. No fur strands, individual scales or fine texture.
- Soft light from the top front, so both sides of the creature are lit about evenly (the image is also shown mirrored). No rim glow, bloom, lens effects or outlines.
- Crisp clean silhouette, no halo, fringe or stray pixels. It must read when shrunk to 130 px across.
- No text, letters, numbers or symbols that would look wrong mirrored.

TONE
- Friendly but capable, suitable for ages 7 and up. No blood, wounds, gore or horror.
```

## Checks

- Clean like the card art (`tools/art/clean_creatures.py` as a model): real alpha, no fringe, the creature's extent inside the 860 px circle, centred. Do not apply the card's ground line; portraits are centred.
- Check every portrait against its card at 130 px: same creature, faces right, the prop that shows its classes still visible.
- **Disc colour.** The game draws a plain dark plum disc (the UI's panel colour) behind each portrait. The cleaning step measures each portrait's edge brightness once and marks those too dark for it; they get a lighter plum disc. The result is stored with the portraits (for example `assets/portraits/discs.json`), not chosen by hand.
