# Fleet Development API Contracts

## Scope and Gating

These contracts document the current development-only fleet API surface for frontend and sandbox work. They are not production gameplay contracts.

Development fleet routes are mapped when the web host runs in `Development` or `VoidEmpires:DevEndpoints:Enabled=true`. If the development surface is disabled, routes return `404 Not Found`. If the route is mapped but `ConnectionStrings:DefaultConnection` is empty, persistence-backed endpoints return `503 Service Unavailable`.

JSON payloads use current .NET enum names such as `ScoutCraft`, `Stationed`, `Reserved`, `Planned`, `InTransit`, `Completed`, and `Cancelled`.

## Common Responses

| Status | Meaning |
|---|---|
| `200 OK` | Successful read, merge, cancel, or complete operation. |
| `201 Created` | Successful group creation, split, or transfer creation. |
| `400 Bad Request` | Missing, empty, non-positive, or non-UTC required data. |
| `404 Not Found` | Development route is disabled, or cancel targets a missing transfer. |
| `409 Conflict` | Valid request shape rejected by current persisted state. |
| `503 Service Unavailable` | Persistence is not configured. |

Error payloads include `succeeded: false` and `errors[]` except for unmapped routes and persistence-gating responses.

## Consistency Review Notes

Phase 6W reviewed the fleet development endpoints and kept the existing response behavior unchanged. The current surface already uses the expected lightweight conventions for frontend tooling: validation failures return `400`, missing persistence returns `503`, disabled development routes return `404`, canceling a missing transfer returns `404`, state or invariant conflicts return `409`, successful reads return `200`, and successful mutating commands retain their nearby endpoint conventions of `200` or `201`.

## Endpoint Summary

| Method | Route | Mode | Main side effect |
|---|---|---|---|
| `POST` | `/api/dev/fleets/orbital-groups/create-from-stock` | Mutating | Creates a stationed orbital group from local stock. |
| `GET` | `/api/dev/fleets/orbital-groups` | Read-only | None. Lists civilization groups. |
| `POST` | `/api/dev/fleets/orbital-groups/split` | Mutating | Decreases source quantity and creates a new group. |
| `POST` | `/api/dev/fleets/orbital-groups/merge` | Mutating | Adds source quantity to target and removes source. |
| `POST` | `/api/dev/fleets/orbital-travel/estimate` | Read-only | None. Previews duration, costs, and affordability. |
| `POST` | `/api/dev/fleets/orbital-transfers/create` | Mutating | Charges resources, reserves group, creates transfer. |
| `GET` | `/api/dev/fleets/orbital-transfers` | Read-only | None. Lists civilization transfers. |
| `POST` | `/api/dev/fleets/orbital-transfers/cancel` | Mutating | Cancels transfer and releases group reservation. |
| `POST` | `/api/dev/fleets/orbital-transfers/complete-due` | Mutating | Completes all due transfers and moves groups. |
| `GET` | `/api/dev/fleets/overview` | Read-only | None. Returns group state and command availability. |

## Orbital Group Contracts

### Create From Stock

`POST /api/dev/fleets/orbital-groups/create-from-stock`

Request: `civilizationId`, `originPlanetId`, `currentPlanetId`, `assetType`, `quantity`.

Response: `succeeded`, `orbitalGroupId`, `errors`.

Restrictions: ids are required, `assetType` is required, and `quantity` must be positive. The backing service can reject insufficient stock or invalid ownership/state. This endpoint creates an orbital group only; it does not create transfers.

### List Groups

`GET /api/dev/fleets/orbital-groups`

Query: required `civilizationId`; optional `currentPlanetId`, `originPlanetId`, `assetType`, `status`.

Response: `succeeded`, `groups[]`, `errors`. Each group contains `id`, `civilizationId`, `originPlanetId`, `currentPlanetId`, `assetType`, `quantity`, `status`, and `isStationedAwayFromOrigin`.

Restrictions: read-only. Missing `civilizationId` returns `400 Bad Request`.

### Split Group

`POST /api/dev/fleets/orbital-groups/split`

Request: `civilizationId`, `sourceOrbitalGroupId`, `quantity`.

Response: `succeeded`, `sourceOrbitalGroupId`, `newOrbitalGroupId`, `sourceQuantity`, `newQuantity`, `errors`.

Restrictions: source must belong to the civilization, be stationed, have no active transfer, and keep at least one unit after the split. `quantity` must be positive and lower than source quantity.

### Merge Groups

`POST /api/dev/fleets/orbital-groups/merge`

Request: `civilizationId`, `targetOrbitalGroupId`, `sourceOrbitalGroupId`.

