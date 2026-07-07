# TASK-41AG

---
id: TASK-41AG
title: Hide materialization controls from product UI
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Ensure resource/queue materialization controls are not visible in normal product UI.

## Context
Materialization is an internal/operator capability and should not look like a player action.

## Implementation steps

1. Search for resource accrual, queue materialization, complete-due, and time-advance controls.
2. Hide controls such as "Aplicar 1 h" and "Actualizar colas vencidas" by default.
3. Expose them only in operator mode if retained.
4. Keep backend behavior unchanged.
5. Ensure copy regression guard captures forbidden visible labels where appropriate.

## Files to read first

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Materialization controls are not visible in product mode.
- Operator mode remains explicit and hidden by default.
- Backend behavior is unchanged.
- Copy regression guard passes.

## Constraints

- Do not remove internal endpoints or scripts.
- Do not change queue/resource gameplay semantics.
- Do not fake completed queue state.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

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
