# TASK-31F

---
id: TASK-31F
title: Shipyard backend-first refresh and stock reflection
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Make Shipyard post-submit refresh behavior understandable and strictly backend-backed.

## Context
After Shipyard real submit is wired, the user needs a clear read-model refresh story comparable to Construction and Research. The page must re-fetch authoritative state, show open production if visible, and fall back honestly if the refreshed read model has not surfaced the new order yet.

## Implementation steps

1. Re-fetch Shipyard UI state after successful backend enqueue.
2. Render queue count, open production details, resource changes, and any stock visibility only from the refreshed backend response.
3. Add the required honest Spanish fallback when the backend accepted the order but the immediate read model still lags behind.
4. Keep completion and stock mutation out of the normal Shipyard cockpit.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/
- docs/dev/shipyard-cockpit-checklist.md

## Acceptance criteria

- Shipyard re-reads backend state after successful enqueue.
- Queue, resource, and stock messaging remains backend-backed only.
- The honest lag fallback copy is shown if the refreshed read model does not yet expose the accepted order.
- No stock or queue values are invented locally.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not add auto-complete or due-processing to the normal UI

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
