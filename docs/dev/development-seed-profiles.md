# Development Seed Profiles

This document is the current source of truth for the development-only `minimal-validation` seed exposed by `POST /api/dev/seeds/apply`.

## Profile catalog

`POST /api/dev/seeds/apply` remains Development-only and accepts exact profile names only. There are no undocumented aliases.

| Profile | Status | Intended QA use |
|---|---|---|
| `minimal-validation` | Implemented | Current deterministic shared baseline for Galaxy, Planet, Construction, Research, Shipyard, and Fleets |
| `cockpit-validation` | Implemented | Richer combined cockpit baseline with non-blocking completed history for Planet, Research, and Shipyard |
| `shipyard-validation` | Implemented | Shipyard-focused richer baseline with completed queue history, two local stock rows, one available hull, and blocked comparisons |
| `fleet-validation` | Planned only | Future fleet-focused richer baseline |
| `research-validation` | Planned only | Future research-focused richer baseline |
| `planet-full-validation` | Planned only | Future richer planet and construction baseline |

- Endpoint: `POST /api/dev/seeds/apply`
- Request body:

```json
{
  "profile": "minimal-validation"
}
```

Unsupported profile requests fail safely. The current response now includes the requested profile name, the applied profile metadata when successful, and the known profile catalog so PowerShell or JSON callers can discover the supported naming contract directly.

## Dependency map

