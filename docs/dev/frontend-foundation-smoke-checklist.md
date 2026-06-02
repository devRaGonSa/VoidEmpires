# Frontend Foundation Smoke Checklist

Use this checklist to validate the current frontend prototype without confusing it with production UI.

For the current Fleet estimate -> confirm -> create-transfer -> confirm -> cancel-transfer or complete-due paths, pair this document with `docs/dev/fleet-controlled-mutation-checklist.md`.

## Backend prerequisites

1. Start `VoidEmpires.Web` with development endpoints enabled.
2. Provide `ConnectionStrings:DefaultConnection` so persistence-backed reads do not return `503`.
3. Keep a sample civilization id available for strategic map and fleet UI state reads.

## Frontend checks

Use the steps below as the final manual QA checklist for Fleet cockpit v1 after the required non-visual validation commands succeed.

Fleet cockpit v1 acceptance boundary:

- Supported executable flows: `estimate`, `create transfer`, `cancel transfer`, and guarded `complete due`
- Prototype-only flows: `split` and `merge`
- Required validation commands: `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`

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
10. Enter the same civilization id and confirm the new cockpit layout renders in clearly separated sections:
   - a top command deck summarizes groups, active transfers, resource contexts, and action hints
   - an orbital group rail lists the available groups with readable asset type, current planet, and compact identifiers
   - a selected-group panel shows asset type, quantity, status, current planet, origin planet, readiness, and active transfer context when present
   - a command column groups estimate inputs, latest estimate state, guarded create-transfer confirmation, and guarded cancel-transfer confirmation together
   - prototype-only mutation controls remain separated from the executable command column
11. Within the same Fleet cockpit, confirm:
   - loading state appears
   - fleet group summaries render as readable rail cards and raw GUIDs stay secondary to ship or planet labels
   - the read-only estimate flow can submit `POST /api/dev/fleets/orbital-travel/estimate` and render loading, success, validation, not-found, conflict, or network-error feedback without creating a transfer
   - `create transfer` remains clearly labeled as a development action, requires explicit confirmation, and can submit only `POST /api/dev/fleets/orbital-transfers/create`
   - a successful create-transfer refresh clears stale estimate state and surfaces an `Estado actualizado desde la API.` cue in the mutation result area
   - `cancel transfer` remains clearly labeled as a development action, appears only when the focused active transfer is actually cancellable, and can submit only `POST /api/dev/fleets/orbital-transfers/cancel`
   - a successful cancel-transfer refresh clears stale cancel context, surfaces an `estado actualizado` cue, and keeps the no-refund rule visible
   - `complete due` remains clearly labeled as a development-only batch action, appears only when the UI shows a due transfer, and can submit only `POST /api/dev/fleets/orbital-transfers/complete-due`
   - a successful complete-due refresh surfaces an `estado actualizado` cue and removes the resolved due transfer from the active-transfer views
   - active transfers show route, timestamps, progress when available, due or in-flight state, and only the actions currently available in the selected-group and overview panels
   - route/fuel and interception readiness notes render as metadata only
   - prototype mutation controls for `split` and `merge` are visible but disabled, clearly marked as prototype-only, and never execute mutation endpoints
   - feedback areas for estimate, create-transfer, cancel-transfer, and complete-due results render readable success, warning, or error messaging rather than JSON-first output
   - fleet and strategic-map manifests render as read-only contract panels
   - mutating manifest actions remain labeled but unavailable from the frontend
12. For the final Fleet cockpit v1 visual review, confirm:
   - the development entry and endpoint context stay visible but compact, so gameplay panels appear in the first viewport earlier than before
   - the screen reads mostly in Spanish and no mixed English labels dominate the main flow
   - primary action labels read like gameplay actions, while cockpit or technical flavor stays secondary
   - the rail, selected-group panel, active-transfer panel, and main action column feel like a simple playable fleet screen rather than a dev console
   - the squad rail is compact and scannable, with ship type, quantity, location, destination, status, and readiness visible before compact ids
   - ids remain available as secondary metadata only and do not dominate selectors, headings, or route summaries
   - the action column reads in this order: escuadra, destino, calcular, revisar, confirmar
   - the five-step flow visually distinguishes pending, current, and completed states clearly enough to scan without reading every paragraph
   - create, cancel, and complete-due remain the only executable mutation paths and all three still require explicit confirmation
   - `split` and `merge` remain visible only as disabled or prototype-only controls
   - technical ids remain available for development use, but compact ids stay secondary to ship names, planet references, and route summaries
   - resource contexts stay readable by planet and do not get buried behind technical metadata
   - active transfers remain visible at a glance with route, status, timeline, progress, due-state cues, and only the currently available controlled actions
   - result and error feedback remain readable at a glance, and no raw enum numbers or `NetworkError` text dominates the panel
   - technical manifests and future mutation metadata stay collapsed or clearly secondary under development details
13. Confirm no buttons other than the explicit `create transfer`, `cancel transfer`, and `complete due` confirmation paths execute mutating dev endpoints from either route.

## Repository validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

If npm registry access is unavailable in the current environment, record that limitation explicitly rather than claiming the frontend build passed.

## Recovery notes

1. `POST /api/dev/fleets/orbital-transfers/create` and `POST /api/dev/fleets/orbital-transfers/cancel` mutate development data, so repeated validation runs can begin from a changed fleet/resource state.
2. Inspect `GET /api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001` before re-running mutation checks.
3. Re-apply `POST /api/dev/seeds/apply` with `{"profile":"minimal-validation"}` only to restore missing baseline rows.
4. The current `minimal-validation` seed is additive and idempotent. It does not delete extra transfers or groups, reset reserved/stationed state, or refill an existing stockpile after resources were spent.
5. When you need the original baseline back exactly, use a fresh disposable local database and then re-apply the seed.
