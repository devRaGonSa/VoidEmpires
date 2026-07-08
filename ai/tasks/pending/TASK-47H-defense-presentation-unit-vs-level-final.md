# TASK-47H

---
id: TASK-47H
title: Finalize defense unit versus level presentation
status: pending
type: fullstack
team: frontend
supporting_teams: [backend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Finalize defense presentation as unit-based or level-based per catalog item.

## Context
Unit-based defenses should behave like production items, while explicitly special defenses may remain level-based.

## Implementation steps

1. Classify missile, laser, ion, plasma, turret, and cannon defenses as unit-based.
2. Keep shield/dome/bunker-style special defenses level-based only when intentionally modeled.
3. Remove "Nivel 0 -> 1" from unit-based defense cards.
4. Add tests for defense view-model classification.

## Files to read first

- src/VoidEmpires.Infrastructure/Services/DevDefenseUiStateService.cs
- src/VoidEmpires.Domain/Gameplay/Production/PlanetaryAssetCatalog.cs
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Infrastructure/Services/DevDefenseUiStateService.cs
- src/VoidEmpires.Domain/Gameplay/Production/PlanetaryAssetCatalog.cs
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Acceptance criteria

- Unit-based defenses show stock, quantity input, build action, and inline blocked reason when unavailable.
- Special defenses remain clearly level-based.

## Constraints

- Do not add combat.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

