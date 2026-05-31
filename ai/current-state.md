# Current State

## Phase

The repository is consolidated through `Phase 7H - Strategic map readiness smoke coverage`.

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
- Read-only orbital route profile metadata for travel estimates, classifying abstract distances into deterministic `LocalOrbit`, `InnerSystem`, `OuterSystem`, and `LongRange` bands with placeholder risk and fuel metadata.
- Read-only placeholder orbital fuel readiness previews for travel estimates, deriving estimated fuel units, estimated range, readiness, and not-ready reasons without adding fuel inventory or fuel charging.
- Reusable resource affordability and spend service for persisted planet stockpiles, with atomic multi-resource validation and spending.
- Orbital transfer creation charges estimated travel costs from persisted planet stockpiles before reserving groups and creating transfers.
- Persistent orbital group split foundation for stationed groups, preserving civilization, origin planet, current planet, asset type, and status while decreasing the source group quantity.
- Persistent orbital group merge foundation for compatible stationed groups, increasing the target group quantity and removing the source group.
- Read-only fleet operational overview for civilization-scoped orbital groups, active transfer summaries, and command availability flags.
- High-level fleet lifecycle smoke coverage validates preview, transfer creation and resource charging, active-transfer command rejection, cancellation, split, merge, second transfer completion, and final overview state.
- Route/fuel lifecycle smoke coverage validates that travel estimates include route profile and placeholder fuel readiness, remain read-only, do not require fuel inventory or route graph persistence, and stay coherent with fleet UI state hints through transfer completion.
- Developer-facing fleet API contract documentation under `docs/dev/fleet-api-contracts.md`, covering development gating, request/response payloads, status codes, read-only versus mutating behavior, resource charging/no-refund behavior, restrictions, and a compact fleet lifecycle example.
- Fleet development endpoint response consistency review found the current status-code and response-shape conventions already aligned for safe frontend tooling use, so no endpoint behavior changes were introduced in Phase 6W.
- Visual sandbox development documentation now reports the current Phase 6X validation baseline instead of the stale Phase 6H/6I test count.
- Development-only fleet UI state endpoint aggregates operational group state, active transfer summaries, command availability, current-planet resource contexts, and action hints for future UI prototypes.
- Development-only fleet UI state endpoint exposes route/fuel readiness capability hints for each group, while leaving concrete route profile and fuel readiness previews null until a destination-specific travel estimate is requested.
- Development-only fleet action manifest exposes deterministic, machine-readable metadata for current fleet dev actions, including route, method, mutability, required fields, success status, common error statuses, and notes for route/fuel preview flows.
- Read-only strategic map service consolidates civilization-scoped relevant systems, planet visual/layout summaries, owned ownership markers, fleet presence, active transfer overlays, and route/fuel capability notes without adding gameplay behavior or production UI endpoints.
- Development-only strategic map endpoint at `GET /api/dev/strategic-map?civilizationId={id}` exposes the Phase 7E read model behind existing development gating and persistence checks.
- Strategic map development contract documentation under `docs/dev/strategic-map-api-contract.md` describes request/response fields, gating behavior, side effects, limitations, and relationship to visual/fleet read models.
- Strategic map projections sanitize foreign owned planet visual intensity details until a real visibility/sensor model exists.
- Development-only strategic map action manifest at `GET /api/dev/strategic-map/action-manifest` exposes deterministic metadata for current strategic map, visual state, fleet UI state, and related manifest read actions.
- Strategic map readiness smoke coverage validates that strategic map, visual state, fleet UI state, and strategic map action manifest read surfaces remain coherent and do not mutate stockpiles, orbital groups, or transfers.
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
- no route graph/pathfinding model or persisted fuel inventory/refueling model
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
- Orbital route profiles are read-only metadata derived from abstract distance units. Current bands are intentionally coarse placeholders: distance `1` is `LocalOrbit`, `2-3` is `InnerSystem`, `4-6` is `OuterSystem`, and `7+` is `LongRange`. Current profiles are supported and use a placeholder fuel multiplier of `1.0`; they do not introduce pathfinding, route graphs, fuel inventory, combat, interception, alliances, or espionage.
- Orbital fuel readiness is a placeholder read model derived from asset type, group quantity, abstract distance, and route profile. It reports estimated fuel units required, estimated range units available, readiness, and not-ready reasons, but it does not add persisted fuel state, refueling, spending, or transfer-creation behavior.
- Creating an orbital transfer charges the estimated travel costs from the current planet stockpile before reserving the orbital group and creating the transfer.
- Cancelling an orbital transfer is explicit, persistent, and only available before completion. Phase 6R cancellation marks the transfer cancelled, releases the reserved orbital group back to stationed status at its current persisted planet, and does not refund charged resources.
- An orbital group with an active transfer cannot be split, merged as a source or target, assigned a second transfer, or used for a new travel estimate preview. Active transfer means a transfer that is neither completed nor cancelled. Completion and cancellation remain the valid lifecycle operations for the active transfer itself.
- Splitting an orbital group is available only for stationed groups owned by the requesting civilization. The split quantity must be positive and lower than the source quantity.
- Merging orbital groups requires different stationed groups owned by the requesting civilization, sharing the same current planet and asset type. The target group quantity increases and the source group is removed.
- Fleet operational overview is read-only. It consolidates orbital group state, active transfer timing/status, and command availability without creating transfers, cancelling transfers, completing transfers, splitting, merging, charging resources, or mutating persisted state.
- Fleet lifecycle smoke tests are xUnit tests over EF in-memory services; the repository integration-test script remains a placeholder and reports no configured integration tests.
- Fleet UI state is read-only development tooling for future UI prototypes. It aggregates existing overview data, resource context for group current planets, and action hints without mutating persisted state.
- Fleet UI state route/fuel readiness hints intentionally do not invent a destination. The travel estimate endpoint remains the source of concrete route profile and fuel readiness previews because it requires `destinationPlanetId`.
- Fleet action manifest is read-only development tooling for future UI prototypes. It lists available dev fleet actions and contracts, including route/fuel preview guidance, but does not replace command validation.
- The current sandbox renders markers and transfer route lines as visual indicators only.
- Strategic map read model is read-only backend preparation for future map UI. Phase 7G scopes relevance to owned planets and active transfer origin/destination planets for the requesting civilization; no separate known/visibility model exists yet. Ownership, fleet details, and detailed planet visual intensity signals from other civilizations are not exposed by this read model.
- Strategic map action manifest is read-only development tooling for future UI prototypes. It lists strategic map, visual-state, fleet UI state, and manifest read actions with method, route, required fields, success status, common error statuses, and notes.
- Strategic map readiness smoke coverage protects the current limitation that map/readiness contracts do not expose mesh, texture, binary, shader, route graph, pathfinding, combat, or interception payload fields.

