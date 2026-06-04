# TASK-26A

---
id: TASK-26A
title: Phase 26A - Final Shipyard and Fleet persisted flow command review
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - backend
  - frontend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: medium
---

## Goal

Review the final scripts and docs for usability, safety, and copy-paste accuracy before closing the block.

## Current problem

The block is meant to give the user practical backend-only QA commands. Even if the underlying tests pass, the final operator experience can still fail if scripts are unclear, defaults are inconsistent, or docs omit expected output and warnings.

## Context from current implementation

- Earlier tasks in the block add or update baseline, Shipyard enqueue, Fleet read-state, and optional allocation instructions.
- Construction and Research already established the pattern of backend-only QA helpers and a shared runbook.
- This final review should check real usability and safety rather than adding new feature scope.

## Files to read first

- scripts/dev-qa-baseline.ps1
- scripts/dev-qa-create-shipyard-production-order.ps1
- scripts/dev-qa-fleet-read-state.ps1
- scripts/dev-qa-create-orbital-group-from-stock.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/development-seed-profiles.md

## Expected files to modify

- scripts/dev-qa-baseline.ps1
- scripts/dev-qa-create-shipyard-production-order.ps1
- scripts/dev-qa-fleet-read-state.ps1
- scripts/dev-qa-create-orbital-group-from-stock.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/development-seed-profiles.md

## Implementation requirements

1. Review the final scripts and docs for consistency rather than building new functionality.
2. Ensure the docs include:
   - backend start command
   - seed apply command
   - Shipyard order command
   - Fleet read-state command
   - optional allocation command only if supported
   - expected success output
   - expected no-op output
   - warning that rows persist
   - no production warning
   - no movement or combat warning
3. Ensure scripts:
   - expose clear parameters
   - default to the documented `cockpit-validation` ids
   - fail clearly when the backend is unavailable
   - print useful operator summaries instead of raw payloads only
4. Limit changes to usability, copy, and safety polish.

## Backend/API requirements

- Do not broaden backend feature scope during this final review.
- If the review exposes a blocking contract issue, fix only the narrow issue needed for the documented command flow.

## Script/QA requirements

- Keep script output mostly Spanish and operator-friendly.
- Do not add destructive cleanup or reset commands.
- Keep the script checker aligned with any final naming or parameter adjustments.

## Safety constraints

- No delete or reset scripts.
- No manual SQL.
- No secrets or environment-specific private values.

## Acceptance criteria

- The backend-only Shipyard and Fleet QA flow is practical and understandable without guesswork.
- Scripts and docs are consistent with each other.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```

## Notes / residual risks

- If optional allocation is excluded, this review should treat that as a valid final state and focus on making the exclusion obvious to the operator.

