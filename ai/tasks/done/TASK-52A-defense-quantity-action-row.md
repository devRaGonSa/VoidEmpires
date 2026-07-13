# TASK-52A

---
id: TASK-52A
title: Align quantity-based defense production actions
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 52"
priority: high
---

## Goal
Align quantity-based Defense inputs and Construir actions in the same responsive production row by sharing the established Shipyard layout.

## Context
Unit defense cards use quantity production but currently separate the quantity control and action vertically. Special level-based defenses must remain unchanged.

## Implementation steps

1. Compare Defense and Shipyard catalog-card markup and styles.
2. Reuse the shared production action-row layout for quantity-based defenses.
3. Extend the existing frontend regression guard for the shared layout contract.

## Files to read first

- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1
- ai/tasks/done/TASK-52A-defense-quantity-action-row.md

## Acceptance criteria

- Quantity-based defense inputs and buttons share one horizontal desktop row with compatible heights.
- Disabled controls preserve the same layout.
- Existing narrow-screen stacking remains responsive.
- DefenseGrid and PlanetaryShield level cards are unaffected.
- A static frontend guard protects the shared Shipyard/Defense production-row contract.

## Constraints

- Do not change production rules, costs, durations, API calls, or modal behavior.
- Do not introduce a new frontend test framework.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1

## Commit and push

At the end, stage only intended files and commit with a focused message. Push after the complete Block 52 validation cycle.

## Change Budget

- Prefer modifying fewer than 5 implementation/guard files.
- Prefer changes under 200 lines of code.
- Prefer one commit for this task.
