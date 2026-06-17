# Defense Catalog Final DB Readiness

This note prepares the current defense catalog boundary for a later final database seed pass. It documents accepted defense types, roles, costs, requirements, image keys, and the combat dependencies that must remain deferred.

It does not add migrations, seed rows, generated images, combat, fleet movement, market transactions, alliance mutations, or production-auth behavior.

## Current Authority

- There is no standalone defense domain catalog today.
- Defensive readiness is derived from `BuildingCatalog` rows where `BuildingCategory` is `Defense`; currently that means `DefenseGrid`.
- `DevDefenseUiStateService` filters Planet construction state into defense structures, defense construction options, and defense queue rows.
- The Defenses cockpit is read-only for defense-specific actions. It can hand off to Construction, but it does not own a safe `POST /api/dev/defenses/...` enqueue path.
- `ResearchType.Shielding` exists in the research catalog, but it is not applied as active mitigation in the Defenses cockpit.
- Combat, interception, damage, bombardment, invasion, and automated defense behavior are not exposed by the current defense read model.

## Current Defense Rows

| Key | Spanish label | Category | Role | Source | Initial level | Footprint | Base cost | Requirement | Placeholder image key | Combat dependency |
|---|---|---|---|---|---:|---:|---:|---|---|---|
| `DefenseGrid` | Malla defensiva | Proteccion planetaria | Fortificacion lista para preparar | `BuildingCatalog` | 1 | 10 | C0 M150 X75 G0 | Construction capacity, local resources, no open construction order | `defense.defense-grid` | No damage, interception, bombardment, or invasion mitigation is modeled today. |

Cost shorthand: `C` credits, `M` metal, `X` crystal, `G` gas.

## Current Readiness Rules

The current Defenses cockpit reuses Planet/Construction readiness:

- The selected planet must be owned by the requesting civilization for management data to be visible.
- The source planet must expose a resource stockpile.
- The source planet must expose building capacity for new construction.
- The current construction queue must not already contain an open order.
- The source planet must have enough resources for the `DefenseGrid` construct or upgrade cost.
- The estimated duration comes from the construction duration policy, including accepted construction automation effects.
- Due construction completion is visible as readiness metadata, but the Defenses cockpit keeps completion disabled because the older backend route is global rather than cockpit-scoped.

The read model reports structure count, total defense level, available/blocked option counts, queue item count, and due queue item count. These are readiness summaries only and do not predict combat outcomes.

## Final Seed Metadata Required

If defenses become a dedicated final catalog, each row should include:

1. Stable key. For the current row this must map back to `BuildingType.DefenseGrid`.
2. Spanish display label and short Spanish card description.
3. Category key and Spanish category label.
4. Defense role key, such as fortification, shield, sensor, orbital defense, or ground defense.
5. Owning module or cockpit.
6. Placeholder image key from the table above.
7. Nullable final asset id for the generated asset phase.
8. Source model type: construction-backed building, independent defense asset, or future tactical system.
9. Base cost with canonical resource keys.
10. Footprint or placement/capacity policy.
11. Build/upgrade duration policy.
12. Required buildings, research, population, crew, or energy as structured keys.
13. Combat-effect metadata only after combat rules are accepted.
14. Readiness-display metadata that can keep Defenses read-only when no safe mutation path exists.
15. Sort order and recommended Defenses cockpit grouping.
16. Version or revision marker for future balance changes.

## Combat Dependencies To Keep Explicit

Final defense metadata must not imply execution until the dependent systems exist:

- Battle target selection and deterministic combat resolution.
- Ground invasion, bombardment, interception, and loss calculation.
- Shielding research effect mapping and stacking rules.
- Orbital defense versus ground defense boundaries.
- Damage repair, disabled-state, cooldown, and persistence rules.
- Fleet movement/arrival integration and sensor/visibility contracts.
- Alliance or pact effects on defense permissions or shared protection.

## Current Gaps Before Final DB

- `DefenseGrid` is the only modeled defense type.
- Labels, roles, and readiness categories are frontend presentation fallbacks, not backend catalog metadata.
- There is no final defense taxonomy for shields, sensors, orbital batteries, ground defenses, or automated interception.
- There is no scoped defense enqueue endpoint; Construction remains the accepted mutation handoff.
- There is no final combat stat model, targeting model, mitigation model, or battle audit contract.
- Seed validation should prove every construction-backed defense catalog row maps to a known domain key and does not create duplicate gameplay state.

## Seed Phase Requirements

- Add final defense catalog rows only in a dedicated final DB/model consolidation task.
- Keep catalog metadata separate from `PlanetBuilding`, construction queue rows, research project state, and future combat state.
- Preserve backend-owned resources, queue state, buildings, and completed research as authoritative gameplay state.
- Do not use catalog metadata to fake combat readiness, mitigation, interception, stock, ownership, or queue progress.
- Keep production auth, combat, fleet movement, market, and alliance behavior outside defense catalog metadata.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, DB migration, final asset generation, or integration validation was performed for this note.
