# TASK-38ABBF

---
id: TASK-38ABBF
title: Normalized name query activation
status: done
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Activate the prepared normalized-name lookup path in application queries and tests using the existing model preparation work.

## Context
`TASK-38ABBD` prepared normalized-name values in the domain model. This follow-up should switch the active duplicate-detection query path over to the prepared normalized values as far as the current relational shape safely allows.

This task is dependency-blocked by `TASK-38ABBG`. Activating the query path before the relational artifacts exist would make current relational databases fail on missing normalized-name columns.

## Implementation steps
1. Read the files listed below before editing.
2. Review the prepared normalized-name model and current query path.
3. Update the smallest safe query path and related tests.
4. Keep the change scoped and avoid automatic real-database migration execution.
5. Run the repository build and test commands before moving the task to done.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- src/VoidEmpires.Domain/Players/
- tests/VoidEmpires.Tests/StartingCivilizationServiceTests.cs

## Expected files to modify
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- Active duplicate-detection queries no longer rely on inline lowercase comparisons where the prepared model can replace them safely.
- Validation commands pass.
- No unrelated files are modified.

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
