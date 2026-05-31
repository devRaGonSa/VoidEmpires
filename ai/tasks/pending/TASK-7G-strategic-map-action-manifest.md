# TASK-7G

---

id: TASK-7G
title: Add strategic map action manifest
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
* docs
  roadmap_item: "Phase 7G - Strategic map action manifest"
  priority: high

---

## Goal

Add a Development-only strategic map action manifest that describes the current strategic map, visual state, and related read actions in a machine-readable way.

This manifest should help a future map UI prototype discover available read endpoints and contracts without hardcoding every route.

This phase must not add final UI, production endpoints, route graph/pathfinding, combat, interception, alliances, espionage, or heavy render data.

## Context

The project already has:

* fleet action manifest
* fleet UI state endpoint
* strategic map read model
* strategic map dev endpoint
* planet/system visual state endpoints
* visual sandbox documentation

The strategic map needs a similar metadata surface for future UI tooling:

* action key
* display name
* HTTP method
* route
* read-only flag
* required fields
* success status
* common error statuses
* notes
* relationship to visual state and fleet UI state

The manifest must be read-only and deterministic.

## Implementation steps

1. Inspect the existing fleet action manifest implementation and tests.
2. Inspect strategic map endpoint and docs from Phase 7F.
3. Inspect planet/system visual state dev endpoints.
4. Add application contracts for strategic map action manifest, for example:

   * `GetDevStrategicMapActionManifestResult`
   * `DevStrategicMapActionManifestItem`
   * `DevStrategicMapActionFieldDto`
   * `IDevStrategicMapActionManifestService`
5. Add a simple deterministic provider/service.
6. Include current strategic/map-related dev actions:

   * `strategicMap.read`
   * `visual.system.read`
   * `visual.planet.read` if the endpoint exists
   * `fleet.uiState.read`
   * `fleet.actionManifest.read`
   * `strategicMap.actionManifest.read`
7. For each action include:

   * action key
   * display name
   * method
   * route
   * read-only vs mutating
   * required fields
   * success status
   * common error statuses
   * notes
8. Add a Development-only endpoint, suggested route:

   * `GET /api/dev/strategic-map/action-manifest`
9. Endpoint should follow existing dev endpoint gating conventions.
10. Add tests:

* service/manifest includes all expected strategic map actions
* each action has method, route, read-only flag, and required fields
* endpoint is gated outside Development/dev-enabled mode
* endpoint success returns deterministic manifest

11. Update `docs/dev/strategic-map-api-contract.md` to reference the manifest endpoint.
12. Update `docs/dev/fleet-api-contracts.md` only if a cross-link is useful.
13. Update `ai/current-state.md` to document Phase 7G.

## Files to read first

* src/VoidEmpires.Application/Fleets/GetDevFleetActionManifestResult.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetActionManifestService.cs
* src/VoidEmpires.Web/DevFleetActionManifestEndpoints.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Web/DevSystemVisualStateEndpoints.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/*StrategicMap*.cs
* tests/VoidEmpires.Tests/*VisualState*.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Application/StrategicMap/GetDevStrategicMapActionManifestResult.cs
* src/VoidEmpires.Application/StrategicMap/IDevStrategicMapActionManifestService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevStrategicMapActionManifestEndpoints.cs
* src/VoidEmpires.Web/Program.cs or DevEndpointMappings.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestEndpointTests.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md if cross-link is useful
* ai/current-state.md

If a static web-only provider is more consistent than infrastructure registration, use the simplest repository-consistent option.

## Acceptance criteria

* A Development-only strategic map action manifest endpoint exists.
* The manifest lists all current strategic/map-related dev read actions.
* Each manifest action includes method, route, read-only flag, required fields, success status, error statuses, and notes.
* The manifest is deterministic and machine-readable.
* Endpoint gating follows existing dev endpoint conventions.
* Tests cover service/manifest contents and endpoint behavior.
* Docs reference the manifest endpoint.
* `ai/current-state.md` documents Phase 7G.

## Constraints

* Read-only only.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add route graph/pathfinding.
* Do not add combat/interception.
* Do not add alliances or espionage.
* Do not add fuel inventory/refueling.
* Do not add migrations.
* Do not return meshes, textures, binary assets, shader data, or heavy render payloads.
* Keep behavior deterministic and testable.

## Validation

Before completing the task, run:

```powershell
dotnet restore
dotnet build --no-restore
dotnet test --no-build
```

Expected result:

* clean build
* 0 errors
* no new warnings
* all tests passing

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only expected files changed.
4. Commit with a clear message, for example:
   `feat(map): add strategic map action manifest`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
