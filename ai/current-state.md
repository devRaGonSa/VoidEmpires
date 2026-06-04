# Current State

## Phase

The repository is consolidated through `Phase 21Z - Cross-cockpit UX consolidation and gameplay language polish`.

## Repository Reality

The AI Platform template has been adapted into a VoidEmpires-specific project workspace with:

- workflow rules in `AGENTS.md`
- planning and orchestration documents under `ai/`
- task lifecycle folders under `ai/tasks/`
- helper scripts under `scripts/`
- the `.NET` solution and projects under `src/` and `tests/`

## Application Status

The repository contains `VoidEmpires.sln` with:

- `src/VoidEmpires.Web`
- `src/VoidEmpires.Application`
- `src/VoidEmpires.Domain`
- `src/VoidEmpires.Infrastructure`
- `tests/VoidEmpires.Tests`

Current frontend cockpit baseline:

- Galaxy v1 remains read-only and has been restored as the accepted seeded cockpit baseline: `/galaxy` is now the canonical route, `/` remains a compatibility alias, shared navigation stays active on both paths, missing or invalid or failed or empty context now renders explicit Spanish states, seeded focus falls back to owned or visible systems instead of leaving the screen blank, diagnostics now expose a compact state summary when context exists, and cross-cockpit handoffs preserve owning Fleet context without exposing gameplay mutations from Galaxy.
- Planet v1 now exists at `/planet` as a 2D dashboard and context hub with a development-only UI-state read endpoint, Spanish-first presentation helpers, deterministic seeded economy and construction context, readable resources or production or capacity sections, grouped buildings, queue visibility, guarded construction enqueue, and dashboard handoff cards for Construction, Fleets, Galaxy, and the specialized placeholders.
- Construction v1 now exists at `/construction` as a focused general-infrastructure route for the same owned-planet construction state, with catalog readability, safe explicit confirmations, Spanish error guidance, queue refresh feedback, and secondary handoff cards for Research, Ground Army, Shipyard, and Defenses.
- Research v1 now exists at `/research` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with a deterministic seeded QA path that exposes at least one available item (`Ingenieria planetaria`), visible blocked items with meaningful Spanish reasons, category-grouped catalog state, truthful summary counts and recommendation fallback, visible requirements, costs, durations, queue and completed-project summaries, guarded enqueue confirmation when the safe dev endpoint is available, aligned read-model and enqueue contract metadata for the seeded available technology, successful Development enqueue with visible queue refresh, specific Spanish error mapping with secondary diagnostics, non-mutating blocked research cards, and a conservative disabled complete-due placeholder when the backend route is not scoped safely to the cockpit.
- Shipyard v1 now exists at `/shipyard` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with deterministic seeded `Aurelia` context, visible resources, production capability and readiness summaries, categorized orbital asset options, visible queue and local stock reads, guarded development enqueue through the scoped orbital production endpoint, explicit success refresh feedback, a conservative disabled complete-due placeholder because the current backend route is still global, cross-navigation back to Planet, Construction, Research, Fleets, and Galaxy, and explicit copy that Fleet movement and command execution remain outside this cockpit.
- Defenses v1 now exists at `/defenses` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with deterministic seeded `Aurelia` context, defense readiness summaries, a visible `DefenseGrid` structure and option state, readable stockpile or missing-resource guidance, truthful queue and complete-due limitation messaging, collapsed diagnostics, and explanatory handoffs toward Construction, Shipyard, Fleets, Planet, and Galaxy while keeping unsafe mutation either disabled or handed off rather than combat-scoped.
- Ground Army v1 now exists at `/ground-army` as a development-safe cockpit foundation upgraded from the earlier placeholder route, with deterministic seeded `Aurelia` context, visible readiness and population summary state, visible ground structures and catalog options, truthful available and blocked training comparisons, completed queue-history context, collapsed diagnostics, and explanatory handoffs toward Construction, Defenses, Fleets, Planet, and Galaxy while keeping unsafe mutation confirmation-based, disabled, or handed off rather than combat-scoped.
- Fleets remains the accepted dev-cockpit foundation and now supports simple URL-based context links into Planet, Construction, and Shipyard while keeping destination context optional.
- Query-context helpers now centralize `civilizationId` and `planetId` navigation so the cockpit links stop rebuilding URLs by hand.
- Module-specific catalog duplication has been reduced by extracting shared planet layout components and route builders.
- The accepted cockpit suite now shares a clearer polish baseline: primary copy is more gameplay-facing, diagnostics stay collapsed or clearly secondary, action hierarchy is more consistent, responsive overflow has been tightened, and sidebar or module-state cues better distinguish implemented versus future modules.
- Development-only seed profiles now provide the standard QA setup path for Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Fleets, and Defenses without manual SQL.
- `minimal-validation` remains the deterministic shared baseline, `cockpit-validation` is now the first coherent cross-cockpit demo scenario for Galaxy, Planet, Construction, Research, Ground Army, Shipyard, Fleets, and Defenses together, and the current cockpit-specific richer profiles are `shipyard-validation`, `fleet-validation`, `research-validation`, and `planet-full-validation`.
- Seed profiles are additive, deterministic, idempotent, and Development-only. They restore documented baseline rows and minimums but do not destructively clear queues, extra transfers, or other user mutations.
- Richer development seed profiles now reserve deterministic high sequence ranges for their completed queue-history rows, preventing runtime collisions when `cockpit-validation` is applied over reused development databases that already contain manual QA queue activity.
- The development seed apply endpoint now converts persisted-state write conflicts into `409 Conflict` responses with diagnostic details instead of surfacing an unhandled runtime failure.
- The current frontend boundary model is documented in `docs/dev/planet-module-boundaries.md`.
- The current Research cockpit QA flow and acceptance boundaries are documented in `docs/dev/research-cockpit-checklist.md`.
- The current Shipyard cockpit QA flow and accepted Fleet boundary are documented in `docs/dev/shipyard-cockpit-checklist.md`.
- The current Defenses cockpit QA flow and accepted non-combat boundary are documented in `docs/dev/defenses-cockpit-checklist.md`.
- The current Ground Army cockpit QA flow and accepted non-combat boundary are documented in `docs/dev/ground-army-cockpit-checklist.md`.
- The current seed profile catalog, discovery endpoint, deterministic ids, and QA URLs are documented in `docs/dev/development-seed-profiles.md`.

