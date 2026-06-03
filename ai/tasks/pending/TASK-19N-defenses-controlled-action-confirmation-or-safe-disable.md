# TASK-19N-defenses-controlled-action-confirmation-or-safe-disable

---
id: TASK-19N-defenses-controlled-action-confirmation-or-safe-disable
title: Defenses controlled action confirmation or safe disable
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - qa
roadmap_item: "Block 19E-19T - Defenses cockpit playable foundation v1"
priority: high
---

## Goal
Enable controlled defense preparation only when a safe path exists; otherwise disable or hand off actions truthfully.

## Purpose
Make the cockpit useful without bypassing Construction ownership or backend validation, and without pretending every visible defense option can be executed directly from Defenses.

## Current Problem
Later cockpit tasks may expose available defense options, but the route must distinguish between:
- direct safe Defenses action
- Construction-owned handoff
- blocked action
- unavailable feature in this build

Without that separation, the UI would mislead users about what actually mutates state.

## Context
- Construction is the established place for general construction enqueue.
- Research and Shipyard already use explicit confirmations for safe Development-only mutations.
- Defenses may remain mostly read-only if the audit shows no safe direct mutation path.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses API client
- Construction API client
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- construction enqueue tests
- Defenses read model

## Implementation Requirements
1. Determine, for each defense action surfaced in the UI, whether it is:
   - directly confirmable in Defenses
   - owned by Construction and therefore a handoff
   - blocked by requirements or resources
   - unavailable in this build
2. If a safe mutation endpoint exists or can be added safely:
   - require explicit confirmation
   - show planet, defense, cost, duration, and requirements
   - provide `Confirmar defensa` and `Cancelar`
3. If the action belongs to Construction:
   - show a clear `Abrir Construccion` handoff with preserved context
4. If no safe support exists:
   - keep the action disabled with a truthful Spanish explanation
5. Cancel must not mutate anything.
6. Confirm must call only the safe endpoint.
7. Refresh cockpit state after success.
8. On failure, surface mapped Spanish errors rather than raw backend messages.

## UI/UX Requirements
- Available actions must be visually distinct from blocked or handoff-only states.
- Confirmation must feel deliberate and consistent with other cockpit mutation flows.
- Spanish copy should emphasize preparation and readiness, not battle execution.

## Backend/API Requirements
- If adding a mutation route, keep it Development-only and fully tested.
- Reuse existing validation rules when possible.
- Do not weaken backend validation just to make the UI more permissive.

## Safety Constraints
- No combat behavior.
- No hidden global mutation.
- No optimistic queue updates.
- No direct Fleet or Galaxy behavior changes.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx`
- Defenses frontend API and helper files
- backend dev endpoint and service files only if a safe direct mutation path is introduced
- related backend and frontend tests as needed

## Acceptance Criteria
- Defenses actions are either safely confirmable, clearly handed off, blocked with reasons, or disabled as unavailable.
- Frontend build passes.
- Backend tests pass if a mutation path is added.

## Validation
- `dotnet build --no-restore` if backend changes are made
- `dotnet test --no-build` if backend changes are made
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- It is acceptable for Defenses v1 to remain mostly read-only with Construction handoff if that is the safest truthful outcome.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split new mutation support into a conservative path only; do not broaden the module.
