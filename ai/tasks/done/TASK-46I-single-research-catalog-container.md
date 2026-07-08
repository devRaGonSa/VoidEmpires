# TASK-46I

---
id: TASK-46I
title: Single research catalog container
status: done
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Make Research use one single technology catalog container.

## Context
All technologies should appear together in one compact catalog grid. Categories can influence order or badges but must not split the page into large sections.

## Implementation steps

1. Inspect Research catalog grouping and sorting.
2. Render all research cards in one grid.
3. Sort by product order: energy/economy, computing, propulsion, military, espionage, colonization/astrophysics, advanced.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/researchTypes.ts
- src/VoidEmpires.Domain/Research/ResearchCatalog.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx

## Acceptance criteria

- All research cards are rendered in one catalog grid.
- No separate category containers such as economy/colonization are rendered.
- Category may appear as a small badge inside each card.
- Desktop grid targets 4 cards per row.
- At least 4 visible research cards.

## Constraints

- Preserve current research actions and backend source of truth.

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
