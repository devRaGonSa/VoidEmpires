# TASK-18P-galaxy-route-and-query-context-normalization

---
id: TASK-18P-galaxy-route-and-query-context-normalization
title: Galaxy route and query context normalization
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: high
---

## Goal
Normalize Galaxy routing and context loading so the cockpit does not silently disappear when navigation arrives with or without explicit query parameters.

## Purpose
Make Galaxy navigation predictable across direct URLs, sidebar navigation, and cross-cockpit handoffs.

## Current Problem
Both `/galaxy` and `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001` are reported as visually empty. The app must either load the seeded civilization context or show a clear, visible missing-context state.

## Context
- `routeUrls.ts` currently builds Galaxy URLs to `/`, not `/galaxy`.
- The sidebar still points `Galaxia` to `/`.
- Other cockpits already preserve `civilizationId` and `planetId` through shared helpers.
- The task must settle whether Galaxy keeps `/`, gains `/galaxy`, or supports both.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`

## Implementation Requirements
1. Decide and implement the canonical Galaxy route contract based on the audit:
   - `/` only
   - `/galaxy` only
   - or both with one canonical helper path
2. Ensure `buildGalaxyUrl(...)` writes the same route shape that `StrategicMapPage` expects.
3. Ensure `StrategicMapPage` reads the same query keys that route helpers produce.
4. Ensure all Galaxy links and sidebar entries use the shared helper instead of hardcoded paths.
5. If `civilizationId` is missing:
   - either use a deterministic Development-only default context with explicit `Uso local`
   - or show a visible Spanish prompt to provide context
6. Never leave the page as a blank shell.
7. Apply suspicious-context validation consistently if `planetId` and `civilizationId` are mixed up.

## UI/UX Requirements
- If context is missing, show `Carga un contexto de civilizacion para abrir Galaxia.`
- If a Development-only default is used, label it clearly with `Uso local`.
- Primary copy must remain Spanish-first.

## Backend/API Requirements
- No backend change is expected.

## Safety Constraints
- No mutations.
- No production-auth assumptions.
- Do not break existing links from Planet, Construction, Research, Shipyard, or Fleets.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- cockpit pages that currently hardcode Galaxy links, if any

## Acceptance Criteria
- `buildGalaxyUrl(...)` and the rendered route agree.
- `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001` loads meaningful Galaxy content or a truthful visible state.
- `/galaxy` no longer appears blank.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Production authentication may later replace manual civilization query params; keep this normalization Development-safe and easy to retire.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep routing changes centralized.
