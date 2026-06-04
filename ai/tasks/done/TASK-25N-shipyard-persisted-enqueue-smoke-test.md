# TASK-25N

---
id: TASK-25N
title: Phase 25N - Shipyard persisted enqueue smoke test
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Add automated coverage proving that a Shipyard asset-production order can be created through the Development application path and persists across subsequent reads.

## Current problem

The Shipyard cockpit already exposes queue and option state, but the repository still lacks a deterministic backend proof that a real enqueue request creates durable rows visible in a follow-up read.

## Context from current implementation

- Shipyard v1 already has a controlled enqueue flow and a Development UI-state read model.
- Construction and Research already have persisted enqueue tests that can serve as the style and infrastructure baseline for this task.
- The reused Development database must remain safe, so the test must be deterministic and avoid depending on wall-clock completion.

## Files to read first

- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- src/VoidEmpires.Web/DevEndpointMappings.cs
- docs/dev/shipyard-cockpit-checklist.md
- ai/current-state.md

## Expected files to modify

- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs
- src/VoidEmpires.Application/Assets/*
- src/VoidEmpires.Web/DevEndpointMappings.cs

## Implementation requirements

1. Add an integration-style test using the existing test host and seed infrastructure.
2. Arrange a deterministic baseline using the existing Development seed approach.
3. Read Shipyard UI-state before mutation and capture the available production options, queue state, and relevant resource context.
4. Select a known valid production option using backend-compatible command metadata rather than UI display text.
5. Call the Shipyard enqueue endpoint.
6. Assert the response is successful and belongs to the expected civilization and planet.
7. Read Shipyard UI-state again and assert one of the following with deterministic checks:
   - queue count increased, or
   - a new order with the expected option metadata is present.
8. Assert any resource validation or mutation that is part of the current backend behavior.
9. If the seeded state already contains a queue item, isolate the test database state or select another valid option so the test remains deterministic.
10. Do not depend on due-time waiting or background worker execution.

## Backend/API requirements

- Use the current Development endpoint and command contract patterns.
- If the current response is missing metadata required for a reliable assertion, add only the smallest safe metadata needed and cover it with tests.
- Do not loosen validation or broaden the endpoint beyond the current Development use case.

## Script/QA requirements

- The resulting test should confirm the same persisted enqueue behavior that later PowerShell QA scripts depend on.
- If a scripted no-op state is expected in reused databases, keep that logic out of this positive-path smoke test.

## Safety constraints

- No manual SQL.
- No bypass of civilization or planet ownership rules.
- No wall-clock sleep or flaky timing behavior.
- No auto-completion or auto-cleanup of Shipyard orders.

## Acceptance criteria

- Automated coverage proves that a Shipyard enqueue request creates a real persisted order visible in follow-up reads.
- The test is deterministic on the existing test infrastructure.
- `dotnet test --no-build` passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If Shipyard resources are reserved rather than immediately deducted, later documentation and scripts must reflect that exact behavior instead of assuming parity with Construction.

