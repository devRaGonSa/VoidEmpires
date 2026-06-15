# Frontend Foundation Smoke Checklist

Use this checklist to validate the current frontend prototype without confusing it with production UI.

For the current Fleet estimate -> confirm -> create-transfer -> confirm -> cancel-transfer or complete-due paths, pair this document with `docs/dev/fleet-controlled-mutation-checklist.md`.
For the current Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Market, Defenses, Espionage, Alliance, and Ranking cockpit review, also pair this document with `docs/dev/strategic-map-cockpit-checklist.md`, `docs/dev/planet-cockpit-checklist.md`, `docs/dev/construction-cockpit-checklist.md`, `docs/dev/research-cockpit-checklist.md`, `docs/dev/ground-army-cockpit-checklist.md`, `docs/dev/shipyard-cockpit-checklist.md`, `docs/dev/market-cockpit-checklist.md`, `docs/dev/defenses-cockpit-checklist.md`, `docs/dev/espionage-cockpit-checklist.md`, `docs/dev/alliance-cockpit-checklist.md`, and `docs/dev/ranking-cockpit-checklist.md`.
Use the dedicated espionage checklist for the route-specific pass on grouped targets, passive signals, disabled future missions, and cross-cockpit handoffs.
For the current module boundary model and placeholder responsibilities, also pair this document with `docs/dev/planet-module-boundaries.md`.
For cross-cockpit wording cleanup, use `docs/dev/cross-cockpit-language-audit.md` as the source of truth before changing visible copy.
For shared preferred terms and allowed limitation patterns, use `docs/dev/cockpit-copy-guidelines.md`.
For deterministic local QA setup, use `docs/dev/development-seed-profiles.md` instead of manual SQL.
For lightweight Espionage copy regression protection before visual QA, run `.\scripts\check-espionage-copy.ps1`.

## Gameplay Modal Guardrails

Use this shared interaction contract for the current gameplay confirmation pattern in `Construction`, `Research`, and `Shipyard`:

- opening the modal is a review step only and must not mutate backend state
- selecting a candidate is a local UI-state change only and must not mutate backend state
- the explicit modal primary action is the only allowed mutation trigger
- the acknowledgement checkbox remains required before the primary action becomes available
- success stays backend-first:
  - the cockpit posts the real Development mutation
  - then refreshes from the authoritative backend read model
  - then shows success or accepted-but-not-yet-visible guidance from the refreshed result
- failure stays honest and useful:
  - Spanish-first primary feedback remains in the main flow
  - raw payloads stay out of the first viewport
  - useful local review state may remain available when it helps retry safely

Non-goals for this pattern:

- no auto-submit on card click
- no auto-complete or auto-process follow-up action
- no optimistic queue fabrication before backend refresh
- no expansion into fleet movement, combat, or other adjacent mutations

## Orbital Preparation Runtime Order

Use this exact manual sequence for the current orbital production or military preparation pass. This remains a user-driven visual QA checklist, not proof that the browser checks have already been performed.

1. Start the backend:
   - `dotnet run --project .\src\VoidEmpires.Web`
2. Reapply the shared deterministic seed twice:
   - `Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'`
   - `Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'`
