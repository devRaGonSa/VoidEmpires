# TASK-43O-research-card-information-hierarchy

---
id: TASK-43O
title: Research card information hierarchy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: medium
---

## Goal
Improve research card content hierarchy.

## Context
Research cards should quickly communicate the technology name, current level, action, costs, duration, requirements, and short effect without verbose internal copy.

## Implementation steps

1. Review current research card or page item markup.
2. Make primary information the technology name, current level, and investigate action.
3. Make secondary information cost, duration, and requirements.
4. Make tertiary information a short effect or description.
5. Remove verbose internal/system copy.
6. Build the frontend.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/utils/researchPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/utils/researchPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Research cards have clear information hierarchy.
- Requirements and block reasons are easy to understand.
- Internal/system copy is removed from normal UI.

## Constraints

- Keep labels Spanish-first.
- Do not introduce fake research actions.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
