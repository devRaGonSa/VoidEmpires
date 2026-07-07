# TASK-42L-frontend-account-api-client

---
id: TASK-42L
title: Frontend account API client
status: pending
type: frontend
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Add a typed frontend account API client for registration, login, logout, and current user lookup.

## Context
The frontend must call backend account endpoints without persisting passwords or auth tokens in localStorage.

## Implementation steps

1. Review existing API helper style and error handling.
2. Add typed request/response models for registration, login, logout, current user, and account errors.
3. Implement `register`, `login`, `logout`, and `getCurrentUser`.
4. Ensure requests support credentials if the backend uses HTTP-only cookies.
5. Ensure password fields are not stored after submit and no localStorage token is introduced.
6. Build the frontend.

## Files to read first

- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts
- src/VoidEmpires.Frontend/src/config.ts
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/api/accountApi.ts
- src/VoidEmpires.Frontend/src/api/accountTypes.ts
- src/VoidEmpires.Frontend/src/api/voidEmpiresApi.ts

## Acceptance criteria

- Typed account API client exists for all required endpoint calls.
- Errors are represented in a typed way for later Spanish UI copy.
- No password or auth token is stored in localStorage.
- Frontend build passes.

## Constraints

- Do not change visible UI routes in this task unless required to compile.
- Preserve lazy loading.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
