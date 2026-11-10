# Spec: battle UI, left/right board, round portraits, visible ground effects

**Status:** FINAL 2026-11-10. The owner's request (section 0) and the quiz answers (rounds 1 to 4) (https://claude.ai/artifact/EcyT2Qns9duAcZpDbMmmJq) are in. The adopted rules are also written into [docs/ui_spec.md](docs/ui_spec.md) (Battle screen), which wins if the two ever differ. This file is the work order: what changes and where.

## 0. Owner's request (2026-11-10)

1. The board changes from bottom/top to **left/right**: the player's team on the left, the enemy on the right. Rows become vertical. The view switcher stays for now and switches between the full view (both sides) and one side only.
2. Every unit gets a separate, smaller **round battle portrait**. It needs new art with the creature **facing right**; the enemy side shows it mirrored.
3. **Ground effects** (game_system.md §12.2) become actual visual effects on the board.

No rule changes. Rows, coordinates, distances, adjacency, interception and ground effects work exactly as in game_system.md; only how they are drawn changes.

## 1. Left/right board

- **Sides.** Player on the left, enemy on the right. From the left edge inward: player Rear, Middle, Front, the battle line, enemy Front, Middle, Rear. The two Front columns face each other in the middle, with the slightly larger gap at the (now vertical) battle line.
- **Rows are columns.** Each row is a vertical column. A row's order (its coordinates, `Formation.CenterOf`) runs **top to bottom** on both sides (lowest coordinate at the top), and columns stay centred vertically. Move gaps, the forced movement preview, 50% insertion points and delayed `direct` slot markers use the same mapping.
- **Facing.** Player portraits face right; enemy portraits are the same image mirrored, facing left.
- **Portrait size** (L2). All portraits on the board share one size: the largest that lets the fullest column fit the board height with 10 px gaps, capped at about **200 px** (both-sides view) and **300 px** (one-side view) at the 1920 × 1080 base. Six in a column give about 136 px (above the 110 px touch minimum). When the fullest column changes, all portraits resize smoothly (instantly with reduced motion).
- **View switcher.** Unchanged in function: the view button top right switches between both sides and one side; the automatic choice below about 7 inches and the remembered manual choice stay. In the one-side view the side button shows a **left/right arrow**, and the flyover scrolls **horizontally** across the battle line (a cut with reduced motion). A skill that targets the enemy still switches over by itself and back after the action.
- **Lines** (B1): faint vertical lines of light between the columns (rows) of each side, and a slightly thicker one down the battle line.
- **Battle start** (B3): your team slides in from the left edge, the enemy from the right.
- **Momentum meter** (L1): unchanged, a vertical bar at the right edge, about 70% of the board height, player's end at the bottom.
- **Timeline** stays a vertical list on the left; its entries, the battle log lines and the target preview use the round portrait (L6).
- **Team editor** (L5): unchanged, rows of cards.
- **Local two-player:** the board flips so the acting player's side is always on the **left**.
- **Unchanged:** battle menu button top left, action bar at the bottom, inspection panel with full card front and back, floating numbers, controls.

## 2. Round battle portraits

