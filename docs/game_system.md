# Everkin — Game Design Document

**Version:** 0.5 — Four global stats (one Power, one Defense) and the creature roster

**Updated:** 2026-10-05

**Format:** Digital tactical card game with creature collection and RPG character builds

**Project name:** Everkin is a working title.

**Companion documents:** [UI specification](ui_spec.md) owns screens and interactions. [Classes and skills](classes_and_skills.md) owns the class and skill definitions. [Creatures](creatures.md) owns the creature roster, size tiers and the type-specific attack library. This document owns game rules, scope, and implementation requirements.

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

Players explore a **story campaign presented as illustrated locations**. For now, use a **linear sequence of four stages**. Each stage has **about five battles plus a boss battle** that ends the stage (about 24 battles in total), played in a fixed order; the location map is illustration, not a route choice. Detailed campaign content will be specified later. The four-stage sequence is the current testing scope, not a final campaign-length commitment.

Units unlock through **guaranteed encounter rewards** for now. Each won encounter gives **one fixed creature**, shown on the map before the battle, and that creature is **one of the defeated enemy team**. Exact encounter-to-unit assignments remain open. A won encounter can be replayed, but its unit reward is given only once. A mixture of acquisition methods may be considered later, but capture or other recruitment mechanics are not currently required.

**Collection.** A player owns each creature at most once, and a player's team uses each creature at most once. Enemy teams may repeat creatures.

**Start of play.** A new player begins with the **campaign tutorial**: three short battles. The player starts with two creatures and gains more after each win until the team has six. All other modes unlock once the tutorial is complete. Which creatures the tutorial gives is open.

**Campaign rules (adopted 2026-10-06).** Losing a campaign battle has no penalty: the player can retry the encounter at once or change the team first. Campaign locations hold battles only for now; story events and other encounters come with the full campaign design. Encounters never restrict the player's team for now. The campaign cannot be restarted; finished encounters can be replayed instead.

**Levels.** Every unit is level 20 in every mode until skill trees exist. Levelling is decided together with the trees (section 4.1).

After completing the story, at least one **random-battle mode** provides continued play. The player picks a difficulty and the enemy team is drawn at random. Random battles give **no rewards**.

**After the story.** Fixed, harder **challenge encounters** appear on the map; each unlocks one more creature. After the story, and in online play, the player also earns **coins**. Coins unlock creatures that are not campaign rewards. Random-battle wins, challenge encounters and online play pay coins. Each coin creature has its own price. Coins can also be bought (production.md §1.2). Coin amounts and the individual prices are open.

There is no established collectible-card business model. The use of cards does not itself imply booster packs, paid randomized acquisition, trading, or duplicate conversion.

### 2.3 Platforms and controls

The required platforms are **Windows, Android, and iOS**, with **landscape orientation only**. Desktop and touch readability must influence the design from the start.

Release order, stores, minimum devices, screen shapes and controller support are adopted in [production.md](production.md), section 1. Required platform support must be verified during implementation; it is not an existing implementation milestone.

### 2.4 Modes and development order

- **AI versus AI:** the first development/testing mode, allowing both parties to play automatically for easier combat testing.
- **Single-player versus AI:** required, including the story campaign and at least one post-story random-battle mode.
- **Free battle versus AI:** available once the tutorial is complete. The player picks a team from their collection, an enemy team (random or chosen from unlocked units) and the AI difficulty. No rewards.
- **Local two-player:** required. Both players build their teams from the collection saved on the device, and both teams may use any creature in it, including the same ones. The board flips so the acting player's side is always at the bottom.
- **AI strength.** Each campaign and random-battle encounter sets its own AI level. Only free battles let the player choose it.
- **Battle viewer:** only in development builds.
- **Online PvP:** planned, but **do not implement it until the owner explicitly greenlights it**. Prepare code interfaces and architectural boundaries for future online play; the backend will be Spring Boot (Java) (section 16.1). This preparation is not authorization to implement networking, matchmaking, or online services.

**Balance testing method.** AI-versus-AI battles start from random units. Each AI picks its action on its turn with a minimax search. Every battle's setup and result are recorded (section 2.5), and statistics are kept on which units, classes and skills win or lose more often. Those statistics drive balance adjustments. Battles must therefore be deterministic given a seed, and the rules core must be able to run battles without any presentation.

The AI-versus-AI milestone comes first once gameplay implementation begins. The current task remains defining the game before implementation.

### 2.5 AI-versus-AI test harness

The harness plays battles automatically for balance testing and debugging. Adopted rules:

- **Teams.** Each side gets 6 random creatures from the roster ([creatures.md](creatures.md)), with no duplicates within a side. The same creature may appear on both sides. Units are level 20 with all their creature and class skills (section 17.1).
- **Party size.** Test battles are 6 versus 6 only for now.
- **Formation.** Each side's starting formation is a random legal placement. It is recorded with the battle, so the statistics also show which rows work for which units.
- **Search.** Each AI uses a minimax search over a fixed number of upcoming timeline turns, counted across both sides. Both AIs in a test battle use the same settings.
- **Depth and difficulty.** Search depth comes from the AI's difficulty level: low difficulty searches shallow, and the highest level uses a high depth that still runs without problems. Test runs use the highest level. The depth for each level is set after measuring speed in the prototype (proposal: lowest 2, highest 8).
- **Chance in the search.** The search uses average values for damage rolls and probabilities. The real seeded roll is applied once the action is chosen.
- **Evaluation.** At the end of its look-ahead the search scores a position as a weighted sum of each side's remaining HP share, units still standing, and Momentum. The weights live in a config file.
- **Information.** The AI knows only what a player would see. Because stealth limits targeting, not information (section 10.1), that includes stealthed units, their HP and statuses. Opposing future skill choices are not known.
- **Length cap.** A battle stops after 500 unit turns in total and is recorded as a draw (section 12.3).
- **Run size.** One test run plays 10,000 battles with seeds numbered 1 to 10,000. Each run gets an ID and stores a snapshot of the rules version, content and AI settings it used, so the runner can reproduce a battle exactly from its run ID and seed, even after later balance changes.
- **Saved per battle.** Run ID, seed, teams, formations, result and length, plus per-unit damage dealt and taken, healing, KOs and skill uses. No event log is stored.
- **Report.** Win rate and appearance count per creature, class and skill; each creature's win rate by starting row; skill usage counts; battle length and draw rate; and pair statistics showing which creatures do well together and against each other.
- **Imbalance flags.** The report flags the top and bottom 10% by win rate. Balancing starts with the most extreme ones.
- **Balance changes.** Claude proposes changes from the report, the owner approves them, and the next run checks their effect.
- **Watching battles.** Headless batch runs come first. A battle viewer on the card UI then serves as a debug mode, also used to debug UI issues. It can **start a new AI-versus-AI battle with the same setup** (teams and formations) as any recorded battle. It does not need to replay the recorded battle exactly, so the new battle may play out differently.

Still open: the search depth for each difficulty level and the first evaluation weights.

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

**Adopted (2026-10-06):** human recruits are **fixed individuals** with names and their own class mix, made (possibly rolled) during content creation and stored as fixed content. They are never generated at runtime. This keeps individual identities while keeping recruitment and balance controllable.

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

### 3.7 Creature roster

The exact creature roster lives in [creatures.md](creatures.md): size tiers (critter, medium, large) with base stat blocks, the shared library of type-specific attacks, and every creature's ID, art, stats, classes and attack. Adopted rules for creatures:

- **Size tiers.** Each creature belongs to one tier and tweaks that tier's base stat block slightly.
- **HP** runs from 20 to 80 in steps of 5. The anchors in section 5.5 stay.
- **Speed** follows the tier (critters 7–9, medium 4–7, large 1–4). A creature may break it with a stated reason.
- **Power** runs from 1 to 10, with the reference human at 5.
- **Natural Defense** is 0–40% in steps of 10. It is rare, and 0% is the default. Skills and statuses supply the rest, up to the 75% cap.
- **Type-specific attack.** Each creature gets one attack from the shared library and cannot change its numbers.
- **Classes** are hand-assigned per creature to fit its concept: humans usually three, animals one or two.
- **No natural traits** for now. A creature's identity comes from its stats, classes and attack.
- **Humans** are generated once during content creation and stored as fixed recruits (section 3.4), each with a name, its own class combination and small stat differences within the medium tier.
- **IDs** are stable text IDs (such as `fox` or `stone_turtle`). The game loads art by ID, with separate front and rear files, from cleaned copies; the original files are never changed (production.md §1.6).

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

Each unit begins with **exactly one type-specific attack**, such as Bite, Sword Slash, Punch, or Arrow. Through its skill trees, a unit should gain **6–8 additional usable skills in total across its classes**, not 6–8 per class. **All unlocked skills are available in combat; there is no separate loadout or equipped-skill cap.**

The intended developed unit therefore has **7–9 usable skills including its type-specific attack**. This is a content-design target, not a hard cap. Initial testing still grants all available creature/class skills before trees exist, so temporary test units may exceed that target. Passives and modifiers are mixed into the trees but do not each require a separate action button.

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
| Damage type | `physical`, `magical` | What kind of skill it is, for rules that react to it (Shield Wall blocks `physical`, Silenced stops `magical`). Both scale with Power and are reduced by Defense (section 5.1). |
| Delivery | `projectile` | Travels the battlefield from the user to the target, so it passes the units in front of the target and can be intercepted by them. |
| | `melee` | Needs reach to the target. Triggers effects that react to melee. Can be intercepted. |
| | `direct` | Hits a **specific slot** (the position on the half-card grid, section 7.3, of the selected target), with nothing passing between. It hits whoever occupies that slot when it resolves, so a delayed `direct` effect hits whatever stands there later. **Ignores interception.** Still needs a legal target and reach when it is cast. |
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
- **Only `melee` and `projectile` skills can be intercepted** (section 8.2). A skill without either tag, such as a `direct` skill, cannot be. A `melee` or `projectile` skill ignores interception only if it says so.

## 5. Unit statistics and damage

### 5.1 The four global stats

| Stat | Definition |
|---|---|
| HP | Maximum health; current HP is tracked during combat. |
| Speed | 1–10, **10 is fastest**. Determines how quickly the unit completes its turn cycle (section 6.1). |
| Power | 1–10. Scaling basis for all skill damage and healing, `physical` and `magical` alike. Each creature type defines its own value. |
| Defense | 0–75%. Percentage reduction of all damage, `physical` and `magical` alike. |

Defense may be **0%**. There is no universal built-in defense allowance: defense comes from the creature type, skill tree, or explicit effects. Display it as the actual percentage reduction rather than an opaque rating. There is a single Power and a single Defense: the earlier split into Physical and Magic Power and Defense was removed on 2026-10-05 to keep the stat sheet small.

Accuracy, evasion, critical chance, critical damage, mana, universal resistance, and healing power are **not additional global stats**. Skill-specific hit chances, defensive effects, or healing scaling may be defined without expanding the stat sheet. Mana and critical-hit systems remain excluded.

### 5.2 Damage scaling

