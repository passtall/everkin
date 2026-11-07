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

## Statuses and states (game_system §10.5, §10.6)

| File | Subject | Fill color |
|---|---|---|
| `status-blind.png` | A closed eye with a puff of powder | Grey-violet |
| `status-poison.png` | A drop with small bubbles | Toxic green |
| `status-bleeding.png` | A single dripping drop shape (stylised, not gore) | Dark red |
| `status-silenced.png` | A mouth shape crossed by a stitched line | Indigo |
| `status-chilled.png` | A snowflake with a down arrow | Pale blue |
| `status-frozen.png` | A solid ice block | Ice blue |
| `status-stunned.png` | Three little stars circling | Yellow |
| `status-confused.png` | A spiral with a question curl | Pink |
| `status-immobilized.png` | Two feet bound by roots | Brown |
| `status-marked.png` | A target crosshair | Orange-red |
| `status-stealth.png` | A half-faded hooded silhouette | Dark teal |
| `status-airborne.png` | A wing with an up arrow | Sky blue |
| `status-ward.png` | A hexagon shield bubble | Steel blue |
| `status-stone-shell.png` | A shell of cracked stone | Stone grey |
| `status-link.png` | Two linked rings (Life Link, Linked and Shared Fate, Bound Resurrect) | Teal |
| `status-enchanted.png` | A sparkle on a blade (Sorcerer enchantments) | Violet |
| `status-cursed.png` | A small hex sigil (Sorcerer curses) | Dark purple |
| `status-charge.png` | A small shield with a heart (Guardian's Charge) | Gold |

## Ground effects (game_system §12.2)

These show on a spot, under the card. Make them as round badges too; the game also draws an effect on the spot.

| File | Subject | Fill color |
|---|---|---|
| `ground-fire.png` | Flames on cracked ground | Orange |
| `ground-ice.png` | A slick ice patch with a glint | Pale blue |
| `ground-water.png` | A puddle with ripples | Blue |
| `ground-steam.png` | Rising steam curls | White-grey |
| `ground-poison.png` | A bubbling green puddle with spores | Toxic green |

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

Save to `art/icons/ui/<file>`.
