# TASK-8Z

---

id: TASK-8Z
title: Add alliance pact readiness dev endpoint and manifest metadata
status: pending
type: feature
team: backend
supporting_teams:
  - web
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8Z - Alliance pact readiness dev endpoint and manifest metadata"
priority: high

---

## Goal

Expose alliance pact readiness through a Development-only read endpoint and manifest metadata.

This is dev tooling only. It must not add production endpoints or gameplay effects.

## Context

Phase 8X adds alliance pact readiness query service.
Phase 8Y integrates pact metadata into strategic map.

Now dev tooling should be able to inspect alliance pact readiness directly.

Suggested endpoint:

* `GET /api/dev/strategic-map/alliances/pacts/readiness?civilizationId={id}`

Manifest action:

* `alliance.pact.readiness.read`

## Implementation steps

1. Read alliance pact readiness contracts/service.
2. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/alliances/pacts/readiness?civilizationId={id}`
3. Endpoint behavior:

   * disabled dev routes return `404`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * success returns `200`
4. Add action manifest metadata:

   * `alliance.pact.readiness.read`
   * method GET
   * route `/api/dev/strategic-map/alliances/pacts/readiness`
   * read-only true
   * required field `civilizationId`
   * common status codes
   * notes that pact readiness is metadata only and does not grant shared visibility, permissions, trade, war, defense, espionage, or combat
5. Add tests:

   * endpoint gating
   * endpoint persistence missing
   * endpoint bad request
   * endpoint success
   * manifest includes alliance pact readiness read action
   * endpoint is read-only/no mutation
6. Update `docs/dev/strategic-map-api-contract.md`.
7. Update `ai/current-state.md` to document Phase 8Z.

## Files to read first

* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Web/DevAllianceReadinessEndpoints.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Application/StrategicMap/GetAlliancePactReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AlliancePactReadinessQueryService.cs
* tests/VoidEmpires.Tests/DevAllianceReadinessEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Web/DevAlliancePactReadinessEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* tests/VoidEmpires.Tests/DevAlliancePactReadinessEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Alliance pact readiness dev endpoint exists.
* Endpoint follows current dev gating conventions.
* Endpoint returns read-only pact readiness result.
* Strategic map manifest includes alliance pact readiness read action.
* Docs describe endpoint and limitations.
* Tests cover endpoint and manifest.
* `ai/current-state.md` documents Phase 8Z.

## Constraints

* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not add pact commands.
* Do not add shared visibility.
* Do not add permissions.
* Do not add trade, war, defense/intervention, espionage, or combat.
* Do not mutate resources/fleets/transfers/missions/knowledge/diplomatic contacts/alliances.
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
   `feat(diplomacy): add alliance pact readiness dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

