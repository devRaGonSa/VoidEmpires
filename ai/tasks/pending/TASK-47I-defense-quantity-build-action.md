# TASK-47I

---
id: TASK-47I
title: Defense quantity build action
status: pending
type: fullstack
team: frontend
supporting_teams: [backend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Ensure defense quantity production works from the UI for unit-based defenses.

## Context
Defense production is wired through planetary asset production and must be exposed like shipyard production in the defense module.

## Implementation steps

1. Allow quantity input for unit-based defenses.
2. Submit with "Construir" through the existing planetary asset production path.
3. Refresh defense state/queue on success.
4. Disable input/action and show inline reason when blocked.
5. Add endpoint/service tests if backend changes are required.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Infrastructure/Services/DevDefenseUiStateService.cs
- src/VoidEmpires.Web/Program.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevAssetProductionEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Infrastructure/Services/DevDefenseUiStateService.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Acceptance criteria

- Unit-based defense cards support quantity input and "Construir".
- Successful builds refresh defense state and queue.
- No fake frontend state is used.

## Constraints

- Do not add combat.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

