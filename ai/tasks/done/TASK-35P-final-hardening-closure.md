# TASK-35P

---
id: TASK-35P
title: Final hardening closure
status: pending
type: platform
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Close Block 35 after all prior tasks have been implemented, validated, committed, and pushed.

## Context
This closure task moves all Block 35 task files to done, confirms pending is empty except `.gitkeep`, and records final output for hardening, diagnostics, and deferred visual QA prep.

## Implementation steps

1. Confirm TASK-35A through TASK-35O are complete and committed.
2. Move TASK-35A through TASK-35P from `ai/tasks/pending` or `ai/tasks/in-progress` to `ai/tasks/done` as appropriate.
3. Confirm `ai/tasks/pending` contains only `.gitkeep`.
4. Run final validation if TASK-35O results are stale.
5. Commit and push the final state.
6. Prepare final output with:
   - commit list;
   - validation results;
   - test count;
   - exact guide command;
   - exact diagnostics command;
   - explicit note that visual QA remains deferred.

## Files to read first

- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- ai/tasks/done/TASK-35A-playable-loop-hardening-audit.md
- ai/tasks/done/TASK-35B-powershell-encoding-hardening.md
- ai/tasks/done/TASK-35C-qa-script-output-consistency.md
- ai/tasks/done/TASK-35D-unified-dev-diagnostics-contract.md
- ai/tasks/done/TASK-35E-dev-playable-session-diagnostics-endpoint.md
- ai/tasks/done/TASK-35F-dev-diagnostics-powershell-helper.md
- ai/tasks/done/TASK-35G-frontend-diagnostics-summary-component.md
- ai/tasks/done/TASK-35H-integrate-diagnostics-planet-and-core-cockpits.md
- ai/tasks/done/TASK-35I-empty-error-loading-state-hardening.md
- ai/tasks/done/TASK-35J-single-command-dev-loop-script.md
- ai/tasks/done/TASK-35K-copy-regression-hardening-spanish.md
- ai/tasks/done/TASK-35L-deferred-visual-qa-master-checklist.md
- ai/tasks/done/TASK-35M-current-scripts-docs-alignment.md
- ai/tasks/done/TASK-35N-regression-test-hardened-loop-no-mutation-on-read.md
- ai/tasks/done/TASK-35O-current-state-and-final-validation.md
- ai/tasks/done/TASK-35P-final-hardening-closure.md

## Acceptance criteria

- TASK-35A through TASK-35P are in `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final expected state is true:
  - QA scripts are UTF-8/copy-paste safer.
  - Diagnostics endpoint/helper exists and is read-only.
  - Core pages have collapsed diagnostics where useful.
  - Empty/error/loading states are clearer.
  - Master deferred visual QA checklist exists.
  - Copy regression catches mojibake/placeholders/forbidden cheat wording.
  - Read-only boundaries are tested.
  - No combat, movement, or missions.
  - No visual QA performed.
  - Build/test/frontend/scripts pass.

## Constraints

- Do not claim browser/visual QA.
- Do not add new gameplay scope during closure.
- Do not leave Block 35 task files in pending.

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
4. Commit with a clear TASK-35P closure message.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for this closure task.
- Do not add implementation scope in the closure commit.
