# Persisted Gameplay Flow Checklist

This document is the authoritative QA scope note for the current persisted `Construction`, `Research`, `Shipyard`, and `Fleet read-state` development flows.
Use it together with `docs/dev/development-seed-profiles.md`, `docs/dev/construction-cockpit-checklist.md`, `docs/dev/research-cockpit-checklist.md`, and `docs/dev/shipyard-fleet-persisted-qa.md`.

## Orbital production and military block audit

Current accepted block boundary for `Shipyard`, `Defenses`, `Fleets`, and `Planet`:

- Real persisted mutation now:
  - `Shipyard` enqueue through `POST /api/dev/assets/production/enqueue`
- Read-only now:
  - `Shipyard` queue, catalog, stock, and readiness through `GET /api/dev/shipyard/ui-state`
  - `Defenses` readiness through `GET /api/dev/defenses/ui-state`, which is derived from `GET /api/dev/planets/ui-state`
  - `Fleets` post-Shipyard verification through `GET /api/dev/fleets/ui-state`, `GET /api/dev/fleets/overview`, and `GET /api/dev/fleets/action-manifest`
  - `Planet` orbital and military summary context through `GET /api/dev/planets/ui-state`
- Requires future backend work before it becomes an accepted cockpit mutation:
  - defense-specific production enqueue or completion
  - shipyard-safe due processing scoped to the visible planet
  - shipyard-safe stock-to-fleet allocation
  - any fleet mutation being driven from the Shipyard, Defenses, or Planet cockpits for this block

## Scope and safety rules

- Development-only flow.
- No production auth work is required for this block.
- Current auth or onboarding reality is still partial:
  - `POST /api/auth/register` and `GET /api/auth/confirm-email` exist for ASP.NET Core Identity accounts.
  - Starting civilization creation exists only as the Development endpoint `POST /api/dev/players/starting-civilization`.
  - No current cockpit route consumes an authenticated session or resolves the active civilization from login state; cockpit routing still depends on explicit `civilizationId` and often seeded QA ids.
- No manual SQL is part of the accepted QA path.
- No destructive reset behavior is part of the accepted QA path.
- No automatic completion is part of the accepted QA path unless a route is already proven safe and scoped.
- No visual QA is required for this block.
- Shipyard persisted QA stops at enqueue plus read-state verification; it does not use global due-processing.
- Shipyard enqueue follows the same immediate-spend rule already proven for Construction and Research: successful enqueue deducts the full visible cost immediately from the local planet stockpile.
- Fleet read-state remains an accepted post-Shipyard verification step: after a real Shipyard order exists, `GET /api/dev/fleets/ui-state` should still return readable stationed groups, active-transfer summaries, command-readiness metadata, and current-planet resource contexts without introducing new Fleet mutation state.
- Fleet persisted QA for this block is read-only after the Shipyard mutation.
- Stock-to-fleet allocation remains optional and excluded from the accepted default loop until its endpoint is hardened and explicitly re-approved.
- Current allocation audit result: `POST /api/dev/fleets/orbital-groups/create-from-stock` is Development-only, consumes real stock, creates new groups per call, is not idempotent, and remains excluded from the repeated-QA runbook.
- Current frontend confirmation audit result:
  - `Construction` uses the shared `PlanetPage` route variant and renders an inline confirmation panel before submit.
  - `Research` renders its own inline confirmation panel inside `ResearchPage`.
  - `Shipyard` renders its own inline review-and-confirm panel inside `ShipyardPage`.
  - No reusable modal or dialog foundation is currently present under `src/VoidEmpires.Frontend/src/components` or the cockpit pages; this block should not assume one already exists.
- Current resource-economy audit result:
  - Planet resources are persisted in `PlanetResourceStockpile`.
  - Per-planet production rates are persisted in `PlanetProductionProfile`.
  - Elapsed-time accrual exists in backend code through `PlanetEconomyTickService` plus `ResourceProductionService`.
  - The cockpit read endpoints do not automatically apply elapsed production during page load, so visible balances remain whatever persistence last stored.
  - Construction, Research, and Shipyard each spend the full visible cost immediately when enqueue succeeds.

## Queue completion materialization contract v1

Persisted order models audited for this block:

- Construction uses `PlanetConstructionOrder`.
  - Scope fields: `PlanetId`.
  - Queue identity fields: `Id`, `Sequence`.
  - Due fields: `StartsAtUtc`, `EndsAtUtc`.
  - Completion payload fields: `Action`, `BuildingType`, `TargetLevel`.
  - Status values: `Pending`, `Active`, `Completed`, `Cancelled`.
  - Open states: `Pending` or `Active`.
- Research uses `ResearchOrder`.
  - Scope fields: `CivilizationId`, `SourcePlanetId`.
  - Queue identity fields: `Id`, `Sequence`.
  - Due fields: `StartsAtUtc`, `EndsAtUtc`.
  - Completion payload fields: `ResearchType`, `TargetLevel`.
  - Status values: `Pending`, `Active`, `Completed`, `Cancelled`.
  - Open states: `Pending` or `Active`.
- Shipyard and ground/asset production use `AssetProductionOrder`.
  - Scope fields: `PlanetId`.
  - Queue identity fields: `Id`, `Sequence`.
  - Due fields: `StartsAtUtc`, `EndsAtUtc`.
  - Completion payload fields: `Target`, `PlanetaryAssetType`, `SpaceAssetType`, `Quantity`.
  - Status values: `Pending`, `Active`, `Completed`, `Cancelled`.
  - Open states: `Pending` or `Active`.

Existing materialization services:

- Construction: `IConstructionOrderCompletionService` -> `ConstructionOrderCompletionService`.
  - It selects all open construction orders with `EndsAtUtc <= nowUtc`.
  - It marks due orders completed after creating or upgrading the target `PlanetBuilding`.
  - The current Development route is `POST /api/dev/buildings/construction-orders/complete-due`.
- Research: `IResearchOrderCompletionService` -> `ResearchOrderCompletionService`.
  - It selects all open research orders with `EndsAtUtc <= nowUtc`.
  - It creates or upgrades the target `ResearchProject`, then marks the order completed.
  - The current Development route is `POST /api/dev/research/orders/complete-due`.
