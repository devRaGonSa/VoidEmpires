# TASK-12H

---
id: TASK-12H
title: Phase 12H - Fleet squad list OGame-style presentation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12H"
priority: medium
---

## Goal
Improve the squad or orbital-group list so it reads like an OGame-style fleet list with clear operational state.

## Context
The current squad rail is useful, but it still carries too much technical weight for a gameplay screen. This task should make each squad card compact, scannable, and stateful so seeded squads can be read at a glance without hiding important status or development metadata.

## Implementation steps

1. Inspect the fleet group rail or list, selected state handling, and any shared presentation helpers that feed squad card labels and statuses.
2. Rework each squad card to prioritize squad type or name, quantity, current planet, status, active destination if in transfer, and primary readiness or action state.
3. Use game-like Spanish labels for the list and card details while keeping technical ids as secondary metadata only.
4. Make selected or active state obvious and keep cards compact enough that multiple seeded squads remain easy to scan.
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

## Acceptance criteria

- The squad list is compact, scannable, and reads like an OGame-style fleet list.
- Each squad card emphasizes type or name, quantity, location, destination, status, and readiness, with technical ids demoted to secondary metadata.
- Selected and active states are clearly visible.
- Important status is not hidden, and complete-due, split, and merge remain disabled or prototype-only.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused.
- Do not remove technical ids entirely.
- Do not enable new mutations or change backend contracts.
- Split follow-up work if the list presentation exceeds the change budget.

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
- If squad list changes require broader layout work, stop and split the rest into another task.
