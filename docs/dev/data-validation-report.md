# Data Validation Report

Status date: 2026-06-18

This report summarizes current catalog completeness and the remaining data-validation risks before the final DB/catalog phase.

It does not add schema changes, seed validators, SQL Server actions, generated assets, or gameplay expansion.

## Validation Scope

Reviewed sources:

- code-owned catalogs in `BuildingCatalog`, `ResearchCatalog`, `OrbitalAssetCatalog`, and `PlanetaryAssetCatalog`
- persisted resource enum `ResourceType`
- current Development seed documentation and deterministic ids
- current readiness docs for buildings, research, ships, defenses, resources, assets, and balance

## Catalog Count Snapshot

| Area | Current authoritative source | Entry count | Validation status today |
|---|---|---:|---|
| Buildings | `BuildingCatalog` / `BuildingType` | 15 | count appears complete and aligned in code |
| Research | `ResearchCatalog` / `ResearchType` | 8 | count appears complete and aligned in code |
| Orbital assets | `OrbitalAssetCatalog` / `SpaceAssetType` | 4 | count appears complete and aligned in code |
| Planetary assets | `PlanetaryAssetCatalog` / `PlanetaryAssetType` | 4 | count appears complete in code, but final metadata is incomplete |
| Defenses | construction-backed `DefenseGrid` readiness | 1 | only one modeled defense row exists |
| Persisted spendable resources | `ResourceType` | 4 | count appears complete for persisted spendable resources |
| Visible non-stockpile resource signals | frontend/readiness docs | 3 | visible labels exist, but they are not final relational metadata |
| Seed civilization identity | `DevelopmentSeedProfiles` | 1 | Development-only identity, not a final faction catalog |
| Planet archetypes | `PlanetType` / `PlanetVisualProfileCatalog` | 7 | stable archetype set exists for visual identity, not a final content catalog |

## Metadata Completeness

### Buildings

- Current strength: stable keys, costs, categories, footprint, and readiness usage are documented.
- Missing metadata: short descriptions, module ownership, final asset ids, explicit upgrade metadata, structured prerequisites, structured effects, sort order, revision marker.
- Image-key status: documented for all 15 building rows.
- Validation risk: final DB seed could drift from `BuildingType` unless a row-per-enum validator is added.

### Research

- Current strength: stable keys, base costs, and bonus keys are documented.
- Missing metadata: final prerequisite graph, max-level policy, explicit category ownership metadata, structured effect metadata, sort order, revision marker.
- Image-key status: documented for all 8 research rows.
- Validation risk: current costs and durations are rule-driven, but there is no final metadata contract proving the seeded tree matches runtime behavior.

### Orbital assets

- Current strength: stable keys, cost, building requirement, storage, range, and crew-gate assumptions are documented.
- Missing metadata: final role/category ownership in the backend catalog, final asset ids, explicit fleet handoff policy metadata, final stat metadata, sort order, revision marker.
- Image-key status: documented for all 4 orbital rows.
- Validation risk: current ship data is readiness-oriented only; a final catalog could overclaim movement, cargo, or combat semantics if validation stays weak.

### Planetary assets

- Current strength: stable keys, costs, and building requirements exist in code.
- Missing metadata: dedicated final catalog doc, placeholder image keys, short descriptions, final category/role metadata, duration policy metadata, final asset ids, revision marker.
- Image-key status: missing for all 4 current rows.
- Validation risk: planetary assets are the clearest catalog gap today because they already affect readiness surfaces but do not yet have the same metadata coverage as buildings, research, or ships.

### Defenses

- Current strength: one stable row exists and is documented as construction-backed.
- Missing metadata: broader taxonomy, multiple defense rows, final role model, combat-effect metadata, scoped mutation ownership, revision marker.
- Image-key status: documented for the only current row, `defense.defense-grid`.
- Validation risk: there is no standalone defense catalog yet, so “catalog completeness” is structurally incomplete by design.

### Resources

- Current strength: persisted spendable resource keys are stable and visible non-stockpile signals are documented honestly.
- Missing metadata: final resource table, sort/group metadata, precision/display metadata, explicit capacity policy, final icon ids, revision marker.
- Image-key status:
  - persisted spendable keys documented: `credits`, `metal`, `crystal`, `gas`
  - visible non-stockpile keys documented: `energy`, `deuterium`, `population`
