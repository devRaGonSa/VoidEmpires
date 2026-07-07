# TASK-43E-home-becomes-planet-overview

---
id: TASK-43E
title: Home becomes planet overview
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Rework Inicio into the main current planet overview.

## Context
Authenticated `/` should show the player's selected/current planet overview, not an abstract dashboard or productivity page.

## Implementation steps

1. Review `HomePage`, `PlanetPage`, planet UI state, and placeholder visual components.
2. Make authenticated `/` render the current planet overview.
3. Use a layout with a large planet visual panel on the left and compact planet details on the right.
4. Include planet name, civilization, coordinates if available, used fields/capacity if available, and production summary.
5. Add active queue summaries for buildings, research, shipyard/production, defense if available, and fleet movement prepared/empty only.
6. Remove `nueva partida`, `partida local`, context, and `cabina` copy from the page.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/PlaceholderAsset.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/PlanetOverviewPanel.tsx
- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Authenticated `/` shows current planet overview.
- The page has planet visual, planet details, resource/production summary, and queue summaries.
- No local game, context, or `cabina` terminology appears.
- Copy regression guard passes.

## Constraints

- Do not generate final image assets.
- Use existing placeholder/asset mechanisms.
- Do not fake active queues.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
