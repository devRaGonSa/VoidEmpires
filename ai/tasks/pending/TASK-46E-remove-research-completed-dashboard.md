# TASK-46E

---
id: TASK-46E
title: Remove research completed dashboard
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Remove "Tecnologias completadas" and dashboard-style research wrappers from Research page.

## Context
Research should present optional active queue and a single technology catalog; current levels belong inside cards.

## Implementation steps

1. Inspect Research page summary/dashboard blocks.
2. Remove completed technologies main block and noisy progress wrapper.
3. Ensure catalog cards show current levels.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/researchTypes.ts
- src/VoidEmpires.Frontend/src/utils/researchPresentation.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx

## Acceptance criteria

- Main block "Tecnologias completadas" is removed.
- "Progreso cientifico" wrapper is removed if it only adds noise.
- Current technology levels appear inside each technology catalog card.
- Research page is optional active queue plus single technology catalog.

## Constraints

- Do not alter research backend behavior.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

