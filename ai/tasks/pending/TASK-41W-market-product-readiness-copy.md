# TASK-41W

---
id: TASK-41W
title: Market product readiness copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Market page product-facing.

## Context
Market should use commercial/economic language while keeping transactions unavailable.

## Implementation steps

1. Inspect Market page copy, cards, empty states, disabled actions, and diagnostics.
2. Use product language such as Mercado galáctico and operaciones comerciales pendientes de activación.
3. Remove Development/read-only primary copy.
4. Keep transaction actions unavailable.
5. Hide technical diagnostics from product mode.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Market page uses commercial product language.
- Product mode avoids Development/read-only primary copy.
- No market transaction is added.

## Constraints

- Do not add market transactions.
- Do not fake economy state.
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
