# TASK-43L-construction-card-information-hierarchy

---
id: TASK-43L
title: Construction card information hierarchy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Improve building card hierarchy in the construction catalog.

## Context
The card should quickly answer what the building is, what level it has, whether it can be built, why not if blocked, and what it costs.

## Implementation steps

1. Review building catalog card content and presentation helpers.
2. Make primary information the building name, current level, and action.
3. Make secondary information cost, duration, and requirements.
4. Make tertiary information a short description or effect.
5. Remove long paragraphs and verbose internal/system copy.
6. Ensure blocked items explain the reason clearly.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Building cards have clear primary, secondary, and tertiary information.
- Blocked state is understandable.
- Text is concise and not internal-sounding.

## Constraints

- Use icons/placeholders only if already available.
- Do not introduce new final assets.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
