# TASK-45B

---
id: TASK-45B
title: Remove context loader panels
status: pending
type: frontend
team: platform
supporting_teams: []
roadmap_item: "Block 45"
priority: high
---

## Goal
Remove player-facing manual context loader panels from core module pages.

## Context
Construction, Research, Shipyard and Defenses must get context from the authenticated account, session, or router context instead of exposing manual IDs.

## Implementation steps

1. Remove context and selection panels from Construction, Research, Shipyard and Defenses.
2. Remove raw civilizationId and planetId input fields from normal UI.
3. Remove Abrir vista and Abrir defensas buttons.
4. Keep any strictly necessary technical context hidden from normal player UI.

## Files to read first

- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Frontend/src
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src

## Acceptance criteria

- No manual context loader panels remain on the four core module pages.
- Normal UI does not expose raw civilization or planet IDs.
- Module pages still resolve authenticated context correctly.

## Constraints

- Do not remove authenticated sidebar
- Do not remove top resource bar
- Do not modify unrelated modules
- Keep the change minimal

## Validation

Before completing the task ensure:

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
