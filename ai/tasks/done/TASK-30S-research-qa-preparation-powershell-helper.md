# TASK-30S

---
id: TASK-30S
title: Research QA preparation PowerShell helper
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 30Q-30T - Research manual QA state preparation"
priority: medium
---

## Goal
Add a PowerShell helper that calls the Development-only research QA preparation endpoint and documents the repeated manual QA command.

## Context
The repository already uses backend-only PowerShell helpers for safe Development QA flows. Research needs a dedicated preparation step for reused Development databases before repeating manual enqueue success-path QA in `/research`.

## Implementation steps

1. Add a focused `dev-qa-prepare-research-ui-state.ps1` helper with clear Development-mutation warnings and scoped defaults.
2. Extend the local script parser-check coverage to include the new helper.
3. Update the research and persisted-flow docs with the preparation command and expected usage.

## Files to read first

- scripts/dev-qa-create-research-order.ps1
- scripts/dev-qa-baseline.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/research-cockpit-checklist.md

## Expected files to modify

- ai/tasks/in-progress/TASK-30S-research-qa-preparation-powershell-helper.md
- scripts/dev-qa-prepare-research-ui-state.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/research-cockpit-checklist.md

## Acceptance criteria

- The helper defaults to `http://localhost:5142` and the seeded civilization and planet.
- The helper warns clearly that it mutates the Development database for manual Research QA.
- The helper prints a concise summary and fails cleanly when the backend is unavailable.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` passes.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
