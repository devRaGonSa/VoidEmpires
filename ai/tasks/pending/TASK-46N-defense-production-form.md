# TASK-46N

---
id: TASK-46N
title: Defense production form
status: pending
type: fullstack
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Add or wire defense quantity production where backend support exists; otherwise add safe backend support.

## Context
Defenses should use existing asset-production architecture where possible, without adding combat or fake frontend-only production.

## Implementation steps

1. Inspect existing asset production and planetary asset production model.
2. Wire Defenses page to enqueue defensive production if supported.
3. Add minimal safe application/web path for defensive production orders only if needed.
4. Add backend and endpoint tests where relevant.

## Files to read first

- src/VoidEmpires.Infrastructure/Assets/AssetProductionQueueService.cs
- src/VoidEmpires.Application/Assets/EnqueueAssetProductionRequest.cs
- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs
- src/VoidEmpires.Web/DevPlanetVisualStateEndpoints.cs
- src/VoidEmpires.Frontend/src/api/defenseApi.ts
- tests/VoidEmpires.Tests/AssetProductionOrderTests.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/api/defenseApi.ts
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Acceptance criteria

- Available defenses show quantity input and Construir/produce action.
- Quantity is submitted and queue/state is refreshed from backend response.
- Blocked defenses disable input/action and show inline requirements.
- If backend already supports defensive assets, it is wired; otherwise a minimal safe backend path is added.

## Constraints

- Do not implement combat.
- Do not fake production in frontend.
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

