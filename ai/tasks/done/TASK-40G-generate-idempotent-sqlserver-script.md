# TASK-40G

---
id: TASK-40G
title: Generate idempotent SQL Server baseline script
status: done
type: platform
team: database
supporting_teams:
  - devops
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Generate the idempotent SQL Server migration script for manual review without executing it.

## Context
Once the baseline migration and helper are ready, the next artifact is an idempotent SQL script that a human can inspect in SSMS before any schema changes happen.

## Implementation steps

1. Run `scripts/sqlserver-script-migration.ps1`.
2. Generate output into a safe folder such as `artifacts/sqlserver/VoidEmpires_Dev_SqlServerInitialBaseline.sql`, unless repository policy prefers documenting the generated path without committing the SQL.
3. Prefer not to commit a large generated SQL script if project policy or artifact size makes that safer.
4. If committing the SQL script is acceptable, verify it contains no secrets, is idempotent, and is marked reviewed/manual.
5. If not committing the SQL script, document the exact generation command and expected local output path.
6. Do not execute the SQL script.

## Files to read first

- ai/architecture-index.md
- scripts/sqlserver-script-migration.ps1
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-review.md
- src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/

## Expected files to modify

- artifacts/sqlserver/
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sqlserver-generated-script-not-committed.md

## Acceptance criteria

- The script generation command succeeds, or a clear blocker is documented with a follow-up task.
- The generated SQL is idempotent and review-only.
- No generated or documented artifact includes real credentials or full real connection strings.
- The final task outcome states whether the SQL file was committed or only generated locally.

## Constraints

- Do not execute generated SQL.
- Do not connect to the real SQL Server.
- Do not apply migrations automatically.
- Keep generated artifacts out of the commit if they are too large or against repository policy.

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

