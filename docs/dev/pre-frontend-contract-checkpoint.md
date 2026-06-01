# Pre-Frontend Contract Checkpoint

## Purpose

This document is the current checkpoint for frontend foundation work.

It summarizes which backend/dev surfaces are stable enough to consume first, which routes are development-only, which operations are read-only versus mutating, and which payloads are still placeholder/readiness metadata rather than gameplay-complete systems.

This checkpoint is intentionally conservative. It is meant to prevent a frontend from treating current development contracts as production APIs or complete gameplay rules.

## Global Rules

- All routes in this checkpoint are development-only routes.
- Development routes are mapped only when the host runs in `Development` or `VoidEmpires:DevEndpoints:Enabled=true`.
- Disabled development routes return `404 Not Found`.
- Persistence-backed routes return `503 Service Unavailable` when `ConnectionStrings:DefaultConnection` is empty.
- Current payloads use .NET enum names directly.
- Readiness metadata is not authorization and is not gameplay execution.

## Intended First Frontend Entry Points

Recommended first reads:

- `GET /api/dev/strategic-map`
- `GET /api/dev/fleets/ui-state`
- `GET /api/dev/fleets/action-manifest`
- `GET /api/dev/strategic-map/action-manifest`
- `GET /api/dev/solar-systems/{systemId}/visual-state`
- `GET /api/dev/planets/{planetId}/visual-state`

Recommended supporting reads after the shell is working:

- `GET /api/dev/strategic-map/exploration-preview`
- `GET /api/dev/strategic-map/exploration-missions`
- `GET /api/dev/strategic-map/exploration-knowledge`
- `GET /api/dev/strategic-map/sensor-profiles`
- `GET /api/dev/strategic-map/detection-coverage`
- `GET /api/dev/strategic-map/interception-opportunities`
- `GET /api/dev/strategic-map/alliances/readiness`
- `GET /api/dev/strategic-map/alliances/pacts/readiness`
- `GET /api/dev/strategic-map/diplomatic-contacts`

Recommended first mutating routes for dev tooling only:

- `POST /api/dev/strategic-map/exploration-missions/create`
- `POST /api/dev/strategic-map/exploration-missions/complete-due`
- `POST /api/dev/fleets/orbital-travel/estimate`
- `POST /api/dev/fleets/orbital-transfers/create`

## Read-Only vs Mutating

Read-only development routes:

- Strategic map read, manifests, visual-state reads, fleet UI state, fleet overview, orbital-group list, orbital-transfer list, exploration preview, exploration mission list, exploration knowledge, sensor profiles, detection coverage, interception opportunities, diplomatic contacts, alliance readiness, and alliance pact readiness.

Development-mutating routes:

- Exploration mission create and complete-due.
- Orbital group create-from-stock, split, and merge.
- Orbital transfer create, cancel, and complete-due.

Important caveat:

- `POST /api/dev/fleets/orbital-travel/estimate` is a POST route but remains read-only. It previews route, cost, affordability, route-profile, and placeholder fuel-readiness data only.

## Stable Payloads For First Frontend Slice

### Strategic map

Use `GET /api/dev/strategic-map` as the main screen entry point.

Current strengths:

- Stable top-level shape for systems, planets, fleet presence, transfer overlays, command availability, route/fuel notes, diplomacy metadata, alliance metadata, and alliance pact metadata.
- Visibility is explicit through `visibilityLevel`, `visibilityReason`, and `isVisible`.
- Unknown planets are intentionally sanitized rather than omitted.
- Command entries are machine-readable capability hints for UI affordances.

Current limitations:

- Relevance is intentionally narrow and does not enumerate every persisted system.
- Visibility is derived from ownership and exploration knowledge only.
- Sensor, detection, interception, alliance, and pact metadata do not reveal new systems or targets.
- Command availability is not command authorization.

### Fleet UI state

Use `GET /api/dev/fleets/ui-state` for fleet screen composition.

Current strengths:

- Consolidates group state, active transfer summaries, resource context, action hints, and interception notes.
- Includes route/fuel readiness capability hints for each group.

Current limitations:

- Concrete route profile and fuel readiness values remain null until a destination-specific travel estimate is requested.
- Interception metadata is read-only and does not imply an executable interception flow.

### Visual state

Use `GET /api/dev/solar-systems/{systemId}/visual-state` and `GET /api/dev/planets/{planetId}/visual-state` for renderer-facing read data.

