# TASK-34J

---
id: TASK-34J
title: Cockpit post-materialization readiness
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: medium
---

## Goal
Make Construction, Research, and Shipyard read state understandable after backend materialization.

## Context
After due orders are materialized and marked complete, cockpits should present the new backend state coherently without auto-completing or inventing history.

## Implementation steps

1. Review current post-refresh/read-state in Construction, Research, and Shipyard.
2. Ensure completed orders no longer appear as blocking open orders once the backend marks them complete.
3. Show clear Spanish copy for:
   - no active order;
   - last known completion if backend exposes it;
   - available next action if resources and prerequisites allow.
4. Do not invent completion history if the backend does not expose it.
5. Do not auto-call materialization on page load.
6. Do not add instant-complete buttons to normal UI.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Cockpits behave coherently after due materialization.
- Completed orders do not incorrectly block next actions.
- No completion history is invented.
- Frontend build passes.

## Constraints

- Do not fake completion in frontend.
- Do not optimistic-update backend-sourced state.
- Do not auto-call materialization on page load.
- Do not claim visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34J message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
