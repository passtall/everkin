# Everkin — Game Design Document

**Version:** 0.1 — consolidated design baseline

**Updated:** 2026-09-29

**Format:** Digital tactical card game with creature collection and RPG character builds

**Project name:** Everkin is a working title.

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

This document consolidates the available project discussions into a design specification. It does not treat every brainstorm or illustrative number as a final rule.

- **Established:** explicitly selected by the project owner or consistently carried forward in the available discussions.
- **Proposed:** a discussed design direction that has not received a final decision.
- **Deferred:** deliberately reserved for a later scope.
- **Open:** requires a decision, clarification, or recovery of missing source context.
- **Superseded:** retained only to explain a replaced direction.

Unless identified otherwise, the core rules below describe the established baseline. Example skills, percentages, names, and content counts are illustrative unless explicitly stated to be fixed. Source coverage and retrieval limitations are recorded in Appendix B.

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

The original RPG discussion framed this as exploration, recruitment, combat, skill-point progression, and access to harder regions. Creature collection and team development remain relevant. Their delivery in the card game is **open**: no campaign map, encounter-selection structure, shop, pack system, capture procedure, or reward economy has been selected.

There is no established collectible-card business model. The use of cards does not itself imply booster packs, paid randomized acquisition, trading, or duplicate conversion.

### 2.3 Platforms and controls

Current high-end smartphones are an explicitly stated major target. Desktop play was also discussed. UI readability and touch interaction must therefore influence the design from the start.

Exact launch platforms, device requirements, screen orientation, controller support, and browser delivery are open. Mobile support is a design target, not a verified implementation milestone.

## 3. Party composition and creature identity

### 3.1 Party baseline

The established party baseline is **six active team members**, placed freely among three rows. Earlier summaries also describe combat without reserve-member swapping. Moving or swapping battlefield positions is a separate mechanic and is permitted when its rules allow it.

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

Every unit has a **single fixed skill-point budget shared across its trees**. The budget is only just sufficient to fill approximately one complete tree. Access to additional trees provides more choices, not extra points.

The exact point total, tree sizes, point acquisition schedule, and respec rules are open. The frequently discussed **20 points** is an example, not a locked balance value.

### 4.2 Specialization and hybrid builds

A unit may specialize in one accessible tree or distribute points across several. A three-tree unit must still choose what to become good at; it cannot fully develop all three trees through an increased budget.

**Proposed tree structure:**

- Multiple branches within a tree support different builds even for a single-class creature.
- Strong upper-tier abilities require meaningful investment in that tree.
- Low-investment nodes should not make identical shallow multiclass builds universally optimal.
- Species characteristics provide identity independently of tree access.

The earlier proposal of six class ranks with automatic single-class mastery was replaced by the shared skill-point model.

### 4.3 Proposed node types

| Node type | Function |
|---|---|
| Active skill | Adds an action available in combat. |
| Passive | Changes statistics, behavior, or conditional effects. |
| Modifier | Changes an existing ability rather than adding another unrelated button. |
| Keystone | Produces a major, build-defining change. |

For example, a marking ability could develop toward ranged-damage support, timeline delay, or transfer to another target on death. These are design examples, not implemented skills.

### 4.4 Skill availability in combat

Combat uses the team's learned skills. A limit of **four to six equipped active skills per unit** was proposed to keep choices readable, with loadout selection before battle. The cap and the distinction between learned and equipped skills are not yet finalized.

There is no established generic standard-attack action separate from skills. If an ordinary attack is required, its definition belongs in the skill system rather than an additional global damage stat.

### 4.5 Proposed skill-tree roster

The following twenty trees were proposed as a design inventory. They are not a commitment to implement all twenty in the first release. Names are working labels.

