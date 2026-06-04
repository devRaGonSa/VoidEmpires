# Shipyard Cockpit Checklist

This checklist documents the current `/shipyard` development cockpit, the seeded QA URL, and the intentional boundary with Fleet work.
Use `docs/dev/development-seed-profiles.md` for the standard Development-only QA commands, ids, and reseed guidance.
Use `docs/dev/shipyard-fleet-persisted-qa.md` for the accepted persisted backend-only QA loop for this block.

Shipyard v1 is a development-safe orbital production cockpit.
It can inspect production context and enqueue one guarded orbital order through the current dev route.
It must not absorb fleet movement, transfer lifecycle control, combat, or split/merge behavior.

## Seeded QA URL

- `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`

## Recommended richer seed

Use `POST /api/dev/seeds/apply` with `{"profile":"shipyard-validation"}` when you want the richer deterministic Shipyard screenshot baseline.

Expected richer baseline:

- `Aurelia` remains the owned Shipyard context.
- exactly one orbital option remains immediately available: `Nave exploradora`
- `Nave de carga` stays blocked by insufficient resources
- `Nave de escolta` and `Nave colonial` stay blocked by missing building requirements
- local stock shows at least `Nave de escolta x4` and `Nave exploradora x1`
- the queue shows one completed production row while enqueue remains available because there is no open production order
- a successful enqueue deducts the full visible resource cost immediately; it does not create a reservation-only hold

## Manual Smoke Checklist

Use this after the required validation commands succeed.

1. Start `VoidEmpires.Web` with development endpoints enabled and with persistence configured.
2. Open the seeded Shipyard URL.
3. Confirm the page loads as a real `Astillero` cockpit route and not as a placeholder-only module shell.
4. Confirm the `Aurelia` context is visible with civilization, planet, system, and local-control cues in the first viewport.
5. Confirm local resources are visible and remain secondary to the cockpit summary instead of raw DTO output.
6. Confirm the production catalog is visible with Spanish asset labels and category grouping.
7. Confirm the seeded scenario shows both available and blocked asset options when the backend supports them.
8. Confirm available options open a review step before enqueue and do not submit immediately.
9. Confirm the guarded confirmation appears before enqueue when the dev action is enabled.
10. Confirm one successful enqueue refreshes queue, catalog, and local reserves from backend-confirmed state.
11. Confirm the successful enqueue immediately reduces the visible local resource balances by the exact asset cost.
12. Confirm `Completar produccion vencida no disponible` stays disabled or placeholder-only unless a future planet-scoped safe route exists.
13. Confirm the cockpit explains clearly that local stock is not the same thing as an operational fleet group.
14. Confirm the links to `Planeta`, `Construccion`, `Investigacion`, `Flotas`, and `Galaxia` preserve `civilizationId` and `planetId`.
15. Confirm diagnostics remain collapsed or visually secondary.
16. Confirm no 3D or WebGL renderer is introduced.
17. Confirm no combat, fleet movement, split, merge, or transfer action can be executed from Shipyard.

## Boundary Summary

- Shipyard owns orbital production context, catalog review, guarded enqueue, queue reading, and local stock reading.
- Fleets owns orbital groups, transfer lifecycle, destination review, movement, cancellation, and due completion.
- Shipyard may link to Fleets for already-visible groups, but it must not mutate Fleet state directly in this block.
- The current complete-due affordance remains intentionally disabled because the backend route is still global and not safely scoped to the visible shipyard context.
- The accepted persisted QA loop for this block is `seed -> shipyard ui-state -> one enqueue -> shipyard ui-state -> fleet ui-state`.
- Successful Shipyard enqueue deducts the full visible cost immediately from `PlanetResourceStockpile`; it does not reserve resources and leave balances unchanged.
- Local orbital stock should stay unchanged immediately after enqueue; produced stock changes only after due processing, which remains out of the safe repeated-QA flow because the current route is global.

## Development Gating

- Development routes exist only when `ASPNETCORE_ENVIRONMENT=Development` or `VoidEmpires:DevEndpoints:Enabled=true` is set in configuration. See `src/VoidEmpires.Web/Program.cs`.
- Persistence-backed routes return `503` when `ConnectionStrings:DefaultConnection` is not configured. See `src/VoidEmpires.Web/DevEndpointMappings.cs`.
- Asset production and orbital transfer background workers are optional registrations, not guaranteed runtime behavior. See `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`.

## Component Map

### Domain

- `src/VoidEmpires.Domain/Assets/OrbitalAssetCatalog.cs`
- `src/VoidEmpires.Domain/Assets/PlanetaryAssetCatalog.cs`
- `src/VoidEmpires.Domain/Assets/AssetProductionOrder.cs`
- `src/VoidEmpires.Domain/Assets/OrbitalAssetStock.cs`
- `src/VoidEmpires.Domain/Assets/PlanetaryAssetStock.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalGroup.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalTransfer.cs`

### Application and Infrastructure

