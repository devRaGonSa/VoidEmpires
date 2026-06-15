# TASK-35F

---
id: TASK-35F
title: Dev diagnostics PowerShell helper
status: pending
type: tooling
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Add a PowerShell helper for fetching playable session diagnostics.

## Context
Developers need a copy-safe, UTF-8-safe command to inspect the consolidated diagnostics endpoint without mutating the database.

## Implementation steps

1. Add `scripts/dev-qa-get-playable-session-diagnostics.ps1`.
2. Default backend base URL to `http://localhost:5142`.
3. Accept `BaseUrl`, `CivilizationId`, and `PlanetId`.
4. Print a concise diagnostics summary.
5. Print raw JSON only with an explicit switch such as `-RawJson`.
6. Fail cleanly if the backend is unavailable or returns non-success.
7. Use UTF-8 safe output initialization.
8. Add parser/check coverage in `scripts/check-dev-qa-scripts.ps1`.
9. Do not mutate the database.

## Files to read first

- scripts/dev-qa-materialize-due-queues.ps1
- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/check-dev-qa-scripts.ps1
- src/VoidEmpires.Web/DevEndpointMappings.cs

## Expected files to modify

- scripts/dev-qa-get-playable-session-diagnostics.ps1
- scripts/check-dev-qa-scripts.ps1
- Optional: docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- Diagnostics helper exists and parses.
- Helper is read-only.
- Raw JSON is hidden unless explicitly requested.
- `check-dev-qa-scripts.ps1` passes.
- Backend tests pass if endpoint contracts are touched.

## Constraints

- Do not mutate Development database state.
- Do not perform browser/visual QA.
- Keep output Spanish-first and UTF-8 safe.

## Validation

Before completing the task run:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35F message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