| Tree | Mechanical identity | Example design space |
|---|---|---|
| Guardian / Wächter | Interception and physical protection | Protect units behind or beside the user, hold position, restrict movement. |
| Breaker / Brecher | High impact at a tempo cost | Heavy hits, anti-defense tools, a later next action. |
| Duelist / Duellant | Single-target control | Counters, evasive skills, benefits in less crowded fights. |
| Hunter / Jäger | Focused pursuit | Marks, repeated pressure, finishing injured targets. |
| Skirmisher / Plänkler | Mobility | Attack while moving, reposition, exploit exposed targets. |
| Assassin / Assassine | Vulnerable-target elimination | Burst damage against exposed units with limited defensive safety. |
| Berserker | Risk-dependent offense | Gain power at low HP or trade survivability for damage. |
| Commander / Kommandant | Allied coordination | Improve allied actions and advance allies on the timeline. |
| Tactician / Taktiker | Enemy timeline control | Delay turns and exploit timing; interruption only if preparation mechanics are added. |
| Disruptor / Störer | Impairment | Blindness, weakening, silence-like effects, movement disruption. |
| Healer / Heiler | Recovery | Direct healing, regeneration, rescue abilities. |
| Lifeweaver / Lebensweber | HP redistribution | Share life steal, transfer HP, convert overhealing into benefits. |
| Protector / Beschützer | Preventive defense | Shields, damage reduction, protection against statuses. |
| Alchemist / Alchemist | Effect combinations | Poison, acid, restorative mixtures, effects built over time. |
| Elementalist / Elementarist | Elemental utility | Distinct status, movement, and tempo effects rather than recolored damage. |
| Controller / Kontrolleur | Formation disruption | Push, pull, prevent movement, obstruct positions. |
| Trapper / Fallensteller | Prepared space | Traps and effects triggered by movement or actions. |
| Beastmaster / Bestienmeister | Creature cooperation | Support animal allies or use temporary companions. |
| Blood Mage / Blutmagier | HP expenditure | Trade the user's health for powerful actions. |
| Trickster | Effect manipulation | Steal buffs, transfer debuffs, mirror effects, change targeting behavior. |

Earlier illustrative names included Warrior, Arcanist, Illusionist, Saboteur, Cleric, Scout, Rogue, Tinkerer, Druid, and Brawler. They are not additional approved trees. Necromancy and Sporeweaving also appeared as examples rather than finalized tree specifications.

The important test for any tree is whether it changes the decisions a unit makes, rather than simply providing a different percentage bonus. Timeline- and formation-related trees are particularly relevant to Everkin's identity.

## 5. Unit statistics and damage

### 5.1 The six global stats

| Stat | Definition |
|---|---|
| HP | Maximum health; current HP is tracked during combat. |
| Speed | Determines the unit's underlying action frequency. |
| Physical Power | Scaling basis for physical skill damage. |
| Magic Power | Scaling basis for magical skill damage. |
| Physical Defense | Percentage reduction of physical damage. |
| Magic Defense | Percentage reduction of magical damage. |

Defense may be **0%**. Units may be strongly specialized in physical or magical power.

Accuracy, evasion, critical chance, critical damage, mana, universal resistance, and healing power are **not additional global stats**. Individual skills, traits, or trees may introduce a relevant mechanic when needed, but the baseline stat sheet remains limited to the six values above.

### 5.2 Damage scaling

Each damaging skill defines its own damage range as a scaling of the relevant Power stat.

Illustrative definitions:

- Bite: 80–100% of Physical Power.
- Heavy Slam: 150–190% of Physical Power.
- Arc Bolt: 90–120% of Magic Power.

These values demonstrate the selected model; they are not approved balance data. A skill may explicitly scale from HP, defense, or multiple stats, but that exception must be part of its definition.

Physical damage interacts with Physical Defense; magical damage interacts with Magic Defense. The exact order of damage modifiers, random sampling, rounding, defense bounds, penetration, shields, and interception is **open**. No undocumented formula should be treated as settled.

### 5.3 Stat sources and growth

The discussed sources of values are creature identity, build investments, and temporary combat effects. Level growth and the relative contribution of levels versus skill trees remain open.

Healing scaling, damage-over-time calculation, status durations, stacking, cleansing, and critical-hit behavior also require individual rules before implementation.

