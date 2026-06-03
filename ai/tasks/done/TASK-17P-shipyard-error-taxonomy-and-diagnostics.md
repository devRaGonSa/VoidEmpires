# TASK-17P-shipyard-error-taxonomy-and-diagnostics

---
id: TASK-17P-shipyard-error-taxonomy-and-diagnostics
title: Shipyard error taxonomy and diagnostics
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: medium
---

## Goal
Map Shipyard errors into specific Spanish user-facing messages while keeping raw technical details secondary.

## Purpose
Prevent the cockpit from surfacing backend failures as opaque validation blobs and make blocked actions understandable to a human tester.

## Current Problem
Shipyard can fail for many reasons: invalid ids, uncontrolled planet, missing shipyard readiness, missing asset, insufficient resources, blocked queue, or endpoint unavailability. If those failures appear as raw backend text or generic errors, the cockpit will feel broken even when validation is working correctly.

## Context
- Construction and Research already use error mapping patterns.
- Shipyard needs both user-facing messages and collapsed diagnostics for deeper troubleshooting.
- Some backend result codes may still be generic and may need tightening.

## Files to Inspect First
- Shipyard presentation helpers
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Shipyard API client files
- Backend asset production result or error code models
- Styles for error cards and diagnostics

## Implementation Requirements
1. Add a Shipyard-specific error mapper for at least:
   - invalid civilization id;
   - invalid planet id;
   - planet not controlled;
   - shipyard unavailable;
   - asset not found;
   - requirement missing;
   - insufficient resources;
   - queue full or blocked;
   - already in production if applicable;
   - persistence unavailable;
   - endpoint unavailable outside Development;
   - unexpected error.
2. The primary message must stay in Spanish and be actionable where possible.
3. Raw backend codes, payload fragments, and stack-oriented details belong only in diagnostics.
4. If the backend only returns generic failures and that blocks a useful mapper, add or refine result codes with tests.

## UI/UX Requirements
- Errors should not break layout.
- Use consistent card, badge, or alert styles already established in the frontend.
- Prefer actionable copy such as:
   - `Revisa recursos.`
   - `Construye o mejora Astillero.`
   - `Usa el contexto de Aurelia.`
   - `Esta accion no esta disponible en esta build.`

## Backend/API Requirements
- Refine backend result codes only if needed for a truthful user experience.
- Do not weaken validation logic to simplify the UI.

## Safety Constraints
- Do not treat mapped errors as success states.
- Do not hide backend failures by swallowing them silently.
- Do not bypass validation.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/api/` Shipyard client or error helper files
- `src/VoidEmpires.Frontend/src/utils/` Shipyard diagnostics or presentation helpers
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Narrow backend result or test files only if missing error codes block useful mapping

## Acceptance Criteria
- Shipyard surfaces specific Spanish errors in primary UI.
- Technical details remain secondary.
- Validation passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend files are changed.

## Notes / Residual Risks
- Error vocabulary can evolve later, but this task should remove raw generic failures from the main cockpit flow.
- Keep the mapper deterministic and easy to extend.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep this focused on mapping and diagnostics, not broader page redesign.