Each damaging skill has a hidden **coefficient range**. The displayed damage range is that range multiplied by the user's Power.

Illustrative definitions:

- Bite: 80–100% of Power.
- Heavy Slam: 150–190% of Power.
- Arc Bolt: 90–120% of Power.

These values demonstrate the selected model; they are not approved balance data. A skill may explicitly scale from HP, defense, or multiple stats, but that exception must be part of its definition.

**Adopted damage formula:** each creature type defines its own Power. A skill's damage is its hidden coefficient range multiplied by the user's Power, rolled uniformly, and the skill tree may change the formula. Players always see **whole numbers only**, shown for the actual user. Damage is **rounded down, with a minimum of 1**, unless a skill explicitly negates it. Numbers in the class documents assume a **reference human with Power 5**.

**There is one damage category.** All damage interacts with the single Defense stat; the `physical` and `magical` tags only matter to rules that name them. Fire, frost, poison, bleeding, mental effects, and similar themes use properties or statuses rather than additional global resistance categories. Defense bypass is a property of a skill, not a third defense stat.

Damage is sampled **uniformly within the skill's damage range**. Each value is equally likely; the range itself defines the variance. Some skills may have a very wide range. There are **no separate critical hits or critical multipliers**; a high roll can provide the excitement of a critical hit without another system.

**Adopted damage resolution order:**

1. **Roll.** The displayed range already includes the attacker's modifiers (coefficient changes, bonus damage, conditions such as Ambush) and is rounded down. The roll is a uniform **whole number** in that range.
2. **Shield.** A shield or similar absorb effect takes **raw** damage first. A shield has no defense of its own. Only the remainder continues.
3. **Defense.** The remainder is reduced by the target's defense percentage, then rounded down once. If anything passes the shield, the result is at least 1.
4. **Final reductions.** Other stated reductions (for example Dampen, which halves the damage, rounded down with a minimum of 1) apply **after** defense, on the final damage. Ward and Dampen can be on the same unit: Ward absorbs first, then defense, then Dampen.
5. **HP.** The remaining damage is subtracted from HP.

**Defense rules.** Defense runs from **0% to 75%**. Defense from several sources (creature, skill tree, statuses) **adds in percentage points**, up to the cap. Defense never goes below 0%: effects such as Exposed or Armor Broken only remove defense down to 0%. **Penetration** lowers the target's defense by a stated number of percentage points for that hit, down to 0%. A skill ignores defense only if it says so. **No skill does so currently**, and no tag exists for it. Fixed-damage skills (for example a counter dealing 2) are reduced by defense like any other hit, with the minimum of 1. The only fixed, unreducible damage is bump damage (section 9.5).

**Rolls across targets.** There is no global rule: each skill states whether its targets share one roll (an explosion rolls once) or roll independently (each hit of a meteor shower rolls separately).

**Healing scaling (adopted 2026-10-06).** Healing works like damage: a hidden coefficient range times the user's Power, rolled uniformly as a whole number, rounded down, minimum 1. A skill may define its healing differently, and that exception must be part of its definition. Healing is capped at maximum HP (section 10.3).

**Several shields on one unit.** Deferred: decided when all skills are revisited after AI-versus-AI testing.

Interception is defined in section 8.2. Target previews should show the resulting damage range against the selected target wherever determinable.

### 5.3 Hit reliability

An ordinary attack hits a valid target by default, subject to applicable defenses and explicit effects. Random hit chance is an explicit property of selected risky skills, not a universal accuracy/evasion roll. A status or skill may introduce an exception, such as blindness affecting attacks that require sight.

Status application is separate: a status-applying skill states its own status chance. Successful damage does not automatically guarantee a status unless that skill says so.

### 5.4 Stat sources and growth

The discussed sources of values are creature identity, build investments, and temporary combat effects. Level growth and the relative contribution of levels versus skill trees remain open.

Healing scaling, damage-over-time calculation, status durations, stacking behavior, and cleansing belong in the relevant skill or effect definitions. Critical hits are excluded from the design.

### 5.5 HP and damage anchors

HP anchors: a critter (the lowest HP in the game, such as a fox) has **20**, a human **35–45**, and a giant stone turtle **60**. Damage anchor: a critter with 0% Defense dies to exactly **three Sniper Shots** from the reference human (Sniper Shot deals 7–9). Keep all numbers low.

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
| Pull Forward | Directly removes time units from the remaining Wait. |
| Push Back | Directly adds time units to the remaining Wait. |
| Reset | Resets current turn progress. |

(The timeline effect formerly called Delay is now **Push Back**, because Delay is the skill scale.)

All friendly and hostile units are always evaluated within this same timeline. Units must be balanced around their actual action frequency.

The UI shows multiple upcoming turns and previews where the acting unit's next turn would move when a skill is selected, before confirmation through target selection. Other foreseeable timing changes should also be previewed whenever practical.

**Adopted timeline details:**

- **Initial progress.** At battle start each unit's first Wait is `11 − Speed`, as if its previous skill had Delay 0. A Speed 10 unit acts at time 1 and a Speed 1 unit at time 10.
- **Ties.** When turns arrive at the same time, the higher Speed acts first. If Speed is equal, a **seeded random roll** decides, so battles can be replayed from the seed.
- **Time model.** Time advances only in jumps between events. Choosing an action and playing animations cost no time, and a turn costs time only when its skill resolves. A turn timer may be added later for online play only.
- **Same-time order.** Status expiries resolve first, then auto and delayed events, then unit turns. This order is expected to be fine-tuned during testing.
- **Haste and Slow.** A skill states a whole number of Speed steps (typically 2, never more than 3). Besides Speed, skills and effects may also give a unit's skills more or less Delay.
- **Pull Forward and Push Back.** There is no global cap. Each skill states its number, checked against the damage anchor and playtesting.
- **Reset.** Restores the full Wait of the unit's last skill, starting from now.
- **Action denial.** There is no safeguard against Slow, Push Back and turn-skipping statuses for now. Test first.
- **Skipped turn.** A skipped turn costs as much as a Delay 5 skill: Wait = 5 + (11 − Speed).

- **Delay bounds.** Effects that add or remove Delay never take a skill outside 1–10: after all modifiers, Delay is clamped to 1–10.
- **Overshoot (adopted 2026-10-06).** If Pull Forward removes more time than remains, the turn arrives now and the extra is lost; it does not shorten a later Wait. Among turns due at the same time, the normal tie and same-time rules apply.
- **When the next Wait starts (adopted 2026-10-06).** The acting unit's next Wait is set when its skill is used, before the skill's effects. Pushes, pulls, Haste or Slow that reach the user during its own action (for example from a counter) apply on top of it.

