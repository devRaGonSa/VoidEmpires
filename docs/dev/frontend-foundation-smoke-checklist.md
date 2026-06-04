# Frontend Foundation Smoke Checklist

Use this checklist to validate the current frontend prototype without confusing it with production UI.

For the current Fleet estimate -> confirm -> create-transfer -> confirm -> cancel-transfer or complete-due paths, pair this document with `docs/dev/fleet-controlled-mutation-checklist.md`.
For the current Galaxy, Planet, Construction, Research, Ground Army, Shipyard, and Defenses cockpit review, also pair this document with `docs/dev/strategic-map-cockpit-checklist.md`, `docs/dev/planet-cockpit-checklist.md`, `docs/dev/construction-cockpit-checklist.md`, `docs/dev/research-cockpit-checklist.md`, `docs/dev/ground-army-cockpit-checklist.md`, `docs/dev/shipyard-cockpit-checklist.md`, and `docs/dev/defenses-cockpit-checklist.md`.
For the current module boundary model and placeholder responsibilities, also pair this document with `docs/dev/planet-module-boundaries.md`.
For cross-cockpit wording cleanup, use `docs/dev/cross-cockpit-language-audit.md` as the source of truth before changing visible copy.
For shared preferred terms and allowed limitation patterns, use `docs/dev/cockpit-copy-guidelines.md`.
For deterministic local QA setup, use `docs/dev/development-seed-profiles.md` instead of manual SQL.

## Final Cross-Cockpit Visual Pass

Use this block as the one-stop manual QA pass for the accepted shared cockpit suite after the non-visual validation commands succeed.

Shared constraints for this pass:

- Do not use manual SQL.
- Do not introduce 3D or WebGL expectations.
- Do not treat combat as supported.
- Galaxy remains read-only.
- Mutations are allowed only in dedicated cockpits and still require explicit confirmation.

Start by reapplying the richer shared seed twice so reused local databases recover their documented baseline rows before you compare screens:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'

Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'
```

Then open and compare these deterministic routes:

1. Galaxy
   URL: `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Helios Gate` loads with visible `Aurelia`, `Cinder Reach`, and `Aether Crown`, fleet markers and one transfer overlay are visible, and the cockpit stays read-only.
2. Planet
   URL: `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Aurelia` loads with readable resources, grouped buildings, queue state, and handoffs back to Galaxy and toward Fleets.
3. Construction
   URL: `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Aurelia` loads with reserves, local economy, queue context, at least one available construction action, and at least one blocked action.
4. Research
   URL: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Aurelia` loads with summary cards, queue history, `Ingenieria planetaria` available, and at least one blocked comparison card.
5. Shipyard
   URL: `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Aurelia` loads with readiness, stock, queue context, one available orbital option, and blocked comparison options.
6. Ground Army
   URL: `/ground-army?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Aurelia` loads with readiness summary, visible ground structures, one available option, blocked comparisons, truthful queue history, and disabled complete-due messaging.
7. Fleets
   URL: `/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: the command deck, group rail, selected-group detail, active-transfer context, and the read-only estimate flow all render with seeded groups and transfers.
8. Defenses
   URL: `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Minimum non-empty result: `Aurelia` loads with defense readiness, one visible `DefenseGrid`, one deterministic defense option, truthful complete-due limitation messaging, and visible handoffs toward neighboring cockpits.

Cross-cockpit comparison checks:

- Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Fleets, and Defenses all open from the shared `cockpit-validation` baseline without shell-only or near-empty regressions.
- Route helpers preserve `civilizationId` and `planetId` when moving between the accepted cockpit links.
- Galaxy remains read-only while Planet, Construction, Research, Shipyard, and Fleets keep their current guarded mutation boundaries.
- Diagnostics stay collapsed or clearly secondary across the cockpit routes.
- Ground Army is now part of the accepted cockpit-foundation suite, but it remains readiness-only and non-combat.
- Defenses is now part of the accepted cockpit-foundation suite, but it remains readiness-only and non-combat.

## Ground Army Block Closure Pass

Run this exact narrow sequence when closing the Ground Army cockpit block:

1. Reapply `cockpit-validation` twice.
2. Open `/galaxy`.
3. Open `/planet`.
4. Open `/construction`.
5. Open `/research`.
6. Open `/shipyard`.
7. Open `/fleets`.
8. Open `/defenses`.
9. Open `/ground-army`.

Closure expectations:

- Ground Army loads as a cockpit foundation, not a placeholder.
- Ground Army keeps combat, invasion, assault, 3D, and fleet movement out of scope.
- Galaxy stays read-only.
- Planet, Construction, Research, Shipyard, Fleets, and Defenses remain usable from the shared seeded baseline.

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
   - the canonical seeded Galaxy URL `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` opens the real cockpit, not only the shared shell
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
   - the summary shows `Disponibles >= 1` and `Bloqueadas >= 1` before any enqueue
   - the catalog shows Spanish technology names, meaningful categories, and both available and blocked research cards
   - blocked cards keep readable Spanish guidance and their disabled buttons remain visually secondary
   - `Requisito pendiente de clasificar` does not appear in the primary seeded blocker text
   - preparing an available item opens the guarded confirmation panel before `Confirmar`
   - the confirmation panel shows civilizacion, planeta, tecnologia, categoria, coste, duracion, and readiness context
   - a successful enqueue refreshes the queue and catalog from the backend-confirmed read model
   - after exactly one successful enqueue, the queue count increases and the enqueued technology no longer appears ready
   - any generic validation rejection during the guarded enqueue flow is treated as a failed smoke pass
   - `Completar vencidas no disponible` stays visibly disabled when the backend route is not scoped safely to this cabin
   - diagnostics remain collapsed and keep technical details secondary
