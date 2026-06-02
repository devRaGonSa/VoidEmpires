# TASK-12K

---
id: TASK-12K
title: Phase 12K - Fleet technical and dev panels collapse
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12K"
priority: medium
---

## Goal
Demote technical or dev manifests and metadata so they remain accessible but no longer dominate the Fleet gameplay screen.

## Context
The Fleet cockpit should feel like a gameplay screen first and a development surface second. This task should push technical route and manifest detail out of the main visual path, while keeping the information available for debugging and prototype support without introducing complex UI libraries.

## Implementation steps

1. Inspect the action manifest rendering, prototype mutation manifest, technical route or action panels, and any dev-only metadata sections in the Fleet page.
2. Move technical content below gameplay panels, collapse it by default if simple state makes sense, or group it under a clear development details label.
3. Keep route names, HTTP verbs, action keys, API paths, and long contract descriptions visually secondary.
4. Preserve action manifest functionality and keep complete-due, split, and merge visible as prototype or future actions without making them look primary.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Technical and dev panels are visually secondary to the gameplay cockpit.
- Collapsed or grouped technical content remains accessible without dominating the screen.
- Route names, verbs, action keys, API paths, and contract descriptions no longer dominate the main gameplay area.
- Complete-due, split, and merge stay prototype-only and secondary.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task frontend-focused and avoid complex accordion libraries.
- Do not remove safety or guardrail information.
- Do not change backend behavior or add new endpoints.
- Use simple React state and CSS only if collapsible behavior is added.

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
- If technical panel collapse requires broader restructuring, stop and create a follow-up task first.
