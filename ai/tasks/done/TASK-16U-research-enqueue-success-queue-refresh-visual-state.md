# TASK-16U-research-enqueue-success-queue-refresh-visual-state

---
id: TASK-16U-research-enqueue-success-queue-refresh-visual-state
title: Research enqueue success queue refresh visual state
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: medium
---

## Goal
Ensure a successful Research enqueue updates the visible queue and card states.

## Purpose
Even after the backend is aligned, the cockpit is not usable unless the user can see the result immediately: the queue should change, the item should no longer look available, and the success state should be understandable in Spanish.

## Current Problem
The UI currently gets as far as confirmation, but the visible post-submit state may still need refresh, status updates, or clearer success messaging once the enqueue actually succeeds.

## Context
- Previous tasks addressed confirmation and command metadata.
- This task focuses on post-success UI truthfulness.
- The backend should remain the source of truth for queue and card state.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- Research UI-state endpoint and queue DTOs

## Implementation Requirements
1. After successful enqueue:
   - show Spanish success feedback;
   - refresh the Research UI state;
   - the queue count updates to reflect backend state;
   - the queue panel shows the technology name, status, and duration or due time if available;
   - the enqueued technology card changes to `En investigacion` or `En cola`;
   - it must not remain `Disponible`.
2. If the backend returns the order directly, use the refreshed state as source of truth.
3. If refresh fails after success, show a warning but do not duplicate local state.
4. Keep the refresh logic deterministic and simple.

## UI/UX Requirements
- Success message should be clear and short:
  - `Investigacion enviada a la cola.`
- Queue item labels must be Spanish.
- Diagnostics should remain collapsed.

## Backend/API Requirements
- No backend change is expected unless queue data is missing or incomplete.

## Safety Constraints
- No optimistic resource spending in the UI.
- No double enqueue from a double-click while the request is pending.
- Do not invent local queue entries without backend confirmation.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`

## Acceptance Criteria
- One successful enqueue visibly updates the queue.
- The enqueued card no longer appears as available.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet test --no-build` if backend changed.

## Notes / Residual Risks
- Manual QA should confirm only one research is enqueued during the smoke pass.
- If the queue panel already receives the updated item from the read model, avoid a second source of truth in the UI.
- The refresh path should not be so clever that it hides backend state changes.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a single refresh after confirmed success.
