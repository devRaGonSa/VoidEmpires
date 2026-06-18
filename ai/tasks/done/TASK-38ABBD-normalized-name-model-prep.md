# TASK-38ABBD

---
id: TASK-38ABBD
title: Normalized name model preparation
status: done
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Prepare the domain, persistence mapping, and query path for normalized display-name and civilization-name lookups without broadening into a full relational rollout.

## Context
The current `StartingCivilizationService` uses case-insensitive `ToLower()` comparisons. This follow-up should prepare the smallest provider-agnostic normalized-name model shape and service query plan that later migration work can activate cleanly.

This task is intentionally limited to domain-model preparation. The relational write-path and indexed persisted rollout remain in `TASK-38ABBE`.

## Implementation steps
1. Read the files listed below before editing.
2. Define the smallest normalized-name shape needed for `PlayerProfile` and `Civilization`.
3. Update the related service/query path only as far as the prepared model allows safely.
4. Keep the change scoped to existing components and tests.
5. Do not apply or run real-database migrations automatically.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Domain/Players/
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- tests/VoidEmpires.Tests/StartingCivilizationServiceTests.cs

## Expected files to modify
- src/VoidEmpires.Domain/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- A normalized-name model strategy is prepared in code or the remaining blocker is made explicit.
- The work stays within the small-task change budget.
- Validation commands pass.

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
