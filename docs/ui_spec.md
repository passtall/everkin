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
| Keep the upcoming-turn timeline as a vertical list on the left and the active unit's horizontal skill bar at the bottom. |
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
| Skill selected | Show legal targets, affected units, and timing preview. A skill with a Momentum cost shows a ghost marker on the meter where it lands after paying. Right-click, Esc, Android back, selecting the same skill again, or tapping empty space on the player's own side cancels; tapping empty space on the enemy side does not. Selecting another skill switches directly. A skill that needs no target shows its preview on the first press and is used on the second press (or Enter). Skills never have modes (game_system.md §6.3). | None. |
| Target preview | Expose relevant range/protection and foreseeable outcomes. The preview shows the damage range and, when targeting, the skill's success chance in percent. Mouse: hover previews, click executes. Touch: the first tap on a target previews, tapping the same target again executes. Keyboard: arrows or Tab cycle targets, Enter executes, I inspects. | None. |
| Resolving | Automatic effect/reaction processing; no enemy-turn decision popups. Choosing actions is locked. Inspection, the speed setting (1x, 2x, instant) and surrender still work; there is no tap to finish the current action instantly. Opening inspection pauses the animation queue until it closes, except in online play. Enemy turns play their effects straight away, with no announcement banner; the timeline shows who acted. | None. |
| Result | An overlay over the final board shows the result (victory, defeat, draw, surrender), any unlocked units, and Continue, Retry (after a defeat) and Exit. A draw reads as a defeat in single-player modes. | Reward contents (campaign). |

These are specification categories, not a mandated engine state-machine implementation.

## Screens and layouts to complete

