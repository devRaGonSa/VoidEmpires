# TASK-39O

---
id: TASK-39O
title: Run final validation gate for SQL Server control block
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "SQL Server final database readiness"
priority: high
---

## Goal

Run the full final validation gate for Block 39A-39P and record the results.

## Context

Before closing the block, the repository must prove that build, tests, frontend build, QA scripts, and guard scripts still pass without requiring a real SQL Server.

## Implementation steps

1. Run the full validation command list in order.
2. Record the `dotnet test` count.
3. Record that no real SQL Server migration was applied automatically.
4. Record that no real SQL Server password was committed.
5. Update the current state or final DB readiness docs with validation results.
6. If validation fails, stop and document the failure before marking the task done.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/final-db-phase-readiness-report.md
- docs/dev/sql-server-runbook.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/current-state.md
- docs/dev/final-db-phase-readiness-report.md

## Acceptance criteria

- All required validation commands pass.
- Test count is recorded.
- The docs/current state record no real DB apply.
- `git status` is reviewed before completion.

## Constraints

- Do not connect to or mutate a real SQL Server.
- Do not apply migrations.
- Do not commit generated build artifacts.
- Do not hide validation failures.

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
