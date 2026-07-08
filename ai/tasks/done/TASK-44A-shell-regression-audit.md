# TASK-44A-shell-regression-audit

---
id: TASK-44A
title: Shell regression audit
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: high
---

## Goal
Audit the current frontend shell regression.

## Requirements

- Identify why the left sidebar disappeared from authenticated game pages.
- Identify whether `/`, `/planet` and module routes are being rendered through public layout instead of game layout.
- Identify where anonymous header actions are being shown together with game content.
- Document intended shell matrix:
  - `/login`: public layout, no sidebar.
  - `/register` or `/registro`: public layout, no sidebar.
  - authenticated `/`: game layout with sidebar and top resources.
  - authenticated `/planet`: game layout with sidebar and top resources.
  - authenticated `/construction`, `/research`, `/shipyard`, `/defenses`, `/ground-army`, `/fleets`, `/market`, `/alliance`, `/ranking`, `/espionage`: game layout with sidebar and top resources where applicable.
  - anonymous access to game routes: login/register prompt or redirect, no game content.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Frontend/src/components/PublicAuthLayout.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/useCurrentAccountSession.ts
- docs/dev/product-readiness-report.md

## Expected files to modify

- docs/dev/product-readiness-report.md

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