- Shipyard and other asset production: `IAssetOrderProcessor` -> `AssetOrderProcessor`.
  - It selects all open asset production orders with `EndsAtUtc <= nowUtc`.
  - It increases `PlanetaryAssetStock` or `OrbitalAssetStock`, then marks the order completed.
  - The current Development route is `POST /api/dev/assets/production/process-due`.

Completion state changes:

- Construction materialization is the only step that applies the already-paid order to buildings:
  - `Construct` creates the requested building at `TargetLevel` when it does not already exist.
  - `Upgrade` upgrades the existing building until it reaches `TargetLevel`.
- Research materialization is the only step that applies the already-paid order to technology state:
  - it creates `ResearchProject` if needed.
  - it upgrades the project until it reaches `TargetLevel`.
- Shipyard/orbital materialization is the only step that applies the already-paid production order to local stock:
  - `Target = Orbital` increases `OrbitalAssetStock`.
  - `Target = Planetary` increases `PlanetaryAssetStock`.
- Resources are spent at enqueue time for Construction, Research, and Shipyard. Completion/materialization must not deduct resources a second time.

Safe v1 materialization policy for follow-up implementation:

- Process only due orders where `EndsAtUtc <= nowUtc`.
- Treat terminal states as idempotency guards: `Completed` and `Cancelled` must not be processed again.
- Scope requests as narrowly as the cockpit context allows:
  - Construction by `civilizationId` plus `planetId`, with ownership checked before mutation.
  - Research by `civilizationId`; include `sourcePlanetId` only as an extra filter or validation aid when it is supplied by a planet-bound cockpit.
  - Shipyard by `civilizationId` plus `planetId`, and restrict cockpit usage to orbital production when the request comes from `/shipyard`.
- Return completed order ids and counts from backend-confirmed state.
- Do not run materialization from ordinary page load, sidebar navigation, or read-only UI-state endpoints.
- Keep the first explicit routes/helpers Development-only until production auth and ownership contracts exist.
- A page may offer a manual refresh action only after a scoped backend materialization route exists and a follow-up read confirms the new queue, building/project, stock, and resource state.
- Avoid using current global completion/process routes as cockpit actions because they mutate every due order of that type across the Development database.

## Resource economy accrual contract v1

Chosen authoritative strategy:

- Keep accrual backend-authoritative and explicit.
- Use a materialized backend application step for accrual, not a frontend timer and not an automatic mutation during ordinary cockpit page load.
- The current repository already has the safe primitive for this shape: `IPlanetEconomyTickService` -> `PlanetEconomyTickService` -> `ResourceProductionService`.

Why this strategy is the accepted v1 contract:

- `PlanetResourceStockpile` is the persisted source of truth for visible balances.
- `PlanetProductionProfile` is the persisted source of truth for base hourly income.
- `ResourceProductionService` already defines deterministic proportional accrual from `elapsed.TotalHours`.
- `PlanetEconomyTickService` already materializes those gains into persistence and applies the current `ResourceExtraction` research multiplier.
- The current read endpoints do not store or expose a persisted `lastAppliedAtUtc` watermark, so automatically mutating balances on every page load would risk repeated uncontrolled accrual.

Deterministic accrual rules for v1:

- Accrual is planet-scoped.
- Base rates come from persisted `PlanetProductionProfile`:
  - `CreditsPerHour`
  - `MetalPerHour`
  - `CrystalPerHour`
  - `GasPerHour`
- Effective rates are multiplied by the civilization's current `ResourceExtraction` research bonus through `ResearchBonusCalculator`.
- Produced amounts are linear over elapsed real time:
  - `produced = effectivePerHour * elapsed.TotalHours`
- Zero elapsed time is a valid no-op.
- Negative elapsed time is invalid and must be rejected.
- Production and stockpile rows must belong to the same planet; mismatched or missing persistence state must fail clearly rather than fabricating balances.
- v1 accepts decimal resource accumulation in persistence; it does not require integer-only truncation.

Current baseline rates that v1 must preserve when profile data is seeded or created:

- Shared seeded minimal/profile baseline for `Aurelia`:
  - credits per hour `18`
  - metal per hour `14`
  - crystal per hour `6`
  - gas per hour `3`
- Current Development-only playable-start backend baseline:
  - credits per hour `18`
  - metal per hour `14`
  - crystal per hour `6`
  - gas per hour `3`

Explicit v1 limitations and non-goals:

- No frontend-only increasing timer.
- No hidden resource mutation on normal `GET /api/dev/planets/ui-state` reads.
- No premium acceleration, boosts, officers, consumables, or catch-up bundles.
- No claim that background workers already own resource accrual.
- No claim that Planet, Market, Research, Construction, or Shipyard currently show freshly materialized gains unless an explicit backend accrual step ran first.
- No visual QA claim for accrual behavior in this documentation-only task.

Implementation guidance for follow-up tasks:

- `TASK-32K` should introduce the first explicit backend application path for resource accrual using the existing tick-service contract rather than inventing a second accrual formula.
- `TASK-32L` should refresh frontend-visible balances only from backend-confirmed state after that explicit accrual path exists.

## Safe baseline

Recommended deterministic setup:

1. Apply `planet-full-validation` before Construction QA.
2. Apply `research-validation` before Research QA.
3. Use a fresh disposable local database only when an exact pre-enqueue baseline is required.
4. Reapply the documented seed when local state becomes confusing, but do not rely on reseeding to clear open orders.

Repeatable backend-only baseline helper:

