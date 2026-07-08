# TASK-46H

---
id: TASK-46H
title: Building availability and starting playability
status: pending
type: backend
team: product
supporting_teams: []
roadmap_item: "Block 46A-46S - OGame Module Catalog & Production Polish v1"
priority: high
---

## Goal
Ensure early Construction does not show every basic building as blocked.

## Context
Newly registered/home planet state should make basic early buildings visible and reasonably buildable when the player has resources.

## Implementation steps

1. Inspect catalog requirements and starting home planet bootstrap.
2. Fix catalog requirements or starting bootstrap if basic construction options are blocked.
3. Add or adjust tests for newly registered player/home planet availability.

## Files to read first

- src/VoidEmpires.Domain/Buildings/BuildingCatalog.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs
- src/VoidEmpires.Infrastructure/SeedData/CatalogSources/buildings.catalog.json
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/BuildingCatalogTests.cs

## Expected files to modify

- src/VoidEmpires.Domain/Buildings/BuildingCatalog.cs
- src/VoidEmpires.Infrastructure/Players/InitialPlayerWorldBootstrapService.cs
- src/VoidEmpires.Infrastructure/SeedData/CatalogSources/buildings.catalog.json
- tests/VoidEmpires.Tests/InitialPlayerWorldBootstrapServiceTests.cs
- tests/VoidEmpires.Tests/BuildingCatalogTests.cs

## Acceptance criteria

- Metal mine / Mina de metal, Crystal synthesizer / Sintetizador de cristal, Gas extractor / Extractor de gas, and Energy plant / Planta de energia are visible and reasonably buildable when resources are available.
- Valid resource-cost blocking remains.
- Tests cover basic construction availability for newly registered player/home planet.

## Constraints

- Backend remains source of truth.
- Do not require SQL Server for automated tests.

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

