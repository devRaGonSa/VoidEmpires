# TASK-40L

---
id: TASK-40L
title: Plan catalog seed after SQL Server schema apply
status: done
type: platform
team: database
supporting_teams:
  - docs
  - devops
roadmap_item: "SQL Server initial baseline migration"
priority: medium
---

## Goal
Prepare the next step after manual schema apply: catalog seeding.

## Context
Catalog seed work must remain controlled and must not write to the real SQL Server during normal validation. This task documents the post-schema sequence and the current limits of the seed script.

## Implementation steps

1. Review `scripts/sqlserver-final-catalog-seed.ps1`.
2. Document the exact sequence after schema exists: dry-run, review output, and apply only after explicit confirmation if a future apply mode is implemented.
3. If apply is still deferred, state that clearly.
4. Do not implement destructive seed apply.
5. Do not write to the real database.
6. Keep catalog seed docs aligned with the manual SSMS schema apply runbook.

## Files to read first

- ai/architecture-index.md
- scripts/sqlserver-final-catalog-seed.ps1
- docs/dev/sql-server-runbook.md
- docs/dev/seed-data-architecture.md
- docs/dev/final-db-phase-readiness-report.md

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/seed-data-architecture.md

## Acceptance criteria

- The post-schema catalog seed sequence is documented.
- Dry-run and review steps are explicit.
- Any apply step is either clearly deferred or gated behind future explicit confirmation.
- No real database write is performed.
- Existing QA script parser checks pass.

## Constraints

- Do not add destructive seed behavior.
- Do not connect to the real SQL Server.
- Do not commit credentials.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
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