- Run `.\scripts\dev-qa-baseline.ps1` to verify the seed catalog, apply `cockpit-validation` twice, and print the current Construction/Planet, Research, Shipyard, and Fleet baseline snapshots before creating any real orders.
- Run `.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed` to apply a deterministic Construction seed, enqueue one real Construction order through the dev API, and print queue plus stockpile deltas from the authoritative read model.
- Run `.\scripts\dev-qa-create-research-order.ps1 -ApplySeed` to apply a deterministic Research seed, enqueue one real Research order through the dev API using backend-provided command metadata, and print queue plus stockpile deltas.
- Run `.\scripts\dev-qa-create-shipyard-production-order.ps1 -ApplySeed` to apply the shared cockpit seed, enqueue one real orbital Shipyard order through backend-provided command metadata, and print queue, stock, and stockpile deltas.
- Run `.\scripts\dev-qa-fleet-read-state.ps1` to re-read Fleet UI state after seed application or Shipyard enqueue and print group, stationed, transfer, and local resource-context summaries without mutating anything.
- Run `.\scripts\check-dev-qa-scripts.ps1` to parser-check the persisted QA PowerShell helpers and run lightweight local formatting checks without requiring the backend.
- Run `.\scripts\dev-qa-prepare-construction-ui-state.ps1` when you need to clear reused-DB blocking Construction state before attempting another Enqueue path.
- Run `.\scripts\dev-qa-prepare-research-ui-state.ps1` when you need to clear reused-DB blocking Research state before attempting another Enqueue path.
- Run `.\scripts\dev-qa-prepare-orbital-production-ui-state.ps1` when you need to clear reused-DB blocking orbital production state before another Shipyard enqueue attempt.
- Exact repeated Research QA preparation command:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-research-ui-state.ps1`
- There is intentionally no `dev-qa-create-orbital-group-from-stock.ps1` helper in this block because stock-to-fleet allocation is still excluded from the accepted reused-database QA loop.
- Exact repeated Shipyard QA preparation command:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-orbital-production-ui-state.ps1`

Construction now has four sanctioned Development QA paths:

- Backend-only helper path:
  - `.\scripts\dev-qa-prepare-construction-ui-state.ps1`
  - `.\scripts\dev-qa-prepare-construction-ui-state.ps1 -CivilizationId ... -PlanetId ...`
  - Useful for clearing open order blockers and topping stockpile minima before another repeated enqueue attempt.
  - This helper is explicit about mutating Development data and does not reset unrelated cockpits.
- Backend-only enqueue path:
  - `.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed`
  - Useful when you want copy-pasteable queue and resource deltas directly in the terminal
- Frontend-confirmed cockpit path:
  - `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
  - Useful when you need to verify the guarded review, confirmation, Spanish copy, and post-submit refresh behavior
- Both paths create real Development database rows through the same persisted enqueue endpoint.

Research now has two sanctioned Development QA paths:

- Backend-only preparation path:
  - `.\scripts\dev-qa-prepare-research-ui-state.ps1`
  - Useful for clearing only open `Pending` or `Active` research orders for the targeted civilization and topping only the targeted source-planet stockpile before another repeated enqueue attempt.
- Backend-only helper path:
  - `.\scripts\dev-qa-create-research-order.ps1 -ApplySeed`
  - Useful when you want queue and resource deltas directly in the terminal while staying on backend-issued `enqueueCommand` metadata.
- Frontend-confirmed cockpit path:
  - `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
  - Useful when you need to verify the guarded review step, confirmation checkbox, Spanish blocker copy, and post-submit refresh behavior.
- Both Research paths create real Development database rows through the same persisted enqueue endpoint.
- On a reused Development database, either path may encounter `Civilization already has an open research order.`; this is an expected no-op state rather than a hidden reset path.

Research QA state preparation contract:

- Development-only boundary.
- Default target:
  - civilization `00000000-0000-0000-0000-000000000001`
  - source planet `40000000-0000-0000-0000-000000000001`
- Safe mutation scope:
  - neutralize only blocking open research orders for the targeted civilization
  - preserve completed or historical research rows that do not block enqueue
  - top up only the targeted source-planet stockpile to a deterministic QA minimum when needed
- Preferred implementation rule:
  - mark open research orders as `Cancelled` instead of completing them or deleting them
  - do not use `POST /api/dev/research/orders/complete-due` for this preparation path because it is global and unsafe for cockpit QA
- This preparation step must stay explicit and manual. It must not run during ordinary page load or ordinary seed application.

Shipyard QA state preparation contract:

- Development-only boundary.
- Default target:
  - civilization `00000000-0000-0000-0000-000000000001`
  - planet `40000000-0000-0000-0000-000000000001`
- Safe mutation scope:
  - cancel only open `Pending` or `Active` asset production orders for the targeted planet
  - preserve completed or historical orbital production rows
  - top up only the targeted planet stockpile to the shared orbital QA minimums needed for the deterministic `ScoutCraft` path
- Preferred implementation rule:
  - mark open asset production blockers as `Cancelled` instead of completing them, deleting them, or calling `POST /api/dev/assets/production/process-due`
  - do not mutate unrelated planets or civilizations while preparing the target Shipyard state
- Current Defenses scope note:
  - there is still no safe `POST /api/dev/defenses/...` enqueue route in this block, so this preparation contract is Shipyard-only even though Defenses may reuse the same planet resource context for read-state QA

Ordered runtime QA command sequence for the frontend-confirmed Research path:

```powershell
dotnet run --project .\src\VoidEmpires.Web
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-research-ui-state.ps1
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
npm run dev --prefix src/VoidEmpires.Frontend
```

- Then open `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- Select the seeded available research card, open the guarded confirmation panel, and check the acknowledgement box before submit.
- On success, expect backend-confirmed feedback plus refreshed resources, queue state, and visible progress from the follow-up read model.
- On a reused Development database, an existing open order is an expected no-op warning, not evidence that the route or helper created a second order.
- For repeated success-path QA on a reused Development database, run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-research-ui-state.ps1` first.
- Backend-only fallback for the same persisted mutation remains `.\scripts\dev-qa-create-research-order.ps1 -ApplySeed`.

Shared helper defaults:

- Base URL default: `http://localhost:5142`
- Default civilization id: `00000000-0000-0000-0000-000000000001`
- Default owned planet id: `40000000-0000-0000-0000-000000000001`
- If the backend is not running or persistence is unavailable, the helpers fail clearly and do not attempt cleanup or reseed resets.

## Playable session foundation audit

Confirmed current baseline after the current playable-start foundation:

- Identity and player model:
  - ASP.NET Core Identity owns account registration and email confirmation.
  - `PlayerProfile` persists `UserId`, `DisplayName`, and related `Civilization` rows.
  - `Civilization` persists `PlayerProfileId`, `Name`, `Archetype`, `Status`, and optional `HomePlanetId`.
  - `PlanetOwnership` is the active ownership link between a planet and a civilization.
