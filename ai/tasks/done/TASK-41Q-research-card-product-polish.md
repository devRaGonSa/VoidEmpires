# TASK-41Q

---
id: TASK-41Q
title: Research card product polish
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Polish technology cards.

## Context
Technology cards should explain category, level, cost, duration, requirements, and impact in product language.

## Implementation steps

1. Inspect Research technology card rendering and blocked reason helpers.
2. Make category, level, cost, duration, requirement, and impact clear.
3. Use placeholder image slots where the existing design supports them.
4. Convert blocked reasons into user-facing Spanish copy.
5. Preserve selection, confirmation, and refresh behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Technology cards clearly show requested information.
- Blocked reasons are product-facing.
- Placeholder areas look intentional when present.

## Constraints

- Do not add or generate final image assets.
- Do not change research costs, requirements, or unlock semantics.
- Do not fake impact values not present in the current data.

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