## 6. Turn timeline and action execution

### 6.1 Conditional turn order

Combat uses a visible turn timeline inspired by the tactical function of FFX's conditional turn order. Units do not simply alternate sides or each act once within a rigid round.

Speed affects action frequency. Skills and effects may accelerate allies, delay enemies, or change when the user acts again. The player should be able to evaluate timing as part of choosing an action.

**Open:** initial initiative, the scheduling formula, tie-breaking, per-skill recovery costs, speed limits, and safeguards against indefinite action denial or repeated-turn loops.

### 6.2 Standard action sequence

1. The timeline identifies the acting unit.
2. The player chooses a skill.
3. The player supplies only the target or other input required by that skill.
4. The action resolves immediately.
5. Automatic effects and the updated combat state are reflected in the timeline.
6. The next eligible unit acts.

The timeline determines **who acts**, not a general queue in which already-selected actions wait to resolve. An animation may take time to play, but it does not open additional tactical decision windows.

Delayed casting or preparation was discussed only as a possible explicit exception. It is not part of the standard execution model.

### 6.3 Minimal decisions

The desired interaction is **skill → target → resolution**, or **skill → resolution** when no target input is necessary.

Parry, counter, interception, thorns, and comparable reactions trigger automatically when their conditions are met. Their strategic control comes from builds, positioning, prior skills, or active effects, not repeated confirmation prompts.

If an action has meaningfully different modes, a distinct skill or directly selectable mode is preferable to a chain of follow-up questions. The maximum reaction depth, trigger ordering, and recursion prevention remain to be defined.

## 7. Battlefield and formation

### 7.1 Rows and slots

Each side has three nominal rows: **Front, Middle, and Back**, with **six positions per row** in the baseline formation. This gives eighteen possible placement positions per side, not eighteen starting party members.

```text
Enemy Back       [1] [2] [3] [4] [5] [6]
Enemy Middle     [1] [2] [3] [4] [5] [6]
Enemy Front      [1] [2] [3] [4] [5] [6]

Allied Front     [1] [2] [3] [4] [5] [6]
Allied Middle    [1] [2] [3] [4] [5] [6]
Allied Back      [1] [2] [3] [4] [5] [6]
```

The diagram expresses relationships, not a finalized screen layout or column orientation.

The player may place all six party members in one row. There is no requirement to fill every row or distribute classes according to conventional roles.

### 7.2 Starting formation

Starting position is a deliberate pre-battle choice. The game must not randomly rearrange the party through surprise formations or arbitrary encounter overrides.

Enemy formations and encounter mechanics may create different tactical demands. Position changes during combat must be an understandable consequence of actions or explicit effects.

### 7.3 Horizontal position

Slots have limited tactical significance beyond capacity. The accepted direction supports adjacency, selected area patterns, and positional protection without turning every attack into detailed geometric simulation.

Same-column and adjacent-column interception were proposed. Exact column relationships, attenuation, and orientation remain open. Row distance and horizontal slot distance should remain separate concepts.

There is no general facing, rotation, or rear-attack subsystem. A backstab or similar behavior must be an explicit skill effect.

### 7.4 Nominal row versus effective distance

Rows retain their identity when empty. Units do not automatically move forward when allies fall.

For range calculations, empty rows are skipped. A unit in Back remains in Back for row-dependent skills, even if it is now the nearest reachable opponent.

Example: if the enemy Front and Middle contain no relevant active visible units, the enemy Back loses the distance protection those rows would otherwise provide. Reviving a visible unit in Front can restore that protection without moving the surviving units.

An exact distance formula, the treatment of allied rows in that formula, and the range of each skill remain open.

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

**Current rule:** every normal unit occupies one slot.

**Deferred:** multi-slot and multi-row creatures. Future support should allow an anchor position and a footprint, such as 2×1 or 2×2. Large-creature concept art does not enable multi-slot gameplay in the initial version.

Unresolved future questions include contiguous space, movement fit, protection across columns, targeting a large body, and whether overlapping an area several times deals damage once or repeatedly.

