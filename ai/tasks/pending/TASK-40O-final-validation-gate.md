# TASK-40O

---
id: TASK-40O
title: Run final validation gate for SQL Server baseline block
status: pending
type: platform
team: qa
supporting_teams:
  - database
  - frontend
  - devops
roadmap_item: "SQL Server initial baseline migration"
priority: high
---

## Goal
Run the final validation gate for the SQL Server initial baseline migration block.

## Context
After migration generation, review docs, script helper hardening, secret-scan coverage, and runbook updates are complete, the block needs one complete validation pass.

## Implementation steps

1. Run all commands listed in the validation section.
2. Record the `dotnet test --no-build` test count.
3. Record that no automatic SQL Server apply occurred.
4. If any command fails, fix within task budget or create a narrowed follow-up task.
5. Do not connect to the real SQL Server unless a documented optional smoke command is explicitly requested outside normal validation.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/sql-server-runbook.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/current-state.md
- docs/dev/final-db-phase-readiness-report.md

## Acceptance criteria

- Final validation commands pass.
- Test count is recorded.
- No automatic SQL Server apply is recorded.
- Any remaining blocker is documented honestly with a follow-up task.

## Constraints

- Do not run `dotnet ef database update`.
- Do not execute generated SQL.
- Do not require SQL Server for normal tests.
- Do not commit build artifacts.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- `git status`

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

