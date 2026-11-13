# Status, ground effect and HUD icons

ui_spec requires status icons that read without color: the **game overlays the letter or short label** (for example "P" for Poison) on the icon, so the generated images carry no text. Final icons replace the in-engine placeholders.

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

## Statuses and states (game_system §10.5, §10.6; helpful and harmful effects of skills.md)

| File | Subject | Fill color |
|---|---|---|
| `status-blind.png` | A closed eye with a puff of powder | Grey-violet |
| `status-poison.png` | A drop with small bubbles | Toxic green |
| `status-bleeding.png` | A single dripping drop shape (stylised, not gore) | Dark red |
| `status-silenced.png` | A mouth shape crossed by a stitched line | Indigo |
| `status-frozen.png` | A solid ice block | Ice blue |
| `status-stunned.png` | Three little stars circling | Yellow |
| `status-confused.png` | A spiral with a question curl | Pink |
| `status-immobilized.png` | Two feet bound by roots | Brown |
| `status-marked.png` | A target crosshair | Orange-red |
| `status-stealth.png` | A half-faded hooded silhouette | Dark teal |
| `status-airborne.png` | A wing with an up arrow | Sky blue |
| `status-ward.png` | A hexagon shield bubble | Steel blue |
| `status-link.png` | Two linked rings (Bound Resurrect's shared HP pool) | Teal |
| `status-enchanted.png` | A sparkle on a blade (Enchanter enchantments, Press the Advantage) | Violet |
| `status-cursed.png` | A small hex sigil (Hexer curses) | Dark purple |
| `status-song.png` | Two music notes (Bard songs) | Magenta |
| `status-glass.png` | A cracked glass shard (Glass Body) | Pale cyan |
| `status-transmuted.png` | Two swirling orbs trading places (Transmute) | Amethyst |
| `status-duel.png` | Two crossed swords in a ring (Single Combat) | Silver |
| `status-delayed.png` | A small hourglass with a weight (Jam, Net Trap: extra Delay on the next skill) | Soot grey |

## Markers and HUD

| File | Subject | Fill color |
|---|---|---|
| `marker-summon.png` | A small summoning circle (marks summon cards) | Purple |
| `marker-undead.png` | A pale spirit flame (marks undead) | Sickly green |
| `marker-boss.png` | A small crown (boss encounter node) | Gold |
| `hud-coin.png` | A gold coin with a star | Gold, no badge rim: the coin is the shape |
| `hud-menu.png` | Three horizontal bars | Cream symbol, plum fill |
| `hud-view-both.png` | Two stacked card rows | Cream symbol, plum fill |
| `hud-view-side.png` | One card row with an up/down arrow | Cream symbol, plum fill |
| `hud-emote.png` | A speech bubble | Cream symbol, plum fill |
| `hud-offline.png` | A small broken link | Grey |

## Unused (kept)

These were made but are no longer needed (2026-11-13): `ground-fire`, `ground-ice`, `ground-water`, `ground-steam`, `ground-poison` (ground effects are drawn by shaders with no icon, ui_spec Battle screen) and `status-chilled`, `status-stone-shell`, `status-charge` (statuses that no longer exist). New: `status-song`, `status-glass`, `status-transmuted`, `status-duel`, `status-delayed` are not made yet.

Save to `assets/icons/ui/<file>`.
