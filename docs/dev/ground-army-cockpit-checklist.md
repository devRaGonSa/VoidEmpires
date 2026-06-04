# Ground Army Cockpit Checklist

Ground Army v1 must stay a terrestrial readiness cockpit.
It must not expand into invasion, assault, raid, occupation, bombardment, fleet mutation, or combat resolution.

Use this checklist with `docs/dev/planet-module-boundaries.md` and `docs/dev/development-seed-profiles.md`.
Today `/ground-army` is still a placeholder route implemented through the shared `ModuleCabinPage`, not through a dedicated Ground Army page or backend UI-state service.

## Current backend inventory

### Domain concepts already implemented

- Ground military buildings exist:
  - `BuildingType.MilitaryAcademy`
  - `BuildingType.Barracks`
  - Both are tagged as `BuildingCategory.MilitaryGround`.
- Planetary ground-force assets exist:
  - `PatrolGroup`
  - `ExpeditionGroup`
  - `VehicleGroup`
  - `SupportGroup`
- Planetary asset requirements are already defined through `PlanetaryAssetCatalog`:
  - `PatrolGroup` requires `Barracks` level `1`
  - `ExpeditionGroup` requires `MilitaryAcademy` level `1`
  - `VehicleGroup` requires `Barracks` level `2`
  - `SupportGroup` requires `LogisticsHub` level `1`
- Ground-force capacity already exists through `PlanetMilitaryCapacityCalculator`:
  - base source: `PlanetPopulationProfile.BaseRecruitablePopulation`
  - building bonuses:
    - `MilitaryAcademy`: `1000 * level`
    - `Barracks`: `2500 * level`
    - `CommandCenter`: `250 * level`
- Planetary production persistence already exists:
  - `AssetProductionOrder` supports `AssetProductionTarget.Planetary`
  - `PlanetaryAssetStock` stores produced ground assets

### Development-only mutation surface already implemented

- Generic Development enqueue route exists: `POST /api/dev/assets/production/enqueue`
- Generic Development processing route exists: `POST /api/dev/assets/production/process-due`
- The shared queue service already supports `AssetProductionTarget.Planetary`
- The enqueue path already guards:
  - owned-planet access
  - required building presence and level
  - resource affordability
  - population-capacity readiness
  - single open production order per planet
- The processor already completes planetary orders by increasing `PlanetaryAssetStock`

This means Ground Army already has a safe Development-only enqueue foundation through the generic asset-production pipeline, even though it has no cockpit-specific route yet.

## What does not exist today

- No dedicated Ground Army backend query service is registered in DI.
- No `MapDevGroundArmy...` endpoint exists in `Program.cs` or the web layer.
- No dedicated Ground Army UI-state DTO exists in Application.
- No seeded Ground Army QA baseline exists in `DevelopmentSeedService`.
- No Ground Army queue read model exists for the frontend.
- No dedicated frontend page exists for `/ground-army`; the route still renders the shared placeholder cabin.

## Existing indirect surfaces Ground Army can rely on

### Safe read-only state already available

- Planet context already exists through the planet UI-state route and the shared module shell:
  - civilization id
  - planet id
  - ownership
  - planet identity
  - nearby route handoffs
- Construction already classifies `MilitaryGround` actions as belonging to the Ground Army module for handoff purposes.
- Building labels already exist for `MilitaryAcademy` and `Barracks`.
- Module routing already preserves `/ground-army?civilizationId=...&planetId=...`.

### Safe guarded mutation already available

- Ground-asset enqueue can already be performed through the generic Development asset-production endpoint by sending:
  - `planetId`
  - `civilizationId`
  - `target = Planetary`
  - `planetaryAssetType`
  - `quantity`
  - `requestedAtUtc`

### Important limitation

The current generic asset-production API is not yet a Ground Army cockpit contract.
It is a shared Development-only backend primitive.
Later Ground Army cockpit work should wrap or project this behavior through a dedicated ground-army read model instead of calling raw queue infrastructure directly from the main page.

## Safe Ground Army v1 scope

Ground Army v1 is allowed to show:

- selected-planet context
- terrestrial military buildings and readiness labels
- readable names for ground-force asset types
- resource affordability and missing-resource guidance
- requirement guidance for barracks, academy, and logistics support
- current planetary ground-asset stock when a dedicated read model exists
- queue context when a dedicated read model exists
- explicit Development-only enqueue readiness if the later cockpit chooses to expose the existing generic planetary production path

Ground Army v1 must remain read-only or guarded Development-only preparation only.
It must describe terrestrial force readiness and training, not battlefield execution.

## Explicit non-goals for v1

Do not implement or imply support for:

- invasion execution
- assault execution
- raid execution
- occupation execution
- bombardment
- planetary damage
- troop movement between planets
- garrison combat resolution
- defense-vs-army battle resolution
- fleet movement or orbital transport execution
- galaxy mutation
- production authentication changes

## Scope statement for later tasks

### Allowed first upgrade path

The safest next backend step is a dedicated Development-only Ground Army read model, for example:

- `GET /api/dev/ground-army/ui-state?civilizationId=...&planetId=...`

Recommended DTO shape:

- `planet`
- `stockpile`
- `groundReadiness`
- `buildings`
- `catalog`
- `queue`
- `stationedGroundAssets`
- `actionHints`
- `diagnostics`
- `errors`

Recommended read-model contents:

- `groundReadiness`
  - `baseRecruitablePopulation`
  - `buildingCapacityBonus`
  - `totalGroundCapacity`
  - `notes`
- `catalog[]`
  - display label
  - asset type key
  - required building
  - required level
  - population requirement
  - resource cost
  - availability
  - blocked reasons
- `queue`
  - current open planetary production order if it targets `Planetary`
  - recent completed history rows if later tasks choose to show them
- `stationedGroundAssets[]`
  - asset type
  - quantity
  - role label

### Allowed guarded action path

If later tasks expose a button, keep it Development-only and route it through a Ground Army-specific wrapper over the existing planetary asset enqueue behavior.
Do not expose the global complete-due route from the Ground Army cockpit until there is a planet-scoped safe contract.

## Seed readiness gap

Current seed profiles do not create:

- `MilitaryAcademy`
- `Barracks`
- `LogisticsHub`
- `PlanetaryAssetStock`
- planetary `AssetProductionOrder` history

So the current shared seed set is not yet sufficient for a meaningful Ground Army cockpit.
Later Ground Army seed tasks should add a deterministic terrestrial baseline without seeding combat or transport.

## Evidence summary

- Domain and persistence already support terrestrial asset production.
- The backend already contains a safe Development-only planetary enqueue primitive.
- The frontend route still behaves as a placeholder cabin.
- No combat, invasion, occupation, or troop-movement execution surface was found in the current backend.

## Final boundary

Ground Army v1 should be treated as a planet-side readiness and preparation cockpit only.
It can safely grow into a dedicated read model plus guarded Development-only training enqueue over existing planetary production primitives.
It must not activate combat, invasions, occupation, or any orbital behavior in this build.
