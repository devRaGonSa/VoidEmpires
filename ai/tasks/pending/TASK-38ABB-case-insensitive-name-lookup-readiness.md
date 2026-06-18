# TASK-38ABB

---
id: TASK-38ABB
title: Case-insensitive name lookup readiness
status: pending
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Refine display-name and civilization-name uniqueness checks so they remain provider-agnostic and index-friendly for SQL Server readiness.

## Context
`TASK-38ABA` covered ownership/session/planet lookup indexing. The remaining lookup gap is the case-insensitive `ToLower()` query shape in `StartingCivilizationService`, which is not solved honestly by adding plain indexes alone.

## Implementation steps
1. Read the files listed below before editing.
2. Review the current display-name and civilization-name uniqueness checks.
3. Choose the smallest provider-agnostic normalization or lookup strategy that can use indexes safely.
4. Add or update tests for the chosen strategy.
5. Run the repository build and test commands before moving the task to done.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- tests/VoidEmpires.Tests/StartingCivilizationServiceTests.cs

## Expected files to modify
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- The remaining case-insensitive lookup path is implemented or narrowed with explicit blockers.
- Validation commands pass.
- No unrelated files are modified.
- No secrets, unsafe connection strings, or local machine state are committed.

## Constraints
- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation
Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push
At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only`.
3. Stage the intended files.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
