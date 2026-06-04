# Fleet Development API Contracts

## Scope and Gating

These contracts document the current development-only fleet API surface for frontend and sandbox work. They are not production gameplay contracts.

Fleet cockpit v1 acceptance summary:

- Executable frontend flows: `estimate`, `create transfer`, `cancel transfer`, and guarded `complete-due`
- Prototype-only frontend flows: `split` and `merge`
- Visual goal: a mostly Spanish gameplay-first screen with compact development context, secondary technical ids, and a clear five-step order flow

Current persisted Shipyard/Fleet audit boundary:

- This block accepts Fleet read-state after a Shipyard mutation.
- This block does not accept transfer creation, cancellation, completion, split, merge, combat, or interception execution as part of the persisted Shipyard/Fleet QA loop.
- `POST /api/dev/fleets/orbital-groups/create-from-stock` exists, is Development-only, and mutates real stock, but it is still too thinly validated to include in the default repeated-QA flow.

Development fleet routes are mapped when the web host runs in `Development` or `VoidEmpires:DevEndpoints:Enabled=true`. If the development surface is disabled, routes return `404 Not Found`. If the route is mapped but `ConnectionStrings:DefaultConnection` is empty, persistence-backed endpoints return `503 Service Unavailable`.

JSON payloads use current .NET enum names such as `ScoutCraft`, `Stationed`, `Reserved`, `Planned`, `InTransit`, `Completed`, and `Cancelled`.

## Common Responses

| Status | Meaning |
|---|---|
| `200 OK` | Successful read, merge, cancel, complete, UI-state, or manifest operation. |
| `201 Created` | Successful group creation, split, or transfer creation. |
| `400 Bad Request` | Missing, empty, non-positive, or non-UTC required data. |
| `404 Not Found` | Development route is disabled, or a command targets missing persisted fleet data. |
| `409 Conflict` | Valid request shape rejected by current persisted state. |
| `503 Service Unavailable` | Persistence is not configured. |

Error payloads include `succeeded: false` and `errors[]` except for unmapped routes and persistence-gating responses. The split, merge, transfer estimate, transfer create, and transfer cancel commands also expose a `status` field so clients can distinguish validation, not-found, conflict, and success outcomes without parsing error strings.

## Consistency Review Notes

Phase 6W reviewed the fleet development endpoints and kept the existing response behavior unchanged. The current surface already uses the expected lightweight conventions for frontend tooling: validation failures return `400`, missing persistence returns `503`, disabled development routes return `404`, canceling a missing transfer returns `404`, state or invariant conflicts return `409`, successful reads return `200`, and successful mutating commands retain their nearby endpoint conventions of `200` or `201`.

## Current Status Transition Matrix

These matrices describe the statuses that the current fleet commands can observe or produce. They document the current implementation only; they do not introduce new statuses or future lifecycle steps.

### Orbital Group Status

| Current status | Estimate | Create transfer | Cancel transfer | Complete due | Split | Merge |
|---|---|---|---|---|---|---|
| `Stationed` | Allowed, no status change | Allowed, becomes `Reserved` | Rejected | Rejected | Allowed, remains `Stationed` | Allowed when both groups are compatible, target remains `Stationed` |
| `Reserved` | Rejected | Rejected | Allowed when the matching transfer belongs to the civilization, becomes `Stationed` | Allowed only through the global due-completion batch, becomes `Stationed` at destination | Rejected | Rejected |
| `Decommissioned` | Rejected by current stationed-only checks | Rejected | Rejected | Rejected | Rejected | Rejected |

### Orbital Transfer Status

| Current status | Create | Cancel | Complete due |
|---|---|---|---|
| `Planned` | New transfers start here | Allowed, becomes `Cancelled` | Allowed once `arrivalAtUtc` is due, becomes `Completed` |
| `InTransit` | Not currently created by these commands | Allowed, becomes `Cancelled` | Allowed once `arrivalAtUtc` is due, becomes `Completed` |
| `Completed` | Rejected for the same group because the group is no longer reserved or active | Rejected | Ignored on repeat completion |
| `Cancelled` | Rejected for the same active transfer because it is no longer active | Rejected | Ignored by due completion |