- Web entrypoint: `src/VoidEmpires.Web/DevEndpointMappings.cs`
- Application contract: `src/VoidEmpires.Application/Development/IDevelopmentSeedService.cs`
- Implementation: `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- Registration: `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`

`Program.cs` maps the dev route only when development endpoints are enabled. The seed service depends only on `VoidEmpiresDbContext`.

## Deterministic ids and labels

These ids are hard-coded today and are relied on by docs, frontend QA URLs, and multiple tests.

| Kind | Value | Current label or note |
|---|---|---|
| Player profile | `90000000-0000-0000-0000-000000000001` | `Validation Commander` / `seed-user-minimal-validation` |
| Civilization | `00000000-0000-0000-0000-000000000001` | `Void Seed Civilization` |
| Galaxy | `10000000-0000-0000-0000-000000000001` | `Validation Galaxy` |
| Solar system | `20000000-0000-0000-0000-000000000001` | `Helios Gate` |
| Star | `30000000-0000-0000-0000-000000000001` | `Helios Gate Star` |
| Owned planet | `40000000-0000-0000-0000-000000000001` | `Aurelia` |
| Visible comparison planet | `40000000-0000-0000-0000-000000000002` | `Cinder Reach` |
| Visible comparison planet | `40000000-0000-0000-0000-000000000003` | `Aether Crown` |

Deterministic transfer timestamps:

- departure: `2026-06-02T08:00:00Z`
- arrival: `2026-06-02T12:00:00Z`

The seeded orbital groups, transfer row, and queue rows do not use fixed ids. Inspect current read endpoints when a test or cockpit needs those runtime ids.

## Seed graph created today

## `cockpit-validation` additions

`cockpit-validation` builds on `minimal-validation`, tops `Aurelia` up to at least `220` credits, `320` metal, `220` crystal, and `120` gas, and then adds one completed construction row, one completed `EnergySystems` research order plus project, and one completed orbital `ScoutCraft` production order plus local `ScoutCraft` stock. It stays non-destructive and avoids extra pending or active queue rows so the current executable cockpit actions remain available.

## `shipyard-validation` additions

`shipyard-validation` builds on `minimal-validation`, tops `Aurelia` up to at least `180` credits, `180` metal, `110` crystal, and `70` gas, adds one completed `ScoutCraft` production row, and adds a second local orbital stock type so Shipyard can show queue history plus richer stock.

Expected Shipyard result:

- `ScoutCraft` remains available.
- `CargoCraft` is blocked by `InsufficientResources`.
- `EscortCraft` and `ColonyCraft` remain blocked by missing building requirements.
- Local orbital stock shows at least `EscortCraft x4` and `ScoutCraft x1`.
- The queue shows one completed production row and no open order, so guarded enqueue remains available.

### Identity and ownership

- Inserts the player profile only when `PlayerProfile.Id = 90000000-0000-0000-0000-000000000001` is missing.
- Inserts the civilization only when `Civilization.Id = 00000000-0000-0000-0000-000000000001` is missing.
- Inserts one active ownership row only for `Aurelia`.
- Result:
  - `Aurelia` is owned by the seeded civilization.
  - `Cinder Reach` and `Aether Crown` are not assigned to the seeded civilization.

### Galaxy layout

- Inserts one galaxy only when missing.
- Inserts one solar system only when missing.
- Inserts one star under that system only when the system row is inserted.
- Inserts three planets only when each exact planet id is missing.
- Current seeded system:
  - `Helios Gate`
  - slot `1`: `Aurelia`, `Terran`, size `118`, colonized
  - slot `2`: `Cinder Reach`, `Desert`, size `94`, unowned in this seed
  - slot `3`: `Aether Crown`, `GasGiant`, size `160`, unowned in this seed

### Economy and colony state

- Ensures one `PlanetResourceStockpile` row exists for `Aurelia`.
- If that row already exists, it is topped up only to these minimums:
  - credits `125`
  - metal `160`
  - crystal `100`
  - gas `50`
- Inserts one `PlanetProductionProfile` for `Aurelia` only when missing:
  - credits per hour `18`
  - metal per hour `14`
  - crystal per hour `6`
  - gas per hour `3`
- Inserts one `PlanetPopulationProfile` for `Aurelia` only when missing:
  - population `2000`
  - workforce `500`
  - military `100`

### Buildings and capacity

- Inserts these `PlanetBuilding` rows only when the exact planet and building type pair is missing:
  - `Aurelia`: `CommandCenter` level `4`, footprint `1`
  - `Aurelia`: `HabitationDistrict` level `3`, footprint `1`
  - `Aurelia`: `Shipyard` level `1`, footprint `1`
  - `Cinder Reach`: `MetalMine` level `6`, footprint `1`
  - `Cinder Reach`: `Shipyard` level `2`, footprint `1`
- Inserts `PlanetBuildingCapacity` only when missing:
  - `Aurelia`: base `120`
  - `Cinder Reach`: base `120`

### Construction-related seeded state

- No construction queue rows are created.
- The seed relies on the existing building catalog plus `Aurelia` stockpile, buildings, capacity, and empty queue to show:
  - at least one available construction action
  - at least one blocked construction action
- Reapplying the seed does not clear existing construction orders.

### Research-related seeded state

- No `ResearchOrder` rows are created.
- No `ResearchProject` rows are created.
- The research cockpit baseline depends on:
  - the seeded civilization existing
  - `Aurelia` being owned and selectable
  - `Aurelia` stockpile minimums being present
  - the research queue starting empty in a fresh database
- Reapplying the seed does not clear existing research orders and does not remove existing research projects.

### Shipyard-related seeded state

- No `AssetProductionOrder` rows are created.
- Inserts one orbital stock row only when the exact `Aurelia + EscortCraft` row is missing:
  - `Aurelia`: `EscortCraft x4`
- Shipyard availability depends on seeded resource stockpile, `Shipyard` level `1`, and `PlanetPopulationProfile`.
- Reapplying the seed does not clear existing orbital production orders and does not top up existing orbital stock quantities.

### Fleet and transfer seeded state

- Inserts these stationed groups only when an exact matching row is missing:
  - `ScoutCraft x3` stationed at `Aurelia`
  - `ScoutCraft x2` stationed at `Aurelia`
  - `EscortCraft x4` with origin `Aurelia`, current planet `Aether Crown`
- Ensures one cargo group matching `CargoCraft x2` at `Aurelia` exists, then reserves it when possible.
- Inserts one planned transfer only when a matching planned row for that reserved cargo group is missing:
  - origin `Aurelia`
  - destination `Cinder Reach`
  - distance units `2`
  - departure `2026-06-02T08:00:00Z`
  - arrival `2026-06-02T12:00:00Z`

## Visibility and read-model consequences

- Strategic-map visibility treats `Aurelia` as owned.
- The seeded `Helios Gate` system becomes visible because it contains the owned planet.
- `Cinder Reach` and `Aether Crown` are visible comparison planets in the same system, but not owned by the seeded civilization.
- Fleet and transfer overlays become visible in that same seeded system.

## Cockpit dependency matrix

| Screen | Deterministic URL | Seeded rows it depends on | Queue expectation |
|---|---|---|---|
| Galaxy | `/?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, `Helios Gate`, all three planets, `Aurelia` ownership, seeded orbital groups, planned transfer | no queue dependency |
| Planet | `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | owned planet, stockpile, production profile, building capacity, seeded buildings, ownership, orbital counts derived from groups and transfer | construction queue should start empty in a fresh database |
| Construction | `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | same `Aurelia` colony rows as Planet, especially stockpile, buildings, and capacity | construction queue should start empty in a fresh database |
| Research | `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, `Helios Gate` context, `Aurelia` stockpile | research queue and research projects should both start empty in a fresh database |
| Shipyard | `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, `Aurelia` stockpile, `Shipyard` building, population profile, `EscortCraft x4` orbital stock row | orbital production queue should start empty in a fresh database |
| Fleets | `/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, all seeded orbital groups, planned transfer, destination planets `Cinder Reach` and `Aether Crown`, current stockpile for estimate and create-transfer costs | no seeded queue; one planned transfer already exists |

## Reapply behavior by subsystem

| Subsystem | Reapply behavior today |
|---|---|
| Player profile, civilization, galaxy, system, planets, ownership | inserts only missing rows |
| `Aurelia` stockpile | inserts missing row or tops existing row up to minimums; never decreases higher balances |
| Production profile, population profile, building capacity, buildings | inserts only missing rows; existing altered values are preserved |
| Construction queue | preserved |
| Research queue | preserved |
| Research projects | preserved |
| Orbital production queue | preserved |
| Orbital asset stock | inserts missing `EscortCraft` row only; existing quantity is preserved |
| Stationed orbital groups | inserts a new baseline row if the exact expected row is no longer present |
| Cargo transfer group | reserves the matching stationed cargo group when found; creates a new reserved baseline cargo group if the expected match is gone |
| Planned transfer | inserts a new planned baseline transfer when the matching row for the baseline cargo group is gone |

## Practical reset guidance

- Safe to reapply when you need missing baseline identity, map, ownership, shipyard, stockpile, or fleet-validation rows to exist again.
- Not a destructive reset:
  - it does not delete extra groups
  - it does not delete extra transfers
  - it does not clear any queue
  - it does not remove completed research projects
  - it does not restore altered building levels
  - it does not restore altered orbital stock quantities
- Use a fresh disposable local database when you need the exact original empty-queue and pre-mutation baseline.
