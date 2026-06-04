# TASK-20C-ground-army-queue-and-complete-due-safe-placeholder

---
id: TASK-20C-ground-army-queue-and-complete-due-safe-placeholder
title: Ground Army queue and complete due safe placeholder
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Display Ground Army training or preparation queue state and keep completion safe.

## Purpose
Expose queue state honestly while avoiding a misleading complete-due button that would suggest unsafe mutation support when none exists.

## Current Problem
If Ground Army actions use a queue, the cockpit must show it. If complete-due is not safely scoped, the UI must show a disabled placeholder, not an active misleading button.

## Context
- Construction, Research, Shipyard, and Defenses keep complete-due disabled or placeholder-only unless safely scoped.
- Ground Army should follow the same standard.

## Files to Inspect First
- Ground Army read model or service
- Construction or training queue services
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add a queue or readiness panel to the Ground Army cockpit.
2. Each queue item should show, where supported:
   - unit, structure, or preparation name
   - action
   - status
   - start and end if available
   - cost if available
   - source planet
3. Show an empty state such as `No hay ordenes terrestres en cola.`
4. If the Ground Army queue is actually a filtered Construction queue, label that relationship clearly.
5. Keep complete-due disabled or secondary unless a safe endpoint exists.
6. Explain why completion is unavailable when the action is disabled.
7. Do not auto-complete anything.
8. Keep raw order ids only in diagnostics.

## UI/UX Requirements
- Spanish-first copy.
- Queue state should be visible but not visually dominant.
- A disabled complete-due control must not look like the primary action.

## Backend/API Requirements
- No backend change is expected unless the read model lacks queue data already present in the domain.

## Safety Constraints
- No unsafe global complete-due.
- No combat.
- No invasion.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army read-model or DTO files if queue fields are missing
- `src/VoidEmpires.Frontend/src/styles.css`
- focused tests only if backend queue shape changes

## Acceptance Criteria
- The queue panel works for empty and seeded states.
- Complete-due behavior is truthful.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made

## Notes / Residual Risks
- The queue can remain read-only in v1 if completion cannot be scoped safely.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on queue visibility and safe completion messaging.
