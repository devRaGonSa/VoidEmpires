# TASK-12I

---
id: TASK-12I
title: Phase 12I - Fleet order panel gameplay simplification
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12I"
priority: medium
---

## Goal
Simplify the command or order panel so estimate, create transfer, and cancel transfer feel like a straightforward gameplay flow.

## Context
The current order panel works, but it still reads like a technical control block in places. This task should make the gameplay sequence obvious, keep the confirmation boundary intact, and present estimate, cost, and readiness information in a compact format that is easier to read during manual validation.

## Implementation steps

1. Inspect the order panel, selected group flow, destination selection, estimate handling, and create or cancel command presentation.
2. Reframe the panel around a simple gameplay sequence: select squad, select destination, calculate estimate, review cost or travel info, confirm order.
3. Show estimate summary in a compact format with origin, destination, duration or travel info, fuel or resource cost, and readiness or result when available.
4. Keep create transfer and cancel transfer as explicit confirmations, with readable disabled states and short prototype warnings.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The order panel presents a clear sequence for select squad, select destination, estimate, review, and confirm.
- Estimate summary is compact and gameplay-oriented.
- Create transfer and cancel transfer remain explicit confirmations and the disabled state explains why an action is unavailable.
- Complete-due, split, and merge stay disabled or prototype-only.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and simple.
- Do not add complex modals, drag and drop, polling, or backend contract changes.
- Do not enable extra mutation commands.
- Split the work if order-panel simplification grows beyond the budget.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
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
- Prefer a single commit for this task.
- If this panel needs broader refactoring, stop and create a narrower follow-up task first.
