# TASK-7C

---

id: TASK-7C
title: Surface route and fuel data in fleet UI state
status: pending
type: feature
team: backend
supporting_teams:

* application
* infrastructure
* web
* tests
* docs
  roadmap_item: "Phase 7C - Route and fuel visibility in fleet UI state"
  priority: high

---

## Goal

Extend the Development-only fleet UI state and fleet action manifest so future UI prototypes can display route profile and fuel readiness information without calling many low-level endpoints manually.

This phase must remain read-only. It must not add route graph/pathfinding, persisted fuel inventory, refueling, combat, interception, alliances, espionage, or final UI.

## Context

Phase 7A added read-only orbital route profile.
Phase 7B added read-only orbital fuel readiness preview.

The current fleet UI state endpoint aggregates fleet screen data:

* groups
* active transfers
* command availability
* resource context
* action hints

Now it should expose enough route/fuel metadata for UI prototypes to present movement readiness and route classification.

This should not replace command validation. It is a UI/dev read model only.

## Implementation steps

1. Inspect Phase 7A route profile contracts and services.
2. Inspect Phase 7B fuel readiness contracts and services.
3. Inspect current `GetDevFleetUiStateResult`, `DevFleetUiStateService`, and endpoint tests.
4. Extend fleet UI state contracts with optional route/fuel preview metadata where appropriate.
5. For each stationary group, expose route/fuel readiness hints only if there is a clear destination context available.

   * If no destination context exists in the current UI state request, expose route/fuel capability metadata and document that concrete route/fuel estimates still require the travel estimate endpoint.
   * Do not invent a destination.
6. Add route/fuel action hints to the action manifest if not already represented.
7. Ensure `/api/dev/fleets/action-manifest` includes route/fuel related fields or notes for:

   * `fleet.travel.estimate`
   * route profile preview endpoint if Phase 7A added one
   * UI state read endpoint
8. Update `/api/dev/fleets/ui-state` response documentation.
9. Add focused tests:

   * UI state includes route/fuel capability metadata or route/fuel notes without mutating state.
   * UI state does not invent a destination-specific estimate when no destination is supplied.
   * action manifest includes route/fuel metadata/notes for travel estimate.
   * endpoint response remains gated and returns successful UI state.
10. Update `docs/dev/fleet-api-contracts.md`.
11. Update `ai/current-state.md` to document Phase 7C.

## Files to read first

* src/VoidEmpires.Application/Fleets/GetDevFleetUiStateResult.cs
* src/VoidEmpires.Application/Fleets/GetDevFleetActionManifestResult.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetActionManifestService.cs
* src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs
* src/VoidEmpires.Web/DevFleetActionManifestEndpoints.cs
* src/VoidEmpires.Application/Fleets/
* src/VoidEmpires.Infrastructure/Fleets/
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/*OrbitalRoute*.cs
* tests/VoidEmpires.Tests/*Fuel*.cs
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on current Phase 7A/7B implementation:

* src/VoidEmpires.Application/Fleets/GetDevFleetUiStateResult.cs
* src/VoidEmpires.Application/Fleets/GetDevFleetActionManifestResult.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetUiStateService.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetActionManifestService.cs
* tests/VoidEmpires.Tests/DevFleetUiStateServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs if it exists
* tests/VoidEmpires.Tests/DevFleetActionManifestEndpointTests.cs if it exists
* docs/dev/fleet-api-contracts.md
* ai/current-state.md

If the route/fuel contracts are better linked through docs/action manifest only, keep code changes minimal and document the decision.

## Acceptance criteria

* Fleet UI state exposes route/fuel readiness capability in a way useful for frontend prototypes.
* UI state does not fabricate destination-specific route/fuel estimates without a destination.
* Action manifest clearly describes how UI should request route profile and fuel readiness.
* Existing travel estimate endpoint remains the source of concrete destination-specific route/fuel estimates unless a dedicated route profile endpoint exists.
* No data is mutated.
* Tests cover the new UI state/manifest behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 7C.

## Constraints

* Read-only only.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add route graph/pathfinding.
* Do not add persisted fuel inventory.
* Do not add refueling.
* Do not charge additional resources.
* Do not change transfer creation behavior.
* Do not add combat, interception, alliances, espionage, or final UI.
* Do not add migrations.
* Keep the change minimal and test-backed.

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
   `feat(dev): surface route fuel readiness in fleet ui state`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