Current strengths:

- Stable visual metadata for star styling, layout hints, orbital group markers, and transfer overlays.
- Useful for a frontend map/detail prototype and for validating renderer payload assumptions.

Current limitations:

- No production renderer contract yet.
- No route graph, physics, meshes, shaders, binary assets, or final 3D pipeline.
- Overlay progress is a read-time visual approximation only.

## Exploration Tooling Lifecycle

Current conservative flow:

1. Read map visibility and unknown-node eligibility from `GET /api/dev/strategic-map` or `GET /api/dev/strategic-map/exploration-preview`.
2. Create a planned mission with `POST /api/dev/strategic-map/exploration-missions/create`.
3. Inspect mission rows with `GET /api/dev/strategic-map/exploration-missions`.
4. Complete due missions with `POST /api/dev/strategic-map/exploration-missions/complete-due`.
5. Inspect recorded knowledge with `GET /api/dev/strategic-map/exploration-knowledge`.
6. Re-read `GET /api/dev/strategic-map` to see knowledge-derived visibility.

What this does not mean:

- No rewards.
- No fleet assignment.
- No real scanner model.
- No fog-of-war persistence.
- No combat, interception, or pathfinding.

## Sensors, Detection, and Interception

Current meaning:

- Sensor profiles are placeholder readiness metadata for owned planets and stationed groups.
- Detection coverage is conservative local-system placeholder metadata.
- Interception opportunities are read-only readiness metadata for currently exposed transfers.

Current non-goals:

- No hidden-target reveal from sensors alone.
- No real scanning mechanics.
- No executable interception command.
- No combat resolution.

Frontend rule:

- Treat these as annotations and badges, not as visibility sources or action authority.

## Diplomacy, Alliances, and Pacts

Current meaning:

- Diplomatic contacts are civilization-scoped relationship metadata.
- Alliance readiness is requesting-civilization membership metadata.
- Alliance pact readiness is requesting-civilization pact metadata for alliances where the requester has an active membership.

Current non-goals:

- No shared visibility.
- No shared sensors, detection, or interception data.
- No allied fleet/system reveal.
- No trade execution.
- No war state.
- No defense/intervention automation.
- No espionage.
- No combat permissions.

Frontend rule:

- Treat diplomacy, alliance, and pact payloads as informational side panels only.

## Recommended First Frontend Slice

1. Build a dev-only shell/layout with no production auth assumptions.
2. Add a small API client for the strategic map, fleet UI state, visual state, and manifest reads.
3. Render strategic map system and planet summaries from `GET /api/dev/strategic-map`.
4. Render fleet side-panel state from `GET /api/dev/fleets/ui-state`.
5. Use action manifests to avoid hardcoding route assumptions for first prototypes.
6. Use system and planet visual-state reads for detail panels or renderer experiments.
7. Keep exploration, diplomacy, alliance, pact, sensor, detection, and interception views read-only at first.

## Current Frontend Foundation Status

The repository now includes `src/VoidEmpires.Frontend`, a Vite + React + TypeScript prototype shell.

Current implemented frontend behavior:

- strategic map route with civilization-id input, loading/error states, system and planet summaries, and conservative readiness metadata rendering
- fleet route with civilization-id input, loading/error states, fleet group summaries, resource/interception notes, and read-only backend action manifest panels
- visible warnings that development endpoints are not production APIs and that readiness metadata is not gameplay authorization

Current non-goals remain unchanged:

- no production auth
- no gameplay mutation buttons
- no WebSockets
- no final UI
- no 3D renderer

Operational docs:

- `src/VoidEmpires.Frontend/README.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

Not recommended for the first slice:

- Production authentication flow.
- Final UI styling assumptions.
- Real-time polling strategy assumptions.
- 3D renderer commitments.
- Gameplay-complete movement, combat, sensor, or diplomacy behavior.

## Warnings

- Development endpoints are not production endpoints.
- Readiness metadata is not gameplay authorization.
- Alliance and pact metadata do not grant shared visibility.
- Sensor, detection, and interception metadata are not full gameplay systems.
- Current mutating routes are developer tooling and may remain temporary.
- Current visual contracts are renderer-facing hints, not final presentation contracts.

## Related Documents

- `docs/dev/strategic-map-api-contract.md`
- `docs/dev/fleet-api-contracts.md`
- `docs/dev/visual-state-sandbox.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
