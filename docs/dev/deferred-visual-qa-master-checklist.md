# Deferred Visual QA Master Checklist

This checklist prepares a later browser pass for the current playable Development loop.
It does not claim that visual QA, screenshots, or manual browser verification have been performed.

Use it with:

- `docs/dev/single-product-demo-guide.md`
- `docs/dev/product-completion-audit.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/product-readiness-report.md`

## Deferred Status

- Status: pending human browser execution.
- Screenshots: pending capture.
- Visual acceptance: not yet claimed.
- Scope: `/`, `/register`, `/login`, `/onboarding` as a registration alias, `/galaxy`, `/planet`, `/construction`, `/research`, `/shipyard`, `/defenses`, `/ground-army`, `/fleets`, `/market`, `/espionage`, `/alliance`, and `/ranking`.
- Non-scope: final production authorization, account recovery/confirmation product UX, real combat, fleet movement from Shipyard or Planet, exploration missions, WebGL/3D acceptance, and hidden auto-completion.

## Product-Facing Copy Checks

Run these checks with a normal browser session: no `?operator=1` query flag and no `localStorage["voidempires.operatorMode"] = "1"` flag.

- Block 43 shell split: confirm `/login`, `/register`, `/registro`, and `/onboarding` render in the public account layout without the game sidebar, game shell intro, or resource/status bar.
- Authenticated game shell: confirm authenticated gameplay routes render the game shell with the persistent left sidebar on desktop and the top resource bar with selected-planet `Creditos`, `Metal`, `Cristal`, and `Gas` amounts plus capacity/production detail when returned by the backend.
- Inicio/Planeta: confirm `/` is the current planet overview after account resolution and `/planet` presents the same overview instead of a competing second dashboard.
- Catalog routes: confirm Construction is a building catalog, Research is a compact technology grid, Shipyard is a compact ship-production grid, Defenses is a defense catalog, and Ground Army is a land-unit catalog/training surface.
- Removed copy terms: confirm normal UI does not show `cabina`, `contexto guardado`, `dar contexto`, `cargar mando`, `siguientes cabinas`, `registrar comandante`, `partida local`, or `nueva partida`.
- Global shell: confirm header, sidebar, route status, secondary shell status, and loading states do not show `Development`, `Dev`, `QA`, `test`, `prueba`, `prototipo`, `endpoint`, `localhost`, backend profiles, raw payload wording, or provider names.
- Home route `/`: confirm the first screen offers account entry, current command continuation, and galaxy entry without presenting a technical route alias or implementation detail.
- Registration/login: confirm account entry reads as the normal product path, `/onboarding` behaves only as a compatibility alias to registration, and neither flow exposes backend URLs or returned raw ids unless operator mode is explicitly enabled.
- Planet and Construction: confirm product mode shows colony state, resources, buildings, queue, and guarded confirmations before any diagnostics or operator controls.
- Research and Shipyard: confirm catalog cards, costs, queues, blocked states, and confirmation modals use product-facing language and do not describe mutation helpers, backend acceptance, or technical complete-due paths as primary gameplay.
- Readiness pages: confirm Galaxy, Defenses, Ground Army, Fleets, Market, Espionage, Alliance, and Ranking keep honest read-only/readiness copy without development/test/prototype wording or primary-looking unsupported actions.
- Error and empty states: confirm recovery guidance is player-facing and never asks the user to inspect endpoints, local URLs, payloads, providers, or database details.

## Operator Visibility Checks

These checks verify default product mode only; do not enable operator mode unless the later pass explicitly includes an operator/internal subsection.

- Confirm `DevEndpointNotice`, `DevelopmentToolsPanel`, `DevDiagnosticsPanel`, and `ActionManifestPanel` are not visible on first render or normal route navigation.
- Confirm materialization controls, action manifests, direct transfer tooling, technical complete-due controls, raw GUID panels, and JSON payload previews are hidden by default.
- Confirm generated product links do not preserve `operator=1` during normal navigation.
- If operator mode is intentionally enabled for a secondary pass, confirm those tools remain visibly secondary, technical, and still require explicit confirmation before any state-changing request.

## Screenshot-Derived Decluttering Checks

These checks come from the user's observed overloaded Planet and Construction screens. This implementation block cleans up the documented issues, but full browser QA remains user-driven until a later manual pass captures and reviews screenshots.

