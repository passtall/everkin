# Interface specification

**Status:** Structural requirements established; detailed layouts and interactions incomplete.
**Game rules:** [game_system.md](game_system.md), section 13. This file owns screen layouts, interaction behavior, and visual checks.

## Required information hierarchy

| Interface requirements |
|---|
| Card fronts show front artwork, name, every class name, Life/HP, Speed, Power, Defense, statuses, and explicit KO. In battle the board shows a **compact card** (front artwork, HP bar, statuses, KO); the full front appears on hover and in inspection (see Battle screen below). A stealthed unit shows a clear stealth cue (icon, optionally a dimmed card) to both sides, and its turns stay on the timeline (game_system.md §10.1). |
| Card backs show rear artwork and every currently usable skill with its actual build modifications. |
| Front and back share the selected cosmetic frame; gameplay readability takes priority. |
| Card backs are inspection, not the action menu. Opposing skills must be inspectable. |
| Keep the horizontal upcoming-turn timeline at the top and the active unit's horizontal skill bar at the bottom. |
| Show valid targets distinctly; invalid targets are visibly unavailable. Preview affected cards and foreseeable timing changes. |
| Execute target-requiring skills through skill selection then target selection, without redundant confirmations. |
| Cards stay spatially stable during ordinary attacks; use traveling/targeted VFX and restrained reactions. |
| KO combines an explicit indication with strong visual change, while preserving visible occupancy. |
| Show adjacency/guard/aura/summon relationships contextually rather than as permanent clutter. |
| Party setup reuses friendly rows and their exact positioning logic, with the enemy side absent. |

## Combat interaction states to specify

| State | Established behavior | Still open |
|---|---|---|
| Reviewing battlefield | Timeline and unit state visible. Right-click, long-press or the inspect key opens the inspection panel (front and back side by side). | None. |
| Active unit awaiting action | All skills shown, plus the basic actions Move and Skip Turn (game_system.md §6.4) as two smaller buttons at the right end of the skill bar; unavailable actions explain why. Move highlights the gaps the unit can land in. More than 9 skills wrap into a second bar row. Keyboard: 1 to 9 pick skills, M Move, Space Skip Turn. | None. |
| Skill selected | Show legal targets, affected units, and timing preview. Right-click, Esc, Android back, selecting the same skill again, or tapping empty space on the player's own side cancels; tapping empty space on the enemy side does not. Selecting another skill switches directly. | Targetless skills, multi-mode skills. |
| Target preview | Expose relevant range/protection and foreseeable outcomes. Mouse: hover previews, click executes. Touch: the first tap on a target previews, tapping the same target again executes. Keyboard: arrows or Tab cycle targets, Enter executes, I inspects. | Uncertain/hidden outcomes. |
| Resolving | Automatic effect/reaction processing; no enemy-turn decision popups. Choosing actions is locked. Inspection, the speed setting (1x, 2x, instant) and surrender still work; there is no tap to finish the current action instantly. Opening inspection pauses the animation queue until it closes, except in online play. Enemy turns play their effects straight away, with no announcement banner; the timeline shows who acted. | None. |
| Result | An overlay over the final board shows the result (victory, defeat, draw, surrender), any unlocked units, and Continue, Retry (after a defeat) and Exit. A draw reads as a defeat in single-player modes. | Reward contents (campaign). |

These are specification categories, not a mandated engine state-machine implementation.

## Screens and layouts to complete

- Support **Windows, Android, and iOS in landscape only**. Exact minimum device requirements, aspect ratios, and layout sizes remain open.
- Begin development with headless AI-versus-AI batch runs (game_system.md §2.5). A **battle viewer** then runs AI-versus-AI battles on the card UI as a debug mode, also used to debug UI issues. It can start a battle with the same setup (teams and formations) as any recorded battle; it does not replay the recorded battle exactly. Its controls are pause, step one action, speed (1x, 2x, instant), inspect any unit, and pick a recorded setup (game_system.md §16.1). It shows only what a player would see, not the AI's scores or alternatives.
- Party setup: a **team editor** in the manner of a deck builder (game_system.md §13.2). Pick up to six creatures from the collection (each at most once), place them in rows, and save the team under a name. Several named teams; the last used one is preselected. Still open: recruit browser layout, filters, card inspection, tree editing, validation messages.
- **Battle screen** (decided 2026-10-06):
  - The player's side is at the bottom and the enemy's at the top, with the two Front rows facing each other in the middle.
  - Board cards are compact: front artwork, HP bar, statuses and KO. Name, classes, Speed, Power and Defense appear on hover and in the inspection panel.
  - Every row uses the same card size; depth is shown by spacing and row labels.
  - Cards keep full size up to the width a row can fit; beyond that every card in that row shrinks evenly.
  - The Momentum meter is a vertical bar at the right edge spanning both halves, with the player's end at the bottom.
  - The timeline shows the next 12 entries; the rest are reached by scrolling it.
  - Still open: exact card dimensions, safe areas, skill labels, status overflow and tooltips.
