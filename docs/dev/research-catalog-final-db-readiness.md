# Research Catalog Final DB Readiness

This note prepares the current research catalog for a later final database seed pass. It documents accepted technology rows, category labels, level behavior, prerequisite gaps, placeholder image keys, and seed requirements.

It does not add migrations, seed rows, generated images, combat, fleet movement, market transactions, alliance mutations, or production-auth behavior.

## Current Authority

- `ResearchCatalog` defines the accepted `ResearchType`, base cost, and bonus key.
- `ResearchProject` stores civilization-owned completed research levels. Creating a project starts at level 1 and upgrades increment the level.
- `ResearchOrder` stores queued research work with source planet, target level, sequence, UTC start/end timestamps, and status.
- `ResearchEnqueueReadinessEvaluator` derives target level as `currentLevel + 1` and multiplies base cost by target level.
- `ResearchQueueService` owns the real Development enqueue path and deducts source-planet resources immediately on success.
- `DevResearchUiStateEndpoints` exposes read-state hints for the Research cockpit. It currently derives only source planet, queue slot, and stockpile readiness.
- Frontend research labels and categories currently live in `researchPresentation.ts`; final DB metadata should eventually replace or validate those fallbacks.

## Current Technology Rows

| Key | Spanish label | Category | Base cost | Bonus key | Placeholder image key | Current level rule | Balance note |
|---|---|---|---:|---|---|---|---|
| `PlanetaryEngineering` | Ingenieria planetaria | Colonizacion | C0 M100 X50 G0 | `planet_capacity` | `research.planetary-engineering` | Target level is current + 1 | Adds planet capacity through code-backed bonus calculation; no final prerequisite chain yet. |
| `ResourceExtraction` | Extraccion de recursos | Economia | C0 M120 X80 G0 | `resource_output` | `research.resource-extraction` | Target level is current + 1 | Production multiplier is real for economy ticks; final seed should describe affected resources. |
| `EnergySystems` | Sistemas energeticos | Energia | C0 M100 X100 G20 | `energy_output` | `research.energy-systems` | Target level is current + 1 | Currently shortens research duration; broader energy output effects are not final. |
| `ConstructionAutomation` | Automatizacion de construccion | Administracion | C0 M200 X150 G50 | `build_speed` | `research.construction-automation` | Target level is current + 1 | Current construction duration multiplier is real; final seed should state the scaling curve. |
| `Propulsion` | Propulsion | Logistica | C0 M250 X200 G75 | `fleet_speed` | `research.propulsion` | Target level is current + 1 | Fleet speed is a readiness concept only until movement productization lands. |
| `ShipWeapons` | Armas de nave | Militar espacial | C0 M300 X250 G50 | `weapon_damage` | `research.ship-weapons` | Target level is current + 1 | No combat execution is attached to this catalog row yet. |
| `Shielding` | Escudos | Defensa | C0 M250 X300 G50 | `shield_strength` | `research.shielding` | Target level is current + 1 | Defenses cockpit does not apply active mitigation from this row today. |
| `Espionage` | Espionaje | Exploracion | C0 M150 X200 G100 | `intel_strength` | `research.espionage` | Target level is current + 1 | Espionage cockpit remains read-only; missions, sabotage, and counter-espionage are out of scope. |

Cost shorthand: `C` credits, `M` metal, `X` crystal, `G` gas.

## Current Prerequisites And Availability

The current Research cockpit and enqueue path use a small readiness set:

- Active civilization id and a source planet owned by that civilization.
- Source planet resource stockpile.
- No pending or active research order for the civilization.
- Enough source-planet resources for base cost multiplied by target level.
- Valid UTC request timestamp for enqueue.

There is no accepted final prerequisite tree yet. The UI currently shows generic requirement keys: `SourcePlanet`, `ResearchQueueSlot`, and `ResourceStockpile`.

## Final Seed Metadata Required

Each final database catalog row should include:

1. Stable key matching `ResearchType`.
2. Spanish display label and short Spanish card description.
3. Category key and Spanish category label.
4. Owning module or cockpit.
5. Placeholder image key from the table above.
6. Nullable final asset id for the generated asset phase.
7. Base cost with canonical resource keys.
8. Cost scaling policy; current behavior is base cost multiplied by target level.
9. Duration scaling policy; current base duration is 10 minutes multiplied by target level, modified by `EnergySystems`.
10. Max level or unlimited-level policy.
11. Structured prerequisite keys for required buildings, previous research, civilization state, or source planet state.
12. Structured effect metadata for production, capacity, duration, fleet, combat, defense, or espionage impacts once accepted.
13. Queue policy metadata: one active/pending research order per civilization today.
14. Sort order and recommended path metadata for the Research cockpit.
15. Version or revision marker for future balance changes.

## Current Gaps Before Final DB

- Categories, labels, and requirement labels are frontend presentation fallbacks, not backend catalog metadata.
- Bonus keys are present, but several effects are readiness-only because combat, movement, and espionage execution are not accepted product behavior yet.
- No explicit max level, prerequisite chain, unlock graph, or final research tree exists.
- Research project storage is civilization state and must remain separate from catalog rows.
- Existing Development seed profiles set QA state; they should not become final catalog seed ownership or progression data.
- Seed validation should prove every `ResearchType` enum has exactly one metadata row and every metadata row maps to a known enum value.

## Seed Phase Requirements

- Add final catalog rows only in a dedicated final DB/model consolidation task.
- Keep final generated assets nullable and separate from ordinary backend tests.
- Preserve backend-owned resources, queue state, and completed research levels as authoritative gameplay state.
- Do not use catalog metadata to fake available technologies, completed levels, stockpile quantities, or queue rows.
- Keep production auth, player ownership, combat, fleet movement, market, and alliance behavior outside research catalog metadata.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, DB migration, final asset generation, or integration validation was performed for this note.
