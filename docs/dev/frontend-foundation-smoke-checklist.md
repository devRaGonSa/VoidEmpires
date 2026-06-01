# Frontend Foundation Smoke Checklist

Use this checklist to validate the current frontend prototype without confusing it with production UI.

## Backend prerequisites

1. Start `VoidEmpires.Web` with development endpoints enabled.
2. Provide `ConnectionStrings:DefaultConnection` so persistence-backed reads do not return `503`.
3. Keep a sample civilization id available for strategic map and fleet UI state reads.

## Frontend checks

1. Run `npm install` in `src/VoidEmpires.Frontend`.
2. Run `npm run build`.
3. Run `npm run dev`.
4. Open the frontend shell.
5. Confirm the header states that the frontend consumes development-only endpoints.
6. Open the `Strategic Map` route.
7. Enter a sample civilization id and confirm:
   - loading state appears
   - request errors are rendered clearly when the backend rejects the request
   - system, planet, fleet, transfer, and readiness summary data render on success
   - systems render in the darker 2D map stage with the compact legend
   - selecting a system from the map updates the focus summary and selection detail panel
   - selecting a system from the chip list updates the same detail panel
   - selecting a visible planet updates the planet details and keeps command metadata read-only
   - `Load system visual state` returns a compact summary plus raw JSON details when the backend is running
   - `Load planet visual state` works for visible planets and stays read-only/dev-only
   - no button on the map page executes a gameplay mutation
8. Open the `Fleets` route.
9. Enter the same civilization id and confirm:
   - loading state appears
   - fleet group summaries render as compact cards on success
   - active transfers show a progress bar when departure and arrival timestamps are available
   - route/fuel and interception readiness notes render as metadata only
   - fleet and strategic-map manifests render as read-only contract panels
   - mutating manifest actions remain labeled but unavailable from the frontend
10. Confirm there are no buttons that execute mutating dev endpoints from either route.

## Repository validation

Run from repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

If npm registry access is unavailable in the current environment, record that limitation explicitly rather than claiming the frontend build passed.
