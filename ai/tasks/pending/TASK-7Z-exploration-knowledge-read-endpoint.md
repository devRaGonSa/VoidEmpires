# TASK-7Z

---

id: TASK-7Z
title: Add exploration knowledge read endpoint
status: pending
type: feature
team: backend
supporting_teams:
  - application
  - infrastructure
  - web
  - tests
  - docs
roadmap_item: "Phase 7Z - Exploration knowledge read endpoint"
priority: high

---

## Goal

Add a read-only exploration knowledge query service and Development-only endpoint so dev tooling can inspect what a civilization currently knows after exploration reveals.

This is a dev/read model only. It must not create knowledge, complete missions, reveal new visibility, mutate map state, add sensors, add espionage, add diplomacy, add combat, add interception, add route graph/pathfinding, or add final UI.

## Context

The project now has:

* `ExplorationKnowledge` persistence
* mission completion records knowledge
* map visibility consumes knowledge
* strategic map shows revealed targets conservatively

Frontend/dev tooling now needs a direct way to inspect current knowledge rows for a civilization.

Suggested endpoint:

* `GET /api/dev/strategic-map/exploration-knowledge?civilizationId={id}`

The response should be lightweight and deterministic.

## Implementation steps

1. Inspect `ExplorationKnowledge` domain/persistence from Phase 7U.
2. Inspect existing dev endpoint conventions.
3. Add application contracts:

   * `GetExplorationKnowledgeRequest`
   * `GetExplorationKnowledgeResult`
   * `ExplorationKnowledgeDto`
   * `IExplorationKnowledgeQueryService`
4. Implement infrastructure service:

   * validates non-empty civilization id
   * returns knowledge scoped to civilization
   * orders deterministically by discovered timestamp, system id, planet id
   * includes ids, source, source mission id, discovered timestamp
   * optionally includes sanitized system/planet display names only if existing visibility rules make that safe; otherwise ids only
5. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/exploration-knowledge?civilizationId={id}`
6. Endpoint behavior:

   * mapped only in Development or `VoidEmpires:DevEndpoints:Enabled=true`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * success returns `200`
   * disabled dev routes return `404`
7. Add tests:

   * service returns only requesting civilization knowledge
   * service orders deterministically
   * endpoint gating/status behavior
   * endpoint success
   * no mutation occurs
8. Update `docs/dev/strategic-map-api-contract.md`.
9. Update `ai/current-state.md` to document Phase 7Z.

## Files to read first

* src/VoidEmpires.Domain/Exploration/
* src/VoidEmpires.Infrastructure/Persistence/VoidEmpiresDbContext.cs
* src/VoidEmpires.Web/DevExplorationMissionEndpoints.cs
* src/VoidEmpires.Web/DevExplorationActionPreviewEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* tests/VoidEmpires.Tests/*ExplorationKnowledge*.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected, depending on conventions:

* src/VoidEmpires.Application/StrategicMap/GetExplorationKnowledgeRequest.cs
* src/VoidEmpires.Application/StrategicMap/GetExplorationKnowledgeResult.cs
* src/VoidEmpires.Application/StrategicMap/IExplorationKnowledgeQueryService.cs
* src/VoidEmpires.Infrastructure/StrategicMap/ExplorationKnowledgeQueryService.cs
* src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
* src/VoidEmpires.Web/DevExplorationKnowledgeEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* tests/VoidEmpires.Tests/ExplorationKnowledgeQueryServiceTests.cs
* tests/VoidEmpires.Tests/DevExplorationKnowledgeEndpointTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Exploration knowledge query service exists.
* Dev-only read endpoint exists.
* Knowledge is scoped by civilization.
* Results are deterministic.
* Endpoint follows current dev gating conventions.
* Service/endpoint are read-only.
* Tests cover service and endpoint behavior.
* Docs are updated.
* `ai/current-state.md` documents Phase 7Z.

## Constraints

* Do not create or mutate knowledge.
* Do not complete missions.
* Do not reveal new visibility.
* Do not add sensors/scanners.
* Do not add rewards.
* Do not mutate resources/fleets.
* Do not add espionage, diplomacy, combat, or interception.
* Do not add route graph/pathfinding.
* Do not add final UI/frontend code.
* Do not add production endpoints.
* Keep payload lightweight and deterministic.

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
   `feat(exploration): add knowledge read endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.
