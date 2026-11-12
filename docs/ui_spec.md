# Interface specification

This file owns screen layouts, interaction behavior and visual checks. Game rules are in [game_system.md](game_system.md) (section 13 sets the presentation principles). Reviewed mockups are in [mockups/](mockups/README.md) as reference; this document wins where they differ.

**Visual direction:** a **night fairytale** look. Screens sit on an illustrated fairytale background (night sky, moon, castle, hills, fireflies) with deep plum panels, cream text and gold highlights.

## Cards

- **One art image per creature.** The front crops it to a **portrait** (head and upper body); the back shows the **full creature**. There is no rear art.
- **Front:** the portrait crop fills the whole card, with the four stats in corner gems: HP top left, Power top right, Speed bottom left, Cost bottom right. A dark plate at the bottom shows the **name and the two class names** (names only, no class icons).
- **Back:** the full creature, dimmed, fills the card behind a list of the **unique attack, the six class skills and the two keystones**. Each skill row shows the name, a short type line, a blue Delay gem and a gold Momentum gem; keystones show name and effect. Numbers are the current ones for this creature (its Power, current modifiers).
- **Frame:** every creature uses its **home biome's frame** by default: a shared dark gold base with the biome's motif, round sockets for the four gems and a plain lip for the name plate. Earned cosmetic frames can replace it (production.md §1.2). Frames never mark class, and never hide KO, statuses, target highlights or values.
- Card backs are for inspection, not the action menu. Opposing units can always be inspected.

## Battle screen

### Board

- **Left/right board.** The player's side is on the left and the enemy's on the right. From the left edge inward: player Rear, Middle, Front, the battle line, then enemy Front, Middle, Rear. Each row is drawn as a vertical column; a row's order (its coordinates) runs top to bottom on both sides, and columns stay centred vertically. Move gaps, the forced-movement preview, 50% insertion points and delayed `direct` slot markers use the same mapping.
- **Battle portraits.** Every unit on the board is a round portrait: the whole creature on a plain disc of a colour that contrasts with it (dark plum, or a lighter plum for a creature whose edge is too dark, measured once from the art), inside one shared dark gold ring. Undead (raised undead too) use the undead ring; other summons and constructs a silver summon ring. Player portraits face right, enemy portraits face left (the same image mirrored). Art: [assets/battle-portraits.md](assets/battle-portraits.md).
- A portrait shows **HP only**, as a small round badge with the number at the bottom of the ring. No Power, Speed, Cost, status icons or side colour: the side is told by position and facing. Hovering a portrait adds no tooltip (it only drives the target preview while aiming); details are in the inspection panel.
- **Active unit:** a steady gold glow behind its disc, shown 10% larger.
- **Stealth:** the portrait is semi-transparent with a faint shimmer, shown to both sides, and turns solid with a short flash when stealth breaks. Its turns stay on the timeline. A stealthed enemy cannot be selected as a target; area and `direct` previews still highlight it when it would be hit.
- **Airborne:** the portrait floats a little higher and gently bobs, with faint rising air wisps under it; ground effects under it stay visible.
- **Target highlights:** a glow on the ring, gold for a legal target, white for selected, light for hover; an illegal target is dimmed.
- **Summons:** same portrait in the silver ring; hovering one highlights its summoner.
- **Portrait size:** all portraits share one size, the largest that lets the fullest column fit the board's height with 10 px gaps, capped at about 200 px at base in the both-sides view and about 300 px in the one-side view; six in a column give about 136 px. When the fullest column changes, all portraits resize smoothly. Columns have no labels.
- **Board fill:** about 20 px at base to the top of the screen and to the action bar; a slightly larger gap at the battle line. Faint vertical lines of light run between the columns of each side, a slightly thicker one down the battle line. At battle start the player's team slides in from the left edge and the enemy's from the right.
- **KO and hits:** a KO portrait cracks (a thin light line with a few side branches), shakes once, turns grey with its ring dimmed and sinks slightly; it stays on its spot as a body, with no KO badge. A revived unit returns to full colour without the crack. The inspection panel shows the crack on both card faces. A hit portrait shakes briefly; there is no screen shake.
- **Ground effects** (rules in game_system §12.2): a ground layer between background and portraits. Each spot with an effect shows a round pool about 1.3 times a portrait's size, centred where a unit on that coordinate stands, so it stays put when a column recentres and shows on an empty spot too. Two effects on one spot are drawn on top of each other, half transparent. Pools of the same effect on touching spots merge into one smooth patch. Drawn by shaders plus a few particles (CPUParticles2D, for the Compatibility renderer); no extra art. Burning ground: a flickering orange pool with rising embers; ice: pale cracked frost with a moving glint; water: a dark blue pool with slow ripples; steam: white mist puffs; poison ground: a green pool with bubbles and drifting spores. A new effect spreads out from where the skill lands (about 0.4 s); combos have short transitions (water freezes over, steam bursts up, poison flashes, then fire spreads). Effect damage shows only as the normal floating number. No effect icon, hover info, fading or aiming preview: the timeline's end marker tells when it ends.

