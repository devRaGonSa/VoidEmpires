# TASK-13H

---
id: TASK-13H
title: Phase 13H - Strategic map fleet and transfer overlays
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13E-13J"
priority: medium
---

## Goal
Represent fleet markers and transfer overlaps on the strategic map in a simple readable way.

## Context
The 2D strategic map should communicate known systems, fleet markers, and transfer overlays without becoming a real-time simulation or 3D scene. This task improves only the read-only visual overlay layer.

## Implementation steps

1. Review the current strategic map response fields for fleets, transfers, and known systems.
2. Improve the SVG or 2D overlay so the marker types are visually distinct.
3. Add or improve the legend so users can quickly decode the markers.
4. Keep the representation deterministic and simple.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers, if needed
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- Fleet and transfer overlays are easy to read.
- The legend explains the map markers clearly.
- Selected or hovered systems are visually distinguishable if the implementation stays simple.
- No real-time animation is added.
- No new backend movement logic is introduced.
- No gameplay mutations are executed.

## Constraints

- Do not add 3D, Three.js, or WebGL.
- Do not add real-time animation.
- Do not add new backend movement logic.
- Keep the implementation deterministic.

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
