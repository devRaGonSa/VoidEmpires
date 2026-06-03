# TASK-18T-galaxy-cockpit-validation-profile-visual-data-alignment

---
id: TASK-18T-galaxy-cockpit-validation-profile-visual-data-alignment
title: Galaxy cockpit validation profile visual data alignment
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
  - qa
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: medium
---

## Goal
Ensure `cockpit-validation` provides enough strategic data for Galaxy to look useful during manual QA.

## Purpose
Prevent a technically non-empty but visually unhelpful Galaxy state that passes tests yet fails screenshot QA.

## Current Problem
Even after render restoration, the seed profile must still produce enough systems, planets, fleets, and transfer context for the cockpit to look meaningfully populated.

## Context
- `cockpit-validation` already enriches Planet, Research, and Shipyard with completed-history data.
- `development-seed-profiles.md` documents deterministic names such as `Helios Gate`, `Aurelia`, `Cinder Reach`, and `Aether Crown`.
- Galaxy is read-only, so visual richness must come from read-state rather than extra interactions.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- strategic map read-model services
- `docs/dev/development-seed-profiles.md`
- fleet or transfer seed sections
- tests added in `TASK-18Q`

## Implementation Requirements
1. Verify that `cockpit-validation` includes or preserves:
   - `Helios Gate`
   - `Aurelia`
   - `Cinder Reach`
   - `Aether Crown`
   - ownership and visibility state
   - fleet markers or orbital presence
   - at least one transfer overlay
   - enough data for legend and summary counts
2. If the profile is too sparse, enrich it carefully using existing seed conventions.
3. Keep the profile deterministic and idempotent over reused development databases.
4. Do not create duplicate baseline transfers or groups on reapply.
5. Add or extend tests that assert the expected non-empty Galaxy result shape.
6. Update the seed docs with explicit Galaxy expectations.

## UI/UX Requirements
- Seed data should support useful screenshot QA without overwhelming the cockpit.
- One rich strategic system is enough; do not flood the map with noise.

## Backend/API Requirements
- Use existing domain invariants and existing development seed patterns.
- No migrations.

## Safety Constraints
- No manual SQL.
- No destructive reset.
- No gameplay expansion beyond read-state richness.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- strategic map smoke tests
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- `cockpit-validation` yields a Galaxy read model that is both non-empty and visually useful.
- Reapply remains deterministic and idempotent.
- Backend validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Keep the profile conservative; this task should not turn Galaxy into a stress-test fixture.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Extend existing seed data instead of inventing a parallel profile.
