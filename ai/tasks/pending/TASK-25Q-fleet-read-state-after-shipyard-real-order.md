# TASK-25Q

---
id: TASK-25Q
title: Phase 25Q - Fleet read-state after real Shipyard order
status: pending
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Ensure Fleet read-state remains stable and readable after a real persisted Shipyard asset-production order exists in the Development database.

## Current problem

Shipyard orders mutate persisted production state and may influence stock or summary projections. Fleets must continue to load cleanly after that mutation without accidental coupling regressions.

## Context from current implementation

- Fleets already provides read-state for orbital groups, transfers, and readiness notes.
- Shipyard already reads queue and stock state and will become part of backend-only persisted QA.
- This task protects cross-cockpit stability without introducing browser automation or Fleet mutations.

## Files to read first

- tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- docs/dev/fleet-api-contracts.md
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- tests/VoidEmpires.Tests/*Fleet*.cs
- docs/dev/persisted-gameplay-flow-checklist.md

## Implementation requirements

1. Add or strengthen an integration-style test that:
   - applies the deterministic Development seed
   - creates one real Shipyard order through the application path
   - reads Shipyard UI-state
   - reads Fleet UI-state
2. Assert that Fleet read-state still returns a successful response and meaningful data.
3. Assert that stationed groups, transfer summaries, and other current Fleet read fields remain readable.
4. Assert that the Shipyard mutation does not accidentally introduce movement, split, merge, or combat state.
5. Keep the test strictly backend-only and read-focused for Fleet.

## Backend/API requirements

- Use existing read endpoints and test infrastructure.
- Do not add new Fleet mutations for this task.
- If a cross-read regression exposes missing null-handling or shape assumptions, fix only the narrow issue required.

## Script/QA requirements

- The later Fleet read-state QA script will rely on this task to prove that a post-Shipyard read remains safe.
- Document any known empty-state behavior that scripts should treat as informative rather than exceptional.

## Safety constraints

- No fleet movement.
- No split or merge.
- No combat.
- No manual SQL.

## Acceptance criteria

- Automated coverage proves Fleet read-state still loads after a real Shipyard order exists.
- The task introduces no new gameplay behavior.
- `dotnet test --no-build` passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If Fleet UI-state indirectly surfaces stock values also touched by Shipyard, assertions should focus on stability and coherence, not on hard-coded values unless those values are deterministic.

