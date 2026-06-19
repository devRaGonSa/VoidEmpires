# Final Catalog Audit

Status date: 2026-06-19

This audit consolidates the current catalog-readiness position for the final database/model phase across buildings, research, ships, defenses, and resources.

It does not add catalog tables, seed rows, generated images, combat, fleet movement, market transactions, alliance mutations, or production authentication.

## Current Coverage Summary

| Area | Current authority | Coverage snapshot | Final DB readiness |
|---|---|---|---|
| Buildings | `BuildingCatalog` plus backend read models | 15 code-backed rows with category, footprint, initial level, and base cost | Partial: keys and cost baseline exist, but final metadata contract is incomplete |
| Research | `ResearchCatalog` plus research queue/readiness services | 8 code-backed rows with base cost and bonus key | Partial: costs and enum coverage exist, but prerequisites, tree shape, and final effect metadata are incomplete |
| Ships | `OrbitalAssetCatalog` plus Shipyard read model | 4 code-backed orbital rows with cost, requirement, storage, and range | Partial: production metadata exists, but movement/combat/cargo semantics are not final |
| Defenses | Construction-backed `DefenseGrid` only | 1 derived defense row from `BuildingCatalog` | Low: no dedicated defense catalog exists yet |
| Resources | `ResourceType`, economy services, and frontend display helpers | 4 persisted spendable resources plus 3 visible non-stockpile terms | Partial: runtime keys exist, but canonical metadata and non-stockpile rules are not centralized |

## Area Audit

### Buildings

- Current source: `src/VoidEmpires.Domain/Buildings/BuildingCatalog.cs`
- Current metadata present: stable key, category, initial level, footprint, base cost
- Current count: 15 accepted rows
- Missing final metadata:
  - short Spanish description
  - owning cockpit/module
  - placeholder asset key as backend-owned metadata instead of doc-only mapping
  - nullable final asset id
  - structured prerequisite metadata
  - structured effect metadata
  - sort order
  - tags and revision marker
- Current balancing gaps:
  - effect scaling is spread across downstream services instead of one catalog contract
  - upgrade multiplier and duration behavior are code rules, not seed metadata
  - logistics, crew, defense, and population effects are not described as a unified final data model

### Research

- Current source: `src/VoidEmpires.Domain/Research/ResearchCatalog.cs`
- Current metadata present: stable key, base cost, bonus key
- Current count: 8 accepted rows
- Missing final metadata:
  - short Spanish description
  - backend-owned category label
  - owning module/cockpit
  - placeholder asset key as final seed metadata
  - nullable final asset id
  - max-level policy
  - structured prerequisite chain
  - structured effect metadata
  - queue policy metadata
  - sort order, recommendation path, tags, and revision marker
- Current balancing gaps:
  - no accepted final research tree or unlock graph
  - several bonus keys still point at readiness-only behavior because combat, fleet movement, and espionage execution are deferred
  - source-planet and queue-slot checks exist, but they are not a final prerequisite model

### Ships

- Current source: `src/VoidEmpires.Domain/Assets/OrbitalAssetCatalog.cs` plus Shipyard read models
- Current metadata present: stable key, requirement, base cost, storage capacity, operating range
- Current count: 4 accepted orbital rows
- Missing final metadata:
  - backend-owned Spanish description
  - canonical category and role labels
  - owning module/cockpit
  - placeholder asset key as seed metadata
  - nullable final asset id
  - production target metadata
  - structured crew/capacity policy
  - fleet handoff policy
  - final upkeep/fuel/speed/combat-stat metadata
  - sort order, tags, and revision marker
- Current balancing gaps:
  - storage and range exist, but cargo, colonization, and movement execution are not final gameplay
  - escort/combat semantics remain descriptive only
  - there is no final maintenance, upkeep, fuel, fleet-slot, or damage model

### Defenses

- Current source: construction-backed `DefenseGrid` derived from `BuildingCatalog`
- Current metadata present: stable building key, footprint, initial level, base cost, construction-driven readiness
- Current count: 1 derived defense row
- Missing final metadata:
  - dedicated defense catalog key space
  - standalone role taxonomy
  - owning module metadata beyond Construction handoff
  - placeholder asset key as backend-owned seed metadata
  - nullable final asset id
  - structured requirement metadata beyond current construction checks
  - readiness-display policy for future defense rows
  - sort order, tags, and revision marker
- Current balancing gaps:
  - only `DefenseGrid` exists today
  - no final taxonomy for shields, sensors, orbital batteries, or ground defenses
  - no combat, interception, mitigation, bombardment, or repair model exists yet

### Resources

- Current source: `ResourceType`, `PlanetResourceStockpile`, `PlanetProductionProfile`, backend economy services, and frontend display helpers
- Current metadata present: stable persisted keys for `Credits`, `Metal`, `Crystal`, and `Gas`; visible Spanish labels in frontend helpers
- Current count: 4 persisted spendable resources plus visible non-stockpile terms `Energy`, `Deuterium`, and `Population`
- Missing final metadata:
  - centralized backend-owned Spanish descriptions
  - resource class metadata
  - explicit persisted/not-persisted flag in one catalog contract
  - spendable flag in one catalog contract
  - placeholder icon/asset key as backend-owned metadata
  - nullable final asset id
  - canonical precision, rounding, and display policy
  - capacity/storage policy
  - market/trade eligibility metadata
  - sort order, tags, and revision marker
- Current balancing gaps:
  - `Energy`, `Deuterium`, and `Population` are visible terms but not stockpile currencies
  - no final capacity, overflow, upkeep, fuel, or market-pricing model exists
  - production rates are Development seed/profile values, not final catalog metadata

## Cross-Cutting Metadata Gaps

Across all requested catalog groups, the missing final-seed contract still needs:

1. Stable backend key to final seed row validation.
2. Spanish display label and short Spanish description stored as backend-owned metadata.
3. Category, role, or class labels stored centrally instead of only in frontend presentation helpers.
4. Owning module/cockpit metadata.
5. Placeholder asset or icon key stored with the row.
6. Nullable final generated-asset id.
7. Structured prerequisites and structured effects.
8. Sort order, tags, and revision/version marker.

## Balance And Scope Risks

1. Catalog drift risk remains high because runtime authority is still code-backed while future seed ownership is not implemented.
2. Frontend labels and backend enums can diverge unless final seed validation proves one row per domain key.
3. Several rows already carry names that imply future gameplay power, but combat, movement, colonization, espionage, and market execution are still out of scope.
4. Development seed profiles currently shape visible balance expectations, but they are QA scaffolding only and must not become final live balance by inertia.
5. Resource terminology is broader than current persisted economy support, so final metadata must distinguish stockpile currency from display-only or future concepts.

## Recommended Follow-Up Sequence

1. Finalize per-area readiness notes into seed-row candidate shapes for buildings, research, ships, defenses, and resources.
2. Decide one repository-owned catalog seed architecture for all gameplay catalogs before adding tables or seed services.
3. Add enum-to-row validation so future relational seed data cannot drift from backend keys.
4. Keep generated image integration deferred until the metadata contract is stable.

## Explicitly Deferred

- final generated image creation
- final icon generation
- combat stats or combat execution metadata
- fleet movement execution metadata
- market transaction metadata beyond readiness placeholders
- alliance or production-auth ownership metadata

Image generation remains intentionally deferred. Catalog rows should keep deterministic placeholder asset keys until a later dedicated asset pass lands.
