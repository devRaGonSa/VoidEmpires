# TASK-11L

---
id: TASK-11L
title: Phase 11L - Create transfer confirmation flow
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11L"
priority: medium
---

## Goal
Prepare a controlled frontend confirmation flow for creating an orbital transfer from an existing read-only estimate result.

## Context
The Fleet page already supports the read-only orbital travel estimate flow and shows mutation commands as prototype-only metadata. The next safe step is to prepare a dedicated confirmation flow for `create transfer` so the UI can summarize the estimated route and cost, require explicit acknowledgement, and keep all other mutation commands disabled.

## Implementation steps

1. Inspect the Fleet page, typed fleet command helpers, estimate result handling, and mutation confirmation metadata from the previous block.
2. Add a frontend-only confirmation flow for `create transfer` that depends on an existing valid estimate result plus a known group and destination context.
3. Show the estimated route and cost summary before confirmation and require an explicit interaction before the action can proceed.
4. Use clear Spanish copy such as `Crear transferencia orbital`, `Accion de desarrollo`, `Mutara datos de desarrollo`, and `Requiere confirmacion explicita`.
5. Keep the task limited to confirmation preparation only and do not call the create-transfer endpoint yet.
6. Run the required build and test validation commands before completing the task.

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

- The Fleet page shows a `create transfer` confirmation flow tied to an existing estimate result.
- The UI summarizes route and cost before confirmation and clearly labels the action as development or prototype-only.
- `create transfer` is not executed automatically after estimate completion.
- Cancel, complete-due, split, and merge remain disabled or metadata-only.
- Backend contracts remain unchanged.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused.
- Do not call the create-transfer endpoint in this phase.
- Do not change backend contracts or apply EF migrations.
- Do not introduce production/auth assumptions, combat, interception, or WebSockets.

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
- If the confirmation flow requires broader command execution plumbing, stop and split the extra work into a follow-up task.
