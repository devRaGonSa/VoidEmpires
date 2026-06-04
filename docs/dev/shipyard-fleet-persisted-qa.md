# Shipyard And Fleet Persisted QA

This document is the authoritative scope audit for the current persisted `Shipyard` and `Fleet read-state` development flow.
Use it together with `docs/dev/persisted-gameplay-flow-checklist.md`, `docs/dev/shipyard-cockpit-checklist.md`, `docs/dev/fleet-api-contracts.md`, and `docs/dev/development-seed-profiles.md`.

## Scope and safety rules

- Development-only flow.
- No production auth work is required for this block.
- No manual SQL is part of the accepted QA path.
- No destructive reset behavior is part of the accepted QA path.
- No hidden auto-completion or worker-driven completion is part of the accepted QA path.
- Shipyard completion processing is currently out of scope because the existing route is a global batch mutation.
- Fleet work in this block is read-only after the Shipyard mutation.
- Stock-to-fleet allocation is optional only for later follow-up work and is not part of the accepted default QA loop.

## Safe baseline

Recommended deterministic setup:

1. Apply `cockpit-validation`.
2. Use civilization `00000000-0000-0000-0000-000000000001`.
3. Use owned planet `40000000-0000-0000-0000-000000000001`.
4. Use a fresh disposable local database only when you need an exact pre-enqueue baseline again.
5. Reapplying the seed is allowed for deterministic read surfaces, but it does not remove manual orders, transfers, or allocation side effects.

Primary QA URLs:

- Shipyard: `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Fleets: `/fleets?civilizationId=00000000-0000-0000-0000-000000000001`

## Endpoint inventory

Shipyard persisted mutation and read surfaces:

- `POST /api/dev/assets/production/enqueue`
- `POST /api/dev/assets/production/process-due`
- `GET /api/dev/shipyard/ui-state?civilizationId={id}&planetId={id}`

Fleet read and optional allocation surfaces relevant to this block:

- `GET /api/dev/fleets/ui-state?civilizationId={id}`
- `GET /api/dev/fleets/overview?civilizationId={id}`
- `GET /api/dev/fleets/action-manifest`
- `POST /api/dev/fleets/orbital-groups/create-from-stock`

Excluded fleet mutation surfaces for this block:

- `POST /api/dev/fleets/orbital-travel/estimate`
- `POST /api/dev/fleets/orbital-transfers/create`
- `POST /api/dev/fleets/orbital-transfers/cancel`
- `POST /api/dev/fleets/orbital-transfers/complete-due`
- `POST /api/dev/fleets/orbital-groups/split`
- `POST /api/dev/fleets/orbital-groups/merge`

## Shipyard current runtime contract

### Enqueue route

Current real persisted order-creation route:

- `POST /api/dev/assets/production/enqueue`

Current safe request shape for the Shipyard flow:

- `civilizationId`
- `planetId`
- `target = Orbital`
- `spaceAssetType`
- `quantity`
- `requestedAtUtc` in UTC

Current audited behavior:

- The route is Development-only.
- The route returns `503` when persistence is not configured.
- The route rejects non-owned planets before enqueueing.
- The route enforces one open asset production order per planet.
- The route spends the full visible resource cost immediately when enqueue succeeds.
- The route persists a real `AssetProductionOrder` row immediately with `Status = Active`.
- The route does not increase orbital stock immediately; stock increases only when due processing runs later.

### Shipyard read route

Current truth-bearing read route:

- `GET /api/dev/shipyard/ui-state?civilizationId={id}&planetId={id}`

Current audited `shipyard` fields:

- planet and system context: `planetId`, `planetName`, `solarSystemId`, `solarSystemName`
- ownership context: `isOwnedByRequestingCivilization`, `ownerCivilizationId`, `ownerCivilizationName`, `controlStatus`
- local resources: `resourceStockpile[]` with `resourceType` and `quantity`
- readiness context: `buildingReadiness.shipyardLevel`, `fleetCommandCenterLevel`, `logisticsHubLevel`, `hasPopulationProfile`
- production catalog: `catalog[]` with `assetType`, `requiredBuildingType`, `requiredBuildingLevel`, `requiredOperatorCapacity`, `estimatedDuration`, `cost[]`, `currentStock`, `availabilityStatus`, `availabilityReason`
- production queue: `queue[]` with `orderId`, `assetType`, `quantity`, `sequence`, `status`, `startsAtUtc`, `endsAtUtc`, `isDue`
- local orbital stock: `orbitalStock[]` with `assetType` and `quantity`
- action summary: `queueActionStatus`, `queueActionReason`, `enqueueSupported`, `enqueueActionStatus`, `enqueueActionReason`, `completeDueSupported`, `completeDueActionStatus`, `completeDueActionReason`, `openQueueCount`, `dueQueueCount`
- diagnostics: `requestPlanetId`, `homePlanetId`, `hasResourceStockpile`, `hasOwnedShipyardBuilding`, `hasPopulationProfile`, `notes[]`

Persisted-QA interpretation:

- Before enqueue, use this endpoint to confirm at least one catalog item is `Available`.
- After enqueue, use this endpoint again to confirm queue count increased and local resources dropped.
- After enqueue, expect `orbitalStock[]` to remain unchanged until due processing is run.
- After enqueue, expect the selected catalog item to become blocked with `OpenProductionOrderExists` because only one open order is currently supported per planet.

## Completion route classification

Current route:

- `POST /api/dev/assets/production/process-due`

Classification for this block:

- global and unsafe for repeatable persisted QA

Reason:

- `AssetOrderProcessor` processes every due asset production order whose `EndsAtUtc <= nowUtc`.
- The route is not scoped by civilization, planet, or cockpit context.
- Running it against a reused Development database can complete unrelated due production rows and mutate local stock unexpectedly.

Accepted guidance:

- Do not include `process-due` in the default persisted Shipyard/Fleet QA loop.
- Do not rely on optional background workers for repeatable assertions.

## Fleet current runtime contract

### Fleet read routes

Current safe read routes for this block:

- `GET /api/dev/fleets/ui-state?civilizationId={id}`
- `GET /api/dev/fleets/overview?civilizationId={id}`
- `GET /api/dev/fleets/action-manifest`

Current audited `ui-state` fields:

- top-level: `civilizationId`, `groups[]`, `resourceContexts[]`, `actionHints[]`, `interceptionNotes[]`
- each `groups[]` row: `id`, `civilizationId`, `originPlanetId`, `currentPlanetId`, `assetType`, `quantity`, `status`, `isStationedAwayFromOrigin`, `hasActiveTransfer`, `activeTransfer`, `commands`, `routeFuelReadiness`
- each `activeTransfer`: `id`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, `status`, optional `interceptionReadiness`
- each `commands`: `canCreateTransfer`, `canSplit`, `canMerge`, `canCancelTransfer`
- each `resourceContexts[]` row: `planetId`, `balances[]`
- each `balances[]` row: `resourceType`, `quantity`

Current audited `overview` fields:

- top-level: `civilizationId`, `groups[]`
- each `groups[]` row: `id`, `civilizationId`, `originPlanetId`, `currentPlanetId`, `assetType`, `quantity`, `status`, `isStationedAwayFromOrigin`, `hasActiveTransfer`, `activeTransfer`, `commands`

Persisted-QA interpretation:

- Use `ui-state` as the primary Fleet re-read after a Shipyard enqueue because it includes both stationed-group state and current-planet resource contexts.
- Use `overview` only as an optional secondary read when you want a simpler group-status confirmation.
- Use `action-manifest` only as metadata; it does not make its mutation routes safe for this block.

### Optional allocation route classification

Current route:

- `POST /api/dev/fleets/orbital-groups/create-from-stock`

Classification for this block:

- Development-only, real mutation, optional follow-up candidate, not safe enough for the accepted default persisted loop

Reason:

- The route consumes real `OrbitalAssetStock` and creates a real stationed `OrbitalGroup`.
- `OrbitalStockGroupService` validates only request shape and available stock.
- The current service does not verify ownership of the source planet.
- The current service does not require `originPlanetId` and `currentPlanetId` to match.
- The current service does not scope the action to a shipyard-only or local-only workflow.
- Repeated runs against a reused database can consume stock and create extra groups without a built-in no-op guard.

Accepted guidance:

- Keep allocation out of the default Shipyard/Fleet persisted QA sequence.
- Treat allocation as an opt-in later task only if the endpoint is hardened, documented, and paired with explicit reused-database expectations.
- Current evidence for exclusion: the service consumes stock and creates a new group on every successful call, allows `currentPlanetId` to differ from `originPlanetId`, and does not currently prove ownership-scoped safety suitable for reused-database automation.
- No allocation helper script is part of this block. That omission is deliberate, not an unfinished gap in the accepted persisted QA flow.

## Accepted persisted QA sequence for this block

Copy-pasteable helper path for the accepted backend-only loop:

```powershell
dotnet run --project .\src\VoidEmpires.Web
.\scripts\dev-qa-baseline.ps1
.\scripts\dev-qa-create-shipyard-production-order.ps1 -ApplySeed
.\scripts\dev-qa-fleet-read-state.ps1
```

Expected operator interpretation:

- Successful Shipyard enqueue prints one real `OrderId`, queue before/after counts, resource deltas, and unchanged local orbital stock.
- Reused-database no-op Shipyard state prints that an open orbital production order already exists and exits without creating a second row.
- Fleet read-state prints group counts, stationed counts, active transfer counts, current resource-context summaries, and any visible transfer rows without mutating fleet state.
- No production environment, frontend clicking, manual SQL, movement, split, merge, combat, or due-processing step is part of this accepted flow.

### 1. Start the backend

```powershell
dotnet run --project .\src\VoidEmpires.Web
```

### 2. Apply the deterministic seed

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
```

