# TASK-19Y-ground-army-route-context-loading-and-placeholder-upgrade

---
id: TASK-19Y-ground-army-route-context-loading-and-placeholder-upgrade
title: Ground Army route context loading and placeholder upgrade
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Upgrade `/ground-army` from a placeholder or readiness screen to a context-aware Ground Army cockpit shell.

## Purpose
Turn the route into a real cockpit entrypoint that participates in the existing module navigation architecture and loads readiness-backed state using the selected civilization and planet context.

## Current Problem
The `Ejercito Tierra` route currently exists as a module boundary placeholder. It must now load real or readiness-backed state using `civilizationId` and `planetId`.

## Context
- Route helpers already preserve context across Planet, Construction, Research, Shipyard, Defenses, Fleets, and Galaxy.
- Ground Army should fit the same module architecture and should not feel like a disconnected prototype screen.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Ensure `/ground-army` accepts:
   - `civilizationId`
   - `planetId`
2. Use the shared route helpers instead of ad hoc query-string construction.
3. Load Ground Army UI state from the typed client introduced earlier.
4. Show loading and error states in Spanish.
5. Show an honest empty or readiness state if no Ground Army data exists yet.
6. Preserve suspicious-context warning behavior if it already exists in nearby cockpit pages.
7. Preserve neighboring links:
   - `Volver a Planeta`
   - `Abrir Construccion`
   - `Abrir Defensas`
   - `Abrir Flotas`
   - `Volver a Galaxia`
8. Explain the boundary explicitly:
   - Ground Army prepares and reads terrestrial forces and readiness
   - it does not execute invasion or combat in this build
   - Construction may build military structures, Defenses manages protection, and Fleets manages orbital movement

## UI/UX Requirements
- Page title should be `Ejercito Tierra`.
- The page should look like a real cockpit shell, not a generic placeholder.
- Primary copy should be Spanish-first.
- Diagnostics must stay collapsed.

## Backend/API Requirements
- No backend change is expected if previous tasks already created the endpoint and client.

## Safety Constraints
- No terrestrial mutations in this task unless a later task wires explicit confirmation.
- No combat.
- No invasion.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/` Ground Army page files
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- related Ground Army API or helper files only if required for page loading

## Acceptance Criteria
- `/ground-army` loads seeded context.
- `/ground-army` no longer looks like an empty placeholder.
- Shared navigation context is preserved.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the backend returns limited data, the cockpit should still show readiness honestly rather than regressing back to a dead placeholder.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the work focused on route loading and cockpit shell behavior.