## 8. Targeting, range, and protection

### 8.1 Target legality

Skills specify valid targets, range, eligible starting rows, and relevant restrictions. Rows generally are not directly selected as targets; a skill can select a unit and derive its affected area from that target.

Explicit position- or row-targeted abilities were discussed as exceptions, particularly for area effects against hidden units. The exact exception list is open. General row targeting must not silently become available for every skill.

Target legality and interception are separate: an enemy may be a legal target while defenders still have a chance to intercept the attack.

### 8.2 Interception

Eligible defenders in front of a target may intercept appropriate attacks. Protection resolves from nearer defensive layers toward the target, with Front considered before Middle where applicable.

- A full interception can stop or take over the incoming attack according to the effect.
- A partial interception leaves a remaining attack or effect.
- That remainder continues toward the original target.
- Further eligible defenders may attempt to intercept it.
- Partial interception does not remove a later defender's defense opportunity or the intended target's applicable defense.

Sequential protection is intentional. Unlimited recursive redirection between defenders is not specified and needs a separate resolution rule.

Proposed interception effects include taking the attack, absorbing part of its damage, reducing its remaining strength, removing one effect, or stopping a projectile component. The attack types eligible for each defense, exact probabilities, processing order, and interaction with the six stats are open.

Ordinary interception should not automatically block every area effect. The area-versus-interception rules require skill-specific definitions.

### 8.3 Target preview

Before committing an action, the interface should communicate:

- Whether the target is legal and in range.
- Which defenders may interfere.
- The relevant success or interception chance.
- Important exceptions such as ignored defenders or stealth interaction.
- The affected targets and movement or timing consequences where available.

An aggregate interception chance was proposed for the main display, with further detail on inspection. Example percentages from the discussions are placeholders.

### 8.4 Area patterns

Discussed patterns include a target and its neighbors, adjacent slots, a row derived from a target, a line, and multiple selected or random units. These define content possibilities rather than a mandatory catalog.

Area effects can affect stealthed units physically inside their area. Whether they affect corpses, objects, allies, or the user must be defined by the skill.

### 8.5 Closest-target ties

Stealth and empty-row rules affect nearest-target selection. Free target choice among equally close legal targets was proposed, unless the skill explicitly selects randomly. The tie rule remains to be finalized without adding unnecessary decisions to automatic actions.

## 9. Movement and positional skills

### 9.1 Repositioning

Voluntary repositioning costs a turn, as carried forward in the lane discussion. Skills may combine an action with movement; movement does not always require a separate turn when it is part of a skill.

Useful design examples include an attack from Middle that advances its user, a powerful action that leaves the user exposed, or a weaker action that improves the user's defensive position.

Exact movement recovery time and the order of movement versus damage are open and must be specified for the relevant action.

### 9.2 Blocked movement

Movement into a full destination or beyond the formation boundary is blocked. Units are not automatically shuffled to make room.

A skill may explicitly swap, push, pull, displace, or apply a consequence when movement fails. Slow, impact damage, or other effects were discussed as possibilities, not universal penalties.

Whether other parts of a combined attack-and-move skill still resolve when movement fails remains a required skill-design decision.

### 9.3 Position-dependent skills

A skill may have multiple valid starting rows and different behavior in each. For example, an attack might have greater force from Front and more reach from Middle. A row can also make a skill unavailable.

This is a means of making formation matter without assigning every row an arbitrary permanent stat bonus. No universal row-bonus table has been finalized.

### 9.4 Engagement

Engagement and movement restriction were discussed as optional mechanics. There is no established universal opportunity-attack or zone-of-control system. Use explicit effects if such behavior is selected.

## 10. Stealth, incapacitation, and resurrection

### 10.1 Stealth

A fully stealthed unit is ignored by normal opposing direct targeting, effective-range calculations, nearest-target selection, and interception eligibility.

A row containing only stealthed units is effectively empty for the opponent's range calculation. Its units still occupy their physical slots.

Stealth does not grant immunity to area effects. An effect covering the unit's position can still hit it. Reveal abilities may explicitly bypass or remove stealth.

