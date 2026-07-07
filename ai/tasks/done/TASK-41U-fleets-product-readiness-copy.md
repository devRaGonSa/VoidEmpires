# TASK-41U

---
id: TASK-41U
title: Fleets product readiness copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Fleets page product-facing while honestly scoped.

## Context
Fleet read state can be shown, but movement and missions remain pending activation.

## Implementation steps

1. Inspect the Fleets page, cards, empty states, diagnostics, and disabled future actions.
2. Remove Development/read-only primary copy.
3. Use product language such as Mando de flotas, Escuadras estacionadas, and Movimiento pendiente de activación.
4. Keep movement and mission actions unavailable.
5. Hide diagnostics behind operator mode or secondary collapsed surfaces.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Fleets page uses honest product readiness language.
- Product mode does not say Development or read-only in primary copy.
- No movement or mission mutation is added.

## Constraints

- Do not add fleet movement, missions, or combat.
- Do not fake fleet state.
- Keep backend as source of truth.

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
