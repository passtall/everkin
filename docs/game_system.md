# Everkin — Game Design Document

**Version:** 0.4 — Class design rules: tags, damage formula, Momentum scale, interception chances

**Updated:** 2026-10-03

**Format:** Digital tactical card game with creature collection and RPG character builds

**Project name:** Everkin is a working title.

**Companion documents:** [UI specification](ui_spec.md) owns screens and interactions. [Classes and skills](classes_and_skills.md) owns the class and skill definitions. This document owns game rules, scope, and implementation requirements.

**Current work:** define the whole game before gameplay implementation. Classes and skills are designed one class at a time, with about ten questions per round, and the answers are applied directly to these documents. Keep unresolved choices explicit; no separate decision history is maintained.

## 1. Design vision

> Build your perfect fighting party from everything the world has to offer.

Everkin centers on collecting and combining humans, animals, and fantasy beings into a highly customizable combat team. Every recruit belongs to the same party system. Humans are combatants alongside other creatures, rather than a separate trainer category.

The player expresses strategy through team selection, skill-tree investment, formation, and individual combat actions. A visible, manipulable turn timeline and meaningful positioning create the tactical identity of the game.

Everkin is now a **card game as its intended product format**, rather than a card prototype awaiting a 3D conversion. Characters appear as illustrated cards. **No 3D assets will be created.** Hearthstone is a reference for the accessibility and appeal of a card-based presentation, not a specification to copy its rules, interface, artwork, or branding.

The established combat model remains skill-driven: cards represent persistent team members, and those members use their learned abilities. The presentation change does not introduce a shuffled deck, hand, random action-card draws, or a per-round mana system. Those systems were not selected in the discussions.

### 1.1 Design pillars

| Pillar | Design requirement |
|---|---|
| Team freedom | Humans, ordinary animals, and fantastic creatures are all valid party members. |
| Build diversity | Species, accessible skill trees, point allocation, and formation produce distinct builds. |
| Tactical clarity | Players can understand legal targets, protection, movement, and turn-order consequences. |
| Meaningful timing | Speed and skills affect when units act; combat does not rely on a fixed once-per-round sequence. |
| Meaningful formation | Position changes access, protection, area effects, and skill behavior. |
| Few required decisions | A turn asks only for the choices needed to execute the action. Reactions resolve automatically. |
| Character variety | Tiny animals, humans, unusual fantasy beings, and immense creatures belong in the same collection. |
| Achievable production | Reusable systems, readable artwork, and a focused playable core take priority over content volume. |

### 1.2 Document status

This is the current game design, still being completed. **Established** rules are selected; **proposed** rules and example numbers require a decision; **open** items remain undefined; **deferred** features are outside the current scope unless explicitly included later.

An agent must not invent missing gameplay rules. Before implementation, settle the scope, numerical rules, content, and UI behavior of every included feature. Examples illustrate behavior without fixing balance values unless explicitly adopted.

## 2. Player experience and game loop

### 2.1 Core combat loop

1. Review available recruits and their characteristics.
2. Select a party.
3. Allocate skill points across each member's available trees.
4. Set the starting formation.
5. Fight using unit skills, positioning, and the turn timeline.
6. Evaluate the result and revise the party, builds, or formation.

The immediate playable experience is a team-building and combat game. Combat must be enjoyable before a larger progression or narrative structure is added.

### 2.2 Collection and progression loop

**Design intent:** discover or unlock recruits, gain access to new combinations, improve builds, and face encounters that reward different strategies.

Players explore a **story campaign presented as illustrated locations with selectable paths and events**. For now, use a **linear sequence of four stages**; detailed campaign content and structure will be specified later. The four-stage sequence is the current testing scope, not a final campaign-length commitment.

Units unlock through **guaranteed encounter rewards** for now. Exact encounter-to-unit assignments and repeat-reward behavior remain open. A mixture of acquisition methods may be considered later, but capture or other recruitment mechanics are not currently required.

After completing the story, at least one **random-battle mode** provides further unit unlocks and continued play. Random-battle generation, difficulty, and reward rules remain to be specified.

There is no established collectible-card business model. The use of cards does not itself imply booster packs, paid randomized acquisition, trading, or duplicate conversion.

### 2.3 Platforms and controls

The required platforms are **Windows, Android, and iOS**, with **landscape orientation only**. Desktop and touch readability must influence the design from the start.

Minimum OS/device requirements, supported aspect ratios, controller support, and distribution remain open. Required platform support must be verified during implementation; it is not an existing implementation milestone.

### 2.4 Modes and development order

- **AI versus AI:** the first development/testing mode, allowing both parties to play automatically for easier combat testing.
- **Single-player versus AI:** required, including the story campaign and at least one post-story random-battle mode.
- **Local two-player:** required; setup, controls, and collection access still need specification.
- **Online PvP:** planned, but **do not implement it until the owner explicitly greenlights it**. Prepare code interfaces and architectural boundaries for future online play. This preparation is not authorization to implement networking, matchmaking, or online services.

The AI-versus-AI milestone comes first once gameplay implementation begins. The current task remains defining the game before implementation.

## 3. Party composition and creature identity

### 3.1 Party baseline

The current intended battle size is **six active team members versus six**, placed freely among three rows. All starting members participate; there is **no reserve-member swapping during combat**. Moving or swapping battlefield positions is a separate mechanic and is permitted when its rules allow it.

Six-unit party readability must be tested in the actual prototype. If combat becomes crowded or difficult to parse, reducing the standard party size to four or five should be considered. This remains a prototype readability question, not a finalized reduction.

Whether a player may deliberately begin with fewer than six members remains open, especially in relation to summons. Enemy party-size exceptions and boss composition have not been finalized.

No class dictates a mandatory row. The system should support unusual but effective combinations, including defensive small creatures or an unconventional back-row guardian.

### 3.2 Three layers of a unit

| Layer | Purpose |
|---|---|
| Creature type / species | Defines the unit's underlying identity, base characteristics, body concept, and natural traits. |
| Accessible classes / skill trees | Defines the development options available to that unit. |
| Build | Defines how its limited skill points are invested and how it functions in the chosen team. |

Species and class are independent concepts. A Hunter could be a wolf, human, or fantasy creature. A Guardian could be a human, turtle, or stone being. A creature's appearance is not a reliable declaration of its class or combat role.

### 3.3 Species consistency

For the initial system, creatures of the same type with the same build have the same stats under equivalent progression and combat conditions. Hidden individual stat rolls or genetic values are not part of the starting design.

A later system of visible natural variation or special traits remains a possibility. It is not a current requirement.

### 3.4 Human individuality

Humans are intended to feel like individuals. Random names, stat variation, and assigned combinations of skill trees were specifically discussed.

**Open, with a preference expressed toward static content:** humans may be generated or rolled during content creation and then stored as fixed recruits, rather than generated repeatedly at runtime. This would retain individual identities while keeping recruitment and balance controllable.

Related proposals to retain for evaluation:

- Keep stat differences bounded rather than producing unusable recruits.
- Randomize the selection of available trees, not the contents of the trees themselves.
- Make relevant information visible before recruitment.
- If runtime generation is used, persist candidates rather than reward repeated reloads or rerolls.
- Allow unusual combinations while ensuring each recruit has a viable use.

The suggested ±10–15% stat variation was an example, not an approved range. Human generation remains an exception under consideration rather than a reason to add random stats to all creatures.

### 3.5 Variants and separate creatures

A **variant differs only in color**. A larger body, changed anatomy, altered silhouette, or distinctive physical features identify a **separate creature**.

For example, a wolf and an alpha wolf are separate creature types if they differ in size or shape. Gray, white, and black presentations of the same wolf are color variants. No independent stat advantage for a color variant has been specified.

### 3.6 Weapons and equipment

The current direction avoids a general gear system. Some humans and other creatures may carry weapons, tools, or props dictated by their type. These are part of their identity and artwork rather than evidence of an interchangeable equipment or loot system.

## 4. Classes and skill-tree progression

### 4.1 Access and point budget

Each creature can access **one to three classes**, each represented by its own skill tree. Humans are generally versatile and may have three; a wolf may have only one.

Every unit has a **single skill-point budget shared across its trees**. Access to additional trees provides more choices, not extra points.

**Current working progression model:** units range from **level 1 to level 20**, with **20 skill points total at level 20**. A class tree contains **10–15 entries split into 3–4 branches**, mixing active skills, passives, and skill modifiers. Individual costs/ranks and prerequisites are deferred with tree design.

The proposed award rule is one point per level gained, with two for the last level. To total 20 points, this corresponds to zero points at level 1, one for each level-up to levels 2–19, and two on reaching level 20. This arithmetic interpretation needs confirmation before normal progression is finalized. Experience requirements, stat growth, and the broader progression system remain deferred for later design.

