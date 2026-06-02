# TASK-12J

---
id: TASK-12J
title: Phase 12J - Fleet active transfer status panel
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12J"
priority: medium
---

## Goal
Create or significantly improve a focused active-transfer or status panel for current fleet movements.

## Context
The Fleet cockpit already exposes transfer data, but the current presentation is still too technical and not prominent enough for a gameplay screen. This task should make active transfer state obvious, readable, and game-like while staying within the existing prototype command boundary and avoiding new live update systems.

## Implementation steps

1. Inspect the Fleet page state and any presentation helpers that expose active transfer counts, origin, destination, timestamps, progress, or cancel availability.
2. Build or refine a dedicated active-transfer panel that makes the current movement state visible at a glance.
3. Show active transfer count, squad or group, origin planet, destination planet, current status, departure or arrival if available, progress if available, and whether cancellation is available.
4. Provide a compact empty state when no active transfers exist and keep the panel game-like instead of API-like.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The active-transfer panel clearly shows current fleet movement state in a readable, game-like format.
- Active transfer count, route context, timestamps, progress, and cancel availability are visible when data exists.
- The empty state clearly says there are no active transfers.
- The panel does not look like raw JSON or API metadata.
- No polling, WebSockets, complete-due execution, or combat or interception execution is added.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and limited to existing fleet UI state.
- Do not add new backend endpoints or live update systems.
- Do not add extra mutation commands.
- Split follow-up work if the panel needs broader state modeling.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single commit for this task.
- If the status panel needs deeper rewiring, stop and create a follow-up task first.
