# TASK-43H-remove-duplicated-navigation-cards

---
id: TASK-43H
title: Remove duplicated navigation cards
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Remove duplicated next-module, cabin-navigation, and access blocks from module pages.

## Context
The sidebar is enough for navigation. Module pages should focus on their catalogs and active queues, not repeat navigation cards.

## Implementation steps

1. Search module pages for shortcut/navigation card sections.
2. Remove Research/Astillero shortcuts from Construction.
3. Remove Construction/Astillero shortcuts from Research.
4. Remove unrelated page shortcuts from Shipyard.
5. Remove `siguientes cabinas` blocks from Ground Army and Defense.
6. Keep only small overview links on Home/Planet if they are part of the planet overview.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx

## Acceptance criteria

- Module pages no longer duplicate sidebar navigation.
- Catalog pages focus on their module content.
- Home/Planet overview keeps only contextual command links if useful.

## Constraints

- Do not remove the sidebar.
- Do not add new gameplay systems.

## Validation

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