Response: `succeeded`, `targetOrbitalGroupId`, `sourceOrbitalGroupId`, `targetQuantity`, `errors`.

Restrictions: target and source must differ, belong to the civilization, be stationed, have no active transfer, share the same current planet, and share the same asset type.

## Orbital Travel and Transfer Contracts

### Estimate Travel

`POST /api/dev/fleets/orbital-travel/estimate`

Request: `civilizationId`, `orbitalGroupId`, `destinationPlanetId`.

Response: `succeeded`, `orbitalGroupId`, `currentPlanetId`, `destinationPlanetId`, `abstractDistanceUnits`, `estimatedDuration`, `resourceCosts[]`, `canAfford`, `insufficientResources[]`, `errors`. Cost components contain `resourceType` and `quantity`; insufficient resources contain `resourceType`, `requiredQuantity`, and `availableQuantity`.

Restrictions: read-only. The endpoint does not persist an estimate, reserve a group, create a transfer, charge resources, or mutate stockpiles. Groups with active transfers are rejected.

### Create Transfer

`POST /api/dev/fleets/orbital-transfers/create`

Request: `civilizationId`, `orbitalGroupId`, `destinationPlanetId`, `requestedAtUtc`.

Response: `succeeded`, `orbitalTransferId`, `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, `errors`.

Restrictions and side effects: `requestedAtUtc` must be UTC. The service calculates travel cost, spends those resources from the group's current planet, reserves the orbital group, and persists a planned transfer. It rejects missing groups, non-stationed groups, groups with active transfers, and destinations equal to the current planet. Cancellation does not refund charged resources.

### List Transfers

`GET /api/dev/fleets/orbital-transfers`

Query: required `civilizationId`; optional `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`, `status`.

Response: `succeeded`, `transfers[]`, `errors`. Each transfer contains `id`, `civilizationId`, `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, and `status`.

Restrictions: read-only. Missing `civilizationId` returns `400 Bad Request`.

### Cancel Transfer

`POST /api/dev/fleets/orbital-transfers/cancel`

Request: `civilizationId`, `orbitalTransferId`.

Response: `succeeded`, `orbitalTransferId`, `orbitalGroupId`, `errors`.

Restrictions and side effects: marks the transfer cancelled and releases the reserved group back to stationed status. It rejects completed transfers, already cancelled transfers, ownership mismatches, and non-reserved groups. It does not refund travel resources.

### Complete Due Transfers

`POST /api/dev/fleets/orbital-transfers/complete-due`

Request: `nowUtc`.

Response: `succeeded`, `completedCount`, `completedTransferIds[]`, `completedOrbitalGroupIds[]`, `errors`.

Restrictions and side effects: `nowUtc` must be UTC. This batch operation is not scoped by civilization. It completes due transfers, moves their groups to their destinations, and returns completed transfer/group ids.

## Fleet Overview

`GET /api/dev/fleets/overview`

Query: required `civilizationId`.

Response: `succeeded`, `overview`, `errors`. `overview` contains `civilizationId` and `groups[]`. Each group contains `id`, `civilizationId`, `originPlanetId`, `currentPlanetId`, `assetType`, `quantity`, `status`, `isStationedAwayFromOrigin`, `hasActiveTransfer`, `activeTransfer`, and `commands`. `activeTransfer` contains `id`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, and `status`. `commands` contains `canCreateTransfer`, `canSplit`, `canMerge`, and `canCancelTransfer`.

Restrictions: read-only. Command availability is a UI planning hint derived from current group state and active transfers; it is not a replacement for command validation.

## Lifecycle Example

1. Preview travel with `POST /api/dev/fleets/orbital-travel/estimate`.
2. Create a transfer with `POST /api/dev/fleets/orbital-transfers/create`; this charges resources and reserves the group.
3. Inspect state with `GET /api/dev/fleets/overview`.
4. Cancel with `POST /api/dev/fleets/orbital-transfers/cancel`; the group is released and resources are not refunded.
5. Split a stationed group with `POST /api/dev/fleets/orbital-groups/split`.
6. Merge compatible stationed groups with `POST /api/dev/fleets/orbital-groups/merge`.
7. Create another transfer with `POST /api/dev/fleets/orbital-transfers/create`.
8. Complete due transfers with `POST /api/dev/fleets/orbital-transfers/complete-due`.
9. Inspect final state with `GET /api/dev/fleets/overview`.

See `docs/dev/visual-state-sandbox.md` for related visual-state endpoints and the current rendering of fleet markers and transfer overlays.
