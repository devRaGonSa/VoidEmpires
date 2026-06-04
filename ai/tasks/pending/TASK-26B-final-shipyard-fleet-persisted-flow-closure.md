# TASK-26B

---
id: TASK-26B
title: Phase 26B - Final Shipyard and Fleet persisted flow closure
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Close the Shipyard and Fleet persisted QA block cleanly after all implementation, documentation, and validation tasks are complete.

## Current problem

Without an explicit closure task, the block can end in a partially cleaned-up state with pending tasks still present, incomplete validation, or unclear final boundaries.

## Context from current implementation

- This block follows the same closure pattern already used for previous persisted QA blocks.
- The final state must leave `ai/tasks/pending` empty except for `.gitkeep`, preserve explicit exclusions, and keep the working tree clean.
- Follow-up task creation should be rare and only used for real blockers.

## Files to read first

- ai/tasks/pending/TASK-25M-shipyard-fleet-persisted-flow-scope-audit.md
- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/fleet-api-contracts.md
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- ai/tasks/in-progress/*
- ai/tasks/done/*
- ai/tasks/pending/.gitkeep
- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Implementation requirements

1. Move `TASK-25M` through `TASK-26B` to `ai/tasks/done` after they are processed.
2. Ensure `ai/tasks/pending` contains only `.gitkeep`.
3. Run final validation:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
4. Confirm the final expected state:
   - Shipyard persisted enqueue covered or explicitly documented as unavailable
   - Fleet read-state after Shipyard real orders covered
   - stock-to-fleet allocation decision documented
   - backend-only PowerShell QA commands and docs available
   - no visual QA required for technical closure
   - no new gameplay systems introduced
5. Keep follow-up task generation capped at three only if a real blocker remains.
6. Ensure the working tree is clean before closing.

## Backend/API requirements

- Do not add new gameplay features in this closure task.
- If final validation exposes a blocking bug, fix that bug in the appropriate implementation task before closure rather than hiding it here.

## Script/QA requirements

- Confirm the final command set and docs are the supported backend-only QA path for this block.
- Ensure script validation is part of the final closure gate.

## Safety constraints

- No production auth.
- No production data.
- No destructive reset.
- No combat.
- No split or merge.
- No new movement.
- No 3D.

## Acceptance criteria

- The block can be closed technically with validation green.
- `ai/tasks/pending` is empty except for `.gitkeep`.
- The working tree is clean.
- Remaining scope boundaries are explicit.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```

## Notes / residual risks

- If a real architectural blocker prevents full closure, the follow-up tasks must be specific and limited rather than reopening the whole block.

