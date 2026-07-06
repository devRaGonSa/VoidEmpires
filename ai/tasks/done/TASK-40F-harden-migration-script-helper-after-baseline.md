# TASK-40F

---
id: TASK-40F
title: Harden SQL Server migration script helper after baseline
status: done
type: platform
team: devops
supporting_teams:
  - database
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Update `scripts/sqlserver-script-migration.ps1` so it generates a reviewable idempotent SQL script once `SqlServerInitialBaseline` exists.

## Context
The helper must remain fail-fast when baseline files are missing, but after the baseline exists it should generate SQL without applying it, without connecting to the real database, and without requiring secrets.

## Implementation steps

1. Review the current helper behavior and baseline-file checks.
2. Update the helper so it generates an idempotent SQL Server migration script.
3. Ensure the helper does not apply the script or connect to the real database.
4. Ensure the helper does not require or print any password.
5. Print the output path, migration name, manual SSMS review step, and a warning that manual execution changes schema.
6. Keep fail-fast behavior if `SqlServerInitialBaseline` files are missing.
7. Add or update parser/stability coverage through the existing QA script path.

## Files to read first

- ai/architecture-index.md
- scripts/sqlserver-script-migration.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/sql-server-migration-strategy.md
- src/VoidEmpires.Infrastructure/Persistence/Migrations/SqlServer/

## Expected files to modify

- scripts/sqlserver-script-migration.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/sql-server-migration-strategy.md

## Acceptance criteria

- The helper generates idempotent SQL to a safe local output path.
- The helper clearly states that the script is for manual review and manual execution only.
- The helper does not require real SQL Server credentials or connectivity.
- Missing baseline files still produce a clear failure.
- Existing QA script parsing remains stable.

## Constraints

- Do not run `dotnet ef database update`.
- Do not execute the generated SQL script.
- Do not print secrets.
- Do not weaken existing secret scan or copy guards.

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

