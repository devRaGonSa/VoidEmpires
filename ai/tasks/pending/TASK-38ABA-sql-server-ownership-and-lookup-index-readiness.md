# TASK-38ABA

---
id: TASK-38ABA
title: SQL Server ownership and lookup index readiness
status: pending
type: backend
team: backend
supporting_teams: [platform]
roadmap_item: "Block 38A-38AZ - Final SQL Server Database & Catalog Consolidation v1"
priority: high
---

## Goal
Audit and add any remaining ownership, session, planet, and civilization lookup indexes needed for SQL Server-backed gameplay flows.

## Context
`TASK-38AB` was narrowed to the three queue-heavy due-order tables to stay within the repository change budget. This follow-up covers the remaining lookup paths that are adjacent to the same SQL Server readiness work.

## Implementation steps
1. Read the files listed below before editing.
2. Review current query shapes for ownership, session, planet, and civilization lookups.
3. Add only the missing indexes or safe query-shape adjustments needed for current SQL Server readiness.
4. Keep the change scoped to persistence configuration, related tests, and minimal docs only if required.
5. Run the repository build and test commands before moving the task to done.

## Files to read first
- AGENTS.md
- src/VoidEmpires.Infrastructure/Persistence/Configurations/
- src/VoidEmpires.Infrastructure/Players/
- tests/VoidEmpires.Tests/

## Expected files to modify
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/

## Acceptance criteria
- Missing ownership or lookup indexes are added or the remaining gaps are documented with explicit blockers.
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