- Validation risk: visible resource terminology still mixes persisted and non-persisted concepts, so final validation must prevent accidental spendability drift.

### Civilizations and planets

- Current strength: one deterministic Development civilization identity exists and `PlanetType` gives a stable seven-archetype planet set.
- Missing metadata: final faction catalog, final production ownership mapping, multi-faction identity coverage, final per-surface planet asset policy, revision marker.
- Image-key status:
  - civilization: one current handoff key documented
  - planets: seven archetype-driven keys documented
- Validation risk: both sets are still partially provisional and should not be mistaken for final content breadth.

## Missing Metadata Categories

The most repeated missing categories across the catalogs are:

1. Spanish short descriptions owned by backend/catalog metadata instead of frontend fallbacks.
2. Structured prerequisite metadata rather than prose-only notes.
3. Structured effect metadata rather than behavior inferred from services.
4. Sort order and display grouping metadata.
5. Nullable final asset ids and later manifest linkage.
6. Version or revision markers for balance changes.
7. Clear ownership boundaries between Development seed state and final relational seed data.

## Missing Image-Key Coverage

Current documented image-key coverage:

- complete: buildings, research, orbital assets, defenses, persisted resources, visible non-stockpile resource signals
- present but provisional: one Development civilization identity, seven planet archetype keys
- missing: all 4 current planetary asset rows

Current missing planetary asset keys that should be added in a later task:

- `PatrolGroup`
- `ExpeditionGroup`
- `VehicleGroup`
- `SupportGroup`

## Prerequisite And Rule Gaps

The largest remaining validation gaps before final DB work are not raw counts. They are rule-shape gaps:

- research has no final prerequisite tree or max-level policy
- construction upgrade and duration rules are runtime behavior, not final seed metadata
- orbital build timing is still a uniform `3 minutes * quantity`
- crew-capacity requirements exist, but their final relationship to population and logistics balance is not finalized
- defense prerequisites remain limited to the single `DefenseGrid` readiness path
- resource metadata does not yet distinguish final spendable, advisory, capacity, and future-economy classes in one authoritative table

## Seed And Data Drift Risks

1. Catalog drift risk: final relational seed rows could diverge from enum-backed runtime keys without a row-per-enum validator.
2. Frontend fallback drift risk: Spanish labels and categories still live partly in frontend helpers and could drift from future backend catalog rows.
3. Asset-key drift risk: placeholder and final asset mappings can diverge if manifest validation is not added before asset replacement.
4. Development-seed drift risk: deterministic QA data could be mistaken for final production initialization.
5. Defense under-modeling risk: the current single-row defense surface can hide how incomplete final defense taxonomy still is.
6. Planetary-asset gap risk: those rows already exist in code, but they are the least documented catalog family in the current prep set.

## Safe Next Validation Steps

1. Add a dedicated validator that proves every final catalog row maps to a known enum or stable key and that every known enum is covered exactly once.
2. Add a dedicated planetary-asset readiness note and placeholder image-key mapping before claiming cross-catalog completeness.
3. Add manifest/key drift validation before replacing placeholder visuals with final assets.
4. Keep Development seed profiles documented as QA scaffolding only and do not reuse them as live-start data without an explicit promotion task.
5. Keep SQL Server validation explicitly gated until provider selection and migration-baseline work are separated from catalog completeness work.

## Honest Conclusion

- The code-owned catalogs are small and countable today, which keeps the final DB phase tractable.
- Buildings, research, ships, defenses, and resources are documented enough to expose their remaining metadata gaps clearly.
- Planetary assets are the most obvious completeness gap.
- Civilization and planet identity data exist, but still represent a narrow handoff set rather than a final content catalog.
- The repository is ready for explicit validator and final-seed-shape tasks, but not yet ready to claim final catalog completeness.

## Validation

- This report is derived from the current code-owned catalog sources, the Development seed profile document, the balance review, and the final DB prep note.
- No schema migration, SQL Server action, runtime validator, browser QA, or asset generation was performed for this report.
