# TASK-10B

---
id: TASK-10B
title: Phase 10B - Seed planet visual state validation data and manual docs
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Phase 10B - Seed planet visual state validation data and manual docs"
priority: high
---

## Goal

Ensure the seeded development data validates `PlanetVisualState` with meaningful visual intensities and profiles, and document the manual validation flow.

## Context

By this phase, the explicit dev seed should already populate strategic map and fleet validation data. This task extends the same seed set so at least three seeded planets produce distinct visual state outputs.

The repository should remain free of Three.js/WebGL or actual frontend 3D work. The intent is only to validate the backend data and the read-only development endpoints.

## Implementation steps

1. Extend the development seed data so at least three seeded planets produce distinct `PlanetVisualState` outputs.
2. Include at least:
   - one owned or colonized continental or earthlike planet with non-zero colonization or urban intensity
   - one industrial, mining, barren, or rocky planet with non-zero industrial intensity if the current model supports it
   - one gas giant or non-colonizable fallback type with orbital-only or low surface intensity profile if the current model supports it
3. Ensure the visual state calculator returns:
   - stable `VisualSeed`
   - distinct `SurfaceProfile` and `Profile` combinations
   - clamped intensities
   - non-empty render-agnostic profile values
4. Add or update documentation with manual validation URLs such as:
   - `/health`
   - `/api/dev/strategic-map?civilizationId=00000000-0000-0000-0000-000000000001`
   - `/api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001`
   - `/api/dev/planets/{planetId}/visual-state`
5. Document how to:
   - run the backend against `VoidEmpireDB_Dev`
   - run the explicit dev seed
   - start the frontend with `VITE_VOIDEMPIRES_API_BASE_URL=http://localhost:5142`
   - validate Galaxia, Flotas, and `PlanetVisualState`
6. Add tests where feasible for:
   - seeded planets produce meaningful and distinct visual state
   - visual-state endpoint returns `200` for a seeded planet
   - missing planet still returns `404`
7. Do not add Three.js/WebGL, frontend 3D, gameplay mutations, or automatic seed execution.

## Files to read first

- `src/VoidEmpires.Application/Visuals/PlanetVisualStateDto.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualProfileDto.cs`
- `src/VoidEmpires.Infrastructure/Visuals/PlanetVisualStateService.cs`
- `src/VoidEmpires.Web/DevPlanetVisualStateEndpoints.cs`
- `tests/VoidEmpires.Tests/DevPlanetVisualStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/PlanetVisualStateServiceTests.cs`

## Expected files to modify

- `README.md`
- `src/VoidEmpires.Infrastructure/` relevant seed or visual-state data files
- `src/VoidEmpires.Web/DevPlanetVisualStateEndpoints.cs`
- `tests/VoidEmpires.Tests/DevPlanetVisualStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/PlanetVisualStateServiceTests.cs`

## Acceptance criteria

- Seeded planets produce distinct, meaningful visual state.
- Manual validation steps are documented.
- The visual-state endpoint behaves correctly for seeded and missing planets.
- No 3D rendering code or gameplay mutations are introduced.
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
- `npm run build --prefix src/VoidEmpires.Frontend` if documentation or frontend-adjacent files are touched; otherwise optional but preferred

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
