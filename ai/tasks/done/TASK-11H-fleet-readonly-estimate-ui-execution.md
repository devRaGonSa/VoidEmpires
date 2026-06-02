# TASK-11H

---
id: TASK-11H
title: Phase 11H - Fleet read-only estimate UI execution
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11H"
priority: medium
---

## Goal
Allow the frontend Fleet page to execute the read-only orbital travel estimate command safely from selected seeded or development fleet data.

## Context
The current Fleet page loads development UI state and action-manifest metadata, but it does not execute any command routes. The next safe step is to enable only the read-only estimate flow so the frontend can preview route, duration, fuel-readiness, and affordability data without creating transfers or mutating gameplay state.

## Implementation steps

1. Inspect the current Fleet page, typed fleet command client helpers, and estimate result presentation helper to understand the existing read-only command path.
2. Add a simple estimate interaction on the Fleet page or a focused child component that lets the user choose an eligible group and a destination planet from seeded or known development context.
3. Call `POST /api/dev/fleets/orbital-travel/estimate` through the typed frontend API helper and render loading, success, validation, not-found, conflict, and network-error states using the existing presentation model.
4. Keep the estimate flow explicitly labeled as `Estimacion`, `Solo lectura`, and `No ejecuta movimiento`, and do not add any mutation execution controls.
5. Run the required backend and frontend validation commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- The Fleet page can execute the read-only travel estimate command from development or seeded fleet context.
- The UI shows loading, success, validation, not-found, conflict, and network-error states clearly.
- The estimate flow stays read-only and does not create transfers, mutate groups, or change resource balances.
- No mutation execution buttons are added for create, cancel, complete, split, or merge commands.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused; only allow tiny backend changes if a contract mismatch is discovered.
- Do not add authentication, 3D visuals, WebSockets, or production gameplay execution UI.
- Preserve the current simple visual style and development-only posture.
- Keep mutation commands protected behind explicit prototype guardrails.

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
- If the UI estimate flow needs broader restructuring, stop and create a follow-up task before exceeding the budget.
