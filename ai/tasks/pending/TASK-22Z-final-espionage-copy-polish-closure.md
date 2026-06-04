# TASK-22Z

---
id: TASK-22Z
title: Phase 22Z - Final Espionage copy polish closure
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
  - qa
roadmap_item: "Block 22Q-22Z - Espionage copy normalization and final read-only polish"
priority: high
---

## Goal

Close the Espionage copy-polish block cleanly after the implementation tasks are processed, validated, and moved through the task lifecycle.

## Purpose

The block should finish with a visually cleaner Espionage cockpit, updated docs, validated builds, and a clean task queue. This closure task ensures the queue does not stop halfway through the polish pass.

## Current problem

Without an explicit closure task, it is easy to finish the visible copy cleanup but leave tasks unmoved, docs partially updated, or validation incomplete. Espionage also touches shared cockpit expectations, so closure should verify that the accepted suite still holds together.

## Context

This block is limited to copy and presentation polish for the accepted read-only Espionage cockpit. It must not quietly expand into gameplay, mission execution, or new technical systems.

## Files to read first

- `ai/tasks/pending/`
- `ai/tasks/in-progress/`
- `ai/tasks/done/`
- `docs/dev/espionage-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

## Component discovery

Inspect the full queue state and the docs that define the accepted Espionage boundary before declaring the block closed. Prefer a narrow regression sweep over broad cleanup work.

## Implementation requirements

1. Process `TASK-22Q` through `TASK-22Z` in order and move them to `ai/tasks/done` when complete.
2. Ensure `ai/tasks/pending` contains only `.gitkeep` at the end of the block.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Confirm the final expected state:
   - Espionage remains read-only
   - primary and secondary Espionage UI no longer show the observed English strings
   - future actions remain disabled
   - diagnostics remain collapsed
   - other accepted cockpits remain usable
   - no new gameplay system was introduced
5. Run `git diff --stat` before closure and verify the block stayed within intentional scope.
6. If real blockers remain, create at most 3 specific follow-up tasks instead of opening a broad new queue.

## UI/UX requirements

- Closure guidance should remain screenshot-QA friendly.
- The cockpit should match the accepted cross-cockpit polish baseline.
- Espionage must still read as an intelligence dashboard, not a mission console.

## Backend/API requirements

- None expected unless an earlier task required a tiny metadata change to avoid hardcoded English leakage
- Any backend change must stay covered by tests and preserve Development-only boundaries

## Expected files to modify

- task files under `ai/tasks/`
- `docs/dev/espionage-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`
- any narrowly scoped Espionage frontend files needed by earlier tasks

## Safety constraints

- No espionage execution
- No sabotage
- No infiltration
- No combat
- No WebSockets
- No production auth
- No Galaxy mutations

## Acceptance criteria

- The Espionage copy-polish block can be considered closed after user visual QA.
- `ai/tasks/pending` is empty except for `.gitkeep`.
- Validation passes.
- The working tree is clean after the final commit.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- This task should not become a broad cleanup bucket.
- If a regression appears in another accepted cockpit because of shared copy or helper changes, fix the narrow regression and keep the block focused.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the working tree matches the intended closure scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for the closure task itself.
- If more than 5 files or 200 lines would be exceeded by one task during closure, split the remaining work into up to 3 specific follow-ups.
