# TASK-17R-shipyard-cross-navigation-and-module-context

---
id: TASK-17R-shipyard-cross-navigation-and-module-context
title: Shipyard cross navigation and module context
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: medium
---

## Goal
Integrate Shipyard with Planet, Construction, Research, Fleets, and Galaxy using context-preserving route helpers and accepted module boundaries.

## Purpose
Make Astillero feel like a first-class cockpit in the navigation flow without regressing the `civilizationId` and `planetId` patterns already established in adjacent pages.

## Current Problem
Shipyard must fit into the existing module architecture. If navigation drops ids, hand-builds query strings, or bypasses suspicious-context warnings, the cockpit will feel inconsistent and can land in confusing states.

## Context
- `routeUrls.ts` is the central place for route construction.
- Planet, Construction, Research, and Fleet pages already preserve context-aware navigation.
- Shipyard should be reachable from Planet and should return cleanly to the surrounding modules.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`

## Implementation Requirements
1. Ensure the Planet `Astillero` card links to `/shipyard` with both `civilizationId` and `planetId`.
2. Ensure Construction can hand off to Shipyard with the same context when useful.
3. Ensure Shipyard links back to:
   - Planet;
   - Construction;
   - Research if relevant;
   - Fleets;
   - Galaxy.
4. Ensure Fleet can optionally link back to Shipyard if that is useful and non-invasive.
5. Use route helpers instead of manual URL assembly.
6. Preserve suspicious-context warnings and established navigation safeguards.
7. Ensure active navigation or sidebar state highlights `Astillero` when on the Shipyard route.

## UI/UX Requirements
- Spanish navigation labels.
- The links should reinforce module boundaries rather than blur them.
- Shipyard should feel like an accepted cockpit, not a one-off route.

## Backend/API Requirements
- No backend changes are expected.

## Safety Constraints
- No mutation from navigation work.
- No Fleet behavior changes.
- Do not introduce hidden redirects that change selected context silently.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Possibly `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx` or shared nav components if narrowly required

## Acceptance Criteria
- Context-preserving navigation works between Shipyard and adjacent modules.
- Suspicious-context behavior remains intact.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Future production auth can replace manual ids later; this task should not pre-empt that design.
- Keep return-link behavior simple and predictable.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Split broad navigation refactors into follow-up tasks if needed.
