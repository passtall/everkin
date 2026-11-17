# Sample team batches

`batches/<name>.json` are named sets of sample teams for the AI-versus-AI harness, built by `TeamBuilder` (`src/Everkin.Core/Setup/TeamBuilder.cs`). Never edit a batch after it has been played: build a new one. How they are made and improved: [docs/team-improvement-prompt.md](../docs/team-improvement-prompt.md).

- Build: `dotnet run --project src/Everkin.Runner -c Release -- teams build --name <name> --seed 1 --per-creature 2 --note "text"`
- Play: `dotnet run --project src/Everkin.Runner -c Release -- run --batch <name> --level harness --battles 1000`
