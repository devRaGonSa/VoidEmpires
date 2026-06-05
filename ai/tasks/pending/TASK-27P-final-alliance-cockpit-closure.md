# TASK-27P Final Alliance Cockpit Closure

---
id: TASK-27P
title: Close Alliance cockpit v1 block with validation and final state
status: pending
type: platform
team: platform
supported_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Close Block 27A-27P with formal validation, task movement, and final recorded status.

## Current problem
Alliance v1 has multiple implementation and documentation steps. The block must end with explicit closure gates and explicit anti-regression confirmation.

## Context from current implementation
Frontend lazy loading is already active. Closure needs to preserve that architecture and the no-alliance-action constraints.

## Goal
Complete the block with:
- all tasks in done
- pending queue cleaned
- final validation pass
- explicit closure summary

## Files to inspect first
- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/alliance-cockpit-checklist.md

## Expected files to modify
- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Move TASK-27A through TASK-27P into ai/tasks/done.
- Keep pending with only .gitkeep.
- Update closure docs/state as needed.
- Run final validation commands:
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
- Record closure summary with:
- validation results
- test count
- bundle/lazy status
- visual QA status

## UI/UX requirements
- No new visual QA required for closure beyond documented checks.

## Backend/API requirements
- No new backend scope unless blocker.

## Safety constraints
- Do not claim gameplay actions shipped unless evidence shows them.
- Avoid overclaiming bundle or test results.

## Acceptance criteria
- /alliance read-only cockpit exists and is route-lazy-loaded.
- Existing accepted cockpits still route.
- Validation and lazy guard are green.
- pending queue is effectively empty.
- Working tree clean after final commit.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If blocker appears, pause and create at most 3 follow-up tasks.
- Visual QA can remain user-driven unless explicitly executed.

## Change Budget
- Prefer modifying fewer than 5 files per task
- Prefer changes under 200 lines per task
- Prefer fewer than 3 commits per task
