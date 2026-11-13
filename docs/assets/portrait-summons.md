# Summon battle portraits (14)

Part of the battle portrait set; overview, geometry, workflow and file names in [battle-portraits.md](battle-portraits.md). Written 2026-11-10 from the owner's battle UI quiz answers.

One image per summon card: `clay-golem`, `rune-golem`, `animated-blades`, `bolt-turret`, `barricade`, `net-trap`, `healing-totem`, `spirit-link-totem`, `totem-of-return` (the 9 in use; the unused summon cards get no portrait). Result: `source-art/portraits/summons/<id>.png`, cleaned copy in `assets/portraits/summons/<id>.png`. Raised undead are not here: they use their creature's portrait in the undead ring.

## Prompt

Attach the summon's card art (`assets/summons/<id>.png`) and paste:

```
The attached image is a summoned creature or object from the Everkin card game. Make a new picture of exactly this summon for a small round battle token: the same shape, colours, materials, markings, props and glow, in the same art style. Change only the direction it faces.

FORMAT
- PNG, 1024 x 1024 px square, fully transparent background (real alpha, not a checkerboard or white).
- Exactly one summon. No ground, shadow, scenery, frame, ring, text, logo, watermark or signature.
- The whole summon (every part, prop and glow) fits inside a centred circle about 860 px across. Leave the corners empty.
- Scale the summon as large as it fits in that circle, centred. Tall, wide and small summons alike fill the circle the same way. Never crop.

POSE AND VIEW
- Keep the pose of the attached image, but turn the summon to a three-quarter view facing RIGHT: its front points toward the right edge of the image, as if facing an opponent on the right. A summon with a face keeps both eyes readable; an object without one (barricade, net trap, totems) shows its front side angled to the right.
- Seen from about chest height.

STYLE
- Low-poly 3D game model exactly like the attached image: broad flat polygon facets, matte colours, gentle facet shading. No fine texture.
- Soft light from the top front, so both sides of the summon are lit about evenly (the image is also shown mirrored). No rim glow, bloom, lens effects or outlines.
- Crisp clean silhouette, no halo, fringe or stray pixels. It must read when shrunk to 130 px across.
- No text, letters, numbers or symbols that would look wrong mirrored.

TONE
- Friendly but capable, suitable for ages 7 and up. No blood, wounds, gore or horror.
```

## Checks

As for creatures ([portrait-creatures.md](portrait-creatures.md), Checks): same summon as its card, faces right, reads at 130 px, centred inside the 860 px circle.
