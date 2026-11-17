# Prompt: improve the sample teams from new run data

Use this prompt at the start of a session, after a harness run with a team batch has finished (`run --batch <name>`). It improves the **team builder**, not the game rules. The owner approves every change before it is made.

## Background

- Sample teams are built by `src/Everkin.Core/Setup/TeamBuilder.cs` (version in `TeamBuilder.Version`) and stored as named batches in `teams/batches/<name>.json`. A run copies its batch into `runs/<id>/batch.json`.
- Rules of the builder (owner decisions 2026-11-17): rule-based builder; roles derived from the unique attack and classes (`TeamBuilder.RoleOf`); templates `balanced`, `wall`, `aggro`, `support_heavy`; every team built around one anchor creature, equal teams per creature; Cost 33 to 35; melee in the Front row, ranged in the Rear, blockers in the middle of the Front row; at most one Commander; healers need two durable frontliners, a Bard two attackers, a Hexer or Saboteur three attackers.
- A batch run plays teams of the same batch against each other (`TeamBatch.SpecFor`); the report adds `teams.csv` (battles, wins and win rate per team).
- Batches are never edited after a run. A rule change makes a new batch (`baseline-1`, `baseline-2`, ... or a name that says the change). One batch stays the baseline so a change is always measured against the same teams.

## What to do

1. Read [PROGRESS.md](../PROGRESS.md), then the latest run in `runs/` or `analysis/`: `report.md`, `teams.csv`, `creatures.csv`, `classes.csv`, `skills.csv`, and the batch file with its template and anchor per team.
2. Find out **which builder rules produced weak or strong teams**, with numbers. Look at, at least:
   - win rate per template, and per anchor role (`TeamBuilder.RoleOf`);
   - teams at the bottom and top of `teams.csv`: what do they share (a class pair, a support with no payoff, no blocker, a row layout)?
   - whether the payoff rules held: teams with a healer, Bard, Hexer or Saboteur against teams without;
   - whether any creature or class sits in most of the strong teams (a sign of a game balance problem, not a builder problem);
   - draws and very long battles: which team shapes stall?
   - creatures that stay at the bottom even in their best teams (they are a balance problem, report them separately).
3. Separate **builder problems** (bad rule, bad template, bad formation) from **balance problems** (a creature, class or skill that is too weak or strong in sensible teams). Fix only the first kind here; list the second kind for the balance dialog.
4. Propose changes to the builder as a short numbered list: what rule, why (the numbers), what batch it makes, how it will be measured. **Stop and wait for the owner's approval.** Do not edit `TeamBuilder.cs` before that.
5. After approval: change the builder, bump `TeamBuilder.Version`, add or update a test in `TeamBuilderTests.cs`, build the new batch (`dotnet run --project src/Everkin.Runner -c Release -- teams build --name <name> --seed 1 --per-creature 2 --note "<what changed>"`), commit the batch file with the code, and update PROGRESS.md. Never invent a game rule; the builder only chooses teams.
6. Start the comparison run on the new batch and on the baseline (`run --batch <name> --battles 1000`), and report the difference in the next session.

## Rules for yourself

- One question at a time to the owner, never a list.
- Report faithfully: if a change made teams worse, say so and say what you would revert.
- Sample sizes: 1,000 battles over 600 teams is about 3 battles per team. Compare templates, roles and classes, not single teams, unless a team has at least 20 battles.
