# TASK-13E

---
id: TASK-13E
title: Phase 13E - Strategic map gameplay layout foundation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13E-13J"
priority: medium
---

## Goal
Refactor the Galaxia page into a clearer strategic cockpit layout that feels like a playable map screen rather than a dev-only readout.

## Context
The strategic map already exists, but its current presentation still leans toward a development readout. This task keeps the read-only behavior and backend contracts intact while reorganizing the screen into a game-like strategic cockpit.

## Implementation steps

1. Inspect the strategic map page, its shared shell chrome, and the CSS that drives layout.
2. Reorganize the page into a compact control area, strategic summary, 2D map, selected-system detail panel, planet list, and transfer overlay summary.
3. Keep technical and development details secondary or collapsed.
4. Preserve the current read-only strategic map behavior and avoid introducing new gameplay mutations.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- shared shell/sidebar/header components
- strategic map API/types/helpers
- `src/VoidEmpires.Frontend/src/styles.css`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- shared shell/sidebar/header components used by the strategic map
- strategic map API/types/helpers, if needed
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- Galaxia reads as a playable strategic cockpit.
- The primary layout surfaces the map and its related strategic panels first.
- Technical details are still available, but secondary.
- No backend contracts are changed.
- No new gameplay mutations are added.
- Split, merge, WebSockets, and polling remain out of scope.

## Constraints

- Do not change backend contracts.
- Do not add 3D, Three.js, or WebGL.
- Do not add new gameplay mutations.
- Do not add WebSockets or polling.

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
