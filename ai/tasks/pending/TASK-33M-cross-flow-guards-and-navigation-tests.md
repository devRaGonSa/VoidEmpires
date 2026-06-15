# TASK-33M

---
id: TASK-33M
title: Cross-flow guards and navigation tests
status: pending
type: frontend
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: medium
---

## Goal
Add lightweight regression coverage for playable-loop navigation and forbidden-action boundaries.

## Context
Block 33 adds local session convenience and more cockpit navigation. Static guards should catch regressions without adding a heavy frontend test framework.

## Implementation steps

1. Inspect existing frontend guard scripts.
2. Add or update lightweight checks for:
   - `/onboarding` route remains lazy-loaded;
   - main pages remain lazy-loaded;
   - route URL helpers preserve `civilizationId` and `planetId`;
   - forbidden active actions are not introduced: attack, move fleet, create mission, auto-complete;
   - session helper does not store credentials or tokens.
3. Prefer existing scripts/static checks.
4. Do not add a heavy frontend test framework unless one already exists and is clearly appropriate.
5. Keep guard messages clear and actionable.

## Files to read first

- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/playableSession.ts

## Expected files to modify

- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- Optional: scripts/check-dev-qa-scripts.ps1
- Optional: docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Guardrails cover session/navigation regressions.
- Lazy route checks remain green.
- Copy/forbidden-action checks remain green.
- Session helper storage restrictions are covered by static guard or documented equivalent.

## Constraints

- Do not introduce browser/visual QA.
- Do not add production auth.
- Do not add a new heavy test stack for this task.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33M message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
