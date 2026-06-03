# TASK-16T-research-enqueue-backend-validation-alignment

---
id: TASK-16T-research-enqueue-backend-validation-alignment
title: Research enqueue backend validation alignment
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 16Q-17B - Research enqueue contract alignment and usable flow closure"
priority: high
---

## Goal
Align backend read availability and backend enqueue validation so the same seeded item cannot be available in read state and rejected during enqueue.

## Purpose
The core defect is a disagreement between read and mutation logic. The backend says an item is `Disponible`, but the enqueue endpoint rejects it. That is not acceptable for a playable cockpit.

## Current Problem
The Research cockpit is now visually truthful, but the backend still rejects a supposedly available item. That means the backend needs either shared validation or a clear contract adjustment so the read model and enqueue path use the same rules.

## Context
- The backend should use the same or equivalent validation logic for Research availability and Research enqueue.
- The problem may involve resource scope, planet ownership, queue capacity, technology key, target level, prerequisites, or existing in-progress/completed state.
- This task is the most important backend task in the block.

## Files to Inspect First
- `src/VoidEmpires.Application/Research/`
- `src/VoidEmpires.Infrastructure/Research/`
- `src/VoidEmpires.Web/`
- `tests/VoidEmpires.Tests/`

## Implementation Requirements
1. Locate the validation logic used by the read model.
2. Locate the validation logic used by enqueue.
3. Identify the differences between them in:
   - resource stockpile scope;
   - planet ownership;
   - queue capacity;
   - technology id or key;
   - target level;
   - prerequisites;
   - completed or in-progress state.
4. Refactor to share validation where appropriate.
5. If shared validation would be too large, add focused alignment logic and tests.
6. Add a test that:
   - applies the `minimal-validation` seed;
   - reads Research UI state;
   - picks the first item marked available;
   - submits enqueue using its command metadata;
   - asserts success.
7. Add a test that:
   - picks a blocked item;
   - asserts enqueue rejects with the expected reason.
8. Keep error messages consistent with the frontend mapping.

## UI/UX Requirements
- No direct UI changes are required for the backend alignment itself.
- The backend should provide clear codes or reasons that the frontend can translate into Spanish.

## Backend/API Requirements
- This is backend work.
- Maintain Development-only endpoint gating.
- Do not weaken validation just to match the current seed.
- Prefer a shared validation path if the architecture allows it.

## Safety Constraints
- No production auth.
- No real technology effects beyond current research queue state.
- No migrations unless absolutely necessary.
- Do not create a second, divergent validation model.

## Expected Files to Modify
- Backend Research validation or command handler code.
- Backend tests proving both success and rejection.
- Potentially a small DTO or result-code adjustment if the two paths need a shared contract.

## Acceptance Criteria
- Available read item enqueues successfully in tests.
- Blocked item remains rejected.
- `dotnet test --no-build` passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend contracts change.

## Notes / Residual Risks
- This is the most important task of the block.
- If the backend exposes one behavior through the read model and a different one through the enqueue command, fix the source of truth rather than papering over the mismatch.
- The end state should be that the backend and frontend agree on what `Disponible` means.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Prefer a shared validation helper over duplicated rules.
