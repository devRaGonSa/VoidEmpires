# TASK-8N

---

id: TASK-8N
title: Add interception readiness dev endpoint and manifest metadata
status: pending
type: feature
team: backend
supporting_teams:
  - web
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8N - Interception readiness dev endpoint and manifest metadata"
priority: high

---

## Goal

Expose interception opportunities/readiness through a Development-only read endpoint and manifest metadata.

This is dev tooling only. It must not add production endpoints or gameplay effects.

## Context

Phase 8L adds an interception opportunity service.
Phase 8M integrates interception metadata into strategic map and fleet UI state.

Now dev tooling should be able to inspect interception readiness directly.

Suggested endpoint:

* `GET /api/dev/strategic-map/interception-opportunities?civilizationId={id}`

Manifest action:

* `interception.opportunity.read`

## Implementation steps

1. Read interception opportunity contracts/service.
2. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/interception-opportunities?civilizationId={id}`
3. Endpoint behavior:

   * disabled dev routes return `404`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * success returns `200`
4. Add action manifest metadata:

   * `interception.opportunity.read`
   * method GET
   * route `/api/dev/strategic-map/interception-opportunities`
   * read-only true
   * required field `civilizationId`
   * common status codes
   * notes that opportunities are metadata only and do not execute interception, combat, damage, or interception resolution
5. Add fleet action manifest metadata too if current architecture makes that more discoverable:

   * optional `fleet.interception.readiness.read`
   * read-only true
   * no execution endpoint
6. Add tests:

   * endpoint gating
   * endpoint persistence missing
   * endpoint bad request
   * endpoint success
   * manifest includes interception opportunity action
   * endpoint is read-only/no mutation
7. Update `docs/dev/strategic-map-api-contract.md` and `docs/dev/fleet-api-contracts.md`.
8. Update `ai/current-state.md` to document Phase 8N.

## Files to read first

* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Web/DevDetectionCoverageEndpoints.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetActionManifestService.cs
* src/VoidEmpires.Application/StrategicMap/GetInterceptionOpportunitiesResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/InterceptionOpportunityService.cs
* tests/VoidEmpires.Tests/*Endpoint*.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Web/DevInterceptionOpportunityEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Infrastructure/Fleets/DevFleetActionManifestService.cs if useful
* tests/VoidEmpires.Tests/DevInterceptionOpportunityEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* tests/VoidEmpires.Tests/DevFleetActionManifestServiceTests.cs if fleet manifest changes
* docs/dev/strategic-map-api-contract.md
* docs/dev/fleet-api-contracts.md
* ai/current-state.md

## Acceptance criteria

* Interception opportunity dev endpoint exists.
* Endpoint follows current dev gating conventions.
* Endpoint returns read-only interception opportunity result.
* Strategic map manifest includes interception opportunity read action.
* Fleet manifest includes an interception readiness note/action if consistent.
* Docs describe endpoint and limitations.
* Tests cover endpoint and manifest.
* `ai/current-state.md` documents Phase 8N.

## Constraints

* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not add interception execution.
* Do not add combat, damage, battle result, or interception resolution.
* Do not persist interception state.
* Do not reveal visibility.
* Do not mutate resources/fleets/transfers/missions/knowledge.
* Do not add espionage or diplomacy.
* Do not add route graph/pathfinding.
* Keep endpoint read-only.

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
   `feat(interception): add readiness dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