### 3. Read Shipyard baseline

```powershell
Invoke-RestMethod "http://localhost:5142/api/dev/shipyard/ui-state?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
```

What to confirm:

- the selected owned planet is `Aurelia`
- queue state is readable
- local orbital stock is readable
- at least one orbital catalog item is `Available`

### 4. Create one real Shipyard order

```powershell
.\scripts\dev-qa-create-shipyard-production-order.ps1 -ApplySeed
```

Expected success:

- a real `OrderId` is returned
- the order is persisted immediately
- local resources are charged immediately
- queue before/after counts and stock/resource summaries are printed from the follow-up Shipyard read

Expected controlled reused-database failure:

- `Planet already has an open asset production order.`
- the helper exits cleanly after summarizing the current open queue instead of attempting a second mutation

### 5. Re-read Shipyard state

```powershell
Invoke-RestMethod "http://localhost:5142/api/dev/shipyard/ui-state?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
```

What to confirm:

- `openQueueCount` increased
- the new queue row is visible
- local resource balances decreased
- `orbitalStock[]` did not increase yet
- enqueue is now blocked by `OpenProductionOrderExists`

### 6. Re-read Fleet state after the Shipyard mutation

```powershell
.\scripts\dev-qa-fleet-read-state.ps1
```

What to confirm:

- stationed group ids, quantities, statuses, and active-transfer state are unchanged
- `resourceContexts[]` for the current planet now reflect the reduced balances caused by Shipyard enqueue
- no fleet transfer, split, merge, or allocation mutation happened as a side effect
- if no groups are currently visible, the helper explains that the read-state is valid and remains non-mutating

### 7. Reapply seed only for read-surface recovery

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
```

What to confirm:

- manual open asset production orders remain present
- seed reapply does not clear extra groups, transfers, or stock mutations
- if you need the exact pre-enqueue baseline again, switch to a fresh disposable local database

## Script set shipped for this block

Current accepted helper set:

- baseline helper: `scripts/dev-qa-baseline.ps1`
- Shipyard enqueue helper: `scripts/dev-qa-create-shipyard-production-order.ps1`
- Fleet read helper: `scripts/dev-qa-fleet-read-state.ps1`
- local script checker: `scripts/check-dev-qa-scripts.ps1`
- optional allocation helper: intentionally absent until allocation is explicitly re-approved

Known reused-database states scripts must treat as expected:

- Shipyard enqueue may legitimately return `Planet already has an open asset production order.`
- Reseeding does not remove existing open asset-production orders.
- Reseeding does not remove extra stationed groups or restore spent orbital stock after allocation.

## Dependency map

Shipyard flow:

- `Program.cs` -> `MapDevEndpointMappings()`
- `/api/dev/assets/production/enqueue` -> ownership check in `DevEndpointMappings` -> `IAssetProductionQueueService` -> `AssetProductionQueueService`
- `AssetProductionQueueService` -> `PlanetResourceStockpiles`, `PlanetBuildings`, `PlanetPopulationProfiles`, `AssetProductionOrders`
- `/api/dev/shipyard/ui-state` -> `IDevShipyardUiStateService` -> `DevShipyardUiStateService`
- `DevShipyardUiStateService` -> `Civilizations`, `Planets`, `SolarSystems`, `PlanetOwnerships`, `PlanetResourceStockpiles`, `PlanetBuildings`, `PlanetPopulationProfiles`, `AssetProductionOrders`, `OrbitalAssetStock`
- `/api/dev/assets/production/process-due` -> `IAssetOrderProcessor` -> `AssetOrderProcessor` -> `AssetProductionOrders`, `OrbitalAssetStock`, `PlanetaryAssetStock`

Fleet flow:

- `Program.cs` -> `MapDevEndpointMappings()` -> `MapDevFleetUiStateEndpoints()` and `MapDevFleetOperationalOverviewEndpoints()`
- `/api/dev/fleets/ui-state` -> `IDevFleetUiStateService` -> `DevFleetUiStateService`
- `DevFleetUiStateService` -> `IFleetOperationalOverviewService`, `IDevFleetActionManifestService`, optional `IInterceptionOpportunityService`, `PlanetResourceStockpiles`
- `/api/dev/fleets/overview` -> `IFleetOperationalOverviewService` -> `FleetOperationalOverviewService`
- `/api/dev/fleets/orbital-groups/create-from-stock` -> `IOrbitalGroupService` -> `OrbitalStockGroupService` -> `OrbitalAssetStock`, `OrbitalGroups`

## Existing coverage relevant to this audit

- `tests/VoidEmpires.Tests/DevShipyardUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevOrbitalGroupEndpointTests.cs`
- `tests/VoidEmpires.Tests/OrbitalStockGroupServiceTests.cs`

Current remaining limit:

- no safe default allocation loop exists yet for reused databases