3. If the reused Development database already has an open orbital production order, clear only the scoped Shipyard blockers first:
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-orbital-production-ui-state.ps1`
4. Start the frontend:
   - `npm run dev --prefix src/VoidEmpires.Frontend`
5. Open `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` and run the guarded enqueue check if the catalog still shows one available orbital option.
6. Open `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` and confirm the same planet context remains read-only plus handoff-only.
7. Open `/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` and confirm the read-state reflects the same colony context without auto-promoting Shipyard stock to a fleet group.
8. Return to `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` and confirm the orbital or military summary still reads as a handoff surface rather than a new mutation cockpit.

Reused-database warning:

- If Shipyard reports an existing open orbital production order, treat it as a real persisted state, not as a hidden reset path.
- Run the preparation helper explicitly before repeating the Shipyard success path.
- The visual checks above remain pending until a human actually performs them in the browser.

## Frontend Copy Regression Guard

Run this guard before closing manual visual batches for the accepted cockpit suite:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- This scans only `src/VoidEmpires.Frontend/src` for known recurring blocked phrases, so docs and scripts are intentionally excluded.
- Current blocked phrases:
  - `Recurso no clasificado`
  - `La cabina de la Alianza permanece en modo solo lectura en esta fase`
  - `Void Seed Civilization | current`
  - `Delta`
  - `solo lectura en esta fase`

## Research Enqueue Lightweight Guard

There is no dedicated frontend test runner in the current repository. Until one exists, protect the Research enqueue flow with these lightweight static checks:

- `npm run build --prefix src/VoidEmpires.Frontend`
  - This is the compile-time guard that confirms `enqueueResearchOrder`, its imported types, and the current `ResearchPage.tsx` flow still type-check together.
- Inspect `src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx` and confirm the guarded confirmation copy still exists:
  - `Confirmar inicio de investigacion`
  - `Confirmo que quiero iniciar esta investigacion`
  - `La investigacion no se enviara hasta que confirmes la orden en el paso final.`
- Confirm the component still keeps mutation behind the prepared-plus-confirmed path:
  - `handleResearchSubmit` remains the only place that calls `enqueueResearchOrder(...)`
  - the confirm button stays disabled until `hasEnqueueAcknowledgement` is true
  - blocked research cards remain read-only and do not call `handleResearchPreparation` through a primary action affordance
- Treat any of the following as a regression even if the build passes:
  - direct enqueue from a catalog card without the confirmation panel
  - confirmation copy removed or renamed without replacing the explicit guard language
  - blocked research rendered as an immediately actionable primary button

Manual QA fallback:

- Open `/research` with the deterministic seeded ids from `docs/dev/research-cockpit-checklist.md`.
- Prepare one available research item and confirm the checkbox is still required before submit.
- Verify blocked cards remain non-mutating and visually secondary during the same pass.

## Frontend Bundle Baseline

Use this baseline before changing route loading or Vite chunking:

- `npm run build --prefix src/VoidEmpires.Frontend` currently succeeds with one JS entry chunk at `551.88 kB` minified (`136.02 kB` gzip) and one CSS asset at `45.44 kB` (`7.20 kB` gzip).
- Vite still emits the standard `Some chunks are larger than 500 kB after minification` warning during the production build.
- The current route layer is eager: `src/App.tsx` imports `StrategicMapPage`, `PlanetPage`, `ConstructionPage`, `ResearchPage`, `ShipyardPage`, `FleetsPage`, `MarketPage`, `DefensesPage`, `GroundArmyPage`, `EspionagePage`, and `ModuleCabinPage` synchronously before routing.
- The most obvious lazy-loading candidates are the large route modules now bundled into first load: `StrategicMapPage`, `FleetsPage`, `ShipyardPage`, `PlanetPage`, `MarketPage`, `DefensesPage`, `EspionagePage`, `ResearchPage`, and `GroundArmyPage`.
- The shared synchronous shell should stay limited to `main.tsx`, `App.tsx`, `styles.css`, `AppShell`, route URL helpers, configuration, and the lightweight route metadata from `planetPresentation.ts`.
- `ConstructionPage` is currently a thin wrapper over `PlanetPage`, so any route split that keeps `ConstructionPage` separate still needs to avoid duplicating the shared Planet implementation unnecessarily.
- No chunking decision should change route paths, seeded QA URLs, shared shell navigation, or the current Spanish-first cockpit behavior.

## Route Loading Verification

Use these checks after route-level lazy loading lands:

- `npm run build --prefix src/VoidEmpires.Frontend` should emit route chunks for the accepted cockpits instead of one oversized application chunk.
- The current expected production result is `87` transformed modules, one `179.32 kB` shared entry chunk, one `45.97 kB` CSS asset, and separate async cockpit chunks.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` should print `Frontend route lazy-import guard passed.`
- After the current lazy-loading pass, the build should stay on Vite defaults without the old `500 kB` warning; reintroducing that warning is a regression for this block.
- Treat this block as a loading-architecture change only; accepted gameplay behavior, backend calls, and route URLs must remain unchanged.
- `/` and `/galaxy` must still resolve to the accepted Galaxy cockpit, not a blank shell or a broken redirect.
- `/planet`, `/construction`, `/research`, `/shipyard`, `/fleets`, `/defenses`, `/ground-army`, `/espionage`, `/alliance`, `/market`, and `/ranking` must still load with their current query-parameter behavior unchanged.
- The shared shell must remain visible while a lazy route resolves; only the page content area should swap to the loading state.
- The route-loading fallback must stay Spanish-first and read as a loading state: `Carga en progreso`, `Cambio de cabina`, and `Cargando cabina...`.
- Missing-context, empty-state, and disabled-action copy inside each cockpit must still appear after the lazy import resolves; route splitting must not replace those states with a generic loader forever.
- `Alianza` and `Ranking` must remain lazy-loaded implemented routes with explicit read-only framing rather than turning into broken links or future placeholders.

