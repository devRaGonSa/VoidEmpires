# TASK-43Z-final-validation-and-closure

---
id: TASK-43Z
title: Final validation and closure
status: pending
type: release
team: platform
supporting_teams: [frontend, security]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Close Block 43 after all TASK-43 tasks are implemented and validated.

## Context
This is the final closure task. It should move all `TASK-43*` files to `ai/tasks/done`, ensure pending contains only `.gitkeep`, update state/docs, commit and push final state, and report validation results.

## Implementation steps

1. Confirm no prior `TASK-43*` task remains pending or in progress except this closure task.
2. Run the final validation commands below.
3. Update `ai/current-state.md` with OGame-like layout rework status, public auth layout split, Inicio/Planeta overview decision, top resource bar status, module catalog status, and no manual/browser QA claim.
4. Update the visual QA checklist with the new acceptance criteria.
5. Move all completed `TASK-43*` files to `ai/tasks/done`.
6. Ensure `ai/tasks/pending` contains only `.gitkeep`.
7. Commit and push final state.

## Files to read first

- ai/tasks/in-progress/TASK-43Z-final-validation-and-closure.md
- ai/current-state.md
- docs/dev/deferred-visual-qa-master-checklist.md
- docs/dev/product-readiness-report.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- scripts/check-repo-secret-scan.ps1

## Expected files to modify

- ai/current-state.md
- docs/dev/deferred-visual-qa-master-checklist.md
- ai/tasks/done/TASK-43A-visual-review-contract.md
- ai/tasks/done/TASK-43B-public-auth-layout-split.md
- ai/tasks/done/TASK-43C-authenticated-game-shell-resource-bar.md
- ai/tasks/done/TASK-43D-selected-planet-context-source.md
- ai/tasks/done/TASK-43E-home-becomes-planet-overview.md
- ai/tasks/done/TASK-43F-planet-route-alias-or-merge.md
- ai/tasks/done/TASK-43G-remove-module-context-strips.md
- ai/tasks/done/TASK-43H-remove-duplicated-navigation-cards.md
- ai/tasks/done/TASK-43I-remove-cabina-terminology.md
- ai/tasks/done/TASK-43J-construction-page-action-catalog.md
- ai/tasks/done/TASK-43K-construction-grid-four-columns.md
- ai/tasks/done/TASK-43L-construction-card-information-hierarchy.md
- ai/tasks/done/TASK-43M-research-page-action-catalog.md
- ai/tasks/done/TASK-43N-research-grid-four-columns.md
- ai/tasks/done/TASK-43O-research-card-information-hierarchy.md
- ai/tasks/done/TASK-43P-shipyard-page-action-catalog.md
- ai/tasks/done/TASK-43Q-shipyard-grid-four-columns.md
- ai/tasks/done/TASK-43R-shipyard-card-information-hierarchy.md
- ai/tasks/done/TASK-43S-defense-page-production-catalog.md
- ai/tasks/done/TASK-43T-ground-army-production-catalog.md
- ai/tasks/done/TASK-43U-queue-summary-components.md
- ai/tasks/done/TASK-43V-resource-capacity-backend-viewmodel.md
- ai/tasks/done/TASK-43W-resource-bar-responsive-polish.md
- ai/tasks/done/TASK-43X-auth-required-state-cleanup.md
- ai/tasks/done/TASK-43Y-copy-guard-ogame-like-ui.md
- ai/tasks/done/TASK-43Z-final-validation-and-closure.md

## Acceptance criteria

- Login/register are standalone public pages.
- Authenticated game shell has top resource bar.
- Inicio is the main planet overview.
- Planeta is merged, aliased, or redirected consistently.
- Construction is a building catalog.
- Research is a compact technology grid.
- Shipyard is a compact ship production grid.
- Defense is a defense catalog.
- Ground Army is a land unit training/catalog page.
- No `cabina`, context, or duplicated navigation clutter remains in normal UI.
- All final validation commands pass.
- `ai/tasks/pending` contains only `.gitkeep`.

## Constraints

- Do not close the block if validation fails without a follow-up task.
- Do not claim manual/browser QA unless it was actually performed.
- Do not add combat, fleet movement, market transactions, alliance mutations, or active espionage execution.

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

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files for any fixes discovered during closure.
- Prefer changes under 200 lines of code for any fixes discovered during closure.
- Split the work into additional tasks if limits are exceeded.
