# TASK-41Y

---
id: TASK-41Y
title: Ranking product readiness copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Ranking page product-facing.

## Context
Ranking should use power index or imperial classification language while honestly describing current scope.

## Implementation steps

1. Inspect Ranking page copy, cards, comparison rows, empty states, and diagnostics.
2. Use product language such as Índice de poder and Clasificación imperial.
3. Explain scope without development terms.
4. Keep public ladder mutation unavailable.
5. Hide technical diagnostics from product mode.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Ranking page uses power/classification product language.
- Scope is honest without dev/test terms.
- No public ladder mutation is added.

## Constraints

- Do not add ranking rewards, matchmaking, public ladder mutation, or profile systems.
- Do not fake ranking state.
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
