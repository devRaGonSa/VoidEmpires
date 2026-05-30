# Visual State Sandbox Development Guide

## Purpose

The visual state sandbox is a development-only inspection surface for the current visual read contracts.

It is intended to validate data shape and visual interpretation before introducing a real renderer such as Three.js, Babylon.js, or a custom WebGL layer.

## Sandbox route

When the web project is running, open:

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

The system response now includes renderer-oriented metadata:

- `systemId`
- `galaxyId`
- `systemName`
- `coordinateX`, `coordinateY`, `coordinateZ`
- `star`
- `layoutHints`
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

### Cards

Simple inspection layout for payload sanity checks.

Use this mode when the pseudo-3D rendering makes debugging harder.

## Required runtime conditions

Development endpoints must be enabled.

They are available when either:

- the web host runs in Development environment, or
- `VoidEmpires:DevEndpoints:Enabled` is set to true

Persistence must also be configured because the visual endpoints read persisted data.

The web host only wires persistence-backed services when `ConnectionStrings:DefaultConnection` is non-empty.

## Local validation commands

From repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current baseline after Phase 5Y/5Z:

```text
at least 279 passing tests
```

## Suggested manual flow

1. Start PostgreSQL with a valid `ConnectionStrings:DefaultConnection`.
2. Start `VoidEmpires.Web` in Development environment.
3. Generate or reuse persisted galaxy/system/planet data.
4. Open `/dev/visual-state/index.html`.
5. Select `Planet` or `Solar system` data mode.
6. Paste a persisted planet id or solar system id.
7. Load the visual state.
8. Compare:
   - preview
   - intensity bars
   - profile panel
   - raw payload
   - system star metadata
   - layout hints

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
- no production route hardening beyond current development endpoint gating
- no direct auth/session integration in the sandbox
- no transfer overlays in system visual state yet
- no orbital group markers in system visual state yet

## Next likely improvements

Recommended next improvements are:

1. Add transfer overlays if moving fleets should appear in the system view.
2. Add orbital group markers if stationed fleets should be visible around planets.
3. Add route/fuel/travel-cost data only when movement mechanics are ready.
4. Replace the CSS pseudo-3D preview with a real renderer once contracts are stable.
