# TASK-9Z

---
id: TASK-9Z
title: Phase 9Z - Seed minimal strategic map development data
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Phase 9Z - Seed minimal strategic map development data"
priority: high
---

## Goal

Add minimal, deterministic development seed data that makes Galaxia and the strategic map show meaningful content.

## Context

The development seed convention from Phase 9Y should be extended with a small deterministic dataset that exercises the current strategic map service without introducing gameplay progression or automatic database mutation.

This seed should make `/api/dev/strategic-map` return non-empty content for the seed civilization `00000000-0000-0000-0000-000000000001`.

## Implementation steps

1. Extend the explicit dev seed mechanism introduced in Phase 9Y.
2. Seed deterministic data sufficient for the strategic map to return meaningful content for civilization `00000000-0000-0000-0000-000000000001`.
3. Include at least:
   - one player or owner entity if required by the current model
   - one civilization if required by the current model
   - one star system
   - at least three planets with distinct planet types or profiles if the current model supports this
   - ownership for at least one planet by the seed civilization
   - basic economy/resource state if required by the strategic map payload
   - visibility, knowledge, or readiness records if required for map data to appear
4. Keep IDs deterministic and document them.
5. Make the seed idempotent so re-running it does not duplicate data.
6. Avoid adding gameplay progression.
7. Do not add frontend changes.
8. Do not auto-apply the seed to PostgreSQL.
9. Add tests where feasible for:
   - seed creates expected strategic map data
   - seed is idempotent
   - strategic map service returns non-empty systems from seeded data

## Files to read first

- `src/VoidEmpires.Infrastructure/Visuals/PlanetVisualStateService.cs`
- `src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs`
- `src/VoidEmpires.Infrastructure/Identity/IdentityAccountService.cs`
- `src/VoidEmpires.Domain`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/DevelopmentCorsEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/` relevant seed or development data files
- `src/VoidEmpires.Web/Program.cs`
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`
- `tests/VoidEmpires.Tests/` relevant seed or dev endpoint tests

## Acceptance criteria

- The strategic map seed produces meaningful non-empty development data.
- The seed is deterministic and idempotent.
- No automatic production startup seeding is introduced.
- No frontend code is changed.
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
