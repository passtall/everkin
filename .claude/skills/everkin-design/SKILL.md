---
name: everkin-design
description: Use for ANY Everkin game-design work — rules, classes, skills, statuses, damage and Momentum numbers, timeline, formation, UI behavior, balance, assets, or edits to the documents in docs/. Forces reading the design documents before answering or proposing anything.
---

# Everkin design reference check

The design documents are the source of truth. Never answer, propose or edit from memory.

## Before doing anything

1. Read the relevant parts of **docs/game_system.md**. It is large (about 1500 lines); read it in pages or search for the section headings. The section map:
   - §2 modes, campaign, coins, AI-versus-AI harness (§2.5) · §4 skill trees and skill rules (§4.1 tree model, §4.7) · §5 stats, damage formula, Cost, anchors · §6 timeline, auto events, Momentum (§6.7)
   - §7 formation, rows and distance (§7.4) · §8 targeting, interception (§8.2), area patterns (§8.4) · §9 movement · §10 stealth, KO, statuses (§10.5), airborne (§10.6)
   - §11 summons and undead (§11.5) · §12 ground effects and battle end · §15 world and chapters · §19 decisions still required · §20 details before implementation
2. Read **docs/class_system.md** for Attack Types and Professions and **docs/skills.md** for every skill. Read **docs/creature-classes.md** for the roster (all 319 creatures: classes, biome, stats, Cost, art and crops).
3. Read **docs/ui_spec.md** for any UI, screen or interaction question.
4. Read **docs/production.md** for platforms, release, business model, technology, saving and online. New decisions that are not game mechanics go there, not into game_system.md.
5. Check §19 of game_system.md. If a rule is listed there as undecided, do not invent it: ask the owner.
6. Never use **docs/archive/** as a source: it holds superseded documents for history only.

## Where things go

| Content | Document |
|---|---|
| Game-wide rules (damage, Momentum, timeline, interception, statuses, formation, summons, ground effects) | docs/game_system.md |
| Attack Types and Professions | docs/class_system.md |
| Every attack and profession skill, with Delay, Momentum, delivery and range | docs/skills.md |
| Creature roster: classes, biome, stats, Cost, art file and crops | docs/creature-classes.md |
| Screens, layouts, interactions | docs/ui_spec.md (reviewed mockups in docs/mockups/, reference only) |
| Implementation tasks / live progress and open questions from coding | docs/implementation-plan.md / PROGRESS.md (how to work: CLAUDE.md) |
| Non-mechanics decisions: platforms, release, business model, technology, saving, online, languages, accessibility | docs/production.md |
| Asset prompts and provenance | docs/assets/ (the files are in assets/, see assets/README.md) |

- When a decision is game-wide, **write it to game_system.md first**, update the §19 table and the §20.5 worked checks if relevant, and have the class or skill document refer to it. Never duplicate global rules.
- After any rule change, check whether docs/ui_spec.md needs a matching update (for example the Momentum meter, timeline contents, skill-unavailable reasons).
- Original creature art in `source-art/` is never changed; cleanup works on copies in `assets/creatures/`.

## How to work with the owner

- Offer options and a recommendation, but **the owner decides**. Present your own ideas only as options, never as decisions.
- Label status clearly: decided vs TBD. Ask when something is unclear; do not guess.
- Every skill is unique and has a clear role, and changes what a unit can do rather than adding flat numbers.
- Keep numbers low and whole. Check new numbers against the HP anchors (critter 20 HP, human 35–45, giant turtle 80).
- Only Melee and Projectile skills can be intercepted (game_system.md §8.2); such a skill ignores interception only if it says so.
- No mana, no cooldowns. Costs come from timeline, Momentum, HP, position or consequences.
- Design changes go straight to main, no pull requests.