- Global header: confirm it does not show disconnected mock resource bars or static empire values as if they were the selected backend context.
- Sidebar: confirm playable mutation-capable routes, read-only routes, readiness routes, operator-only helpers, and future/disabled work are grouped or labeled distinctly.
- Mutating pages: confirm Construction, Research, Shipyard, guarded Fleet transfer actions, and Development materialization controls do not use obsolete primary `solo lectura` copy.
- Planet hub: confirm primary actions and handoffs are visible before diagnostics, raw ids, endpoint metadata, or deep technical notes.
- Construction catalog: confirm available and blocked construction options appear without excessive first-viewport scrolling, duplicate planet-summary noise, or debug-first panels.
- Operator tools: confirm resource/materialization actions are hidden from product mode, then collapsed, secondary, or visually separated if an operator-only pass explicitly reveals them.
- Operator actions: confirm any revealed action that mutates local validation data opens an explicit review or confirmation modal/flow before the backend request.
- Diagnostics: confirm diagnostics remain collapsed by default or clearly secondary, and never dominate the primary workflow.
- Resource labels: confirm visible resource terms stay coherent as `Creditos`, `Metal`, `Cristal`, `Gas`, with `Energia`, `Deuterio`, and `Poblacion` treated as distinct context terms when shown.
- Readiness pages: confirm Defensas, Flotas, Mercado, Alianzas, Ranking, Espionaje, and Ground Army keep honest read-only/readiness scope without primary-looking unavailable actions.

## Ordered Browser Plan

1. Start the backend:
   - `dotnet run --project .\src\VoidEmpires.Web`
