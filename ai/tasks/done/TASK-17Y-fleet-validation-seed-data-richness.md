# TASK-17Y-fleet-validation-seed-data-richness

---
id: TASK-17Y-fleet-validation-seed-data-richness
title: Fleet validation seed data richness
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: medium
---

## Goal
Enrich development seed data for Fleet QA while preserving the accepted Fleet cockpit behavior and boundaries.

## Purpose
Improve Fleet regression QA and make the Shipyard-to-Fleets handoff more legible with richer reproducible development data.

## Current Problem
Fleets is already accepted as a playable development cockpit, but richer seeded data would make screenshots, smoke checks, and cross-cockpit validation more meaningful. The current baseline is useful but limited in scenario variety.

## Context
- Fleets already supports reading state, selecting groups, estimating travel, creating and canceling transfers, and completing due transfers through safe dev flows.
- `minimal-validation` already includes stationed groups and one active transfer.
- Seed improvements must not enable split/merge or introduce combat.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- Fleet domain, application, and infrastructure service files
- `docs/dev/fleet-api-contracts.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add or extend a `fleet-validation` profile, or explicitly scope Fleet richness inside `cockpit-validation`.
2. Ensure seeded Fleet context includes, where supported safely:
   - multiple stationed groups at Aurelia;
   - one group or visible destination context away from Aurelia if the current model supports it;
   - one active transfer;
   - one due or arrived transfer only if safe and useful;
   - resource context at the origin planet;
   - at least one cargo or logistics example;
   - at least one escort or scout example;
   - a clear relationship to Shipyard stock where possible.
3. Preserve current accepted Fleet cockpit behavior.
4. Do not enable split/merge.
5. Do not add combat or interception execution.
6. Add tests for:
   - Fleet state loading;
   - stationed group count expectations;
   - active transfer presence;
   - due or arrived transfer presence if seeded;
   - idempotent reapply.
7. Update docs so QA knows which profile to use.

## UI/UX Requirements
- Fleet QA should show enough variety for screenshots and manual walkthroughs.
- The seeded state should make Shipyard handoff explanations feel grounded.

## Backend/API Requirements
- Use existing domain services and invariants.
- Keep seed behavior Development-only and deterministic.

## Safety Constraints
- No new movement command behavior beyond existing safe dev flows.
- No split or merge enablement.
- No combat.
- No destructive reset.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs` if shared idempotency coverage is needed
- `docs/dev/fleet-controlled-mutation-checklist.md` or related docs
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- `fleet-validation` or `cockpit-validation` creates richer Fleet state.
- Fleet tests pass.
- Accepted Fleet cockpit behavior is preserved.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- Do not overcomplicate seeded scenarios into pseudo-campaign logic.
- If due-transfer seeding adds brittleness, keep the profile simpler and document the tradeoff.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Prefer narrow scenario enrichment over broader Fleet feature work.
