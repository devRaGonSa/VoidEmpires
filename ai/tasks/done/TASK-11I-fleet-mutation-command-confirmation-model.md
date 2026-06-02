# TASK-11I

---
id: TASK-11I
title: Phase 11I - Fleet mutation command confirmation model
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11I"
priority: medium
---

## Goal
Prepare a reusable frontend confirmation model for mutation fleet commands without executing mutation endpoints from the UI.

## Context
After the read-only estimate flow exists, the frontend needs a structured way to describe future mutation commands without making them active. A confirmation model keeps create, cancel, complete-due, split, and merge actions clearly marked as prototype-only work and helps the UI explain why explicit confirmation will be required later.

## Implementation steps

1. Review the current fleet readiness presentation and action-manifest metadata to identify where mutation command summaries already exist.
2. Add a reusable frontend presentation or confirmation model for create transfer, cancel transfer, complete due, split group, and merge groups.
3. Include command key, command label, prototype or danger level, mutation summary, required confirmation text or flag, and disabled reason when execution is not allowed.
4. Integrate the model into the Fleet page as metadata only so future confirmation requirements are visible without calling mutation endpoints.
5. Validate the changes with the standard backend and frontend build and test commands.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetTypes.ts
- src/VoidEmpires.Frontend/src/api/actionManifestTypes.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/fleetTypes.ts
- src/VoidEmpires.Frontend/src/components/ActionManifestPanel.tsx
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- A reusable confirmation model exists for the mutation fleet commands.
- The Fleet page shows mutation confirmation requirements as metadata only.
- Mutation endpoints are not executed from the UI.
- Read-only estimate behavior remains separate from mutation command preparation.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused.
- Do not change backend contracts.
- Do not mutate development data from the UI.
- Do not add authentication or manual visual validation requirements.

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
- If the confirmation model starts spreading into broader command execution logic, split the remainder into a follow-up task.
