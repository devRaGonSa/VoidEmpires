# TASK-14Z

---
id: TASK-14Z
title: Defenses cabin boundary placeholder
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Create or refine the Defenses cabin so defensive structures and future defense mechanics are not mixed into general construction.

## Current problem
Visual QA showed defense content inside `/construction`. Even if some defensive structures are buildings, the player expects the Defensas module to own that space.

## Context from current implementation
The sidebar already includes Defensas. This task should make that screen meaningful as a future module boundary without introducing real combat or interception.

## Files to inspect first
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx` or the equivalent route page
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation requirements
- Implement or refine `/defenses`.
- Show:
  - selected planet context;
  - defensive module explanation;
  - current readiness placeholder;
  - a note that real defense execution/combat is not implemented;
  - related infrastructure if any, such as `Malla defensiva`.
- If defensive construction actions exist in the generic data, do not show them in the main `/construction` catalog.
- Either show them in `/defenses` as disabled/readiness cards or show a `pendiente de implementación` panel.
- Do not add defense mutation calls.

## UI/UX requirements
- Spanish-first.
- Clear `Defensas` identity.
- Do not look like a broken placeholder.
- Keep diagnostics collapsed if any are shown.

## Backend/API requirements
- No backend change expected.

## Safety constraints
- No combat.
- No interception execution.
- No damage systems.
- No defense queue execution unless it already exists and is read-only.

## Acceptance criteria
- `/defenses` exists and has useful boundary copy.
- Defensive categories are not mixed into `/construction` as full action sections.
- Navigation context works.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- A later Defenses v1 block can add actual defensive construction or queue behavior.
