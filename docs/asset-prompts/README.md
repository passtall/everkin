# Missing asset prompts

Written 2026-11-07 from the implementation dry run. Each file is a ready-to-paste prompt for an image or audio generator (ChatGPT, Codex or similar). Put every result at the path the file names, then push.

| File | What | Count | Goes to |
|---|---|---:|---|
| [summons.md](summons.md) | Card art for summons, constructs, totems and traps | 14 | `art/summons/` |
| [skill-icons.md](skill-icons.md) | Action bar icons: one per profession branch, one per Attack Type attack, Move, Skip Turn | 49 | `art/icons/skills/` |
| [ui-icons.md](ui-icons.md) | Status, ground effect, marker and HUD icons | 32 | `art/icons/ui/` |
| [brand.md](brand.md) | Wordmark, emblem and app icon | 3 | `art/brand/` |
| [sounds.md](sounds.md) | Sound effects, result stings and biome ambient loops | 37 effects (×3 variants for battle sounds), 2 stings, 18 loops | `audio/` |

Already in place, no prompt needed:

- Undead card frame: `art/frames/card/undead.png` (2026-11-07).
- Menu background: `art/ui/menu-background.png` and `.svg` (the adopted night fairytale background, copied from the project files 2026-11-07).
- Fonts are downloaded, not generated. Recommended in the dry run: **Alegreya** (headings and names) and **Alegreya Sans** (body and numbers), both SIL Open Font License, from Google Fonts. Put the TTF files and their `OFL.txt` in `art/fonts/`.

Skill icons follow the dry-run recommendation (one icon per branch, shared by that branch's skills). If unique icons per skill are wanted later, skill-icons.md explains how to extend it.
