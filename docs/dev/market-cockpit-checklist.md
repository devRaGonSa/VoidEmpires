# Market Cockpit Checklist

Market cockpit v1 is accepted as a read-only economy cockpit foundation for the current development build.
Use `docs/dev/development-seed-profiles.md` for deterministic QA setup and `docs/dev/planet-module-boundaries.md` to keep Market separate from Planet, Construction, Shipyard, Fleets, and Galaxy ownership boundaries.

## Acceptance boundary

- `/market` is a real route.
- `civilizationId` is required and `planetId` remains optional.
- The page stays development-safe and read-only.
- Market may expose reserves, production, advisory ratios, trade signals, disabled future operations, and cross-cockpit handoffs.
- Market must not buy, sell, transfer, auction, mutate resources, or execute trade routes.
- Diagnostics stay collapsed or clearly secondary.

## Seeded QA scenario

Use `cockpit-validation` for the richer deterministic Market QA baseline:

- Civilization id: `00000000-0000-0000-0000-000000000001`
- Owned planet id: `40000000-0000-0000-0000-000000000001`
- Owned planet name: `Aurelia`
- Solar system: `Helios Gate`
- Deterministic QA URL:
  `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Expected comparison baseline:
  - civilization and selected-planet reserves are non-zero
  - selected-planet production is visible
  - advisory reference ratios are present
  - at least one economy signal plus one future-route signal are visible
  - future Market actions remain disabled placeholders

## Final manual QA

Run first:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

If integration checks are reviewed for this cockpit block, record:

`No integration tests configured.`

Then apply the richer shared seed twice:

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

Then confirm on `/market`:

- The deterministic seeded scenario can open as `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- The first viewport reads like an economy cockpit, not a transaction console or shell-only placeholder.
- The page loads with Spanish loading, missing-context, and failure states.
- Civilization and selected-planet context are visible.
- Resource summary and reserve posture are visible.
- Production or economy summary is visible when the seeded profile is applied.
- Reference prices or ratios are visible as advisory-only data.
- Trade signals and future-route placeholders are visible as secondary context.
- Future Market actions stay disabled and visually secondary.
- Handoff links to Planet, Construction, Shipyard, Fleets, and Galaxy preserve context.
- Diagnostics stay collapsed by default.
- Buying is not available.
- Selling is not available.
- Resource mutation is not available.
- Trade-route execution is not available.
- Raw backend wording does not dominate the first viewport.

## Intentional exclusions

- No buying.
- No selling.
- No player-to-player trading.
- No auctions.
- No resource mutation.
- No trade-route execution.
- No WebSockets.
- No production authentication.
- No hidden Fleet or Galaxy mutation through Market handoffs.

## Backend scope audit

This section keeps the original backend-scope contract that guided Market v1 implementation.

### Accepted boundary

- `/market` is not backed by a dedicated production market service or endpoint.
- Market v1 stays read-only, development-safe, and honest about missing commerce mechanics.
- Market may summarize civilization and selected-planet economy context by reusing existing read models and persisted economy data.
- Market may show deterministic advisory ratios or placeholder price signals only as derived guidance.
- Market must not buy, sell, transfer, auction, list, match orders, or execute trade routes.

### Backend inventory

#### Resource taxonomy

Implemented canonical resource types:

- `Credits`
- `Metal`
- `Crystal`
- `Gas`

Primary components:

- `src/VoidEmpires.Domain/Economy/ResourceType.cs`
- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs`

#### Planet reserves and production

Implemented now:

- Per-planet persisted stockpiles in `PlanetResourceStockpile`.
- Per-planet persisted hourly production in `PlanetProductionProfile`.
- Production application through `ResourceProductionService` and `PlanetEconomyTickService`.
- Research-based production multiplier through `ResearchBonusCalculator`.

Primary components:

- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs`
- `src/VoidEmpires.Domain/Economy/ResourceProductionService.cs`
- `src/VoidEmpires.Infrastructure/Economy/PlanetEconomyTickService.cs`
- `src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs`

