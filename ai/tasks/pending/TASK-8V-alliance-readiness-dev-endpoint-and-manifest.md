# TASK-8V

---

id: TASK-8V
title: Add alliance readiness dev endpoint and manifest metadata
status: pending
type: feature
team: backend
supporting_teams:
  - web
  - application
  - infrastructure
  - tests
  - docs
roadmap_item: "Phase 8V - Alliance readiness dev endpoint and manifest metadata"
priority: high

---

## Goal

Expose alliance readiness through a Development-only read endpoint and manifest metadata.

This is dev tooling only. It must not add production endpoints or gameplay effects.

## Context

Phase 8T adds alliance readiness query service.
Phase 8U integrates alliance metadata into strategic map.

Now dev tooling should be able to inspect alliance readiness directly.

Suggested endpoint:

* `GET /api/dev/strategic-map/alliances/readiness?civilizationId={id}`

Manifest action:

* `alliance.readiness.read`

## Implementation steps

1. Read alliance readiness contracts/service.
2. Add Development-only endpoint:

   * `GET /api/dev/strategic-map/alliances/readiness?civilizationId={id}`
3. Endpoint behavior:

   * disabled dev routes return `404`
   * missing persistence returns `503`
   * missing/empty civilization id returns `400`
   * success returns `200`
4. Add action manifest metadata:

   * `alliance.readiness.read`
   * method GET
   * route `/api/dev/strategic-map/alliances/readiness`
   * read-only true
   * required field `civilizationId`
   * common status codes
   * notes that alliance readiness is metadata only and does not grant shared visibility, permissions, pacts, trade, war, espionage, or combat
5. Add tests:

   * endpoint gating
   * endpoint persistence missing
   * endpoint bad request
   * endpoint success
   * manifest includes alliance readiness read action
   * endpoint is read-only/no mutation
6. Update `docs/dev/strategic-map-api-contract.md`.
7. Update `ai/current-state.md` to document Phase 8V.

## Files to read first

* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Web/DevDiplomaticContactEndpoints.cs
* src/VoidEmpires.Web/DevStrategicMapEndpoints.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs
* src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs
* tests/VoidEmpires.Tests/DevDiplomaticContactEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md
* AGENTS.md

## Expected files to modify

Expected:

* src/VoidEmpires.Web/DevAllianceReadinessEndpoints.cs
* src/VoidEmpires.Web/Program.cs
* src/VoidEmpires.Infrastructure/StrategicMap/DevStrategicMapActionManifestService.cs
* tests/VoidEmpires.Tests/DevAllianceReadinessEndpointTests.cs
* tests/VoidEmpires.Tests/DevStrategicMapActionManifestServiceTests.cs
* docs/dev/strategic-map-api-contract.md
* ai/current-state.md

## Acceptance criteria

* Alliance readiness dev endpoint exists.
* Endpoint follows current dev gating conventions.
* Endpoint returns read-only alliance readiness result.
* Strategic map manifest includes alliance readiness read action.
* Docs describe endpoint and limitations.
* Tests cover endpoint and manifest.
* `ai/current-state.md` documents Phase 8V.

## Constraints

* Do not add production endpoints.
* Do not add final UI/frontend code.
* Do not add alliance commands.
* Do not add invitations.
* Do not add shared visibility.
* Do not add permissions.
* Do not add pacts, trade, war, espionage, or combat.
* Do not mutate resources/fleets/transfers/missions/knowledge/diplomatic contacts.
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
   `feat(diplomacy): add alliance readiness dev endpoint`
5. Push the branch to the remote.

The AI Platform runner must push after successful validation.