Stop was removed from the timeline effects: its role is covered by Push Back and by statuses that skip turns (Frozen). Turn Now (immediately completing a unit's Wait) was removed on 2026-10-05 because no skill uses it; Pull Forward covers the role.

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

**Skills never have modes (adopted 2026-10-06).** If an action needs meaningfully different variants, each variant is a distinct skill. **There are never decision popups during the enemy turn.**

### 6.4 Required actions and skipped turns

Every unit always has three basic actions besides its class skills: its **type-specific attack**, **Move** (section 9.1) and **Skip Turn**. The attack still needs a legal target in reach. Skip Turn costs the same as a Delay 5 skill (section 6.1). There is **no universal Defend command**; defensive actions belong to specific creature or class movesets.

No unit is ever forced to act or move: Skip Turn is always allowed. A turn is skipped automatically only when an effect says so (for example Frozen), with the same cost. (This replaces the earlier rule that a unit must act whenever it has a viable action and has no Wait command; it was changed on 2026-10-05 because Move is almost always possible.)

### 6.5 Event resolution and KO checks

Skills define a fixed order for their effects. **Check KO after each event.** Moving before damage and moving after damage may intentionally produce different outcomes; meaningful order must be visible in the skill description or preview.

A skill may declare a **simultaneous event block**. Apply the whole block together, check its resulting KO state, then collect and process the resulting reactions. Internal slot iteration must not change who receives effects intended to be simultaneous.

Each skill determines whether it continues or stops when its user becomes KO during resolution. Each hit of a multi-hit skill is an event with its own KO check; the skill defines whether remaining hits expire, retarget, or follow another explicit rule.

**Order for using a skill (adopted 2026-10-06):**

1. Check that the skill is usable and the target legal.
2. Pay the Momentum cost (section 6.7).
3. Set the user's next Wait (section 6.1).
4. For each event in the skill's order (or each simultaneous block): apply it, check KO, apply its Momentum shift, resolve its reactions (section 6.6), then check victory (section 12.3).
5. The skill ends.

**Targets that change mid-skill (adopted 2026-10-06).** What later hits of a skill do when their target moved, became illegal or was KO'd after an earlier hit depends on the targeting type. The targeting types are what the skill selects: a **unit**, a **slot** (`direct`) or a **row** (`row`); `melee` and `projectile` are delivery, not targeting.

- **A unit, delivered `melee`.** If the target moves out of reach, the remaining hits are cancelled.
- **A unit, delivered `projectile`.** The remaining shots fly to the target's **original column**. If the target is still in that column (for example moved to another row in it), it can be hit, and interception applies normally. If it left the column, the shots miss.
- **A slot (`direct`).** The remaining hits land on the same coordinate and hit whoever covers it, both units at a 50% overlap.
- **`random`.** Each hit picks again among the targets legal at that moment.
- **`chain`.** Each jump is chosen when it happens, from the targets legal at that moment; if none is legal, the chain ends.
- **Areas (`column`, `row`, `circular`, `all`).** The affected units are determined again for each hit, from the positions at that moment.

A KO'd target still follows its skill's own KO rule (section 6.5 above).

### 6.6 Reaction chains

Reactions are additional effects, not automatically normal turns. They change the timeline only when explicitly defined to do so.

**Adopted reaction rules:**

- **Propagation.** Reactions can trigger further reactions by default. A reaction that causes a hit can trigger reactions to that hit. A reaction may state that it cannot.
- **No chain limit for now.** There is no global chain limit, no self-recursion rule and no once-per-chain rule. An endless chain is a design bug that testing must find. The AI-versus-AI harness (section 2.4) must detect and report chains that do not terminate. The detection threshold is an implementation detail, not a game rule.
- **Reaction hits.** A damaging reaction hits its target directly: it ignores interception, and defense applies as usual. A reaction has **its own tags** and does not inherit the tags of the skill or event that triggered it, or of the skill that created it. A reaction that should count as `melee` (for example the Spike and Parry counters) says so itself, so such counters can trigger melee-only reactions and can chain.
- **Timing.** Reactions resolve after each event or simultaneous block, before the triggering skill's next event.
- **KO'd owners.** A unit KO'd by the triggering event does not react, except through explicit "when KO'd" triggers.
- **"Once per hit".** Reactions limited to once use hit events as the unit. A multi-hit skill can trigger such a reaction once per hit.
- **Priority.** Simultaneous reactions use three levels. **First:** effects that change or remove other effects (cleanse, reveal). **Normal:** damage, healing and statuses. **Last:** timeline changes (pulls and pushes). Reactions of the same level resolve by their owners' timeline order, with explicit skill exceptions allowed. If owners share a timeline position, the tie rule of section 6.1 applies: the higher Speed first, then the seeded roll.

### 6.7 Momentum and the battle clean slate

**Momentum is a single battle-wide contested meter shared by both opposing parties, not two independent party pools.** It represents which side currently has the flow of battle in its favor.

**Every battle starts with all participating units at full HP and the Momentum meter at neutral zero.** Momentum and all other temporary combat states never carry over between battles.

Momentum represents the current flow of battle, rather than a resource generated by fixed values on individual skills. A party's successful attacks generally move the meter toward that party's advantage; its failed attacks and successful enemy attacks generally move it away. The two sides contest the same meter.

Fast creatures naturally build Momentum effectively because they can perform several fast actions in a short span of timeline time. If those attacks succeed, they can quickly shift Momentum in their party's favor.

Many of the most powerful attacks and abilities require Momentum to use. This connects fast units that build Momentum through repeated successful actions with powerful units or abilities that spend the accumulated Momentum for high-impact effects.

Momentum gain is not an explicit **Momentum Gain** stat attached to skills. Adopted rules:

- The meter runs from **−10 to +10**, with neutral at 0.
- A damaging hit that lands moves it **1** toward the acting side. A KO gives no bonus.
- A missed attack moves it 1 against the attacker. A hit that is intercepted moves it **0**. A hit on a unit of the **acting side itself** (friendly fire, or a skill used on an ally) moves it 0 either way, so Momentum cannot be farmed by hitting your own units. A hit that a shield **fully absorbs** also moves it 0. A partly absorbed hit still moves it 1.
- A skill or status may modify the shift (for example, double Momentum for hits on a marked target).
- A skill may have a Momentum cost from **0 to 10**. It requires the user's side to be at least +N ahead and, when used, pushes the meter **N toward neutral**. If the requirement is not met, the UI states it.

Further adopted rules:

- **Limits.** The meter clamps at −10 and +10, and any excess shift is lost.
- **Snowballing.** There is no snowball safeguard for now. Momentum costs already spend a lead. Revisit after playtesting.
- **Periodic effects.** Poison, bleeding and similar ticks shift the meter by 0.
- **Reactions and counters.** Their hits follow the normal rule (1 toward the acting side if they land, 1 against if they miss, 0 if intercepted).
- **Multi-hit and area actions.** By default an action causes **one** shift: 1 toward its side if any hit lands, otherwise 1 against if it missed, and 0 if everything was intercepted. A skill may state otherwise.
- **Auto events.** Each auto event is its own event on the timeline and shifts the meter when it resolves, like a separate action. Rapid Shots therefore shifts the meter for each of its three shots.

- **Non-damaging actions.** A skill that deals no damage moves the meter by 0, even when it is harmful and lands (a Mark, Provoke, Silence, or a push without damage). Move and Skip Turn also move it by 0. Healing already moves it by 0.
- **Paying a cost.** A Momentum cost is paid when the skill is used, before any of its effects. It is never refunded, even if the skill achieves nothing (every hit intercepted or absorbed, or a chance-based effect failing).
- **Several shifts in one simultaneous block.** They are applied one by one in the block's event order, and the meter is clamped after each one.

**Core principle:** successful combat builds Momentum, mistakes and enemy success erode it, and powerful actions often consume it.

## 7. Battlefield and formation

### 7.1 Centered rows and capacity

Each side has three horizontal rows: **Front, Middle, and Rear**, separated from the opponent by a central battle line. There are **no visibly fixed card slots**. Units within each row are automatically centered and placed directly beside one another without gaps, similar to Hearthstone's automatic minion positioning. All normal cards have exactly the same width.

Each row may contain at most **six normal units**. Regular six-unit parties are not expected to fill all three rows; additional capacity mainly supports summons and other temporary units. The player may place all six party members in one row, with no required distribution by class.

Eighteen normal units per side, the sum of the row capacities, is the adopted per-side cap (section 11.3). It may be revisited after prototype readability testing. Standard party size and total battlefield capacity are separate limits.

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

The order of units within a row matters for adjacency, certain area attacks, positional abilities, and other effects. Internally, the game tracks ordered positions and their derived alignment. Row distance and horizontal alignment remain separate concepts. References elsewhere to a slot mean an occupied ordered position or capacity, not a permanently numbered visible cell; future position-bound effects must define how they interact with row recentering. **Default (adopted 2026-10-06):** an effect tied to a spot (a trap, a slot effect, a delayed `direct` effect) stays on its coordinate when rows recenter. When it triggers, it affects the unit covering that coordinate; if two units overlap it by 50% each, it affects **both** (as for `direct`, section 10.1).

There is no general facing, rotation, or rear-attack subsystem. A backstab or similar behavior must be an explicit skill effect.

### 7.4 Nominal row versus effective distance

Rows retain their identity when empty. Units do not automatically move forward when allies fall.

For range calculations, empty rows are skipped. A unit in Rear remains in Rear for row-dependent skills, even if it is now the nearest reachable opponent.

Example: if the enemy Front and Middle contain no relevant active visible units, the enemy Rear loses the distance protection those rows would otherwise provide. Reviving a visible unit in Front can restore that protection without changing the surviving units' row or order.

**Distance between two units on the same side** (used for ranges of cover, heal, swap and similar skills): **rows apart + whole cards of horizontal gap between them.** Touching neighbors in one row have distance 0. A neighbor one row away counts 1. The largest possible distance is 6. A skill's range is measured with this formula for a target on the user's own side and with the opposing-row table below for a target on the other side, so one range value works for any unit. Every skill states its own target type and its maximum distance (**range**); there is no general penalty for distance. Reaching far is paid for in that skill's Delay and Momentum cost.

Both the attacker's row and the target's row matter to reach. A melee attack from Rear cannot automatically reach the opposing Rear. The original discussion proposed the following distances before empty-row compression:

| Attacker row | Enemy Front | Enemy Middle | Enemy Rear |
|---|---:|---:|---:|
| Front | 1 | 2 | 3 |
| Middle | 2 | 3 | 4 |
| Rear | 3 | 4 | 5 |

**Adopted opposing distance.** The table is final. It equals `attacker's effective row + target's effective row − 1`, counting Front as 1, Middle as 2 and Rear as 3 **after skipping empty rows on both sides**. A row is skipped when it has no living unit. On the opposing side, stealthed units and KO bodies do not make a row count (sections 10.1 and 10.2). The target's own row always counts. On the user's own side, stealthed allies do count. A unit's actual row never changes: a Rear unit is always in Rear for row-dependent skills, and only the distance calculation ignores empty rows.

Example: your only unit is in Rear, and the enemy has units only in Middle and Rear. Your Rear counts as row 1 and the enemy Middle as row 1, so the distance is 1. With every row occupied it would be 4.

- Horizontal position is ignored for opposing distance. It still matters for cover and interception.
- "Any visible unit" is unlimited: every visible unit is legal at any distance.
- Range bonuses, such as Far Reach's +2, can exceed the table maximum. Delay and Momentum cost pay for reach.
- Range is checked **when the skill is cast**. Delayed events do not check it again when they land.

**Same-side distance also skips empty rows.** Rows apart is counted between effective rows, so an ally in Rear is 1 row from your Front when your Middle is empty, plus the whole cards of horizontal gap. Only range calculation ignores empty rows. Physical interactions (adjacency for area skills, bumps and forced movement) use the nominal rows (section 8.4).

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

The underlying system should avoid assumptions that would make larger units impossible later. Bosses or special encounters may eventually use creatures that consume the space or capacity of multiple normal units. **Bosses are normal-size units (adopted 2026-10-06).** A boss uses one card width and one unit of row capacity like any unit and follows every normal rule. It differs only through higher HP or other stats, which may exceed its tier's bands in creatures.md, and its skills. These are explicit content values, not hidden multipliers (section 12.3). Large multi-slot units and multi-row footprints are not planned.

Unresolved future questions include movement fit, protection across horizontal alignments, targeting a large body, and whether overlapping an area several times deals damage once or repeatedly.

## 8. Targeting, range, and protection

### 8.1 Target legality

Skills specify valid targets, range, eligible starting rows, and relevant restrictions. A skill selects a unit unless it carries a targeting tag for another target type (section 4.7), such as `row`, which selects a row.

Explicit position- or row-targeted abilities were discussed as exceptions, particularly for area effects against hidden units. The exact exception list is open. General row targeting must not silently become available for every skill.

**Default target side.** Unless a skill states otherwise, a skill may target **any legal unit, ally or enemy**. Shove can move an ally. A heal can be used on an enemy, for example to trigger an unwanted effect. Resurrection skills can target allied or enemy bodies. A skill that is restricted to allies or enemies must say so in its Reach. Skills that choose targets automatically (`random`, `all`, `chain` jumps) fix their own side, since a random hit on an ally is not a useful default. Area shapes (`circular`, `column`, `row`) affect the units of the selected target's side. The class document lists the restricted skills with reasons.

Target legality and interception are separate: an enemy may be a legal target while defenders still have a chance to intercept the attack.

### 8.2 Interception

Eligible defenders in front of a target may intercept appropriate attacks. Protection resolves from nearer defensive layers toward the target, with Front considered before Middle where applicable.

Normal cover depends on horizontal overlap between a forward unit and the unit behind it:

| Horizontal overlap | Normal covering relationship |
|---|---|
| 100% | Full column alignment: strongest normal cover, with a 50% chance to intercept. |
| 50% | Partial cover: a 25% chance to intercept. |
| None | That forward unit provides no normal cover to this rearward unit. |

**Which skills can be intercepted (adopted).** Only skills with the `melee` or `projectile` tag can be intercepted, whichever side uses them and whatever they do. A `projectile` heal can therefore be intercepted, while a `direct` heal cannot. A `melee` or `projectile` skill ignores interception only if it says so (for example Sniper Shot). **Intercepted effects that are not damage (adopted).** An interceptor receives the intercepted effect instead of the target. On a full interception it receives the whole effect. On a partial interception an amount that can be split, such as healing, is split like damage: the interceptor receives half, rounded down, and the rest continues. An effect that cannot be split, such as a status or a push, goes entirely to the interceptor, as on a full interception.

**Defender order (adopted).** Defenders are tried from the nearest row toward the target. Within one row they are tried **from left to right**, as the player sees the board.

**Adopted chances.** A forward unit that is not the intended target (passively targeted) intercepts with a **50%** chance at **100% overlap** and a **25%** chance at **50% overlap**. When it intercepts, it takes the **full hit (50%)** or a **partial hit (50%)**. A partial hit splits the damage in half: the interceptor takes half **rounded down**, and the remainder continues down the interception chain. A hit of 1 cannot be split and goes entirely to the interceptor. An intercepted hit changes Momentum by 0. Individual skills, passives, statuses, and attack types may override or modify these standard rules. Summons and constructs that act as regular units can intercept like any unit.

- A full interception can stop or take over the incoming attack according to the effect.
- A partial interception leaves a remaining attack or effect.
- That remainder continues toward the original target.
- Further eligible defenders may attempt to intercept it.
- Partial interception does not remove a later defender's defense opportunity or the intended target's applicable defense.

Sequential protection is intentional and must obey the reaction-chain safeguards in section 6.6.

The project owner's examples establish two important skill concepts: **Sniper Shot** can bypass interception, while **Shield Wall** can allow three cooperating Front units to block 100% of physical attacks directed at rear rows. Their exact requirements, exceptions, and costs remain to be designed.

Proposed interception effects include taking the attack, absorbing part of its damage, reducing its remaining strength, removing one effect, or stopping a projectile component. The attack types eligible for each defense, exact probabilities, processing order, and interaction with the four stats are open.

An area effect can be intercepted when it is `melee` or `projectile`, like any other skill.

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

Area effects can affect stealthed units physically inside their area. Area effects never hit KO bodies unless the skill says so. Whether they affect allies or the user is defined by the skill.

**Adjacency (adopted).** For `circular`, `chain` and similar patterns, the units adjacent to a unit are its **left and right neighbors in the same row**, plus the units in the row **directly in front of and directly behind it with at least 50% horizontal overlap** (section 7.3). Adjacency is a physical relationship, so it uses the **nominal rows**: an empty row is not skipped, and units in Front and Rear are not adjacent when Middle is empty. Only range calculation ignores empty rows (section 7.4). Bumps and forced movement follow the same rule: a unit shoved from Front into an empty Middle lands there and bumps nothing.

A skill that hits units **behind** its target hits those with **100% overlap** automatically and those with **50% overlap** with a **50%** chance. Stealthed enemies can always be hit by attacks that reach them without targeting them. If a skill's target is KO when it executes, the skill targets units behind that target. **Only resurrection skills can target KO bodies.**

### 8.5 Closest-target ties

Stealth and empty-row rules affect nearest-target selection. **Adopted tie rule:** when several legal targets are equally close, the **leftmost** one as the player sees the board is chosen. The same rule applies to players, the AI and automatic effects, so a skill that says "nearest" never asks for a choice. A skill that selects randomly says so.

## 9. Movement and positional skills

### 9.1 Repositioning

Voluntary repositioning costs a turn, as carried forward in the lane discussion. Skills may combine an action with movement; movement does not always require a separate turn when it is part of a skill.

Useful design examples include an attack from Middle that advances its user, a powerful action that leaves the user exposed, or a weaker action that improves the user's defensive position.

**Move (adopted).** Every unit has a basic Move action:

- It moves the unit to **any other row on its own side**. It cannot reorder a unit within its own row.
- It has **Delay 5**, the same as a skipped turn, and the `move` tag, so it triggers Bleeding (section 10.5).
- The player picks where the unit lands: **any gap in the destination row**, between two units or at either end. The row recenters. A full row cannot be chosen (section 9.2), so Move never bumps.
- Move never swaps. Swapping places with an ally comes only from skills such as Swap Places.

**Movement inside a skill (adopted).** When a skill moves its own user (for example Charge or Leap Away), the movement follows the forced-repositioning rules of section 9.5: realistic landing, bumping, and, if the chain is blocked, nothing moves and every unit in the chain takes 1 bump damage, allies included. A blocked movement does not cancel the rest of the skill: its other effects still resolve, from wherever the user ends up. The skill states the order of movement and damage (section 6.5).

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

**Status: adopted, except the bump damage amount.** Pushes, pulls, swaps, and similar effects move a unit other than the user (for example the Guardian's Haul, Shove, and Swap Places).

- **Reach.** Position and range always matter: a skill can only reposition units within its reach. Each skill defines whether the moved unit can be an enemy, an ally, or both.
- **Realistic landing.** A moved unit keeps its horizontal position, using the half-card coordinates of section 7.3, and lands in the destination row where that position falls. If it lands with **100% overlap** on a unit in the destination row, that unit is **bumped** and shoved on in the same direction, and so on down the chain. If it lands with **50% overlap** between two units, it is **inserted between them** (the row recenters; this uses one unit of capacity). If no unit is within its reach, it joins the end of the row on that side.
- **Bump damage.** A chain either completes or it does not. If every unit in the chain has room, they all move and **no damage** is dealt. If some bumped unit has nowhere to go (the next row is full, for example because of summons, or the formation boundary is reached), **nothing moves** and **bump damage of 1** is dealt to **each unit in the blocked chain**: the moved unit, every unit pressed along the chain, and the blocker. A 50% insertion into a full row is blocked the same way, and the moved unit and the two units it would have been inserted between take 1 each. A long chain can therefore hit many units. Bump damage is fixed at 1 and is not scaled by any stat. It counts as a hit (it breaks stealth, section 10.1) and can KO.
- **Immunity.** Only explicit skill or passive effects grant immunity to forced movement. There is no global resistance stat.
- **KO bodies.** Bodies cannot be moved or swapped except by an explicit skill (section 10.2). A swap with a body fails.
- **Position-bound things.** Effects attached to a unit follow it. Effects attached to a slot stay. **Constructs and traps can be forcibly moved** like units.
- **Reactions.** Forced movement itself does not trigger reactions by default. A passive may opt in explicitly with a "when moved" trigger. Bump damage is a hit, so it **does** trigger "when hit" reactions.
- **Stealth.** Movement does not affect stealth (section 10.1).

- **Pulls.** Pulls such as Haul chain the same way: a pulled unit bumps units in front of it at 100% overlap and shoves them forward, with the same all-or-nothing rule and bump damage.

- **Momentum.** Bump damage shifts the meter by 0, even though it counts as a hit, since it is not an attack.
- **Defenses.** Bump damage is fixed at 1 and cannot be lowered by defenses, shields or any other effect.

All forced repositioning rules are now specified.

## 10. Stealth, incapacitation, and resurrection

### 10.1 Stealth

A fully stealthed unit is ignored by normal opposing direct targeting, effective-range calculations, nearest-target selection, and interception eligibility.

A row containing only stealthed units is effectively empty for the opponent's range calculation. Its units still occupy their physical slots.

Stealth does not grant immunity to area effects. An effect covering the unit's position can still hit it. Reveal abilities may explicitly bypass or remove stealth.

**Adopted stealth rules:**

- **Gaining.** Stealth comes only from skills used in battle. No unit starts a battle stealthed, and there are no natural stealth passives. There is no global row restriction; each skill states its own (for example Prowl needs the user outside the Front row).
- **Breaking.** Stealth ends when the unit **attacks or is hit**, whether or not damage gets through. A hit includes area effects and hits that reach it without targeting it, and a hit a shield fully absorbs still breaks stealth. A hit that is intercepted hits the interceptor instead and does not break it. Damage that is not a hit, such as poison or bleeding ticks, does not break stealth. A skill may override this explicitly (for example Silent Strike). KO also ends it, as KO removes all statuses. Explicit reveal effects (for example Quarry) remove it too.
- **Cleansing.** A cleansing skill that hits a stealthed unit removes its stealth. Because a stealthed enemy cannot be directly targeted, an enemy can only be reached this way by an effect that covers its position.
- **Duration.** Stealth lasts until broken. A skill may add its own timer.
- **Allies.** A stealthed unit never intercepts for its allies (it is ignored for interception eligibility, as above).
- **Visibility.** Stealth is binary: there is no partial visibility, and adjacency or alignment does not reveal a unit. If an entire side is stealthed, a stealth skill that would hide the last visible unit cannot be used (it is not offered as legal).
- **Presentation.** The opponent sees a normal card with a clear stealth cue, and the unit's turns remain on the timeline. A dimmed card is an acceptable presentation of that cue. Stealth limits targeting, not information.
- **Locked targets that become stealthed.** Skills normally resolve at once, so this only concerns delayed events and auto events (section 6.2). A `direct` skill hits a specific slot (section 4.7), so it still hits the occupant after that unit becomes stealthed, and if the target has moved away it hits whatever now occupies the slot. A `projectile` passes the stealthed unit and hits the unit behind it, using the behind rule of section 8.4 (100% overlap automatically, 50% overlap with a 50% chance; if nothing is hit, the shot is lost). Row and area skills hit a position and are unaffected. A skill may state a different behavior.
- **Forced movement.** Being pushed, pulled or swapped does not affect stealth. Bump damage (section 9.5) counts as a hit and breaks it.

A slot is a coordinate on the half-card grid (section 7.3). If the row recenters after the cast, the coordinate stays fixed, so a delayed `direct` effect may hit a different unit than its original target.

If two units overlap the coordinate at 50% each, a `direct` effect hits **both**, matching the 50% overlap idea of section 8.4.

### 10.2 Knocked-out units

A dead or incapacitated unit remains in its row and ordered position and **continues to consume row capacity**. Normal movement and displacement cannot move it. Only an explicit skill may move, remove, replace, or otherwise manipulate the body. Automatic visual recentering of a row is not a movement action and does not remove this occupancy.

This replaces the earlier proposal that a corpse would retain its position but leave its slot free.

Slot blocking is distinct from range and protection. A row containing only knocked-out units contributes no active defenders; bodies do not preserve living interception or effective-distance protection.

### 10.3 Healing and resurrection

Healing and resurrection are intended parts of the system. Keeping the body's slot makes resurrection at its retained position the natural baseline. Reviving an active defender can immediately change range and protection relationships.

**Healing is capped at maximum HP. There is no overheal.** Excess healing does not automatically create shields or additional health.

**0 HP means KO. KO removes all statuses**, including temporary positive and negative effects. Units are not automatically removed from the battlefield.

By default, resurrection returns the unit to its existing slot and schedules it **as though resurrection had been its own action**. It does not grant an immediate free turn. A particular resurrection skill may explicitly override that default. Revival HP, exact scheduling cost, and other consequences belong to the skill definition.

**Adopted KO and revival rules:**

- **Repeat revival.** There is no global limit on how often a unit can be revived in a battle. A revival skill's costs (for example Revive's Momentum 5 and Delay 9) are the limit.
- **No out-of-combat consequence.** KO has no persistent effect. Permanent death is excluded, and every battle starts at full HP (section 6.7).
- **Bodies.** A body stays in its slot until revived. The only body removal is that of KO'd summons (section 11.3), which are removed at once. A later skill may state an exception, in which case the body's slot is freed, the unit cannot be revived, and it still counts as KO for the defeat rule.
- **Pending events.** When a unit is KO'd, its pending auto events and delayed events are removed by default. A skill may state that its events persist. Meteor does, because it has already been launched and needs nothing more from its caster. A channeled event that still needs its owner, such as Rapid Shots' follow-up shots, is removed.
- **Revival HP.** There are no global bounds. Each skill states the HP it returns.
- **Automatic revival.** Effects that revive a unit automatically, such as an Emergency Revival passive or Second Wind, are allowed as explicit skill effects. They are not a default mechanic.
- **Which bodies can be targeted.** See section 8.1: a revival skill can target allied or enemy bodies unless it states otherwise.

### 10.4 Named recovery skills

**Burnout Heal:** massively heals units around the user's current position and sacrifices the user. Its healing effects resolve simultaneously, then their reactions are processed. **Sacrifice timing (adopted default):** the user's KO resolves after the skill's simultaneous block and before reactions are collected, so the user's own reactions do not fire (section 6.6). Exact healing amount, affected pattern, team eligibility, and timeline cost remain to be designed; the discussion used allied area healing as its working example.

**Bound Resurrect:** revives a KO unit, after which the revived unit and the reviver share one HP pool. **Shared pool (adopted):** the pool's maximum is the **sum of both units' maximum HP**. It starts at the reviver's current HP plus the revived unit's revival HP. Every hit on either unit reduces the pool, and healing either unit heals the pool. When the pool reaches zero, **both units are KO**. The link ends when either unit is KO. Its relationship to the rule that KO removes statuses and any further details are decided in the skill's definition.

Other proposed recovery concepts included Life Exchange, Sacrifice, Emergency Revival, temporary Reanimate, Second Wind, and revival with Exhausted-like consequences. They are not a finalized skill roster.

### 10.5 Status framework

Each status has its own application chance, duration, triggering conditions, and stacking behavior as defined by its source skill and status type. There is no universal stack rule. Creature types or skill-tree choices may grant specific status immunities. The suggestion that statuses always apply after a successful hit was rejected.

**Adopted status framework:**

- **What a status is.** Any effect with a duration on a unit is a status: harmful effects, buffs, Orders, Ward, Dampen, marks and stealth. KO removes all of them. Purify and similar effects can remove any status from any unit, unless the status says it cannot be cleansed.
- **Duration clocks.** A status lasts for the affected unit's own turns ("its next 3 turns") or until the caster's next turn (Orders, Ward). **Fixed time-unit durations are not a status clock.** An effect that should cost a number of time units uses the timeline instead, through Push Back, Pull Forward or Delay (section 6.1).
- **Ticking.** There is no global tick timing. Each status states when it ticks. Burning and Poison tick at the start of the affected unit's turn, as the class drafts define. A tick that KOs the unit ends its turn.
- **Reapplication.** A unit has one instance per status type. Reapplying refreshes the duration. There is **no stacking**, and none is planned.
- **Source.** A status keeps going if its source is KO, unless the skill says it ends.
- **Limit.** There is no limit on the number of statuses per unit. The UI handles overflow.
- **Momentum.** Periodic ticks shift Momentum by 0 (section 6.7).
- **Damage from statuses.** Poison, Burning and Bleeding damage is not a hit, but Defense reduces it like any damage (rounded down, minimum 1). Shields absorb Burning damage; they do not absorb Poison or Bleeding.
- **Status chances.** A skill's stated status chance is the real chance. Defense does not lower it.
- **Reapplication with a different duration.** The new duration always replaces the old one, even if it is shorter.
- **Skipped turns.** Durations counted in the unit's turns count every turn that arrives, including skipped ones (Skip Turn, Frozen).
- **Immunities.** Only skills and statuses grant immunities (for example Purify's immunity until the unit's next turn). Creatures have no natural immunities for now (section 3.7). A summon's own definition may list immunities, since it comes from a skill (section 11.2).
- **New statuses.** A status joins the table below only when a skill needs it.

**The named statuses:**

| Status | Effect | Duration |
|---|---|---|
| **Blind** | All the unit's attacks have a 50% chance to miss. A miss shifts Momentum 1 against the unit's side, per section 6.7. | The unit's next 2 turns |
| **Poison** | 1 damage at the start of each of the unit's turns. It does less damage per tick than Burning (2) and lasts longer. | The unit's next 5 turns |
| **Bleeding** | 2 damage after each `melee` or `move` skill the unit uses. | The unit's next 3 such skills |
| **Silenced** | The unit cannot use `magical` skills. | The unit's next 2 turns |
| **Burning** (`fire`) | 2 damage at the start of each of the unit's turns. | The unit's next 3 turns |
| **Chilled** (`frost`) | The unit's Speed is lowered by 2 (section 6.1). | The unit's next 2 turns |
| **Frozen** (`frost`) | The unit's next turn is skipped, at the normal skipped-turn cost (section 6.4). It ends early if the unit is hit by a `fire` skill. | The unit's next turn |

Blind and Silenced are adopted although no skill applies them yet. Any skill may apply any named status.

The status design pool (Rooted, Disarmed, Stunned, Weakened, Exposed, Marked, Fear, Bound and others) remains below as candidates. None is added until a skill needs it.

Additional welcomed design candidates (not decided) are Rooted/Immobilized, Disarmed, Stunned, Slowed/Hasted, Weakened, Enfeebled, Exposed/Armor Broken, Marked, Taunted/Provoked, Fear, Burning, Frozen, and Bound. Delayed/Accelerated can be one-off timeline changes rather than persistent statuses. These are a design pool, not finalized effects. A unit with no viable action because of statuses simply loses that turn.

## 11. Summoning

**Status: in the first-release scope. Kinds, placement, cost, limits, Momentum and status handling are adopted (2026-10-06); section 11.2's first list remains background.** Summons never count as original party members for avoiding defeat. The dedicated discussion explored a model but did not settle its global limits or approve every detail. The Hunter's traps (see [classes_and_skills.md](classes_and_skills.md)) are the first adopted summons: stationary constructs that take a slot, have HP, and can be targeted and intercept like any unit.

### 11.1 Summon kinds (adopted 2026-10-06)

| Kind | Battlefield behavior |
|---|---|
| Full unit | Occupies space, receives turns, uses skills, and can interact with ordinary unit systems. |
| Temporary unit | Behaves as a unit but expires after a duration or condition. When it expires it disappears like a KO'd summon and frees its slot, but it does not count as a KO, so KO-triggered effects do not fire. |
| Stationary construct | Occupies space and produces passive or triggered effects without ordinary turns. Has HP, can be targeted, and can intercept like any unit. |

There is **no assist kind**. A creature that appears only inside one skill (for example a raven swarm that strikes and leaves) is just that skill's effect and its animation. It takes no slot and is not a unit, and it follows the skill's tags: a swarm tagged `projectile` can be intercepted like any projectile (section 8.2).

Examples discussed include skeletons, thorn spirits, wolf spirits, healing totems, ballistae and reactive mushrooms. These are concept examples, not committed content.

### 11.2 Proposed operating rules

- Persistent summons need a valid free position, block it normally, and count toward the row's capacity. They may be placed between existing units, which recenters the row.
- Unit-like summons reuse normal stats, targeting, healing, statuses, movement, and timeline systems where applicable.
- A summon may be player-controlled, act autonomously, or use automatic triggers.
- Duration and the relationship to the summoner belong in the summoning skill.
- A skill may impose its own limit on simultaneous summons.
- Summoning is distinct from permanent collection; temporary summons disappear after combat as part of the battle-wide clean slate.
- A separate global summon resource was not recommended, but the cost model is not finalized.

Possible relationships include lasting until death, lasting for a number of turns, disappearing with the summoner, surviving independently, consuming HP, consuming a body or object, replacing the summoner, or upgrading/sacrificing an existing summon.

Summoning before battle is **not adopted**: summoning happens only through skills in battle.

**Adopted summon rules:**

- **Placement (2026-10-06).** Each summoning skill states where its summon may appear. A skill may allow free placement: the player picks any gap in the allowed rows, including between units, as when a unit moves (section 9.1), and the row recenters.
- **Cost.** Summoning costs only what the skill states, like any skill: its Delay and any Momentum cost. There is no summon resource and no HP cost. As a design guideline, summoning skills are slow (long Delay).
- **Per-skill limits.** By default only the side cap applies. A skill may still state its own limit.
- **Momentum.** Hits by summons and hits on summons move Momentum exactly like any unit's hits (section 6.7).
- **Statuses.** Each summon's definition states in detail which statuses affect it. For example, a ballista may be immune to Poison but can burn. How statuses that tick or count on a unit's own turns work on a summon without turns is defined with that summon.

- **Cap.** The per-side limit is the row capacity, 6 per row and **18 in total**. It may be revisited after readability testing. A summoning skill is unavailable when the side is at its cap or has no free slot, and the UI states why.
- **Placement.** By default the player chooses the insertion point within the rows the skill allows. A skill may fix the position.
- **Control.** Summons that take turns act **autonomously**, following behavior defined in their own summoning skill. There is no global default behavior. Constructs such as traps take no turns.
- **First turn.** A turn-taking summon first acts `11 − Speed` time units after it appears.
- **Lifetime.** A summon lasts until it is KO'd or the battle ends. A skill may add a duration, counted in the summon's own turns or the summoner's turns, never in fixed time units.
- **Summoner KO.** Summons persist when their summoner is KO'd, unless the skill says they disappear.
- **KO'd summons.** A KO'd summon's body is **removed at once and frees its slot**. This is the first body-removal exception (section 10.3). Summons cannot be revived.
- **Stats.** A summon's stats are fixed numbers in its skill (for example Spike's HP 6). They do not scale with the summoner.

