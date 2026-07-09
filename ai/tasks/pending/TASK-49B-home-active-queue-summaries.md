# TASK-49B

---
id: TASK-49B
title: Home active queue summaries
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 49"
priority: high
---

## Goal
Inicio must show active queue summaries only if they exist.

## Context
Block 48 removed detailed empty queue panels. Block 49 restores active summaries without empty cards.

## Implementation steps

1. Inspect current Home and queue summary components.
2. Render only active construction, research, shipyard, defense, and fleet movement summaries when real items exist.
3. Avoid all empty "Sin ..." queue cards on Inicio.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx

## Acceptance criteria

- Active queue summaries appear on Inicio only when backed by state.
- Empty queue cards do not appear.

## Constraints

- No fake frontend state.
- Backend remains source of truth.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, stage and commit the task result.

## Change Budget

- Prefer modifying fewer than 5 files.
