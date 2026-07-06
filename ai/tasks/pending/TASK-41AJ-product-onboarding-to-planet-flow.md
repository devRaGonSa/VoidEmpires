# TASK-41AJ

---
id: TASK-41AJ
title: Product onboarding to planet flow
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Ensure onboarding naturally leads into the Planet hub.

## Context
After a new local game is created, the primary next action should take the player to the Planet hub with product language.

## Implementation steps

1. Inspect onboarding success state, redirect behavior, and post-create call to action.
2. Ensure the primary success action routes to Planet with the current backend-provided context.
3. Use product language only.
4. Remove dev, seed, and validation terminology from primary UI.
5. Preserve backend behavior and returned ids.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/api/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- New-game completion naturally leads to Planet.
- Primary copy contains no dev/seed terminology.
- Backend-created context is preserved.

## Constraints

- Do not add production auth.
- Do not fake ids or frontend state.
- Do not change starting-civilization backend behavior.

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