Compact route-loading smoke pass:

- Open `/galaxy`, `/planet`, `/construction`, `/research`, `/shipyard`, `/fleets`, `/defenses`, `/ground-army`, `/espionage`, `/alliance`, `/market`, and `/ranking` from the shared shell.
- If a route resolves slowly, expect the Spanish loading state rather than a blank page: `Carga en progreso`, `Cambio de cabina`, `Cargando cabina...`.
- After the loader clears, expect the target cockpit to render its usual first viewport and keep diagnostics secondary where that cockpit already uses collapsible diagnostics.
- Treat a blank page, a shell-only screen that never resolves, or a generic loader that never yields cockpit content as a failed smoke pass.
- `Alianza` and `Ranking` are now implemented read-only routes; verify the sidebar keeps both navigable and marked as `Solo lectura`.

## Playable Session Navigation Contract

Current query-parameter dependencies:

- `App.tsx` reads `civilizationId`, `planetId`, and `systemId` from the active URL and passes them through sidebar links with the shared route helpers.
- `/planet` reads `civilizationId` and optional `planetId`; if `planetId` is missing and the backend selects an owned planet, the page replaces the URL with the backend-selected `planetId`.
- `/construction` is `PlanetPage` in the `construction` variant, so it has the same `civilizationId` and optional `planetId` behavior as `/planet`.
- `/research`, `/shipyard`, and `/defenses` read `civilizationId` and optional `planetId`; each route can replace the URL with the backend-selected planet when the read model returns one.
- `/fleets` reads `civilizationId` for the Fleet UI-state request and treats `planetId` as focus context for resource, group, and handoff panels.
- Existing helper usage is centralized in `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`: `buildPlanetUrl`, `buildConstructionUrl`, `buildResearchUrl`, `buildShipyardUrl`, `buildDefensesUrl`, `buildFleetsUrl`, `buildGalaxyUrl`, and related cockpit helpers trim empty values and omit missing query params instead of fabricating ids. `buildDevelopmentHelperUrl()` is the explicit deterministic seeded shortcut.

Current missing-id behavior:

- Missing `civilizationId` prevents backend reads on `/planet`, `/construction`, `/research`, `/shipyard`, `/defenses`, and `/fleets`; the route stays on a local entry form or guidance state instead of assuming a logged-in civilization.
- Missing `planetId` is accepted on planet-bound reads when the backend can choose a selected owned planet for the supplied civilization; the route then writes that selected planet back into the URL.
- Invalid, foreign, suspicious, or unavailable ids surface route-specific Spanish error or warning copy and keep diagnostics secondary; they must not silently switch to production auth or an unrelated local session.
- Handoff links should preserve the active ids when present, and when ids are absent they should remain plain routes or explicit seeded helper links rather than creating hidden authority.

`/onboarding` success response and redirect:

- The page posts `displayName`, `civilizationName`, and optional `homePlanetName` to `POST /api/dev/players/starting-civilization`.
- A successful response is expected to include `succeeded = true`, `userId`, `playerProfileId`, `civilizationId`, `homePlanetId`, `homePlanetName`, `homeSystemId`, `homeSystemName`, `startingResources`, `limitations`, and `errors`.
- `/onboarding` requires both `civilizationId` and `homePlanetId` before navigation, then redirects to `buildPlanetUrl(civilizationId, homePlanetId)`.
- This is Development-only navigation context. It is not a login, production session, token, cookie, role claim, or authenticated active-civilization resolver.

