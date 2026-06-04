# Persisted Gameplay Flow Checklist

This document is the authoritative QA scope note for the current persisted `Construction` and `Research` development flows.
Use it together with `docs/dev/development-seed-profiles.md`, `docs/dev/construction-cockpit-checklist.md`, and `docs/dev/research-cockpit-checklist.md`.

## Scope and safety rules

- Development-only flow.
- No production auth work is required for this block.
- No manual SQL is part of the accepted QA path.
- No destructive reset behavior is part of the accepted QA path.
- No automatic completion is part of the accepted QA path unless a route is already proven safe and scoped.
- No visual QA is required for this block.

## Safe baseline

Recommended deterministic setup:

1. Apply `planet-full-validation` before Construction QA.
2. Apply `research-validation` before Research QA.
3. Use a fresh disposable local database only when an exact pre-enqueue baseline is required.
4. Reapply the documented seed when local state becomes confusing, but do not rely on reseeding to clear open orders.

Repeatable backend-only baseline helper:

- Run `.\scripts\dev-qa-baseline.ps1` to verify the seed catalog, apply `cockpit-validation` twice, and print the current Construction/Planet and Research baseline snapshot before creating any real orders.

Primary QA URLs:

- Construction: `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Research: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`

## Endpoint inventory

Construction persisted mutation and read surfaces: `POST /api/dev/buildings/construction-orders/enqueue`, `POST /api/dev/buildings/construction-orders/complete-due`, `GET /api/dev/planets/ui-state?civilizationId={id}&planetId={id}`

Research persisted mutation and read surfaces: `POST /api/dev/research/orders/enqueue`, `POST /api/dev/research/orders/complete-due`, `GET /api/dev/research/ui-state?civilizationId={id}&planetId={id}`

Seed/bootstrap surfaces used by the safe QA loop: `POST /api/dev/seeds/apply`, `GET /api/dev/seeds/profiles`

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

## Accepted QA checklist for this block

- Use documented seed profiles, not manual SQL.
- Read from the cockpit-backed endpoint before mutating.
- Enqueue only actions already marked available by the read model.
- Refresh from the authoritative read endpoint after mutation.
- Expect local planet resources to drop immediately after a successful Construction or Research enqueue.
- Treat any global completion path as out of scope for repeatable cockpit QA.
- Keep the flow Development-only and backend-authoritative.
