# TASK-29A

---
id: TASK-29A-construction-real-enqueue-contract-audit
title: Construction contract audit before UI enqueue
status: pending
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

## Validation

- dotnet build --no-restore
- dotnet test --no-build
