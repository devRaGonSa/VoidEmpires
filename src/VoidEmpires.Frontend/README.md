# VoidEmpires Frontend Prototype

This project is the current development-only frontend shell for VoidEmpires.

It is a Vite + React + TypeScript prototype that consumes backend readiness contracts and intentionally avoids production auth, gameplay mutation wiring, WebSockets, and 3D rendering.

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

The current frontend shell also keeps space reserved for:

- `GET /api/dev/solar-systems/{systemId}/visual-state`
- `GET /api/dev/planets/{planetId}/visual-state`

## Runtime assumptions

- Development endpoints must be enabled through the normal backend rules:
  - `ASPNETCORE_ENVIRONMENT=Development`, or
  - `VoidEmpires:DevEndpoints:Enabled=true`
- Persistence-backed reads still return `503` when `ConnectionStrings:DefaultConnection` is empty.
- A valid civilization id is required for the strategic map and fleet UI state pages.

## Current limitations

- Development endpoints are not production APIs.
- Readiness metadata is not gameplay authorization.
- Mutating backend actions are displayed only as manifest metadata.
- The frontend does not execute transfer creation, exploration creation, cancellation, or completion flows.
- No production authentication is implemented.
- No polling, WebSockets, or final renderer pipeline is implemented.
- No final game UI styling or 3D map is implemented.

## Smoke validation

See `docs/dev/frontend-foundation-smoke-checklist.md`.