- Current runtime wiring:
  - `Program.cs` maps `POST /api/auth/register` and `GET /api/auth/confirm-email`.
  - `Program.cs` does not enable `UseAuthentication()` or `UseAuthorization()`.
  - Current cockpit and development routes still require explicit `civilizationId` and, for most planet-bound views, explicit `planetId`.
- Current Development-only bootstrap contract:
  - `POST /api/dev/players/starting-civilization` creates a Development-only playable start, not a production-auth session.
  - The frontend sends `displayName`, `civilizationName`, and optional `homePlanetName`; the backend API shape also accepts optional `userId` and `archetype`.
  - A successful response returns `succeeded`, `userId`, `playerProfileId`, `civilizationId`, `homePlanetId`, `homePlanetName`, `homeSystemId`, `homeSystemName`, `startingResources`, `limitations`, and `errors`.
  - The current service creates `PlayerProfile`, `Civilization`, a home solar system, a colonized home planet, active `PlanetOwnership`, starting resource stockpile, production, population, capacity, and baseline buildings for the new Development-only start.
  - The route still does not verify an authenticated session and does not create login cookies, bearer tokens, production authorization claims, or a production active-civilization session.
- Current onboarding gap:
  - The repository has no production-safe session-to-civilization bootstrap flow yet.
  - The existing starting-civilization creation route is Development-only and separate from account registration.
  - `/onboarding` redirects to `/planet` with the returned `civilizationId` and `homePlanetId` as `planetId`.
  - Frontend cockpit routes still resolve context from explicit query parameters rather than from authenticated user state.
- Safe implementation scope for this block:
  - treat current cockpit work as a seeded Development-only gameplay foundation
  - preserve explicit `civilizationId` and `planetId` handoff behavior unless a later task adds a real authenticated bootstrap contract
  - do not claim production authentication, player session resolution, or automatic user-to-civilization onboarding
  - treat `POST /api/dev/players/starting-civilization` as a Development-only playable-start helper only, even though it now produces owned homeworld gameplay state

Safe onboarding decision for this block:

- Use a Development-only playable-start flow, not a production auth-backed onboarding flow.
- The correct baseline for repeatable shared cockpit QA is still the deterministic seed system, especially `cockpit-validation`.
- Real account registration may coexist in the repository, but it is not yet wired to civilization resolution, owned-planet bootstrap, or cockpit navigation.

Playable session QA helper:

