# TASK-43J-construction-page-action-catalog

---
id: TASK-43J
title: Construction page action catalog
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Rework the Construction page into a pure building construction catalog.

## Context
The page should help the player decide what building to build next. It should not be a navigation hub or context dashboard.

## Implementation steps

1. Review construction API state, page view model, and existing build action behavior.
2. Ensure the page focuses on current building queue, available resources, building catalog, and current levels.
3. Show available and blocked buildings.
4. Explain blocked requirements in player terms.
5. Keep build/enqueue action clear where allowed.
6. Remove generic CTA cards, unrelated navigation, and context strips.
7. Add backend tests only if the page requires DTO/readiness changes.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/utils/planetPresentation.ts
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Infrastructure/Buildings/PlanetConstructionQueueService.cs
- tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/ConstructionCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Application/Buildings/EnqueueConstructionOrderResult.cs
- tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs

## Acceptance criteria

- Construction page is a building action catalog.
- Available and blocked buildings are visible.
- Block reasons are understandable to players.
- No generic CTA, duplicated navigation, or context strips remain.

## Constraints

- Do not change construction queue semantics unless needed for display readiness.
- Do not add unrelated gameplay systems.

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
