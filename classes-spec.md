# Spec: branches become classes

**Status:** DRAFT 2026-11-11, being decided in quiz rounds (round 1 applied, round 2 posted). Separate from [spec.md](spec.md) (battle UI work order). Until this spec is final, the current rules in docs/ stay valid; when it is final, its decisions are written into class_system.md, skills.md, game_system.md, creature-classes.md and ui_spec.md, and this file becomes the work order for the code.

## 0. Owner's request (2026-11-11)

1. The branches of the current professions become classes (roughly as they are; they will be adjusted later).
2. Every creature gets **two** of the new classes.
3. Skill points and skill trees are gone.
4. A creature has **all** skills of its classes unlocked at all times.
5. Keystones stay as plain passives. Creatures with the same class may still have different keystones (a keystone is assigned per creature, not per class).
6. Classes may have damage skills again; most classes should have at least one.

First step: go through every new class and decide whether it stays, merges into another or is removed.

## 1. Classes (round 1 decided 2026-11-11)

The 12 professions had 38 branches in [skills.md](docs/skills.md). Round 1 (https://claude.ai/artifact/TuKYsxEXMpwNqNjvSsc4hd, db doc answers/round1) kept or merged each one; none was removed outright. **30 classes:**

| Old profession | Classes (merged branches in brackets) |
|---|---|
| Hunter | Stalker, Tracker (+ Predator); Trapper merged into Engineer |
| Guardian | Bulwark, Bodyguard, Keeper |
| Healer | Mender, Cleric, Lifebinder |
| Sorcerer | Enchantment, Transmutation, Malediction |
| Elementalist | Pyromancy, Stormcalling, Cryomancy |
| Warrior | Berserker (+ Whirlwind), Champion (+ Footwork) |
| Leader | Tactician, Commander, Bard |
| Rogue | Thief, Assassin, Saboteur |
| Tinker | Engineer (+ Trapper), Chemist (+ Pyrotechnician) |
| Summoner | Necromancer, Golemancer + Conjurer (one class, name open) |
| Primalist | Totemist, Ritualist, Spiritwalker |
| Flying | Flying (Mobility, Skywatch and Assault in one class) |

New skill requested for **Stalker**: a real backstab, only from stealth, range 1, high damage (round 2 suggests "Pounce": D 5 / M 2, Melee · D1, hit for 12–14, breaks stealth).

Round 2 (which skills each class keeps): same page, db doc answers/round2.

## 2. Open questions (later rounds)

- **Names.** Some branch names read as skill groups, not classes (Mobility, Assault, Enchantment, Pyromancy). Rename where needed; a class must still be visible in the art (class_system §4).
- **Creature assignment.** 319 creatures need exactly two new classes each; 56 have only one profession today. Does a creature's two classes have to come from its old professions?
- **Keystones per creature.** Which creature gets which keystone, how many at most (0 or 1?), and whether classes without a keystone get one.
- **Damage skills.** Which classes get one, and how strong (today a damage skill must "count": big, with a condition or a cost).
- **Thin classes.** Classes with 3 or 4 skills (Trapper, Footwork, Tactician, Conjurer and others) may need more skills; with everything unlocked, class size sets how many buttons a creature has.
- **Attack Type.** Stays as it is (one per creature, its shared attack)? Confirm.
- **Action bar size.** Two classes with all skills plus the attack can mean 12 or more buttons (ui_spec action bar).
- **Balance.** Cost (game_system §5.1, cost.py) and the AI harness builds depend on classes and learned skills; `content/default_builds.json` and the tree editor go away.
- **Marked.** Marks come from Tracker, Champion, Assassin and Skywatch; check that each Mark payoff class can still get Marks.
