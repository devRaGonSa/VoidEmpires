# TASK-46S

---
id: TASK-46S
title: Final validation and closure
status: pending
type: release
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Close Block 46.

## Context
All TASK-46 work should be completed, moved to done, validated, committed, and pushed.

## Implementation steps

1. Move all TASK-46 files to ai/tasks/done.
2. Confirm ai/tasks/pending contains only .gitkeep.
3. Run final validation commands.
4. Commit and push final state.

## Files to read first

- ai/tasks/pending
- ai/tasks/done
- ai/current-state.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/tasks/pending/TASK-46A-ogame-module-polish-contract.md
- ai/tasks/pending/TASK-46B-remove-continuation-panels-from-modules.md
- ai/tasks/pending/TASK-46C-queues-render-only-when-active.md
- ai/tasks/pending/TASK-46D-remove-current-infrastructure-dashboard.md
- ai/tasks/pending/TASK-46E-remove-research-completed-dashboard.md
- ai/tasks/pending/TASK-46F-remove-module-version-and-status-pills.md
- ai/tasks/pending/TASK-46G-single-building-catalog-container.md
- ai/tasks/pending/TASK-46H-building-availability-and-starting-playability.md
- ai/tasks/pending/TASK-46I-single-research-catalog-container.md
- ai/tasks/pending/TASK-46J-single-shipyard-catalog-container.md
- ai/tasks/pending/TASK-46K-shipyard-quantity-production-form.md
- ai/tasks/pending/TASK-46L-defense-unit-vs-level-model.md
- ai/tasks/pending/TASK-46M-single-defense-catalog-container.md
- ai/tasks/pending/TASK-46N-defense-production-form.md
- ai/tasks/pending/TASK-46O-inline-blocked-reasons-no-review-modal.md
- ai/tasks/pending/TASK-46P-card-action-copy-polish.md
- ai/tasks/pending/TASK-46Q-copy-guard-module-polish.md
- ai/tasks/pending/TASK-46R-docs-current-state-update.md
- ai/tasks/pending/TASK-46S-final-validation-and-closure.md

## Acceptance criteria

- All TASK-46 files are in ai/tasks/done.
- ai/tasks/pending contains only .gitkeep.
- Construction has no continuation panel, no separate current infrastructure block, queue only if active, and one building catalog grid.
- Research has no completed technologies block, no progress dashboard, queue only if active, and one research catalog grid.
- Shipyard has no Revisar bloqueo, no block-review modal, one ship catalog grid, quantity production form where available, and inline blocked reasons.
- Defenses has unit-based quantity display, special defense level only if intentional, one defense catalog grid, quantity production form where available, and inline blocked reasons.
- Sidebar and top resource bar remain.
- Login/Register remain public without sidebar.
- Build/test/frontend/scripts pass.

## Constraints

- No manual/browser QA claim.
- Do not discard existing work.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-repo-secret-scan.ps1`
- `git status`
- `dir ai/tasks/pending`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