- Exact command for a one-hour materialized Development start:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-playable-session-state.ps1 -ElapsedSeconds 3600`
- The helper creates a real Development-only playable start through `POST /api/dev/players/starting-civilization`.
- When `-ElapsedSeconds` is greater than zero, it applies backend resource accrual through the explicit Development economy endpoint and then re-reads the planet state.
- The helper prints the created `UserId`, `PlayerProfileId`, `CivilizationId`, `HomePlanetId`, homeworld names, starting resources, current resources, and resource deltas when accrual is materialized.
- The helper does not open a browser, does not inspect frontend rendering, and does not perform visual QA.
- Treat its printed ids and route-ready context as setup input for a later manual browser pass, not as proof that cockpit screens were visually verified.

Expected result payload for the safe Development-only playable-start contract:

- Identity block:
  - `userId`
  - `playerProfileId`
  - display name from the submitted request
- Civilization block:
  - `civilizationId`
  - civilization name from the submitted request
  - optional `archetype` from the submitted request
  - `status = Active`
- Homeworld block:
  - `homePlanetId`
  - `homePlanetName`
  - `homeSystemId`
  - `homeSystemName`
  - `ownershipStatus = Active`
- Starting resources block:
  - `credits`
  - `metal`
  - `crystal`
  - `gas`
- Limitations block:
  - explicit Development-only limitation strings
  - error strings on validation or duplicate-name failures
- Navigation block:
  - canonical QA URLs or query ids needed to open `/galaxy`, `/planet`, `/construction`, `/research`, `/shipyard`, `/defenses`, `/ground-army`, `/market`, `/espionage`, `/alliance`, `/ranking`, and `/fleets`

Safe local navigation persistence contract for follow-up tasks:

- `localStorage` may store only non-sensitive navigation context returned by the Development-only playable start.
- Allowed fields are `civilizationId`, `planetId` or `homePlanetId`, player/display name, civilization name, planet name, and client timestamps when useful for debugging stale local context.
- `localStorage` is only a navigation convenience for rebuilding cockpit URLs. It is never authoritative game state, ownership, entitlement, authorization, or identity.
- The backend remains the source of truth for player, civilization, planet ownership, resources, queues, fleets, and every gameplay mutation.
- Do not store production auth data, login state, bearer tokens, cookies, email-confirmation state, role claims, or any session claim in this local cockpit context.
- Do not infer production authentication from the presence of a local playable context. Missing or stale local values must fall back to explicit route entry or onboarding, not hidden backend authority.
- The current frontend has no unit-test runner; the static guard for the typed helper is `npm run build --prefix src/VoidEmpires.Frontend`, which type-checks `src/VoidEmpires.Frontend/src/utils/playableSession.ts`.

Explicit limitations of the current onboarding story:

- No authenticated session is currently resolved into an active civilization automatically.
- No production-safe login or session middleware protects cockpit routes today.
- No current endpoint combines account creation, email confirmation, player profile creation, civilization creation, homeworld claiming, and seeded economy bootstrap into one safe production flow.
- The existing Development-only starting-civilization route is the accepted local playable-start helper, but it must not be overclaimed as production onboarding.

Shipyard/Fleet companion note:

- Use `docs/dev/shipyard-fleet-persisted-qa.md` for the exact endpoint inventory, read-model fields, and current inclusion or exclusion rules for Shipyard and Fleets.
- The companion note now also carries the copy-pasteable backend-only runbook for `baseline -> shipyard enqueue -> fleet read-state` using the final helper script names.

Current cockpit audit note for the wider orbital or military block:

- `Defenses` remains a read-only readiness cockpit backed by planet construction data and does not have an accepted defense-specific mutation route.
- `Planet` may surface orbital or military summaries only as already-backed read state such as stationed-group counts, active transfer counts, building readiness, queue visibility, and resource context.
- `Fleets` still owns transfer, split, merge, and completion mutations in the broader repository, but those flows are outside this block's accepted persisted Shipyard verification loop.

Primary QA URLs:

- Construction: `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Research: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`

## Backend-only runbook

This is the practical backend-only QA path for real persisted rows. It is intended for a local Development backend only.

### 1. Start the backend

```powershell
dotnet run --project .\src\VoidEmpires.Web
```

Warnings:

- These commands create real rows in the Development database.
- Do not run this flow against production.
- No manual SQL is required or recommended.
- No script in this checklist runs migrations, deletes rows, or clears queues.

### 2. Capture a clean baseline

Run the shared baseline helper first:

```powershell
.\scripts\dev-qa-baseline.ps1
```

What to confirm:

- `cockpit-validation` exists in the seed catalog.
- Applying the seed twice succeeds.
- Construction and Research state load for civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
- The baseline output shows current queue counts before you create any new real order.
- Construction resources come from `uiState.planet.stockpile` rows shaped as `resourceType + quantity`, not `resourceType + amount`.
- Research availability comes from `uiState.technologyHints`, and Research queue counts come from `uiState.queue`.
- If resource formatting cannot recognize the current DTO shape, the script now prints a warning instead of crashing.

### 3. Create one real Construction order

Use the focused Construction helper:

```powershell
.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed
```

Optional explicit target:

```powershell
.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed -BuildingType MetalMine
```

What success looks like:

- The script prints `Real persisted construction order created.`
- The script prints the selected backend action and a sanitized payload summary before the POST.
- `QueueAfter` is greater than `QueueBefore`.
- `OpenQueueAfter` is greater than `OpenQueueBefore`.
- The resource delta shows negative values for the spent resources.
- The response includes a real `OrderId`, `StartsAtUtc`, and `EndsAtUtc`.

What failure looks like:

- The helper fails clearly if the backend is unreachable.
- A conflict response prints backend errors such as `Planet already has an open construction order.` or `Planet is not owned by the requesting civilization.`
- If no available Construction action exists, the helper prints the queue counts so you can see whether the planet is already occupied.
- If the backend rejects the payload with `400`, the helper now prints the endpoint, the selected option label, the selected backend action value, a sanitized payload summary, and the response body.
- No cleanup or destructive reset runs automatically after a failure.

### 4. Create one real Research order

Use the Research helper:

```powershell
.\scripts\dev-qa-create-research-order.ps1 -ApplySeed
```

Optional explicit target:

```powershell
.\scripts\dev-qa-create-research-order.ps1 -ApplySeed -ResearchType PlanetaryEngineering
```

What success looks like:

- The script prints `Real persisted research order created.`
- The script prints the selected research and a sanitized payload summary before the POST.
- `QueueAfter` is greater than `QueueBefore`.
- `OpenQueueAfter` is greater than `OpenQueueBefore`.
- The resource delta shows negative values for the spent resources.
- The response includes a real `OrderId`, `StartsAtUtc`, and `EndsAtUtc`.

What failure looks like:

- The helper fails clearly if the backend is unreachable.
- A conflict response prints backend errors such as `Civilization already has an open research order.` or `Planet is not owned by the requesting civilization.`
- If the backend returns `Civilization already has an open research order.`, the helper now treats it as an expected reused-Development-database state, re-reads the queue, prints a concise summary, and exits without creating a second order.
- The helper matches that condition from either parsed JSON errors or the raw HTTP body text, because Development QA may be run from Windows PowerShell where JSON parsing capabilities differ from newer shells.
- If no available research exists because the queue is already occupied, the helper now says so explicitly instead of failing with a vague empty-selection message.
- No cleanup or destructive reset runs automatically after a failure.

### 5. Re-read authoritative state

If you want direct read-model confirmation after either helper, fetch state again:

```powershell
Invoke-RestMethod "http://localhost:5142/api/dev/planets/ui-state?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
Invoke-RestMethod "http://localhost:5142/api/dev/research/ui-state?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
```

What to confirm:

- Construction read-state now includes the new queue row and reduced local stockpile.
- Research read-state now includes the new active queue row and no longer exposes the same hint as immediately enqueueable.
- The read model reflects persisted backend state rather than optimistic UI state.

### 6. Confirm seed reapply preserves manual orders

Re-run the baseline helper or reapply the seed manually:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'
```

Then fetch state again with the read endpoints above.

What to confirm:

- Existing manual Construction orders are still present.
- Existing manual Research orders are still present.
- Seed reapply can restore deterministic read surfaces, but it does not delete already created active orders.
- If you need an exact pre-enqueue baseline again, switch to a fresh disposable local database instead of relying on reseed cleanup.

## Endpoint inventory

Construction persisted mutation and read surfaces: `POST /api/dev/buildings/construction-orders/enqueue`, `POST /api/dev/buildings/construction-orders/complete-due`, `GET /api/dev/planets/ui-state?civilizationId={id}&planetId={id}`, `POST /api/dev/construction/qa-state/prepare`

Research persisted mutation and read surfaces: `POST /api/dev/research/orders/enqueue`, `POST /api/dev/research/orders/complete-due`, `GET /api/dev/research/ui-state?civilizationId={id}&planetId={id}`

Seed/bootstrap surfaces used by the safe QA loop: `POST /api/dev/seeds/apply`, `GET /api/dev/seeds/profiles`

Shipyard/Fleet persisted mutation and read surfaces for the current block: `POST /api/dev/assets/production/enqueue`, `POST /api/dev/assets/production/process-due`, `POST /api/dev/shipyard/qa-state/prepare`, `GET /api/dev/shipyard/ui-state?civilizationId={id}&planetId={id}`, `GET /api/dev/fleets/ui-state?civilizationId={id}`, `GET /api/dev/fleets/overview?civilizationId={id}`, `GET /api/dev/fleets/action-manifest`, `POST /api/dev/fleets/orbital-groups/create-from-stock`

## Runtime contract notes for PowerShell helpers

Current audited object paths:

- Construction resources or reserves:
  - `/api/dev/planets/ui-state`
  - path: `uiState.planet.stockpile`
  - row shape: `resourceType + quantity`
- Construction queue count:
  - path: `uiState.planet.constructionQueue`
- Construction available actions:
  - path: `uiState.planet.constructionActions`
  - available rows currently expose `availabilityStatus = "Available"`
