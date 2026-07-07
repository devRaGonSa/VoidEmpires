# TASK-43C-authenticated-game-shell-resource-bar

---
id: TASK-43C
title: Authenticated game shell resource bar
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Add an authenticated top resource bar similar to browser strategy games.

## Context
Authenticated game pages should show selected planet resources at the top. Public auth pages must not show the resource bar.

## Implementation steps

1. Inspect current planet UI state and account/session summary for resource data.
2. Identify whether resource amount and capacity are already available.
3. Add safe backend view-model fields only if the frontend lacks the required source data.
4. Update `TopResourceBar` and game shell to show Creditos, Metal, Cristal, Gas, Energia, and Poblacion if available.
5. Show amount and storage/capacity where available.
6. Do not invent disconnected frontend-only resource values.
7. Add tests for any backend DTO changes.

## Files to read first

- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/ui/TopResourceBar.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx
- src/VoidEmpires.Frontend/src/api/planetTypes.ts
- src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs

## Acceptance criteria

- Authenticated game pages show top resources for the selected planet.
- Amount and capacity are displayed when backend data exists.
- Login/register pages do not show the resource bar.
- Backend remains source of truth.

## Constraints

- Do not fake resources in frontend.
- Do not show raw enum names when a Spanish label exists.
- Do not require SQL Server for tests.

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
