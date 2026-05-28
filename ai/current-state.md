# Current State

## Phase

The repository is in `Phase 5A - Fleet ownership/origin foundation` while retaining the AI Platform workflow assets from Phase 0.

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

The repository now has:

- PostgreSQL 16 selected as the primary relational database engine.
- EF Core with Npgsql package references in `VoidEmpires.Infrastructure`.
- An empty `ConnectionStrings:DefaultConnection` placeholder in web appsettings files.
- A `VoidEmpiresDbContext` in the Infrastructure persistence boundary using ASP.NET Core Identity tables.
- EF Core migrations for Identity, galaxy, player/civilization, planet ownership, planet economy, planet building, research, construction queue, research queue, planet population, asset production order, asset stock, and orbital group models. Migrations exist in source but are not automatically applied to the real database.
- Infrastructure service registration that enables PostgreSQL only when a non-empty connection string is configured.
- ASP.NET Core Identity registration with unique-email and confirmed-email defaults.
- Application contracts for user registration, email confirmation, and transactional email.
- Infrastructure services for registration and email confirmation backed by ASP.NET Core Identity.
- Brevo transactional email sender wiring behind the provider-agnostic email contract.
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
- Construction queue, research queue, asset production queue, asset inventory, and optional queue workers.
- Fleet ownership/origin foundation through `OrbitalGroupStatus` and `OrbitalGroup`.
- Orbital group contracts through `CreateOrbitalGroupRequest`, `CreateOrbitalGroupResult`, and `IOrbitalGroupService`.
- Orbital group persistence mapping and migration through `OrbitalGroupConfiguration` and `AddOrbitalGroupModel`.
- Orbital asset stock allocation support through `OrbitalAssetStock.Decrease(...)`.

Current gameplay foundation supports this backend chain:

```text
Identity user id -> PlayerProfile -> Civilization -> PlanetOwnership -> Planet -> Economy -> Buildings -> Construction queue/worker -> Research queue/dev endpoints/worker -> Population and military capacity foundation -> Asset requirement foundation -> Asset production queue/dev endpoints/worker -> Asset inventory foundation -> Orbital group ownership/origin foundation
```

## Fleet Ownership and Origin Design Note

The fleet foundation intentionally starts with stationary orbital group ownership and origin tracking only.

Accepted current rules:

- `OrbitalGroup` represents a grouped set of orbital assets.
- `OrbitalGroup.CivilizationId` identifies the owning civilization.
- `OrbitalGroup.OriginPlanetId` identifies where the assets were originally produced or allocated from.
- `OrbitalGroup.CurrentPlanetId` identifies where the group is currently stationed.
- `OrbitalGroup.IsStationedAwayFromOrigin` makes origin/current-location separation explicit.
- `OrbitalAssetStock.Decrease(...)` allows future services to allocate locally produced stock into an orbital group.
- A group stationed away from its origin does not imply population or crew consumption on the current planet.
- Local crew/operator capacity remains validated during production, not during parking/stationing.

Current intentional limitation:

- the persistent service that allocates stock into a group is defined by contract but not implemented yet because the connector blocked the infrastructure implementation.
- no movement
- no travel timing
- no routes
- no combat
- no fleet splitting or merging
- no UI

## Queue Worker Alignment Design Note

The time-based queues share the same operational pattern:

- construction queue has an optional background worker
- research queue has an optional background worker
- asset production queue has an optional background worker
- all workers are disabled by default
- all workers are registered only through configuration
- all workers use configurable intervals with a 30-second fallback
- web host registration only happens when persistence is configured

Current worker configuration sections:

- `VoidEmpires:ConstructionQueueWorker`
- `VoidEmpires:ResearchQueueWorker`
- `VoidEmpires:AssetProductionWorker`

## Population and Military Capacity Design Note

The project has accepted this rule:

- population limits what a planet can generate, recruit, train, or crew locally
- population does not limit what can be parked or stationed on that planet if it was produced elsewhere
- ground force capacity represents the local ability to recruit/train ground forces
- ship crew capacity represents the local ability to crew locally built ships
- parked foreign or transferred ships should be handled by fleet ownership/origin systems, not by the planet population profile itself

## Asset Production and Inventory Design Note

The asset production queue supports timed production orders, persistent local stock creation, development-only HTTP validation, and optional background processing.

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
- asset production background processing is disabled by default

Current intentional limitation:

- stock is planet-local until explicitly allocated to an orbital group; no fleet transfers, movement, deployment, stationed asset mechanics, or combat behavior exists yet

## Research Queue Design Note

The research queue supports time-based research progression with explicit completion, development-only HTTP validation, and optional background processing.

Accepted current rules:

- each civilization can have at most one open research order at this stage
- enqueueing research spends resources immediately
- enqueueing research does not immediately increase the `ResearchProject` level
- due orders can be completed explicitly through `IResearchOrderCompletionService`
- completing a research order creates `ResearchProject` if it does not exist
- completing a research order raises the existing `ResearchProject` level to the queued target level
- research queue HTTP endpoints are development-only and guarded by the existing development endpoint switch
- research background processing is disabled by default

## Validation Status

Repository-specific application validation exists through the .NET solution.

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validation baseline: `191` passing tests.

Current fleet/origin tests include orbital group origin/current location separation, stationed-away-from-origin detection, reserve status behavior, and orbital stock allocation decrease behavior. Tests do not use the real NAS PostgreSQL database.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Constraints

Current constraints remain:

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, movement, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
