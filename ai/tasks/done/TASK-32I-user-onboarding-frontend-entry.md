# TASK-32I

---
id: TASK-32I
title: Add frontend onboarding entry for playable start creation
status: pending
type: feature
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 32A-32P - Playable Session Foundation"
priority: high
---

## Goal
Provide a minimal lazy-loaded frontend route that lets a user create or enter a supported playable start state from the UI.

## Context
Once the backend can create a playable start, the frontend needs a development-safe entry point that makes the flow discoverable without pretending the application has a full production login or session system.

## Implementation steps

1. Add a lazy-loaded page or route such as `/onboarding`, `/start`, or `/new-game` using current routing conventions.
2. Build a Spanish-first form for player name, civilization name, and optional starting planet name when supported.
3. Submit to the backend onboarding endpoint with loading and validation handling.
4. Show development-only labeling if the backend flow is not a production auth flow.
5. Navigate to Planet using the returned ids after success and preserve honest limitations in the UI copy.
6. Verify lazy-loading protections after wiring the route.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/api`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/components`
- `scripts/check-frontend-route-lazy-imports.ps1`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx` or equivalent new page file
- `src/VoidEmpires.Frontend/src/api/*`
- `src/VoidEmpires.Frontend/src/styles.css` only if minimal form styling is required

## Acceptance criteria

- A user can create a supported playable start from the frontend in the supported scope.
- The route is lazy-loaded and passes the lazy-import guard.
- Validation and error states are shown in Spanish-first copy.
- The UI does not claim login, session, or auth behavior that does not exist.

## Constraints

- Preserve lazy loading.
- Do not store sensitive credentials.
- Keep the flow simple and backend-driven.
- Do not introduce a broad new shell or navigation redesign.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend` succeeds.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` succeeds.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(frontend): add playable start onboarding entry`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If routing and page work exceed budget, split styling or API client follow-up work.