### 11.3 Summon capacity

The intended six-member starting party does **not** fill the battlefield's total capacity. Additional row capacity primarily exists for summons and other temporary units, subject to the maximum of six normal units per row.

The eighteen-unit total is adopted as the per-side cap. A separate summon budget is not planned. A skill may impose its own limit. Players are not required to leave starting party vacancies solely to make summoning possible.

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

Boss concepts include pushing or pulling units, disrupting a formation, and manipulating available space. Bosses are normal-size units with higher stats (section 7.6); multi-slot or multi-row bosses are not planned. Slot destruction is a possibility rather than a current core rule.

**Standard defeat condition:** a team loses when all six original members are simultaneously KO. Active summons do not prevent that loss. A revived original member counts as active again. If both sides' last original members are KO'd in the same block, the battle is a **draw**. A team does not automatically lose merely because it currently lacks an offensive action.

**Adopted battle-end rules:**

- **Stalemates.** The game has no time limit and no escalation. Only the AI-versus-AI harness has a technical cap on battle length. It reports battles that reach it, and they are recorded as draws in the statistics.
- **Draws.** In single-player modes a draw counts as a defeat for the player. In local two-player and in AI-versus-AI it is a shared result, recorded as a draw.
- **Surrender.** A player can surrender at any time. It counts as a defeat with no other penalty.
- **Victory check.** Victory is checked after each event's KO check and after that event's reactions have fully resolved. A counter can therefore still KO the attacker and turn a win into a draw. Once the result is decided, pending events and remaining hits are cancelled.
- **Objectives.** Every battle uses the standard defeat rule. Special objectives (boss defeat, survival, protection, positional objectives, interrupting a ritual) are **excluded for now** and may return later.
- **Enemy side.** An enemy side has six original members by default. An encounter may use a different number, and the defeat rule then uses that side's own original members.
- **Enemy behavior.** Enemies use the same search-based AI as AI-versus-AI testing (section 2.5). Difficulty changes how deep it searches or how often it picks a weaker action.
- **Difficulty.** It is set through the enemy roster and formation. There are no hidden stat multipliers.
- **Encounter exceptions (adopted 2026-10-06).** None for now. Encounters differ only in roster, formation and team size. Any other exception must be written into that encounter's definition and approved.

