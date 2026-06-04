# TASK-25M

---
id: TASK-25M
title: Phase 25M - Shipyard and Fleet persisted flow scope audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Audit the current Shipyard asset-production and Fleet read or allocation surfaces, then define the exact safe persisted QA flow for this block.

## Current problem

Construction and Research already have accepted real persisted QA coverage. Shipyard and Fleets still need the same level of clarity about which Development endpoints are safe to call repeatedly against a reused database and which operations must remain out of scope.

## Context from current implementation

- Shipyard already exposes a cockpit foundation, categorized production options, queue visibility, stock visibility, and a guarded enqueue action.
- Fleets already exposes accepted read and transfer context, but this block must stay focused on read-state and only include allocation if an existing safe path already exists.
- The persisted-flow runbook currently reflects Construction and Research, so later tasks need a precise map of the Shipyard and Fleet endpoints, state transitions, and reused-database risks before implementation begins.

## Files to read first

- AGENTS.md
- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Web/DevEndpointMappings.cs

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/fleet-api-contracts.md
- docs/dev/shipyard-fleet-persisted-qa.md
- tests/VoidEmpires.Tests/*

## Implementation requirements

1. Identify the current Shipyard enqueue endpoint used for real asset-production order creation.
2. Identify the current Shipyard UI-state or read endpoint that exposes queue, stock, resources, and available options.
3. Identify whether due-order completion or processing exists, whether it is global or scoped, and whether it is safe or explicitly unsafe for repeated QA.
4. Identify the current Fleet read endpoints and the exact fields they expose for stationed groups, transfers, and local stock or reserve context.
5. Identify whether a stock-to-orbital-group allocation endpoint already exists, whether it is Development-only, and whether it is safe enough for later inclusion.
6. Confirm that movement, transfer execution, split, merge, combat, interception, and any other mutating Fleet flows remain outside this block unless already documented as safe and explicitly in scope.
7. Document the safe persisted QA sequence for later tasks:
   - apply `cockpit-validation`
   - create one real Shipyard asset-production order if available
   - verify queue, stock, and resource behavior
   - verify Shipyard read-state after enqueue
   - verify Fleet read-state after the Shipyard mutation
   - include stock-to-fleet allocation only if the current endpoint is already safe, scoped, and documented
8. Record the decision in either `docs/dev/persisted-gameplay-flow-checklist.md` or a new dedicated Shipyard/Fleet persisted QA document.

## Backend/API requirements

- Prefer documentation and tests only in this audit task.
- Do not change runtime behavior unless the audit exposes a concrete mismatch between documented and actual safe behavior.
- If additional metadata is missing, note it explicitly for later implementation tasks instead of broadening scope in this audit.

## Script/QA requirements

- Identify the baseline, enqueue, read-state, and optional allocation scripts that should exist by the end of the block.
- Call out any known reused-database no-op states that scripts must treat as expected outcomes.

## Safety constraints

- No destructive reset behavior.
- No manual SQL.
- No hidden auto-completion of production orders.
- No new fleet movement or combat behavior.
- No production auth or production data changes.

## Acceptance criteria

- The safe persisted Shipyard and Fleet QA flow is documented with exact endpoints and expected state transitions.
- The block clearly states what is in scope, what is optional, and what remains excluded.
- Later tasks can implement tests and scripts without rediscovering the boundaries.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- The current Shipyard complete-due route may still be global, which could force this block to stop at enqueue and read-state verification.
- Existing Fleet allocation behavior may consume real stock in a non-idempotent way, so this audit must treat allocation as opt-in only when proven safe.

