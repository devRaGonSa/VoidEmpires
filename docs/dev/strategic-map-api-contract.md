# Strategic Map Development API Contract

## Scope and Gating

This contract documents the current development-only strategic map read endpoint for future UI and sandbox work. It is not a production gameplay endpoint.

The route is mapped when the web host runs in `Development` or `VoidEmpires:DevEndpoints:Enabled=true`. If the development surface is disabled, the route returns `404 Not Found`. If the route is mapped but `ConnectionStrings:DefaultConnection` is empty, it returns `503 Service Unavailable`.

JSON payloads use current .NET enum names such as `YellowDwarf`, `Terran`, `Colonized`, `ScoutCraft`, `Stationed`, `Planned`, and `PlaceholderDerived`.

## Endpoint

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

## Map Result

`map` contains:

- `civilizationId`: requesting civilization id.
- `systems[]`: relevant systems for the current Phase 7F model.
- `routeFuelNotes[]`: capability notes for route/fuel previews.

Current relevance is inherited from the Phase 7E service: systems are included when they contain owned planets or active transfer origin/destination planets for the requesting civilization. There is no separate known-system, sensor, alliance, or espionage visibility model yet.

Each `systems[]` item contains:

- `systemId`, `galaxyId`, `systemName`
- `coordinateX`, `coordinateY`, `coordinateZ`
- `starType`
- `planets[]`
- `fleetPresence[]`
- `transferOverlays[]`

Each `planets[]` item contains identity and summary fields:

- `planetId`, `planetName`, `planetType`, `size`, `colonizationStatus`
- `isOwnedByRequestingCivilization`
- `civilizationId`: populated only when owned by the requesting civilization.
- `orbitalSlot`, `orbitRadius`, `orbitAngleDegrees`, `visualScale`
- `colonizationIntensity`, `urbanIntensity`, `industrialIntensity`, `militaryIntensity`, `orbitalPresenceIntensity`

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

## Side Effects

None. This endpoint is read-only. It does not create transfers, reserve fleets, complete transfers, charge resources, mutate stockpiles, persist route estimates, or write map state.

## Relationship to Other Dev Contracts

The strategic map read model reuses the same underlying persisted state summarized by visual state and fleet services:

- System visual state provides star, coordinate, layout, planet visual, marker, and transfer overlay concepts.
- Fleet UI state provides group command and route/fuel readiness hints for screen-specific fleet tooling.
- The strategic map endpoint consolidates map-level system, planet, fleet presence, transfer overlay, and route/fuel capability summaries.

Frontend prototypes should call `POST /api/dev/fleets/orbital-travel/estimate` when they need destination-specific route class, risk, placeholder fuel readiness, travel costs, and affordability.

## Limitations

- No final game UI.
- No production route.
- No route graph or pathfinding.
- No combat or interception.
- No alliances, diplomacy, sensors, or espionage visibility model.
- No fuel inventory, refueling, or fuel spending.
- No meshes, textures, binary assets, shader data, or heavy render payloads.
- Transfer progress remains a read-time visual approximation.
