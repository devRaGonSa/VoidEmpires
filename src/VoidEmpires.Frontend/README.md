# VoidEmpires Frontend Prototype

This project is the current development-only frontend shell for VoidEmpires.

It is a Vite + React + TypeScript prototype that consumes backend readiness contracts and intentionally avoids production auth, gameplay mutation wiring, WebSockets, and 3D rendering. The current strategic map route is a visual readiness slice only: it renders a simple 2D map, supports read-only system and planet selection, and can inspect visual-state payloads from the existing development endpoints.
`package-lock.json` is intentionally tracked for deterministic frontend installs in this repository.

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
- Uses the current Figma-aligned panel language for the map stage, legend, focus summary, and selection details.
- Lets the user select a system from the map or the list below it.
- Lets the user inspect read-only system and planet metadata from the strategic-map payload.
- Lets the user load system and visible-planet visual-state previews as renderer-facing development payloads.
- Keeps readiness metadata visibly non-authoritative and non-mutating.

## Current fleet behavior

- Loads the fleet UI-state read model for a civilization id.
- Renders a Fleet command cockpit with a top summary deck, orbital-group rail, selected-group detail panel, command column, and prototype-only mutation manifest.
- Executes only the read-only travel estimate preview at `POST /api/dev/fleets/orbital-travel/estimate`.
- Executes `POST /api/dev/fleets/orbital-transfers/create` only after an explicit development-only confirmation tied to the latest matching estimate.
- Executes `POST /api/dev/fleets/orbital-transfers/cancel` only after an explicit development-only confirmation tied to a currently visible active transfer.
- Renders readable selected-group readiness, transfer status, resource contexts, interception notes, active-transfer progress bars, feedback panels, read-only action manifests, mutation confirmation metadata, and disabled prototype mutation controls.
- Keeps `complete-due`, `split`, and `merge` visibly guarded and non-executable from the UI.

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
- `create transfer` mutates development data and is allowed only behind the explicit confirmation flow.
- `cancel transfer` also mutates development data, is allowed only behind the explicit confirmation flow, and does not refund previously charged travel resources.
- Re-applying the `minimal-validation` seed is additive and idempotent, but it does not remove extra transfers, reset group state, or refill already-existing stockpiles after a mutation run.
- `complete-due`, `split`, `merge`, and exploration creation remain manifest metadata only.
- No production authentication is implemented.
- No polling, WebSockets, or final renderer pipeline is implemented.
- No final game UI styling or 3D map is implemented.

## Smoke validation

Required non-visual validation for the current fleet execution block:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

Optional seed and API confirmation:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"minimal-validation"}'
Invoke-RestMethod -Method Get -Uri "http://localhost:5142/api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001"
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-travel/estimate" -ContentType "application/json" -Body '{"civilizationId":"00000000-0000-0000-0000-000000000001","orbitalGroupId":"<stationed-group-id>","destinationPlanetId":"40000000-0000-0000-0000-000000000002"}'
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-transfers/create" -ContentType "application/json" -Body '{"civilizationId":"00000000-0000-0000-0000-000000000001","orbitalGroupId":"<stationed-group-id>","destinationPlanetId":"40000000-0000-0000-0000-000000000002","requestedAtUtc":"2026-06-02T13:00:00Z"}'
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/fleets/orbital-transfers/cancel" -ContentType "application/json" -Body '{"civilizationId":"00000000-0000-0000-0000-000000000001","orbitalTransferId":"<active-transfer-id>"}'
```

If a previous local run already mutated fleet state, inspect `ui-state` first. Re-applying `minimal-validation` restores missing baseline rows only; use a fresh disposable local database when you need the original transfer/resource baseline back.

Manual browser review is deferred for this block unless a clear regression appears. See `docs/dev/frontend-foundation-smoke-checklist.md`.

For the Fleet cockpit milestone, use the manual visual checklist there after the required non-visual validation commands succeed. Focus the browser review on command deck readability, selected-group context, guarded create/cancel confirmations, disabled prototype controls, and readable transfer/result feedback.
