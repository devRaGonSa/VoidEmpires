# TASK-29K

---
id: TASK-29K-construction-cross-cockpit-safety-regression
title: Verify construction mutation does not affect other cockpits
status: done
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Ensure safe boundaries and no accidental mutation expansion outside Construction.

## Context
Navigation and query parameters across cockpits must remain stable.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- ai/tasks/pending/TASK-29K-construction-cross-cockpit-safety-regression.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Review notes

- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts` centralizes `civilizationId` and `planetId` preservation for `Planet`, `Construction`, `Research`, `Shipyard`, `Defenses`, `Galaxy`, and `Fleets`.
- The current `/construction` route is still implemented through `PlanetPage variant="construction"` and does not introduce mutation helpers for non-construction cockpits.

## Implementation steps

1. Review handoff links from Construction to Planet, Research, Defenses, Shipyard, Galaxy.
2. Confirm query params are preserved.
3. Verify no other cockpit gained mutation surface.
4. Add smoke checklist note for construction real-enqueue path.

## Acceptance criteria

- Safe cross-cockpit navigation and unchanged non-construction behavior.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