The source of stealth, duration, break conditions, partial visibility, and team-specific visibility rules are open.

### 10.2 Knocked-out units

A dead or incapacitated unit remains at its position and **continues to occupy its slot**. Normal movement and displacement cannot move it. Only an explicit skill may move, remove, replace, or otherwise manipulate the body.

This replaces the earlier proposal that a corpse would retain its position but leave its slot free.

Slot blocking is distinct from range and protection. A row containing only knocked-out units contributes no active defenders; bodies do not preserve living interception or effective-distance protection.

### 10.3 Healing and resurrection

Healing and resurrection are intended parts of the system. Keeping the body's slot makes resurrection at its retained position the natural baseline. Reviving an active defender can immediately change range and protection relationships.

Resurrection HP, turn timing, repeated revival limits, body-removal behavior, and persistence of statuses remain open. The distinction between temporary battle KO and permanent death has not been selected.

## 11. Summoning

**Status: proposed subsystem.** The dedicated discussion explored a model but did not settle its global limits or approve every detail.

### 11.1 Proposed summon categories

| Category | Battlefield behavior |
|---|---|
| Full unit | Occupies space, receives turns, uses skills, and can interact with ordinary unit systems. |
| Temporary unit | Behaves as a unit but expires after a duration or condition. |
| Stationary construct | Occupies space and produces passive or triggered effects without ordinary turns. |
| Assist | Appears as part of a skill, performs its effect, and disappears; no persistent battlefield entity is required. |

Examples discussed include skeletons, thorn spirits, wolf spirits, healing totems, ballistae, reactive mushrooms, and a briefly appearing raven swarm. These are concept examples, not committed content.

### 11.2 Proposed operating rules

- Persistent summons need a valid free position and block it normally.
- Unit-like summons reuse normal stats, targeting, healing, statuses, movement, and timeline systems where applicable.
- A summon may be player-controlled, act autonomously, or use automatic triggers.
- Duration and the relationship to the summoner belong in the summoning skill.
- A skill may impose its own limit on simultaneous summons.
- Summoning is distinct from permanent collection; temporary summons were proposed to disappear after combat.
- A separate global summon resource was not recommended, but the cost model is not finalized.

Possible relationships include lasting until death, lasting for a number of turns, disappearing with the summoner, surviving independently, consuming HP, consuming a body or object, replacing the summoner, or upgrading/sacrificing an existing summon.

Summoning before battle was mentioned as an option and has not been approved as an additional phase.

### 11.3 Unresolved capacity conflict

The formation has **six party members and eighteen possible positions per side**. A later summon proposal treated six as the total available battlefield spaces and suggested leaving party vacancies for summons. These are different models.

Before implementing summons, decide whether:

- Six is a cap shared by party members and summons;
- Summons can increase the active body count beyond six while using the eighteen positions; or
- Another explicit summon budget or cap applies.

The document does not select one of these alternatives. A six-member starting party is not automatically unable to summon merely because it is full.

Likewise, multi-slot summons remain deferred with other multi-slot units. The summon brainstorm does not override the current one-slot baseline.

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

Enemy behavior, difficulty progression, encounter length, victory/defeat conditions, draws, retreat, and special objectives are still open. Even the normal battle-ending rule must be specified explicitly when the combat rules are finalized.

## 13. Card interface and combat presentation

### 13.1 Card role

A card is the persistent visual representation of a unit. Its presentation should connect collection, team building, build inspection, and combat without changing the underlying identity.

Core information discussed for a unit card:

- Name and portrait.
- Current and maximum HP.
- Available skill trees or class identifiers.
- Relevant statistics.
- Active status effects.
- Next turn or a clear connection to the timeline.

Information density should adapt to context. Detailed inspection may show the full build while the battlefield emphasizes immediate decisions.

### 13.2 Team builder

The team builder supports inspecting recruits, comparing builds, allocating points, and arranging cards into formation. Drag-and-drop was proposed, with mobile-friendly interaction required. Precise filters, sorting, saved teams, and gesture behavior remain to be designed.

