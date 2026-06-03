# TASK-15C

---
id: TASK-15C
title: Planet module navigation panel
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Add a clear module navigation panel to `/planet` so the player uses the page as a dashboard entry point.

## Current problem
If `/planet` stops showing the full construction catalog, it needs strong navigation to the specialized modules. Otherwise the player may feel that content disappeared rather than being reorganized.

## Context from current implementation
The sidebar already exists, but `/planet` itself should guide the player based on the active planet. The page should answer “what can I manage from this planet?” and offer quick entry points.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/App.tsx`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Add a `Cabinas del planeta` or `Módulos de gestión` panel to `/planet`.
- Include cards or buttons for:
  - Construcción
  - Investigación
  - Ejército Tierra
  - Astillero
  - Defensas
  - Flotas
  - Galaxia
- Each card should show:
  - a short purpose;
  - a current status such as `Disponible`, `Preparado`, `Solo lectura` or `Próximamente`;
  - a link with `civilizationId` and `planetId`.
- Do not overstate unavailable features.
- Use the taxonomy from `TASK-14U` to show counts if useful, such as available construction actions or moved specialized items.

## UI/UX requirements
- Spanish-first.
- Compact cards, not giant panels.
- Make `/planet` feel like the command hub.
- Add useful explanation rather than duplicating sidebar labels only.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No new gameplay execution.

## Acceptance criteria
- `/planet` clearly routes to all major planet modules.
- Context is preserved.
- The full construction catalog is no longer needed on `/planet`.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Future modules can replace placeholders without changing the overall dashboard structure much.
