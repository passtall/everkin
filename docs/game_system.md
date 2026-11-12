# Everkin — Game Design Document

**Format:** digital tactical card game with creature collection.

**Companion documents:** [class_system.md](class_system.md) owns the classes, keystones and unique attack rules; [skills.md](skills.md) every skill and keystone; [creature-classes.md](creature-classes.md) the creature roster (classes, keystones, attack, biome, stats, Cost, art); [ui_spec.md](ui_spec.md) screens and interactions; [campaign.md](campaign.md) the campaign content; [production.md](production.md) platforms, technology, saving, online and the roadmap. This document owns the game rules and scope.

## 1. Design vision

> Build your perfect fighting party from everything the world has to offer.

Everkin is about collecting and combining humans, animals and fantasy beings into a combat team. Every recruit belongs to the same party system; humans fight alongside the other creatures rather than training them.

The player expresses strategy through team selection, formation and individual combat actions. A visible, manipulable turn timeline and meaningful positioning give combat its identity.

Everkin is a **card game**: characters appear as illustrated cards, and no 3D assets are made. Cards represent persistent team members who use their own skills. There is no shuffled deck, hand, card draw or per-round mana.

### 1.1 Design pillars

| Pillar | Design requirement |
|---|---|
| Team freedom | Humans, ordinary animals and fantastic creatures are all valid party members. |
| Variety | Every creature has two classes, two keystones and its own attack, so even creatures of the same classes play differently. |
| Tactical clarity | Players can understand legal targets, protection, movement and turn-order consequences. |
| Meaningful timing | Speed and skills decide when units act; there is no fixed once-per-round sequence. |
| Meaningful formation | Position decides who can use melee, who covers whom, area effects and movement. |
| Few required decisions | A turn asks only for the choices needed to execute the action. Reactions resolve automatically. |
| Achievable production | Reusable systems, readable artwork and a focused playable core take priority over content volume. |

### 1.2 Document rules

Rules here are decided unless section 19 lists them as open. An agent must not invent missing rules: if a case is not covered, it asks the owner. Examples illustrate behavior without fixing balance values unless they say so. All numbers are starting values that AI-versus-AI testing tunes.

## 2. Player experience and game loop

### 2.1 Core combat loop

1. Review the collection.
2. Pick a team of up to six creatures within the team budget (section 5.1).
3. Set the starting formation.
4. Fight with skills, positioning and the turn timeline.
5. Evaluate the result and revise the team or formation.

There are no skill points, skill trees, builds or respecs: a creature's skills and keystones are fixed (section 4.1).

### 2.2 Collection and progression

**Campaign.** The story campaign is a linear sequence of **four chapters** of fixed-order battles, each of about five battles plus a boss battle, shown on illustrated maps (section 15; content in campaign.md). Each won encounter gives **one fixed creature** from the defeated enemy team, shown on the map before the battle. A won encounter can be replayed, but its reward is given only once. Losing has no penalty: the player can retry at once or change the team first. Encounters never restrict the player's team beyond the team budget. The campaign cannot be restarted; finished encounters can be replayed.

**Collection.** A player owns each creature at most once, and a player's team uses each creature at most once. Enemy teams may repeat creatures. There are no creature levels.

**Tutorial.** A new player starts with the campaign tutorial: three short battles. The player starts with **Daggerwhisk** and **Slingwhisker**, gains **Tailwind Kite** after the first win, **Ripplequill** after the second, and **Hornpip** and **Potionpaw** after the third. During the tutorial the game keeps one automatic team with every owned creature in a default formation; Edit team in the campaign panel works on it. All other modes unlock once the tutorial is complete.

**After the story.** Fixed, harder **challenge encounters** appear on the map; each unlocks one more creature. At least one **random-battle mode** gives continued play: the player picks a difficulty and the enemy team is drawn at random. Random battles unlock no creatures.

**Coins.** Coins unlock creatures that are not campaign rewards. A random-battle win pays **10 coins** at every difficulty and an online win **15**; losses, surrenders and challenge encounters pay none. Coins are earned, spent and bought only **while connected** to the online server, which keeps the balance (production.md §1.4). A coin creature costs **50 + 25 × its Cost** coins and is bought from its card in the collection; the Shop sells only coin packs (ui_spec.md, production.md §1.2).

**Roster split (guide).** About 6 tutorial creatures, about 24 campaign rewards, one challenge creature per chapter, and the rest bought with coins.

There are no booster packs, randomized paid acquisition, trading or duplicate conversion.

### 2.3 Platforms and controls

Windows, Android and iOS, **landscape only**. Desktop and touch readability shape the design from the start. Release order, stores, devices and controllers are in production.md §1.

### 2.4 Modes

- **AI versus AI:** the first development and testing mode (section 2.5).
- **Campaign and random battles versus AI** (section 2.2).
- **Free battle versus AI:** after the tutorial. The player picks a team from the collection, an enemy team (random or chosen from unlocked creatures) and the AI difficulty. No rewards.
- **Local two-player:** both players build their teams from the collection on the device and may use the same creatures. The board turns so the acting player's side is always their own (ui_spec.md).
- **AI strength.** Each campaign and random-battle encounter sets its own AI level; only free battles let the player choose it.
- **Battle viewer:** development builds only.
- **Online PvP:** in the first release, built after post-story content (production.md §3). Players use only their own collection. Opponents come from **quick match** (hidden rating; the allowed gap widens over time until anyone can be matched; cancel at any time; no bots) or a **private match** with a short code that expires when the match starts. Each player picks a saved team before matching and sees the opponent's team when the battle starts. Each turn has a **60-second timer**; when it runs out, the unit uses Skip Turn. A player who disconnects has **1 minute** to reconnect (their timer keeps running), then loses. Players can send fixed emotes (Hello, Well played, Thanks), which the other side can mute; there is no text chat. Players are shown by their platform display name. After a battle both can choose **Rematch**; it starts only if both accept. A player who quits **3 times in a day** (surrender or disconnect loss) cannot use quick match for **10 minutes**. Online needs the full game, not the demo. Backend: production.md §2.4.

### 2.5 AI-versus-AI test harness

The harness plays battles automatically for balance testing and debugging. Battles are deterministic given a seed, and the rules core runs them without any presentation.

- **Teams.** Each side gets 6 random creatures from the roster whose total Cost fits the team budget (section 5.1), with no duplicates within a side. The same creature may appear on both sides.
- **Formation.** Each side's starting formation is a random legal placement, recorded with the battle, so the statistics show which rows work for which creatures.
- **Search.** Each AI picks its action with a minimax search over a fixed number of upcoming timeline turns, counted across both sides. Both AIs in a test battle use the same settings. Search depth comes from the difficulty: **Easy 2, Normal 4, Hard 6**; test runs use Hard. Weaker picks: Easy picks at random among its top 3 actions 35% of the time, Normal among its top 2 10% of the time, Hard always picks its best.
- **Chance in the search.** The search uses average values for damage rolls and probabilities; the real seeded roll is applied once the action is chosen.
- **Scoring.** A position is scored by each side's remaining HP as a share of its maximum, where every living unit counts as if it had 25% more HP. Each point of Momentum is worth 2% of a team's total HP. Harmful statuses on enemies and helpful ones on allies count a little, more the longer they last. Summons and constructs count like units at half value. The weights live in a settings file per AI level.
- **Information.** The AI knows only what a player would see. Because stealth limits targeting, not information (section 10.1), that includes stealthed units, their HP and statuses. Opposing future skill choices are not known.
- **Length cap.** A battle stops after 500 unit turns in total and is recorded as a draw.
- **Run size.** One test run plays 10,000 battles with seeds 1 to 10,000. Each run gets an ID and stores a snapshot of the rules version, content and AI settings, so any battle can be reproduced from its run ID and seed.
- **Saved per battle.** Run ID, seed, teams, formations, result and length, plus per-unit damage dealt and taken, healing, KOs and skill uses. No event log.
- **Report.** Win rate and appearance count per creature, class, keystone and skill; each creature's win rate by starting row; skill usage counts; battle length and draw rate; pair statistics for creatures that do well together and against each other.
- **Imbalance flags.** The report flags the top and bottom 10% by win rate. Balancing starts with the most extreme.
- **Balance changes.** Claude proposes changes from the report, the owner approves them, and the next run checks their effect.
- **Endless chains.** The harness detects and reports reaction chains that do not end (section 6.6).
- **Watching battles.** Headless batch runs come first. The battle viewer can start a new AI-versus-AI battle with the same teams and formations as any recorded battle; it need not replay it exactly.

