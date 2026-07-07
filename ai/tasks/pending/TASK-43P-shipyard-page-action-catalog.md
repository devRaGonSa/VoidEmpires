# TASK-43P-shipyard-page-action-catalog

---
id: TASK-43P
title: Shipyard page action catalog
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Rework the Shipyard page into a ship production catalog.

## Context
The Shipyard page should focus on production queue, available resources, ship catalog, and current orbital stock. It must not add fleet movement actions in this block.

## Implementation steps

1. Review shipyard API state, page view model, and existing production action behavior.
2. Ensure production queue and current orbital stock are visible.
3. Show available resources and ship catalog.
4. Show available and blocked ships.
5. Remove generic CTA cards, unrelated navigation, and context strips.
6. Add backend tests only if DTO/readiness data must change.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/shipyardTypes.ts
- src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts
- src/VoidEmpires.Infrastructure/Assets/AssetProductionQueueService.cs
- tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Application/Assets/EnqueueAssetProductionResult.cs
- tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs

## Acceptance criteria

- Shipyard page is a ship production catalog.
- Available and blocked ships are visible.
- Current stock and production queue are understandable.
- No fleet movement actions are added.

## Constraints

- Do not add fleet movement.
- Do not change asset production semantics unless required for display readiness.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
