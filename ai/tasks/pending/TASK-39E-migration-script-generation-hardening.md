# TASK-39E

---
id: TASK-39E
title: Harden SQL Server migration script generation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Harden `scripts/sqlserver-script-migration.ps1` so it safely generates an idempotent SQL script for manual review without connecting to or mutating a real database.

## Context

The first real database flow must separate script generation from manual execution. The helper should never apply migrations or require a password.

## Implementation steps

1. Read the migration helper script and SQL Server migration strategy docs.
2. Ensure the script generates an idempotent `.sql` output file.
3. Ensure the script never calls `database update` or otherwise applies migrations.
4. Ensure the script does not require a password or real connection string.
5. Ensure the script clearly fails if the expected baseline migration does not exist.
6. Print:
   - output path;
   - next manual SSMS review step;
   - warning that the generated script may alter schema when manually executed.
7. Keep generated one-off SQL output out of source control unless an intentional template already exists.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- scripts/sqlserver-script-migration.ps1
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-dry-run.md
- scripts/check-dev-qa-scripts.ps1

## Expected files to modify

- scripts/sqlserver-script-migration.ps1
- docs/dev/sql-server-migration-strategy.md
- docs/dev/sql-server-migration-dry-run.md

## Acceptance criteria

- The helper generates an idempotent SQL script only.
- The helper does not connect to the real SQL Server.
- Missing baseline migration produces a clear failure.
- The console output explains manual review and schema-change risk.

## Constraints

- Do not apply migrations.
- Do not require or print a password.
- Do not commit generated one-off SQL output.
- Keep user-facing docs Spanish-first.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `git diff --stat`
- `git diff --name-only`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote if the branch is configured for remote collaboration.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer fewer than 3 commits.
