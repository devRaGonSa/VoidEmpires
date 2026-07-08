# TASK-46L

---
id: TASK-46L
title: Defense unit vs level model
status: done
type: fullstack
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Correct defense presentation model: most defenses are unit-based, special shields may be level-based.

## Context
Defensive items like missile batteries, turrets, laser cannons and ion cannons should be produced by quantity, not shown as level upgrades.

## Implementation steps

1. Inspect defense catalog metadata and frontend view model.
2. Classify unit-based versus level-based/special defenses.
3. Update presentation model and tests for both categories.

## Files to read first

- src/VoidEmpires.Infrastructure/SeedData/CatalogSources/defenses.catalog.json
- src/VoidEmpires.Domain/Assets/PlanetaryAssetCatalog.cs
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- tests/VoidEmpires.Tests/AssetCatalogTests.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/SeedData/CatalogSources/defenses.catalog.json
- src/VoidEmpires.Frontend/src/utils/defenseViewModel.ts
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- tests/VoidEmpires.Tests/AssetCatalogTests.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Acceptance criteria

- Unit-based defenses do not show "Nivel 0 -> 1".
- Unit-based defenses show current quantity/stock and quantity input.
- Level-based special defenses may show level upgrade if applicable.
- Tests cover unit-based and level-based defense presentation.

## Constraints

- Do not implement combat.
- Do not fake production in frontend.

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
