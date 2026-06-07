# TASK-29N

---
id: TASK-29N-final-validation-construction-enqueue-ux
title: Final validation pass for construction enqueue UX block
status: pending
type: platform
team: platform
supporting_teams: [platform]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Run required validation command set and capture final status.

## Context
Administrative validation task after all implementation tasks are done.

## Files to read first

- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- ai/tasks/pending/TASK-29N-final-validation-construction-enqueue-ux.md

## Implementation steps

1. Run required dotnet/npm/powershell checks.
2. Confirm pending folder is clean at closure.
3. Record any warnings in task notes.

## Acceptance criteria

- All listed validation commands are run and reported.
- No scope regression is introduced.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