Safe local navigation persistence for later tasks:

- `localStorage` may store only non-sensitive navigation context returned by the Development-only playable start: `civilizationId`, `planetId` or `homePlanetId`, player/display name, civilization name, planet name, and client timestamps when useful.
- Stored context is only a URL rebuilding convenience. The backend remains authoritative for ownership, resources, queues, fleets, and all gameplay state.
- Do not store or imply production auth data, bearer tokens, cookies, email-confirmation state, login state, roles, or session claims.
- Missing or stale local context must fall back to explicit route entry or `/onboarding`; it must not create hidden gameplay authority.

Compact navigation regression pass:

- `Onboarding -> Planet`: submit `/onboarding` through the Development-only playable start flow and confirm the redirect lands on `/planet` with the backend-returned `civilizationId` and `homePlanetId` as `planetId`.
- `Sidebar context handoff`: while any seeded or newly created cockpit URL includes `civilizationId` and `planetId`, use the shared shell links for `Planeta`, `Construccion`, `Investigacion`, `Astillero`, `Defensas`, `Flotas`, `Mercado`, and `Galaxia`; confirm each supported destination keeps the active ids instead of falling back to a bare route.
- `Galaxy -> Planet`: open `/galaxy`, follow the seeded planet handoff, and confirm `civilizationId` and `planetId` remain present.
- `Planet -> Construction`: open the `Construccion` handoff from `Planeta` and confirm the selected colony context is preserved.
- `Construction -> Planet`: return to `Planeta` and confirm the same colony context still resolves.
- `Construction -> Research/Shipyard/Defenses/Fleets/Galaxy`: confirm each handoff preserves the active `civilizationId`, keeps the same `planetId` when the destination supports it, and does not expose new mutation controls outside the Construction cockpit.
- `Planet -> Research`: open `Investigacion` from `Planeta` and confirm the selected colony context is preserved.
- `Research -> Planet/Construction/Galaxy`: from `/research`, verify `Volver a Planeta`, `Abrir Construccion`, and `Volver a Galaxia` keep the active `civilizationId` and preserve the same `planetId` whenever the destination supports it.
- `Research -> neighboring mutable/read-only cluster`: while the current Research cockpit does not link directly to `Mercado`, `Espionaje`, or `Ranking`, confirm the shared `buildUrl(...)` helpers still trim and preserve query params consistently so handoffs into those cockpits keep the same visible civilization context instead of inventing a new route state.
- `Research mutation boundary`: confirm `handleResearchSubmit` remains the only Research mutation path, no navigation chip triggers a dev write, and no neighboring cockpit gains a new mutation affordance just because it was opened after a Research enqueue attempt.
- `Planet -> Shipyard`: open `Astillero` from `Planeta` and confirm the selected colony context is preserved.
- `Shipyard -> Fleets`: open `Flotas` from `Astillero` and confirm the planet context still appears in the destination route.
- `Planet -> Shipyard -> Fleets -> Planet`: confirm the same `civilizationId` and `planetId` survive the full handoff loop and that no intermediate cockpit drops to a seedless shell.
- `Planet -> Defenses -> Construction/Shipyard/Fleets/Galaxy`: confirm each handoff preserves the active `civilizationId`, keeps the same `planetId` when the destination supports it, and never swaps in a hard-coded route without query params.
- `Defenses mutation boundary`: confirm the cockpit never exposes a local enqueue, complete-due, combat, interception, movement, or mission action; the only allowed next steps are explicit handoffs.
- `Shipyard mutation boundary`: confirm the cockpit never exposes combat, transfer creation, split, merge, mission creation, or safe complete-due; guarded orbital enqueue remains the only local write path.
- `Fleets mutation boundary`: confirm only the explicit Fleet transfer controls can mutate state and that no Planet, Shipyard, or Defenses handoff implicitly sends a transfer, creates a mission, or auto-promotes stock to a fleet group.
- `Market -> Planet/Fleets/Galaxy`: verify those three handoffs preserve the current `civilizationId` and `planetId`.
- `Espionage -> Galaxy/Planet/Fleets`: verify those handoffs preserve the current `civilizationId` and reuse available `systemId` or `planetId` context where expected.
- `Alliance -> Galaxy/Market/Espionage`: verify those handoffs preserve the current `civilizationId` and reuse homeworld context where the destination route supports it.
- `Ranking -> Galaxy/Market/Espionage/Alliance`: verify those handoffs preserve the current `civilizationId` and reuse homeworld context where the destination route supports it.
- Sidebar active state must continue to track the current cockpit route, with `Galaxia` still highlighted for both `/` and `/galaxy`.

