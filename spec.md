# Spec: battle UI, left/right board, round portraits, visible ground effects

**Status:** DRAFT 2026-11-10. Owner's request is settled (below); the details marked *open* wait for the owner's quiz answers (https://claude.ai/artifact/EcyT2Qns9duAcZpDbMmmJq) and must not be implemented before they are filled in. Once final, the adopted rules move into [docs/ui_spec.md](docs/ui_spec.md) (Battle screen), which then wins over this file.

## 0. Owner's request (2026-11-10)

1. The board changes from bottom/top to **left/right**: the player's team on the left, the enemy on the right. Rows become vertical. The view switcher stays for now and switches between the full view (both sides) and one side only.
2. Every unit gets a separate, smaller (round) **battle portrait frame**. This needs new art with the creature **facing right**; the enemy side shows it mirrored. An art prompt is written after 15 questions to the owner.
3. **Ground effects** (game_system.md §12.2) become actual visual effects on the board.

Nothing in the rules changes. Rows, coordinates, distances, adjacency, interception and ground effects work exactly as in game_system.md; only how they are drawn changes.

## 1. Left/right board

### 1.1 Settled

- **Sides.** The player's side is on the left, the enemy's on the right. From the left screen edge inward: player Rear, Middle, Front; then the battle line; then enemy Front, Middle, Rear. The two Front columns face each other in the middle, with the slightly larger gap at the battle line (ui_spec Board fill) now vertical.
- **Rows are columns.** Each row is drawn as a vertical column. A row's order (its coordinates, `Formation.CenterOf`) runs **top to bottom** on both sides, so the lowest coordinate is at the top. The rows stay centred vertically, as they were centred horizontally. A delayed `direct` event's marked slot, move gaps, the forced movement preview and the 50% insertion point are placed by the same top-to-bottom mapping.
- **Facing.** Player portraits face right, enemy portraits face left (the same image mirrored).
- **View switcher.** The view button stays in the top right corner with the same two modes: both sides, or one side at a time. In the one-side view the second button switches sides with a **left/right arrow** (it was up/down), and the flyover now scrolls **horizontally** across the battle line (an instant cut with reduced motion). Picking a skill that targets the enemy still switches to the enemy side by itself and returns after the action. The automatic choice (one-side view below about 7 inches) and the remembered manual choice stay as they are.
- **Local two-player.** The board flips so the acting player's side is always on the **left** (it was the bottom), with the same clear cue when the side changes.
- **Unchanged.** Battle menu button top left, action bar at the bottom (same content and behaviour), timeline as a vertical list on the left edge, inspection panel with full card front and back, target preview, floating numbers, battle log.
- **Board cards become portraits.** The compact 2:3 board card is replaced by the round battle portrait of section 2. The full card (front and back) is still shown on hover preview where it was, and in the inspection panel.

### 1.2 Open (quiz part 1)

| # | Question | Recommendation |
|---|---|---|
| L1 | Momentum meter position | Horizontal, top centre over the battle line, player's end on the left |
| L2 | Portrait size | Both-sides view about 136 px at base (6 fit a column); one-side view up to about 200 px, a column with more than 4 units shrinks evenly |
| L3 | Status icons on a portrait | Along the top of the ring, up to 4 then +N |
| L4 | KO look | Crack across the portrait, greyed, ring dimmed; no flip |
| L5 | Team editor formation | Also three columns of portraits, Front on the right |
| L6 | Other uses of the portrait | Timeline, battle log and target preview too |

## 2. Round battle portraits

### 2.1 Settled

- Every creature and every summon has a battle portrait in its own small frame, used on the board instead of the compact card.
- New art is made for it, with the creature facing **right**. The enemy side shows the same image mirrored horizontally; no second image.
- The original card art (`source-art/`, `assets/creatures/`) is not changed; portraits are separate files.

### 2.2 Open (quiz part 2, 15 art questions)

Frame shape (A1), crop (A2), view angle (A3), identity reference (A4), image format (A5), background inside the ring (A6), breaking the frame (A7), ring design (A8), side cue (A9), numbers on the portrait (A10), lighting (A11), asymmetric details (A12), pose (A13), size across creatures (A14), summons and undead (A15). Recommendations are on the quiz page.

After the answers: the art prompt goes to `docs/assets/battle-portraits.md` (with a style block like docs/assets/creature-art-prompt.md), the file layout to `assets/README.md` (proposed: `assets/portraits/<art file>.png` for creatures, `assets/portraits/summons/<id>.png`, ring art in `assets/frames/`).

## 3. Ground effects as visual effects

### 3.1 Today

The core already places and tracks ground effects (`GroundField`, event `GroundEffectPlaced`), and `assets/icons/ui/` has `ground-fire`, `ground-ice`, `ground-water`, `ground-steam`, `ground-poison`. The battle screen draws nothing for them (`BattleBoard` only waits 0.15 s on `GroundEffectPlaced`).

### 3.2 Proposal

- A **ground layer** sits between the battle background and the portraits. Each spot that holds an effect gets a **pool**: a round patch, a little larger than a portrait, centred where a unit on that coordinate stands (so it stays put when the column recentres, like the rule).
- Pools are drawn by **Godot shaders** (a `ShaderMaterial` on a circle sprite: scrolling noise for flames and poison, ripple rings for water, crack lines and a moving glint for ice, soft mist for steam) plus a few **particles** (`CPUParticles2D`, safe on the Compatibility renderer the project uses; embers, steam wisps, bubbles, spores) that rise in front of the portrait's lower edge. No new art; it follows the effects decision in production.md §1.6 (engine particles and shaders).
- The existing ground icon sits small on the pool's edge, so the effect reads without colour.
- Placing, ending, combos (water freezing, steam, poison explosion) and damage moments get short animations; reduced motion keeps pools static without particles.

### 3.3 Open (quiz part 3)

Approach (G1), shape (G2), layering (G3), icon cue (G4), information on hover (G5), running out (G6), placing (G7), combos (G8), damage moments (G9), empty spots (G10), aiming preview (G11), reduced motion (G12), the per-effect looks (G13).

## 4. What changes where (for the implementing session)

| Area | Change |
|---|---|
| docs/ui_spec.md, Battle screen | Rewrite the side placement, phone view, layout sizes, board fill, Momentum and KO bullets from sections 1 to 3; keep everything else |
| docs/mockups/README.md | Note that the battle mockup's top/bottom layout is superseded |
| scripts/App/BattleBoard.cs | `RowY` becomes a column X; coordinates map to Y; `Layout` sizes portraits per section 1; one-side view and flyover go horizontal; Momentum meter per L1; new ground layer (section 3) driven by `GroundEffectPlaced` and the effects' end and combo events |
| scripts/App/CardView.cs | New portrait mode (round mask, ring, HP, statuses, KO, mirroring) beside the existing full and compact modes |
| Timeline, battle log, target preview | Use the portrait if L6 is adopted |
| scripts/App/TeamEditor.cs | Formation as columns if L5 is adopted |
| Local two-player (M6.1) | Acting side on the left |
| assets/, docs/assets/ | Portrait files, ring art, shaders and particle materials for ground effects |

## 5. Checks

- Every unit, KO body and summon sits in the column and at the height its row and coordinate give; the forced movement preview, gap markers and delayed `direct` markers match.
- Mirroring: enemy portraits face left, player portraits face right, in both views and after a local two-player flip.
- The one-side view switches with the left/right button and by itself when a skill targets the enemy; reduced motion cuts instead of flying.
- A ground effect stays on its coordinate when units move or the column recentres, is visible on an empty spot, and its pool matches `GroundField` after every event (including combos and expiry).
- Cosmetic frames and ground pools never hide HP, statuses, KO or target highlighting (ui_spec Interaction checks).
