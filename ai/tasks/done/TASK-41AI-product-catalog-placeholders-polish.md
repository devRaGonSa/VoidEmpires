# TASK-41AI

---
id: TASK-41AI
title: Product catalog placeholders polish
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make image placeholders look intentional.

## Context
Building, Research, Ship, and Defense placeholders should feel like awaiting final assets rather than broken or missing media.

## Implementation steps

1. Inspect catalog cards for buildings, research, ships, and defenses.
2. Polish existing placeholder areas without adding final images or generated assets.
3. Avoid visible "placeholder" text if it feels technical.
4. Use "Imagen pendiente" or subtle existing iconography if needed.
5. Preserve responsive layout and card state.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Catalog placeholder areas look intentional.
- No final image assets or generated image files are added.
- No broken/missing asset language appears in product mode.

## Constraints

- Do not add final images/assets.
- Do not generate image files.
- Do not use fake screenshots or external assets.

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
