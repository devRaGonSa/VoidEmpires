# TASK-40H

---
id: TASK-40H
title: Add static safety review for SQL Server scripts
status: done
type: platform
team: security
supporting_teams:
  - database
  - devops
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Add a static safety review path for generated SQL Server scripts.

## Context
Generated migration SQL must be manually reviewed before execution. A lightweight static guard can catch obvious unsafe statements and credential leaks before a human opens the script in SSMS.

## Implementation steps

1. Decide whether to add a PowerShell guard script or a documented checklist based on stability and false-positive risk.
2. Detect obvious unsafe SQL such as `DROP DATABASE`, `DROP LOGIN`, `DROP USER`, `TRUNCATE TABLE`, password literals, hardcoded server values, hardcoded usernames, and hardcoded passwords.
3. Allow normal EF migration table operations when expected, including `__EFMigrationsHistory` operations.
4. If adding a script, wire it into `check-dev-qa-scripts.ps1` only when stable and low false-positive.
5. Document manual review requirements and what the static guard does not prove.
6. Do not execute any SQL script.

## Files to read first

- ai/architecture-index.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-repo-secret-scan.ps1
- scripts/sqlserver-script-migration.ps1
- docs/dev/sql-server-migration-review.md

## Expected files to modify

- scripts/check-sqlserver-generated-script-safety.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/sql-server-migration-review.md

## Acceptance criteria

- Static SQL safety review exists as either a script or documented checklist.
- The review catches obvious destructive statements and credential leaks.
- Normal EF migration-history operations are not incorrectly blocked.
- The manual review requirement remains explicit.
- Existing QA scripts remain stable.

## Constraints

- Do not weaken existing secret scan behavior.
- Do not run generated SQL.
- Do not add broad scanning that creates noisy false positives across unrelated files.

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

