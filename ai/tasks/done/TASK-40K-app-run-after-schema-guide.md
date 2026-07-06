# TASK-40K

---
id: TASK-40K
title: Document running app after SQL Server schema apply
status: done
type: platform
team: docs
supporting_teams:
  - backend
  - database
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Document how to run the application against SQL Server after the schema has been manually applied.

## Context
The app should remain safe by default. SQL Server app execution is opt-in and should only happen after manual schema application and secret configuration outside the repository.

## Implementation steps

1. Update the SQL Server runbook/checklist docs with app-run steps after schema apply.
2. Document how to set the explicit SQL Server provider.
3. Document how to set the connection string safely outside the repository.
4. Document how to run `VoidEmpires.Web`.
5. Document the expected health endpoint provider.
6. Document expected behavior before and after catalog seed.
7. Document how to clear any temporary environment variables after testing.
8. Do not run the real app automatically in tests.

## Files to read first

- ai/architecture-index.md
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/sql-server-test-strategy.md
- src/VoidEmpires.Web/

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-user-checklist.md

## Acceptance criteria

- The docs explain explicit SQL Server provider setup and safe connection-string setup.
- The docs explain how to start `VoidEmpires.Web` and verify the health endpoint provider.
- The docs explain expected pre-catalog-seed and post-catalog-seed behavior.
- The docs explain cleanup of temporary environment variables.
- No secrets or full real connection strings are committed.

## Constraints

- Do not make normal tests require SQL Server.
- Do not run against the real SQL Server during task validation.
- Do not change runtime defaults.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat` and verify the task stays within the change budget.
2. Run `git diff --name-only` and compare modified files with the expected files above.
3. Stage the intended files.
4. Commit with a clear message.
5. Push the branch if it has an upstream and this repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.

