# TASK-43T-ground-army-production-catalog

---
id: TASK-43T
title: Ground Army production catalog
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Rework Ejercito de tierra into a unit recruitment/training catalog.

## Context
The Ground Army page should focus on land unit creation or training. If active mutation is not supported, it should show catalog/readiness without fake actions.

## Implementation steps

1. Review ground army API state, page view model, and any barracks/building requirement data.
2. Show recruitable units if the barracks or required building exists.
3. If requirements are missing, show a clear requirement message.
4. If active mutation is not supported, show catalog/readiness without fake action.
5. Remove context strips, navigation cards, and `consulta/mando/cargar mando` language.
6. Use compact grid or list layout.
7. Add backend tests only if DTO/readiness data must change.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- src/VoidEmpires.Frontend/src/api/groundArmyTypes.ts
- src/VoidEmpires.Frontend/src/utils/groundArmyPresentation.ts
- src/VoidEmpires.Infrastructure/Planets/DevGroundArmyUiStateService.cs
- tests/VoidEmpires.Tests/DevGroundArmyUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx
- src/VoidEmpires.Frontend/src/components/GroundArmyCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Infrastructure/Planets/DevGroundArmyUiStateService.cs
- tests/VoidEmpires.Tests/DevGroundArmyUiStateEndpointTests.cs

## Acceptance criteria

- Ground Army page is a unit recruitment/training catalog or readiness page.
- Missing requirements are explained clearly.
- No fake actions are shown.
- No context/navigation clutter or internal wording remains.

## Constraints

- Do not add combat.
- Do not add active unsupported mutations.

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
