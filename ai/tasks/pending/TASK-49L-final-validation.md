# TASK-49L

---
id: TASK-49L
title: Final validation
status: pending
type: validation
team: platform
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Close Block 49.

## Context
Update current state, move tasks to done, validate, commit, and push.

## Implementation steps

1. Update ai/current-state.md.
2. Move tasks to done.
3. Run final validation suite.
4. Commit and push the branch.

## Files to read first

- ai/current-state.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/current-state.md
- ai/tasks/pending/TASK-49L-final-validation.md

## Acceptance criteria

- Pending contains only `.gitkeep`.
- Final validation passes.
- Branch is pushed.

## Constraints

- No browser/manual QA claimed.
- No secrets.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`

## Commit and push

At the end, stage, commit, and push.

## Change Budget

- Prefer modifying fewer than 5 files.