Current intentional exclusions:

- no 3D or WebGL renderer
- no combat gameplay
- no Ground Army combat, invasion, or assault resolution
- no real interception execution
- no espionage gameplay
- no espionage execution
- no alliances
- no market
- no production authentication
- no Fleet movement, transfer creation, split, merge, or combat execution from Shipyard
- no combat, interception execution, fleet movement, or shield simulation from Defenses
- no invasion, bombardment, orbital transport command, or fleet movement execution from Ground Army
- no real specialized module execution yet outside the current backend-supported Research and Shipyard enqueue paths plus the accepted Fleets flow
- no real research effects beyond queue and completion state
- no destructive seed reset behavior

Current implemented foundations:

- PostgreSQL/EF Core persistence boundary.
- ASP.NET Core Identity persistence foundation.
- Galaxy, player, civilization, ownership, economy, buildings, research, construction queue, research queue, population, asset production, asset stock, orbital group, and orbital transfer persistence models and migrations.
- Optional queue workers for construction, research, asset production, and orbital transfers.
- Development-only HTTP validation endpoints for current backend foundations.
- Planet visual state contracts and persisted visual state service.
- Solar system visual state contracts and persisted system visual state service.
- System visual metadata for renderer preparation: star metadata, coordinates, orbital slots, orbit radii, orbit angles, and scale hints.
- Read-only system visual overlays: stationed orbital group markers and planned/active transfer route overlays.
- Read-only orbital travel estimate previews through the application, infrastructure, and development API layers, including affordability and insufficient-resource details.
- Read-only orbital route profile metadata for travel estimates, classifying abstract distances into deterministic `LocalOrbit`, `InnerSystem`, `OuterSystem`, and `LongRange` bands with placeholder risk and fuel metadata.
- Read-only placeholder orbital fuel readiness previews for travel estimates, deriving estimated fuel units, estimated range, readiness, and not-ready reasons without adding fuel inventory or fuel charging.
- Reusable resource affordability and spend service for persisted planet stockpiles, with atomic multi-resource validation and spending.
- Orbital transfer creation charges estimated travel costs from persisted planet stockpiles before reserving groups and creating transfers.
- Persistent orbital group split foundation for stationed groups, preserving civilization, origin planet, current planet, asset type, and status while decreasing the source group quantity.
- Persistent orbital group merge foundation for compatible stationed groups, increasing the target group quantity and removing the source group.
- Read-only fleet operational overview for civilization-scoped orbital groups, active transfer summaries, and command availability flags.
- High-level fleet lifecycle smoke coverage validates preview, transfer creation and resource charging, active-transfer command rejection, cancellation, split, merge, second transfer completion, and final overview state.
- Route/fuel lifecycle smoke coverage validates that travel estimates include route profile and placeholder fuel readiness, remain read-only, do not require fuel inventory or route graph persistence, and stay coherent with fleet UI state hints through transfer completion.
- Developer-facing fleet API contract documentation under `docs/dev/fleet-api-contracts.md`, covering development gating, request/response payloads, status codes, read-only versus mutating behavior, resource charging/no-refund behavior, restrictions, and a compact fleet lifecycle example.
- Fleet development endpoint response consistency review found the current status-code and response-shape conventions already aligned for safe frontend tooling use, so no endpoint behavior changes were introduced in Phase 6W.
- Visual sandbox development documentation now reports the current Phase 6X validation baseline instead of the stale Phase 6H/6I test count.
- Development-only fleet UI state endpoint aggregates operational group state, active transfer summaries, command availability, current-planet resource contexts, and action hints for future UI prototypes.
- Development-only fleet UI state endpoint exposes route/fuel readiness capability hints for each group, while leaving concrete route profile and fuel readiness previews null until a destination-specific travel estimate is requested.
- Development-only fleet action manifest exposes deterministic, machine-readable metadata for current fleet dev actions, including route, method, mutability, required fields, success status, common error statuses, and notes for route/fuel preview flows.
- Read-only strategic map service consolidates civilization-scoped relevant systems, planet visual/layout summaries, owned ownership markers, fleet presence, active transfer overlays, and route/fuel capability notes without adding gameplay behavior or production UI endpoints.
- Development-only strategic map endpoint at `GET /api/dev/strategic-map?civilizationId={id}` exposes the Phase 7E read model behind existing development gating and persistence checks.
- Strategic map development contract documentation under `docs/dev/strategic-map-api-contract.md` describes request/response fields, gating behavior, side effects, limitations, and relationship to visual/fleet read models.
- Strategic map projections sanitize foreign owned planet visual intensity details until a real visibility/sensor model exists.
- Read-only map visibility service derives civilization-scoped visibility from current persisted ownership and exploration knowledge: owned planets are `Owned`, systems containing owned planets are `Visible`, other planets in ownership-visible systems are `Visible` but not owned, explored systems are `Visible`, explored planets are `Visible`, and unknown systems/planets keep detail fields hidden.
- Strategic map system and planet DTOs now include derived visibility level, reason, and visibility booleans so development clients can distinguish owned, visible, and unknown relevant map nodes without adding persisted fog-of-war.
- Strategic map system and planet DTOs now include read-only command availability metadata for map view/detail and fleet travel/transfer capability hints. These flags are UI-readiness metadata and do not replace existing command validation.
- Strategic map system and planet DTOs now include read-only exploration preview metadata for unknown nodes while blocking preview for already-visible or owned nodes.
- Strategic map command metadata includes `exploration.mission.create` as a UI-readiness hint only when the target is preview-eligible; visible, owned, or revealed targets block it with `ExplorationPreviewUnavailable`, and the create service remains authoritative for validation.
- Strategic map planet DTOs sanitize unknown planets by keeping detail fields null while preserving stable ids, visibility metadata, preview metadata, and command availability.
- Development-only strategic map action manifest at `GET /api/dev/strategic-map/action-manifest` exposes deterministic metadata for current strategic map, visual state, fleet UI state, exploration preview, and related manifest read actions.
- Strategic map action manifest now includes exploration tooling metadata for preview read, mission create, mission complete-due, and knowledge read actions, including routes, methods, mutability, required fields, success statuses, common error statuses, and safety notes.
- Strategic map action manifest includes `exploration.mission.list` metadata for the development-only mission list read, including optional status filtering.
- Development-only exploration preview endpoint at `GET /api/dev/strategic-map/exploration-preview?civilizationId={id}` exposes read-only exploration readiness metadata derived from map visibility.
- Minimal persistent exploration mission foundation exists with `ExplorationMission`, `ExplorationMissionStatus`, EF mapping, and a migration for planned/completed mission lifecycle state. No creation endpoint, completion worker, visibility reveal, sensors, fog-of-war, route graph, pathfinding, combat, interception, or UI behavior has been added.
- Minimal persistent exploration knowledge foundation exists with `ExplorationKnowledge`, `ExplorationKnowledgeSource`, EF mapping, and a migration for civilization-scoped known systems and optional known planets. Mission completion records it, and map visibility consumes it as read-only visibility.
- Development-only exploration mission creation exists at `POST /api/dev/strategic-map/exploration-missions/create`, creating planned missions only for current exploration-preview-eligible unknown targets with deterministic placeholder due times and no resource cost, fleet assignment, completion, visibility reveal, sensors, fog-of-war, route graph, pathfinding, combat, interception, or UI behavior.
- Development-only exploration mission completion exists at `POST /api/dev/strategic-map/exploration-missions/complete-due`, marking due planned missions completed and recording exploration knowledge that read models can expose as visibility without fog-of-war/sensor persistence, rewards, combat, interception, route graph, pathfinding, background worker, or UI behavior.
- Development-only exploration mission list endpoint exists at `GET /api/dev/strategic-map/exploration-missions?civilizationId={id}&status={optional}`, returning read-only, civilization-scoped planned/completed mission rows in deterministic requested/due/id order without creating, completing, cancelling, or mutating missions.
- Development-only exploration knowledge read endpoint exists at `GET /api/dev/strategic-map/exploration-knowledge?civilizationId={id}`, returning ids-only, civilization-scoped exploration knowledge rows in deterministic discovered/system/planet order without mutating missions, knowledge, visibility, resources, fleets, sensors, fog-of-war, route graph, pathfinding, combat, interception, or UI state.
- Exploration mission lifecycle smoke coverage validates preview -> create planned mission -> complete due mission -> record knowledge -> reveal read-model visibility -> strategic map exposure and preview blocking, while ownership remains unassigned, foreign-owned details stay sanitized, and seeded fleet/resource state remains unchanged.
- Exploration tooling readiness smoke coverage validates preview, mission creation, mission list, due completion, knowledge read, map visibility, strategic map reveal, action manifest metadata, resource/fleet non-mutation, and current no-sensors/no-rewards/no-combat/no-route-graph limitations together.
- Read-only sensor profile service derives civilization-scoped placeholder sensor metadata from active owned planets and stationed orbital groups, using deterministic sensor class, range tier, scan strength, and source-kind values without adding persisted sensor state, visibility reveal, scanner mechanics, or gameplay behavior.
- Read-only detection coverage service derives civilization-scoped, conservative local-system coverage metadata from owned planet and stationed scout sensor context, using deterministic source kind, source system/planet ids, coverage class, range tier, confidence, and limitation notes without revealing hidden targets, creating knowledge, or persisting detection state.
- Strategic map metadata now surfaces sensor profile notes and visible local profile summaries for owned planets and stationed orbital groups, while keeping unknown nodes unrevealed and leaving visibility and command validation unchanged.
- Strategic map metadata now also surfaces detection coverage notes plus visible system/planet coverage summaries derived from owned planets and stationed scout groups, while keeping unknown nodes unrevealed and leaving visibility and command validation unchanged.
- Development-only sensor profile read endpoint at `GET /api/dev/strategic-map/sensor-profiles?civilizationId={id}` exposes derived profile rows for tooling, and the strategic map action manifest now includes `sensor.profile.read`.
- Development-only detection coverage read endpoint at `GET /api/dev/strategic-map/detection-coverage?civilizationId={id}` exposes derived coverage rows for tooling, and the strategic map action manifest now includes `detection.coverage.read`.
- Sensor readiness smoke coverage validates sensor profiles, strategic-map sensor metadata, exploration knowledge visibility, unknown target sanitization, manifest metadata, and resource/fleet/knowledge non-mutation together.
- Detection readiness smoke coverage validates sensor profiles, detection coverage, strategic-map detection metadata, exploration-knowledge visibility, action-manifest metadata, unknown-target sanitization, and no-mutation guarantees together.
- Read-only interception opportunity service derives civilization-scoped metadata for active orbital transfers, surfacing own transfers as self-observed/non-hostile items and exposing only conservatively detected foreign transfers with friendly-context readiness blocking when applicable.
- Strategic map transfer overlays and fleet UI active-transfer summaries now surface read-only interception readiness metadata plus top-level notes, while keeping current transfer behavior, visibility rules, and command validation unchanged.
- Development-only interception opportunity endpoint at `GET /api/dev/strategic-map/interception-opportunities?civilizationId={id}` exposes the interception-readiness read model directly for tooling, and manifest metadata now advertises the action from both the strategic-map and fleet manifest surfaces.
- Interception readiness smoke coverage now validates the current path across detection coverage, interception opportunities, strategic map overlays, fleet UI state, and manifest metadata while proving the reads stay conservative and do not mutate gameplay state.
- Minimal alliance readiness foundation exists with `Alliance`, `AllianceMembership`, status/role enums, EF mapping, and a civilization-scoped read-only query service that returns deterministic alliance membership metadata without granting gameplay permissions, shared visibility, diplomacy automation, pacts, trade, espionage, war, combat, chat, or UI behavior.
- Strategic map now surfaces top-level alliance readiness notes plus requesting-civilization alliance membership metadata, while keeping alliance readiness read-only and preserving the current rule that alliances do not add relevance, reveal allied systems/fleets, share visibility, or share sensor/detection/interception data.
- Development-only alliance readiness read endpoint at `GET /api/dev/strategic-map/alliances/readiness?civilizationId={id}` now exposes civilization-scoped alliance metadata directly for tooling, and the strategic map action manifest now includes `alliance.readiness.read`.
- Alliance readiness smoke coverage now validates direct alliance readiness reads, diplomatic contacts, strategic map metadata, manifest discoverability, conservative visibility, and no-mutation guarantees together.
- Minimal alliance pact readiness foundation exists with `AlliancePact`, pact type/status enums, EF mapping, and a civilization-scoped read-only query service that returns deterministic pact metadata for alliances where the requesting civilization has an active membership. It does not add pact gameplay effects, shared visibility, permissions, trade, war, defense execution, espionage, combat, production endpoints, or final UI.
- Strategic map now surfaces top-level alliance pact readiness notes plus requesting-civilization alliance pact metadata, while keeping pact readiness read-only and preserving the current rule that pacts do not add relevance, reveal allied systems or fleets, share visibility, or share sensor, detection, or interception data.
- Development-only alliance pact readiness read endpoint at `GET /api/dev/strategic-map/alliances/pacts/readiness?civilizationId={id}` now exposes civilization-scoped pact metadata directly for tooling, and the strategic map action manifest now includes `alliance.pact.readiness.read`.
- Pre-frontend checkpoint documentation now consolidates the current development-only backend surfaces, read-only versus mutating tooling routes, readiness limitations, and a recommended first frontend slice in `docs/dev/pre-frontend-contract-checkpoint.md`.
- Strategic map readiness smoke coverage validates that strategic map, visual state, fleet UI state, strategic map action manifest, and exploration preview read surfaces remain coherent and do not mutate stockpiles, orbital groups, or transfers.
- Visibility and command readiness smoke coverage validates owned, foreign-owned, and unknown strategic map nodes across visibility and strategic map read models; verifies command availability for visible nodes and blocked commands for unknown nodes; and protects read-only behavior across systems, planets, ownerships, stockpiles, orbital groups, and transfers.
- Static visual sandbox at `/dev/visual-state/index.html`.
- CSS-only pseudo-3D visual sandbox rendering for planet/system preview, overlays, markers, and transfer routes.
- Static sandbox assets are gated behind the same development switch as development APIs.
- `src/VoidEmpires.Frontend` now provides a minimal Vite + React + TypeScript frontend shell with a conservative app layout, route placeholders for strategic map and fleet inspection, backend base URL configuration through `VITE_VOIDEMPIRES_API_BASE_URL`, and explicit development-only warnings.
- The frontend strategic map page now reads `GET /api/dev/strategic-map?civilizationId={id}`, exposes civilization-id driven loading/error/success states, renders system and planet summaries, and surfaces readiness metadata as informational only.
- The frontend strategic map page now also renders a deterministic, read-only SVG 2D map view that normalizes backend system coordinates into a responsive viewport while keeping the existing summary cards below the visual layer.
- The frontend strategic map page now supports deterministic local selection for systems and planets, allowing read-only inspection of visibility, counts, and command/readiness metadata without calling mutating endpoints.
- The frontend strategic map page now also links selected systems and visible planets to the existing visual-state development endpoints, exposing compact renderer-facing summaries plus raw JSON payload inspection without adding rendering or gameplay mutations.
- The frontend fleet page now reads `GET /api/dev/fleets/ui-state`, `GET /api/dev/fleets/action-manifest`, and `GET /api/dev/strategic-map/action-manifest`, rendering fleet summaries and manifest metadata as read-only inspection panels without wiring gameplay mutations.
- Frontend setup, limitations, current visual map behavior, and smoke validation are now documented in `src/VoidEmpires.Frontend/README.md` and `docs/dev/frontend-foundation-smoke-checklist.md`.
- Phase 9K adds a Figma-derived frontend token foundation in `src/VoidEmpires.Frontend/src/styles.css`, including reusable raw palette variables, semantic color tokens, spacing/radius/elevation scales, and reserved shell layout variables aligned to the `Xuniverse UI v1 - Modern Simple` concept without introducing the final UI.
- Frontend Figma alignment guidance is now documented in `src/VoidEmpires.Frontend/README.md` and `docs/dev/frontend-figma-alignment.md` so later UI tasks can reuse the same color and layout vocabulary.
- Phase 9L aligns the frontend shell with the Figma layout language through reusable `components/ui` primitives, a 64px-style top resource bar, a 230px-style sidebar with Figma navigation labels, safe disabled placeholders for non-implemented sections, and an `AppShell` composition that preserves the existing read-only route behavior.