- Research resources used by the helper:
  - the Research script re-reads `/api/dev/planets/ui-state`
  - path: `uiState.planet.stockpile`
  - row shape: `resourceType + quantity`
- Research queue count:
  - `/api/dev/research/ui-state`
  - path: `uiState.queue`
- Research available actions:
  - path: `uiState.technologyHints`
  - available rows currently expose `canEnqueue = true` and an `enqueueCommand`

Accepted script behavior:

- The scripts should prefer authoritative backend fields such as `action`, `buildingType`, `researchType`, and `enqueueCommand`.
- The scripts may format resources from multiple shapes defensively, but they must not hide real HTTP failures.
- If resource formatting cannot recognize the current shape, the script should print a readable warning instead of throwing.
- Known Development conflict states should be recognized from both parsed response objects and raw JSON body text so the Research helper stays stable across supported PowerShell runtimes.
- Construction enqueue currently expects the request body shape:
  - `planetId`
  - `civilizationId`
  - `action`
  - `buildingType`
  - `requestedAtUtc`
- The current backend request type does not use the Research-style string enum converter for `action`, so the helper must send the backend-compatible action value from the read model instead of Spanish display text or stringified UI labels.

## Dependency map

Construction flow:

- `Program.cs` -> `MapDevEndpointMappings()` and `MapDevPlanetUiStateEndpoints()`
- `/api/dev/buildings/construction-orders/enqueue` -> `IPlanetConstructionQueueService` -> `PlanetConstructionQueueService`
- `PlanetConstructionQueueService` -> `VoidEmpiresDbContext` tables for `PlanetConstructionOrders`, `PlanetBuildings`, `PlanetBuildingCapacities`, `PlanetResourceStockpiles`, `ResearchProjects`
- `/api/dev/planets/ui-state` -> `IDevPlanetUiStateService` -> `DevPlanetUiStateService`
- `DevPlanetUiStateService` -> `PlanetConstructionOrders`, `PlanetBuildings`, `PlanetBuildingCapacities`, `PlanetResourceStockpiles`, `ResearchProjects`, ownership/system context

Research flow:

- `Program.cs` -> `MapDevEndpointMappings()` and `MapDevResearchUiStateEndpoints()`
- `/api/dev/research/orders/enqueue` -> `IResearchQueueService` -> `ResearchQueueService`
- `ResearchQueueService` -> `ResearchEnqueueReadinessEvaluator` -> `VoidEmpiresDbContext` tables for `ResearchOrders`, `ResearchProjects`, `PlanetOwnerships`, `PlanetResourceStockpiles`
- `/api/dev/research/ui-state` -> direct web endpoint query in `DevResearchUiStateEndpoints`
- `DevResearchUiStateEndpoints` -> `ResearchOrders`, `ResearchProjects`, `PlanetOwnership`, `PlanetResourceStockpiles`, civilization/planet context

Worker/completion chain present today but not part of the safe cockpit QA loop:

- `AddVoidEmpiresConstructionQueueWorker()` -> `ConstructionQueueWorker` -> `IConstructionOrderCompletionService`
- `AddVoidEmpiresResearchQueueWorker()` -> `ResearchProgressWorker` -> `IResearchOrderCompletionService`

## Construction persisted flow

Safe mutation path:

1. Apply `planet-full-validation`.
2. Read current colony state from `GET /api/dev/planets/ui-state`.
3. Choose an action that the read model marks as `AvailabilityStatus = "Available"`.
4. Enqueue with `POST /api/dev/buildings/construction-orders/enqueue`.
5. Refresh from `GET /api/dev/planets/ui-state` and verify the queue changed from read-model state, not local optimistic state.

Repeatable backend-only helper:

- `.\scripts\dev-qa-prepare-construction-ui-state.ps1`
  - Use this first when your reused Development database has a pre-existing open construction order.
  - Defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
  - Returns exact blocking order counts before/after and resource top-up before/after if stockpile exists.
- `.\scripts\dev-qa-create-construction-order.ps1`
- Defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
- Add `-ApplySeed` to apply `planet-full-validation` before the enqueue.
- Add `-BuildingType MetalMine` or another available type to force a specific safe action instead of auto-picking the first available one.
- The helper requires a running Development backend, creates a real persisted row, and prints before or after queue plus resource changes without running migrations or cleanup.

Frontend-confirmed cockpit path:

- Load `/construction` with the deterministic civilization and planet ids.
- Select only actions the read model marks as available.
- Confirm the guarded review step before sending the order.
- The route posts to the same persisted enqueue endpoint as the backend helper.
- A successful submit still requires a follow-up read from `/api/dev/planets/ui-state`; the cockpit may report accepted-but-not-yet-visible state, but it must not fabricate queue rows.

Current enqueue behavior:

- Requires `planetId`, `civilizationId`, `action`, `buildingType`, and UTC `requestedAtUtc`.
- Rejects a second open construction order for the same planet.
- Spends the full construction cost immediately from `PlanetResourceStockpile`.
- Persists a `PlanetConstructionOrder` row immediately with `Status = Active`.
- Enforces capacity only for `Construct`, including `PlanetaryEngineering` bonus capacity.
- Uses `ConstructionAutomation` research to shorten duration.
- Does not require production auth.

Current read behavior after mutation:

- `GET /api/dev/planets/ui-state` returns current stockpile, buildings, queue, action summary, and action availability for the selected planet.
- The queue is planet-scoped and ordered with open items first, then completed history.
- The action summary already marks complete-due as unsupported in this cockpit.

Resource handling:

- Construction does not use `IResourceSpendService`.
- `PlanetConstructionQueueService` validates affordability directly against the planet stockpile and calls `stockpile.Spend(...)` before saving the order.
- Current verified rule: Construction deducts the full visible cost immediately when enqueue succeeds. It does not use a separate reservation-only stockpile model.

Scope enforcement:

- Queue uniqueness is enforced per planet for open orders.
- Read state exposes management details only for owned planets.
- Mutation logic depends on the requested `planetId`; there is no separate ownership check in the enqueue service today.

## Research persisted flow

Safe mutation path:

1. Apply `research-validation`.
2. Read current state from `GET /api/dev/research/ui-state`.
3. Choose the technology hint with `CanEnqueue = true`, currently expected to be `PlanetaryEngineering` in the standard seeded flow.
4. Enqueue with `POST /api/dev/research/orders/enqueue`.
5. Refresh from `GET /api/dev/research/ui-state` and verify the queue changed from persisted backend state.