## Dev Surface Gating Note

The same switch controls development APIs and static sandbox files.

Accepted current rules:

- Development environment serves dev APIs and `/dev/visual-state/*` static files.
- Production environment does not serve `/dev/visual-state/*` by default.
- Production serves those files and development API routes only when `VoidEmpires:DevEndpoints:Enabled=true`.
- The visual sandbox and assets remain development tooling.

## Validation Status

Run from repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validated baseline after Phase 7H: `397` passing tests.

Recent expected coverage includes orbital groups, orbital transfers, workers, visual state services/endpoints, system layout hints, markers, transfer overlays, static sandbox asset serving, overlay sandbox hooks, static sandbox gating behavior, fleet UI state service, fleet action manifest service, the strategic map read model, and the strategic map development endpoint.

## Recommended Next Work

1. Deepen movement only after deciding whether route graphs, pathfinding, persisted fuel inventory, or refueling are required.
2. Add combat/interception foundations only after fleet movement and visibility contracts stabilize.
3. Start a real renderer spike only after the visual state contract remains stable.
4. Keep `XUniversePlanet Generator Variator` as an external/local prototype reference until the renderer/prototype phase needs it.

## Constraints

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- avoid full UI/3D implementation until the required read contracts and development validation endpoints exist
- keep private operational configuration out of version control