**Continuous testing:** every test unit starts at **level 20**, with the planned full budget of **20 skill points**. **Design classes and skills first; trees and point spending come later.** For now, each unit has **all skills available to its creature and assigned classes**, without tree purchases or a separate loadout selection. This does not grant skills from unrelated classes. Do not block initial testing on unfinished costs, ranks, prerequisites, or tree topology. Mutually exclusive or conflicting effects must be resolved in their skill definitions rather than assumed to stack.

**Respec is free outside battle.** Players may reset and reassign skill points without a resource cost. Once trees exist, reallocation must obey their defined prerequisites and budget; exact dependency handling belongs to the later tree design.

### 4.2 Specialization and hybrid builds

A unit may specialize in one accessible tree or distribute points across several. A three-tree unit must still choose what to become good at; it cannot fully develop all three trees through an increased budget.

**Proposed tree structure:**

- Multiple branches within a tree support different builds even for a single-class creature.
- Strong upper-tier abilities require meaningful investment in that tree.
- Low-investment nodes should not make identical shallow multiclass builds universally optimal.
- Species characteristics provide identity independently of tree access.

The earlier proposal of six class ranks with automatic single-class mastery was replaced by the shared skill-point model.

### 4.3 Node types and meaningful effects

| Node type | Function |
|---|---|
| Active skill | Adds an action available in combat. |
| Passive | Adds meaningful behavior, conditional effects, or rule interactions rather than generic stat-only increases. |
| Modifier | Changes an existing ability rather than adding another unrelated button. |
| Keystone | Produces a major, build-defining change. |

For example, a marking ability could develop toward ranged-damage support, timeline delay, or transfer to another target on death. These are design examples, not implemented skills.

Trees mix active skills, passives, and modifiers with **meaningful mechanical effects**. Do not fill them with generic percentage-only upgrades such as **+10% damage**. Entries should change what a unit can do or how an ability works—for example its targeting, movement, timing interaction, trigger conditions, status behavior, or interaction with other units. These are design directions, not approved individual skills.

Numerical values still define and balance real effects; the restriction is against stat-only filler as a progression reward. Create the actual skills and effects first, then organize them into trees later.

### 4.4 Skill availability in combat

Each unit begins with **exactly one type-specific attack**, such as Bite, Sword Slash, Punch, or Arrow. Through its skill trees, a unit should gain **6?8 additional usable skills in total across its classes**, not 6?8 per class. **All unlocked skills are available in combat; there is no separate loadout or equipped-skill cap.**

The intended developed unit therefore has **7?9 usable skills including its type-specific attack**. This is a content-design target, not a hard cap. Initial testing still grants all available creature/class skills before trees exist, so temporary test units may exceed that target. Passives and modifiers are mixed into the trees but do not each require a separate action button.

There is **no separate Basic Attack system**. Bite, Claw, Shoot, and comparable attacks are ordinary skills. Detailed tree topology and node design were deliberately deferred.

### 4.6 Skill costs and prerequisites

**No mana. No cooldowns.** Skills are balanced through timeline costs, shared party Momentum expenditure, or other consequences: HP loss, sacrifice, unfavorable movement, self-inflicted statuses, harm or disruption to allies, and changes to later actions. Many of the strongest attacks and abilities require Momentum; its battle-wide rules are defined in section 6.7.

Skills may have specific requirements involving position, range, statuses, HP thresholds, allies, formation, free space, KO targets, or earlier actions. The UI must show the concrete reason whenever a skill is unavailable.

There is no global anti-spam rule. A skill's consequences and the resulting combat state must make repetition an interesting choice. Skills may modify their own future behavior, become faster or slower, accumulate recoil, transform into another version, or consume a required state. These are individual skill definitions rather than a replacement universal cooldown system.

**Design principle:** rules that do not require global consistency belong in the individual skill or effect definition. This includes effect order, costs, targeting exceptions, and behavior when the user or target becomes KO.

### 4.5 Current class roster

The classes in the current scope are **Hunter, Elementalist, Healer, Warrior, Guardian, Feral, and Leader**. Their skill sets and mechanical identities are being designed again from scratch, one class at a time in [classes_and_skills.md](classes_and_skills.md). Healer is first, then Leader. Do not add other classes without an explicit scope change.

Class skills use the **same rules across creature types**. A class ability does not acquire a species-specific version merely because a turtle rather than a human uses it. Outcomes can still differ through the user's stats and explicitly defined build effects.

Creature identity contributes one type-specific starting attack, such as Bite, Sword Slash, Punch, or Arrow. It remains an ordinary skill using the same timing, targeting, and resolution systems as other attacks.

### 4.7 Skill cards, tags and design rules

**Skill card.** Every skill has:

| Field | Meaning |
|---|---|
| Name | Unique. |
| Class | The class it belongs to. |
| Tree slot | Branch letter and number, such as A1 (position in the class tree). |
| Delay | 1–10, **1 is quickest**. Sets the wait before the user's next turn (section 6.1). |
| Momentum cost | 0–10, most skills 0 (section 6.7). |
| Tags | The tags below. |
| Mechanic | Exact rules, written with tags. |
| Reach | Rows and distance it can affect. |
| Hidden stats | Designer-only numbers, such as the damage or healing coefficient, from which the displayed range is calculated (section 5.2). |

**Other tree nodes** (passive, modifier, keystone) have a name, class, tree slot, prerequisite, trigger and effect. A node must change targeting, timing, movement, triggers, status behavior or interactions. **No node may only add numbers** (such as +10 damage).

**Tags.** Skills have tags only, no categories. Tags are based on function and exist so skills, statuses and passives can react to each other. Add a tag only when a rule needs it.

| Group | Tags | Rule |
|---|---|---|
| Damage type | `physical`, `magical` | Interact with Physical or Magic Defense. |
| Delivery | `projectile` | Travels the battlefield from the user to the target, so it passes the units in front of the target and can be intercepted by them. |
| | `melee` | Needs reach to the target. Triggers effects that react to melee. Can be intercepted. |
| | `direct` | Hits the selected target itself, with nothing passing between. **Ignores interception.** Still needs a legal target and reach. |
| Targeting and shape | `single` | The player selects one unit; only that unit is hit. |
| | `column` | Hits the target and the units behind it (section 8.4). |
| | `row` | The player selects a **row** (the UI highlights it) and the skill affects the units in it. |
| | `circular` | Hits the target and the units adjacent to it. |
| | `random` | Picks its target at random among legal targets. |
| | `chain` | Hits the selected target, then jumps to further targets by the skill's own rule. |
| | `all` | Affects every unit of one side that the skill's target type allows. |
| Function | `attack`, `heal`, `status`, `summon` | What the skill does. |
| | `move` | Repositions units. |
| | `cover` | Redirects or absorbs hits for other units. |
| | `timeline` | Moves turns or events on the timeline. |
| | `delayed` | Puts an event on the timeline that resolves later. |
| | `momentum` | Moves the Momentum meter directly. |
| Element | `fire`, `frost`, `storm` | Properties other skills and statuses can react to (for example a `fire` hit breaking a freeze). They are not extra defense categories. |

**Targeting tags decide what the UI lets the player select.** A skill selects a unit unless it has a tag for another target type. Only skills with `row` can select a row. Any future target type (a column, a slot) needs its own tag. Skills never silently gain other target types.

The tag list is a starting point. Exact shape definitions (for example which units count as adjacent for `circular`) belong in section 8.4 when decided.

**Design rules:**

- **Every skill has its own role.** Two skills of one class must not do the same job.
- **Realistic positioning.** A projectile must pass the units in front to reach those behind. A unit far from an ally cannot cover that ally. A skill that crosses distance must say how, for example a leap that moves the user (`move`) and then covers.
- **No skill has a minimum distance** unless it would be physically unrealistic.
- **Classes declare no defenses.** Defense comes from the creature type, skill tree choices, and explicit effects.
- **Every skill has a reach.** Cover, redirect, swap, push, and pull skills only work on units within their reach, and an attack can only be redirected to a unit that could legally be its target.
- **Numbers stay low.** Players see whole numbers only.
- **Every skill respects interception by default** (section 8.2). Only the `direct` tag, or a skill that states it explicitly, ignores it.

## 5. Unit statistics and damage

### 5.1 The six global stats

| Stat | Definition |
|---|---|
| HP | Maximum health; current HP is tracked during combat. |
| Speed | 1–10, **10 is fastest**. Determines how quickly the unit completes its turn cycle (section 6.1). |
| Physical Power | Scaling basis for physical skill damage. Each creature type defines its own value. |
| Magic Power | Scaling basis for magical skill damage. Each creature type defines its own value. |
| Physical Defense | Percentage reduction of physical damage. |
| Magic Defense | Percentage reduction of magical damage. |

Defense may be **0%**. There is no universal built-in defense allowance: defense comes from the creature type, skill tree, or explicit effects. Display it as the actual percentage reduction rather than an opaque rating. Units may be strongly specialized in physical or magical power.

Accuracy, evasion, critical chance, critical damage, mana, universal resistance, and healing power are **not additional global stats**. Skill-specific hit chances, defensive effects, or healing scaling may be defined without expanding the stat sheet. Mana and critical-hit systems remain excluded.

