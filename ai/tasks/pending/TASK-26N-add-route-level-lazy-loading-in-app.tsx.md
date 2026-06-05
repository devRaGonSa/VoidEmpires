# TASK-26N Add Route Level Lazy Loading In App.tsx

---
id: TASK-26N
title: Add route-level lazy loading for cockpit pages
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: high
---

## Purpose
Lazy-load cockpit page components at the route layer so the initial frontend bundle only includes the shared application shell and globally required code.

## Current problem
Most cockpit pages are likely imported eagerly, which increases the initial JavaScript payload and contributes directly to the repeated Vite chunk-size warning.

## Context from current implementation
The accepted cockpit suite now covers galaxy, planet, construction, research, shipyard, fleets, defenses, ground army, espionage, and market. Those pages should load on demand while the shell, sidebar, route helpers, and shared navigation stay synchronous and stable.

## Goal
Use `React.lazy` with `Suspense`, or an equivalent existing project pattern, to route-lazy-load cockpit pages without changing URLs, behavior, or accepted shell navigation.

## Implementation steps
1. Inspect the current route setup in `App.tsx` and identify each page import.
2. Convert cockpit page imports to lazy imports where appropriate.
3. Add a shared route-loading fallback with Spanish-first copy such as `Cargando cabina...`.
4. Place `Suspense` boundaries so the shell remains visible while route content loads.
5. Confirm all existing route paths and query-parameter behavior remain unchanged.

## Files to inspect first
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/main.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Implementation requirements
- Use `React.lazy` and `Suspense`, or the repo's existing lazy-loading pattern if one already exists.
- Lazy-load these route pages where they currently exist:
- `StrategicMapPage` or galaxy page
- `PlanetPage`
- `ConstructionPage`
- `ResearchPage`
- `ShipyardPage`
- `FleetsPage`
- `DefensesPage`
- `GroundArmyPage`
- `EspionagePage`
- `MarketPage`
- `ModuleCabinPage` or future-module placeholder if still used
- Keep `AppShell`, sidebar, route helpers, and other shell infrastructure synchronous if they are globally needed.
- Add a shared loading fallback with Spanish-first copy and styling compatible with the current cockpit theme.
- Do not change route paths.

## Frontend requirements
- All existing routes must keep working with current query parameters.
- Sidebar active-state behavior must continue to work.
- Loading fallback must read as a loading state, not an error state.
- No visual redesign.

## Backend/API requirements
- None.

## Safety constraints
- Do not change page logic.
- Do not change endpoint calls.
- Do not change seed IDs or route contract assumptions.

## Acceptance criteria
- The frontend compiles successfully.
- Target cockpit routes are lazy-loaded from the route layer.
- Existing route URLs remain valid.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Lazy loading can accidentally hide the full shell if `Suspense` is placed too high. Keep the fallback boundary scoped to route content whenever practical.
- If one or more pages already use a special loading mechanism, keep that behavior unless it conflicts with route splitting.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
- If the route file becomes too large, split shared fallback UI into one small existing component file.
