# Current State

## Phase

The repository is in `Phase 4C - Asset requirement foundation` while retaining the AI Platform workflow assets from Phase 0.

## Repository Reality

The AI Platform template has been adapted into a VoidEmpires-specific project workspace.

Current repository contents are centered on:

- workflow rules in `AGENTS.md`
- planning and orchestration documents under `ai/`
- task lifecycle folders under `ai/tasks/`
- helper scripts under `scripts/`
- the `.NET` solution and projects under `src/` and `tests/`

## Application Status

The repository contains `VoidEmpires.sln` with these projects:

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Domain`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

`VoidEmpires.Web` is a minimal ASP.NET Core host. It exposes:

- `GET /` for a simple product identity response
- `GET /health` for deterministic health checks, including whether persistence is configured
- `POST /api/auth/register` for minimal user registration
- `GET /api/auth/confirm-email` for minimal email confirmation
- `POST /api/dev/galaxies/generate` for controlled development/test galaxy generation when development endpoints and persistence are configured
- `POST /api/dev/players/starting-civilization` for controlled development/test player profile and starting civilization creation when development endpoints and persistence are configured
- `POST /api/dev/buildings/construction-orders/enqueue` for controlled construction queue enqueue testing when development endpoints and persistence are configured
- `POST /api/dev/buildings/construction-orders/complete-due` for controlled construction queue completion testing when development endpoints and persistence are configured

The repository now has:

- PostgreSQL 16 selected as the primary relational database engine.
- EF Core with Npgsql package references in `VoidEmpires.Infrastructure`.
- An empty `ConnectionStrings:DefaultConnection` placeholder in web appsettings files.
- A `VoidEmpiresDbContext` in the Infrastructure persistence boundary using ASP.NET Core Identity tables.
- EF Core migrations for Identity, initial galaxy model, player/civilization model, planet ownership model, planet economy model, planet building model, research model, construction queue model, research queue model, and planet population model. Migrations exist in source but are not automatically applied to the real database.
- Infrastructure service registration that enables PostgreSQL only when a non-empty connection string is configured.
- ASP.NET Core Identity registration with unique-email and confirmed-email defaults.
- Application contracts for user registration, email confirmation, and transactional email.
- Infrastructure services for registration and email confirmation backed by ASP.NET Core Identity.
- Brevo transactional email sender wiring behind the provider-agnostic email contract.
- A disabled, placeholder-only `Brevo` configuration section for transactional email integration.
- Deterministic in-memory galaxy generation through `IGalaxyGenerator`, registered in Infrastructure dependency injection.
- Persisted galaxy generation through `IGalaxyGenerationService`.
- Player gameplay identity through `PlayerProfile`.
- Starting civilization model through `Civilization`, `CivilizationArchetype`, and `CivilizationStatus`.
- Starting civilization creation through `IStartingCivilizationService`.
- Planet control model through `PlanetOwnership` and `PlanetControlStatus`.
- Planet colonization/control creation through `IPlanetColonizationService`.
- Planet economy domain through `ResourceType`, `PlanetResourceStockpile`, `PlanetProductionProfile`, and `ResourceProductionService`.
- Persisted planet economy tick through `IPlanetEconomyTickService`.
- Planet building model through `BuildingType`, `PlanetBuilding`, `PlanetBuildingCapacity`, `BuildingDefinition`, `BuildingCatalog`, and `ConstructionCost`.
- Building construction through `IPlanetBuildingConstructionService`, including capacity checks and resource spending.
- Building upgrades through `IPlanetBuildingUpgradeService`, including resource spending and level increment.
- Building role classification through `BuildingCategory` and categorized `BuildingDefinition` entries.
- Expanded building catalog with population, military ground, military space, and logistics building types.
- Construction queue foundation through `PlanetConstructionOrder`, `ConstructionQueueItemAction`, `ConstructionQueueItemStatus`, and `IPlanetConstructionQueueService`.
- Construction orders can enqueue building construction or upgrade work, spend resources, store start/end timestamps, and enforce one open order per planet.
- Construction order completion through `IConstructionOrderCompletionService`, which can explicitly complete due orders.
- Construction queue background worker foundation through `ConstructionQueueWorker`, disabled by default and controlled by configuration.
- Development-only construction queue endpoints for manual HTTP validation without introducing gameplay UI.
- Research queue foundation through `ResearchOrder`, `ResearchQueueItemStatus`, `IResearchQueueService`, and `IResearchOrderCompletionService`.
- Research orders can enqueue research upgrades, spend resources, store start/end timestamps, enforce one open research order per civilization, and complete due research explicitly.
- Planet population foundation through `PlanetPopulationProfile`.
- Military capacity foundation through `PlanetMilitaryCapacityCalculator` for local ground recruitment and locally built ship crew capacity.
- Asset requirement foundation through `PlanetaryAssetType`, `SpaceAssetType`, `AssetRequirement`, `PlanetaryAssetDefinition`, `OrbitalAssetDefinition`, `PlanetaryAssetCatalog`, and `OrbitalAssetCatalog`.
- Asset definitions can express local population capacity requirements, local operator capacity requirements, required building type, required building level, resource cost, storage capacity, and operating range.

Current gameplay foundation supports this backend chain:

```text
Identity user id -> PlayerProfile -> Civilization -> PlanetOwnership -> Planet -> Economy -> Buildings -> Construction queue -> Research queue -> Population and military capacity foundation -> Asset requirement foundation
```

## Building Capacity Design Note

The project has accepted this rule:

- each planet has a finite building capacity
- each building consumes a configurable amount of that capacity
- some buildings consume more capacity than others
- civilization archetype may affect usable building capacity or building footprint later
- future technologies may increase available construction capacity or reduce building footprint later
- buildings must not be treated as unlimited per planet

## Building Role Design Note

Buildings are now categorized by gameplay role so future systems can reason about population, economy, research, military, defense, and logistics separately.

Current categories:

- `Civilian`
- `Industrial`
- `Research`
- `MilitaryGround`
- `MilitarySpace`
- `Defense`
- `Logistics`

Current expanded building types include:

- `HabitationDistrict`
- `MedicalCenter`
- `MilitaryAcademy`
- `Barracks`
- `CrewAcademy`
- `FleetCommandCenter`
- `LogisticsHub`

## Population and Military Capacity Design Note

The project has accepted this rule:

- population limits what a planet can generate, recruit, train, or crew locally
- population does not limit what can be parked or stationed on that planet if it was produced elsewhere
- ground force capacity represents the local ability to recruit/train ground forces
- ship crew capacity represents the local ability to crew locally built ships
- parked foreign or transferred ships should be handled by later fleet ownership/origin systems, not by the planet population profile itself

Current population model:

- `PlanetPopulationProfile.TotalPopulation`
- `PlanetPopulationProfile.BaseRecruitablePopulation`
- `PlanetPopulationProfile.BaseCrewCapacity`

Current capacity calculator:

- `PlanetMilitaryCapacityCalculator.CalculateGroundForceCapacity(...)`
- `PlanetMilitaryCapacityCalculator.CalculateShipCrewCapacity(...)`

## Asset Requirement Design Note

The asset requirement foundation intentionally defines templates only. It does not create player-owned units, ships, fleets, combat entities, movement, or training/construction queues yet.

Accepted current rules:

- planetary assets use local population capacity requirements
- orbital assets use local operator/crew capacity requirements
- definitions can require a specific building type and minimum building level
- definitions include resource costs
- orbital definitions can include storage capacity and operating range
- this layer is a validation foundation for later production/recruitment systems

Current asset catalogs:

- `PlanetaryAssetCatalog`
- `OrbitalAssetCatalog`

Current asset types:

- `PlanetaryAssetType.PatrolGroup`
- `PlanetaryAssetType.ExpeditionGroup`
- `PlanetaryAssetType.VehicleGroup`
- `PlanetaryAssetType.SupportGroup`
- `SpaceAssetType.ScoutCraft`
- `SpaceAssetType.CargoCraft`
- `SpaceAssetType.EscortCraft`
- `SpaceAssetType.ColonyCraft`

## Construction Queue Design Note

The construction queue now supports explicit completion of due orders, an optional background worker that can trigger completion periodically when enabled by configuration, and development-only endpoints for controlled manual validation.

Accepted current rules:

- each planet can have at most one open construction order at this stage
- enqueueing construction spends resources immediately
- enqueueing an upgrade spends resources immediately but does not increase the building level yet
- enqueueing a new building validates current capacity but does not create the final `PlanetBuilding` yet
- order duration is calculated through the existing construction duration calculation model
- due orders can be completed explicitly through `IConstructionOrderCompletionService`
- completing a construction order creates the final `PlanetBuilding`
- completing an upgrade order raises the existing `PlanetBuilding` level to the queued target level
- the background worker is disabled by default
- the background worker is only registered when persistence is configured and `VoidEmpires:ConstructionQueueWorker:Enabled` is `true`
- the background worker delegates completion to `IConstructionOrderCompletionService`
- construction queue HTTP endpoints are development-only and guarded by the existing development endpoint switch

## Research Queue Design Note

The research queue now supports time-based research progression with explicit completion.

Accepted current rules:

- each civilization can have at most one open research order at this stage
- enqueueing research spends resources immediately
- enqueueing research does not immediately increase the `ResearchProject` level
- order duration is calculated through `ResearchDurationCalculator`
- due orders can be completed explicitly through `IResearchOrderCompletionService`
- completing a research order creates `ResearchProject` if it does not exist
- completing a research order raises the existing `ResearchProject` level to the queued target level
- no research background worker exists yet
- no research development endpoints exist yet

Current intentional exclusions:

- no player-owned unit instances
- no player-owned ship instances
- no fleets
- no combat
- no alliances
- no espionage gameplay
- no login/session/JWT endpoints
- no production deployment definition
- no UI gameplay client

PostgreSQL remains the persistence target. Real database and Brevo configuration are external to the repository and must not be committed. Brevo is the transactional email provider for user creation and email confirmation, but secrets such as API keys and sender credentials must come from environment variables, user secrets, deployment secrets, or private infrastructure configuration. CI and tests run without requiring the real NAS PostgreSQL database, private network access, or Brevo network calls.

## Task Workflow Status

The repository is actively using the AI task lifecycle:

- `ai/tasks/pending`
- `ai/tasks/in-progress`
- `ai/tasks/review`
- `ai/tasks/done`
- `ai/tasks/blocked`
- `ai/tasks/obsolete`

Inherited template history has been moved out of `ai/tasks/done` into `ai/tasks/obsolete` so future project tracking reflects VoidEmpires work only.

## Planning Status

The repository has established:

- a VoidEmpires-specific repository context
- an initial roadmap
- an initial architecture index
- the first bootstrap implementation plan
- the initial `VoidEmpires` solution structure
- persisted galaxy generation foundation
- player/civilization foundation
- planet ownership and colonization foundation
- planet economy foundation
- planet buildings foundation
- construction queue foundation
- construction queue completion foundation
- construction queue background worker foundation
- construction queue development endpoint foundation
- research queue foundation
- population and building role foundation
- asset requirement foundation

## Validation Status

Repository-specific application validation exists through the .NET solution.

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validation baseline: `174` passing tests.

Current tests include assembly-boundary coverage, smoke checks for `/` and `/health`, auth endpoint tests with fake services, development galaxy endpoint tests with fake services, development construction queue endpoint tests with fake services, persistence and identity registration checks, application contract tests, deterministic galaxy generation tests, persisted galaxy generation service tests with EF Core InMemory, player/civilization domain tests, starting civilization service tests, planet ownership domain tests, planet colonization service tests, planet economy domain tests, persisted planet economy tick tests, planet building domain tests, building catalog tests, building category tests, asset catalog tests, planet population profile tests, planet military capacity calculator tests, persisted building construction tests, persisted building upgrade tests, construction queue service tests, construction order completion service tests, construction queue worker options and registration tests, research duration tests, research queue service tests, research order completion service tests, registration and email confirmation service tests with EF Core InMemory, Brevo sender tests with fake HTTP handlers, and verification that health output does not expose connection string values. Tests do not use the real NAS PostgreSQL database.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Constraints

Current constraints remain:

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, player-owned units, player-owned ships, fleets, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
