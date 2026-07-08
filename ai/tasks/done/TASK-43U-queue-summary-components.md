# TASK-43U-queue-summary-components

---
id: TASK-43U
title: Queue summary components
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Create reusable queue summary components for Home/Planet overview.

## Context
The planet overview needs concise summaries for active work without faking queues. Empty states should be short and game-like.

## Implementation steps

1. Review available queue data in planet, construction, research, and shipyard UI state.
2. Create reusable queue summary components for buildings, research, shipyard/production, defense if available, and fleet movement.
3. Use backend state only.
4. Use concise empty states: `Sin obras en cola.`, `Sin investigacion activa.`, `Sin produccion orbital.`, `Sin movimientos de flota.`
5. Integrate summaries into Home/Planet overview.
6. Do not show fake active queues.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/api/researchTypes.ts
- src/VoidEmpires.Frontend/src/api/shipyardTypes.ts
- src/VoidEmpires.Frontend/src/components/PageStatePanel.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Reusable queue summary components exist.
- Home/Planet overview uses queue summaries.
- Empty states are concise and truthful.
- No fake active queues are displayed.

## Constraints

- Backend remains source of truth.
- Do not add fleet movement functionality.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
