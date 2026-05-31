# Current State

## Phase

The repository is consolidated through `Phase 6R - Orbital transfer cancellation foundation`.

## Repository Reality

The AI Platform template has been adapted into a VoidEmpires-specific project workspace with:

- workflow rules in `AGENTS.md`
- planning and orchestration documents under `ai/`
- task lifecycle folders under `ai/tasks/`
- helper scripts under `scripts/`
- the `.NET` solution and projects under `src/` and `tests/`

## Application Status

The repository contains `VoidEmpires.sln` with:

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Domain`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

Current implemented foundations:

- PostgreSQL/EF Core persistence boundary.
- ASP.NET Core Identity persistence foundation.
- Galaxy, player, civilization, ownership, economy, buildings, research, construction queue, research queue, population, asset production, asset stock, orbital group, and orbital transfer persistence models and migrations.
- Optional queue workers for construction, research, asset production, and orbital transfers.
- Development-only HTTP validation endpoints for current backend foundations.
- Planet visual state contracts and persisted visual state service.
- Solar system visual state contracts and persisted system visual state service.
- System visual metadata for renderer preparation: star metadata, coordinates, orbital slots, orbit radii, orbit angles, and scale hints.
- Read-only system visual overlays: stationed orbital group markers and planned/active transfer route overlays.
- Read-only orbital travel estimate previews through the application, infrastructure, and development API layers, including affordability and insufficient-resource details.
- Reusable resource affordability and spend service for persisted planet stockpiles, with atomic multi-resource validation and spending.
- Orbital transfer creation charges estimated travel costs from persisted planet stockpiles before reserving groups and creating transfers.
- Persistent orbital group split foundation for stationed groups, preserving civilization, origin planet, current planet, asset type, and status while decreasing the source group quantity.
- Persistent orbital group merge foundation for compatible stationed groups, increasing the target group quantity and removing the source group.
- Static visual sandbox at `/dev/visual-state/index.html`.
- CSS-only pseudo-3D visual sandbox rendering for planet/system preview, overlays, markers, and transfer routes.
- Static sandbox assets are gated behind the same development switch as development APIs.

Current foundation chain:

```text
Identity -> PlayerProfile -> Civilization -> PlanetOwnership -> Economy -> Buildings -> Queues -> Assets -> OrbitalGroups -> OrbitalTransfers -> PlanetVisualState -> SystemVisualState -> VisualSandbox -> DevSandboxGating
```

## Visual State Design Note

Accepted current rules:

- `PlanetVisualStateDto` is a read contract, not a persisted gameplay entity.
- `PlanetVisualProfileDto` describes render hints.
- `PlanetVisualProfileCatalog` differentiates visual behavior by `PlanetType`.
- `PlanetVisualIntensityCalculator` derives deterministic normalized intensities from existing game data.
- `PlanetVisualStateService` derives single-planet visual state from persisted game data.
- `SystemVisualStateService` returns ordered planet visual states for a solar system.
- `SystemVisualStateDto` includes identity, coordinates, star metadata, layout hints, orbital group markers, transfer overlays, and planets.
- `OrbitalGroupVisualMarkerDto` and `OrbitalTransferVisualOverlayDto` are renderer-facing read projections, not command models.
- Transfer overlay progress is a read-time visual approximation from departure/arrival timestamps.
- `/dev/visual-state/index.html` is development tooling, not final game UI.
- Static sandbox assets are not served in Production by default.
- Static sandbox assets are served outside Development only when `VoidEmpires:DevEndpoints:Enabled=true`.

Current intentional limitations:

- no Three.js/Babylon.js implementation
- no real WebGL renderer
- no meshes, shaders, textures, or binary render assets from the backend
- no persisted visual customization model
- no route graph or fuel/resource travel-cost model
- no combat/interception overlay model
- no final game UI layout

## Fleet and Transfer Design Notes

Accepted current rules:

- `OrbitalGroup` represents grouped orbital assets.
- Origin and current planet are intentionally separated.
- Local crew/operator capacity is validated during production, not during parking/stationing.
- `OrbitalTransfer` persists transfer intent, timing, status, origin, and destination.
- Creating a transfer reserves the orbital group.
- Completing a due transfer moves the group to the destination and marks the transfer completed.
- Orbital travel estimates are preview-only read models. They calculate distance, duration, and estimated resource costs without creating transfers, reserving groups, charging resources, mutating stockpiles, or persisting estimates.
- Orbital travel estimates report whether the current planet stockpile can afford estimated costs and identify insufficient resources without spending balances.
- Creating an orbital transfer charges the estimated travel costs from the current planet stockpile before reserving the orbital group and creating the transfer.
- Cancelling an orbital transfer is explicit, persistent, and only available before completion. Phase 6R cancellation marks the transfer cancelled, releases the reserved orbital group back to stationed status at its current persisted planet, and does not refund charged resources.
- Splitting an orbital group is available only for stationed groups owned by the requesting civilization. The split quantity must be positive and lower than the source quantity.
- Merging orbital groups requires different stationed groups owned by the requesting civilization, sharing the same current planet and asset type. The target group quantity increases and the source group is removed.
- The current sandbox renders markers and transfer route lines as visual indicators only.

## Dev Surface Gating Note

The same switch controls development APIs and static sandbox files.

Accepted current rules:

- Development environment serves dev APIs and `/dev/visual-state/*` static files.
- Production environment does not serve `/dev/visual-state/*` by default.
- Production serves those files only when `VoidEmpires:DevEndpoints:Enabled=true`.
- The visual sandbox and assets remain development tooling.

## Validation Status

Run from repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validated baseline after Phase 6R: `350` passing tests.

Recent expected coverage includes orbital groups, orbital transfers, workers, visual state services/endpoints, system layout hints, markers, transfer overlays, static sandbox asset serving, overlay sandbox hooks, and static sandbox gating behavior.

## Recommended Next Work

1. Add route/fuel/travel-cost foundation for orbital transfers if movement should become deeper.
2. Add fleet split/merge foundations if group manipulation is needed before combat.
3. Start a real renderer spike only after the visual state contract remains stable.
4. Keep `XUniversePlanet Generator Variator` as an external/local prototype reference until the renderer/prototype phase needs it.

## Constraints

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- avoid full UI/3D implementation until the required read contracts and development validation endpoints exist
- keep private operational configuration out of version control
