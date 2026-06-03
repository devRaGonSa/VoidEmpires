# TASK-17M-shipyard-enqueue-success-feedback-and-state-refresh

---
id: TASK-17M-shipyard-enqueue-success-feedback-and-state-refresh
title: Shipyard enqueue success feedback and state refresh
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Ensure a successful Shipyard enqueue produces visible success feedback and refreshes the cockpit state consistently.

## Purpose
Make the mutation legible to the player so the result of confirmation is immediately visible in the queue, stock, and action states.

## Current Problem
A successful submit is not enough if the page stays visually stale. Research previously needed extra alignment so the queue visibly updated after submit. Shipyard needs that feedback loop from the start.

## Context
- Shipyard queue and catalog sections should already exist by the time this task runs.
- The backend may spend or reserve resources during enqueue, so the summary area may also need to refresh.
- UI feedback should remain calm and informational.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Shipyard API client and mapping helpers
- Shipyard backend response or read-model contracts
- Any relevant enqueue endpoint tests

## Implementation Requirements
1. After a successful enqueue:
   - show a visible Spanish success message;
   - refresh Shipyard UI state;
   - update queue count;
   - render the new queue item;
   - refresh card availability if backend state changed;
   - refresh resources if the backend spends or reserves them.
2. Avoid duplicate optimistic queue items when the backend already confirms the result.
3. If the refresh fails after a confirmed submit, preserve the success acknowledgment and show a secondary warning.
4. Prevent double submit while the request is in flight.
5. Keep technical response data in diagnostics only.

## UI/UX Requirements
- Success feedback should be visible but not intrusive.
- Queue updates must be easy to spot.
- Spanish-first copy throughout the flow.

## Backend/API Requirements
- No backend change is expected unless the response or read model lacks data needed to refresh truthfully.
- If backend changes are required, keep them narrow and covered by tests.

## Safety Constraints
- No optimistic resource spending.
- No duplicate enqueue from repeated clicks.
- Do not display speculative queue state if refresh failed before confirming the result.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/api/` Shipyard client files
- `src/VoidEmpires.Frontend/src/utils/` Shipyard view-model or feedback helpers
- Narrow backend or test files only if one missing contract field blocks truthful refresh behavior

## Acceptance Criteria
- One successful Shipyard enqueue visibly updates the cockpit state.
- The queue and summary reflect the new state after refresh.
- Frontend build passes and backend tests still pass if backend changes are made.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet test --no-build` if backend files are changed.

## Notes / Residual Risks
- Manual QA should confirm that only one queue item appears after one submit.
- Keep this task focused on post-submit feedback rather than broader styling changes.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Split extra polish into later tasks if state-refresh handling becomes large.
