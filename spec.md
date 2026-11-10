# Spec: battle UI, left/right board, round portraits, visible ground effects

**Status:** FINAL 2026-11-10. The owner's request (section 0) and his quiz answers (https://claude.ai/artifact/EcyT2Qns9duAcZpDbMmmJq) are in. The adopted rules are also written into [docs/ui_spec.md](docs/ui_spec.md) (Battle screen), which wins if the two ever differ. This file is the work order: what changes and where.

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
- **Momentum meter** (L1): unchanged, a vertical bar at the right edge, about 70% of the board height, player's end at the bottom.
- **Timeline** stays a vertical list on the left; its entries, the battle log lines and the target preview use the round portrait (L6).
- **Team editor** (L5): unchanged, rows of cards.
- **Local two-player:** the board flips so the acting player's side is always on the **left**.
- **Unchanged:** battle menu button top left, action bar at the bottom, inspection panel with full card front and back, floating numbers, controls.

## 2. Round battle portraits

- **Look.** A round portrait of the **whole creature**, three-quarter view facing right, in the same pose and expression as its card, on a **plain disc** that contrasts with it (dark plum by default; a lighter colour for creatures too dark for it), inside **one shared dark gold ring**. Undead use the undead ring with their creature's portrait. Summons have their own portraits and keep the summon marker; hovering a summon highlights its summoner.
- **On the portrait: HP only.** An HP arc in the ring's groove and the HP number in the ring's bottom socket (at least 22 px at base). No Power, Speed or Cost, **no status icons**, no side colour (side = position and facing). Statuses and changed values are seen on the full card in the inspection panel (L3, A9, A10).
- **Stealth** (S1): semi-transparent with a faint shimmer; it turns solid again with a short flash when stealth breaks.
- **Airborne** (S2): the portrait floats a little higher and gently bobs, with a soft updraft (faint rising air wisps) under it; ground effects under it stay visible.
- **Target highlights** (S3): a glow on the ring in today's colours: gold legal, white selected, light hover; illegal targets are dimmed.
- **Hover** shows name and classes as before; right-click, long-press or I opens the inspection panel with the full cards.
- **KO** (L4): the portrait cracks (the same thin light crack line), shakes once, turns grey with its ring dimmed and sinks slightly; it stays as a body. No flip, since a portrait has no back. Revive restores colour and removes the crack.
- **Hit:** the portrait shakes briefly, as cards did.
- **Art:** prompts, geometry and file layout in [docs/assets/battle-portraits.md](docs/assets/battle-portraits.md): 319 creature portraits and 14 summon portraits (1024 × 1024, creature inside a centred 860 px circle), `portrait-ring.png` and `portrait-ring-undead.png`. Until the art exists, the game may show the card art masked into the circle as a stand-in (mirrored where the card faces left), so the layout work does not wait for art.

## 3. Ground effects as visual effects

**Today** the core places and tracks effects (`GroundField`, `GroundEffectPlaced`), but `BattleBoard` draws nothing (it only waits 0.15 s).

- **Ground layer** between the battle background and the portraits.
- **Pools** (G2, G10). Each spot with an effect shows a round pool about 1.3× the portrait size, centred where a unit on that coordinate stands. It stays on its coordinate when the column recentres and looks the same on an empty spot. **Same effects on touching spots merge** into one smooth patch (touching as in §12.2: side by side in a column, or overlapping across neighbouring columns of one side). Suggested technique: draw each effect type's pools as soft discs into a `SubViewport` and threshold them in the effect's shader (metaball-style), so touching pools melt together.
- **Drawing** (G1, G3). Godot shaders (`ShaderMaterial`) for the surfaces plus a few `CPUParticles2D` (Compatibility renderer) that rise in front of the portrait's lower edge. No new art.
- **Looks** (G13), tuned during implementation: burning ground a flickering orange pool with rising embers; ice pale cracked frost with a moving glint; water a dark blue pool with slow ripples; steam white mist puffs rising; poison ground a green pool with bubbles and drifting spores.
- **Placing** (G7): the pool spreads out from where the skill lands, about 0.4 s. **Ending:** it fades out (short).
- **Combos** (G8) have their own short transitions: water freezes over into ice; steam bursts up and leaves water; poison flashes on explosion, then fire spreads.
- **Damage** (G9): only the normal floating number.
- **Not shown** (G4, G5, G6, G11): no effect icon, no hover or tap information, no fading as the time runs out, no aiming preview. The timeline's end marker shows when an effect ends.
- **Reduced motion** (G12): ground effects play as normal (an exception to the reduced-motion rule).

## 4. What changes where

| Area | Change |
|---|---|
| scripts/App/BattleBoard.cs | `RowY` becomes a column X; coordinates map to Y; portrait sizing per section 1; one-side view and flyover horizontal; side button arrows left/right; gap markers and previews in the new mapping; new ground layer driven by `GroundEffectPlaced` and the effects' end and combo events (add core events if any are missing) |
| scripts/App/CardView.cs (or a new PortraitView) | Round portrait: disc, masked art, mirroring, ring, HP arc and number, summon marker, KO crack and grey, highlights for legal / selected / hover / illegal targets on the ring |
| Timeline, battle log, target preview | Use the portrait |
| PlayScreen / local two-player (M6.1) | Acting side on the left |
| assets/ | `portraits/`, the two rings, shaders and particle materials for ground effects; stand-in until the art exists |
| docs | Done: ui_spec.md, assets/README.md, docs/assets/README.md, docs/assets/battle-portraits.md, docs/mockups/README.md |

## 5. Checks

- Every unit, KO body and summon sits in the column and at the height its row and coordinate give; the forced movement preview, gap markers and delayed `direct` markers match.
- Player portraits face right, enemy portraits face left, in both views and after a local two-player flip.
- All portraits share one size, set by the fullest column within the caps; adding a sixth unit to a column shrinks every portrait.
- The one-side view switches with the left/right button and by itself when a skill targets the enemy; reduced motion cuts instead of flying.
- A ground pool stays on its coordinate when units move or the column recentres, shows on an empty spot, merges with the same effect on touching spots, and matches `GroundField` after every event (combos and expiry included).
- Rings, discs and ground pools never hide HP, KO or target highlighting (ui_spec Interaction checks).
