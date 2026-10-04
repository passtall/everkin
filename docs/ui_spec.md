# Interface specification

**Status:** Structural requirements established; detailed layouts and interactions incomplete.
**Game rules:** [game_system.md](game_system.md), section 13. This file owns screen layouts, interaction behavior, and visual checks.

## Required information hierarchy

| Interface requirements |
|---|
| Card fronts show front artwork, name, every class name, Life/HP, Speed, powers, defenses, statuses, and explicit KO. |
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
| Reviewing battlefield | Timeline and unit state visible; unit inspection available. | Input gestures, pause/time behavior, inspection modality. |
| Active unit awaiting action | All skills shown; unavailable actions explain why. | Reposition control, keyboard/touch bindings, layout for 7?9 skills and overflow beyond that in testing. |
| Skill selected | Show legal targets, affected units, and timing preview. | Cancel/back behavior, targetless skills, multi-mode skills. |
| Target preview | Expose relevant range/protection and foreseeable outcomes. | Hover versus touch preview/commit semantics, uncertain/hidden outcomes. |
| Resolving | Automatic effect/reaction processing; no enemy-turn decision popups. | Input locking, animation speed/skip, inspection during resolution. |
| Result | Must communicate result and allow the selected between-battle flow. | Exact screen, rewards, retry/exit options, simultaneous outcomes. |

These are specification categories, not a mandated engine state-machine implementation.

## Screens and layouts to complete

- Support **Windows, Android, and iOS in landscape only**. Exact minimum device requirements, aspect ratios, and layout sizes remain open.
- Begin development with an **AI-versus-AI testing view**. Its simulation controls and inspection tools still need definition.
- Party setup: recruit browser, card inspection, tree editing, row placement/reorder, validation, saved teams if selected.
- Battle: landscape layouts for desktop and mobile, card dimensions, safe areas, timeline capacity, skill labels, status overflow, tooltips.
- Card front/back inspection: all class names, long skill text, scrolling, modified values, source of modifiers, opponent visibility.
- Main menu, settings, collection, exploratory story campaign, unit-unlock presentation, and at least one post-story random-battle mode with further unit unlocks.
- Campaign exploration uses **illustrated locations with selectable paths and events**. The current campaign is a linear sequence of **four stages**; detailed story/location content comes later. Show guaranteed encounter unit rewards and their unlock results; do not add capture interactions for now.
- During continuous development testing, units start at **level 20 with all skills available through their creature and assigned classes**. **Design classes and skills first; the tree editor and point-spending UI come later.** Do not require a skill loadout or tree purchases for initial testing. The skill bar must expose all available active skills, normally one type-specific attack plus 6?8 additional usable skills (7?9 total), with overflow support for unrestricted initial testing. Later trees mix **10–15 meaningful active skills, passives, and modifiers across 3–4 branches** per class; avoid percentage-only upgrade filler.
- Show **one contested Momentum meter** indicating advantage toward either party, starting at neutral zero. The meter runs from −10 to +10. Exact placement and spending previews remain open. A skill whose Momentum requirement is not met shows the reason. Every battle begins with all participating units at full HP.
- Show auto events (such as queued shots) on the timeline with their owner. A unit that is busy until its auto events finish is indicated and takes no turn.
- Provide **free respec outside battle** once point allocation is available. Reset/reallocation must respect the later-defined budget and prerequisites; exact controls and dependent-node handling still need specification.
- Single-player AI battles and local two-player setup/handover flows; exact interaction rules remain open.
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

Complete these checks for mouse/touch and the final target screen sizes. Specify dense-state behavior for many statuses, long class/skill names, 7?9 skills and larger test sets, and the selected maximum unit count. These are verification requirements, not completed test results.

The current class labels are Hunter, Elementalist, Healer, Warrior, Guardian, Feral, and Leader. Shared class skills use consistent descriptions across creatures; display the current stat-scaled values and explicit modifications where relevant. Each unit also displays its one type-specific attack.
