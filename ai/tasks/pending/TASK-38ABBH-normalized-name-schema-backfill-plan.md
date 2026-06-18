# TASK-38ABBH

---
id: TASK-38ABBH
title: Normalized name schema and backfill plan
status: pending
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Define and implement the smallest safe schema and backfill path for normalized display-name and civilization-name fields.

## Context
Prepared normalized-name fields now exist in the domain model, but relational rollout still needs a safe plan for existing persisted rows. This task should keep automatic database application out of the repository while making the schema/backfill path explicit and reviewable.

## Implementation steps
1. Read the files listed below before editing.
2. Review the prepared normalized-name model and current migration snapshot pattern.
3. Choose the smallest safe schema-plus-backfill approach for persisted rows.
4. Keep real-database migration application manual and opt-in.
5. Run repository validations before moving the task to done.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Domain/Players/
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- src/VoidEmpires.Infrastructure/Persistence/Migrations/
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs

## Expected files to modify
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- The schema/backfill path is implemented or the remaining blocker is explicit.
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