- `IAssetProductionQueueService` -> `AssetProductionQueueService`
- `IAssetOrderProcessor` -> `AssetOrderProcessor`
- `IOrbitalGroupService` -> `OrbitalStockGroupService`
- `IOrbitalGroupLookupService` -> `OrbitalGroupLookupService`
- `IFleetOperationalOverviewService` -> `FleetOperationalOverviewService`
- `IDevFleetUiStateService` -> `DevFleetUiStateService`

### Web Entrypoints

- `POST /api/dev/assets/production/enqueue`
- `POST /api/dev/assets/production/process-due`
- `POST /api/dev/fleets/orbital-groups/create-from-stock`
- `GET /api/dev/fleets/orbital-groups`
- `GET /api/dev/fleets/overview`
- `GET /api/dev/fleets/ui-state`
- `GET /api/dev/fleets/action-manifest`

## Current Backend Inventory

### Asset taxonomy, requirements, costs, and durations

Available now:

- Orbital asset taxonomy exists in `OrbitalAssetCatalog` with `ScoutCraft`, `CargoCraft`, `EscortCraft`, and `ColonyCraft`.
- Each orbital asset definition includes:
  - building requirement;
  - resource cost;
  - storage capacity;
  - operating range.
- Orbital definitions currently require:
  - `ScoutCraft`: `Shipyard` level `1`;
  - `CargoCraft`: `Shipyard` level `1`;
  - `EscortCraft`: `FleetCommandCenter` level `1`;
  - `ColonyCraft`: `LogisticsHub` level `2`.
- Queue duration is currently a fixed `3` minutes per produced unit through `AssetProductionQueueService`, not per-asset custom duration.

Implication for Shipyard v1:

- It is safe to show orbital asset keys, building requirements, costs, and the current queue-duration rule.
- It is not safe to invent per-asset production times or hidden backend labels that do not exist yet.

### Production orders and queue state

Available now:

- `AssetProductionOrder` persists:
  - target kind (`Planetary` or `Orbital`);
  - selected asset type;
  - quantity;
  - sequence;
  - start and end UTC timestamps;
  - status.
- `AssetProductionQueueService` enforces only one open asset production order per planet.
- Queue enqueue spends resources immediately when the order is created.
- The processor marks due orders completed and increases either `PlanetaryAssetStock` or `OrbitalAssetStock`.

Current limits:

- There is no shipyard-specific read service or read endpoint for listing asset production orders.
- There is no existing endpoint that returns a production queue read model for frontend use.
- `process-due` is a batch mutation over all due asset orders and is not scoped by civilization.

Implication for Shipyard v1:

- The backend supports enqueueing and completing orders.
- The backend does not yet support truthfully rendering a dedicated queue panel from a shipyard-specific read endpoint.

### Stock and inventory ownership models

Available now:

- Planet resource ownership is modeled through `PlanetResourceStockpile`.
- Produced units land in `PlanetaryAssetStock` or `OrbitalAssetStock`, keyed by `PlanetId` and asset type.
- The development seed creates orbital stock for the owned seed planet: `EscortCraft x4`.

Current limits:

- There is no read endpoint dedicated to planetary or orbital asset stock.
- `GET /api/dev/fleets/ui-state` exposes resource stockpile context for current fleet planets, but not orbital asset stock.
- `GET /api/dev/planet/ui-state` exposes resource stockpile and orbital traffic context, but not asset stock rows or asset production queue rows.

Implication for Shipyard v1:

- The persistence model for local stock exists.
- A truthful stock panel still needs a read model or dev endpoint before it can become a real cockpit section.

### Orbital group creation and fleet handoff

Available now:

- `OrbitalStockGroupService` can create a stationed `OrbitalGroup` by consuming `OrbitalAssetStock`.
- `GET /api/dev/fleets/orbital-groups`, `GET /api/dev/fleets/overview`, and `GET /api/dev/fleets/ui-state` already expose existing orbital groups for fleet inspection flows.

Current limits:

- `create-from-stock` is currently a fleet development endpoint, not a shipyard endpoint.
- `OrbitalStockGroupService` validates only request shape and available stock.
- It does not currently verify:
  - that the civilization owns the source planet;
  - that `originPlanetId` and `currentPlanetId` are the same local shipyard planet;
  - that the request belongs to a controlled shipyard-only workflow.
- The endpoint returns `409` on service rejection, but the service surface is still thin and development-oriented.

Implication for Shipyard v1:

- Linking from Shipyard to Fleets for already-existing orbital groups is safe.
- Executing stock-to-group handoff directly from Shipyard v1 would overclaim current safety and should stay out of scope for this block.

## Dev-Only Routes That Affect Shipyard Behavior

### Asset production

- `POST /api/dev/assets/production/enqueue`
  - status: development-only mutation
  - behavior: creates one asset production order and spends resources immediately
  - scope note: supports both `Planetary` and `Orbital` targets, so Shipyard must constrain itself to orbital usage only
