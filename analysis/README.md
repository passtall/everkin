# Analysis of harness run 20261117-175706-harness

- `run/`: the run to analyze (level harness, seeds 1-4085 played of 10,000 planned: the owner stopped it early; rules commit in `run.json`).
  `battles.jsonl` has every battle (with the Momentum meter after every unit turn, every payment, and per skill the turns it was legal, searched further and chosen); `report.md`, `creatures.csv`, `classes.csv`, `skills.csv`, `pairs.csv`, `rows.csv` are the summaries; `snapshot/` is the content and AI settings the run used; `run.json` is the run's parameters.
- `docs/`: the rules and content tables the numbers come from (Cost formula: game_system.md section 5.1).
- M5.10 changes: new Momentum costs 0-3 for all 84 actives (every class has a free and a costed active).