## Local Validation Flow

Use this flow only against a disposable local development database. These routes are development-only validation surfaces for backend and prototype work. They are not gameplay UI, combat execution, or interception execution contracts.

Start the backend with a placeholder-only local connection string pattern:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=<local-dev-db>;Username=<local-user>;Password=<local-password>"
dotnet run --project src/VoidEmpires.Web/VoidEmpires.Web.csproj
```

The current validation seed uses civilization `00000000-0000-0000-0000-000000000001`, owned planet `40000000-0000-0000-0000-000000000001`, and destination planet `40000000-0000-0000-0000-000000000002`. Use `ui-state` to copy the current stationed scout-group id and any active transfer id before running the mutating examples.

```powershell
Invoke-RestMethod -Method Get -Uri "http://localhost:5142/health"

Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"minimal-validation"}'

Invoke-RestMethod -Method Get -Uri "http://localhost:5142/api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001"

Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-travel/estimate" -ContentType "application/json" -Body '{"civilizationId":"00000000-0000-0000-0000-000000000001","orbitalGroupId":"<stationed-scout-group-id-from-ui-state>","destinationPlanetId":"40000000-0000-0000-0000-000000000002"}'

Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-transfers/create" -ContentType "application/json" -Body '{"civilizationId":"00000000-0000-0000-0000-000000000001","orbitalGroupId":"<stationed-scout-group-id-from-ui-state>","destinationPlanetId":"40000000-0000-0000-0000-000000000002","requestedAtUtc":"2026-06-02T13:00:00Z"}'

Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-transfers/cancel" -ContentType "application/json" -Body '{"civilizationId":"00000000-0000-0000-0000-000000000001","orbitalTransferId":"<active-or-new-transfer-id>"}'

Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-transfers/complete-due" -ContentType "application/json" -Body '{"nowUtc":"2026-06-02T12:00:00Z"}'
```

Expected validation outcomes:

- `GET /health` confirms the host is up and shows whether persistence is configured.
- `POST /api/dev/seeds/apply` is explicit and idempotent for the `minimal-validation` profile.
- `GET /api/dev/fleets/ui-state` is the safest place to inspect the current group state before mutating anything.
- `POST /api/dev/fleets/orbital-travel/estimate` is read-only. It previews duration, route profile, placeholder fuel-readiness metadata, and resource costs without reserving a group, charging resources, or creating a transfer.
- `POST /api/dev/fleets/orbital-transfers/create` charges the estimated travel resources, reserves the selected stationed group, and persists a planned transfer.
- `POST /api/dev/fleets/orbital-transfers/cancel` marks an active transfer as cancelled, returns the reserved group to `Stationed`, and does not refund previously charged travel resources.
- `POST /api/dev/fleets/orbital-transfers/complete-due` completes only transfers whose `arrivalAtUtc` is due, moves their groups to the destination planet, and is safe to repeat after completion because already completed transfers are ignored.

## Seed Re-Apply And Recovery

The `minimal-validation` seed is additive and idempotent, not a destructive reset.

- Re-applying it is safe when you only need missing baseline rows such as the sample civilization, seeded planets, baseline groups, or the owned-planet stockpile row to exist.
- Re-applying it does not delete extra transfers or groups created during validation, reset a mutated group back to its original stationed state, cancel or remove planned transfers, or refund spent travel resources.
- Re-applying it does top up the existing `Aurelia` resource stockpile to at least `125` credits, `160` metal, `100` crystal, and `50` gas, but it does not restore the exact pre-mutation balance when higher or lower values already exist.
- If a previous local validation run already changed fleet, transfer, or resource state and you need the original baseline back exactly, point the app to a fresh disposable local database and then apply `minimal-validation` again.
- Before re-running create-transfer validation, inspect `GET /api/dev/fleets/ui-state` to confirm the currently stationed groups, active transfers, and current-planet resource context that the next estimate or mutation will use.

## Non-Visual Validation Checklist

Use this checklist as the non-visual baseline for Fleet cockpit v1 before manual browser QA.

1. Run `dotnet build --no-restore` from the repository root.
2. Run `dotnet test --no-build` from the repository root.
3. Run `npm run build --prefix src/VoidEmpires.Frontend`.
4. Optionally apply the `minimal-validation` seed, inspect `GET /api/dev/fleets/ui-state`, and then call `POST /api/dev/fleets/orbital-travel/estimate` when you need non-visual confirmation that readiness metadata and estimate shapes still match the documented contracts.
5. Treat frontend mutation controls as development-only affordances. The current Fleet page may execute `create`, `cancel`, and a guarded `complete-due` flow, each behind an explicit confirmation path; `split` and `merge` must stay disabled or metadata-only.
6. The frontend should expose `complete-due` only when the current UI shows a due transfer and should refresh state after success instead of pretending a stale transfer already resolved locally.
7. For the estimate -> confirm -> create-transfer -> confirm -> cancel-transfer or complete-due paths, use [fleet-controlled-mutation-checklist.md](./fleet-controlled-mutation-checklist.md) as the focused non-visual regression pass.
8. For final manual browser QA, pair that checklist with [frontend-foundation-smoke-checklist.md](./frontend-foundation-smoke-checklist.md) and confirm a mostly Spanish gameplay-style screen with a scannable squad rail, readable selected-group panel, clear active-transfer states, explicit create/cancel/complete-due confirmations, readable resource support, and collapsed secondary technical panels.

## Endpoint Summary

Frontend execution status:

| Route | Frontend v1 status |
|---|---|
| `POST /api/dev/fleets/orbital-travel/estimate` | Executable as read-only route preview |
| `POST /api/dev/fleets/orbital-transfers/create` | Executable behind explicit confirmation |
| `POST /api/dev/fleets/orbital-transfers/cancel` | Executable behind explicit confirmation |
| `POST /api/dev/fleets/orbital-transfers/complete-due` | Executable behind explicit confirmation when a due transfer is visible |
| `POST /api/dev/fleets/orbital-groups/split` | Prototype-only, not executable from the current frontend |
| `POST /api/dev/fleets/orbital-groups/merge` | Prototype-only, not executable from the current frontend |

| Method | Route | Mode | Main side effect |
|---|---|---|---|
| `POST` | `/api/dev/fleets/orbital-groups/create-from-stock` | Mutating | Creates a stationed orbital group from local stock. |
| `GET` | `/api/dev/fleets/orbital-groups` | Read-only | None. Lists civilization groups. |
| `POST` | `/api/dev/fleets/orbital-groups/split` | Mutating | Decreases source quantity and creates a new group. |
| `POST` | `/api/dev/fleets/orbital-groups/merge` | Mutating | Adds source quantity to target and removes source. |
| `POST` | `/api/dev/fleets/orbital-travel/estimate` | Read-only | None. Previews duration, costs, affordability, route profile metadata, and placeholder fuel readiness. |
| `POST` | `/api/dev/fleets/orbital-transfers/create` | Mutating | Charges resources, reserves group, creates transfer. |
| `GET` | `/api/dev/fleets/orbital-transfers` | Read-only | None. Lists civilization transfers. |
| `POST` | `/api/dev/fleets/orbital-transfers/cancel` | Mutating | Cancels transfer and releases group reservation. |
| `POST` | `/api/dev/fleets/orbital-transfers/complete-due` | Mutating | Completes all due transfers and moves groups. |
| `GET` | `/api/dev/fleets/overview` | Read-only | None. Returns group state and command availability. |
| `GET` | `/api/dev/fleets/ui-state` | Read-only | None. Returns fleet screen state for dev UI prototypes. |
| `GET` | `/api/dev/fleets/action-manifest` | Read-only | None. Returns machine-readable fleet action metadata. |

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

Response: `status`, `succeeded`, `sourceOrbitalGroupId`, `newOrbitalGroupId`, `sourceQuantity`, `newQuantity`, `errors`.

Restrictions: source must belong to the civilization, be stationed, have no active transfer, and keep at least one unit after the split. `quantity` must be positive and lower than source quantity. Missing source groups return `404`; ownership or lifecycle/state rejections return `409`.

### Merge Groups

`POST /api/dev/fleets/orbital-groups/merge`

Request: `civilizationId`, `targetOrbitalGroupId`, `sourceOrbitalGroupId`.

Response: `status`, `succeeded`, `targetOrbitalGroupId`, `sourceOrbitalGroupId`, `targetQuantity`, `errors`.

Restrictions: target and source must differ, belong to the civilization, be stationed, have no active transfer, share the same current planet, and share the same asset type. Missing target or source groups return `404`; ownership or lifecycle/state rejections return `409`.

## Orbital Travel and Transfer Contracts

### Estimate Travel

`POST /api/dev/fleets/orbital-travel/estimate`

Request: `civilizationId`, `orbitalGroupId`, `destinationPlanetId`.

Response: `status`, `succeeded`, `orbitalGroupId`, `currentPlanetId`, `destinationPlanetId`, `abstractDistanceUnits`, `estimatedDuration`, `routeProfile`, `fuelReadiness`, `resourceCosts[]`, `canAfford`, `insufficientResources[]`, `errors`. Cost components contain `resourceType` and `quantity`; insufficient resources contain `resourceType`, `requiredQuantity`, and `availableQuantity`.

`routeProfile` contains `routeClass`, `distanceBand`, `riskBand`, `fuelMultiplier`, `complexityNotes[]`, and `isSupported`. Current deterministic distance bands are `LocalOrbit` for distance `1`, `InnerSystem` for distances `2-3`, `OuterSystem` for distances `4-6`, and `LongRange` for distances `7+`. Risk and fuel values are placeholders for future movement systems; the current fuel multiplier is `1.0` and supported route profiles are read-only metadata.

`fuelReadiness` contains `estimatedFuelUnitsRequired`, `estimatedRangeUnitsAvailable`, `isFuelReady`, `notReadyReason`, and `policy`. The current policy is `PlaceholderDerived`: it derives fuel and range from asset type, group quantity, abstract distance, and route profile metadata. It is not a real fuel inventory, does not persist fuel state, and does not charge fuel or resources.

Restrictions: read-only. The endpoint does not persist an estimate, reserve a group, create a transfer, charge resources or fuel, or mutate stockpiles. Groups with active transfers are rejected.

### Create Transfer

`POST /api/dev/fleets/orbital-transfers/create`

Request: `civilizationId`, `orbitalGroupId`, `destinationPlanetId`, `requestedAtUtc`.

Response: `status`, `succeeded`, `orbitalTransferId`, `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, `errors`.