## Final Cross-Cockpit Visual Pass

Use this block as the one-stop manual QA pass for the accepted shared cockpit suite after the non-visual validation commands succeed.

Shared constraints for this pass:

- Do not use manual SQL.
- Do not introduce 3D or WebGL expectations.
- Do not treat combat as supported.
- Galaxy remains read-only.
- Mutations are allowed only in dedicated cockpits and still require explicit confirmation.
- Primary UI should read in gameplay-oriented Spanish and avoid raw technical copy in the first viewport.
- Diagnostics should remain collapsed by default or clearly secondary when a cockpit needs technical detail.

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
   Expected primary state: `Helios Gate` loads with visible `Aurelia`, `Cinder Reach`, and `Aether Crown`, fleet markers, one transfer overlay, and a read-only command boundary.
   No-go: no gameplay mutation buttons, no raw payload or endpoint language dominating the first viewport.
   Screenshot target: map stage plus focused system and planet context in the same frame.
2. Planet
   URL: `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with readable resources, grouped buildings, queue state, and clear handoffs toward Galaxy and Fleets.
   No-go: no diagnostics expansion by default, no module cards implying broken or missing implemented cockpits.
   Screenshot target: dashboard header, resources, and grouped module or queue context.
3. Construction
   URL: `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with reserves, local economy, queue context, at least one available construction action, and at least one blocked action.
   No-go: blocked actions must not visually overpower available ones, and no complete-due path should look executable.
   Screenshot target: available-versus-blocked action comparison with queue summary nearby.
4. Research
   URL: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with summary cards, queue history, `Ingenieria planetaria` available, and at least one blocked comparison card.
   No-go: no `pendiente de clasificar` fallback text in seeded primary content and no raw technical wording leading the catalog.
   Screenshot target: summary plus one available and one blocked research card.
5. Shipyard
   URL: `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with readiness, stock, queue context, one available orbital option, and blocked comparison options.
   No-go: no fleet-command implication from Shipyard, no complete-due action looking active, and no diagnostics leading the page.
   Screenshot target: readiness summary plus stock or queue plus one available orbital card.
6. Market
   URL: `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with economy summary, reserves, production, advisory ratios, visible trade signals, and disabled future Market actions.
   No-go: no active buy or sell implication, no diagnostics leading the page, no full disabled-action grid overpowering the economy summary, and no raw backend wording dominating the first viewport.
   Screenshot target: summary plus advisory ratios or trade signals in the same frame.
   User-driven note: this route still requires manual browser confirmation; passing build or test commands does not replace the final visual QA pass.
7. Fleets
   URL: `/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: the command deck, group rail, selected-group detail, active-transfer context, and the read-only estimate flow all render with seeded groups and transfers.
   No-go: no prototype-only control should look executable and no raw ids should dominate selectors or headings.
   Screenshot target: group rail, selected squad panel, and action column in one frame.
8. Defenses
   URL: `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with defense readiness, one visible `DefenseGrid`, one deterministic defense option, truthful complete-due limitation messaging, and visible handoffs toward neighboring cockpits.
   No-go: no combat tone, no defense-specific mutation pretending to exist locally, and no diagnostics opened by default.
   Screenshot target: readiness summary plus deployed `DefenseGrid` and the visible defense option.
9. Ground Army
   URL: `/ground-army?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Aurelia` loads with readiness summary, visible ground structures, one available option, blocked comparisons, truthful queue history, and disabled complete-due messaging.
   No-go: no invasion or combat implication, no direct action pretending to be available, and no expanded technical copy ahead of the main cards.
   Screenshot target: readiness summary plus one available and one blocked ground option.
