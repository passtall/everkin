# Analysis of harness run 20261116-210306-harness

- `run/`: the run to analyze (level harness, seeds 1-1000: a shortened run of 1,000 battles, not the full 10,000; rules commit in `run.json`).
  `battles.jsonl` has every battle (with the Momentum meter after every unit turn, every payment, and per skill the turns it was legal, searched further and chosen); `report.md`, `creatures.csv`, `classes.csv`, `skills.csv`, `pairs.csv`, `rows.csv` are the summaries; `snapshot/` is the content and AI settings the run used; `run.json` is the run's parameters.
- `docs/`: the rules and content tables the numbers come from (Cost formula: game_system.md section 5.1).
- M5.9 changes: Momentum with diminishing value, tempo, one weight per status, summon and totem/trap scoring, costed and control candidates searched further, formation by role.
