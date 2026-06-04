# TASK-25T

---
id: TASK-25T
title: Phase 25T - Dev QA Shipyard production order script
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Add a repeatable PowerShell command that creates one real Shipyard asset-production order against a running Development backend without manual SQL or visual QA.

## Current problem

The user already has backend-only persisted QA commands for Construction and Research. Shipyard needs an equivalent safe command path for real order creation.

## Context from current implementation

- Shared PowerShell helpers already support backend base URL handling, resource formatting, and readable diagnostics.
- Shipyard already has a guarded enqueue endpoint and UI-state contract.
- Reused Development databases may already contain open orders, so the script must treat some no-op outcomes as expected rather than fatal.

## Files to read first

- scripts/dev-qa-common.ps1
- scripts/dev-qa-create-construction-order.ps1
- scripts/dev-qa-create-research-order.ps1
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Web/DevEndpointMappings.cs

## Expected files to modify

- scripts/dev-qa-create-shipyard-production-order.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md

## Implementation requirements

1. Add `scripts/dev-qa-create-shipyard-production-order.ps1`.
2. The script must:
   - require a running backend
   - accept `civilizationId` and `planetId`, defaulting to the documented `cockpit-validation` ids
   - optionally apply the seed profile when an explicit switch is provided
   - fetch Shipyard UI-state before mutation
   - choose the first safe available production option using backend-compatible command metadata
   - print the chosen option and a sanitized payload summary
   - call the enqueue endpoint
   - fetch Shipyard UI-state again
   - print before and after queue counts
   - print before and after resources when available
   - print stock summary when available
3. If there is no available safe action or the current reused database state already has an open queue that blocks the request:
   - print a controlled no-op message
   - do not fail fatally for expected Development states
4. The script must never run migrations, delete data, or auto-complete due production.

## Backend/API requirements

- No backend change is expected unless the current endpoint omits metadata required to create a backend-compatible payload safely.
- If metadata must be added, keep the change narrowly scoped and covered by tests.

## Script/QA requirements

- Must be PowerShell 5.1 compatible.
- Must create at most one order per invocation.
- Must favor Spanish-first operator output with raw DTO details only as secondary diagnostics.
- Must be added to `scripts/check-dev-qa-scripts.ps1`.

## Safety constraints

- Script creates real Development rows, so it must be explicit and conservative.
- No fleet mutation.
- No manual SQL.
- No destructive cleanup path.

## Acceptance criteria

- There is a repeatable backend-only command path for creating one real Shipyard production order.
- Expected reused-database no-op states are handled cleanly.
- Script checks and backend validation pass.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If the enqueue endpoint does not surface enough command metadata from the current UI-state, a narrow backend contract alignment task may be required before the script can be fully reliable.

