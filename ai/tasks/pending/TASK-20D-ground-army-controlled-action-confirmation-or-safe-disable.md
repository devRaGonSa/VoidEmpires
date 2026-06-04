# TASK-20D-ground-army-controlled-action-confirmation-or-safe-disable

---
id: TASK-20D-ground-army-controlled-action-confirmation-or-safe-disable
title: Ground Army controlled action confirmation or safe disable
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Enable controlled Ground Army preparation or training only if safe; otherwise disable actions truthfully.

## Purpose
Make the cockpit useful without allowing it to bypass Construction ownership, backend validation, or the current no-combat and no-invasion boundaries.

## Current Problem
Ground Army should feel useful, but mutating terrestrial actions must not bypass Construction or backend validation. If no safe ground-specific mutation exists, actions must hand off to the correct module or stay disabled.

## Context
- Construction is the known place for building-related work.
- Ground Army may either show military or terrestrial construction options and hand off to Construction, or use a safe Development-only endpoint if one already exists or can be added safely.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army API client
- Construction API client
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- Construction enqueue tests
- Ground Army read model

## Implementation Requirements
1. Determine for each Ground Army action whether it is:
   - directly available in Ground Army
   - a Construction handoff
   - blocked
   - unavailable in this build
2. If a safe mutation exists:
   - require explicit confirmation
   - show planet, unit or preparation, cost, duration, and requirements
   - expose `Confirmar preparacion` and `Cancelar`
3. If an action belongs to Construction:
   - show `Abrir Construccion` with preserved context
4. If no safe support exists:
   - keep the action disabled with an honest explanation
5. Cancel must not mutate.
6. Confirm must call only a safe endpoint.
7. Refresh state after successful confirmation.
8. On failure, show mapped Spanish errors rather than raw backend payloads.

## UI/UX Requirements
- Available actions must be clear.
- Blocked and handoff states must not be misleading.
- Primary copy must be Spanish.

## Backend/API Requirements
- If an endpoint is added, it must be Development-only and covered by tests.
- Do not weaken backend validation or reuse an unsafe generic mutation surface.

## Safety Constraints
- No combat.
- No invasion.
- No hidden global mutation.
- No optimistic queue assumptions.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army API client files
- `src/VoidEmpires.Web/DevEndpointMappings.cs` only if a safe endpoint is added
- focused backend service and test files only if mutation support is truly safe

## Acceptance Criteria
- Ground Army actions are either safely confirmable or truthfully disabled or handed off.
- Frontend build passes.
- Backend tests pass if a mutation path is added.

## Validation
- `dotnet build --no-restore` if backend changes are made
- `dotnet test --no-build` if backend changes are made
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- It is acceptable for Ground Army v1 to remain mostly read-only with handoffs if no mutation path can be proven safe.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task narrow and safety-first.
