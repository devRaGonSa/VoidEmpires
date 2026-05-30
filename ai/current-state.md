# Current State

## Phase

The repository is consolidated through `Phase 5Y/5Z - System visual layout metadata` while retaining the AI Platform workflow assets from Phase 0.

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
- Development-only orbital group endpoint for HTTP validation of creating orbital groups from local stock.
- Persisted orbital transfer intents, manual dev endpoints, lookup endpoints, endpoint tests, and configurable automatic arrival worker.
- Planet visual state contracts, deterministic intensity calculator, planet visual profile catalog, persisted planet visual state service, and development endpoint for single-planet visual state.
- Solar system visual state contracts, persistence-backed system visual state service, and development endpoint for system-level visual state.
- System visual metadata for real renderer preparation: galaxy id, system name, coordinates, star visual state, planet orbital slots, orbit radii, orbit angles, and visual scale hints.
- Static development sandbox at `/dev/visual-state/index.html` for inspecting planet/system visual state payloads.
- CSS-only pseudo-3D visual sandbox mode for planet and system previews, with card mode fallback, intensity bars, profile panel, and raw JSON payload view.

Current gameplay/backend/frontend foundation supports this chain:

```text
Identity user id -> PlayerProfile -> Civilization -> PlanetOwnership -> Planet -> Economy -> Buildings -> Construction queue/worker -> Research queue/dev endpoints/worker -> Population and military capacity foundation -> Asset requirement foundation -> Asset production queue/dev endpoints/worker -> Asset inventory foundation -> Orbital group ownership/origin foundation -> Orbital group allocation service -> Orbital group HTTP validation -> Orbital group listing/query HTTP validation -> Orbital transfer persistence -> Orbital transfer create/list/complete dev endpoints -> Orbital transfer worker -> Planet visual state -> Solar system visual state -> System visual layout metadata -> Visual state sandbox
```

## Visual State Design Note

The visual backend follows the uploaded 3D planet technical document: game data, computed visual state, and frontend render state stay separated.

Accepted current rules:

- `PlanetVisualStateDto` is a read contract, not a persisted gameplay entity.
- `PlanetVisualProfileDto` describes render hints such as surface profile, light distribution, platform mode, atmosphere, cloud profile, and supported layers.
- `PlanetVisualProfileCatalog` differentiates visual behavior by `PlanetType`.
- `PlanetVisualIntensityCalculator` derives deterministic normalized intensities from existing game data.
- The visual seed is deterministic from `PlanetId` so the same planet keeps a stable base pattern.
- `PlanetVisualStateService` derives single-planet visual state from `Planet`, `PlanetOwnership`, `PlanetBuilding`, and `OrbitalGroup`.
- `SystemVisualStateService` returns ordered planet visual states for a solar system.
- `SystemVisualStateDto` now includes system identity, galaxy id, system name, coordinates, star visual state, layout hints, and planet visual states.
- `StarVisualStateDto` exposes star id, name, type, visual class, and light intensity.
- `PlanetVisualLayoutHintDto` exposes planet id, orbital slot, orbit radius, orbit angle degrees, and visual scale.
- Layout hints are deterministic and derived from persisted orbital slot and planet size; they are frontend/render hints, not gameplay physics.
- `GET /api/dev/planets/{planetId}/visual-state` validates single-planet visual state through HTTP when development endpoints and persistence are configured.
- `GET /api/dev/solar-systems/{systemId}/visual-state` validates system-level visual state through HTTP when development endpoints and persistence are configured.
- `/dev/visual-state/index.html` is a development-only inspection surface for visual contracts and should not be treated as the final game UI.
- The sandbox currently supports pseudo-3D CSS rendering, card rendering, selected planet inspection, intensity bars, profile metadata, and raw payload inspection.

Current intentional limitation:

- no Three.js/Babylon.js implementation
- no real WebGL renderer
- no meshes, shaders, textures, or binary render assets from the backend
- no persisted visual customization model
- no visual landmarks or hand-authored biomes
- no gameplay terraform system yet
- no transfer overlay model in visual state yet
- no orbital group marker model in system visual state yet
- no final game UI layout yet

## Fleet Ownership and Origin Design Note

The fleet foundation supports stationary orbital group ownership, origin tracking, stock allocation, development-only HTTP validation, query/read validation, persisted transfer intents, transfer completion, and optional automatic arrival processing.

Accepted current rules:

- `OrbitalGroup` represents a grouped set of orbital assets.
- `OrbitalGroup.CivilizationId` identifies the owning civilization.
- `OrbitalGroup.OriginPlanetId` identifies where the assets were originally produced or allocated from.
- `OrbitalGroup.CurrentPlanetId` identifies where the group is currently stationed.
- `OrbitalGroup.IsStationedAwayFromOrigin` makes origin/current-location separation explicit.
- Local crew/operator capacity remains validated during production, not during parking/stationing.

## Orbital Transfer Design Note

The orbital transfer foundation supports planned persisted transfers, manual development validation, query/read access, due completion, and optional automatic processing.

Accepted current rules:

- `OrbitalTransfer` persists the transfer intent with civilization id, orbital group id, origin planet id, destination planet id, abstract distance, departure time, arrival time, and status.
- `IOrbitalTransferPersistenceService.PersistAsync(...)` creates a planned transfer from a stationed orbital group.
- Creating a transfer reserves the orbital group.
- Completing a due transfer moves the orbital group to `DestinationPlanetId` and marks the transfer as completed.
- `OrbitalTransferWorker` is disabled by default and only registered when `VoidEmpires:OrbitalTransferWorker:Enabled` is true and persistence exists.

Current intentional limitation:

- no route graph
- no fuel or resource cost for travel
- no interception
- no combat
- no fleet splitting or merging
- no final UI

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
- processing due orders creates or increments stock and then marks orders as completed
- stock is planet-local until explicitly allocated to an orbital group

## Validation Status

Repository-specific application validation exists through the .NET solution.

Run these commands from the repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current expected baseline after Phase 5Y/5Z: at least `279` passing tests. Phase 5Y/5Z added system visual metadata coverage on top of that baseline.

Recent expected coverage includes orbital group lookup, orbital transfer persistence, orbital transfer completion, orbital transfer lookup, development endpoint access control, persistence-required behavior, invalid request validation, successful response payloads, conflict handling, optional filter propagation, worker option interval fallback behavior, planet visual profile/intensity calculation, planet visual state service/endpoint, solar system visual state service/endpoint, system visual metadata/layout hints, static sandbox asset serving, and pseudo-3D sandbox asset hooks.

If a task later introduces integration boundaries before tests exist, record `No integration tests configured.`

## Recommended Next Work

Recommended next backend/frontend line:

1. Add transfer overlay read contracts if moving fleets should be visible in the system view.
2. Add orbital group marker read contracts if stationed fleets should be visible around planets.
3. Add lightweight UI route protection/configuration so development sandboxes can be disabled outside safe environments.
4. Add route/fuel/travel-cost foundation for orbital transfers if movement should become deeper.
5. Add fleet split/merge foundations if group manipulation is needed before combat.
6. Start a real renderer spike only after the visual state contract remains stable.

## Constraints

Current constraints remain:

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- avoid full UI/3D implementation until the required read contracts and development validation endpoints exist
- never commit real database secrets, Brevo secrets, private hostnames, VPN details, NAS connection information, or production email configuration