Current foundation chain:

```text
Identity -> PlayerProfile -> Civilization -> PlanetOwnership -> Economy -> Buildings -> Queues -> Assets -> OrbitalGroups -> OrbitalTransfers -> PlanetVisualState -> SystemVisualState -> VisualSandbox -> DevSandboxGating
```

## Visual State Design Note

Accepted current rules:

- `PlanetVisualStateDto` is a read contract, not a persisted gameplay entity.
- `PlanetVisualProfileDto` describes render hints.
- `PlanetVisualProfileCatalog` differentiates visual behavior by `PlanetType`.
- `PlanetVisualIntensityCalculator` derives deterministic normalized intensities from existing game data.
- `PlanetVisualStateService` derives single-planet visual state from persisted game data.
- `SystemVisualStateService` returns ordered planet visual states for a solar system.
- `SystemVisualStateDto` includes identity, coordinates, star metadata, layout hints, orbital group markers, transfer overlays, and planets.
- `OrbitalGroupVisualMarkerDto` and `OrbitalTransferVisualOverlayDto` are renderer-facing read projections, not command models.
- Transfer overlay progress is a read-time visual approximation from departure/arrival timestamps.
- `/dev/visual-state/index.html` is development tooling, not final game UI.
- Static sandbox assets are not served in Production by default.
- Static sandbox assets are served outside Development only when `VoidEmpires:DevEndpoints:Enabled=true`.

