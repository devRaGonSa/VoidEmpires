# TASK-42P-authenticated-home-route

---
id: TASK-42P
title: Authenticated home route
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Update the home route for the online multiplayer account model.

## Context
The home page should guide anonymous users to login/register and authenticated users to continue to their command hub. It must not invite users to start a local game.

## Implementation steps

1. Review `HomePage` and current account/session state.
2. For authenticated users, show commander/civilization/home planet context and a continue action.
3. For anonymous users, show login and registration calls to action.
4. Update copy to describe online multiplayer account entry.
5. Avoid raw technical ids in visible UI.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Authenticated users can continue to the command hub/current planet.
- Anonymous users see login/register actions.
- No "start local game" or equivalent product copy remains on home.
- UI remains Spanish-first.

## Constraints

- Do not claim browser QA.
- Keep changes scoped to the home route and shared route helpers if needed.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