- Card front/back inspection: all class names, long skill text, scrolling, modified values, source of modifiers, opponent visibility.
- Main menu, settings, collection, exploratory story campaign, unit-unlock presentation, free battle against the AI, and at least one post-story random-battle mode (no rewards). A new player starts with the campaign tutorial; the other menu entries unlock when it is complete (game_system.md §2.2). The battle viewer appears only in development builds.
- Campaign exploration uses **illustrated locations with selectable paths and events**. The current campaign is a linear sequence of **four stages**; detailed story/location content comes later. Show guaranteed encounter unit rewards and their unlock results; do not add capture interactions for now.
- During continuous development testing, units start at **level 20 with all skills available through their creature and assigned classes**. **Design classes and skills first; the tree editor and point-spending UI come later.** Do not require a skill loadout or tree purchases for initial testing. The skill bar must expose all available active skills, normally one type-specific attack plus 6–8 additional usable skills (7–9 total), with overflow support for unrestricted initial testing. Later trees mix **10–15 meaningful active skills, passives, and modifiers across 3–4 branches** per class; avoid percentage-only upgrade filler.
- A **surrender** control is available throughout a battle. It asks for confirmation once and counts as a defeat (game_system.md §12.3).
- Show **one contested Momentum meter** indicating advantage toward either party, starting at neutral zero. The meter runs from −10 to +10. It is a vertical bar at the right edge (see Battle screen). Spending previews remain open. A skill whose Momentum requirement is not met shows the reason. Every battle begins with all participating units at full HP.
- Show auto events (such as queued shots) on the timeline with their owner. A unit that is busy until its auto events finish is indicated and takes no turn. Each auto event shows the Momentum shift it will cause when it resolves (game_system.md §6.7). The timeline order follows the tie and same-time rules of §6.1, and a skipped turn is visibly marked as skipped.
- **Forced movement preview** (game_system.md §9.5). Selecting a push, pull or swap shows where the moved unit lands, any units that would be bumped along the chain, and the insertion point for a 50% landing. If the chain is blocked, the preview shows that nothing moves and marks every unit in the chain with the 1 bump damage it would take. Bump damage and constructs moved like units use the normal damage and KO feedback.
- **Delayed `direct` effects.** A delayed `direct` event on the timeline marks its slot (a coordinate in a row, §4.7 and §7.3) on the battlefield, not a unit, so the player can see that it may hit someone else if the target moves away.
- **Stealth feedback.** A unit that loses stealth (by attacking, being hit, or a reveal or cleanse) shows that change clearly. A stealthed unit cannot be selected as a normal target, and area or `direct` previews still highlight it when it is hit (game_system.md §10.1).
- Provide **free respec outside battle** once point allocation is available. Reset/reallocation must respect the later-defined budget and prerequisites; exact controls and dependent-node handling still need specification.
- **Local two-player** (game_system.md §2.4): each player picks a saved team built from the device's collection; both may use the same creatures. During battle the board flips so the acting player's side is always at the bottom, with a clear cue when the side changes. No pause on inspection in online play applies only to online; locally inspection pauses as usual.
- Prepare interfaces for future online PvP, but **do not implement the online mode or its connected flows/services until explicit owner greenlight**. Planning inspection and shared control boundaries is allowed; online implementation is gated.
- First-time guidance and error/empty/loading states for every included flow.

## Visual acceptance still required

Specify target screen sizes and minimum text/touch dimensions, distinction between cosmetic and gameplay borders, color-independent status cues, action/target selection appearance, damage/heal/status/KO feedback, reduced-motion behavior, and audio equivalents if selected.

Produce annotated layouts before declaring UI specification complete. Existing creature images and contact sheets are art references, not approved finished card layouts.

## Interaction checks

- A modified skill is shown with its actual current effects on the card back.
- Selecting a skill previews the same scheduling consequence that execution produces when relevant state is unchanged.
- Selecting a valid target executes without redundant confirmation.
- An ordinary attack leaves formation/order unchanged unless the skill explicitly moves a unit.
- Cosmetic frames never conceal KO, statuses, target highlighting, or combat values.
- Party setup and combat use the same ordering and centering rules.
- The forced movement preview matches the executed result when the board state is unchanged, including blocked chains and their bump damage.
- A delayed `direct` event's marked slot is the slot it will hit, and a queued projectile's preview shows the unit behind a stealthed or KO target when relevant.
- The Momentum meter never displays beyond −10 or +10, and a shift that would exceed a limit is shown as clamped.

Complete these checks for mouse/touch and the final target screen sizes. Specify dense-state behavior for many statuses, long class/skill names, 7–9 skills and larger test sets, and the selected maximum unit count. These are verification requirements, not completed test results.

The current class labels are Hunter, Elementalist, Healer, Warrior, Guardian, Feral, and Leader. Shared class skills use consistent descriptions across creatures; display the current stat-scaled values and explicit modifications where relevant. Each unit also displays its one type-specific attack.
