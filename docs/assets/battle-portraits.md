# Battle portraits: overview

Written 2026-11-10 from the owner's answers on the battle UI quiz (https://claude.ai/artifact/EcyT2Qns9duAcZpDbMmmJq; log `battle-ui-decisions-2.md` in the project files). Where the portraits are used: [ui_spec.md](../ui_spec.md), Battle screen, Battle portraits; the change spec is [spec.md](../../spec.md).

Every creature (319 in `assets/creatures/`) and every summon card in use (9 of the 14 in `assets/summons/`) gets one round battle portrait, facing **right**. The enemy side shows the same image mirrored, so there is no left-facing version. Undead reuse their creature's portrait inside the undead ring; summons sit in a silver summon ring. The card art is not changed; portraits are new, separate files.

## What gets made

| Asset | Count | Source file | Result |
|---|---|---|---|
| Creature portrait | 319 | the creature's card art, `assets/creatures/<art file>.png`, attached as identity reference | `source-art/portraits/<art file>.png` (original, never changed), cleaned copy in `assets/portraits/<art file>.png` |
| Summon portrait | 9 | `assets/summons/<id>.png` attached | `source-art/portraits/summons/<id>.png`, `assets/portraits/summons/<id>.png` |
| Ring | 1 | — | `assets/frames/portrait-ring.png` |
| Undead ring | 1 | `assets/frames/undead.png` attached as style reference | `assets/frames/portrait-ring-undead.png` |
| Summon ring | 1 | `assets/frames/portrait-ring.png` attached | `assets/frames/portrait-ring-summon.png` |

Run one request per image. Test with three or four creatures of different shapes (a tall one, a wide one, a tiny one, a flier) before the batch. An implementing session on the owner's PC extends `tools/art/generate_creatures.py` for this (reference image = the card art, size 1024 × 1024; needs the image API key there), makes 4 test portraits for the owner to check, then runs the batch and shows a contact sheet before cleaning (owner, 2026-11-10). The new board is built only once the portraits exist; there is no stand-in.

## Geometry (shared by portraits and rings)

All files are 1024 × 1024. The ring's open window is a circle of about **860 px diameter centred at (512, 512)**. The game draws a plain disc in that window, puts the portrait on top, masks it to the circle and lays the ring over it. So a portrait must keep the whole creature inside that 860 px circle; anything outside it is cut off.

## Prompt files

| Topic | File |
|---|---|
| Creature portraits (319) | [portrait-creatures.md](portrait-creatures.md) |
| Summon portraits (9) | [portrait-summons.md](portrait-summons.md) |
| Gold ring | [portrait-ring.md](portrait-ring.md) |
| Silver summon ring | [portrait-ring-summon.md](portrait-ring-summon.md) |
| Undead ring | [portrait-ring-undead.md](portrait-ring-undead.md) |

Cleaning, checks and the disc colour step are in [portrait-creatures.md](portrait-creatures.md) and apply to summons too.
