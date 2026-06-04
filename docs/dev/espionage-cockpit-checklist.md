# Espionage Cockpit Checklist

This note defines the safe backend scope for `Espionaje v1`.

Current accepted boundary:

- `Espionaje` is an analysis-first read surface built on top of the existing strategic-map readiness stack.
- It may reframe existing visibility, knowledge, sensor, detection, interception-readiness, transfer, and diplomatic-contact metadata.
- It must not imply that spy missions, infiltration, sabotage, theft, or counter-espionage gameplay exists.

## Current backend reality

There is no dedicated espionage backend service, domain module, or production endpoint today.

Existing reusable intelligence inputs already live under the strategic-map and fleet-readiness foundations:

- Map visibility comes from owned planets plus `ExplorationKnowledge` via [src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/MapVisibilityService.cs).
- The consolidated strategic read model is `IStrategicMapService` implemented by [src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs).
- Placeholder sensor metadata comes from [src/VoidEmpires.Infrastructure/StrategicMap/SensorProfileService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/SensorProfileService.cs).
- Placeholder detection coverage comes from [src/VoidEmpires.Infrastructure/StrategicMap/DetectionCoverageService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/DetectionCoverageService.cs).
- Read-only interception readiness comes from [src/VoidEmpires.Infrastructure/StrategicMap/InterceptionOpportunityService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/InterceptionOpportunityService.cs).
- Development seed baseline for the accepted Galaxy story lives in [src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs).
- Development-only routing is exposed from [src/VoidEmpires.Web/Program.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/Program.cs) plus the `Dev*Endpoints` files under `src/VoidEmpires.Web/`.

`ai/current-state.md` remains authoritative that the repository still has `no espionage gameplay` and `no espionage execution`.

## Dependency map

Primary read chain for current intelligence data:

- `Program.cs` -> `MapDevStrategicMapEndpoints`, `MapDevSensorProfileEndpoints`, `MapDevDetectionCoverageEndpoints`, `MapDevInterceptionOpportunityEndpoints`, `MapDevExplorationMissionEndpoints`, `MapDevExplorationKnowledgeEndpoints`
- Dev endpoints -> `Application.StrategicMap` interfaces such as `IStrategicMapService`, `ISensorProfileService`, `IDetectionCoverageService`, `IInterceptionOpportunityService`, `IExplorationMissionQueryService`, `IExplorationKnowledgeQueryService`
- DI registration -> [src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs)
- `StrategicMapService` -> `MapVisibilityService` + `SensorProfileService` + `DetectionCoverageService` + `InterceptionOpportunityService` + `ISystemVisualStateService` + `ExplorationKnowledge` + `OrbitalTransfer` + `DiplomaticContact`
- `InterceptionOpportunityService` -> `IMapVisibilityService` + `IDetectionCoverageService` + `IFleetOperationalOverviewService`
- Seed profiles -> `DevelopmentSeedService` -> owned planet, visible comparison planets, stationed orbital groups, active transfer overlay, and deterministic cockpit QA ids

This means a safe espionage cockpit should assemble from the existing strategic read surfaces, not from a new parallel gameplay subsystem.

## Implemented read-only intelligence inventory

### Already safe to reuse

- Owned systems and owned planets:
  `MapVisibilityService` derives `Owned` / `SystemContainsOwnedPlanet`, and `StrategicMapService` exposes them in the map DTOs.
- Visible systems and visible planets:
  current visibility uses ownership plus recorded `ExplorationKnowledge`; foreign nodes remain sanitized when not visible.
- Partial or unknown nodes:
  unknown systems and planets remain in the strategic projection with stable ids and `Unknown` visibility while hiding detail fields.
- Exploration knowledge:
  `ExplorationKnowledge` is persisted, queryable, and consumed by visibility as conservative read-only reveal.
- Fleet markers:
  `StrategicMapService` reuses system visual markers as `FleetPresence`.
- Transfer visibility:
  `StrategicMapService` reuses transfer overlays for the requesting civilization and annotates them with interception-readiness summaries when available.
- Sensor profile notes:
  `SensorProfileService` derives placeholder rows from owned planets and stationed orbital groups only.
- Detection coverage notes:
  `DetectionCoverageService` derives local-system-only placeholder coverage from sensor profiles.
