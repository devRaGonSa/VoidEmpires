# TASK-42O-current-user-session-store

---
id: TASK-42O
title: Current user session store
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Replace the local playable-session concept with backend-backed account/current user state.

## Context
LocalStorage may remember non-sensitive UI convenience only. The backend `/me` endpoint is the source of truth for authenticated player identity and current world entry.

## Implementation steps

1. Review playable session utilities, route context hooks, and app shell data flow.
2. Add a current account/session state utility or hook backed by `getCurrentUser`.
3. Preserve only non-sensitive convenience context in localStorage if needed.
4. Rename code and copy away from local game/session product language where touched.
5. Keep route fallback behavior where useful, but do not treat localStorage as identity.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/api/accountApi.ts
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/App.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/useCurrentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts

## Acceptance criteria

- Frontend has a backend-backed current user/session state.
- LocalStorage is not used for identity or auth tokens.
- Product-facing local game/session language is removed where touched.
- Existing route behavior remains usable.

## Constraints

- Avoid broad renames beyond the task budget.
- Preserve lazy loading and frontend build.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
