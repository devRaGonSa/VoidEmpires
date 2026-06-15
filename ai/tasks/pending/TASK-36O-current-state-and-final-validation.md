# TASK-36O

---
id: TASK-36O
title: Current state and final validation
status: pending
type: platform
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Update current-state documentation and run full validation for Block 36.

## Context
After decluttering is complete, `ai/current-state.md` should accurately describe the UI information architecture cleanup and confirm gameplay semantics remain unchanged.

## Implementation steps

1. Update `ai/current-state.md` with:
   - UI information architecture cleanup status;
   - header/sidebar modernization status;
   - Planet, Construction, Research, and Shipyard decluttering status;
   - DevelopmentToolsPanel status;
   - Development actions modal confirmation status;
   - diagnostics secondary/collapsed status;
   - no gameplay semantics changed;
   - no combat, movement, or missions.
2. Run final validation:
   - `dotnet build --no-restore`;
   - `dotnet test --no-build`;
   - `npm run build --prefix src/VoidEmpires.Frontend`;
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`;
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`;
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
3. Record test count and warnings.
4. Do not claim full visual QA.

## Files to read first

- ai/current-state.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- ai/current-state.md
- Optional: docs/dev/frontend-foundation-smoke-checklist.md
- Optional: docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- `ai/current-state.md` is accurate.
- Full validation is green.
- Test count and warnings are recorded.
- No visual QA overclaim is introduced.

## Constraints

- Do not add gameplay scope during final validation unless fixing a Block 36 regression.
- Do not perform or claim full browser QA.
- If a fix exceeds budget, create a follow-up task.

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
4. Commit with a clear TASK-36O message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