10. Espionage
   URL: `/espionage?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
   Expected primary state: `Helios Gate` loads with owned `Aurelia`, visible comparison targets, passive signal cards, grouped intelligence, and disabled future mission placeholders.
   No-go: no mission execution cues, no fake real-time surveillance language, no visible English technical strings outside collapsed diagnostics, and no raw backend diagnostics in the first viewport.
   Screenshot target: grouped target catalog plus passive signals or future-missions strip in the same frame.
11. Alliance
   URL: `/alliance?civilizationId=00000000-0000-0000-0000-000000000001`
   Expected primary state: `Void Seed Civilization` loads with visible diplomatic identity, `Sin alianza activa`, one deterministic contact row, disabled future pact and action placeholders, and visible handoffs toward neighboring read-only cockpits.
   No-go: no executable alliance actions, no membership-authority implication, no raw ids leading the page, and no expanded diagnostics in the first viewport.
   Screenshot target: diplomatic summary plus known contact and disabled future-action strip in the same frame.
12. Ranking
   URL: `/ranking?civilizationId=00000000-0000-0000-0000-000000000001`
   Expected primary state: `Void Seed Civilization` loads with visible ranking context, non-zero power summary, category cards, demo comparison rows, disabled future placeholders, and visible handoffs toward neighboring read-only cockpits.
   No-go: no public leaderboard implication, no live opponent framing, no rewards or matchmaking language presented as implemented, and no expanded diagnostics in the first viewport.
   Screenshot target: summary cards plus demo comparison and future placeholder context in the same frame.

Cross-cockpit comparison checks:

- Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Market, Fleets, Defenses, Espionage, Alliance, and Ranking all open from the shared `cockpit-validation` baseline without shell-only or near-empty regressions.
- Route helpers preserve `civilizationId` and `planetId` when moving between the accepted cockpit links.
- Galaxy remains read-only while Planet, Construction, Research, Shipyard, and Fleets keep their current guarded mutation boundaries.
- Research keeps its guarded real-enqueue path without spreading new mutation authority into Planet, Galaxy, Market, Espionage, Ranking, or any navigation-only cockpit handoff.
- Construction remains the only cockpit in this handoff cluster that can enqueue construction orders; Research, Shipyard, Defenses, Planet, Galaxy, and Fleets must not gain a new construction mutation surface through navigation regressions.
- Diagnostics stay collapsed or clearly secondary across the cockpit routes.
- Espionage visible copy stays Spanish-first; English technical wording outside collapsed diagnostics fails visual QA.
- Ground Army is now part of the accepted cockpit-foundation suite, but it remains readiness-only and non-combat.
- Defenses is now part of the accepted cockpit-foundation suite, but it remains readiness-only and non-combat.

Market-specific visual audit note:

- Use `docs/dev/market-cockpit-checklist.md` as the source of truth for the current Market QA target list before changing copy or hierarchy.
- Treat a dev-loader-first first viewport, duplicated diagnostics, or action-like future Market wording as a failed visual pass even if the route remains technically read-only.
- Treat route placeholders that overshadow the economy summary or resemble executable logistics controls as a failed Market visual pass.
- Confirm the shared sidebar marks `Mercado` as an implemented cockpit when `/market` is active, `Alianza` as a navigable `Solo lectura` cockpit when `/alliance` is active, and `Ranking` as a navigable `Solo lectura` cockpit when `/ranking` is active.
- Keep the final Market verdict honest: polished read-only baseline implemented, screenshot-backed visual acceptance still user-driven.

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

## Final Demo-Readiness Regression Note

Use this narrow final pass before declaring the polish block ready for user visual QA:

1. Reapply `cockpit-validation` twice.
2. Open `Galaxy`, `Planet`, `Construction`, `Research`, `Shipyard`, `Fleets`, `Defenses`, `Ground Army`, `Espionage`, `Alliance`, `Market`, and `Ranking` in that order.
3. Confirm each cockpit renders its expected primary state without blank shells, broken handoffs, or horizontal overflow.
4. Confirm primary UI copy stays gameplay-facing and no obvious raw technical wording dominates the first viewport.
5. Confirm diagnostics stay collapsed by default or clearly secondary on every accepted cockpit route.

Treat any of the following as a regression for this block:

- an accepted cockpit opening to a near-empty shell when the shared seed is present
- an implemented cockpit still labeled as future or placeholder navigation
- a blocked or unavailable action styled like a primary executable action
- raw technical ids, payload wording, or endpoint wording dominating the main visible surface
- diagnostics opening by default in the main accepted flow

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

Construction enqueue visual checklist (manual, pending execution):

- [ ] Selecting an available action clearly marks it as the active review target without sending any request yet.
- [ ] The guarded confirmation section appears only for an available prepared action.
- [ ] The confirmation section shows planet, building, action, level, cost, and duration together.
- [ ] The confirmation checkbox remains required before the send button becomes actionable.
- [ ] The primary loading state during submit reads as an in-progress order send, not as a generic page reload.
- [ ] A successful submit shows backend-confirmed success copy in the main flow without surfacing raw payload text.
- [ ] Blocked cards keep readable Spanish reason cues such as missing resources, pending requirement, queue unavailable, or out-of-scope guidance.
- [ ] Blocked cards and disabled buttons remain visually secondary to available actions.
- [ ] Queue and resource changes after submit appear only after the backend refresh, not as fabricated optimistic state.
- [ ] If the backend accepts the order but the refreshed queue still lags, the UI shows the accepted-but-not-yet-visible message instead of inventing a queue row.
- [ ] No complete-due or auto-completion affordance appears executable from the construction cockpit.
- [ ] Primary UI surfaces never show raw JSON, raw payload fragments, or backend field names ahead of the Spanish gameplay copy.

Research cockpit v1 visual review:

- Confirm the first viewport reads like a specialized management cockpit rather than a placeholder or debug page.
- Confirm context, summary, queue, catalog, actions, and diagnostics appear in that order before deep technical detail.
- Confirm the seeded `Aurelia` scenario visibly includes both available and blocked research cards.
- Confirm blocked cards remain visually quieter than available actions.
- Confirm requirement chips, long technology names, and cost rows wrap cleanly without horizontal overflow.
- Confirm guarded enqueue is the only executable Research mutation path and still requires explicit confirmation.
- Confirm the Research navigation chips remain handoff-only controls: they preserve `civilizationId` and `planetId`, do not trigger a route-level eager import regression, and do not introduce Market, Espionage, or Ranking mutation affordances by way of neighboring cockpit context.
- Confirm the summary recommendation never presents a blocked technology as immediately startable.
- Confirm complete-due remains visibly disabled in this build.

Research enqueue visual checklist (manual, pending execution):

- [ ] Selecting an available research card clearly marks it as the active review target without sending any request yet.
- [ ] The guarded confirmation panel appears only after selecting an available research item.
- [ ] The confirmation panel shows planet, technology, category, target level, cost, duration, and readiness context together.
- [ ] The confirmation checkbox remains required before `Confirmar investigacion` becomes actionable.
- [ ] The primary loading state during submit reads as `Confirmando...`, not as a generic page reload or route transition.
- [ ] A successful submit shows backend-confirmed success copy in the main flow without surfacing raw payload text in the first viewport.
- [ ] Blocked technologies keep readable Spanish guidance and remain visually secondary to the available review target.
- [ ] Queue, resources, and visible progress update only after the backend refresh, not as fabricated optimistic local state.
- [ ] If the backend accepts the order but the refreshed queue still lags, the UI shows the accepted-but-not-yet-visible message instead of inventing a queue row.
- [ ] `Completar vencidas no disponible` stays visibly disabled and never turns into an executable auto-complete control.
- [ ] Primary UI surfaces never show raw JSON, raw payload fragments, or backend field names ahead of the Spanish gameplay copy.

Shipyard cockpit v1 visual review:

- Confirm the first viewport reads like a specialized orbital-production cockpit rather than a placeholder or fleet console.
- Confirm context, readiness, queue, stock, catalog, and diagnostics appear in that order before deep technical detail.
- Confirm the seeded `Aurelia` scenario visibly includes at least one available item plus visible blocked or unsupported comparisons.
- Confirm guarded enqueue is the only executable Shipyard mutation path and still requires explicit confirmation.
- Confirm complete-due remains visibly disabled in this build.
- Confirm the Fleet handoff copy stays explicit that Shipyard does not move or command fleets directly.

Shared gameplay modal visual checklist (manual, pending execution):

- [ ] `Construction`, `Research`, and `Shipyard` open the shared modal only after a local review action, never on initial page load.
- [ ] Opening the modal by itself does not create a backend row or mutate visible queue or resource state.
- [ ] Each modal keeps its acknowledgement checkbox requirement before the primary confirm action becomes actionable.
- [ ] Closing the modal does not mutate backend state and preserves the surrounding cockpit context.
- [ ] A successful confirm path refreshes visible state from the backend instead of fabricating optimistic queue rows.
- [ ] A failed confirm path keeps Spanish-first guidance in the main cockpit flow while technical detail stays secondary.

## Final Block Checklist

- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1` passes.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1` passes.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1` (explicit gate) passes.
- `dotnet build --no-restore` may emit the known transient `MSB3026` copy-retry warnings while `testhost` still holds test output DLLs, but the build must still complete successfully.
- `ai/tasks/pending` contains only `.gitkeep` after the final closure task.
- `/planet` behaves as a dashboard, not a full construction catalog.
- `/construction` stays scoped to general construction.
- `/research` behaves as a development-safe cockpit foundation.
- `/research` keeps the corrected QA path with one seeded available item, visible blocked items, guarded enqueue, and disabled complete-due placeholder.
- `/ground-army` behaves as a readiness cockpit foundation, not a placeholder and not a combat surface.
- `/defenses` behaves as a readiness cockpit foundation, not a placeholder and not a combat surface.
- `/shipyard` behaves as a development-safe cockpit foundation with guarded enqueue, disabled complete-due, and explicit Fleet boundaries.
- `/market` behaves as a read-only economy cockpit foundation with advisory ratios, visible trade signals, disabled future actions, and no transaction execution.
- `/alliance` behaves as a read-only diplomacy cockpit foundation with visible identity, known contacts, disabled future pact and action placeholders, visible handoffs, and no alliance gameplay execution.
- `/ranking` behaves as a read-only power-index cockpit foundation with visible civilization context, category scores, demo comparisons, disabled future placeholders, and no public leaderboard claims.
- `Galaxy` remains read-only.
- `Fleets` still preserves context and read-only command flow.
- `Espionage` remains read-only, `Alliance` remains read-only, `Market` remains read-only, and `Ranking` remains read-only.
- Ranking visual acceptance remains a user-driven seeded browser pass; this closure block validates route availability, lazy loading, and non-visual regressions only.
- No 3D/WebGL renderer is introduced.
- Diagnostics stay collapsed by default on the cockpit routes.

## Repository validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

If npm registry access is unavailable in the current environment, record that limitation explicitly rather than claiming the frontend build passed.

Expected script results:

- `.\scripts\check-frontend-route-lazy-imports.ps1` prints `Frontend route lazy-import guard passed.` when `App.tsx` keeps cockpit routes lazy-loaded.
- `.\scripts\check-dev-qa-scripts.ps1` now includes that same route guard before its existing persisted-QA parser and helper checks.

## Recovery notes

1. `POST /api/dev/fleets/orbital-transfers/create` and `POST /api/dev/fleets/orbital-transfers/cancel` mutate development data, so repeated validation runs can begin from a changed fleet/resource state.
2. Inspect `GET /api/dev/fleets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001` before re-running mutation checks.
3. Re-apply `POST /api/dev/seeds/apply` with `{"profile":"minimal-validation"}` only to restore missing baseline rows.
4. The current `minimal-validation` seed is additive and idempotent. It does not delete extra transfers or groups or reset reserved/stationed state, but it does top up the existing `Aurelia` resource stockpile to at least `125` credits, `160` metal, `100` crystal, and `50` gas.
5. When you need the original baseline back exactly, use a fresh disposable local database and then re-apply the seed.
