# TASK-43G-remove-module-context-strips

---
id: TASK-43G
title: Remove module context strips
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Remove useless player-facing context strips from gameplay pages.

## Context
Players should not see internal context-management actions inside module pages. Any technical context needed internally should be hidden in operator mode only.

## Implementation steps

1. Search module pages for `PageContextStrip`, `contexto guardado`, `dar contexto`, `cargar mando`, `consulta`, `mando`, generic `continuar`, and `registrar comandante`.
2. Remove those blocks from Construction, Research, Shipyard, Defense, Ground Army, Planet/Home.
3. Keep technical context only in operator/dev-only surfaces when clearly gated.
4. Ensure pages still load selected planet context automatically.
5. Run frontend build and copy regression guard.

## Files to read first

- src/VoidEmpires.Frontend/src/components/PageContextStrip.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx

## Acceptance criteria

- Gameplay pages no longer show context strips or context-management buttons.
- Pages still obtain selected planet context automatically.
- Copy regression guard passes.

## Constraints

- Do not remove useful operator-only diagnostics unless they are normal UI.
- Do not show raw JSON in primary UI.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
