# TASK-42T-registration-to-planet-navigation

---
id: TASK-42T
title: Registration to planet navigation
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Ensure post-registration navigation enters the generated world.

## Context
Registration response data is the backend source of truth for the new player's civilization and home planet. The frontend may store only non-sensitive convenience context.

## Implementation steps

1. Review registration page success handling and route URL helpers.
2. Use returned `civilizationId`, `homePlanetId`, and `nextRoute` where available.
3. Navigate to the planet hub/current world route after successful registration.
4. Store only non-sensitive convenience context if needed for route fallback.
5. Add static guards or focused tests if the repo has a frontend test pattern available.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/api/accountTypes.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts

## Acceptance criteria

- Successful registration navigates into the generated home planet/world.
- Navigation uses backend response ids or next route.
- No password or auth token is persisted.
- Frontend build passes.

## Constraints

- Do not reintroduce local game copy.
- Keep changes narrow.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
