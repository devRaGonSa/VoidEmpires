# Development Seed Profiles

This document is the source of truth for the current Development-only seed profile system.
Use these profiles instead of manual SQL for standard cockpit QA.

## Profile catalog

`POST /api/dev/seeds/apply` remains Development-only and accepts exact profile names only. There are no undocumented aliases.

| Profile | Status | Intended QA use |
|---|---|---|
| `minimal-validation` | Implemented | Current deterministic shared baseline for Galaxy, Planet, Construction, Research, Shipyard, Fleets, and Market |
| `cockpit-validation` | Implemented | Richer combined cockpit baseline with non-blocking completed history plus visible Market, Defenses, Ground Army, Espionage, and Alliance read-only readiness on Aurelia |
| `shipyard-validation` | Implemented | Shipyard-focused richer baseline with completed queue history, two local stock rows, one available hull, and blocked comparisons |
| `fleet-validation` | Implemented | Fleet-focused richer baseline with one extra stationed cargo example and one additional due active transfer |
| `research-validation` | Implemented | Research-focused richer baseline with one deterministic available technology, completed history, and truthful resource-blocked comparisons |
| `planet-full-validation` | Implemented | Planet and Construction richer baseline with extra general buildings and completed queue history |

- Endpoint: `POST /api/dev/seeds/apply`
- Discovery endpoint: `GET /api/dev/seeds/profiles`
- Request body:

```json
{
  "profile": "minimal-validation"
}
```

Unsupported profile requests fail safely. The current response now includes the requested profile name, the applied profile metadata when successful, and the known profile catalog so PowerShell or JSON callers can discover the supported naming contract directly.
If persisted development state still triggers a database write conflict, the endpoint now returns `409 Conflict` with diagnostic error text instead of an unhandled runtime failure.

## Quick start

Apply a profile:

```powershell
Invoke-RestMethod `
  -Method Post `
  -Uri "http://localhost:5142/api/dev/seeds/apply" `
  -ContentType "application/json" `
  -Body '{"profile":"cockpit-validation"}'
```

Discover available profiles:

```powershell
Invoke-RestMethod `
  -Method Get `
  -Uri "http://localhost:5142/api/dev/seeds/profiles"
```

Operational guidance:

- Seed profiles are Development-only, deterministic, and idempotent.
- Reapply the documented profile when local QA state becomes confusing.
- Reapplying `cockpit-validation`, `shipyard-validation`, `research-validation`, or `planet-full-validation` is supported on reused development databases even after manual QA has already created queue rows.
- `cockpit-validation` is now also verified against real manual Construction and Research orders created through the supported dev endpoints; reapplying the profile preserves those active manual rows while still avoiding duplicate seeded history rows.
- `cockpit-validation` is now also verified against a real manual Shipyard enqueue on a reused Development database; reapplying the profile preserves the active orbital production order, avoids duplicating seeded completed Shipyard history, and keeps Shipyard UI-state readable afterward.
- `cockpit-validation` now also seeds one deterministic diplomatic contact row for the requesting civilization so Alliance can render a stable read-only contact baseline without creating any real alliance, pact, invitation, or membership state.
- Richer profiles reserve high sequence ranges for their completed queue-history seed rows, avoiding collisions with pre-existing manual queue activity without resetting the database.
- Do not use manual SQL for the standard Galaxy, Planet, Construction, Research, Shipyard, Fleet, or Market QA flows.
- Use a fresh disposable local database only when you need the exact original pre-mutation baseline.
- Pair Galaxy QA with `docs/dev/strategic-map-cockpit-checklist.md` so the route, expected seeded names, and shell-only regression checks stay aligned.

Backend-only persisted QA helpers:

- `.\scripts\dev-qa-baseline.ps1`
  - defaults to `http://localhost:5142`
  - applies `cockpit-validation` twice and prints the current Construction, Research, Shipyard, and Fleet baseline snapshots
- `.\scripts\dev-qa-create-construction-order.ps1`
  - defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`
  - add `-ApplySeed` to apply `planet-full-validation` before enqueueing one real Construction order
- `.\scripts\dev-qa-create-research-order.ps1`
  - defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`
  - add `-ApplySeed` to apply `research-validation` before enqueueing one real Research order
