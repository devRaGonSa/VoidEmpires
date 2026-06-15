# TASK-35O

---
id: TASK-35O
title: Current state and final validation
status: pending
type: platform
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Update current-state documentation and run final Block 35 validation.

## Context
After the hardening tasks complete, `ai/current-state.md` should reflect playable loop maturity, diagnostics, QA helper status, and the continued visual QA deferral.

## Implementation steps

1. Update `ai/current-state.md` with:
   - playable loop technically complete through materialization;
   - hardening/diagnostics status;
   - QA helper scripts status;
   - encoding/copy guard status;
   - diagnostics endpoint/helper status;
   - visual QA remains deferred;
   - no combat, movement, or missions.
2. Run final validation:
   - `dotnet build --no-restore`;
   - `dotnet test --no-build`;
   - `npm run build --prefix src/VoidEmpires.Frontend`;
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`;
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`;
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
3. Record test count and warnings.
4. Do not claim visual QA.

## Files to read first

- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- ai/current-state.md
- Optional: docs/dev/persisted-gameplay-flow-checklist.md
- Optional: docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- `ai/current-state.md` is accurate.
- Full validation is green.
- Test count and warnings are recorded.
- No visual QA overclaim is introduced.

## Constraints

- Do not perform browser/visual QA.
- Do not add new gameplay scope during final validation unless fixing a Block 35 validation failure.
- If a fix exceeds the change budget, create a follow-up task.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35O message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
