# TASK-23I

---
id: TASK-23I
title: Phase 23I - Market trade signals and route placeholders
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Show trade signals and future commercial route placeholders without executing logistics.

## Purpose

Market should connect economy state to future logistics in a safe way, helping the player understand where future trade or movement pressures may exist while keeping execution out of scope.

## Current problem

The cockpit currently has no dedicated way to express trade pressure, route context, or logistics dependency. Without that framing, Market would feel disconnected from Fleets and Galaxy.

## Context

Fleets already owns transfer and logistics behavior, and Galaxy already owns route context. Market can point toward those systems, but it must not create routes or move fleets.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- Market view-model files

## Component discovery

Inspect how current cockpits present secondary signals, disabled action cards, and cross-cockpit handoffs. Prefer reusing those patterns for route placeholders and logistics notes.

## Dependency analysis

Expected Market signal flow:

- Market view model -> grouped trade signals
- route helpers -> handoffs to Fleets and Galaxy
- disabled action metadata -> placeholder cards with explicit limitations

## Implementation requirements

1. Add a section such as:
   - `Senales comerciales`
   - `Rutas comerciales futuras`
2. Show read-only hints such as:
   - resource surplus
   - resource constraint
   - planet or system route context
   - fleet or logistics dependency
3. Add disabled route placeholders such as:
   - `Crear ruta comercial no disponible`
   - `Transferencia de recursos no disponible`
4. Add handoff links such as:
   - `Revisar logistica en Flotas`
   - `Ver contexto de ruta en Galaxia`
5. Do not create trade routes.
6. Do not move fleets.
7. Keep the section visibly secondary to the core reserve and summary information.

## UI/UX requirements

- Secondary section, not the primary hero
- Spanish-first
- Disabled actions should be present but not dominant
- Signal text should remain understandable without exposing raw backend terminology

## Backend/API requirements

- No backend change is expected unless the read model needs a narrow derived signal field that can be tested safely.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- Market helper or view-model files
- route helper files only if a small handoff helper change is needed

## Safety constraints

- No route execution
- No resource transfer
- No fleet movement
- No hidden mutation through cross-cockpit links

## Acceptance criteria

- Trade signals and placeholders are visible and safe.
- The Market page points players toward Fleets and Galaxy where appropriate.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation only if backend files are touched.

## Notes / residual risks

- Real trade logistics remain a future system; this task should create orientation, not mechanics.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to Market signal and handoff presentation.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer read-only hints over derived signal complexity.
- If realistic route planning logic becomes necessary, stop and split it into a future task.
