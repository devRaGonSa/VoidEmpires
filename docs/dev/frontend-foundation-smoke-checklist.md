# Frontend Foundation Smoke Checklist

Use this checklist to validate the current frontend prototype without confusing it with production UI.

For the current Fleet estimate -> confirm -> create-transfer -> confirm -> cancel-transfer or complete-due paths, pair this document with `docs/dev/fleet-controlled-mutation-checklist.md`.
For the current Galaxy, Planet, Construction, and Research cockpit review, also pair this document with `docs/dev/strategic-map-cockpit-checklist.md`, `docs/dev/planet-cockpit-checklist.md`, `docs/dev/construction-cockpit-checklist.md`, and `docs/dev/research-cockpit-checklist.md`.
For the current module boundary model and placeholder responsibilities, also pair this document with `docs/dev/planet-module-boundaries.md`.

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
   - the top cockpit strip renders command input, strategic summary, and read-only safety rules first
   - systems render in the darker 2D map stage with a readable legend for ownership, visibility, fleets, and transfer overlays
   - selecting a system from the map updates the active focus rail and the focused-system panel
   - selecting a system from the chip list updates the same focused-system panel
   - the primary strategic surface reads mostly in Spanish and keeps technical or API-oriented language out of the first viewport where practical
   - selecting a visible planet updates the planet details, keeps command metadata read-only, and shows the current navigation intent toward `Flotas` and `Planeta`
   - the technical drawer starts collapsed and keeps renderer payloads, readiness ledgers, and raw JSON secondary
   - `Load system visual state` returns a compact summary plus raw JSON details when the backend is running
   - `Load planet visual state` works for visible planets and stays read-only/dev-only
   - no button on the map page executes a gameplay mutation
9. Open the `Planet` route.
10. Enter the same civilization id or arrive from Galaxy and confirm:
   - the seeded dashboard URL `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` opens the cockpit with the same context as the Galaxy handoff
   - `/planet` loads as a real cockpit route
   - when `planetId` is omitted, the page defaults to the home or first owned planet
   - the first viewport prioritizes identity, colony state, resources, buildings, queue, and guarded actions
   - resources, production, and capacity read clearly in Spanish
   - buildings are grouped into readable categories
   - the queue shows readable timing and status cues, or `No hay construcciones en cola.`
   - construction actions show readable availability states and only available actions can be prepared
   - the enqueue flow requires explicit confirmation and refreshes after success
   - the complete-due area stays disabled with `No disponible en esta build`
   - links back to Galaxy and toward Fleets preserve context
   - diagnostics remain collapsed by default
11. Open the `Construction` route with civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`, then confirm:
   - the seeded construction URL `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` opens the focused infrastructure cockpit
   - `/construction` loads as a focused cockpit route for the seeded `Aurelia` colony
   - the seeded scenario exposes existing buildings, visible stockpile, visible local economy, and an empty or clearly readable queue
   - the catalog shows both at least one available action and at least one blocked action for comparison
   - blocked actions keep readable Spanish guidance and their disabled buttons remain visually secondary
   - the confirmation flow still requires explicit acknowledgement before `Enviar orden`
   - a successful enqueue refreshes the queue and keeps diagnostics collapsed
12. Open the `Research` route with civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`, then confirm:
   - the seeded research URL `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` opens the cockpit with deterministic context
   - the route keeps the `Aurelia` context tied to the seeded `Helios Gate` scenario
   - the top cockpit strip shows context, summary, and queue before the catalog
   - the catalog shows Spanish technology names, meaningful categories, and both available and blocked research cards
   - blocked cards keep readable Spanish guidance and their disabled buttons remain visually secondary
   - preparing an available item opens the guarded confirmation panel before `Enviar orden`
   - a successful enqueue refreshes the queue and catalog from the backend-confirmed read model
   - `Completar investigaciones vencidas` stays visibly disabled when the backend route is not scoped safely to this cabin
   - diagnostics remain collapsed and keep technical details secondary
13. Open the `Fleets` route.
14. Enter the same civilization id and confirm the new cockpit layout renders in clearly separated sections:
   - a top command deck summarizes groups, active transfers, resource contexts, and action hints
   - an orbital group rail lists the available groups with readable asset type, current planet, and compact identifiers
   - a selected-group panel shows asset type, quantity, status, current planet, origin planet, readiness, and active transfer context when present
   - a command column groups estimate inputs, latest estimate state, guarded create-transfer confirmation, and guarded cancel-transfer confirmation together
   - prototype-only mutation controls remain separated from the executable command column
15. Within the same Fleet cockpit, confirm:
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
16. For the final Fleet cockpit v1 visual review, confirm:
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
17. Confirm no buttons other than the explicit `create transfer`, `cancel transfer`, `complete due`, Planet or Construction `enqueue construction`, and guarded Research `enqueue research` confirmation paths execute mutating dev endpoints from any route.

Strategic map cockpit v1 visual review:

- Confirm the first viewport feels like a playable map cockpit rather than a raw dev readout.
- Confirm the map remains the largest surface and the selection, planet, and transfer context stay adjacent to it.
- Confirm fleet markers and transfer overlays are visually distinct enough to decode without reading raw counts first.
- Confirm focused-system labels, planet labels, and primary actions are Spanish-first.
- Confirm ids remain secondary metadata only.
- Confirm the read-only boundary is always visible and the page does not imply command authority.
- Confirm technical diagnostics remain collapsed or clearly secondary after the cockpit loads.

Planet cockpit v1 visual review:

- Confirm the first viewport reads like a management cockpit rather than a debug payload dump.
- Confirm identity, resources, buildings, queue, and actions appear in that order before diagnostics.
- Confirm primary labels are Spanish-first and raw ids remain secondary.
- Confirm construction creation is the only executable Planet mutation path and still requires explicit confirmation.
- Confirm complete-due remains visibly disabled in this build.

Construction cockpit v1 visual review:

- Confirm the first viewport reads like a focused construction command surface rather than a duplicate debug page.
- Confirm active planet context, reserves, economy, queue, and catalog appear before diagnostics.
- Confirm the seeded `Aurelia` scenario visibly includes both available and blocked construction actions.
- Confirm blocked cards remain visually quieter than available actions.
- Confirm technical details for command failures remain secondary to the primary Spanish guidance.

Research cockpit v1 visual review:

- Confirm the first viewport reads like a specialized management cockpit rather than a placeholder or debug page.
- Confirm context, summary, queue, catalog, actions, and diagnostics appear in that order before deep technical detail.
- Confirm the seeded `Aurelia` scenario visibly includes both available and blocked research cards.
- Confirm blocked cards remain visually quieter than available actions.
- Confirm requirement chips, long technology names, and cost rows wrap cleanly without horizontal overflow.
- Confirm guarded enqueue is the only executable Research mutation path and still requires explicit confirmation.
- Confirm complete-due remains visibly disabled in this build.

## Final Block Checklist

- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
- `ai/tasks/pending` contains only `.gitkeep`.
- `/planet` behaves as a dashboard, not a full construction catalog.
- `/construction` stays scoped to general construction.
- `/research` behaves as a development-safe cockpit foundation.
- `/ground-army`, `/shipyard`, and `/defenses` still open placeholders only.
- `Galaxy` remains read-only.
- `Fleets` still preserves context and read-only command flow.
- No 3D/WebGL renderer is introduced.
- Diagnostics stay collapsed by default on the cockpit routes.

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