Current intentional limitations:

- no Three.js/Babylon.js implementation
- no real WebGL renderer
- no meshes, shaders, textures, or binary render assets from the backend
- no persisted visual customization model
- no route graph/pathfinding model or persisted fuel inventory/refueling model
- no combat/interception overlay model
- no final game UI layout

## Fleet and Transfer Design Notes

Accepted current rules:

- `OrbitalGroup` represents grouped orbital assets.
- Origin and current planet are intentionally separated.
- Local crew/operator capacity is validated during production, not during parking/stationing.
- `OrbitalTransfer` persists transfer intent, timing, status, origin, and destination.
- Creating a transfer reserves the orbital group.
- Completing a due transfer moves the group to the destination and marks the transfer completed.
- Orbital travel estimates are preview-only read models. They calculate distance, duration, and estimated resource costs without creating transfers, reserving groups, charging resources, mutating stockpiles, or persisting estimates.
- Orbital travel estimates report whether the current planet stockpile can afford estimated costs and identify insufficient resources without spending balances.
- Orbital route profiles are read-only metadata derived from abstract distance units. Current bands are intentionally coarse placeholders: distance `1` is `LocalOrbit`, `2-3` is `InnerSystem`, `4-6` is `OuterSystem`, and `7+` is `LongRange`. Current profiles are supported and use a placeholder fuel multiplier of `1.0`; they do not introduce pathfinding, route graphs, fuel inventory, combat, interception, alliances, or espionage.
- Orbital fuel readiness is a placeholder read model derived from asset type, group quantity, abstract distance, and route profile. It reports estimated fuel units required, estimated range units available, readiness, and not-ready reasons, but it does not add persisted fuel state, refueling, spending, or transfer-creation behavior.
- Creating an orbital transfer charges the estimated travel costs from the current planet stockpile before reserving the orbital group and creating the transfer.
- Cancelling an orbital transfer is explicit, persistent, and only available before completion. Phase 6R cancellation marks the transfer cancelled, releases the reserved orbital group back to stationed status at its current persisted planet, and does not refund charged resources.
- An orbital group with an active transfer cannot be split, merged as a source or target, assigned a second transfer, or used for a new travel estimate preview. Active transfer means a transfer that is neither completed nor cancelled. Completion and cancellation remain the valid lifecycle operations for the active transfer itself.
- Splitting an orbital group is available only for stationed groups owned by the requesting civilization. The split quantity must be positive and lower than the source quantity.
- Merging orbital groups requires different stationed groups owned by the requesting civilization, sharing the same current planet and asset type. The target group quantity increases and the source group is removed.
- Fleet operational overview is read-only. It consolidates orbital group state, active transfer timing/status, and command availability without creating transfers, cancelling transfers, completing transfers, splitting, merging, charging resources, or mutating persisted state.
- Fleet lifecycle smoke tests are xUnit tests over EF in-memory services; the repository integration-test script remains a placeholder and reports no configured integration tests.
- Fleet UI state is read-only development tooling for future UI prototypes. It aggregates existing overview data, resource context for group current planets, and action hints without mutating persisted state.
- Fleet UI state route/fuel readiness hints intentionally do not invent a destination. The travel estimate endpoint remains the source of concrete route profile and fuel readiness previews because it requires `destinationPlanetId`.
- Fleet action manifest is read-only development tooling for future UI prototypes. It lists available dev fleet actions and contracts, including route/fuel preview guidance, but does not replace command validation.
- The current sandbox renders markers and transfer route lines as visual indicators only.
- Strategic map read model is read-only backend preparation for future map UI. Relevance includes owned planets, active transfer origin/destination planets, and exploration-known systems for the requesting civilization. Ownership, fleet details, and detailed planet visual intensity signals from other civilizations are not exposed by this read model.
- Map visibility is a derived read-only projection, not persisted fog-of-war. Phase 7I does not add exploration missions, sensors, known-system persistence, espionage, diplomacy, route graphs, pathfinding, combat, interception, or UI. Until a real knowledge model exists, unknown systems and planets are returned with identifiers and `Unknown` visibility while names, coordinates, star type, planet type, size, colonization status, and orbital slot are hidden.
- Strategic map visibility integration remains annotation-only. Phase 7J preserves the existing strategic-map relevance scope and can annotate already-relevant active-transfer destinations as `Unknown`, but it does not make the strategic map endpoint enumerate every unknown persisted system.
- Strategic map command availability is deterministic and read-only. Phase 7K derives availability from visibility and current requesting-civilization fleet context, blocks unknown/not-visible nodes explicitly, and keeps transfer creation routed through existing fleet command validation.
- Phase 7L adds integrated smoke coverage for the current visibility and command readiness contract without adding gameplay behavior or persistence.
- Phase 7M corrected strategic map travel/transfer command availability to use requesting-civilization fleet context from the returned map rather than only the local system being projected.
- Strategic map exploration preview is deterministic and read-only. Unknown nodes can show `exploration.preview` as available; already-visible and owned nodes are blocked as `AlreadyVisible` or `AlreadyOwned`. It does not create exploration missions, sensors, persisted fog-of-war, scanners, espionage, diplomacy, combat, interception, route graph, or pathfinding state.
- Strategic map action manifest is read-only development tooling for future UI prototypes. It lists strategic map, exploration preview, visual-state, fleet UI state, and manifest read actions with method, route, required fields, success status, common error statuses, and notes.
- Strategic map readiness smoke coverage protects the current limitation that map/readiness contracts do not expose mesh, texture, binary, shader, route graph, pathfinding, combat, or interception payload fields.
- Phase 7Q adds only the persistent exploration mission data model: requesting civilization, target system, optional target planet, requested/due/completed timestamps, and planned/completed status. Exploration preview remains read-only and does not create missions or reveal map visibility.
- Phase 7R adds a minimal creation service and dev-only endpoint for planned exploration missions. System-level placeholder missions are due after 30 minutes, planet-level placeholder missions after 45 minutes, and creation is allowed only when the existing exploration preview marks the target as eligible.
- Phase 7S adds a minimal completion service and dev-only endpoint that completes due planned missions by timestamp. Completion currently closes mission lifecycle state only and intentionally does not reveal visibility or create knowledge/fog-of-war persistence.
- Phase 7T adds lifecycle smoke coverage and documentation for the current preview -> create -> complete flow. The expected behavior remains conservative: completed exploration missions do not reveal targets, create known-system/fog-of-war/sensor state, grant rewards, or mutate fleet/resource state.
- Phase 7U adds only the exploration knowledge persistence foundation: civilization, system, optional planet, discovery source, optional source mission, discovery timestamp, and database uniqueness indexes for system-level and planet-level knowledge. It does not change visibility, mission completion, sensors, scanners, fog-of-war behavior, or UI.
- Phase 7V records exploration knowledge when due planned exploration missions complete. System-target missions record system knowledge; planet-target missions record both system and planet knowledge. Map visibility and strategic map reads do not consume this knowledge until Phase 7W.
- Phase 7W integrates exploration knowledge into map visibility and strategic map relevance. Ownership still has priority; explored systems and planets use existing `Visible` visibility with `ExploredSystem` and `ExploredPlanet` reasons; system-level knowledge does not reveal every planet detail.
- Phase 7X hardens the full exploration reveal lifecycle with smoke coverage and documentation. The validated path proves knowledge recording, visibility consumption, conservative strategic-map exposure, blocked preview after reveal, no ownership leakage, and no resource/fleet/reward mutation.
- Phase 7Y hardens strategic map projection sanitization so unknown planets in explored systems do not leak names, planet types, sizes, colonization status, orbital layout, visual scale, or intensity details.
- Phase 7Z adds the exploration knowledge query service and development-only read endpoint so tooling can inspect currently recorded knowledge rows for a civilization. The endpoint stays ids-only and read-only, with deterministic ordering and standard dev-route gating.
- Phase 8A expands strategic map tooling metadata for exploration preview read, mission create, mission complete-due, and knowledge read actions, and adds `exploration.mission.create` command hints to strategic map systems and planets without changing gameplay behavior.
- Phase 8B adds the exploration mission query service and development-only mission list endpoint with optional status filtering, plus manifest/docs coverage for the new read action.
- Phase 8C adds consolidated smoke coverage for the exploration dev tooling lifecycle across preview, mission create/list/complete, knowledge read, visibility, strategic map reveal, manifest metadata, and non-mutation guarantees.
- Phase 8D adds the read-only sensor profile service foundation for future exploration, detection, espionage, and interception systems. Profiles are derived placeholders only, scoped by civilization, and do not persist sensor state or reveal visibility.
- Phase 8E integrates those placeholder sensor summaries into the strategic map read model as metadata-only notes, per-system/per-planet summaries for visible nodes, and fleet-presence summaries where a stationed group has a derived profile.
- Phase 8F adds the development-only sensor profile read endpoint and manifest metadata so tooling can inspect sensor readiness directly without adding production endpoints or gameplay effects.
- Phase 8G hardens the sensor readiness path with smoke coverage across sensor profiles, strategic map metadata, exploration knowledge visibility, manifest metadata, and no-mutation guarantees.
- Phase 8I integrates detection coverage into strategic-map readiness metadata with top-level notes plus visible system/planet summaries, while preserving current visibility and command behavior.
- Phase 8J adds a development-only detection coverage read endpoint plus strategic-map action manifest metadata so tooling can inspect detection readiness directly without adding gameplay behavior.
- Phase 8K hardens the detection readiness path with integrated smoke coverage across sensor profiles, detection coverage, strategic-map metadata, visibility, manifest metadata, and no-mutation guarantees.
- Phase 8L adds the first interception readiness read model. It remains read-only, civilization-scoped, and conservative: own transfers appear only as self-observed metadata, foreign transfers surface only when both endpoints are already visible and current local-system detection coverage exists, friendly interceptor context is derived from stationed requester fleets, and no interception execution, combat, persistence, or visibility reveal is added.
- Phase 8M integrates interception readiness into the existing strategic map and fleet UI read surfaces. Current transfer overlays and active-transfer summaries can now expose self-observed or blocked readiness notes, top-level interception notes explain that execution is unsupported, and hidden foreign transfers remain omitted from these UI/readiness surfaces.
- Phase 8N adds a development-only interception opportunity read endpoint plus manifest metadata for direct tooling discovery. The endpoint follows current development gating and persistence rules, returns read-only readiness rows only, and does not execute interception or reveal hidden transfer state.
- Phase 8O adds end-to-end smoke coverage for the interception readiness stack. The validated path proves self-observed own-transfer metadata, conservative omission of hidden foreign transfer data, manifest discoverability, and no mutation of transfers, fleets, resources, knowledge, or missions.
- Phase 8T adds a minimal alliance readiness foundation: persisted alliance and alliance membership metadata, conservative validation, a deterministic civilization-scoped read query, and focused tests. It intentionally does not add invitations, permissions, shared visibility, pacts, trade, espionage, war, combat, chat, production endpoints, or final UI.
- Phase 8U integrates that alliance readiness foundation into the strategic map as top-level notes plus requesting-civilization membership metadata only. Diplomatic contacts remain separate metadata, and alliance readiness still does not change relevance, visibility, authorization, or allied data exposure.
- Phase 8V adds a development-only alliance readiness read endpoint plus strategic-map action manifest metadata so tooling can inspect alliance readiness directly without adding production endpoints, gameplay effects, shared visibility, permissions, or allied data exposure.
- Phase 8W hardens the full alliance readiness path with smoke coverage across the query service, diplomatic contacts, strategic map, manifest metadata, conservative visibility, and no-mutation guarantees.
- Phase 8X adds the minimal alliance pact readiness domain foundation: persisted pact metadata between alliances, conservative validation, a deterministic civilization-scoped read query for active alliance participants, and focused domain/persistence/query tests. It intentionally does not add pact gameplay effects, visibility sharing, permissions, trade execution, war, defense automation, espionage, combat, production endpoints, or final UI.
- Phase 8Y integrates that alliance pact readiness foundation into the strategic map as top-level notes plus requesting-civilization pact metadata only. Pact readiness still does not change strategic-map relevance, visibility, authorization, allied data exposure, trade behavior, war state, defense behavior, or combat behavior.
- Phase 8Z adds a development-only alliance pact readiness read endpoint plus strategic-map action manifest metadata so tooling can inspect pact readiness directly without adding production endpoints, shared visibility, permissions, trade behavior, war state, defense behavior, espionage, combat, or final UI.
- Phase 9A adds a documentation-first checkpoint for frontend foundation work, consolidating stable dev/backend contracts, current read-only versus mutating surfaces, readiness limitations, non-goals, and a recommended safe first frontend slice without adding production endpoints or gameplay behavior.
- Phase 9B adds the first frontend project foundation under `src/VoidEmpires.Frontend` with Vite, React, TypeScript, route placeholders for the strategic map and fleet prototype views, a shared backend API base URL configuration, and a visible reminder that the shell consumes development-only backend contracts.
- Phase 9C adds the first frontend strategic map read slice: typed strategic map response handling, a read-only query flow keyed by civilization id, map/system summary rendering, and conservative display of readiness metadata without introducing command execution, auth, backend changes, or 3D rendering.
- Phase 9D adds the first frontend fleet inspection slice: typed fleet UI-state and manifest handling, a read-only fleet query flow keyed by civilization id, grouped fleet summaries, resource/interception notes, and manifest panels that clearly mark mutating actions without executing them.
- Phase 9E documents how to install, run, build, and smoke-check the frontend prototype, updates the pre-frontend contract checkpoint with the current frontend foundation status, and keeps the prototype limitations explicit.
- Phase 9G adds the first visual readiness layer for the frontend strategic map: a simple 2D SVG projection that uses backend coordinates, handles empty and single-system maps safely, shows visibility state directly on map nodes, and preserves the existing list/card inspection view.
- Phase 9H adds a read-only selection detail layer for the frontend strategic map so users can choose a system from the map or list, inspect planet metadata, and review readiness notes without executing gameplay actions.
- Phase 9I connects the frontend strategic map selection flow to the existing visual-state development endpoints with read-only system and planet preview loaders, compact payload summaries, and raw JSON inspection for renderer-facing contracts.
- Phase 9J hardens the current frontend visual map slice with updated README guidance, an explicit smoke checklist for 2D map and visual-state preview behavior, and refreshed frontend checkpoint documentation that keeps the UI framed as development-only readiness tooling.
- Phase 9M aligns the current strategic map and fleet prototype screens with the established Figma token layer: the strategic map now uses a staged map panel, compact legend, grouped selection/detail cards, and cleaner visual-state previews, while the fleet page now uses compact fleet cards, active-transfer progress bars, clearer resource-context framing, and explicit read-only manifest presentation without wiring any mutating actions.
- Phase 9N finalizes the current frontend baseline by tracking `src/VoidEmpires.Frontend/package-lock.json`, keeping `node_modules` and `dist` untracked, and updating the README/checkpoint docs so install, build, smoke, and read-only UI expectations stay deterministic after the Figma alignment work.

