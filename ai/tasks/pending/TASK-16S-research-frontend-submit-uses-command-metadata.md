# TASK-16S-research-frontend-submit-uses-command-metadata

---
id: TASK-16S-research-frontend-submit-uses-command-metadata
title: Research frontend submit uses command metadata
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: high
---

## Goal
Make the frontend submit Research enqueue using backend command metadata instead of reconstructed or display-only fields.

## Purpose
The UI currently renders the available item correctly but still submits something the backend rejects. The most likely failure is that the submit payload does not use the same stable fields that the backend validates.

## Current Problem
The confirmation panel and available card are driven by the read model, but the submit handler may still be reconstructing payload data from translated labels, category names, or incomplete view-model state. That can produce a payload that looks plausible in the UI but fails backend validation.

## Context
- `ResearchPage.tsx` contains card rendering, confirmation state, and submit handling.
- `researchApi.ts` contains the endpoint call.
- `researchPresentation.ts` maps backend data to view models.
- The fix should make the submit path depend on backend-provided command metadata, not UI decoration.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`

## Implementation Requirements
1. Update the Research view model so an available action carries a typed command payload.
2. Ensure the confirmation panel stores that command payload, not just display fields.
3. Ensure the submit handler sends the exact command payload expected by the backend.
4. Avoid using translated labels or category names in the request body.
5. Guard against missing command metadata:
   - do not open confirmation;
   - show `No se puede preparar esta investigacion en esta build.`
6. Keep blocked cards non-mutating.
7. After a successful submit, refresh the Research UI state.

## UI/UX Requirements
- The confirmation panel stays Spanish.
- No raw ids in the main confirmation body unless they are necessary for clarity.
- If command metadata is missing, show a clear Spanish error.

## Backend/API Requirements
- No backend change is expected if the previous task exposes the correct metadata.
- The frontend should not guess or synthesize payload fields.

## Safety Constraints
- Do not call mutation from blocked cards.
- Do not optimistic-update the queue before backend success.
- Do not hide missing metadata behind a silent failure.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/api/researchTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`

## Acceptance Criteria
- Frontend submit payload matches the backend contract.
- Available seeded Research can be submitted if the backend supports it.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- If the backend still rejects the request after this change, the next task must align backend validation with the read model.
- Keep the command payload opaque enough to avoid leaking implementation details into the UI.
- The view model should remain easy to extend for future research actions.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one typed command payload rather than multiple ad hoc fields.
