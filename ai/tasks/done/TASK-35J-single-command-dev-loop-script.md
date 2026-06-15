# TASK-35J

---
id: TASK-35J
title: Single command dev loop script
status: pending
type: tooling
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: medium
---

## Goal
Add a Development QA orchestration helper that prints the correct sequence for the playable loop.

## Context
Developers need one safe guide command that explains the backend, frontend, playable session, materialization, and diagnostics flow without hiding mutating steps.

## Implementation steps

1. Add `scripts/dev-qa-playable-loop-guide.ps1`.
2. By default, print a clear ordered guide:
   - start backend command;
   - create/playable session helper command;
   - frontend dev command;
   - onboarding/planet URL guidance;
   - materialize due queues command;
   - diagnostics command.
3. Do not execute every mutating step automatically by default.
4. Optional flags may run safe read-only diagnostics or prepare state, but must be explicitly documented.
5. Do not auto-enqueue gameplay unless explicitly documented and safe.
6. Add `check-dev-qa-scripts.ps1` coverage.
7. Use UTF-8 safe output.

## Files to read first

- scripts/dev-qa-prepare-playable-session-state.ps1
- scripts/dev-qa-materialize-due-queues.ps1
- scripts/dev-qa-get-playable-session-diagnostics.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- scripts/dev-qa-playable-loop-guide.ps1
- scripts/check-dev-qa-scripts.ps1
- Optional: docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Developer has a single guide command.
- Script parses.
- Default behavior does not introduce hidden mutations.
- Output is UTF-8 safe and Spanish-first.

## Constraints

- Do not perform browser/visual QA.
- Do not auto-enqueue or auto-materialize by default.
- Do not add production behavior.

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
4. Commit with a clear TASK-35J message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
