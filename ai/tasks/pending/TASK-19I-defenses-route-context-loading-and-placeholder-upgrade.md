# TASK-19I-defenses-route-context-loading-and-placeholder-upgrade

---
id: TASK-19I-defenses-route-context-loading-and-placeholder-upgrade
title: Defenses route context loading and placeholder upgrade
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Upgrade `/defenses` from a placeholder/readiness screen into a context-aware cockpit shell that loads real state using `civilizationId` and `planetId`.

## Purpose
Make the route feel like an actual playable foundation, even before every later panel is complete, while preserving the current module boundaries and cockpit navigation patterns.

## Current Problem
The route exists today but still behaves like a placeholder cabin. It needs consistent query-context loading, loading and error handling, and real contextual messaging based on the selected civilization and planet.

## Context
- Shared route helpers already preserve context across Planet, Construction, Research, Shipyard, Fleets, and Galaxy.
- The current placeholder behavior documents boundaries, but it no longer matches the desired cockpit progression.
- Diagnostics patterns already exist for suspicious or missing context.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Ensure `/defenses` accepts and uses:
   - `civilizationId`
   - `planetId`
2. Use existing route helper conventions instead of ad hoc URL building.
3. Load Defenses UI state through the typed client from earlier tasks.
4. Replace the generic placeholder body with a cockpit shell that includes:
   - loading state in Spanish
   - error state in Spanish
   - empty or readiness state when the backend has limited defense data
   - contextual explanation of what Defenses owns in this build
5. Preserve suspicious-context warning behavior if the shared cockpit conventions already support it.
6. Preserve or add context-aware links for:
   - `Volver a Planeta`
   - `Abrir Construccion`
   - `Abrir Astillero`
   - `Abrir Flotas`
   - `Volver a Galaxia`
7. Make the boundary explicit:
   - Defenses prepares planetary protection and readiness
   - this build does not execute combat
   - Construction may still own some infrastructure actions

## UI/UX Requirements
- Page title must be `Defensas`.
- The route should look like a real cockpit shell, not a generic cabin placeholder.
- Primary copy must be Spanish-first.
- Diagnostics must stay collapsed or secondary.

## Backend/API Requirements
- No backend change is expected if the read-model task landed correctly.
- If the route exposes a missing contract assumption, feed it back into the Defenses read-model work without improvising raw endpoint composition in the page.

## Safety Constraints
- No defense mutations yet unless later tasks explicitly wire a safe confirmation path.
- No combat execution or simulation.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx` or equivalent route component
- `src/VoidEmpires.Frontend/src/App.tsx` if route wiring needs refinement
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- shared styles only if necessary

## Acceptance Criteria
- `/defenses` loads seeded context.
- `/defenses` is no longer an empty placeholder.
- Context-preserving links behave like neighboring cockpits.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If backend data is intentionally limited, the cockpit shell should still feel complete by explaining readiness and limitations clearly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Focus on route loading and shell upgrade, not every final panel detail.
