# TASK-12D

---
id: TASK-12D
title: Phase 12D - Fleet cockpit technical ID demotion
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12D"
priority: medium
---

## Goal
Reduce the visual prominence of GUIDs and technical IDs in the Fleet cockpit while keeping them available as secondary development metadata.

## Context
Manual visual review found that technical identifiers still dominate too many areas of the Fleet cockpit, making the page feel more like a dev console than a simple playable fleet screen. This task should promote friendly fleet and planet labels while keeping IDs accessible in muted or secondary form for development use.

## Implementation steps

1. Inspect all Fleet cockpit areas where technical IDs appear, including group cards, selected-group details, planet labels, transfer status, command results, and guarded mutation areas.
2. Rework display priority so friendly names are primary while short IDs or development identifiers are shown only as secondary metadata.
3. Avoid repeating full IDs in places where they do not add immediate value, especially next to friendly planet or group names.
4. Use the existing style system to mute technical metadata without removing it entirely from the development UI.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Friendly names such as ship or planet labels are visually primary across the Fleet cockpit.
- Short IDs may remain available as muted secondary metadata, but full GUIDs no longer dominate cards, labels, or result areas.
- Repeated technical identifiers are reduced where they do not materially help development workflows.
- No backend contracts, new API endpoints, or additional executable mutations are introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and styling-oriented.
- Do not remove useful development metadata entirely.
- Do not change backend contracts, add endpoints, or enable additional mutations.
- Split the work if identifier cleanup grows beyond the Fleet cockpit budget.

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
- If ID demotion requires broader presentation refactors, stop and create a follow-up task first.
