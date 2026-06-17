# Building Catalog Final DB Readiness

This note prepares the current building catalog for a later final database seed pass. It documents the accepted static rows, metadata gaps, placeholder image keys, balance notes, and seed requirements.

It does not add migrations, seed rows, generated images, combat, fleet movement, market transactions, alliance mutations, or production-auth behavior.

## Current Authority

The gameplay authority for building definitions is still code-backed:

- `BuildingCatalog` defines the accepted `BuildingType`, initial level, footprint, base construction cost, and `BuildingCategory`.
- `DevPlanetUiStateService` formats Spanish labels and derives construction actions, cost multipliers, availability, capacity checks, and estimated durations from the domain catalog plus current backend state.
- `PlanetBuilding`, `PlanetConstructionOrder`, and `PlanetBuildingCapacity` remain persisted gameplay state; the final catalog seed must describe content, not replace owned planet state.
- Frontend Planet and Construction pages consume backend read models. They must not fabricate building availability, resources, queue rows, or completed structures from catalog metadata alone.

## Current Building Rows

| Key | Spanish label | Category | Initial level | Footprint | Base cost | Placeholder image key | Balance note |
|---|---|---:|---:|---:|---:|---|---|
| `CommandCenter` | Centro de mando | Civilian | 1 | 20 | C0 M500 X250 G0 | `building.command-center` | Large footprint anchor; should stay central to colony identity and capacity decisions. |
| `MetalMine` | Mina de metal | Industrial | 1 | 5 | C0 M60 X15 G0 | `building.metal-mine` | Low footprint and early cost make it the baseline metal growth path. |
| `CrystalMine` | Mina de cristal | Industrial | 1 | 5 | C0 M50 X40 G0 | `building.crystal-mine` | Similar footprint to MetalMine, with a stronger crystal bias for research and shipyard costs. |
| `GasExtractor` | Extractor de gas | Industrial | 1 | 6 | C0 M75 X30 G20 | `building.gas-extractor` | Gas is present in the cost, so seed stockpiles must support first construction tests. |
| `SolarPlant` | Planta solar | Industrial | 1 | 4 | C0 M40 X10 G0 | `building.solar-plant` | Smallest current footprint; final effects should clarify energy production and caps. |
| `ResearchLab` | Laboratorio de investigacion | Research | 1 | 12 | C0 M200 X120 G25 | `building.research-lab` | Research readiness depends on this row, but research catalog effects stay separate. |
| `Shipyard` | Astillero | MilitarySpace | 1 | 15 | C0 M250 X100 G50 | `building.shipyard` | Shipyard unlocks production readiness only through backend read models, not catalog presence alone. |
| `DefenseGrid` | Malla defensiva | Defense | 1 | 10 | C0 M150 X75 G0 | `building.defense-grid` | Current Defenses cockpit reads construction readiness, not combat execution. |
| `HabitationDistrict` | Distrito habitacional | Civilian | 1 | 8 | C0 M120 X60 G0 | `building.habitation-district` | Population and capacity effects need final explicit seed metadata. |
| `MedicalCenter` | Centro medico | Civilian | 1 | 6 | C0 M160 X120 G20 | `building.medical-center` | Health or growth effects are not yet final; keep them described as metadata gaps. |
| `MilitaryAcademy` | Academia militar | MilitaryGround | 1 | 10 | C0 M220 X140 G20 | `building.military-academy` | Ground readiness only; no combat or invasion behavior is implied. |
| `Barracks` | Barracones | MilitaryGround | 1 | 9 | C0 M180 X90 G10 | `building.barracks` | Supports current Ground Army readiness and seed baselines. |
| `CrewAcademy` | Academia de tripulacion | MilitarySpace | 1 | 10 | C0 M240 X160 G40 | `building.crew-academy` | Crew capacity effects should be seeded as structured requirements/effects later. |
| `FleetCommandCenter` | Mando de flota | MilitarySpace | 1 | 14 | C0 M320 X220 G60 | `building.fleet-command-center` | Fleet command readiness is separate from fleet movement productization. |
| `LogisticsHub` | Centro logistico | Logistics | 1 | 12 | C0 M200 X100 G30 | `building.logistics-hub` | Shared logistics dependency for military and shipping readiness; final seed should keep module ownership explicit. |

Cost shorthand: `C` credits, `M` metal, `X` crystal, `G` gas.

## Final Seed Metadata Required

Each final database catalog row should include:

1. Stable key matching `BuildingType`.
2. Spanish display label.
3. Short Spanish card description.
4. Category key matching `BuildingCategory`.
5. Owning cockpit/module: Planet, Construction, Research, Shipyard, Defenses, Ground Army, Fleets, or Logistics.
6. Placeholder image key from the table above.
7. Nullable final asset id for the generated asset phase.
8. Initial level and base footprint.
9. Base cost with canonical resource keys.
10. Upgrade cost multiplier policy; current read model uses target level as the upgrade multiplier.
11. Duration policy; current read model estimates from target level and construction automation.
12. Capacity policy for construct actions.
13. Requirement/unlock metadata as structured keys, not prose-only notes.
14. Structured effect metadata for production, population, capacity, crew, defense, or logistics effects once accepted.
15. Sort order for catalog display.
16. Version or revision marker for future balance changes.

## Current Gaps Before Final DB

- Labels are duplicated in presentation helpers instead of catalog metadata.
- Building descriptions, module ownership, sort order, and final asset IDs are not stored with the domain definitions.
- Effects are partly inferred by downstream services and are not a unified catalog contract.
- Upgrade and duration policies are code behavior, not seed metadata.
- Defense, ground army, shipyard, and logistics readiness consume building state, but do not have final combat, movement, allocation, or logistics execution attached.
- Seed validation should verify that every `BuildingType` enum has exactly one final catalog row and that every row maps back to a domain key.

## Seed Phase Requirements

- Add catalog rows only in a dedicated final DB/model consolidation task.
- Keep seed data separate from `PlanetBuilding` ownership rows and construction queue state.
- Preserve current Development seed profiles as gameplay-state setup; do not use catalog metadata to create fake buildings or stock.
- Include a migration or seed validator that fails on enum/catalog drift.
- Keep final generated image assets optional and nullable until the asset pass lands.
- Keep production account/civilization ownership outside building catalog metadata.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, DB migration, final asset generation, or integration validation was performed for this note.
