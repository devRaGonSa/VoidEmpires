# TASK-33P

---
id: TASK-33P
title: Final playable loop closure
status: pending
type: platform
team: platform
supporting_teams: [frontend, gameplay]
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Close Block 33 after all prior tasks have been implemented, validated, committed, and pushed.

## Context
This is the closure task for the playable loop integration phase. It moves all Block 33 task files to done, confirms pending is empty except `.gitkeep`, and records final output.

## Implementation steps

1. Confirm TASK-33A through TASK-33O are complete and committed.
2. Move TASK-33A through TASK-33P from `ai/tasks/pending` or `ai/tasks/in-progress` to `ai/tasks/done` as appropriate.
3. Confirm `ai/tasks/pending` contains only `.gitkeep`.
4. Run the final validation set if TASK-33O results are stale.
5. Commit and push the final state.
6. Prepare final output with:
   - commit list;
   - validation results;
   - test count;
   - exact helper command:
     `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-playable-session-state.ps1 -ElapsedSeconds 3600`
   - explicit note that visual QA remains deferred.

## Files to read first

- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- ai/tasks/done/TASK-33A-playable-loop-session-contract-audit.md
- ai/tasks/done/TASK-33B-playable-session-storage-helper.md
- ai/tasks/done/TASK-33C-onboarding-save-session-and-navigate.md
- ai/tasks/done/TASK-33D-session-aware-route-entry-fallback.md
- ai/tasks/done/TASK-33E-session-banner-component.md
- ai/tasks/done/TASK-33F-planet-session-banner-and-hub-links.md
- ai/tasks/done/TASK-33G-construction-session-context-integration.md
- ai/tasks/done/TASK-33H-research-session-context-integration.md
- ai/tasks/done/TASK-33I-shipyard-session-context-integration.md
- ai/tasks/done/TASK-33J-readiness-pages-session-context.md
- ai/tasks/done/TASK-33K-resource-summary-consistency.md
- ai/tasks/done/TASK-33L-playable-session-qa-helper-docs.md
- ai/tasks/done/TASK-33M-cross-flow-guards-and-navigation-tests.md
- ai/tasks/done/TASK-33N-runtime-checklist-for-deferred-visual-qa.md
- ai/tasks/done/TASK-33O-current-state-and-final-validation.md
- ai/tasks/done/TASK-33P-final-playable-loop-closure.md

## Acceptance criteria

- TASK-33A through TASK-33P are in `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final expected state is true:
  - `/onboarding` is a natural Development-safe entry point;
  - latest playable session can be remembered locally;
  - localStorage stores only non-sensitive navigation context;
  - Planet acts as hub;
  - Construction, Research, and Shipyard are session-aware;
  - Defenses and Fleets are session-aware but remain read-only;
  - resource display is more consistent;
  - no production auth overclaim;
  - no combat, movement, or missions;
  - no visual QA performed.
- Build/test/frontend/scripts pass.

## Constraints

- Do not claim browser/visual QA.
- Do not add new gameplay scope during closure.
- Do not leave Block 33 task files in pending.

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
4. Commit with a clear TASK-33P closure message.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for this closure task.
- Do not add implementation scope in the closure commit.
