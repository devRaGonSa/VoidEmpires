# Current State

## Phase

The repository is consolidated through `Phase 5L - Orbital transfer worker` while retaining the AI Platform workflow assets from Phase 0.

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
- EF Core migrations for Identity, galaxy, player/civilization, planet ownership, planet economy, planet building, research, construction queue, research queue, planet population, asset production order, asset stock, orbital group, and orbital transfer models. Migrations exist in source but are not automatically applied to the real database.
- Infrastructure service registration that enables PostgreSQL only when a non-empty connection string is configured.
- Construction queue, research queue, asset production queue, asset inventory, optional queue workers, and fleet ownership/origin foundations.
- Orbital group allocation service through `OrbitalStockGroupService`, registered as `IOrbitalGroupService`.
- Orbital stock allocation from `OrbitalAssetStock` into `OrbitalGroup` with persisted stock decrease and group creation.
- Development-only orbital group endpoint for HTTP validation of creating orbital groups from local stock.
- Orbital group lookup contracts and persistence-backed lookup service through `IOrbitalGroupLookupService`.
- Development-only orbital group listing endpoint with optional filters for current planet, origin planet, asset type, and status.
- Development endpoint mappings factored out of `Program.cs` to keep application composition smaller and easier to maintain.
- Persisted orbital transfer intents through `OrbitalTransfer`, `OrbitalTransferStatus`, EF configuration, migration, and `IOrbitalTransferPersistenceService`.
- Orbital transfer arrival execution through `IOrbitalTransferCompletionService`, which completes due transfers and moves reserved orbital groups to their destination planet.
- Development-only orbital transfer endpoints for creation, listing/query, and due completion.
- Automated HTTP coverage for orbital transfer development endpoints.
- Optional configurable `OrbitalTransferWorker`, disabled by default, registered only when persistence exists and the worker section enables it.

Current gameplay foundation supports this backend chain:

```text
Identity user id -> PlayerProfile -> Civilization -> PlanetOwnership -> Planet -> Economy -> Buildings -> Construction queue/worker -> Research queue/dev endpoints/worker -> Population and military capacity foundation -> Asset requirement foundation -> Asset production queue/dev endpoints/worker -> Asset inventory foundation -> Orbital group ownership/origin foundation -> Orbital group allocation service -> Orbital group HTTP validation -> Orbital group listing/query HTTP validation -> Orbital transfer persistence -> Orbital transfer create/list/complete dev endpoints -> Orbital transfer worker
```

## Fleet Ownership and Origin Design Note

The fleet foundation supports stationary orbital group ownership, origin tracking, stock allocation, development-only HTTP validation, query/read validation, persisted transfer intents, transfer completion, and optional automatic arrival processing.

Accepted current rules:

- `OrbitalGroup` represents a grouped set of orbital assets.
- `OrbitalGroup.CivilizationId` identifies the owning civilization.
- `OrbitalGroup.OriginPlanetId` identifies where the assets were originally produced or allocated from.
- `OrbitalGroup.CurrentPlanetId` identifies where the group is currently stationed.
- `OrbitalGroup.IsStationedAwayFromOrigin` makes origin/current-location separation explicit.
- `OrbitalAssetStock.Decrease(...)` allocates locally produced stock into an orbital group.
- `IOrbitalGroupService.CreateFromLocalStockAsync(...)` validates stock and persists the resulting group.
- `IOrbitalGroupLookupService.ListAsync(...)` reads orbital groups by civilization with optional current planet, origin planet, asset type, and status filters.
- `POST /api/dev/fleets/orbital-groups/create-from-stock` validates group creation through HTTP when development endpoints and persistence are configured.
- `GET /api/dev/fleets/orbital-groups` validates group listing through HTTP when development endpoints and persistence are configured.
- A group stationed away from its origin does not imply population or crew consumption on the current planet.
- Local crew/operator capacity remains validated during production, not during parking/stationing.

## Orbital Transfer Design Note

The orbital transfer foundation supports planned persisted transfers, manual development validation, query/read access, due completion, and optional automatic processing.

Accepted current rules:

- `OrbitalTransfer` persists the transfer intent with civilization id, orbital group id, origin planet id, destination planet id, abstract distance, departure time, arrival time, and status.
- `IOrbitalTransferPersistenceService.PersistAsync(...)` creates a planned transfer from a stationed orbital group.
- Creating a transfer reserves the orbital group.
- The initial travel model uses `OrbitalTravelEstimator` and abstract distance units.
- `IOrbitalTransferCompletionService.CompleteDueAsync(...)` processes due transfers using an explicit timestamp.
- Completing a due transfer moves the orbital group to `DestinationPlanetId` and marks the transfer as completed.
- Completed transfers are not processed again.
- `IOrbitalTransferLookupService.ListAsync(...)` reads transfers by civilization with optional orbital group, origin planet, destination planet, and status filters.
- `POST /api/dev/fleets/orbital-transfers/create` validates transfer creation through HTTP when development endpoints and persistence are configured.
- `GET /api/dev/fleets/orbital-transfers` validates transfer listing/query through HTTP when development endpoints and persistence are configured.
- `POST /api/dev/fleets/orbital-transfers/complete-due` validates manual completion of due transfers through HTTP when development endpoints and persistence are configured.
- `OrbitalTransferWorker` is disabled by default and only registered when `VoidEmpires:OrbitalTransferWorker:Enabled` is true and persistence exists.

Current intentional limitation:

- no route graph
- no fuel or resource cost for travel
- no interception
- no combat
- no fleet splitting or merging
- no UI

## Queue Worker Alignment Design Note

The time-based queues share the same operational pattern:

- construction queue has an optional background worker
- research queue has an optional background worker
- asset production queue has an optional background worker
- orbital transfers have an optional background worker
- all workers are disabled by default
- all workers are registered only through configuration
- all workers use configurable intervals with a 30-second fallback
- web host registration only happens when persistence is configured

## Population and Military Capacity Design Note

The project has accepted this rule:

- population limits what a planet can generate, recruit, train, or crew locally
- population does not limit what can be parked or stationed on that planet if it was produced elsewhere
- ground force capacity represents the local ability to recruit/train ground forces
- ship crew capacity represents the local ability to crew locally built ships
- parked foreign or transferred ships are handled by fleet ownership/origin systems, not by the planet population profile itself

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
- stock is planet-local until explicitly allocated to an orbital group

## Validation Status

Repository-specific application validation exists through the .NET solution.

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validated baseline after Phase 5L: `246` passing tests.

Recent expected coverage includes orbital group lookup, orbital transfer persistence, orbital transfer completion, orbital transfer lookup, development endpoint access control, persistence-required behavior, invalid request validation, successful response payloads, conflict handling, optional filter propagation, and worker option interval fallback behavior.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Recommended Next Work

Recommended next backend line before UI:

1. Add a route/fuel/travel-cost foundation for orbital transfers if movement should become deeper.
2. Add fleet split/merge foundations if group manipulation is needed before combat.
3. Add read models for solar-system tactical state if the next goal is a 3D/system map.
4. Add `PlanetVisualState` contracts and a development endpoint if the next goal is the procedural 3D planet visual pipeline.

The latest uploaded 3D planet document recommends starting visual work with backend `PlanetVisualState` contracts, an intensity calculator, and a dev endpoint before implementing a full UI/3D scene.

## Constraints

Current constraints remain:

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- avoid full UI/3D implementation until the required read contracts and development validation endpoints exist
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
