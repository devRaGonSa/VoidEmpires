# TASK-17G-shipyard-route-context-loading-and-placeholder-upgrade

---
id: TASK-17G-shipyard-route-context-loading-and-placeholder-upgrade
title: Shipyard route context loading and placeholder upgrade
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Upgrade `/shipyard` from a placeholder boundary page into a context-aware Shipyard cockpit shell that loads real state and handles loading, error, and empty conditions.

## Purpose
Make the route behave like a first-class cockpit entrypoint without yet committing to full production mutation support.

## Current Problem
Shipyard currently reads like a readiness cabin rather than a playable development cockpit. It must accept `civilizationId` and `planetId`, load Shipyard state through route helpers, and render useful states even when the backend catalog is partial or disabled.

## Context
- Planet and Construction already preserve navigation context.
- Shipyard must keep the boundary explanation that it produces or prepares assets while Fleets moves existing orbital groups.
- This task upgrades the shell only; enqueue wiring belongs to later tasks.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Ensure `/shipyard` accepts `civilizationId` and `planetId`.
2. Use route helpers instead of manual query-string concatenation.
3. Load Shipyard UI state through the typed client from the earlier task.
4. Render Spanish loading state, Spanish error state, and a truthful empty state when the backend has no catalog or readiness data.
5. Preserve any existing suspicious-context warning pattern already used by adjacent pages.
6. Preserve or add navigation links for:
   - `Volver a Planeta`
   - `Abrir Construccion`
   - `Abrir Flotas`
   - `Volver a Galaxia`
7. Keep the module boundary explanation visible:
   - Shipyard prepares or produces assets.
   - Fleets moves existing orbital groups.
8. Do not add production actions yet unless the later enqueue task is also complete.

## UI/UX Requirements
- The page title should be `Astillero`.
- The route should feel like a real cockpit shell, not a blank placeholder.
- Spanish-first copy.
- Diagnostics must stay secondary or collapsed.

## Backend/API Requirements
- No backend change is expected if the Shipyard UI-state endpoint or read path already exists.

## Safety Constraints
- No enqueue action in this task.
- No fleet mutation.
- No hidden retries that look like automatic mutation.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/App.tsx` if routing needs adjustment
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/styles.css` if shell-specific styles are needed

## Acceptance Criteria
- `/shipyard` loads seeded context using `civilizationId` and `planetId`.
- The page is no longer a pure placeholder.
- Loading, error, and empty states are visible and truthful.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the backend returns limited data, the empty or disabled state should explain that honestly in Spanish.
- Keep the shell compatible with the later dashboard, catalog, queue, and confirmation tasks.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep this task focused on routing, loading, and shell behavior.