### 5.2 Damage scaling

Each damaging skill has a hidden **coefficient range**. The displayed damage range is that range multiplied by the user's relevant Power stat.

Illustrative definitions:

- Bite: 80–100% of Physical Power.
- Heavy Slam: 150–190% of Physical Power.
- Arc Bolt: 90–120% of Magic Power.

These values demonstrate the selected model; they are not approved balance data. A skill may explicitly scale from HP, defense, or multiple stats, but that exception must be part of its definition.

**Adopted damage formula:** each creature type defines its own attack power (Physical Power and Magic Power). A skill's damage is its hidden coefficient range multiplied by the user's relevant power, rolled uniformly, and the skill tree may change the formula. Players always see **whole numbers only**, shown for the actual user. Damage is **rounded down, with a minimum of 1**, unless a skill explicitly negates it. Numbers in the class documents assume a **reference human with attack power 5**.

**Physical and Magic are the only baseline damage categories.** Physical damage interacts with Physical Defense; magical damage interacts with Magic Defense. Fire, frost, poison, bleeding, mental effects, and similar themes use properties or statuses rather than additional global resistance categories. Defense bypass is a property of a skill, not a third defense stat.

Damage is sampled **uniformly within the skill's damage range**. Each value is equally likely; the range itself defines the variance. Some skills may have a very wide range. There are **no separate critical hits or critical multipliers**; a high roll can provide the excitement of a critical hit without another system.

Defense then reduces the appropriate raw damage by its stated percentage, and the result is rounded down with a minimum of 1. Defense bounds and interactions with penetration and shields remain open. Interception is defined in section 8.2. Target previews should show the resulting damage range against the selected target wherever determinable.

### 5.3 Hit reliability

An ordinary attack hits a valid target by default, subject to applicable defenses and explicit effects. Random hit chance is an explicit property of selected risky skills, not a universal accuracy/evasion roll. A status or skill may introduce an exception, such as blindness affecting attacks that require sight.

Status application is separate: a status-applying skill states its own status chance. Successful damage does not automatically guarantee a status unless that skill says so.

### 5.4 Stat sources and growth

The discussed sources of values are creature identity, build investments, and temporary combat effects. Level growth and the relative contribution of levels versus skill trees remain open.

Healing scaling, damage-over-time calculation, status durations, stacking behavior, and cleansing belong in the relevant skill or effect definitions. Critical hits are excluded from the design.

### 5.5 HP and damage anchors

HP anchors: a critter (the lowest HP in the game, such as a fox) has **20**, a human **35–45**, and a giant stone turtle **60**. Damage anchor: a critter with 0% Physical Defense dies to exactly **three Sniper Shots** from the reference human (Sniper Shot deals 7–9). Keep all numbers low.

## 6. Turn timeline and action execution

### 6.1 Shared timeline and turn progress

Everkin uses a **single shared timeline for all friendly and enemy units**, inspired by the tactical function of FFX's conditional turn order. There are no separate team turns or traditional fixed rounds. Units act whenever their position on the timeline is reached.

Turn frequency depends on both the unit's **Speed** and the **Delay** of the skill it used on its previous turn. A fast unit using quick skills may act many times before a slow unit using heavy skills acts again. This is intentional and part of each unit's balance and identity; there is no artificial limit on how many turns one unit may receive before another acts.

**Two scales, both 1–10:**

| Scale | Belongs to | Meaning |
|---|---|---|
| **Speed** | Unit stat | **10 is fastest**, 1 is slowest. |
| **Delay** | Skill | **1 is the quickest** skill, 10 the heaviest. |

Speed and Delay have **equal effect**: one point of either changes the wait by one time unit.

```text
Wait (time units) = Delay + (11 - Speed)
```

| Unit Speed | Skill Delay | Wait |
|---|---|---:|
| 10 | 1 | 2 |
| 10 | 5 | 6 |
| 5 | 5 | 11 |
| 1 | 1 | 11 |
| 1 | 10 | 20 |

The fastest possible turn cycle (Speed 10, Delay 1) is **10 times shorter** than the slowest (Speed 1, Delay 10). With one unit on each side, the fast unit acts about ten times before the slow one acts again. Any Speed/Delay combination can be read directly from the formula.

Internally the system tracks **turn progress** in time units. When a unit acts, its skill and current Speed fix the Wait. The turn arrives when that Wait has elapsed. Reactions and auto events do not use this formula unless their definition says so.

- **Haste / Slow** change Speed by a stated number of steps. Remaining Wait changes by the same number of time units; completed progress never changes.
- Speed effects expire the same way: the remaining Wait is adjusted by the reverse amount. Speed stays within 1–10 for this formula; a technical minimum Wait of 1 prevents infinite loops.
- Multiple Speed changes may repeatedly reposition a unit on the timeline.

The same model supports direct timeline manipulation:

| Effect | Progress behavior |
|---|---|
| Haste | Raises Speed: shortens the remaining Wait. |
| Slow | Lowers Speed: lengthens the remaining Wait. |
| Stop | Temporarily prevents progress. |
| Pull Forward | Directly removes time units from the remaining Wait. |
| Push Back | Directly adds time units to the remaining Wait. |
| Reset | Resets current turn progress. |
| Turn Now | Immediately completes the Wait. |

(The timeline effect formerly called Delay is now **Push Back**, because Delay is the skill scale.)

All friendly and hostile units are always evaluated within this same timeline. Units must be balanced around their actual action frequency.

The UI shows multiple upcoming turns and previews where the acting unit's next turn would move when a skill is selected, before confirmation through target selection. Other foreseeable timing changes should also be previewed whenever practical.

**Open:** initial initiative and initial progress, tie-breaking between units, whether Haste/Slow steps are the right granularity, precise handling of direct progress changes, and safeguards against indefinite action denial. No fixed numerical manipulation cap has been selected.

### 6.2 Standard action sequence

1. The timeline identifies the acting unit.
2. The player chooses a skill.
3. The player supplies only the target or other input required by that skill.
4. The action resolves immediately.
5. Automatic effects and the updated combat state are reflected in the timeline.
6. The next eligible unit acts.

The timeline determines **who acts**, not a general queue in which already-selected actions wait to resolve. An animation may take time to play, but it does not open additional tactical decision windows.

The timeline normally displays **turns, not intended attacks**. Enemy skill choices are not automatically revealed in advance.

**Delayed events are supported exceptions.** Once a skill creates a delayed effect, such as a comet impact, it appears as its own visible timeline object. An event can be manipulated only when its definition allows that. Each skill specifies whether the effect follows a unit, remains on a slot or area, or uses another condition, including what movement and KO do to it. A delayed event is distinct from an enemy's unchosen future action.

**Auto events.** A skill can put an auto event on the timeline, occupying a position like a unit's turn. Later effects can move it. When it reaches its turn it executes automatically. If its owner is KO, it is removed. If its target is KO when it executes, it targets units behind that target. A skill can make its auto events **blocking**: the owner takes no turn until all of them have resolved or been removed, and the owner's own turn cost starts after the last one. Haste or Slow on the owner changes only the remaining wait for pending auto events.

### 6.3 Minimal decisions

The desired interaction is **skill → target → resolution**, or **skill → resolution** when no target input is necessary.

Parry, counter, interception, thorns, and comparable reactions trigger automatically when their conditions are met. Their strategic control comes from builds, positioning, prior skills, or active effects, not repeated confirmation prompts.

If an action has meaningfully different modes, a distinct skill or directly selectable mode is preferable to a chain of follow-up questions. **There are never decision popups during the enemy turn.**

### 6.4 Required actions and skipped turns

A unit must use a viable skill or reposition when it has a viable action. There is **no Wait command and no universal Defend command**. Defensive actions belong to specific creature or class movesets.

If no viable action exists, the turn is automatically skipped and the timeline continues. There is no requirement to invent an always-available emergency attack. The precise scheduling cost of this skipped turn still needs implementation detail.

### 6.5 Event resolution and KO checks

Skills define a fixed order for their effects. **Check KO after each event.** Moving before damage and moving after damage may intentionally produce different outcomes; meaningful order must be visible in the skill description or preview.

A skill may declare a **simultaneous event block**. Apply the whole block together, check its resulting KO state, then collect and process the resulting reactions. Internal slot iteration must not change who receives effects intended to be simultaneous.

Each skill determines whether it continues or stops when its user becomes KO during resolution. Each hit of a multi-hit skill is an event with its own KO check; the skill defines whether remaining hits expire, retarget, or follow another explicit rule.

### 6.6 Reaction chains

Reactions are additional effects, not automatically normal turns. They change the timeline only when explicitly defined to do so.

Each reaction specifies whether it can trigger further reactions. Clear limits must prevent endless chains. Proposed safeguards are no self-recursion, at most one firing per concrete reaction instance in a chain, and a global chain limit. The exact numeric limit is open; eight or ten were examples, not selected values.

