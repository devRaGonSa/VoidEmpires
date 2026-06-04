# TASK-25O

---
id: TASK-25O
title: Phase 25O - Shipyard resource before and after assertions
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
  - frontend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Determine and prove the real resource behavior of Shipyard enqueue so tests, scripts, and user-facing copy describe the backend truth.

## Current problem

A persisted queue row alone is not enough for QA. The block must know whether Shipyard deducts resources immediately, reserves them, only validates affordability, or mixes those behaviors.

## Context from current implementation

- Construction and Research already proved their immediate-deduction rules.
- Shipyard may share the same underlying spend logic or may apply a distinct asset-production rule.
- The current cockpit copy and later scripts will become misleading if they assume the wrong economic behavior.

## Files to read first

- src/VoidEmpires.Application/Assets/*
- src/VoidEmpires.Domain/Assets/*
- tests/VoidEmpires.Tests/*Shipyard*
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Expected files to modify

- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- tests/VoidEmpires.Tests/*Shipyard*.cs
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Implementation requirements

1. Inspect the current asset-production enqueue and affordability logic to identify the real resource rule.
2. Add tests that capture the actual before and after resource behavior for a successful enqueue.
3. If multiple resource surfaces exist, assert against the authoritative backend read model rather than UI-only derivations.
4. If the behavior is deterministic, assert the exact delta.
5. If the behavior is reserve-based or mixed, assert the exact state transition that the backend already implements.
6. Do not change business rules unless the tests expose a clear bug or contradiction.
7. Update Shipyard QA docs to state the exact backend rule in plain language.
8. If any current UI or script copy implies the wrong rule, correct the copy without expanding feature scope.

## Backend/API requirements

- Tests are required.
- No schema changes.
- Prefer asserting through existing read models and command responses instead of creating new read endpoints.

## Script/QA requirements

- Future Shipyard QA scripts should print before and after resource values when the current read model exposes them.
- Script copy must distinguish between deduction, reservation, or validation-only behavior.

## Safety constraints

- No hidden resource creation or refund logic.
- No manual SQL.
- No destructive reseed or cleanup behavior.

## Acceptance criteria

- The real Shipyard resource rule is covered by tests.
- Docs state the actual backend behavior clearly.
- Any touched UI copy remains aligned with the backend.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If Shipyard uses a different economic rule from Construction and Research, later QA instructions must call that out explicitly to avoid operator confusion.

