# TASK-41B

---
id: TASK-41B
title: Document disposable SQL Server baseline replay validation
status: done
type: platform
team: qa
supporting_teams:
  - database
  - devops
roadmap_item: "SQL Server baseline replay validation"
priority: high
---

## Goal
Document the disposable SQL Server replay validation gate needed before treating the accepted baseline script as schema-replay ready.

## Context
The repository has an accepted SQL Server baseline migration and idempotent SQL review script, but current validation remains provider-independent plus an opt-in connection smoke. A future operator or CI-style workflow needs a safe disposable replay checklist that does not target the user-managed real database and does not hide migration apply inside app startup or tests.

## Implementation steps

1. Inspect the current SQL Server runbook, migration strategy, generated script safety guard, and readiness report.
2. Document a disposable replay validation plan that uses only a disposable target and explicit operator confirmation.
3. Keep the plan separate from the real `VoidEmpires_Dev` manual apply path.
4. Record that the plan is not yet automated and was not executed in this task.
5. Do not run SQL Server, execute generated SQL, or run `dotnet ef database update`.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-migration-strategy.md
- docs/dev/final-db-phase-readiness-report.md
- scripts/check-sqlserver-generated-script-safety.ps1

## Expected files to modify

- docs/dev/sql-server-disposable-replay-validation.md
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-phase-readiness-report.md
- ai/current-state.md

## Acceptance criteria

- A disposable replay validation document exists with clear preconditions, commands/checks to review, and stop conditions.
- The document explicitly avoids the real user-managed database and avoids automatic app/test migration apply.
- Existing SQL Server runbook/readiness docs link or reference the disposable replay gate.
- No real SQL Server credentials, resolved connection strings, or passwords are added.
- No SQL script or migration is executed.

## Constraints

- Do not run `dotnet ef database update`.
- Do not execute generated SQL.
- Do not connect to SQL Server.
- Do not change runtime provider defaults.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`

## Commit and push

At the end:

1. Run `git diff --stat` and verify the task stays within the change budget.
2. Run `git diff --name-only` and compare modified files with the expected files above.
3. Stage the intended files.
4. Commit with a clear message.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.
