# New status icons (5)

Status icons for effects of the 28 classes that have no icon yet (skills.md). Not made yet (2026-11-13). They join the set in [ui-icons.md](ui-icons.md) and must look like it: attach two or three existing icons from `assets/icons/ui/` (for example `status-ward.png`, `status-cursed.png`, `status-marked.png`) as style references. The game overlays a letter or short label on each icon, so the images carry no text. Save to `assets/icons/ui/<file>`.

## How to run

Paste one row's subject and fill color, then the style block. One image per request, or all five with "ONE separate image per subject". Check them at 32 × 32 px beside the existing icons.

## Style block (paste after the subject)

```
Make ONE separate small game icon for the subject described above, for the Everkin card game (a night fairytale creature card battler).

FORMAT
- PNG, 256 × 256 px, fully transparent background (real alpha).
- A round badge shape filling the canvas with about 8 px margin. No text, letters or numbers.

STYLE
- One bold symbol inside the badge that reads at 32 × 32 px. Simple silhouette, minimal detail.
- Chunky low-poly faceted style with broad flat facets and soft light from the upper left, matching the game's creature art.
- Badge rim: thin dark-bronze ring, same on every icon. Badge fill: the given color, slightly darker toward the edge.
- Harmful statuses use the given color with a dark, angular feel; helpful ones a soft, rounded feel.
- Storybook tone for ages 7 and up.
```

## Icons

| File | Subject | Fill color |
|---|---|---|
| `status-song.png` | Two music notes (Bard songs) | Magenta |
| `status-glass.png` | A cracked glass shard (Glass Body) | Pale cyan |
| `status-transmuted.png` | Two swirling orbs trading places (Transmute) | Amethyst |
| `status-duel.png` | Two crossed swords in a ring (Single Combat) | Silver |
| `status-delayed.png` | A small hourglass with a weight (Jam, Net Trap: extra Delay on the next skill) | Soot grey |
