# TASK-42Q-app-shell-auth-state

---
id: TASK-42Q
title: App shell auth state
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Update the app shell, header, and sidebar for registered account state.

## Context
The shell should show real account/player context when authenticated and login/register actions when anonymous. It must not display fake users or raw technical ids.

## Implementation steps

1. Review app shell, sidebar, top resource bar, and current session hook.
2. Show commander and civilization names when authenticated.
3. Show login/register actions when anonymous.
4. Show logout action when authenticated and wire it to the account API.
5. Avoid raw ids and technical auth terms in primary UI.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/api/accountApi.ts
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts

## Acceptance criteria

- Authenticated shell shows commander/civilization.
- Anonymous shell shows login/register.
- Authenticated shell offers logout.
- No fake user data or raw ids appear in normal UI.

## Constraints

- Keep UI Spanish-first.
- Preserve build and lazy route behavior.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
