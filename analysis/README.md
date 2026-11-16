# Analysis of harness run 20261116-191930-harness

- `run/`: the run to analyze (level harness, seeds 1-5737, **stopped early at 5737 of the planned 10,000 battles** by the owner; rules commit in `run.json`).
  `battles.jsonl` has every battle (now with the Momentum meter after every unit turn and every payment); `report.md` (with the new Momentum section), `creatures.csv`, `classes.csv`, `skills.csv`, `pairs.csv`, `rows.csv` are the summaries;
  `snapshot/` is the content and AI settings the run used; `run.json` is the run's parameters.
- `docs/`: the rules and content tables the numbers come from (Cost formula: game_system.md section 5.1).
- Changes against the previous run: Momentum point 0.5%, status relevance and renewal in the AI score, harness formation by unique-attack delivery, Searing Light 4-5, Vanish costs 1.
