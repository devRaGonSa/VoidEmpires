# TASK-42N-login-page

---
id: TASK-42N
title: Login page
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Add a product login page.

## Context
Anonymous players need a clear entry point for existing accounts. Copy should be Spanish-first and avoid technical auth wording in the primary UI.

## Implementation steps

1. Review routing, page layout conventions, and account API client.
2. Add `/login` or `/entrar` route with email and password fields.
3. Submit through the account API client and clear password after submit.
4. Show safe Spanish errors for failed login or server unavailability.
5. Link to registration.
6. On success, navigate to the current planet/hub from the returned session.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/api/accountApi.ts
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/LoginPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Login route exists and lazy-loads if routes are lazy-loaded.
- Login uses email/password and safe errors.
- Registration link is available.
- Successful login navigates to the current world/hub.

## Constraints

- No password persistence.
- No localStorage auth token.
- Keep UI Spanish-first.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
