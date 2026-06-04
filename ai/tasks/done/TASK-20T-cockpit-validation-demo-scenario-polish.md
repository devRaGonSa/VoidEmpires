# TASK-20T-cockpit-validation-demo-scenario-polish

---
id: TASK-20T-cockpit-validation-demo-scenario-polish
title: Cockpit validation demo scenario polish
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
  - docs
  - qa
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Tune `cockpit-validation` as a coherent demo scenario across all accepted cockpits.

## Purpose
Make the seeded experience feel like one consistent in-universe situation rather than a set of unrelated test fixtures stitched together.

## Current Problem
`cockpit-validation` is already functionally rich and idempotent, but the scenario may still feel like independent test data rather than one coherent situation shared by Galaxy, Planet, Construction, Research, Shipyard, Fleets, Defenses, and Ground Army.

## Context
- The seed currently supports Aurelia, Helios Gate, visible planets, resources, construction history, research queue or completed state, shipyard stock or queue, fleet transfer, defenses readiness, and ground-army readiness.
- The accepted cockpit set now depends heavily on this profile for QA and demo screenshots.

## Files to Inspect First
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `docs/dev/development-seed-profiles.md`
- current cockpit read-model tests
- frontend visible labels connected to the seeded scenario

## Implementation Requirements
1. Review the `cockpit-validation` scenario as a cross-cockpit narrative.
2. Ensure the same names and entities appear consistently, such as:
   - Aurelia
   - Helios Gate
   - Cinder Reach
   - Aether Crown
   - Void Seed Civilization if present
3. Avoid contradictory states, including:
   - resource stock that appears inconsistent between cockpits
   - queue counts that do not match summary panels
   - available actions that would predictably fail if confirmed
4. Do not add complex new data or new gameplay systems.
5. Small seed label or data improvements are allowed if they are tested.
6. Update seed docs with a short scenario description that QA can follow.

## UI/UX Requirements
- The demo should feel coherent when moving between cockpits.
- Screenshot QA should be able to tell one story about the same colony and its surrounding context.

## Backend/API Requirements
- Add or update tests if seed data changes.
- Keep `cockpit-validation` deterministic and idempotent.

## Safety Constraints
- No manual SQL.
- No destructive reset behavior.
- No combat or invasion scenario data that implies unsupported execution.

## Expected Files to Modify
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- focused seed or read-model tests under `tests/VoidEmpires.Tests/`
- `docs/dev/development-seed-profiles.md`
- related frontend labels only if needed for coherence

## Acceptance Criteria
- `cockpit-validation` reads as a coherent demo scenario.
- Tests pass.
- Frontend build passes if frontend labels changed.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched

## Notes / Residual Risks
- Do not overfit the seed to hide real blockers or data gaps.
- The scenario should remain honest about unsupported systems.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on coherence and seed truthfulness.