Current limit:

- No civilization-level treasury or shared reserve aggregate is persisted today. The real reserve source is planet-scoped stockpile data.

#### Cost and affordability foundations

Implemented now:

- Construction costs from `BuildingCatalog`.
- Research costs from `ResearchCatalog`.
- Orbital and planetary asset costs from `OrbitalAssetCatalog` and `PlanetaryAssetCatalog`.
- Fleet travel costs from `OrbitalTravelEstimator`.
- Shared affordability and spend validation through `PlanetResourceStockpile` and `ResourceSpendService`.

Primary components:

- `src/VoidEmpires.Infrastructure/Buildings/PlanetConstructionQueueService.cs`
- `src/VoidEmpires.Infrastructure/Research/ResearchEnqueueReadinessEvaluator.cs`
- `src/VoidEmpires.Infrastructure/Assets/AssetProductionQueueService.cs`
- `src/VoidEmpires.Infrastructure/Economy/ResourceSpendService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs`

#### Queue and production state

Implemented now:

- Construction queue persistence in `PlanetConstructionOrder`.
- Research queue persistence in `ResearchOrder`.
- Asset production queue persistence in `AssetProductionOrder`.
- Orbital stock persistence in `OrbitalAssetStock`.
- Planetary unit stock persistence in `PlanetaryAssetStock`.

#### Fleet and logistics-adjacent signals

Implemented now:

- Civilizations can inspect orbital groups, active transfers, route profile metadata, travel cost previews, and placeholder fuel readiness.
- Fleet UI state already exposes current-planet resource contexts for fleet groups.
- Planet UI state already exposes orbital activity counts for stationed groups, departures, and arrivals.

Current limit:

- These signals describe logistics readiness and travel cost pressure, not trade execution or resource shipping.

### Safe Market v1 scope

Market v1 may safely do the following:

- read civilization context
- read selected planet context when `planetId` is provided
- summarize planet-scoped reserves for `Credits`, `Metal`, `Crystal`, and `Gas`
- summarize selected-planet production per hour when a production profile exists
- summarize resource pressure implied by construction, research, shipyard, and ground-army costs
- summarize logistics pressure implied by fleet travel-cost previews and active transfer context
- derive deterministic advisory ratios or reference prices from existing static costs or seeded economy rules
- show future trade-route, listing, import, export, and exchange actions only as disabled placeholders
- expose diagnostics that clarify when values are advisory, development-only, selected-planet-only, or unavailable

Market v1 must not do the following:

- mutate stockpiles
- spend resources
- create or complete fleet transfers
- create resource shipments
- create buy or sell orders
- create listings or auctions
- execute player-to-player trades
- imply that civilization-wide reserves already exist as a backend truth when only planet stockpiles are persisted

### Ownership boundaries

- `Planet` remains the source for local colony reserves, production, building capacity, and orbital-activity context.
- `Construction` remains the source for building queue management, building costs, and guarded construction enqueue.
- `Research` remains the source for technology queue state, technology costs, and research readiness.
- `Shipyard` remains the source for orbital production queue state, local orbital stock, and orbital build requirements.
- `Defenses` remains the source for defense-specific construction readiness.
- `Ground Army` remains the source for local troop stock and terrestrial production readiness.
- `Fleets` remains the source for orbital groups, transfers, route previews, and travel-cost logistics hints.
- `Galaxy` remains read-only strategic context and must not be duplicated as a market execution surface.

### Decision summary

- The repository already has enough read-only economy and logistics foundations to support a truthful Market cockpit.
- The repository does not have a real market subsystem.
- The correct `Mercado v1` scope is an economy-analysis cockpit assembled from existing reserves, production, cost, affordability, and logistics-readiness data.
- All transaction gameplay, commerce execution, and resource movement remain explicitly disabled in this phase.
