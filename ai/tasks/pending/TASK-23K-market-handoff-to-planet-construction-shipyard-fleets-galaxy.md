# TASK-23K

---
id: TASK-23K
title: Phase 23K - Market handoff to Planet Construction Shipyard Fleets and Galaxy
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Clarify how Market relates to Planet, Construction, Shipyard, Fleets, and Galaxy through explicit handoff guidance and context-preserving links.

## Purpose

Market should interpret economy state and route the player toward the right cockpit for local reserves, production sinks, logistics, or route context instead of duplicating those responsibilities.

## Current problem

Without explicit handoff guidance, Market risks either duplicating neighboring cockpit responsibilities or feeling disconnected from the rest of the accepted module suite.

## Context

Cross-cockpit polish already standardized handoff patterns and context-preserving route helpers. Market should preserve that consistency.

## Files to read first

- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx`

## Component discovery

Inspect current handoff cards, sidebar state cues, and shared route builders. Prefer extending those helpers rather than rebuilding query strings by hand inside Market.

## Dependency analysis

Expected flow:

- Market page -> route builders
- route builders -> preserve `civilizationId` and optional `planetId`
- handoff cards -> neighboring cockpit entrypoints

If a `buildMarketUrl(...)` helper is missing, add it using current conventions.

## Implementation requirements

1. Add a handoff panel that explains:
   - `Planeta`: local reserves and production
   - `Construccion`: resource sinks and infrastructure
   - `Astillero`: orbital production and stock
   - `Flotas`: logistics and movement context
   - `Galaxia`: route and system context
2. Preserve `civilizationId` and `planetId` when applicable.
3. Add route helper support if missing, such as:
   - `buildMarketUrl(...)`
4. Ensure Market links align with accepted navigation and sidebar expectations.
5. Do not add new Fleet behavior.
6. Do not imply that buying or selling can be triggered from another cockpit.

## UI/UX requirements

- Spanish-first
- Handoff cards should remain secondary but clearly useful
- Module boundaries should be easy to understand from the copy
- Do not flood the page with repeated links

## Backend/API requirements

- None

## Expected files to modify

- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- sidebar or nav files only if a narrow Market state cue update is required

## Safety constraints

- No mutations
- No fleet movement
- No transactions
- No duplicate management controls from neighboring cockpits

## Acceptance criteria

- Market explains neighboring modules clearly.
- Links preserve route context consistently.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Future transaction flows may later use selected resource or destination context, but this task should stay focused on navigation and module boundaries only.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm changed files are limited to route or handoff scope.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer shared route-helper updates over inline link-building.
- If sidebar or nav changes begin affecting many modules, split that work into a follow-up task.
