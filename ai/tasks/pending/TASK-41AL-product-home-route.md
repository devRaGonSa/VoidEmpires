# TASK-41AL

---
id: TASK-41AL
title: Product home route
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the root/home route product-facing.

## Context
The default home should guide players to continue or start a game, not show a development dashboard.

## Implementation steps

1. Inspect `/` and any home/alias route behavior.
2. If a local session exists, show a continue-game path.
3. If no local session exists, guide to new game/onboarding.
4. Remove development dashboard copy from the default home experience.
5. Preserve compatibility aliases and lazy loading.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Home route is product-facing.
- Existing local session offers continuation.
- Missing local session guides to onboarding/new game.
- No development dashboard is the default home.

## Constraints

- Do not remove required compatibility behavior without tests.
- Do not fake session data.
- Preserve lazy loading.

## Validation

Before completing the task ensure:

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
