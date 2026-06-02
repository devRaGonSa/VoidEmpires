# TASK-11P

---
id: TASK-11P
title: Phase 11P - Create transfer double-submit and stale execution guard
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11P"
priority: medium
---

## Goal
Prevent duplicate create-transfer submissions and stale command execution from the frontend Fleet page.

## Context
The controlled create-transfer path now exists behind explicit confirmation. The next hardening step is to make that path resilient against duplicate clicks, stale estimates, and context changes so the UI cannot accidentally submit a request that no longer matches the current group or destination.

## Implementation steps

1. Inspect the Fleet page create-transfer flow, estimate state, confirmation state, command presentation model, and typed create-transfer API helper.
2. Add guards so the confirmation button is disabled while a request is in flight and repeated clicks cannot issue duplicate POST requests.
3. Reject stale estimates after fleet UI state refreshes, after selected group or destination changes, and when the selected group is no longer available or stationed.
4. Require a live estimate that matches the current group and destination before create-transfer can run.
5. Surface clear user-facing copy for stale and in-flight states.
6. Run the standard backend and frontend validation commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- The create-transfer confirmation button cannot double-submit while a request is active.
- Stale estimates are rejected after context changes or UI-state refreshes.
- Create-transfer cannot run unless the current group, destination, and estimate still match.
- User-facing copy clearly explains stale and in-flight states.
- Cancel, complete-due, split, and merge remain disabled or prototype-only.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused.
- Do not add execution for cancel, complete, split, or merge.
- Do not change backend unless an obvious contract mismatch prevents correct guarding.
- Do not apply EF migrations.
- Add frontend tests only if the existing tooling supports them cheaply.

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
- If the stale-state guard needs broader state management, split the extra work into a follow-up task.
