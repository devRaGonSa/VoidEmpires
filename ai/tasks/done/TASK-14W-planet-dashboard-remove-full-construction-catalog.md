# TASK-14W

---
id: TASK-14W
title: Planet dashboard remove full construction catalog
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Reduce `/planet` to a dashboard/resumen screen and remove the full construction catalog from the main Planet cockpit.

## Current problem
`/planet` currently contains detailed construction categories and action cards. That is too much for a planet overview and makes the screen feel semantically mixed with `/construction`.

## Context from current implementation
Planet already has useful sections for identity, resources, economy, current buildings, queue summary and navigation. The remaining issue is that it also duplicates the full action catalog, which turns the dashboard into a long management page.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/planet-cockpit-checklist.md`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Remove the full construction action catalog from `/planet`.
- Keep only a compact construction summary:
  - active queue count;
  - next general construction action if useful;
  - blocked items count if useful;
  - an `Abrir Construcción` CTA.
- Keep current buildings and queue summaries, but keep them compact.
- If there are no construction orders, show:
  - `No hay construcciones en cola.`
  - `Gestiona nuevas obras desde Construcción.`
- Keep a related-cabins area for:
  - Construcción
  - Investigación
  - Ejército Tierra
  - Astillero
  - Defensas
  - Flotas
  - Galaxia
- Preserve `civilizationId` and `planetId` in navigation links.

## UI/UX requirements
- `/planet` should feel like a command overview.
- It should answer:
  - What planet am I viewing?
  - What resources do I have?
  - What is being built?
  - What are my key modules?
  - Where do I go next?
- Primary copy should be Spanish.
- Diagnostics should stay collapsed.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- Removing the catalog from `/planet` must not remove construction functionality from `/construction`.
- Direct planet links from galaxy must continue to work.

## Acceptance criteria
- `/planet` no longer displays the full construction catalog.
- `/planet` shows a dashboard summary plus navigation.
- `/construction` remains the construction action screen.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- If the screen still feels too dense, a later task can split summary cards further.
