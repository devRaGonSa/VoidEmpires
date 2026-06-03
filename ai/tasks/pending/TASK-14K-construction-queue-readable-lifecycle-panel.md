# TASK-14K

---
id: TASK-14K
title: Phase 14K - Construction queue readable lifecycle panel
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Improve the construction queue panel for Planet and Construction.

## Context
The queue panel should clearly show the lifecycle of active orders instead of exposing raw identifiers or sparse technical state. It also needs a clear empty state and a safe explanation when completion is not available from the current cockpit.

## Implementation steps

1. Review the current queue data available on the Planet screen.
2. Render queue items with readable lifecycle details.
3. Add a Spanish empty state.
4. Keep complete-due disabled or clearly explained unless a safe planet-scoped path exists.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- queue panel styles or helpers, if needed

## Acceptance criteria

- Queue items show building name, target level, status, and timing when available.
- Empty state reads `No hay construcciones en cola.`
- Complete-due remains disabled or placeholder unless safe.
- Raw order IDs remain diagnostics-only.

## Constraints

- Keep status text Spanish and player-readable.
- Do not add unsafe queue mutations.
- If the backend only supports a global completion action, explain that clearly in Spanish.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
