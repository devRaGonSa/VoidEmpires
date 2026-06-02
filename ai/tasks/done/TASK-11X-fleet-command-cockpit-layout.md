# TASK-11X

---
id: TASK-11X
title: Phase 11X - Fleet command cockpit layout
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11X"
priority: medium
---

## Goal
Refactor the Fleet page into a clearer command cockpit layout suitable for manual visual validation after the block is complete.

## Context
The current Fleet page already supports read-only estimate execution and controlled create or cancel transfer commands, but the overall presentation remains too fragmented for a substantial UI validation milestone. This task should reorganize the page into a stronger development command cockpit while preserving the current execution boundaries and prototype-only restrictions.

## Implementation steps

1. Inspect the Fleet page, the summary panel, shared styles, and the current frontend command state types to understand the present layout and data dependencies.
2. Reorganize the Fleet page into clear cockpit sections for top summary, orbital group list, selected group details, command execution, transfer status, and prototype action manifest context.
3. Improve hierarchy, section titles, card readability, spacing, and technical detail density so operational context is easier to scan without removing important development information.
4. Keep existing behavior intact: estimate remains read-only, create transfer still requires explicit confirmation, cancel transfer still requires explicit confirmation, and complete-due, split, and merge remain disabled or prototype-only.
5. Validate the repository with backend and frontend build or test commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The Fleet page is reorganized into a clearer cockpit layout with distinct summary, list, selected-group, command, transfer, and prototype sections.
- Visual hierarchy is improved with stronger section labels, readable cards, compact technical details, and reduced raw GUID prominence.
- Existing command behavior remains unchanged: estimate works, create requires confirmation, cancel requires confirmation, and complete-due, split, or merge remain disabled or prototype-only.
- No backend endpoints, migrations, 3D rendering, or new mutation commands are added.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused and aligned with the current galactic or Figma-inspired visual direction.
- Do not add Three.js, WebGL, EF migrations, or new backend routes.
- Do not enable complete-due, split, or merge execution from the frontend.
- Keep the task within the repository AI change budget; split follow-up work if the cockpit restructuring grows too large.

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
- If layout restructuring exceeds the budget, stop and create a focused follow-up task before continuing.
