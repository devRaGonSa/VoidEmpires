# TASK-36P

---
id: TASK-36P
title: Final UI decluttering closure
status: done
type: platform
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Close Block 36 after all prior tasks have been implemented, validated, committed, and pushed.

## Context
This closure task moves all Block 36 task files to done, confirms pending is empty except `.gitkeep`, and records final output for UI information architecture cleanup.

## Implementation steps

1. Confirm TASK-36A through TASK-36O are complete and committed.
2. Move TASK-36A through TASK-36P from `ai/tasks/pending` or `ai/tasks/in-progress` to `ai/tasks/done` as appropriate.
3. Confirm `ai/tasks/pending` contains only `.gitkeep`.
4. Run final validation if TASK-36O results are stale.
5. Commit and push final state.
6. Prepare final output with:
   - commit list;
   - validation results;
   - test count;
   - explicit list of pages decluttered;
   - explicit note whether full browser QA was or was not performed.

## Files to read first

- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/deferred-visual-qa-master-checklist.md

## Expected files to modify

- ai/tasks/done/TASK-36A-ui-information-architecture-audit.md
- ai/tasks/done/TASK-36B-app-shell-header-reality-alignment.md
- ai/tasks/done/TASK-36C-sidebar-status-and-navigation-declutter.md
- ai/tasks/done/TASK-36D-page-hero-copy-modernization.md
- ai/tasks/done/TASK-36E-shared-page-header-and-context-strip.md
- ai/tasks/done/TASK-36F-development-tools-panel-foundation.md
- ai/tasks/done/TASK-36G-development-actions-modal-confirmation.md
- ai/tasks/done/TASK-36H-planet-hub-decluttering.md
- ai/tasks/done/TASK-36I-construction-page-decluttering.md
- ai/tasks/done/TASK-36J-research-page-decluttering.md
- ai/tasks/done/TASK-36K-shipyard-page-decluttering.md
- ai/tasks/done/TASK-36L-readiness-pages-decluttering.md
- ai/tasks/done/TASK-36M-resource-and-terminology-coherence.md
- ai/tasks/done/TASK-36N-visual-qa-checklist-update-from-findings.md
- ai/tasks/done/TASK-36O-current-state-and-final-validation.md
- ai/tasks/done/TASK-36P-final-ui-decluttering-closure.md

## Acceptance criteria

- TASK-36A through TASK-36P are in `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final expected state is true:
  - UI has less repeated information.
  - Header no longer contradicts real session/economy.
  - Sidebar reflects playable vs read-only pages.
  - Obsolete prototype copy is removed or demoted.
  - Planet is a clearer hub.
  - Construction catalog is easier to reach.
  - Research and Shipyard are less noisy.
  - Development/QA tools are collapsed/secondary.
  - Development mutating actions use clear confirmation.
  - Diagnostics remain available but secondary/collapsed.
  - Resource terminology is more coherent.
  - Gameplay semantics unchanged.
  - No combat, movement, or missions.
  - Build/test/frontend/scripts pass.

## Constraints

- Do not add gameplay scope during closure.
- Do not leave Block 36 task files in pending.
- Be explicit whether full browser QA was or was not performed.

## Validation

Before completing the task ensure recent green results for:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and confirm only intended closure files changed.
3. Stage only intended files.
4. Commit with a clear TASK-36P closure message.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for this closure task.
- Do not add implementation scope in the closure commit.