Simultaneously triggered reactions use a small set of priority levels. Equal priorities resolve according to their owners' current timeline order, with explicit skill exceptions allowed. High/Normal/Low was an example classification; final categories and a secondary tie-break when owners have equal timeline positions remain open.

### 6.7 Momentum and the battle clean slate

**Momentum is a single battle-wide contested meter shared by both opposing parties, not two independent party pools.** It represents which side currently has the flow of battle in its favor.

**Every battle starts with all participating units at full HP and the Momentum meter at neutral zero.** Momentum and all other temporary combat states never carry over between battles.

Momentum represents the current flow of battle, rather than a resource generated by fixed values on individual skills. A party's successful attacks generally move the meter toward that party's advantage; its failed attacks and successful enemy attacks generally move it away. The two sides contest the same meter.

Fast creatures naturally build Momentum effectively because they can perform several fast actions in a short span of timeline time. If those attacks succeed, they can quickly shift Momentum in their party's favor.

Many of the most powerful attacks and abilities require Momentum to use. This connects fast units that build Momentum through repeated successful actions with powerful units or abilities that spend the accumulated Momentum for high-impact effects.

Momentum gain is not an explicit **Momentum Gain** stat attached to skills. Adopted rules:

- The meter runs from **−10 to +10**, with neutral at 0.
- A damaging hit that lands moves it **1** toward the acting side. A KO gives no bonus.
- A missed attack moves it 1 against the attacker. A hit that is intercepted moves it **0**.
- A skill or status may modify the shift (for example, double Momentum for hits on a marked target).
- A skill may have a Momentum cost from **0 to 10**. It requires the user's side to be at least +N ahead and, when used, pushes the meter **N toward neutral**. If the requirement is not met, the UI states it.

Still open: behavior at the ±10 limits, safeguards against excessive snowballing, and how periodic, reaction, and simultaneous effects change the meter.

**Core principle:** successful combat builds Momentum, mistakes and enemy success erode it, and powerful actions often consume it.

## 7. Battlefield and formation

### 7.1 Centered rows and capacity

Each side has three horizontal rows: **Front, Middle, and Rear**, separated from the opponent by a central battle line. There are **no visibly fixed card slots**. Units within each row are automatically centered and placed directly beside one another without gaps, similar to Hearthstone's automatic minion positioning. All normal cards have exactly the same width.

Each row may contain at most **six normal units**. Regular six-unit parties are not expected to fill all three rows; additional capacity mainly supports summons and other temporary units. The player may place all six party members in one row, with no required distribution by class.

Eighteen normal units per side is only the theoretical sum of the row capacities. If that becomes excessive, an overall cap of approximately **twelve or fifteen** may be adopted after prototype readability testing. Neither alternative is fixed yet. Standard party size and total battlefield capacity are separate limits.

### 7.2 Starting formation

Starting position is a deliberate pre-battle choice. The game must not randomly rearrange the party through surprise formations or arbitrary encounter overrides.

Enemy formations and encounter mechanics may create different tactical demands. Position changes during combat must be an understandable consequence of actions or explicit effects.

### 7.3 Horizontal alignment and ordered positions

Equal-width, gapless, centered cards in different rows have only three relevant horizontal relationships: **100% overlap, 50% overlap, or no overlap**. Automatic centering prevents arbitrary intermediate overlap values.

Rows with card counts of the same odd-or-even parity can align directly. When one row has an odd count and the other an even count, their positions are offset by half a card width. This creates an **invisible half-card-width positioning grid**, without exposing explicit slots to the player.

A derived coordinate model uses one half-card width as a unit. For a row of `n` cards and zero-based ordered index `i`:

```text
Card center = 2 ? i - (n - 1)
Card width = 2
Overlap fraction = max(0, 1 - abs(centerA - centerB) / 2)
```

A three-card row has centers `-2, 0, +2`; a two-card row has `-1, +1`, producing half-overlaps. Against a one-card row centered at `0`, only the middle card of the three-card row fully overlaps. This expresses the existing formation rules, not a new visible grid.

The order of units within a row matters for adjacency, certain area attacks, positional abilities, and other effects. Internally, the game tracks ordered positions and their derived alignment. Row distance and horizontal alignment remain separate concepts. References elsewhere to a slot mean an occupied ordered position or capacity, not a permanently numbered visible cell; future position-bound effects must define how they interact with row recentering.

There is no general facing, rotation, or rear-attack subsystem. A backstab or similar behavior must be an explicit skill effect.

### 7.4 Nominal row versus effective distance

Rows retain their identity when empty. Units do not automatically move forward when allies fall.

For range calculations, empty rows are skipped. A unit in Rear remains in Rear for row-dependent skills, even if it is now the nearest reachable opponent.

Example: if the enemy Front and Middle contain no relevant active visible units, the enemy Rear loses the distance protection those rows would otherwise provide. Reviving a visible unit in Front can restore that protection without changing the surviving units' row or order.

**Distance between two units on the same side** (used for ranges of cover, heal, swap and similar skills): **rows apart + whole cards of horizontal gap between them.** Touching neighbors in one row have distance 0. A neighbor one row away counts 1. The largest possible distance is 6. Every skill states its own target type and its maximum distance (**range**); there is no general penalty for distance. Reaching far is paid for in that skill's Delay and Momentum cost.

Both the attacker's row and the target's row matter to reach. A melee attack from Rear cannot automatically reach the opposing Rear. The original discussion proposed the following distances before empty-row compression:

| Attacker row | Enemy Front | Enemy Middle | Enemy Rear |
|---|---:|---:|---:|
| Front | 1 | 2 | 3 |
| Middle | 2 | 3 | 4 |
| Rear | 3 | 4 | 5 |

This numeric table is a proposed implementation, not an explicitly selected final formula. Its interaction with skipped empty rows and the range of each skill still need precise definitions.

### 7.5 Different meanings of occupancy

| Property | Intended use |
|---|---|
| Physical occupancy | Determines whether another unit or object may occupy the position. |
| Visibility | Determines whether the opponent can select the unit normally. |
| Range relevance | Determines whether an active visible unit contributes to effective row distance. |
| Area-effect presence | Determines whether a unit lies within an affected area, regardless of stealth. |
| Interception eligibility | Depends on activity, visibility, position, and the relevant defensive ability. |

These properties must not collapse into a single occupied/unoccupied rule. In particular, a knocked-out unit blocks its slot but does not become an active defender.

### 7.6 Unit size

**Current rule:** every normal playable unit uses the standard card width and one unit of row capacity. Large units are not planned as a standard player mechanic.

The underlying system should avoid assumptions that would make larger units impossible later. Bosses or special encounters may eventually use creatures that consume the space or capacity of multiple normal units. This is an architectural allowance, not a current player-facing rule. Multi-row footprints also remain deferred.

Unresolved future questions include movement fit, protection across horizontal alignments, targeting a large body, and whether overlapping an area several times deals damage once or repeatedly.

## 8. Targeting, range, and protection

### 8.1 Target legality

Skills specify valid targets, range, eligible starting rows, and relevant restrictions. A skill selects a unit unless it carries a targeting tag for another target type (section 4.7), such as `row`, which selects a row.

Explicit position- or row-targeted abilities were discussed as exceptions, particularly for area effects against hidden units. The exact exception list is open. General row targeting must not silently become available for every skill.

Target legality and interception are separate: an enemy may be a legal target while defenders still have a chance to intercept the attack.

### 8.2 Interception

Eligible defenders in front of a target may intercept appropriate attacks. Protection resolves from nearer defensive layers toward the target, with Front considered before Middle where applicable.

Normal cover depends on horizontal overlap between a forward unit and the unit behind it:

| Horizontal overlap | Normal covering relationship |
|---|---|
| 100% | Full column alignment: strongest normal cover, with a 50% chance to intercept. |
| 50% | Partial cover: a 25% chance to intercept. |
| None | That forward unit provides no normal cover to this rearward unit. |

**Adopted chances.** A forward unit that is not the intended target (passively targeted) intercepts with a **50%** chance at **100% overlap** and a **25%** chance at **50% overlap**. When it intercepts, it takes the **full hit (50%)** or a **partial hit (50%)**. A partial hit splits the damage in half: the interceptor takes half **rounded down**, and the remainder continues down the interception chain. A hit of 1 cannot be split and goes entirely to the interceptor. An intercepted hit changes Momentum by 0. **Every skill respects interception by default**; a skill must state explicitly if it ignores it (for example Sniper Shot). Individual skills, passives, statuses, and attack types may override or modify these standard rules. Summons and constructs that act as regular units can intercept like any unit.

- A full interception can stop or take over the incoming attack according to the effect.
- A partial interception leaves a remaining attack or effect.
- That remainder continues toward the original target.
- Further eligible defenders may attempt to intercept it.
- Partial interception does not remove a later defender's defense opportunity or the intended target's applicable defense.

