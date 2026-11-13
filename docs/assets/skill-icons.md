# Skill icons for the action bar

The action bar (ui_spec, Battle screen) shows each skill as a square icon slot with the name below, Delay in a corner and the Momentum cost in a gold gem. The game draws the slot frame, the name, the key number and the gems; the icon is only the picture inside.

Every slot has its own icon (ui_spec): **84 class actives**, **319 unique attacks** and Move and Skip Turn, **405 icons**. Keystones never sit on the bar and have no icon. Each file is named after the skill ID: the skill or attack name in snake case, apostrophes and punctuation dropped (`hunters_shot.png`, `spotted.png`). Save to `assets/icons/skills/<file>`.

**Status (2026-11-13).** Only `action-move.png` and `action-skip.png` are final. The 47 old `attack-<type>.png` and `<profession>-<branch>.png` icons stand in until the new set exists and are deleted then (milestone M5.6). The prompts of the old run are kept in [skill-icons-prompts-used.json](skill-icons-prompts-used.json).

## Move and Skip Turn (final)

| File | For | Subject |
|---|---|---|
| `action-move.png` | Move | Two curved arrows, one up and one down, around a small card shape. Cream with gold accent. |
| `action-skip.png` | Skip Turn | A small hourglass with a curved "pass" arrow over it. Cream with blue accent. |

## Prompt files

| Icons | Count | File | Made |
|---|---|---|---|
| Move, Skip Turn | 2 | this file (style block in the class file) | yes |
| Class actives | 84 | [skill-icons-classes.md](skill-icons-classes.md) | no |
| Unique attacks | 319 | [skill-icons-attacks.md](skill-icons-attacks.md) | no |