### Phones

Only one side shows at a time, with bigger portraits. Picking a skill that targets the enemy switches to the enemy side by itself, and the view returns once the action has resolved. Every switch is a smooth flyover across the battle line (a cut with reduced motion). A **view button** in the top right corner switches manually between both sides and one side at a time; the choice is remembered on the device. It is an icon button in a small framed bar; in the one-side view the bar adds a second button that switches sides (arrow left or right). No "Your side" or "Enemy side" labels. The one-side view is the default below about 7 inches of screen diagonal, from the screen size the OS reports (not DPI).

### Timeline, log and Momentum

- **Timeline:** a vertical list on the left, about 70% of the board's height, showing the next 8 entries from top to bottom; the rest by scrolling, with a visible scroll bar. Each entry shows a portrait, the time until the turn, the name and a side colour stripe. Auto events are darker and show their owner and the Momentum shift they will cause. A unit busy until its auto events finish is marked and takes no turn. A skipped turn is visibly marked. Effect ends show as markers. Order follows the tie rule of game_system §6.1.
- **Battle log:** the timeline extended into the past. A small button on the timeline bar reveals the **last 20 actions** above the upcoming ones, one line each: who, which skill, on whom, and the result ("Wolf used Bite on Bear: 4 damage, intercepted by Turtle"). Tapping or hovering a line shows its rolls and chances.
- **Momentum meter:** a vertical bar at the right edge, about 70% of the board's height, running −10 to +10 with the player's end at the bottom and neutral 0 at the start; no labels. It never shows beyond ±10; a clamped shift shows as clamped.
- **HUD style:** timeline, Momentum meter and view bar look like the action bar: a dark panel with a gold frame; timeline entries are styled like action bar slots (bronze rim, side colour stripe on the left).

### Action bar

- An **MMO-style action bar** at the bottom: a framed bar of square icon slots with the skill's name below, a key number (1–9) in the corner, Delay in the bottom corner and the Momentum cost in a gold gem. It hugs its content.
- **Order:** the unique attack first, then the first class's 3 skills, then the second class's 3 skills, with a small gap between the groups; hovering a slot shows its class name. **Move** and **Skip Turn** are round slots set apart at the right end. Keystones are not on the bar; they are on the card back in the inspection panel.
- Unavailable actions are greyed and say why (wrong row for melee, Momentum lead missing, no legal target, side full, Silenced).
- Every slot has an icon: one per skill and one per unique attack.

### Interaction states

| State | Behavior |
|---|---|
| Reviewing the board | Timeline and unit state visible. Right-click, long-press or the inspect key opens the inspection panel. |
| Active unit awaiting action | The action bar shows the attack, six skills, Move and Skip Turn. Move highlights the gaps the unit can land in. |
| Skill selected | Legal targets, affected units and the timing preview show. A skill with a Momentum cost shows a ghost marker on the meter where it lands after paying. Right-click, Esc, Android back, selecting the same skill again, or tapping empty space on the player's own side cancels; tapping empty space on the enemy side does not. Selecting another skill switches directly. A skill with no target previews on the first press and is used on the second press (or Enter). |
| Target preview | A floating panel with the damage or healing range, the **chance that the skill reaches this target directly** (after interception, in percent) and the units that may intercept. No range text. Mouse: hover previews, click executes. Touch: the first tap previews, a second tap on the same target executes. Keyboard: arrows or Tab cycle targets, Enter executes, I inspects. |
| Resolving | Effects and reactions play automatically; choosing actions is locked; inspection and surrender still work. Enemy turns play straight away with no banner. There is no battle speed setting (only the development viewer has one) and no tap-to-finish. Opening inspection pauses the animation queue, except online. |
| Result | An overlay over the final board shows victory, defeat, draw or surrender, any unlocked creature, and Continue, Retry (after a defeat) and Exit. No per-unit summary. A draw reads as a defeat in single-player modes. |

