# TASK-15G

---
id: TASK-15G
title: Route context safety and query param normalization
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Prevent context mistakes such as using `planetId` as `civilizationId` and make navigation between modules reliable.

## Current problem
During QA, a planet page was opened with the wrong identifier in the civilization slot. That causes ownership errors and makes the cockpit harder to trust.

## Context from current implementation
The app currently relies on query parameters such as `civilizationId` and `planetId`. That is workable in development, but the UI should reduce mistakes and keep navigation helpers centralized.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/utils/*.ts`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/*.tsx`

## Implementation requirements
- Add or refine route helper functions such as:
  - `buildPlanetUrl(civilizationId, planetId)`
  - `buildConstructionUrl(civilizationId, planetId)`
  - `buildResearchUrl(civilizationId, planetId)`
  - `buildGroundArmyUrl(civilizationId, planetId)`
  - `buildShipyardUrl(civilizationId, planetId)`
  - `buildDefensesUrl(civilizationId, planetId)`
  - `buildGalaxyUrl(civilizationId)`
  - `buildFleetsUrl(civilizationId, planetId?)`
- Avoid manual query-string concatenation in many places.
- Add a basic frontend warning if the context looks suspicious:
  - when `civilizationId` starts with `40000000` and `planetId` is missing or equal, show a Spanish warning that the context may be wrong.
- Do not silently mutate suspicious context.
- Keep the normal path smooth for valid dev seed URLs.

## UI/UX requirements
- If context is invalid, show:
  - `El identificador de civilización no parece válido para esta cabina.`
  - `Revisa que no hayas usado el id del planeta como civilización.`
- Provide a helper link to the known development seed context only if that pattern already exists in the app.

## Backend/API requirements
- No backend change.

## Safety constraints
- Do not hardcode seed IDs as production behavior.
- If a helper seed link is used, clearly mark it as a development helper.

## Acceptance criteria
- Module links preserve correct context.
- The previous `planetId` as `civilizationId` mistake is easier to diagnose.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Production auth will eventually replace manual `civilizationId` entry.
