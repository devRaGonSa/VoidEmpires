# TASK-12Q

---
id: TASK-12Q
title: Phase 12Q - Fleet cockpit width and spacing polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12Q"
priority: medium
---

## Goal
Improve Fleet cockpit spacing, width, density, and readability on desktop so the page uses available space better without becoming complex.

## Context
The Fleet screen is close to the right experience, but the wide-screen layout still leaves too much empty space and makes some panels feel less intentional than they should. This task should tighten the spatial rhythm, improve panel proportions, and make the whole cockpit feel more balanced without changing the visual identity or introducing a new layout system.

## Implementation steps

1. Inspect the Fleet page at common desktop widths and review the current max-width behavior, content proportions, and spacing rhythm.
2. Improve layout balance across the main grid, rail, detail panels, order panel, and supporting sections so the page uses available space more effectively.
3. Tune card padding, spacing, and selected-state emphasis so the cockpit stays readable from roughly 67% to 100% browser zoom.
4. Keep the left navigation stable and preserve the current color and visual identity while making the page feel less squeezed.
5. Validate the repository with the standard backend and frontend commands before completing the task.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- The Fleet cockpit uses desktop width more effectively and no longer feels overly squeezed or overly sparse.
- Card padding, vertical rhythm, selected-state highlight, and rail/detail proportions feel more balanced.
- The current color and visual identity are preserved.
- No new functionality, backend changes, or complex responsive framework is introduced.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the task CSS and layout focused.
- Do not touch backend code or add new features.
- Do not introduce a new responsive framework or major visual redesign.
- Split follow-up work if spacing changes need broader cockpit restructuring.

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
- If width or spacing work grows too broad, stop and create a follow-up task first.