2. In a separate terminal, print the playable loop guide:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-playable-loop-guide.ps1`
   - Confirm the guide only prints the sequence by default and does not enqueue or materialize anything.
3. Start the frontend:
   - `npm run dev --prefix src\VoidEmpires.Frontend`
4. Open `/`.
5. Confirm the home route offers account entry, command continuation, and galaxy entry with no product-surface forbidden terms.
6. Open `/register`.
7. Register a fresh account through the UI and confirm it routes to the generated initial world.
8. Open `/login` and `/onboarding`; verify login presents account entry and onboarding only aliases to registration.
9. Open the returned Planet link.
10. Verify the local playable-session banner or continuation card is available when ids are missing on a cockpit route.
11. Verify the Planet hub first:
   - colony identity, ownership, resources, production, buildings, construction queue, and handoff links are visible
   - diagnostics remain secondary
   - no queue materialization runs on page load
12. Confirm operator-only materialization controls are hidden in the normal product session.
13. Open Construction from the Planet hub.
14. Enqueue one available construction action only through the guarded confirmation flow.
15. Verify the result comes from a backend refresh:
   - queue is updated from the read model
   - resources are reduced by the visible cost
   - accepted-but-not-visible state does not fabricate a queue row
16. Open Research with the same ids.
17. Enqueue one available research item only through the guarded confirmation flow.
18. Verify the refreshed Research state:
   - active queue row is visible when backend returns it
   - blocked research remains non-mutating
   - complete-due remains unavailable from the normal cockpit
19. Open Shipyard with the same ids.
20. Enqueue one available orbital production item only through the guarded confirmation flow.
21. Verify the refreshed Shipyard state:
   - queue and resource deltas come from backend state
   - local orbital stock does not change until due-queue materialization runs
   - Shipyard still does not move or command fleets
22. Materialize due queues explicitly with the printed scoped helper command, after orders are due:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-materialize-due-queues.ps1 -CivilizationId {created-civilization-id} -PlanetId {created-planet-id} -ElapsedSeconds 3600`
23. Re-open or refresh Planet, Construction, Research, and Shipyard.
24. Verify completed construction, research progress, and orbital stock are visible only after backend-confirmed materialization and follow-up reads.
25. Run read-only diagnostics:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-get-playable-session-diagnostics.ps1 -CivilizationId {created-civilization-id} -PlanetId {created-planet-id}`
26. Open Defenses with the same ids.
27. Verify Defenses stays read-only or construction-handoff scoped:
   - no defense-specific combat, interception, or instant completion action appears executable
   - blocked and unavailable options remain visually secondary
28. Open Fleets with the same ids.
29. Verify Fleets read-state remains honest:
   - visible groups, transfers, estimates, and resource contexts are readable
   - no movement, cancellation, complete-due, split, merge, or stock promotion runs without explicit guarded Fleet actions
30. Open Galaxy with the same civilization and any known planet/system context.
31. Verify Galaxy remains read-only and map-first, with selected-system and planet detail adjacent to the map.
32. Open Ground Army, Market, Espionage, Alliance, and Ranking.
33. Verify those routes keep their accepted boundaries:
   - Ground Army is readiness only and does not execute invasion or combat
   - Market is advisory and does not buy, sell, auction, trade, or execute routes
   - Espionage is intelligence-readiness only and does not run missions
   - Alliance is diplomacy-readiness only and does not create alliances, pacts, invitations, roles, treasury changes, or messages
   - Ranking is a power-index read and does not claim public ladder, reward, matchmaking, or profile readiness
34. Return to Planet and verify the hub still preserves the same `civilizationId` and `planetId`.
35. Confirm no route turns combat, movement, exploration missions, or materialization into normal always-on gameplay copy.

## Screenshot Naming Convention

Use stable names so captures can be compared across passes without relying on local machine paths:

- `01-registration-entry.png`
- `02-registration-success.png`
- `03-planet-hub-initial.png`
- continue with two-digit route order plus short state, for example `08-research-post-enqueue.png`
- use `desktop` or `mobile` suffix only when the same state is captured at multiple viewport sizes

Do not commit screenshots in this task. Capture files belong to the later human/browser QA artifact location selected for that pass.

## Screenshot Capture List

Capture these screenshots during the later browser pass. Each capture should show the primary workflow first and keep diagnostics collapsed or visibly secondary unless the row says otherwise.

- `/register` before submit.
- `/register` success state or generated-world navigation after account creation.
- `/login` account entry state.
- `/` home route with account entry, command continuation, and galaxy entry.
- `/galaxy` map-first read-only view with selected system and planet context.
- `/planet` initial loaded hub for the created playable session.
- `/planet` local-session continuation state with ids absent.
- `/planet` resource materialization feedback, including a no-op case if no due work changed.
- `/construction` available action review before confirmation.
- `/construction` post-enqueue refreshed queue and resource state.
- `/research` available research review before confirmation.
- `/research` post-enqueue queue and blocked-card comparison.
- `/shipyard` available production review before confirmation.
- `/shipyard` post-enqueue queue, resource delta, and unchanged stock state.
- `/planet` after due-queue materialization.
- `/research` after due-queue materialization.
- `/shipyard` after due-queue materialization with updated orbital stock when produced.
- `/defenses` read-only readiness and handoff state.
- `/ground-army` readiness state with available or blocked training comparison.
- `/fleets` read-state with same planet context.
- `/fleets` travel estimate result, if the dedicated Fleet checklist is included in the pass.
- `/fleets` guarded transfer confirmation and refreshed state, only if the dedicated Fleet controlled-mutation pass is intentionally executed.
- `/market` advisory economy state with disabled transaction actions.
- `/espionage` intelligence coverage and disabled mission actions.
- `/alliance` diplomacy identity/contact state with disabled diplomacy actions.
- `/ranking` power-index category and comparison state.
- A diagnostics panel screenshot showing raw details secondary, not dominant.

## Failure Conditions

Treat the later pass as failed if any of these appear:

- a cockpit opens to a blank shell or never leaves a generic loading state
- a route drops the active `civilizationId` or `planetId` unexpectedly
- backend errors are hidden or replaced with fake success
- a page fabricates resources, buildings, research, queues, stock, or fleet groups before backend confirmation
- materialization runs from page load, route entry, sidebar navigation, or ordinary card selection
- Defenses presents real combat/interception execution
- Ground Army presents invasion or combat execution
- Market presents buying, selling, auctions, player trading, or route execution
- Espionage presents mission, sabotage, infiltration, or counter-espionage execution
- Alliance presents creation, invitation, pact, role, treasury, or messaging execution
- Ranking presents public ladder, reward, matchmaking, or profile readiness as implemented
- Shipyard or Planet presents fleet movement as executable
- Fleets presents movement, cancel, complete-due, split, merge, or stock promotion without explicit guarded user action
- diagnostics dominate the primary viewport instead of staying secondary
- product mode shows `Development`, `Dev`, `QA`, `test`, `prueba`, `prototipo`, endpoint URLs, localhost details, backend profile names, provider names, raw payload wording, or database details in primary UI
- operator-only panels or controls appear without an explicit operator reveal

## Validation Before Running The Future Browser Pass

Run these from the repository root before a human starts the deferred browser pass:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

## Post-DB And Asset Correction Phase Prep

After final database/model consolidation and final asset replacement land, the correction phase should use this checklist as the acceptance baseline instead of treating screenshots as an open-ended redesign pass.

Recommended entry criteria:

1. Final DB/model changes have passed their build, test, seed-drift, and migration validation gates.
2. Final asset or manifest changes have passed frontend build, lazy-route guard, copy guard, and asset-key validation.
3. The browser pass starts from a known seed/profile or production-equivalent setup path documented for that phase.
4. Any route that depends on production authorization hardening, combat, movement productization, market transactions, or alliance mutations has an explicit accepted task before it is tested as executable.
5. Screenshots are captured with the naming convention above and are reviewed route by route.

Recommended correction loop:

1. Capture the full desktop route set first, then mobile or constrained-width variants for the same states.
2. Classify each finding as backend-state mismatch, frontend copy issue, layout/overflow issue, asset mapping issue, route/context loss, accessibility issue, or unsupported-gameplay overclaim.
3. Fix only one narrow class of issue per follow-up task when changes would cross route or module boundaries.
4. Re-run the validation commands above after each correction batch.
5. Re-capture only the affected screenshots plus any neighboring route that shares the changed component.
6. Keep diagnostics secondary; do not accept a correction that hides backend-owned state, errors, or limitations behind imagery.

Correction acceptance rules:

- A corrected route must still preserve `civilizationId` and `planetId` handoffs when those ids are part of the accepted flow.
- Resource, queue, stock, ranking, and readiness displays must still come from backend reads.
- Final images may improve recognition, but must not imply ownership, availability, combat power, market price, or alliance membership that the backend does not return.
- Production authorization wording is accepted only after active civilization resolution and ownership enforcement are implemented.
- Combat, fleet movement productization, market transactions, and alliance mutations remain non-accepted unless their separate executable workflows exist.

No browser, screenshot, DB migration, final asset generation, or correction pass was performed for this prep update.
