# TASK-43Y-copy-guard-ogame-like-ui

---
id: TASK-43Y
title: Copy guard OGame-like UI
status: pending
type: tooling
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Expand the copy guard for removed visual-review terms.

## Context
The copy guard should prevent normal frontend UI from regressing into internal/orchestrator wording.

## Implementation steps

1. Review the current copy regression script and exception patterns.
2. Make normal frontend UI fail for: `contexto guardado`, `dar contexto`, `cargar mando`, `siguientes cabinas`, `cabina`, `registrar comandante` on gameplay pages, `partida local`, and `nueva partida`.
3. Allow docs/dev and operator-only exceptions if needed.
4. Keep script output actionable.
5. Run copy guard and frontend build.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- docs/dev/cockpit-copy-guidelines.md
- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- docs/dev/cockpit-copy-guidelines.md

## Acceptance criteria

- Copy guard fails for the listed forbidden normal UI terms.
- Legitimate docs/dev/operator exceptions remain allowed.
- Guard passes on the current codebase.

## Constraints

- Avoid broad false positives that block docs/dev.
- Do not weaken existing auth/product copy guards.

## Validation

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
