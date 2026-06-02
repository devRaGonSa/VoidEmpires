# TASK-13I

---
id: TASK-13I
title: Phase 13I - Strategic map read-only action surface polish
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13E-13J"
priority: medium
---

## Goal
Polish the Galaxia action and metadata areas so they feel like read-only strategic tools, not raw API or debug manifests.

## Context
The strategic map should remain clearly read-only while still exposing technical and development information in secondary areas. This task focuses on moving technical copy out of the primary gameplay surface.

## Implementation steps

1. Inspect the current action manifests, dev notes, and metadata sections.
2. Move technical or route-style content into secondary or collapsed details.
3. Keep the primary UI focused on the map, selection, planets, visibility, and transfer context.
4. Preserve the read-only safety copy and avoid visible English in the main gameplay surface.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers
- shared shell/sidebar/header components used by the strategic map

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- strategic map API/types/helpers, if needed
- shared shell/sidebar/header components used by the strategic map

## Acceptance criteria

- The primary strategic map UI is Spanish-first.
- Technical or API-oriented copy is secondary or collapsed.
- The read-only boundary remains clear.
- Raw API route names do not dominate the main surface.
- No mutations are added.
- Backend contracts remain unchanged.

## Constraints

- Do not add mutations.
- Do not change backend contracts.
- Do not add 3D.
- Keep safety warnings visible.

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
