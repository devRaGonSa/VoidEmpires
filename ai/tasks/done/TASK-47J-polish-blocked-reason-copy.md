# TASK-47J

---
id: TASK-47J
title: Polish blocked reason copy
status: done
type: fullstack
team: frontend
supporting_teams: [backend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: medium
---

## Goal
Polish blocked reason copy for construction, shipyard, and defenses.

## Context
Blocked reasons should be concise, card-local, and tied to real requirements.

## Implementation steps

1. Replace vague copy such as "Requisito pendiente" where a precise requirement is known.
2. Replace missing-capacity final copy with a real backend fix and "Sin campos disponibles" only for true capacity exhaustion.
3. Use concise building, research, resource, and capacity requirements.
4. Keep copy card-local and Spanish-first.

## Files to read first

- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts
- src/VoidEmpires.Frontend/src/utils/shipyardViewModel.ts
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts
- src/VoidEmpires.Frontend/src/utils/shipyardViewModel.ts
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Frontend/src/utils/researchPresentation.ts
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx

## Acceptance criteria

- Normal UI no longer shows vague blocked reasons when precise data is available.
- Copy guard remains green.

## Constraints

- Do not expose raw ids or internal/operator copy in normal UI.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
