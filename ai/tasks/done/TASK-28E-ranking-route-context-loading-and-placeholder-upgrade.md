# TASK-28E Ranking Route Context Loading And Placeholder Upgrade

---
id: TASK-28E
title: Upgrade /ranking from placeholder to context-aware read-only cockpit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Move Ranking from placeholder route behavior to a real read-only cockpit shell with civilization context and query-state handling.

## Current problem
Without route context and lazy loading, Ranking would not behave consistently with accepted cockpit pattern.

## Context from current implementation
Frontend uses lazy routes and shared route helpers. Ranking must fit this established structure.

## Goal
Register Ranking with route-level lazy import and no eager page import regressions.

## Files to inspect first
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Implementation requirements
- Ensure /ranking route exists and supports `civilizationId`.
- Keep Ranking page lazy-loaded like other accepted cockpits.
- Add loading UI through the shared Suspense fallback.
- Preserve `/` and route helper aliasing conventions where needed.
- Show missing-context state when no `civilizationId` is available.
- Add links:
- Volver a Galaxia
- Abrir Mercado
- Abrir Espionaje
- Abrir Alianzas
- Ensure page title is `Ranking`.

## UI/UX requirements
- Spanish-first.
- Diagnostics collapsed by default.
- Should look like a cockpit foundation, not a placeholder stub.

## Backend/API requirements
- Consume existing or new read endpoint for ranking state.

## Safety constraints
- Do not add mutation actions.
- Do not break lazy route guard.
- No auth changes.

## Acceptance criteria
- /ranking loads with seeded context.
- Route remains lazy-loaded.
- Route-lazy import guard remains passing.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If route helper is missing, add it consistently with other modules.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
