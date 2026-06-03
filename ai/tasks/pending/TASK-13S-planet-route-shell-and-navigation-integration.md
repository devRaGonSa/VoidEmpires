# TASK-13S

---
id: TASK-13S
title: Phase 13S - Planet route shell and navigation integration
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Create the Planet cockpit route shell and connect it cleanly to the existing frontend navigation.

## Context
The Galaxy cockpit already hints at Planet navigation, but the frontend shell still treats Planeta as a placeholder. This task should wire the actual route, the sidebar entry, and simple query-param based context transfer without introducing a larger routing architecture unless the app already needs it.

## Implementation steps

1. Review the current app shell, route setup, and sidebar navigation structure.
2. Implement or complete the `/planet` route with `planetId` query-param support.
3. Default to the primary or first owned planet when no `planetId` is provided.
4. Connect Galaxy planet quick links and the sidebar `Planeta` entry to the new cockpit route with Spanish loading, error, and empty states.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/config.ts`
- frontend Planet API or view-model files from Phase 13Q and 13R, when available

## Expected files to modify

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- new Planet route page under `src/VoidEmpires.Frontend/src/pages/`

## Acceptance criteria

- `/planet` loads as a real route.
- `/planet?planetId=<guid>` is supported.
- The sidebar `Planeta` entry navigates to the cockpit.
- Galaxy planet links can open the corresponding Planet cockpit route.
- Loading, error, and empty states are Spanish and player-readable.

## Constraints

- Do not introduce a complex router rewrite unless current patterns require it.
- Keep navigation simple and query-param based where practical.
- Do not add unrelated gameplay mutations.
- Keep placeholder or missing data graceful.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms `/planet` opens, the sidebar works, and Galaxy links can open `/planet?planetId=...`

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
