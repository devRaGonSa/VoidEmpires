# Final Assets Handoff

This handoff lists the deferred catalog and identity images that may replace `PlaceholderAsset` or adjacent placeholder marks in a later asset phase.

It does not generate assets, add manifests, change runtime loading, add database rows, or expand gameplay scope.

## Handoff Rules

- Keep every asset mapped by stable `imageKey`, not by visible Spanish copy.
- Reuse the conventions from `docs/dev/placeholder-image-key-conventions.md`.
- Treat buildings, research, ships, defenses, and persisted resources as stable today.
- Treat civilization and planet keys as partially provisional until final metadata lands; the entries below are the current handoff set, not a promise that the catalog is complete forever.
- Keep fallback behavior to the current deterministic placeholder treatment when an asset is missing.

## Buildings

| imageKey | Current consumer | Visual direction |
|---|---|---|
| `building.command-center` | Planet, Construction | Capital planetary hub, fortified command tiers, civic lighting, no battle damage. |
| `building.metal-mine` | Planet, Construction | Industrial mining complex with exposed ore conveyors and heavy extraction rigs. |
| `building.crystal-mine` | Planet, Construction | Refined crystal excavation site with luminous mineral faces and precision gantries. |
| `building.gas-extractor` | Planet, Construction | Pressurized gas towers, pipes, and storage bulbs over volatile vents. |
| `building.solar-plant` | Planet, Construction | Compact solar field with angled panels and visible energy routing hardware. |
| `building.research-lab` | Planet, Construction, Research | Clean high-tech lab campus with observatory silhouettes and data glow. |
| `building.shipyard` | Planet, Construction, Shipyard | Orbital-capable industrial dock with assembly arms and launch framework. |
| `building.defense-grid` | Planet, Construction, Defenses | Planetary shield-control or bastion grid node, defensive but not firing. |
| `building.habitation-district` | Planet, Construction | Dense colony housing blocks with transit links and civic lighting. |
| `building.medical-center` | Planet, Construction | Sterile medical campus with emergency pads and visible care infrastructure. |
| `building.military-academy` | Planet, Construction, Ground Army | Formal military training complex with parade geometry and secure compounds. |
| `building.barracks` | Planet, Construction, Ground Army | Practical troop housing and drill yard, readiness-focused rather than combat-active. |
| `building.crew-academy` | Planet, Construction, Shipyard | Spacefaring crew school with simulators, hangar classrooms, and orbital motifs. |
| `building.fleet-command-center` | Planet, Construction, Fleets | Strategic fleet coordination tower with sensor dishes and command bridges. |
| `building.logistics-hub` | Planet, Construction, Shipyard, Fleets | Freight routing campus with depots, cranes, and structured cargo lanes. |

## Research

| imageKey | Current consumer | Visual direction |
|---|---|---|
| `research.planetary-engineering` | Research | Planetary infrastructure schematics, orbital survey overlays, expansion planning tone. |
| `research.resource-extraction` | Research, Market advisory | Automated extraction rigs, refinery diagrams, and disciplined industrial throughput. |
| `research.energy-systems` | Research | Reactor-grid or energy lattice imagery with controlled power transfer arcs. |
| `research.construction-automation` | Research, Construction advisory | Fabrication robotics, queued assembly lines, and modular build orchestration. |
| `research.propulsion` | Research, Fleets advisory | Engine plume studies, thrust-vector diagrams, and efficient route mechanics. |
| `research.ship-weapons` | Research | Naval weapons engineering bench, targeting optics, and hardpoint studies without combat. |
| `research.shielding` | Research, Defenses advisory | Layered barrier field studies, shield harmonics, and containment geometry. |
| `research.espionage` | Research, Espionage advisory | Covert signal analysis, sensor masking, and intelligence relay tooling. |

## Ships

| imageKey | Current consumer | Visual direction |
|---|---|---|
| `ship.scout-craft` | Shipyard, Fleets | Fast light reconnaissance hull with long sensor lines and minimal cargo bulk. |
| `ship.cargo-craft` | Shipyard, Fleets, Market advisory | Sturdy logistics vessel with modular containers and visible loading hardware. |
| `ship.escort-craft` | Shipyard, Fleets | Compact armed escort silhouette, defensive posture, no active firing scene. |
| `ship.colony-craft` | Shipyard, Fleets, colonization-adjacent docs | Large settlement carrier with habitat sections and deployment infrastructure. |

