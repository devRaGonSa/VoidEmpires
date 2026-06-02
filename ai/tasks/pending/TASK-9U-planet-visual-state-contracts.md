# TASK-9U

---
id: TASK-9U
title: Phase 9U - Planet visual state application contracts
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Phase 9U - Planet visual state application contracts"
priority: high
---

## Goal

Define the application-layer contract surface for future procedural planet visual state without adding persistence or rendering behavior.

## Context

The repository already has a `Visuals` application area, and future backend services will need a stable contract for planet-level visual state.

The contract should stay lightweight and render-agnostic so it can be consumed later by infrastructure services and the frontend without coupling to implementation details.

## Implementation steps

1. Review the existing `VoidEmpires.Application.Visuals` contract style.
2. Add or align DTOs/records such as:
   - `PlanetVisualStateDto`
   - `PlanetVisualProfileDto`
3. Include request/result contracts and an interface if consistent with existing style:
   - `GetPlanetVisualStateRequest`
   - `GetPlanetVisualStateResult`
   - `IPlanetVisualStateService`
4. Keep the payload limited to lightweight planet state, for example:
   - `PlanetId`
   - `PlanetName`
   - `PlanetType`
   - `Size`
   - `ColonizationStatus` if available
   - `IsOwned`
   - `CivilizationId`
   - `CivilizationColor` or a nullable placeholder
   - `VisualSeed`
   - `ColonizationIntensity`
   - `UrbanIntensity`
   - `IndustrialIntensity`
   - `TerraformingIntensity`
   - `MilitaryIntensity`
   - `OrbitalPresenceIntensity`
   - `Profile`
5. Keep `PlanetVisualProfileDto` render-agnostic, with fields like:
   - `SurfaceProfile`
   - `LightDistributionMode`
   - `PlatformMode`
   - `AtmosphereProfile`
   - `CloudProfile`
   - `SupportsNightLights`
   - `SupportsSurfacePlatforms`
   - `SupportsOrbitalMegastructureHints`
6. Add compile-level or unit tests only if they meaningfully protect the contract shape.
7. Do not add persistence queries, migrations, or frontend rendering.

## Files to read first

- `src/VoidEmpires.Application/Visuals/PlanetVisualStateDto.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualProfileDto.cs`
- `src/VoidEmpires.Application/Visuals/GetPlanetVisualStateRequest.cs`
- `src/VoidEmpires.Application/Visuals/GetPlanetVisualStateResult.cs`
- `src/VoidEmpires.Application/Visuals/IPlanetVisualStateService.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualIntensityCalculator.cs`

## Expected files to modify

- `src/VoidEmpires.Application/Visuals/PlanetVisualStateDto.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualProfileDto.cs`
- `src/VoidEmpires.Application/Visuals/GetPlanetVisualStateRequest.cs`
- `src/VoidEmpires.Application/Visuals/GetPlanetVisualStateResult.cs`
- `src/VoidEmpires.Application/Visuals/IPlanetVisualStateService.cs`
- `tests/VoidEmpires.Tests/PlanetVisualStateServiceTests.cs`

## Acceptance criteria

- The application layer exposes a clean contract for planet visual state.
- The contract remains lightweight and render-agnostic.
- No EF migrations, persistence queries, or frontend rendering are introduced.
- Validation commands pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.
