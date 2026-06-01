# VoidEmpires Frontend Prototype

This project is the current development-only frontend shell for VoidEmpires.

It is a Vite + React + TypeScript prototype that consumes backend readiness contracts and intentionally avoids production auth, gameplay mutation wiring, WebSockets, and 3D rendering. The current strategic map route is a visual readiness slice only: it renders a simple 2D map, supports read-only system and planet selection, and can inspect visual-state payloads from the existing development endpoints.

## Prerequisites

- Node.js 20+ with npm
- The backend web host from `src/VoidEmpires.Web`

## Configure the backend base URL

The frontend reads `VITE_VOIDEMPIRES_API_BASE_URL`.

If it is not provided, the frontend defaults to:

```text
http://localhost:5142
```

This matches the current `VoidEmpires.Web` `http` launch profile.

Example PowerShell session:

```powershell
$env:VITE_VOIDEMPIRES_API_BASE_URL = "http://localhost:5142"
```

## Install dependencies

```powershell
cd src/VoidEmpires.Frontend
npm install
```

## Run the frontend dev server

```powershell
cd src/VoidEmpires.Frontend
npm run dev
```

## Build and type-check

```powershell
cd src/VoidEmpires.Frontend
npm run typecheck
npm run build
```

## Consumed backend endpoints

- `GET /api/dev/strategic-map?civilizationId={id}`
- `GET /api/dev/fleets/ui-state?civilizationId={id}`
- `GET /api/dev/fleets/action-manifest`
- `GET /api/dev/strategic-map/action-manifest`
- `GET /api/dev/solar-systems/{systemId}/visual-state`
- `GET /api/dev/planets/{planetId}/visual-state`

## Current strategic map behavior

- Renders a deterministic SVG 2D map from backend system coordinates.
- Lets the user select a system from the map or the list below it.
- Lets the user inspect read-only system and planet metadata from the strategic-map payload.
- Lets the user load system and visible-planet visual-state previews as renderer-facing development payloads.
- Keeps the older system summary cards and readiness metadata panels available for inspection.

## Current fleet behavior

- Loads the fleet UI-state read model for a civilization id.
- Renders fleet summaries, resource contexts, interception notes, and read-only action manifests.
- Does not execute any manifest-listed mutating actions.

## Figma token foundation

Phase 9K adds a frontend token layer derived from `Xuniverse UI v1 - Modern Simple`.

The current stylesheet exposes:

- raw Figma palette variables such as `--ve-figma-bg`, `--ve-figma-panel`, and `--ve-figma-blue`
- semantic UI variables such as `--ve-color-bg`, `--ve-color-panel`, `--ve-color-border`, and `--ve-color-accent-blue`
- resource color variables such as `--ve-color-resource-metal`, `--ve-color-resource-crystal`, and `--ve-color-resource-deuterium`
- layout and surface variables such as `--ve-layout-topbar-height`, `--ve-layout-sidebar-width`, `--ve-radius-*`, and `--ve-shadow-*`

Current alignment intent:

- Figma desktop reference: `1440 x 960`
- topbar target height: `64px`
- sidebar target width: `230px`
- main content target start: about `260px`
- cards stay compact, dark, and low-noise

These tokens are foundation only. They support later UI alignment work without changing the current prototype into a final game UI.

## Current shell alignment

Phase 9L adds the first Figma-aligned shell layer:

- `AppShell` for the topbar + sidebar layout
- `TopResourceBar` for compact resource pills
- `SidebarNav` for Figma navigation labels with safe disabled placeholders
- `UiCard`, `UiBadge`, `UiProgressBar`, and `DevEndpointNotice` as reusable shell primitives

Only `Galaxia` and `Flotas` are active routes in the sidebar today. Other labels mirror the Figma navigation vocabulary but remain disabled until those read surfaces exist.

## Runtime assumptions

- Development endpoints must be enabled through the normal backend rules:
  - `ASPNETCORE_ENVIRONMENT=Development`, or
  - `VoidEmpires:DevEndpoints:Enabled=true`
- Persistence-backed reads still return `503` when `ConnectionStrings:DefaultConnection` is empty.
- A valid civilization id is required for the strategic map and fleet UI state pages.

## Current limitations

- Development endpoints are not production APIs.
- Readiness metadata is not gameplay authorization.
- Visual-state previews are renderer-facing dev contracts, not final rendering.
- Mutating backend actions are displayed only as manifest metadata.
- The frontend does not execute transfer creation, exploration creation, cancellation, or completion flows.
- No production authentication is implemented.
- No polling, WebSockets, or final renderer pipeline is implemented.
- No final game UI styling or 3D map is implemented.

## Smoke validation

See `docs/dev/frontend-foundation-smoke-checklist.md`.