- **Forced movement preview:** a push, pull or swap shows where the moved unit lands, any units bumped along the chain and the insertion point for a 50% landing. A blocked chain shows that nothing moves and marks each unit with the 1 bump damage it would take.
- **Delayed `direct` effects** mark their slot on the board, not a unit, so the player sees they may hit someone else if the target moves.
- **Floating numbers:** damage, healing and absorbed damage appear briefly, coloured and with an icon.
- **Keywords** (Poisoned, `projectile`, Delay, melee reach) show a one-line explanation on hover or tap in the inspection panel and on action slots.

### Inspection panel

The board stays visible and is not dimmed. The inspected card grows out of the portrait toward the centre and shows front and back side by side. Tapping a row on the back expands it in place with the full text. A changed value shows its current number with the base value small beside it. Statuses show as icons with a letter or short label inside (readable without colour); tapping one shows its name, time left, who applied it and its effect. Tapping another unit closes the panel and opens that unit's; there is no stepping between units.

### Controls

- **Keyboard:** 1–9 pick skills, **M** Move, **S** Skip Turn (no confirmation; first press previews, second press skips).
- **Controller (Windows):** holding a trigger opens a **radial menu** of the active unit's skills with Move and Skip Turn; A picks. The cursor jumps to the first legal target, and the D-pad or stick moves between legal targets only, which previews; A executes. B cancels or closes, Y inspects the card under the cursor, Menu opens the battle menu. Button prompts follow the device used last. The Windows release aims for **Steam Deck Verified**: full controller play and text readable at 1280 × 800.
- **Battle menu:** an icon button in the top left corner (Menu on a controller, Esc when nothing is selected), in the action bar's framed style. It holds Resume, Settings, Exit to menu, and Surrender set apart at the bottom; Surrender asks once. No undo and no restart in any mode. While it is open the animation queue pauses, except online.
- **Reduced motion** removes shakes, sliding and flying effects (the phone flyover becomes a cut); numbers and flashes fade in place. Ground effects still play.

### Layout sizes

- One **1920 × 1080** base layout, scaled evenly to the screen.
- Touch targets at least 7 mm on the test phone (about 110 px at base); all text, numbers and status letters at least 22 px at base.
- On 19.5:9 to 21:9 phones the timeline and Momentum bar move into the side margins; background art fills the rest.
- On 4:3 and 16:10 tablets the action bar moves into the bottom band and the board grows into the space it frees.
- Nothing to tap or read goes into the system's unsafe areas; background art runs edge to edge.

## Team editor and collection

- **Team editor**, like a deck builder: pick up to six creatures from the collection (each at most once) within the team budget of 35, place them in rows, and the team is saved under a name. Saved teams are chips across the top with the team name; the formation is on the left; the collection is a panel on the right, a drawer sliding in from the right on phones.
- Creatures are added and rearranged **by dragging only**: from the collection into a row, between rows, or out of the team to remove. On touch a drag starts as soon as the finger moves; a long-press without moving opens inspection.
- Changes save automatically; there is no Save button. The selected chip has a small menu with Rename, Copy and Delete; Delete asks once. A team may have one to six creatures; wherever teams are picked (chips, battle team picker, campaign team picker), a team with fewer than six shows a warning icon. The last used team is preselected.
- **Filter and sort:** filter by class; sort by name, HP, Speed, Power or Cost.
- **Collection:** the team editor's collection panel is the collection; there is no separate gallery. The "Not owned" filter shows missing creatures as dark silhouettes with where to get them: the encounter's name, or the coin price. A coin creature's silhouette opens its card with the price and Buy; the price is red and Buy unavailable when the player has too few coins, with no prompt to buy coins. Buy asks once. A newly unlocked creature has a "New" badge until inspected.
- **Achievements:** a page inside Collection and teams lists every achievement with its progress. A newly earned one shows a small toast; one earned during a battle waits for the result overlay. No statistics page and no per-creature records.

## Shop

One page that sells only coin packs (production.md §1.2), available from the first launch. Coin creatures can be bought after the story. The coin count is always shown in the main menu, the collection, the Shop and the campaign map's top bar. Offline, a small mark sits on the coin balance, and Online, Shop and coin-creature Buy buttons say "Needs connection".

## Game flow

