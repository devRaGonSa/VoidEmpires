# TASK-41M

---
id: TASK-41M
title: Planet primary actions product layout
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Improve Planet hub primary actions.

## Context
Planet should guide players toward the main command surfaces with product-facing labels and descriptions.

## Implementation steps

1. Inspect Planet handoff/navigation cards and route parameter preservation.
2. Add or polish primary navigation cards for Construir, Investigar, Producir naves, Revisar defensas, and Gestionar flotas.
3. Use concise product descriptions.
4. Remove raw ids and technical labels from primary cards.
5. Preserve existing route params and navigation behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Planet primary actions use requested product labels.
- Route params are preserved.
- Primary cards expose no raw ids.

## Constraints

- Do not add combat, fleet movement, market, alliance, or espionage mutations.
- Do not fake available actions that are not implemented.
- Keep UI Spanish-first.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

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
