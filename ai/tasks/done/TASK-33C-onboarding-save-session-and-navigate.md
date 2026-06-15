# TASK-33C

---
id: TASK-33C
title: Onboarding saves playable session and navigates
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
After successful `/onboarding`, save the returned playable start in the local helper and guide the user to Planet without manual id copying.

## Context
Onboarding is the Development-safe entry point for creating a playable start. It should become a natural transition into the Planet hub while keeping backend state authoritative.

## Implementation steps

1. Read the onboarding page and API client used to create the playable start.
2. Use the TASK-33B helper to save returned `civilizationId` and `planetId`, plus any returned display names.
3. Show a clear Spanish success message after creation.
4. Offer primary navigation to Planet with the returned ids.
5. Include secondary links to Construction, Research, and Shipyard when route helpers can preserve the ids.
6. Keep Development-only wording if the endpoint is Development-only.
7. Keep raw ids in secondary details or diagnostics only.
8. Preserve route lazy loading and avoid importing page components eagerly into `App.tsx`.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/App.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- Optional: src/VoidEmpires.Frontend/src/utils/routeUrls.ts if missing route builders are needed.
- Optional: docs/dev/frontend-foundation-smoke-checklist.md if copy or manual flow notes must be updated.

## Acceptance criteria

- New playable start can be remembered locally.
- User can navigate to Planet without manually copying ids.
- Secondary cockpit links preserve `civilizationId` and `planetId`.
- No authentication is implied.
- Raw ids remain secondary or diagnostic.

## Constraints

- Do not perform or claim browser/visual QA.
- Do not add login, password, email verification, tokens, or auth/session behavior.
- Do not fake resources or optimistic-update backend state.
- Preserve lazy loading.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33C message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
