# Logo, emblem and app icon

production.md §1.6: a hand-lettered gold storybook wordmark "Everkin" with a small emblem; the app and store icon is that emblem in gold on a plum night sky, readable at small sizes. Generated, then redrawn as a vector. Run the three prompts in order and attach the emblem result to the third.

## 1. Emblem → `assets/brand/emblem.png`

```
Design a small emblem for "Everkin", a night fairytale card game where wild creatures and enchanted objects live side by side. The emblem: a round crest with a stylised tree whose roots and branches form a ring, a crescent moon tucked in the branches, and two tiny creature silhouettes (a fox and a little bird) among the roots. A castle tower is only hinted at in the distance between the branches.

FORMAT: PNG, 1024 × 1024 px, real transparent background. One emblem, centered, about 80% of the canvas. No text, letters or watermark.
STYLE: antique gold metal with soft bevels, a subtle warm glow, and dark plum shadows in the recesses. Bold simple shapes that still read at 48 × 48 px; avoid fine filigree. Storybook and warm, slightly magical, suitable for ages 7 and up.
```

## 2. Wordmark → `assets/brand/wordmark.png`

```
Hand-letter the word "Everkin" as a game logo wordmark. Spell it exactly E-v-e-r-k-i-n, one word, capital E only.

FORMAT: PNG, 2048 × 768 px, real transparent background. The word centered, filling about 85% of the width. No other text.
STYLE: a warm storybook serif with gentle hand-drawn swashes on the E and the k, antique gold with a soft bevel and a thin dark plum outline so it reads on both light and dark night skies. A tiny crescent moon may dot the i. Readable at small sizes; no thin hairlines that disappear when shrunk. Fairytale, not horror, not medieval blackletter.
```

## 3. App icon → `assets/brand/app-icon.png`

Attach the emblem from step 1.

```
Make an app and store icon from the attached emblem.

FORMAT: PNG, 1024 × 1024 px, square, full bleed, no transparency and no rounded corners (the stores round them). No text.
CONTENT: the attached gold emblem, centered, about 70% of the icon, on a deep plum night sky with a soft radial glow behind the emblem and a few tiny stars near the edges. Keep everything important inside the central 80% so platform masks (circle, squircle) never cut it.
STYLE: the emblem stays exactly as attached; only the background is new. Must read clearly at 48 × 48 px.
```

Done: the PNGs in `assets/brand/` are final and large enough for every screen and store; there are no SVG redraws (decided 2026-11-08). The Godot project icon is `assets/brand/app-icon.png`.
