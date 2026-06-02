# TASK-11U

---
id: TASK-11U
title: Phase 11U - Cancel transfer UI execution
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11U"
priority: medium
---

## Goal
Enable the frontend Fleet page to execute cancel orbital transfer through the typed API helper behind the explicit confirmation flow.

## Context
The cancel-transfer confirmation surface should now be able to perform the actual dev-only cancellation request. This phase should wire only `POST /api/dev/fleets/orbital-transfers/cancel`, keep the action explicitly confirmed, and preserve the current create-transfer path while leaving complete-due, split, and merge disabled.

## Implementation steps

1. Inspect the current cancel-transfer confirmation flow, active transfer context, typed cancel-transfer API helper, and command presentation model.
2. Wire the Fleet page so cancel-transfer execution can happen only when a visible active transfer exists and the user explicitly confirms.
3. Call `POST /api/dev/fleets/orbital-transfers/cancel` through the typed API helper and show loading, success, validation, not-found, conflict, and network-error outcomes.
4. Refresh fleet UI state after a successful cancel and clear or mark stale the cancelled transfer context.
5. Preserve the create-transfer execution path and keep complete-due, split, and merge disabled or prototype-only.
6. Run the required backend and frontend validation commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- The Fleet page can execute only `cancel transfer` through the typed frontend API helper.
- Execution requires an explicit confirmation flow and never happens automatically.
- The UI shows loading, success, validation, not-found, conflict, and network-error states clearly.
- Fleet UI state refreshes after success and the cancelled transfer context is cleared or marked stale.
- Create-transfer behavior remains preserved and complete-due, split, and merge stay disabled or prototype-only.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused; only allow tiny backend changes if a response contract mismatch is discovered.
- Do not add combat, interception execution, WebSockets, or production authentication assumptions.
- Do not apply EF migrations.
- Add frontend tests only if existing tooling supports them cheaply; otherwise rely on TypeScript and build validation.
- Keep the no-refund rule visible if that remains the documented behavior.

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
- If safe cancel execution requires broader mutation infrastructure, stop and create a follow-up task before exceeding the budget.
