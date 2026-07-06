# TASK-41AK

---
id: TASK-41AK
title: Product session persistence copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make persisted local session copy product-facing.

## Context
Stored local session behavior can remain, but the UI should present it as a local game session.

## Implementation steps

1. Inspect home, onboarding, and any session banner/callout copy.
2. Use "Continuar partida", "Nueva partida", and "Olvidar partida".
3. Remove localStorage mention from primary UI.
4. Keep any implementation caveat operator-only if needed.
5. Preserve session persistence behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Persisted session UI uses requested product labels.
- Product mode does not mention localStorage.
- Session behavior is unchanged.

## Constraints

- Do not claim production login.
- Do not alter backend identity/session contracts.
- Keep product copy Spanish-first.

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
