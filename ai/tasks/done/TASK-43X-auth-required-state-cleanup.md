# TASK-43X-auth-required-state-cleanup

---
id: TASK-43X
title: Auth-required state cleanup
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Clean auth-required states for gameplay routes.

## Context
Anonymous users opening gameplay routes should see a clean login/register prompt or redirect. The app should not render broken game module context.

## Implementation steps

1. Review auth guard behavior and `AuthRequiredState`.
2. Ensure anonymous gameplay route access redirects to login or shows a clean public prompt.
3. Do not render the game module page with broken context.
4. Do not show `registrar comandante` inside gameplay module content.
5. Ensure left sidebar is not shown on public auth-required pages unless authenticated shell is valid.
6. Run frontend build and copy regression guard.

## Files to read first

- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Anonymous gameplay access is cleanly handled.
- Broken module pages are not rendered without account/planet context.
- Public auth-required state does not show game shell clutter.
- Copy regression guard passes.

## Constraints

- Do not remove login/register.
- Do not expose raw ids or internal context.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