### 13.3 Battle interface

The interface must make the acting unit, its skills, legal targets, current formation, statuses, and upcoming turns easy to identify. The timeline is a distinct, persistent tactical display.

Target inspection exposes range and protection. Skill selection should communicate foreseeable affected areas and changes without requiring extra confirmation chains. The exact screen layout is not fixed.

### 13.4 Animation and effects

Use card motion, highlights, projectiles, impact effects, and readable numbers to communicate actions. Selection can raise or highlight a card; an attack can move it toward the target before returning; a KO can change its appearance while preserving its occupied slot.

Earlier technical examples of cards being drawn or played were generic presentation possibilities, not approval of a draw-and-hand combat system. Visual destruction must not make an occupied KO slot appear vacant.

Effects should be short, clear, and restrained. Players must always understand who acted, who was affected, which units remain active, and where everyone stands.

## 14. Art direction

### 14.1 Visual identity

The selected qualities are **charming, colorful, varied, warm, and slightly fairy-tale**. Art is strongly stylized, expressive, and readable rather than realistic.

The former low-poly discussion contributes simple shapes, strong silhouettes, clear color grouping, and manageable detail. These are now illustration principles. They do not require meshes, rigs, 3D environments, or 3D animation.

### 14.2 Character illustration

- Give each creature a clear silhouette and one or two memorable visual features.
- Keep faces simple but expressive.
- Use slightly stylized humanoid proportions where appropriate.
- Preserve variety among humans, animals, humanoids, floating beings, and unconventional fantasy forms.
- Avoid mandatory class-specific outfits, colors, or silhouettes.
- Use type-defined weapons and props when they contribute to identity.
- Retain the distinction between color variants and separate creatures.

The concept-art requests for humans representing different classes explore variety; they do not reverse the rule that appearance must not rigidly encode class.

### 14.3 Size contrast

The collection should include a range from mice to mountain-like beings. The selected intent is to preserve strong size contrast within readable limits.

In the card format, artwork framing and visual scale cues must communicate that contrast while keeping each card usable. Exact framing standards are open. Visual size does not change the one-slot rule.

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

The available card-art discussion records requests for batches of ten additional animals, ten humanoid fantasy creatures, ten large extravagant fantasy creatures, ten male humans, and ten female humans.

The image outputs were not returned by the chat reader. These requests demonstrate intended breadth, but do not provide a recoverable named creature roster or approved card statistics. Individual designs must be recovered before they can be cataloged accurately.

## 15. World and narrative

**Status: proposed direction.** Story was discussed as support for gameplay rather than its primary driver.

The narrative should explain why the player encounters different beings, why they join a team, and why new combinations matter. A lightweight central conflict with local stories was proposed, with an explorer, wanderer, researcher, mercenary, or guardian-like player role rather than a required chosen-one premise.

Possible layers include a concise main storyline, regional conflicts, and emergent party events or relationships. Reactions to party composition, recruitment stories, rivalries, and personal quests are future possibilities. None is an implementation commitment.

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

### 16.2 Asset workflow

The production direction is creature concept → illustrated card artwork → UI integration → lightweight animation and effects. AI-assisted artwork and code were discussed as production aids.

The former Blender/model/material/rig/skin/animation pipeline is superseded. There is no requirement to build 3D characters, environments, cameras, navigation, or collision for this product.

### 16.3 Cost and delivery planning

Engine costs, storefront fees, external assets, sound, fonts, and possible service costs were discussed as planning topics. This document sets no budget, engine version, commercial license conclusion, or online-service commitment. Licensing and platform support must be checked when delivery decisions are made.

## 17. Playable scope and development priorities

### 17.1 First playable direction

The discussions consistently favored a small combat-focused build that demonstrates team construction and meaningful fights before expanding content.

The first playable should exercise:

