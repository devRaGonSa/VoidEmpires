# TASK-42AB-rename-civilization-and-home-planet

---
id: TASK-42AB
title: Rename civilization and home planet
status: done
type: fullstack
team: platform
supporting_teams: [frontend]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: medium
---

## Goal
Implement or prepare rename functionality for civilization and home planet.

## Context
Players may choose initial names at registration and should eventually be able to rename civilization and home planet. If backend support is too large for the task budget, prepare disabled UI and document the contract.

## Implementation steps

1. Review civilization and planet ownership models plus account settings UI.
2. If safe within budget, add endpoints/services for renaming with ownership validation and normalized length-limited names.
3. If not safe within budget, add disabled UI affordances and document the backend contract as deferred.
4. Add tests for ownership validation when implementing endpoints.
5. Avoid raw ids in UI.

## Files to read first

- src/VoidEmpires.Domain/Players/Civilization.cs
- src/VoidEmpires.Domain/Galaxy/Planet.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/CivilizationConfiguration.cs
- src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetConfiguration.cs
- src/VoidEmpires.Frontend/src/pages/AccountSettingsPage.tsx

## Expected files to modify

- src/VoidEmpires.Application/Players/RenamePlayerWorldContracts.cs
- src/VoidEmpires.Infrastructure/Players/PlayerWorldRenameService.cs
- src/VoidEmpires.Web/AccountEndpoints.cs
- src/VoidEmpires.Frontend/src/pages/AccountSettingsPage.tsx
- tests/VoidEmpires.Tests/PlayerWorldRenameTests.cs

## Acceptance criteria

- Rename support is implemented and tested, or disabled UI plus documented contract is prepared.
- Name validation is consistent with registration naming rules.
- Ownership is validated for mutations.
- UI does not expose raw ids.

## Constraints

- Respect the 5-file/200-line budget; defer implementation if it grows too large.
- Do not add unrelated account settings features.

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
