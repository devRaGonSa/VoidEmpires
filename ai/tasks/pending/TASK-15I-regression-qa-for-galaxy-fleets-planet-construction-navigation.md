# TASK-15I

---
id: TASK-15I
title: Regression QA for galaxy fleets planet construction navigation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Protect the accepted Galaxy and Fleets cockpits while changing Planet, Construction and module routing.

## Current problem
Boundary changes can accidentally break navigation that already works:
- Galaxy -> Planet
- Planet -> Galaxy
- Planet -> Fleets
- Construction -> Planet
- Construction -> Galaxy
- Construction -> Fleets
- Fleets -> Planet

## Context from current implementation
Galaxy and Fleets are already accepted as base cockpits. This block should not redesign them; it should only protect routing and context flow while the planet boundary work lands.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- route helpers from `TASK-15G`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/*.ts`

## Implementation requirements
- Audit links between Galaxy, Fleets, Planet and Construction.
- Replace manual URL building with shared helpers where possible.
- Ensure:
  - Galaxy `Abrir Planeta` uses the correct identifiers.
  - Planet `Volver a Galaxia` preserves `civilizationId`.
  - Planet `Abrir Flotas` preserves `civilizationId` and optional planet context.
  - Planet `Abrir Construcción` preserves both ids.
  - Construction `Abrir Planeta` preserves both ids.
  - Construction `Volver a Galaxia` preserves `civilizationId`.
  - Construction `Abrir Flotas` preserves `civilizationId` and optional planet context.
- Do not modify fleet command behavior.
- Do not add Galaxy mutations.

## UI/UX requirements
- Link labels must stay Spanish and clear.
- No broken placeholders.

## Backend/API requirements
- No backend change.

## Safety constraints
- No new fleet mutations.
- No Galaxy mutations.

## Acceptance criteria
- Navigation path is coherent.
- Frontend build passes.
- Manual QA checklist can be updated without route confusion.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Browser manual QA may still need the user if the in-app browser is unavailable.
