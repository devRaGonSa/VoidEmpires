# TASK-14M

---
id: TASK-14M
title: Phase 14M - Construction route foundation and context loading
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Create or complete a dedicated `/construction` route as a construction command center.

## Context
The navigation already exposes Construction, but the route still needs to become a useful screen rather than a placeholder. This task should reuse the safe dev cockpit data already available to Planet and link the route into the existing app shell.

## Implementation steps

1. Review the current router, sidebar, and Planet context handling.
2. Implement or complete `/construction` with `civilizationId` and optional `planetId` query params.
3. Default to the current or first owned planet when no `planetId` is provided.
4. Add Spanish loading, error, and empty states.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`
- new route page under `src/VoidEmpires.Frontend/src/pages/`
- shared Planet or construction navigation helpers, if needed

## Acceptance criteria

- `/construction` loads as a real route.
- It accepts `civilizationId` and optional `planetId`.
- Sidebar Construction navigates to it.
- Planet can link to it with current context.
- The route is not a placeholder.

## Constraints

- Keep the route safe and dev-friendly.
- Do not add a large router rewrite unless current patterns require it.
- Keep the loading and empty states Spanish.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms `/construction` opens and the sidebar works

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