Sequential protection is intentional and must obey the reaction-chain safeguards in section 6.6.

The project owner's examples establish two important skill concepts: **Sniper Shot** can bypass interception, while **Shield Wall** can allow three cooperating Front units to block 100% of physical attacks directed at rear rows. Their exact requirements, exceptions, and costs remain to be designed.

Proposed interception effects include taking the attack, absorbing part of its damage, reducing its remaining strength, removing one effect, or stopping a projectile component. The attack types eligible for each defense, exact probabilities, processing order, and interaction with the six stats are open.

Whether an area effect can be intercepted is part of its skill definition; by default it respects interception.

### 8.3 Target preview

Before committing an action, the interface should communicate:

- Whether the target is legal and in range.
- Which defenders may interfere.
- The relevant success or interception chance.
- Important exceptions such as ignored defenders or stealth interaction.
- The affected targets and movement or timing consequences where available.

An aggregate interception chance was proposed for the main display, with further detail on inspection. Example percentages from the discussions are placeholders.

### 8.4 Area patterns

Area and multi-target skills use **fixed, skill-specific patterns based on slots, rows, and adjacency**. There is no freely positioned radius or cone. Patterns may include a target and neighbors, a row, a line, or a chain with defined rules.

The UI highlights all foreseeable affected units before execution, including secondary damage, healing, movement, statuses, and friendly fire. For random outcomes, distinguish possible targets from guaranteed targets.

Friendly fire is **skill-specific** and may be an intentional consequence of a powerful ability. Skills define whether they affect enemies, allies, the user, any unit, or all units in an area. Only legal choices should appear as selectable targets.

Area effects can affect stealthed units physically inside their area. Whether they affect corpses, objects, allies, or the user must be defined by the skill.

A skill that hits units **behind** its target hits those with **100% overlap** automatically and those with **50% overlap** with a **50%** chance. Stealthed enemies can always be hit by attacks that reach them without targeting them. If a skill's target is KO when it executes, the skill targets units behind that target. **Only resurrection skills can target KO bodies.**

### 8.5 Closest-target ties

Stealth and empty-row rules affect nearest-target selection. Free target choice among equally close legal targets was proposed, unless the skill explicitly selects randomly. The tie rule remains to be finalized without adding unnecessary decisions to automatic actions.

## 9. Movement and positional skills

### 9.1 Repositioning

Voluntary repositioning costs a turn, as carried forward in the lane discussion. Skills may combine an action with movement; movement does not always require a separate turn when it is part of a skill.

Useful design examples include an attack from Middle that advances its user, a powerful action that leaves the user exposed, or a weaker action that improves the user's defensive position.

Exact movement recovery time and the order of movement versus damage are open and must be specified for the relevant action.

### 9.2 Blocked movement

Movement into a full destination row or beyond the formation boundary is blocked. Automatic row centering does not create extra capacity or displace units into other rows to make room; it only updates the display and alignment of the row's ordered occupants.

A skill may explicitly swap, push, pull, displace, or apply a consequence when movement fails. Slow, impact damage, or other effects were discussed as possibilities, not universal penalties.

Whether other parts of a combined attack-and-move skill still resolve when movement fails remains a required skill-design decision.

### 9.3 Position-dependent skills

A skill may have multiple valid starting rows and different behavior in each. For example, an attack might have greater force from Front and more reach from Middle. A row can also make a skill unavailable.

This is a means of making formation matter without assigning every row an arbitrary permanent stat bonus. No universal row-bonus table has been finalized.

### 9.4 Engagement

Engagement and movement restriction were discussed as optional mechanics. There is no established universal opportunity-attack or zone-of-control system. Use explicit effects if such behavior is selected.

### 9.5 Forced repositioning

