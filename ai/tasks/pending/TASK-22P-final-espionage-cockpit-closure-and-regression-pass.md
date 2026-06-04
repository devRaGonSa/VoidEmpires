# TASK-22P

---
id: TASK-22P
title: Phase 22P - Final Espionage cockpit closure and regression pass
status: pending
type: platform
team: platform
supporting_teams:
  - qa
  - docs
  - frontend
  - backend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Close the Espionage cockpit block, move all related tasks through the task lifecycle, and run a narrow regression pass over all accepted cockpits.

## Purpose

Espionage touches shared navigation, route helpers, seed docs, sidebar state, and strategic read-state assumptions. This final task exists to confirm that the new cockpit did not regress accepted surfaces.

## Current problem

Without an explicit closure task, it is easy to stop after the main page works and leave the queue half-moved, shared docs incomplete, or neighboring cockpits silently regressed.

## Context

Accepted cockpits currently include:

- `Galaxy`
- `Planet`
- `Construction`
- `Research`
- `Shipyard`
- `Fleets`
- `Defenses`
- `Ground Army`

Espionage should join that suite only after final validation and queue cleanup.

## Files to read first

- `ai/tasks/pending/`
- `ai/tasks/done/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/espionage-cockpit-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `ai/current-state.md`

## Component discovery

Inspect the final task lifecycle state, the shared smoke checklist, and the module docs before declaring the block closed. Prefer small doc cleanup over last-minute feature changes.

## Implementation requirements

1. Process `TASK-22A` through `TASK-22P` in order and move them to `ai/tasks/done` as they are completed.
2. Ensure `ai/tasks/pending` contains only `.gitkeep` when the block is finished.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
4. Update final regression guidance to include:
   - apply `cockpit-validation` twice
   - open `/galaxy`
   - open `/planet`
   - open `/construction`
   - open `/research`
   - open `/shipyard`
   - open `/fleets`
   - open `/defenses`
   - open `/ground-army`
   - open `/espionage`
5. Confirm the expected state:
   - Espionage loads and is no longer a placeholder
   - Espionage does not execute missions
   - other accepted cockpits remain usable
   - Galaxy remains read-only
6. If real blockers remain, create at most 3 specific follow-up tasks instead of opening a broad new wave.

## UI/UX requirements

- Final regression docs should stay screenshot-QA friendly
- Espionage should match the accepted cockpit polish baseline

## Backend/API requirements

- No new feature work unless a regression fix is strictly required to close the block
- Preserve Development-only gating for any Espionage backend surface

## Expected files to modify

- task files under `ai/tasks/`
- shared smoke or closure docs if needed
- `ai/current-state.md` only if the final accepted description still needs alignment

## Safety constraints

- No espionage execution
- No sabotage
- No combat
- No WebSockets
- No broad scope expansion beyond read-only Espionage

## Acceptance criteria

- The Espionage block can be closed after user visual QA.
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

- This task should not become a generic cleanup bucket. Limit it to closure, queue movement, validation, and narrowly scoped regressions.
- If one accepted cockpit regresses because of shared navigation or route changes, fix that regression within the block rather than documenting around it.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Verify the working tree matches the expected closure scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer fewer than 3 commits for the closure task itself.
- If multiple regressions appear, prioritize the most critical ones and create up to 3 follow-up tasks only if needed.
