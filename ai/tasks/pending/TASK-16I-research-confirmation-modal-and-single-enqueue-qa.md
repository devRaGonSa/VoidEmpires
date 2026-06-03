# TASK-16I-research-confirmation-modal-and-single-enqueue-qa

---
id: TASK-16I-research-confirmation-modal-and-single-enqueue-qa
title: Research confirmation modal and single enqueue QA
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16E-16P - Research cockpit QA correction and first usable research flow"
priority: high
---

## Goal
Ensure the available seeded Research item can open an explicit confirmation flow and enqueue safely.

## Purpose
The first usable Research flow is not complete unless a real available item can be reviewed, confirmed, and sent once through the safe development path.

## Current Problem
Because no item is currently available, the confirmation flow could not be verified visually or functionally. That makes it impossible to validate the end-to-end play path that the cockpit is supposed to support.

## Context
- The previous block introduced guarded enqueue flow and feedback.
- This task must validate the flow against a real available seeded item rather than a synthetic UI state.
- The confirmation must remain explicit and user-controlled.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- Existing Research enqueue endpoint and tests
- Construction or Fleet confirmation patterns for reference

## Implementation Requirements
1. Ensure clicking the available seeded research opens a confirmation modal or panel.
2. The confirmation should show:
   - technology name;
   - category;
   - cost;
   - duration;
   - selected planet and civilization context;
   - whether the requirements are satisfied;
   - `Confirmar`;
   - `Cancelar`.
3. Cancel must not mutate.
4. Confirm must call only the safe dev endpoint.
5. After success:
   - refresh the Research UI state;
   - the queue count changes;
   - the card no longer appears simply available;
   - a Spanish success message appears.
6. If the backend rejects the enqueue, show the mapped Spanish error.
7. Do not enqueue multiple items automatically as part of QA.

## UI/UX Requirements
- Primary text must be Spanish.
- No raw DTO names should appear in the modal title or primary body.
- Technical response data should remain diagnostics-only.

## Backend/API Requirements
- If endpoint behavior is broken, fix it with tests.
- Keep Development-only gating intact.
- Do not create a production mutation path.

## Safety Constraints
- No optimistic queue without backend confirmation.
- No hidden research effects beyond the current backend state.
- No silent mutation on card click.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- Backend tests if the mutation path needs correction

## Acceptance Criteria
- One seeded available Research item can be reviewed.
- The confirmation appears before the enqueue action.
- Confirming one Research item refreshes the queue or returns a clear backend rejection.
- Build, tests, and frontend build pass.

## Validation
- `dotnet build --no-restore` if backend changes are made.
- `dotnet test --no-build` if backend changes are made.
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- The final browser QA should confirm only one safe item is enqueued during the smoke pass.
- If the available item changes in the seed, the modal should still display the same set of truth fields.
- Keep the flow simple enough that a human can trust it during manual testing.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer one confirmation path and one refresh path.
