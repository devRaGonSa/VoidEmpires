# TASK-38ABBE

---
id: TASK-38ABBE
title: Normalized name migration rollout
status: done
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Roll out the relational migration or persisted column change required to back normalized display-name and civilization-name lookups.

## Context
This task depends on the normalized-name model preparation task. It covers the relational shape needed so provider-agnostic case-insensitive uniqueness checks can use indexed persisted values instead of non-sargable lowercase comparisons.

This task is narrowed due to the repository change budget. The remaining work is split into a query-path activation task and a separate migration-artifact task.

## Implementation steps
1. Read the files listed below before editing.
2. Review the prepared normalized-name model shape and required persistence artifacts.
3. Add the smallest safe migration or relational rollout needed for the prepared model.
4. Keep Development flow safe and do not apply migrations automatically to the user's real database.
5. Run the repository validations before moving the task to done.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- src/VoidEmpires.Infrastructure/Persistence/Migrations/
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- tests/VoidEmpires.Tests/StartingCivilizationServiceTests.cs

## Expected files to modify
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- The normalized-name relational rollout is implemented or narrowed with explicit blockers.
- Validation commands pass.
- No automatic real-database migration execution is introduced.

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