## 13. Card interface and combat presentation

### 13.1 Card role

A card is the visual and functional representation of a battlefield unit, not a traditional object for drawing, playing, or deckbuilding. The card-game prototype uses the intended card presentation; it does not imply a later conversion to 3D gameplay.

The **front** contains:

- A customizable cosmetic frame and an image of the unit from the front.
- The unit's name and the names of all its classes.
- Relevant combat stats, including Life (current and maximum HP), Speed, Power, and Defense.
- Clear status and KO overlays on or around the border.

On the battlefield a unit is shown as a **compact card**: front artwork, HP bar, statuses and KO. The full front appears on hover and in the inspection panel, which shows front and back side by side ([ui_spec.md](ui_spec.md)).

Cosmetic frames must never reduce gameplay readability. KO must be unmistakable, for example through desaturation or darkening in addition to an explicit KO indication.

The **back** uses the same selected cosmetic frame and shows the unit from behind, together with every skill it can currently use. Descriptions show the actual current versions, including all modifications and effects selected through skill trees.

The card back is primarily an **inspection interface**, not an action menu. With future online PvP in mind, players must be able to inspect opposing units and understand exactly which abilities they may use. This inspection requirement does not by itself commit to an online implementation.

### 13.2 Team builder

Party setup uses the same battlefield interface with the enemy side absent. The player's Front, Middle, and Rear rows remain visible; available units are placed and reordered using the same centering and positioning logic as combat.

