# TASK-13F

---
id: TASK-13F
title: Phase 13F - Strategic map system selection and detail panel
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13E-13J"
priority: medium
---

## Goal
Add or improve selected-system behavior so the user can focus a system and understand its strategic state.

## Context
The strategic map should let the user inspect a specific system without inventing new data or changing the backend shape. This task focuses on selection and the detail panel that follows it.

## Implementation steps

1. Review the existing strategic map response data and selection flow.
2. Add or refine system selection from the 2D map and/or system list.
3. Present the selected system with concise Spanish labels and secondary technical metadata only.
4. Show a compact empty state when no system is selected.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers
- shared shell/sidebar/header components used by the strategic map

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers, if needed
- shared shell/sidebar/header components used by the strategic map

## Acceptance criteria

- The selected system panel shows the system state clearly.
- Spanish labels are used for the primary display.
- IDs are secondary metadata only.
- The empty state is compact and readable.
- No backend data is invented.
- No new mutation commands are added.

## Constraints

- Do not add 3D.
- Do not add mutation commands.
- Do not change backend unless a tiny typing mismatch blocks the UI.
- Keep the change minimal.

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
- Split the work into additional tasks if limits are exceeded.
