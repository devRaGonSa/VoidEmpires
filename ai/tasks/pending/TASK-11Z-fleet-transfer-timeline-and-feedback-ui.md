# TASK-11Z

---
id: TASK-11Z
title: Phase 11Z - Fleet transfer timeline and feedback UI
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11Z"
priority: medium
---

## Goal
Improve the visual presentation of active transfers, command results, and operation feedback in the Fleet cockpit UI.

## Context
The frontend already exposes active-transfer and command-result data, but too much of that state still reads as low-level development output instead of useful operational feedback. This task should make transfer status and result messaging readable and visible while staying within the current prototype scope and avoiding new live-update behavior.

## Implementation steps

1. Inspect the Fleet page, presentation helpers, and current transfer or command result state to understand what data is already available.
2. Add or improve a transfer or status panel that highlights active transfer counts, involved group, route context, timestamps, progress when present, and cancel readiness when available.
3. Add or improve a feedback area that clearly presents the latest estimate, create transfer, and cancel transfer results using readable success, warning, and error states instead of raw JSON-first rendering.
4. Use readable Spanish copy for refreshed-state indicators plus network, conflict, and not-found errors while keeping technical details secondary or collapsed.
5. Preserve current behavior by avoiding polling, WebSockets, combat or interception execution, and additional mutation controls.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The Fleet UI includes a readable transfer or status panel showing active transfer context, route details, timestamps, and progress or cancel readiness when present.
- The feedback area clearly shows the latest estimate, create transfer, and cancel transfer outcomes with readable success, warning, and error states.
- Network, conflict, and not-found failures are rendered with readable Spanish copy, and raw JSON is no longer the primary display.
- No complete-due execution, polling, WebSockets, combat, interception, or 3D features are added.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and limited to data already available in Fleet UI state.
- Do not add real-time refresh infrastructure, new backend endpoints, or new executable commands.
- Keep technical debug details secondary rather than removing them entirely.
- Split the work into a follow-up task if transfer-feedback improvements exceed the repository AI change budget.

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
- If improved transfer feedback requires broader UI restructuring, stop and create a smaller follow-up task first.
