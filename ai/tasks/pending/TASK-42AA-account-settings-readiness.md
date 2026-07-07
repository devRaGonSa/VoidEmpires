# TASK-42AA-account-settings-readiness

---
id: TASK-42AA
title: Account settings readiness
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Prepare an account settings page or section.

## Context
The UI should show safe account/world summary data and leave future email/password changes clearly prepared but disabled if not implemented.

## Implementation steps

1. Review app shell navigation and current session data.
2. Add account/settings route or section following existing UI conventions.
3. Show commander, civilization, and home planet.
4. Prepare disabled future email/password change actions if useful.
5. Avoid sensitive data and raw ids in primary UI.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/AccountSettingsPage.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Account settings/readiness UI exists.
- Commander, civilization, and home planet are visible when authenticated.
- Future sensitive actions are disabled/prepared if not implemented.
- No sensitive data is exposed.

## Constraints

- Keep UI Spanish-first.
- Do not implement password/email mutation unless already safely supported.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
