# TASK-51B-defense-units-and-resource-bars

---
id: TASK-51B
title: Defense unit rendering and resource capacity bars
status: done
type: frontend
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 51 - Live queues and visible resource capacity"
priority: high
---

## Goal
Keep canonical unit defenses quantity-based and add stored-capacity bars to the top resource cards.

## Context
Manual QA showed numeric defense enum values rendering as level defenses and top resource cards lacking capacity utilization.

## Implementation steps

1. Normalize defense building types from symbolic strings, numeric strings, and numbers.
2. Map MissileBattery, LaserTurret, IonCannon, and PlasmaCannon to unit production.
3. Preserve DefenseGrid and PlanetaryShield as level-based special defenses.
4. Carry raw resource quantity/capacity into the top resource model.
5. Render accessible capped utilization bars when capacity is available.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts

## Acceptance criteria

- Unit defenses never show `Nivel 0 -> 1`.
- Unit defenses show current quantity, integer input, and `Construir`.
- Special defenses remain level-based.
- Credits, Metal, Crystal, and Gas render accessible utilization bars only with valid capacity.
- Over-capacity values do not overflow the resource card.

## Constraints

- Do not duplicate backend gameplay validation in React.
- Do not invent missing capacities.
- Do not change SQL schema or backend queue rules.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1

## Commit and push

Handled by the final Block 51 closure task.

## Change Budget

- Prefer fewer than 5 files and under 200 changed lines for this task.
