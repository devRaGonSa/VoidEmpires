# TASK-10A

---
id: TASK-10A
title: Phase 10A - Seed minimal fleet development data
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Phase 10A - Seed minimal fleet development data"
priority: high
---

## Goal

Extend development seed data so Flotas and the fleet UI state display meaningful orbital and fleet-related content.

## Context

The strategic map seed should already exist by this phase, and the same explicit seed mechanism should be extended to populate fleet-related data for the seed civilization `00000000-0000-0000-0000-000000000001`.

The goal is to make `/api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001` return meaningful content without adding gameplay mutation behavior.

## Implementation steps

1. Extend the same explicit dev seed mechanism.
2. Seed deterministic fleet and orbital data for civilization `00000000-0000-0000-0000-000000000001`.
3. Include at least:
   - one orbital group around an owned planet
   - one stock or idle orbital group, or the equivalent current model concept
   - assets, ships, or resources required by the current fleet UI state service
   - one route or transfer candidate if supported without executing movement
   - enough resource context to make action hints meaningful
4. Keep mutation actions metadata-only in the frontend; do not add real UI execution.
5. Make the seed idempotent.
6. Do not add gameplay progression.
7. Do not apply seed automatically.
8. Add tests where feasible for:
   - seed creates expected orbital group or fleet data
   - seed is idempotent
   - fleet UI state service returns non-empty fleet or orbital sections from seeded data

## Files to read first

- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalGroupLookupService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalStockGroupService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/FleetOperationalOverviewService.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/` relevant seed or development data files
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs`
- `README.md` if the seed workflow needs to be documented alongside 9Y

## Acceptance criteria

- The fleet seed produces meaningful orbital and fleet validation data.
- The seed is deterministic and idempotent.
- No automatic startup seeding is introduced.
- No real frontend mutation execution is added.
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
