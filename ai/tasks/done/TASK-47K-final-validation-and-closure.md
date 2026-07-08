# TASK-47K

---
id: TASK-47K
title: Final validation and closure for Block 47
status: done
type: validation
team: platform
supporting_teams: [frontend, backend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Close Block 47.

## Context
All Block 47 task files must be moved to done, final current state updated, and the branch committed and pushed after validation.

## Implementation steps

1. Move all TASK-47 files to ai/tasks/done.
2. Confirm ai/tasks/pending contains only .gitkeep.
3. Update ai/current-state.md.
4. Run final validation.
5. Commit and push final state.

## Files to read first

- ai/current-state.md
- ai/tasks/pending
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- ai/current-state.md
- ai/tasks/pending/TASK-47K-final-validation-and-closure.md
- ai/tasks/done/TASK-47K-final-validation-and-closure.md

## Acceptance criteria

- Repeated "Centro imperial" hero is removed from module pages.
- Sidebar and top resource bar remain.
- Basic construction is buildable on valid starting planets when resources/capacity allow.
- Missing building capacity blocker is fixed.
- Shipyard card actions align consistently.
- Unit-based defenses support quantity input and build action.
- Special defenses may remain level-based if explicitly modeled.
- Blocked reasons are inline and useful.
- Build, test, frontend, and guard scripts pass.

## Constraints

- No manual/browser QA claim.
- Do not add combat, fleet movement, market transactions, alliance mutations, or active espionage.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1
- git status
- dir ai/tasks/pending
