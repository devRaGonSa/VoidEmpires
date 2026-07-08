# TASK-46K

---
id: TASK-46K
title: Shipyard quantity production form
status: pending
type: frontend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Make ship production cards support quantity-based production where available.

## Context
Available ship cards should provide quantity input and production action that submits quantity to backend production.

## Implementation steps

1. Inspect existing shipyard production API and UI card actions.
2. Add quantity input and submit action for available ships.
3. Disable blocked card inputs/actions and show inline reasons.
4. Add tests if backend/API changes are needed.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/shipyardApi.ts
- src/VoidEmpires.Frontend/src/api/shipyardTypes.ts
- src/VoidEmpires.Application/Assets/EnqueueAssetProductionRequest.cs
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/ShipyardCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/shipyardApi.ts
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs

## Acceptance criteria

- Available ships show quantity input and Producir action.
- Quantity is submitted to backend production endpoint/service.
- Blocked ships disable input/action and show inline requirement/resource reason.
- Confirmation modal remains only for actual production submission if existing pattern requires it.

## Constraints

- No modal just for blocked state.
- Backend remains source of truth.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage intended files.
3. Commit with a clear message.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