Repeatable backend-only helper:

- `.\scripts\dev-qa-prepare-research-ui-state.ps1`
  - Use this first when your reused Development database has a pre-existing open research order.
  - Defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
  - Safe target state:
    - no open `Pending` or `Active` research order for the targeted civilization
    - enough targeted source-planet resources for at least one available enqueue candidate
    - existing completed history may remain
  - Open research blockers should be preserved as history by moving them to `Cancelled`, not by deleting them or forcing global completion.
- `.\scripts\dev-qa-create-research-order.ps1`
- Defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
- Add `-ApplySeed` to apply `research-validation` before the enqueue.
- Add `-ResearchType PlanetaryEngineering` or another available type to force the exact enqueue target instead of auto-picking the first available hint.
- The helper reads Research command metadata from `/api/dev/research/ui-state`, posts the real enqueue request, then re-reads Research and Planet state to print queue plus resource changes without running migrations or cleanup.

Frontend-confirmed cockpit path:

- Load `/research` with the deterministic civilization and planet ids.
- Prepare only a card whose read model exposes `canEnqueue = true` and a non-null `enqueueCommand`.
- Confirm the acknowledgement checkbox before sending the real order.
- The route posts to the same persisted enqueue endpoint as the backend helper and then re-reads backend state before finalizing the visible queue.
- On a reused Development database, the route may surface `Civilization already has an open research order.` as a safe no-op state rather than pretending a second order was created.

Current enqueue behavior:

- Requires `civilizationId`, `sourcePlanetId`, `researchType`, and UTC `requestedAtUtc`.
- Accepts frontend string enum payloads such as `PlanetaryEngineering`.
- The route only exists when Development endpoints are enabled, and it returns `503` if persistence is not configured.
- Requires the source planet to be actively owned by the requesting civilization.
- Rejects a second open research order for the civilization.
- Spends the full research cost immediately from the source planet stockpile.
- Persists a `ResearchOrder` row immediately with `Status = Active`.
- Uses current `EnergySystems` project level to shorten duration.
- Does not create or upgrade `ResearchProject` until completion.
- Web-layer `400` validation currently covers:
  - missing or empty `civilizationId`
  - missing or empty `sourcePlanetId`
  - missing `researchType`
  - missing `requestedAtUtc`
  - non-UTC `requestedAtUtc`
- Current `409` conflict set from the service layer is:
  - `Planet is not owned by the requesting civilization.`
  - `Civilization already has an open research order.`
  - `Planet resource stockpile was not found.`
  - `Insufficient resources.`
- The current backend does not yet expose dedicated enqueue rejections for:
  - hidden prerequisite failures
  - invalid civilization as a distinct lookup error
  - already researched or max-level caps
  - technology-specific availability beyond ownership, queue occupancy, stockpile presence, and resource affordability

Current read behavior after mutation:

- `GET /api/dev/research/ui-state` is a direct read endpoint in the web layer.
- The read model returns queue rows, completed projects, and per-technology readiness hints.
- Readiness currently distinguishes owned-source availability, open queue slot, stockpile presence, and insufficient resources.
- The endpoint is read-only and explicitly states that it does not execute research effects.

Resource handling:

- Research does not use `IResourceSpendService`.
- `ResearchQueueService` validates readiness through `ResearchEnqueueReadinessEvaluator` and then calls `stockpile.Spend(...)` before saving the order.
- Current verified rule: Research deducts the full visible cost immediately when enqueue succeeds. It does not reserve resources and leave balances unchanged.

Scope enforcement:

- Open queue uniqueness is enforced per civilization.
- Ownership is checked against `PlanetOwnerships` for the requested `sourcePlanetId`.
- The read model only exposes an owned selected planet; unknown civilization or foreign planet selection returns `404`.
- Safe cockpit mutation should stay constrained to backend-issued `enqueueCommand` metadata from `GET /api/dev/research/ui-state`, followed by a fresh read after `201 Created`.

## Completion route classification

Construction completion route:

- Route: `POST /api/dev/buildings/construction-orders/complete-due`
- Classification for this block: global and unsafe for cockpit QA
- Reason: `ConstructionOrderCompletionService` completes every due open construction order whose `EndsAtUtc <= nowUtc`, without planet or civilization scoping.

Research completion route:

- Route: `POST /api/dev/research/orders/complete-due`
- Classification for this block: global and unsafe for cockpit QA
- Reason: `ResearchOrderCompletionService` completes every due open research order whose `EndsAtUtc <= nowUtc`, without civilization, planet, or cockpit scoping.

Worker execution:

- Construction and Research background workers can invoke the same global completion services when enabled.
- Those workers are infrastructure capabilities, not part of the accepted manual QA path for this block.

Accepted guidance for this block:

- Do not use manual `complete-due` calls as the standard Construction or Research cockpit QA flow.
- Do not rely on workers for repeatable cockpit QA assertions.

## Existing test coverage

Construction coverage already present:

- `tests/VoidEmpires.Tests/PlanetConstructionQueueServiceTests.cs`
- `tests/VoidEmpires.Tests/ConstructionOrderCompletionServiceTests.cs`
- `tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevDevelopmentSeedEndpointTests.cs`

Research coverage already present:

- `tests/VoidEmpires.Tests/ResearchQueueServiceTests.cs`
- `tests/VoidEmpires.Tests/ResearchOrderCompletionServiceTests.cs`
- `tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs`
- `tests/VoidEmpires.Tests/DevDevelopmentSeedEndpointTests.cs`

Concrete gaps for later tasks:

- No persisted end-to-end Construction smoke test currently proves `seed -> ui-state -> enqueue -> refreshed ui-state` through the real endpoint and real database.
- No persisted end-to-end Research smoke test currently proves the same loop through the real endpoint and real database outside the UI-state endpoint test fixture.
- No current test asserts before/after stockpile deltas for both loops in one QA-focused scenario document.
- No current test fences the global `complete-due` routes with cockpit-specific scoping, so those routes must remain out of the accepted manual flow.

## PowerShell script checks

Parser and lightweight helper validation:

