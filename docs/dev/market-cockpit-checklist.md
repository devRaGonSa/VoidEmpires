# Market Cockpit Checklist

This note defines the safe backend scope for `Mercado v1`.
It is an audit-first contract for later Market tasks, not a claim that a real market backend already exists.

## Accepted boundary
- `/market` is not backed by a dedicated production market service or endpoint yet.
- Market v1 must stay read-only, development-safe, and honest about missing commerce mechanics.
- Market may summarize civilization and selected-planet economy context by reusing existing read models and persisted economy data.
- Market may show deterministic advisory ratios or placeholder price signals later, but only as derived guidance.
- Market must not buy, sell, transfer, auction, list, match orders, or execute trade routes.

## Backend inventory
### Resource taxonomy
Implemented canonical resource types:
- `Credits`
- `Metal`
- `Crystal`
- `Gas`
Primary components:
- `src/VoidEmpires.Domain/Economy/ResourceType.cs`
- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs`
### Planet reserves and production
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
### Cost and affordability foundations
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
### Queue and production state
Implemented now:
- Construction queue persistence in `PlanetConstructionOrder`.
- Research queue persistence in `ResearchOrder`.
- Asset production queue persistence in `AssetProductionOrder`.
- Orbital stock persistence in `OrbitalAssetStock`.
- Planetary unit stock persistence in `PlanetaryAssetStock`.
Primary components:
- `src/VoidEmpires.Domain/Buildings/PlanetConstructionOrder.cs`
- `src/VoidEmpires.Domain/Research/ResearchOrder.cs`
- `src/VoidEmpires.Domain/Assets/AssetProductionOrder.cs`
- `src/VoidEmpires.Domain/Assets/OrbitalAssetStock.cs`
- `src/VoidEmpires.Domain/Assets/PlanetaryAssetStock.cs`
### Fleet and logistics-adjacent signals
Implemented now:
- Civilizations can inspect orbital groups, active transfers, route profile metadata, travel cost previews, and placeholder fuel readiness.
- Fleet UI state already exposes current-planet resource contexts for fleet groups.
- Planet UI state already exposes orbital activity counts for stationed groups, departures, and arrivals.
Primary components:
- `src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimate.cs`
- `src/VoidEmpires.Infrastructure/Fleets/FleetOperationalOverviewService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs`
- `src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs`
- `src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs`
Current limit:
- These signals describe logistics readiness and travel cost pressure, not trade execution or resource shipping.
### Market or trade naming audit
Search result across `src/`, `tests/`, `docs/`, and `ai/`:
- No domain, application, infrastructure, or web component currently implements a dedicated `market` service, DTO, endpoint, or aggregate.
- No backend commerce terms such as order book, exchange, listing, buy order, sell order, or auction are implemented.
- `trade` references today are almost entirely limitation statements in docs and tasks, not executable backend behavior.
- `route` references today belong to orbital travel, visual overlays, and strategic-map readiness, not commercial routes.
## Current read surfaces
### Planet and Construction
Current read chain:
- `GET /api/dev/planet/ui-state`
- `IDevPlanetUiStateService` -> `DevPlanetUiStateService`
- `VoidEmpiresDbContext` -> `PlanetResourceStockpile`, `PlanetProductionProfile`, `PlanetBuilding`, `PlanetBuildingCapacity`, `PlanetConstructionOrder`, `OrbitalGroup`, `OrbitalTransfer`, `ResearchProject`
What it already exposes safely:
- selected owned-planet stockpile
- selected owned-planet production per hour
- construction costs and availability
- open and completed construction queue rows
- orbital activity counts
### Shipyard
Current read chain:
- `GET /api/dev/shipyard/ui-state`
- `IDevShipyardUiStateService` -> `DevShipyardUiStateService`
- `VoidEmpiresDbContext` -> `PlanetResourceStockpile`, `PlanetBuilding`, `PlanetPopulationProfile`, `AssetProductionOrder`, `OrbitalAssetStock`
What it already exposes safely:
- selected owned-planet stockpile
- orbital production catalog costs and requirements
- orbital production queue rows
- local orbital stock
### Research
Current read chain:
- `GET /api/dev/research/ui-state`
- endpoint-local query composition in `DevResearchUiStateEndpoints`
- `VoidEmpiresDbContext` -> `ResearchOrder`, `ResearchProject`, `PlanetResourceStockpile`
- readiness logic -> `ResearchEnqueueReadinessEvaluator`
What it already exposes safely:
- research costs
- queue status
- completed levels
- stockpile-based affordability hints
### Defenses
Current read chain:
- defense cockpit read -> `IDevDefenseUiStateService` -> `DevDefenseUiStateService`
- `DevDefenseUiStateService` -> `IDevPlanetUiStateService`

What it already exposes safely:
- defense-related construction options
- defense queue rows
- reused planet stockpile context
### Ground Army
Current read chain:
- ground-army cockpit read -> `IDevGroundArmyUiStateService` -> `DevGroundArmyUiStateService`
- `DevGroundArmyUiStateService` -> `IDevPlanetUiStateService` plus direct reads of `PlanetaryAssetStock` and `AssetProductionOrder`

What it already exposes safely:
- local ground-unit stock
- planetary production queue rows
- stockpile and capacity readiness
### Fleets
Current read chain:
- `GET /api/dev/fleets/overview`
- `IFleetOperationalOverviewService` -> `FleetOperationalOverviewService`
- `GET /api/dev/fleets/ui-state`
- `IDevFleetUiStateService` -> `DevFleetUiStateService` -> `IFleetOperationalOverviewService` and `IDevFleetActionManifestService`
- travel preview -> `POST /api/dev/fleets/orbital-travel/estimate`
- `IOrbitalTravelEstimateService` -> `OrbitalTravelEstimateService` -> `IResourceSpendService`, `IOrbitalRouteProfileService`, `IOrbitalFuelReadinessService`
What it already exposes safely:
- current-planet reserve context for fleet groups
- active transfer summaries
- travel-cost affordability previews
- route and logistics readiness metadata
## Dependency map
- Planet cockpit endpoint -> `IDevPlanetUiStateService` -> persisted stockpile, production, construction, research, and orbital activity tables.
- Construction enqueue endpoint -> `IPlanetConstructionQueueService` -> `BuildingCatalog` + `PlanetResourceStockpile` + `PlanetBuildingCapacity` + `ResearchProject`.
- Research UI state endpoint -> `ResearchEnqueueReadinessEvaluator` -> `ResearchCatalog` + selected-planet stockpile + research queue state.
- Shipyard cockpit endpoint -> `IDevShipyardUiStateService` -> persisted stockpile + buildings + population + asset orders + orbital stock.
- Asset production enqueue endpoint -> `IAssetProductionQueueService` -> asset catalogs + stockpile + buildings + population.
- Fleet UI state endpoint -> `IDevFleetUiStateService` -> `IFleetOperationalOverviewService` + current-planet stockpiles + optional interception-readiness metadata.
- Fleet travel estimate endpoint -> `IOrbitalTravelEstimateService` -> `IResourceSpendService` + route profile + fuel readiness + current-planet stockpile.
- Defense cockpit read -> `IDevDefenseUiStateService` -> `IDevPlanetUiStateService`.
- Ground Army cockpit read -> `IDevGroundArmyUiStateService` -> `IDevPlanetUiStateService` + planetary stock and asset-production rows.
- Development seeds -> `IDevelopmentSeedService` -> deterministic stockpiles, production profiles, buildings, queues, asset stock, groups, and transfers for development-only QA.
## Safe Market v1 scope
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
## Ownership boundaries
- `Planet` remains the source for local colony reserves, production, building capacity, and orbital-activity context.
- `Construction` remains the source for building queue management, building costs, and guarded construction enqueue.
- `Research` remains the source for technology queue state, technology costs, and research readiness.
- `Shipyard` remains the source for orbital production queue state, local orbital stock, and orbital build requirements.
- `Defenses` remains the source for defense-specific construction readiness.
- `Ground Army` remains the source for local troop stock and terrestrial production readiness.
- `Fleets` remains the source for orbital groups, transfers, route previews, and travel-cost logistics hints.
- `Galaxy` remains read-only strategic context and must not be duplicated as a market execution surface.
## Recommended backend approach for later tasks
- Prefer one dedicated development read model for Market rather than reconstructing the cockpit in the frontend from Planet, Research, Shipyard, Ground Army, and Fleet payloads.
- That read model should reuse existing persisted economy and logistics concepts instead of inventing new commerce entities.
- If advisory ratios or reference prices are introduced, derive them deterministically from existing costs, stockpiles, production, or seeded heuristics and mark them as advisory only.
- If a civilization summary is needed, aggregate existing planet stockpiles at query time rather than introducing a new mutable civilization reserve table in this block.
## Decision summary
- The repository already has enough read-only economy and logistics foundations to support a truthful Market cockpit.
- The repository does not have a real market subsystem.
- The correct `Mercado v1` scope is an economy-analysis cockpit assembled from existing reserves, production, cost, affordability, and logistics-readiness data.
- All transaction gameplay, commerce execution, and resource movement must remain explicitly disabled in this phase.