- Interception-readiness notes:
  `InterceptionOpportunityService` exposes own-transfer observation and conservative foreign-transfer readiness only when endpoints are already visible and locally covered.
- Diplomatic contact metadata:
  `StrategicMapService` exposes read-only contact summaries but explicitly separates them from alliances, pacts, visibility, and espionage behavior.

### Diagnostics-only material for later tasks

- `ResearchType.Espionage` exists in [src/VoidEmpires.Domain/Research/ResearchType.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Research/ResearchType.cs) and `ResearchCatalog`, but it is not a current espionage gameplay contract.
- DTO and action keys such as `exploration.preview`, `exploration.mission.create`, `sensor.profile.read`, `detection.coverage.read`, and `interception.opportunity.read` are tooling metadata, not final espionage UX language.
- `SensorProfileSourceKind.ExplorationKnowledge` exists as an enum slot, but the current sensor service does not emit profiles from exploration knowledge.

### Not implemented and out of scope

- spy mission creation
- spy mission execution
- sabotage
- infiltration
- theft of resources or technology
- counter-espionage execution
- hidden-target reveal from sensors alone
- foreign-transfer reveal without existing visibility plus local detection coverage
- persisted fog-of-war, scanner sweeps, or confidence simulation
- combat or interception execution

## What stays in Galaxy vs Espionage

Keep in `Galaxy` because it is map-first:

- system navigation
- visible node selection
- visual-state previews
- fleet markers as map overlays
- transfer overlays as route overlays
- command availability hints for fleet travel and exploration preview

Allow in `Espionaje` because it is analysis-first:

- selected civilization intelligence summary
- categorized lists of owned, visible, explored, and still-unknown targets
- explanation of why a target is known: ownership, explored system, explored planet, local detection, or self-observed transfer
- passive sensor profile summaries
- passive detection coverage summaries
- passive interception-readiness summaries
- passive diplomatic-contact summaries when framed as situational awareness only

Rule of thumb:

- if the value answers "where is this on the map and what can I click next?", it belongs to `Galaxy`
- if the value answers "what do we currently know, how strong is that knowledge, and what remains uncertain?", it can be reframed in `Espionaje`

## Safe `Espionaje v1` scope

- Accept `civilizationId` as the primary context.
- Reuse current development-only strategic read contracts; do not add a production espionage endpoint.
- Show owned, visible, explored, and unknown targets using existing visibility fields and reasons.
- Surface confidence honestly through current notes, not invented percentages:
  owned = strongest current knowledge,
  visible via ownership/exploration = readable but still limited,
  unknown = ids only or omitted details,
  sensor/detection/interception metadata = advisory only.
- Show passive signals only when they already exist in current read models or seed data.
- If future mission affordances are shown, they must be disabled placeholders with explicit copy that execution is unavailable.

Recommended first backend source for an espionage cockpit:

- `GET /api/dev/strategic-map`
- `GET /api/dev/strategic-map/exploration-knowledge`
- `GET /api/dev/strategic-map/sensor-profiles`
- `GET /api/dev/strategic-map/detection-coverage`
- `GET /api/dev/strategic-map/interception-opportunities`

## Language guardrails

- Prefer `known`, `visible`, `observed`, `partial`, `uncertain`, `coverage`, and `readiness`.
- Avoid copy that implies active agents, infiltration, stolen data, covert action, or guaranteed detection.
- Treat raw enums, DTO names, ids, and manifest action keys as diagnostics-only content.

## Seed and validation notes

- `minimal-validation` and `cockpit-validation` already seed the current strategic story with `Aurelia`, visible comparison planets, stationed groups, and one planned transfer.
- `tests/VoidEmpires.Tests/StrategicMapServiceTests.cs`, `StrategicMapReadinessSmokeTests.cs`, `DetectionReadinessSmokeTests.cs`, `InterceptionReadinessSmokeTests.cs`, `ExplorationToolingReadinessSmokeTests.cs`, and `DevelopmentSeedServiceTests.cs` are the most relevant regression coverage for this scope.
- No integration tests configured.

## Non-goals

- No production espionage API.
- No new espionage persistence model.
- No mutation from the espionage cockpit.
- No duplicate visibility model separate from strategic map.
- No fabricated certainty beyond what current DTOs already support.
