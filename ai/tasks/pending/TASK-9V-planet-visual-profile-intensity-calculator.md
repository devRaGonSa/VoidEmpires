# TASK-9V

---
id: TASK-9V
title: Phase 9V - Planet visual profile and intensity calculator
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Phase 9V - Planet visual profile and intensity calculator"
priority: high
---

## Goal

Add deterministic backend calculation rules for planet visual profiles and initial intensities based on existing domain and persistence data, without introducing rendering behavior.

## Context

This phase should produce stable visual state data for a planet using the current repository model and read-only data already available in the database layer.

The result should be deterministic, clamped, and safe for empty or unowned planets. The profile mapping should follow conservative fallbacks when the current planet type names do not match the future visual taxonomy exactly.

## Implementation steps

1. Review the current planet visual contracts and any existing visual-state services.
2. Implement a backend service in the layer that best fits the current architecture.
3. Return `PlanetVisualStateDto` through the Phase 9U contract.
4. Use existing persisted data only, such as:
   - `Planet`
   - `PlanetOwnership`
   - `PlanetBuilding` if available
   - `OrbitalGroup` or orbital presence if available
   - current size, type, and status fields
5. Keep intensity calculation deterministic and clamp each intensity between `0` and `1`.
6. Make `VisualSeed` stable for the same `PlanetId` and visual version.
7. Map planet types conservatively to profile families such as:
   - Terran / Earthlike / Continental -> `land_city_networks`
   - Ice / Frozen -> `isolated_colony_nodes`
   - Volcanic / Lava -> `shielded_platform_clusters`
   - Oceanic -> `floating_arcologies`
   - Barren / Rocky / Desert -> `mining_outposts`
   - GasGiant -> `orbital_only`
8. Add tests for:
   - deterministic seed generation
   - clamp behavior
   - profile mapping for representative planet types
   - safe low intensities for empty or unowned planets
9. Do not add new gameplay systems, migrations, or frontend rendering.

## Files to read first

- `src/VoidEmpires.Infrastructure/Visuals/PlanetVisualStateService.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualIntensityCalculator.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualProfileCatalog.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualStateDto.cs`
- `tests/VoidEmpires.Tests/PlanetVisualStateServiceTests.cs`
- `tests/VoidEmpires.Tests/PlanetVisualIntensityCalculatorTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Visuals/PlanetVisualStateService.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualIntensityCalculator.cs`
- `src/VoidEmpires.Application/Visuals/PlanetVisualProfileCatalog.cs`
- `tests/VoidEmpires.Tests/PlanetVisualStateServiceTests.cs`
- `tests/VoidEmpires.Tests/PlanetVisualIntensityCalculatorTests.cs`

## Acceptance criteria

- The service produces deterministic, clamped planet visual state.
- The mapping covers the known representative planet types and has safe fallbacks.
- The behavior is test-covered for the important edges.
- No migrations or rendering code are introduced.

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