## 3. Party and creatures

### 3.1 Party

A battle is **six versus six**, placed freely among three rows. A player may fight with one to six creatures; teams with fewer than six show a warning icon where teams are picked. There is no reserve swapping during combat. No class dictates a row, but melee needs the Front row (section 8.1).

### 3.2 What a creature is

A creature has a species (its art and identity), stats (section 5.1), **two classes**, **two keystones** and **one unique attack** (class_system.md). Creatures of the same species have the same stats; there are no hidden individual rolls.

### 3.3 Humans

Human recruits are **fixed individuals** with names and their own class mix, made during content creation and stored as content; they are never generated at runtime. One per chapter is a campaign reward: chapter 1 **Tom Reed**, chapter 2 **Kofi Mensah**, chapter 3 **Edwin Marsh**, and **Ragnhild Eriksen** as the reward of chapter 4's challenge encounter.

### 3.4 Variants

A **variant differs only in colour**. A larger body, changed anatomy, altered silhouette or distinctive features make a **separate creature**. There is no equipment system: weapons and props are part of a creature's art and identity.

### 3.7 Creature roster

The roster is [creature-classes.md](creature-classes.md).

- **HP** 20 to 80 in steps of 5; **Speed** 1 to 10 (small creatures are usually fast, big ones slow); **Power** 1 to 10, the reference human at 5; **Cost** 1 to 10 (section 5.1).
- **No size tiers** and **no natural traits or immunities**. A creature's identity comes from its stats, classes, keystones and attack.
- **IDs** are the unit names (`weasel-rogue-dagger`), stable once released. The game loads one image per creature by ID from the cleaned copies; the front shows a portrait crop, the back the whole creature. The original art is never changed (production.md §1.6).

## 4. Classes and skills

### 4.1 Skills and keystones

Every creature has its unique attack, the three actives of each of its two classes, Move and Skip Turn, and one keystone from each class (class_system.md). All of them are always available; there are no skill points, trees, loadouts, builds or respecs. Keystones are passives and never take an action.

### 4.4 Actions in combat

On its turn a unit uses one of: its **unique attack**, one of its **six class actives**, **Move** (section 9.1) or **Skip Turn**. Skip Turn costs the same as a Delay 5 skill and is always allowed. There is no Defend command and no separate basic attack system: the unique attack is an ordinary skill using the same timing, targeting and resolution as any other.

### 4.6 Skill costs

**No mana, no cooldowns.** Skills are balanced through Delay (section 6.1), Momentum cost (section 6.7) and other consequences: HP loss, sacrifice, position, self-inflicted statuses and harm to allies. A skill may require a position, statuses, HP thresholds, allies, free space, KO bodies or earlier actions; the UI always shows the concrete reason when a skill is unavailable. There is no global anti-spam rule.

Rules that do not need global consistency belong to the individual skill: effect order, costs, targeting exceptions, and behavior when the user or target is KO.

### 4.7 Skill rules and tags

**Skill card.** Every skill has a unique name, its class (or its creature, for an attack), Delay (1–10, **1 is quickest**), Momentum cost (0–10, most 0), delivery, target, effect, and hidden coefficients from which the displayed numbers come (section 5.2). Keystones have a name, class, trigger and effect.

**Tags are derived.** skills.md and creature-classes.md write no tags; where a rule names one, it is read like this:

| Tag | Read from | Used by |
|---|---|---|
| `melee`, `projectile`, `direct` | The Delivery column. | Interception (section 8.2), the Front-row rule (section 8.1), airborne reach (section 10.6), Bleeding, Static Field. |
| `magical` | The actives of the magical classes, the Bard's songs, and attacks with the bolt or pulse style (class_system §2 and §4). Everything else is `physical`. | Silenced. |
| `move` | The Move action and every skill that moves its own user. | Bleeding. |
| `row` | A skill whose text says "one row" or "pick a row": it selects a row. | Targeting. |

There are no element tags. Ground effects react only to the effects named in section 12.2.

**Deliveries.**

- **`melee`** needs the user in its own Front row (section 8.1) and cannot reach an airborne unit (section 10.6). It can be intercepted.
- **`projectile`** travels from the user to the target and passes the units in front of it, so it can be intercepted.
- **`direct`** hits a **specific slot** (the coordinate of the selected target on the half-card grid, section 7.3) with nothing passing between. A delayed `direct` effect hits whoever stands there when it resolves. It is never intercepted.

**Attack.** An **attack** is any skill or summon action that deals damage to a unit, including turret and golem hits. A skill that only applies a status or moves units is not an attack. Rules that say "attack" (Blind, Phase Weapon, misses in section 6.7) use this meaning.

**Targeting.** A skill selects one unit unless its text selects a row. Skills never silently gain other target types.

**Design rules.**

- **Every skill has its own role.** Two skills of one class do not do the same job.
- **Realistic positioning.** A projectile passes the units in front of its target; melee strikes from the Front row and has to get past the enemy's front units.
- **Real effects.** A skill or keystone changes targeting, timing, movement, triggers, statuses or interactions; none only adds a flat number.
- **Low whole numbers.** Players see whole numbers only.
- **Only `melee` and `projectile` can be intercepted**, and such a skill ignores interception only if it says so.

## 5. Unit statistics and damage

### 5.1 The four stats

| Stat | Definition |
|---|---|
| HP | Maximum health; current HP is tracked during combat. |
| Speed | 1–10, **10 is fastest**. Sets how quickly the unit's turns come round (section 6.1). |
| Power | 1–10. Scales all skill damage, healing and Wards, `physical` and `magical` alike. |
| Cost | 1–10. Team-building points; not used in combat. |

There is no Defense, accuracy, evasion, critical hit, mana, resistance or healing-power stat. A unit's toughness is its HP; protection comes from explicit effects such as Wards.

**Team budget.** A team holds **up to six creatures** whose total Cost must not exceed **35**, in every mode including the campaign. Campaign enemy teams are not bound by it. Randomly drawn teams (random battles, free battles with a random enemy, AI-versus-AI tests) are drawn within it.

**How Cost is set.** Each creature gets a power score from its offense and durability; the roster is ranked by score and split into Cost 1–10 in the shares 3, 6, 10, 14, 17, 17, 14, 10, 6 and 3 percent, equal scores getting equal Cost. The current Costs come from a formula built on the old classes; the formula for the current classes is decided when AI-versus-AI testing starts, and Cost is recalculated then (section 19).

### 5.2 Damage and healing

**Scaling.** skills.md and creature-classes.md write every number for **Power 5**. Every damage, healing and Ward number of a skill is multiplied by its user's **Power ÷ 5**, rounded down, minimum 1; a range scales at both ends (4–5 at Power 3 is 2–3). Internally each number is a hidden coefficient (hit for 5–6 at Power 5 is 100–120%). A skill may scale from something else only if its text says so. Summons and constructs scale with their summoner's Power; their HP and Speed are fixed. Rule numbers never scale: bump damage, Bleeding, Poisoned, ground-effect damage and the poison explosion.

**Rolls.** Damage and healing are rolled **uniformly** as a whole number within the range; the displayed range already includes the user's modifiers and is rounded down. There are no critical hits. When a skill or attack hits several units, its damage and every chance are rolled separately for each unit, unless the skill says otherwise.

**Damage order.**

1. **Roll.**
2. **Ward.** A Ward absorbs damage first; only the remainder continues.
3. **Reductions** stated by a skill apply to the remainder (rounded down, minimum 1 if anything passed the Ward).
4. **HP.** The rest is subtracted from HP.