- **Look.** A round portrait of the **whole creature**, three-quarter view facing right, in the same pose and expression as its card, on a **plain disc** (P3: dark plum; a lighter plum when the creature's edge is too dark for it, measured once from the art when it is cleaned and stored with the portrait), inside **one shared dark gold ring**. **Undead** use the undead ring with their creature's portrait; **summons** have their own portraits in a **silver summon ring** (P2); a raised undead, though a summon, gets the undead ring (X2). There are no marker badges. Hovering a summon highlights its summoner.
- **On the portrait: HP only** (A10, P4). A small round **HP badge** at the bottom of the ring with the current HP number (at least 22 px at base). No HP arc or bar. No Power, Speed or Cost, **no status icons**, no side colour (side = position and facing). Statuses and changed values are seen on the full card in the inspection panel (L3, A9, A10).
- **Stealth** (S1): semi-transparent with a faint shimmer; it turns solid again with a short flash when stealth breaks.
- **Airborne** (S2): the portrait floats a little higher and gently bobs, with a soft updraft (faint rising air wisps) under it; ground effects under it stay visible.
- **Target highlights** (S3): a glow on the ring in today's colours: gold legal, white selected, light hover; illegal targets are dimmed.
- **Hover and details** (X1): hovering adds no tooltip (while aiming it drives the target preview, as now). Name, classes and every detail stay on the full card front and back in the inspection panel, as now; right-click, long-press or I opens it.
- **KO** (L4): the portrait cracks (the same thin light crack line), shakes once, turns grey with its ring dimmed and sinks slightly; it stays as a body. No flip, since a portrait has no back. Revive restores colour and removes the crack.
- **Hit:** the portrait shakes briefly, as cards did.
- **File size** (X3): portraits are 1024 × 1024 on every platform for now; all images are scaled for phones before release (M10.3).
- **Art:** prompts, geometry and file layout in [docs/assets/battle-portraits.md](docs/assets/battle-portraits.md): 319 creature portraits and 14 summon portraits (1024 × 1024, creature inside a centred 860 px circle), `portrait-ring.png`, `portrait-ring-undead.png` and `portrait-ring-summon.png`. **The portraits come first** (W2: no stand-in): the new board is built once the art exists. **Who makes them** (W1): the implementing session, on the owner's PC, extends `tools/art/generate_creatures.py` for portraits and rings (it needs the image API key there, as for the card art), makes 4 test portraits of different shapes for the owner to check, then the batch, and shows the owner a contact sheet before cleaning.
- **Active unit** (P1): a steady gold glow behind the disc and the portrait shown 10% larger; legal targets glow on the ring, so both can show at once.
- **Inspection** (P5): the full card pair grows out of the portrait toward the screen centre.

## 3. Ground effects as visual effects

**Today** the core places and tracks effects (`GroundField`, `GroundEffectPlaced`), but `BattleBoard` draws nothing (it only waits 0.15 s).

- **Ground layer** between the battle background and the portraits.
- **Pools** (G2, G10). Each spot with an effect shows a round pool about 1.3× the portrait size, centred where a unit on that coordinate stands. It stays on its coordinate when the column recentres and looks the same on an empty spot. **Same effects on touching spots merge** into one smooth patch (touching as in §12.2: side by side in a column, or overlapping across neighbouring columns of one side). Suggested technique: draw each effect type's pools as soft discs into a `SubViewport` and threshold them in the effect's shader (metaball-style), so touching pools melt together.
- **Drawing** (G1, G3). Godot shaders (`ShaderMaterial`) for the surfaces plus a few `CPUParticles2D` (Compatibility renderer) that rise in front of the portrait's lower edge. No new art.
- **Looks** (G13), tuned during implementation: burning ground a flickering orange pool with rising embers; ice pale cracked frost with a moving glint; water a dark blue pool with slow ripples; steam white mist puffs rising; poison ground a green pool with bubbles and drifting spores.
- **Placing** (G7): the pool spreads out from where the skill lands, about 0.4 s. **Ending:** it fades out (short).
- **Two effects on one spot** (B2), which the rules allow (e.g. poison ground on ice or water): both pools are drawn on top of each other, half transparent, with both sets of particles.
- **Combos** (G8) have their own short transitions: water freezes over into ice; steam bursts up and leaves water; poison flashes on explosion, then fire spreads.
- **Damage** (G9): only the normal floating number.
- **Not shown** (G4, G5, G6, G11): no effect icon, no hover or tap information, no fading as the time runs out, no aiming preview. The timeline's end marker shows when an effect ends.
- **Reduced motion** (G12): ground effects play as normal (an exception to the reduced-motion rule).

## 4. What changes where

| Area | Change |
|---|---|
| scripts/App/BattleBoard.cs | `RowY` becomes a column X; coordinates map to Y; portrait sizing per section 1; one-side view and flyover horizontal; side button arrows left/right; gap markers and previews in the new mapping; new ground layer driven by `GroundEffectPlaced` and the effects' end and combo events (add core events if any are missing) |
| scripts/App/CardView.cs (or a new PortraitView) | Round portrait: disc, masked art, mirroring, ring (normal / undead / summon), HP badge, KO crack and grey, stealth, airborne, active-unit glow, target highlights on the ring |
| Timeline, battle log, target preview | Use the portrait |
| PlayScreen / local two-player (M6.1) | Acting side on the left |
| assets/ | `portraits/`, the three rings, shaders and particle materials for ground effects |
| docs/implementation-plan.md | New task **M5.5** (W3), done before M6.1: portraits and rings first, then the board |
| docs | Done: ui_spec.md, assets/README.md, docs/assets/README.md, docs/assets/battle-portraits.md, docs/mockups/README.md |

## 5. Checks

- Every unit, KO body and summon sits in the column and at the height its row and coordinate give; the forced movement preview, gap markers and delayed `direct` markers match.
- Player portraits face right, enemy portraits face left, in both views and after a local two-player flip.
- All portraits share one size, set by the fullest column within the caps; adding a sixth unit to a column shrinks every portrait.
- The one-side view switches with the left/right button and by itself when a skill targets the enemy; reduced motion cuts instead of flying.
- A ground pool stays on its coordinate when units move or the column recentres, shows on an empty spot, merges with the same effect on touching spots, and matches `GroundField` after every event (combos and expiry included).
- Rings, discs and ground pools never hide HP, KO or target highlighting (ui_spec Interaction checks).
