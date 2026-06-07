# TASK-29R

---
id: TASK-29R
title: Construction QA preparation endpoint and persisted tests
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 29Q-29T"
priority: medium
---

## Goal
Add a Development-only endpoint and backing service that prepares construction state for manual QA, scoped to seeded IDs by default and optionally overridden.

## Context
`/construction` manual QA on shared Development DBs can be blocked by open construction rows. This task creates a scoped state-preparation path that removes blocking open orders, tops up resources to a safe minimum, and returns an explicit summary.

## Implementation steps

1. Implement an application contract and infrastructure service for scoped QA prep.
2. Map `POST /api/dev/construction/qa-state/prepare` in development endpoint mapping with optional request ids and default IDs.
3. Ensure the behavior:
   - Development-only execution
   - blocks/unblocks only target civilization+planet
   - clears only open orders for that planet
   - resource top-up only as needed
   - preserves completed/history rows and unrelated planets/orders
4. Add endpoint tests with fake/in-memory context:
   - outside-development unavailable
   - no persistence unavailable
   - prepares for enqueue
   - idempotent repeated call
   - no mutation on unrelated scope

## Files to read first

- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/Development`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/DevConstructionPersistedFlowTests.cs`
- `tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Application/Development/*`
- `src/VoidEmpires.Infrastructure/Development/*`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs` (or a focused new endpoint test file)

## Acceptance criteria

- New endpoint prepares scoped state and can unblock a real enqueue path.
- Repeated calls are safe and idempotent.
- Unrelated states are not modified.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
