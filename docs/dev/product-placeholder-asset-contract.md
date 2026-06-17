# Product Placeholder Asset Contract

This contract defines how placeholder visuals may represent buildings, technologies, ships, defenses, resources, and civilizations until the final generated asset phase.

It does not add final images, sprites, textures, icons, database rows, gameplay stats, combat behavior, movement behavior, market behavior, alliance behavior, or production authentication.

## General Rules

- Placeholder visuals must be deterministic from existing backend/domain keys, not invented per render.
- Placeholder visuals must never imply ownership, availability, stock, resource quantity, queue progress, ranking, or readiness that the backend did not provide.
- Use Spanish visible labels from the existing presentation helpers when available.
- Keep raw enum keys available only in diagnostics or docs.
- Prefer simple CSS shapes, existing UI badges, initials, or category marks until final generated assets exist.
- Do not commit generated bitmap assets during placeholder-contract tasks.
- Do not use placeholders to hide missing final model work. If a backend field is missing, show an honest unavailable or readiness state.

## Stable Placeholder Keys

| Domain | Stable key source | Placeholder role | Must not imply |
|---|---|---|---|
| Buildings | `BuildingType` plus target level when present | Structure category mark for construction, planet, defenses, and ground-readiness cards | completed building state unless backend building or queue state says so |
| Technologies | `ResearchType` plus current or target level | Research discipline mark for catalog, queue, and completed-project rows | researched level, effects, or prerequisites not returned by the read model |
| Ships and orbital assets | `SpaceAssetType` plus quantity when present | Orbital asset silhouette/category mark for Shipyard and Fleets | operational fleet group unless Fleet read state exposes a group |
| Defenses | defense-backed `BuildingType` or defense readiness key | Defensive readiness mark for Defenses cards | combat, interception, or automated defense execution |
| Resources | canonical resource type from resource display helpers | Resource swatch or compact chip for reserves, costs, and deltas | live accrual, hidden stockpile, market price, or spending not confirmed by backend |
| Civilizations | civilization id plus display name when present | Faction initial, color token, or compact identity mark | production auth, player profile, diplomacy permission, or alliance membership |

## Visual Treatment

- Placeholder marks should be compact and secondary to the label and backend state.
- Use consistent category color families, but avoid making color the only state indicator.
- Cards may show one placeholder mark per entity; repeated decorative art blocks should wait for the final asset phase.
- Empty states should say that no backend-backed item is visible rather than filling the layout with fake examples.
- Diagnostics may show raw keys, ids, or payload summaries, but they should remain collapsed or visually secondary.

## Future Asset Handoff Requirements

Before replacing placeholders with final assets, each asset set should define:

1. Stable key mapping from backend/domain key to asset id.
2. Fallback behavior when an asset is missing.
3. Licensing/source metadata for generated or authored assets.
4. Accessibility text strategy based on the Spanish visible label.
5. Bundle/loading strategy that preserves lazy cockpit routes.
6. Visual QA checklist entries for desktop and mobile.

## Final Asset Phase Prep Plan

The later final generated-asset phase should treat this contract as the replacement boundary for the current `PlaceholderAsset` component and CSS marks. It should not add gameplay state, availability, ownership, queue progress, stock, rankings, production auth, combat, fleet movement, market transactions, or alliance mutations.

Recommended entry criteria:

1. Final DB/catalog metadata either exposes nullable final asset ids or documents the temporary source for the key-to-asset map.
2. Every generated or authored asset has source/licensing metadata, generation prompt or art-source notes when applicable, dimensions, format, and owner.
3. Each asset maps to an existing stable backend/domain key already documented in catalog readiness notes.
4. Missing assets have an explicit fallback to the current deterministic placeholder treatment.
5. Route-level loading remains lazy; final assets must not create a global eager bundle for all cockpits.
6. Accessibility text uses the Spanish visible label for the represented entity, not raw enum keys.

Recommended asset sets for the first final pass:

| Asset set | Key source | First consumer | Replacement target |
|---|---|---|---|
| Building cards | `BuildingType` and catalog metadata | Planet, Construction, Defenses, Ground Army | `PlaceholderAsset` building/defense marks |
| Research cards | `ResearchType` and catalog metadata | Research | `PlaceholderAsset` technology marks |
| Orbital assets | `SpaceAssetType` and catalog metadata | Shipyard, Fleets | `PlaceholderAsset` ship marks |
| Resources | resource metadata keys | resource strips, costs, Market advisory views | compact swatches or resource marks |
| Civilizations | civilization/faction metadata | Onboarding, Alliance, Ranking, diagnostics summaries | civilization initials or identity marks |

Recommended implementation sequence:

1. Add an asset manifest shape that maps stable keys to final asset ids, file paths or URLs, dimensions, alt-text source, and fallback behavior.
2. Add validation that all manifest keys are known catalog/domain keys and that missing final assets fall back to placeholders.
3. Introduce asset loading at the cockpit/component boundary so lazy routes keep their split chunks.
4. Replace one asset set at a time, starting with the most repeated card surfaces, and keep cards readable when images fail or load slowly.
5. Update visual QA checklist entries with desktop and mobile screenshot targets for every replaced route.
6. Keep the current placeholder component available until every accepted route has passed visual QA with the final asset set.

Required validation gates for that phase:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
- asset manifest/key drift validation once a manifest exists
- browser screenshot QA through `docs/dev/deferred-visual-qa-master-checklist.md`

No generated images, sprites, textures, icons, manifests, or browser screenshots were added for this prep update.

## Deferred Dependencies

- Final generated images and sprites.
- Final visual QA/corrections.
- Final database/model consolidation for any new cosmetic metadata.
- Production account/civilization identity mapping.
- Combat, fleet movement productization, market transactions, and alliance gameplay.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- Browser and screenshot QA remain deferred.
