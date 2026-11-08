# Summon and construct card art

14 cards for things that skills put on the battlefield (skills.md). They use the same card as creatures (ui_spec: 2:3, 1024 × 1536, home frame plus a summon marker), so they are made exactly like creature art. Undead need no art: they reuse the raised creature's art in the undead frame.

## How to use

1. Attach 3–4 creature images from `assets/creatures/` as style references (mix sizes; golems go well with `kiln-01.png`, `sarcophagus-01.png`, `organ-01.png`, `lantern-01.png`).
2. Paste one brief, then the style block below. One image per request, or several briefs with "ONE separate image per brief".
3. Save as `assets/summons/<file>.png`. Clean them with the same rules as creatures (transparent, ground line at y 1456).

## Style block

```
The attached images are style references for the Everkin card game. Match their look exactly: chunky low-poly faceting, simple matte color blocks, soft top-left light, and one small glowing or contrasting accent. Do NOT copy their species, poses or props; use them only for style, proportions and level of detail.

Render the summoned object or creature described above as card art. Make ONE separate image per brief.

FORMAT
- PNG, 1024 × 1536 px portrait (2:3), fully transparent background (real alpha, not a checkerboard or white).
- Exactly one subject per image. No ground, shadow, scenery, frame, text, logo, watermark or signature.
- The whole subject is inside the image with at least 48 px of empty space on every side, scaled as large as it fits, centered, its lowest point about 80 px above the bottom edge.

STYLE
- Low-poly 3D game model: broad flat polygon facets, matte colors, gentle facet shading. No fine texture.
- Soft directional light from the upper left, moderate contrast. No rim glow, bloom, lens effects or outlines.
- Crisp clean silhouette, no halo, fringe or stray pixels.
- Natural, slightly muted colors that read on a dark plum background, plus ONE brighter accent. Glowing parts may be brighter.
- Three-quarter front view from about chest height. If it has a face, the face is visible and mostly toward the viewer.

TONE
- Storybook and friendly-capable, suitable for ages 7 and up. No blood, gore or horror.
```

## Briefs

| File | Skill (profession) | Brief |
|---|---|---|
| `clay-golem.png` | Clay Golem (Summoner) | [hulking] A plain, sturdy golem of reddish fired clay, thick round arms and stubby legs, finger marks pressed into its body, two warm amber eye holes. Simple and solid. |
| `rune-golem.png` | Rune Golem (Summoner) | [tall] A slate-grey stone golem with glowing blue runes carved over its chest and arms, a narrow head with one rune for a face. It looks like it hushes magic. |
| `clockwork-golem.png` | Clockwork Golem (Summoner) | [medium, upright] A brass and copper clockwork golem with visible turning gears in its chest, a wind-up key in its back and round lens eyes glowing soft green. |
| `bell-golem.png` | Bell Golem (Summoner) | [round, wide] A golem whose body is a big bronze temple bell with short stone legs and arms, a clapper hanging inside like a heart, a calm face engraved on the bell, faint golden sound rings. |
| `animated-blades.png` | Animate Blades (Summoner) | [floating] Three short enchanted swords hovering in a loose fan, points forward, linked by a faint violet glow, with a ribbon on one hilt. No hands holding them. |
| `animated-mirror.png` | Animate Mirror (Summoner) | [floating] A tall oval standing mirror in a dark wooden frame with small clawed feet, its glass showing a swirl of pale silver light instead of a reflection, and two little eyes in the frame's crest. |
| `bolt-turret.png` | Bolt Turret (Tinker) | [small, mechanical] A small wooden and brass crossbow turret on a three-legged stand, a loaded bolt, a crank handle and a round sighting lens glowing orange. |
| `flame-turret.png` | Flame Turret (Tinker) | [small, mechanical] A squat iron turret with a stubby flared nozzle, a pressure gauge and copper tank, a little pilot flame at the nozzle tip as its accent. |
| `barricade.png` | Barricade (Tinker) | [wide, low] A sturdy field barricade of crossed timber planks, an iron-banded shield board in front and a few sandbags, with one small brass plate as the accent. No creature. |
| `healing-totem.png` | Healing Totem (Primalist) | [tall, thin] A carved wooden totem pole with a gentle smiling top face, leaves and green feathers tied around it, and a soft green glowing crystal set in the middle. |
| `spirit-link-totem.png` | Spirit Link Totem (Primalist) | [tall, thin] A pale stone totem with two faces back to back joined by a carved chain, a turquoise spirit wisp circling it. |
| `totem-of-return.png` | Totem of Return (Primalist) | [tall, thin] A weathered wooden totem shaped like an owl with folded wings, an hourglass carved in its chest and a warm white glow in its eyes. |
| `net-trap.png` | Net Trap (Hunter) | [wide, low] A coiled hunter's net trap: a rope net piled over a bent sapling spring, wooden stakes and a small bell on a cord as the accent. |
| `hunter-decoy.png` | Decoy (Hunter) | [medium] A scruffy decoy dummy of straw and bark wrapped in a hooded green cloak, stick arms, a carved wooden face with button eyes, propped on a stake. |