Fixed-damage effects go through Wards and reductions like any hit. The only damage that nothing reduces is bump damage (section 9.5) and damage a skill calls unreducible.

**Wards.** A unit has at most one Ward. A new Ward replaces the old one only if it absorbs at least as much as the old one has left.

**Healing** is capped at maximum HP; there is no overheal unless a skill says what happens to the excess.

**Target previews** show the resulting damage range against the selected target wherever it can be determined.

### 5.3 Hit reliability

An attack hits its target by default, subject to interception, Wards and explicit effects such as Blind. There is no global accuracy roll. A status-applying effect states its own chance; damage does not guarantee a status unless the skill says so.

### 5.5 HP and damage anchors

A critter (such as a fox) has **20** HP, a human **35–45**, a giant stone turtle **80**. A 20 HP critter falls to **4–5 unique attacks** from a Power 5 unit (for example 4–5 damage). Keep all numbers low.

## 6. Turn timeline and action execution

### 6.1 Shared timeline

All units of both sides share **one timeline**. There are no team turns or rounds: a unit acts whenever its turn comes up. How often depends on its **Speed** and the **Delay** of the skill it used last; a fast unit using quick skills may act many times before a slow one acts again, with no artificial limit.

| Scale | Belongs to | Meaning |
|---|---|---|
| **Speed** | Unit stat | 1–10, **10 is fastest**. |
| **Delay** | Skill | 1–10, **1 is quickest**. |

```text
Wait (time units) = Delay + (11 − Speed)
```

| Speed | Delay | Wait |
|---|---|---:|
| 10 | 1 | 2 |
| 10 | 5 | 6 |
| 5 | 5 | 11 |
| 1 | 1 | 11 |
| 1 | 10 | 20 |

Internally the timeline tracks progress in time units. Time advances only in jumps between events; choosing an action and playing animations cost no time.

