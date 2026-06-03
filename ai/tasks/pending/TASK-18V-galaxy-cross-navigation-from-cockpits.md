# TASK-18V-galaxy-cross-navigation-from-cockpits

---
id: TASK-18V-galaxy-cross-navigation-from-cockpits
title: Galaxy cross navigation from cockpits
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Verify and fix navigation back into Galaxy from Planet, Construction, Research, Shipyard, and Fleets.

## Purpose
Protect manual QA and normal cockpit handoff flow after the main Galaxy route is restored.

## Current Problem
Even if direct Galaxy navigation works, return links from other cockpits may still send invalid context or point to the wrong route.

## Context
- `routeUrls.ts` centralizes cross-cockpit URL building.
- The accepted cockpit model depends on moving between Galaxy and the specialized cabins without rebuilding URLs by hand.
- Recent route helper work reduced duplicated URL construction, so regressions may now be centralized.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`

## Implementation Requirements
1. Audit every `Galaxia` or `Volver a Galaxia` link in the cockpit pages.
2. Ensure all of them use the shared Galaxy route helper.
3. Ensure `civilizationId` is preserved correctly.
4. Ensure `planetId` is not accidentally passed as `civilizationId`.
5. Ensure Galaxy can consume the handed-off context and render the cockpit without manual form entry.
6. If suspicious context is detected, surface a visible warning instead of navigating silently into a broken state.

## UI/UX Requirements
- Link labels must remain Spanish.
- Navigation must feel consistent across all accepted cockpits.
- Avoid broken or surprising route transitions.

## Backend/API Requirements
- None.

## Safety Constraints
- No mutations.
- No new navigation patterns outside the existing cockpit model.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- the cockpit pages that still hardcode or misuse Galaxy links

## Acceptance Criteria
- Planet to Galaxy works with preserved context.
- Construction to Galaxy works with preserved context.
- Research to Galaxy works with preserved context.
- Shipyard to Galaxy works with preserved context.
- Fleets to Galaxy works with preserved context.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual browser QA will still be needed to verify the exact click flow after the helper fixes land.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Centralize fixes through the route helper where possible.
