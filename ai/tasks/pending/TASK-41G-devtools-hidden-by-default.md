# TASK-41G

---
id: TASK-41G
title: Devtools hidden by default
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Ensure DevelopmentToolsPanel/operator tools are hidden by default in normal UI.

## Context
Tools such as time materialization, due queue updates, diagnostics, and seed-related actions should not appear as normal player actions.

## Implementation steps

1. Locate DevelopmentToolsPanel and all page-specific development/operator controls.
2. Hide controls like "Aplicar 15 min", "Actualizar colas vencidas", materialize, diagnostics, and seed actions by default.
3. Retain tools only behind explicit operator mode if needed.
4. Ensure normal player actions do not look like cheat/debug buttons.
5. Preserve backend behavior and existing APIs.

## Files to read first

- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Development/operator tools are not visible by default.
- Retained tools require explicit operator mode.
- Normal UI does not expose cheat/debug labels.
- Copy regression guard passes.

## Constraints

- Do not remove internal APIs or scripts needed for local operation.
- Do not optimistic-update authoritative resources or queues.
- Keep backend as source of truth.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

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
