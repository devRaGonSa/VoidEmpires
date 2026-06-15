# TASK-33J

---
id: TASK-33J
title: Readiness pages session context
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: medium
---

## Goal
Integrate session context into the read-only readiness pages, Defenses and Fleets.

## Context
Defenses and Fleets should fit the playable loop navigation while remaining strictly read-only readiness surfaces.

## Implementation steps

1. Read Defenses and Fleets pages and route helper behavior.
2. Apply the session banner/context or a consistent fallback to both pages.
3. If query ids are missing and local session exists, offer continuation.
4. Preserve handoffs to Planet and Shipyard with `civilizationId` and `planetId`.
5. Keep read-only scope:
   - no defense production mutation;
   - no fleet movement;
   - no missions;
   - no combat.
6. Keep Spanish-first copy and raw ids secondary/diagnostic only.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- Optional: src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- Optional: docs/dev/planet-cockpit-checklist.md

## Acceptance criteria

- Defenses and Fleets are session-aware.
- Both pages remain read-only.
- Handoffs preserve ids.
- Missing-id entry can continue from local session.

## Constraints

- Do not add combat, movement, missions, attacks, or defense production.
- Do not perform or claim visual QA.
- Preserve lazy loading.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33J message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