- A small selection of humans, animals, and fantasy creatures.
- Six-member formation across three rows.
- Shared skill-point allocation across accessible trees.
- The six global stats and skill-specific damage scaling.
- Visible turn order and a limited set of timing effects.
- Movement, targeting, and a readable form of protection.
- Card-based presentation and mobile-conscious controls.

The implementation sequence is not fixed. Summons, terrain, and multi-slot creatures should not silently enter the first playable simply because they have been explored.

### 17.2 Content counts remain proposals

Different discussions suggested around 10 recruits for an early slice or 15–20 for a combat prototype, and between 6–8, 8–12, or 10–12 initial trees. These are alternative scoping suggestions, not simultaneous targets.

The twenty-tree inventory is a design pool. A possible eventual thirty to forty trees was speculative. No final roster size, enemy count, or launch-content budget is established.

### 17.3 Deferred or unselected systems

Multi-slot creatures, complex terrain, a full narrative campaign, emergent relationships, genetic variation, and expanded summon archetypes remain deferred or open.

Online PvP, cooperative play, rankings, trading, crafting, an equipment economy, monetization, achievements, and save-progression rules have no approved specification in the available discussions.

## 18. Identity and reference boundaries

Everkin remains a working title. A previous naming discussion raised potential existing-name conflicts; final commercial naming and clearance are unresolved. This document records that issue without treating the earlier legal commentary as clearance or a current legal assessment.

All artwork, character designs, UI, names, text, music, and branding must have an independent identity. References guide broad design goals rather than supplying assets or exact presentation.

Previously discussed reference games include FFX for turn-order tactics; Hearthstone for card presentation; Siralim Ultimate and Monster Sanctuary for team-building and build synergies; and LumenTale, World of Final Fantasy, Cassette Beasts, Dragon Quest Monsters, and Monster Hunter Stories as comparison points. Their individual rules are not adopted by default.

Minecraft and Super Mario 64 were earlier references for simple geometry and readable 3D form. They no longer define a 3D asset-production requirement.

Earlier naming candidates were Wildbound, Kinforge, Riftkin, Beastfall, Veyra, Tamerift, Everkin, Roamkin, Feralis, and Bondfall. They are historical alternatives, not cleared commercial names.

## 19. Decisions still required

| Area | Outstanding specification |
|---|---|
| Party | Starting with fewer than six, reserve rules confirmation, enemy limits, duplicate recruits. |
| Timeline | Initial order, scheduling and recovery formulas, ties, delay/acceleration limits. |
| Skills | Equipped-skill cap, costs, cooldowns, exact action economy, preparation exceptions. |
| Damage | Roll distribution, modifier order, rounding, defense bounds, penetration, shield interaction. |
| Defense | Interception probabilities, eligible attacks, column relationships, reaction ordering. |
| Range | Exact effective-distance calculation and per-skill ranges. |
| Movement | Destination selection, combined-action failure behavior, recovery time, swaps. |
| Statuses | Catalog, stacking, durations, expiry timing, cleansing, stealth breaks. |
| KO and revival | Health on revival, timeline re-entry, body removal, permanence of death. |
| Summons | Six-member versus battlefield-capacity limits, placement, control, lifetime, costs. |
| Builds | Point total, tree contents, unlock schedule, respec, progression curves. |
| Recruitment | Acquisition process, human generation, individual persistence, collection progression. |
| Encounters | Victory/defeat, draws, retreat, objectives, enemy behavior, difficulty, length. |
| Product structure | Campaign or challenge format, between-battle flow, rewards, launch content. |
| Presentation | Final card design, mobile board layout, size framing, input scheme, audio. |
| Delivery | Launch platforms, technical validation, save system, business model, final name. |

These are intentional gaps in the current design, not permission to inherit equivalent rules from Hearthstone or another reference game.

## Appendix A. Superseded directions and reconciliations

