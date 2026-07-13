# TASK-54B

---
id: TASK-54B
title: Fleet creation from orbital stock
status: done
type: product
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 54"
priority: high
---

## Goal
Expose authoritative one-asset-type fleet creation from selected-planet orbital stock.

## Context
Reuse `OrbitalStockGroupService`; the backend must enforce ownership, positive quantity, stock availability, atomic stock reduction, and stationed group creation.

## Implementation steps
1. Audit the stock-to-group service, endpoint, fleet UI state, and registrations.
2. Add or refine the scoped API/read contract needed by the normal page.
3. Add backend and frontend coverage for asset type, quantity, ownership, stock spending, and refreshed group visibility.

## Files to read first
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalStockGroupService.cs`
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `tests/VoidEmpires.Tests/OrbitalStockGroupServiceTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs`

## Expected files to modify
- `src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs`
- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs`
- `src/VoidEmpires.Frontend/src/api/fleetCommandTypes.ts`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`

## Acceptance criteria
- Real selected-planet stock is exposed and can create one stationed group.
- Successful creation decreases stock by the selected quantity and reloads UI state.
- Invalid quantity, insufficient stock, and wrong ownership are rejected without mutation.
- No mixed compositions, templates, schema, or frontend fake state is added.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- frontend build and Block 54 guards

## Commit and push
Commit separately on the Block 54 branch; push after the complete block validates.

## Change Budget
- Prefer fewer than 5 files and under 200 changed lines.