Setup can provide more room for inspecting card fronts and backs, comparing builds, allocating skill-tree points, and other character-management systems if those features require it. Equipment inspection would apply only if such a system were later selected; it does not establish a general gear system. Combat prioritizes speed and readability, while setup supports deeper inspection and optimization.

Teams are built in a **team editor**, in the manner of deck building: the player picks up to six creatures from the collection and their formation, and saves the team under a name. Several named teams can be saved, and the last used team is preselected. Each mode, including local two-player, starts from a saved team.

Creatures are added, moved and removed **by dragging only** (adopted 2026-10-06); filters, sorting and layout are in ui_spec.md.

### 13.3 Battle interface

The interface has a clear information hierarchy:

| Location | Purpose |
|---|---|
| Unit cards | Current unit state, stats, statuses, and KO. |
| Formation | Positional relationships, adjacency, and covering. |
| Bottom horizontal skill bar | The active unit's immediately available actions, similar to an MMORPG skill interface. |
| Left vertical timeline | The shared temporal state of combat: the next 8 entries from top to bottom, scrollable for more, matching the Momentum bar's height. |
| Right edge | The vertical Momentum meter, about 70% of the board's height, with the player's end at the bottom. |

The player's side is at the bottom and the enemy's at the top, with the two Front rows facing each other in the middle. All rows use the same card size. Interaction details are in [ui_spec.md](ui_spec.md).

All of the active unit's skills should be directly visible, normally 7–9 including the type-specific attack, without nested menus. The intended interaction is **select skill → highlight valid targets → select target → execute**. Invalid targets are visibly unavailable. Skills that require no target input execute directly according to section 6.3. Avoid unnecessary chained decisions and repeated confirmation prompts.

During targeting, area attacks and skills affecting additional units should preview all affected cards whenever possible. Target inspection also exposes range, protection, and relevant exceptions.

The timeline is permanently displayed as a vertical list on the left. Selecting a skill previews where the acting unit's next turn would move before execution. Skills that manipulate other timeline positions preview their consequences whenever practical. Already-created delayed effects, such as a future comet impact, may appear as timeline events, and auto events (such as queued shots) appear with their owner; ordinary attack animations do not. Enemy units' unchosen future attacks are not revealed.

### 13.4 Animation and effects

Cards generally **do not physically move around the battlefield when attacking**. Attacks primarily use effects originating from the acting unit and traveling toward or appearing on the targets. Brief shakes, vibration, flashes, and other hit reactions are allowed, with stronger attacks potentially producing stronger reactions. Cards must remain spatially stable enough for the battlefield to be read immediately. Explicit repositioning skills still change formation according to their rules.

Healing, buffs, debuffs, KO, and other important outcomes need distinct feedback without unreadable visual noise. The new presentation brief also names critical-hit feedback; because the current combat rules explicitly exclude separate critical hits, that feedback is conditional on a future rule change and does not introduce critical-hit mechanics here.

Statuses may use icons, border effects, or other clear overlays. Gameplay information always takes priority over cosmetic frames. Visual destruction must not make a KO unit's retained position and capacity appear vacant.

Earlier examples of cards being drawn or played do not establish a draw-and-hand combat system. Effects should be short, clear, and restrained: players must understand who acted, who was affected, which units remain active, and where everyone stands.

### 13.5 Contextual relationships and presentation principle

Unit relationships may be visualized temporarily when relevant or inspected. Hovering over an adjacency passive can highlight or connect the neighboring cards it affects; guards, auras, summons, and combos can similarly reveal their relationships. These indicators should generally not remain permanently visible.

Preserve a strong distinction between **visual simplicity and mechanical depth**. Players see clean, centered card rows; internally, ordered positions use half-card-width alignment. Formation, cover, adjacency, summons, targeting, and future positional mechanics should work consistently without exposing unnecessary grids or slots.

The default prototype is a six-versus-six battlefield with three rows per side, a left vertical timeline, a bottom skill bar, card fronts for combat state, card backs for inspection and current skill information, and attack effects that leave cards spatially stable. Actual prototype testing must determine whether party size or total capacity needs to be reduced as described in sections 3.1 and 7.1.

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

Not used. Cards do not communicate creature size: every creature fills its frame (production.md §1.6). Size contrast was part of the former 3D direction. Size tiers in creatures.md only set stat blocks.

### 14.4 Color and detail

Use controlled, recognizable palettes and broad color zones. Warmth and variety should not become visual noise. Strong silhouettes and primary forms matter more than dense surface detail.

The art guide proposed two to five major color blocks with a selective accent, modest surface patterns, and subtle rather than heavy outlines. These are production guidelines, not fixed gameplay categories.

### 14.5 Biomes and environments

Visually distinct, relatively isolated biomes were accepted in the original direction. Their role in the card game may include artwork settings, encounter backgrounds, or campaign identity; the delivery format remains open.

The art guide proposed Forest, Plains/Meadow, Coast/Shore, Swamp, Desert, Snow/Frostlands, Volcanic/Ashlands, Crystal/Mystic regions, Ancient Ruins, and Caves/Underground. Optional ideas included fairy groves, autumn woods, high mountains, luminous wetlands, and storm cliffs.

These are a proposed palette and setting library, not a confirmed launch-region list.

### 14.6 UI and VFX style

Use clean panels, modest ornament, generous spacing, bold icons, legible text, and comfortable touch targets. Effects may have recognizable elemental identities, but should not obscure the board or create constant screen-filling spectacle.

Art files, backgrounds, sound, music and settings are adopted in [production.md](production.md), section 1.6.

### 14.7 Existing concept exploration

The card-art discussion began with five animals and five fantasy creatures, initially excluding humans. Later requests added batches of ten animals, ten humanoid fantasy creatures, ten large extravagant fantasy creatures, ten male humans, and ten female humans.

**Artwork delivery rule:** create separate artwork images in a card-friendly aspect ratio, not finished cards with baked-in UI and not a single collage. The precise numerical aspect ratio is not established in the recovered text.

The browser exposes generated-image galleries, but the individual images have not been cataloged as a named creature roster in this document. The text establishes direction and requested batch sizes, not approved creature statistics. Fox and otter are explicitly referenced in the expression feedback.

## 15. World and narrative

**Status: story campaign required; narrative details open.** Players explore the campaign and unlock units, then continue with post-story challenge encounters and coin unlocks (section 2.2). Story presentation and the player's role are tracked in [production.md](production.md).

The narrative should explain why the player encounters different beings, why they join a team, and why new combinations matter. A lightweight central conflict with local stories was proposed, with an explorer, wanderer, researcher, mercenary, or guardian-like player role rather than a required chosen-one premise.

Possible narrative layers include regional conflicts and emergent party events or relationships. Reactions to party composition, recruitment stories, rivalries, and personal quests remain proposals rather than required campaign features.

The suggested ten to fifteen main chapters was an example. There is no approved plot, named setting, faction roster, protagonist, or chapter count. The original free-roaming 3D world is superseded; any story structure must now suit the card game.

## 16. Technical and production direction

### 16.1 Engine and architecture

Godot 4.7 with .NET is installed and the repository contains a Godot project. No code exists yet.

The adopted technical foundation (language, rules core, content format, AI search, Godot app, saving) is in [production.md](production.md), section 2.

The earlier proposal below is kept for context.

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

Engine costs, storefront fees, external assets, sound, fonts, and possible service costs were discussed as planning topics. This document sets no budget or commercial license conclusion. The business model, stores and other non-mechanics decisions are in [production.md](production.md). Licensing must be checked when delivery decisions are made.

## 17. Playable scope and development priorities

### 17.1 First playable direction

Begin implementation with **AI-versus-AI combat for development testing**, before adding player-controlled modes and campaign progression. Both sides should use the same combat rules that player-controlled units will use.

The first playable should exercise:

- A small selection of humans, animals, and fantasy creatures.
- Six-member formation across three rows.
- Direct testing of active skills, passives, and modifiers on level-20 units; tree organization and point allocation follow later.
- The four global stats and skill-specific damage scaling.
- Visible turn order and a limited set of timing effects.
- Movement, targeting, and a readable form of protection.
- Card-based presentation and mobile-conscious controls.

The order of work after the AI-versus-AI milestone is adopted in [production.md](production.md), section 3. Summons are in the first release; terrain and large multi-slot units are not (section 20.1). Prepare online-facing interfaces, but keep online PvP implementation gated on explicit owner greenlight.

### 17.2 Content counts remain proposals

Different discussions suggested around 10 recruits for an early slice or 15–20 for a combat prototype, and between 6–8, 8–12, or 10–12 initial trees. These are alternative scoping suggestions, not simultaneous targets.

The current class roster is fixed at seven: Hunter, Elementalist, Healer, Warrior, Guardian, Feral, and Leader. Creature roster size, enemy count, and final content budget remain open.

### 17.3 Deferred or unselected systems

Complex terrain, large multi-slot units, emergent relationships and genetic variation remain deferred. Summons are in the first release and still need full specification (section 11). Bosses are normal-size units with higher stats (section 7.6). The story campaign and post-story content are required; their detailed content remains open.

Online PvP is planned with interface preparation now, but implementation requires explicit owner greenlight. Its detailed rules and networking specification remain open. Cooperative play, rankings, trading, crafting, an equipment economy and achievements remain unspecified. The business model and saving are adopted in [production.md](production.md).

## 18. Identity and reference boundaries

Everkin remains a working title. A previous naming discussion raised potential existing-name conflicts; final commercial naming and clearance are unresolved. This document records that issue without treating the earlier legal commentary as clearance or a current legal assessment.