| Earlier direction | Current treatment |
|---|---|
| Free-roaming low-poly 3D party RPG | Replaced by the card-game product direction; no 3D assets. |
| Cards as a temporary prototype before 3D | Cards are now the intended presentation. |
| Rigging, meshes, materials, 3D camera, navigation, collision | Removed from current production requirements. |
| Six class ranks and automatic single-class mastery | Replaced by a fixed shared skill-point budget. |
| Knocked-out units leave their slots free | Replaced by corpses retaining and blocking their slots. |
| Empty rows preserve full nominal range | Empty rows are skipped for effective range while retaining formation identity. |
| Future large units use horizontal space only | Future footprints may span both columns and rows; current units remain 1×1. |
| Prevent all further interception after one defense | Partial interception permits later eligible defense against the remainder. |
| Encounters randomly rearrange starting formation | Rejected; starting formation is the player's pre-battle decision. |
| Alpha/legendary shapes treated as color variants | Different size or anatomy means a separate creature. |
| Runtime-generated humans as a settled requirement | Open; statically pre-rolled individuals were subsequently considered. |
| Full six-member party automatically leaves no summon space | Unresolved capacity conflict; not adopted as a rule. |

## Appendix B. Discussion sources and coverage

This revision reviewed all eleven earlier Everkin project chats returned by the available chat listing, plus the current documentation conversation and the project instructions. No archived ChatGPT chats were returned. The local synced `sources/` directory contained no reference files.

| Discussion | Design contribution |
|---|---|
| [Engine Empfehlung für Everkin](https://chatgpt.com/c/6ab2f045-65f4-83eb-b048-9dbcfd865dda) | Godot discussion, C# core proposal, mobile target, art rules, and the move toward cards. |
| [Summon Typen Erkunden](https://chatgpt.com/c/6ab40232-9ca8-83ed-85b5-997191fcf989) | Summon categories, behavior, costs, archetypes, and unresolved capacity assumptions. |
| [Klassen vorschlagen](https://chatgpt.com/c/6ab3dc44-2a40-83ed-b8a1-4b7d32393951) | Twenty proposed skill-tree identities. |
| [Kartenspiel Kartenentwürfe](https://chatgpt.com/c/6ab3bf0f-1cd4-83eb-b0e1-9ef0f814ab20) | Creature and human concept-art requests. |
| [Kampfformation Entwerfen](https://chatgpt.com/c/6ab2e78f-9ea8-83eb-b2a9-2f729a5a474c) | Six stats, skill damage scaling, species consistency, human individuality. |
| [Lanes Positioning System](https://chatgpt.com/c/6ab3941b-e650-83ed-9cff-825f7439909d) | Position, range, movement, stealth, corpses, interception, minimal-decision philosophy. |
| [Projektideen sammeln](https://chatgpt.com/c/6ab391c2-c6dc-83ed-98b2-ec69bef52a29) | Core loop, formation summary, small-scope prototype proposals. |
| [Rechtsrisiken Spielkonzept](https://chatgpt.com/c/6ab2e228-aee4-83eb-8467-6f4b8ba5a8eb) | Working title, reference comparisons, original identity, baseline system summary. |
| [Kartenspiel UI Entwurf](https://chatgpt.com/c/6ab2ecb0-55a4-83eb-af53-2a31b4b648c4) | Cards as units, skill-driven combat, team builder, loadout proposal. |
| [Story für Everkin](https://chatgpt.com/c/6ab2eab3-5118-83eb-a1b3-01c7c7d3d1e3) | Lightweight narrative and optional party-driven storytelling. |
| [Klassenunabhängige Kreaturen](https://chatgpt.com/c/6ab2e8dd-1dbc-83eb-b3f3-0d5811b574e4) | Species/class separation, one to three trees, shared point budget. |

**Coverage limitation:** the reader returned at most five recent exchanges for several longer chats and reported no older page. Some conversations consequently begin mid-discussion. The art-rulebook response was also truncated, and generated concept images were not included. The browser required sign-in to attempt fuller retrieval. This is therefore a consolidation of all retrievable material, not a claim that every historical message or image has been recovered.

Earlier rules repeated in later discussions are retained where supported. Missing details are left open rather than reconstructed as facts. Raw chat transcripts, unrelated personal content, installation troubleshooting, and conversational phrasing are excluded from the specification.
