# TASK-14X

---
id: TASK-14X
title: Module route foundation for specialized planet cabins
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Ensure the specialized planet module routes exist as useful, development-safe placeholders.

## Current problem
The sidebar already lists specialized modules, but if those routes are missing or mixed into construction, the navigation becomes misleading. We need dedicated module surfaces even when the real gameplay is not implemented yet.

## Context from current implementation
The app already has a strong cockpit visual style. Some specialized modules may already exist as routes, placeholders or partial shells. This task should make those surfaces reliable without adding real gameplay.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/*.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Ensure routes exist for:
  - `/research`
  - `/ground-army` or the existing route used by `Ejército Tierra`
  - `/shipyard` or the existing route used by `Astillero`
  - `/defenses`
- Each route must accept `civilizationId` and optional `planetId`.
- Each route should display:
  - selected planet context if available;
  - module purpose;
  - current implementation status;
  - what belongs here;
  - what does not belong here;
  - a link back to `/planet`;
  - a link to `/construction` if relevant.
- Do not add real mutation flows or new endpoints.

## UI/UX requirements
- The routes must not look broken.
- They should say clearly:
  - `Cabina preparada para futura implementación.`
  - `Esta sección no ejecuta órdenes todavía.`
- Do not expose raw backend payloads in the primary view.
- Use the same cockpit visual language as the rest of the frontend.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No real research queue execution.
- No real troop training.
- No real ship production.
- No real defense execution.

## Acceptance criteria
- Sidebar specialized modules open meaningful placeholder/readiness screens.
- Each route preserves context.
- Each route explains its boundary.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- These are placeholders by design. Documentation must not overclaim gameplay support.
