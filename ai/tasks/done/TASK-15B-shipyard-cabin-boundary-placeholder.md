# TASK-15B

---
id: TASK-15B
title: Shipyard cabin boundary placeholder
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Create or refine the Astillero module boundary so orbital production and shipyard concepts do not live in the general construction catalog.

## Current problem
Visual QA showed shipyard-like content inside construction. The sidebar already has Astillero, so ships and orbital assets should be conceptually separated from general construction and from Flotas.

## Context from current implementation
The backend may already have asset production, orbital group and fleet foundations. That does not mean the Astillero UI should behave like Flotas. Astillero should later prepare or produce ships and assets, while Flotas manages deployed groups.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `ai/current-state.md`
- `docs/dev/fleet-api-contracts.md`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx` or the equivalent route page
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Implement or refine `/shipyard`.
- Show:
  - selected planet context;
  - module purpose;
  - relationship to Flotas;
  - current placeholder or dev-safe status;
  - related infrastructure such as `Astillero` or `Mando de flota` if known;
  - a link to Flotas.
- Do not implement ship production enqueue.
- Do not implement fleet split or merge.
- Do not implement new orbital transfer behavior.
- Do not show shipyard or military-space items as full `/construction` catalog sections unless taxonomy says they are general infrastructure.

## UI/UX requirements
- Spanish-first.
- Explain clearly:
  - Astillero will produce or prepare orbital assets later.
  - Flotas manages deployed orbital groups.
- Provide clear navigation to `/planet`, `/construction` and `/fleets`.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No ship production mutation.
- No new fleet commands.
- No split/merge enablement.

## Acceptance criteria
- `/shipyard` exists as a useful module screen.
- Shipyard content no longer pollutes `/construction` as a full category.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Future Shipyard v1 can use existing asset production foundations.
