# TASK-19M-defenses-queue-and-complete-due-safe-placeholder

---
id: TASK-19M-defenses-queue-and-complete-due-safe-placeholder
title: Defenses queue and complete due safe placeholder
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: medium
---

## Goal
Display any defense-related queue state in the Defenses cockpit and keep completion behavior conservative through a truthful disabled placeholder unless a safe endpoint already exists.

## Purpose
Align Defenses with the current pattern used by Construction, Research, and Shipyard, where queue visibility is useful but complete-due only becomes interactive when a cockpit-safe path is clearly scoped.

## Current Problem
If defense actions rely on a queue, the player needs to see that state. At the same time, an unsafe or global complete-due action would be misleading and potentially dangerous for the broader dev environment.

## Context
- Research and Shipyard already expose a conservative disabled complete-due placeholder when the backend route is not safely scoped to the cockpit.
- Defenses may need to present either a dedicated defense queue or a filtered view of existing construction queue data.
- The queue section should remain useful even if it is read-only in v1.

## Files to Inspect First
- Defenses read model or service
- construction queue services
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add a queue or readiness-orders panel to the cockpit.
2. Each queue item should show, when supported:
   - defense name
   - action
   - status
   - start or end time
   - cost
   - source planet
3. If the queue is empty, show a Spanish empty state such as `No hay ordenes defensivas en cola.`
4. If the shown queue is actually a filtered construction queue, label it clearly so the user understands the boundary.
5. Complete-due behavior must be:
   - disabled or secondary by default
   - explained when unavailable
   - only enabled if a truly safe cockpit-scoped endpoint already exists
6. Do not auto-complete or imply background execution from the page.
7. Keep raw order ids in diagnostics only.

## UI/UX Requirements
- Queue visibility should help the player, not dominate the page.
- Disabled complete-due controls must not look like the primary next action.
- Spanish copy should explain limitations plainly.

## Backend/API Requirements
- No backend change expected unless the read model lacks the queue metadata already available elsewhere.
- If backend support is extended, keep it read-only or explicitly Development-only and tested.

## Safety Constraints
- No unsafe global complete-due behavior.
- No combat behavior.
- No hidden queue mutation.

## Expected Files to Modify
- Defenses read-model DTO or service files only if queue metadata is missing
- related tests if backend changes are made
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- supporting styles or helper files

## Acceptance Criteria
- Queue panel handles both empty and seeded states.
- Complete-due messaging is truthful and conservative.
- Frontend build passes, plus backend validation if the contract changed.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` if backend changes are made
- `dotnet test --no-build` if backend changes are made

## Notes / Residual Risks
- Queue can remain read-only in v1 as long as the UI communicates that clearly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task limited to queue visibility and safe completion messaging.