## Dev Surface Gating Note

The same switch controls development APIs and static sandbox files.

Accepted current rules:

- Development environment serves dev APIs and `/dev/visual-state/*` static files.
- Production environment does not serve `/dev/visual-state/*` by default.
- Production serves those files and development API routes only when `VoidEmpires:DevEndpoints:Enabled=true`.
- The visual sandbox and assets remain development tooling.

## Validation Status

Run from repository root:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Current validated baseline after Phase 21Z:

- backend: `dotnet build --no-restore` succeeded
- tests: `dotnet test --no-build` succeeded with `641` passing tests
- frontend: `npm run build --prefix src/VoidEmpires.Frontend` succeeded
- Manual visual QA for the accepted cross-cockpit demo flow remains a documented seeded-browser pass through `docs/dev/frontend-foundation-smoke-checklist.md` and the cockpit-specific checklists; the Browser runtime was unavailable in this session, so final screenshot-style acceptance is still user-driven.

Current validated cockpit QA seed baseline:

- `POST /api/dev/seeds/apply` supports `minimal-validation`, `cockpit-validation`, `shipyard-validation`, `fleet-validation`, `research-validation`, and `planet-full-validation`.
- `GET /api/dev/seeds/profiles` exposes the current profile catalog for Development-only discovery.
- Standard manual QA should start from the documented seed profiles rather than ad hoc SQL.
- The documented canonical seeded Galaxy QA path is `/galaxy?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`, while `/?...` remains a compatibility alias.
- `cockpit-validation` now restores a non-empty, focusable, read-only Galaxy baseline alongside the accepted Planet, Construction, Research, Ground Army, Shipyard, Fleets, and Defenses cockpit flows.
- Reapplying richer seed profiles after manual QA queue activity is now supported without colliding on persisted queue `Sequence` uniqueness.
- `cockpit-validation` now also seeds meaningful Defenses readiness through a visible `DefenseGrid` on `Aurelia` while keeping defense queue completion and combat behavior out of scope.
- `cockpit-validation` now also seeds meaningful Ground Army readiness through a visible `Barracks`, one deterministic available `PatrolGroup` path, blocked comparison options, and completed planetary training history on `Aurelia` while keeping combat, invasion, and complete-due execution out of scope.

