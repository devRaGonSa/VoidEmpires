# Visual State Sandbox Development Guide

## Purpose

The visual state sandbox is a development-only inspection surface for the current visual read contracts.

It is intended to validate data shape and visual interpretation before introducing a real renderer such as Three.js, Babylon.js, or a custom WebGL layer.

## Sandbox route

When the web project is running with development surfaces enabled, open:

```text
/dev/visual-state/index.html
```

The sandbox is served from static assets under:

```text
src/VoidEmpires.Web/wwwroot/dev/visual-state/
```

Current files:

- `index.html`
- `visual-state.css`
- `visual-state.js`

## Development surface gating

The sandbox static assets are gated by the same switch as development API endpoints.

They are served when either:

- the web host runs in Development environment, or
- `VoidEmpires:DevEndpoints:Enabled` is set to true

They are not served in Production by default.

This means `/dev/visual-state/index.html`, `/dev/visual-state/visual-state.css`, and `/dev/visual-state/visual-state.js` are development tooling, not production UI.

## Supported data modes

The sandbox can request either one planet or a complete solar system visual state.

### Planet mode

Endpoint:

```text
GET /api/dev/planets/{planetId}/visual-state
```

Expected response contract:

```text
PlanetVisualStateDto
```

Use this mode to inspect one planet's visual profile, visual seed, normalized intensities, ownership marker, and raw JSON payload.

### Solar system mode

Endpoint:

```text
GET /api/dev/solar-systems/{systemId}/visual-state
```

Expected response contract:

```text
SystemVisualStateDto
```

Use this mode to inspect all planets in a solar system ordered by orbital slot.

The system response includes renderer-oriented metadata:

- `systemId`
- `galaxyId`
- `systemName`
- `coordinateX`, `coordinateY`, `coordinateZ`
- `star`
- `layoutHints`
- `orbitalGroupMarkers`
- `transferOverlays`
- `planets`

## System visual metadata

### Star metadata

`star` contains:

- `starId`
- `starName`
- `starType`
- `visualClass`
- `lightIntensity`

`visualClass` is intended as a stable frontend styling/rendering hint. It should not be interpreted as gameplay physics.

Current classes include:

- `red_dwarf`
- `yellow_dwarf`
- `blue_giant`
- `white_dwarf`
- `neutron_star`

### Layout hints

`layoutHints` contains one hint per planet and is ordered by orbital slot.

Each hint contains:

- `planetId`
- `orbitalSlot`
- `orbitRadius`
- `orbitAngleDegrees`
- `visualScale`

These values are deterministic frontend hints derived from persisted orbital slot and planet size.

They are not a route graph, physics simulation, combat range model, or final orbital mechanics model.

A frontend renderer should treat them as default layout suggestions that can later be replaced by richer map/render contracts.

## System visual overlays

### Orbital group markers

`orbitalGroupMarkers` contains renderer-facing markers for orbital groups currently stationed on planets in the requested solar system.

Each marker contains:

- `orbitalGroupId`
- `civilizationId`
- `originPlanetId`
- `currentPlanetId`
- `assetType`
- `quantity`
- `status`
- `markerScale`
- `markerKind`

Current marker kinds include:

- `stationed_orbital_group`
- `reserved_orbital_group`
- `decommissioned_orbital_group`

These markers are read-only projections. They must not be used as commands to move, merge, split, attack, or reserve fleets.

### Transfer overlays

`transferOverlays` contains renderer-facing route overlays for non-completed and non-cancelled transfers touching planets in the requested solar system.

Each overlay contains:

- `transferId`
- `civilizationId`
- `orbitalGroupId`
- `originPlanetId`
- `destinationPlanetId`
- `status`
- `departureAtUtc`
- `arrivalAtUtc`
- `progress`
- `overlayKind`

Current overlay kinds include:

- `planned_transfer_route`
- `active_transfer_route`

`progress` is computed at read time from UTC departure/arrival timestamps. It is a visual approximation for UI rendering, not a combat timing rule, route physics model, or authoritative simulation clock.

## Render modes

### Pseudo-3D

CSS-only preview. This is not a real renderer.

It gives an early visual approximation of:

- planet type palette
- sphere-like lighting
- atmospheric glow
- city/industrial light hints
- orbital presence glow
- system star/orbit/planet-node composition
- orbital group markers near planets
- transfer route lines and progress dots between planets when both endpoints exist in the viewed system

### Cards

Simple inspection layout for payload sanity checks.

Use this mode when the pseudo-3D rendering makes debugging harder.

### Overlays panel

The sandbox includes an `Overlays` panel.

It lists:

- orbital group marker kind, asset type, quantity, and current planet id
- transfer overlay kind, progress percentage, origin planet id, and destination planet id

This panel is intended for quick payload interpretation. The raw JSON payload remains the authoritative inspection source.

## Required runtime conditions

Development endpoints and sandbox assets require the development surface gate to be enabled.

Persistence must also be configured because the visual endpoints read persisted data.

The web host only wires persistence-backed services when `ConnectionStrings:DefaultConnection` is non-empty.

## Local validation commands

From repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current baseline after Phase 6H/6I:

```text
289 passing tests
```

## Suggested manual flow

1. Start PostgreSQL with a valid `ConnectionStrings:DefaultConnection`.
2. Start `VoidEmpires.Web` in Development environment, or explicitly set `VoidEmpires:DevEndpoints:Enabled=true`.
3. Generate or reuse persisted galaxy/system/planet data.
4. Create orbital groups and/or transfer records if overlay rendering needs to be inspected.
5. Open `/dev/visual-state/index.html`.
6. Select `Solar system` data mode.
7. Paste a persisted solar system id.
8. Load the visual state.
9. Compare:
   - preview
   - intensity bars
   - profile panel
   - overlays panel
   - raw payload
   - system star metadata
   - layout hints
   - orbital group markers
   - transfer overlays

## Existing related endpoints

Galaxy/data setup endpoints already exist in the development API surface, including:

```text
POST /api/dev/galaxies/generate
POST /api/dev/players/starting-civilization
POST /api/dev/fleets/orbital-groups/create-from-stock
GET  /api/dev/fleets/orbital-groups
POST /api/dev/fleets/orbital-transfers/create
GET  /api/dev/fleets/orbital-transfers
POST /api/dev/fleets/orbital-transfers/complete-due
```

Use those endpoints as needed to create richer state before inspecting visual output.

## Current intentional limitations

- no Three.js
- no Babylon.js
- no WebGL
- no persisted visual customization
- no final game UI layout
- no direct auth/session integration in the sandbox
- no route graph or physical trajectory model in visual state yet
- no fuel/resource travel-cost model in visual state yet
- no combat/interception overlay model yet

## Next likely improvements

Recommended next improvements are:

1. Add route/fuel/travel-cost data only when movement mechanics are ready.
2. Replace the CSS pseudo-3D preview with a real renderer once contracts are stable.
3. Review the local `XUniversePlanet Generator Variator` Python prototype when entering the renderer/prototype phase.
