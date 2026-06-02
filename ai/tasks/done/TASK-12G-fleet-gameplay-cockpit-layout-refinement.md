# TASK-12G

---
id: TASK-12G
title: Phase 12G - Fleet gameplay cockpit layout refinement
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12G"
priority: medium
---

## Goal
Refine the Fleet page layout into a clearer gameplay cockpit with a simple OGame-inspired structure.

## Context
The Fleet cockpit already supports the safe prototype command boundary and has been visually improved, but the screen still reads too much like a technical dashboard in places. This task should reshape the page into a more complete gameplay screen with clear hierarchy, while preserving the current estimate, create, and cancel behavior and keeping prototype-only actions disabled.

## Implementation steps

1. Inspect the Fleet page, summary panel, selected-group panel, styles, and existing fleet API or presentation helpers to understand the current page hierarchy.
2. Rework the layout so the main visible flow is fleet overview, squad list, selected squad, orders, active transfers, resources, and technical panels as secondary content.
3. Make the primary screen feel more like a fleet command view and less like an API or debug dashboard by tightening cards, section titles, and technical detail placement.
4. Keep estimate, create transfer, and cancel transfer behavior intact, while leaving complete-due, split, and merge disabled or prototype-only.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts

## Acceptance criteria

- The Fleet page layout is clearly organized into fleet overview, squad list, selected squad, orders, active transfers, resources, and secondary technical sections.
- The screen feels more like a gameplay cockpit and less like a debug dashboard.
- Existing safe behavior remains intact: estimate works, create transfer works with explicit confirmation, cancel transfer works with explicit confirmation, and complete-due, split, and merge stay disabled or prototype-only.
- No new backend endpoints, 3D rendering, WebSockets, or EF migrations are added.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused and aligned with the current galactic UI direction.
- Do not add new executable commands or enable extra mutations.
- Do not add Three.js, WebGL, polling, or backend contract changes.
- Split follow-up work if the layout refinement exceeds the AI change budget.

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
- If layout work grows too broad, stop and create a follow-up task first.
