# TASK-11Y

---
id: TASK-11Y
title: Phase 11Y - Fleet selected group command panel
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11Y"
priority: medium
---

## Goal
Create or improve a selected-group command panel that makes the current Fleet command context understandable before estimate, create, or cancel actions are run.

## Context
The current frontend flow can execute narrow Fleet commands, but the selected group context and action readiness are still too implicit. This task should make the active group, destination, latest estimate, confirmation state, and guardrails more legible without expanding the allowed command surface.

## Implementation steps

1. Inspect the Fleet page state, typed API models, and current selected-group or command presentation helpers.
2. Add or refine selected-group state and presentation so the panel shows asset type, quantity, status, current or origin planet, active transfer summary when present, readiness, and compact secondary technical identifiers.
3. Improve the command panel so estimate context, selected destination, latest estimate result, create confirmation state, and cancel confirmation state are visually clear and grouped together.
4. Strengthen UX guardrails by showing why disabled actions are unavailable, keeping stale estimate warnings visible, and making in-flight request state obvious.
5. Preserve the command boundary: only estimate, create transfer, and cancel transfer may remain executable, while complete-due, split, and merge stay disabled or prototype-only.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- The selected-group panel clearly shows the current command context, including labels for asset type, quantity, readiness, current location, origin, and active transfer information when available.
- The command panel clearly shows estimate context, destination, latest estimate state, and explicit create or cancel confirmation requirements.
- Disabled actions explain why they are disabled, stale estimate warnings remain visible, and in-flight request state is clearly communicated.
- No new executable mutation commands are added beyond create and cancel transfer.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused; only allow tiny contract typing fixes if a mismatch is discovered.
- Do not add authentication, 3D rendering, new backend APIs, or broader command execution.
- Do not enable complete-due, split, or merge from the frontend.
- Keep the changes within the repository AI change budget and split further if the panel work becomes too broad.

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
- If selected-group UX improvements exceed the budget, split the extra work into a follow-up task.