- **First launch.** The full main menu appears with only Campaign and Shop available; the other entries are visible but locked until the tutorial is complete.
- **Main menu,** which doubles as the title screen with the logo above it: Campaign · Battle (free battle, random battles after the story, local two-player) · Online · Collection and teams · Shop · Settings. No Continue button.
- **Resuming.** Launching always opens the main menu. An unfinished battle is kept, and its mode's menu entry (Campaign or Battle) shows a Resume marker. Exit to menu leaves a battle saved; starting another battle first asks to give the unfinished one up (it then counts as surrendered). Only one unfinished battle is kept. Local two-player battles resume like any other; online battles cannot be left this way. A battle whose content changed in an update is dropped with a short note (production.md §1.4). Campaign opens the chapter of the next unplayed encounter with it selected.
- **Starting a battle.** After Fight the board appears with both teams sliding in, then the first turn starts. No versus screen.
- **New creature.** After a win that unlocks a creature, its card flips over large with its name and classes, and Continue moves on.
- **After a campaign win,** Continue returns to the chapter map with the next encounter selected. **After a defeat,** Retry restarts with the same team and formation; a second button opens the team editor first.
- **Random battles:** the result shows the coins earned and offers Next battle (same difficulty and team) or Exit.
- **Local two-player:** player 1 picks a saved team, then player 2; both teams are shown side by side, then Fight. There is no secrecy. During battle the board flips so the acting player's side is always on the left, with a clear cue when the side changes. Inspection pauses animations as in single-player.
- **Tips:** a small storybook note with an arrow pointing at the thing it explains, the first time its mechanic happens in play (first status, first interception, first KO, first melee blocked by the row rule, and so on). Tapping anywhere closes it; each tip shows once; Settings has Reset tips. There is no guided step-by-step battle.
- **Loading** shows a plain spinner. Other error and empty states are specified with each flow.
- **Battle viewer** (development builds only): runs AI-versus-AI battles on the card UI, also to debug UI issues. It can start a battle with the same teams and formations as any recorded battle (not an exact replay). Controls: pause, step one action, speed (1x, 2x, instant), inspect any unit, pick a recorded setup (production.md §2.3). It shows only what a player would see.

## Campaign map

- A **world map** shows the chapters as regions; locked chapters are dark and misty, their names hidden until reached. Tapping a chapter opens its map.
- One illustrated screen per chapter, with the encounters on a path in their fixed order and arrows to the other unlocked chapters. The castle (chapter 3) is visible in the distance on the earlier maps.
- Won encounters are marked done, the next one is highlighted, later ones stay dark; the boss is a larger node at the end. After the story, challenge encounters appear as extra nodes.
- Selecting an encounter opens a panel with its reward creature, the **whole enemy team** (for the next encounter and won ones; locked ones show only the reward), the current saved team with a picker and an Edit team button, and Fight (Replay for won encounters, noting the reward was already received).

## Online

Rules in game_system §2.4.

- **Online screen:** the saved-team picker, Quick match, Private match (open one and share its code, or join with a code) and the player's online wins and losses. The rating is never shown.
- **Searching** shows the search time and Cancel; nothing else can be done meanwhile.
- **Match found:** no versus screen; the board appears with both teams sliding in, each player's platform name on their side.
- **Turn timer:** a ring runs down around the acting unit's timeline portrait and turns red at 10 seconds.
- **Opponent disconnected:** a banner says the opponent is reconnecting and counts down their 1 minute.
- **Emotes:** a small speech button beside the battle menu button opens Hello, Well played and Thanks; an emote appears as a bubble on the sender's side.
- **Battle menu online:** Settings, Mute emotes, and Surrender set apart; no Exit to menu, and it does not pause the battle.
- **Result:** win or loss, coins earned, Rematch (starts when both accept), Find next match (same team) and Back.
- **Sync** is silent unless a merge from another device added something; then a short note says what.

## Interaction checks

- The card back shows each skill's actual current numbers for its creature.
- Selecting a skill previews the same scheduling consequence that execution produces when nothing else changes.
- Selecting a legal target executes without a further confirmation.
- An ordinary attack leaves the formation unchanged unless the skill moves a unit.
- The forced-movement preview matches the executed result, including blocked chains and their bump damage.
- A delayed `direct` event's marked slot is the slot it will hit; a queued projectile's preview shows the unit behind a stealthed or KO target when relevant.
- The interception chance shown in the target preview matches the rules of game_system §8.2 for the current board.
- A melee skill or attack is unavailable, with its reason, while its unit is outside its Front row.
- Party setup and combat use the same ordering and centering rules.

Complete these for mouse, touch, controller and the target screen sizes, including dense states (many statuses, long names, a full side of 18 units).