- `.\scripts\dev-qa-create-shipyard-production-order.ps1`
  - defaults to civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`
  - add `-ApplySeed` to apply `cockpit-validation` before enqueueing one real Shipyard order
- `.\scripts\dev-qa-fleet-read-state.ps1`
  - defaults to civilization `00000000-0000-0000-0000-000000000001`
  - re-reads Fleet UI state only and prints group, stationed, transfer, and resource-context summaries without mutating anything
- There is intentionally no `.\scripts\dev-qa-create-orbital-group-from-stock.ps1` helper in this block because stock-to-fleet allocation remains excluded from the accepted reused-database QA loop.

All five helpers create or inspect Development-only persisted state. Do not run them against production.

The discovery endpoint is Development-only and returns a concise list of all known profiles with:

- `name`
- `description`
- `destructive` set to `false`
- `deterministic` set to `true`
- intended cockpits
- recommended QA URLs
- key ids

Example:

```json
{
  "succeeded": true,
  "profiles": [
    {
      "name": "minimal-validation",
      "description": "Implemented today. Use this for the current deterministic QA baseline.",
      "destructive": false,
      "deterministic": true,
      "intendedCockpits": ["Galaxy", "Planet", "Construction", "Research", "Shipyard", "Fleets"],
      "recommendedQaUrls": ["/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"],
      "keyIds": []
    }
  ],
  "errors": []
}
```

## QA routes

Primary deterministic local QA routes:

- Galaxy: `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Planet: `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Construction: `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Research: `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Shipyard: `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Fleets: `/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Market: `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Espionage: `/espionage?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Alliance: `/alliance?civilizationId=00000000-0000-0000-0000-000000000001`
- Ground Army placeholder: `/ground-army?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- Defenses: `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`

Route note:

- `/galaxy` is the canonical Galaxy route. `/` remains supported as a compatibility alias.
- Ground Army now has a seeded cockpit-foundation readiness baseline, but it still stays scoped to readiness and explicit training preparation only.
- Defenses now has a seeded cockpit-foundation readiness baseline, but it still does not execute combat or seed an active defensive queue.

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

`cockpit-validation` builds on `minimal-validation`, tops `Aurelia` up to at least `220` credits, `320` metal, `220` crystal, and `120` gas, adds a visible `DefenseGrid` level `1` plus `Barracks` level `1` on `Aurelia`, and then adds one completed construction row, one completed `EnergySystems` research order plus project, one completed orbital `ScoutCraft` production order plus local `ScoutCraft` stock, and one completed planetary `PatrolGroup x2` training row plus local `PatrolGroup x2` stock. It stays non-destructive and avoids extra pending or active queue rows so the current executable cockpit actions remain available.

Use it as one shared demo story: `Void Seed Civilization` controls `Aurelia` in `Helios Gate`, while `Cinder Reach` and `Aether Crown` stay visible as nearby comparison worlds. Galaxy, Planet, Construction, Research, Shipyard, Fleets, Market, Defenses, Ground Army, and Espionage should all read as different views of that same prepared colony rather than unrelated fixtures.

Expected Alliance result for screenshot QA:

- canonical route: `/alliance?civilizationId=00000000-0000-0000-0000-000000000001`
- `Void Seed Civilization` loads as the diplomatic identity with `Aurelia` as the known homeworld context
- no active alliance is shown for the requesting civilization
- one deterministic diplomatic contact remains visible for the read-only contact catalog
- future pact and future action placeholders remain visible and disabled
- no alliance, pact, invitation, membership, or messaging mutation is seeded

Expected Market result for screenshot QA:

- canonical route: `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
- `Aurelia` loads as the active colony inside `Helios Gate`
- the first viewport reads as a read-only economy cockpit, not as a transaction console
- civilization and local reserves are non-zero and derived from the seeded owned-planet stockpile baseline
- production remains visible for the selected colony
- advisory reference ratios are present for the seeded resource set
- at least one economy signal plus the disabled future-route signal remain visible
- all future Market actions remain disabled placeholders
- the cockpit stays honest about being read-only and advisory only

Expected Galaxy result for screenshot QA:

- one visible strategic system: `Helios Gate`
- three visible planets in that system: owned `Aurelia`, visible comparison `Cinder Reach`, and visible comparison `Aether Crown`
- one owned-system summary row with non-zero visible, owned, fleet-marker, and transfer-marker counts
- four fleet markers from the seeded stationed and reserved orbital groups
- one transfer overlay from the seeded planned cargo route
- enough read-state for the legend, focus panel, transfer summary, and collapsed diagnostics to render together without looking empty

Expected Defenses result for screenshot QA:

- `Aurelia` loads as the controlled planet with the richer cockpit-validation stockpile
- one visible `DefenseGrid` structure appears in the deployed/readiness inventory
- one deterministic defense readiness option remains visible for `DefenseGrid`
- the baseline stays truthful about current scope: no seeded active defense queue and complete-due remains a limitation, not a fake action
- blocked defense comparisons are not seeded in the default baseline because the current defensive catalog exposes only one real structure type; blocked-state coverage is validated through targeted low-resource tests instead

Expected Ground Army result for screenshot QA:

- `Aurelia` loads as the controlled planet with the richer cockpit-validation stockpile and population profile
- one visible `Barracks` structure appears in the ground-readiness inventory
- one deterministic `PatrolGroup` option is available
- blocked comparisons remain visible for `ExpeditionGroup`, `VehicleGroup`, and `SupportGroup`
- the garrison shows one local `PatrolGroup x2` row
- the queue shows one completed planetary training history row and no open ground order
- the baseline stays truthful about current scope: complete-due remains a limitation, not a fake action, and no combat or invasion state is seeded

Expected Espionage result for screenshot QA:

- `Helios Gate` loads as the shared strategic context with owned `Aurelia` plus visible `Cinder Reach` and `Aether Crown`
- the target catalog shows at least one owned target and at least one visible comparison target
- passive readings include at least one transfer-derived signal from the seeded planned cargo route
- future mission placeholders remain visible but disabled
- the cockpit stays read-only and does not imply mission execution

## `shipyard-validation` additions

`shipyard-validation` builds on `minimal-validation`, tops `Aurelia` up to at least `180` credits, `180` metal, `110` crystal, and `70` gas, adds one completed `ScoutCraft` production row, and adds a second local orbital stock type so Shipyard can show queue history plus richer stock.

Expected Shipyard result:

- `ScoutCraft` remains available.
- `CargoCraft` is blocked by `InsufficientResources`.
- `EscortCraft` and `ColonyCraft` remain blocked by missing building requirements.
- Local orbital stock shows at least `EscortCraft x4` and `ScoutCraft x1`.
- The queue shows one completed production row and no open order, so guarded enqueue remains available.

## `fleet-validation` additions

`fleet-validation` builds on `minimal-validation`, tops `Aurelia` up to at least `260` credits, `260` metal, `160` crystal, and `100` gas, adds one extra stationed `CargoCraft x1` at `Aurelia`, and adds one second active cargo transfer from `Aurelia` to `Aether Crown` with arrival `2026-06-01T10:00:00Z`.

Expected Fleet result:

- multiple stationed groups remain at `Aurelia`
- one active transfer remains the standard in-flight baseline
- one additional planned transfer is already due for `complete-due` QA
- one controlled fleet group remains stationed away from `Aurelia`
- origin-planet resource context remains visible for travel estimate and transfer actions

## `research-validation` additions

`research-validation` builds on `minimal-validation`, resets `Aurelia` to `125` credits, `110` metal, `70` crystal, and `30` gas, and adds one completed `EnergySystems` research project plus one matching completed research order history row.

Expected Research result:

- `PlanetaryEngineering` remains the single deterministic available technology
- `ResourceExtraction`, `EnergySystems`, and other higher-cost entries remain blocked by `InsufficientResources`
- the queue shows one completed history row and no active order before a manual enqueue
- the projects list shows one completed `EnergySystems` project
- the primary enqueue smoke path remains available for one successful `PlanetaryEngineering` order

Current limitation:

- Research readiness does not currently model hidden prerequisites or multiple gameplay blocker categories.
- Richer research QA variety therefore comes from completed history plus truthful cost differences, not invented unlock mechanics.

## `planet-full-validation` additions

`planet-full-validation` builds on `minimal-validation`, tops `Aurelia` up to at least `240` credits, `220` metal, `140` crystal, and `90` gas, adds visible `SolarPlant` and `MetalMine` rows on `Aurelia`, and adds one completed `SolarPlant` upgrade row to the construction history.

Expected Planet and Construction result:

- `/planet` stays a dashboard, not a full construction catalog
- `/construction` stays scoped to general infrastructure
- visible building inventory is richer than the minimal baseline
- the queue shows one completed construction-history row and no open order
- at least one general construction action remains available while several remain blocked

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
| Galaxy | `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, `Helios Gate`, all three planets, `Aurelia` ownership, seeded orbital groups, planned transfer | no queue dependency |
| Planet | `/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | owned planet, stockpile, production profile, building capacity, seeded buildings, ownership, orbital counts derived from groups and transfer | construction queue should start empty in a fresh database |
| Construction | `/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | same `Aurelia` colony rows as Planet, especially stockpile, buildings, and capacity | construction queue should start empty in a fresh database |
| Research | `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, `Helios Gate` context, `Aurelia` stockpile | research queue and research projects should both start empty in a fresh database |
| Shipyard | `/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, `Aurelia` stockpile, `Shipyard` building, population profile, `EscortCraft x4` orbital stock row | orbital production queue should start empty in a fresh database |
| Fleets | `/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, all seeded orbital groups, planned transfer, destination planets `Cinder Reach` and `Aether Crown`, current stockpile for estimate and create-transfer costs | no seeded queue; one planned transfer already exists |
| Market | `/market?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, owned planet, `Aurelia` stockpile, production profile, visible `Helios Gate` context, seeded transfer counts, deterministic advisory ratios and future disabled actions from the market read model | no executable queue; all actions stay disabled |
| Espionage | `/espionage?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001` | civilization, `Helios Gate`, owned `Aurelia`, visible `Cinder Reach`, visible `Aether Crown`, seeded orbital groups, planned transfer, current strategic visibility | no mission queue or active espionage execution |
| Alliance | `/alliance?civilizationId=00000000-0000-0000-0000-000000000001` | civilization, homeworld identity, one deterministic diplomatic contact row, future pact placeholders, future action placeholders | no alliance membership, pact execution, invitation, or messaging rows |

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
| Completed queue-history rows added by richer profiles | inserted only when the matching logical history row is missing; reserved high sequence ranges avoid collisions with manual QA rows |
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