13. Open the `Shipyard` route with civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`, then confirm:
   - the seeded shipyard URL `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` opens the cockpit with deterministic `Aurelia` context
   - the top cockpit strip shows context, readiness, and stock or queue summaries before diagnostics
   - resources, orbital stock, queue state, and production catalog are visible without exposing raw DTOs in the primary flow
   - the catalog shows meaningful Spanish asset labels and visible available or blocked comparisons
   - preparing an available item opens a guarded review step before `Confirmar produccion`
   - one successful enqueue refreshes queue, stock, and catalog from the backend-confirmed state
   - `Completar produccion vencida no disponible` stays visibly disabled while the backend route remains global
   - the handoff to `Flotas` stays explanatory only and never mutates fleet groups
   - links to `Planeta`, `Construccion`, `Investigacion`, `Flotas`, and `Galaxia` preserve context
   - diagnostics remain collapsed or secondary and the page stays free of 3D, combat, and fleet movement actions
14. Open the `Fleets` route.
15. Enter the same civilization id and confirm the new cockpit layout renders in clearly separated sections:
   - a top command deck summarizes groups, active transfers, resource contexts, and action hints
   - an orbital group rail lists the available groups with readable asset type, current planet, and compact identifiers
   - a selected-group panel shows asset type, quantity, status, current planet, origin planet, readiness, and active transfer context when present
   - a command column groups estimate inputs, latest estimate state, guarded create-transfer confirmation, and guarded cancel-transfer confirmation together
   - prototype-only mutation controls remain separated from the executable command column
16. Within the same Fleet cockpit, confirm:
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
17. For the final Fleet cockpit v1 visual review, confirm:
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
18. Confirm no buttons other than the explicit `create transfer`, `cancel transfer`, `complete due`, Planet or Construction `enqueue construction`, guarded Research `enqueue research`, and guarded Shipyard `confirmar produccion` paths execute mutating dev endpoints from any route.

Neighbor cockpit regression checkpoints:

- Confirm the seeded `Galaxy`, `Planet`, `Construction`, `Research`, `Ground Army`, `Shipyard`, `Fleets`, and `Defenses` routes still load with the same local development context used in this block.
- Confirm the current route helpers still preserve `civilizationId` and `planetId` when moving between `Galaxy`, `Planet`, `Construction`, `Research`, `Ground Army`, `Shipyard`, `Fleets`, and `Defenses`.
- Confirm `Galaxy` remains read-only while `Planet`, `Construction`, `Research`, `Ground Army`, `Shipyard`, and `Fleets` keep their accepted cockpit boundaries.

Ground Army cockpit v1 visual review:

- Confirm the first viewport reads like a readiness cockpit rather than a placeholder or battle screen.
- Confirm context, summary, structures, options, queue, handoffs, and diagnostics appear in that order before deep technical detail.
- Confirm the seeded `Aurelia` scenario visibly includes one available option plus visible blocked comparisons.
- Confirm complete-due remains visibly disabled in this build.
- Confirm links toward `Construccion`, `Defensas`, `Flotas`, `Planeta`, and `Galaxia` preserve context.
- Confirm no card or button implies invasion, combat, 3D, or fleet movement support.

Defenses cockpit v1 visual review:

- Confirm the first viewport reads like a readiness cockpit rather than a placeholder or battle screen.
- Confirm context, readiness summary, structure inventory, option state, queue messaging, handoffs, and diagnostics appear in that order before deep technical detail.
- Confirm the seeded `Aurelia` scenario visibly includes one deployed `DefenseGrid` plus one deterministic defense option.
- Confirm available or blocked state copy is readable in Spanish and does not imply unsupported combat behavior.
- Confirm any mutation remains construction-scoped or explicitly handed off rather than executed as a defense-specific action.
- Confirm complete-due remains visibly disabled in this build.
- Confirm links toward `Construccion`, `Astillero`, `Flotas`, `Planeta`, and `Galaxia` preserve context.

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
- Confirm the summary recommendation never presents a blocked technology as immediately startable.
- Confirm complete-due remains visibly disabled in this build.

Shipyard cockpit v1 visual review:

- Confirm the first viewport reads like a specialized orbital-production cockpit rather than a placeholder or fleet console.
- Confirm context, readiness, queue, stock, catalog, and diagnostics appear in that order before deep technical detail.
- Confirm the seeded `Aurelia` scenario visibly includes at least one available item plus visible blocked or unsupported comparisons.
- Confirm guarded enqueue is the only executable Shipyard mutation path and still requires explicit confirmation.
- Confirm complete-due remains visibly disabled in this build.
- Confirm the Fleet handoff copy stays explicit that Shipyard does not move or command fleets directly.

## Final Block Checklist

- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
- `ai/tasks/pending` contains only `.gitkeep`.
- `/planet` behaves as a dashboard, not a full construction catalog.
- `/construction` stays scoped to general construction.
- `/research` behaves as a development-safe cockpit foundation.
- `/research` keeps the corrected QA path with one seeded available item, visible blocked items, guarded enqueue, and disabled complete-due placeholder.
- `/ground-army` behaves as a readiness cockpit foundation, not a placeholder and not a combat surface.
- `/defenses` behaves as a readiness cockpit foundation, not a placeholder and not a combat surface.
- `/shipyard` behaves as a development-safe cockpit foundation with guarded enqueue, disabled complete-due, and explicit Fleet boundaries.
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
4. The current `minimal-validation` seed is additive and idempotent. It does not delete extra transfers or groups or reset reserved/stationed state, but it does top up the existing `Aurelia` resource stockpile to at least `125` credits, `160` metal, `100` crystal, and `50` gas.
5. When you need the original baseline back exactly, use a fresh disposable local database and then re-apply the seed.
