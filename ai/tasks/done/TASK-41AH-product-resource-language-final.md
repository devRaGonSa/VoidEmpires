# TASK-41AH

---
id: TASK-41AH
title: Product resource language final
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Finalize resource language in UI.

## Context
Spanish product UI should consistently display resource names without changing backend values.

## Implementation steps

1. Search resource display helpers, cards, labels, modals, and error messages.
2. Use resource names: Créditos, Metal, Cristal, Gas, Energía, and Población.
3. Avoid raw English names such as Credits or Crystal in Spanish UI unless in operator-only technical context.
4. Preserve backend enum/string values and API contracts.
5. Keep costs, stock, production, and queue math unchanged.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/api/

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/

## Acceptance criteria

- Product UI consistently uses Spanish resource labels.
- Backend values and API contracts are unchanged.
- No resource math or gameplay semantics change.

## Constraints

- Do not rename backend resource identifiers.
- Do not alter authoritative stock, production, cost, or queue semantics.
- Keep operator-only technical context clearly separated.

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
