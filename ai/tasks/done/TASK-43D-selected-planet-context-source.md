# TASK-43D-selected-planet-context-source

---
id: TASK-43D
title: Selected planet context source
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Centralize selected planet context for authenticated pages.

## Context
Gameplay pages should load the selected or home planet automatically when authenticated. The UI must not ask players to provide technical context.

## Implementation steps

1. Review current account session utilities and route context helpers.
2. Use `/me` or current account/session world summary as the source of truth for the home planet.
3. If a route lacks `planetId` and the user has `homePlanetId`, default to the home planet.
4. Remove or hide player-facing context blocks such as `dar contexto`, `contexto guardado`, and raw context dumps.
5. Ensure anonymous users are redirected to login or shown a clean auth-required public state.
6. Add backend tests only if `/me` needs extra selected planet fields.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/useCurrentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/components/PageContextStrip.tsx
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Web/AccountEndpoints.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/currentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/useCurrentAccountSession.ts
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/components/AuthRequiredState.tsx
- src/VoidEmpires.Web/AccountEndpoints.cs

## Acceptance criteria

- Authenticated gameplay pages can resolve the selected planet automatically.
- Home planet is used when route context is absent.
- Normal UI does not show technical context wording or raw context blocks.
- Anonymous gameplay access is cleanly handled.

## Constraints

- Backend remains source of truth.
- Do not show raw ids in primary UI.
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