```powershell
.\scripts\check-dev-qa-scripts.ps1
```

What this check covers:

- PowerShell parsing for the persisted QA helper scripts
- resource formatting for row-based `amount` shapes
- resource formatting for flat `credits/metal/crystal/gas` objects
- unknown-shape fallback behavior

What this check does not cover:

- live backend runtime success
- actual HTTP responses
- queue mutation or persistence

## Final concise runtime checklist

1. Start the backend:

```powershell
dotnet run --project .\src\VoidEmpires.Web
```

2. Parser-check the QA scripts:

```powershell
.\scripts\check-dev-qa-scripts.ps1
```

3. Capture a seeded baseline:

```powershell
.\scripts\dev-qa-baseline.ps1
```

Expected success:

- profile catalog loads
- `cockpit-validation` applies twice
- Construction, Research, Shipyard, and Fleet baseline snapshots print
- resources print or a readable formatting warning is shown

4. Create one Construction order:

```powershell
\.\scripts\dev-qa-prepare-construction-ui-state.ps1
.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed
```

5. Create one Construction order:

```powershell
.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed
```

Expected success:

- selected action is printed
- queue counts increase
- a real `OrderId` is returned

Expected controlled failure:

- queue already occupied
- no available action
- backend conflict with a real message

Alternative frontend check for the same persisted mutation:

- Open `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Prepare one available action and confirm it from the UI
- Expected success:
  - the route shows a backend-confirmed success message
  - the route re-reads backend state instead of inserting a fake local row
  - diagnostics can show visible queue and resource changes from the refreshed read model
- Expected honest lag handling:
  - if the backend accepts the order but the immediate refresh does not yet expose it, the route says `La orden fue aceptada por el backend; la cola visible se actualizara con la siguiente lectura disponible.`

5. Create one Research order:

```powershell
.\scripts\dev-qa-create-research-order.ps1 -ApplySeed
```

Expected success:

- selected research is printed
- queue counts increase
- a real `OrderId` is returned

Expected controlled failure:

- queue already occupied
- no available research hint
- backend conflict with a real message
- repeated runs on a reused database may report that an open research order already exists; this is now treated as a safe no-op state, not as an unhandled fatal failure

Alternative frontend check for the same persisted mutation:

- Open `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Runtime command order for this pass:
  - start backend with `dotnet run --project .\src\VoidEmpires.Web`
  - apply `cockpit-validation` twice with `POST /api/dev/seeds/apply`
  - start frontend with `npm run dev --prefix src/VoidEmpires.Frontend`
- Prepare the available research card and confirm the guarded submit from the UI.
- Expected success:
  - the route shows a backend-confirmed success message
  - the route re-reads backend state instead of inserting a fake local queue row
  - the refreshed state shows updated queue, resources, and visible progress rather than only a local toast
  - diagnostics keep the backend payload secondary while the main cockpit stays Spanish-first
- Expected controlled no-op:
  - on a reused Development database, the route may report that an open research order already exists
  - that state still reflects the real persisted Development database and must not be presented as a fresh enqueue success

6. Reapply `cockpit-validation` and baseline again:

```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5142/api/dev/seeds/apply" -ContentType "application/json" -Body '{"profile":"cockpit-validation"}'
.\scripts\dev-qa-baseline.ps1
```

Expected result:

- the baseline still runs
- pre-existing manual orders remain present
- manual Shipyard orders are now also covered on this reused-database path: reapply preserves the active orbital production row and does not duplicate seeded completed Shipyard history
- no destructive reset occurs

7. Create one Shipyard order:

```powershell
.\scripts\dev-qa-prepare-orbital-production-ui-state.ps1
.\scripts\dev-qa-create-shipyard-production-order.ps1 -ApplySeed
```

Expected success:

- selected orbital asset and payload summary are printed
- queue counts increase
- a real `OrderId` is returned
- local resources decrease immediately
- local orbital stock remains unchanged immediately after enqueue

Expected controlled no-op:

- repeated runs on a reused database may report that an open orbital production order already exists
- the helper re-reads Shipyard state, prints the current queue summary, and exits without creating a second order
- the explicit recovery path is `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-orbital-production-ui-state.ps1`, which cancels only the targeted planet blockers and does not imply that visual QA has already been rerun

Alternative frontend-confirmed runtime order for this orbital-preparation pass:

- start backend with `dotnet run --project .\src\VoidEmpires.Web`
- apply `cockpit-validation` twice
- run `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\dev-qa-prepare-orbital-production-ui-state.ps1` before repeated Shipyard success-path QA on a reused Development database
- start frontend with `npm run dev --prefix src/VoidEmpires.Frontend`
- open `/shipyard`, then `/defenses`, then `/fleets`, then `/planet` with the deterministic seeded ids
- treat that browser sequence as a user-driven validation pass; this document does not claim it has already been executed

8. Re-read Fleet state after the Shipyard mutation:

```powershell
.\scripts\dev-qa-fleet-read-state.ps1
```

Expected success:

- current group counts, stationed counts, active-transfer counts, and resource-context summaries print
- the helper stays read-only and does not create movement, transfer, split, merge, or combat state

Expected controlled no-op:

- if no groups are currently visible, the helper explains that the read-state is still valid and no mutation was attempted

Important reminders:

- These scripts create real Development database rows when you run the Construction or Research helpers.
- The `/research` confirmation flow also creates a real Development database row when the backend accepts the order.
- The Shipyard helper also creates a real Development database row when enqueue succeeds.
- Repeated runs may find the queue already occupied and should now report that clearly.
- Fleet read-state is accepted only as a post-Shipyard read. Stock-to-fleet allocation, new movement, split, merge, combat, and due-processing stay out of this default loop.
- The scripts do not delete, cancel, or complete existing orders automatically.
- Do not paste printed console output back into PowerShell as if it were a command.

## Accepted QA checklist for this block

- Use documented seed profiles, not manual SQL.
- Read from the cockpit-backed endpoint before mutating.
- Enqueue only actions already marked available by the read model.
- Refresh from the authoritative read endpoint after mutation.
- Expect local planet resources to drop immediately after a successful Construction or Research enqueue.
- Treat any global completion path as out of scope for repeatable cockpit QA.
- Keep the flow Development-only and backend-authoritative.

