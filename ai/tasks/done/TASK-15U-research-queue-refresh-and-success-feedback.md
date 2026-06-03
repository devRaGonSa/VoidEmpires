# TASK-15U-research-queue-refresh-and-success-feedback

---
id: TASK-15U-research-queue-refresh-and-success-feedback
title: Research queue refresh and success feedback
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Make research enqueue results visible and ensure the UI refreshes correctly after a successful mutation.

## Purpose
A mutation is only useful if the user can see the queue change, the card status update and the success or failure result clearly.

## Current Problem
Without refresh and feedback, a successful research enqueue can feel like nothing happened, or worse, can duplicate state in the UI.

## Context
- Construction already improved queue refresh behavior and provides a useful pattern.
- Research should replicate the quality, but not the exact implementation blindly.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`
- Existing Construction enqueue feedback implementation

## Implementation Requirements
1. After successful research enqueue:
   - show success message in Spanish;
   - refresh Research UI state;
   - update the queue panel;
   - update the technology card status;
   - avoid duplicate optimistic entries.
2. If the backend returns created order details, show them.
3. If the backend only returns success, rely on the refreshed read model.
4. If the mutation fails:
   - show a mapped Spanish error;
   - keep previous state visible;
   - allow retry if still valid.
5. Ensure blocked actions remain blocked after refresh.
6. Keep technical response data in diagnostics if that pattern already exists.

## UI/UX Requirements
- Success should be noticeable but not intrusive.
- Errors should explain the next action.
- No raw English backend errors in the primary UI if a readable translation is available.

## Backend/API Requirements
- No backend change expected.
- If the API contract currently returns extra details, consume them without overcoupling the UI.

## Safety Constraints
- Do not retry automatically.
- Do not mutate local resources optimistically.
- Backend remains the source of truth.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx`
- `src/VoidEmpires.Frontend/src/api/researchApi.ts`
- `src/VoidEmpires.Frontend/src/utils/researchPresentation.ts`

## Acceptance Criteria
- Enqueue success updates the queue.
- Enqueue failure is readable.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Manual QA should start one safe research only.
- The refresh path should remain simple and deterministic.
- Avoid turning success handling into a mini state machine unless it is truly needed.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single refresh after confirmed success.
