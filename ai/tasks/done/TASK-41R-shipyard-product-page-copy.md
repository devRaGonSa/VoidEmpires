# TASK-41R

---
id: TASK-41R
title: Shipyard product page copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Shipyard page product-facing.

## Context
Shipyard production is available, but fleet movement and missions remain out of scope.

## Implementation steps

1. Inspect Shipyard page sections, cards, confirmation modal, diagnostics, and disabled future actions.
2. Use product labels: Astillero orbital, Cola de producción, Naves disponibles, Stock orbital, and Preparar escuadras.
3. Remove development copy from product mode.
4. Keep production behavior unchanged.
5. Do not add movement, missions, or fleet execution.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Shipyard page uses requested product language.
- Product mode contains no development copy.
- No movement or mission action is added.
- Copy regression guard passes.

## Constraints

- Do not change shipyard queue, stock, or resource semantics.
- Do not optimistic-update authoritative production state.
- No raw ids in primary UI.

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
