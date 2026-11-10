# Battle portrait ring (undead)

Part of the battle portrait set; overview, geometry, workflow and file names in [battle-portraits.md](battle-portraits.md). Written 2026-11-10 from the owner's battle UI quiz answers.

The ring for undead units, including raised undead (which are summons but get this ring). Undead reuse their creature's portrait. Make it after the gold ring. Result: `assets/frames/portrait-ring-undead.png`.

## Prompt

Attach the finished gold ring (`assets/frames/portrait-ring.png`) for the shape and the undead card frame (`assets/frames/undead.png`) for the metal, and paste:

```
Generate one image: a round frame ring for undead units in the Everkin card game. Its shape, size and socket are exactly those of the attached gold ring; its metal and motif match the attached undead card frame. File name: portrait-ring-undead.png.

FORMAT
- PNG, 1024 x 1024 px square, real transparent background (alpha). The open centre and everything outside the ring are fully transparent. If you cannot make real transparency, fill those areas with flat pure magenta #FF00FF and use no magenta in the ring itself.
- Only the ring: no creature, no scenery, no text, numbers, letters, logos or watermark.

LAYOUT
- A perfect circle centred in the image. Outer diameter about 1000 px; the band is about 70 px wide, so the open window is a clean circle about 860 px across with nothing reaching into it.
- At the bottom centre, one small round empty gem socket about 200 px across overlaps the band (the HP badge). It is a ring with a dark, flat, empty centre, because the game prints a number there.

STYLE
- Instead of warm antique gold, the metal is tarnished, cold pewter-grey gold, matching the attached undead card frame, with a faint sickly green glow in the cracks and the socket rim. Softly lit from the top, with a subtle inner bevel.
- A few small carved bones on the band and a little skull at the top centre. Eerie but storybook gentle (ages 7+): no blood, no gore, nothing grotesque.
- Left-right symmetric (it is also shown mirrored). Straight on and perfectly flat, no perspective, tilt or drop shadow.
- Bold, simple shapes that read at 130 px across. No fine filigree, noise or thin scratchy detail. Dark rather than bright.
```
