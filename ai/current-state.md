# Current State

## Phase

The repository is in `Phase 4G - Research development endpoints` while retaining the AI Platform workflow assets from Phase 0.

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
- `POST /api/dev/assets/production/enqueue` for controlled asset production queue testing when development endpoints and persistence are configured
- `POST /api/dev/assets/production/process-due` for controlled asset production processing when development endpoints and persistence are configured
- `POST /api/dev/research/orders/enqueue` for controlled research queue testing when development endpoints and persistence are configured
- `POST /api/dev/research/orders/complete-due` for controlled research order completion testing when development endpoints and persistence are configured

The repository now has:

- PostgreSQL 16 selected as the primary relational database engine.
- EF Core with Npgsql package references in `VoidEmpires.Infrastructure`.
- An empty `ConnectionStrings:DefaultConnection` placeholder in web appsettings files.
- A `VoidEmpiresDbContext` in the Infrastructure persistence boundary using ASP.NET Core Identity tables.
- EF Core migrations for Identity, galaxy, player/civilization, planet ownership, planet economy, planet building, research, construction queue, research queue, planet population, asset production order, and asset stock models. Migrations exist in source but are not automatically applied to the real database.
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
- Construction order completion through `IConstructionOrderCompletionService`, which can explicitly complete due orders.
- Construction queue background worker foundation through `ConstructionQueueWorker`, disabled by default and controlled by configuration.
- Development-only construction queue endpoints for manual HTTP validation without introducing gameplay UI.
- Research queue foundation through `ResearchOrder`, `ResearchQueueItemStatus`, `IResearchQueueService`, and `IResearchOrderCompletionService`.
- Research orders can enqueue research upgrades, spend resources, store start/end timestamps, enforce one open research order per civilization, and complete due research explicitly.
- Research development endpoints for HTTP validation of enqueueing and completing due research orders.
- Planet population foundation through `PlanetPopulationProfile`.
- Military capacity foundation through `PlanetMilitaryCapacityCalculator` for local ground recruitment and locally built ship crew capacity.
- Asset requirement foundation through `PlanetaryAssetType`, `SpaceAssetType`, `AssetRequirement`, `PlanetaryAssetDefinition`, `OrbitalAssetDefinition`, `PlanetaryAssetCatalog`, and `OrbitalAssetCatalog`.
- Asset production queue foundation through `AssetProductionOrder`, `AssetProductionTarget`, `AssetProductionOrderStatus`, `IAssetProductionQueueService`, and `IAssetOrderProcessor`.
- Asset inventory foundation through `PlanetaryAssetStock` and `OrbitalAssetStock`.
- Asset production development endpoints for HTTP validation of enqueueing and processing due asset production orders.
- Asset production can validate resources, required building, local population/operator capacity, enqueue a timed order, spend resources, process due orders, and write persistent asset stock.

Current gameplay foundation supports this backend chain:

```text
Identity user id -> PlayerProfile -> Civilization -> PlanetOwnership -> Planet -> Economy -> Buildings -> Construction queue -> Research queue -> Research HTTP validation -> Population and military capacity foundation -> Asset requirement foundation -> Asset production queue foundation -> Asset inventory foundation -> Asset production HTTP validation
```

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

The asset requirement foundation intentionally defines templates only. It does not create fleets, combat entities, movement, or final deployment behavior yet.

Accepted current rules:

- planetary assets use local population capacity requirements
- orbital assets use local operator/crew capacity requirements
- definitions can require a specific building type and minimum building level
- definitions include resource costs
- orbital definitions can include storage capacity and operating range
- this layer is a validation foundation for later production/recruitment systems

## Asset Production and Inventory Design Note

The asset production queue now supports timed production orders, persistent local stock creation, and development-only HTTP validation.

Accepted current rules:

- each planet can have at most one open asset production order at this stage
- enqueueing asset production spends resources immediately
- enqueueing validates required building type and level
- enqueueing validates local population capacity for planetary assets
- enqueueing validates local operator/crew capacity for orbital assets
- enqueueing creates a timed `AssetProductionOrder`
- due asset production orders can be processed explicitly through `IAssetOrderProcessor`
- processing due planetary asset orders creates or increments `PlanetaryAssetStock`
- processing due orbital asset orders creates or increments `OrbitalAssetStock`
- processing due orders then marks them as completed
- asset production HTTP endpoints are development-only and guarded by the existing development endpoint switch

Current intentional limitation:

- stock is planet-local only; no fleets, transfers, movement, deployment, stationed assets, or combat behavior exists yet

## Construction Queue Design Note

The construction queue supports explicit completion of due orders, an optional background worker that can trigger completion periodically when enabled by configuration, and development-only endpoints for controlled manual validation.

Accepted current rules:

- each planet can have at most one open construction order at this stage
- enqueueing construction spends resources immediately
- enqueueing an upgrade spends resources immediately but does not increase the building level yet
- enqueueing a new building validates current capacity but does not create the final `PlanetBuilding` yet
- due orders can be completed explicitly through `IConstructionOrderCompletionService`
- completing a construction order creates the final `PlanetBuilding`
- completing an upgrade order raises the existing `PlanetBuilding` level to the queued target level
- the background worker is disabled by default
- construction queue HTTP endpoints are development-only and guarded by the existing development endpoint switch

## Research Queue Design Note

The research queue supports time-based research progression with explicit completion and development-only HTTP validation.

Accepted current rules:

- each civilization can have at most one open research order at this stage
- enqueueing research spends resources immediately
- enqueueing research does not immediately increase the `ResearchProject` level
- due orders can be completed explicitly through `IResearchOrderCompletionService`
- completing a research order creates `ResearchProject` if it does not exist
- completing a research order raises the existing `ResearchProject` level to the queued target level
- research queue HTTP endpoints are development-only and guarded by the existing development endpoint switch
- no research background worker exists yet

Current intentional exclusions:

- no fleets
- no movement
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
- research development endpoint foundation
- population and building role foundation
- asset requirement foundation
- asset production queue foundation
- asset inventory foundation
- asset production development endpoint foundation

## Validation Status

Repository-specific application validation exists through the .NET solution.

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validation baseline: `178` passing tests.

Current tests include assembly-boundary coverage, smoke checks for `/` and `/health`, auth endpoint tests with fake services, development galaxy endpoint tests with fake services, development construction queue endpoint tests with fake services, persistence and identity registration checks, application contract tests, deterministic galaxy generation tests, persisted galaxy generation service tests with EF Core InMemory, player/civilization domain tests, starting civilization service tests, planet ownership domain tests, planet colonization service tests, planet economy domain tests, persisted planet economy tick tests, planet building domain tests, building catalog tests, building category tests, asset catalog tests, asset production order tests, asset stock tests, planet population profile tests, planet military capacity calculator tests, persisted building construction tests, persisted building upgrade tests, construction queue service tests, construction order completion service tests, construction queue worker options and registration tests, research duration tests, research queue service tests, research order completion service tests, registration and email confirmation service tests with EF Core InMemory, Brevo sender tests with fake HTTP handlers, and verification that health output does not expose connection string values. Tests do not use the real NAS PostgreSQL database.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Constraints

Current constraints remain:

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, fleets, movement, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