- Support **Windows, Android, and iOS in landscape only**. Devices and screen shapes are set in production.md §1.1; layout sizes are adopted (Battle screen, below).
- Begin development with headless AI-versus-AI batch runs (game_system.md §2.5). A **battle viewer** then runs AI-versus-AI battles on the card UI as a debug mode, also used to debug UI issues. It can start a battle with the same setup (teams and formations) as any recorded battle; it does not replay the recorded battle exactly. Its controls are pause, step one action, speed (1x, 2x, instant), inspect any unit, and pick a recorded setup (production.md §2.3). It shows only what a player would see, not the AI's scores or alternatives.
- Party setup: a **team editor** in the manner of a deck builder (game_system.md §13.2). Pick up to six creatures from the collection (each at most once), place them in rows, and save the team under a name. Several named teams; the last used one is preselected. The collection can be filtered by class, size tier and attack type and sorted by name, HP, Speed or Power. Layout (adopted 2026-10-06 from the mockup): saved teams as chips across the top with the team name and Save; the formation on the left; the collection as a panel on the right, which becomes a drawer sliding in from the right on phones. Creatures are added and rearranged **by dragging only**: drag from the collection into a row, between rows, or out of the team to remove. On touch, a drag starts as soon as the finger moves; a long-press without moving opens inspection. Still open: recruit browser layout, tree editing, validation messages.
- **Battle screen** (decided 2026-10-06):
  - The player's side is at the bottom and the enemy's at the top, with the two Front rows facing each other in the middle.
  - Board cards are compact: front artwork, HP bar, statuses and KO. Name, classes, Speed, Power and Defense appear on hover and in the inspection panel.
  - Every row uses the same card size. Rows have no labels.
  - **Phones:** only one side of the battle shows at a time, with bigger cards and a button to switch sides (adopted 2026-10-06). Desktop and tablets show both sides. Picking a skill that targets the enemy switches to the enemy side by itself, and the view returns to the player's side once the action has resolved. Every side switch is a smooth flyover: the view scrolls across the battle line to the other side (an instant cut with reduced motion).
  - Cards keep full size up to the width a row can fit; beyond that every card in that row shrinks evenly.
  - The Momentum meter is a vertical bar at the right edge, about 70% of the board's height, with the player's end at the bottom.
  - The timeline is a vertical list on the left, the same height as the Momentum bar (about 70% of the board's height), showing the next 8 entries from top to bottom; the rest are reached by scrolling it. Each entry shows a portrait, the time until the turn, the name and a side color; auto events are darker and show their owner.
  - A summon uses the same compact card with a summon marker; hovering it highlights its summoner.
  - Skills sit in an **MMO-style action bar** (adopted 2026-10-06; a Hearthstone-like hand of cards was compared and not chosen): a framed bar of square icon slots with the skill's name below, a key number (1–9) in the corner, Delay in the bottom corner and the Momentum cost in a gold gem. Move and Skip Turn are round slots set apart at the end of the same frame.
  - A compact card shows up to four status icons, then a +N badge; the inspection panel lists all of them.
  - In the inspection panel and on skill buttons, hovering or tapping a keyword (Poison, `projectile`, Delay) shows a one-line explanation.
  - Damage, healing and absorbed damage appear as short floating numbers, colored and with an icon.
  - Touch targets are at least 7 mm on the test phone (about 110 px at the 1920 × 1080 base). Body text is at least 22 px at base.
  - The target preview is a floating panel with the damage range, the success chance in percent, and the units that may intercept.
  - Status icons carry a letter or short label inside, so they read without color. Final icons come with the art pass.
  - The arrangement in the reviewed mockup is adopted (timeline left, board middle, Momentum right, skill bar bottom, enemy above).
  - **Layout sizes** (adopted 2026-10-06):
    - One 1920 × 1080 base layout, scaled evenly to the screen. The one-side phone view switches on below about 7 inches of screen diagonal; a setting lets the player override it.
    - Board cards on desktop and tablets are 110 × 150 px at base, which meets the touch minimum and still fits six rows. Every card has a 5:7 shape; the compact board card crops the 2:3 art around the creature.
    - Numbers and status letters on cards are at least 22 px at base, like all other text.
    - On 19.5:9 to 21:9 phones the timeline and Momentum bar move out into the side margins, giving the board more room; background art fills the rest.
    - On 4:3 and 16:10 tablets the action bar moves into the bottom band and the board grows into the space it frees.
    - Nothing to tap or read goes into the system's unsafe areas (notches, rounded corners); background art runs edge to edge.
    - Summons such as traps use the same 5:7 card with the summon marker; their art follows the same 1024 × 1536 canvas as creatures.
- Card front/back inspection: all class names, long skill text, scrolling, modified values, source of modifiers, opponent visibility.
- Main menu, settings, collection, exploratory story campaign, unit-unlock presentation, free battle against the AI, and at least one post-story random-battle mode (no rewards). A new player starts with the campaign tutorial; the other menu entries unlock when it is complete (game_system.md §2.2). The battle viewer appears only in development builds.
- Campaign exploration uses **illustrated locations**; battles are played in a fixed order, and the map shows each encounter's creature reward before the battle. The current campaign is a linear sequence of **four stages**; detailed story/location content comes later. Show guaranteed encounter unit rewards and their unlock results; do not add capture interactions for now.
- **Game flow** (adopted 2026-10-07):
  - **First launch.** The full main menu appears with only Campaign available; the other entries are visible but locked until the tutorial is complete.
  - **Main menu.** Campaign · Battle (free battle, random battles after the story, local two-player) · Collection and teams · Shop · Settings. There is no Continue button. How a returning player resumes an open battle or the campaign is still open and is specified separately.
  - **Starting a battle.** After Fight, the board appears with both teams sliding in, then the first turn starts. There is no versus screen.
  - **New creature.** After a win that unlocks a creature, its card flips over large with its name and classes, and Continue moves on. There is no Add to team button.
  - **After a campaign win.** Continue returns to the stage map with the next encounter selected.
  - **After a defeat.** Retry restarts the battle with the same team and formation; a second button opens the team editor first.
  - **Random battles.** The result shows the coins earned and offers Next battle (same difficulty and team) or Exit.
  - **Local two-player.** Player 1 picks a saved team, then player 2; both teams are shown side by side, then Fight.
- **Campaign map** (adopted 2026-10-06 from the mockup):
  - One illustrated screen per stage, with the encounters on a path in their fixed order and arrows to the other unlocked stages.
  - Won encounters are marked done, the next one is highlighted, later ones stay dark; the boss is a larger node at the end.
  - Selecting an encounter opens a panel with its reward creature, the **whole enemy team** (for the next encounter and won ones; locked ones show only the reward), the current saved team with a picker and an Edit team button, and Fight (Replay for won encounters, noting the reward was already received).
  - After the story, challenge encounters appear as extra nodes on the stage maps.
  - The coin balance is always shown in the map's top bar and the main menu.
- During continuous development testing, units start at **level 20 with all skills available through their creature and assigned classes**. **Design classes and skills first; the tree editor and point-spending UI come later.** Do not require a skill loadout or tree purchases for initial testing. The skill bar must expose all available active skills, normally one type-specific attack plus 6–8 additional usable skills (7–9 total), with overflow support for unrestricted initial testing. Later trees mix **10–15 meaningful active skills, passives, and modifiers across 3–4 branches** per class; avoid percentage-only upgrade filler.
- A **surrender** control is available throughout a battle. It asks for confirmation once and counts as a defeat (game_system.md §12.3).
- Show **one contested Momentum meter** indicating advantage toward either party, starting at neutral zero. The meter runs from −10 to +10. It is a vertical bar at the right edge (see Battle screen). Spending previews remain open. A skill whose Momentum requirement is not met shows the reason. Every battle begins with all participating units at full HP.
- Show auto events (such as queued shots) on the timeline with their owner. A unit that is busy until its auto events finish is indicated and takes no turn. Each auto event shows the Momentum shift it will cause when it resolves (game_system.md §6.7). The timeline order follows the tie and same-time rules of §6.1, and a skipped turn is visibly marked as skipped.
- **Forced movement preview** (game_system.md §9.5). Selecting a push, pull or swap shows where the moved unit lands, any units that would be bumped along the chain, and the insertion point for a 50% landing. If the chain is blocked, the preview shows that nothing moves and marks every unit in the chain with the 1 bump damage it would take. Bump damage and constructs moved like units use the normal damage and KO feedback.
- **Delayed `direct` effects.** A delayed `direct` event on the timeline marks its slot (a coordinate in a row, §4.7 and §7.3) on the battlefield, not a unit, so the player can see that it may hit someone else if the target moves away.
- **Stealth feedback.** A unit that loses stealth (by attacking, being hit, or a reveal or cleanse) shows that change clearly. A stealthed unit cannot be selected as a normal target, and area or `direct` previews still highlight it when it is hit (game_system.md §10.1).
- Provide **free respec outside battle** once point allocation is available. Reset/reallocation must respect the later-defined budget and prerequisites; exact controls and dependent-node handling still need specification.
- **Local two-player** (game_system.md §2.4): each player picks a saved team built from the device's collection; both may use the same creatures. There is no secrecy: both teams are shown before the battle. Local two-player is a nice extra for offline players, not a competitive mode. During battle the board flips so the acting player's side is always at the bottom, with a clear cue when the side changes. Inspection pauses the animations as in single-player.
- Prepare interfaces for future online PvP, but **do not implement the online mode or its connected flows/services until explicit owner greenlight**. Planning inspection and shared control boundaries is allowed; online implementation is gated.
- First-time guidance: short tips the first time something new happens, with no guided step-by-step battle (game_system.md §2.2 tutorial). Error/empty/loading states for every included flow.

## Visual acceptance still required

Specify target screen sizes and minimum text/touch dimensions, distinction between cosmetic and gameplay borders, color-independent status cues, action/target selection appearance, damage/heal/status/KO feedback, reduced-motion behavior, and audio equivalents if selected.

Produce annotated layouts before declaring UI specification complete. Claude builds them as clickable HTML mockups (battle screen, team editor, campaign map) for the owner to review; all three were reviewed on 2026-10-06 and their layouts are adopted above. **Visual direction (2026-10-06):** a **night fairytale** look: screens sit on an illustrated fairytale background (night sky, moon, castle, hills, fireflies), with deep plum panels, cream text and gold highlights. A lighter daytime storybook version was tried and rejected. Existing creature images and contact sheets are art references, not approved finished card layouts.

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