Restrictions and side effects: `requestedAtUtc` must be UTC. The service calculates travel cost, spends those resources from the group's current planet, reserves the orbital group, and persists a planned transfer. It rejects missing groups, non-stationed groups, groups with active transfers, and destinations equal to the current planet. Cancellation does not refund charged resources.

### List Transfers

`GET /api/dev/fleets/orbital-transfers`

Query: required `civilizationId`; optional `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`, `status`.

Response: `succeeded`, `transfers[]`, `errors`. Each transfer contains `id`, `civilizationId`, `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, and `status`.

Restrictions: read-only. Missing `civilizationId` returns `400 Bad Request`.

### Cancel Transfer

`POST /api/dev/fleets/orbital-transfers/cancel`

Request: `civilizationId`, `orbitalTransferId`.

Response: `status`, `succeeded`, `orbitalTransferId`, `orbitalGroupId`, `errors`.

Restrictions and side effects: marks the transfer cancelled and releases the reserved group back to stationed status. It rejects completed transfers, already cancelled transfers, ownership mismatches, and non-reserved groups. It does not refund travel resources.

### Complete Due Transfers

`POST /api/dev/fleets/orbital-transfers/complete-due`

Request: `nowUtc`.

Response: `succeeded`, `completedCount`, `completedTransferIds[]`, `completedOrbitalGroupIds[]`, `errors`.

Restrictions and side effects: `nowUtc` must be UTC. This batch operation is not scoped by civilization. It completes due transfers, moves their groups to their destinations, and returns completed transfer/group ids.

## Fleet Overview and UI Readiness

### Fleet Overview

`GET /api/dev/fleets/overview`

Query: required `civilizationId`.

Response: `succeeded`, `overview`, `errors`. `overview` contains `civilizationId` and `groups[]`. Each group contains `id`, `civilizationId`, `originPlanetId`, `currentPlanetId`, `assetType`, `quantity`, `status`, `isStationedAwayFromOrigin`, `hasActiveTransfer`, `activeTransfer`, and `commands`. `activeTransfer` contains `id`, `destinationPlanetId`, `abstractDistanceUnits`, `departureAtUtc`, `arrivalAtUtc`, and `status`. `commands` contains `canCreateTransfer`, `canSplit`, `canMerge`, and `canCancelTransfer`.

Restrictions: read-only. Command availability is a UI planning hint derived from current group state and active transfers; it is not a replacement for command validation.

### Fleet UI State

`GET /api/dev/fleets/ui-state`

Query: required `civilizationId`.

Response: `succeeded`, `uiState`, `errors`. `uiState` contains `civilizationId`, `groups[]`, `resourceContexts[]`, `actionHints[]`, and `interceptionNotes[]`. Groups mirror the operational overview shape and include command availability plus `routeFuelReadiness` capability hints. Resource contexts include current-planet stockpile balances for `Credits`, `Metal`, `Crystal`, and `Gas` when a stockpile exists for a group's current planet. Action hints include action key, display name, route, method, read-only flag, and notes.

Persisted-QA interpretation for the current Shipyard/Fleet block:

- Use this endpoint after a Shipyard enqueue to confirm Fleet read-state still reflects the same stationed groups and only picks up the shared current-planet resource balance change.
- Do not treat the manifest entries for transfer, split, merge, or completion as approved steps in the persisted Shipyard/Fleet QA loop for this block.

`routeFuelReadiness` contains `canRequestTravelEstimate`, `requiresDestination`, `estimateActionKey`, `estimateRoute`, `fuelReadinessPolicy`, nullable `routeProfile`, nullable `fuelReadiness`, and `notes[]`. The current UI-state request has no destination context, so it does not include destination-specific `routeProfile` or `fuelReadiness` values. Frontend prototypes should call `fleet.travel.estimate` with `destinationPlanetId` when they need route class, risk, estimated fuel requirements, range readiness, travel costs, and affordability.

When a group has `activeTransfer`, the transfer summary can also include nullable `interceptionReadiness` metadata. Current own-transfer summaries surface `ObservedOwnTransfer` plus self-observed/non-hostile notes only; the UI-state endpoint does not expose hostile interception commands or reveal hidden foreign transfers.

Each `interceptionNotes[]` item is a lightweight reminder that interception readiness is read-only metadata only and that actual interception execution is not implemented.

Restrictions: read-only. This endpoint aggregates existing read models for dev UI prototypes; it does not create, cancel, complete, split, merge, estimate, charge, intercept, or mutate persisted state.

Frontend readiness guidance:

- Treat `commands` and `routeFuelReadiness.canRequestTravelEstimate` as readiness metadata only. Render them as `Ready` or `Blocked` labels instead of executable controls.
- Keep read-only inspection actions such as `estimate`, `overview`, `ui-state`, and manifest reads visually distinct from mutation contracts, even when an inspection route uses `POST`.
- The current frontend prototype may execute `POST /api/dev/fleets/orbital-travel/estimate` plus the explicitly confirmed development-only `POST /api/dev/fleets/orbital-transfers/create`.
- The cancel-transfer flow may execute `POST /api/dev/fleets/orbital-transfers/cancel` only for a currently visible active transfer with a known `transfer id` after explicit confirmation, and it should still summarize origin, current planet, destination, arrival, progress, and the no-refund rule before submission.
- After a successful cancel-transfer refresh, the frontend should clear the prepared cancel context, show an `estado actualizado` style cue, and prevent accidental re-submission for the now-inactive transfer.
- `create transfer` should remain behind the latest matching estimate context, summarize route and cost before submission, and clear or mark stale estimate data after a successful mutation refresh with an `estado actualizado` style cue.
- Frontend guards should invalidate the pending estimate when fleet UI state refreshes, when the selected group changes, when the destination changes, or when the estimated group is no longer stationed and create-ready.
- Frontend guards should block duplicate `create transfer` submissions while the request is in flight and should reject submission unless the currently selected group and destination still match the latest successful estimate.
- Frontend result feedback should distinguish success, validation (`400`), missing data (`404`), stale or already-active conflicts (`409`), persistence gating (`503`), network failures, and unexpected non-JSON or malformed JSON responses.
- Keep mutation contracts in clearly marked development or prototype sections. Current Fleet page work must not wire split or merge to ordinary gameplay-style buttons or click handlers, and only the guarded create-transfer, cancel-transfer, and complete-due flows may execute.
- Disabled prototype controls are allowed for discoverability only when they stay visibly guarded, non-submitting, and non-executable.
- When complete-due executes, require an explicit development-only affordance, show that it is a batch contract boundary, and refresh UI state before treating the due transfer as resolved in the cockpit.

### Fleet Action Manifest

`GET /api/dev/fleets/action-manifest`

Response: `succeeded`, `manifest`, `errors`. `manifest.actions[]` includes `actionKey`, `displayName`, `method`, `route`, `isReadOnly`, `requiredFields[]`, `successStatus`, `errorStatuses[]`, and `notes`.

Current action keys: `fleet.overview.read`, `fleet.uiState.read`, `fleet.interception.readiness.read`, `fleet.travel.estimate`, `fleet.transfer.create`, `fleet.transfer.cancel`, `fleet.transfer.complete`, `fleet.group.split`, and `fleet.group.merge`.

`fleet.uiState.read` notes that UI state includes route/fuel capability hints but no destination-specific estimates. `fleet.interception.readiness.read` cross-links the strategic-map interception-readiness endpoint for direct transfer-readiness inspection. `fleet.travel.estimate` is read-only in the manifest and is the source of route profile and fuel readiness previews when a UI has an explicit destination.

Restrictions: read-only and deterministic. This endpoint is a dev metadata surface for frontend tooling and does not require persistence beyond development-route gating.
Mutation entries exposed through this manifest remain documentation and prototype alignment aids only until a later task introduces an explicit dev-only execution workflow.

## Lifecycle Example

1. Inspect available actions with `GET /api/dev/fleets/action-manifest`.
2. Load screen state with `GET /api/dev/fleets/ui-state`.
3. Preview travel with `POST /api/dev/fleets/orbital-travel/estimate`.
4. Create a transfer with `POST /api/dev/fleets/orbital-transfers/create`; this charges resources and reserves the group.
5. Inspect state with `GET /api/dev/fleets/overview` or `GET /api/dev/fleets/ui-state`.
6. Cancel with `POST /api/dev/fleets/orbital-transfers/cancel`; the group is released and resources are not refunded.
7. Split a stationed group with `POST /api/dev/fleets/orbital-groups/split`.
8. Merge compatible stationed groups with `POST /api/dev/fleets/orbital-groups/merge`.
9. Create another transfer with `POST /api/dev/fleets/orbital-transfers/create`.
10. Complete due transfers with `POST /api/dev/fleets/orbital-transfers/complete-due`.
11. Inspect final state with `GET /api/dev/fleets/overview` or `GET /api/dev/fleets/ui-state`.

See `docs/dev/visual-state-sandbox.md` for related visual-state endpoints and the current rendering of fleet markers and transfer overlays.
See `docs/dev/strategic-map-api-contract.md` for the map-level read model that consolidates system, planet, fleet presence, transfer overlay, and route/fuel capability summaries for future UI work.
The strategic map contract also exposes `GET /api/dev/strategic-map/action-manifest`, which cross-links map, visual-state, fleet UI state, and fleet action manifest read actions.
