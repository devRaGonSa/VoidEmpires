# Ship Catalog Final DB Readiness

This note prepares the current ship/orbital catalog for a later final database seed pass. It documents accepted orbital asset rows, roles, costs, build-time behavior, requirements, image keys, and seed requirements.

It does not add migrations, seed rows, generated images, combat, fleet movement, market transactions, alliance mutations, or production-auth behavior.

## Current Authority

- `OrbitalAssetCatalog` defines the accepted `SpaceAssetType`, requirement, production cost, storage capacity, and operating range.
- `AssetProductionQueueService` owns the real Development enqueue path for orbital production, deducts source-planet resources immediately, and creates one active order per planet.
- `DevShipyardUiStateService` exposes the current Shipyard read model, availability reasons, estimated duration, local orbital stock, and enqueue command metadata.
- `OrbitalAssetStock` stores local completed orbital stock. Fleet group creation and stock-to-fleet allocation remain separate development endpoints and are not part of this catalog.
- Frontend labels, categories, and roles currently live in `shipyardPresentation.ts`; final DB metadata should eventually replace or validate those fallbacks.

## Current Orbital Asset Rows

| Key | Spanish label | Category | Role | Requirement | Base cost | Build time | Storage | Range | Placeholder image key | Balance note |
|---|---|---|---|---|---:|---|---:|---:|---|---|
| `ScoutCraft` | Nave exploradora | Exploracion | Reconocimiento rapido | Shipyard 1, crew 25 | C0 M120 X80 G40 | 3 min/unit | 0 | 3 | `ship.scout-craft` | Smallest current ship; supports scouting readiness only, not accepted movement execution. |
| `CargoCraft` | Nave de carga | Logistica | Transporte de suministros | Shipyard 1, crew 60 | C0 M250 X120 G80 | 3 min/unit | 1000 | 2 | `ship.cargo-craft` | Storage metadata exists, but market and trade-route execution remain out of scope. |
| `EscortCraft` | Nave de escolta | Escolta | Cobertura orbital | FleetCommandCenter 1, crew 120 | C0 M500 X250 G150 | 3 min/unit | 100 | 4 | `ship.escort-craft` | Combat stats are not modeled; current row is production and readiness metadata only. |
| `ColonyCraft` | Nave colonial | Colonial | Expansion y asentamiento | LogisticsHub 2, crew 500 | C0 M1500 X800 G500 | 3 min/unit | 500 | 5 | `ship.colony-craft` | Expansion behavior is not productized; do not imply colonization execution from this row alone. |

Cost shorthand: `C` credits, `M` metal, `X` crystal, `G` gas.

## Current Production Rules

The current Development enqueue path uses these backend-owned checks:

- Planet id is required and the request timestamp must be UTC.
- Quantity must be positive.
- Only one pending or active asset production order is supported per planet.
- Asset type must exist for the selected `AssetProductionTarget`.
- Source planet must have a resource stockpile with enough resources for base cost multiplied by quantity.
- Source planet must have the required building at or above the required level.
- Source planet must have a population profile.
- Orbital production checks local ship crew capacity using current buildings and population.
- The current build-time rule is `3 minutes * quantity`.

The Shipyard cockpit currently exposes only orbital options. Planetary asset rows belong to the adjacent Ground Army readiness path and should be handled by a separate catalog seed review if they are promoted to product catalog metadata.

## Final Seed Metadata Required

Each final database ship/orbital catalog row should include:

1. Stable key matching `SpaceAssetType`.
2. Spanish display label and short Spanish card description.
3. Category key, category label, role key, and role label.
4. Owning module or cockpit.
5. Placeholder image key from the table above.
6. Nullable final asset id for the generated asset phase.
7. Production target: `Orbital`.
8. Base cost with canonical resource keys.
9. Build-time policy; current behavior is 3 minutes per unit.
10. Required building type and level as structured keys.
11. Required operator capacity and future crew/capacity policy metadata.
12. Storage capacity and operating range semantics.
13. Fleet handoff policy: stock row, fleet group creation, or future allocation rule.
14. Combat, movement, colonization, cargo, or support effect metadata only after those systems are accepted.
15. Sort order and recommended Shipyard display grouping.
16. Version or revision marker for future balance changes.

## Current Gaps Before Final DB

- Labels, categories, and roles are frontend presentation fallbacks, not backend catalog metadata.
- Storage and operating range exist in the domain definition but do not yet execute cargo, movement, or colonization gameplay.
- Combat-facing concepts such as escort coverage and weapon/shield behavior are not accepted product behavior yet.
- There is no final max stock, maintenance, upkeep, fuel, speed, combat stat, or fleet-slot policy.
- Final generated images are not present; placeholders must stay deterministic from `SpaceAssetType`.
- Seed validation should prove every `SpaceAssetType` enum has exactly one metadata row and every row maps back to a known enum value.

## Seed Phase Requirements

- Add final ship/orbital catalog rows only in a dedicated final DB/model consolidation task.
- Keep catalog metadata separate from `OrbitalAssetStock`, `AssetProductionOrder`, and fleet group state.
- Preserve backend-owned resources, queue state, and stock as authoritative gameplay state.
- Do not use catalog metadata to fake available ships, local stock, fleet groups, cargo, range, movement, or combat readiness.
- Keep production auth, fleet movement productization, combat, market, and alliance behavior outside ship catalog metadata.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, DB migration, final asset generation, or integration validation was performed for this note.
