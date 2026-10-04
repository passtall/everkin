---
name: everkin-design
description: Use for ANY Everkin game-design work — rules, classes, skills, statuses, damage and Momentum numbers, timeline, formation, UI behavior, balance, or edits to docs/game_system.md, docs/classes_and_skills.md and docs/ui_spec.md. Forces reading the design documents before answering or proposing anything.
---

# Everkin design reference check

The design documents are the source of truth. Never answer, propose or edit from memory.

## Before doing anything

1. Read the relevant parts of **docs/game_system.md**. It is large (about 1000 lines); read it in pages or search for the section headings. The section map:
   - §4 classes, tags and skill rules (§4.7) · §5 stats, damage formula, anchors · §6 timeline, auto events, Momentum (§6.7)
   - §7 formation and rows · §8 targeting, interception (§8.2), columns (§8.4) · §9 movement · §10 stealth, KO, statuses
   - §11 summons · §19 decisions still required · §20 details before implementation
2. Read **docs/classes_and_skills.md** for any class or skill work.
3. Read **docs/ui_spec.md** for any UI, screen or interaction question.
4. Check §19 of game_system.md. If a rule is listed there as undecided, do not invent it: ask the owner.

## Where things go

| Content | Document |
|---|---|
| Game-wide rules (damage, Momentum, timeline, interception, tags, statuses, formation) | docs/game_system.md |
| Class and skill definitions (one class at a time) | docs/classes_and_skills.md |
| Screens, layouts, interactions | docs/ui_spec.md |

- When a decision is game-wide, **write it to game_system.md first**, update the §19 table and the §20.5 worked checks if relevant, and have the class document refer to it. Never duplicate global rules in the classes document.
- Class documents contain only that class's lanes, skills, numbers and class-specific rules, plus a TBD list.
- After any rule change, check whether docs/ui_spec.md needs a matching update (for example the Momentum meter, timeline contents, skill-unavailable reasons).

## How to work with the owner

- Offer options and a recommendation, but **the owner decides**. Present your own ideas only as options, never as decisions.
- Label status clearly: decided vs TBD. Ask when something is unclear; do not guess.
- Every skill is unique and has a clear role. Skills use **tags**, not categories.
- Keep numbers low and whole. Always check new numbers against the damage anchor (critter 20 HP, three Sniper Shots).
- Respect interception by default. Only specific tags ignore it by default. e.g. airstrike
- No mana, no cooldowns. Costs come from timeline, Momentum, HP, position or consequences.
