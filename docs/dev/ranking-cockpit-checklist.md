# Ranking Cockpit Checklist

Use this document as the source of truth for `Ranking v1` scope before any frontend route, API, or seed work lands.
This task is documentation-first: it defines what the repository can already support safely and what must stay out of scope.

## Safe v1 scope summary

Accepted safe scope for `Ranking v1`:

- own-civilization power summary derived from existing read-only cockpit data
- category breakdown sourced from already persisted ownership, economy, research, fleet, defense, ground-army, market, espionage, and alliance metadata
- deterministic demo comparison rows only when they are clearly development-only and non-public
- disabled future leaderboard or competitive actions shown only as placeholders
- no global ladder, no public profile, no rewards, no matchmaking, no write model, and no ranking worker

Required wording posture:

- `Ranking` must read as an internal `power index` or `readiness index`, not as a published leaderboard
- `comparison` must read as seeded or demo-only context, not live player competition
- `future leaderboard` copy must stay explicitly disabled

## Current backend reality

Current backend surfaces already provide enough read-state to support a conservative ranking cockpit:

- civilization identity and homeworld context from [src/VoidEmpires.Domain/Players/Civilization.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Players/Civilization.cs) and [src/VoidEmpires.Domain/Players/PlayerProfile.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Players/PlayerProfile.cs)
- owned-planet, stockpile, production, building, and construction state through [src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Planets/DevPlanetUiStateService.cs)
- research queue and completed-project state through [src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs), [src/VoidEmpires.Domain/Research/ResearchProject.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Research/ResearchProject.cs), and [src/VoidEmpires.Domain/Research/ResearchOrder.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Research/ResearchOrder.cs)
- orbital production, stock, and readiness through [src/VoidEmpires.Infrastructure/Assets/DevShipyardUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Assets/DevShipyardUiStateService.cs)
- fleet, transfer, route, and interception-readiness metadata through [src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs) and [src/VoidEmpires.Infrastructure/Fleets/FleetOperationalOverviewService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Fleets/FleetOperationalOverviewService.cs)
- defensive readiness through [src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Planets/DevDefenseUiStateService.cs)
- ground-army readiness through [src/VoidEmpires.Infrastructure/Planets/DevGroundArmyUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Planets/DevGroundArmyUiStateService.cs)
- market and logistics advisory state through [src/VoidEmpires.Infrastructure/Markets/DevMarketUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Markets/DevMarketUiStateService.cs)
- intelligence, visibility, and passive-signal metadata through [src/VoidEmpires.Infrastructure/StrategicMap/DevEspionageUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/DevEspionageUiStateService.cs) and [src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs)
- alliance and contact metadata through [src/VoidEmpires.Infrastructure/StrategicMap/DevAllianceUiStateService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/DevAllianceUiStateService.cs)

Important current constraint:

- these are cockpit or development read models
- they are scoped to the requesting civilization
- they do not currently define a canonical `ranking`, `leaderboard`, or persisted `power score` concept anywhere in Domain, Application, Infrastructure, or Web

## Component discovery

Most relevant components for Ranking scope work:

- dev endpoint gating and composition root: [src/VoidEmpires.Web/Program.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/Program.cs)
- shared dev endpoint inventory: [src/VoidEmpires.Web/DevEndpointMappings.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/DevEndpointMappings.cs)
- DI wiring: [src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs)
- deterministic seed owner: [src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs)
- current accepted adjacent cockpit docs: [docs/dev/market-cockpit-checklist.md](/D:/Proyectos/VoidEmpires/docs/dev/market-cockpit-checklist.md), [docs/dev/alliance-cockpit-checklist.md](/D:/Proyectos/VoidEmpires/docs/dev/alliance-cockpit-checklist.md), and [docs/dev/frontend-foundation-smoke-checklist.md](/D:/Proyectos/VoidEmpires/docs/dev/frontend-foundation-smoke-checklist.md)

## Dependency map

Current safe read chain:

- `Program.cs` -> development gating -> `MapDevEndpointMappings` plus specialized cockpit read endpoints
- dev endpoints -> typed application contracts or direct infrastructure read services
- DI registrations -> [src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs)
- read services -> `VoidEmpiresDbContext` plus existing derived helpers such as strategic-map, fleet-overview, sensor, detection, and alliance-readiness services
- seed baseline -> `DevelopmentSeedService` creates deterministic owned planets, stockpiles, production, fleets, queues, contacts, and visibility rows used by the current cockpits

Implication for Ranking:

- `Ranking v1` should compose existing read-state
- it should not create a new simulation subsystem
- it should not add a worker, score table, or public endpoint family

## Metric-source audit

### Civilization identity

Safe sources:

- `Civilization.Id`, `Civilization.Name`, `Civilization.Archetype`, `Civilization.Status`, `Civilization.HomePlanetId`
- optional owner lineage from `PlayerProfile.DisplayName` for diagnostics only

Assessment:

- `Civilization` is the correct primary scope key for Ranking
- `PlayerProfile` should remain secondary and diagnostic

### Planet ownership

Safe sources:

- [src/VoidEmpires.Domain/Colonization/PlanetOwnership.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Colonization/PlanetOwnership.cs)
- planet identity and system placement from [src/VoidEmpires.Domain/Galaxy/Planet.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Galaxy/Planet.cs)
- current owned-planet projections already used by Planet, Market, and Shipyard read models

Assessment:

- this is safe for owned-colony count, homeworld context, and owned-versus-non-owned classification
- it is not a safe source for public territorial supremacy claims because visibility and diplomacy stay limited

### Resource reserves and production

Safe sources:

- [src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs)
- [src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Economy/PlanetProductionProfile.cs)
- Market aggregation already sums owned-planet reserves without introducing a shared treasury

Assessment:

- safe for own-civilization reserve totals and selected-planet production contribution
- not safe to describe as a real interstellar market valuation or empire-wide bank balance because the repository only persists planet stockpiles

### Colony development

Safe sources:

- [src/VoidEmpires.Domain/Buildings/PlanetBuilding.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Buildings/PlanetBuilding.cs)
- [src/VoidEmpires.Domain/Buildings/PlanetBuildingCapacity.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Buildings/PlanetBuildingCapacity.cs)
- [src/VoidEmpires.Domain/Buildings/PlanetConstructionOrder.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Buildings/PlanetConstructionOrder.cs)

Assessment:

- safe for structural maturity, capacity usage, and current build-readiness summaries
- current building taxonomy already covers civilian, industrial, research, defense, military-ground, military-space, and logistics categories

### Research progress

Safe sources:

- [src/VoidEmpires.Domain/Research/ResearchProject.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Research/ResearchProject.cs)
- [src/VoidEmpires.Domain/Research/ResearchOrder.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Research/ResearchOrder.cs)
- [src/VoidEmpires.Domain/Research/ResearchCatalog.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Research/ResearchCatalog.cs)

Assessment:

- safe for completed-level count, queue state, and category-weighted readiness
- current catalog already exposes useful military and intelligence-adjacent categories such as `Propulsion`, `ShipWeapons`, `Shielding`, and `Espionage`
- these remain internal research progress signals, not player-facing ranking achievements

### Shipyard and industrial stock

Safe sources:

- [src/VoidEmpires.Domain/Assets/OrbitalAssetStock.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Assets/OrbitalAssetStock.cs)
- [src/VoidEmpires.Domain/Assets/AssetProductionOrder.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Assets/AssetProductionOrder.cs)
- [src/VoidEmpires.Domain/Assets/OrbitalAssetCatalog.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Assets/OrbitalAssetCatalog.cs)

Assessment:

- safe for current orbital stock, available hull readiness, and industrial capability
- not safe for combat power claims beyond owned stock and local production readiness

### Fleet readiness and transport state

Safe sources:

- [src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs)
- [src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs)
- fleet overview and UI-state read models already expose stationed groups, active transfers, travel-readiness hints, and interception notes

Assessment:

- safe for own-fleet presence, movement readiness, and transfer pressure summaries
- not safe for live tactical strength, battle outcomes, or public comparisons across hidden foreign fleets

### Defensive readiness

Safe sources:

- defensive structure rows derived from Planet read-state
- `DefenseGrid` and defense-category construction readiness from the Defenses cockpit
- research adjacency from `Shielding` exists, but current Defenses explicitly says active mitigation is not modeled

Assessment:

- safe for structural defense readiness only
- not safe for shield simulation, bombardment resistance, or actual combat survivability claims

### Ground-army readiness

Safe sources:

- [src/VoidEmpires.Domain/Population/PlanetPopulationProfile.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Population/PlanetPopulationProfile.cs)
- [src/VoidEmpires.Domain/Assets/PlanetaryAssetStock.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Assets/PlanetaryAssetStock.cs)
- [src/VoidEmpires.Domain/Assets/PlanetaryAssetCatalog.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Assets/PlanetaryAssetCatalog.cs)

Assessment:

- safe for recruitable-capacity, stocked-unit, and training-readiness summaries
- not safe for invasion strength, assault power, or occupation success projections

### Intelligence and visibility context

Safe sources:

- strategic-map visibility, exploration knowledge, sensor profiles, detection coverage, transfer overlays, and espionage passive signals

Assessment:

- safe for confidence or intelligence-readiness modifiers inside the requesting civilization scope
- not safe for exposing hidden foreign state or implying omniscient surveillance

### Market and alliance read state

Safe sources:

- Market logistics summary, reserve aggregates, advisory ratios, and signals
- Alliance readiness, pact-readiness, and diplomatic-contact metadata

Assessment:

- safe as contextual category signals
- not safe as economic ranking authority, diplomatic prestige score, or public reputation system

## Existing naming audit

Search result summary:

- no current backend type, endpoint, or contract is named `Ranking`, `Leaderboard`, `Ladder`, or `PowerIndex`
- the strongest adjacent names are descriptive, not competitive: `FleetOperationalOverview`, `AllianceReadiness`, `DetectionCoverage`, `PlanetMilitaryCapacityCalculator`, and `StrategicMap`

Decision:

- later Ranking tasks should introduce presentation terminology at the cockpit/API layer only
- they should not reinterpret current domain names as if a canonical competitive scoring model already exists

## Recommended v1 boundary

Accepted Ranking v1 boundary:

- one requesting-civilization summary score or index derived at read time only
- category cards for economy, colonies, research, fleets, defenses, ground army, intelligence, market posture, and diplomacy posture
- optional demo-only comparison rows against deterministic seeded reference entries when the source is clearly labeled as development-only
- diagnostics may expose contributing counts or notes, but raw ids and formula details should stay secondary

Do not add in v1:

- public global ladder
- real cross-player ranking table
- persistent score history
- automatic recalculation worker
- rewards, leagues, seasons, or matchmaking
- public profile pages
- authorization changes
- mutation endpoints

## Seed and validation posture

Current seed reality:

- `cockpit-validation` already seeds the owned colony, reserves, production, queue history, fleet activity, one diplomatic contact, market-ready stockpiles, defense readiness, ground-army readiness, and strategic visibility needed for a deterministic Ranking read model
- no dedicated ranking seed rows exist yet, which is correct for this audit phase

Current test posture:

- existing adjacent tests cover the individual read foundations
- this task introduces no code-path changes, so no new tests are required here
- No integration tests configured.

## Decision summary

- The repository already has enough read-only state to support a safe internal `Ranking v1` cockpit.
- The correct model is a non-persistent read-time aggregation scoped to the requesting civilization.
- The repository does not yet support any truthful public ladder, competitive reward loop, or matchmaking interpretation.
- Later Ranking tasks should reuse current cockpit/service outputs, preserve dev-only gating, and keep every comparison explicitly non-public.