All artwork, character designs, UI, names, text, music, and branding must have an independent identity. References guide broad design goals rather than supplying assets or exact presentation.

Previously discussed reference games include FFX for turn-order tactics; Hearthstone for card presentation and its deck builder as the model for the team editor; Siralim Ultimate and Monster Sanctuary for team-building and build synergies; and LumenTale, World of Final Fantasy, Cassette Beasts, Dragon Quest Monsters, and Monster Hunter Stories as comparison points. Their individual rules are not adopted by default.

Minecraft and Super Mario 64 were earlier references for simple geometry and readable 3D form. They no longer define a 3D asset-production requirement.

Earlier naming candidates were Wildbound, Kinforge, Riftkin, Beastfall, Veyra, Tamerift, Everkin, Roamkin, Feralis, and Bondfall. They are historical alternatives, not cleared commercial names.

## 19. Decisions still required

| Area | Outstanding specification |
|---|---|
| Party | Prototype validation of six-versus-six readability, possible four- or five-unit standard, enemy exceptions. No combat reserve swapping. Each creature at most once per player team (enemies may repeat) is adopted (section 2.2). |
| Timeline | None open. Delay bounds (clamped to 1–10) are adopted and Turn Now was removed (section 6.1). Initial progress, ties, Haste/Slow, Reset, skipped turns and same-time order are adopted (section 6.1). Speed 1–10 (10 fastest), skill Delay 1–10 (1 quickest), and Wait = Delay + 11 − Speed are adopted (section 6.1). |
| Momentum | None open. Non-damaging actions, payment timing, refunds and simultaneous shifts are adopted (section 6.7). Limits, snowball (none for now), periodic, reaction and multi-hit shifts are adopted (section 6.7). The scale, gain, spending, and interception rules are adopted (section 6.7). One shared meter starts at neutral zero; no per-skill Momentum Gain stat or between-battle carryover. |
| Skills | Individual prerequisites, Momentum and other consequence costs, effect order, and delayed-event definitions. One type-specific attack plus 6–8 additional usable skills is the target, not a loadout cap; no mana or cooldowns. |
| Damage | Multiple shields on one unit (deferred to the skill revisit after AI-versus-AI testing). Healing scaling is adopted (section 5.2). Roll, shield, defense, final reduction order, defense bounds (0–75%, additive), penetration, and fixed damage are adopted (section 5.2). Uniform rolls, no crits, and round-down with a minimum of 1 are fixed. |
| Defense | Eligible attacks for other defenses and precise interaction with damage events. Interception chances, the partial-hit split, `melee`/`projectile` eligibility, intercepted non-damaging effects, left-to-right defender order and the nearest-target tie rule are adopted (sections 8.2 and 8.5). |
| Reactions | None open. Propagation, priority levels, tie-breaks, timing and KO'd owners are adopted (section 6.6). There is deliberately no chain limit or recursion safeguard for now: AI-versus-AI testing must detect endless chains. |
| Range | Per-skill ranges. Opposing and same-side distance (both skip empty rows), unlimited "any visible", uncapped range bonuses and cast-time checks are adopted (section 7.4). Adjacency, which uses nominal rows, is adopted (section 8.4). |
| Movement | None open. Move (any row, Delay 5, chosen gap, no swaps), movement inside skills and forced repositioning are adopted (sections 9.1 and 9.5). Skip Turn is always available (section 6.4). |
| Statuses | Further candidates, added only when a skill needs them. The framework (clocks, ticking, reapplication, cleansing, source, limit, damage from statuses, chances, skipped turns, immunities) and Blind, Poison, Bleeding, Silenced, Burning, Chilled and Frozen are adopted (section 10.5), as are stealth rules (section 10.1). Application chances are explicit; KO removes all statuses. |
| KO and revival | Per-skill revival HP and recovery cost, and what a draw means in each mode. Repeat revival, no out-of-combat consequence, bodies, pending events, sacrifice timing, the shared pool, automatic revival and body targeting are adopted (sections 8.1, 10.3 and 10.4). |
| Summons | Per-summon status rules and individual summon content. Kinds, placement, cost, limits, Momentum and expiry are adopted (section 11). In the first release (section 20.1). Summon costs (set per skill). Autonomous behavior is defined per skill. Cap (18), placement, first turn, lifetime, summoner KO, KO'd bodies and stats are adopted (section 11.3). Extra capacity beyond the starting party is supported. |
| Builds | Class/skill designs and assignments, conflicting modifiers; later tree organization, node costs/ranks, prerequisites, progression/stat growth, and confirmation of level-up awards. Initial level-20 units receive all creature/class skills. Mixed meaningful effects and free out-of-battle respec are selected; trees and spending come later. |
| Creatures | Balance of the Draft roster in creatures.md (testing), the first human recruits, art framing and cleanup. Tiers, stat ranges, the attack library rule, class counts, no natural traits and text IDs are adopted (section 3.7). |
| Recruitment | Guaranteed encounter-to-unit reward assignments, the tutorial's creatures, coin sources and prices, individual persistence. One copy per creature, one fixed reward per encounter from the defeated team, rewards given once, the three-battle tutorial, post-story challenge encounters, coins, and fixed human recruits are adopted (sections 2.2 and 3.4). Other acquisition methods are not currently required. |
| Test harness | Search depth per difficulty level (after measuring speed) and the first evaluation weights. Team drawing, formation, search, information, cap, run size, records, report and flags are adopted (section 2.5). |
| Encounters | Specific encounter rosters, boss stats, and the AI's difficulty settings. Draws, surrender, stalemates, victory timing, objectives (excluded for now), enemy size, behavior and difficulty are adopted (section 12.3). All six original members KO means defeat. |
| Product structure | Campaign content (locations, encounters, boss rosters), later full campaign design, and final content. Stage size (about five battles plus a boss), fixed battle order, no team restrictions and no restart are adopted (section 2.2). Modes, free battles, local two-player, campaign losses, random battles (no rewards), saved teams and saving are adopted (sections 2.2, 2.4 and 13.2, and production.md). AI-versus-AI testing comes first; online implementation requires greenlight. |
| Presentation | Final card styling, mobile readability, gestures, and prototype validation of party/capacity limits. Centered rows, front/back roles, left vertical timeline, and bottom skill bar are selected. |
| Delivery | Technical validation, cloud saves, final name. Platforms, stores, business model, languages, accessibility, crash reports, save files and the technical foundation are adopted ([production.md](production.md)). The battle screen arrangement, interactions and layout sizes are adopted (section 13.3, ui_spec.md). |

These are intentional gaps in the current design, not permission to inherit equivalent rules from Hearthstone or another reference game.

## 20. Details to complete before implementation

### 20.1 Finished-game scope

Required scope includes AI-versus-AI development testing, single-player AI battles, local two-player, an exploratory story campaign unlocking units, and at least one post-story random-battle mode unlocking further units. Target Windows, Android, and iOS in landscape only. Online PvP interfaces must be prepared, while online implementation remains gated on explicit owner greenlight. Select the complete roster and remaining systems. **First-release scope (adopted 2026-10-06):** summons are developed fully; bosses are normal-size units with higher stats (section 7.6); terrain and large multi-slot units are excluded.

For each required mode, define the full flow from launching the game through party setup, battle, results, and replay or progression. Specify AI behavior/difficulty and local handover rules. Define future online requirements and interface boundaries without implementing matchmaking, transport, or services before greenlight.

Use illustrated locations and a four-stage line of fixed-order battles for current campaign testing, with guaranteed encounter unit rewards. Define the tutorial's creatures, specific rewards, coin sources and prices, and the completion flow; the tutorial, stage size, duplicates, loss consequences, replays, post-story challenge encounters, coins and saved teams are adopted (sections 2.2 and 13.2). Respec is free outside battle. Full campaign design and normal progression details remain for later; test units start at level 20, and skills precede trees and point allocation. Unselected systems should be explicitly excluded rather than left ambiguous.

### 20.2 Exact combat details

In addition to section 19, resolve these edge cases:

- Timeline: none open. Overshoot, when the next Wait starts, Speed and Delay bounds, the skipped-turn cost and the time model are adopted (section 6.1).
- Momentum: all adopted (section 6.7).
- Damage: multiple shields on one unit (deferred to the skill revisit). Healing scaling, sampling, rounding, defense bounds, penetration, shield order and shared versus independent rolls are adopted.
- Resolution: none open. The full order for using a skill and the rules for targets that change mid-skill are adopted (section 6.5). Reaction priorities, ties and the absence of a chain limit are adopted (section 6.6).
- Statuses: none open beyond new candidates. Immunities, damage from statuses, clocks, tick timing, reapplication, cleansing, source removal and shared HP are adopted (sections 10.4 and 10.5).
- Formation: none open. Spot-bound effects stay on their coordinate (section 7.3). Cover eligibility, defender order, Move, swaps and failed movement are adopted (sections 8.2 and 9.1).
- Battle boundaries: none open. Encounter exceptions (none for now), persistent consequences (none), simultaneous wipes (draw), stalemates, surrender and victory timing are adopted (section 12.3). Full starting HP, neutral-zero Momentum, and cleared temporary combat states are selected.

### 20.3 Complete content definitions

Each included creature needs a stable ID, name, front/rear artwork, framing, all four stats and growth rules, classes, its one type-specific attack, natural passives/immunities, and acquisition rules. Artwork filenames are not approved creature names or combat values.

Each class needs its complete skill-tree graph: node IDs, costs, ranks, prerequisites, branches, exclusivity, effects, and respec behavior. Define how multiple modifiers combine and how the final usable skill is displayed.

Each skill needs availability conditions, valid targets and range, area pattern, timing multiplier, Momentum/other costs and payment behavior, ordered effects, probabilities and formulas, reactions, KO/interruption/retargeting behavior, and presentation. Do not add a per-skill Momentum Gain field.

Each status needs application, duration clock, tick/expiry order, stacking/reapplication, effects, cleansing/immunity, and KO/reset behavior. Each encounter needs its roster, builds, formation, opponent behavior, outcome conditions, and rewards or exceptions if applicable.

Final content lists must be complete; examples and partially filled entries do not count as implemented content.

### 20.4 Technical and delivery requirements

Finalize the authoritative rules state, UI preview/execution relationship, event ordering, and build/export process for the selected platforms. Language, the rules-core split, content format and IDs, and random-number control are adopted (section 16.1).

Save timing, format, versioning, corruption recovery, battle resume, supported devices and screen shapes, languages, accessibility and crash reporting are adopted in [production.md](production.md). Still to resolve: online responsibilities if relevant, performance targets, sound/music/VFX inventory, tutorials, settings, and missing-data/error behavior. Detailed screen behavior belongs in [ui_spec.md](ui_spec.md).

### 20.5 Worked checks

Use concrete examples alongside rules to verify implementation. Initial checks include:

