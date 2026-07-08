# TASK-47G

---
id: TASK-47G
title: Align shipyard card quantity actions
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: medium
---

## Goal
Align shipyard quantity inputs and production buttons across cards.

## Context
Shipyard cards currently have uneven vertical placement for quantity and action controls.

## Implementation steps

1. Adjust shipyard card layout to use consistent vertical structure.
2. Pin quantity/action controls to the lower action area.
3. Reserve similar action space on blocked cards.
4. Avoid backend semantic changes.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Quantity input and "Producir" buttons align consistently across shipyard cards.
- Blocked cards reserve comparable card action space.

## Constraints

- Do not change backend production behavior.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend

