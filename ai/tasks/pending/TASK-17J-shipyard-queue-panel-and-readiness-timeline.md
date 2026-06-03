# TASK-17J-shipyard-queue-panel-and-readiness-timeline

---
id: TASK-17J-shipyard-queue-panel-and-readiness-timeline
title: Shipyard queue panel and readiness timeline
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
Expose the current asset production queue in a readable Shipyard panel that shows order status, timing, and readiness without pretending that due-completion support already exists.

## Purpose
Make queue state visible so Shipyard feels operational and so later enqueue feedback has somewhere meaningful to land.

## Current Problem
If Shipyard can only show a catalog and top-level summary, the player still cannot tell whether a planet is already building something or whether a new enqueue changed anything. The queue needs a dedicated panel with clear due-state language and truthful completion affordances.

## Context
- Construction and Research both use queue-based cockpit sections.
- Shipyard queues represent orbital asset production, which can later feed stock or orbital groups.
- Completion behavior may remain disabled if the backend completion path is not safely scoped.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Shipyard read-model and view-model files
- `src/VoidEmpires.Frontend/src/styles.css`
- Any backend queue DTOs or tests if queue status lacks needed fields

## Implementation Requirements
1. Add a queue section that renders current production orders clearly.
2. Each queue item should show, when available:
   - asset name;
   - quantity or output;
   - status;
   - submitted or due timing;
   - blocked or due indicator if applicable.
3. Render a truthful empty-state message when the queue is empty.
4. If the backend exposes due items but completion is not safely callable, show that state without turning it into an active mutation affordance.
5. Keep technical timing and ids secondary if they must be shown at all.
6. Ensure the queue panel can visibly update after a later enqueue task.

## UI/UX Requirements
- Spanish-first queue labels and empty states.
- The panel should make order progression readable without feeling like a log dump.
- Use restrained diagnostics and avoid alert fatigue.

## Backend/API Requirements
- Reuse the existing queue data if present.
- Only add backend fields if the current contract prevents a truthful queue display.

## Safety Constraints
- No automatic completion.
- No hidden mutation.
- Do not imply that due items were applied if they are merely detectable.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/` Shipyard queue mapping helpers
- `src/VoidEmpires.Frontend/src/styles.css`
- Narrow backend DTO or test files only if queue state needs one missing display field

## Acceptance Criteria
- Shipyard has a readable queue panel with honest empty and due states.
- The panel is ready to reflect enqueue refreshes later in the block.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend files are touched.

## Notes / Residual Risks
- Completion support can remain disabled in v1 if it is not planet-scoped and safe.
- Keep the queue section structurally independent from the catalog so later refresh logic is straightforward.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Avoid pulling completion logic into this display-only task.
