# TASK-17W-cockpit-validation-seed-profile-foundation

---
id: TASK-17W-cockpit-validation-seed-profile-foundation
title: Cockpit validation seed profile foundation
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: high
---

## Goal
Add a new `cockpit-validation` seed profile that prepares a richer cross-cockpit QA state without manual database edits.

## Purpose
Provide one reusable, richer QA baseline spanning Galaxy, Planet, Construction, Research, Shipyard, and Fleets while preserving the current `minimal-validation` safety net.

## Current Problem
`minimal-validation` is adequate for smoke tests but too thin for realistic cockpit QA. The product direction now calls for richer simulation data that remains deterministic, idempotent, and Development-only.

## Context
- The current seed already creates Aurelia, Helios Gate, resources, fleets, research state, and Shipyard readiness.
- `cockpit-validation` should either build on `minimal-validation` or share clear helper methods with it.
- This profile is intended for reproducible QA, not as a procedural scenario generator.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs`
- `ai/current-state.md`
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add support for a `cockpit-validation` profile.
2. Make the profile idempotent and deterministic.
3. Reuse the `minimal-validation` baseline where that reduces duplication and preserves existing expectations.
4. Ensure the richer profile includes, if supported by the existing backend:
   - one owned primary planet, Aurelia;
   - at least one visible non-owned planet;
   - meaningful resource state;
   - existing building state;
   - construction action data;
   - research available, blocked, and possibly queue data;
   - Shipyard available, blocked, stock, and possibly queue data;
   - Fleet stationed and transfer context;
   - stable ids for core entities.
5. Do not break `minimal-validation`.
6. Do not duplicate rows when the profile is applied repeatedly.
7. Add tests for:
   - profile acceptance;
   - presence of key ids;
   - idempotent reapply;
   - smoke assertions for at least Planet, Research, and Shipyard contexts.
8. Return useful apply messages that help manual QA.

## UI/UX Requirements
- The profile should support richer screenshot-friendly cockpit QA without manual SQL.
- It should enable visible variety in multiple cockpits while keeping primary gameplay UI Spanish.

## Backend/API Requirements
- Development-only profile support.
- No migrations unless unavoidable.
- Reuse existing domain services and invariants rather than bypassing them with direct ad hoc insertion patterns.

## Safety Constraints
- No destructive clearing.
- No production auth.
- No secrets.
- No cross-cockpit state that introduces unsupported gameplay systems such as combat or split/merge.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Application/Development/` seed profile contract files if present
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- Relevant cockpit endpoint or service tests
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- Applying `cockpit-validation` succeeds through the dev seed apply flow.
- Reapplying it is safe and non-duplicating.
- Existing `minimal-validation` tests still pass.
- The new profile creates a richer reusable cockpit QA baseline.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- This is a QA-rich baseline, not a generic scenario engine.
- If some cockpit-specific richness must remain in dedicated profiles, document that boundary clearly.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- If the shared helper extraction grows large, split follow-up tasks instead of overloading this one.
