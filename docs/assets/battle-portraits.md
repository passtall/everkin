# Battle portraits: art prompts

Written 2026-11-10 from the owner's answers on the battle UI quiz (https://claude.ai/artifact/EcyT2Qns9duAcZpDbMmmJq; log `battle-ui-decisions-2.md` in the project files). Where the portraits are used: [ui_spec.md](../ui_spec.md), Battle screen, Battle portraits; the change spec is [spec.md](../../spec.md).

Every creature (319 in `assets/creatures/`) and every summon card (14 in `assets/summons/`) gets one round battle portrait, facing **right**. The enemy side shows the same image mirrored, so there is no left-facing version. Undead reuse their creature's portrait inside the undead ring. The card art is not changed; portraits are new, separate files.

## What gets made

| Asset | Count | Source file | Result |
|---|---|---|---|
| Creature portrait | 319 | the creature's card art, `assets/creatures/<art file>.png`, attached as identity reference | `source-art/portraits/<art file>.png` (original, never changed), cleaned copy in `assets/portraits/<art file>.png` |
| Summon portrait | 14 | `assets/summons/<id>.png` attached | `source-art/portraits/summons/<id>.png`, `assets/portraits/summons/<id>.png` |
| Ring | 1 | — | `assets/frames/portrait-ring.png` |
| Undead ring | 1 | `assets/frames/undead.png` attached as style reference | `assets/frames/portrait-ring-undead.png` |

Run one request per image. Test with three or four creatures of different shapes (a tall one, a wide one, a tiny one, a flier) before the batch. `tools/art/generate_creatures.py` can be extended for this (reference image = the card art, size 1024 × 1024).

## Geometry (shared by portraits and rings)

All files are 1024 × 1024. The ring's open window is a circle of about **860 px diameter centred at (512, 512)**. The game draws a plain disc in that window, puts the portrait on top, masks it to the circle and lays the ring over it. So a portrait must keep the whole creature inside that 860 px circle; anything outside it is cut off.

## Creature portrait prompt

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

**Summons:** the same prompt with "a creature" replaced by "a summoned creature or object" (golems, blades, mirror, turrets, barricade, totems, net trap, decoy). An object without a face (barricade, net trap, totems) is shown at a three-quarter angle with its front to the right.

## Ring prompt

Attach two of the biome card frames (`assets/frames/<biome>.png`) as metal reference and paste:

```
Generate one image: a round frame ring for battle tokens in the Everkin card game. It must match the metal of the attached card frames. File name: portrait-ring.png.

FORMAT
- PNG, 1024 x 1024 px square, real transparent background (alpha). The open centre and everything outside the ring are fully transparent. If you cannot make real transparency, fill those areas with flat pure magenta #FF00FF and use no magenta in the ring itself.
- Only the ring: no creature, no scenery, no text, numbers, letters, logos or watermark.

LAYOUT
- A perfect circle centred in the image. Outer diameter about 1000 px; the band is about 70 px wide, so the open window is a clean circle about 860 px across with nothing reaching into it.
- A thin, dark, recessed groove runs all the way around the face of the band, centred on it, about 18 px wide. Keep it plain and dark: the game draws a coloured health bar inside it.
- At the bottom centre, one round empty gem socket about 200 px across overlaps the band. It is a ring with a dark, flat, empty centre, because the game prints a number there.

STYLE
- The same dark antique gold as the attached card frames, polished premium fantasy look, softly lit from the top, with a subtle inner bevel. No biome motif: one plain shared ring with only a little simple ornament on the band.
- Left-right symmetric (it is also shown mirrored). Straight on and perfectly flat, no perspective, tilt or drop shadow.
- Bold, simple shapes that read at 130 px across. No fine filigree, noise or thin scratchy detail. Dark rather than bright.
```

**Undead ring** (`portrait-ring-undead.png`): attach `assets/frames/undead.png` instead and use the same prompt with the STYLE block's first two lines replaced by: "Instead of warm antique gold, the metal is tarnished, cold pewter-grey gold, matching the attached undead card frame, with a faint sickly green glow in the cracks and the socket rim. A few small carved bones on the band and a little skull at the top centre; eerie but storybook gentle, no gore."

## Cleaning and checks

- Clean like the card art (`tools/art/clean_creatures.py` as a model): real alpha, no fringe, the creature's extent inside the 860 px circle, centred. Do not apply the card's ground line; portraits are centred.
- Check every portrait against its card at 130 px: same creature, faces right, profession prop still visible.
- **Disc colour.** The game draws a plain dark plum disc (the UI's panel colour) behind each portrait. A creature that is too dark to stand out on it gets a lighter plain disc colour of its own; the list goes in `assets/portraits/README.md` with the cleaning step (owner's note: plain colour with good contrast).
