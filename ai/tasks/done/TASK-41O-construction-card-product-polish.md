# TASK-41O

---
id: TASK-41O
title: Construction card product polish
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Polish building cards.

## Context
Building cards should clearly communicate availability, current state, costs, duration, and requirements without technical metadata.

## Implementation steps

1. Inspect Construction building card rendering and blocked reason helpers.
2. Make available, blocked, current level, cost, and duration states clear.
3. Use an existing placeholder asset area if already present.
4. Remove technical labels such as command metadata from primary cards.
5. Keep blocked reasons user-facing and truthful.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Building cards clearly show state, level, cost, duration, and requirements.
- Technical metadata is absent from primary cards.
- Blocked reasons are user-facing.

## Constraints

- Do not add or generate final image assets.
- Do not change enqueue semantics or resource costs.
- Do not fake backend state.

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