- **First turn.** At battle start each unit's first Wait is `11 − Speed`.
- **When the next Wait starts.** The acting unit's next Wait is set when its skill is used, before the skill's effects. Pushes, pulls or Speed changes that reach the user during its own action apply on top.
- **Ties.** When turns or other timeline entries fall into the same time unit, the one that joined that time unit **first stays first**. An entry moved by Pull Forward, Push Back or a Speed change joins its new time unit behind the entries already there. Entries created for the same time unit at the same moment (every unit's first turn, several effects from one event) are ordered by the seeded roll, each with equal chance.
- **Speed changes** (for example Song of Haste's +1) change the remaining Wait by the same number of time units; completed progress never changes. When they end, the remaining Wait changes back. Speed stays within 1–10 for the formula, and a Wait is at least 1.
- **Pull Forward / Push Back** remove or add time units to the remaining Wait. There is no global cap; each skill states its number. If Pull Forward removes more than remains, the turn arrives now and the extra is lost.
- **Delay changes.** Effects that add or remove Delay never take a skill outside 1–10: after all modifiers, Delay is clamped to 1–10.
- **Skipped turn.** A skipped turn, chosen or forced, costs like a Delay 5 skill: Wait = 5 + (11 − Speed).
- **Durations** of statuses and ground effects are stated in time units from the moment they start. 10 time units is about one turn of an average unit; 20 is the reference length. An effect's end is shown as a marker on the timeline.
- **Action denial.** There is no safeguard against Push Back and turn-skipping statuses for now.

The UI shows the upcoming turns and previews where the acting unit's next turn moves when a skill is selected; other foreseeable timing changes are previewed where practical.

### 6.2 Action sequence

1. The timeline names the acting unit.
2. The player chooses a skill.
3. The player gives only the target or other input the skill needs.
4. The action resolves at once.
5. Automatic effects and the new combat state show on the timeline.
6. The next unit acts.

The timeline shows **turns, not intended actions**; enemy skill choices are not revealed.

**Delayed events.** A skill may create a delayed effect that appears as its own timeline entry. The skill states whether it follows a unit or stays on a slot or row, whether it can be moved, and what movement and KO do to it.

**Auto events.** A skill can put an auto event on the timeline, occupying a position like a unit's turn; later effects can move it. When its turn comes it executes automatically. If its owner is KO, it is removed. If its target is KO when it executes, it targets the unit behind that target (section 8.4). A skill can make its auto events **blocking**: the owner takes no turn until they have all resolved, and its own turn cost starts after the last one. A Speed change on the owner changes only the remaining wait of pending auto events.

### 6.3 Minimal decisions

The interaction is **skill → target → resolution**, or **skill → resolution** when no target is needed. Interception, counters and other reactions trigger automatically. **Skills never have modes**: different variants are different skills. **There are never decision popups during the enemy's turn.**

### 6.5 Event order and KO checks

**Order for using a skill:**

1. Check that the skill is usable and the target legal.
2. Pay the Momentum cost (section 6.7).
3. Set the user's next Wait (section 6.1).
4. For each event in the skill's order (or each simultaneous block): apply it, check KO, apply its Momentum shift, resolve its reactions (section 6.6), then check victory (section 12.3).
5. The skill ends.

Each skill states the order of its effects. **Whenever the rules leave an order undefined** (which unit, event or roll comes first), the order is random, each possibility equally likely, from the battle's seeded roll; defined orders such as the timeline tie rule apply first.

A skill may declare a **simultaneous block**: apply the whole block, check KO, then collect and process the reactions. Each hit of a multi-hit skill is its own event with its own KO check. A skill states whether it continues when its user is KO mid-resolution.

**Targets that change mid-skill.** What later hits do when their target moved, became illegal or was KO after an earlier hit:

- **A unit, delivered `melee`:** the remaining hits are cancelled if the hit is no longer possible (the user left its Front row or the target became airborne).
- **A unit, delivered `projectile`:** the remaining shots fly to the target's **original column**. If the target is still in that column it can be hit, with interception as usual; if it left the column, the shots miss.
- **A slot (`direct`):** the remaining hits land on the same coordinate and hit whoever covers it, both units at a 50% overlap.
- **Random picks and chains:** each pick or jump is made when it happens, from the targets legal then; a chain with no legal next target ends.
- **Areas** (rows, adjacency, melee reach): the affected units are found again for each hit.

### 6.6 Reactions

Reactions are effects, not turns; they change the timeline only when they say so.

- **Propagation.** Reactions can trigger further reactions, unless a reaction says it cannot. There is no chain limit; an endless chain is a design bug that the AI-versus-AI harness must report.
- **Reaction hits** go straight to their target: they ignore interception, while Wards and reductions apply. A reaction has its own delivery and does not inherit the triggering skill's; one that counts as melee says so.
- **Timing.** Reactions resolve after each event or simultaneous block, before the triggering skill's next event.
- **KO'd owners** do not react, except through explicit "when KO" triggers.
- **Once per hit.** A multi-hit skill can trigger a once-per-hit reaction once per hit.
- **Priority.** Simultaneous reactions resolve in three levels: **first** effects that change or remove other effects (cleanse, reveal), **then** damage, healing and statuses, **last** timeline changes. Within a level, by their owners' timeline order; owners at the same position use the tie rule of section 6.1.

### 6.7 Momentum

**Momentum is one battle-wide meter shared by both sides**, from **−10 to +10**, showing which side has the flow of battle. Every battle starts with every unit at full HP and the meter at **0**; nothing carries over between battles.

**Shifts.**

- A damaging hit that lands moves it **1** toward the acting side. A KO gives no bonus.
- A missed attack moves it 1 against the attacker.
- A hit that is intercepted, fully absorbed by a Ward, or lands on the acting side's own unit moves it **0**. A partly absorbed hit still moves it 1.
- Skills that deal no damage, healing, Move and Skip Turn move it 0, even when they land something harmful.
- Status ticks and ground damage move it 0. Bump damage moves it 0.
- **One shift per action:** a multi-hit or area action moves it 1 toward its side if any hit lands, otherwise 1 against if it missed, and 0 if everything was intercepted, unless the skill says otherwise.
- Reactions and counters follow the same rules. Each **auto event** shifts the meter when it resolves, like a separate action (each turret shot shifts it).
- Several shifts in one simultaneous block apply one by one in event order, clamped after each.
- The meter clamps at −10 and +10; excess is lost. There is no snowball safeguard for now.
- A skill may modify the shift if it says so.

**Costs.** A skill may cost **0 to 10** Momentum. It needs the user's side to be at least that far ahead and, when used, moves the meter that many points toward neutral. The cost is paid before any effect and never refunded, even if the skill achieves nothing. If the requirement is not met, the UI says so.

Fast units build Momentum with many quick successful actions; strong skills spend it.

## 7. Battlefield and formation

### 7.1 Rows and capacity

Each side has three rows: **Front, Middle and Rear**; the two Front rows face each other across the battle line. There are no visible card slots: the units of a row are centered and placed side by side without gaps. All normal units have the same width.

A row holds at most **six units**, so a side holds at most **18**. A six-creature team never fills all rows; the extra room is for summons and constructs. The player may place all six creatures in one row.

### 7.2 Starting formation

The starting formation is the player's choice before battle. The game never rearranges a team by surprise; position changes in combat come only from actions and stated effects.

### 7.3 Horizontal alignment

Because cards are equal-width, gapless and centered, units in different rows overlap by **100%, 50% or 0%**. Rows with the same odd-or-even count align directly; otherwise they are offset by half a card. Internally positions use a half-card grid:

```text
Card center = 2 × i − (n − 1)     (row of n cards, zero-based index i)
Card width = 2
Overlap fraction = max(0, 1 − abs(centerA − centerB) / 2)
```

A three-card row has centers −2, 0, +2; a two-card row −1, +1.

The order of units within a row matters for adjacency, areas and positional skills. A "slot" or "spot" means a coordinate on this grid, not a visible cell. **An effect tied to a spot** (ground effects, delayed `direct` effects) stays on its coordinate when rows recenter; when it triggers it affects the unit covering that coordinate, and **both** units if two overlap it by 50% each.

There is no facing or rear-attack system.

### 7.4 Rows

Rows keep their identity when they empty: units never move forward on their own when allies fall. **"Front row" always means a side's real Front row.** If it is empty, no unit of that side can use melee until a unit moves into it, and enemy skills with melee reach against that side hit nobody (section 8.1).

Skills have **no range**: rows matter for melee, interception, area shapes and movement only.

### 7.5 Kinds of presence

| Property | Meaning |
|---|---|
| Physical occupancy | Whether another unit can take the position. KO bodies occupy it. |
| Visibility | Whether the opponent can select the unit. Stealthed units are not visible. |
| Area presence | Whether a unit is inside an area, regardless of stealth. |
| Interception eligibility | Living, visible, not Stunned, Frozen or Immobilized (section 8.2). |

These never collapse into one occupied/unoccupied rule: a KO body blocks its slot but never intercepts.

### 7.6 Unit size and bosses

Every unit uses one card width and one unit of row capacity. **Bosses are normal-size units**: a boss is a strengthened version of a roster creature from its chapter, with higher HP or other stats (which may exceed the normal ranges) set as explicit content values. Large multi-slot units are not planned.

## 8. Targeting and protection

### 8.1 Target legality

- **No range.** A skill can target any legal unit on either side; using it on the other side is the player's choice, even when it is usually a weak one. **Own side only** skills are the exception: Leap to Defend, Rescue, Swap Turns, Advance and Switch Places. Curse of Betrayal works on any unit, and the healing it takes always goes to the Hexer's side.
- **Melee needs the Front row.** A unit can use a `melee` skill or attack only while it stands in its own Front row. Projectile and direct skills work from any row. Dive and Puppet Soul's forced hit are the only exceptions.
- **Melee reach.** A skill or attack that works "in melee reach" (pulses, breaths, Frost Nova, Sweeping Kick, Battle Roar, Flash Powder, Ward Burst) needs its user in its own Front row, whatever its delivery, and hits **every enemy in the enemy's Front row** (a breath picks one of them as its target). If that row is empty the skill can still be used but hits nobody, and its turn and cost are spent.
- **Airborne.** Melee cannot target an airborne unit (section 10.6).
- **Stealth.** A stealthed enemy cannot be targeted (section 10.1).
- **No empty spots.** No skill targets an empty spot: a ground effect goes on a target unit's spot or on a row. Placing a summon, construct, turret, trap, totem or barricade on a free spot is placement, not targeting (section 11).
- **Rows.** A skill that selects a row may pick any row of either side, even one that looks empty; if nothing is there, its turn and cost are still spent. This way a row with only stealthed units is never given away.
- **Picks.** A skill asks the player (or the AI) for at most one unit and one target. Any further unit a skill needs ("one ally next to the target": Twin Wards, Twin Enchant, Lift) is picked by the seeded roll, each with equal chance.
- **Areas.** An area skill hits every unit in its area; areas built from a target affect that target's side. Skills that pick targets themselves (random picks, chain jumps, "every enemy") state their side.
- **Keystones** that react to an enemy or an ally are triggers, not targets.

Target legality and interception are separate: an enemy may be a legal target while defenders still have a chance to intercept.

### 8.2 Interception

Only `melee` and `projectile` skills can be intercepted, whichever side uses them and whatever they do (a projectile heal can be intercepted). A skill aimed at the user's own side is never intercepted. Reaction hits are never intercepted (section 6.6).

**Who covers whom.** A living unit in a row in front of the target (on the target's side) covers it if their horizontal overlap is 50% or more. Defenders are tried from the row nearest the attacker toward the target; within one row their order is random, each with equal chance.

| Overlap | Melee | Projectile |
|---|---|---|
| 100% | 75% | 50% |
| 50% | 50% | 25% |
| None | no cover | no cover |

**What an interceptor takes.** It takes the **full hit (50%)** or a **partial hit (50%)**. A partial hit splits the damage: the interceptor takes half, rounded down, and the rest continues toward the target, where further defenders may still try and the target's Ward applies. A hit of 1 cannot be split and goes entirely to the interceptor. Effects that are not damage work the same way: an amount that can be split (healing) is split like damage; an effect that cannot be split (a status, a push, a Delay) goes entirely to the interceptor.

**Who can intercept.** Not a unit that is Stunned, Frozen, Immobilized, stealthed or KO. Airborne units intercept and are covered like any other unit, also against melee. Summons and constructs intercept like any unit. Other defenses come only from skills, each stating what it works against.

An intercepted hit moves Momentum 0 (section 6.7). Sequential protection obeys the reaction rules of section 6.6.

### 8.3 Target preview

Before committing, the interface shows:

- which units are legal targets;
- for each legal target, the chance that the skill reaches it directly (after interception), and which defenders may interfere;
- important exceptions (cannot be intercepted, stealth, airborne);
- the units an area affects, and movement or timing consequences where they can be foreseen;
- the resulting damage or healing range.

### 8.4 Area patterns

Areas use **fixed patterns based on rows, adjacency and melee reach**; there is no free radius or cone. The UI highlights every foreseeable affected unit before execution, including friendly fire, and separates possible from certain targets for random outcomes.

Areas hit stealthed units inside them. Areas never hit KO bodies unless the skill says so. Only skills that say so target KO bodies (revives, Raise Undead, Corpse Explosion, Desecrate).

**Adjacency.** The units adjacent to a unit are its **left and right neighbours in the same row**, plus the units in the row **directly in front of and directly behind it with at least 50% overlap**, on the same side. Adjacency uses the real rows: an empty row is not skipped, so units in Front and Rear are not adjacent when Middle is empty.

**Behind.** A skill that hits units **behind** its target hits them in the **next row behind the target's row that holds a unit that can be hit**: an empty row is passed over, a row with only KO bodies counts as empty, and stealthed units count. In that one row, units with **100% overlap** are hit automatically and those with **50% overlap** with a **50%** chance; nothing further back is reached. If nothing there overlaps the target, nothing is hit behind. If a skill's target is KO when it executes, the skill hits the unit behind that target instead.

### 8.5 Nearest

"The nearest enemy" (turrets, golems, blades, undead, Spreading Curse) is the legal enemy in the fewest rows from that side's Front: Front before Middle before Rear, empty rows ignored. Stealthed units are not legal. Ties are picked by the seeded roll, each with equal chance, for players, the AI and automatic effects alike, so "nearest" never asks for a choice.

## 9. Movement

### 9.1 Move

Every unit has a basic **Move** action:

- It moves the unit to **any other row on its own side**; it cannot reorder a unit within its row.
- It has **Delay 5** and counts as `move` (Bleeding).
- The player picks any gap in the destination row; the row recenters. A full row cannot be chosen, so Move never bumps.
- Move never swaps; swaps come only from skills.

**Movement inside a skill.** When a skill moves its own user (Leap to Defend, Rescue), the movement follows the forced-movement rules of section 9.5. If it is blocked, the rest of the skill still resolves from wherever the user ends up.

### 9.2 Blocked movement

Movement into a full row or past the formation edge is blocked. Recentering never creates room or shoves units into other rows.

### 9.5 Forced movement

Pushes, pulls and swaps of a unit other than the user follow these rules. Each skill says whether it can move enemies, allies or both.

- **Landing.** A moved unit keeps its horizontal coordinate and lands in the destination row where that falls. If it lands with **100% overlap** on a unit there, that unit is **bumped** on in the same direction, and so on down the chain. If it lands with **50% overlap** between two units, it is inserted between them. If nothing is within reach, it joins the end of the row on that side. Pulls chain the same way toward the battle line.
- **All or nothing.** If every unit in the chain has room, they all move and no damage is dealt. If some unit has nowhere to go (a full row, the formation edge), **nothing moves** and every unit in the chain, the blocker included, takes **1 bump damage**. A 50% insertion into a full row is blocked the same way: the moved unit and the two units around it take 1 each.
- **Bump damage** is fixed at 1, never scaled or reduced. It is a hit (it breaks stealth and triggers "when hit" reactions) and can KO, but it moves Momentum 0.
- **Immunity** comes only from skills and statuses (Hold Position, Immobilized, Last Stand, Steadfast Ward).
- **KO bodies** cannot be moved or swapped unless a skill says so; a swap with a body fails.
- **Attached effects** follow the unit; spot effects stay. Constructs and traps can be moved like units.
- **Reactions.** Forced movement triggers only reactions that say "when moved".
- **Stealth** is not affected by movement.

## 10. Stealth, KO and statuses

### 10.1 Stealth

A stealthed unit cannot be targeted by the opponent, is ignored for "nearest" (section 8.5) and never intercepts. Areas still hit it.

- **Gaining.** Stealth comes only from skills and keystones (Vanish, Melt Away, Unseen, Vanishing Kill, Escape Artist). There is no global row restriction.
- **Breaking.** Stealth ends when the unit **attacks or is hit**, whether or not damage gets through: area hits, hits that reach it without targeting it, hits a Ward fully absorbs and bump damage all count. An intercepted hit hits the interceptor instead and does not break it. Damage that is not a hit (Poisoned, Bleeding, ground damage) does not break it. A skill may override this (Feint keeps it). KO ends it, and reveal effects remove it (Read Tracks, Eagle Eye).
- **Cleansing.** A cleansing skill that hits a stealthed unit removes its stealth (Dispel).
- **Duration.** Stealth lasts until broken unless a skill adds a timer.
- **Visibility.** Stealth is all or nothing; adjacency and alignment never reveal a unit. Any stealth may hide a whole side; areas and melee reach still hit stealthed units.
- **Presentation.** The opponent sees the unit with a clear stealth cue and its turns stay on the timeline. Stealth limits targeting, not information.
- **Locked targets that become stealthed** (delayed and auto events): a `direct` effect still hits its slot, and whoever stands there. A `projectile` passes the stealthed unit and hits the unit behind it (section 8.4); if nothing is hit, the shot is lost. Row and area effects are unaffected.
- **Movement** does not affect stealth.

### 10.2 KO

**0 HP means KO.** A KO unit stays in its row and position as a **body** and keeps using row capacity; normal movement cannot move it. A body never intercepts. **KO removes all statuses.** KO has no effect after the battle; there is no permanent death.

When a unit is KO, its pending auto events and delayed events are removed, unless the skill says they persist because they need nothing more from their owner.

KO'd summons and constructs are removed at once and free their slot (section 11).

### 10.3 Healing and revival

**Healing is capped at maximum HP.**

A revived unit returns to its own body's slot and is scheduled **as though the revival had been its own action**, with no free turn. The revival skill states the HP it returns. There is no limit on how often a unit can be revived; the skill's costs are the limit. Revival skills can target bodies of either side.

### 10.4 Burnout Heal and Bound Resurrect

**Burnout Heal:** its healing resolves as one simultaneous block; then the Lifebinder is KO, before reactions are collected, so the Lifebinder's own reactions do not fire (Martyr is a "when KO" trigger and does).

**Bound Resurrect: shared pool.** The pool's maximum is the **sum of both units' maximum HP**; it starts at the reviver's current HP plus the revived unit's revival HP. Every hit on either unit reduces the pool and healing either heals it. When the pool reaches 0, **both units are KO**. The link ends when either unit is KO.

### 10.5 Statuses

**What a status is.** Any effect with a duration on a unit: the named statuses below, Wards, enchantments, curses, song effects, Marks and stealth. Helpful ones (Wards, enchantments, songs, stealth) and harmful ones (the named statuses, curses and hexes) are told apart for cleansing and stealing.

- **Durations** run in time units on the battle clock (section 6.1), whatever the unit does. Exceptions: effects that last "until the caster's next turn" (Ward, Shield Dome; they also end when the caster is KO), Frozen (skips turns) and Bleeding (counts skills).
- **One instance per status.** Reapplying refreshes it with the new duration, even if shorter. There is no stacking. A new Stun on a Stunned unit keeps whichever time is longer.
- **Source.** A status keeps going if its source is KO, unless the skill says it ends.
- **No limit** on statuses per unit; the UI handles overflow.
- **Ticking.** Damage and healing over time build up per time unit and land whenever any unit gets a turn; fractions carry over. Everything built up shows as one number on the unit. A tick that KOs a unit ends its turn.
- **Damage from statuses** (Poisoned, Bleeding, Withering Curse) is not a hit and moves Momentum 0. Reductions apply to it; Wards do not absorb it.
- **Chances.** A stated chance is the real chance; no stat lowers it.
- **Immunities** come only from skills and keystones (Sanctity, War Drums, Spell Ward, Unstoppable, Steadfast Ward, Eye of the Storm). Creatures have none of their own. Every status applies to summons, constructs and totems unless their skill says otherwise.
- **Cleansing** can remove any status unless the status says it cannot.
- **New statuses** join the table only when a skill or attack needs one.

| Status | Effect | Duration |
|---|---|---|
| **Blind** | Each attack of the unit has a 50% chance to miss. A miss moves Momentum 1 against the unit's side. | As stated (Flash Powder: 20) |
| **Poisoned** | 1 damage per 10 time units, built up and landing whenever any unit gets a turn. | As stated (usually 20) |
| **Bleeding** | 2 damage after each `melee` or `move` skill the unit uses (Move included). | The unit's next 3 such skills |
| **Silenced** | The unit cannot use `magical` skills or a magical attack. | As stated |
| **Frozen** | The unit's next turn is skipped, at the skipped-turn cost (section 6.1). It cannot intercept. It ends early when fire is set on the unit's spot. | The unit's next turn |
| **Stunned** | The unit takes no actions and cannot intercept. Each of its turns that arrives while Stunned is skipped, at the skipped-turn cost. Any hit ends it early. | As stated |
| **Confused** | Each skill the unit uses, helpful ones included, has a 50% chance to target a random legal unit instead, on either side. | As stated |
| **Immobilized** | The unit cannot Move and cannot be moved. It cannot intercept. | As stated |
| **Marked** | Only enemies can be Marked. A skill that reads a Mark works on every Marked enemy, whoever placed it. A Mark lasts 30 time units unless the marking skill says otherwise (Relentless); the marking skill states any limit. | 30 |

Only the ground burns, never a unit: a unit takes fire damage only from standing on burning ground (section 12.2).

### 10.6 Airborne

Some skills make a unit **airborne** (Take Off, Lift, Sky Lord). Airborne is a state on the unit, removed by KO, by Dive and by skills that make it land.

- **Reach.** Melee cannot reach an airborne unit, Dive included. Projectile and direct skills, areas and pulses can (a pulse or breath still hits only the enemy Front row).
- **Ground effects.** An airborne unit ignores every ground effect, including those already on its spot.
- **Own skills.** An airborne unit uses its skills as normal, melee from its Front row included.
- **Interception.** An airborne unit intercepts and is covered like any other unit, melee included.
- **Curse of Weight** keeps a unit from becoming airborne.

## 11. Summons and constructs

Summons and constructs come only from skills in battle. They never count as original team members for the defeat rule.

| Kind | Behavior |
|---|---|
| **Turn-taking summon** (golems, Animate Blades, undead) | Occupies space, takes turns and acts on its own. |
| **Construct** (turrets, barricades, traps, totems) | Occupies space and has HP, takes no turns. Can be targeted, moved and intercept like any unit. |

A creature that appears only inside one skill's animation is that skill's effect, takes no slot and follows the skill's delivery.

- **Placement.** The player (or the AI) puts a new summon or construct on **any free spot of its side**, between units or at either end; the row recenters. Portal places them at random instead. Raised undead follow section 11.5.
- **Cap.** A side holds at most 18 units (6 per row). A summoning skill is unavailable when the side is full; the UI says why. Skills may add their own limits (Golem Heart, Great Totem).
- **Cost.** Only what the skill states: Delay and Momentum. There is no summon resource.
- **Stats.** HP and Speed are fixed in the skill (a Clay Golem has HP 25); damage and healing scale with the summoner's Power (section 5.2).
- **Turns.** A turn-taking summon first acts `11 − Speed` time units after it appears (Quickening: at once). On each turn it attacks the **nearest legal enemy** (section 8.5) with Delay 3 and the usual Momentum shifts: golems with a melee hit, Animate Blades with a projectile hit, undead with their creature's unique attack. A summon with no legal target (a golem outside its Front row, a melee summon facing only airborne units) takes Skip Turn.
- **Lifetime.** Until KO or the end of battle, unless the skill gives a duration. When a timed summon ends it disappears and frees its slot without counting as KO. Summons stay when their summoner is KO.
- **KO.** A KO'd summon or construct is removed at once and frees its slot; it cannot be revived. Hits by and on summons move Momentum like any hit.
- **Statuses** apply to summons and constructs unless their skill says otherwise.

### 11.5 Undead

- **Raising costs a body.** Raise Undead targets a KO body of either side. The body leaves its slot, moves to a free spot on the Necromancer's side (picked by the player or the AI) and becomes an **undead**: the creature's own art in an undead frame, its own unique attack and Speed, with HP and Power from the skill. With no free spot on that side, it cannot be used.
- **Healing hurts.** Healing an undead damages it by that amount instead.
- **Bodies stay.** A KO undead leaves its body on the Necromancer's side. The units a battle starts with are never removed from the field.
- **Revival restores.** Reviving an undead, standing or KO, restores the original creature. It returns to its owner's side, into the free slot nearest its old row, and is scheduled as a new action. With no free slot on the owner's side, it cannot be targeted.
- **Defeat.** While raised, the original counts as KO for its owner's defeat check.

## 12. Ground effects and battle end

### 12.2 Ground effects

A ground effect sits on a spot (section 7.3) and affects the unit standing there; it stays on its coordinate when rows recenter. Airborne units ignore it.

- **"The spots next to" a spot** are one card width to the left and right in the same row (coordinate ±2), within the row's width (coordinates −5 to 5). **"A whole row"** means the spot of every unit standing in that row; empty gaps get nothing.
- **Length.** Each skill states its length; 20 time units is the reference. Placing the same effect again restarts its length. A combo starts the full length of the new effect. The effects stay when their caster is KO.
- **Damage over time** builds up for every time unit a unit stands on the effect and lands whenever any unit gets a turn. A unit moved on or off starts or stops building up then.

| Effect | What it does | Combines with | Sources |
|---|---|---|---|
| **Burning ground** | 1 damage per 2 time units to the unit there (10 over 20). Wards absorb it. | Ice: the fire burns there for 10, then both end and the spot holds water, whichever came first. Water: fire set on water burns there for 10, then both end and the water becomes steam; water placed on fire ends the fire at once and stays. Poison ground: explosion. | Pyromancer, Chemist (Firebomb), attack extras |
| **Ice** | A unit pushed or pulled across it slides one row further (bumps apply). | Fire: water after 10. | Ritualist (Hoarfrost), Cryomancer (Deep Chill), attack extras |
| **Water** | Harmless alone. | Any Stun on a unit standing on water also Stuns the units on water spots touching it, for half the time (rounded down). Ice placed on water: the water turns to ice and the unit there is Frozen. Fire set on water: steam after 10. Water placed on fire: the fire ends. | Ritualist (Rain Dance), attack extras |
| **Steam** | Not a lasting effect: when fire turns water to steam, the unit there is Confused for 10 and the spot is left clear. | | Fire on water |
| **Poison ground** | A unit that has stood there for 5 time units is Poisoned for 20, and every further 5 restart its Poison. | Fire placed on the same row as poison ground at once: every poison spot in that row explodes, each unit on one takes 6 and the spot becomes burning ground. | Chemist (Poison Gas), attack extras |

Water spots touch when they are side by side in a row or overlap across neighbouring rows.

### 12.3 Battle end

- **Defeat.** A side loses when all of its **original members** are KO at the same time; summons do not prevent it, and a revived member counts again. If both sides' last original members are KO in the same block, the battle is a **draw**. A side does not lose just because it has no attack available.
- **Victory check** after each event's KO check and its reactions; a counter can still KO the attacker and turn a win into a draw. Once the result is decided, pending events and hits are cancelled.
- **Draws** count as a defeat for the player in single-player modes; in local two-player and AI versus AI they are recorded as draws.
- **Surrender** is possible at any time and counts as a defeat with no other penalty.
- **No time limit** and no escalation; only the AI-versus-AI harness has a length cap (section 2.5).
- **Objectives.** Every battle uses the standard defeat rule; special objectives are excluded for now.
- **Enemy side.** Six original members by default; an encounter may use another number.
- **Enemy behavior.** Enemies use the AI of section 2.5. The AI has three levels, **Easy, Normal and Hard**, which set only search depth and weaker picks. Difficulty comes from the enemy team and formation; there are no hidden stat multipliers. In free and random battles the player picks the level (a random battle draws from the whole unlocked roster at every level); in the campaign each encounter sets it and it rises through the chapters.
- **Encounter exceptions.** None for now; encounters differ only in team, formation and size.

## 13. Cards and combat presentation

Screens, layouts and interactions are owned by [ui_spec.md](ui_spec.md); this section sets the principles.

### 13.1 Cards

A card represents a team member, not something drawn or played. Its **front** shows the portrait crop of the creature's art in its frame, the four stats, and a name plate with the name and its two class names. Its **back** shows the whole creature and lists the unique attack, the six class skills and the two keystones, with the current numbers for that creature. The back is for inspection, not an action menu: players can inspect every unit, opponents included, and see exactly what it can do.

On the board a unit is a **round battle portrait** showing HP and KO only; statuses and details are in the inspection panel (ui_spec.md). Cosmetic frames never reduce readability, and KO is always unmistakable while the body visibly keeps its place.

### 13.2 Team editor

Teams are built in a **team editor** like a deck builder: up to six creatures from the collection within the budget of 35 Cost, placed in rows, saved under a name. Several teams can be saved; the last used is preselected. Every mode starts from a saved team. Creatures are added, moved and removed by dragging (ui_spec.md).

### 13.3 Battle interface

The battle screen shows the board (player's side left, enemy right), the timeline, the Momentum meter and the active unit's action bar, without nested menus. The interaction is **select skill → highlight legal targets → select target → execute**; skills with no target follow section 6.3. Illegal targets are visibly unavailable; areas, interception chances and timing consequences are previewed where they can be foreseen (section 8.3). Enemy units' future choices are never revealed. Layout: ui_spec.md.

### 13.4 Animation and effects

Units do not travel across the board when attacking: effects start at the acting unit and travel to or appear on their targets. Brief shakes and flashes are allowed; the board stays spatially stable. Explicit movement skills still move units. Healing, buffs, debuffs and KO need distinct, restrained feedback. There are no critical-hit effects.

### 13.5 Relationships

Relationships (adjacency, cover, summoner, Marks) are shown in context when relevant or inspected, not permanently. Players see clean rows; the half-card grid stays internal.

## 14. Art direction

- **Look:** charming, colourful, varied, warm and slightly fairy-tale; strongly stylized and readable; simple shapes, strong silhouettes, broad colour zones, two to five major colour blocks with an accent.
- **Creatures:** a clear silhouette and one or two memorable features; simple but expressive faces, not all friendly. Classes must be visible in the art (class_system §3), but there are no mandatory class outfits or colours.
- **Size:** cards do not show creature size; every creature fills its frame.
- **Biomes (18, all at release):** Pine and dry forest, Jungle, Wet and mixed forest, Desert, Mountain, Giant cathedral or castle (inside), Grassland and savannah, Arctic and iceberg, Swamp and bog, Coast and tide pools, Volcanic ashlands, Crystal caverns, Mushroom grotto, Lagoon and reef, Overgrown ruins, Autumn woods, Storm cliffs, Moonlit flower meadow. Every creature has one **home biome**; backgrounds stay dark (night look).
- **UI and effects:** clean panels, modest ornament, bold icons, legible text, comfortable touch targets; effects never obscure the board.
- **Artwork** is delivered as separate images in the card canvas, never as finished cards with baked-in UI.

Art files, backgrounds, sound, music and settings: production.md §1.6.

## 15. World and campaign

**Setting.** The land is called **Everkin**, a gentle night fairytale where wild creatures and enchanted objects live side by side, with a hint of danger. There is **no player character** and **no story text** beyond chapter and encounter names: the campaign follows the creatures themselves. The team travels toward the castle seen in the distance on every map. Creatures test strangers, and the ones the team beats respect it and may join; they are given no further reason. Nobody rules the castle: its objects woke up on their own, and its final boss is the castle's heart, whose waking wakes the dragons that slept beyond it.

**Structure.** The campaign opens on a **world map** with the chapters as regions; tapping one opens its chapter map. Locked chapters are dark and misty, their names hidden until reached. In the game a stage is called a **Chapter**.

- **Chapters 1 and 2: the wild lands.** Each battle may be in a different biome along the journey; a chapter covers 3 to 5 neighbouring biomes.
- **Chapter 3: the castle.** Opponents are mostly the living object creatures, in the castle's halls and towers.
- **Chapter 4: the dragons.** About five battles; dragons lead mixed teams, one per battle, each in its home biome and giving that dragon as the reward; the strongest is the final boss. Dragons are not foreshadowed before the castle.

Every encounter, boss, reward, challenge encounter and achievement is in [campaign.md](campaign.md).

## 17. Scope

**First release:** AI-versus-AI testing, single-player battles against the AI (campaign, free battles, post-story random battles and challenge encounters), local two-player and online PvP, on Windows, Android and iOS in landscape. Summons and constructs are fully in. Bosses are normal-size units.

**Excluded or deferred:** terrain and reduced-capacity arenas, large multi-slot units, special battle objectives, creature levels, equipment and loot, capture, trading, crafting, cooperative play, story events, relationships between creatures, and genetic variation.

Implementation starts with AI-versus-AI combat; the order of work is in production.md §3 and docs/implementation-plan.md.

## 18. Identity

**Everkin** is the chosen name, subject to an availability check (trademarks, store listings, domains) before any store page. All art, designs, UI, names, text, music and branding are original. Reference games (FFX for turn order, Hearthstone for card presentation and its deck builder, Siralim Ultimate and Monster Sanctuary for team building, and others) guide broad goals only; their rules are not adopted.

## 19. Open decisions

| Area | Open |
|---|---|
| Party size | Prototype check of six-versus-six readability; a smaller standard team only if it proves crowded. |
| Cost | The power-score formula for the current classes, decided when AI-versus-AI testing starts; then Cost and the budget of 35 are recalculated and tuned. |
| Balance | Every number in skills.md and creature-classes.md, tuned by AI-versus-AI testing. |
| Encounters | Tuning the rosters, boss stats and AI levels of campaign.md in testing. |
| Content | Each chapter's conflict and later full campaign design. |
| Statuses | New statuses only when a skill needs one. |
| Presentation | Mobile readability checks of party and capacity limits. |
| Delivery | Technical validation and the name's availability check. |

Gaps in this design are not permission to inherit rules from Hearthstone or another reference game.

## 20. Before implementation

### 20.3 Content definitions

- **Creature:** unit name (ID), display name, art and crops, HP, Speed, Power, Cost, home biome, two classes, two keystones, unique attack (creature-classes.md), and how it is acquired (campaign.md, coin price).
- **Skill and attack:** Delay, Momentum, delivery, target, area, ordered effects, chances and numbers at Power 5, reactions, KO and retarget behavior (skills.md, creature-classes.md, and the rules here).
- **Status:** effect, duration, ticking, reapplication, cleansing and KO behavior (section 10.5).
- **Encounter:** team, formation, AI level, rewards (campaign.md).

Final content lists must be complete; examples and partly filled entries do not count as implemented content.

### 20.5 Worked checks

Concrete examples of the rules; each becomes a test. They are expected behavior, not claims that tests pass.

| Situation | Expected result |
|---|---|
| Speed 5 uses a Delay 5 skill | Wait is 5 + (11 − 5) = 11 time units. |
| A Speed 8 unit and a Speed 5 unit start a battle | First Waits are 3 and 6, so the Speed 8 unit acts first. |
| A unit is pushed back into a time unit that already holds another unit's turn | The unit that was there first acts first; the moved one acts after it. |
| Every unit's first turn falls in the same time unit at battle start | A seeded roll orders them, each order equally likely, and the same seed gives the same order. |
| A unit chooses Skip Turn at Speed 9 | It waits 5 + (11 − 9) = 7 time units. |
| A status's end joined a time unit before a unit's turn moved into it | The status ends first, so the unit acts without it. |
| Song of Haste reaches a Speed 5 ally 4 time units into an 11-unit Wait | Its Speed is 6; the remaining 7 becomes 6; completed progress is kept. |
| Speed 10 with Delay 1 versus Speed 1 with Delay 10 | Waits are 2 and 20. |
| A skill with Delay 9 gets +3 Delay from Jam | Its Delay is 10. |
| Three-card row opposite a two-card row | Relevant overlaps are 50% or 0%. |
| A unit becomes KO | Its statuses clear; it keeps its position and row capacity; it no longer intercepts. |
| Healing exceeds missing HP | HP stops at maximum. |
| Start another battle | Every unit begins at full HP, Momentum is 0, and nothing carries over. |
| A melee attack aims at an enemy in the Middle row behind a Front unit at 100% overlap | The Front unit intercepts with 75%. |
| A projectile aims at the same enemy | The Front unit intercepts with 50%. |
| A melee attack aims at a Rear enemy behind a Front unit (100%) and a Middle unit (50%) | The Front unit tries first at 75%; if the hit (or a remainder) continues, the Middle unit tries at 50%. |
| An interceptor takes a partial hit of 7 | It takes 3 and 4 continues toward the target. |
| An interceptor takes a partial hit of 1 | It cannot be split; the interceptor takes 1. |
| Two units in the row in front each half-cover the target | A random one of them, each with equal chance, tries first. |
| A unit in the Middle row has only a melee attack | It cannot attack; it can use its non-melee class skills, Move or Skip Turn. |
| A side's Front row is empty | None of its units can use melee until one moves into Front. |
| A Breath attack is used while the enemy Front row is empty | It can be used; it hits nobody, and its Delay is spent. |
| A pulse attack is used while the enemy Front row holds three units, one of them stealthed | All three are hit (an area hits stealthed units). |
| A Breath attack hits a Front unit that has a neighbour and a Middle unit behind it at 50% overlap | Both are adjacent and take the splash. |
| A Breath attack hits a Front unit, the Middle row is empty and a Rear unit is behind it at 100% | The Rear unit is not adjacent and is not hit. |
| A melee attack is aimed at an airborne enemy | It is not a legal target. |
| An airborne unit stands in front of the target of a melee attack | It may intercept, like any unit. |
| Dive is used from the Rear row | It is legal and cannot be intercepted; an airborne target is not legal. |
| Phase Weapon is on a unit whose next attack is melee | The attack cannot be intercepted; the unit still needs to stand in its Front row. |
| A Keeper in the Middle row uses Ward on an ally in the Rear row | It works: own-side targets have no range and are never intercepted. |
| A Keeper in the Middle row tries Ward Burst | It cannot: Ward Burst needs the Keeper in its Front row. |
| A Healing Draught (projectile) is aimed at an ally behind another ally | It cannot be intercepted, because own-side skills are never intercepted. |
| Hunter's Shot (projectile, Tracker) is aimed at an enemy and partly intercepted for 9 | The interceptor takes 4 and the rest, 5, continues. |
| A projectile heal of 9 is aimed at an enemy and partly intercepted | The interceptor heals 4 and the target 5. |
| Jam (projectile) is intercepted | The interceptor gets the +5 Delay; the target does not. |
| A `direct` skill is aimed at a unit with a defender in front | It cannot be intercepted. |
| A reaction's hit is aimed at a unit with a defender in front | The defender does not intercept: reaction hits go straight to their target. |
| A 9-damage hit lands on a unit with no Ward | It deals 9. |
| The same hit lands on a unit with a Ward of 8 | The Ward takes 8 and 1 reaches HP. |
| A 7-damage hit lands on a unit with a Ward of 8 | Nothing passes; no damage is dealt and Momentum does not change. |
| A unit with 3 Ward left gets Shield Dome's Ward 2 | It keeps the Ward with 3 left. |
| A unit with Ward 2 left gets Ward 4 | The new Ward 4 replaces it. |
| A Power 5 unit with a 4–5 attack hits a 20 HP critter | The critter falls on the 4th or 5th hit, never earlier. |
| A Power 3 Mender uses Mend (8–10 at Power 5) | It heals 4–6 (8 × 3 ÷ 5 = 4.8 → 4; 10 × 3 ÷ 5 = 6). |
| A Power 7 Keeper uses Ward (4 at Power 5) | The Ward absorbs 5 (4 × 7 ÷ 5 = 5.6 → 5). |
| A unit uses a Momentum 2 skill with its side at +2 and the first hit lands | The cost is paid first (meter 0), then the hit moves it to +1. |
| Every hit of a Momentum 3 skill is intercepted | The 3 Momentum is not refunded. |
| A Silence lands on an enemy | Momentum does not change. |
| A unit uses its attack on its own ally | The ally takes the hit and Momentum does not change. |
| An attack is intercepted | Momentum does not change. |
| A Bolt Turret hits on three of the Engineer's turns | Momentum shifts three times. |
| A pulse hits three enemies and two hits land | One shift of 1 toward the attacker's side. |
| A Poisoned unit ticks | It takes 1 per 10 time units, landing whenever any unit gets a turn; Momentum is unchanged; a Ward does not absorb it. |
| Poison with 30 time units left is reapplied with 20 | It now has 20 left. |
| Burning ground damages a unit with a Ward of 8 | The Ward absorbs the damage. |
| A unit with Bleeding (3 skills left) uses a melee skill | It takes 2 after the skill, and 2 skills remain. |
| A Bleeding unit uses Move | It takes 2 after moving. |
| A Bleeding unit uses a projectile skill | It takes nothing. |
| A Blind unit's attack misses | The miss moves Momentum 1 against its side. |
| A Silenced unit's attack has the bolt style | It cannot use it (a bolt attack is magical). |
| A Silenced Mender's turn arrives | It cannot use Mend, Rain of Mending or Searing Light, but can use a physical attack, Move or Skip Turn. |
| A Frozen unit's turn arrives | The turn is skipped and Frozen ends. |
| Fire is set on a Frozen unit's spot | Frozen ends early and the unit keeps its turn. |
| A Stunned unit's turn arrives | The turn is skipped and costs 5 + (11 − Speed); the Stun keeps running. |
| The caster of a Poison is KO | The Poison keeps ticking. |
| A stealthed unit is hit by a pulse | It is hit and its stealth ends. |
| A stealthed unit with a Ward is hit and the Ward absorbs everything | Stealth still ends; Momentum does not change. |
| A stealthed unit takes a Poison tick | Stealth does not end. |
| A stealthed unit stands in front of an ally that is attacked | It cannot intercept. |
| A side's last visible unit uses Vanish | It works; the enemy can still hit it with areas and melee reach. |
| A queued projectile shot's target becomes stealthed | The shot hits the unit behind it under the behind rule. |
| A queued `direct` effect's target moves away and another unit takes its slot | The effect hits the unit now in that slot. |
| A skill hits behind its target, the row directly behind is empty and the Rear row has a unit at 100% overlap | The Rear unit is hit. |
| A skill hits behind its target and the next row has units, none overlapping the target | Nothing is hit behind. |
| A skill hits the nearest enemy and two are equally near | A seeded roll picks one; a replay from the same seed picks the same one. |
| A Speed 6 unit uses Move | It lands in the gap its player chose and waits 5 + (11 − 6) = 10. |
| A unit tries to Move into a full row | That row is not offered. |
| Gust pushes a Front unit back while the Middle row is empty | It lands in Middle and nothing is bumped, even with a Rear unit behind it. |
| A unit is pushed back with a unit directly behind it at 100% overlap | That unit is pushed back too. |
| A unit is pushed into a row where it lands at 50% overlap between two units | It is inserted between them. |
| A pushed chain of three units is blocked by a full row | Nothing moves, and each unit in the chain takes 1 bump damage. |
| A stealthed unit takes bump damage | Stealth ends; Momentum does not change. |
| A unit with Static Field takes bump damage | Static Field does not trigger (bump damage is not melee). |
| A side has 18 units and its Engineer uses Barricade | The skill is unavailable, and the UI says why. |
| A Net Trap is KO | It is removed at once and its slot is free; it cannot be revived. |
| An Engineer is KO while its turrets are on the board | The turrets stay; its queued shots are removed. |
| A Speed 3 Clay Golem appears at time 10 | It first acts at time 18 (11 − 3 = 8 later). |
| A Clay Golem stands in the Middle row on its turn | It has no legal target and takes Skip Turn. |
| Raise Undead is used with no free spot on the Necromancer's side | It cannot be used. |
| An undead is healed for 6 | It takes 6 damage. |
| Bound Resurrect: reviver 30 HP of 40, revived unit max 20 with 25% revival HP | The pool maximum is 60 and it starts at 30 + 5 = 35; both are KO when it reaches 0. |
| All original members are KO while a summon stands | That side loses. |
| Both sides' last original members are KO in one block | Draw. |
| A player's last original member is KO by a counter after the player's attack KO'd the enemy's last member | Draw, since victory is checked after reactions. |
| A draw in a single-player battle | It counts as a defeat for the player. |
| A battle reaches the harness's cap | The harness reports it and records a draw. |
| A player surrenders | Defeat with no other penalty. |
| A coin creature has Cost 6 | It costs 50 + 25 × 6 = 200 coins. |

## 21. Definition of done

The specification is ready when every included feature has concrete rules, content, numbers, UI behavior and completion criteria, with no contradictions or open decisions it depends on. Implementation is done when everything in the final versions of this document and the UI specification is implemented and verified: all modes, complete content, working play flows, saving, presentation, online and the platform builds. No required feature may be silently left out or replaced by an unapproved placeholder. A prototype is a milestone, not the finished game.
