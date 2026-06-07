# TASK-29A

---
id: TASK-29A-construction-real-enqueue-contract-audit
title: Construction contract audit before UI enqueue
status: done
type: platform
team: platform
supporting_teams: [frontend, backend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Audit existing backend contract and safe UI boundaries for Construction order enqueue before any mutation code changes are made.

## Context
This task is intentionally docs-only and does not mutate behavior. It must identify exact endpoint details and all rejection branches already proven in backend/QA tooling.

## Files to read first

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/DevConstructionPersistedFlowTests.cs
- scripts/dev-qa-create-construction-order.ps1
- docs/dev/construction-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Expected files to modify

- ai/tasks/pending/TASK-29A-construction-real-enqueue-contract-audit.md
- docs/dev/construction-cockpit-checklist.md

## Implementation steps

1. Verify endpoint path, method, request object, and response object for real persisted enqueue.
2. Confirm Development-only exposure and any environment gate.
3. Validate resource deduction semantics in backend behavior/tests.
4. Enumerate existing rejection cases:
   - insufficient resources
   - unavailable action
   - invalid civilization
   - invalid planet
   - wrong owner
   - existing/open order constraints
5. Add explicit safe boundary notes in construction checklist: what is allowed from UI and what remains blocked.

## Acceptance criteria

- Contract is documented with explicit path and payloads.
- Safe boundary is updated in construction checklist.
- No source behavior changes.

## Audit notes

- `POST /api/dev/buildings/construction-orders/enqueue` is mapped only when Development endpoints are enabled in `src/VoidEmpires.Web/Program.cs`.
- Development exposure is enabled by `environment.IsDevelopment()` or `VoidEmpires:DevEndpoints:Enabled=true`.
- The route also requires a configured `ConnectionStrings:DefaultConnection`; otherwise it returns `503 Service Unavailable`.
- Request contract is `planetId`, `civilizationId`, `action`, `buildingType`, `requestedAtUtc`.
- Success contract is `201 Created` with `succeeded`, `orderId`, `startsAtUtc`, `endsAtUtc`, and `errors`.
- Invalid request shape returns `400 Bad Request`; service-level enqueue rejection returns `409 Conflict`.
- Persisted enqueue spends the full visible construction cost immediately from `PlanetResourceStockpile` before saving the active order.
- Distinct enqueue conflict branches currently proven in service/tests are:
  - `Planet is not owned by the requesting civilization.`
  - `Planet already has an open construction order.`
  - `Insufficient resources.`
  - `Planet building capacity would be exceeded.`
  - `Planet resource stockpile was not found.`
  - `Building was not found.` for upgrade requests against a missing building row.
- There is no distinct persisted enqueue error today for an unknown civilization id or unknown planet id; those cases collapse into request validation or ownership/read-state boundaries instead.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
