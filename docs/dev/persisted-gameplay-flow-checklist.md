# Persisted Gameplay Flow Checklist

This document is the authoritative QA scope note for the current persisted `Construction`, `Research`, `Shipyard`, and `Fleet read-state` development flows.
Use it together with `docs/dev/development-seed-profiles.md`, `docs/dev/construction-cockpit-checklist.md`, `docs/dev/research-cockpit-checklist.md`, and `docs/dev/shipyard-fleet-persisted-qa.md`.

## Scope and safety rules

- Development-only flow.
- No production auth work is required for this block.
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
- There is intentionally no `dev-qa-create-orbital-group-from-stock.ps1` helper in this block because stock-to-fleet allocation is still excluded from the accepted reused-database QA loop.

Construction now has two sanctioned Development QA paths:

- Backend-only helper path:
  - `.\scripts\dev-qa-create-construction-order.ps1 -ApplySeed`
  - Useful when you want copy-pasteable queue and resource deltas directly in the terminal
- Frontend-confirmed cockpit path:
  - `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
  - Useful when you need to verify the guarded review, confirmation, Spanish copy, and post-submit refresh behavior
- Both paths create real Development database rows through the same persisted enqueue endpoint.

Shared helper defaults:

- Base URL default: `http://localhost:5142`
- Default civilization id: `00000000-0000-0000-0000-000000000001`
- Default owned planet id: `40000000-0000-0000-0000-000000000001`
- If the backend is not running or persistence is unavailable, the helpers fail clearly and do not attempt cleanup or reseed resets.

Shipyard/Fleet companion note:

- Use `docs/dev/shipyard-fleet-persisted-qa.md` for the exact endpoint inventory, read-model fields, and current inclusion or exclusion rules for Shipyard and Fleets.
- The companion note now also carries the copy-pasteable backend-only runbook for `baseline -> shipyard enqueue -> fleet read-state` using the final helper script names.

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

Construction persisted mutation and read surfaces: `POST /api/dev/buildings/construction-orders/enqueue`, `POST /api/dev/buildings/construction-orders/complete-due`, `GET /api/dev/planets/ui-state?civilizationId={id}&planetId={id}`

Research persisted mutation and read surfaces: `POST /api/dev/research/orders/enqueue`, `POST /api/dev/research/orders/complete-due`, `GET /api/dev/research/ui-state?civilizationId={id}&planetId={id}`

Seed/bootstrap surfaces used by the safe QA loop: `POST /api/dev/seeds/apply`, `GET /api/dev/seeds/profiles`

Shipyard/Fleet persisted mutation and read surfaces for the current block: `POST /api/dev/assets/production/enqueue`, `POST /api/dev/assets/production/process-due`, `GET /api/dev/shipyard/ui-state?civilizationId={id}&planetId={id}`, `GET /api/dev/fleets/ui-state?civilizationId={id}`, `GET /api/dev/fleets/overview?civilizationId={id}`, `GET /api/dev/fleets/action-manifest`, `POST /api/dev/fleets/orbital-groups/create-from-stock`

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

- `.\scripts\dev-qa-create-research-order.ps1`
- Defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
- Add `-ApplySeed` to apply `research-validation` before the enqueue.
- Add `-ResearchType PlanetaryEngineering` or another available type to force the exact enqueue target instead of auto-picking the first available hint.
- The helper reads Research command metadata from `/api/dev/research/ui-state`, posts the real enqueue request, then re-reads Research and Planet state to print queue plus resource changes without running migrations or cleanup.

Current enqueue behavior:

- Requires `civilizationId`, `sourcePlanetId`, `researchType`, and UTC `requestedAtUtc`.
- Accepts frontend string enum payloads such as `PlanetaryEngineering`.
- Requires the source planet to be actively owned by the requesting civilization.
- Rejects a second open research order for the civilization.
- Spends the full research cost immediately from the source planet stockpile.
- Persists a `ResearchOrder` row immediately with `Status = Active`.
- Uses current `EnergySystems` project level to shorten duration.
- Does not create or upgrade `ResearchProject` until completion.

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
