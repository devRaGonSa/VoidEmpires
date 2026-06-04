# TASK-25U

---
id: TASK-25U
title: Phase 25U - Dev QA Fleet read-state script
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

Add a repeatable PowerShell command that inspects Fleet read-state after seed application or Shipyard mutations without mutating Fleet gameplay state.

## Current problem

The block needs a backend-only verification path showing that Fleet read-state still works after real Shipyard orders and reused Development database activity.

## Context from current implementation

- Fleets is already an accepted cockpit foundation with a Development UI-state read model.
- This block must keep Fleets focused on inspection, not on movement or command execution.
- The later runbook should let an operator verify the Fleet state quickly without opening the frontend.

## Files to read first

- scripts/dev-qa-common.ps1
- docs/dev/fleet-api-contracts.md
- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/DevFleetUiStateEndpointTests.cs

## Expected files to modify

- scripts/dev-qa-fleet-read-state.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/fleet-api-contracts.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Implementation requirements

1. Add `scripts/dev-qa-fleet-read-state.ps1`.
2. The script must:
   - require a running backend
   - accept `civilizationId` with the documented `cockpit-validation` default
   - accept `planetId` only if the Fleet endpoint uses it
   - fetch Fleet UI-state or the documented Fleet read endpoint
   - print orbital group or squad count
   - print stationed count
   - print active transfer count
   - print local reserve or stock context when available
   - print route or transfer summaries when available
3. If the endpoint returns an empty but valid read-state, print a controlled explanation instead of failing.
4. Do not create transfers, groups, split, merge, cancel, or complete anything.

## Backend/API requirements

- No backend changes are expected unless the script discovers a narrow contract mismatch or an undocumented required parameter.
- Prefer adapting the script to the documented read model instead of widening the endpoint.

## Script/QA requirements

- Must be PowerShell 5.1 compatible.
- Must remain strictly non-mutating.
- Must be included in the parser and helper checks.
- Output should stay mostly Spanish and readable for manual QA use.

## Safety constraints

- No movement.
- No transfer mutation.
- No group creation.
- No manual SQL.

## Acceptance criteria

- Operators have a repeatable backend-only command for Fleet read-state inspection.
- The script is non-mutating and resilient to empty-state responses.
- Validation passes.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If multiple Fleet read endpoints exist, this task should standardize on the one already documented for the cockpit rather than creating a parallel helper-only path.

