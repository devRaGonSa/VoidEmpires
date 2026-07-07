# TASK-42AD-frontend-auth-flow-static-guards

---
id: TASK-42AD
title: Frontend auth flow static guards
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Add or update frontend static guards for the auth flow.

## Context
The frontend should preserve lazy loading, avoid forbidden local game copy, avoid password localStorage usage, and keep development/test wording out of normal UI.

## Implementation steps

1. Review existing route lazy import and copy regression scripts.
2. Add guard coverage for register/login lazy routes.
3. Add guard coverage that fails if passwords or auth tokens are stored in localStorage.
4. Add guard coverage for forbidden local game copy and dev/test wording in normal UI.
5. Keep documented operator/dev exceptions where appropriate.

## Files to read first

- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/pages/LoginPage.tsx

## Expected files to modify

- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/App.tsx

## Acceptance criteria

- Register/login routes are covered by lazy route guard.
- Guard fails on password/auth token localStorage usage.
- Guard fails on forbidden normal UI copy.
- Existing valid operator/dev exceptions continue to pass.

## Constraints

- Do not over-block docs/dev or operator-only files.
- Keep scripts portable in PowerShell.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
