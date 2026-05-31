# TASK-7F

---

id: TASK-7F
title: Add strategic map dev endpoint and documentation
status: done
type: feature
team: backend
supporting_teams:

* web
* tests
* docs
  roadmap_item: "Phase 7F - Strategic map dev endpoint and docs"
  priority: high

---

## Goal

Expose the strategic map read model through a Development-only endpoint and document the contract for future UI/frontend work.

This phase must not add final UI, production endpoints, route graph pathfinding, combat, interception, alliances, espionage, or heavy render data.

## Context

TASK-7E adds the strategic map read model.

This task makes it accessible to dev tooling and documents the contract so the future UI can consume it.

The endpoint should be development-only and follow existing gating conventions.

## Implementation steps

1. Read TASK-7E implementation.
2. Inspect existing dev endpoint mapping/gating conventions.
3. Add a Development-only endpoint, suggested route:

   * `GET /api/dev/strategic-map?civilizationId={id}`
4. Endpoint should follow existing dev endpoint conventions:

   * mapped only in Development or `VoidEmpires:DevEndpoints:Enabled=true`
   * disabled route returns `404`
   * missing persistence returns `503` if the service requires persistence
   * invalid request returns `400`
   * success returns `200`
5. Add endpoint tests:

   * endpoint not exposed outside Development/dev-enabled mode
   * endpoint returns `503` when persistence is missing
   * endpoint returns `400` for missing/empty civilization id
   * endpoint returns `200` for valid request
   * endpoint excludes other civilizations' data according to TASK-7E rules
6. Add documentation under `docs/dev/`, suggested file:

   * `docs/dev/strategic-map-api-contract.md`
7. Documentation should include:

   * route and method
   * dev gating behavior
   * request fields
   * response fields
   * side effects: none/read-only
   * limitations: no route graph, no pathfinding, no combat, no heavy render data, no final UI
   * relationship to visual state and fleet UI state
8. Update `docs/dev/fleet-api-contracts.md` only if it should link to the strategic map endpoint.
9. Update `ai/current-state.md` to document Phase 7F.

## Files to read first

* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Web/DevEndpointMappings.cs
* src/VoidEmpires.Web/DevFleetUiStateEndpoints.cs
* src/VoidEmpires.Web/DevSystemVisualStateEndpoints.cs
* src/VoidEmpires.Application/StrategicMap/
* src/VoidEmpires.Infrastructure/StrategicMap/
* tests/VoidEmpires.Tests/*Endpoint*.cs
* tests/VoidEmpires.Tests/StrategicMapServiceTests.cs
* docs/dev/
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on repository conventions:

* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Web/Program.cs or DevEndpointMappings.cs
* tests/VoidEmpires.Tests/DevStrategicMapEndpointTests.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md if cross-link is useful
* ai/current-state.md

## Acceptance criteria

* Development-only strategic map endpoint exists.
* Endpoint follows current dev gating conventions.
* Endpoint returns map read model from TASK-7E.
* Endpoint tests cover gating, persistence missing, invalid request, success, and scoping.
* Contract documentation exists.
* Docs state that endpoint is read-only and does not return heavy render data.
* `ai/current-state.md` documents Phase 7F.

## Constraints

* Read-only only.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Do not add route graph/pathfinding.
* Do not add combat/interception.
* Do not add alliances or espionage.
* Do not add fuel inventory/refueling.
* Do not add migrations unless absolutely required; if a migration appears necessary, stop and create a follow-up task.
* Do not return meshes, textures, binary assets, or shader data.
* Keep endpoint payload lightweight and deterministic.

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
   `feat(map): add strategic map dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
