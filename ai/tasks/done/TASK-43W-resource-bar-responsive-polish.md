# TASK-43W-resource-bar-responsive-polish

---
id: TASK-43W
title: Resource bar responsive polish
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Polish the top resource bar visually.

## Context
The resource bar should feel like a compact browser strategy game status bar. It must be readable on desktop and wrap cleanly on mobile.

## Implementation steps

1. Review `TopResourceBar` and shell styles.
2. Make desktop layout compact and scan-friendly.
3. Ensure mobile wraps cleanly without overlapping text.
4. Show amount/capacity clearly.
5. Highlight near-capacity only if the logic is easy and safe.
6. Ensure Spanish labels are used instead of raw enum names.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Resource bar is compact on desktop.
- Resource bar wraps cleanly on mobile.
- Amount and capacity are readable.
- Spanish resource labels are shown.

## Constraints

- Do not use raw enum names if Spanish labels exist.
- Do not fake resource/capacity values.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