## Defenses

| imageKey | Current consumer | Visual direction |
|---|---|---|
| `defense.defense-grid` | Defenses | Planetary fortification node, shield relays or ground batteries in a ready but inactive stance. |

## Resources

| imageKey | Current consumer | Visual direction |
|---|---|---|
| `resource.credits` | Planet, Construction, Research, Shipyard, Market, Fleets | Abstract economic token, ledger-grade metallic chip or secure credit stamp. |
| `resource.metal` | Planet, Construction, Shipyard, Fleets | Dense industrial alloy ingots or ore-refined stock with rugged surfaces. |
| `resource.crystal` | Planet, Construction, Research, Shipyard | Faceted luminous crystal clusters suited to advanced fabrication. |
| `resource.gas` | Planet, Construction, Research, Shipyard, Fleets | Pressurized energy gas canister or volatile atmospheric capsule. |
| `resource.energy` | Research, Market advisory, planet summaries | Clean power-cell or grid-node mark, not a spendable stockpile icon. |
| `resource.deuterium` | Market advisory, fleet-adjacent docs | Refined exotic fuel container, reserved for non-persisted future economy signals. |
| `resource.population` | Planet, Ground Army, civilization summaries | Civil population or workforce emblem, abstract and non-military. |

## Civilization Assets

These keys are current handoff placeholders for identity surfaces. They should stay stable until a real faction catalog exists.

| imageKey | Current consumer | Status | Visual direction |
|---|---|---|---|
| `civilization.void-seed-civilization` | Alliance, Ranking, onboarding-adjacent summaries | Current stable seed identity | Emblem for a pragmatic frontier polity; readable crest, no alliance insignia or rank badges. |

## Planet Assets

Use archetype-driven keys rather than mutable discovered planet names. These keys map to the current `PlanetType` set and can serve Galaxy, Planet, Espionage, and summary cards.

| imageKey | Current source | Status | Visual direction |
|---|---|---|---|
| `planet.terran` | `PlanetType.Terran` | Stable | Temperate world with oceans, continents, cloud cover, and visible settlement lights. |
| `planet.desert` | `PlanetType.Desert` | Stable | Dry ochre world with dune seas, oasis basins, and sparse colony infrastructure. |
| `planet.ice` | `PlanetType.Ice` | Stable | Frozen world with fractured ice sheets, polar haze, and thermal colonies. |
| `planet.volcanic` | `PlanetType.Volcanic` | Stable | Dark basalt planet with lava fissures, ash haze, and shielded industry. |
| `planet.oceanic` | `PlanetType.Oceanic` | Stable | Deep-water world with floating arcologies and broad storm bands. |
| `planet.barren` | `PlanetType.Barren` | Stable | Cratered regolith world with mining scars and sparse outposts. |
| `planet.gas-giant` | `PlanetType.GasGiant` | Stable | Massive banded atmosphere with storm belts and orbital-only infrastructure. |

## Deferred Follow-Up For The Asset Phase

1. Add a manifest that maps each `imageKey` above to file path, variant, dimensions, alt-text source, and fallback behavior.
2. Validate manifest drift against the current catalog/domain keys before replacing any placeholder surface.
3. Replace one asset set at a time, starting with repeated card surfaces such as buildings, research, and ships.
4. Keep browser screenshot QA and lazy-route checks explicit; passing build or tests alone is not visual acceptance.

## Known Gaps

- Civilization identity currently has one documented seed-backed key, not a final faction catalog.
- Planet keys are stable by archetype, but route-specific art direction may still need a later split between card, map, and detail variants.
- No binary assets, source prompts, licenses, or manifests are committed in this task.

## Validation

- This handoff inventory was derived from the current catalog/readiness docs, `PlanetType`, `PlanetVisualProfileCatalog`, and the current Development seed identity.
- Asset creation, browser QA, screenshot approval, manifest validation, and runtime wiring remain deferred.
