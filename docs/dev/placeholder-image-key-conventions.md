# Placeholder Image Key Conventions

This note defines the placeholder image key format for catalog-backed visuals that still use `PlaceholderAsset`.

It does not add generated images, binary assets, asset manifests, migrations, seed rows, gameplay state, or route-loading changes.

## Current Boundary

- `PlaceholderAsset` accepts an optional `imageKey` and exposes it through `data-placeholder-image-key`.
- Shipyard presentation already uses stable keys such as `ship.scout-craft` and `ship.cargo-craft`.
- Building, research, defense, resource, civilization, and planet surfaces should follow the same convention before any final asset pass starts.

## Canonical Format

Use this format for every placeholder image key:

`<namespace>.<slug>`

Rules:

- `namespace` must be one of the approved catalog domains in this note.
- `slug` must be lowercase kebab-case.
- Use ASCII letters and digits only in the slug, separated with `-`.
- Do not include spaces, underscores, camelCase, accents, or file extensions in the key.
- Keep the key stable from backend/domain/catalog identity, not from UI position or language copy.
- Prefer one stable key per catalog row. Variants, crops, resolutions, or themes belong in a later asset manifest, not inside the placeholder key.

## Approved Namespaces

| Namespace | Scope | Example |
|---|---|---|
| `building` | Construction and planet structure rows backed by `BuildingType` | `building.command-center` |
| `research` | Research rows backed by `ResearchType` | `research.construction-automation` |
| `ship` | Orbital asset rows backed by `SpaceAssetType` | `ship.escort-craft` |
| `defense` | Defense-facing rows, including construction-backed defense entries | `defense.defense-grid` |
| `resource` | Resource metadata and non-stockpile resource signals | `resource.crystal` |
| `civilization` | Civilization or faction identity marks | `civilization.aurora-compact` |
| `planet` | Planet environment, biome, or world-summary marks | `planet.temperate-world` |

## Naming Source

Choose the slug from the existing stable identity in this order:

1. Final catalog row key when it exists.
2. Current backend enum or domain key.
3. Existing deterministic frontend catalog entry that maps directly to the backend key.

Normalization guidance:

- Convert PascalCase or camelCase keys to kebab-case.
- Expand only enough to stay readable and deterministic.
- Keep singular nouns unless the domain key is already plural.
- Do not translate the slug to Spanish if the backend/domain key is English; Spanish belongs in visible labels, not in the placeholder identity.

Examples:

| Current stable key | Placeholder image key |
|---|---|
| `CommandCenter` | `building.command-center` |
| `ResourceExtraction` | `research.resource-extraction` |
| `ScoutCraft` | `ship.scout-craft` |
| `DefenseGrid` | `defense.defense-grid` |
| `Credits` | `resource.credits` |

## Domain Conventions

### Buildings

- Use `building.<building-type-slug>`.
- Reuse the same key across Planet, Construction, Defenses handoff, and Ground Army readiness views when they point to the same building row.
- Examples: `building.shipyard`, `building.research-lab`, `building.logistics-hub`.

### Research

- Use `research.<research-type-slug>`.
- Level, completion, and queue state must not create alternate keys.
- Examples: `research.planetary-engineering`, `research.ship-weapons`, `research.espionage`.

### Ships

- Use `ship.<space-asset-type-slug>`.
- Stock quantity, queue quantity, and fleet grouping must not create alternate keys.
- Examples: `ship.cargo-craft`, `ship.colony-craft`.

### Defenses

- Use `defense.<defense-row-slug>`.
- If the defense is currently construction-backed, keep the defense namespace instead of falling back to `building.*` on defense-facing surfaces.
- Example: `defense.defense-grid`.

### Resources

- Use `resource.<resource-key-slug>`.
- Persisted stockpile resources and visible non-stockpile signals may share the namespace, but only when the stable backend or catalog key is distinct.
- Examples: `resource.metal`, `resource.energy`, `resource.population`.

### Civilizations

- Use `civilization.<civilization-key-slug>`.
- Derive the slug from the stable catalog or seed identity, not from player-owned mutable display names alone.
- Reserve this namespace for faction, polity, or emblem identity marks.
- Examples: `civilization.aurora-compact`, `civilization.iron-hegemony`.

### Planets

- Use `planet.<planet-key-slug>`.
- Prefer a stable environment, archetype, or catalog key rather than a mutable discovered planet name.
- Reserve this namespace for world-summary, biome, or colony-context marks rather than for owned building rows.
- Examples: `planet.temperate-world`, `planet.desert-colony`, `planet.gas-giant`.

## Disallowed Patterns

- `building_CommandCenter`
- `Building.CommandCenter`
- `ship.scoutCraft`
- `resource.cristal`
- `planet.New Terra`
- `civilization.player-raul`
- `ship.scout-craft.webp`

## Future Asset Path Strategy

This repository does not add the asset files yet, but later asset work should keep the key-to-file layout consistent with the key format above.

Recommended path strategy:

- Manifest key: `ship.scout-craft`
- Domain folder: `ship/`
- Asset slug folder: `scout-craft/`
- Variant file names: `card`, `thumb`, `badge`, or other explicit surface variants

Recommended relative layout:

`assets/catalog/<namespace>/<slug>/<variant>.<ext>`

Examples:

- `assets/catalog/building/command-center/card.webp`
- `assets/catalog/research/construction-automation/card.webp`
- `assets/catalog/ship/scout-craft/card.webp`
- `assets/catalog/resource/crystal/badge.svg`
- `assets/catalog/civilization/aurora-compact/emblem.webp`
- `assets/catalog/planet/temperate-world/card.webp`

Implementation guidance for the later asset phase:

- Keep the placeholder key as the stable manifest lookup key.
- Store file format, dimensions, and fallback behavior in the manifest, not in the key.
- Load assets at the cockpit or component boundary so lazy routes stay split.
- Keep placeholder fallback active whenever a manifest row or file is missing.

## Validation

- The current convention aligns with `PlaceholderAsset` and the existing Shipyard `imageKey` usage.
- No asset generation, manifest creation, frontend runtime change, or database change was performed for this note.
