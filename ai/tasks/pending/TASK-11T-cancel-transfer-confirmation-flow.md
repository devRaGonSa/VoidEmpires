# TASK-11T

---
id: TASK-11T
title: Phase 11T - Cancel transfer confirmation flow
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11T"
priority: medium
---

## Goal
Prepare a controlled frontend confirmation flow for cancelling an active orbital transfer.

## Context
The Fleet page can already create transfers through an explicit confirmation flow, but canceling an active transfer is still disabled from the UI. The next safe step is to prepare a cancel-only confirmation surface that summarizes the active transfer, warns that development data will change, and keeps complete-due, split, and merge disabled.

## Implementation steps

1. Inspect the Fleet page, active transfer rendering, typed fleet command helpers, command presentation model, create-transfer confirmation flow, and mutation confirmation metadata.
2. Add a cancel-transfer confirmation flow that requires a visible active transfer context and a known transfer id.
3. Show a summary of the transfer to cancel, including transfer id, group id, origin, current planet, destination, and arrival or progress when available.
4. Use clear Spanish copy such as `Cancelar transferencia orbital`, `Accion de desarrollo`, `Mutara datos de desarrollo`, `No reembolsa recursos`, and `Requiere confirmacion explicita`.
5. Keep the task limited to confirmation preparation only and do not call the cancel endpoint yet.
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
- src/VoidEmpires.Frontend/src/components/ActionManifestPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- The Fleet page shows a cancel-transfer confirmation flow tied to a visible active transfer.
- The UI summarizes the transfer to cancel before confirmation and clearly labels the action as development or prototype-only.
- `cancel transfer` is not executed automatically.
- `complete-due`, `split`, and `merge` remain disabled or metadata-only.
- Create-transfer behavior remains preserved.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused.
- Do not call the cancel endpoint in this phase.
- Do not change backend contracts or apply EF migrations.
- Do not add execution for complete-due, split, or merge.
- Preserve the existing create-transfer path.

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
- If the cancel confirmation flow requires broader command execution plumbing, stop and split the extra work into a follow-up task.
