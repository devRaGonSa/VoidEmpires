# TASK-54C

---
id: TASK-54C
title: Fleet movement product flow
status: done
type: product
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 54"
priority: high
---

## Goal
Productize the existing estimate, transfer, cancellation, and arrival lifecycle as a compact five-step movement flow.

## Context
The page must select a stationed one-type group, estimate a valid destination, review authoritative cost/duration/arrival, confirm transfer, and render active movements with safe one-shot arrival refresh.

## Implementation steps
1. Audit transfer services, Fleet UI-state refresh, endpoint contracts, and UTC serialization.
2. Wire compact estimate/review/confirm/cancel actions using existing services.
3. Add regression coverage for reservation, cancellation, completion, active-read removal, UTC countdowns, and stale/missing estimates.

## Files to read first
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `src/VoidEmpires.Web/DevOrbitalTransferCreationEndpoints.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTransferCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`

## Expected files to modify
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts`

## Acceptance criteria
- Stationed fleets can estimate, review, and confirm valid transfers.
- Transfer creation reserves the group; valid cancellation releases it.
- Due arrival moves the group, restores Stationed, and removes the active transfer.
- Active movement timestamps are unambiguous UTC and expiry refresh fires once with manual retry on failure.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- frontend build and Block 54 guards

## Commit and push
Commit separately on the Block 54 branch; push after the complete block validates.

## Change Budget
- Prefer fewer than 5 files and under 200 changed lines.
