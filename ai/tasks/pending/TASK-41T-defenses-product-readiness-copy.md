# TASK-41T

---
id: TASK-41T
title: Defenses product readiness copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Defenses page product-facing while honestly scoped.

## Context
Defenses should look intentionally prepared or locked without claiming active mutation behavior that does not exist.

## Implementation steps

1. Inspect the Defenses page, readiness states, cards, and diagnostics.
2. Remove Development/read-only primary copy.
3. Use product language such as Defensa planetaria, Sistemas defensivos, and Producción defensiva pendiente de activación.
4. Keep active mutation unavailable.
5. Hide diagnostics behind operator mode or secondary collapsed surfaces.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Defenses page uses honest product readiness language.
- Product mode does not say Development or read-only in primary copy.
- No active defense mutation is added.

## Constraints

- Do not add combat or defense production mutation.
- Do not fake readiness state.
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
