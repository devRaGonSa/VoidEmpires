# TASK-38ABBG

---
id: TASK-38ABBG
title: Normalized name migration artifacts
status: done
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: medium
---

## Goal
Add the relational migration and snapshot artifacts required to persist normalized-name lookup fields.

## Context
This follow-up covers the migration-layer work that would otherwise push normalized-name rollout beyond the small-task budget. It should only update the required persistence artifacts and keep real-database application manual and opt-in.

This task is narrowed. A safe migration artifact change also needs an explicit backfill strategy for existing persisted rows before normalized-name query activation can be turned on.

## Implementation steps
1. Read the files listed below before editing.
2. Review the prepared normalized-name model and current migration snapshot pattern.
3. Add the smallest required migration and snapshot updates.
4. Do not apply migrations automatically to the user's real database.
5. Run the repository build and test commands before moving the task to done.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- src/VoidEmpires.Infrastructure/Persistence/Migrations/
- src/VoidEmpires.Domain/Players/

## Expected files to modify
- src/VoidEmpires.Infrastructure/

## Acceptance criteria
- The required migration artifacts are added or the remaining blocker is made explicit.
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
