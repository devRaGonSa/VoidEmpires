# TASK-33I

---
id: TASK-33I
title: Shipyard session context integration
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Integrate session banner/context into Shipyard without changing accepted shipyard behavior.

## Context
Shipyard already has persisted production enqueue behavior. This task only makes the page session-aware and improves navigation handoffs.

## Implementation steps

1. Read Shipyard page behavior and accepted checklist notes.
2. Display the reusable playable session banner/context.
3. Use the route context fallback for missing ids and offer continuation from local playable session.
4. Ensure handoffs to Planet and Fleets preserve `civilizationId` and `planetId`.
5. Preserve accepted behavior:
   - selection does not mutate;
   - modal open does not mutate;
   - confirm mutates;
   - refresh/fallback is backend-first;
   - no fleet mission creation;
   - no movement;
   - no combat;
   - no auto-complete is added.
6. Keep raw ids secondary or diagnostic only.

## Files to read first

- docs/dev/shipyard-cockpit-checklist.md
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- Optional: docs/dev/shipyard-cockpit-checklist.md

## Acceptance criteria

- Shipyard is session-aware without behavior regression.
- Missing-id entry offers local-session continuation.
- Handoffs to Planet/Fleets preserve ids.
- Raw ids remain secondary/diagnostic only.

## Constraints

- Do not add fleet missions, movement, combat, attacks, or auto-complete.
- Do not fake resources, queues, or stock.
- Do not claim visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33I message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
