# TASK-42M-registration-page-replaces-onboarding

---
id: TASK-42M
title: Registration page replaces onboarding
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Replace the onboarding product flow with account registration.

## Context
The primary route should be `/register` or `/registro`. `/onboarding` may remain as an alias, but player-facing copy must say create/register account, not new local game.

## Implementation steps

1. Review current routing and `OnboardingPage`.
2. Add or rename the page to a registration page with fields for email, password, confirm password, commander name, civilization name, and optional planet name.
3. Submit through the account API client and clear password fields after submit.
4. On success, navigate to the returned next route or the planet/home route using returned ids.
5. Replace product copy so it reflects online multiplayer account registration.
6. Preserve lazy route imports and copy regression guard expectations.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/api/accountApi.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Acceptance criteria

- Primary registration route exists.
- `/onboarding` is removed or acts only as an alias to registration.
- UI fields match the required registration contract.
- Success navigates into the generated world using backend response data.
- No visible "new local game" copy remains in this flow.

## Constraints

- Keep UI Spanish-first.
- Do not claim visual/browser QA.
- Preserve lazy loading.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
