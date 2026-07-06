# TASK-41AB

---
id: TASK-41AB
title: Product action label system
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Standardize action labels across pages.

## Context
Primary UI actions should use consistent product language rather than technical labels.

## Implementation steps

1. Search pages and components for primary action labels.
2. Standardize product labels: Construir, Investigar, Producir, Revisar, Abrir, Confirmar, Cancelar, and Volver.
3. Remove technical action labels from primary UI.
4. Keep operator-only technical labels behind operator mode if needed.
5. Preserve existing click behavior and disabled states.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Primary actions use standardized product labels.
- Technical action labels are absent from product mode.
- Existing actions and disabled states still behave as before.

## Constraints

- Do not add actions that lack backend support.
- Do not change mutation semantics.
- Keep labels Spanish-first.

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
