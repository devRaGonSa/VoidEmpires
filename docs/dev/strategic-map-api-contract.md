# Strategic Map Development API Contract

## Scope and Gating

This contract documents the current development-only strategic map endpoints and action manifest for future UI and sandbox work. It is not a production gameplay endpoint.

The routes are mapped when the web host runs in `Development` or `VoidEmpires:DevEndpoints:Enabled=true`. If the development surface is disabled, routes return `404 Not Found`. If a persistence-backed route is mapped but `ConnectionStrings:DefaultConnection` is empty, it returns `503 Service Unavailable`.

JSON payloads use current .NET enum names such as `YellowDwarf`, `Terran`, `Colonized`, `ScoutCraft`, `Stationed`, `Planned`, `PlaceholderDerived`, `Owned`, `Visible`, `Unknown`, `AlreadyVisible`, and `AlreadyOwned`.

## Endpoints

### Strategic Map Read

`GET /api/dev/strategic-map?civilizationId={id}`

Query:

- `civilizationId`: required non-empty GUID. Scopes the map to the requesting civilization.

Responses:

| Status | Meaning |
|---|---|
| `200 OK` | Strategic map read model returned. |
| `400 Bad Request` | Missing or empty `civilizationId`. |
| `404 Not Found` | Development route is disabled. |
| `503 Service Unavailable` | Persistence is not configured. |

Response envelope:

- `succeeded`: `true` on success.
- `map`: strategic map result, or `null` on validation failure.
- `errors[]`: validation errors.

### Exploration Preview Read

`GET /api/dev/strategic-map/exploration-preview?civilizationId={id}`

Query:

- `civilizationId`: required non-empty GUID. Scopes the preview to the requesting civilization.

Responses:

| Status | Meaning |
|---|---|
| `200 OK` | Exploration preview read model returned. |
| `400 Bad Request` | Missing or empty `civilizationId`. |
| `404 Not Found` | Development route is disabled. |
| `503 Service Unavailable` | Persistence is not configured. |

Response envelope:

- `succeeded`: `true` on success.
- `preview`: exploration action preview result, or `null` on validation failure.
- `errors[]`: validation errors.

The preview is read-only metadata derived from map visibility. It does not create exploration missions, sensors, persisted fog-of-war, scanner data, espionage, diplomacy, combat, interception, route graph, pathfinding, or UI state.

### Exploration Mission Create

`POST /api/dev/strategic-map/exploration-missions/create`

Body:

- `civilizationId`: required non-empty GUID. Scopes the request to the creating civilization.
- `targetSystemId`: required non-empty GUID. Target system to explore.
- `targetPlanetId`: optional non-empty GUID. When provided, it must belong to `targetSystemId`.
- `requestedAtUtc`: required UTC timestamp.

Responses:

| Status | Meaning |
|---|---|
| `201 Created` | Planned exploration mission created. |
| `400 Bad Request` | Missing fields, invalid ids/timestamp, unknown civilization/system, or planet not in system. |
| `404 Not Found` | Development route is disabled. |
| `409 Conflict` | Current visibility/preview rules do not allow exploration for the target. |
| `503 Service Unavailable` | Persistence is not configured. |

Response envelope:

- `succeeded`: `true` on success.
- `mission`: created mission summary, or `null` on failure.
- `errors[]`: validation or eligibility errors.

The placeholder duration is deterministic: system-level missions are due 30 minutes after `requestedAtUtc`; planet-level missions are due 45 minutes after `requestedAtUtc`. Creation does not complete the mission, reveal visibility, assign fleets, charge resources, create sensors/scanners, mutate fog-of-war, or add route/pathfinding state.

### Exploration Mission Complete Due

`POST /api/dev/strategic-map/exploration-missions/complete-due`

Body:

- `nowUtc`: required UTC timestamp. Planned missions with `dueAtUtc <= nowUtc` are completed.

Responses:

| Status | Meaning |
|---|---|
| `200 OK` | Due planned missions were processed. |
| `400 Bad Request` | Missing or non-UTC `nowUtc`. |
| `404 Not Found` | Development route is disabled. |
| `503 Service Unavailable` | Persistence is not configured. |

Response envelope:

- `succeeded`: `true` on success.
- `completedCount`: number of missions completed in this call.
- `completedMissionIds[]`: completed mission ids.
- `errors[]`: validation errors.

Completion marks existing due planned missions as completed and records durable exploration knowledge for the target system and optional target planet. It does not reveal visibility, consume knowledge in strategic-map reads, create fog-of-war/sensor state, assign rewards, add combat/interception, or run a background worker.

### Minimal Exploration Lifecycle

The current development lifecycle is intentionally conservative:

1. `GET /api/dev/strategic-map/exploration-preview` can show `Unknown` systems or planets as eligible.
2. `POST /api/dev/strategic-map/exploration-missions/create` can persist a planned mission for an eligible target.
3. `POST /api/dev/strategic-map/exploration-missions/complete-due` can mark due planned missions completed.
4. Strategic map and visibility reads still report the target as `Unknown` until a future visibility-reveal phase consumes recorded exploration knowledge.

This lifecycle protects command plumbing and persistence only. Completion records exploration knowledge for later visibility work, but it does not grant rewards, create scanners/sensors, reveal fog-of-war, assign fleets, mutate resources, or expose final UI behavior.

### Strategic Map Action Manifest

`GET /api/dev/strategic-map/action-manifest`

Responses:

| Status | Meaning |
|---|---|
| `200 OK` | Strategic map action manifest returned. |
| `404 Not Found` | Development route is disabled. |

Response envelope:

- `succeeded`: `true` on success.
- `manifest.actions[]`: deterministic action metadata.
- `errors[]`: empty on success.

Each manifest action contains `actionKey`, `displayName`, `method`, `route`, `isReadOnly`, `requiredFields[]`, `successStatus`, `errorStatuses[]`, and `notes`.

Current action keys: `strategicMap.read`, `strategicMap.explorationPreview.read`, `exploration.mission.create`, `exploration.mission.completeDue`, `visual.system.read`, `visual.planet.read`, `fleet.uiState.read`, `fleet.actionManifest.read`, and `strategicMap.actionManifest.read`.

The manifest is read-only metadata for UI discovery. It does not require persistence and does not execute the listed actions.

## Map Result

`map` contains:

- `civilizationId`: requesting civilization id.
- `systems[]`: relevant systems for the current strategic map model.
- `routeFuelNotes[]`: capability notes for route/fuel previews.

Current relevance is inherited from the Phase 7E service: systems are included when they contain owned planets or active transfer origin/destination planets for the requesting civilization. There is no separate known-system, sensor, alliance, or espionage visibility model yet.

Visibility fields are inherited from the Phase 7I derived map visibility service. They are read-only annotations and do not represent persisted fog-of-war. Current levels are `Owned`, `Visible`, and `Unknown`; current reasons are `OwnedPlanet`, `SystemContainsOwnedPlanet`, and `NoKnownVisibilitySource`.

Each `systems[]` item contains:

- `systemId`, `galaxyId`, `systemName`
- `coordinateX`, `coordinateY`, `coordinateZ`
- `starType`
- `visibilityLevel`, `visibilityReason`
- `isVisible`
- `isOwnedByRequestingCivilization`: currently true when the derived visibility model finds an owned planet in the system.
- `explorationPreview`: read-only exploration preview metadata.
- `commands[]`: read-only system-level command availability metadata.
- `planets[]`
- `fleetPresence[]`
- `transferOverlays[]`

Each `planets[]` item contains identity and summary fields:

- `planetId`, `planetName`, `planetType`, `size`, `colonizationStatus`
- `isOwnedByRequestingCivilization`
- `visibilityLevel`, `visibilityReason`
- `isVisible`
- `explorationPreview`: read-only exploration preview metadata.
- `commands[]`: read-only planet-level command availability metadata.
- `civilizationId`: populated only when owned by the requesting civilization.
- `orbitalSlot`, `orbitRadius`, `orbitAngleDegrees`, `visualScale`
- `colonizationIntensity`, `urbanIntensity`, `industrialIntensity`, `militaryIntensity`, `orbitalPresenceIntensity`

Each `explorationPreview` item contains:

- `canPreviewExploration`: true only when the current visibility projection supports an exploration preview.
- `blockReason`: `None`, `AlreadyVisible`, `AlreadyOwned`, or `NoKnownVisibilitySource`.
- `note`: UI-facing explanation. This is not mission execution and not authorization.

