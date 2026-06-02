# TASK-11Q

---
id: TASK-11Q
title: Phase 11Q - Create transfer error and result feedback
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11Q"
priority: medium
---

## Goal
Improve create-transfer success, error, and result feedback so frontend state is clear after every outcome.

## Context
The first controlled mutation path needs stronger feedback now that it can execute. The Fleet page should make success, validation errors, not found, conflicts, network failures, and unexpected response shapes easy to distinguish while keeping technical detail secondary.

## Implementation steps

1. Inspect the current create-transfer result rendering, command presentation model, Fleet page error handling, and backend response statuses.
2. Improve success feedback so the UI clearly states that development state mutated and shows transfer id, group, and destination when available.
3. Improve error feedback for validation, not found, conflict, network failure, and unexpected response shape cases.
4. Ensure conflict messaging explains likely stale state or an already-active transfer.
5. Keep failed mutations from clearing valid fleet state unless a refresh succeeds explicitly.
6. Run the required backend and frontend validation commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- Success feedback clearly states that development state mutated.
- Success feedback shows transfer id, group, and destination when available.
- Validation, not found, conflict, network failure, and unexpected response cases are easy to distinguish.
- Failed mutations do not mark the action successful or wipe valid state prematurely.
- Refreshed state is clearly indicated when available.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused.
- Do not add cancel, complete, split, or merge execution.
- Do not add a new backend endpoint.
- Do not apply EF migrations.
- Keep technical detail secondary or collapsible if the current UI pattern supports it.

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
- If result feedback needs broader response-shape work, split the extra work into a follow-up task.
