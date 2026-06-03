# TASK-18R-galaxy-frontend-api-error-and-empty-state-visibility

---
id: TASK-18R-galaxy-frontend-api-error-and-empty-state-visibility
title: Galaxy frontend API error and empty state visibility
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: high
---

## Goal
Make Galaxy loading, context, error, and empty states visible instead of silently falling back to an empty shell.

## Purpose
Improve operator and QA visibility so Galaxy failures are diagnosable from the UI without opening source code first.

## Current Problem
The current shell can render without any useful explanation when Galaxy data does not load or does not render. This makes route and data regressions look identical.

## Context
- `StrategicMapPage.tsx` currently has a query form and error string handling, but successful cockpit rendering is tied to `result`.
- Other cockpit pages already surface clearer loading, error, and empty-state panels.
- The accepted Galaxy cockpit must remain primary when real data exists.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`

## Implementation Requirements
1. Add explicit render branches for:
   - loading
   - missing context
   - invalid context
   - API error
   - empty strategic read model
   - successful cockpit render
2. If the API fails, show `No se pudo cargar el mapa de Galaxia.`
3. If the API succeeds but returns no systems, show `No hay sistemas visibles para esta civilizacion.`
4. If the context is invalid, show `El contexto de civilizacion no es valido para Galaxia.`
5. Keep technical payloads and raw diagnostics collapsed behind a secondary disclosure.
6. Do not let fallback panels replace the real cockpit when non-empty data exists.
7. Keep the page compact; avoid giant debug consoles.

## UI/UX Requirements
- Primary content must remain Spanish-first.
- Empty or error states should appear in the cockpit region, not only as tiny helper text near the query field.
- Diagnostics must remain collapsed by default.

## Backend/API Requirements
- No backend change expected.

## Safety Constraints
- No mutations.
- No debug-first redesign.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`
- Galaxy API types or helpers only if error-state parsing needs alignment

## Acceptance Criteria
- Galaxy never appears as a blank shell.
- Missing-context, error, and empty-data states are visually distinguishable.
- The successful cockpit render remains unchanged in purpose.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- This task improves observability even if a separate task fixes the true route or data defect.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the work limited to visibility and diagnostics.
