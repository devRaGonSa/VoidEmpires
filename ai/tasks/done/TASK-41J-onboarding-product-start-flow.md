# TASK-41J

---
id: TASK-41J
title: Onboarding product start flow
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make `/onboarding` feel like starting a new game, not creating a development session.

## Context
The backend behavior can remain unchanged, but the normal UI should present a product-facing start flow.

## Implementation steps

1. Inspect onboarding page copy, action labels, local session copy, and error/technical notes.
2. Use product copy: Crear comandante, Nombrar civilización, Fundar mundo inicial, Comenzar partida.
3. Remove Development-safe/no-auth wording from primary UI.
4. Move any no-auth caveat to hidden technical/operator notes.
5. Keep backend requests and redirect behavior unchanged unless a later task explicitly changes the product flow button.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/api/
- docs/dev/product-mode-visibility-contract.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Onboarding primary copy reads like a game start flow.
- Product mode does not mention development-only/no-auth implementation details.
- Backend behavior remains unchanged.

## Constraints

- Do not add real production auth.
- Do not fake session state.
- Do not use seed/dev terminology in primary UI.

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