Recent expected coverage includes orbital groups, orbital transfers, workers, visual state services/endpoints, system layout hints, markers, transfer overlays, static sandbox asset serving, overlay sandbox hooks, static sandbox gating behavior, fleet UI state service, fleet action manifest service, the strategic map read model, the strategic map development endpoint, the map visibility read model, exploration preview readiness, the minimal exploration mission lifecycle, the current Planet or Construction cockpit readability baseline, the minimal-validation Research seed readiness path, the development Research UI-state endpoint baseline, the full seeded Research enqueue smoke path through queue refresh, the development Shipyard UI-state endpoint baseline, the scoped Shipyard enqueue endpoint path, and the strengthened minimal-validation Shipyard seed expectations.

## Recommended Next Work

1. Deepen movement only after deciding whether route graphs, pathfinding, persisted fuel inventory, or refueling are required.
2. Consider whether exploration knowledge should later evolve into a richer fog-of-war model with separate known/scanned/sensored states.
3. Add combat/interception foundations only after fleet movement and visibility contracts stabilize.
4. Start a real renderer spike only after the visual state contract remains stable.
5. Keep `XUniversePlanet Generator Variator` as an external/local prototype reference until the renderer/prototype phase needs it.

## Constraints

- do not add gameplay behavior unless a task explicitly requires it
- do not treat template documentation as authoritative if it conflicts with VoidEmpires-specific planning docs
- do not apply migrations automatically to the real database
- avoid login/session endpoints, deployment, combat, alliances, espionage gameplay, and UI complexity until explicit tasks introduce them
- avoid full UI/3D implementation until the required read contracts and development validation endpoints exist
- keep private operational configuration out of version control
