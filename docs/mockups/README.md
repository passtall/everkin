# UI mockups

The three interactive mockups Andreas reviewed in October 2026, saved from their claude.ai pages on 2026-11-08 (implementation dry run a9). Open them in a browser. They show the look and layout to build: night storybook style, the battle board, the team editor and the campaign map.

| File | Screen | Reviewed page |
|---|---|---|
| `battle.html` | Battle screen: board, vertical timeline on the left, MMO-style action bar, Momentum meter | https://claude.ai/artifact/QGb4wpanu6v3JkosgRnsgC |
| `team-editor.html` | Team editor: team chips, formation, collection panel | https://claude.ai/artifact/YD8PLuQghB3zCC2C5AXowd |
| `campaign-map.html` | Campaign map | https://claude.ai/artifact/5EkfboUi299g2mshCjna67 |

**[ui_spec.md](../ui_spec.md) wins wherever the two differ.** The mockups were made before several later decisions, so these parts are out of date:

- **Stats.** The cards show a Defense stat and size tiers (Small, Medium, Large). Defense was replaced by Cost (2026-10-15), and there are no size tiers (2026-11-08). Corner gems are HP, Power, Speed and Cost; the name plate shows the name and the two class names.
- **Classes and skills.** Class names and skills on the cards are from the old class roster. Each creature now has two of the 28 classes, two keystones and its own unique attack: [class_system.md](../class_system.md), [skills.md](../skills.md), [creature-classes.md](../creature-classes.md).
- **Collection filter.** By class only (ui_spec.md, Team editor).
- **Fonts.** The mockups use Fraunces and Atkinson Hyperlegible from Google Fonts. The game uses Alegreya and Alegreya Sans from `assets/fonts/` (production.md §1.6).
- **Art.** The art embedded in the mockups is not the final asset set; the game uses `assets/`.
- **Phone view, board fill, layout sizes and interface details** were settled after the mockups (ui_spec.md, Battle screen).
- **Skill trees** no longer exist (2026-11-11); any tree editing shown is obsolete.
- **Battle board** (2026-11-10): the board is now left/right (player left, enemy right, rows as vertical columns) with round portraits instead of board cards, and ground effects are drawn as pools (ui_spec.md, Battle screen; spec.md).
