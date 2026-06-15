# TASK-34P

---
id: TASK-34P
title: Final queue materialization closure
status: pending
type: platform
team: platform
supporting_teams: [gameplay, frontend]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Close Block 34 after all prior tasks have been implemented, validated, committed, and pushed.

## Context
This closure task moves the Block 34 task files to done, confirms pending is empty except `.gitkeep`, and records final output for queue progression/materialization v1.

## Implementation steps

1. Confirm TASK-34A through TASK-34O are complete and committed.
2. Move TASK-34A through TASK-34P from `ai/tasks/pending` or `ai/tasks/in-progress` to `ai/tasks/done` as appropriate.
3. Confirm `ai/tasks/pending` contains only `.gitkeep`.
4. Run final validation if TASK-34O results are stale.
5. Commit and push the final state.
6. Prepare final output with:
   - commit list;
   - validation results;
   - test count;
   - exact materialization helper command;
   - explicit note that visual QA remains deferred.

## Files to read first

- ai/current-state.md
- ai/tasks/pending/
- ai/tasks/in-progress/
- ai/tasks/done/
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- ai/tasks/done/TASK-34A-queue-completion-contract-audit.md
- ai/tasks/done/TASK-34B-shared-queue-materialization-service.md
- ai/tasks/done/TASK-34C-construction-due-completion-materialization.md
- ai/tasks/done/TASK-34D-research-due-completion-materialization.md
- ai/tasks/done/TASK-34E-shipyard-due-completion-materialization.md
- ai/tasks/done/TASK-34F-development-materialization-endpoint.md
- ai/tasks/done/TASK-34G-materialization-qa-helper-script.md
- ai/tasks/done/TASK-34H-frontend-materialization-api-contract.md
- ai/tasks/done/TASK-34I-planet-materialize-and-refresh-action.md
- ai/tasks/done/TASK-34J-cockpit-post-materialization-readiness.md
- ai/tasks/done/TASK-34K-playable-session-helper-integration.md
- ai/tasks/done/TASK-34L-idempotency-and-safety-regression-tests.md
- ai/tasks/done/TASK-34M-cross-cockpit-guardrails.md
- ai/tasks/done/TASK-34N-runtime-checklist-for-deferred-queue-qa.md
- ai/tasks/done/TASK-34O-current-state-and-final-validation.md
- ai/tasks/done/TASK-34P-final-queue-materialization-closure.md

## Acceptance criteria

- TASK-34A through TASK-34P are in `ai/tasks/done`.
- `ai/tasks/pending` contains only `.gitkeep`.
- Final expected state is true:
  - Due construction orders can be materialized into building upgrades.
  - Due research orders can be materialized into research/technology state.
  - Due shipyard orders can be materialized into stock/inventory state.
  - Materialization is idempotent and scoped.
  - Development-only endpoint/helper exists.
  - Planet can trigger or reflect materialization safely if implemented.
  - No production cheating semantics.
  - No combat, movement, or missions.
  - No visual QA performed.
  - Build/test/frontend/scripts pass.

## Constraints

- Do not claim browser/visual QA.
- Do not add new gameplay scope during closure.
- Do not leave Block 34 task files in pending.

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
4. Commit with a clear TASK-34P closure message.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for this closure task.
- Do not add implementation scope in the closure commit.
