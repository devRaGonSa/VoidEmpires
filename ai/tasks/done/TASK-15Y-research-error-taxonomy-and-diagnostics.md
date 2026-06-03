# TASK-15Y-research-error-taxonomy-and-diagnostics

---
id: TASK-15Y-research-error-taxonomy-and-diagnostics
title: Research error taxonomy and diagnostics
status: obsolete
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Normalize Research UI error handling so backend and dev errors appear as Spanish user-facing messages with technical details kept secondary.

## Purpose
The Research cockpit needs a clear, non-technical failure language so users understand what went wrong without reading raw backend phrases.

## Current Problem
If the page exposes backend or English error text directly, the cockpit becomes harder to trust and harder to support during QA.

## Context
- Planet and Construction already added error mapping and diagnostics separation.
- Research should follow the same principle while keeping its own vocabulary.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- Research presentation helpers
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- API client error patterns
- Existing backend Research result or error types

## Implementation Requirements
1. Add a research error mapper for cases such as:
   - invalid civilization id;
   - suspicious context;
   - planet not controlled, if relevant;
   - research unavailable;
   - technology not found;
   - prerequisite missing;
   - insufficient resources;
   - already in queue;
   - already completed;
   - persistence unavailable;
   - endpoint unavailable outside Development;
   - unexpected error.
2. The primary UI must show Spanish messages.
3. Technical error code or payload goes to diagnostics.
4. Avoid raw backend text as the first-level message where possible.
5. Keep messages actionable and short.

## UI/UX Requirements
- Errors should not break page layout.
- Use a consistent badge or card style.
- Suggested user-facing guidance can include phrases like:
  - `Revisa los requisitos.`
  - `Actualiza la cabina.`
  - `Usa el perfil de seed minimal-validation.`
  - `Esta accion no esta disponible en esta build.`

## Backend/API Requirements
- No backend change unless result contracts are inconsistent and need tests.
- Do not mask real backend rejection.
- Do not treat frontend validation as authority.

## Safety Constraints
- Do not hide the real cause from diagnostics.
- Do not invent translations that change the meaning of the backend error.
- Do not convert errors into silent no-ops.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts` if error parsing lives there

## Acceptance Criteria
- Research errors are Spanish and useful.
- Technical details are collapsed or secondary.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build/test` only if backend contracts change.

## Notes / Residual Risks
- Exact backend error vocabulary can be refined later.
- Keep the mapper easy to extend as new dev-only actions appear.
- A clear fallback error is better than a misleading specific one.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer one shared error mapper.
