# TASK-43S-defense-page-production-catalog

---
id: TASK-43S
title: Defense page production catalog
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Rework Defensas into a defense production catalog.

## Context
The Defense page should let players understand defensive construction. It must not present fake actions if active mutation is not safely supported.

## Implementation steps

1. Review defense API state, page view model, and mutation support.
2. Show defense catalog, stock/readiness where available, costs, durations, and requirements.
3. Expose production action only if active mutation is already safely supported.
4. If mutation is not supported, show catalog/readiness without fake action.
5. Remove context strips and navigation cards.
6. Use compact grid targeting four cards per row on desktop.
7. Add backend tests only if DTO/readiness data must change.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/api/defenseTypes.ts
- src/VoidEmpires.Frontend/src/utils/defensePresentation.ts
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/components/DefenseCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs

## Acceptance criteria

- Defense page is a defense catalog.
- Supported actions are real; unsupported mutations are not faked.
- Context strips and navigation cards are gone.
- Desktop layout uses compact grid behavior.

## Constraints

- Do not add combat.
- Do not present fake production actions.

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
