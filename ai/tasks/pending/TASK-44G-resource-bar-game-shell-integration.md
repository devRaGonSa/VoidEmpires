# TASK-44G-resource-bar-game-shell-integration

---
id: TASK-44G
title: Resource bar game shell integration
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 44 - Restore Authenticated Game Sidebar Shell v1"
priority: medium
---

## Goal
Ensure top resource bar integrates with restored game shell.

## Requirements

- Resource bar appears above game page content.
- Resource bar uses selected planet data.
- It must not appear on login/register.
- It should not push the sidebar away or collapse layout incorrectly on desktop.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
