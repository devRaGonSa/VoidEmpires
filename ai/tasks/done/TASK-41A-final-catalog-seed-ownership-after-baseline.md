# TASK-41A

---
id: TASK-41A
title: Audit final catalog seed ownership after SQL Server baseline
status: done
type: platform
team: database
supporting_teams:
  - platform
  - qa
roadmap_item: "Final relational catalog seed readiness"
priority: high
---

## Goal
Audit the final catalog seed path now that the SQL Server baseline migration and review script exist, and decide the next safe repository step before any real catalog apply is enabled.

## Context
Block 40 closed with an accepted isolated SQL Server baseline and a guarded idempotent script, but final relational catalog apply is still deferred. The repository has catalog source files, a dry-run-first seed helper, and documentation that warns against treating Development seed profiles as production catalog initialization. This task should narrow the remaining blocker without applying data to SQL Server.

## Implementation steps

1. Inspect the current final catalog seed service, catalog source files, and SQL Server seed helper.
2. Compare the current implementation with the SQL Server runbook and final database readiness report.
3. Document whether final relational catalog ownership is ready, blocked by schema/model gaps, or needs a separate implementation task.
4. Keep the result audit/documentation-focused unless a narrowly scoped guard or wording fix is required.
5. Do not run a real SQL Server connection, seed apply, migration apply, or `dotnet ef database update`.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/sql-server-runbook.md
- docs/dev/final-db-phase-readiness-report.md
- scripts/sqlserver-final-catalog-seed.ps1
- src/VoidEmpires.Infrastructure/SeedData/

## Expected files to modify

- docs/dev/sql-server-runbook.md
- docs/dev/final-db-phase-readiness-report.md
- ai/current-state.md

## Acceptance criteria

- The current final catalog seed readiness is documented without overclaiming apply support.
- Any blocker to enabling real relational catalog apply is stated explicitly.
- Development seed profiles remain separate from final catalog seed guidance.
- No real credentials, resolved connection strings, or SQL Server passwords are added.
- No migration or seed apply is performed.

## Constraints

- Do not run `dotnet ef database update`.
- Do not run generated SQL.
- Do not run `scripts/sqlserver-final-catalog-seed.ps1` with `-Apply`.
- Do not connect to the real SQL Server.
- Keep PostgreSQL as the checked-in default provider.

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
