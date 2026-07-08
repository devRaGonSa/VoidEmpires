# TASK-44C-fix-authenticated-vs-anonymous-rendering

---
id: TASK-44C
title: Fix authenticated vs anonymous rendering
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Prevent mixed anonymous header plus game content states.

## Requirements

- If user is anonymous:
  - do not render planet/game content;
  - show clean public landing/login/register prompt, or redirect to login/register.
- If user is authenticated:
  - show game shell with sidebar;
  - header should show commander/civilization/account state, not `Entrar / Crear cuenta`.
- Direct URLs with `civilizationId`/`planetId` must not bypass auth state in normal UI.
- Operator/dev exceptions may remain hidden and documented, not normal UI.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/useCurrentAccountSession.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/useCurrentAccountSession.ts

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
