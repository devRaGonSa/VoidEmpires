# TASK-42Y-registration-error-copy

---
id: TASK-42Y
title: Registration and login error copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Polish registration and login errors in Spanish.

## Context
Normal player UI should show helpful, non-technical error messages for duplicate email, weak password, missing fields, failed login, and server unavailability.

## Implementation steps

1. Review account API error types and registration/login pages.
2. Map backend structured errors to Spanish UI messages.
3. Avoid stack traces, raw Identity codes, and technical internals in primary UI.
4. Include duplicate email, weak password, missing fields, login failed, and server unavailable cases.
5. Run frontend build and copy regression guard.

## Files to read first

- src/VoidEmpires.Frontend/src/api/accountTypes.ts
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/pages/LoginPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/accountErrorCopy.ts
- src/VoidEmpires.Frontend/src/pages/RegisterPage.tsx
- src/VoidEmpires.Frontend/src/pages/LoginPage.tsx

## Acceptance criteria

- Registration and login pages show Spanish-friendly safe errors.
- Technical Identity errors and stack traces are not shown.
- Server unavailable has a clear non-technical message.
- Build and copy guard pass.

## Constraints

- Do not weaken backend authoritative validation.
- Do not add dev/test wording to normal UI.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
