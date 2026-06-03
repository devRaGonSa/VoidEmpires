# TASK-15A

---
id: TASK-15A
title: Ground army cabin boundary placeholder
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Create or refine `Ejército Tierra` as its own module so barracks, academy and troop concepts are not mixed into construction.

## Current problem
Visual QA showed terrestrial military content inside the construction catalog. The sidebar already contains `Ejército Tierra`, so that content should be separated conceptually and presented as its own cabin.

## Context from current implementation
The backend may already have some population or military-capacity foundations, but this task must not implement real troop training or army management. It is only about a readable and safe module boundary.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `ai/current-state.md`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx` or the equivalent route page
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Implement or refine the route behind `Ejército Tierra`.
- Use a route name consistent with the existing app routing, for example `/ground-army` if no route already exists.
- Show:
  - selected planet;
  - purpose of the ground army cabin;
  - capacity or readiness placeholder;
  - related buildings such as `Barracones` or `Academia militar` if known;
  - a clear message that training or army orders are not yet implemented.
- Do not show the full construction catalog.
- Do not add troop mutation endpoints.
- Do not invent units.

## UI/UX requirements
- Spanish-first.
- Explain the difference:
  - Construction builds infrastructure.
  - `Ejército Tierra` will manage troops and ground military capacity later.
- Preserve navigation to `/planet` and `/construction`.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No troop training.
- No battles.
- No invasions.
- No planetary assault mechanics.

## Acceptance criteria
- `Ejército Tierra` opens a meaningful module screen.
- Ground-military content is not duplicated in `/construction` as a full action section.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Future Army v1 should be a separate block.