- `POST /api/dev/assets/production/process-due`
  - status: development-only batch mutation
  - behavior: completes all due asset production orders and increases stock
  - scope note: not civilization-scoped and not shipyard-scoped

### Fleet handoff and inspection

- `POST /api/dev/fleets/orbital-groups/create-from-stock`
  - status: development-only mutation
  - behavior: consumes orbital stock and creates a stationed orbital group
  - scope note: too thinly validated for Shipyard v1 execution and excluded from the accepted persisted default QA loop
- `GET /api/dev/fleets/orbital-groups`
  - status: read-only
  - behavior: lists existing orbital groups with filters
- `GET /api/dev/fleets/overview`
  - status: read-only
  - behavior: summarizes groups, active transfers, and command availability
- `GET /api/dev/fleets/ui-state`
  - status: read-only
  - behavior: exposes fleet cockpit state and current-planet resource context
- `GET /api/dev/fleets/action-manifest`
  - status: read-only metadata
  - behavior: documents current fleet dev actions, including `create-from-stock`, split, merge, transfer, and completion flows

## Automated Coverage Present Today

Covered:

- asset catalog and requirement shape: `AssetCatalogTests`
- production-order domain behavior: `AssetProductionOrderTests`
- asset stock increase and decrease domain behavior: `AssetStockTests`, `OrbitalAssetStockAllocationTests`
- stock-to-group handoff service behavior: `OrbitalStockGroupServiceTests`
- development seed asset and fleet baseline: `DevelopmentSeedServiceTests`
- development HTTP contract for `create-from-stock`: `DevOrbitalGroupEndpointTests`

Missing or thin:

- no focused tests for `AssetProductionQueueService`
- no focused tests for `AssetOrderProcessor`
- no development endpoint tests for `POST /api/dev/assets/production/enqueue`
- no development endpoint tests for `POST /api/dev/assets/production/process-due`
- no read-model tests for a shipyard-specific queue or stock surface because that surface does not exist yet

## Safe Shipyard V1 Scope

### Available now

- Read orbital asset options from the existing backend catalogs.
- Show requirements and resource costs for orbital production options.
- Use the current backend rule that production duration is `3` minutes per unit.
- Link users to Fleets for existing orbital groups.
- Reuse the current development seed as a validation baseline for owned planet resources, one seeded orbital stock row, and visible fleet context.

### Available only through dev-only endpoints

- Enqueue one orbital asset production order through `POST /api/dev/assets/production/enqueue`.
- Complete due production through `POST /api/dev/assets/production/process-due`.
- Inspect existing orbital groups through fleet read endpoints.
- Create an orbital group from stock through `POST /api/dev/fleets/orbital-groups/create-from-stock`.

### Read-only only

- Fleet overview, fleet UI state, and orbital group lookup routes.
- Any shipyard presentation of catalogs, requirements, costs, and current documented limitations.
- Planet-level contextual stockpile information that already comes from the planet UI state or fleet UI state read surfaces.

### Unsafe or out of scope for Block 17C-17T

- Treating `create-from-stock` as a safe shipyard action.
- Exposing split, merge, transfer creation, transfer cancellation, or transfer completion from Shipyard.
- Treating `POST /api/dev/assets/production/process-due` as a repeatable shipyard QA step, because it currently processes all due asset orders globally.
- Pretending a shipyard queue read model already exists.
- Pretending orbital stock read endpoints already exist.
- Inventing shipyard-specific ownership validation that the current service does not perform.
- Adding production endpoints outside the development surface.

## Decision Check For Requested Behaviors

| Behavior | Classification | Notes |
|---|---|---|
| Read available orbital production options | Available now | Catalog data exists in domain code. |
| Show requirements, costs, and durations | Available now | Requirements and costs exist; duration is the current queue-service rule, not asset metadata. |
| Show local planetary or orbital stock | Not safely available yet | Persistence exists, but no truthful shipyard-ready read endpoint exists for asset stock. |
| Show a production queue | Not safely available yet | Order persistence exists, but no queue read model or endpoint exists. |
| Enqueue one safe asset production order | Dev-only only | Supported only through `POST /api/dev/assets/production/enqueue`. |
| Complete due production | Dev-only only | Supported only through `POST /api/dev/assets/production/process-due`, which is a global batch mutation. |
| Link to Fleets for existing orbital groups | Available now | Fleet read endpoints already expose this state. |

## Recommended Block Boundary For Shipyard V1

Shipyard v1 should currently be framed as:

- a truthful orbital production catalog;
- a development-only enqueue affordance only when the dev route is intentionally enabled;
- a truthful queue and stock read surface backed by the current development UI-state endpoint;
- a handoff surface to Fleets for existing orbital groups, not a replacement for fleet lifecycle management.

Shipyard v1 should not currently be framed as:

- a full production management cockpit;
- a stock-to-fleet execution surface;
- a queue operations module;
- a movement or command cockpit.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```
