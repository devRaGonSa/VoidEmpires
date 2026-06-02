# TASK-12P

---
id: TASK-12P
title: Phase 12P - Fleet order estimate result card
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12P"
priority: medium
---

## Goal
Improve the estimate result presentation so the order panel shows origin, destination, duration, cost, and readiness in a compact gameplay format.

## Context
The estimate flow is already functional, but the result presentation still needs to feel like part of a game order screen rather than a technical response viewer. This task should make the estimate card clearer, more compact, and more directly useful before the user confirms a transfer.

## Implementation steps

1. Inspect the Fleet page estimate result handling, the command presentation helper, the typed estimate response, and the order panel UI.
2. Present estimate results in a compact gameplay card with origin, destination, squad, estimated duration or travel time, estimated cost, readiness or viability, and warnings or blocked reasons when available.
3. Provide clean fallbacks when fields are not available rather than showing technical text or raw JSON-like output.
4. Make create-transfer confirmation visually depend on the estimate result, keeping the estimate-first review flow obvious.
5. Preserve stale estimate guards, duplicate-submit guards, and cancel transfer behavior, then validate with the standard backend and frontend commands.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- The order panel shows a compact estimate result card with origin, destination, squad, estimated duration or time, cost, and readiness or viability when available.
- Fallbacks are clean and readable when backend fields are missing.
- The create-transfer confirmation clearly follows an estimate-first review flow.
- Stale estimate and duplicate-submit guards remain intact.
- Cancel transfer behavior remains unchanged.
- No polling, WebSockets, or additional executable commands are introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and gameplay-oriented.
- Do not add new backend endpoints.
- Do not enable complete-due, split, or merge execution.
- Split follow-up work if the estimate card needs deeper response-shape changes.

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
- If the estimate card needs broader UX changes, stop and create a follow-up task first.
