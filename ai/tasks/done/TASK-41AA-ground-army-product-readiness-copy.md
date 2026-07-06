# TASK-41AA

---
id: TASK-41AA
title: Ground Army product readiness copy
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Ground Army page product-facing.

## Context
Ground Army should use army command language while keeping training and ground combat honestly scoped.

## Implementation steps

1. Inspect Ground Army page copy, cards, readiness states, disabled actions, and diagnostics.
2. Replace dev/read-only primary copy with army command language.
3. Describe training or ground combat as pending activation.
4. Do not add recruitment or combat mutation unless it already exists safely and is covered by tests.
5. Hide technical diagnostics from product mode.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Ground Army page uses army command product language.
- Training/ground combat scope is honest without dev terms.
- No unsafe recruitment or combat mutation is added.

## Constraints

- Do not add combat.
- Do not fake army state.
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
