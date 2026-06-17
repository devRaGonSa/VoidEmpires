# Catalog Metadata Readiness

This note prepares the catalog metadata shape needed before final database seeding. It documents current static catalogs and the metadata that a later final DB phase should seed.

It does not add migrations, new catalog tables, generated assets, production authentication, combat, fleet movement productization, market transactions, or alliance mutations.

## Current Catalog Sources

| Catalog area | Current source | Current metadata | Final seed gap |
|---|---|---|---|
| Buildings | `BuildingCatalog` | `BuildingType`, initial level, footprint, cost, category | display label, short description, module owner, placeholder asset key, upgrade metadata, prerequisite metadata |
| Research | `ResearchCatalog` | `ResearchType`, base cost, bonus key | display label, category, effect summary, prerequisite chain, max level policy, placeholder asset key |
| Orbital assets | `OrbitalAssetCatalog` | `SpaceAssetType`, requirement, cost, storage capacity, operating range | display label, role, category, production duration policy, placeholder asset key, final visual metadata |
| Planetary assets | `PlanetaryAssetCatalog` | `PlanetaryAssetType`, requirement, cost | display label, role, category, training duration policy, placeholder asset key, final visual metadata |
| Defenses | construction-backed `DefenseGrid` plus Defenses read model | defensive structure and construction readiness | dedicated defense catalog metadata if defenses become independent from construction |
| Resources | `resourceDisplay.ts` helpers and cost records | canonical visible names for current resources | canonical resource metadata, color/token mapping, storage/capacity rules, final icons |
| Civilizations | Development seed/profile data and UI identity fields | deterministic local names and ids | faction metadata, emblem placeholder key, final identity assets, production account mapping |

## Target Final Seed Shape

Each catalog row should be able to provide:

1. Stable backend key.
2. Spanish display label.
3. Short Spanish description for card/list use.
4. Category key and category label.
5. Owning module or cockpit.
6. Placeholder asset key compatible with `PlaceholderAsset`.
7. Final asset id, nullable until the asset pass lands.
8. Requirement metadata as structured keys, not prose-only text.
9. Cost metadata using canonical resource keys.
10. Duration or timing policy when the domain supports it.
11. Availability/readiness policy key.
12. Sort order for catalog display.
13. Version or revision marker for future balance changes.

## Frontend Readiness Rules

- Frontend presentation helpers may keep Spanish labels while final catalog tables are not present.
- Placeholder visuals must use existing backend/domain keys and must not invent availability, stock, level, or readiness.
- Unknown catalog keys should fall back to honest unavailable or diagnostics-only copy, not primary fake examples.
- Read models remain authoritative for actual state: queues, resources, stock, completed research, buildings, rankings, and readiness.
- Lazy-loaded cockpit routes must not depend on a global eager catalog bundle once final metadata grows.

## Final DB Phase Dependencies

- Decide whether catalog metadata lives in seed-only relational tables, versioned JSON content tables, or code-generated seed rows.
- Define migration ownership for catalog tables separately from gameplay state tables.
- Define how final generated assets map to catalog rows without requiring binary assets in ordinary backend tests.
- Add validation that seeded catalog keys still match domain enums and frontend presentation fallbacks.
- Keep production auth and civilization ownership outside catalog metadata; catalog rows describe content, not access.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, DB migration, or final asset validation was performed for this note.
