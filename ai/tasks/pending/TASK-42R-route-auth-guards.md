# TASK-42R-route-auth-guards

---
id: TASK-42R
title: Route auth guards
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Add route guards or graceful auth-required states for main game pages.

## Context
Planet, construction, research, shipyard, fleets, and other main game pages should require an authenticated account unless a retained operator/seed route is intentionally documented.

## Implementation steps

1. Review route definitions and lazy imports.
2. Add a small auth-required wrapper or per-page guard using backend-backed current session state.
3. Guide anonymous users to login/register with Spanish copy.
4. Preserve intentionally allowed development/operator routes and document any exception.
5. Keep route lazy imports passing the guard script.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/planetModuleRoutes.ts
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- docs/dev/product-mode-visibility-contract.md

## Acceptance criteria

- Main game pages require authenticated account state or show a graceful auth-required state.
- Anonymous users are guided to login/register.
- Retained dev/operator access is intentional and documented.
- Lazy import guard passes.

## Constraints

- Do not remove internal dev/operator scripts.
- Do not expose dev/test language in normal player UI.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