Each `fleetPresence[]` item contains:

- `orbitalGroupId`, `planetId`, `assetType`, `quantity`, `status`, `markerKind`

Each `transferOverlays[]` item contains:

- `transferId`, `orbitalGroupId`, `originPlanetId`, `destinationPlanetId`
- `abstractDistanceUnits`, `status`, `departureAtUtc`, `arrivalAtUtc`
- `progress`, `overlayKind`

Each `routeFuelNotes[]` item contains:

- `actionKey`: currently `fleet.travel.estimate`
- `requiresDestination`: currently `true`
- `fuelReadinessPolicy`: currently `PlaceholderDerived`
- `note`

Each `commands[]` item contains:

- `actionKey`: current examples include `strategicMap.system.view`, `strategicMap.planet.viewDetail`, `exploration.preview`, `fleet.travel.estimate`, and `fleet.transfer.create`.
- `isAvailable`: whether the current read model can show the action as available.
- `blockReason`: `None`, `Unknown`, `NotVisible`, `NoFleetContext`, or `ExplorationPreviewUnavailable`.
- `note`: short UI-facing guidance. Fleet actions are capability hints only and must still use the existing fleet command endpoints and validation.

## Exploration Preview Result

`preview` contains:

- `civilizationId`: requesting civilization id.
- `systems[]`: derived exploration preview state for the current visibility model.
- `notes[]`: read-only policy notes.

Each `systems[]` item contains `systemId`, `visibilityLevel`, `canPreviewSystemExploration`, `blockReason`, `note`, and `planets[]`.

Each `planets[]` item contains `planetId`, `visibilityLevel`, `canPreviewPlanetExploration`, `blockReason`, and `note`.

Current placeholder rule: `Unknown` nodes can show exploration preview as available; `Visible` and `Owned` nodes are blocked as already visible or already owned. This is a UI-readiness preview only and does not create any mission or persisted knowledge.

## Side Effects

The exploration mission create endpoint persists a planned `ExplorationMission` only. The exploration mission complete-due endpoint updates due planned missions to completed and records exploration knowledge without changing current visibility reads. The current read endpoints remain read-only.

The strategic map endpoints do not create transfers, reserve fleets, complete transfers, charge resources, mutate stockpiles, persist route estimates, create sensor data, reveal visibility, create known-system/fog-of-war state, or write map state.

Command availability is UI metadata, not authorization and not command execution. It does not bypass fleet command validation.

## Relationship to Other Dev Contracts

The strategic map read model reuses the same underlying persisted state summarized by visual state and fleet services:

- System visual state provides star, coordinate, layout, planet visual, marker, and transfer overlay concepts.
- Fleet UI state provides group command and route/fuel readiness hints for screen-specific fleet tooling.
- The strategic map endpoint consolidates map-level system, planet, fleet presence, transfer overlay, exploration preview, and route/fuel capability summaries.
- The exploration preview endpoint exposes the same placeholder exploration readiness as a direct read contract for UI tooling.
- The exploration mission create endpoint consumes that preview eligibility and creates a planned mission for unknown targets only.
- The exploration mission complete-due endpoint closes the placeholder mission lifecycle and records exploration knowledge for a later visibility reveal model.
- The strategic map action manifest lists these related read actions so future prototypes can discover routes and required fields without hardcoding every contract.

Frontend prototypes should call `POST /api/dev/fleets/orbital-travel/estimate` when they need destination-specific route class, risk, placeholder fuel readiness, travel costs, and affordability.

## Limitations

- No final game UI.
- No production route.
- No route graph or pathfinding.
- No combat or interception.
- No alliances, diplomacy, sensors, or espionage visibility model.
- Unknown visibility can appear for strategic-map nodes that are relevant for another reason, such as an active transfer destination, but the strategic map endpoint does not return every persisted unknown system.
- Exploration preview is placeholder/read-only. Mission creation and completion are separate development-only POST endpoints; completion records exploration knowledge but does not create persisted fog-of-war or reveal visibility yet.
- No fuel inventory, refueling, or fuel spending.
- No meshes, textures, binary assets, shader data, or heavy render payloads.
- Transfer progress remains a read-time visual approximation.