**Status: to be specified.** Pushes, pulls, swaps, and similar effects that move a unit other than the user (for example the Guardian's Haul, Shove, and Swap Places) need their own rules.

Adopted so far:

- Position and range always matter: a skill can only reposition units within its reach, and it fails if the destination is full, as in section 9.2.
- Each skill defines whether the moved unit can be an enemy, an ally, or both.

Still to specify: where a moved unit lands horizontally in its new row, immunity and resistance (for example, a unit that cannot be moved), interaction with stealth and KO bodies, effects on position-bound skills and traps, and whether movement triggers reactions.

## 10. Stealth, incapacitation, and resurrection

### 10.1 Stealth

A fully stealthed unit is ignored by normal opposing direct targeting, effective-range calculations, nearest-target selection, and interception eligibility.

A row containing only stealthed units is effectively empty for the opponent's range calculation. Its units still occupy their physical slots.

Stealth does not grant immunity to area effects. An effect covering the unit's position can still hit it. Reveal abilities may explicitly bypass or remove stealth.

The source of stealth, duration, break conditions, partial visibility, and team-specific visibility rules are open.

### 10.2 Knocked-out units

A dead or incapacitated unit remains in its row and ordered position and **continues to consume row capacity**. Normal movement and displacement cannot move it. Only an explicit skill may move, remove, replace, or otherwise manipulate the body. Automatic visual recentering of a row is not a movement action and does not remove this occupancy.

This replaces the earlier proposal that a corpse would retain its position but leave its slot free.

Slot blocking is distinct from range and protection. A row containing only knocked-out units contributes no active defenders; bodies do not preserve living interception or effective-distance protection.

### 10.3 Healing and resurrection

Healing and resurrection are intended parts of the system. Keeping the body's slot makes resurrection at its retained position the natural baseline. Reviving an active defender can immediately change range and protection relationships.

**Healing is capped at maximum HP. There is no overheal.** Excess healing does not automatically create shields or additional health.

**0 HP means KO. KO removes all statuses**, including temporary positive and negative effects. Units are not automatically removed from the battlefield.

By default, resurrection returns the unit to its existing slot and schedules it **as though resurrection had been its own action**. It does not grant an immediate free turn. A particular resurrection skill may explicitly override that default. Revival HP, exact scheduling cost, and other consequences belong to the skill definition.

Permanent death outside combat, body-removal exceptions, and detailed repeat-revival mechanics remain open.

### 10.4 Named recovery skills

**Burnout Heal:** massively heals units around the user's current position and sacrifices the user. Its healing effects resolve simultaneously, then their reactions are processed. The skill specifies the exact point at which the user's KO resolves. Exact healing amount, affected pattern, team eligibility, and timeline cost remain to be designed; the discussion used allied area healing as its working example.

**Bound Resurrect:** revives a KO unit, after which the revived unit and the reviver share one HP pool. The shared maximum, initial pool value, multiple-hit handling, what happens when the pool reaches zero, and persistence through KO require skill-level decisions. Adding both maximum HP values was a proposal, not a final rule. Its relationship to the rule that KO removes statuses also needs an explicit definition.

Other proposed recovery concepts included Life Exchange, Sacrifice, Emergency Revival, temporary Reanimate, Second Wind, and revival with Exhausted-like consequences. They are not a finalized skill roster.

### 10.5 Status framework

Each status has its own application chance, duration, triggering conditions, and stacking behavior as defined by its source skill and status type. There is no universal stack rule. Creature types or skill-tree choices may grant specific status immunities. The suggestion that statuses always apply after a successful hit was rejected.

The owner's initial status list is **Blind, Poison, Bleeding, and Silenced**. Their exact mechanics remain to be designed. Suggested distinctions include blindness affecting sight-dependent skills, poison ticking on its own schedule, bleeding responding to physical activity or movement, and silence blocking appropriately tagged abilities.

Additional welcomed design candidates are Rooted/Immobilized, Disarmed, Stunned, Slowed/Hasted, Weakened, Enfeebled, Exposed/Armor Broken, Marked, Taunted/Provoked, Fear, Burning, Frozen, and Bound. Delayed/Accelerated can be one-off timeline changes rather than persistent statuses. These are a design pool, not finalized effects. A unit with no viable action because of statuses simply loses that turn.

## 11. Summoning

**Status: proposed subsystem, with one established loss-condition rule.** Summons never count as original party members for avoiding defeat. The dedicated discussion explored a model but did not settle its global limits or approve every detail. The Hunter's traps (see [classes_and_skills.md](classes_and_skills.md)) are the first adopted summons: stationary constructs that take a slot, have HP, and can be targeted and intercept like any unit.

### 11.1 Proposed summon categories

| Category | Battlefield behavior |
|---|---|
| Full unit | Occupies space, receives turns, uses skills, and can interact with ordinary unit systems. |
| Temporary unit | Behaves as a unit but expires after a duration or condition. |
| Stationary construct | Occupies space and produces passive or triggered effects without ordinary turns. Has HP, can be targeted, and can intercept like any unit. |
| Assist | Appears as part of a skill, performs its effect, and disappears; no persistent battlefield entity is required. |

Examples discussed include skeletons, thorn spirits, wolf spirits, healing totems, ballistae, reactive mushrooms, and a briefly appearing raven swarm. These are concept examples, not committed content.

### 11.2 Proposed operating rules

- Persistent summons need a valid free position, block it normally, and count toward the row's capacity. They may be placed between existing units, which recenters the row.
- Unit-like summons reuse normal stats, targeting, healing, statuses, movement, and timeline systems where applicable.
- A summon may be player-controlled, act autonomously, or use automatic triggers.
- Duration and the relationship to the summoner belong in the summoning skill.
- A skill may impose its own limit on simultaneous summons.
- Summoning is distinct from permanent collection; temporary summons disappear after combat as part of the battle-wide clean slate.
- A separate global summon resource was not recommended, but the cost model is not finalized.

Possible relationships include lasting until death, lasting for a number of turns, disappearing with the summoner, surviving independently, consuming HP, consuming a body or object, replacing the summoner, or upgrading/sacrificing an existing summon.

Summoning before battle was mentioned as an option and has not been approved as an additional phase.

### 11.3 Summon capacity

The intended six-member starting party does **not** fill the battlefield's total capacity. Additional row capacity primarily exists for summons and other temporary units, subject to the maximum of six normal units per row.

The theoretical eighteen-unit total may be reduced to approximately twelve or fifteen per side after readability testing. A separate summon budget or additional per-skill limits remain open. Players are not required to leave starting party vacancies solely to make summoning possible.

Multi-capacity summons remain deferred with other large units. The summon brainstorm does not override the standard card-size baseline.

### 11.4 Proposed summoner archetypes

Minion Master uses several smaller allies; Binder focuses on one powerful summon; Engineer/Totemist uses constructs; Sacrificer converts existing resources or units into summons. These are possible tree families rather than four additional approved classes.

## 12. Synergies, battlefield effects, and encounters

### 12.1 Synergy sources

Team-building should support interactions among species traits, skill trees, statuses, position, and timing.

Discussed examples include:

- A mark enabling focused damage or timeline delay.
- Interception improving an ally's next turn.
- A shield wall or neighboring defenders protecting a formation.
- A pack benefiting from cooperation.
- A support unit improving adjacent allies.
- A controller moving enemies into traps or unfavorable rows.
- HP expenditure supported by healing or life redistribution.
- Summons interacting with ally-death, summon-entry, or creature-type effects.

These examples establish design space, not finalized numerical effects. Formation synergies should be few enough to read clearly and strong enough to justify positioning decisions.

### 12.2 Row and slot states

Row effects such as burning ground, fog, or darkness, and slot effects such as traps, ice, or cover were welcomed as future tools. The initial implementation scope remains open.

An effect may apply to a row without requiring general direct row targeting. Its placement and targeting rule must be defined by its source skill.

Objects such as barricades and totems may occupy slots. They must not become a reason to require a fixed distribution of party members across rows.

### 12.3 Terrain and bosses

Terrain was discussed as a later encounter layer: blocked positions, restrictive spaces, water, or other positional conditions. Exact terrain rules and any reduced-capacity arenas need approval before they can override the normal formation.

Boss concepts include pushing or pulling units, disrupting a formation, and manipulating available space. Multi-row bosses and slot destruction are possibilities rather than current core rules.

**Standard defeat condition:** a team loses when all six original members are simultaneously KO. Active summons do not prevent that loss. A revived original member counts as active again. A team does not automatically lose merely because it currently lacks an offensive action.

Enemy behavior, difficulty progression, encounter length, simultaneous team wipes, retreat, and special objectives remain open. Boss defeat, survival, protection, positional objectives, and interrupting a ritual were proposed encounter variants.

## 13. Card interface and combat presentation

### 13.1 Card role

A card is the visual and functional representation of a battlefield unit, not a traditional object for drawing, playing, or deckbuilding. The card-game prototype uses the intended card presentation; it does not imply a later conversion to 3D gameplay.

The **front** contains:

- A customizable cosmetic frame and an image of the unit from the front.
- The unit's name and the names of all its classes.
- Relevant combat stats, including Life (current and maximum HP), Speed, attack powers, and defenses.
- Clear status and KO overlays on or around the border.

Cosmetic frames must never reduce gameplay readability. KO must be unmistakable, for example through desaturation or darkening in addition to an explicit KO indication.

The **back** uses the same selected cosmetic frame and shows the unit from behind, together with every skill it can currently use. Descriptions show the actual current versions, including all modifications and effects selected through skill trees.

The card back is primarily an **inspection interface**, not an action menu. With future online PvP in mind, players must be able to inspect opposing units and understand exactly which abilities they may use. This inspection requirement does not by itself commit to an online implementation.

### 13.2 Team builder

Party setup uses the same battlefield interface with the enemy side absent. The player's Front, Middle, and Rear rows remain visible; available units are placed and reordered using the same centering and positioning logic as combat.

Setup can provide more room for inspecting card fronts and backs, comparing builds, allocating skill-tree points, and other character-management systems if those features require it. Equipment inspection would apply only if such a system were later selected; it does not establish a general gear system. Combat prioritizes speed and readability, while setup supports deeper inspection and optimization.

Drag-and-drop was proposed, with mobile-friendly interaction required. Precise filters, sorting, saved teams, and gesture behavior remain to be designed.

### 13.3 Battle interface

The interface has a clear information hierarchy:

| Location | Purpose |
|---|---|
| Unit cards | Current unit state, stats, statuses, and KO. |
| Formation | Positional relationships, adjacency, and covering. |
| Bottom horizontal skill bar | The active unit's immediately available actions, similar to an MMORPG skill interface. |
| Top horizontal timeline | The shared temporal state of combat and multiple upcoming turns. |

All of the active unit's skills should be directly visible, normally 7?9 including the type-specific attack, without nested menus. The intended interaction is **select skill → highlight valid targets → select target → execute**. Invalid targets are visibly unavailable. Skills that require no target input execute directly according to section 6.3. Avoid unnecessary chained decisions and repeated confirmation prompts.

During targeting, area attacks and skills affecting additional units should preview all affected cards whenever possible. Target inspection also exposes range, protection, and relevant exceptions.

The timeline is permanently displayed at the top. Selecting a skill previews where the acting unit's next turn would move before execution. Skills that manipulate other timeline positions preview their consequences whenever practical. Already-created delayed effects, such as a future comet impact, may appear as timeline events, and auto events (such as queued shots) appear with their owner; ordinary attack animations do not. Enemy units' unchosen future attacks are not revealed.

### 13.4 Animation and effects

Cards generally **do not physically move around the battlefield when attacking**. Attacks primarily use effects originating from the acting unit and traveling toward or appearing on the targets. Brief shakes, vibration, flashes, and other hit reactions are allowed, with stronger attacks potentially producing stronger reactions. Cards must remain spatially stable enough for the battlefield to be read immediately. Explicit repositioning skills still change formation according to their rules.

Healing, buffs, debuffs, KO, and other important outcomes need distinct feedback without unreadable visual noise. The new presentation brief also names critical-hit feedback; because the current combat rules explicitly exclude separate critical hits, that feedback is conditional on a future rule change and does not introduce critical-hit mechanics here.

Statuses may use icons, border effects, or other clear overlays. Gameplay information always takes priority over cosmetic frames. Visual destruction must not make a KO unit's retained position and capacity appear vacant.

Earlier examples of cards being drawn or played do not establish a draw-and-hand combat system. Effects should be short, clear, and restrained: players must understand who acted, who was affected, which units remain active, and where everyone stands.

### 13.5 Contextual relationships and presentation principle

Unit relationships may be visualized temporarily when relevant or inspected. Hovering over an adjacency passive can highlight or connect the neighboring cards it affects; guards, auras, summons, and combos can similarly reveal their relationships. These indicators should generally not remain permanently visible.

Preserve a strong distinction between **visual simplicity and mechanical depth**. Players see clean, centered card rows; internally, ordered positions use half-card-width alignment. Formation, cover, adjacency, summons, targeting, and future positional mechanics should work consistently without exposing unnecessary grids or slots.

The default prototype is a six-versus-six battlefield with three rows per side, a top timeline, a bottom skill bar, card fronts for combat state, card backs for inspection and current skill information, and attack effects that leave cards spatially stable. Actual prototype testing must determine whether party size or total capacity needs to be reduced as described in sections 3.1 and 7.1.

## 14. Art direction

### 14.1 Visual identity

The selected qualities are **charming, colorful, varied, warm, and slightly fairy-tale**. Art is strongly stylized, expressive, and readable rather than realistic.

The former low-poly discussion contributes simple shapes, strong silhouettes, clear color grouping, and manageable detail. These are now illustration principles. They do not require meshes, rigs, 3D environments, or 3D animation.

### 14.2 Character illustration

- Give each creature a clear silhouette and one or two memorable visual features.
- Keep faces simple but expressive. Do not give every creature a friendly expression: foxes and otters may look approachable, while other creatures should look suitably menacing.
- Use slightly stylized humanoid proportions where appropriate.
- Preserve variety among humans, animals, humanoids, floating beings, and unconventional fantasy forms.
- Avoid mandatory class-specific outfits, colors, or silhouettes.
- Use type-defined weapons and props when they contribute to identity.
- Retain the distinction between color variants and separate creatures.

The concept-art requests for humans representing different classes explore variety; they do not reverse the rule that appearance must not rigidly encode class.

### 14.3 Size contrast

The collection should include a range from mice to mountain-like beings. The selected intent is to preserve strong size contrast within readable limits.

In the card format, artwork framing and visual scale cues must communicate that contrast while keeping each card usable. Exact framing standards are open. Visual size does not change the standard card width or normal one-unit row-capacity rule; possible large bosses remain deferred.

### 14.4 Color and detail

Use controlled, recognizable palettes and broad color zones. Warmth and variety should not become visual noise. Strong silhouettes and primary forms matter more than dense surface detail.

The art guide proposed two to five major color blocks with a selective accent, modest surface patterns, and subtle rather than heavy outlines. These are production guidelines, not fixed gameplay categories.

### 14.5 Biomes and environments

Visually distinct, relatively isolated biomes were accepted in the original direction. Their role in the card game may include artwork settings, encounter backgrounds, or campaign identity; the delivery format remains open.

The art guide proposed Forest, Plains/Meadow, Coast/Shore, Swamp, Desert, Snow/Frostlands, Volcanic/Ashlands, Crystal/Mystic regions, Ancient Ruins, and Caves/Underground. Optional ideas included fairy groves, autumn woods, high mountains, luminous wetlands, and storm cliffs.

These are a proposed palette and setting library, not a confirmed launch-region list.

### 14.6 UI and VFX style

Use clean panels, modest ornament, generous spacing, bold icons, legible text, and comfortable touch targets. Effects may have recognizable elemental identities, but should not obscure the board or create constant screen-filling spectacle.

No final sound or music direction has been defined.

### 14.7 Existing concept exploration

The card-art discussion began with five animals and five fantasy creatures, initially excluding humans. Later requests added batches of ten animals, ten humanoid fantasy creatures, ten large extravagant fantasy creatures, ten male humans, and ten female humans.

**Artwork delivery rule:** create separate artwork images in a card-friendly aspect ratio, not finished cards with baked-in UI and not a single collage. The precise numerical aspect ratio is not established in the recovered text.

The browser exposes generated-image galleries, but the individual images have not been cataloged as a named creature roster in this document. The text establishes direction and requested batch sizes, not approved creature statistics. Fox and otter are explicitly referenced in the expression feedback.

## 15. World and narrative

**Status: story campaign required; narrative details open.** Players explore the campaign and unlock units, then continue unlocking units through at least one post-story random-battle mode.

The narrative should explain why the player encounters different beings, why they join a team, and why new combinations matter. A lightweight central conflict with local stories was proposed, with an explorer, wanderer, researcher, mercenary, or guardian-like player role rather than a required chosen-one premise.

Possible narrative layers include regional conflicts and emergent party events or relationships. Reactions to party composition, recruitment stories, rivalries, and personal quests remain proposals rather than required campaign features.

The suggested ten to fifteen main chapters was an example. There is no approved plot, named setting, faction roster, protagonist, or chapter count. The original free-roaming 3D world is superseded; any story structure must now suit the card game.

## 16. Technical and production direction

### 16.1 Engine and architecture

Godot has been installed and the repository contains a Godot project. The earlier technical recommendation was Godot with C#/.NET and a separate, engine-independent rules core. This architecture is a proposal, not an implemented system.

The proposed separation is:

- **Rules core:** units, builds, skills, damage, targeting, formation, timeline, triggers, and encounter state.
- **Presentation:** cards, UI, animation, effects, sound, and input.
- **Content definitions:** creatures, trees, skills, statuses, and encounter data.
- **Validation:** reproducible checks for the combat rules independently of visual rendering.

Chance-based mechanics still exist. Reproducibility would require controlled random inputs; an engine-independent core does not mean combat contains no randomness.

A reusable card component and shared content definitions were proposed for collection, team-building, inspection, and combat. Earlier sample folders, code, engine-version claims, and platform-export claims are not frozen technical requirements.

Input abstraction and a reusable card scene were proposed to support desktop and touch controls without duplicating game logic. Windows, Linux/Steam Deck, Android, and iOS were specifically discussed; the final delivery plan remains open. The working approach assumes substantial AI-assisted development with the owner reviewing decisions and results.

### 16.2 Asset workflow

The production direction is creature concept → illustrated card artwork → UI integration → lightweight animation and effects. AI-assisted artwork and code were discussed as production aids.

The former Blender/model/material/rig/skin/animation pipeline is superseded. There is no requirement to build 3D characters, environments, cameras, navigation, or collision for this product. Historical triangle budgets, skeleton families, LOD guidance, and GLB export rules are not current card-art requirements.

Retained production guidelines are consistent asset naming, reusable presentation components, coherent palettes, and checking each asset for identity, phone-size readability, manageable detail, and battlefield clarity before acceptance. Artwork is supplied separately from card UI so the same illustration can serve several contexts.

### 16.3 Cost and delivery planning

Engine costs, storefront fees, external assets, sound, fonts, and possible service costs were discussed as planning topics. This document sets no budget, engine version, commercial license conclusion, or online-service commitment. Licensing and platform support must be checked when delivery decisions are made.

## 17. Playable scope and development priorities

### 17.1 First playable direction

Begin implementation with **AI-versus-AI combat for development testing**, before adding player-controlled modes and campaign progression. Both sides should use the same combat rules that player-controlled units will use.

The first playable should exercise:

- A small selection of humans, animals, and fantasy creatures.
- Six-member formation across three rows.
- Direct testing of active skills, passives, and modifiers on level-20 units; tree organization and point allocation follow later.
- The six global stats and skill-specific damage scaling.
- Visible turn order and a limited set of timing effects.
- Movement, targeting, and a readable form of protection.
- Card-based presentation and mobile-conscious controls.

The sequence after the AI-versus-AI milestone remains open. Summons, terrain, and multi-slot creatures should not silently enter the first playable simply because they have been explored. Prepare online-facing interfaces, but keep online PvP implementation gated on explicit owner greenlight.

### 17.2 Content counts remain proposals

Different discussions suggested around 10 recruits for an early slice or 15–20 for a combat prototype, and between 6–8, 8–12, or 10–12 initial trees. These are alternative scoping suggestions, not simultaneous targets.

The current class roster is fixed at seven: Hunter, Elementalist, Healer, Warrior, Guardian, Feral, and Leader. Creature roster size, enemy count, and final content budget remain open.

### 17.3 Deferred or unselected systems

Multi-slot creatures, complex terrain, emergent relationships, genetic variation, and expanded summon archetypes remain deferred or open. The story campaign and post-story random battles are required; their detailed content and mechanics remain open.

Online PvP is planned with interface preparation now, but implementation requires explicit owner greenlight. Its detailed rules and networking specification remain open. Cooperative play, rankings, trading, crafting, an equipment economy, monetization, achievements, and save-progression rules also remain unspecified.

## 18. Identity and reference boundaries

Everkin remains a working title. A previous naming discussion raised potential existing-name conflicts; final commercial naming and clearance are unresolved. This document records that issue without treating the earlier legal commentary as clearance or a current legal assessment.

All artwork, character designs, UI, names, text, music, and branding must have an independent identity. References guide broad design goals rather than supplying assets or exact presentation.

Previously discussed reference games include FFX for turn-order tactics; Hearthstone for card presentation; Siralim Ultimate and Monster Sanctuary for team-building and build synergies; and LumenTale, World of Final Fantasy, Cassette Beasts, Dragon Quest Monsters, and Monster Hunter Stories as comparison points. Their individual rules are not adopted by default.

Minecraft and Super Mario 64 were earlier references for simple geometry and readable 3D form. They no longer define a 3D asset-production requirement.

Earlier naming candidates were Wildbound, Kinforge, Riftkin, Beastfall, Veyra, Tamerift, Everkin, Roamkin, Feralis, and Bondfall. They are historical alternatives, not cleared commercial names.

## 19. Decisions still required

| Area | Outstanding specification |
|---|---|
| Party | Prototype validation of six-versus-six readability, possible four- or five-unit standard, starting below the standard size, enemy exceptions, duplicate recruits. No combat reserve swapping. |
| Timeline | Initial order/progress, ties, Haste/Slow granularity, direct progress manipulation details. Speed 1–10 (10 fastest), skill Delay 1–10 (1 quickest), and Wait = Delay + 11 − Speed are adopted (section 6.1). |
| Momentum | Behavior at the ±10 limits, snowball safeguards, and shifts from periodic, reaction, and simultaneous effects. The scale, gain, spending, and interception rules are adopted (section 6.7). One shared meter starts at neutral zero; no per-skill Momentum Gain stat or between-battle carryover. |
| Skills | Individual prerequisites, Momentum and other consequence costs, effect order, and delayed-event definitions. One type-specific attack plus 6?8 additional usable skills is the target, not a loadout cap; no mana or cooldowns. |
| Damage | Modifier order, defense bounds, penetration, shield interaction. Uniform rolls, no crits, formula-based whole-number damage, and round-down with a minimum of 1 are fixed (section 5.2). |
| Defense | Eligible attacks for each defense and precise interaction with damage events. Interception chances and the partial-hit split are adopted (section 8.2). |
| Reactions | Numeric chain limit, exact priority categories, secondary tie-breaks. Skill-controlled propagation and safeguards are required. |
| Range | Exact effective-distance calculation and per-skill ranges. |
| Movement | Destination selection, combined-action failure behavior, recovery time, swaps. Forced repositioning is specified in section 9.5. |
| Statuses | Individual stacking, durations, expiry timing, cleansing, stealth breaks. Application chances are explicit; KO removes all statuses. |
| KO and revival | Per-skill revival HP and recovery cost, body-removal exceptions, out-of-combat consequences, shared-HP details. |
| Summons | Overall per-side cap (theoretical eighteen; approximately twelve or fifteen under consideration), placement, control, lifetime, costs. Extra capacity beyond the starting party is supported. |
| Builds | Class/skill designs and assignments, conflicting modifiers; later tree organization, node costs/ranks, prerequisites, progression/stat growth, and confirmation of level-up awards. Initial level-20 units receive all creature/class skills. Mixed meaningful effects and free out-of-battle respec are selected; trees and spending come later. |
| Recruitment | Guaranteed encounter-to-unit reward assignments, repeats/duplicates, human generation, individual persistence, starting collection. Other acquisition methods are not currently required. |
| Encounters | Simultaneous wipes, retreat, special objectives, enemy behavior, difficulty, length. All six original members KO means defeat. |
| Product structure | Four-stage illustrated campaign details, later full campaign design, post-story random-battle rules, local two-player flow, and final content. AI-versus-AI testing comes first; online implementation requires greenlight. |
| Presentation | Final card styling, mobile readability, artwork framing, gestures, audio, and prototype validation of party/capacity limits. Centered rows, front/back roles, top timeline, and bottom skill bar are selected. |
| Delivery | Windows/Android/iOS minimum requirements and distribution, landscape layouts, technical validation, save system, business model, final name. |

These are intentional gaps in the current design, not permission to inherit equivalent rules from Hearthstone or another reference game.

## 20. Details to complete before implementation

### 20.1 Finished-game scope

Required scope includes AI-versus-AI development testing, single-player AI battles, local two-player, an exploratory story campaign unlocking units, and at least one post-story random-battle mode unlocking further units. Target Windows, Android, and iOS in landscape only. Online PvP interfaces must be prepared, while online implementation remains gated on explicit owner greenlight. Select the complete roster and remaining systems; summons, terrain, and large bosses still need explicit inclusion or exclusion.

For each required mode, define the full flow from launching the game through party setup, battle, results, and replay or progression. Specify AI behavior/difficulty and local handover rules. Define future online requirements and interface boundaries without implementing matchmaking, transport, or services before greenlight.

Use illustrated locations with selectable paths/events and a four-stage line for current campaign testing, with guaranteed encounter unit rewards. Define the starting collection, specific rewards, duplicate restrictions, loss consequences, saved teams, and completion flow. Respec is free outside battle. Full campaign design and normal progression details remain for later; test units start at level 20, and skills precede trees and point allocation. Unselected systems should be explicitly excluded rather than left ambiguous.

### 20.2 Exact combat details

In addition to section 19, resolve these edge cases:

- Timeline: time units, initial progress, equal readiness, event/expiry priority, progress overshoot, Speed bounds, Stop, Reset, Turn Now, skipped-turn cost, and whether time advances while choosing actions or playing animations.
- Momentum: bounds and spending rules for the single contested meter starting at zero, success with interception/shields, multi-hit/area/reaction/periodic effects, non-damaging actions, payment timing, failed-action refunds, and simultaneous changes.
- Damage: integer or continuous sampling, rounding, modifier order, defense bounds, penetration, shields, minimum damage, healing scaling, and independent versus shared rolls across targets.
- Resolution: validation, cost payment, effect processing, Momentum updates, reactions, KO, and victory-check order; priorities, ties, and chain-limit behavior.
- Statuses: whose turns or which timeline clock measures duration, tick/expiry boundaries, reapplication, cleansing, immunity, source removal, and shared HP.
- Formation: cover eligibility and same-layer defender order, range formula, movement/reorder/swap costs, failed movement, and whether anchored effects follow occupants or coordinates when rows recenter.
- Battle boundaries: persistent progression consequences, simultaneous wipes, stalemates, retreat, and encounter exceptions. Full starting HP, neutral-zero Momentum, and cleared temporary combat states are selected.

### 20.3 Complete content definitions

Each included creature needs a stable ID, name, front/rear artwork, framing, all six stats and growth rules, classes, its one type-specific attack, natural passives/immunities, and acquisition rules. Artwork filenames are not approved creature names or combat values.

Each class needs its complete skill-tree graph: node IDs, costs, ranks, prerequisites, branches, exclusivity, effects, and respec behavior. Define how multiple modifiers combine and how the final usable skill is displayed.

Each skill needs availability conditions, valid targets and range, area pattern, timing multiplier, Momentum/other costs and payment behavior, ordered effects, probabilities and formulas, reactions, KO/interruption/retargeting behavior, and presentation. Do not add a per-skill Momentum Gain field.

Each status needs application, duration clock, tick/expiry order, stacking/reapplication, effects, cleansing/immunity, and KO/reset behavior. Each encounter needs its roster, builds, formation, opponent behavior, outcome conditions, and rewards or exceptions if applicable.

Final content lists must be complete; examples and partially filled entries do not count as implemented content.

### 20.4 Technical and delivery requirements

Finalize the language/runtime, content format and IDs, authoritative rules state, UI preview/execution relationship, event ordering, random-number control, and build/export process for the selected platforms. The existing rules-core/presentation/content separation remains a proposal until selected.

Define persistent data and save timing, format/versioning, recovery from corruption, battle resume if included, and online responsibilities if relevant. Resolve performance targets, supported devices/resolutions, input, languages, accessibility, sound/music/VFX inventory, tutorials, settings, and missing-data/error behavior. Detailed screen behavior belongs in [ui_spec.md](ui_spec.md).

### 20.5 Worked checks

Use concrete examples alongside rules to verify implementation. Initial checks include:

| Situation | Expected result |
|---|---|
| Speed 5 uses a Delay 5 skill | Wait is 5 + (11 − 5) = 11 time units. |
| After 4 of those 11 units, Haste raises Speed by 2 | Remaining 7 becomes 5; completed progress is preserved. |
| Speed 10 with Delay 1 versus Speed 1 with Delay 10 | Waits are 2 and 20: the fast unit acts about ten times per slow turn. |
| Three-card row opposite a two-card row | Relevant overlaps are half or zero, matching section 7.3. |
| A unit becomes KO | Statuses clear; ordered occupancy and row capacity remain; normal active defense stops. |
| Partial interception with another eligible defender behind | The remainder may undergo further defense. |
| A simultaneous effect block hits several units | Apply the block before resulting KO checks and reaction collection. |
| Healing exceeds missing HP | HP stops at maximum without an automatic excess-healing benefit. |
| All original members are KO while a summon survives | The party loses under the standard defeat rule. |
| Start another battle | Every participating unit begins at full HP, the contested Momentum meter is neutral zero, and previous temporary combat states do not carry over. |
| An interceptor takes a partial hit from 7 damage | It takes 3 and 4 continues down the chain. |
| An interceptor takes a partial hit from 1 damage | It cannot be split; the interceptor takes 1. |
| Defense reduces a hit to 0.6 damage | The hit deals 1 (rounded down, minimum 1). |
| An attack is intercepted | Momentum does not change. |
| The reference human uses Sniper Shot (7–9) on a 0%-defense critter with 20 HP | The critter dies on the third hit and never on the second. |

Expand these examples when numerical and mode-specific rules are decided. They are expected behavior, not claims that tests have passed.

## 21. Definition of done

The specification is ready when every included feature has concrete rules, content, numerical values, UI behavior, and observable completion criteria, with no unresolved contradictions or required decisions. Provisional values must be explicitly adopted as the current implementation values; the agent must not guess them.

**Implementation is done when everything required by the final versions of this game design document and the UI specification is implemented and verified.** This includes all selected modes, complete content, working play flows, saving, presentation, and required platform builds. No required feature may be silently omitted or replaced by an unapproved placeholder.

Online PvP is an explicit gated exception: before owner greenlight, completion requires the specified online-ready interfaces, not a functioning online mode. Once greenlit, its agreed implementation requirements become part of the completion scope.

A prototype is an intermediate validation milestone, not the finished game. Readability testing may lead to documented changes, but finishing only the first playable does not satisfy this definition.
