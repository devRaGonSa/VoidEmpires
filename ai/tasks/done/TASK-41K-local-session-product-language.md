# TASK-41K

---
id: TASK-41K
title: Local session product language
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Rewrite local session copy as "partida local" or "sesión de juego local" without exposing implementation details.

## Context
The UI may still use local state internally, but primary copy should not mention implementation details such as localStorage.

## Implementation steps

1. Locate local session helpers, onboarding copy, home route copy, and session-management actions.
2. Replace implementation language with product-facing terms such as partida local and sesión de juego local.
3. Rename "Limpiar memoria local" to "Olvidar partida local" where present.
4. Rename "Crear otro inicio" to "Nueva partida" where present.
5. Keep behavior and stored values unchanged.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Primary UI does not mention localStorage or implementation storage.
- Session actions use product language.
- Copy regression guard passes.

## Constraints

- Do not claim production login or production auth.
- Do not change storage semantics.
- Keep backend as source of truth for authoritative game data.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

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
