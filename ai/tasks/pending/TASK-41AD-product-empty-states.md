# TASK-41AD

---
id: TASK-41AD
title: Product empty states
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Polish empty states into product language.

## Context
Empty states should feel intentional and should not mention endpoints or missing technical data.

## Implementation steps

1. Search normal pages and components for empty-state copy.
2. Remove "no data from endpoint" and similar technical language.
3. Use product empty states such as No hay órdenes activas, No hay escuadras estacionadas, Tecnología pendiente de selección, and Sistema aún sin actividad.
4. Keep technical details available only in operator diagnostics where appropriate.
5. Preserve loading/error/empty-state behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Empty states use product language.
- Product mode does not mention endpoint data absence.
- Behavior is unchanged.

## Constraints

- Do not hide real errors as empty states.
- Do not fake data.
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
