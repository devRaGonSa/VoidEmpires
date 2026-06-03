# Defenses Cockpit Checklist

Defenses cockpit v1 is a conservative development-only surface.
It must stay focused on planet protection readiness and defensive construction context without enabling combat, interception, fleet mutation, bombardment, invasion, or autonomous defense behavior.

Use `docs/dev/planet-module-boundaries.md` to keep `/defenses` separate from `/construction`, `/shipyard`, `/ground-army`, and `/fleets`.
Use `docs/dev/development-seed-profiles.md` for the current seeded ids and placeholder QA URLs.

## Current implemented defense-adjacent backend surface

### Domain inventory

- `BuildingType.DefenseGrid` exists in `src/VoidEmpires.Domain/Buildings/BuildingType.cs`.
- `BuildingCatalog` classifies `DefenseGrid` under `BuildingCategory.Defense` with standard construction cost and footprint rules.
- `ResearchType.Shielding` exists in `src/VoidEmpires.Domain/Research/ResearchCatalog.cs` with effect key `shield_strength`.
- `PlanetVisualProfileCatalog` includes renderer-facing profile strings such as `shielded_platform_clusters` and `thermal_shields_and_extractor_platforms`, but these are visual tags only.
- `PlanetVisualStateService` rolls `DefenseGrid` into aggregate military intensity together with other military and logistics buildings.

### Important non-inventory findings

- There is no dedicated planetary defense asset catalog today. `PlanetaryAssetCatalog` contains patrol, expedition, vehicle, and support groups only.
- There is no defense-specific application contract, dev endpoint, queue service, action manifest, or completion service.
- There is no backend concept for shield hit points, interception execution, bombardment, invasion defense, planetary damage, or defense automation.

## Existing safe backend contracts

### Read-only state already available

- `GET /api/dev/planets/ui-state`
  - Safe source for owned-planet defensive context.
  - Returns current buildings, stockpile, building capacity, construction queue, action summary, and full construction action catalog.
  - `DefenseGrid` already appears here as a normal building/action entry, including readable labels and the `Defense` category.
  - For non-owned planets, management data stays hidden, so the cockpit must not infer foreign defensive state from this endpoint.
- `GET /api/dev/planets/{planetId}/visual-state`
  - Safe renderer-facing read only.
  - May reflect aggregated military intensity influenced by `DefenseGrid`, but it is not a gameplay defense summary.
- Existing docs and manifests already treat nearby military surfaces such as interception readiness as metadata only.

### Existing guarded mutation path

- `POST /api/dev/buildings/construction-orders/enqueue`
  - Development-only existing mutation.
  - Already accepts any `BuildingType`, including `DefenseGrid`.
  - Enforces the generic construction safeguards already used elsewhere:
    - owned planet context
    - UTC request time
    - single open construction order per planet
    - stockpile affordability
    - building-capacity fit checks for new construction
    - normal construction duration and sequence assignment
- `POST /api/dev/buildings/construction-orders/complete-due`
  - Exists, but is global and not planet-scoped.
  - Current cockpit guidance must keep this disabled in `/defenses`.

## Defenses v1 scope

### Safe read-only state

Defenses v1 may show only the following:

- current `DefenseGrid` presence, level, and readable labels from planet UI state
- the planet stockpile and building-capacity context needed to explain defensive affordability
- any queued or completed construction rows that happen to target `DefenseGrid`
- the existing construction-action availability for `DefenseGrid`
- explanatory notes that shielding research and visual military intensity exist nearby but do not yet provide executable defense systems

### Safe guarded action

Defenses v1 may optionally offer one guarded action only:

- explicit Development-only enqueue of `DefenseGrid` through the existing generic construction endpoint

If later tasks wire this action into `/defenses`, they must keep these rules:

- no new production API
- no defense-specific mutation endpoint
- no queue completion button
- no bulk actions
- no automatic follow-up behavior
- no hidden side effects beyond the existing construction queue row and resource spend
- the UI must clearly label the action as defensive construction preparation, not combat or interception

### Intentionally unsupported in v1

The following must stay disabled or documented as unavailable:

- shield activation, regeneration, or damage absorption
- combat execution
- interception execution
- bombardment
- invasion defense resolution
- fleet mutation from the defenses cockpit
- hidden enemy threat detection
- planetary weapon stock
- defense automation
- alliance or pact based defense behavior
- direct complete-due execution from the cockpit

## Unsafe hooks and nearby systems that must not be activated

- Strategic-map interception endpoints are read-only readiness metadata only. They do not execute interception, combat, damage, or transfer resolution.
- Fleet endpoints own orbital movement, split, merge, travel estimate, transfer create, cancel, and complete flows. Defenses must not call them to simulate protection behavior.
- Planet visual-state intensity is presentation metadata only. It must not be translated into real mitigation, shield strength, or threat resolution.
- `ResearchType.Shielding` is present as research metadata only. No current backend path applies it to a defensive read model or battle rule.

## Recommended cockpit contract shape for later tasks

Do not add a new backend endpoint in v1 unless the existing planet UI payload proves too coarse.
If a dedicated DTO becomes necessary later, keep it development-only and read-focused with a shape equivalent to:

- planet identity and ownership context
- `defenseBuildings[]` limited to defensive structures only
- `defenseCatalog[]` limited to defensive construction options only
- `queue[]` filtered to defense-related construction rows only
- `actionAvailability.enqueueDefenseGrid`
- `notes[]` documenting disabled systems such as shields, interception, and damage

This future DTO must still avoid combat, fleet, and damage state.

## QA boundary

- `/defenses` remains a placeholder/readiness cabin before later cockpit tasks land.
- The standard seeded URL remains `/defenses?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`.
- Seed profiles preserve navigation context for Defenses but do not seed a dedicated defense queue, shield state, or combat state.

## Final boundary statement

Defenses v1 prepares readiness and protection only.
Today that means reading existing planet and construction metadata and, at most, reusing the guarded Development-only construction enqueue for `DefenseGrid`.
Anything that looks like combat, interception, damage, fleet control, or active defense resolution is outside scope and must remain unavailable.