| Situation | Expected result |
|---|---|
| Speed 5 uses a Delay 5 skill | Wait is 5 + (11 − 5) = 11 time units. |
| A Speed 8 unit and a Speed 5 unit start a battle | First Waits are 3 and 6, so the Speed 8 unit acts first. |
| Two units with equal remaining Wait, Speed 7 and Speed 6 | The Speed 7 unit acts first. |
| Two units with equal Wait and equal Speed | A seeded random roll decides, and the same seed gives the same result. |
| A unit has no viable action at Speed 9 | It skips its turn and waits 5 + (11 − 9) = 7 time units. |
| A status expires at the same time a unit's turn arrives | The status expires first, so the unit acts without it. |
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
| A 9-damage hit lands on a unit with 50% defense and no shield | 9 × 0.5 = 4.5, rounded down to 4. |
| The same hit lands on a unit with a shield absorbing 8 and 50% defense | The shield takes 8 raw, the remaining 1 is reduced to 0.5 and becomes 1 (minimum 1). |
| A 7-damage hit lands on a unit with a shield absorbing 8 | Nothing passes the shield, so no damage is dealt, and Momentum does not change. |
| Two sources give 40% and 50% Defense | The total is 75%, the cap. |
| Exposed is applied to a unit with 0% defense | Its defense stays at 0%. |
| A skill with 30 points of penetration hits a unit with 20% defense | The unit's defense counts as 0% for that hit. |
| A counter deals fixed 2 damage to a unit with 50% defense | It deals 1. |
| A 9-damage hit lands on a unit with 50% defense and Dampen | 9 × 0.5 = 4.5 rounds to 4, then Dampen halves it to 2. |
| A 9-damage hit lands on a unit with Ward (8), 50% defense and Dampen | Ward takes 8, the remaining 1 becomes 1 after defense, and Dampen leaves it at 1 (minimum 1). |
| A Spike trap's counter hits an attacker who has its own "when hit" counter | The attacker's counter triggers on that hit, since reactions can trigger reactions. |
| A Warrior with Parry meleeing a Spike trap | The trap's Spike counter (melee) hits the Warrior, Parry counters it for 4 (melee) onto the trap, and, if the trap survives (a KO'd trap does not react), its Spike hits the Warrior again. Parry is spent after the first counter, so the chain ends there. |
| A reaction is created by a `melee` skill but its own text does not say `melee` | Its hits are not `melee`. |
| A multi-hit skill hits a unit with a once-per-hit reaction three times | The reaction triggers three times. |
| A counter and a timeline pull trigger from the same hit | The counter (Normal) resolves before the pull (Last). |
| A unit is KO'd by a hit and had a "when hit" counter | The counter does not fire. |
| A Speed 6 unit uses Move | It lands in the gap its player chose and waits 5 + (11 − 6) = 10 time units. |
| A unit chooses Skip Turn at Speed 9 | It waits 5 + (11 − 9) = 7 time units. |
| A unit tries to Move into a full row | That row is not offered. |
| A Warrior uses Charge from Middle while Front is full | Charge's move is blocked, every unit in the chain takes 1 bump damage, and the Warrior stays in Middle and still makes the hit if the target is in reach. |
| A `direct` heal is aimed at a unit with an ally in front of it | It cannot be intercepted. |
| A `projectile` heal is aimed at a unit with an ally in front of it | It can be intercepted. |
| Two units in the row in front each half-cover the target | The left one, as the player sees it, rolls to intercept first. |
| A skill hits the nearest enemy and two are equally near | The leftmost one is hit. |
| A skill with Delay 9 gets +3 Delay from an effect | Its Delay is 10. |
| A unit uses a Momentum 2 skill with its side at +2 and the first hit lands | The cost is paid first (meter 0), then the hit moves it to +1. |
| Every hit of a Momentum 3 skill is intercepted | The 3 Momentum is not refunded. |
| A Silence lands on an enemy | Momentum does not change. |
| A projectile heal of 9 is partly intercepted | The interceptor is healed by 4 and the target by 5. |
| A projectile status is partly intercepted | The interceptor gets the status; the target does not. |
| A Burning tick hits a unit with 50% Defense and a Ward of 8 | The Ward absorbs the 2. |
| A Poison tick hits a unit with 50% Defense and a Ward | It deals 1 (minimum 1); the Ward does not absorb it. |
| Burning with 3 turns left is reapplied by a skill giving 2 turns | It now has 2 turns left. |
| A Blind unit chooses Skip Turn | That turn counts toward Blind's 2 turns. |
| A Bleeding unit uses Move | It takes 2 Bleeding damage after moving. |
| A reaction's hit is aimed at a unit with a defender in front of it | The defender does not intercept: reaction hits go directly to their target. |
| A unit is Poisoned | It takes 1 damage at the start of each of its next 5 turns, 5 in total. |
| A Frozen unit's turn arrives | The turn is skipped and Frozen ends. |
| A Frozen unit is hit by a `fire` skill | Frozen ends early and the unit keeps its turn. |
| Attacker in Front, enemy units only in Rear | Distance is 1 (both effective rows are 1). |
| Attacker in Rear with its own Front and Middle empty, enemy fully occupied | The attacker counts as row 1, so the distance to the enemy Front is 1 and to the enemy Rear is 3. It still counts as Rear for row-dependent skills. |
| The enemy Front contains only a stealthed unit | The row does not count for distance, so the enemy Middle counts as row 1. |
| A Hunter uses Sniper Shot ("any visible unit") on the farthest visible enemy | The shot is legal at any distance. |
| Far Reach (+2 range) is used on a skill whose range already reaches the table maximum | The skill can reach 2 further. |
| Meteor lands and a KO body is adjacent to the occupant | The body is not hit. |
| A Healer in Front heals an ally in Rear with the Middle row empty and no horizontal gap | The same-side distance is 1. |
| Cleave hits a Front unit, the Middle row is empty and the Rear row has a unit at 100% overlap | The Rear unit is not adjacent, so it is not hit. |
| Shove moves a Front unit back while the Middle row is empty | The unit lands in Middle and nothing is bumped, even if the Rear row has a unit behind it. |
| Cleave hits a unit that has a neighbor on the left and a unit in the row behind at 50% overlap | Both are adjacent and are hit. |
| A side has 18 units and its Hunter uses Spike | The skill is unavailable, and the UI states why. |
| A trap is KO'd | It is removed at once and its slot is free. It cannot be revived. |
| A Hunter is KO'd while its traps are on the board | The traps persist. |
| A Speed 6 turn-taking summon appears at time 10 | It first acts at time 15 (11 − 6 = 5 time units later). |
| Burning is applied twice | There is still one Burning instance, with its duration refreshed. |
| A unit with Bleeding (3 skills left) uses a `melee` skill | It takes 2 damage after the skill, and 2 skills remain. |
| A unit with Bleeding uses a `heal` skill | It takes no bleed damage. |
| A Blind unit's attack misses | The miss shifts Momentum 1 against that unit's side. |
| A Silenced unit has only `magical` skills and its creature attack is physical | It can still use the creature attack. |
| The caster of Burning is KO'd | Burning keeps ticking. |
| A KO'd Elementalist had a Meteor on the timeline | The Meteor still lands (it persists). A KO'd Hunter's queued Rapid Shots are removed. |
| Both sides' last original members are KO'd in one simultaneous block | The battle is a draw. |
| A player's last original member is KO'd by a counter after the player's attack KO'd the enemy's last member | The battle is a draw, since victory is checked after reactions resolve. |
| A draw happens in a single-player battle | It counts as a defeat for the player. |
| A draw happens in an AI-versus-AI test | It is recorded as a draw. |
| A battle reaches the AI-versus-AI harness's technical cap | The harness reports it and records a draw. |
| A player surrenders | The battle is a defeat with no other penalty. |
| Bound Resurrect: reviver 30 HP of 40, revived unit max 20 with 25% revival HP | The pool maximum is 60 and it starts at 30 + 5 = 35. Both are KO when it reaches 0. |
| A heal whose Reach does not restrict the target side is used on an enemy | The enemy is healed. |
| A push whose Reach does not restrict the target side is used on an ally | The ally is moved, following section 9.5. |
| A skill whose Reach says "enemy" only | Allies are not legal targets. |
| A Warrior uses Heavy Slam on its own ally | The ally takes the hit and Momentum does not change. |
| Sniper Shot's range-agnostic Reach is used on an ally in the same row | The same-side distance formula is used to check reach. |
| A unit with a "when hit" counter takes bump damage | The counter triggers, since bump damage is a hit. |
| A stealthed unit takes bump damage | It counts as a hit and stealth ends, but Momentum does not change. |
| An attack is intercepted | Momentum does not change. |
| A stealthed Feral is hit by the splash of an area attack | It is hit and its stealth ends. |
| A stealthed unit with a shield is hit and the shield absorbs everything | Stealth still ends. Momentum does not change. |
| A stealthed unit takes a poison tick | Stealth does not end. |
| A stealthed unit stands in front of an ally and an attack is aimed at the ally | The stealthed unit cannot intercept. |
| A side's last non-stealthed unit tries to use a stealth skill on itself | The skill is not legal, so a side never has zero visible units through its own stealth. |
| A queued projectile shot's target becomes stealthed before the shot resolves | The shot hits the unit behind it under the section 8.4 behind rule. |
| A queued `direct` effect's target becomes stealthed but has not moved | It still hits the target. |
| A queued `direct` effect's target moves away and another unit takes its slot | The effect hits the unit now in that slot. |
| A unit is shoved back and a unit sits directly behind it at 100% overlap | That unit is shoved back too. |
| A unit is shoved into a row where it lands at 50% overlap between two units | It is inserted between them. |
| A shoved chain of three units has room to move | All three move and no bump damage is dealt. |
| A shoved chain of three units is blocked by a full row | Nothing moves, and each of the units in the chain takes 1 bump damage. |
| A trap is shoved | It moves like any unit. |
| Rapid Shots lands all three shots | Momentum shifts three times, once per shot. |
| A Feral Frenzy action lands 3 of 4 hits | By default one shift of 1 toward the Feral's side, unless the skill says otherwise. |
| Poison ticks | Momentum is unchanged. |
| The reference human uses Sniper Shot (7–9) on a 0%-defense critter with 20 HP | The critter dies on the third hit and never on the second. |

Expand these examples when numerical and mode-specific rules are decided. They are expected behavior, not claims that tests have passed.

## 21. Definition of done

The specification is ready when every included feature has concrete rules, content, numerical values, UI behavior, and observable completion criteria, with no unresolved contradictions or required decisions. Provisional values must be explicitly adopted as the current implementation values; the agent must not guess them.

**Implementation is done when everything required by the final versions of this game design document and the UI specification is implemented and verified.** This includes all selected modes, complete content, working play flows, saving, presentation, and required platform builds. No required feature may be silently omitted or replaced by an unapproved placeholder.

Online PvP is an explicit gated exception: before owner greenlight, completion requires the specified online-ready interfaces, not a functioning online mode. Once greenlit, its agreed implementation requirements become part of the completion scope.

A prototype is an intermediate validation milestone, not the finished game. Readability testing may lead to documented changes, but finishing only the first playable does not satisfy this definition.
