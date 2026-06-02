# Frontend Foundation Smoke Checklist

Use this checklist to validate the current frontend prototype without confusing it with production UI.

## Backend prerequisites

1. Start `VoidEmpires.Web` with development endpoints enabled.
2. Provide `ConnectionStrings:DefaultConnection` so persistence-backed reads do not return `503`.
3. Keep a sample civilization id available for strategic map and fleet UI state reads.

## Frontend checks

Manual visual validation is deferred for the current fleet execution block unless a clear regression appears. Use the steps below only when you need extra local confidence after the required non-visual validation commands succeed.

1. Run `npm install` in `src/VoidEmpires.Frontend`.
2. Run `npm run build`.
3. Run `npm run dev`.
4. Confirm `src/VoidEmpires.Frontend/package-lock.json` is tracked and neither `node_modules` nor `dist` is staged.
5. Open the frontend shell.
6. Confirm the header states that the frontend consumes development-only endpoints.
7. Open the `Strategic Map` route.
8. Enter a sample civilization id and confirm:
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
9. Open the `Fleets` route.
10. Enter the same civilization id and confirm:
   - loading state appears
   - fleet group summaries render as compact cards on success
   - the read-only estimate flow can submit `POST /api/dev/fleets/orbital-travel/estimate` and render loading, success, validation, not-found, conflict, or network-error feedback without creating a transfer
   - active transfers show a progress bar when departure and arrival timestamps are available
   - route/fuel and interception readiness notes render as metadata only
   - prototype mutation controls are visible but disabled, clearly marked as prototype-only, and never execute mutation endpoints
   - fleet and strategic-map manifests render as read-only contract panels
   - mutating manifest actions remain labeled but unavailable from the frontend
11. Confirm there are no buttons that execute mutating dev endpoints from either route.

## Repository validation

Run from repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

If npm registry access is unavailable in the current environment, record that limitation explicitly rather than claiming the frontend build passed.
