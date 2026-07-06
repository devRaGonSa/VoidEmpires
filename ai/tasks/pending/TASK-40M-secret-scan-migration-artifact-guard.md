# TASK-40M

---
id: TASK-40M
title: Guard migration artifacts against committed secrets
status: pending
type: platform
team: security
supporting_teams:
  - devops
  - database
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Ensure generated migration files, generated SQL scripts, and SQL Server documentation cannot leak credentials.

## Context
The repository already has a lightweight secret scan. SQL Server migration work introduces new artifact locations that should be covered if they are committed or documented.

## Implementation steps

1. Review `scripts/check-repo-secret-scan.ps1`.
2. Extend the scan if needed to cover migration folders, `artifacts/sqlserver`, generated SQL scripts, and docs containing connection examples.
3. Allow safe placeholder values already used by the repository.
4. Fail on real-looking connection-string password assignments and hardcoded server/user/password combinations.
5. Ensure current repository content passes.
6. Keep false positives low and document any intentional exclusions.

## Files to read first

- ai/architecture-index.md
- scripts/check-repo-secret-scan.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/sql-server-runbook.md
- docs/dev/sql-server-migration-strategy.md

## Expected files to modify

- scripts/check-repo-secret-scan.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/final-db-security-notes.md

## Acceptance criteria

- Secret scan coverage includes SQL Server migration/script artifact locations that can be committed.
- Safe placeholders are allowed.
- Real-looking password assignments and hardcoded SQL Server credentials fail the scan.
- The current repository passes the scan and QA script checks.

## Constraints

- Do not print or commit secrets.
- Do not make the scan require network or database access.
- Do not weaken existing checks.

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
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

