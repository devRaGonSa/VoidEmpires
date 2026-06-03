# TASK-18H-research-seed-sequence-collision-fix

---
id: TASK-18H-research-seed-sequence-collision-fix
title: Research seed sequence collision fix
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: high
---

## Goal
Fix `ResearchOrder` seeding so `cockpit-validation` cannot collide on `CivilizationId + Sequence`.

## Purpose
Make the richer seed profile safe to reapply over a development database that already contains manual or endpoint-created research queue rows.

## Current Problem
`cockpit-validation` seeds a completed `ResearchOrder` with a fixed `Sequence`, but it only checks for an existing row using logical fields like research type and status. If a reused development database already has any other order with that same sequence for the civilization, PostgreSQL rejects the insert.

## Context
- The real database uniqueness contract is `CivilizationId + Sequence`.
- The existing local QA workflow can create research orders through the supported dev endpoint.
- The fix must preserve the accepted Research cockpit flow and must not delete user-created QA data.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/ResearchOrderConfiguration.cs`
- `src/VoidEmpires.Domain/Research/ResearchOrder.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`

## Implementation Requirements
1. Align seeded `ResearchOrder` handling with the actual database uniqueness dimensions.
2. Before inserting a seeded row, check whether the chosen sequence is already used by that civilization.
3. If the sequence is already occupied:
   - reuse or update the seeded row when it is the same logical seed row; or
   - choose a deterministic safe sequence range reserved for `cockpit-validation`.
4. Do not delete existing user-created QA orders.
5. Add a regression test that reproduces:
   - apply `minimal-validation`;
   - create or enqueue one research order through the existing safe flow or equivalent persisted state;
   - apply `cockpit-validation`;
   - assert no exception and no duplicated seeded research rows.
6. Preserve Research UI expectations and queue readability.

## UI/UX Requirements
- Research cockpit expectations must remain stable after the fix.
- No player-facing copy changes are required unless diagnostics become clearer.

## Backend/API Requirements
- Keep the fix inside Development seed behavior.
- Reuse the existing domain and persistence conventions.

## Safety Constraints
- No reset.
- No deletion of existing research orders.
- No destructive cleanup behavior.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs` if read-model protections need expansion

## Acceptance Criteria
- Applying `cockpit-validation` after prior research QA no longer throws a sequence collision.
- Reapplying the profile stays idempotent.
- Research UI expectations remain green.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- If the seed moves to a reserved sequence range, document that choice clearly.
- Keep the fix deterministic rather than switching to opportunistic random sequences.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the fix tightly scoped to sequence-safe idempotency.
