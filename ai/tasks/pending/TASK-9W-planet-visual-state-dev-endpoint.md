# TASK-9W

---
id: TASK-9W
title: Phase 9W - Planet visual state development endpoint
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Phase 9W - Planet visual state development endpoint"
priority: high
---

## Goal

Expose a development-only read endpoint for planet visual state so the future frontend or sandbox can request procedural planet visual data.

## Context

The repository already uses gated development endpoints for read-only gameplay and visual inspection.

This task should follow the existing endpoint conventions, including development gating and persistence-disabled behavior, while remaining strictly read-only.

## Implementation steps

1. Review the current dev endpoint mapping and the existing visual-state endpoint style.
2. Add or align a development endpoint such as:
   - `GET /api/dev/planets/{planetId}/visual-state`
3. Ensure the endpoint is enabled only when:
   - the environment is `Development`, or
   - `VoidEmpires:DevEndpoints:Enabled=true`
4. Return appropriate responses:
   - `503` if persistence is not configured
   - `400` for invalid ids if applicable
   - `404` if the planet is not found
   - `200` with `PlanetVisualStateDto` when found
5. Register the Phase 9V service in the correct DI location.
6. Add `WebApplicationFactory` tests for:
   - dev gating / not available outside allowed conditions if consistent with the repo pattern
   - `503` when persistence is disabled
   - `404` for missing planets
   - `200` success with seeded test persistence when supported by existing patterns
7. Do not add gameplay mutations, migrations, Three.js/WebGL, or frontend 3D work.

## Files to read first

- `src/VoidEmpires.Web/DevPlanetVisualStateEndpoints.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/DevPlanetVisualStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevelopmentCorsEndpointTests.cs`
- `tests/VoidEmpires.Tests/TestWebApplicationFactoryExtensions.cs`

## Expected files to modify

- `src/VoidEmpires.Web/DevPlanetVisualStateEndpoints.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Infrastructure/Visuals/PlanetVisualStateService.cs`
- `tests/VoidEmpires.Tests/DevPlanetVisualStateEndpointTests.cs`

## Acceptance criteria

- The development endpoint returns deterministic planet visual state when available.
- The endpoint respects the repo's development gating conventions.
- The endpoint returns the expected status codes for disabled persistence, missing records, and success.
- No gameplay mutations or 3D rendering are introduced.

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
